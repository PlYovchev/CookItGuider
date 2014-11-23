using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;

namespace CookItUniversal.Converters
{
    public class BytesToImageConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value != null && value is byte[])
            {
                byte[] bytes = value as byte[];
                MemoryStream stream = new MemoryStream(bytes);
                BitmapImage image = new BitmapImage();

                image.SetSource(stream.AsRandomAccessStream());

                return image;
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
