namespace Pointel.Interactions.Contact.Core.Application
{
    using System;

    using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.CfgObjects;
    using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.Queries;
    using Genesyslab.Platform.Commons.Collections;
    using Genesyslab.Platform.Configuration.Protocols.Types;

    using Pointel.Interactions.Contact.Core.Util;
    using Pointel.Configuration.Manager;
    using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel;

    internal class ReadConfigObjects
    {
        #region Fields

        private Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");
        private ConfigContainer _configContainer = ConfigContainer.Instance();
        #endregion Fields

        #region Methods
        /// <summary>
        /// Reads the application object.
        /// </summary>
        /// <param name="contactAppName">Name of the application.</param>
        /// <returns></returns>
        public CfgApplication ReadApplicationObject(string contactAppName)
        {
            CfgApplication application = null;
            try
            {
                string[] tServers = contactAppName.Split(',');
                string primaryTserver = string.Empty;
                string secondaryTServer = string.Empty;
                if (tServers != null && tServers.Length > 0)
                {
                    if (!string.IsNullOrEmpty(tServers[0]))
                        Settings.PrimaryContactServerName = tServers[0].Trim().ToString();
                    if (tServers.Length > 1 && !string.IsNullOrEmpty(tServers[1]))
                        Settings.SecondaryContactServerName = tServers[1].Trim().ToString();
                }
                if (!string.IsNullOrEmpty(Settings.PrimaryContactServerName) && string.IsNullOrEmpty(Settings.SecondaryContactServerName))
                    Settings.SecondaryContactServerName = Settings.PrimaryContactServerName;
                else if (!string.IsNullOrEmpty(Settings.SecondaryContactServerName) && string.IsNullOrEmpty(Settings.PrimaryContactServerName))
                    Settings.PrimaryContactServerName = Settings.SecondaryContactServerName;

                application = new CfgApplication(Settings.comObject);
                CfgApplicationQuery queryApp = new CfgApplicationQuery();
                queryApp.TenantDbid = WellKnownDbids.EnterpriseModeTenantDbid;
                if (!string.IsNullOrEmpty(Settings.PrimaryContactServerName))
                    queryApp.Name = Settings.PrimaryContactServerName;
                application = Settings.comObject.RetrieveObject<CfgApplication>(queryApp);
                if (application != null)
                {
                    if (application.Type == CfgAppType.CFGContactServer)
                    {
                        Settings.PrimaryApplication = application;
                        Settings.PrimaryContactServerName = Settings.PrimaryApplication.Name;
                        if (application.ServerInfo.BackupServer != null)
                        {
                            Settings.SecondaryApplication = application.ServerInfo.BackupServer;
                            Settings.SecondaryContactServerName = Settings.SecondaryApplication.Name;
                        }
                        else
                        {
                            Settings.SecondaryApplication = Settings.PrimaryApplication;
                            Settings.SecondaryContactServerName = Settings.PrimaryContactServerName;
                        }
                    }
                    else
                    {
                        Settings.PrimaryContactServerName = string.Empty;
                        Settings.SecondaryContactServerName = string.Empty;
                        Settings.PrimaryApplication = null;
                        Settings.SecondaryApplication = null;
                    }

                    if (_configContainer.AllKeys.Contains("contact.reconnect-attempts") &&
                            ((string)_configContainer.GetValue("contact.reconnect-attempts")) != string.Empty)
                        Settings.WarmStandbyAttempts = ((string)_configContainer.GetValue("contact.reconnect-attempts"));

                    if (_configContainer.AllKeys.Contains("contact.reconnect-timeout") &&
                            ((string)_configContainer.GetValue("contact.reconnect-timeout")) != string.Empty)
                        Settings.WarmStandbyTimeout = ((string)_configContainer.GetValue("contact.reconnect-timeout"));

                    if (_configContainer.AllKeys.Contains("contact.server-timeout") &&
                            ((string)_configContainer.GetValue("contact.server-timeout")) != string.Empty)
                        Settings.AddpServerTimeout = ((string)_configContainer.GetValue("contact.server-timeout"));

                    if (_configContainer.AllKeys.Contains("contact.client-timeout") &&
                            ((string)_configContainer.GetValue("contact.client-timeout")) != string.Empty)
                        Settings.AddpClientTimeout = ((string)_configContainer.GetValue("contact.client-timeout"));
                }

            }
            catch (Exception commonException)
            {
                logger.Error("Error occurred while reading application object" + contactAppName + "  =  " + commonException.ToString());
            }
            return application;
        }

        #endregion Methods
    }
}