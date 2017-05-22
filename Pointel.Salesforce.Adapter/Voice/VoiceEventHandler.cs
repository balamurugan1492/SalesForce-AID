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
    /// Comment: Manages the Voice Media Events and SFDC Popup Last Modified: 20-12-2015 Created by:
    /// Pointel Inc
    /// </summary>
    internal class VoiceEventHandler
    {
        #region Field Declaration

        public static string ConsultText;
        public static EventEstablished eventEstablished = null;
        public static bool IsCallTransfered = false;
        public IDictionary<string, string> FinishedCallDuration = new Dictionary<string, string>();
        private string _callDuration = string.Empty;
        private IDictionary<string, DateTime> _callDurationData = new Dictionary<string, DateTime>();
        private string _consultConnId = string.Empty;
        private EventDialing _eventDialing = null;
        private EventReleased _eventReleased = null;
        private EventRinging _eventRinging = null;
        private bool _isConsultCallReceived = false;
        private Log _logger;

        #endregion Field Declaration

        #region SubscribeVoiceEvents

        public VoiceEventHandler()
        {
            try
            {
                this._logger = Log.GenInstance();
            }
            catch (Exception generalException)
            {
                this._logger.Error("SubscribeVoiceEvents : Error occurred while initializing Subscriber Event : " + generalException.ToString());
            }
        }

        #endregion SubscribeVoiceEvents

        #region ReceiveCalls

        public void ReceiveCalls(IMessage events, TimeSpan? callDuration = null)
        {
            try
            {
                if (Settings.SFDCOptions.SFDCPopupPages != null)
                {
                    if (events != null)
                    {
                        try
                        {
                            this._logger.Info("ReceiveCalls: Voice Event Received : " + events.ToString());
                        }
                        catch
                        {
                            this._logger.Error("Error occurred while printing Voice Event in Log");
                        }
                        switch (events.Id)
                        {
                            #region EventRinging

                            case EventRinging.MessageId:
                                _eventRinging = (EventRinging)events;
                                IsCallTransfered = false;
                                ConsultText = string.Empty;
                                if (_eventRinging.UserData != null && _eventRinging.UserData.ContainsKey(Settings.SFDCOptions.ConsultAttachDataKey))
                                {
                                    IsCallTransfered = true;
                                    ConsultText = _eventRinging.UserData.GetAsString(Settings.SFDCOptions.ConsultAttachDataKey);
                                    this._logger.Info("EventRinging : Consult Attach data key found for the ConnId :" + _eventRinging.ConnID.ToString() + " value : " + ConsultText);
                                }

                                if (_eventRinging.CallType == CallType.Inbound)
                                {
                                    if (!IsCallTransfered)
                                    {
                                        _isConsultCallReceived = false;
                                        CallRinging.GetInstance().PopupRecords(_eventRinging, SFDCCallType.InboundVoice);
                                    }
                                    else
                                    {
                                        _isConsultCallReceived = true;
                                        this._consultConnId = _eventRinging.ConnID.ToString();
                                        CallRinging.GetInstance().PopupRecords(_eventRinging, SFDCCallType.ConsultVoiceReceived);
                                    }
                                }
                                else if (_eventRinging.CallType == CallType.Consult)
                                {
                                    _isConsultCallReceived = true;
                                    CallRinging.GetInstance().PopupRecords(_eventRinging, SFDCCallType.ConsultVoiceReceived);
                                }
                                else if (_eventRinging.CallType == CallType.Outbound)
                                {
                                    if (IsCallTransfered)
                                    {
                                        _isConsultCallReceived = true;
                                        this._consultConnId = _eventRinging.ConnID.ToString();
                                        CallRinging.GetInstance().PopupRecords(_eventRinging, SFDCCallType.ConsultVoiceReceived);
                                    }
                                }
                                break;

                            #endregion EventRinging

                            #region EventPartyChanged

                            case EventPartyChanged.MessageId:
                                EventPartyChanged partyChanged = (EventPartyChanged)events;
                                _isConsultCallReceived = true;
                                if (_callDurationData.ContainsKey(partyChanged.PreviousConnID.ToString()) && !_callDurationData.ContainsKey(partyChanged.ConnID.ToString()))
                                {
                                    _callDurationData.Add(partyChanged.ConnID.ToString(), _callDurationData[partyChanged.PreviousConnID.ToString()]);
                                }
                                if (Settings.UpdateSFDCLog.ContainsKey(partyChanged.PreviousConnID.ToString()))
                                {
                                    if (!Settings.UpdateSFDCLog.ContainsKey(partyChanged.ConnID.ToString()))
                                        Settings.UpdateSFDCLog.Add(partyChanged.ConnID.ToString(), Settings.UpdateSFDCLog[partyChanged.PreviousConnID.ToString()]);
                                    else
                                        Settings.UpdateSFDCLog[partyChanged.ConnID.ToString()] = Settings.UpdateSFDCLog[partyChanged.PreviousConnID.ToString()];
                                }
                                this._consultConnId = partyChanged.ConnID.ToString();

                                if (partyChanged.UserData != null && partyChanged.UserData.ContainsKey(Settings.SFDCOptions.ConsultAttachDataKey))
                                {
                                    IsCallTransfered = true;
                                    ConsultText = partyChanged.UserData.GetAsString(Settings.SFDCOptions.ConsultAttachDataKey);
                                    this._logger.Info("PartyChanged : Consult Attach data key found for the ConnId :" + partyChanged.ConnID.ToString() + " value : " + ConsultText);
                                }

                                string duration = "0 Hr 0 Mins 0 Secs";
                                if (_callDurationData.ContainsKey(partyChanged.ConnID.ToString()))
                                {
                                    TimeSpan ts = System.DateTime.Now.Subtract(_callDurationData[partyChanged.ConnID.ToString()]);
                                    duration = ts.Hours + " Hr " + ts.Minutes + " Mins " + ts.Seconds + " Secs";
                                }
                                CallPartyChanged.GetInstance().PopupRecords(partyChanged, SFDCCallType.ConsultVoiceReceived, duration);
                                break;

                            #endregion EventPartyChanged

                            #region EventDialing

                            case EventDialing.MessageId:
                                _eventDialing = (EventDialing)events;
                                if (Settings.SFDCListener.Click2EmailData.ContainsKey("ClickToDial"))
                                {
                                    //Settings.ClickToDialData.Add(_eventDialing.ConnID.ToString(), _eventDialing.Reasons.GetAsString("ClickToDial"));
                                    Settings.ClickToDialData.Add(_eventDialing.ConnID.ToString(), Settings.SFDCListener.Click2EmailData["ClickToDial"]);
                                    Settings.SFDCListener.Click2EmailData.Clear();
                                }
                                if (_eventDialing.CallType == CallType.Unknown || _eventDialing.CallType == CallType.Outbound)
                                {
                                    CallDialing.GetInstance().PopupRecords(_eventDialing, SFDCCallType.OutboundVoiceSuccess);
                                }

                                break;

                            #endregion EventDialing

                            #region EventEstablished

                            case EventEstablished.MessageId:

                                eventEstablished = (EventEstablished)events;
                                if (!_callDurationData.ContainsKey(eventEstablished.ConnID.ToString()))
                                {
                                    _callDurationData.Add(eventEstablished.ConnID.ToString(), System.DateTime.Now);
                                }
                                if (eventEstablished.CallType == CallType.Inbound)
                                {
                                    if (_isConsultCallReceived)
                                    {
                                        CallConnected.GetInstance().PopupRecords(eventEstablished, SFDCCallType.ConsultVoiceReceived);
                                    }
                                    else
                                    {
                                        CallConnected.GetInstance().PopupRecords(eventEstablished, SFDCCallType.InboundVoice);
                                    }
                                }
                                else if (eventEstablished.CallType == CallType.Unknown || eventEstablished.CallType == CallType.Outbound)
                                {
                                    if (_isConsultCallReceived)
                                    {
                                        CallConnected.GetInstance().PopupRecords(eventEstablished, SFDCCallType.ConsultVoiceReceived);
                                    }
                                    else
                                    {
                                        CallConnected.GetInstance().PopupRecords(eventEstablished, SFDCCallType.OutboundVoiceSuccess);
                                    }
                                }
                                else if (eventEstablished.CallType == CallType.Consult)
                                {
                                    if (_isConsultCallReceived)
                                        CallConnected.GetInstance().PopupRecords(eventEstablished, SFDCCallType.ConsultVoiceReceived);
                                }
                                break;

                            #endregion EventEstablished

                            #region EventReleased

                            case EventReleased.MessageId:

                                _eventReleased = (EventReleased)events;
                                this._logger.Info("Event Release Call State: " + _eventReleased.CallState.ToString());
                                if (_eventReleased.CallState != 22)
                                {
                                    if (Settings.SFDCOptions.CanUseGenesysCallDuration && callDuration != null)
                                    {
                                        _callDuration = callDuration.Value.Hours + " Hr " + callDuration.Value.Minutes + " Mins " + callDuration.Value.Seconds + " Secs"; ;
                                        this._logger.Info("ConnectionId : " + _eventReleased.ConnID.ToString() + " Call duration is " + _callDuration);
                                    }
                                    else if (_callDurationData.ContainsKey(_eventReleased.ConnID.ToString()))
                                    {
                                        TimeSpan ts = System.DateTime.Now.Subtract(_callDurationData[_eventReleased.ConnID.ToString()]);
                                        _callDuration = ts.Hours + " Hr " + ts.Minutes + " Mins " + ts.Seconds + " Secs";
                                        this._logger.Info("ConnectionId : " + _eventReleased.ConnID.ToString() + " Call duration is " + _callDuration);

                                        if (!FinishedCallDuration.ContainsKey(_eventReleased.ConnID.ToString()))
                                        {
                                            FinishedCallDuration.Add(_eventReleased.ConnID.ToString(), _callDuration);
                                        }
                                        else
                                        {
                                            FinishedCallDuration[_eventReleased.ConnID.ToString()] = _callDuration;
                                        }
                                    }
                                    else
                                    {
                                        _callDuration = "0 Hr 0 Mins 0 Secs";
                                        this._logger.Info("Call duration not found for the connection Id : " + _eventReleased.ConnID.ToString());
                                    }
                                    HandleReleaseEvent(_eventReleased, _callDuration);
                                }
                                else
                                {
                                    this._logger.Info("The call has been rejected for the connectionId : " + _eventReleased.ConnID.ToString());
                                    HandleCallRejected(_eventReleased, "0 Hr 0 Mins 0 Secs");
                                }

                                break;

                            #endregion EventReleased

                            #region EventError

                            case EventError.MessageId:
                                EventError eventError = (EventError)events;
                                this._logger.Info("EventError occurred for the connectionId : " + eventError.ConnID.ToString());
                                if (eventError.CallType == CallType.Outbound)
                                {
                                    CallEventError.GetInstance().PopupRecords(eventError, SFDCCallType.OutboundVoiceFailure);
                                }

                                break;

                            #endregion EventError

                            #region EventAbandoned

                            case EventAbandoned.MessageId:
                                EventAbandoned eventAbandoned = (EventAbandoned)events;
                                this._logger.Info("EventAbandoned occurred for the connectionId : " + eventAbandoned.ConnID.ToString());
                                if (eventAbandoned.CallType == CallType.Outbound)
                                {
                                    CallEventAbandoned.GetInstance().PopupRecords(eventAbandoned, SFDCCallType.OutboundVoiceFailure);
                                }

                                break;

                            #endregion EventAbandoned

                            #region EventDestinationBusy

                            case EventDestinationBusy.MessageId:
                                EventDestinationBusy destinationBusyEvent = (EventDestinationBusy)events;
                                this._logger.Info("EventDestinationBusy occurred for the connectionId : " + destinationBusyEvent.ConnID.ToString());
                                if (destinationBusyEvent.CallType == CallType.Outbound)
                                {
                                    CallEventDestinationBusy.GetInstance().PopupRecords(destinationBusyEvent, SFDCCallType.OutboundVoiceFailure);
                                }

                                break;

                            #endregion EventDestinationBusy

                            default:
                                _logger.Info("ReceiveCalls: Unhandled Event " + events.Name);
                                break;
                        }
                    }
                }
            }
            catch (Exception generalException)
            {
                _logger.Error("ReceiveCalls: Error occurred while receiving voice events " + generalException.ToString());
            }
        }

        #endregion ReceiveCalls

        #region UpdateOnDispositionCodeChange

        public void UpdateOnDispositionCodeChange(IMessage message, string callDuration)
        {
            try
            {
                this._logger.Info("UpdateOnDispositionCodeChange: Event Name " + message.ToString());
                switch (message.Id)
                {
                    case EventAttachedDataChanged.MessageId:
                        EventAttachedDataChanged attachedDataChanged = (EventAttachedDataChanged)message;

                        if (Settings.SFDCOptions.CanUseGenesysCallDuration && callDuration != null)
                        {
                            _callDuration = callDuration;
                        }
                        else if (_callDurationData.ContainsKey(attachedDataChanged.ConnID.ToString()))
                        {
                            TimeSpan ts = System.DateTime.Now.Subtract(_callDurationData[attachedDataChanged.ConnID.ToString()]);
                            _callDuration = ts.Hours + " Hr " + ts.Minutes + " Mins " + ts.Seconds + " Secs";
                        }
                        else
                        {
                            this._logger.Info("UpdateOnDispositionCodeChange: Call Duration data is not found in Collection");
                            _callDuration = "0 Hr 0 Mins 0 Secs";
                        }
                        if (attachedDataChanged.CallType == CallType.Inbound)
                        {
                            if (this._consultConnId == attachedDataChanged.ConnID.ToString())
                                CallAttachedDataChanged.GetInstance().UpdateRecords(attachedDataChanged, SFDCCallType.ConsultVoiceReceived, _callDuration);
                            else
                                CallAttachedDataChanged.GetInstance().UpdateRecords(attachedDataChanged, SFDCCallType.InboundVoice, _callDuration);
                        }
                        else if ((attachedDataChanged.CallType == CallType.Outbound) || (attachedDataChanged.CallType == CallType.Unknown))
                        {
                            if (this._consultConnId == attachedDataChanged.ConnID.ToString())
                                CallAttachedDataChanged.GetInstance().UpdateRecords(attachedDataChanged, SFDCCallType.ConsultVoiceReceived, _callDuration);
                            else
                                CallAttachedDataChanged.GetInstance().UpdateRecords(attachedDataChanged, SFDCCallType.OutboundVoiceSuccess, _callDuration);
                        }
                        else if (attachedDataChanged.CallType == CallType.Consult)
                        {
                            CallAttachedDataChanged.GetInstance().UpdateRecords(attachedDataChanged, SFDCCallType.ConsultVoiceReceived, _callDuration);
                        }

                        break;

                    #region EventReleased

                    case EventReleased.MessageId:
                        EventReleased released = (EventReleased)message;

                        if (Settings.SFDCOptions.CanUseGenesysCallDuration && callDuration != null)
                        {
                            _callDuration = callDuration;
                        }
                        else if (FinishedCallDuration.ContainsKey(released.ConnID.ToString()))
                        {
                            _callDuration = FinishedCallDuration[released.ConnID.ToString()];
                        }
                        else if (_callDurationData.ContainsKey(released.ConnID.ToString()))
                        {
                            TimeSpan ts = System.DateTime.Now.Subtract(_callDurationData[released.ConnID.ToString()]);
                            _callDuration = ts.Hours + " Hr " + ts.Minutes + " Mins " + ts.Seconds + " Secs";
                            this._logger.Error("UpdateOnDispositionCodeChange: call duration not found in FinishedCallDuration collection for the connId :" + released.ConnID.ToString());

                            if (!FinishedCallDuration.ContainsKey(released.ConnID.ToString()))
                            {
                                FinishedCallDuration.Add(released.ConnID.ToString(), _callDuration);
                            }
                        }
                        else
                        {
                            this._logger.Error("UpdateOnDispositionCodeChange: call duration not found in both call data collection for the connId :" + released.ConnID.ToString());
                            _callDuration = "0 Hr 0 Mins 0 Secs";
                        }

                        if (released.CallType == CallType.Inbound)
                        {
                            if (this._consultConnId == released.ConnID.ToString())
                                CallReleased.GetInstance().UpdateRecords(released, SFDCCallType.ConsultVoiceReceived, _callDuration);
                            else
                                CallReleased.GetInstance().UpdateRecords(released, SFDCCallType.InboundVoice, _callDuration);
                        }
                        else if ((released.CallType == CallType.Outbound) || (released.CallType == CallType.Unknown))
                        {
                            if (this._consultConnId == released.ConnID.ToString())
                                CallReleased.GetInstance().UpdateRecords(released, SFDCCallType.ConsultVoiceReceived, _callDuration);
                            else
                                CallReleased.GetInstance().UpdateRecords(released, SFDCCallType.OutboundVoiceSuccess, _callDuration);
                        }
                        else if (released.CallType == CallType.Consult)
                        {
                            CallReleased.GetInstance().UpdateRecords(released, SFDCCallType.ConsultVoiceReceived, _callDuration);
                        }

                        break;

                    #endregion EventReleased

                    default:
                        break;
                }
            }
            catch (Exception generalException)
            {
                this._logger.Error("UpdateOnDispositionCodeChange: Error Occurred : " + generalException.ToString());
            }
        }

        #endregion UpdateOnDispositionCodeChange

        #region HandleReleaseEvent

        private void HandleReleaseEvent(EventReleased released, string duration)
        {
            try
            {
                if (_eventReleased.CallType == CallType.Inbound)
                {
                    if (!_isConsultCallReceived)
                    {
                        CallReleased.GetInstance().PopupRecords(_eventReleased, SFDCCallType.InboundVoice, duration);
                    }
                    else
                    {
                        _isConsultCallReceived = false;
                        CallReleased.GetInstance().PopupRecords(_eventReleased, SFDCCallType.ConsultVoiceReceived, duration);
                    }
                }
                else if (_eventReleased.CallType == CallType.Unknown || _eventReleased.CallType == CallType.Outbound)
                {
                    if (_isConsultCallReceived)
                    {
                        CallReleased.GetInstance().PopupRecords(_eventReleased, SFDCCallType.ConsultVoiceReceived, duration);
                        _isConsultCallReceived = false;
                    }
                    else
                    {
                        CallReleased.GetInstance().PopupRecords(_eventReleased, SFDCCallType.OutboundVoiceSuccess, duration);
                    }
                }
                else if (_eventReleased.CallType == CallType.Consult)
                {
                    if (_isConsultCallReceived)
                    {
                        CallReleased.GetInstance().PopupRecords(_eventReleased, SFDCCallType.ConsultVoiceReceived, duration);
                        _isConsultCallReceived = false;
                    }
                    //else
                    //    CallReleased.GetInstance().PopupRecords(eventReleased, SFDCCallType.ConsultSuccess, duration);
                }
            }
            catch (Exception generalException)
            {
                this._logger.Error("HandleReleaseEvent: Error Occurred while on handling call release event:" + generalException.ToString());
            }
        }

        #endregion HandleReleaseEvent

        #region HandleCallRejected

        private void HandleCallRejected(EventReleased released, string duration)
        {
            try
            {
                if (_eventReleased.CallType == CallType.Inbound)
                {
                    if (!_isConsultCallReceived)
                    {
                        CallRejected.GetInstance().PopupRecords(_eventReleased, SFDCCallType.InboundVoice, duration);
                    }
                    else
                    {
                        _isConsultCallReceived = false;
                        CallRejected.GetInstance().PopupRecords(_eventReleased, SFDCCallType.ConsultVoiceReceived, duration);
                    }
                }
                else if (_eventReleased.CallType == CallType.Unknown || _eventReleased.CallType == CallType.Outbound)
                {
                    if (_isConsultCallReceived)
                    {
                        CallRejected.GetInstance().PopupRecords(_eventReleased, SFDCCallType.ConsultVoiceReceived, duration);
                        _isConsultCallReceived = false;
                    }
                    else
                    {
                        CallRejected.GetInstance().PopupRecords(_eventReleased, SFDCCallType.OutboundVoiceSuccess, duration);
                    }
                }
                else if (_eventReleased.CallType == CallType.Consult)
                {
                    if (_isConsultCallReceived)
                    {
                        CallRejected.GetInstance().PopupRecords(_eventReleased, SFDCCallType.ConsultVoiceReceived, duration);
                        _isConsultCallReceived = false;
                    }
                    //else
                    //    CallRejected.GetInstance().PopupRecords(eventReleased, SFDCCallType.ConsultSuccess, duration);
                }
            }
            catch (Exception generalException)
            {
                this._logger.Error("HandleReleaseEvent: Error Occurred while on handling call release event :" + generalException.ToString());
            }
        }

        #endregion HandleCallRejected

        #region Handle Markdone event

        internal void UpdateDispositionCodeChange(IXNCustomData data)
        {
            try
            {
                this._logger.Info("UpdateDispositionCodeChange()");
                if (!Settings.SFDCOptions.CanUseGenesysCallDuration || string.IsNullOrWhiteSpace(data.Duration))
                {
                    if (_callDurationData.ContainsKey(data.InteractionId))
                    {
                        TimeSpan ts = System.DateTime.Now.Subtract(_callDurationData[data.InteractionId]);
                        data.Duration = ts.Hours + " Hr " + ts.Minutes + " Mins " + ts.Seconds + " Secs";
                    }
                }
                if (!string.IsNullOrWhiteSpace(data.EventName) && data.EventName == "InteractionMarkDone")
                {
                    CallMarkDone.GetInstance().UpdateRecords(data);
                }
                else if (data.InteractionEvent != null)
                {
                    UpdateOnDispositionCodeChange(data.InteractionEvent, data.Duration);
                }
            }
            catch (Exception generalException)
            {
                this._logger.Error("UpdateDispositionCodeChange: Error occurred, details: " + generalException);
            }
        }

        #endregion Handle Markdone event
    }
}