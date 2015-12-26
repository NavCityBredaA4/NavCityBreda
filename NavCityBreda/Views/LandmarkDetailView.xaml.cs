using NavCityBreda.Model;
using NavCityBreda.ViewModels;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace NavCityBreda.Views
{
    public sealed partial class LandmarkDetailView : Page
    {
        LandmarkDetailVM landmarkvm;

        public LandmarkDetailView()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Landmark landmark = e.Parameter as Landmark;
            landmarkvm = new LandmarkDetailVM(landmark);
            this.DataContext = landmarkvm;
        }

        private void Button_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Skip();
        }

        private async void Skip()
        {
            await App.RouteManager.SkipLandmark();
            landmarkvm.Skip();
        }
    }
}
