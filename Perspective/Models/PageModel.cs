using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Perspective.ViewModels;

namespace Perspective.Models
{
    public class PageModel : NotifyBase
    {
        public string pageName { get; set; }

        private double _pageSize_Width;
        public double pageSize_Width
        {
            get { return _pageSize_Width; }
            set
            {
                _pageSize_Width = value;
                OnPropertyChanged("pageSize_Width");

                if(!double.IsNaN(value))
                    Page_unigrid_column = (int)Math.Truncate(_pageSize_Width / _fileboxSize_Width);
            }
        }

        private int _unigrid_column = 9;
        public int Page_unigrid_column
        {
            get { return _unigrid_column; }
            set
            {
                _unigrid_column = value;
                OnPropertyChanged_Normal("Page_unigrid_column");
            }
        }

        private double _fileboxSize_Width = 140;
        public double fileboxSize_Width
        {
            get { return _fileboxSize_Width; }
            set
            {
                _fileboxSize_Width = value;
                OnPropertyChanged("fileboxSize_Width");

                if (!double.IsNaN(value))
                    Page_unigrid_column = (int)Math.Truncate(_pageSize_Width / value);
            }
        }
    }
}
