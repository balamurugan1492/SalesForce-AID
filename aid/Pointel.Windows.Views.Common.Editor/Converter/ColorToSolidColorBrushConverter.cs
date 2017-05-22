using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Pointel.Windows.Views.Common.Editor.Converter
{
    [ValueConversion(typeof(Color), typeof(Brush))]
    public class ColorToSolidColorBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new SolidColorBrush((Color)(value ?? Colors.Black));
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
