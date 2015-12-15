using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.UI.Core;

namespace NavCityBreda.ViewModel
{
    class MapVM : INotifyPropertyChanged
    {
        CoreDispatcher dispatcher;

        public MapVM()
        {
            dispatcher = Windows.UI.Core.CoreWindow.GetForCurrentThread().Dispatcher;
            App.Geo.StatusChanged += Geo_StatusChanged;
            App.Geo.PositionChanged += Geo_PositionChanged;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Geo_StatusChanged(Geolocator sender, StatusChangedEventArgs args)
        {
            dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {

            });

        }

        private void Geo_PositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {

            });
        }
    }
}
