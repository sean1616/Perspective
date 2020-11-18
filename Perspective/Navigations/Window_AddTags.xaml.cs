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
using Perspective.ViewModels;

namespace Perspective.Navigations
{
    /// <summary>
    /// Window_AddTags.xaml 的互動邏輯
    /// </summary>
    public partial class Window_AddTags : Window
    {
        VM vm = new VM();

        public Window_AddTags(VM vm)
        {
            InitializeComponent();

            this.vm = vm;
        }

        string _NewTagName = "";
        public string NewTagName
        {
            get { _NewTagName = txt_newTagName.Text; return _NewTagName;  }
        }

        private void Btn_addTag_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txt_newTagName.Text))
            {
                vm.list_tags.Add(txt_newTagName.Text);
                vm.dictonary_tag_files.Add(txt_newTagName.Text, new List<string>());
            }                
        }

        private void Txt_newTagName_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (!string.IsNullOrEmpty(txt_newTagName.Text))
                {
                    vm.list_tags.Add(txt_newTagName.Text);
                    vm.dictonary_tag_files.Add(txt_newTagName.Text, new List<string>());
                }                        
            }
        }

        
    }
}
