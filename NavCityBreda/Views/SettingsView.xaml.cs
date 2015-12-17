using NavCityBreda.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace NavCityBreda.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsView : Page
    {
        public SettingsView()
        {
            this.InitializeComponent();
            throw new NotImplementedException("Tracking");
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Debug.WriteLine("Switching to settings: " + Settings.CurrentLanguage);

            switch (Settings.CurrentLanguage)
            {
                default:
                    Debug.WriteLine("No language found: " + Settings.CurrentLanguage);
                    break;
                case "en-US":
                    Language.SelectedIndex = 0;
                    break;
                case "de-DE":
                    Language.SelectedIndex = 2;
                    break;
                case "nl-NL":
                    Language.SelectedIndex = 1;
                    break;
                case "ja":
                    Language.SelectedIndex = 3;
                    break;
            }
        }

        private void Language_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LanguageLoading.IsActive = true;

            switch(Language.SelectedIndex)
            {
                default:
                    Language.SelectedIndex = 0;
                    break;
                case 0:
                    if(Settings.CurrentLanguage != "en-US")
                        Settings.ChangeLanguage("en-US");
                    break;
                case 2:
                    if (Settings.CurrentLanguage != "de-DE")
                        Settings.ChangeLanguage("de-DE");
                    break;
                case 1:
                    if (Settings.CurrentLanguage != "nl-NL")
                        Settings.ChangeLanguage("nl-NL");
                    break;
                case 3:
                    if (Settings.CurrentLanguage != "ja")
                        Settings.ChangeLanguage("ja-JP");
                    break;
            }

            LanguageLoading.IsActive = false;
        }
    }
}
