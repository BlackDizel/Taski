using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media.Imaging;

namespace TaskiWindowsPhone7.Controls
{
    public partial class SelectableImage : UserControl
    {
        public SelectableImage()
        {
            InitializeComponent();
            IsSelected = false;

        }
        private bool isSelected;
        public bool IsSelected
        {
            get { return isSelected; }
            set 
            {
                isSelected = value;
                if (isSelected) brd.Visibility = Visibility.Visible;
                else brd.Visibility = Visibility.Collapsed;
            
            }
        }

        public BitmapImage Source
        {
            get {return img.Source as BitmapImage;}
            set { img.Source = value; }
        }
    }
}
