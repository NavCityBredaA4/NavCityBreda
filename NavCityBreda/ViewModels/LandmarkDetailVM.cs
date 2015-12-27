using NavCityBreda.Helpers;
using NavCityBreda.Model.Object;
using System.Collections.Generic;

namespace NavCityBreda.ViewModels
{
    public class LandmarkDetailVM : TemplateVM
    {
        Landmark landmark;

        string address;

        public LandmarkDetailVM(Landmark l) : base(l.Name)
        {
            this.landmark = l;

            address = Util.Loader.GetString("Searching") + "...";
            LoadStreet();
        }

        protected override void UpdatePropertiesToNewLanguage()
        {
            NotifyPropertyChanged(nameof(Visited));
            NotifyPropertyChanged(nameof(DescriptionTitle));
            NotifyPropertyChanged(nameof(Description));
        }

        public void Skip()
        {
            NotifyPropertyChanged(nameof(CurrentLandmark));
            NotifyPropertyChanged(nameof(IsVisited));
            NotifyPropertyChanged(nameof(Visited));
        }

        public bool CurrentLandmark
        {
            get
            {
                return landmark == App.RouteManager.CurrentLandmark && landmark.Status == Landmark.LandmarkStatus.NOTVISITED;
            }
        }

        public string SkipLandmarkText
        {
            get
            {
                return Util.Loader.GetString("SkipLandmark");
            }
        }

        public string Address
        {
            get
            {
                return address;
            }
        }

        public List<Image> Images
        {
            get
            {
                return landmark.Images;
            }
        }

        public Landmark.LandmarkStatus IsVisited
        {
            get
            {
                return landmark.Status;
            }
        }

        public string Visited
        {
            get
            {
                switch (landmark.Status)
                {
                    default:
                    case Landmark.LandmarkStatus.NOTVISITED:
                        return Util.Loader.GetString("NotVisited");
                    case Landmark.LandmarkStatus.VISITED:
                        return Util.Loader.GetString("Visited");
                    case Landmark.LandmarkStatus.SKIPPED:
                        return Util.Loader.GetString("Skipped");
                }
            }
        }

        public string DescriptionTitle
        {
            get
            {
                return Util.Loader.GetString("Description");
            }
        }

        public string Description
        {
            get
            {
                return landmark.Description;
            }
        }

        private async void LoadStreet()
        {
            address = await Util.FindAddress(landmark.Position);
            NotifyPropertyChanged(nameof(Address));
        }
    }
}
