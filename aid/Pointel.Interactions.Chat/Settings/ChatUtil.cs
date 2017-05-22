using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using Pointel.Interactions.Chat.Helpers;
using System.Collections.Generic;
using Genesyslab.Platform.Commons.Collections;

namespace Pointel.Interactions.Chat.Settings
{
    public class ChatUtil : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        #region INotifyPropertyChange

        /// <summary>
        /// Raises the property changed.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action">The action.</param>
        protected void RaisePropertyChanged<T>(Expression<Func<T>> action)
        {
            var propertyName = GetPropertyName(action);
            RaisePropertyChanged(propertyName);
        }

        /// <summary>
        /// Gets the name of the property.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action">The action.</param>
        /// <returns></returns>
        private static string GetPropertyName<T>(Expression<Func<T>> action)
        {
            var expression = (MemberExpression)action.Body;
            var propertyName = expression.Member.Name;
            return propertyName;
        }

        /// <summary>
        /// Raises the property changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        private void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion INotifyPropertyChange


        private string _dialedNumbers = string.Empty;
        private double _modifiedTextSize = 0;
        private System.Windows.Media.Brush _mainBorderBrush;
        private bool _enableCallMenuitems;
        private bool _enableDelConfMenuitems;
        private bool _enableEmailCallMenuitems;
        private string _agentID = string.Empty;
        private string _placeID = string.Empty;
        private string _contactID = string.Empty;
        private string _interactionId = string.Empty;
        private GridLength _chatContentRowHeight = new GridLength();
        private GridLength _errorRowHeight = new GridLength();
        private GridLength _caseExpanderRowHight = new GridLength();
        private GridLength _chatAgentsStatusRowHight = new GridLength();
        private GridLength _consultChatRowHight = new GridLength();
        private GridLength _consultChatWindowRowHeight = new GridLength();
        private GridLength _sendChatWindowRowHeight = new GridLength();
        private GridLength _sendchatConsultWindowRowHeight = new GridLength();
        private GridLength _chatToolBarRowHeight = new GridLength();
        private FlowDocument _rtbDocument = new FlowDocument();
        private FlowDocument _rtbConsultDocument = new FlowDocument();
        private ObservableCollection<IChatCaseData> _notifyCaseData = new ObservableCollection<IChatCaseData>();
        private string _sessionID;
        private string _titleText;
        private string _chatWindowTitleText;

        private string _chatConsultWindowTitleText;
        private string _inprogressInteractionCount;
        private string _chatMessageText;
        private string _chatConsultMessageText;
        private string _chatNoticeText;
        private string _chatTypeStatus;

