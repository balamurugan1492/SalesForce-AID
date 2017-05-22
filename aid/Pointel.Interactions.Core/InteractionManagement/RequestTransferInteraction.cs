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
using Genesyslab.Platform.Commons.Collections;

namespace Pointel.Interactions.Core.InteractionManagement
{
    class RequestTransferInteraction
    {
        #region Field Declaration
        private Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");
        #endregion

        #region TransferInteraction
        public OutputValues TransferInteractiontoAgent(int proxyClientId, string interactionID, string agentID)
        {
            OutputValues output = OutputValues.GetInstance();
            output.Message = string.Empty;
            output.MessageCode = string.Empty;
            output.ErrorCode = 0;
            try
            {

                RequestTransfer requestTransfer = RequestTransfer.Create();
                requestTransfer.ProxyClientId = proxyClientId;
                requestTransfer.InteractionId = interactionID;
                requestTransfer.AgentId = agentID;
                if (Settings.InteractionProtocol != null && Settings.InteractionProtocol.State == ChannelState.Opened)
                {
                    IMessage message = Settings.InteractionProtocol.Request(requestTransfer);
                    if (message != null)
                    {
                        switch (message.Id)
                        {

                            case EventAck.MessageId:
                                EventAck eventAck = (EventAck)message;
                                logger.Info("------------Transfer Interaction to agent-------------");
                                logger.Info("AgentID  :" + requestTransfer.AgentId);
                                logger.Info("ProxyClientId  :" + requestTransfer.ProxyClientId);
                                logger.Info("InteractionId  :" + requestTransfer.InteractionId);
                                logger.Info("----------------------------------------------");
                                logger.Trace(eventAck.ToString());
                                output.MessageCode = "200";
                                output.Message = "Transfer interaction Successful";
                                break;

                            case EventError.MessageId:
                                EventError eventError = (EventError)message;
                                logger.Info("------------Error on Transferring Interaction to Agent-------------");
                                logger.Info("AgentID  :" + requestTransfer.AgentId);
                                logger.Info("ProxyClientId  :" + requestTransfer.ProxyClientId);
                                logger.Info("InteractionId  :" + requestTransfer.InteractionId);
                                logger.Info("----------------------------------------------");
                                logger.Trace(eventError.ToString());
                                output.MessageCode = "2001";
                                output.Message = Convert.ToString(eventError.ErrorDescription);
                                logger.Error("Error occurred while transferring  interaction : " + Convert.ToString(eventError.ErrorDescription));
                                break;
                        }

                    }
                    else
                    {
                        output.MessageCode = "2001";
                        output.Message = "Transfer Interaction UnSuccessful";
                    }
                }
                else
                {
                    logger.Warn("TransferInteraction() : Interaction Server protocol is Null..");
                }

            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred while transfer the interaction" + generalException.ToString());
                output.MessageCode = "2001";
                output.Message = generalException.Message;
            }
            return output;
        }
        #endregion

        #region TransferInteraction
        public OutputValues TransferInteractiontoQueue(int proxyClientId, string interactionID, string queueName)
        {
            OutputValues output = OutputValues.GetInstance();
            try
            {
                RequestChangeProperties requestChangeProperties = RequestChangeProperties.Create();
                RequestPlaceInQueue requestPlaceInQueue = RequestPlaceInQueue.Create();

                KeyValueCollection attachData = new KeyValueCollection();
                attachData.Add("EmailSkills", queueName);

                requestChangeProperties.ChangedProperties = attachData;
                requestChangeProperties.InteractionId = interactionID;
                requestChangeProperties.ProxyClientId = proxyClientId;

                requestPlaceInQueue.InteractionId = interactionID;
                requestPlaceInQueue.Queue = queueName;
                requestPlaceInQueue.ProxyClientId = proxyClientId;
                requestPlaceInQueue.AddedProperties = attachData;

                if (Settings.InteractionProtocol != null && Settings.InteractionProtocol.State == ChannelState.Opened)
                {
                    IMessage message = Settings.InteractionProtocol.Request(requestPlaceInQueue);
                    if (message != null)
                    {
                        switch (message.Id)
                        {

                            case EventAck.MessageId:
                                EventAck eventAck = (EventAck)message;
                                logger.Info("------------Transfer Interaction By Queue-------------");
                                logger.Info("InteractionId  :" + interactionID);
                                logger.Info("ProxyClientId    :" + proxyClientId);
                                logger.Info("QueueName    :" + queueName);
                                logger.Info("--------------------------------------------");
                                logger.Trace(eventAck.ToString());
                                output.MessageCode = "200";
                                output.Message = "Transfer interaction Successful";
                                break;

                            case EventError.MessageId:
                                EventError eventError = (EventError)message;
                                logger.Info("------------Error on Transfer Interaction By Queue-------------");
                                logger.Info("InteractionId  :" + interactionID);
                                logger.Info("ProxyClientId    :" + proxyClientId);
                                logger.Info("QueueName    :" + queueName);
                                logger.Info("--------------------------------------------");
                                logger.Trace(eventError.ToString());
                                output.MessageCode = "2001";
                                output.Message = Convert.ToString(eventError.ErrorDescription);
                                logger.Error("Error occurred while transferring  interaction : " + Convert.ToString(eventError.ErrorDescription));
                                break;
                        }

                    }
                    else
                    {
                        output.MessageCode = "2001";
                        output.Message = "Transfer Interaction UnSuccessful";
                    }
                    Settings.InteractionProtocol.Send(requestPlaceInQueue);
                    logger.Info("------------Transfer Interaction By Queue-------------");
                    logger.Info("InteractionId  :" + interactionID);
                    logger.Info("ProxyClientId    :" + proxyClientId);
                    logger.Info("QueueName    :" + queueName);
                    logger.Info("--------------------------------------------");
                    logger.Info(requestChangeProperties.ToString());
                    logger.Info(requestPlaceInQueue.ToString());
                }
                else
                {
                    logger.Warn("TransferInteractiontoQueue() : Interaction Server protocol is Null..");
                }

            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred while transfer the interaction" + generalException.ToString());
                output.MessageCode = "2001";
                output.Message = generalException.Message;
            }
            return output;
        }
        #endregion
    }
}
