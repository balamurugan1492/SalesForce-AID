using System;
using Genesyslab.Platform.Commons.Protocols;
using Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events;
using Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Requests.AgentManagement;
using Pointel.Interactions.Core.Common;
using Pointel.Interactions.Core.Util;

namespace Pointel.Interactions.Core.AgentManagement
{
    class RequestAgentDoNotDisturbOff
    {
        #region Field Declaration
        private Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");
        #endregion

        #region RequestAgentDoNotDisturbOff
        /// <summary>
        /// Requests the agent application do not disturb off.
        /// </summary>
        /// <param name="proxyClientId">The proxy client identifier.</param>
        /// <returns></returns>
        public OutputValues RequestAgentAppDoNotDisturbOff(int proxyClientId)
        {
            OutputValues output = OutputValues.GetInstance();
            output.Message = string.Empty;
            output.MessageCode = string.Empty;
            output.ErrorCode = 0;
            try
            {
                RequestDoNotDisturbOff requestDoNotDisturbOff = RequestDoNotDisturbOff.Create();
                requestDoNotDisturbOff.ProxyClientId = proxyClientId;
                if (Settings.InteractionProtocol != null && Settings.InteractionProtocol.State == ChannelState.Opened)
                {
                    IMessage message = Settings.InteractionProtocol.Request(requestDoNotDisturbOff);
                    if (message != null)
                    {
                        switch (message.Id)
                        {

                            case EventAck.MessageId:
                                EventAck eventAgentDND = (EventAck)message;
                                logger.Info("---------------RequestAgentDoNotDisturbOff---------------");
                                logger.Info("ProxyClientId    :" + proxyClientId);
                                logger.Info("Name    :" + eventAgentDND.Name.ToString());
                                logger.Info("--------------------------------------------------------");
                                output.MessageCode = "200";
                                output.Message = "Agent Do Not Disturb Off Successful";
                                logger.Trace(eventAgentDND.ToString());
                                break;

                            case EventError.MessageId:
                                EventError eventError = (EventError)message;
                                string LoginErrorCode = Convert.ToString(eventError.ErrorCode);
                                string LoginErrorDescription = Convert.ToString(eventError.ErrorDescription);
                                output.ErrorCode = eventError.ErrorCode;
                                output.MessageCode = "2002";
                                output.Message = "RequestAgentAppDoNotDisturbOff() : " + LoginErrorDescription;
                                logger.Trace(eventError.ToString());
                                break;
                        }

                    }
                    else
                    {
                        output.MessageCode = "2001";
                        output.Message = "Agent Do Not Disturb Off UnSuccessful";
                    }
                }
                else
                {
                    logger.Warn("RequestAgentAppDoNotDisturbOff() : Interaction Server Protocol is Null..");
                }
            }
            catch (Exception commonException)
            {
                logger.Error("Error occurred while Agent Application Do Not Disturb Off request " + commonException.ToString());
                output.MessageCode = "2001";
                output.Message = commonException.Message;
            }
            return output;
        }
        #endregion RequestAgentDoNotDisturbOff
    }
}
