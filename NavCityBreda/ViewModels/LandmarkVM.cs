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
    public class LandmarkVM : TemplateVM
    {
        Landmark landmark;

        string address;

        public LandmarkVM(Landmark l) : base(l.Name)
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

        public string Image
        {
            get
            {
                return landmark.Image;
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
            address = await Util.FindAddress(landmark.Location);
            NotifyPropertyChanged(nameof(Address));
        }
    }
}
