using Genesyslab.Platform.Commons.Protocols;
using Genesyslab.Platform.Contacts.Protocols.ContactServer.Events;
using Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events;
using Genesyslab.Platform.WebMedia.Protocols.BasicChat;
using Genesyslab.Platform.WebMedia.Protocols.BasicChat.Events;
using Pointel.Salesforce.Adapter.LogMessage;
using Pointel.Salesforce.Adapter.Utility;
using System;
using System.Collections.Generic;

namespace Pointel.Salesforce.Adapter.Chat
{
    public class ChatEventHandler
    {   /// <summary>
        /// Fields for the class SubscribeInteractionServerEvents </summary>

        #region Field Declaration

        public IDictionary<string, string> FinishedCallDuration = new Dictionary<string, string>();
        private string _callDuration = "00Hr 00Mins 00Sec";
        private IDictionary<string, DateTime> _callDurationData = new Dictionary<string, DateTime>();
        private IDictionary<string, EventInvite> _chatIXNCollection = new Dictionary<string, EventInvite>();
        private EventInvite _eventInvite = null;

        //private bool _isConsultReceived = false;
        private string _lastSessionId = string.Empty;

        private Log _logger = null;
        private EventSessionInfo _sessionInfo = null;

        #endregion Field Declaration

        #region SubscribeAgentChatEvents

        /// <summary>
        /// Constructor of the Class SubscribeInteractionServerEvents
        /// </summary>
        public ChatEventHandler()
        {
            try
            {
                _logger = Log.GenInstance();
            }
            catch
            {
                _logger.Error("SubscribeInteractionServerEvents : Error occoured at Constructor");
            }
        }

        #endregion SubscribeAgentChatEvents

        #region RecieveChatEvents

        /// <summary>
        /// Receives Chat Events
        /// </summary>
        /// <param name="events"></param>
        public void ReceiveChatEvents(IMessage events)
        {
            try
            {
                if (Settings.SFDCOptions.SFDCPopupPages != null)
                {
                    if (events != null)
                    {
                        _logger.Info("Agent Chat Event : " + events.Name);
                        switch (events.Id)
                        {
                            case EventInvite.MessageId:
                                _eventInvite = (EventInvite)events;
                                Settings.ChatProxyClientId = _eventInvite.ProxyClientId;
                                IXNCustomData ixnData = new IXNCustomData();
                                ixnData.InteractionId = _eventInvite.Interaction.InteractionId;
                                ixnData.InteractionEvent = _eventInvite;
                                if (!_chatIXNCollection.ContainsKey(_eventInvite.Interaction.InteractionId))
                                {
                                    _chatIXNCollection.Add(_eventInvite.Interaction.InteractionId, _eventInvite);
                                }
                                GetChatData(_eventInvite.Interaction.InteractionId, ixnData);
                                if (_eventInvite.Interaction.InteractionType == "Inbound")
                                {
                                    if (_eventInvite.VisibilityMode.ToString() != "Coach" && _eventInvite.VisibilityMode.ToString() != "Conference")
                                    {
                                        ChatInvite.GetInstance().PopupRecords(ixnData, SFDCCallType.InboundChat);
                                    }
                                    else
                                    {
                                        ChatInvite.GetInstance().PopupRecords(ixnData, SFDCCallType.ConsultChatReceived);
                                    }
                                }
                                else if (_eventInvite.Interaction.InteractionType == "Consult")
                                {
                                    ChatInvite.GetInstance().PopupRecords(ixnData, SFDCCallType.ConsultChatReceived);
                                }
                                break;

                            case EventSessionInfo.MessageId:
                                _sessionInfo = (EventSessionInfo)events;
                                try
                                {
                                    if (Settings.SFDCOptions.ChatAttachActivityId && Settings.SFDCListener.ChatActivityIdCollection.Keys.Contains(_sessionInfo.ChatTranscript.SessionId.ToString()) && _sessionInfo.SessionStatus == SessionStatus.Alive)
                                    {
                                        Settings.SFDCListener.SetAttachedData(_sessionInfo.ChatTranscript.SessionId.ToString(), Settings.SFDCOptions.ChatAttachActivityIdKeyname, Settings.SFDCListener.ChatActivityIdCollection[_sessionInfo.ChatTranscript.SessionId.ToString()], Settings.ChatProxyClientId);
                                        Settings.SFDCListener.ChatActivityIdCollection.Remove(_sessionInfo.ChatTranscript.SessionId.ToString());
                                    }
                                    if (_lastSessionId != _eventInvite.Interaction.InteractionId)
                                    {
                                        _lastSessionId = _eventInvite.Interaction.InteractionId;

                                        if (!_callDurationData.ContainsKey(_eventInvite.Interaction.InteractionId))
                                        {
                                            _callDurationData.Add(_eventInvite.Interaction.InteractionId, System.DateTime.Now);
                                        }
                                    }
                                    if (_sessionInfo.SessionStatus == SessionStatus.Over)
                                    {
                                        if (_callDurationData.ContainsKey(_sessionInfo.ChatTranscript.SessionId))
                                        {
                                            TimeSpan ts = System.DateTime.Now.Subtract(_callDurationData[_sessionInfo.ChatTranscript.SessionId]);
                                            _callDuration = ts.Hours + "Hr " + ts.Minutes + " mins " + ts.Seconds + "secs";
                                            if (!FinishedCallDuration.ContainsKey(_sessionInfo.ChatTranscript.SessionId))
                                            {
                                                FinishedCallDuration.Add(_sessionInfo.ChatTranscript.SessionId, _callDuration);
                                            }
                                            else
                                            {
                                                FinishedCallDuration[_sessionInfo.ChatTranscript.SessionId] = _callDuration;
                                            }
                                        }
                                    }
                                }
                                catch (Exception generalException)
                                {
                                    this._logger.Error(generalException.ToString());
                                }
                                break;

                            default:
                                _logger.Info("Unhandled Event " + events.Name);
                                break;
                        }
                    }
                }
            }
            catch (Exception generalException)
            {
                _logger.Error("ReceiveChatEvents : Error occured while receiving Chat events " + generalException.ToString());
            }
        }

