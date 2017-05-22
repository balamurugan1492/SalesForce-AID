/*
  Copyright (c) Pointel Inc., All Rights Reserved.

 This software is the confidential and proprietary information of
 Pointel Inc., ("Confidential Information"). You shall not
 disclose such Confidential Information and shall use it only in
 accordance with the terms of the license agreement you entered into
 with Pointel.

 POINTEL MAKES NO REPRESENTATIONS OR WARRANTIES ABOUT THE
  *SUITABILITY OF THE SOFTWARE, EITHER EXPRESS OR IMPLIED, INCLUDING
  *BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY,
  *FITNESS FOR A PARTICULAR PURPOSE, OR NON-INFRINGEMENT. POINTEL
  *SHALL NOT BE LIABLE FOR ANY DAMAGES SUFFERED BY LICENSEE AS A
  *RESULT OF USING, MODIFYING OR DISTRIBUTING THIS SOFTWARE OR ITS
  *DERIVATIVES.
 */

using Genesyslab.Platform.Commons.Protocols;
using Genesyslab.Platform.Voice.Protocols.TServer;
using Genesyslab.Platform.Voice.Protocols.TServer.Events;
using Pointel.Salesforce.Adapter.LogMessage;
using Pointel.Salesforce.Adapter.Utility;
using System;
using System.Collections.Generic;

//using Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events;

namespace Pointel.Salesforce.Adapter.Voice
{
    /// <summary>
    /// Comment: Manages the Voice Media Events and SFDC Popup
    /// Last Modified: 20-12-2015
    /// Created by: Pointel Inc
    /// </summary>
    internal class SubscribeVoiceEvents
    {
        #region Field Declaration

        private Log logger;
        private EventRinging eventRinging = null;
        private EventReleased eventReleased = null;
        private EventDialing eventDialing = null;
        public static EventEstablished eventEstablished = null;
        private IDictionary<string, DateTime> CallDurationData = new Dictionary<string, DateTime>();
        public IDictionary<string, string> FinishedCallDuration = new Dictionary<string, string>();
        private bool IsConsultCallReceived = false;
        private string consultConnId = string.Empty;
        private string callDuration = string.Empty;
        public static string ConsultText;
        public static bool IsCallTransfered = false;

        #endregion Field Declaration

        #region SubscribeVoiceEvents

        public SubscribeVoiceEvents()
        {
            try
            {
                this.logger = Log.GenInstance();
            }
            catch (Exception generalException)
            {
                this.logger.Error("SubscribeVoiceEvents : Error occurred while initializing Subscriber Event : " + generalException.ToString());
            }
        }

        #endregion SubscribeVoiceEvents

        #region ReceiveCalls

