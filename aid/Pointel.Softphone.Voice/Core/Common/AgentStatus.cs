#region System Namespace

using System.Collections.Generic;

#endregion System Namespace

#region Genesys Namespace

using Genesyslab.Platform.Commons.Collections;
using Pointel.Softphone.Voice.Core;

#endregion Genesys Namespace

namespace Pointel.Softphone.Voice
{
    /// <summary>
    /// Softphone subscriber enumerator
    /// </summary>
    public enum SoftPhoneSubscriber
    {
        UI,
        Integration
    }

    /// <summary>
    /// Sets/gets the Current Agent Status using the enumerator
    /// </summary>
    public enum CurrentAgentStatus
    {
        Registered,
        Ready,
        NotReady,
        AfterCallWork,
        OnCall,
        OnHeld,
        OnRetrieve,
        OnRelease,
        Logout,
        CallRinging,
        CallDialing,
        DNDOn,
        DNDOff
    }

    public class AgentStatus
    {
        #region Field Declaration

        private string agentStatus;
        public Dictionary<string, string> reasons = new Dictionary<string, string>();
        private KeyValueCollection extensions = new KeyValueCollection();
        public static AgentStatus agentStatusObject = null;
        private string agentID = null;
        private string addressType = null;
        private string connId = null;
        private string callType = null;
        private VoiceEvents exactEvent = VoiceEvents.None;

        #endregion Field Declaration

        ~AgentStatus()
        {
            agentStatus = null;
            reasons = null;
            extensions = null;
            addressType = null;
            agentStatusObject = null;
            agentID = null;
            connId = null;
            callType = null;
        }

        #region Properties

        /// <summary>
        /// Gets or sets the connection unique identifier.
        /// </summary>
        /// <value>
        /// The connection unique identifier.
        /// </value>
        public string ConnId
        {
            get { return connId; }
            set { connId = value; }
        }

        /// <summary>
        /// Gets or sets the agent unique identifier.
        /// </summary>
        /// <value>
        /// The agent unique identifier.
        /// </value>
        public string AgentID
        {
            get { return agentID; }
            set { agentID = value; }
        }

        /// <summary>
        /// Gets or sets the type of the address.
        /// </summary>
        /// <value>
        /// The type of the address.
        /// </value>
        public string AddressType
        {
            get { return addressType; }
            set { addressType = value; }
        }

        /// <summary>
        /// Gets or sets the agent current status.
        /// </summary>
        /// <value>
        /// The agent current status.
        /// </value>
        public string AgentCurrentStatus
        {
            get { return agentStatus; }
            set { agentStatus = value; }
        }

        public VoiceEvents ExactEvent
        {
            get { return exactEvent; }
            set { exactEvent = value; }
        }

        /// <summary>
        /// Gets or sets the reasons.
        /// </summary>
        /// <value>
        /// The reasons.
        /// </value>
        public Dictionary<string, string> Reasons
        {
            get { return reasons; }
            set { reasons = value; }
        }

        /// <summary>
        /// Gets or sets the extensions.
        /// </summary>
        /// <value>
        /// The extensions.
        /// </value>
        public KeyValueCollection Extensions
        {
            get { return extensions; }
            set { extensions = value; }
        }

        public string CallType
        {
            get { return callType; }
            set { callType = value; }
        }

        #endregion Properties
    }
}