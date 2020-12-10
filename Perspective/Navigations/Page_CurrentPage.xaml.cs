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
        PathProcess pps;
        string tagsDirectoryPath = "";

        public Page_CurrentPage(VM vm)
        {
            InitializeComponent();

            this.vm = vm;
            this.DataContext = vm;

            pps = new PathProcess(vm);
        }
        private void tbtn_Checked(object sender, RoutedEventArgs e)
        {
            ToggleButton uc = (ToggleButton)sender;
            string s = uc.Tag.ToString();
            DataModel dm = new DataModel();
            try
            {
                dm = (DataModel)uc.DataContext;
            }
            catch { return; }

            if (!vm.list_selected_items.Contains(dm))
            {
                vm.list_selected_items.Add(dm);
                vm.msg.txt_msg1 = string.Concat((vm.list_selected_items.Count), " items");
            }

            pps.Set_FileBox_Info(dm);

            //// get the file attributes for file or directory
            //FileAttributes attr = File.GetAttributes(dm.pathInfo);

            ////detect whether its a directory or file
            //if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
            //{
            //    var directory_size = DirectoryInfoExtension.GetSize(@dm.pathInfo);
            //    vm.msg.txt_msg2 = dm.Name;
            //    vm.msg.txt_msg3 = string.Concat("大小: ", MathCalculation.Calculate_FileSize(directory_size));
            //    vm.msg.txt_msg4 = string.Concat("建立日期: ", dm.creationTime.ToShortDateString(), " ", dm.creationTime.ToShortTimeString());
            //    vm.msg.txt_msg5 = string.Concat("修改日期: ", dm.updateTime.ToShortDateString(), " ", dm.updateTime.ToShortTimeString());
            //}
            //else
            //{
            //    FileInfo fi = new FileInfo(dm.pathInfo);

            //    if (fi.Exists)
            //    {
            //        vm.msg.txt_msg2 = dm.Name;                   
            //        vm.msg.txt_msg3 = string.Concat("大小: ", MathCalculation.Calculate_FileSize(fi.Length));
            //        vm.msg.txt_msg4 = string.Concat("建立日期: ", dm.creationTime.ToShortDateString(), " ", dm.creationTime.ToShortTimeString());
            //        vm.msg.txt_msg5 = string.Concat("修改日期: ", dm.updateTime.ToShortDateString(), " ", dm.updateTime.ToShortTimeString());
            //        //Console.WriteLine("File Size in Bytes: {0}", size);
            //        //// File ReadOnly ?  
            //        //bool IsReadOnly = fi.IsReadOnly;
            //        //Console.WriteLine("Is ReadOnly: {0}", IsReadOnly);
            //        //// Creation, last access, and last write time   
            //        //DateTime creationTime = fi.CreationTime;
            //        //Console.WriteLine("Creation time: {0}", creationTime);
            //        //DateTime accessTime = fi.LastAccessTime;
            //        //Console.WriteLine("Last access time: {0}", accessTime);
            //        //DateTime updatedTime = fi.LastWriteTime;
            //        //Console.WriteLine("Last write time: {0}", updatedTime);
            //    }
            //}

            
        }

        private void tbtn_Unchecked(object sender, RoutedEventArgs e)
        {
            ToggleButton uc = (ToggleButton)sender;
            string s = uc.Tag.ToString();
            DataModel dm = new DataModel();
            try
            {
                 dm= (DataModel)uc.DataContext;
            }
            catch { return; }

            if (vm.list_selected_items.Contains(dm))
            {
                vm.list_selected_items.Remove(dm);

                vm.msg.txt_msg1 = string.Concat((vm.list_selected_items.Count), " items");
            }
        }

        //private void tbtn_directories_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        //{
        //    ToggleButton tbtn = (ToggleButton)sender;

        //    string selected_fileName = tbtn.Content.ToString();

        //    int dir_no = vm.list_dirNames.IndexOf(selected_fileName);

        //    string selectedDir_path = vm.list_directories[dir_no];

        //    if (Directory.Exists(selectedDir_path))
        //    {
        //        // opens the folder in explorer
        //        Process.Start(selectedDir_path);
        //    }

        //    tbtn.IsChecked = false;
        //}

        //private void tbtn_files_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        //{
        //    ToggleButton tbtn = (ToggleButton)sender;

        //    string selected_fileName = tbtn.Content.ToString();

        //    int file_no = vm.list_fileNames.IndexOf(selected_fileName);

        //    string selectedFile_path = vm.list_files[file_no];

        //    if (File.Exists(selectedFile_path))
        //    {
        //        Process.Start(selectedFile_path);
        //    }

        //    tbtn.IsChecked = false;
        //}

        private void UC_FileBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {            
            UC_FileBox uc = (UC_FileBox)sender;
            uc.tbtn_isChecked = false;
            e.Handled = true;            

            if (File.Exists(@uc.path_info))  // This path is a file
            {
                Process.Start(@uc.path_info);
            }
            else if (Directory.Exists(@uc.path_info))  // This path is a directory
            {                
                vm.path_previous.Add(vm.path);
                vm.path = uc.path_info;
                
                pps.SearchDirectory(vm.path);
            }
        }

        private void UC_FileBox_btn_delete_Click(object sender, RoutedEventArgs e)
        {
            Button uc = (Button)sender;
            string selectedDir_path = uc.Tag.ToString();

            if (Directory.Exists(tagsDirectoryPath))
            {
                foreach (TagModel tm in vm.list_selectedTagModels)
                {
                    string tag = tm.tagName;
                    string tagTxtPath = tagsDirectoryPath + @"\" + tag + @".txt";
                    if (!File.Exists(tagTxtPath)) continue;

                    var tempFile = Path.GetTempFileName();
                    var linesToKeep = File.ReadLines(tagTxtPath).Where(l => l != selectedDir_path);

                    File.WriteAllLines(tempFile, linesToKeep);

                    if (Directory.Exists(tagTxtPath))  //刪除指定文件至資源回收筒，並顯示進度視窗
                    {
                        if (MessageBox.Show("Delete Tag ?", "", MessageBoxButton.YesNo) == MessageBoxResult.OK)
                        {
                            try
                            {
                                Microsoft.VisualBasic.FileIO.FileSystem.DeleteDirectory(tagTxtPath, Microsoft.VisualBasic.FileIO.UIOption.AllDialogs,
                              Microsoft.VisualBasic.FileIO.RecycleOption.SendToRecycleBin, Microsoft.VisualBasic.FileIO.UICancelOption.ThrowException);
                            }
                            catch { }
                        }
                    }
                    else vm.msg.txt_msg1 = "Directory is not exist.";
                   
                    File.Move(tempFile, tagTxtPath);
                }
            }
        }
       

        private void UC_Tbtn_MouseEnter(object sender, RoutedEventArgs e)
        {
            ToggleButton uc = (ToggleButton)sender;
            DataModel dm = (DataModel)uc.DataContext;

            if (Mouse.LeftButton==MouseButtonState.Pressed)
            {
                uc.IsChecked = true;

                string selected_fileName = uc.Tag.ToString();
                if (!vm.list_selected_items.Contains(dm))
                {
                    vm.list_selected_items.Add(dm);
                }

                vm.msg.txt_msg1 = string.Concat((vm.list_selected_items.Count), " items");
            }
        }

        private void viewer_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;  //避免Togglebutton ischecked 狀態改變

                if (vm.list_selected_items.Count == 1)
                {
                    if (File.Exists(vm.list_selected_items[0].pathInfo))  // This path is a file
                    {
                        Process.Start(vm.list_selected_items[0].pathInfo);
                        
                    }
                    else if (Directory.Exists(vm.list_selected_items[0].pathInfo))  // This path is a directory
                    {
                        vm.path_previous.Add(vm.path);

                        vm.path = vm.list_selected_items[0].pathInfo;

                        pps.SearchDirectory(vm.path);

                        vm.list_selected_items.Clear();
                    }
                }
            }
            else
            {

            }
        }
    }
}
