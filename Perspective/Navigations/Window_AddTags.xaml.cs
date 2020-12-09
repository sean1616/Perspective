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
using Perspective.Models;

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

            tagsDirectoryPath = vm.tagsDirectoryPath;

            
        }

        string _NewTagName = "";
        public string NewTagName
        {
            get { _NewTagName = txt_newTagName.Text; return _NewTagName;  }
        }

        private void Btn_addTag_Click(object sender, RoutedEventArgs e)
        {
            AddNewTag(txt_newTagName.Text);
        }

        private void Txt_newTagName_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                AddNewTag(txt_newTagName.Text);
            }
        }

        private void AddNewTag(string tag)
        {            
            if (!string.IsNullOrEmpty(tag))
            {
                TagModel model = new TagModel() { tagName = tag, isChecked = false };
                
                if (!vm.list_TagModels.Contains(model))
                {                    
                    vm.list_TagModels.Add(model);
                }
                   
                vm.dictonary_tag_files.Add(tag, new List<string>());

                string tagTxtPath = "";
                if (!vm._isInTagMode)
                    tagTxtPath = tagsDirectoryPath + @"\" + tag + @".txt";   //Txt path of this tag
                else tagTxtPath = vm.IntagsDirectoryPath + @"\" + tag + @".txt";   //Txt path of this tag


                if (!File.Exists(tagTxtPath))
                    using (StreamWriter sw = File.CreateText(@tagTxtPath)) { }  //建立空的文件檔
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txt_newTagName.Focus();
        }
    }
}
