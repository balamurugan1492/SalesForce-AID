using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using Pointel.Configuration.Manager;
using Pointel.Interactions.Contact.Settings;

namespace Pointel.Interactions.Contact.Converters
{
    public class NameToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                string _value = value.ToString();
                //List<string> items = ((string)ConfigContainer.Instance().GetValue("contact.mandatory-attributes")).Split(',').ToList();
                if (!ContactDataContext.GetInstance().ContactMandatoryAttributes.Contains(Settings.ContactDataContext.GetInstance().ContactValidAttribute.Where(x => x.Value.Equals(_value)).Single().Key))
                    return Visibility.Collapsed;
            }

            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}
