namespace Pointel.Interactions.Core
{
    using System;
    using System.Collections.Generic;

    using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel;
    using Genesyslab.Platform.Commons.Collections;

    using Pointel.Configuration.Manager;
    using Pointel.Interactions.Core.AgentManagement;
    using Pointel.Interactions.Core.Application;
    using Pointel.Interactions.Core.Common;
    using Pointel.Interactions.Core.ConnectionManager;
    using Pointel.Interactions.Core.InteractionDelivery;
    using Pointel.Interactions.Core.InteractionManagement;
    using Pointel.Interactions.Core.Listener;
    using Pointel.Interactions.Core.Request;
    using Pointel.Interactions.Core.Util;

    #region Delegates

    public delegate void NotifyInteractionServerState(bool isOpen);

    #endregion Delegates

    public class InteractionService
    {
        #region Fields

        public static IInteractionServices messageToClient = null;

        private Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");

        #endregion Fields

        #region Events

        public static event NotifyInteractionServerState InteractionServerNotifier;

        #endregion Events

        #region Methods

        public static void NotifyInteractionServerState(bool isOpen)
        {
            if (InteractionServerNotifier != null)
                InteractionServerNotifier.Invoke(isOpen);
        }

        public OutputValues AcceptInteraction(int ticketID, string interactionID, int proxyID)
        {
            RequestAcceptInteraction requestAcceptInteraction = new RequestAcceptInteraction();
            return requestAcceptInteraction.AcceptInteraction(ticketID, interactionID, proxyID);
        }

        public OutputValues AddCaseDataProperties(string interactionId, int proxyClientID, KeyValueCollection addedProperties)
        {
            OutputValues output = OutputValues.GetInstance();
            try
            {
                RequestToChangeProperties requestToChangeProperties = new RequestToChangeProperties();
                output = requestToChangeProperties.AddCaseInformation(interactionId, proxyClientID, addedProperties);
            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred add case data property " + generalException.ToString());
            }
            return output;
        }

        public OutputValues AddMedia(int proxyClientId, string mediaType)
        {
            RequestToAddMedia requestToAddMedia = new RequestToAddMedia();
            return requestToAddMedia.AddMedia(proxyClientId, mediaType);
        }

        public OutputValues AgentDoNotDisturbOff(int proxyClientId)
        {
            RequestAgentDoNotDisturbOff requestAgentDoNotDisturbOff = new RequestAgentDoNotDisturbOff();
            return requestAgentDoNotDisturbOff.RequestAgentAppDoNotDisturbOff(proxyClientId);
        }

        public OutputValues AgentDoNotDisturbOn(int proxyClientId)
        {
            RequestAgentDoNotDisturbOn requestAgentDoNotDisturbOn = new RequestAgentDoNotDisturbOn();
            return requestAgentDoNotDisturbOn.RequestAgentAppDoNotDisturbOn(proxyClientId);
        }

        public OutputValues AgentLogin(string agentId, string placeId, int proxyId, int tenantDBID, Dictionary<string, int> mediaList)
        {
            RequestToAgentLogin requestToAgentLogin = new RequestToAgentLogin();
            return requestToAgentLogin.AgentLogin(agentId, placeId, proxyId, tenantDBID, mediaList);
        }

        public OutputValues AgentLogout(int proxyId)
        {
            RequestToAgentLogout requestToAgentLogout = new RequestToAgentLogout();
            return requestToAgentLogout.AgentLogout(proxyId);
        }

        public void AgentMediaStateSubscriber(IInteractionServices clientObject)
        {
            InteractionManager.GetInstance().SubscribeAgentMediaStatus(clientObject);
        }

        public OutputValues AgentNotReady(int proxyClientId, string mediaType)
        {
            RequestAgentNotReady requestAgentNotReady = new RequestAgentNotReady();
            return requestAgentNotReady.AgentNotReady(proxyClientId, mediaType);
        }

        public OutputValues AgentNotReadyWithReason(int proxyClientId, string mediaType, string reason, string code)
        {
            RequestToChangeAgentStateReason requestToChangeAgentStateReason = new RequestToChangeAgentStateReason();
            return requestToChangeAgentStateReason.AgentNotReadyWithReason(proxyClientId, mediaType, reason, code);
        }

        public OutputValues AgentReady(int proxyClientId, string mediaType)
        {
            RequestAgentReady requestAgentReady = new RequestAgentReady();
            return requestAgentReady.AgentReady(proxyClientId, mediaType);
        }

