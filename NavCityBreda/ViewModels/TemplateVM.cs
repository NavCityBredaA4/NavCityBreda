using NavCityBreda.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;

namespace NavCityBreda.ViewModels
{
    public class TemplateVM : INotifyPropertyChanged
    {
        protected CoreDispatcher dispatcher;

        public TemplateVM(string title)
        {
            dispatcher = App.Dispatcher;
            Settings.OnLanguageUpdate += Settings_OnLanguageUpdate;

            if(App.MainPage != null)
                App.MainPage.Title = title;
        }

        private void Settings_OnLanguageUpdate(EventArgs e)
        {
            dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                UpdatePropertiesToNewLanguage();
            });
        }

        protected virtual void UpdatePropertiesToNewLanguage()
        {
            //to implement in underlying class
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
