using NavCityBreda.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Services.Maps;
using Windows.UI.Xaml.Controls.Maps;

namespace NavCityBreda.Model
{
    public class Route
    {
        public GeoboundingBox Bounds { get { return _route.BoundingBox; } }

        private string _namekey;
        public string Name { get { return Util.Loader.GetString( _namekey ); } }

        private string _desckey;
        public string Description { get { return Util.Loader.GetString( _desckey ); } }

        private string _landdesckey;
        public string LandmarksDescription { get { return Util.Loader.GetString( _landdesckey ); } }

        private string foldername { get; set; }

        public List<Landmark> Landmarks
        {
            get
            {
                return _waypoints.Where(l => l is Landmark).Cast<Landmark>().ToList();
            }
        }

        private List<Waypoint> _waypoints;
        public List<Waypoint> Waypoints { get { return _waypoints; } }

        private MapRoute _route;
        public MapRoute RouteObject { get { return _route; } }

        public Route(string name, string desc, string landmarks, string folder)
        {
            _namekey = name;
            _desckey = desc;
            _landdesckey = landmarks;
            foldername = folder;
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
            _route = await Util.FindWalkingRoute(_waypoints.Select(p => p.Position).ToList());
            return "success";
        }

        public async Task<String> Reset()
        {
            foreach(Landmark l in Landmarks)
            {
                l.Visited = false;
                await Task.Delay(TimeSpan.FromMilliseconds(2));
            }
            return "success";
        }
    }
}
