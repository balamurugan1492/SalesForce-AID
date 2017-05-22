using Genesyslab.Platform.Commons.Protocols;
using Genesyslab.Platform.WebMedia.Protocols;
using Genesyslab.Platform.WebMedia.Protocols.BasicChat;
using Genesyslab.Platform.WebMedia.Protocols.BasicChat.Events;
using Pointel.Configuration.Manager;
using Pointel.Interactions.Chat.Core;
using Pointel.Interactions.Chat.Core.General;
using Pointel.Interactions.Chat.Helpers;
using Pointel.Interactions.Chat.Settings;
using Pointel.Interactions.Chat.UserControls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;
using Pointel.Interactions.Chat.CustomControls;
using System.Windows.Interop;
using Pointel.Interactions.Chat.WinForms;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Pointel.Interactions.Chat.HandleInteractions
{
    public class ChatListener
    {
        #region Field Declaration
        private Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");
        private enum InfoType { MessageInfo, NewPartyInfo, NoticeInfo, PartyLeftInfo }
        string employeeID = string.Empty;
        string placeID = string.Empty;
        private ChatDataContext _chatDataContext = ChatDataContext.GetInstance();
        WindowInteropHelper winInteropHelper;
        RadialAnimation radialAnimationHelper;
        public Notifier _taskbarNotifier = null;
        private MediaPlayer mediaPlayer;
        private bool isPlayTone = false;
        private DispatcherTimer timerforstopplayingtone;
        #endregion

        /// <summary>
        /// Chat_s the session.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Chat_Session(Object messages, string interactionId, Notifier notifier)
        {
            IMessage message = messages as IMessage;
            ChatMedia chatMedia = new ChatMedia();
            ChatTranscript transcript = null;
            bool isDelayToCustomerResponse = false;
            _taskbarNotifier = notifier;
            if (message != null)
                Application.Current.Dispatcher.Invoke((System.Action)(delegate
                {
                    try
                    {
                        switch (message.Id)
                        {
                            #region EventSessionInfo
                            case EventSessionInfo.MessageId:
                                if (message != null && message.Id == EventSessionInfo.MessageId)
                                {
                                    EventSessionInfo eventSessionInfo = (EventSessionInfo)message;
                                    logger.Trace(eventSessionInfo.ToString());

                                    transcript = eventSessionInfo.ChatTranscript;
                                    if (transcript != null)
                                    {
                                        string _interactionID = transcript.SessionId;
                                        if (!_chatDataContext.MainWindowSession.ContainsKey(_interactionID)) return;
                                        (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).SessionID = transcript.SessionId;
                                        logger.Trace(transcript.ToString());

                                        (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).SessionStartedTime = Convert.ToDateTime(transcript.StartAt.ToString());
                                        if (transcript.ChatEventList != null)
                                        {
                                            IEnumerator iteration = transcript.ChatEventList.GetEnumerator();
                                            while (iteration.MoveNext())
                                            {
                                                object objinfo = iteration.Current;
                                                if (objinfo != null)
                                                {
                                                    #region MessageInfo
                                                    if (objinfo is MessageInfo)
                                                    {
                                                        MessageInfo _newMessageInfo = (MessageInfo)objinfo;
                                                        if (_newMessageInfo.MessageText != null)
                                                        {
                                                            //if (!String.IsNullOrEmpty(_newMessageInfo.MessageText.Text))
                                                            //{
                                                            ObservableCollection<IPartyInfo> temp = (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).PartiesInfo;
                                                            var getUserNickName = temp.Where(x => x.UserID == _newMessageInfo.UserId).ToList();
                                                            if (getUserNickName.Count > 0)
                                                                foreach (var item in getUserNickName)
                                                                {
                                                                    if (_newMessageInfo.Visibility == Genesyslab.Platform.WebMedia.Protocols.BasicChat.Visibility.All)
                                                                    {
                                                                        if (item.UserType == ChatDataContext.ChatUsertype.Client.ToString())
                                                                        {
                                                                            isDelayToCustomerResponse = true;
                                                                            chatConversationBinding(_interactionID, ChatDataContext.ChatUsertype.Client, getCurrentTimewithSpecFormat(_newMessageInfo.TimeShift, _interactionID) + " " + item.UserNickName.ToString() + ": ".ToString(), InfoType.MessageInfo, _newMessageInfo.MessageText.Text.ToString(), string.Empty);
                                                                        }
                                                                        else if (item.UserType == ChatDataContext.ChatUsertype.Agent.ToString())
                                                                        {
                                                                            isDelayToCustomerResponse = false;
                                                                            if (_chatDataContext.AgentNickName == item.UserNickName)
                                                                            {
                                                                                chatConversationBinding(_interactionID, ChatDataContext.ChatUsertype.Agent, getCurrentTimewithSpecFormat(_newMessageInfo.TimeShift, _interactionID) + " " + item.UserNickName.ToString() + ": ".ToString(), InfoType.MessageInfo, _newMessageInfo.MessageText.Text.ToString(), string.Empty);
                                                                            }
                                                                            else
                                                                            {
                                                                                chatConversationBinding(_interactionID, ChatDataContext.ChatUsertype.OtherAgent, getCurrentTimewithSpecFormat(_newMessageInfo.TimeShift, _interactionID) + " " + item.UserNickName.ToString() + ": ".ToString(), InfoType.MessageInfo, _newMessageInfo.MessageText.Text.ToString(), string.Empty);
                                                                            }
                                                                        }
                                                                    }
                                                                    else if (_newMessageInfo.Visibility == Genesyslab.Platform.WebMedia.Protocols.BasicChat.Visibility.Int)
                                                                    {
                                                                        if (item.UserType == ChatDataContext.ChatUsertype.Client.ToString())
                                                                        {
                                                                            isDelayToCustomerResponse = true;
                                                                            chatConsultConversationBinding(_interactionID, ChatDataContext.ChatUsertype.Client, getCurrentTimewithSpecFormat(_newMessageInfo.TimeShift, _interactionID) + " " + item.UserNickName.ToString() + ": ".ToString(), InfoType.MessageInfo, _newMessageInfo.MessageText.Text.ToString(), string.Empty);
                                                                        }
                                                                        else if (item.UserType == ChatDataContext.ChatUsertype.Agent.ToString())
                                                                        {
                                                                            isDelayToCustomerResponse = false;
                                                                            if (_chatDataContext.AgentNickName == item.UserNickName)
                                                                            {
                                                                                chatConsultConversationBinding(_interactionID, ChatDataContext.ChatUsertype.Agent, getCurrentTimewithSpecFormat(_newMessageInfo.TimeShift, _interactionID) + " " + item.UserNickName.ToString() + ": ".ToString(), InfoType.MessageInfo, _newMessageInfo.MessageText.Text.ToString(), string.Empty);
                                                                            }
                                                                            else
                                                                            {
                                                                                chatConsultConversationBinding(_interactionID, ChatDataContext.ChatUsertype.OtherAgent, getCurrentTimewithSpecFormat(_newMessageInfo.TimeShift, _interactionID) + " " + item.UserNickName.ToString() + ": ".ToString(), InfoType.MessageInfo, _newMessageInfo.MessageText.Text.ToString(), string.Empty);
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                        }
                                                        // }
                                                    }

                                                    #endregion MessageInfo

                                                    #region NoticeInfo
                                                    if (objinfo is NoticeInfo)
                                                    {
                                                        NoticeInfo noticeInfo = (NoticeInfo)objinfo;
                                                        if (noticeInfo != null)
                                                        {
                                                            ObservableCollection<IPartyInfo> temp = (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).PartiesInfo;
                                                            var getUserNickName = temp.Where(x => x.UserID == noticeInfo.UserId).ToList();
                                                            if (getUserNickName.Count > 0)
                                                                foreach (var item in getUserNickName)
                                                                {
                                                                    if (noticeInfo.MessageText != null)
                                                                        if (!String.IsNullOrEmpty(noticeInfo.MessageText.Text))
                                                                        {
                                                                            if (item.UserType == ChatDataContext.ChatUsertype.Client.ToString())
                                                                            {
                                                                                chatConversationBinding(_interactionID, ChatDataContext.ChatUsertype.Client, getCurrentTimewithSpecFormat(noticeInfo.TimeShift, _interactionID) + " " + item.UserNickName.ToString() + ": ".ToString(), InfoType.NoticeInfo, " " + noticeInfo.MessageText.Text.ToString(), string.Empty);
                                                                            }
                                                                            else if (item.UserType == ChatDataContext.ChatUsertype.Agent.ToString())
                                                                            {
                                                                                if (_chatDataContext.AgentNickName == item.UserNickName)
                                                                                {
                                                                                    chatConversationBinding(_interactionID, ChatDataContext.ChatUsertype.Agent, getCurrentTimewithSpecFormat(noticeInfo.TimeShift, _interactionID) + " " + item.UserNickName.ToString() + ": ".ToString(), InfoType.NoticeInfo, " " + noticeInfo.MessageText.Text.ToString(), string.Empty);
                                                                                }
                                                                                else
                                                                                {
                                                                                    chatConversationBinding(_interactionID, ChatDataContext.ChatUsertype.OtherAgent, getCurrentTimewithSpecFormat(noticeInfo.TimeShift, _interactionID) + " " + item.UserNickName.ToString() + ": ".ToString(), InfoType.NoticeInfo, " " + noticeInfo.MessageText.Text.ToString(), string.Empty);
                                                                                }
                                                                            }
                                                                        }
                                                                    if (noticeInfo.NoticeText != null)
                                                                        if (noticeInfo.NoticeText.NoticeType == NoticeType.PushUrl && !String.IsNullOrEmpty(noticeInfo.NoticeText.Text.ToString()))
                                                                        {
                                                                            if (item.UserType == ChatDataContext.ChatUsertype.Client.ToString())
                                                                            {
                                                                                isDelayToCustomerResponse = true;
                                                                                chatConversationBinding(_interactionID, ChatDataContext.ChatUsertype.Client, getCurrentTimewithSpecFormat(noticeInfo.TimeShift, _interactionID), InfoType.NoticeInfo, " " + item.UserNickName.ToString() + " Pushed the URL ", noticeInfo.NoticeText.Text.ToString());

                                                                            }
                                                                            else if (item.UserType == ChatDataContext.ChatUsertype.Agent.ToString())
                                                                            {
                                                                                if (_chatDataContext.AgentNickName == item.UserNickName)
                                                                                {
                                                                                    isDelayToCustomerResponse = false;
                                                                                    chatConversationBinding(_interactionID, ChatDataContext.ChatUsertype.Agent, getCurrentTimewithSpecFormat(noticeInfo.TimeShift, _interactionID), InfoType.NoticeInfo, " " + item.UserNickName.ToString() + " Pushed the URL ", noticeInfo.NoticeText.Text.ToString());
                                                                                }
                                                                                else
                                                                                {
                                                                                    chatConversationBinding(_interactionID, ChatDataContext.ChatUsertype.OtherAgent, getCurrentTimewithSpecFormat(noticeInfo.TimeShift, _interactionID), InfoType.NoticeInfo, " " + item.UserNickName.ToString() + " Pushed the URL ", noticeInfo.NoticeText.Text.ToString());
                                                                                }
                                                                            }
                                                                        }
                                                                }
                                                        }
                                                    }

                                                    #endregion NoticeInfo

                                                    #region NewPartyInfo
                                                    if (objinfo is NewPartyInfo)
                                                    {
                                                        employeeID = string.Empty;
                                                        placeID = string.Empty;
                                                        NewPartyInfo _newUserMessageInfo = (NewPartyInfo)objinfo;
                                                        employeeID = _newUserMessageInfo.UserInfo.PersonId;
                                                        if ((_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatParties.Count > 0 && !string.IsNullOrEmpty(employeeID))
                                                        {
                                                            if ((_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatParties.ContainsKey(employeeID))
                                                                placeID = (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatParties[employeeID].ToString();
                                                        }
                                                        (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatPersonStatusIcon = _chatDataContext.GetBitmapImage(new Uri(_chatDataContext.Imagepath + "\\Chat\\Chat.png", UriKind.Relative));
                                                        if (_newUserMessageInfo != null)
                                                            if (_newUserMessageInfo.Visibility == Genesyslab.Platform.WebMedia.Protocols.BasicChat.Visibility.Int)
                                                            {
                                                                if (_newUserMessageInfo.UserInfo.UserType == UserType.Client)
                                                                {
                                                                    if ((_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).PartiesInfo.Count(p => p.UserID == _newUserMessageInfo.UserId) == 0)
                                                                        (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).PartiesInfo.Add(new PartyInfo(_newUserMessageInfo.UserId, _newUserMessageInfo.UserInfo.UserType.ToString(), _newUserMessageInfo.UserInfo.UserNickname.ToString(), "Connected", _newUserMessageInfo.UserInfo.PersonId, "Int"));

                                                                    chatConsultConversationBinding(_interactionID, ChatDataContext.ChatUsertype.Client, getCurrentTimewithSpecFormat(_newUserMessageInfo.TimeShift, _interactionID), InfoType.NewPartyInfo, " New party '" + _newUserMessageInfo.UserInfo.UserNickname.ToString() + "' has joined the session", string.Empty);

                                                                }
                                                                else if (_newUserMessageInfo.UserInfo.UserType == UserType.Agent)
                                                                {
                                                                    if ((_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).PartiesInfo.Count(p => p.UserID == _newUserMessageInfo.UserId) == 0)
                                                                        (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).PartiesInfo.Add(new PartyInfo(_newUserMessageInfo.UserId, _newUserMessageInfo.UserInfo.UserType.ToString(), _newUserMessageInfo.UserInfo.UserNickname.ToString(), "Connected", _newUserMessageInfo.UserInfo.PersonId, "Int"));

                                                                    if (_chatDataContext.AgentNickName == _newUserMessageInfo.UserInfo.UserNickname.ToString())
                                                                    {
                                                                        chatConsultConversationBinding(_interactionID, ChatDataContext.ChatUsertype.Agent, getCurrentTimewithSpecFormat(_newUserMessageInfo.TimeShift, _interactionID), InfoType.NewPartyInfo, " New party '" + _newUserMessageInfo.UserInfo.UserNickname.ToString() + "' has joined the session", string.Empty);
                                                                    }
                                                                    else
                                                                    {
                                                                        //if ((_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).IxnType != "Consult")
                                                                        //{
                                                                        //    if ((_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatPersonsStatusInfo.Count(p => p.ChatPersonName == _newUserMessageInfo.UserInfo.UserNickname.ToString()) == 0)
                                                                        //        (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatPersonsStatusInfo.Add(new ChatPersonsStatus(employeeID, placeID, _newUserMessageInfo.UserInfo.UserNickname.ToString(), (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatPersonStatusIcon, "Connected"));
                                                                        //}
                                                                        chatConsultConversationBinding(_interactionID, ChatDataContext.ChatUsertype.OtherAgent, getCurrentTimewithSpecFormat(_newUserMessageInfo.TimeShift, _interactionID), InfoType.NewPartyInfo, " New party '" + _newUserMessageInfo.UserInfo.UserNickname.ToString() + "' has joined the session", string.Empty);
                                                                    }
                                                                }
                                                                loadConsultUserControlData(_newUserMessageInfo, _interactionID);
                                                            }
                                                            else if (_newUserMessageInfo.Visibility == Genesyslab.Platform.WebMedia.Protocols.BasicChat.Visibility.All)
                                                            {
                                                                if (_newUserMessageInfo.UserInfo.UserType == UserType.Client)
                                                                {
                                                                    if ((_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).PartiesInfo.Count(p => p.UserID == _newUserMessageInfo.UserId) == 0)
                                                                        (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).PartiesInfo.Add(new PartyInfo(_newUserMessageInfo.UserId, _newUserMessageInfo.UserInfo.UserType.ToString(), _newUserMessageInfo.UserInfo.UserNickname.ToString(), "Connected", _newUserMessageInfo.UserInfo.PersonId, "All"));

                                                                    chatConversationBinding(_interactionID, ChatDataContext.ChatUsertype.Client, getCurrentTimewithSpecFormat(_newUserMessageInfo.TimeShift, _interactionID), InfoType.NewPartyInfo, " New party '" + _newUserMessageInfo.UserInfo.UserNickname.ToString() + "' has joined the session", string.Empty);

                                                                }
                                                                else if (_newUserMessageInfo.UserInfo.UserType == UserType.Agent)
                                                                {
                                                                    if ((_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).PartiesInfo.Count(p => p.UserID == _newUserMessageInfo.UserId) == 0)
                                                                        (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).PartiesInfo.Add(new PartyInfo(_newUserMessageInfo.UserId, _newUserMessageInfo.UserInfo.UserType.ToString(), _newUserMessageInfo.UserInfo.UserNickname.ToString(), "Connected", _newUserMessageInfo.UserInfo.PersonId, "All"));

                                                                    if (_chatDataContext.AgentNickName == _newUserMessageInfo.UserInfo.UserNickname.ToString())
                                                                    {
                                                                        chatConversationBinding(_interactionID, ChatDataContext.ChatUsertype.Agent, getCurrentTimewithSpecFormat(_newUserMessageInfo.TimeShift, _interactionID), InfoType.NewPartyInfo, " New party '" + _newUserMessageInfo.UserInfo.UserNickname.ToString() + "' has joined the session", string.Empty);
                                                                    }
                                                                    else
                                                                    {
                                                                        if ((_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).IxnType != "Consult")
                                                                        {
                                                                            if ((_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatPersonsStatusInfo.Count(p => p.ChatPersonName == _newUserMessageInfo.UserInfo.UserNickname.ToString()) == 0)
                                                                                (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatPersonsStatusInfo.Add(new ChatPersonsStatus(employeeID, placeID, _newUserMessageInfo.UserInfo.UserNickname.ToString(), (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatPersonStatusIcon, "Connected"));
                                                                        }
                                                                        else
                                                                        {
                                                                            if ((_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatConsultPersonStatusInfo.Count(p => p.ChatPersonName == _newUserMessageInfo.UserInfo.UserNickname.ToString()) == 0)
                                                                                (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatConsultPersonStatusInfo.Add(new ChatPersonsStatus(employeeID, placeID, _newUserMessageInfo.UserInfo.UserNickname.ToString(), (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatPersonStatusIcon, "Connected"));
                                                                        }
                                                                        chatConversationBinding(_interactionID, ChatDataContext.ChatUsertype.OtherAgent, getCurrentTimewithSpecFormat(_newUserMessageInfo.TimeShift, _interactionID), InfoType.NewPartyInfo, " New party '" + _newUserMessageInfo.UserInfo.UserNickname.ToString() + "' has joined the session", string.Empty);
                                                                    }
                                                                }
                                                            }
                                                        ObservableCollection<Pointel.Interactions.Chat.Helpers.IPartyInfo> temp = (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).PartiesInfo;
                                                        var getUsers = temp.Where(x => x.UserType == "Agent" && x.UserState == "Connected").ToList();
                                                        if (getUsers.Count >= 2)
                                                        {
                                                            (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).IsConferenceChat = true;
                                                            (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).IsChatConferenceClick = true;
                                                        }
                                                        else
                                                        {
                                                            (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).IsConferenceChat = false;
                                                            (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).IsChatConferenceClick = false;
                                                        }
                                                    }
                                                    #endregion NewPartyInfo

                                                    #region PartyLeftInfo
                                                    if (objinfo is PartyLeftInfo)
                                                    {
                                                        var _partyLeftInfo = (PartyLeftInfo)objinfo;
                                                        var tempChatStatus = (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatPersonsStatusInfo;
                                                        var tempConsultChatStatus = (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatConsultPersonStatusInfo;
                                                        var temp = (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).PartiesInfo;
                                                        if (_partyLeftInfo != null)
                                                        {
                                                            var getUserNickName = temp.Where(x => x.UserID == _partyLeftInfo.UserId).ToList();
                                                            if (getUserNickName.Count > 0)
                                                                foreach (var item in getUserNickName)
                                                                {
                                                                    int position = temp.IndexOf(temp.Where(p => p.UserNickName == item.UserNickName).FirstOrDefault());

                                                                    (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).PartiesInfo.RemoveAt(position);
                                                                    if (_partyLeftInfo.Visibility == Genesyslab.Platform.WebMedia.Protocols.BasicChat.Visibility.Int)
                                                                    {
                                                                        if (item.UserType == ChatDataContext.ChatUsertype.Client.ToString())
                                                                        {
                                                                            chatConsultConversationBinding(_interactionID, ChatDataContext.ChatUsertype.Client, getCurrentTimewithSpecFormat(_partyLeftInfo.TimeShift, _interactionID), InfoType.MessageInfo, " Party '" + item.UserNickName.ToString() + "' has left the session", string.Empty);
                                                                        }
                                                                        else if (item.UserType == ChatDataContext.ChatUsertype.Agent.ToString())
                                                                        {
                                                                            if (_chatDataContext.AgentNickName == item.UserNickName)
                                                                            {
                                                                                chatConsultConversationBinding(_interactionID, ChatDataContext.ChatUsertype.Agent, getCurrentTimewithSpecFormat(_partyLeftInfo.TimeShift, _interactionID), InfoType.MessageInfo, " Party '" + item.UserNickName.ToString() + "' has left the session", string.Empty);
                                                                            }
                                                                            else
                                                                            {
                                                                                var getUser = tempConsultChatStatus.Where(x => x.ChatPersonName == item.UserNickName).ToList();
                                                                                if (getUser.Count > 0)
                                                                                {
                                                                                    foreach (var data in getUser)
                                                                                    {
                                                                                        //(_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatPersonsStatusInfo.Remove(data);
                                                                                        (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatConsultPersonStatusInfo.Remove(data);
                                                                                    }
                                                                                }
                                                                                chatConsultConversationBinding(_interactionID, ChatDataContext.ChatUsertype.OtherAgent, getCurrentTimewithSpecFormat(_partyLeftInfo.TimeShift, _interactionID), InfoType.MessageInfo, " Party '" + item.UserNickName.ToString() + "' has left the session", string.Empty);
                                                                            }
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        if (item.UserType == ChatDataContext.ChatUsertype.Client.ToString())
                                                                        {
                                                                            chatConversationBinding(_interactionID, ChatDataContext.ChatUsertype.Client, getCurrentTimewithSpecFormat(_partyLeftInfo.TimeShift, _interactionID), InfoType.MessageInfo, " Party '" + item.UserNickName.ToString() + "' has left the session", string.Empty);
                                                                        }
                                                                        else if (item.UserType == ChatDataContext.ChatUsertype.Agent.ToString())
                                                                        {
                                                                            if (_chatDataContext.AgentNickName == item.UserNickName)
                                                                            {
                                                                                chatConversationBinding(_interactionID, ChatDataContext.ChatUsertype.Agent, getCurrentTimewithSpecFormat(_partyLeftInfo.TimeShift, _interactionID), InfoType.MessageInfo, " Party '" + item.UserNickName.ToString() + "' has left the session", string.Empty);
                                                                            }
                                                                            else
                                                                            {
                                                                                var getUser = tempChatStatus.Where(x => x.ChatPersonName == item.UserNickName).ToList();
                                                                                if (getUser.Count > 0)
                                                                                {
                                                                                    foreach (var data in getUser)
                                                                                    {
                                                                                        (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatPersonsStatusInfo.Remove(data);
                                                                                    }
                                                                                }
                                                                                chatConversationBinding(_interactionID, ChatDataContext.ChatUsertype.OtherAgent, getCurrentTimewithSpecFormat(_partyLeftInfo.TimeShift, _interactionID), InfoType.MessageInfo, " Party '" + item.UserNickName.ToString() + "' has left the session", string.Empty);
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            ObservableCollection<Pointel.Interactions.Chat.Helpers.IPartyInfo> temp1 = (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).PartiesInfo;
                                                            var getUsers = temp1.Where(x => x.UserType == "Agent" && x.UserState == "Connected").ToList();
                                                            if (getUsers.Count >= 2)
                                                            {
                                                                (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).IsConferenceChat = true;
                                                                (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).IsChatConferenceClick = true;
                                                            }
                                                            else
                                                            {
                                                                (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).IsConferenceChat = false;
                                                                (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).IsChatConferenceClick = false;
                                                            }
                                                        }
                                                    }

                                                    #endregion PartyLeftInfo
                                                }
                                            }

                                            if ((_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatConsultPersonStatusInfo != null && (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatConsultPersonStatusInfo.Count > 0)
                                            {
                                                if ((_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ConsultChatWindowRowHeight == new GridLength(0))
                                                {
                                                    (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ConsultDockPanel.Children.Clear();
                                                    (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ConsultDockPanel.Children.Add(new ChatConsultationWindow((_chatDataContext.MainWindowSession[_interactionID] as ChatUtil), _interactionID));
                                                    (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ConsultChatWindowRowHeight = GridLength.Auto;
                                                }
                                                else
                                                {
                                                    (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ConsultChatWindowRowHeight = GridLength.Auto;
                                                    //(_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ConsultPersonStatus = "Connected";
                                                    //(_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ConsultReleaseImageSource = _chatDataContext.GetBitmapImage(new Uri("/Agent.Interaction.Desktop;component/Images/Chat/Chat.Release.png", UriKind.Relative));
                                                    //(_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ConsultReleaseText = "Release";
                                                    //(_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ConsultReleaseTTHeading = "End Chat";
                                                    //(_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ConsultReleaseTTContent = "Release the Chat.";
                                                    //(_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).IsButtonConsultSendEnabled = false;
                                                    //(_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).SendchatConsultWindowRowHeight = GridLength.Auto;
                                                }
                                            }


                                            // alive code here

                                            //if ((_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).TransferReleavePersonId != string.Empty)
                                            //{
                                            //    string userID = string.Empty;
                                            //    ObservableCollection<Pointel.Interactions.Chat.Helpers.IChatPersonsStatus> tempChatStatus = (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatPersonsStatusInfo;
                                            //    var getClient = tempChatStatus.Where(x => x.ChatPersonName == (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatFromPersonName).ToList();
                                            //    ObservableCollection<Pointel.Interactions.Chat.Helpers.IPartyInfo> temp = (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).PartiesInfo;
                                            //    var getUserNickName = temp.Where(x => x.PersonId == (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).TransferReleavePersonId).ToList();
                                            //    if (getUserNickName.Count > 0)
                                            //        foreach (var item in getUserNickName)
                                            //        {
                                            //            userID = item.UserID;
                                            //        }
                                            //    if (userID != string.Empty)
                                            //    {
                                            //        OutputValues output = chatMedia.DoKeepAliveReleasePartyChatSession((_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).SessionID, _chatDataContext.ProxyId, userID);
                                            //        if (output.MessageCode == "200")
                                            //        {
                                            //            foreach (var data in getClient)
                                            //            {
                                            //                int position1 = tempChatStatus.IndexOf(tempChatStatus.Where(p => p.ChatPersonName == data.ChatPersonName).FirstOrDefault());
                                            //                (_chatDataContext.MainWindowSession[interactionId] as ChatUtil).ChatPersonsStatusInfo.RemoveAt(position1);
                                            //                (_chatDataContext.MainWindowSession[interactionId] as ChatUtil).ChatPersonsStatusInfo.Insert(position1, new ChatPersonsStatus(data.AgentID, data.PlaceID, (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatFromPersonName, (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatPersonStatusIcon, "Connected"));
                                            //            }
                                            //        }
                                            //    }
                                            //    // (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).TransferReleavePersonId = string.Empty;
                                            //}
                                        }
                                    }
                                    if (isDelayToCustomerResponse)
                                    {
                                        if (!_chatDataContext.MainWindowSession.ContainsKey(eventSessionInfo.ChatTranscript.SessionId)) return;
                                        (_chatDataContext.MainWindowSession[eventSessionInfo.ChatTranscript.SessionId] as ChatUtil).NotificationImageSource = _chatDataContext.Imagepath + "\\Chat\\Chat.Pending.Notification.gif";
                                        isDelayToCustomerResponse = false;
                                    }
                                }
                                break;
                            #endregion EventSessionInfo
                            case EventError.MessageId:
                                EventError eventError = (EventError)message;
                                logger.Trace(eventError.ToString());
                                if (ChatDataContext.messageToClientChat != null)
                                {
                                    (_chatDataContext.MainWindowSession[interactionId] as ChatUtil).ReleaseImageSource = _chatDataContext.GetBitmapImage(new Uri(_chatDataContext.Imagepath + "\\Chat\\Chat.Release.Disable.png", UriKind.Relative));
                                    (_chatDataContext.MainWindowSession[interactionId] as ChatUtil).IsEnableRelease = false;
                                    (_chatDataContext.MainWindowSession[interactionId] as ChatUtil).TransImageSource = _chatDataContext.GetBitmapImage(new Uri(_chatDataContext.Imagepath + "\\Chat\\Chat.Transfer.Disable.png", UriKind.Relative));
                                    (_chatDataContext.MainWindowSession[interactionId] as ChatUtil).IsEnableTransfer = false;
                                    (_chatDataContext.MainWindowSession[interactionId] as ChatUtil).ConfImageSource = _chatDataContext.GetBitmapImage(new Uri(_chatDataContext.Imagepath + "\\Chat\\Chat.Conference.Disable.png", UriKind.Relative));
                                    (_chatDataContext.MainWindowSession[interactionId] as ChatUtil).IsEnableConference = false;
                                    (_chatDataContext.MainWindowSession[interactionId] as ChatUtil).ConsultChatImageSource = _chatDataContext.GetBitmapImage(new Uri(_chatDataContext.Imagepath + "\\Chat\\Chat.Consult.Disable.png", UriKind.Relative));
                                    (_chatDataContext.MainWindowSession[interactionId] as ChatUtil).IsEnableChatConsult = false;
                                    (_chatDataContext.MainWindowSession[interactionId] as ChatUtil).DoneImageSource = _chatDataContext.GetBitmapImage(new Uri(_chatDataContext.Imagepath + "\\Chat\\Chat.MarkDone.png", UriKind.Relative));
                                    (_chatDataContext.MainWindowSession[interactionId] as ChatUtil).IsEnableDone = true;
                                    (_chatDataContext.MainWindowSession[interactionId] as ChatUtil).VoiceConsultImageSource = _chatDataContext.GetBitmapImage(new Uri(_chatDataContext.Imagepath + "\\Chat\\Chat.Consult.Call.Disable.png", UriKind.Relative));
                                    (_chatDataContext.MainWindowSession[interactionId] as ChatUtil).IsEnableVoiceConsult = false;
                                    (_chatDataContext.MainWindowSession[interactionId] as ChatUtil).IsTextMessageEnabled = false;
                                    (_chatDataContext.MainWindowSession[interactionId] as ChatUtil).IsTextURLEnabled = false;
                                    (_chatDataContext.MainWindowSession[interactionId] as ChatUtil).IsButtonSendEnabled = false;
                                    (_chatDataContext.MainWindowSession[interactionId] as ChatUtil).IsButtonCheckURL = false;
                                    (_chatDataContext.MainWindowSession[interactionId] as ChatUtil).IsButtonAvailableURL = false;
                                    (_chatDataContext.MainWindowSession[interactionId] as ChatUtil).IsButtonPushURLExpander = false;
                                    (_chatDataContext.MainWindowSession[interactionId] as ChatUtil).IsConversationRTBEnabled = false;
                                    (_chatDataContext.MainWindowSession[interactionId] as ChatUtil).ChatAgentsStatusRowHight = new GridLength(0);
                                    (_chatDataContext.MainWindowSession[interactionId] as ChatUtil).ErrorRowHeight = GridLength.Auto;
                                    (_chatDataContext.MainWindowSession[interactionId] as ChatUtil).ErrorMessage = eventError.Description.Text.ToString();
                                    ChatDataContext.messageToClientChat.PluginErrorMessage(IPlugins.PluginType.Chat, eventError.Description.Text.ToString());
                                    ChatDataContext.messageToClientChat.PluginInteractionStatus(IPlugins.PluginType.Chat, Pointel.Interactions.IPlugins.IXNState.Closed);
                                }
                                break;
                        }
                    }
                    catch (Exception generalException)
                    {
                        logger.Error("Error occurred while do Chat_Session()" + generalException.ToString());
                    }
                    finally
                    {
                        chatMedia = null;
                    }
                }));
        }


        /// <summary>
        /// Chats the conversation binding.
        /// </summary>
        /// <param name="interactionId">The interaction identifier.</param>
        /// <param name="chatUserType">Type of the chat user.</param>
        /// <param name="userNameWithTimeShift">The user name with time shift.</param>
        /// <param name="infoType">Type of the information.</param>
        /// <param name="messageText">The message text.</param>
        /// <param name="noticeText">The notice text.</param>
        private void chatConversationBinding(string interactionId, ChatDataContext.ChatUsertype chatUserType, string userNameWithTimeShift, InfoType infoType, string messageText, string noticeText)
        {
            Paragraph normalMessageParagraph = new Paragraph();
            Paragraph urlMessageParagraph = new Paragraph();
            Run commonRun = null;
            Application.Current.Dispatcher.Invoke((System.Action)(delegate
            {
                try
                {
                    if (!_chatDataContext.MainWindowSession.ContainsKey(interactionId)) return;
                    switch (infoType)
                    {
                        case InfoType.MessageInfo:
                            switch (chatUserType)
                            {
                                case ChatDataContext.ChatUsertype.Agent:
                                    if (_chatDataContext.AgentPromptColor.ToString().Contains("#"))
                                    {
                                        commonRun = new Run(userNameWithTimeShift);
                                        commonRun.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom(_chatDataContext.AgentPromptColor.Name.ToString()));
                                    }
                                    else
                                    {
                                        commonRun = new Run(userNameWithTimeShift);
                                        commonRun.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(_chatDataContext.AgentPromptColor.A, _chatDataContext.AgentPromptColor.R, _chatDataContext.AgentPromptColor.G, _chatDataContext.AgentPromptColor.B));
                                    }
                                    if (_chatDataContext.AgentTextColor.ToString().Contains("#"))
                                    {

                                        normalMessageParagraph.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom(_chatDataContext.AgentTextColor.Name.ToString()));
                                        normalMessageParagraph.Inlines.Add(new Bold(commonRun));
                                        if (messageText.Contains("<FlowDocument"))
                                        {
                                            var flowDoc = (FlowDocument)XamlReader.Parse(messageText);
                                            if (flowDoc != null)
                                            {
                                                List<Block> flowDocumentBlocks = new List<Block>(flowDoc.Blocks);
                                                foreach (Block aBlock in flowDocumentBlocks)
                                                {
                                                    Paragraph paragraph = aBlock as Paragraph;
                                                    if (paragraph.Inlines.Count > 0)
                                                    {
                                                        foreach (Inline inline in paragraph.Inlines)
                                                        {
                                                            Run run = inline as Run;
                                                            if (run != null)
                                                            {
                                                                normalMessageParagraph.Inlines.Add(run);
                                                                break;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        else if (isAllDigits(messageText))
                                        {
                                            Hyperlink hl = dynamicHyperlinkforDial(messageText, interactionId);
                                            normalMessageParagraph.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom(_chatDataContext.AgentTextColor.Name.ToString()));
                                            normalMessageParagraph.Inlines.Add(new Bold(commonRun));
                                            normalMessageParagraph.Inlines.Add(hl);
                                        }
                                        else
                                        {
                                            normalMessageParagraph.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom(_chatDataContext.AgentTextColor.Name.ToString()));
                                            normalMessageParagraph.Inlines.Add(new Bold(commonRun));
                                            normalMessageParagraph.Inlines.Add(messageText);
                                        }
                                    }
                                    else
                                    {
                                        normalMessageParagraph.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom(_chatDataContext.AgentTextColor.Name.ToString()));
                                        normalMessageParagraph.Inlines.Add(new Bold(commonRun));
                                        if (messageText.Contains("<FlowDocument"))
                                        {
                                            var flowDoc = (FlowDocument)XamlReader.Parse(messageText);
                                            if (flowDoc != null)
                                            {
                                                List<Block> flowDocumentBlocks = new List<Block>(flowDoc.Blocks);
                                                foreach (Block aBlock in flowDocumentBlocks)
                                                {
                                                    Paragraph paragraph = aBlock as Paragraph;
                                                    if (paragraph.Inlines.Count > 0)
                                                    {
                                                        foreach (Inline inline in paragraph.Inlines)
                                                        {
                                                            Run run = inline as Run;
                                                            if (run != null)
                                                            {
                                                                normalMessageParagraph.Inlines.Add(run);
                                                                break;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        else if (isAllDigits(messageText))
                                        {
                                            Hyperlink hl = dynamicHyperlinkforDial(messageText, interactionId);
                                            normalMessageParagraph.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(_chatDataContext.AgentTextColor.A, _chatDataContext.AgentTextColor.R, _chatDataContext.AgentTextColor.G, _chatDataContext.AgentTextColor.B));
                                            normalMessageParagraph.Inlines.Add(new Bold(commonRun));
                                            normalMessageParagraph.Inlines.Add(hl);
                                        }
                                        else
                                        {
                                            normalMessageParagraph.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(_chatDataContext.AgentTextColor.A, _chatDataContext.AgentTextColor.R, _chatDataContext.AgentTextColor.G, _chatDataContext.AgentTextColor.B));
                                            normalMessageParagraph.Inlines.Add(new Bold(commonRun));
                                            normalMessageParagraph.Inlines.Add(messageText);
                                        }
                                    }
                                    break;
                                case ChatDataContext.ChatUsertype.Client:
                                    if (_chatDataContext.ClientPromptColor.ToString().Contains("#"))
                                    {
                                        commonRun = new Run(userNameWithTimeShift);
                                        commonRun.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom(_chatDataContext.ClientPromptColor.Name.ToString()));
                                    }
                                    else
                                    {
                                        commonRun = new Run(userNameWithTimeShift);
                                        commonRun.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(_chatDataContext.ClientPromptColor.A, _chatDataContext.ClientPromptColor.R, _chatDataContext.ClientPromptColor.G, _chatDataContext.ClientPromptColor.B));
                                    }
                                    if (_chatDataContext.ClientTextColor.ToString().Contains("#"))
                                    {
                                        normalMessageParagraph.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom(_chatDataContext.ClientTextColor.Name.ToString()));
                                        normalMessageParagraph.Inlines.Add(new Bold(commonRun));
                                        if (isAllDigits(messageText))
                                        {
                                            Hyperlink hl = dynamicHyperlinkforDial(messageText, interactionId);
                                            normalMessageParagraph.Inlines.Add(hl);
                                        }
                                        else
                                            normalMessageParagraph.Inlines.Add(messageText);
                                    }
                                    else
                                    {
                                        normalMessageParagraph.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(_chatDataContext.ClientTextColor.A, _chatDataContext.ClientTextColor.R, _chatDataContext.ClientTextColor.G, _chatDataContext.ClientTextColor.B));
                                        normalMessageParagraph.Inlines.Add(new Bold(commonRun));
                                        if (isAllDigits(messageText))
                                        {
                                            Hyperlink hl = dynamicHyperlinkforDial(messageText, interactionId);
                                            normalMessageParagraph.Inlines.Add(hl);
                                        }
                                        else
                                            normalMessageParagraph.Inlines.Add(messageText);
                                    }
                                    break;
                                case ChatDataContext.ChatUsertype.OtherAgent:
                                    if (_chatDataContext.OtherAgentPromptColor.ToString().Contains("#"))
                                    {
                                        commonRun = new Run(userNameWithTimeShift);
                                        commonRun.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom(_chatDataContext.OtherAgentPromptColor.Name.ToString()));
                                    }
                                    else
                                    {
                                        commonRun = new Run(userNameWithTimeShift);
                                        commonRun.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(_chatDataContext.OtherAgentPromptColor.A, _chatDataContext.OtherAgentPromptColor.R, _chatDataContext.OtherAgentPromptColor.G, _chatDataContext.OtherAgentPromptColor.B));
                                    }
                                    if (_chatDataContext.OtherAgentTextColor.ToString().Contains("#"))
                                    {
                                        normalMessageParagraph.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom(_chatDataContext.OtherAgentTextColor.Name.ToString()));
                                        normalMessageParagraph.Inlines.Add(new Bold(commonRun));
                                        if (isAllDigits(messageText))
                                        {
                                            Hyperlink hl = dynamicHyperlinkforDial(messageText, interactionId);
                                            normalMessageParagraph.Inlines.Add(hl);
                                        }
                                        else
                                            normalMessageParagraph.Inlines.Add(messageText);
                                    }
                                    else
                                    {
                                        normalMessageParagraph.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(_chatDataContext.OtherAgentTextColor.A, _chatDataContext.OtherAgentTextColor.R, _chatDataContext.OtherAgentTextColor.G, _chatDataContext.OtherAgentTextColor.B));
                                        normalMessageParagraph.Inlines.Add(new Bold(commonRun));
                                        if (isAllDigits(messageText))
                                        {
                                            Hyperlink hl = dynamicHyperlinkforDial(messageText, interactionId);
                                            normalMessageParagraph.Inlines.Add(hl);
                                        }
                                        else
                                            normalMessageParagraph.Inlines.Add(messageText);
                                    }
                                    break;
                            }
                            if ((_chatDataContext.MainWindowSession[interactionId] as ChatUtil).RTBDocument.Blocks.Contains((_chatDataContext.MainWindowSession[interactionId] as ChatUtil).TypeNotifierParagraph))
                                (_chatDataContext.MainWindowSession[interactionId] as ChatUtil).RTBDocument.Blocks.Remove((_chatDataContext.MainWindowSession[interactionId] as ChatUtil).TypeNotifierParagraph);
                            DetectURLs(normalMessageParagraph);
                            //flowDocument.Blocks.Add(normalMessageParagraph);
                            (_chatDataContext.MainWindowSession[interactionId] as ChatUtil).RTBDocument.Blocks.Add(normalMessageParagraph);
                            break;
                        case InfoType.NewPartyInfo:
                            switch (chatUserType)
                            {
                                case ChatDataContext.ChatUsertype.Agent:
                                    if (_chatDataContext.AgentPromptColor.ToString().Contains("#"))
                                    {
                                        commonRun = new Run(userNameWithTimeShift);
                                        commonRun.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom(_chatDataContext.AgentPromptColor.Name.ToString()));
                                    }
                                    else
                                    {
                                        commonRun = new Run(userNameWithTimeShift);
                                        commonRun.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(_chatDataContext.AgentPromptColor.A, _chatDataContext.AgentPromptColor.R, _chatDataContext.AgentPromptColor.G, _chatDataContext.AgentPromptColor.B));
                                    }
                                    if (_chatDataContext.AgentTextColor.ToString().Contains("#"))
                                    {
                                        normalMessageParagraph.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom(_chatDataContext.AgentTextColor.Name.ToString()));
                                        normalMessageParagraph.Inlines.Add(new Bold(commonRun));
                                        normalMessageParagraph.Inlines.Add(messageText);
                                    }
                                    else
                                    {
                                        normalMessageParagraph.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(_chatDataContext.AgentTextColor.A, _chatDataContext.AgentTextColor.R, _chatDataContext.AgentTextColor.G, _chatDataContext.AgentTextColor.B));
                                        normalMessageParagraph.Inlines.Add(new Bold(commonRun));
                                        normalMessageParagraph.Inlines.Add(messageText);
                                    }
                                    break;
                                case ChatDataContext.ChatUsertype.Client:
                                    if (_chatDataContext.ClientPromptColor.ToString().Contains("#"))
                                    {
                                        commonRun = new Run(userNameWithTimeShift);
                                        commonRun.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom(_chatDataContext.ClientPromptColor.Name.ToString()));
                                    }
                                    else
                                    {
                                        commonRun = new Run(userNameWithTimeShift);
                                        commonRun.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(_chatDataContext.ClientPromptColor.A, _chatDataContext.ClientPromptColor.R, _chatDataContext.ClientPromptColor.G, _chatDataContext.ClientPromptColor.B));
                                    }
                                    if (_chatDataContext.ClientTextColor.ToString().Contains("#"))
                                    {
                                        normalMessageParagraph.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom(_chatDataContext.ClientTextColor.Name.ToString()));
                                        normalMessageParagraph.Inlines.Add(new Bold(commonRun));
                                        normalMessageParagraph.Inlines.Add(messageText);
                                    }
                                    else
                                    {
                                        normalMessageParagraph.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(_chatDataContext.ClientTextColor.A, _chatDataContext.ClientTextColor.R, _chatDataContext.ClientTextColor.G, _chatDataContext.ClientTextColor.B));
                                        normalMessageParagraph.Inlines.Add(new Bold(commonRun));
                                        normalMessageParagraph.Inlines.Add(messageText);
                                    }
                                    break;
                                case ChatDataContext.ChatUsertype.OtherAgent:
                                    if (_chatDataContext.OtherAgentPromptColor.ToString().Contains("#"))
                                    {
                                        commonRun = new Run(userNameWithTimeShift);
                                        commonRun.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom(_chatDataContext.OtherAgentPromptColor.Name.ToString()));
                                    }
                                    else
                                    {
                                        commonRun = new Run(userNameWithTimeShift);
                                        commonRun.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(_chatDataContext.OtherAgentPromptColor.A, _chatDataContext.OtherAgentPromptColor.R, _chatDataContext.OtherAgentPromptColor.G, _chatDataContext.OtherAgentPromptColor.B));
                                    }
                                    if (_chatDataContext.OtherAgentTextColor.ToString().Contains("#"))
                                    {
                                        normalMessageParagraph.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom(_chatDataContext.OtherAgentTextColor.Name.ToString()));
                                        normalMessageParagraph.Inlines.Add(new Bold(commonRun));
                                        normalMessageParagraph.Inlines.Add(messageText);
                                    }
                                    else
                                    {
                                        normalMessageParagraph.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(_chatDataContext.OtherAgentTextColor.A, _chatDataContext.OtherAgentTextColor.R, _chatDataContext.OtherAgentTextColor.G, _chatDataContext.OtherAgentTextColor.B));
                                        normalMessageParagraph.Inlines.Add(new Bold(commonRun));
                                        normalMessageParagraph.Inlines.Add(messageText);
                                    }
                                    break;
                            }
                            DetectURLs(normalMessageParagraph);
                            //flowDocument.Blocks.Add(normalMessageParagraph);
                            (_chatDataContext.MainWindowSession[interactionId] as ChatUtil).RTBDocument.Blocks.Add(normalMessageParagraph);

                            break;
                        case InfoType.NoticeInfo:
                            switch (chatUserType)
                            {
                                case ChatDataContext.ChatUsertype.Agent:
                                    if (noticeText == string.Empty && messageText == string.Empty)
                                    {

                                    }
                                    if (noticeText == string.Empty && messageText != string.Empty)
                                    {
                                        if (_chatDataContext.AgentPromptColor.ToString().Contains("#"))
                                        {
                                            commonRun = new Run(userNameWithTimeShift);
                                            commonRun.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom(_chatDataContext.AgentPromptColor.Name.ToString()));
                                        }
                                        else
                                        {
                                            commonRun = new Run(userNameWithTimeShift);
                                            commonRun.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(_chatDataContext.AgentPromptColor.A, _chatDataContext.AgentPromptColor.R, _chatDataContext.AgentPromptColor.G, _chatDataContext.AgentPromptColor.B));
                                        }
                                        if (_chatDataContext.AgentTextColor.ToString().Contains("#"))
                                        {
                                            normalMessageParagraph.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom(_chatDataContext.AgentTextColor.Name.ToString()));
                                            normalMessageParagraph.Inlines.Add(new Bold(commonRun));
                                            normalMessageParagraph.Inlines.Add(messageText);
                                        }
                                        else
                                        {
                                            normalMessageParagraph.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(_chatDataContext.AgentTextColor.A, _chatDataContext.AgentTextColor.R, _chatDataContext.AgentTextColor.G, _chatDataContext.AgentTextColor.B));
                                            normalMessageParagraph.Inlines.Add(new Bold(commonRun));
                                            normalMessageParagraph.Inlines.Add(messageText);
                                        }
                                        DetectURLs(normalMessageParagraph);
                                        //flowDocument.Blocks.Add(normalMessageParagraph);
                                        (_chatDataContext.MainWindowSession[interactionId] as ChatUtil).RTBDocument.Blocks.Add(normalMessageParagraph);

                                    }
                                    else if (noticeText != string.Empty && messageText != string.Empty)
                                    {
                                        if (_chatDataContext.AgentPromptColor.ToString().Contains("#"))
                                        {
                                            commonRun = new Run(userNameWithTimeShift);
                                            commonRun.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom(_chatDataContext.AgentPromptColor.Name.ToString()));
                                        }
                                        else
                                        {
                                            commonRun = new Run(userNameWithTimeShift);
                                            commonRun.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(_chatDataContext.AgentPromptColor.A, _chatDataContext.AgentPromptColor.R, _chatDataContext.AgentPromptColor.G, _chatDataContext.AgentPromptColor.B));
                                        }
                                        if (_chatDataContext.AgentTextColor.ToString().Contains("#"))
                                        {
                                            urlMessageParagraph.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom(_chatDataContext.AgentTextColor.Name.ToString()));
                                        }
                                        else
                                        {
                                            urlMessageParagraph.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(_chatDataContext.AgentTextColor.A, _chatDataContext.AgentTextColor.R, _chatDataContext.AgentTextColor.G, _chatDataContext.AgentTextColor.B));
                                        }
                                        urlMessageParagraph.Inlines.Add(new Bold(commonRun));
                                        urlMessageParagraph.Inlines.Add(messageText);
                                        Run temprun = null;
                                        var urlkey = _chatDataContext.LoadAvailablePushURL.FirstOrDefault(x => x.Value.Contains(noticeText)).Key;
                                        if (string.IsNullOrEmpty(urlkey))
                                        {
                                            if (noticeText.Contains("http://"))
                                            {
                                                string originalURL = noticeText.Replace("http://", "");
                                                var originalUrlKey = _chatDataContext.LoadAvailablePushURL.FirstOrDefault(x => x.Value.Contains(originalURL.ToString())).Key;
                                                if (string.IsNullOrEmpty(originalUrlKey))
                                                {
                                                    originalURL = originalURL.Replace("/", "");
                                                    var originalUrlKey1 = _chatDataContext.LoadAvailablePushURL.FirstOrDefault(x => x.Value.Contains(originalURL.ToString())).Key;
                                                    if (string.IsNullOrEmpty(originalUrlKey1))
                                                        temprun = new Run("Click Here");
                                                    else
                                                        temprun = new Run(originalUrlKey1);
                                                }
                                                else
                                                    temprun = new Run(originalUrlKey);
                                                originalURL = null;
                                            }
                                            else
                                                temprun = new Run("Click Here");
                                        }
                                        else
                                            temprun = new Run(urlkey.ToString());
                                        Hyperlink hl = new Hyperlink(temprun);
                                        hl.Foreground = Brushes.Blue;
                                        hl.NavigateUri = new Uri(noticeText);
                                        hl.ToolTip = dynamicToolTip("Pushed URL", noticeText);
                                        urlMessageParagraph.Inlines.Add(hl);
                                        (_chatDataContext.MainWindowSession[interactionId] as ChatUtil).RTBDocument.Blocks.Add(urlMessageParagraph);
                                    }
                                    break;
                                case ChatDataContext.ChatUsertype.Client:
                                    if (noticeText == string.Empty && messageText == string.Empty)
                                    {
                                        (_chatDataContext.MainWindowSession[interactionId] as ChatUtil).TypeNotifierParagraph.Inlines.Clear();
                                        (_chatDataContext.MainWindowSession[interactionId] as ChatUtil).TypeNotifierParagraph.FontStyle = FontStyles.Italic;
                                        (_chatDataContext.MainWindowSession[interactionId] as ChatUtil).TypeNotifierParagraph.Inlines.Add(new Run(userNameWithTimeShift));
                                        (_chatDataContext.MainWindowSession[interactionId] as ChatUtil).TypeNotifierParagraph.Foreground = Brushes.LightGray;
                                        //flowDocument.Blocks.Add(typeNotifierParagraph);
                                        (_chatDataContext.MainWindowSession[interactionId] as ChatUtil).RTBDocument.Blocks.Add((_chatDataContext.MainWindowSession[interactionId] as ChatUtil).TypeNotifierParagraph);

                                    }
                                    if (noticeText == string.Empty && messageText != string.Empty)
                                    {
                                        if (_chatDataContext.ClientPromptColor.ToString().Contains("#"))
                                        {
                                            commonRun = new Run(userNameWithTimeShift);
                                            commonRun.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom(_chatDataContext.ClientPromptColor.Name.ToString()));
                                        }
                                        else
                                        {
                                            commonRun = new Run(userNameWithTimeShift);
                                            commonRun.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(_chatDataContext.ClientPromptColor.A, _chatDataContext.ClientPromptColor.R, _chatDataContext.ClientPromptColor.G, _chatDataContext.ClientPromptColor.B));
                                        }
                                        if (_chatDataContext.ClientTextColor.ToString().Contains("#"))
                                        {
                                            normalMessageParagraph.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom(_chatDataContext.ClientTextColor.Name.ToString()));
                                            normalMessageParagraph.Inlines.Add(new Bold(commonRun));
                                            normalMessageParagraph.Inlines.Add(messageText);
                                        }
                                        else
                                        {
                                            normalMessageParagraph.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(_chatDataContext.ClientTextColor.A, _chatDataContext.ClientTextColor.R, _chatDataContext.ClientTextColor.G, _chatDataContext.ClientTextColor.B));
                                            normalMessageParagraph.Inlines.Add(new Bold(commonRun));
                                            normalMessageParagraph.Inlines.Add(messageText);
                                        }
                                        DetectURLs(normalMessageParagraph);
                                        //flowDocument.Blocks.Add(normalMessageParagraph);
                                        (_chatDataContext.MainWindowSession[interactionId] as ChatUtil).RTBDocument.Blocks.Add(normalMessageParagraph);

                                    }
                                    else if (noticeText != string.Empty && messageText != string.Empty)
                                    {
                                        if (_chatDataContext.ClientPromptColor.ToString().Contains("#"))
                                        {
                                            commonRun = new Run(userNameWithTimeShift);
                                            commonRun.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom(_chatDataContext.ClientPromptColor.Name.ToString()));
                                        }
                                        else
                                        {
                                            commonRun = new Run(userNameWithTimeShift);
                                            commonRun.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(_chatDataContext.ClientPromptColor.A, _chatDataContext.ClientPromptColor.R, _chatDataContext.ClientPromptColor.G, _chatDataContext.ClientPromptColor.B));
                                        }
                                        if (_chatDataContext.ClientTextColor.ToString().Contains("#"))
                                        {
                                            urlMessageParagraph.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom(_chatDataContext.ClientTextColor.Name.ToString()));
                                        }
                                        else
                                        {
                                            urlMessageParagraph.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(_chatDataContext.ClientTextColor.A, _chatDataContext.ClientTextColor.R, _chatDataContext.ClientTextColor.G, _chatDataContext.ClientTextColor.B));
                                        }
                                        urlMessageParagraph.Inlines.Add(new Bold(commonRun));
                                        urlMessageParagraph.Inlines.Add(messageText);
                                        if (_chatDataContext.PushedURLKey != string.Empty)
                                        {
                                            Hyperlink hl = new Hyperlink(new Run(_chatDataContext.PushedURLKey));
                                            hl.Foreground = Brushes.Blue;
                                            hl.NavigateUri = new Uri(noticeText);
                                            urlMessageParagraph.Inlines.Add(hl);
                                        }
                                        else
                                        {
                                            Run temprun = null;
                                            var urlkey = _chatDataContext.LoadAvailablePushURL.FirstOrDefault(x => x.Value.Contains(noticeText)).Key;
                                            if (string.IsNullOrEmpty(urlkey))
                                            {
                                                if (noticeText.Contains("http://"))
                                                {
                                                    string originalURL = noticeText.Replace("http://", "");
                                                    var originalUrlKey = _chatDataContext.LoadAvailablePushURL.FirstOrDefault(x => x.Value.Contains(originalURL.ToString())).Key;
                                                    if (string.IsNullOrEmpty(originalUrlKey))
                                                    {
                                                        originalURL = originalURL.Replace("/", "");
                                                        var originalUrlKey1 = _chatDataContext.LoadAvailablePushURL.FirstOrDefault(x => x.Value.Contains(originalURL.ToString())).Key;
                                                        if (string.IsNullOrEmpty(originalUrlKey1))
                                                            temprun = new Run("Click Here");
                                                        else
                                                            temprun = new Run(originalUrlKey1);
                                                    }
                                                    else
                                                        temprun = new Run(originalUrlKey);
                                                    originalURL = null;
                                                }
                                                else
                                                    temprun = new Run("Click Here");
                                            }
                                            else
                                                temprun = new Run(urlkey.ToString());
                                            Hyperlink hl = new Hyperlink(temprun);
                                            hl.Foreground = Brushes.Blue;
                                            hl.NavigateUri = new Uri(noticeText);
                                            hl.ToolTip = dynamicToolTip("Pushed URL", noticeText);
                                            urlMessageParagraph.Inlines.Add(hl);
                                        }
                                        (_chatDataContext.MainWindowSession[interactionId] as ChatUtil).RTBDocument.Blocks.Add(urlMessageParagraph);

                                    }
                                    break;
                                case ChatDataContext.ChatUsertype.OtherAgent:
                                    if (noticeText == string.Empty && messageText == string.Empty)
                                    {

                                    }
                                    if (noticeText == string.Empty && messageText != string.Empty)
                                    {
                                        if (_chatDataContext.OtherAgentPromptColor.ToString().Contains("#"))
                                        {
                                            commonRun = new Run(userNameWithTimeShift);
                                            commonRun.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom(_chatDataContext.OtherAgentPromptColor.Name.ToString()));
                                        }
                                        else
                                        {
                                            commonRun = new Run(userNameWithTimeShift);
                                            commonRun.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(_chatDataContext.OtherAgentPromptColor.A, _chatDataContext.OtherAgentPromptColor.R, _chatDataContext.OtherAgentPromptColor.G, _chatDataContext.OtherAgentPromptColor.B));
                                        }
                                        if (_chatDataContext.OtherAgentTextColor.ToString().Contains("#"))
                                        {
                                            normalMessageParagraph.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom(_chatDataContext.OtherAgentTextColor.Name.ToString()));
                                            normalMessageParagraph.Inlines.Add(new Bold(commonRun));
                                            normalMessageParagraph.Inlines.Add(messageText);
                                        }
                                        else
                                        {
                                            normalMessageParagraph.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(_chatDataContext.OtherAgentTextColor.A, _chatDataContext.OtherAgentTextColor.R, _chatDataContext.OtherAgentTextColor.G, _chatDataContext.OtherAgentTextColor.B));
                                            normalMessageParagraph.Inlines.Add(new Bold(commonRun));
                                            normalMessageParagraph.Inlines.Add(messageText);
                                        }
                                        DetectURLs(normalMessageParagraph);
                                        //flowDocument.Blocks.Add(normalMessageParagraph);
                                        (_chatDataContext.MainWindowSession[interactionId] as ChatUtil).RTBDocument.Blocks.Add(normalMessageParagraph);

                                    }
                                    else if (noticeText != string.Empty && messageText != string.Empty)
                                    {
                                        if (_chatDataContext.OtherAgentPromptColor.ToString().Contains("#"))
                                        {
                                            commonRun = new Run(userNameWithTimeShift);
                                            commonRun.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom(_chatDataContext.OtherAgentPromptColor.Name.ToString()));
                                        }
                                        else
                                        {
                                            commonRun = new Run(userNameWithTimeShift);
                                            commonRun.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(_chatDataContext.OtherAgentPromptColor.A, _chatDataContext.OtherAgentPromptColor.R, _chatDataContext.OtherAgentPromptColor.G, _chatDataContext.OtherAgentPromptColor.B));
                                        }
                                        if (_chatDataContext.OtherAgentTextColor.ToString().Contains("#"))
                                        {
                                            urlMessageParagraph.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom(_chatDataContext.OtherAgentTextColor.Name.ToString()));
                                        }
                                        else
                                        {
                                            urlMessageParagraph.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(_chatDataContext.OtherAgentTextColor.A, _chatDataContext.OtherAgentTextColor.R, _chatDataContext.OtherAgentTextColor.G, _chatDataContext.OtherAgentTextColor.B));
                                        }
                                        urlMessageParagraph.Inlines.Add(new Bold(commonRun));
                                        urlMessageParagraph.Inlines.Add(messageText);
                                        Run temprun = null;
                                        var urlkey = _chatDataContext.LoadAvailablePushURL.FirstOrDefault(x => x.Value.Contains(noticeText)).Key;
                                        if (string.IsNullOrEmpty(urlkey))
                                        {
                                            if (noticeText.Contains("http://"))
                                            {
                                                string originalURL = noticeText.Replace("http://", "");
                                                var originalUrlKey = _chatDataContext.LoadAvailablePushURL.FirstOrDefault(x => x.Value.Contains(originalURL.ToString())).Key;
                                                if (string.IsNullOrEmpty(originalUrlKey))
                                                {
                                                    originalURL = originalURL.Replace("/", "");
                                                    var originalUrlKey1 = _chatDataContext.LoadAvailablePushURL.FirstOrDefault(x => x.Value.Contains(originalURL.ToString())).Key;
                                                    if (string.IsNullOrEmpty(originalUrlKey1))
                                                        temprun = new Run("Click Here");
                                                    else
                                                        temprun = new Run(originalUrlKey1);
                                                }
                                                else
                                                    temprun = new Run(originalUrlKey);
                                                originalURL = null;
                                            }
                                            else
                                                temprun = new Run("Click Here");
                                        }
                                        else
                                            temprun = new Run(urlkey.ToString());
                                        Hyperlink hl = new Hyperlink(temprun);
                                        hl.Foreground = Brushes.Blue;
                                        hl.NavigateUri = new Uri(noticeText);
                                        hl.ToolTip = dynamicToolTip("Pushed URL", noticeText);
                                        urlMessageParagraph.Inlines.Add(hl);
                                        (_chatDataContext.MainWindowSession[interactionId] as ChatUtil).RTBDocument.Blocks.Add(urlMessageParagraph);

                                    }
                                    break;
                            }
                            break;
                        case InfoType.PartyLeftInfo:
                            switch (chatUserType)
                            {
                                case ChatDataContext.ChatUsertype.Agent:
                                    if (_chatDataContext.AgentPromptColor.ToString().Contains("#"))
                                    {
                                        commonRun = new Run(userNameWithTimeShift);
                                        commonRun.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom(_chatDataContext.AgentPromptColor.Name.ToString()));
                                    }
                                    else
                                    {
                                        commonRun = new Run(userNameWithTimeShift);
                                        commonRun.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(_chatDataContext.AgentPromptColor.A, _chatDataContext.AgentPromptColor.R, _chatDataContext.AgentPromptColor.G, _chatDataContext.AgentPromptColor.B));
                                    }
                                    if (_chatDataContext.AgentTextColor.ToString().Contains("#"))
                                    {
                                        normalMessageParagraph.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom(_chatDataContext.AgentTextColor.Name.ToString()));
                                        normalMessageParagraph.Inlines.Add(new Bold(commonRun));
                                        normalMessageParagraph.Inlines.Add(messageText);
                                    }
                                    else
                                    {
                                        normalMessageParagraph.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(_chatDataContext.AgentTextColor.A, _chatDataContext.AgentTextColor.R, _chatDataContext.AgentTextColor.G, _chatDataContext.AgentTextColor.B));
                                        normalMessageParagraph.Inlines.Add(new Bold(commonRun));
                                        normalMessageParagraph.Inlines.Add(messageText);
                                    }
                                    break;
                                case ChatDataContext.ChatUsertype.Client:
                                    if (_chatDataContext.ClientPromptColor.ToString().Contains("#"))
                                    {
                                        commonRun = new Run(userNameWithTimeShift);
                                        commonRun.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom(_chatDataContext.ClientPromptColor.Name.ToString()));
                                    }
                                    else
                                    {
                                        commonRun = new Run(userNameWithTimeShift);
                                        commonRun.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(_chatDataContext.ClientPromptColor.A, _chatDataContext.ClientPromptColor.R, _chatDataContext.ClientPromptColor.G, _chatDataContext.ClientPromptColor.B));
                                    }
                                    if (_chatDataContext.ClientTextColor.ToString().Contains("#"))
                                    {
                                        normalMessageParagraph.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom(_chatDataContext.ClientTextColor.Name.ToString()));
                                        normalMessageParagraph.Inlines.Add(new Bold(commonRun));
                                        normalMessageParagraph.Inlines.Add(messageText);
                                    }
                                    else
                                    {
                                        normalMessageParagraph.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(_chatDataContext.ClientTextColor.A, _chatDataContext.ClientTextColor.R, _chatDataContext.ClientTextColor.G, _chatDataContext.ClientTextColor.B));
                                        normalMessageParagraph.Inlines.Add(new Bold(commonRun));
                                        normalMessageParagraph.Inlines.Add(messageText);
                                    }
                                    break;
                                case ChatDataContext.ChatUsertype.OtherAgent:
                                    if (_chatDataContext.OtherAgentPromptColor.ToString().Contains("#"))
                                    {
                                        commonRun = new Run(userNameWithTimeShift);
                                        commonRun.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom(_chatDataContext.OtherAgentPromptColor.Name.ToString()));
                                    }
                                    else
                                    {
                                        commonRun = new Run(userNameWithTimeShift);
                                        commonRun.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(_chatDataContext.OtherAgentPromptColor.A, _chatDataContext.OtherAgentPromptColor.R, _chatDataContext.OtherAgentPromptColor.G, _chatDataContext.OtherAgentPromptColor.B));
                                    }
                                    if (_chatDataContext.OtherAgentTextColor.ToString().Contains("#"))
                                    {
                                        normalMessageParagraph.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom(_chatDataContext.OtherAgentTextColor.Name.ToString()));
                                        normalMessageParagraph.Inlines.Add(new Bold(commonRun));
                                        normalMessageParagraph.Inlines.Add(messageText);
                                    }
                                    else
                                    {
                                        normalMessageParagraph.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(_chatDataContext.OtherAgentTextColor.A, _chatDataContext.OtherAgentTextColor.R, _chatDataContext.OtherAgentTextColor.G, _chatDataContext.OtherAgentTextColor.B));
                                        normalMessageParagraph.Inlines.Add(new Bold(commonRun));
                                        normalMessageParagraph.Inlines.Add(messageText);
                                    }
                                    break;
                            }
                            DetectURLs(normalMessageParagraph);
                            //flowDocument.Blocks.Add(normalMessageParagraph);
                            (_chatDataContext.MainWindowSession[interactionId] as ChatUtil).RTBDocument.Blocks.Add(normalMessageParagraph);
                            break;
                    }
                }
                catch (Exception generalException)
                {
                    logger.Error("Error occurred while do chatConversationBinding()" + generalException.ToString());
                }
            }));
        }

        private ToolTip dynamicToolTip(string header, string content)
        {
            ToolTip toolTip = null;
            try
            {
                toolTip = new ToolTip();
                StackPanel sPanel = new StackPanel();
                sPanel.Background = Brushes.White;
                sPanel.Orientation = Orientation.Vertical;
                sPanel.Margin = new Thickness(2);
                toolTip.Background = Brushes.White;
                toolTip.BorderBrush = (System.Windows.Media.Brush)(new BrushConverter().ConvertFromString("#ADAAAD"));
                toolTip.BorderThickness = new Thickness(0.5);
                TextBlock txtBlockHead = new TextBlock();
                txtBlockHead.Text = header;
                txtBlockHead.FontWeight = FontWeights.Bold;
                txtBlockHead.FontFamily = new FontFamily("Calibri");
                TextBlock txtBlockContent = new TextBlock();
                txtBlockContent.Text = content;
                txtBlockContent.FontWeight = FontWeights.Normal;
                txtBlockContent.FontFamily = new FontFamily("Calibri");
                sPanel.Children.Add(txtBlockHead);
                sPanel.Children.Add(txtBlockContent);
                toolTip.Content = sPanel;
                sPanel = null;
                txtBlockContent = null;
                txtBlockHead = null;
            }
            catch (Exception generalException)
            {
                logger.Warn("Error Occurred while dynamicToolTip() + " + generalException.ToString());
                toolTip = null;
            }
            return toolTip;
        }

        void hl_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var data = sender as Hyperlink;
                if (data != null)
                {
                    Run run = (Run)data.Inlines.FirstInline;
                    string num = run.Text.ToString();
                    if (ChatDataContext.messageToClientChat != null && ChatDataContext.GetInstance().IsAvailableVoiceMedia)
                    {
                        ChatDataContext.messageToClientChat.PluginDialEvents(Interactions.IPlugins.PluginType.Chat, num);
                        ChatDataContext.messageToClientChat.PluginDialEvents(Interactions.IPlugins.PluginType.Chat, "DIAL");
                    }
                    else
                    {
                        (_chatDataContext.MainWindowSession[data.Tag.ToString()] as ChatUtil).ErrorRowHeight = GridLength.Auto;
                        (_chatDataContext.MainWindowSession[data.Tag.ToString()] as ChatUtil).ErrorMessage = "Voice media is not available to make interaction.";
                    }
                }
            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred while do hl_Click()" + generalException.ToString());
            }
        }

        private Hyperlink dynamicHyperlinkforDial(string messageText, string interactionId)
        {
            Run temprun = null;
            Hyperlink hl = null;
            try
            {
                temprun = new Run(messageText);
                hl = new Hyperlink(temprun);
                hl.Foreground = Brushes.Blue;
                hl.Tag = interactionId;
                hl.ToolTip = dynamicToolTip("Dial", "Click to Call " + messageText);
                hl.Click += new RoutedEventHandler(hl_Click);
            }
            catch (Exception generalException)
            {
                logger.Error("Error Occurred in dynamicHyperlinkforDial() : " + generalException.ToString());
            }
            return hl;
        }

        /// <summary>
        /// Chats the consult conversation binding.
        /// </summary>
        /// <param name="chatUserType">Type of the chat user.</param>
        /// <param name="userNameWithTimeShift">The user name with time shift.</param>
        /// <param name="infoType">Type of the information.</param>
        /// <param name="messageText">The message text.</param>
        /// <param name="noticeText">The notice text.</param>
        private void chatConsultConversationBinding(string interactionId, ChatDataContext.ChatUsertype chatUserType, string userNameWithTimeShift, InfoType infoType, string messageText, string noticeText)
        {
            Paragraph normalMessageParagraph = new Paragraph();
            Paragraph urlMessageParagraph = new Paragraph();
            Run commonRun = null;
            Application.Current.Dispatcher.Invoke((System.Action)(delegate
            {

                try
                {
                    if (!_chatDataContext.MainWindowSession.ContainsKey(interactionId)) return;
                    switch (infoType)
                    {
                        case InfoType.MessageInfo:
                            switch (chatUserType)
                            {
                                case ChatDataContext.ChatUsertype.Agent:
                                    if (_chatDataContext.AgentPromptColor.ToString().Contains("#"))
                                    {
                                        commonRun = new Run(userNameWithTimeShift);
                                        commonRun.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom(_chatDataContext.AgentPromptColor.Name.ToString()));
                                    }
                                    else
                                    {
                                        commonRun = new Run(userNameWithTimeShift);
                                        commonRun.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(_chatDataContext.AgentPromptColor.A, _chatDataContext.AgentPromptColor.R, _chatDataContext.AgentPromptColor.G, _chatDataContext.AgentPromptColor.B));
                                    }
                                    if (_chatDataContext.AgentTextColor.ToString().Contains("#"))
                                    {
                                        normalMessageParagraph.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom(_chatDataContext.AgentTextColor.Name.ToString()));
                                        normalMessageParagraph.Inlines.Add(new Bold(commonRun));
                                        if (isAllDigits(messageText))
                                        {
                                            Hyperlink hl = dynamicHyperlinkforDial(messageText, interactionId);
                                            normalMessageParagraph.Inlines.Add(hl);
                                        }
                                        else
                                            normalMessageParagraph.Inlines.Add(messageText);
                                    }
                                    else
                                    {
                                        normalMessageParagraph.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(_chatDataContext.AgentTextColor.A, _chatDataContext.AgentTextColor.R, _chatDataContext.AgentTextColor.G, _chatDataContext.AgentTextColor.B));
                                        normalMessageParagraph.Inlines.Add(new Bold(commonRun));
                                        if (isAllDigits(messageText))
                                        {
                                            Hyperlink hl = dynamicHyperlinkforDial(messageText, interactionId);
                                            normalMessageParagraph.Inlines.Add(hl);
                                        }
                                        else
                                            normalMessageParagraph.Inlines.Add(messageText);
                                    }
                                    break;
                                case ChatDataContext.ChatUsertype.Client:
                                    if (_chatDataContext.ClientPromptColor.ToString().Contains("#"))
                                    {
                                        commonRun = new Run(userNameWithTimeShift);
                                        commonRun.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom(_chatDataContext.ClientPromptColor.Name.ToString()));
                                    }
                                    else
                                    {
                                        commonRun = new Run(userNameWithTimeShift);
                                        commonRun.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(_chatDataContext.ClientPromptColor.A, _chatDataContext.ClientPromptColor.R, _chatDataContext.ClientPromptColor.G, _chatDataContext.ClientPromptColor.B));
                                    }
                                    if (_chatDataContext.ClientTextColor.ToString().Contains("#"))
                                    {
                                        normalMessageParagraph.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom(_chatDataContext.ClientTextColor.Name.ToString()));
                                        normalMessageParagraph.Inlines.Add(new Bold(commonRun));
                                        if (isAllDigits(messageText))
                                        {
                                            Hyperlink hl = dynamicHyperlinkforDial(messageText, interactionId);
                                            normalMessageParagraph.Inlines.Add(hl);
                                        }
                                        else
                                            normalMessageParagraph.Inlines.Add(messageText);
                                    }
                                    else
                                    {
                                        normalMessageParagraph.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(_chatDataContext.ClientTextColor.A, _chatDataContext.ClientTextColor.R, _chatDataContext.ClientTextColor.G, _chatDataContext.ClientTextColor.B));
                                        normalMessageParagraph.Inlines.Add(new Bold(commonRun));
                                        if (isAllDigits(messageText))
                                        {
                                            Hyperlink hl = dynamicHyperlinkforDial(messageText, interactionId);
                                            normalMessageParagraph.Inlines.Add(hl);
                                        }
                                        else
                                            normalMessageParagraph.Inlines.Add(messageText);
                                    }
                                    break;
                                case ChatDataContext.ChatUsertype.OtherAgent:
                                    if (_chatDataContext.OtherAgentPromptColor.ToString().Contains("#"))
                                    {
                                        commonRun = new Run(userNameWithTimeShift);
                                        commonRun.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom(_chatDataContext.OtherAgentPromptColor.Name.ToString()));
                                    }
                                    else
                                    {
                                        commonRun = new Run(userNameWithTimeShift);
                                        commonRun.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(_chatDataContext.OtherAgentPromptColor.A, _chatDataContext.OtherAgentPromptColor.R, _chatDataContext.OtherAgentPromptColor.G, _chatDataContext.OtherAgentPromptColor.B));
                                    }
                                    if (_chatDataContext.OtherAgentTextColor.ToString().Contains("#"))
                                    {
                                        normalMessageParagraph.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom(_chatDataContext.OtherAgentTextColor.Name.ToString()));
                                        normalMessageParagraph.Inlines.Add(new Bold(commonRun));
                                        if (isAllDigits(messageText))
                                        {
                                            Hyperlink hl = dynamicHyperlinkforDial(messageText, interactionId);
                                            normalMessageParagraph.Inlines.Add(hl);
                                        }
                                        else
                                            normalMessageParagraph.Inlines.Add(messageText);
                                    }
                                    else
                                    {
                                        normalMessageParagraph.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(_chatDataContext.OtherAgentTextColor.A, _chatDataContext.OtherAgentTextColor.R, _chatDataContext.OtherAgentTextColor.G, _chatDataContext.OtherAgentTextColor.B));
                                        normalMessageParagraph.Inlines.Add(new Bold(commonRun));
                                        if (isAllDigits(messageText))
                                        {
                                            Hyperlink hl = dynamicHyperlinkforDial(messageText, interactionId);
                                            normalMessageParagraph.Inlines.Add(hl);
                                        }
                                        else
                                            normalMessageParagraph.Inlines.Add(messageText);
                                    }
                                    break;
                            }
                            DetectURLs(normalMessageParagraph);
                            //flowDocument.Blocks.Add(normalMessageParagraph);
                            (_chatDataContext.MainWindowSession[interactionId] as ChatUtil).RTBConsultDocument.Blocks.Add(normalMessageParagraph);

                            break;
                        case InfoType.NewPartyInfo:
                            switch (chatUserType)
                            {
                                case ChatDataContext.ChatUsertype.Agent:
                                    if (_chatDataContext.AgentPromptColor.ToString().Contains("#"))
                                    {
                                        commonRun = new Run(userNameWithTimeShift);
                                        commonRun.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom(_chatDataContext.AgentPromptColor.Name.ToString()));
                                    }
                                    else
                                    {
                                        commonRun = new Run(userNameWithTimeShift);
                                        commonRun.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(_chatDataContext.AgentPromptColor.A, _chatDataContext.AgentPromptColor.R, _chatDataContext.AgentPromptColor.G, _chatDataContext.AgentPromptColor.B));
                                    }
                                    if (_chatDataContext.AgentTextColor.ToString().Contains("#"))
                                    {
                                        normalMessageParagraph.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom(_chatDataContext.AgentTextColor.Name.ToString()));
                                        normalMessageParagraph.Inlines.Add(new Bold(commonRun));
                                        normalMessageParagraph.Inlines.Add(messageText);
                                    }
                                    else
                                    {
                                        normalMessageParagraph.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(_chatDataContext.AgentTextColor.A, _chatDataContext.AgentTextColor.R, _chatDataContext.AgentTextColor.G, _chatDataContext.AgentTextColor.B));
                                        normalMessageParagraph.Inlines.Add(new Bold(commonRun));
                                        normalMessageParagraph.Inlines.Add(messageText);
                                    }
                                    break;
                                case ChatDataContext.ChatUsertype.Client:
                                    if (_chatDataContext.ClientPromptColor.ToString().Contains("#"))
                                    {
                                        commonRun = new Run(userNameWithTimeShift);
                                        commonRun.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom(_chatDataContext.ClientPromptColor.Name.ToString()));
                                    }
                                    else
                                    {
                                        commonRun = new Run(userNameWithTimeShift);
                                        commonRun.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(_chatDataContext.ClientPromptColor.A, _chatDataContext.ClientPromptColor.R, _chatDataContext.ClientPromptColor.G, _chatDataContext.ClientPromptColor.B));
                                    }
                                    if (_chatDataContext.ClientTextColor.ToString().Contains("#"))
                                    {
                                        normalMessageParagraph.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom(_chatDataContext.ClientTextColor.Name.ToString()));
                                        normalMessageParagraph.Inlines.Add(new Bold(commonRun));
                                        normalMessageParagraph.Inlines.Add(messageText);
                                    }
                                    else
                                    {
                                        normalMessageParagraph.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(_chatDataContext.ClientTextColor.A, _chatDataContext.ClientTextColor.R, _chatDataContext.ClientTextColor.G, _chatDataContext.ClientTextColor.B));
                                        normalMessageParagraph.Inlines.Add(new Bold(commonRun));
                                        normalMessageParagraph.Inlines.Add(messageText);
                                    }
                                    break;
                                case ChatDataContext.ChatUsertype.OtherAgent:
                                    if (_chatDataContext.OtherAgentPromptColor.ToString().Contains("#"))
                                    {
                                        commonRun = new Run(userNameWithTimeShift);
                                        commonRun.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom(_chatDataContext.OtherAgentPromptColor.Name.ToString()));
                                    }
                                    else
                                    {
                                        commonRun = new Run(userNameWithTimeShift);
                                        commonRun.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(_chatDataContext.OtherAgentPromptColor.A, _chatDataContext.OtherAgentPromptColor.R, _chatDataContext.OtherAgentPromptColor.G, _chatDataContext.OtherAgentPromptColor.B));
                                    }
                                    if (_chatDataContext.OtherAgentTextColor.ToString().Contains("#"))
                                    {
                                        normalMessageParagraph.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom(_chatDataContext.OtherAgentTextColor.Name.ToString()));
                                        normalMessageParagraph.Inlines.Add(new Bold(commonRun));
                                        normalMessageParagraph.Inlines.Add(messageText);
                                    }
                                    else
                                    {
                                        normalMessageParagraph.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(_chatDataContext.OtherAgentTextColor.A, _chatDataContext.OtherAgentTextColor.R, _chatDataContext.OtherAgentTextColor.G, _chatDataContext.OtherAgentTextColor.B));
                                        normalMessageParagraph.Inlines.Add(new Bold(commonRun));
                                        normalMessageParagraph.Inlines.Add(messageText);
                                    }
                                    break;
                            }
                            DetectURLs(normalMessageParagraph);
                            //flowDocument.Blocks.Add(normalMessageParagraph);
                            (_chatDataContext.MainWindowSession[interactionId] as ChatUtil).RTBConsultDocument.Blocks.Add(normalMessageParagraph);

                            break;
                        case InfoType.NoticeInfo:
                            switch (chatUserType)
                            {
                                case ChatDataContext.ChatUsertype.Agent:
                                    if (noticeText == string.Empty && messageText == string.Empty)
                                    {

                                    }
                                    if (noticeText == string.Empty && messageText != string.Empty)
                                    {
                                        if (_chatDataContext.AgentPromptColor.ToString().Contains("#"))
                                        {
                                            commonRun = new Run(userNameWithTimeShift);
                                            commonRun.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom(_chatDataContext.AgentPromptColor.Name.ToString()));
                                        }
                                        else
                                        {
                                            commonRun = new Run(userNameWithTimeShift);
                                            commonRun.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(_chatDataContext.AgentPromptColor.A, _chatDataContext.AgentPromptColor.R, _chatDataContext.AgentPromptColor.G, _chatDataContext.AgentPromptColor.B));
                                        }
                                        if (_chatDataContext.AgentTextColor.ToString().Contains("#"))
                                        {
                                            normalMessageParagraph.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom(_chatDataContext.AgentTextColor.Name.ToString()));
                                            normalMessageParagraph.Inlines.Add(new Bold(commonRun));
                                            normalMessageParagraph.Inlines.Add(messageText);
                                        }
                                        else
                                        {
                                            normalMessageParagraph.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(_chatDataContext.AgentTextColor.A, _chatDataContext.AgentTextColor.R, _chatDataContext.AgentTextColor.G, _chatDataContext.AgentTextColor.B));
                                            normalMessageParagraph.Inlines.Add(new Bold(commonRun));
                                            normalMessageParagraph.Inlines.Add(messageText);
                                        }
                                        DetectURLs(normalMessageParagraph);
                                        //flowDocument.Blocks.Add(normalMessageParagraph);
                                        (_chatDataContext.MainWindowSession[interactionId] as ChatUtil).RTBConsultDocument.Blocks.Add(normalMessageParagraph);

                                    }
                                    else if (noticeText != string.Empty && messageText != string.Empty)
                                    {
                                        if (_chatDataContext.AgentPromptColor.ToString().Contains("#"))
                                        {
                                            commonRun = new Run(userNameWithTimeShift);
                                            commonRun.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom(_chatDataContext.AgentPromptColor.Name.ToString()));
                                        }
                                        else
                                        {
                                            commonRun = new Run(userNameWithTimeShift);
                                            commonRun.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(_chatDataContext.AgentPromptColor.A, _chatDataContext.AgentPromptColor.R, _chatDataContext.AgentPromptColor.G, _chatDataContext.AgentPromptColor.B));
                                        }
                                        if (_chatDataContext.AgentTextColor.ToString().Contains("#"))
                                        {
                                            urlMessageParagraph.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom(_chatDataContext.AgentTextColor.Name.ToString()));
                                        }
                                        else
                                        {
                                            urlMessageParagraph.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(_chatDataContext.AgentTextColor.A, _chatDataContext.AgentTextColor.R, _chatDataContext.AgentTextColor.G, _chatDataContext.AgentTextColor.B));
                                        }
                                        urlMessageParagraph.Inlines.Add(new Bold(commonRun));
                                        urlMessageParagraph.Inlines.Add(messageText);
                                        Run temprun = null;
                                        var urlkey = _chatDataContext.LoadAvailablePushURL.FirstOrDefault(x => x.Value.Contains(noticeText)).Key;
                                        if (string.IsNullOrEmpty(urlkey))
                                        {
                                            if (noticeText.Contains("http://"))
                                            {
                                                string originalURL = noticeText.Replace("http://", "");
                                                var originalUrlKey = _chatDataContext.LoadAvailablePushURL.FirstOrDefault(x => x.Value.Contains(originalURL.ToString())).Key;
                                                if (string.IsNullOrEmpty(originalUrlKey))
                                                {
                                                    originalURL = originalURL.Replace("/", "");
                                                    var originalUrlKey1 = _chatDataContext.LoadAvailablePushURL.FirstOrDefault(x => x.Value.Contains(originalURL.ToString())).Key;
                                                    if (string.IsNullOrEmpty(originalUrlKey1))
                                                        temprun = new Run("Click Here");
                                                    else
                                                        temprun = new Run(originalUrlKey1);
                                                }
                                                else
                                                    temprun = new Run(originalUrlKey);
                                                originalURL = null;
                                            }
                                            else
                                                temprun = new Run("Click Here");
                                        }
                                        else
                                            temprun = new Run(urlkey.ToString());
                                        Hyperlink hl = new Hyperlink(temprun);
                                        hl.Foreground = Brushes.Blue;
                                        hl.NavigateUri = new Uri(noticeText);
                                        hl.ToolTip = dynamicToolTip("Pushed URL", noticeText);
                                        urlMessageParagraph.Inlines.Add(hl);
                                        (_chatDataContext.MainWindowSession[interactionId] as ChatUtil).RTBConsultDocument.Blocks.Add(urlMessageParagraph);

                                    }
                                    break;
                                case ChatDataContext.ChatUsertype.Client:
                                    if (noticeText == string.Empty && messageText == string.Empty)
                                    {
                                        (_chatDataContext.MainWindowSession[interactionId] as ChatUtil).TypeNotifierParagraph.Inlines.Clear();
                                        (_chatDataContext.MainWindowSession[interactionId] as ChatUtil).TypeNotifierParagraph.FontStyle = FontStyles.Italic;
                                        (_chatDataContext.MainWindowSession[interactionId] as ChatUtil).TypeNotifierParagraph.Inlines.Add(new Run(userNameWithTimeShift));
                                        (_chatDataContext.MainWindowSession[interactionId] as ChatUtil).TypeNotifierParagraph.Foreground = Brushes.LightGray;
                                        //flowDocument.Blocks.Add(typeNotifierParagraph);
                                        (_chatDataContext.MainWindowSession[interactionId] as ChatUtil).RTBConsultDocument.Blocks.Add((_chatDataContext.MainWindowSession[interactionId] as ChatUtil).TypeNotifierParagraph);

                                    }
                                    if (noticeText == string.Empty && messageText != string.Empty)
                                    {
                                        if (_chatDataContext.ClientPromptColor.ToString().Contains("#"))
                                        {
                                            commonRun = new Run(userNameWithTimeShift);
                                            commonRun.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom(_chatDataContext.ClientPromptColor.Name.ToString()));
                                        }
                                        else
                                        {
                                            commonRun = new Run(userNameWithTimeShift);
                                            commonRun.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(_chatDataContext.ClientPromptColor.A, _chatDataContext.ClientPromptColor.R, _chatDataContext.ClientPromptColor.G, _chatDataContext.ClientPromptColor.B));
                                        }
                                        if (_chatDataContext.ClientTextColor.ToString().Contains("#"))
                                        {
                                            normalMessageParagraph.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom(_chatDataContext.ClientTextColor.Name.ToString()));
                                            normalMessageParagraph.Inlines.Add(new Bold(commonRun));
                                            normalMessageParagraph.Inlines.Add(messageText);
                                        }
                                        else
                                        {
                                            normalMessageParagraph.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(_chatDataContext.ClientTextColor.A, _chatDataContext.ClientTextColor.R, _chatDataContext.ClientTextColor.G, _chatDataContext.ClientTextColor.B));
                                            normalMessageParagraph.Inlines.Add(new Bold(commonRun));
                                            normalMessageParagraph.Inlines.Add(messageText);
                                        }
                                        DetectURLs(normalMessageParagraph);
                                        //flowDocument.Blocks.Add(normalMessageParagraph);
                                        (_chatDataContext.MainWindowSession[interactionId] as ChatUtil).RTBConsultDocument.Blocks.Add(normalMessageParagraph);

                                    }
                                    else if (noticeText != string.Empty && messageText != string.Empty)
                                    {
                                        if (_chatDataContext.ClientPromptColor.ToString().Contains("#"))
                                        {
                                            commonRun = new Run(userNameWithTimeShift);
                                            commonRun.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom(_chatDataContext.ClientPromptColor.Name.ToString()));
                                        }
                                        else
                                        {
                                            commonRun = new Run(userNameWithTimeShift);
                                            commonRun.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(_chatDataContext.ClientPromptColor.A, _chatDataContext.ClientPromptColor.R, _chatDataContext.ClientPromptColor.G, _chatDataContext.ClientPromptColor.B));
                                        }
                                        if (_chatDataContext.ClientTextColor.ToString().Contains("#"))
                                        {
                                            urlMessageParagraph.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom(_chatDataContext.ClientTextColor.Name.ToString()));
                                        }
                                        else
                                        {
                                            urlMessageParagraph.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(_chatDataContext.ClientTextColor.A, _chatDataContext.ClientTextColor.R, _chatDataContext.ClientTextColor.G, _chatDataContext.ClientTextColor.B));
                                        }
                                        urlMessageParagraph.Inlines.Add(new Bold(commonRun));
                                        urlMessageParagraph.Inlines.Add(messageText);
                                        if (_chatDataContext.PushedURLKey != string.Empty)
                                        {
                                            Hyperlink hl = new Hyperlink(new Run(_chatDataContext.PushedURLKey));
                                            hl.Foreground = Brushes.Blue;
                                            hl.NavigateUri = new Uri(noticeText);
                                            urlMessageParagraph.Inlines.Add(hl);
                                        }
                                        else
                                        {
                                            Run temprun = null;
                                            var urlkey = _chatDataContext.LoadAvailablePushURL.FirstOrDefault(x => x.Value.Contains(noticeText)).Key;
                                            if (string.IsNullOrEmpty(urlkey))
                                            {
                                                if (noticeText.Contains("http://"))
                                                {
                                                    string originalURL = noticeText.Replace("http://", "");
                                                    var originalUrlKey = _chatDataContext.LoadAvailablePushURL.FirstOrDefault(x => x.Value.Contains(originalURL.ToString())).Key;
                                                    if (string.IsNullOrEmpty(originalUrlKey))
                                                    {
                                                        originalURL = originalURL.Replace("/", "");
                                                        var originalUrlKey1 = _chatDataContext.LoadAvailablePushURL.FirstOrDefault(x => x.Value.Contains(originalURL.ToString())).Key;
                                                        if (string.IsNullOrEmpty(originalUrlKey1))
                                                            temprun = new Run("Click Here");
                                                        else
                                                            temprun = new Run(originalUrlKey1);
                                                    }
                                                    else
                                                        temprun = new Run(originalUrlKey);
                                                    originalURL = null;
                                                }
                                                else
                                                    temprun = new Run("Click Here");
                                            }
                                            else
                                                temprun = new Run(urlkey.ToString());
                                            Hyperlink hl = new Hyperlink(temprun);
                                            hl.Foreground = Brushes.Blue;
                                            hl.NavigateUri = new Uri(noticeText);
                                            hl.ToolTip = dynamicToolTip("Pushed URL", noticeText);
                                            urlMessageParagraph.Inlines.Add(hl);
                                        }
                                        (_chatDataContext.MainWindowSession[interactionId] as ChatUtil).RTBConsultDocument.Blocks.Add(urlMessageParagraph);

                                    }
                                    break;
                                case ChatDataContext.ChatUsertype.OtherAgent:
                                    if (noticeText == string.Empty && messageText == string.Empty)
                                    {

                                    }
                                    if (noticeText == string.Empty && messageText != string.Empty)
                                    {
                                        if (_chatDataContext.OtherAgentPromptColor.ToString().Contains("#"))
                                        {
                                            commonRun = new Run(userNameWithTimeShift);
                                            commonRun.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom(_chatDataContext.OtherAgentPromptColor.Name.ToString()));
                                        }
                                        else
                                        {
                                            commonRun = new Run(userNameWithTimeShift);
                                            commonRun.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(_chatDataContext.OtherAgentPromptColor.A, _chatDataContext.OtherAgentPromptColor.R, _chatDataContext.OtherAgentPromptColor.G, _chatDataContext.OtherAgentPromptColor.B));
                                        }
                                        if (_chatDataContext.OtherAgentTextColor.ToString().Contains("#"))
                                        {
                                            normalMessageParagraph.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom(_chatDataContext.OtherAgentTextColor.Name.ToString()));
                                            normalMessageParagraph.Inlines.Add(new Bold(commonRun));
                                            normalMessageParagraph.Inlines.Add(messageText);
                                        }
                                        else
                                        {
                                            normalMessageParagraph.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(_chatDataContext.OtherAgentTextColor.A, _chatDataContext.OtherAgentTextColor.R, _chatDataContext.OtherAgentTextColor.G, _chatDataContext.OtherAgentTextColor.B));
                                            normalMessageParagraph.Inlines.Add(new Bold(commonRun));
                                            normalMessageParagraph.Inlines.Add(messageText);
                                        }
                                        DetectURLs(normalMessageParagraph);
                                        //flowDocument.Blocks.Add(normalMessageParagraph);
                                        (_chatDataContext.MainWindowSession[interactionId] as ChatUtil).RTBConsultDocument.Blocks.Add(normalMessageParagraph);

                                    }
                                    else if (noticeText != string.Empty && messageText != string.Empty)
                                    {
                                        if (_chatDataContext.OtherAgentPromptColor.ToString().Contains("#"))
                                        {
                                            commonRun = new Run(userNameWithTimeShift);
                                            commonRun.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom(_chatDataContext.OtherAgentPromptColor.Name.ToString()));
                                        }
                                        else
                                        {
                                            commonRun = new Run(userNameWithTimeShift);
                                            commonRun.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(_chatDataContext.OtherAgentPromptColor.A, _chatDataContext.OtherAgentPromptColor.R, _chatDataContext.OtherAgentPromptColor.G, _chatDataContext.OtherAgentPromptColor.B));
                                        }
                                        if (_chatDataContext.OtherAgentTextColor.ToString().Contains("#"))
                                        {
                                            urlMessageParagraph.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom(_chatDataContext.OtherAgentTextColor.Name.ToString()));
                                        }
                                        else
                                        {
                                            urlMessageParagraph.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(_chatDataContext.OtherAgentTextColor.A, _chatDataContext.OtherAgentTextColor.R, _chatDataContext.OtherAgentTextColor.G, _chatDataContext.OtherAgentTextColor.B));
                                        }
                                        urlMessageParagraph.Inlines.Add(new Bold(commonRun));
                                        urlMessageParagraph.Inlines.Add(messageText);
                                        Run temprun = null;
                                        var urlkey = _chatDataContext.LoadAvailablePushURL.FirstOrDefault(x => x.Value.Contains(noticeText)).Key;
                                        if (string.IsNullOrEmpty(urlkey))
                                        {
                                            if (noticeText.Contains("http://"))
                                            {
                                                string originalURL = noticeText.Replace("http://", "");
                                                var originalUrlKey = _chatDataContext.LoadAvailablePushURL.FirstOrDefault(x => x.Value.Contains(originalURL.ToString())).Key;
                                                if (string.IsNullOrEmpty(originalUrlKey))
                                                {
                                                    originalURL = originalURL.Replace("/", "");
                                                    var originalUrlKey1 = _chatDataContext.LoadAvailablePushURL.FirstOrDefault(x => x.Value.Contains(originalURL.ToString())).Key;
                                                    if (string.IsNullOrEmpty(originalUrlKey1))
                                                        temprun = new Run("Click Here");
                                                    else
                                                        temprun = new Run(originalUrlKey1);
                                                }
                                                else
                                                    temprun = new Run(originalUrlKey);
                                                originalURL = null;
                                            }
                                            else
                                                temprun = new Run("Click Here");
                                        }
                                        else
                                            temprun = new Run(urlkey.ToString());
                                        Hyperlink hl = new Hyperlink(temprun);
                                        hl.Foreground = Brushes.Blue;
                                        hl.NavigateUri = new Uri(noticeText);
                                        urlMessageParagraph.Inlines.Add(hl);
                                        hl.ToolTip = dynamicToolTip("Pushed URL", noticeText);
                                        (_chatDataContext.MainWindowSession[interactionId] as ChatUtil).RTBConsultDocument.Blocks.Add(urlMessageParagraph);
                                    }
                                    break;
                            }
                            break;
                        case InfoType.PartyLeftInfo:
                            switch (chatUserType)
                            {
                                case ChatDataContext.ChatUsertype.Agent:
                                    if (_chatDataContext.AgentPromptColor.ToString().Contains("#"))
                                    {
                                        commonRun = new Run(userNameWithTimeShift);
                                        commonRun.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom(_chatDataContext.AgentPromptColor.Name.ToString()));
                                    }
                                    else
                                    {
                                        commonRun = new Run(userNameWithTimeShift);
                                        commonRun.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(_chatDataContext.AgentPromptColor.A, _chatDataContext.AgentPromptColor.R, _chatDataContext.AgentPromptColor.G, _chatDataContext.AgentPromptColor.B));
                                    }
                                    if (_chatDataContext.AgentTextColor.ToString().Contains("#"))
                                    {
                                        normalMessageParagraph.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom(_chatDataContext.AgentTextColor.Name.ToString()));
                                        normalMessageParagraph.Inlines.Add(new Bold(commonRun));
                                        normalMessageParagraph.Inlines.Add(messageText);
                                    }
                                    else
                                    {
                                        normalMessageParagraph.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(_chatDataContext.AgentTextColor.A, _chatDataContext.AgentTextColor.R, _chatDataContext.AgentTextColor.G, _chatDataContext.AgentTextColor.B));
                                        normalMessageParagraph.Inlines.Add(new Bold(commonRun));
                                        normalMessageParagraph.Inlines.Add(messageText);
                                    }
                                    break;
                                case ChatDataContext.ChatUsertype.Client:
                                    if (_chatDataContext.ClientPromptColor.ToString().Contains("#"))
                                    {
                                        commonRun = new Run(userNameWithTimeShift);
                                        commonRun.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom(_chatDataContext.ClientPromptColor.Name.ToString()));
                                    }
                                    else
                                    {
                                        commonRun = new Run(userNameWithTimeShift);
                                        commonRun.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(_chatDataContext.ClientPromptColor.A, _chatDataContext.ClientPromptColor.R, _chatDataContext.ClientPromptColor.G, _chatDataContext.ClientPromptColor.B));
                                    }
                                    if (_chatDataContext.ClientTextColor.ToString().Contains("#"))
                                    {
                                        normalMessageParagraph.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom(_chatDataContext.ClientTextColor.Name.ToString()));
                                        normalMessageParagraph.Inlines.Add(new Bold(commonRun));
                                        normalMessageParagraph.Inlines.Add(messageText);
                                    }
                                    else
                                    {
                                        normalMessageParagraph.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(_chatDataContext.ClientTextColor.A, _chatDataContext.ClientTextColor.R, _chatDataContext.ClientTextColor.G, _chatDataContext.ClientTextColor.B));
                                        normalMessageParagraph.Inlines.Add(new Bold(commonRun));
                                        normalMessageParagraph.Inlines.Add(messageText);
                                    }
                                    break;
                                case ChatDataContext.ChatUsertype.OtherAgent:
                                    if (_chatDataContext.OtherAgentPromptColor.ToString().Contains("#"))
                                    {
                                        commonRun = new Run(userNameWithTimeShift);
                                        commonRun.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom(_chatDataContext.OtherAgentPromptColor.Name.ToString()));
                                    }
                                    else
                                    {
                                        commonRun = new Run(userNameWithTimeShift);
                                        commonRun.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(_chatDataContext.OtherAgentPromptColor.A, _chatDataContext.OtherAgentPromptColor.R, _chatDataContext.OtherAgentPromptColor.G, _chatDataContext.OtherAgentPromptColor.B));
                                    }
                                    if (_chatDataContext.OtherAgentTextColor.ToString().Contains("#"))
                                    {
                                        normalMessageParagraph.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom(_chatDataContext.OtherAgentTextColor.Name.ToString()));
                                        normalMessageParagraph.Inlines.Add(new Bold(commonRun));
                                        normalMessageParagraph.Inlines.Add(messageText);
                                    }
                                    else
                                    {
                                        normalMessageParagraph.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(_chatDataContext.OtherAgentTextColor.A, _chatDataContext.OtherAgentTextColor.R, _chatDataContext.OtherAgentTextColor.G, _chatDataContext.OtherAgentTextColor.B));
                                        normalMessageParagraph.Inlines.Add(new Bold(commonRun));
                                        normalMessageParagraph.Inlines.Add(messageText);
                                    }
                                    break;
                            }
                            DetectURLs(normalMessageParagraph);
                            //flowDocument.Blocks.Add(normalMessageParagraph);
                            (_chatDataContext.MainWindowSession[interactionId] as ChatUtil).RTBConsultDocument.Blocks.Add(normalMessageParagraph);
                            break;
                    }
                }
                catch (Exception generalException)
                {
                    logger.Error("Error occurred while do chatConsultConversationBinding()" + generalException.ToString());
                }
            }));
        }


        /// <summary>
        /// Chat_s the listener.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Chat_Listener(IMessage iMessage)
        {
            var chatMedia = new ChatMedia();
            EventSessionInfo statusEvent = null;
            ChatTranscript transcript = null;
            if (iMessage == null)
                return;
            Application.Current.Dispatcher.Invoke((System.Action)(delegate
            {
                try
                {
                    switch (iMessage.Id)
                    {
                        case EventSessionInfo.MessageId:
                            if (iMessage.Id == EventSessionInfo.MessageId)
                            {
                                statusEvent = (EventSessionInfo)iMessage;
                                logger.Trace(statusEvent.ToString());
                                transcript = statusEvent.ChatTranscript;
                                if (transcript == null) return;
                                string _interactionID = transcript.SessionId;
                                if (!_chatDataContext.MainWindowSession.ContainsKey(_interactionID)) return;
                                if (statusEvent.SessionStatus == SessionStatus.Over)
                                {
                                    ObservableCollection<Pointel.Interactions.Chat.Helpers.IPartyInfo> temp = (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).PartiesInfo;
                                    if (transcript.ChatEventList.GetAsPartyLeftInfo(0) != null)
                                    {
                                        var getUsers = temp.Where(x => x.UserType == "Agent" && x.UserState == "Connected").ToList();
                                        PartyLeftInfo partyLeftInfo = transcript.ChatEventList.GetAsPartyLeftInfo(0);
                                        var getUserNickName = temp.Where(x => x.UserID == partyLeftInfo.UserId).ToList();
                                        if (getUserNickName.Count > 0)
                                            foreach (var item in getUserNickName)
                                            {
                                                if (_chatDataContext.AgentNickName == item.UserNickName && getUsers.Count > 1)
                                                {
                                                    OutputValues output = chatMedia.DoLeaveInteractionFromConference(_interactionID, (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ProxyId, item.UserID);
                                                    if (output.MessageCode == "200")
                                                        logger.Info(output.Message);
                                                    else
                                                        logger.Error(output.Message);
                                                }
                                            }
                                    }
                                    (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ReleaseImageSource = _chatDataContext.GetBitmapImage(new Uri(_chatDataContext.Imagepath + "\\Chat\\Chat.Release.Disable.png", UriKind.Relative));
                                    (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).IsEnableRelease = false;
                                    (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).TransImageSource = _chatDataContext.GetBitmapImage(new Uri(_chatDataContext.Imagepath + "\\Chat\\Chat.Transfer.Disable.png", UriKind.Relative));
                                    (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).IsEnableTransfer = false;
                                    (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ConfImageSource = _chatDataContext.GetBitmapImage(new Uri(_chatDataContext.Imagepath + "\\Chat\\Chat.Conference.Disable.png", UriKind.Relative));
                                    (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).IsEnableConference = false;
                                    (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ConsultChatImageSource = _chatDataContext.GetBitmapImage(new Uri(_chatDataContext.Imagepath + "\\Chat\\Chat.Consult.Disable.png", UriKind.Relative));
                                    (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).IsEnableChatConsult = false;
                                    (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).DoneImageSource = _chatDataContext.GetBitmapImage(new Uri(_chatDataContext.Imagepath + "\\Chat\\Chat.MarkDone.png", UriKind.Relative));
                                    (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).IsEnableDone = true;
                                    (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).VoiceConsultImageSource = _chatDataContext.GetBitmapImage(new Uri(_chatDataContext.Imagepath + "\\Chat\\Chat.Consult.Call.Disable.png", UriKind.Relative));
                                    (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).IsEnableVoiceConsult = false;
                                    (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).IsTextMessageEnabled = false;
                                    (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).IsTextURLEnabled = false;
                                    (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).IsButtonSendEnabled = false;
                                    (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).IsButtonCheckURL = false;
                                    (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).IsButtonAvailableURL = false;
                                    (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).IsButtonPushURLExpander = false;
                                    (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).IsConversationRTBEnabled = true;
                                }
                                (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).SessionID = transcript.SessionId;
                                if (transcript != null)
                                {
                                    (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).SessionStartedTime = Convert.ToDateTime(transcript.StartAt.ToString());

                                    #region NewPartyInfo

                                    if (transcript.ChatEventList.GetAsNewPartyInfo(0) != null)
                                    {
                                        employeeID = string.Empty;
                                        placeID = string.Empty;
                                        NewPartyInfo _newUserMessageInfo = transcript.ChatEventList.GetAsNewPartyInfo(0);
                                        employeeID = _newUserMessageInfo.UserInfo.PersonId;
                                        if ((_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatParties.Count > 0 && !string.IsNullOrEmpty(employeeID))
                                        {
                                            if ((_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatParties.ContainsKey(employeeID))
                                                placeID = (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatParties[employeeID].ToString();
                                        }
                                        if (_newUserMessageInfo.Visibility == Genesyslab.Platform.WebMedia.Protocols.BasicChat.Visibility.Int)
                                        {
                                            (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatPersonStatusIcon = _chatDataContext.GetBitmapImage(new Uri(_chatDataContext.Imagepath + "\\Chat\\Chat.png", UriKind.Relative));
                                            if (_chatDataContext.AgentNickName != _newUserMessageInfo.UserInfo.UserNickname.ToString())
                                            {
                                                if ((_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatConsultPersonStatusInfo.Count(p => p.ChatPersonName == _newUserMessageInfo.UserInfo.UserNickname.ToString()) == 0 &&
                                                    (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatConsultPersonStatusInfo.Count(p => p.AgentID == employeeID) == 0)
                                                    (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatConsultPersonStatusInfo.Add(new ChatPersonsStatus(employeeID, placeID, _newUserMessageInfo.UserInfo.UserNickname.ToString(), (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatPersonStatusIcon, "Connected"));
                                                else
                                                {
                                                    var item = (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatConsultPersonStatusInfo.FirstOrDefault(p => p.AgentID == employeeID);
                                                    if (item != null)
                                                    {
                                                        int index = (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatConsultPersonStatusInfo.IndexOf(item);
                                                        (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatConsultPersonStatusInfo.RemoveAt(index);
                                                        (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatConsultPersonStatusInfo.Insert(index, new ChatPersonsStatus(item.AgentID, item.PlaceID, _newUserMessageInfo.UserInfo.UserNickname.ToString(), (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatPersonStatusIcon, "Connected"));
                                                    }
                                                }
                                            }

                                            if (_newUserMessageInfo.UserInfo.UserType == UserType.Client)
                                            {
                                                if ((_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).PartiesInfo.Count(p => p.UserID == _newUserMessageInfo.UserId) == 0)
                                                    (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).PartiesInfo.Add(new PartyInfo(_newUserMessageInfo.UserId, _newUserMessageInfo.UserInfo.UserType.ToString(), _newUserMessageInfo.UserInfo.UserNickname.ToString(), "Connected", _newUserMessageInfo.UserInfo.PersonId, "Int"));

                                                chatConsultConversationBinding(_interactionID, ChatDataContext.ChatUsertype.Client, getCurrentTimewithSpecFormat(_newUserMessageInfo.TimeShift, _interactionID), InfoType.NewPartyInfo, " New party '" + _newUserMessageInfo.UserInfo.UserNickname.ToString() + "' has joined the session", string.Empty);
                                            }
                                            else if (_newUserMessageInfo.UserInfo.UserType == UserType.Agent)
                                            {
                                                if ((_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).PartiesInfo.Count(p => p.UserID == _newUserMessageInfo.UserId) == 0)
                                                    (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).PartiesInfo.Add(new PartyInfo(_newUserMessageInfo.UserId, _newUserMessageInfo.UserInfo.UserType.ToString(), _newUserMessageInfo.UserInfo.UserNickname.ToString(), "Connected", _newUserMessageInfo.UserInfo.PersonId, "Int"));

                                                if (_chatDataContext.AgentNickName == _newUserMessageInfo.UserInfo.UserNickname.ToString())
                                                {
                                                    chatConsultConversationBinding(_interactionID, ChatDataContext.ChatUsertype.Agent, getCurrentTimewithSpecFormat(_newUserMessageInfo.TimeShift, _interactionID), InfoType.NewPartyInfo, " New party '" + _newUserMessageInfo.UserInfo.UserNickname.ToString() + "' has joined the session", string.Empty);
                                                }
                                                else
                                                {
                                                    if ((_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ConsultChatWindowRowHeight == new GridLength(0))
                                                    {
                                                        (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatConsultPersonStatusInfo.Clear();
                                                        (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ConsultDockPanel.Children.Clear();
                                                        (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ConsultDockPanel.Children.Add(new ChatConsultationWindow((_chatDataContext.MainWindowSession[_interactionID] as ChatUtil), _interactionID));
                                                        (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ConsultChatWindowRowHeight = GridLength.Auto;
                                                        loadConsultUserControlData(_newUserMessageInfo, _interactionID);
                                                    }
                                                    else
                                                    {
                                                        loadConsultUserControlData(_newUserMessageInfo, _interactionID);
                                                        (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ConsultChatRowHight = GridLength.Auto;
                                                        //(_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ImgConsultChatExpander = _chatDataContext.GetBitmapImage(new Uri("/Agent.Interaction.Desktop;component/Images/upArrow.png", UriKind.Relative));
                                                        //btnShowHideConsultEx.Tag = "Hide";
                                                        (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ConsultPersonStatus = "Connected";
                                                        (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ConsultReleaseImageSource = _chatDataContext.GetBitmapImage(new Uri("/Agent.Interaction.Desktop;component/Images/Chat/Chat.Release.png", UriKind.Relative));
                                                        (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ConsultReleaseText = "Release";
                                                        (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ConsultReleaseTTHeading = "End Chat";
                                                        (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ConsultReleaseTTContent = "Release the Chat.";
                                                        (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).IsButtonConsultSendEnabled = false;
                                                        (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).SendchatConsultWindowRowHeight = GridLength.Auto;
                                                    }
                                                    chatConsultConversationBinding(_interactionID, ChatDataContext.ChatUsertype.OtherAgent, getCurrentTimewithSpecFormat(_newUserMessageInfo.TimeShift, _interactionID), InfoType.NewPartyInfo, " New party '" + _newUserMessageInfo.UserInfo.UserNickname.ToString() + "' has joined the session", string.Empty);
                                                }
                                            }

                                            //if ((_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ISConsultChatInitialized)
                                            //{
                                            //    //if (_chatDataContext.ConsultUserControl.Parent == null)
                                            //    //{
                                            //    (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatConsultPersonStatusInfo.Clear();
                                            //    (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ConsultDockPanel.Children.Clear();
                                            //    //BindingOperations.ClearBinding((DependencyObject)_chatDataContext.ConsultUserControl, ContentControl.ContentProperty);
                                            //    //_chatDataContext.ConsultUserControl = null;
                                            //    //ContentControl cc = new ContentControl();
                                            //    (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ConsultDockPanel.Children.Add(new ChatConsultationWindow((_chatDataContext.MainWindowSession[_interactionID] as ChatUtil), _chatDataContext.InteractionId));
                                            //    //BindingOperations.SetBinding(cc, ContentControl.ContentProperty, new Binding() { Path = new PropertyPath("ConsultUserControl"), Source = _chatDataContext });
                                            //    //_chatDataContext.ConsultDockPanel.Children.Add(cc);
                                            //    (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ConsultChatWindowRowHeight = GridLength.Auto;
                                            //    //_chatDataContext.ConsultUserControl.Height = double.NaN;
                                            //    loadConsultUserControlData(_newUserMessageInfo);
                                            //    //}
                                            //}
                                        }
                                        else if (_newUserMessageInfo.Visibility == Genesyslab.Platform.WebMedia.Protocols.BasicChat.Visibility.All)
                                        {
                                            (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatPersonStatusIcon = _chatDataContext.GetBitmapImage(new Uri(_chatDataContext.Imagepath + "\\Chat\\Chat.png", UriKind.Relative));
                                            if (_chatDataContext.AgentNickName != _newUserMessageInfo.UserInfo.UserNickname.ToString() && (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).IxnType != "Consult")
                                            {
                                                if ((_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatPersonsStatusInfo.Count(p => p.ChatPersonName == _newUserMessageInfo.UserInfo.UserNickname.ToString()) == 0 &&
                                                    (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatPersonsStatusInfo.Count(p => p.AgentID == employeeID) == 0)
                                                    (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatPersonsStatusInfo.Add(new ChatPersonsStatus(employeeID, placeID, _newUserMessageInfo.UserInfo.UserNickname.ToString(), (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatPersonStatusIcon, "Connected"));
                                                else
                                                {
                                                    var item = (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatPersonsStatusInfo.FirstOrDefault(p => p.AgentID == employeeID);
                                                    if (item != null)
                                                    {
                                                        int index = (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatPersonsStatusInfo.IndexOf(item);
                                                        (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatPersonsStatusInfo.RemoveAt(index);
                                                        (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatPersonsStatusInfo.Insert(index, new ChatPersonsStatus(item.AgentID, item.PlaceID, _newUserMessageInfo.UserInfo.UserNickname.ToString(), (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatPersonStatusIcon, "Connected"));
                                                    }
                                                }
                                            }
                                            if (_newUserMessageInfo.UserInfo.UserType == UserType.Client)
                                            {
                                                if ((_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).PartiesInfo.Count(p => p.UserID == _newUserMessageInfo.UserId) == 0)
                                                    (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).PartiesInfo.Add(new PartyInfo(_newUserMessageInfo.UserId, _newUserMessageInfo.UserInfo.UserType.ToString(), _newUserMessageInfo.UserInfo.UserNickname.ToString(), "Connected", _newUserMessageInfo.UserInfo.PersonId, "All"));

                                                chatConversationBinding(_interactionID, ChatDataContext.ChatUsertype.Client, getCurrentTimewithSpecFormat(_newUserMessageInfo.TimeShift, _interactionID), InfoType.NewPartyInfo, " New party '" + _newUserMessageInfo.UserInfo.UserNickname.ToString() + "' has joined the session", string.Empty);
                                            }
                                            else if (_newUserMessageInfo.UserInfo.UserType == UserType.Agent)
                                            {
                                                if ((_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).PartiesInfo.Count(p => p.UserID == _newUserMessageInfo.UserId) == 0)
                                                    (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).PartiesInfo.Add(new PartyInfo(_newUserMessageInfo.UserId, _newUserMessageInfo.UserInfo.UserType.ToString(), _newUserMessageInfo.UserInfo.UserNickname.ToString(), "Connected", _newUserMessageInfo.UserInfo.PersonId, "All"));

                                                if (_chatDataContext.AgentNickName == _newUserMessageInfo.UserInfo.UserNickname.ToString())
                                                {
                                                    chatConversationBinding(_interactionID, ChatDataContext.ChatUsertype.Agent, getCurrentTimewithSpecFormat(_newUserMessageInfo.TimeShift, _interactionID), InfoType.NewPartyInfo, " New party '" + _newUserMessageInfo.UserInfo.UserNickname.ToString() + "' has joined the session", string.Empty);
                                                }
                                                else
                                                {
                                                    chatConversationBinding(_interactionID, ChatDataContext.ChatUsertype.OtherAgent, getCurrentTimewithSpecFormat(_newUserMessageInfo.TimeShift, _interactionID), InfoType.NewPartyInfo, " New party '" + _newUserMessageInfo.UserInfo.UserNickname.ToString() + "' has joined the session", string.Empty);
                                                    //code added for the purpose of consult chat change original agent during transfer
                                                    ObservableCollection<Pointel.Interactions.Chat.Helpers.IPartyInfo> temp1 = (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).PartiesInfo;
                                                    var getConsultUsers = temp1.Where(x => x.UserType == "Agent" && x.UserState == "Connected" && x.Visibility == "Int").ToList();
                                                    if (getConsultUsers != null && getConsultUsers.Count > 0 && (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).IxnType == "Consult")
                                                    {
                                                        if ((_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatConsultPersonStatusInfo.Count(p => p.ChatPersonName == _newUserMessageInfo.UserInfo.UserNickname.ToString()) == 0)
                                                            (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatConsultPersonStatusInfo.Add(new ChatPersonsStatus(employeeID, placeID, _newUserMessageInfo.UserInfo.UserNickname.ToString(), (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatPersonStatusIcon, "Connected"));
                                                    }
                                                    //end
                                                }
                                            }
                                        }
                                        ObservableCollection<Pointel.Interactions.Chat.Helpers.IPartyInfo> temp = (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).PartiesInfo;

                                        var getUsers = temp.Where(x => x.UserType == "Agent" && x.UserState == "Connected").ToList();
                                        var getCustomer = temp.Where(x => x.UserType == "Client" && x.UserState == "Connected").ToList();
                                        if (getUsers != null && getUsers.Count >= 2 && getCustomer != null && getCustomer.Count >= 1)
                                        {
                                            (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).IsConferenceChat = true;
                                            (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).IsChatConferenceClick = true;
                                        }
                                        else if (getUsers != null && getUsers.Count >= 3)
                                        {
                                            (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).IsConferenceChat = true;
                                            (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).IsChatConferenceClick = true;
                                        }
                                        else
                                        {
                                            (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).IsConferenceChat = false;
                                            (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).IsChatConferenceClick = false;
                                        }
                                    }
                                    #endregion NewPartyInfo

                                    #region MessageInfo

                                    if (transcript.ChatEventList.GetAsMessageInfo(0) != null)
                                    {
                                        MessageInfo messageInfo = transcript.ChatEventList.GetAsMessageInfo(0);
                                        if (messageInfo != null && messageInfo.MessageText != null)
                                            if (messageInfo.Visibility == Genesyslab.Platform.WebMedia.Protocols.BasicChat.Visibility.Int)
                                            {
                                                //if (!String.IsNullOrEmpty(messageInfo.MessageText.Text))
                                                //{
                                                ObservableCollection<IPartyInfo> temp = (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).PartiesInfo;
                                                var getUserNickName = temp.Where(x => x.UserID == messageInfo.UserId).ToList();
                                                if (getUserNickName.Count > 0)
                                                    foreach (var item in getUserNickName)
                                                    {
                                                        if (item.UserType == ChatDataContext.ChatUsertype.Client.ToString())
                                                        {
                                                            chatConsultConversationBinding(_interactionID, ChatDataContext.ChatUsertype.Client, getCurrentTimewithSpecFormat(messageInfo.TimeShift, _interactionID) + " " + item.UserNickName.ToString() + ": ".ToString(), InfoType.MessageInfo, messageInfo.MessageText.Text.ToString(), string.Empty);
                                                        }
                                                        else if (item.UserType == ChatDataContext.ChatUsertype.Agent.ToString())
                                                        {
                                                            if (_chatDataContext.AgentNickName == item.UserNickName)
                                                            {
                                                                chatConsultConversationBinding(_interactionID, ChatDataContext.ChatUsertype.Agent, getCurrentTimewithSpecFormat(messageInfo.TimeShift, _interactionID) + " " + item.UserNickName.ToString() + ": ".ToString(), InfoType.MessageInfo, messageInfo.MessageText.Text.ToString(), string.Empty);
                                                            }
                                                            else
                                                            {
                                                                PlayTone();
                                                                //StopTone();
                                                                chatConsultConversationBinding(_interactionID, ChatDataContext.ChatUsertype.OtherAgent, getCurrentTimewithSpecFormat(messageInfo.TimeShift, _interactionID) + " " + item.UserNickName.ToString() + ": ".ToString(), InfoType.MessageInfo, messageInfo.MessageText.Text.ToString(), string.Empty);
                                                            }
                                                        }
                                                    }
                                                // }
                                            }
                                            else if (messageInfo.Visibility == Genesyslab.Platform.WebMedia.Protocols.BasicChat.Visibility.All)
                                            {
                                                //if (!String.IsNullOrEmpty(messageInfo.MessageText.Text))
                                                //{
                                                ObservableCollection<IPartyInfo> temp = (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).PartiesInfo;
                                                var getUserNickName = temp.Where(x => x.UserID == messageInfo.UserId).ToList();
                                                if (getUserNickName.Count > 0)
                                                    foreach (var item in getUserNickName)
                                                    {
                                                        if (item.UserType == ChatDataContext.ChatUsertype.Client.ToString())
                                                        {
                                                            PlayTone();
                                                            // StopTone();
                                                            NotificationPending(true, _interactionID);
                                                            startSplash(_interactionID, item.UserNickName.ToString(), messageInfo.MessageText.Text.ToString());
                                                            chatConversationBinding(_interactionID, ChatDataContext.ChatUsertype.Client, getCurrentTimewithSpecFormat(messageInfo.TimeShift, _interactionID) + " " + item.UserNickName.ToString() + ": ".ToString(), InfoType.MessageInfo, messageInfo.MessageText.Text.ToString(), string.Empty);
                                                        }
                                                        else if (item.UserType == ChatDataContext.ChatUsertype.Agent.ToString())
                                                        {
                                                            NotificationPending(false, _interactionID);
                                                            if (_chatDataContext.AgentNickName == item.UserNickName)
                                                            {
                                                                chatConversationBinding(_interactionID, ChatDataContext.ChatUsertype.Agent, getCurrentTimewithSpecFormat(messageInfo.TimeShift, _interactionID) + " " + item.UserNickName.ToString() + ": ".ToString(), InfoType.MessageInfo, messageInfo.MessageText.Text.ToString(), string.Empty);
                                                            }
                                                            else
                                                            {
                                                                PlayTone();
                                                                //StopTone();
                                                                startSplash(_interactionID, item.UserNickName.ToString(), messageInfo.MessageText.Text.ToString());
                                                                chatConversationBinding(_interactionID, ChatDataContext.ChatUsertype.OtherAgent, getCurrentTimewithSpecFormat(messageInfo.TimeShift, _interactionID) + " " + item.UserNickName.ToString() + ": ".ToString(), InfoType.MessageInfo, messageInfo.MessageText.Text.ToString(), string.Empty);
                                                            }
                                                        }
                                                    }
                                                //}
                                            }
                                    }


                                    #endregion MessageInfo

                                    #region NoticeInfo

                                    if (transcript.ChatEventList.GetAsNoticeInfo(0) != null)
                                    {
                                        NoticeInfo noticeInfo = transcript.ChatEventList.GetAsNoticeInfo(0);
                                        if (noticeInfo != null)
                                            if (noticeInfo.MessageText == null && noticeInfo.NoticeText != null && string.IsNullOrEmpty(noticeInfo.NoticeText.Text))
                                            {
                                                ObservableCollection<IPartyInfo> temp1 = (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).PartiesInfo;
                                                var getUserNickName = temp1.Where(x => x.UserID == noticeInfo.UserId).ToList();
                                                //if (_chatDataContext.IsChatEnableTyping)
                                                //{
                                                if (noticeInfo.NoticeText.NoticeType == NoticeType.TypingStarted)
                                                {
                                                    if (getUserNickName.Count > 0)
                                                        foreach (var item in getUserNickName)
                                                        {
                                                            chatConversationBinding(_interactionID, ChatDataContext.ChatUsertype.Client, item.UserNickName.ToString() + " is typing a message", InfoType.NoticeInfo, string.Empty, string.Empty);

                                                        }
                                                }
                                                if (noticeInfo.NoticeText.NoticeType == NoticeType.TypingStopped)
                                                {
                                                    if ((_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).RTBDocument.Blocks.Contains((_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).TypeNotifierParagraph))
                                                        (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).RTBDocument.Blocks.Remove((_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).TypeNotifierParagraph);
                                                    if (getUserNickName.Count > 0)
                                                        foreach (var item in getUserNickName)
                                                        {
                                                            chatConversationBinding(_interactionID, ChatDataContext.ChatUsertype.Client, item.UserNickName.ToString() + " has Stopped typing a message", InfoType.NoticeInfo, string.Empty, string.Empty);
                                                        }
                                                }
                                                // }

                                            }
                                            else
                                            {
                                                ObservableCollection<IPartyInfo> temp = (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).PartiesInfo;
                                                var getUserNickName = temp.Where(x => x.UserID == noticeInfo.UserId).ToList();
                                                if (getUserNickName.Count > 0)
                                                    foreach (var item in getUserNickName)
                                                    {
                                                        if (noticeInfo.MessageText != null)
                                                            if (!String.IsNullOrEmpty(noticeInfo.MessageText.Text))
                                                            {
                                                                if (item.UserType == ChatDataContext.ChatUsertype.Client.ToString())
                                                                {
                                                                    chatConversationBinding(_interactionID, ChatDataContext.ChatUsertype.Client, getCurrentTimewithSpecFormat(noticeInfo.TimeShift, _interactionID) + " " + item.UserNickName.ToString() + ": ".ToString(), InfoType.NoticeInfo, " " + noticeInfo.MessageText.Text.ToString(), string.Empty);
                                                                }
                                                                else if (item.UserType == ChatDataContext.ChatUsertype.Agent.ToString())
                                                                {
                                                                    if (_chatDataContext.AgentNickName == item.UserNickName)
                                                                    {
                                                                        chatConversationBinding(_interactionID, ChatDataContext.ChatUsertype.Agent, getCurrentTimewithSpecFormat(noticeInfo.TimeShift, _interactionID) + " " + item.UserNickName.ToString() + ": ".ToString(), InfoType.NoticeInfo, " " + noticeInfo.MessageText.Text.ToString(), string.Empty);
                                                                    }
                                                                    else
                                                                    {
                                                                        chatConversationBinding(_interactionID, ChatDataContext.ChatUsertype.OtherAgent, getCurrentTimewithSpecFormat(noticeInfo.TimeShift, _interactionID) + " " + item.UserNickName.ToString() + ": ".ToString(), InfoType.NoticeInfo, " " + noticeInfo.MessageText.Text.ToString(), string.Empty);
                                                                    }
                                                                }
                                                            }
                                                        if (noticeInfo.NoticeText != null)
                                                            if (noticeInfo.NoticeText.NoticeType == NoticeType.PushUrl && !String.IsNullOrEmpty(noticeInfo.NoticeText.Text.ToString()))
                                                            {
                                                                if (item.UserType == ChatDataContext.ChatUsertype.Client.ToString())
                                                                {
                                                                    NotificationPending(true, _interactionID);
                                                                    startSplash(_interactionID, item.UserNickName.ToString(), noticeInfo.NoticeText.Text.ToString());
                                                                    chatConversationBinding(_interactionID, ChatDataContext.ChatUsertype.Client, getCurrentTimewithSpecFormat(noticeInfo.TimeShift, _interactionID), InfoType.NoticeInfo, " " + item.UserNickName.ToString() + " Pushed the URL ", noticeInfo.NoticeText.Text.ToString());
                                                                }
                                                                else if (item.UserType == ChatDataContext.ChatUsertype.Agent.ToString())
                                                                {
                                                                    if (_chatDataContext.AgentNickName == item.UserNickName)
                                                                    {
                                                                        NotificationPending(false, _interactionID);
                                                                        chatConversationBinding(_interactionID, ChatDataContext.ChatUsertype.Agent, getCurrentTimewithSpecFormat(noticeInfo.TimeShift, _interactionID), InfoType.NoticeInfo, " " + item.UserNickName.ToString() + " Pushed the URL ", noticeInfo.NoticeText.Text.ToString());
                                                                    }
                                                                    else
                                                                    {
                                                                        startSplash(_interactionID, item.UserNickName.ToString(), noticeInfo.NoticeText.Text.ToString());
                                                                        chatConversationBinding(_interactionID, ChatDataContext.ChatUsertype.OtherAgent, getCurrentTimewithSpecFormat(noticeInfo.TimeShift, _interactionID), InfoType.NoticeInfo, " " + item.UserNickName.ToString() + " Pushed the URL ", noticeInfo.NoticeText.Text.ToString());
                                                                    }
                                                                }
                                                            }
                                                    }
                                            }
                                    }

                                    #endregion NoticeInfo

                                    #region PartyLeftInfo

                                    if (transcript.ChatEventList.GetAsPartyLeftInfo(0) != null)
                                    {
                                        PartyLeftInfo partyLeftInfo = transcript.ChatEventList.GetAsPartyLeftInfo(0);
                                        ObservableCollection<Pointel.Interactions.Chat.Helpers.IChatPersonsStatus> tempChatStatus = (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatPersonsStatusInfo;
                                        ObservableCollection<Pointel.Interactions.Chat.Helpers.IChatPersonsStatus> tempConsultChatStatus = (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatConsultPersonStatusInfo;
                                        (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatPersonStatusIcon = _chatDataContext.GetBitmapImage(new Uri(_chatDataContext.Imagepath + "\\Chat\\Chat.png", UriKind.Relative));
                                        string leftPartyNickName = string.Empty;
                                        ObservableCollection<Pointel.Interactions.Chat.Helpers.IPartyInfo> temp = (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).PartiesInfo;
                                        if (partyLeftInfo != null)
                                        {
                                            var getUserNickName = temp.Where(x => x.UserID == partyLeftInfo.UserId).ToList();
                                            if (partyLeftInfo.Visibility == Genesyslab.Platform.WebMedia.Protocols.BasicChat.Visibility.All)
                                            {
                                                if (getUserNickName.Count > 0)
                                                    foreach (var item in getUserNickName)
                                                    {
                                                        leftPartyNickName = item.UserNickName.ToString();
                                                        int position = temp.IndexOf(temp.Where(p => p.UserNickName == item.UserNickName).FirstOrDefault());
                                                        (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).PartiesInfo.RemoveAt(position);
                                                        if (item.UserType == ChatDataContext.ChatUsertype.Client.ToString())
                                                        {
                                                            var getClient = tempChatStatus.Where(x => x.ChatPersonName == (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatFromPersonName).ToList();
                                                            if (getClient.Count > 0)
                                                            {
                                                                foreach (var data in getClient)
                                                                {
                                                                    if ((_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).IsConferenceChat)
                                                                    {
                                                                        (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatPersonsStatusInfo.Remove(data);
                                                                        //(_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).IsConferenceChat = false;
                                                                        //int position1 = tempChatStatus.IndexOf(tempChatStatus.Where(p => p.ChatPersonName == data.ChatPersonName).FirstOrDefault());
                                                                        //(_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatPersonsStatusInfo.RemoveAt(position1);
                                                                        //(_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatPersonsStatusInfo.Insert(position1, new ChatPersonsStatus(string.Empty, string.Empty, (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatFromPersonName, (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatPersonStatusIcon, "Ended"));
                                                                    }
                                                                    else
                                                                    {
                                                                        // (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatPersonsStatusInfo.Remove(data);
                                                                        int position1 = tempChatStatus.IndexOf(tempChatStatus.Where(p => p.ChatPersonName == data.ChatPersonName).FirstOrDefault());
                                                                        (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatPersonsStatusInfo.RemoveAt(position1);
                                                                        (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatPersonsStatusInfo.Insert(position1, new ChatPersonsStatus(string.Empty, string.Empty, (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatFromPersonName, (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatPersonStatusIcon, "Ended"));
                                                                    }
                                                                }
                                                            }
                                                            chatConversationBinding(_interactionID, ChatDataContext.ChatUsertype.Client, getCurrentTimewithSpecFormat(partyLeftInfo.TimeShift, _interactionID), InfoType.PartyLeftInfo, " Party '" + item.UserNickName.ToString() + "' has left the session", string.Empty);

                                                            #region Chat Enable Auto Disconnect
                                                            if ((((string)ConfigContainer.Instance().GetValue("chat.enable.auto-disconnect")).ToLower().Equals("true")) && !(_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).IsConferenceChat)
                                                            {
                                                                string userID = string.Empty;
                                                                ObservableCollection<Pointel.Interactions.Chat.Helpers.IPartyInfo> tempdata = (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).PartiesInfo;
                                                                var getagentNickName = tempdata.Where(x => x.UserNickName == _chatDataContext.AgentNickName).ToList();
                                                                if (getagentNickName.Count > 0)
                                                                    foreach (var name in getagentNickName)
                                                                    {
                                                                        userID = name.UserID;
                                                                    }
                                                                if (userID != string.Empty)
                                                                {
                                                                    OutputValues output = chatMedia.DoForceReleasePartyChatSession((_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).SessionID, (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ProxyId, userID, _chatDataContext.ChatFareWellMessage);
                                                                    if (output.MessageCode == "200")
                                                                    {
                                                                        userID = string.Empty;
                                                                    }
                                                                }
                                                            }
                                                            #endregion Chat Enable Auto Disconnect
                                                        }
                                                        else if (item.UserType == ChatDataContext.ChatUsertype.Agent.ToString())
                                                        {
                                                            var getUser = tempChatStatus.Where(x => x.ChatPersonStatus == "Connected").ToList();
                                                            var getOtherUser = tempConsultChatStatus.Where(x => x.ChatPersonStatus == "Connected").ToList();
                                                            if ((_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).IsChatReleaseClick)
                                                            {
                                                                (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).IsChatReleaseClick = false;
                                                                if (_chatDataContext.AgentNickName == item.UserNickName)
                                                                {
                                                                    foreach (var data in getUser)
                                                                    {
                                                                        int position1 = tempChatStatus.IndexOf(tempChatStatus.Where(p => p.ChatPersonName == data.ChatPersonName).FirstOrDefault());
                                                                        (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatPersonsStatusInfo.RemoveAt(position1);
                                                                        (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatPersonsStatusInfo.Insert(position1, new ChatPersonsStatus(data.AgentID, data.PlaceID, data.ChatPersonName, (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatPersonStatusIcon, "Ended"));
                                                                    }
                                                                    chatConversationBinding(_interactionID, ChatDataContext.ChatUsertype.Agent, getCurrentTimewithSpecFormat(partyLeftInfo.TimeShift, _interactionID), InfoType.PartyLeftInfo, " Party '" + item.UserNickName.ToString() + "' has left the session", string.Empty);
                                                                }
                                                                else
                                                                {
                                                                    foreach (var data in getUser)
                                                                    {
                                                                        if ((_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).IsConferenceChat)
                                                                        {
                                                                            int position1 = tempChatStatus.IndexOf(tempChatStatus.Where(p => p.ChatPersonName == data.ChatPersonName).FirstOrDefault());
                                                                            (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatPersonsStatusInfo.RemoveAt(position1);
                                                                            (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatPersonsStatusInfo.Insert(position1, new ChatPersonsStatus(data.AgentID, data.PlaceID, data.ChatPersonName, (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatPersonStatusIcon, "Ended"));
                                                                        }
                                                                        else
                                                                        {
                                                                            if (item.UserNickName == data.ChatPersonName)
                                                                            {
                                                                                int position1 = tempChatStatus.IndexOf(tempChatStatus.Where(p => p.ChatPersonName == data.ChatPersonName).FirstOrDefault());
                                                                                (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatPersonsStatusInfo.RemoveAt(position1);
                                                                                (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatPersonsStatusInfo.Insert(position1, new ChatPersonsStatus(data.AgentID, data.PlaceID, data.ChatPersonName, (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatPersonStatusIcon, "Ended"));
                                                                                (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).IsConferenceChat = false;
                                                                                var getClient = temp.Where(x => x.UserType == "Client" && x.UserState == "Connected").ToList();
                                                                                var getAvailableUser = temp.Where(x => x.UserState == "Connected").ToList();
                                                                                if (getClient.Count == 0 && getAvailableUser.Count == 1)
                                                                                {
                                                                                    string userID = string.Empty;
                                                                                    foreach (var name in getAvailableUser)
                                                                                    {
                                                                                        userID = name.UserID;
                                                                                    }
                                                                                    if (userID != string.Empty)
                                                                                    {
                                                                                        OutputValues output = chatMedia.DoForceReleasePartyChatSession((_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).SessionID, (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ProxyId, userID, _chatDataContext.ChatFareWellMessage);
                                                                                        if (output.MessageCode == "200")
                                                                                        {
                                                                                            userID = string.Empty;
                                                                                        }
                                                                                    }
                                                                                }
                                                                                else
                                                                                    (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatPersonsStatusInfo.Remove(data);
                                                                            }
                                                                        }
                                                                    }
                                                                    chatConversationBinding(_interactionID, ChatDataContext.ChatUsertype.OtherAgent, getCurrentTimewithSpecFormat(partyLeftInfo.TimeShift, _interactionID), InfoType.PartyLeftInfo, " Party '" + item.UserNickName.ToString() + "' has left the session", string.Empty);
                                                                }
                                                            }
                                                            else
                                                            {
                                                                if (item.UserNickName == _chatDataContext.AgentNickName)
                                                                {
                                                                    ObservableCollection<IChatPersonsStatus> tempChatPersonStatus = new ObservableCollection<IChatPersonsStatus>();
                                                                    foreach (var status in (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatPersonsStatusInfo)
                                                                        tempChatPersonStatus.Add(new ChatPersonsStatus(status.AgentID, status.PlaceID, status.ChatPersonName, status.ChatPersonStatusIcon, "Ended"));
                                                                    (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatPersonsStatusInfo.Clear();
                                                                    (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatPersonsStatusInfo = tempChatPersonStatus;
                                                                    chatConversationBinding(_interactionID, ChatDataContext.ChatUsertype.Agent, getCurrentTimewithSpecFormat(partyLeftInfo.TimeShift, _interactionID), InfoType.PartyLeftInfo, " Party '" + item.UserNickName.ToString() + "' has left the session", string.Empty);
                                                                    if (_taskbarNotifier != null)
                                                                        _taskbarNotifier.ForceHidden();
                                                                }
                                                                else if (leftPartyNickName == item.UserNickName)
                                                                {
                                                                    foreach (var data in getUser)
                                                                    {
                                                                        if (data.ChatPersonName == leftPartyNickName)
                                                                        {
                                                                            if ((_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).TransferReleavePersonId != string.Empty)
                                                                            {
                                                                                (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatPersonsStatusInfo.Remove(data);
                                                                                (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).TransferReleavePersonId = string.Empty;
                                                                            }
                                                                            else if ((_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).IsConferenceChat || _chatDataContext.AgentNickName != leftPartyNickName)
                                                                            {
                                                                                (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatPersonsStatusInfo.Remove(data);
                                                                                (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).IsConferenceChat = false;
                                                                            }
                                                                            else
                                                                            {
                                                                                int position1 = tempChatStatus.IndexOf(tempChatStatus.Where(p => p.ChatPersonName == data.ChatPersonName).FirstOrDefault());
                                                                                (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatPersonsStatusInfo.RemoveAt(position1);
                                                                                (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatPersonsStatusInfo.Insert(position1, new ChatPersonsStatus(data.AgentID, data.PlaceID, data.ChatPersonName, (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatPersonStatusIcon, "Ended"));
                                                                            }
                                                                            var getClient = temp.Where(x => x.UserType == "Client" && x.UserState == "Connected").ToList();
                                                                            var getAvailableUser = temp.Where(x => x.UserState == "Connected").ToList();
                                                                            if (getClient.Count == 0 && getAvailableUser.Count == 1)
                                                                            {
                                                                                string userID = string.Empty;
                                                                                foreach (var name in getAvailableUser)
                                                                                {
                                                                                    userID = name.UserID;
                                                                                }
                                                                                if (userID != string.Empty)
                                                                                {
                                                                                    OutputValues output = chatMedia.DoForceReleasePartyChatSession((_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).SessionID, (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ProxyId, userID, _chatDataContext.ChatFareWellMessage);
                                                                                    if (output.MessageCode == "200")
                                                                                    {
                                                                                        userID = string.Empty;
                                                                                    }
                                                                                }
                                                                            }
                                                                            else
                                                                                (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatPersonsStatusInfo.Remove(data);
                                                                        }
                                                                    }
                                                                    if (getOtherUser != null && getOtherUser.Count > 0)
                                                                        foreach (var data1 in getOtherUser)
                                                                        {
                                                                            if (data1.ChatPersonName == leftPartyNickName)
                                                                            {
                                                                                if ((_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).IsConferenceChat || _chatDataContext.AgentNickName != leftPartyNickName)
                                                                                {
                                                                                    (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatConsultPersonStatusInfo.Remove(data1);
                                                                                    (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).IsConferenceChat = false;
                                                                                }
                                                                                else
                                                                                {
                                                                                    int position1 = tempChatStatus.IndexOf(tempChatStatus.Where(p => p.ChatPersonName == data1.ChatPersonName).FirstOrDefault());
                                                                                    (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatConsultPersonStatusInfo.RemoveAt(position1);
                                                                                    (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatConsultPersonStatusInfo.Insert(position1, new ChatPersonsStatus(data1.AgentID, data1.PlaceID, data1.ChatPersonName, (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatPersonStatusIcon, "Ended"));
                                                                                }
                                                                            }
                                                                        }
                                                                    chatConversationBinding(_interactionID, ChatDataContext.ChatUsertype.Agent, getCurrentTimewithSpecFormat(partyLeftInfo.TimeShift, _interactionID), InfoType.PartyLeftInfo, " Party '" + item.UserNickName.ToString() + "' has left the session", string.Empty);
                                                                }
                                                            }

                                                        }
                                                    }
                                            }
                                            else if (partyLeftInfo.Visibility == Genesyslab.Platform.WebMedia.Protocols.BasicChat.Visibility.Int)
                                            {
                                                var getUser = tempConsultChatStatus.Where(x => x.ChatPersonStatus == "Connected").ToList();
                                                var user = tempChatStatus.Where(x => x.ChatPersonStatus == "Connected").ToList();
                                                if (getUserNickName.Count > 0)
                                                    foreach (var item in getUserNickName)
                                                    {
                                                        leftPartyNickName = item.UserNickName.ToString();
                                                        int position = temp.IndexOf(temp.Where(p => p.UserNickName == item.UserNickName).FirstOrDefault());
                                                        (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).PartiesInfo.RemoveAt(position);

                                                        if (item.UserType == ChatDataContext.ChatUsertype.Agent.ToString())
                                                        {
                                                            if (_chatDataContext.AgentNickName == item.UserNickName)
                                                            {
                                                                foreach (var data in getUser)
                                                                {
                                                                    int position1 = tempConsultChatStatus.IndexOf(tempConsultChatStatus.Where(p => p.ChatPersonName == data.ChatPersonName).FirstOrDefault());
                                                                    (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatConsultPersonStatusInfo.RemoveAt(position1);
                                                                    (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatConsultPersonStatusInfo.Insert(position1, new ChatPersonsStatus(data.AgentID, data.PlaceID, data.ChatPersonName, (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatPersonStatusIcon, "Ended"));
                                                                }
                                                                foreach (var data in user)
                                                                {
                                                                    int position1 = tempChatStatus.IndexOf(tempChatStatus.Where(p => p.ChatPersonName == data.ChatPersonName).FirstOrDefault());
                                                                    (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatPersonsStatusInfo.RemoveAt(position1);
                                                                    (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatPersonsStatusInfo.Insert(position1, new ChatPersonsStatus(data.AgentID, data.PlaceID, data.ChatPersonName, (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatPersonStatusIcon, "Ended"));
                                                                }
                                                                // chatConversationBinding(ChatDataContext.ChatUsertype.Agent, getCurrentTimewithSpecFormat(partyLeftInfo.TimeShift), InfoType.PartyLeftInfo, " Party '" + item.UserNickName.ToString() + "' has left the session", string.Empty);
                                                                chatConsultConversationBinding(_interactionID, ChatDataContext.ChatUsertype.Agent, getCurrentTimewithSpecFormat(partyLeftInfo.TimeShift, _interactionID), InfoType.PartyLeftInfo, " Party '" + item.UserNickName.ToString() + "' has left the session", string.Empty);
                                                                (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ConsultReleaseImageSource = _chatDataContext.GetBitmapImage(new Uri(_chatDataContext.Imagepath + "\\Chat\\Chat.MarkDone.png", UriKind.Relative));
                                                                (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ConsultReleaseText = "Done";
                                                                (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ConsultReleaseTTHeading = "Mark Done";
                                                                (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ConsultReleaseTTContent = "Agent can mark done after release the chat interaction.";

                                                                if ((_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ConsultDockPanel.Children.Count > 0)
                                                                {
                                                                    if ((_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ConsultDockPanel.Children[0] is ChatConsultationWindow)
                                                                    {
                                                                        ((_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ConsultDockPanel.Children[0] as ChatConsultationWindow).lblTabItemShowTimer.Stop_CustomTimer();
                                                                    }
                                                                }
                                                                //_chatDataContext.ConsultUserControl.lblTabItemShowTimer.Stop_CustomTimer();
                                                                (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).SendchatConsultWindowRowHeight = new GridLength(0);
                                                                (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatToolBarRowHeight = GridLength.Auto;
                                                                (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ReleaseImageSource = _chatDataContext.GetBitmapImage(new Uri(_chatDataContext.Imagepath + "\\Chat\\Chat.Release.Disable.png", UriKind.Relative));
                                                                (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).IsEnableRelease = false;
                                                                (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).TransImageSource = _chatDataContext.GetBitmapImage(new Uri(_chatDataContext.Imagepath + "\\Chat\\Chat.Transfer.Disable.png", UriKind.Relative));
                                                                (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).IsEnableTransfer = false;
                                                                (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ConfImageSource = _chatDataContext.GetBitmapImage(new Uri(_chatDataContext.Imagepath + "\\Chat\\Chat.Conference.Disable.png", UriKind.Relative));
                                                                (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).IsEnableConference = false;
                                                                (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ConsultChatImageSource = _chatDataContext.GetBitmapImage(new Uri(_chatDataContext.Imagepath + "\\Chat\\Chat.Consult.Disable.png", UriKind.Relative));
                                                                (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).IsEnableChatConsult = false;
                                                                (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).DoneImageSource = _chatDataContext.GetBitmapImage(new Uri(_chatDataContext.Imagepath + "\\Chat\\Chat.MarkDone.png", UriKind.Relative));
                                                                (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).IsEnableDone = true;
                                                                (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).VoiceConsultImageSource = _chatDataContext.GetBitmapImage(new Uri(_chatDataContext.Imagepath + "\\Chat\\Chat.Consult.Call.Disable.png", UriKind.Relative));
                                                                (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).IsEnableVoiceConsult = false;
                                                                if (_taskbarNotifier != null)
                                                                    _taskbarNotifier.ForceHidden();
                                                            }
                                                            else if (leftPartyNickName == item.UserNickName)
                                                            {
                                                                foreach (var data in getUser)
                                                                {
                                                                    if (data.ChatPersonName == leftPartyNickName)
                                                                    {
                                                                        if ((_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).IsConferenceChat)
                                                                        {
                                                                            (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatConsultPersonStatusInfo.Remove(data);
                                                                            // (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).IsConferenceChat = false;
                                                                        }
                                                                        else
                                                                        {
                                                                            int position1 = tempConsultChatStatus.IndexOf(tempConsultChatStatus.Where(p => p.ChatPersonName == data.ChatPersonName).FirstOrDefault());
                                                                            (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatConsultPersonStatusInfo.RemoveAt(position1);
                                                                            (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatConsultPersonStatusInfo.Insert(position1, new ChatPersonsStatus(data.AgentID, data.PlaceID, data.ChatPersonName, (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatPersonStatusIcon, "Ended"));
                                                                        }
                                                                        ObservableCollection<Pointel.Interactions.Chat.Helpers.IChatPersonsStatus> consultPerson = (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatConsultPersonStatusInfo;
                                                                        var getConsultUser = consultPerson.Where(x => x.ChatPersonStatus == "Connected").ToList();
                                                                        if (getConsultUser.Count == 0)
                                                                        {
                                                                            (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ConsultReleaseImageSource = _chatDataContext.GetBitmapImage(new Uri(_chatDataContext.Imagepath + "\\Chat\\Chat.MarkDone.png", UriKind.Relative));
                                                                            (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ConsultReleaseText = "Done";
                                                                            (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ConsultReleaseTTHeading = "Mark Done";
                                                                            (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ConsultReleaseTTContent = "Agent can mark done after release the chat interaction.";
                                                                            if ((_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ConsultDockPanel.Children.Count > 0)
                                                                            {
                                                                                if ((_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ConsultDockPanel.Children[0] is ChatConsultationWindow)
                                                                                {
                                                                                    ((_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ConsultDockPanel.Children[0] as ChatConsultationWindow).lblTabItemShowTimer.Stop_CustomTimer();
                                                                                }
                                                                            }
                                                                            (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).SendchatConsultWindowRowHeight = new GridLength(0);

                                                                        }
                                                                        chatConsultConversationBinding(_interactionID, ChatDataContext.ChatUsertype.OtherAgent, getCurrentTimewithSpecFormat(partyLeftInfo.TimeShift, _interactionID), InfoType.PartyLeftInfo, " Party '" + item.UserNickName.ToString() + "' has left the session", string.Empty);
                                                                    }
                                                                }
                                                            }
                                                            else
                                                            {
                                                                (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ConsultReleaseImageSource = _chatDataContext.GetBitmapImage(new Uri(_chatDataContext.Imagepath + "\\Chat\\Chat.MarkDone.png", UriKind.Relative));
                                                                (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ConsultReleaseText = "Done";
                                                                (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ConsultReleaseTTHeading = "Mark Done";
                                                                (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ConsultReleaseTTContent = "Agent can mark done after release the chat interaction.";
                                                                if ((_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ConsultDockPanel.Children.Count > 0)
                                                                {
                                                                    if ((_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ConsultDockPanel.Children[0] is ChatConsultationWindow)
                                                                    {
                                                                        ((_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ConsultDockPanel.Children[0] as ChatConsultationWindow).lblTabItemShowTimer.Stop_CustomTimer();
                                                                    }
                                                                }
                                                                (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).SendchatConsultWindowRowHeight = new GridLength(0);
                                                                chatConsultConversationBinding(_interactionID, ChatDataContext.ChatUsertype.OtherAgent, getCurrentTimewithSpecFormat(partyLeftInfo.TimeShift, _interactionID), InfoType.PartyLeftInfo, " Party '" + item.UserNickName.ToString() + "' has left the session", string.Empty);
                                                            }
                                                        }
                                                        if (item.UserType == ChatDataContext.ChatUsertype.Client.ToString())
                                                        {
                                                            if ((((string)ConfigContainer.Instance().GetValue("chat.enable.auto-disconnect")).ToLower().Equals("true")) && !(_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).IsConferenceChat)
                                                            {
                                                                string userID = string.Empty;
                                                                ObservableCollection<Pointel.Interactions.Chat.Helpers.IPartyInfo> tempdata = (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).PartiesInfo;
                                                                var getagentNickName = tempdata.Where(x => x.UserNickName == _chatDataContext.AgentNickName).ToList();
                                                                if (getagentNickName.Count > 0)
                                                                    foreach (var name in getagentNickName)
                                                                    {
                                                                        userID = name.UserID;
                                                                    }
                                                                if (userID != string.Empty)
                                                                {
                                                                    OutputValues output = chatMedia.DoForceReleasePartyChatSession((_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).SessionID, (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ProxyId, userID, _chatDataContext.ChatFareWellMessage);
                                                                    if (output.MessageCode == "200")
                                                                    {
                                                                        userID = string.Empty;
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                            }
                                            (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).IsConferenceChat = false;
                                        }
                                        var getUsers = temp.Where(x => x.UserType == "Agent" && x.UserState == "Connected").ToList();
                                        var getCustomer = temp.Where(x => x.UserType == "Client" && x.UserState == "Connected").ToList();
                                        if (getUsers != null && getUsers.Count >= 2 && getCustomer != null && getCustomer.Count >= 1)
                                        {
                                            (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).IsConferenceChat = true;
                                            (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).IsChatConferenceClick = true;
                                        }
                                        else if (getUsers != null && getUsers.Count >= 3)
                                        {
                                            (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).IsConferenceChat = true;
                                            (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).IsChatConferenceClick = true;
                                        }
                                        else
                                        {
                                            (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).IsConferenceChat = false;
                                            (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).IsChatConferenceClick = false;
                                        }
                                    }

                                    #endregion PartyLeftInfo
                                }
                            }
                            break;
                        case EventError.MessageId:
                            EventError eventError = (EventError)iMessage;
                            logger.Trace(eventError.ToString());
                            if (ChatDataContext.messageToClientChat != null && eventError.ErrorId == "8199")
                            {
                                //8102 Error Code : No such session is expired

                                //(_chatDataContext.MainWindowSession[_chatDataContext.InteractionId] as ChatUtil).ReleaseImageSource = _chatDataContext.GetBitmapImage(new Uri(_chatDataContext.Imagepath + "\\Chat\\Chat.Release.Disable.png", UriKind.Relative));
                                //(_chatDataContext.MainWindowSession[_chatDataContext.InteractionId] as ChatUtil).IsEnableRelease = false;
                                //(_chatDataContext.MainWindowSession[_chatDataContext.InteractionId] as ChatUtil).TransImageSource = _chatDataContext.GetBitmapImage(new Uri(_chatDataContext.Imagepath + "\\Chat\\Chat.Transfer.Disable.png", UriKind.Relative));
                                //(_chatDataContext.MainWindowSession[_chatDataContext.InteractionId] as ChatUtil).IsEnableTransfer = false;
                                //(_chatDataContext.MainWindowSession[_chatDataContext.InteractionId] as ChatUtil).ConfImageSource = _chatDataContext.GetBitmapImage(new Uri(_chatDataContext.Imagepath + "\\Chat\\Chat.Conference.Disable.png", UriKind.Relative));
                                //(_chatDataContext.MainWindowSession[_chatDataContext.InteractionId] as ChatUtil).IsEnableConference = false;
                                //(_chatDataContext.MainWindowSession[_chatDataContext.InteractionId] as ChatUtil).ConsultChatImageSource = _chatDataContext.GetBitmapImage(new Uri(_chatDataContext.Imagepath + "\\Chat\\Chat.Consult.Disable.png", UriKind.Relative));
                                //(_chatDataContext.MainWindowSession[_chatDataContext.InteractionId] as ChatUtil).IsEnableChatConsult = false;
                                //(_chatDataContext.MainWindowSession[_chatDataContext.InteractionId] as ChatUtil).DoneImageSource = _chatDataContext.GetBitmapImage(new Uri(_chatDataContext.Imagepath + "\\Chat\\Chat.MarkDone.png", UriKind.Relative));
                                //(_chatDataContext.MainWindowSession[_chatDataContext.InteractionId] as ChatUtil).IsEnableDone = true;
                                //(_chatDataContext.MainWindowSession[_chatDataContext.InteractionId] as ChatUtil).VoiceConsultImageSource = _chatDataContext.GetBitmapImage(new Uri(_chatDataContext.Imagepath + "\\Chat\\Chat.Consult.Call.Disable.png", UriKind.Relative));
                                //(_chatDataContext.MainWindowSession[_chatDataContext.InteractionId] as ChatUtil).IsEnableVoiceConsult = false;
                                //(_chatDataContext.MainWindowSession[_chatDataContext.InteractionId] as ChatUtil).IsTextMessageEnabled = false;
                                //(_chatDataContext.MainWindowSession[_chatDataContext.InteractionId] as ChatUtil).IsTextURLEnabled = false;
                                //(_chatDataContext.MainWindowSession[_chatDataContext.InteractionId] as ChatUtil).IsButtonSendEnabled = false;
                                //(_chatDataContext.MainWindowSession[_chatDataContext.InteractionId] as ChatUtil).IsButtonCheckURL = false;
                                //(_chatDataContext.MainWindowSession[_chatDataContext.InteractionId] as ChatUtil).IsButtonAvailableURL = false;
                                //(_chatDataContext.MainWindowSession[_chatDataContext.InteractionId] as ChatUtil).IsButtonPushURLExpander = false;
                                //(_chatDataContext.MainWindowSession[_chatDataContext.InteractionId] as ChatUtil).IsConversationRTBEnabled = false;
                                //(_chatDataContext.MainWindowSession[_chatDataContext.InteractionId] as ChatUtil).ErrorRowHeight = GridLength.Auto;
                                //(_chatDataContext.MainWindowSession[_chatDataContext.InteractionId] as ChatUtil).ErrorMessage = eventError.Description.Text.ToString();
                                //(_chatDataContext.MainWindowSession[_chatDataContext.InteractionId] as ChatUtil).ChatPersonsStatusInfo.Clear();
                                //(_chatDataContext.MainWindowSession[_chatDataContext.InteractionId] as ChatUtil).ChatConsultPersonStatusInfo.Clear();
                                ////ChatDataContext.messageToClientChat.ChatErrorMessage(eventError.Description.Text.ToString());
                                logger.Error("Chat_Listener() : EventError:" + eventError.Description.Text.ToString());
                            }
                            break;
                    }
                }
                catch (Exception generalException)
                {
                    logger.Error("Error occurred while Chat_Listener()" + generalException.ToString());
                }
                finally
                {
                    chatMedia = null;
                }
            }));
        }

        #region New Message Bell
        /// <summary>
        /// Plays the tone.
        /// </summary>
        public void PlayTone()
        {
            logger.Info("PlayTone Method Entry");
            if (ConfigContainer.Instance().AllKeys.Contains("chat.new-message-bell") && !string.IsNullOrEmpty((string)ConfigContainer.Instance().GetValue("chat.new-message-bell")))
            {
                try
                {
                    if (mediaPlayer != null)
                    {
                        mediaPlayer.MediaEnded -= mediaPlayer_MediaEnded;
                        mediaPlayer = null;
                    }
                    mediaPlayer = new System.Windows.Media.MediaPlayer();
                    mediaPlayer.MediaEnded += mediaPlayer_MediaEnded;

                    string path = System.IO.Path.GetFullPath((((string)ConfigContainer.Instance().GetValue("chat.new-message-bell")).Split('|')[0]));
                    // Assign path to mediaplayer
                    mediaPlayer.Open(new Uri(path));

                    // Set Volume to mediaplayer in double valid values from 0.0 to 1.0
                    if (!string.IsNullOrEmpty(((string)ConfigContainer.Instance().GetValue("chat.new-message-bell")).Split('|')[1]))
                    {
                        double volume;
                        double.TryParse(((string)ConfigContainer.Instance().GetValue("chat.new-message-bell")).Split('|')[1], out volume);
                        if (volume > 0)
                            mediaPlayer.Volume = volume;
                        else
                            mediaPlayer.Volume = 1.0;
                    }
                    else
                        mediaPlayer.Volume = 1.0;


                    // Set duration -1 means plays and repeats until an notifier closes, 0 means play the whole sound one time and  > 0 means a time, in milliseconds, to play and repeat the sound.
                    if (!string.IsNullOrEmpty(((string)ConfigContainer.Instance().GetValue("chat.new-message-bell")).Split('|')[2]))
                    {
                        int secondsforPlaying;
                        int.TryParse(((string)ConfigContainer.Instance().GetValue("chat.new-message-bell")).Split('|')[2], out secondsforPlaying);
                        if (secondsforPlaying > 0)
                        {
                            // Timer to stop the mediaplayer in defined seconds example 10 seconds
                            timerforstopplayingtone = new DispatcherTimer();
                            timerforstopplayingtone.Interval = TimeSpan.FromSeconds(secondsforPlaying);
                            timerforstopplayingtone.Tick += timerforstopplayingtone_Tick;
                            timerforstopplayingtone.Start();
                            // plays the audio 
                            mediaPlayer.Play();
                        }
                        else
                            mediaPlayer.Play();
                    }
                    else
                        mediaPlayer.Play();

                    // Assign true for stopping mediaplayer when notifier closes
                    isPlayTone = true;
                }
                catch (Exception ex)
                {
                    isPlayTone = false;
                    logger.Error("Error occurred while opening url for chat ringing bell " + ex.Message);
                }
            }
            else
                isPlayTone = false;
            logger.Info("PlayTone Method Exit");
        }

        void timerforstopplayingtone_Tick(object sender, EventArgs e)
        {
            try
            {
                mediaPlayer.Stop();
                mediaPlayer.Play();
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred in timerforstopplayingtone_Tick stopping " + ex.Message);
            }
        }

        public void StopTone()
        {
            try
            {
                if (isPlayTone)
                {
                    mediaPlayer.Stop();
                    if (timerforstopplayingtone != null)
                    {
                        timerforstopplayingtone.Stop();
                        timerforstopplayingtone.Tick -= timerforstopplayingtone_Tick;
                        timerforstopplayingtone = null;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred in media player stopping " + ex.Message);
            }
        }

        void mediaPlayer_MediaEnded(object sender, EventArgs e)
        {
            try
            {
                mediaPlayer.Stop();

                //Check the duration is -1, then repeat the audio.
                if (!string.IsNullOrEmpty(((string)ConfigContainer.Instance().GetValue("chat.new-message-bell")).Split('|')[2]) && (((string)ConfigContainer.Instance().GetValue("chat.new-message-bell")).Split('|')[2]) == "-1")
                    mediaPlayer.Play();
                else
                    isPlayTone = false;
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred in media player ended event " + ex.Message);
            }
        }
        #endregion

        /// <summary>
        /// Loads the consult user control data.
        /// </summary>
        /// <param name="_newUserMessageInfo">The _new user message information.</param>
        /// <param name="_interactionID">The _interaction identifier.</param>
        void loadConsultUserControlData(NewPartyInfo _newUserMessageInfo, string _interactionID)
        {
            try
            {
                employeeID = string.Empty;
                placeID = string.Empty;
                employeeID = _newUserMessageInfo.UserInfo.PersonId;
                if ((_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatParties.Count > 0 && !string.IsNullOrEmpty(employeeID))
                {
                    if ((_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatParties.ContainsKey(employeeID))
                        placeID = (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatParties[employeeID].ToString();
                }

                string userNickName = string.Empty;
                ObservableCollection<Pointel.Interactions.Chat.Helpers.IPartyInfo> temp = (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).PartiesInfo;
                var getUserNickName = temp.Where(x => x.PersonId == employeeID).ToList();
                var getOriginalAgent = temp.Where(x => x.Visibility == "All" && x.UserType == ChatDataContext.ChatUsertype.Agent.ToString()).ToList();

                if (getUserNickName.Count > 0)
                    foreach (var item in getUserNickName)
                    {
                        userNickName = item.UserNickName.ToString();
                        if ((_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ISConsultChatInitialized)
                        {
                            (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatConsultWindowTitleText = "Consultation: " + " '" + _newUserMessageInfo.UserInfo.UserNickname + "'";
                            //foreach (var item1 in (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatPersonsStatusInfo)
                            //{
                            //    if (item1.ChatPersonName != _chatDataContext.AgentNickName && item1.ChatPersonName != (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatFromPersonName)
                            //    {
                            //        if ((_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatConsultPersonStatusInfo.Count(p => p.ChatPersonName == item1.ChatPersonName) == 0)
                            //            (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatConsultPersonStatusInfo.Add(new ChatPersonsStatus(item1.AgentID, item1.PlaceID, item1.ChatPersonName, item1.ChatPersonStatusIcon, item1.ChatPersonStatus));
                            //    }
                            //}
                            if ((_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatConsultPersonStatusInfo.Count(p => p.ChatPersonName == _newUserMessageInfo.UserInfo.UserNickname.ToString()) == 0)
                                (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatConsultPersonStatusInfo.Add(new ChatPersonsStatus(employeeID, placeID, _newUserMessageInfo.UserInfo.UserNickname.ToString(), (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatPersonStatusIcon, "Connected"));
                            else
                            {
                                var data = (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatConsultPersonStatusInfo.FirstOrDefault(p => p.ChatPersonName == _newUserMessageInfo.UserInfo.UserNickname.ToString());
                                if (data != null)
                                {
                                    int index = (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatConsultPersonStatusInfo.IndexOf(data);
                                    (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatConsultPersonStatusInfo.RemoveAt(index);
                                    (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatConsultPersonStatusInfo.Insert(index, new ChatPersonsStatus(data.AgentID, data.PlaceID, _newUserMessageInfo.UserInfo.UserNickname.ToString(), (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatPersonStatusIcon, "Connected"));
                                }
                            }
                            (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ISConsultChatInitialized = false;
                        }
                        else
                        {
                            if (getOriginalAgent != null && getOriginalAgent.Count > 0)
                                (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatConsultWindowTitleText = "Consultation: " + " '" + getOriginalAgent[0].UserNickName + "'";
                            else
                                (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatConsultWindowTitleText = "Consultation: " + " '" + item.UserNickName + "'";
                            //foreach (var agent in getOriginalAgent)
                            //{
                            //    if (agent.PersonId != item.PersonId)
                            //        if ((_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatConsultPersonStatusInfo.Count(p => p.ChatPersonName == agent.UserNickName) == 0)
                            //            (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatConsultPersonStatusInfo.Add(new ChatPersonsStatus(employeeID, placeID, _newUserMessageInfo.UserInfo.UserNickname.ToString(), (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatPersonStatusIcon, "Connected"));
                            //}
                            if (_chatDataContext.AgentNickName != userNickName)
                                if ((_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatConsultPersonStatusInfo.Count(p => p.ChatPersonName == _newUserMessageInfo.UserInfo.UserNickname.ToString()) == 0)
                                    (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatConsultPersonStatusInfo.Add(new ChatPersonsStatus(employeeID, placeID, _newUserMessageInfo.UserInfo.UserNickname.ToString(), (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatPersonStatusIcon, "Connected"));
                        }
                    }
                (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatConsultWindowTitleText = string.IsNullOrEmpty((_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatConsultWindowTitleText) ? ("Consultation: 'UnIdentified'") : (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatConsultWindowTitleText;
            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred in loadConsultUserControlData() " + generalException.Message);
            }
        }

        public static bool IsEmailReachMaximumCount()
        {
            try
            {
                int maximumEmailCount = 5;
                if (ConfigContainer.Instance().AllKeys.Contains("email.max.intstance-count"))
                    int.TryParse(((string)ConfigContainer.Instance().GetValue("email.max.intstance-count")), out maximumEmailCount);
                List<Window> emailWindows = Application.Current.Windows.Cast<Window>().Where(x => x.Title.Equals("Email")).ToList();
                if (emailWindows.Count == maximumEmailCount)
                {
                    return true;
                }
            }
            catch (Exception generalException)
            { }
            return false;
        }

        #region Detect Rich Text Box contains hyper links
        private static readonly Regex UrlRegex = new Regex(@"(?#Protocol)(?:(?:ht|f)tp(?:s?)\:\/\/|~/|/)?(?#Username:Password)(?:\w+:\w+@)?(?#Subdomains)(?:(?:[-\w]+\.)+(?#TopLevel Domains)(?:com|org|net|gov|mil|biz|info|mobi|name|aero|jobs|museum|travel|[a-z]{2}))(?#Port)(?::[\d]{1,5})?(?#Directories)(?:(?:(?:/(?:[-\w~!$+|.,=]|%[a-f\d]{2})+)+|/)+|\?|#)?(?#Query)(?:(?:\?(?:[-\w~!$+|.,*:]|%[a-f\d{2}])+=(?:[-\w~!$+|.,*:=]|%[a-f\d]{2})*)(?:&amp;(?:[-\w~!$+|.,*:]|%[a-f\d{2}])+=(?:[-\w~!$+|.,*:=]|%[a-f\d]{2})*)*)*(?#Anchor)(?:#(?:[-\w~!$+|.,*:=]|%[a-f\d]{2})*)?");
        private static readonly Regex urlRegex = new Regex(@"^(((ht|f)tp(s?))\://)?((([a-zA-Z0-9_\-]{2,}\.)+[a-zA-Z]{2,})|((?:(?:25[0-5]|2[0-4]\d|[01]\d\d|\d?\d)(?(\.?\d)\.)){4}))(:[a-zA-Z0-9]+)?(/[a-zA-Z0-9\-\._\?\,\'/\\\+&amp;%\$#\=~]*)?$");

        /// <summary>
        /// Determines whether the specified word is hyperlink.
        /// </summary>
        /// <param name="word">The word.</param>
        /// <returns></returns>
        public static bool IsHyperlink(string word)
        {
            if (word.IndexOfAny(@":.\/".ToCharArray()) != -1)
            {
                if (UrlRegex.IsMatch(word))
                {
                    if (!word.StartsWith("http:"))
                        word = "http://" + word;
                    Uri uri;
                    if (Uri.TryCreate(word, UriKind.Absolute, out uri))
                    {
                        return true;
                    }
                }
                else if (urlRegex.IsMatch(word))
                {
                    if (!word.StartsWith("http:"))
                        word = "http://" + word;
                    Uri uri;
                    if (Uri.TryCreate(word, UriKind.Absolute, out uri))
                    {
                        return true;
                    }
                }
            }
            return false;
        }


        /// <summary>
        /// Detects the urls.
        /// </summary>
        /// <param name="par">The par.</param>
        public static void DetectURLs(Paragraph par)
        {
            Application.Current.Dispatcher.Invoke((System.Action)(delegate
            {
                try
                {
                    string paragraphText = new TextRange(par.ContentStart, par.ContentEnd).Text;
                    paragraphText = paragraphText.Replace("\n", string.Empty);
                    foreach (string word in paragraphText.Split(' ').ToList())
                    {
                        if (IsHyperlink(word))
                        {
                            Uri uri = new Uri(word, UriKind.RelativeOrAbsolute);

                            if (!uri.IsAbsoluteUri)
                            {
                                uri = new Uri(@"http://" + word, UriKind.Absolute);
                            }

                            if (uri != null)
                            {
                                TextPointer position = par.ContentStart;
                                while (position != null)
                                {
                                    if (position.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.Text)
                                    {
                                        string textRun = position.GetTextInRun(LogicalDirection.Forward);
                                        int indexInRun = textRun.IndexOf(word);
                                        if (indexInRun >= 0)
                                        {
                                            TextPointer start = position.GetPositionAtOffset(indexInRun);
                                            TextPointer end = start.GetPositionAtOffset(word.Length);
                                            var link = new Hyperlink(start, end);
                                            link.NavigateUri = uri;
                                            link.RequestNavigate += Hyperlink_Click;
                                            link.Foreground = Brushes.Blue;
                                            link.Style = null;
                                        }
                                    }
                                    position = position.GetNextContextPosition(LogicalDirection.Forward);
                                }
                            }
                        }
                    }
                }
                catch (Exception generalException)
                {
                    //  logger.Error("Error occurred in DetectURLs() " + generalException.Message);
                }
            }));

        }

        /// <summary>
        /// Handles the Click event of the Hyperlink control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        public static void Hyperlink_Click(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            try
            {
                string url = (sender as Hyperlink).NavigateUri.AbsoluteUri;

                string email = url.Replace("http", "");
                email = email.Replace("https", "");
                email = email.Replace(@"\", "");
                email = email.Replace(@"/", "");
                email = email.Replace("ftp", "");
                email = email.Replace(":", "");
                #region Email Address Checking
                string regex = @"^(?("")("".+?(?<!\\)""@)|(([0-9a-zA-Z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-zA-Z])@))" +
                    @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-zA-Z][-\w]*[0-9a-zA-Z]*\.)+[a-zA-Z0-9][\-a-zA-Z0-9]{0,22}[a-zA-Z0-9]))$";

                if (!string.IsNullOrEmpty(ChatDataContext.GetInstance().EmailValidateExpression))
                    regex = ChatDataContext.GetInstance().EmailValidateExpression;

                bool status = System.Text.RegularExpressions.Regex.IsMatch(email.TrimStart().TrimEnd(), regex);
                if (status)
                {
                    status = (email.Length - (email.IndexOf("@") + 1) <= 64);
                    if (status)
                    {
                        if (!IsEmailReachMaximumCount())
                        {
                            // "Email reached maximum count. Please close opened mail and then try to open.";
                            if (!string.IsNullOrEmpty(url) && Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections.ContainsKey(Pointel.Interactions.IPlugins.Plugins.Email))
                                ((Pointel.Interactions.IPlugins.IEmailPlugin)Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections[Pointel.Interactions.IPlugins.Plugins.Email]).NotifyNewEmail(email, null);
                        }
                    }
                }
                #endregion
                else
                    Process.Start(url);
                e.Handled = true;
            }
            catch (Exception generalException)
            {

            }
        }

        #endregion Detect Rich Text Box contains hyper links

        /// <summary>
        /// Gets the current timewith spec format.
        /// </summary>
        /// <param name="timeShift">The time shift.</param>
        /// <returns></returns>
        private string getCurrentTimewithSpecFormat(int timeShift, string _interactionId)
        {
            try
            {
                if ((((string)ConfigContainer.Instance().GetValue("chat.enable.time-stamp")).ToLower().Equals("true")))
                {
                    string format = _chatDataContext.ChatTimeStampFormat.ToLower();
                    string result = string.Empty;
                    DateTime time = (_chatDataContext.MainWindowSession[_interactionId] as ChatUtil).SessionStartedTime.AddSeconds(timeShift);
                    switch (format)
                    {
                        case "hh:mm":
                            result = "[" + time.ToString("hh:mm") + "]";
                            break;
                        case "hh:mm tt":
                            result = "[" + time.ToString("hh:mm tt") + "]";
                            break;
                        case "hh:mm:ss":
                            result = "[" + time.ToString("hh:mm:ss") + "]";
                            break;
                        case "hh:mm:ss tt":
                            result = "[" + time.ToString("hh:mm:ss tt") + "]";
                            break;
                        default:
                            result = "[" + time.ToString("hh:mm:ss tt") + "]";
                            break;
                    }
                    return result;
                }
                else
                    return string.Empty;
            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred while getCurrentTimewithSpecFormat() : " + generalException.ToString());
                return string.Empty;
            }
        }

        /// <summary>
        /// Notifications the pending.
        /// </summary>
        /// <param name="isNeed">if set to <c>true</c> [is need].</param>
        private void NotificationPending(bool isNeed, string _interactionId)
        {
            try
            {
                if (isNeed)
                {
                    TimeSpan duration;
                    string toSplit = _chatDataContext.ChatPendingResponseToCustomer;
                    string[] arr = toSplit.Split(',');
                    if (arr.Length == 2)
                    {
                        int total = (Convert.ToInt32(arr[1]) + Convert.ToInt32(arr[0])) / 2;
                        duration = TimeSpan.FromSeconds(total);
                        radialAnimationHelper = null;
                        radialAnimationHelper = new RadialAnimation();
                        radialAnimationHelper.AnimationCompleted += new Action<string>(radialAnimationHelper_AnimationCompleted); //+= new System.Action(radialAnimationHelper_AnimationCompleted(_interactionId));
                        radialAnimationHelper.MakeRadialAnimation((_chatDataContext.MainWindowSession[_interactionId] as ChatUtil).NotificationImage, duration, _interactionId);
                    }
                    else
                    {
                        (_chatDataContext.MainWindowSession[_interactionId] as ChatUtil).NotificationImageSource = "\\Pointel.Interactions.Chat;component\\Images\\Chat\\Chat.png";
                    }
                }
                else
                {
                    if (radialAnimationHelper != null)
                    {
                        radialAnimationHelper.AnimationCompleted -= new Action<string>(radialAnimationHelper_AnimationCompleted);
                        radialAnimationHelper.StopRadialAnimation();
                        radialAnimationHelper = null;
                    }
                    (_chatDataContext.MainWindowSession[_interactionId] as ChatUtil).NotificationImageSource = "\\Pointel.Interactions.Chat;component\\Images\\Chat\\Chat.png";
                }
            }
            catch (Exception generalException)
            {
                logger.Error("NotificationPending() :" + generalException.ToString());
            }
        }

        void radialAnimationHelper_AnimationCompleted(string _interactionId)
        {
            try
            {
                if (_chatDataContext.MainWindowSession.ContainsKey(_interactionId))
                    (_chatDataContext.MainWindowSession[_interactionId] as ChatUtil).NotificationImageSource = "\\Pointel.Interactions.Chat;component\\Images\\Chat\\Chat.Pending.Notification.gif";
            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred as radialAnimationHelper_AnimationCompleted() : " + generalException.ToString());
            }
        }

        private bool isAllDigits(string phone)
        {
            if (phone.Length > 3 && phone.Length <= _chatDataContext.DialpadDigits)
                return phone.All(Char.IsDigit);
            else
                return false;
        }

        private void startSplash(string interactionID, string personName, string message)
        {
            var windows = Application.Current.Windows.OfType<Window>().Where(x => x.Name == "ChatWindow");
            foreach (var win in windows)
            {
                if (win != null && win.Tag.ToString() == interactionID)
                {
                    winInteropHelper = null;
                    winInteropHelper = new WindowInteropHelper(win);
                    FlashWindow.Start(winInteropHelper.Handle);
                    if (!win.IsActive)
                    {
                        _taskbarNotifier.DataContext = (_chatDataContext.MainWindowSession[interactionID] as ChatUtil);
                        (_chatDataContext.MainWindowSession[interactionID] as ChatUtil).TitleText = "New Message From " + personName;
                        (_chatDataContext.MainWindowSession[interactionID] as ChatUtil).NotifyMessage = message;
                        _taskbarNotifier.ReloadUI(true, interactionID, true);
                        _taskbarNotifier.Notify(0, false);
                    }
                    break;
                }
            }
        }
    }
}
