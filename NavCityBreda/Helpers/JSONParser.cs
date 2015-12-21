using NavCityBreda.Model;
using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NavCityBreda.Helpers
{
    class JSONParser
    {
        public static Route LoadRoute(string foldername)
        {
            if (!Directory.Exists("Routes/" + foldername))
                throw new FileNotFoundException("The route folder does not exist: " + foldername);

            string datafile = "Routes/" + foldername + "/data.json";

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
            foreach(JToken t in waypoints)
            {
                if(!ValidateWaypointObject(t, out error))
                    throw new FileLoadException("Invalid Waypoint information in " + datafile + ", " + error);

                Waypoint w;

                if ((bool)t["landmark"])
                    w = new Landmark((double)t["latitude"], (double)t["longitude"], (string)t["name"], (string)t["description"], count, t["image"].NullOrEmpty() ? "" : (string)t["image"]);
                else
                    w = new Waypoint((double)t["latitude"], (double)t["longitude"], (string)t["name"], count);

                r.Add(w);
                count++;
            }

            return r;
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

            if( valid && (bool)o["landmark"])
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
