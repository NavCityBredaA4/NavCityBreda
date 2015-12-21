using NavCityBreda.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;

namespace NavCityBreda.ViewModels
{
    public class MapVM : TemplateVM
    {
        public MapVM() : base(Util.Loader.GetString("Map"))
        {

        }

        protected override void UpdatePropertiesToNewLanguage()
        {
            
        }

        internal void UpdateMap()
        {
            if(App.MainPage != null)
                App.MainPage.Title = Util.Loader.GetString("Map");
        }
    }
}