        private ImageSource _btnArrowImageSource;
        private ImageSource _transImageSource;
        private ImageSource _confImageSource;
        private ImageSource _releaseImageSource;
        private ImageSource _doneImageSource;
        private ImageSource _voiceConsultImageSource;
        private ImageSource _consultChatImageSource;
        private ImageSource _casedataImageSource;
        private ImageSource _contactImageSource;
        private ImageSource _responseImageSource;
        private ImageSource _chatPersonStatusIcon;
        private ImageSource _imgConsultChatExpander;
        private ImageSource _consultReleaseImageSource;
        private ObservableCollection<IContacts> _contacts = new ObservableCollection<IContacts>();
        private ObservableCollection<IContacts> _contactsFilter = new ObservableCollection<IContacts>();
        private ObservableCollection<IPartyInfo> _partyInfo = new ObservableCollection<IPartyInfo>();
        private ObservableCollection<IRecentInteractions> _recentInteraction = new ObservableCollection<IRecentInteractions>();
        private ObservableCollection<IChatPersonsStatus> _chatPersonsStatus = new ObservableCollection<IChatPersonsStatus>();
        private ObservableCollection<IChatPersonsStatus> _consultChatPersonsStatus = new ObservableCollection<IChatPersonsStatus>();
        private string _userName;
        private string _subject = string.Empty;
        private string _chatFromPersonName;
        private string _consultPersonStatus;
        private string _errorMessage;
        private Visibility _inProgressIXNCountVisibility = Visibility.Collapsed;
        private Visibility _recentIXNListVisibility = Visibility.Collapsed;
        private Visibility _isChatEnabledAddCaseData = Visibility.Hidden;
        private bool _isConsultChatInitialized = false;
        private bool _isChatReleaseClick = false;
        private bool _isChatEnabledModifyCaseData = false;
        private bool _isEnableRelease = false;
        private bool _isEnableTransfer = false;
        private bool _isEnableConference = false;
        private bool _isEnableDone = false;
        private bool _isEnableVoiceConsult = false;
        private bool _isEnableChatConsult = false;
        private bool _isOnChatInteraction = false;
        private bool _isTextMessageEnabled = true;
        private bool _isTextURLEnabled = true;
        private bool _isButtonSendEnabled = true;
        private bool _isButtonConsultSendEnabled = true;
        private bool _isButtonCheckURL = true;
        private bool _isButtonAvailableURL = true;
        private bool _isButtonPushURLExpander = true;
        private bool _isConversationRTBEnabled = true;
        private bool _isEnableSpellCheck = false;
        private string _totalInprogessIXNCount;
        private string _interactionNoteContent;
        private string _recentInteractionCount;
        private string _recentIXNCount;
        private string _remainingDetails;
        private string _notificationImageSource;
        private string _consultReleaseText;
        private string _consultReleaseTTHeading;
        private string _consultReleaseTTContent;
        private string _notifyMessage;
        public bool IsDispositionSelected = false;
        public bool IsChatTransferClick = false;
        public bool IsChatConferenceClick = false;
        public bool IsConferenceChat = false;
        public Paragraph TypeNotifierParagraph = new Paragraph();
        public DockPanel ConsultDockPanel = null;
        public string IxnType = string.Empty;
        public string StartDate = string.Empty;
        public int TimeShift;
        public string TransferReleavePersonId = string.Empty;
        public DateTime SessionStartedTime = new DateTime();
        public SpellChecker Spellcheck = null;
        public FrameworkElement NotificationImage = null;
        public Dictionary<string, string> ChatParties = new Dictionary<string, string>();
        public KeyValueCollection UserData = new KeyValueCollection(StringComparer.OrdinalIgnoreCase);
        public int TicketId;
        public int ProxyId;

        public delegate void UpdatedCaseData();
        public event UpdatedCaseData CaseDataUpdated;
        #region properties

        public string DialedNumbers
        {
            get
            {
                return _dialedNumbers;
            }
            set
            {
                if (_dialedNumbers != value)
                {
                    _dialedNumbers = value;
                    RaisePropertyChanged(() => DialedNumbers);
                }
            }
        }

        public double ModifiedTextSize
        {
            get
            {
                return _modifiedTextSize;
            }
            set
            {
                if (_modifiedTextSize != value)
                {
                    _modifiedTextSize = value;
                    RaisePropertyChanged(() => ModifiedTextSize);
                }
            }
        }

        public System.Windows.Media.Brush MainBorderBrush
        {
            get
            {
                return _mainBorderBrush;
            }
            set
            {
                if (_mainBorderBrush != value)
                {
                    _mainBorderBrush = value;
                    RaisePropertyChanged(() => MainBorderBrush);
                }
            }
        }

        public bool EnableCallMenuitems
        {
            get
            {
                return _enableCallMenuitems;
            }
            set
            {
                if (_enableCallMenuitems != value)
                {
                    _enableCallMenuitems = value;
                    RaisePropertyChanged(() => EnableCallMenuitems);
                }
            }
        }

        public bool EnableEmailMenuitems
        {
            get
            {
                return _enableEmailCallMenuitems;
            }
            set
            {
                if (_enableEmailCallMenuitems != value)
                {
                    _enableEmailCallMenuitems = value;
                    RaisePropertyChanged(() => EnableEmailMenuitems);
                }
            }
        }

