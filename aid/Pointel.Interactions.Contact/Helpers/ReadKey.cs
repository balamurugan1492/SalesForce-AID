using Pointel.Configuration.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pointel.Interactions.Contact.Helpers
{
    public static class ReadKey
    {
        public static List<string> ReadConfigKeys(string keyName, string[] defaultValues, List<string> validValues)
        {
            List<string> listValues = new List<string>();
            Pointel.Logger.Core.ILog _logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
             "AID");
            try
            {
                if (ConfigContainer.Instance().AllKeys.Contains(keyName) &&
                          !string.IsNullOrEmpty(ConfigContainer.Instance().GetAsString(keyName).Trim()))
                {
                    listValues = validValues.Intersect(ConfigContainer.Instance().GetAsString(keyName).Split(',').Select(x => x.Trim()).ToArray().ToList()).ToList();
                    
                    if (listValues.Count > 1)
                        listValues = listValues.Distinct().ToList();

                    if (listValues.Count == 0)
                        listValues = new List<string>(defaultValues);
                }
                else
                {
                    listValues = new List<string>(defaultValues);
                }
            }
            catch (Exception generalException)
            {
                _logger.Error("btnAdvanceSearch_Click: Error occurred as " + generalException.ToString());
            }
            return listValues;
        }
    }
}
