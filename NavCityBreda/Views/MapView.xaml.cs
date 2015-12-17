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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace NavCityBreda.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MapView : Page
    {
        MapIcon CurrenPosition;

        public MapView()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Enabled;

            this.DataContext = new MapVM();

            CurrenPosition = new MapIcon();
            Map.MapElements.Add(CurrenPosition);

            App.Geo.PositionChanged += Geo_PositionChanged;

            DrawLandmarks();
        }

        private void DrawLandmarks()
        {
            Route r = App.RouteManager.CurrentRoute;

            Map.MapElements.Clear();

            foreach(Landmark l in r.Waypoints.Where(l => l is Landmark))
            {
                MapIcon m = new MapIcon();
                m.Location = l.Location;
                m.NormalizedAnchorPoint = new Point(0.5, 1.0);
                m.Title = l.Name;
                m.ZIndex = 10;
                //m.Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///" + l.Image));
                Map.MapElements.Add(m);
            }
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
            CurrenPosition.Location = p;
            CurrenPosition.NormalizedAnchorPoint = new Point(0.5, 1.0);
            CurrenPosition.Title = "Current Position";
            CurrenPosition.ZIndex = 999;
        }

        private void Map_MapElementClick(MapControl sender, MapElementClickEventArgs args)
        {
            MapIcon i = args.MapElements.Where(p => p is MapIcon).Cast<MapIcon>().First();

            Waypoint w = App.RouteManager.CurrentRoute.Get(i);
            if (!(w is Landmark))
                return;

            Landmark l = w as Landmark;

            Util.MainPage.Navigate(typeof(LandmarkView), l);

            //Map.TrySetViewBoundsAsync(App.RouteManager.Routes.First().Bounds, new Thickness(10), MapAnimationKind.Bow);
        }
    }
}
