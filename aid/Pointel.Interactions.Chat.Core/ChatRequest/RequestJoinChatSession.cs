using System;

namespace Pointel.Interactions.Chat.Core.ChatRequest
{
    internal class RequestJoinChatSession
    {
        #region RequestJoinChat
        /// <summary>
        /// Joins the chat session.
        /// </summary>
        /// <param name="sessionId">The session identifier.</param>
        /// <param name="subject">The subject.</param>
        /// <returns></returns>
        public static Pointel.Interactions.Chat.Core.General.OutputValues JoinChatSession(string sessionID, string subject, string message, Genesyslab.Platform.Commons.Collections.KeyValueCollection userData)
        {
            Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");
            Pointel.Interactions.Chat.Core.General.OutputValues output = Pointel.Interactions.Chat.Core.General.OutputValues.GetInstance();
            try
            {
                Genesyslab.Platform.WebMedia.Protocols.BasicChat.Requests.RequestJoin requestJoinChat = Genesyslab.Platform.WebMedia.Protocols.BasicChat.Requests.RequestJoin.Create();
                Genesyslab.Platform.WebMedia.Protocols.BasicChat.MessageText newMessageText = Genesyslab.Platform.WebMedia.Protocols.BasicChat.MessageText.Create();
                newMessageText.Text = message;
                requestJoinChat.MessageText = newMessageText;
                requestJoinChat.SessionId = sessionID;
                requestJoinChat.UserData = userData;
                requestJoinChat.Visibility = Genesyslab.Platform.WebMedia.Protocols.BasicChat.Visibility.All;
                requestJoinChat.Subject = subject;
                if (Pointel.Interactions.Chat.Core.Util.Settings.ChatProtocol != null && Pointel.Interactions.Chat.Core.Util.Settings.ChatProtocol.State == Genesyslab.Platform.Commons.Protocols.ChannelState.Opened)
                {
                    Genesyslab.Platform.Commons.Protocols.IMessage iMessage = Pointel.Interactions.Chat.Core.Util.Settings.ChatProtocol.Request(requestJoinChat);
                    if (iMessage != null)
                    {

                        logger.Info("------------JoinChatSession-------------");
                        logger.Info("SessionID  :" + sessionID);
                        logger.Info("subject    :" + subject);
                        logger.Info("MessageText:" + message);
                        logger.Info("----------------------------------------");
                        logger.Trace(iMessage.ToString());
                        output.RequestJoinIMessage = iMessage;
                        output.MessageCode = "200";
                        output.Message = "Join Chat Session Successful";
                    }
                    else
                    {
                        logger.Warn("JoinChatSession() : IMessage is Null..");
                    }
                }
                else
                {
                    logger.Warn("JoinChatSession() : Interaction Server protocol is Null..");
                }
            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred while Join the chat session request " + generalException.ToString());
                output.MessageCode = "2001";
                output.Message = generalException.Message;
            }
            return output;
        }

        /// <summary>
        /// Joins the consult chat session.
        /// </summary>
        /// <param name="sessionID">The session unique identifier.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="message">The message.</param>
        /// <param name="userData">The user data.</param>
        /// <returns></returns>
        public static Pointel.Interactions.Chat.Core.General.OutputValues JoinConsultChatSession(string sessionID, string subject, string message, Genesyslab.Platform.Commons.Collections.KeyValueCollection userData)
        {
            Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");
            Pointel.Interactions.Chat.Core.General.OutputValues output = Pointel.Interactions.Chat.Core.General.OutputValues.GetInstance();
            try
            {
                Genesyslab.Platform.WebMedia.Protocols.BasicChat.Requests.RequestJoin requestJoinChat = Genesyslab.Platform.WebMedia.Protocols.BasicChat.Requests.RequestJoin.Create();
                Genesyslab.Platform.WebMedia.Protocols.BasicChat.MessageText newMessageText = Genesyslab.Platform.WebMedia.Protocols.BasicChat.MessageText.Create();
                newMessageText.Text = message;
                requestJoinChat.MessageText = newMessageText;
                requestJoinChat.SessionId = sessionID;
                requestJoinChat.UserData = userData;
                requestJoinChat.Visibility = Genesyslab.Platform.WebMedia.Protocols.BasicChat.Visibility.Int;
                requestJoinChat.Subject = subject;
                if (Pointel.Interactions.Chat.Core.Util.Settings.ChatProtocol != null && Pointel.Interactions.Chat.Core.Util.Settings.ChatProtocol.State == Genesyslab.Platform.Commons.Protocols.ChannelState.Opened)
                {
                    Genesyslab.Platform.Commons.Protocols.IMessage iMessage = Pointel.Interactions.Chat.Core.Util.Settings.ChatProtocol.Request(requestJoinChat);
                    if (iMessage != null)
                    {

                        logger.Info("------------JoinConsultChatSession-------------");
                        logger.Info("SessionID  :" + sessionID);
                        logger.Info("subject    :" + subject);
                        logger.Info("MessageText:" + message);
                        logger.Info("----------------------------------------");
                        logger.Trace(iMessage.ToString());
                        output.RequestJoinIMessage = iMessage;
                        output.MessageCode = "200";
                        output.Message = "Join Consult Chat Session Successful";
                    }
                    else
                    {
                        logger.Warn("JoinConsultChatSession() : IMessage is Null..");
                    }
                }
                else
                {
                    logger.Warn("JoinConsultChatSession() : Interaction Server protocol is Null..");
                }
            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred while Join the consult chat session request " + generalException.ToString());
                output.MessageCode = "2001";
                output.Message = generalException.Message;
            }
            return output;
        }
        #endregion
    }
}
