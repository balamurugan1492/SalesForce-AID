using System;
using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel;
using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.CfgObjects;
using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.Queries;
using Genesyslab.Platform.Commons.Collections;
using Genesyslab.Platform.Configuration.Protocols.Types;

using Pointel.Interactions.Core.ConnectionManager;
using Pointel.Interactions.Core.Util;
using Pointel.Configuration.Manager;

namespace Pointel.Interactions.Core.Application
{
    internal class ReadConfigObjects
    {
        #region Field Declaration
        private Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");
        private ConfigContainer _configContainer = ConfigContainer.Instance();
        #endregion

        #region Read Application Object
        /// <summary>
        /// Reads the application object.
        /// </summary>
        /// <param name="applicationName">Name of the application.</param>
        /// <returns></returns>
        public CfgApplication ReadApplicationObject(string ixnAppName)
        {
            CfgApplication application = null;
            try
            {
                string[] tServers = ixnAppName.Split(',');
                string primaryTserver = string.Empty;
                string secondaryTServer = string.Empty;
                if (tServers != null && tServers.Length > 0)
                {
                    if (!string.IsNullOrEmpty(tServers[0]))
                        Settings.PrimaryInteractionServerName = tServers[0].Trim().ToString();
                    if (tServers.Length > 1 && !string.IsNullOrEmpty(tServers[1]))
                        Settings.SecondaryInteractionServerName = tServers[1].Trim().ToString();
                }
                if (!string.IsNullOrEmpty(Settings.PrimaryInteractionServerName) && string.IsNullOrEmpty(Settings.SecondaryInteractionServerName))
                    Settings.SecondaryInteractionServerName = Settings.PrimaryInteractionServerName;
                else if (!string.IsNullOrEmpty(Settings.SecondaryInteractionServerName) && string.IsNullOrEmpty(Settings.PrimaryInteractionServerName))
                    Settings.PrimaryInteractionServerName = Settings.SecondaryInteractionServerName;

                application = new CfgApplication(ConfigContainer.Instance().ConfServiceObject);
                CfgApplicationQuery queryApp = new CfgApplicationQuery();
                queryApp.TenantDbid = WellKnownDbids.EnterpriseModeTenantDbid;
                if (!string.IsNullOrEmpty(Settings.PrimaryInteractionServerName))
                    queryApp.Name = Settings.PrimaryInteractionServerName;
                application = ConfigContainer.Instance().ConfServiceObject.RetrieveObject<CfgApplication>(queryApp);
                if (application != null)
                {
                    if (application.Type == CfgAppType.CFGInteractionServer)
                    {
                        Settings.PrimaryApplication = application;
                        Settings.PrimaryInteractionServerName = Settings.PrimaryApplication.Name;
                        if (application.ServerInfo.BackupServer != null)
                        {
                            Settings.SecondaryApplication = application.ServerInfo.BackupServer;
                            Settings.SecondaryInteractionServerName = Settings.SecondaryApplication.Name;
                        }
                        else
                        {
                            Settings.SecondaryApplication = Settings.PrimaryApplication;
                            Settings.SecondaryInteractionServerName = Settings.PrimaryInteractionServerName;
                        }
                    }
                    else
                    {
                        Settings.PrimaryInteractionServerName = string.Empty;
                        Settings.SecondaryInteractionServerName = string.Empty;
                        Settings.PrimaryApplication = null;
                        Settings.SecondaryApplication = null;
                    }

                    if (_configContainer.AllKeys.Contains("interaction.reconnect-attempts") &&
                            ((string)_configContainer.GetValue("interaction.reconnect-attempts")) != string.Empty)
                        Settings.WarmStandbyAttempts = ((string)_configContainer.GetValue("interaction.reconnect-attempts"));

                    if (_configContainer.AllKeys.Contains("interaction.reconnect-timeout") &&
                            ((string)_configContainer.GetValue("interaction.reconnect-timeout")) != string.Empty)
                        Settings.WarmStandbyTimeout = ((string)_configContainer.GetValue("interaction.reconnect-timeout"));

                    if (_configContainer.AllKeys.Contains("interaction.server-timeout") &&
                            ((string)_configContainer.GetValue("interaction.server-timeout")) != string.Empty)
                        Settings.AddpServerTimeout = ((string)_configContainer.GetValue("interaction.server-timeout"));

                    if (_configContainer.AllKeys.Contains("interaction.client-timeout") &&
                            ((string)_configContainer.GetValue("interaction.client-timeout")) != string.Empty)
                        Settings.AddpClientTimeout = ((string)_configContainer.GetValue("interaction.client-timeout"));
                }

            }
            catch (Exception commonException)
            {
                logger.Error("Error occurred while reading application object" + ixnAppName + "  =  " + commonException.ToString());
            }
            return application;
        }
        #endregion
    }
}
