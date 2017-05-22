namespace Pointel.Interactions.Core.ConnectionManager
{
    using System;

    using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.CfgObjects;
    using Genesyslab.Platform.Commons.Protocols;
    using Genesyslab.Platform.OpenMedia.Protocols;
    using Genesyslab.Platform.OpenMedia.Protocols.InteractionServer;

    using Pointel.Connection.Manager;
    using Pointel.Interactions.Core.Common;
    using Pointel.Interactions.Core.Listener;
    using Pointel.Interactions.Core.Util;

    internal class InteractionConnectionManager
    {
        #region Fields

        private Pointel.Logger.Core.ILog _logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");

        #endregion Fields

        #region Methods

        public OutputValues ConnectInteractionServer(CfgApplication primaryServer, CfgApplication secondayServer, string clientName)
        {
            OutputValues output = OutputValues.GetInstance();
            var ixnserverconfProperties = new IxnServerConfProperties();
            try
            {
                if (Settings.InteractionProtocol == null)
                {

                    _logger.Debug("ConnectInteractionServer : Connecting to the Ixn server with the details of " +
                        primaryServer.ServerInfo.Host.IPaddress + ":" + primaryServer.ServerInfo.Port + " backup server " +
                        secondayServer.ServerInfo.Port + ":" + secondayServer.ServerInfo.Port);

                    ProtocolManagers.Instance().DisConnectServer(ServerType.Ixnserver);
                    Settings.InteractionProtocol = null;
                    ixnserverconfProperties.Create(new Uri("tcp://" + primaryServer.ServerInfo.Host.IPaddress + ":" + primaryServer.ServerInfo.Port),
                        clientName, InteractionClient.Proxy, new Uri("tcp://" + secondayServer.ServerInfo.Host.IPaddress + ":" + secondayServer.ServerInfo.Port),
                        Convert.ToInt32(primaryServer.ServerInfo.Timeout), Convert.ToInt16(primaryServer.ServerInfo.Attempts),
                         Convert.ToInt32(Settings.AddpServerTimeout), Convert.ToInt32(Settings.AddpClientTimeout), Genesyslab.Platform.Commons.Connection.Configuration.AddpTraceMode.Both);

                    var ixnProtocolConfiguration = ixnserverconfProperties.Protocolconfiguration;

                    string error = "";
                    if (!ProtocolManagers.Instance().ConnectServer(ixnProtocolConfiguration, out error))
                    {
                        _logger.Error("Interaction protocol is not opened due to, " + error);
                        output.MessageCode = "2001";
                        output.Message = error;
                        return output;
                    }
                    Settings.InteractionProtocol = ProtocolManagers.Instance().ProtocolManager[ixnProtocolConfiguration.Name] as InteractionServerProtocol;
                    InteractionManager.EventCreated = false;
                    Settings.InteractionProtocol.Received += InteractionManager.InteractionEvents;

                    if (Settings.InteractionProtocol.State == ChannelState.Opened)
                    {
                        InteractionManager.isAfterConnect = true;
                        _logger.Debug("ConnectInteractionServer : Interaction protocol is opened ");
                        _logger.Info("ConnectInteractionServer : Interaction Protocol object id is " + Settings.InteractionProtocol.GetHashCode().ToString());
                        output.MessageCode = "200";
                        output.Message = "InteractionServer Connected";
                    }
                    else
                    {
                        _logger.Warn("CreateInteractionConnection : Interaction protocol is closed ");
                    }
                }
                else
                {
                    _logger.Warn("Interaction Protocol status is " + Settings.InteractionProtocol.State.ToString());
                }
            }

            catch (Exception commonException)
            {
                Settings.InteractionProtocol = null;
                _logger.Error("ConnectInteractionServer :" + commonException.ToString());
                output.MessageCode = "2001";
                output.Message = commonException.Message;
            }
            return output;
        }

        #endregion Methods
    }
}