﻿using System;
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
    /// PageDevState.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class PageDevState : Page
    {
        public PageDevState()
        {
            InitializeComponent();
        }

        public PageDevState(int srmNum)
        {
            InitializeComponent();
            lbl_SrmNum.Content = "SRM #" + srmNum.ToString();
        }
    }
}
