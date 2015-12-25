using NavCityBreda.Model;
using NavCityBreda.ViewModels;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace NavCityBreda.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RouteView : Page
    {
        RouteVM routevm;

        public RouteView()
        {
            this.InitializeComponent();
            routevm = new RouteVM();
            this.DataContext = routevm;
        }

        private void RouteList_ItemClick(object sender, ItemClickEventArgs e)
        {
            MainPage mp = App.MainPage;
            mp.Navigate(typeof(RouteDetailView), e.ClickedItem as Route);
        }
    }
}
