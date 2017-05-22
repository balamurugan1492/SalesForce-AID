using System;
using Genesyslab.Platform.Commons.Protocols;
using Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events;
using Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Requests.AgentManagement;
using Pointel.Interactions.Core.Common;
using Pointel.Interactions.Core.Util;

namespace Pointel.Interactions.Core.AgentManagement
{
    internal class RequestAgentDoNotDisturbOn
    {
        #region Field Declaration
        private Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");
        #endregion

        #region RequestAgentDoNotDisturbOn
        /// <summary>
        /// Requests the agent application do not disturb on.
        /// </summary>
        /// <param name="proxyClientId">The proxy client identifier.</param>
        /// <returns></returns>
        public OutputValues RequestAgentAppDoNotDisturbOn(int proxyClientId) 
        {
            OutputValues output = OutputValues.GetInstance();
            output.Message = string.Empty;
            output.MessageCode = string.Empty;
            output.ErrorCode = 0;
            try
            {
                RequestDoNotDisturbOn requestDoNotDisturbOn = RequestDoNotDisturbOn.Create();
                requestDoNotDisturbOn.ProxyClientId = proxyClientId;
                if (Settings.InteractionProtocol != null && Settings.InteractionProtocol.State == ChannelState.Opened)
                {
                    IMessage message = Settings.InteractionProtocol.Request(requestDoNotDisturbOn);
                    if (message != null)
                    {
                        switch (message.Id)
                        {

                            case EventAck.MessageId:
                                EventAck eventAgentDND = (EventAck)message;
                                logger.Info("---------------RequestAgentDoNotDisturbOn---------------");
                                logger.Info("ProxyClientId    :" + proxyClientId);
                                logger.Info("Name    :" + eventAgentDND.Name.ToString());
                                logger.Info("-------------------------------------------------------");
                                logger.Trace(eventAgentDND.ToString());
                                output.MessageCode = "200";
                                output.Message = "Agent Do Not Disturb On Successful";
                                break;

                            case EventError.MessageId:
                                EventError eventError = (EventError)message;
                                string LoginErrorCode = Convert.ToString(eventError.ErrorCode);
                                string LoginErrorDescription = Convert.ToString(eventError.ErrorDescription);
                                logger.Trace(eventError.ToString());
                                output.ErrorCode = eventError.ErrorCode;
                                output.MessageCode = "2002";
                                output.Message = "RequestAgentAppDoNotDisturbOn() : " + LoginErrorDescription;
                                break;
                        }

                    }
                    else
                    {
                        output.MessageCode = "2001";
                        output.Message = "Request DND UnSuccessful";
                    }
                }
                else
                {
                    logger.Warn("RequestAgentAppDoNotDisturbOn() : Interaction Server Protocol is Null..");
                }
            }
            catch (Exception commonException)
            {
                logger.Error("Error occurred while Agent Application Do Not Disturb On request " + commonException.ToString());
                output.MessageCode = "2001";
                output.Message = commonException.Message;
            }
            return output;
        }
        #endregion RequestAgentDoNotDisturbOn
    }
}
