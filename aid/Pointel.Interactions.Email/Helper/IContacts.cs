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

namespace Pointel.Interactions.Email.Helper
{
    /// <summary>
    /// Interface IContacts
    /// </summary>
    public interface IContacts
    {
        string Name { get; set; }

        string Number { get; set; }

        string Type { get; set; }
    }
}
