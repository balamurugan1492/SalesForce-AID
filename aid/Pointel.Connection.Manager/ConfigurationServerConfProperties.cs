namespace Pointel.Connection.Manager
{
    using System;

    using Genesyslab.Platform.ApplicationBlocks.Commons.Protocols;
    using Genesyslab.Platform.Commons.Connection.Configuration;
    using Genesyslab.Platform.Configuration.Protocols.Types;

    public class ConfigurationServerConfProperties
    {
        #region Fields

        private ConfServerConfiguration _protocolconfiguration;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets or sets the Tserver protocol configuration.
        /// </summary>
        /// <value>
        /// The protocol configuration.
        /// </value>
        public ConfServerConfiguration Protocolconfiguration
        {
            get { return _protocolconfiguration; }
            set { if (_protocolconfiguration != value) _protocolconfiguration = value; }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Creates the specified server types.
        /// </summary>
        public void Create()
        {
            Protocolconfiguration = null;
            Protocolconfiguration = new ConfServerConfiguration(ServerType.Configserver.ToString());
        }

        /// <summary>
        /// Creates the specified server protocol configuration.
        /// Use this method for FaultTolerance : None & HotStandby
        /// </summary>        
        /// <param name="uri">Server uri.</param>
        /// <param name="appType">Type of the application.</param>
        /// <param name="userName">Name of the application user.</param>
        /// <param name="password">user's password.</param>
        /// <param name="faultTolerance">The fault tolerance mode.</param>
        public void Create(Uri serverUri, CfgAppType appType, string clientName, string userName, string password, FaultToleranceMode faultTolerance)
        {
            Protocolconfiguration = null;
            Protocolconfiguration = new ConfServerConfiguration(ServerType.Configserver.ToString());
            Protocolconfiguration.Uri = serverUri;
            Protocolconfiguration.ClientApplicationType = appType;
            Protocolconfiguration.ClientName = clientName;
            Protocolconfiguration.UserName = userName;
            Protocolconfiguration.UserPassword = password;
            Protocolconfiguration.FaultTolerance = faultTolerance;
        }

        /// <summary>
        /// Creates the specified server protocol configuration.
        /// Use this method for FaultTolerance : None & HotStandby
        /// </summary>        
        /// <param name="serverUri">The server URI.</param>
        /// <param name="appType">Type of the application.</param>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">user's password.</param>
        /// <param name="faultTolerance">The fault tolerance.</param>
        /// <param name="addpServerTimeOut">Addp server time out.</param>
        /// <param name="addpClientTimeOut">Addp client time out.</param>
        /// <param name="addpTrace">Addp trace mode.</param>
        public void Create(Uri serverUri, CfgAppType appType, string clientName, string userName, string password, FaultToleranceMode faultTolerance, Int32 addpServerTimeOut, Int32 addpClientTimeOut,
            AddpTraceMode addpTrace)
        {
            Protocolconfiguration = null;
            Protocolconfiguration = new ConfServerConfiguration(ServerType.Configserver.ToString());
            Protocolconfiguration.Uri = serverUri;
            Protocolconfiguration.ClientApplicationType = appType;
            Protocolconfiguration.ClientName = clientName;
            Protocolconfiguration.UserName = userName;
            Protocolconfiguration.UserPassword = password;
            Protocolconfiguration.FaultTolerance = faultTolerance;
            Protocolconfiguration.UseAddp = true;
            Protocolconfiguration.AddpServerTimeout = addpServerTimeOut;
            Protocolconfiguration.AddpClientTimeout = addpClientTimeOut;
            Protocolconfiguration.AddpTrace = addpTrace.ToString();
        }

        /// <summary>
        /// Creates the specified server protocol configuration.
        /// Use this method for FaultTolerance : WarmStandby
        /// Warm standby switch over is set to unlimited time.
        /// </summary>        
        /// <param name="uri">Server uri.</param>
        /// <param name="appType">Type of the application.</param>
        /// <param name="userName">Name of the application user.</param>
        /// <param name="password">user's password.</param>
        /// <param name="warmStandbyUri">Warm standby URI.</param>
        /// <param name="warmStandByTimeOut">Warm stand by time out.</param>
        /// <param name="warmStandByAttempts">Warm stand by attempts.</param>
        public void Create(Uri serverUri, CfgAppType appType, string clientName, string userName, string password, Uri warmStandbyUri, Int32 warmStandByTimeOut, Int16 warmStandByAttempts)
        {
            Protocolconfiguration = null;
            Protocolconfiguration = new ConfServerConfiguration(ServerType.Configserver.ToString());
            Protocolconfiguration.Uri = serverUri;
            Protocolconfiguration.ClientApplicationType = appType;
            Protocolconfiguration.ClientName = clientName;
            Protocolconfiguration.UserName = userName;
            Protocolconfiguration.UserPassword = password;
            Protocolconfiguration.FaultTolerance = FaultToleranceMode.WarmStandby;
            Protocolconfiguration.WarmStandbyUri = warmStandbyUri;
            Protocolconfiguration.WarmStandbyTimeout = warmStandByTimeOut;
            Protocolconfiguration.WarmStandbyAttempts = warmStandByAttempts;
        }

        /// <summary>
        /// Creates the specified server protocol configuration.
        /// </summary>
        /// <param name="serverUri">The server URI.</param>
        /// <param name="appType">Type of the application.</param>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">user's password.</param>
        /// <param name="warmStandbyUri">The warm standby URI.</param>
        /// <param name="warmStandByTimeOut">The warm stand by time out.</param>
        /// <param name="warmStandByAttempts">The warm stand by attempts.</param>
        /// <param name="addpServerTimeOut">Addp server time out.</param>
        /// <param name="addpClientTimeOut">Addp client time out.</param>
        /// <param name="addpTrace">Addp trace mode.</param>
        public void Create(Uri serverUri, CfgAppType appType, string clientName, string userName, string password, Uri warmStandbyUri, Int32 warmStandByTimeOut, Int16 warmStandByAttempts,
            Int32 addpServerTimeOut, Int32 addpClientTimeOut, AddpTraceMode addpTrace)
        {
            Protocolconfiguration = null;
            Protocolconfiguration = new ConfServerConfiguration(ServerType.Configserver.ToString());
            Protocolconfiguration.Uri = serverUri;
            Protocolconfiguration.ClientApplicationType = appType;
            Protocolconfiguration.ClientName = clientName;
            Protocolconfiguration.UserName = userName;
            Protocolconfiguration.UserPassword = password;
            Protocolconfiguration.FaultTolerance = FaultToleranceMode.WarmStandby;
            Protocolconfiguration.WarmStandbyUri = warmStandbyUri;
            Protocolconfiguration.WarmStandbyTimeout = warmStandByTimeOut;
            Protocolconfiguration.WarmStandbyAttempts = warmStandByAttempts;
            Protocolconfiguration.UseAddp = true;
            Protocolconfiguration.AddpServerTimeout = addpServerTimeOut;
            Protocolconfiguration.AddpClientTimeout = addpClientTimeOut;
            Protocolconfiguration.AddpTrace = addpTrace.ToString();
        }

        #endregion Methods
    }
}