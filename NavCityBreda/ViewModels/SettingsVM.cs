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
            NotifyPropertyChanged(nameof(Reset));
            NotifyPropertyChanged(nameof(ResetHeader));
            App.MainPage.Title = Util.Loader.GetString("Settings");
        }

        public string Language
        {
            get
            {
                return Util.Loader.GetString("Language");
            }
        }

        public string ResetHeader
        {
            get
            {
                return Util.Loader.GetString("Reset");
            }
        }

        public string Reset
        {
            get
            {
                return Util.Loader.GetString("Reset") + " " + Util.Loader.GetString("Application");
            }
        }
    }
}
