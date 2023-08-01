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
    /// PageManual.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class PageManual : Page
    {
        //Test Timer
        Timer myTimer = new Timer();

        int fmove=0;

        //Singletone
        singletonClass gClass;
        public PageManual()
        {
            InitializeComponent();

            gClass = singletonClass.Instance;

            myTimer.Interval = 1000; // 1 second
            myTimer.AutoReset = true; // Repeat the timer
            myTimer.Elapsed += TestTimer_Elapsed;
            myTimer.Start();

            // 개별 그리드 Width 조절
            //gridFork.ColumnDefinitions[3].Width = new GridLength(0);

            //lbl_BMove.Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/gcp_Wpf;component/Resources/moveRC.png")));
            //lbl_FMove.Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/gcp_Wpf;component/Resources/moveLC.png")));
        }

        private void TestTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            //Console.WriteLine("TestTimer_Elapsed Page Manual");
            // Execute the code here
            Dispatcher.Invoke(() => {
                if (fmove == 0)
                {
                    fmove = 1;
                    lbl_BMove.Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/gcp_Wpf;component/Resources/moveR.png")));
                    lbl_FMove.Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/gcp_Wpf;component/Resources/moveL.png")));
                }
                else if(fmove == 1)
                {
                    fmove = 0;
                    lbl_BMove.Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/gcp_Wpf;component/Resources/moveRC.png")));
                    lbl_FMove.Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/gcp_Wpf;component/Resources/moveLC.png")));
                }
            });

            Console.WriteLine("MANUAL BUTTON STATE : "+ gClass.srmNum + " - " + gClass.str.SrmPacket[gClass.srmNum].manClicked);
        }



        // 수동 제어 버튼 이벤트 처리 함수
        private void Button_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Button btnDown = sender as Button;

            for (int i = 0; i < 3; i++) { 
                if(i == gClass.srmNum)
                {
                    gClass.str.SrmPacket[i].manClicked = true;          // 수동 버튼 클릭 플래그 ON
                }
                else
                {
                    gClass.str.SrmPacket[i].manClicked = false;          // 현재 선택한 호기 외 OFF
                }
            }

            // 주행 동작 버튼---------------------------------------------------------
            if (btnDown.Name == "fastF")
            {
                gClass.str.SrmPacket[gClass.srmNum].manCmd = cConstDefine.TRAV_FW_FAST;
                gClass.str.SrmPacket[gClass.srmNum].manAxis = 128;
                gClass.str.SrmPacket[gClass.srmNum].manTrav = 11;
                Console.WriteLine("Pressed TRAV_FW_FAST");
            }
            else if (btnDown.Name == "slowF")
            {
                gClass.str.SrmPacket[gClass.srmNum].manCmd = cConstDefine.TRAV_FW_SLOW;
                gClass.str.SrmPacket[gClass.srmNum].manAxis = 128;
                gClass.str.SrmPacket[gClass.srmNum].manTrav = 1;
                Console.WriteLine("Pressed TRAV_FW_SLOW");
            }
            else if (btnDown.Name == "fastB")
            {
                gClass.str.SrmPacket[gClass.srmNum].manCmd = cConstDefine.TRAV_BW_FAST;
                gClass.str.SrmPacket[gClass.srmNum].manAxis = 128;
                gClass.str.SrmPacket[gClass.srmNum].manTrav = 12;
                Console.WriteLine("Pressed TRAV_BW_FAST");
            }
            else if (btnDown.Name == "slowB")
            {
                gClass.str.SrmPacket[gClass.srmNum].manCmd = cConstDefine.TRAV_BW_SLOW;
                gClass.str.SrmPacket[gClass.srmNum].manAxis = 128;
                gClass.str.SrmPacket[gClass.srmNum].manTrav = 2;
                Console.WriteLine("Pressed TRAV_BW_SLOW");
            }

            // 승강 동작 버튼---------------------------------------------------------
            else if (btnDown.Name == "fastU")
            {
                gClass.str.SrmPacket[gClass.srmNum].manCmd = cConstDefine.LIFT_UP_FAST;
                gClass.str.SrmPacket[gClass.srmNum].manAxis = 64;
                gClass.str.SrmPacket[gClass.srmNum].manLift = 11;
                Console.WriteLine("Pressed LIFT_UP_FAST");
            }
            else if (btnDown.Name == "slowU")
            {
                gClass.str.SrmPacket[gClass.srmNum].manCmd = cConstDefine.LIFT_UP_SLOW;
                gClass.str.SrmPacket[gClass.srmNum].manAxis = 64;
                gClass.str.SrmPacket[gClass.srmNum].manLift = 1;
                Console.WriteLine("Pressed LIFT_UP_SLOW");
            }
            else if (btnDown.Name == "fastD")
            {
                gClass.str.SrmPacket[gClass.srmNum].manCmd = cConstDefine.LIFT_DW_FAST;
                gClass.str.SrmPacket[gClass.srmNum].manAxis = 64;
                gClass.str.SrmPacket[gClass.srmNum].manLift = 12;
                Console.WriteLine("Pressed LIFT_DW_FAST");
            }
            else if (btnDown.Name == "slowD")
            {
                gClass.str.SrmPacket[gClass.srmNum].manCmd = cConstDefine.LIFT_DW_SLOW;
                gClass.str.SrmPacket[gClass.srmNum].manAxis = 64;
                gClass.str.SrmPacket[gClass.srmNum].manLift = 2;
                Console.WriteLine("Pressed LIFT_DW_SLOW");
            }

            // 포크 동작 버튼---------------------------------------------------------
            else if (btnDown.Name == "slowL")
            {
                gClass.str.SrmPacket[gClass.srmNum].manCmd = cConstDefine.FORK_MOVE_L;
                gClass.str.SrmPacket[gClass.srmNum].manAxis = 32;
                gClass.str.SrmPacket[gClass.srmNum].manFork1 = 2;
                Console.WriteLine("Pressed FORK_MOVE_L");
            }
            else if (btnDown.Name == "slowR")
            {
                gClass.str.SrmPacket[gClass.srmNum].manCmd = cConstDefine.FORK_MOVE_R;
                gClass.str.SrmPacket[gClass.srmNum].manAxis = 32;
                gClass.str.SrmPacket[gClass.srmNum].manFork2 = 3;
                Console.WriteLine("Pressed FORK_MOVE_R");
            }


            Console.WriteLine("Button_PreviewMouseDown");
        }

        private void Button_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            Button btnUp = sender as Button;

            gClass.str.SrmPacket[gClass.srmNum].manClicked = false;          // 현재 선택한 호기 OFF
            gClass.str.SrmPacket[gClass.srmNum].manCmd = cConstDefine.MAN_BTN_NONE;         //  MANUAL BUTTON NONE
            gClass.str.SrmPacket[gClass.srmNum].manAxis = 0;
            gClass.str.SrmPacket[gClass.srmNum].manTrav = 0;
            gClass.str.SrmPacket[gClass.srmNum].manLift = 0;
            gClass.str.SrmPacket[gClass.srmNum].manFork1 = 0;
            gClass.str.SrmPacket[gClass.srmNum].manFork2 = 0;

            Console.WriteLine("Button_PreviewMouseUp");
        }
    }
}
