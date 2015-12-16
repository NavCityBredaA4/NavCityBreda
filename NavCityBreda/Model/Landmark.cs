using NavCityBreda.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace NavCityBreda.Model
{
    public class Landmark : Waypoint
    {
        public string Description { get; private set; }
        public string Image { get; private set; }
        public bool Visited { get; set; }


        public Landmark(Geopoint p, string name, string desc, string image_loc = "default.jpg") : base (p, name)
        {
            Visited = false;
            Description = desc;
            if (image_loc == "" || !File.Exists(Util.RouteImagesFolder + image_loc))
                image_loc = "default.jpg";
            Image = Util.RouteImagesFolder + image_loc;
        }

        public Landmark(double la, double lo, string name, string desc, string image_loc = "default.jpg") : base(la, lo, name)
        {
            Visited = false;
            Description = desc;
            if (image_loc == "" || !File.Exists(Util.RouteImagesFolder + image_loc))
                image_loc = "default.jpg";
            Image = Util.RouteImagesFolder + image_loc;
        }
    }
}
