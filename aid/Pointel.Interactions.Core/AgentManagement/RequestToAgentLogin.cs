namespace Pointel.Interactions.Core.AgentManagement
{
    using System;
    using System.Collections.Generic;

    using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel;
    using Genesyslab.Platform.Commons.Protocols;
    using Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events;
    using Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Requests.AgentManagement;

    using Pointel.Interactions.Core.Common;
    using Pointel.Interactions.Core.Util;

    internal class RequestToAgentLogin
    {
        #region Fields

        private Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");

        #endregion Fields

        #region Methods

        /// <summary>
        /// Agents the login.
        /// </summary>
        /// <param name="agentId">The agent identifier.</param>
        /// <param name="placeId">The place identifier.</param>
        /// <param name="proxyId">The proxy identifier.</param>
        /// <param name="chatMedia">The chat media.</param>
        /// <param name="chatMediaState">State of the chat media.</param>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <returns></returns>
        public OutputValues AgentLogin(string agentId, string placeId, int proxyId, int tenantDBID, Dictionary<string, int> mediaList)
        {
            OutputValues output = OutputValues.GetInstance();
            output.Message = string.Empty;
            output.MessageCode = string.Empty;
            output.ErrorCode = 0;
            try
            {
                RequestAgentLogin requestAgentlogin = RequestAgentLogin.Create();
                requestAgentlogin.AgentId = agentId;
                requestAgentlogin.PlaceId = placeId;
                requestAgentlogin.ProxyClientId = proxyId;
                requestAgentlogin.MediaList = new Genesyslab.Platform.Commons.Collections.KeyValueCollection();
                foreach (KeyValuePair<string, int> media in mediaList)
                {
                    requestAgentlogin.MediaList.Add(media.Key, media.Value);
                }
                requestAgentlogin.TenantId = tenantDBID;
                if (Settings.InteractionProtocol != null && Settings.InteractionProtocol.State == ChannelState.Opened)
                {
                    IMessage message = Settings.InteractionProtocol.Request(requestAgentlogin);
                    if (message != null)
                    {
                        switch (message.Id)
                        {

                            case EventAck.MessageId:
                                EventAck eventAgentLogin = (EventAck)message;
                                logger.Info("--------------AgentLogin-----------------");
                                logger.Info("AgentId    :" + agentId);
                                logger.Info("PlaceId    :" + placeId);
                                logger.Info("ProxyClientId    :" + proxyId);
                                foreach (KeyValuePair<string, int> media in mediaList)
                                {
                                    logger.Info("Media Name   :" + media.Key + "\t Media State  :" + media.Value);
                                }
                                logger.Info("TenantId    :" + requestAgentlogin.TenantId);
                                logger.Info("Name    :" + eventAgentLogin.Name.ToString());
                                logger.Info("----------------------------------------");
                                logger.Trace(eventAgentLogin.ToString());
                                output.MessageCode = "200";
                                output.Message = "Agent Login Successful";
                                break;

                            case EventError.MessageId:
                                EventError eventError = (EventError)message;
                                string LoginErrorCode = Convert.ToString(eventError.ErrorCode);
                                string LoginErrorDescription = Convert.ToString(eventError.ErrorDescription);
                                logger.Trace(eventError.ToString());
                                output.ErrorCode = eventError.ErrorCode;
                                output.MessageCode = "2002";
                                output.Message = LoginErrorDescription;
                                break;
                        }

                    }
                    else
                    {
                        output.MessageCode = "2002";
                        output.Message = "Agent Login UnSuccessful";
                    }
                }
                else
                {
                    output.MessageCode = "2001";
                    output.Message = "Interaction Server Protocol is Null";
                    logger.Warn("AgentLogin() : Interaction Server Protocol is Null..");
                }
            }
            catch (Exception commonException)
            {
                logger.Error("Error occurred while Agent Login request " + commonException.ToString());
                output.MessageCode = "2001";
                output.Message = commonException.Message;
            }
            return output;
        }

        #endregion Methods
    }
}