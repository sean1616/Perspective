using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Perspective.ViewModels;

namespace Perspective.ViewModels
{
    public class VM : NotifyBase
    {
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

        private ObservableCollection<string> _list_folderFileNames = new ObservableCollection<string>();
        public ObservableCollection<string> list_folderFileNames
        {
            get { return _list_folderFileNames; }
            set
            {
                _list_folderFileNames = value;
                OnPropertyChanged_Normal("list_folderFileNames");
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

        private ObservableCollection<string> _list_folderNames = new ObservableCollection<string>();
        public ObservableCollection<string> list_folderNames
        {
            get { return _list_folderNames; }
            set
            {
                _list_folderNames = value;
                OnPropertyChanged_Normal("list_folderNames");
            }
        }

        private ObservableCollection<string> _list_selected_files = new ObservableCollection<string>();
        public ObservableCollection<string> list_selected_files
        {
            get { return _list_selected_files; }
            set
            {
                _list_selected_files = value;
                OnPropertyChanged_Normal("list_selected_files");
            }
        }

        private ObservableCollection<string> _list_tags = new ObservableCollection<string>();
        public ObservableCollection<string> list_tags
        {
            get { return _list_tags; }
            set
            {
                _list_tags = value;
                OnPropertyChanged_Normal("list_tags");
            }
        }

        public Dictionary<string, ObservableCollection<string>> dictonary_tag_files { get; set; } = new Dictionary<string, ObservableCollection<string>>();

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

        public bool _isTagRemoveMode { get; set; } = false;
    }
}
