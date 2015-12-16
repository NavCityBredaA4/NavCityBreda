using NavCityBreda.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
using NavCityBreda.Model;
using Windows.Storage.Streams;
using Windows.UI.Popups;

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
            Map.MapElements.Add(CurrenPosition);

            App.Geo.PositionChanged += Geo_PositionChanged;

            DrawLandmarks();
        }

        private void DrawLandmarks()
        {
            foreach(Route r in App.RouteManager.Routes)
            {
                foreach(Landmark l in r.Waypoints.Where(l => l is Landmark))
                {
                    MapIcon m = new MapIcon();
                    m.Location = l.Location;
                    m.NormalizedAnchorPoint = new Point(0.5, 1.0);
                    m.Title = l.Name;
                    m.ZIndex = 10;
                    //m.Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///" + l.Image));
                    Map.MapElements.Add(m);
                }
            }
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
        }

        private void Map_MapElementClick(MapControl sender, MapElementClickEventArgs args)
        {
            MessageDialog m = new MessageDialog("Not yet implemnted. Not sure how to get the tapped element.", "Element tapped");
            m.ShowAsync();

            Map.TrySetViewBoundsAsync(App.RouteManager.Routes.First().Bounds, new Thickness(10), MapAnimationKind.Bow);
        }
    }
}