        public bool EnableDelConfMenuitems
        {
            get
            {
                return _enableDelConfMenuitems;
            }
            set
            {
                if (_enableDelConfMenuitems != value)
                {
                    _enableDelConfMenuitems = value;
                    RaisePropertyChanged(() => EnableDelConfMenuitems);
                }
            }
        }

        public string AgentID
        {
            get
            {
                return _agentID;
            }
            set
            {
                if (_agentID != value)
                {
                    _agentID = value;
                    RaisePropertyChanged(() => AgentID);
                }
            }
        }

        public string PlaceID
        {
            get
            {
                return _placeID;
            }
            set
            {
                if (_placeID != value)
                {
                    _placeID = value;
                    RaisePropertyChanged(() => PlaceID);
                }
            }
        }

        public string ContactID
        {
            get
            {
                return _contactID;
            }
            set
            {
                if (_contactID != value)
                {
                    _contactID = value;
                }
            }
        }

        public string InteractionID
        {
            get
            {
                return _interactionId;
            }
            set
            {
                if (_interactionId != value)
                {
                    _interactionId = value;
                }
            }
        }

        public GridLength ChatContentRowHeight
        {
            get
            {
                return _chatContentRowHeight;
            }
            set
            {
                if (_chatContentRowHeight != value)
                {
                    _chatContentRowHeight = value;
                    RaisePropertyChanged(() => ChatContentRowHeight);
                }
            }
        }

        public GridLength ErrorRowHeight
        {
            get
            {
                return _errorRowHeight;
            }
            set
            {
                if (_errorRowHeight != value)
                {
                    _errorRowHeight = value;
                    RaisePropertyChanged(() => ErrorRowHeight);
                }
            }
        }

        public GridLength CaseExpanderRowHight
        {
            get
            {
                return _caseExpanderRowHight;
            }
            set
            {
                if (_caseExpanderRowHight != value)
                {
                    _caseExpanderRowHight = value;
                    RaisePropertyChanged(() => CaseExpanderRowHight);
                }
            }
        }

        public GridLength ChatAgentsStatusRowHight
        {
            get
            {
                return _chatAgentsStatusRowHight;
            }
            set
            {
                if (_chatAgentsStatusRowHight != value)
                {
                    _chatAgentsStatusRowHight = value;
                    RaisePropertyChanged(() => ChatAgentsStatusRowHight);
                }
            }
        }

        public GridLength ConsultChatRowHight
        {
            get
            {
                return _consultChatRowHight;
            }
            set
            {
                if (_consultChatRowHight != value)
                {
                    _consultChatRowHight = value;
                    RaisePropertyChanged(() => ConsultChatRowHight);
                }
            }
        }

        public GridLength ConsultChatWindowRowHeight
        {
            get
            {
                return _consultChatWindowRowHeight;
            }
            set
            {
                if (_consultChatWindowRowHeight != value)
                {
                    _consultChatWindowRowHeight = value;
                    RaisePropertyChanged(() => ConsultChatWindowRowHeight);
                }
            }
        }

        public GridLength SendChatWindowRowHeight
        {
            get
            {
                return _sendChatWindowRowHeight;
            }
            set
            {
                if (_sendChatWindowRowHeight != value)
                {
                    _sendChatWindowRowHeight = value;
                    RaisePropertyChanged(() => SendChatWindowRowHeight);
                }
            }
        }

        public GridLength SendchatConsultWindowRowHeight
        {
            get
            {
                return _sendchatConsultWindowRowHeight;
            }
            set
            {
                if (_sendchatConsultWindowRowHeight != value)
                {
                    _sendchatConsultWindowRowHeight = value;
                    RaisePropertyChanged(() => SendchatConsultWindowRowHeight);
                }
            }
        }

        public GridLength ChatToolBarRowHeight
        {
            get
            {
                return _chatToolBarRowHeight;
            }
            set
            {
                if (_chatToolBarRowHeight != value)
                {
                    _chatToolBarRowHeight = value;
                    RaisePropertyChanged(() => ChatToolBarRowHeight);
                }
            }
        }

