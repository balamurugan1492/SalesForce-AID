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
    /// Class RecentInteractions.
    /// </summary>
    public class RecentInteractions : IRecentInteractions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RecentInteractions"/> class.
        /// </summary>
        /// <param name="mediaType">Type of the media.</param>
        /// <param name="interactionStarted">The interaction started.</param>
        /// <param name="interactionSubject">The interaction subject.</param>
        public RecentInteractions(string mediaType, string interactionStarted, string interactionSubject)
        {
            MediaType = mediaType;
            InteractionStarted = interactionStarted;
            InteractionSubject = interactionSubject;
        }
        public string MediaType { get; set; }

        public string InteractionStarted { get; set; }

        public string InteractionSubject { get; set; }
    }
}
