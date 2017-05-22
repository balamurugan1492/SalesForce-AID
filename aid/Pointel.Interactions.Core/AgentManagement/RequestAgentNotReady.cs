using System;
using Genesyslab.Platform.Commons.Protocols;
using Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events;
using Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Requests.AgentManagement;

using Pointel.Interactions.Core.Common;
using Pointel.Interactions.Core.Util;

namespace Pointel.Interactions.Core.AgentManagement
{
    internal class RequestAgentNotReady
    {
        #region Field Declaration
        private Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");
        #endregion

        #region RequestAgentNotReady
        /// <summary>
        /// Agents the ready.
        /// </summary>
        /// <param name="proxyClientId">The proxy client identifier.</param>
        /// <param name="mediaType">Type of the media.</param>
        /// <returns></returns>
        public OutputValues AgentNotReady(int proxyClientId, string mediaType)
        {
            OutputValues output = OutputValues.GetInstance();
            output.Message = string.Empty;
            output.MessageCode = string.Empty;
            output.ErrorCode = 0;
            try
            {
                RequestNotReadyForMedia requestAgentNotReady = RequestNotReadyForMedia.Create();
                requestAgentNotReady.ProxyClientId = proxyClientId;
                requestAgentNotReady.MediaTypeName = mediaType;
                if (Settings.InteractionProtocol != null && Settings.InteractionProtocol.State == ChannelState.Opened)
                {
                    IMessage message = Settings.InteractionProtocol.Request(requestAgentNotReady);
                    if (message != null)
                    {
                        switch (message.Id)
                        {

                            case EventAck.MessageId:
                                EventAck eventAgentNotReady = (EventAck)message;
                                logger.Info("------------AgentNotReady---------------");
                                logger.Info("ProxyClientId    :" + proxyClientId);
                                logger.Info("MediaTypeName    :" + mediaType);
                                logger.Info("Name    :" + eventAgentNotReady.Name.ToString());
                                logger.Info("----------------------------------------");
                                logger.Trace(eventAgentNotReady.ToString());
                                output.MessageCode = "200";
                                output.Message = "Agent Not Ready Successful";
                                break;

                            case EventError.MessageId:
                                EventError eventError = (EventError)message;
                                string LoginErrorCode = Convert.ToString(eventError.ErrorCode);
                                string LoginErrorDescription = Convert.ToString(eventError.ErrorDescription);
                                logger.Trace(eventError.ToString());
                                output.ErrorCode = eventError.ErrorCode;
                                output.MessageCode = "2002";
                                output.Message = "AgentNotReady() : " + LoginErrorDescription;
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
                    logger.Warn("AgentNotReady() : Interaction Server Protocol is Null..");
                }
            }
            catch (Exception commonException)
            {
                logger.Error("Error occurred while Agent Not Ready request " + commonException.ToString());
                output.MessageCode = "2001";
                output.Message = commonException.Message;
            }
            return output;
        }
        #endregion
    }
}
