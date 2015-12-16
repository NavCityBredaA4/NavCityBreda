using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace NavCityBreda.Model
{
    class Waypoint
    {
        Geopoint location;

        public Waypoint(Geopoint p)
        {
            location = p;
        }

        public Waypoint(int a, int la, int lo)
        {
            location = new Geopoint(new BasicGeoposition() { Altitude = a, Latitude = la, Longitude = lo });
        }
    }
}
