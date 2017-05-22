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
    /// Class Contacts.
    /// </summary>
    public class Contacts : IContacts
    {
        public Contacts(string name, string number, string type)
        {
            Name = name;
            Number = number;
            Type = type;
        }
        public string Name { get; set; }
        public string Number { get; set; }
        public string Type { get; set; }  
    }
}
