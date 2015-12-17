using NavCityBreda.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Services.Maps;
using Windows.UI.Xaml.Controls.Maps;

namespace NavCityBreda.Model
{
    public class Route
    {
        public GeoboundingBox Bounds { get { return _route.BoundingBox; } }

        public string Name { get; private set; }
        public string Description { get; private set; }
        public string Landmarks { get; private set; }
        public int Minutes { get; private set; }

        private List<Waypoint> _waypoints;
        public List<Waypoint> Waypoints { get { return _waypoints; } }

        private MapRoute _route;
        public MapRoute RouteObject { get { return _route; } }

        public Route(string name, string desc, string landmarks, int minutes)
        {
            Name = name;
            Description = desc;
            Landmarks = landmarks;
            Minutes = minutes;
            _waypoints = new List<Waypoint>();
        }

        public void Add(Waypoint w)
        {
            _waypoints.Add(w);
        }

        public Waypoint Get(int i)
        {
            return _waypoints.ElementAt(i);
        }

        public Waypoint Get(MapIcon m)
        {
            List<Waypoint> ws = _waypoints.Where(p => p.Equals(m)).ToList();

            if (ws.Count < 1)
                return null;

            return ws.First();
        }

        public void Remove(Waypoint w)
        {
            _waypoints.Remove(w);
        }

        public async Task<String> CalculateRoute()
        {
            _route = await Util.FindWalkingRoute(_waypoints.Select(p => p.Location).ToList());
            return "";
        }
    }
}
