using System;

namespace Pointel.Interactions.Chat.Core.ChatRequest
{
    internal class RequestTransferInteraction
    {
        #region RequestTransferInteraction
        /// <summary>
        /// Transfers the chat session.
        /// </summary>
        /// <param name="interactionId">The interaction identifier.</param>
        /// <param name="agentId">The agent identifier.</param>
        /// <param name="placeId">The place identifier.</param>
        /// <param name="proxyId">The proxy identifier.</param>
        /// <param name="queueName">Name of the queue.</param>
        /// <returns></returns>
        public static Pointel.Interactions.Chat.Core.General.OutputValues TransferChatSession(string interactionId, string agentId, string placeId, int proxyId, string queueName, Genesyslab.Platform.Commons.Collections.KeyValueCollection userData)
        {
            Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");
            Pointel.Interactions.Chat.Core.General.OutputValues output = Pointel.Interactions.Chat.Core.General.OutputValues.GetInstance();
            try
            {
                if (!string.IsNullOrEmpty(placeId) && !string.IsNullOrEmpty(agentId))
                {
                    Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Requests.InteractionManagement.RequestTransfer requestTransfer = Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Requests.InteractionManagement.RequestTransfer.Create();
                    requestTransfer.Extension = userData;
                    requestTransfer.InteractionId = interactionId;
                    requestTransfer.ProxyClientId = proxyId;
                    requestTransfer.PlaceId = placeId;
                    requestTransfer.AgentId = agentId;
                    if (Pointel.Interactions.Chat.Core.Util.Settings.IxnServerProtocol != null && Pointel.Interactions.Chat.Core.Util.Settings.IxnServerProtocol.State == Genesyslab.Platform.Commons.Protocols.ChannelState.Opened)
                    {
                        Genesyslab.Platform.Commons.Protocols.IMessage message = Pointel.Interactions.Chat.Core.Util.Settings.IxnServerProtocol.Request(requestTransfer);
                        if (message != null)
                        {
                            switch (message.Id)
                            {
                                case Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events.EventAck.MessageId:
                                    Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events.EventAck eventack = (Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events.EventAck)message;
                                    logger.Info("------------TransferChatSession-------------");
                                    logger.Info("InteractionId  :" + interactionId);
                                    logger.Info("ProxyClientId    :" + proxyId);
                                    logger.Info("AgentId    :" + agentId);
                                    logger.Info("PlaceId    :" + placeId);
                                    logger.Info("--------------------------------------------");
                                    logger.Trace(eventack.ToString());
                                    output.MessageCode = "200";
                                    output.Message = "Transfer Chat Session Successful";
                                    break;
                                case Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events.EventError.MessageId:
                                    Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events.EventError eventError = (Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events.EventError)message;
                                    string LoginErrorCode = Convert.ToString(eventError.ErrorCode);
                                    string LoginErrorDescription = Convert.ToString(eventError.ErrorDescription);
                                    logger.Info("------------TransferChatSession-------------");
                                    logger.Error(LoginErrorCode + ":" + LoginErrorDescription);
                                    logger.Info("--------------------------------------------");
                                    logger.Trace(eventError.ToString());
                                    output.MessageCode = "2001";
                                    output.Message = LoginErrorDescription;
                                    break;
                            }
                        }
                    }
                    else
                    {
                        logger.Warn("TransferChatSession() : Interaction Server protocol is Null..");
                    }
                }
                else if (!string.IsNullOrEmpty(queueName))
                {
                    Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Requests.InteractionManagement.RequestChangeProperties requestChangeProperties = Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Requests.InteractionManagement.RequestChangeProperties.Create();
                    Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Requests.InteractionManagement.RequestPlaceInQueue requestPlaceInQueue = Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Requests.InteractionManagement.RequestPlaceInQueue.Create();
                    Genesyslab.Platform.Commons.Collections.KeyValueCollection attachData = new Genesyslab.Platform.Commons.Collections.KeyValueCollection();
                    attachData.Add("ChatSkills", queueName);
                    requestChangeProperties.ChangedProperties = attachData;
                    requestChangeProperties.InteractionId = interactionId;
                    requestChangeProperties.ProxyClientId = proxyId;
                    requestPlaceInQueue.InteractionId = interactionId;
                    requestPlaceInQueue.Queue = queueName;
                    requestPlaceInQueue.ProxyClientId = proxyId;
                    requestPlaceInQueue.AddedProperties = attachData;

                    if (Pointel.Interactions.Chat.Core.Util.Settings.IxnServerProtocol != null && Pointel.Interactions.Chat.Core.Util.Settings.IxnServerProtocol.State == Genesyslab.Platform.Commons.Protocols.ChannelState.Opened)
                    {
                        Genesyslab.Platform.Commons.Protocols.IMessage message = Pointel.Interactions.Chat.Core.Util.Settings.IxnServerProtocol.Request(requestPlaceInQueue);
                        if (message != null)
                        {
                            switch (message.Id)
                            {
                                case Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events.EventAck.MessageId:
                                    Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events.EventAck eventAck = (Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events.EventAck)message;
                                    logger.Info("------------Chat Transfer Interaction By Queue-------------");
                                    logger.Info("InteractionId  :" + interactionId);
                                    logger.Info("ProxyClientId    :" + proxyId);
                                    logger.Info("QueueName    :" + queueName);
                                    logger.Info("----------------------------------------------------------");
                                    logger.Trace(eventAck.ToString());
                                    output.MessageCode = "200";
                                    output.Message = "Chat Transfer interaction Successful";
                                    break;

                                case Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events.EventError.MessageId:
                                    Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events.EventError eventError = (Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events.EventError)message;
                                    logger.Info("------------Error on Chat Transfer Interaction By Queue-------------");
                                    logger.Info("InteractionId  :" + interactionId);
                                    logger.Info("ProxyClientId    :" + proxyId);
                                    logger.Info("QueueName    :" + queueName);
                                    logger.Info("-------------------------------------------------------------------");
                                    logger.Trace(eventError.ToString());
                                    output.MessageCode = "2001";
                                    output.Message = Convert.ToString(eventError.ErrorDescription);
                                    logger.Error("Error occurred while chat transferring  interaction : " + Convert.ToString(eventError.ErrorDescription));
                                    break;
                            }
                        }
                        else
                        {
                            output.MessageCode = "2001";
                            output.Message = "Chat Transfer Interaction UnSuccessful";
                        }
                        logger.Info("------------TransferChatSession-------------");
                        logger.Info("InteractionId  :" + interactionId);
                        logger.Info("ProxyClientId    :" + proxyId);
                        logger.Info("QueueName    :" + queueName);
                        logger.Info("--------------------------------------------");
                        logger.Info(requestChangeProperties.ToString());
                        logger.Info(requestPlaceInQueue.ToString());
                    }
                    else
                    {
                        logger.Warn("TransferChatSession() : Interaction Server protocol is Null..");
                    }
                }

            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred while Transfer Chat Session request " + generalException.ToString());
                output.MessageCode = "2001";
                output.Message = generalException.Message;
            }
            return output;
        }
        #endregion
    }
}
