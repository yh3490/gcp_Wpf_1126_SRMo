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
using System.Globalization;
using gcp_Wpf.Properties;

namespace gcp_Wpf
{
    /// <summary>
    /// PageCraneSet.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class PageCraneSet : Page
    {
        singletonClass gClass;
        public PageCraneSet()
        {
            // SingleTone Test
            gClass = singletonClass.Instance;
            InitializeComponent();

            // Initialize Config Data  - 불러온 설정 값 UI에 표시
            // #1
            edit_srmID1.Text = gClass.str.SrmInfo[0].srmID.ToString();
            edit_srmType1.Text = gClass.str.SrmInfo[0].srmID.ToString();
            combo_forkType1.SelectedIndex = gClass.str.SrmInfo[0].forkType;
            edit_srmRow1.Text = gClass.str.SrmInfo[0].row.ToString();
            edit_srmBay1.Text = gClass.str.SrmInfo[0].bay.ToString();
            edit_srmLev1.Text = gClass.str.SrmInfo[0].lev.ToString();
            edit_srmStn1.Text = gClass.str.SrmInfo[0].stn.ToString();
            // #2
            edit_srmID2.Text = gClass.str.SrmInfo[1].srmID.ToString();
            edit_srmType2.Text = gClass.str.SrmInfo[1].srmID.ToString();
            combo_forkType2.SelectedIndex = gClass.str.SrmInfo[1].forkType;
            edit_srmRow2.Text = gClass.str.SrmInfo[1].row.ToString();
            edit_srmBay2.Text = gClass.str.SrmInfo[1].bay.ToString();
            edit_srmLev2.Text = gClass.str.SrmInfo[1].lev.ToString();
            edit_srmStn2.Text = gClass.str.SrmInfo[1].stn.ToString();
            // #3
            edit_srmID3.Text = gClass.str.SrmInfo[2].srmID.ToString();
            edit_srmType3.Text = gClass.str.SrmInfo[2].srmID.ToString();
            combo_forkType3.SelectedIndex = gClass.str.SrmInfo[2].forkType;
            edit_srmRow3.Text = gClass.str.SrmInfo[2].row.ToString();
            edit_srmBay3.Text = gClass.str.SrmInfo[2].bay.ToString();
            edit_srmLev3.Text = gClass.str.SrmInfo[2].lev.ToString();
            edit_srmStn3.Text = gClass.str.SrmInfo[2].stn.ToString();


            combo_Lang.SelectedIndex = gClass.str.GcpInfo.language;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            int selectedIdx = comboBox.SelectedIndex;
            Console.WriteLine("ComboBox_SelectionChanged : " + selectedIdx);

            switch (selectedIdx)
            {
                case 0:
                    if (TranslationSource.Instance.CurrentCulture != null)
                        TranslationSource.Instance.CurrentCulture = null;
                    break;
                case 1:
                    if (TranslationSource.Instance.CurrentCulture == null)
                        TranslationSource.Instance.CurrentCulture = new CultureInfo("en");
                    break;
            }

            gClass.str.GcpInfo.language = selectedIdx;
            cIniAccess.Write(AppDomain.CurrentDomain.BaseDirectory + "\\Config\\Config.ini", "GCPINFO", "LanguageIdx", selectedIdx.ToString());
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            //TranslationSource.Instance
            MessageBoxResult result = MessageBox.Show(Properties.Resources.ResourceManager.GetString("저장문구", TranslationSource.Instance.CurrentCulture), Properties.Resources.ResourceManager.GetString("저장", TranslationSource.Instance.CurrentCulture),
            MessageBoxButton.OKCancel, MessageBoxImage.Information);

            if (result == MessageBoxResult.OK)
            {
                // OK button clicked
                // INI 파일 설정 정보 공유 구조체로 초기화  (순서 중요)

                // #1
                cIniAccess.Write(AppDomain.CurrentDomain.BaseDirectory + "\\Config\\Config.ini", "SRMINFO_0", "SRMID", edit_srmID1.Text);
                cIniAccess.Write(AppDomain.CurrentDomain.BaseDirectory + "\\Config\\Config.ini", "SRMINFO_0", "SRMTYPE", edit_srmType1.Text);
                cIniAccess.Write(AppDomain.CurrentDomain.BaseDirectory + "\\Config\\Config.ini", "SRMINFO_0", "FORKTYPE", combo_forkType1.SelectedIndex.ToString());

                cIniAccess.Write(AppDomain.CurrentDomain.BaseDirectory + "\\Config\\Config.ini", "RACKINFO_0", "MaxRow", edit_srmRow1.Text);
                cIniAccess.Write(AppDomain.CurrentDomain.BaseDirectory + "\\Config\\Config.ini", "RACKINFO_0", "Maxbay", edit_srmBay1.Text);
                cIniAccess.Write(AppDomain.CurrentDomain.BaseDirectory + "\\Config\\Config.ini", "RACKINFO_0", "MaxLev", edit_srmLev1.Text);
                cIniAccess.Write(AppDomain.CurrentDomain.BaseDirectory + "\\Config\\Config.ini", "RACKINFO_0", "MaxStn", edit_srmStn1.Text);

                // #2
                cIniAccess.Write(AppDomain.CurrentDomain.BaseDirectory + "\\Config\\Config.ini", "SRMINFO_1", "SRMID", edit_srmID2.Text);
                cIniAccess.Write(AppDomain.CurrentDomain.BaseDirectory + "\\Config\\Config.ini", "SRMINFO_1", "SRMTYPE", edit_srmType2.Text);
                cIniAccess.Write(AppDomain.CurrentDomain.BaseDirectory + "\\Config\\Config.ini", "SRMINFO_1", "FORKTYPE", combo_forkType2.SelectedIndex.ToString());

                cIniAccess.Write(AppDomain.CurrentDomain.BaseDirectory + "\\Config\\Config.ini", "RACKINFO_1", "MaxRow", edit_srmRow2.Text);
                cIniAccess.Write(AppDomain.CurrentDomain.BaseDirectory + "\\Config\\Config.ini", "RACKINFO_1", "Maxbay", edit_srmBay2.Text);
                cIniAccess.Write(AppDomain.CurrentDomain.BaseDirectory + "\\Config\\Config.ini", "RACKINFO_1", "MaxLev", edit_srmLev2.Text);
                cIniAccess.Write(AppDomain.CurrentDomain.BaseDirectory + "\\Config\\Config.ini", "RACKINFO_1", "MaxStn", edit_srmStn2.Text);

                // #3
                cIniAccess.Write(AppDomain.CurrentDomain.BaseDirectory + "\\Config\\Config.ini", "SRMINFO_2", "SRMID", edit_srmID3.Text);
                cIniAccess.Write(AppDomain.CurrentDomain.BaseDirectory + "\\Config\\Config.ini", "SRMINFO_2", "SRMTYPE", edit_srmType3.Text);
                cIniAccess.Write(AppDomain.CurrentDomain.BaseDirectory + "\\Config\\Config.ini", "SRMINFO_2", "FORKTYPE", combo_forkType3.SelectedIndex.ToString());

                cIniAccess.Write(AppDomain.CurrentDomain.BaseDirectory + "\\Config\\Config.ini", "RACKINFO_2", "MaxRow", edit_srmRow3.Text);
                cIniAccess.Write(AppDomain.CurrentDomain.BaseDirectory + "\\Config\\Config.ini", "RACKINFO_2", "Maxbay", edit_srmBay3.Text);
                cIniAccess.Write(AppDomain.CurrentDomain.BaseDirectory + "\\Config\\Config.ini", "RACKINFO_2", "MaxLev", edit_srmLev3.Text);
                cIniAccess.Write(AppDomain.CurrentDomain.BaseDirectory + "\\Config\\Config.ini", "RACKINFO_2", "MaxStn", edit_srmStn3.Text);

                MessageBox.Show(Properties.Resources.ResourceManager.GetString("저장완료재시작", TranslationSource.Instance.CurrentCulture), Properties.Resources.ResourceManager.GetString("저장", TranslationSource.Instance.CurrentCulture),
                MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else if (result == MessageBoxResult.Cancel)
            {
                // Cancel button clicked or dialog closed using the X button
            }
        }

        private void NumberTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Check if the input is a non-numeric character
            if (!IsNumericInput(e.Text))
            {
                e.Handled = true; // Cancel the input event
            }
        }

        private bool IsNumericInput(string input)
        {
            return input.All(char.IsDigit);
        }
    }
}
