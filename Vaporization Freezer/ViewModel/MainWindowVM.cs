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
        public NoConditionCMD FreezeBtn { get; private set; }
        public NoConditionCMD RepeatAlarmBtn { get; private set; }
        //1 tick == 30min
        public double Tick
        {
            get
            {
                //TODO: Fix CoerceValueCallback
                return tick + 1;
            }

            set
            {
                tick = value;
                OnPropertyChanged("Tick");
            }
        }
        public double TimePerTick
        {
            get
            {
                //TODO: Fix CoerceValueCallback
#if DEBUG
                return (timePerTick + 1) * (Int32)Application.Current.Resources["TickPerTime_DEBUG"];
#else
                return (timePerTick + 1) * (Int32)Application.Current.Resources["TickPerTime"];
#endif
            }

            set
            {
                timePerTick = value;
                OnPropertyChanged("TimePerTick");
            }
        }
        public Visibility CoverVisiblity
        {
            get
            {
                return coverVisiblity;
            }
            private set
            {
                coverVisiblity = value;
                OnPropertyChanged("CoverVisiblity");
            }
        }
        public Visibility CoverBtnVisibility
        {
            get
            {
                return coverBtnVisibility;
            }
            private set
            {
                coverBtnVisibility = value;
                OnPropertyChanged("CoverBtnVisibility");
            }
        }
        public bool StrongSwitch
        {
            get
            {
                return Blocker.IsStrong;
            }

            set
            {
                Blocker.IsStrong = value;
                OnPropertyChanged("StrongSwitch");
            }
        }
        public bool LogOnScrSwitch
        {
            get
            {
                return Blocker.LogOnScreen;
            }

            set
            {
                Blocker.LogOnScreen = value;
                OnPropertyChanged("LockScr");
            }
        }
        public bool RepeatAlarm
        {
            get
            {
                return Blocker.RepeatAlarm;
            }

            set
            {
                Blocker.RepeatAlarm = value;
                OnPropertyChanged("RepeatAlarm");
            }
        }

        private double tick;
        private double timePerTick;
        private Visibility coverVisiblity;
        private Visibility coverBtnVisibility;

        public MainWindowVM()
        {
            CoverVisiblity = Visibility.Collapsed;
            FreezeBtn = new NoConditionCMD(pressFreezeBtn);
            RepeatAlarmBtn = new NoConditionCMD(pressConfirmBtn);
            Blocker.AlarmOverCallBack = TimeOverCallBack;
            Blocker.RepeatOverCallBack = () => CoverVisiblity = Visibility.Collapsed;

            if (Properties.Settings.Default.IsRunning && Properties.Settings.Default.IsStrong)
            {
                Tick = Properties.Settings.Default.InitialedTick;
                Blocker.StartBlocking(Properties.Settings.Default.OverTime.Subtract(DateTime.Now).TotalMilliseconds);
            }
        }

        private void TimeOverCallBack()
        {
            if (RepeatAlarm && LogOnScrSwitch)
            {
                CoverBtnVisibility = Visibility.Visible;
            }
            else
            {
                CoverVisiblity = Visibility.Collapsed;
            }
        }

        private void pressFreezeBtn(Object obj)
        {
            Blocker.StartBlocking(Tick * TimePerTick * 60000);
            CoverVisiblity = Visibility.Visible;
            CoverBtnVisibility = Visibility.Collapsed;
        }

        private void pressConfirmBtn(Object obj)
        {
            Blocker.TurnOffRepeatAlarm();
            CoverVisiblity = Visibility.Collapsed;
        }
    }
}
