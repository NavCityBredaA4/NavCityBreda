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

        public RouteManager()
        {
            _routes = new List<Route>();
            LoadRoutes();
        }

        private void LoadRoutes()
        {
            string[] routefiles = Directory.GetFiles(Util.RouteWaypointsFolder);

            _routes.Clear();

            foreach(string file in routefiles)
            {
                _routes.Add(JSONParser.LoadRoute(file));
            }
        }
    }
}
