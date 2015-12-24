using NavCityBreda.Helpers;
using NavCityBreda.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
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
        SettingsVM settingsvm;

        public SettingsView()
        {
            this.InitializeComponent();
            settingsvm = new SettingsVM();
            this.DataContext = settingsvm;
            //throw new NotImplementedException("Tracking + tilemaps");
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            switch (Settings.CurrentLanguage)
            {
                default:
                    Debug.WriteLine("Unsupported language: " + Settings.CurrentLanguage);
                    Language.SelectedIndex = 0;
                    break;
                case "en":
                    Language.SelectedIndex = 0;
                    break;
                case "nl":
                    Language.SelectedIndex = 1;
                    break;
                case "de":
                    Language.SelectedIndex = 2;
                    break;
                case "ja":
                    Language.SelectedIndex = 3;
                    break;
            }
        }

        private void Language_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch(Language.SelectedIndex)
            {
                default:
                    Language.SelectedIndex = 0;
                    break;
                case 0:
                    if(Settings.CurrentLanguage != "en")
                        Settings.ChangeLanguage("en");
                    break;
                case 1:
                    if (Settings.CurrentLanguage != "nl")
                        Settings.ChangeLanguage("nl");
                    break;
                case 2:
                    if (Settings.CurrentLanguage != "de")
                        Settings.ChangeLanguage("de");
                    break;
                case 3:
                    if (Settings.CurrentLanguage != "ja")
                        Settings.ChangeLanguage("ja");
                    break;
            }
        }

        private async void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            MessageDialog dlg = new MessageDialog(Util.Loader.GetString("ResetConfirmation"), Util.Loader.GetString("Reset"));
            dlg.Commands.Add(new UICommand(Util.Loader.GetString("Yes")) { Id = 1 } );
            dlg.Commands.Add(new UICommand(Util.Loader.GetString("No")) { Id = 0 });

            dlg.DefaultCommandIndex = 0;
            dlg.CancelCommandIndex = 1;

            var result = await dlg.ShowAsync();

            if((int)result.Id == 1)
            {
                ResetProgress.IsActive = true;
                App.RouteManager.StopRoute();
                Language.SelectedIndex = 0;
                await App.RouteManager.Reset();
                ResetProgress.IsActive = false;
            }
        }
    }
}
