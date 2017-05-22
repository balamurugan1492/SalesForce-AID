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
using System.Windows.Media;

namespace Pointel.Interactions.Email.Helper
{
    /// <summary>
    /// Interface IRecentInteractions
    /// </summary>
    public interface IRecentInteractions
    {
        string MediaType { get; set; }

        string InteractionStarted { get; set; }

        string InteractionSubject { get; set; }
    }
}
