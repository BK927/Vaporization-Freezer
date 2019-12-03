using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using VF.ViewModel.Command;

namespace VF.ViewModel
{
    class BlockScrVM : BaseViewModel
    {
        public string ClockStr { get; private set; }
        public bool IsClickable { get; private set; }
        public NoConditionCMD StopAlarm { get; private set; }
        public NoConditionCMD DoNothing { get; private set; }
        private System.Timers.Timer oneMinTimer = new System.Timers.Timer(1000);
        private TimeSpan remainedTime;
        private bool closingAllowed = false;

        public BlockScrVM(DateTime overTime)
        {
            DoNothing = new NoConditionCMD((obj) => { });
            StopAlarm = new NoConditionCMD(stopAlarm);
            this.remainedTime = overTime.Subtract(DateTime.Now);
            IsClickable = false;
            oneMinTimer.Elapsed += OnTimedEvent;
            oneMinTimer.AutoReset = true;
            oneMinTimer.Start();

            TimeSpan displayedTime = remainedTime.Add(new TimeSpan(0, 1, 0));
            ClockStr = ((int)displayedTime.TotalHours).ToString("D2") + " : " + ((int)displayedTime.Minutes).ToString("D2");
            OnPropertyChanged("ClockStr");
        }

        private void OnTimedEvent(Object source, System.Timers.ElapsedEventArgs e)
        {
            TimeSpan displayedTime = remainedTime.Add(new TimeSpan(0, 1, 0));
            ClockStr = ((int)displayedTime.TotalHours).ToString("D2") + " : " + ((int)displayedTime.Minutes).ToString("D2");
            OnPropertyChanged("ClockStr");

            if(remainedTime.TotalSeconds <= 0)
            {
                oneMinTimer.Stop();

                if(Properties.Settings.Default.RepeatAlarm)
                {
                    IsClickable = true;
                    OnPropertyChanged("IsClickable");
                }
                else
                {
                    foreach(Window window in Application.Current.Windows)
                    {
                        if(window is BlockScreen)
                        {
                            window.Close();
                            break;
                        }
                    }
                }
            }
            remainedTime = remainedTime.Subtract(new TimeSpan(0,0,1));
        }

        private void stopAlarm(Object win)
        {
            Blocker.TurnOffRepeatAlarm();
            closingAllowed = true;
            (win as Window).Close();
        }

        public void OnDeactivated(object sender, EventArgs e)
        {
            (sender as Window).Activate();
        }

        public void OnWindowClosing(object sender, CancelEventArgs e)
        {
            if(Blocker.IsBlocked() || !closingAllowed)
            {
                e.Cancel = true;
            }
        }
    }
}
