using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace NavCityBreda.Helpers.Converter
{
    class BoolToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if ((bool)value)
                return Symbol.Like;
            else
                return Symbol.Cancel;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            //Not implemented
            return null;
        }
    }
}
