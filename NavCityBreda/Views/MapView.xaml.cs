using NavCityBreda.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Devices.Geolocation;
using Windows.UI.Xaml.Controls.Maps;
using NavCityBreda.Model;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using System.Diagnostics;
using NavCityBreda.Helpers;
using Windows.UI;
using System.Threading.Tasks;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace NavCityBreda.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MapView : Page
    {
        MapIcon CurrentPosition;

        public MapView()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Enabled;

            this.DataContext = new MapVM();

            CurrentPosition = new MapIcon();
            CurrentPosition.NormalizedAnchorPoint = new Point(0.5, 1.0);
            CurrentPosition.Title = "Current Position";
            CurrentPosition.ZIndex = 999;
            Map.MapElements.Add(CurrentPosition);

            App.Geo.PositionChanged += Geo_PositionChanged;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (App.RouteManager.CurrentRoute != null)
                DrawRoute();
        }

        private void DrawRoute()
        {
            Route r = App.RouteManager.CurrentRoute;

            Map.MapElements.Clear();
            Map.MapElements.Add(CurrentPosition);

            foreach(Landmark l in r.Waypoints.Where(l => l is Landmark))
            {
                MapIcon m = new MapIcon();
                m.Location = l.Location;
                m.NormalizedAnchorPoint = new Point(0.5, 1.0);
                m.Title = l.Name;
                m.ZIndex = 10;
                Map.MapElements.Add(m);
            }

            Map.MapElements.Add(Util.GetRouteLine(App.RouteManager.CurrentRoute.RouteObject, Color.FromArgb(255, 100, 100, 255)));

            ZoomRoute();
        }

        private void Geo_PositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, () =>
            {
                DrawCurrenPosition(new Geopoint(args.Position.Coordinate.Point.Position));
            });
        }

        private void DrawCurrenPosition(Geopoint p)
        {
            if (!Map.MapElements.Contains(CurrentPosition))
                Map.MapElements.Add(CurrentPosition);

            CurrentPosition.Location = p;
            if ((bool)Settings.LOCAL_SETTINGS.Values["track"]) ;
                Zoom();
        }

        private async void Zoom()
        {
            await Task.Delay(TimeSpan.FromSeconds(1));
            await Map.TrySetViewAsync(CurrentPosition.Location);
            Map.ZoomLevel = 14;
        }

        private async void ZoomRoute()
        {
            await Task.Delay(TimeSpan.FromSeconds(1));
            await Map.TrySetViewBoundsAsync(App.RouteManager.CurrentRoute.Bounds, null, Windows.UI.Xaml.Controls.Maps.MapAnimationKind.Linear);
        }

        private void Map_MapElementClick(MapControl sender, MapElementClickEventArgs args)
        {
            MapIcon i = args.MapElements.Where(p => p is MapIcon).Cast<MapIcon>().First();

            Waypoint w = App.RouteManager.CurrentRoute.Get(i);
            if (!(w is Landmark))
                return;

            Landmark l = w as Landmark;

            Util.MainPage.Navigate(typeof(LandmarkView), l);
        }
    }
}
