using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;
using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel;
using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.CfgObjects;
using Genesyslab.Platform.Commons.Protocols;
using Pointel.Interactions.IPlugins;
using Pointel.Interactions.TeamCommunicator.AppReader;
using Pointel.Interactions.TeamCommunicator.Helpers;
using Pointel.Interactions.TeamCommunicator.Settings;
using System.Windows;
using Genesyslab.Platform.Reporting.Protocols.StatServer;
using Genesyslab.Platform.Reporting.Protocols.StatServer.Events;
using Pointel.Interactions.TeamCommunicator.ConnectionManager;
using Pointel.Configuration.Manager;
using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.Queries;
using Pointel.Interactions.TeamCommunicator.Request;
using System.Collections.ObjectModel;
using System.Windows.Data;
using System.Threading;

namespace Pointel.Interactions.TeamCommunicator.InteractionListener
{
    public class Listener : ITeamCommunicatorPlugin
    {
        #region Private read only members
        private Pointel.Logger.Core.ILog _logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
                    "AID");
        #endregion

        #region Delegate declaration
        public delegate void StatusUpdation();
        #endregion

        #region Event Declaration
        public static event StatusUpdation _statusUpdation;
        #endregion

        #region Variable Declarations
        BitmapImage statusImageSource = new BitmapImage();
        BitmapImage callImageSource = new BitmapImage();
        string callToolTip = "";
        Datacontext _dataContext = Datacontext.GetInstance();
        #endregion

        #region ITeamCommunicatorPlugin Members

