using NavCityBreda.Helpers;
using NavCityBreda.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace NavCityBreda
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        public MainPage()
        {
            this.InitializeComponent();
            Frame.Navigated += Frame_Navigated;
            SystemNavigationManager.GetForCurrentView().BackRequested += OnBackRequested;
            Frame.Navigate(typeof(MapView));
        }

        private void OnBackRequested(object sender, BackRequestedEventArgs e)
        {
            if (Frame.CanGoBack)
            {
                e.Handled = true;
                Frame.GoBack();
            }
        }

        public void NavButton_Click(object sender, RoutedEventArgs arg)
        {
            NavView.IsPaneOpen = !NavView.IsPaneOpen;
        }

        private void Frame_Navigated(object sender, NavigationEventArgs e)
        {
            //Dirty Hack
            string pagename = e.SourcePageType.ToString().Split('.').Last();
            Debug.WriteLine(pagename);

            NavList.SelectedIndex = -1;

            switch (pagename.ToLower())
            { 
                default:
                case "helpview":
                    PageTitle.Text = Util.Loader.GetString("PageTitleHelp");
                    NavListHelp.IsSelected = true;
                    break;
                case "settingsview":
                    PageTitle.Text = Util.Loader.GetString("PageTitleSettings");
                    NavListSettings.IsSelected = true;
                    break;
                case "mapview":
                    PageTitle.Text = Util.Loader.GetString("PageTitleMap");
                    break;
                case "routedetailview":
                    PageTitle.Text = Util.Loader.GetString("PageTitleRouteDetail");
                    break;
                case "routeview":
                    PageTitle.Text = Util.Loader.GetString("PageTitleRoute");
                    break;
                case "waypointview":
                    PageTitle.Text = Util.Loader.GetString("PageTitleWaypoint");
                    break;
            }

            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                Frame.CanGoBack ?
                AppViewBackButtonVisibility.Visible :
                AppViewBackButtonVisibility.Collapsed;
        }

        private void NavList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            NavView.IsPaneOpen = false;

            if (NavListHelp.IsSelected)
            {
                Frame.Navigate(typeof(HelpView));
            }
            else if (NavListSettings.IsSelected)
            {
                Frame.Navigate(typeof(SettingsView));
            }
        }

        private void Grid_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            if (e.Cumulative.Translation.X > 50)
            {
                NavView.IsPaneOpen = true;
            }
        }

        private void MySplitviewPane_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            if (e.Cumulative.Translation.X < -50)
            {
                NavView.IsPaneOpen = false;
            }
        }
    }
}
