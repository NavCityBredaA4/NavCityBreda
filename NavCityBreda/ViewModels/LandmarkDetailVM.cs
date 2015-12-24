using NavCityBreda.Helpers;
using NavCityBreda.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

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

        public bool IsVisited
        {
            get
            {
                return landmark.Visited;
            }
        } 

        public string Visited
        {
            get
            {
                if (landmark.Visited)
                    return Util.Loader.GetString("Visited");
                else
                    return Util.Loader.GetString("NotVisited");
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
