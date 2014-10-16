using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaskiWindowsPhone7
{
    public class LocalizedStrings
    {
        private static AppResources _localizedResources = new AppResources();

        public AppResources AppResources
        {
            get { return _localizedResources; }
        }
    }
}
