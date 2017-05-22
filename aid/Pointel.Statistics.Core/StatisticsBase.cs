namespace Pointel.Statistics.Core
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Data.OracleClient;
    using System.Data.SqlClient;
    using System.Data.SQLite;
    using System.Drawing;
    using System.IO;
    using System.IO.Pipes;
    using System.Linq;
    using System.Security.AccessControl;
    using System.Security.Principal;
    using System.Text;
    using System.Threading;
    using System.Windows.Forms;
    using System.Xml;
    using System.Xml.Linq;

    using Genesyslab.Platform.ApplicationBlocks.Commons.Broker;
    using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel;
    using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.CfgObjects;
    using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.Queries;
    using Genesyslab.Platform.Commons.Collections;
    using Genesyslab.Platform.Commons.Protocols;
    using Genesyslab.Platform.Configuration.Protocols.Types;
    using Genesyslab.Platform.Reporting.Protocols;
    using Genesyslab.Platform.Reporting.Protocols.StatServer;
    using Genesyslab.Platform.Reporting.Protocols.StatServer.Events;
    using Genesyslab.Platform.Reporting.Protocols.StatServer.Requests;

    using Pointel.Logger.Core;
    using Pointel.Statistics.Core;
    using Pointel.Statistics.Core.Application;
    using Pointel.Statistics.Core.ConnectionManager;
    using Pointel.Statistics.Core.General;
    using Pointel.Statistics.Core.Provider;
    using Pointel.Statistics.Core.StatisticsProvider;
    using Pointel.Statistics.Core.StatisticsRequest;
    using Pointel.Statistics.Core.Subscriber;
    using Pointel.Statistics.Core.Utility;

    public class StatisticsBase
    {
        #region Fields

        public static Dictionary<string, IStatTicker> ToClient = new Dictionary<string, IStatTicker>();

        public int AlertAudioDuration;
        public int AlertNotifyTime;
        public int AlertPopupAttempt;
        public string ApplicationType = string.Empty;
        public int AudioPlayAttempt;
        public string currentAgentState = string.Empty;
        public string DefaultAuthentication = string.Empty;
        public Dictionary<string, bool> DictFolderPermissions = new Dictionary<string, bool>();
        public int ErrorCount = 0;
        public double GadgetWidth;
        public bool IsAllType = false;
        public bool IsCCStats = false;
        public bool isCMEAuthentication = false;
        public bool isDBAuthentication = false;
        public bool isGadgetClose;
        public bool IsGenesysSupport = false;
        public bool IsMandatoryFieldsMissing = false;
        public bool IsMyStats = false;
        public bool isPlugin;
        public bool IsSystemDraggable;
        public bool IsThresholdNotifierBold = false;
        public List<string> ListMissedValues = new List<string>();
        public string QueueDisplayName = string.Empty;
        public bool StatBold;
        public int statsIntervalTime;
        public string StatSource = string.Empty;
        public string ThresholdBreachAlertPath = string.Empty;
        public string ThresholdContentBackground = string.Empty;
        public string ThresholdContentForeground = string.Empty;
        public string ThresholdTitleBackground = string.Empty;

        private static ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().
             DeclaringType, "STF");

        //static IStatisticsCollection statisticsCollection;
        static StatisticsDataProvider newStatisticsDataProvider;
        static StatisticsBase singletonInstance;

        ACDStatisticsSubscriber acdSubscriber;
        AgentGroupStatisticsSubscriber agentGroupSubscriber;
        AgentStatisticsSubscriber agentSubscriber;
        CommonStatisticsSubscriber commonSubscriber;
        DNGroupStatisticsSubscriber dngroupSubscriber;
        bool IsSubscribed = false;
        StatisticsSetting settings = StatisticsSetting.GetInstance();
        StatVariables statInfo = new StatVariables();
        StatisticsInformation statisticsInformation = new StatisticsInformation();
        Request statRequest = new Request();
        VQStatisticsSubscriber vqSubscriber;

        #endregion Fields

        #region Methods

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <returns></returns>
        public static StatisticsBase GetInstance()
        {
            if (singletonInstance == null)
            {
                singletonInstance = new StatisticsBase();
                return singletonInstance;
            }
            return singletonInstance;
        }

        public void CancelExistingRequest()
        {
            try
            {
                if (StatisticsSetting.GetInstance().RequestIds.Count != 0)
                {
                    foreach (int refId in StatisticsSetting.GetInstance().RequestIds)
                    {
                        RequestCloseStatistic closeStat = RequestCloseStatistic.Create(refId);
                        settings.statServerProtocol.Request(closeStat);
                    }
                }
            }
            catch (Exception GeneralException)
            {

            }
        }

        public void CancelStatRequest()
        {
            try
            {
                Request objRequest = new Request();

                if (StatisticsSetting.GetInstance().ListRequestIds.Count > 0)
                {
                    foreach (int id in StatisticsSetting.GetInstance().ListRequestIds)
                    {
                        objRequest.CancelRequest(id);
                    }
                }
                StatisticsSetting.GetInstance().ListRequestIds.Clear();
            }
            catch (Exception generalException)
            {
                logger.Error("StatisticsBase : CancelStatRequest Method : " + generalException.Message);
            }
        }

        public bool CheckExistingProperties(string type)
        {
            StatisticsSetting.GetInstance().IsExist = false;
            try
            {
                logger.Debug("StatisticsBase : CheckExistingProperties Method : Entry");

                if (StatisticsSetting.GetInstance().Application != null)
                {
                    #region Check Application Options

                    KeyValueCollection applicationOptions = (KeyValueCollection)StatisticsSetting.GetInstance().Application.Options["agent.ixn.desktop"];

                    if (applicationOptions != null)
                    {
                        logger.Info("StatisticsBase : CheckExistingProperties Method : Checking Application options for existing configurations");

                        if (string.Compare("statistics", type, true) == 0)
                        {
                            if (applicationOptions.ContainsKey("agent-statistics") && (applicationOptions["agent-statistics"] != string.Empty || applicationOptions["agent-statistics"] != ""))
                            {
                                StatisticsSetting.GetInstance().IsExist = true;
                            }

                            if (applicationOptions.ContainsKey("agent-group-statistics") && (applicationOptions["agent-group-statistics"] != string.Empty || applicationOptions["agent-group-statistics"] != ""))
                            {
                                StatisticsSetting.GetInstance().IsExist = true;
                            }

                            if (applicationOptions.ContainsKey("acd-queue-statistics") && (applicationOptions["acd-queue-statistics"] != string.Empty || applicationOptions["acd-queue-statistics"] != ""))
                            {
                                StatisticsSetting.GetInstance().IsExist = true;
                            }

                            if (applicationOptions.ContainsKey("dn-group-statistics") && (applicationOptions["dn-group-statistics"] != string.Empty || applicationOptions["dn-group-statistics"] != ""))
                            {
                                StatisticsSetting.GetInstance().IsExist = true;
                            }

                            if (applicationOptions.ContainsKey("virtual-queue-statistics") && (applicationOptions["virtual-queue-statistics"] != string.Empty || applicationOptions["virtual-queue-statistics"] != ""))
                            {
                                StatisticsSetting.GetInstance().IsExist = true;
                            }
                        }
                        else if (string.Compare("objects", type, true) == 0)
                        {
                            foreach (string sectionName in applicationOptions.Keys)
                            {
                                if ((sectionName.StartsWith("statistics.objects-agents") && !string.IsNullOrEmpty(applicationOptions["statistics.objects-agents"].ToString())) ||
                                    (sectionName.StartsWith("statistics.objects-agent-groups") && !string.IsNullOrEmpty(applicationOptions["statistics.objects-agent-groups"].ToString())) ||
                                    (sectionName.StartsWith("statistics.objects-acd-queues") && !string.IsNullOrEmpty(applicationOptions["statistics.objects-acd-queues"].ToString())) ||
                                    (sectionName.StartsWith("statistics.objects-dn-groups") && !string.IsNullOrEmpty(applicationOptions["statistics.objects-dn-groups"].ToString())) ||
                                    (sectionName.StartsWith("statistics.objects-virtual-queues") && !string.IsNullOrEmpty(applicationOptions["statistics.objects-virtual-queues"].ToString())))
                                {
                                    StatisticsSetting.GetInstance().IsExist = true;
                                }
                            }
                        }
                    }
                    else
                    {
                        StatisticsSetting.GetInstance().IsExist = false;
                    }

                    #endregion

                    #region Check Application Annex

                    if (!StatisticsSetting.GetInstance().IsExist)
                    {
                        KeyValueCollection applicationAnnex = (KeyValueCollection)StatisticsSetting.GetInstance().Application.UserProperties;

                        if (applicationAnnex != null)
                        {
                            if (string.Compare("statistics", type, true) == 0)
                            {
                                logger.Info("StatisticsBase : CheckExistingProperties Method : Checking Application annex for existing configurations");

                                foreach (string sectionName in applicationAnnex.Keys)
                                {
                                    if (sectionName.StartsWith("agent") || sectionName.StartsWith("group") || sectionName.StartsWith("acd") || sectionName.StartsWith("dn") || sectionName.StartsWith("vq"))
                                    {
                                        StatisticsSetting.GetInstance().IsExist = true;
                                    }
                                }
                            }
                        }
                    }

                    #endregion

                    StatisticsSetting.GetInstance().Application.Save();
                }
            }
            catch (Exception generalException)
            {
                logger.Error("StatisticsBase : CheckExistingProperties Method : " + generalException.Message);
            }

            logger.Info("StatisticsBase : CheckExistingProperties Method : Property Exists : " + StatisticsSetting.GetInstance().IsExist);

            return StatisticsSetting.GetInstance().IsExist;

            logger.Debug("StatisticsBase : CheckExistingProperties Method : Exit");
        }

        /// <summary>
        /// Checks the permission.
        /// </summary>
        /// <param name="folderPath">The folder path.</param>
        public void CheckPermission(string folderPath)
        {
            logger.Debug("LoginWindowViewModel : CheckPermission Method : Entry");
            try
            {
                string user = Environment.UserDomainName + "\\" + Environment.UserName;
                FileSecurity accessControlList = File.GetAccessControl(folderPath);
                if (accessControlList != null)
                {
                    Boolean flagWritePermission = true;
                    Boolean flagReadPermission = true;
                    AuthorizationRuleCollection tempaccess = accessControlList.GetAccessRules(true, true, typeof(System.Security.Principal.NTAccount));
                    if (tempaccess != null)
                    {
                        foreach (FileSystemAccessRule rule in tempaccess)
                        {
                            NTAccount ntAccount = rule.IdentityReference as NTAccount;
                            if (ntAccount.Value.Equals(user))
                            {
                                if (rule.AccessControlType == AccessControlType.Deny)
                                {

                                    if ((FileSystemRights.Write & rule.FileSystemRights) == FileSystemRights.Write)
                                    {
                                        flagWritePermission = false;
                                        if (!DictFolderPermissions.ContainsKey(FileSystemRights.Write.ToString()))
                                        {
                                            DictFolderPermissions.Add(FileSystemRights.Write.ToString(), false);
                                        }
                                        else
                                            DictFolderPermissions[FileSystemRights.Write.ToString()] = false;
                                    }
                                    if ((FileSystemRights.Read & rule.FileSystemRights) == FileSystemRights.Read)
                                    {
                                        flagReadPermission = false;
                                        if (!DictFolderPermissions.ContainsKey(FileSystemRights.Read.ToString()))
                                        {
                                            DictFolderPermissions.Add(FileSystemRights.Read.ToString(), false);
                                        }
                                        else
                                            DictFolderPermissions[FileSystemRights.Read.ToString()] = false;
                                    }
                                }
                            }
                        }
                        if (flagWritePermission)
                        {
                            if (!DictFolderPermissions.ContainsKey(FileSystemRights.Write.ToString()))
                            {
                                DictFolderPermissions.Add(FileSystemRights.Write.ToString(), true);
                            }
                            else
                                DictFolderPermissions[FileSystemRights.Write.ToString()] = true;
                        }
                        if (flagReadPermission)
                        {
                            if (!DictFolderPermissions.ContainsKey(FileSystemRights.Read.ToString()))
                            {
                                DictFolderPermissions.Add(FileSystemRights.Read.ToString(), true);
                            }
                            else
                                DictFolderPermissions[FileSystemRights.Read.ToString()] = true;
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("LoginWindowViewModel : CheckPermission Method : " + ex.Message);
            }
            finally
            {
                logger.Debug("LoginWindowViewModel : CheckPermission Method : Exit");
                GC.Collect();
            }
        }

        /// <summary>
        /// Checks the place.
        /// </summary>
        /// <param name="place">The place.</param>
        /// <returns></returns>
        public OutputValues CheckPlace(string place)
        {
            OutputValues output = new OutputValues();

            try
            {
                logger.Debug("StatisticsBase : CheckPlace Method : Entry");
                output.MessageCode = "2000";
                output.Message = StatisticsSetting.GetInstance().statisticsCollection.ApplicationContainer.PlaceNotFound.ToString();

                CfgTenantQuery qTenant = new CfgTenantQuery();
                qTenant.Name = StatisticsSetting.GetInstance().statisticsCollection.StatisticsCommon.TenantName;
                CfgTenant tenant = StatisticsSetting.GetInstance().confObject.RetrieveObject<CfgTenant>(qTenant);

                if (tenant != null)
                {
                    CfgPlaceQuery qPlace = new CfgPlaceQuery();
                    qPlace.TenantDbid = tenant.DBID;
                    ICollection<CfgPlace> places = StatisticsSetting.GetInstance().confObject.RetrieveMultipleObjects<CfgPlace>(qPlace);

                    if (places != null)
                    {
                        foreach (var physicalPlace in places)
                        {
                            if (physicalPlace.Name == place)
                            {
                                output.Message = "True";
                                output.MessageCode = "2001";
                            }
                        }

                        if (output.MessageCode == "2000")
                            logger.Debug("StatisticsBase : CheckPlace Method : Place Not Found");
                    }
                    else
                    {
                        logger.Debug("StatisticsBase : CheckPlace Method : Places Null");
                    }
                }
                else
                {
                    logger.Debug("StatisticsBase : CheckPlace Method : Tenant is Null");
                }
            }
            catch (Exception generalException)
            {
                output.Message = "2000";
                output.Message = generalException.Message.ToString();
                logger.Error("StatisticsBase : CheckPlace Method : " + generalException.Message);
            }

            logger.Debug("StatisticsBase : CheckPlace Method : Exit");
            return output;
        }

        /// <summary>
        /// Checks the user previleges.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <returns></returns>
        public string CheckUserPrevileges(string userName)
        {
            string isAuthenticated;
            try
            {
                logger.Debug("StatisticsBase : CheckUserPrevileges Method : Entry");
                ConfigConnectionManager config = new ConfigConnectionManager();
                isAuthenticated = config.CheckUserPrivilege(userName, StatisticsSetting.GetInstance().statisticsCollection);
            }
            catch (Exception generalException)
            {
                isAuthenticated = "false,false";
                logger.Error("StatisticsBase : CheckUserPrevileges Method : " + generalException.Message);
            }
            finally
            {
                GC.Collect();
                logger.Debug("StatisticsBase : CheckUserPrivileges Method : Exit");
            }
            return isAuthenticated;
        }

        /// <summary>
        /// Clears the tagged stats.
        /// </summary>
        public void ClearTaggedStats()
        {
            try
            {
                if (StatisticsSetting.GetInstance().DictTaggedStats != null)
                {
                    StatisticsSetting.GetInstance().DictTaggedStats.Clear();
                }
            }
            catch (Exception GeneralException)
            {

            }
        }

        public void CloseConfigServer()
        {
            try
            {
                (new ConfigConnectionManager()).CloseConfigServer();
            }
            catch (Exception generalException)
            {
                logger.Error("StatisticsBase : CloseConfigServer() " + generalException.Message.ToString());
                logger.Error("StatisticsBase : CloseConfigServer() : Config-Server not closed.");
            }
        }

        public void CloseStatistics()
        {
            logger.Debug("StatisticsBase : CloseStatistics Method : Entry");
            if (settings.statProtocols != null)
            {
                var reporting = new ReportingServer();
                reporting.CloseStatServer();

                settings.statProtocols = null;

                //foreach (KeyValuePair<string, IStatTicker> messageToClient in ToClient)
                //{
                //    if (messageToClient.Key == "AID")//messageToClient.Key == "StatTickerFive" ||
                //    {
                //        messageToClient.Value.NotifyStatServerStatustoTC(false);

                //        //OutputValues serverError = new OutputValues();

                //        //serverError.Message = StatisticsSetting.GetInstance().statisticsCollection.ApplicationContainer.ServerDown.ToString();

                //        //serverError.MessageCode = "2001";
                //        //messageToClient.Value.NotifyStatErrorMessage(serverError);
                //    }
                //}

            }

            logger.Debug("StatisticsBase : CloseStatistics Method : Exit");
        }

        public void closeTeamCommStat(int refId)
        {
            if (StatisticsSetting.GetInstance().TeamCommRefIDs.ContainsKey(refId))
            {
                var serverName = StatisticsSetting.GetInstance().TeamCommRefIDs[refId];
                RequestCloseStatistic closeStat = RequestCloseStatistic.Create(refId);

                if (string.IsNullOrEmpty(serverName.Trim()))
                {
                    if (StatisticsSetting.GetInstance().statProtocols.Count > 0)
                    {
                        if (StatisticsSetting.GetInstance().statProtocols[0] != null)
                        {
                            if (StatisticsSetting.GetInstance().statProtocols[0].State == ChannelState.Opened || StatisticsSetting.GetInstance().statProtocols[0].State == ChannelState.Opening)
                            {
                                StatisticsSetting.GetInstance().statProtocols[0].Request(closeStat);
                                logger.Info("Request : Close TeamComm Statistics :" + refId);
                            }
                        }
                    }
                }
                else
                {
                    if (StatisticsSetting.GetInstance().rptProtocolManager[serverName] != null)
                    {
                        if (StatisticsSetting.GetInstance().rptProtocolManager[serverName].State == ChannelState.Opened || StatisticsSetting.GetInstance().rptProtocolManager[serverName].State == ChannelState.Opening)
                        {
                            StatisticsSetting.GetInstance().rptProtocolManager[serverName].Request(closeStat);
                            logger.Info("Request : Close TeamComm Statistics :" + refId);
                        }
                    }
                }

                StatisticsSetting.GetInstance().TeamCommRefIDs.Remove(refId);
            }
        }

        /// <summary>
        /// Configs the connection establish.
        /// </summary>
        /// <param name="ConfigServerHost">The config server host.</param>
        /// <param name="ConfigServerPort">The config server port.</param>
        /// <param name="appName">Name of the app.</param>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        public OutputValues ConfigConnectionEstablish(string ConfigServerHost, string ConfigServerPort, string appName, string userName, string password, string logUserName, string source, string statUsername, bool IsServerAuthentication)
        {
            OutputValues output = OutputValues.GetInstance();
            ConfigConnectionManager connect = new ConfigConnectionManager();
            ReadApplication read = new ReadApplication();
            try
            {
                logger.Debug("StatisticsBase : ConfigConnectionEstablish Method : Entry");
                StatisticsBase.GetInstance().ListMissedValues.Clear();
                StatisticsSetting.GetInstance().AppName = appName;
                output = connect.ConnectConfigServer(ConfigServerHost, ConfigServerPort, userName, password, appName, logUserName);

                StatisticsBase.GetInstance().StatSource = source;

                if (output.MessageCode != "2001")
                {
                    //MessageBox.Show(StatisticsSetting.GetInstance().isLogEnabled.ToString());
                    StatisticsBase.GetInstance().isCMEAuthentication = true;

                    if (StatisticsSetting.GetInstance().isLogEnabled)
                    {
                        ConfigureLogger(userName + "_Stat");
                        //MessageBox.Show("StatisticsBase : Username : " + userName + "_Stat");
                    }
                    //+"_Stat"

                    read.ReadApplicationDetails(appName, source);

                    read.ReadBusinessAttributeDetails(StatisticsSetting.GetInstance().statisticsCollection.StatisticsCommon.BusinessAttributeName);

                    if (StatisticsSetting.GetInstance().IsApplicationFound)
                    {
                        if (IsServerAuthentication)
                        {
                            read.ReadAgent(userName, false);
                            StatisticsSetting.GetInstance().statisticsCollection = read.ReadAgentGroupDetails(StatisticsSetting.GetInstance().statisticsCollection, false);
                            StatisticsSetting.GetInstance().statisticsCollection = read.ReadAgentDetails(StatisticsSetting.GetInstance().statisticsCollection, userName);
                        }
                        else
                        {
                            read.ReadAgent(statUsername, false);
                            StatisticsSetting.GetInstance().statisticsCollection = read.ReadAgentGroupDetails(StatisticsSetting.GetInstance().statisticsCollection, false);
                            StatisticsSetting.GetInstance().statisticsCollection = read.ReadAgentDetails(StatisticsSetting.GetInstance().statisticsCollection, statUsername);
                        }
                    }
                    else
                    {
                        output.MessageCode = "2001";
                        output.Message = "No Such Application found.";
                    }

                    StatisticsBase.GetInstance().IsMandatoryFieldsMissing = read.SaveMissedValues();

                    if (StatisticsBase.GetInstance().IsMandatoryFieldsMissing)
                    {
                        output.MessageCode = "2002";
                        output.Message = "Values Missing";
                    }
                }
                else
                {
                    logger.Debug("StatisticsBase : ConfigConnectionEstablish Method : Failed to connect Genesys Server");
                }

                if (output.MessageCode == "2000")
                {
                    DisplayAppValues();
                    output.Message = "Config Server Connection Established";
                }
            }
            catch (Exception generalException)
            {
                logger.Error("StatisticsBase : ConfigConnectionEstablish Method : " + generalException.Message);
            }
            finally
            {
                connect = null;
                GC.Collect();
                logger.Debug("StatisticsBase : ConfigConnectionEstablish Method : Exit");
            }
            return output;
        }

        /// <summary>
        /// Configs the connection establish.
        /// </summary>
        /// <param name="appname">The appname.</param>
        /// <param name="ConfObject">The conf object.</param>
        /// <param name="logUserName">Name of the log user.</param>
        /// <param name="logEnableAIDRequest">if set to <c>true</c> [log enable AID request].</param>
        /// <returns></returns>
        public OutputValues ConfigConnectionEstablish(string appname, ConfService ConfObject, string logUserName, string UserName, string source, List<CfgAgentGroup> agentGroups)
        {
            OutputValues output = OutputValues.GetInstance();
            ConfigConnectionManager connect = new ConfigConnectionManager();
            ReadApplication read = new ReadApplication();
            try
            {
                logger.Debug("StatisticsBase : ConfigConnectionEstablish Method : Entry");
                StatisticsBase.GetInstance().ListMissedValues.Clear();
                output = connect.GetPluginValues(appname, ConfObject);

                if (output.MessageCode != "2001")
                {
                    read.ReadApplicationDetails(appname, source);
                    read.ReadAgent(UserName, false);

                    //Commented for new logger functionality on 20/01/2015 by Elango.T

                    //if (StatisticsSetting.GetInstance().isLogEnabled)
                    //    ConfigureLogger(logUserName);

                    if (StatisticsSetting.GetInstance().AgentGroupCollection.Count != 0)
                        StatisticsSetting.GetInstance().AgentGroupCollection.Clear();
                    StatisticsSetting.GetInstance().AgentGroupCollection = agentGroups;

                    StatisticsSetting.GetInstance().statisticsCollection = read.ReadAgentGroupDetails(StatisticsSetting.GetInstance().statisticsCollection, true);
                    StatisticsSetting.GetInstance().statisticsCollection = read.ReadAgentDetails(StatisticsSetting.GetInstance().statisticsCollection, UserName);

                    StatisticsBase.GetInstance().IsMandatoryFieldsMissing = read.SaveMissedValues();

                    if (StatisticsBase.GetInstance().IsMandatoryFieldsMissing)
                    {
                        output.MessageCode = "2002";
                        output.Message = "Values Missing";
                    }
                }

                if (output.MessageCode == "200")
                {
                    DisplayAppValues();
                    output.Message = "Config Server Connection Established";
                }
            }
            catch (Exception generalException)
            {
                logger.Error("StatisticsBase : ConfigConnectionEstablish Method : " + generalException.Message);
            }
            finally
            {
                connect = null;
                read = null;
                GC.Collect();
                logger.Debug("StatisticsBase : ConfigConnectionEstablish Method : Exit");
            }
            return output;
        }

        /// <summary>
        /// Configures the logger.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        public void ConfigureLogger(string userName)
        {
            try
            {
                logger.Debug("StatisticsBase : ConfigureLogger Method : Entry");

                // MessageBox.Show("StatisticsBase : ConfigureLogger Method : Entry");
                //  MessageBox.Show("StatisticsBase : ConfigureLogger Method :" + StatisticsSetting.GetInstance().logFileName + userName.ToString().Trim());
                Pointel.Logger.Core.Logger.ConfigureLog4net(StatisticsSetting.GetInstance().logmaxSizeRoll, StatisticsSetting.GetInstance().logmaxFileSize,
                    StatisticsSetting.GetInstance().rollingStyle, StatisticsSetting.GetInstance().logconversionPattern,
                    StatisticsSetting.GetInstance().logFileName + userName.ToString().Trim(),
                    StatisticsSetting.GetInstance().logFilterLevel, StatisticsSetting.GetInstance().logLevel,
                    StatisticsSetting.GetInstance().logdatePattern);
                // MessageBox.Show("StatisticsBase : ConfigureLogger Method : Exit");
            }
            catch (Exception generalException)
            {
                // MessageBox.Show("StatisticsBase : ConfigureLogger Method : " + generalException.Message);
                logger.Error("StatisticsBase : ConfigureLogger Method : " + generalException.Message);
            }
            finally
            {
                GC.Collect();
                logger.Debug("StatisticsBase : ConfigureLogger Method : Exit");
            }
        }

        //Commented by Elango.T, to remove DB from AID
        /// <summary>
        /// DBs the login check.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <param name="port">The port.</param>
        /// <param name="loginQuery">The login query.</param>
        /// <param name="dbType">Type of the db.</param>
        /// <param name="dbName">Name of the db.</param>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        /// <param name="sid">The sid.</param>
        /// <param name="sname">The sname.</param>
        /// <param name="dbSource">The db source.</param>
        /// <returns></returns>
        //public OutputValues DBLoginCheck(string host, string port, string loginQuery, string dbType, string dbName, string userName, string password, string sid, string sname, string dbSource, string logUserName, string source, string loginUsername, string loginPassword)
        //{
        //    OutputValues output = new OutputValues();
        //    DataTable dataTable = new DataTable();
        //    ReadApplication read = new ReadApplication();
        //    try
        //    {
        //        logger.Debug("StatisticsBase : DBLoginCheck : Method Entry");
        //        StatisticsBase.GetInstance().ListMissedValues.Clear();
        //        StatisticsBase.GetInstance().StatSource = source;
        //        StatisticsBase.GetInstance().IsMandatoryFieldsMissing = CheckDBSettingsValue(host, port, loginQuery, dbType, dbName, userName, sid, sname, dbSource);
        //        if (!StatisticsBase.GetInstance().IsMandatoryFieldsMissing)
        //        {
        //            if (!StatisticsBase.GetInstance().IsAllType)
        //                StatisticsBase.GetInstance().StatSource = StatisticsEnum.StatSource.DB.ToString();
        //            loginQuery = loginQuery + " UserName='" + loginUsername + "' and Password='" + loginPassword + "'";
        //            StoreDBValues(host, port, loginQuery, dbType, dbName, userName, password, sid, sname, dbSource);
        //            #region DB Connection
        //            if (string.Compare(StatisticsSetting.GetInstance().DBType, StatisticsEnum.DBType.SQLServer.ToString(), true) == 0)
        //            {
        //                #region SQL Server DB Connection
        //                if (StatisticsSetting.GetInstance().sqlConnection.State == ConnectionState.Closed)
        //                {
        //                    StatisticsSetting.GetInstance().sqlConnection.ConnectionString = "Data Source=" + StatisticsSetting.GetInstance().DBDataSource + ";Initial Catalog=" + StatisticsSetting.GetInstance().DBName + ";Persist Security Info=True;User ID=" + StatisticsSetting.GetInstance().DBUserName + ";Password=" + StatisticsSetting.GetInstance().DBPassword;
        //                    StatisticsSetting.GetInstance().sqlConnection.Open();
        //                }
        //                #endregion
        //            }
        //            else if (string.Compare(StatisticsSetting.GetInstance().DBType, StatisticsEnum.DBType.SQLite.ToString(), true) == 0)
        //            {
        //                #region SQLite DB Connection
        //                if (StatisticsSetting.GetInstance().sqliteCon.State == ConnectionState.Closed)
        //                {
        //                    StatisticsSetting.GetInstance().sqliteCon.ConnectionString = "Data Source=" + StatisticsSetting.GetInstance().DBDataSource;
        //                    StatisticsSetting.GetInstance().sqliteCon.Open();
        //                }
        //                #endregion
        //            }
        //            else if (string.Compare(StatisticsSetting.GetInstance().DBType, StatisticsEnum.DBType.ORACLE.ToString(), true) == 0)
        //            {
        //                #region ORACLE DB Connection
        //                if (StatisticsSetting.GetInstance().oracleConn.State == ConnectionState.Closed)
        //                {
        //                    if (StatisticsSetting.GetInstance().DBSID == "" || StatisticsSetting.GetInstance().DBSID == null)
        //                    {
        //                        StatisticsSetting.GetInstance().DBDataSource = "(Description=(Address_list=(Address=(Protocol=TCP)(HOST=" + StatisticsSetting.GetInstance().DBHost + ")(PORT=" + StatisticsSetting.GetInstance().DBPort + ")))(CONNECT_DATA=(SERVICE_NAME=" + StatisticsSetting.GetInstance().DBSName + ")));User Id = " + StatisticsSetting.GetInstance().DBUserName + ";Password=" + StatisticsSetting.GetInstance().DBPassword + ";";
        //                    }
        //                    else
        //                    {
        //                        StatisticsSetting.GetInstance().DBDataSource = "(Description=(Address_list=(Address=(Protocol=TCP)(HOST=" + StatisticsSetting.GetInstance().DBHost + ")(PORT=" + StatisticsSetting.GetInstance().DBPort + ")))(CONNECT_DATA=(SID=" + StatisticsSetting.GetInstance().DBSID + ")));User Id = " + StatisticsSetting.GetInstance().DBUserName + ";Password=" + StatisticsSetting.GetInstance().DBPassword + ";";
        //                    }
        //                    StatisticsSetting.GetInstance().oracleConn.ConnectionString = "Data Source=" + StatisticsSetting.GetInstance().DBDataSource;
        //                    StatisticsSetting.GetInstance().oracleConn.Open();
        //                }
        //                #endregion
        //            }
        //            #endregion
        //            #region DB Login check
        //            if (string.Compare(StatisticsSetting.GetInstance().DBType, StatisticsEnum.DBType.SQLServer.ToString(), true) == 0)
        //            {
        //                if (StatisticsSetting.GetInstance().sqlConnection.State == ConnectionState.Open)
        //                {
        //                    StatisticsSetting.GetInstance().isDBConnectionOpened = true;
        //                    dataTable = new DataTable();
        //                    StatisticsSetting.GetInstance().sqlCommand = new SqlCommand(StatisticsSetting.GetInstance().DBLoginQuery, StatisticsSetting.GetInstance().sqlConnection);
        //                    StatisticsSetting.GetInstance().sqlAdapter = new SqlDataAdapter(StatisticsSetting.GetInstance().sqlCommand);
        //                    StatisticsSetting.GetInstance().sqlAdapter.Fill(dataTable);
        //                }
        //            }
        //            else if (string.Compare(StatisticsSetting.GetInstance().DBType, StatisticsEnum.DBType.SQLite.ToString(), true) == 0)
        //            {
        //                if (StatisticsSetting.GetInstance().sqliteCon.State == ConnectionState.Open)
        //                {
        //                    StatisticsSetting.GetInstance().isDBConnectionOpened = true;
        //                    dataTable = new DataTable();
        //                    StatisticsSetting.GetInstance().sqliteCmd = StatisticsSetting.GetInstance().sqliteCon.CreateCommand();
        //                    StatisticsSetting.GetInstance().sqliteCmd.CommandText = StatisticsSetting.GetInstance().DBLoginQuery;
        //                    StatisticsSetting.GetInstance().sqliteDA = new SQLiteDataAdapter(StatisticsSetting.GetInstance().sqliteCmd);
        //                    StatisticsSetting.GetInstance().sqliteDA.Fill(dataTable);
        //                }
        //            }
        //            else if (string.Compare(StatisticsSetting.GetInstance().DBType, StatisticsEnum.DBType.ORACLE.ToString(), true) == 0)
        //            {
        //                if (StatisticsSetting.GetInstance().oracleConn.State == ConnectionState.Open)
        //                {
        //                    StatisticsSetting.GetInstance().isDBConnectionOpened = true;
        //                    dataTable = new DataTable();
        //                    StatisticsSetting.GetInstance().oracleCmd = StatisticsSetting.GetInstance().oracleConn.CreateCommand();
        //                    StatisticsSetting.GetInstance().oracleCmd.CommandText = StatisticsSetting.GetInstance().DBLoginQuery;
        //                    StatisticsSetting.GetInstance().oracleDA = new OracleDataAdapter(StatisticsSetting.GetInstance().oracleCmd);
        //                    StatisticsSetting.GetInstance().oracleDA.Fill(dataTable);
        //                }
        //            }
        //            read.ReadDbLoggerDetails();
        //            //Added logger creation functionality for the database authendicaiton on 24/10/2014
        //            if (StatisticsBase.GetInstance().StatSource == StatisticsEnum.StatSource.DB.ToString() || (!StatisticsBase.GetInstance().isCMEAuthentication && StatisticsBase.GetInstance().StatSource == StatisticsEnum.StatSource.All.ToString()))
        //            {
        //                if (StatisticsSetting.GetInstance().DictEnableDisableChannels["log"])
        //                    ConfigureLogger(userName);
        //                logger.Debug("---------------------------------------------------------------------------------------");
        //                logger.Debug("StatisticsBase : DisplayDBAppValues : DB Application Configuration Values : DbSetting");
        //                logger.Debug("---------------------------------------------------------------------------------------");
        //                logger.Debug("StatisticsBase : DisplayDBAppValues : db-Type : " + dbType.ToString());
        //                logger.Debug("StatisticsBase : DisplayDBAppValues : db-Host : " + host.ToString());
        //                logger.Debug("StatisticsBase : DisplayDBAppValues : db-Port : " + port.ToString());
        //                logger.Debug("StatisticsBase : DisplayDBAppValues : db-Name : " + dbName.ToString());
        //                logger.Debug("StatisticsBase : DisplayDBAppValues : db-Username : " + userName.ToString());
        //                logger.Debug("StatisticsBase : DisplayDBAppValues : db-Password : " + password.ToString());
        //                logger.Debug("StatisticsBase : DisplayDBAppValues : db-Sid : " + sid.ToString());
        //                logger.Debug("StatisticsBase : DisplayDBAppValues : db-Sname : " + sname.ToString());
        //                logger.Debug("StatisticsBase : DisplayDBAppValues : db-Source : " + dbSource.ToString());
        //                logger.Debug("StatisticsBase : DisplayDBAppValues : db-loginquery : " + loginQuery.ToString());
        //            }
        //            if (dataTable.Rows[0][0].ToString() == "1")
        //            {
        //                output.MessageCode = "2000";
        //                output.Message = "Authenticated";
        //                StatisticsBase.GetInstance().isDBAuthentication = true;
        //                read.ReadDBDetails();
        //                if (!StatisticsBase.GetInstance().isCMEAuthentication)
        //                {
        //                    DisplayDBAppValues();
        //                }
        //            }
        //            else
        //            {
        //                StatisticsBase.GetInstance().isDBAuthentication = false;
        //                read.ReadDBDetails();
        //                output.MessageCode = "2001";
        //                output.Message = StatisticsSetting.GetInstance().DictErrorValues["db.authentication"];
        //                logger.Error("StatisticsBase : StartStatistics : " + StatisticsSetting.GetInstance().DictErrorValues["db.authentication"]);
        //            }
        //            if (StatisticsSetting.GetInstance().statisticsCollection.DBValues == null)
        //            {
        //                StatisticsBase.GetInstance().isDBAuthentication = false;
        //                read.ReadDBDetails();
        //            }
        //            StatisticsBase.GetInstance().IsMandatoryFieldsMissing = read.SaveMissedValues();
        //            if (StatisticsBase.GetInstance().IsMandatoryFieldsMissing)
        //            {
        //                output.MessageCode = "2002";
        //                output.Message = "Values Missing";
        //            }
        //            #endregion
        //        }
        //        else
        //        {
        //            output.MessageCode = "2002";
        //            output.Message = "Values Missing";
        //        }
        //    }
        //    catch (Exception GeneralException)
        //    {
        //        logger.Error("StatisticsBase : StartStatistics : " + GeneralException.Message);
        //        output.MessageCode = "2001";
        //        output.Message = GeneralException.InnerException == null ? GeneralException.Message : GeneralException.InnerException.Message;
        //    }
        //    finally
        //    {
        //        logger.Debug("StatisticsBase : DBLoginCheck : Method Exit");
        //    }
        //    return output;
        //}
        public bool DisplayAgents()
        {
            return StatisticsSetting.GetInstance().statisticsCollection.AdminValues.AutoAgents;
        }

        /// <summary>
        /// Displays the app values.
        /// </summary>
        public void DisplayAppValues()
        {
            try
            {
                logger.Debug("-------------------------------------------------------------------------------------------------");
                logger.Debug("StatisticsBase : DisplayAppValues : Application Configuration Values : agent.ixn.desktop");
                logger.Debug("-------------------------------------------------------------------------------------------------");
                logger.Debug("StatisticsBase : DisplayAppValues : Primary Server Name : " + StatisticsSetting.GetInstance().statisticsCollection.StatisticsCommon.PrimaryServerName);
                logger.Debug("StatisticsBase : DisplayAppValues : Secondary Server Name : " + StatisticsSetting.GetInstance().statisticsCollection.StatisticsCommon.SecondaryServerName);
                logger.Debug("StatisticsBase : DisplayAppValues : Insensitivity : " + StatisticsSetting.GetInstance().statisticsCollection.StatisticsCommon.Insensitivity);
                logger.Debug("StatisticsBase : DisplayAppValues : Notify Seconds : " + StatisticsSetting.GetInstance().statisticsCollection.StatisticsCommon.NotifySeconds);
                logger.Debug("StatisticsBase : DisplayAppValues : Tenant Name : " + StatisticsSetting.GetInstance().statisticsCollection.StatisticsCommon.TenantName);
                logger.Debug("StatisticsBase : DisplayAppValues : Display Time : " + StatisticsSetting.GetInstance().statisticsCollection.StatisticsCommon.DisplayTime);
                logger.Debug("StatisticsBase : DisplayAppValues : Agent Statistics : " + StatisticsSetting.GetInstance().statisticsCollection.StatisticsCommon.AgentStatistics);
                logger.Debug("StatisticsBase : DisplayAppValues : Agentgroup Statistics : " + StatisticsSetting.GetInstance().statisticsCollection.StatisticsCommon.AgentGroupStatistics);
                logger.Debug("StatisticsBase : DisplayAppValues : ACD Queue Statistics : " + StatisticsSetting.GetInstance().statisticsCollection.StatisticsCommon.ACDStatistics);
                logger.Debug("StatisticsBase : DisplayAppValues : DNGroup Statistics : " + StatisticsSetting.GetInstance().statisticsCollection.StatisticsCommon.DNGroupStatistics);
                logger.Debug("StatisticsBase : DisplayAppValues : VQueue Statistics : " + StatisticsSetting.GetInstance().statisticsCollection.StatisticsCommon.VQueueStatistics);
                logger.Debug("StatisticsBase : DisplayAppValues : ACD Queue Objects : " + StatisticsSetting.GetInstance().statisticsCollection.StatisticsCommon.ACDObjects);
                logger.Debug("StatisticsBase : DisplayAppValues : DNGroup Objects : " + StatisticsSetting.GetInstance().statisticsCollection.StatisticsCommon.DNGroupObjects);
                logger.Debug("StatisticsBase : DisplayAppValues : VQueue Objects : " + StatisticsSetting.GetInstance().statisticsCollection.StatisticsCommon.VQueueObjects);

                logger.Debug("---------------------------------------------------------------------------------------");
                logger.Debug("StatisticsBase : DisplayAppValues : Application Configuration Values : _errors_");
                logger.Debug("---------------------------------------------------------------------------------------");
                logger.Debug("StatisticsBase : DisplayAppValues : config.connection : " + StatisticsSetting.GetInstance().statisticsCollection.ApplicationContainer.ConfigConnection);
                logger.Debug("StatisticsBase : DisplayAppValues : place.notfound : " + StatisticsSetting.GetInstance().statisticsCollection.ApplicationContainer.PlaceNotFound);
                logger.Debug("StatisticsBase : DisplayAppValues : pristat.server : " + StatisticsSetting.GetInstance().statisticsCollection.ApplicationContainer.PrimaryServer);
                logger.Debug("StatisticsBase : DisplayAppValues : secstat.server : " + StatisticsSetting.GetInstance().statisticsCollection.ApplicationContainer.SecondaryServer);
                logger.Debug("StatisticsBase : DisplayAppValues : server.down : " + StatisticsSetting.GetInstance().statisticsCollection.ApplicationContainer.ServerDown);
                logger.Debug("StatisticsBase : DisplayAppValues : user.autorization : " + StatisticsSetting.GetInstance().statisticsCollection.ApplicationContainer.UserAuthorization);
                logger.Debug("StatisticsBase : DisplayAppValues : user.permission : " + StatisticsSetting.GetInstance().statisticsCollection.ApplicationContainer.UserPermission);
                logger.Debug("StatisticsBase : DisplayAppValues : nostat.configured : " + StatisticsSetting.GetInstance().statisticsCollection.ApplicationContainer.NoStats);

                logger.Debug("---------------------------------------------------------------------------------------");
                logger.Debug("StatisticsBase : DisplayAppValues : Application Configuration Values : _system_");
                logger.Debug("---------------------------------------------------------------------------------------");
                logger.Debug("StatisticsBase : DisplayAppValues : admin.access-group : " + StatisticsSetting.GetInstance().statisticsCollection.ApplicationContainer.AdminGroupName);
                logger.Debug("StatisticsBase : DisplayAppValues : statistics.display-time : " + StatisticsSetting.GetInstance().statisticsCollection.ApplicationContainer.DisplayTime);
                logger.Debug("StatisticsBase : DisplayAppValues : user.access-group : " + StatisticsSetting.GetInstance().statisticsCollection.ApplicationContainer.UserGroupName);

                logger.Debug("------------------------------------------------------------------------------------------------------");
                logger.Debug("StatisticsBase : DisplayAppValues : Application Configuration Values : enable-disable-channels");
                logger.Debug("------------------------------------------------------------------------------------------------------");
                logger.Debug("StatisticsBase : DisplayAppValues : statistics.enable-alwaysontop : " + StatisticsSetting.GetInstance().statisticsCollection.ApplicationContainer.EnableAlwaysOnTop);
                logger.Debug("StatisticsBase : DisplayAppValues : statistics.enable-hhmmss-format : " + StatisticsSetting.GetInstance().statisticsCollection.ApplicationContainer.EnableHHMMSS);
                logger.Debug("StatisticsBase : DisplayAppValues : statistics.enable-ccstat-aid : " + StatisticsSetting.GetInstance().statisticsCollection.ApplicationContainer.EnableCCStatAID);
                logger.Debug("StatisticsBase : DisplayAppValues : statistics.enable-log : " + StatisticsSetting.GetInstance().statisticsCollection.ApplicationContainer.EnableLog);
                logger.Debug("StatisticsBase : DisplayAppValues : statistics.enable-maingadget : " + StatisticsSetting.GetInstance().statisticsCollection.ApplicationContainer.EnableMainGadget);
                logger.Debug("StatisticsBase : DisplayAppValues : statistics.enable-menu-button : " + StatisticsSetting.GetInstance().statisticsCollection.ApplicationContainer.EnableMenuButton);
                logger.Debug("StatisticsBase : DisplayAppValues : statistics.enable-mystat-aid : " + StatisticsSetting.GetInstance().statisticsCollection.ApplicationContainer.EnableMyStatAID);
                logger.Debug("StatisticsBase : DisplayAppValues : statistics.enable-notification-close : " + StatisticsSetting.GetInstance().statisticsCollection.ApplicationContainer.EnableTaskbarClose);
                logger.Debug("StatisticsBase : DisplayAppValues : statistics.enable-notification-balloon : " + StatisticsSetting.GetInstance().statisticsCollection.ApplicationContainer.EnableNotificationBalloon);
                logger.Debug("StatisticsBase : DisplayAppValues : statistics.enable-submenu-ccstatistics : " + StatisticsSetting.GetInstance().statisticsCollection.ApplicationContainer.EnableMenuShowCCStat);
                logger.Debug("StatisticsBase : DisplayAppValues : statistics.enable-submenu-close-gadget : " + StatisticsSetting.GetInstance().statisticsCollection.ApplicationContainer.EnableMenuClose);
                logger.Debug("StatisticsBase : DisplayAppValues : statistics.enable-submenu-mystatistics : " + StatisticsSetting.GetInstance().statisticsCollection.ApplicationContainer.EnableMenuShowMyStat);
                logger.Debug("StatisticsBase : DisplayAppValues : statistics.enable-submenu-ontop : " + StatisticsSetting.GetInstance().statisticsCollection.ApplicationContainer.EnableMenuOnTop);
                logger.Debug("StatisticsBase : DisplayAppValues : statistics.enable-system-draggable : " + StatisticsSetting.GetInstance().statisticsCollection.ApplicationContainer.EnableSystemDraggable);
                logger.Debug("StatisticsBase : DisplayAppValues : statistics.enable-tag-button : " + StatisticsSetting.GetInstance().statisticsCollection.ApplicationContainer.EnableTagButton);
                logger.Debug("StatisticsBase : DisplayAppValues : statistics.enable-tag-vertical : " + StatisticsSetting.GetInstance().statisticsCollection.ApplicationContainer.EnableTagVertical);
                logger.Debug("StatisticsBase : DisplayAppValues : statistics.enable-untag-button : " + StatisticsSetting.GetInstance().statisticsCollection.ApplicationContainer.EnableUntagButton);
                logger.Debug("StatisticsBase : DisplayAppValues : statistics.enable.submenu.alert-notification: " + StatisticsSetting.GetInstance().statisticsCollection.ApplicationContainer.EnableThresholdNotification);

                logger.Debug("-----------------------------------------------------------------------------------------------");
                logger.Debug("StatisticsBase : DisplayAppValues : Application Configuration Values : _statistics.log_");
                logger.Debug("-----------------------------------------------------------------------------------------------");
                logger.Debug("StatisticsBase : DisplayAppValues : statistics.log-conversion-pattern : " + StatisticsSetting.GetInstance().logconversionPattern);
                logger.Debug("StatisticsBase : DisplayAppValues : statistics.log-date-pattern : " + StatisticsSetting.GetInstance().logdatePattern);
                logger.Debug("StatisticsBase : DisplayAppValues : statistics.log-level : " + StatisticsSetting.GetInstance().logLevel);
                logger.Debug("StatisticsBase : DisplayAppValues : statistics.log-file-name : " + StatisticsSetting.GetInstance().logFileName);
                logger.Debug("StatisticsBase : DisplayAppValues : statistics.log.filter-level : " + StatisticsSetting.GetInstance().logFilterLevel);
                logger.Debug("StatisticsBase : DisplayAppValues : statistics.log-maxfile-size : " + StatisticsSetting.GetInstance().logmaxFileSize);
                logger.Debug("StatisticsBase : DisplayAppValues : statistics.log-maxrollback-size : " + StatisticsSetting.GetInstance().logmaxSizeRoll);
                logger.Debug("StatisticsBase : DisplayAppValues : statistics.log-min-level : " + StatisticsSetting.GetInstance().logMinLevel);
                logger.Debug("StatisticsBase : DisplayAppValues : statistics.log-max-level : " + StatisticsSetting.GetInstance().logMaxLevel);
                logger.Debug("StatisticsBase : DisplayAppValues : statistics.log.roll-style : " + StatisticsSetting.GetInstance().rollingStyle);
                logger.Debug("-----------------------------------------------------------------------------------------------");
            }
            catch (Exception GeneralException)
            {
                logger.Error("StatisticsBase : DisplayAppValues : Exception caught : " + GeneralException.Message);
            }
        }

        /// <summary>
        /// Displays the database application values.
        /// </summary>
        public void DisplayDBAppValues()
        {
            try
            {
                logger.Debug("---------------------------------------------------------------------------------------");
                logger.Debug("StatisticsBase : DisplayDBAppValues : DB Application Configuration Values : _errors_");
                logger.Debug("---------------------------------------------------------------------------------------");

                if (StatisticsSetting.GetInstance().DictErrorValues.ContainsKey("db.authentication"))
                {
                    logger.Debug("StatisticsBase : DisplayDBAppValues : db.authentication : " + StatisticsSetting.GetInstance().DictErrorValues["db.authentication"].ToString());
                }

                if (StatisticsSetting.GetInstance().DictErrorValues.ContainsKey("db.connection"))
                {
                    logger.Debug("StatisticsBase : DisplayDBAppValues : db.connection : " + StatisticsSetting.GetInstance().DictErrorValues["db.connection"].ToString());
                }

                logger.Debug("------------------------------------------------------------------------------------------------------");
                logger.Debug("StatisticsBase : DisplayDBAppValues : Application Configuration Values : enable.disable-channels");
                logger.Debug("------------------------------------------------------------------------------------------------------");
                logger.Debug("StatisticsBase : DisplayDBAppValues : statistics.enable-alwaysontop : " + StatisticsSetting.GetInstance().statisticsCollection.ApplicationContainer.EnableAlwaysOnTop);
                logger.Debug("StatisticsBase : DisplayDBAppValues : statistics.enable-hhmmssformat : " + StatisticsSetting.GetInstance().statisticsCollection.ApplicationContainer.EnableHHMMSS);
                logger.Debug("StatisticsBase : DisplayDBAppValues : statistics.enable-log : " + StatisticsSetting.GetInstance().statisticsCollection.ApplicationContainer.EnableLog);
                logger.Debug("StatisticsBase : DisplayDBAppValues : statistics.enable-maingadget : " + StatisticsSetting.GetInstance().statisticsCollection.ApplicationContainer.EnableMainGadget);
                logger.Debug("StatisticsBase : DisplayDBAppValues : statistics.enable-menu-button : " + StatisticsSetting.GetInstance().statisticsCollection.ApplicationContainer.EnableMenuButton);
                logger.Debug("StatisticsBase : DisplayDBAppValues : statistics.enable-notification-close : " + StatisticsSetting.GetInstance().statisticsCollection.ApplicationContainer.EnableTaskbarClose);
                logger.Debug("StatisticsBase : DisplayDBAppValues : statistics.enable-notification-balloon : " + StatisticsSetting.GetInstance().statisticsCollection.ApplicationContainer.EnableNotificationBalloon);
                logger.Debug("StatisticsBase : DisplayDBAppValues : statistics.enable-submenu-close-gadget : " + StatisticsSetting.GetInstance().statisticsCollection.ApplicationContainer.EnableMenuClose);
                logger.Debug("StatisticsBase : DisplayDBAppValues : statistics.enable-submenu-ontop : " + StatisticsSetting.GetInstance().statisticsCollection.ApplicationContainer.EnableMenuOnTop);
                logger.Debug("StatisticsBase : DisplayDBAppValues : statistics.enable-systemdraggable : " + StatisticsSetting.GetInstance().statisticsCollection.ApplicationContainer.EnableSystemDraggable);
                logger.Debug("StatisticsBase : DisplayDBAppValues : statistics.enable-tag-button : " + StatisticsSetting.GetInstance().statisticsCollection.ApplicationContainer.EnableTagButton);
                logger.Debug("StatisticsBase : DisplayDBAppValues : statistics.enable-tag-vertical : " + StatisticsSetting.GetInstance().statisticsCollection.ApplicationContainer.EnableTagVertical);
                logger.Debug("StatisticsBase : DisplayDBAppValues : statistics.enable-untag-button : " + StatisticsSetting.GetInstance().statisticsCollection.ApplicationContainer.EnableUntagButton);
                logger.Debug("StatisticsBase : DisplayDBAppValues : statistics.enable.submenu-alertnotification: " + StatisticsSetting.GetInstance().statisticsCollection.ApplicationContainer.EnableThresholdNotification);

                logger.Debug("-----------------------------------------------------------------------------------------------");
                logger.Debug("StatisticsBase : DisplayDBAppValues : Application Configuration Values : statistics.log");
                logger.Debug("-----------------------------------------------------------------------------------------------");
                logger.Debug("StatisticsBase : DisplayDBAppValues : statistics.log-conversion-pattern : " + StatisticsSetting.GetInstance().logconversionPattern);
                logger.Debug("StatisticsBase : DisplayDBAppValues : statistics.log-date-pattern : " + StatisticsSetting.GetInstance().logdatePattern);
                logger.Debug("StatisticsBase : DisplayDBAppValues : statistics.log-level : " + StatisticsSetting.GetInstance().logLevel);
                logger.Debug("StatisticsBase : DisplayDBAppValues : statistics.log-file-name : " + StatisticsSetting.GetInstance().logFileName);
                logger.Debug("StatisticsBase : DisplayDBAppValues : statistics.log.filter-level : " + StatisticsSetting.GetInstance().logFilterLevel);
                logger.Debug("StatisticsBase : DisplayDBAppValues : statistics.log-maxfile-size : " + StatisticsSetting.GetInstance().logmaxFileSize);
                logger.Debug("StatisticsBase : DisplayDBAppValues : statistics.log-maxrollback-size : " + StatisticsSetting.GetInstance().logmaxSizeRoll);
                logger.Debug("StatisticsBase : DisplayDBAppValues : statistics.log-min-level : " + StatisticsSetting.GetInstance().logMinLevel);
                logger.Debug("StatisticsBase : DisplayDBAppValues : statistics.log-max-level : " + StatisticsSetting.GetInstance().logMaxLevel);
                logger.Debug("StatisticsBase : DisplayDBAppValues : statistics.log.roll-style : " + StatisticsSetting.GetInstance().rollingStyle);
                logger.Debug("-----------------------------------------------------------------------------------------------");
            }
            catch (Exception GeneralException)
            {
                logger.Error("DisplayDBAppValues : Exception Caught : " + GeneralException.Message);
            }
        }

        /// <summary>
        /// Displays the database error.
        /// </summary>
        /// <param name="ErrorMsg">The error MSG.</param>
        public void DisplayDBError(string ErrorMsg)
        {
            try
            {
                logger.Debug("StatisticsBase : DisplayDBError : Method Entry");

                foreach (KeyValuePair<string, IStatTicker> messageToClient in ToClient)
                {
                    OutputValues serverError = new OutputValues();

                    serverError.Message = ErrorMsg;

                    serverError.MessageCode = "2003";
                    messageToClient.Value.NotifyStatErrorMessage(serverError);
                }

            }
            catch (Exception GeneralException)
            {
                logger.Error("StatisticsBase : DisplayDBError : " + GeneralException.Message);
            }
            finally
            {
                logger.Debug("StatisticsBase : DisplayDBError :  Method Exit");
            }
        }

        public Dictionary<string, Dictionary<string, List<string>>> GetAgentGroupObjects()
        {
            return StatisticsSetting.GetInstance().DictAgentGroupObjects;
        }

        /// <summary>
        /// Gets the agent group values.
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, List<string>> GetAgentGroupValues()
        {
            statisticsInformation.ReadAllAgentGroupValues();
            return StatisticsSetting.GetInstance().DictAgentGroupStatistics;
        }

        public string GetAgentId()
        {
            return StatisticsSetting.GetInstance().PersonDetails.EmployeeID;
        }

        public Dictionary<string, Dictionary<string, List<string>>> GetAgentObjects()
        {
            return StatisticsSetting.GetInstance().DictAgentObjects;
        }

        /// <summary>
        /// Gets the agents names.
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> GetAgentsNames()
        {
            return StatisticsSetting.GetInstance().DictAgentNames;
        }

        /// <summary>
        /// Gets the agent values.
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, List<string>> GetAgentValues()
        {
            statisticsInformation.ReadAllAgentValues();
            return StatisticsSetting.GetInstance().DictAgentStatistics;
        }

        /// <summary>
        /// Gets all stats.
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> GetAllStats()
        {
            return settings.DictAllStats;
        }

        /// <summary>
        /// Gets the application annex.
        /// </summary>
        /// <param name="applicationName">Name of the application.</param>
        /// <returns></returns>
        public Dictionary<string, Dictionary<string, string>> GetApplicationAnnex(string applicationName)
        {
            logger.Debug("StatisticsBase : GetApplicationAnnex : Method Entry");
            try
            {
                StatisticsSetting.GetInstance().Application = new CfgApplication(StatisticsSetting.GetInstance().confObject);
                CfgApplicationQuery queryApp = new CfgApplicationQuery();
                queryApp.Name = applicationName;
                StatisticsSetting.GetInstance().Application = StatisticsSetting.GetInstance().confObject.RetrieveObject<CfgApplication>(queryApp);

                if (StatisticsSetting.GetInstance().Application != null)
                {
                    statisticsInformation.ReadApplicationAnnex();
                }
            }
            catch (Exception GeneralException)
            {
                logger.Error("StatisticsBase : GetApplicationAnnex : " + GeneralException.Message);
            }

            return StatisticsSetting.GetInstance().StatistcisDetails;

            logger.Debug("StatisticsBase : GetApplicationAnnex : Method Exit");
        }

        /// <summary>
        /// Gets the app position.
        /// </summary>
        /// <returns></returns>
        public string GetAppPosition()
        {
            string position = string.Empty;
            logger.Debug("StatisticsBase : GetAppPosition Method : Entry");
            try
            {
                if ((StatisticsSetting.GetInstance().statisticsCollection.StatisticsCommon.Source == StatisticsEnum.StatSource.StatServer.ToString()) || (StatisticsSetting.GetInstance().statisticsCollection.StatisticsCommon.Source == StatisticsEnum.StatSource.All.ToString() && StatisticsBase.GetInstance().isCMEAuthentication))
                {
                    if (StatisticsSetting.GetInstance().statisticsCollection.StatisticsCommon.Position == null || StatisticsSetting.GetInstance().statisticsCollection.StatisticsCommon.Position.ToString() == "" || StatisticsSetting.GetInstance().statisticsCollection.StatisticsCommon.Position.ToString() == string.Empty)
                        position = string.Empty;
                    else
                        position = StatisticsSetting.GetInstance().statisticsCollection.StatisticsCommon.Position.ToString();
                }
                else
                {
                    return position;
                }
            }
            catch (Exception generalException)
            {
                //throw;
                logger.Error("StatisticsBase : GetAppPosition Method : " + generalException.Message.ToString());
            }
            finally
            {
                logger.Debug("StatisticsBase : GetAppPosition Method : Exit");
            }
            return position;
        }

        /// <summary>
        /// Gets the color of the database.
        /// </summary>
        /// <returns></returns>
        public Color GetDBColor()
        {
            return StatisticsSetting.GetInstance().statisticsCollection.StatisticsCommon.DBColor;
        }

        /// <summary>
        /// Gets the display time.
        /// </summary>
        /// <returns></returns>
        public int GetDisplayTime()
        {
            logger.Info("StatisticsBase : GetDisplayTime : Method");
            return Convert.ToInt32(StatisticsSetting.GetInstance().statisticsCollection.ApplicationContainer.DisplayTime);
        }

        /// <summary>
        /// Gets the enable disable channels.
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, bool> GetEnableDisableChannels()
        {
            return StatisticsSetting.GetInstance().DictEnableDisableChannels;
        }

        /// <summary>
        /// Gets the error values.
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> GetErrorValues()
        {
            return StatisticsSetting.GetInstance().DictErrorValues;
        }

        public string GetExistingStatColor()
        {
            return StatisticsSetting.GetInstance().statisticsCollection.AdminValues.ExistingColor.Name.ToString();
        }

        public Dictionary<string, List<string>> GetExistingValues()
        {
            try
            {
                logger.Debug("StatisticsBase : GetExistingValues Method : Entry");

                statisticsInformation.ReadExistingValues();
            }
            catch (Exception GeneralException)
            {
                logger.Error("StatisticsBase : GetExistingValues Method : " + GeneralException.Message);
            }
            logger.Debug("StatisticsBase : GetExistingValues Method : Exit");

            return StatisticsSetting.GetInstance().DictExistingValues;
        }

        public string GetLogoutColor()
        {
            return StatisticsSetting.GetInstance().statisticsCollection.AdminValues.LogoutColor.Name.ToString();
        }

        public int GetMaxObjectCount()
        {
            return StatisticsSetting.GetInstance().statisticsCollection.StatisticsCommon.MaxObject;
        }

        public int GetMaxTabs()
        {
            return StatisticsSetting.GetInstance().statisticsCollection.AdminValues.MaxTabs;
        }

        /// <summary>
        /// Gets the no stat error message.
        /// </summary>
        /// <returns></returns>
        public string GetNoStatErrorMessage()
        {
            return StatisticsSetting.GetInstance().statisticsCollection.ApplicationContainer.NoStats;
        }

        public string GetNReadyColor()
        {
            return StatisticsSetting.GetInstance().statisticsCollection.AdminValues.NRColor.Name.ToString();
        }

        public List<string> GetObjectTypes()
        {
            List<string> lstObjectTypes = new List<string>();
            try
            {
                if (!string.IsNullOrEmpty(StatisticsSetting.GetInstance().statisticsCollection.StatisticsCommon.QueueObjectType))
                {
                    //Commented for GUD PHS, by Elango
                    //if (StatisticsSetting.GetInstance().statisticsCollection.StatisticsCommon.QueueObjectType.Contains(StatisticsEnum.ObjectType.ACDQueue.ToString()))
                    //    lstObjectTypes.Add(StatisticsEnum.ObjectType.ACDQueue.ToString());

                    if (StatisticsSetting.GetInstance().statisticsCollection.StatisticsCommon.QueueObjectType.Contains(StatisticsEnum.ObjectType.DNGroup.ToString()))
                        lstObjectTypes.Add(StatisticsEnum.ObjectType.DNGroup.ToString());

                    if (StatisticsSetting.GetInstance().statisticsCollection.StatisticsCommon.QueueObjectType.Contains(StatisticsEnum.ObjectType.VirtualQueue.ToString()))
                        lstObjectTypes.Add(StatisticsEnum.ObjectType.VirtualQueue.ToString());
                }
                else
                {
                    lstObjectTypes.Add(StatisticsEnum.ObjectType.ACDQueue.ToString());
                    lstObjectTypes.Add(StatisticsEnum.ObjectType.DNGroup.ToString());
                    lstObjectTypes.Add(StatisticsEnum.ObjectType.VirtualQueue.ToString());
                }
            }
            catch (Exception generalException)
            {

            }
            return lstObjectTypes;
        }

        public string GetReadyColor()
        {
            return StatisticsSetting.GetInstance().statisticsCollection.AdminValues.ReadyColor.Name.ToString();
        }

        /// <summary>
        /// Gets the request.
        /// </summary>
        /// <param name="TenantName">Name of the tenant.</param>
        /// <param name="ObjectID">The object unique identifier.</param>
        /// <param name="statObjectType">Type of the stat object.</param>
        /// <param name="statTypeName">Name of the stat type.</param>
        /// <param name="Seconds">The seconds.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="insensitivity">The insensitivity.</param>
        /// <param name="refID">The preference unique identifier.</param>
        public void GetRequest(string TenantName, string ObjectID, StatisticObjectType statObjectType, string statTypeName, int Seconds, string filter, int insensitivity, int refID, bool isSecondLimit)
        {
            try
            {
                logger.Debug("StatisticsBase : GetRequest Method : Entry");

                logger.Info("StatisticsBase : GetRequest Method : Tenant Name : " + TenantName.ToString());
                logger.Info("StatisticsBase : GetRequest Method : Object Id : " + ObjectID.ToString());
                logger.Info("StatisticsBase : GetRequest Method : Object Type : " + statObjectType.ToString());
                logger.Info("StatisticsBase : GetRequest Method : Stat Type Name : " + statTypeName.ToString());
                logger.Info("StatisticsBase : GetRequest Method : Seconds : " + Seconds.ToString());
                logger.Info("StatisticsBase : GetRequest Method : Filter : " + filter.ToString());
                logger.Info("StatisticsBase : GetRequest Method : Insensitivity : " + insensitivity.ToString());
                logger.Info("StatisticsBase : GetRequest Method : Ref Id : " + refID.ToString());

                if (isSecondLimit && StatisticsSetting.GetInstance().SecondLimit != 0)
                {
                    StatisticsSetting.GetInstance().SecondLimit = refID;
                }

                IMessage response = statRequest.StatRequest(TenantName, ObjectID, statObjectType, statTypeName, Seconds, filter, insensitivity, refID, "");

                logger.Info("StatisticsBase : GetRequest Method : Response : " + response.ToString());

                if (response != null)
                {
                    switch (response.Id)
                    {
                        case EventStatisticOpened.MessageId:
                            EventStatisticOpened info;
                            info = (EventStatisticOpened)response;
                            logger.Trace("StatisticsBase : GetRequest Method : EventStatisticsOpened : " + info.Name);
                            break;
                        case EventError.MessageId:
                            EventError eventError = (EventError)response;
                            logger.Trace("StatisticsBase : GetRequest Method : EventError : " + eventError.StringValue);
                            break;
                    }
                }
            }
            catch (Exception GeneralException)
            {
                logger.Error("StatisticsBase : GetRequest Method : Exception Caught : " + GeneralException.Message);
            }
            logger.Debug("StatisticsBase : GetRequest Method : Exit");
        }

        /// <summary>
        /// Gets the request.
        /// </summary>
        /// <param name="TenantName">Name of the tenant.</param>
        /// <param name="ObjectID">The object unique identifier.</param>
        /// <param name="statObjectType">Type of the stat object.</param>
        /// <param name="statTypeName">Name of the stat type.</param>
        /// <param name="Seconds">The seconds.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="insensitivity">The insensitivity.</param>
        /// <param name="refID">The preference unique identifier.</param>
        public void GetRequest(string TenantName, string[] ObjectIDs, StatisticObjectType statObjectType, string sectionName, int Seconds, int insensitivity, bool isSecondLimit)
        {
            try
            {
                logger.Debug("StatisticsBase : GetRequest Method : Entry");

                int refID = StatisticsSetting.GetInstance().ReferenceIdLimit;

                foreach (string ObjectID in ObjectIDs)
                {
                    logger.Info("StatisticsBase : GetRequest Method : Tenant Name : " + TenantName.ToString());
                    logger.Info("StatisticsBase : GetRequest Method : Object Id : " + ObjectID.ToString());
                    logger.Info("StatisticsBase : GetRequest Method : Object Type : " + statObjectType.ToString());
                    logger.Info("StatisticsBase : GetRequest Method : Stat Type Name : " + sectionName.ToString());
                    logger.Info("StatisticsBase : GetRequest Method : Seconds : " + Seconds.ToString());
                    logger.Info("StatisticsBase : GetRequest Method : Insensitivity : " + insensitivity.ToString());
                    logger.Info("StatisticsBase : GetRequest Method : Ref Id : " + refID.ToString());

                    if (isSecondLimit && StatisticsSetting.GetInstance().SecondLimit != 0)
                    {
                        StatisticsSetting.GetInstance().SecondLimit = refID;
                    }

                    #region RequestOpenStatisticsEx creation
                    var requestStat = RequestOpenStatisticEx.Create();

                    requestStat.StatisticObject = StatisticObject.Create();
                    requestStat.StatisticObject.ObjectId = ObjectID;
                    requestStat.StatisticObject.ObjectType = statObjectType;
                    requestStat.StatisticObject.TenantName = TenantName;

                    requestStat.Notification = Notification.Create();
                    requestStat.Notification.Mode = NotificationMode.Immediate;
                    requestStat.Notification.Frequency = Seconds;

                    DnActionMask mainMask = null;
                    DnActionMask relMask = null;

                    requestStat.StatisticMetricEx = StatisticMetricEx.Create();

                    #region ReadBusinessAttributeValues

                    Dictionary<string, string> statMetricDetail = new Dictionary<string, string>();

                    CfgEnumeratorQuery businessAttributeQuery = new CfgEnumeratorQuery();
                    businessAttributeQuery.Name = StatisticsSetting.GetInstance().statisticsCollection.StatisticsCommon.BusinessAttributeName;
                    businessAttributeQuery.TenantDbid = StatisticsSetting.GetInstance().CFGTenantDBID;

                    CfgEnumerator businessAttribute = StatisticsSetting.GetInstance().confObject.RetrieveObject<CfgEnumerator>(businessAttributeQuery);

                    if (businessAttribute != null)
                    {
                        CfgEnumeratorValueQuery attributeValuesQuery = new CfgEnumeratorValueQuery();
                        attributeValuesQuery.EnumeratorDbid = businessAttribute.DBID;
                        var attributeValues = StatisticsSetting.GetInstance().confObject.RetrieveMultipleObjects<CfgEnumeratorValue>(attributeValuesQuery);
                        if (attributeValues != null && attributeValues.Count > 0)
                        {
                            logger.Debug("Retrieving values from business attribute");
                            foreach (CfgEnumeratorValue enumerator in attributeValues)
                            {
                                if (string.Compare(enumerator.Name, "Stat1", true) == 0)
                                {
                                    #region BusinessAttribute Annex

                                    string[] BAttributeKeys = enumerator.UserProperties.AllKeys;

                                    foreach (string section in BAttributeKeys)
                                    {
                                        if (string.Compare(section, sectionName, true) == 0)
                                        {
                                            #region Read Team Communicator Statistics

                                            KeyValueCollection kvColl = new KeyValueCollection();
                                            kvColl = (KeyValueCollection)enumerator.UserProperties[section];

                                            #region ANNEX

                                            logger.Debug("-------------------------------------------------------------------");
                                            logger.Debug("StatisticsBase  : ReadBusinessAttributeDetails : " + section.ToString());
                                            logger.Debug("-------------------------------------------------------------------");
                                            foreach (string Key in kvColl.AllKeys)
                                            {
                                                if (string.Compare(Key, "Filter", true) == 0)
                                                {
                                                    statMetricDetail.Add(Key, kvColl[Key].ToString());
                                                    logger.Debug("StatisticsBase  : ReadBusinessAttributeDetails : " + kvColl[Key].ToString());
                                                }
                                                else if (string.Compare(Key, "Category", true) == 0)
                                                {
                                                    statMetricDetail.Add(Key, kvColl[Key].ToString());
                                                    logger.Debug("StatisticsBase  : ReadBusinessAttributeDetails : " + kvColl[Key].ToString());
                                                }
                                                else if (string.Compare(Key, "MainMask", true) == 0)
                                                {
                                                    statMetricDetail.Add(Key, kvColl[Key].ToString());
                                                    logger.Debug("StatisticsBase  : ReadBusinessAttributeDetails : " + kvColl[Key].ToString());
                                                }
                                                else if (string.Compare(Key, "RelMask", true) == 0)
                                                {
                                                    statMetricDetail.Add(Key, kvColl[Key].ToString());
                                                    logger.Debug("StatisticsBase  : ReadBusinessAttributeDetails : " + kvColl[Key].ToString());
                                                }
                                                else if (string.Compare(Key, "Subject", true) == 0)
                                                {
                                                    statMetricDetail.Add(Key, kvColl[Key].ToString());
                                                    logger.Debug("StatisticsBase  : ReadBusinessAttributeDetails : " + kvColl[Key].ToString());
                                                }

                                            }
                                            #endregion

                                            #endregion
                                        }

                                    }

                                    #endregion

                                }
                            }
                        }
                    }
                    #endregion

                    if (statMetricDetail.Count != 0)
                    {
                        foreach (string key in statMetricDetail.Keys)
                        {
                            if (string.Compare(key, "Category", true) == 0)
                            {
                                if (statMetricDetail[key] != null)
                                {
                                    var values = Enum.GetValues(typeof(StatisticCategory));
                                    foreach (StatisticCategory categoryItem in values)
                                    {
                                        if (string.Compare(categoryItem.ToString(), statMetricDetail[key], true) == 0)
                                        {
                                            requestStat.StatisticMetricEx.Category = categoryItem;
                                        }
                                    }
                                }
                            }
                            else if (string.Compare(key, "MainMask", true) == 0)
                            {
                                mainMask = ActionsMask.CreateDnActionMask();

                                string[] actions = statMetricDetail[key].ToString().Split(',');

                                foreach (string customAction in actions)
                                {
                                    string myAction = string.Empty;
                                    if (customAction.Contains('~'))
                                    {

                                        myAction = customAction.Substring(1, customAction.Length - 1);
                                        var values = Enum.GetValues(typeof(DnActions));
                                        foreach (DnActions action in values)
                                        {
                                            if (string.Compare(action.ToString(), myAction, true) == 0)
                                            {
                                                mainMask.ClearBit(action);
                                            }
                                        }
                                    }
                                    else if (customAction.Contains('*'))
                                    {
                                        var values = Enum.GetValues(typeof(DnActions));
                                        foreach (DnActions action in values)
                                        {
                                            mainMask.SetBit(action);
                                        }
                                    }
                                    else
                                    {
                                        var values = Enum.GetValues(typeof(DnActions));
                                        foreach (DnActions action in values)
                                        {
                                            if (string.Compare(action.ToString(), customAction.Trim().ToString(), true) == 0)
                                            {
                                                mainMask.SetBit(action);
                                            }
                                        }
                                    }
                                }
                            }
                            else if (string.Compare(key, "RelMask", true) == 0)
                            {
                                relMask = ActionsMask.CreateDnActionMask();

                                if (statMetricDetail[key] != null)
                                {
                                    string[] actions = statMetricDetail[key].ToString().Split(',');

                                    foreach (string customAction in actions)
                                    {
                                        string myAction = string.Empty;
                                        if (customAction.Contains('~'))
                                        {
                                            myAction = customAction.Substring(1, customAction.Length - 1);
                                            var values = Enum.GetValues(typeof(DnActions));
                                            foreach (DnActions action in values)
                                            {
                                                if (string.Compare(action.ToString(), myAction, true) == 0)
                                                {
                                                    mainMask.ClearBit(action);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            var values = Enum.GetValues(typeof(DnActions));
                                            foreach (DnActions action in values)
                                            {
                                                if (string.Compare(action.ToString(), customAction.Trim().ToString(), true) == 0)
                                                {
                                                    relMask.SetBit(action);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            else if (string.Compare(key, "Subject", true) == 0)
                            {
                                if (statMetricDetail[key] != null)
                                {
                                    var values = Enum.GetValues(typeof(StatisticSubject));
                                    foreach (StatisticSubject subjectItem in values)
                                    {
                                        if (string.Compare(subjectItem.ToString(), statMetricDetail[key], true) == 0)
                                        {
                                            requestStat.StatisticMetricEx.Subject = subjectItem;
                                        }
                                    }
                                }

                            }
                            else if (string.Compare(key, "Filter", true) == 0)
                            {
                                if (statMetricDetail[key] != null)
                                {
                                    requestStat.StatisticMetricEx.Filter = statMetricDetail[key].ToString();
                                }
                                else
                                {
                                    requestStat.StatisticMetricEx.Filter = string.Empty;
                                }

                            }
                        }
                    }
                    requestStat.StatisticMetricEx.IntervalType = StatisticInterval.GrowingWindow;
                    requestStat.StatisticMetricEx.IntervalLength = 0;
                    requestStat.StatisticMetricEx.MainMask = mainMask;
                    requestStat.StatisticMetricEx.RelativeMask = relMask;
                    requestStat.ReferenceId = refID;
                    #endregion

                    IMessage response = statRequest.StatRequest(requestStat);

                    logger.Info("StatisticsBase : GetRequest Method : Response : " + response.ToString());

                    if (response != null)
                    {
                        switch (response.Id)
                        {
                            case EventStatisticOpened.MessageId:
                                EventStatisticOpened info;
                                info = (EventStatisticOpened)response;
                                logger.Trace("StatisticsBase : GetRequest Method : EventStatisticsOpened : " + info.Name);
                                break;
                            case EventError.MessageId:
                                EventError eventError = (EventError)response;
                                logger.Trace("StatisticsBase : GetRequest Method : EventError : " + eventError.StringValue);
                                break;
                        }
                    }
                    refID++;
                }

            }
            catch (Exception GeneralException)
            {
                logger.Error("StatisticsBase : GetRequest Method : Exception Caught : " + GeneralException.Message);
            }
            logger.Debug("StatisticsBase : GetRequest Method : Exit");
        }

        /// <summary>
        /// Gets the color of the server.
        /// </summary>
        /// <returns></returns>
        public Color GetServerColor()
        {
            return StatisticsSetting.GetInstance().statisticsCollection.StatisticsCommon.ServerColor;
        }

        public string GetServerStatColor()
        {
            return StatisticsSetting.GetInstance().statisticsCollection.AdminValues.NewColor.Name.ToString();
        }

        /// <summary>
        /// Gets the server stats.
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, KeyValueCollection> GetServerStats()
        {
            logger.Debug("StatisticsBase : GetServerStats : Method Entry");
            try
            {
                if (StatisticsSetting.GetInstance().statisticsCollection.StatisticsLocalSetting.PrimaryStatServer != null)
                {
                    statisticsInformation.ReadServerStatistcis();
                }
                else
                {
                    logger.Error("StatisticsBase : GetServerStats : PrimaryServer is Null");
                }
            }
            catch (Exception GeneralException)
            {
                logger.Debug("StatisticsBase : GetServerStats : " + GeneralException.Message);
            }
            logger.Debug("StatisticsBase : GetServerStats : Method Exit");

            return StatisticsSetting.GetInstance().ServerStatistics;
        }

        public Dictionary<string, bool> GetServerStatus()
        {
            return StatisticsSetting.GetInstance().serverNames;
        }

        /// <summary>
        /// Gets the switches.
        /// </summary>
        /// <returns></returns>
        public List<string> GetSwitches()
        {
            List<string> lstSwitch = null;
            try
            {
                logger.Debug("StatisticsBase : GetSwitches Method : Entry");
                if (!string.IsNullOrEmpty(StatisticsSetting.GetInstance().statisticsCollection.StatisticsCommon.Switches))
                {
                    lstSwitch = new List<string>();

                    string[] switches = StatisticsSetting.GetInstance().statisticsCollection.StatisticsCommon.Switches.Split(',');

                    foreach (string switchName in switches)
                    {
                        lstSwitch.Add(switchName);
                    }
                }
                else
                {
                    CfgSwitchQuery _switchQuery = new CfgSwitchQuery();
                    _switchQuery.TenantDbid = StatisticsSetting.GetInstance().CFGTenantDBID;

                    ICollection<CfgSwitch> ListSwitches = StatisticsSetting.GetInstance().confObject.RetrieveMultipleObjects<CfgSwitch>(_switchQuery);

                    if (ListSwitches.Count > 0)
                    {
                        lstSwitch = new List<string>();

                        foreach (CfgSwitch switchs in ListSwitches)
                        {
                            lstSwitch.Add(switchs.Name);
                        }
                    }
                }
            }
            catch (Exception GeneralException)
            {
                logger.Debug("StatisticsBase : GetSwitches Method : " + GeneralException.Message);
            }
            logger.Debug("StatisticsBase : GetSwitches Method : Exit");
            return lstSwitch;
        }

        /// <summary>
        /// Gets the tagged stats.
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> GetTaggedStats()
        {
            return StatisticsSetting.GetInstance().DictTaggedStats;
        }

        public bool IsDisplayStatistics()
        {
            return StatisticsSetting.GetInstance().statisticsCollection.ApplicationContainer.EnableMyQueueStatistics;
        }

        /// <summary>
        /// Determines whether [is statistics bold].
        /// </summary>
        /// <returns></returns>
        public bool IsStatisticsBold()
        {
            return StatisticsSetting.GetInstance().statisticsCollection.ApplicationContainer.StatBold;
        }

        /// <summary>
        /// Determines whether [is statistics configured].
        /// </summary>
        /// <returns>
        ///   <c>true</c> if [is statistics configured]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsStatisticsConfigured()
        {
            bool isStatsConfigured = false;
            try
            {
                logger.Debug("StatisticsBase : IsStatisticsConfigured : Method Entry");

                if (StatisticsBase.GetInstance().StatSource == StatisticsEnum.StatSource.DB.ToString())
                {
                    if (StatisticsSetting.GetInstance().statisticsCollection.DBValues[0].Query != "" || StatisticsSetting.GetInstance().statisticsCollection.DBValues[0].Query != "")
                        isStatsConfigured = true;
                    else
                        isStatsConfigured = false;
                }
                else if (StatisticsBase.GetInstance().StatSource == StatisticsEnum.StatSource.StatServer.ToString())
                {
                    string agentstat = StatisticsSetting.GetInstance().statisticsCollection.StatisticsCommon.AgentStatistics;
                    string agentgroupstat = StatisticsSetting.GetInstance().statisticsCollection.StatisticsCommon.AgentGroupStatistics;
                    string acdstat = StatisticsSetting.GetInstance().statisticsCollection.StatisticsCommon.ACDStatistics;
                    string dngroupstat = StatisticsSetting.GetInstance().statisticsCollection.StatisticsCommon.DNGroupStatistics;
                    string vqueuestat = StatisticsSetting.GetInstance().statisticsCollection.StatisticsCommon.VQueueStatistics;

                    if ((agentstat == "" || agentstat == null) && (agentgroupstat == "" || agentgroupstat == null) &&
                        (acdstat == "" || acdstat == null) && (dngroupstat == "" || dngroupstat == null) &&
                        (vqueuestat == "" || vqueuestat == null))
                        isStatsConfigured = false;
                    else
                        isStatsConfigured = true;
                }
                else if (StatisticsBase.GetInstance().StatSource == StatisticsEnum.StatSource.All.ToString())
                {
                    string agentstat = string.Empty;
                    string agentgroupstat = string.Empty;
                    string acdstat = string.Empty;
                    string dngroupstat = string.Empty;
                    string vqueuestat = string.Empty;
                    string DBQuery = string.Empty;

                    if (StatisticsSetting.GetInstance().statisticsCollection.StatisticsCommon != null)
                    {
                        agentstat = StatisticsSetting.GetInstance().statisticsCollection.StatisticsCommon.AgentStatistics;
                        agentgroupstat = StatisticsSetting.GetInstance().statisticsCollection.StatisticsCommon.AgentGroupStatistics;
                        acdstat = StatisticsSetting.GetInstance().statisticsCollection.StatisticsCommon.ACDStatistics;
                        dngroupstat = StatisticsSetting.GetInstance().statisticsCollection.StatisticsCommon.DNGroupStatistics;
                        vqueuestat = StatisticsSetting.GetInstance().statisticsCollection.StatisticsCommon.VQueueStatistics;
                    }

                    if (StatisticsSetting.GetInstance().statisticsCollection.DBValues != null)
                    {
                        if (StatisticsSetting.GetInstance().statisticsCollection.DBValues.Count != 0)
                            DBQuery = StatisticsSetting.GetInstance().statisticsCollection.DBValues[0].Query;
                        else
                            DBQuery = string.Empty;
                    }

                    if (((agentstat != "" || agentstat != null) && (agentgroupstat != "" || agentgroupstat != null) &&
                        (acdstat != "" || acdstat != null) && (dngroupstat != "" || dngroupstat != null) &&
                        (vqueuestat != "" || vqueuestat != null)) || (DBQuery != null || DBQuery != string.Empty))
                        isStatsConfigured = true;
                    else
                        isStatsConfigured = false;
                }
            }
            catch (Exception GeneralException)
            {
                logger.Error("StatisticsBase : IsStatisticsConfigured Method : " + GeneralException.Message);
            }
            logger.Debug("StatisticsBase : IsStatisticsConfigured Method : Exit");
            return isStatsConfigured;
        }

        /// <summary>
        /// Notifies the DB statistics.
        /// </summary>
        /// <param name="refid">The refid.</param>
        public void NotifyDBStatistics(string refid)
        {
            logger.Debug("StatisticsBase : NotifyDBStatistics Method : Entry");
            ThresholdSettings getColor = new ThresholdSettings();
            try
            {

                if (StatisticsSetting.GetInstance().DictDBStatHolder.ContainsKey(refid))
                {
                    DBValues tempDBValues = StatisticsSetting.GetInstance().DictDBStatHolder[refid];

                    string StatValue = string.Empty;

                    if (string.Compare(tempDBValues.Format, "t", true) == 0)
                    {
                        string Hours = string.Empty;
                        string Minutes = string.Empty;

                        Decimal dec = (StatisticsSetting.GetInstance().DictDBStatValuesHolder[refid].ToString() != null ? decimal.Parse(StatisticsSetting.GetInstance().DictDBStatValuesHolder[refid].ToString(), System.Globalization.NumberStyles.Any) : 0);

                        TimeSpan span = TimeSpan.FromSeconds(Convert.ToInt64(dec));

                        if (StatisticsSetting.GetInstance().statisticsCollection.ApplicationContainer.EnableHHMMSS)
                        {
                            StatValue = span.ToString();
                        }
                        else
                        {
                            Hours = span.Hours.ToString().Length > 1 ? span.Hours.ToString() : "0" + span.Hours.ToString();
                            Minutes = span.Minutes.ToString().Length > 1 ? span.Minutes.ToString() : "0" + span.Minutes.ToString();

                            StatValue = Hours + ":" + Minutes;
                        }
                    }
                    else if (string.Compare(tempDBValues.Format, "d", true) == 0)
                    {
                        StatValue = Convert.ToDouble(StatisticsSetting.GetInstance().DictDBStatValuesHolder[refid]).ToString();
                    }
                    else if (string.Compare(tempDBValues.Format, "s", true) == 0)
                    {
                        StatValue = Convert.ToDouble(StatisticsSetting.GetInstance().DictDBStatValuesHolder[refid]).ToString();
                    }

                    foreach (KeyValuePair<string, IStatTicker> messageToClient in ToClient)
                    {
                        messageToClient.Value.NotifyDBStatistics(refid, tempDBValues.DisplayName, StatValue, tempDBValues.TooltipName, getColor.ThresholdColor(StatisticsSetting.GetInstance().DictDBStatValuesHolder[refid].ToString(), statInfo.ObjectID, refid, tempDBValues.Format), StatisticsSetting.GetInstance().isThresholdBreach, tempDBValues.TempStat, StatisticsSetting.GetInstance().isLevelTwo);
                    }
                }

            }
            catch (Exception GeneralException)
            {
                logger.Error("StatisticsBase : NotifyDBStatistics Method : Exception Caught : " + GeneralException.Message);
            }
            logger.Debug("StatisticsBase : NotifyDBStatistics Method : Exit");
        }

        /// <summary>
        /// Reads all ACD queues.
        /// </summary>
        /// <param name="switchName">Name of the switch.</param>
        /// <returns></returns>
        public List<string> ReadAllACDQueues(IList<string> switchName)
        {
            List<string> _listAllACDQueues = new List<string>();
            try
            {
                logger.Debug("StatisticsBase : ReadAllACDQueues Method : Entry");

                if (switchName != null)
                {
                    foreach (string _switch in switchName)
                    {
                        CfgSwitchQuery switchQuery = new CfgSwitchQuery();
                        switchQuery.Name = _switch.ToString().Trim();
                        switchQuery.TenantDbid = StatisticsSetting.GetInstance().CFGTenantDBID;
                        switchQuery["objecttype"] = CfgObjectType.CFGSwitch;
                        CfgSwitch switchDetails = StatisticsSetting.GetInstance().confObject.RetrieveObject<CfgSwitch>(switchQuery);
                        if (switchDetails != null)
                        {
                            CfgDNQuery dnQuery = new CfgDNQuery();
                            dnQuery.SwitchDbid = switchDetails.DBID;
                            dnQuery.DnType = CfgDNType.CFGACDQueue;
                            IList<CfgDN> _listACDQueues = StatisticsSetting.GetInstance().confObject.RetrieveMultipleObjects<CfgDN>(dnQuery) as IList<CfgDN>;

                            if (_listACDQueues != null)
                            {
                                if (_listACDQueues.Count > 0)
                                {
                                    logger.Info("StatisticsBase : ReadAllACDQueues Method : ACD Queue count" + _listACDQueues.Count.ToString());

                                    foreach (CfgDN ACDQueue in _listACDQueues)
                                    {
                                        _listAllACDQueues.Add(ACDQueue.Number.ToString().Trim() + "_@" + switchDetails.Name);
                                    }
                                }

                                logger.Info("StatisticsBase : ReadAllACDQueues Method : ACD Queues count" + _listACDQueues.Count.ToString());
                            }
                        }
                    }
                }
            }
            catch (Exception generalException)
            {
                logger.Debug("StatisticsBase : ReadAllACDQueues Method : Exception Caught : " + generalException.Message);
            }
            finally
            {
                GC.Collect();
                logger.Debug("StatisticsBase : ReadAllACDQueues Method : Exit");
            }

            return _listAllACDQueues;
        }

        /// <summary>
        /// Reads all agent groups.
        /// </summary>
        /// <returns></returns>
        public List<string> ReadAllAgentGroups()
        {
            List<string> _listAllGroups = new List<string>();
            try
            {
                logger.Debug("StatisticsBase : ReadAllAgentGroups : Method Entry");
                CfgAgentGroupQuery agentGropupQuery = new CfgAgentGroupQuery();
                agentGropupQuery.TenantDbid = StatisticsSetting.GetInstance().CFGTenantDBID;
                agentGropupQuery["objecttype"] = CfgObjectType.CFGAgentGroup;
                IList<CfgAgentGroup> _listAgentGroup = StatisticsSetting.GetInstance().confObject.RetrieveMultipleObjects<CfgAgentGroup>(agentGropupQuery) as IList<CfgAgentGroup>;

                if (_listAgentGroup != null)
                {
                    if (_listAgentGroup.Count > 0)
                    {
                        logger.Info("StatisticsBase : ReadAllAgentGroups Method : Agent Group count" + _listAgentGroup.Count.ToString());

                        foreach (CfgAgentGroup _agentGroup in _listAgentGroup)
                        {
                            _listAllGroups.Add(_agentGroup.GroupInfo.Name.ToString().Trim());
                        }
                    }
                    logger.Info("StatisticsBase : ReadAllAgentGroups Method : Agent Group count" + _listAgentGroup.Count.ToString());
                }
            }
            catch (Exception generalException)
            {
                logger.Error("StatisticsBase : ReadAllAgentGroups : Exception Caught : " + generalException.Message);
            }
            finally
            {
                GC.Collect();
                logger.Debug("StatisticsBase : ReadAllAgentGroups : Method Exit");
            }

            StatisticsSetting.GetInstance().LstAgentGroups = _listAllGroups;
            return _listAllGroups;
        }

        /// <summary>
        /// Reads all persons.
        /// </summary>
        /// <returns></returns>
        public List<string> ReadAllAgents()
        {
            List<string> _listAllPerson = new List<string>();
            try
            {
                logger.Debug("StatisticsBase : ReadAllAgents : Method Entry");
                CfgPersonQuery personQuery = new CfgPersonQuery();
                personQuery.TenantDbid = StatisticsSetting.GetInstance().CFGTenantDBID;
                personQuery["objecttype"] = CfgObjectType.CFGPerson;
                personQuery.IsAgent = (Int32)CfgFlag.CFGTrue;
                IList<CfgPerson> _listPerson = StatisticsSetting.GetInstance().confObject.RetrieveMultipleObjects<CfgPerson>(personQuery) as IList<CfgPerson>;
                if (_listPerson != null)
                {
                    if (_listPerson.Count > 0)
                    {
                        logger.Info("StatisticsBase : ReadAllAgents Method : Agent count" + _listPerson.Count.ToString());

                        foreach (CfgPerson _person in _listPerson)
                        {
                            StatisticsSetting.GetInstance().LstAgents.Add(_person.EmployeeID);
                            _listAllPerson.Add((string.IsNullOrEmpty(_person.FirstName) ? string.Empty : _person.FirstName.Trim()) + " " + (string.IsNullOrEmpty(_person.LastName) ? string.Empty : _person.LastName.Trim()) + "*&" + _person.EmployeeID);
                        }
                    }

                    logger.Info("StatisticsBase : ReadAllAgents Method : Agent count" + _listPerson.Count.ToString());
                }
            }
            catch (Exception generalException)
            {
                logger.Error("StatisticsBase : ReadAllAgents : Exception Caught : " + generalException.Message);
            }
            finally
            {
                GC.Collect();
                logger.Debug("StatisticsBase : ReadAllAgents : Method Exit");
            }
            return _listAllPerson;
        }

        /// <summary>
        /// Reads all DN groups.
        /// </summary>
        /// <returns></returns>
        public List<string> ReadAllDNGroups()
        {
            List<string> _listAllDNGroups = new List<string>();
            try
            {
                logger.Debug("StatisticsBase : ReadAllDNGroups Method : Entry");
                CfgDNGroupQuery dnGroupQuery = new CfgDNGroupQuery();
                dnGroupQuery.TenantDbid = StatisticsSetting.GetInstance().CFGTenantDBID;
                //Commented for GUD PHS, by Elango
                //dnGroupQuery["objecttype"] = CfgObjectType.CFGDNGroup;
                IList<CfgDNGroup> _listDNGroup = StatisticsSetting.GetInstance().confObject.RetrieveMultipleObjects<CfgDNGroup>(dnGroupQuery) as IList<CfgDNGroup>;
                if (_listDNGroup != null)
                {
                    if (_listDNGroup.Count > 0)
                    {
                        logger.Info("StatisticsBase : ReadAllDNGroups Method : DN Group count" + _listDNGroup.Count.ToString());

                        foreach (CfgDNGroup dnGroup in _listDNGroup)
                        {
                            _listAllDNGroups.Add(dnGroup.GroupInfo.Name.ToString().Trim());
                        }
                    }

                    logger.Info("StatisticsBase : ReadAllDNGroups Method :  DN Group count" + _listDNGroup.Count.ToString());
                }
            }
            catch (Exception generalException)
            {
                logger.Error("StatisticsBase : ReadAllDNGroups Method : Exception Caught : " + generalException.Message);
            }
            finally
            {
                GC.Collect();
                logger.Debug("StatisticsBase : ReadAllDNGroups Method : Exit");
            }
            return _listAllDNGroups;
        }

        /// <summary>
        /// Reads all virtual queues.
        /// </summary>
        /// <param name="switchName">Name of the switch.</param>
        /// <returns></returns>
        public List<string> ReadAllVirtualQueues(IList<string> switchName)
        {
            List<string> _listAllVirtualQueues = new List<string>();
            try
            {
                logger.Debug("StatisticsBase : ReadAllVirtualQueues Method : Entry");
                if (switchName != null)
                {
                    foreach (string _switch in switchName)
                    {
                        CfgSwitchQuery switchQuery = new CfgSwitchQuery();
                        switchQuery.Name = _switch.ToString().Trim();
                        switchQuery.TenantDbid = StatisticsSetting.GetInstance().CFGTenantDBID;
                        switchQuery["objecttype"] = CfgObjectType.CFGSwitch;
                        CfgSwitch switchDetails = StatisticsSetting.GetInstance().confObject.RetrieveObject<CfgSwitch>(switchQuery);
                        if (switchDetails != null)
                        {
                            CfgDNQuery dnQuery = new CfgDNQuery();
                            dnQuery.SwitchDbid = switchDetails.DBID;
                            dnQuery.DnType = CfgDNType.CFGVirtACDQueue;
                            IList<CfgDN> _listVQueues = StatisticsSetting.GetInstance().confObject.RetrieveMultipleObjects<CfgDN>(dnQuery) as IList<CfgDN>;

                            if (_listVQueues != null)
                            {
                                if (_listVQueues.Count > 0)
                                {
                                    logger.Info("StatisticsBase : ReadAllVirtualQueues Method : Virtual Queues count" + _listVQueues.Count.ToString());

                                    foreach (CfgDN VQueue in _listVQueues)
                                    {
                                        _listAllVirtualQueues.Add(VQueue.Number.ToString().Trim() + "_@" + switchDetails.Name);
                                    }
                                }
                                logger.Info("StatisticsBase : ReadAllVirtualQueues Method : Virtual Queues count" + _listVQueues.Count.ToString());
                            }
                        }
                    }
                }
            }
            catch (Exception generalException)
            {
                logger.Debug("StatisticsBase : ReadAllVirtualQueues Method : Exception Caught : " + generalException.Message);
            }
            finally
            {
                logger.Debug("StatisticsBase : ReadAllVirtualQueues Method : Exit");
                GC.Collect();
            }

            return _listAllVirtualQueues;
        }

        public Dictionary<string, List<CfgPerson>> ReadDisplayValues()
        {
            statisticsInformation.ReadAdminValues();
            return StatisticsSetting.GetInstance().DictDisplayObjects;
        }

        /// <summary>
        /// Reads the filters.
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> ReadFilters()
        {
            logger.Debug("StatisticsBase : ReadFilters : Method Entry");
            try
            {
                if (StatisticsSetting.GetInstance().statisticsCollection.StatisticsLocalSetting.PrimaryStatServer != null)
                {
                    statisticsInformation.ReadServerFilters();
                }
            }
            catch (Exception GeneralException)
            {
                logger.Error("StatisticsBase : ReadFilters : " + GeneralException.Message);
            }
            logger.Debug("StatisticsBase : ReadFilters : Method Exit");

            return StatisticsSetting.GetInstance().ServerFilters;
        }

        /// <summary>
        /// Reads the statistics.
        /// </summary>
        /// <param name="ObjectId">The object id.</param>
        /// <param name="ObjectType">Type of the object.</param>
        /// <returns></returns>
        public List<string> ReadStatistics(string ObjectId, string ObjectType)
        {
            return statisticsInformation.ReadStatisticValues(ObjectId, ObjectType);
        }

        /// <summary>
        /// Reads the statistics objects.
        /// </summary>
        /// <param name="objectId">The object id.</param>
        /// <param name="objectType">Type of the object.</param>
        /// <returns></returns>
        public Dictionary<string, Dictionary<string, List<string>>> ReadStatisticsObjects(string objectId, string objectType)
        {
            return statisticsInformation.ReadStatisticsObjects(objectId, objectType);
        }

        /// <summary>
        /// Reads the statistics reference unique identifier.
        /// </summary>
        /// <param name="RefernceId">The refernce unique identifier.</param>
        public void ReadStatisticsReferenceId(int RefernceId)
        {
            try
            {
                logger.Debug("StatisticsBase : ReadStatisticsReferenceId Method : Entry");

                StatisticsSetting.GetInstance().ReferenceIdLimit = RefernceId;
            }
            catch (Exception GeneralException)
            {
                logger.Error("StatisticsBase : ReadStatisticsReferenceId Method : Exception Caught : " + GeneralException.Message);

            }
            logger.Debug("StatisticsBase : ReadStatisticsReferenceId Method: Exit");
        }

        /// <summary>
        /// Reportings the success message.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        public void ReportingSuccessMessage(object sender, EventArgs e)
        {
            IMessage message = ((MessageEventArgs)e).Message;
            if (message == null)
                return;
            try
            {
                logger.Debug("StatisticsBase : ReportingSuccessMessage Method : Entry");
                logger.Debug("StatisticsBase : ReportingSuccessMessage Method : Server Response : " + message.Id.ToString());
                if (message != null)
                {
                    switch (message.Id)
                    {
                        case EventInfo.MessageId:
                            EventInfo newPackageMessage = message as EventInfo;
                            if (newPackageMessage != null)
                            {
                                if (StatisticsSetting.GetInstance().FinalAgentPackageID.Contains(newPackageMessage.ReferenceId))
                                    NotifyAgentValues(newPackageMessage.ReferenceId, newPackageMessage.StringValue, newPackageMessage);
                                //ThreadPool.QueueUserWorkItem(new WaitCallback(NotifyAgentValues(newPackageMessage.ReferenceId,newPackageMessage.StringValue));
                                else if (StatisticsSetting.GetInstance().FinalAgentGroupPackageID.Contains(newPackageMessage.ReferenceId))
                                    NotifyAgentGroupValues(newPackageMessage.ReferenceId, newPackageMessage.StringValue);
                                // ThreadPool.QueueUserWorkItem(new WaitCallback(NotifyAgentGroupValues), newPackageMessage.StringValue);
                                else if (StatisticsSetting.GetInstance().FinalACDPackageID.Contains(newPackageMessage.ReferenceId))
                                    NotifyACDQueueValues(newPackageMessage.ReferenceId, newPackageMessage.StringValue);
                                //ThreadPool.QueueUserWorkItem(new WaitCallback(NotifyACDQueueValues), newPackageMessage.StringValue);
                                else if (StatisticsSetting.GetInstance().FinalDNGroupsPackageID.Contains(newPackageMessage.ReferenceId))
                                    NotifyDNGroupValues(newPackageMessage.ReferenceId, newPackageMessage.StringValue);
                                //ThreadPool.QueueUserWorkItem(new WaitCallback(NotifyDNGroupValues), newPackageMessage.StringValue);
                                else if (StatisticsSetting.GetInstance().FinalVQPackageID.Contains(newPackageMessage.ReferenceId))
                                    NotifyVQueueValues(newPackageMessage.ReferenceId, newPackageMessage.StringValue);
                                //ThreadPool.QueueUserWorkItem(new WaitCallback(NotifyVQueueValues), newPackageMessage.StringValue);
                                else if (StatisticsSetting.GetInstance().ReferenceIdLimit != 0 && newPackageMessage.ReferenceId >= StatisticsSetting.GetInstance().ReferenceIdLimit)
                                    NotifyAIDStatistics(newPackageMessage);
                                //ThreadPool.QueueUserWorkItem(new WaitCallback(NotifyAIDStatistics), newPackageMessage.StringValue);
                                else if (StatisticsSetting.GetInstance().FinalAdminPackageID.Contains(newPackageMessage.ReferenceId))
                                    NotifyAdminValues(newPackageMessage);
                                else if (StatisticsSetting.GetInstance().TeamCommRefIDs.ContainsKey(newPackageMessage.ReferenceId))
                                    NotifyTeamCommValues(newPackageMessage);

                                //ThreadPool.QueueUserWorkItem(new WaitCallback(NotifyAdminValues), newPackageMessage.StringValue);
                            }
                            break;

                        case EventError.MessageId:
                            EventError packageError = message as EventError;
                            if (packageError != null)
                            {
                                logger.Warn("StatisticsBase : ReportingSuccessMessage : packageError :  " + packageError.StringValue);
                            }
                            break;

                        default:

                            break;
                    }
                }
            }
            catch (Exception generalException)
            {
                logger.Error("StatisticsBase : ReportingSuccessMessage Method : " + generalException.Message);
            }
            finally
            {
                //GC.Collect();
                logger.Debug("StatisticsBase : ReportingSuccessMessage Method : Exit");
            }
        }

        public void RequestAgentStatus()
        {
            Request statreq = new Request();
            int refId = 1;

            try
            {
                if (StatisticsSetting.GetInstance().LstPersons != null && StatisticsSetting.GetInstance().LstPersons.Count != 0)
                {
                    foreach (CfgPerson agent in StatisticsSetting.GetInstance().LstPersons)
                    {
                        IMessage response = statreq.StatRequest(StatisticsSetting.GetInstance().statisticsCollection.StatisticsCommon.TenantName, agent.EmployeeID,
                                                StatisticObjectType.Agent, "CurrentStateReason", StatisticsSetting.GetInstance().statisticsCollection.StatisticsCommon.NotifySeconds,
                                                 string.Empty, StatisticsSetting.GetInstance().statisticsCollection.StatisticsCommon.Insensitivity, refId, "");
                        StatisticsSetting.GetInstance().FinalAdminPackageID.Add(refId);
                        refId++;
                    }
                }
            }
            catch (Exception GeneralException)
            {

            }
        }

        public void RequestIndividualStatistics(string empId, string application)
        {
            try
            {
                ReadApplication read = new ReadApplication();
                StatisticsSetting.GetInstance().IndividualAgent = empId;
                read.ReadApplicationDetails(application, "StatServer");
                read.ReadAgent(empId, true);
                StatisticsSetting.GetInstance().statisticsCollection = read.ReadAgentGroupDetails(StatisticsSetting.GetInstance().statisticsCollection, false);
                StatisticsSetting.GetInstance().statisticsCollection = read.ReadAgentDetails(StatisticsSetting.GetInstance().statisticsCollection, empId);
                AgentStatisticsSubscriber agentSubscriber = new AgentStatisticsSubscriber();
                agentSubscriber.OnNext(StatisticsSetting.GetInstance().statisticsCollection);
            }
            catch (Exception GeneralException)
            {
            }
        }

        public IMessage RequestTeamComStats(RequestOpenStatisticEx requestStat, string serverName = "")
        {
            if (!StatisticsSetting.GetInstance().TeamCommRefIDs.ContainsKey(requestStat.ReferenceId))
            {
                StatisticsSetting.GetInstance().TeamCommRefIDs.Add(requestStat.ReferenceId, serverName);
            }
            IMessage message = statRequest.StatRequest(requestStat, serverName);

            if (message != null)
            {

                switch (message.Id)
                {

                    case EventStatisticOpened.MessageId:
                        EventStatisticOpened info;
                        info = (EventStatisticOpened)message;
                        if (!StatisticsSetting.GetInstance().TeamCommRefIDs.ContainsKey(info.ReferenceId))
                        {
                            StatisticsSetting.GetInstance().TeamCommRefIDs.Add(info.ReferenceId, serverName);
                        }
                        break;

                }
            }
            return message;
        }

        public OutputValues SaveAgentsTaggedStats(Dictionary<string, string> DicttaggedStats, bool isVertical, double top, double left, bool isHeader)
        {
            OutputValues output = new OutputValues();
            try
            {
                logger.Debug("StatisticsBase : SaveAgentTaggedStats : Method Entry");
                if (StatisticsSetting.GetInstance().isAgentConfigFromLocal)
                {
                    string agentTaggedStats = string.Empty;
                    if (DicttaggedStats.ContainsKey(StatisticsEnum.StatSource.StatServer.ToString()))
                    {
                        agentTaggedStats = DicttaggedStats[StatisticsEnum.StatSource.StatServer.ToString()];
                    }
                    else
                    {
                        agentTaggedStats = "";
                    }
                    SaveTaggedStatsInXML(agentTaggedStats, isVertical.ToString(), isHeader.ToString(), left.ToString(), top.ToString());
                    output.MessageCode = "2007";
                    output.Message = "Saved";
                }
                else
                {

                    if (StatisticsBase.GetInstance().StatSource == StatisticsEnum.StatSource.StatServer.ToString())
                    {
                        #region Source : Server

                        string tempStat = string.Empty;
                        string kvpname = "agent-tagged-statistics";
                        bool issaved = false;
                        int kvpno = 0;

                        KeyValueCollection agentDeatils = (KeyValueCollection)StatisticsSetting.GetInstance().PersonDetails.UserProperties["agent.ixn.desktop"];

                        if (agentDeatils == null)
                        {
                            KeyValueCollection taggs = new KeyValueCollection();
                            taggs.Add("agent-tagged-statistics", " ");
                            StatisticsSetting.GetInstance().PersonDetails.UserProperties.Add("agent.ixn.desktop", taggs);

                            StatisticsSetting.GetInstance().PersonDetails.Save();
                            agentDeatils = (KeyValueCollection)StatisticsSetting.GetInstance().PersonDetails.UserProperties["agent.ixn.desktop"];
                        }

                        List<string> kvpremoved = new List<string>();
                        foreach (string keys in agentDeatils.Keys)
                        {
                            if (keys.Contains(kvpname))
                            {
                                kvpremoved.Add(keys);
                            }
                        }

                        foreach (string removedkvp in kvpremoved)
                        {
                            agentDeatils.Remove(removedkvp);
                        }

                        if (DicttaggedStats.ContainsKey(StatisticsEnum.StatSource.StatServer.ToString()))
                        {
                            string taggedStats = DicttaggedStats[StatisticsEnum.StatSource.StatServer.ToString()];
                            string[] stattagged = taggedStats.Split(',');
                            int remaintags = 5;

                            if (stattagged.Length > 5)
                            {
                                for (int i = 5; i <= stattagged.Length; )
                                {
                                    tempStat = string.Empty;
                                    for (int j = i - remaintags; j < i; j++)
                                    {
                                        if (tempStat == string.Empty)
                                            tempStat = stattagged[j];
                                        else
                                            tempStat = tempStat + "," + stattagged[j];
                                    }

                                    kvpno++;
                                    string tempKVPname = kvpname + "_" + kvpno.ToString();
                                    if (agentDeatils.ContainsKey(tempKVPname))
                                    {
                                        agentDeatils.Remove(tempKVPname);
                                        agentDeatils[tempKVPname] = tempStat;
                                    }
                                    else
                                    {
                                        if (tempStat != "" && tempStat != null && tempStat != string.Empty)
                                            agentDeatils.Add(tempKVPname, tempStat);
                                    }
                                    if (issaved)
                                    {
                                        i = i + 5;
                                        break;
                                    }

                                    if (i + 5 > stattagged.Length)
                                    {
                                        remaintags = stattagged.Length - i;
                                        i = i + (stattagged.Length - i);
                                        issaved = true;
                                    }
                                    else
                                        i = i + 5;
                                }
                            }
                            else
                            {
                                tempStat = string.Empty;

                                if (tempStat == string.Empty)
                                    tempStat = taggedStats;

                                kvpno++;

                                string tempKVPname = kvpname + "_" + kvpno.ToString();

                                if (agentDeatils.ContainsKey(tempKVPname))
                                {
                                    agentDeatils.Remove(tempKVPname);
                                    agentDeatils[tempKVPname] = tempStat;
                                }
                                else
                                {
                                    agentDeatils.Add(tempKVPname, tempStat);
                                }

                            }
                        }

                        if (agentDeatils.ContainsKey("statistics.gadget-position"))
                        {
                            agentDeatils["statistics.gadget-position"] = left.ToString() + "," + top.ToString();
                        }
                        else
                        {
                            agentDeatils.Add("statistics.gadget-position", left.ToString() + "," + top.ToString());
                        }

                        StatisticsSetting.GetInstance().PersonDetails.UserProperties["agent.ixn.desktop"] = agentDeatils;
                        StatisticsSetting.GetInstance().PersonDetails.Save();

                        KeyValueCollection enableDisableDetails = (KeyValueCollection)StatisticsSetting.GetInstance().PersonDetails.UserProperties["enable-disable-channels"];

                        if (enableDisableDetails == null)
                        {
                            KeyValueCollection taggs = new KeyValueCollection();
                            taggs.Add("statistics.enable-tag-vertical", " ");
                            StatisticsSetting.GetInstance().PersonDetails.UserProperties.Add("enable-disable-channels", taggs);
                            taggs.Add("statistics.enable-header", " ");
                            StatisticsSetting.GetInstance().PersonDetails.UserProperties.Add("enable-disable-channels", taggs);

                            StatisticsSetting.GetInstance().PersonDetails.Save();
                            enableDisableDetails = (KeyValueCollection)StatisticsSetting.GetInstance().PersonDetails.UserProperties["enable-disable-channels"];
                        }

                        if (enableDisableDetails.ContainsKey("statistics.enable-tag-vertical"))
                        {
                            enableDisableDetails["statistics.enable-tag-vertical"] = isVertical.ToString().ToLower();
                        }
                        else
                        {
                            enableDisableDetails.Add("statistics.enable-tag-vertical", isVertical.ToString().ToLower());
                        }

                        if (enableDisableDetails.ContainsKey("statistics.enable-header"))
                        {
                            enableDisableDetails["statistics.enable-header"] = isHeader.ToString().ToLower();
                        }
                        else
                        {
                            enableDisableDetails.Add("statistics.enable-header", isHeader.ToString().ToLower());
                        }

                        StatisticsSetting.GetInstance().PersonDetails.Save();

                        if (settings.statServerProtocol != null)
                        {
                            if (settings.statServerProtocol.State != ChannelState.Closed || settings.statServerProtocol.State != ChannelState.Opening)
                                StatisticsSetting.GetInstance().PersonDetails.Save();
                        }

                        output.MessageCode = "2007";
                        output.Message = "Saved";

                        #endregion
                    }
                    else if (StatisticsBase.GetInstance().StatSource == StatisticsEnum.StatSource.DB.ToString())
                    {
                        #region Source : DB

                        string taggedStats = string.Empty;
                        if (DicttaggedStats.ContainsKey(StatisticsEnum.StatSource.DB.ToString()))
                        {
                            taggedStats = DicttaggedStats[StatisticsEnum.StatSource.DB.ToString()];
                        }
                        else
                        {
                            taggedStats = "";
                        }

                        XmlDataDocument xmldoc = new XmlDataDocument();
                        string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\PHS\\AgentInteractionDesktop\\app_db_config.xml";
                        xmldoc.Load(path);
                        XmlElement node1 = xmldoc.SelectSingleNode("/AppConfig/StatTickerFive/DbTaggedStats") as XmlElement;

                        if (node1 != null)
                        {
                            node1.InnerText = taggedStats;
                        }

                        node1 = null;
                        node1 = xmldoc.SelectSingleNode("/AppConfig/StatTickerFive/enable.disable-channels/statistics.enable-tag-vertical") as XmlElement;

                        if (node1 != null)
                        {
                            node1.InnerText = isVertical.ToString();
                        }

                        node1 = null;
                        node1 = xmldoc.SelectSingleNode("/AppConfig/StatTickerFive/enable.disable-channels/statistics.enable-header") as XmlElement;

                        if (node1 != null)
                        {
                            node1.InnerText = isHeader.ToString();
                        }
                        else
                        {
                            xmldoc.CreateNode(XmlNodeType.Element, "statistics.enable-header", "/AppConfig/StatTickerFive/enable.disable-channels/statistics.enable-header");
                            node1 = xmldoc.SelectSingleNode("/AppConfig/StatTickerFive/enable.disable-channels/statistics.enable-header") as XmlElement;
                            node1.InnerText = isHeader.ToString();
                        }

                        node1 = null;
                        node1 = xmldoc.SelectSingleNode("/AppConfig/StatTickerFive/DbSetting/Position") as XmlElement;

                        if (node1 != null)
                        {
                            node1.InnerText = left.ToString() + "," + top.ToString();
                        }

                        xmldoc.Save(path);

                        output.MessageCode = "2007";
                        output.Message = "Saved";

                        #endregion
                    }
                    else if (StatisticsBase.GetInstance().StatSource == StatisticsEnum.StatSource.All.ToString())
                    {
                        #region Source : All

                        string taggedStats = string.Empty;

                        if (StatisticsBase.GetInstance().isCMEAuthentication)
                        {
                            #region Source : Server

                            string tempStat = string.Empty;
                            string kvpname = "agent-tagged-statistics";
                            bool issaved = false;
                            int kvpno = 0;

                            KeyValueCollection agentDeatils = (KeyValueCollection)StatisticsSetting.GetInstance().PersonDetails.UserProperties["agent.ixn.desktop"];

                            if (agentDeatils == null)
                            {
                                KeyValueCollection taggs = new KeyValueCollection();
                                taggs.Add("agent-tagged-statistics", " ");
                                StatisticsSetting.GetInstance().PersonDetails.UserProperties.Add("agent.ixn.desktop", taggs);

                                StatisticsSetting.GetInstance().PersonDetails.Save();
                                agentDeatils = (KeyValueCollection)StatisticsSetting.GetInstance().PersonDetails.UserProperties["agent.ixn.desktop"];
                            }

                            List<string> kvpremoved = new List<string>();
                            foreach (string keys in agentDeatils.Keys)
                            {
                                if (keys.Contains(kvpname))
                                {
                                    kvpremoved.Add(keys);
                                }
                            }

                            foreach (string removedkvp in kvpremoved)
                            {
                                agentDeatils.Remove(removedkvp);
                            }

                            if (DicttaggedStats.ContainsKey(StatisticsEnum.StatSource.StatServer.ToString()))
                            {
                                taggedStats = DicttaggedStats[StatisticsEnum.StatSource.StatServer.ToString()];
                                string[] stattagged = taggedStats.Split(',');
                                int remaintags = 5;

                                if (stattagged.Length > 5)
                                {
                                    for (int i = 5; i <= stattagged.Length; )
                                    {
                                        tempStat = string.Empty;
                                        for (int j = i - remaintags; j < i; j++)
                                        {
                                            if (tempStat == string.Empty)
                                                tempStat = stattagged[j];
                                            else
                                                tempStat = tempStat + "," + stattagged[j];
                                        }

                                        kvpno++;
                                        string tempKVPname = kvpname + "_" + kvpno.ToString();
                                        if (agentDeatils.ContainsKey(tempKVPname))
                                        {
                                            agentDeatils.Remove(tempKVPname);
                                            agentDeatils[tempKVPname] = tempStat;
                                        }
                                        else
                                        {
                                            if (tempStat != "" && tempStat != null && tempStat != string.Empty)
                                                agentDeatils.Add(tempKVPname, tempStat);
                                        }
                                        if (issaved)
                                        {
                                            i = i + 5;
                                            break;
                                        }

                                        if (i + 5 > stattagged.Length)
                                        {
                                            remaintags = stattagged.Length - i;
                                            i = i + (stattagged.Length - i);
                                            issaved = true;
                                        }
                                        else
                                            i = i + 5;
                                    }
                                }
                                else
                                {
                                    tempStat = string.Empty;

                                    if (tempStat == string.Empty)
                                        tempStat = taggedStats;

                                    kvpno++;

                                    string tempKVPname = kvpname + "_" + kvpno.ToString();

                                    if (agentDeatils.ContainsKey(tempKVPname))
                                    {
                                        agentDeatils.Remove(tempKVPname);
                                        agentDeatils[tempKVPname] = tempStat;
                                    }
                                    else
                                    {
                                        agentDeatils.Add(tempKVPname, tempStat);
                                    }

                                }

                                if (agentDeatils.ContainsKey("statistics.gadget-position"))
                                {
                                    agentDeatils["statistics.gadget-position"] = left.ToString() + "," + top.ToString();
                                }
                                else
                                {
                                    agentDeatils.Add("statistics.gadget-position", left.ToString() + "," + top.ToString());
                                }

                            }

                            StatisticsSetting.GetInstance().PersonDetails.UserProperties["agent.ixn.desktop"] = agentDeatils;
                            StatisticsSetting.GetInstance().PersonDetails.Save();

                            KeyValueCollection enableDisableDetails = (KeyValueCollection)StatisticsSetting.GetInstance().PersonDetails.UserProperties["enable-disable-channels"];

                            if (enableDisableDetails == null)
                            {
                                KeyValueCollection taggs = new KeyValueCollection();
                                taggs.Add("statistics.enable-tag-vertical", " ");
                                StatisticsSetting.GetInstance().PersonDetails.UserProperties.Add("enable-disable-channels", taggs);
                                taggs.Add("statistics.enable-header", " ");
                                StatisticsSetting.GetInstance().PersonDetails.UserProperties.Add("enable-disable-channels", taggs);

                                StatisticsSetting.GetInstance().PersonDetails.Save();
                                enableDisableDetails = (KeyValueCollection)StatisticsSetting.GetInstance().PersonDetails.UserProperties["enable-disable-channels"];
                            }

                            if (enableDisableDetails.ContainsKey("statistics.enable-tag-vertical"))
                            {
                                enableDisableDetails["statistics.enable-tag-vertical"] = isVertical.ToString().ToLower();
                            }
                            else
                            {
                                enableDisableDetails.Add("statistics.enable-tag-vertical", isVertical.ToString().ToLower());
                            }

                            if (enableDisableDetails.ContainsKey("statistics.enable-header"))
                            {
                                enableDisableDetails["statistics.enable-header"] = isHeader.ToString().ToLower();
                            }
                            else
                            {
                                enableDisableDetails.Add("statistics.enable-header", isHeader.ToString().ToLower());
                            }

                            StatisticsSetting.GetInstance().PersonDetails.Save();

                            if (settings.statServerProtocol != null)
                            {
                                if (settings.statServerProtocol.State != ChannelState.Closed || settings.statServerProtocol.State != ChannelState.Opening)
                                    StatisticsSetting.GetInstance().PersonDetails.Save();
                            }

                            output.MessageCode = "2007";
                            output.Message = "Saved";

                            #endregion
                        }

                        if (StatisticsBase.GetInstance().isDBAuthentication)
                        {
                            #region Source : DB

                            taggedStats = string.Empty;
                            if (DicttaggedStats.ContainsKey(StatisticsEnum.StatSource.DB.ToString()))
                            {
                                taggedStats = DicttaggedStats[StatisticsEnum.StatSource.DB.ToString()];
                            }
                            else
                            {
                                taggedStats = "";
                            }

                            XmlDataDocument xmldoc = new XmlDataDocument();
                            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\PHS\\AgentInteractionDesktop\\app_db_config.xml";
                            xmldoc.Load(path);
                            XmlElement node1 = xmldoc.SelectSingleNode("/AppConfig/StatTickerFive/DbTaggedStats") as XmlElement;

                            if (node1 != null)
                            {
                                node1.InnerText = taggedStats;
                            }

                            node1 = null;
                            node1 = xmldoc.SelectSingleNode("/AppConfig/StatTickerFive/enable.disable-channels/statistics.enable-tag-vertical") as XmlElement;

                            if (node1 != null)
                            {
                                node1.InnerText = isVertical.ToString();
                            }

                            node1 = null;
                            node1 = xmldoc.SelectSingleNode("/AppConfig/StatTickerFive/enable.disable-channels/statistics.enable-header") as XmlElement;

                            if (node1 != null)
                            {
                                node1.InnerText = isHeader.ToString();
                            }

                            node1 = null;
                            node1 = xmldoc.SelectSingleNode("/AppConfig/StatTickerFive/DbSetting/Position") as XmlElement;

                            if (node1 != null)
                            {
                                node1.InnerText = left.ToString() + "," + top.ToString();
                            }

                            xmldoc.Save(path);

                            output.MessageCode = "2007";
                            output.Message = "Saved";

                            #endregion
                        }

                        #endregion
                    }
                }

            }
            catch (ProtocolException proException)
            {
                logger.Error("StatisticsBase : SaveAgentsTaggedStats : " + proException.Message);
                output.MessageCode = "2006";
                output.Message = StatisticsSetting.GetInstance().statisticsCollection.ApplicationContainer.ConfigConnection.ToString();

                foreach (KeyValuePair<string, IStatTicker> messageToClient in ToClient)
                {
                    if (messageToClient.Key == "StatTickerFive")
                    {
                        messageToClient.Value.NotifyStatErrorMessage(output);
                    }
                }

                return output;
            }
            catch (Exception generalException)
            {
                logger.Error("StatisticsBase : SaveAgentsTaggedStats : " + generalException.Message);

                output.MessageCode = "2003";
                output.Message = generalException.Message + " " + StatisticsSetting.GetInstance().statisticsCollection.ApplicationContainer.UserPermission.ToString();

                foreach (KeyValuePair<string, IStatTicker> messageToClient in ToClient)
                {
                    if (messageToClient.Key == "StatTickerFive")
                    {
                        messageToClient.Value.NotifyStatErrorMessage(output);
                    }
                }

                return output;
            }
            finally
            {
                GC.Collect();
                logger.Debug("StatisticsBase : SaveAgentTaggedStats : Method Exit");
            }
            return output;
        }

        //#region isAgentConfigFromLocalFileSystem
        //public bool isAgentConfigFromLocalFileSystem()
        //{
        //    return StatisticsSetting.GetInstance().isAgentConfigFromLocal;
        //}
        //#endregion
        //#region SaveAgentConfigFromLocalFileSystem
        //public void SaveAgentConfigFromLocalFileSystem(Dictionary<string,string> dictAgentConfig)
        //{
        //    StatisticsSetting.GetInstance().agentConfigColl = dictAgentConfig;
        //}
        //#endregion
        /// <summary>
        /// Saves the application values.
        /// </summary>
        /// <param name="dictApplicationValues">The dict application values.</param>
        /// <returns></returns>
        public OutputValues SaveApplicationValues(Dictionary<string, Dictionary<string, string>> dictApplicationValues, bool isOverwrite)
        {
            OutputValues output = new OutputValues();
            try
            {
                logger.Debug("StatisticsBase : SaveApplicationValues Method : Entry");

                if (StatisticsSetting.GetInstance().Application != null)
                {
                    #region Save Application Annex value

                    logger.Info("StatisticsBase : SaveApplicationValues Method : Saving Application Annex Values");

                    StatisticsSetting.GetInstance().LstAgentStatistics = new List<string>();
                    StatisticsSetting.GetInstance().LstDNGroupStatistics = new List<string>();
                    StatisticsSetting.GetInstance().LstAgentGroupStatistics = new List<string>();
                    StatisticsSetting.GetInstance().LstACDStatistics = new List<string>();
                    StatisticsSetting.GetInstance().LstVQueueStatistics = new List<string>();

                    if (isOverwrite)
                        StatisticsSetting.GetInstance().Application.UserProperties.Clear();

                    foreach (string keyname in dictApplicationValues.Keys)
                    {
                        KeyValueCollection tempKVC = new KeyValueCollection();

                        foreach (string key in dictApplicationValues[keyname].Keys)
                        {
                            tempKVC.Add(key, dictApplicationValues[keyname][key]);
                        }

                        StatisticsSetting.GetInstance().Application.UserProperties.Add(keyname, tempKVC);
                        StatisticsSetting.GetInstance().Application.Save();

                        if (keyname.StartsWith("agent"))
                        {
                            if (!StatisticsSetting.GetInstance().LstAgentStatistics.Contains(keyname))
                                StatisticsSetting.GetInstance().LstAgentStatistics.Add(keyname);
                        }
                        else if (keyname.StartsWith("group"))
                        {
                            if (!StatisticsSetting.GetInstance().LstAgentGroupStatistics.Contains(keyname))
                                StatisticsSetting.GetInstance().LstAgentGroupStatistics.Add(keyname);
                        }
                        else if (keyname.StartsWith("acd"))
                        {
                            if (!StatisticsSetting.GetInstance().LstACDStatistics.Contains(keyname))
                                StatisticsSetting.GetInstance().LstACDStatistics.Add(keyname);
                        }
                        else if (keyname.StartsWith("dn"))
                        {
                            if (!StatisticsSetting.GetInstance().LstDNGroupStatistics.Contains(keyname))
                                StatisticsSetting.GetInstance().LstDNGroupStatistics.Add(keyname);
                        }
                        else if (keyname.StartsWith("vq"))
                        {
                            if (!StatisticsSetting.GetInstance().LstVQueueStatistics.Contains(keyname))
                                StatisticsSetting.GetInstance().LstVQueueStatistics.Add(keyname);
                        }
                    }

                    #endregion

                    #region Save Application Option values

                    logger.Info("StatisticsBase : SaveApplicationValues Method : Saving Application Options Values");

                    KeyValueCollection appDetails = (KeyValueCollection)StatisticsSetting.GetInstance().Application.Options["agent.ixn.desktop"];

                    string AgentTempValue = string.Empty;
                    string AgentGroupTempValue = string.Empty;
                    string ACDTempValue = string.Empty;
                    string DNTempValue = string.Empty;
                    string VQTempValue = string.Empty;

                    if (appDetails == null)
                    {
                        KeyValueCollection kvps = new KeyValueCollection();

                        kvps.Add("agent-statistics", AgentTempValue);
                        kvps.Add("agent-group-statistics", AgentGroupTempValue);
                        kvps.Add("acd-queue-statistics", ACDTempValue);
                        kvps.Add("dn-group-statistics", DNTempValue);
                        kvps.Add("virtual-queue-statistics", VQTempValue);

                        appDetails = kvps;
                    }
                    else
                    {

                        #region Agent Statistics

                        foreach (string stat in StatisticsSetting.GetInstance().LstAgentStatistics)
                        {
                            if (string.IsNullOrEmpty(AgentTempValue))
                                AgentTempValue = stat;
                            else
                                AgentTempValue = AgentTempValue + "," + stat;
                        }

                        string[] AStatistics = AgentTempValue.Split(',');

                        if (isOverwrite)
                        {
                            #region Overwrite Agent Statistics

                            string tempStat = string.Empty;
                            string kvpname = "agent-statistics";
                            bool issaved = false;
                            int kvpno = 0;

                            List<string> kvpremoved = new List<string>();
                            foreach (string keys in appDetails.Keys)
                            {
                                if (keys.Contains(kvpname))
                                {
                                    kvpremoved.Add(keys);
                                }
                            }

                            foreach (string removedkvp in kvpremoved)
                            {
                                appDetails.Remove(removedkvp);
                            }

                            if (AStatistics.Length > 10)
                            {
                                int remainstats = 10;

                                for (int i = 10; i <= AgentTempValue.Length; )
                                {
                                    tempStat = string.Empty;
                                    for (int j = i - remainstats; j < i; j++)
                                    {
                                        if (tempStat == string.Empty)
                                            tempStat = AStatistics[j];
                                        else
                                            tempStat = tempStat + "," + AStatistics[j];
                                    }

                                    kvpno++;
                                    string tempKVPname = kvpname + "_" + kvpno.ToString();
                                    if (appDetails.ContainsKey(tempKVPname))
                                    {
                                        appDetails.Remove(tempKVPname);
                                        appDetails[tempKVPname] = tempStat;
                                    }
                                    else
                                    {
                                        if (tempStat != "" && tempStat != null && tempStat != string.Empty)
                                            appDetails.Add(tempKVPname, tempStat);
                                    }
                                    if (issaved)
                                    {
                                        i = i + 10;
                                        break;
                                    }

                                    if (i + 10 > AStatistics.Length)
                                    {
                                        remainstats = AStatistics.Length - i;
                                        i = i + (AStatistics.Length - i);
                                        issaved = true;
                                    }
                                    else
                                        i = i + 10;
                                }
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(AStatistics[0].ToString()))
                                {
                                    if (appDetails.ContainsKey("agent-statistics"))
                                    {
                                        appDetails["agent-statistics"] = AgentTempValue;
                                    }
                                    else
                                    {
                                        appDetails.Add("agent-statistics", AgentTempValue);
                                    }
                                }
                            }

                            #endregion
                        }
                        else
                        {
                            #region Append Agent Statistics

                            string kvpname = "agent-statistics";
                            string configuredStats = string.Empty; ;
                            AgentTempValue = string.Empty;

                            foreach (string keys in appDetails.Keys)
                            {
                                if (keys.Contains(kvpname))
                                {
                                    if (string.IsNullOrEmpty(configuredStats))
                                        configuredStats = appDetails[keys].ToString();
                                    else
                                        configuredStats = configuredStats + "," + appDetails[keys].ToString();

                                    appDetails.Remove(keys);
                                }
                            }

                            string[] stats = configuredStats.Split(',');

                            if (!string.IsNullOrEmpty(stats[0]))
                            {
                                foreach (string existingstat in stats)
                                {
                                    if (!StatisticsSetting.GetInstance().LstAgentStatistics.Contains(existingstat))
                                    {
                                        StatisticsSetting.GetInstance().LstAgentStatistics.Add(existingstat);
                                    }
                                }
                            }

                            foreach (string stat in StatisticsSetting.GetInstance().LstAgentStatistics)
                            {
                                if (string.IsNullOrEmpty(AgentTempValue))
                                    AgentTempValue = stat;
                                else
                                    AgentTempValue = AgentTempValue + "," + stat;
                            }

                            int kvpno = 1;

                            foreach (string keys in appDetails.Keys)
                            {
                                if (keys.Contains(kvpname))
                                {
                                    string[] name = keys.Split('_');
                                    if (name.Length > 1)
                                    {
                                        if (kvpno <= Convert.ToInt16(name[1]))
                                        {
                                            kvpno = Convert.ToInt16(name[1]) + 1;
                                        }
                                    }
                                }
                            }

                            AStatistics = AgentTempValue.Split(',');

                            if (AStatistics.Length > 10)
                            {
                                string tempStat = string.Empty;
                                bool issaved = false;

                                int remainstats = 10;

                                for (int i = 10; i <= AgentTempValue.Length; )
                                {
                                    tempStat = string.Empty;
                                    for (int j = i - remainstats; j < i; j++)
                                    {
                                        if (tempStat == string.Empty)
                                            tempStat = AStatistics[j];
                                        else
                                            tempStat = tempStat + "," + AStatistics[j];
                                    }

                                    string tempKVPname = kvpname + "_" + kvpno.ToString();
                                    if (appDetails.ContainsKey(tempKVPname))
                                    {
                                        appDetails.Remove(tempKVPname);
                                        appDetails[tempKVPname] = tempStat;
                                    }
                                    else
                                    {
                                        if (tempStat != "" && tempStat != null && tempStat != string.Empty)
                                            appDetails.Add(tempKVPname, tempStat);
                                    }
                                    if (issaved)
                                    {
                                        i = i + 10;
                                        break;
                                    }

                                    if (i + 10 > AStatistics.Length)
                                    {
                                        remainstats = AStatistics.Length - i;
                                        i = i + (AStatistics.Length - i);
                                        issaved = true;
                                    }
                                    else
                                        i = i + 10;

                                    kvpno++;
                                }
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(AStatistics[0].ToString()))
                                {
                                    if (appDetails.ContainsKey("agent-statistics"))
                                    {
                                        appDetails["agent-statistics"] = AgentTempValue;
                                    }
                                    else
                                    {
                                        appDetails.Add("agent-statistics", AgentTempValue);
                                    }
                                }
                            }

                            #endregion
                        }
                        #endregion

                        #region AgentGroup Statistics

                        foreach (string stat in StatisticsSetting.GetInstance().LstAgentGroupStatistics)
                        {
                            if (string.IsNullOrEmpty(AgentGroupTempValue))
                                AgentGroupTempValue = stat;
                            else
                                AgentGroupTempValue = AgentGroupTempValue + "," + stat;
                        }

                        string[] AGStatistics = AgentGroupTempValue.Split(',');

                        if (isOverwrite)
                        {
                            #region Overwrite Agentgroup Statistics

                            string tempStat = string.Empty;
                            string kvpname = "agent-group-statistics";
                            bool issaved = false;
                            int kvpno = 0;

                            List<string> kvpremoved = new List<string>();
                            foreach (string keys in appDetails.Keys)
                            {
                                if (keys.Contains(kvpname))
                                {
                                    kvpremoved.Add(keys);
                                }
                            }

                            foreach (string removedkvp in kvpremoved)
                            {
                                appDetails.Remove(removedkvp);
                            }

                            if (AGStatistics.Length > 10)
                            {
                                int remainstats = 10;

                                for (int i = 10; i <= AGStatistics.Length; )
                                {
                                    tempStat = string.Empty;
                                    for (int j = i - remainstats; j < i; j++)
                                    {
                                        if (tempStat == string.Empty)
                                            tempStat = AGStatistics[j];
                                        else
                                            tempStat = tempStat + "," + AGStatistics[j];
                                    }

                                    kvpno++;
                                    string tempKVPname = kvpname + "_" + kvpno.ToString();
                                    if (appDetails.ContainsKey(tempKVPname))
                                    {
                                        appDetails.Remove(tempKVPname);
                                        appDetails[tempKVPname] = tempStat;
                                    }
                                    else
                                    {
                                        if (tempStat != "" && tempStat != null && tempStat != string.Empty)
                                            appDetails.Add(tempKVPname, tempStat);
                                    }
                                    if (issaved)
                                    {
                                        i = i + 10;
                                        break;
                                    }

                                    if (i + 10 > AGStatistics.Length)
                                    {
                                        remainstats = AGStatistics.Length - i;
                                        i = i + (AGStatistics.Length - i);
                                        issaved = true;
                                    }
                                    else
                                        i = i + 10;
                                }

                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(AGStatistics[0].ToString()))
                                {
                                    if (appDetails.ContainsKey("agent-group-statistics"))
                                    {
                                        appDetails["agent-group-statistics"] = AgentGroupTempValue;
                                    }
                                    else
                                    {
                                        appDetails.Add("agent-group-statistics", AgentGroupTempValue);
                                    }
                                }
                            }

                            #endregion
                        }
                        else
                        {
                            #region Append Agentgroup Statistics

                            string kvpname = "agent-group-statistics";
                            string configuredStats = string.Empty; ;
                            AgentGroupTempValue = string.Empty;

                            foreach (string keys in appDetails.Keys)
                            {
                                if (keys.Contains(kvpname))
                                {
                                    if (string.IsNullOrEmpty(configuredStats))
                                        configuredStats = appDetails[keys].ToString();
                                    else
                                        configuredStats = configuredStats + "," + appDetails[keys].ToString();

                                    appDetails.Remove(keys);
                                }
                            }

                            string[] stats = configuredStats.Split(',');

                            if (!string.IsNullOrEmpty(stats[0]))
                            {
                                foreach (string existingstat in stats)
                                {
                                    if (!StatisticsSetting.GetInstance().LstAgentGroupStatistics.Contains(existingstat))
                                    {
                                        StatisticsSetting.GetInstance().LstAgentGroupStatistics.Add(existingstat);
                                    }
                                }
                            }

                            foreach (string stat in StatisticsSetting.GetInstance().LstAgentGroupStatistics)
                            {
                                if (string.IsNullOrEmpty(AgentGroupTempValue))
                                    AgentGroupTempValue = stat;
                                else
                                    AgentGroupTempValue = AgentGroupTempValue + "," + stat;
                            }

                            int kvpno = 1;

                            foreach (string keys in appDetails.Keys)
                            {
                                if (keys.Contains(kvpname))
                                {
                                    string[] name = keys.Split('_');
                                    if (name.Length > 1)
                                    {
                                        if (kvpno <= Convert.ToInt16(name[1]))
                                        {
                                            kvpno = Convert.ToInt16(name[1]) + 1;
                                        }
                                    }
                                }
                            }

                            AGStatistics = AgentGroupTempValue.Split(',');

                            if (AGStatistics.Length > 10)
                            {
                                string tempStat = string.Empty;
                                bool issaved = false;

                                int remainstats = 10;

                                for (int i = 10; i <= AgentGroupTempValue.Length; )
                                {
                                    tempStat = string.Empty;
                                    for (int j = i - remainstats; j < i; j++)
                                    {
                                        if (tempStat == string.Empty)
                                            tempStat = AGStatistics[j];
                                        else
                                            tempStat = tempStat + "," + AGStatistics[j];
                                    }

                                    string tempKVPname = kvpname + "_" + kvpno.ToString();
                                    if (appDetails.ContainsKey(tempKVPname))
                                    {
                                        appDetails.Remove(tempKVPname);
                                        appDetails[tempKVPname] = tempStat;
                                    }
                                    else
                                    {
                                        if (tempStat != "" && tempStat != null && tempStat != string.Empty)
                                            appDetails.Add(tempKVPname, tempStat);
                                    }
                                    if (issaved)
                                    {
                                        i = i + 10;
                                        break;
                                    }

                                    if (i + 10 > AGStatistics.Length)
                                    {
                                        remainstats = AGStatistics.Length - i;
                                        i = i + (AGStatistics.Length - i);
                                        issaved = true;
                                    }
                                    else
                                        i = i + 10;

                                    kvpno++;
                                }
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(AGStatistics[0].ToString()))
                                {
                                    if (appDetails.ContainsKey("agent-group-statistics"))
                                    {
                                        appDetails["agent-group-statistics"] = AgentGroupTempValue;
                                    }
                                    else
                                    {
                                        appDetails.Add("agent-group-statistics", AgentGroupTempValue);
                                    }
                                }
                            }

                            #endregion
                        }

                        #endregion

                        #region ACD Statistics

                        foreach (string stat in StatisticsSetting.GetInstance().LstACDStatistics)
                        {
                            if (string.IsNullOrEmpty(ACDTempValue))
                                ACDTempValue = stat;
                            else
                                ACDTempValue = ACDTempValue + "," + stat;
                        }

                        string[] ACDStatistics = ACDTempValue.Split(',');

                        if (isOverwrite)
                        {
                            #region Overwrite ACD Statistics

                            string tempStat = string.Empty;
                            string kvpname = "acd-queue-statistics";
                            bool issaved = false;
                            int kvpno = 0;

                            List<string> kvpremoved = new List<string>();
                            foreach (string keys in appDetails.Keys)
                            {
                                if (keys.Contains(kvpname))
                                {
                                    kvpremoved.Add(keys);
                                }
                            }

                            foreach (string removedkvp in kvpremoved)
                            {
                                appDetails.Remove(removedkvp);
                            }

                            if (ACDStatistics.Length > 10)
                            {
                                int remainstats = 10;

                                for (int i = 10; i <= ACDStatistics.Length; )
                                {
                                    tempStat = string.Empty;
                                    for (int j = i - remainstats; j < i; j++)
                                    {
                                        if (tempStat == string.Empty)
                                            tempStat = ACDStatistics[j];
                                        else
                                            tempStat = tempStat + "," + ACDStatistics[j];
                                    }

                                    kvpno++;
                                    string tempKVPname = kvpname + "_" + kvpno.ToString();
                                    if (appDetails.ContainsKey(tempKVPname))
                                    {
                                        appDetails.Remove(tempKVPname);
                                        appDetails[tempKVPname] = tempStat;
                                    }
                                    else
                                    {
                                        if (tempStat != "" && tempStat != null && tempStat != string.Empty)
                                            appDetails.Add(tempKVPname, tempStat);
                                    }
                                    if (issaved)
                                    {
                                        i = i + 10;
                                        break;
                                    }

                                    if (i + 10 > ACDStatistics.Length)
                                    {
                                        remainstats = ACDStatistics.Length - i;
                                        i = i + (ACDStatistics.Length - i);
                                        issaved = true;
                                    }
                                    else
                                        i = i + 10;
                                }
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(ACDStatistics[0].ToString()))
                                {
                                    if (appDetails.ContainsKey("acd-queue-statistics"))
                                    {
                                        appDetails["acd-queue-statistics"] = ACDTempValue;
                                    }
                                    else
                                    {
                                        appDetails.Add("acd-queue-statistics", ACDTempValue);
                                    }
                                }
                            }

                            #endregion
                        }
                        else
                        {
                            #region Append ACD Statistics

                            string kvpname = "acd-queue-statistics";
                            string configuredStats = string.Empty; ;
                            ACDTempValue = string.Empty;

                            foreach (string keys in appDetails.Keys)
                            {
                                if (keys.Contains(kvpname))
                                {
                                    if (string.IsNullOrEmpty(configuredStats))
                                        configuredStats = appDetails[keys].ToString();
                                    else
                                        configuredStats = configuredStats + "," + appDetails[keys].ToString();

                                    appDetails.Remove(keys);
                                }
                            }

                            string[] stats = configuredStats.Split(',');

                            if (!string.IsNullOrEmpty(stats[0]))
                            {
                                foreach (string existingstat in stats)
                                {
                                    if (!StatisticsSetting.GetInstance().LstACDStatistics.Contains(existingstat))
                                    {
                                        StatisticsSetting.GetInstance().LstACDStatistics.Add(existingstat);
                                    }
                                }
                            }

                            foreach (string stat in StatisticsSetting.GetInstance().LstACDStatistics)
                            {
                                if (string.IsNullOrEmpty(ACDTempValue))
                                    ACDTempValue = stat;
                                else
                                    ACDTempValue = ACDTempValue + "," + stat;
                            }

                            int kvpno = 1;

                            foreach (string keys in appDetails.Keys)
                            {
                                if (keys.Contains(kvpname))
                                {
                                    string[] name = keys.Split('_');
                                    if (name.Length > 1)
                                    {
                                        if (kvpno <= Convert.ToInt16(name[1]))
                                        {
                                            kvpno = Convert.ToInt16(name[1]) + 1;
                                        }
                                    }
                                }
                            }

                            ACDStatistics = ACDTempValue.Split(',');

                            if (ACDStatistics.Length > 10)
                            {
                                string tempStat = string.Empty;
                                bool issaved = false;

                                int remainstats = 10;

                                for (int i = 10; i <= ACDTempValue.Length; )
                                {
                                    tempStat = string.Empty;
                                    for (int j = i - remainstats; j < i; j++)
                                    {
                                        if (tempStat == string.Empty)
                                            tempStat = ACDStatistics[j];
                                        else
                                            tempStat = tempStat + "," + ACDStatistics[j];
                                    }

                                    string tempKVPname = kvpname + "_" + kvpno.ToString();
                                    if (appDetails.ContainsKey(tempKVPname))
                                    {
                                        appDetails.Remove(tempKVPname);
                                        appDetails[tempKVPname] = tempStat;
                                    }
                                    else
                                    {
                                        if (tempStat != "" && tempStat != null && tempStat != string.Empty)
                                            appDetails.Add(tempKVPname, tempStat);
                                    }
                                    if (issaved)
                                    {
                                        i = i + 10;
                                        break;
                                    }

                                    if (i + 10 > ACDStatistics.Length)
                                    {
                                        remainstats = ACDStatistics.Length - i;
                                        i = i + (ACDStatistics.Length - i);
                                        issaved = true;
                                    }
                                    else
                                        i = i + 10;

                                    kvpno++;
                                }

                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(ACDStatistics[0].ToString()))
                                {
                                    if (appDetails.ContainsKey("acd-queue-statistics"))
                                    {
                                        appDetails["acd-queue-statistics"] = ACDTempValue;
                                    }
                                    else
                                    {
                                        appDetails.Add("acd-queue-statistics", ACDTempValue);
                                    }
                                }
                            }

                            #endregion
                        }

                        #endregion

                        #region DNGroup Statistics

                        foreach (string stat in StatisticsSetting.GetInstance().LstDNGroupStatistics)
                        {
                            if (string.IsNullOrEmpty(DNTempValue))
                                DNTempValue = stat;
                            else
                                DNTempValue = DNTempValue + "," + stat;
                        }
                        string[] DNStatistics = DNTempValue.Split(',');

                        if (isOverwrite)
                        {
                            #region Overwrite DNGroup Statistics

                            string tempStat = string.Empty;
                            string kvpname = "dn-group-statistics";
                            bool issaved = false;
                            int kvpno = 0;

                            List<string> kvpremoved = new List<string>();
                            foreach (string keys in appDetails.Keys)
                            {
                                if (keys.Contains(kvpname))
                                {
                                    kvpremoved.Add(keys);
                                }
                            }

                            foreach (string removedkvp in kvpremoved)
                            {
                                appDetails.Remove(removedkvp);
                            }

                            if (DNStatistics.Length > 7)
                            {
                                int remainstats = 10;

                                for (int i = 10; i <= DNStatistics.Length; )
                                {
                                    tempStat = string.Empty;
                                    for (int j = i - remainstats; j < i; j++)
                                    {
                                        if (tempStat == string.Empty)
                                            tempStat = DNStatistics[j];
                                        else
                                            tempStat = tempStat + "," + DNStatistics[j];
                                    }

                                    kvpno++;
                                    string tempKVPname = kvpname + "_" + kvpno.ToString();
                                    if (appDetails.ContainsKey(tempKVPname))
                                    {
                                        appDetails.Remove(tempKVPname);
                                        appDetails[tempKVPname] = tempStat;
                                    }
                                    else
                                    {
                                        if (tempStat != "" && tempStat != null && tempStat != string.Empty)
                                            appDetails.Add(tempKVPname, tempStat);
                                    }
                                    if (issaved)
                                    {
                                        i = i + 10;
                                        break;
                                    }

                                    if (i + 10 > DNStatistics.Length)
                                    {
                                        remainstats = DNStatistics.Length - i;
                                        i = i + (DNStatistics.Length - i);
                                        issaved = true;
                                    }
                                    else
                                        i = i + 10;
                                }
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(DNStatistics[0].ToString()))
                                {
                                    if (appDetails.ContainsKey("dn-group-statistics"))
                                    {
                                        appDetails["dn-group-statistics"] = DNTempValue;
                                    }
                                    else
                                    {
                                        appDetails.Add("dn-group-statistics", DNTempValue);
                                    }
                                }
                            }

                            #endregion
                        }
                        else
                        {
                            #region Append DNGroup Statistics

                            string kvpname = "dn-group-statistics";
                            string configuredStats = string.Empty; ;
                            DNTempValue = string.Empty;

                            foreach (string keys in appDetails.Keys)
                            {
                                if (keys.Contains(kvpname))
                                {
                                    if (string.IsNullOrEmpty(configuredStats))
                                        configuredStats = appDetails[keys].ToString();
                                    else
                                        configuredStats = configuredStats + "," + appDetails[keys].ToString();

                                    appDetails.Remove(keys);
                                }
                            }

                            string[] stats = configuredStats.Split(',');

                            if (!string.IsNullOrEmpty(stats[0]))
                            {
                                foreach (string existingstat in stats)
                                {
                                    if (!StatisticsSetting.GetInstance().LstDNGroupStatistics.Contains(existingstat))
                                    {
                                        StatisticsSetting.GetInstance().LstDNGroupStatistics.Add(existingstat);
                                    }
                                }
                            }

                            foreach (string stat in StatisticsSetting.GetInstance().LstDNGroupStatistics)
                            {
                                if (string.IsNullOrEmpty(DNTempValue))
                                    DNTempValue = stat;
                                else
                                    DNTempValue = DNTempValue + "," + stat;
                            }

                            int kvpno = 1;

                            foreach (string keys in appDetails.Keys)
                            {
                                if (keys.Contains(kvpname))
                                {
                                    string[] name = keys.Split('_');

                                    if (name.Length > 1)
                                    {
                                        if (kvpno <= Convert.ToInt16(name[1]))
                                        {
                                            kvpno = Convert.ToInt16(name[1]) + 1;
                                        }
                                    }
                                }
                            }

                            DNStatistics = DNTempValue.Split(',');

                            if (DNStatistics.Length > 10)
                            {
                                string tempStat = string.Empty;
                                bool issaved = false;

                                int remainstats = 10;

                                for (int i = 10; i <= DNTempValue.Length; )
                                {
                                    tempStat = string.Empty;
                                    for (int j = i - remainstats; j < i; j++)
                                    {
                                        if (tempStat == string.Empty)
                                            tempStat = DNStatistics[j];
                                        else
                                            tempStat = tempStat + "," + DNStatistics[j];
                                    }

                                    string tempKVPname = kvpname + "_" + kvpno.ToString();
                                    if (appDetails.ContainsKey(tempKVPname))
                                    {
                                        appDetails.Remove(tempKVPname);
                                        appDetails[tempKVPname] = tempStat;
                                    }
                                    else
                                    {
                                        if (tempStat != "" && tempStat != null && tempStat != string.Empty)
                                            appDetails.Add(tempKVPname, tempStat);
                                    }
                                    if (issaved)
                                    {
                                        i = i + 10;
                                        break;
                                    }

                                    if (i + 10 > DNStatistics.Length)
                                    {
                                        remainstats = DNStatistics.Length - i;
                                        i = i + (DNStatistics.Length - i);
                                        issaved = true;
                                    }
                                    else
                                        i = i + 10;

                                    kvpno++;
                                }
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(DNStatistics[0].ToString()))
                                {
                                    if (appDetails.ContainsKey("dn-group-statistics"))
                                    {
                                        appDetails["dn-group-statistics"] = DNTempValue;
                                    }
                                    else
                                    {
                                        appDetails.Add("dn-group-statistics", DNTempValue);
                                    }
                                }
                            }
                            #endregion
                        }

                        #endregion

                        #region VQ Statistics

                        foreach (string stat in StatisticsSetting.GetInstance().LstVQueueStatistics)
                        {
                            if (string.IsNullOrEmpty(VQTempValue))
                                VQTempValue = stat;
                            else
                                VQTempValue = VQTempValue + "," + stat;
                        }
                        string[] VQStatistics = VQTempValue.Split(',');

                        if (isOverwrite)
                        {
                            #region Overwrite VQ Statistics

                            string tempStat = string.Empty;
                            string kvpname = "virtual-queue-statistics";
                            bool issaved = false;
                            int kvpno = 0;

                            List<string> kvpremoved = new List<string>();
                            foreach (string keys in appDetails.Keys)
                            {
                                if (keys.Contains(kvpname))
                                {
                                    kvpremoved.Add(keys);
                                }
                            }

                            foreach (string removedkvp in kvpremoved)
                            {
                                appDetails.Remove(removedkvp);
                            }

                            if (VQStatistics.Length > 7)
                            {
                                int remainstats = 10;

                                for (int i = 10; i <= VQStatistics.Length; )
                                {
                                    tempStat = string.Empty;
                                    for (int j = i - remainstats; j < i; j++)
                                    {
                                        if (tempStat == string.Empty)
                                            tempStat = VQStatistics[j];
                                        else
                                            tempStat = tempStat + "," + VQStatistics[j];
                                    }

                                    kvpno++;
                                    string tempKVPname = kvpname + "_" + kvpno.ToString();
                                    if (appDetails.ContainsKey(tempKVPname))
                                    {
                                        appDetails.Remove(tempKVPname);
                                        appDetails[tempKVPname] = tempStat;
                                    }
                                    else
                                    {
                                        if (tempStat != "" && tempStat != null && tempStat != string.Empty)
                                            appDetails.Add(tempKVPname, tempStat);
                                    }
                                    if (issaved)
                                    {
                                        i = i + 10;
                                        break;
                                    }

                                    if (i + 10 > VQStatistics.Length)
                                    {
                                        remainstats = VQStatistics.Length - i;
                                        i = i + (VQStatistics.Length - i);
                                        issaved = true;
                                    }
                                    else
                                        i = i + 10;
                                }
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(VQStatistics[0].ToString()))
                                {
                                    if (appDetails.ContainsKey("virtual-queue-statistics"))
                                    {
                                        appDetails["virtual-queue-statistics"] = VQTempValue;
                                    }
                                    else
                                    {
                                        appDetails.Add("virtual-queue-statistics", VQTempValue);
                                    }
                                }
                            }

                            #endregion
                        }
                        else
                        {
                            #region Append VQ Statistics

                            string kvpname = "virtual-queue-statistics";
                            string configuredStats = string.Empty; ;
                            VQTempValue = string.Empty;

                            foreach (string keys in appDetails.Keys)
                            {
                                if (keys.Contains(kvpname))
                                {
                                    if (string.IsNullOrEmpty(configuredStats))
                                        configuredStats = appDetails[keys].ToString();
                                    else
                                        configuredStats = configuredStats + "," + appDetails[keys].ToString();
                                }
                            }

                            string[] stats = configuredStats.Split(',');

                            if (!string.IsNullOrEmpty(stats[0]))
                            {
                                foreach (string existingstat in stats)
                                {
                                    if (!StatisticsSetting.GetInstance().LstVQueueStatistics.Contains(existingstat))
                                    {
                                        StatisticsSetting.GetInstance().LstVQueueStatistics.Add(existingstat);
                                    }
                                }
                            }

                            foreach (string stat in StatisticsSetting.GetInstance().LstVQueueStatistics)
                            {
                                if (string.IsNullOrEmpty(ACDTempValue))
                                    VQTempValue = stat;
                                else
                                    VQTempValue = VQTempValue + "," + stat;
                            }

                            int kvpno = 1;

                            foreach (string keys in appDetails.Keys)
                            {
                                if (keys.Contains(kvpname))
                                {
                                    string[] name = keys.Split('_');

                                    if (name.Length > 1)
                                    {
                                        if (kvpno <= Convert.ToInt16(name[1]))
                                        {
                                            kvpno = Convert.ToInt16(name[1]) + 1;
                                        }
                                    }
                                }
                            }

                            VQStatistics = VQTempValue.Split(',');

                            if (VQStatistics.Length > 10)
                            {
                                string tempStat = string.Empty;
                                bool issaved = false;

                                int remainstats = 10;

                                for (int i = 10; i <= DNTempValue.Length; )
                                {
                                    tempStat = string.Empty;
                                    for (int j = i - remainstats; j < i; j++)
                                    {
                                        if (tempStat == string.Empty)
                                            tempStat = VQStatistics[j];
                                        else
                                            tempStat = tempStat + "," + VQStatistics[j];
                                    }

                                    string tempKVPname = kvpname + "_" + kvpno.ToString();
                                    if (appDetails.ContainsKey(tempKVPname))
                                    {
                                        appDetails.Remove(tempKVPname);
                                        appDetails[tempKVPname] = tempStat;
                                    }
                                    else
                                    {
                                        if (tempStat != "" && tempStat != null && tempStat != string.Empty)
                                            appDetails.Add(tempKVPname, tempStat);
                                    }
                                    if (issaved)
                                    {
                                        i = i + 10;
                                        break;
                                    }

                                    if (i + 10 > VQStatistics.Length)
                                    {
                                        remainstats = VQStatistics.Length - i;
                                        i = i + (VQStatistics.Length - i);
                                        issaved = true;
                                    }
                                    else
                                        i = i + 10;

                                    kvpno++;
                                }
                            }
                            else
                            {

                                if (!string.IsNullOrEmpty(VQStatistics[0].ToString()))
                                {
                                    if (appDetails.ContainsKey("virtual-queue-statistics"))
                                    {
                                        appDetails["virtual-queue-statistics"] = VQTempValue;
                                    }
                                    else
                                    {
                                        appDetails.Add("virtual-queue-statistics", VQTempValue);
                                    }
                                }
                            }

                            #endregion
                        }

                        #endregion

                        #region Old Code

                        //if (!isOverwrite)                        //{
                        //    logger.Info("StatisticsBase : SaveApplicationValues Method : Appending Application Options Values");

                        //    if (appDetails.ContainsKey("agent-statistics") && (appDetails["agent-statistics"] != string.Empty || appDetails["agent-statistics"] != " "))
                        //    {
                        //        string[] stats = appDetails["agent-statistics"].ToString().Split(',');

                        //        foreach (string existingstat in stats)
                        //        {
                        //            if (StatisticsSetting.GetInstance().LstAgentStatistics.Contains(existingstat))
                        //            {
                        //                StatisticsSetting.GetInstance().LstAgentStatistics.Remove(existingstat);
                        //            }
                        //        }

                        //        if (StatisticsSetting.GetInstance().LstAgentStatistics.Count != 0)
                        //        {
                        //            foreach (string finalStat in StatisticsSetting.GetInstance().LstAgentStatistics)
                        //            {
                        //                appDetails["agent-statistics"] = appDetails["agent-statistics"] + "," + finalStat;
                        //            }
                        //        }

                        //     }
                        //    if (appDetails.ContainsKey("agent-group-statistics") && (appDetails["agent-group-statistics"] != string.Empty || appDetails["agent-group-statistics"] != " "))
                        //    {

                        //        string[] stats = appDetails["agent-group-statistics"].ToString().Split(',');

                        //        foreach (string existingstat in stats)
                        //        {
                        //            if (StatisticsSetting.GetInstance().LstAgentGroupStatistics.Contains(existingstat))
                        //            {
                        //                StatisticsSetting.GetInstance().LstAgentGroupStatistics.Remove(existingstat);
                        //            }
                        //        }

                        //        if (StatisticsSetting.GetInstance().LstAgentGroupStatistics.Count != 0)
                        //        {
                        //            foreach (string finalStat in StatisticsSetting.GetInstance().LstAgentGroupStatistics)
                        //            {
                        //                appDetails["agent-group-statistics"] = appDetails["agent-group-statistics"] + "," + finalStat;
                        //            }
                        //        }

                        //      }
                        //    if (appDetails.ContainsKey("acd-queue-statistics") && (appDetails["acd-queue-statistics"] != string.Empty || appDetails["acd-queue-statistics"] != " "))
                        //    {
                        //        string[] stats = appDetails["acd-queue-statistics"].ToString().Split(',');

                        //        foreach (string existingstat in stats)
                        //        {
                        //            if (StatisticsSetting.GetInstance().LstACDStatistics.Contains(existingstat))
                        //            {
                        //                StatisticsSetting.GetInstance().LstACDStatistics.Remove(existingstat);
                        //            }
                        //        }

                        //        if (StatisticsSetting.GetInstance().LstACDStatistics.Count != 0)
                        //        {
                        //            foreach(string finalStat in StatisticsSetting.GetInstance().LstACDStatistics)
                        //            {
                        //                appDetails["acd-queue-statistics"] = appDetails["acd-queue-statistics"] + "," + finalStat;
                        //            }
                        //        }

                        //    }
                        //    if (appDetails.ContainsKey("dn-group-statistics") && (appDetails["dn-group-statistics"] != string.Empty || appDetails["dn-group-statistics"] != " "))
                        //    {

                        //        string[] stats = appDetails["dn-group-statistics"].ToString().Split(',');

                        //        foreach (string existingstat in stats)
                        //        {
                        //            if (StatisticsSetting.GetInstance().LstDNGroupStatistics.Contains(existingstat))
                        //            {
                        //                StatisticsSetting.GetInstance().LstDNGroupStatistics.Remove(existingstat);
                        //            }
                        //        }

                        //        if (StatisticsSetting.GetInstance().LstDNGroupStatistics.Count != 0)
                        //        {
                        //            foreach (string finalStat in StatisticsSetting.GetInstance().LstDNGroupStatistics)
                        //            {
                        //                appDetails["dn-group-statistics"] = appDetails["dn-group-statistics"] + "," + finalStat;
                        //            }
                        //        }

                        //    }
                        //    if (appDetails.ContainsKey("virtual-queue-statistics") && (appDetails["virtual-queue-statistics"] != string.Empty || appDetails["virtual-queue-statistics"] != " "))
                        //    {

                        //        string[] stats = appDetails["virtual-queue-statistics"].ToString().Split(',');

                        //        foreach (string existingstat in stats)
                        //        {
                        //            if (StatisticsSetting.GetInstance().LstVQueueStatistics.Contains(existingstat))
                        //            {
                        //                StatisticsSetting.GetInstance().LstVQueueStatistics.Remove(existingstat);
                        //            }
                        //        }

                        //        if (StatisticsSetting.GetInstance().LstVQueueStatistics.Count != 0)
                        //        {
                        //            foreach (string finalStat in StatisticsSetting.GetInstance().LstVQueueStatistics)
                        //            {
                        //                appDetails["virtual-queue-statistics"] = appDetails["virtual-queue-statistics"] + "," + finalStat;
                        //            }
                        //        }
                        //    }
                        //}
                        #endregion
                    }

                    #endregion

                    StatisticsSetting.GetInstance().Application.Options["agent.ixn.desktop"] = appDetails;

                    StatisticsSetting.GetInstance().Application.Save();
                }
            }
            catch (Exception GeneralException)
            {
                logger.Error("StatisticsBase : SaveApplicationValues Method : " + GeneralException.Message);

            }
            logger.Debug("StatisticsBase : SaveApplicationValues Method : Exit");
            return output;
        }

        public bool SaveLevelStatsObjects(Dictionary<string, Dictionary<string, Dictionary<string, List<string>>>> dictStatsValues, string objectId)
        {
            bool isObjectsSaved = false;
            try
            {
                logger.Debug("StatisticsBase : SaveLevelStatsObjects Method : Entry");

                if (dictStatsValues.Count != 0)
                {
                    foreach (string UID in dictStatsValues.Keys)
                    {
                        if (string.Compare(UID, objectId, true) == 0)
                        {
                            Dictionary<string, List<string>> tempStatisticsValues = new Dictionary<string, List<string>>();
                            Dictionary<string, List<string>> tempObjectsValues = new Dictionary<string, List<string>>();
                            tempStatisticsValues = dictStatsValues[UID]["Statistics"];
                            tempObjectsValues = dictStatsValues[UID]["Objects"];

                            if (StatisticsSetting.GetInstance().statisticsCollection.ApplicationContainer.EnableMyQueueConfig)
                            {
                                objectId = StatisticsSetting.GetInstance().PersonDetails.EmployeeID;
                                StatisticsSetting.GetInstance().AppName = StatisticsSetting.GetInstance().PersonDetails.EmployeeID;
                                if (!StatisticsSetting.GetInstance().LstAgents.Contains(objectId))
                                    StatisticsSetting.GetInstance().LstAgents.Add(objectId);
                            }

                            if (string.Compare(UID, StatisticsSetting.GetInstance().AppName, true) == 0)
                            {
                                if (StatisticsSetting.GetInstance().Application == null)
                                {
                                    CfgApplicationQuery ApplicationQuery = new CfgApplicationQuery();
                                    ApplicationQuery.Name = UID;
                                    StatisticsSetting.GetInstance().Application = StatisticsSetting.GetInstance().confObject.RetrieveObject<CfgApplication>(ApplicationQuery);
                                }
                                if (tempStatisticsValues.Count != 0)
                                    StatisticsSetting.GetInstance().Application.Options["agent.ixn.desktop"] = SaveStats(tempStatisticsValues, (KeyValueCollection)StatisticsSetting.GetInstance().Application.Options["agent.ixn.desktop"]);

                                if (tempObjectsValues.Count != 0)
                                    StatisticsSetting.GetInstance().Application.Options["agent.ixn.desktop"] = SaveObjects(tempObjectsValues, (KeyValueCollection)StatisticsSetting.GetInstance().Application.Options["agent.ixn.desktop"]);
                                StatisticsSetting.GetInstance().Application.Save();
                                isObjectsSaved = true;
                            }
                            else
                            {
                                if (StatisticsSetting.GetInstance().LstAgentGroups.Contains(UID))
                                {
                                    CfgAgentGroupQuery agentGroupQuery = new CfgAgentGroupQuery();
                                    agentGroupQuery.Name = UID;
                                    agentGroupQuery["objecttype"] = CfgObjectType.CFGAgentGroup;
                                    CfgAgentGroup agentGroup = StatisticsSetting.GetInstance().confObject.RetrieveObject<CfgAgentGroup>(agentGroupQuery);

                                    if (tempStatisticsValues.Count != 0)
                                        agentGroup.GroupInfo.UserProperties["agent.ixn.desktop"] = SaveStats(tempStatisticsValues, (KeyValueCollection)agentGroup.GroupInfo.UserProperties["agent.ixn.desktop"]);

                                    if (tempObjectsValues.Count != 0)
                                        agentGroup.GroupInfo.UserProperties["agent.ixn.desktop"] = SaveObjects(tempObjectsValues, (KeyValueCollection)agentGroup.GroupInfo.UserProperties["agent.ixn.desktop"]);
                                    agentGroup.Save();
                                    isObjectsSaved = true;
                                }
                                //else if (StatisticsSetting.GetInstance().LstAgents.Contains(UID))
                                else if (StatisticsSetting.GetInstance().LstAgents.Contains(objectId))
                                {
                                    CfgPersonQuery agentQuery = new CfgPersonQuery();
                                    //agentQuery.EmployeeId = UID;
                                    agentQuery.EmployeeId = objectId;
                                    CfgPerson agent = StatisticsSetting.GetInstance().confObject.RetrieveObject<CfgPerson>(agentQuery);

                                    if (tempStatisticsValues.Count != 0)
                                        agent.UserProperties["agent.ixn.desktop"] = SaveStats(tempStatisticsValues, (KeyValueCollection)agent.UserProperties["agent.ixn.desktop"]);

                                    if (tempObjectsValues.Count != 0)
                                        agent.UserProperties["agent.ixn.desktop"] = SaveObjects(tempObjectsValues, (KeyValueCollection)agent.UserProperties["agent.ixn.desktop"]);
                                    agent.Save();
                                    isObjectsSaved = true;
                                }
                            }
                        }
                    }
                }
                else
                {
                    logger.Info("StatisticsBase : SaveLevelStatsObjects Method : Saving Values null");

                    if (StatisticsSetting.GetInstance().statisticsCollection.ApplicationContainer.EnableMyQueueConfig)
                    {
                        objectId = StatisticsSetting.GetInstance().PersonDetails.EmployeeID;
                        StatisticsSetting.GetInstance().AppName = StatisticsSetting.GetInstance().PersonDetails.EmployeeID;
                        if (!StatisticsSetting.GetInstance().LstAgents.Contains(objectId))
                            StatisticsSetting.GetInstance().LstAgents.Add(objectId);
                    }

                    if (StatisticsSetting.GetInstance().LstAgents.Contains(objectId))
                    {
                        Dictionary<string, List<string>> tempObjectsValues = new Dictionary<string, List<string>>();
                        CfgPersonQuery agentQuery = new CfgPersonQuery();
                        //agentQuery.EmployeeId = UID;
                        agentQuery.EmployeeId = objectId;
                        CfgPerson agent = StatisticsSetting.GetInstance().confObject.RetrieveObject<CfgPerson>(agentQuery);

                        agent.UserProperties["agent.ixn.desktop"] = SaveObjects(tempObjectsValues, (KeyValueCollection)agent.UserProperties["agent.ixn.desktop"]);
                        agent.Save();
                        isObjectsSaved = true;
                    }
                }
            }
            catch (Exception GeneralException)
            {
                logger.Debug("StatisticsBase : SaveLevelStatsObjects Method : " + GeneralException.Message);

            }
            logger.Debug("StatisticsBase : SaveLevelStatsObjects Method : Exit");

            return isObjectsSaved;
        }

        public void SaveObjectValues(Dictionary<string, List<string>> DictObjects, bool isOverwrite)
        {
            logger.Debug("StatisticsBase : SaveObjectValues Method : Entry");
            List<string> LstAgentId = new List<string>();
            bool isExceedLimit = false;
            string tempObjs = string.Empty;

            try
            {
                if (StatisticsSetting.GetInstance().Application != null)
                {
                    KeyValueCollection appDetails = (KeyValueCollection)StatisticsSetting.GetInstance().Application.Options["agent.ixn.desktop"];

                    if (appDetails != null)
                    {
                        if (DictObjects.ContainsKey(StatisticsEnum.ObjectType.Agent.ToString()))
                        {
                            if (DictObjects[StatisticsEnum.ObjectType.Agent.ToString()] != null && DictObjects[StatisticsEnum.ObjectType.Agent.ToString()].Count != 0)
                            {
                                #region Agents

                                string AgentIds = string.Empty;
                                List<string> lstTempAgents = new List<string>();
                                isExceedLimit = DictObjects[StatisticsEnum.ObjectType.Agent.ToString()].Count > 15 ? true : false;
                                string kvpname = "statistics.objects-agents";
                                bool issaved = false;
                                int kvpno = 0;

                                foreach (string Agents in DictObjects[StatisticsEnum.ObjectType.Agent.ToString()])
                                {
                                    string[] AgentNames = Agents.Split(',');

                                    CfgPersonQuery _personQuery = new CfgPersonQuery();
                                    _personQuery.FirstName = AgentNames[0].Trim();
                                    _personQuery.LastName = AgentNames[1].Trim();
                                    _personQuery.TenantDbid = StatisticsSetting.GetInstance().CFGTenantDBID;

                                    CfgPerson PersonDetails = StatisticsSetting.GetInstance().confObject.RetrieveObject<CfgPerson>(_personQuery);

                                    if (PersonDetails == null)
                                    {
                                        continue;
                                    }

                                    lstTempAgents.Add(PersonDetails.EmployeeID);

                                    if (!isExceedLimit)
                                    {
                                        if (string.IsNullOrEmpty(AgentIds))
                                            AgentIds = PersonDetails.EmployeeID;
                                        else
                                            AgentIds = AgentIds + "," + PersonDetails.EmployeeID;
                                    }
                                }

                                if (isExceedLimit)
                                {
                                    int remainObjects = 10;

                                    for (int i = 10; i <= lstTempAgents.Count; )
                                    {
                                        tempObjs = string.Empty;
                                        for (int j = i - remainObjects; j < i; j++)
                                        {
                                            if (tempObjs == string.Empty)
                                                tempObjs = lstTempAgents[j];
                                            else
                                                tempObjs = tempObjs + "," + lstTempAgents[j];
                                        }

                                        kvpno++;
                                        string tempKVPname = kvpname + "_" + kvpno.ToString();
                                        if (appDetails.ContainsKey(tempKVPname))
                                        {
                                            appDetails.Remove(tempKVPname);
                                            appDetails[tempKVPname] = tempObjs;
                                        }
                                        else
                                        {
                                            if (tempObjs != "" && tempObjs != null && tempObjs != string.Empty)
                                                appDetails.Add(tempKVPname, tempObjs);
                                        }
                                        if (issaved)
                                        {
                                            i = i + 10;
                                            break;
                                        }

                                        if (i + 10 > lstTempAgents.Count)
                                        {
                                            remainObjects = lstTempAgents.Count - i;
                                            i = i + (lstTempAgents.Count - i);
                                            issaved = true;
                                        }
                                        else
                                            i = i + 10;
                                    }
                                }
                                else
                                {
                                    if (appDetails.ContainsKey("statistics.objects-agents"))
                                        appDetails.Remove("statistics.objects-agents");

                                    appDetails.Add("statistics.objects-agents", AgentIds);
                                }

                                #endregion
                            }
                        }

                        if (DictObjects.ContainsKey(StatisticsEnum.ObjectType.AgentGroup.ToString()))
                        {
                            if (DictObjects[StatisticsEnum.ObjectType.AgentGroup.ToString()] != null && DictObjects[StatisticsEnum.ObjectType.AgentGroup.ToString()].Count != 0)
                            {
                                #region AgentGroup

                                string AgentGroups = string.Empty;
                                isExceedLimit = DictObjects[StatisticsEnum.ObjectType.AgentGroup.ToString()].Count > 15 ? true : false;
                                string kvpname = "statistics.objects-agent-groups";
                                bool issaved = false;
                                int kvpno = 0;

                                foreach (string AgentGroup in DictObjects[StatisticsEnum.ObjectType.AgentGroup.ToString()])
                                {

                                    if (!isExceedLimit)
                                    {
                                        if (string.IsNullOrEmpty(AgentGroups))
                                            AgentGroups = AgentGroup;
                                        else
                                            AgentGroups = AgentGroups + "," + AgentGroup;
                                    }
                                }

                                if (isExceedLimit)
                                {
                                    int remainObjects = 10;

                                    for (int i = 10; i <= DictObjects[StatisticsEnum.ObjectType.AgentGroup.ToString()].Count; )
                                    {
                                        tempObjs = string.Empty;
                                        for (int j = i - remainObjects; j < i; j++)
                                        {
                                            if (tempObjs == string.Empty)
                                                tempObjs = DictObjects[StatisticsEnum.ObjectType.AgentGroup.ToString()][j];
                                            else
                                                tempObjs = tempObjs + "," + DictObjects[StatisticsEnum.ObjectType.AgentGroup.ToString()][j];
                                        }

                                        kvpno++;
                                        string tempKVPname = kvpname + "_" + kvpno.ToString();
                                        if (appDetails.ContainsKey(tempKVPname))
                                        {
                                            appDetails.Remove(tempKVPname);
                                            appDetails[tempKVPname] = tempObjs;
                                        }
                                        else
                                        {
                                            if (tempObjs != "" && tempObjs != null && tempObjs != string.Empty)
                                                appDetails.Add(tempKVPname, tempObjs);
                                        }
                                        if (issaved)
                                        {
                                            i = i + 10;
                                            break;
                                        }

                                        if (i + 10 > DictObjects[StatisticsEnum.ObjectType.AgentGroup.ToString()].Count)
                                        {
                                            remainObjects = DictObjects[StatisticsEnum.ObjectType.AgentGroup.ToString()].Count - i;
                                            i = i + (DictObjects[StatisticsEnum.ObjectType.AgentGroup.ToString()].Count - i);
                                            issaved = true;
                                        }
                                        else
                                            i = i + 10;
                                    }
                                }
                                else
                                {
                                    if (appDetails.ContainsKey("statistics.objects-agent-groups"))
                                        appDetails.Remove("statistics.objects-agent-groups");

                                    appDetails.Add("statistics.objects-agent-groups", AgentGroups);
                                }

                                #endregion
                            }
                        }

                        if (DictObjects.ContainsKey(StatisticsEnum.ObjectType.ACDQueue.ToString()))
                        {
                            if (DictObjects[StatisticsEnum.ObjectType.ACDQueue.ToString()] != null && DictObjects[StatisticsEnum.ObjectType.ACDQueue.ToString()].Count != 0)
                            {
                                #region ACDQueue

                                string ACDQueues = string.Empty;
                                isExceedLimit = DictObjects[StatisticsEnum.ObjectType.ACDQueue.ToString()].Count > 15 ? true : false;
                                string kvpname = "statistics.objects-acd-queues";
                                bool issaved = false;
                                int kvpno = 0;

                                foreach (string Queue in DictObjects[StatisticsEnum.ObjectType.ACDQueue.ToString()])
                                {

                                    if (!isExceedLimit)
                                    {
                                        if (string.IsNullOrEmpty(ACDQueues))
                                            ACDQueues = Queue;
                                        else
                                            ACDQueues = ACDQueues + "," + Queue;
                                    }
                                }

                                if (isExceedLimit)
                                {
                                    int remainObjects = 10;

                                    for (int i = 10; i <= DictObjects[StatisticsEnum.ObjectType.ACDQueue.ToString()].Count; )
                                    {
                                        tempObjs = string.Empty;
                                        for (int j = i - remainObjects; j < i; j++)
                                        {
                                            if (tempObjs == string.Empty)
                                                tempObjs = DictObjects[StatisticsEnum.ObjectType.ACDQueue.ToString()][j];
                                            else
                                                tempObjs = tempObjs + "," + DictObjects[StatisticsEnum.ObjectType.ACDQueue.ToString()][j];
                                        }

                                        kvpno++;
                                        string tempKVPname = kvpname + "_" + kvpno.ToString();
                                        if (appDetails.ContainsKey(tempKVPname))
                                        {
                                            appDetails.Remove(tempKVPname);
                                            appDetails[tempKVPname] = tempObjs;
                                        }
                                        else
                                        {
                                            if (tempObjs != "" && tempObjs != null && tempObjs != string.Empty)
                                                appDetails.Add(tempKVPname, tempObjs);
                                        }
                                        if (issaved)
                                        {
                                            i = i + 10;
                                            break;
                                        }

                                        if (i + 10 > DictObjects[StatisticsEnum.ObjectType.ACDQueue.ToString()].Count)
                                        {
                                            remainObjects = DictObjects[StatisticsEnum.ObjectType.ACDQueue.ToString()].Count - i;
                                            i = i + (DictObjects[StatisticsEnum.ObjectType.ACDQueue.ToString()].Count - i);
                                            issaved = true;
                                        }
                                        else
                                            i = i + 10;
                                    }
                                }
                                else
                                {
                                    if (appDetails.ContainsKey("statistics.objects-acd-queues"))
                                        appDetails.Remove("statistics.objects-acd-queues");

                                    appDetails.Add("statistics.objects-acd-queues", ACDQueues);
                                }

                                #endregion
                            }
                        }

                        if (DictObjects.ContainsKey(StatisticsEnum.ObjectType.DNGroup.ToString()))
                        {
                            if (DictObjects[StatisticsEnum.ObjectType.DNGroup.ToString()] != null && DictObjects[StatisticsEnum.ObjectType.DNGroup.ToString()].Count != 0)
                            {
                                #region DNGroup

                                string DNGroups = string.Empty;
                                isExceedLimit = DictObjects[StatisticsEnum.ObjectType.DNGroup.ToString()].Count > 15 ? true : false;
                                string kvpname = "statistics.objects-dn-groups";
                                bool issaved = false;
                                int kvpno = 0;

                                foreach (string DNGroup in DictObjects[StatisticsEnum.ObjectType.DNGroup.ToString()])
                                {

                                    if (!isExceedLimit)
                                    {
                                        if (string.IsNullOrEmpty(DNGroups))
                                            DNGroups = DNGroup;
                                        else
                                            DNGroups = DNGroups + "," + DNGroup;
                                    }
                                }

                                if (isExceedLimit)
                                {
                                    int remainObjects = 10;

                                    for (int i = 10; i <= DictObjects[StatisticsEnum.ObjectType.DNGroup.ToString()].Count; )
                                    {
                                        tempObjs = string.Empty;
                                        for (int j = i - remainObjects; j < i; j++)
                                        {
                                            if (tempObjs == string.Empty)
                                                tempObjs = DictObjects[StatisticsEnum.ObjectType.DNGroup.ToString()][j];
                                            else
                                                tempObjs = tempObjs + "," + DictObjects[StatisticsEnum.ObjectType.DNGroup.ToString()][j];
                                        }

                                        kvpno++;
                                        string tempKVPname = kvpname + "_" + kvpno.ToString();
                                        if (appDetails.ContainsKey(tempKVPname))
                                        {
                                            appDetails.Remove(tempKVPname);
                                            appDetails[tempKVPname] = tempObjs;
                                        }
                                        else
                                        {
                                            if (tempObjs != "" && tempObjs != null && tempObjs != string.Empty)
                                                appDetails.Add(tempKVPname, tempObjs);
                                        }
                                        if (issaved)
                                        {
                                            i = i + 10;
                                            break;
                                        }

                                        if (i + 10 > DictObjects[StatisticsEnum.ObjectType.DNGroup.ToString()].Count)
                                        {
                                            remainObjects = DictObjects[StatisticsEnum.ObjectType.DNGroup.ToString()].Count - i;
                                            i = i + (DictObjects[StatisticsEnum.ObjectType.DNGroup.ToString()].Count - i);
                                            issaved = true;
                                        }
                                        else
                                            i = i + 10;
                                    }
                                }
                                else
                                {
                                    if (appDetails.ContainsKey("statistics.objects-dn-groups"))
                                        appDetails.Remove("statistics.objects-dn-groups");

                                    appDetails.Add("statistics.objects-dn-groups", DNGroups);
                                }

                                #endregion
                            }
                        }

                        if (DictObjects.ContainsKey(StatisticsEnum.ObjectType.VirtualQueue.ToString()))
                        {
                            if (DictObjects[StatisticsEnum.ObjectType.VirtualQueue.ToString()] != null && DictObjects[StatisticsEnum.ObjectType.VirtualQueue.ToString()].Count != 0)
                            {
                                #region VirtualQueue

                                string VirtualQueues = string.Empty;
                                isExceedLimit = DictObjects[StatisticsEnum.ObjectType.VirtualQueue.ToString()].Count > 15 ? true : false;
                                string kvpname = "statistics.objects-virtual-queues";
                                bool issaved = false;
                                int kvpno = 0;

                                foreach (string VQueue in DictObjects[StatisticsEnum.ObjectType.VirtualQueue.ToString()])
                                {

                                    if (!isExceedLimit)
                                    {
                                        if (string.IsNullOrEmpty(VirtualQueues))
                                            VirtualQueues = VQueue;
                                        else
                                            VirtualQueues = VirtualQueues + "," + VQueue;
                                    }
                                }

                                if (isExceedLimit)
                                {
                                    int remainObjects = 10;

                                    for (int i = 10; i <= DictObjects[StatisticsEnum.ObjectType.VirtualQueue.ToString()].Count; )
                                    {
                                        tempObjs = string.Empty;
                                        for (int j = i - remainObjects; j < i; j++)
                                        {
                                            if (tempObjs == string.Empty)
                                                tempObjs = DictObjects[StatisticsEnum.ObjectType.VirtualQueue.ToString()][j];
                                            else
                                                tempObjs = tempObjs + "," + DictObjects[StatisticsEnum.ObjectType.VirtualQueue.ToString()][j];
                                        }

                                        kvpno++;
                                        string tempKVPname = kvpname + "_" + kvpno.ToString();
                                        if (appDetails.ContainsKey(tempKVPname))
                                        {
                                            appDetails.Remove(tempKVPname);
                                            appDetails[tempKVPname] = tempObjs;
                                        }
                                        else
                                        {
                                            if (tempObjs != "" && tempObjs != null && tempObjs != string.Empty)
                                                appDetails.Add(tempKVPname, tempObjs);
                                        }
                                        if (issaved)
                                        {
                                            i = i + 10;
                                            break;
                                        }

                                        if (i + 10 > DictObjects[StatisticsEnum.ObjectType.VirtualQueue.ToString()].Count)
                                        {
                                            remainObjects = DictObjects[StatisticsEnum.ObjectType.VirtualQueue.ToString()].Count - i;
                                            i = i + (DictObjects[StatisticsEnum.ObjectType.VirtualQueue.ToString()].Count - i);
                                            issaved = true;
                                        }
                                        else
                                            i = i + 10;
                                    }
                                }
                                else
                                {
                                    if (appDetails.ContainsKey("statistics.objects-virtual-queues"))
                                        appDetails.Remove("statistics.objects-virtual-queues");

                                    appDetails.Add("statistics.objects-virtual-queues", VirtualQueues);
                                }

                                #endregion
                            }
                        }

                        StatisticsSetting.GetInstance().Application.Options["agent.ixn.desktop"] = appDetails;

                        StatisticsSetting.GetInstance().Application.Save();
                    }
                }
            }
            catch (Exception GeneralException)
            {
                logger.Error("StatisticsBase : SaveObjectValues Method : " + GeneralException.Message);
            }
            logger.Debug("StatisticsBase : SaveObjectValues Method : Exit");
        }

        /// <summary>
        /// Saves the queues.
        /// </summary>
        /// <param name="queuename">The queuename.</param>
        public void SaveQueues(string queuename)
        {
            try
            {
                logger.Debug("StatisticsBase : SaveQueues Method : Entry");
                logger.Info("StatisticsBase : SaveQueues Method : Queue Name : " + queuename);
                string Dilimitor = "_@";
                string[] Queues = queuename.Split(new[] { Dilimitor }, StringSplitOptions.None);

                if (Queues.Length > 1)
                {
                    queuename = Queues[0].ToString() + "_@Skill_@" + Queues[1].ToString();
                    if (StatisticsSetting.GetInstance().statisticsCollection.StatisticsCommon.ACDObjects != "" && StatisticsSetting.GetInstance().statisticsCollection.StatisticsCommon.ACDObjects != string.Empty)
                        StatisticsSetting.GetInstance().statisticsCollection.StatisticsCommon.ACDObjects = StatisticsSetting.GetInstance().statisticsCollection.StatisticsCommon.ACDObjects + "," + queuename;
                    else
                        StatisticsSetting.GetInstance().statisticsCollection.StatisticsCommon.ACDObjects = queuename;
                }
                else
                {
                    logger.Error("StatisticsBase : SaveQueues Method : Agent Interaction Desktop Queue is in wrong format (Queue_Name@Switch_Name)");
                }
            }
            catch (Exception GeneralException)
            {
                logger.Error("StatisticsBase : SaveQueues Method : " + GeneralException.Message);
            }
            finally
            {
                logger.Debug("StatisticsBase : SaveQueues Method :  Exit");
            }
        }

        /// <summary>
        /// Saves the initialize parameters.
        /// </summary>
        /// <param name="applicationName">Name of the application.</param>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        /// <param name="ConfigHost">The config host.</param>
        /// <param name="ConfigPort">The config port.</param>
        /// <returns></returns>
        public void SaveTaggedStatsInXML(string dictTaggedStats, string isVertical, string isHeader, string left, string top)
        {
            XmlTextWriter writeUserDetails;
            XmlDocument doc = new XmlDocument();
            try
            {
                logger.Debug("StatisticsBase : SaveTaggedStatsInXML : Method Entry");

                if (!Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Pointel\\AgentInteractionDesktop"))
                {
                    logger.Debug("StatisticsBase : Create a new directory  : AgentInteractionDesktop");
                    Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
                                              "\\Pointel\\AgentInteractionDesktop");
                }
                string curFile = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Pointel\\AgentInteractionDesktop\\" + StatisticsSetting.GetInstance().logUserName + ".stat.config";
                if (File.Exists(curFile))
                {
                    logger.Debug("StatisticsBase : modifying existing config file in app data");
                    logger.Debug("StatisticsBase : checking permission for that directory  : AgentInteractionDesktop");
                    CheckPermission(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Pointel\\AgentInteractionDesktop\\" + StatisticsSetting.GetInstance().logUserName + ".stat.config");
                    if (DictFolderPermissions.Count != 0)
                    {
                        if (DictFolderPermissions.ContainsKey(FileSystemRights.Read.ToString()) && DictFolderPermissions.ContainsKey(FileSystemRights.Write.ToString()))
                        {
                            if (DictFolderPermissions[FileSystemRights.Read.ToString()] && DictFolderPermissions[FileSystemRights.Write.ToString()])
                            {
                                XmlDocument xDocument = new XmlDocument();
                                xDocument.Load(curFile);
                                XmlNode root = xDocument.DocumentElement;
                                XmlNodeList reminders = xDocument.SelectNodes("//AgentConfig//StatTickerFive");
                                foreach (XmlNode reminder in reminders)
                                {
                                    XmlNode myNode1 = root.SelectSingleNode("descendant::agent-tagged-statistics");
                                    if (myNode1 == null)
                                    {
                                        XmlNode attr1 = xDocument.CreateNode(XmlNodeType.Element, "agent-tagged-statistics", null);
                                        attr1.InnerText = dictTaggedStats;
                                        reminder.AppendChild(attr1);
                                    }
                                    else
                                    {
                                        myNode1.InnerText = dictTaggedStats;
                                    }

                                    XmlNode myNode2 = root.SelectSingleNode("descendant::statistics.enable-header");
                                    if (myNode2 == null)
                                    {
                                        XmlNode attr1 = xDocument.CreateNode(XmlNodeType.Element, "statistics.enable-header", null);
                                        attr1.InnerText = isHeader;
                                        reminder.AppendChild(attr1);
                                    }
                                    else
                                    {
                                        myNode2.InnerText = isHeader;
                                    }

                                    XmlNode myNode3 = root.SelectSingleNode("descendant::statistics.enable-tag-vertical");
                                    if (myNode3 == null)
                                    {

                                        XmlNode attr1 = xDocument.CreateNode(XmlNodeType.Element, "statistics.enable-tag-vertical", null);
                                        attr1.InnerText = isVertical;
                                        reminder.AppendChild(attr1);
                                    }
                                    else
                                    {
                                        myNode3.InnerText = isVertical;
                                    }

                                    XmlNode myNode4 = root.SelectSingleNode("descendant::statistics.gadget-position");
                                    if (myNode4 == null)
                                    {
                                        XmlNode attr1 = xDocument.CreateNode(XmlNodeType.Element, "statistics.gadget-position", null);
                                        attr1.InnerText = left + "," + top;
                                        reminder.AppendChild(attr1);
                                    }
                                    else
                                    {
                                        myNode4.InnerText = left + "," + top;
                                    }

                                }

                                xDocument.Save(curFile);
                            }
                            else
                            {
                                logger.Warn("No permission to read and write in the app data");
                                WriteAgentConfigFile(Environment.CurrentDirectory + "\\" + StatisticsSetting.GetInstance().logUserName + ".stat.config", dictTaggedStats, isVertical, isHeader, left, top);
                            }
                        }
                        else
                        {
                            logger.Warn("No permission to read and write in the app data");
                            WriteAgentConfigFile(Environment.CurrentDirectory + "\\" + StatisticsSetting.GetInstance().logUserName + ".stat.config", dictTaggedStats, isVertical, isHeader, left, top);
                        }
                    }
                    else
                    {
                        logger.Warn("No permission to read and write in the app data");
                        WriteAgentConfigFile(Environment.CurrentDirectory + "\\" + StatisticsSetting.GetInstance().logUserName + ".stat.config", dictTaggedStats, isVertical, isHeader, left, top);
                    }
                }
                else
                {
                    logger.Debug("StatisticsBase : create a new config file in app data");
                    WriteAgentConfigFile(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Pointel\\AgentInteractionDesktop\\" + StatisticsSetting.GetInstance().logUserName + ".stat.config", dictTaggedStats, isVertical, isHeader, left, top);
                }
            }
            catch (Exception generalException)
            {
                logger.Error("StatisticsBase : SaveTaggedStatsInXML Method : Exception caught : " +
                             generalException.Message);
            }
            finally
            {
                writeUserDetails = null;
                GC.Collect();
                logger.Debug("StatisticsBase : SaveTaggedStatsInXML : Method Exit");
            }
        }

        public void SendToAIDPipe()
        {
            try
            {
                logger.Debug("StatisticsBase : StartUpdatedStatistics : Method Entry");

                NamedPipeClientStream clientStream = new NamedPipeClientStream(".", StatisticsSetting.GetInstance().PersonDetails.UserName, PipeDirection.Out, PipeOptions.Asynchronous);
                clientStream.Connect();

                if (!clientStream.IsConnected)
                {
                    //Added by Elango to avoid application freeze while namedpipe server diconnected
                    Thread th = new Thread(new ThreadStart(clientStream.Connect));
                    th.Start();
                }

                if (clientStream.IsConnected)
                {
                    string data;
                    byte[] dataToWrite;

                    //data = "ACD:" + StatisticsSetting.GetInstance().statisticsCollection.StatisticsCommon.ACDObjects + "<>" + "DN:" + StatisticsSetting.GetInstance().statisticsCollection.StatisticsCommon.DNGroupObjects + "<>" + "VQ:" + StatisticsSetting.GetInstance().statisticsCollection.StatisticsCommon.VQueueObjects;
                    dataToWrite = System.Text.ASCIIEncoding.ASCII.GetBytes("Queues updated");
                    clientStream.Write(dataToWrite, 0, dataToWrite.Length);

                    //data = "DN:" + StatisticsSetting.GetInstance().statisticsCollection.StatisticsCommon.DNGroupObjects;
                    //dataToWrite = System.Text.ASCIIEncoding.ASCII.GetBytes(data);
                    //clientStream.Write(dataToWrite, 0, dataToWrite.Length);

                    //data = "VQ:" + StatisticsSetting.GetInstance().statisticsCollection.StatisticsCommon.VQueueObjects;
                    //dataToWrite = System.Text.ASCIIEncoding.ASCII.GetBytes(data);
                    //clientStream.Write(dataToWrite, 0, dataToWrite.Length);
                }

                logger.Debug("StatisticsBase : StartUpdatedStatistics : Method Exit");
            }
            catch (Exception generalException)
            {
                logger.Error("StatisticsBase : ConnectAIDPipe Method : " + generalException.Message);
            }
        }

        /// <summary>
        /// Shows the CC statistics AID.
        /// </summary>
        /// <param name="ccstatistics">if set to <c>true</c> [ccstatistics].</param>
        public void ShowCCStatisticsAID(bool ccstatistics)
        {
            logger.Debug("StatisticsBase : ShowCCStatisticsAID Method : Entry");
            foreach (KeyValuePair<string, IStatTicker> messageToClient in ToClient)
            {
                if (messageToClient.Key == "StatTickerFive" || messageToClient.Key == "AID")
                {
                    messageToClient.Value.NotifyShowCCStatistics(ccstatistics);
                }
            }
            logger.Debug("StatisticsBase : ShowCCStatisticsAID Method : Exit");
        }

        /// <summary>
        /// Shows the state of the gadget.
        /// </summary>
        /// <param name="gadgetState">State of the gadget.</param>
        public void ShowGadgetState(StatisticsEnum.GadgetState gadgetState)
        {
            logger.Debug("StatisticsBase : ShowGadgetState Method : Entry");
            foreach (KeyValuePair<string, IStatTicker> messageToClient in ToClient)
            {
                if (messageToClient.Key == "StatTickerFive" || messageToClient.Key == "AID")
                {
                    messageToClient.Value.NotifyGadgetStatus(gadgetState);
                }
            }
            logger.Debug("StatisticsBase : ShowGadgetState Method : Exit");
        }

        /// <summary>
        /// Shows my statistis AID.
        /// </summary>
        /// <param name="mystatistics">if set to <c>true</c> [mystatistics].</param>
        public void ShowMyStatistisAID(bool mystatistics)
        {
            logger.Debug("StatisticsBase : ShowMyStatistisAID Method : Entry");

            foreach (KeyValuePair<string, IStatTicker> messageToClient in ToClient)
            {
                if (messageToClient.Key == "StatTickerFive" || messageToClient.Key == "AID")
                {
                    messageToClient.Value.NotifyShowMyStatistics(mystatistics);
                }
            }

            logger.Debug("StatisticsBase : ShowMyStatistisAID Method : Exit");
        }

        //public StatServerProtocol StartServerConnection(string User)
        //{
        //    ReportingServer reporting = new ReportingServer();
        //    try
        //    {
        //        string host = StatisticsSetting.GetInstance().statisticsCollection.StatisticsLocalSetting.PrimaryStatServer.ServerInfo.Host.IPaddress;
        //        string port = StatisticsSetting.GetInstance().statisticsCollection.StatisticsLocalSetting.PrimaryStatServer.ServerInfo.Port;
        //        string secondaryHost;
        //        string secondaryPort;
        //        if (StatisticsSetting.GetInstance().statisticsCollection.StatisticsLocalSetting.SecondaryStatServer != null)
        //        {
        //            secondaryHost = StatisticsSetting.GetInstance().statisticsCollection.StatisticsLocalSetting.SecondaryStatServer.ServerInfo.Host.Name;
        //            secondaryPort = StatisticsSetting.GetInstance().statisticsCollection.StatisticsLocalSetting.SecondaryStatServer.ServerInfo.Port;
        //        }
        //        else
        //        {
        //            secondaryHost = StatisticsSetting.GetInstance().statisticsCollection.StatisticsLocalSetting.PrimaryStatServer.ServerInfo.Host.Name;
        //            secondaryPort = StatisticsSetting.GetInstance().statisticsCollection.StatisticsLocalSetting.PrimaryStatServer.ServerInfo.Port;
        //        }
        //        int addpServerTimeOut = StatisticsSetting.GetInstance().statisticsCollection.StatisticsLocalSetting.AddpServerTimeOut;
        //        int addpClientTimeOut = StatisticsSetting.GetInstance().statisticsCollection.StatisticsLocalSetting.AddpClientTimeOut;
        //        string agent = User;
        //        int timeout = Convert.ToInt32(StatisticsSetting.GetInstance().statisticsCollection.StatisticsLocalSetting.PrimaryStatServer.ServerInfo.Timeout);
        //        short attempts = Convert.ToInt16(StatisticsSetting.GetInstance().statisticsCollection.StatisticsLocalSetting.PrimaryStatServer.ServerInfo.Attempts);
        //return reporting.CreateConnection(host, port, secondaryHost, secondaryPort, addpServerTimeOut, addpClientTimeOut, agent, timeout, attempts);
        //    }
        //    catch (Exception GeneralException)
        //    {
        //    }
        //    return null;
        //}
        public void StartServerConnection(string User, int addpServerTimeOut, int addpClientTimeOut)
        {
            logger.Debug("StatisticsBase : StartServerConnection Method Entry");
            if (settings.statProtocols == null)
            {
                ReportingServer reporting = new ReportingServer();
                StatisticsSetting.GetInstance().statisticsCollection.StatisticsLocalSetting.AddpServerTimeOut = addpServerTimeOut;
                StatisticsSetting.GetInstance().statisticsCollection.StatisticsLocalSetting.AddpClientTimeOut = addpClientTimeOut;
                logger.Info("Stat AddpServerTimeOut : " + StatisticsSetting.GetInstance().statisticsCollection.StatisticsLocalSetting.AddpServerTimeOut);
                logger.Info("Stat AddpClientTimeOut : " + StatisticsSetting.GetInstance().statisticsCollection.StatisticsLocalSetting.AddpClientTimeOut);
                try
                {
                    if (StatisticsSetting.GetInstance().statisticsCollection.StatisticsLocalSetting.PrimaryStatServer != null)
                    {
                        foreach (CfgApplication server in StatisticsSetting.GetInstance().statisticsCollection.StatisticsLocalSetting.PrimaryStatServer)
                        {
                            Uri primaryUri = new Uri("tcp://" + server.ServerInfo.Host.IPaddress + ":" + server.ServerInfo.Port);
                            Uri backupUri = new Uri("tcp://" + server.ServerInfo.Host.IPaddress + ":" + server.ServerInfo.Port);

                            //Uri backupUri = new Uri("tcp://" + server.ServerInfo.BackupServer.DBID != "0" ? server.ServerInfo.BackupServer.ServerInfo.Host.IPaddress + ":" + server.ServerInfo.BackupServer.ServerInfo.Port : server.ServerInfo.Host.IPaddress + ":" + server.ServerInfo.Port);
                            string agent = User;
                            int timeout = Convert.ToInt32(server.ServerInfo.Timeout);
                            short attempts = Convert.ToInt16(server.ServerInfo.Attempts);
                            string protocolName = server.Name;
                            //StatisticsSetting.GetInstance().serverNames.Add(server.Name);
                            reporting.CreateConnection(primaryUri, backupUri, addpServerTimeOut, addpClientTimeOut, agent, timeout, attempts, protocolName);
                        }
                    }
                    if (StatisticsSetting.GetInstance().statisticsCollection.StatisticsLocalSetting.PrimaryStatServerUri != null)
                    {
                        foreach (Uri server in StatisticsSetting.GetInstance().statisticsCollection.StatisticsLocalSetting.PrimaryStatServerUri)
                        {
                            Uri primaryUri = server;
                            Uri backupUri = server;
                            string agent = User;
                            int timeout = Convert.ToInt32(10);
                            short attempts = Convert.ToInt16(1);
                            string protocolName = server.Port.ToString();
                            //StatisticsSetting.GetInstance().serverNames.Add(server.Port.ToString());
                            reporting.CreateConnection(primaryUri, backupUri, addpServerTimeOut, addpClientTimeOut, agent, timeout, attempts, protocolName);
                        }
                    }

                }
                catch (Exception GeneralException)
                {
                    logger.Error("StatisticsBase : StartServerConnection Method Exception:" + GeneralException.Message.ToString());
                }
                logger.Debug("StatisticsBase : StartServerConnection Method Exit");
            }
        }

        /// <summary>
        /// Starts the statistics.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="appname">The appname.</param>
        /// <returns></returns>
        public bool StartStatistics(string userName, string appname, int addpServerTimeOut, int addpClientTimeOut, bool isStatEnabled, bool isAdmin)
        {
            try
            {
                logger.Debug("StatisticsBase : StartStatistics Method : Entry");

                if (StatisticsSetting.GetInstance().statisticsCollection.StatisticsCommon.AgentStatistics == "" &&
                     StatisticsSetting.GetInstance().statisticsCollection.StatisticsCommon.AgentGroupStatistics == "" &&
                      StatisticsSetting.GetInstance().statisticsCollection.StatisticsCommon.ACDStatistics == "" &&
                      StatisticsSetting.GetInstance().statisticsCollection.StatisticsCommon.DNGroupStatistics == "" &&
                        StatisticsSetting.GetInstance().statisticsCollection.StatisticsCommon.VQueueStatistics == "" && !isAdmin)
                {

                    foreach (KeyValuePair<string, IStatTicker> messageToClient in ToClient)
                    {
                        if (messageToClient.Key == "StatTickerFive" || messageToClient.Key == "AID")
                        {
                            OutputValues serverError = new OutputValues();

                            serverError.Message = StatisticsSetting.GetInstance().DictErrorValues["nostat.configured"].ToString();

                            serverError.MessageCode = "2002";
                            messageToClient.Value.NotifyStatErrorMessage(serverError);
                        }
                    }

                    return false;

                }
                else
                {
                    if ((StatisticsSetting.GetInstance().statisticsCollection.StatisticsLocalSetting.PrimaryStatServer != null) || (StatisticsSetting.GetInstance().statisticsCollection.StatisticsLocalSetting.PrimaryStatServerUri != null))
                    {
                        StartServerConnection(userName, addpServerTimeOut, addpClientTimeOut);
                        if (settings.statProtocols != null)
                        {
                            for (int pCount = 0; pCount < settings.statProtocols.Count; pCount++)
                            {
                                settings.statProtocols[pCount].Opened += statServerProtocol_Opened;
                                settings.statProtocols[pCount].Closed += statServerProtocol_Closed;
                            }

                            if (!isAdmin || isStatEnabled)
                            {
                                if (!StatisticsSetting.GetInstance().statisticsCollection.ApplicationContainer.EnableMyQueueConfig)
                                {

                                    if (settings.statProtocols[0].State == ChannelState.Opened & !IsSubscribed)
                                    {
                                        newStatisticsDataProvider = new StatisticsDataProvider();
                                        agentSubscriber = new AgentStatisticsSubscriber();
                                        agentGroupSubscriber = new AgentGroupStatisticsSubscriber();
                                        acdSubscriber = new ACDStatisticsSubscriber();
                                        dngroupSubscriber = new DNGroupStatisticsSubscriber();
                                        vqSubscriber = new VQStatisticsSubscriber();

                                        agentSubscriber.Subscribe(newStatisticsDataProvider);
                                        agentGroupSubscriber.Subscribe(newStatisticsDataProvider);
                                        acdSubscriber.Subscribe(newStatisticsDataProvider);
                                        dngroupSubscriber.Subscribe(newStatisticsDataProvider);
                                        vqSubscriber.Subscribe(newStatisticsDataProvider);

                                        newStatisticsDataProvider.NewStatisticsData(StatisticsSetting.GetInstance().statisticsCollection);
                                        IsSubscribed = true;
                                    }
                                }
                                else
                                {

                                    if (settings.statProtocols[0].State == ChannelState.Opened & !IsSubscribed)
                                    {
                                        newStatisticsDataProvider = new StatisticsDataProvider();
                                        agentSubscriber = new AgentStatisticsSubscriber();
                                        agentGroupSubscriber = new AgentGroupStatisticsSubscriber();
                                        commonSubscriber = new CommonStatisticsSubscriber();

                                        agentSubscriber.Subscribe(newStatisticsDataProvider);
                                        agentGroupSubscriber.Subscribe(newStatisticsDataProvider);
                                        commonSubscriber.Subscribe(newStatisticsDataProvider);

                                        newStatisticsDataProvider.NewStatisticsData(StatisticsSetting.GetInstance().statisticsCollection);
                                        IsSubscribed = true;
                                    }
                                }
                            }
                            //foreach (KeyValuePair<string, IStatTicker> messageToClient in ToClient)
                            //{
                            //    if (messageToClient.Key == "StatTickerFive" || messageToClient.Key == "AID")
                            //    {
                            //        messageToClient.Value.NotifyStatServerStatustoTC(true);
                            //    }
                            //}
                            return true;
                        }

                        else
                            return false;
                    }
                    else
                    {

                        foreach (KeyValuePair<string, IStatTicker> messageToClient in ToClient)
                        {
                            if (messageToClient.Key == "StatTickerFive" || messageToClient.Key == "AID")
                            {
                                OutputValues serverError = new OutputValues();

                                serverError.Message = StatisticsSetting.GetInstance().statisticsCollection.ApplicationContainer.PrimaryServer.ToString();

                                serverError.MessageCode = "2001";
                                messageToClient.Value.NotifyStatErrorMessage(serverError);
                            }
                        }
                        logger.Error("StatisticsBase : StartStatistics Method : " + StatisticsSetting.GetInstance().statisticsCollection.ApplicationContainer.PrimaryServer.ToString());
                        return false;
                    }
                }
            }
            catch (Exception GeneralException)
            {
                logger.Error("StatisticsBase : StartStatistics Method : " + GeneralException.Message);
                return false;
            }
            finally
            {
                agentSubscriber = null;
                agentGroupSubscriber = null;
                acdSubscriber = null;
                dngroupSubscriber = null;
                vqSubscriber = null;
                logger.Debug("StatisticsBase : StartStatistics Method : Exit");
            }
        }

        //Commented by Elango.T, for removing DB in AID
        //public bool StartStatistics()
        //{
        //    ReportingServer reporting = new ReportingServer();
        //    ReadApplication read = new ReadApplication();
        //    try
        //    {
        //        logger.Debug("StatisticsBase : StartStatistics : Method Entry");
        //        logger.Info("StatisticsBase : StartStatistics :  DBType : " + StatisticsSetting.GetInstance().DBType);
        //        if (StatisticsSetting.GetInstance().statisticsCollection.DBValues == null)
        //            read.ReadDBDetails();
        //        if (StatisticsSetting.GetInstance().statisticsCollection.DBValues != null && StatisticsSetting.GetInstance().statisticsCollection.DBValues.Count != 0)
        //        {
        //            if (StatisticsSetting.GetInstance().statisticsCollection.DBValues[0].Query == "" || StatisticsSetting.GetInstance().statisticsCollection.DBValues[0].Query == "")
        //            {
        //                foreach (KeyValuePair<string, IStatTicker> messageToClient in ToClient)
        //                {
        //                    if (messageToClient.Key == "StatTickerFive" || messageToClient.Key == "AID")
        //                    {
        //                        OutputValues serverError = new OutputValues();
        //                        serverError.Message = StatisticsSetting.GetInstance().DictErrorValues["nostat.configured"].ToString();
        //                        serverError.MessageCode = "2002";
        //                        messageToClient.Value.NotifyStatErrorMessage(serverError);
        //                    }
        //                }
        //                return false;
        //            }
        //            else
        //            {
        //                newStatisticsDataProvider = new StatisticsDataProvider();
        //                dbSubscriber = new DBSubscriber();
        //                dbSubscriber.Subscribe(newStatisticsDataProvider);
        //                newStatisticsDataProvider.NewStatisticsData(StatisticsSetting.GetInstance().statisticsCollection);
        //                return true;
        //            }
        //        }
        //        else
        //        {
        //            foreach (KeyValuePair<string, IStatTicker> messageToClient in ToClient)
        //            {
        //                if (messageToClient.Key == "StatTickerFive" || messageToClient.Key == "AID")
        //                {
        //                    OutputValues serverError = new OutputValues();
        //                    serverError.Message = StatisticsSetting.GetInstance().DictErrorValues["nostat.configured"].ToString();
        //                    serverError.MessageCode = "2002";
        //                    messageToClient.Value.NotifyStatErrorMessage(serverError);
        //                }
        //            }
        //            return false;
        //        }
        //    }
        //    catch (Exception GeneralException)
        //    {
        //        logger.Error("StatisticsBase : StartStatistics : " + GeneralException.Message);
        //        return false;
        //    }
        //    finally
        //    {
        //        agentSubscriber = null;
        //        agentGroupSubscriber = null;
        //        acdSubscriber = null;
        //        dngroupSubscriber = null;
        //        vqSubscriber = null;
        //        logger.Debug("StatisticsBase : StartStatistics : Method Exit");
        //    }
        //}
        public void StartUpdatedStatistics()
        {
            try
            {
                logger.Debug("StatisticsBase : StartUpdatedStatistics : Method Entry");

                ReadApplication read = new ReadApplication();
                read.ReadAgent(StatisticsSetting.GetInstance().PersonDetails.UserName, false);
                read.ReadAgentDetails(StatisticsSetting.GetInstance().statisticsCollection, StatisticsSetting.GetInstance().PersonDetails.UserName);
                StatisticsSetting.GetInstance().FinalAgentPackageID.Clear();
                StatisticsSetting.GetInstance().FinalAgentGroupPackageID.Clear();
                StatisticsSetting.GetInstance().FinalACDPackageID.Clear();
                StatisticsSetting.GetInstance().FinalDNGroupsPackageID.Clear();
                StatisticsSetting.GetInstance().FinalVQPackageID.Clear();

                newStatisticsDataProvider = new StatisticsDataProvider();
                agentSubscriber = new AgentStatisticsSubscriber();
                agentGroupSubscriber = new AgentGroupStatisticsSubscriber();
                commonSubscriber = new CommonStatisticsSubscriber();

                agentSubscriber.Subscribe(newStatisticsDataProvider);
                agentGroupSubscriber.Subscribe(newStatisticsDataProvider);
                commonSubscriber.Subscribe(newStatisticsDataProvider);

                newStatisticsDataProvider.NewStatisticsData(StatisticsSetting.GetInstance().statisticsCollection);

                logger.Debug("StatisticsBase : StartUpdatedStatistics : Method Exit");
            }
            catch (Exception generalException)
            {
                logger.Error("StatisticsBase : StartUpdatedStatistics Method : " + generalException.Message);
            }
        }

        /// <summary>
        /// Stores the database values.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <param name="port">The port.</param>
        /// <param name="loginQuery">The login query.</param>
        /// <param name="dbType">Type of the database.</param>
        /// <param name="dbName">Name of the database.</param>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        /// <param name="sid">The sid.</param>
        /// <param name="sname">The sname.</param>
        /// <param name="dbSource">The database source.</param>
        public void StoreDBValues(string host, string port, string loginQuery, string dbType, string dbName, string userName, string password, string sid, string sname, string dbSource)
        {
            try
            {
                logger.Debug("StatisticsBase : StoreDBValues : Method Entry");

                #region Storing DB Connection Values

                StatisticsSetting.GetInstance().DBHost = host;
                StatisticsSetting.GetInstance().DBPort = port;
                StatisticsSetting.GetInstance().DBLoginQuery = loginQuery;
                StatisticsSetting.GetInstance().DBType = dbType;
                StatisticsSetting.GetInstance().DBName = dbName;
                StatisticsSetting.GetInstance().DBUserName = userName;
                StatisticsSetting.GetInstance().DBPassword = password;
                StatisticsSetting.GetInstance().DBSID = sid;
                StatisticsSetting.GetInstance().DBSName = sname;
                StatisticsSetting.GetInstance().DBDataSource = dbSource;

                #endregion
            }
            catch (Exception GeneralException)
            {
                logger.Error("StatisticsBase : StoreDBValues : " + GeneralException.Message);
            }
        }

        /// <summary>
        /// Subscribes the specified listener.
        /// </summary>
        /// <param name="listener">The listener.</param>
        public void Subscribe(string Source, IStatTicker listener)
        {
            try
            {
                if (!ToClient.ContainsKey(Source))
                    ToClient.Add(Source, listener);

                //Code added by Elango T on 13/03/2015 for notifying Plugin gadget with full statistics.
                if (StatisticsBase.GetInstance().isPlugin)
                {
                    if (ToClient.ContainsKey("StatTickerFive"))
                    {
                        ThresholdSettings getColor = new ThresholdSettings();
                        if (StatisticsSetting.GetInstance().agentStatisticsPluginHolder != null)
                        {
                            if (StatisticsSetting.GetInstance().agentStatisticsPluginHolder.Count > 0)
                            {
                                foreach (KeyValuePair<int, StatVariables> values in StatisticsSetting.GetInstance().agentStatisticsPluginHolder)
                                {
                                    StatVariables StatsValue = new StatVariables();
                                    StatsValue = (StatVariables)values.Value;

                                    string Svalue = StatsValue.StatValue;
                                    string Sid = StatsValue.ObjectID;
                                    string Srefid = StatsValue.ReferenceId;
                                    string SFormat = StatsValue.Statformat;
                                    ToClient["StatTickerFive"].NotifyAgentStatistics(StatsValue.ReferenceId, StatsValue.DisplayName, StatsValue.StatValue, StatsValue.ObjectID + "\n" + StatsValue.Tooltip, getColor.ThresholdColor(Svalue, Sid, Srefid, SFormat), StatsValue.StatType, StatisticsSetting.GetInstance().isThresholdBreach, StatisticsSetting.GetInstance().isLevelTwo);
                                }
                            }
                        }

                        if (StatisticsSetting.GetInstance().agentGroupStatsPluginHolder != null)
                        {
                            if (StatisticsSetting.GetInstance().agentGroupStatsPluginHolder.Count > 0)
                            {
                                foreach (KeyValuePair<int, StatVariables> values in StatisticsSetting.GetInstance().agentGroupStatsPluginHolder)
                                {
                                    StatVariables StatsValue = new StatVariables();
                                    StatsValue = (StatVariables)values.Value;

                                    string Svalue = StatsValue.StatValue;
                                    string Sid = StatsValue.ObjectID;
                                    string Srefid = StatsValue.ReferenceId;
                                    string SFormat = StatsValue.Statformat;
                                    ToClient["StatTickerFive"].NotifyAgentGroupStatistics(StatsValue.ReferenceId, StatsValue.DisplayName, StatsValue.StatValue, StatsValue.ObjectID + "\n" + StatsValue.Tooltip, getColor.ThresholdColor(Svalue, Sid, Srefid, SFormat), StatsValue.StatType, StatisticsSetting.GetInstance().isThresholdBreach, StatisticsSetting.GetInstance().isLevelTwo);
                                }
                            }
                        }

                        if (StatisticsSetting.GetInstance().ACDStatisticsPluginHolder != null)
                        {
                            if (StatisticsSetting.GetInstance().ACDStatisticsPluginHolder.Count > 0)
                            {
                                foreach (KeyValuePair<int, StatVariables> values in StatisticsSetting.GetInstance().ACDStatisticsPluginHolder)
                                {
                                    StatVariables StatsValue = new StatVariables();
                                    StatsValue = (StatVariables)values.Value;

                                    string Svalue = StatsValue.StatValue;
                                    string Sid = StatsValue.ObjectID;
                                    string Srefid = StatsValue.ReferenceId;
                                    string SFormat = StatsValue.Statformat;
                                    ToClient["StatTickerFive"].NotifyQueueStatistics(StatsValue.ReferenceId, StatsValue.DisplayName, StatsValue.StatValue, StatsValue.ObjectID + "\n" + StatsValue.Tooltip, getColor.ThresholdColor(Svalue, Sid, Srefid, SFormat), StatsValue.StatType, StatisticsSetting.GetInstance().isThresholdBreach, StatisticsSetting.GetInstance().isLevelTwo);
                                }
                            }
                        }

                        if (StatisticsSetting.GetInstance().dnGroupStatisticsPluginHolder != null)
                        {
                            if (StatisticsSetting.GetInstance().dnGroupStatisticsPluginHolder.Count > 0)
                            {
                                foreach (KeyValuePair<int, StatVariables> values in StatisticsSetting.GetInstance().dnGroupStatisticsPluginHolder)
                                {
                                    StatVariables StatsValue = new StatVariables();
                                    StatsValue = (StatVariables)values.Value;

                                    string Svalue = StatsValue.StatValue;
                                    string Sid = StatsValue.ObjectID;
                                    string Srefid = StatsValue.ReferenceId;
                                    string SFormat = StatsValue.Statformat;
                                    ToClient["StatTickerFive"].NotifyQueueStatistics(StatsValue.ReferenceId, StatsValue.DisplayName, StatsValue.StatValue, StatsValue.ObjectID + "\n" + StatsValue.Tooltip, getColor.ThresholdColor(Svalue, Sid, Srefid, SFormat), StatsValue.StatType, StatisticsSetting.GetInstance().isThresholdBreach, StatisticsSetting.GetInstance().isLevelTwo);
                                }
                            }
                        }

                        if (StatisticsSetting.GetInstance().VQStatisticsPlugineHolder != null)
                        {
                            if (StatisticsSetting.GetInstance().VQStatisticsPlugineHolder.Count > 0)
                            {
                                foreach (KeyValuePair<int, StatVariables> values in StatisticsSetting.GetInstance().VQStatisticsPlugineHolder)
                                {
                                    StatVariables StatsValue = new StatVariables();
                                    StatsValue = (StatVariables)values.Value;

                                    string Svalue = StatsValue.StatValue;
                                    string Sid = StatsValue.ObjectID;
                                    string Srefid = StatsValue.ReferenceId;
                                    string SFormat = StatsValue.Statformat;
                                    ToClient["StatTickerFive"].NotifyQueueStatistics(StatsValue.ReferenceId, StatsValue.DisplayName, StatsValue.StatValue, StatsValue.ObjectID + "\n" + StatsValue.Tooltip, getColor.ThresholdColor(Svalue, Sid, Srefid, SFormat), StatsValue.StatType, StatisticsSetting.GetInstance().isThresholdBreach, StatisticsSetting.GetInstance().isLevelTwo);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception GeneralException)
            {
                logger.Error("StatisticsBase : Subscribe Method : " + GeneralException.Message);
            }
        }

        public void SubscribeStats()
        {
        }

        public void UnSubscribe(string source)
        {
            if (ToClient.ContainsKey(source))
            {
                ToClient.Remove(source);
            }
        }

        public void WriteAgentConfigFile(string path, string dictTaggedStats, string isVertical, string isHeader, string left, string top)
        {
            logger.Debug("StatisticsBase: WriteAgentConfigFile method entry");
            try
            {
                if (File.Exists(path))
                {
                    XmlDocument xDocument = new XmlDocument();
                    xDocument.Load(path);
                    XmlNode root = xDocument.DocumentElement;
                    XmlNodeList reminders = xDocument.SelectNodes("//AgentConfig//StatTickerFive");
                    foreach (XmlNode reminder in reminders)
                    {
                        XmlNode myNode1 = root.SelectSingleNode("descendant::agent-tagged-statistics");
                        if (myNode1 == null)
                        {
                            XmlNode attr1 = xDocument.CreateNode(XmlNodeType.Element, "agent-tagged-statistics", null);
                            attr1.InnerText = dictTaggedStats;
                            reminder.AppendChild(attr1);
                        }
                        else
                        {
                            myNode1.InnerText = dictTaggedStats;
                        }

                        XmlNode myNode2 = root.SelectSingleNode("descendant::statistics.enable-header");
                        if (myNode2 == null)
                        {
                            XmlNode attr1 = xDocument.CreateNode(XmlNodeType.Element, "statistics.enable-header", null);
                            attr1.InnerText = isHeader;
                            reminder.AppendChild(attr1);
                        }
                        else
                        {
                            myNode2.InnerText = isHeader;
                        }

                        XmlNode myNode3 = root.SelectSingleNode("descendant::statistics.enable-tag-vertical");
                        if (myNode3 == null)
                        {

                            XmlNode attr1 = xDocument.CreateNode(XmlNodeType.Element, "statistics.enable-tag-vertical", null);
                            attr1.InnerText = isVertical;
                            reminder.AppendChild(attr1);
                        }
                        else
                        {
                            myNode3.InnerText = isVertical;
                        }

                        XmlNode myNode4 = root.SelectSingleNode("descendant::statistics.gadget-position");
                        if (myNode4 == null)
                        {
                            XmlNode attr1 = xDocument.CreateNode(XmlNodeType.Element, "statistics.gadget-position", null);
                            attr1.InnerText = left + "," + top;
                            reminder.AppendChild(attr1);
                        }
                        else
                        {
                            myNode4.InnerText = left + "," + top;
                        }

                    }

                    xDocument.Save(path);
                }
                else
                {
                    logger.Debug("The agent config file path is : " + path);
                    XmlTextWriter writeUserDetails = new XmlTextWriter(path, Encoding.Default);
                    writeUserDetails.Formatting = Formatting.Indented;

                    writeUserDetails.WriteStartElement("AgentConfig");
                    writeUserDetails.WriteStartElement("StatTickerFive");
                    writeUserDetails.WriteElementString("agent-tagged-statistics", dictTaggedStats);
                    writeUserDetails.WriteElementString("statistics.gadget-position", left + "," + top);

                    writeUserDetails.WriteElementString("statistics.enable-header", isHeader);
                    writeUserDetails.WriteElementString("statistics.enable-tag-vertical", isVertical);
                    writeUserDetails.WriteEndElement();
                    writeUserDetails.Close();
                    writeUserDetails = null;
                }

            }
            catch (Exception generalException)
            {
                logger.Error("StatisticsBase: WriteAgentConfigFile method Exception" + generalException.ToString());
            }
            logger.Debug("StatisticsBase : WriteAgentConfigFile method - exit");
        }

        bool CheckDBSettingsValue(string host, string port, string loginQuery, string dbType, string dbName, string userName, string sid, string sname, string dbSource)
        {
            try
            {
                if (dbType != string.Empty || dbType != "")
                {
                    if (dbType == StatisticsEnum.DBType.SQLServer.ToString())
                    {
                        if (dbName == string.Empty || dbName == "")
                        {
                            StatisticsBase.GetInstance().ListMissedValues.Add("db-Name");
                        }

                        if (userName == string.Empty || userName == "")
                        {
                            StatisticsBase.GetInstance().ListMissedValues.Add("db-Username");
                        }

                        if (dbSource == string.Empty || dbSource == "")
                        {
                            StatisticsBase.GetInstance().ListMissedValues.Add("db-Source");
                        }
                    }
                    else if (dbType == StatisticsEnum.DBType.SQLite.ToString())
                    {
                        if (dbSource == string.Empty || dbSource == "")
                        {
                            StatisticsBase.GetInstance().ListMissedValues.Add("db-Source");
                        }
                    }
                    else if (dbType == StatisticsEnum.DBType.ORACLE.ToString())
                    {
                        if (host == string.Empty || host == "")
                        {
                            StatisticsBase.GetInstance().ListMissedValues.Add("db-Host");
                        }

                        if (port == string.Empty || port == "")
                        {
                            StatisticsBase.GetInstance().ListMissedValues.Add("db-Port");
                        }

                        if (dbName == string.Empty || dbName == "")
                        {
                            StatisticsBase.GetInstance().ListMissedValues.Add("db-Name");
                        }

                        if (userName == string.Empty || userName == "")
                        {
                            StatisticsBase.GetInstance().ListMissedValues.Add("db-Username");
                        }

                        if ((sid == string.Empty || sid == "") && (sname == string.Empty || sname == ""))
                        {
                            StatisticsBase.GetInstance().ListMissedValues.Add("db-Sid (or) db-Sname");
                        }

                        if (dbSource == string.Empty || dbSource == "")
                        {
                            StatisticsBase.GetInstance().ListMissedValues.Add("db-Source");
                        }
                    }

                    if (loginQuery == string.Empty || loginQuery == "")
                    {
                        StatisticsBase.GetInstance().ListMissedValues.Add("db-loginquery");
                    }

                }
                else
                {
                    StatisticsBase.GetInstance().ListMissedValues.Add("db-Type");
                    return true;
                }

                if (StatisticsBase.GetInstance().ListMissedValues.Count != 0)
                    return true;
                else
                    return false;
            }
            catch (Exception generalException)
            {
                return true;
            }
        }

        private string GetState(int stateReason)
        {
            string state = string.Empty;
            try
            {
                logger.Debug("StatisticsBase : GetStatus Method : Entry");
                switch (stateReason)
                {
                    case 0:
                        state = "LoggedOut";
                        break;
                    case 23:
                        state = "LoggedOut";
                        break;
                    case 8:
                        state = "NotReady";
                        break;
                    case 4:
                        state = "Ready";
                        break;
                    case 2:
                        state = "LoggedIn";
                        break;
                    case 6:
                        state = "OnDialing";
                        break;
                    case 7:
                        state = "OnRinging";
                        break;
                    case 13:
                        state = "OnHeld";
                        break;
                    case 20:
                        state = "OnCall";
                        break;
                    case 19:
                        state = "OnCall";
                        break;
                    case 21:
                        state = "OnCall";
                        break;
                    case 22:
                        state = "OnCall";
                        break;
                    case 18:
                        state = "OnCall";
                        break;
                }
            }
            catch (Exception GeneralException)
            {
            }
            return state;
        }

        /// <summary>
        /// Gets the status.
        /// </summary>
        /// <param name="state">The state.</param>
        private void GetStatus(int state)
        {
            try
            {
                logger.Debug("StatisticsBase : GetStatus Method : Entry");
                switch (state)
                {
                    case 0:
                        statInfo.AgentStatus = "LoggedOut";
                        break;
                    case 23:
                        statInfo.AgentStatus = "LoggedOut";
                        break;                        
                    case 9:
                        statInfo.AgentStatus = "ACW";
                        break;
                    case 8:
                        statInfo.AgentStatus = "NotReady";
                        break;
                    case 4:
                        statInfo.AgentStatus = "Ready";
                        break;
                    case 2:
                        statInfo.AgentStatus = "LoggedIn";
                        break;
                    case 6:
                        statInfo.AgentStatus = "OnDialing";
                        break;
                    case 7:
                        statInfo.AgentStatus = "OnRinging";
                        break;
                    case 13:
                        statInfo.AgentStatus = "OnHeld";
                        break;
                    case 20:
                        statInfo.AgentStatus = "OnCall";
                        break;
                    case 19:
                        statInfo.AgentStatus = "OnCall";
                        break;
                    case 21:
                        statInfo.AgentStatus = "OnCall";
                        break;
                    case 22:
                        statInfo.AgentStatus = "OnCall";
                        break;
                    case 18:
                        statInfo.AgentStatus = "OnCall";
                        break;
                }
            }
            catch (Exception generalException)
            {
                logger.Error("StatisticsBase : GetStatus Method : " + generalException.Message);
            }
            finally
            {
                GC.Collect();
                logger.Debug("StatisticsBase : GetStatus Method : Exit");
            }
        }

        /// <summary>
        /// Fills the ACD queue values.
        /// </summary>
        /// <param name="_eventInfo">The event info.</param>
        private void NotifyACDQueueValues(int refId, string stringValue)
        {
            //var _eventInfo = eventInfo as EventInfo;
            string Value;
            string format;
            ThresholdSettings getColor = new ThresholdSettings();
            try
            {
                logger.Debug("StatisticsBase : NotifyACDQueueValues Method : Entry");
                if (StatisticsSetting.GetInstance().ACDStatisticsValueHolder.ContainsKey(refId))
                {
                    statInfo = (StatVariables)StatisticsSetting.GetInstance().ACDStatisticsValueHolder[refId];
                }

                Value = stringValue;
                format = statInfo.Statformat;

                #region Value Conversion

                if (string.Compare(format, "t", true) == 0)
                {
                    try
                    {
                        string Hours = string.Empty;
                        string Minutes = string.Empty;

                        Decimal dec = (Value != null ? decimal.Parse(Value, System.Globalization.NumberStyles.Any) : 0);

                        TimeSpan span = TimeSpan.FromSeconds(Convert.ToInt64(dec));
                        if (StatisticsSetting.GetInstance().statisticsCollection.ApplicationContainer.EnableHHMMSS)
                        {
                            Value = span.ToString();
                        }
                        else
                        {
                            Hours = span.Hours.ToString().Length > 1 ? span.Hours.ToString() : "0" + span.Hours.ToString();
                            Minutes = span.Minutes.ToString().Length > 1 ? span.Minutes.ToString() : "0" + span.Minutes.ToString();

                            Value = Hours + ":" + Minutes;

                        }

                        statInfo.StatValue = Value;
                        StatisticsSetting.GetInstance().ACDStatisticsValueHolder[refId] = statInfo;

                    }
                    catch (Exception generalException)
                    {
                        logger.Error("StatisticsBase : NotifyACDQueueValues Method : " + generalException.Message);
                    }
                }
                else if (string.Compare(format, "d", true) == 0)
                {
                    Value = Convert.ToDouble(Value).ToString();

                    statInfo.StatValue = Value;
                    StatisticsSetting.GetInstance().ACDStatisticsValueHolder[refId] = statInfo;

                }
                else if (string.Compare(format, "p", true) == 0)
                {
                    try
                    {
                        Decimal decimeal = decimal.Parse(Value, System.Globalization.NumberStyles.Any);
                        Value = decimeal.ToString("n");

                        statInfo.StatValue = Value;
                        StatisticsSetting.GetInstance().ACDStatisticsValueHolder[refId] = statInfo;
                    }
                    catch (Exception generalException)
                    {
                        logger.Error("StatisticsBase : NotifyACDQueueValues Method : " + generalException.Message);
                    }
                }

                #endregion

                logger.Info("StatisticsBase : NotifyACDQueueValues Method : Server Response : " + "ReferenceId : " + statInfo.ReferenceId);
                logger.Info("StatisticsBase : NotifyACDQueueValues Method : Server Response : " + "StatName : " + statInfo.DisplayName);
                logger.Info("StatisticsBase : NotifyACDQueueValues Method : Server Response : " + "StatValue : " + statInfo.StatValue);
                logger.Info("StatisticsBase : NotifyACDQueueValues Method : Server Response : " + "StatType : " + statInfo.StatType);

                #region Notifier

                if (!StatisticsSetting.GetInstance().ACDStatisticsPluginHolder.ContainsKey(refId))
                {
                    StatisticsSetting.GetInstance().ACDStatisticsPluginHolder.Add(refId, statInfo);
                }

                foreach (KeyValuePair<string, IStatTicker> messageToClient in ToClient)
                {
                    if (messageToClient.Key == "StatTickerFive")
                    {
                        messageToClient.Value.NotifyQueueStatistics(statInfo.ReferenceId, statInfo.DisplayName, statInfo.StatValue, statInfo.ObjectID + "\n" + statInfo.Tooltip, getColor.ThresholdColor(statInfo.StatValue, statInfo.ObjectID, refId.ToString(), format), statInfo.StatType, StatisticsSetting.GetInstance().isThresholdBreach, StatisticsSetting.GetInstance().isLevelTwo);
                    }
                    else if (messageToClient.Key == "AID")
                    {
                        messageToClient.Value.NotifyQueueStatistics(statInfo.ReferenceId, statInfo.DisplayName, statInfo.StatValue, statInfo.ObjectID + "\n" + statInfo.Tooltip, getColor.ThresholdColor(statInfo.StatValue, statInfo.ObjectID, refId.ToString(), format), StatisticsEnum.ObjectType.ACDQueue.ToString(), StatisticsSetting.GetInstance().isThresholdBreach, StatisticsSetting.GetInstance().isLevelTwo);
                    }
                }

                #endregion
            }
            catch (Exception generalException)
            {
                logger.Error("StatisticsBase : NotifyACDQueueValues Method : " + generalException.Message);
            }
            finally
            {
                GC.Collect();
                Value = null;
                format = null;
                logger.Debug("StatisticsBase : NotifyACDQueueValues Method : Exit");
            }
        }

        private void NotifyAdminValues(object eventInfo)
        {
            var _eventInfo = eventInfo as EventInfo;
            string status = string.Empty;
            try
            {
                if (_eventInfo.StateValue != null)
                {

                    #region AgentGlobalStatus

                    AgentReasons agentReasons = _eventInfo.StateValue as AgentReasons;

                    if (agentReasons != null)
                    {
                        KeyValueCollection reasonKey = agentReasons.Reasons;
                        if (agentReasons.Reasons.Count > 0)
                        {
                            foreach (var _key in reasonKey.Keys)
                            {
                                string reason = reasonKey[_key.ToString()].ToString();
                                if (!string.IsNullOrEmpty(reason))
                                {
                                    if (reason == "After Call Work")
                                    {
                                        status = "NR - ACW";
                                    }
                                    else if (reason == "AfterCallWork")
                                    {
                                        status = "NR - ACW";
                                    }
                                    else
                                    {
                                        status = "NR - " + reason.ToString();
                                    }
                                }
                            }
                        }
                        else
                        {
                            status = GetState(Convert.ToInt16(agentReasons.Status));
                        }

                    }

                    #endregion

                    #region Notifier

                    foreach (KeyValuePair<string, IStatTicker> messageToClient in ToClient)
                    {
                        if (messageToClient.Key == "STFAdmin")
                        {
                            messageToClient.Value.NotifyAgentStatus(agentReasons.AgentId, status);
                        }
                    }

                    #endregion
                }
            }
            catch (Exception GeneralException)
            {
            }
        }

        /// <summary>
        /// Fills the agent group values.
        /// </summary>
        /// <param name="_eventInfo">The event info.</param>
        private void NotifyAgentGroupValues(int refId, string stringValue)
        {
            //var _eventInfo = eventInfo as EventInfo;
            string Value;
            string format;
            ThresholdSettings getColor = new ThresholdSettings();
            try
            {
                logger.Debug("StatisticsBase : NotifyAgentGroupValues Method : Entry");
                if (StatisticsSetting.GetInstance().agentGroupStatsValueHolder.ContainsKey(refId))
                {
                    statInfo = (StatVariables)StatisticsSetting.GetInstance().agentGroupStatsValueHolder[refId];
                }

                Value = stringValue;
                format = statInfo.Statformat;

                #region Value Conversion

                if (string.Compare(format, "t", true) == 0)
                {
                    try
                    {
                        string Hours = string.Empty;
                        string Minutes = string.Empty;

                        Decimal dec = (Value != null ? decimal.Parse(Value, System.Globalization.NumberStyles.Any) : 0);

                        TimeSpan span = TimeSpan.FromSeconds(Convert.ToInt64(dec));
                        if (StatisticsSetting.GetInstance().statisticsCollection.ApplicationContainer.EnableHHMMSS)
                        {
                            Value = span.ToString();
                        }
                        else
                        {
                            Hours = span.Hours.ToString().Length > 1 ? span.Hours.ToString() : "0" + span.Hours.ToString();
                            Minutes = span.Minutes.ToString().Length > 1 ? span.Minutes.ToString() : "0" + span.Minutes.ToString();

                            Value = Hours + ":" + Minutes;
                        }

                        statInfo.StatValue = Value;
                        StatisticsSetting.GetInstance().agentGroupStatsValueHolder[refId] = statInfo;

                    }
                    catch (Exception generalException)
                    {
                        logger.Error("StatisticsBase : NotifyAgentGroupValues Method : " + generalException.Message);
                    }
                }
                else if (string.Compare(format, "d", true) == 0)
                {
                    Value = Convert.ToDouble(Value).ToString();

                    statInfo.StatValue = Value;
                    StatisticsSetting.GetInstance().agentGroupStatsValueHolder[refId] = statInfo;

                }
                else if (string.Compare(format, "p", true) == 0)
                {
                    try
                    {
                        Decimal decimeal = decimal.Parse(Value, System.Globalization.NumberStyles.Any);
                        Value = decimeal.ToString("n");

                        statInfo.StatValue = Value;
                        StatisticsSetting.GetInstance().agentGroupStatsValueHolder[refId] = statInfo;
                    }
                    catch (Exception generalException)
                    {
                        logger.Error("StatisticsBase : NotifyAgentGroupValues Method : " + generalException.Message);
                    }
                }

                #endregion

                logger.Info("StatisticsBase : NotifyAgentGroupValues : Server Response : " + "ReferenceId : " + statInfo.ReferenceId);
                logger.Info("StatisticsBase : NotifyAgentGroupValues : Server Response : " + "StatName : " + statInfo.DisplayName);
                logger.Info("StatisticsBase : NotifyAgentGroupValues : Server Response : " + "StatValue : " + statInfo.StatValue);
                logger.Info("StatisticsBase : NotifyAgentGroupValues : Server Response : " + "StatType : " + statInfo.StatType);

                #region Notifier

                if (!StatisticsSetting.GetInstance().agentGroupStatsPluginHolder.ContainsKey(refId))
                {
                    StatisticsSetting.GetInstance().agentGroupStatsPluginHolder.Add(refId, statInfo);
                }

                foreach (KeyValuePair<string, IStatTicker> messageToClient in ToClient)
                {
                    if (messageToClient.Key == "StatTickerFive" || messageToClient.Key == "AID")
                    {
                        messageToClient.Value.NotifyAgentGroupStatistics(statInfo.ReferenceId, statInfo.DisplayName, statInfo.StatValue, statInfo.ObjectID + "\n" + statInfo.Tooltip, getColor.ThresholdColor(statInfo.StatValue, statInfo.ObjectID, refId.ToString(), format), statInfo.StatType, StatisticsSetting.GetInstance().isThresholdBreach, StatisticsSetting.GetInstance().isLevelTwo);
                    }
                }

                #endregion

            }
            catch (Exception generalException)
            {
                logger.Error("StatisticsBase : NotifyAgentGroupValues Method : " + generalException.Message);
            }
            finally
            {
                Value = null;
                format = null;
                GC.Collect();
                logger.Debug("StatisticsBase : NotifyAgentGroupValues Method : Exit");
            }
        }

        /// <summary>
        /// Fills the agent values.
        /// </summary>
        /// <param name="_eventInfo">The event info.</param>
        private void NotifyAgentValues(int refId, string stringValue, object eventInfo)
        {
            var _eventInfo = eventInfo as EventInfo;
            string Value;
            string format;
            ThresholdSettings getColor = new ThresholdSettings();
            try
            {
                logger.Debug("StatisticsBase : NotifyAgentValues Method : Entry");
                if (StatisticsSetting.GetInstance().agentStatisticsPluginHolder.ContainsKey(refId))
                {
                    statInfo = (StatVariables)StatisticsSetting.GetInstance().agentStatisticsPluginHolder[refId];
                }

                Value = stringValue;
                format = statInfo.Statformat;

                #region Value Conversion

                if (string.Compare(statInfo.Statformat, "t", true) == 0)
                {
                    try
                    {
                        string Hours = string.Empty;
                        string Minutes = string.Empty;

                        Decimal dec = (Value != null ? decimal.Parse(Value, System.Globalization.NumberStyles.Any) : 0);

                        TimeSpan span = TimeSpan.FromSeconds(Convert.ToInt64(dec));

                        if (StatisticsSetting.GetInstance().statisticsCollection.ApplicationContainer.EnableHHMMSS)
                        {
                            Value = span.ToString();
                        }
                        else
                        {
                            Hours = span.Hours.ToString().Length > 1 ? span.Hours.ToString() : "0" + span.Hours.ToString();
                            Minutes = span.Minutes.ToString().Length > 1 ? span.Minutes.ToString() : "0" + span.Minutes.ToString();

                            Value = Hours + ":" + Minutes;
                        }

                        statInfo.StatValue = Value;
                        StatisticsSetting.GetInstance().agentStatisticsValueHolder[refId] = statInfo;

                    }
                    catch (Exception generalException)
                    {
                        logger.Error("StatisticsBase : NotifyAgentValues Method : " + generalException.Message);
                    }
                }
                else if (string.Compare(statInfo.Statformat, "d", true) == 0)
                {
                    Value = Convert.ToDouble(Value).ToString();

                    statInfo.StatValue = Value;
                    StatisticsSetting.GetInstance().agentStatisticsValueHolder[refId] = statInfo;

                }
                #region AgentStatus

                else if (string.Compare(statInfo.Statformat, "s", true) == 0)
                {
                    try
                    {
                        if (_eventInfo.StateValue != null)
                        {
                            AgentReasons agentStatus = (AgentReasons)_eventInfo.StateValue;
                            var reasonKey = agentStatus.Reasons;
                            logger.Debug("StatisticsBase : NotifyAgentValues Method : Agent Reasons" + reasonKey);
                            if (reasonKey.Count > 0)
                            {                                
                                var reason = "";
                                if (String.IsNullOrEmpty(StatisticsSetting.GetInstance().NotReadyReasonKeyName) ||
                                    !reasonKey.ContainsKey(StatisticsSetting.GetInstance().NotReadyReasonKeyName))
                                    reason = reasonKey[reasonKey.AllKeys[0]].ToString();
                                else
                                    reason = reasonKey[StatisticsSetting.GetInstance().NotReadyReasonKeyName].ToString();

                                if (!string.IsNullOrEmpty(reason))
                                {
                                    if (reason.Length > 6)
                                    {
                                        statInfo.StatValue = "NR - " + reason.Substring(0, 4) + "..";
                                    }
                                    else
                                    {
                                        statInfo.StatValue = "NR - " + reason.ToString();
                                    }
                                }
                            }
                            else
                            {
                                GetStatus(Convert.ToInt16(agentStatus.Status));
                                statInfo.StatValue = statInfo.AgentStatus.ToString();
                            }
                            logger.Debug("StatisticsBase : NotifyAgentValues Method : Agent NotReady Reasons" + statInfo.StatValue);
                        }

                    }
                    catch (Exception generalException)
                    {
                        logger.Error("StatisticsBase : NotifyAgentValues Method : " + generalException.Message);
                    }
                }

                #endregion

                #endregion

                logger.Info("StatisticsBase : NotifyAgentValues Method : Server Response : " + "ReferenceId : " + statInfo.ReferenceId);
                logger.Info("StatisticsBase : NotifyAgentValues Method : Server Response : " + "StatName : " + statInfo.DisplayName);
                logger.Info("StatisticsBase : NotifyAgentValues Method : Server Response : " + "StatValue : " + statInfo.StatValue);
                logger.Info("StatisticsBase : NotifyAgentValues Method : Server Response : " + "StatType : " + statInfo.StatType);

                #region Notifier

                if (!StatisticsSetting.GetInstance().agentStatisticsPluginHolder.ContainsKey(refId))
                {
                    StatisticsSetting.GetInstance().agentStatisticsPluginHolder.Add(refId, statInfo);
                }

                foreach (KeyValuePair<string, IStatTicker> messageToClient in ToClient)
                {
                    if (messageToClient.Key == "StatTickerFive" || messageToClient.Key == "AID" || messageToClient.Key == "STFAdmin")
                    {
                        messageToClient.Value.NotifyAgentStatistics(statInfo.ReferenceId, statInfo.DisplayName,
                            statInfo.StatValue, statInfo.ObjectID + "\n" + statInfo.Tooltip,
                            getColor.ThresholdColor(statInfo.StatValue, statInfo.ObjectID, refId.ToString(), format),
                            statInfo.StatType, StatisticsSetting.GetInstance().isThresholdBreach, StatisticsSetting.GetInstance().isLevelTwo);
                    }
                }

                #endregion

            }
            catch (Exception generalException)
            {
                logger.Error("StatisticsBase : NotifyAgentValues Method : " + generalException.Message);
            }
            finally
            {
                Value = null;
                format = null;
                GC.Collect();
                logger.Debug("StatisticsBase : NotifyAgentValues Method : Exit");
            }
        }

        /// <summary>
        /// Notifies the aid statistics.
        /// </summary>
        /// <param name="_eventInfo">The event information.</param>
        private void NotifyAIDStatistics(object eventInfo)
        {
            var _eventInfo = eventInfo as EventInfo;
            try
            {
                logger.Debug("StatisticsBase : NotifyAIDStatistics Method : Entry");

                foreach (KeyValuePair<string, IStatTicker> messageToClient in ToClient)
                {
                    if (messageToClient.Key == "AID")
                    {
                        messageToClient.Value.NotifyAIDStatistics(_eventInfo);
                    }
                }

                if (_eventInfo.ReferenceId < StatisticsSetting.GetInstance().SecondLimit)
                {
                    RequestCloseStatistic closeStat = RequestCloseStatistic.Create(_eventInfo.ReferenceId);
                    settings.statServerProtocol.Request(closeStat);
                }
            }
            catch (Exception GeneralException)
            {
                logger.Error("StatisticsBase : NotifyAIDStatistics Method : Exception Caught : " + GeneralException.Message);
            }
            logger.Debug("StatisticsBase : NotifyAIDStatistics Method : Exit");
        }

        /// <summary>
        /// Fills the DN group values.
        /// </summary>
        /// <param name="_eventInfo">The event info.</param>
        private void NotifyDNGroupValues(int refId, string stringValue)
        {
            //MessageBox.Show("dnresponse=");
            //var _eventInfo = eventInfo as EventInfo;
            string Value;
            string format;
            ThresholdSettings getColor = new ThresholdSettings();
            try
            {
                logger.Debug("StatisticsBase : NotifyDNGroupValues Method : Entry");
                if (StatisticsSetting.GetInstance().dnGroupStatisticsValueHolder.ContainsKey(refId))
                {
                    statInfo = (StatVariables)StatisticsSetting.GetInstance().dnGroupStatisticsValueHolder[refId];
                }

                Value = stringValue;
                format = statInfo.Statformat;

                #region Value Conversion

                if (string.Compare(format, "t", true) == 0)
                {
                    try
                    {
                        string Hours = string.Empty;
                        string Minutes = string.Empty;

                        Decimal dec = (Value != null ? decimal.Parse(Value, System.Globalization.NumberStyles.Any) : 0);

                        TimeSpan span = TimeSpan.FromSeconds(Convert.ToInt64(dec));

                        if (StatisticsSetting.GetInstance().statisticsCollection.ApplicationContainer.EnableHHMMSS)
                        {
                            Value = span.ToString();
                        }
                        else
                        {
                            Hours = span.Hours.ToString().Length > 1 ? span.Hours.ToString() : "0" + span.Hours.ToString();
                            Minutes = span.Minutes.ToString().Length > 1 ? span.Minutes.ToString() : "0" + span.Minutes.ToString();

                            Value = Hours + ":" + Minutes;
                        }

                        statInfo.StatValue = Value;
                        StatisticsSetting.GetInstance().dnGroupStatisticsValueHolder[refId] = statInfo;

                    }
                    catch (Exception generalException)
                    {
                        logger.Error("StatisticsBase : NotifyDNGroupValues Method : " + generalException.Message);
                    }
                }
                else if (string.Compare(format, "d", true) == 0)
                {
                    Value = Convert.ToDouble(Value).ToString();

                    statInfo.StatValue = Value;
                    StatisticsSetting.GetInstance().dnGroupStatisticsValueHolder[refId] = statInfo;

                }
                else if (string.Compare(format, "p", true) == 0)
                {
                    try
                    {
                        Decimal decimeal = decimal.Parse(Value, System.Globalization.NumberStyles.Any);
                        Value = decimeal.ToString("n");

                        statInfo.StatValue = Value;
                        StatisticsSetting.GetInstance().dnGroupStatisticsValueHolder[refId] = statInfo;
                    }
                    catch (Exception generalException)
                    {
                        logger.Error("StatisticsBase : NotifyDNGroupValues Method : " + generalException.Message);
                    }
                }

                #endregion

                logger.Info("StatisticsBase : NotifyDNGroupValues Method : Server Response : " + "ReferenceId : " + statInfo.ReferenceId);
                logger.Info("StatisticsBase : NotifyDNGroupValues Method : Server Response : " + "StatName : " + statInfo.DisplayName);
                logger.Info("StatisticsBase : NotifyDNGroupValues Method : Server Response : " + "StatValue : " + statInfo.StatValue);
                logger.Info("StatisticsBase : NotifyDNGroupValues Method : Server Response : " + "StatType : " + statInfo.StatType);

                #region Notifier

                if (!StatisticsSetting.GetInstance().dnGroupStatisticsPluginHolder.ContainsKey(refId))
                {
                    StatisticsSetting.GetInstance().dnGroupStatisticsPluginHolder.Add(refId, statInfo);
                }

                foreach (KeyValuePair<string, IStatTicker> messageToClient in ToClient)
                {
                    if (messageToClient.Key == "StatTickerFive")
                    {
                        messageToClient.Value.NotifyQueueStatistics(statInfo.ReferenceId, statInfo.DisplayName, statInfo.StatValue, statInfo.ObjectID + "\n" + statInfo.Tooltip, getColor.ThresholdColor(statInfo.StatValue, statInfo.ObjectID, refId.ToString(), format), statInfo.StatType, StatisticsSetting.GetInstance().isThresholdBreach, StatisticsSetting.GetInstance().isLevelTwo);
                    }
                    else if (messageToClient.Key == "AID")
                    {
                        messageToClient.Value.NotifyQueueStatistics(statInfo.ReferenceId, statInfo.CCStat, statInfo.StatValue, statInfo.ObjectID + "\n" + statInfo.Tooltip, getColor.ThresholdColor(statInfo.StatValue, statInfo.ObjectID, refId.ToString(), format), StatisticsEnum.ObjectType.DNGroup.ToString(), StatisticsSetting.GetInstance().isThresholdBreach, StatisticsSetting.GetInstance().isLevelTwo);
                    }
                }

                #endregion

            }
            catch (Exception generalException)
            {
                logger.Error("StatisticsBase : NotifyDNGroupValues Method: " + generalException.Message);
            }
            finally
            {
                GC.Collect();
                Value = null;
                format = null;
                logger.Debug("StatisticsBase : NotifyDNGroupValues Method : Exit");
            }
        }

        private void NotifyTeamCommValues(object eventInfo)
        {
            var _eventInfo = eventInfo as EventInfo;
            try
            {
                logger.Debug("StatisticsBase : NotifyTeamCommValues Method : Entry");

                foreach (KeyValuePair<string, IStatTicker> messageToClient in ToClient)
                {
                    if (messageToClient.Key == "AID")
                    {
                        messageToClient.Value.NotifyAIDStatistics(_eventInfo);
                    }
                }
            }
            catch (Exception GeneralException)
            {
                logger.Error("StatisticsBase : NotifyTeamCommValues Method : Exception Caught : " + GeneralException.Message);
            }
            logger.Debug("StatisticsBase : NotifyTeamCommValues Method : Exit");
        }

        /// <summary>
        /// Fills the V queue values.
        /// </summary>
        /// <param name="_eventInfo">The event info.</param>
        private void NotifyVQueueValues(int refId, string stringValue)
        {
            //MessageBox.Show("vqresponse=");
            //var _eventInfo = eventInfo as EventInfo;
            string Value;
            string format;
            ThresholdSettings getColor = new ThresholdSettings();
            try
            {
                logger.Debug("StatisticsBase : NotifyVQueueValues Method : Entry");
                if (StatisticsSetting.GetInstance().VQStatisticsValueHolder.ContainsKey(refId))
                {
                    statInfo = (StatVariables)StatisticsSetting.GetInstance().VQStatisticsValueHolder[refId];
                }

                Value = stringValue;
                format = statInfo.Statformat;

                #region Value Conversion

                if (string.Compare(format, "t", true) == 0)
                {
                    try
                    {
                        string Hours = string.Empty;
                        string Minutes = string.Empty;

                        Decimal dec = (Value != null ? decimal.Parse(Value, System.Globalization.NumberStyles.Any) : 0);

                        TimeSpan span = TimeSpan.FromSeconds(Convert.ToInt64(dec));

                        if (StatisticsSetting.GetInstance().statisticsCollection.ApplicationContainer.EnableHHMMSS)
                        {
                            Value = span.ToString();
                        }
                        else
                        {
                            Hours = span.Hours.ToString().Length > 1 ? span.Hours.ToString() : "0" + span.Hours.ToString();
                            Minutes = span.Minutes.ToString().Length > 1 ? span.Minutes.ToString() : "0" + span.Minutes.ToString();

                            Value = Hours + ":" + Minutes;
                        }

                        statInfo.StatValue = Value;
                        StatisticsSetting.GetInstance().VQStatisticsValueHolder[refId] = statInfo;

                    }
                    catch (Exception generalException)
                    {
                        logger.Error("StatisticsBase : NotifyVQueueValues Method: " + generalException.Message);
                    }
                }
                else if (string.Compare(format, "d", true) == 0)
                {
                    Value = Convert.ToDouble(Value).ToString();

                    statInfo.StatValue = Value;
                    StatisticsSetting.GetInstance().VQStatisticsValueHolder[refId] = statInfo;

                }
                else if (string.Compare(format, "p", true) == 0)
                {
                    try
                    {
                        Decimal decimeal = decimal.Parse(Value, System.Globalization.NumberStyles.Any);
                        Value = decimeal.ToString("n");

                        statInfo.StatValue = Value;
                        StatisticsSetting.GetInstance().VQStatisticsValueHolder[refId] = statInfo;
                    }
                    catch (Exception generalException)
                    {
                        logger.Error("StatisticsBase : NotifyVQueueValues Method: " + generalException.Message);
                    }
                }

                #endregion

                logger.Info("StatisticsBase : NotifyVQueueValues : Server Response : " + "ReferenceId : " + statInfo.ReferenceId);
                logger.Info("StatisticsBase : NotifyVQueueValues : Server Response : " + "StatName : " + statInfo.DisplayName);
                logger.Info("StatisticsBase : NotifyVQueueValues : Server Response : " + "StatValue : " + statInfo.StatValue);
                logger.Info("StatisticsBase : NotifyVQueueValues : Server Response : " + "StatType : " + statInfo.StatType);

                #region Notifier

                if (!StatisticsSetting.GetInstance().VQStatisticsPlugineHolder.ContainsKey(refId))
                {
                    StatisticsSetting.GetInstance().VQStatisticsPlugineHolder.Add(refId, statInfo);
                }

                foreach (KeyValuePair<string, IStatTicker> messageToClient in ToClient)
                {
                    if (messageToClient.Key == "StatTickerFive")
                    {
                        messageToClient.Value.NotifyQueueStatistics(statInfo.ReferenceId, statInfo.DisplayName, statInfo.StatValue, statInfo.ObjectID + "\n" + statInfo.Tooltip, getColor.ThresholdColor(statInfo.StatValue, statInfo.ObjectID, refId.ToString(), format), statInfo.StatType, StatisticsSetting.GetInstance().isThresholdBreach, StatisticsSetting.GetInstance().isLevelTwo);
                    }
                    else if (messageToClient.Key == "AID")
                    {
                        messageToClient.Value.NotifyQueueStatistics(statInfo.ReferenceId, statInfo.CCStat, statInfo.StatValue, statInfo.ObjectID + "\n" + statInfo.Tooltip, getColor.ThresholdColor(statInfo.StatValue, statInfo.ObjectID, refId.ToString(), format), StatisticsEnum.ObjectType.VirtualQueue.ToString(), StatisticsSetting.GetInstance().isThresholdBreach, StatisticsSetting.GetInstance().isLevelTwo);
                    }
                }

                #endregion
            }
            catch (Exception generalException)
            {
                logger.Error("StatisticsBase : NotifyVQueueValues Method: " + generalException.Message);
            }
            finally
            {
                GC.Collect();
                Value = null;
                format = null;
                logger.Debug("StatisticsBase : NotifyVQueueValues Method: Exit");
            }
        }

        /// <summary>
        /// Reads all statistics.
        /// </summary>
        /// <returns></returns>
        private List<string> ReadAllStatistics(string applicationName)
        {
            List<string> _listAllStatistics = new List<string>();

            string statServerName = string.Empty;
            try
            {
                logger.Debug("StatisticsBase : ReadAllStatistics Method : Entry");
                if (!applicationName.Equals(string.Empty))
                {
                    CfgApplicationQuery applicationQuery = new CfgApplicationQuery();
                    applicationQuery.Name = applicationName;
                    CfgApplication application = StatisticsSetting.GetInstance().confObject.RetrieveObject<CfgApplication>(applicationQuery);
                    if (application != null)
                    {
                        if (application.AppServers != null && application.AppServers.Count != 0)
                        {
                            logger.Info("StatisticsBase : ReadAllStatistics Method : Application servers count" + application.AppServers.Count.ToString());

                            foreach (CfgConnInfo appDetails in application.AppServers)
                            {
                                if (appDetails.AppServer.Type == CfgAppType.CFGStatServer)
                                {
                                    statServerName = appDetails.AppServer.Name.ToString().Trim();
                                }
                            }
                            CfgApplicationQuery appQuery = new CfgApplicationQuery();
                            appQuery.Name = statServerName;
                            CfgApplication app = StatisticsSetting.GetInstance().confObject.RetrieveObject<CfgApplication>(appQuery);
                            if (app != null)
                            {
                                string[] appKeys = app.Options.AllKeys;
                                foreach (string key in appKeys)
                                {
                                    _listAllStatistics.Add(key);
                                }
                            }
                        }
                        logger.Info("StatisticsBase : ReadAllStatistics Method : Application servers count" + application.AppServers.Count.ToString());
                    }
                }
            }
            catch (Exception generalException)
            {
                logger.Error("StatisticsBase : ReadAllStatistics Method : Exception Caught : " + generalException.Message);
            }
            finally
            {
                GC.Collect();
                logger.Debug("StatisticsBase : ReadAllStatistics Method : Exit");
            }

            return _listAllStatistics;
        }

        /// <summary>
        /// Reads all switches.
        /// </summary>
        /// <returns></returns>
        private List<string> ReadAllSwitches()
        {
            List<string> _listAllSwitches = new List<string>();
            try
            {
                logger.Debug("StatisticsBase : ReadAllSwitched Method : Entry");
                CfgSwitchQuery switchQuery = new CfgSwitchQuery();
                switchQuery.TenantDbid = StatisticsSetting.GetInstance().CFGTenantDBID;
                switchQuery["objecttype"] = CfgObjectType.CFGSwitch;
                IList<CfgSwitch> _listSwitches = StatisticsSetting.GetInstance().confObject.RetrieveMultipleObjects<CfgSwitch>(switchQuery) as IList<CfgSwitch>;
                if (_listSwitches != null)
                {
                    if (_listSwitches.Count > 0)
                    {
                        logger.Info("StatisticsBase : ReadAllSwitches Method : Switch count" + _listSwitches.Count.ToString());

                        foreach (CfgSwitch _switch in _listSwitches)
                        {
                            _listAllSwitches.Add(_switch.Name.ToString().Trim());
                        }
                    }

                    logger.Info("StatisticsBase : ReadAllSwitches Method : Switch count" + _listSwitches.Count.ToString());
                }
            }
            catch (Exception generalException)
            {
                logger.Debug("StatisticsBase : ReadAllSwitches Method : Exception Caught : " + generalException.Message);
            }
            finally
            {
                GC.Collect();
                logger.Debug("StatisticsBase : ReadAllSwitches Method : Exit");
            }
            return _listAllSwitches;
        }

        /// <summary>
        /// Saves the objects.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <param name="appDetails">The app details.</param>
        /// <returns></returns>
        private KeyValueCollection SaveObjects(Dictionary<string, List<string>> values, KeyValueCollection appDetails)
        {
            List<string> lstACDobjects = new List<string>();
            List<string> lstDNobjects = new List<string>();
            List<string> lstVQobjects = new List<string>();
            try
            {
                logger.Debug("StatisticsBase : SaveObjects Method : Entry");

                foreach (string objectType in values.Keys)
                {
                    if (string.Compare(objectType, StatisticsEnum.ObjectType.ACDQueue.ToString(), true) == 0)
                        lstACDobjects = values[objectType];
                    else if (string.Compare(objectType, StatisticsEnum.ObjectType.DNGroup.ToString(), true) == 0)
                        lstDNobjects = values[objectType];
                    else if (string.Compare(objectType, StatisticsEnum.ObjectType.VirtualQueue.ToString(), true) == 0)
                        lstVQobjects = values[objectType];

                }

                string ACDTempValue = string.Empty;
                string DNTempValue = string.Empty;
                string VQTempValue = string.Empty;

                if (appDetails == null)
                {
                    KeyValueCollection kvps = new KeyValueCollection();

                    kvps.Add("statistics.objects-acd-queues", ACDTempValue);
                    kvps.Add("statistics.objects-dn-groups", DNTempValue);
                    kvps.Add("statistics.objects-virtual-queues", VQTempValue);

                    appDetails = kvps;
                }

                #region ACD Objects

                logger.Info("StatisticsBase : SaveObjects Method : Saving ACD Objects");

                foreach (string stat in lstACDobjects)
                {
                    if (string.IsNullOrEmpty(ACDTempValue))
                        ACDTempValue = stat;
                    else
                        ACDTempValue = ACDTempValue + "," + stat;
                }

                string[] ACDStatistics = ACDTempValue.Split(',');

                #region Overwrite ACD Statistics

                string tempStat = string.Empty;
                string kvpname = "statistics.objects-acd-queues";
                bool issaved = false;
                int kvpno = 0;

                List<string> kvpremoved = new List<string>();
                foreach (string keys in appDetails.Keys)
                {
                    if (keys.Contains(kvpname))
                    {
                        kvpremoved.Add(keys);
                    }
                }

                foreach (string removedkvp in kvpremoved)
                {
                    appDetails.Remove(removedkvp);
                }

                if (ACDStatistics.Length > 10)
                {
                    int remainstats = 10;

                    for (int i = 10; i <= ACDStatistics.Length; )
                    {
                        tempStat = string.Empty;
                        for (int j = i - remainstats; j < i; j++)
                        {
                            if (tempStat == string.Empty)
                                tempStat = ACDStatistics[j];
                            else
                                tempStat = tempStat + "," + ACDStatistics[j];
                        }

                        kvpno++;
                        string tempKVPname = kvpname + "_" + kvpno.ToString();
                        if (appDetails.ContainsKey(tempKVPname))
                        {
                            appDetails.Remove(tempKVPname);
                            appDetails[tempKVPname] = tempStat;
                        }
                        else
                        {
                            if (tempStat != "" && tempStat != null && tempStat != string.Empty)
                                appDetails.Add(tempKVPname, tempStat);
                        }
                        if (issaved)
                        {
                            i = i + 10;
                            break;
                        }

                        if (i + 10 > ACDStatistics.Length)
                        {
                            remainstats = ACDStatistics.Length - i;
                            i = i + (ACDStatistics.Length - i);
                            issaved = true;
                        }
                        else
                            i = i + 10;
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(ACDStatistics[0].ToString()))
                    {
                        if (appDetails.ContainsKey("statistics.objects-acd-queues"))
                        {
                            appDetails["statistics.objects-acd-queues"] = ACDTempValue;
                        }
                        else
                        {
                            appDetails.Add("statistics.objects-acd-queues", ACDTempValue);
                        }
                    }
                    else
                    {
                        appDetails.Add("statistics.objects-acd-queues", "");
                    }
                }

                #endregion

                #endregion

                #region DNGroup Objects

                logger.Info("StatisticsBase : SaveObjects Method : Saving DNGroup Objects");

                foreach (string stat in lstDNobjects)
                {
                    if (string.IsNullOrEmpty(DNTempValue))
                        DNTempValue = stat;
                    else
                        DNTempValue = DNTempValue + "," + stat;
                }
                string[] DNStatistics = DNTempValue.Split(',');

                #region Overwrite DNGroup Statistics

                tempStat = string.Empty;
                kvpname = "statistics.objects-dn-groups";
                issaved = false;
                kvpno = 0;

                kvpremoved = new List<string>();
                foreach (string keys in appDetails.Keys)
                {
                    if (keys.Contains(kvpname))
                    {
                        kvpremoved.Add(keys);
                    }
                }

                foreach (string removedkvp in kvpremoved)
                {
                    appDetails.Remove(removedkvp);
                }

                if (DNStatistics.Length > 10)
                {
                    int remainstats = 10;

                    for (int i = 10; i <= DNStatistics.Length; )
                    {
                        tempStat = string.Empty;
                        for (int j = i - remainstats; j < i; j++)
                        {
                            if (tempStat == string.Empty)
                                tempStat = DNStatistics[j];
                            else
                                tempStat = tempStat + "," + DNStatistics[j];
                        }

                        kvpno++;
                        string tempKVPname = kvpname + "_" + kvpno.ToString();
                        if (appDetails.ContainsKey(tempKVPname))
                        {
                            appDetails.Remove(tempKVPname);
                            appDetails[tempKVPname] = tempStat;
                        }
                        else
                        {
                            if (tempStat != "" && tempStat != null && tempStat != string.Empty)
                                appDetails.Add(tempKVPname, tempStat);
                        }
                        if (issaved)
                        {
                            i = i + 10;
                            break;
                        }

                        if (i + 10 > DNStatistics.Length)
                        {
                            remainstats = DNStatistics.Length - i;
                            i = i + (DNStatistics.Length - i);
                            issaved = true;
                        }
                        else
                            i = i + 10;
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(DNStatistics[0].ToString()))
                    {
                        if (appDetails.ContainsKey("statistics.objects-dn-groups"))
                        {
                            appDetails["statistics.objects-dn-groups"] = DNTempValue;
                        }
                        else
                        {
                            appDetails.Add("statistics.objects-dn-groups", DNTempValue);
                        }
                    }
                    else
                    {
                        appDetails.Add("statistics.objects-dn-groups", "");
                    }
                }

                #endregion

                #endregion

                #region VQ Objects

                logger.Info("StatisticsBase : SaveObjects Method : Saving VQ Objects");

                foreach (string stat in lstVQobjects)
                {
                    if (string.IsNullOrEmpty(VQTempValue))
                        VQTempValue = stat;
                    else
                        VQTempValue = VQTempValue + "," + stat;
                }
                string[] VQStatistics = VQTempValue.Split(',');

                #region Overwrite VQ Statistics

                tempStat = string.Empty;
                kvpname = "statistics.objects-virtual-queues";
                issaved = false;
                kvpno = 0;

                kvpremoved = new List<string>();
                foreach (string keys in appDetails.Keys)
                {
                    if (keys.Contains(kvpname))
                    {
                        kvpremoved.Add(keys);
                    }
                }

                foreach (string removedkvp in kvpremoved)
                {
                    appDetails.Remove(removedkvp);
                }

                if (VQStatistics.Length > 10)
                {
                    int remainstats = 10;

                    for (int i = 10; i <= VQStatistics.Length; )
                    {
                        tempStat = string.Empty;
                        for (int j = i - remainstats; j < i; j++)
                        {
                            if (tempStat == string.Empty)
                                tempStat = VQStatistics[j];
                            else
                                tempStat = tempStat + "," + VQStatistics[j];
                        }

                        kvpno++;
                        string tempKVPname = kvpname + "_" + kvpno.ToString();
                        if (appDetails.ContainsKey(tempKVPname))
                        {
                            appDetails.Remove(tempKVPname);
                            appDetails[tempKVPname] = tempStat;
                        }
                        else
                        {
                            if (tempStat != "" && tempStat != null && tempStat != string.Empty)
                                appDetails.Add(tempKVPname, tempStat);
                        }
                        if (issaved)
                        {
                            i = i + 10;
                            break;
                        }

                        if (i + 10 > VQStatistics.Length)
                        {
                            remainstats = VQStatistics.Length - i;
                            i = i + (VQStatistics.Length - i);
                            issaved = true;
                        }
                        else
                            i = i + 10;
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(VQStatistics[0].ToString()))
                    {
                        if (appDetails.ContainsKey("statistics.objects-virtual-queues"))
                        {
                            appDetails["statistics.objects-virtual-queues"] = VQTempValue;
                        }
                        else
                        {
                            appDetails.Add("statistics.objects-virtual-queues", VQTempValue);
                        }
                    }
                    else
                    {
                        appDetails.Add("statistics.objects-virtual-queues", "");
                    }
                }

                #endregion

                #endregion
            }
            catch (Exception GeneralException)
            {
                logger.Error("StatisticsBase : SaveObjects Method : " + GeneralException.Message);
            }
            finally
            {
                lstACDobjects = null;
                lstDNobjects = null;
                lstVQobjects = null;
            }

            logger.Debug("StatisticsBase : SaveObjects Method : Exit");

            return appDetails;
        }

        /// <summary>
        /// Saves the stats.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <param name="appDetails">The app details.</param>
        /// <returns></returns>
        private KeyValueCollection SaveStats(Dictionary<string, List<string>> values, KeyValueCollection appDetails)
        {
            List<string> lstAgentobjects = new List<string>();
            List<string> lstAgentGroupobjects = new List<string>();
            List<string> lstACDobjects = new List<string>();
            List<string> lstDNobjects = new List<string>();
            List<string> lstVQobjects = new List<string>();

            try
            {
                logger.Debug("StatisticsBase : SaveStats Method : Entry");

                foreach (string objectType in values.Keys)
                {
                    if (string.Compare(objectType, StatisticsEnum.ObjectType.Agent.ToString(), true) == 0)
                        lstAgentobjects = values[objectType];
                    else if (string.Compare(objectType, StatisticsEnum.ObjectType.AgentGroup.ToString(), true) == 0)
                        lstAgentGroupobjects = values[objectType];
                    else if (string.Compare(objectType, StatisticsEnum.ObjectType.ACDQueue.ToString(), true) == 0)
                        lstACDobjects = values[objectType];
                    else if (string.Compare(objectType, StatisticsEnum.ObjectType.DNGroup.ToString(), true) == 0)
                        lstDNobjects = values[objectType];
                    else if (string.Compare(objectType, StatisticsEnum.ObjectType.VirtualQueue.ToString(), true) == 0)
                        lstVQobjects = values[objectType];

                }

                string AgentTempValue = string.Empty;
                string AgentGroupTempValue = string.Empty;
                string ACDTempValue = string.Empty;
                string DNTempValue = string.Empty;
                string VQTempValue = string.Empty;

                if (appDetails == null)
                {
                    KeyValueCollection kvps = new KeyValueCollection();

                    kvps.Add("agent-statistics", AgentTempValue);
                    kvps.Add("agent-group-statistics", AgentGroupTempValue);
                    kvps.Add("acd-queue-statistics", ACDTempValue);
                    kvps.Add("dn-group-statistics", DNTempValue);
                    kvps.Add("virtual-queue-statistics", VQTempValue);

                    appDetails = kvps;
                }

                #region Agent Statistics

                logger.Info("StatisticsBase : SaveStats Method : Saving Agent Statistics");

                foreach (string stat in lstAgentobjects)
                {
                    if (string.IsNullOrEmpty(AgentTempValue))
                        AgentTempValue = stat;
                    else
                        AgentTempValue = AgentTempValue + "," + stat;
                }

                string[] AStatistics = AgentTempValue.Split(',');

                #region Overwrite Agent Statistics

                string tempStat = string.Empty;
                string kvpname = "agent-statistics";
                bool issaved = false;
                int kvpno = 0;

                List<string> kvpremoved = new List<string>();
                foreach (string keys in appDetails.Keys)
                {
                    if (keys.Contains(kvpname))
                    {
                        kvpremoved.Add(keys);
                    }
                }

                foreach (string removedkvp in kvpremoved)
                {
                    appDetails.Remove(removedkvp);
                }

                if (AStatistics.Length > 10)
                {
                    int remainstats = 10;

                    for (int i = 10; i <= AgentTempValue.Length; )
                    {
                        tempStat = string.Empty;
                        for (int j = i - remainstats; j < i; j++)
                        {
                            if (tempStat == string.Empty)
                                tempStat = AStatistics[j];
                            else
                                tempStat = tempStat + "," + AStatistics[j];
                        }

                        kvpno++;
                        string tempKVPname = kvpname + "_" + kvpno.ToString();
                        if (appDetails.ContainsKey(tempKVPname))
                        {
                            appDetails.Remove(tempKVPname);
                            appDetails[tempKVPname] = tempStat;
                        }
                        else
                        {
                            if (tempStat != "" && tempStat != null && tempStat != string.Empty)
                                appDetails.Add(tempKVPname, tempStat);
                        }
                        if (issaved)
                        {
                            i = i + 10;
                            break;
                        }

                        if (i + 10 > AStatistics.Length)
                        {
                            remainstats = AStatistics.Length - i;
                            i = i + (AStatistics.Length - i);
                            issaved = true;
                        }
                        else
                            i = i + 10;
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(AStatistics[0].ToString()))
                    {
                        if (appDetails.ContainsKey("agent-statistics"))
                        {
                            appDetails["agent-statistics"] = AgentTempValue;
                        }
                        else
                        {
                            appDetails.Add("agent-statistics", AgentTempValue);
                        }
                    }
                }

                #endregion

                #endregion

                #region AgentGroup Statistics

                logger.Info("StatisticsBase : SaveStats Method : Saving AgentGroup Statistics");

                foreach (string stat in lstAgentGroupobjects)
                {
                    if (string.IsNullOrEmpty(AgentGroupTempValue))
                        AgentGroupTempValue = stat;
                    else
                        AgentGroupTempValue = AgentGroupTempValue + "," + stat;
                }

                string[] AGStatistics = AgentGroupTempValue.Split(',');

                #region Overwrite Agentgroup Statistics

                tempStat = string.Empty;
                kvpname = "agent-group-statistics";
                issaved = false;
                kvpno = 0;

                kvpremoved = new List<string>();
                foreach (string keys in appDetails.Keys)
                {
                    if (keys.Contains(kvpname))
                    {
                        kvpremoved.Add(keys);
                    }
                }

                foreach (string removedkvp in kvpremoved)
                {
                    appDetails.Remove(removedkvp);
                }

                if (AGStatistics.Length > 10)
                {
                    int remainstats = 10;

                    for (int i = 10; i <= AGStatistics.Length; )
                    {
                        tempStat = string.Empty;
                        for (int j = i - remainstats; j < i; j++)
                        {
                            if (tempStat == string.Empty)
                                tempStat = AGStatistics[j];
                            else
                                tempStat = tempStat + "," + AGStatistics[j];
                        }

                        kvpno++;
                        string tempKVPname = kvpname + "_" + kvpno.ToString();
                        if (appDetails.ContainsKey(tempKVPname))
                        {
                            appDetails.Remove(tempKVPname);
                            appDetails[tempKVPname] = tempStat;
                        }
                        else
                        {
                            if (tempStat != "" && tempStat != null && tempStat != string.Empty)
                                appDetails.Add(tempKVPname, tempStat);
                        }
                        if (issaved)
                        {
                            i = i + 10;
                            break;
                        }

                        if (i + 10 > AGStatistics.Length)
                        {
                            remainstats = AGStatistics.Length - i;
                            i = i + (AGStatistics.Length - i);
                            issaved = true;
                        }
                        else
                            i = i + 10;
                    }

                }
                else
                {
                    if (!string.IsNullOrEmpty(AGStatistics[0].ToString()))
                    {
                        if (appDetails.ContainsKey("agent-group-statistics"))
                        {
                            appDetails["agent-group-statistics"] = AgentGroupTempValue;
                        }
                        else
                        {
                            appDetails.Add("agent-group-statistics", AgentGroupTempValue);
                        }
                    }
                }

                #endregion

                #endregion

                #region ACD Statistics

                logger.Info("StatisticsBase : SaveStats Method : Saving ACD Statistics");

                foreach (string stat in lstACDobjects)
                {
                    if (string.IsNullOrEmpty(ACDTempValue))
                        ACDTempValue = stat;
                    else
                        ACDTempValue = ACDTempValue + "," + stat;
                }

                string[] ACDStatistics = ACDTempValue.Split(',');

                #region Overwrite ACD Statistics

                tempStat = string.Empty;
                kvpname = "acd-queue-statistics";
                issaved = false;
                kvpno = 0;

                kvpremoved = new List<string>();
                foreach (string keys in appDetails.Keys)
                {
                    if (keys.Contains(kvpname))
                    {
                        kvpremoved.Add(keys);
                    }
                }

                foreach (string removedkvp in kvpremoved)
                {
                    appDetails.Remove(removedkvp);
                }

                if (ACDStatistics.Length > 10)
                {
                    int remainstats = 10;

                    for (int i = 10; i <= ACDStatistics.Length; )
                    {
                        tempStat = string.Empty;
                        for (int j = i - remainstats; j < i; j++)
                        {
                            if (tempStat == string.Empty)
                                tempStat = ACDStatistics[j];
                            else
                                tempStat = tempStat + "," + ACDStatistics[j];
                        }

                        kvpno++;
                        string tempKVPname = kvpname + "_" + kvpno.ToString();
                        if (appDetails.ContainsKey(tempKVPname))
                        {
                            appDetails.Remove(tempKVPname);
                            appDetails[tempKVPname] = tempStat;
                        }
                        else
                        {
                            if (tempStat != "" && tempStat != null && tempStat != string.Empty)
                                appDetails.Add(tempKVPname, tempStat);
                        }
                        if (issaved)
                        {
                            i = i + 10;
                            break;
                        }

                        if (i + 10 > ACDStatistics.Length)
                        {
                            remainstats = ACDStatistics.Length - i;
                            i = i + (ACDStatistics.Length - i);
                            issaved = true;
                        }
                        else
                            i = i + 10;
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(ACDStatistics[0].ToString()))
                    {
                        if (appDetails.ContainsKey("acd-queue-statistics"))
                        {
                            appDetails["acd-queue-statistics"] = ACDTempValue;
                        }
                        else
                        {
                            appDetails.Add("acd-queue-statistics", ACDTempValue);
                        }
                    }
                }

                #endregion

                #endregion

                #region DNGroup Statistics

                logger.Info("StatisticsBase : SaveStats Method : Saving DNGroup Statistics");

                foreach (string stat in lstDNobjects)
                {
                    if (string.IsNullOrEmpty(DNTempValue))
                        DNTempValue = stat;
                    else
                        DNTempValue = DNTempValue + "," + stat;
                }
                string[] DNStatistics = DNTempValue.Split(',');

                #region Overwrite DNGroup Statistics

                tempStat = string.Empty;
                kvpname = "dn-group-statistics";
                issaved = false;
                kvpno = 0;

                kvpremoved = new List<string>();
                foreach (string keys in appDetails.Keys)
                {
                    if (keys.Contains(kvpname))
                    {
                        kvpremoved.Add(keys);
                    }
                }

                foreach (string removedkvp in kvpremoved)
                {
                    appDetails.Remove(removedkvp);
                }

                if (DNStatistics.Length > 10)
                {
                    int remainstats = 10;

                    for (int i = 10; i <= DNStatistics.Length; )
                    {
                        tempStat = string.Empty;
                        for (int j = i - remainstats; j < i; j++)
                        {
                            if (tempStat == string.Empty)
                                tempStat = DNStatistics[j];
                            else
                                tempStat = tempStat + "," + DNStatistics[j];
                        }

                        kvpno++;
                        string tempKVPname = kvpname + "_" + kvpno.ToString();
                        if (appDetails.ContainsKey(tempKVPname))
                        {
                            appDetails.Remove(tempKVPname);
                            appDetails[tempKVPname] = tempStat;
                        }
                        else
                        {
                            if (tempStat != "" && tempStat != null && tempStat != string.Empty)
                                appDetails.Add(tempKVPname, tempStat);
                        }
                        if (issaved)
                        {
                            i = i + 10;
                            break;
                        }

                        if (i + 10 > DNStatistics.Length)
                        {
                            remainstats = DNStatistics.Length - i;
                            i = i + (DNStatistics.Length - i);
                            issaved = true;
                        }
                        else
                            i = i + 10;
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(DNStatistics[0].ToString()))
                    {
                        if (appDetails.ContainsKey("dn-group-statistics"))
                        {
                            appDetails["dn-group-statistics"] = DNTempValue;
                        }
                        else
                        {
                            appDetails.Add("dn-group-statistics", DNTempValue);
                        }
                    }
                }

                #endregion

                #endregion

                #region VQ Statistics

                logger.Info("StatisticsBase : SaveStats Method : Saving VQ Statistics");

                foreach (string stat in lstVQobjects)
                {
                    if (string.IsNullOrEmpty(VQTempValue))
                        VQTempValue = stat;
                    else
                        VQTempValue = VQTempValue + "," + stat;
                }
                string[] VQStatistics = VQTempValue.Split(',');

                #region Overwrite VQ Statistics

                tempStat = string.Empty;
                kvpname = "virtual-queue-statistics";
                issaved = false;
                kvpno = 0;

                kvpremoved = new List<string>();
                foreach (string keys in appDetails.Keys)
                {
                    if (keys.Contains(kvpname))
                    {
                        kvpremoved.Add(keys);
                    }
                }

                foreach (string removedkvp in kvpremoved)
                {
                    appDetails.Remove(removedkvp);
                }

                if (VQStatistics.Length > 10)
                {
                    int remainstats = 10;

                    for (int i = 10; i <= VQStatistics.Length; )
                    {
                        tempStat = string.Empty;
                        for (int j = i - remainstats; j < i; j++)
                        {
                            if (tempStat == string.Empty)
                                tempStat = VQStatistics[j];
                            else
                                tempStat = tempStat + "," + VQStatistics[j];
                        }

                        kvpno++;
                        string tempKVPname = kvpname + "_" + kvpno.ToString();
                        if (appDetails.ContainsKey(tempKVPname))
                        {
                            appDetails.Remove(tempKVPname);
                            appDetails[tempKVPname] = tempStat;
                        }
                        else
                        {
                            if (tempStat != "" && tempStat != null && tempStat != string.Empty)
                                appDetails.Add(tempKVPname, tempStat);
                        }
                        if (issaved)
                        {
                            i = i + 10;
                            break;
                        }

                        if (i + 10 > VQStatistics.Length)
                        {
                            remainstats = VQStatistics.Length - i;
                            i = i + (VQStatistics.Length - i);
                            issaved = true;
                        }
                        else
                            i = i + 10;
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(VQStatistics[0].ToString()))
                    {
                        if (appDetails.ContainsKey("virtual-queue-statistics"))
                        {
                            appDetails["virtual-queue-statistics"] = VQTempValue;
                        }
                        else
                        {
                            appDetails.Add("virtual-queue-statistics", VQTempValue);
                        }
                    }
                }

                #endregion

                #endregion
            }
            catch (Exception GeneralException)
            {
                logger.Error("StatisticsBase : SaveStats Method : " + GeneralException.Message);
            }
            finally
            {
                lstAgentobjects = null;
                lstAgentGroupobjects = null;
                lstACDobjects = null;
                lstDNobjects = null;
                lstVQobjects = null;
            }

            logger.Debug("StatisticsBase : SaveStats Method : Exit");

            return appDetails;
        }

        /// <summary>
        /// Handles the Closed event of the statServerProtocol control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        void statServerProtocol_Closed(object sender, EventArgs e)
        {
            try
            {
                if (sender is StatServerProtocol)
                {
                    var sProtocol = sender as StatServerProtocol;

                    logger.Debug("StatisticsBase : StatServerProtocol_Closed : Entry");
                    if (sProtocol.State == ChannelState.Closed || sProtocol.State == ChannelState.Opening)
                    {
                        logger.Trace("ReportingServer : ReportingServer_Closed: Connection to StatServer is Lost");
                        if ((StatisticsSetting.GetInstance().serverNames != null) && (StatisticsSetting.GetInstance().serverNames.Count > 0))
                        {

                            if (StatisticsSetting.GetInstance().serverNames.ContainsKey(sProtocol.Endpoint.Name))
                                StatisticsSetting.GetInstance().serverNames[sProtocol.Endpoint.Name] = false;
                        }
                        foreach (KeyValuePair<string, IStatTicker> messageToClient in ToClient)
                        {
                            if (messageToClient.Key == "StatTickerFive" || messageToClient.Key == "AID")
                            {
                                messageToClient.Value.NotifyStatServerStatustoTC(false, sProtocol.Endpoint.Name);

                                OutputValues serverError = new OutputValues();

                                serverError.Message = StatisticsSetting.GetInstance().statisticsCollection.ApplicationContainer.ServerDown.ToString();

                                serverError.MessageCode = "2001";
                                messageToClient.Value.NotifyStatErrorMessage(serverError);
                            }
                        }
                    }
                    IsSubscribed = false;
                }

            }
            catch (Exception GeneralException)
            {
                logger.Error("StatisticsBase : statServerProtocol_Closed : " + GeneralException.Message);
            }
            finally
            {

                logger.Debug("StatisticsBase : StatServerProtocol_Closed : Exit");
            }
        }

        /// <summary>
        /// Handles the Opened event of the statServerProtocol control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        void statServerProtocol_Opened(object sender, EventArgs e)
        {
            try
            {
                logger.Debug("StatisticsBase : StatServerProtocol_Opened : Entry");

                logger.Trace("ReportingServer : Connection to StatServer is Opened");
                if (sender is StatServerProtocol)
                {
                    var sProtocol = sender as StatServerProtocol;
                    StatisticsSetting.GetInstance().rptProtocolManager[sProtocol.Endpoint.Name].Received -= ReportingSuccessMessage;
                    StatisticsSetting.GetInstance().rptProtocolManager[sProtocol.Endpoint.Name].Received += ReportingSuccessMessage;

                    if ((StatisticsSetting.GetInstance().serverNames != null) && (StatisticsSetting.GetInstance().serverNames.Count > 0))
                    {

                        if (StatisticsSetting.GetInstance().serverNames.ContainsKey(sProtocol.Endpoint.Name))
                            StatisticsSetting.GetInstance().serverNames[sProtocol.Endpoint.Name] = true;
                    }

                    if (!IsSubscribed)
                    {
                        if (!StatisticsSetting.GetInstance().statisticsCollection.ApplicationContainer.EnableMyQueueConfig)
                        {
                            newStatisticsDataProvider = new StatisticsDataProvider();
                            agentSubscriber = new AgentStatisticsSubscriber();
                            agentGroupSubscriber = new AgentGroupStatisticsSubscriber();
                            acdSubscriber = new ACDStatisticsSubscriber();
                            dngroupSubscriber = new DNGroupStatisticsSubscriber();
                            vqSubscriber = new VQStatisticsSubscriber();

                            agentSubscriber.Subscribe(newStatisticsDataProvider);
                            agentGroupSubscriber.Subscribe(newStatisticsDataProvider);
                            acdSubscriber.Subscribe(newStatisticsDataProvider);
                            dngroupSubscriber.Subscribe(newStatisticsDataProvider);
                            vqSubscriber.Subscribe(newStatisticsDataProvider);

                            newStatisticsDataProvider.NewStatisticsData(StatisticsSetting.GetInstance().statisticsCollection);
                            IsSubscribed = true;

                        }
                        else
                        {
                            newStatisticsDataProvider = new StatisticsDataProvider();
                            agentSubscriber = new AgentStatisticsSubscriber();
                            agentGroupSubscriber = new AgentGroupStatisticsSubscriber();
                            commonSubscriber = new CommonStatisticsSubscriber();

                            agentSubscriber.Subscribe(newStatisticsDataProvider);
                            agentGroupSubscriber.Subscribe(newStatisticsDataProvider);
                            commonSubscriber.Subscribe(newStatisticsDataProvider);

                            newStatisticsDataProvider.NewStatisticsData(StatisticsSetting.GetInstance().statisticsCollection);
                            IsSubscribed = true;

                        }
                        //newStatisticsDataProvider = new StatisticsDataProvider();
                        //agentSubscriber = new AgentStatisticsSubscriber();
                        //agentGroupSubscriber = new AgentGroupStatisticsSubscriber();
                        //commonSubscriber = new CommonStatisticsSubscriber();

                        //agentSubscriber.Subscribe(newStatisticsDataProvider);
                        //agentGroupSubscriber.Subscribe(newStatisticsDataProvider);
                        //commonSubscriber.Subscribe(newStatisticsDataProvider);

                        //newStatisticsDataProvider.NewStatisticsData(StatisticsSetting.GetInstance().statisticsCollection);
                        //IsSubscribed = true;

                        foreach (KeyValuePair<string, IStatTicker> messageToClient in ToClient)
                        {
                            if (messageToClient.Key == "StatTickerFive" || messageToClient.Key == "AID")
                            {
                                messageToClient.Value.NotifyStatServerStatustoTC(true, sProtocol.Endpoint.Name);
                                //messageToClient.Value.NotifyStatServerStatustoTC(true);
                                OutputValues serverError = new OutputValues();

                                serverError.Message = "Server Started";

                                serverError.MessageCode = "2000";
                                messageToClient.Value.NotifyStatErrorMessage(serverError);
                            }
                        }
                    }
                }

            }
            catch (Exception GeneralException)
            {
                logger.Error("StatisticsBase : statServerProtocol_Opened : " + GeneralException.Message);
            }

            finally
            {
                agentSubscriber = null;
                agentGroupSubscriber = null;
                acdSubscriber = null;
                dngroupSubscriber = null;
                vqSubscriber = null;
                logger.Debug("StatisticsBase : StatServerProtocol_Opened : Exit");
            }
        }

        #endregion Methods
    }
}