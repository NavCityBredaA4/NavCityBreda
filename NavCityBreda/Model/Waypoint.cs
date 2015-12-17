using NavCityBreda.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.UI.Xaml.Controls.Maps;

namespace NavCityBreda.Model
{
    public class Waypoint
    {
        public Geopoint Location { get; protected set; }
        public string Name { get; protected set; }
        public int Order { get; protected set; }

        public Waypoint(Geopoint p, string name, int num)
        {
            Location = p;
            Name = name;
            Order = num;
        }

        public Waypoint(double la, double lo, string name, int num)
        {
            Location = new Geopoint(new BasicGeoposition() { Altitude = 0, Latitude = la, Longitude = lo });
            Name = name;
            Order = num;
        }

        public override bool Equals(object obj)
        {
            Waypoint r = obj as Waypoint;
            MapIcon i = obj as MapIcon;

            if (r != null && r.Name == this.Name && r.Location == this.Location)
                return true;

            if (i != null && i.Title == this.Name && i.Location == this.Location)
                return true;

            return false;
        }
    }
}
