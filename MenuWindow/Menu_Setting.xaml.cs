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
using System.Windows.Shapes;

namespace gcp_Wpf.MenuWindow
{
    /// <summary>
    /// Menu_Setting.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Menu_Setting : Window
    {

        MainWindow pMain;
        public Menu_Setting()
        {
            InitializeComponent();
        }

        public Menu_Setting(MainWindow parent)
        {
            pMain = parent;
            InitializeComponent();
        }

            private void Window_Deactivated(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void Btn_ExhibitSetting_Click(object sender, RoutedEventArgs e)
        {
            pMain.Page_Change(3);
        }

        private void Btn_CommSetting_Click(object sender, RoutedEventArgs e)
        {
            pMain.Page_Change(4);
        }

        private void Btn_CraneSetting_Click(object sender, RoutedEventArgs e)
        {
            pMain.Page_Change(5);
        }

        private void Btn_StationSetting_Click(object sender, RoutedEventArgs e)
        {
            pMain.Page_Change(6);
        }
    }
}
