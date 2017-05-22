using System;
using Genesyslab.Platform.Commons.Protocols;
using Genesyslab.Platform.Voice.Protocols.TServer.Events;
using System.Collections.Generic;
using Genesyslab.Platform.Commons.Collections;
using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.CfgObjects;
using Pointel.Salesforce.Adapter.LogMessage;
using Pointel.Salesforce.Adapter.Voice;
using Pointel.Salesforce.Adapter.SFDCUtils;
using Genesyslab.Platform.Voice.Protocols.TServer;
using Pointel.Salesforce.Adapter.Utility;
//using Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events;

namespace Pointel.Salesforce.Adapter.Voice
{
    /// <summary>
    /// Comment: Manages the Voice,Chat,Email Media Events and SFDC Popup
    /// Last Modified: 25-08-2015
    /// Created by: Pointel Inc
    /// </summary>
    internal class SubscribeAgentVoiceEvents
    {
        #region Field Declaration
        private Log logger;
        private EventRinging eventRinging = null;
        private EventReleased eventReleased = null;
        private EventDialing eventDialing = null;
        public static EventEstablished eventEstablished = null;
        private IDictionary<string, DateTime> CallDurationData = new Dictionary<string, DateTime>();
        public IDictionary<string, string> FinishedCallDuration = new Dictionary<string, string>();
        bool IsConsultCallReceived = false;
        string consultConnId = string.Empty;
        string callDuration = string.Empty;
        #endregion

        #region SubscribeAgentVoiceEvents
        public SubscribeAgentVoiceEvents()
        {
            try
            {
                this.logger = Log.GenInstance();
            }
            catch (Exception generalException)
            {
                this.logger.Error("SubscribeAgentVoiceEvents : Error occured while initializing Subscriber Event : " + generalException.ToString());
            }
        }
        #endregion

