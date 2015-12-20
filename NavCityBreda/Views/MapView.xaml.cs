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

            App.Geo.OnPositionUpdate += Geo_OnPositionUpdate;
            App.RouteManager.OnStatusUpdate += RouteManager_OnStatusUpdate;
            App.RouteManager.OnRouteChanged += RouteManager_OnRouteChanged;
            App.RouteManager.OnLandmarkVisited += RouteManager_OnLandmarkVisited;
            App.CompassTracker.OnHeadingUpdated += CompassTracker_OnHeadingUpdated;
        }

        private void CompassTracker_OnHeadingUpdated(object sender, HeadingUpdatedEventArgs e)
        {
            Dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
            {
                if (Settings.Tracking && e.Heading.HeadingTrueNorth.HasValue) return;
                // Map.TryRotateToAsync((double)e.Heading.HeadingTrueNorth);
            });
            
        }

        private void RouteManager_OnLandmarkVisited(object sender, LandmarkVisitedEventArgs e)
        {
            Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
            {
                if (e.Status == LandmarkVisitedEventArgs.VisitedStatus.ENTERED)
                    App.MainPage.Navigate(typeof(LandmarkView), e.Landmark);
            }); 
        }

        private void RouteManager_OnRouteChanged(object sender, RouteChangedEventArgs e)
        {
            Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                MapPolyline linebit = Util.GetRouteLine(e.Route, Color.FromArgb(255, 255, 100, 100));
                Map.MapElements.Add(linebit);
            });
        }

        private void Geo_OnPositionUpdate(object sender, PositionUpdatedEventArgs e)
        {
            if (App.RouteManager.Status == RouteManager.RouteStatus.STARTED)
            {
                Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    MapPolyline linebit = Util.GetRouteLine(e.Old.Coordinate.Point.Position, e.New.Coordinate.Point.Position, Color.FromArgb(255, 155, 155, 155));
                    Map.MapElements.Add(linebit);
                });
            }

            Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
            {
                DrawCurrenPosition(e.New.Coordinate.Point);
            });
        }

        private void RouteManager_OnStatusUpdate(object sender, RouteStatusChangedEventArgs e)
        {
            Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (e.Status == RouteManager.RouteStatus.STARTED)
                    DrawRoute();
                else
                    RemoveRoute();
            });           
        }

        private void DrawRoute()
        {
            Route r = App.RouteManager.CurrentRoute;

            Settings.Tracking = false;

            GeofenceMonitor.Current.Geofences.Clear();
            Map.MapElements.Clear();
            Map.MapElements.Add(CurrentPosition);

            foreach(Landmark l in r.Landmarks)
            {
                GeofenceMonitor.Current.Geofences.Add(new Geofence(l.Id, new Geocircle(l.Location.Position, 40)));
                l.UpdateIconImage();
                Map.MapElements.Add(l.Icon);
            }

            Map.MapElements.Add(Util.GetRouteLine(App.RouteManager.CurrentRoute.RouteObject, Color.FromArgb(200, 100, 100, 255)));

            ZoomRoute();
        }

        private void RemoveRoute()
        {
            Settings.Tracking = true;
            Map.MapElements.Clear();
            DrawCurrenPosition(App.Geo.Position.Coordinate.Point);
        }

        private void DrawCurrenPosition(Geopoint p)
        {
            if (!Map.MapElements.Contains(CurrentPosition))
                Map.MapElements.Add(CurrentPosition);

            CurrentPosition.Location = p;
            if (Settings.Tracking)
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
            if (App.RouteManager.Status == RouteManager.RouteStatus.STOPPED) return;

            MapIcon i = args.MapElements.Where(p => p is MapIcon).Cast<MapIcon>().FirstOrDefault();
            if (i == null) return;

            Waypoint w = App.RouteManager.CurrentRoute.Get(i);
            if (w == null || !(w is Landmark))
                return;

            Landmark l = w as Landmark;

            App.MainPage.Navigate(typeof(LandmarkView), l);
        }
    }
}
