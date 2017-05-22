#region System Namespace
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#endregion

#region Pointel Namespace
using Pointel.Statistics.Core.StatisticsProvider;
#endregion

#region Genesys Namespace
using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.CfgObjects;
using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel;
using Genesyslab.Platform.ApplicationBlocks.Commons.Protocols;
using Genesyslab.Platform.Configuration.Protocols;
#endregion

namespace Pointel.Statistics.Core.Utility
{
    class StatisticsLocalSetting : IStatisticsLocalSetting
    {
        private List<CfgApplication> _primaryStatServer;
        public List<CfgApplication> PrimaryStatServer
        {
            get
            {
                return _primaryStatServer;
            }
            set
            {
                _primaryStatServer = value;
            }
        }
        private List<Uri> _primaryStatServerUri;
        public List<Uri> PrimaryStatServerUri
        {
            get
            {
                return _primaryStatServerUri;
            }
            set
            {
                _primaryStatServerUri = value;
            }
        }

        private CfgApplication _secondaryStatServer;
        public CfgApplication SecondaryStatServer
        {
            get
            {
                return _secondaryStatServer;
            }
            set
            {
                _secondaryStatServer = value;
            }
        }

        private string _userName;
        public string UserName
        {
            get
            {
                return _userName;
            }
            set
            {
                _userName = value;
            }
        }

        private string _password;
        public string Password
        {
            get
            {
                return _password;
            }
            set
            {
                _password = value;
            }
        }

        private string _appName;
        public string AppName
        {
            get
            {
                return _appName;
            }
            set
            {
                _appName = value;
            }
        }

        private int _addpServerTimeOut = 0;
        public int AddpServerTimeOut
        {
            get
            {
                return _addpServerTimeOut;
            }
            set
            {
                _addpServerTimeOut = value;
            }
        }

        private int _addpClientTimeOut = 0; 
        public int AddpClientTimeOut   
        {
            get
            {
                return _addpClientTimeOut;
            }
            set
            {
                _addpClientTimeOut = value;
            }
        }

        private string _hostName;
        public string HostName
        {
            get
            {
                return _hostName;
            }
            set
            {
                _hostName = value;
            }
        }

        private string _port;
        public string Port
        {
            get
            {
                return _port;
            }
            set
            {
                _port = value;
            }
        }

        private string _confServer = "config";
        public string ConfServer
        {
            get
            {
                return _confServer;
            }
        }

        private ConfService _confObject;
        public ConfService ConfObject
        {
            get
            {
                return _confObject;
            }
            set
            {
                _confObject = value;
            }
        }

        private ProtocolManagementService _protocolManager;
        public ProtocolManagementService ProtocolManager
        {
            get
            {
                return _protocolManager;
            }
            set
            {
                _protocolManager = value;
            }
        }

        private ConfServerConfiguration _configurationProperties;
        public ConfServerConfiguration ConfigurationProperties
        {
            get
            {
                return _configurationProperties;
            }
            set
            {
                _configurationProperties = value;
            }
        }

        private ConfServerProtocol _confProtocol;
        public ConfServerProtocol ConfProtocol
        {
            get
            {
                return _confProtocol;
            }
            set
            {
                _confProtocol = value;
            }
        }
    }
}
