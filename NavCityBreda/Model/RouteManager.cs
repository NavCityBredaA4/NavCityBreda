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
            LoadingElement = "Initializing...";
            RouteState = State.STOPPED;
            LoadRoutes();
            App.Geo.PositionChanged += Geo_PositionChanged;
        }

        private void Geo_PositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            if (RouteState == State.STARTED)
                _history.Add(args.Position);
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

        public void StartRoute(Route r)
        {
            _currentroute = r;
            RouteState = State.STARTED;
        }

        public void StopRoute()
        {
            if (RouteState == State.STARTED)
            {
                _currentroute = null;
                RouteState = State.STOPPED;
            }

            _history.Clear();
        }
    }
}
