using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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

namespace BaseWindow.UserControls
{
    /// <summary>
    /// UC_CertificateKeyBoard.xaml 的交互逻辑
    /// </summary>
    public partial class UC_CertificateKeyBoard : UserControl
    {
        public UC_CertificateKeyBoard()
        {
            InitializeComponent();
        }

        public TextBox inputTextBox;
        public delegate void BtnDel(string content);
        public BtnDel outputcontent;
        public Window ownerwindow;


        private void btn_Click(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            if (string.IsNullOrEmpty(Convert.ToString(outputcontent)))
                outputcontent(b.Content.ToString());
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            outputcontent = output;
        }

        private void output(string content)
        {
            handWriteResultValue(content, inputTextBox);
        }

        private void handWriteResultValue(string strinput, TextBox inputTextBox)
        {
            if (inputTextBox != null)
            {
                string s = inputTextBox.Text;
                int idx = inputTextBox.SelectionStart;
                s = s.Insert(idx, strinput.Substring(0, 1));
                inputTextBox.Text = s;
                inputTextBox.SelectionStart = idx + 1;
                inputTextBox.Focus();
            }
        }

        [DllImport("user32.dll")]
        public static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);

        private void btn_delete_Click(object sender, RoutedEventArgs e)
        {
            if (inputTextBox != null)
            {
                inputTextBox.Focus();
                keybd_event(0x08, 0, 0, 0);
                keybd_event(0x08, 0, 0x02, 0);
            }
        }
    }
}
