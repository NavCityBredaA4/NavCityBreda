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
    public class MenuVM : INotifyPropertyChanged
    {
        CoreDispatcher dispatcher;

        public MenuVM()
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
                NotifyPropertyChanged(nameof(Status));
            });
            
        }

        private void Geo_PositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                NotifyPropertyChanged(nameof(Source));
                NotifyPropertyChanged(nameof(Accuracy));
            });
        }

        public string Status
        {
            get
            {
                return App.Geo.Status;
            }
        }

        public string Source
        {
            get
            {
                if (App.Geo.Connected == true && App.Geo.Position != null)
                    return App.Geo.Position.Coordinate.PositionSource.ToString();
                else
                    return "N/A";
            }
        }

        public string Accuracy
        {
            get
            {
                
                if (App.Geo.Connected == true && App.Geo.Position != null)
                    return App.Geo.Position.Coordinate.Accuracy.ToString() + "m";
                else
                    return "0m";
            }
        }

        public string Year
        {
            get
            {
                int year = DateTime.Now.Year;
                return year.ToString();
            }
        }
    }
}