        public FlowDocument RTBDocument
        {
            get
            {
                return _rtbDocument;
            }
            set
            {
                if (_rtbDocument != value)
                {
                    _rtbDocument = value;
                    RaisePropertyChanged(() => RTBDocument);
                }
            }
        }

        public FlowDocument RTBConsultDocument
        {
            get
            {
                return _rtbConsultDocument;
            }
            set
            {
                if (_rtbConsultDocument != value)
                {
                    _rtbConsultDocument = value;
                    RaisePropertyChanged(() => RTBConsultDocument);
                }
            }
        }

        public ObservableCollection<IChatCaseData> NotifyCaseData
        {
            get
            {
                return _notifyCaseData;
            }
            set
            {
                if (_notifyCaseData != value)
                {
                    _notifyCaseData = value;
                    RaisePropertyChanged(() => NotifyCaseData);
                    CaseDataUpdated.Invoke();
                }
            }
        }

        public string SessionID
        {
            get
            {
                return _sessionID;
            }
            set
            {
                if (_sessionID != value)
                {
                    _sessionID = value;
                    RaisePropertyChanged(() => SessionID);
                }
            }
        }

        public string TitleText
        {
            get
            {
                return _titleText;
            }
            set
            {
                if (_titleText != value)
                {
                    _titleText = value;
                    RaisePropertyChanged(() => TitleText);
                }
            }
        }

        public string ChatWindowTitleText
        {
            get
            {
                return _chatWindowTitleText;
            }
            set
            {
                if (_chatWindowTitleText != value)
                {
                    _chatWindowTitleText = value;
                    RaisePropertyChanged(() => ChatWindowTitleText);
                }
            }
        }

        public string ChatConsultWindowTitleText
        {
            get
            {
                return _chatConsultWindowTitleText;
            }
            set
            {
                if (_chatConsultWindowTitleText != value)
                {
                    _chatConsultWindowTitleText = value;
                    RaisePropertyChanged(() => ChatConsultWindowTitleText);
                }
            }
        }

        public string InprogressInteractionCount
        {
            get
            {
                return _inprogressInteractionCount;
            }
            set
            {
                if (_inprogressInteractionCount != value)
                {
                    _inprogressInteractionCount = value;
                    RaisePropertyChanged(() => InprogressInteractionCount);
                }
            }
        }

        public string ChatMessageText
        {
            get
            {
                return _chatMessageText;
            }
            set
            {
                if (_chatMessageText != value)
                {
                    _chatMessageText = value;
                    RaisePropertyChanged(() => ChatMessageText);
                }
            }
        }

        public string ChatConsultMessageText
        {
            get
            {
                return _chatConsultMessageText;
            }
            set
            {
                if (_chatConsultMessageText != value)
                {
                    _chatConsultMessageText = value;
                    RaisePropertyChanged(() => ChatConsultMessageText);
                }
            }
        }

        public string ChatNoticeText
        {
            get
            {
                return _chatNoticeText;
            }
            set
            {
                if (_chatNoticeText != value)
                {
                    _chatNoticeText = value;
                    RaisePropertyChanged(() => ChatNoticeText);
                }
            }
        }

        public string ChatTypeStatus
        {
            get
            {
                return _chatTypeStatus;
            }
            set
            {
                if (_chatTypeStatus != value)
                {
                    _chatTypeStatus = value;
                    RaisePropertyChanged(() => ChatTypeStatus);
                }
            }
        }

        public ImageSource BtnArrowImageSource
        {
            get
            {
                return _btnArrowImageSource;
            }
            set
            {
                if (_btnArrowImageSource != value)
                {
                    _btnArrowImageSource = value;
                    RaisePropertyChanged(() => BtnArrowImageSource);
                }
            }
        }

        public ImageSource TransImageSource
        {
            get
            {
                return _transImageSource;
            }
            set
            {
                if (_transImageSource != value)
                {
                    _transImageSource = value;
                    RaisePropertyChanged(() => TransImageSource);
                }
            }
        }

