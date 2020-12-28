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
using System.Windows.Input;
using System.Windows.Media;
using System.ComponentModel;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Controls.Primitives;
using Microsoft.VisualBasic.FileIO;
using Xabe.FFmpeg;
using WpfPageTransitions;

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
        Page_Setting _page_Setting;
        Page_CurrentPage _page_CurrentPage_2;

        System.Timers.Timer timer_showFilebox = new System.Timers.Timer();
                
        string currentPath = Directory.GetCurrentDirectory();
        string tagsDirectoryPath = "", InTagsDirectoryPath = "";

        public MainWindow()
        {           
            InitializeComponent();

            this.DataContext = vm;

            _page_CurrentPage = new Page_CurrentPage(vm);
            _page_Setting = new Page_Setting(vm);
            _page_CurrentPage.Width = border_PageBackground.Width / 2;
            //vm.tagsDirectoryPath = currentPath + @"\Tags";
            //vm.IntagsDirectoryPath = currentPath + @"\InTags";
            //vm.ini_path = currentPath + @"\Setting\Instrument.ini";

            vm.path = @"D:\Download"; ;
            //Binding newBinding = new Binding(vm.pageModel_1.pageName);
            //_page_CurrentPage.grid_main.SetBinding(Grid.TagProperty, newBinding);
            //vm.pageModel_1.pageName = vm.path;

            vm.tagsDirectoryPath = Directory.GetParent(vm.ini_path) + @"\Tags";
            vm.IntagsDirectoryPath = Directory.GetParent(vm.ini_path) + @"\InTags";
            vm.DirectFolders_DirectoryPath = Directory.GetParent(vm.ini_path) + @"\DirectFolders\DirecFolders.txt";

            tagsDirectoryPath = vm.tagsDirectoryPath;
            InTagsDirectoryPath = vm.IntagsDirectoryPath;

            Binding myBinding = new Binding("list_files[1]");
            myBinding.Source = vm;

            pps = new PathProcess(vm);
           
            //itemsource_directFolders.ItemsSource = vm.list_DirectFolderModels;
            grid_msg.DataContext = vm.msg;

            #region Background worker setting
            vm.worker.WorkerReportsProgress = true;
            vm.worker.DoWork += new DoWorkEventHandler(pps.DoWork);
            vm.worker.ProgressChanged += new ProgressChangedEventHandler(pps.DuringWork);
            vm.worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(pps.RunWorkerCompleted);

            //vm.worker_getFileSize.WorkerReportsProgress = true;
            //vm.worker_getFileSize.DoWork += new DoWorkEventHandler(pps.DoWork_GetSize);
            //vm.worker_getFileSize.ProgressChanged += new ProgressChangedEventHandler(pps.DuringWork_GetSize);
            //vm.worker_getFileSize.RunWorkerCompleted += new RunWorkerCompletedEventHandler(pps.RunWorkerCompleted_GetSize);

            vm.timer.Interval = TimeSpan.FromMilliseconds(20);
            vm.timer.Tick += pps._timer_Tick;
            #endregion
        }

        
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //style_tag = Application.Current.FindResource("BtnStyle_TagBox") as Style;
            //btn_tag.Style = style_tag;

            //建立存放影片大頭照資料夾
            if (!Directory.Exists(vm.ThumbnailPath))
            {
                Directory.CreateDirectory(vm.ThumbnailPath);
            }

            pps.GetSavedDirectFolders(vm.DirectFolders_DirectoryPath);

            pps.GetSavedTags(tagsDirectoryPath, InTagsDirectoryPath);

            vm.unigrid_column = (int)Math.Truncate(pageTransitionControl.ActualWidth / 140);
            
            pps.SearchDirectory(vm.path);

            pageTransitionControl.ShowPage(_page_CurrentPage);

            //vm.unigrid_column = (int)Math.Truncate(pageTransitionControl.ActualWidth / 140);
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (stk_mainPage.Children.Count < 2)
            {
                if (pageTransitionControl.ActualWidth != 0)
                {
                    //vm.unigrid_column = (int)Math.Truncate(border_PageBackground.ActualWidth / 140);
                    vm.msg.txt_msg2 = vm.unigrid_column.ToString();
                }
            }
            else
            {
                int width = (int)Math.Truncate(border_PageBackground.ActualWidth / 140);
                if (width % 2 > 0)  //odd
                {
                    //vm.unigrid_column = (width - 1) / 2;
                }
                //else vm.unigrid_column = (width) / 2;

                vm.msg.txt_msg2 = stk_mainPage.ActualWidth.ToString();


                pageTransitionControl.Width = border_PageBackground.ActualWidth / 2;
                _page_CurrentPage_2.Width = border_PageBackground.ActualWidth / 2;
            }
        }

        private void Txt_path_PreviewKeyDown(object sender, KeyEventArgs e)
        {            
            if (e.Key == Key.Enter)
            {
                TextBox tbk = (TextBox)sender;
                if (tbk.Text != vm.path)
                {
                    vm.list_PathBoxModels.Clear();
                    pps.SearchDirectory(@tbk.Text);
                    vm.pageModel_1.pageName = vm.path;
                }

                //手動更新Property(Textbox的UpdateSourceTrigger = Explicit)
                BindingExpression binding = tbk.GetBindingExpression(TextBox.TextProperty);
                binding.UpdateSource();

                vm.Visibility_txt_path = true;                
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


        //Style style_tag;
        private void Btn_addTagWindow_Click(object sender, RoutedEventArgs e)
        {          
            _window_addTags = new Window_AddTags(vm);

            if (_window_addTags.ShowDialog() == false)
            {
                //if (!string.IsNullOrEmpty(_window_addTags.NewTagName))
                //{
                //    Button btn_tag = new Button { Content = _window_addTags.NewTagName };
                //    btn_tag.Style = style_tag;
                //    btn_tag.Click += Btn_tag_Click;
                   
                //    if (!vm.dictonary_tag_files.ContainsKey(_window_addTags.NewTagName))
                //        vm.dictonary_tag_files.Add(_window_addTags.NewTagName, new ObservableCollection<string>());
                //}                    
            }
        }

        //當標籤左鍵點擊時，呼叫有此標籤的物件
        private void Btn_tag_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton btn = (ToggleButton)sender;
            TagModel tm = (TagModel)btn.DataContext;
            string tag = tm.tagName;

            //將此標籤加入/移除List_selectedTag
            if ((bool)btn.IsChecked)
            {
                if (!vm.list_selectedTagModels.Contains(tm)) vm.list_selectedTagModels.Add(tm);
            }
            else
            {
                vm.list_selectedTagModels.Remove(tm);
            }

            pps.Refresh_Taged_File(vm.list_selectedTagModels.ToList());
            //pps.Refresh_Taged_File(tagsDirectoryPath);

            vm.list_selected_items.Clear();
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

                //pps.Refresh_Taged_File(tagsDirectoryPath);
            }
        }

      

      

        private void btn_tag_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            ToggleButton btn = (ToggleButton)sender;
            TagModel tm = (TagModel)btn.DataContext;
            string tag = tm.tagName;

            if (vm._isTagRemoveMode)
            {                
                if (vm.list_TagModels.Contains(tm)) vm.list_TagModels.Remove(tm);
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
            if (vm.list_TagModels.Count <= 0) return;

            if (Directory.Exists(tagsDirectoryPath))
            {
                string[] tagsPath = Directory.GetFiles(tagsDirectoryPath);
                List<string> tags_already = new List<string>();
                foreach (string s in tagsPath)
                {
                    tags_already.Add(Path.GetFileNameWithoutExtension(s));
                }

                foreach(TagModel tm in vm.list_TagModels)
                {
                    string s = tm.tagName;
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

        //private void GetSavedTags()
        //{           
        //    if (Directory.Exists(tagsDirectoryPath))
        //    {
        //        vm.list_TagModels = new ObservableCollection<TagModel>();
        //        vm.list_tags = new System.Collections.ObjectModel.ObservableCollection<string>();
        //        string[] tagsPath = Directory.GetFiles(tagsDirectoryPath);
        //        foreach (string s in tagsPath)
        //        {
        //            string tag = Path.GetFileNameWithoutExtension(s);
        //            vm.list_tags.Add(tag);

        //            if (!vm.dictonary_tag_files.ContainsKey(tag))
        //                vm.dictonary_tag_files.Add(tag, new ObservableCollection<string>());

        //            TagModel tagModel = new TagModel() { tagName = tag, isChecked = false };
        //            vm.list_TagModels.Add(tagModel);
        //        }
        //    }
        //    else
        //    {
        //        Directory.CreateDirectory(tagsDirectoryPath);
        //    }

        //    #region Invisible tags
        //    if (!Directory.Exists(InTagsDirectoryPath))
        //    {
        //        Directory.CreateDirectory(InTagsDirectoryPath);
        //    }
        //    #endregion
        //}

        private void btn_selectedTagClear_Click(object sender, RoutedEventArgs e)
        {
            vm.list_fileNames.Clear();
            vm.list_files.Clear();
            vm.list_selectedTagModels.Clear();

            pps.GetSavedTags(tagsDirectoryPath, InTagsDirectoryPath);

            //string[] keys = vm.dictonary_tag_files.Keys.ToArray();
            //foreach(string key in keys)
            //{
            //    vm.dictonary_tag_files[key] = new List<string>();
            //}
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
                foreach(TagModel tm in vm.list_selectedTagModels)
                {
                    string tag = tm.tagName;
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
                    //pageTransitionControl.ShowPage(_page_CurrentPage);
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

                //pageTransitionControl.ShowPage(_page_CurrentPage);

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

                //pageTransitionControl.ShowPage(_page_CurrentPage);
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
            Task.Delay(500);
            vm.pageModel_1.pageSize_Width = border_PageBackground.ActualWidth;
        }

        private void btn_close_Click(object sender, RoutedEventArgs e)
        {
            //刪除影片用大頭照
            string[] files = Directory.GetFiles(vm.ThumbnailPath);
            if (files.Length > 0)
            {
                foreach (string s in files)
                    File.Delete(s);
            }

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
                try
                {
                    mRestoreForDragMove = false;
                    this.DragMove();
                }
                catch { }
            }
        }

        private void border_title_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            mRestoreForDragMove = false;
        }

        private void btn_deleteTag_Click(object sender, RoutedEventArgs e)
        {
            if (vm.list_selectedTagModels.Count == 0)
            {
                MessageBox.Show("No selected tag");
                return;
            }                

            if (MessageBox.Show("Delete tags ?", "Question", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                foreach (TagModel tm in vm.list_selectedTagModels)
                {
                    string tag = tm.tagName;
                    string tagTxtPath = tagsDirectoryPath + @"\" + tag + @".txt";   //Txt path of this tag

                    if (File.Exists(tagTxtPath))
                    {
                        FileSystem.DeleteFile(tagTxtPath, UIOption.AllDialogs, RecycleOption.SendToRecycleBin, UICancelOption.ThrowException);
                    }
                    else vm.msg.txt_msg1 = tag + " not exist";
                    
                    List<TagModel> tagModels = vm.list_TagModels.Where(x => x.tagName == tag).ToList();
                    foreach (TagModel t in tagModels)
                        vm.list_TagModels.Remove(t);
                }

                vm.list_selectedTagModels.Clear();

                pps.SearchDirectory(vm.path);

                MessageBox.Show("Done");
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
                        string newFilePath = vm.path + @"\" + dm.Name;
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
            else  //Search texbox key in
            {
                if (e.Key != Key.LeftCtrl && e.Key != Key.RightCtrl)
                   if(!txt_nTagName.IsFocused)
                        txt_searchFiles.Focus();
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
                    pps.GetSavedTags(tagsDirectoryPath, InTagsDirectoryPath);
                    border_tagBackground.Background = Brushes.Orange;
                }
                else
                {
                    tagsDirectoryPath = vm.tagsDirectoryPath;
                    pps.GetSavedTags(tagsDirectoryPath, InTagsDirectoryPath);
                    border_tagBackground.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#FFE7F1F0");
                }

                
            }
        }

        private void Btn_DirecFolder_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;

            DirectFolderModel dfm;
            try
            {
                dfm = (DirectFolderModel)btn.DataContext;
            }
            catch { return; }

            vm.path = dfm.pathInfo;
            pps.SearchDirectory(dfm.pathInfo);

            if (_isPage)
                pageTransitionControl.ShowPage(_page_CurrentPage);

            //foreach (DataModel dm in vm.list_DirDataModels)
            //{
            //    dm.fileboxSize_Width = vm.pageModel_1.fileboxSize_Width;
            //}

            //foreach (DataModel dm in vm.list_FileDataModels)
            //{
            //    dm.fileboxSize_Width = vm.pageModel_1.fileboxSize_Width;
            //}
        }

        private void Border_MouseEnter(object sender, MouseEventArgs e)
        {
            if(!_isMouseDown)
                if (this.Cursor != Cursors.Wait)
                    Mouse.OverrideCursor = Cursors.Hand;
        }

        private void Border_MouseLeave(object sender, MouseEventArgs e)
        {
            if(!_isMouseDown)
                if (this.Cursor != Cursors.Wait)
                    Mouse.OverrideCursor = Cursors.Arrow;
        }

        Point p = new Point();
        bool _isMouseDown = false;
        private void Border_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isMouseDown)
            {
                var point = e.GetPosition(col_pathCollection);
                double delta_P = p.X - point.X;
                vm.colum_collectionPath_width = vm.colum_collectionPath_width - (delta_P);
                p = e.GetPosition(col_pathCollection);
                vm.msg.txt_msg4 = delta_P.ToString();
                vm.msg.txt_msg5 = vm.colum_collectionPath_width.ToString();
                
                //Thickness margin = grid_shortCutPath.Margin;
                //margin.Right = (p.X- point.X);
                //grid_shortCutPath.Margin = margin;
                //vm.msg.txt_msg1 = point.X.ToString() + " , " + point.Y.ToString();
            }
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _isMouseDown = true;
            p = e.GetPosition(col_pathCollection);
        }

        private void Border_MouseUp(object sender, MouseButtonEventArgs e)
        {
            _isMouseDown = false;

            if (this.Cursor != Cursors.Wait)
                Mouse.OverrideCursor = Cursors.Arrow;
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
            
            var listDirs = vm.list_DirDataModels.Where(x => x.Name.Contains(vm.txt_for_searchFiles)).ToList();
            vm.list_DirDataModels.Clear();
            foreach (DataModel d in listDirs)
            {
                vm.list_DirDataModels.Add(d);
            }

            var listFiles = vm.list_FileDataModels.Where(x => x.Name.Contains(vm.txt_for_searchFiles)).ToList();
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
                foreach (DataModel dm in vm.list_DirDataModels)
                {
                    dm.isChecked = false;
                }

                foreach (DataModel dm in vm.list_FileDataModels)
                {
                    dm.isChecked = false;
                }

                vm.list_selected_items.Clear();
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
            var list_dirs = vm.list_DirDataModels.OrderBy(x => x.Name).ToList();
            vm.list_DirDataModels = new ObservableCollection<DataModel>(list_dirs);

            var list_files = vm.list_FileDataModels.OrderBy(x => x.Name).ToList();
            vm.list_FileDataModels = new ObservableCollection<DataModel>(list_files);
        }

        private void btn_OrderByDateTime_Click(object sender, RoutedEventArgs e)
        {
            var list_dirs = vm.list_DirDataModels.OrderBy(x => x.updateTime).ToList();
            vm.list_DirDataModels = new ObservableCollection<DataModel>(list_dirs);

            var list_files = vm.list_FileDataModels.OrderBy(x => x.updateTime).ToList();
            vm.list_FileDataModels = new ObservableCollection<DataModel>(list_files);
        }

        private void Btn_OrderByType_Click(object sender, RoutedEventArgs e)
        {
            var list_files = vm.list_FileDataModels.OrderBy(x => x.ExtensionName).ToList();
            vm.list_FileDataModels = new ObservableCollection<DataModel>(list_files);

            List<string> tags = new List<string>() { "#txt", "#zip", "#pdf" };
            foreach(string tag in tags)
            {
                pps.AddNewTag(tag, false);
            }            
        }

       
        bool _isPage = false;
        private void Btn_Setting_Click(object sender, RoutedEventArgs e)
        {
            if(_isPage)
                pageTransitionControl.ShowPage(_page_CurrentPage);
            else
                pageTransitionControl.ShowPage(_page_Setting);

            _isPage = !_isPage;
        }

        private void Btn_Test_Click_1(object sender, RoutedEventArgs e)
        {
            pps.GetVideoFrames(@"D:\Download\1234.mp4");
            //PathProcess.generateThumb(@"D:\Download\1234.mp4", @"D:\Download\1234.jpg", 100);
        }

        

        private void Border_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            vm.Visibility_txt_path = false;
            txt_path.Focus();
        }

        private void btn_pathBox_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            PathBoxModel pathBox = (PathBoxModel)btn.DataContext;
            vm.path = pathBox.pathInfo;
            pps.SearchDirectory(vm.path);
        }

        private void Txt_path_LostFocus(object sender, RoutedEventArgs e)
        {
            vm.Visibility_txt_path = true;
        }

        private void Txt_search_MouseEnter(object sender, MouseEventArgs e)
        {
            if (this.Cursor != Cursors.Wait)
                Mouse.OverrideCursor = Cursors.IBeam;
        }

        private void Txt_search_MouseLeave(object sender, MouseEventArgs e)
        {
            if (this.Cursor != Cursors.Wait)
                Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void MenuItem_SavePath_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(vm.path))
            {
                DirectFolderModel model = new DirectFolderModel() { pathInfo = vm.path, Name = new DirectoryInfo(vm.path).Name, user = "user", visible = true };

                if (!vm.list_DirectFolderModels.Contains(model))
                {
                    vm.list_DirectFolderModels.Add(model);
                }

                //vm.dictonary_tag_files.Add(tag, new List<string>());

                string DirectFolderTxtPath = vm.DirectFolders_DirectoryPath;   //Txt path of this tag
                if (!File.Exists(DirectFolderTxtPath))
                    using (StreamWriter sw = File.CreateText(@DirectFolderTxtPath)) { }  //建立空的文件檔

                string[] lines = System.IO.File.ReadAllLines(DirectFolderTxtPath);
                try
                {
                    using (StreamWriter file = new StreamWriter(@DirectFolderTxtPath, true))
                    {
                        foreach (DirectFolderModel dfm in vm.list_DirectFolderModels)
                        {
                            if (!lines.Contains(dfm.pathInfo))
                            {
                                //vm.dictonary_tag_files[tag].Add(dm.pathInfo);
                                file.WriteLine(dfm.pathInfo);  //寫入選取的檔案or資料夾 路徑
                            }
                        }
                    }
                }
                catch { }

               
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //string desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);//桌面路徑 
            vm.path = SpecialFolders.GetSpecialFolderPath("Desktop");
            pps.SearchDirectory(vm.path);
        }

        private void Txt_nTagName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(txt_nTagName.Text))
            {
                pps.GetSavedTags(tagsDirectoryPath, InTagsDirectoryPath);
            }
        }

        private void txt_nTagName_LostFocus(object sender, RoutedEventArgs e)
        {
            txt_searchFiles.Focus();
        }

        private void btn_C_Click(object sender, RoutedEventArgs e)
        {
            vm.path = @"C:\";
            pps.SearchDirectory(vm.path);
        }

        private void btn_D_Click(object sender, RoutedEventArgs e)
        {
            vm.path = @"D:\";
            pps.SearchDirectory(vm.path);
        }

        private void menuItem_Delete_Click(object sender, RoutedEventArgs e)
        {
            if(sender is MenuItem)
            {
                MenuItem btn = (MenuItem)sender;
                DirectFolderModel dfm = (DirectFolderModel)btn.DataContext;
                if (vm.list_DirectFolderModels.Contains(dfm))
                {
                    vm.list_DirectFolderModels.Remove(dfm);
                }
            }
        }

        private void btn_addPage_Click(object sender, RoutedEventArgs e)
        {
            if (stk_mainPage.Children.Count < 2)
            {
                pageTransitionControl.Width = border_PageBackground.ActualWidth / 2;
                _page_CurrentPage_2 = new Page_CurrentPage(vm);
                _page_CurrentPage_2.Name = "page_2";
                _page_CurrentPage_2.Width = border_PageBackground.ActualWidth / 2;

                vm.unigrid_column = (int)Math.Truncate(pageTransitionControl.ActualWidth / 140);
                if (vm.unigrid_column % 2 > 0)  //odd
                {
                    vm.unigrid_column = (vm.unigrid_column - 1) / 2;
                }
                else vm.unigrid_column = (vm.unigrid_column) / 2;

                stk_mainPage.Children.Add(_page_CurrentPage_2);
                vm.multiPages = true;
            }
            else
            {
                stk_mainPage.Children.RemoveAt(stk_mainPage.Children.Count-1);
                vm.multiPages=false;
            }
        }

        private void border_PageBackground_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            vm.pageModel_1.pageSize_Width = border_PageBackground.ActualWidth;
            //MessageBox.Show(vm.pageModel_1.pageSize_Width.ToString());
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            foreach(DataModel dm in vm.list_DirDataModels)
            {
                dm.fileboxSize_Width = vm.pageModel_1.fileboxSize_Width;
            }

            foreach(DataModel dm in vm.list_FileDataModels)
            {
                dm.fileboxSize_Width = vm.pageModel_1.fileboxSize_Width;
            }
        }

        private async void GetVideoImage(string inputPath, string outputPath)
        {
            //string output = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + FileExtensions.Png);
            IConversionResult result = await Conversion.Snapshot(inputPath, outputPath, TimeSpan.FromSeconds(0)).Start();

        }
      

    }

}
