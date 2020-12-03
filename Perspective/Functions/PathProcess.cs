using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Threading.Tasks;
using Perspective.ViewModels;
using Perspective.Models;

namespace Perspective.Functions
{
    public class PathProcess
    {
        VM vm;
        string currentPath = Directory.GetCurrentDirectory();

        public PathProcess()
        {

        }

        public PathProcess(VM vm)
        {
            this.vm = vm;
        }

        public bool IsImageJudge(string path)
        {
            bool _isImg = false;
            string[] list_extension = new string[] { ".png", ".jpg", ".bmp", "jpeg" };
            string fileExtension = Path.GetExtension(path);
            foreach(string s in list_extension)
            {
                if (string.Compare(fileExtension, s) == 0)
                {
                    _isImg = true;
                    break;
                }
            }
            return _isImg;
        }

        public string FileBox_NameExtensionJudge(string path)
        {
            string img_strSource = "";           

            string fileExtention = Path.GetExtension(path);
            switch (fileExtention)
            {
                case ".txt":
                    img_strSource = currentPath + @"\ImgSource\Text.png";
                    //img_strSource = "../Resources/Text.png";
                    break;
                case ".xlsx":
                    img_strSource = currentPath + @"\ImgSource\excel.png";
                    //img_strSource = "../Resources/excel.png";
                    break;
                case ".csv":
                    img_strSource = currentPath + @"\ImgSource\excel.png";
                    //img_strSource = "../Resources/excel.png";
                    break;
                case ".png":
                    img_strSource = path;
                    break;
                case ".jpg":
                    img_strSource = path;
                    break;
                case ".bmp":
                    img_strSource = path;
                    break;
            }

            return img_strSource;
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

        // load an image into memory and use it as an image source, and the image could be delete when it was used by process
        public ImageSource BitmapFromUri(string path)
        {            
            var bitmap = new BitmapImage();            
            bitmap.BeginInit();

            Uri source = new Uri(path);
            bitmap.UriSource = source;
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.DecodePixelHeight = 100;
            bitmap.EndInit();   //when image is not exist, error will happen here.
            //bitmap.Freeze();
           
            return bitmap;
        }

        public ImageSource BitmapFromUri(Uri source)
        {
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
                                    
            bitmap.UriSource = source;
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.DecodePixelHeight = 100;
            bitmap.EndInit();   //when image is not exist, error will happen here.
                                //bitmap.Freeze();

            return bitmap;
        }

        public ImageSource LoadImage(string path)
        {
            var bitmap = new BitmapImage();

            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.StreamSource = stream;
                bitmap.EndInit();
                bitmap.Freeze();
            }

            return bitmap;
        }
    }
}
