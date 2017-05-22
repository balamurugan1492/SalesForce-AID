#region Header

/*
* =======================================
* Pointel.Configuration.Manager.Core
* =======================================
* Project    : Agent Interaction Desktop
* Created on : 31-March-2015
* Author     : Manikandan
* Owner      : Pointel Solutions
* =======================================
*/

#endregion Header

namespace Pointel.Configuration.Manager
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel;
    using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.CfgObjects;
    using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.Queries;
    using Genesyslab.Platform.Commons.Protocols;
    using Genesyslab.Platform.Configuration.Protocols.ConfServer.Events;
    using Genesyslab.Platform.Configuration.Protocols.ConfServer.Requests.Security;

    using Pointel.Configuration.Manager.Common;
    using Pointel.Configuration.Manager.ConnectionManager;
    using Pointel.Configuration.Manager.Core;
    using Pointel.Configuration.Manager.Helpers;
    using Pointel.Connection.Manager;

    /// <summary>
    /// 
    /// </summary>
    public class ConfigManager
    {
        #region Fields

        ConfigContainer _configContainer;
        private Pointel.Logger.Core.ILog _logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
             "AID");

        #endregion Fields

        #region Methods

        /// <summary>
        /// Establish connection to the configuration server
        /// </summary>
        /// <param name="pri_ConfigServerHost">The pri_ configuration server host.</param>
        /// <param name="pri_ConfigServerPort">The pri_ configuration server port.</param>
        /// <param name="clientName">Name of the client.</param>
        /// <param name="applicationName">Name of the application.</param>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        /// <param name="sec_ConfigServerHost">The sec_ configuration server host.</param>
        /// <param name="sec_ConfigServerPort">The sec_ configuration server port.</param>
        /// <returns></returns>
        public OutputValues ConfigConnectionEstablish(string pri_ConfigServerHost, string pri_ConfigServerPort, string applicationName,
            string userName, string password, string sec_ConfigServerHost, string sec_ConfigServerPort, string[] sectionToRead, params string[] sections)
        {
            _configContainer = null;
            _configContainer = ConfigContainer.Instance();
            OutputValues output = new OutputValues();
            try
            {
                var assembly = typeof(ConfigManager).Assembly;
                if (assembly != null)
                {
                    _logger.Info("-----------------------------------------------------------------------------");
                    _logger.Info(assembly.GetName().Name + " : " + assembly.GetName().Version);
                    _logger.Info("-----------------------------------------------------------------------------");
                }
            }
            catch (Exception generalException)
            {
                _logger.Error("Error occurred while getting the assembly version of library : " +
                    ((generalException.InnerException == null) ? generalException.Message : generalException.InnerException.ToString()));
            }
            ConfigConnectionManager connect = new ConfigConnectionManager();

            //Clear and null all values
            if (_configContainer.ConfServiceObject != null)
            {
                if (_configContainer.ConfServiceObject.Protocol != null && _configContainer.ConfServiceObject.Protocol.State == ChannelState.Opened)
                    _configContainer.ConfServiceObject.Protocol.Close();
                if (_configContainer.ConfServiceObject != null)
                    ConfServiceFactory.ReleaseConfService(_configContainer.ConfServiceObject);
                _configContainer.ConfServiceObject = null;
            }
            //_configContainer.AllKeys.Clear();
            _configContainer.CMEValues.Clear();
            //Connect Config Server
            output = connect.ConnectConfigServer(pri_ConfigServerHost, pri_ConfigServerPort, applicationName, userName, password,
                                                 sec_ConfigServerHost, sec_ConfigServerPort);

            if (output.MessageCode == "200")
            {
                //Authenticate the user first and read the CME configurations
                output = AuthenticateUser(userName, password);
                if (output.MessageCode == "200")
                {
                    ConfigurationHandler readValues = new ConfigurationHandler();

                    ConfigContainer.Instance().UserName = userName;

                    //Get access group first then read values hierarchy

                    //Read System section to get Tenant and switch type
                    output = readValues.ReadSystemSection(applicationName);

                    if (output.MessageCode == "2001")
                        return output;
                    //Register for CME Alter Notification
                    //readValues.RegisterCMEAlterNotification(_configContainer.TenantDbId);

                    //Read logger to print CME values in log

                    readValues.ReadLoggerData(userName, applicationName);
                    InitializeLogger(userName);
                    //Read Application keys
                    readValues.ReadApplication(applicationName, sectionToRead, sections);
                    //Read Agent Group Keys and override existing application keys
                    readValues.ReadAgentGroup(userName, sectionToRead, sections);
                    //Read person Keys and override existing application keys and Agent group keys

                    readValues.ReadPerson(userName, sectionToRead, sections);
                    GetContactBusinessAttribute("ContactAttributes");
                    //Check the user is in access group
                    bool accessAuthenticationEnable = true;
                    try
                    {
                        if (_configContainer.CMEValues.ContainsKey("login.enable.access.group-authentication"))
                            if (!string.IsNullOrEmpty(((ConfigValue)_configContainer.CMEValues["login.access-group"]).Value.ToString())
                                && !Convert.ToBoolean(((ConfigValue)_configContainer.CMEValues["login.enable.access.group-authentication"]).Value.ToString()))
                                accessAuthenticationEnable = false;
                    }
                    catch
                    {
                        _logger.Warn("Authentication using access group may not work as expected. Implementing default functionality.");
                    }
                    if (_configContainer.CMEValues.ContainsKey("login.access-group"))
                    {
                        if (_configContainer.CMEValues["login.access-group"] != null &&
                                    ((ConfigValue)_configContainer.CMEValues["login.access-group"]).Value.ToString() != string.Empty)
                        {
                            if (accessAuthenticationEnable)
                            {
                                output = readValues.ReadAccessPermission(((ConfigValue)_configContainer.CMEValues["login.access-group"]).Value);
                                if (output.MessageCode == "2001")
                                {
                                    return output;
                                }
                            }
                        }
                        else
                        {
                            _logger.Warn("login.access-group value is null or empty");
                            output.MessageCode = "2001";
                            output.Message = "Access group name is not configured. Please contact your Administrator";
                            return output;
                        }
                    }
                    else
                    {
                        _logger.Warn("login.access-group key is not configured");
                        output.MessageCode = "2001";
                        output.Message = "Access group name is not configured. Please contact your Administrator";
                        return output;
                    }
                    //Read the available Queues such as ACD queues
                    if (_configContainer.CMEValues.ContainsKey("login.voice.available-queues") && ((ConfigValue)_configContainer.CMEValues["login.voice.available-queues"]).Value != null)
                        readValues.ReadQueues(((ConfigValue)_configContainer.CMEValues["login.voice.available-queues"]).Value.ToString());
                    else
                        _logger.Warn("login.voice.available-queues key-value is not configured");

                    //Check case data and disposition from business attribute or transaction object
                    if (_configContainer.CMEValues.ContainsKey("interaction.casedata.use-transaction-object") &&
                        ((ConfigValue)_configContainer.CMEValues["interaction.casedata.use-transaction-object"]).Value.ToLower().Equals("true") ? true : false)
                    {
                        //Read the case data Transaction object for add/filter/sort case data
                        if (_configContainer.CMEValues.ContainsKey("interaction.casedata-object-name") &&
                            !string.IsNullOrEmpty(((ConfigValue)_configContainer.CMEValues["interaction.casedata-object-name"]).Value))
                        {

                            readValues.ReadCaseDataTransactionObject(((ConfigValue)_configContainer.CMEValues["interaction.casedata-object-name"]).Value.ToString());
                            readValues.ReadCaseDataFilterTransactionObject(((ConfigValue)_configContainer.CMEValues["interaction.casedata-object-name"]).Value.ToString());
                            readValues.ReadCaseDataSortingTransactionObject(((ConfigValue)_configContainer.CMEValues["interaction.casedata-object-name"]).Value.ToString());
                        }
                        else
                            _logger.Warn("interaction.casedata-object-name value is not configured");

                        //Read the case data Transaction object for disposition
                        if (_configContainer.CMEValues.ContainsKey("interaction.disposition-object-name") &&
                            !string.IsNullOrEmpty(((ConfigValue)_configContainer.CMEValues["interaction.disposition-object-name"]).Value))
                            readValues.ReadDispositionTransctionObject(((ConfigValue)_configContainer.CMEValues["interaction.disposition-object-name"]).Value.ToString());
                        else
                            _logger.Warn("interaction.disposition-object-name value is not configured");
                    }
                    else
                    {
                        if (_configContainer.CMEValues.ContainsKey("interaction.casedata-object-name") &&
                            !string.IsNullOrEmpty(((ConfigValue)_configContainer.CMEValues["interaction.casedata-object-name"]).Value))
                        {
                            //Read casedata information from Business attributes
                            readValues.ReadCaseDataFromBusinessAttribute(((ConfigValue)_configContainer.CMEValues["interaction.casedata-object-name"]).Value.ToString());
                        }
                        else
                            _logger.Warn("interaction.casedata-object-name key-value is not configured");

                        if (_configContainer.CMEValues.ContainsKey("interaction.disposition-object-name") &&
                            !string.IsNullOrEmpty(((ConfigValue)_configContainer.CMEValues["interaction.disposition-object-name"]).Value))
                        {
                            //Read disposition information from Business attributes
                            readValues.ReadDispositionFromBusinessAttribute(((ConfigValue)_configContainer.CMEValues["interaction.disposition-object-name"]).Value.ToString());
                        }
                        else
                            _logger.Warn("interaction.disposition-object-name key-value is not configured");
                    }
                    //Read all skills
                    readValues.ReadAllSkills();
                    //Read all places
                    readValues.ReadAllPlaces();
                    //Read all DN's based on switch type
                    //readValues.ReadAllDNs();
                    //Read channel based not ready reason codes for voice, email and chat medias
                    readValues.ReadChannelNotReadyReasonCodes();
                    //Read global not ready reason codes
                    if (_configContainer.AllKeys.Contains("agent-global-status.not-ready-reasoncodes"))
                    {
                        string[] notReadyReasonCodes = ((string)_configContainer.GetValue("agent-global-status.not-ready-reasoncodes")).Split(',');
                        if (notReadyReasonCodes.Length > 0)
                            readValues.ReadGlobalNotReadyReasonCodes(notReadyReasonCodes.Distinct().ToArray<string>());
                    }
                    //Read Voice not Ready Reason Codes
                    if (_configContainer.AllKeys.Contains("agent-voice-status.not-ready-reasoncodes"))
                    {
                        string[] notReadyReasonCodes = ((string)_configContainer.GetValue("agent-voice-status.not-ready-reasoncodes")).Split(',');
                        if (notReadyReasonCodes.Length > 0)
                            readValues.ReadVoiceHierarchyLevelNotReadyReasonCodes(notReadyReasonCodes.Distinct().ToArray<string>());
                    }

                    CfgPersonQuery qPerson = new CfgPersonQuery();
                    qPerson.TenantDbid = ConfigContainer.Instance().TenantDbId;
                    qPerson.IsAgent = 0;// 0 - Means select only Agents (or) 1 - means select only user(default)
                    System.Collections.Generic.ICollection<CfgPerson> _allPersons = _configContainer.ConfServiceObject.RetrieveMultipleObjects<CfgPerson>(qPerson);
                    //_configContainer.AllKeys.Add("AllPersons");
                    _configContainer.CMEValues.Add("AllPersons", _allPersons);
                    readValues.RegisterCMEAlterNotification(ConfigContainer.Instance().TenantDbId);
                    readValues = null;
                }
                else
                    return output;
            }
            return output;
        }

        /// <summary>
        /// Disconnects the configuration server.
        /// </summary>
        public void DisconnectConfigServer()
        {
            try
            {
                if (ProtocolManagers.Instance().ProtocolManager[ServerType.Configserver.ToString()].State == ChannelState.Opened ||
                    ProtocolManagers.Instance().ProtocolManager[ServerType.Configserver.ToString()].State == ChannelState.Opening)
                {
                    try
                    {
                        if (ConfigContainer.Instance().ConfServiceObject != null)
                            ConfServiceFactory.ReleaseConfService(ConfigContainer.Instance().ConfServiceObject);
                        ProtocolManagers.Instance().DisConnectServer(ServerType.Configserver);
                        if (_configContainer.ConfServiceObject != null)
                        {
                            if (_configContainer.ConfServiceObject.Protocol != null && _configContainer.ConfServiceObject.Protocol.State == ChannelState.Opened)
                                _configContainer.ConfServiceObject.Protocol.Close();
                            _configContainer.ConfServiceObject = null;
                        }
                        //_configContainer.AllKeys.Clear();
                        _configContainer.CMEValues.Clear();

                    }
                    catch { }
                }
                else if (ProtocolManagers.Instance().ProtocolManager[ServerType.Configserver.ToString()].State == ChannelState.Closed)
                {

                    //_configContainer.AllKeys.Clear();
                    _configContainer.CMEValues.Clear();
                }
            }
            catch (Exception generalException)
            {
                _logger.Error("Error occurred while closing the config server : " +
                    ((generalException.InnerException == null) ? generalException.Message : generalException.InnerException.ToString()));
            }
        }

        public CfgEnumeratorValue GetBusinessAttribute(string bussinessAttributeName, string attributeValueName)
        {
            if (string.IsNullOrEmpty(bussinessAttributeName))
                throw new Exception("The business attribute name is null or empty.");

            CfgEnumeratorValue objBusinessAttribute = null;
            try
            {
                if (ConfigContainer.Instance().ConfServiceObject != null)
                {
                    CfgEnumeratorQuery enumQuery = new CfgEnumeratorQuery();
                    enumQuery.TenantDbid = ConfigContainer.Instance().TenantDbId;
                    enumQuery.Name = bussinessAttributeName;
                    CfgEnumerator enumarator = ConfigContainer.Instance().ConfServiceObject.RetrieveObject<CfgEnumerator>(enumQuery);
                    if (enumarator != null)
                    {
                        CfgEnumeratorValueQuery enumaeratorValueQuery = new CfgEnumeratorValueQuery();
                        enumaeratorValueQuery.EnumeratorDbid = enumarator.DBID;
                        enumaeratorValueQuery.Name = attributeValueName;
                        objBusinessAttribute = ConfigContainer.Instance().ConfServiceObject.RetrieveObject<CfgEnumeratorValue>(enumaeratorValueQuery);
                    }
                }
            }
            catch (Exception generalException)
            {
                _logger.Error("Error occurred as " + generalException.Message);
            }
            return objBusinessAttribute;
        }

        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        public void GetContactBusinessAttribute(string businessAttributeName)
        {
            try
            {
                CfgEnumeratorQuery enumaratorQuery = new CfgEnumeratorQuery();
                enumaratorQuery.TenantDbid = ConfigContainer.Instance().TenantDbId;
                enumaratorQuery.Name = businessAttributeName;
                if (ConfigContainer.Instance().ConfServiceObject != null)
                {
                    CfgEnumerator enumarator = ConfigContainer.Instance().ConfServiceObject.RetrieveObject<CfgEnumerator>(enumaratorQuery);
                    if (enumarator != null)
                    {
                        CfgEnumeratorValueQuery enumaeratorValueQuery = new CfgEnumeratorValueQuery();
                        enumaeratorValueQuery.EnumeratorDbid = enumarator.DBID;
                        // enumarator.SetTenantDBID(ConfigContainer.Instance().TenantDbId);
                        ICollection<CfgEnumeratorValue> enumeratorValue = ConfigContainer.Instance().ConfServiceObject.RetrieveMultipleObjects<CfgEnumeratorValue>(enumaeratorValueQuery);
                        Dictionary<string, string> contactBusinessAttribute = new Dictionary<string, string>();
                        foreach (CfgEnumeratorValue enumVal in enumeratorValue)
                        {
                            contactBusinessAttribute.Add(enumVal.Name, enumVal.DisplayName);
                        }
                        _configContainer.CMEValues.Add("contactBusinessAttribute", contactBusinessAttribute);
                        // _configContainer.AllKeys.Add("contactBusinessAttribute");
                    }
                }

            }
            catch (OperationCanceledException ex)
            {
                _logger.Warn("Get Operation Cancelled issue while reading contact business attribute as " + ex.Message);
                GetContactBusinessAttribute(businessAttributeName);
            }
            catch (Exception ex)
            {
                _logger.Error("Error occurred as while reading contact attribute as " + ex.Message);
            }
        }

        /// <summary>
        /// Reads all DNS.
        /// </summary>
        /// <param name="SwitchDbID">The switch database identifier.</param>
        public void ReadAllDns(int SwitchDbID)
        {
            ConfigurationHandler ch = new ConfigurationHandler();
            ch.ReadAllDNs(SwitchDbID);
        }

        /// <summary>
        /// Authenticates the user.
        /// </summary>
        internal OutputValues AuthenticateUser(string userName, string password)
        {
            OutputValues output = new OutputValues();
            try
            {
                RequestAuthenticate requestAuthenticate = RequestAuthenticate.Create(userName, password);
                IMessage response = ProtocolManagers.Instance().ProtocolManager[ServerType.Configserver.ToString()].Request(requestAuthenticate);
                if (response != null)
                {
                    switch (response.Id)
                    {
                        case EventAuthenticated.MessageId:
                            {
                                output.MessageCode = "200";
                                output.Message = "User " + userName + "  Authenticated ";
                                _logger.Info("User " + userName + "  Authenticated ");

                                CfgPersonQuery qPerson = new CfgPersonQuery();
                                qPerson.UserName = userName;

                                CfgPerson person = _configContainer.ConfServiceObject.RetrieveObject<CfgPerson>(qPerson);
                                if (person != null)
                                {
                                    CfgTenant tenant = person.Tenant;
                                    if (tenant != null)
                                        _configContainer.TenantDbId = tenant.DBID;
                                }
                                break;
                            }
                        case EventError.MessageId:
                            {
                                EventError eventError = (EventError)response;
                                if (eventError.Description != null)
                                {
                                    _logger.Warn("User " + userName + "  is not Authenticated   " + eventError.Description);
                                }
                                output.MessageCode = "2001";
                                output.Message = "User " + userName + "  not Authenticated ";
                                _logger.Warn("User " + userName + "  is not Authenticated   ");
                                break;
                            }
                    }
                }
            }
            catch (Exception commonException)
            {
                _logger.Error("Error occurred while authenticating user " + userName + "  " + commonException.ToString());
            }
            return output;
        }

        /// <summary>
        /// Initializes the logger.
        /// </summary>
        internal void InitializeLogger(string userName)
        {
            int maxRollBacks = 0;
            string maxFileSize = string.Empty;
            string logRollStyle = string.Empty;
            string conversionPattern = string.Empty;
            string logFileName = string.Empty;
            string logFilterLevel = string.Empty;
            string levelsToFilter = string.Empty;
            string datePattern = string.Empty;
            try
            {
                if (_configContainer.CMEValues.ContainsKey("log.max-roll-size"))
                    maxRollBacks = Convert.ToInt32(_configContainer.GetValue("log.max-roll-size"));

                if (_configContainer.CMEValues.ContainsKey("log.max-file-size"))
                    maxFileSize = _configContainer.GetValue("log.max-file-size");

                if (_configContainer.CMEValues.ContainsKey("log.conversion-pattern"))
                    conversionPattern = _configContainer.GetValue("log.conversion-pattern");

                if (_configContainer.CMEValues.ContainsKey("log.date-pattern"))
                    datePattern = _configContainer.GetValue("log.date-pattern");

                if (_configContainer.CMEValues.ContainsKey("log.levels-to-filter"))
                    levelsToFilter = _configContainer.GetValue("log.levels-to-filter");

                if (_configContainer.CMEValues.ContainsKey("log.file-name"))
                    logFileName = _configContainer.GetValue("log.file-name");

                if (_configContainer.CMEValues.ContainsKey("log.filter-level"))
                    logFilterLevel = _configContainer.GetValue("log.filter-level");

                if (_configContainer.CMEValues.ContainsKey("log.roll-style"))
                    logRollStyle = _configContainer.GetValue("log.roll-style");

                var lastchar = logFileName.Contains(@"\") ? @"\" : @"/";

                if (!logFileName.Substring(logFileName.Length - 1, 1).Contains(lastchar))
                    logFileName = logFileName + lastchar;

                //var directoryInfo = new DirectoryInfo(logFileName);
                //if (logFileName == string.Empty || !Directory.Exists(directoryInfo.Root.Name))
                //{
                //    string folder = Path.Combine(Environment.CurrentDirectory.ToString(), "Logs");
                //    if (!Directory.Exists(folder))
                //        Directory.CreateDirectory(folder);
                //    logFileName = folder + @"\";
                //}

                Pointel.Logger.Core.Logger.ConfigureLog4net(maxRollBacks.ToString(), maxFileSize, logRollStyle, conversionPattern, logFileName + userName, logFilterLevel, levelsToFilter, datePattern);
                //Pointel.Logger.Core.Logger.ConfigureLog4net(maxRollBacks, maxFileSize, logRollStyle, conversionPattern, logFileName + userName, logFilterLevel, levelsToFilter, datePattern);

                var assembly = Assembly.GetExecutingAssembly();
                var fvi = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);
                DateTime buildDate = new FileInfo(Assembly.GetExecutingAssembly().Location).LastWriteTime;

                _logger.Info("---------------------------------------------------------------");
                _logger.Info("   Agent Interaction Desktop Application   ");
                _logger.Info("   Version : " + FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion);
                _logger.Info("   Framework : " + Environment.Version);
                var currentOs = Environment.OSVersion;
                _logger.Info("   Environment : " + currentOs.VersionString);
                _logger.Info("---------------------------------------------------------------");
            }
            catch { }
            finally
            {
                maxRollBacks = 0;
                maxFileSize = null;
                logRollStyle = null;
                conversionPattern = null;
                logFileName = null;
                logFilterLevel = null;
                levelsToFilter = null;
                datePattern = null;
            }
        }

        #endregion Methods
    }
}