        public void ReceiveCalls(IMessage events)
        {
            try
            {
                if (Settings.SFDCOptions.SFDCPopupPages != null)
                {
                    if (events != null)
                    {
                        try
                        {
                            this.logger.Info("ReceiveCalls: Voice Event Received : " + events.ToString());
                        }
                        catch
                        {
                            this.logger.Error("Error occurred while printing Voice Event in Log");
                        }
                        switch (events.Id)
                        {
                            #region EventRinging

                            case EventRinging.MessageId:
                                eventRinging = (EventRinging)events;
                                IsCallTransfered = false;
                                ConsultText = string.Empty;
                                if (eventRinging.UserData != null && eventRinging.UserData.ContainsKey(Settings.SFDCOptions.ConsultAttachDataKey))
                                {
                                    IsCallTransfered = true;
                                    ConsultText = eventRinging.UserData.GetAsString(Settings.SFDCOptions.ConsultAttachDataKey);
                                    this.logger.Info("EventRinging : Consult Attach data key found for the ConnId :" + eventRinging.ConnID.ToString() + " value : " + ConsultText);
                                }

                                if (eventRinging.CallType == CallType.Inbound)
                                {
                                    if (!IsCallTransfered)
                                    {
                                        IsConsultCallReceived = false;
                                        CallRinging.GetInstance().PopupRecords(eventRinging, SFDCCallType.Inbound);
                                    }
                                    else
                                    {
                                        IsConsultCallReceived = true;
                                        this.consultConnId = eventRinging.ConnID.ToString();
                                        CallRinging.GetInstance().PopupRecords(eventRinging, SFDCCallType.ConsultReceived);
                                    }
                                }
                                else if (eventRinging.CallType == CallType.Consult)
                                {
                                    IsConsultCallReceived = true;
                                    CallRinging.GetInstance().PopupRecords(eventRinging, SFDCCallType.ConsultReceived);
                                }
                                else if (eventRinging.CallType == CallType.Outbound)
                                {
                                    if (IsCallTransfered)
                                    {
                                        IsConsultCallReceived = true;
                                        this.consultConnId = eventRinging.ConnID.ToString();
                                        CallRinging.GetInstance().PopupRecords(eventRinging, SFDCCallType.ConsultReceived);
                                    }
                                }
                                break;

                            #endregion EventRinging

                            #region EventPartyChanged

                            case EventPartyChanged.MessageId:
                                EventPartyChanged partyChanged = (EventPartyChanged)events;
                                IsConsultCallReceived = true;
                                if (CallDurationData.ContainsKey(partyChanged.PreviousConnID.ToString()) && !CallDurationData.ContainsKey(partyChanged.ConnID.ToString()))
                                {
                                    CallDurationData.Add(partyChanged.ConnID.ToString(), CallDurationData[partyChanged.PreviousConnID.ToString()]);
                                }
                                if (Settings.UpdateSFDCLog.ContainsKey(partyChanged.PreviousConnID.ToString()))
                                {
                                    if (!Settings.UpdateSFDCLog.ContainsKey(partyChanged.ConnID.ToString()))
                                        Settings.UpdateSFDCLog.Add(partyChanged.ConnID.ToString(), Settings.UpdateSFDCLog[partyChanged.PreviousConnID.ToString()]);
                                    else
                                        Settings.UpdateSFDCLog[partyChanged.ConnID.ToString()] = Settings.UpdateSFDCLog[partyChanged.PreviousConnID.ToString()];
                                }
                                this.consultConnId = partyChanged.ConnID.ToString();

                                if (partyChanged.UserData != null && partyChanged.UserData.ContainsKey(Settings.SFDCOptions.ConsultAttachDataKey))
                                {
                                    IsCallTransfered = true;
                                    ConsultText = partyChanged.UserData.GetAsString(Settings.SFDCOptions.ConsultAttachDataKey);
                                    this.logger.Info("PartyChanged : Consult Attach data key found for the ConnId :" + partyChanged.ConnID.ToString() + " value : " + ConsultText);
                                }

                                string duration = "0 Hr 0 Mins 0 Secs";
                                if (CallDurationData.ContainsKey(partyChanged.ConnID.ToString()))
                                {
                                    TimeSpan ts = System.DateTime.Now.Subtract(CallDurationData[partyChanged.ConnID.ToString()]);
                                    duration = ts.Hours + " Hr " + ts.Minutes + " Mins " + ts.Seconds + " Secs";
                                }
                                CallPartyChanged.GetInstance().PopupRecords(partyChanged, SFDCCallType.ConsultReceived, duration);
                                break;

                            #endregion EventPartyChanged

                            #region EventDialing

                            case EventDialing.MessageId:
                                eventDialing = (EventDialing)events;

                                if (eventDialing.Reasons != null && eventDialing.Reasons.ContainsKey("ClickToDial"))
                                {
                                    Settings.ClickToDialData.Add(eventDialing.ConnID.ToString(), eventDialing.Reasons.GetAsString("ClickToDial"));
                                }
                                if (eventDialing.CallType == CallType.Unknown || eventDialing.CallType == CallType.Outbound)
                                {
                                    CallDialing.GetInstance().PopupRecords(eventDialing, SFDCCallType.OutboundSuccess);
                                }

                                break;

                            #endregion EventDialing

                            #region EventEstablished

                            case EventEstablished.MessageId:

                                eventEstablished = (EventEstablished)events;
                                if (!CallDurationData.ContainsKey(eventEstablished.ConnID.ToString()))
                                {
                                    CallDurationData.Add(eventEstablished.ConnID.ToString(), System.DateTime.Now);
                                }
                                if (eventEstablished.CallType == CallType.Inbound)
                                {
                                    if (IsConsultCallReceived)
                                    {
                                        CallConnected.GetInstance().PopupRecords(eventEstablished, SFDCCallType.ConsultReceived);
                                    }
                                    else
                                    {
                                        CallConnected.GetInstance().PopupRecords(eventEstablished, SFDCCallType.Inbound);
                                    }
                                }
                                else if (eventEstablished.CallType == CallType.Unknown || eventEstablished.CallType == CallType.Outbound)
                                {
                                    if (IsConsultCallReceived)
                                    {
                                        CallConnected.GetInstance().PopupRecords(eventEstablished, SFDCCallType.ConsultReceived);
                                    }
                                    else
                                    {
                                        CallConnected.GetInstance().PopupRecords(eventEstablished, SFDCCallType.OutboundSuccess);
                                    }
                                }
                                else if (eventEstablished.CallType == CallType.Consult)
                                {
                                    if (IsConsultCallReceived)
                                        CallConnected.GetInstance().PopupRecords(eventEstablished, SFDCCallType.ConsultReceived);
                                }
                                break;

                            #endregion EventEstablished

                            #region EventReleased

                            case EventReleased.MessageId:

                                eventReleased = (EventReleased)events;
                                this.logger.Info("Event Release Call State: " + eventReleased.CallState.ToString());
                                if (eventReleased.CallState != 22)
                                {
                                    if (CallDurationData.ContainsKey(eventReleased.ConnID.ToString()))
                                    {
                                        TimeSpan ts = System.DateTime.Now.Subtract(CallDurationData[eventReleased.ConnID.ToString()]);
                                        callDuration = ts.Hours + " Hr " + ts.Minutes + " Mins " + ts.Seconds + " Secs";
                                        this.logger.Info("ConnectionId : " + eventReleased.ConnID.ToString() + " Call duration is " + callDuration);
                                        HandleReleaseEvent(eventReleased, callDuration);
                                        if (!FinishedCallDuration.ContainsKey(eventReleased.ConnID.ToString()))
                                        {
                                            FinishedCallDuration.Add(eventReleased.ConnID.ToString(), callDuration);
                                        }
                                        else
                                        {
                                            FinishedCallDuration[eventReleased.ConnID.ToString()] = callDuration;
                                        }
                                    }
                                    else
                                    {
                                        this.logger.Info("Call duration not found for the connection Id : " + eventReleased.ConnID.ToString());
                                        HandleReleaseEvent(eventReleased, "0 Hr 0 Mins 0 Secs");
                                    }
                                }
                                else
                                {
                                    this.logger.Info("The call has been rejected for the connectionId : " + eventReleased.ConnID.ToString());
                                    HandleCallRejected(eventReleased, "0 Hr 0 Mins 0 Secs");
                                }

                                break;

                            #endregion EventReleased

                            #region EventError

                            case EventError.MessageId:
                                EventError eventError = (EventError)events;
                                this.logger.Info("EventError occurred for the connectionId : " + eventError.ConnID.ToString());
                                if (eventError.CallType == CallType.Outbound)
                                {
                                    CallEventError.GetInstance().PopupRecords(eventError, SFDCCallType.OutboundFailure);
                                }
                                else if (eventError.CallType == CallType.Consult)
                                {
                                    CallEventError.GetInstance().PopupRecords(eventError, SFDCCallType.ConsultFailure);
                                }
                                break;

                            #endregion EventError

                            #region EventAbandoned

                            case EventAbandoned.MessageId:
                                EventAbandoned eventAbandoned = (EventAbandoned)events;
                                this.logger.Info("EventAbandoned occurred for the connectionId : " + eventAbandoned.ConnID.ToString());
                                if (eventAbandoned.CallType == CallType.Outbound)
                                {
                                    CallEventAbandoned.GetInstance().PopupRecords(eventAbandoned, SFDCCallType.OutboundFailure);
                                }
                                else if (eventAbandoned.CallType == CallType.Consult)
                                {
                                    CallEventAbandoned.GetInstance().PopupRecords(eventAbandoned, SFDCCallType.ConsultFailure);
                                }
                                break;

                            #endregion EventAbandoned

                            #region EventDestinationBusy

                            case EventDestinationBusy.MessageId:
                                EventDestinationBusy destinationBusyEvent = (EventDestinationBusy)events;
                                this.logger.Info("EventDestinationBusy occurred for the connectionId : " + destinationBusyEvent.ConnID.ToString());
                                if (destinationBusyEvent.CallType == CallType.Outbound)
                                {
                                    CallEventDestinationBusy.GetInstance().PopupRecords(destinationBusyEvent, SFDCCallType.OutboundFailure);
                                }
                                else if (destinationBusyEvent.CallType == CallType.Consult)
                                {
                                    CallEventDestinationBusy.GetInstance().PopupRecords(destinationBusyEvent, SFDCCallType.ConsultFailure);
                                }
                                break;

                            #endregion EventDestinationBusy

                            default:
                                logger.Info("ReceiveCalls: Unhandled Event " + events.Name);
                                break;
                        }
                    }
                }
            }
            catch (Exception generalException)
            {
                logger.Error("ReceiveCalls: Error occurred while receiving voice events " + generalException.ToString());
            }
        }

