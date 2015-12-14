using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Globalization;
using Windows.Storage;

namespace NavCityBreda.Helpers
{
    static class Settings
    {
        public static ApplicationDataContainer LOCAL_SETTINGS = ApplicationData.Current.LocalSettings;

        static Settings()
        {
            //Define default settings here
        }

        public static string Language
        {
            get
            {
                return "";
            }
            set
            {
                ApplicationLanguages.PrimaryLanguageOverride = value;
            }
        }
    }
}