        public ImageSource ConfImageSource
        {
            get
            {
                return _confImageSource;
            }
            set
            {
                if (_confImageSource != value)
                {
                    _confImageSource = value;
                    RaisePropertyChanged(() => ConfImageSource);
                }
            }
        }

        public ImageSource ReleaseImageSource
        {
            get
            {
                return _releaseImageSource;
            }
            set
            {
                if (_releaseImageSource != value)
                {
                    _releaseImageSource = value;
                    RaisePropertyChanged(() => ReleaseImageSource);
                }
            }
        }

        public ImageSource DoneImageSource
        {
            get
            {
                return _doneImageSource;
            }
            set
            {
                if (_doneImageSource != value)
                {
                    _doneImageSource = value;
                    RaisePropertyChanged(() => DoneImageSource);
                }
            }
        }

        public ImageSource VoiceConsultImageSource
        {
            get
            {
                return _voiceConsultImageSource;
            }
            set
            {
                if (_voiceConsultImageSource != value)
                {
                    _voiceConsultImageSource = value;
                    RaisePropertyChanged(() => VoiceConsultImageSource);
                }
            }
        }

        public ImageSource ConsultChatImageSource
        {
            get
            {
                return _consultChatImageSource;
            }
            set
            {
                if (_consultChatImageSource != value)
                {
                    _consultChatImageSource = value;
                    RaisePropertyChanged(() => ConsultChatImageSource);
                }
            }
        }

        public ImageSource CasedataImageSource
        {
            get
            {
                return _casedataImageSource;
            }
            set
            {
                if (_casedataImageSource != value)
                {
                    _casedataImageSource = value;
                    RaisePropertyChanged(() => CasedataImageSource);
                }
            }
        }

        public ImageSource ContactImageSource
        {
            get
            {
                return _contactImageSource;
            }
            set
            {
                if (_contactImageSource != value)
                {
                    _contactImageSource = value;
                    RaisePropertyChanged(() => ContactImageSource);
                }
            }
        }

        public ImageSource ResponseImageSource
        {
            get
            {
                return _responseImageSource;
            }
            set
            {
                if (_responseImageSource != value)
                {
                    _responseImageSource = value;
                    RaisePropertyChanged(() => ResponseImageSource);
                }
            }
        }

        public ImageSource ChatPersonStatusIcon
        {
            get
            {
                return _chatPersonStatusIcon;
            }
            set
            {
                if (_chatPersonStatusIcon != value)
                {
                    _chatPersonStatusIcon = value;
                    RaisePropertyChanged(() => ChatPersonStatusIcon);
                }
            }
        }

        public ImageSource ImgConsultChatExpander
        {
            get
            {
                return _imgConsultChatExpander;
            }
            set
            {
                if (_imgConsultChatExpander != value)
                {
                    _imgConsultChatExpander = value;
                    RaisePropertyChanged(() => ImgConsultChatExpander);
                }
            }
        }

        public ImageSource ConsultReleaseImageSource
        {
            get
            {
                return _consultReleaseImageSource;
            }
            set
            {
                if (_consultReleaseImageSource != value)
                {
                    _consultReleaseImageSource = value;
                    RaisePropertyChanged(() => ConsultReleaseImageSource);
                }
            }
        }

        public ObservableCollection<IContacts> Contacts
        {
            get
            {
                return _contacts;
            }
            set
            {
                if (_contacts != value)
                {
                    _contacts = value;
                    RaisePropertyChanged(() => Contacts);
                }
            }
        }

        public ObservableCollection<IContacts> ContactsFilter
        {
            get
            {
                return _contactsFilter;
            }
            set
            {
                if (_contactsFilter != value)
                {
                    _contactsFilter = value;
                    RaisePropertyChanged(() => ContactsFilter);
                }
            }
        }

        public ObservableCollection<IPartyInfo> PartiesInfo
        {
            get
            {
                return _partyInfo;
            }
            set
            {
                if (_partyInfo != value)
                {
                    _partyInfo = value;
                    RaisePropertyChanged(() => PartiesInfo);
                }
            }
        }

