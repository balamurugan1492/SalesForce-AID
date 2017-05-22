using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Pointel.Configuration.Manager;

namespace Pointel.Interactions.Contact.Utility
{
    public class EmailUtility
    {
        public static bool IsEmailReachMaximumCount()
        {
            int maximumEmailCount = 5;

            if (ConfigContainer.Instance().AllKeys.Contains("email.max.intstance-count"))
                int.TryParse(((string)ConfigContainer.Instance().GetValue("email.max.intstance-count")), out maximumEmailCount);
            List<Window> emailWindows = Application.Current.Windows.Cast<Window>().Where(x => x.Title.Equals("Email")).ToList();
            if (emailWindows.Count == maximumEmailCount)
            {
                return true;
            }
            return false;
        }
    }
}
