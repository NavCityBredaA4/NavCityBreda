using NavCityBreda.Helpers;
using NavCityBreda.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
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
    public sealed partial class LandmarkView : Page
    {
        Landmark landmark; 

        public LandmarkView()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            landmark = e.Parameter as Landmark;
            this.DataContext = landmark;
            Util.MainPage.Title = landmark.Name;
            LoadStreet();
        }

        private async void LoadStreet()
        {
            await Task.Delay(TimeSpan.FromMilliseconds(350));
            string address = await Util.FindAddress(landmark.Location);
            StreetLoading.IsActive = false;
            StreetLoading.Visibility = Visibility.Collapsed;
            StreetAddress.Text = address;
        }
    }
}
