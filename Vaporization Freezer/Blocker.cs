using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Media;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace VF
{
    static class Blocker
    {
        static public Action AlarmOverCallBack { get; set; }
        static public Action RepeatOverCallBack { get; set; }
        static public bool IsRinging { get; private set; } = false;

        [DllImport("user32.dll")]
        private static extern void LockWorkStation();

        static private Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

        private static bool logonScreen;
        private static bool blockScreen;
        private static bool repeatAlarm;
        private static bool isStrong;
        private static System.Timers.Timer blockTimer = new System.Timers.Timer();
        private static System.Timers.Timer blockRepeater = new System.Timers.Timer(100);
        static private SoundPlayer freezingSfx = new SoundPlayer(Properties.Resources.freezing_sfx);
        static private SoundPlayer iceBreakSfx = new SoundPlayer(Properties.Resources.ice_destruction_sfx);
        static private SoundPlayer iceCrackSfx = new SoundPlayer(Properties.Resources.ice_crack);

        static public bool LogOnScreen
        {
            get
            {
                return logonScreen;
            }
            set
            {
                blockScreen = !value;
                logonScreen = value;
                Properties.Settings.Default.IsLogOnScreen = logonScreen;
                Properties.Settings.Default.Save();
            }
        }
        static public bool BlockScreen
        {
            get
            {
                return blockScreen;
            }
            set
            {
                logonScreen = !value;
                blockScreen = value;
                Properties.Settings.Default.IsLogOnScreen = logonScreen;
                Properties.Settings.Default.Save();
            }
        }
        static public bool RepeatAlarm
        {
            get
            {
                return repeatAlarm;
            }
            set 
            {
                repeatAlarm = value;
                Properties.Settings.Default.RepeatAlarm = repeatAlarm;
                Properties.Settings.Default.Save();
            }
        }
        static public bool IsStrong
        {
            get
            {
                return isStrong;
            }
            set
            {
                isStrong = value;
                Properties.Settings.Default.IsStrong = isStrong;
                Properties.Settings.Default.Save();
            }
        }

        static Blocker()
        {
            LogOnScreen = Properties.Settings.Default.IsLogOnScreen;
            RepeatAlarm = Properties.Settings.Default.RepeatAlarm;
            IsStrong = Properties.Settings.Default.IsStrong;

            blockTimer.Elapsed += (sender, e) => BlockOver();
            blockTimer.AutoReset = false;

            blockRepeater.Elapsed += (sender, e) => LockWorkStation();
            blockRepeater.AutoReset = true;
        }

        public static void StartBlocking(double millisec)
        {

            if (millisec <= 0)
            {
                BlockOver();
                return;
            }

            blockTimer.Interval = millisec;
            DateTime overTime = DateTime.Now.AddMilliseconds(millisec);

            if (IsStrong && !Properties.Settings.Default.IsRunning)
            {
                Properties.Settings.Default.OverTime = overTime;
                Properties.Settings.Default.IsRunning = true;
                Properties.Settings.Default.Save();

                key.SetValue("Vaporization Freezer", Process.GetCurrentProcess().MainModule.FileName);
            }

            
            if(LogOnScreen)
            {
                blockRepeater.Start();
            }
            else if(BlockScreen)
            {
                Window blockScreen = new BlockScreen(overTime);
                blockScreen.Show();
            }


            blockTimer.Start();
            freezingSfx.Play();
        }

        public static void BlockOver()
        {
            if (LogOnScreen)
            {
                blockRepeater.Stop();
            }

            blockTimer.Stop();
            Properties.Settings.Default.IsRunning = false;
            Properties.Settings.Default.Save();
            key.DeleteValue("Vaporization Freezer", false);

            if (RepeatAlarm)
            {
                iceCrackSfx.PlayLooping();
                IsRinging = true;
            }

            AlarmOverCallBack?.Invoke();
        }

        public static bool IsBlocked()
        {
            return blockTimer.Enabled;
        }

        public static void TurnOffRepeatAlarm()
        {
            IsRinging = false;
            RepeatOverCallBack?.Invoke();
            iceCrackSfx.Stop();
            iceBreakSfx.Play();
        }
    }
}
