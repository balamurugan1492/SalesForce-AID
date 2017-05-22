using System;
using Genesyslab.Platform.Commons.Protocols;
using Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events;
using Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Requests.AgentManagement;

using Pointel.Interactions.Core.Common;
using Pointel.Interactions.Core.Util;

namespace Pointel.Interactions.Core.AgentManagement
{
    internal class RequestAgentReady
    {
        #region Field Declaration
        private Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");
        #endregion

        #region RequestAgentReady
        /// <summary>
        /// Agents the ready.
        /// </summary>
        /// <param name="proxyClientId">The proxy client identifier.</param>
        /// <param name="mediaType">Type of the media.</param>
        /// <returns></returns>
        public OutputValues AgentReady(int proxyClientId, string mediaType)
        {
            OutputValues output = OutputValues.GetInstance();
            output.Message = string.Empty;
            output.MessageCode = string.Empty;
            output.ErrorCode = 0;
            try
            {
                RequestCancelNotReadyForMedia requestAgentReady = RequestCancelNotReadyForMedia.Create();
                requestAgentReady.ProxyClientId = proxyClientId;
                requestAgentReady.MediaTypeName = mediaType;
                if (Settings.InteractionProtocol != null && Settings.InteractionProtocol.State == ChannelState.Opened)
                {
                    IMessage message = Settings.InteractionProtocol.Request(requestAgentReady);
                    if (message != null)
                    {
                        switch (message.Id)
                        {

                            case EventAck.MessageId:
                                EventAck eventAgentReady = (EventAck)message;
                                logger.Info("---------------AgentReady---------------");
                                logger.Info("ProxyClientId    :" + proxyClientId);
                                logger.Info("MediaTypeName    :" + mediaType);
                                logger.Info("Name    :" + eventAgentReady.Name.ToString());
                                logger.Info("----------------------------------------");
                                logger.Trace(eventAgentReady.ToString());
                                output.MessageCode = "200";
                                output.Message = "Agent Ready Successful";
                                break;

                            case EventError.MessageId:
                                EventError eventError = (EventError)message;
                                string LoginErrorCode = Convert.ToString(eventError.ErrorCode);
                                string LoginErrorDescription = Convert.ToString(eventError.ErrorDescription);
                                logger.Trace(eventError.ToString());
                                output.ErrorCode = eventError.ErrorCode;
                                output.MessageCode = "2002";
                                output.Message = "AgentReady() : " + LoginErrorDescription;
                                break;
                        }

                    }
                    else
                    {
                        output.MessageCode = "2001";
                        output.Message = "Agent Ready UnSuccessful";
                    }
                }
                else
                {
                    logger.Warn("AgentReady() : Interaction Server Protocol is Null..");
                }
            }
            catch (Exception commonException)
            {
                logger.Error("Error occurred while Agent Ready request " + commonException.ToString());
                output.MessageCode = "2001";
                output.Message = commonException.Message;
            }
            return output;
        }
        #endregion
    }
}
