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
                    Page_unigrid_column = (int)Math.Truncate(value / 140);
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
    }
}