        public ObservableCollection<IRecentInteractions> RecentInteraction
        {
            get
            {
                return _recentInteraction;
            }
            set
            {
                if (_recentInteraction != value)
                {
                    _recentInteraction = value;
                    RaisePropertyChanged(() => RecentInteraction);
                }
            }
        }

        public ObservableCollection<IChatPersonsStatus> ChatPersonsStatusInfo
        {
            get
            {
                return _chatPersonsStatus;
            }
            set
            {
                _chatPersonsStatus = value;
                RaisePropertyChanged(() => ChatPersonsStatusInfo);
            }
        }

        public ObservableCollection<IChatPersonsStatus> ChatConsultPersonStatusInfo
        {
            get
            {
                return _consultChatPersonsStatus;
            }
            set
            {
                if (_consultChatPersonsStatus != value)
                {
                    _consultChatPersonsStatus = value;
                    RaisePropertyChanged(() => ChatConsultPersonStatusInfo);
                }
            }
        }

        public string UserName
        {
            get
            {
                return _userName;
            }
            set
            {
                if (_userName != value)
                {
                    _userName = value;
                    RaisePropertyChanged(() => UserName);
                }
            }
        }

        public string Subject
        {
            get
            {
                return _subject;
            }
            set
            {
                if (_subject != value)
                {
                    _subject = value;
                    RaisePropertyChanged(() => Subject);
                }
            }
        }

        public string ChatFromPersonName
        {
            get
            {
                return _chatFromPersonName;
            }
            set
            {
                if (_chatFromPersonName != value)
                {
                    _chatFromPersonName = value;
                    RaisePropertyChanged(() => ChatFromPersonName);
                }
            }
        }

        public string ConsultPersonStatus
        {
            get
            {
                return _consultPersonStatus;
            }
            set
            {
                if (_consultPersonStatus != value)
                {
                    _consultPersonStatus = value;
                    RaisePropertyChanged(() => ConsultPersonStatus);
                }
            }
        }

        public string ErrorMessage
        {
            get
            {
                return _errorMessage;
            }
            set
            {
                if (_errorMessage != value)
                {
                    _errorMessage = value;
                    RaisePropertyChanged(() => ErrorMessage);
                }
            }
        }

        public Visibility InProgressIXNCountVisibility
        {
            get { return _inProgressIXNCountVisibility; }
            set
            {
                _inProgressIXNCountVisibility = value;
                RaisePropertyChanged(() => InProgressIXNCountVisibility);
            }
        }

        public Visibility RecentIXNListVisibility
        {
            get { return _recentIXNListVisibility; }
            set
            {
                _recentIXNListVisibility = value;
                RaisePropertyChanged(() => RecentIXNListVisibility);
            }
        }

        public Visibility IsChatEnabledAddCaseData
        {
            get { return _isChatEnabledAddCaseData; }
            set
            {
                _isChatEnabledAddCaseData = value;
                RaisePropertyChanged(() => IsChatEnabledAddCaseData);
            }
        }

        public bool ISConsultChatInitialized
        {
            get { return _isConsultChatInitialized; }
            set
            {
                _isConsultChatInitialized = value;
                RaisePropertyChanged(() => ISConsultChatInitialized);
            }
        }

        public bool IsChatReleaseClick
        {
            get { return _isChatReleaseClick; }
            set
            {
                _isChatReleaseClick = value;
                RaisePropertyChanged(() => IsChatReleaseClick);
            }
        }

        public bool IsChatEnabledModifyCaseData
        {
            get { return _isChatEnabledModifyCaseData; }
            set
            {
                _isChatEnabledModifyCaseData = value;
                RaisePropertyChanged(() => IsChatEnabledModifyCaseData);
            }
        }

        public bool IsEnableRelease
        {
            get { return _isEnableRelease; }
            set
            {
                _isEnableRelease = value;
                RaisePropertyChanged(() => IsEnableRelease);
            }
        }

