using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Imaging;
using System.Text;
using System.Threading.Tasks;
using Perspective.ViewModels;

namespace Perspective.Models
{
    public class DataModel:NotifyBase
    {
        public bool DirOrFile { get; set; }
        public string Name { get; set; }
        public string pathInfo { get; set; }
        public string ExtensionName { get; set; }
        public bool Selected { get; set; }
        public DateTime updateTime { get; set; }
        public DateTime creationTime { get; set; }

        private double _fileboxSize_Width = 140;
        public double fileboxSize_Width
        {
            get { return _fileboxSize_Width; }
            set
            {
                _fileboxSize_Width = value;
                OnPropertyChanged_Normal("fileboxSize_Width");

                fileboxSize_Height = value / 7 * 8;
            }
        }

        private double _fileboxSize_Height = 160;
        public double fileboxSize_Height
        {
            get { return _fileboxSize_Height; }
            set
            {
                _fileboxSize_Height = value;
                OnPropertyChanged_Normal("fileboxSize_Height");
            }
        }

        private bool _Visibility_img = true;
        public bool Visibility_img
        {
            get { return _Visibility_img; }
            set
            {
                _Visibility_img = value;
                OnPropertyChanged_Normal("Visibility_img");
            }
        }

        private bool _Visibility_btn_remove = false;
        public bool Visibility_btn_remove
        {
            get { return _Visibility_btn_remove; }
            set
            {
                _Visibility_btn_remove = value;
                OnPropertyChanged_Normal("Visibility_btn_remove");
            }
        }

        private bool _isChecked = false;
        public bool isChecked
        {
            get { return _isChecked; }
            set
            {
                _isChecked = value;
                OnPropertyChanged_Normal("isChecked");
            }
        }

        private BitmapImage _imgSource;
        public BitmapImage imgSource
        {
            get { return _imgSource; }
            set
            {
                _imgSource = value;
                OnPropertyChanged_Normal("imgSource");
            }
        }

        private string _mediaSource;
        public string mediaSource
        {
            get { return _mediaSource; }
            set
            {
                _mediaSource = value;
                OnPropertyChanged_Normal("mediaSource");
            }
        }

        //private string _imgSource = "";
        //public string imgSource
        //{
        //    get { return _imgSource; }
        //    set
        //    {
        //        _imgSource = value;
        //        OnPropertyChanged_Normal("imgSource");
        //    }
        //}
    }
}
