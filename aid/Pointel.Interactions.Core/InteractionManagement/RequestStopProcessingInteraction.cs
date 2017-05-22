using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pointel.Interactions.Core.Common;
using Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Requests.InteractionDelivery;
using Pointel.Interactions.Core.Util;
using Genesyslab.Platform.Commons.Protocols;
using Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events;
using Genesyslab.Platform.OpenMedia.Protocols.InteractionServer;
using Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Requests.InteractionManagement;

namespace Pointel.Interactions.Core.InteractionManagement
{
    class RequestStopProcessingInteraction
    {
        #region Field Declaration
        private Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");
        #endregion

        #region StopProcessingInteraction
        public OutputValues StopProcessingInteraction(int proxyClientId, string interactionID)
        {
            OutputValues output = OutputValues.GetInstance();
            output.Message = string.Empty;
            output.MessageCode = string.Empty;
            output.ErrorCode = 0;
            try
            {

                RequestStopProcessing requestStopProcessing = RequestStopProcessing.Create();
                requestStopProcessing.ProxyClientId = proxyClientId;
                requestStopProcessing.InteractionId = interactionID;
                
                if (Settings.InteractionProtocol != null && Settings.InteractionProtocol.State == ChannelState.Opened)
                {
                    IMessage message = Settings.InteractionProtocol.Request(requestStopProcessing);
                    if (message != null)
                    {
                        switch (message.Id)
                        {

                            case EventAck.MessageId:
                                EventAck eventAck = (EventAck)message;
                                logger.Info("------------Stop Processing Interaction-------------");
                                logger.Info("ProxyClientId  :" + requestStopProcessing.ProxyClientId);
                                logger.Info("InteractionId  :" + requestStopProcessing.InteractionId);
                                logger.Info("----------------------------------------------");
                                logger.Trace(eventAck.ToString());
                                output.MessageCode = "200";
                                output.Message = "Stop Processing Successful";
                                break;

                            case EventError.MessageId:
                                EventError eventError = (EventError)message;
                                logger.Info("------------Error on Stop Processing Interaction-------------");
                                logger.Info("ProxyClientId  :" + requestStopProcessing.ProxyClientId);
                                logger.Info("InteractionId  :" + requestStopProcessing.InteractionId);
                                logger.Info("----------------------------------------------");
                                logger.Trace(eventError.ToString());
                                output.MessageCode = "2001";
                                output.Message = Convert.ToString(eventError.ErrorDescription);
                                logger.Error("Error occurred while stop processing the  interaction : " + Convert.ToString(eventError.ErrorDescription));
                                break;
                        }

                    }
                    else
                    {
                        output.MessageCode = "2001";
                        output.Message = "Stop Processing Interaction UnSuccessful";
                    }
                }
                else
                {
                    logger.Warn("StopProcessingInteraction() : Interaction Server protocol is Null..");
                }

            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred while stop processing the interaction" + generalException.ToString());
                output.MessageCode = "2001";
                output.Message = generalException.Message;
            }
            return output;
        }
        #endregion
    }
}
