using NavCityBreda.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NavCityBreda.ViewModel
{
    class RouteVM
    {
        private List<Route> _routes;

        public RouteVM()
        {
            _routes = App.RouteManager.Routes;
        }

        public List<Route> GetRoutes()
        {
            return _routes;
        }
    }
}
