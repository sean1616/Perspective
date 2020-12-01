using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Perspective.ViewModels;
using Perspective.Models;

namespace Perspective.Functions
{
    public class PathProcess
    {
        VM vm;

        public PathProcess()
        {

        }

        public PathProcess(VM vm)
        {
            this.vm = vm;
        }

        public string FileBox_NameExtensionJudge(string path)
        {
            string img_source = "";

            string fileExtention = Path.GetExtension(path);
            switch (fileExtention)
            {
                case ".txt":
                    img_source = "../Resources/Text.png";
                    break;
                case ".xlsx":
                    img_source = "../Resources/excel.png";
                    break;
                case ".csv":
                    img_source = "../Resources/excel.png";
                    break;
                case ".png":
                    img_source = path;
                    break;
                case ".jpg":
                    img_source = path;
                    break;
                case ".bmp":
                    img_source = path;
                    break;
            }

            return img_source;
        }

        public void SearchTag(string tag)
        {
            if (string.IsNullOrEmpty(tag)) return;

            var sTag = vm.list_TagModels.Where(x => x.tagName == tag).ToList();

            vm.list_TagModels.Clear();
            foreach (TagModel t in sTag)
            {
                vm.list_TagModels.Add(t);
            }
        }
    }
}
