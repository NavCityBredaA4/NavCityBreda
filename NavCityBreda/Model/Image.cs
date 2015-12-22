using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.ViewManagement;

namespace NavCityBreda.Model
{
    public class Image
    {
        public string Source { get; private set; }

        public Image(string source)
        {
            Source = source;
        }

        public double Width
        {
            get
            {
                return ApplicationView.GetForCurrentView().VisibleBounds.Width;
            }
        }
    }
}