        /// <summary>
        /// Initializes the specified cme objects.
        /// </summary>
        /// <param name="CMEObjects">The cme objects.</param>
        protected void Initialize(Dictionary<string, object> CMEObjects)
        {
            try
            {
                //_logger.Info("Listener Class : Initialize() Entry.");
                if (CMEObjects != null)
                {
                    if (CMEObjects.ContainsKey("UserName"))
                    {
                        _dataContext.UserName = CMEObjects["UserName"].ToString();
                    }
                    if (CMEObjects.ContainsKey("teamcommunicator.filter-list"))
                    {
                        string list = Convert.ToString(CMEObjects["teamcommunicator.filter-list"]);
                        string[] tempList = list.Split(',');
                        _dataContext.FilterList.Clear();
                        foreach (string item in tempList)
                        {
                            Object obj = Enum.Parse(typeof(Datacontext.SelectorFilters), item);
                            if (obj != null && obj is Datacontext.SelectorFilters)
                                _dataContext.FilterList.Add((Datacontext.SelectorFilters)obj);
                        }
                        if (_dataContext.FilterList.Count == 0)
                        {
                            _dataContext.FilterList.Add(Datacontext.SelectorFilters.AllTypes);
                            _dataContext.FilterList.Add(Datacontext.SelectorFilters.Agent);
                            _dataContext.FilterList.Add(Datacontext.SelectorFilters.AgentGroup);
                            _dataContext.FilterList.Add(Datacontext.SelectorFilters.Skill);
                            //_dataContext.FilterList.Add(Datacontext.SelectorFilters.Queue);
                            _dataContext.FilterList.Add(Datacontext.SelectorFilters.InteractionQueue);
                            //_dataContext.FilterList.Add(Datacontext.SelectorFilters.Contact);
                        }
                    }

                    if (CMEObjects.ContainsKey("teamcommunicator.custom-favorite-list") &&
                        !string.IsNullOrEmpty(CMEObjects["teamcommunicator.custom-favorite-list"] as string))
                    {
                        string list = Convert.ToString(CMEObjects["teamcommunicator.custom-favorite-list"]);
                        string[] tempList = list.Split(',');
                        _dataContext.CustomFavoriteList.Clear();
                        foreach (string item in tempList)
                        {
                            switch (item.Trim())
                            {
                                case "Category":
                                case "DisplayName":
                                case "FirstName":
                                case "LastName":
                                case "EmailAddress":
                                case "PhoneNumber": _dataContext.CustomFavoriteList.Add(item.Trim()); break;
                                default: break;
                            }
                        }

                    }
                    if (_dataContext.CustomFavoriteList.Count == 0)
                    {
                        _dataContext.CustomFavoriteList.Clear();
                        _dataContext.CustomFavoriteList = new List<string>(new string[] { "Category", "DisplayName", "FirstName", "LastName", "EmailAddress", "PhoneNumber" });
                    }
                    _dataContext.CustomFavoriteList = _dataContext.CustomFavoriteList.Distinct().ToList();

                    if (CMEObjects.ContainsKey("teamcommunicator.internal-favorite-list") &&
                        !string.IsNullOrEmpty(CMEObjects["teamcommunicator.internal-favorite-list"] as string))
                    {
                        string list = Convert.ToString(CMEObjects["teamcommunicator.internal-favorite-list"]);
                        string[] tempList = list.Split(',');
                        _dataContext.InternalFavoriteList.Clear();
                        foreach (string item in tempList)
                        {
                            switch (item.Trim())
                            {
                                case "Category":
                                case "DisplayName":
                                case "FirstName":
                                case "LastName":
                                case "EmailAddress":
                                    _dataContext.InternalFavoriteList.Add(item.Trim()); break;
                                default: break;
                            }
                        }

                    }
                    if (_dataContext.InternalFavoriteList.Count == 0)
                    {
                        _dataContext.InternalFavoriteList.Clear();
                        _dataContext.InternalFavoriteList = new List<string>(new string[] { "Category", "DisplayName", "FirstName", "LastName", "EmailAddress"});
                    }
                    _dataContext.InternalFavoriteList = _dataContext.InternalFavoriteList.Distinct().ToList();

                   
                    int count = 0;
                    if (CMEObjects.ContainsKey("teamcommunicator.max-recent-records")
                        && int.TryParse(CMEObjects["teamcommunicator.max-recent-records"].ToString(), out count)
                        && count > 0)
                    {
                        _dataContext.RecentMaxRecords = count;
                        count = 0;
                    }
                    if (CMEObjects.ContainsKey("teamcommunicator.max-suggestion-size")
                        && int.TryParse(CMEObjects["teamcommunicator.max-suggestion-size"].ToString(), out count)
                        && count > 0)
                    {
                        _dataContext.MaxSuggestionSize = count;
                        count = 0;
                    }
                    if (CMEObjects.ContainsKey("teamcommunicator.max-favorites-size")
                        && int.TryParse(CMEObjects["teamcommunicator.max-favorites-size"].ToString(), out count)
                        && count > 0)
                    {
                        _dataContext.MaxFavouriteSize = count;
                        count = 0;
                    }
                    if (CMEObjects.ContainsKey("teamcommunicator.statistics-routing-points"))
                    {
                        _dataContext.RoutingAddress = (CMEObjects["teamcommunicator.statistics-routing-points"]).ToString();
                    }
                    if (_dataContext.dtRecentInteractions.Columns.Count < 1)
                    {
                        _dataContext.dtRecentInteractions.Columns.Add("Type", typeof(string));
                        _dataContext.dtRecentInteractions.Columns.Add("UniqueIdentity", typeof(string));
                        _dataContext.dtRecentInteractions.Columns.Add("DN", typeof(string));
                    }
                    if (_dataContext.dtFavorites.Columns.Count < 1)
                    {
                        _dataContext.dtFavorites.Columns.Add("Category", typeof(string));
                        _dataContext.dtFavorites.Columns.Add("DisplayName", typeof(string));
                        _dataContext.dtFavorites.Columns.Add("FirstName", typeof(string));
                        _dataContext.dtFavorites.Columns.Add("LastName", typeof(string));
                        _dataContext.dtFavorites.Columns.Add("PhoneNumber", typeof(string));
                        _dataContext.dtFavorites.Columns.Add("EmailAddress", typeof(string));
                        _dataContext.dtFavorites.Columns.Add("UniqueIdentity", typeof(string));
                        _dataContext.dtFavorites.Columns.Add("Type", typeof(string));
                    }

                    if (CMEObjects.ContainsKey("workbins"))
                    {
                        string workbins = CMEObjects["workbins"].ToString();
                        string[] workbinList = workbins.Split(',');
                        _dataContext.EmailWorkbins.Clear();
                        foreach (string value in workbinList)
                            if (!_dataContext.EmailWorkbins.Contains(value))
                                _dataContext.EmailWorkbins.Add(value);
                    }
                    if (CMEObjects.ContainsKey("teamworkbins"))
                    {
                        string workbins = CMEObjects["teamworkbins"].ToString();
                        string[] workbinList = workbins.Split(',');
                        _dataContext.TeamWorkbins.Clear();
                        foreach (string value in workbinList)
                            if (!_dataContext.TeamWorkbins.Contains(value))
                                _dataContext.TeamWorkbins.Add(value);
                    }
                    if (CMEObjects.ContainsKey("log.filter-level"))
                    {
                        _dataContext.LogFilterLevel = CMEObjects["log.filter-level"].ToString();
                        //_logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
                        //    _dataContext.LogFilterLevel);
                    }
                    if (CMEObjects.ContainsKey("teamcommunicator.corporate-favorites-file"))
                    {
                        _dataContext.CorporateFavoriteFile = Path.Combine(CMEObjects["teamcommunicator.corporate-favorites-file"].ToString());
                    }
                    if (CMEObjects.ContainsKey("TenantName"))
                    {
                        _dataContext.TenantName = CMEObjects["TenantDBID"].ToString();
                    }
                    if (CMEObjects.ContainsKey("AllContacts"))
                    {
                        _dataContext.ContactsIMessage = (IMessage)CMEObjects["AllContacts"];
                    }
                    if (CMEObjects.ContainsKey("myteamworkbin.supervisor.enable-move-workbin"))
                    {
                        _dataContext.IsSupervisorMoveWorkbinEnabled = Convert.ToBoolean(CMEObjects["myteamworkbin.supervisor.enable-move-workbin"].ToString());
                    }
                    if (CMEObjects.ContainsKey("myteamworkbin.supervisor.enable-move-interactionqueue"))
                    {
                        _dataContext.IsSupervisorMoveQueueEnabled = Convert.ToBoolean(CMEObjects["myteamworkbin.supervisor.enable-move-interactionqueue"].ToString());
                    }
                    if (CMEObjects.ContainsKey("AllPersons"))
                    {
                        Datacontext.GetInstance().AllPersons = (ICollection<CfgPerson>)CMEObjects["AllPersons"];
                    }

                    if (CMEObjects.ContainsKey("Initialize"))
                    {
                        XMLHandler _xmlHandler = new XMLHandler();
                        _xmlHandler.ReadFavoriteFields();
                        InitializeMenuItems();

                        if (ConfigContainer.Instance().AllKeys.Contains("teamcommunicator.list-chat-status-reachable"))
                        {
                            string[] reachableStates = ((string)ConfigContainer.Instance().GetValue("teamcommunicator.list-chat-status-reachable")).Split(',');
                            if (reachableStates.Length > 0)
                            {
                                foreach (string availableStates in reachableStates)
                                {
                                    Pointel.Interactions.IPlugins.AgentMediaStatus tempStatus = Pointel.Interactions.IPlugins.AgentMediaStatus.None;
                                    if (Enum.TryParse(availableStates.Trim(), true, out tempStatus))
                                    {
                                        if (tempStatus == AgentMediaStatus.All)
                                        {
                                            if (!Datacontext.GetInstance().AvailableChatStatus.Contains(AgentMediaStatus.Ready))
                                                Datacontext.GetInstance().AvailableChatStatus.Add(AgentMediaStatus.Ready);
                                            if (!Datacontext.GetInstance().AvailableChatStatus.Contains(AgentMediaStatus.NotReady))
                                                Datacontext.GetInstance().AvailableChatStatus.Add(AgentMediaStatus.NotReady);
                                            if (!Datacontext.GetInstance().AvailableChatStatus.Contains(AgentMediaStatus.Busy))
                                                Datacontext.GetInstance().AvailableChatStatus.Add(AgentMediaStatus.Busy);
                                            if (!Datacontext.GetInstance().AvailableChatStatus.Contains(AgentMediaStatus.LoggedOut))
                                                Datacontext.GetInstance().AvailableChatStatus.Add(AgentMediaStatus.LoggedOut);
                                            if (!Datacontext.GetInstance().AvailableChatStatus.Contains(AgentMediaStatus.LoggedIn))
                                                Datacontext.GetInstance().AvailableChatStatus.Add(AgentMediaStatus.LoggedIn);
                                            if (!Datacontext.GetInstance().AvailableChatStatus.Contains(AgentMediaStatus.ConditionallyReady))
                                                Datacontext.GetInstance().AvailableChatStatus.Add(AgentMediaStatus.ConditionallyReady);
                                            break;
                                        }
                                        else
                                        {
                                            if (!Datacontext.GetInstance().AvailableChatStatus.Contains(tempStatus))
                                                Datacontext.GetInstance().AvailableChatStatus.Add(tempStatus);
                                            if (tempStatus == AgentMediaStatus.NotReady)
                                                if (!Datacontext.GetInstance().AvailableChatStatus.Contains(AgentMediaStatus.Ready))
                                                    Datacontext.GetInstance().AvailableChatStatus.Add(AgentMediaStatus.Ready);
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (!Datacontext.GetInstance().AvailableChatStatus.Contains(AgentMediaStatus.Ready))
                                Datacontext.GetInstance().AvailableChatStatus.Add(AgentMediaStatus.Ready);
                            if (!Datacontext.GetInstance().AvailableChatStatus.Contains(AgentMediaStatus.NotReady))
                                Datacontext.GetInstance().AvailableChatStatus.Add(AgentMediaStatus.NotReady);
                            if (!Datacontext.GetInstance().AvailableChatStatus.Contains(AgentMediaStatus.Busy))
                                Datacontext.GetInstance().AvailableChatStatus.Add(AgentMediaStatus.Busy);
                            if (!Datacontext.GetInstance().AvailableChatStatus.Contains(AgentMediaStatus.LoggedOut))
                                Datacontext.GetInstance().AvailableChatStatus.Add(AgentMediaStatus.LoggedOut);
                            if (!Datacontext.GetInstance().AvailableChatStatus.Contains(AgentMediaStatus.LoggedIn))
                                Datacontext.GetInstance().AvailableChatStatus.Add(AgentMediaStatus.LoggedIn);
                            if (!Datacontext.GetInstance().AvailableChatStatus.Contains(AgentMediaStatus.ConditionallyReady))
                                Datacontext.GetInstance().AvailableChatStatus.Add(AgentMediaStatus.ConditionallyReady);
                        }

                        if (ConfigContainer.Instance().AllKeys.Contains("teamcommunicator.list-email-status-reachable"))
                        {
                            string[] reachableStates = ((string)ConfigContainer.Instance().GetValue("teamcommunicator.list-email-status-reachable")).Split(',');
                            if (reachableStates.Length > 0)
                            {
                                foreach (string availableStates in reachableStates)
                                {
                                    Pointel.Interactions.IPlugins.AgentMediaStatus tempStatus = Pointel.Interactions.IPlugins.AgentMediaStatus.None;
                                    if (Enum.TryParse(availableStates.Trim(), true, out tempStatus))
                                    {
                                        if (tempStatus == AgentMediaStatus.All)
                                        {
                                            if (!Datacontext.GetInstance().AvailableEmailStatus.Contains(AgentMediaStatus.Ready))
                                                Datacontext.GetInstance().AvailableEmailStatus.Add(AgentMediaStatus.Ready);
                                            if (!Datacontext.GetInstance().AvailableEmailStatus.Contains(AgentMediaStatus.NotReady))
                                                Datacontext.GetInstance().AvailableEmailStatus.Add(AgentMediaStatus.NotReady);
                                            if (!Datacontext.GetInstance().AvailableEmailStatus.Contains(AgentMediaStatus.Busy))
                                                Datacontext.GetInstance().AvailableEmailStatus.Add(AgentMediaStatus.Busy);
                                            if (!Datacontext.GetInstance().AvailableEmailStatus.Contains(AgentMediaStatus.LoggedOut))
                                                Datacontext.GetInstance().AvailableEmailStatus.Add(AgentMediaStatus.LoggedOut);
                                            if (!Datacontext.GetInstance().AvailableEmailStatus.Contains(AgentMediaStatus.LoggedIn))
                                                Datacontext.GetInstance().AvailableEmailStatus.Add(AgentMediaStatus.LoggedIn);
                                            if (!Datacontext.GetInstance().AvailableEmailStatus.Contains(AgentMediaStatus.ConditionallyReady))
                                                Datacontext.GetInstance().AvailableEmailStatus.Add(AgentMediaStatus.ConditionallyReady);
                                            break;
                                        }
                                        else
                                        {
                                            if (!Datacontext.GetInstance().AvailableEmailStatus.Contains(tempStatus))
                                                Datacontext.GetInstance().AvailableEmailStatus.Add(tempStatus);
                                            if (tempStatus == AgentMediaStatus.NotReady)
                                                if (!Datacontext.GetInstance().AvailableEmailStatus.Contains(AgentMediaStatus.Ready))
                                                    Datacontext.GetInstance().AvailableEmailStatus.Add(AgentMediaStatus.Ready);
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (!Datacontext.GetInstance().AvailableEmailStatus.Contains(AgentMediaStatus.Ready))
                                Datacontext.GetInstance().AvailableEmailStatus.Add(AgentMediaStatus.Ready);
                            if (!Datacontext.GetInstance().AvailableEmailStatus.Contains(AgentMediaStatus.NotReady))
                                Datacontext.GetInstance().AvailableEmailStatus.Add(AgentMediaStatus.NotReady);
                            if (!Datacontext.GetInstance().AvailableEmailStatus.Contains(AgentMediaStatus.Busy))
                                Datacontext.GetInstance().AvailableEmailStatus.Add(AgentMediaStatus.Busy);
                            if (!Datacontext.GetInstance().AvailableEmailStatus.Contains(AgentMediaStatus.LoggedOut))
                                Datacontext.GetInstance().AvailableEmailStatus.Add(AgentMediaStatus.LoggedOut);
                            if (!Datacontext.GetInstance().AvailableEmailStatus.Contains(AgentMediaStatus.LoggedIn))
                                Datacontext.GetInstance().AvailableEmailStatus.Add(AgentMediaStatus.LoggedIn);
                            if (!Datacontext.GetInstance().AvailableEmailStatus.Contains(AgentMediaStatus.ConditionallyReady))
                                Datacontext.GetInstance().AvailableEmailStatus.Add(AgentMediaStatus.ConditionallyReady);
                        }

                        ConfigParser parser = new ConfigParser();
                        parser.LoadInitialData();

                        //commented by rajkumar
                        //if (CMEObjects.ContainsKey("OpenServer") && CMEObjects["OpenServer"].ToString().ToLower().Equals("true"))
                        //{
                        _dataContext.IsOpenStatServer = true;
                        ConnectStatServer();
                        //}
                    }
                }

                CMEObjects = null;
            }
            catch (Exception ex)
            {
                _logger.Error("Listener Class : Initialize() : " + ex.Message.ToString());
            }
            _logger.Info("Listener Class : Initialize() Exit.");
        }

        public void ConnectStatServer()
        {
            try
            {
                StatisticsConnectionManager _statisticsConnectionManager = new StatisticsConnectionManager();
                string primaryHost = string.Empty;
                string primaryPort = string.Empty;
                string secondaryHost = string.Empty;
                string secondaryPort = string.Empty;
                int addpServerTimeOut = 30;
                int addClientTimeOut = 60;
                int warmStandbyTimeOut = 10;
                short warmStandbyAttempts = 5;

                if (ConfigContainer.Instance().AllKeys.Contains("teamcommunicator.statistics-host"))//primary Host -----------------------------------------------------------
                    primaryHost = (string)ConfigContainer.Instance().GetValue("teamcommunicator.statistics-host");
                if (ConfigContainer.Instance().AllKeys.Contains("teamcommunicator.statistics-port"))//primary port -----------------------------------------------------------
                    primaryPort = (string)ConfigContainer.Instance().GetValue("teamcommunicator.statistics-port");
                //if (ConfigContainer.Instance().AllKeys.Contains(""))//secondary Host -----------------------------------------------------------
                //    secondaryHost = (string)ConfigContainer.Instance().GetValue("");
                //if (ConfigContainer.Instance().AllKeys.Contains(""))//secondary port -----------------------------------------------------------
                //    secondaryPort = (string)ConfigContainer.Instance().GetValue("");

                if (string.IsNullOrEmpty(secondaryHost))
                    secondaryHost = primaryHost;
                if (string.IsNullOrEmpty(secondaryPort))
                    secondaryPort = primaryPort;

                bool isStatServerOpened = false;
                if (!string.IsNullOrEmpty(primaryHost) && !string.IsNullOrEmpty(primaryPort))
                    isStatServerOpened = _statisticsConnectionManager.ConnectStatServer(primaryHost, primaryPort, secondaryHost, secondaryPort,
                                addpServerTimeOut, addClientTimeOut, warmStandbyTimeOut, warmStandbyAttempts);

                if (isStatServerOpened)
                {
                    _logger.Debug("Stat server connection opened");
                    Thread thAGStatistics = new Thread(delegate()
                    {
                        RequestAgentGroupStatistics();
                    });
                    thAGStatistics.Start();
                }
                else
                {
                    _logger.Warn("Stat server connection is not opened");
                }
                Datacontext.GetInstance().AllPersons = null;
            }
            catch (Exception ex)
            {
                _logger.Error("ConnectStatServer() : " + ex.Message.ToString());
            }
        }

        public void RequestAgentStatistics()
        {
            try
            {
                int count = 0;
                if (Datacontext.GetInstance().AllPersons != null && Datacontext.GetInstance().AllPersons.Count > 0)
                    count = Datacontext.GetInstance().AllPersons.Count;
                string[] _persons = new string[count];
                int index = 0;
                List<string> _subscribedUsers = new List<string>();
                if (ConfigContainer.Instance().AllKeys.Contains("teamcommunicator.enable-config-agent-groups") &&
                       ((string)ConfigContainer.Instance().GetValue("teamcommunicator.enable-config-agent-groups")).ToLower().Equals("true") &&
                       ConfigContainer.Instance().AllKeys.Contains("teamcommunicator.consult-agent-groups") &&
                       ((string)ConfigContainer.Instance().GetValue("teamcommunicator.consult-agent-groups")).ToString() != string.Empty)
                {
                    string[] agentGroups = ((string)ConfigContainer.Instance().GetValue("teamcommunicator.consult-agent-groups")).Split(',');
                    if (agentGroups != null && agentGroups.Length > 0)
                    {
                        foreach (string groupName in agentGroups)
                        {
                            Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.Queries.CfgAgentGroupQuery qAgentGroup = new CfgAgentGroupQuery();
                            qAgentGroup.TenantDbid = ConfigContainer.Instance().TenantDbId;
                            qAgentGroup.Name = groupName;
                            CfgAgentGroup agentGroup = ConfigContainer.Instance().ConfServiceObject.RetrieveObject<CfgAgentGroup>(qAgentGroup);
                            if (agentGroup != null && agentGroup.Agents != null && agentGroup.Agents.Count > 0)
                            {
                                _logger.Debug("Getting users from Agent Group : " + agentGroup.GroupInfo.Name);
                                foreach (CfgPerson person in agentGroup.Agents)
                                {
                                    if (person.UserName != _dataContext.UserName && !_subscribedUsers.Contains(person.UserName))
                                    {
                                        _persons[index] = person.EmployeeID;
                                        index++;
                                    }
                                }
                            }
                            agentGroup = null;
                        }
                    }
                    _persons = _persons.Where(x => !string.IsNullOrEmpty(x)).ToArray(); //remove null values and empty values in array
                    //Request stat server
                    //if (_persons != null && _persons.Length > 0 && !string.IsNullOrEmpty(_agentStatisticsName))

                }
                else
                {
                    _logger.Warn("No agent group configured for team communicator");
                    //Request all users
                    if (Datacontext.GetInstance().AllPersons != null && Datacontext.GetInstance().AllPersons.Count > 0)
                    {
                        foreach (var person in Datacontext.GetInstance().AllPersons)
                        {
                            if (person.UserName != _dataContext.UserName)
                            {
                                _persons[index] = person.EmployeeID;
                                index++;
                            }
                        }
                        _persons = _persons.Where(x => !string.IsNullOrEmpty(x)).ToArray();
                        //Request stat server
                        Datacontext.GetInstance().AllPersons = null;
                    }
                }
                if (_persons.Length > 0)
                {
                    string dynamicStatSectionName = string.Empty;
                    string businessAttributeName = string.Empty;

                    if (ConfigContainer.Instance().AllKeys.Contains("dynamic-stats") &&
                                !string.IsNullOrEmpty((string)ConfigContainer.Instance().GetValue("dynamic-stats")))
                        businessAttributeName = (string)ConfigContainer.Instance().GetValue("dynamic-stats");

                    if (ConfigContainer.Instance().AllKeys.Contains("dynamic-stats") &&
                                !string.IsNullOrEmpty((string)ConfigContainer.Instance().GetValue("dynamic.statistics.name")))
                        dynamicStatSectionName = (string)ConfigContainer.Instance().GetValue("dynamic.statistics.name");
                    RequestStatistics.RequestTeamCommunicatorStatistics(businessAttributeName, dynamicStatSectionName, _persons, StatisticObjectType.Agent, 1, 1);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.InnerException == null ? ex.Message : ex.InnerException.ToString());
            }
        }

        public void RequestAgentGroupStatistics()
        {
            try
            {
                string[] configuredAgentGroups = new string[5000];
                if (ConfigContainer.Instance().AllKeys.Contains("teamcommunicator.enable-config-agent-groups") &&
                       ((string)ConfigContainer.Instance().GetValue("teamcommunicator.enable-config-agent-groups")).ToLower().Equals("true") &&
                       ConfigContainer.Instance().AllKeys.Contains("teamcommunicator.consult-agent-groups") &&
                       ((string)ConfigContainer.Instance().GetValue("teamcommunicator.consult-agent-groups")).ToString() != string.Empty)
                {
                    string[] agentGroups = ((string)ConfigContainer.Instance().GetValue("teamcommunicator.consult-agent-groups")).Split(',');
                    int index = 0;
                    foreach (string groupName in agentGroups)
                    {
                        Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.Queries.CfgAgentGroupQuery qAgentGroup = new CfgAgentGroupQuery();
                        qAgentGroup.TenantDbid = ConfigContainer.Instance().TenantDbId;
                        qAgentGroup.Name = groupName;
                        CfgAgentGroup agentGroup = ConfigContainer.Instance().ConfServiceObject.RetrieveObject<CfgAgentGroup>(qAgentGroup);
                        if (agentGroup != null && agentGroup.Agents != null && agentGroup.Agents.Count > 0)
                        {
                            _logger.Info("Agent group : " + groupName + " found.");
                            if (!configuredAgentGroups.Contains(agentGroup.GroupInfo.Name))
                            {
                                configuredAgentGroups[index] = (agentGroup.GroupInfo.Name);
                                index++;
                            }
                        }
                        else
                        {
                            _logger.Warn("No such agent group is configured with name : " + groupName);
                        }
                    }
                }
                else
                {
                    _logger.Warn("No Agent group configured for Team communicator");
                }
                configuredAgentGroups = configuredAgentGroups.Where(x => !string.IsNullOrEmpty(x)).ToArray();
                string dynamicStatSectionName = string.Empty;
                string businessAttributeName = string.Empty;

                if (ConfigContainer.Instance().AllKeys.Contains("dynamic-stats") &&
                            !string.IsNullOrEmpty((string)ConfigContainer.Instance().GetValue("dynamic-stats")))
                {
                    businessAttributeName = (string)ConfigContainer.Instance().GetValue("dynamic-stats");
                    _logger.Debug("Dynamic stats ");
                }

                if (ConfigContainer.Instance().AllKeys.Contains("dynamic-stats") &&
                            !string.IsNullOrEmpty((string)ConfigContainer.Instance().GetValue("dynamic.statistics.name")))
                    dynamicStatSectionName = (string)ConfigContainer.Instance().GetValue("dynamic.statistics.name");
                RequestStatistics.RequestTeamCommunicatorStatistics(businessAttributeName, dynamicStatSectionName, configuredAgentGroups, StatisticObjectType.GroupAgents, 1, 1);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.InnerException == null ? ex.Message : ex.InnerException.ToString());
            }
        }

        /// <summary>
        /// Subscribes the specified listener.
        /// </summary>
        /// <param name="listener">The listener.</param>
        private void Subscribe(Pointel.Interactions.IPlugins.ITeamCommunicator listener)
        {
            try
            {
                Datacontext context = _dataContext;
            }
            catch (Exception generalException)
            {
                _logger.Error("Error in Subscribe " + generalException.ToString());
            }
        }

        /// <summary>
        /// Notifies the statistics.
        /// </summary>
        /// <param name="statHolderID">The stat holder unique identifier.</param>
        /// <param name="statisticsType">Type of the statistics.</param>
        /// <param name="statisticsObject">The statistics object.</param>
        public void NotifyStatistics(string statHolderID, string statisticsType, Dictionary<string, object> statisticsObject)
        {
            //System.Windows.Application.Current.Dispatcher.Invoke((Action)(delegate
            //{
            //}));
            UpdateTeamCommunicatorStatus(statHolderID, statisticsType, statisticsObject);
        }

        private Pointel.Interactions.IPlugins.AgentMediaStatus GetStatus(int state)
        {
            Pointel.Interactions.IPlugins.AgentMediaStatus status = AgentMediaStatus.None;
            switch (state)
            {
                case 23:
                    status = Pointel.Interactions.IPlugins.AgentMediaStatus.LoggedOut;
                    break;
                case 8:
                    status = Pointel.Interactions.IPlugins.AgentMediaStatus.NotReady;
                    break;
                case 4:
                    status = Pointel.Interactions.IPlugins.AgentMediaStatus.Ready;
                    break;
                case 2:
                    status = Pointel.Interactions.IPlugins.AgentMediaStatus.LoggedIn;
                    break;
                case 6:
                    status = Pointel.Interactions.IPlugins.AgentMediaStatus.Busy;
                    break;
                case 7:
                    status = Pointel.Interactions.IPlugins.AgentMediaStatus.Busy;
                    break;
                case 13:
                    status = Pointel.Interactions.IPlugins.AgentMediaStatus.Busy;
                    break;
                case 20:
                    status = Pointel.Interactions.IPlugins.AgentMediaStatus.Busy;
                    break;
                case 19:
                    status = Pointel.Interactions.IPlugins.AgentMediaStatus.Busy;
                    break;
                case 21:
                    status = Pointel.Interactions.IPlugins.AgentMediaStatus.Busy;
                    break;
                case 22:
                    status = Pointel.Interactions.IPlugins.AgentMediaStatus.Busy;
                    break;
                case 18:
                    status = Pointel.Interactions.IPlugins.AgentMediaStatus.Busy;
                    break;
                default:
                    break;
            }
            return status;

        }

        public void ReportingSuccessMessage(object sender, EventArgs e)
        {
            IMessage message = ((MessageEventArgs)e).Message;
            if (message == null)
                return;
            switch (message.Name)
            {
                case EventInfo.MessageName:
                    EventInfo eventInfo = (EventInfo)message;
                    NotifyStatistics(eventInfo);
                    //ThreadPool.QueueUserWorkItem(new WaitCallback(NotifyStatistics), eventInfo);
                    break;

                case EventError.MessageName:

                    break;

                case EventStatisticOpened.MessageName:
                    break;

                case EventStatisticClosed.MessageName:
                    break;

                case EventCurrentTargetStateSnapshot.MessageName:
                    break;
                default:
                    break;
            }
        }

        public void NotifyStatistics(object statisticsInfo)
        {
            var _statisticsInfo = statisticsInfo as Genesyslab.Platform.Reporting.Protocols.StatServer.Events.EventInfo;
            try
            {
                if (_statisticsInfo != null)
                {
                    _logger.Trace("Response from Stat Server : " + _statisticsInfo.ToString());
                    if (_statisticsInfo.StateValue != null)
                    {
                        Application.Current.Dispatcher.Invoke((Action)(delegate
                        {
                            int state = 0;
                            int interactionCount = 0;
                            #region Agent Statistics

                            if (_statisticsInfo.StateValue.ObjectType == StatisticObjectType.Agent && _statisticsInfo.StateValue is AgentStatus)
                            {

                                AgentStatus agentStatus = (AgentStatus)_statisticsInfo.StateValue;
                                state = Convert.ToInt32(agentStatus.Status);
                                AgentMediaStatus status = GetStatus(state);
                                DnCollection dnCollection = agentStatus.Place.DnStatuses;
                                string dn = string.Empty;
                                string place = string.Empty;
                                Dictionary<string, object> agentStatistics = new Dictionary<string, object>();
                                agentStatistics.Clear();
                                if (agentStatus.Place != null)
                                    place = agentStatus.Place.PlaceId;
                                agentStatistics.Add("AgentStatus", status);
                                agentStatistics.Add("VoiceStatus", AgentMediaStatus.LoggedOut);
                                agentStatistics.Add("DN", string.Empty);
                                agentStatistics.Add("Place", string.Empty);
                                agentStatistics.Add("ChatStatus", AgentMediaStatus.LoggedOut);
                                agentStatistics.Add("EmailStatus", AgentMediaStatus.LoggedOut);
                                if (dnCollection != null && dnCollection.Count > 0)
                                {
                                    foreach (DnStatus dnStatus in dnCollection)
                                    {
                                        try
                                        {
                                            if (dnStatus != null)
                                            {
                                                if (dnStatus.SwitchId != null)  //For media Voice
                                                {
                                                    AgentMediaStatus voiceStatus = GetStatus(dnStatus.Status.Value);
                                                    dn = dnStatus.DnId;
                                                    agentStatistics["VoiceStatus"] = voiceStatus;
                                                    agentStatistics["DN"] = !(string.IsNullOrEmpty(dn)) ? dn : string.Empty;
                                                    agentStatistics["Place"] = !(string.IsNullOrEmpty(place)) ? place : string.Empty;
                                                }
                                                else
                                                {
                                                    if (dnStatus.DnId.ToString().ToLower().Contains("chat"))  //For media Chat
                                                    {
                                                        AgentMediaStatus chatStatus = GetStatus(dnStatus.Status.Value);
                                                        agentStatistics["ChatStatus"] = chatStatus;
                                                        if (Datacontext.GetInstance().CurrentInteractionType == Pointel.Interactions.IPlugins.InteractionType.Chat)
                                                        {
                                                            for (int i = 0; i < dnStatus.Actions.Count; i++)
                                                            {
                                                                if (dnStatus.Actions[i].Action.ToString() == "CallInbound" ||
                                                                   dnStatus.Actions[i].Action.ToString() == "CallOutbound" ||
                                                                   dnStatus.Actions[i].Action.ToString() == "CallRinging")
                                                                    interactionCount++;
                                                            }
                                                        }
                                                        if (interactionCount < GetMaxInteractionCount(agentStatus.AgentId) && chatStatus == AgentMediaStatus.Busy)
                                                            agentStatistics["ChatStatus"] = AgentMediaStatus.ConditionallyReady;
                                                    }
                                                    else if (dnStatus.DnId.ToString().ToLower().Contains("email"))  //For media Email
                                                    {
                                                        AgentMediaStatus emailStatus = GetStatus(dnStatus.Status.Value);
                                                        agentStatistics["EmailStatus"] = emailStatus;
                                                        if (Datacontext.GetInstance().CurrentInteractionType == Pointel.Interactions.IPlugins.InteractionType.Email)
                                                        {
                                                            for (int i = 0; i < dnStatus.Actions.Count; i++)
                                                            {
                                                                if (dnStatus.Actions[i].Action.ToString() == "CallInbound" ||
                                                                   dnStatus.Actions[i].Action.ToString() == "CallOutbound" ||
                                                                   dnStatus.Actions[i].Action.ToString() == "CallRinging")
                                                                    interactionCount++;
                                                            }
                                                        }
                                                        if (interactionCount < GetMaxInteractionCount(agentStatus.AgentId) && emailStatus == AgentMediaStatus.Busy)
                                                            agentStatistics["EmailStatus"] = AgentMediaStatus.ConditionallyReady;

                                                    }
                                                }
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            _logger.Error("Error occurred as " + ex.Message);
                                        }
                                    }
                                }
                                UpdateTeamCommunicatorStatus(agentStatus.AgentId.ToString(), "Agent", agentStatistics);
                                agentStatistics = null;
                            }
                            #endregion

                            #region Agent Group Statistics

                            else if (_statisticsInfo.StateValue.ObjectType == StatisticObjectType.GroupAgents)
                            {
                                //Dictionary<string, object> agentGroupStatistics = new Dictionary<string, object>();
                                //agentGroupStatistics.Clear();
                                //AgentGroup aGroup = statisticsInfo.StateValue as AgentGroup;
                                //state = Convert.ToInt32(aGroup.Status);
                                //AgentMediaStatus groupStatus = GetStatus(state);
                                //agentGroupStatistics.Add("Status", groupStatus);
                                //UpdateTeamCommunicatorStatus(aGroup.GroupId, "AgentGroup", agentGroupStatistics);
                                //agentGroupStatistics = null;

                                AgentGroup aGroup = _statisticsInfo.StateValue as AgentGroup;
                                if (aGroup != null && aGroup.Agents.Count > 0)
                                {
                                    foreach (AgentStatus agentStatus in aGroup.Agents)
                                    {
                                        if (agentStatus.AgentId == "10001")
                                        {
                                            string ss = agentStatus.AgentId;
                                            string st = ss;
                                            string sb = st;
                                        }
                                        state = Convert.ToInt32(agentStatus.Status);
                                        AgentMediaStatus status = GetStatus(state);
                                        DnCollection dnCollection = agentStatus.Place.DnStatuses;
                                        string dn = string.Empty;
                                        string place = string.Empty;
                                        Dictionary<string, object> agentStatistics = new Dictionary<string, object>();
                                        agentStatistics.Clear();
                                        if (agentStatus.Place != null)
                                            place = agentStatus.Place.PlaceId;
                                        agentStatistics.Add("AgentStatus", status);
                                        agentStatistics.Add("VoiceStatus", AgentMediaStatus.LoggedOut);
                                        agentStatistics.Add("DN", string.Empty);
                                        agentStatistics.Add("Place", string.Empty);
                                        agentStatistics.Add("ChatStatus", AgentMediaStatus.LoggedOut);
                                        agentStatistics.Add("EmailStatus", AgentMediaStatus.LoggedOut);
                                        if (dnCollection != null && dnCollection.Count > 0)
                                        {
                                            foreach (DnStatus dnStatus in dnCollection)
                                            {
                                                try
                                                {
                                                    if (dnStatus != null)
                                                    {
                                                        if (dnStatus.SwitchId != null)  //For media Voice
                                                        {
                                                            AgentMediaStatus voiceStatus = GetStatus(dnStatus.Status.Value);
                                                            dn = dnStatus.DnId;
                                                            agentStatistics["VoiceStatus"] = voiceStatus;
                                                            agentStatistics["DN"] = !(string.IsNullOrEmpty(dn)) ? dn : string.Empty;
                                                            agentStatistics["Place"] = !(string.IsNullOrEmpty(place)) ? place : string.Empty;
                                                        }
                                                        else
                                                        {
                                                            if (dnStatus.DnId.ToString().ToLower().Contains("chat"))  //For media Chat
                                                            {
                                                                AgentMediaStatus chatStatus = GetStatus(dnStatus.Status.Value);
                                                                agentStatistics["ChatStatus"] = chatStatus;
                                                                if (Datacontext.GetInstance().CurrentInteractionType == Pointel.Interactions.IPlugins.InteractionType.Chat)
                                                                {
                                                                    for (int i = 0; i < dnStatus.Actions.Count; i++)
                                                                    {
                                                                        if (dnStatus.Actions[i].Action.ToString() == "CallInbound" ||
                                                                           dnStatus.Actions[i].Action.ToString() == "CallOutbound" ||
                                                                           dnStatus.Actions[i].Action.ToString() == "CallRinging")
                                                                            interactionCount++;
                                                                    }
                                                                }
                                                                if (interactionCount < GetMaxInteractionCount(agentStatus.AgentId) && chatStatus == AgentMediaStatus.Busy)
                                                                    agentStatistics["ChatStatus"] = AgentMediaStatus.ConditionallyReady;
                                                            }
                                                            else if (dnStatus.DnId.ToString().ToLower().Contains("email"))  //For media Email
                                                            {
                                                                AgentMediaStatus emailStatus = GetStatus(dnStatus.Status.Value);
                                                                agentStatistics["EmailStatus"] = emailStatus;
                                                                if (Datacontext.GetInstance().CurrentInteractionType == Pointel.Interactions.IPlugins.InteractionType.Email)
                                                                {
                                                                    for (int i = 0; i < dnStatus.Actions.Count; i++)
                                                                    {
                                                                        if (dnStatus.Actions[i].Action.ToString() == "CallInbound" ||
                                                                           dnStatus.Actions[i].Action.ToString() == "CallOutbound" ||
                                                                           dnStatus.Actions[i].Action.ToString() == "CallRinging")
                                                                            interactionCount++;
                                                                    }
                                                                }
                                                                if (interactionCount < GetMaxInteractionCount(agentStatus.AgentId) && emailStatus == AgentMediaStatus.Busy)
                                                                    agentStatistics["EmailStatus"] = AgentMediaStatus.ConditionallyReady;
                                                            }
                                                        }
                                                    }
                                                }
                                                catch (Exception ex)
                                                {
                                                    _logger.Error("Error occurred as " + ex.Message);
                                                }
                                            }
                                        }
                                        UpdateTeamCommunicatorStatus(agentStatus.AgentId.ToString(), "Agent", agentStatistics);
                                    }

                                }
                                if (_statisticsInfo.ReferenceId >= 5000)
                                    Request.RequestStatistics.CloseStatistics(_statisticsInfo.ReferenceId);
                            }

                            #endregion
                        }));
                    }
                }
            }
            catch (Exception commonException)
            {
                _logger.Error("NotifyAIDStatistics : " + commonException.Message.ToString());
            }
            finally
            {
                _statisticsInfo = null;
            }
        }

        void UpdateTeamCommunicatorStatus(string statHolderID, string statisticsType, Dictionary<string, object> statisticsObject)
        {

            int index = 0;
            _dataContext.IsStatAlive = true;
            #region Agent Group

            if (statisticsType == "AgentGroup")
            {
                Pointel.Interactions.IPlugins.AgentMediaStatus status = AgentMediaStatus.LoggedOut;
                if (statisticsObject.ContainsKey("Status"))
                    status = (Pointel.Interactions.IPlugins.AgentMediaStatus)statisticsObject["Status"];

                if (_dataContext.hshAgentGroupStatus.ContainsKey(statHolderID))
                    _dataContext.hshAgentGroupStatus[statHolderID] = status;
                else
                    _dataContext.hshAgentGroupStatus.Add(statHolderID, status);

                #region Update Status of Current Collection Objects

                if (_dataContext.TeamCommunicator.Count > 0)
                {
                    //var item = _dataContext.TeamCommunicator.FirstOrDefault(i => i.UniqueIdentity == statHolderID);
                    //if (item != null)
                    //{
                    //    switch(status)
                    //    {
                    //        case "LoggedOut":
                    //            callToolTip = "Add to Favorites";
                    //            callImageSource = new BitmapImage(new Uri("/Agent.Interaction.Desktop;component/Images/Favourite.png", UriKind.Relative));
                    //            statusImageSource = new BitmapImage(new Uri("/Agent.Interaction.Desktop;component/Images/Logout-state.png", UriKind.Relative));
                    //            break;

                    //        case "Busy":
                    //            status = "Busy";
                    //            callToolTip = "Add to Favorites";
                    //            callImageSource = new BitmapImage(new Uri("/Agent.Interaction.Desktop;component/Images/Favourite.png", UriKind.Relative));
                    //            statusImageSource = new BitmapImage(new Uri("/Agent.Interaction.Desktop;component/Images/Voice/Call.png", UriKind.Relative));
                    //            break;

                    //        case "Ready":
                    //            callToolTip = "Establish a new Phone Call";
                    //            callImageSource = new BitmapImage(new Uri("/Agent.Interaction.Desktop;component/Images/Voice.Short.png", UriKind.Relative));
                    //            statusImageSource = new BitmapImage(new Uri("/Agent.Interaction.Desktop;component/Images/Ready.png", UriKind.Relative));
                    //            break;

                    //        case "NotReady":
                    //            callToolTip = "Establish a new Phone Call";
                    //            callImageSource = new BitmapImage(new Uri("/Agent.Interaction.Desktop;component/Images/Voice.Short.png", UriKind.Relative));
                    //            statusImageSource = new BitmapImage(new Uri("/Agent.Interaction.Desktop;component/Images/Not_Ready.png", UriKind.Relative));
                    //            break;

                    //        default:
                    //            status = "";
                    //            callToolTip = "Add to Favorites";
                    //            callImageSource = new BitmapImage(new Uri("/Agent.Interaction.Desktop;component/Images/Favourite.png", UriKind.Relative));
                    //            statusImageSource = new BitmapImage(new Uri("/Agent.Interaction.Desktop;component/Images/Disable.btn.png", UriKind.Relative));
                    //            break;
                    //    }
                    //    index = _dataContext.TeamCommunicator.IndexOf(item);
                    //    item.Status = status;
                    //    item.ImageToolTip = callToolTip;
                    //    item.StatusImageSource = statusImageSource;
                    //    item.SearchIconImageSource = callImageSource;
                    //    _dataContext.TeamCommunicator[index] = item;
                    //_statusUpdation.Invoke();
                    //}
                }
                #endregion
            }

            #endregion

            #region Agent

            if (statisticsType == "Agent")
            {
                Pointel.Interactions.IPlugins.AgentMediaStatus status = AgentMediaStatus.LoggedOut;
                Pointel.Interactions.IPlugins.AgentMediaStatus voiceStatus = AgentMediaStatus.LoggedOut;
                Pointel.Interactions.IPlugins.AgentMediaStatus emailStatus = AgentMediaStatus.LoggedOut;
                Pointel.Interactions.IPlugins.AgentMediaStatus chatStatus = AgentMediaStatus.LoggedOut;
                string place = "";
                string dn = "";
                if (statisticsObject.ContainsKey("AgentStatus"))
                    status = (Pointel.Interactions.IPlugins.AgentMediaStatus)statisticsObject["AgentStatus"];
                if (statisticsObject.ContainsKey("VoiceStatus"))
                    voiceStatus = (Pointel.Interactions.IPlugins.AgentMediaStatus)statisticsObject["VoiceStatus"];
                if (statisticsObject.ContainsKey("ChatStatus"))
                    chatStatus = (Pointel.Interactions.IPlugins.AgentMediaStatus)statisticsObject["ChatStatus"];
                if (statisticsObject.ContainsKey("EmailStatus"))
                    emailStatus = (Pointel.Interactions.IPlugins.AgentMediaStatus)statisticsObject["EmailStatus"];
                if (statisticsObject.ContainsKey("DN"))
                    dn = statisticsObject["DN"].ToString();
                if (statisticsObject.ContainsKey("Place"))
                    place = statisticsObject["Place"].ToString();

                if (statHolderID == "10001")
                {
                    string ss = statHolderID;
                    string st = ss;
                    string sb = st;
                }

                //if (chatStatus == null)
                //    chatStatus = Pointel.Interactions.IPlugins.MediaStatus.LoggedOut;
                //if (emailStatus == null)
                //    emailStatus = Pointel.Interactions.IPlugins.MediaStatus.LoggedOut;
                //if (voiceStatus == null)
                //    voiceStatus = Pointel.Interactions.IPlugins.MediaStatus.LoggedOut;

                if (_dataContext.hshAgentStatus.ContainsKey(statHolderID))//Stores Agent status
                    _dataContext.hshAgentStatus[statHolderID] = (Pointel.Interactions.IPlugins.AgentMediaStatus)status;
                else
                    _dataContext.hshAgentStatus.Add(statHolderID, status);

                if (_dataContext.hshAgentPlace.ContainsKey(statHolderID))//Stores Agent Place
                    _dataContext.hshAgentPlace[statHolderID] = place;
                else
                    _dataContext.hshAgentPlace.Add(statHolderID, place);

                if (_dataContext.hshAgentVoiceStatus.ContainsKey(statHolderID))//Stores Agent Voice status
                    _dataContext.hshAgentVoiceStatus[statHolderID] = (Pointel.Interactions.IPlugins.AgentMediaStatus)voiceStatus;
                else
                    _dataContext.hshAgentVoiceStatus.Add(statHolderID, voiceStatus);

                if (_dataContext.hshAgentEmailStatus.ContainsKey(statHolderID))//Stores Agent Email status
                    _dataContext.hshAgentEmailStatus[statHolderID] = (Pointel.Interactions.IPlugins.AgentMediaStatus)emailStatus;
                else
                    _dataContext.hshAgentEmailStatus.Add(statHolderID, emailStatus);

                if (_dataContext.hshAgentChatStatus.ContainsKey(statHolderID))//Stores Agent Chat status
                    _dataContext.hshAgentChatStatus[statHolderID] = (Pointel.Interactions.IPlugins.AgentMediaStatus)chatStatus;
                else
                    _dataContext.hshAgentChatStatus.Add(statHolderID, chatStatus);

                if (_dataContext.hshAgentDN.ContainsKey(statHolderID))//Stores Agent DN
                    _dataContext.hshAgentDN[statHolderID] = dn;
                else
                    _dataContext.hshAgentDN.Add(statHolderID, dn);

                #region Update Status of Current Collection Objects

                if (_dataContext.TeamCommunicator.Count > 0)
                {
                    //System.Windows.Application.Current.Dispatcher.Invoke((Action)(delegate
                    //{
                    //Get the username of the agent using the Employee ID
                    string userName = "";
                    if (_dataContext.hshAgentEmployeeIdUserName.ContainsKey(statHolderID))
                    {
                        userName = _dataContext.hshAgentEmployeeIdUserName[statHolderID].ToString();
                        //var name = (from row in _dataContext.dtPersons.AsEnumerable()
                        //            where
                        //                row.Field<string>("EmployeeID").ToString() == statHolderID
                        //            select row.Field<string>("UserName")).First<string>();
                        //if (name != null)
                        //    userName = name;
                        statusImageSource = null;
                        if (_dataContext.CurrentInteractionType == Pointel.Interactions.IPlugins.InteractionType.Email)
                            status = emailStatus;
                        else if (_dataContext.CurrentInteractionType == Pointel.Interactions.IPlugins.InteractionType.Chat)
                            status = chatStatus;
                        else if (_dataContext.CurrentInteractionType == Pointel.Interactions.IPlugins.InteractionType.Voice)
                            status = voiceStatus;
                        else if (_dataContext.CurrentInteractionType == Pointel.Interactions.IPlugins.InteractionType.WorkBin)
                            status = emailStatus;
                        List<DataRow> foundRows = _dataContext.dtFavorites.Rows.Cast<DataRow>().Where(x => x["UniqueIdentity"].ToString() == userName).ToList();
                        var item = _dataContext.TeamCommunicator.FirstOrDefault(i => i.UniqueIdentity == userName);
                        #region Old
                        //if (item != null)
                        //{
                        //    //if (_dataContext.CurrentInteractionType != Pointel.Interactions.IPlugins.InteractionType.WorkBin)
                        //    {
                        //        switch (status)
                        //        {
                        //            case Pointel.Interactions.IPlugins.AgentMediaStatus.LoggedOut:
                        //                statusImageSource = new BitmapImage(new Uri("/Agent.Interaction.Desktop;component/Images/Ready.png", UriKind.Relative));
                        //                if (_dataContext.CurrentInteractionType == Pointel.Interactions.IPlugins.InteractionType.WorkBin)
                        //                {
                        //                    if (_dataContext.SelectedOperationType == Pointel.Interactions.IPlugins.OperationType.Queue)
                        //                    {
                        //                        callToolTip = "Move to Queue";
                        //                        callImageSource = new BitmapImage(new Uri("/Agent.Interaction.Desktop;component/Images/move.to.queue.png", UriKind.Relative));
                        //                    }
                        //                    else if (_dataContext.SelectedOperationType == Pointel.Interactions.IPlugins.OperationType.Queue)
                        //                    {
                        //                        callToolTip = "Select a Workbin";
                        //                        callImageSource = new BitmapImage(new Uri("/Agent.Interaction.Desktop;component/Images/move.to.workbin.png", UriKind.Relative));
                        //                    }
                        //                    else if (_dataContext.SelectedOperationType == Pointel.Interactions.IPlugins.OperationType.Queue)
                        //                    {
                        //                        callToolTip = "Start a Consult Interaction";
                        //                        callImageSource = new BitmapImage(new Uri("/Agent.Interaction.Desktop;component/Images/Email.png", UriKind.Relative));
                        //                    }

                        //                    break;
                        //                }
                        //                callToolTip = "Add to Favorites";
                        //                if (foundRows != null)
                        //                {
                        //                    callToolTip = "Remove from Favorites";
                        //                    callImageSource = new BitmapImage(new Uri("/Agent.Interaction.Desktop;component/Images/Favourite.png", UriKind.Relative));
                        //                }
                        //                else
                        //                {
                        //                    callToolTip = "Add to Favorites";
                        //                    callImageSource = new BitmapImage(new Uri("/Agent.Interaction.Desktop;component/Images/Favourite.Remove.png.png", UriKind.Relative));
                        //                }
                        //                statusImageSource = new BitmapImage(new Uri("/Agent.Interaction.Desktop;component/Images/Logout-state.png", UriKind.Relative));
                        //                break;

                        //            case Pointel.Interactions.IPlugins.AgentMediaStatus.Busy:
                        //                status = Pointel.Interactions.IPlugins.AgentMediaStatus.Busy;
                        //                statusImageSource = new BitmapImage(new Uri("/Agent.Interaction.Desktop;component/Images/Busy.png", UriKind.Relative));
                        //                if (_dataContext.CurrentInteractionType == Pointel.Interactions.IPlugins.InteractionType.WorkBin)
                        //                {
                        //                    if (_dataContext.SelectedOperationType == Pointel.Interactions.IPlugins.OperationType.Queue)
                        //                    {
                        //                        callToolTip = "Move to Queue";
                        //                        callImageSource = new BitmapImage(new Uri("/Agent.Interaction.Desktop;component/Images/move.to.queue.png", UriKind.Relative));
                        //                    }
                        //                    else if (_dataContext.SelectedOperationType == Pointel.Interactions.IPlugins.OperationType.Queue)
                        //                    {
                        //                        callToolTip = "Select a Workbin";
                        //                        callImageSource = new BitmapImage(new Uri("/Agent.Interaction.Desktop;component/Images/move.to.workbin.png", UriKind.Relative));
                        //                    }
                        //                    else if (_dataContext.SelectedOperationType == Pointel.Interactions.IPlugins.OperationType.Queue)
                        //                    {
                        //                        callToolTip = "Start a Consult Interaction";
                        //                        callImageSource = new BitmapImage(new Uri("/Agent.Interaction.Desktop;component/Images/Email.png", UriKind.Relative));
                        //                    }

                        //                    break;
                        //                }
                        //                callToolTip = "Add to Favorites";
                        //                if (foundRows != null)
                        //                {
                        //                    callToolTip = "Remove from Favorites";
                        //                    callImageSource = new BitmapImage(new Uri("/Agent.Interaction.Desktop;component/Images/Favourite.png", UriKind.Relative));
                        //                }
                        //                else
                        //                {
                        //                    callToolTip = "Add to Favorites";
                        //                    callImageSource = new BitmapImage(new Uri("/Agent.Interaction.Desktop;component/Images/Favourite.Remove.png.png", UriKind.Relative));
                        //                }
                        //                break;

                        //            case Pointel.Interactions.IPlugins.AgentMediaStatus.Ready:
                        //                if (_dataContext.CurrentInteractionType == Pointel.Interactions.IPlugins.InteractionType.Voice)
                        //                {
                        //                    callToolTip = "Establish a new Phone Call";
                        //                    callImageSource = new BitmapImage(new Uri("/Agent.Interaction.Desktop;component/Images/Voice.Short.png", UriKind.Relative));
                        //                }
                        //                else if (_dataContext.CurrentInteractionType == Pointel.Interactions.IPlugins.InteractionType.Chat)
                        //                {
                        //                    callToolTip = "Start a Consult Interaction";
                        //                    callImageSource = new BitmapImage(new Uri("/Agent.Interaction.Desktop;component/Images/Chat.png", UriKind.Relative));
                        //                }
                        //                else if (_dataContext.CurrentInteractionType == Pointel.Interactions.IPlugins.InteractionType.Email)
                        //                {
                        //                    callToolTip = "Start a Consult Interaction";
                        //                    callImageSource = new BitmapImage(new Uri("/Agent.Interaction.Desktop;component/Images/Email.png", UriKind.Relative));
                        //                }
                        //                else if (_dataContext.CurrentInteractionType == Pointel.Interactions.IPlugins.InteractionType.WorkBin)
                        //                {
                        //                    if (_dataContext.SelectedOperationType == Pointel.Interactions.IPlugins.OperationType.Queue)
                        //                    {
                        //                        callToolTip = "Move to Queue";
                        //                        callImageSource = new BitmapImage(new Uri("/Agent.Interaction.Desktop;component/Images/move.to.queue.png", UriKind.Relative));
                        //                    }
                        //                    else if (_dataContext.SelectedOperationType == Pointel.Interactions.IPlugins.OperationType.Queue)
                        //                    {
                        //                        callToolTip = "Select a Workbin";
                        //                        callImageSource = new BitmapImage(new Uri("/Agent.Interaction.Desktop;component/Images/move.to.workbin.png", UriKind.Relative));
                        //                    }
                        //                    else if (_dataContext.SelectedOperationType == Pointel.Interactions.IPlugins.OperationType.Queue)
                        //                    {
                        //                        callToolTip = "Start a Consult Interaction";
                        //                        callImageSource = new BitmapImage(new Uri("/Agent.Interaction.Desktop;component/Images/Email.png", UriKind.Relative));
                        //                    }
                        //                }
                        //                statusImageSource = new BitmapImage(new Uri("/Agent.Interaction.Desktop;component/Images/Ready.png", UriKind.Relative));
                        //                break;

                        //            case Pointel.Interactions.IPlugins.AgentMediaStatus.NotReady:
                        //                if (_dataContext.CurrentInteractionType == Pointel.Interactions.IPlugins.InteractionType.Voice)
                        //                {
                        //                    callToolTip = "Establish a new Phone Call";
                        //                    callImageSource = new BitmapImage(new Uri("/Agent.Interaction.Desktop;component/Images/Voice.Short.png", UriKind.Relative));
                        //                }
                        //                else if (_dataContext.CurrentInteractionType == Pointel.Interactions.IPlugins.InteractionType.Chat)
                        //                {
                        //                    callToolTip = "Start a Consult Interaction";
                        //                    callImageSource = new BitmapImage(new Uri("/Agent.Interaction.Desktop;component/Images/Chat.png", UriKind.Relative));
                        //                }
                        //                else if (_dataContext.CurrentInteractionType == Pointel.Interactions.IPlugins.InteractionType.Email)
                        //                {
                        //                    callToolTip = "Start a Consult Interaction";
                        //                    callImageSource = new BitmapImage(new Uri("/Agent.Interaction.Desktop;component/Images/Email.png", UriKind.Relative));
                        //                }
                        //                else if (_dataContext.CurrentInteractionType == Pointel.Interactions.IPlugins.InteractionType.WorkBin)
                        //                {
                        //                    if (_dataContext.SelectedOperationType == Pointel.Interactions.IPlugins.OperationType.Queue)
                        //                    {
                        //                        callToolTip = "Move to Queue";
                        //                        callImageSource = new BitmapImage(new Uri("/Agent.Interaction.Desktop;component/Images/move.to.queue.png", UriKind.Relative));
                        //                    }
                        //                    else if (_dataContext.SelectedOperationType == Pointel.Interactions.IPlugins.OperationType.Workbin)
                        //                    {
                        //                        callToolTip = "Select a Workbin";
                        //                        callImageSource = new BitmapImage(new Uri("/Agent.Interaction.Desktop;component/Images/move.to.workbin.png", UriKind.Relative));
                        //                    }
                        //                    else if (_dataContext.SelectedOperationType == Pointel.Interactions.IPlugins.OperationType.Transfer)
                        //                    {
                        //                        callToolTip = "Start a Consult Interaction";
                        //                        callImageSource = new BitmapImage(new Uri("/Agent.Interaction.Desktop;component/Images/Email.png", UriKind.Relative));
                        //                    }
                        //                }
                        //                statusImageSource = new BitmapImage(new Uri("/Agent.Interaction.Desktop;component/Images/Not_Ready.png", UriKind.Relative));
                        //                break;

                        //            default:
                        //                if (foundRows != null)
                        //                {
                        //                    callToolTip = "Remove from Favorites";
                        //                    callImageSource = new BitmapImage(new Uri("/Agent.Interaction.Desktop;component/Images/Favourite.png", UriKind.Relative));
                        //                }
                        //                else
                        //                {
                        //                    callToolTip = "Add to Favorites";
                        //                    callImageSource = new BitmapImage(new Uri("/Agent.Interaction.Desktop;component/Images/Favourite.Remove.png.png", UriKind.Relative));
                        //                }
                        //                statusImageSource = new BitmapImage(new Uri("/Agent.Interaction.Desktop;component/Images/Disable.btn.png", UriKind.Relative));
                        //                break;
                        //        }
                        //    }

                        #endregion
                        if (item != null)
                        {
                            Application.Current.Dispatcher.Invoke((Action)(delegate
                            {


                                string callToolTip = "";
                                //string toolTip = "";
                                //string favToolTip = "";
                                callToolTip = GetStatusData(status);

                                index = _dataContext.TeamCommunicator.IndexOf(item);
                                if (index >= 0)
                                {
                                    if (_dataContext.CurrentInteractionType == InteractionType.Email && !Datacontext.GetInstance().AvailableEmailStatus.Contains(status))
                                    {
                                        _dataContext.TeamCommunicator.RemoveAt(index);
                                    }
                                    else if (_dataContext.CurrentInteractionType == InteractionType.Chat && !Datacontext.GetInstance().AvailableChatStatus.Contains(status))
                                    {
                                        _dataContext.TeamCommunicator.RemoveAt(index);
                                    }
                                    else
                                    {
                                        item.Status = status.ToString();
                                        item.ImageToolTip = callToolTip;
                                        item.StatusImageSource = statusImageSource;
                                        item.SearchIconImageSource = callImageSource;
                                        _dataContext.TeamCommunicator.RemoveAt(index);
                                        _dataContext.TeamCommunicator.Insert(index, item);
                                    }
                                }
                                //_dataContext.TeamCommunicator[index] = item;
                                //_statusUpdation.Invoke();
                                Datacontext.GetInstance().InternalTargets = Datacontext.GetInstance().TeamCommunicator.Count.ToString() + " " + "Matching Internal Targets";
                            }));
                        }
                        //}
                    }
                    //}));
                }
                #endregion
            }

            #endregion

            statisticsObject = null;
        }

        /// <summary>
        /// Notifies the stat server status.
        /// </summary>
        /// <param name="status">The status.</param>
        public void NotifyStatServerStatus(string status)
        {
            //System.Windows.Application.Current.Dispatcher.Invoke((Action)(delegate
            //{
            try
            {

                if (status.ToLower().Contains("start"))
                {
                    _dataContext.IsStatAlive = true;
                }
                else
                {
                    if (!_dataContext.IsStatAlive)
                        return;
                    _dataContext.IsStatAlive = false;
                    foreach (string agent in _dataContext.hshAgentStatus.Keys)
                    {
                        //_dataContext.hshAgentStatus[agent] = AgentMediaStatus.None;
                        if (_dataContext.hshAgentVoiceStatus.ContainsKey(agent))
                            _dataContext.hshAgentVoiceStatus[agent] = AgentMediaStatus.None;
                        if (_dataContext.hshAgentChatStatus.ContainsKey(agent))
                            _dataContext.hshAgentChatStatus[agent] = AgentMediaStatus.None;
                        if (_dataContext.hshAgentEmailStatus.ContainsKey(agent))
                            _dataContext.hshAgentEmailStatus[agent] = AgentMediaStatus.None;
                    }
                    //if (_dataContext.TeamCommunicator != null && _dataContext.TeamCommunicator.Count > 0)
                    //{
                    //    _statusUpdation.Invoke();
                    //}
                }

            }
            catch (Exception ex)
            {
                _logger.Error(ex.InnerException == null ? ex.Message : ex.InnerException.ToString());
            }//}));
        }

        /// <summary>
        /// Disconnects this instance.
        /// </summary>
        public void Disconnect()
        {
            try
            {
                if (_dataContext.Writer != null)
                {
                    _dataContext.Writer.DeleteAll();
                    _dataContext.Writer.Close();
                }
                if (_dataContext.Analyzer != null)
                    _dataContext.Analyzer.Close();
                if (_dataContext.Directory != null)
                    _dataContext.Directory.Close();
            }
            catch (Exception generalException)
            {
                _logger.Error("Error occurred while disconnecting Team communicator : " +
                            ((generalException.InnerException == null) ? generalException.Message : generalException.InnerException.ToString()));
            }
        }

        /// <summary>
        /// Notifies the cme objects.
        /// </summary>
        /// <param name="CMEObjects">The cme objects.</param>
        public void NotifyCMEObjects(object CMEObjects)
        {
            Initialize((Dictionary<string, object>)CMEObjects);
        }

        #endregion

        #region Method Definitions

        /// <summary>
        /// Gets the status data.
        /// </summary>
        /// <param name="status">The status.</param>
        /// <returns></returns>
        private string GetStatusData(Pointel.Interactions.IPlugins.AgentMediaStatus status)
        {
            statusImageSource = null;
            try
            {
                switch (status)
                {
                    case Pointel.Interactions.IPlugins.AgentMediaStatus.LoggedOut:

                        if (_dataContext.CurrentInteractionType == Pointel.Interactions.IPlugins.InteractionType.WorkBin)
                        {
                            status = Pointel.Interactions.IPlugins.AgentMediaStatus.LoggedOut;
                            statusImageSource = new BitmapImage(new Uri("/Pointel.Interactions.TeamCommunicator;component/Images/Logout-state.png", UriKind.Relative));
                            if (_dataContext.SelectedOperationType == Pointel.Interactions.IPlugins.OperationType.Queue)
                            {
                                callToolTip = "Move to Queue";
                                statusImageSource = null;
                                callImageSource = new BitmapImage(new Uri("/Pointel.Interactions.TeamCommunicator;component/Images/move.to.queue.png", UriKind.Relative));
                            }
                            else if (_dataContext.SelectedOperationType == Pointel.Interactions.IPlugins.OperationType.Workbin)
                            {
                                callToolTip = "Select a Workbin";
                                callImageSource = new BitmapImage(new Uri("/Pointel.Interactions.TeamCommunicator;component/Images/move.to.workbin.png", UriKind.Relative));
                            }
                            else
                            {
                                callToolTip = "Add to Favorites";
                                callImageSource = new BitmapImage(new Uri("/Pointel.Interactions.TeamCommunicator;component/Images/Favourite.png", UriKind.Relative));
                            }
                        }
                        else if (_dataContext.CurrentInteractionType == Pointel.Interactions.IPlugins.InteractionType.Contact)
                        {
                            callToolTip = "Add e-mail address";
                            callImageSource = new BitmapImage(new Uri("/Pointel.Interactions.TeamCommunicator;component/Images/Email.png", UriKind.Relative));
                            statusImageSource = null;
                        }
                        else
                        {
                            callToolTip = "Add to Favorites";
                            callImageSource = new BitmapImage(new Uri("/Pointel.Interactions.TeamCommunicator;component/Images/Favourite.png", UriKind.Relative));
                            statusImageSource = new BitmapImage(new Uri("/Pointel.Interactions.TeamCommunicator;component/Images/Logout-state.png", UriKind.Relative));
                        }
                        break;
                    case Pointel.Interactions.IPlugins.AgentMediaStatus.Busy:
                        status = Pointel.Interactions.IPlugins.AgentMediaStatus.Busy;
                        statusImageSource = new BitmapImage(new Uri("/Pointel.Interactions.TeamCommunicator;component/Images/Busy.png", UriKind.Relative));
                        if (_dataContext.CurrentInteractionType == Pointel.Interactions.IPlugins.InteractionType.WorkBin)
                        {
                            if (_dataContext.SelectedOperationType == Pointel.Interactions.IPlugins.OperationType.Queue)
                            {
                                callToolTip = "Move to Queue";
                                callImageSource = new BitmapImage(new Uri("/Pointel.Interactions.TeamCommunicator;component/Images/move.to.queue.png", UriKind.Relative));
                                statusImageSource = null;
                            }
                            else if (_dataContext.SelectedOperationType == Pointel.Interactions.IPlugins.OperationType.Workbin)
                            {
                                callToolTip = "Select a Workbin";
                                callImageSource = new BitmapImage(new Uri("/Pointel.Interactions.TeamCommunicator;component/Images/move.to.workbin.png", UriKind.Relative));
                            }
                            else
                            {
                                callToolTip = "Add to Favorites";
                                callImageSource = new BitmapImage(new Uri("/Pointel.Interactions.TeamCommunicator;component/Images/Favourite.png", UriKind.Relative));
                            }
                        }
                        else
                        {
                            callToolTip = "Add to Favorites";
                            callImageSource = new BitmapImage(new Uri("/Pointel.Interactions.TeamCommunicator;component/Images/Favourite.png", UriKind.Relative));
                        }
                        break;

                    case Pointel.Interactions.IPlugins.AgentMediaStatus.ConditionallyReady:
                        status = Pointel.Interactions.IPlugins.AgentMediaStatus.ConditionallyReady;
                        statusImageSource = new BitmapImage(new Uri("/Pointel.Interactions.TeamCommunicator;component/Images/Busy.png", UriKind.Relative));
                        switch (_dataContext.CurrentInteractionType)
                        {
                            case Pointel.Interactions.IPlugins.InteractionType.Email:
                                callToolTip = "Start a Consult Interaction";
                                callImageSource = new BitmapImage(new Uri("/Pointel.Interactions.TeamCommunicator;component/Images/Email.png", UriKind.Relative));
                                break;

                            case Pointel.Interactions.IPlugins.InteractionType.Chat:
                                callToolTip = "Start a Consult Interaction";
                                callImageSource = new BitmapImage(new Uri("/Pointel.Interactions.TeamCommunicator;component/Images/Chat.png", UriKind.Relative));
                                break;

                            case Pointel.Interactions.IPlugins.InteractionType.WorkBin:
                                if (_dataContext.SelectedOperationType == Pointel.Interactions.IPlugins.OperationType.Queue)
                                {
                                    callToolTip = "Move to Queue";
                                    statusImageSource = null;
                                    callImageSource = new BitmapImage(new Uri("/Pointel.Interactions.TeamCommunicator;component/Images/move.to.queue.png", UriKind.Relative));
                                }
                                else if (_dataContext.SelectedOperationType == Pointel.Interactions.IPlugins.OperationType.Workbin)
                                {
                                    callToolTip = "Select a Workbin";
                                    callImageSource = new BitmapImage(new Uri("/Pointel.Interactions.TeamCommunicator;component/Images/move.to.workbin.png", UriKind.Relative));
                                }
                                else if (_dataContext.SelectedOperationType == Pointel.Interactions.IPlugins.OperationType.Transfer)
                                {
                                    callToolTip = "Start a Consult Interaction";
                                    callImageSource = new BitmapImage(new Uri("/Pointel.Interactions.TeamCommunicator;component/Images/Email.png", UriKind.Relative));
                                }
                                break;

                            default:
                                callToolTip = "";
                                callImageSource = null;
                                break;
                        }
                        break;
                    case Pointel.Interactions.IPlugins.AgentMediaStatus.Ready:
                        statusImageSource = new BitmapImage(new Uri("/Pointel.Interactions.TeamCommunicator;component/Images/Ready.png", UriKind.Relative));
                        status = Pointel.Interactions.IPlugins.AgentMediaStatus.Ready;
                        switch (_dataContext.CurrentInteractionType)
                        {
                            case Pointel.Interactions.IPlugins.InteractionType.Email:
                                callToolTip = "Start a Consult Interaction";
                                callImageSource = new BitmapImage(new Uri("/Pointel.Interactions.TeamCommunicator;component/Images/Email.png", UriKind.Relative));
                                break;

                            case Pointel.Interactions.IPlugins.InteractionType.Voice:
                                callToolTip = "Establish a new Phone Call";
                                callImageSource = new BitmapImage(new Uri("/Pointel.Interactions.TeamCommunicator;component/Images/Voice.Short.png", UriKind.Relative));
                                break;

                            case Pointel.Interactions.IPlugins.InteractionType.Chat:
                                callToolTip = "Start a Consult Interaction";
                                callImageSource = new BitmapImage(new Uri("/Pointel.Interactions.TeamCommunicator;component/Images/Chat.png", UriKind.Relative));
                                break;

                            case Pointel.Interactions.IPlugins.InteractionType.WorkBin:
                                if (_dataContext.SelectedOperationType == Pointel.Interactions.IPlugins.OperationType.Queue)
                                {
                                    callToolTip = "Move to Queue";
                                    statusImageSource = null;
                                    callImageSource = new BitmapImage(new Uri("/Pointel.Interactions.TeamCommunicator;component/Images/move.to.queue.png", UriKind.Relative));
                                }
                                else if (_dataContext.SelectedOperationType == Pointel.Interactions.IPlugins.OperationType.Workbin)
                                {
                                    callToolTip = "Select a Workbin";
                                    callImageSource = new BitmapImage(new Uri("/Pointel.Interactions.TeamCommunicator;component/Images/move.to.workbin.png", UriKind.Relative));
                                }
                                else if (_dataContext.SelectedOperationType == Pointel.Interactions.IPlugins.OperationType.Transfer)
                                {
                                    callToolTip = "Start a Consult Interaction";
                                    callImageSource = new BitmapImage(new Uri("/Pointel.Interactions.TeamCommunicator;component/Images/Email.png", UriKind.Relative));
                                }
                                break;

                            default:
                                callToolTip = "";
                                callImageSource = null;
                                break;
                        }

                        break;
                    case Pointel.Interactions.IPlugins.AgentMediaStatus.NotReady:
                        status = Pointel.Interactions.IPlugins.AgentMediaStatus.NotReady;
                        statusImageSource = new BitmapImage(new Uri("/Pointel.Interactions.TeamCommunicator;component/Images/Not_Ready.png", UriKind.Relative));
                        switch (_dataContext.CurrentInteractionType)
                        {
                            case Pointel.Interactions.IPlugins.InteractionType.Email:
                                callToolTip = "Start a Consult Interaction";
                                callImageSource = new BitmapImage(new Uri("/Pointel.Interactions.TeamCommunicator;component/Images/Email.png", UriKind.Relative));
                                break;

                            case Pointel.Interactions.IPlugins.InteractionType.Voice:
                                callToolTip = "Establish a new Phone Call";
                                callImageSource = new BitmapImage(new Uri("/Pointel.Interactions.TeamCommunicator;component/Images/Voice.Short.png", UriKind.Relative));
                                break;

                            case Pointel.Interactions.IPlugins.InteractionType.Chat:
                                callToolTip = "Start a Consult Interaction";
                                callImageSource = new BitmapImage(new Uri("/Pointel.Interactions.TeamCommunicator;component/Images/Chat.png", UriKind.Relative));
                                break;

                            case Pointel.Interactions.IPlugins.InteractionType.WorkBin:
                                if (_dataContext.SelectedOperationType == Pointel.Interactions.IPlugins.OperationType.Queue)
                                {
                                    callToolTip = "Move to Queue";
                                    statusImageSource = null;
                                    callImageSource = new BitmapImage(new Uri("/Pointel.Interactions.TeamCommunicator;component/Images/move.to.queue.png", UriKind.Relative));
                                }
                                else if (_dataContext.SelectedOperationType == Pointel.Interactions.IPlugins.OperationType.Workbin)
                                {
                                    callToolTip = "Select a Workbin";
                                    callImageSource = new BitmapImage(new Uri("/Pointel.Interactions.TeamCommunicator;component/Images/move.to.workbin.png", UriKind.Relative));
                                }
                                else if (_dataContext.SelectedOperationType == Pointel.Interactions.IPlugins.OperationType.Transfer)
                                {
                                    callToolTip = "Start a Consult Interaction";
                                    callImageSource = new BitmapImage(new Uri("/Pointel.Interactions.TeamCommunicator;component/Images/Email.png", UriKind.Relative));
                                }
                                break;

                            default:
                                callToolTip = "";
                                callImageSource = null;
                                break;
                        }
                        break;
                    default:
                        callToolTip = "Add to Favorites";
                        callImageSource = new BitmapImage(new Uri("/Pointel.Interactions.TeamCommunicator;component/Images/Favourite.png", UriKind.Relative));
                        statusImageSource = new BitmapImage(new Uri("/Pointel.Interactions.TeamCommunicator;component/Images/Disable.btn.png", UriKind.Relative));
                        break;
                }
            }
            catch (Exception generalException)
            {
                _logger.Error("DialPad Class : GetStatusData() : " + generalException.Message.ToString());
            }
            return callToolTip;
        }

        /// <summary>
        /// Contacts the updation.
        /// </summary>
        /// <param name="operationType">Type of the operation.</param>
        /// <param name="contactId">The contact unique identifier.</param>
        /// <param name="attributeList">The attribute list.</param>
        public void ContactUpdation(string operationType, string contactId, Genesyslab.Platform.Contacts.Protocols.ContactServer.ContactAttributeList attributeList)
        {
            ConfigParser parser = new ConfigParser();
            parser.ContactUpdation(operationType, contactId, attributeList);
        }

        /// <summary>
        /// Initializes the items.
        /// </summary>
        private void InitializeMenuItems()
        {
            try
            {

                _dataContext.MnuCall.StaysOpenOnClick = true;
                _dataContext.MnuCall.Background = System.Windows.Media.Brushes.Transparent;
                _dataContext.MnuCall.Header = "Call";
                _dataContext.MnuCall.Icon = new System.Windows.Controls.Image
                {
                    Height = 15,
                    Width = 15,
                    Source = new BitmapImage(new Uri("/Agent.Interaction.Desktop;component/Images/Voice.Short.png", UriKind.Relative))
                };


                _dataContext.MnuAddFavorite.StaysOpenOnClick = true;
                _dataContext.MnuAddFavorite.Background = System.Windows.Media.Brushes.Transparent;
                _dataContext.MnuAddFavorite.Header = "Add to favorite";
                _dataContext.MnuAddFavorite.Icon = new System.Windows.Controls.Image
                {
                    Height = 15,
                    Width = 15,
                    Source = new BitmapImage(new Uri("/Agent.Interaction.Desktop;component/Images/Favourite.png", UriKind.Relative))
                };


                _dataContext.MnuRemoveFavorite.StaysOpenOnClick = true;
                _dataContext.MnuRemoveFavorite.Background = System.Windows.Media.Brushes.Transparent;
                _dataContext.MnuRemoveFavorite.Header = "Remove from Favorites";
                _dataContext.MnuRemoveFavorite.Icon = new System.Windows.Controls.Image
                {
                    Height = 15,
                    Width = 15,
                    Source = new BitmapImage(new Uri("/Agent.Interaction.Desktop;component/Images/Favourite.Remove.png", UriKind.Relative))
                };


                _dataContext.MnuEditFavorite.StaysOpenOnClick = true;
                _dataContext.MnuEditFavorite.Background = System.Windows.Media.Brushes.Transparent;
                _dataContext.MnuEditFavorite.Header = "Edit favorite";
                _dataContext.MnuEditFavorite.Icon = new System.Windows.Controls.Image
                {
                    Height = 15,
                    Width = 15,
                    Source = new BitmapImage(new Uri("/Agent.Interaction.Desktop;component/Images/Favourite.Edit.png", UriKind.Relative))
                };

            }
            catch (Exception ex)
            {
                _logger.Error((ex.InnerException == null) ? ex.Message : ex.InnerException.ToString());
            }
        }

        private int GetMaxInteractionCount(string employeeID)
        {
            int maxInteractionCount = 1;
            try
            {
                CfgPersonQuery cfgPersonQuery = new CfgPersonQuery();
                cfgPersonQuery.EmployeeId = employeeID;
                cfgPersonQuery.TenantDbid = ConfigContainer.Instance().TenantDbId;
                CfgPerson cfgPerson = ((ConfService)ConfigContainer.Instance().ConfServiceObject).RetrieveObject<CfgPerson>(cfgPersonQuery);
                if (cfgPerson != null && cfgPerson.AgentInfo.CapacityRule != null)
                {
                    CfgScript cfgScript = cfgPerson.AgentInfo.CapacityRule;
                    if (cfgScript.UserProperties != null && cfgScript.UserProperties.Count > 0 && cfgScript.UserProperties.ContainsKey("_CRWizardMediaCapacityList_"))
                    {
                        Genesyslab.Platform.Commons.Collections.KeyValueCollection list = cfgScript.UserProperties["_CRWizardMediaCapacityList_"] as Genesyslab.Platform.Commons.Collections.KeyValueCollection;
                        foreach (string data in list.AllKeys)
                        {
                            if (Datacontext.GetInstance().CurrentInteractionType == Pointel.Interactions.IPlugins.InteractionType.Email && data.ToLower() == "email")
                                int.TryParse(list[data].ToString(), out maxInteractionCount);
                            if (Datacontext.GetInstance().CurrentInteractionType == Pointel.Interactions.IPlugins.InteractionType.Chat && data.ToLower() == "chat")
                                int.TryParse(list[data].ToString(), out maxInteractionCount);
                        }
                    }
                }
                maxInteractionCount = maxInteractionCount == 0 ? 1 : maxInteractionCount;
            }
            catch (Exception ex)
            {
                maxInteractionCount = 1;
                _logger.Error((ex.InnerException == null) ? ex.Message : ex.InnerException.ToString());
            }
            return maxInteractionCount;
        }


        #endregion

    }
}
