using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Documents;
using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel;
using Genesyslab.Platform.Commons.Collections;
using Genesyslab.Platform.OpenMedia.Protocols;
using Pointel.Interactions.Chat.Helpers;
using Pointel.Interactions.IPlugins;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using System.IO;

namespace Pointel.Interactions.Chat.Settings
{
    public class ChatDataContext
    {
        #region Single instance

        private static ChatDataContext _instance = null;

        public static ChatDataContext GetInstance()
        {
            if (_instance == null)
            {
                _instance = new ChatDataContext();
                return _instance;
            }
            return _instance;
        }

        #endregion Single instance

        #region private members
        private Importer _importClass;

        private string _userName;
        private string _imagePath;
        private System.Drawing.Color _agentPromptColor;
        private System.Drawing.Color _agentTextColor;
        private System.Drawing.Color _clientPromptColor;
        private System.Drawing.Color _clientTextColor;
        private System.Drawing.Color _otherAgentTextColor;
        private System.Drawing.Color _otherAgentPromptColor;
        private System.Drawing.Color _chatSystemTextColor;
        private string _chatPendingResponseToCustomer;
        private string _chatTimeStampFormat;
        private string _chatTypingTimout;
        private string _chatWelcomeMessage;
        private string _chatFarewellMessage; 
        private Visibility _isChatEnabledAddCaseData = Visibility.Hidden;
        private List<string> _loadCaseDataKeys = new List<string>();
        private List<string> _loadCaseDataFilterKeys = new List<string>();
        private List<string> _loadCaseDataSortKeys = new List<string>();
        private Dictionary<string, string> _loadAvailablePushURL = new Dictionary<string, string>();
        private Dictionary<string, string> _loadDispositionCodes = new Dictionary<string, string>();
        private Dictionary<string, Dictionary<string, string>> _loadSubDispositionCodes = new Dictionary<string, Dictionary<string, string>>();
        public static IPluginCallBack messageToClientChat = null;
        public bool IsAvailableVoiceMedia = false;
        public enum ChatUsertype { Agent, Client, External, Supervisor, OtherAgent };
        public string PushedURLKey = string.Empty;
        private string _dialedNumbers = string.Empty;
        private int _consultDialDigits = 0;
        private int _maxDialDigits = 0;
        private int _dialpadDigits = 0;
        private string _agentID = string.Empty;
        private string _placeID = string.Empty;
        private string _contactID = string.Empty;
        private Dictionary<string, ChatUtil> _mainWindowSession = new Dictionary<string, ChatUtil>(); 
        #endregion

        #region public members
        public bool RTBTextHasChanged = false;
        public KeyValuePair<string, object> DispositionObjCollection = new KeyValuePair<string, object>();
        public Dictionary<string, string> CustomerDetails = new Dictionary<string, string>();
        public int OwnerIDorPersonDBID;
        //ChatNotify
        public bool isEnableChatAccept = false;
        public bool isEnableChatReject = false;
        public static ConfService ComObject;
        public static InteractionServerProtocol ixnServerProtocol;
        public string ApplicationName = string.Empty;
        public string DisPositionKeyName = string.Empty;
        public string DispositionCollectionKeyName = string.Empty;
        public string EmailValidateExpression = string.Empty;
        public Dictionary<string, object> dicCMEObjects = new Dictionary<string, object>();
        public static Hashtable hshLoadGroupContact = new Hashtable();
        public string SpeedDialXMLFile = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData).ToString() + @"\SpeedDial.xml";
        private Dictionary<string, string> _annexContacts = new Dictionary<string, string>();
        public Hashtable HshApplicationLevel = new Hashtable(StringComparer.InvariantCultureIgnoreCase);
        public System.Windows.Controls.ContextMenu cmshow = new System.Windows.Controls.ContextMenu();
        public string AgentNickName = string.Empty;
        //end 
        #endregion

