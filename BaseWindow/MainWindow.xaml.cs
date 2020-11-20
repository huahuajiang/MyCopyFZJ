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
        private UIFlowConfig NowUiConfig;
        private string Adm = "";
        private int click_sum = 0;
        public string UI_CFG = "";
        public string ADMIN_UI_CFG = "";
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

        }

        private void mainFrame_Navigated(object sender, NavigationEventArgs e)
        {
            
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
