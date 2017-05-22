using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using Pointel.Interactions.Chat.Settings;
using System.Windows.Media;

namespace Pointel.Interactions.Chat.Converters
{
    public class StringtoImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            ImageSource imgSource = null;
            if (value.ToString().ToLower().Contains("email"))
                imgSource = new BitmapImage(new Uri(ChatDataContext.GetInstance().Imagepath + "\\Email\\Email.png", UriKind.Relative));
            else if (value.ToString().ToLower().Contains("chat"))
                imgSource = new BitmapImage(new Uri(ChatDataContext.GetInstance().Imagepath + "\\Chat\\Chat.png", UriKind.Relative));
            else
                imgSource = null;
            return imgSource;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
