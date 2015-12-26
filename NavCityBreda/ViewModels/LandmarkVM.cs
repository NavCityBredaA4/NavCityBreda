using NavCityBreda.Helpers;
using NavCityBreda.Helpers.Comparer;
using NavCityBreda.Model;
using System;
using System.Collections.Generic;

namespace NavCityBreda.ViewModels
{
    class LandmarkVM : TemplateVM
    {
        public enum Sort { ALPHA, ALPHA_REVERSE, VISITED, NOT_VISITED }

        private List<Landmark> _landmarks;

        public LandmarkVM() : base(Util.Loader.GetString("Landmarks"))
        {
            _landmarks = App.RouteManager.GetAllLandmarks();
        }

        protected override void UpdatePropertiesToNewLanguage()
        {
            NotifyPropertyChanged(nameof(Description));
            NotifyPropertyChanged(nameof(Landmarks));
            NotifyPropertyChanged(nameof(LandmarksTitle));

            NotifyPropertyChanged(nameof(Ascending));
            NotifyPropertyChanged(nameof(Descending));
            NotifyPropertyChanged(nameof(Visited));
            NotifyPropertyChanged(nameof(NotVisited));
        }

        public string SortOrderTitle
        {
            get
            {
                return Util.Loader.GetString("SortOrder");
            }
        }

        public string LandmarksTitle
        {
            get
            {
                return Util.Loader.GetString("Landmarks");
            }
        }

        public string Ascending
        {
            get
            {
                return Util.Loader.GetString("Ascending");
            }
        }

        public string Descending
        {
            get
            {
                return Util.Loader.GetString("Descending");
            }
        }

        public string Visited
        {
            get
            {
                return Util.Loader.GetString("Visited");
            }
        }

        public string NotVisited
        {
            get
            {
                return Util.Loader.GetString("NotVisited");
            }
        }

        public string Description
        {
            get
            {
                return Util.Loader.GetString("LandmarkListDescription");
            }
        }

        public List<Landmark> Landmarks
        {
            get
            {
                return _landmarks;
            }
        }

        public void SortList(Sort s)
        {
            IComparer<Landmark> comparer;

            switch(s)
            {
                default:
                case Sort.ALPHA:
                    comparer = new LandmarkAlphaComparer();
                    break;
                case Sort.ALPHA_REVERSE:
                    comparer = new LandmarkAlphaReversedComparer();
                    break;
                case Sort.VISITED:
                    comparer = new LandmarkVisitedComparer();
                    break;
                case Sort.NOT_VISITED:
                    comparer = new LandmarkNotVisitedComparer();
                    break;
            }

            _landmarks.Sort(comparer);
            _landmarks = new List<Landmark>(_landmarks);

            NotifyPropertyChanged(nameof(Landmarks));
        }
    }
}
