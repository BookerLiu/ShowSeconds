using Gma.System.MouseKeyHook;
using ShowSeconds.Util;
using ShowSeconds.ViewModel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
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

namespace ShowSeconds
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class SecondsWindow : Window
    {

        private System.Drawing.Color beforeColor;
        private System.Drawing.Color topBeforeColor;

        private bool expandClock = true; //是否展开时钟
        private System.Windows.Forms.Timer timer;
        public SecondsWindow()
        {
            SecondsDataContext dc = new SecondsDataContext
            {
                Seconds = (DateTime.Now.Hour).ToString() + ":" +
                        FormatMS(DateTime.Now.Minute) + ":" +
                        FormatMS(DateTime.Now.Second)
            };
            InitializeComponent();
            SolidColorBrush scb = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 47, 52, 44))
            {
                Opacity = 0.8
            };
            BGBorder.Background = scb;
            this.DataContext = dc;
            this.Topmost = true;
            BGBorder.Visibility = Visibility.Collapsed;
            this.Show();
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            timer = new System.Windows.Forms.Timer();
            timer.Interval = 1000;
            timer.Tick += Timer_Tick;

            Dispatcher secondsDP = DispatcherBuild.Build();
            IKeyboardMouseEvents secondsHook = Hook.GlobalEvents();
            secondsDP.Invoke((Action)(() =>
            {
                secondsHook.MouseDownExt += SecondsBakColorFun;
                secondsHook.MouseUpExt += SecondsHookSetFuc;
            }));
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            string str = (DateTime.Now.Hour).ToString() + ":" +
                        FormatMS(DateTime.Now.Minute) + ":" +
                        FormatMS(DateTime.Now.Second);
            SecondsDataContext dc = this.DataContext as SecondsDataContext;
            dc.Seconds = str;

            this.DataContext = null;
            this.DataContext = dc;
        }



        private static string FormatMS(int ms)
        {
            if (ms < 10)
            {
                return "0" + ms;
            }
            else
            {
                return ms.ToString();
            }
        }

        private void SecondsHookSetFuc(object sender, MouseEventExtArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                if (ScreenUtil.IsPrimaryFullScreen()) return;

                int sleepTime = 800;
                App.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render, new Action(() =>
                {
                    int x = e.X;
                    int y = e.Y;
                    double w = 1920;
                    double h = 1080;
                    double width = SystemParameters.PrimaryScreenWidth;
                    double height = SystemParameters.PrimaryScreenHeight;
                    if (x > 1843 / w * width
                        && x < 1907 / w * width
                        && y > 1037 / h * height
                        && y < 1074 / h * height)
                    {
                        Thread.Sleep(sleepTime);
                        System.Drawing.Color c = GetBottomBeforeColor();
                        if (c.A != beforeColor.A
                            || c.R != beforeColor.R
                            || c.G != beforeColor.G
                            || c.B != beforeColor.B)
                        {
                            //判断是否展开时钟
                            System.Drawing.Color ct = GetTopBeforeColor();
                            if (ct.A != topBeforeColor.A
                            || ct.R != topBeforeColor.R
                            || ct.G != topBeforeColor.G
                            || ct.B != topBeforeColor.B)
                            {
                                expandClock = true;
                            }
                            else
                            {
                                expandClock = false;
                            }

                            if (!BGBorder.IsVisible)
                            {
                                SecondsDataContext dc = this.DataContext as SecondsDataContext;
                                dc.Seconds = (DateTime.Now.Hour).ToString() + ":" +
                                   FormatMS(DateTime.Now.Minute) + ":" +
                                   FormatMS(DateTime.Now.Second);

                                int sx = (int)(SystemParameters.PrimaryScreenWidth * 0.82);
                                int sMarginBottom = (int)(SystemParameters.WorkArea.Height * 0.03);
                                Left = sx - Width;
                                Top = SystemParameters.WorkArea.Height - Height;
                                BGBorder.Visibility = Visibility.Visible;
                                timer.Start();
                            }
                            else
                            {
                                BGBorder.Visibility= Visibility.Collapsed;
                                timer.Stop();
                            }
                        }
                    }
                    else if (true)
                    {
                        if ((expandClock && (x < 1574 / w * width
                              || x > 1906 / w * width
                              || y < 598 / h * height
                              || y > 1020 / h * height)
                              )
                              || !expandClock && (x < 1574 / w * width
                              || x > 1906 / w * width
                              || y < 950 / h * height
                              || y > 1020 / h * height)
                              )
                        {
                            BGBorder.Visibility = Visibility.Collapsed;
                            timer.Stop();
                        }
                    }
                }));
            }
        }

        private void SecondsBakColorFun(object sender, MouseEventExtArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                beforeColor = GetBottomBeforeColor();
                topBeforeColor = GetTopBeforeColor();
            }
        }

        private static System.Drawing.Color GetBottomBeforeColor()
        {
            return GetColor(1760, 985);
        }

        private static System.Drawing.Color GetTopBeforeColor()
        {
            return GetColor(1751, 693);
        }

        private static System.Drawing.Color GetColor(int w2, int h2)
        {
            double w = 1920;
            double h = 1080;
            double width = SystemParameters.PrimaryScreenWidth;
            double height = SystemParameters.PrimaryScreenHeight;
            System.Drawing.Point p = new System.Drawing.Point((int)(w2 / w * width), (int)(h2 / h * height));
            return ScreenUtil.GetColorAt(p);
        }

    }
}
