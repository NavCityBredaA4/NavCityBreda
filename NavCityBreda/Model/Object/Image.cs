using Windows.UI.ViewManagement;

namespace NavCityBreda.Model.Object
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
