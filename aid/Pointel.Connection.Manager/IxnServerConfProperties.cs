using Genesyslab.Platform.ApplicationBlocks.Commons.Protocols;
using Genesyslab.Platform.Commons.Connection.Configuration;
using Genesyslab.Platform.OpenMedia.Protocols.InteractionServer;
using System;

namespace Pointel.Connection.Manager
{
    public class IxnServerConfProperties
    {
        private InteractionServerConfiguration _protocolconfiguration;
        /// <summary>
        /// Gets or sets the Ixnserver protocol configuration.
        /// </summary>
        /// <value>
        /// The protocol configuration.
        /// </value>
        public InteractionServerConfiguration Protocolconfiguration
        {
            get { return _protocolconfiguration; }
            set { if (_protocolconfiguration != value) _protocolconfiguration = value; }
        }

        /// <summary>
        /// Creates the specified server types.
        /// </summary>
        public void Create()
        {
            Protocolconfiguration = null;
            Protocolconfiguration = new InteractionServerConfiguration(ServerType.Ixnserver.ToString());
        }

        /// <summary>
        /// Creates the specified server URI.
        /// </summary>
        /// <param name="serverUri">The server URI.</param>
        /// <param name="userName">Name of the user.</param>
        /// <param name="faultTolerance">The fault tolerance.</param>
        /// <param name="interactionClientType">Type of the interaction client.</param>
        public void Create(Uri serverUri, string userName, FaultToleranceMode faultTolerance, InteractionClient interactionClientType)
        {
            Protocolconfiguration = null;
            Protocolconfiguration = new InteractionServerConfiguration(ServerType.Ixnserver.ToString());
            Protocolconfiguration.Uri = serverUri;
            Protocolconfiguration.ClientName = "AID_" + userName;
            Protocolconfiguration.ClientType = interactionClientType;
            Protocolconfiguration.FaultTolerance = faultTolerance;
        }

        /// <summary>
        /// Creates the specified server URI.
        /// </summary>
        /// <param name="serverUri">The server URI.</param>
        /// <param name="userName">Name of the user.</param>
        /// <param name="faultTolerance">The fault tolerance.</param>
        /// <param name="interactionClientType">Type of the interaction client.</param>
        /// <param name="addpServerTimeOut">The addp server time out.</param>
        /// <param name="addpClientTimeOut">The addp client time out.</param>
        /// <param name="addpTrace">The addp trace.</param>
        public void Create(Uri serverUri, string userName, FaultToleranceMode faultTolerance, InteractionClient interactionClientType , Int32 addpServerTimeOut, Int32 addpClientTimeOut,
            AddpTraceMode addpTrace)
        {
            Protocolconfiguration = null;
            Protocolconfiguration = new InteractionServerConfiguration(ServerType.Ixnserver.ToString());
            Protocolconfiguration.Uri = serverUri;
            Protocolconfiguration.ClientName = "AID_" + userName;
            Protocolconfiguration.FaultTolerance = faultTolerance;
            Protocolconfiguration.ClientType = interactionClientType;
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
        /// <param name="serverUri">The server URI.</param>
        /// <param name="userName">Name of the application user.</param>
        /// <param name="interactionClientType">Type of the interaction client.</param>
        /// <param name="warmStandbyUri">Warm standby URI.</param>
        /// <param name="warmStandByTimeOut">Warm stand by time out.</param>
        /// <param name="warmStandByAttempts">Warm stand by attempts.</param>
        public void Create(Uri serverUri, string userName, InteractionClient interactionClientType , Uri warmStandbyUri, Int32 warmStandByTimeOut, Int16 warmStandByAttempts)
        {
            Protocolconfiguration = null;
            Protocolconfiguration = new InteractionServerConfiguration(ServerType.Ixnserver.ToString());
            Protocolconfiguration.Uri = serverUri;
            Protocolconfiguration.ClientName = "AID_" + userName;
            Protocolconfiguration.FaultTolerance = FaultToleranceMode.WarmStandby;
            Protocolconfiguration.ClientType = interactionClientType;
            Protocolconfiguration.WarmStandbyUri = warmStandbyUri;
            Protocolconfiguration.WarmStandbyTimeout = warmStandByTimeOut;
            Protocolconfiguration.WarmStandbyAttempts = warmStandByAttempts;
        }

        /// <summary>
        /// Creates the specified server protocol configuration.
        /// </summary>
        /// <param name="serverUri">The server URI.</param>
        /// <param name="userName">Name of the user.</param>
        /// <param name="interactionClientType">Type of the interaction client.</param>
        /// <param name="warmStandbyUri">The warm standby URI.</param>
        /// <param name="warmStandByTimeOut">The warm stand by time out.</param>
        /// <param name="warmStandByAttempts">The warm stand by attempts.</param>
        /// <param name="addpServerTimeOut">Addp server time out.</param>
        /// <param name="addpClientTimeOut">Addp client time out.</param>
        /// <param name="addpTrace">Addp trace mode.</param>
        public void Create(Uri serverUri, string userName, InteractionClient interactionClientType , Uri warmStandbyUri, Int32 warmStandByTimeOut, Int16 warmStandByAttempts,
            Int32 addpServerTimeOut, Int32 addpClientTimeOut, AddpTraceMode addpTrace)
        {
            Protocolconfiguration = null;
            Protocolconfiguration = new InteractionServerConfiguration(ServerType.Ixnserver.ToString());
            Protocolconfiguration.Uri = serverUri;
            Protocolconfiguration.ClientName = "AID_" + userName;
            Protocolconfiguration.FaultTolerance = FaultToleranceMode.WarmStandby;
            Protocolconfiguration.ClientType = interactionClientType;
            Protocolconfiguration.WarmStandbyUri = warmStandbyUri;
            Protocolconfiguration.WarmStandbyTimeout = warmStandByTimeOut;
            Protocolconfiguration.WarmStandbyAttempts = warmStandByAttempts;
            Protocolconfiguration.UseAddp = true;
            Protocolconfiguration.AddpServerTimeout = addpServerTimeOut;
            Protocolconfiguration.AddpClientTimeout = addpClientTimeOut;
            Protocolconfiguration.AddpTrace = addpTrace.ToString();
        }
    }
}
