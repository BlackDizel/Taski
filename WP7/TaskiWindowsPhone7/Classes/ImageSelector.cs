using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;

namespace TaskiWindowsPhone7.Classes
{
    static class ImageSelector
    {
        public static BitmapImage[] images =
            {
                new BitmapImage(new Uri("/Assets/ProjectIcon/blue.png", UriKind.Relative)),
                new BitmapImage(new Uri("/Assets/ProjectIcon/green.png", UriKind.Relative)),
                new BitmapImage(new Uri("/Assets/ProjectIcon/lightblue.png", UriKind.Relative)),
                new BitmapImage(new Uri("/Assets/ProjectIcon/lightgray.png", UriKind.Relative)),
                new BitmapImage(new Uri("/Assets/ProjectIcon/orange.png", UriKind.Relative)),
                new BitmapImage(new Uri("/Assets/ProjectIcon/red.png", UriKind.Relative))            
            };
    }
}
