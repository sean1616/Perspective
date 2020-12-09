﻿using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using System.Windows.Media.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Perspective.ViewModels;
using Perspective.Models;

namespace Perspective.ViewModels
{
    public class VM : NotifyBase
    {
        public VM()
        {
           
        }

        public BackgroundWorker worker = new BackgroundWorker();
        public DispatcherTimer timer = new DispatcherTimer();

        public string tagsDirectoryPath { get; set; } = "";
        public string IntagsDirectoryPath { get; set; } = "";
        public int ExpanderItemsHeigh { get; set; } = 25;

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

        public string[] searchFiles_Result { get; set; } = new string[] { };

        public ObservableCollection<DataModel> temp_list_DirDataModels { get; set; } = new ObservableCollection<DataModel>();
        public ObservableCollection<DataModel> temp_list_FileDataModels { get; set; } = new ObservableCollection<DataModel>();

        public ObservableCollection<string> path_previous { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<string> path_after { get; set; } = new ObservableCollection<string>();

        //public string path_origin_clipboard { get; set; } = "";
        public List<DataModel> path_clipboard { get; set; } = new List<DataModel>();
        public ObservableCollection<string> path_Files_clipboard { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<string> path_Dirs_clipboard { get; set; } = new ObservableCollection<string>();

        public bool _isLMouseDown_in_MainPage { get; set; } = false;

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

        //private ObservableCollection<DataModel> _list_DataModels = new ObservableCollection<DataModel>() { new DataModel() { Names = "No.1" } };
        //public ObservableCollection<DataModel> list_DataModels
        //{
        //    get { return _list_DataModels; }
        //    set
        //    {
        //        _list_DataModels = value;
        //        OnPropertyChanged_Normal("list_DataModels");
        //    }
        //}

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
                _path = value;
                OnPropertyChanged_Normal("path");
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
    }
}
