using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VF.ViewModel.Command;
using System.Runtime.InteropServices;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Media;
using System.Diagnostics;

namespace VF.ViewModel
{
    class MainWindowVM : BaseViewModel
    {
        [DllImport("user32.dll")]
        public static extern void LockWorkStation();

        static private Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

        private string minutes;
        private static System.Timers.Timer blockTimer = new System.Timers.Timer();
        private static System.Timers.Timer blockRepeater = new System.Timers.Timer(100);
        static private SoundPlayer freezingSfx = new SoundPlayer(Properties.Resources.freezing_sfx);
        static private SoundPlayer iceBreakSfx = new SoundPlayer(Properties.Resources.ice_destruction_sfx);
        public NoConditionCMD freezeBtn { get; private set; }
        public string Minutes { get { return minutes; } set { minutes = value; OnPropertyChanged("Minutes"); } }
        public Visibility BlockScrSwitch
        {
            get
            {
                if (blockTimer.Enabled)
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Collapsed;
                }
            }
            private set { }
        }
        public bool StrongSwitch
        {
            get
            {
                return Properties.Settings.Default.IsStrong;
            }

            set
            {
                Properties.Settings.Default.IsStrong = value;
                Properties.Settings.Default.Save();
                OnPropertyChanged("StrongSwitch");
            }
        }

        public MainWindowVM()
        {
            freezeBtn = new NoConditionCMD(freeze);
            blockTimer.Elapsed += (sender, e) => blockOver();
            blockTimer.AutoReset = false;

            blockRepeater.Elapsed += (sender, e) => LockWorkStation();
            blockRepeater.AutoReset = true;

            if(Properties.Settings.Default.IsRunning)
            {
                startBlocking();
            }
        }

        private void startBlocking()
        {
            if(Properties.Settings.Default.IsRunning && Properties.Settings.Default.IsStrong)
            {
                Minutes = Properties.Settings.Default.InitialedTime;
                double remainedTime = Properties.Settings.Default.OverTime.Subtract(DateTime.Now).TotalMilliseconds;

                if(remainedTime <= 0)
                {
                    blockOver();
                    return;
                }
                else
                {
                    blockTimer.Interval = remainedTime;
                }
            }

            if (string.IsNullOrEmpty(Minutes))
            {
                SystemSounds.Hand.Play();
                System.Windows.MessageBox.Show("Please input value");
            }
            else
            {
                if (Properties.Settings.Default.IsStrong && !Properties.Settings.Default.IsRunning)
                {
                    double min = Convert.ToDouble(Minutes);

                    Properties.Settings.Default.InitialedTime = Minutes;
                    Properties.Settings.Default.OverTime = DateTime.Now.AddMinutes(min);
                    Properties.Settings.Default.IsRunning = true;
                    Properties.Settings.Default.Save();

                    blockTimer.Interval = min * 60000;

                    key.SetValue("Vaporization Freezer", Process.GetCurrentProcess().MainModule.FileName);
                }

                blockTimer.Start();
                blockRepeater.Start();
                freezingSfx.Play();
                OnPropertyChanged("BlockScrSwitch");
            }
        }

        private void blockOver()
        {
            blockTimer.Stop();
            blockRepeater.Stop();
            Properties.Settings.Default.IsRunning = false;
            Properties.Settings.Default.Save();
            key.DeleteValue("Vaporization Freezer", false);
            OnPropertyChanged("BlockScrSwitch");
            iceBreakSfx.Play();
        }

        private void freeze(object obj)
        {
            startBlocking();
        }
    }
}
