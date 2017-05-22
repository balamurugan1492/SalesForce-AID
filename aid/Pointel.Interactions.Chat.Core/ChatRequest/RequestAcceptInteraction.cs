using System;

namespace Pointel.Interactions.Chat.Core.ChatRequest 
{
    internal class RequestAcceptInteraction
    {
        #region RequestAcceptChat
        /// <summary>
        /// Accepts the chat interaction.
        /// </summary>
        /// <param name="ticketID">The ticket identifier.</param>
        /// <param name="interactionID">The interaction identifier.</param>
        /// <param name="proxyID">The proxy identifier.</param>
        /// <returns></returns>
        public static Pointel.Interactions.Chat.Core.General.OutputValues AcceptChatInteraction(int ticketID, string interactionID, int proxyID)
        {
            Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");
            Pointel.Interactions.Chat.Core.General.OutputValues output = Pointel.Interactions.Chat.Core.General.OutputValues.GetInstance();
            try
            {
                Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Requests.InteractionDelivery.RequestAccept requestAccept = Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Requests.InteractionDelivery.RequestAccept.Create();
                requestAccept.TicketId = ticketID;
                requestAccept.InteractionId = interactionID;
                requestAccept.ProxyClientId = Convert.ToInt32(proxyID);
                //added 27-02-2015
                requestAccept.InitialInFocusState = true;          
                //end
                if (Pointel.Interactions.Chat.Core.Util.Settings.IxnServerProtocol != null && Pointel.Interactions.Chat.Core.Util.Settings.IxnServerProtocol.State == Genesyslab.Platform.Commons.Protocols.ChannelState.Opened)
                {
                    Genesyslab.Platform.Commons.Protocols.IMessage message = Pointel.Interactions.Chat.Core.Util.Settings.IxnServerProtocol.Request(requestAccept);
                    if (message != null)
                    {
                        switch (message.Id)
                        {

                            case Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events.EventAck.MessageId:
                                Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events.EventAck eventAck = (Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events.EventAck)message;                               
                                logger.Info("------------Accepted  Chat Interaction-------------");
                                logger.Info("TicketID  :" + ticketID); 
                                logger.Info("InteractionID  :" + interactionID);
                                logger.Info("ProxyID        :" + proxyID);
                                logger.Info("----------------------------------------------");
                                logger.Trace(eventAck.ToString());
                                output.MessageCode = "200";
                                output.Message = "Accept Chat Interaction Successful";
                                break;

                            case Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events.EventError.MessageId:
                                Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events.EventError eventError = (Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events.EventError)message;
                                 logger.Info("------------Error on Chat Interaction-------------");
                                logger.Info("TicketID  :" + ticketID); 
                                logger.Info("InteractionID  :" + interactionID);
                                logger.Info("ProxyID        :" + proxyID);
                                logger.Info("----------------------------------------------");
                                logger.Trace(eventError.ToString());
                                output.MessageCode = "2001";
                                output.Message = Convert.ToString(eventError.ErrorDescription);
                                logger.Error("Error occurred while accepting the interaction : " + Convert.ToString(eventError.ErrorDescription));
                                break;
                        }

                    }
                    else
                    {
                        output.MessageCode = "2001";
                        output.Message = "Chat Media Agent Not Ready UnSuccessful";
                    }     
                }
                else
                {
                    logger.Warn("AcceptChatInteraction() : Interaction Server protocol is Null..");
                }
               
            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred while Accept the chat request " + generalException.ToString());
                output.MessageCode = "2001";
                output.Message = generalException.Message;
            }
            return output;
        }
        #endregion
    }
}