        #endregion ReceiveCalls

        #region UpdateOnDispositionCodeChange

        public void UpdateOnDispositionCodeChange(IMessage message)
        {
            try
            {
                this.logger.Info("UpdateOnDispositionCodeChange: Event Name " + message.ToString());
                switch (message.Id)
                {
                    case EventAttachedDataChanged.MessageId:
                        EventAttachedDataChanged attachedDataChanged = (EventAttachedDataChanged)message;

                        if (CallDurationData.ContainsKey(attachedDataChanged.ConnID.ToString()))
                        {
                            TimeSpan ts = System.DateTime.Now.Subtract(CallDurationData[attachedDataChanged.ConnID.ToString()]);
                            callDuration = ts.Hours + " Hr " + ts.Minutes + " Mins " + ts.Seconds + " Secs";
                        }
                        else
                        {
                            this.logger.Info("UpdateOnDispositionCodeChange: Call Duration data is not found in Collection");
                            callDuration = "0 Hr 0 Mins 0 Secs";
                        }
                        if (attachedDataChanged.CallType == CallType.Inbound)
                        {
                            if (this.consultConnId == attachedDataChanged.ConnID.ToString())
                                CallAttachedDataChanged.GetInstance().UpdateRecords(attachedDataChanged, SFDCCallType.ConsultReceived, callDuration);
                            else
                                CallAttachedDataChanged.GetInstance().UpdateRecords(attachedDataChanged, SFDCCallType.Inbound, callDuration);
                        }
                        else if ((attachedDataChanged.CallType == CallType.Outbound) || (attachedDataChanged.CallType == CallType.Unknown))
                        {
                            if (this.consultConnId == attachedDataChanged.ConnID.ToString())
                                CallAttachedDataChanged.GetInstance().UpdateRecords(attachedDataChanged, SFDCCallType.ConsultReceived, callDuration);
                            else
                                CallAttachedDataChanged.GetInstance().UpdateRecords(attachedDataChanged, SFDCCallType.OutboundSuccess, callDuration);
                        }
                        else if (attachedDataChanged.CallType == CallType.Consult)
                        {
                            CallAttachedDataChanged.GetInstance().UpdateRecords(attachedDataChanged, SFDCCallType.ConsultReceived, callDuration);
                        }

                        break;

                    #region EventReleased

                    case EventReleased.MessageId:
                        EventReleased released = (EventReleased)message;

                        if (FinishedCallDuration.ContainsKey(released.ConnID.ToString()))
                        {
                            callDuration = FinishedCallDuration[released.ConnID.ToString()];
                        }
                        else if (CallDurationData.ContainsKey(released.ConnID.ToString()))
                        {
                            TimeSpan ts = System.DateTime.Now.Subtract(CallDurationData[released.ConnID.ToString()]);
                            callDuration = ts.Hours + " Hr " + ts.Minutes + " Mins " + ts.Seconds + " Secs";
                            this.logger.Error("UpdateOnDispositionCodeChange: call duration not found in FinishedCallDuration collection for the connId :" + released.ConnID.ToString());

                            if (!FinishedCallDuration.ContainsKey(released.ConnID.ToString()))
                            {
                                FinishedCallDuration.Add(released.ConnID.ToString(), callDuration);
                            }
                        }
                        else
                        {
                            this.logger.Error("UpdateOnDispositionCodeChange: call duration not found in both call data collection for the connId :" + released.ConnID.ToString());
                            callDuration = "0 Hr 0 Mins 0 Secs";
                        }

                        if (released.CallType == CallType.Inbound)
                        {
                            if (this.consultConnId == released.ConnID.ToString())
                                CallReleased.GetInstance().UpdateRecords(released, SFDCCallType.ConsultReceived, callDuration);
                            else
                                CallReleased.GetInstance().UpdateRecords(released, SFDCCallType.Inbound, callDuration);
                        }
                        else if ((released.CallType == CallType.Outbound) || (released.CallType == CallType.Unknown))
                        {
                            if (this.consultConnId == released.ConnID.ToString())
                                CallReleased.GetInstance().UpdateRecords(released, SFDCCallType.ConsultReceived, callDuration);
                            else
                                CallReleased.GetInstance().UpdateRecords(released, SFDCCallType.OutboundSuccess, callDuration);
                        }
                        else if (released.CallType == CallType.Consult)
                        {
                            CallReleased.GetInstance().UpdateRecords(released, SFDCCallType.ConsultReceived, callDuration);
                        }

                        break;

                    #endregion EventReleased

                    default:
                        break;
                }
            }
            catch (Exception generalException)
            {
                this.logger.Error("UpdateOnDispositionCodeChange: Error Occurred : " + generalException.ToString());
            }
        }

