#region System Namespace



#endregion System Namespace

#region Genesys Namespaces

using System;
using Genesyslab.Platform.Commons.Collections;
using Genesyslab.Platform.Commons.Protocols;
using Pointel.Softphone.Voice.Common;


#endregion Genesys Namespaces

namespace Pointel.Softphone.Voice.Core
{
    public enum VoiceEvents
    {
        None,
        EventAttachedDataChanged, EventRinging, EventError,
        EventEstablished,EventReleased,EventHeld,
        EventRetrieved,EventPartyAdded,EventPartyDeleted,
        EventAgentLogin,EventAgentLogout,EventAgentReady,
        EventAgentNotReady,EventAbandoned,EventRegistered,
        EventPartyChanged,EventAddressInfo,EventDialing,
        EventUnregistered,EventUserEvent,EventDNDOn,
        EventDNDOff,EventForwardSet,EventForwardCancel,
        EventLinkConnected,EventLinkDisconnected,EventServerInfo,
        EventDtmfSent,EventNetworkReached, EventPartyInfo,EventDestinationBusy
    }
    /// <summary>
    /// This interface provide to subscribe events
    /// </summary>
    public interface ISoftphoneListener
    {
        /// <summary>
        /// This method Notifies the subscriber.
        /// </summary>
        /// <param name="callData">The call data.</param>
        void NotifySubscriber(VoiceEvents voiceEvents, object callData);

        /// <summary>
        /// This method Notifies the UI status.
        /// </summary>
        /// <param name="status">The status.</param>
        void NotifyUIStatus(SoftPhoneStatusController status);

        ///// <summary>
        ///// This method Notifies error message from T-Server
        ///// </summary>
        ///// <param name="errorMessage"></param>
        void NotifyErrorMessage(OutputValues errorMessage);

        /// <summary>
        /// This method Notifies Agent Current Status
        /// </summary>
        /// <param name="agentStatus"></param>
        void NotifyAgentStatus(AgentStatus agentStatus);

        /// <summary>
        /// This method Notifies Desktop about call ringing
        /// </summary>
        /// <param name="message"></param>
        void NotifyCallRinging(IMessage message);
    }
}