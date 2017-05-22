namespace Pointel.Statistics.Core.ConnectionManager
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows.Forms;

    using Genesyslab.Platform.ApplicationBlocks.Commons.Broker;
    using Genesyslab.Platform.ApplicationBlocks.Commons.Protocols;
    using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel;
    using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.CfgObjects;
    using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.Queries;
    using Genesyslab.Platform.Commons.Collections;
    using Genesyslab.Platform.Commons.Protocols;
    using Genesyslab.Platform.Configuration.Protocols;
    using Genesyslab.Platform.Configuration.Protocols.ConfServer.Events;
    using Genesyslab.Platform.Configuration.Protocols.ConfServer.Requests.Objects;
    using Genesyslab.Platform.Configuration.Protocols.Types;

    using Pointel.Logger.Core;
    using Pointel.Statistics.Core.Application;
    using Pointel.Statistics.Core.General;
    using Pointel.Statistics.Core.StatisticsProvider;
    using Pointel.Statistics.Core.Utility;

    /// <summary>
    /// This class contains to connection establishment and check user priviliges etc.
    /// </summary>
    internal class ConfigConnectionManager
    {
        #region Fields

        private static ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().
        DeclaringType, "STF");

        #endregion Fields

        #region Methods

        /// <summary>
        /// Checks the user privilege.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <returns></returns>
        public string CheckUserPrivilege(string username, IStatisticsCollection statCollection)
        {
            bool isAuthenticated = false;
            bool isAdmin = false;
            string AdminUsers = string.Empty;
            try
            {
                logger.Debug("ConfigConnectionManager : CheckUserPrivilege Method: Entry");

                CfgAccessGroupQuery queryAccessGroup = null;
                CfgAccessGroup accessGroup = null;
                CfgPersonQuery queryPerson = null;
                CfgPerson Person = null;

                if (statCollection.ApplicationContainer.UserGroupName != null && !isAdmin)
                {
                    queryAccessGroup = new CfgAccessGroupQuery();
                    queryAccessGroup.Name = statCollection.ApplicationContainer.UserGroupName;
                    accessGroup = StatisticsSetting.GetInstance().confObject.RetrieveObject<CfgAccessGroup>(queryAccessGroup);

                    queryPerson = new CfgPersonQuery();
                    queryPerson.UserName = username;
                    Person = StatisticsSetting.GetInstance().confObject.RetrieveObject<CfgPerson>(queryPerson);

                    if (accessGroup != null)
                    {
                        foreach (CfgID memberID in accessGroup.MemberIDs)
                        {
                            int id = memberID.DBID;
                            if (memberID.Type == CfgObjectType.CFGPerson)
                            {
                                if (id == Person.DBID)
                                {
                                    isAuthenticated = true;
                                    StatisticsSetting.GetInstance().isAdmin = false;
                                }
                            }
                        }
                    }

                    queryPerson = new CfgPersonQuery();
                    ICollection<CfgPerson> Persons = StatisticsSetting.GetInstance().confObject.RetrieveMultipleObjects<CfgPerson>(queryPerson);

                    if (Persons != null)
                    {
                        foreach (CfgPerson agent in Persons)
                        {
                            if (accessGroup != null)
                            {
                                foreach (CfgID memberID in accessGroup.MemberIDs)
                                {
                                    int id = memberID.DBID;
                                    if (memberID.Type == CfgObjectType.CFGPerson)
                                    {
                                        if (id == agent.DBID)
                                        {
                                            StatisticsSetting.GetInstance().LstAgentUNames.Add(agent.UserName);
                                            StatisticsSetting.GetInstance().LstPersons.Add(agent);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                if (statCollection.AdminValues.AdminUsers.Count != 0)
                {
                    if (statCollection.AdminValues.AdminUsers.Contains(username))
                    {
                        isAdmin = true;
                        StatisticsSetting.GetInstance().isAdmin = true;
                    }
                }
            }
            catch (Exception generalException)
            {
                logger.Error("ConfigConnectionManager : CheckUserPrivilege Method: " + generalException.Message.ToString());
            }
            finally
            {
                logger.Debug("ConfigConnectionManager : CheckUserPrevilege Method: Exit");
                GC.Collect();
            }
            return isAuthenticated.ToString() + "," + isAdmin.ToString();
        }

        public void CloseConfigServer()
        {
            try
            {

                if (StatisticsSetting.GetInstance().protocolManager != null && StatisticsSetting.GetInstance().protocolManager[StatisticsSetting.ConfServer].State == ChannelState.Opened)
                {
                    logger.Info("ConfigConnectionManager : Config server protocol state : " + StatisticsSetting.GetInstance().protocolManager[StatisticsSetting.ConfServer].State.ToString());
                    StatisticsSetting.GetInstance().protocolManager[StatisticsSetting.ConfServer].Close();
                    logger.Info("ConfigConnectionManager : Config server protocol has been closed.");
                }
            }

            catch (Exception ex)
            {
                logger.Error("Error occurred while closing the " + "Config server protocol : " + ex.Message);
            }
        }

        /// <summary>
        /// Connects the config server.
        /// </summary>
        /// <param name="ConfigServerHost">The config server host.</param>
        /// <param name="ConfigServerPort">The config server port.</param>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        public OutputValues ConnectConfigServer(string ConfigServerHost, string ConfigServerPort,
            string userName, string password, string clientName, string logUserName)
        {
            EventBrokerService comEventBrokerService;
            OutputValues output = OutputValues.GetInstance();
            ReadApplication read = new ReadApplication();
            try
            {
                StatisticsSetting.GetInstance().logUserName = logUserName;
                logger.Debug("ConfigConnectionManager : ConnectConfigServer Method: Entry");
                if (StatisticsSetting.GetInstance().protocolManager == null)
                {
                    StatisticsSetting.GetInstance().configurationProperties = new ConfServerConfiguration("config");
                    StatisticsSetting.GetInstance().protocolManager = new ProtocolManagementService();

                    //Primary Server settings
                    StatisticsSetting.GetInstance().configurationProperties.Uri = new Uri("tcp://" + ConfigServerHost + ":"
                                                                             + ConfigServerPort);
                    StatisticsSetting.GetInstance().configurationProperties.ClientApplicationType = CfgAppType.CFGAgentDesktop;
                    StatisticsSetting.GetInstance().configurationProperties.ClientName = clientName;
                    StatisticsSetting.GetInstance().configurationProperties.UserName = userName;
                    StatisticsSetting.GetInstance().configurationProperties.UserPassword = password;

                    //Set ADDP
                    StatisticsSetting.GetInstance().configurationProperties.UseAddp = true;
                    StatisticsSetting.GetInstance().configurationProperties.AddpServerTimeout = 30;
                    StatisticsSetting.GetInstance().configurationProperties.AddpClientTimeout = 60;

                    //Open the connection
                    try
                    {
                        StatisticsSetting.GetInstance().protocolManager.Register(StatisticsSetting.GetInstance().configurationProperties);
                    }
                    catch (Exception generalException)
                    {
                        output.MessageCode = "2001";
                        output.Message = (generalException.InnerException == null ? generalException.Message : generalException.InnerException.Message);
                        logger.Error("ConfigConnectionManager : ConnectConfigServer Method: " + generalException.Message.ToString());
                        return output;
                    }

                    StatisticsSetting.GetInstance().protocolManager[StatisticsSetting.ConfServer].Open();

                    comEventBrokerService = new EventBrokerService(StatisticsSetting.GetInstance().protocolManager.Receiver);
                    comEventBrokerService.Activate();

                    //comEventBrokerService.Register(OnConfEventError);
                    //comEventBrokerService.Register(OnConfEventObjectsRead);
                    //comEventBrokerService.Register(OnConfEventObjectsSent);

                    //KeyValueCollection filterKey = new KeyValueCollection();
                    //filterKey.Add("switch_dbid", 102);
                    //filterKey.Add("dn_type", (int)CfgDNType.CFGExtension);
                    //RequestReadObjects requestReadObjects = RequestReadObjects.Create((int)CfgObjectType.CFGDN, filterKey);
                    //StatisticsSetting.GetInstance().protocolManager[StatisticsSetting.ConfServer].Send(requestReadObjects);

                    if (StatisticsSetting.GetInstance().confObject == null)
                        StatisticsSetting.GetInstance().confObject = (ConfService)ConfServiceFactory.CreateConfService(
                            StatisticsSetting.GetInstance().protocolManager[StatisticsSetting.ConfServer] as ConfServerProtocol,
                            comEventBrokerService, true);

                    //NotificationQuery NQuery=new NotificationQuery();
                    //NQuery.ObjectType=CfgObjectType.CFGPerson;

                    //StatisticsSetting.GetInstance().confObject.Subscribe(NQuery);

                    if (StatisticsSetting.GetInstance().protocolManager[StatisticsSetting.ConfServer].State == ChannelState.Opened)
                    {
                        StatisticsSetting.GetInstance().confProtocol = (ConfServerProtocol)StatisticsSetting.GetInstance().protocolManager[StatisticsSetting.ConfServer];

                        read.ReadLoggerDetails(StatisticsSetting.GetInstance().AppName);
                        output.MessageCode = "2000";
                        output.Message = "Config Server Protocol Opened";
                        logger.Trace("ConfigConnectionManager : ConnectConfigServer Method: Config Server Protocol Opened");

                    }
                    else
                    {
                        output.MessageCode = "2001";
                        output.Message = "Config Server Protocol Closed";
                        logger.Warn("ConfigConnectionManager : ConnectConfigServer Method: Config Server Protocol Closed");
                    }
                }
            }
            catch (Exception connectionException)
            {
                logger.Error("ConfigConnectionManager : ConnectConfigServer Method: " + connectionException.Message.ToString());
                output.MessageCode = "2001";
                output.Message = (connectionException.InnerException == null ? connectionException.Message : connectionException.InnerException.Message);
            }
            finally
            {
                logger.Debug("ConfigConnectionManager : ConnectConfigServer Method: Exit");
                //StatisticsSetting.GetInstance().protocolManager = null;
                GC.Collect();
            }
            return output;
        }

        /// <summary>
        /// Gets the plugin values.
        /// </summary>
        /// <param name="AppName">Name of the app.</param>
        /// <param name="ConfObject">The conf object.</param>
        /// <returns></returns>
        public OutputValues GetPluginValues(string AppName, ConfService ConfObject)
        {
            ReadApplication read = new ReadApplication();
            OutputValues output = OutputValues.GetInstance();
            try
            {
                logger.Debug("ConfigConnectionManager : GetPluginValues Method: Entry");
                StatisticsSetting.GetInstance().confObject = ConfObject;

                read.ReadLoggerDetails(AppName);
                output.MessageCode = "200";
                output.Message = "Config Server Protocol Opened";
            }
            catch (Exception connectionException)
            {
                logger.Error("ConfigConnectionManager :GetPluginValues Method: " + connectionException.Message.ToString());
            }
            finally
            {
                logger.Debug("ConfigConnectionManager : GetPluginValues Method: Exit");
                GC.Collect();
            }

            return output;
        }

        private void ObjectCreated(IMessage message)
        {
            try
            {

            }
            catch (Exception GeneralException)
            {

            }
        }

        [MessageRangeFilter(new int[] { EventError.MessageId },
           ProtocolName = "ConfServer", SdkName = "Configuration")]
        private void OnConfEventError(IMessage theMessage)
        {
            EventError eventError = theMessage as EventError;
        }

        [MessageRangeFilter(new int[] { EventObjectsRead.MessageId },
           ProtocolName = "ConfServer", SdkName = "Configuration")]
        private void OnConfEventObjectsRead(IMessage theMessage)
        {
            EventObjectsRead objectsRead = theMessage as EventObjectsRead;
            if (objectsRead != null)
            {
                int count;
                count = objectsRead.ObjectCount;
            }
        }

        [MessageIdFilter(EventObjectsSent.MessageId, ProtocolName = "ConfServer", SdkName = "Configuration")]
        private void OnConfEventObjectsSent(IMessage theMessage)
        {
            EventObjectsSent objectsSent = theMessage as EventObjectsSent;
            if (objectsSent == null)
            {
            }
        }

        #endregion Methods
    }
}