        #endregion UpdateOnDispositionCodeChange

        #region HandleReleaseEvent

        private void HandleReleaseEvent(EventReleased released, string duration)
        {
            try
            {
                if (eventReleased.CallType == CallType.Inbound)
                {
                    if (!IsConsultCallReceived)
                    {
                        CallReleased.GetInstance().PopupRecords(eventReleased, SFDCCallType.Inbound, duration);
                    }
                    else
                    {
                        IsConsultCallReceived = false;
                        CallReleased.GetInstance().PopupRecords(eventReleased, SFDCCallType.ConsultReceived, duration);
                    }
                }
                else if (eventReleased.CallType == CallType.Unknown || eventReleased.CallType == CallType.Outbound)
                {
                    if (IsConsultCallReceived)
                    {
                        CallReleased.GetInstance().PopupRecords(eventReleased, SFDCCallType.ConsultReceived, duration);
                        IsConsultCallReceived = false;
                    }
                    else
                    {
                        CallReleased.GetInstance().PopupRecords(eventReleased, SFDCCallType.OutboundSuccess, duration);
                    }
                }
                else if (eventReleased.CallType == CallType.Consult)
                {
                    if (IsConsultCallReceived)
                    {
                        CallReleased.GetInstance().PopupRecords(eventReleased, SFDCCallType.ConsultReceived, duration);
                        IsConsultCallReceived = false;
                    }
                    //else
                    //    CallReleased.GetInstance().PopupRecords(eventReleased, SFDCCallType.ConsultSuccess, duration);
                }
            }
            catch (Exception generalException)
            {
                this.logger.Error("HandleReleaseEvent: Error Occurred while on handling call release event:" + generalException.ToString());
            }
        }

