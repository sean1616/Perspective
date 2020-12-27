using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.ComponentModel;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using DexterLib;
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

        public int IsImageVideoJudge(string path)
        {
            int _isImg = 0;

            string fileExtension = Path.GetExtension(path);

            string[] list_img_extension = new string[] { ".png", ".jpg", ".bmp", "jpeg" };            
            foreach(string s in list_img_extension)
            {
                if (string.Compare(fileExtension, s) == 0)
                {
                    _isImg = 1;
                    return _isImg;
                }
            }

            string[] list_video_extension = new string[] { ".mp4", ".mkv", ".avi" };
            foreach (string s in list_video_extension)
            {
                if (string.Compare(fileExtension, s) == 0)
                {
                    _isImg = 2;
                    return _isImg;
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
                vm.list_directories.Clear();
                vm.temp_list_FileDataModels.Clear();

                string[] files = Directory.GetFiles(vm.ThumbnailPath);
                if (files.Length > 0)
                {
                    foreach (string s in files)
                        File.Delete(s);
                }

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

            int code = IsImageVideoJudge(path);   //1 is image, 2 is video
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
                string newImgPath = string.Concat(vm.ThumbnailPath, @"\", Path.GetFileNameWithoutExtension(path), @".jpg");
               
                int getFrame_TimePosition = 3;  //Second(S)
                GetThumb(path, newImgPath, getFrame_TimePosition);  //將影片中的特定幀存成圖片

                using (var stream = new FileStream(newImgPath, FileMode.Open, FileAccess.Read, FileShare.Read))
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
                // This path is a directory
                if (Directory.Exists(@s))
                {
                    FileInfo fi = new FileInfo(s);
                    var filtered = !fi.Attributes.HasFlag(FileAttributes.Hidden);  //Check file is hidden or not

                    if (vm.settingModel.isInVisFlagShow) filtered = true;

                    if (filtered)
                    {                        
                        DirectoryInfo di = new DirectoryInfo(s);
                        vm.list_DirDataModels.Add(new DataModel()
                        {
                            Name = Path.GetFileName(s),
                            Visibility_btn_remove = false,
                            pathInfo = s,
                            DirOrFile = false,
                            updateTime = di.LastWriteTime,
                            creationTime = di.CreationTime
                        });
                    }                      
                }
            }
            #endregion

            #region 搜尋本資料夾內的所有檔案
            string[] files = Directory.GetFiles(targetDirectory);

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

        public static string GetThumb(string videoPath, string newImgPath, int specificFrame)
        {
            string thumb = "";

            try
            {
                //每幾幀取一次圖
                //string s = "ffmpeg -i D:\\Download\\1234.mp4 -vf select='not(mod(n,2000))' -vsync 0D:\\Download\\\\image%d.jpg";

                //在特定秒數取圖
                //string s = "ffmpeg -ss 5 -i D:\\Download\\1234.mp4 -s 360x200 -f image2 -vframes 1 -y D:\\Download\\1234.jpg";
                var processInfo = new ProcessStartInfo();
                processInfo.FileName = VM.FFmpegPath;
                //processInfo.Arguments = string.Format("-ss {0} -i {1} -s 480x270 -f image2 -vframes 1 -y {2}", specificFrame, "\"" + videoPath + "\"", "\"" + newImgPath + "\"");
                processInfo.Arguments = string.Format("-ss {0} -i {1} -f image2 -vframes 1 -y {2}", specificFrame, "\"" + videoPath + "\"", "\"" + newImgPath + "\"");
                processInfo.CreateNoWindow = true;
                processInfo.UseShellExecute = false;
                using (var process = new Process())
                {
                    process.StartInfo = processInfo;
                    process.Start();
                    process.WaitForExit();
                    thumb = newImgPath;                    
                }
            }
            catch (Exception ex)
            {
                string error = ex.Message;
            }

            return thumb;
        }

        #region FFmpeg參數說明
//        ffmpeg.exe -i F:/娱乐/动力之歌.mp3 -ab 56 -ar 22050 -b 500 -r 15 -s 320x240 f:/11.flv
//ffmpeg -i F:/01.wmv -ab 56 -ar 22050 -b 500 -r 15 -s 320x240 f:/test.flv
//使用-ss参数 作用（time_off set the start time offset），可以从指定时间点开始转换任务。如:
//转换文件格式的同时抓缩微图：
//ffmpeg -i "test.avi" -y -f image2 -ss 8 -t 0.001 -s 350x240 'test.jpg'
//对已有flv抓图：
//ffmpeg -i "test.flv" -y -f image2 -ss 8 -t 0.001 -s 350x240 'test.jpg'
//-ss后跟的时间单位为秒
//Ffmpeg转换命令
//ffmpeg -y -i test.mpeg -bitexact -vcodec h263 -b 128 -r 15 -s 176x144 -acodec aac -ac 2 -ar 22500
//-ab 24 -f 3gp test.3gp
//或者
//ffmpeg -y -i test.mpeg -ac 1 -acodec amr_nb -ar 8000 -s 176x144 -b 128 -r 15 test.3gp
//ffmpeg参数设定解说
//-bitexact 使用标准比特率
//-vcodec xvid 使用xvid压缩
//-s 320x240 指定分辨率
//-r 29.97 桢速率（可以改，确认非标准桢率会导致音画不同步，所以只能设定为15或者29.97）
//画面部分，选其一
//-b<比特率> 指定压缩比特率，似乎ffmpeg是自动VBR的，指定了就大概是平均比特率，比如768，1500这样的
//就是原来默认项目中有的
//-qscale<数值> 以<数值>质量为基础的VBR，取值0.01-255，约小质量越好
//-qmin<数值> 设定最小质量，与-qmax（设定最大质量）共用，比如-qmin 10 -qmax 31
//-sameq 使用和源同样的质量
//声音部分
//-acodec aac 设定声音编码
//-ac<数值> 设定声道数，1就是单声道，2就是立体声，转换单声道的TVrip可以用1（节省一半容量），高品质
//的DVDrip就可以用2
//-ar<采样率> 设定声音采样率，PSP只认24000
//-ab<比特率> 设定声音比特率，前面-ac设为立体声时要以一半比特率来设置，比如192kbps的就设成96，转换
//君默认比特率都较小，要听到较高品质声音的话建议设到160kbps（80）以上
//-vol<百分比> 设定音量，某些DVDrip的AC3轨音量极小，转换时可以用这个提高音量，比如200就是原来的2倍
//这样，要得到一个高画质音质低容量的MP4的话，首先画面最好不要用固定比特率，而用VBR参数让程序自己去
//判断，而音质参数可以在原来的基础上提升一点，听起来要舒服很多，也不会太大（看情况调整
//例子：ffmpeg -y -i "1.avi" -title "Test" -vcodec xvid -s 368x208 -r 29.97 -b 1500 -acodec aac -ac 2 -ar 24000 -ab 128 -vol 200 -f psp -muxvb 768 "1.mp4"

//解释：以上命令可以在Dos命令行中输入，也可以创建到批处理文件中运行。不过，前提是：要在ffmpeg所在的目录中执行（转换君所在目录下面的cores子目录）。
//参数：
//-y（覆盖输出文件，即如果1.mp4文件已经存在的话，不经提示就覆盖掉了）
//-i "1.avi"（输入文件是和ffmpeg在同一目录下的1.avi文件，可以自己加路径，改名字）
//-title "Test"（在PSP中显示的影片的标题）
//-vcodec xvid（使用XVID编码压缩视频，不能改的）
//-s 368x208（输出的分辨率为368x208，注意片源一定要是16:9的不然会变形）
//-r 29.97（帧数，一般就用这个吧）
//-b 1500（视频数据流量，用-b xxxx的指令则使用固定码率，数字随便改，1500以上没效果；还可以用动态码率如：-qscale 4和-qscale 6，4的质量比6高）
//-acodec aac（音频编码用AAC）
//-ac 2（声道数1或2）
//-ar 24000（声音的采样频率，好像PSP只能支持24000Hz）
//-ab 128（音频数据流量，一般选择32、64、96、128）
//-vol 200（200%的音量，自己改）
//-f psp（输出psp专用格式）
//-muxvb 768（好像是给PSP机器识别的码率，一般选择384、512和768，我改成1500，PSP就说文件损坏了）
//"1.mp4"（输出文件名，也可以加路径改文件名）

        #endregion

        public string GetVideoFrames(string videoPath)
        {
            string output = "";

            try
            {
                string s= "ffmpeg -discard nokey -i D:\\Download\\1234.mp4 -map 0:v:0 -c copy -f null -";
                var processInfo = new ProcessStartInfo();
                processInfo.FileName = VM.FFmpegPath;
                processInfo.Arguments = "ffmpeg -discard nokey -i " + videoPath + " -map 0:v:0 -c copy -f null -";
                //processInfo.Arguments = "ffprobe -v error -select_streams v:0 -show_entries stream=nb_frames -of default=nokey=1:noprint_wrappers=1 " + videoPath;
                processInfo.CreateNoWindow = true;
                processInfo.UseShellExecute = false;

                Process process = new Process();
                process.StartInfo = processInfo;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.CreateNoWindow = true;
                //process.ErrorDataReceived += new DataReceivedEventHandler(Output);
                process.Start();
                //* Read the output (or the error)
                output = process.StandardOutput.ReadToEnd();
                Console.WriteLine(output);
                string err = process.StandardError.ReadToEnd();
                Console.WriteLine(err);
                process.WaitForExit();
                
            }
            catch (Exception ex)
            {
                string error = ex.Message;
            }

            return output;
        }

        private void Output(object sendProcess, DataReceivedEventArgs output)
        {
            if (!String.IsNullOrEmpty(output.Data))
            {
                vm.msg.txt_msg1 = output.Data.ToString();
            }
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
            //drawingContext.DrawVideo(mediaPlayer, new System.Windows.Rect(0, 0, 160, 100));
            drawingContext.Close();

            double dpiX = 1 / 200;
            double dpiY = 1 / 200;
            RenderTargetBitmap bmp = new RenderTargetBitmap(160, 100, dpiX, dpiY, PixelFormats.Pbgra32);
            //RenderTargetBitmap bmp=new RenderTargetBitmap(drawingContext.wi)
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
