using NavCityBreda.Helpers;
using NavCityBreda.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;

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
