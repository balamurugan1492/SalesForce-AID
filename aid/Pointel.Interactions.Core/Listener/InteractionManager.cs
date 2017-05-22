using System;
using Genesyslab.Platform.ApplicationBlocks.Commons.Broker;
using Genesyslab.Platform.Commons.Collections;
using Genesyslab.Platform.Commons.Protocols;
using Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events;
using Pointel.Interactions.Core.Common;
using Pointel.Interactions.Core.ConnectionManager;
using Pointel.Interactions.Core.Util;


namespace Pointel.Interactions.Core.Listener
{
    internal class InteractionManager
    {
        #region Field Declaration
        private static Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");
        static IInteractionServices messageToClient = null;
        private static InteractionManager _interactionListener;
        public static bool isAfterConnect = false;
        private static int protocolClosed = 0;
        private static int protocolOpened = 0;
        public static bool EventCreated { get; set; }
        #endregion

        /// <summary>
        /// This method used to create singleton object for this class
        /// </summary>
        /// <returns>VoiceManager</returns>
        #region GetInstance
        public static InteractionManager GetInstance()
        {
            if (_interactionListener == null)
            {
                _interactionListener = new InteractionManager();

                return _interactionListener;
            }
            else
                return _interactionListener;
        }
        #endregion

        # region Interaction Listener
        /// <summary>
        /// Interactions the events. 
        /// </summary>
        /// <param name="message">The message.</param>
        public static void InteractionEvents(object sender, EventArgs e)
        {
            Settings settings = Settings.GetInstance();
            try
            {
                logger.Debug("Listening both Solicited and Unsolicited Events through EventBroker");
                IMessage message = ((MessageEventArgs)e).Message;
                if (message != null)
                {

                    try
                    {
                        if (!EventCreated)
                        {
                            Settings.InteractionProtocol.Opened += InteractionManager_Opened;
                            Settings.InteractionProtocol.Closed += InteractionManager_Closed;
                            EventCreated = true;
                        }
                    }
                    catch (Exception protocolStatusException)
                    {
                        logger.Error("Error occurred while display the protocol status " + protocolStatusException.ToString());
                    }
                    switch (message.Id)
                    {
                        case EventAgentLogin.MessageId:
                            EventAgentLogin eventAgentLogin = (EventAgentLogin)message;
                            logger.Trace("EventAgentLogin:" + eventAgentLogin.ToString());
                            break;
                        case EventAgentInvited.MessageId:
                            EventAgentInvited eventInvited = (EventAgentInvited)message;
                            logger.Trace("EventAgentInvited:" + eventInvited.ToString());
                            if (eventInvited.Interaction.InteractionMediatype.Equals("email"))
                            {
                                if (Settings.subscriberObject.ContainsKey(InteractionTypes.Email))
                                {
                                    IInteractionServices ixnService = Settings.subscriberObject[InteractionTypes.Email];
                                    ixnService.NotifyEServiceInteraction(message, InteractionTypes.Email);
                                }
                            }
                            if (eventInvited.Interaction.InteractionMediatype.Equals("chat"))
                            {
                                IInteractionServices ixnService = Settings.subscriberObject[InteractionTypes.Chat];

                                ixnService.NotifyEServiceInteraction(message, InteractionTypes.Chat);
                            }
                            break;
                        case EventInvite.MessageId:
                            EventInvite eventInvite = (EventInvite)message;
                            logger.Trace("EventInvite:" + eventInvite.ToString());
                            if (eventInvite.Interaction.InteractionMediatype.Equals("email"))
                            {
                                if (Settings.subscriberObject.ContainsKey(InteractionTypes.Email))
                                {
                                    IInteractionServices ixnService = Settings.subscriberObject[InteractionTypes.Email];
                                    ixnService.NotifyEServiceInteraction(message, InteractionTypes.Email);
                                }
                            }
                            if (eventInvite.Interaction.InteractionMediatype.Equals("chat"))
                            {
                                if (Settings.subscriberObject.ContainsKey(InteractionTypes.Chat))
                                {
                                    IInteractionServices ixnService = Settings.subscriberObject[InteractionTypes.Chat];
                                    ixnService.NotifyEServiceInteraction(message, InteractionTypes.Chat);
                                }
                            }
                            if (eventInvite.Interaction.InteractionMediatype.Equals("outboundpreview"))
                            {
                                if (Settings.subscriberObject.ContainsKey(InteractionTypes.OutboundPreview))
                                {
                                    IInteractionServices ixnService = Settings.subscriberObject[InteractionTypes.OutboundPreview];
                                    ixnService.NotifyEServiceInteraction(message, InteractionTypes.OutboundPreview);
                                }
                            }
                            break;
                        case EventPropertiesChanged.MessageId:
                            EventPropertiesChanged eventPropertiesChanged = (EventPropertiesChanged)message;
                            logger.Trace("EventPropertiesChanged:" + eventPropertiesChanged.ToString());
                            if (eventPropertiesChanged.Interaction.InteractionMediatype.Equals("email"))
                            {
                                if (Settings.subscriberObject.ContainsKey(InteractionTypes.Email))
                                {
                                    IInteractionServices ixnService = Settings.subscriberObject[InteractionTypes.Email];

                                    ixnService.NotifyEServiceInteraction(message, InteractionTypes.Email);
                                }

                            }
                            if (eventPropertiesChanged.Interaction.InteractionMediatype.Equals("chat"))
                            {
                                if (Settings.subscriberObject.ContainsKey(InteractionTypes.Chat))
                                {
                                    IInteractionServices ixnService = Settings.subscriberObject[InteractionTypes.Chat];
                                    ixnService.NotifyEServiceInteraction(message, InteractionTypes.Chat);
                                }
                            }
                            break;
                        case EventProcessingStopped.MessageId:
                            EventProcessingStopped eventProcessingStopped = (EventProcessingStopped)message;
                            logger.Trace("EventProcessingStopped:" + eventProcessingStopped.ToString());
                            if (eventProcessingStopped.Interaction.InteractionMediatype.Equals("email"))
                            {
                                if (Settings.subscriberObject.ContainsKey(InteractionTypes.Email))
                                {
                                    IInteractionServices ixnService = Settings.subscriberObject[InteractionTypes.Email];
                                    ixnService.NotifyEServiceInteraction(message, InteractionTypes.Email);
                                }
                            }
                            break;

                        case EventRevoked.MessageId:
                            EventRevoked eventRevoked = (EventRevoked)message;
                            logger.Trace("EventRevoked:" + eventRevoked.ToString());
                            if (eventRevoked.Interaction.InteractionMediatype.Equals("email"))
                            {
                                IInteractionServices ixnService = Settings.subscriberObject[InteractionTypes.Email];

                                ixnService.NotifyEServiceInteraction(message, InteractionTypes.Email);
                            }
                            if (eventRevoked.Interaction.InteractionMediatype.Equals("chat"))
                            {
                                IInteractionServices ixnService = Settings.subscriberObject[InteractionTypes.Chat];

                                ixnService.NotifyEServiceInteraction(message, InteractionTypes.Chat);
                            }
                            if (eventRevoked.Interaction.InteractionMediatype.Equals("outboundpreview"))
                            {
                                if (Settings.subscriberObject.ContainsKey(InteractionTypes.OutboundPreview))
                                {
                                    IInteractionServices ixnService = Settings.subscriberObject[InteractionTypes.OutboundPreview];
                                    ixnService.NotifyEServiceInteraction(message, InteractionTypes.OutboundPreview);
                                }
                            }
                            break;
                        case EventCurrentAgentStatus.MessageId:
                            EventCurrentAgentStatus eventCurrentAgentStatus = (EventCurrentAgentStatus)message;
                            logger.Trace("EventCurrentAgentStatus:" + eventCurrentAgentStatus.ToString());
                            //if (messageToClient != null && eventCurrentAgentStatus.DonotDisturb)
                            //    messageToClient.NotifyIxnAgentMediaStatus(message);
                            break;
                        case EventForcedAgentStateChange.MessageId:
                            EventForcedAgentStateChange eventForcedAgentStateChange = (EventForcedAgentStateChange)message;
                            logger.Trace("EventForcedAgentStateChange:" + eventForcedAgentStateChange.ToString());
                            if (messageToClient != null)
                                messageToClient.NotifyIxnAgentMediaStatus(message);
                            break;
                        case EventPartyAdded.MessageId:
                            EventPartyAdded eventPartyAdded = (EventPartyAdded)message;
                            logger.Trace("EventPartyAdded" + eventPartyAdded.ToString());
                            if (eventPartyAdded.Interaction.InteractionMediatype.Equals("email"))
                            {
                                if (Settings.subscriberObject.ContainsKey(InteractionTypes.Email))
                                {
                                    IInteractionServices ixnService = Settings.subscriberObject[InteractionTypes.Email];
                                    ixnService.NotifyEServiceInteraction(message, InteractionTypes.Email);
                                }
                            }
                            if (eventPartyAdded.Interaction.InteractionMediatype.Equals("chat"))
                            {
                                IInteractionServices ixnService = Settings.subscriberObject[InteractionTypes.Chat];

                                ixnService.NotifyEServiceInteraction(message, InteractionTypes.Chat);
                            }
                            break;
                        case EventPartyRemoved.MessageId:
                            EventPartyRemoved eventPartyRemoved = (EventPartyRemoved)message;
                            logger.Trace("EventPartyRemoved" + eventPartyRemoved.ToString());
                            if (eventPartyRemoved.Interaction.InteractionMediatype.Equals("email"))
                            {
                                if (Settings.subscriberObject.ContainsKey(InteractionTypes.Email))
                                {
                                    IInteractionServices ixnService = Settings.subscriberObject[InteractionTypes.Email];
                                    ixnService.NotifyEServiceInteraction(message, InteractionTypes.Email);
                                }
                            }
                            if (eventPartyRemoved.Interaction.InteractionMediatype.Equals("chat"))
                            {
                                IInteractionServices ixnService = Settings.subscriberObject[InteractionTypes.Chat];

                                ixnService.NotifyEServiceInteraction(message, InteractionTypes.Chat);
                            }
                            break;
                        case EventRejected.MessageId:
                            EventRejected eventRejected = (EventRejected)message;
                            if (eventRejected.Interaction.InteractionMediatype == null) break;
                            switch (eventRejected.Interaction.InteractionMediatype)
                            {
                                case "email":
                                    if (Settings.subscriberObject.ContainsKey(InteractionTypes.Email))
                                        Settings.subscriberObject[InteractionTypes.Email].NotifyEServiceInteraction(message, InteractionTypes.Email);
                                    break;
                                case "chat":
                                    if (Settings.subscriberObject.ContainsKey(InteractionTypes.Chat))
                                        Settings.subscriberObject[InteractionTypes.Chat].NotifyEServiceInteraction(message, InteractionTypes.Chat);
                                    break;
                            }
                            break;
                        case EventWorkbinContentChanged.MessageId:
                            if (Settings.subscriberObject.ContainsKey(InteractionTypes.Email))
                            {
                                Settings.subscriberObject[InteractionTypes.Email].NotifyEServiceInteraction(message, InteractionTypes.Email);
                            }
                            break;
                        case EventPulledInteractions.MessageId:
                            EventPulledInteractions eventPulledInteractions = (EventPulledInteractions)message;
                            if (eventPulledInteractions.Interactions == null) break;
                            if (eventPulledInteractions.Interactions.AllValues == null) break;
                            if (!(eventPulledInteractions.Interactions.AllValues[0] as KeyValueCollection).ContainsKey("MediaType")) break;
                            switch ((eventPulledInteractions.Interactions.AllValues[0] as KeyValueCollection)["MediaType"].ToString().ToLower())
                            {
                                case "email":
                                    //if (Settings.subscriberObject.ContainsKey(InteractionTypes.Email))
                                    //    Settings.subscriberObject[InteractionTypes.Email].NotifyEServiceInteraction(message, InteractionTypes.Email);
                                    break;
                                case "chat":
                                    //if (Settings.subscriberObject.ContainsKey(InteractionTypes.Chat))
                                    //    Settings.subscriberObject[InteractionTypes.Chat].NotifyEServiceInteraction(message, InteractionTypes.Chat);
                                    break;
                            }
                            break;
                        case EventError.MessageId:
                            EventError eventError = (EventError)message;
                            messageToClient.NotifyIxnAgentMediaStatus(message);
                            logger.Trace("EventError" + eventError.ToString());
                            break;
                        case EventAck.MessageId:
                            EventAck eventAck = (EventAck)message;
                            logger.Trace("EventAck" + eventAck.ToString());
                            if (Settings.subscriberObject.ContainsKey(InteractionTypes.Email))
                            {
                                IInteractionServices ixnService = Settings.subscriberObject[InteractionTypes.Email];
                                ixnService.NotifyEServiceInteraction(message, InteractionTypes.Email);
                            }
                            if (Settings.subscriberObject.ContainsKey(InteractionTypes.Chat))
                            {
                                IInteractionServices chatClient = Settings.subscriberObject[InteractionTypes.Chat];
                                chatClient.NotifyEServiceInteraction(message, InteractionTypes.Chat);
                            }
                            break;
                    }

                }
                else
                {
                    logger.Error("IntercationEvents: Error Occurred in Interaction Server");
                }

            }
            catch (Exception commonException)
            {
                logger.Error("IntercationEvents: Error Occurred " + commonException.ToString());
            }
        }
        # endregion

