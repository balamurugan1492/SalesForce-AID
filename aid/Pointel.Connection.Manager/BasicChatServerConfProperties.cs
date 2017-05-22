using Genesyslab.Platform.ApplicationBlocks.Commons.Protocols;
using Genesyslab.Platform.Commons.Connection.Configuration;
using Genesyslab.Platform.WebMedia.Protocols.BasicChat;
using System;

namespace Pointel.Connection.Manager
{
    public class BasicChatServerConfProperties
    {
        private BasicChatConfiguration _protocolconfiguration;
        /// <summary>
        /// Gets or sets the BasicChatserver protocol configuration.
        /// </summary>
        /// <value>
        /// The protocol configuration.
        /// </value>
        public BasicChatConfiguration Protocolconfiguration
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
            Protocolconfiguration = new BasicChatConfiguration(ServerType.Chatserver.ToString());
        }

        /// <summary>
        /// Creates the specified server types.
        /// </summary>        
        /// <param name="serverUri">The server URI.</param>
        /// <param name="userNickName">Nick name of the user.</param>
        /// <param name="userType">Type of the user.</param>
        /// <param name="personId">The person identifier.</param>
        /// <param name="faultTolerance">The fault tolerance.</param>
        public void Create(Uri serverUri, string userNickName, UserType userType, string personId, FaultToleranceMode faultTolerance)
        {
            Protocolconfiguration = null;
            Protocolconfiguration = new BasicChatConfiguration(ServerType.Chatserver.ToString());
            Protocolconfiguration.Uri = serverUri;
            Protocolconfiguration.UserNickname = userNickName;
            Protocolconfiguration.UserType = userType;
            Protocolconfiguration.PersonId = personId;
            Protocolconfiguration.FaultTolerance = faultTolerance;
        }

        /// <summary>
        /// Creates the specified server types.
        /// </summary>        
        /// <param name="serverUri">The server URI.</param>
        /// <param name="userNickName">Nick name of the user.</param>
        /// <param name="userType">Type of the user.</param>
        /// <param name="personId">The person identifier.</param>
        /// <param name="faultTolerance">The fault tolerance.</param>
        /// <param name="addpServerTimeOut">The addp server time out.</param>
        /// <param name="addpClientTimeOut">The addp client time out.</param>
        /// <param name="addpTrace">The addp trace.</param>
        public void Create(Uri serverUri, string userNickName, UserType userType, string personId, FaultToleranceMode faultTolerance, Int32 addpServerTimeOut, Int32 addpClientTimeOut,
            AddpTraceMode addpTrace)
        {
            Protocolconfiguration = null;
            Protocolconfiguration = new BasicChatConfiguration(ServerType.Chatserver.ToString());
            Protocolconfiguration.Uri = serverUri;
            Protocolconfiguration.UserNickname = userNickName;
            Protocolconfiguration.UserType = userType;
            Protocolconfiguration.PersonId = personId;
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
        /// <param name="serverUri">The server URI.</param>
        /// <param name="userName">Name of the application user.</param>
        /// <param name="warmStandbyUri">Warm standby URI.</param>
        /// <param name="userNickName">Nick name of the user.</param>
        /// <param name="userType">Type of the user.</param>
        /// <param name="personId">The person identifier.</param>
        /// <param name="warmStandByTimeOut">Warm stand by time out.</param>
        /// <param name="warmStandByAttempts">Warm stand by attempts.</param>
        public void Create(Uri serverUri, string userName, Uri warmStandbyUri, string userNickName, UserType userType, string personId, Int32 warmStandByTimeOut, Int16 warmStandByAttempts)
        {
            Protocolconfiguration = null;
            Protocolconfiguration = new BasicChatConfiguration(ServerType.Chatserver.ToString());
            Protocolconfiguration.Uri = serverUri;
            Protocolconfiguration.UserNickname = userNickName;
            Protocolconfiguration.UserType = userType;
            Protocolconfiguration.PersonId = personId;
            Protocolconfiguration.FaultTolerance = FaultToleranceMode.WarmStandby;
            Protocolconfiguration.WarmStandbyUri = warmStandbyUri;
            Protocolconfiguration.WarmStandbyTimeout = warmStandByTimeOut;
            Protocolconfiguration.WarmStandbyAttempts = warmStandByAttempts;
        }

        /// <summary>
        /// Creates the specified server protocol configuration.
        /// </summary>        
        /// <param name="serverUri">The server URI.</param>
        /// <param name="userNickName">Nick name of the user.</param>
        /// <param name="userType">Type of the user.</param>
        /// <param name="personId">The person identifier.</param>
        /// <param name="warmStandbyUri">The warm standby URI.</param>
        /// <param name="warmStandByTimeOut">The warm stand by time out.</param>
        /// <param name="warmStandByAttempts">The warm stand by attempts.</param>
        /// <param name="addpServerTimeOut">Addp server time out.</param>
        /// <param name="addpClientTimeOut">Addp client time out.</param>
        /// <param name="addpTrace">Addp trace mode.</param>
        public void Create(Uri serverUri, string userNickName, UserType userType, string personId, Uri warmStandbyUri, Int32 warmStandByTimeOut, Int16 warmStandByAttempts,
            Int32 addpServerTimeOut, Int32 addpClientTimeOut, AddpTraceMode addpTrace)
        {
            Protocolconfiguration = null;
            Protocolconfiguration = new BasicChatConfiguration(ServerType.Chatserver.ToString());
            Protocolconfiguration.Uri = serverUri;
            Protocolconfiguration.UserNickname = userNickName;
            Protocolconfiguration.UserType = userType;
            Protocolconfiguration.PersonId = personId;
            Protocolconfiguration.FaultTolerance = FaultToleranceMode.WarmStandby;
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
