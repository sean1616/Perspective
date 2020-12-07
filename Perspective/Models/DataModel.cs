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
        public string Names { get; set; }
        public string pathInfo { get; set; }
        public bool Selected { get; set; }

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
