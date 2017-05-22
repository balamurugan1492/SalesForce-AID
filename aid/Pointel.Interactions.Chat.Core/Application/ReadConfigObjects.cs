using System;
using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.CfgObjects;
using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.Queries;
using Genesyslab.Platform.Configuration.Protocols.Types;

using Pointel.Interactions.Chat.Core.ConnectionManager;
using Pointel.Interactions.Chat.Core.Util;
using Pointel.Configuration.Manager;

namespace Pointel.Interactions.Chat.Core.Application
{
    internal class ReadConfigObjects
    {
        #region Field Declaration
        private Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");
        #endregion

        #region Read Application Object
        /// <summary>
        /// Reads the application object.
        /// </summary>
        /// <param name="applicationName">Name of the application.</param>
        /// <returns></returns>
        public CfgApplication ReadApplicationObject(string applicationName)
        {
            CfgApplication application = null;
            try
            {
                ChatConnectionSettings.GetInstance().TenantDBID = ConfigContainer.Instance().TenantDbId;
                application = new CfgApplication(ChatConnectionSettings.GetInstance().ComObject);
                CfgApplicationQuery queryApp = new CfgApplicationQuery();
                // queryApp.TenantDbid = ChatConnectionSettings.GetInstance().TenantDBID;
                queryApp.Name = applicationName;
                application = ChatConnectionSettings.GetInstance().ComObject.RetrieveObject<CfgApplication>(queryApp);
                if (application != null)
                {
                    if (application.Type == CfgAppType.CFGChatServer)
                    {
                        Settings.PrimaryApplication = application;
                        if (Settings.PrimaryApplication != null)
                        {
                            Settings.PrimaryChatServerName = Settings.PrimaryApplication.Name;
                        }
                        Settings.SecondaryApplication = application.ServerInfo.BackupServer;
                        if (Settings.SecondaryApplication == null)
                        {
                            Settings.SecondaryChatServerName = Settings.PrimaryChatServerName;
                            Settings.SecondaryApplication = Settings.PrimaryApplication;
                        }
                        else
                        {
                            Settings.SecondaryChatServerName = Settings.SecondaryApplication.Name;
                        }
                    }
                    else
                    {
                        Settings.PrimaryApplication = null;
                        Settings.PrimaryChatServerName = string.Empty;
                        Settings.SecondaryApplication = null;
                        Settings.SecondaryChatServerName = string.Empty;
                    }
                }
                if (ConfigContainer.Instance().AllKeys.Contains("CfgPerson"))
                    Settings.Person = ConfigContainer.Instance().GetValue("CfgPerson");
                if (ConfigContainer.Instance().AllKeys.Contains("chat.nickname"))
                    Settings.NickNameFormat = (string)ConfigContainer.Instance().GetValue("chat.nickname");
                if (ConfigContainer.Instance().AllKeys.Contains("client-timeout"))
                    Settings.AddpClientTimeout = (string)ConfigContainer.Instance().GetValue("client-timeout");
                if (ConfigContainer.Instance().AllKeys.Contains("server-timeout"))
                    Settings.AddpServerTimeout = (string)ConfigContainer.Instance().GetValue("server-timeout");
                if (ConfigContainer.Instance().AllKeys.Contains("chat.reconnect-attempts"))
                    Settings.WarmStandbyAttempts = (string)ConfigContainer.Instance().GetValue("chat.reconnect-attempts");
                if (ConfigContainer.Instance().AllKeys.Contains("chat.reconnect-timeout"))
                    Settings.WarmStandbyTimeout = (string)ConfigContainer.Instance().GetValue("chat.reconnect-timeout");
            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred while reading application object" + applicationName + "  =  " + generalException.ToString());
            }
            return application;
        }
        #endregion

        #region Read Application Level Server Details
        /// <summary>
        /// Reads the application level server details.
        /// </summary>
        /// <param name="applicationName">Name of the application.</param>
        /// <returns></returns>
        public CfgApplication ReadApplicationLevelServerDetails(string applicationName)
        {
            CfgApplication application = null;
            try
            {
                application = new CfgApplication(ChatConnectionSettings.GetInstance().ComObject);
                CfgApplicationQuery queryApp = new CfgApplicationQuery();
                //queryApp.TenantDbid = ChatConnectionSettings.GetInstance().TenantDBID;
                queryApp.Name = applicationName;
                application = ChatConnectionSettings.GetInstance().ComObject.RetrieveObject<CfgApplication>(queryApp);
            }
            catch (Exception generalException)
            {
                logger.Error("Error Occurred while reading:ReadApplicationLevelServerDetails()" + applicationName + "=" + generalException.ToString());
            }
            return application;
        }
        #endregion
    }
}
