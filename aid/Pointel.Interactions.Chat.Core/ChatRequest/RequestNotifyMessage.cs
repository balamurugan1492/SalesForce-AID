using System;

namespace Pointel.Interactions.Chat.Core.ChatRequest
{
    internal class RequestNotifyMessage
    {
        #region RequestNotifyMessage
        /// <summary>
        /// Sends the notify message.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="sessionID">The session identifier.</param>
        /// <param name="messageText">The message text.</param>
        /// <param name="noticeText">The notice text.</param>
        /// <param name="chatProtocol">The chat protocol.</param>
        /// <returns></returns>
        public static Pointel.Interactions.Chat.Core.General.OutputValues SendNotifyMessage(string sessionID, string noticeText)
        {
            Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");
            Pointel.Interactions.Chat.Core.General.OutputValues output = Pointel.Interactions.Chat.Core.General.OutputValues.GetInstance();
            try
            {
                Genesyslab.Platform.WebMedia.Protocols.BasicChat.Requests.RequestNotify requestNotify = Genesyslab.Platform.WebMedia.Protocols.BasicChat.Requests.RequestNotify.Create();
                requestNotify.SessionId = sessionID;
                Genesyslab.Platform.WebMedia.Protocols.BasicChat.NoticeText newNoticeText = Genesyslab.Platform.WebMedia.Protocols.BasicChat.NoticeText.Create();
                newNoticeText.NoticeType = Genesyslab.Platform.WebMedia.Protocols.NoticeType.PushUrl;
                newNoticeText.Text = noticeText;                
                requestNotify.NoticeText = newNoticeText;
                requestNotify.Visibility = Genesyslab.Platform.WebMedia.Protocols.BasicChat.Visibility.All;
                if (Pointel.Interactions.Chat.Core.Util.Settings.ChatProtocol != null && Pointel.Interactions.Chat.Core.Util.Settings.ChatProtocol.State == Genesyslab.Platform.Commons.Protocols.ChannelState.Opened)
                {
                    Pointel.Interactions.Chat.Core.Util.Settings.ChatProtocol.Send(requestNotify);
                    logger.Info("------------SendNotifyMessage-------------");
                    logger.Info("SessionID      :" + sessionID);
                    logger.Info("NoticeText     :" + noticeText);
                    logger.Info("----------------------------------------");
                    logger.Info(requestNotify.ToString());
                    output.MessageCode = "200";
                    output.Message = "Send Chat Message with push URL Successful";
                }
                else
                {
                    logger.Warn("SendNotifyMessage() : Basic Chat Protocol is Null..");
                }
            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred while Send Notify Message request " + generalException.ToString());
                output.MessageCode = "2001";
                output.Message = generalException.Message;
            }
            return output;
        }

        /// <summary>
        /// Sends the type start notification.
        /// </summary>
        /// <param name="sessionID">The session unique identifier.</param>
        /// <param name="noticeText">The notice text.</param>
        /// <param name="chatProtocol">The chat protocol.</param>
        /// <returns></returns>
        public static Pointel.Interactions.Chat.Core.General.OutputValues SendTypeStartNotification(string sessionID, string noticeText)
        {
            Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");
            Pointel.Interactions.Chat.Core.General.OutputValues output = Pointel.Interactions.Chat.Core.General.OutputValues.GetInstance();
            try
            {
                Genesyslab.Platform.WebMedia.Protocols.BasicChat.Requests.RequestNotify requestNotify = Genesyslab.Platform.WebMedia.Protocols.BasicChat.Requests.RequestNotify.Create();
                requestNotify.SessionId = sessionID;
                Genesyslab.Platform.WebMedia.Protocols.BasicChat.NoticeText newNoticeText = Genesyslab.Platform.WebMedia.Protocols.BasicChat.NoticeText.Create();
                newNoticeText.NoticeType = Genesyslab.Platform.WebMedia.Protocols.NoticeType.TypingStarted;
                newNoticeText.Text = noticeText;
                requestNotify.NoticeText = newNoticeText;
                requestNotify.Visibility = Genesyslab.Platform.WebMedia.Protocols.BasicChat.Visibility.All;
                if (Pointel.Interactions.Chat.Core.Util.Settings.ChatProtocol != null && Pointel.Interactions.Chat.Core.Util.Settings.ChatProtocol.State == Genesyslab.Platform.Commons.Protocols.ChannelState.Opened)
                {
                    Pointel.Interactions.Chat.Core.Util.Settings.ChatProtocol.Send(requestNotify);
                    logger.Info("------------Send Type Start Notification-------------");
                    logger.Info("SessionID      :" + sessionID);
                    logger.Info("NoticeText     :" + noticeText);
                    logger.Info("----------------------------------------");
                    logger.Info(requestNotify.ToString());
                    output.MessageCode = "200";
                    output.Message = "Send Type Start Notification Successful";
                }
                else
                {
                    logger.Warn("SendTypeStartNotification() : Basic Chat Protocol is Null..");
                }
            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred while Send Type Start Notification request " + generalException.ToString());
                output.MessageCode = "2001";
                output.Message = generalException.Message;
            }
            return output;
        }

        /// <summary>
        /// Sends the type stop notification.
        /// </summary>
        /// <param name="sessionID">The session unique identifier.</param>
        /// <param name="noticeText">The notice text.</param>
        /// <param name="chatProtocol">The chat protocol.</param>
        /// <returns></returns>
        public static Pointel.Interactions.Chat.Core.General.OutputValues SendTypeStopNotification(string sessionID, string noticeText)
        {
            Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");
            Pointel.Interactions.Chat.Core.General.OutputValues output = Pointel.Interactions.Chat.Core.General.OutputValues.GetInstance();
            try
            {
                Genesyslab.Platform.WebMedia.Protocols.BasicChat.Requests.RequestNotify requestNotify = Genesyslab.Platform.WebMedia.Protocols.BasicChat.Requests.RequestNotify.Create();
                requestNotify.SessionId = sessionID;
                Genesyslab.Platform.WebMedia.Protocols.BasicChat.NoticeText newNoticeText = Genesyslab.Platform.WebMedia.Protocols.BasicChat.NoticeText.Create();
                newNoticeText.NoticeType = Genesyslab.Platform.WebMedia.Protocols.NoticeType.TypingStopped;
                newNoticeText.Text = noticeText;
                requestNotify.NoticeText = newNoticeText;
                requestNotify.Visibility = Genesyslab.Platform.WebMedia.Protocols.BasicChat.Visibility.All;
                if (Pointel.Interactions.Chat.Core.Util.Settings.ChatProtocol != null && Pointel.Interactions.Chat.Core.Util.Settings.ChatProtocol.State == Genesyslab.Platform.Commons.Protocols.ChannelState.Opened)
                {
                    Pointel.Interactions.Chat.Core.Util.Settings.ChatProtocol.Send(requestNotify);
                    logger.Info("------------Send Type Stop Notification-------------");
                    logger.Info("SessionID      :" + sessionID);
                    logger.Info("NoticeText     :" + noticeText);
                    logger.Info("----------------------------------------");
                    logger.Info(requestNotify.ToString());
                    output.MessageCode = "200";
                    output.Message = "Send Type Stop Notification Successful";
                }
                else
                {
                    logger.Warn("SendTypeStopNotification() : Basic Chat Protocol is Null..");
                }
            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred while Send Type Stop Notification request " + generalException.ToString());
                output.MessageCode = "2001";
                output.Message = generalException.Message;
            }
            return output;
        }
        #endregion RequestNotifyMessage
    }
}
