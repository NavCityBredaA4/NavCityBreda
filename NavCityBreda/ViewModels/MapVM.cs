using NavCityBreda.Helpers;
using Windows.UI.Core;

namespace NavCityBreda.ViewModels
{
    public class MapVM : TemplateVM
    {
        public MapVM() : base(Util.Loader.GetString("Map"))
        {
            App.RouteManager.OnManeuverChanged += RouteManager_OnManeuverChanged;
            App.RouteManager.OnStatusUpdate += RouteManager_OnStatusUpdate;
        }

        protected override void UpdatePropertiesToNewLanguage()
        {
            NotifyPropertyChanged(nameof(Landmark));
            NotifyPropertyChanged(nameof(Maneuver));
        }

        private void RouteManager_OnStatusUpdate(object sender, Model.RouteStatusChangedEventArgs e)
        {
            dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                NotifyPropertyChanged(nameof(Instructions));
            });
        }

        private void RouteManager_OnManeuverChanged(object sender, Model.ManeuverChangedEventArgs e)
        {
            dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                NotifyPropertyChanged(nameof(Landmark));
                NotifyPropertyChanged(nameof(Maneuver));
            });
        }

        public bool Instructions
        {
            get
            {
                return App.RouteManager.Status == Model.RouteManager.RouteStatus.STARTED;
            }
        }

        public string Maneuver
        {
            get
            {
                if (App.RouteManager.CurrentManeuver != null)
                    return Util.TranslatedManeuver(App.RouteManager.CurrentManeuver, App.RouteManager.DistanceToManeuver);
                else
                    return Util.Loader.GetString("Unknown");
            }
        }

        public string Landmark
        {
            get
            {
                if (App.RouteManager.CurrentLandmark != null)
                    return App.RouteManager.CurrentLandmark.Name;
                else
                    return Util.Loader.GetString("Unknown");
            }
        }

        public bool Tracking
        {
            get
            {
                return Settings.Tracking;
            }
            set
            {
                Settings.Tracking = value;
                NotifyPropertyChanged(nameof(Tracking));
            }
        }

        public void UpdateMap()
        {
            if (App.MainPage != null)
                App.MainPage.Title = Util.Loader.GetString("Map");
        }
    }
}
