using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Perspective.ViewModels;

namespace Perspective.Models
{
    public class DirectFolderModel : NotifyBase
    {
        public string Name { get; set; }
        public string pathInfo { get; set; }
        public string user { get; set; }
        public bool visible { get; set; }
    }
}
