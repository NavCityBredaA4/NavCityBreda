using NavCityBreda.ViewModels;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace NavCityBreda.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
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
