using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace Perspective.Functions
{
    public static class SpecialFolders
    {
        public static string GetSpecialFolderPath(string target)
        {
            string path = "";
            RegistryKey folders;
            folders = OpenRegistryPath(Registry.CurrentUser, @"\software\microsoft\windows\currentversion\explorer\shell folders");

            switch (target.ToUpper())
            {
                case "DESKTOP":
                    // Windows使用者桌面路徑
                    path = folders.GetValue("Desktop").ToString();
                    break;
                default:
                    // Windows使用者字型目錄路徑
                    path = folders.GetValue("Fonts").ToString();
                    // Windows使用者網路鄰居路徑
                    path = folders.GetValue("Nethood").ToString();
                    // Windows使用者我的文件路徑
                    path = folders.GetValue("Personal").ToString();
                    // Windows使用者開始選單程式路徑
                    path = folders.GetValue("Programs").ToString();
                    // Windows使用者存放使用者最近訪問文件快捷方式的目錄路徑
                    path = folders.GetValue("Recent").ToString();
                    // Windows使用者傳送到目錄路徑
                    path = folders.GetValue("Sendto").ToString();
                    // Windows使用者開始選單目錄路徑
                    path = folders.GetValue("Startmenu").ToString();
                    // Windows使用者開始選單啟動項目錄路徑
                    path = folders.GetValue("Startup").ToString();
                    // Windows使用者收藏夾目錄路徑
                    path = folders.GetValue("Favorites").ToString();
                    // Windows使用者網頁歷史目錄路徑
                    path = folders.GetValue("History").ToString();
                    // Windows使用者Cookies目錄路徑
                    path = folders.GetValue("Cookies").ToString();
                    // Windows使用者Cache目錄路徑
                    path = folders.GetValue("Cache").ToString();
                    // Windows使用者應用程式資料目錄路徑
                    path = folders.GetValue("Appdata").ToString();
                    // Windows使用者列印目錄路徑
                    path = folders.GetValue("Printhood").ToString();
                    break;
            }

            return path;
        }

        private static RegistryKey OpenRegistryPath(RegistryKey root, string s)
        {
            s = s.Remove(0, 1) + @"\";
            while (s.IndexOf(@"\") != -1)
            {
                root = root.OpenSubKey(s.Substring(0, s.IndexOf(@"\")));
                s = s.Remove(0, s.IndexOf(@"\") + 1);
            }
            return root;
        }
    }
}
