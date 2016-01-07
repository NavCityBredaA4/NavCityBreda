using NavCityBreda.Helpers;
using NavCityBreda.Model.Object;
using NavCityBreda.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Navigation;

namespace NavCityBreda.Views
{
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
            }
            else
            {
                route = r;
                routedetailvm = new RouteDetailVM(route);
                this.DataContext = routedetailvm;
            }

            DrawRoute();
        }

        private async void DrawRoute()
        {
            while (Map.MapElements.Any())
            {
                Map.MapElements.Clear();
                await Task.Delay(TimeSpan.FromMilliseconds(100));
            }

            Zoom();

            await Task.Delay(TimeSpan.FromMilliseconds(25));

            MapPolyline m = Util.GetRouteLine(route.RouteObject, Color.FromArgb(255, 100, 100, 255), 25, 6);
            Map.MapElements.Add(m);

            foreach (Landmark l in route.Landmarks)
            {
                l.UpdateIcon();
                await Task.Delay(TimeSpan.FromMilliseconds(3));
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
            mp.Navigate(typeof(LandmarkDetailView), e.ClickedItem as Landmark);
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
