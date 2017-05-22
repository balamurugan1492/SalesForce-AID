namespace Pointel.Salesforce.Adapter
{
    using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.CfgObjects;
    using Genesyslab.Platform.Commons.Collections;
    using Genesyslab.Platform.Commons.Protocols;
    using Genesyslab.Platform.Contacts.Protocols;
    using Genesyslab.Platform.Contacts.Protocols.ContactServer.Events;
    using Genesyslab.Platform.Contacts.Protocols.ContactServer.Requests;
    using Genesyslab.Platform.Voice.Protocols;
    using Genesyslab.Platform.Voice.Protocols.TServer.Requests.Party;
    using Pointel.Salesforce.Adapter.LogMessage;
    using Pointel.Salesforce.Adapter.Utility;
    using Pointel.Salesforce.Adapter.Voice;
    using Pointel.Salesforce.Plugin;
    using System;
    using System.Collections.Generic;

    public class ConnectorHelper : Pointel.Salesforce.Plugin.ISFDCConnector, ISFDCListener, IAgentDetails
    {
        #region Fields

        private IDictionary<string, string> activityIds = new Dictionary<string, string>();
        private SFDCConnectionOptions aidOptions;
        private SFDCController sfdcAdapter = null;
        private Log _logger = null;

        #endregion Fields

        #region Events

        public event Plugin.AgentStateHandler AgentState;

        //public event Plugin.SalesforceIntegration EnableIntigeration;
        public event Plugin.LogMessage LogMessage;

        public event Plugin.ConnectionStatusMessage NotifyConnectionStatusMessage;

        public event Plugin.ConnectionStatusChange NotifySFDCConnectionStatusChanges;

        public event Plugin.NotifySFDCUrl NotifyUrl;

        #endregion Events

        #region Properties

        public List<CfgAgentGroup> AgentGroups
        {
            get
            {
                if (this.aidOptions != null)
                    return this.aidOptions.AgentGroups;
                return null;
            }
        }

        public CurrentAgentStatus AgentVoiceMediaStatus
        {
            get
            {
                if (this.AgentState != null)
                {
                    Plugin.AgentState stat = this.AgentState();
                    switch (stat.CurrentAgentStatus)
                    {
                        case Plugin.AgentStatus.Ready:
                            return CurrentAgentStatus.Ready;

                        case Plugin.AgentStatus.NotReady:
                            return CurrentAgentStatus.NotReady;

                        case Plugin.AgentStatus.NotReadyActionCode:
                            return CurrentAgentStatus.NotReadyActionCode;

                        case Plugin.AgentStatus.DndOn:
                            return CurrentAgentStatus.DndOn;

                        case Plugin.AgentStatus.Logout:
                            return CurrentAgentStatus.Logout;

                        case Plugin.AgentStatus.LogoutDndOn:
                            return CurrentAgentStatus.LogoutDndOn;

                        case Plugin.AgentStatus.Unknown:
                            return CurrentAgentStatus.Unknown;

                        case Plugin.AgentStatus.OutOfService:
                            return CurrentAgentStatus.OutOfService;

                        case Plugin.AgentStatus.NotReadyAfterCallWork:
                            return CurrentAgentStatus.NotReadyAfterCallWork;
                    }
                }
                return CurrentAgentStatus.Unknown;
            }
        }

        public int IsAgentOnCall
        {
            get
            {
                if (this.AgentState != null)
                {
                    Plugin.AgentState stat = this.AgentState();
                    if (stat != null && stat.IsOnCall)
                        return 1;
                }
                return 0;
            }
        }

        public bool IsSFDCConnected
        {
            get;
            set;
        }

        public CfgApplication MyApplication
        {
            get
            {
                if (this.aidOptions != null)
                    return this.aidOptions.Application;
                return null;
            }
        }

        public CfgPerson Person
        {
            get
            {
                if (this.aidOptions != null)
                    return this.aidOptions.Person;
                return null;
            }
        }

        public string ThisDN
        {
            get
            {
                if (this.aidOptions != null)
                    return this.aidOptions.ThisDN;
                return null;
            }
        }

        public TServerProtocol TServer
        {
            get
            {
                if (this.aidOptions != null)
                    return this.aidOptions.TserverProtocol as TServerProtocol;
                return null;
            }
        }

        public UniversalContactServerProtocol UCSServer
        {
            get
            {
                if (this.aidOptions != null)
                    return this.aidOptions.UCSProtocol as UniversalContactServerProtocol;
                return null;
            }
        }

        IDictionary<string, string> ISFDCListener.ConsultResponseIds
        {
            get { return activityIds; }
        }

        public IDictionary<string, string> Click2EmailData
        {
            get;
            set;
        }

        public IDictionary<string, string> ChatActivityIdCollection
        {
            get;
            set;
        }

        public IDictionary<string, string> EmailActivityIdCollection
        {
            get;
            set;
        }

        #endregion Properties

        #region Methods

        public void NotifyChatDispositionCode(string interactionId, string dispKeyName, string dispCode)
        {
            if (sfdcAdapter != null)
            {
                IXNCustomData data = new IXNCustomData();
                data.InteractionId = interactionId;
                data.MediaType = MediaType.Chat;
                data.DispositionCode = new System.Tuple<string, string>(dispKeyName, dispCode);
                sfdcAdapter.DispositionCodeChanged(data);
            }
        }

        public void NotifyInteractionEvents(Genesyslab.Platform.Commons.Protocols.IMessage message)
        {
            if (sfdcAdapter != null)
                sfdcAdapter.InteactionEvents(message);
        }

        public void NotifyVoiceDispositionCode(Genesyslab.Platform.Commons.Protocols.IMessage message)
        {
            if (sfdcAdapter != null)
            {
                sfdcAdapter.DispositionCodeChanged(message);
            }
        }

        public void PopupBrowser()
        {
            if (sfdcAdapter != null)
                sfdcAdapter.PopupBrowser();
        }

        public void ReceiveSFDCWindow(string url)
        {
            if (NotifyUrl != null)
                NotifyUrl(url);
        }

        public void ReceiveSFDCWindow(System.Windows.Controls.UserControl SFDCBrowserWindow)
        {
            if (Settings.SFDCOptions != null)
            {
                ReceiveSFDCWindow(Settings.SFDCOptions.SFDCLoginURL);
            }
        }

        public void SendSessionStatus(SFDCSessionStatus sessionStatus)
        {
            switch (sessionStatus)
            {
                case SFDCSessionStatus.Connected:
                    IsSFDCConnected = true;
                    if (NotifySFDCConnectionStatusChanges != null)
                        NotifySFDCConnectionStatusChanges(Plugin.SFDCConnectionStatus.Connected);
                    break;

                case SFDCSessionStatus.NotConnected:
                    IsSFDCConnected = false;
                    if (NotifySFDCConnectionStatusChanges != null)
                        NotifySFDCConnectionStatusChanges(Plugin.SFDCConnectionStatus.NotConnected);
                    break;

                default:
                    break;
            }
        }

        public void SFDCConnectionStatus(LogMode mode, string message)
        {
            if (this.NotifyConnectionStatusMessage != null)
            {
                switch (mode)
                {
                    case LogMode.Info:
                        this.NotifyConnectionStatusMessage(Plugin.MessageMode.Info, message);
                        break;

                    case LogMode.Error:
                        this.NotifyConnectionStatusMessage(Plugin.MessageMode.Error, message);
                        break;

                    case LogMode.Warn:
                        this.NotifyConnectionStatusMessage(Plugin.MessageMode.Warn, message);
                        break;

                    case LogMode.Debug:
                        this.NotifyConnectionStatusMessage(Plugin.MessageMode.Debug, message);
                        break;

                    default:
                        break;
                }
            }
        }

        public void Subscribe(Plugin.SFDCConnectionOptions options)
        {
            sfdcAdapter = new SFDCController();
            this.aidOptions = options;
            sfdcAdapter.SubscribeSFDCPopup(this, this, options.ConfService, options.EnableSubcriberLog, this.TServer, this.UCSServer);
            _logger = Log.GenInstance();
        }

        public void UnSubscribe()
        {
            if (sfdcAdapter != null)
                sfdcAdapter.StopSFDCAdapter();
        }

        public void WriteLogMessage(string message, LogMessage.LogMode mode)
        {
            if (this.LogMessage != null)
            {
                switch (mode)
                {
                    case LogMode.Info:
                        this.LogMessage(Plugin.MessageMode.Info, message);
                        break;

                    case LogMode.Error:
                        this.LogMessage(Plugin.MessageMode.Error, message);
                        break;

                    case LogMode.Warn:
                        this.LogMessage(Plugin.MessageMode.Warn, message);
                        break;

                    case LogMode.Debug:
                        this.LogMessage(Plugin.MessageMode.Debug, message);
                        break;

                    default:
                        break;
                }
            }
        }

        #endregion Methods

        #region Other

        //public void IntegrateSalesforce(bool IsSalesforceEnabled)
        //{
        //    this.EnableIntigeration(IsSalesforceEnabled);
        //}

        #endregion Other

        #region GetOpenMediaInteractionContent

        public Genesyslab.Platform.Contacts.Protocols.ContactServer.Events.EventGetInteractionContent GetOpenMediaInteractionContent(string InteractionId, bool includeAttachments)
        {
            try
            {
                this._logger.Info("GetOpenMediaInteractionContent: Retrieving UCS Content for the IXN Id : " + InteractionId);
                if (this.UCSServer != null)
                {
                    RequestGetInteractionContent ucsRequest = RequestGetInteractionContent.Create();
                    ucsRequest.InteractionId = InteractionId;
                    ucsRequest.IncludeAttachments = includeAttachments;
                    IMessage ucsResponse = this.UCSServer.Request(ucsRequest);
                    if (ucsResponse is Genesyslab.Platform.Contacts.Protocols.ContactServer.Events.EventError)
                    {
                        this._logger.Error("GetOpenMediaInteractionContent: Event Error Returend while requesting GetOpenMediaInteractionContent for the Interaction Id : " + InteractionId + "\n Error Message :" + ucsResponse.ToString());
                    }
                    return ucsResponse as EventGetInteractionContent;
                }
                else
                    this._logger.Error("GetOpenMediaInteractionContent: UCS Protocol is null.");
            }
            catch (Exception generalException)
            {
                this._logger.Error("GetOpenMediaInteractionContent: Error occurred at :" + generalException.ToString());
            }
            return null;
        }

        #endregion GetOpenMediaInteractionContent

        #region MakeOutboundEmail

        public void MakeOutboundEmail(string emailAddress, string recordId, string objectName)
        {
        }

        #endregion MakeOutboundEmail

        #region SetAttachedData

        public void SetAttachedData(string interactionId, string key, string value, int proxyClienntID)
        {
        }

        #endregion SetAttachedData

        #region GetVoiceComments

        public string GetVoiceComments(string connId)
        {
            return null;
        }

        #endregion GetVoiceComments

        #region GetEmailAttachment

        public Genesyslab.Platform.Commons.Protocols.IMessage GetEmailAttachment(string documentID, bool canOpen)
        {
            this._logger.Info("Trying to retrieve email attachment for the document id: " + documentID);
            RequestGetDocument requestDocument = null;
            try
            {
                requestDocument = RequestGetDocument.Create();
                requestDocument.DocumentId = documentID;

                if (this.UCSServer != null)
                {
                    if (this.UCSServer.State == Genesyslab.Platform.Commons.Protocols.ChannelState.Opened)
                        return this.UCSServer.Request(requestDocument);
                }
            }
            catch (Exception generalException)
            {
                this._logger.Error("Error occurred while sending GetEmailAttachment request to UCS, exception:" + generalException);
            }
            return null;
        }

        #endregion GetEmailAttachment

        #region MakeVoiceCall

        public void MakeVoiceCall(string phoneNumber, string attachDataKey, string attachDataValue)
        {
            try
            {
                this._logger.Info("MakeVoiceCall : Dialing outbound/consult for PhoneNumber :" + phoneNumber);

                if (Settings.AgentDetails.IsAgentOnCall > 0)
                {
                    try
                    {
                        this._logger.Info("Agent is on Call - Adapter initiating Consult call");
                        if (Settings.SFDCOptions.EnableConsultDialingFromSFDC)
                        {
                            if (Settings.SFDCOptions.ConsultVoiceDialPlanPrefix != null)
                                phoneNumber = Settings.SFDCOptions.ConsultVoiceDialPlanPrefix + phoneNumber;
                            else
                                this._logger.Info("Consult Dial Plan Prefix is Empty");

                            DialVoiceConsultation(phoneNumber, attachDataKey, attachDataValue);
                        }
                        else
                        {
                            this._logger.Error("MakeVoiceCall : Dialing Consult Call From Adapter is disabled ");
                        }
                    }
                    catch (Exception generalException)
                    {
                        this._logger.Error("MakeVoiceCall : Error Occurred while making call : " + generalException.Message);
                    }
                }
                else
                {
                    try
                    {
                        this._logger.Info("MakeVoiceCall : Agent is not on Call");
                        if (this.TServer != null)
                        {
                            try
                            {
                                if (this.AgentVoiceMediaStatus == CurrentAgentStatus.Ready)
                                {
                                    this._logger.Info("MakeVoiceCall : Current Agent Status : Ready ");
                                    MakeOutBoundCall(phoneNumber, attachDataKey, attachDataValue);
                                }
                                else if (Settings.SFDCOptions.EnableClickToDialOnNotReady && (this.AgentVoiceMediaStatus == CurrentAgentStatus.NotReady ||
                                    this.AgentVoiceMediaStatus == CurrentAgentStatus.NotReadyActionCode ||
                                    this.AgentVoiceMediaStatus == CurrentAgentStatus.NotReadyAfterCallWork))
                                {
                                    MakeOutBoundCall(phoneNumber, attachDataKey, attachDataValue);
                                }
                                else
                                {
                                    this._logger.Info("MakeVoiceCall : Could not make outbound call for the number : " + phoneNumber);
                                    this._logger.Info("MakeVoiceCall : Agent Current Status : " + this.AgentVoiceMediaStatus.ToString());
                                    this._logger.Info("MakeVoiceCall : Is Adapter configured to dial Outbound call on Not Ready ? : " + Settings.SFDCOptions.EnableClickToDialOnNotReady.ToString());
                                }
                            }
                            catch (Exception generalError)
                            {
                                this._logger.Error("MakeVoiceCall : Error Occurred while making outboud call " + generalError.ToString());
                            }
                        }
                        else
                        {
                            this._logger.Info("MakeVoiceCall : Can not make outbound call for the number : " + phoneNumber + "  because the voice media is not available for the agent ");
                        }
                    }
                    catch (System.Exception generalException)
                    {
                        this._logger.Error("MakeVoiceCall : Error occured : " + generalException.ToString());
                    }
                }
            }
            catch (Exception generalException)
            {
                this._logger.Error("MakeVoiceCall : Error occured while passing user data " + generalException.ToString());
            }
        }

        #endregion MakeVoiceCall

        #region Make OutboundCall

        private void MakeOutBoundCall(string phoneNumber, string attachkey, string attachValue)
        {
            try
            {
                this._logger.Info("MakeOutBoundCall : Dialing Outbound Call from Adapter....");
                if (this.TServer != null && this.TServer.State == Genesyslab.Platform.Commons.Protocols.ChannelState.Opened)
                {
                    var requestMakeCall = Genesyslab.Platform.Voice.Protocols.TServer.Requests.Party.RequestMakeCall.Create();
                    requestMakeCall.ThisDN = Settings.AgentDetails.ThisDN;
                    requestMakeCall.OtherDN = phoneNumber;
                    requestMakeCall.MakeCallType = Genesyslab.Platform.Voice.Protocols.TServer.MakeCallType.Regular;
                    KeyValueCollection reason = new KeyValueCollection();
                    reason.Add(attachkey, attachValue);
                    requestMakeCall.Reasons = reason;
                    IMessage message = this.TServer.Request(requestMakeCall);
                    if (message is EventError)
                    {
                        this._logger.Error("MakeOutBoundCall : EventError Returned while making outbound call : " + message.ToString());
                    }
                }
                else
                {
                    this._logger.Info("MakeOutBoundCall : Can not make Outbound Call because T Server Protocol null or Channel closed ");
                }
            }
            catch (Exception generalException)
            {
                this._logger.Error("MakeOutBoundCall : Error occured while making outbound call : " + generalException.ToString());
            }
        }

        #endregion Make OutboundCall

        #region DialVoiceConsultation

        public void DialVoiceConsultation(string OtherDN, string key, string value)
        {
            try
            {
                this._logger.Info("DialVoiceConsultation : Dialing Consult call for the DN : " + OtherDN);
                if (this.TServer != null)
                {
                    RequestInitiateConference requestInitConference = RequestInitiateConference.Create();
                    requestInitConference.OtherDN = OtherDN;
                    if (VoiceEventHandler.eventEstablished.TransferConnID != null)
                    {
                        requestInitConference.ConnID = VoiceEventHandler.eventEstablished.TransferConnID;
                    }
                    else
                    {
                        requestInitConference.ConnID = VoiceEventHandler.eventEstablished.ConnID;
                    }
                    KeyValueCollection reason = new KeyValueCollection();
                    reason.Add(key, value);
                    requestInitConference.Reasons = reason;
                    requestInitConference.ThisDN = VoiceEventHandler.eventEstablished.ThisDN;
                    requestInitConference.Location = VoiceEventHandler.eventEstablished.Location;
                    requestInitConference.UserData = VoiceEventHandler.eventEstablished.UserData;
                    requestInitConference.Extensions = VoiceEventHandler.eventEstablished.Extensions;
                    this._logger.Info("*****************************************************************");
                    this._logger.Info("DialVoiceConsultation : Dialing Consult call ......");
                    this._logger.Info("DialVoiceConsultation : Request  Data :" + requestInitConference.ToString());
                    this._logger.Info("*****************************************************************");
                    IMessage mymessage = this.TServer.Request(requestInitConference);

                    if (mymessage is EventError)
                    {
                        this._logger.Error("DialVoiceConsultation : Error Occured while Initializing Two step Conference" + mymessage.ToString());
                    }
                }
                else
                {
                    this._logger.Info("DialVoiceConsultation : T-Server Protocol is Null");
                }
            }
            catch (Exception generalException)
            {
                this._logger.Error("DialVoiceConsultation : Error Occured : " + generalException.ToString());
            }
        }

        #endregion DialVoiceConsultation

        public void MakeOutboundCall(string phoneNumber, string attachDataKey, string attachDataValue)
        {
            MakeOutBoundCall(phoneNumber, attachDataKey, attachDataValue);
        }

        public void MakeConsultCall(string phoneNumber, string attachDataKey, string attachDataValue, ConnectionId connId)
        {
            DialVoiceConsultation(phoneNumber, attachDataKey, attachDataValue);
        }

        public CurrentAgentStatus AgentStatus
        {
            get { return this.AgentVoiceMediaStatus; }
        }
    }
}