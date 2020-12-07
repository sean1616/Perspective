using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Media;
using System.Windows.Media.Imaging;
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

        public bool IsImageJudge(string path)
        {
            bool _isImg = false;
            string[] list_extension = new string[] { ".png", ".jpg", ".bmp", "jpeg" };
            string fileExtension = Path.GetExtension(path);
            foreach(string s in list_extension)
            {
                if (string.Compare(fileExtension, s) == 0)
                {
                    _isImg = true;
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
                    //img_strSource = "../Resources/Text.png";
                    break;
                case ".xlsx":
                    img_strSource = currentPath + @"\ImgSource\excel.png";
                    //img_strSource = "../Resources/excel.png";
                    break;
                case ".csv":
                    img_strSource = currentPath + @"\ImgSource\excel.png";
                    //img_strSource = "../Resources/excel.png";
                    break;
                //case ".png":
                //    img_strSource = path;
                //    break;
                //case ".jpg":
                //    img_strSource = path;
                //    break;
                //case ".bmp":
                //    img_strSource = path;
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

            if (IsImageJudge(path))
            {
                using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    bitmap.BeginInit();
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.StreamSource = stream;
                    bitmap.EndInit();
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
                if (Directory.Exists(@s))
                {
                    // This path is a directory
                    vm.list_directories.Add(s);
                    vm.list_dirNames.Add(Path.GetFileName(s));
                    vm.list_DirDataModels.Add(new DataModel() { Names = Path.GetFileName(s), Visibility_btn_remove = false, pathInfo = s });
                }
            }
            #endregion

            #region 搜尋本資料夾內的所有檔案
            string[] files = Directory.GetFiles(targetDirectory);
            DataModel[] dataModels = new DataModel[files.Length];

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
                DataModel dm = new DataModel() { Names = Path.GetFileName(s), Visibility_btn_remove = false, pathInfo = s };
                worker.ReportProgress((int)i, dm);
            }
        }

        public void DuringWork(object sender, ProgressChangedEventArgs e)
        {
            DataModel dm = (DataModel)e.UserState;

            vm.list_FileDataModels.Add((DataModel)e.UserState);
        }

        public void RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            timerCount = 0;
            vm.timer.Start();

            int itemsCount = vm.list_FileDataModels.Count + vm.list_DirDataModels.Count;
            if (itemsCount == 1)
                vm.txt_msg = itemsCount.ToString() + " item";
            else
                vm.txt_msg = itemsCount.ToString() + " items";
        }

        int timerCount = 0;
        public void _timer_Tick(object sender, EventArgs e)
        {
            if (timerCount >= vm.list_FileDataModels.Count)
            {
                vm.timer.Stop();

                //Save all files/dirs to temp list
                List<DataModel> ld = vm.list_DirDataModels.ToList();
                vm.temp_list_DirDataModels = new System.Collections.ObjectModel.ObservableCollection<DataModel>(ld);

                ld = vm.list_FileDataModels.ToList();
                vm.temp_list_FileDataModels = new System.Collections.ObjectModel.ObservableCollection<DataModel>(ld);

               
                return;
            }
            string path = vm.list_FileDataModels[timerCount].pathInfo;
            vm.list_FileDataModels[timerCount].imgSource = LoadImage(path);

            timerCount++;
        }

        public void Refresh_Taged_File(string tagsDirectoryPath)
        {
            var list_all_files_in_tags = new List<List<string>>();
            List<List<string>> list_all_Dirs_in_tags = new List<List<string>>();

            vm.list_DirDataModels.Clear();
            vm.list_FileDataModels.Clear();

            //將所有已選擇的標籤對應的檔案存成List
            foreach (string t in vm.list_selectedTags)
            {
                string tagTxtPath = tagsDirectoryPath + @"\" + t + @".txt";   //Txt path of this tag

                //if (vm.dictonary_tag_files.ContainsKey(tag))

                //呼叫具此標籤的檔案們              
                string[] lines;
                if (File.Exists(tagTxtPath))
                {
                    lines = File.ReadAllLines(tagTxtPath);
                }
                else continue;

                List<string> list_f = new List<string>();
                List<string> list_d = new List<string>();

                foreach (string s in lines)
                {
                    if (File.Exists(@s)) // This path is a file
                    {
                        list_f.Add(s);
                    }
                    else if (Directory.Exists(@s)) // This path is a directory
                    {
                        list_d.Add(s);
                    }
                }

                list_all_files_in_tags.Add(list_f);
                list_all_Dirs_in_tags.Add(list_d);
            }


            if (vm.list_selectedTags.Count != 0)
            {
                var list_F_intersection = GetFilesIntersection(list_all_files_in_tags[0]);
                for (int i = 1; i < list_all_files_in_tags.Count; i++)
                {
                    list_F_intersection = list_F_intersection.Intersect(list_all_files_in_tags[i]);
                }

                foreach (string s in list_F_intersection)
                {
                    DataModel dataModel = new DataModel() { Names = Path.GetFileName(s), pathInfo = s, imgSource = LoadImage(s) };

                    vm.list_FileDataModels.Add(dataModel);
                }

                var list_D_intersection = GetFilesIntersection(list_all_Dirs_in_tags[0]);
                for (int i = 1; i < list_all_Dirs_in_tags.Count; i++)
                {
                    list_D_intersection = list_D_intersection.Intersect(list_all_Dirs_in_tags[i]);
                }

                foreach (string s in list_D_intersection)
                {
                    vm.list_DirDataModels.Add(new DataModel() { Names = Path.GetFileName(s), pathInfo = s });
                }
            }
            else
            {
                SearchDirectory(vm.path);
            }
        }

        private IEnumerable<string> GetFilesIntersection(List<string> list)
        {
            List<string> listF = list;
            return listF;
        }
    }
}