        public bool IsEnableTransfer
        {
            get { return _isEnableTransfer; }
            set
            {
                _isEnableTransfer = value;
                RaisePropertyChanged(() => IsEnableTransfer);
            }
        }

        public bool IsEnableConference
        {
            get { return _isEnableConference; }
            set
            {
                _isEnableConference = value;
                RaisePropertyChanged(() => IsEnableConference);
            }
        }

        public bool IsEnableDone
        {
            get { return _isEnableDone; }
            set
            {
                _isEnableDone = value;
                RaisePropertyChanged(() => IsEnableDone);
            }
        }

        public bool IsEnableVoiceConsult
        {
            get { return _isEnableVoiceConsult; }
            set
            {
                _isEnableVoiceConsult = value;
                RaisePropertyChanged(() => IsEnableVoiceConsult);
            }
        }

        public bool IsEnableChatConsult
        {
            get { return _isEnableChatConsult; }
            set
            {
                _isEnableChatConsult = value;
                RaisePropertyChanged(() => IsEnableChatConsult);
            }
        }

        public bool IsOnChatInteraction
        {
            get { return _isOnChatInteraction; }
            set
            {
                _isOnChatInteraction = value;
                RaisePropertyChanged(() => IsOnChatInteraction);
            }
        }

        public bool IsTextMessageEnabled
        {
            get { return _isTextMessageEnabled; }
            set
            {
                _isTextMessageEnabled = value;
                RaisePropertyChanged(() => IsTextMessageEnabled);
            }
        }

        public bool IsTextURLEnabled
        {
            get { return _isTextURLEnabled; }
            set
            {
                _isTextURLEnabled = value;
                RaisePropertyChanged(() => IsTextURLEnabled);
            }
        }

        public bool IsButtonSendEnabled
        {
            get { return _isButtonSendEnabled; }
            set
            {
                _isButtonSendEnabled = value;
                RaisePropertyChanged(() => IsButtonSendEnabled);
            }
        }

        public bool IsButtonConsultSendEnabled
        {
            get { return _isButtonConsultSendEnabled; }
            set
            {
                _isButtonConsultSendEnabled = value;
                RaisePropertyChanged(() => IsButtonConsultSendEnabled);
            }
        }

        public bool IsButtonCheckURL
        {
            get { return _isButtonCheckURL; }
            set
            {
                _isButtonCheckURL = value;
                RaisePropertyChanged(() => IsButtonCheckURL);
            }
        }

        public bool IsButtonAvailableURL
        {
            get { return _isButtonAvailableURL; }
            set
            {
                _isButtonAvailableURL = value;
                RaisePropertyChanged(() => IsButtonAvailableURL);
            }
        }

        public bool IsButtonPushURLExpander
        {
            get { return _isButtonPushURLExpander; }
            set
            {
                _isButtonPushURLExpander = value;
                RaisePropertyChanged(() => IsButtonPushURLExpander);
            }
        }

        public bool IsConversationRTBEnabled
        {
            get { return _isConversationRTBEnabled; }
            set
            {
                _isConversationRTBEnabled = value;
                RaisePropertyChanged(() => IsConversationRTBEnabled);
            }
        }

        public bool IsEnableSpellCheck
        {
            get
            { return _isEnableSpellCheck; }
            set
            {
                _isEnableSpellCheck = value;
            }
        }

        public string TotalInprogessIXNCount
        {
            get
            {
                return _totalInprogessIXNCount;
            }
            set
            {
                if (_totalInprogessIXNCount != value)
                {
                    _totalInprogessIXNCount = value;
                    RaisePropertyChanged(() => TotalInprogessIXNCount);
                }
            }
        }

        public string InteractionNoteContent
        {
            get
            {
                return _interactionNoteContent;
            }
            set
            {
                if (_interactionNoteContent != value)
                {
                    _interactionNoteContent = value;
                    RaisePropertyChanged(() => InteractionNoteContent);
                }
            }
        }

