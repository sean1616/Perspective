using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Perspective.Functions
{
    public class PathProcess
    {
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
    }
}
