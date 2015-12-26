using NavCityBreda.ViewModels;
using Windows.UI.Xaml.Controls;

namespace NavCityBreda.Views
{
    public sealed partial class HelpView : Page
    {
        HelpVM helpvm;

        public HelpView()
        {
            helpvm = new HelpVM();
            this.DataContext = helpvm;
            this.InitializeComponent();
        }
    }
}
