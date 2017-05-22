using System;
using Genesyslab.Platform.Commons.Protocols;
using Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events;
using Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Requests.AgentManagement;

using Pointel.Interactions.Core.Common;
using Pointel.Interactions.Core.Util;

namespace Pointel.Interactions.Core.AgentManagement
{
    internal class RequestToAgentLogout
    {
        #region Field Declaration
        private Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");
        #endregion

        #region RequestToAgentLogout
        /// <summary>
        /// Agents the logout.
        /// </summary>
        /// <param name="proxyId">The proxy identifier.</param>
        /// <returns></returns>
        public OutputValues AgentLogout(int proxyId)
        {
            OutputValues output = OutputValues.GetInstance();
            output.Message = string.Empty;
            output.MessageCode = string.Empty;
            output.ErrorCode = 0;
            try
            {
                RequestAgentLogout requestAgentlogout = RequestAgentLogout.Create();
                requestAgentlogout.ProxyClientId = proxyId;
                
                if (Settings.InteractionProtocol != null && Settings.InteractionProtocol.State == ChannelState.Opened)
                {
                    IMessage message = Settings.InteractionProtocol.Request(requestAgentlogout);
                    if (message != null)
                    {
                        switch (message.Id)
                        {

                            case EventAck.MessageId:
                                EventAck eventAgentLogout = (EventAck)message;
                                logger.Info("-------------AgentLogout----------------");
                                logger.Info("ProxyClientId    :" + proxyId);
                                logger.Info("Name    :" + eventAgentLogout.Name.ToString());
                                logger.Info("----------------------------------------");
                                logger.Trace(eventAgentLogout.ToString());
                                output.MessageCode = "200";
                                output.Message = "Agent Logout Successful";
                                break;

                            case EventError.MessageId:
                                EventError eventError = (EventError)message;
                                string LoginErrorCode = Convert.ToString(eventError.ErrorCode);
                                string LoginErrorDescription = Convert.ToString(eventError.ErrorDescription);
                                logger.Trace(eventError.ToString());
                                output.ErrorCode = eventError.ErrorCode;
                                output.MessageCode = "2002";
                                output.Message = "AgentLogout() : " + LoginErrorDescription;
                                break;
                        }

                    }
                    else
                    {
                        output.MessageCode = "2001";
                        output.Message = "Agent Logout UnSuccessful";
                    }
                }
                else
                {
                    logger.Warn("AgentLogout() : Interaction Server Protocol is Null..");
                }
            }
            catch (Exception commonException)
            {
                logger.Error("Error occurred while Agent Logout request " + commonException.ToString());
                output.MessageCode = "2001";
                output.Message = commonException.Message;
            }
            return output;
        }
        #endregion
    }
}
