using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using gcp_Wpf.commClass;
using gcp_Wpf.MenuWindow;
using System.Runtime.InteropServices;
using System.Timers;
using System.Windows.Interop;
using System.IO;

namespace gcp_Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 


    public partial class MainWindow : Window
    {
        [DllImport("kernel32.dll")]
        private static extern bool FreeConsole();


        public static int watchDogCount = 0;

        //General Var
        public int testVar;
        double dpiX, dpiY;
        int craneCnt;

        //Gcp UdpClient
        private udpClientClass[] srmComm;
        //Wcs TcpServer
        private tcpServerClass[] hostComm;
        // Modbus Rtu DIO
        private modbusRtuClass[] rtuComm;
        //Serve Server
        private tcpServerClass servServer;

        private tcpClientClass tcpClient;

        //Test Timer
        Timer myTimer = new Timer();

        // Menu
        Menu_Setting menu_Setting;


        // Host Log Window 생성
        WindowSrmLog win_HostLog1 = new WindowSrmLog(1, 1);
        WindowSrmLog win_HostLog2 = new WindowSrmLog(1, 2);
        WindowSrmLog win_HostLog3 = new WindowSrmLog(1, 3);

        // SRM Log Window 생성
        WindowSrmLog win_SrmLog1 = new WindowSrmLog(2, 1);
        WindowSrmLog win_SrmLog2 = new WindowSrmLog(2, 2);
        WindowSrmLog win_SrmLog3 = new WindowSrmLog(2, 3);

        // DIO Log Window 생성
        WindowSrmLog win_DioLog1 = new WindowSrmLog(3, 1);
        WindowSrmLog win_DioLog2 = new WindowSrmLog(3, 2);
        WindowSrmLog win_DioLog3 = new WindowSrmLog(3, 3);

        // Dev State Page
        public PageDevState pageDev1 = new PageDevState(1);
        public PageDevState pageDev2 = new PageDevState(2);
        public PageDevState pageDev3 = new PageDevState(3);

        // Page Define
        PageMain pageMain;
        PageManual pageManual;
        PageAuto pageAuto;
        PageProhibitRack pageProhRack;
        PageCommSet pageCommSet;
        PageCraneSet pageCraneSet;
        PageStationSet pageStationSet;
        PageSemiAuto pageSemiAuto;
        PageAlarmLog pageAlarmLog;
        PageDIO pageDio;

        //Frm_Manual.Source = new Uri("PageManual.xaml", UriKind.Relative);
        //Frm_ProhRack.Source = new Uri("PageProhibitRack.xaml", UriKind.Relative);
        //Frm_CommSet.Source = new Uri("PageCommSet.xaml", UriKind.Relative);
        //Frm_CraneSet.Source = new Uri("PageCraneSet.xaml", UriKind.Relative);
        //Frm_StationSet.Source = new Uri("PageStationSet.xaml", UriKind.Relative);
        //Frm_SemiAuto.Source = new Uri("PageSemiAuto.xaml", UriKind.Relative);
        //Frm_AlarmList.Source = new Uri("PageAlarmLog.xaml", UriKind.Relative);

        Point m_position;

        // Set the Source property to the URI of the Page to display

        //Singletone
        singletonClass gClass;

        public MainWindow()
        {
            // SingleTone Test
            gClass = singletonClass.Instance;
            string curDir = System.IO.Path.Combine(Environment.CurrentDirectory, cConstDefine.PATH_LOG, cConstDefine.PATH_SRMLOG);
            //string pathString = AppDomain.CurrentDomain.BaseDirectory + cConstDefine.PATH_SRMLOG;
            Console.WriteLine("CURPATH - " + curDir);

            InitializeComponent();

            myTimer.Interval = 1000; // 1 second
            myTimer.AutoReset = true; // Repeat the timer
            myTimer.Elapsed += TestTimer_Elapsed;

            menu_Setting = new Menu_Setting(this);
            menu_Setting.Hide();

            myTimer.Start();

            dpiX = VisualTreeHelper.GetDpi(this).PixelsPerInchX;
            dpiY = VisualTreeHelper.GetDpi(this).PixelsPerInchY;

            Btn_Main.Click += Mode_Click;
            Btn_Auto.Click += Mode_Click;
            Btn_Manual.Click += Mode_Click;
            Btn_SemiAuto.Click += Mode_Click;

            // UI만 표시  --- Test

            // INI 파일 설정 정보 공유 구조체로 초기화  (순서 중요)  language
            gClass.str.GcpInfo.srmCount = int.Parse(cIniAccess.Read(AppDomain.CurrentDomain.BaseDirectory + "\\Config\\Config.ini", "GCPINFO", "CraneCount", "1"));
            gClass.str.GcpInfo.language = int.Parse(cIniAccess.Read(AppDomain.CurrentDomain.BaseDirectory + "\\Config\\Config.ini", "GCPINFO", "LanguageIdx", "0"));
            craneCnt = gClass.str.GcpInfo.srmCount;

            string tmpPath;
            for (int i=0; i< craneCnt; i++)
            {
                // Config.ini
                gClass.str.SrmInfo[i].srmID = int.Parse(cIniAccess.Read(AppDomain.CurrentDomain.BaseDirectory + "\\Config\\Config.ini", "SRMINFO_" + i, "SRMID"));
                gClass.str.SrmInfo[i].srmType = int.Parse(cIniAccess.Read(AppDomain.CurrentDomain.BaseDirectory + "\\Config\\Config.ini", "SRMINFO_" + i, "SRMTYPE"));
                gClass.str.SrmInfo[i].forkType = int.Parse(cIniAccess.Read(AppDomain.CurrentDomain.BaseDirectory + "\\Config\\Config.ini", "SRMINFO_" + i, "FORKTYPE"));


                gClass.str.SrmInfo[i].row = int.Parse(cIniAccess.Read(AppDomain.CurrentDomain.BaseDirectory + "\\Config\\Config.ini", "RACKINFO_" +i, "MaxRow"));
                gClass.str.SrmInfo[i].bay = int.Parse(cIniAccess.Read(AppDomain.CurrentDomain.BaseDirectory + "\\Config\\Config.ini", "RACKINFO_" +i, "Maxbay"));
                gClass.str.SrmInfo[i].lev = int.Parse(cIniAccess.Read(AppDomain.CurrentDomain.BaseDirectory + "\\Config\\Config.ini", "RACKINFO_" +i, "MaxLev"));
                gClass.str.SrmInfo[i].stn = int.Parse(cIniAccess.Read(AppDomain.CurrentDomain.BaseDirectory + "\\Config\\Config.ini", "RACKINFO_" +i, "MaxStn"));


                gClass.str.SrmInfo[i].srmIP = cIniAccess.Read(AppDomain.CurrentDomain.BaseDirectory + "\\Config\\Config.ini", "SRMCOMM_" + i, "SRMIP");
                gClass.str.SrmInfo[i].srmPORT = int.Parse(cIniAccess.Read(AppDomain.CurrentDomain.BaseDirectory + "\\Config\\Config.ini", "SRMCOMM_" + i, "SRMPORT"));

                gClass.str.SrmInfo[i].hostPORT = int.Parse(cIniAccess.Read(AppDomain.CurrentDomain.BaseDirectory + "\\Config\\Config.ini", "HOSTCOMM_" + i, "HOSTPORT"));

                gClass.str.SrmInfo[i].comPORT = cIniAccess.Read(AppDomain.CurrentDomain.BaseDirectory + "\\Config\\Config.ini", "DIOCOMM_" + i, "COMPORT");
                gClass.str.SrmInfo[i].baudRate = int.Parse(cIniAccess.Read(AppDomain.CurrentDomain.BaseDirectory + "\\Config\\Config.ini", "DIOCOMM_" + i, "BAUDRATE"));
                gClass.str.SrmInfo[i].parity = int.Parse(cIniAccess.Read(AppDomain.CurrentDomain.BaseDirectory + "\\Config\\Config.ini", "DIOCOMM_" + i, "PARITY"));
                gClass.str.SrmInfo[i].dataBit = int.Parse(cIniAccess.Read(AppDomain.CurrentDomain.BaseDirectory + "\\Config\\Config.ini", "DIOCOMM_" + i, "DATABIT"));
                gClass.str.SrmInfo[i].stopBit = int.Parse(cIniAccess.Read(AppDomain.CurrentDomain.BaseDirectory + "\\Config\\Config.ini", "DIOCOMM_" + i, "STOPBIT"));

                // ProhRack
                tmpPath = System.IO.Path.Combine(Environment.CurrentDirectory, "SRM" + craneCnt, cConstDefine.PATH_PRHRACK);
                if (!Directory.Exists(tmpPath))
                {
                    Directory.CreateDirectory(tmpPath);
                    Console.WriteLine("Folder created at: " + tmpPath);
                }
            }


            // Page 객체 생성
            pageMain = new PageMain();
            pageManual = new PageManual();
            pageAuto = new PageAuto();
            pageProhRack = new PageProhibitRack();
            pageCommSet = new PageCommSet();
            pageCraneSet = new PageCraneSet();
            pageStationSet = new PageStationSet();
            pageSemiAuto = new PageSemiAuto();
            pageAlarmLog = new PageAlarmLog();
            pageDio = new PageDIO();

            Frm_Page.Content = pageMain;

            for (int i = 1; i <= craneCnt; i++)
            {
                Combo_srmNum.Items.Add("SRM #" + i.ToString());
            }

            if (Combo_srmNum.Items.Count > 0)
            {
                Combo_srmNum.SelectedIndex = 0;
            }

            // SRM <-> GCP   Udp Connection Init
            srmComm = new udpClientClass[craneCnt];
            // HOST <-> GCP   Tcp Connection Init
            hostComm = new tcpServerClass[craneCnt];
            // DIO <-> GCP    Modbus RTU Connection Init
            rtuComm = new modbusRtuClass[craneCnt];
            for (int i = 0; i < craneCnt; i++)
            {
                srmComm[i] = new udpClientClass(i);
                srmComm[i].connect(gClass.str.SrmInfo[i].srmIP, gClass.str.SrmInfo[i].srmPORT);

                hostComm[i] = new tcpServerClass(i, gClass.str.SrmInfo[i].hostPORT);

                rtuComm[i] = new modbusRtuClass(i, gClass.str.SrmInfo[i].comPORT, gClass.str.SrmInfo[i].baudRate, gClass.str.SrmInfo[i].parity, gClass.str.SrmInfo[i].dataBit, gClass.str.SrmInfo[i].stopBit);
            }

            //rtuComm[0] = new modbusRtuClass(0, gClass.str.SrmInfo[0].comPort, gClass.str.SrmInfo[0].baudRate, gClass.str.SrmInfo[0].parity, gClass.str.SrmInfo[0].dataBit, gClass.str.SrmInfo[0].stopBit);

            //tcpClient = new tcpClientClass("127.0.0.1", 5000);


            foreach (var window in Application.Current.Windows)
            {
                Console.WriteLine("ClassNmae : " + window.GetType().Name);

                if (window.GetType().Name == "WindowSrmLog1")
                {
                    //(window.GetType().
                    //Console.WriteLine("찾았다!!!");
                    // Found the window!
                }
            }
        }

        ~MainWindow()
        {
            Console.WriteLine("Destructor MainWindow Delete");
        }

        private void TestTimer_Elapsed(object sender, ElapsedEventArgs e)
        {

            // Execute the code here
            //IntPtr WindowToFind = cConstDefine.FindWindow(null, "WindowSrmLog" + (gClass.srmNum + 1));
            //if (WindowToFind != IntPtr.Zero)
            //{
            //    //Console.WriteLine("Send Message " + WindowToFind);
            //    string message = DateTime.Now.ToString("hh:mm:ss:FFF -") + "Hello, World!";
            //    IntPtr hwnd = WindowToFind;
            //    var copyData = new cConstDefine.COPYDATASTRUCT();
            //    copyData.dwData = IntPtr.Zero;
            //    copyData.lpData = message;
            //    copyData.cbData = Encoding.Unicode.GetBytes(message).Length + 1; // add 1 for null-terminator
            //    cConstDefine.SendMessage(WindowToFind, cConstDefine.WM_USER, IntPtr.Zero, ref copyData);               // Send - Post 차이 비교 필요
            //    //PostMessage(WindowToFind, cConstDefine.WM_USER, IntPtr.Zero, ref copyData);
            //}
            //else
            //{
            //    Console.WriteLine("Find Srm Window Fail " + WindowToFind);
            //}


            // 메인 스레드 종료여부 확인 워치독
            if (watchDogCount > 255)
            {
                watchDogCount = 0;
            }
            else
            {
                watchDogCount += 1;
            }
            
        }

        public void Page_Change(int pageIdx)
        {
            Btn_Main.IsChecked = false;
            Btn_Auto.IsChecked = false;
            Btn_Manual.IsChecked = false;
            Btn_SemiAuto.IsChecked = false;
            switch (pageIdx)
            {
                case 0:
                    Frm_Page.Content = pageAuto;
                    //Frm_Page.Source = new Uri("PageAuto.xaml", UriKind.Relative);
                    Btn_Auto.IsChecked = true;
                    break;
                case 1:
                    Frm_Page.Content = pageManual;
                    Btn_Manual.IsChecked = true;
                    break;
                case 2:
                    Frm_Page.Content = pageSemiAuto;
                    Btn_SemiAuto.IsChecked = true;
                    break;
                case 3:
                    Frm_Page.Content = pageProhRack;
                    break;
                case 4:
                    Frm_Page.Content = pageCommSet;
                    break;
                case 5:
                    Frm_Page.Content = pageCraneSet;
                    break;
                case 6:
                    Frm_Page.Content = pageStationSet;
                    break;
                case 7:
                    Frm_Page.Content = pageAlarmLog;
                    break;
                case 8:
                    Frm_Page.Content = pageMain;
                    Btn_Main.IsChecked = true;
                    break;
                case 9:
                    Frm_Page.Content = pageDio;
                    break;
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            // Handle the button click event here
            this.Close();
        }

        private void Mode_Click(object sender, RoutedEventArgs e)
        {
            // Handle the button click event here
            ToggleButton toggleButton = sender as ToggleButton;

            if (toggleButton.IsChecked == true)
            {
                if (toggleButton == Btn_Auto)
                {
                    // Button Mode is Auto
                    Page_Change(0);
                }
                else if (toggleButton == Btn_Manual)
                {
                    // Button Mode is Manual
                    Page_Change(1);
                }
                else if (toggleButton == Btn_SemiAuto)
                {
                    // Button Mode is SemiAuto
                    Page_Change(2);
                }
                else
                {
                    // Button Mode is Main
                    Page_Change(8);
                }
            }
            else
            {
                toggleButton.IsChecked = true;
            }
            

            Console.WriteLine("Get Mode Button Click Event");
        }


        private void SRM_COM_Click(object sender, RoutedEventArgs e)
        {
            SrmLogWindowChanged();
        }

        private void SrmLogWindowChanged()
        {
            win_SrmLog1.Hide();
            win_SrmLog2.Hide();
            win_SrmLog3.Hide();
            switch (gClass.srmNum)
            {
                case 0:
                    win_SrmLog1.Show();
                    break;
                case 1:
                    win_SrmLog2.Show();
                    break;
                case 2:
                    win_SrmLog3.Show();
                    break;
            }
        }

        private void DIO_COM_Click(object sender, RoutedEventArgs e)
        {
            DioLogWindowChanged();
        }

        private void DioLogWindowChanged()
        {
            win_DioLog1.Hide();
            win_DioLog2.Hide();
            win_DioLog3.Hide();
            switch (gClass.srmNum)
            {
                case 0:
                    win_DioLog1.Show();
                    break;
                case 1:
                    win_DioLog2.Show();
                    break;
                case 2:
                    win_DioLog3.Show();
                    break;
            }
        }

        private void WCS_COM_Click(object sender, RoutedEventArgs e)
        {
            HostLogWindowChanged();
        }

        private void HostLogWindowChanged()
        {
            win_HostLog1.Hide();
            win_HostLog2.Hide();
            win_HostLog3.Hide();
            switch (gClass.srmNum)
            {
                case 0:
                    win_HostLog1.Show();
                    break;
                case 1:
                    win_HostLog2.Show();
                    break;
                case 2:
                    win_HostLog3.Show();
                    break;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            myTimer.Stop();
            win_HostLog1.Finished();
            win_HostLog2.Finished();
            win_HostLog3.Finished();

            win_SrmLog1.Finished();
            win_SrmLog2.Finished();
            win_SrmLog3.Finished();

            win_DioLog1.Finished();
            win_DioLog2.Finished();
            win_DioLog3.Finished();

            for (int i = 0; i < craneCnt; i++)
            {
                if (srmComm[i] != null)
                {
                    srmComm[i].Close();
                }

                if (hostComm[i] != null)
                {
                    hostComm[i].Close();
                }

                if (rtuComm[i] != null)
                {
                    rtuComm[i].Close();
                }

            }
            Console.WriteLine("MainWindow_Closing");

            Application.Current.Shutdown();

            //IntPtr hwnd = GetConsoleWindow();
            //if (hwnd != IntPtr.Zero)
            //{
            //    // Hide the console window
            //    ShowWindow(hwnd, 0);
            //}
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            //FreeConsole();
        }

        private void btn_log_Click(object sender, RoutedEventArgs e)
        {
            Page_Change(7);
        }

        private void Combo_srmNum_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Console.WriteLine("Combo_srmNum_SelectionChanged");
            win_HostLog1.Hide();
            win_HostLog2.Hide();
            win_HostLog3.Hide();
            win_SrmLog1.Hide();
            win_SrmLog2.Hide();
            win_SrmLog3.Hide();
            win_DioLog1.Hide();
            win_DioLog2.Hide();
            win_DioLog3.Hide();


            gClass.srmNum = Combo_srmNum.SelectedIndex;
            switch (Combo_srmNum.SelectedIndex)
            {
                case 0:
                    Frm_State.Content = pageDev1;
                    pageMain.craneDisplay(ref pageDev2, ref pageDev3);
                    break;
                case 1:
                    Frm_State.Content = pageDev2;
                    pageMain.craneDisplay(ref pageDev1, ref pageDev3);
                    break;
                case 2:
                    Frm_State.Content = pageDev3;
                    pageMain.craneDisplay(ref pageDev1, ref pageDev2);
                    break;
            }

            //HostLogWindowChanged();
            //DioLogWindowChanged();
            //SrmLogWindowChanged();
        }

        private void Btn_IO_Click(object sender, RoutedEventArgs e)
        {
            Page_Change(9);
        }

        private void Btn_Setting_Click(object sender, RoutedEventArgs e)
        {

            //menu_Setting
            Point position = Btn_Setting.PointToScreen(new Point(0, 0));
            Point m_position = this.PointToScreen(new Point(0, 0));

            Console.WriteLine("Get Screen Position " + SystemParameters.PrimaryScreenWidth + " " + SystemParameters.PrimaryScreenHeight + " " + m_position.X + " " + m_position.Y + " " + this.Width);

            menu_Setting.Left = position.X / dpiX * 96.0 - 120;
            menu_Setting.Top = position.Y / dpiY * 96.0 - 80;
            menu_Setting.Show();
            Console.WriteLine("Position {0} {1} = {2} {3} {4} {5}", position, m_position , menu_Setting.Left , menu_Setting.Top, dpiX, dpiY);
        }
    }
}
