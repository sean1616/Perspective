using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
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

using System.Diagnostics;
using Perspective.UI;
using Perspective.Models;
using Perspective.ViewModels;


namespace Perspective.Navigations
{
    /// <summary>
    /// Interaction logic for Page_CurrentPage.xaml
    /// </summary>
    public partial class Page_CurrentPage : UserControl
    {
        VM vm;
        string tagsDirectoryPath = "";

        public Page_CurrentPage(VM vm)
        {
            InitializeComponent();

            this.vm = vm;
            this.DataContext = vm;
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
            ToggleButton uc = (ToggleButton)sender;
            string selected_fileName = uc.Tag.ToString();
            if (!vm.list_selected_files.Contains(selected_fileName))
            {
                vm.list_selected_files.Add(selected_fileName);
            }

            //ToggleButton tbtn = (ToggleButton)sender;

            //string selected_fileName = tbtn.Content.ToString();

            //int file_no = vm.list_fileNames.IndexOf(selected_fileName);

            //string selectedFile_path = vm.list_files[file_no];

            //if (!vm.list_selected_files.Contains(selectedFile_path))
            //{
            //    vm.list_selected_files.Add(selectedFile_path);
            //}            
        }

        private void tbtn_files_Unchecked(object sender, RoutedEventArgs e)
        {
            ToggleButton uc = (ToggleButton)sender;
            string selected_fileName = uc.Tag.ToString();
            if (!vm.list_selected_files.Contains(selected_fileName))
            {
                vm.list_selected_files.Remove(selected_fileName);
            }

            //ToggleButton tbtn = (ToggleButton)sender;

            //string selected_fileName = tbtn.Content.ToString();

            //int file_no = vm.list_fileNames.IndexOf(selected_fileName);

            //string selectedFile_path = vm.list_files[file_no];

            //if (vm.list_selected_files.Contains(selectedFile_path))
            //{
            //    vm.list_selected_files.Remove(selectedFile_path);
            //}
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
                foreach (string tag in vm.list_selectedTags)
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


            //this.files = files;
            //timer_showFilebox.Start();

            //vm.txt_msg = elapsedMs.ToString();
            #endregion
        }
    }
}
