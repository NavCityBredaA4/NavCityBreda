using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace NavCityBreda.Model
{
    public class Waypoint
    {
        public Geopoint Location { get; protected set; }
        public string Name { get; protected set; }

        public Waypoint(Geopoint p, string name)
        {
            Location = p;
            Name = name;
        }

        public Waypoint(double la, double lo, string name)
        {
            Location = new Geopoint(new BasicGeoposition() { Altitude = 0, Latitude = la, Longitude = lo });
            Name = name;
        }
    }
}
