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
        private static ApplicationDataContainer LOCAL_SETTINGS = ApplicationData.Current.LocalSettings;

        public delegate void OnLanguageUpdateHandler(EventArgs e);
        public static event OnLanguageUpdateHandler OnLanguageUpdate; 

        public static bool Tracking {
            get
            {
                return (bool)LOCAL_SETTINGS.Values["tracking"];
            }
            set
            {
                LOCAL_SETTINGS.Values["tracking"] = value;
            }
        }

        static Settings()
        {
            LOCAL_SETTINGS.Values["tracking"] = true;

            if (CurrentLanguage == "")
                ApplicationLanguages.PrimaryLanguageOverride = "en";
        }

        public static async void ChangeLanguage(string lang)
        {
                ApplicationLanguages.PrimaryLanguageOverride = lang;
                await Task.Delay(TimeSpan.FromMilliseconds(100));
                //App.rootFrame.Navigate(typeof(MainPage));
                OnLanguageUpdate(new EventArgs());
        }

        public static string CurrentLanguage { get { return ApplicationLanguages.PrimaryLanguageOverride; } }
    }
}
