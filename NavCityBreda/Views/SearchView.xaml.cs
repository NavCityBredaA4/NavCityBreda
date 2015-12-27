using NavCityBreda.Helpers;
using NavCityBreda.Model;
using NavCityBreda.Model.Search;
using NavCityBreda.ViewModels;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

namespace NavCityBreda.Views
{
    public sealed partial class SearchView : Page
    {
        SearchVM searchvm;

        public SearchView()
        {
            this.InitializeComponent();
            searchvm = new SearchVM(new SearchResult(""));
            NavigationCacheMode = NavigationCacheMode.Enabled;
            Settings.OnLanguageUpdate += Settings_OnLanguageUpdate;
            this.DataContext = searchvm;
        }

        private void Settings_OnLanguageUpdate(EventArgs e)
        {
            Search();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            RouteList.SelectedIndex = -1;
            LandmarkList.SelectedIndex = -1;
            searchvm.Update();

            if (e.NavigationMode != NavigationMode.Back)
            {
                SearchTerm.Text = "";
                searchvm = new SearchVM(new SearchResult(""));
                this.DataContext = searchvm;
            }
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            SearchTerm.Text = SearchTerm.Text.Trim();
            Search();
        }

        private async void Search()
        {
            if (SearchTerm.Text.Length < 1) return;

            SearchTerm.IsEnabled = false;
            Icon.Visibility = Visibility.Collapsed;
            SearchLoading.Visibility = Visibility.Visible;
            SearchLoading.IsActive = true;
            SearchResult s = await SearchHandler.Search(SearchTerm.Text, (bool)Description.IsChecked);
            SearchLoading.IsActive = false;
            SearchLoading.Visibility = Visibility.Collapsed;
            Icon.Visibility = Visibility.Visible;
            SearchTerm.IsEnabled = true;

            searchvm = new SearchVM(s);
            this.DataContext = searchvm;
        }

        private void RouteList_ItemClick(object sender, ItemClickEventArgs e)
        {
            MainPage mp = App.MainPage;
            RouteResult r = e.ClickedItem as RouteResult;
            mp.Navigate(typeof(RouteDetailView), r.Route);
        }

        private void LandmarkList_ItemClick(object sender, ItemClickEventArgs e)
        {
            MainPage mp = App.MainPage;
            LandmarkResult l = e.ClickedItem as LandmarkResult;
            mp.Navigate(typeof(LandmarkDetailView), l.Landmark);
        }

        private void SearchTerm_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                SearchButton_Click(this, new RoutedEventArgs());
            }
        }
    }
}