        /// <summary>
        /// Connects the interaction server.
        /// </summary>
        /// <param name="primaryServer">The primary server.</param>
        /// <param name="secondaryServer">The secondary server.</param>
        /// <param name="addpClientTimeout">The addp client timeout.</param>
        /// <param name="addpServerTimeout">The addp server timeout.</param>
        public OutputValues ConnectInteractionServer(string ixnAppName, string agentID)
        {
            OutputValues output = null;
            ReadConfigObjects readConfigObjects = new ReadConfigObjects();
            try
            {

                readConfigObjects.ReadApplicationObject(ixnAppName);
                InteractionConnectionManager createProtocol = new InteractionConnectionManager();
                output = createProtocol.ConnectInteractionServer(Settings.PrimaryApplication, Settings.SecondaryApplication, agentID);
                if (output.MessageCode == "200")
                {
                    messageToClient.NotifyInteractionProtocol(Settings.InteractionProtocol);
                }
                ////Passing to client
                //messageToClient.NotifyInteractionMediaStatus(output);

            }
            catch (Exception commonException)
            {
                logger.Error("InteractionLibrary:ConnectInteractionServer:" + commonException.ToString());
            }
            return output;
        }

        public OutputValues DeleteCaseDataProperties(string interactionId, int proxyClientID, KeyValueCollection deletedProperties)
        {
            OutputValues output = OutputValues.GetInstance();
            try
            {
                RequestToChangeProperties requestToDeleteProperties = new RequestToChangeProperties();
                output = requestToDeleteProperties.DeletedProperties(interactionId, proxyClientID, deletedProperties);
            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred while deleting case data" + generalException.ToString());
            }
            return output;
        }

        public OutputValues GetInteractionProperties(int proxyClientId, string interactionID)
        {
            OutputValues output = OutputValues.GetInstance();
            try
            {
                RequesttoGetInteractionProperties requesttoGetInteractionProperties = new RequesttoGetInteractionProperties();
                output = requesttoGetInteractionProperties.GetInteractionProperties(proxyClientId, interactionID);
            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred while getting interaction properties " + generalException.ToString());
            }
            return output;
        }

        public OutputValues GetWorkbin(int tenantId, int proxyId)
        {
            return GetWorkbinTypeInformation.GetWorkbinTypes(tenantId, proxyId);
        }

        public OutputValues GetWorkbinContent(string agentId, string placeId, string workbinName, int proxyId)
        {
            GetWorkbinContent workbinContent = new GetWorkbinContent();
            return workbinContent.getWorkbinContent(agentId, placeId, workbinName, proxyId);
        }

        public OutputValues PlaceInQueue(string interactionId, int proxyId, string queueName)
        {
            RequestPlaceinQueue requestPlaceQueue = new RequestPlaceinQueue();
            return requestPlaceQueue.RequestPlaceQueue(interactionId, proxyId, queueName);
        }

        public OutputValues PlaceInWorkbin(string interactionId, string agentId
            ,// string placeId,
            string workbin, int proxyId)
        {
            RequestPlaceinWorkbin requestPlaceWorkbin = new RequestPlaceinWorkbin();
            return requestPlaceWorkbin.RequestPlaceWorkbin(interactionId, agentId,
                // placeId,
                workbin, proxyId);
        }

        public OutputValues PullInteraction(int tenantId, int proxyClientId, KeyValueCollection interactionIDs)
        {
            OutputValues output = OutputValues.GetInstance();
            try
            {
                RequestPullInteraction requestTransferInteraction = new RequestPullInteraction();
                output = requestTransferInteraction.PullInteraction(tenantId, proxyClientId, interactionIDs);
            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred while pull the interaction " + generalException.ToString());
            }
            return output;
        }

        public OutputValues RegisterClient(string clientName)
        {
            RequestRegisterAgent requestRegisterAgent = new RequestRegisterAgent();
            OutputValues output = requestRegisterAgent.AgentRegister(clientName);
            return output;
        }

        public OutputValues RejectInteraction(int ticketID, string interactionID, int proxyID, KeyValueCollection data)
        {
            RequestRejectInteraction requestRejectInteraction = new RequestRejectInteraction();
            return requestRejectInteraction.RejectInteraction(ticketID, interactionID, proxyID, data);
        }

        public OutputValues RemoveMedia(int proxyClientId, string mediaType)
        {
            RequestToRemoveMedia requestToRemoveMedia = new RequestToRemoveMedia();
            return requestToRemoveMedia.RemoveMedia(proxyClientId, mediaType);
        }

        public bool RequestTeamWorkbinNotifications(int proxyClientId, List<string> workbinType, List<string> worbinEmpId, string workbinGroupId, string WorkbinPlaceId, string workbinPlaceGroupId)
        {
            try
            {
                RequestNotifyWorkbin requestNotifyWorkbin = new RequestNotifyWorkbin();
                if (workbinType != null && worbinEmpId != null)
                    foreach (string workbinName in workbinType)
                        foreach (string empId in worbinEmpId)
                            requestNotifyWorkbin.NotifyWorbin(proxyClientId, workbinName, empId, workbinGroupId, WorkbinPlaceId, workbinPlaceGroupId);
                return true;
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred as :" + ex.Message);
                return false;
            }
        }

        public OutputValues RequestWorkbinNotifications(int proxyClientId, string workbinType, string worbinEmpId, string workbinGroupId, string WorkbinPlaceId, string workbinPlaceGroupId)
        {
            RequestNotifyWorkbin requestNotifyWorkbin = new RequestNotifyWorkbin();
            return requestNotifyWorkbin.NotifyWorbin(proxyClientId, workbinType, worbinEmpId, workbinGroupId, WorkbinPlaceId, workbinPlaceGroupId);
        }

