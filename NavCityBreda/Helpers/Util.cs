using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.Devices.Geolocation;
using Windows.Services.Maps;
using Windows.UI;
using Windows.UI.Xaml.Controls.Maps;

namespace NavCityBreda.Helpers
{
    class Util
    {
        public static ResourceLoader Loader 
        {
            get {
               return new Windows.ApplicationModel.Resources.ResourceLoader();
            }
        }

        public static async Task<MapLocation> FindLocation(string location, Geopoint reference)
        {
            MapLocationFinderResult result = await MapLocationFinder.FindLocationsAsync(location, reference);
            MapLocation from = result.Locations.First();
            return from;
        }

        public static async Task<MapRoute> FindWalkingRoute(Geopoint from, Geopoint to)
        {
            MapRouteFinderResult routeResult = await MapRouteFinder.GetWalkingRouteAsync(from, to);
            MapRoute b = routeResult.Route;
            return b;
        }

        public static async Task<MapRoute> FindWalkingRoute(string from, string to, Geopoint reference)
        {
            MapLocation f = await FindLocation(from, reference);
            MapLocation t = await FindLocation(to, reference);
            MapRoute m = await FindWalkingRoute(f.Point, t.Point);
            return m;
        }

        public static async Task<MapRoute> FindDrivingRoute(Geopoint from, Geopoint to)
        {
            MapRouteFinderResult routeResult = await MapRouteFinder.GetDrivingRouteAsync(from, to);
            MapRoute b = routeResult.Route;
            return b;
        }

        public static async Task<MapRoute> FindDrivingRoute(string from, string to, Geopoint reference)
        {
            MapLocation f = await FindLocation(from, reference);
            MapLocation t = await FindLocation(to, reference);
            MapRoute m = await FindDrivingRoute(f.Point, t.Point);
            return m;
        }

        public static MapPolyline GetRouteLine(MapRoute m, Color color, int thickness = 10, bool dashed = false)
        {
            var line = new MapPolyline
            {
                StrokeThickness = thickness,
                StrokeColor = color,
                StrokeDashed = dashed,
                ZIndex = 2
            };

            line.Path = new Geopath(m.Path.Positions);

            return line;
        }
    }
}
