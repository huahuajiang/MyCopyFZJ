using BaseWindow.Common;
using BaseWindow.UserControls;
using FormUIFlow;
using MisFrameWork.core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace BaseWindow
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// 左上角图标
        /// </summary>
        public Image Title_Img
        {
            get { return this.img_Tilte; }
            set { this.img_Tilte = value; }
        }

        /// <summary>
        /// 左上角名称
        /// </summary>
        public Label Title_Word
        {
            get { return this.lbl_Title; }
            set { this.lbl_Title = value; }
        }

        private DispatcherTimer GCTimer = new DispatcherTimer();
        private int maxWorkingSet = 209715200;
        public UIFlowConfig UiConfig;//当前Ui流程
        private UIFlowConfig AdminUiConfig;
        private UIFlowConfig NomUiConfig;
        private string Adm = "";
        private int click_sum = 0;
        public string UI_CFG = "";//用户流程
        public string ADMIN_UI_CFG = "";//管理员流程
        private string VERSION = "";
        private int SystemMode = 0;

        UnCaseSenseHashTable configFile;
        private string OldPage;
        private bool IsStartSreenPrt = true;//屏保线程是否启动
        Thread SreenThread;//屏保线程
        private int SRT_TIME = 600;//多长时间后启动屏保
        private int SRT_COUNT = 0;
        private bool IsCloseProtectScreen = false;//是否屏保已打开

        public delegate void BeforeExistProgramHandler();
        public BeforeExistProgramHandler beforeExistProgramHandler;

        /// <summary>
        /// 输入板声明
        /// </summary>
        public UC_CertificateKeyBoard uc_Certificatekeyboard = null;
        /// <summary>
        /// 获取焦点的声明
        /// </summary>
        public TextBox tbGetForcus = null;
        /// <summary>
        /// 是否显示输入板
        /// </summary>
        public bool isvisibHand = true;

        public MainWindow()
        {
            InitializeComponent();
            InitUiConfig();

        }

        private void InitUiConfig()
        {
            try
            {
                configFile = new UnCaseSenseHashTable();
                string SysConfig = AppDomain.CurrentDomain.BaseDirectory + "SystemConfig\\main_cfg.json";
                configFile.LoadFromJsonFile(SysConfig);

                if (configFile.HasValue("UI_CFG"))
                {
                    UI_CFG = AppDomain.CurrentDomain.BaseDirectory + configFile["UI_CFG"].ToString();
                }
                else
                {
                    MessageBox.Show("加载UI配置失败", "错误");
                    this.Close();
                    return;
                }

                if (configFile.HasValue("ADMIN_UI_CFG"))
                {
                    ADMIN_UI_CFG = AppDomain.CurrentDomain.BaseDirectory + configFile["ADMIN_UI_CFG"].ToString();
                    AdminUiConfig = UIFlowConfig.LoadFromJSONFile(ADMIN_UI_CFG);
                }

                NomUiConfig = UIFlowConfig.LoadFromJSONFile(UI_CFG);

                UnCaseSenseHashTable UIGolbal = new UnCaseSenseHashTable();
                UIGolbal.LoadFromJsonFile(AppDomain.CurrentDomain.BaseDirectory + "SystemConfig\\app_config_vars.json");
                for (int i = 0; i < UIGolbal.Count; i++)
                {
                    string[] key = new string[UIGolbal.Count];
                    UIGolbal.Keys.CopyTo(key, 0);
                    NomUiConfig.GetGlobalVar().Add(key[i], UIGolbal[key[i]]);
                    if (AdminUiConfig != null)
                    {
                        AdminUiConfig.GetGlobalVar().Add(key[i], UIGolbal[key[i]]);
                    }
                }

                UiConfig = NomUiConfig;
            }
            catch (Exception ex)
            {
                FlashLogger.Error(ComFun.ErrorMessage(ex));
            }
        }

        public void EventsRegistion()
        {
            this.GCTimer.Tick += new EventHandler(OnGarbageCollection);
        }

        public void EventDeregistration()
        {
            this.GCTimer.Tick -= new EventHandler(OnGarbageCollection);
        }

        private void OnGarbageCollection(object sender, EventArgs e)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }

        private void mainFrame_Navigated(object sender, NavigationEventArgs e)
        {
            try
            {
                if (this.mainFrame.CanGoBack)
                {
                    this.mainFrame.RemoveBackEntry();
                    this.mainFrame.NavigationService.RemoveBackEntry();
                }
            }
            catch (Exception ex)
            {
                FlashLogger.Error(ComFun.ErrorMessage(ex));
            }
        }

        private void txtB_pwd_GotFocus(object sender, RoutedEventArgs e)
        {
            HandlePhInputClick(sender);
        }

        /// <summary>
        /// 实例化输入板
        /// </summary>
        /// <param name="sender"></param>
        private void HandlePhInputClick(object sender)
        {
            try
            {
                TextBox tb = sender as TextBox;
                if (tb == null)
                {
                    return;
                }
                this.tbGetForcus = tb;
                if (uc_Certificatekeyboard == null)
                {
                    uc_Certificatekeyboard = new UC_CertificateKeyBoard() { Width = 630, Height = 350, inputTextBox = txtB_pwd };
                }
                Panel can = tb.Parent as Panel;
                if (can != null)
                {
                    if (!can.Children.Contains(uc_Certificatekeyboard))
                    {
                        can.Children.Add(uc_Certificatekeyboard);
                    }
                }
                if (this.isvisibHand == true)
                {
                    this.uc_Certificatekeyboard.Visibility = Visibility.Visible;
                }
                else
                {
                    this.uc_Certificatekeyboard.Visibility = Visibility.Visible;
                }
                Canvas.SetLeft(uc_Certificatekeyboard, Canvas.GetLeft(tb));
                Canvas.SetLeft(uc_Certificatekeyboard, Canvas.GetTop(tb) + tb.Height + 10);
            }
            catch (Exception ex)
            {
                FlashLogger.Error(ComFun.ErrorMessage(ex));
            }
        }

        private void btn_submit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (UiConfig.GetGlobalVar().HasKeyValue("ADMIN_PWD"))
                {
                    if (txtB_pwd.Text == Convert.ToString(UiConfig.GetGlobalVar()["ADMIN_PWD"]))
                    {
                        Point curPoint = new Point();
                        curPoint.Y = 0;
                        curPoint.X = -this.ActualWidth;
                        MoveTo(curPoint, CanSreenPrt, 1);
                        IsCloseProtectScreen = true;
                        if (Convert.ToString(UiConfig.GetGlobalVar()["NEED_PROTECT_SCREEN"]) != "0")
                        {
                            StartSreenProtect();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                FlashLogger.Error(ComFun.ErrorMessage(ex));
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            can_bg.Height = this.ActualHeight;
            can_bg.Width = this.ActualWidth;
            CanSreenPrt.Width = this.ActualWidth;
            CanSreenPrt.Height = this.ActualHeight;
            this.MouseDown += UI_MouseDown;
            Canvas.SetLeft(CanSreenPrt, -this.ActualWidth);
            Canvas.SetTop(CanSreenPrt, 0);
            Canvas.SetLeft(can_pwd, CanSreenPrt.Width / 2 - can_pwd.Width / 2);
            Canvas.SetTop(can_pwd, CanSreenPrt.Height * 5 / 7);

            FileInfo fi = new FileInfo(System.Windows.Forms.Application.ExecutablePath);
            if (File.Exists(fi.Directory.Parent.FullName + "\\is_autoupdate_server.txt"))
            {
                MessageBox.Show("又不听话了，不要在服务器上运行程序!");
                Environment.Exit(0);
            }

            try
            {
                FlashLogger.Instance().Register();

                this.GCTimer.Interval = TimeSpan.FromSeconds(5);
                this.GCTimer.Start();

                this.EventsRegistion();

                uc_keyboard.Visibility = Visibility.Hidden;
                uc_keyboard.SwitchEvent = new HB_UserControls.UC.UC_Keyboard_Admin.SwitchEventHandle(Logout);
                uc_keyboard.ComFirmEvent += new HB_UserControls.UC.UC_Keyboard_Admin.ComFirmEventHandle(btn_comfirm);
                uc_keyboard.Set_Parent_Data(this.Width, this.Height);

                InitWindow(0);
                JumpPage("");
            }
            catch (Exception ex)
            {
                FlashLogger.Error(ComFun.ErrorMessage(ex));
                MessageBox.Show("系统错误：" + ex.Message, "错误");
            }

        }


        /// <summary>
        /// 确认按钮
        /// </summary>
        /// <param name="data">输入数字</param>
        private void btn_comfirm(string data)
        {
            try
            {
                if (UiConfig.GetGlobalVar().HasKeyValue("EXIST_PWD"))
                {
                    FlashLogger.Debug("比对退出密码");
                    if (data == Convert.ToString(UiConfig.GetGlobalVar()["EXIST_PWD"]))
                    {
                        FlashLogger.Debug("比对退出密码成功");
                        if (beforeExistProgramHandler != null)
                        {
                            beforeExistProgramHandler();
                        }
                        Environment.Exit(123456);
                    }
                }

                if (UiConfig.GetGlobalVar().HasKeyValue("DEBUG_PWD"))
                {
                    FlashLogger.Debug("比对调试密码");
                    if (data == Convert.ToString(UiConfig.GetGlobalVar()["DEBUG_PWD"]))
                    {
                        FlashLogger.Debug("比对调试密码成功");
                        if (UiConfig.GetGlobalVar().HasKeyValue("DEBUG"))
                        {
                            if (Convert.ToString(UiConfig.GetGlobalVar()["DEBUG"]) == "1")
                            {
                                UiConfig.GetGlobalVar()["DEBUG"] = "0";
                                this.Topmost = true;
                            }
                            else
                            {
                                UiConfig.GetGlobalVar()["DEBUG"] = "1";
                                this.Topmost = false;
                            }
                            JumpPage("Home");
                        }
                    }
                }

                if (UiConfig.GetGlobalVar().HasKeyValue("ADMIN_PWD"))
                {
                    FlashLogger.Debug("比对管理员密码");
                    if (data == Convert.ToString(UiConfig.GetGlobalVar()["ADMIN_PWD"]))
                    {
                        FlashLogger.Debug("比对管理员密码");
                        InitWindow(1);
                        JumpPage("Home");
                    }
                }
            }
            catch (Exception ex)
            {
                FlashLogger.Error(ComFun.ErrorMessage(ex));
            }
        }

        private void Logout()
        {
            try
            {
                InitWindow(0);
                JumpPage("Home");
            }
            catch (Exception ex)
            {
                FlashLogger.Error(ComFun.ErrorMessage(ex));
            }
        }

        public void JumpPage(string Operation)
        {
            bool IsNoNeedJump = false;
            try
            {
                SRT_COUNT = 0;
                switch (Operation)
                {
                    case "Self":
                        break;
                    case "Next":
                        UnCaseSenseHashTable ushtmp = UiConfig.GetCurrentProp();
                        if (ushtmp.HasValue("__GO_BACK__"))
                        {
                            int step = ushtmp.GetIntValue("__GO_BACK__");
                            for (int i = 0; i < step; i++)
                            {
                                UiConfig.GotoPreviousStep();
                            }
                        }
                        else
                        {
                            UiConfig.GotoNextStep();
                        }
                        break;
                    case "NextStep":
                        UiConfig.GotoNextStep();
                        break;
                    case "Last":
                        UnCaseSenseHashTable usht = UiConfig.GetCurrentProp();
                        if (usht.HasValue("__PRE_STEP__"))
                        {
                            int step = usht.GetIntValue("__PRE_STEP__");
                            for (int i = 0; i < step; i++)
                                UiConfig.GotoPreviousStep();
                        }
                        else
                        {
                            UiConfig.GotoPreviousStep();
                        }
                        break;
                    case "Home":
                        UiConfig.GotoFirstStep();
                        break;
                    case "NoneJump":
                        IsNoNeedJump = true;
                        break;
                    default:
                        UiConfig.GotoFirstStep();
                        break;
                }
                try
                {
                    if (IsNoNeedJump == true)
                    {
                        return;
                    }
                    BasePage control = UiConfig.GetCurrentStepObject() as BasePage;
                    control.ParentWindow = this;
                    this.mainFrame.Navigate(control);

                }
                catch (Exception e)
                {
                    FlashLogger.Error(ComFun.ErrorMessage(e));
                    MessageBox.Show("JumpPage出错，请重新配置配置文件:" + e.Message);
                    Environment.Exit(0);
                }
            }
            catch (Exception ex)
            {
                FlashLogger.Error(ComFun.ErrorMessage(ex));
                MessageBox.Show("系统页面跳转出错：" + ex.Message);
            }
        }

        private void InitWindow(int type)
        {
            try
            {
                SwithUi(type);

                To_Top_Left();

                Is_Out_Log();

                SetEdition(VERSION);
            }
            catch (Exception ex)
            {
                FlashLogger.Error(ComFun.ErrorMessage(ex));
            }
        }

        /// <summary>
        /// 设置版本
        /// </summary>
        /// <param name="version"></param>
        private void SetEdition(string version = "")
        {
            if (!string.IsNullOrEmpty(version))
            {
                lbl_version.Width = this.ActualWidth;
                lbl_version.Content = "版本编号:" + version + "   " + "机器编号:" + UiConfig.GetGlobalVar()["MACHINE_NO"].ToString();
            }
            else
            {
                lbl_version.Content = "版本编号:" + Assembly.GetExecutingAssembly().GetName().Version.ToString() + "   " + "机器编号：" + UiConfig.GetGlobalVar()["MACHINE_NO"].ToString();
            }
        }

        private void Is_Out_Log()
        {
            if (UiConfig.GetGlobalVar().HasValue("IS_CREATE_LOG"))
            {
                UiConfig.IsOutLogger = true;
            }
            else
            {
                UiConfig.IsOutLogger = false;
            }
        }

        /// <summary>
        /// 控制窗口显示位置
        /// </summary>
        private void To_Top_Left()
        {
            try
            {
                this.WindowStartupLocation = WindowStartupLocation.Manual;
                this.Left = 0;
                this.Top = 0;
                if (UiConfig.GetGlobalVar().HasValue("DEBUG"))
                {
                    if (Convert.ToString(UiConfig.GetGlobalVar()["DEBUG"]) != "1")
                    {
                        this.Topmost = true;
                    }
                }
            }
            catch (Exception ex)
            {
                FlashLogger.Error(ComFun.ErrorMessage(ex));
            }
        }

        private void SwithUi(int type = 0)
        {
            try
            {
                uc_keyboard.Change_Btn_Type("Full_Keyboard");
                switch (type)
                {
                    case 0:
                        UiConfig = NomUiConfig;
                        SystemMode = 0;
                        StopSreenProtect();
                        break;
                    case 1:
                        if (AdminUiConfig != null)
                        {
                            UiConfig = AdminUiConfig;
                            SystemMode = 1;
                            if (Convert.ToString(UiConfig.GetGlobalVar()["NEED_PROTECT_SCREEN"]) != "0")
                            {
                                StartSreenProtect();
                            }
                        }
                        break;
                    default:
                        UiConfig = NomUiConfig;
                        SystemMode = 0;
                        StopSreenProtect();
                        break;
                }
            }
            catch (Exception ex)
            {
                FlashLogger.Error(ComFun.ErrorMessage(ex));
            }
        }

        /// <summary>
        /// 停止屏保线程
        /// </summary>
        private void StopSreenProtect()
        {
            try
            {
                if (SreenThread != null)
                {
                    if (SreenThread.IsAlive)
                    {
                        IsStartSreenPrt = false;
                    }
                }
            }
            catch (Exception ex)
            {
                FlashLogger.Error(ComFun.ErrorMessage(ex));
            }
        }

        /// <summary>
        /// 开启屏保线程
        /// </summary>
        private void StartSreenProtect()
        {
            try
            {
                if (SreenThread != null)
                {
                    if (SreenThread.IsAlive == true)
                    {
                        return;
                    }
                }
                txtB_pwd.Text = "";
                IsStartSreenPrt = true;
                SreenThread = new Thread(SreenThreadHandler);
                SreenThread.IsBackground = true;
                SreenThread.Start();

            }
            catch (Exception ex)
            {
                FlashLogger.Error(ComFun.ErrorMessage(ex));
            }
        }

        /// <summary>
        /// 屏保线程
        /// </summary>
        private void SreenThreadHandler()
        {
            try
            {
                SRT_COUNT = 0;
                while (true)
                {
                    if (IsStartSreenPrt == false)
                    {
                        return;
                    }

                    if (SRT_COUNT > SRT_TIME)
                    {
                        this.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            Point curPoint = new Point();
                            curPoint.Y = 0;
                            curPoint.X = 0;
                            MoveTo(curPoint, CanSreenPrt, 1);
                        }));
                        IsCloseProtectScreen = true;
                        return;
                    }
                    Thread.Sleep(1000);
                    SRT_COUNT++;
                }
            }
            catch (Exception ex)
            {
                FlashLogger.Error(ComFun.ErrorMessage(ex));
            }
        }

        /// <summary>
        /// 移动
        /// </summary>
        /// <param name="deskPoint"></param>
        /// <param name="ell"></param>
        /// <param name="space"></param>
        private void MoveTo(Point deskPoint, Canvas ell, double space)
        {
            try
            {
                Point curPoint = new Point();
                curPoint.X = Canvas.GetLeft(ell);
                curPoint.Y = Canvas.GetTop(ell);
                Storyboard storyboard = new Storyboard();
                double lxspeed = space, lyspeed = space;
                DoubleAnimation doubleAnimation = new DoubleAnimation(Canvas.GetLeft(ell), deskPoint.X, new Duration(TimeSpan.FromMilliseconds(lxspeed)));
                Storyboard.SetTarget(doubleAnimation, ell);
                Storyboard.SetTargetProperty(doubleAnimation, new PropertyPath("(Canvas.Left)"));
                storyboard.Children.Add(doubleAnimation);
                doubleAnimation = new DoubleAnimation(
                  Canvas.GetTop(ell),
                  deskPoint.Y,
                  new Duration(TimeSpan.FromMilliseconds(lyspeed))
                );
                //storyboard.Completed += Storyboard_Completed;
                Storyboard.SetTarget(doubleAnimation, ell);
                Storyboard.SetTargetProperty(doubleAnimation, new PropertyPath("(Canvas.Top)"));
                storyboard.Children.Add(doubleAnimation);
                storyboard.Begin();
            }
            catch (Exception ex)
            {
                FlashLogger.Error(ComFun.ErrorMessage(ex));
            }
        }

        /// <summary>
        /// 按下鼠标按钮时发生
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UI_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                SRT_COUNT = 0;
                if (this.uc_Certificatekeyboard != null)
                {
                    TraversalRequest t = new TraversalRequest(FocusNavigationDirection.Next);
                    txtB_pwd.MoveFocus(t);
                    this.uc_Certificatekeyboard.Visibility = Visibility.Hidden;
                }
            }
            catch (Exception ex)
            {
                FlashLogger.Error(ComFun.ErrorMessage(ex));
            }
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            try
            {
                SetWorking(maxWorkingSet);
            }
            catch (Exception ex)
            {
                FlashLogger.Error(ComFun.ErrorMessage(ex));
            }
        }

        private void SetWorking(int maxWorkingSet)
        {
            IntPtr min = Process.GetCurrentProcess().MinWorkingSet;
            try
            {
                Process.GetCurrentProcess().MaxWorkingSet = (IntPtr)maxWorkingSet;
            }
            catch (Exception ex)
            {
                FlashLogger.Error(ComFun.ErrorMessage(ex));
                Process.GetCurrentProcess().MaxWorkingSet = min;
            }
        }

        /// <summary>
        /// 打开屏保
        /// </summary>
        /// <param name="type">1显示密码输入 0不显示密码输入</param>
        public void OpenProtectScreen(int type)
        {
            this.Dispatcher.BeginInvoke((Action)delegate ()
            {
                if (type == 0)
                {
                    this.Dispatcher.BeginInvoke((Action)delegate ()
                    {
                        label.Content = "请验证二维码或者指纹退出屏保";
                        txtB_pwd.Visibility = Visibility.Collapsed;
                        btn_submit.Visibility = Visibility.Collapsed;
                    });
                }
                Point curPoint = new Point();
                curPoint.Y = 0;
                curPoint.X = 0;
                MoveTo(curPoint, CanSreenPrt, 1);
                IsCloseProtectScreen = true;
            });
        }


        /// <summary>
        /// 关闭屏保
        /// </summary>
        public void CloseProtectScreen()
        {
            this.Dispatcher.BeginInvoke((Action)delegate ()
            {
                Point curPoint = new Point();
                curPoint.Y = 0;
                curPoint.X = -this.ActualWidth;
                MoveTo(curPoint, CanSreenPrt, 1);
                IsCloseProtectScreen = false;
            });
        }

        /// <summary>
        /// 获取屏保状态
        /// </summary>
        /// <returns></returns>
        public bool GetProtectScreenStatus()
        {
            return IsCloseProtectScreen;
        }

        public int GetSystemMode()
        {
            return SystemMode;
        }

        public void SetMarqueePosition(double left, double top)
        {
            Canvas.SetTop(this.scrollingTextControl, top);
            Canvas.SetLeft(this.scrollingTextControl, left);
        }

        public void SetMarqueeTextColor(Brush forergroundBrush)
        {
            scrollingTextControl.setTextFontColor(forergroundBrush);
        }

        public void SetSreenProtectTime(int second = 600)
        {
            SRT_TIME = second;
        }

        public void SetVersionControlColor(Brush brushes)
        {
            lbl_version.Foreground = brushes;
        }


        /// <summary>
        /// 获取版本号
        /// </summary>
        /// <param name="version"></param>
        public void SetMainEdition(string version = "")
        {
            VERSION = version;
        }

    }
}
