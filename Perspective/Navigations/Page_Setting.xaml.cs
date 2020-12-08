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
using Perspective.ViewModels;
using Perspective.Models;

namespace Perspective.Navigations
{
    /// <summary>
    /// Page_Setting.xaml 的互動邏輯
    /// </summary>
    public partial class Page_Setting : UserControl
    {
        VM vm;
        public Page_Setting(VM vm)
        {
            InitializeComponent();

            this.vm = vm;
            this.DataContext = vm;
        }
    }
}
