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
using System.Windows.Threading;
using System.Windows.Controls.Primitives;

using System.Diagnostics;
using Perspective.UI;
using Perspective.Models;
using Perspective.ViewModels;
using Perspective.Functions;


namespace Perspective.Navigations
{
    /// <summary>
    /// Interaction logic for Page_CurrentPage.xaml
    /// </summary>
    public partial class Page_CurrentPage : UserControl
    {
        VM vm;
        PathProcess pathProcess;
        string tagsDirectoryPath = "";

        public Page_CurrentPage(VM vm)
        {
            InitializeComponent();

            this.vm = vm;
            this.DataContext = vm;

            pathProcess = new PathProcess(vm);
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
                //SearchDirectory(vm.path);
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
        
        
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            timer.Interval = TimeSpan.FromMilliseconds(20);
            timer.Tick += _timer_Tick;
            timer.Start();

            //foreach (DataModel dm in vm.list_FileDataModels)
            //{
            //    if (pathProcess.IsImageJudge(dm.pathInfo))
            //    {
            //        dm.imgSource = pathProcess.BitmapFromUri(dm.pathInfo);
            //    }

            //}
        }
        int c = 0;
        void _timer_Tick(object sender, EventArgs e)
        {
            if (c == vm.list_FileDataModels.Count)
            {
                timer.Stop();                
                return;
            }
            string path = vm.list_FileDataModels[c].pathInfo;
           
            {
                //vm.list_FileDataModels[c].imgSource = pathProcess.FileBox_NameExtensionJudge(path);
                //vm.list_FileDataModels[c].imgSource = pathProcess.BitmapFromUri(path);
                //vm.list_FileDataModels[c].imgSource = LoadImage(path);
                vm.list_FileDataModels[c].imgSource = pathProcess.LoadImage(path);
            }

            c++;
        }

        DispatcherTimer timer = new DispatcherTimer();

        public static ImageSource LoadImage(string path)
        {
            var bitmap = new BitmapImage();

            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.UriSource = new Uri(path);
            bitmap.EndInit();
            bitmap.Freeze();

            return bitmap;
        }
    }
}
