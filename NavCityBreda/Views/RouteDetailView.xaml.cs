using NavCityBreda.Helpers;
using NavCityBreda.Model;
using NavCityBreda.ViewModel;
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
        private RouteDetailVM vm;

        public RouteDetailView()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Enabled;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Route r = e.Parameter as Route;
            
            if (r == route)
            {
                vm.UpdateRoute();
                LandmarkList.SelectedIndex = -1;
                return;
            }

            route = r;
            vm = new RouteDetailVM(route);
            this.DataContext = vm;
            Util.MainPage.Title = route.Name;

            Map.MapElements.Clear();

            Zoom();

            MapPolyline m = Util.GetRouteLine(route.RouteObject, Color.FromArgb(255, 100, 100, 255));
            Map.MapElements.Add(m);

            Random rand = new Random();

            foreach (Landmark l in route.Landmarks)
            {
                int randint = rand.Next(0,20);
                if (randint > 15)
                    l.Visited = true;
                l.UpdateIconImage();
                Map.MapElements.Add(l.Icon);
            }
        }

        private async void Zoom()
        {
            await Task.Delay(TimeSpan.FromSeconds(1));
            await Map.TrySetViewBoundsAsync(route.Bounds, null, Windows.UI.Xaml.Controls.Maps.MapAnimationKind.None);
        }

        private void LandmarkList_ItemClick(object sender, ItemClickEventArgs e)
        {
            MainPage mp = Util.MainPage;
            mp.Navigate(typeof(LandmarkView), e.ClickedItem as Landmark);
        }

        private void StartRouteButton_Click(object sender, RoutedEventArgs e)
        {
            App.RouteManager.StartRoute(route);
            Util.MainPage.Navigate(typeof(MapView));
        }

        private void StopRouteButton_Click(object sender, RoutedEventArgs e)
        {
            App.RouteManager.StopRoute();
            Util.MainPage.Navigate(typeof(MapView));
        }
    }
}
