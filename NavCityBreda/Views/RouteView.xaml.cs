using NavCityBreda.Helpers;
using NavCityBreda.Model;
using NavCityBreda.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

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
