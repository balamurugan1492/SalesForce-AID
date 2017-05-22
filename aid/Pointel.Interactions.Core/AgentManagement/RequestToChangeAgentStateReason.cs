using System;
using Genesyslab.Platform.Commons.Collections;
using Genesyslab.Platform.Commons.Protocols;
using Genesyslab.Platform.OpenMedia.Protocols.InteractionServer;
using Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events;
using Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Requests.AgentManagement;

using Pointel.Interactions.Core.Common;
using Pointel.Interactions.Core.Util;

namespace Pointel.Interactions.Core.AgentManagement
{
    internal class RequestToChangeAgentStateReason
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
        public OutputValues AgentNotReadyWithReason(int proxyClientId, string mediaType, string reason,string code) 
        {
            OutputValues output = OutputValues.GetInstance();
            output.Message = string.Empty;
            output.MessageCode = string.Empty;
            output.ErrorCode = 0;
            try
            {
                KeyValueCollection ReasonCode = new KeyValueCollection();
                if (!string.IsNullOrEmpty(reason))
                {
                    ReasonCode.Add("Name", reason);
                    ReasonCode.Add("Code", code);
                }
                RequestChangeMediaStateReason requestChangeMediaStateReason = RequestChangeMediaStateReason.Create(proxyClientId, ReasonCode, mediaType, ReasonInfo.Create(mediaType, reason, Convert.ToInt32(code)));
                //requestChangeMediaStateReason.ProxyClientId = proxyClientId;
                //requestChangeMediaStateReason.MediaTypeName = mediaType;
                
                //requestChangeMediaStateReason.Extension = ReasonCode;
                //requestChangeMediaStateReason.Reason = ReasonInfo.Create(mediaType, reason, Convert.ToInt32(code));
           
                if (Settings.InteractionProtocol != null && Settings.InteractionProtocol.State == ChannelState.Opened)
                {
                    IMessage message = Settings.InteractionProtocol.Request(requestChangeMediaStateReason);
                    if (message != null)
                    {
                        switch (message.Id)
                        {

                            case EventAck.MessageId:
                                EventAck eventAgentNotReady = (EventAck)message;                               
                                logger.Info("----------------AgentNotReadyWithReason-------------");
                                logger.Info("ProxyClientId    :" + proxyClientId);
                                logger.Info("MediaTypeName    :" + mediaType);
                                logger.Info("Name    :" + eventAgentNotReady.Name.ToString());
                                logger.Info("Reason Name    :"+reason);
                                logger.Info("Reason Code    :" + code);
                                logger.Info("----------------------------------------------------");
                                logger.Trace(eventAgentNotReady.ToString());
                                output.MessageCode = "200";
                                output.Message = "Agent Not Ready with reason Successful";
                                break;

                            case EventError.MessageId:
                                EventError eventError = (EventError)message;
                                string LoginErrorCode = Convert.ToString(eventError.ErrorCode);
                                string LoginErrorDescription = Convert.ToString(eventError.ErrorDescription);
                                logger.Trace(eventError.ToString());
                                output.ErrorCode = eventError.ErrorCode;
                                output.MessageCode = "2002";
                                output.Message = "AgentNotReadyWithReason() : " + LoginErrorDescription;
                                break;
                        }

                    }
                    else
                    {
                        output.MessageCode = "2001";
                        output.Message = "Agent Not Ready with reason UnSuccessful";
                    }
                }
                else
                {
                    logger.Warn("AgentNotReadyWithReason() : Interaction Server Protocol is Null..");
                }
            }
            catch (Exception commonException)
            {
                logger.Error("Error occurred while Agent Not Ready with reason request " + commonException.ToString());
                output.MessageCode = "2001";
                output.Message = commonException.Message;
            }
            return output;
        }
        #endregion
    }
}
