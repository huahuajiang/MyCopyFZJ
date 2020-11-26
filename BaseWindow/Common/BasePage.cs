using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace BaseWindow.Common
{
    public class BasePage:Page
    {
        private MainWindow _parentWin;
        private int TimeOutValue = 10;//倒计时
        private bool IsGoToHomePage = true;

        private DateTime DtStartShow = DateTime.Now;//窗口开始显示的时间
        private DispatcherTimer DisTimer;

        /// <summary>
        /// 超时处理的委托
        /// </summary>
        public delegate void TimeOverHandler();
        public TimeOverHandler TimeOverHandler_Tick;

        /// <summary>
        /// 倒计时处理的委托
        /// </summary>
        /// <param name="sum_tick"></param>
        public delegate void GetCurrentTimerTickHandler(int sum_tick);
        public GetCurrentTimerTickHandler GetCurrentTimerTickHandler_Tick;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="countdown"></param>
        public delegate void GetCountDownHandler(int countdown);
        public GetCountDownHandler GetCountDownHandler_Tick;

        private List<bool> enableButtonsInited = new List<bool>();
        private List<Button> enableButtons = new List<Button>();
        private List<int> enableButtonsTimeMS = new List<int>();
        private List<RoutedEventHandler> enableButtonsEvent = new List<RoutedEventHandler>();

        public int TimeOutValueConfig
        {
            get
            {
                return TimeOutValue;
            }

            set
            {
                TimeOutValue = value;
            }
        }

        public MainWindow ParentWindow
        {
            get
            {
                return _parentWin;
            }

            set
            {
                _parentWin = value;
            }
        }

        public BasePage()
        {
            this.Loaded += Base_Loaded;
            this.Unloaded += BasePage_Unloaded;
        }

        private void BasePage_Unloaded(object sender, RoutedEventArgs e)
        {
            DisTimer.Stop();
            DisTimer.IsEnabled = false;
            bool IsSuccess = OutPutParameter();
        }

        private void Base_Loaded(object sender, RoutedEventArgs e)
        {
            DisTimer.Interval = new TimeSpan(0, 0, 0, 0, 300);
            DisTimer.Tick += new EventHandler(DisTimer_Tick);
            DisTimer.Start();

            bool IsSuccess = InPutParameter();

            if (this.ParentWindow.UiConfig.GetGlobalVar().HasValue("GLOBAL_WAIT_TIME"))
            {
                TimeOutValue = int.Parse(this.ParentWindow.UiConfig.GetGlobalVar()["GLOBAL_WAIT_TIME"].ToString());
            }else
            {
                TimeOutValue = 60;
            }
        }

        private void DisTimer_Tick(object sender, EventArgs e)
        {
            int totalms = (int)DateTime.Now.Subtract(DtStartShow).TotalMilliseconds;
            int totalseconds = (int)DateTime.Now.Subtract(DtStartShow).TotalSeconds;
            if (GetCurrentTimerTickHandler_Tick != null)
            {
                GetCurrentTimerTickHandler_Tick(totalseconds);
            }
            int t = TimeOutValue - totalseconds;
            if (GetCountDownHandler_Tick != null)
            {
                GetCountDownHandler_Tick(t);
            }
            DisTimerEvent(totalms);
            if (t <= 0)
            {
                if (TimeOverHandler_Tick != null)
                {
                    TimeOverHandler_Tick();
                }else
                {
                    if (IsGoToHomePage == true)
                    {
                        TimerTickEnd();
                    }
                }
            }
        }

        private void TimerTickEnd()
        {
            this.ParentWindow.JumpPage("Home");
        }

        private void DisTimerEvent(int startTimeMS)
        {
           for(int i = 0; i < enableButtons.Count; i++)
            {
                if (enableButtonsInited[i])
                    continue;
                if (startTimeMS < enableButtonsTimeMS[i])
                    continue;
                enableButtons[i].Click += enableButtonsEvent[i];
                enableButtonsInited[i] = true;
            }
        }

        public void StopTimerTick()
        {
            DisTimer.Stop();
        }

        public virtual bool InPutParameter()
        {
            return true;
        }

        public virtual bool OutPutParameter()
        {
            return true;
        }
    }
}
