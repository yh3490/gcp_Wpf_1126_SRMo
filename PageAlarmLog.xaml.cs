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
    /// PageAlarmLog.xaml에 대한 상호 작용 논리
    /// </summary>
    /// 
    public class Data
    {
        public int errNum { get; set; }
        public string occuredTime { get; set; }
        public string errCode { get; set; }
        public string subCode { get; set; }
        public string errContent { get; set; }
    }
    public partial class PageAlarmLog : Page
    {
        ObservableCollection<Data> errStrList = new ObservableCollection<Data>();

        public PageAlarmLog()
        {
            InitializeComponent();

            errStrList.Add(new Data { errNum = 1, occuredTime = "2023-03-31 15:02:26", errCode = "06", subCode = "1", errContent = "테스트" });
            errStrList.Add(new Data { errNum = 2, occuredTime = "2023-03-31 15:02:26", errCode = "06", subCode = "2", errContent = "테스트" });
            errStrList.Add(new Data { errNum = 3, occuredTime = "2023-03-31 15:02:26", errCode = "06", subCode = "3", errContent = "테스트" });
            errStrList.Add(new Data { errNum = 4, occuredTime = "2023-03-31 15:02:26", errCode = "06", subCode = "4", errContent = "테스트" });
            errStrList.Add(new Data { errNum = 5, occuredTime = "2023-03-31 15:02:26", errCode = "06", subCode = "5", errContent = "테스트" });
            AlarmList.ItemsSource = errStrList;
        }
    }
}
