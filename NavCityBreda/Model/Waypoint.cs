using NavCityBreda.Helpers;
using Windows.Devices.Geolocation;
using Windows.UI.Xaml.Controls.Maps;

namespace NavCityBreda.Model
{
    public class Waypoint
    {
        public Geopoint Position { get; protected set; }

        protected string _namekey;
        public string Name { get { return Util.Loader.GetString( _namekey ); } }

        public int Order { get; protected set; }

        public Waypoint(Geopoint p, string name, int num)
        {
            Position = p;
            _namekey = name;
            Order = num;
        }

        public Waypoint(double la, double lo, string name, int num)
        {
            Position = new Geopoint(new BasicGeoposition() { Altitude = 0, Latitude = la, Longitude = lo });
            _namekey = name;
            Order = num;
        }

        public override bool Equals(object obj)
        {
            Waypoint r = obj as Waypoint;
            MapIcon i = obj as MapIcon;

            if (r != null && r.Name == this.Name && r.Position == this.Position)
                return true;

            if (i != null && i.Title == this.Name && i.Location == this.Position)
                return true;

            return false;
        }
    }
}
