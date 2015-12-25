using NavCityBreda.Model;
using NavCityBreda.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace NavCityBreda.Views
{
    public sealed partial class LandmarkView : Page
    {
        LandmarkVM landmarkvm;

        public LandmarkView()
        {
            landmarkvm = new LandmarkVM();
            this.DataContext = landmarkvm;
            this.InitializeComponent();
        }

        private void LandmarkList_ItemClick(object sender, ItemClickEventArgs e)
        {
            MainPage mp = App.MainPage;
            mp.Navigate(typeof(LandmarkDetailView), e.ClickedItem as Landmark);
        }

        private void Sort_Checked(object sender, RoutedEventArgs e)
        {
            if ((bool)AlphaSorted.IsChecked)
                landmarkvm.SortList(LandmarkVM.Sort.ALPHA);
            else if ((bool)AlphaReverseSorted.IsChecked)
                landmarkvm.SortList(LandmarkVM.Sort.ALPHA_REVERSE);
            else if ((bool)VisitedSorted.IsChecked)
                landmarkvm.SortList(LandmarkVM.Sort.VISITED);
            else if ((bool)NotVisitedSorted.IsChecked)
                landmarkvm.SortList(LandmarkVM.Sort.NOT_VISITED);
        }
    }
}
