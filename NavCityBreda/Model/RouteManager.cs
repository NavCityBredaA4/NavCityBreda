using NavCityBreda.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NavCityBreda.Model
{
    public class RouteManager
    {
        private List<Route> _routes;
        public List<Route> Routes { get { return _routes;  } }

        private Route _currentroute;
        public Route CurrentRoute { get { return _currentroute; } }

        public string LoadingElement;

        public RouteManager()
        {
            _routes = new List<Route>();
            LoadingElement = "Initializing...";
            LoadRoutes();
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
                await Task.Delay(TimeSpan.FromMilliseconds(250));
            }

            LoadingElement = "Done";
        }

        public void SetCurrentRoute(int index)
        {
            _currentroute = _routes.ElementAt(index);
        }

        public void SetCurrentRoute(Route r)
        {
            _currentroute = r;
        }
    }
}
