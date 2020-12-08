using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Perspective.ViewModels;

namespace Perspective.Models
{
    public class MsgModel : NotifyBase
    {
        private string _txt_msg1 = "Message";
        public string txt_msg1
        {
            get { return _txt_msg1; }
            set
            {
                _txt_msg1 = value;
                OnPropertyChanged_Normal("txt_msg1");
            }
        }

        private string _txt_msg2 = " ";
        public string txt_msg2
        {
            get { return _txt_msg2; }
            set
            {
                _txt_msg2 = value;
                OnPropertyChanged_Normal("txt_msg2");
            }
        }

        private string _txt_msg3 = " ";
        public string txt_msg3
        {
            get { return _txt_msg3; }
            set
            {
                _txt_msg3 = value;
                OnPropertyChanged_Normal("txt_msg3");
            }
        }

        private string _txt_msg4 = " ";
        public string txt_msg4
        {
            get { return _txt_msg4; }
            set
            {
                _txt_msg4 = value;
                OnPropertyChanged_Normal("txt_msg4");
            }
        }

        private string _txt_msg5 = " ";
        public string txt_msg5
        {
            get { return _txt_msg5; }
            set
            {
                _txt_msg5 = value;
                OnPropertyChanged_Normal("txt_msg5");
            }
        }
    }
}
