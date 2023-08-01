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
    /// PageMain.xaml에 대한 상호 작용 논리
    /// </summary>
    /// 
    public partial class PageMain : Page
    {
        //Singletone
        singletonClass gClass;
        int craneCount = -1;

        public PageMain()
        {
            Console.WriteLine("Page 생성사 호출 확인");
            InitializeComponent();
            gClass = singletonClass.Instance;
        }

        public void craneDisplay(ref PageDevState page2, ref PageDevState page3)
        {
            Frm_Dev2.Content = page2;
            Frm_Dev3.Content = page3;
        }
    }
}
