using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using VF.ViewModel.Command;

namespace VF.ViewModel
{
    class BlockScrVM : BaseViewModel
    {
        public string ClockStr { get; private set; }
        public bool IsClickable { get; private set; }
        public RelayCommand<ICloseable> CloseWindowCommand { get; private set; }
        public NoConditionCMD DoNothing { get; private set; }
        private System.Timers.Timer oneMinTimer = new System.Timers.Timer(1000);
        private TimeSpan displayedTime;
        private bool closingAllowed = false;

        public BlockScrVM(DateTime overTime)
        {
            CloseWindowCommand = new RelayCommand<ICloseable>(this.CloseWindow);

#if DEBUG
            DoNothing = new NoConditionCMD((obj) => { System.Windows.MessageBox.Show("DoNothing Called"); });
#else
            DoNothing = new NoConditionCMD((obj) => { });
#endif
            this.displayedTime = overTime.Subtract(DateTime.Now).Add(new TimeSpan(0, 1, 0));
            IsClickable = false;
            oneMinTimer.Elapsed += OnTimedEvent;
            oneMinTimer.AutoReset = true;
            oneMinTimer.Start();

            ClockStr = ((int)displayedTime.TotalHours).ToString("D2") + " : " + ((int)displayedTime.Minutes).ToString("D2");
            OnPropertyChanged("ClockStr");

            Blocker.AlarmOverCallBack += blockerCallback;
        }
        ~BlockScrVM()
        {
            Blocker.AlarmOverCallBack -= blockerCallback;
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

        private void OnTimedEvent(Object source, System.Timers.ElapsedEventArgs e)
        {
#if DEBUG
            ClockStr = ((int)displayedTime.TotalHours).ToString("D2") + " : " + ((int)displayedTime.Minutes).ToString("D2") + " : " + ((int)displayedTime.Seconds).ToString("D2");
#else
            ClockStr = ((int)displayedTime.TotalHours).ToString("D2") + " : " + ((int)displayedTime.Minutes).ToString("D2");
#endif

            OnPropertyChanged("ClockStr");

            this.displayedTime = this.displayedTime.Subtract(new TimeSpan(0, 0, 1));

            if(this.displayedTime.Minutes <= 0)
            {
                this.oneMinTimer.Stop();
            }
        }

        private void blockerCallback()
        {
            if (Properties.Settings.Default.RepeatAlarm)
            {
                IsClickable = true;
                OnPropertyChanged("IsClickable");
            }
            else
            {
                closingAllowed = true;

                Application.Current.Dispatcher.Invoke(() =>
                {
                    foreach (Window window in Application.Current.Windows)
                    {
                        if (window is BlockScreen)
                        {
                            window.Close();
                            break;
                        }
                    }
                });
            }
        }

        private void CloseWindow(ICloseable window)
        {
            Blocker.TurnOffRepeatAlarm();
            closingAllowed = true;
            if (window != null)
            {
                window.Close();
            }
        }
    }
}
