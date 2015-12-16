﻿using NavCityBreda.Model;
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
        public static Route LoadRoute(string file)
        {
            if (!File.Exists(file))
                throw new FileNotFoundException("The file does not exist: " + file);

            string json = File.ReadAllText(file);
            JObject o = JObject.Parse(json);

            string error;
            if (!ValidateRouteObject(o, out error))
                throw new FileLoadException("Invalid Route information in " + file + ", " + error);

            Route r = new Route((string)o["name"], (string)o["description"], (string)o["landmarks"], Int32.Parse((string)o["minutes"]));

            JToken[] waypoints = o["waypoints"].ToArray();
            foreach(JToken t in waypoints)
            {
                if(!ValidateWaypointObject(t, out error))
                    throw new FileLoadException("Invalid Waypoint information in " + file + ", " + error);

                Waypoint w;

                if ((bool)t["landmark"])
                    w = new Landmark((double)t["latitude"], (double)t["longitude"], (string)t["name"], (string)o["description"], t["image"].NullOrEmpty() ? "" : (string)t["image"]);
                else
                    w = new Waypoint((double)t["latitude"], (double)t["longitude"], (string)t["name"]);

                r.Add(w);
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

            if (o["minutes"].NullOrEmpty())
            {
                valid = false;
                error = "Minutes missing";
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