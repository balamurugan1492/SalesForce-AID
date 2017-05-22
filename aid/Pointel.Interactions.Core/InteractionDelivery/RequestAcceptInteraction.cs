using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pointel.Interactions.Core.Common;
using Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Requests.InteractionDelivery;
using Pointel.Interactions.Core.Util;
using Genesyslab.Platform.Commons.Protocols;
using Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events;


namespace Pointel.Interactions.Core.InteractionDelivery
{
    class RequestAcceptInteraction
    {
        #region Field Declaration
        private Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");
        #endregion

        #region AcceptInteraction
        /// <summary>
        /// Accepts the interaction.
        /// </summary>
        /// <param name="ticketID">The ticket identifier.</param>
        /// <param name="interactionID">The interaction identifier.</param>
        /// <param name="proxyID">The proxy identifier.</param>
        /// <returns></returns>
        public OutputValues AcceptInteraction(int ticketID, string interactionID, int proxyID)
        {
            OutputValues output = OutputValues.GetInstance();
            output.Message = string.Empty;
            output.MessageCode = string.Empty;
            output.ErrorCode = 0;
            try
            {
                RequestAccept requestAccept = RequestAccept.Create();
                requestAccept.TicketId = ticketID;
                requestAccept.InteractionId = interactionID;
                requestAccept.ProxyClientId = Convert.ToInt32(proxyID);
                //added 27-02-2015
                requestAccept.InitialInFocusState = true;
                //end
                if (Settings.InteractionProtocol != null && Settings.InteractionProtocol.State == ChannelState.Opened)
                {
                    IMessage message = Settings.InteractionProtocol.Request(requestAccept);
                    if (message != null)
                    {
                        switch (message.Id)
                        {

                            case EventAck.MessageId:
                                EventAck eventAck = (EventAck)message;
                                logger.Info("------------Accepted  Interaction-------------");
                                logger.Info("TicketID  :" + ticketID);
                                logger.Info("InteractionID  :" + interactionID);
                                logger.Info("ProxyID        :" + proxyID);
                                logger.Info("----------------------------------------------");
                                logger.Trace(eventAck.ToString());
                                output.MessageCode = "200";
                                output.Message = "Accept Interaction Successful";
                                break;

                            case EventError.MessageId:
                                EventError eventError = (EventError)message;
                                logger.Info("------------Error on Email Interaction-------------");
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
                        output.Message = "Agent Not Ready UnSuccessful";
                    }
                }
                else
                {
                    logger.Warn("AcceptChatInteraction() : Interaction Server protocol is Null..");
                }

            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred while Accept the request " + generalException.ToString());
                output.MessageCode = "2001";
                output.Message = generalException.Message;
            }
            return output;
        }
        #endregion
       
    }
}
