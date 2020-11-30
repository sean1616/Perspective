using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Perspective.ViewModels;

namespace Perspective.Models
{
    public class TagModel:NotifyBase
    {
        private bool _isChecked = false;
        public bool isChecked
        {
            get { return _isChecked; }
            set
            {
                _isChecked = value;
                OnPropertyChanged("isChecked");
            }
        }

        private string _tagName = "";
        public string tagName
        {
            get { return _tagName; }
            set
            {
                _tagName = value;
                OnPropertyChanged_Normal("tagName");
            }
        }
    }
}