        #region Common Properties

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
                }
            }
        }

        public string Imagepath
        {
            get
            {
                return _imagePath;
            }
            set
            {
                if (_imagePath != value)
                {
                    _imagePath = value;
                }
            }
        }

        public Dictionary<string, ChatUtil> MainWindowSession
        {
            get
            {
                return _mainWindowSession;
            }
            set
            {
                if (_mainWindowSession != value)
                {
                    _mainWindowSession = value;
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
                }
            }
        }

        public Dictionary<string, string> AnnexContacts
        {
            get
            {
                return _annexContacts;
            }
            set
            {
                if (_annexContacts != value)
                {
                    _annexContacts = value;
                }
            }
        }

        public int MaxDialDigits
        {
            get
            {
                return _maxDialDigits;
            }
            set
            {
                if (_maxDialDigits != value)
                {
                    _maxDialDigits = value;
                }
            }
        }

        public int DialpadDigits
        {
            get
            {
                return _dialpadDigits;
            }
            set
            {
                if (_dialpadDigits != value)
                {
                    _dialpadDigits = value;
                }
            }
        }

        public int ConsultDialDigits
        {
            get
            {
                return _consultDialDigits;
            }
            set
            {
                if (_consultDialDigits != value)
                {
                    _consultDialDigits = value;
                }
            }
        }

        public Visibility IsChatEnabledAddCaseData
        {
            get { return _isChatEnabledAddCaseData; }
            set
            {
                _isChatEnabledAddCaseData = value;
            }
        }

        public List<string> LoadCaseDataKeys
        {
            get
            {
                return _loadCaseDataKeys;
            }
            set
            {
                if (_loadCaseDataKeys != value)
                {
                    _loadCaseDataKeys = value;
                }
            }
        }

        public List<string> LoadCaseDataFilterKeys
        {
            get
            {
                return _loadCaseDataFilterKeys;
            }
            set
            {
                if (_loadCaseDataFilterKeys != value)
                {
                    _loadCaseDataFilterKeys = value;
                }
            }
        }

        public List<string> LoadCaseDataSortKeys
        {
            get
            {
                return _loadCaseDataSortKeys;
            }
            set
            {
                if (_loadCaseDataSortKeys != value)
                {
                    _loadCaseDataSortKeys = value;
                }
            }
        }

        public Dictionary<string, string> LoadAvailablePushURL
        {
            get
            {
                return _loadAvailablePushURL;
            }
            set
            {
                if (_loadAvailablePushURL != value)
                {
                    _loadAvailablePushURL = value;
                }
            }
        }

        public Dictionary<string, string> LoadDispositionCodes
        {
            get
            {
                return _loadDispositionCodes;
            }
            set
            {
                if (_loadDispositionCodes != value)
                {
                    _loadDispositionCodes = value;
                }
            }
        }

        public Dictionary<string, Dictionary<string, string>> LoadSubDispositionCodes
        {
            get
            {
                return _loadSubDispositionCodes;
            }
            set
            {
                if (_loadSubDispositionCodes != value)
                {
                    _loadSubDispositionCodes = value;
                }
            }
        }

        public bool IsEnableAutoAnswerReject
        {
            get;
            set;
        }
        public int AutoAnswerDelay
        {
            get;
            set;
        }

        public System.Drawing.Color AgentPromptColor
        {
            get
            {
                return _agentPromptColor;
            }
            set
            {
                if (_agentPromptColor != value)
                {
                    _agentPromptColor = value;
                }
            }
        }

        public System.Drawing.Color AgentTextColor
        {
            get
            {
                return _agentTextColor;
            }
            set
            {
                if (_agentTextColor != value)
                {
                    _agentTextColor = value;
                }
            }
        }

        public System.Drawing.Color ClientPromptColor
        {
            get
            {
                return _clientPromptColor;
            }
            set
            {
                if (_clientPromptColor != value)
                {
                    _clientPromptColor = value;
                }
            }
        }

        public System.Drawing.Color ClientTextColor
        {
            get
            {
                return _clientTextColor;
            }
            set
            {
                if (_clientTextColor != value)
                {
                    _clientTextColor = value;
                }
            }
        }

        public System.Drawing.Color OtherAgentTextColor
        {
            get
            {
                return _otherAgentTextColor;
            }
            set
            {
                if (_otherAgentTextColor != value)
                {
                    _otherAgentTextColor = value;
                }
            }
        }

        public System.Drawing.Color OtherAgentPromptColor
        {
            get
            {
                return _otherAgentPromptColor;
            }
            set
            {
                if (_otherAgentPromptColor != value)
                {
                    _otherAgentPromptColor = value;
                }
            }
        }

        public string ChatPendingResponseToCustomer
        {
            get
            {
                return _chatPendingResponseToCustomer;
            }
            set
            {
                if (_chatPendingResponseToCustomer != value)
                {
                    _chatPendingResponseToCustomer = value;
                }
            }
        }

        public System.Drawing.Color ChatSystemTextColor
        {
            get
            {
                return _chatSystemTextColor;
            }
            set
            {
                if (_chatSystemTextColor != value)
                {
                    _chatSystemTextColor = value;
                }
            }
        }

        public string ChatTimeStampFormat
        {
            get
            {
                return _chatTimeStampFormat;
            }
            set
            {
                if (_chatTimeStampFormat != value)
                {
                    _chatTimeStampFormat = value;
                }
            }
        }

        public string ChatTypingTimout
        {
            get
            {
                return _chatTypingTimout;
            }
            set
            {
                if (_chatTypingTimout != value)
                {
                    _chatTypingTimout = value;
                }
            }
        }

        public string ChatWelcomeMessage
        {
            get
            {
                return _chatWelcomeMessage;
            }
            set
            {
                if (_chatWelcomeMessage != value)
                {
                    _chatWelcomeMessage = value;
                }
            }
        }

        public string ChatFareWellMessage
        {
            get
            {
                return _chatFarewellMessage;
            }
            set
            {
                if (_chatFarewellMessage != value)
                {
                    _chatFarewellMessage = value;
                }
            }
        }

        public Importer ImportClass
        {
            get
            {
                _importClass = _importClass ?? new Importer();
                return _importClass;
            }
        }

        #endregion Common Properties

        #region Methods
        /// <summary>
        /// Gets the bitmap image.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <returns></returns>
        public BitmapImage GetBitmapImage(Uri uri)
        {
            StreamResourceInfo imageInfo = System.Windows.Application.GetResourceStream(uri);
            var bitmap = new BitmapImage();
            try
            {
                byte[] imageBytes = ReadFully(imageInfo.Stream);
                using (Stream stream = new MemoryStream(imageBytes))
                {
                    bitmap.BeginInit();
                    bitmap.StreamSource = stream;
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    bitmap.UriSource = uri;
                    if (bitmap.CanFreeze)
                        bitmap.Freeze();
                }
                imageBytes = null;
                return bitmap;
            }
            catch
            {
                return null;
            }
            finally
            {
                imageInfo = null;
                bitmap = null;
            }

        }

        /// <summary>
        /// Reads the fully.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        private byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }
        #endregion
    }
}
