using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Interop;
using System.Runtime.InteropServices;
using System.IO;
using System.Data;

namespace gcp_Wpf.MenuWindow
{
    /// <summary>
    /// WindowSrmLog.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class WindowSrmLog : Window
    {
        private HwndSource hwndSource;

        ObservableCollection<String> logStrList = new ObservableCollection<String>();
        Timer myTimer = new Timer();
        static System.Threading.Mutex mutex = new System.Threading.Mutex();
        int srmNum;
        string pathString;

        //Singletone
        singletonClass gClass;
        public WindowSrmLog()
        {
            InitializeComponent();
        }

        public WindowSrmLog(int type, int srmNum)               // 클래스 공유 사용  - type = 1,2,3   1= HOST-GCP, 2= SRM-GCP, 3= DIO-GCP
        {
            InitializeComponent();
            this.srmNum = srmNum;
            string strTitle = "";
            switch (type)
            {
                case 1:
                    pathString = System.IO.Path.Combine(Environment.CurrentDirectory, "SRM" + srmNum, cConstDefine.PATH_LOG, cConstDefine.PATH_HOSTLOG);
                    strTitle = "WindowHostLog";
                    break;
                case 2:
                    pathString = System.IO.Path.Combine(Environment.CurrentDirectory, "SRM" + srmNum, cConstDefine.PATH_LOG, cConstDefine.PATH_SRMLOG);
                    strTitle = "WindowSrmLog";
                    break;
                case 3:
                    pathString = System.IO.Path.Combine(Environment.CurrentDirectory, "SRM" + srmNum, cConstDefine.PATH_LOG, cConstDefine.PATH_DIOLOG);
                    strTitle = "WindowDioLog";
                    break;
            }
            this.Title = strTitle + srmNum;

            gClass = singletonClass.Instance;
            gClass.str.test1 = 10;
            //gClass.Name = "test";

            Console.WriteLine("Get SingleTone Class with WindowSrmLog = " + gClass.test);

            //---------------------Invisible------------------------------
            WindowState initialWindowState = WindowState;

            // making window invisible
            ShowInTaskbar = false;
            WindowState = WindowState.Minimized;

            // showing and hiding window
            Visibility = Visibility.Visible;
            Visibility = Visibility.Hidden;

            WindowState = initialWindowState;
            //------------------------------------------------------------


            myTimer.Interval = 1000; // 1 second
            myTimer.AutoReset = true; // Repeat the timer
            myTimer.Elapsed += LogTimer_Elapsed;
            myTimer.Start();
            //this.Hide();

            List_SrmLog.ItemsSource = logStrList;
            //logStrList.CollectionChanged += Log_CollectionChanged;

            Console.WriteLine("Log SRM Init");
        }

        ~WindowSrmLog() {
            Console.WriteLine("WindowSrmLog Delete");
            Finished();
        }

        public void Finished()
        {
            myTimer.Stop();
            this.Close ();
        }

        private void Log_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Count > 0)
            {
                var lastItem = e.NewItems[e.NewItems.Count - 1];
                List_SrmLog.ScrollIntoView(lastItem);
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            myTimer.Stop();
            e.Cancel = true; // Cancel the close
            this.Hide(); // Hide the window
        }

        private void Window_FormShow(object sender, EventArgs e)
        {
            Console.WriteLine("Window_FormShow SrmLog");
            hwndSource = HwndSource.FromHwnd(new WindowInteropHelper(this).Handle);
            hwndSource.AddHook(WndProc);
        }

        private void LogTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            gClass.str.test2 += 1;
            //Console.WriteLine("Get SingleTone Class with WindowSrmLog = " + gClass.test + " " + gClass.str.test1);
            if(gClass.str.test1 > 20)
            {
                gClass.str.test1 = 1;
                //gClass.SharedData.test1 = 0;
            }
            // Execute the code here
            //Console.WriteLine("Timer Elapsed " + logStrList.Count);
            //Dispatcher.Invoke(() => logList.Add(new Item { Text = DateTime.Now.ToString("hh:mm:ss tt")}));
            //Dispatcher.Invoke(() => logStrList.Add(DateTime.Now.ToString("hh:mm:ss tt")));
            //List_SrmLog.ScrollIntoView(List_SrmLog.Items[List_SrmLog.Items.Count - 1]);

        }

        private void Btn_AlwaysTop_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton toggleButton = sender as ToggleButton;

            if (toggleButton.IsChecked == true)
            {
                Topmost = true;
            }
            else
            {
                Topmost = false;
            }
        }

        private void Btn_Stop_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton toggleButton = sender as ToggleButton;

            if (toggleButton.IsChecked == true)
            {
            }
            else
            {
            }
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == cConstDefine.WM_USER)
            {
                // Handle the message here
                var data = (cConstDefine.COPYDATASTRUCT)Marshal.PtrToStructure(lParam, typeof(cConstDefine.COPYDATASTRUCT));            // 단순하게 바꾸자
                string message = data.lpData;

                // Handle the message here
                if (this.IsVisible)
                {
                    if (Btn_Stop.IsChecked == false)
                    {
                        Console.WriteLine("SendMessage Called On Visible " + cConstDefine.WM_USER);
                        Dispatcher.Invoke(() => logStrList.Add(DateTime.Now.ToString("hh:mm:ss:FFF ") + message));
                        if(logStrList.Count > 255)
                        {
                            logStrList.RemoveAt(0);
                        }
                        List_SrmLog.ScrollIntoView(List_SrmLog.Items[List_SrmLog.Items.Count - 1]);
                    }
                }

                //SaveLogFile(DateTime.Now.ToString("hh:mm:ss ") + message);

                handled = true;
                return IntPtr.Zero;
            }
            else
            {
                //Console.WriteLine("ANOTHER Called");
                return IntPtr.Zero;
            }
        }


        private async void SaveLogFile(string text)
        {
            await Task.Run(() =>
            {
                mutex.WaitOne();

                if (!Directory.Exists(pathString))
                {
                    Directory.CreateDirectory(pathString);
                    Console.WriteLine("Folder created at: " + pathString);
                }


                string filePath = System.IO.Path.Combine(pathString,"SRMLOG_" + DateTime.Now.ToString("yyyyMMdd") + ".log");

                if (!File.Exists(filePath))
                {
                    using (StreamWriter writer = File.CreateText(filePath))
                    {
                        writer.WriteLine("File created on " + DateTime.Now.ToString());
                    }
                }

                // Write the text to the file
                using (StreamWriter writer = new StreamWriter(filePath, true))
                {
                    writer.WriteLine(text);
                }

                mutex.ReleaseMutex();
            });
        }
    }
}
