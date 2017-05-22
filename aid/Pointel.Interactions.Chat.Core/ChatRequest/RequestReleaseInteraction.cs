namespace Pointel.Interactions.Chat.Core.ChatRequest
{
    using System;

    internal class RequestReleaseInteraction
    {
        #region RequestReleaseChat
        /// <summary>
        /// Releases the party chat session.
        /// </summary>
        /// <param name="sessionID">The session identifier.</param>
        /// <param name="proxyID">The proxy identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="chatClosingProtocol">The chat closing protocol.</param>
        /// <returns></returns>
        public static Pointel.Interactions.Chat.Core.General.OutputValues ReleasePartyChatSession(string sessionID, int proxyID, string userId, string messageText)
        {
            Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");
            Pointel.Interactions.Chat.Core.General.OutputValues output = Pointel.Interactions.Chat.Core.General.OutputValues.GetInstance();
            try
            {
                Genesyslab.Platform.WebMedia.Protocols.BasicChat.MessageText newMessageText = Genesyslab.Platform.WebMedia.Protocols.BasicChat.MessageText.Create();
                newMessageText.Text = messageText;
                Genesyslab.Platform.WebMedia.Protocols.BasicChat.Requests.RequestReleaseParty requestReleaseParty = Genesyslab.Platform.WebMedia.Protocols.BasicChat.Requests.RequestReleaseParty.Create(sessionID, userId, Genesyslab.Platform.WebMedia.Protocols.BasicChat.Action.CloseIfNoAgents,
                newMessageText);
                if (Pointel.Interactions.Chat.Core.Util.Settings.ChatProtocol != null && Pointel.Interactions.Chat.Core.Util.Settings.ChatProtocol.State == Genesyslab.Platform.Commons.Protocols.ChannelState.Opened)
                {
                    Pointel.Interactions.Chat.Core.Util.Settings.ChatProtocol.Send(requestReleaseParty);
                    logger.Info("------------ReleasePartyChatSession-------------");
                    logger.Info("SessionID  :" + sessionID);
                    logger.Info("ProxyID    :" + proxyID);
                    logger.Info("UserID    :" + userId);
                    logger.Info("MessageText :" + messageText);
                    logger.Info("------------------------------------------------");
                    logger.Info(requestReleaseParty.ToString());
                    output.MessageCode = "200";
                    output.Message = "Release Party ChatSession Successful";
                }
                else
                {
                    logger.Warn("ReleasePartyChatSession() : Chat Server protocol is Null..");
                }

            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred while Release Party Chat Session request " + generalException.ToString());
                output.MessageCode = "2001";
                output.Message = generalException.Message;
            }
            return output;
        }

        /// <summary>
        /// Releases the keep alive party chat interaction.
        /// </summary>
        /// <param name="sessionID">The session identifier.</param>
        /// <param name="proxyID">The proxy identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="chatClosingProtocol">The chat closing protocol.</param>
        /// <returns></returns>
        public static Pointel.Interactions.Chat.Core.General.OutputValues ReleaseKeepAlivePartyChatInteraction(string sessionID, int proxyID, string userId)
        {
            Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");
            Pointel.Interactions.Chat.Core.General.OutputValues output = Pointel.Interactions.Chat.Core.General.OutputValues.GetInstance();
            try
            {
                Genesyslab.Platform.WebMedia.Protocols.BasicChat.Requests.RequestReleaseParty requestReleaseParty = Genesyslab.Platform.WebMedia.Protocols.BasicChat.Requests.RequestReleaseParty.Create();
                requestReleaseParty.SessionId = sessionID;
                requestReleaseParty.UserId = userId;
                requestReleaseParty.AfterAction = Genesyslab.Platform.WebMedia.Protocols.BasicChat.Action.KeepAlive;

                if (Pointel.Interactions.Chat.Core.Util.Settings.ChatProtocol != null && Pointel.Interactions.Chat.Core.Util.Settings.ChatProtocol.State == Genesyslab.Platform.Commons.Protocols.ChannelState.Opened)
                {
                    Pointel.Interactions.Chat.Core.Util.Settings.ChatProtocol.Send(requestReleaseParty);
                    logger.Info("------------ReleaseKeepAlivePartyChatInteraction-------------");
                    logger.Info("SessionID  :" + sessionID);
                    logger.Info("ProxyID    :" + proxyID);
                    logger.Info("UserID    :" + userId);
                    logger.Info("-------------------------------------------------------------");
                    logger.Info(requestReleaseParty.ToString());
                    output.MessageCode = "200";
                    output.Message = "Release Keep Alive Party Chat Session Successful";
                }
                else
                {
                    logger.Warn("ReleasePartyChatSession() : Chat Server protocol is Null..");
                }

            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred while Release Party Chat Session request " + generalException.ToString());
                output.MessageCode = "2001";
                output.Message = generalException.Message;
            }
            return output;
        }

        /// <summary>
        /// Releases the force party chat session.
        /// </summary>
        /// <param name="sessionID">The session identifier.</param>
        /// <param name="proxyID">The proxy identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="chatClosingProtocol">The chat closing protocol.</param>
        /// <returns></returns>
        public static Pointel.Interactions.Chat.Core.General.OutputValues ReleaseForcePartyChatSession(string sessionID, int proxyID, string userId, string messageText)
        {
            Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");
            Pointel.Interactions.Chat.Core.General.OutputValues output = Pointel.Interactions.Chat.Core.General.OutputValues.GetInstance();
            try
            {
                Genesyslab.Platform.WebMedia.Protocols.BasicChat.MessageText newMessageText = Genesyslab.Platform.WebMedia.Protocols.BasicChat.MessageText.Create();
                newMessageText.Text = messageText;
                Genesyslab.Platform.WebMedia.Protocols.BasicChat.Requests.RequestReleaseParty requestReleaseParty = Genesyslab.Platform.WebMedia.Protocols.BasicChat.Requests.RequestReleaseParty.Create(sessionID, userId, Genesyslab.Platform.WebMedia.Protocols.BasicChat.Action.ForceClose,
                newMessageText);
                if (Pointel.Interactions.Chat.Core.Util.Settings.ChatProtocol != null && Pointel.Interactions.Chat.Core.Util.Settings.ChatProtocol.State == Genesyslab.Platform.Commons.Protocols.ChannelState.Opened)
                {
                    Pointel.Interactions.Chat.Core.Util.Settings.ChatProtocol.Send(requestReleaseParty);
                    logger.Info("------------ReleaseForcePartyChatSession-------------");
                    logger.Info("SessionID  :" + sessionID);
                    logger.Info("ProxyID    :" + proxyID);
                    logger.Info("UserID    :" + userId);
                    logger.Info("-----------------------------------------------------");
                    logger.Info(requestReleaseParty.ToString());
                    output.MessageCode = "200";
                    output.Message = "Release Force Party ChatSession Successful";
                }
                else
                {
                    logger.Warn("ReleaseForcePartyChatSession() : Chat Server protocol is Null..");
                }

            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred while Release Party Chat Session request " + generalException.ToString());
                output.MessageCode = "2001";
                output.Message = generalException.Message;
            }
            return output;
        }

        /// <summary>
        /// Releases the chat interaction.
        /// </summary>
        /// <param name="sessionID">The session identifier.</param>
        /// <param name="proxyID">The proxy identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="chatClosingProtocol">The chat closing protocol.</param>
        /// <param name="queueName">Name of the queue.</param>
        /// <returns></returns>
        public static Pointel.Interactions.Chat.Core.General.OutputValues ReleaseChatInteraction(string sessionID, int proxyID, string userId, string queueName)
        {
            Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");
            Pointel.Interactions.Chat.Core.General.OutputValues output = Pointel.Interactions.Chat.Core.General.OutputValues.GetInstance();
            Genesyslab.Platform.Commons.Protocols.IMessage requestPlaceInQueueMsg = null;
            try
            {
                Genesyslab.Platform.WebMedia.Protocols.BasicChat.Requests.RequestReleaseParty requestReleaseParty = Genesyslab.Platform.WebMedia.Protocols.BasicChat.Requests.RequestReleaseParty.Create();
                requestReleaseParty.AfterAction = Genesyslab.Platform.WebMedia.Protocols.BasicChat.Action.ForceClose;
                requestReleaseParty.SessionId = sessionID;
                if (Pointel.Interactions.Chat.Core.Util.Settings.ChatProtocol != null && Pointel.Interactions.Chat.Core.Util.Settings.ChatProtocol.State == Genesyslab.Platform.Commons.Protocols.ChannelState.Opened)
                {
                    Pointel.Interactions.Chat.Core.Util.Settings.ChatProtocol.Send(requestReleaseParty);
                    logger.Info("------------ReleaseChatInteraction-------------");
                    logger.Info("SessionID  :" + sessionID);
                    logger.Info("ProxyID    :" + proxyID);
                    logger.Info("UserID    :" + userId);
                    logger.Info("-----------------------------------------------------");
                    logger.Info(requestReleaseParty.ToString());
                    output.MessageCode = "200";
                    output.Message = "Release Force Party ChatSession Successful";
                }
                Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Requests.InteractionManagement.RequestPlaceInQueue requestPlaceInQueue = Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Requests.InteractionManagement.RequestPlaceInQueue.Create();
                requestPlaceInQueue.InteractionId = sessionID;
                requestPlaceInQueue.ProxyClientId = proxyID;
                requestPlaceInQueue.Queue = queueName;
                if (Pointel.Interactions.Chat.Core.Util.Settings.IxnServerProtocol != null && Pointel.Interactions.Chat.Core.Util.Settings.IxnServerProtocol.State == Genesyslab.Platform.Commons.Protocols.ChannelState.Opened)
                {
                    requestPlaceInQueueMsg = Pointel.Interactions.Chat.Core.Util.Settings.IxnServerProtocol.Request(requestPlaceInQueue);
                    if (requestPlaceInQueueMsg != null)
                    {
                        switch (requestPlaceInQueueMsg.Id)
                        {
                            case Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events.EventAck.MessageId:
                                // chatClosingProtocol.Close();
                                break;
                        }
                        logger.Info("------------ReleaseChatInteraction-------------");
                        logger.Info("SessionID  :" + sessionID);
                        logger.Info("ProxyID    :" + proxyID);
                        logger.Info("UserID    :" + userId);
                        logger.Info("------------------------------------------------");
                        logger.Info(requestReleaseParty.ToString());
                        output.MessageCode = "200";
                        output.Message = "Release Chat Interaction Successful";
                    }
                    else
                    {
                        output.MessageCode = "2001";
                        output.Message = "Don't Release Chat Interaction Successful";
                    }
                }
                else
                {
                    logger.Warn("ReleaseChatInteraction() : Interaction Server protocol is Null..");
                }
            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred while Release Chat Interaction request " + generalException.ToString());
                output.MessageCode = "2001";
                output.Message = generalException.Message;
            }
            return output;
        }
        #endregion
    }
}
