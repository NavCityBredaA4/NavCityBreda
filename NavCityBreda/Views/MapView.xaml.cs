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
using Windows.Storage;
using Windows.Devices.Geolocation.Geofencing;
using Windows.UI.Core;

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

            CurrentPosition = new MapIcon();
            CurrentPosition.NormalizedAnchorPoint = new Point(0.5, 0.5);
            CurrentPosition.Title = "";
            CurrentPosition.ZIndex = 999;
            CurrentPosition.Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/CurrentLocationRound.png"));
            Map.MapElements.Add(CurrentPosition);

            App.Geo.PositionChanged += Geo_PositionChanged;
            GeofenceMonitor.Current.GeofenceStateChanged += Current_GeofenceStateChanged;
            App.RouteManager.OnPositionUpdate += RouteManager_OnPositionUpdate;
            App.RouteManager.OnStatusUpdate += RouteManager_OnStatusUpdate;
        }

        private void Current_GeofenceStateChanged(GeofenceMonitor sender, object args)
        {
            var reports = sender.ReadReports();

            Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                foreach (GeofenceStateChangeReport report in reports)
                {
                    GeofenceState state = report.NewState;
                    Geofence geofence = report.Geofence;

                    if (state == GeofenceState.Removed)
                    {
                        GeofenceMonitor.Current.Geofences.Remove(geofence);
                    }

                    else if (state == GeofenceState.Entered)
                    {
                        Landmark i = App.RouteManager.CurrentRoute.Landmarks.Where(t => t.Id == geofence.Id).First();
                        i.Visited = true;
                        Util.MainPage.Navigate(typeof(LandmarkView), i);
                    }
                }
            });

        }

        private void RouteManager_OnPositionUpdate(object sender, RoutePositionChangedEventArgs e)
        {
            Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                MapPolyline linebit = Util.GetRouteLine(e.Old.Coordinate.Point.Position, e.New.Coordinate.Point.Position, Color.FromArgb(255, 155, 155, 155));
                Map.MapElements.Add(linebit);
            });
        }

        private void RouteManager_OnStatusUpdate(object sender, RouteStatusChangedEventArgs e)
        {
            Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (e.Status == RouteManager.State.STARTED)
                    DrawRoute();
                else
                    RemoveRoute();
            });           
        }

        private void Geo_PositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
            {
                DrawCurrenPosition(args.Position.Coordinate.Point);
            });
        }

        private void DrawRoute()
        {
            Route r = App.RouteManager.CurrentRoute;

            Settings.LOCAL_SETTINGS.Values["track"] = false;

            GeofenceMonitor.Current.Geofences.Clear();
            Map.MapElements.Clear();
            Map.MapElements.Add(CurrentPosition);

            foreach(Landmark l in r.Landmarks)
            {
                GeofenceMonitor.Current.Geofences.Add(new Geofence(l.Id, new Geocircle(l.Location.Position, 10)));
                l.UpdateIconImage();
                Map.MapElements.Add(l.Icon);
            }

            Map.MapElements.Add(Util.GetRouteLine(App.RouteManager.CurrentRoute.RouteObject, Color.FromArgb(200, 100, 100, 255)));

            ZoomRoute();
        }

        private void RemoveRoute()
        {
            Settings.LOCAL_SETTINGS.Values["track"] = true;
            Map.MapElements.Clear();
            DrawCurrenPosition(App.Geo.Position.Coordinate.Point);
        }

        private void DrawCurrenPosition(Geopoint p)
        {
            if (!Map.MapElements.Contains(CurrentPosition))
                Map.MapElements.Add(CurrentPosition);

            CurrentPosition.Location = p;
            if ((bool)Settings.LOCAL_SETTINGS.Values["track"])
                Zoom();
        }

        private async void Zoom()
        {
            await Task.Delay(TimeSpan.FromSeconds(1));
            await Map.TrySetViewAsync(CurrentPosition.Location, 15, 0, 0, MapAnimationKind.Linear);
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
            if (w == null || !(w is Landmark))
                return;

            Landmark l = w as Landmark;

            Util.MainPage.Navigate(typeof(LandmarkView), l);
        }
    }
}
