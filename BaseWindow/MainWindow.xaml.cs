using BaseWindow.Common;
using BaseWindow.UserControls;
using FormUIFlow;
using MisFrameWork.core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
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
        private UIFlowConfig UiConfig;
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
        private bool IsStartSreenPrt = true;
        Thread SreenThread;
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
                string SysConfig=AppDomain.CurrentDomain.BaseDirectory+ "SystemConfig\\main_cfg.json";
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
                for(int i = 0; i < UIGolbal.Count; i++)
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
            }catch(Exception ex)
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
            }catch(Exception ex)
            {
                FlashLogger.Error(ComFun.ErrorMessage(ex));
            }
        }

        private void txtB_pwd_GotFocus(object sender, RoutedEventArgs e)
        {

        }

        private void btn_submit_Click(object sender, RoutedEventArgs e)
        {

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
            if(File.Exists(fi.Directory.Parent.FullName+ "\\is_autoupdate_server.txt"))
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

                uc_Certificatekeyboard.Visibility = Visibility.Hidden;
                uc_keyboard.SwitchEvent = new HB_UserControls.UC.UC_Keyboard_Admin.SwitchEventHandle(Logout);
                uc_keyboard.ComFirmEvent += new HB_UserControls.UC.UC_Keyboard_Admin.ComFirmEventHandle(btn_comfirm);
                uc_keyboard.Set_Parent_Data(this.Width, this.Height);

                InitWindow(0);
                JumpPage("");
            }
            catch(Exception ex)
            {
                FlashLogger.Error(ComFun.ErrorMessage(ex));
                MessageBox.Show("系统错误：" + ex.Message, "错误");
            }

        }

        private void btn_comfirm(string data)
        {
            try
            {
                if (UiConfig.GetGlobalVar().HasKeyValue("EXIST_PWD"))
                {

                }
            }
            catch(Exception ex)
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
            catch(Exception ex)
            {
                FlashLogger.Error(ComFun.ErrorMessage(ex));
            }
        }

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
            }catch(Exception ex)
            {
                FlashLogger.Error(ComFun.ErrorMessage(ex));
            }
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {


        }
    }
}
