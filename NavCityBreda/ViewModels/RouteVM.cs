using NavCityBreda.Helpers;
using NavCityBreda.Model.Object;
using System.Collections.Generic;

namespace NavCityBreda.ViewModels
{
    public class RouteVM : TemplateVM
    {
        private List<Route> _routes;

        public RouteVM() : base(Util.Loader.GetString("Route"))
        {
            _routes = App.RouteManager.Routes;
        }

        protected override void UpdatePropertiesToNewLanguage()
        {
            NotifyPropertyChanged(nameof(Routes));
            NotifyPropertyChanged(nameof(Description));
        }

        public string Description
        {
            get
            {
                return Util.Loader.GetString("RouteListDescription");
            }
        }

        public List<Route> Routes
        {
            get
            {
                return _routes;
            }
        }
    }
}
