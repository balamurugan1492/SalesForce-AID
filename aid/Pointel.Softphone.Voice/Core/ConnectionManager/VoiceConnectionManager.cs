namespace Pointel.Softphone.Voice.Core.ConnectionManager
{
    using System;

    using Genesyslab.Platform.ApplicationBlocks.Commons.Broker;
    using Genesyslab.Platform.ApplicationBlocks.Commons.Protocols;
    using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.CfgObjects;
    using Genesyslab.Platform.Commons.Protocols;
    using Genesyslab.Platform.Voice.Protocols;

    using Pointel.Connection.Manager;
    using Pointel.Softphone.Voice.Common;
    using Pointel.Softphone.Voice.Core.Listener;
    using Pointel.Softphone.Voice.Core.Util;

    /// <summary>
    /// This Class provide connection with TServer to support voice feature
    /// </summary>
    internal class VoiceConnectionManager
    {
        #region Fields

        private Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");

        #endregion Fields

        #region Methods

        public OutputValues ConnectTServer(CfgApplication primaryServer, CfgApplication secondaryServer, bool switchoverServer = true)
        {
            OutputValues output = OutputValues.GetInstance();
            var tServerConfProperties = new TServerConfProperties();
            try
            {
                if (primaryServer != null)
                {
                    ProtocolManagers.Instance().DisConnectServer(ServerType.Tserver);
                    Settings.GetInstance().VoiceProtocol = null;
                    logger.Debug("ConnectTServer : Applied primary server properties to Voice protocol");

                    logger.Debug("ConnectTServer : Primary server uri " + "tcp://" + primaryServer.ServerInfo.Host.IPaddress
                                                                                + ":" + primaryServer.ServerInfo.Port);
                    if (secondaryServer != null)
                    {
                        logger.Debug("ConnectTServer : Applied secondary server properties to Voice protocol");
                        logger.Debug("ConnectTServer : Secondary server uri " + "tcp://" + secondaryServer.ServerInfo.Host.IPaddress
                                                                                    + ":" + secondaryServer.ServerInfo.Port);
                    }
                    else
                    {
                        logger.Warn("ConnectTServer : Secondary server is not mentioned");
                        logger.Info("ConnectTServer : Application has no backup servers");
                    }

                    tServerConfProperties.Create(new Uri("tcp://" + primaryServer.ServerInfo.Host.IPaddress + ":" + primaryServer.ServerInfo.Port),
                        Settings.GetInstance().UserName, new Uri("tcp://" + secondaryServer.ServerInfo.Host.IPaddress + ":" + secondaryServer.ServerInfo.Port),
                        Convert.ToInt32(primaryServer.ServerInfo.Timeout), Convert.ToInt16(primaryServer.ServerInfo.Attempts),
                        Convert.ToInt32(Settings.GetInstance().AddpServerTimeout), Convert.ToInt32(Settings.GetInstance().AddpClientTimeout),
                        Genesyslab.Platform.Commons.Connection.Configuration.AddpTraceMode.Both);
                    var TserverProtocolConfiguration = tServerConfProperties.Protocolconfiguration;
                    string error = "";
                    if (!ProtocolManagers.Instance().ConnectServer(TserverProtocolConfiguration, out error))
                    {
                        logger.Error("Tserver protocol is not opened due to, " + error);
                        output.MessageCode = "2001";
                        output.Message = error;
                        return output;
                    }
                    Settings.GetInstance().VoiceProtocol = ProtocolManagers.Instance().ProtocolManager[TserverProtocolConfiguration.Name] as TServerProtocol;
                    VoiceManager.EventCreated = false;
                    Settings.GetInstance().VoiceProtocol.Received += VoiceManager.GetInstance().ReportingVoiceMessage;

                    if (Settings.GetInstance().VoiceProtocol.State == ChannelState.Opened)
                    {
                        logger.Debug("ConnectTServer : Voice protocol is opened ");
                        logger.Info("ConnectTServer : Voice Protocol object id is "
                            + Settings.GetInstance().VoiceProtocol.GetHashCode().ToString());
                        output.MessageCode = "200";
                        output.Message = "TServer Connected";
                    }
                    else
                    {
                        logger.Debug("CreateVoiceConnection : Voice protocol is closed ");
                    }
                }
                else
                {
                    Settings.GetInstance().VoiceProtocol = null;
                    if (switchoverServer)
                        output = ConnectTServer(secondaryServer, primaryServer, false);
                    else
                    {
                        logger.Error("ConnectTServer : No primary server configured.");
                        output.MessageCode = "2002";
                        output.Message = "No primary server configured. Could not able to connect T-server";
                    }
                }
            }

            catch (Exception CommonException)
            {
                Settings.GetInstance().VoiceProtocol = null;
                if (switchoverServer)
                    output = ConnectTServer(secondaryServer, primaryServer, false);
                else
                {
                    logger.Error("ConnectTServer :" + CommonException.ToString());
                    output.MessageCode = "2001";
                    output.Message = CommonException.Message;
                }
            }
            return output;
        }

        #endregion Methods

        #region Other

        /// <summary>
        /// This method used to connect with TServer
        /// </summary>
        /// <param name="primaryServer">Primary TServer Application Object</param>
        /// <param name="secondaryServer">Secondary TServer Application Object</param>
        /// <returns>Return Output value</returns>

        #endregion Other
    }
}