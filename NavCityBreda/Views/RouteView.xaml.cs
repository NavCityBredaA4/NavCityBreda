﻿using NavCityBreda.Helpers;
using NavCityBreda.Model;
using NavCityBreda.ViewModel;
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
        private RouteVM _routemodel = new RouteVM();
        public List<Route> RouteViewModel
        {
            get { return _routemodel.GetRoutes(); }
        }

        public RouteView()
        {
            this.InitializeComponent();
            this.DataContext = RouteViewModel;
        }

        private void RouteList_ItemClick(object sender, ItemClickEventArgs e)
        {
            MainPage mp = App.MainPage;
            mp.Navigate(typeof(RouteDetailView), e.ClickedItem as Route);
        }
    }
}
