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
    public class SettingsVM : TemplateVM
    {

        public SettingsVM() : base(Util.Loader.GetString("Settings"))
        {
            
        }

        protected override void UpdatePropertiesToNewLanguage()
        {
            NotifyPropertyChanged(nameof(Language));
            App.MainPage.Title = Util.Loader.GetString("Settings");
        }

        public string Language
        {
            get
            {
                return Util.Loader.GetString("Language");
            }
        }
    }
}
