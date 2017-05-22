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
    class RequestSubmitInteraction
    {
        #region Field Declaration
        private Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");
        #endregion

        #region SubmitNewInteraction
        public string SubmitNewInteraction(int tenantId, int proxyClientId, string queuename, KeyValueCollection userdata)
        {
            string interactionID = string.Empty;
            try
            {

                RequestSubmit requestSumbit = RequestSubmit.Create();
                requestSumbit.TenantId = tenantId;
                requestSumbit.ProxyClientId = proxyClientId;
                requestSumbit.Queue = queuename;
                requestSumbit.InteractionType = "Outbound";
                requestSumbit.InteractionSubtype = "OutboundNew";
                requestSumbit.MediaType = "email";
                requestSumbit.UserData = userdata;

                if (Settings.InteractionProtocol != null && Settings.InteractionProtocol.State == ChannelState.Opened)
                {
                    IMessage message = Settings.InteractionProtocol.Request(requestSumbit);
                    if (message != null)
                    {
                        switch (message.Id)
                        {

                            case EventAck.MessageId:
                                EventAck eventAck = (EventAck)message;
                                logger.Info("------------Submit New Email Interaction-------------");
                                logger.Info("TenantID  :" + requestSumbit.TenantId);
                                logger.Info("ProxyClientId  :" + requestSumbit.ProxyClientId);
                                logger.Info("Queue  :" + requestSumbit.Queue);
                                logger.Info("InteractionType        :" + requestSumbit.InteractionType);
                                logger.Info("InteractionSubtype        :" + requestSumbit.InteractionSubtype);
                                logger.Info("MediaType        :" + requestSumbit.MediaType);
                                logger.Info("UserData        :" + requestSumbit.UserData.ToString());
                                logger.Info("----------------------------------------------");
                                logger.Trace(eventAck.ToString());
                                if (eventAck.Extension.ContainsKey("InteractionId"))
                                {
                                    interactionID = eventAck.Extension["InteractionId"].ToString();
                                }
                                break;

                            case EventError.MessageId:
                                EventError eventError = (EventError)message;
                                logger.Info("------------Error on Submitting new  Interaction-------------");
                                logger.Info("TenantID  :" + requestSumbit.TenantId);
                                logger.Info("ProxyClientId  :" + requestSumbit.ProxyClientId);
                                logger.Info("Queue  :" + requestSumbit.Queue);
                                logger.Info("InteractionType        :" + requestSumbit.InteractionType);
                                logger.Info("InteractionSubtype        :" + requestSumbit.InteractionSubtype);
                                logger.Info("MediaType        :" + requestSumbit.MediaType);
                                logger.Info("UserData        :" + requestSumbit.UserData.ToString());
                                logger.Info("----------------------------------------------");
                                logger.Trace(eventError.ToString());
                                interactionID = string.Empty;
                                logger.Error("Error occurred while submittting new interaction : " + Convert.ToString(eventError.ErrorDescription));
                                break;
                        }

                    }
                    else
                    {
                        interactionID = string.Empty;
                    }
                }
                else
                {
                    logger.Warn("PlaceInWorkbin() : Interaction Server protocol is Null..");
                    interactionID = string.Empty;
                }

            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred while submit the new email interaction" + generalException.ToString());
                interactionID = string.Empty;
            }
            return interactionID;
        }
        #endregion

        #region SubmitReplyInteraction
        public string SubmitReplyInteraction(int tenantId, int proxyClientId, string queuename, string parentInteractionId, KeyValueCollection userdata)
        {
            string interactionID = string.Empty;
            try
            {

                RequestSubmit requestSumbit = RequestSubmit.Create();
                requestSumbit.TenantId = tenantId;
                requestSumbit.ProxyClientId = proxyClientId;
                requestSumbit.Queue = queuename;
                requestSumbit.InteractionType = "Outbound";
                requestSumbit.InteractionSubtype = "OutboundReply";
                requestSumbit.MediaType = "email";
                requestSumbit.UserData = userdata;

                requestSumbit.ParentInteractionId = parentInteractionId;

                if (Settings.InteractionProtocol != null && Settings.InteractionProtocol.State == ChannelState.Opened)
                {
                    IMessage message = Settings.InteractionProtocol.Request(requestSumbit);
                    if (message != null)
                    {
                        switch (message.Id)
                        {

                            case EventAck.MessageId:
                                EventAck eventAck = (EventAck)message;
                                logger.Info("------------Submit Reply Email Interaction-------------");
                                logger.Info("TenantID  :" + requestSumbit.TenantId);
                                logger.Info("ProxyClientId  :" + requestSumbit.ProxyClientId);
                                logger.Info("Queue  :" + requestSumbit.Queue);
                                logger.Info("InteractionType        :" + requestSumbit.InteractionType);
                                logger.Info("InteractionSubtype        :" + requestSumbit.InteractionSubtype);
                                logger.Info("MediaType        :" + requestSumbit.MediaType);
                                logger.Info("UserData        :" + requestSumbit.UserData.ToString());
                                logger.Info("----------------------------------------------");
                                logger.Trace(eventAck.ToString());
                                if (eventAck.Extension.ContainsKey("InteractionId"))
                                {
                                    interactionID = eventAck.Extension["InteractionId"].ToString();
                                }
                                break;

                            case EventError.MessageId:
                                EventError eventError = (EventError)message;
                                logger.Info("------------Error on Submitting new  Interaction-------------");
                                logger.Info("TenantID  :" + requestSumbit.TenantId);
                                logger.Info("ProxyClientId  :" + requestSumbit.ProxyClientId);
                                logger.Info("Queue  :" + requestSumbit.Queue);
                                logger.Info("InteractionType        :" + requestSumbit.InteractionType);
                                logger.Info("InteractionSubtype        :" + requestSumbit.InteractionSubtype);
                                logger.Info("MediaType        :" + requestSumbit.MediaType);
                                logger.Info("UserData        :" + requestSumbit.UserData.ToString());
                                logger.Info("----------------------------------------------");
                                logger.Trace(eventError.ToString());
                                interactionID = "Error:" + eventError.ErrorDescription;
                                logger.Error("Error occurred while submittting reply interaction : " + Convert.ToString(eventError.ErrorDescription));
                                break;
                        }

                    }
                    else
                    {
                        interactionID = string.Empty;
                    }
                }
                else
                {
                    logger.Warn("PlaceInWorkbin() : Interaction Server protocol is Null..");
                }

            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred while submit the new email interaction" + generalException.ToString());
                interactionID = string.Empty;
            }
            return interactionID;
        }
        #endregion
    }
}
