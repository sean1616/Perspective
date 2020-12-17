using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Perspective.ViewModels;
using Perspective.Models;
using Perspective.Functions;

namespace Perspective.ViewModels
{
    public class VM : NotifyBase
    {
        public static string currentPath = Directory.GetCurrentDirectory();
        //public static string FFmpegPath { get; set; } = @"D:\Download\ffmpeg-20200831-4a11a6f-win64-static\ffmpeg-20200831-4a11a6f-win64-static\bin\ffmpeg.exe";
        public static string FFmpegPath { get; set; } = currentPath + @"\ffmpeg.exe";
        public static string FFprobePath { get; set; } = @"D:\Download\ffmpeg-20200831-4a11a6f-win64-static\ffmpeg-20200831-4a11a6f-win64-static\bin\ffprobe.exe";

        public VM()
        {
           
        }

        public string videoPath { get; set; }
        public string newVideoPath { get; set; }

        public string ThumbnailPath { get; set; } = string.Concat(currentPath, @"\Thumbnail");

        private string _ini_path = @"D:\MobiusLink\Instrument.ini";
        public string ini_path
        {
            get { return _ini_path; }
            set
            {
                _ini_path = value;
                OnPropertyChanged_Normal("ini_path");
            }
        }
        public static SetupIniIP ini = new SetupIniIP();

        private bool _Visibility_txt_path = true;
        public bool Visibility_txt_path
        {
            get { return _Visibility_txt_path; }
            set
            {
                _Visibility_txt_path = value;
                OnPropertyChanged_Normal("Visibility_txt_path");
            }
        }

        public BackgroundWorker worker = new BackgroundWorker();
        //public BackgroundWorker worker_getFileSize = new BackgroundWorker();
        public DispatcherTimer timer = new DispatcherTimer();

        public string tagsDirectoryPath { get; set; } = "";
        public string IntagsDirectoryPath { get; set; } = "";
        public string DirectFolders_DirectoryPath { get; set; } = "";

        public int ExpanderItemsHeigh { get; set; } = 30;

        private string _txt_localPath = "";
        public string txt_localPath
        {
            get { return _txt_localPath; }
            set
            {
                _txt_localPath = value;
                OnPropertyChanged_Normal("txt_localPath");
            }
        }

        private string _txt_for_searchFiles = "";
        public string txt_for_searchFiles
        {
            get { return _txt_for_searchFiles; }
            set
            {
                _txt_for_searchFiles = value;
                OnPropertyChanged_Normal("txt_for_searchFiles");
            }
        }

        public string Ini_Read(string Section, string key)
        {
            string _ini_read;
            if (File.Exists(ini_path))
            {
                _ini_read = ini.IniReadValue(Section, key, ini_path);
            }
            else
                _ini_read = "";

            return _ini_read;
        }

        public void Ini_Write(string Section, string key, string value)
        {
            if (!File.Exists(ini_path))
                Directory.CreateDirectory(System.IO.Directory.GetParent(ini_path).ToString());  //建立資料夾
            ini.IniWriteValue(Section, key, value, ini_path);  //創建ini file並寫入基本設定
        }

        public string[] searchFiles_Result { get; set; } = new string[] { };

        public ObservableCollection<DataModel> temp_list_DirDataModels { get; set; } = new ObservableCollection<DataModel>();
        public ObservableCollection<DataModel> temp_list_FileDataModels { get; set; } = new ObservableCollection<DataModel>();

