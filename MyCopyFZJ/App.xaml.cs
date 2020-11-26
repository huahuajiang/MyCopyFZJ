using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace MyCopyFZJ
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        public EventWaitHandle ProgramStarted { get; set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            bool createNew;
            ProgramStarted = new EventWaitHandle(false, EventResetMode.AutoReset, "MyCopyFZJ", out createNew);
            if (!createNew)
            {
                MessageBox.Show("程序已经启动");
                App.Current.Shutdown();
                Environment.Exit(0);
            }

            base.OnStartup(e);
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            try
            {
                BaseWindow.MainWindow wnd = new BaseWindow.MainWindow();
                wnd.Width = 1280;
                wnd.Height = 1024;
                wnd.SetVersionControlColor(Brushes.White);
                //动态样式加载
                string themeFile;
                if (wnd.UiConfig.GetGlobalVar().HasKeyValue("THEME_TYPE"))
                {
                    string styleType = Convert.ToString(wnd.UiConfig.GetGlobalVar()["THEME_TYPE"]);

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Application_LoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {

        }

        private void Application_Activated(object sender, EventArgs e)
        {

        }
    }
}
