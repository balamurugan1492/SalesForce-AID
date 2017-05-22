/*
 *======================================================
 * Pointel.Interaction.Workbin.Helpers.EmailCaseData
 *======================================================
 * Project    : Agent Interaction Desktop
 * Created on : 3-Feb-2015
 * Author     : Sakthikumar
 * Owner      : Pointel Solutions
 *======================================================
 */

namespace Pointel.Interaction.Workbin.Helpers
{
    public class EmailCaseData : IEmailCaseData 
    {
        public EmailCaseData(string key, string value)
        {
            Key = key;
            Value = value;           
        }

        public string Key { get; set; }

        public string Value { get; set; }
    }
}
