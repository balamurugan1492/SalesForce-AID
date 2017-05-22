using System;
using Genesyslab.Platform.Commons.Protocols;
using Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events;
using Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Requests;

using Pointel.Interactions.Core.Common;
using Pointel.Interactions.Core.Util;

namespace Pointel.Interactions.Core.Request
{
    internal class RequestUnRegisterAgent
    {
        #region Field Declaration
        private Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");
        #endregion

        #region RequestUnRegisterAgent
        /// <summary>
        /// Agents the unregister.
        /// </summary>
        /// <param name="proxyId">The proxy identifier.</param>
        /// <returns></returns>
        public OutputValues AgentUnRegister(int proxyId)
        {
            OutputValues output = OutputValues.GetInstance();
            output.Message = string.Empty;
            output.MessageCode = string.Empty;
            output.ErrorCode = 0;
            try
            {
                RequestUnregisterClient requestUnregisterClient = RequestUnregisterClient.Create();
                requestUnregisterClient.ProxyClientId = proxyId;
                if (Settings.InteractionProtocol != null && Settings.InteractionProtocol.State == ChannelState.Opened)
                {
                    IMessage message = Settings.InteractionProtocol.Request(requestUnregisterClient);
                    if (message != null)
                    {
                        switch (message.Id)
                        {

                            case EventAck.MessageId:
                                EventAck eventAgentUnregister = (EventAck)message;
                                logger.Info("------------ChatAgentUnRegister-------------");
                                logger.Info("ProxyClientId    :" + proxyId);
                                logger.Info("Name    :" + eventAgentUnregister.Name.ToString());
                                logger.Info("--------------------------------------------");
                                logger.Trace(eventAgentUnregister.ToString());
                                output.MessageCode = "200";
                                output.Message = "Chat Media Agent UnRegister Successful";
                                break;

                            case EventError.MessageId:
                                EventError eventError = (EventError)message;
                                string LoginErrorCode = Convert.ToString(eventError.ErrorCode);
                                string LoginErrorDescription = Convert.ToString(eventError.ErrorDescription);
                                logger.Trace(eventError.ToString());
                                output.MessageCode = "2001";
                                output.Message = "AgentUnRegister() : " + LoginErrorDescription;
                                break;
                        }

                    }
                    else
                    {
                        output.MessageCode = "2001";
                        output.Message = "Chat Media Agent UnRegister UnSuccessful";
                    }
                }
                else
                {
                    logger.Warn("AgentUnRegister() : Interaction Server Protocol is Null..");
                }
            }
            catch (Exception commonException)
            {
                logger.Error("Error occurred while Chat Agent UnRegister request " + commonException.ToString());
                output.MessageCode = "2001";
                output.Message = commonException.Message;
            }
            return output;
        }
        #endregion
    }
}
