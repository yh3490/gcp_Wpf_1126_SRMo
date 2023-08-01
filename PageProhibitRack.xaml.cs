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
using System.Collections.ObjectModel;
using System.Windows.Controls.Primitives;

namespace gcp_Wpf
{
    /// <summary>
    /// PageProhibitRack.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class PageProhibitRack : Page
    {
        //ObservableCollection<Rack> rackList = null;
        UniformGrid uniformGrid = new UniformGrid();

        int cntRow, cntBay, cntLev, cntStn;

        bool changed = false;                           // 설정 변경 상태 체크
        //Singletone
        singletonClass gClass;

        Rack preSelRack;

        private void Combo_row_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Btn_UnCheck_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < uniformGrid.Children.Count; i++)
            {
                var tmp = uniformGrid.Children[i] as Rack;
                if (tmp != null)
                {
                    if (tmp.select)
                    {
                        tmp.Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/gcp_Wpf;component/Resources/rack.png")));
                        tmp.BorderBrush = null;
                        tmp.select = false;
                    }
                }
            }
        }

        private void Btn_SelBayS_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < uniformGrid.Children.Count; i++)
            {
                var tmp = uniformGrid.Children[i] as Rack;
                if (tmp != null)
                {
                    if(tmp.bay == preSelRack.bay)
                    {
                        tmp.Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/gcp_Wpf;component/Resources/rack_sel.png")));
                        tmp.select = true;
                    }
                }
            }
        }

        private void Btn_SelLevS_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < uniformGrid.Children.Count; i++)
            {
                var tmp = uniformGrid.Children[i] as Rack;
                if (tmp != null)
                {
                    if (tmp.lev == preSelRack.lev)
                    {
                        tmp.Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/gcp_Wpf;component/Resources/rack_sel.png")));
                        tmp.select = true;
                    }
                }
            }
        }

        private void Btn_Proh_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < uniformGrid.Children.Count; i++)
            {
                var tmp = uniformGrid.Children[i] as Rack;
                if (tmp != null)
                {
                    if (tmp.select)
                    {
                        if (tmp.proh)
                        {
                            tmp.Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/gcp_Wpf;component/Resources/rack.png")));
                            tmp.BorderBrush = null;
                            tmp.select = false;
                            tmp.spec = 0;
                            tmp.proh = false;
                        }
                        else
                        {
                            tmp.Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/gcp_Wpf;component/Resources/rack_proh.png")));
                            tmp.BorderBrush = null;
                            tmp.select = false;
                            tmp.spec = 0;
                            tmp.proh = true;
                        }
                    }
                }
            }
        }

        private void Btn_Spec_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < uniformGrid.Children.Count; i++)
            {
                var tmp = uniformGrid.Children[i] as Rack;
                if (tmp != null)
                {
                    if (tmp.select)
                    {
                        if (tmp.spec > 0)
                        {
                            tmp.Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/gcp_Wpf;component/Resources/rack.png")));
                            tmp.BorderBrush = null;
                            tmp.select = false;
                            tmp.spec = 0;
                            tmp.proh = false;
                        }
                        else
                        {
                            tmp.Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/gcp_Wpf;component/Resources/rack_spec.png")));
                            tmp.BorderBrush = null;
                            tmp.select = false;
                            tmp.proh = false;
                            tmp.spec = 1;
                        }
                    }
                }
            }
        }

        private void Btn_Save_Click(object sender, RoutedEventArgs e)
        {
            // 통신 전송 및 결과 확인하여 현재 상태 표시 갱신 
        }

        private void Btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < uniformGrid.Children.Count; i++)
            {
                var tmp = uniformGrid.Children[i] as Rack;
                if (tmp != null)
                {
                    tmp.Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/gcp_Wpf;component/Resources/rack.png")));
                    tmp.BorderBrush = null;
                    tmp.select = false;
                    tmp.spec = 0;
                    tmp.proh = false;
                }
            }
        }

        public PageProhibitRack()
        {
            gClass = singletonClass.Instance;

            InitializeComponent();


            // 크레인 설정 별 - ROW선택 별 호출가능하도록
            rackInitialize(1);

            //Rack Rack10 = new Rack();
            //uniformGrid.Children.Add(Rack10);
            //Rack10.Content = "test";

            //UIElement childItem = uniformGrid.Children[9];
            //Rack childButton = childItem as Rack;

            //childButton.Content = "Rack";

        }

        private void rackInitialize(int row)
        {
            //Combo_row.Items.Clear();


            int curSrm = gClass.srmNum;
            cntRow = gClass.str.SrmInfo[curSrm].row;
            cntBay = gClass.str.SrmInfo[curSrm].bay;
            cntLev = gClass.str.SrmInfo[curSrm].lev;
            cntStn = gClass.str.SrmInfo[curSrm].stn;

            uniformGrid.Rows = cntLev;
            uniformGrid.Columns = cntBay;

            for (int i = 0; i < cntLev; i++)
            {
                for (int j = 0; j < cntBay; j++)
                {
                    Rack tmp = new Rack(j,i,false, 0) {ParentUniformGrid = uniformGrid };
                    tmp.ButtonClick += RackClickHandler;
                    uniformGrid.Children.Add(tmp);
                }
            }
            RackStack.Children.Add(uniformGrid);

            for (int i = 0; i < uniformGrid.Children.Count; i++)
            {
                
            }

                //rackListView.Items.Clear();

                //GridView gridView = rackListView.View as GridView;

                //for (int i = 0; i < cntBay; i++)
                //{
                //    gridView.Columns.Add(new GridViewColumn
                //    {
                //        Header = "Bay"+i,
                //        DisplayMemberBinding = new Binding("Bay"+i)
                //    });
                //}

                //rackList = new ObservableCollection<Rack>();
                //rackList.Add(new Rack() { Bay1 = "123", Bay2 = "456", Bay3 = "789" });
                //rackList.Add(new Rack() { Bay1 = "123", Bay2 = "456", Bay3 = "789" });
                //rackList.Add(new Rack() { Bay1 = "123", Bay2 = "456", Bay3 = "789" });
                //rackList.Add(new Rack() { Bay1 = "123", Bay2 = "456", Bay3 = "789" });
                //rackList.Add(new Rack() { Bay1 = "123", Bay2 = "456", Bay3 = "789" });
                //rackList.Add(new Rack() { Bay1 = "123", Bay2 = "456", Bay3 = "789" });
                //rackList.Add(new Rack() { Bay1 = "123", Bay2 = "456", Bay3 = "789" });
                //rackListView.DataContext = rackList;


            }

        private void RackClickHandler(object sender, RoutedEventArgs e)
        {
            // Call the parent method when the custom button is clicked
            var selectedRack = sender as Rack;
            if (selectedRack != null)
            {
                if(preSelRack != null)
                {
                    preSelRack.BorderBrush = null;          // 현재 선택된 랙 만 보더 표시
                }
                preSelRack = selectedRack;
                //selectedRack.ro
                // Access members of SpecificSenderClass using specificSender variable
            }
        }

        //private void YourMouseDownEventHandler(object sender, MouseButtonEventArgs e)
        //{
        //    UIElement clickedElement = (UIElement)sender;

        //    if (Keyboard.Modifiers == ModifierKeys.Control)
        //    {
        //        // Multi-select behavior when holding the Control key
        //        if (selectedElements.Contains(clickedElement))
        //            selectedElements.Remove(clickedElement);
        //        else
        //            selectedElements.Add(clickedElement);
        //    }
        //    else
        //    {
        //        // Single-select behavior
        //        selectedElements.Clear();
        //        selectedElements.Add(clickedElement);
        //    }

        //    // Update the visual state or perform any necessary operations based on the selection
        //    UpdateSelectionVisuals();
        //}
    }


    public class Rack : Button
    {

        public UniformGrid ParentUniformGrid { get; set; }

        public delegate void ButtonClickHandler(object sender, RoutedEventArgs e);
        public event ButtonClickHandler ButtonClick;
        public int row { get; set; }
        public int bay { get; set; }
        public int lev { get; set; }

        public bool select { get; set; }
        public bool proh { get; set; }
        public int spec { get; set; }

        //public String Bay1 { get; set; }
        public static readonly DependencyProperty CustomTextProperty =
        DependencyProperty.Register("CustomText", typeof(string), typeof(Rack), new PropertyMetadata(""));

        public Rack(int bay, int lev, bool proh=false, int spec=0 )
        {
            this.bay = bay;
            this.lev = lev;
            this.proh = proh;
            this.spec = spec;
            this.Foreground = Brushes.White;
            this.Content = "(" + bay + "," + lev + ")";
            this.MinHeight = 50;
            this.MinWidth = 50;
            this.Click += MyButton_Click;
            this.Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/gcp_Wpf;component/Resources/rack.png")));
            this.BorderBrush = null;
        }

        private void MyButton_Click(object sender, RoutedEventArgs e)
        {
            int index = -1;
            if (ParentUniformGrid != null)
            {
                for (int i = 0; i < ParentUniformGrid.Children.Count; i++)
                {
                    if (ParentUniformGrid.Children[i] == sender)
                    {
                        index = i;
                        break;
                    }
                }
            }

            if (select)
            {
                this.Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/gcp_Wpf;component/Resources/rack.png")));
                this.BorderBrush = null;
                select = false;
            }
            else
            {
                this.Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/gcp_Wpf;component/Resources/rack_sel.png")));
                this.BorderBrush = Brushes.Red;
                select = true;
            }

            ButtonClick(sender, e);

            //MessageBox.Show("Button Item Index = " + index);
        }
    }
}
