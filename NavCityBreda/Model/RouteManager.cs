﻿using NavCityBreda.Helpers;
using NavCityBreda.Model.Object;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        public delegate void OnLandmarkChangedHandler(object sender, LandmarkChangedEventArgs e);
        public event OnLandmarkChangedHandler OnLandmarkChanged;

        public delegate void OnManeuverChangedHandler(object sender, ManeuverChangedEventArgs e);
        public event OnManeuverChangedHandler OnManeuverChanged;



        private List<Route> _routes;
        public List<Route> Routes { get { return _routes; } }

        private Route _currentroute;
        public Route CurrentRoute { get { return _currentroute; } }

        private Landmark _currentlandmark;
        public Landmark CurrentLandmark { get { return _currentlandmark; } }

        private MapRoute _routetolandmark;
        public MapRoute RouteToLandmark { get { return _routetolandmark; } }

        private int _currentroutelegcount;
        private MapRouteLeg _currentrouteleg { get { return _currentroutelegs[_currentroutelegcount]; } }
        private List<MapRouteLeg> _currentroutelegs;

        private int _currentmaneuvercount;
        private MapRouteManeuver _currentmaneuver;
        private List<MapRouteManeuver> _currentmaneuvers { get { return _currentrouteleg.Maneuvers.ToList(); } }
        public MapRouteManeuver CurrentManeuver { get { return _currentmaneuver; } }
        public int DistanceToManeuver { get; private set; }


        public string LoadingElement;

        public enum RouteStatus { LOADING, STOPPED, STARTED }
        public RouteStatus Status;

        CoreDispatcher dispatcher;

        public RouteManager()
        {
            _routes = new List<Route>();
            dispatcher = App.Dispatcher;

            LoadingElement = Util.Loader.GetString("Initializing") + "...";
            Status = RouteStatus.LOADING;
            DistanceToManeuver = -1;
            GeofenceMonitor.Current.GeofenceStateChanged += Current_GeofenceStateChanged;
            App.Geo.OnPositionUpdate += Geo_OnPositionUpdate;
            LoadRoutes();
        }

        private void Geo_OnPositionUpdate(object sender, PositionUpdatedEventArgs e)
        {
            if (Status == RouteStatus.STARTED)
            {
                bool found = false;

                for (int q = _currentroutelegcount; q < _currentroutelegs.Count; q++)
                {
                    int startindex = 0;
                    if (q == _currentroutelegcount)
                        startindex = _currentmaneuvercount;

                    for (int i = startindex; i < _currentroutelegs[q].Maneuvers.Count; i++)
                    {
                        double dif_lat = Math.Abs(_currentroutelegs[q].Maneuvers[i].StartingPoint.Position.Latitude - e.New.Coordinate.Point.Position.Latitude);
                        double dif_lon = Math.Abs(_currentroutelegs[q].Maneuvers[i].StartingPoint.Position.Longitude - e.New.Coordinate.Point.Position.Longitude);

                        if (dif_lat < 0.000025 || dif_lon < 0.000025)
                        {
                            _currentroutelegcount = q;
                            _currentmaneuvercount = i + 1;
                            found = true;
                            break;
                        }
                    }
                }

                if (found)
                {
                    if (_currentmaneuvercount >= _currentmaneuvers.Count)
                    {
                        if (_currentroutelegcount + 1 < _currentroutelegs.Count)
                        {
                            _currentroutelegcount++;
                            _currentmaneuvercount = 0;
                        }
                        else
                        {
                            _currentmaneuvercount--;
                        }
                    }

                    if (_currentmaneuver != _currentmaneuvers[_currentmaneuvercount])
                    {
                        _currentmaneuver = _currentmaneuvers[_currentmaneuvercount];
                    }
                }

                UpdateManeuver(_currentmaneuver);
            }
        }

        private void Current_GeofenceStateChanged(GeofenceMonitor sender, object args)
        {
            if (Status != RouteStatus.STARTED) return;

            var reports = sender.ReadReports();

            foreach (GeofenceStateChangeReport report in reports)
            {
                GeofenceState state = report.NewState;
                Geofence geofence = report.Geofence;
                Landmark i = CurrentRoute.Landmarks.Where(t => t.Id == geofence.Id).FirstOrDefault();

                if (state == GeofenceState.Removed)
                {
                    GeofenceMonitor.Current.Geofences.Remove(geofence);
                }

                else if (state == GeofenceState.Entered)
                {
                    dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        i.Status = Landmark.LandmarkStatus.VISITED;
                    });
                    _currentlandmark = i;
                    LandmarkVisited(i, LandmarkVisitedEventArgs.VisitedStatus.ENTERED);
                    Util.ShowToastNotification(i.Name, Util.Loader.GetString("LandmarkReached"));
                    UpdateRoute();
                }

                else if (state == GeofenceState.Exited)
                {
                    LandmarkVisited(i, LandmarkVisitedEventArgs.VisitedStatus.EXITED);
                }
            }
        }

        private async void LoadRoutes()
        {
            string[] routefolders = Directory.GetDirectories("Routes/");

            _routes.Clear();

            foreach (string folder in routefolders)
            {
                string foldername = Path.GetFileName(folder);
                if (foldername != "img")
                {
                    Route r = RouteParser.LoadRoute(foldername);
                    LoadingElement = Util.Loader.GetString("Loading") + " " + r.Name.ToLower() + "...";
                    await r.CalculateRoute();
                    _routes.Add(r);
                }
            }

            LoadingElement = Util.Loader.GetString("Done") + "...";
            Status = RouteStatus.STOPPED;
        }

        public async Task<String> UpdateRoute()
        {
            //Can't navigate without current position
            if (App.Geo.Position == null) return "error";

            //Stop route if end is reached
            _currentlandmark = _currentroute.Landmarks.FirstOrDefault(p => p.Status == Landmark.LandmarkStatus.NOTVISITED);
            if (_currentlandmark == null)
            {
                StopRoute();
                return "stopped";
            }

            //Create route from last landmark trough all the waypoints
            List<Geopoint> _waypoints = new List<Geopoint>();
            _waypoints.Add(App.Geo.Position.Coordinate.Point);

            Landmark lastlandmark = _currentroute.Landmarks.ElementAtOrDefault(_currentroute.Landmarks.IndexOf(_currentlandmark) - 1);

            if (lastlandmark != null)
            {
                int start = _currentroute.Waypoints.IndexOf(lastlandmark) + 1;
                int offset = _currentroute.Waypoints.IndexOf(_currentlandmark) - start;
                List<Geopoint> temp = _currentroute.Waypoints.GetRange(start, offset).Select(e => e.Position).ToList();
                _waypoints.AddRange(temp);
            }
            _waypoints.Add(_currentlandmark.Position);


            //Update all other vars to match
            //_routetolandmark = await Util.FindWalkingRoute(App.Geo.Position.Coordinate.Point, _currentlandmark.Position);
            _routetolandmark = await Util.FindWalkingRoute(_waypoints);
            _currentroutelegs = _routetolandmark.Legs.ToList() as List<MapRouteLeg>;
            _currentroutelegcount = 0;
            _currentmaneuvercount = 0;
            _currentmaneuver = _currentmaneuvers[_currentmaneuvercount];


            //Send out events
            UpdateRoute(_routetolandmark, _currentlandmark);
            UpdateManeuver(_currentmaneuver);

            return "success";
        }

        public async Task<String> SkipLandmark()
        {
            if (Status != RouteStatus.STARTED) return "error";

            await dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                _currentlandmark.Status = Landmark.LandmarkStatus.SKIPPED;
            });
            string s = await UpdateRoute();
            return s;
        }

        public async void StartRoute(Route r)
        {
            App.Geo.ClearHistory();
            _currentroute = r;
            await UpdateRoute();
            UpdateStatus(RouteStatus.STARTED);
        }

        public void StopRoute()
        {
            if (Status == RouteStatus.STARTED)
            {
                _currentroute = null;
                _currentlandmark = null;
                _routetolandmark = null;
                _currentroutelegcount = 0;
                _currentroutelegs = null;
                _currentmaneuvercount = 0;
                _currentmaneuver = null;
                UpdateStatus(RouteStatus.STOPPED);
            }
        }

        public List<Landmark> GetAllLandmarks()
        {
            List<Landmark> l = new List<Landmark>();

            foreach (Route r in _routes)
            {
                l.AddRange(r.Landmarks);
            }

            return l;
        }

        private async Task<int> DistanceToCurrentManeuver()
        {
            if (_currentmaneuver == null) return -1;
            if (App.Geo.Position == null) return -1;

            MapRoute route = await Util.FindWalkingRoute(App.Geo.Position.Coordinate.Point, _currentmaneuver.StartingPoint);

            return (int)route.LengthInMeters;
        }

        public async Task<String> Reset()
        {
            foreach (Route r in _routes)
            {
                await r.Reset();
            }
            return "success";
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

        private void LandmarkVisited(Landmark l, LandmarkVisitedEventArgs.VisitedStatus status)
        {
            // Make sure someone is listening to event
            if (OnLandmarkVisited == null) return;

            OnLandmarkVisited(this, new LandmarkVisitedEventArgs(l, status));
        }

        private void UpdateRoute(MapRoute route, Landmark l)
        {
            // Make sure someone is listening to event
            if (OnLandmarkChanged == null) return;

            OnLandmarkChanged(this, new LandmarkChangedEventArgs(route, l));
        }

        private async void UpdateManeuver(MapRouteManeuver curman)
        {
            DistanceToManeuver = await DistanceToCurrentManeuver();

            // Make sure someone is listening to event
            if (OnManeuverChanged == null) return;

            OnManeuverChanged(this, new ManeuverChangedEventArgs(curman));
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

    public class LandmarkChangedEventArgs : EventArgs
    {
        public Landmark Landmark { get; private set; }
        public MapRoute Route { get; private set; }

        public LandmarkChangedEventArgs(MapRoute newroute, Landmark tolandmark)
        {
            Route = newroute;
            Landmark = tolandmark;
        }
    }

    public class ManeuverChangedEventArgs
    {
        public MapRouteManeuver Maneuver { get; private set; }

        public ManeuverChangedEventArgs(MapRouteManeuver curman)
        {
            Maneuver = curman;
        }
    }
}
