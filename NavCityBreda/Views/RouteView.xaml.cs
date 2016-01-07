using NavCityBreda.Model.Object;
using NavCityBreda.ViewModels;
using Windows.UI.Xaml.Controls;

namespace NavCityBreda.Views
{
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
