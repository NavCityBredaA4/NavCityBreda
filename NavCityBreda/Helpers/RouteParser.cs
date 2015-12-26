using NavCityBreda.Model;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace NavCityBreda.Helpers
{
    class RouteParser
    {
        public static Route LoadRoute(string foldername)
        {
            if (!Directory.Exists("Routes/" + foldername))
                throw new FileNotFoundException("The route folder does not exist: " + foldername);

            string datafile = "Routes/" + foldername + "/data.json";
            string mediafolder = "Routes/" + foldername + "/media";

            if (!File.Exists(datafile))
                throw new FileNotFoundException("The route data file does not exist.");

            string json = File.ReadAllText(datafile);
            JObject o = JObject.Parse(json);

            string error;
            if (!ValidateRouteObject(o, out error))
                throw new FileLoadException("Invalid Route information in " + datafile + ", " + error);

            Route r = new Route((string)o["name"], (string)o["description"], (string)o["landmarks"], foldername);

            JToken[] waypoints = o["waypoints"].ToArray();
            int count = 0;
            foreach (JToken t in waypoints)
            {
                if (!ValidateWaypointObject(t, out error))
                    throw new FileLoadException("Invalid Waypoint (#" + (count + 1) + ") information in " + datafile + ", " + error);

                Waypoint w;

                if ((bool)t["landmark"])
                {
                    List<Image> images = new List<Image>();

                    if (!t["image"].NullOrEmpty())
                    {
                        JToken img = t["image"];

                        if (img.Type == JTokenType.String)
                        {
                            if (ImageExists(foldername, (string)img))
                                images.Add(new Image(ImagePath(foldername, (string)img)));
                        }
                        else if (img.Type == JTokenType.Array)
                        {
                            foreach (string s in img.ToArray())
                            {
                                if (ImageExists(foldername, s))
                                    images.Add(new Image(ImagePath(foldername, s)));
                            }
                        }
                    }

                    w = new Landmark((double)t["latitude"], (double)t["longitude"], (string)t["name"], count, (string)t["description"], images);
                }
                else
                    w = new Waypoint((double)t["latitude"], (double)t["longitude"], (string)t["name"], count);

                r.Add(w);
                count++;
            }

            return r;
        }

        private static bool ImageExists(string foldername, string imagename)
        {
            return File.Exists("Routes/" + foldername + "/media/" + imagename);
        }

        private static string ImagePath(string foldername, string imagename)
        {
            return "/Routes/" + foldername + "/media/" + imagename;
        }

        private static bool ValidateRouteObject(JObject o, out string error)
        {
            bool valid = true;
            error = "Success";

            if (o["name"].NullOrEmpty())
            {
                valid = false;
                error = "Name missing";
            }

            if (o["description"].NullOrEmpty())
            {
                valid = false;
                error = "Description missing";
            }

            if (o["landmarks"].NullOrEmpty())
            {
                valid = false;
                error = "Landmarks missing";
            }

            if (o["waypoints"].NullOrEmpty())
            {
                valid = false;
                error = "Waypoints missing or empty";
            }

            return valid;
        }

        private static bool ValidateWaypointObject(JToken o, out string error)
        {
            bool valid = true;
            error = "Success";

            if (o["name"].NullOrEmpty())
            {
                valid = false;
                error = "Name missing";
            }

            if (o["landmark"].NullOrEmpty())
            {
                valid = false;
                error = "Landmark missing";
            }

            if (o["latitude"].NullOrEmpty())
            {
                valid = false;
                error = "Latitude missing";
            }

            if (o["longitude"].NullOrEmpty())
            {
                valid = false;
                error = "Longitude missing";
            }

            if (valid && (bool)o["landmark"])
            {
                if (o["description"].NullOrEmpty())
                {
                    valid = false;
                    error = "Description missing";
                }
            }

            return valid;
        }
    }
}
