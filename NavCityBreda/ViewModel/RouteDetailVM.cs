using NavCityBreda.Helpers;
using NavCityBreda.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace NavCityBreda.ViewModel
{
    public class RouteDetailVM : INotifyPropertyChanged
    {
        Route route;

        public RouteDetailVM(Route r)
        {
            this.route = r;
        }

        public string Description
        {
            get
            {
                return route.Description;
            }
        }

        public string Time
        {
            get
            {
                TimeSpan length = route.RouteObject.EstimatedDuration;
                return length.Hours + "h " + length.Minutes + "m";

                //int hours = route.Minutes / 60;
                //int minutes = route.Minutes % 60;

                //return hours + "h " + minutes + "m";
            }
        }

        public string Distance
        {
            get
            {
                double length = route.RouteObject.LengthInMeters;
                length /= 1000;
                length = Math.Round(length, 2);
                return length + "km";
            }
        }

        public GridLength ScreenWidth
        {
            get
            {
                return new GridLength(ApplicationView.GetForCurrentView().VisibleBounds.Width);
            }
        }

        public List<Landmark> Landmarks
        {
            get
            {
                return route.Waypoints.Where(p => p is Landmark).Cast<Landmark>().ToList();
            }
        }

        public bool StartEnabled
        {
            get
            {
                return App.RouteManager.CurrentRoute != route;
            }
        }

        public bool StopEnabled
        {
            get
            {
                return !StartEnabled;
            }
        }

        public void UpdateRoute()
        {
            NotifyPropertyChanged(nameof(StartEnabled));
            NotifyPropertyChanged(nameof(StopEnabled));
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
