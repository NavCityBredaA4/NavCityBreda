using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Graphics.Display;
using Windows.Services.Maps;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
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

        public static string RouteWaypointsFolder
        {
            get
            {
                return "Routes/Waypoints/";
            }
        }

        public static string RouteImagesFolder
        {
            get
            {
                return "Routes/Images/";
            }
        }

        public static Size ScreenSize
        {
            get
            {
                var bounds = ApplicationView.GetForCurrentView().VisibleBounds;
                var scaleFactor = DisplayInformation.GetForCurrentView().RawPixelsPerViewPixel;
                Size size = new Size(bounds.Width * scaleFactor, bounds.Height * scaleFactor);
                return size;
            }
        }

        public static MainPage MainPage
        {
            get {
                Frame f = Window.Current.Content as Frame;
                MainPage mp = f.Content as MainPage;
                return mp;
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

        public static async Task<MapRoute> FindWalkingRoute(List<Geopoint> points)
        {
            MapRouteFinderResult routeResult = await MapRouteFinder.GetWalkingRouteFromWaypointsAsync(points);
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

        public static async Task<MapRoute> FindDrivingRoute(List<Geopoint> points)
        {
            MapRouteFinderResult routeResult = await MapRouteFinder.GetDrivingRouteFromWaypointsAsync(points);
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

        public static async Task<String> FindAddress(Geopoint p)
        {
            // Reverse geocode the specified geographic location.
            MapLocationFinderResult result =
                await MapLocationFinder.FindLocationsAtAsync(p);

            string returnstring = "";

            // If the query returns results, display the name of the town
            // contained in the address of the first result.
            if (result.Status == MapLocationFinderStatus.Success)
            {
                MapAddress address = result.Locations[0].Address;

                //returnstring = address.Street + " " + address.StreetNumber + ", " + address.Town;
                returnstring += (address.BuildingName == "" ? "" : address.BuildingName + ", ");
                returnstring += (address.Street == "" ? "" : address.Street + (address.StreetNumber == "" ? ", " : " " + address.StreetNumber + ", "));
                returnstring += address.Town;
            }

            return returnstring;
        }

        public static async Task<String> FindAddress(double latitude, double longitude)
        {
            Geopoint p = new Geopoint(new BasicGeoposition() { Latitude = latitude, Longitude = longitude });
            string address = await FindAddress(p);
            return address;
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

            if (m != null)
                line.Path = new Geopath(m.Path.Positions);

            return line;
        }
    }
}
