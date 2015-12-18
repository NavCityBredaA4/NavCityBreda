using NavCityBreda.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls.Maps;

namespace NavCityBreda.Model
{
    public class Landmark : Waypoint
    {
        public string Description { get; private set; }
        public string Image { get; private set; }

        private bool _visited;
        public bool Visited
        {
            get { return _visited; }
            set
            {
                _visited = value;
                UpdateIconImage();
            }
        }

        public string Id { get; private set; }
        public MapIcon Icon { get; set; }


        public Landmark(Geopoint p, string name, string desc, int num, string image_loc = "default.jpg") : base (p, name, num)
        {
            Create(Location, name, desc, num, image_loc);
        }

        public Landmark(double la, double lo, string name, string desc, int num, string image_loc = "default.jpg") : base(la, lo, name, num)
        {
            Create(Location, name, desc, num, image_loc);
        }

        private void Create(Geopoint p, string name, string desc, int num, string image_loc)
        {
            _visited = false;
            Description = desc;
            if (image_loc == "" || !File.Exists(Util.RouteImagesFolder + image_loc))
                image_loc = "default.jpg";
            Image = "/" + Util.RouteImagesFolder + image_loc;
            Id = Name + "_" + Order;

            Icon = new MapIcon();
            Icon.Location = Location;
            Icon.NormalizedAnchorPoint = new Point(0.5, 1.0);
            Icon.Title = Name;
            Icon.ZIndex = 10;
        }

        public void UpdateIconImage()
        {
            if(_visited)
                Icon.Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/LandmarkVisited.png"));
            else
                Icon.Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/LandmarkNotVisited.png"));
        }
    }
}
