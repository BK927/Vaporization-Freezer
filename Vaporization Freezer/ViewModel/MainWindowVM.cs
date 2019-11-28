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

namespace VF.ViewModel
{
    class MainWindowVM : BaseViewModel
    {
        [DllImport("user32.dll")]
        public static extern void LockWorkStation();
        public NoConditionCMD freezeBtn { get; private set; }
        public string Minutes { get; set; }
        public Visibility BlockScrSwitch
        { 
            get 
            {
                if(blockTimer.Enabled)
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Collapsed;
                }
            }
            private set{ }
        }

        private static System.Timers.Timer blockTimer = new System.Timers.Timer();
        private static System.Timers.Timer blockRepeater = new System.Timers.Timer(100);
        static private SoundPlayer freezingSfx = new SoundPlayer(Properties.Resources.freezing_sfx);
        static private SoundPlayer iceBreakSfx = new SoundPlayer(Properties.Resources.ice_destruction_sfx);

        public MainWindowVM()
        {
            freezeBtn = new NoConditionCMD(freeze);
            blockTimer.Elapsed += (sender, e) => timerOverEvent(sender, e, OnPropertyChanged);
            blockTimer.AutoReset = false;

            blockRepeater.Elapsed += (sender, e) => LockWorkStation();
            blockRepeater.AutoReset = true;
        }

        private void freeze(object obj)
        {
            if(string.IsNullOrEmpty(Minutes))
            {
                System.Windows.MessageBox.Show("Please input value");
            }
            else
            {
                blockTimer.Interval = Convert.ToDouble(Minutes) * 60000;
                blockTimer.Start();
                blockRepeater.Start();
                freezingSfx.Play();
                OnPropertyChanged("BlockScrSwitch");
            }
        }

        private static void timerOverEvent(Object source, ElapsedEventArgs e, Action<string> onPropertyChanged)
        {
            blockTimer.Stop();
            blockRepeater.Stop();
            onPropertyChanged("BlockScrSwitch");
            iceBreakSfx.Play();
        }
    }
}
