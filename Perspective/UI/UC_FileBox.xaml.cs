﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Perspective.UI
{
    /// <summary>
    /// UC_FileBox.xaml 的互動邏輯
    /// </summary>
    public partial class UC_FileBox : UserControl
    {
        public UC_FileBox()
        {
            InitializeComponent();
        }

        #region Dependency Property
        public static readonly DependencyProperty imgSource_Property =
                DependencyProperty.Register("imgSource", typeof(string), typeof(UC_FileBox),
                new UIPropertyMetadata(null));

        public string imgSource //提供內部binding之相依屬性
        {
            get { return (string)GetValue(imgSource_Property); }
            set { SetValue(imgSource_Property, value); }
        }

        public static readonly DependencyProperty str_btn_text_Property =
               DependencyProperty.Register("str_btn_text", typeof(string), typeof(UC_FileBox),
               new UIPropertyMetadata(null));

        public string str_btn_text //提供內部binding之相依屬性
        {
            get { return (string)GetValue(str_btn_text_Property); }
            set { SetValue(str_btn_text_Property, value); }
        }

        public static readonly DependencyProperty vis_btn_remove_Property =
               DependencyProperty.Register("vis_btn_remove", typeof(bool), typeof(UC_FileBox),
               new UIPropertyMetadata(null));

        public bool vis_btn_remove //提供內部binding之相依屬性
        {
            get { return (bool)GetValue(vis_btn_remove_Property); }
            set { SetValue(vis_btn_remove_Property, value); }
        }

        public static readonly DependencyProperty path_info_Property =
               DependencyProperty.Register("path_info", typeof(string), typeof(UC_FileBox),
               new UIPropertyMetadata(null));

        public string path_info //提供內部binding之相依屬性
        {
            get { return (string)GetValue(path_info_Property); }
            set { SetValue(path_info_Property, value); }
        }

        public static readonly DependencyProperty tbtn_isChecked_Property =
               DependencyProperty.Register("tbtn_isChecked", typeof(bool), typeof(UC_FileBox),
               new UIPropertyMetadata(null));

        public bool tbtn_isChecked //提供內部binding之相依屬性
        {
            get { return (bool)GetValue(tbtn_isChecked_Property); }
            set { SetValue(tbtn_isChecked_Property, value); }
        }
        #endregion

        #region Button Event
        public event MouseButtonEventHandler Tbtn_DoubleClick = delegate { };
        private void Tbtn_DoubleClick_Click(object sender, MouseButtonEventArgs e)
        {
            Tbtn_DoubleClick(sender, e);
        }
        
        public event RoutedEventHandler Tbtn_Checked = delegate { };
        private void Tbtn_Checked_Click(object sender, RoutedEventArgs e)
        {
            Tbtn_Checked(sender, e);
        }

        public event RoutedEventHandler Tbtn_UnChecked = delegate { };
        private void Tbtn_UnChecked_Click(object sender, RoutedEventArgs e)
        {
            Tbtn_UnChecked(sender, e);
        }

        public event RoutedEventHandler btn_delete_Click = delegate { };
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            btn_delete_Click(sender, e);
        }
        #endregion

    }
}
