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
    class RequestPlaceinQueue
    {
        #region Field Declaration
        private Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");
        #endregion

        #region RequestPlaceQueue
        public OutputValues RequestPlaceQueue(string interactionId, int proxyId,string queueName)
        {
            OutputValues output = OutputValues.GetInstance();
            output.Message = string.Empty;
            output.MessageCode = string.Empty;
            output.ErrorCode = 0;
            try
            {
                RequestPlaceInQueue requestPlaceInQueue = RequestPlaceInQueue.Create();
                requestPlaceInQueue.InteractionId = interactionId;
                requestPlaceInQueue.Queue = queueName;
                requestPlaceInQueue.ProxyClientId = proxyId;

                if (Settings.InteractionProtocol != null && Settings.InteractionProtocol.State == ChannelState.Opened)
                {
                    IMessage message = Settings.InteractionProtocol.Request(requestPlaceInQueue);
                    if (message != null)
                    {
                        switch (message.Id)
                        {

                            case EventAck.MessageId:
                                EventAck eventAck = (EventAck)message;
                                logger.Info("------------PlaceInQueue-------------");                              
                                logger.Info("InteractionID  :" + interactionId);
                                logger.Info("ProxyID        :" + proxyId);
                                logger.Info("Queue Name        :" + queueName);
                                logger.Info("----------------------------------------------");
                                logger.Trace(eventAck.ToString());
                                output.MessageCode = "200";
                                output.Message = "Place in Queue Successful";
                                break;

                            case EventError.MessageId:
                                EventError eventError = (EventError)message;
                                logger.Info("------------Error on Place In Queue-------------");
                                logger.Info("InteractionID  :" + interactionId);
                                logger.Info("ProxyID        :" + proxyId);
                                logger.Info("Queue Name        :" + queueName);
                                logger.Info("----------------------------------------------");
                                logger.Trace(eventError.ToString());
                                output.MessageCode = "2001";
                                output.Message = Convert.ToString(eventError.ErrorDescription);
                                logger.Error("Error occurred while placeinqueue : " + Convert.ToString(eventError.ErrorDescription));
                                break;
                        }

                    }
                    else
                    {
                        output.MessageCode = "2001";
                        output.Message = "Place in Queue UnSuccessful";
                    }
                }
                else
                {
                    logger.Warn("PlaceInQueue() : Interaction Server protocol is Null..");
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
