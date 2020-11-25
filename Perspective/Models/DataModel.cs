using System;
using System.Collections.Generic;
using System.Linq;
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
                OnPropertyChanged("Visibility_btn_remove");
            }
        }


    }
}
