using NavCityBreda.Helpers;
using NavCityBreda.Model.Object;
using System;
using System.Collections.Generic;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace NavCityBreda.ViewModels
{
    public class RouteDetailVM : TemplateVM
    {
        Route route;

        public RouteDetailVM(Route r) : base(r.Name)
        {
            this.route = r;
        }

        protected override void UpdatePropertiesToNewLanguage()
        {
            NotifyPropertyChanged(nameof(Description));
            NotifyPropertyChanged(nameof(DescriptionTitle));
            NotifyPropertyChanged(nameof(LandmarksTitle));
            NotifyPropertyChanged(nameof(StartRouteText));
            NotifyPropertyChanged(nameof(StopRouteText));
        }

        public string Description
        {
            get
            {
                return route.Description;
            }
        }

        public string DescriptionTitle
        {
            get
            {
                return Util.Loader.GetString("Description");
            }
        }

        public string LandmarksTitle
        {
            get
            {
                return Util.Loader.GetString("Landmarks");
            }
        }

        public string Time
        {
            get
            {
                TimeSpan length = route.RouteObject.EstimatedDuration;
                return length.Hours + "h " + length.Minutes + "m";
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
                return route.Landmarks;
            }
        }

        public bool StartEnabled
        {
            get
            {
                if (route.Landmarks.TrueForAll(lm => lm.Status != Landmark.LandmarkStatus.NOTVISITED))
                    return false;

                return App.RouteManager.CurrentRoute != route;
            }
        }

        public string StartRouteText
        {
            get
            {
                return Util.Loader.GetString("StartRoute");
            }
        }

        public bool StopEnabled
        {
            get
            {
                if (route.Landmarks.TrueForAll(lm => lm.Status != Landmark.LandmarkStatus.NOTVISITED))
                    return false;

                return App.RouteManager.CurrentRoute == route;
            }
        }

        public string StopRouteText
        {
            get
            {
                return Util.Loader.GetString("StopRoute");
            }
        }

        public void UpdateRoute()
        {
            NotifyPropertyChanged(nameof(StartEnabled));
            NotifyPropertyChanged(nameof(StopEnabled));
            NotifyPropertyChanged(nameof(Landmarks));
            App.MainPage.Title = route.Name;
        }
    }
}
