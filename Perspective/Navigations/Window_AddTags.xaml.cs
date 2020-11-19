using System;
using System.IO;
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
using System.Collections.ObjectModel;
using Perspective.ViewModels;

namespace Perspective.Navigations
{
    /// <summary>
    /// Window_AddTags.xaml 的互動邏輯
    /// </summary>
    public partial class Window_AddTags : Window
    {
        VM vm = new VM();
        string currentPath = Directory.GetCurrentDirectory();
        string tagsDirectoryPath = "";

        public Window_AddTags(VM vm)
        {
            InitializeComponent();

            this.vm = vm;

            tagsDirectoryPath = currentPath + @"\Tags";
        }

        string _NewTagName = "";
        public string NewTagName
        {
            get { _NewTagName = txt_newTagName.Text; return _NewTagName;  }
        }

        private void Btn_addTag_Click(object sender, RoutedEventArgs e)
        {
            string tag = txt_newTagName.Text;
            if (!string.IsNullOrEmpty(tag))
            {
                vm.list_tags.Add(tag);
                vm.dictonary_tag_files.Add(tag, new ObservableCollection<string>());

                string tagTxtPath = tagsDirectoryPath + @"\" + tag + @".txt";   //Txt path of this tag
                if (!File.Exists(tagTxtPath))
                    using (StreamWriter sw = File.CreateText(@tagTxtPath)) { }  //建立空的文件檔
            }                
        }

        private void Txt_newTagName_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                string tag = txt_newTagName.Text;
                if (!string.IsNullOrEmpty(tag))
                {
                    vm.list_tags.Add(tag);
                    vm.dictonary_tag_files.Add(tag, new ObservableCollection<string>());

                    string tagTxtPath = tagsDirectoryPath + @"\" + tag + @".txt";   //Txt path of this tag
                    if (!File.Exists(tagTxtPath))
                        using (StreamWriter sw = File.CreateText(@tagTxtPath)) { }  //建立空的文件檔
                }                        
            }
        }
    }
}
