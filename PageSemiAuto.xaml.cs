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

namespace gcp_Wpf
{
    /// <summary>
    /// PageSemiAuto.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class PageSemiAuto : Page
    {
        //Singletone
        singletonClass gClass;
        public PageSemiAuto()
        {
            InitializeComponent();
            gClass = singletonClass.Instance;
            Edit_Fk1FromLev.IsEnabled = true;
        }

        public void Page_Init()         // 페이지 전환 시 마다 호출
        {
            //  현재 선택한 크레인의 포크타입에 따라 화면 사용설정
            if (gClass.str.SrmInfo[gClass.srmNum].forkType == 0)            // 싱글포크        
            {
                Bdr_Fork2.IsEnabled = false;
                Lbl_Fork2Sel.Background = null;
                Bdr_Fork2.IsEnabled = false;
                if (gClass.str.SrmInfo[gClass.srmNum].bUse_fork1)           // 포크 사용 선택에 따라서 버튼 이미지 전환
                {
                    Lbl_Fork1Sel.Background = Brushes.White;
                }
                else
                {
                    Lbl_Fork1Sel.Background = null;
                }
            }
            else
            {
                Bdr_Fork2.IsEnabled = true;                                 // 트윈포크
                Lbl_Fork2Sel.Background = Brushes.White;
                Bdr_Fork2.IsEnabled = true;
                if (gClass.str.SrmInfo[gClass.srmNum].bUse_fork1)           // 포크 사용 선택에 따라서 버튼 이미지 전환
                {
                    Lbl_Fork1Sel.Background = Brushes.White;
                }
                else
                {
                    Lbl_Fork1Sel.Background = null;
                }
                if (gClass.str.SrmInfo[gClass.srmNum].bUse_fork2)
                {
                    Lbl_Fork2Sel.Background = Brushes.White;
                }
                else
                {
                    Lbl_Fork2Sel.Background = null;
                }
            }
        }
        private void Btn_CommandClick(object sender, RoutedEventArgs e)
        {
            Button cmdButton = sender as Button;

            if (cmdButton.Name == "Btn_Move")
            {
                Console.WriteLine("Click Btn_Move");
                if (gClass.str.SrmInfo[gClass.srmNum].bUse_fork1)           // 포크 사용 선택에 R/B/L Enable 설정
                {
                    Edit_Fk1FromStn.IsEnabled = false;
                    Edit_Fk1FromRow.IsEnabled = false;
                    Edit_Fk1FromBay.IsEnabled = false;
                    Edit_Fk1FromLev.IsEnabled = false;
                    Edit_Fk1ToStn.IsEnabled = true;
                    Edit_Fk1ToRow.IsEnabled = false;
                    Edit_Fk1ToBay.IsEnabled = true;
                    Edit_Fk1ToLev.IsEnabled = true;
                }

                if (gClass.str.SrmInfo[gClass.srmNum].bUse_fork2)           // 포크 사용 선택에 R/B/L Enable 설정
                {
                    Edit_Fk2FromStn.IsEnabled = false;
                    Edit_Fk2FromRow.IsEnabled = false;
                    Edit_Fk2FromBay.IsEnabled = false;
                    Edit_Fk2FromLev.IsEnabled = false;
                    Edit_Fk2ToStn.IsEnabled = true;
                    Edit_Fk2ToRow.IsEnabled = false;
                    Edit_Fk2ToBay.IsEnabled = true;
                    Edit_Fk2ToLev.IsEnabled = true;
                }
            }
            else if (cmdButton.Name == "Btn_Store")
            {
                Console.WriteLine("Click Btn_Store");
            }
            else if (cmdButton.Name == "Btn_Retrieve")
            {
                Console.WriteLine("Click Btn_Retrieve");
            }
            else if (cmdButton.Name == "Btn_RtoR")
            {
                Console.WriteLine("Click Btn_RtoR");
            }
            else if (cmdButton.Name == "Btn_StoS")
            {
                Console.WriteLine("Click Btn_StoS");
            }
        }

        private void Btn_FromTo_Select(object sender, MouseButtonEventArgs e)
        {
            TextBox edit = sender as TextBox;
            edit.PointToScreen(new Point(0, 0));
            MenuWindow.NumPad tmpNumPad = new();

            tmpNumPad.open_RelativePos(edit);
            Console.WriteLine("Btn_FromTo_Select : " + edit.Name);
        }

        private void Lbl_Fork1Sel_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (gClass.str.SrmInfo[gClass.srmNum].bUse_fork1)           // 포크 사용 선택에 따라서 버튼 이미지 전환
            {
                gClass.str.SrmInfo[gClass.srmNum].bUse_fork1 = false;
                Lbl_Fork1Sel.Background = null;
            }
            else
            {
                gClass.str.SrmInfo[gClass.srmNum].bUse_fork1 = true;
                Lbl_Fork1Sel.Background = Brushes.White;
            }
            Console.WriteLine("Lbl_Fork1Sel_PreviewMouseDown");
        }

        private void Lbl_Fork2Sel_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (gClass.str.SrmInfo[gClass.srmNum].bUse_fork2)           // 포크 사용 선택에 따라서 버튼 이미지 전환
            {
                gClass.str.SrmInfo[gClass.srmNum].bUse_fork2 = false;
                Lbl_Fork2Sel.Background = null;
            }
            else
            {
                gClass.str.SrmInfo[gClass.srmNum].bUse_fork2 = true;
                Lbl_Fork2Sel.Background = Brushes.White;
            }
        }
    }
}
