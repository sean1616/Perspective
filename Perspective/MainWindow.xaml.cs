using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.Diagnostics;
using System.Windows.Controls.Primitives;
using Perspective.Functions;
using Perspective.ViewModels;
using Perspective.Navigations;
using Perspective.Models;
using Perspective.UI;

namespace Perspective
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        VM vm = new VM();
        Window_AddTags _window_addTags;
        ListCollection listCollection;
        ItemsControl itemsControl = new ItemsControl();

        System.Timers.Timer timer_showFilebox = new System.Timers.Timer();

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
            //itms_directories.ItemsSource = vm.list_DirDataModels;
            //itms_files.ItemsSource = vm.list_FileDataModels;

            timer_showFilebox.Interval = 50;
            timer_showFilebox.Elapsed += Timer_showFilebox_Elapsed;
        }

        Style style_tag;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            style_tag = Application.Current.FindResource("BtnStyle_TagBox") as Style;
            //btn_tag.Style = style_tag;                       

            GetSavedTags();

            vm.unigrid_column = (int)Math.Truncate(viewer.ActualWidth / 140);

            
            SearchDirectory(vm.path);
            
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            vm.unigrid_column = (int)Math.Truncate(viewer.ActualWidth / 140);
        }

        private void Txt_path_PreviewKeyDown(object sender, KeyEventArgs e)
        {            
            if (e.Key == Key.Enter)
            {
                TextBox tbk = (TextBox)sender;
                SearchDirectory(tbk.Text);
            }
        }

        private void Txt_path_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Set the event as handled
            e.Handled = true;
            // Select the Text
            (sender as TextBox).SelectAll();
    }

        private void btn_check_path_Click(object sender, RoutedEventArgs e)
        {
            SearchDirectory(vm.path);
        }

        private void SearchDirectory(string path)
        {
            try
            {
                if (string.IsNullOrEmpty(path)) return;

                vm.list_DirDataModels.Clear();
                vm.list_FileDataModels.Clear();
                vm.list_files.Clear();
                vm.list_directories.Clear();
                vm.list_dirNames.Clear();
                vm.list_fileNames.Clear();
                
                if (File.Exists(@path))  // This path is a file
                {                    
                    ProcessFile(@path);
                }
                else if (Directory.Exists(@path))  // This path is a directory
                {
                    Task GetFile_Task = new Task(() => ProcessDirectory(path));
                    GetFile_Task.Start();
                    //ProcessDirectory(@path);
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

        public void ProcessGetFilesInDirectory(string targetDirectory)
        {
            // Process the list of files found in the directory.
            string[] fileEntries = Directory.GetFiles(targetDirectory);
            foreach (string fileName in fileEntries)
            {
                ProcessFile(fileName);
            }
        }

        // Insert logic for processing found files here.
        public void ProcessFile(string path)
        {
            string path_directory_of_file = System.IO.Path.GetDirectoryName(path);

            ProcessDirectory(path_directory_of_file);
        }

        string[] files;
        int fileCount = 0;
        private void Timer_showFilebox_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (files == null) return;
            if (fileCount < files.Length)
            {
                string s = files[fileCount];

                if (vm.list_FileDataModels.Count <= fileCount) return;
                              
                string fileExtention = Path.GetExtension(s);
                switch (fileExtention)
                {
                    case ".txt":
                        vm.list_FileDataModels[fileCount].imgSource = "../Resources/Text.png";
                        break;
                    case ".xlsx":
                        vm.list_FileDataModels[fileCount].imgSource = "../Resources/excel.png";
                        break;
                    case ".csv":
                        vm.list_FileDataModels[fileCount].imgSource = "../Resources/excel.png";
                        break;
                    case ".png":
                        vm.list_FileDataModels[fileCount].imgSource = @s;
                        break;
                    case ".jpg":
                        vm.list_FileDataModels[fileCount].imgSource = @s;
                        break;
                    case ".bmp":
                        vm.list_FileDataModels[fileCount].imgSource = @s;
                        break;
                }

                fileCount++;
            }
            else
            {
                timer_showFilebox.Stop();
                fileCount = 0;
            }
        }

        // Process all files in the directory passed in, recurse on any directories
        // that are found, and process the files they contain.
        public void ProcessDirectory(string targetDirectory)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            #region 搜尋本資料夾內的所有資料夾
            string[] directories = System.IO.Directory.GetDirectories(targetDirectory);

            foreach (string s in directories)
            {
                if (Directory.Exists(@s))
                {
                    // This path is a directory
                    vm.list_directories.Add(s);
                    vm.list_dirNames.Add(Path.GetFileName(s));

                    Action methodDeleagate = delegate ()
                    {
                        Task.Delay(50);
                        vm.list_DirDataModels.Add(new DataModel() { Names = Path.GetFileName(s), Visibility_btn_remove = false, pathInfo = s });
                    };
                    this.Dispatcher.BeginInvoke(methodDeleagate);
                    //vm.list_DirDataModels.Add(new DataModel() { Names = Path.GetFileName(s), Visibility_btn_remove = false, pathInfo = s });
                }
            }
            #endregion

            #region 搜尋本資料夾內的所有檔案
            string[] files = Directory.GetFiles(targetDirectory);
            string[] fileNames = new string[files.Length];
            DataModel[] dataModels = new DataModel[files.Length];

            Parallel.For(0, files.Length, i =>
            {
                string s = files[i];
                fileNames[i] = Path.GetFileName(s);                                

                dataModels[i] = new DataModel() { Names = Path.GetFileName(s), Visibility_btn_remove = false, pathInfo = s };

                //string fileExtention = Path.GetExtension(s);
                //switch (fileExtention)
                //{
                //    case ".txt":
                //        dataModels[i].imgSource = "../Resources/Text.png";
                //        break;
                //    case ".xlsx":
                //        dataModels[i].imgSource = "../Resources/excel.png";
                //        break;
                //    case ".csv":
                //        dataModels[i].imgSource = "../Resources/excel.png";
                //        break;
                //    case ".png":
                //        dataModels[i].imgSource = @s;
                //        break;
                //    case ".jpg":
                //        dataModels[i].imgSource = @s;
                //        break;
                //    case ".bmp":
                //        dataModels[i].imgSource = @s;
                //        break;
                //}

                Action methodDeleagate = delegate ()
                {
                    vm.list_FileDataModels.Add(dataModels[i]);
                };
                this.Dispatcher.BeginInvoke(methodDeleagate);
            });

            //foreach(DataModel dataModel in dataModels)
            //{
            //    Action methodDeleagate = delegate ()
            //    {                    
            //        vm.list_FileDataModels.Add(dataModel);
            //    };
            //    this.Dispatcher.BeginInvoke(methodDeleagate);
            //}

            //foreach (string s in files)
            //{
            //    vm.list_files.Add(s);
            //    vm.list_fileNames.Add(Path.GetFileName(s));

            //    DataModel newDataModel = new DataModel() { Names = Path.GetFileName(s), Visibility_btn_remove = false, pathInfo = s };               

            //    //vm.list_FileDataModels.Add(newDataModel);

            //    Action methodDeleagate = delegate ()
            //    {
            //        Task.Delay(50);
            //        vm.list_FileDataModels.Add(newDataModel);
            //    };
            //    this.Dispatcher.BeginInvoke(methodDeleagate);

            //}

            long elapsedMs = watch.ElapsedMilliseconds;


            this.files = files;
            timer_showFilebox.Start();

            //vm.txt_msg = elapsedMs.ToString();
            #endregion
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
                    if (!vm.dictonary_tag_files.ContainsKey(_window_addTags.NewTagName))
                        vm.dictonary_tag_files.Add(_window_addTags.NewTagName, new ObservableCollection<string>());
                }                    
            }
        }

        //當標籤左鍵點擊時，呼叫有此標籤的物件
        private void Btn_tag_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton btn = (ToggleButton)sender;
            string tag = btn.Content.ToString();

            //將此標籤加入List_selectedTag
            if (!vm.list_selectedTags.Contains(tag)) vm.list_selectedTags.Add(tag);

            string tagTxtPath = tagsDirectoryPath + @"\" + tag + @".txt";   //Txt path of this tag

            if (vm._isTagRemoveMode)
            {
                //if (vm.list_tags.Contains(tag)) vm.list_tags.Remove(tag);
                listCollection.RemoveTag(tag, tagTxtPath);
                return;
            }

            if ((bool)btn.IsChecked)
            {
                vm.list_DirDataModels.Clear();
                //vm.list_directories.Clear();
                //vm.list_dirNames.Clear();
                vm.list_files.Clear();
                vm.list_fileNames.Clear();

                string[] lines;
                if (File.Exists(tagTxtPath))
                {
                    lines = System.IO.File.ReadAllLines(tagTxtPath);
                }
                else return;

                foreach (string s in lines)
                {
                    if (vm.dictonary_tag_files.ContainsKey(tag))
                    {
                        if (File.Exists(@s)) // This path is a file
                        {
                            vm.list_files.Add(s);
                            vm.list_fileNames.Add(Path.GetFileName(s));
                        }
                        else if (Directory.Exists(@s)) // This path is a directory
                        {
                            vm.list_DirDataModels.Add(new DataModel() { Names = Path.GetFileName(s), pathInfo = s });
                        }
                    }
                }
            }
           
        }

        //當標籤中鍵點擊時
        private void btn_tag_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.MiddleButton == MouseButtonState.Pressed)
            {
                foreach(string s in vm.list_selected_dirs)
                {
                    string line = null;
                    string line_to_delete = "the line i want to delete";

                    using (StreamReader reader = new StreamReader("C:\\input"))
                    {
                        using (StreamWriter writer = new StreamWriter("C:\\output"))
                        {
                            while ((line = reader.ReadLine()) != null)
                            {
                                if (String.Compare(line, line_to_delete) == 0)
                                    continue;

                                writer.WriteLine(line);
                            }
                        }
                    }
                }

                foreach(string s in vm.list_selected_files)
                {

                }
            }
        }

        private void tbtn_directories_Checked(object sender, RoutedEventArgs e)
        {
            ToggleButton uc = (ToggleButton)sender;
            string selectedDir_path = uc.Tag.ToString();
            if (!vm.list_selected_dirs.Contains(selectedDir_path))
            {
                vm.list_selected_dirs.Add(selectedDir_path);
            }

            //ToggleButton tbtn = (ToggleButton)sender;

            //string selected_fileName = tbtn.Content.ToString();

            //int dir_no = vm.list_dirNames.IndexOf(selected_fileName);

            //string selectedDir_path = vm.list_directories[dir_no];

            //if (!vm.list_selected_dirs.Contains(selectedDir_path))
            //{
            //    vm.list_selected_dirs.Add(selectedDir_path);
            //}
        }

        private void tbtn_directories_Unchecked(object sender, RoutedEventArgs e)
        {
            ToggleButton uc = (ToggleButton)sender;
            string selectedDir_path = uc.Tag.ToString();
            if (!vm.list_selected_dirs.Contains(selectedDir_path))
            {
                vm.list_selected_dirs.Remove(selectedDir_path);
            }

            //ToggleButton tbtn = (ToggleButton)sender;

            //string selected_fileName = tbtn.Content.ToString();

            //int dir_no = vm.list_dirNames.IndexOf(selected_fileName);

            //string selectedDir_path = vm.list_directories[dir_no];

            //if (vm.list_selected_dirs.Contains(selectedDir_path))
            //{
            //    vm.list_selected_dirs.Remove(selectedDir_path);
            //}
        }

        private void tbtn_files_Checked(object sender, RoutedEventArgs e)
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

        private void tbtn_files_Unchecked(object sender, RoutedEventArgs e)
        {
            ToggleButton tbtn = (ToggleButton)sender;

            string selected_fileName = tbtn.Content.ToString();

            int file_no = vm.list_fileNames.IndexOf(selected_fileName);

            string selectedFile_path = vm.list_files[file_no];

            if (vm.list_selected_files.Contains(selectedFile_path))
            {
                vm.list_selected_files.Remove(selectedFile_path);
            }
        }

        private void tbtn_directories_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ToggleButton tbtn = (ToggleButton)sender;

            string selected_fileName = tbtn.Content.ToString();

            int dir_no = vm.list_dirNames.IndexOf(selected_fileName);

            string selectedDir_path = vm.list_directories[dir_no];

            if (Directory.Exists(selectedDir_path))
            {
                // opens the folder in explorer
                Process.Start(selectedDir_path);
            }

            tbtn.IsChecked = false;
        }

        private void tbtn_files_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ToggleButton tbtn = (ToggleButton)sender;

            string selected_fileName = tbtn.Content.ToString();

            int file_no = vm.list_fileNames.IndexOf(selected_fileName);

            string selectedFile_path = vm.list_files[file_no];

            if (File.Exists(selectedFile_path))
            {
                Process.Start(selectedFile_path);
            }

            tbtn.IsChecked = false;
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
                if (!File.Exists(tagTxtPath))
                    using (StreamWriter sw = File.CreateText(@tagTxtPath)) { }  //建立空的文件檔

                string[] lines = System.IO.File.ReadAllLines(tagTxtPath);

                try
                {
                    using (StreamWriter file = new StreamWriter(@tagTxtPath, true))
                    {
                        foreach (string s in vm.list_selected_dirs)
                        {
                            if (!lines.Contains(s))
                            {
                                vm.dictonary_tag_files[tag].Add(s);
                                file.WriteLine(s);
                            }
                        }

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
                catch { }
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
            vm.list_selectedTags.Clear();

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

        private void Btn_open_tags_location_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(tagsDirectoryPath);
        }

        private void Btn_test_Click(object sender, RoutedEventArgs e)
        {
            vm._isTagEditMode = !vm._isTagEditMode;

            if (vm._isTagEditMode)
            {
                for (int i = 0; i < vm.list_DirDataModels.Count; i++)
                {
                    vm.list_DirDataModels[i].Visibility_btn_remove = true;
                }
                
            }
            else
            {
                for (int i = 0; i < vm.list_DirDataModels.Count; i++)
                {
                    vm.list_DirDataModels[i].Visibility_btn_remove = false;
                }
                
            }
           
        }

        private void UC_FileBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            UC_FileBox uc = (UC_FileBox)sender;

            if (File.Exists(@uc.path_info))  // This path is a file
            {
                Process.Start(@uc.path_info);
            }
            else if (Directory.Exists(@uc.path_info))  // This path is a directory
            {
                vm.path_previous.Add(vm.path);

                vm.path = uc.path_info;
                SearchDirectory(vm.path);
            }
        }                

        private void UC_FileBox_btn_delete_Click(object sender, RoutedEventArgs e)
        {
            Button uc = (Button)sender;
            string selectedDir_path = uc.Tag.ToString();

            if (Directory.Exists(tagsDirectoryPath))
            {
                foreach(string tag in vm.list_selectedTags)
                {
                    string tagTxtPath = tagsDirectoryPath + @"\" + tag + @".txt";
                    if (!File.Exists(tagTxtPath)) continue;

                    var tempFile = Path.GetTempFileName();
                    var linesToKeep = File.ReadLines(tagTxtPath).Where(l => l != selectedDir_path);

                    File.WriteAllLines(tempFile, linesToKeep);

                    File.Delete(tagTxtPath);
                    File.Move(tempFile, tagTxtPath);

                    //using (var sr = new StreamReader(tagTxtPath))
                    //using (var sw = new StreamWriter(tagTxtPath_Temp))
                    //{
                    //    string line;

                    //    while ((line = sr.ReadLine()) != null)
                    //    {
                    //        if (line != "removeme")
                    //            sw.WriteLine(line);
                    //    }
                    //}

                    //File.Delete(tagTxtPath);
                    //File.Move(tagTxtPath_Temp, tagTxtPath);
                }

               

                
            }
        }

        private void Btn_previous_Click(object sender, RoutedEventArgs e)
        {
            if (vm.path_previous.Count == 0) return;
            if (Directory.Exists(@vm.path_previous.Last()))  // This path is a directory
            {
                vm.path_after.Add(vm.path);
                vm.path = vm.path_previous.Last();
                
                                
                SearchDirectory(vm.path);

                vm.path_previous.RemoveAt(vm.path_previous.IndexOf(vm.path_previous.Last()));
            }
        }

        private void Btn_after_Click(object sender, RoutedEventArgs e)
        {
            if (vm.path_after.Count == 0) return;
            if (Directory.Exists(@vm.path_after.Last()))  // This path is a directory
            {
                vm.path_previous.Add(vm.path);
                vm.path = vm.path_after.Last();
               

                SearchDirectory(vm.path);

                vm.path_after.RemoveAt(vm.path_after.IndexOf(vm.path_after.Last()));
            }
        }
    }
}
