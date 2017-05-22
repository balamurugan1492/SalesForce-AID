using System;

namespace Pointel.Interactions.Chat.Core.ChatRequest
{
    internal class RequestRejectInteraction
    {
        #region RequestRejectChat
        /// <summary>
        /// Rejects the chat interaction.
        /// </summary>
        /// <param name="ticketID">The ticket identifier.</param>
        /// <param name="interactionID">The interaction identifier.</param>
        /// <param name="proxyID">The proxy identifier.</param>
        /// <returns></returns>
        public static Pointel.Interactions.Chat.Core.General.OutputValues RejectChatInteraction(int ticketID, string interactionID, int proxyID, Genesyslab.Platform.Commons.Collections.KeyValueCollection data)
        {
            Pointel.Interactions.Chat.Core.General.OutputValues output = Pointel.Interactions.Chat.Core.General.OutputValues.GetInstance();
            Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");
            try
            {
                Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Requests.InteractionDelivery.RequestReject requestReject = Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Requests.InteractionDelivery.RequestReject.Create();
                requestReject.TicketId = ticketID;
                requestReject.InteractionId = interactionID;
                requestReject.ProxyClientId = proxyID;
                requestReject.Extension = data;
                Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.ReasonInfo reasonInfo = Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.ReasonInfo.Create();
                reasonInfo.ReasonDescription = "Agent has reject this interaction";
                requestReject.Reason = reasonInfo;
                if (Pointel.Interactions.Chat.Core.Util.Settings.IxnServerProtocol != null && Pointel.Interactions.Chat.Core.Util.Settings.IxnServerProtocol.State == Genesyslab.Platform.Commons.Protocols.ChannelState.Opened)
                {
                    Genesyslab.Platform.Commons.Protocols.IMessage message = Pointel.Interactions.Chat.Core.Util.Settings.IxnServerProtocol.Request(requestReject);
                    if (message != null)
                    {
                        switch (message.Id)
                        {
                            case Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events.EventAck.MessageId:
                                Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events.EventAck eventInteractionReject = (Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events.EventAck)message;
                                logger.Info("------------RejectChatInteraction-------------");
                                logger.Info("InteractionId  :" + interactionID);
                                logger.Info("ProxyClientId    :" + proxyID);
                                logger.Info("---------------------------------------------");
                                logger.Trace(eventInteractionReject.ToString());
                                output.MessageCode = "200";
                                output.Message = "Reject Chat Interaction Successful";
                                break;

                            case Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events.EventError.MessageId:
                                Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events.EventError eventError = (Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events.EventError)message;
                                string LoginErrorCode = Convert.ToString(eventError.ErrorCode);
                                string LoginErrorDescription = Convert.ToString(eventError.ErrorDescription);
                                logger.Trace(eventError.ToString());
                                output.MessageCode = "2001";
                                output.Message = "RejectChatInteraction() : " + LoginErrorDescription;
                                break;
                        }

                    }
                    else
                    {
                        output.MessageCode = "2001";
                        output.Message = "Chat Media Agent Reject Interaction UnSuccessful";
                    }

                }
                else
                {
                    logger.Warn("RejectChatInteraction() : Interaction Server protocol is Null..");
                }

            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred while Reject Chat Interaction request " + generalException.ToString());
                output.MessageCode = "2001";
                output.Message = generalException.Message;
            }
            return output;
        }
        #endregion
    }
}
