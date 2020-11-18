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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BaseWindow.UserControls
{
    /// <summary>
    /// MarqueeControl.xaml 的交互逻辑
    /// </summary>
    public partial class MarqueeControl : UserControl
    {
        Storyboard std = null;
        DoubleAnimation animation = null;
        int index, total;

        public MarqueeControl()
        {
            InitializeComponent();
        }

        public MarqueeType ShowType
        {
            get { return (MarqueeType)this.GetValue(ShowTypeProperty); }
            set { this.SetValue(ShowTypeProperty, value); }
        }
        public static readonly DependencyProperty ShowTypeProperty = DependencyProperty.Register("ShowType", typeof(MarqueeType), typeof(MarqueeControl), new PropertyMetadata(MarqueeType.Up));


        public double Speed
        {
            get { return (double)this.GetValue(SpeedProperty); }
            set { this.SetValue(SpeedProperty, value); }
        }
        public static readonly DependencyProperty SpeedProperty = DependencyProperty.Register("Speed", typeof(double), typeof(MarqueeControl), new PropertyMetadata(1.5));


        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (ShowType == MarqueeType.Up || ShowType == MarqueeType.Down)
            {
                std = (Storyboard)canvas.Resources["stdUp"];
                content.Width = canvas.ActualWidth;
                txtItem.TextWrapping = TextWrapping.Wrap;
            }
            if (ShowType == MarqueeType.Left || ShowType == MarqueeType.Right)
            {
                std = (Storyboard)canvas.Resources["stdLeft"];
                content.Height = canvas.ActualHeight;
            }

            animation = (DoubleAnimation)std.Children[0];
            std.Completed += (t, r) => changeItem();
        }


        private List<string> itemsSource;
        public List<string> ItemsSource
        {
            get { return itemsSource; }
            set
            {
                this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    if (std != null)
                    {
                        std.Stop();
                        txtItem.Text = "";
                        itemsSource = value;

                        if (itemsSource != null && itemsSource.Count > 0)
                        {
                            index = 0;
                            total = value.Count;
                            changeItem();
                        }
                    }
                }));
            }
        }
        public void setTextFontColor(Brush foregroundBrush)
        {
            txtItem.Foreground = foregroundBrush;
        }
        private void changeItem()
        {
            txtItem.Text = itemsSource[index].ToString();
            txtItem.UpdateLayout();
            double canvasWidth = canvas.ActualWidth;
            double canvasHeight = canvas.ActualHeight;
            double txtWidth = txtItem.ActualWidth;
            double txtHeight = txtItem.ActualHeight;


            if (ShowType == MarqueeType.Up)
            {
                animation.From = canvasHeight;
                animation.To = -txtHeight;
            }
            else if (ShowType == MarqueeType.Down)
            {
                animation.From = -txtHeight;
                animation.To = canvasHeight;
            }
            else if (ShowType == MarqueeType.Left)
            {
                animation.From = canvasWidth;
                animation.To = -txtWidth;
            }
            else if (ShowType == MarqueeType.Right)
            {
                animation.From = -txtWidth;
                animation.To = canvasWidth;
            }
            int time = 0;
            if (ShowType == MarqueeType.Up || ShowType == MarqueeType.Down)
            {
                time = (int)(txtHeight / canvasHeight * Speed);
            }
            if (ShowType == MarqueeType.Left || ShowType == MarqueeType.Right)
            {
                time = (int)(txtWidth / canvasWidth * Speed);
            }
            if (time < 4) time = 4;
            animation.Duration = new Duration(new TimeSpan(0, 0, time));


            index++;
            if (index == total) index = 0;


            if (std != null)
            {
                std.Begin();
            }
        }
    }


    public enum MarqueeType
    {
        Up,
        Down,
        Left,
        Right
    }
}