        #endregion RecieveChatEvents

        #region UpdateDispositionCode

        /// <summary>
        /// Gets Updated Disposition Code Change
        /// </summary>
        /// <param name="ixnId"></param>
        /// <param name="key"></param>
        /// <param name="code"></param>
        public void UpdateDispositionCodeChange(IXNCustomData dispCode)
        {
            try
            {
                this._logger.Info("UpdateDispositionCodeChange()");
                GetChatData(dispCode.InteractionId, dispCode);
                ChatInvite.GetInstance().UpdateRecords(dispCode, dispCode.InteractionType);
            }
            catch (Exception generalException)
            {
                this._logger.Error("UpdateDispositionCodeChange: Error at updating disposition code Changed : " + generalException.ToString());
            }
        }

        #endregion UpdateDispositionCode

        #region Events from WDE Command

        public void InteractionEventChat(IXNCustomData chatData)
        {
            try
            {
                this._logger.Info("Chat Event Received : " + chatData.EventName);
                switch (chatData.EventName)
                {
                    case "InteractionChatAcceptChat":
                        GetChatData(chatData.InteractionId, chatData);
                        ChatSessionConnected.GetInstance().PopupRecords(chatData, SFDCCallType.InboundChat);
                        return;

                    case "InteractionChatDeclineChat":
                        GetChatData(chatData.InteractionId, chatData);
                        ChatRejected.GetInstance().UpdateRecords(chatData, chatData.InteractionType);
                        return;

                    case "InteractionChatCloseInteraction":

                    case "InteractionChatDisconnectChat":

                    case "InteractionChatAutoCloseInteraction":

                    case "InteractionChatAutoDisconnect":
                        GetChatData(chatData.InteractionId, chatData);
                        ChatSessionEnded.GetInstance().PopupRecords(chatData, chatData.InteractionType);
                        return;

                    case "InteractionMarkDone":
                        GetChatData(chatData.InteractionId, chatData);
                        ChatMarkDone.GetInstance().UpdateRecords(chatData, chatData.InteractionType);
                        break;

                    default:

                        return;
                }
            }
            catch (Exception generalException)
            {
                this._logger.Error("Error occurred while processing chat event, exception : " + generalException.ToString());
            }
        }

        #endregion Events from WDE Command

        #region GetChatData

        private void GetChatData(string ixnId, IXNCustomData chatData)
        {
            try
            {
                this._logger.Info("Retrieving UCS data for the InteractionId : " + ixnId + "\t EventName : " + chatData.EventName);
                EventGetInteractionContent chatContent = Settings.SFDCListener.GetOpenMediaInteractionContent(ixnId, false);
                if (chatContent != null)
                {
                    if (chatContent.InteractionAttributes.TypeId == "Inbound")
                        chatData.InteractionType = SFDCCallType.InboundChat;
                    else if (chatContent.InteractionAttributes.TypeId == "Consult")
                        chatData.InteractionType = SFDCCallType.ConsultChatReceived;

                    EventInvite currentEvent = null;
                    _chatIXNCollection.TryGetValue(ixnId, out currentEvent);
                    chatData.InteractionEvent = currentEvent;
                    chatData.IXN_Attributes = chatContent.InteractionAttributes;
                    chatData.EntityAttributes = chatContent.EntityAttributes;
                    chatData.AttachmentLists = chatContent.Attachments;
                    chatData.InteractionContents = chatContent.InteractionContent;
                    if (chatData.UserData == null)
                        chatData.UserData = chatContent.InteractionAttributes.AllAttributes;

                    if (chatData.InteractionNotes == null && chatContent.InteractionAttributes != null)
                        chatData.InteractionNotes = chatContent.InteractionAttributes.TheComment;

                    if (!Settings.SFDCOptions.CanUseGenesysCallDuration || string.IsNullOrWhiteSpace(chatData.Duration))
                    {
                        if (_callDurationData.ContainsKey(chatData.InteractionId))
                        {
                            TimeSpan ts = System.DateTime.Now.Subtract(_callDurationData[chatData.InteractionId]);
                            chatData.Duration = ts.Hours + "Hr " + ts.Minutes + " Mins " + ts.Seconds + "Secs";
                        }
                    }
                }
                else
                    this._logger.Info("GetChatData: Null is returned from UCS for the  interaction id : " + chatData.InteractionId);
            }
            catch (Exception generalException)
            {
                this._logger.Info("GetChatData: Error occurred, Exception : " + generalException.ToString());
            }
        }

        #endregion GetChatData
    }
}