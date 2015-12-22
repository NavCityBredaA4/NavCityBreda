using NavCityBreda.Helpers;
using NavCityBreda.Model;
using NavCityBreda.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
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
    public sealed partial class RouteDetailView : Page
    {
        private Route route;
        private RouteDetailVM routedetailvm;

        public RouteDetailView()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Enabled;
            Settings.OnLanguageUpdate += Settings_OnLanguageUpdate;
        }

        private void Settings_OnLanguageUpdate(EventArgs e)
        {
            DrawRoute();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Route r = e.Parameter as Route;
            
            if (r == route)
            {
                routedetailvm.UpdateRoute();
                LandmarkList.SelectedIndex = -1;
                return;
            }

            route = r;
            routedetailvm = new RouteDetailVM(route);
            this.DataContext = routedetailvm;

            DrawRoute(); 
        }

        private async void DrawRoute()
        {
            Map.MapElements.Clear();

            Zoom();

            MapPolyline m = Util.GetRouteLine(route.RouteObject, Color.FromArgb(255, 100, 100, 255), 25, 6);
            Map.MapElements.Add(m);

            foreach (Landmark l in route.Landmarks)
            {
                l.UpdateIcon();
                Map.MapElements.Add(l.Icon);
            }
        }

        private async void Zoom()
        {
            await Task.Delay(TimeSpan.FromMilliseconds(500));
            await Map.TrySetViewBoundsAsync(route.Bounds, null, Windows.UI.Xaml.Controls.Maps.MapAnimationKind.None);
        }

        private void LandmarkList_ItemClick(object sender, ItemClickEventArgs e)
        {
            MainPage mp = App.MainPage;
            mp.Navigate(typeof(LandmarkView), e.ClickedItem as Landmark);
        }

        private void StartRouteButton_Click(object sender, RoutedEventArgs e)
        {
            App.RouteManager.StartRoute(route);
            App.MainPage.Navigate(typeof(MapView));
        }

        private void StopRouteButton_Click(object sender, RoutedEventArgs e)
        {
            App.RouteManager.StopRoute();
            App.MainPage.Navigate(typeof(MapView));
        }
    }
}
