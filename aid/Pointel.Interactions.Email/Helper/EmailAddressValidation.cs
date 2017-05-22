/*
* =======================================
* Pointel.Configuration.Manager.Core
* =======================================
* Project    : Agent Interaction Desktop
* Created on : 
* Author     : Rajkumar
* Owner      : Pointel Solutions
* =======================================
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace Pointel.Interactions.Email.Helper
{
    /// <summary>
    /// Class EmailAddressValidation.
    /// </summary>
    class EmailAddressValidation
    {
        /// <summary>
        /// Gets the email address.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <returns>System.String.</returns>
        public static string GetEmailAddress(string email)
        {
            string result = string.Empty;

            email = email.Replace(";", ",");
            email = email.Replace("\r", string.Empty);
            email = email.Replace("\n", string.Empty);
            email = email.Replace("\t", string.Empty);

            string[] emailAddress = (email.EndsWith(",") ? email.Remove(email.Length - 1) : email).Split(',');
            foreach (string address in emailAddress)
            {
                if (address.Contains("<") || address.Contains(">"))
                    if (!string.IsNullOrEmpty(result))
                        result = result + ";" + Between(address, "<", ">");
                    else
                        result = Between(address, "<", ">");
                else
                    if (!string.IsNullOrEmpty(result))
                        result = result + ";" + address;
                    else
                        result = address;
            }
            return result;
        }

        /// <summary>
        /// Betweens the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="a">a.</param>
        /// <param name="b">The b.</param>
        /// <returns>System.String.</returns>
        public static string Between(string value, string a, string b)
        {
            int posA = value.IndexOf(a);
            int posB = value.LastIndexOf(b);
            if (posA == -1)
            {
                return "";
            }
            if (posB == -1)
            {
                return "";
            }
            int adjustedPosA = posA + a.Length;
            if (adjustedPosA >= posB)
            {
                return "";
            }
            return value.Substring(adjustedPosA, posB - adjustedPosA);
        }

        /// <summary>
        /// Gets the local date time.
        /// </summary>
        /// <param name="datetime">The datetime.</param>
        /// <returns>System.String.</returns>
        public static string GetLocalDateTime(string datetime)
        {
            CultureInfo enUS = CultureInfo.CreateSpecificCulture("en-US");
            DateTimeFormatInfo dtfi = enUS.DateTimeFormat;
            DateTime _time;
            if (DateTime.TryParse(datetime, null, System.Globalization.DateTimeStyles.AssumeLocal, out _time))
                return _time.ToString(dtfi);
            else
                return datetime;
        }
    }
}
