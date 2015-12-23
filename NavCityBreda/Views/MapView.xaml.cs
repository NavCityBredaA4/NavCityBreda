using NavCityBreda.ViewModels;
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
        MapPolyline CurrentNavigationLine;

        MapVM mapvm;

        public MapView()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Enabled;

            mapvm = new MapVM();
            this.DataContext = mapvm;

            CurrentPosition = new MapIcon();
            CurrentPosition.NormalizedAnchorPoint = new Point(0.5, 0.5);
            CurrentPosition.Title = "";
            CurrentPosition.ZIndex = 999;
            CurrentPosition.Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/CurrentLocationRound.png"));
            Map.MapElements.Add(CurrentPosition);

            CurrentNavigationLine = new MapPolyline() { StrokeColor = Color.FromArgb(255, 255, 100, 100), StrokeThickness = 6, ZIndex = 100 };
            //Map.MapElements.Add(CurrentNavigationLine);

            App.Geo.OnPositionUpdate += Geo_OnPositionUpdate;
            App.RouteManager.OnStatusUpdate += RouteManager_OnStatusUpdate;
            App.RouteManager.OnLandmarkChanged += RouteManager_OnLandmarkChanged;
            App.RouteManager.OnLandmarkVisited += RouteManager_OnLandmarkVisited;
            App.CompassTracker.OnSlowHeadingUpdated += CompassTracker_OnHeadingUpdated;
            Settings.OnLanguageUpdate += Settings_OnLanguageUpdate;
        }

        private void Settings_OnLanguageUpdate(EventArgs e)
        {
            DrawRoute();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            mapvm.UpdateMap();
            App.Geo.TryConnectIfNull();
        }

        private void CompassTracker_OnHeadingUpdated(object sender, HeadingUpdatedEventArgs e)
        {
            Dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
            {
                if (Settings.Tracking && e.Heading.HeadingTrueNorth.HasValue)
                   Map.TryRotateToAsync((double)e.Heading.HeadingTrueNorth);
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

        private void RouteManager_OnLandmarkChanged(object sender, LandmarkChangedEventArgs e)
        {
            Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                CurrentNavigationLine.Path = e.Route.Path;

                if (!Map.MapElements.Contains(CurrentNavigationLine))
                    Map.MapElements.Add(CurrentNavigationLine);
            });
        }

        private void Geo_OnPositionUpdate(object sender, PositionUpdatedEventArgs e)
        {
            if (App.RouteManager.Status == RouteManager.RouteStatus.STARTED)
            {
                Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    MapPolyline linebit = Util.GetRouteLine(e.Old.Coordinate.Point.Position, e.New.Coordinate.Point.Position, Color.FromArgb(255, 155, 155, 155), 250, 6);
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
            if (App.RouteManager.Status != RouteManager.RouteStatus.STARTED) return;

            Route r = App.RouteManager.CurrentRoute;

            if (r == null) return;

            GeofenceMonitor.Current.Geofences.Clear();
            Map.MapElements.Clear();
            Map.MapElements.Add(CurrentPosition);
            Map.MapElements.Add(CurrentNavigationLine);
            if (App.Geo.History.Count > 1) 
                Map.MapElements.Add(Util.GetRouteLine(App.Geo.History.Select(p => p.Coordinate.Point.Position).ToList(), Color.FromArgb(255, 155, 155, 155), 250, 6));

            foreach (Landmark l in r.Landmarks)
            {
                if (!l.Visited)
                    GeofenceMonitor.Current.Geofences.Add(new Geofence(l.Id, new Geocircle(l.Location.Position, 35), MonitoredGeofenceStates.Entered, true));
                l.UpdateIcon();
                Map.MapElements.Add(l.Icon);
            }

            Map.MapElements.Add(Util.GetRouteLine(App.RouteManager.CurrentRoute.RouteObject, Color.FromArgb(200, 100, 100, 255), 50, 3));

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

        private async Task<String> Zoom()
        {
            await Task.Delay(TimeSpan.FromMilliseconds(500));
            await Map.TrySetViewAsync(CurrentPosition.Location, 17);

            if(App.RouteManager.Status == RouteManager.RouteStatus.STARTED)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(500));
                await Map.TryTiltToAsync(40);
            }
            else
            {
                await Task.Delay(TimeSpan.FromMilliseconds(500));
                await Map.TryTiltToAsync(0);
            }
            return "";
        }

        private async void ZoomRoute()
        {
            Settings.Tracking = false;
            await Task.Delay(TimeSpan.FromMilliseconds(500));
            await Map.TrySetViewBoundsAsync(App.RouteManager.CurrentRoute.Bounds, null, Windows.UI.Xaml.Controls.Maps.MapAnimationKind.Default);
            await Task.Delay(TimeSpan.FromMilliseconds(500));
            await Map.TryRotateToAsync(0);
            await Task.Delay(TimeSpan.FromMilliseconds(1500));
            await Zoom();
            Settings.Tracking = true;
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
