namespace Pointel.Salesforce.Adapter
{
    using System.Collections.Generic;

    using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.CfgObjects;
    using Genesyslab.Platform.Contacts.Protocols;
    using Genesyslab.Platform.Voice.Protocols;

    using Pointel.Salesforce.Adapter.LogMessage;
    using Pointel.Salesforce.Adapter.Utility;
    using Pointel.Salesforce.Plugin;

    public class ConnectorHelper : Pointel.Salesforce.Plugin.ISFDCConnector, ISFDCListener, IAgentDetails
    {
        #region Fields

        private IDictionary<string, string> activityIds = new Dictionary<string, string>();
        private SFDCConnectionOptions aidOptions;
        private SFDCController sfdcAdapter = null;

        #endregion Fields

        #region Events

        public event Plugin.AgentStateHandler AgentState;

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

        #endregion Properties

        #region Methods

        public void NotifyChatDispositionCode(string interactionId, string dispKeyName, string dispCode)
        {
            if (sfdcAdapter != null)
                sfdcAdapter.DispositionCodeChanged(interactionId, dispKeyName, dispCode);
        }

        public void NotifyInteractionEvents(Genesyslab.Platform.Commons.Protocols.IMessage message)
        {
            if (sfdcAdapter != null)
                sfdcAdapter.InteactionEvents(message);
        }

        public void NotifyVoiceDispositionCode(Genesyslab.Platform.Commons.Protocols.IMessage message)
        {
            if (sfdcAdapter != null)
                sfdcAdapter.DispositionCodeChanged(message);
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
    }
}