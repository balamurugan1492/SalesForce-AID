namespace Pointel.Softphone.Voice.Core.Util
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.CfgObjects;
    using Genesyslab.Platform.Commons.Collections;
    using Genesyslab.Platform.Voice.Protocols;
    using Genesyslab.Platform.Voice.Protocols.TServer;

    internal class Settings
    {
        #region Fields

        public int alternatetoggle = 1;
        public Dictionary<string, Dictionary<string, string>> BroadCastMessageList = new Dictionary<string, Dictionary<string, string>>();
        public bool ISAlreadyLogin = false;

        //19.08.2013 End
        public bool IsAlternateCallClicked = false;
        public bool IsAutoAnswerEnabled = false;
        public bool IsDNRegistered = false;
        public string PartyState = string.Empty;
        public KeyValueCollection userData = new KeyValueCollection();

        //End
        //Below Variable has been added
        //V.Palaniappan
        public string UserName = string.Empty;
        public bool _isEnableSingleStepTransfer = false;

        private static Settings _settings = null;

        private string _acdPosition = string.Empty;
        private string _aciveDN = string.Empty;
        private string _addpClientTimeout = string.Empty;
        private string _addpServerTimeout = string.Empty;
        private string _agentLoginID;
        private bool _bEnableCompleteConference;
        private bool _bEnableCompleteTransfer;
        private bool _bEnableInitiateConference;
        private bool _bEnableInitiateTransfer;

        //End
        //Below variable has declared to get the type of DN like ACD/Extension/Both
        //and to resolve the call control issue
        //05-14-2013 Palaniappan
        private string _callcontrol = string.Empty;
        private Int32 _conferenceLimit;
        private string _consultationConnID = string.Empty;
        private string _dispositionCodeKey = string.Empty;
        private string _dispositionCollectionKey = string.Empty;
        private bool _enablePIPE = false;
        private ThreadSafeDictionary<string, string> _errorMessages = new ThreadSafeDictionary<string, string>();
        private string _extensionDN = string.Empty;
        private bool _isAgentOnConferenceCall = false;
        private bool _isBothEndUsersReleaseConfCallBeforeComplete = false;

        //End
        //Below condition added to re-solve issue REG - PHN 20
        //04-17-2013 Ram
        private bool _isBothEndUsersReleaseTransferCallBeforeComplete = false;
        private bool _isCallHeld = false;
        private bool _isCustomerReleaseConfCallAfterEstablish = false;

        //End
        //Below condition added to re-solve issue REG - PHN 19
        //04-17-2013 Ram
        private bool _isCustomerReleaseConfCallBeforeEstablish = false;
        private bool _isCustomerReleaseTransferCallAfterEstablish = false;

        //Below condition added to re-solve issue REG - PHN 16
        //04-15-2013 Ram
        private bool _isCustomerReleaseTransferCallBeforeEstablish = false;
        private bool _isDeleteConfEnabled = false;
        private bool _isOnCall = false;
        private bool _isTserverFailed = false;
        private bool _isWorkmodeSet = false;
        private string _locationName = string.Empty;
        private bool _logOffEnable = false;
        private string _notreadyCodeKey = string.Empty;
        private string _notreadyKey = string.Empty;
        private string _notreadyRequest = string.Empty;
        private string _otherDN = string.Empty;
        private string _PIPEName = string.Empty;
        private string _placeName = string.Empty;
        private CfgApplication _primaryApplication;
        private string _primaryTServerName = null;
        private string _queueName = string.Empty;
        private CfgApplication _secondaryApplication;
        private string _secondaryTServerName = null;
        private string _statusMessage = string.Empty;
        private string _strConnectionID = string.Empty;
        private string _strConsultConnectionID = string.Empty;
        private CfgSwitch _switch;
        private string _switchType = string.Empty;
        private TServerProtocol _voiceProtocol = null;
        private string _warmStandbyAttempts = string.Empty;
        private string _warmStandbyTimeout = string.Empty;
        private AgentWorkMode _workMode;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets or sets the acd position.
        /// </summary>
        /// <value>
        /// The acd position.
        /// </value>
        public string ACDPosition
        {
            set { _acdPosition = value; }
            get { return _acdPosition; }
        }

        /// <summary>
        /// Gets or sets the active dn.
        /// </summary>
        /// <value>
        /// The active dn.
        /// </value>
        public string ActiveDN
        {
            set { _aciveDN = value; }
            get { return _aciveDN; }
        }

        /// <summary>
        /// Gets or sets the addp client timeout.
        /// </summary>
        /// <value>
        /// The addp client timeout.
        /// </value>
        public string AddpClientTimeout
        {
            set { _addpClientTimeout = value; }
            get { return _addpClientTimeout; }
        }

        /// <summary>
        /// Gets or sets the addp server timeout.
        /// </summary>
        /// <value>
        /// The addp server timeout.
        /// </value>
        public string AddpServerTimeout
        {
            set { _addpServerTimeout = value; }
            get { return _addpServerTimeout; }
        }

        public string AgentLoginID
        {
            set { _agentLoginID = value; }
            get { return _agentLoginID; }
        }

        //End
        //Below Property has been added to get the type of DN ACD/Extension/both
        //to avoid call control issue
        //05-15-2013 Palaniappan
        /// <summary>
        /// Gets or sets the call control.
        /// </summary>
        /// <value>
        /// The call control.
        /// </value>
        public string CallControl
        {
            set { _callcontrol = value; }
            get { return _callcontrol; }
        }

        /// <summary>
        /// Gets or sets the conference limit.
        /// </summary>
        /// <value>
        /// The conference limit.
        /// </value>
        public Int32 ConferenceLimit
        {
            set { _conferenceLimit = value; }
            get { return _conferenceLimit; }
        }

        /// <summary>
        /// Gets or sets the connection unique identifier.
        /// </summary>
        /// <value>
        /// The connection unique identifier.
        /// </value>
        public string ConnectionID
        {
            set { _strConnectionID = value; }
            get { return _strConnectionID; }
        }

        //Below condition added to re-solve issue REG - PHN 16
        //04-15-2013 Ram
        /// <summary>
        /// Gets or sets the consultation connection unique identifier.
        /// </summary>
        /// <value>
        /// The consultation connection unique identifier.
        /// </value>
        public string ConsultationConnID
        {
            set { _consultationConnID = value; }
            get { return _consultationConnID; }
        }

        /// <summary>
        /// Gets or sets the consult connection unique identifier.
        /// </summary>
        /// <value>
        /// The consult connection unique identifier.
        /// </value>
        public string ConsultConnectionID
        {
            set { _strConsultConnectionID = value; }
            get { return _strConsultConnectionID; }
        }

        /// <summary>
        /// Gets or sets the disposition code key.
        /// </summary>
        /// <value>The disposition code key.</value>
        public string DispositionCodeKey
        {
            set { _dispositionCodeKey = value; }
            get { return _dispositionCodeKey; }
        }

        /// <summary>
        /// Gets or sets the disposition collection key.
        /// </summary>
        /// <value>
        /// The disposition collection key.
        /// </value>
        public string DispositionCollectionKey
        {
            set { _dispositionCollectionKey = value; }
            get { return _dispositionCollectionKey; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [enable pipe].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [enable pipe]; otherwise, <c>false</c>.
        /// </value>
        public bool EnablePIPE
        {
            set { _enablePIPE = value; }
            get { return _enablePIPE; }
        }

        /// <summary>
        /// Gets or sets the error messages.
        /// </summary>
        /// <value>
        /// The error messages.
        /// </value>
        public ThreadSafeDictionary<string, string> ErrorMessages
        {
            set { _errorMessages = value; }
            get { return _errorMessages; }
        }

        /// <summary>
        /// Gets or sets the extension dn.
        /// </summary>
        /// <value>
        /// The extension dn.
        /// </value>
        public string ExtensionDN
        {
            set { _extensionDN = value; }
            get { return _extensionDN; }
        }

        //End
        /// <summary>
        /// Gets or sets a value indicating whether [is agent configuration conference call].
        /// </summary>
        /// <value>
        /// <c>true</c> if [is agent configuration conference call]; otherwise, <c>false</c>.
        /// </value>
        public bool IsAgentOnConferenceCall
        {
            set { _isAgentOnConferenceCall = value; }
            get { return _isAgentOnConferenceCall; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [is both end users release configuration call before complete].
        /// </summary>
        /// <value>
        /// <c>true</c> if [is both end users release configuration call before complete]; otherwise, <c>false</c>.
        /// </value>
        public bool IsBothEndUsersReleaseConfCallBeforeComplete
        {
            set { _isBothEndUsersReleaseConfCallBeforeComplete = value; }
            get { return _isBothEndUsersReleaseConfCallBeforeComplete; }
        }

        //End
        //Below condition added to re-solve issue REG - PHN 20
        //04-17-2013 Ram
        /// <summary>
        /// Gets or sets a value indicating whether [is both end users release transfer call before complete].
        /// </summary>
        /// <value>
        /// <c>true</c> if [is both end users release transfer call before complete]; otherwise, <c>false</c>.
        /// </value>
        public bool IsBothEndUsersReleaseTransferCallBeforeComplete
        {
            set { _isBothEndUsersReleaseTransferCallBeforeComplete = value; }
            get { return _isBothEndUsersReleaseTransferCallBeforeComplete; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [is call held].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [is call held]; otherwise, <c>false</c>.
        /// </value>
        public bool IsCallHeld
        {
            get { return _isCallHeld; }
            set { _isCallHeld = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [is customer release configuration call after establish].
        /// </summary>
        /// <value>
        /// <c>true</c> if [is customer release configuration call after establish]; otherwise, <c>false</c>.
        /// </value>
        public bool IsCustomerReleaseConfCallAfterEstablish
        {
            set { _isCustomerReleaseConfCallAfterEstablish = value; }
            get { return _isCustomerReleaseConfCallAfterEstablish; }
        }

        //End
        //Below condition added to re-solve issue REG - PHN 19
        //04-17-2013 Ram
        /// <summary>
        /// Gets or sets a value indicating whether [is customer release configuration call before establish].
        /// </summary>
        /// <value>
        /// <c>true</c> if [is customer release configuration call before establish]; otherwise, <c>false</c>.
        /// </value>
        public bool IsCustomerReleaseConfCallBeforeEstablish
        {
            set { _isCustomerReleaseConfCallBeforeEstablish = value; }
            get { return _isCustomerReleaseConfCallBeforeEstablish; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [is customer release transfer call after establish].
        /// </summary>
        /// <value>
        /// <c>true</c> if [is customer release transfer call after establish]; otherwise, <c>false</c>.
        /// </value>
        public bool IsCustomerReleaseTransferCallAfterEstablish
        {
            set { _isCustomerReleaseTransferCallAfterEstablish = value; }
            get { return _isCustomerReleaseTransferCallAfterEstablish; }
        }

        //End
        //Below condition added to re-solve issue REG - PHN 16
        //04-15-2013 Ram
        /// <summary>
        /// Gets or sets a value indicating whether [is customer release transfer call before establish].
        /// </summary>
        /// <value>
        /// <c>true</c> if [is customer release transfer call before establish]; otherwise, <c>false</c>.
        /// </value>
        public bool IsCustomerReleaseTransferCallBeforeEstablish
        {
            set { _isCustomerReleaseTransferCallBeforeEstablish = value; }
            get { return _isCustomerReleaseTransferCallBeforeEstablish; }
        }

        public bool IsDeleteConfEnabled
        {
            set { _isDeleteConfEnabled = value; }
            get { return _isDeleteConfEnabled; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [is enable complete conference].
        /// </summary>
        /// <value>
        /// <c>true</c> if [is enable complete conference]; otherwise, <c>false</c>.
        /// </value>
        public bool IsEnableCompleteConference
        {
            set { _bEnableCompleteConference = value; }
            get { return _bEnableCompleteConference; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [is enable complete transfer].
        /// </summary>
        /// <value>
        /// <c>true</c> if [is enable complete transfer]; otherwise, <c>false</c>.
        /// </value>
        public bool IsEnableCompleteTransfer
        {
            set { _bEnableCompleteTransfer = value; }
            get { return _bEnableCompleteTransfer; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [is enable initiate conference].
        /// </summary>
        /// <value>
        /// <c>true</c> if [is enable initiate conference]; otherwise, <c>false</c>.
        /// </value>
        public bool IsEnableInitiateConference
        {
            set { _bEnableInitiateConference = value; }
            get { return _bEnableInitiateConference; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [is enable initiate transfer].
        /// </summary>
        /// <value>
        /// <c>true</c> if [is enable initiate transfer]; otherwise, <c>false</c>.
        /// </value>
        public bool IsEnableInitiateTransfer
        {
            set { _bEnableInitiateTransfer = value; }
            get { return _bEnableInitiateTransfer; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [is enable single step transfer].
        /// </summary>
        /// <value>
        /// <c>true</c> if [is enable single step transfer]; otherwise, <c>false</c>.
        /// </value>
        public bool IsEnableSingleStepTransfer
        {
            get { return _isEnableSingleStepTransfer; }
            set { _isEnableSingleStepTransfer = value; }
        }

        public bool IsOnCall
        {
            get { return _isOnCall; }
            set { _isOnCall = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [is tserver failed].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [is tserver failed]; otherwise, <c>false</c>.
        /// </value>
        public bool IsTserverFailed
        {
            set { _isTserverFailed = value; }
            get { return _isTserverFailed; }
        }

        public bool IsWorkModeSet
        {
            set { _isWorkmodeSet = value; }
            get { return _isWorkmodeSet; }
        }

        /// <summary>
        /// Gets or sets the name of the location.
        /// </summary>
        /// <value>
        /// The name of the location.
        /// </value>
        public string LocationName
        {
            set { _locationName = value; }
            get { return _locationName; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [log off enable].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [log off enable]; otherwise, <c>false</c>.
        /// </value>
        public bool LogOffEnable
        {
            set { _logOffEnable = value; }
            get { return _logOffEnable; }
        }

        /// <summary>
        /// Gets or sets the not ready code key.
        /// </summary>
        /// <value>
        /// The not ready code key.
        /// </value>
        public string NotReadyCodeKey
        {
            set { _notreadyCodeKey = value; }
            get { return _notreadyCodeKey; }
        }

        /// <summary>
        /// Gets or sets the not ready key.
        /// </summary>
        /// <value>
        /// The not ready key.
        /// </value>
        public string NotReadyKey
        {
            set { _notreadyKey = value; }
            get { return _notreadyKey; }
        }

        /// <summary>
        /// Gets or sets the not ready request.
        /// </summary>
        /// <value>
        /// The not ready request.
        /// </value>
        public string NotReadyRequest
        {
            set { _notreadyRequest = value; }
            get { return _notreadyRequest; }
        }

        /// <summary>
        /// Gets or sets the other dn.
        /// </summary>
        /// <value>
        /// The other dn.
        /// </value>
        public string OtherDN
        {
            get { return _otherDN; }
            set { _otherDN = value; }
        }

        /// <summary>
        /// Gets or sets the pipename.
        /// </summary>
        /// <value>
        /// The pipename.
        /// </value>
        public string PIPENAME
        {
            set { _PIPEName = value; }
            get { return _PIPEName; }
        }

        /// <summary>
        /// Gets or sets the name of the place.
        /// </summary>
        /// <value>
        /// The name of the place.
        /// </value>
        public string PlaceName
        {
            set { _placeName = value; }
            get { return _placeName; }
        }

        /// <summary>
        /// Gets or sets the primary application.
        /// </summary>
        /// <value>
        /// The primary application.
        /// </value>
        public CfgApplication PrimaryApplication
        {
            set { _primaryApplication = value; }
            get { return _primaryApplication; }
        }

        /// <summary>
        /// Gets or sets the name of the primary attribute server.
        /// </summary>
        /// <value>
        /// The name of the primary attribute server.
        /// </value>
        public string PrimaryTServerName
        {
            set { _primaryTServerName = value; }
            get { return _primaryTServerName; }
        }

        /// <summary>
        /// Gets or sets the name of the queue.
        /// </summary>
        /// <value>
        /// The name of the queue.
        /// </value>
        public string QueueName
        {
            set { _queueName = value; }
            get { return _queueName; }
        }

        /// <summary>
        /// Gets or sets the secondary application.
        /// </summary>
        /// <value>
        /// The secondary application.
        /// </value>
        public CfgApplication SecondaryApplication
        {
            set { _secondaryApplication = value; }
            get { return _secondaryApplication; }
        }

        /// <summary>
        /// Gets or sets the name of the secondary attribute server.
        /// </summary>
        /// <value>
        /// The name of the secondary attribute server.
        /// </value>
        public string SecondaryTServerName
        {
            set { _secondaryTServerName = value; }
            get { return _secondaryTServerName; }
        }

        /// <summary>
        /// Gets or sets the status message.
        /// </summary>
        /// <value>
        /// The status message.
        /// </value>
        public string StatusMessage
        {
            get { return _statusMessage; }
            set { _statusMessage = value; }
        }

        public CfgSwitch Switch
        {
            set { _switch = value; }
            get { return _switch; }
        }

        /// <summary>
        /// Gets or sets the name of the switch type.
        /// </summary>
        /// <value>
        /// The name of the switch type.
        /// </value>
        public string SwitchTypeName
        {
            set { _switchType = value; }
            get { return _switchType; }
        }

        /// <summary>
        /// Gets or sets the voice protocol.
        /// </summary>
        /// <value>
        /// The voice protocol.
        /// </value>
        public TServerProtocol VoiceProtocol
        {
            set { _voiceProtocol = value; }
            get { return _voiceProtocol; }
        }

        /// <summary>
        /// Gets or sets the warm standby attempts.
        /// </summary>
        /// <value>
        /// The warm standby attempts.
        /// </value>
        public string WarmStandbyAttempts
        {
            set { _warmStandbyAttempts = value; }
            get { return _warmStandbyAttempts; }
        }

        /// <summary>
        /// Gets or sets the warm standby timeout.
        /// </summary>
        /// <value>
        /// The warm standby timeout.
        /// </value>
        public string WarmStandbyTimeout
        {
            set { _warmStandbyTimeout = value; }
            get { return _warmStandbyTimeout; }
        }

        /// <summary>
        /// Gets or sets the work mode.
        /// </summary>
        /// <value>
        /// The work mode.
        /// </value>
        public AgentWorkMode WorkMode
        {
            set { _workMode = value; }
            get { return _workMode; }
        }

        #endregion Properties

        #region Methods

        internal static Settings GetInstance()
        {
            if (_settings == null)
            {
                _settings = new Settings();
                return _settings;
            }
            return _settings;
        }

        #endregion Methods

        #region Other

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <returns></returns>

        #endregion Other
    }
}