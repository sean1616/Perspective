using System;
using System.IO;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Perspective.ViewModels;

namespace Perspective.Functions
{
    public class ListCollection
    {
        VM vm;

        public ListCollection(VM vm)
        {
            this.vm = vm;
        }

        public void RemoveTag(string tag, string tagPath)
        {
            if (vm.list_tags.Contains(tag))
                vm.list_tags.Remove(tag);

            if (vm.dictonary_tag_files.ContainsKey(tag))
                vm.dictonary_tag_files.Remove(tag);

            if (File.Exists(@tagPath))
            {
                File.Delete(tagPath);
                MessageBox.Show("Tag removed");
            }
        }

        
    }
}
