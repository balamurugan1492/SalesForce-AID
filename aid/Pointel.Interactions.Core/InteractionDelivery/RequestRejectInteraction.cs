using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pointel.Interactions.Core.Common;
using Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Requests.InteractionDelivery;
using Genesyslab.Platform.OpenMedia.Protocols.InteractionServer;
using Pointel.Interactions.Core.Util;
using Genesyslab.Platform.Commons.Protocols;
using Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events;
using Genesyslab.Platform.Commons.Collections;

namespace Pointel.Interactions.Core.InteractionDelivery
{
    class RequestRejectInteraction
    {
        #region Field Declaration
        private Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");
        #endregion

        #region RejectInteraction
        /// <summary>
        /// Rejects the interaction.
        /// </summary>
        /// <param name="ticketID">The ticket identifier.</param>
        /// <param name="interactionID">The interaction identifier.</param>
        /// <param name="proxyID">The proxy identifier.</param>
        /// <returns></returns>
        public OutputValues RejectInteraction(int ticketID, string interactionID, int proxyID, KeyValueCollection data)
        {
            OutputValues output = OutputValues.GetInstance();
            output.Message = string.Empty;
            output.MessageCode = string.Empty;
            output.ErrorCode = 0;
            try
            {
                RequestReject requestReject = RequestReject.Create();
                requestReject.TicketId = ticketID;
                requestReject.InteractionId = interactionID;
                requestReject.ProxyClientId = proxyID;
                requestReject.Extension = data;
                ReasonInfo reasonInfo = ReasonInfo.Create();
                reasonInfo.ReasonDescription = "Agent has reject this interaction";
                requestReject.Reason = reasonInfo;
                if (Settings.InteractionProtocol != null && Settings.InteractionProtocol.State == ChannelState.Opened)
                {
                    IMessage message = Settings.InteractionProtocol.Request(requestReject);
                    if (message != null)
                    {
                        switch (message.Id)
                        {

                            case EventAck.MessageId:
                                EventAck eventInteractionReject = (EventAck)message;
                                logger.Info("------------RejectInteraction-------------");
                                logger.Info("InteractionId  :" + interactionID);
                                logger.Info("ProxyClientId    :" + proxyID);
                                logger.Info("---------------------------------------------");
                                logger.Trace(eventInteractionReject.ToString());
                                output.MessageCode = "200";
                                output.Message = "Reject Interaction Successful";
                                break;

                            case EventError.MessageId:
                                EventError eventError = (EventError)message;
                                string LoginErrorCode = Convert.ToString(eventError.ErrorCode);
                                string LoginErrorDescription = Convert.ToString(eventError.ErrorDescription);
                                logger.Trace(eventError.ToString());
                                output.MessageCode = "2001";
                                output.Message = "RejectInteraction() : " + LoginErrorDescription;
                                break;
                        }

                    }
                    else
                    {
                        output.MessageCode = "2001";
                        output.Message = "Agent Reject Interaction UnSuccessful";
                    }

                }
                else
                {
                    logger.Warn("RejectInteraction() : Interaction Server protocol is Null..");
                }

            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred while Reject Interaction request " + generalException.ToString());
                output.MessageCode = "2001";
                output.Message = generalException.Message;
            }
            return output;
        }
        #endregion
    }
}
