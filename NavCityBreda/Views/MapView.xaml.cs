using NavCityBreda.ViewModels;
using System;
using System.Linq;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using Windows.Devices.Geolocation;
using Windows.UI.Xaml.Controls.Maps;
using NavCityBreda.Model;
using Windows.Storage.Streams;
using NavCityBreda.Helpers;
using Windows.UI;
using System.Threading.Tasks;
using Windows.Devices.Geolocation.Geofencing;
using Windows.UI.Core;

namespace NavCityBreda.Views
{
    public sealed partial class MapView : Page
    {
        MapIcon CurrentPosition;
        MapPolyline CurrentNavigationLine;

        private bool Zooming = false;

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

            CurrentNavigationLine = new MapPolyline() { StrokeColor = Color.FromArgb(255, 125, 255, 150), StrokeThickness = 5, ZIndex = 100 };

            App.Geo.OnPositionUpdate += Geo_OnPositionUpdate;
            App.RouteManager.OnStatusUpdate += RouteManager_OnStatusUpdate;
            App.RouteManager.OnLandmarkChanged += RouteManager_OnLandmarkChanged;
            App.RouteManager.OnLandmarkVisited += RouteManager_OnLandmarkVisited;
            App.CompassTracker.OnSlowHeadingUpdated += CompassTracker_OnHeadingUpdated;
            Settings.OnLanguageUpdate += Settings_OnLanguageUpdate;

            Track(false);
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
                if (mapvm.Tracking && e.Heading.HeadingTrueNorth.HasValue)
                    Map.TryRotateToAsync((double)e.Heading.HeadingTrueNorth);
            });
        }

        private void RouteManager_OnLandmarkVisited(object sender, LandmarkVisitedEventArgs e)
        {
            Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
            {
                if (e.Status == LandmarkVisitedEventArgs.VisitedStatus.ENTERED)
                    App.MainPage.Navigate(typeof(LandmarkDetailView), e.Landmark);
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
                {
                    DrawRoute();
                    ZoomRoute();
                }
                else
                    RemoveRoute();
            });
        }

        private async void DrawRoute()
        {
            if (App.RouteManager.Status != RouteManager.RouteStatus.STARTED) return;

            Route r = App.RouteManager.CurrentRoute;

            if (r == null) return;

            GeofenceMonitor.Current.Geofences.Clear();
            Map.MapElements.Clear();
            await Task.Delay(TimeSpan.FromMilliseconds(10));
            Map.MapElements.Add(CurrentPosition);
            Map.MapElements.Add(CurrentNavigationLine);

            Map.MapElements.Add(Util.GetRouteLine(App.RouteManager.CurrentRoute.RouteObject, Color.FromArgb(200, 100, 100, 255), 50, 3));

            if (App.Geo.History.Count > 1)
                Map.MapElements.Add(Util.GetRouteLine(App.Geo.History.Select(p => p.Coordinate.Point.Position).ToList(), Color.FromArgb(255, 155, 155, 155), 250, 6));

            foreach (Landmark l in r.Landmarks)
            {
                if (l.Status != Landmark.LandmarkStatus.VISITED)
                    GeofenceMonitor.Current.Geofences.Add(new Geofence(l.Id, new Geocircle(l.Position.Position, 25), MonitoredGeofenceStates.Entered, true));
                l.UpdateIcon();
                await Task.Delay(TimeSpan.FromMilliseconds(3));
                Map.MapElements.Add(l.Icon);
            }
        }

        private void RemoveRoute()
        {
            Track(false);
            Map.MapElements.Clear();
            GeofenceMonitor.Current.Geofences.Clear();
            DrawCurrenPosition(App.Geo.Position.Coordinate.Point);
        }

        private void DrawCurrenPosition(Geopoint p)
        {
            if (!Map.MapElements.Contains(CurrentPosition))
            {
                Map.MapElements.Add(CurrentPosition);
                Zoom();
            }

            CurrentPosition.Location = p;
            if (mapvm.Tracking)
                Zoom();
        }

        private async Task<String> Zoom()
        {
            if (!Zooming)
            {
                Zooming = true;

                await Task.Delay(TimeSpan.FromMilliseconds(500));
                await Map.TrySetViewAsync(CurrentPosition.Location, 18);

                if (mapvm.Tracking)
                {
                    if (Map.Pitch != 35)
                    {
                        await Task.Delay(TimeSpan.FromMilliseconds(300));
                        await Map.TryTiltToAsync(35);
                    }

                    if (App.CompassTracker.Heading != null && App.CompassTracker.Heading.HeadingTrueNorth.HasValue)
                    {
                        await Task.Delay(TimeSpan.FromMilliseconds(300));
                        await Map.TryRotateToAsync((double)App.CompassTracker.Heading.HeadingTrueNorth);
                    }
                }
                else
                {
                    if (Map.Pitch != 0)
                    {
                        await Task.Delay(TimeSpan.FromMilliseconds(300));
                        await Map.TryTiltToAsync(0);
                    }
                    if (Map.Heading != 0)
                    {
                        await Task.Delay(TimeSpan.FromMilliseconds(300));
                        await Map.TryRotateToAsync(0);
                    }

                }
                Zooming = false;
                return "success";
            }

            return "error";
        }

        private async void ZoomRoute()
        {
            DisableControls(true);
            mapvm.Tracking = false;
            TrackUser.IsEnabled = false;
            await Task.Delay(TimeSpan.FromMilliseconds(500));
            await Map.TryRotateToAsync(0);
            await Task.Delay(TimeSpan.FromMilliseconds(300));
            await Map.TrySetViewBoundsAsync(App.RouteManager.CurrentRoute.Bounds, null, Windows.UI.Xaml.Controls.Maps.MapAnimationKind.Default);
            await Task.Delay(TimeSpan.FromMilliseconds(1000));
            mapvm.Tracking = true;
            await Zoom();
            TrackUser.IsEnabled = true;
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

            App.MainPage.Navigate(typeof(LandmarkDetailView), l);
        }

        private void LandmarkName_Tapped(object sender, TappedRoutedEventArgs e)
        {
            App.MainPage.Navigate(typeof(LandmarkDetailView), App.RouteManager.CurrentLandmark);
        }

        private void RecalculateRoute_Click(object sender, RoutedEventArgs e)
        {
            App.RouteManager.UpdateRoute();
        }

        private void TrackUser_Click(object sender, RoutedEventArgs e)
        {
            Track((bool)((ToggleButton)sender).IsChecked);
        }

        private async void Track(bool tracking)
        {
            mapvm.Tracking = tracking;

            if (tracking)
            {
                DisableControls(true);

                if (CurrentPosition.Location != null)
                    await Zoom();
            }
            else
            {
                if (CurrentPosition.Location != null)
                    await Zoom();

                DisableControls(false);
            }
        }

        private void DisableControls(bool disable)
        {
            if (disable)
            {
                Map.PanInteractionMode = MapPanInteractionMode.Disabled;
                Map.RotateInteractionMode = MapInteractionMode.Disabled;
                Map.TiltInteractionMode = MapInteractionMode.Disabled;
                Map.ZoomInteractionMode = MapInteractionMode.Disabled;
            }
            else
            {
                Map.PanInteractionMode = MapPanInteractionMode.Auto;
                Map.RotateInteractionMode = MapInteractionMode.GestureAndControl;
                Map.TiltInteractionMode = MapInteractionMode.GestureOnly;
                Map.ZoomInteractionMode = MapInteractionMode.GestureAndControl;
            }
        }
    }
}
