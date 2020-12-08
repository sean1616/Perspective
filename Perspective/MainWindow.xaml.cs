﻿using System;
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
using System.ComponentModel;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Controls.Primitives;
using Microsoft.VisualBasic.FileIO;

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
        PathProcess pps;
        ItemsControl itemsControl = new ItemsControl();

        Page_CurrentPage _page_CurrentPage;

        System.Timers.Timer timer_showFilebox = new System.Timers.Timer();

        
        string currentPath = Directory.GetCurrentDirectory();
        string tagsDirectoryPath = "", InTagsDirectoryPath = "";

        public MainWindow()
        {           
            InitializeComponent();

            this.DataContext = vm;

            _page_CurrentPage = new Page_CurrentPage(vm);

            tagsDirectoryPath = currentPath + @"\Tags";
            InTagsDirectoryPath = currentPath + @"\InTags";

            Binding myBinding = new Binding("list_files[1]");
            myBinding.Source = vm;

            pps = new PathProcess(vm);
            //btn_2.SetBinding(Button.ContentProperty, myBinding);
            //itms_directories.ItemsSource = vm.list_DirDataModels;
            //itms_files.ItemsSource = vm.list_FileDataModels;

            grid_msg.DataContext = vm.msg;

            #region Background worker setting
            vm.worker.WorkerReportsProgress = true;
            vm.worker.DoWork += new DoWorkEventHandler(pps.DoWork);
            vm.worker.ProgressChanged += new ProgressChangedEventHandler(pps.DuringWork);
            vm.worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(pps.RunWorkerCompleted);

            vm.timer.Interval = TimeSpan.FromMilliseconds(20);
            vm.timer.Tick += pps._timer_Tick;
            #endregion
        }

        
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //style_tag = Application.Current.FindResource("BtnStyle_TagBox") as Style;
            //btn_tag.Style = style_tag;

            GetSavedTags();

            vm.unigrid_column = (int)Math.Truncate(pageTransitionControl.ActualWidth / 140);
            
            pps.SearchDirectory(vm.path);

            pageTransitionControl.ShowPage(_page_CurrentPage);

        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            vm.unigrid_column = (int)Math.Truncate(pageTransitionControl.ActualWidth / 140);
        }

        private void Txt_path_PreviewKeyDown(object sender, KeyEventArgs e)
        {            
            if (e.Key == Key.Enter)
            {
                TextBox tbk = (TextBox)sender;
                pps.SearchDirectory(tbk.Text);
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
            pps.SearchDirectory(vm.path);
        }


        Style style_tag;
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

            //將此標籤加入/移除List_selectedTag
            TagModel tagModel = new TagModel() { tagName = tag };
            if ((bool)btn.IsChecked)
            {
                if (!vm.list_selectedTags.Contains(tag)) vm.list_selectedTags.Add(tag);                                
                vm.list_selectedTagModels.Add(tagModel);
            }
            else
            {
                vm.list_selectedTags.Remove(tag);
                vm.list_selectedTagModels.Remove(tagModel);
            }
                        
            pps.Refresh_Taged_File(tagsDirectoryPath);

            vm.list_selected_items.Clear();
            //vm.list_selected_files.Clear();
            //vm.list_selected_dirs.Clear();
        }

        private IEnumerable<string> GetFilesIntersection(List<string> list)
        {
            List<string> listF = list;
            return listF;
        }

       

        //當"中鍵"點擊標籤時
        private void btn_tag_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.MiddleButton == MouseButtonState.Pressed)
            {
                ToggleButton obj = (ToggleButton)sender;
                string selectedTag = obj.Content.ToString();
                string tagPath = tagsDirectoryPath + @"\" + selectedTag + @".txt";   //Txt path of this tag

                //var tempList = vm.list_selected_dirs.Concat(vm.list_selected_files);
                var tempList = vm.list_selected_items;

                foreach (DataModel s in tempList)
                {
                    var tempFile = Path.GetTempFileName();
                    var linesToKeep = File.ReadLines(tagPath).Where(l => l != s.pathInfo);

                    File.WriteAllLines(tempFile, linesToKeep);

                    File.Delete(tagPath);
                    File.Move(tempFile, tagPath);
                }

                pps.Refresh_Taged_File(tagsDirectoryPath);
            }
        }

      

      

        private void btn_tag_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            ToggleButton btn = (ToggleButton)sender;
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
                        foreach(DataModel dm in vm.list_selected_items)
                        {
                            if (!lines.Contains(dm.pathInfo))
                            {
                                vm.dictonary_tag_files[tag].Add(dm.pathInfo);
                                file.WriteLine(dm.pathInfo);  //寫入選取的檔案or資料夾 路徑
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
                            foreach(DataModel dm in vm.list_selected_items)
                            {
                                file.WriteLine(dm.pathInfo);  //寫入選取的檔案or資料夾 路徑
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
                vm.list_TagModels = new ObservableCollection<TagModel>();
                vm.list_tags = new System.Collections.ObjectModel.ObservableCollection<string>();
                string[] tagsPath = Directory.GetFiles(tagsDirectoryPath);
                foreach (string s in tagsPath)
                {
                    string tag = Path.GetFileNameWithoutExtension(s);
                    vm.list_tags.Add(tag);

                    if (!vm.dictonary_tag_files.ContainsKey(tag))
                        vm.dictonary_tag_files.Add(tag, new ObservableCollection<string>());

                    TagModel tagModel = new TagModel() { tagName = tag, isChecked = false };
                    vm.list_TagModels.Add(tagModel);
                }
            }
            else
            {
                Directory.CreateDirectory(tagsDirectoryPath);
            }

            #region Invisible tags
            if (!Directory.Exists(InTagsDirectoryPath))
            {
                Directory.CreateDirectory(InTagsDirectoryPath);
            }
            #endregion
        }

        private void btn_selectedTagClear_Click(object sender, RoutedEventArgs e)
        {
            vm.list_fileNames.Clear();
            vm.list_files.Clear();
            vm.list_selectedTags.Clear();

            GetSavedTags();

            string[] keys = vm.dictonary_tag_files.Keys.ToArray();
            foreach(string key in keys)
            {
                vm.dictonary_tag_files[key] = new ObservableCollection<string>();
            }
            pps.SearchDirectory(vm.path);

            var v = (from tagM in vm.list_TagModels where tagM.isChecked == true select tagM );

            foreach(TagModel t in v)
            {
                t.isChecked = false;
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
                pps.SearchDirectory(vm.path);
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
            if (vm.path_previous.Count == 0)
            {
                string p = "";
                try
                {                    
                    p = Directory.GetParent(vm.path).FullName;
                    if (!string.IsNullOrEmpty(p))
                    {                       
                        vm.path_after.Add(vm.path);
                        vm.path = p;                        
                    }                    
                }
                catch { }
                if (!string.IsNullOrEmpty(p))
                {
                    pps.SearchDirectory(vm.path);
                    pageTransitionControl.ShowPage(_page_CurrentPage);
                    vm.list_selected_items.Clear();
                }
                return;
            }
            if (Directory.Exists(@vm.path_previous.Last()))  // This path is a directory
            {
                vm.path_after.Add(vm.path);
                vm.path = vm.path_previous.Last();                
                                
                pps.SearchDirectory(vm.path);

                vm.path_previous.RemoveAt(vm.path_previous.IndexOf(vm.path_previous.Last()));

                pageTransitionControl.ShowPage(_page_CurrentPage);

                vm.list_selected_items.Clear();
            }
        }

        private void Btn_after_Click(object sender, RoutedEventArgs e)
        {
            if (vm.path_after.Count == 0) return;
            if (Directory.Exists(@vm.path_after.Last()))  // This path is a directory
            {
                if (vm.path == vm.path_after.Last()) return;
                vm.path_previous.Add(vm.path);
                vm.path = vm.path_after.Last();               

                pps.SearchDirectory(vm.path);

                vm.path_after.RemoveAt(vm.path_after.IndexOf(vm.path_after.Last()));

                pageTransitionControl.ShowPage(_page_CurrentPage);
                vm.list_selected_items.Clear();
            }
        }

        private void btn_min_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void btn_max_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Normal)
            {
                this.WindowState = WindowState.Maximized;
            }
            else
            {
                this.WindowState = WindowState.Normal;
            }            
        }

        private void btn_close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private bool mRestoreForDragMove;
        private void border_title_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //判斷滑鼠點擊次數
            if (e.ClickCount == 2)
            {
                if ((this.ResizeMode != ResizeMode.CanResize) && (this.ResizeMode != ResizeMode.CanResizeWithGrip))
                    return;
                this.WindowState = this.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized; //雙擊最大化
            }
            else
            {
                mRestoreForDragMove = this.WindowState == WindowState.Normal;
            }
        }

        private void border_title_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (mRestoreForDragMove && this.WindowState == WindowState.Normal)
            {
                //mRestoreForDragMove = false;
                mRestoreForDragMove = false;
                this.DragMove();
            }
        }

        private void border_title_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            mRestoreForDragMove = false;
        }

        private void btn_deleteTag_Click(object sender, RoutedEventArgs e)
        {
            if (vm.list_selectedTags.Count == 0)
            {
                MessageBox.Show("No selected tag");
                return;
            }                

            if (MessageBox.Show("Delete tags ?", "Question", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                foreach (string tag in vm.list_selectedTags)
                {
                    string tagTxtPath = tagsDirectoryPath + @"\" + tag + @".txt";   //Txt path of this tag

                    if (File.Exists(tagTxtPath))
                    {
                        File.Delete(tagTxtPath);
                    }
                    else vm.msg.txt_msg1 = tag + " not exist";

                    foreach(string s in vm.list_selectedTags)
                    {
                        vm.list_tags.Remove(s);
                    }
                    //foreach(TagModel t in vm.list_selectedTagModels)
                    //{
                    //    vm.list_TagModels.Remove(t);
                    //}


                    List<TagModel> tagModels = vm.list_TagModels.Where(x => x.tagName == tag).ToList();
                    foreach (TagModel t in tagModels)
                        vm.list_TagModels.Remove(t);
                }

                vm.list_selectedTagModels.Clear();
                vm.list_selectedTags.Clear();

                MessageBox.Show("Done");
            }
        }

        private void btn_addTag_Click(object sender, RoutedEventArgs e)
        {
            string tag = txt_nTagName.Text;
            if (!string.IsNullOrEmpty(tag))
            {
                vm.list_tags.Add(tag);
                vm.list_TagModels.Add(new TagModel() { tagName = tag, isChecked = false });
                vm.dictonary_tag_files.Add(tag, new ObservableCollection<string>());

                string tagTxtPath = tagsDirectoryPath + @"\" + tag + @".txt";   //Txt path of this tag
                if (!File.Exists(tagTxtPath))
                    using (StreamWriter sw = File.CreateText(@tagTxtPath)) { }  //建立空的文件檔
            }
        }

        bool clip_or_copy = false;
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                if (vm.list_selected_items.Count != 0)
                {
                    foreach (DataModel dm in vm.list_selected_items)
                    {
                        if (File.Exists(dm.pathInfo)) //刪除指定文件至資源回收筒，並顯示進度視窗
                        {
                            try
                            {
                                FileSystem.DeleteFile(dm.pathInfo, UIOption.AllDialogs, RecycleOption.SendToRecycleBin, UICancelOption.ThrowException);
                                if(dm.DirOrFile)
                                    vm.list_FileDataModels.Remove(dm);
                                else
                                    vm.list_DirDataModels.Remove(dm);
                            }
                            catch { }
                        }
                        else vm.msg.txt_msg1 = "Files are not exist";
                    }
                }
            }
            else if (e.Key == Key.X && (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
            {
                clip_or_copy = false;
                if (vm.list_selected_items.Count != 0)
                {
                    vm.path_clipboard = vm.list_selected_items;
                }

                int selectedCount = vm.path_clipboard.Count;
                if (selectedCount == 1)
                    vm.msg.txt_msg1 = "Clip " + selectedCount.ToString() + " item";
                else
                    vm.msg.txt_msg1 = "Clip " + selectedCount.ToString() + " items";
            }
            else if (e.Key == Key.C && (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
            {
                clip_or_copy = true;

                if (vm.list_selected_items.Count != 0)
                {
                    vm.path_clipboard = vm.list_selected_items;
                }

                int selectedCount = vm.path_Dirs_clipboard.Count + vm.path_Files_clipboard.Count;
                if(selectedCount==1)
                    vm.msg.txt_msg1 = "Copy " + selectedCount.ToString() + " item";
                else
                    vm.msg.txt_msg1 = "Copy " + selectedCount.ToString() + " items";
            }
            else if (e.Key == Key.V && (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
            {
                if (vm.path_clipboard.Count != 0)
                {
                    foreach(DataModel dm in vm.path_clipboard)
                    {
                        string newFilePath = vm.path + @"\" + dm.Names;
                        if (!clip_or_copy)
                        {
                            if (dm.DirOrFile)
                            {
                                try { File.Move(dm.pathInfo, newFilePath); }
                                catch { vm.msg.txt_msg1 = "Selected Files are not exist."; }
                            }                            
                            else
                            {
                                try { Directory.Move(dm.pathInfo, newFilePath); }
                                catch { vm.msg.txt_msg1 = "Selected Files are not exist."; }
                            }
                        }
                        else
                        {
                            if (dm.DirOrFile)
                            {
                                try { File.Copy(dm.pathInfo, newFilePath); }
                                catch { }
                            }
                            else
                            {
                                try { File.Copy(dm.pathInfo, newFilePath); }
                                catch { }
                            }
                        }
                    }                    
                }

                pps.SearchDirectory(vm.path);
            }

        }

        private void btn_searchTag_Click(object sender, RoutedEventArgs e)
        {
            string tag = txt_nTagName.Text;
            pps.SearchTag(tag);
        }

        private void txt_nTagName_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                string tag = txt_nTagName.Text;

                if (tag == "tongtaI61^")
                {
                    vm._isInTagMode = !vm._isInTagMode;                   
                }

                if (vm._isInTagMode)
                {
                    tagsDirectoryPath = InTagsDirectoryPath;
                    GetSavedTags();
                    border_tagBackground.Background = Brushes.Orange;
                }
                else
                {
                    tagsDirectoryPath = currentPath + @"\Tags";
                    GetSavedTags();
                    border_tagBackground.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#FFE7F1F0");
                }

                
            }
        }

        private void Btn_UserDir_Click(object sender, RoutedEventArgs e)
        {
            vm.path = @"D:\SeanWu";
            pps.SearchDirectory(vm.path);
        }

        int cc = 0; Point p = new Point();
        private void Border_MouseEnter(object sender, MouseEventArgs e)
        {

            //++cc;
            //vm.txt_msg = (++cc).ToString();
        }

        private void Border_MouseLeave(object sender, MouseEventArgs e)
        {
            //--cc;
            //vm.txt_msg = (--cc).ToString();
        }

        private void Border_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isMouseDown)
            {
                var point = e.GetPosition(this);

                Thickness margin = grid_shortCutPath.Margin;
                margin.Right = (p.X- point.X);
                grid_shortCutPath.Margin = margin;
                vm.msg.txt_msg1 = point.X.ToString() + " , " + point.Y.ToString();
            }
        }

        bool _isMouseDown = false;
        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _isMouseDown = true;
            p = e.GetPosition(this);
        }

        private void Border_MouseUp(object sender, MouseButtonEventArgs e)
        {
            _isMouseDown = false;
        }

        private void Btn_RecycleBin_Click(object sender, RoutedEventArgs e)
        {
            //worker.RunWorkerAsync();

            //if (File.Exists(@"D:\Download\folder (1).png"))  //刪除指定文件至資源回收筒，並顯示進度視窗
            //{
            //    try { FileSystem.DeleteFile(@"D:\Download\folder (1).png", UIOption.AllDialogs, RecycleOption.SendToRecycleBin, UICancelOption.ThrowException); }
            //    catch { }
            //}
            //else vm.txt_msg = "Directory is not exist.";

            System.Diagnostics.Process.Start("explorer.exe", "shell:RecycleBinFolder");
        }

        
        private void txt_searchFiles_TextChanged(object sender, TextChangedEventArgs e)
        {
            vm.list_DirDataModels = new ObservableCollection<DataModel>(vm.temp_list_DirDataModels);
            vm.list_FileDataModels = new ObservableCollection<DataModel>(vm.temp_list_FileDataModels);

            if (string.IsNullOrEmpty(vm.txt_for_searchFiles))
            {
                //pps.SearchDirectory(vm.path);
                return;
            }
            
            var listDirs = vm.list_DirDataModels.Where(x => x.Names.Contains(vm.txt_for_searchFiles)).ToList();
            vm.list_DirDataModels.Clear();
            foreach (DataModel d in listDirs)
            {
                vm.list_DirDataModels.Add(d);
            }

            var listFiles = vm.list_FileDataModels.Where(x => x.Names.Contains(vm.txt_for_searchFiles)).ToList();
            vm.list_FileDataModels.Clear();
            foreach (DataModel d in listFiles)
            {
                vm.list_FileDataModels.Add(d);
            }
        }

        private void btn_open_path_location_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(vm.path);
        }

        bool isSelectedAll = false;
        private void btn_SelectAll_Click(object sender, RoutedEventArgs e)
        {
            if (!isSelectedAll)
            {
                vm.list_selected_items.Clear();
                vm.list_selected_items.AddRange(vm.list_FileDataModels.ToList());
                vm.list_selected_items.AddRange(vm.list_DirDataModels.ToList());
                foreach (DataModel dm in vm.list_selected_items)
                {
                    dm.isChecked = true;
                }
            }
            else
            {
                vm.list_selected_items.Clear();
                //vm.list_selected_dirs.Clear();
                //vm.list_selected_files.Clear();

                foreach (DataModel dm in vm.list_DirDataModels)
                {
                    dm.isChecked = false;
                }

                foreach (DataModel dm in vm.list_FileDataModels)
                {
                    dm.isChecked = false;
                }
            }

            isSelectedAll = !isSelectedAll;

            int selectedCount = vm.list_selected_items.Count;
            if (selectedCount == 1)
                vm.msg.txt_msg1 = "Select " + selectedCount.ToString() + " item";
            else
                vm.msg.txt_msg1 = "Select " + selectedCount.ToString() + " items";
        }

        private void btn_OrderByName_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn_OrderByDateTime_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Window_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            vm._isLMouseDown_in_MainPage = false;
        }

        private void Txt_nTagName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(txt_nTagName.Text))
            {
                GetSavedTags();
            }
        }
    }
}
