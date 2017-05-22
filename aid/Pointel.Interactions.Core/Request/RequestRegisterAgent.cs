using System;
using Genesyslab.Platform.Commons.Protocols;
using Genesyslab.Platform.OpenMedia.Protocols.InteractionServer;
using Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events;
using Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Requests;

using Pointel.Interactions.Core.Common;
using Pointel.Interactions.Core.Util;

namespace Pointel.Interactions.Core.Request
{
    internal class RequestRegisterAgent
    {
        #region Field Declaration
        private Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");
        #endregion

        #region RequestRegisterAgent
        /// <summary>
        /// Agents the register.
        /// </summary>
        /// <param name="clientName">Name of the client.</param>
        /// <returns></returns>
        public OutputValues AgentRegister(string clientName)
        {
            OutputValues output = OutputValues.GetInstance();
            output.Message = string.Empty;
            output.MessageCode = string.Empty;
            output.ErrorCode = 0;
            try
            {
                RequestRegisterClient requestRegisterClient = RequestRegisterClient.Create();
                requestRegisterClient.ClientName = clientName;
                requestRegisterClient.ClientType = InteractionClient.AgentApplication;
                requestRegisterClient.ProxyClientId = 0;
                requestRegisterClient.Extension = null;
                if (Settings.InteractionProtocol != null && Settings.InteractionProtocol.State == ChannelState.Opened)
                {
                    IMessage message = Settings.InteractionProtocol.Request(requestRegisterClient);

                    if (message != null)
                    {
                        switch (message.Id)
                        {
                            case EventAck.MessageId:
                                EventAck eventAgentRegister = (EventAck)message;
                                logger.Info("------------AgentRegister-------------");
                                logger.Info("ClientName    :" + clientName);
                                logger.Info("--------------------------------------");
                                logger.Trace(eventAgentRegister.ToString());
                                output.MessageCode = "200";
                                output.ProxyID = eventAgentRegister.ProxyClientId;
                                output.Message = "Agent Register Successful"; 
                                break;

                            case EventError.MessageId:
                                EventError eventError = (EventError)message;
                                string LoginErrorCode = Convert.ToString(eventError.ErrorCode);
                                string LoginErrorDescription = Convert.ToString(eventError.ErrorDescription);
                                logger.Trace(eventError.ToString());
                                output.MessageCode = "2001";
                                output.Message = "AgentRegister() : " + LoginErrorDescription;
                                break;
                        }

                    }
                    else
                    {
                        output.MessageCode = "2001";
                        output.Message = "Agent Register UnSuccessful";
                    }
                }
                else
                {
                    logger.Warn("AgentRegister() : Interaction Server Protocol is Null..");
                }
            }
            catch (Exception commonException)
            {
                logger.Error("Error occurred while Register request " + commonException.ToString());
                output.MessageCode = "2001";
                output.Message = commonException.Message;
            }
            return output;
        }
        #endregion
    }
}
