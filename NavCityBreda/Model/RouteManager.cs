using NavCityBreda.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace NavCityBreda.Model
{
    public class RouteManager
    {
        public delegate void StatusUpdateHandler(object sender, RouteStatusChangedEventArgs e);
        public event StatusUpdateHandler OnStatusUpdate;

        public delegate void PositionUpdateHandler(object sender, RoutePositionChangedEventArgs e);
        public event PositionUpdateHandler OnPositionUpdate;

        private List<Route> _routes;
        public List<Route> Routes { get { return _routes;  } }

        private Route _currentroute;
        public Route CurrentRoute { get { return _currentroute; } }

        public string LoadingElement;

        public enum State { STOPPED, STARTED }
        public State RouteState;

        private List<Geoposition> _history;
        public List<Geoposition> History
        {
            get
            {
                return _history;
            }
        }

        public RouteManager()
        {
            _routes = new List<Route>();
            _history = new List<Geoposition>();
            LoadingElement = "Initializing...";
            App.Geo.PositionChanged += Geo_PositionChanged;
            RouteState = State.STOPPED;
            LoadRoutes();  
        }

        private void Geo_PositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            if (RouteState == State.STARTED)
            {
                OnPositionUpdate(this, new RoutePositionChangedEventArgs(_history.Last(), args.Position));
                _history.Add(args.Position);
            }
        }

        private async void LoadRoutes()
        {
            string[] routefiles = Directory.GetFiles(Util.RouteWaypointsFolder);

            _routes.Clear();

            foreach(string file in routefiles)
            {
                Route r = JSONParser.LoadRoute(file);
                LoadingElement = "Loading " + r.Name + "...";
                await r.CalculateRoute();
                _routes.Add(r);
            }

            LoadingElement = "Done";
        }

        private void UpdateStatus(State status)
        {
            RouteState = status;
            // Make sure someone is listening to event
            if (OnStatusUpdate == null) return;

            OnStatusUpdate(this, new RouteStatusChangedEventArgs(status));
        }

        public void StartRoute(Route r)
        {
            _currentroute = r;
            _history.Add(App.Geo.Position);
            UpdateStatus(State.STARTED);
        }

        public void StopRoute()
        {
            if (RouteState == State.STARTED)
            {
                _currentroute.Reset();
                _currentroute = null;
                UpdateStatus(State.STOPPED);
            }

            _history.Clear();
        }
    }

    public class RouteStatusChangedEventArgs : EventArgs
    {
        public RouteManager.State Status { get; private set; }

        public RouteStatusChangedEventArgs(RouteManager.State status)
        {
            Status = status;
        }
    }

    public class RoutePositionChangedEventArgs : EventArgs
    {
        public Geoposition Old { get; private set; }
        public Geoposition New { get; private set; }

        public RoutePositionChangedEventArgs(Geoposition old, Geoposition notold)
        {
            Old = old;
            New = notold;
        }
    }
}
