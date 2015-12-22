using NavCityBreda.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;

namespace NavCityBreda.ViewModels
{
    public class MapVM : TemplateVM
    {
        //In een route de MapLegs gaan bijhouden. Dan de eerste als current instellen. 
        //Dan kan je daarvan de informatie laten zien. Ook een event toevoegen als je dan aan het einde bent, dat weet je door lon-lon > 0.? te doen elke positie update.
        //Allemaal in RouteManager afhandelen
        //Nog een manier vinden om van een route afwijken op te vangen en dan opnieuw de route te berekenen.

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
            dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => {
                NotifyPropertyChanged(nameof(Instructions));
            });
        }

        private void RouteManager_OnManeuverChanged(object sender, Model.ManeuverChangedEventArgs e)
        {
            dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => {
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
                    return Util.TranslatedManeuver(App.RouteManager.CurrentManeuver);
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

        public void UpdateMap()
        {
            if(App.MainPage != null)
                App.MainPage.Title = Util.Loader.GetString("Map");
        }
    }
}
