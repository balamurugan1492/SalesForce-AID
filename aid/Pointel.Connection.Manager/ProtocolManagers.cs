namespace Pointel.Connection.Manager
{
    using System;

    using Genesyslab.Platform.ApplicationBlocks.Commons.Protocols;
    using Genesyslab.Platform.Commons.Protocols;

    public class ProtocolManagers
    {
        #region Fields

        private static ProtocolManagers _instance;

        private ProtocolManagementService _protocolManager = new ProtocolManagementService();

        #endregion Fields

        #region Properties

        public ProtocolManagementService ProtocolManager
        {
            get { return _protocolManager; }
            set { if (_protocolManager != value) _protocolManager = value; }
        }

        #endregion Properties

        #region Methods

        public static ProtocolManagers Instance()
        {
            if (_instance == null)
            {
                _instance = new ProtocolManagers();
                return _instance;
            }
            return _instance;
        }

        public bool ConnectServer(ProtocolConfiguration protocolConfig, out string error)
        {
            error = "";
            try
            {
                ProtocolManager.Register(protocolConfig);
                ProtocolManager[protocolConfig.Name].Open();
                return true;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Description:"))
                    error = ex.Message.Substring(ex.Message.IndexOf("Description:"));
                else if (ex.Message.Contains("Exception occured during channel opening")
                    || (ex.InnerException != null && ex.InnerException.ToString().Contains("No such host is known")))
                {
                    error = "Could not connect to ";
                    switch (protocolConfig.Name)
                    {
                        case "Configserver":
                            error += "configuration";
                            break;
                        case "Tserver":
                            error += "voice";
                            break;
                        case "Ixnserver":
                            error += "interaction";
                            break;
                        case "Chatserver":
                            error += "chat";
                            break;
                        case "Ucserver":
                            error += "universal contact";
                            break;
                        case "Statisticsserver":
                        case "ElavonStatisticsserver":
                            error += "stat";
                            break;

                    }
                    error += " server host '" + protocolConfig.Uri.Host + "' on port '" + protocolConfig.Uri.Port;
                }
                else
                    error = ex.Message;
            }
            return false;
        }

        public void DisConnectServer(ServerType serverType)
        {
            try
            {
                if (ProtocolManager[serverType.ToString()].State == ChannelState.Opened)
                    ProtocolManager[serverType.ToString()].Close();
                ProtocolManager.Unregister(serverType.ToString());
            }
            catch { }
        }

        #endregion Methods
    }
}