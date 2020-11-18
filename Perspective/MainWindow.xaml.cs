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
using System.Windows.Navigation;

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

        public MainWindow()
        {           
            InitializeComponent();

            this.DataContext = vm;

            Binding myBinding = new Binding("list_files[1]");
            myBinding.Source = vm;
            //btn_2.SetBinding(Button.ContentProperty, myBinding);
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

                    vm.dictonary_tag_files.Add(_window_addTags.NewTagName, new List<string>());
                }                    
            }
        }

        private void Btn_tag_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            string tag = btn.Content.ToString();
            foreach (string s in vm.list_selected_files)
            {
                if (!vm.dictonary_tag_files.ContainsKey(tag))
                    vm.dictonary_tag_files[tag].Add(s);
            }
        }

        private void Btn_1_Checked(object sender, RoutedEventArgs e)
        {
            ToggleButton tbtn = (ToggleButton)sender;

            int file_no = 0;
            int.TryParse(tbtn.Name.Split('_')[1], out file_no);

            if (vm.list_files.Count < file_no) return;
            if (!vm.list_selected_files.Contains(vm.list_files[--file_no]))
            {
                vm.list_selected_files.Add(tbtn.Content.ToString());
            }
            
        }
        
        Style style_tag;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            style_tag = Application.Current.FindResource("BtnStyle_TagBox") as Style;
            //btn_tag.Style = style_tag;

            SearchDirectory(vm.path);
        }
                
        
        private void btn_tag_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (vm._isTagRemoveMode)
            {
                Button btn = (Button)sender;
                string tag = btn.Content.ToString();
                if (vm.list_tags.Contains(tag)) vm.list_tags.Remove(tag);
            }
            else
            {

            }
        }

        private void btn_saveTag_Click(object sender, RoutedEventArgs e)
        {
            string currentPath = Directory.GetCurrentDirectory();
            string tagsDirectoryPath = currentPath + @"\Tags";
            Directory.CreateDirectory(tagsDirectoryPath);

            string tag = "123";
            if (vm.list_tags.Count <= 0) return;
            string tagTxtPath = tagsDirectoryPath + @"\" + tag + @".txt";
            using (StreamWriter file = new StreamWriter(@tagTxtPath, true))
            {
                file.WriteLine(vm.list_files[0]);
            }
        }

        
    }
}
