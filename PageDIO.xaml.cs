using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace gcp_Wpf
{
    /// <summary>
    /// PageDIO.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class PageDIO : Page
    {

        //Test Timer
        Timer dioTimer = new Timer();

        //Singletone
        singletonClass gClass;
        public PageDIO()
        {
            InitializeComponent();
            gClass = singletonClass.Instance;

            dioTimer.Interval = 1000; // 1 second
            dioTimer.AutoReset = true; // Repeat the timer
            dioTimer.Elapsed += dioTimer_Elapsed;

            Btn_DO0_On.Click += Btn_DO_Click;
            Btn_DO1_On.Click += Btn_DO_Click;
            Btn_DO2_On.Click += Btn_DO_Click;
            Btn_DO3_On.Click += Btn_DO_Click;
            Btn_DO4_On.Click += Btn_DO_Click;
            Btn_DO5_On.Click += Btn_DO_Click;
            Btn_DO6_On.Click += Btn_DO_Click;
            Btn_DO7_On.Click += Btn_DO_Click;
            Btn_DO0_Off.Click += Btn_DO_Click;
            Btn_DO1_Off.Click += Btn_DO_Click;
            Btn_DO2_Off.Click += Btn_DO_Click;
            Btn_DO3_Off.Click += Btn_DO_Click;
            Btn_DO4_Off.Click += Btn_DO_Click;
            Btn_DO5_Off.Click += Btn_DO_Click;
            Btn_DO6_Off.Click += Btn_DO_Click;
            Btn_DO7_Off.Click += Btn_DO_Click;


            dioTimer.Start();
        }

        private void Btn_DO_Click(object sender, RoutedEventArgs e)
        {
            var clickedBtn = sender as Button;

            if(clickedBtn != null)
            {
                if(clickedBtn.Name == "Btn_DO0_On")
                {
                    gClass.str.DioPacket[gClass.srmNum].DO[0] = 1;
                }
                else if(clickedBtn.Name == "Btn_DO0_Off")
                {
                    gClass.str.DioPacket[gClass.srmNum].DO[0] = 0;
                }

                if (clickedBtn.Name == "Btn_DO1_On")
                {
                    gClass.str.DioPacket[gClass.srmNum].DO[0] = 1;
                }
                else if (clickedBtn.Name == "Btn_DO1_Off")
                {
                    gClass.str.DioPacket[gClass.srmNum].DO[0] = 0;
                }
            }
        }

        private void dioTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
           // Console.WriteLine("DI0 = " + gClass.str.DioPacket[gClass.srmNum].DI[0]);
           // Console.WriteLine("DO0 = " + gClass.str.DioPacket[gClass.srmNum].DO[0]);

            Dispatcher.Invoke(() => {
                // Digital Input State
                if (gClass.str.DioPacket[gClass.srmNum].DI[0] > 0)
                {
                    Lbl_DI0.Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/gcp_Wpf;component/Resources/checked.png")));
                }
                else
                {
                    Lbl_DI0.Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/gcp_Wpf;component/Resources/unchecked.png")));
                }
                if (gClass.str.DioPacket[gClass.srmNum].DI[1] > 0)
                {
                    Lbl_DI1.Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/gcp_Wpf;component/Resources/checked.png")));
                }
                else
                {
                    Lbl_DI1.Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/gcp_Wpf;component/Resources/unchecked.png")));
                }
                if (gClass.str.DioPacket[gClass.srmNum].DI[2] > 0)
                {
                    Lbl_DI2.Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/gcp_Wpf;component/Resources/checked.png")));
                }
                else
                {
                    Lbl_DI2.Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/gcp_Wpf;component/Resources/unchecked.png")));
                }
                if (gClass.str.DioPacket[gClass.srmNum].DI[3] > 0)
                {
                    Lbl_DI3.Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/gcp_Wpf;component/Resources/checked.png")));
                }
                else
                {
                    Lbl_DI3.Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/gcp_Wpf;component/Resources/unchecked.png")));
                }
                if (gClass.str.DioPacket[gClass.srmNum].DI[4] > 0)
                {
                    Lbl_DI4.Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/gcp_Wpf;component/Resources/checked.png")));
                }
                else
                {
                    Lbl_DI4.Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/gcp_Wpf;component/Resources/unchecked.png")));
                }
                if (gClass.str.DioPacket[gClass.srmNum].DI[5] > 0)
                {
                    Lbl_DI5.Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/gcp_Wpf;component/Resources/checked.png")));
                }
                else
                {
                    Lbl_DI5.Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/gcp_Wpf;component/Resources/unchecked.png")));
                }
                if (gClass.str.DioPacket[gClass.srmNum].DI[6] > 0)
                {
                    Lbl_DI6.Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/gcp_Wpf;component/Resources/checked.png")));
                }
                else
                {
                    Lbl_DI6.Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/gcp_Wpf;component/Resources/unchecked.png")));
                }
                if (gClass.str.DioPacket[gClass.srmNum].DI[7] > 0)
                {
                    Lbl_DI7.Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/gcp_Wpf;component/Resources/checked.png")));
                }
                else
                {
                    Lbl_DI7.Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/gcp_Wpf;component/Resources/unchecked.png")));
                }


                // Digital OutPut State
                if (gClass.str.DioPacket[gClass.srmNum].DO[0] > 0)
                {
                    Lbl_DO0.Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/gcp_Wpf;component/Resources/checked.png")));
                }
                else
                {
                    Lbl_DO0.Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/gcp_Wpf;component/Resources/unchecked.png")));
                }
                if (gClass.str.DioPacket[gClass.srmNum].DO[1] > 0)
                {
                    Lbl_DO1.Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/gcp_Wpf;component/Resources/checked.png")));
                }
                else
                {
                    Lbl_DO1.Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/gcp_Wpf;component/Resources/unchecked.png")));
                }
                if (gClass.str.DioPacket[gClass.srmNum].DO[2] > 0)
                {
                    Lbl_DO2.Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/gcp_Wpf;component/Resources/checked.png")));
                }
                else
                {
                    Lbl_DO2.Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/gcp_Wpf;component/Resources/unchecked.png")));
                }
                if (gClass.str.DioPacket[gClass.srmNum].DO[3] > 0)
                {
                    Lbl_DO3.Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/gcp_Wpf;component/Resources/checked.png")));
                }
                else
                {
                    Lbl_DO3.Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/gcp_Wpf;component/Resources/unchecked.png")));
                }
                if (gClass.str.DioPacket[gClass.srmNum].DO[4] > 0)
                {
                    Lbl_DO4.Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/gcp_Wpf;component/Resources/checked.png")));
                }
                else
                {
                    Lbl_DO4.Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/gcp_Wpf;component/Resources/unchecked.png")));
                }
                if (gClass.str.DioPacket[gClass.srmNum].DO[5] > 0)
                {
                    Lbl_DO5.Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/gcp_Wpf;component/Resources/checked.png")));
                }
                else
                {
                    Lbl_DO5.Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/gcp_Wpf;component/Resources/unchecked.png")));
                }
                if (gClass.str.DioPacket[gClass.srmNum].DO[6] > 0)
                {
                    Lbl_DO6.Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/gcp_Wpf;component/Resources/checked.png")));
                }
                else
                {
                    Lbl_DO6.Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/gcp_Wpf;component/Resources/unchecked.png")));
                }
                if (gClass.str.DioPacket[gClass.srmNum].DO[7] > 0)
                {
                    Lbl_DO7.Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/gcp_Wpf;component/Resources/checked.png")));
                }
                else
                {
                    Lbl_DO7.Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/gcp_Wpf;component/Resources/unchecked.png")));
                }
            });
        }
    }
}