        public OutputValues StopProcessingInteraction(int proxyClientId, string interactionID)
        {
            OutputValues output = OutputValues.GetInstance();
            try
            {
                RequestStopProcessingInteraction requestStopProcessingInteraction = new RequestStopProcessingInteraction();
                output = requestStopProcessingInteraction.StopProcessingInteraction(proxyClientId, interactionID);
            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred while stop processing the interaction " + generalException.ToString());
            }
            return output;
        }

        public string SubmitNewInteraction(int tenantId, int proxyClientId, string queuename, KeyValueCollection userdata)
        {
            string interactionID = string.Empty;
            try
            {
                RequestSubmitInteraction requestSubmitInteraction = new RequestSubmitInteraction();
                interactionID = requestSubmitInteraction.SubmitNewInteraction(tenantId, proxyClientId, queuename, userdata);
            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred add case data property " + generalException.ToString());
            }
            return interactionID;
        }

        public string SubmitReplyInteraction(int tenantId, int proxyClientId, string queuename, string parentInteractionId, KeyValueCollection userdata)
        {
            string interactionID = string.Empty;
            try
            {
                RequestSubmitInteraction requestSubmitInteraction = new RequestSubmitInteraction();
                interactionID = requestSubmitInteraction.SubmitReplyInteraction(tenantId, proxyClientId, queuename, parentInteractionId, userdata);
            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred add case data property " + generalException.ToString());
            }
            return interactionID;
        }

        public OutputValues Subscribe(int proxyClientId, KeyValueCollection topicList)
        {
            RequestToSubscribe requestToSubscribe = new RequestToSubscribe();
            return requestToSubscribe.Subscribe(proxyClientId, topicList);
        }

        /// <summary>
        /// This method holds the client object.  And appropriate channel interactions will be notified to the subscriber
        /// </summary>
        /// <param name="types">The types.</param>
        /// <param name="clientObject">The client object.</param>
        public void Subscriber(InteractionTypes types, IInteractionServices clientObject)
        {
            // Settings settings = new Settings();
            if (!Settings.subscriberObject.ContainsKey(InteractionTypes.Email) && types == InteractionTypes.Email)
                Settings.subscriberObject.Add(types, clientObject);
            if (!Settings.subscriberObject.ContainsKey(InteractionTypes.Chat) && types == InteractionTypes.Chat)
                Settings.subscriberObject.Add(types, clientObject);
            if (!Settings.subscriberObject.ContainsKey(InteractionTypes.OutboundPreview) && types == InteractionTypes.OutboundPreview)
                Settings.subscriberObject.Add(types, clientObject);
            messageToClient = clientObject;
        }

        public OutputValues TransferInteractiontoAgent(int proxyClientId, string interactionID, string agentID)
        {
            OutputValues output = OutputValues.GetInstance();
            try
            {
                RequestTransferInteraction requestTransferInteraction = new RequestTransferInteraction();
                output = requestTransferInteraction.TransferInteractiontoAgent(proxyClientId, interactionID, agentID);
            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred while transferring " + generalException.ToString());
            }
            return output;
        }

        public OutputValues TransferInteractiontoQueue(int proxyClientId, string interactionID, string queueName)
        {
            OutputValues output = OutputValues.GetInstance();
            try
            {
                RequestTransferInteraction requestTransferInteraction = new RequestTransferInteraction();
                output = requestTransferInteraction.TransferInteractiontoQueue(proxyClientId, interactionID, queueName);
            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred while transferring " + generalException.ToString());
            }
            return output;
        }

        public OutputValues UnRegisterClient(int proxyID)
        {
            RequestUnRegisterAgent requestUnRegisterAgent = new RequestUnRegisterAgent();
            return requestUnRegisterAgent.AgentUnRegister(proxyID);
        }

        public OutputValues UpdateCaseDataProperties(string interactionId, int proxyClientID, KeyValueCollection updatedProperties)
        {
            OutputValues output = OutputValues.GetInstance();
            try
            {
                RequestToChangeProperties requestToChangeProperties = new RequestToChangeProperties();
                output = requestToChangeProperties.UpdateProperties(interactionId, proxyClientID, updatedProperties);
            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred update case data property " + generalException.ToString());
            }
            return output;
        }

        public OutputValues TakeSnapshotID(int proxyClientId, string condition)
        {
            return TakeSnapshot.TakeSnapshotID(proxyClientId, condition);
        }

        public OutputValues GetSnapshotInteractionsbyID(int proxyClientId, int snapshotID, int startFrom, int numberOfInteractions)
        {
            return GetSnapshotInteractions.GetSnapshotInteractionsbyID(proxyClientId, snapshotID, startFrom, numberOfInteractions);
        }

        public OutputValues ReleaseSnapshotID(int proxyClientId, int snapshotID)
        {
            return ReleaseSnapshot.ReleaseSnapshotID(proxyClientId, snapshotID);
        }

        #endregion Methods
    }
}