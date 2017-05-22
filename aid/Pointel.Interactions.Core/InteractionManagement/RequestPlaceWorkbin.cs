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

namespace Pointel.Interactions.Core.InteractionManagement
{
    class RequestPlaceinWorkbin
    {
        #region Field Declaration
        private Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");
        #endregion

        #region RequestPlaceWorkbin
        public OutputValues RequestPlaceWorkbin(string interactionId, string agentId, 
            //string placeId,
            string workbin, int proxyId)
        {
            OutputValues output = OutputValues.GetInstance();
            output.Message = string.Empty;
            output.MessageCode = string.Empty;
            output.ErrorCode = 0;
            try
            {
                WorkbinInfo workbinInfo = WorkbinInfo.Create();
                workbinInfo.WorkbinAgentId = agentId;
              //  workbinInfo.WorkbinPlaceId = placeId;
                workbinInfo.WorkbinTypeId = workbin;

                RequestPlaceInWorkbin requestPlaceInWorkbin = RequestPlaceInWorkbin.Create();
                requestPlaceInWorkbin.InteractionId = interactionId;
                requestPlaceInWorkbin.ProxyClientId = proxyId;

                requestPlaceInWorkbin.Workbin = workbinInfo;

                if (Settings.InteractionProtocol != null && Settings.InteractionProtocol.State == ChannelState.Opened)
                {
                    IMessage message = Settings.InteractionProtocol.Request(requestPlaceInWorkbin);
                    if (message != null)
                    {
                        switch (message.Id)
                        {

                            case EventAck.MessageId:
                                EventAck eventAck = (EventAck)message;
                                logger.Info("------------PlaceInWorkbin-------------");
                                logger.Info("AgentID  :" + agentId);
                                logger.Info("InteractionID  :" + interactionId);
                               // logger.Info("PlaceID  :" + placeId);
                                logger.Info("ProxyID        :" + proxyId);
                                logger.Info("Workbin        :" + workbin);
                                logger.Info("----------------------------------------------");
                                logger.Trace(eventAck.ToString());
                                output.MessageCode = "200";
                                output.Message = "Place in Workbin Successful";
                                break;

                            case EventError.MessageId:
                                EventError eventError = (EventError)message;
                                logger.Info("------------Error on Email Interaction-------------");
                                logger.Info("AgentID  :" + agentId);
                                logger.Info("InteractionID  :" + interactionId);
                               // logger.Info("PlaceID  :" + placeId);
                                logger.Info("ProxyID        :" + proxyId);
                                logger.Info("Workbin        :" + workbin);
                                logger.Info("----------------------------------------------");
                                logger.Trace(eventError.ToString());
                                output.MessageCode = "2001";
                                output.Message = Convert.ToString(eventError.ErrorDescription);
                                logger.Error("Error occurred while placeinworkbin : " + Convert.ToString(eventError.ErrorDescription));
                                break;
                        }

                    }
                    else
                    {
                        output.MessageCode = "2001";
                        output.Message = "Place in Workbin UnSuccessful";
                    }
                }
                else
                {
                    logger.Warn("PlaceInWorkbin() : Interaction Server protocol is Null..");
                }

            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred while Accept the request " + generalException.ToString());
                output.MessageCode = "2001";
                output.Message = generalException.Message;
            }
            return output;
        }
        #endregion
    }
}
