/*
* =======================================
* Pointel.Configuration.Manager.Core
* =======================================
* Project    : Agent Interaction Desktop
* Created on : 
* Author     : Sakthikumar
* Owner      : Pointel Solutions
* =======================================
*/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pointel.Interactions.Email.Helper
{
    /// <summary>
    /// Class ContactOperation.
    /// </summary>
    internal class ContactOperation
    {
        internal int MaxLength = 15;
        
        /// <summary>
        /// Adjusts the description.
        /// </summary>
        /// <param name="descriptionToAdjust">The description to adjust.</param>
        /// <returns>System.String.</returns>
        internal string AdjustDescription(string descriptionToAdjust)
        {

            try
            {
                string description = descriptionToAdjust;
                if (descriptionToAdjust.Length > MaxLength)
                {
                    description = descriptionToAdjust.Substring(0, MaxLength - 3);
                    if (description.Contains(" "))
                    {
                        string[] original = descriptionToAdjust.Split(' ');
                        string[] temp = description.Split(' ');
                        if (!temp[temp.Length - 1].Equals(original[temp.Length - 1]))
                        {
                            description = description.Substring(0, description.LastIndexOf(' '));
                        }
                    }
                    description += "...";
                }
                return description;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
