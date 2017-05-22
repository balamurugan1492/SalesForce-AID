using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pointel.Interactions.Core.Common;
using Genesyslab.Platform.Commons.Collections;
using Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Requests.AgentManagement;
using Pointel.Interactions.Core.Util;
using Genesyslab.Platform.Commons.Protocols;
using Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events;

namespace Pointel.Interactions.Core.AgentManagement
{
    internal class RequestToSubscribe
    {
        #region Field Declaration
        private Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");
        #endregion

        #region RequestToSubscribe

        /// <summary>
        /// Subscribes the specified proxy client identifier.
        /// </summary>
        /// <param name="proxyClientId">The proxy client identifier.</param>
        /// <param name="topicList">The topic list.</param>
        /// <returns></returns>
        public OutputValues Subscribe(int proxyClientId, KeyValueCollection topicList)
        { 
            OutputValues output = OutputValues.GetInstance();
            output.Message = string.Empty;
            output.MessageCode = string.Empty;
            output.ErrorCode = 0;
            try
            {
                RequestSubscribe requestSubscribe = RequestSubscribe.Create();
                requestSubscribe.ProxyClientId = proxyClientId;
                requestSubscribe.PsTopicList = topicList;
                if (Settings.InteractionProtocol != null && Settings.InteractionProtocol.State == ChannelState.Opened)
                {
                    IMessage message = Settings.InteractionProtocol.Request(requestSubscribe);
                    if (message != null)
                    {
                        switch (message.Id)
                        {
                            case EventAck.MessageId:
                                EventAck eventSubscribe = (EventAck)message;
                                logger.Info("-------------RequestSubscribe----------------");
                                logger.Info("ProxyClientId    :" + proxyClientId);
                                logger.Info("PsTopicList    :" + topicList.ToString());
                                logger.Info("Name    :" + eventSubscribe.Name.ToString());
                                logger.Info("--------------------------------------------");
                                logger.Trace(eventSubscribe.ToString());
                                output.MessageCode = "200";
                                output.Message = "Subscribe Successful";
                                break;

                            case EventError.MessageId:
                                EventError eventError = (EventError)message;
                                string LoginErrorCode = Convert.ToString(eventError.ErrorCode);
                                string LoginErrorDescription = Convert.ToString(eventError.ErrorDescription);
                                logger.Trace(eventError.ToString());
                                output.ErrorCode = eventError.ErrorCode;
                                output.MessageCode = "2002";
                                output.Message = "Subscribe() : " + LoginErrorDescription;
                                break;
                        }

                    }
                    else
                    {
                        output.MessageCode = "2001";
                        output.Message = "Subscribe UnSuccessful";
                    }
                }
                else
                {
                    logger.Warn("Subscribe() : Interaction Server Protocol is Null..");
                }
            }
            catch (Exception commonException)
            {
                logger.Error("Error occurred while Subscribe request " + commonException.ToString());
                output.MessageCode = "2001";
                output.Message = commonException.Message;
            }
            return output;
        }
        #endregion
    }
}
