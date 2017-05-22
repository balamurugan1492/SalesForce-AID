#region Header

/*
* =====================================
* Pointel.Configuration.Manager.Core.ConnectionManager
* ====================================
* Project    : Agent Interaction Desktop
* Created on : 31-March-2015
* Author     : Manikandan
* Owner      : Pointel Solutions
* ====================================
*/

#endregion Header

namespace Pointel.Configuration.Manager.ConnectionManager
{
    using System;
    using System.Text;
    using System.Xml.Linq;

    using Genesyslab.Platform.ApplicationBlocks.Commons.Broker;
    using Genesyslab.Platform.ApplicationBlocks.Commons.Protocols;
    using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel;
    using Genesyslab.Platform.Commons.Protocols;
    using Genesyslab.Platform.Configuration.Protocols;
    using Genesyslab.Platform.Configuration.Protocols.ConfServer.Events;
    using Genesyslab.Platform.Configuration.Protocols.Types;

    using log4net;

    using Pointel.Configuration.Manager.Common;
    using Pointel.Configuration.Manager.Core;
    using Pointel.Connection.Manager;

    class ConfigConnectionManager
    {
        #region Fields

        private Pointel.Logger.Core.ILog _logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
              "AID");

        #endregion Fields

        #region Methods

        public OutputValues ConnectConfigServer(string pri_ConfigServerHost, string pri_ConfigServerPort,
            string applicationName, string userName, string password, string sec_ConfigServerHost,
            string sec_ConfigServerPort, bool switchoverServer = true)
        {
            var output = new OutputValues();
            var confServerProperties = new ConfigurationServerConfProperties();
            try
            {
                _logger.Debug("ConnectTServer : Applied primary server properties to Voice protocol");
                _logger.Debug("ConnectTServer : Primary server uri " + "tcp://" + pri_ConfigServerHost
                                                                            + ":" + pri_ConfigServerPort);
                if (sec_ConfigServerHost != null)
                {
                    _logger.Debug("ConnectTServer : Applied secondary server properties to Voice protocol");
                    _logger.Debug("ConnectTServer : Secondary server uri " + "tcp://" + sec_ConfigServerHost
                                                                                + ":" + sec_ConfigServerPort);
                }
                else
                {
                    _logger.Warn("ConnectTServer : Secondary server is not mentioned");
                    _logger.Info("ConnectTServer : Application has no backup servers");
                }

                if (ConfigContainer.Instance().ConfServiceObject != null)
                    ConfServiceFactory.ReleaseConfService(ConfigContainer.Instance().ConfServiceObject);

                ProtocolManagers.Instance().DisConnectServer(ServerType.Configserver);

                confServerProperties.Create(new Uri("tcp://" + pri_ConfigServerHost + ":" + pri_ConfigServerPort), CfgAppType.CFGAgentDesktop, applicationName,
                    userName, password, new Uri("tcp://" + (sec_ConfigServerHost == "optional" ? pri_ConfigServerHost : sec_ConfigServerHost) +
                 ":" + (sec_ConfigServerPort == "optional" ? pri_ConfigServerPort : sec_ConfigServerPort)),
                 10, 5, 60, 60, Genesyslab.Platform.Commons.Connection.Configuration.AddpTraceMode.Both);
                var cfgProtocolConfiguration = confServerProperties.Protocolconfiguration;
                string error = "";
                if (!ProtocolManagers.Instance().ConnectServer(cfgProtocolConfiguration, out error))
                {
                    _logger.Error("Configuration protocol is not opened due to, " + error);
                    output.MessageCode = "2001";
                    output.Message = error;
                    return output;
                }
                ConfigContainer.Instance().ConfServiceObject = (ConfService)ConfServiceFactory.CreateConfService(
                                  ProtocolManagers.Instance().ProtocolManager[cfgProtocolConfiguration.Name] as ConfServerProtocol);
                if (ProtocolManagers.Instance().ProtocolManager[cfgProtocolConfiguration.Name].State == ChannelState.Opened)
                {
                    _logger.Debug("getProtocol : Configuration protocol is opened ");
                    _logger.Info("getProtocol : Configuration Protocol object id is "
                        + ProtocolManagers.Instance().ProtocolManager[cfgProtocolConfiguration.Name].GetHashCode().ToString());

                    output.MessageCode = "200";
                    output.Message = "Config Server Protocol Opened";
                }
                else
                {
                    _logger.Debug("getProtocol : Configuration protocol is closed ");
                    output.MessageCode = "2001";
                    output.Message = "Config Server Connection not Established";
                }

                #region Old Code
                //catch (Exception generalException)
                //{
                //    _logger.Info("Configuration protocol already opened " + generalException.ToString());
                //    output.MessageCode = "2001";
                //    output.Message = (generalException.InnerException == null ? generalException.Message : generalException.InnerException.Message);
                //    return output;
                //}

                //_logger.Debug("getProtocol : Registered protocol manager ");
                ////Open the connection
                //ConnectionSettings.protocolManager[ConnectionSettings.ConfServer].Open();

                //comEventBrokerService = new EventBrokerService(ConnectionSettings.protocolManager.Receiver);
                ////Subscription of CFGObjects
                ////    Register(new Action<ConfEvent>(ReadApplicationObject.GetInstance().ConfigServiceHandler), new MessageIdFilter(EventObjectUpdated.MessageId));
                ////comEventBrokerService.Register(ReadApplicationObject.GetInstance().OnCMEObjectUpdated, new MessageIdFilter(EventObjectUpdated.MessageId));

                ////if (ConnectionSettings.comObject != null)
                ////    ConfServiceFactory.ReleaseConfService(ConnectionSettings.comObject);
                //if (ConfigContainer.Instance().ConfServiceObject != null)
                //    ConfServiceFactory.ReleaseConfService(ConfigContainer.Instance().ConfServiceObject);

                //ConfigContainer.Instance().ConfServiceObject = (ConfService)ConfServiceFactory.CreateConfService(
                //                  ConnectionSettings.protocolManager[ConnectionSettings.ConfServer] as ConfServerProtocol,
                //                  comEventBrokerService, true);
                //comEventBrokerService.Activate();

                //_logger.Debug("Trying to open the connection");
                //if (ConnectionSettings.protocolManager[ConnectionSettings.ConfServer].State == ChannelState.Opened)
                //{
                //    _logger.Debug("getProtocol : Configuration protocol is opened ");
                //    _logger.Info("getProtocol : Configuration Protocol object id is "
                //        + ConnectionSettings.protocolManager[ConnectionSettings.ConfServer].GetHashCode().ToString());

                //    output.MessageCode = "200";
                //    output.Message = "Config Server Protocol Opened";
                //}
                //else
                //{
                //    _logger.Debug("getProtocol : Configuration protocol is closed ");
                //    output.MessageCode = "2001";
                //    output.Message = "Config Server Connection Established";
                //}
                #endregion old code
            }
            catch (Exception connectionException)
            {
                if (switchoverServer)
                    output = ConnectConfigServer(sec_ConfigServerHost, sec_ConfigServerPort,
                   applicationName, userName, password, pri_ConfigServerHost,
                   pri_ConfigServerPort, false);
                else
                {
                    output.MessageCode = "2001";
                    output.Message = (connectionException.InnerException == null ? connectionException.Message : connectionException.InnerException.Message);
                }
            }
            return output;
        }

        #endregion Methods
    }
}