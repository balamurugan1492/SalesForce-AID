namespace Pointel.Statistics.Core.ConnectionManager
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data.OracleClient;
    using System.Data.SqlClient;
    using System.Data.SQLite;
    using System.Drawing;
    using System.Linq;
    using System.Text;

    using Genesyslab.Platform.ApplicationBlocks.Commons.Broker;
    using Genesyslab.Platform.ApplicationBlocks.Commons.Protocols;
    using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel;
    using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.CfgObjects;
    using Genesyslab.Platform.Commons.Collections;
    using Genesyslab.Platform.Commons.Protocols;
    using Genesyslab.Platform.Configuration.Protocols;
    using Genesyslab.Platform.Reporting.Protocols;

    using Pointel.Statistics.Core.StatisticsProvider;
    using Pointel.Statistics.Core.Utility;

    /// <summary>
    /// 
    /// </summary>
    internal class StatisticsSetting
    {
        #region Fields

        public const string ConfServer = "config";

        //StatServerValues
        public const string StatServer = "statistics";

        public List<string> ACDQueuesListCollections = new List<string>();
        public Dictionary<int, StatVariables> ACDStatisticsPluginHolder = new Dictionary<int, StatVariables>();
        public Dictionary<int, StatVariables> ACDStatisticsValueHolder = new Dictionary<int, StatVariables>();
        public string agentDBID = string.Empty;
        public string AgentEmpId;

        //AgentGroupCollection
        public List<CfgAgentGroup> AgentGroupCollection = new List<CfgAgentGroup>();

        //Object Collections
        public List<string> AgentGroupsListCollections = new List<string>();
        public Dictionary<int, StatVariables> agentGroupStatsPluginHolder = new Dictionary<int, StatVariables>();
        public Dictionary<int, StatVariables> agentGroupStatsValueHolder = new Dictionary<int, StatVariables>();
        public Dictionary<int, StatVariables> agentStatisticsPluginHolder = new Dictionary<int, StatVariables>();

        //Statistics Value Holders
        public Dictionary<int, StatVariables> agentStatisticsValueHolder = new Dictionary<int, StatVariables>();

        //Admin Configurations
        public CfgApplication Application = null;
        public string AppName = string.Empty;
        public int BAttributeReferenceId = 10001;
        public int CFGTenantDBID = WellKnownDbids.EnterpriseModeTenantDbid;
        public ConfServerConfiguration configurationProperties;

        //Application
        public ConfService confObject = null;
        public ConfServerProtocol confProtocol;

        //DB Requirements
        public string DBDataSource = string.Empty;
        public string DBHost = string.Empty;
        public string DBLoginQuery = string.Empty;
        public string DBName = string.Empty;
        public string DBPassword = string.Empty;
        public string DBPort = string.Empty;
        public string DBSID = string.Empty;
        public string DBSName = string.Empty;
        public string DBType = string.Empty;
        public string DBUserName = string.Empty;

        //ACD Displays
        public Dictionary<string, string> DictACDDisplays = new Dictionary<string, string>();

        //AgentGroup Objects
        public Dictionary<string, Dictionary<string, List<string>>> DictAgentGroupObjects = new Dictionary<string, Dictionary<string, List<string>>>();

        //AgentGroup Statistics
        public Dictionary<string, List<string>> DictAgentGroupStatistics = new Dictionary<string, List<string>>();
        public Dictionary<string, string> DictAgentNames = new Dictionary<string, string>();

        //Agent Objects
        public Dictionary<string, Dictionary<string, List<string>>> DictAgentObjects = new Dictionary<string, Dictionary<string, List<string>>>();

        //Agent Statistics
        public Dictionary<string, List<string>> DictAgentStatistics = new Dictionary<string, List<string>>();

        //Statistics Display name & Section name
        public Dictionary<string, string> DictAllStats = new Dictionary<string, string>();
        public Dictionary<string, DBValues> DictDBStatHolder = new Dictionary<string, DBValues>();

        //DB Statistics value holder
        public Dictionary<string, string> DictDBStatValuesHolder = new Dictionary<string, string>();
        public Dictionary<string, List<CfgPerson>> DictDisplayObjects;

        //Enable-Disable-Channels
        public Dictionary<string, bool> DictEnableDisableChannels = new Dictionary<string, bool>();

        //System Values
        public Dictionary<string, string> DictErrorValues = new Dictionary<string, string>();

        //Exisiting Objects
        public Dictionary<string, List<string>> DictExistingValues = new Dictionary<string, List<string>>();

        //DB Non Value holder
        public Dictionary<string, string> DictMissingValues = new Dictionary<string, string>();

        //Statistics Metrics
        public Dictionary<string, Dictionary<string, string>> DictStatisticsMetrics = new Dictionary<string, Dictionary<string, string>>();

        //Tagged Statistics
        public Dictionary<string, string> DictTaggedStats = new Dictionary<string, string>();
        public List<string> DNGroupListCollections = new List<string>();
        public Dictionary<int, StatVariables> dnGroupStatisticsPluginHolder = new Dictionary<int, StatVariables>();
        public Dictionary<int, StatVariables> dnGroupStatisticsValueHolder = new Dictionary<int, StatVariables>();

        //Statistics Color
        public string ExistingStatColor = string.Empty;
        public List<int> FinalACDPackageID = new List<int>();
        public List<int> FinalAdminPackageID = new List<int>();
        public List<int> FinalAgentGroupPackageID = new List<int>();

        //Package ID's
        public List<int> FinalAgentPackageID = new List<int>();
        public List<int> FinalDNGroupsPackageID = new List<int>();
        public List<int> FinalVQPackageID = new List<int>();
        public Hashtable HashLogDetails = new Hashtable();

        //Individual Statistics
        public string IndividualAgent = string.Empty;
        public bool isACDThresholdBreach = false;
        public bool isAdmin = false;
        public bool isAgentGroupThresholdBreach = false;
        public bool IsApplicationFound = false;
        public bool isDBConnectionOpened = false;
        public bool isDnGroupThresholdBreach = false;

        //Application Values Exist
        public bool IsExist;
        public bool isLevelTwo = false;
        public bool isLogEnabled = false;

        //IsThresholdBreach
        public bool isThresholdBreach = false;
        public bool isVQThresholdBreach = false;

        //Request Id Collection
        public List<int> ListRequestIds = new List<int>();

        //logger Values
        public string logconversionPattern = string.Empty;
        public string logdatePattern = string.Empty;
        public string logFileName = string.Empty;
        public string logFilterLevel = string.Empty;
        public string logLevel = string.Empty;
        public string logmaxFileSize = string.Empty;
        public string logMaxLevel = string.Empty;
        public string logmaxSizeRoll = string.Empty;
        public string logMinLevel = string.Empty;
        public string logUserName = string.Empty;
        public List<string> LstACDStatistics;
        public List<string> LstAgentGroups = new List<string>();
        public List<string> LstAgentGroupStatistics;

        //Agent and Group Lists
        public List<string> LstAgents = new List<string>();

        //Server Collections
        public List<string> LstAgentStatistics;

        //Admin MainWindow objects to be shown
        public List<string> LstAgentUNames = new List<string>();
        public List<string> LstDNGroupStatistics;
        public List<CfgPerson> LstPersons = new List<CfgPerson>();
        public List<string> LstTaggedStats = new List<string>();
        public List<string> LstVQueueStatistics;

        //Max Tags
        public int MaxTags = 0;
        public OracleCommand oracleCmd = null;
        public OracleConnection oracleConn = new OracleConnection();
        public OracleDataAdapter oracleDA = null;

        //Agent Details
        public CfgPerson PersonDetails;
        public CfgApplication primaryStatServer = null;
        public ProtocolManagementService protocolManager;

        //Reference_ID
        public int ReferenceId = 1;
        public int ReferenceIdLimit = 0;

        //Reference ID Collection
        public Dictionary<string, string> ReferenceIds = new Dictionary<string, string>();
        public EventBrokerService reportingBroker;

        //RequestId's
        public List<int> RequestIds = new List<int>();
        public string rollingStyle = string.Empty;
        public ProtocolManagementService rptProtocolManager = new ProtocolManagementService();
        public CfgApplication secondaryStatServer = null;
        public int SecondLimit = 0;

        //StatServer Filters
        public Dictionary<string, string> ServerFilters = null;
        public string ServerStatColor = string.Empty;
        public Dictionary<string, KeyValueCollection> ServerStatistics = new Dictionary<string, KeyValueCollection>();

        //Stat server statistics
        public Hashtable ServerValues = new Hashtable();
        //public SqlDataAdapter sqlAdapter;
        //public SqlCommand sqlCommand;
        //public SqlConnection sqlConnection = new SqlConnection();
        //public SQLiteCommand sqliteCmd = null;
        //public SQLiteConnection sqliteCon = new SQLiteConnection();
        //public SQLiteDataAdapter sqliteDA = null;

        //ApplicationAnnex
        public Dictionary<string, Dictionary<string, string>> StatistcisDetails = null;
        public int StatisticDisplayTime = 0;

        //Statistics collection
        public IStatisticsCollection statisticsCollection;
        public StatServerConfiguration statisticsProperties = null;
        //public StatServerConfiguration statisticsProperties = new StatServerConfiguration("statistics");
        public StatServerProtocol statServerProtocol = null;

        //AccessGroup
        public string statTickerAdminGroup = string.Empty;
        public string statTickerUserGroup = string.Empty;
        public Dictionary<string, List<Color>> ThresholdColors = new Dictionary<string, List<Color>>();

        //Threshold Collections
        public Dictionary<string, List<string>> ThresholdValues = new Dictionary<string, List<string>>();
        public int TotalNumberOfStatCount = 0;
        public Dictionary<int, StatVariables> VQStatisticsPlugineHolder = new Dictionary<int, StatVariables>();
        public Dictionary<int, StatVariables> VQStatisticsValueHolder = new Dictionary<int, StatVariables>();
        public List<string> VQueueListCollections = new List<string>();

        static StatisticsSetting singletonInstance = null;
        public List<StatServerProtocol> statProtocols = null;
        public Dictionary<string,bool> serverNames = null;
        public Dictionary<int, string> TeamCommRefIDs = new Dictionary<int, string>();

        //Save Agent Stat configuration in XML file local system
        public bool isAgentConfigFromLocal = false;
        public Dictionary<string, string> agentConfigColl = new Dictionary<string, string>();
        #endregion Fields

        //NotReady reason code key configuration
        public string NotReadyReasonKeyName = string.Empty;

        #region Methods

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <returns></returns>
        public static StatisticsSetting GetInstance()
        {
            if (singletonInstance == null)
            {
                singletonInstance = new StatisticsSetting();
                return singletonInstance;
            }
            else
            {
                return singletonInstance;
            }
        }

        #endregion Methods
    }
}