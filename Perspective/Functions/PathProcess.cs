using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Perspective.ViewModels;
using Perspective.Models;

namespace Perspective.Functions
{
    public class PathProcess
    {
        VM vm;
        string currentPath = Directory.GetCurrentDirectory();

        public PathProcess()
        {

        }

        public PathProcess(VM vm)
        {
            this.vm = vm;
        }

        public void GetSavedDirectFolders(string DirectFoldersPath)
        {
            if (File.Exists(DirectFoldersPath))
            {
                vm.list_DirectFolderModels = new ObservableCollection<DirectFolderModel>();

                string[] DirectFolders = File.ReadAllLines(DirectFoldersPath);
                foreach (string s in DirectFolders)
                {
                    var dirName = new DirectoryInfo(s).Name;
                    DirectFolderModel dfm = new DirectFolderModel() { Name = dirName, pathInfo = s, user = "user", visible = true };
                    vm.list_DirectFolderModels.Add(dfm);
                }
            }
            else
            {
                if (!Directory.Exists(@"D:\MobiusLink"))
                {
                    Directory.CreateDirectory(@"D:\MobiusLink");
                }
                if (!Directory.Exists(new DirectoryInfo(DirectFoldersPath).Parent.FullName))
                {
                    Directory.CreateDirectory(new DirectoryInfo(DirectFoldersPath).Parent.FullName);
                }                    
                File.Create(DirectFoldersPath);
            }
        }