        public ObservableCollection<string> path_previous { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<string> path_after { get; set; } = new ObservableCollection<string>();

        //public string path_origin_clipboard { get; set; } = "";
        public List<DataModel> path_clipboard { get; set; } = new List<DataModel>();
        public ObservableCollection<string> path_Files_clipboard { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<string> path_Dirs_clipboard { get; set; } = new ObservableCollection<string>();

        private double _colum_collectionPath_width = 145;
        public double colum_collectionPath_width
        {
            get { return _colum_collectionPath_width; }
            set
            {
                _colum_collectionPath_width = value;
                OnPropertyChanged_Normal("colum_collectionPath_width");
            }
        }
        //public bool _isLMouseDown_in_MainPage { get; set; } = false;

        private MsgModel _msg = new MsgModel();
        public MsgModel msg
        {
            get { return _msg; }
            set
            {
                _msg = value;
                OnPropertyChanged_Normal("msg");
            }
        }

        public bool _isInTagMode { get; set; } = false;
        public bool _isTagEditMode { get; set; } = false;

        private ObservableCollection<DirectFolderModel> _list_DirectFolderModels = new ObservableCollection<DirectFolderModel>();
        public ObservableCollection<DirectFolderModel> list_DirectFolderModels
        {
            get { return _list_DirectFolderModels; }
            set
            {
                _list_DirectFolderModels = value;
                OnPropertyChanged_Normal("list_DirectFolderModels");
            }
        }

        private ObservableCollection<TagModel> _list_TagModels = new ObservableCollection<TagModel>();
        public ObservableCollection<TagModel> list_TagModels
        {
            get { return _list_TagModels; }
            set
            {
                _list_TagModels = value;
                OnPropertyChanged_Normal("list_TagModels");
            }
        }

        private ObservableCollection<PathBoxModel> _list_PathBoxModels = new ObservableCollection<PathBoxModel>();
        public ObservableCollection<PathBoxModel> list_PathBoxModels
        {
            get { return _list_PathBoxModels; }
            set
            {
                _list_PathBoxModels = value;
                OnPropertyChanged_Normal("list_PathBoxModels");
            }
        }

        private ObservableCollection<DataModel> _list_DirDataModels = new ObservableCollection<DataModel>() { new DataModel() { Name = "No.1" } };
        public ObservableCollection<DataModel> list_DirDataModels
        {
            get { return _list_DirDataModels; }
            set
            {
                _list_DirDataModels = value;
                OnPropertyChanged_Normal("list_DirDataModels");
            }
        }

        //private List<DataModel> _list_FileDataModels = new List<DataModel>() { new DataModel() { Names = "No.2" } };
        //public List<DataModel> list_FileDataModels
        //{
        //    get { return _list_FileDataModels; }
        //    set
        //    {
        //        _list_FileDataModels = value;
        //        OnPropertyChanged_Normal("list_FileDataModels");
        //    }
        //}

        private ObservableCollection<DataModel> _list_FileDataModels = new ObservableCollection<DataModel>() { new DataModel() { Name = "No.2" } };
        public ObservableCollection<DataModel> list_FileDataModels
        {
            get { return _list_FileDataModels; }
            set
            {
                _list_FileDataModels = value;
                OnPropertyChanged_Normal("list_FileDataModels");
            }
        }

        private ObservableCollection<string> _list_files = new ObservableCollection<string>();
        public ObservableCollection<string> list_files
        {
            get { return _list_files; }
            set
            {
                _list_files = value;
                OnPropertyChanged_Normal("list_files");
            }
        }       

        private ObservableCollection<string> _list_fileNames = new ObservableCollection<string>();
        public ObservableCollection<string> list_fileNames
        {
            get { return _list_fileNames; }
            set
            {
                _list_fileNames = value;
                OnPropertyChanged_Normal("list_fileNames");
            }
        }

        private ObservableCollection<string> _list_directories = new ObservableCollection<string>();
        public ObservableCollection<string> list_directories
        {
            get { return _list_directories; }
            set
            {
                _list_directories = value;
                OnPropertyChanged_Normal("list_directories");
            }
        }

        private ObservableCollection<string> _list_dirNames = new ObservableCollection<string>();
        public ObservableCollection<string> list_dirNames
        {
            get { return _list_dirNames; }
            set
            {
                _list_dirNames = value;
                OnPropertyChanged_Normal("list_dirNames");
            }
        }

        public List<DataModel> list_selected_items { get; set; } = new List<DataModel>();

        //private ObservableCollection<string> _list_selected_files = new ObservableCollection<string>();
        //public ObservableCollection<string> list_selected_files
        //{
        //    get { return _list_selected_files; }
        //    set
        //    {
        //        _list_selected_files = value;
        //        OnPropertyChanged_Normal("list_selected_files");                
        //    }
        //}

        //private ObservableCollection<string> _list_selected_dirs = new ObservableCollection<string>();
        //public ObservableCollection<string> list_selected_dirs
        //{
        //    get { return _list_selected_dirs; }
        //    set
        //    {
        //        _list_selected_dirs = value;
        //        OnPropertyChanged_Normal("list_selected_dirs");
        //    }
        //}

        //private ObservableCollection<string> _list_tags = new ObservableCollection<string>();
        //public ObservableCollection<string> list_tags
        //{
        //    get { return _list_tags; }
        //    set
        //    {
        //        _list_tags = value;
        //        OnPropertyChanged_Normal("list_tags");
        //    }
        //}


        //public ObservableCollection<string> list_selectedTags { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<TagModel> list_selectedTagModels { get; set; } = new ObservableCollection<TagModel>();


        public Dictionary<string, List<string>> dictonary_tag_files { get; set; } = new Dictionary<string, List<string>>();

        private string _path = @"D:\Download";
        public string path
        {
            get { return _path; }
            set
            {
                list_PathBoxModels.Clear();
                _path = value;
                OnPropertyChanged_Normal("path");

                if (!string.IsNullOrEmpty(_path))
                {
                    string[] list_splitPath = _path.Split('\\');
                    string pathBox_pathInfo = list_splitPath[0];

                    for (int i = 0; i < list_splitPath.Length; i++)
                    {
                        if (i > 0)
                        {
                            pathBox_pathInfo = string.Concat(pathBox_pathInfo, @"\", list_splitPath[i]);
                        }
                        else pathBox_pathInfo = string.Concat(pathBox_pathInfo, @"\");

                        if (!string.IsNullOrWhiteSpace(list_splitPath[i]))
                            list_PathBoxModels.Add(new PathBoxModel() { Name = list_splitPath[i], pathInfo = pathBox_pathInfo });
                    }                                      
                }              
            }
        }

        private int _unigrid_column = 9;
        public int unigrid_column
        {
            get { return _unigrid_column; }
            set
            {
                _unigrid_column = value;
                OnPropertyChanged_Normal("unigrid_column");
            }
        }

        public bool _isTagRemoveMode { get; set; } = false;

        public DataModel dm { get; set; } = new DataModel();
        public int index_dragItem { get; set; } = 0;
        public int index_dropItem { get; set; } = 0;

    }
}
