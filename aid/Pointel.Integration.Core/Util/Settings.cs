namespace Pointel.Integration.Core.Util
{
    using System.Collections.Generic;
    using System.Data.OracleClient;
    using System.Data.SqlClient;

    using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel;

    internal class Settings
    {
        #region Fields

        public static string agentId = string.Empty;
        public static string callDataEventPortType = string.Empty;
        public static ConfService configProtocol = null;
        public static string dn = string.Empty;
        public static List<string> loadUserDataJSonObject = new List<string>();

        public Dictionary<string, string> attachDataList = new Dictionary<string, string>();
        public SqlConnection cn = null;
        public OracleConnection connection = null;
        public List<string> VoiceFilterKey = new List<string>();

        private static Settings _settings = null;

        private string employeeID = string.Empty;
        private bool enableCrmDbCommunication = false;
        private bool enableDualCommunication = false;
        private bool enableFacetCommunication = false;
        private bool enableFileCommunication = false;
        private bool enablepipeCommunication = false;
        private bool enablePortCommunication = false;
        private bool enableURLCommunication = false;
        private string firstName = string.Empty;
        private string lastName = string.Empty;
        private string queueName;
        private string userName = string.Empty;

        #endregion Fields

        #region Constructors

        public Settings()
        {
            PortSetting = PortSettings.GetInstance();
            EnablePortCommunication = true;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets the agent unique identifier.
        /// </summary>
        /// <value>
        /// The agent unique identifier.
        /// </value>
        public static string AgentId
        {
            get { return agentId; }
            set { agentId = value; }
        }

        /// <summary>
        /// Gets or sets the agent unique identifier.
        /// </summary>
        /// <value>
        /// The agent unique identifier.
        /// </value>
        public static string DNNumber
        {
            get { return dn; }
            set { dn = value; }
        }

        public string AgentLoginID
        {
            get;
            set;
        }

        public string EmployeeID
        {
            get { return employeeID; }
            set { employeeID = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [enable CRM database communication].
        /// </summary>
        /// <value>
        /// <c>true</c> if [enable CRM database communication]; otherwise, <c>false</c>.
        /// </value>
        public bool EnableCrmDbCommunication
        {
            get { return enableCrmDbCommunication; }
            set { enableCrmDbCommunication = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [enable dual communication].
        /// </summary>
        /// <value>
        /// <c>true</c> if [enable dual communication]; otherwise, <c>false</c>.
        /// </value>
        public bool EnableDualCommunication
        {
            get { return enableDualCommunication; }
            set { enableDualCommunication = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [enable facet communication].
        /// </summary>
        /// <value>
        /// <c>true</c> if [enable facet communication]; otherwise, <c>false</c>.
        /// </value>
        public bool EnableFacetCommunication
        {
            get { return enableFacetCommunication; }
            set { enableFacetCommunication = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [enable file communication].
        /// </summary>
        /// <value>
        /// <c>true</c> if [enable file communication]; otherwise, <c>false</c>.
        /// </value>
        public bool EnableFileCommunication
        {
            get { return enableFileCommunication; }
            set { enableFileCommunication = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [enable pipe communication].
        /// </summary>
        /// <value>
        /// <c>true</c> if [enable pipe communication]; otherwise, <c>false</c>.
        /// </value>
        public bool EnablePipeCommunication
        {
            get { return enablepipeCommunication; }
            set { enablepipeCommunication = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [enable port communication].
        /// </summary>
        /// <value>
        /// <c>true</c> if [enable port communication]; otherwise, <c>false</c>.
        /// </value>
        public bool EnablePortCommunication
        {
            get { return enablePortCommunication; }
            set { enablePortCommunication = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [enable URL communication].
        /// </summary>
        /// <value>
        /// <c>true</c> if [enable URL communication]; otherwise, <c>false</c>.
        /// </value>
        public bool EnableURLCommunication
        {
            get { return enableURLCommunication; }
            set { enableURLCommunication = value; }
        }

        /// <summary>
        /// Gets or sets the agent unique identifier.
        /// </summary>
        /// <value>
        /// The FirstName unique identifier.
        /// </value>
        public string FirstName
        {
            get { return firstName; }
            set { firstName = value; }
        }

        public bool IsFacetEnabled
        {
            get;
            set;
        }

        public bool IsLawsonEnabled
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the agent unique identifier.
        /// </summary>
        /// <value>
        /// The LastName unique identifier.
        /// </value>
        public string LastName
        {
            get { return lastName; }
            set { lastName = value; }
        }

        public string Place
        {
            get { return dn; }
            set { dn = value; }
        }

        public PortSettings PortSetting
        {
            get;
            set;
        }

        public string QueueName
        {
            get { return queueName; }
            set { queueName = value; }
        }

        /// <summary>
        /// Gets or sets the agent unique identifier.
        /// </summary>
        /// <value>
        /// The UserName unique identifier.
        /// </value>
        public string UserName
        {
            get { return userName; }
            set { userName = value; }
        }

        public string VoiceDispositionCollectionName
        {
            get;
            set;
        }

        public string VoiceDispositionKeyName
        {
            get;
            set;
        }

        #endregion Properties

        #region Methods

        internal static Settings GetInstance()
        {
            if (_settings == null)
            {
                _settings = new Settings();
                return _settings;
            }
            return _settings;
        }

        #endregion Methods

        #region Other

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <returns></returns>

        #endregion Other
    }
}