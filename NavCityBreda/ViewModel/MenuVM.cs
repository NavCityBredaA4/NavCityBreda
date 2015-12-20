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
            dispatcher = App.Dispatcher;
            App.Geo.OnPositionUpdate += Geo_OnPositionUpdate;
            App.Geo.OnStatusUpdate += Geo_OnStatusUpdate;
        }

        private void Geo_OnStatusUpdate(object sender, Model.StatusUpdatedEventArgs e)
        {
            dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                NotifyPropertyChanged(nameof(Status));
            });
        }

        private void Geo_OnPositionUpdate(object sender, Model.PositionUpdatedEventArgs e)
        {
            dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                NotifyPropertyChanged(nameof(Source));
                NotifyPropertyChanged(nameof(Accuracy));
            });
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string Status
        {
            get
            {
                return App.Geo.Status.ToString();
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
