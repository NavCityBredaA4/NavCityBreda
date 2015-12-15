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
using Windows.Devices.Geolocation;
using Windows.UI.Xaml.Controls.Maps;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace NavCityBreda.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MapView : Page
    {
        MapIcon CurrenPosition;

        public MapView()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Enabled;

            this.DataContext = new MapVM();

            CurrenPosition = new MapIcon();

            App.Geo.PositionChanged += Geo_PositionChanged;
        }

        private void Geo_PositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, () =>
            {
                DrawCurrenPosition(new Geopoint(args.Position.Coordinate.Point.Position));
            });
        }

        private void DrawCurrenPosition(Geopoint p)
        {
            CurrenPosition.Location = p;
            CurrenPosition.NormalizedAnchorPoint = new Point(0.5, 1.0);
            CurrenPosition.Title = "Current Position";
            CurrenPosition.ZIndex = 999;

            Map.MapElements.Add(CurrenPosition);
        }
    }
}
