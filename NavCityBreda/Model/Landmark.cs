using NavCityBreda.Helpers;
using System;
using System.Collections.Generic;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls.Maps;

namespace NavCityBreda.Model
{
    public class Landmark : Waypoint
    {
        private string _desckey;
        public string Description { get { return Util.Loader.GetString(_desckey); } }

        public List<Image> Images { get; private set; }

        public enum LandmarkStatus { NOTVISITED, VISITED, SKIPPED }

        private LandmarkStatus _status;
        public LandmarkStatus Status
        {
            get { return _status; }
            set
            {
                _status = value;
                UpdateIcon();
            }
        }

        public string Id { get; private set; }
        public MapIcon Icon { get; set; }


        public Landmark(Geopoint p, string name, int num, string desc, List<Image> images) : base (p, name, num)
        {
            Create(name, desc, num, images);
        }

        public Landmark(double la, double lo, string name, int num, string desc, List<Image> images) : base(la, lo, name, num)
        {
            Create(name, desc, num, images);
        }

        private void Create(string name, string desc, int num, List<Image> images)
        {
            _status = LandmarkStatus.NOTVISITED;
            _desckey = desc;

            Images = images;

            Id = _namekey + "_" + Order;

            Icon = new MapIcon();
            Icon.Location = Position;
            Icon.NormalizedAnchorPoint = new Point(0.5, 1.0);
            Icon.Title = Name;
            Icon.ZIndex = 500;
        }

        public void UpdateIcon()
        {
            Icon.Title = Name;

            switch(_status)
            {
                default:
                case LandmarkStatus.NOTVISITED:
                    Icon.Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/LandmarkNotVisited.png"));
                    break;
                case LandmarkStatus.VISITED:
                    Icon.Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/LandmarkVisited.png"));
                    break;
                case LandmarkStatus.SKIPPED:
                    Icon.Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/LandmarkSkipped.png"));
                    break;
            }
        }
    }
}
