using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Perspective.Functions
{
    public static class MathCalculation
    {
        public static string Calculate_FileSize(double size)
        {
            string txt_size = "";

            string sizeUnit = "Byte";
            
            if ((size / 10000) < 1000 && (size / 10000) > 1)
            {
                size = Math.Round(size / 1000, 1);
                sizeUnit = "KB";
            }
            else if ((size / 10000000) < 1000 && (size / 10000000) > 1)
            {
                size = Math.Round(size / 1000000, 1);
                sizeUnit = " MB";
            }
            else if ((size / 1000000000) < 1000 && (size / 1000000000) > 1)
            {
                size = Math.Round(size / 1000000000, 1);
                sizeUnit = " GB";
            }
            txt_size = string.Concat(size.ToString()," ", sizeUnit);

            return txt_size;
        }
    }
}
