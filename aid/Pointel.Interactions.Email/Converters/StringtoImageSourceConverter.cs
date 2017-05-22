/*
* =======================================
* Pointel.Configuration.Manager.Core
* =======================================
* Project    : Agent Interaction Desktop
* Created on : 
* Author     : Moorthy
* Owner      : Pointel Solutions
* =======================================
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using Pointel.Configuration.Manager;


namespace Pointel.Interactions.Email.Converters
{
    public class StringtoImageSourceConverter : IValueConverter
    {
        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>A converted value. If the method returns null, the valid null value is used.</returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            ImageSource imgSource = null;
            if (value.ToString().ToLower().Contains("email"))
                imgSource = new BitmapImage(new Uri((ConfigContainer.Instance().AllKeys.Contains("image-path") ? (string)ConfigContainer.Instance().GetValue("image-path") : string.Empty) + "\\Email\\Email.png", UriKind.Relative));
            else if (value.ToString().ToLower().Contains("chat"))
                imgSource = new BitmapImage(new Uri((ConfigContainer.Instance().AllKeys.Contains("image-path") ? (string)ConfigContainer.Instance().GetValue("image-path") : string.Empty) + "\\Chat\\Chat.png", UriKind.Relative));
            else
                imgSource = null;
            return imgSource;
        }

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>A converted value. If the method returns null, the valid null value is used.</returns>
        /// <exception cref="NotImplementedException"></exception>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
