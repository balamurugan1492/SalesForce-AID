using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pointel.Interactions.Core.Common;
using Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Requests.InteractionDelivery;
using Pointel.Interactions.Core.Util;
using Genesyslab.Platform.Commons.Protocols;
using Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events;
using Genesyslab.Platform.OpenMedia.Protocols.InteractionServer;
using Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Requests.InteractionManagement;
using Genesyslab.Platform.Commons.Collections;

namespace Pointel.Interactions.Core.InteractionManagement
{
    class RequestPullInteraction
    {
        #region Field Declaration
        private Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");
        #endregion

        #region PullInteraction
        public OutputValues PullInteraction(int tenantId,int proxyClientId, KeyValueCollection interactionIDs)
        {
            OutputValues output = OutputValues.GetInstance();
            output.Message = string.Empty;
            output.MessageCode = string.Empty;
            output.ErrorCode = 0;
            try
            {

                RequestPull requestPull = RequestPull.Create();
                requestPull.ViewId = "_system_";
                requestPull.TenantId = tenantId;
                requestPull.ProxyClientId = proxyClientId;
                requestPull.PullParameters = interactionIDs;


                if (Settings.InteractionProtocol != null && Settings.InteractionProtocol.State == ChannelState.Opened)
                {
                    IMessage message = Settings.InteractionProtocol.Request(requestPull);
                    if (message != null)
                    {
                        switch (message.Id)
                        {

                            case EventPulledInteractions.MessageId:
                                logger.Info("------------Pull Interaction-------------");
                                logger.Info("ViewId  :" + requestPull.ViewId);
                                logger.Info("TenantID  :" + requestPull.TenantId);
                                logger.Info("ProxyClientId  :" + requestPull.ProxyClientId);
                                logger.Info("InteractionIds  :" + requestPull.PullParameters.ToString());
                                logger.Info("----------------------------------------------");
                                output.MessageCode = "200";
                                output.IMessage = message;
                                output.Message = "Pull interaction Successful";
                                break;

                            case EventError.MessageId:
                                EventError eventError = (EventError)message;
                                logger.Info("------------Error on Pull Interaction-------------");
                                 logger.Info("ViewId  :" + requestPull.ViewId);
                                logger.Info("TenantID  :" + requestPull.TenantId);
                                logger.Info("ProxyClientId  :" + requestPull.ProxyClientId);
                                logger.Info("InteractionIds  :" + requestPull.PullParameters.ToString());
                                logger.Info("----------------------------------------------");
                                logger.Trace(eventError.ToString());
                                output.MessageCode = "2001";
                                output.Message = Convert.ToString(eventError.ErrorDescription);
                                logger.Error("Error occurred while pull the  interaction : " + Convert.ToString(eventError.ErrorDescription));
                                break;
                        }

                    }
                    else
                    {
                        output.MessageCode = "2001";
                        output.Message = "Pull Interaction UnSuccessful";
                    }
                }
                else
                {
                    logger.Warn("TransferInteraction() : Interaction Server protocol is Null..");
                }

            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred while pull the interaction" + generalException.ToString());
                output.MessageCode = "2001";
                output.Message = generalException.Message;
            }
            return output;
        }
        #endregion
    }
}
