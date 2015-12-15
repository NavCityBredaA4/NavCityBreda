using NavCityBreda.Helpers;
using System;
using System.Collections.Generic;
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
        }

        private void DeutschesKnopfe_Click(object sender, RoutedEventArgs e)
        {
            Settings.ChangeLanguage("de-DE");
        }

        private void EnglishButton_Click(object sender, RoutedEventArgs e)
        {
            Settings.ChangeLanguage("en-US");
        }

        private void NederlandseKnop_Click(object sender, RoutedEventArgs e)
        {
            Settings.ChangeLanguage("nl-NL");
        }
    }
}
