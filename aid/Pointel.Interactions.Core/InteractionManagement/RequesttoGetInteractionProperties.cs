using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pointel.Interactions.Core.Common;
using Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Requests.InteractionManagement;
using Pointel.Interactions.Core.Util;
using Genesyslab.Platform.Commons.Protocols;
using Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events;

namespace Pointel.Interactions.Core.InteractionManagement
{
    class RequesttoGetInteractionProperties
    {
        #region Field Declaration
        private Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");
        #endregion

        #region GetInteractionProperties
        public OutputValues GetInteractionProperties(int proxyClientId, string interactionId)
        {
            OutputValues output = OutputValues.GetInstance();
            output.Message = string.Empty;
            output.MessageCode = string.Empty;
            output.ErrorCode = 0;
            try
            {
                RequestGetInteractionProperties requestGetInteractionProperties = RequestGetInteractionProperties.Create();
                requestGetInteractionProperties.InteractionId = interactionId;
                requestGetInteractionProperties.ProxyClientId = proxyClientId;
               

                if (Settings.InteractionProtocol != null && Settings.InteractionProtocol.State == ChannelState.Opened)
                {
                    IMessage message = Settings.InteractionProtocol.Request(requestGetInteractionProperties);
                    if (message != null)
                    {
                        switch (message.Id)
                        {

                            case EventInteractionProperties.MessageId:
                                EventInteractionProperties eventInteractionProperties = (EventInteractionProperties)message;
                                logger.Info("------------Get Interaction Properties-------------");
                                logger.Info("InteractionID  :" + interactionId);
                                logger.Info("ProxyID        :" + proxyClientId);                                
                                logger.Info("----------------------------------------------");
                                logger.Trace(eventInteractionProperties.ToString());
                                output.MessageCode = "200";
                                output.Message = "Place in Queue Successful";
                                output.IMessage = message;
                                break;

                            case EventError.MessageId:
                                EventError eventError = (EventError)message;
                                logger.Info("------------Error on getting interaction properties-------------");
                                logger.Info("InteractionID  :" + interactionId);
                                logger.Info("ProxyID        :" + proxyClientId);                                
                                logger.Info("----------------------------------------------");
                                logger.Trace(eventError.ToString());
                                output.MessageCode = "2001";
                                output.Message = Convert.ToString(eventError.ErrorDescription);
                                logger.Warn("Error occurred while getting interaction properties : " + Convert.ToString(eventError.ErrorDescription));
                                break;
                        }

                    }
                    else
                    {
                        output.MessageCode = "2001";
                        output.Message = "Get Interaction Properties UnSuccessful";
                    }
                }
                else
                {
                    logger.Warn("GetInteractionProperties() : Interaction Server protocol is Null..");
                }

            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred while getting interaction properties " + generalException.ToString());
                output.MessageCode = "2001";
                output.Message = generalException.Message;
            }
            return output;
        }
        #endregion
    }
}
