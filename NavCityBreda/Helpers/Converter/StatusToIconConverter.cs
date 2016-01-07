using NavCityBreda.Model.Object;
using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace NavCityBreda.Helpers.Converter
{
    class StatusToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            Landmark.LandmarkStatus st = (Landmark.LandmarkStatus)value;

            switch (st)
            {
                default:
                case Landmark.LandmarkStatus.NOTVISITED:
                    return Symbol.Cancel;
                case Landmark.LandmarkStatus.VISITED:
                    return Symbol.Accept;
                case Landmark.LandmarkStatus.SKIPPED:
                    return Symbol.Forward;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            //Not implemented
            return null;
        }
    }
}
