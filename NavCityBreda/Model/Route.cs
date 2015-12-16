using NavCityBreda.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace NavCityBreda.Model
{
    public class Route
    {
        private GeoboundingBox _bounds;
        public GeoboundingBox Bounds { get { if (_bounds == null) CalculateBounds(); return _bounds; } }

        public string Name { get; private set; }
        public string Description { get; private set; }
        public string Landmarks { get; private set; }
        public int Minutes { get; private set; }

        private List<Waypoint> _waypoints;
        public List<Waypoint> Waypoints { get { return _waypoints; } }

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

        public void Remove(Waypoint w)
        {
            _waypoints.Remove(w);
        }

        private void CalculateBounds()
        {
            _bounds = BoundingBox.GetBoundingBox(_waypoints.Select(p => p.Location).ToList());
        }
    }
}
