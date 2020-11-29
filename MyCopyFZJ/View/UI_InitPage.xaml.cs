using BaseWindow.Common;
using OpsCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace MyCopyFZJ.View
{
    /// <summary>
    /// UI_InitPage.xaml 的交互逻辑
    /// </summary>
    public partial class UI_InitPage : BasePage
    {
        public UI_InitPage()
        {
            InitializeComponent();
        }

        private void BasePage_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                OpsHelper.Instance.CrashClear();
                FlashLogger.Debug("进入初始化界面");
                ComFuntion
            }
            catch(Exception ex)
            {

            }
        }
    }
}