        //Code added for the purpose of get the interaction server open and close events
        //Smoorthy - 09-06-2014

        #region InteractionManager_Closed
        /// <summary>
        /// Handles the Closed event of the InteractionManager control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private static void InteractionManager_Closed(object sender, EventArgs e)
        {
            logger.Warn("Interaction Protocol Closed");
            if (protocolClosed == 0)
            {
                protocolClosed++;
                protocolOpened = 0;
                isAfterConnect = true;
                messageToClient.NotifyInteractionServerStatus(IXNServerState.Closed);
            }
        }
        #endregion

        #region InteractionManager_Opened
        /// <summary>
        /// Handles the Opened event of the InteractionManager control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private static void InteractionManager_Opened(object sender, EventArgs e)
        {
            logger.Warn("Interaction Protocol Opened");
            if (protocolOpened == 0 && isAfterConnect)
            {
                protocolOpened++;
                protocolClosed = 0;
                isAfterConnect = false;
                messageToClient.NotifyInteractionServerStatus(IXNServerState.Opened);
            }
        }
        #endregion

        //end

        #region Subscribe
        /// <summary>
        /// Subscribes the agent media status.
        /// </summary>
        /// <param name="listener">The listener.</param>
        public void SubscribeAgentMediaStatus(IInteractionServices listener)
        {
            messageToClient = listener;
        }
        #endregion

    }
}
