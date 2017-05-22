using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.CfgObjects;
using Genesyslab.Platform.Contacts.Protocols;
using Genesyslab.Platform.Voice.Protocols;
using Pointel.Salesforce.Adapter.LogMessage;
using Pointel.Salesforce.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pointel.Salesforce.Adapter
{
    public class SFDCConnector : Pointel.Salesforce.Plugin.ISFDCConnector, ISFDCListener, IAgentDetails
    {
        public event Plugin.AgentStateHandler AgentState;

        public event Plugin.LogMessage LogMessage;

        public event Plugin.NotifySFDCUrl NotifyUrl;

        public event Plugin.ConnectionStatus NotifyConnectionStatus;

        private SFDCController sfdcAdapter = null;

        SFDCConnectionOptions aidOptions;

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

        public void NotifyChatDispositionCode(string interactionId, string dispKeyName, string dispCode)
        {
            if (sfdcAdapter != null)
                sfdcAdapter.DispositionCodeChanged(interactionId, dispKeyName, dispCode);
        }

        public void Subscribe(Plugin.SFDCConnectionOptions options)
        {
            sfdcAdapter = new SFDCController();
            this.aidOptions = options;
            sfdcAdapter.SubscribeSFDCPopup(this, this, options.ConfService, options.EnableSubcriberLog);
        }

        public void UnSubscribe()
        {
            if (sfdcAdapter != null)
                sfdcAdapter.StopSFDCAdapter();
        }

        public void SFDCConnectionStatus(LogMode mode, string message)
        {
            if (this.NotifyConnectionStatus != null)
            {
                switch (mode)
                {
                    case LogMode.Info:
                        this.NotifyConnectionStatus(Plugin.MessageMode.Info, message);
                        break;
                    case LogMode.Error:
                        this.NotifyConnectionStatus(Plugin.MessageMode.Error, message);
                        break;
                    case LogMode.Warn:
                        this.NotifyConnectionStatus(Plugin.MessageMode.Warn, message);
                        break;
                    case LogMode.Debug:
                        this.NotifyConnectionStatus(Plugin.MessageMode.Debug, message);
                        break;
                    default:
                        break;
                }
            }
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

        public void ReceiveSFDCWindow(string url)
        {
            if (NotifyUrl != null)
                NotifyUrl(url);
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

        public CfgPerson Person
        {
            get
            {
                if (this.aidOptions != null)
                    return this.aidOptions.Person;
                return null;
            }
        }

        public List<CfgAgentGroup> AgentGroups
        {
            get
            {
                if (this.aidOptions != null)
                    return this.aidOptions.AgentGroups;
                return null;
            }
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
    }
}
