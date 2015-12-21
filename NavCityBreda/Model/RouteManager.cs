using NavCityBreda.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Devices.Geolocation.Geofencing;
using Windows.Services.Maps;
using Windows.UI.Core;

namespace NavCityBreda.Model
{
    public class RouteManager
    {
        public delegate void StatusUpdateHandler(object sender, RouteStatusChangedEventArgs e);
        public event StatusUpdateHandler OnStatusUpdate;

        public delegate void OnLandmarkVisitedHandler(object sender, LandmarkVisitedEventArgs e);
        public event OnLandmarkVisitedHandler OnLandmarkVisited;

        public delegate void OnRouteChangedHandler (object sender, RouteChangedEventArgs e);
        public event OnRouteChangedHandler OnRouteChanged;

        private List<Route> _routes;
        public List<Route> Routes { get { return _routes;  } }

        private Route _currentroute;
        public Route CurrentRoute { get { return _currentroute; } }

        public string LoadingElement;

        private Landmark _currentlandmark;

        private MapRoute _routetolandmark;
        public MapRoute RouteToLandmark { get { return _routetolandmark;  } }

        public enum RouteStatus { LOADING, STOPPED, STARTED }
        public RouteStatus Status;

        CoreDispatcher dispatcher;

        public RouteManager()
        {
            _routes = new List<Route>();
            dispatcher = App.Dispatcher;

            LoadingElement = Util.Loader.GetString("Initializing") + "...";
            Status = RouteStatus.LOADING;
            GeofenceMonitor.Current.GeofenceStateChanged += Current_GeofenceStateChanged;
            LoadRoutes();  
        }

        private void Current_GeofenceStateChanged(GeofenceMonitor sender, object args)
        {
            var reports = sender.ReadReports();

            foreach (GeofenceStateChangeReport report in reports)
            {
                GeofenceState state = report.NewState;
                Geofence geofence = report.Geofence;
                Landmark i = App.RouteManager.CurrentRoute.Landmarks.Where(t => t.Id == geofence.Id).First();

                if (state == GeofenceState.Removed)
                {
                    GeofenceMonitor.Current.Geofences.Remove(geofence);
                }

                else if (state == GeofenceState.Entered)
                {
                    dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        i.Visited = true;
                    }); 
                    _currentlandmark = i;
                    LandmarkVisited(i, LandmarkVisitedEventArgs.VisitedStatus.ENTERED);
                    Util.SendToastNotification(i.Name, "TRANSLATED MESSAGE");
                    UpdateRoute();
                }

                else if(state == GeofenceState.Exited)
                {
                    LandmarkVisited(i, LandmarkVisitedEventArgs.VisitedStatus.EXITED);
                }
            }
        }

        private async void LoadRoutes()
        {
            string[] routefolders = Directory.GetDirectories("Routes/");

            _routes.Clear();

            foreach(string folder in routefolders)
            {
                Debug.WriteLine(folder);
                string foldername = Path.GetFileName(folder);
                if (foldername != "img")
                {
                    Route r = JSONParser.LoadRoute(foldername);
                    LoadingElement =  Util.Loader.GetString("Loading") + " " + r.Name + "...";
                    await r.CalculateRoute();
                    _routes.Add(r);
                }
            }

            LoadingElement = Util.Loader.GetString("Done") + "...";
            Status = RouteStatus.STOPPED;
        }

        public async void UpdateRoute()
        {
            if (App.Geo.Position == null) return;

            _currentlandmark = _currentroute.Landmarks.FirstOrDefault(p => !p.Visited);
            _routetolandmark = await Util.FindWalkingRoute(App.Geo.Position.Coordinate.Point, _currentlandmark.Location);
            UpdateRoute(_routetolandmark, _currentlandmark);
        }

        public void StartRoute(Route r)
        {
            Util.SendToastNotification("Starting", "TRANSLATED MESSAGE");

            App.Geo.ClearHistory();
            _currentroute = r;
            UpdateRoute();
            UpdateStatus(RouteStatus.STARTED);
        }

        public void StopRoute()
        {
            if (Status == RouteStatus.STARTED)
            {
                _currentroute.Reset();
                _currentroute = null;
                _currentlandmark = null;
                _routetolandmark = null;
                UpdateStatus(RouteStatus.STOPPED);
            }
        }

        // =============
        // Handle Events
        // =============
        private void UpdateStatus(RouteStatus status)
        {
            Status = status;
            // Make sure someone is listening to event
            if (OnStatusUpdate == null) return;

            OnStatusUpdate(this, new RouteStatusChangedEventArgs(status));
        }

        //Handle Events
        private void LandmarkVisited(Landmark l, LandmarkVisitedEventArgs.VisitedStatus status)
        {
            // Make sure someone is listening to event
            if (OnLandmarkVisited == null) return;

            OnLandmarkVisited(this, new LandmarkVisitedEventArgs(l, status));
        }

        //Handle Events
        private void UpdateRoute(MapRoute route, Landmark l)
        {
            // Make sure someone is listening to event
            if (OnRouteChanged == null) return;

            OnRouteChanged(this, new RouteChangedEventArgs(route, l));
        }
    }

    public class RouteStatusChangedEventArgs : EventArgs
    {
        public RouteManager.RouteStatus Status { get; private set; }

        public RouteStatusChangedEventArgs(RouteManager.RouteStatus status)
        {
            Status = status;
        }
    }

    public class LandmarkVisitedEventArgs : EventArgs
    {
        public Landmark Landmark { get; private set; }
        public enum VisitedStatus { ENTERED, EXITED }
        public VisitedStatus Status { get; private set; }

        public LandmarkVisitedEventArgs(Landmark landmark, VisitedStatus status)
        {
            Landmark = landmark;
            Status = status;
        }
    }

    public class RouteChangedEventArgs : EventArgs
    {
        public Landmark Landmark { get; private set; }
        public MapRoute Route { get; private set; }

        public RouteChangedEventArgs(MapRoute newroute, Landmark tolandmark)
        {
            Route = newroute;
            Landmark = tolandmark;
        }
    }
}
