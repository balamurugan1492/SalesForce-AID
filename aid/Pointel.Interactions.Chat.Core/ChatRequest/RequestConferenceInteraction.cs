using System;

namespace Pointel.Interactions.Chat.Core.ChatRequest
{
    internal class RequestConferenceInteraction
    {
        #region RequestConferenceInteraction
        /// <summary>
        /// Conferences the chat session.
        /// </summary>
        /// <param name="interactionId">The interaction identifier.</param>
        /// <param name="placeId">The place identifier.</param>
        /// <param name="proxyId">The proxy identifier.</param>
        /// <returns></returns>
        public static Pointel.Interactions.Chat.Core.General.OutputValues ConferenceChatSession(string interactionId, string agentID, string placeId, string queueName, int proxyId, Genesyslab.Platform.Commons.Collections.KeyValueCollection userData)
        {
            Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");
            Pointel.Interactions.Chat.Core.General.OutputValues output = Pointel.Interactions.Chat.Core.General.OutputValues.GetInstance();
            try
            {
                if (!string.IsNullOrEmpty(agentID) && !string.IsNullOrEmpty(placeId))
                {
                    Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Requests.InteractionManagement.RequestConference requestConference = Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Requests.InteractionManagement.RequestConference.Create();
                    requestConference.PlaceId = placeId;
                    requestConference.AgentId = agentID;
                    requestConference.InteractionId = interactionId;
                    requestConference.ProxyClientId = proxyId;
                    requestConference.Extension = userData;
                    requestConference.VisibilityMode = Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.VisibilityMode.Conference;
                    if (Pointel.Interactions.Chat.Core.Util.Settings.IxnServerProtocol != null && Pointel.Interactions.Chat.Core.Util.Settings.IxnServerProtocol.State == Genesyslab.Platform.Commons.Protocols.ChannelState.Opened)
                    {
                        Genesyslab.Platform.Commons.Protocols.IMessage message = Pointel.Interactions.Chat.Core.Util.Settings.IxnServerProtocol.Request(requestConference);
                        if (message != null)
                        {
                            switch (message.Id)
                            {

                                case Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events.EventAck.MessageId:
                                    Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events.EventAck eventack = (Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events.EventAck)message;
                                    logger.Info("------------RequestConferenceInteraction-------------");
                                    logger.Info("InteractionId  :" + interactionId);
                                    logger.Info("ProxyClientId    :" + proxyId);
                                    logger.Info("AgentId    :" + agentID);
                                    logger.Info("PlaceId    :" + placeId);
                                    logger.Info("--------------------------------------------");
                                    logger.Trace(eventack.ToString());
                                    output.MessageCode = "200";
                                    output.Message = "Conference Chat Session Successful";
                                    break;
                                case Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events.EventError.MessageId:
                                    Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events.EventError eventError = (Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events.EventError)message;
                                    string LoginErrorCode = Convert.ToString(eventError.ErrorCode);
                                    string LoginErrorDescription = Convert.ToString(eventError.ErrorDescription);
                                    logger.Info("------------RequestConferenceInteraction-------------");
                                    logger.Error(LoginErrorCode + ":" + LoginErrorDescription);
                                    logger.Info("--------------------------------------------");
                                    logger.Trace(eventError.ToString());
                                    output.MessageCode = "2001";
                                    output.Message = LoginErrorDescription;
                                    break;
                            }
                        }
                    }
                    else
                    {
                        logger.Warn("ConferenceChatSession() : Interaction Server protocol is Null..");
                    }
                }
                else if (!string.IsNullOrEmpty(queueName))
                {
                    Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Requests.InteractionManagement.RequestChangeProperties requestChangeProperties = Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Requests.InteractionManagement.RequestChangeProperties.Create();
                    Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Requests.InteractionManagement.RequestPlaceInQueue requestPlaceInQueue = Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Requests.InteractionManagement.RequestPlaceInQueue.Create();
                    Genesyslab.Platform.Commons.Collections.KeyValueCollection attachData = new Genesyslab.Platform.Commons.Collections.KeyValueCollection();
                    attachData.Add("ChatSkills", queueName);
                    requestChangeProperties.ChangedProperties = attachData;
                    requestChangeProperties.InteractionId = interactionId;
                    requestChangeProperties.ProxyClientId = proxyId;
                    requestPlaceInQueue.InteractionId = interactionId;
                    requestPlaceInQueue.Queue = queueName;
                    requestPlaceInQueue.ProxyClientId = proxyId;
                    requestPlaceInQueue.AddedProperties = attachData;

                    if (Pointel.Interactions.Chat.Core.Util.Settings.IxnServerProtocol != null && Pointel.Interactions.Chat.Core.Util.Settings.IxnServerProtocol.State == Genesyslab.Platform.Commons.Protocols.ChannelState.Opened)
                    {
                        Genesyslab.Platform.Commons.Protocols.IMessage message = Pointel.Interactions.Chat.Core.Util.Settings.IxnServerProtocol.Request(requestPlaceInQueue);
                        if (message != null)
                        {
                            switch (message.Id)
                            {
                                case Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events.EventAck.MessageId:
                                    Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events.EventAck eventAck = (Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events.EventAck)message;
                                    logger.Info("------------Chat Conference Interaction By Queue-------------");
                                    logger.Info("InteractionId  :" + interactionId);
                                    logger.Info("ProxyClientId    :" + proxyId);
                                    logger.Info("QueueName    :" + queueName);
                                    logger.Info("------------------------------------------------------------");
                                    logger.Trace(eventAck.ToString());
                                    output.MessageCode = "200";
                                    output.Message = "Chat Conference interaction Successful";
                                    break;

                                case Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events.EventError.MessageId:
                                    Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events.EventError eventError = (Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events.EventError)message;
                                    logger.Info("------------Error on Chat Conference Interaction By Queue-------------");
                                    logger.Info("InteractionId  :" + interactionId);
                                    logger.Info("ProxyClientId    :" + proxyId);
                                    logger.Info("QueueName    :" + queueName);
                                    logger.Info("----------------------------------------------------------------------");
                                    logger.Trace(eventError.ToString());
                                    output.MessageCode = "2001";
                                    output.Message = Convert.ToString(eventError.ErrorDescription);
                                    logger.Error("Error occurred while chat Conference  interaction : " + Convert.ToString(eventError.ErrorDescription));
                                    break;
                            }
                        }
                        else
                        {
                            output.MessageCode = "2001";
                            output.Message = "Chat Conference Interaction UnSuccessful";
                        }
                        logger.Info("------------ConferenceChatSession-------------");
                        logger.Info("InteractionId  :" + interactionId);
                        logger.Info("ProxyClientId    :" + proxyId);
                        logger.Info("QueueName    :" + queueName);
                        logger.Info("--------------------------------------------");
                        logger.Info(requestChangeProperties.ToString());
                        logger.Info(requestPlaceInQueue.ToString());
                    }
                    else
                    {
                        logger.Warn("ConferenceChatSession() : Interaction Server protocol is Null..");
                    }
                }

            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred while Conference Chat Session request " + generalException.ToString());
                output.MessageCode = "2001";
                output.Message = generalException.Message;
            }
            return output;
        }

