using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Devices.Geolocation.Geofencing;
using Windows.Foundation;
using Windows.System;

namespace NavCityBreda.Model
{
    public class GeoTracker
    {
        private Geolocator geo;

        private string status;
        public string Status { get { return status; } }

        private Geoposition pos;
        public Geoposition Position { get { return pos; } }

        public bool? Connected { get; private set; }

        public GeoTracker()
        {
            status = "Waiting...";
            Connected = false;
            StartTracking();
        }

        public event TypedEventHandler<Geolocator, StatusChangedEventArgs> StatusChanged
        {
            add { if(Connected == true) geo.StatusChanged += value; }
            remove { geo.StatusChanged -= value; }
        }

        public event TypedEventHandler<Geolocator, PositionChangedEventArgs> PositionChanged
        {
            add { if (Connected == true) geo.PositionChanged += value; }
            remove { geo.PositionChanged -= value; }
        }

        public async void ForceRefresh()
        {
            if(geo != null)
                pos = await geo.GetGeopositionAsync();
        }

        private async void StartTracking()
        {
            // Request permission to access location
            var accessStatus = await Geolocator.RequestAccessAsync();

            switch (accessStatus)
            {
                case GeolocationAccessStatus.Allowed:
                    geo = new Geolocator {
                        DesiredAccuracy = PositionAccuracy.High,
                        //MovementThreshold = 5
                        ReportInterval = 1500
                    };

                    Connected = true;

                    geo.PositionChanged += Geo_PositionChanged;
                    geo.StatusChanged += Geo_StatusChanged;

                    status = "Waiting for update...";
                    break;

                case GeolocationAccessStatus.Denied:
                    Connected = false;
                    status = "Access Denied";
                    bool result = await Launcher.LaunchUriAsync(new Uri("ms-settings:privacy-location"));
                    break;

                default:
                case GeolocationAccessStatus.Unspecified:
                    Connected = false;
                    status = "Unspecified Error Occured";
                    break;
            }
        }

        private void Geo_StatusChanged(Geolocator sender, StatusChangedEventArgs args)
        {
            status = args.Status.ToString();
        }

        private void Geo_PositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            pos = args.Position;
        }
    }
}
