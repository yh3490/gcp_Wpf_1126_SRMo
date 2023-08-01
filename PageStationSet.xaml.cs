using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// PageStationSet.xaml에 대한 상호 작용 논리
    /// </summary>
    /// 
    public class StnData
    {
        public int stNum { get; set; }
        public string stnType { get; set; }
        public string goodsType { get; set; }
        public string stnInfo { get; set; }
    }
    public partial class PageStationSet : Page
    {
        ObservableCollection<StnData> stnList = new ObservableCollection<StnData>();

        public PageStationSet()
        {
            InitializeComponent();

            stnList.Add(new StnData { stNum = 1, stnType = "2023-03-31 15:02:26", goodsType = "06", stnInfo = "1" });
            stnList.Add(new StnData { stNum = 2, stnType = "2023-03-31 15:02:26", goodsType = "06", stnInfo = "2" });
            stnList.Add(new StnData { stNum = 3, stnType = "2023-03-31 15:02:26", goodsType = "06", stnInfo = "3" });
            stnList.Add(new StnData { stNum = 4, stnType = "2023-03-31 15:02:26", goodsType = "06", stnInfo = "4" });
            stnList.Add(new StnData { stNum = 5, stnType = "2023-03-31 15:02:26", goodsType = "06", stnInfo = "5" });
            StationList.ItemsSource = stnList;
        }
    }
}
