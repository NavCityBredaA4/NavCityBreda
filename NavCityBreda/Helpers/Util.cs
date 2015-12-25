using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.Data.Xml.Dom;
using Windows.Devices.Geolocation;
using Windows.Services.Maps;
using Windows.UI;
using Windows.UI.Notifications;
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

        public static double Now { get { return (DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds; } }

        public static string MillisecondsToTime(double millis)
        {
            DateTime time = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            string timestr = time.AddMilliseconds(millis) + "";
            return timestr;
        }

        public static async Task<Geopoint> FindLocation(string location, Geopoint reference)
        {
            MapLocationFinderResult result = await MapLocationFinder.FindLocationsAsync(location, reference);
            MapLocation from = result.Locations.FirstOrDefault();
            Geopoint p = from.Point;
            return p;
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
            Geopoint f = await FindLocation(from, reference);
            Geopoint t = await FindLocation(to, reference);
            MapRoute m = await FindWalkingRoute(f, t);
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
            Geopoint f = await FindLocation(from, reference);
            Geopoint t = await FindLocation(to, reference);
            MapRoute m = await FindDrivingRoute(f, t);
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

        public static MapPolyline GetRouteLine(MapRoute m, Color color, int zindex, int thickness = 5)
        {
            var line = new MapPolyline
            {
                StrokeThickness = thickness,
                StrokeColor = color,
                StrokeDashed = false,
                ZIndex = zindex
            };

            if (m != null)
                line.Path = new Geopath(m.Path.Positions);

            return line;
        }

        public static MapPolyline GetRouteLine(List<BasicGeoposition> positions, Color color, int zindex, int thickness = 5)
        {
            var line = new MapPolyline
            {
                StrokeThickness = thickness,
                StrokeColor = color,
                StrokeDashed = false,
                ZIndex = zindex
            };

            line.Path = new Geopath(positions);

            return line;
        }

        public static MapPolyline GetRouteLine(BasicGeoposition p1, BasicGeoposition p2, Color color, int zindex, int thickness = 5)
        {
            var line = new MapPolyline
            {
                StrokeThickness = thickness,
                StrokeColor = color,
                StrokeDashed = false,
                ZIndex = zindex
            };

            List<BasicGeoposition> plist = new List<BasicGeoposition>();
            plist.Add(p1);
            plist.Add(p2);

            line.Path = new Geopath(plist);

            return line;
        }

        public static void SendToastNotification(string title, string text)
        {
            ToastTemplateType toastTemplate = ToastTemplateType.ToastText02;
            XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(toastTemplate);

            XmlNodeList toastTextElements = toastXml.GetElementsByTagName("text");
            toastTextElements[0].AppendChild(toastXml.CreateTextNode(title));
            toastTextElements[1].AppendChild(toastXml.CreateTextNode(text));

            IXmlNode toastNode = toastXml.SelectSingleNode("/toast");
            XmlElement audio = toastXml.CreateElement("audio");

            audio.SetAttribute("src", "ms-winsoundevent:Notification.IM");

            toastNode.AppendChild(audio);

            ToastNotification toast = new ToastNotification(toastXml);
            ToastNotificationManager.CreateToastNotifier().Show(toast);
        }

        public static string TranslatedManeuver(MapRouteManeuver maneuver)
        {
            string response = "";
            bool onstreet = false;

            switch(maneuver.Kind)
            {
                default:
                    response = Util.Loader.GetString("RouteSeeMap");
                    break;
                case MapRouteManeuverKind.End:
                    response = Util.Loader.GetString("RouteEnd");
                    break;
                case MapRouteManeuverKind.GoStraight:
                    response = Util.Loader.GetString("RouteGoStraight");
                    onstreet = true;
                    break;
                case MapRouteManeuverKind.None:
                    response = Util.Loader.GetString("RouteNone");
                    break;
                case MapRouteManeuverKind.Start:
                    response = Util.Loader.GetString("RouteStart");
                    break;
                case MapRouteManeuverKind.TurnHardLeft:
                case MapRouteManeuverKind.TurnLeft:
                    response = Util.Loader.GetString("RouteLeft");
                    onstreet = true;
                    break;
                case MapRouteManeuverKind.TurnHardRight:
                case MapRouteManeuverKind.TurnRight:
                    response = Util.Loader.GetString("RouteRight");
                    onstreet = true;
                    break;
                case MapRouteManeuverKind.TrafficCircleLeft:
                    response = Util.Loader.GetString("RouteTrafficCircleLeft");
                    onstreet = true;
                    break;
                case MapRouteManeuverKind.TrafficCircleRight:
                    response = Util.Loader.GetString("RouteTrafficCircleRight");
                    onstreet = true;
                    break;
                case MapRouteManeuverKind.TurnKeepLeft:
                case MapRouteManeuverKind.TurnLightLeft:
                    response = Util.Loader.GetString("RouteKeepLeft");
                    break;
                case MapRouteManeuverKind.TurnKeepRight:
                case MapRouteManeuverKind.TurnLightRight:
                    response = Util.Loader.GetString("RouteKeepRight");
                    break;
                case MapRouteManeuverKind.UTurnLeft:
                case MapRouteManeuverKind.UTurnRight:
                    response = Util.Loader.GetString("RouteUTurn");
                    break;
            }

            if (maneuver.StreetName == "")
                onstreet = false;

            if (onstreet)
               response += " " + Util.Loader.GetString("RouteOn") + " " + maneuver.StreetName;

            return response;
        }
    }
}