        #endregion HandleReleaseEvent

        #region HandleCallRejected

        private void HandleCallRejected(EventReleased released, string duration)
        {
            try
            {
                if (eventReleased.CallType == CallType.Inbound)
                {
                    if (!IsConsultCallReceived)
                    {
                        CallRejected.GetInstance().PopupRecords(eventReleased, SFDCCallType.Inbound, duration);
                    }
                    else
                    {
                        IsConsultCallReceived = false;
                        CallRejected.GetInstance().PopupRecords(eventReleased, SFDCCallType.ConsultReceived, duration);
                    }
                }
                else if (eventReleased.CallType == CallType.Unknown || eventReleased.CallType == CallType.Outbound)
                {
                    if (IsConsultCallReceived)
                    {
                        CallRejected.GetInstance().PopupRecords(eventReleased, SFDCCallType.ConsultReceived, duration);
                        IsConsultCallReceived = false;
                    }
                    else
                    {
                        CallRejected.GetInstance().PopupRecords(eventReleased, SFDCCallType.OutboundSuccess, duration);
                    }
                }
                else if (eventReleased.CallType == CallType.Consult)
                {
                    if (IsConsultCallReceived)
                    {
                        CallRejected.GetInstance().PopupRecords(eventReleased, SFDCCallType.ConsultReceived, duration);
                        IsConsultCallReceived = false;
                    }
                    //else
                    //    CallRejected.GetInstance().PopupRecords(eventReleased, SFDCCallType.ConsultSuccess, duration);
                }
            }
            catch (Exception generalException)
            {
                this.logger.Error("HandleReleaseEvent: Error Occurred while on handling call release event :" + generalException.ToString());
            }
        }

        #endregion HandleCallRejected
    }
}