        #region ReceiveCalls
        public void ReceiveCalls(IMessage events)
        {
            try
            {
                if (Settings.SFDCOptions.SFDCPopupPages != null)
                {
                    if (events != null)
                    {
                        logger.Info("ReceiveCalls : Agent Voice Event " + events.ToString());
                        switch (events.Id)
                        {
                            #region EventRinging
                            case EventRinging.MessageId:
                                eventRinging = (EventRinging)events;

                                if (eventRinging.CallType == CallType.Inbound)
                                {
                                    IsConsultCallReceived=false;                                                                      
                                    CallRinging.GetInstance().PopupRecords(eventRinging, SFDCCallType.Inbound);
                                    if(!Settings.UserEventData.ContainsKey(eventRinging.ConnID.ToString()))
                                    {
                                        Settings.UserEventData.Add(eventRinging.ConnID.ToString(), SFDCCallType.Inbound);
                                    }
                                }
                                else if (eventRinging.CallType == CallType.Consult)
                                {
                                    IsConsultCallReceived = true;
                                    CallRinging.GetInstance().PopupRecords(eventRinging, SFDCCallType.ConsultReceived);
                                    if (!Settings.UserEventData.ContainsKey(eventRinging.ConnID.ToString()))
                                    {
                                        Settings.UserEventData.Add(eventRinging.ConnID.ToString(), SFDCCallType.ConsultReceived);
                                    }
                                }
                                break;
                            #endregion

                            #region EventPartyChanged
                            case EventPartyChanged.MessageId:
                                EventPartyChanged partyChanged = (EventPartyChanged)events;
                                IsConsultCallReceived = true;
                                if (CallDurationData.ContainsKey(partyChanged.PreviousConnID.ToString())&& !CallDurationData.ContainsKey(partyChanged.ConnID.ToString()))
                                {
                                    CallDurationData.Add(partyChanged.ConnID.ToString(), CallDurationData[partyChanged.PreviousConnID.ToString()]);
                                }
                                if (Settings.UpdateSFDCLog.ContainsKey(partyChanged.PreviousConnID.ToString()))
                                {
                                    Settings.UpdateSFDCLog.Add(partyChanged.ConnID.ToString(), Settings.UpdateSFDCLog[partyChanged.PreviousConnID.ToString()]);
                                }
                                this.consultConnId = partyChanged.PreviousConnID.ToString();
                                break;
                            #endregion

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

                                    if (!Settings.UserEventData.ContainsKey(eventDialing.ConnID.ToString()))
                                    {
                                        Settings.UserEventData.Add(eventDialing.ConnID.ToString(), SFDCCallType.OutboundSuccess);
                                    }
                                }
                                else if (eventDialing.CallType == CallType.Consult)
                                {
                                    CallDialing.GetInstance().PopupRecords(eventDialing, SFDCCallType.ConsultSuccess);

                                    if (!Settings.UserEventData.ContainsKey(eventDialing.ConnID.ToString()))
                                    {
                                        Settings.UserEventData.Add(eventDialing.ConnID.ToString(), SFDCCallType.ConsultSuccess);
                                    }

                                }
                                break;
                            #endregion

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
                                    if(IsConsultCallReceived)
                                        CallConnected.GetInstance().PopupRecords(eventEstablished, SFDCCallType.ConsultReceived);
                                    else
                                        CallConnected.GetInstance().PopupRecords(eventEstablished, SFDCCallType.ConsultSuccess);
                                }


                                break;
                            #endregion

                            #region EventReleased
                            case EventReleased.MessageId:

                                eventReleased = (EventReleased)events;

                                if (eventReleased.CallState != 22)
                                {
                                    string callDuration = string.Empty;
                                    if (CallDurationData.ContainsKey(eventReleased.ConnID.ToString()))
                                    {
                                        TimeSpan ts = System.DateTime.Now.Subtract(CallDurationData[eventReleased.ConnID.ToString()]);
                                        callDuration = ts.Hours + "Hr " + ts.Minutes + " mins " + ts.Seconds + "secs";
                                        if (!FinishedCallDuration.ContainsKey(eventReleased.ConnID.ToString()))
                                        {
                                            FinishedCallDuration.Add(eventReleased.ConnID.ToString(), callDuration);
                                        }
                                        else
                                        {
                                            FinishedCallDuration[eventReleased.ConnID.ToString()] = callDuration;
                                        }
                                    }
                                    if (eventReleased.CallType == CallType.Inbound)
                                    {
                                        if (!IsConsultCallReceived)
                                        {
                                            CallReleased.GetInstance().PopupRecords(eventReleased, SFDCCallType.Inbound, callDuration);
                                        }
                                        else
                                        {
                                            IsConsultCallReceived = false;
                                            CallReleased.GetInstance().PopupRecords(eventReleased, SFDCCallType.ConsultReceived, callDuration);
                                        }

                                    }
                                    else if (eventReleased.CallType == CallType.Unknown || eventReleased.CallType == CallType.Outbound)
                                    {
                                        if (IsConsultCallReceived)
                                        {
                                            CallReleased.GetInstance().PopupRecords(eventReleased, SFDCCallType.ConsultReceived, callDuration);
                                            IsConsultCallReceived = false;
                                        }
                                        else
                                        {
                                            CallReleased.GetInstance().PopupRecords(eventReleased, SFDCCallType.OutboundSuccess, callDuration);
                                        }
                                    }
                                    else if (eventReleased.CallType == CallType.Consult)
                                    {
                                        if (IsConsultCallReceived)
                                        {
                                            CallReleased.GetInstance().PopupRecords(eventReleased, SFDCCallType.ConsultReceived, callDuration);
                                            IsConsultCallReceived = false;
                                        }
                                        else
                                            CallReleased.GetInstance().PopupRecords(eventReleased, SFDCCallType.ConsultSuccess, callDuration);
                                    }
                                }

                                break;
                            #endregion

                            #region EventUserEvent
                            case EventUserEvent.MessageId:
                                EventUserEvent userEvent = (EventUserEvent)events;

                                if (userEvent.ConnID != null)
                                {
                                    if (Settings.UserEventData.ContainsKey(userEvent.ConnID.ToString()))
                                    {
                                        if (FinishedCallDuration.ContainsKey(userEvent.ConnID.ToString()))
                                            CallUserEvent.GetInstance().UpdateRecords(userEvent, Settings.UserEventData[userEvent.ConnID.ToString()], FinishedCallDuration[userEvent.ConnID.ToString()]);
                                        else
                                            CallUserEvent.GetInstance().UpdateRecords(userEvent, Settings.UserEventData[userEvent.ConnID.ToString()], "");
                                    }

                                }

                                break;
                            #endregion

                            #region EventError
                            case EventError.MessageId:
                                EventError eventError = (EventError)events;
                                if (eventError.CallType == CallType.Outbound)
                                {
                                    CallEventError.GetInstance().PopupRecords(eventError, SFDCCallType.OutboundFailure);
                                }
                                else if (eventError.CallType == CallType.Consult)
                                {
                                    CallEventError.GetInstance().PopupRecords(eventError, SFDCCallType.ConsultFailure);
                                }
                                break;
                            #endregion

                            #region EventAbandoned
                            case EventAbandoned.MessageId:
                                EventAbandoned eventAbandoned = (EventAbandoned)events;
                                if (eventAbandoned.CallType == CallType.Outbound)
                                {
                                    CallEventAbandoned.GetInstance().PopupRecords(eventAbandoned, SFDCCallType.OutboundFailure);
                                }
                                else if (eventAbandoned.CallType == CallType.Consult)
                                {
                                    CallEventAbandoned.GetInstance().PopupRecords(eventAbandoned, SFDCCallType.ConsultFailure);
                                }
                                break;
                            #endregion

                            #region EventDestinationBusy
                            case EventDestinationBusy.MessageId:

                                EventDestinationBusy destinationBusyEvent = (EventDestinationBusy)events;
                                if (destinationBusyEvent.CallType == CallType.Outbound)
                                {
                                    CallEventDestinationBusy.GetInstance().PopupRecords(destinationBusyEvent, SFDCCallType.OutboundFailure);
                                }
                                else if (destinationBusyEvent.CallType == CallType.Consult)
                                {
                                    CallEventDestinationBusy.GetInstance().PopupRecords(destinationBusyEvent, SFDCCallType.ConsultFailure);
                                }
                                break;
                            #endregion

                            default:
                                logger.Info("ReceiveCalls : Unhandled Event " + events.Name);
                                break;
                        }
                    }
                }
            }
            catch (Exception generalException)
            {
                logger.Error("ReceiveCalls : Error occured while receiving voice events " + generalException.ToString());
            }
        }
        #endregion

        #region Update Activity Log on Disposition Code Set
        public void UpdateActivityForReleasedEvent(IMessage message)
        {
            try
            {
                this.logger.Info("UpdateActivityForReleasedEvent : Event Name " + message.ToString());
                switch (message.Id)
                {
                    case EventAttachedDataChanged.MessageId:
                        EventAttachedDataChanged attachedDataChanged = (EventAttachedDataChanged)message;
                        
                        if (CallDurationData.ContainsKey(attachedDataChanged.ConnID.ToString()))
                        {
                            TimeSpan ts = System.DateTime.Now.Subtract(CallDurationData[attachedDataChanged.ConnID.ToString()]);
                            callDuration = ts.Hours + "Hr " + ts.Minutes + " mins " + ts.Seconds + "secs";
                        }
                        if (attachedDataChanged.CallType == CallType.Inbound)
                        {
                            if (this.consultConnId==attachedDataChanged.ConnID.ToString())
                                CallAttachedDataChanged.GetInstance().UpdateRecords(attachedDataChanged, SFDCCallType.ConsultReceived, callDuration);
                            else
                                CallAttachedDataChanged.GetInstance().UpdateRecords(attachedDataChanged, SFDCCallType.Inbound, callDuration);
                           
                            
                        }
                        else if ((attachedDataChanged.CallType == CallType.Outbound) || (attachedDataChanged.CallType == CallType.Unknown))
                        {
                            CallAttachedDataChanged.GetInstance().UpdateRecords(attachedDataChanged, SFDCCallType.OutboundSuccess, callDuration);
                        }
                        else if (attachedDataChanged.CallType == CallType.Consult)
                        {
                            CallAttachedDataChanged.GetInstance().UpdateRecords(attachedDataChanged, SFDCCallType.ConsultSuccess, callDuration);
                        }

                        break;

                    #region EventReleased
                    case EventReleased.MessageId:
                        EventReleased released = (EventReleased)message;

                        if (FinishedCallDuration.ContainsKey(released.ConnID.ToString()))
                        {
                            callDuration = FinishedCallDuration[released.ConnID.ToString()];
                        }
                        else
                        {
                            callDuration = string.Empty;
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
                            CallReleased.GetInstance().UpdateRecords(released, SFDCCallType.OutboundSuccess, callDuration);
                        }
                        else if (released.CallType == CallType.Consult)
                        {
                            CallReleased.GetInstance().UpdateRecords(released, SFDCCallType.ConsultSuccess, callDuration);
                        }

                        break;
                    #endregion

                    default:
                        break;
                }
            }
            catch (Exception generalException)
            {
                this.logger.Error("UpdateActivityForReleasedEvent : Error Occurred : " + generalException.ToString());
            }
        }
        #endregion

    }
}
