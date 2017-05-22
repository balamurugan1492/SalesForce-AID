namespace Pointel.Interactions.Contact.Core.ConnectionManager
{
    using System;

    using Genesyslab.Platform.ApplicationBlocks.Commons.Broker;
    using Genesyslab.Platform.ApplicationBlocks.Commons.Protocols;
    using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.CfgObjects;
    using Genesyslab.Platform.Commons.Protocols;
    using Genesyslab.Platform.Contacts.Protocols;

    using Pointel.Connection.Manager;
    using Pointel.Interactions.Contact.Core.Common;
    using Pointel.Interactions.Contact.Core.Util;

    #region Delegates

    internal delegate bool NotifyContactServerState(bool isOpen);

    #endregion Delegates

    internal class ContactConnectionManager
    {
        #region Fields

        internal NotifyContactServerState ContactServerNotificationHandler;

        private Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");

        #endregion Fields

        #region Methods

        /// <summary>
        /// Connects the contact server.
        /// </summary>
        /// <param name="primaryServer">The primary server.</param>
        /// <param name="secondaryServer">The secondary server.</param>
        /// <returns></returns>
        public OutputValues ConnectContactServer(CfgApplication primaryServer, CfgApplication secondaryServer, bool switchoverServer = true)
        {
            OutputValues output = OutputValues.GetInstance();
            var ucsServerConfProperties = new UcsServerConfProperties();
            try
            {
                if (Settings.UCSProtocol == null)
                {

                    logger.Debug("ConnectContactServer : Applied primary server properties to UCS protocol");
                    logger.Debug("ConnectContactServer : Primary server uri " + "tcp://" + primaryServer.ServerInfo.Host.IPaddress
                                                                                + ":" + primaryServer.ServerInfo.Port);
                    if (secondaryServer != null)
                    {
                        logger.Debug("ConnectContactServer : Applied secondary server properties to UCS protocol");
                        logger.Debug("ConnectContactServer : Secondary server uri " + "tcp://" + secondaryServer.ServerInfo.Host.IPaddress
                                                                                    + ":" + secondaryServer.ServerInfo.Port);
                    }
                    else
                    {
                        logger.Warn("ConnectContactServer : Secondary server is not mentioned");
                        logger.Info("ConnectContactServer : Application has no backup servers");
                    }

                    ProtocolManagers.Instance().DisConnectServer(ServerType.Ucserver);

                    ucsServerConfProperties.Create(new Uri("tcp://" + primaryServer.ServerInfo.Host.IPaddress + ":" + primaryServer.ServerInfo.Port),
                       "", new Uri("tcp://" + secondaryServer.ServerInfo.Host.IPaddress + ":" + secondaryServer.ServerInfo.Port),
                       Convert.ToInt32(primaryServer.ServerInfo.Timeout), Convert.ToInt16(primaryServer.ServerInfo.Attempts),
                       Convert.ToInt32(Settings.AddpServerTimeout), Convert.ToInt32(Settings.AddpClientTimeout),
                       Genesyslab.Platform.Commons.Connection.Configuration.AddpTraceMode.Both);
                    var USCserverProtocolConfiguration = ucsServerConfProperties.Protocolconfiguration;
                    string error = "";
                    if (!ProtocolManagers.Instance().ConnectServer(USCserverProtocolConfiguration, out error))
                    {
                        logger.Error("Ucs protocol is not opened due to, " + error);
                        output.MessageCode = "2001";
                        output.Message = error;
                        return output;
                    }
                    Settings.UCSProtocol = ProtocolManagers.Instance().ProtocolManager[USCserverProtocolConfiguration.Name] as UniversalContactServerProtocol;
                    Settings.UCSProtocol.Opened += ContactConnectionManager_Opened;
                    Settings.UCSProtocol.Closed += ContactConnectionManager_Closed;
                    if (Settings.UCSProtocol.State == ChannelState.Opened)
                    {
                        ContactConnectionManager_Opened(null, null);
                        logger.Warn("ConnectContactServer : Contact protocol is opened ");
                        logger.Info("ConnectContactServer : Contact Protocol object id is " + Settings.UCSProtocol.GetHashCode().ToString());
                        output.MessageCode = "200";
                        output.Message = "ConnectContactServer Connected";
                    }
                    else
                    {
                        logger.Warn("ConnectContactServer : Contact protocol is closed ");
                    }
                }
                else
                {
                    logger.Info("Contact Protocol status is " + Settings.UCSProtocol.State.ToString());
                }
            }
            catch (Exception commonException)
            {
                Settings.UCSProtocol = null;
                if (switchoverServer)
                    output = ConnectContactServer(secondaryServer, primaryServer, false);
                else
                {
                    logger.Error("ConnectContactServer :" + commonException.ToString());
                    output.MessageCode = "2001";
                    output.Message = commonException.Message;
                }
            }
            return output;
        }

        void ContactConnectionManager_Closed(object sender, EventArgs e)
        {
            //Code to notify contact Server protocol closed.
            if (!Settings.IsConnectionOpened) return;
            logger.Info("Contact protocol closed");
            Settings.IsConnectionOpened = false;
            if (ContactServerNotificationHandler != null)
                ContactServerNotificationHandler.Invoke(Settings.IsConnectionOpened);
        }

        void ContactConnectionManager_Opened(object sender, EventArgs e)
        {
            //Code to notify contact Server protocol opened.
            logger.Info("Contact protocol opened");
            Settings.IsConnectionOpened = true;
            if (ContactServerNotificationHandler != null)
                ContactServerNotificationHandler.Invoke(Settings.IsConnectionOpened);
        }

        #endregion Methods
    }
}