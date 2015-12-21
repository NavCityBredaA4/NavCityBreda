using NavCityBreda.Helpers;
using NavCityBreda.ViewModels;
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
            
            this.DataContext = new MainPageVM();
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

        public void Navigate(Type type)
        {
            Frame.Navigate(type);
        }

        public void Navigate(Type type, object param)
        {
            Frame.Navigate(type, param);
        }

        public string Title { get { return PageTitle.Text; } set { PageTitle.Text = value; } }

        public void NavButton_Click(object sender, RoutedEventArgs arg)
        {
            NavView.IsPaneOpen = !NavView.IsPaneOpen;
        }

        private void Frame_Navigated(object sender, NavigationEventArgs e)
        {
            //Dirty Hack
            string pagename = e.SourcePageType.ToString().Split('.').Last();

            NavList.SelectedIndex = -1;

            switch (pagename.ToLower())
            {
                default:
                    PageTitle.Text = "Nav City Breda";
                    break;
                case "helpview":
                    NavList.SelectedIndex = 2;
                    break;
                case "settingsview":
                    NavList.SelectedIndex = 3;
                    break;
                case "mapview":
                    NavList.SelectedIndex = 0;
                    break;
                case "routedetailview":
                    NavList.SelectedIndex = 1;
                    break;
                case "routeview":
                    NavList.SelectedIndex = 1;
                    break;
                case "landmarkview":
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

            if (NavListMap.IsSelected)
            {
                if (Frame != null)
                {
                    if (Frame.CanGoBack)
                        Frame.BackStack.Clear();
                    Frame.Navigate(typeof(MapView));
                }
            }
            else if (NavListHelp.IsSelected)
                Frame.Navigate(typeof(HelpView));
            else if (NavListSettings.IsSelected)
                Frame.Navigate(typeof(SettingsView));
            else if (NavListRoute.IsSelected)
            {
                if (App.RouteManager.CurrentRoute == null)
                    Frame.Navigate(typeof(RouteView));
                else
                    Frame.Navigate(typeof(RouteDetailView), App.RouteManager.CurrentRoute);
            }
        }

        private void NavList_Tapped(object sender, TappedRoutedEventArgs e)
        {
            NavView.IsPaneOpen = false;
        }

        private void Grid_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            if (e.Cumulative.Translation.X > 20)
            {
                NavView.IsPaneOpen = true;
            }
        }

        private void MySplitviewPane_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            if (e.Cumulative.Translation.X < -20)
            {
                NavView.IsPaneOpen = false;
            }
        }

        private void GPSRefresh_Tapped(object sender, TappedRoutedEventArgs e)
        {
            App.Geo.ForceRefresh();
        }
    }
}
