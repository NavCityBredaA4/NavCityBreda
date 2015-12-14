using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;

namespace NavCityBreda.Helpers
{
    class Util
    {
        public static ResourceLoader Loader 
        {
            get {
               return new Windows.ApplicationModel.Resources.ResourceLoader();
            }
        }
    }
}
