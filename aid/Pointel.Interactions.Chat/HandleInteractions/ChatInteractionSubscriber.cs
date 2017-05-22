namespace Pointel.Interactions.Chat.HandleInteractions
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls.Primitives;
    using System.Windows.Documents;
    using System.Windows.Media;

    using Genesyslab.Platform.Commons.Collections;
    using Genesyslab.Platform.Commons.Protocols;
    using Genesyslab.Platform.Contacts.Protocols.ContactServer;
    using Genesyslab.Platform.Contacts.Protocols.ContactServer.Events;
    using Genesyslab.Platform.OpenMedia.Protocols;
    using Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events;
    using Genesyslab.Platform.WebMedia.Protocols;

    using Pointel.Configuration.Manager;
    using Pointel.Interactions.Chat.ApplicationReader;
    using Pointel.Interactions.Chat.Core;
    using Pointel.Interactions.Chat.Core.General;
    using Pointel.Interactions.Chat.Helpers;
    using Pointel.Interactions.Chat.Settings;
    using Pointel.Interactions.Chat.WinForms;
    using Pointel.Interactions.IPlugins;
    using Pointel.Salesforce.Plugin;

    public class ChatInteractionSubscriber : IChatPlugin, IChatListener
    {
        #region Fields

        public Notifier _taskbarNotifier = null;

        private IChatListener chatListener;
        Paragraph errorMessageParagraph = new Paragraph();
        InteractionDataList interactionDataList = null;
        bool isContactServerSuccess = false;
        private Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");
        private ChatDataContext _chatDataContext = ChatDataContext.GetInstance();
        private ChatListener _chatListener = new ChatListener();
        private ChatUtil _chatUtil = null;
        private bool _isChatServerClosed = false;
        private bool _isChatServerOpened = false;

        #endregion Fields

        #region Delegates

        public delegate void IxnServerFailed();

        #endregion Delegates

        #region Events

        public static event IxnServerFailed _ixnFailed;

        #endregion Events

        #region Methods

        public static T IsWindowOpen<T>(string name = null)
            where T : Window
        {
            var windows = Application.Current.Windows.OfType<T>();
            return string.IsNullOrEmpty(name) ? windows.FirstOrDefault() : windows.FirstOrDefault(w => w.Name.Equals(name));
        }

        /// <summary>
        /// Binds the grid.
        /// </summary>
        /// <param name="interactionId">The interaction identifier.</param>
        public void BindGrid(string interactionId)
        {
            try
            {
                if (!_chatDataContext.MainWindowSession.ContainsKey(interactionId)) return;
                if ((_chatDataContext.MainWindowSession[interactionId] as ChatUtil).UserData.Count != 0)
                {
                    (_chatDataContext.MainWindowSession[interactionId] as ChatUtil).NotifyCaseData.Clear();
                    foreach (string key in (_chatDataContext.MainWindowSession[interactionId] as ChatUtil).UserData.Keys)
                    {
                        if ((_chatDataContext.MainWindowSession[interactionId] as ChatUtil).NotifyCaseData.Count(p => p.Key == key) == 0)
                        {
                            if (ConfigContainer.Instance().AllKeys.Contains("chat.enable.case-data-filter") && (((string)ConfigContainer.Instance().GetValue("chat.enable.case-data-filter")).ToLower().Equals("true")))
                            {
                                if ((_chatDataContext.LoadCaseDataFilterKeys != null && _chatDataContext.LoadCaseDataFilterKeys.Contains(key)) || (_chatDataContext.LoadCaseDataFilterKeys != null && _chatDataContext.LoadCaseDataKeys.Contains(key)))
                                    (_chatDataContext.MainWindowSession[interactionId] as ChatUtil).NotifyCaseData.Add(new ChatCaseData(key, (_chatDataContext.MainWindowSession[interactionId] as ChatUtil).UserData[key].ToString()));
                            }
                            else
                            {
                                (_chatDataContext.MainWindowSession[interactionId] as ChatUtil).NotifyCaseData.Add(new ChatCaseData(key, (_chatDataContext.MainWindowSession[interactionId] as ChatUtil).UserData[key].ToString()));
                            }
                        }
                    }
                    if ((_chatDataContext.MainWindowSession[interactionId] as ChatUtil).UserData.ContainsKey(_chatDataContext.DisPositionKeyName))
                        (_chatDataContext.MainWindowSession[interactionId] as ChatUtil).NotifyCaseData.Add(new ChatCaseData(_chatDataContext.DisPositionKeyName, (_chatDataContext.MainWindowSession[interactionId] as ChatUtil).UserData[_chatDataContext.DisPositionKeyName].ToString()));
                    if ((_chatDataContext.MainWindowSession[interactionId] as ChatUtil).UserData.ContainsKey(_chatDataContext.DispositionCollectionKeyName))
                        (_chatDataContext.MainWindowSession[interactionId] as ChatUtil).NotifyCaseData.Add(new ChatCaseData(_chatDataContext.DispositionCollectionKeyName, (_chatDataContext.MainWindowSession[interactionId] as ChatUtil).UserData[_chatDataContext.DispositionCollectionKeyName].ToString()));
                    (_chatDataContext.MainWindowSession[interactionId] as ChatUtil).NotifyCaseData = new ObservableCollection<IChatCaseData>((_chatDataContext.MainWindowSession[interactionId] as ChatUtil).NotifyCaseData.OrderBy(callData => callData.Key));
                }
            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred while do BindGrid() :" + generalException.ToString());
            }
        }

        /// <summary>
        /// Initializes the chat.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="comObject">The COM object.</param>
        /// <param name="applicationName">Name of the application.</param>
        /// <param name="placeName"></param>
        /// <param name="chatListener"></param>
        public void InitializeChat(string userName, Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.ConfService comObject, string applicationName,
            string placeName, IPluginCallBack calllbackListener)
        {
            var chatMedia = new ChatMedia();
            ComClass getAppDetails = new ComClass();
            try
            {
                LoadNotifier();
                chatListener = this;
                chatMedia.Subscribe(chatListener);
                _chatDataContext.UserName = userName;
                ChatDataContext.ComObject = comObject;
                _chatDataContext.ApplicationName = applicationName;
                ChatDataContext.messageToClientChat = calllbackListener;
                _chatDataContext.PlaceID = placeName;
                OutputValues output = chatMedia.InitializeChatMedia(_chatDataContext.UserName, ChatDataContext.ComObject, _chatDataContext.ApplicationName);
                if (output.MessageCode == "200")
                {
                    getAppDetails.GetAllValues();
                    chatMedia.CreateChatProtocol();
                }
            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred while do InitializeChat()" + generalException.ToString());
            }
            finally
            {
                chatMedia = null;
                getAppDetails = null;
            }
        }

        /// <summary>
        /// Loads the notifier.
        /// </summary>
        public void LoadNotifier()
        {
            try
            {
                _taskbarNotifier = new Notifier();
                _taskbarNotifier.StayOpenMilliseconds = 1000000000;
                _taskbarNotifier.OpeningMilliseconds = 1000;
                _taskbarNotifier.HidingMilliseconds = 500;
                _taskbarNotifier.Show();
                _taskbarNotifier.Hide();
                _taskbarNotifier.ChatAction += new Notifier.NotifChatAction(_taskbarNotifier_ChatAction);
                this._taskbarNotifier.DisplayState = Pointel.TaskbarNotifier.TaskbarNotifier.DisplayStates.Hiding;
            }
            catch (Exception generalException)
            {
                logger.Error("Error  occurred in LoadNotifier() : " + generalException.ToString());
            }
        }

        /// <summary>
        /// Notifies the chat interaction.
        /// </summary>
        /// <param name="message">The message.</param>
        public void NotifyChatInteraction(Genesyslab.Platform.Commons.Protocols.IMessage message)
        {
            string _interactionID = string.Empty;
            try
            {
                if (message != null)
                {
                    if (Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections.ContainsKey(IPlugins.Plugins.Salesforce))
                        ((ISFDCConnector)Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections[Pointel.Interactions.IPlugins.Plugins.Salesforce]).NotifyInteractionEvents(message);
                }
                else
                    return;
                Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                    try
                    {
                        switch (message.Id)
                        {
                            case EventInvite.MessageId:
                                EventInvite eventInvite = (EventInvite)message;
                                logger.Trace("Event Invite");
                                logger.Trace("------------");
                                logger.Trace("ChatInteractionSubscriber:" + eventInvite.ToString());
                                _interactionID = eventInvite.Interaction.InteractionId.ToString();
                                _chatUtil = new ChatUtil();
                                _chatUtil.InteractionID = eventInvite.Interaction.InteractionId.ToString();
                                if (!_chatDataContext.MainWindowSession.ContainsKey(_interactionID))
                                    _chatDataContext.MainWindowSession.Add(_interactionID, _chatUtil);
                                else
                                {
                                    if (eventInvite.VisibilityMode.ToString() == "Coach")
                                        checkSameInteractionWindowOpened(_interactionID);
                                    _chatDataContext.MainWindowSession[_interactionID] = _chatUtil;
                                }
                                //chatWindow = new ChatMainWindow(_chatDataContext.InteractionId, chatUtil);
                                //(_chatDataContext.MainWindowSession[_chatDataContext.InteractionId] as ChatUtil).InteractionID = _chatDataContext.InteractionId;
                                Style style = new Style(typeof(Paragraph));
                                style.Setters.Add(new Setter(Block.MarginProperty, new Thickness(2)));
                                if (!(_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).RTBDocument.Resources.Contains(style))
                                    (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).RTBDocument.Resources.Add(typeof(Paragraph), style);
                                if (eventInvite.Interaction.InteractionUserData.Contains("ContactId"))
                                    (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ContactID = eventInvite.Interaction.InteractionUserData["ContactId"].ToString();
                                KeyValueCollection InteractionData = eventInvite.Interaction.InteractionUserData;
                                (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).UserData.Clear();
                                (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).UserData = InteractionData;
                                getTheComment(_interactionID);
                                _chatUtil.TicketId = eventInvite.TicketId;
                                _chatUtil.ProxyId = eventInvite.ProxyClientId;
                                (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).IxnType = eventInvite.Interaction.InteractionType;

                                if ((_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).UserData.ContainsKey("Subject"))
                                    (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).Subject = (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).UserData["Subject"].ToString();
                                if (eventInvite.Interaction.InteractionUserData.Contains("RTargetAgentSelected"))
                                    _chatDataContext.AgentID = eventInvite.Interaction.InteractionUserData["RTargetAgentSelected"].ToString();
                                if (eventInvite.Contains("VisibilityMode"))
                                {
                                    if (eventInvite.VisibilityMode.ToString() == "Conference")
                                    {
                                        (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).TransferReleavePersonId = string.Empty;
                                        (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).IsChatConferenceClick = true;
                                    }
                                    else if (eventInvite.VisibilityMode.ToString() == "Coach")
                                    {
                                        (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).IxnType = "Consult";
                                        //  (_chatDataContext.MainWindowSession[_chatDataContext.InteractionId] as ChatUtil).ISConsultChatInitialized = true;
                                    }
                                    if (eventInvite.Parties != null)
                                    {
                                        (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatParties.Clear();
                                        KeyValueCollection users = eventInvite.Parties;
                                        if (users.Count > 0)
                                        {
                                            object[] arr = new object[users.Count];
                                            arr = users.AllValues;
                                            foreach (KeyValueCollection user in arr)
                                            {
                                                string agentID = string.Empty;
                                                string placeID = string.Empty;
                                                foreach (string key in user.Keys)
                                                {
                                                    if (key == "agent_id")
                                                        agentID = user["agent_id"].ToString();
                                                    if (key == "place_id")
                                                        placeID = user["place_id"].ToString();
                                                }
                                                if (!string.IsNullOrEmpty(agentID) && !string.IsNullOrEmpty(placeID))
                                                    (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatParties.Add(agentID, placeID);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (eventInvite.Parties != null)
                                    {
                                        (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatParties.Clear();
                                        KeyValueCollection users = eventInvite.Parties;
                                        if (users.Count > 0)
                                        {
                                            object[] arr = new object[users.Count];
                                            arr = users.AllValues;
                                            foreach (KeyValueCollection user in arr)
                                            {
                                                foreach (string key in user.Keys)
                                                {
                                                    if (key == "agent_id")
                                                        (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).TransferReleavePersonId = user["agent_id"].ToString();
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).TransferReleavePersonId = string.Empty;
                                    }
                                }
                                if ((_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).UserData.Count > 0)
                                {
                                    if ((_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).NotifyCaseData.Count > 0)
                                    {
                                        (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).NotifyCaseData.Clear();
                                    }

                                    foreach (string pair in (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).UserData.Keys)
                                    {
                                        if ((_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).NotifyCaseData.Count(p => p.Key == pair.ToString()) == 0)
                                        {
                                            if (ConfigContainer.Instance().AllKeys.Contains("chat.enable.case-data-filter") && (((string)ConfigContainer.Instance().GetValue("chat.enable.case-data-filter")).ToLower().Equals("true")))
                                            {
                                                if ((_chatDataContext.LoadCaseDataFilterKeys != null && _chatDataContext.LoadCaseDataFilterKeys.Contains(pair.ToString()))
                                                   || (_chatDataContext.LoadCaseDataFilterKeys != null && _chatDataContext.LoadCaseDataKeys.Contains(pair.ToString())))
                                                    (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).NotifyCaseData.Add(new ChatCaseData(pair.ToString(), (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).UserData[pair].ToString()));
                                            }
                                            else
                                            {
                                                (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).NotifyCaseData.Add(new ChatCaseData(pair.ToString(), (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).UserData[pair].ToString()));
                                            }
                                        }
                                    }
                                    if ((_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).NotifyCaseData.Count > 0)
                                    {
                                        if (!Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections.ContainsKey(Plugins.Contact) || string.IsNullOrEmpty((_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ContactID))
                                        {
                                            assignContactName(_interactionID);
                                        }
                                        if (!isContactServerSuccess)
                                        {
                                            assignContactName(_interactionID);
                                        }
                                        int indexofSubject = (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).NotifyCaseData.IndexOf((_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).NotifyCaseData.Where(p => p.Key.ToLower().Trim().ToString() == "subject").FirstOrDefault());
                                        if (indexofSubject >= 0)
                                            (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).Subject = (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).NotifyCaseData[indexofSubject].Value.ToString();
                                    }
                                    if (((string)ConfigContainer.Instance().GetValue("chat.enable.auto-answer")).ToLower().Equals("true") && (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).IxnType != "Consult")
                                    {
                                        //Get auto answer timer value from CME and show notifier when timer > 0
                                        int seconds = 0;
                                        int.TryParse(ConfigContainer.Instance().AllKeys.Contains("chat.auto-answer.timer") ?
                                            ((string)ConfigContainer.Instance().GetValue("chat.auto-answer.timer")) : "0", out seconds);

                                        if (seconds > 0)
                                        {
                                            _taskbarNotifier.objiNotifier = (TaskbarNotifier.INotifier)_taskbarNotifier;
                                            _taskbarNotifier.isAutoAnswerTimer = true;
                                            _taskbarNotifier.StayOpenMilliseconds = seconds * 1000;
                                            _taskbarNotifier.OpeningMilliseconds = 1000;
                                            _taskbarNotifier.HidingMilliseconds = 500;

                                            _taskbarNotifier.DataContext = (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil);
                                            _taskbarNotifier.PlayTone();
                                            // Show or Hide reject button when email.auto-answer is set to true and the value of email.auto-answer.timer > 0
                                            if (ConfigContainer.Instance().AllKeys.Contains("chat.enable.auto-answer-reject") &&
                                                ((string)ConfigContainer.Instance().GetValue("chat.enable.auto-answer-reject")).ToLower().Equals("true") &&
                                                ConfigContainer.Instance().AllKeys.Contains("chat.enable.interaction-notify-reject") &&
                                             ConfigContainer.Instance().GetAsBoolean("chat.enable.interaction-notify-reject"))
                                            {
                                                _taskbarNotifier.ReloadUI(true, _interactionID, false);
                                            }
                                            else
                                            {
                                                _taskbarNotifier.ReloadUI(false, _interactionID, false);
                                            }
                                            _taskbarNotifier.Notify(0, false);
                                        }
                                        else
                                        {
                                            _taskbarNotifier.objiNotifier = null;
                                            _taskbarNotifier.isAutoAnswerTimer = false;
                                            _taskbarNotifier.StayOpenMilliseconds = 1000000000;
                                            _taskbarNotifier.OpeningMilliseconds = 1000;
                                            _taskbarNotifier.HidingMilliseconds = 500;
                                            _taskbarNotifier.btnRight.Visibility = Visibility.Visible;
                                            //Auto Answer
                                            AutoAcceptInteraction(_interactionID);

                                        }
                                        //Thread chatAutoAnswerThread = new Thread(new ThreadStart(AutoAnswer));
                                        //chatAutoAnswerThread.Start();
                                    }
                                    else
                                    {
                                        logger.Info("-----------Call Notify UI----------");
                                        _taskbarNotifier.DataContext = (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil);
                                        _taskbarNotifier.PlayTone();
                                        if (ConfigContainer.Instance().AllKeys.Contains("chat.enable.interaction-notify-reject") &&
                                             ConfigContainer.Instance().GetAsBoolean("chat.enable.interaction-notify-reject"))
                                            _taskbarNotifier.ReloadUI(true, _interactionID, false);
                                        else
                                            _taskbarNotifier.ReloadUI(false, _interactionID, false);
                                        _taskbarNotifier.Notify(0, false);
                                    }
                                }
                                break;
                            case EventAgentInvited.MessageId:
                                EventAgentInvited eventAgentInvited = (EventAgentInvited)message;
                                logger.Trace("ChatInteractionSubscriber:" + eventAgentInvited.ToString());
                                break;
                            case EventRevoked.MessageId:
                                EventRevoked eventRevoked = (EventRevoked)message;
                                logger.Trace("ChatInteractionSubscriber:" + eventRevoked.ToString());
                                this._taskbarNotifier.objiNotifier = null;
                                this._taskbarNotifier.StopTone();
                                this._taskbarNotifier.stayOpenTimer.Stop();
                                this._taskbarNotifier.DisplayState = Pointel.TaskbarNotifier.TaskbarNotifier.DisplayStates.Hiding;
                                this._taskbarNotifier.ForceHidden();
                                _chatUtil = null;
                                _interactionID = eventRevoked.Interaction.InteractionId;
                                if (!_chatDataContext.MainWindowSession.ContainsKey(_interactionID))
                                    return;
                                (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatConsultPersonStatusInfo.Clear();
                                (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatPersonsStatusInfo.Clear();
                                if (_chatDataContext.MainWindowSession.ContainsKey(eventRevoked.Interaction.InteractionId))
                                {
                                    _chatDataContext.MainWindowSession[_interactionID] = null;
                                    _chatDataContext.MainWindowSession.Remove(_interactionID);
                                }
                                break;
                            case EventPropertiesChanged.MessageId:
                                EventPropertiesChanged eventPropertiesChanged = (EventPropertiesChanged)message;
                                logger.Trace("ChatInteractionSubscriber:" + eventPropertiesChanged.ToString());
                                _interactionID = eventPropertiesChanged.Interaction.InteractionId;
                                KeyValueCollection UpdatedData = eventPropertiesChanged.Interaction.InteractionUserData;
                                if (!_chatDataContext.MainWindowSession.ContainsKey(_interactionID))
                                    return;
                                (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).IsDispositionSelected = false;
                                if (UpdatedData != null)
                                    if (UpdatedData.AllKeys.Contains(_chatDataContext.DisPositionKeyName) || UpdatedData.AllKeys.Contains(_chatDataContext.DispositionCollectionKeyName))
                                        if (!string.IsNullOrEmpty(UpdatedData[_chatDataContext.DisPositionKeyName].ToString()) || !string.IsNullOrEmpty(UpdatedData[_chatDataContext.DispositionCollectionKeyName].ToString()))
                                        {
                                            Dictionary<string, string> dispositionTree = new Dictionary<string, string>();
                                            if (_chatDataContext.DispositionObjCollection.Value != null)
                                            {
                                                var dispositionObject = (Pointel.Interactions.DispositionCodes.UserControls.Disposition)
                                                                _chatDataContext.DispositionObjCollection.Value;
                                                if (UpdatedData.ContainsKey(_chatDataContext.DispositionCollectionKeyName))
                                                {
                                                    if (!string.IsNullOrEmpty(UpdatedData[_chatDataContext.DispositionCollectionKeyName].ToString()))
                                                    {
                                                        (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).IsDispositionSelected = true;
                                                        dispositionTree = UpdatedData[_chatDataContext.DispositionCollectionKeyName].ToString().Split(';').Select(s => s.Split(':')).ToDictionary(a => a[0].Trim().ToString(), a => a[1].Trim().ToString());
                                                    }
                                                    else
                                                        (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).IsDispositionSelected = false;
                                                }
                                                if (UpdatedData.ContainsKey(_chatDataContext.DisPositionKeyName))
                                                {
                                                    if (!string.IsNullOrEmpty(UpdatedData[_chatDataContext.DisPositionKeyName].ToString()) && UpdatedData[_chatDataContext.DisPositionKeyName].ToString() != "None")
                                                    {
                                                        (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).IsDispositionSelected = true;
                                                        dispositionTree.Add(_chatDataContext.DisPositionKeyName, UpdatedData[_chatDataContext.DisPositionKeyName].ToString());
                                                    }
                                                    else
                                                        (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).IsDispositionSelected = false;
                                                }
                                                if (dispositionTree.Count > 0)
                                                    dispositionObject.ReLoadDispositionCodes(dispositionTree, (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).InteractionID);
                                                dispositionTree.Clear();
                                                dispositionTree = null;
                                            }
                                        }
                                //if (UpdatedData.ContainsKey("completeTransfer") || UpdatedData.ContainsKey("completeConference"))
                                //{
                                //    KeyValueCollection caseData = new KeyValueCollection();
                                //    var chatMedia = new ChatMedia();
                                //    var output = OutputValues.GetInstance();
                                //    string _operationType = string.Empty;
                                //    if (UpdatedData.ContainsKey("completeTransfer"))
                                //        _operationType = "completeTransfer";
                                //    else
                                //        _operationType = "completeConference";
                                //    caseData.Add(_operationType, UpdatedData[_operationType].ToString());

                                //    output = chatMedia.DoLeaveInteractionFromConference(_interactionID, (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ProxyId, UpdatedData[_operationType].ToString());
                                //    if (output.MessageCode == "200")
                                //    {
                                //        output = chatMedia.DoDeleteCaseDataProperties((_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).InteractionID, (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ProxyId, caseData);
                                //        if (output.MessageCode == "200" && (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).UserData.ContainsKey(_operationType))
                                //        {
                                //            (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).UserData.Remove(_operationType);
                                //            int position1 = (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).NotifyCaseData.IndexOf((_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).NotifyCaseData.Where(p => p.Key == _operationType).FirstOrDefault());
                                //            _chatUtil.NotifyCaseData.RemoveAt(position1);
                                //        }
                                //    }
                                //}
                                (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).UserData = UpdatedData;
                                BindGrid(_interactionID);
                                getTheComment(_interactionID);
                                break;
                            case EventPartyAdded.MessageId:
                                EventPartyAdded eventPartyAdded = (EventPartyAdded)message;
                                logger.Trace("ChatInteractionSubscriber:" + eventPartyAdded.ToString());
                                string userID = string.Empty;
                                _interactionID = eventPartyAdded.Interaction.InteractionId;
                                if (eventPartyAdded.Operation.ToString() == "Transfer")
                                {

                                }
                                else if (eventPartyAdded.Operation.ToString() == "Conference")
                                {
                                    if (!_chatDataContext.MainWindowSession.ContainsKey(_interactionID)) return;
                                    (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatPersonStatusIcon = _chatDataContext.GetBitmapImage(new Uri("/Pointel.Interactions.Chat;component/Images/Chat/Chat.png", UriKind.Relative));
                                    (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).IsChatConferenceClick = true;
                                    if (eventPartyAdded.Party.VisibilityMode.ToString() == "Coach")
                                    {
                                        var item = (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatConsultPersonStatusInfo.FirstOrDefault(p => p.AgentID == eventPartyAdded.Party.AgentId);
                                        if (item != null)
                                        {
                                            int index = (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatConsultPersonStatusInfo.IndexOf(item);
                                            item.PlaceID = eventPartyAdded.Party.PlaceId.ToString();
                                            (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatConsultPersonStatusInfo[index] = item;
                                        }
                                        else
                                        {
                                            (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatConsultPersonStatusInfo.Add(new ChatPersonsStatus(eventPartyAdded.Party.AgentId.ToString(), eventPartyAdded.Party.PlaceId.ToString(), eventPartyAdded.Party.Name.ToString(), (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatPersonStatusIcon, "Connected"));
                                        }
                                    }
                                    else
                                    {
                                        var item = (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatPersonsStatusInfo.FirstOrDefault(p => p.AgentID == eventPartyAdded.Party.AgentId);
                                        if (item != null)
                                        {
                                            int index = (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatPersonsStatusInfo.IndexOf(item);
                                            item.PlaceID = eventPartyAdded.Party.PlaceId.ToString();
                                            (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatPersonsStatusInfo[index] = item;
                                        }
                                        else
                                        {
                                            (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatPersonsStatusInfo.Add(new ChatPersonsStatus(eventPartyAdded.Party.AgentId.ToString(), eventPartyAdded.Party.PlaceId.ToString(), eventPartyAdded.Party.Name.ToString(), (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatPersonStatusIcon, "Connected"));
                                        }
                                    }
                                    if (eventPartyAdded.Party != null)
                                    {
                                        if (!(_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatParties.ContainsKey(eventPartyAdded.Party.AgentId))
                                            (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatParties.Add(eventPartyAdded.Party.AgentId, eventPartyAdded.Party.PlaceId);
                                    }
                                    getTheComment(_interactionID);
                                }
                                break;
                            case EventPartyRemoved.MessageId:
                                EventPartyRemoved eventPartyRemoved = (EventPartyRemoved)message;
                                logger.Trace("ChatInteractionSubscriber:" + eventPartyRemoved.ToString());
                                _interactionID = eventPartyRemoved.Interaction.InteractionId;
                                if (eventPartyRemoved.Operation.ToString() == "Conference")
                                {

                                }
                                else if (eventPartyRemoved.Operation.ToString() == "Transfer")
                                {

                                }
                                break;
                        }
                    }
                    catch (Exception generalException)
                    {
                        logger.Error("Error occurred while get NotifyChatInteraction()" + generalException.ToString());
                    }
                }));
            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred while get NotifyChatInteraction()" + generalException.ToString());
            }
        }

        /// <summary>
        /// Notifies the chat media events.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="AgentID">The agent identifier.</param>
        public void NotifyChatMediaEvents(IMessage message)
        {
            try
            {
                if (_chatListener != null)
                    _chatListener.Chat_Listener(message);
            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred while NotifyChatMediaEvents() : " + generalException.ToString());
            }
        }

        /// <summary>
        /// Notifies the chat protocol.
        /// </summary>
        /// <param name="chatprotocol">The chatprotocol.</param>
        /// <param name="nickName">Name of the nick.</param>
        public void NotifyChatProtocol(BasicChatProtocol chatprotocol, string nickName)
        {
            try
            {
                // ChatDataContext.ChatProtocol = chatprotocol;
                _chatDataContext.AgentNickName = nickName;
            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred while NotifyChatProtocol() : " + generalException.ToString());
            }
        }

        /// <summary>
        /// Notifies the state of the chat.
        /// </summary>
        public void NotifyChatState(string chatProtocolState)
        {
            Application.Current.Dispatcher.Invoke((System.Action)(delegate
            {
                try
                {
                    if (chatProtocolState == "Closed" && !_isChatServerClosed)
                    {
                        _isChatServerClosed = true;
                        errorMessageParagraph.Inlines.Clear();
                        errorMessageParagraph.FontStyle = FontStyles.Italic;
                        errorMessageParagraph.Inlines.Add(new Run("Lost connection to chat server, trying to reconnect... "));
                        errorMessageParagraph.Foreground = Brushes.LightGray;
                        errorMessageParagraph.FontWeight = FontWeights.Bold;

                        foreach (string interactionId in _chatDataContext.MainWindowSession.Keys)
                        {
                            (_chatDataContext.MainWindowSession[interactionId] as ChatUtil).RTBDocument.Blocks.Add(errorMessageParagraph);
                            (_chatDataContext.MainWindowSession[interactionId] as ChatUtil).SendChatWindowRowHeight = new GridLength(0);
                            (_chatDataContext.MainWindowSession[interactionId] as ChatUtil).ChatToolBarRowHeight = new GridLength(0);
                        }
                        //if (ChatDataContext.messageToClientChat != null)
                        //{
                        //    ChatDataContext.messageToClientChat.PluginErrorMessage(IPlugins.PluginType.Chat, "The channel chat is out of service.");
                        //}
                    }
                    if (chatProtocolState == "Opened" && !_isChatServerOpened)
                    {
                        _isChatServerOpened = true;
                        foreach (string interactionId in _chatDataContext.MainWindowSession.Keys)
                        {
                            (_chatDataContext.MainWindowSession[interactionId] as ChatUtil).RTBDocument.Blocks.Remove(errorMessageParagraph);
                            (_chatDataContext.MainWindowSession[interactionId] as ChatUtil).SendChatWindowRowHeight = GridLength.Auto;
                            (_chatDataContext.MainWindowSession[interactionId] as ChatUtil).ChatToolBarRowHeight = GridLength.Auto;
                        }
                        //if (ChatDataContext.messageToClientChat != null)
                        //{
                        //    ChatDataContext.messageToClientChat.PluginErrorMessage(IPlugins.PluginType.Chat, "The channel chat is back in service.");
                        //}
                    }
                }
                catch (Exception generalException)
                {
                    logger.Error("Error occurred while NotifyChatState() : " + generalException.ToString());
                }
            }));
        }

        /// <summary>
        /// Notifies the interaction protocol.
        /// </summary>
        /// <param name="ixnProtocol">The ixn protocol.</param>
        public void NotifyInteractionProtocol(InteractionServerProtocol ixnProtocol)
        {
            try
            {
                if (ixnProtocol != null)
                    ChatDataContext.ixnServerProtocol = ixnProtocol;
            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred while get NotifyInteractionProtocol()" + generalException.ToString());
            }
        }

        /// <summary>
        /// Notifies the state of the interaction server.
        /// </summary>
        /// <param name="isConnected">if set to <c>true</c> [is connected].</param>
        /// <param name="proxyClientID">The proxy client unique identifier.</param>
        public void NotifyIXNState(bool isConnected, int? proxyClientID = null)
        {
            try
            {
                if (isConnected && proxyClientID != null)
                {
                    foreach (string interactionId in _chatDataContext.MainWindowSession.Keys)
                    {
                        (_chatDataContext.MainWindowSession[interactionId] as ChatUtil).ProxyId = (int)proxyClientID;
                    }
                }
                else
                {
                    _ixnFailed.Invoke();
                }
            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred in NotifyIXNState() : " + generalException.ToString());
            }
        }

        public void NotifyPlace(string place)
        {
            _chatDataContext.PlaceID = place;
        }

        /// <summary>
        /// Notifies the voice media status.
        /// </summary>
        /// <param name="isVoiceEnabled">if set to <c>true</c> [is voice enabled].</param>
        public void NotifyVoiceMediaStatus(bool isVoiceEnabled)
        {
            try
            {
                ChatDataContext.GetInstance().IsAvailableVoiceMedia = isVoiceEnabled;
                if (!isVoiceEnabled)
                    foreach (string interactionId in _chatDataContext.MainWindowSession.Keys)
                    {
                        (_chatDataContext.MainWindowSession[interactionId] as ChatUtil).VoiceConsultImageSource = _chatDataContext.GetBitmapImage(new Uri(_chatDataContext.Imagepath + "\\Chat\\Chat.Consult.Call.Disable.png", UriKind.Relative));
                        (_chatDataContext.MainWindowSession[interactionId] as ChatUtil).IsEnableVoiceConsult = false;
                        (_chatDataContext.MainWindowSession[interactionId] as ChatUtil).EnableCallMenuitems = false;
                    }
                else
                {
                    foreach (string interactionId in _chatDataContext.MainWindowSession.Keys)
                    {
                        if (!(_chatDataContext.MainWindowSession[interactionId] as ChatUtil).IsEnableDone)
                        {
                            (_chatDataContext.MainWindowSession[interactionId] as ChatUtil).VoiceConsultImageSource = _chatDataContext.GetBitmapImage(new Uri(_chatDataContext.Imagepath + "\\Chat\\Chat.Consult.Call.png", UriKind.Relative));
                            (_chatDataContext.MainWindowSession[interactionId] as ChatUtil).IsEnableVoiceConsult = true;
                            (_chatDataContext.MainWindowSession[interactionId] as ChatUtil).EnableCallMenuitems = true;
                        }
                    }
                }
            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred in NotifyVoiceMediaStatus() : " + generalException.ToString());
            }
        }

        /// <summary>
        /// Accepts the interaction.
        /// </summary>
        private void AcceptInteraction(string interactionId)
        {
            try
            {
                AutoAcceptInteraction(interactionId);
                _taskbarNotifier.ForceHidden();
            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred while do AcceptInteraction() :" + generalException.ToString());
            }
        }

        /// <summary>
        /// Assigns the name of the contact.
        /// </summary>
        private void assignContactName(string interactionId)
        {
            try
            {
                if (!_chatDataContext.MainWindowSession.ContainsKey(interactionId)) return;
                int indexofFirstName = (_chatDataContext.MainWindowSession[interactionId] as ChatUtil).NotifyCaseData.IndexOf((_chatDataContext.MainWindowSession[interactionId] as ChatUtil).NotifyCaseData.Where(p => p.Key.ToLower().Trim().ToString() == "firstname").FirstOrDefault());
                (_chatDataContext.MainWindowSession[interactionId] as ChatUtil).TitleText = (_chatDataContext.MainWindowSession[interactionId] as ChatUtil).NotifyCaseData[indexofFirstName].Value.ToString();
                (_chatDataContext.MainWindowSession[interactionId] as ChatUtil).ChatFromPersonName = (_chatDataContext.MainWindowSession[interactionId] as ChatUtil).NotifyCaseData[indexofFirstName].Value.ToString();
                int indexofLastName = (_chatDataContext.MainWindowSession[interactionId] as ChatUtil).NotifyCaseData.IndexOf((_chatDataContext.MainWindowSession[interactionId] as ChatUtil).NotifyCaseData.Where(p => p.Key.ToLower().Trim().ToString() == "lastname").FirstOrDefault());
                (_chatDataContext.MainWindowSession[interactionId] as ChatUtil).TitleText = (_chatDataContext.MainWindowSession[interactionId] as ChatUtil).TitleText + " " + (_chatDataContext.MainWindowSession[interactionId] as ChatUtil).NotifyCaseData[indexofLastName].Value.ToString() + " - Agent Interaction Desktop";
                (_chatDataContext.MainWindowSession[interactionId] as ChatUtil).ChatFromPersonName = (_chatDataContext.MainWindowSession[interactionId] as ChatUtil).ChatFromPersonName + " " + (_chatDataContext.MainWindowSession[interactionId] as ChatUtil).NotifyCaseData[indexofLastName].Value.ToString();
            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred while do assignContactName() :" + generalException.ToString());
            }
        }

        /// <summary>
        /// Automatics the answer.
        /// </summary>
        private void AutoAcceptInteraction(string interactionId)
        {
            var chatMedia = new ChatMedia();
            var output = OutputValues.GetInstance();
            try
            {
                if (!_chatDataContext.MainWindowSession.ContainsKey(interactionId)) return;
                output = chatMedia.DoAcceptChatInteraction((_chatDataContext.MainWindowSession[interactionId] as ChatUtil).TicketId, interactionId, (_chatDataContext.MainWindowSession[interactionId] as ChatUtil).ProxyId, ChatDataContext.ixnServerProtocol);
                if (output.MessageCode == "200")
                {
                    output = chatMedia.CheckChatServerStatus();
                    if (output.MessageCode != "200")
                        output = chatMedia.CreateChatProtocol();
                    if (output.MessageCode == "200")
                    {
                        if ((_chatDataContext.MainWindowSession[interactionId] as ChatUtil).IxnType == "Consult")
                        {
                            output = chatMedia.DOConsultChatJoin(interactionId, (_chatDataContext.MainWindowSession[interactionId] as ChatUtil).Subject, _chatDataContext.ChatWelcomeMessage, (_chatDataContext.MainWindowSession[interactionId] as ChatUtil).UserData);
                            if (output.MessageCode == "200")
                                goto common;
                        }
                        else
                        {
                            output = chatMedia.DOChatJoin(interactionId, (_chatDataContext.MainWindowSession[interactionId] as ChatUtil).Subject, _chatDataContext.ChatWelcomeMessage, (_chatDataContext.MainWindowSession[interactionId] as ChatUtil).UserData);
                            if (output.MessageCode == "200")
                                goto common;
                        }
                    common:
                        _taskbarNotifier.ForceHidden();
                        (_chatDataContext.MainWindowSession[interactionId] as ChatUtil).ChatPersonsStatusInfo.Clear();
                        (_chatDataContext.MainWindowSession[interactionId] as ChatUtil).ChatConsultPersonStatusInfo.Clear();
                        ChatMainWindow chatWindow = new ChatMainWindow(interactionId, _chatUtil);
                        chatWindow.Show();
                        (_chatDataContext.MainWindowSession[interactionId] as ChatUtil).ChatPersonStatusIcon = _chatDataContext.GetBitmapImage(new Uri(_chatDataContext.Imagepath + "\\Chat\\Chat.png", UriKind.Relative));
                        (_chatDataContext.MainWindowSession[interactionId] as ChatUtil).ChatPersonsStatusInfo.Add(new ChatPersonsStatus(string.Empty, string.Empty, (_chatDataContext.MainWindowSession[interactionId] as ChatUtil).ChatFromPersonName, (_chatDataContext.MainWindowSession[interactionId] as ChatUtil).ChatPersonStatusIcon, "Connected"));
                        (_chatDataContext.MainWindowSession[interactionId] as ChatUtil).IsOnChatInteraction = true;
                        //Application.Current.Dispatcher.Invoke((System.Action)(delegate
                        //{
                        ThreadPool.QueueUserWorkItem(new WaitCallback(delegate(object state) { _chatListener.Chat_Session(output.RequestJoinIMessage, interactionId, _taskbarNotifier); }), null);
                        //}));
                    }
                }
                else
                {
                    if (ChatDataContext.messageToClientChat != null)
                    {
                        ChatDataContext.messageToClientChat.PluginErrorMessage(IPlugins.PluginType.Chat, output.Message.ToString());
                    }
                    ProcessBeforeClosing(interactionId);
                }
            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred while do AutoAcceptInteraction() :" + generalException.ToString());
            }
            finally
            {
                chatMedia = null;
            }
        }

        private void checkSameInteractionWindowOpened(string interactionId)
        {
            try
            {
                var window = IsWindowOpen<Window>("ChatWindow");
                if (window != null && window is ChatMainWindow)
                {
                    ChatMainWindow chatWin = (ChatMainWindow)window;
                    if (chatWin.Tag.ToString() == interactionId)
                    {
                        chatWin.btnChatDone.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// Gets the name of the contact.
        /// </summary>
        /// <param name="contactID">The contact identifier.</param>
        /// <param name="interactionId">The interaction identifier.</param>
        private void GetContactName(string contactID, string interactionId)
        {
            try
            {
                if (!_chatDataContext.MainWindowSession.ContainsKey(interactionId)) return;
                string contactName = string.Empty;
                string firstName = string.Empty;
                string lastName = string.Empty;
                List<string> attributes = new List<string>();
                //attributes.Add("PhoneNumber");
                //attributes.Add("EmailAddress");
                attributes.Add("FirstName");
                attributes.Add("LastName");
                //attributes.Add("Title");
                IMessage message = ((IContactPlugin)Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections[Pointel.Interactions.IPlugins.Plugins.Contact]).GetAllAttributes(contactID, attributes);
                if (message != null)
                {
                    switch (message.Id)
                    {
                        case Genesyslab.Platform.Contacts.Protocols.ContactServer.Events.EventGetAttributes.MessageId:
                            Genesyslab.Platform.Contacts.Protocols.ContactServer.Events.EventGetAttributes eventGetAttributes = (Genesyslab.Platform.Contacts.Protocols.ContactServer.Events.EventGetAttributes)message;
                            if (eventGetAttributes != null)
                            {
                                logger.Info("------------RequestToGetAllAttributes-------------");
                                logger.Info("Contact ID  :" + contactID);
                                logger.Info("-------------------------------------------------------");
                                logger.Info(eventGetAttributes.ToString());
                                logger.Info("-------------------------------------------------------");
                                logger.Trace(eventGetAttributes.ToString());
                                AttributesList attributeList = eventGetAttributes.Attributes;
                                AttributesHeader firstNameHeader = attributeList.Cast<AttributesHeader>().Where(x => x.AttrName.Equals("FirstName")).SingleOrDefault();
                                if (firstNameHeader != null && firstNameHeader.AttributesInfoList.Count > 0)
                                {
                                    contactName += firstNameHeader.AttributesInfoList[0].AttrValue.ToString() + " ";
                                    firstName = firstNameHeader.AttributesInfoList[0].AttrValue.ToString().Trim();
                                }
                                AttributesHeader LastNameHeader = attributeList.Cast<AttributesHeader>().Where(x => x.AttrName.Equals("LastName")).SingleOrDefault();
                                if (LastNameHeader != null && LastNameHeader.AttributesInfoList.Count > 0)
                                {
                                    contactName += LastNameHeader.AttributesInfoList[0].AttrValue.ToString();
                                    lastName = LastNameHeader.AttributesInfoList[0].AttrValue.ToString().Trim();
                                }
                                (_chatDataContext.MainWindowSession[interactionId] as ChatUtil).TitleText = contactName + " - Agent Interaction Desktop";
                                (_chatDataContext.MainWindowSession[interactionId] as ChatUtil).ChatFromPersonName = contactName;
                            }
                            break;
                        case Genesyslab.Platform.Contacts.Protocols.ContactServer.Events.EventError.MessageId:
                            Genesyslab.Platform.Contacts.Protocols.ContactServer.Events.EventError eventError = (Genesyslab.Platform.Contacts.Protocols.ContactServer.Events.EventError)message;
                            string LoginErrorDescription = Convert.ToString(eventError.ErrorDescription);
                            logger.Trace(eventError.ToString());
                            break;
                    }
                }
            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred while do GetContactName() :" + generalException.ToString());
            }
        }

        /// <summary>
        /// Gets the inprogess interaction count.
        /// </summary>
        /// <param name="interactionId">The interaction identifier.</param>
        private void getInprogessInteractionCount(string interactionId)
        {
            try
            {
                if (Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections.ContainsKey(Plugins.Contact))
                {
                    if (!_chatDataContext.MainWindowSession.ContainsKey(interactionId)) return;
                    (_chatDataContext.MainWindowSession[interactionId] as ChatUtil).InProgressIXNCountVisibility = Visibility.Collapsed;
                    IMessage response = ((IContactPlugin)Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections[Pointel.Interactions.IPlugins.Plugins.Contact]).GetTotalInteractionCount((_chatDataContext.MainWindowSession[interactionId] as ChatUtil).ContactID, (_chatDataContext.MainWindowSession[interactionId] as ChatUtil).InteractionID);
                    if (response != null)
                    {
                        if (response != null && response.Id == EventCountInteractions.MessageId)
                        {
                            EventCountInteractions eventInteractionListGet = (EventCountInteractions)response;

                            if (eventInteractionListGet != null && !eventInteractionListGet.TotalCount.IsNull)
                            {
                                int inprogressIXNCount = 0;
                                inprogressIXNCount = (int)eventInteractionListGet.TotalCount.Value;
                                if (inprogressIXNCount > 0)
                                {
                                    (_chatDataContext.MainWindowSession[interactionId] as ChatUtil).InprogressInteractionCount = "(" + inprogressIXNCount + ")";
                                    if (inprogressIXNCount > 1)
                                        (_chatDataContext.MainWindowSession[interactionId] as ChatUtil).TotalInprogessIXNCount = inprogressIXNCount + " Interactions In Progress.";
                                    else
                                        (_chatDataContext.MainWindowSession[interactionId] as ChatUtil).TotalInprogessIXNCount = inprogressIXNCount + " Interaction In Progress.";
                                    (_chatDataContext.MainWindowSession[interactionId] as ChatUtil).InProgressIXNCountVisibility = Visibility.Visible;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred while do getInprogessInteractionCount() :" + generalException.ToString());
            }
        }

        /// <summary>
        /// Gets the recent interaction list.
        /// </summary>
        /// <param name="interactionId">The interaction identifier.</param>
        private void getRecentInteractionList(string interactionId)
        {
            try
            {
                if (Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections.ContainsKey(Plugins.Contact))
                {
                    if (!_chatDataContext.MainWindowSession.ContainsKey(interactionId)) return;
                    (_chatDataContext.MainWindowSession[interactionId] as ChatUtil).RecentInteraction.Clear();
                    List<string> attributes = new List<string>();
                    attributes.Add("MediaTypeId");
                    attributes.Add("StartDate");
                    attributes.Add("Subject");
                    (_chatDataContext.MainWindowSession[interactionId] as ChatUtil).RecentIXNListVisibility = Visibility.Collapsed;
                    IMessage response = ((IContactPlugin)Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections[Pointel.Interactions.IPlugins.Plugins.Contact]).GetInteractionList((_chatDataContext.MainWindowSession[interactionId] as ChatUtil).ContactID, ConfigContainer.Instance().TenantDbId, (_chatDataContext.MainWindowSession[interactionId] as ChatUtil).InteractionID, attributes);
                    if (response != null)
                    {
                        if (response != null && response.Id == EventInteractionListGet.MessageId)
                        {
                            EventInteractionListGet eventInteractionListGet = (EventInteractionListGet)response;

                            if (eventInteractionListGet != null && eventInteractionListGet.InteractionData != null)
                            {
                                interactionDataList = eventInteractionListGet.InteractionData;
                                if (interactionDataList.Count > 0 && interactionDataList != null)
                                {
                                    int remaining = 0;
                                    (_chatDataContext.MainWindowSession[interactionId] as ChatUtil).RecentIXNCount = "(" + (interactionDataList.Count) + ")";
                                    (_chatDataContext.MainWindowSession[interactionId] as ChatUtil).RecentInteractionCount = (interactionDataList.Count) + " Recent Interactions in last day:";
                                    for (int listCount = 0; listCount < interactionDataList.Count; listCount++)
                                    {
                                        if (listCount < 5)
                                        {
                                            var startDate = string.Empty;
                                            var mediaType = string.Empty;
                                            var subject = string.Empty;
                                            foreach (Genesyslab.Platform.Contacts.Protocols.ContactServer.Attribute item in interactionDataList[listCount].Attributes)
                                            {
                                                switch (item.AttributeName.ToLower())
                                                {
                                                    case "subject":
                                                        subject = Convert.ToString(item.AttributeValue);
                                                        break;
                                                    case "startdate":
                                                        startDate = Convert.ToString(Convert.ToDateTime(item.AttributeValue.ToString()));
                                                        break;
                                                    case "mediatypeid":
                                                        mediaType = Convert.ToString(item.AttributeValue);
                                                        break;
                                                }
                                            }
                                            (_chatDataContext.MainWindowSession[interactionId] as ChatUtil).RecentInteraction.Add(new RecentInteractions(mediaType, startDate, subject));
                                        }
                                        else
                                        {
                                            remaining++;
                                        }
                                    }
                                    if (remaining > 0)
                                        (_chatDataContext.MainWindowSession[interactionId] as ChatUtil).RemainingDetails = (remaining) + " more ...";
                                    (_chatDataContext.MainWindowSession[interactionId] as ChatUtil).RecentIXNListVisibility = Visibility.Visible;
                                }
                            }
                        }
                    }
                    attributes.Clear();
                    attributes = null;
                }
            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred while do getRecentInteractionList() :" + generalException.ToString());
            }
        }

        /// <summary>
        /// Gets the comment.
        /// </summary>
        /// <param name="interactionId">The interaction identifier.</param>
        private void getTheComment(string interactionId)
        {
            if (Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections.ContainsKey(Plugins.Contact))
            {
                try
                {
                    if (!_chatDataContext.MainWindowSession.ContainsKey(interactionId)) return;
                    IMessage response = ((IContactPlugin)Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections[Pointel.Interactions.IPlugins.Plugins.Contact]).GetInteractionContent(interactionId, false);

                    if (response != null)
                    {
                        switch (response.Id)
                        {
                            case Genesyslab.Platform.Contacts.Protocols.ContactServer.Events.EventGetInteractionContent.MessageId:
                                Genesyslab.Platform.Contacts.Protocols.ContactServer.Events.EventGetInteractionContent eventGetInteractionContent = (Genesyslab.Platform.Contacts.Protocols.ContactServer.Events.EventGetInteractionContent)response;
                                if (eventGetInteractionContent != null)
                                {
                                    logger.Info("------------RequestToGetInteractionContent-------------");
                                    logger.Info("InteractionId  :" + interactionId);
                                    logger.Info("-------------------------------------------------------");
                                    logger.Info(eventGetInteractionContent.ToString());
                                    logger.Info("-------------------------------------------------------");
                                    logger.Trace(eventGetInteractionContent.ToString());
                                    if (eventGetInteractionContent.InteractionAttributes.TheComment != null)
                                        (_chatDataContext.MainWindowSession[interactionId] as ChatUtil).InteractionNoteContent = eventGetInteractionContent.InteractionAttributes.TheComment.ToString();
                                    if (string.IsNullOrEmpty((_chatDataContext.MainWindowSession[interactionId] as ChatUtil).ContactID))
                                    {
                                        if (!string.IsNullOrEmpty(eventGetInteractionContent.InteractionAttributes.ContactId))
                                        {
                                            (_chatDataContext.MainWindowSession[interactionId] as ChatUtil).ContactID = eventGetInteractionContent.InteractionAttributes.ContactId;
                                        }
                                        else if (eventGetInteractionContent.InteractionAttributes.AllAttributes.ContainsKey("ContactId"))
                                        {
                                            (_chatDataContext.MainWindowSession[interactionId] as ChatUtil).ContactID = (eventGetInteractionContent.InteractionAttributes.AllAttributes["ContactId"].ToString() == null ? string.Empty : eventGetInteractionContent.InteractionAttributes.AllAttributes["ContactId"].ToString());
                                        }
                                        else
                                        {
                                            if (string.IsNullOrEmpty((_chatDataContext.MainWindowSession[interactionId] as ChatUtil).ContactID))
                                            {
                                                Dictionary<ContactDetails, string> _contactdetails = null;
                                                _contactdetails = ((IContactPlugin)Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections[Pointel.Interactions.IPlugins.Plugins.Contact]).GetContactId(ConfigContainer.Instance().TenantDbId.ToString(), MediaTypes.Chat, (_chatDataContext.MainWindowSession[interactionId] as ChatUtil).UserData);
                                                if (_contactdetails != null)
                                                {
                                                    (_chatDataContext.MainWindowSession[interactionId] as ChatUtil).ContactID = _contactdetails.ContainsKey(ContactDetails.ContactId) ? _contactdetails[ContactDetails.ContactId].ToString() : string.Empty;
                                                }
                                            }
                                        }
                                    }
                                    if (!string.IsNullOrEmpty(eventGetInteractionContent.InteractionAttributes.StartDate.ToString()))
                                        (_chatDataContext.MainWindowSession[interactionId] as ChatUtil).StartDate = eventGetInteractionContent.InteractionAttributes.StartDate.ToString();
                                    else
                                        (_chatDataContext.MainWindowSession[interactionId] as ChatUtil).StartDate = string.Empty;
                                    if (eventGetInteractionContent.InteractionAttributes.Timeshift != null)
                                        (_chatDataContext.MainWindowSession[interactionId] as ChatUtil).TimeShift = eventGetInteractionContent.InteractionAttributes.Timeshift.Value;
                                    else
                                        (_chatDataContext.MainWindowSession[interactionId] as ChatUtil).TimeShift = 0;
                                }
                                break;
                            case Genesyslab.Platform.Contacts.Protocols.ContactServer.Events.EventError.MessageId:
                                Genesyslab.Platform.Contacts.Protocols.ContactServer.Events.EventError eventError = (Genesyslab.Platform.Contacts.Protocols.ContactServer.Events.EventError)response;
                                if (eventError != null)
                                {
                                    string LoginErrorDescription = Convert.ToString(eventError.ErrorDescription);
                                    logger.Trace(eventError.ToString());
                                }
                                else
                                {
                                    logger.Warn("EventError Occurred while doing GetInteractionContent()");
                                }
                                break;
                        }
                        isContactServerSuccess = true;
                    }
                    else
                    {
                        isContactServerSuccess = false;
                    }
                    //get the contactID
                    if (string.IsNullOrEmpty((_chatDataContext.MainWindowSession[interactionId] as ChatUtil).ContactID))
                    {
                        Dictionary<ContactDetails, string> _contactdetails = null;
                        _contactdetails = ((IContactPlugin)Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections[Pointel.Interactions.IPlugins.Plugins.Contact]).GetContactId(ConfigContainer.Instance().TenantDbId.ToString(), MediaTypes.Chat, (_chatDataContext.MainWindowSession[interactionId] as ChatUtil).UserData);
                        if (_contactdetails != null)
                        {
                            (_chatDataContext.MainWindowSession[interactionId] as ChatUtil).ContactID = _contactdetails.ContainsKey(ContactDetails.ContactId) ? _contactdetails[ContactDetails.ContactId].ToString() : string.Empty;
                        }
                    }
                    if (!string.IsNullOrEmpty((_chatDataContext.MainWindowSession[interactionId] as ChatUtil).ContactID))
                    {
                        getInprogessInteractionCount(interactionId);
                        getRecentInteractionList(interactionId);
                        GetContactName((_chatDataContext.MainWindowSession[interactionId] as ChatUtil).ContactID, interactionId);
                    }
                }
                catch (Exception generalException)
                {
                    logger.Warn(generalException.ToString());
                }
            }
        }

        /// <summary>
        /// Processes the before closing.
        /// </summary>
        /// <param name="interactionId">The interaction identifier.</param>
        private void ProcessBeforeClosing(string interactionId)
        {
            System.Windows.Application.Current.Dispatcher.Invoke((Action)(delegate
            {
                try
                {
                    int count = 0;
                    foreach (var item in Application.Current.Windows)
                    {
                        if (item is ChatMainWindow)
                            if (_chatDataContext.MainWindowSession.ContainsKey((item as ChatMainWindow).Tag.ToString()))
                                count++;
                    }
                    if (count == 1 || count == 0)
                    {
                        if (ChatDataContext.messageToClientChat != null)
                            ChatDataContext.messageToClientChat.PluginInteractionStatus(IPlugins.PluginType.Chat, Pointel.Interactions.IPlugins.IXNState.Closed, true);
                        if (_chatDataContext.MainWindowSession.ContainsKey(interactionId))
                        {
                            (_chatDataContext.MainWindowSession[interactionId] as ChatUtil).IsOnChatInteraction = false;
                            _chatDataContext.MainWindowSession[interactionId] = null;
                            _chatDataContext.MainWindowSession.Remove(interactionId);
                        }
                    }
                }
                catch (Exception generalException)
                {
                    logger.Error("Error occurred while ProcessBeforeClosing() :" + generalException.ToString());
                }
            }));
        }

        /// <summary>
        /// Rejects the interaction.
        /// </summary>
        private void RejectInteraction(string interactionId)
        {
            try
            {
                if (!_chatDataContext.MainWindowSession.ContainsKey(interactionId)) return;
                ChatMedia chatMedia = new ChatMedia();
                OutputValues output = chatMedia.DoRejectChatInteraction((_chatDataContext.MainWindowSession[interactionId] as ChatUtil).TicketId, interactionId, (_chatDataContext.MainWindowSession[interactionId] as ChatUtil).ProxyId, ChatDataContext.ixnServerProtocol, (_chatDataContext.MainWindowSession[interactionId] as ChatUtil).UserData);
                if (output.MessageCode == "200")
                {
                    ProcessBeforeClosing(interactionId);
                    _taskbarNotifier.ForceHidden();
                }
            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred while do RejectInteraction() :" + generalException.ToString());
            }
        }

        /// <summary>
        /// _taskbars the notifier_ chat action.
        /// </summary>
        /// <param name="action">The action.</param>
        void _taskbarNotifier_ChatAction(string action, string interactionId)
        {
            if (_taskbarNotifier != null)
            {
                _taskbarNotifier.StopTone();
                _taskbarNotifier.objiNotifier = null;
            }
            if (action == "Reject")
            {
                RejectInteraction(interactionId);
            }
            else
            {
                AcceptInteraction(interactionId);
            }
        }

        #endregion Methods
    }
}