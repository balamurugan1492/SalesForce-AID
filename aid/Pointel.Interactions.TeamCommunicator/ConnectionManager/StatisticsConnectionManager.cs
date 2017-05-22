#region Header

/*
* =====================================
* Pointel.Interactions.TeamCommunicator.Helpers
* ====================================
* Project    : Agent Interaction Desktop
* Created on : 12-June-2015
* Author     : Manikandan
* Owner      : Pointel Solutions
* ====================================
*/

#endregion Header

namespace Pointel.Interactions.TeamCommunicator.ConnectionManager
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Genesyslab.Platform.ApplicationBlocks.Commons.Broker;
    using Genesyslab.Platform.ApplicationBlocks.Commons.Protocols;
    using Genesyslab.Platform.Reporting.Protocols;

    using Pointel.Connection.Manager;
    using Pointel.Interactions.TeamCommunicator.InteractionListener;
    using Pointel.Interactions.TeamCommunicator.Settings;

    internal class StatisticsConnectionManager
    {
        #region Fields

        Listener _listener = new Listener();
        private Pointel.Logger.Core.ILog _logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
                    "AID");

        #endregion Fields

        #region Methods

        public bool ConnectStatServer(string primaryHostName, string primaryPort, string secondaryHostName, string secondaryPort, int addpServerTimeOut, int addpClientTimeOut,
            int warmStandbyTimeout, short warmStandbyAttempts)
        {
            //StatServerConfiguration _statServerConfiguration = new StatServerConfiguration(Datacontext.GetInstance().StatServerName);
            //if (Datacontext.GetInstance().ProtocolManager != null)
            //    Datacontext.GetInstance().ProtocolManager = null;
            //EventBrokerService _eventBrokerService = null;
            try
            {
                //_statServerConfiguration.Uri = new System.Uri("tcp://" + primaryHostName + ":" + primaryPort);
                //_statServerConfiguration.ClientName = Datacontext.GetInstance().StatServerName + "_" + Datacontext.GetInstance().UserName;
                //_statServerConfiguration.FaultTolerance = new FaultToleranceMode?(FaultToleranceMode.WarmStandby);
                //_statServerConfiguration.WarmStandbyTimeout = new int?(warmStandbyTimeout);
                //_statServerConfiguration.WarmStandbyAttempts = new short?(warmStandbyAttempts);
                //_statServerConfiguration.WarmStandbyUri = new System.Uri("tcp://" + secondaryHostName + ":" + secondaryPort);
                //_statServerConfiguration.UseAddp = new bool?(true);
                //_statServerConfiguration.AddpServerTimeout = new int?(addpServerTimeOut);
                //_statServerConfiguration.AddpClientTimeout = new int?(addpClientTimeOut);
                //_statServerConfiguration.AddpTrace = "both";
                //Datacontext.GetInstance().ProtocolManager.Register(_statServerConfiguration);

                //_eventBrokerService = new EventBrokerService(Datacontext.GetInstance().ProtocolManager.Receiver);
                //_eventBrokerService.Activate();
                //_eventBrokerService.Register(Listener.Listener.ReportingSuccessMessage);
                //Datacontext.GetInstance().ProtocolManager[Datacontext.GetInstance().StatServerName].Opened += new EventHandler(StatServerOpened);
                //Datacontext.GetInstance().ProtocolManager[Datacontext.GetInstance().StatServerName].Closed +=new EventHandler(StatServerClosed);
                //Datacontext.GetInstance().ProtocolManager[Datacontext.GetInstance().StatServerName].Open();

                StatServerConfProperties _statServerProperties = new StatServerConfProperties();
                _statServerProperties.Create(new Uri("tcp://" + primaryHostName + ":" + primaryPort), Datacontext.GetInstance().UserName,
                    new Uri("tcp://" + secondaryHostName + ":" + secondaryPort), warmStandbyTimeout, warmStandbyAttempts, addpServerTimeOut, addpClientTimeOut,
                        Genesyslab.Platform.Commons.Connection.Configuration.AddpTraceMode.Both);

                try
                {
                    ProtocolManagers.Instance().ProtocolManager.Unregister(ServerType.Statisticsserver.ToString());
                    _logger.Warn("Protocol configuration is already registered, clear existing configurations");
                }
                catch (Exception ex)
                {
                    _logger.Warn("Try to remove existing protocol configurations throws exception");
                }

                string error = "";
                if (!ProtocolManagers.Instance().ConnectServer(_statServerProperties.Protocolconfiguration, out error))
                {
                    _logger.Error("Statserver protocol is not opened due to, " + error);
                    return false;
                }

                Listener _listener = new Listener();
                ((StatServerProtocol)ProtocolManagers.Instance().ProtocolManager[ServerType.Statisticsserver.ToString()]).Received += _listener.ReportingSuccessMessage;

                if (ProtocolManagers.Instance().ProtocolManager[ServerType.Statisticsserver.ToString()].State == Genesyslab.Platform.Commons.Protocols.ChannelState.Opened)
                {
                    _logger.Info("Stat server connection opened");
                    Datacontext.GetInstance().IsStatAlive = true;
                    ProtocolManagers.Instance().ProtocolManager[ServerType.Statisticsserver.ToString()].Opened += new EventHandler(StatisticsConnectionManager_Opened);
                    ProtocolManagers.Instance().ProtocolManager[ServerType.Statisticsserver.ToString()].Closed += new EventHandler(StatisticsConnectionManager_Closed);
                    return true;
                }
                else
                {
                    _logger.Info("Stat server connection not opened");
                    Datacontext.GetInstance().IsStatAlive = false;
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error occurred while connecting statistics server : " + ((ex.InnerException == null) ? ex.Message : ex.InnerException.ToString()));
                return false;
            }
        }

        private void StatisticsConnectionManager_Closed(object sender, System.EventArgs e)
        {
            Datacontext.GetInstance().IsStatAlive = false;
            _listener.NotifyStatServerStatus("ServerClosed");
        }

        private void StatisticsConnectionManager_Opened(object sender, System.EventArgs e)
        {
            Datacontext.GetInstance().IsStatAlive = true;
            _listener.NotifyStatServerStatus("ServerStarted");
        }

        private void SwitchOverServer()
        {
            try
            {

            }
            catch (Exception ex)
            {
                _logger.Error("Error occurred while connecting switch over statistics server : " + ((ex.InnerException == null) ? ex.Message : ex.InnerException.ToString()));
            }
        }

        #endregion Methods
    }
}