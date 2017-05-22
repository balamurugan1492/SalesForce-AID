namespace Pointel.Interactions.Chat.Core
{
    using System;
    using System.Reflection;

    using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel;
    using Genesyslab.Platform.Commons.Collections;
    using Genesyslab.Platform.OpenMedia.Protocols;
    using Genesyslab.Platform.WebMedia.Protocols;

    using Pointel.Connection.Manager;
    using Pointel.Interactions.Chat.Core.Application;
    using Pointel.Interactions.Chat.Core.ChatRequest;
    using Pointel.Interactions.Chat.Core.ConnectionManager;
    using Pointel.Interactions.Chat.Core.General;
    using Pointel.Interactions.Chat.Core.Listener;
    using Pointel.Interactions.Chat.Core.Util;

    public class ChatMedia
    {
        #region Fields

        private Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");

        #endregion Fields

        #region Constructors

        public ChatMedia()
        {
        }

        #endregion Constructors

        #region Methods

        public OutputValues CheckChatServerStatus()
        {
            OutputValues output = OutputValues.GetInstance();
            try
            {
                try
                {
                    var temp = ProtocolManagers.Instance().ProtocolManager[ServerType.Chatserver.ToString()];
                    if (Pointel.Interactions.Chat.Core.Util.Settings.ChatProtocol.State == Genesyslab.Platform.Commons.Protocols.ChannelState.Closed)
                    {
                        Settings.ChatProtocol = null;
                        output.MessageCode = "2002";
                        output.Message = "Chat Server protocol is closed Need to Open it.";
                    }
                    else if (Pointel.Interactions.Chat.Core.Util.Settings.ChatProtocol.State == Genesyslab.Platform.Commons.Protocols.ChannelState.Opened)
                    {
                        output.MessageCode = "200";
                        output.Message = "Chat Server protocol is opened.";
                    }
                }
                catch
                {
                    output.MessageCode = "2001";
                    output.Message = "Chat Server protocol is null. Need to create a chat protocol";
                }
            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred while Check Chat Server Status " + generalException.ToString());
                output.MessageCode = "2003";
                output.Message = "Error occurred while Check Chat Server Status";
            }
            return output;
        }

        public OutputValues CreateChatProtocol()
        {
            OutputValues output = OutputValues.GetInstance();
            try
            {
                ChatConnectionManager chatConnectionManager = new ChatConnectionManager();
                output = chatConnectionManager.OpenChatProtocol(Settings.NickName, Settings.PersonID, Settings.PrimaryApplication, Settings.SecondaryApplication);
                if (output.MessageCode == "200")
                {
                    Settings.MessageToClient.NotifyChatProtocol(Settings.ChatProtocol, Settings.NickName);
                }
                else
                {
                    output.MessageCode = "2001";
                    output.Message = "Error occurred while Create Chat Protocol : " + output.Message;
                    logger.Error("Error occurred while Create Chat Protocol");
                }
            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred while Create Chat Protocol " + generalException.ToString());
                output.MessageCode = "2001";
                output.Message = "Error occurred while Create Chat Protocol";
            }
            return output;
        }

        /// <summary>
        /// Does the accept chat interaction.
        /// </summary>
        /// <param name="ticketID">The ticket identifier.</param>
        /// <param name="interactionID">The interaction identifier.</param>
        /// <param name="proxyID">The proxy identifier.</param>
        /// <returns></returns>
        public OutputValues DoAcceptChatInteraction(int ticketID, string interactionID, int proxyID, InteractionServerProtocol ixnProtocol)
        {
            OutputValues output = OutputValues.GetInstance();
            if (ixnProtocol != null)
                Settings.IxnServerProtocol = ixnProtocol;
            try
            {
                output = RequestAcceptInteraction.AcceptChatInteraction(ticketID, interactionID, proxyID);
            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred while Do Accept Chat Interaction " + generalException.ToString());
            }
            return output;
        }

        public OutputValues DoAddCaseDataProperties(string interactionId, int proxyClientID, KeyValueCollection addedProperties)
        {
            OutputValues output = OutputValues.GetInstance();
            try
            {
                output = RequestToChangeProperties.AddCaseInformation(interactionId, proxyClientID, addedProperties);
            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred add case data property " + generalException.ToString());
            }
            return output;
        }

        public OutputValues DoDeleteCaseDataProperties(string interactionId, int proxyClientID, KeyValueCollection deletedProperties)
        {
            OutputValues output = OutputValues.GetInstance();
            try
            {
                output = RequestToChangeProperties.DeleteCaseInformation(interactionId, proxyClientID, deletedProperties);
            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred delete case data property " + generalException.ToString());
            }
            return output;
        }

        public OutputValues DoChangeCaseDataProperties(string interactionId, int proxyClientID, KeyValueCollection addedProperties, KeyValueCollection updatedProperties)
        {
            OutputValues output = OutputValues.GetInstance();
            try
            {
                output = RequestToChangeProperties.ChangeProperties(interactionId, proxyClientID, addedProperties, updatedProperties);
            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred update case data property " + generalException.ToString());
            }
            return output;
        }

        /// <summary>
        /// Does the chat conference interaction.
        /// </summary>
        /// <param name="interactionId">The interaction identifier.</param>
        /// <param name="placeId">The place identifier.</param>
        /// <param name="proxyId">The proxy identifier.</param>
        /// <returns></returns>
        public OutputValues DoChatConferenceInteraction(string interactionId, string agentID, string placeId, string queueName, int proxyId, KeyValueCollection userData)
        {
            OutputValues output = OutputValues.GetInstance();
            try
            {
                output = RequestConferenceInteraction.ConferenceChatSession(interactionId, agentID, placeId, queueName, proxyId, userData);
            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred while Do Chat Conference Interaction " + generalException.ToString());
            }
            return output;
        }

        /// <summary>
        /// Documents the chat consult interaction.
        /// </summary>
        /// <param name="interactionId">The interaction unique identifier.</param>
        /// <param name="agentID">The agent unique identifier.</param>
        /// <param name="placeId">The place unique identifier.</param>
        /// <param name="proxyId">The proxy unique identifier.</param>
        /// <param name="userData">The user data.</param>
        /// <returns></returns>
        public OutputValues DoChatConsultInteraction(string interactionId, string agentID, string placeId, int proxyId, KeyValueCollection userData)
        {
            OutputValues output = OutputValues.GetInstance();
            try
            {
                output = RequestConferenceInteraction.ConsultChatSession(interactionId, agentID, placeId, proxyId, userData);
            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred while Do Chat Consult Interaction " + generalException.ToString());
            }
            return output;
        }

        /// <summary>
        /// Does the chat join.
        /// </summary>
        /// <param name="interactionID">The interaction identifier.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="message">The message.</param>
        /// <param name="userData">The user data.</param>
        /// <returns></returns>
        public OutputValues DOChatJoin(string interactionID, string subject, string message, KeyValueCollection userData)
        {
            OutputValues output = OutputValues.GetInstance();
            try
            {
                output = RequestJoinChatSession.JoinChatSession(interactionID, subject, message, userData);
            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred while Do Chat Join " + generalException.ToString());
            }
            return output;
        }

        /// <summary>
        /// Does the chat transfer interaction.
        /// </summary>
        /// <param name="interactionId">The interaction identifier.</param>
        /// <param name="agentId">The agent identifier.</param>
        /// <param name="placeId">The place identifier.</param>
        /// <param name="proxyId">The proxy identifier.</param>
        /// <param name="queueName">Name of the queue.</param>
        /// <returns></returns>
        public OutputValues DoChatTransferInteraction(string interactionId, string agentId, string placeId, int proxyId, string queueName, KeyValueCollection userData)
        {
            OutputValues output = OutputValues.GetInstance();
            try
            {
                output = RequestTransferInteraction.TransferChatSession(interactionId, agentId, placeId, proxyId, queueName, userData);
            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred while Do Chat Transfer Interaction " + generalException.ToString());
            }
            return output;
        }

        /// <summary>
        /// Documents the consult chat join.
        /// </summary>
        /// <param name="interactionID">The interaction unique identifier.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="message">The message.</param>
        /// <param name="userData">The user data.</param>
        /// <returns></returns>
        public OutputValues DOConsultChatJoin(string interactionID, string subject, string message, KeyValueCollection userData)
        {
            OutputValues output = OutputValues.GetInstance();
            try
            {
                output = RequestJoinChatSession.JoinConsultChatSession(interactionID, subject, message, userData);
            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred while Do Consult Chat Join " + generalException.ToString());
            }
            return output;
        }

        /// <summary>
        /// Does the force release party chat session.
        /// </summary>
        /// <param name="sessionID">The session identifier.</param>
        /// <param name="proxyID">The proxy identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="chatClosingProtocol">The chat closing protocol.</param>
        /// <returns></returns>
        public OutputValues DoForceReleasePartyChatSession(string sessionID, int proxyID, string userId, string messageText)
        {
            OutputValues output = OutputValues.GetInstance();
            try
            {
                output = RequestReleaseInteraction.ReleaseForcePartyChatSession(sessionID, proxyID, userId, messageText);
            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred while Do Force Release Party Chat Session " + generalException.ToString());
            }
            return output;
        }

        public OutputValues DoHoldInteraction(int proxyID, string sessionID, KeyValueCollection extensions)
        {
            OutputValues output = OutputValues.GetInstance();
            try
            {
                output = RequestHoldInteraction.HoldInteraction(proxyID, sessionID, extensions);
            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred while Do Hold Interaction" + generalException.ToString());
            }
            return output;
        }

        /// <summary>
        /// Does the keep alive release party chat session.
        /// </summary>
        /// <param name="sessionID">The session identifier.</param>
        /// <param name="proxyID">The proxy identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="chatClosingProtocol">The chat closing protocol.</param>
        /// <returns></returns>
        public OutputValues DoKeepAliveReleasePartyChatSession(string sessionID, int proxyID, string userId)
        {
            OutputValues output = OutputValues.GetInstance();
            try
            {
                output = RequestReleaseInteraction.ReleaseKeepAlivePartyChatInteraction(sessionID, proxyID, userId);
            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred while Do Keep Alive Party Chat Session " + generalException.ToString());
            }
            return output;
        }

        /// <summary>
        /// Does the leave interaction from conference.
        /// </summary>
        /// <param name="interactionID">The interaction identifier.</param>
        /// <param name="proxyID">The proxy identifier.</param>
        /// <param name="agentID">The agent identifier.</param>
        /// <param name="_chatClosingProtocol">The _chat closing protocol.</param>
        /// <returns></returns>
        public OutputValues DoLeaveInteractionFromConference(string interactionID, int proxyID, string agentID)
        {
            OutputValues output = OutputValues.GetInstance();
            try
            {
                output = RequestLeaveConfInteraction.LeaveInteractionFromConference(interactionID, proxyID, agentID);
            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred while Do Leave Interaction From Conference " + generalException.ToString());
            }
            return output;
        }

        /// <summary>
        /// Does the reject chat interaction.
        /// </summary>
        /// <param name="ticketID">The ticket identifier.</param>
        /// <param name="interactionID">The interaction identifier.</param>
        /// <param name="proxyID">The proxy identifier.</param>
        /// <returns></returns>
        public OutputValues DoRejectChatInteraction(int ticketID, string interactionID, int proxyID, InteractionServerProtocol ixnProtocol, KeyValueCollection ixnData)
        {
            OutputValues output = OutputValues.GetInstance();
            if (ixnProtocol != null)
                Settings.IxnServerProtocol = ixnProtocol;
            try
            {
                output = RequestRejectInteraction.RejectChatInteraction(ticketID, interactionID, proxyID, ixnData);
            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred while Do Reject Chat Interaction " + generalException.ToString());
            }
            return output;
        }

        /// <summary>
        /// Does the release chat interaction.
        /// </summary>
        /// <param name="sessionID">The session identifier.</param>
        /// <param name="proxyID">The proxy identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="chatClosingProtocol">The chat closing protocol.</param>
        /// <param name="queueName">Name of the queue.</param>
        /// <returns></returns>
        public OutputValues DoReleaseChatInteraction(string sessionID, int proxyID, string userId, string queueName)
        {
            OutputValues output = OutputValues.GetInstance();
            try
            {
                output = RequestReleaseInteraction.ReleaseChatInteraction(sessionID, proxyID, userId, queueName);
            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred while Do Release Chat Interaction " + generalException.ToString());
            }
            return output;
        }

        /// <summary>
        /// Does the release party chat session.
        /// </summary>
        /// <param name="sessionID">The session identifier.</param>
        /// <param name="proxyID">The proxy identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="chatClosingProtocol">The chat closing protocol.</param>
        /// <returns></returns>
        public OutputValues DoReleasePartyChatSession(string sessionID, int proxyID, string userId, string messageText)
        {
            OutputValues output = OutputValues.GetInstance();
            try
            {
                output = RequestReleaseInteraction.ReleasePartyChatSession(sessionID, proxyID, userId, messageText);
            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred while Do Release Party Chat Session " + generalException.ToString());
            }
            return output;
        }

        /// <summary>
        /// Documents the send consult message.
        /// </summary>
        /// <param name="sessionID">The session unique identifier.</param>
        /// <param name="messageText">The message text.</param>
        /// <param name="chatProtocol">The chat protocol.</param>
        /// <returns></returns>
        public OutputValues DoSendConsultMessage(string sessionID, string messageText)
        {
            OutputValues output = OutputValues.GetInstance();
            try
            {
                output = RequestSendMessage.SendChatConsultMessage(sessionID, messageText);
            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred while Do Send Consult Message " + generalException.ToString());
            }
            return output;
        }

        /// <summary>
        /// Does the send message.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="sessionID">The session identifier.</param>
        /// <param name="messageText">The message text.</param>
        /// <param name="chatProtocol">The chat protocol.</param>
        /// <returns></returns>
        public OutputValues DoSendMessage(string sessionID, string messageText)
        {
            OutputValues output = OutputValues.GetInstance();
            try
            {
                output = RequestSendMessage.SendChatMessage(sessionID, messageText);
            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred while Do Send Message " + generalException.ToString());
            }
            return output;
        }

        /// <summary>
        /// Does the send notify message.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="sessionID">The session identifier.</param>
        /// <param name="messageText">The message text.</param>
        /// <param name="noticeText">The notice text.</param>
        /// <param name="chatProtocol">The chat protocol.</param>
        /// <returns></returns>
        public OutputValues DoSendNotifyMessage(string sessionID, string noticeText)
        {
            OutputValues output = OutputValues.GetInstance();
            try
            {
                output = RequestNotifyMessage.SendNotifyMessage(sessionID, noticeText);
            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred while Do Send Notify Message " + generalException.ToString());
            }
            return output;
        }

        /// <summary>
        /// Documents the send type start notification.
        /// </summary>
        /// <param name="sessionID">The session unique identifier.</param>
        /// <param name="noticeText">The notice text.</param>
        /// <param name="chatProtocol">The chat protocol.</param>
        /// <returns></returns>
        public OutputValues DoSendTypeStartNotification(string sessionID, string noticeText)
        {
            OutputValues output = OutputValues.GetInstance();
            try
            {
                output = RequestNotifyMessage.SendTypeStartNotification(sessionID, noticeText);
            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred while Do Send Type Start Notification " + generalException.ToString());
            }
            return output;
        }

        /// <summary>
        /// Documents the send type stop notification.
        /// </summary>
        /// <param name="sessionID">The session unique identifier.</param>
        /// <param name="noticeText">The notice text.</param>
        /// <param name="chatProtocol">The chat protocol.</param>
        /// <returns></returns>
        public OutputValues DoSendTypeStopNotification(string sessionID, string noticeText)
        {
            OutputValues output = OutputValues.GetInstance();
            try
            {
                output = RequestNotifyMessage.SendTypeStopNotification(sessionID, noticeText);
            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred while Do Send Type Stop Notification " + generalException.ToString());
            }
            return output;
        }

        /// <summary>
        /// Does the stop interaction.
        /// </summary>
        /// <param name="interactionID">The interaction identifier.</param>
        /// <param name="proxyID">The proxy identifier.</param>
        /// <returns></returns>
        public OutputValues DoStopInteraction(string interactionID, int proxyID)
        {
            OutputValues output = OutputValues.GetInstance();
            try
            {
                output = RequestAgentStopInteraction.StopInteractionProcess(interactionID, proxyID);
            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred while Do Stop Interaction " + generalException.ToString());
            }
            return output;
        }

        public OutputValues DoUpdateCaseDataProperties(string interactionId, int proxyClientID, KeyValueCollection updatedProperties)
        {
            OutputValues output = OutputValues.GetInstance();
            try
            {
                output = RequestToChangeProperties.UpdateProperties(interactionId, proxyClientID, updatedProperties);
            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred update case data property " + generalException.ToString());
            }
            return output;
        }

        public OutputValues InitializeChatMedia(string userName, ConfService configObject, string applicationName)
        {
            OutputValues output = OutputValues.GetInstance();
            ReadConfigObjects read = new ReadConfigObjects();
            //Print DLL Info
            try
            {
                Assembly assemblyVersion = Assembly.LoadFrom(Environment.CurrentDirectory + @"\Pointel.Interactions.Chat.Core.dll");
                if (assemblyVersion != null)
                {
                    logger.Info("*********************************************");
                    logger.Info(assemblyVersion.GetName().Name + " : " + assemblyVersion.GetName().Version);
                    logger.Info("*********************************************");
                }
            }
            catch (Exception versionException)
            {
                logger.Error("Error occurred while getting the version of the chat core library " + versionException.ToString());
                output.MessageCode = "2001";
                output.Message = "Error occurred while Initialize Chat Media : " + versionException.ToString();
            }
            try
            {
                ChatConnectionSettings.GetInstance().ComObject = configObject;
                read.ReadApplicationObject(applicationName);
                Settings.PersonID = Settings.Person.EmployeeID.ToString();
                if (Settings.NickNameFormat != null || Settings.NickNameFormat != string.Empty)
                {
                    GetNickName(Settings.NickNameFormat);
                }
                else
                {
                    if (Settings.Person != null)
                        Settings.NickName = Settings.Person.UserName;
                }
                output.MessageCode = "200";
                output.Message = "Initialized Chat Media Successful.";
            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred while Initialize Chat Media " + generalException.ToString());
                output.MessageCode = "2001";
                output.Message = "Error occurred while Initialize Chat Media : " + generalException.ToString();
            }
            return output;
        }

        public void Subscribe(IChatListener chatListener)
        {
            BasicChatListener basicChatListener = BasicChatListener.GetInstance();
            basicChatListener.SubscribeChatMediaEvents(chatListener);
        }

        /// <summary>
        /// Gets the name of the nick.
        /// </summary>
        /// <param name="nickNameFormat">The nick name format.</param>
        private void GetNickName(string nickNameFormat)
        {
            if (Settings.Person != null)
            {
                if (nickNameFormat.ToLower() == "$agent.fullname$")
                {
                    Settings.NickName = Settings.Person.FirstName + " " + Settings.Person.LastName;

                }
                else if (nickNameFormat.ToLower() == "$agent.firstname$")
                {
                    Settings.NickName = Settings.Person.FirstName;
                }
                else if (nickNameFormat.ToLower() == "$agent.lastname$")
                {
                    Settings.NickName = Settings.Person.LastName;
                }
                else if (nickNameFormat.ToLower() == "$agent.username$")
                {
                    Settings.NickName = Settings.Person.UserName;
                }
                else if (nickNameFormat.ToLower() == "$agent.employeeid$")
                {
                    Settings.NickName = Settings.Person.EmployeeID;
                }
                else
                {
                    Settings.NickName = Settings.Person.UserName;
                }
            }
        }

        #endregion Methods
    }
}