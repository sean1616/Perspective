using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Globalization;
using System.Windows;

namespace Perspective.ValueConverters
{
    public class FileBoxNameConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string result = "";
            var input = (string)value;
            if (input.Length > 20)
            {
                try
                {
                    string str_extention = Path.GetExtension(input);
                    string str_middle = input.Substring(8, input.Length - str_extention.Length - 7);

                    StringBuilder sb = new StringBuilder(input);
                    sb.Replace(str_middle, "...");
                    result = sb.ToString();
                }
                catch
                {

                }
            }
            else result = input;
            //result = input;

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
