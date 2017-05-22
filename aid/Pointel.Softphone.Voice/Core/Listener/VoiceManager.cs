namespace Pointel.Softphone.Voice.Core.Listener
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    using Genesyslab.Platform.ApplicationBlocks.Commons.Broker;
    using Genesyslab.Platform.Commons.Collections;
    using Genesyslab.Platform.Commons.Protocols;
    using Genesyslab.Platform.Commons.Protocols.Internal;
    using Genesyslab.Platform.Voice.Protocols;
    using Genesyslab.Platform.Voice.Protocols.TServer;
    using Genesyslab.Platform.Voice.Protocols.TServer.Events;
    using Genesyslab.Platform.Voice.Protocols.TServer.Requests.Special;

    using Pointel.Interactions.IPlugins;
    using Pointel.Softphone.Voice.Common;
    using Pointel.Softphone.Voice.Core.Common;
    using Pointel.Softphone.Voice.Core.Request;
    using Pointel.Softphone.Voice.Core.Util;

    /// <summary>
    /// This Class handle Events from TServer and send message back to subscriber
    /// </summary>
    internal class VoiceManager
    {
        #region Fields

        public static bool EventCreated = false;
        public static ISoftphoneListener messageToClient = null;

        private static bool isAfterConnect = false;
        private static Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
            "AID");

        //Code Added for Third party Integration -senthil--------14/03/14
        private static List<ISoftphoneListener> messageToIntegration = new List<ISoftphoneListener>();
        private static int protocolClosed = 0;
        private static int protocolOpened = 0;
        private static VoiceManager voiceListnerInstance;

        #endregion Fields

        #region Methods

        public static VoiceManager GetInstance()
        {
            if (voiceListnerInstance == null)
            {
                voiceListnerInstance = new VoiceManager();

                return voiceListnerInstance;
            }
            else
            {
                return voiceListnerInstance;
            }
        }

        public static string GetTServerErrorMessage(int errorCode)
        {
            string output = string.Empty;
            try
            {
                //foreach (int value in Enum.GetValues(typeof(Errors)))
                //{
                //    if (value == errorCode)
                output = Enum.GetName(typeof(Errors), errorCode);
                //}
            }
            catch (Exception commonException)
            {
                logger.Error("Error occurred while getting error message for TEvents from Voice library " + commonException.ToString());
            }
            return output;
        }

        public void ReportingVoiceMessage(object sender, EventArgs e)
        {
            try
            {
                IMessage message = null;
                if (e != null)
                    message = ((MessageEventArgs)e).Message;
                else if (sender != null && sender is IMessage)
                    message = sender as IMessage;
                if (message != null)
                {
                    try
                    {
                        if (!EventCreated)
                        {
                            Settings.GetInstance().VoiceProtocol.Opened += VoiceManager_Opened;
                            Settings.GetInstance().VoiceProtocol.Closed += VoiceManager_Closed;
                            EventCreated = true;
                        }
                    }
                    catch (Exception protocolStatusException)
                    {
                        logger.Error("Error occurred while display the protocol status " + protocolStatusException.ToString());
                    }
                    //Code added for third party CRM plugin
                    //18.03.14
                    //Senthil
                    try
                    {
                        foreach (ISoftphoneListener listener in messageToIntegration)
                        {
                            listener.NotifyCallRinging(message);
                        }
                        if (messageToClient != null)
                            messageToClient.NotifyCallRinging(message);
                    }
                    catch (Exception ex)
                    {
                        logger.Error("Error occurred while 3rd status " + ex.ToString());
                    }
                    //End
                    switch (message.Id)
                    {
                        //Below code added for get the forward status
                        //Shenbagamoorthy B.
                        case EventServerInfo.MessageId:
                            EventServerInfo eventServerInfo = (EventServerInfo)message;
                            logger.Trace("Response from T-Server:" + eventServerInfo.ToString() + eventServerInfo.ToString());
                            break;

                        case EventRegistered.MessageId:
                            EventRegistered eventRegistered = (EventRegistered)message;
                            Settings.GetInstance().IsDNRegistered = true;
                            int callState = eventRegistered.CallState;
                            logger.Trace("Response from T-Server:" + eventRegistered.ToString());
                            var registeredStatus = new AgentStatus();
                            registeredStatus.AgentID = eventRegistered.AgentID;
                            registeredStatus.AgentCurrentStatus = CurrentAgentStatus.Registered.ToString();
                            registeredStatus.Extensions = eventRegistered.Extensions;
                            if (eventRegistered.Reasons != null && eventRegistered.Reasons.Count > 0)
                            {
                                registeredStatus.Reasons.Clear();
                                foreach (string key in eventRegistered.Reasons.Keys)
                                {
                                    if (!registeredStatus.Reasons.ContainsKey(key))
                                        registeredStatus.Reasons.Add(key, eventRegistered.Reasons[key].ToString());
                                }
                            }
                            registeredStatus.AddressType = eventRegistered.AddressType.ToString();
                            messageToClient.NotifyAgentStatus(registeredStatus);
                            registeredStatus = null;
                            break;
                        //Code Added - Get the Query Call status works in some environment. Not working in BCBS.
                        //30.05.2014 - SMoorthy
                        case EventPartyInfo.MessageId:
                            EventPartyInfo eventPartyInfo = (EventPartyInfo)message;
                            logger.Trace("Response from T-Server:" + eventPartyInfo.ToString());
                            Settings.GetInstance().ConnectionID = eventPartyInfo.ConnID.ToString();
                            Settings.GetInstance().ActiveDN = eventPartyInfo.ThisDN;
                            Settings.GetInstance().userData.Clear();
                            Settings.GetInstance().userData.Add("CallType", eventPartyInfo.CallType.ToString());
                            Settings.GetInstance().userData.Add("ConnectionId", eventPartyInfo.ConnID.ToString());
                            Settings.GetInstance().userData.Add("CallerId", eventPartyInfo.CallID.ToString());
                            Settings.GetInstance().userData.Add("ThisDNRole", eventPartyInfo.ThisDNRole.ToString());
                            if (eventPartyInfo.DNIS != null)
                            {
                                Settings.GetInstance().userData.Add("DNIS", eventPartyInfo.DNIS.ToString());
                            }
                            if (eventPartyInfo.ANI != null)
                            {
                                Settings.GetInstance().userData.Add("ANI", eventPartyInfo.ANI.ToString());
                            }
                            if (eventPartyInfo.OtherDN != null)
                            {
                                Settings.GetInstance().userData.Add("OtherDN", eventPartyInfo.OtherDN.ToString());
                            }
                            if (eventPartyInfo.UserData != null)
                            {
                                KeyValueCollection data = eventPartyInfo.UserData;

                                if (data != null && !data.Equals(string.Empty))
                                {
                                    foreach (string keys in data.Keys)
                                    {
                                        if (keys != Settings.GetInstance().DispositionCollectionKey)
                                        {
                                            if (!Settings.GetInstance().userData.ContainsKey(keys))
                                                Settings.GetInstance().userData.Add(keys.ToString(), data[keys].ToString());
                                        }
                                        else
                                        {
                                            if (data[keys] is KeyValueCollection)
                                            {
                                                KeyValueCollection tempCollection = data[keys] as KeyValueCollection;
                                                string result = string.Empty;
                                                foreach (string item in tempCollection.Keys)
                                                {
                                                    result += item + ":" + tempCollection[item] + "; ";
                                                }
                                                if (!string.IsNullOrEmpty(result))
                                                    result = result.Substring(0, (result.Length - 2));

                                                if (!Settings.GetInstance().userData.ContainsKey(keys))
                                                    Settings.GetInstance().userData.Add(keys.ToString(), result);
                                                else
                                                    Settings.GetInstance().userData[keys.ToString()] = result;
                                            }
                                            else
                                            {
                                                if (Settings.GetInstance().userData.ContainsKey(keys))
                                                    Settings.GetInstance().userData.Remove(keys.ToString());
                                            }
                                        }
                                    }
                                }
                            }
                            if (Settings.GetInstance().PartyState == "10")
                            {
                                //Ringing
                                Settings.GetInstance().IsOnCall = true;
                                messageToClient.NotifySubscriber(VoiceEvents.EventRinging, Settings.GetInstance().userData.Clone());
                                messageToClient.NotifyUIStatus(ButtonStatusController.GetCallRiningStatus());
                                try
                                {
                                    var readyStatus = new AgentStatus();
                                    readyStatus.AgentCurrentStatus = CurrentAgentStatus.CallRinging.ToString();
                                    readyStatus.Extensions = null;
                                    readyStatus.Reasons = null;
                                    messageToClient.NotifyAgentStatus(readyStatus);
                                    readyStatus = null;
                                }
                                catch (Exception readyAgentStatusException)
                                {
                                    logger.Error("Error occurred while notifying Event Party Info status to subscriber "
                                        + readyAgentStatusException.ToString());
                                }
                                Settings.GetInstance().PartyState = string.Empty;
                            }
                            if (Settings.GetInstance().PartyState == "12")
                            {
                                Settings.GetInstance().IsOnCall = true;
                                if (!Settings.GetInstance().IsCallHeld)
                                {
                                    //Oncall
                                    messageToClient.NotifySubscriber(VoiceEvents.EventEstablished, Settings.GetInstance().userData.Clone());
                                    messageToClient.NotifyUIStatus(ButtonStatusController.GetOnCallStatus());
                                    try
                                    {
                                        var onCallStatus = new AgentStatus();
                                        onCallStatus.AgentCurrentStatus = CurrentAgentStatus.OnCall.ToString();
                                        onCallStatus.Extensions = null;
                                        onCallStatus.Reasons = null;
                                        messageToClient.NotifyAgentStatus(onCallStatus);
                                        onCallStatus = null;
                                    }
                                    catch (Exception readyAgentStatusException)
                                    {
                                        logger.Error("Error occurred while notifying Event Party Info status to subscriber "
                                            + readyAgentStatusException.ToString());
                                    }
                                }
                                else
                                {
                                    //Held
                                    Settings.GetInstance().IsCallHeld = true;
                                    messageToClient.NotifySubscriber(VoiceEvents.EventHeld, Settings.GetInstance().userData.Clone());
                                    messageToClient.NotifyUIStatus(ButtonStatusController.GetCallOnHoldStatus());
                                    try
                                    {
                                        var heldStatus = new AgentStatus();
                                        heldStatus.AgentCurrentStatus = CurrentAgentStatus.OnHeld.ToString();
                                        heldStatus.Extensions = null;
                                        heldStatus.Reasons = null;
                                        messageToClient.NotifyAgentStatus(heldStatus);
                                        heldStatus = null;
                                    }
                                    catch (Exception readyAgentStatusException)
                                    {
                                        logger.Error("Error occurred while notifying Event Party Info status to subscriber "
                                            + readyAgentStatusException.ToString());
                                    }
                                }
                                Settings.GetInstance().PartyState = string.Empty;
                            }
                            if (Settings.GetInstance().PartyState == "14")
                            {
                                //Held
                                Settings.GetInstance().IsOnCall = true;
                                Settings.GetInstance().IsCallHeld = true;
                                messageToClient.NotifySubscriber(VoiceEvents.EventHeld, Settings.GetInstance().userData.Clone());
                                messageToClient.NotifyUIStatus(ButtonStatusController.GetCallOnHoldStatus());
                                try
                                {
                                    var heldStatus = new AgentStatus();
                                    heldStatus.AgentCurrentStatus = CurrentAgentStatus.OnHeld.ToString();
                                    heldStatus.Extensions = null;
                                    heldStatus.Reasons = null;
                                    messageToClient.NotifyAgentStatus(heldStatus);
                                    heldStatus = null;
                                }
                                catch (Exception readyAgentStatusException)
                                {
                                    logger.Error("Error occurred while notifying Event Party Info status to subscriber "
                                        + readyAgentStatusException.ToString());
                                }
                                Settings.GetInstance().PartyState = string.Empty;
                            }
                            if (eventPartyInfo.AddressInfoStatus == 0)
                            {
                                messageToClient.NotifySubscriber(VoiceEvents.EventReleased, Settings.GetInstance().userData.Clone());
                                messageToClient.NotifyUIStatus(ButtonStatusController.GetCallOnReleaseStatus(Settings.GetInstance().LogOffEnable));
                                try
                                {
                                    var readyStatus = new AgentStatus();
                                    readyStatus.AgentCurrentStatus = CurrentAgentStatus.OnRelease.ToString();
                                    readyStatus.Extensions = null;
                                    readyStatus.Reasons = null;
                                    messageToClient.NotifyAgentStatus(readyStatus);
                                    readyStatus = null;
                                }
                                catch (Exception readyAgentStatusException)
                                {
                                    logger.Error("Error occurred while notifying Event Party Info status to subscriber "
                                        + readyAgentStatusException.ToString());
                                }
                            }
                            break;
                        //end;
                        case EventDestinationBusy.MessageId:
                            EventDestinationBusy eventDestinationBusy = (EventDestinationBusy)message;
                            logger.Trace("Response from T-Server:" + eventDestinationBusy.ToString());
                            RequestAgentReleaseCall.Release(eventDestinationBusy.ConnID.ToString());
                            OutputValues eventOutput = new OutputValues();
                            eventOutput.MessageCode = message.Id.ToString();
                            eventOutput.Message = eventDestinationBusy.Name;
                            messageToClient.NotifyErrorMessage(eventOutput);
                            break;

                        case EventForwardSet.MessageId:
                            EventForwardSet eventForwardSet = (EventForwardSet)message;
                            logger.Trace("Response from T-Server:" + eventForwardSet.ToString());
                            var eventForwardstatus = new AgentStatus();
                            eventForwardstatus.AgentCurrentStatus = "Forwarded";
                            eventForwardstatus.Extensions = null;
                            eventForwardstatus.Reasons = null;
                            messageToClient.NotifyAgentStatus(eventForwardstatus);
                            eventForwardstatus = null;
                            break;

                        case EventForwardCancel.MessageId:
                            EventForwardCancel eventCancelForward = (EventForwardCancel)message;
                            logger.Trace("Response from T-Server:" + eventCancelForward.ToString());
                            var eventCancelForwardstatus = new AgentStatus();
                            eventCancelForwardstatus.AgentCurrentStatus = "ForwardCancel";
                            eventCancelForwardstatus.Extensions = null;
                            eventCancelForwardstatus.Reasons = null;
                            messageToClient.NotifyAgentStatus(eventCancelForwardstatus);
                            eventCancelForwardstatus = null;
                            break;

                        case EventLinkDisconnected.MessageId:
                            EventLinkDisconnected eventLinkDisconnected = (EventLinkDisconnected)message;
                            if (!Settings.GetInstance().IsTserverFailed)
                            {
                                logger.Trace("Response from T-Server:" + eventLinkDisconnected.ToString());
                                var eventLinkDisconnectedStatus = new AgentStatus();
                                eventLinkDisconnectedStatus.AgentCurrentStatus = "SwitchDisconnected";
                                eventLinkDisconnectedStatus.Extensions = null;
                                eventLinkDisconnectedStatus.Reasons = null;
                                messageToClient.NotifyAgentStatus(eventLinkDisconnectedStatus);
                                eventLinkDisconnectedStatus = null;
                            }
                            break;

                        case EventLinkConnected.MessageId:
                            EventLinkConnected eventLinkConnected = (EventLinkConnected)message;
                            if (eventLinkConnected.ApplicationName == null && !Settings.GetInstance().IsTserverFailed)
                            {
                                logger.Trace("Response from T-Server:" + eventLinkConnected.ToString());
                                var eventLinkConnectedStatus = new AgentStatus();
                                eventLinkConnectedStatus.AgentCurrentStatus = "SwitchConnected";
                                eventLinkConnectedStatus.Extensions = null;
                                eventLinkConnectedStatus.Reasons = null;
                                messageToClient.NotifyAgentStatus(eventLinkConnectedStatus);
                                eventLinkConnectedStatus = null;
                            }
                            break;

                        case EventAgentLogin.MessageId:
                            EventAgentLogin eventAgentLogin = (EventAgentLogin)message;
                            logger.Trace("Response from T-Server:" + eventAgentLogin.ToString());
                            messageToClient.NotifyUIStatus(ButtonStatusController.GetLoginStatus());
                            var loginStatus = new AgentStatus();
                            loginStatus.AgentCurrentStatus = "LoggedIn";
                            loginStatus.Extensions = null;
                            loginStatus.Reasons = null;
                            logger.Trace("Sending Agent Login to SoftphoneBar:" + "LoggedIn");
                            messageToClient.NotifyAgentStatus(loginStatus);
                            loginStatus = null;
                            break;

                        case EventAgentLogout.MessageId:
                            EventAgentLogout eventAgentLogout = (EventAgentLogout)message;
                            try
                            {
                                //Unregister DN's
                                if (Settings.GetInstance().ACDPosition == Settings.GetInstance().ExtensionDN)
                                {
                                    Pointel.Softphone.Voice.Core.Request.RequestUnRegisterPlace.UnRegisterDN(Settings.GetInstance().ExtensionDN);
                                }
                                else
                                {
                                    Pointel.Softphone.Voice.Core.Request.RequestUnRegisterPlace.UnRegisterDN(Settings.GetInstance().ACDPosition);
                                    Pointel.Softphone.Voice.Core.Request.RequestUnRegisterPlace.UnRegisterDN(Settings.GetInstance().ExtensionDN);
                                }
                                //end

                                logger.Trace("Response from T-Server:" + eventAgentLogout.ToString());
                                if (!Settings.GetInstance().IsOnCall)
                                    messageToClient.NotifyUIStatus(ButtonStatusController.GetLogoutStatus());

                                var logoutStatus = new AgentStatus();
                                logoutStatus.AgentCurrentStatus = CurrentAgentStatus.Logout.ToString();
                                logoutStatus.Extensions = null;
                                logoutStatus.Reasons = null;
                                logger.Trace("Sending Agent Logout to SoftphoneBar:" + CurrentAgentStatus.Logout.ToString());
                                messageToClient.NotifyAgentStatus(logoutStatus);
                                logoutStatus = null;
                            }
                            catch (Exception commonException)
                            {
                                logger.Error("ReportingVoiceMessage:AgentLogout:" + commonException.ToString());
                            }

                            break;

                        case EventAgentReady.MessageId:
                            EventAgentReady eventAgentReady = (EventAgentReady)message;
                            logger.Trace("Response from T-Server:" + eventAgentReady.ToString());
                            try
                            {
                                var readyStatus = new AgentStatus();
                                readyStatus.AgentCurrentStatus = CurrentAgentStatus.Ready.ToString();
                                readyStatus.Extensions = null;
                                readyStatus.Reasons = null;
                                messageToClient.NotifyAgentStatus(readyStatus);
                                readyStatus = null;
                            }
                            catch (Exception readyAgentStatusException)
                            {
                                logger.Error("Error occurred while notifying agent ready status to subscriber "
                                    + readyAgentStatusException.ToString());
                            }
                            if (!Settings.GetInstance().IsOnCall)
                                messageToClient.NotifyUIStatus(ButtonStatusController.GetReadyStatus(Settings.GetInstance().LogOffEnable));
                            break;

                        case EventAgentNotReady.MessageId:
                            EventAgentNotReady eventAgentNReady = (EventAgentNotReady)message;
                            logger.Trace("Response from T-Server:" + eventAgentNReady.ToString());
                            var nreadyStatus = new AgentStatus();
                            try
                            {
                                AgentStatus.agentStatusObject = null;

                                if (eventAgentNReady.AgentWorkMode == AgentWorkMode.AfterCallWork)
                                {
                                    nreadyStatus.AgentCurrentStatus = CurrentAgentStatus.AfterCallWork.ToString();
                                    eventAgentNReady.Extensions = null;
                                    //messageToClient.NotifyAgentStatus(nreadyStatus);
                                    //goto NotReadyLabel;
                                }
                                else
                                    nreadyStatus.AgentCurrentStatus = CurrentAgentStatus.NotReady.ToString();

                                if (eventAgentNReady.Reasons != null)
                                {
                                    nreadyStatus.AgentCurrentStatus = CurrentAgentStatus.NotReady.ToString();
                                    nreadyStatus.reasons = new Dictionary<string, string>();
                                    foreach (string keys in eventAgentNReady.Reasons.AllKeys)
                                    {
                                        nreadyStatus.reasons.Add(keys, eventAgentNReady.Reasons[keys].ToString());
                                    }

                                    //if (nreadyStatus.reasons.ContainsValue("AfterCallWork"))
                                    //{
                                    //    nreadyStatus.AgentCurrentStatus = CurrentAgentStatus.AfterCallWork.ToString();
                                    //}
                                }

                                if (eventAgentNReady.Extensions != null)
                                {
                                    nreadyStatus.AgentCurrentStatus = CurrentAgentStatus.NotReady.ToString();
                                    foreach (string keys in eventAgentNReady.Extensions.AllKeys)
                                    {
                                        if (nreadyStatus.Extensions != null)
                                        {
                                            nreadyStatus.Extensions.Add(keys, eventAgentNReady.Extensions[keys].ToString());
                                        }
                                        else
                                        {
                                            nreadyStatus.Extensions = new KeyValueCollection();
                                            nreadyStatus.Extensions.Add(keys, eventAgentNReady.Extensions[keys].ToString());
                                        }
                                        //if (eventAgentNReady.Extensions[keys].ToString().Contains("AfterCallWork"))
                                        //{
                                        //    nreadyStatus.AgentCurrentStatus = CurrentAgentStatus.AfterCallWork.ToString();
                                        //}
                                    }
                                }

                                messageToClient.NotifyAgentStatus(nreadyStatus);
                            }
                            catch (Exception nreadyAgentStatusException)
                            {
                                logger.Error("Error occurred while notifying agent not ready status to subscriber "
                                    + nreadyAgentStatusException.ToString());
                                messageToClient.NotifyAgentStatus(nreadyStatus);
                            }
                            nreadyStatus = null;
                            //NotReadyLabel :
                            if (!Settings.GetInstance().IsOnCall)
                                messageToClient.NotifyUIStatus(ButtonStatusController.GetNotReadyStatus());
                            break;

                        case EventHeld.MessageId:
                            EventHeld eventHeld = (EventHeld)message;
                            Settings.GetInstance().IsCallHeld = true;
                            logger.Trace("Response from T-Server:" + eventHeld.ToString());
                            if (eventHeld.CallType == CallType.Consult)
                            {
                                Settings.GetInstance().ConnectionID = eventHeld.ConnID.ToString();
                                Settings.GetInstance().ConsultConnectionID = eventHeld.TransferConnID.ToString();

                                logger.Debug("Settings.GetInstance().ConsultConnectionID : " + Settings.GetInstance().ConsultConnectionID);
                                logger.Debug("Settings.GetInstance().ConnectionID : " + Settings.GetInstance().ConnectionID);
                            }
                            try
                            {
                                var readyStatus = new AgentStatus();

                                readyStatus.AgentCurrentStatus = CurrentAgentStatus.OnHeld.ToString();
                                readyStatus.Extensions = null;
                                readyStatus.Reasons = null;
                                messageToClient.NotifyAgentStatus(readyStatus);
                                readyStatus = null;
                            }
                            catch (Exception readyAgentStatusException)
                            {
                                logger.Error("Error occurred while notifying agent hold status to subscriber "
                                    + readyAgentStatusException.ToString());
                            }
                            messageToClient.NotifyUIStatus(ButtonStatusController.GetCallOnHoldStatus());
                            break;

                        case EventPartyChanged.MessageId:
                            EventPartyChanged partyChngData = (EventPartyChanged)message;
                            logger.Trace("Response from T-Server:" + partyChngData.ToString());
                            Settings.GetInstance().ConnectionID = partyChngData.ConnID.ToString();
                            Settings.GetInstance().userData.Clear();

                            Settings.GetInstance().userData.Add("CallType", partyChngData.CallType.ToString());
                            Settings.GetInstance().userData.Add("ConnectionId", partyChngData.ConnID.ToString());
                            Settings.GetInstance().userData.Add("CallerId", partyChngData.CallID.ToString());
                            Settings.GetInstance().userData.Add("ThirdPartyDNRole", partyChngData.ThirdPartyDNRole.ToString().ToLower());
                            if (partyChngData.DNIS != null)
                            {
                                Settings.GetInstance().userData.Add("DNIS", partyChngData.DNIS.ToString());
                            }
                            if (partyChngData.ANI != null)
                            {
                                Settings.GetInstance().userData.Add("ANI", partyChngData.ANI.ToString());
                            }
                            if (partyChngData.OtherDN != null)
                            {
                                Settings.GetInstance().userData.Add("OtherDN", partyChngData.OtherDN.ToString());
                            }
                            Settings.GetInstance().userData.Add("ThisDNRole", partyChngData.ThisDNRole.ToString());
                            if (partyChngData.UserData != null)
                            {
                                KeyValueCollection data = partyChngData.UserData;
                                if (data != null && !data.Equals(string.Empty))
                                {
                                    foreach (string keys in data.Keys)
                                    {
                                        if (keys != Settings.GetInstance().DispositionCollectionKey)
                                        {
                                            if (!Settings.GetInstance().userData.ContainsKey(keys))
                                                Settings.GetInstance().userData.Add(keys.ToString(), data[keys].ToString());
                                        }
                                        else
                                        {
                                            if (data[keys] is KeyValueCollection)
                                            {
                                                KeyValueCollection tempCollection = data[keys] as KeyValueCollection;
                                                string result = string.Empty;
                                                foreach (string item in tempCollection.Keys)
                                                {
                                                    result += item + ":" + tempCollection[item] + "; ";
                                                }
                                                if (!string.IsNullOrEmpty(result))
                                                    result = result.Substring(0, (result.Length - 2));

                                                if (!Settings.GetInstance().userData.ContainsKey(keys))
                                                    Settings.GetInstance().userData.Add(keys.ToString(), result);
                                                else
                                                    Settings.GetInstance().userData[keys.ToString()] = result;
                                            }
                                            else
                                            {
                                                if (Settings.GetInstance().userData.ContainsKey(keys))
                                                    Settings.GetInstance().userData.Remove(keys.ToString());
                                            }
                                        }
                                    }
                                }
                            }

                            ////Code added by Manikandan to fix auto answer and single transfer issue
                            if (Settings.GetInstance().IsAutoAnswerEnabled && partyChngData.ThirdPartyDNRole.ToString().ToLower().Contains("transfer"))
                            {
                                logger.Warn("Warn code 007");
                                //var soft = new SoftPhone();
                                //soft.Answer();
                            }
                            //end

                            //Below condition added to re-solve issue REG - DAT 01
                            //04-15-2013 Ram
                            messageToClient.NotifySubscriber(VoiceEvents.EventPartyChanged, Settings.GetInstance().userData.Clone());
                            //End

                            OutputValues eventPartyOutput = new OutputValues();
                            eventPartyOutput.MessageCode = message.Id.ToString();
                            eventPartyOutput.Message = partyChngData.Name;
                            messageToClient.NotifyErrorMessage(eventPartyOutput);

                            break;

                        case EventAttachedDataChanged.MessageId:
                            EventAttachedDataChanged getChangedCallData = (EventAttachedDataChanged)message;
                            //Settings.GetInstance().userData.Clear();
                            //Added to insert and as well as update
                            if (!Settings.GetInstance().userData.ContainsKey("CallType"))
                                Settings.GetInstance().userData.Add("CallType", getChangedCallData.CallType.ToString());
                            else
                                Settings.GetInstance().userData["CallType"] = getChangedCallData.CallType.ToString();

                            if (!Settings.GetInstance().userData.ContainsKey("ConnectionId"))
                                Settings.GetInstance().userData.Add("ConnectionId", getChangedCallData.ConnID.ToString());
                            else
                                Settings.GetInstance().userData["ConnectionId"] = getChangedCallData.ConnID.ToString();

                            if (!Settings.GetInstance().userData.ContainsKey("CallerId"))
                                Settings.GetInstance().userData.Add("CallerId", getChangedCallData.CallID.ToString());
                            else
                                Settings.GetInstance().userData["CallerId"] = getChangedCallData.CallID.ToString();

                            if (getChangedCallData.DNIS != null)
                            {
                                if (!Settings.GetInstance().userData.ContainsKey("DNIS"))
                                    Settings.GetInstance().userData.Add("DNIS", getChangedCallData.DNIS.ToString());
                                else
                                    Settings.GetInstance().userData["DNIS"] = getChangedCallData.DNIS.ToString();
                            }
                            if (getChangedCallData.ANI != null)
                            {
                                if (!Settings.GetInstance().userData.ContainsKey("ANI"))
                                    Settings.GetInstance().userData.Add("ANI", getChangedCallData.ANI.ToString());
                                else
                                    Settings.GetInstance().userData["ANI"] = getChangedCallData.ANI.ToString();
                            }
                            if (getChangedCallData.OtherDN != null)
                            {
                                if (!Settings.GetInstance().userData.ContainsKey("OtherDN"))
                                    Settings.GetInstance().userData.Add("OtherDN", getChangedCallData.OtherDN.ToString());
                                else
                                    Settings.GetInstance().userData["OtherDN"] = getChangedCallData.OtherDN.ToString();
                            }
                            if (getChangedCallData.UserData != null)
                            {
                                KeyValueCollection data = getChangedCallData.UserData;

                                if (data != null && !data.Equals(string.Empty))
                                {
                                    foreach (string keys in data.Keys)
                                    {
                                        if (keys != Settings.GetInstance().DispositionCollectionKey)
                                        {
                                            if (!Settings.GetInstance().userData.ContainsKey(keys))
                                                Settings.GetInstance().userData.Add(keys.ToString(), data[keys].ToString());
                                            else
                                                Settings.GetInstance().userData[keys.ToString()] = data[keys].ToString();
                                        }
                                        else
                                        {
                                            if (data[keys] is KeyValueCollection)
                                            {
                                                KeyValueCollection tempCollection = data[keys] as KeyValueCollection;
                                                string result = string.Empty;
                                                foreach (string item in tempCollection.Keys)
                                                {
                                                    result += item + ":" + tempCollection[item] + "; ";
                                                }
                                                if (!string.IsNullOrEmpty(result))
                                                    result = result.Substring(0, (result.Length - 2));

                                                if (!Settings.GetInstance().userData.ContainsKey(keys))
                                                    Settings.GetInstance().userData.Add(keys.ToString(), result);
                                                else
                                                    Settings.GetInstance().userData[keys.ToString()] = result;
                                            }
                                            else
                                            {
                                                if (Settings.GetInstance().userData.ContainsKey(keys))
                                                    Settings.GetInstance().userData.Remove(keys.ToString());
                                            }
                                        }
                                    }
                                }
                            }
                            if (getChangedCallData.ThirdPartyDN != null)
                            {
                                if (!Settings.GetInstance().userData.ContainsKey("ThirdPartyDN"))
                                    Settings.GetInstance().userData.Add("ThirdPartyDN", getChangedCallData.ThirdPartyDN.ToString());
                                else
                                    Settings.GetInstance().userData["ThirdPartyDN"] = getChangedCallData.ThirdPartyDN.ToString();
                            }
                            else if (getChangedCallData.DNIS != null)
                            {
                                if (!Settings.GetInstance().userData.ContainsKey("ThirdPartyDN"))
                                    Settings.GetInstance().userData.Add("ThirdPartyDN", getChangedCallData.DNIS.ToString());
                                else
                                    Settings.GetInstance().userData["ThirdPartyDN"] = getChangedCallData.DNIS.ToString();
                            }
                            messageToClient.NotifySubscriber(VoiceEvents.EventAttachedDataChanged, Settings.GetInstance().userData.Clone());
                            logger.Trace("Response from T-Server:" + getChangedCallData.ToString());
                            //End
                            break;

                        case EventRinging.MessageId:
                            EventRinging getCallData = (EventRinging)message;
                            Settings.GetInstance().IsOnCall = true;
                            logger.Trace("Response from T-Server for EventRinging.");
                            //logger.Trace("Response from T-Server:" + getCallData.ToString());
                            //Error that we got for random call
                            //can't process DataSupport class cause exception: [ArgumentException] An entry with the same key already exists.
                            //at:    at System.ThrowHelper.ThrowArgumentException(ExceptionResource resource)
                            //   at System.Collections.Generic.TreeSet`1.AddIfNotPresent(T item)
                            //   at System.Collections.Generic.SortedDictionary`2.Add(TKey key, TValue value)
                            //   at Genesyslab.Platform.Commons.Collections.Internal.OrderedHashmapImpl.GetAllKeys()
                            //   at Genesyslab.Platform.Commons.Collections.Internal.OrderedHashmapImpl.OrderedHashmapImplEnumerator..ctor(OrderedHashmapImpl hm)
                            //   at Genesyslab.Platform.Commons.Collections.Internal.OrderedHashmapImpl.GetEnumerator()
                            //   at Genesyslab.Platform.Commons.Collections.Filters.KeyValueFilterSet.FilterList[T](StringBuilder buf, KeyValueCollectionBase`1 col, Int32 indent)
                            //   at Genesyslab.Platform.Commons.Protocols.Internal.KeyValueCodec.Genesyslab.Platform.Commons.Protocols.Internal.IPrintable.AppendLogValue(StringBuilder buf, Object customTypeObject, Int32 ident, Boolean truncate, Boolean hide)
                            //   at Genesyslab.Platform.Commons.Protocols.Internal.ToStringHelper.AppendLogValue(StringBuilder buffer, IDataSupport data, Int32 indent, Boolean hide, Boolean truncate)

                            Settings.GetInstance().ConnectionID = getCallData.ConnID.ToString();
                            //Code Added to get the active DN dynamically to control the call control
                            //05-14-2013 Palaniappan
                            Settings.GetInstance().ActiveDN = getCallData.ThisDN;
                            //End
                            Settings.GetInstance().userData.Clear();

                            Settings.GetInstance().userData.Add("CallType", getCallData.CallType.ToString());
                            Settings.GetInstance().userData.Add("ConnectionId", getCallData.ConnID.ToString());
                            Settings.GetInstance().userData.Add("CallerId", getCallData.CallID.ToString());
                            if (getCallData.DNIS != null)
                            {
                                Settings.GetInstance().userData.Add("DNIS", getCallData.DNIS.ToString());
                            }
                            if (getCallData.ANI != null)
                            {
                                Settings.GetInstance().userData.Add("ANI", getCallData.ANI.ToString());
                            }
                            if (getCallData.OtherDN != null)
                            {
                                Settings.GetInstance().userData.Add("OtherDN", getCallData.OtherDN.ToString());
                            }
                            if (getCallData.ThirdPartyDN != null)
                            {
                                if (!Settings.GetInstance().userData.ContainsKey("ThirdPartyDN"))
                                    Settings.GetInstance().userData.Add("ThirdPartyDN", getCallData.ThirdPartyDN.ToString());
                                else
                                    Settings.GetInstance().userData["ThirdPartyDN"] = getCallData.ThirdPartyDN.ToString();
                            }
                            else if (getCallData.DNIS != null)
                            {
                                if (!Settings.GetInstance().userData.ContainsKey("ThirdPartyDN"))
                                    Settings.GetInstance().userData.Add("ThirdPartyDN", getCallData.DNIS.ToString());
                                else
                                    Settings.GetInstance().userData["ThirdPartyDN"] = getCallData.DNIS.ToString();
                            }

                            if (getCallData.UserData != null)
                            {
                                try
                                {
                                    logger.Trace("Response from T-Server:" + getCallData.ToString());
                                    KeyValueCollection data = getCallData.UserData;
                                    if (data != null && !data.Equals(string.Empty))
                                    {
                                        //foreach (string keys in data.Keys)
                                        //{
                                        //    if (!Settings.GetInstance().userData.ContainsKey(keys))
                                        //    {
                                        //        Settings.GetInstance().userData.Add(keys.ToString(), data[keys].ToString());
                                        //    }
                                        //}
                                        foreach (string keys in data.Keys)
                                        {
                                            if (keys != Settings.GetInstance().DispositionCollectionKey)
                                            {
                                                if (!Settings.GetInstance().userData.ContainsKey(keys))
                                                    Settings.GetInstance().userData.Add(keys.ToString(), data[keys].ToString());
                                            }
                                            else
                                            {
                                                if (data[keys] is KeyValueCollection)
                                                {
                                                    KeyValueCollection tempCollection = data[keys] as KeyValueCollection;
                                                    string result = string.Empty;
                                                    foreach (string item in tempCollection.Keys)
                                                    {
                                                        result += item + ":" + tempCollection[item] + "; ";
                                                    }
                                                    if (!string.IsNullOrEmpty(result))
                                                        result = result.Substring(0, (result.Length - 2));

                                                    if (!Settings.GetInstance().userData.ContainsKey(keys))
                                                        Settings.GetInstance().userData.Add(keys.ToString(), result);
                                                    else
                                                        Settings.GetInstance().userData[keys.ToString()] = result;
                                                }
                                                else
                                                {
                                                    if (Settings.GetInstance().userData.ContainsKey(keys))
                                                        Settings.GetInstance().userData.Remove(keys.ToString());
                                                }
                                            }
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    logger.Warn("Error occurred while fetching and formating the userdata as :" + ex.Message +
                                       (ex.StackTrace != null ? Environment.NewLine + "#############StackTrace#############" + Environment.NewLine +
                                       ex.StackTrace : ""));
                                }
                            }
                            logger.Trace("calling softphonebar method NotifySubscriber");
                            //Below condition added to re-solve issue REG - DAT 01
                            //04-15-2013 Ram
                            messageToClient.NotifySubscriber(VoiceEvents.EventRinging, Settings.GetInstance().userData.Clone());
                            //End
                            logger.Trace("calling softphonebar method NotifyUIStatus");
                            messageToClient.NotifyUIStatus(ButtonStatusController.GetCallRiningStatus());
                            try
                            {
                                var callRingingStatus = new AgentStatus();
                                callRingingStatus.AgentCurrentStatus = CurrentAgentStatus.CallRinging.ToString();
                                callRingingStatus.Extensions = null;
                                callRingingStatus.Reasons = null;
                                callRingingStatus.ConnId = getCallData.ConnID.ToString();
                                messageToClient.NotifyAgentStatus(callRingingStatus);
                                callRingingStatus = null;
                            }
                            catch (Exception readyAgentStatusException)
                            {
                                logger.Error("Error occurred while notifying agent ringing status to subscriber "
                                    + readyAgentStatusException.ToString());
                            }

                            break;
                        //Below code added for get the dtmf sent
                        //03-12-2013
                        //Shenbagamoorthy B.
                        case EventDtmfSent.MessageId:
                            EventDtmfSent eventDtmfSent = (EventDtmfSent)message;
                            logger.Trace("Response from T-Server:" + eventDtmfSent.ToString());
                            try
                            {
                                var dtmfStatus = new AgentStatus();
                                dtmfStatus.AgentCurrentStatus = "DTMFSent";
                                dtmfStatus.Extensions = null;
                                dtmfStatus.Reasons = null;
                                messageToClient.NotifyAgentStatus(dtmfStatus);
                                dtmfStatus = null;
                            }
                            catch (Exception readyAgentStatusException)
                            {
                                logger.Error("Error occurred while notifying agent ringing status to subscriber "
                                    + readyAgentStatusException.ToString());
                            }

                            break;

                        case EventNetworkReached.MessageId:
                            EventNetworkReached eventNetworkReached = (EventNetworkReached)message;
                            logger.Trace("Response from T-Server:" + eventNetworkReached.ToString());
                            //Settings.GetInstance().ConnectionID = eventNetworkReached.ConnID.ToString();
                            Settings.GetInstance().userData.Clear();

                            Settings.GetInstance().userData.Add("CallType", eventNetworkReached.CallType.ToString());
                            Settings.GetInstance().userData.Add("ConnectionId", eventNetworkReached.ConnID.ToString());
                            Settings.GetInstance().userData.Add("CallerId", eventNetworkReached.CallID.ToString());
                            if (eventNetworkReached.DNIS != null)
                            {
                                Settings.GetInstance().userData.Add("DNIS", eventNetworkReached.DNIS.ToString());
                            }
                            if (eventNetworkReached.ANI != null)
                            {
                                Settings.GetInstance().userData.Add("ANI", eventNetworkReached.ANI.ToString());
                            }
                            if (eventNetworkReached.OtherDN != null)
                            {
                                Settings.GetInstance().userData.Add("OtherDN", eventNetworkReached.OtherDN.ToString());
                            }
                            if (eventNetworkReached.UserData != null)
                            {
                                KeyValueCollection data = eventNetworkReached.UserData;

                                if (data != null && !data.Equals(string.Empty))
                                {
                                    //foreach (string keys in data.Keys)
                                    //{
                                    //    if (!Settings.GetInstance().userData.ContainsKey(keys))
                                    //    {
                                    //        Settings.GetInstance().userData.Add(keys.ToString(), data[keys].ToString());
                                    //    }
                                    //}
                                    foreach (string keys in data.Keys)
                                    {
                                        if (keys != Settings.GetInstance().DispositionCollectionKey)
                                        {
                                            if (!Settings.GetInstance().userData.ContainsKey(keys))
                                                Settings.GetInstance().userData.Add(keys.ToString(), data[keys].ToString());
                                        }
                                        else
                                        {
                                            if (data[keys] is KeyValueCollection)
                                            {
                                                KeyValueCollection tempCollection = data[keys] as KeyValueCollection;
                                                string result = string.Empty;
                                                foreach (string item in tempCollection.Keys)
                                                {
                                                    result += item + ":" + tempCollection[item] + "; ";
                                                }
                                                if (!string.IsNullOrEmpty(result))
                                                    result = result.Substring(0, (result.Length - 2));

                                                if (!Settings.GetInstance().userData.ContainsKey(keys))
                                                    Settings.GetInstance().userData.Add(keys.ToString(), result);
                                                else
                                                    Settings.GetInstance().userData[keys.ToString()] = result;
                                            }
                                            else
                                            {
                                                if (Settings.GetInstance().userData.ContainsKey(keys))
                                                    Settings.GetInstance().userData.Remove(keys.ToString());
                                            }
                                        }
                                    }
                                }
                            }
                            messageToClient.NotifySubscriber(VoiceEvents.EventNetworkReached, Settings.GetInstance().userData.Clone());
                            break;
                        //End
                        case EventEstablished.MessageId:
                            EventEstablished eventCallEstablish = (EventEstablished)message;
                            logger.Trace("Response from T-Server:" + eventCallEstablish.ToString());
                            Settings.GetInstance().ConnectionID = eventCallEstablish.ConnID.ToString();
                            Settings.GetInstance().userData.Clear();
                            Settings.GetInstance().userData.Add("CallType", eventCallEstablish.CallType.ToString());
                            Settings.GetInstance().userData.Add("ConnectionId", eventCallEstablish.ConnID.ToString());
                            Settings.GetInstance().userData.Add("CallerId", eventCallEstablish.CallID.ToString());
                            //Settings.GetInstance().userData.Add("ServerTime", eventCallEstablish.Time.TimeinSecs.ToString());
                            if (eventCallEstablish.DNIS != null)
                            {
                                Settings.GetInstance().userData.Add("DNIS", eventCallEstablish.DNIS.ToString());
                            }
                            if (eventCallEstablish.ANI != null)
                            {
                                Settings.GetInstance().userData.Add("ANI", eventCallEstablish.ANI.ToString());
                            }
                            if (eventCallEstablish.OtherDN != null)
                            {
                                Settings.GetInstance().userData.Add("OtherDN", eventCallEstablish.OtherDN.ToString());
                            }
                            Settings.GetInstance().userData.Add("ThisDNRole", eventCallEstablish.ThisDNRole.ToString());
                            if (eventCallEstablish.UserData != null)
                            {
                                KeyValueCollection data = eventCallEstablish.UserData;

                                if (data != null && !data.Equals(string.Empty))
                                {
                                    foreach (string keys in data.Keys)
                                    {
                                        if (keys != Settings.GetInstance().DispositionCollectionKey)
                                        {
                                            if (!Settings.GetInstance().userData.ContainsKey(keys))
                                                Settings.GetInstance().userData.Add(keys.ToString(), data[keys].ToString());
                                        }
                                        else
                                        {
                                            if (data[keys] is KeyValueCollection)
                                            {
                                                KeyValueCollection tempCollection = data[keys] as KeyValueCollection;
                                                string result = string.Empty;
                                                foreach (string item in tempCollection.Keys)
                                                    result += item + ":" + tempCollection[item] + "; ";
                                                if (!string.IsNullOrEmpty(result))
                                                    result = result.Substring(0, (result.Length - 2));

                                                if (!Settings.GetInstance().userData.ContainsKey(keys))
                                                    Settings.GetInstance().userData.Add(keys.ToString(), result);
                                                else
                                                    Settings.GetInstance().userData[keys.ToString()] = result;
                                            }
                                            else
                                            {
                                                if (Settings.GetInstance().userData.ContainsKey(keys))
                                                    Settings.GetInstance().userData.Remove(keys.ToString());
                                            }
                                        }
                                    }
                                }
                            }

                            try
                            {
                                var establishedStatus = new AgentStatus();

                                establishedStatus.AgentCurrentStatus = CurrentAgentStatus.OnCall.ToString();
                                establishedStatus.ConnId = eventCallEstablish.ConnID.ToString();
                                establishedStatus.ExactEvent = VoiceEvents.EventEstablished;

                                messageToClient.NotifyAgentStatus(establishedStatus);
                                establishedStatus = null;
                            }
                            catch (Exception readyAgentStatusException)
                            {
                                logger.Error("Error occurred while notifying agent hold status to subscriber "
                                    + readyAgentStatusException.ToString());
                            }

                            //Code Added to get the active DN dynamically to control the call control
                            //05-14-2013 Palaniappan
                            Settings.GetInstance().ActiveDN = eventCallEstablish.ThisDN;
                            //End
                            messageToClient.NotifyUIStatus(ButtonStatusController.GetOnCallStatus());

                            if (eventCallEstablish.CallType == CallType.Consult)
                            {
                                Settings.GetInstance().ConsultConnectionID = eventCallEstablish.TransferConnID.ToString();
                                Settings.GetInstance().ConnectionID = eventCallEstablish.ConnID.ToString();

                                logger.Debug("Settings.GetInstance().ConsultConnectionID : " + Settings.GetInstance().ConsultConnectionID);
                                logger.Debug("Settings.GetInstance().ConnectionID : " + Settings.GetInstance().ConnectionID);

                                if (Settings.GetInstance().IsEnableInitiateTransfer)
                                {
                                    //messageToClient.NotifyUIStatus(ButtonStatusController.GetInitiateTransferStatus());
                                    //Below condition added to re-solve issue REG - PHN 16
                                    //04-15-2013 Ram
                                    if (Settings.GetInstance().IsCustomerReleaseTransferCallBeforeEstablish)
                                    {
                                        Settings.GetInstance().ConnectionID = eventCallEstablish.ConnID.ToString();
                                        messageToClient.NotifyUIStatus(ButtonStatusController.GetCallReleaseStatus());
                                    }
                                    else
                                    {
                                        //Code Added - Based on Switch Type Connection Id will get differ
                                        //04.10.2013 - V.Palaniappan
                                        //code commented by vinoth 27th Oct after facing issue in elavon environment
                                        //if (string.Compare(Settings.GetInstance().SwitchTypeName, "nortel") == 0)
                                        //{
                                        //}
                                        //else
                                        //{
                                        Settings.GetInstance().ConsultConnectionID = eventCallEstablish.ConnID.ToString();
                                        Settings.GetInstance().ConnectionID = eventCallEstablish.TransferConnID.ToString();

                                        logger.Debug("Settings.GetInstance().ConsultConnectionID : " + Settings.GetInstance().ConsultConnectionID);
                                        logger.Debug("Settings.GetInstance().ConnectionID : " + Settings.GetInstance().ConnectionID);
                                        //}//End
                                        Settings.GetInstance().IsCustomerReleaseTransferCallAfterEstablish = true;
                                        messageToClient.NotifyUIStatus(ButtonStatusController.GetAcceptTransferStatus());
                                    }
                                    //End
                                }
                                else if (Settings.GetInstance().IsEnableInitiateConference)
                                {
                                    //Below condition added to re-solve issue REG - PHN 19
                                    //04-17-2013 Ram
                                    if (Settings.GetInstance().IsCustomerReleaseConfCallBeforeEstablish)
                                    {
                                        Settings.GetInstance().ConnectionID = eventCallEstablish.ConnID.ToString();
                                        messageToClient.NotifyUIStatus(ButtonStatusController.GetCallReleaseStatus());
                                    }
                                    else
                                    {
                                        //Code Added - Based on Switch Type Connection Id will get differ
                                        //04.10.2013 - V.Palaniappan
                                        //code commented by vinoth 27th Oct after facing issue in elavon environment
                                        //if (string.Compare(Settings.GetInstance().SwitchTypeName, "nortel") == 0)
                                        //{
                                        //}
                                        //else
                                        //{
                                        Settings.GetInstance().ConsultConnectionID = eventCallEstablish.ConnID.ToString();
                                        Settings.GetInstance().ConnectionID = eventCallEstablish.TransferConnID.ToString();

                                        logger.Debug("Settings.GetInstance().ConsultConnectionID : " + Settings.GetInstance().ConsultConnectionID);
                                        logger.Debug("Settings.GetInstance().ConnectionID : " + Settings.GetInstance().ConnectionID);
                                        //}//End
                                        Settings.GetInstance().IsCustomerReleaseConfCallAfterEstablish = true;
                                        messageToClient.NotifyUIStatus(ButtonStatusController.GetAcceptConferenceStatus());
                                    }
                                }
                            }
                            messageToClient.NotifySubscriber(VoiceEvents.EventEstablished, Settings.GetInstance().userData.Clone());
                            break;

                        case EventDialing.MessageId:

                            EventDialing eventDialing = (EventDialing)message;
                            Settings.GetInstance().IsOnCall = true;
                            logger.Trace("Response from T-Server  " + eventDialing.ToString());
                            if (eventDialing.CallType != CallType.Consult)
                                Settings.GetInstance().ConnectionID = eventDialing.ConnID.ToString();
                            Settings.GetInstance().userData.Clear();

                            Settings.GetInstance().userData.Add("CallType", eventDialing.CallType.ToString());
                            Settings.GetInstance().userData.Add("ConnectionId", eventDialing.ConnID.ToString());
                            Settings.GetInstance().userData.Add("CallerId", eventDialing.CallID.ToString());
                            if (eventDialing.DNIS != null)
                            {
                                Settings.GetInstance().userData.Add("DNIS", eventDialing.DNIS.ToString());
                            }
                            if (eventDialing.ANI != null)
                            {
                                Settings.GetInstance().userData.Add("ANI", eventDialing.ANI.ToString());
                            }
                            if (eventDialing.OtherDN != null)
                            {
                                Settings.GetInstance().userData.Add("OtherDN", eventDialing.OtherDN.ToString());
                            }

                            //Below condition added to re-solve Hardphone Scenario
                            //17-08-2015 Smoorthy
                            if (eventDialing.ConnID != null && !string.IsNullOrEmpty(Settings.GetInstance().ConnectionID) && Settings.GetInstance().ConnectionID != eventDialing.ConnID.ToString())
                            {
                                SoftPhone soft = new SoftPhone();
                                IMessage iMessages = soft.GetCallStatusInfo(eventDialing.ConnID.ToString());
                                if (iMessages != null && iMessages.Id == EventPartyInfo.MessageId)
                                {
                                    EventPartyInfo info = (EventPartyInfo)iMessages;
                                    string data = info.Cause.ToString();
                                }
                                if (eventDialing.Reasons != null)
                                {
                                    if (eventDialing.Reasons.ContainsKey("OperationMode"))
                                    {
                                        if (eventDialing.Reasons["OperationMode"].ToString() == "Conference")
                                        {
                                            //  HoldingFlagStatus(PhoneFunctions.IntiateConference);
                                            Settings.GetInstance().userData.Add("OperationMode", "Conference");
                                        }
                                        else
                                        {
                                            // HoldingFlagStatus(PhoneFunctions.InitiateTransfer);
                                            Settings.GetInstance().userData.Add("OperationMode", "Transfer");
                                        }
                                    }
                                }
                                else
                                {
                                    //HoldingFlagStatus(PhoneFunctions.InitiateTransfer);
                                    Settings.GetInstance().userData.Add("OperationMode", "Transfer");
                                }
                            }
                            //end

                            if (eventDialing.UserData != null)
                            {
                                KeyValueCollection data = eventDialing.UserData;

                                if (data != null && !data.Equals(string.Empty))
                                {
                                    //foreach (string keys in data.Keys)
                                    //{
                                    //    if (!Settings.GetInstance().userData.ContainsKey(keys))
                                    //    {
                                    //        Settings.GetInstance().userData.Add(keys.ToString(), data[keys].ToString());
                                    //    }
                                    //}
                                    foreach (string keys in data.Keys)
                                    {
                                        if (keys != Settings.GetInstance().DispositionCollectionKey)
                                        {
                                            if (!Settings.GetInstance().userData.ContainsKey(keys))
                                                Settings.GetInstance().userData.Add(keys.ToString(), data[keys].ToString());
                                        }
                                        else
                                        {
                                            if (data[keys] is KeyValueCollection)
                                            {
                                                KeyValueCollection tempCollection = data[keys] as KeyValueCollection;
                                                string result = string.Empty;
                                                foreach (string item in tempCollection.Keys)
                                                {
                                                    result += item + ":" + tempCollection[item] + "; ";
                                                }
                                                if (!string.IsNullOrEmpty(result))
                                                    result = result.Substring(0, (result.Length - 2));

                                                if (!Settings.GetInstance().userData.ContainsKey(keys))
                                                    Settings.GetInstance().userData.Add(keys.ToString(), result);
                                                else
                                                    Settings.GetInstance().userData[keys.ToString()] = result;
                                            }
                                            else
                                            {
                                                if (Settings.GetInstance().userData.ContainsKey(keys))
                                                    Settings.GetInstance().userData.Remove(keys.ToString());
                                            }
                                        }
                                    }
                                }
                            }
                            //Below condition added to re-solve issue REG - DAT 01
                            //04-15-2013 Ram
                            messageToClient.NotifySubscriber(VoiceEvents.EventDialing, Settings.GetInstance().userData.Clone());
                            //End

                            if (eventDialing.CallType == CallType.Consult)
                            {
                                Settings.GetInstance().ConsultConnectionID = eventDialing.ConnID.ToString();
                                Settings.GetInstance().ConnectionID = eventDialing.TransferConnID.ToString();
                                Settings.GetInstance().ConsultationConnID = eventDialing.ConnID.ToString();
                                logger.Debug("Settings.GetInstance().ConsultConnectionID : " + Settings.GetInstance().ConsultConnectionID);
                                logger.Debug("Settings.GetInstance().ConnectionID : " + Settings.GetInstance().ConnectionID);
                                logger.Debug("Settings.GetInstance().ConsultationConnID : " + Settings.GetInstance().ConsultationConnID);
                                if (Settings.GetInstance().IsEnableInitiateTransfer)
                                {
                                    //Below condition added to re-solve issue REG - PHN 16
                                    //04-16-2013 Ram
                                    //End
                                    logger.Debug("EventDialing:TransferInitiated");
                                    messageToClient.NotifyUIStatus(ButtonStatusController.GetInitiateTransferStatus());
                                    try
                                    {
                                        var dialingStatus = new AgentStatus();
                                        dialingStatus.AgentCurrentStatus = CurrentAgentStatus.CallDialing.ToString();
                                        dialingStatus.Extensions = null;
                                        dialingStatus.Reasons = null;
                                        dialingStatus.CallType = eventDialing.CallType.ToString();
                                        dialingStatus.ConnId = eventDialing.ConnID.ToString();
                                        messageToClient.NotifyAgentStatus(dialingStatus);
                                        dialingStatus = null;
                                    }
                                    catch (Exception readyAgentStatusException)
                                    {
                                        logger.Error("Error occurred while notifying agent ringing status to subscriber "
                                            + readyAgentStatusException.ToString());
                                    }
                                }
                                else if (Settings.GetInstance().IsEnableInitiateConference)
                                {
                                    //Below condition added to re-solve issue REG - PHN 19
                                    //04-17-2013 Ram
                                    //End
                                    logger.Debug("EventDialing:ConferenceInitiated");
                                    logger.Debug("Original Connection ID:" + Settings.GetInstance().ConnectionID);
                                    logger.Debug("Consult Connection ID:" + Settings.GetInstance().ConsultConnectionID);
                                    messageToClient.NotifyUIStatus(ButtonStatusController.GetInitiateConferenceStatus());
                                    try
                                    {
                                        var dialingStatus = new AgentStatus();
                                        dialingStatus.AgentCurrentStatus = CurrentAgentStatus.CallDialing.ToString();
                                        dialingStatus.Extensions = null;
                                        dialingStatus.Reasons = null;
                                        dialingStatus.CallType = eventDialing.CallType.ToString();
                                        dialingStatus.ConnId = eventDialing.ConnID.ToString();

                                        messageToClient.NotifyAgentStatus(dialingStatus);
                                        dialingStatus = null;
                                    }
                                    catch (Exception readyAgentStatusException)
                                    {
                                        logger.Error("Error occurred while notifying agent Dialling status to subscriber "
                                            + readyAgentStatusException.ToString());
                                    }
                                }
                            }
                            else
                            {
                                Settings.GetInstance().ConnectionID = eventDialing.ConnID.ToString();
                                logger.Debug("Settings.GetInstance().ConnectionID : " + Settings.GetInstance().ConnectionID);
                                messageToClient.NotifyUIStatus(ButtonStatusController.GetCallDialingStatus());
                                try
                                {
                                    var dialingStatus = new AgentStatus();
                                    dialingStatus.ConnId = eventDialing.ConnID.ToString();
                                    dialingStatus.AgentCurrentStatus = CurrentAgentStatus.CallDialing.ToString();
                                    dialingStatus.Extensions = null;
                                    dialingStatus.Reasons = null;
                                    dialingStatus.CallType = eventDialing.CallType.ToString();
                                    dialingStatus.ConnId = eventDialing.ConnID.ToString();
                                    messageToClient.NotifyAgentStatus(dialingStatus);
                                    dialingStatus = null;
                                }
                                catch (Exception readyAgentStatusException)
                                {
                                    logger.Error("Error occurred while notifying agent dialling status to subscriber "
                                        + readyAgentStatusException.ToString());
                                }
                            }
                            break;

                        case EventError.MessageId:
                            EventError eventError = (EventError)message;
                            logger.Trace("Response from T-Server:" + eventError.ToString());
                            OutputValues errorOutput = new OutputValues();
                            errorOutput.MessageCode = Convert.ToString(eventError.ErrorCode);
                            string errorMessage = GetTServerErrorMessage(eventError.ErrorCode);
                            errorOutput.Message = eventError.Name + " : " + errorMessage;

                            //code added by smoorthy to avoid getting call for agent who is not login.
                            if (errorMessage.Contains("TERR_SOFT_AGENT_PSWD_DOESNT_MATCH"))
                            {
                                SoftPhone unregSoftPhone = new SoftPhone();
                                if (Settings.GetInstance().ACDPosition == Settings.GetInstance().ExtensionDN)
                                    unregSoftPhone.UnRegister(Settings.GetInstance().ExtensionDN);
                                else
                                {
                                    unregSoftPhone.UnRegister(Settings.GetInstance().ACDPosition);
                                    unregSoftPhone.UnRegister(Settings.GetInstance().ExtensionDN);
                                }
                            }
                            //end
                            //code added to fix the `
                            else if (errorMessage.Contains("TERR_ORIG_ACCS_BLK"))
                            {
                                if (Settings.GetInstance().IsEnableInitiateTransfer)
                                    Settings.GetInstance().IsEnableInitiateTransfer = false;
                                if (Settings.GetInstance().IsEnableInitiateConference)
                                    Settings.GetInstance().IsEnableInitiateConference = false;
                            }
                            //Incomplete or Invalid Calling or Called DN
                            else if (errorMessage.Contains("TERR_INCM_CALD_DN") || errorMessage.Contains("TERR_INV_CALL_TN")
                                || errorMessage.Contains("TERR_INV_CALL_DN") || errorMessage.Contains("TERR_INCM_CALL_DN") || errorMessage.Contains("TERR_INV_CALD_DN")
                                || errorMessage.Contains("TERR_INCM_CALD_TN"))
                            {
                                logger.Debug("Set False to IsEnableInitiateTransfer : " + Settings.GetInstance().IsEnableInitiateTransfer.ToString());
                                if (Settings.GetInstance().IsEnableInitiateTransfer)
                                    Settings.GetInstance().IsEnableInitiateTransfer = false;
                                if (Settings.GetInstance().IsEnableInitiateConference)
                                    Settings.GetInstance().IsEnableInitiateConference = false;
                                logger.Debug("IsEnableInitiateTransfer : " + Settings.GetInstance().IsEnableInitiateTransfer.ToString());

                            }
                            else if (errorMessage.Contains("TERR_DEST_ACCS_BLK")
                                || errorMessage.Contains("TERR_TERM_PTY_BUSY")
                                || errorMessage.Contains("TERR_DEST_RSR_BLK")
                                || errorMessage.Contains("TERR_DEST_INV_STATE")
                                || errorMessage.Contains("TERR_DEST_SYS_ERR")
                                || errorMessage.Contains("TERR_CANT_INIT_TRN"))
                            {
                                Settings.GetInstance().IsEnableInitiateConference = false;
                                Settings.GetInstance().IsEnableInitiateTransfer = false;
                            }
                            else if (errorMessage.Contains("TERR_CANT_COML_TRN") || errorMessage.Contains("TERR_CANT_CMPL_TRN"))
                            {
                                logger.Debug("Cant complete transfer InitiateTrasfer bool is : " + Settings.GetInstance().IsEnableInitiateTransfer.ToString());
                                //commented for event destination busy during single step confer
                                logger.Debug("ConnectionID :" + Settings.GetInstance().ConnectionID
                                    + "\n ConsultationConnID :" + Settings.GetInstance().ConsultationConnID
                                    + "\n ConsultConnectionID :" + Settings.GetInstance().ConsultConnectionID);
                                if (!string.IsNullOrEmpty(Settings.GetInstance().ConsultConnectionID))
                                {
                                    if (errorMessage.Contains("TERR_CANT_CMPL_TRN"))
                                        Settings.GetInstance().IsEnableInitiateTransfer = true;
                                    if (errorMessage.Contains("TERR_CANT_COML_TRN"))
                                        Settings.GetInstance().IsEnableInitiateConference = true;
                                }
                                //end
                                Settings.GetInstance().IsEnableCompleteTransfer = false;
                                Settings.GetInstance().IsEnableCompleteConference = false;
                            }

                            messageToClient.NotifyErrorMessage(errorOutput);
                            break;

                        case EventRetrieved.MessageId:
                            EventRetrieved eventRetrieved = (EventRetrieved)message;
                            logger.Trace("Response from T-Server:" + eventRetrieved.ToString());
                            Settings.GetInstance().IsCallHeld = false;
                            // if (eventRetrieved.CallType == CallType.Consult)
                            {
                                if (Settings.GetInstance().ConsultConnectionID.Equals(string.Empty) || Settings.GetInstance().ConsultConnectionID == null)
                                {
                                    Settings.GetInstance().ConnectionID = eventRetrieved.ConnID.ToString();
                                }
                                else if (!Settings.GetInstance().ConnectionID.Equals(Settings.GetInstance().ConsultConnectionID))
                                {
                                    //Settings.GetInstance().ConnectionID = eventRetrieved.ConnID.ToString();
                                }
                                else
                                {
                                    Settings.GetInstance().ConnectionID = eventRetrieved.ConnID.ToString();
                                }
                            }
                            //messageToClient.NotifyUIStatus(ButtonStatusController.GetOnCallStatus());
                            Settings.GetInstance().userData.Clear();

                            Settings.GetInstance().userData.Add("CallType", eventRetrieved.CallType.ToString());
                            Settings.GetInstance().userData.Add("ConnectionId", eventRetrieved.ConnID.ToString());
                            Settings.GetInstance().userData.Add("CallerId", eventRetrieved.CallID.ToString());
                            if (eventRetrieved.DNIS != null)
                            {
                                Settings.GetInstance().userData.Add("DNIS", eventRetrieved.DNIS.ToString());
                            }

                            if (eventRetrieved.ANI != null)
                            {
                                Settings.GetInstance().userData.Add("ANI", eventRetrieved.ANI.ToString());
                            }
                            if (eventRetrieved.OtherDN != null)
                            {
                                Settings.GetInstance().userData.Add("OtherDN", eventRetrieved.OtherDN.ToString());
                            }
                            Settings.GetInstance().userData.Add("ThisDNRole", eventRetrieved.ThisDNRole.ToString());
                            if (eventRetrieved.UserData != null)
                            {
                                KeyValueCollection data = eventRetrieved.UserData;

                                if (data != null && !data.Equals(string.Empty))
                                {
                                    //foreach (string keys in data.Keys)
                                    //{
                                    //    if (!Settings.GetInstance().userData.ContainsKey(keys))
                                    //    {
                                    //        Settings.GetInstance().userData.Add(keys.ToString(), data[keys].ToString());
                                    //    }
                                    //}

                                    foreach (string keys in data.Keys)
                                    {
                                        if (keys != Settings.GetInstance().DispositionCollectionKey)
                                        {
                                            if (!Settings.GetInstance().userData.ContainsKey(keys))
                                                Settings.GetInstance().userData.Add(keys.ToString(), data[keys].ToString());
                                        }
                                        else
                                        {
                                            if (data[keys] is KeyValueCollection)
                                            {
                                                KeyValueCollection tempCollection = data[keys] as KeyValueCollection;
                                                string result = string.Empty;
                                                foreach (string item in tempCollection.Keys)
                                                {
                                                    result += item + ":" + tempCollection[item] + "; ";
                                                }
                                                if (!string.IsNullOrEmpty(result))
                                                    result = result.Substring(0, (result.Length - 2));

                                                if (!Settings.GetInstance().userData.ContainsKey(keys))
                                                    Settings.GetInstance().userData.Add(keys.ToString(), result);
                                                else
                                                    Settings.GetInstance().userData[keys.ToString()] = result;
                                            }
                                            else
                                            {
                                                if (Settings.GetInstance().userData.ContainsKey(keys))
                                                    Settings.GetInstance().userData.Remove(keys.ToString());
                                            }
                                        }
                                    }
                                }
                            }
                            try
                            {
                                var retrieveStatus = new AgentStatus();
                                retrieveStatus.AgentCurrentStatus = CurrentAgentStatus.OnRetrieve.ToString();
                                retrieveStatus.Extensions = null;
                                retrieveStatus.Reasons = null;
                                messageToClient.NotifyAgentStatus(retrieveStatus);
                                retrieveStatus = null;
                            }
                            catch (Exception commonException)
                            {
                                logger.Error("ReportingVoiceMessage:EventRetrieve:" + commonException.ToString());
                            }

                            //Below condition added to re-solve issue REG - DAT 01
                            //04-15-2013 Ram
                            messageToClient.NotifySubscriber(VoiceEvents.EventRetrieved, Settings.GetInstance().userData.Clone());
                            //End
                            //Below condition added to re-solve issue REG - PHN 15
                            //04-08-2013 Ram
                            if (Settings.GetInstance().IsAgentOnConferenceCall)
                                messageToClient.NotifyUIStatus(ButtonStatusController.GetCompleteConferenceStatus());
                            else
                                messageToClient.NotifyUIStatus(ButtonStatusController.GetOnCallStatus());
                            //End
                            //code Added - V.Palaniappan
                            //09.12.2013
                            if (eventRetrieved.TransferConnID != null)
                            {
                                //if (Settings.GetInstance().ConnectionID.Equals(eventRetrieved.TransferConnID.ToString()))
                                //{
                                //}
                                //else
                                //{
                                //    //Commented for the purpose of consult call hold retrieve problem
                                //    //Settings.GetInstance().ConnectionID = eventRetrieved.TransferConnID.ToString();
                                //    //Settings.GetInstance().ConsultConnectionID = eventRetrieved.ConnID.ToString();
                                //    //end;

                                //    //Settings.GetInstance().ConsultConnectionID = eventRetrieved.TransferConnID.ToString();
                                //}
                                if (Settings.GetInstance().IsAlternateCallClicked)
                                {
                                    if (Settings.GetInstance().ConnectionID.Equals(eventRetrieved.TransferConnID.ToString()))
                                    {
                                    }
                                    else
                                    {
                                        Settings.GetInstance().ConnectionID = eventRetrieved.TransferConnID.ToString();
                                        Settings.GetInstance().ConsultConnectionID = eventRetrieved.ConnID.ToString();

                                        logger.Debug("Settings.GetInstance().ConsultConnectionID : " + Settings.GetInstance().ConsultConnectionID);
                                        logger.Debug("Settings.GetInstance().ConnectionID : " + Settings.GetInstance().ConnectionID);
                                    }

                                    if (Settings.GetInstance().IsEnableInitiateTransfer)
                                    {
                                        messageToClient.NotifyUIStatus(ButtonStatusController.GetAcceptTransferStatus());
                                    }
                                    else if (Settings.GetInstance().IsEnableInitiateConference)
                                    {
                                        messageToClient.NotifyUIStatus(ButtonStatusController.GetAcceptConferenceStatus());
                                    }
                                }
                            }
                            else
                            {
                                if (!Settings.GetInstance().IsAlternateCallClicked)
                                {
                                    Settings.GetInstance().ConsultConnectionID = string.Empty;
                                }
                                else
                                {
                                    // code added on 17th feb by vinoth to hide complete conf and merge call button
                                    if (Settings.GetInstance().IsEnableInitiateTransfer || Settings.GetInstance().IsEnableInitiateConference)
                                    {
                                        messageToClient.NotifyUIStatus(ButtonStatusController.GetAlternateStatus());
                                    }
                                    //else if (Settings.GetInstance().IsEnableInitiateConference)
                                    //{
                                    //    messageToClient.NotifyUIStatus(ButtonStatusController.GetAlternateConfrenceStatus());
                                    //    //messageToClient.NotifyUIStatus(ButtonStatusController.GetAcceptConferenceStatus());
                                    //}
                                }
                            }
                            //End
                            if (eventRetrieved.CallType == CallType.Consult)
                            {
                                if (eventRetrieved.ConnID != null && eventRetrieved.TransferConnID != null)
                                {
                                    Settings.GetInstance().IsAlternateCallClicked = false;
                                }
                                else
                                {
                                    Settings.GetInstance().IsAlternateCallClicked = true;
                                }
                            }
                            break;

                        case EventAbandoned.MessageId:

                            EventAbandoned eventAbandoned = (EventAbandoned)message;
                            Settings.GetInstance().IsOnCall = false;
                            logger.Trace("Response from T-Server:" + eventAbandoned.ToString());
                            //Code commented on 23-02-2015 to avoid moving agent to not ready state without checking prev state
                            //messageToClient.NotifyUIStatus(ButtonStatusController.GetNotReadyStatus());
                            Settings.GetInstance().ConnectionID = string.Empty;
                            Settings.GetInstance().userData.Clear();
                            messageToClient.NotifySubscriber(VoiceEvents.EventAbandoned, Settings.GetInstance().userData.Clone());
                            try
                            {
                                var abandonedstatus = new AgentStatus();
                                abandonedstatus.AgentCurrentStatus = "Abandoned";
                                abandonedstatus.Extensions = null;
                                abandonedstatus.Reasons = null;
                                messageToClient.NotifyAgentStatus(abandonedstatus);
                                abandonedstatus = null;
                            }
                            catch (Exception readyAgentStatusException)
                            {
                                logger.Error("Error occurred while notifying agent abandoned status to subscriber "
                                    + readyAgentStatusException.ToString());
                            }

                            break;

                        case EventReleased.MessageId:
                            EventReleased eventReleased = (EventReleased)message;
                            logger.Trace("Response from T-Server:" + eventReleased.ToString());
                            Settings.GetInstance().ConnectionID = string.Empty;
                            //Settings.GetInstance().IsAlternateCallClicked = false;
                            Settings.GetInstance().StatusMessage = string.Empty;

                            SoftPhoneStatusController currentStatus = new SoftPhoneStatusController();

                            //if (eventReleased.ConnID != null && eventReleased.TransferConnID != null && eventReleased.CallType == CallType.Consult)
                            //{
                            //    SoftPhone soft = new SoftPhone();
                            //    IMessage iMessages = soft.GetCallStatusInfo(eventReleased.TransferConnID.ToString());
                            //    if (iMessages != null && iMessages.Id == EventPartyInfo.MessageId)
                            //    {
                            //        EventPartyInfo info = (EventPartyInfo)iMessages;
                            //        string data = info.Cause.ToString();
                            //    }
                            //    HoldingFlagStatus(PhoneFunctions.InitiateTransfer);
                            //}

                            if (eventReleased.CallType == CallType.Consult)
                            {
                                logger.Debug("Call type : " + eventReleased.CallType);
                                Settings.GetInstance().ConnectionID = eventReleased.TransferConnID.ToString();
                                if (Settings.GetInstance().IsEnableInitiateTransfer)
                                {
                                    logger.Debug("IsEnableInitiateTransfer : " + Settings.GetInstance().IsEnableInitiateTransfer);
                                    logger.Debug("IsCustomerReleaseTransferCallBeforeEstablish : " + Settings.GetInstance().IsCustomerReleaseTransferCallBeforeEstablish);
                                    logger.Debug("IsBothEndUsersReleaseTransferCallBeforeComplete : " + Settings.GetInstance().IsBothEndUsersReleaseTransferCallBeforeComplete);
                                    Settings.GetInstance().IsEnableInitiateTransfer = false;
                                    //Below condition added to re-solve issue REG - PHN 16
                                    //04-15-2013 Ram
                                    if (Settings.GetInstance().IsCustomerReleaseTransferCallBeforeEstablish)
                                    {
                                        TranferElseProcess(VoiceEvents.EventReleased);
                                        currentStatus = (ButtonStatusController.GetCallOnReleaseStatus(Settings.GetInstance().LogOffEnable));
                                        //Code Added - V.Palaniappan
                                        //14.10.2013 - To avoid agent status change once a customer has release the call before answering agent B on doing consult call
                                        Settings.GetInstance().IsBothEndUsersReleaseTransferCallBeforeComplete = false;
                                        //End
                                    }
                                    else //End
                                    {
                                        //Below condition added to re-solve issue REG - PHN 20
                                        //04-17-2013 Ram
                                        if (Settings.GetInstance().IsBothEndUsersReleaseTransferCallBeforeComplete)
                                        {
                                            logger.Debug("Both End Users released call at state IsBothEndUsersReleaseTransferCallBeforeComplete "
                                        + Settings.GetInstance().IsBothEndUsersReleaseTransferCallBeforeComplete.ToString());
                                            TranferElseProcess(VoiceEvents.EventReleased);

                                            currentStatus = (ButtonStatusController.GetCallOnReleaseStatus(Settings.GetInstance().LogOffEnable));
                                            Settings.GetInstance().IsBothEndUsersReleaseTransferCallBeforeComplete = false;
                                        }
                                        else
                                        {//End
                                            //Code Added - To avoid Disable button state while clicking on Reconnect Button
                                            //28.10.2013 - V.Palaniappan
                                            Settings.GetInstance().StatusMessage = "Consult";
                                            //End
                                            if (Settings.GetInstance().IsAlternateCallClicked)
                                            {
                                                logger.Debug("IsAlternateCallClicked : " + Settings.GetInstance().IsAlternateCallClicked);
                                                currentStatus = ButtonStatusController.GetOnCallStatus();
                                                //messageToClient.NotifyUIStatus(ButtonStatusController.GetOnCallStatus());
                                            }
                                            else
                                            {
                                                logger.Debug("IsAlternateCallClicked is false : Change status to CallOnHoldStatus");
                                                currentStatus = ButtonStatusController.GetCallOnHoldStatus();
                                                //messageToClient.NotifyUIStatus(ButtonStatusController.GetCallOnHoldStatus());
                                            }
                                        }
                                    }
                                }
                                else if (Settings.GetInstance().IsEnableInitiateConference)
                                {
                                    logger.Debug("IsEnableInitiateConference : " + Settings.GetInstance().IsEnableInitiateConference);
                                    Settings.GetInstance().IsEnableInitiateConference = false;
                                    //Below condition added to re-solve issue REG - PHN 19
                                    //04-17-2013 Ram
                                    if (Settings.GetInstance().IsCustomerReleaseConfCallBeforeEstablish)
                                    {
                                        logger.Debug("IsCustomerReleaseConfCallBeforeEstablish : " + Settings.GetInstance().IsCustomerReleaseConfCallBeforeEstablish);
                                        logger.Debug("Change status to GetCallOnReleaseStatus(Settings.GetInstance().LogOffEnable)");
                                        TranferElseProcess(VoiceEvents.EventReleased);

                                        currentStatus = (ButtonStatusController.GetCallOnReleaseStatus(Settings.GetInstance().LogOffEnable));
                                        //Code Added - V.Palaniappan
                                        //14.10.2013 - To avoid agent status change once a customer has release the call before answering agent B on doing consult call
                                        Settings.GetInstance().IsBothEndUsersReleaseConfCallBeforeComplete = false;
                                        //End
                                    }
                                    else //End
                                    {
                                        //Below condition added to re-solve issue REG - PHN 20
                                        //04-17-2013 Ram
                                        logger.Debug("IsBothEndUsersReleaseConfCallBeforeComplete : " + Settings.GetInstance().IsBothEndUsersReleaseConfCallBeforeComplete);
                                        if (Settings.GetInstance().IsBothEndUsersReleaseConfCallBeforeComplete)
                                        {
                                            logger.Debug("Both End Users released call at state IsBothEndUsersReleaseConfCallBeforeComplete "
                                        + Settings.GetInstance().IsBothEndUsersReleaseConfCallBeforeComplete.ToString());
                                            TranferElseProcess(VoiceEvents.EventReleased);
                                            logger.Debug("IsBothEndUsersReleaseConfCallBeforeComplete is true : change status to GetCallOnReleaseStatus(Settings.GetInstance().LogOffEnable)");

                                            currentStatus = (ButtonStatusController.GetCallOnReleaseStatus(Settings.GetInstance().LogOffEnable));

                                            Settings.GetInstance().IsBothEndUsersReleaseConfCallBeforeComplete = false;
                                        }
                                        else
                                        {//End
                                            //Code Added - To avoid Disable button state while clicking on Reconnect Button
                                            //28.10.2013 - V.Palaniappan
                                            Settings.GetInstance().StatusMessage = "Consult";
                                            //End
                                            logger.Debug("IsAlternateCallClicked : " + Settings.GetInstance().IsAlternateCallClicked);
                                            if (Settings.GetInstance().IsAlternateCallClicked)
                                            {
                                                logger.Debug("IsAlternateCallClicked : " + Settings.GetInstance().IsAlternateCallClicked + "move to GetOnCallStatus()");
                                                currentStatus = ButtonStatusController.GetOnCallStatus();
                                                //messageToClient.NotifyUIStatus(ButtonStatusController.GetOnCallStatus());
                                            }
                                            else
                                            {
                                                logger.Debug("IsAlternateCallClicked : " + Settings.GetInstance().IsAlternateCallClicked + "move to GetCallOnHoldStatus()");
                                                currentStatus = ButtonStatusController.GetCallOnHoldStatus();
                                                //messageToClient.NotifyUIStatus(ButtonStatusController.GetCallOnHoldStatus());
                                            }
                                        }
                                    }
                                }
                                else if (Settings.GetInstance().IsEnableCompleteTransfer)
                                {
                                    Settings.GetInstance().IsEnableCompleteTransfer = false;
                                    Settings.GetInstance().IsEnableInitiateTransfer = false;

                                    //Below condition added to re-solve issue REG - PHN 20
                                    //19-11-2013 V.Palaniappan
                                    logger.Debug("IsBothEndUsersReleaseTransferCallBeforeComplete : " + Settings.GetInstance().IsBothEndUsersReleaseTransferCallBeforeComplete);
                                    if (Settings.GetInstance().IsBothEndUsersReleaseTransferCallBeforeComplete)
                                    {
                                        logger.Debug("Both End Users released call at state IsBothEndUsersReleaseTransferCallBeforeComplete "
                                    + Settings.GetInstance().IsBothEndUsersReleaseTransferCallBeforeComplete.ToString());
                                        TranferElseProcess(VoiceEvents.EventReleased);
                                        currentStatus = (ButtonStatusController.GetCallOnReleaseStatus(Settings.GetInstance().LogOffEnable));
                                        Settings.GetInstance().IsBothEndUsersReleaseTransferCallBeforeComplete = false;
                                        //Code Added - V.Palaniappan
                                        //18.10.2013
                                        Settings.GetInstance().StatusMessage = "CompleteTransfer";
                                        //End
                                    }
                                    else
                                    {
                                        //Code Added - To avoid Disable button state while clicking on Reconnect Button
                                        //28.10.2013 - V.Palaniappan
                                        Settings.GetInstance().StatusMessage = "ConsultTransfer";
                                        //End
                                        logger.Debug("StatusMessage : " + Settings.GetInstance().StatusMessage);
                                        if (Settings.GetInstance().IsAlternateCallClicked)
                                        {
                                            logger.Debug("IsAlternateCallClicked : " + Settings.GetInstance().IsAlternateCallClicked + "Move to GetOnCallStatus()");
                                            currentStatus = ButtonStatusController.GetOnCallStatus();
                                            //messageToClient.NotifyUIStatus(ButtonStatusController.GetOnCallStatus());
                                        }
                                        else
                                        {
                                            logger.Debug("IsAlternateCallClicked : " + Settings.GetInstance().IsAlternateCallClicked + "Move to GetCallOnHoldStatus()");
                                            currentStatus = ButtonStatusController.GetCallOnHoldStatus();
                                            //messageToClient.NotifyUIStatus(ButtonStatusController.GetCallOnHoldStatus());
                                        }
                                    }
                                    //End

                                    //Below condition added to re-solve issue REG - PHN 16
                                    //04-15-2013 Ram
                                    Settings.GetInstance().IsCustomerReleaseTransferCallBeforeEstablish = false;
                                    Settings.GetInstance().IsCustomerReleaseTransferCallAfterEstablish = false;
                                    //End
                                }
                                else if (Settings.GetInstance().IsEnableCompleteConference)
                                {
                                    logger.Debug("IsEnableCompleteConference : " + Settings.GetInstance().IsEnableCompleteConference);
                                    Settings.GetInstance().IsEnableCompleteConference = false;
                                    Settings.GetInstance().IsEnableInitiateConference = false;
                                    //Below condition added to re-solve issue REG - PHN 19
                                    //04-17-2013 Ram
                                    Settings.GetInstance().IsCustomerReleaseConfCallAfterEstablish = false;
                                    Settings.GetInstance().IsCustomerReleaseConfCallBeforeEstablish = false;
                                    //End
                                    Settings.GetInstance().StatusMessage = "CompleteConference";
                                }
                                //Code Added - To resolve button status issue like while releasing the consultcall from testphone
                                //04.10.2013 - V.Palaniappan
                                else
                                {
                                    logger.Debug("else part Move to GetCallOnReleaseStatus(Settings.GetInstance().LogOffEnable)");
                                    currentStatus = ButtonStatusController.GetCallOnReleaseStatus(Settings.GetInstance().LogOffEnable);
                                    //messageToClient.NotifyUIStatus(ButtonStatusController.GetCallOnReleaseStatus(Settings.GetInstance().LogOffEnable));
                                }//End
                            }
                            else if (eventReleased.CallType == CallType.Inbound)
                            {
                                //Below condition added to re-solve issue REG - PHN 16
                                //04-15-2013 Ram
                                logger.Debug("IsEnableInitiateTransfer : " + Settings.GetInstance().IsEnableInitiateTransfer);
                                if (Settings.GetInstance().IsEnableInitiateTransfer)
                                {
                                    logger.Debug("Customer released call at state IsCustomerReleaseTransferCallAfterEstablish "
                                        + Settings.GetInstance().IsCustomerReleaseTransferCallAfterEstablish.ToString());
                                    if (Settings.GetInstance().IsCustomerReleaseTransferCallAfterEstablish)
                                    {
                                        //Below condition added to re-solve issue REG - PHN 20
                                        //04-17-2013 Ram
                                        Settings.GetInstance().IsBothEndUsersReleaseTransferCallBeforeComplete = true;
                                        //End

                                        currentStatus = ButtonStatusController.GetCallReleaseStatus();
                                        //messageToClient.NotifyUIStatus(ButtonStatusController.GetCallReleaseStatus());
                                        //Code Added - V.Palaniappan
                                        //14.10.2013 - To resolve issues like if customer release the call before completing the transfer call then
                                        //the connection id will get changed
                                        Settings.GetInstance().ConnectionID = Settings.GetInstance().ConsultConnectionID;
                                        //End
                                    }
                                    else
                                    {
                                        Settings.GetInstance().IsCustomerReleaseTransferCallBeforeEstablish = true;
                                        //Below condition added to re-solve issue REG - PHN 20
                                        //04-17-2013 Ram
                                        Settings.GetInstance().IsBothEndUsersReleaseTransferCallBeforeComplete = true;
                                        //End
                                        Settings.GetInstance().ConnectionID = Settings.GetInstance().ConsultationConnID;
                                        logger.Debug("Conn ID " + Settings.GetInstance().ConnectionID.ToString());
                                        currentStatus = ButtonStatusController.GetCallReleaseStatus();
                                        // messageToClient.NotifyUIStatus(ButtonStatusController.GetCallReleaseStatus());
                                    }
                                }
                                //Below condition added to re-solve issue REG - PHN 19
                                //04-17-2013 Ram
                                else if (Settings.GetInstance().IsEnableInitiateConference)
                                {
                                    logger.Debug("IsEnableInitiateConference : " + Settings.GetInstance().IsEnableInitiateConference);
                                    logger.Debug("Customer released call at state IsCustomerReleaseConfCallAfterEstablish "
                                        + Settings.GetInstance().IsCustomerReleaseConfCallAfterEstablish.ToString());
                                    if (Settings.GetInstance().IsCustomerReleaseConfCallAfterEstablish)
                                    {
                                        currentStatus = ButtonStatusController.GetCallReleaseStatus();
                                        //Below condition added to re-solve issue REG - PHN 20
                                        //04-17-2013 Ram
                                        Settings.GetInstance().IsBothEndUsersReleaseConfCallBeforeComplete = true;
                                        //End
                                        //Code Added - V.Palaniappan
                                        //14.10.2013 - To resolve issues like if customer release the call before completing the conference call then
                                        //the connection id will get changed
                                        Settings.GetInstance().ConnectionID = Settings.GetInstance().ConsultConnectionID;
                                        //End
                                    }
                                    else
                                    {
                                        Settings.GetInstance().IsCustomerReleaseConfCallBeforeEstablish = true;
                                        //Below condition added to re-solve issue REG - PHN 20
                                        //04-17-2013 Ram
                                        Settings.GetInstance().IsBothEndUsersReleaseConfCallBeforeComplete = true;
                                        //End
                                        Settings.GetInstance().ConnectionID = Settings.GetInstance().ConsultationConnID;
                                        logger.Debug("Conn ID " + Settings.GetInstance().ConnectionID.ToString());
                                        currentStatus = ButtonStatusController.GetCallReleaseStatus();
                                        //messageToClient.NotifyUIStatus(ButtonStatusController.GetCallReleaseStatus());
                                    }
                                }//End
                                else
                                {
                                    TranferElseProcess(VoiceEvents.EventReleased);
                                    currentStatus = (ButtonStatusController.GetCallOnReleaseStatus(Settings.GetInstance().LogOffEnable));
                                }
                                //End

                                //Code added by rajkumar for disposition form
                                //start

                                if (eventReleased.UserData != null && eventReleased.UserData.Count > 0)
                                {
                                    foreach (var keyname in eventReleased.UserData.AllKeys)
                                    {
                                        switch (keyname.ToLower())
                                        {
                                            case "calltype":
                                            case "dnis":
                                            case "mid":
                                                if (Settings.GetInstance().userData.ContainsKey(keyname))
                                                    Settings.GetInstance().userData[keyname] = eventReleased.UserData[keyname].ToString();
                                                else
                                                    Settings.GetInstance().userData.Add(keyname, eventReleased.UserData[keyname].ToString());
                                                break;

                                            case "connectionid":
                                                if (Settings.GetInstance().userData.ContainsKey("ConnectionId"))
                                                    Settings.GetInstance().userData["ConnectionId"] = eventReleased.UserData[keyname].ToString();
                                                else
                                                    Settings.GetInstance().userData.Add("ConnectionId", eventReleased.UserData[keyname].ToString());
                                                break;

                                            case "ani":
                                            case "otherDN":
                                                if (Settings.GetInstance().userData.ContainsKey("ANI") && string.IsNullOrEmpty(Settings.GetInstance().userData["ANI"].ToString()))
                                                    Settings.GetInstance().userData["ANI"] = eventReleased.UserData[keyname].ToString();
                                                else
                                                    Settings.GetInstance().userData.Add("ANI", eventReleased.UserData[keyname].ToString());
                                                break;

                                            default:
                                                if (Settings.GetInstance().userData.ContainsKey(keyname))
                                                    Settings.GetInstance().userData[keyname] = eventReleased.UserData[keyname].ToString();
                                                else
                                                    Settings.GetInstance().userData.Add(keyname, eventReleased.UserData[keyname].ToString());
                                                break;
                                        }
                                    }
                                }

                                if (!string.IsNullOrEmpty(eventReleased.CallType.ToString()))
                                    if (Settings.GetInstance().userData.ContainsKey("CallType"))
                                        Settings.GetInstance().userData["CallType"] = eventReleased.CallType.ToString();
                                    else
                                        Settings.GetInstance().userData.Add("CallType", eventReleased.CallType.ToString());

                                if (eventReleased.ConnID != null && !string.IsNullOrEmpty(eventReleased.ConnID.ToString()))
                                    if (Settings.GetInstance().userData.ContainsKey("ConnectionId"))
                                        Settings.GetInstance().userData["ConnectionId"] = eventReleased.ConnID.ToString();
                                    else
                                        Settings.GetInstance().userData.Add("ConnectionId", eventReleased.ConnID.ToString());

                                if (eventReleased.ANI != null && !string.IsNullOrEmpty(eventReleased.ANI.ToString()))
                                    if (Settings.GetInstance().userData.ContainsKey("ANI"))
                                        Settings.GetInstance().userData["ANI"] = eventReleased.ANI.ToString();
                                    else
                                        Settings.GetInstance().userData.Add("ANI", eventReleased.ANI.ToString());

                                if (eventReleased.DNIS != null && !string.IsNullOrEmpty(eventReleased.DNIS.ToString()))
                                    if (Settings.GetInstance().userData.ContainsKey("DNIS"))
                                        Settings.GetInstance().userData["DNIS"] = eventReleased.DNIS.ToString();
                                    else
                                        Settings.GetInstance().userData.Add("DNIS", eventReleased.DNIS.ToString());
                                //end

                                messageToClient.NotifySubscriber(VoiceEvents.EventReleased, Settings.GetInstance().userData.Clone());
                            }
                            else if (eventReleased.CallType == CallType.Internal)
                            {
                                //Below condition added to re-solve issue REG - PHN 16
                                //04-15-2013 Ram
                                logger.Debug("IsEnableInitiateTransfer : " + Settings.GetInstance().IsEnableInitiateTransfer);
                                if (Settings.GetInstance().IsEnableInitiateTransfer)
                                {
                                    logger.Debug("Customer released call at state IsCustomerReleaseTransferCallAfterEstablish "
                                        + Settings.GetInstance().IsCustomerReleaseTransferCallAfterEstablish.ToString());
                                    if (Settings.GetInstance().IsCustomerReleaseTransferCallAfterEstablish)
                                    {
                                        //Below condition added to re-solve issue REG - PHN 20
                                        //04-17-2013 Ram
                                        Settings.GetInstance().IsBothEndUsersReleaseTransferCallBeforeComplete = true;
                                        //End
                                        currentStatus = ButtonStatusController.GetCallReleaseStatus();
                                        //messageToClient.NotifyUIStatus(ButtonStatusController.GetCallReleaseStatus());
                                        //Code Added - V.Palaniappan
                                        //14.10.2013 - To resolve issues like if customer release the call before completing the transfer call then
                                        //the connection id will get changed
                                        Settings.GetInstance().ConnectionID = Settings.GetInstance().ConsultConnectionID;
                                        //End
                                    }
                                    else
                                    {
                                        Settings.GetInstance().IsCustomerReleaseTransferCallBeforeEstablish = true;
                                        //Below condition added to re-solve issue REG - PHN 20
                                        //04-17-2013 Ram
                                        Settings.GetInstance().IsBothEndUsersReleaseTransferCallBeforeComplete = true;
                                        //End
                                        Settings.GetInstance().ConnectionID = Settings.GetInstance().ConsultationConnID;
                                        logger.Debug("Conn ID " + Settings.GetInstance().ConnectionID.ToString());
                                        currentStatus = ButtonStatusController.GetCallReleaseStatus();
                                        // messageToClient.NotifyUIStatus(ButtonStatusController.GetCallReleaseStatus());
                                    }
                                }
                                //Below condition added to re-solve issue REG - PHN 19
                                //04-17-2013 Ram
                                else if (Settings.GetInstance().IsEnableInitiateConference)
                                {
                                    logger.Debug("IsEnableInitiateConference : " + Settings.GetInstance().IsEnableInitiateConference);
                                    logger.Debug("Customer released call at state IsCustomerReleaseConfCallAfterEstablish "
                                        + Settings.GetInstance().IsCustomerReleaseConfCallAfterEstablish.ToString());
                                    if (Settings.GetInstance().IsCustomerReleaseConfCallAfterEstablish)
                                    {
                                        currentStatus = ButtonStatusController.GetCallReleaseStatus();
                                        //messageToClient.NotifyUIStatus(ButtonStatusController.GetCallReleaseStatus());
                                        //Below condition added to re-solve issue REG - PHN 20
                                        //04-17-2013 Ram
                                        Settings.GetInstance().IsBothEndUsersReleaseConfCallBeforeComplete = true;
                                        //End
                                        //Code Added - V.Palaniappan
                                        //14.10.2013 - To resolve issues like if customer release the call before completing the Conference call then
                                        //the connection id will get changed
                                        Settings.GetInstance().ConnectionID = Settings.GetInstance().ConsultConnectionID;
                                        //End
                                    }
                                    else
                                    {
                                        Settings.GetInstance().IsCustomerReleaseConfCallBeforeEstablish = true;
                                        //Below condition added to re-solve issue REG - PHN 20
                                        //04-17-2013 Ram
                                        Settings.GetInstance().IsBothEndUsersReleaseConfCallBeforeComplete = true;
                                        //End
                                        Settings.GetInstance().ConnectionID = Settings.GetInstance().ConsultationConnID;
                                        logger.Debug("Conn ID " + Settings.GetInstance().ConnectionID.ToString());
                                        currentStatus = ButtonStatusController.GetCallReleaseStatus();
                                        //messageToClient.NotifyUIStatus(ButtonStatusController.GetCallReleaseStatus());
                                    }
                                }//End
                                else
                                {
                                    TranferElseProcess(VoiceEvents.EventReleased);

                                    currentStatus = (ButtonStatusController.GetCallOnReleaseStatus(Settings.GetInstance().LogOffEnable));
                                }
                                //End
                            }
                            // Code Implemented on 04/02/2015 for BCBS Outbound issue scenario
                            //start
                            else if (eventReleased.CallType == CallType.Outbound)
                            {
                                //Below condition added to re-solve issue REG - PHN 16
                                //04-15-2013 Ram
                                logger.Debug("IsEnableInitiateTransfer : " + Settings.GetInstance().IsEnableInitiateTransfer);
                                if (Settings.GetInstance().IsEnableInitiateTransfer)
                                {
                                    logger.Info("Customer released call at state IsCustomerReleaseTransferCallAfterEstablish "
                                        + Settings.GetInstance().IsCustomerReleaseTransferCallAfterEstablish.ToString());
                                    if (Settings.GetInstance().IsCustomerReleaseTransferCallAfterEstablish)
                                    {
                                        //Below condition added to re-solve issue REG - PHN 20
                                        //04-17-2013 Ram
                                        Settings.GetInstance().IsBothEndUsersReleaseTransferCallBeforeComplete = true;
                                        //End
                                        currentStatus = ButtonStatusController.GetCallReleaseStatus();
                                        //messageToClient.NotifyUIStatus(ButtonStatusController.GetCallReleaseStatus());
                                        //Code Added - V.Palaniappan
                                        //14.10.2013 - To resolve issues like if customer release the call before completing the transfer call then
                                        //the connection id will get changed
                                        Settings.GetInstance().ConnectionID = Settings.GetInstance().ConsultConnectionID;
                                        //End
                                    }
                                    else
                                    {
                                        Settings.GetInstance().IsCustomerReleaseTransferCallBeforeEstablish = true;
                                        //Below condition added to re-solve issue REG - PHN 20
                                        //04-17-2013 Ram
                                        Settings.GetInstance().IsBothEndUsersReleaseTransferCallBeforeComplete = true;
                                        //End
                                        Settings.GetInstance().ConnectionID = Settings.GetInstance().ConsultationConnID;
                                        logger.Info("Conn ID " + Settings.GetInstance().ConnectionID.ToString());
                                        currentStatus = ButtonStatusController.GetCallReleaseStatus();
                                        //messageToClient.NotifyUIStatus(ButtonStatusController.GetCallReleaseStatus());
                                    }
                                }
                                //Below condition added to re-solve issue REG - PHN 19
                                //04-17-2013 Ram
                                else if (Settings.GetInstance().IsEnableInitiateConference)
                                {
                                    logger.Debug("IsEnableInitiateConference : " + Settings.GetInstance().IsEnableInitiateConference);
                                    logger.Info("Customer released call at state IsCustomerReleaseConfCallAfterEstablish "
                                        + Settings.GetInstance().IsCustomerReleaseConfCallAfterEstablish.ToString());
                                    if (Settings.GetInstance().IsCustomerReleaseConfCallAfterEstablish)
                                    {
                                        currentStatus = ButtonStatusController.GetCallReleaseStatus();
                                        //messageToClient.NotifyUIStatus(ButtonStatusController.GetCallReleaseStatus());
                                        //Below condition added to re-solve issue REG - PHN 20
                                        //04-17-2013 Ram
                                        Settings.GetInstance().IsBothEndUsersReleaseConfCallBeforeComplete = true;
                                        //End
                                        //Code Added - V.Palaniappan
                                        //14.10.2013 - To resolve issues like if customer release the call before completing the Conference call then
                                        //the connection id will get changed
                                        Settings.GetInstance().ConnectionID = Settings.GetInstance().ConsultConnectionID;
                                        //End
                                    }
                                    else
                                    {
                                        Settings.GetInstance().IsCustomerReleaseConfCallBeforeEstablish = true;
                                        //Below condition added to re-solve issue REG - PHN 20
                                        //04-17-2013 Ram
                                        Settings.GetInstance().IsBothEndUsersReleaseConfCallBeforeComplete = true;
                                        //End
                                        Settings.GetInstance().ConnectionID = Settings.GetInstance().ConsultationConnID;
                                        logger.Info("Conn ID " + Settings.GetInstance().ConnectionID.ToString());
                                        currentStatus = ButtonStatusController.GetCallReleaseStatus();
                                        //messageToClient.NotifyUIStatus(ButtonStatusController.GetCallReleaseStatus());
                                    }
                                }//End
                                else
                                {
                                    TranferElseProcess(VoiceEvents.EventReleased);
                                    currentStatus = (ButtonStatusController.GetCallOnReleaseStatus(Settings.GetInstance().LogOffEnable));
                                }
                                //End

                                //Code added by rajkumar for disposition form
                                //start

                                if (eventReleased.UserData != null && eventReleased.UserData.Count > 0)
                                {
                                    foreach (var keyname in eventReleased.UserData.AllKeys)
                                    {
                                        switch (keyname.ToLower())
                                        {
                                            case "calltype":
                                            case "dnis":
                                            case "mid":
                                                if (Settings.GetInstance().userData.ContainsKey(keyname))
                                                    Settings.GetInstance().userData[keyname] = eventReleased.UserData[keyname].ToString();
                                                else
                                                    Settings.GetInstance().userData.Add(keyname, eventReleased.UserData[keyname].ToString());
                                                break;

                                            case "connectionid":
                                                if (Settings.GetInstance().userData.ContainsKey("ConnectionId"))
                                                    Settings.GetInstance().userData["ConnectionId"] = eventReleased.UserData[keyname].ToString();
                                                else
                                                    Settings.GetInstance().userData.Add("ConnectionId", eventReleased.UserData[keyname].ToString());
                                                break;

                                            case "ani":
                                            case "otherdn":
                                                if (Settings.GetInstance().userData.ContainsKey("ANI") && string.IsNullOrEmpty(Settings.GetInstance().userData["ANI"].ToString()))
                                                    Settings.GetInstance().userData["ANI"] = eventReleased.UserData[keyname].ToString();
                                                else
                                                    Settings.GetInstance().userData.Add("ANI", eventReleased.UserData[keyname].ToString());
                                                break;

                                            default:
                                                if (Settings.GetInstance().userData.ContainsKey(keyname))
                                                    Settings.GetInstance().userData[keyname] = eventReleased.UserData[keyname].ToString();
                                                else
                                                    Settings.GetInstance().userData.Add(keyname, eventReleased.UserData[keyname].ToString());
                                                break;
                                        }
                                    }
                                }

                                if (eventReleased.CallType != null && !string.IsNullOrEmpty(eventReleased.CallType.ToString()))
                                    if (Settings.GetInstance().userData.ContainsKey("CallType"))
                                        Settings.GetInstance().userData["CallType"] = eventReleased.CallType.ToString();
                                    else
                                        Settings.GetInstance().userData.Add("CallType", eventReleased.CallType.ToString());

                                if (eventReleased.ConnID != null && !string.IsNullOrEmpty(eventReleased.ConnID.ToString()))
                                    if (Settings.GetInstance().userData.ContainsKey("ConnectionId"))
                                        Settings.GetInstance().userData["ConnectionId"] = eventReleased.ConnID.ToString();
                                    else
                                        Settings.GetInstance().userData.Add("ConnectionId", eventReleased.ConnID.ToString());

                                if (eventReleased.ANI != null && !string.IsNullOrEmpty(eventReleased.ANI.ToString()))
                                    if (Settings.GetInstance().userData.ContainsKey("ANI"))
                                        Settings.GetInstance().userData["ANI"] = eventReleased.ANI.ToString();
                                    else
                                        Settings.GetInstance().userData.Add("ANI", eventReleased.ANI.ToString());

                                if (eventReleased.OtherDN != null && !string.IsNullOrEmpty(eventReleased.OtherDN.ToString())
                                    && !(Settings.GetInstance().userData.ContainsKey("ANI")))
                                    Settings.GetInstance().userData.Add("ANI", eventReleased.OtherDN.ToString());

                                if (eventReleased.DNIS != null && !string.IsNullOrEmpty(eventReleased.DNIS.ToString()))
                                    if (Settings.GetInstance().userData.ContainsKey("DNIS"))
                                        Settings.GetInstance().userData["DNIS"] = eventReleased.DNIS.ToString();
                                    else
                                        Settings.GetInstance().userData.Add("DNIS", eventReleased.DNIS.ToString());
                                //end

                                messageToClient.NotifySubscriber(VoiceEvents.EventReleased, Settings.GetInstance().userData.Clone());
                            }
                            //end
                            else
                            {
                                Settings.GetInstance().userData.Clear();
                                Settings.GetInstance().ConsultConnectionID = string.Empty;
                                messageToClient.NotifySubscriber(VoiceEvents.EventReleased, Settings.GetInstance().userData.Clone());
                                currentStatus = ButtonStatusController.GetCallOnReleaseStatus(Settings.GetInstance().LogOffEnable);
                                // messageToClient.NotifyUIStatus(ButtonStatusController.GetCallOnReleaseStatus(Settings.GetInstance().LogOffEnable));
                            }

                            var releasestatus = new AgentStatus();
                            if (!Settings.GetInstance().IsAlternateCallClicked)
                            {
                                logger.Debug("IsAlternateCallClicked : " + Settings.GetInstance().IsAlternateCallClicked);
                                //Code Added - To handle the agent status if customer release the call before completing consult call
                                //14.10.2013 - V.Palaniappan
                                if (Settings.GetInstance().IsBothEndUsersReleaseTransferCallBeforeComplete || Settings.GetInstance().IsBothEndUsersReleaseConfCallBeforeComplete)
                                {
                                    logger.Debug("IsBothEndUsersReleaseTransferCallBeforeComplete : " + Settings.GetInstance().IsBothEndUsersReleaseTransferCallBeforeComplete);
                                    logger.Debug("IsBothEndUsersReleaseConfCallBeforeComplete : " + Settings.GetInstance().IsBothEndUsersReleaseConfCallBeforeComplete);
                                    releasestatus.AgentCurrentStatus = CurrentAgentStatus.OnRelease.ToString() + "@" + "IsBothEndUserRelease";
                                    logger.Debug("Releasestatus.AgentCurrentStatus : " + releasestatus.AgentCurrentStatus);
                                }
                                else
                                {
                                    if (!Settings.GetInstance().StatusMessage.Equals(string.Empty))
                                    {
                                        //---------------------------------------------------------------------------
                                        //releasestatus.AgentCurrentStatus = CurrentAgentStatus.OnHeld.ToString();
                                        releasestatus.AgentCurrentStatus = CurrentAgentStatus.OnRelease.ToString() + "&" + Settings.GetInstance().StatusMessage;
                                        logger.Debug("Status message : " + releasestatus.AgentCurrentStatus.ToString());
                                    }
                                    else
                                    {
                                        releasestatus.AgentCurrentStatus = CurrentAgentStatus.OnRelease.ToString() + Settings.GetInstance().StatusMessage;
                                        //code added by Manikandan on 18/11/2014
                                        //for resolving issue of transfer a call to agent B and agent B again make conf call to this agent and agent B releases the call
                                        //This agent status is on call but displaying as Hold due to Settings.GetInstance()._isCallHeld = true set in Event Held;
                                        Settings.GetInstance().IsCallHeld = false;
                                        Settings.GetInstance().IsOnCall = false;
                                        Settings.GetInstance().IsEnableCompleteTransfer = false;
                                        Settings.GetInstance().IsEnableInitiateTransfer = false;
                                        Settings.GetInstance().IsEnableCompleteConference = false;
                                        Settings.GetInstance().IsEnableCompleteTransfer = false;
                                        //Settings.GetInstance().userData.Clear();
                                        logger.Debug("AgentCurrentStatus : " + releasestatus.AgentCurrentStatus);
                                        //End
                                    }
                                }//End
                            }
                            else
                            {
                                Settings.GetInstance().IsAlternateCallClicked = false;
                                releasestatus.AgentCurrentStatus = CurrentAgentStatus.OnCall.ToString();
                                logger.Debug("AgentCurrentStatus : " + releasestatus.AgentCurrentStatus);
                            }
                            releasestatus.Extensions = null;
                            releasestatus.Reasons = null;
                            logger.Debug("------------EventReleased functionalities------------");
                            logger.Debug("Current Agent Status : " + releasestatus.AgentCurrentStatus);
                            logger.Debug("Current UI status: " + currentStatus.ToString());
                            messageToClient.NotifyUIStatus(currentStatus);
                            messageToClient.NotifyAgentStatus(releasestatus);
                            releasestatus = null;
                            break;

                        case EventPartyAdded.MessageId:

                            logger.Info("Party added to the Conference Call ");
                            EventPartyAdded partyAdded = (EventPartyAdded)message;
                            logger.Info("Response from T-Server " + partyAdded.ToString());

                            //Conference initiated and completed by this agent
                            if (partyAdded.ThisDN == partyAdded.ThirdPartyDN && partyAdded.ThirdPartyDNRole == DNRole.RoleAddedBy)
                            {
                                if (Settings.GetInstance().IsDeleteConfEnabled)
                                {
                                    logger.Debug("Party added by : " + partyAdded.ThisDN);
                                    logger.Debug("Party added : " + partyAdded.OtherDN);
                                    //Delete conf is enabled, so move to delete conf status
                                    messageToClient.NotifyUIStatus(ButtonStatusController.GetDeleteConferenceStatus());
                                }
                                else
                                {
                                    //Delete conf is disabled, so move to on call status
                                    logger.Info("Delete From conference feature disabled ");
                                    messageToClient.NotifyUIStatus(ButtonStatusController.GetOnCallStatus());
                                }
                                try
                                {
                                    var partyAddedStatus = new AgentStatus();

                                    partyAddedStatus.AgentCurrentStatus = CurrentAgentStatus.OnCall.ToString();
                                    partyAddedStatus.ConnId = partyAdded.ConnID.ToString();
                                    partyAddedStatus.ExactEvent = VoiceEvents.EventEstablished;

                                    messageToClient.NotifyAgentStatus(partyAddedStatus);
                                    partyAddedStatus = null;
                                }
                                catch (Exception partyAddedStatusException)
                                {
                                    logger.Error("Error occurred while notifying agent on call status to subscriber "
                                        + partyAddedStatusException.ToString());
                                }

                                //Below condition added to re-solve issue on showing third agent contact after complete conf/tran
                                //if this is not the perfect code here pls notify me team...
                                //may-18th-2013 vinoth
                                Settings.GetInstance().userData.Clear();

                                Settings.GetInstance().userData.Add("CallType", partyAdded.CallType.ToString());
                                Settings.GetInstance().userData.Add("ConnectionId", partyAdded.ConnID.ToString());
                                Settings.GetInstance().userData.Add("CallerId", partyAdded.CallID.ToString());
                                if (partyAdded.DNIS != null)
                                {
                                    Settings.GetInstance().userData.Add("DNIS", partyAdded.DNIS.ToString());
                                }
                                if (partyAdded.ANI != null)
                                {
                                    Settings.GetInstance().userData.Add("ANI", partyAdded.ANI.ToString());
                                }
                                if (partyAdded.OtherDN != null)
                                {
                                    Settings.GetInstance().userData.Add("OtherDN", partyAdded.OtherDN.ToString());
                                }
                                if (partyAdded.UserData != null)
                                {
                                    KeyValueCollection data = partyAdded.UserData;

                                    if (data != null && !data.Equals(string.Empty))
                                    {
                                        foreach (string keys in data.Keys)
                                        {
                                            if (keys != Settings.GetInstance().DispositionCollectionKey)
                                            {
                                                if (!Settings.GetInstance().userData.ContainsKey(keys))
                                                    Settings.GetInstance().userData.Add(keys.ToString(), data[keys].ToString());
                                            }
                                            else
                                            {
                                                if (data[keys] is KeyValueCollection)
                                                {
                                                    KeyValueCollection tempCollection = data[keys] as KeyValueCollection;
                                                    string result = string.Empty;
                                                    foreach (string item in tempCollection.Keys)
                                                    {
                                                        result += item + ":" + tempCollection[item] + "; ";
                                                    }
                                                    if (!string.IsNullOrEmpty(result))
                                                        result = result.Substring(0, (result.Length - 2));

                                                    if (!Settings.GetInstance().userData.ContainsKey(keys))
                                                        Settings.GetInstance().userData.Add(keys.ToString(), result);
                                                    else
                                                        Settings.GetInstance().userData[keys.ToString()] = result;
                                                }
                                                else
                                                {
                                                    if (Settings.GetInstance().userData.ContainsKey(keys))
                                                        Settings.GetInstance().userData.Remove(keys.ToString());
                                                }
                                            }
                                        }
                                    }
                                }
                                messageToClient.NotifySubscriber(VoiceEvents.EventPartyAdded, Settings.GetInstance().userData.Clone());
                                //End
                            }
                            //End
                            break;

                        #region Old Code

                        //logger.Debug("Party added to the Conference Call ");
                        //EventPartyAdded partyAdded = (EventPartyAdded)message;
                        //logger.Trace("Response from T-Server " + partyAdded.ToString());

                        ////Below condition added to avoid customer to add more people on conference
                        //switch (Settings.GetInstance().ConferenceLimit)
                        //{
                        //    case 2:
                        //        //Below condition added to re-solve issue REG - PHN 15
                        //        //04-15-2013 Ram
                        //        Settings.GetInstance().IsAgentOnConferenceCall = true;
                        //        //End
                        //        messageToClient.NotifyUIStatus(ButtonStatusController.GetCompleteConferenceStatus());
                        //        break;

                        //    default:
                        //        messageToClient.NotifyUIStatus(ButtonStatusController.GetDeleteConferenceStatus());
                        //        break;
                        //}
                        //break;

                        #endregion Old Code

                        case EventPartyDeleted.MessageId:
                            logger.Info("Party deleted from the conference call ");
                            EventPartyDeleted partyDeleted = (EventPartyDeleted)message;
                            Settings.GetInstance().ConnectionID = partyDeleted.ConnID.ToString();
                            logger.Info("Response from T-Server " + partyDeleted.ToString());
                            //Below condition added to re-solve issue REG - PHN 15
                            //04-15-2013 Ram
                            Settings.GetInstance().IsAgentOnConferenceCall = false;
                            //End

                            if (partyDeleted.ThirdPartyDNRole == DNRole.RoleObserver || partyDeleted.OtherDNRole == DNRole.RoleObserver)
                            {
                                //Other party DN or Third party DN is Observer, so dont change the UI
                                logger.Debug("ThirdpartyDNRole : " + partyDeleted.ThirdPartyDNRole);
                                logger.Debug("OtherDNRole : " + partyDeleted.OtherDNRole);
                            }
                            else
                            {
                                //DN's are not observer, hence handle the status
                                logger.Debug("Call Type : " + partyDeleted.CallType);
                                logger.Debug("Settings.GetInstance()._isCallHeld : " + Settings.GetInstance().IsCallHeld);

                                //Code added by vinoth on 17th feb.
                                //To get connection ID status and assign ui state according to that.
                                //I think, passing connection ID should be analyzed if any issues occurs in this event.
                                //Possible solutions passing main call connection ID or consult call connection ID should be handled
                                //based on scenarios.

                                RequestQuerycall.DoRequestQueryCall(Settings.GetInstance().CallControl == "both" ? Settings.GetInstance().ActiveDN : (Settings.GetInstance().CallControl == "acd" ?
                                                              Settings.GetInstance().ACDPosition : Settings.GetInstance().ExtensionDN), Settings.GetInstance().ConnectionID);

                                //Commented by vinoth on 17th feb.
                                //No need to assign the ui based on this event.
                                //if (Settings.GetInstance()._isCallHeld)
                                //{
                                //    logger.Debug("Settings.GetInstance()._isCallHeld : " + Settings.GetInstance()._isCallHeld + " Move button state to On Hold Status");
                                //    messageToClient.NotifyUIStatus(ButtonStatusController.GetCallOnHoldStatus());
                                //}
                                //else
                                //{
                                //    logger.Debug("Settings.GetInstance()._isCallHeld : " + Settings.GetInstance()._isCallHeld + " Move button state to On Call Status");
                                //    messageToClient.NotifyUIStatus(ButtonStatusController.GetOnCallStatus());
                                //}
                            }

                            break;

                        case EventUnregistered.MessageId:
                            EventUnregistered unRegister = (EventUnregistered)message;
                            logger.Trace("Response from T-Server " + unRegister.ToString());
                            Settings.GetInstance().IsDNRegistered = false;
                            //messageToClient.NotifyUIStatus(ButtonStatusController.GetLogoutStatus());
                            break;

                        case EventUserEvent.MessageId:
                            //code Added to invoke BroadCastMessage
                            //19.08.2013
                            EventUserEvent userEvent = (EventUserEvent)message;
                            if (userEvent.UserData != null)
                            {
                                logger.Trace("Response from T-Server " + userEvent.ToString());
                                if (Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections != null)
                                {
                                    if (Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections.ContainsKey(Pointel.Interactions.IPlugins.Plugins.OutboundPreview))
                                    {
                                        ((IOutboundPlugin)Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections[Pointel.Interactions.IPlugins.Plugins.OutboundPreview]).NotifyEventMessage(Pointel.Interactions.IPlugins.MediaTypes.Voice, message);
                                    }
                                }
                                if (userEvent.UserData.ContainsKey("IWS_Message"))
                                    messageToClient.NotifySubscriber(VoiceEvents.EventUserEvent, userEvent.UserData);
                            }
                            break;

                        case EventDNDOn.MessageId:
                            EventDNDOn dndON = (EventDNDOn)message;
                            logger.Trace("Response from T-Server " + dndON.ToString());
                            logger.Debug("Agent Moved to DNDOn State via DN:" + dndON.ThisDN);
                            try
                            {
                                var dndONStatus = new AgentStatus();

                                dndONStatus.AgentCurrentStatus = CurrentAgentStatus.DNDOn.ToString();
                                dndONStatus.Extensions = null;
                                dndONStatus.Reasons = null;

                                messageToClient.NotifyAgentStatus(dndONStatus);
                                dndONStatus = null;
                            }
                            catch (Exception dndONAgentStatusException)
                            {
                                logger.Error("Error occurred while notifying agent DNDOn status to subscriber "
                                    + dndONAgentStatusException.ToString());
                            }
                            break;

                        case EventDNDOff.MessageId:
                            EventDNDOff dndOFF = (EventDNDOff)message;
                            logger.Trace("Response from T-Server " + dndOFF.ToString());
                            logger.Debug("Agent Moved to DNDOff State via DN:" + dndOFF.ThisDN);
                            try
                            {
                                var dndOFFStatus = new AgentStatus();

                                dndOFFStatus.AgentCurrentStatus = CurrentAgentStatus.DNDOff.ToString();
                                dndOFFStatus.Extensions = null;
                                dndOFFStatus.Reasons = null;

                                messageToClient.NotifyAgentStatus(dndOFFStatus);
                                dndOFFStatus = null;
                            }
                            catch (Exception dndOFFAgentStatusException)
                            {
                                logger.Error("Error occurred while notifying agent DNDOff status to subscriber "
                                    + dndOFFAgentStatusException.ToString());
                            }
                            break;

                        default:
                            logger.Info("Unhandled Events:" + message.ToString());
                            break;
                    }
                }
                else
                {
                    logger.Warn("Null Message from TServer ");
                }
            }
            catch (Exception commonException)
            {
                logger.Error("ReportingVoiceMessage : Error occurred while invoking the events about TServer "
                    + commonException.ToString());
            }
        }

        public void Subscribe(ISoftphoneListener listener, SoftPhoneSubscriber list)
        {
            if (list == SoftPhoneSubscriber.Integration)
            {
                if (!messageToIntegration.Contains(listener))
                {
                    messageToIntegration.Add(listener);
                }
                else
                {
                    messageToClient = listener;
                }
            }
            else
            {
                messageToClient = listener;
            }
        }

        private static void TranferElseProcess(VoiceEvents voiceEventType)
        {
            try
            {
                Settings.GetInstance().userData.Clear();
                messageToClient.NotifySubscriber(voiceEventType, Settings.GetInstance().userData.Clone());
                //messageToClient.NotifyUIStatus(ButtonStatusController.GetCallOnReleaseStatus(Settings.GetInstance().LogOffEnable));
                Settings.GetInstance().IsCustomerReleaseTransferCallBeforeEstablish = false;
                Settings.GetInstance().IsCustomerReleaseTransferCallAfterEstablish = false;
                Settings.GetInstance().IsCustomerReleaseConfCallBeforeEstablish = false;
                Settings.GetInstance().IsCustomerReleaseConfCallAfterEstablish = false;
            }
            catch (Exception commonException)
            {
                logger.Error("Error occurred while executing transfer else process" + commonException.ToString());
            }
        }

        private static void VoiceManager_Closed(object sender, EventArgs e)
        {
            logger.Trace("TServer Protocol Closed");
            if (protocolClosed == 0)
            {
                //Code added - get the server disconnect details
                //smoorthy
                Settings.GetInstance().IsTserverFailed = true;
                var eventLinkDisconnectedStatus = new AgentStatus();
                eventLinkDisconnectedStatus.AgentCurrentStatus = "ServerDisconnected";
                eventLinkDisconnectedStatus.Extensions = null;
                eventLinkDisconnectedStatus.Reasons = null;
                messageToClient.NotifyAgentStatus(eventLinkDisconnectedStatus);
                eventLinkDisconnectedStatus = null;
                // messageToClient.NotifyUIStatus(ButtonStatusController.GetAllButtonDisableStatus());
                protocolClosed++;
                protocolOpened = 0;
                isAfterConnect = true;
                //end
            }
        }

        private static void VoiceManager_Opened(object sender, EventArgs e)
        {
            logger.Trace("TServer Protocol Opened");
            if (protocolOpened == 0 && isAfterConnect)
            {
                //Code added - get the server connected details
                //smoorthy
                Settings.GetInstance().IsTserverFailed = false;
                var eventLinkDisconnectedStatus = new AgentStatus();
                eventLinkDisconnectedStatus.AgentCurrentStatus = "ServerConnected";
                eventLinkDisconnectedStatus.Extensions = null;
                eventLinkDisconnectedStatus.Reasons = null;
                messageToClient.NotifyAgentStatus(eventLinkDisconnectedStatus);
                eventLinkDisconnectedStatus = null;
                messageToClient.NotifyUIStatus(ButtonStatusController.GetInitialSoftPhoneStatus());
                protocolOpened++;
                protocolClosed = 0;
                isAfterConnect = false;
                //end
            }
        }

        /// <summary>
        /// Holdings the flag status.
        /// </summary>
        /// <param name="value">The value.</param>
        private void HoldingFlagStatus(PhoneFunctions value)
        {
            switch (value)
            {
                case PhoneFunctions.InitiateTransfer:
                    Settings.GetInstance().IsEnableInitiateTransfer = true;
                    Settings.GetInstance().IsEnableCompleteTransfer = false;
                    Settings.GetInstance().IsEnableInitiateConference = false;
                    Settings.GetInstance().IsEnableCompleteConference = false;
                    break;

                case PhoneFunctions.IntiateConference:
                    Settings.GetInstance().IsEnableInitiateTransfer = false;
                    Settings.GetInstance().IsEnableCompleteTransfer = false;
                    Settings.GetInstance().IsEnableInitiateConference = true;
                    Settings.GetInstance().IsEnableCompleteConference = false;
                    break;

                case PhoneFunctions.CompleteTransfer:
                    Settings.GetInstance().IsEnableInitiateTransfer = false;
                    Settings.GetInstance().IsEnableCompleteTransfer = true;
                    Settings.GetInstance().IsEnableInitiateConference = false;
                    Settings.GetInstance().IsEnableCompleteConference = false;
                    //Below condition added to re-solve issue REG - PHN 16
                    //04-15-2013 Ram
                    Settings.GetInstance().IsCustomerReleaseTransferCallAfterEstablish = false;
                    Settings.GetInstance().IsCustomerReleaseTransferCallBeforeEstablish = false;
                    //End
                    break;

                case PhoneFunctions.CompleteConference:
                    Settings.GetInstance().IsEnableInitiateTransfer = false;
                    Settings.GetInstance().IsEnableCompleteTransfer = false;
                    Settings.GetInstance().IsEnableInitiateConference = false;
                    Settings.GetInstance().IsEnableCompleteConference = true;
                    //Below condition added to re-solve issue REG - PHN 19
                    //04-17-2013 Ram
                    Settings.GetInstance().IsCustomerReleaseConfCallAfterEstablish = false;
                    Settings.GetInstance().IsCustomerReleaseConfCallBeforeEstablish = false;
                    //End
                    break;

                default:
                    Settings.GetInstance().IsEnableInitiateTransfer = false;
                    Settings.GetInstance().IsEnableCompleteTransfer = false;
                    Settings.GetInstance().IsEnableInitiateConference = false;
                    Settings.GetInstance().IsEnableCompleteConference = false;
                    break;
            }
        }

        #endregion Methods

        #region Other

        /// <summary>
        /// This method used to create singleton object for this class
        /// </summary>
        /// <returns>VoiceManager</returns>
        /// <summary>
        /// This method used to listen all events occurring in T-Server through eventbroker service
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        ///
        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <summary>
        /// This method used to deliver incoming call user data and current call control status on UI
        /// </summary>
        /// <param name="listener">Soft phone listener which gives control to subscriber to receive user data and control status</param>
        /// <summary>
        /// This method used to get exact error message
        /// </summary>
        /// <param name="errorCode">Error Code</param>
        /// <returns>Error Message</returns>
        /// <summary>
        /// This method used to execute default process of Transfer call scenario.
        /// </summary>

        #endregion Other
    }
}