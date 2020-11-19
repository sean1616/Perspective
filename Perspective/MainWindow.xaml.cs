﻿using System;
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
using System.Windows.Navigation;
using System.Collections.ObjectModel;

using System.Windows.Controls.Primitives;
using Perspective.ViewModels;
using Perspective.Navigations;

namespace Perspective
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        VM vm = new VM();
        Window_AddTags _window_addTags;
        ItemsControl itemsControl = new ItemsControl();

        string currentPath = Directory.GetCurrentDirectory();
        string tagsDirectoryPath = "";

        public MainWindow()
        {           
            InitializeComponent();

            this.DataContext = vm;

            tagsDirectoryPath = currentPath + @"\Tags";

            Binding myBinding = new Binding("list_files[1]");
            myBinding.Source = vm;
            //btn_2.SetBinding(Button.ContentProperty, myBinding);
        }

        Style style_tag;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            style_tag = Application.Current.FindResource("BtnStyle_TagBox") as Style;
            //btn_tag.Style = style_tag;

            SearchDirectory(vm.path);

            GetSavedTags();
        }

        private void Txt_path_PreviewKeyDown(object sender, KeyEventArgs e)
        {            
            if (e.Key == Key.Enter)
            {
                TextBox tbk = (TextBox)sender;
                SearchDirectory(tbk.Text);
            }
        }

        private void SearchDirectory(string path)
        {
            try
            {
                if (string.IsNullOrEmpty(path)) return;

                vm.list_files.Clear();

                if (File.Exists(@path))
                {
                    // This path is a file
                    ProcessFile(@path);
                }
                else if (Directory.Exists(@path))
                {
                    // This path is a directory
                    ProcessDirectory(@path);
                }
                else
                {
                    Console.WriteLine("{0} is not a valid file or directory.", path);
                }

                //取得本資料夾路徑
                //string thisFld = System.IO.Directory.GetParent(@tbk.Text).FullName.ToString();

            }
            catch { }
        }

        // Insert logic for processing found files here.
        public void ProcessFile(string path)
        {
            string filename = System.IO.Path.GetFileNameWithoutExtension(path);
            vm.list_files.Add(filename);
            vm.list_fileNames.Add(System.IO.Path.GetFileName(path));

            Console.WriteLine("Processed file '{0}'.", path);
        }

        public void ProcessGetFilesInDirectory(string targetDirectory)
        {
            // Process the list of files found in the directory.
            string[] fileEntries = Directory.GetFiles(targetDirectory);
            foreach (string fileName in fileEntries)
            {
                ProcessFile(fileName);
            }
        }

        // Process all files in the directory passed in, recurse on any directories
        // that are found, and process the files they contain.
        public void ProcessDirectory(string targetDirectory)
        {
            string[] directories = System.IO.Directory.GetDirectories(targetDirectory);

            foreach(string s in directories)
            {
                if (Directory.Exists(@s))
                {
                    // This path is a directory
                    vm.list_files.Add(s);
                    vm.list_fileNames.Add(Path.GetFileName(s));
                }                
            }


            //// Process the list of files found in the directory.
            //string[] fileEntries = Directory.GetFiles(targetDirectory);
            //foreach (string fileName in fileEntries)
            //{
            //    ProcessFile(fileName);
            //}
               

            //// Recurse into subdirectories of this directory.
            //string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
            //foreach (string subdirectory in subdirectoryEntries)
            //    ProcessDirectory(subdirectory);

           
            
        }

        private void Btn_addTagWindow_Click(object sender, RoutedEventArgs e)
        {          
            _window_addTags = new Window_AddTags(vm);

            if (_window_addTags.ShowDialog() == false)
            {
                if (!string.IsNullOrEmpty(_window_addTags.NewTagName))
                {
                    Button btn_tag = new Button { Content = _window_addTags.NewTagName };
                    btn_tag.Style = style_tag;
                    btn_tag.Click += Btn_tag_Click;
                    //vm.list_tags = new System.Collections.ObjectModel.ObservableCollection<string>(vm.list_tags);

                    //stkpanel_tags.Children.Add(btn_tag);

                    vm.dictonary_tag_files.Add(_window_addTags.NewTagName, new ObservableCollection<string>());
                }                    
            }
        }

        private void Btn_tag_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            string tag = btn.Content.ToString();

            vm.list_files.Clear();
            vm.list_fileNames.Clear();

            string tagTxtPath = tagsDirectoryPath + @"\" + tag + @".txt";

            string[] lines = System.IO.File.ReadAllLines(tagTxtPath);

            foreach(string s in lines)
            {
                if (vm.dictonary_tag_files.ContainsKey(tag))
                {
                    vm.list_files.Add(s);
                    vm.list_fileNames.Add(Path.GetFileName(s));
                }
            }
            
                 
        }

        private void Btn_1_Checked(object sender, RoutedEventArgs e)
        {
            ToggleButton tbtn = (ToggleButton)sender;
                       
            string selected_fileName = tbtn.Content.ToString();
            
            int file_no = vm.list_fileNames.IndexOf(selected_fileName);

            string selectedFile_path = vm.list_files[file_no];

            if (!vm.list_selected_files.Contains(selectedFile_path))
            {
                vm.list_selected_files.Add(selectedFile_path);
            }            
        } 
        
        private void btn_tag_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            Button btn = (Button)sender;
            string tag = btn.Content.ToString();

            if (vm._isTagRemoveMode)
            {                
                if (vm.list_tags.Contains(tag)) vm.list_tags.Remove(tag);
            }
            else
            {                
                string tagTxtPath = tagsDirectoryPath + @"\" + tag + @".txt";

                string[] lines = System.IO.File.ReadAllLines(tagTxtPath);
                

                using (StreamWriter file = new StreamWriter(@tagTxtPath, true))
                {
                    foreach (string s in vm.list_selected_files)
                    {
                        if (!lines.Contains(s))
                        {
                            vm.dictonary_tag_files[tag].Add(s);
                            file.WriteLine(s);  //寫入選取的檔案or資料夾 路徑
                        }                        
                    }
                }
            }
        }

        private void btn_saveTag_Click(object sender, RoutedEventArgs e)
        {            
            if (vm.list_tags.Count <= 0) return;

            if (Directory.Exists(tagsDirectoryPath))
            {
                string[] tagsPath = Directory.GetFiles(tagsDirectoryPath);
                List<string> tags_already = new List<string>();
                foreach (string s in tagsPath)
                {
                    tags_already.Add(Path.GetFileNameWithoutExtension(s));
                }

                foreach(string s in vm.list_tags)
                {
                    string tagTxtPath = tagsDirectoryPath + @"\" + s + @".txt";
                    if (!tags_already.Contains(s))  //若tag未有文字檔
                    {                        
                        using (StreamWriter sw = File.CreateText(@tagTxtPath)) { }  //建立空的文件檔
                    }

                    if (vm.list_files.Count <= 0)  //若未選取檔案or資料夾
                    {
                        continue;
                    }
                    else
                    {
                        using (StreamWriter file = new StreamWriter(@tagTxtPath, true))
                        {
                            foreach(string str in vm.list_selected_files)
                            {
                                file.WriteLine(str);  //寫入選取的檔案or資料夾 路徑
                            }
                            
                        }
                    }
                }
            }
            else
                Directory.CreateDirectory(tagsDirectoryPath);
                                   
        }

        private void GetSavedTags()
        {           
            if (Directory.Exists(tagsDirectoryPath))
            {
                vm.list_tags = new System.Collections.ObjectModel.ObservableCollection<string>();
                string[] tagsPath = Directory.GetFiles(tagsDirectoryPath);
                foreach (string s in tagsPath)
                {
                    string tag = Path.GetFileNameWithoutExtension(s);
                    vm.list_tags.Add(tag);
                    vm.dictonary_tag_files.Add(tag, new ObservableCollection<string>());
                }
            }            
        }

        private void btn_tagClear_Click(object sender, RoutedEventArgs e)
        {
            vm.list_fileNames.Clear();
            vm.list_files.Clear();
            vm.list_selected_files.Clear();

            string[] keys = vm.dictonary_tag_files.Keys.ToArray();
            foreach(string key in keys)
            {
                vm.dictonary_tag_files[key] = new ObservableCollection<string>();
            }
            SearchDirectory(vm.path);
            
        }

        private void btn_RefreshTags_Click(object sender, RoutedEventArgs e)
        {
            foreach(string tag in vm.list_tags)
            {
                string tagTxtPath = tagsDirectoryPath + @"\" + tag + @".txt";
                string[] files_in_lines = System.IO.File.ReadAllLines(tagTxtPath);

                IEnumerable<string> distinctAges = files_in_lines.Distinct();

                File.WriteAllText(tagTxtPath, string.Empty);

                using (StreamWriter file = new StreamWriter(@tagTxtPath, true))
                {
                    foreach (string s in distinctAges)
                    {
                        file.WriteLine(s);  //寫入選取的檔案or資料夾 路徑
                    }
                }
                   
            }
        }
    }
}
