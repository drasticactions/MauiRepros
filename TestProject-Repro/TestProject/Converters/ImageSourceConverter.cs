using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProject.Converters
{
    public class ImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                ImageSource imageSource = ImageSource.FromResource((string)value, typeof(ImageSourceConverter));
                return imageSource;
            }
            catch
            {
                ImageSource imageSource = ImageSource.FromResource("dotnet_bot.png", typeof(ImageSourceConverter));
                return imageSource;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