        public string RecentInteractionCount
        {
            get
            {
                return _recentInteractionCount;
            }
            set
            {
                if (_recentInteractionCount != value)
                {
                    _recentInteractionCount = value;
                    RaisePropertyChanged(() => RecentInteractionCount);
                }
            }
        }

        public string RecentIXNCount
        {
            get
            {
                return _recentIXNCount;
            }
            set
            {
                if (_recentIXNCount != value)
                {
                    _recentIXNCount = value;
                    RaisePropertyChanged(() => RecentIXNCount);
                }
            }
        }

        public string RemainingDetails
        {
            get
            {
                return _remainingDetails;
            }
            set
            {
                if (_remainingDetails != value)
                {
                    _remainingDetails = value;
                    RaisePropertyChanged(() => RemainingDetails);
                }
            }
        }

        public string NotificationImageSource
        {
            get
            {
                return _notificationImageSource;
            }
            set
            {
                if (_notificationImageSource != value)
                {
                    _notificationImageSource = value;
                    RaisePropertyChanged(() => NotificationImageSource);
                }
            }
        }

        public string ConsultReleaseText
        {
            get
            {
                return _consultReleaseText;
            }
            set
            {
                if (_consultReleaseText != value)
                {
                    _consultReleaseText = value;
                    RaisePropertyChanged(() => ConsultReleaseText);
                }
            }
        }

        public string ConsultReleaseTTHeading
        {
            get
            {
                return _consultReleaseTTHeading;
            }
            set
            {
                if (_consultReleaseTTHeading != value)
                {
                    _consultReleaseTTHeading = value;
                    RaisePropertyChanged(() => ConsultReleaseTTHeading);
                }
            }
        }

        public string ConsultReleaseTTContent
        {
            get
            {
                return _consultReleaseTTContent;
            }
            set
            {
                if (_consultReleaseTTContent != value)
                {
                    _consultReleaseTTContent = value;
                    RaisePropertyChanged(() => ConsultReleaseTTContent);
                }
            }
        }
        public string NotifyMessage
        {
            get
            {
                return _notifyMessage;
            }
            set
            {
                if (_notifyMessage != value)
                {
                    _notifyMessage = value;
                    RaisePropertyChanged(() => NotifyMessage);
                }
            }
        }
        #endregion

        public void Dispose()
        {
            DialedNumbers = null;
            MainBorderBrush = null;
            AgentID = null;
            PlaceID = null;
            ContactID = null;
            InteractionID = null;
            NotifyCaseData = null;
            SessionID = null;
            TitleText = null;
            ChatWindowTitleText = null;
            ChatConsultWindowTitleText = null;
            InprogressInteractionCount = null;
            ChatMessageText = null;
            ChatConsultMessageText = null;
            ChatNoticeText = null;
            ChatTypeStatus = null;
            BtnArrowImageSource = null;
            TransImageSource = null;
            ConfImageSource = null;
            ReleaseImageSource = null;
            DoneImageSource = null;
            VoiceConsultImageSource = null;
            ConsultChatImageSource = null;
            CasedataImageSource = null;
            ContactImageSource = null;
            ResponseImageSource = null;
            ChatPersonStatusIcon = null;
            ImgConsultChatExpander = null;
            ConsultReleaseImageSource = null;
            Contacts = null;
            ContactsFilter = null;
            PartiesInfo = null;
            RecentInteraction = null;
            ChatPersonsStatusInfo = null;
            ChatConsultPersonStatusInfo = null;
            UserName = null;
            Subject = null;
            ChatFromPersonName = null;
            ConsultPersonStatus = null;
            ErrorMessage = null;
            TotalInprogessIXNCount = null;
            InteractionNoteContent = null;
            RecentInteractionCount = null;
            RecentIXNCount = null;
            RemainingDetails = null;
            NotificationImageSource = null;
            ConsultReleaseText = null;
            ConsultReleaseTTHeading = null;
            ConsultReleaseTTContent = null;
            Spellcheck = null;
            ChatParties = null;
            UserData = null;
            NotificationImage = null;
            NotifyMessage = null;
            StartDate = null;
        }
    }
}
