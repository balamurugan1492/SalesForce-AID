using System;

namespace Pointel.Interactions.Chat.Core.ChatRequest
{
    internal class RequestSendMessage
    {
        #region RequestSendMessage
        /// <summary>
        /// Sends the chat message.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="sessionID">The session identifier.</param>
        /// <param name="messageText">The message text.</param>
        /// <param name="chatProtocol">The chat protocol.</param>
        /// <returns></returns>
        public static Pointel.Interactions.Chat.Core.General.OutputValues SendChatMessage(string sessionID, string messageText)
        {
            Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");
            Pointel.Interactions.Chat.Core.General.OutputValues output = Pointel.Interactions.Chat.Core.General.OutputValues.GetInstance();
            try
            {
                Genesyslab.Platform.WebMedia.Protocols.BasicChat.Requests.RequestMessage requestSendMessage = Genesyslab.Platform.WebMedia.Protocols.BasicChat.Requests.RequestMessage.Create();
                requestSendMessage.SessionId = sessionID;
                Genesyslab.Platform.WebMedia.Protocols.BasicChat.MessageText newMessageText = Genesyslab.Platform.WebMedia.Protocols.BasicChat.MessageText.Create();
                newMessageText.Text = messageText;
                requestSendMessage.Visibility = Genesyslab.Platform.WebMedia.Protocols.BasicChat.Visibility.All;
                requestSendMessage.MessageText = newMessageText;
                if (Pointel.Interactions.Chat.Core.Util.Settings.ChatProtocol != null && Pointel.Interactions.Chat.Core.Util.Settings.ChatProtocol.State == Genesyslab.Platform.Commons.Protocols.ChannelState.Opened)
                {
                    Pointel.Interactions.Chat.Core.Util.Settings.ChatProtocol.Send(requestSendMessage);
                    logger.Info("------------SendChatMessage-------------");
                    logger.Info("SessionID      :" + sessionID);
                    logger.Info("MessageText    :" + messageText);
                    logger.Info("----------------------------------------");
                    logger.Info(requestSendMessage.ToString());
                    output.MessageCode = "200";
                    output.Message = "Send Chat Message Successful";
                }
                else
                {
                    logger.Warn("SendChatMessage() : Basic Chat Protocol is Null..");
                }
            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred while Send Chat Message request " + generalException.ToString());
                output.MessageCode = "2001";
                output.Message = generalException.Message;
            }
            return output;
        }


        /// <summary>
        /// Sends the chat consult message.
        /// </summary>
        /// <param name="sessionID">The session unique identifier.</param>
        /// <param name="messageText">The message text.</param>
        /// <param name="chatProtocol">The chat protocol.</param>
        /// <returns></returns>
        public static Pointel.Interactions.Chat.Core.General.OutputValues SendChatConsultMessage(string sessionID, string messageText)
        {
            Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");
            Pointel.Interactions.Chat.Core.General.OutputValues output = Pointel.Interactions.Chat.Core.General.OutputValues.GetInstance();
            try
            {
                Genesyslab.Platform.WebMedia.Protocols.BasicChat.Requests.RequestMessage requestSendMessage = Genesyslab.Platform.WebMedia.Protocols.BasicChat.Requests.RequestMessage.Create();
                requestSendMessage.SessionId = sessionID;
                Genesyslab.Platform.WebMedia.Protocols.BasicChat.MessageText newMessageText = Genesyslab.Platform.WebMedia.Protocols.BasicChat.MessageText.Create();
                newMessageText.Text = messageText;
                requestSendMessage.Visibility = Genesyslab.Platform.WebMedia.Protocols.BasicChat.Visibility.Int;
                requestSendMessage.MessageText = newMessageText;
                if (Pointel.Interactions.Chat.Core.Util.Settings.ChatProtocol != null && Pointel.Interactions.Chat.Core.Util.Settings.ChatProtocol.State == Genesyslab.Platform.Commons.Protocols.ChannelState.Opened)
                {
                    Pointel.Interactions.Chat.Core.Util.Settings.ChatProtocol.Send(requestSendMessage);
                    logger.Info("------------SendChatConsultMessage-------------");
                    logger.Info("SessionID      :" + sessionID);
                    logger.Info("MessageText    :" + messageText);
                    logger.Info("----------------------------------------");
                    logger.Info(requestSendMessage.ToString());
                    output.MessageCode = "200";
                    output.Message = "Send Chat Consult Message Successful";
                }
                else
                {
                    logger.Warn("SendChatConsultMessage() : Basic Chat Protocol is Null..");
                }
            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred while Send Chat Consult Message request " + generalException.ToString());
                output.MessageCode = "2001";
                output.Message = generalException.Message;
            }
            return output;
        }
        #endregion
    }
}
