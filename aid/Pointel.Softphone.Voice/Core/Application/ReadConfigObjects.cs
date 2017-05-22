namespace Pointel.Softphone.Voice.Core.Application
{
    using System;
    using System.Collections.Generic;

    using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel;
    using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.CfgObjects;
    using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.Queries;
    using Genesyslab.Platform.Commons.Collections;
    using Genesyslab.Platform.Configuration.Protocols.Types;
    using Genesyslab.Platform.Voice.Protocols.TServer;

    using Pointel.Configuration.Manager;
    using Pointel.Softphone.Voice.Common;
    using Pointel.Softphone.Voice.Core.Util;

    /// <summary>
    /// This Class provide members to read all type of CME objects
    /// </summary>
    internal class ReadConfigObjects
    {
        #region Fields

        private Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");
        private ConfigContainer _configContainer = ConfigContainer.Instance();

        #endregion Fields

        #region Methods

        public CfgApplication ReadApplicationLevelServerDetails(string applicationName)
        {
            CfgApplication application = null;
            try
            {
                application = new CfgApplication(_configContainer.ConfServiceObject);
                CfgApplicationQuery queryApp = new CfgApplicationQuery();
                queryApp.TenantDbid = WellKnownDbids.EnterpriseModeTenantDbid;
                queryApp.Name = applicationName;
                application = _configContainer.ConfServiceObject.RetrieveObject<CfgApplication>(queryApp);
            }
            catch (Exception commonException)
            {
                logger.Error("Error occurred while reading:ReadApplicationAndConnectionTab" + applicationName + "=" + commonException.ToString());
            }
            return application;
        }

        public CfgApplication ReadApplicationObject(string tserverApplicationName, string agentLogin)
        {
            CfgApplication application = null;
            try
            {
                string[] tServers = tserverApplicationName.Split(',');
                string primaryTserver = string.Empty;
                string secondaryTServer = string.Empty;
                int switchDBID = 0;
                if (tServers != null && tServers.Length > 0)
                {
                    if (!string.IsNullOrEmpty(tServers[0]))
                        Settings.GetInstance().PrimaryTServerName = tServers[0].Trim().ToString();
                    if (tServers.Length > 1 && !string.IsNullOrEmpty(tServers[1]))
                        Settings.GetInstance().SecondaryTServerName = tServers[1].Trim().ToString();
                }
                if (!string.IsNullOrEmpty(Settings.GetInstance().PrimaryTServerName) && string.IsNullOrEmpty(Settings.GetInstance().SecondaryTServerName))
                    Settings.GetInstance().SecondaryTServerName = Settings.GetInstance().PrimaryTServerName;
                else if (!string.IsNullOrEmpty(Settings.GetInstance().SecondaryTServerName) && string.IsNullOrEmpty(Settings.GetInstance().PrimaryTServerName))
                    Settings.GetInstance().PrimaryTServerName = Settings.GetInstance().SecondaryTServerName;

                #region To get Correct switch
                application = new CfgApplication(_configContainer.ConfServiceObject);
                CfgApplicationQuery queryApp = new CfgApplicationQuery();
                if (!string.IsNullOrEmpty(Settings.GetInstance().PrimaryTServerName))
                    queryApp.Name = Settings.GetInstance().PrimaryTServerName;
                application = _configContainer.ConfServiceObject.RetrieveObject<CfgApplication>(queryApp);
                //if (application != null)
                //{
                //    var _flexibleProperties = application.FlexibleProperties;
                //    if (_flexibleProperties != null && _flexibleProperties.Count > 0)
                //    {
                //        if (_flexibleProperties.ContainsKey("CONN_INFO"))
                //        {
                //            var _connInfoCollection = _flexibleProperties["CONN_INFO"];
                //            if (_connInfoCollection != null)
                //            {
                //                var _switchCollection = (KeyValueCollection)_connInfoCollection;
                //                if (_switchCollection != null && _switchCollection.ContainsKey("CFGSwitch"))
                //                {
                //                    var _switchCollection1 = _switchCollection["CFGSwitch"];
                //                    if (_switchCollection1 != null)
                //                    {
                //                        KeyValueCollection _switchDBIds = (KeyValueCollection)_switchCollection1;
                //                        if (_switchDBIds != null && _switchDBIds.Count > 0)
                //                            foreach (string id in _switchDBIds.AllKeys)
                //                                switchDBID = Convert.ToInt32(id);
                //                    }
                //                }
                //            }
                //        }
                //    }
                //}

                //CfgAgentLoginQuery _queryAGLogin = new CfgAgentLoginQuery();
                //_queryAGLogin.LoginCode = agentLogin;
                //_queryAGLogin.TenantDbid = _configContainer.TenantDbId;
                //var agentInfo = (ICollection<CfgAgentLoginInfo>)((CfgPerson)_configContainer.GetValue("CfgPerson")).AgentInfo.AgentLogins;
                //foreach (var loginDetails in agentInfo)
                //{
                //    if (_queryAGLogin.LoginCode == loginDetails.AgentLogin.LoginCode && switchDBID == loginDetails.AgentLogin.Switch.DBID)
                //    {
                //        _queryAGLogin.Dbid = loginDetails.AgentLogin.DBID;
                //        break;
                //    }
                //}
                //CfgAgentLogin _cfgAGLogin = _configContainer.ConfServiceObject.RetrieveObject<CfgAgentLogin>(_queryAGLogin);
                //CfgApplication _cfgApplication = (_cfgAGLogin.Switch).TServer;
                //if (_cfgApplication == null)
                //    logger.Warn("The switch name " + _cfgAGLogin.Switch.Name + " does not contains T-server configurations");

                if (application != null)
                {
                    //if (application.DBID == _cfgApplication.DBID)
                    //{
                    Settings.GetInstance().PrimaryApplication = application;
                    if (Settings.GetInstance().PrimaryApplication != null)
                    {
                        Settings.GetInstance().PrimaryTServerName = Settings.GetInstance().PrimaryApplication.Name;
                    }
                    Settings.GetInstance().SecondaryApplication = application.ServerInfo.BackupServer;
                    if (Settings.GetInstance().SecondaryApplication == null)
                    {
                        Settings.GetInstance().SecondaryTServerName = Settings.GetInstance().PrimaryTServerName;
                        Settings.GetInstance().SecondaryApplication = Settings.GetInstance().PrimaryApplication;
                    }
                    else
                    {
                        Settings.GetInstance().SecondaryTServerName = Settings.GetInstance().SecondaryApplication.Name;
                    }
                    //}
                    //else
                    //{
                    //    Settings.GetInstance().PrimaryTServerName = string.Empty;
                    //    Settings.GetInstance().SecondaryTServerName = string.Empty;
                    //    Settings.GetInstance().PrimaryApplication = null;
                    //    Settings.GetInstance().SecondaryApplication = null;
                    //}

                    if (!string.IsNullOrEmpty(Settings.GetInstance().PrimaryTServerName))
                    {
                        if (Settings.GetInstance().PrimaryApplication == null)
                        {
                            Settings.GetInstance().PrimaryApplication = ReadApplicationLevelServerDetails(Settings.GetInstance().PrimaryTServerName);
                        }
                    }

                    if (!string.IsNullOrEmpty(Settings.GetInstance().SecondaryTServerName))
                    {
                        if (Settings.GetInstance().SecondaryApplication == null)
                        {
                            logger.Debug("Secondary server is retrieving from configured key");
                            Settings.GetInstance().SecondaryApplication = ReadApplicationLevelServerDetails(Settings.GetInstance().SecondaryTServerName);
                        }
                        if (Settings.GetInstance().PrimaryApplication == null && Settings.GetInstance().SecondaryApplication != null)
                        {
                            logger.Debug("Primary server is not configured, Secondary server is assigned to Primary server");
                            Settings.GetInstance().PrimaryApplication = Settings.GetInstance().SecondaryApplication;
                        }
                    }
                    else
                    {
                        logger.Debug("secondary application name is not configured");
                        if (Settings.GetInstance().SecondaryApplication == null)
                        {
                            logger.Debug("Secondary server is not configured, primary server is assigned to secondary server");
                            Settings.GetInstance().SecondaryApplication = Settings.GetInstance().PrimaryApplication;
                        }
                    }

                    if (_configContainer.AllKeys.Contains("interaction.disposition-collection.key-name") &&
                            ((string)_configContainer.GetValue("interaction.disposition-collection.key-name")) != string.Empty)
                        Settings.GetInstance().DispositionCollectionKey = ((string)_configContainer.GetValue("interaction.disposition-collection.key-name"));

                    if (_configContainer.AllKeys.Contains("interaction.disposition.key-name") &&
                            ((string)_configContainer.GetValue("interaction.disposition.key-name")) != string.Empty)
                        Settings.GetInstance().DispositionCodeKey = ((string)_configContainer.GetValue("interaction.disposition.key-name"));

                    if (_configContainer.AllKeys.Contains("login.voice.workmode") &&
                            ((string)_configContainer.GetValue("login.voice.workmode")) != string.Empty)
                        Settings.GetInstance().WorkMode = FindWorkMode((string)_configContainer.GetValue("login.voice.workmode"));

                    if (_configContainer.AllKeys.Contains("not-ready.request.attribute-name") &&
                            ((string)_configContainer.GetValue("not-ready.request.attribute-name")) != string.Empty)
                        Settings.GetInstance().NotReadyRequest = ((string)_configContainer.GetValue("not-ready.request.attribute-name"));

                    if (_configContainer.AllKeys.Contains("not-ready.code-name") &&
                            ((string)_configContainer.GetValue("not-ready.code-name")) != string.Empty)
                        Settings.GetInstance().NotReadyCodeKey = ((string)_configContainer.GetValue("not-ready.code-name"));

                    if (_configContainer.AllKeys.Contains("not-ready.key-name") &&
                            ((string)_configContainer.GetValue("not-ready.key-name")) != string.Empty)
                        Settings.GetInstance().NotReadyKey = ((string)_configContainer.GetValue("not-ready.key-name"));

                    if (_configContainer.AllKeys.Contains("attempts") &&
                            ((string)_configContainer.GetValue("attempts")) != string.Empty)
                        Settings.GetInstance().WarmStandbyAttempts = ((string)_configContainer.GetValue("attempts"));

                    if (_configContainer.AllKeys.Contains("client-timeout") &&
                            ((string)_configContainer.GetValue("client-timeout")) != string.Empty)
                        Settings.GetInstance().AddpClientTimeout = ((string)_configContainer.GetValue("client-timeout"));

                    if (_configContainer.AllKeys.Contains("log-off-enable") &&
                            ((string)_configContainer.GetValue("log-off-enable")) != string.Empty)
                        Settings.GetInstance().LogOffEnable = ((string)_configContainer.GetValue("log-off-enable")).ToLower().Equals("true") ? true : false;

                    if (_configContainer.AllKeys.Contains("server-timeout") &&
                            ((string)_configContainer.GetValue("server-timeout")) != string.Empty)
                        Settings.GetInstance().AddpServerTimeout = ((string)_configContainer.GetValue("server-timeout"));

                    if (_configContainer.AllKeys.Contains("call-control") &&
                            ((string)_configContainer.GetValue("call-control")) != string.Empty)
                        Settings.GetInstance().CallControl = ((string)_configContainer.GetValue("call-control"));

                    if (_configContainer.AllKeys.Contains("voice.enable.auto-answer") &&
                            ((string)_configContainer.GetValue("voice.enable.auto-answer")) != string.Empty)
                        Settings.GetInstance().IsAutoAnswerEnabled = ((string)_configContainer.GetValue("voice.enable.auto-answer")).ToLower().Equals("true") ? true : false;

                    if (_configContainer.AllKeys.Contains("voice.enable.delete-conf.call") &&
                            ((string)_configContainer.GetValue("voice.enable.delete-conf.call")) != string.Empty)
                        Settings.GetInstance().IsDeleteConfEnabled = ((string)_configContainer.GetValue("voice.enable.delete-conf.call")).ToLower().Equals("true") ? true : false;

                }

                #endregion
            }
            catch (Exception commonException)
            {
                logger.Error("Error occurred while reading " + tserverApplicationName + "  =  " + commonException.ToString());
            }
            return application;
        }

        public OutputValues ReadPlaceObject()
        {
            OutputValues output = OutputValues.GetInstance();
            try
            {
                Settings.GetInstance().ACDPosition = string.Empty;
                Settings.GetInstance().ExtensionDN = string.Empty;
                CfgPlace application = new CfgPlace(_configContainer.ConfServiceObject);
                CfgPlaceQuery queryApp = new CfgPlaceQuery();
                queryApp.TenantDbid = _configContainer.TenantDbId;
                queryApp.Name = Settings.GetInstance().PlaceName;
                application = _configContainer.ConfServiceObject.RetrieveObject<CfgPlace>(queryApp);

                IList<CfgDN> DNCollection = (IList<CfgDN>)application.DNs;

                if (DNCollection != null && DNCollection.Count > 0)
                {
                    foreach (CfgDN DN in DNCollection)
                    {
                        if (!String.IsNullOrEmpty(DN.Number))
                        {
                            //Code changed to get both DN's by checking its string empty value
                            if (DN.Type == CfgDNType.CFGACDPosition && Settings.GetInstance().SwitchTypeName == "nortel")
                            {
                                Settings.GetInstance().ACDPosition = DN.Number;
                            }
                            if (DN.Type == CfgDNType.CFGExtension)
                            {
                                Settings.GetInstance().ExtensionDN = DN.Number;
                                if (Settings.GetInstance().ACDPosition.Equals(string.Empty))
                                {
                                    Settings.GetInstance().ACDPosition = DN.Number;
                                }
                            }
                            //End
                        }
                    }
                }
                else
                {
                    output.MessageCode = "2001";
                    output.Message = Settings.GetInstance().ErrorMessages["place.config"];
                }

                output.MessageCode = "200";
                output.Message = "Login Successful";
            }
            catch (Exception commonException)
            {
                logger.Error("Error occurred while reading " + Settings.GetInstance().PlaceName + "  =  " + commonException.ToString());

                output.MessageCode = "2001";
                output.Message = Settings.GetInstance().ErrorMessages["place.config"];
            }
            return output;
        }

        private AgentWorkMode FindWorkMode(string workMode)
        {
            AgentWorkMode resultMode;

            if (Enum.TryParse<Genesyslab.Platform.Voice.Protocols.TServer.AgentWorkMode>(workMode, true, out resultMode))
            {
                Settings.GetInstance().IsWorkModeSet = true;
            }

            return resultMode;
        }

        #endregion Methods

        #region Other

        /// <summary>
        /// Reads the application object.
        /// </summary>
        /// <param name="applicationName">Name of the application.</param>
        /// <returns></returns>
        /// <summary>
        /// Finds the work mode.
        /// </summary>
        /// <param name="workMode">The work mode.</param>
        /// <returns></returns>
        /// <summary>
        /// Reads the application level server details.
        /// </summary>
        /// <param name="applicationName">Name of the application.</param>
        /// <returns></returns>
        /// <summary>
        /// This method used to read Extension and ACDosition DN's from the given place
        /// </summary>
        /// <returns>output</returns>

        #endregion Other
    }
}