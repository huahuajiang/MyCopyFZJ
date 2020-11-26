using MisFrameWork.core;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

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
                    if (styleType.Equals("ThemeType_3.josn"))
                    {
                        wnd.SetVersionControlColor(Brushes.Gray);
                        wnd.Title_Word.Foreground = Brushes.Gray;
                        wnd.Title_Word.FontFamily = new FontFamily("微软雅黑");
                    }
                    themeFile = AppDomain.CurrentDomain.BaseDirectory + "SystemConfig\\Theme\\" + styleType;
                    List<UnCaseSenseHashTable> themeFileUncaseList = getJosn(themeFile);
                    
                    foreach(UnCaseSenseHashTable tmpfile in themeFileUncaseList)
                    {
                        string xmlfile = Convert.ToString(tmpfile["FILENAME"]);
                        ResourceDictionary languageResDic = new ResourceDictionary();
                        languageResDic.Source = new Uri("pack://application:,,," + xmlfile, UriKind.RelativeOrAbsolute);
                        this.Resources.MergedDictionaries.Add(languageResDic);
                    }
                }

                ImageBrush brush = new ImageBrush();
                string path;
                if (wnd.UiConfig.GetGlobalVar().HasKeyValue("IMG_BG_PATH"))
                {
                    path = Convert.ToString(wnd.UiConfig.GetGlobalVar()["IMG_BG_PATH"]);
                }
                else
                {
                    path = AppDomain.CurrentDomain.BaseDirectory + "UI_Images\\Background.jpg";
                }
                brush.ImageSource = new BitmapImage(new Uri(path, UriKind.Relative));
                wnd.Background = brush;
                wnd.SetMainEdition(Assembly.GetExecutingAssembly().GetName().Version.ToString());
                string title;
                if (wnd.UiConfig.GetGlobalVar().HasKeyValue("MAIN_TITLE"))
                {
                    title = Convert.ToString(wnd.UiConfig.GetGlobalVar()["MAIN_TITLE"]);
                }
                else
                {
                    title = "自动领证系统";
                }

                wnd.Title_Word.Content = title;
                wnd.Title_Word.FontSize = 40;
                wnd.SetMarqueePosition(0, 995);
                wnd.SetMarqueeTextColor(Brushes.Gray);
                if (wnd.UiConfig.GetGlobalVar().HasKeyValue("SRT_TIME"))
                {
                    wnd.SetSreenProtectTime(Convert.ToInt32(wnd.UiConfig.GetGlobalVar()["SRT_TIME"]));
                }
                wnd.Show();

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
