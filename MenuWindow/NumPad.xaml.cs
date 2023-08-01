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
    /// NumPad.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class NumPad : Window
    {
        public double dValue;

        double pixelWidth;
        double pixelHeight;

        String dValueStr;

        TextBox pEditBox;

        public NumPad()
        {
            InitializeComponent();
        }

        public void open_RelativePos(TextBox editTbox)
        {
            double editHeight = 0;

            editHeight = editTbox.Height;
            Point editPos = editTbox.PointToScreen(new Point(0, 0));

            pEditBox = editTbox;
            Color color = (Color)ColorConverter.ConvertFromString("#4CFF0000");
            pEditBox.Background = new SolidColorBrush(color);
           // dValueStr = editTbox.Text;
            dValueStr = "";

            double dpiX = VisualTreeHelper.GetDpi(this).PixelsPerInchX;
            double dpiY = VisualTreeHelper.GetDpi(this).PixelsPerInchY;

            pixelWidth = this.Width * dpiX / 96;    // position to pixel
            pixelHeight = this.Height * dpiX / 96;  // position to pixel
            editHeight = editHeight * dpiY / 96;            // position to pixel

            double screenWidth = SystemParameters.PrimaryScreenWidth * dpiX / 96;  // 2560 * dpiX / 96  position to pixel
            double screenHeight = SystemParameters.PrimaryScreenHeight * dpiY / 96; // 1440

            this.Left = (editPos.X) / dpiX * 96.0;    // pixel to position
            this.Top = (editPos.Y + editHeight) / dpiY * 96.0;


            if (editPos.X + pixelWidth > screenWidth)
            {
                this.Left = (editPos.X - (editPos.X + pixelWidth - screenWidth)) / dpiX * 96.0; // pixel to position
            }

            if (editPos.Y + pixelHeight > screenHeight)
            {
                this.Top = (editPos.Y - pixelHeight) / dpiX * 96.0;
            }
            

            Console.WriteLine("Get Screen Position " + editPos.X + " " + editPos.Y + " " + this.Left + " " + this.Top + " " + this.Width + " " + screenWidth + " " + screenHeight);
            this.ShowDialog();

            ////menu_Setting
            //Point position = Btn_Setting.PointToScreen(new Point(0, 0));
            //Point m_position = this.PointToScreen(new Point(0, 0));

            //menu_Setting.Left = position.X / dpiX * 96.0 - 120;
            //menu_Setting.Top = position.Y / dpiY * 96.0 - 80;
            //menu_Setting.Show();

            //Console.WriteLine("Get Screen Position " + SystemParameters.PrimaryScreenWidth + " " + SystemParameters.PrimaryScreenHeight + " " + m_position.X + " " + m_position.Y);
        }

        private void Btn_Enter_Click(object sender, RoutedEventArgs e)
        {
            pEditBox.Background = null;
            this.Close();
        }

        private void Btn_Append_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;

            if (btn == Btn_No0)
            {
                // Button 0
                dValueStr = dValueStr + "0";
            }
            else if (btn == Btn_No1)
            {
                // Button 1
                dValueStr = dValueStr + "1"; 
            }
            else if (btn == Btn_No2)
            {
                // Button 2
                dValueStr = dValueStr + "2";
            }
            else if (btn == Btn_No3)
            {
                // Button 3
                dValueStr = dValueStr + "3";
            }
            else if (btn == Btn_No4)
            {
                // Button 4
                dValueStr = dValueStr + "4";
            }
            else if (btn == Btn_No5)
            {
                // Button 5
                dValueStr = dValueStr + "5";
            }
            else if (btn == Btn_No6)
            {
                // Button 6
                dValueStr = dValueStr + "6";
            }
            else if (btn == Btn_No7)
            {
                // Button 7
                dValueStr = dValueStr + "7";
            }
            else if (btn == Btn_No8)
            {
                // Button 8
                dValueStr = dValueStr + "8";
            }
            else if (btn == Btn_No9)
            {
                // Button 9
                dValueStr = dValueStr + "9";
            }
            else if (btn == Btn_Back)
            {
                // Button Back
                dValueStr = dValueStr.Remove(dValueStr.Length - 1);
            }

            pEditBox.Text = dValueStr;
        }
    }
}