        /// <summary>
        /// Consults the chat session.
        /// </summary>
        /// <param name="interactionId">The interaction unique identifier.</param>
        /// <param name="agentID">The agent unique identifier.</param>
        /// <param name="placeId">The place unique identifier.</param>
        /// <param name="proxyId">The proxy unique identifier.</param>
        /// <param name="userData">The user data.</param>
        /// <returns></returns>
        public static Pointel.Interactions.Chat.Core.General.OutputValues ConsultChatSession(string interactionId, string agentID, string placeId, int proxyId, Genesyslab.Platform.Commons.Collections.KeyValueCollection userData)
        {
            Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");
            Pointel.Interactions.Chat.Core.General.OutputValues output = Pointel.Interactions.Chat.Core.General.OutputValues.GetInstance();
            try
            {
                Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Requests.InteractionManagement.RequestConference requestConference = Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Requests.InteractionManagement.RequestConference.Create();
                requestConference.PlaceId = placeId;
                requestConference.AgentId = agentID;
                requestConference.InteractionId = interactionId;
                requestConference.ProxyClientId = proxyId;
                requestConference.Extension = userData;
                requestConference.Extension.Add("ConsultUserData", "separate");
                requestConference.VisibilityMode = Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.VisibilityMode.Coach;
                if (Pointel.Interactions.Chat.Core.Util.Settings.IxnServerProtocol != null && Pointel.Interactions.Chat.Core.Util.Settings.IxnServerProtocol.State == Genesyslab.Platform.Commons.Protocols.ChannelState.Opened)
                {
                    Genesyslab.Platform.Commons.Protocols.IMessage message = Pointel.Interactions.Chat.Core.Util.Settings.IxnServerProtocol.Request(requestConference);
                    if (message != null)
                    {
                        switch (message.Id)
                        {

                            case Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events.EventAck.MessageId:
                                Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events.EventAck eventack = (Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events.EventAck)message;
                                logger.Info("------------ConsultChatSession-------------");
                                logger.Info("InteractionId  :" + interactionId);
                                logger.Info("ProxyClientId    :" + proxyId);
                                logger.Info("AgentId    :" + agentID);
                                logger.Info("PlaceId    :" + placeId);
                                logger.Info("--------------------------------------------");
                                logger.Trace(eventack.ToString());
                                output.MessageCode = "200";
                                output.Message = "Consult Chat Session Successful";
                                break;
                            case Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events.EventError.MessageId:
                                Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events.EventError eventError = (Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events.EventError)message;
                                string LoginErrorCode = Convert.ToString(eventError.ErrorCode);
                                string LoginErrorDescription = Convert.ToString(eventError.ErrorDescription);
                                logger.Info("------------ConsultChatSession-------------");
                                logger.Error(LoginErrorCode + ":" + LoginErrorDescription);
                                logger.Info("--------------------------------------------");
                                logger.Trace(eventError.ToString());
                                output.MessageCode = "2001";
                                output.Message = LoginErrorDescription;
                                break;
                        }
                    }
                }
                else
                {
                    logger.Warn("ConferenceChatSession() : Interaction Server protocol is Null..");
                }

            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred while Conference Chat Session request " + generalException.ToString());
                output.MessageCode = "2001";
                output.Message = generalException.Message;
            }
            return output;
        }

        #endregion
    }
}