        public void GetSavedTags(string tagsDirectoryPath, string InTagsDirectoryPath)
        {
            if (Directory.Exists(tagsDirectoryPath))
            {
                vm.list_TagModels = new ObservableCollection<TagModel>();

                string[] tagsPath = Directory.GetFiles(tagsDirectoryPath);
                foreach (string s in tagsPath)
                {
                    string tag = Path.GetFileNameWithoutExtension(s);
                    //vm.list_tags.Add(tag);

                    if (!vm.dictonary_tag_files.ContainsKey(tag))
                    {
                        vm.dictonary_tag_files.Add(tag, new List<string>());

                        Get_AllFilesPath_in_Tags(s);
                    }                        

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

        public void AddNewTag(string tag, bool _isCreatTagTxt)
        {
            if (!string.IsNullOrEmpty(tag))
            {
                TagModel model = new TagModel() { tagName = tag, isChecked = false };

                List<TagModel> list = vm.list_TagModels.Where(x => x.tagName == tag).ToList();
                if (list.Count==0)
                {
                    vm.list_TagModels.Add(model);
                }

                if (_isCreatTagTxt)
                {
                    vm.dictonary_tag_files.Add(tag, new List<string>());

                    string tagTxtPath = currentPath + @"\Tags\" + tag + @".txt";   //Txt path of this tag
                    if (!File.Exists(tagTxtPath))
                        using (StreamWriter sw = File.CreateText(@tagTxtPath)) { }  //建立空的文件檔
                }
            }
        }

        public int IsImageJudge(string path)
        {
            int _isImg = 0;

            string fileExtension = Path.GetExtension(path);

            string[] list_img_extension = new string[] { ".png", ".jpg", ".bmp", "jpeg" };            
            foreach(string s in list_img_extension)
            {
                if (string.Compare(fileExtension, s) == 0)
                {
                    _isImg = 1;
                    break;
                }
            }

            string[] list_video_extension = new string[] { ".mp4" };
            foreach (string s in list_video_extension)
            {
                if (string.Compare(fileExtension, s) == 0)
                {
                    _isImg = 2;
                    break;
                }
            }

            return _isImg;
        }

        public string FileBox_NameExtensionJudge(string path)
        {
            string img_strSource = "";           

            string fileExtention = Path.GetExtension(path);
            switch (fileExtention)
            {
                case ".txt":
                    img_strSource = currentPath + @"\ImgSource\Text.png";
                    break;
                case ".xlsx":
                    img_strSource = currentPath + @"\ImgSource\excel.png";
                    break;
                case ".csv":
                    img_strSource = currentPath + @"\ImgSource\excel.png";
                    break;
                case ".pdf":
                    img_strSource = currentPath + @"\ImgSource\pdf.png";
                    break;
                case ".zip":
                    img_strSource = currentPath + @"\ImgSource\zip.png";                    
                    break;
                case ".7z":
                    img_strSource = currentPath + @"\ImgSource\7z.png";
                    break;
                case ".doc":
                    img_strSource = currentPath + @"\ImgSource\doc.png";
                    break;
                case ".docx":
                    img_strSource = currentPath + @"\ImgSource\doc.png";
                    break;
                case ".ppt":
                    img_strSource = currentPath + @"\ImgSource\ppt.png";
                    break;
                case ".pptx":
                    img_strSource = currentPath + @"\ImgSource\ppt.png";
                    break;
                case ".gif":
                    img_strSource = currentPath + @"\ImgSource\gif.png";
                    break;
                case ".mp3":
                    img_strSource = currentPath + @"\ImgSource\mp3.png";
                    break;
                case ".cad":
                    img_strSource = currentPath + @"\ImgSource\cad.png";
                    break;
                case ".ai":
                    img_strSource = currentPath + @"\ImgSource\ai.png";
                    break;
                default:
                    img_strSource = currentPath + @"\ImgSource\paper.png";
                    break;
            }

            return img_strSource;
        }

        public void SearchTag(string tag)
        {
            if (string.IsNullOrEmpty(tag)) return;

            var sTag = vm.list_TagModels.Where(x => x.tagName == tag).ToList();

            vm.list_TagModels.Clear();
            foreach (TagModel t in sTag)
            {
                vm.list_TagModels.Add(t);
            }
        }

        public void SearchDirectory(string path)
        {
            try
            {
                if (string.IsNullOrEmpty(path)) return;

                vm.list_DirDataModels.Clear();
                vm.list_FileDataModels.Clear();
                //vm.list_files.Clear();
                vm.list_directories.Clear();
                //vm.list_dirNames.Clear();
                //vm.list_fileNames.Clear();                

                if (File.Exists(@path))  // This path is a file
                {
                    ProcessFile(@path);
                }
                else if (Directory.Exists(@path))  // This path is a directory
                {                    
                    vm.timer.Stop();
                    timerCount = 0;
                    ProcessDirectory(@path);                    
                }
                else
                {
                    Console.WriteLine("{0} is not a valid file or directory.", path);
                }
            }
            catch { }
        }

        public BitmapImage LoadImage(string path)
        {
            var bitmap = new BitmapImage();

            int code = IsImageJudge(path);
            if (code == 1)  //image
            {
                using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    try
                    {
                        bitmap.BeginInit();
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.StreamSource = stream;
                        bitmap.EndInit();
                    }
                    catch
                    {
                        bitmap = new BitmapImage();
                    }
                }
            }
            else if (code == 2)  //video
            {
                //System.Windows.Shell.ShellFile shellFile = ShellFile.FromFilePath(VideoFileName);
                //Bitmap bm = shellFile.Thumbnail.Bitmap;

                //add_Video_Image(path);

                //var renderTargetBitmap = source;
                //var bitmapImage = new BitmapImage();
                //var bitmapEncoder = new PngBitmapEncoder();
                //bitmapEncoder.Frames.Add(BitmapFrame.Create(renderTargetBitmap));

                //using (var stream = new MemoryStream())
                //{
                //    bitmapEncoder.Save(stream);
                //    stream.Seek(0, SeekOrigin.Begin);

                //    bitmapImage.BeginInit();
                //    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                //    bitmapImage.StreamSource = stream;
                //    bitmapImage.EndInit();
                //}

                //bitmap = bitmapImage;
            }
            else
            {
                string ph = FileBox_NameExtensionJudge(path);
                if (!string.IsNullOrEmpty(ph))
                {
                    using (var stream = new FileStream(ph, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        bitmap.BeginInit();
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.StreamSource = stream;
                        bitmap.EndInit();
                    }
                }
            }

            return bitmap;
        }

        private void ProcessFile(string path)
        {
            string path_directory_of_file = System.IO.Path.GetDirectoryName(path);

            System.Diagnostics.Process.Start(path);

            ProcessDirectory(path_directory_of_file);
        }

        private void ProcessDirectory(string targetDirectory)
        {
            #region 搜尋本資料夾內的所有資料夾
            string[] directories = System.IO.Directory.GetDirectories(targetDirectory);

            foreach (string s in directories)
            {
                if (Directory.Exists(@s))
                {
                    // This path is a directory
                    DirectoryInfo di = new DirectoryInfo(s);
                    vm.list_DirDataModels.Add(new DataModel() { Name = Path.GetFileName(s), Visibility_btn_remove = false,
                        pathInfo = s, DirOrFile=false, updateTime=di.LastWriteTime, creationTime=di.CreationTime });
                }
            }
            #endregion

            #region 搜尋本資料夾內的所有檔案
            string[] files = Directory.GetFiles(targetDirectory);
            //DataModel[] dataModels = new DataModel[files.Length];

            vm.searchFiles_Result = files;
            
            vm.worker.RunWorkerAsync();   //Background worker start to set thumbnail image to file which is a picture
            #endregion
        }

        public void DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            for (int i = 0; i < vm.searchFiles_Result.Length; i++)
            {
                if (worker.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }
                string s = vm.searchFiles_Result[i];
                worker.ReportProgress((int)i, GetDataModel(s, false));  //將檔案路徑轉成檔案資訊
            }
        }

        public void DuringWork(object sender, ProgressChangedEventArgs e)
        {
            DataModel dm = (DataModel)e.UserState;
            if (dm != null)
                vm.list_FileDataModels.Add((DataModel)e.UserState);
        }

        //此步驟所有資料夾/檔案皆已載入頁面，尚餘檔案的小圖未載入
        public void RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //Save all files/dirs to temp list
            List<DataModel> ld = vm.list_DirDataModels.ToList();
            vm.temp_list_DirDataModels = new System.Collections.ObjectModel.ObservableCollection<DataModel>(ld);

            timerCount = 0;
            vm.timer.Start();

            int itemsCount = vm.list_FileDataModels.Count + vm.list_DirDataModels.Count;
            if (itemsCount == 1)
                vm.msg.txt_msg1 = itemsCount.ToString() + " item";
            else
                vm.msg.txt_msg1 = itemsCount.ToString() + " items";
        }

        //檔案載入小圖顯示
        int timerCount = 0;
        public void _timer_Tick(object sender, EventArgs e)
        {
            if (timerCount >= vm.list_FileDataModels.Count)
            {
                vm.timer.Stop();

                //Save all files/dirs to temp list
                //List<DataModel> ld = vm.list_DirDataModels.ToList();
                //vm.temp_list_DirDataModels = new System.Collections.ObjectModel.ObservableCollection<DataModel>(ld);

                //ld = vm.list_FileDataModels.ToList();
                //vm.temp_list_FileDataModels = new System.Collections.ObjectModel.ObservableCollection<DataModel>(ld);

               
                return;
            }
            string path = vm.list_FileDataModels[timerCount].pathInfo;
            vm.list_FileDataModels[timerCount].imgSource = LoadImage(path);
            vm.temp_list_FileDataModels.Add(vm.list_FileDataModels[timerCount]);

            timerCount++;
        }

        private DataModel GetDataModel(string path, bool _isImage)
        {
            DataModel dm;
            FileInfo fi = new FileInfo(path);
            var filtered = !fi.Attributes.HasFlag(FileAttributes.Hidden);  //Check file is hidden or not
            //var filtered = true;
            if (filtered)
            {
                if (_isImage)
                {
                    dm = new DataModel()
                    {
                        Name = fi.Name,
                        ExtensionName = fi.Extension,
                        Visibility_btn_remove = false,
                        pathInfo = path,
                        updateTime = fi.LastWriteTime,
                        creationTime = fi.CreationTime,
                        DirOrFile = true,
                        imgSource = LoadImage(path)
                    };
                }
                else
                {
                    dm = new DataModel()
                    {
                        Name = fi.Name,
                        ExtensionName = fi.Extension,
                        Visibility_btn_remove = false,
                        pathInfo = path,
                        updateTime = fi.LastWriteTime,
                        creationTime = fi.CreationTime,
                        DirOrFile = true
                    };
                }
            }
            else dm = null;
            return dm;
        }

        private void Get_AllFilesPath_in_Tags(string tagTxtPath)
        {
            string tag = Path.GetFileNameWithoutExtension(tagTxtPath);

            //呼叫具此標籤的檔案們              
            string[] lines;
            
            //Get all filepath in this tag
            if (File.Exists(tagTxtPath))
            {
                lines = File.ReadAllLines(tagTxtPath);

                vm.dictonary_tag_files[tag].Clear();
                foreach (string filepath in lines)
                {
                    vm.dictonary_tag_files[tag].Add(filepath);
                }
            }            
        }

        public void Refresh_Taged_File(List<TagModel> tagModels)
        {
            //var list_all_files_in_tags = new List<List<string>>();
            //List<List<string>> list_all_Dirs_in_tags = new List<List<string>>();

            vm.list_DirDataModels.Clear();
            vm.list_FileDataModels.Clear();

            FileAttributes attr;

            if (vm.list_selectedTagModels.Count != 0)
            {

                var list = new List<string>();
                for (int i = 0; i < tagModels.Count; i++)  //計算所有標籤中的物件路徑交集
                {
                    if (i == 0)
                    {
                        string tag = tagModels[i].tagName;

                        list = vm.dictonary_tag_files[tag];
                    }
                    else
                    {
                        list = list.Intersect(vm.dictonary_tag_files[tagModels[i].tagName]).ToList();
                    }
                }

                foreach(string s in list)
                {
                    //DataModel dataModel = new DataModel() { Name = Path.GetFileName(s), pathInfo = s, imgSource = LoadImage(s) };

                    vm.list_FileDataModels.Add(GetDataModel(s, true));
                }

                //var list_F_intersection = GetFilesIntersection(list_all_files_in_tags[0]);
                //for (int i = 1; i < list_all_files_in_tags.Count; i++)
                //{
                //    list_F_intersection = list_F_intersection.Intersect(list_all_files_in_tags[i]);
                //}

                //foreach (string s in list_F_intersection)
                //{
                //    DataModel dataModel = new DataModel() { Name = Path.GetFileName(s), pathInfo = s, imgSource = LoadImage(s) };

                //    vm.list_FileDataModels.Add(dataModel);
                //}

                //var list_D_intersection = GetFilesIntersection(list_all_Dirs_in_tags[0]);
                //for (int i = 1; i < list_all_Dirs_in_tags.Count; i++)
                //{
                //    list_D_intersection = list_D_intersection.Intersect(list_all_Dirs_in_tags[i]);
                //}

                //foreach (string s in list_D_intersection)
                //{
                //    vm.list_DirDataModels.Add(new DataModel() { Name = Path.GetFileName(s), pathInfo = s });
                //}
            }
            else
            {
                SearchDirectory(vm.path);
            }
        }

        

        public async void Set_FileBox_Info(DataModel dm)
        {
            // get the file attributes for file or directory
            FileAttributes attr = File.GetAttributes(dm.pathInfo);

            //detect whether its a directory or file
            if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
            {
                //非同步執行緒計算檔案大小
                Task task1 = Task.Factory.StartNew(() => 1);
                var directory_size = await Task.Run(() => DirectoryInfoExtension.GetSize(@dm.pathInfo));

                //var directory_size = DirectoryInfoExtension.GetSize(@dm.pathInfo);


                vm.msg.txt_msg3 = string.Concat("大小: ", MathCalculation.Calculate_FileSize(directory_size));
            }
            else
            {
                FileInfo fi = new FileInfo(dm.pathInfo);
                if (fi.Exists)
                {
                    vm.msg.txt_msg3 = string.Concat("大小: ", MathCalculation.Calculate_FileSize(fi.Length));
                }
            }
            vm.msg.txt_msg2 = dm.Name;

            vm.msg.txt_msg4 = string.Concat("建立日期: ", dm.creationTime.ToShortDateString(), " ", dm.creationTime.ToShortTimeString());
            vm.msg.txt_msg5 = string.Concat("修改日期: ", dm.updateTime.ToShortDateString(), " ", dm.updateTime.ToShortTimeString());

        }

        private IEnumerable<string> GetFilesIntersection(List<string> list)
        {
            List<string> listF = list;
            return listF;
        }

        private void add_Video_Image(string sFullname_Path_of_Video)
        {
            //----------------< add_Video_Image() >----------------
            //*create mediaplayer in memory and jump to position
            MediaPlayer mediaPlayer = new MediaPlayer();

            mediaPlayer.MediaOpened += new EventHandler(mediaplayer_OpenMedia);
            mediaPlayer.ScrubbingEnabled = true;
            mediaPlayer.Open(new Uri(sFullname_Path_of_Video));
            mediaPlayer.Position = TimeSpan.FromSeconds(0);
            //----------------</ add_Video_Image() >----------------           
        }
        RenderTargetBitmap source;
        private void mediaplayer_OpenMedia(object sender, EventArgs e)
        {
            //----------------< mediaplayer_OpenMedia() >----------------
            //*create mediaplayer in memory and jump to position
            //< draw video_image >
            MediaPlayer mediaPlayer = sender as MediaPlayer;
            DrawingVisual drawingVisual = new DrawingVisual();
            DrawingContext drawingContext = drawingVisual.RenderOpen();
            drawingContext.DrawVideo(mediaPlayer, new System.Windows.Rect(0, 0, 160, 100));
            drawingContext.Close();

            double dpiX = 1 / 200;
            double dpiY = 1 / 200;
            RenderTargetBitmap bmp = new RenderTargetBitmap(160, 100, dpiX, dpiY, PixelFormats.Pbgra32);
            bmp.Render(drawingVisual);
            //</ draw video_image >

            //< set Image >
            System.Windows.Controls.Image newImage = new System.Windows.Controls.Image();
            newImage.Source = bmp;
            newImage.Stretch = Stretch.Uniform;
            newImage.Height = 100;
            //</ set Image >

            //< add >
            source = bmp;
            //panel_Images.Children.Add(newImage);
            //</ add >
            //----------------< mediaplayer_OpenMedia() >----------------
        }

        //public static BitmapImage GetThumbnail(string video, string thumbnail)
        //{
        //    var cmd = "ffmpeg  -itsoffset -1  -i " + '"' + video + '"' + " -vcodec mjpeg -vframes 1 -an -f rawvideo -s 320x240 " + '"' + thumbnail + '"';

        //    var startInfo = new System.Diagnostics.ProcessStartInfo
        //    {
        //        WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden,
        //        FileName = "cmd.exe",
        //        Arguments = "/C " + cmd
        //    };

        //    var process = new System.Diagnostics.Process
        //    {
        //        StartInfo = startInfo
        //    };

        //    process.Start();
        //    process.WaitForExit(5000);

        //    return _LoadImage(thumbnail);
        //}

        //static BitmapImage _LoadImage(string path)
        //{
        //    var ms = new MemoryStream(File.ReadAllBytes(path));

        //    var bitmap = new BitmapImage();
        //    bitmap.BeginInit();
        //    bitmap.StreamSource = ms;
        //    bitmap.CacheOption = BitmapCacheOption.OnLoad;
        //    bitmap.EndInit();

        //    return bitmap;
        //}

        public void DoWork_GetSize(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            for (int i = 0; i < vm.searchFiles_Result.Length; i++)
            {
                if (worker.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }
                string s = vm.searchFiles_Result[i];
                worker.ReportProgress((int)i, GetDataModel(s, false));
            }
        }

        public void DuringWork_GetSize(object sender, ProgressChangedEventArgs e)
        {
            DataModel dm = (DataModel)e.UserState;

            vm.list_FileDataModels.Add((DataModel)e.UserState);
        }

        //此步驟所有資料夾/檔案皆已載入頁面，尚餘檔案的小圖未載入
        public void RunWorkerCompleted_GetSize(object sender, RunWorkerCompletedEventArgs e)
        {
            timerCount = 0;
            vm.timer.Start();

            int itemsCount = vm.list_FileDataModels.Count + vm.list_DirDataModels.Count;
            if (itemsCount == 1)
                vm.msg.txt_msg1 = itemsCount.ToString() + " item";
            else
                vm.msg.txt_msg1 = itemsCount.ToString() + " items";
        }
    }    
}
