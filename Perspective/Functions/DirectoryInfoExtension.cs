using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Perspective.Functions
{
    public static class DirectoryInfoExtension
    {
        public static long GetSize(string path)
        {
            Type tp = Type.GetTypeFromProgID("Scripting.FileSystemObject");
            object fso = Activator.CreateInstance(tp);
            object fd = tp.InvokeMember("GetFolder", BindingFlags.InvokeMethod, null, fso, new object[] { path });
            long ret = Convert.ToInt64(tp.InvokeMember("Size", BindingFlags.GetProperty, null, fd, null));
            Marshal.ReleaseComObject(fso);
            return ret;
        }
    }
}
