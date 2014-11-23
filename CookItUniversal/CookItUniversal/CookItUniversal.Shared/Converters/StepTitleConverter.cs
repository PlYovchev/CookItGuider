using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml.Data;

namespace CookItUniversal.Converters
{
    public class StepTitleConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string stepNumber = value.ToString();
            string fullTitle = "Step " + stepNumber;

            return fullTitle;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}
