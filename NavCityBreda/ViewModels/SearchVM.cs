using NavCityBreda.Helpers;
using NavCityBreda.Model.Search;
using System.Collections.Generic;

namespace NavCityBreda.ViewModels
{
    class SearchVM : TemplateVM
    {
        SearchResult result;

        public SearchVM(SearchResult s) : base(Util.Loader.GetString("Search"))
        {
            result = s;
        }

        public void Update()
        {
            App.MainPage.Title = Util.Loader.GetString("Search");
        }

        protected override void UpdatePropertiesToNewLanguage()
        {
            NotifyPropertyChanged(nameof(Search));
            NotifyPropertyChanged(nameof(Search));
            NotifyPropertyChanged(nameof(Search));
            NotifyPropertyChanged(nameof(Search));
        }

        public string SearchTerm
        {
            get
            {
                return Util.Loader.GetString("SearchTerm");
            }
        }

        public string Search
        {
            get
            {
                return Util.Loader.GetString("Search");
            }
        }

        public string SeachDescription
        {
            get
            {
                return Util.Loader.GetString("SearchDescription");
            }
        }

        public string SearchResults
        {
            get
            {
                return Util.Loader.GetString("SearchResults");
            }
        }

        public string Settings
        {
            get
            {
                return Util.Loader.GetString("Settings");
            }
        }

        public string ResultCount
        {
            get
            {
                return result.Results + " " + Util.Loader.GetString("SearchResultCount");
            }
        }

        public List<RouteResult> Routes
        {
            get
            {
                return result.Routes;
            }
        }

        public List<LandmarkResult> Landmarks
        {
            get
            {
                return result.Landmarks;
            }
        }

    }
}
