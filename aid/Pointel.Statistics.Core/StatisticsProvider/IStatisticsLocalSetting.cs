#region System Namespace
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#endregion

#region Genesys Namespace
using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.CfgObjects;
using Genesyslab.Platform.ApplicationBlocks.Commons.Protocols;
using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel;
using Genesyslab.Platform.Configuration.Protocols;
#endregion

namespace Pointel.Statistics.Core.StatisticsProvider
{
    /// <summary>
    /// IStatisticsLocalSetting
    /// </summary>
    internal interface IStatisticsLocalSetting
    {
        List<CfgApplication> PrimaryStatServer
        {
            get;
            set;
        }
        
        List<Uri> PrimaryStatServerUri
        {
            get;
            set;
        }

        CfgApplication SecondaryStatServer
        {
            get;
            set;
        }

        ConfService ConfObject
        {
            get;
            set;
        }

        string ConfServer
        {
            get;
        }

        ProtocolManagementService ProtocolManager
        {
            get;
            set;
        }

        ConfServerConfiguration ConfigurationProperties
        {
            get;
            set;
        }

        ConfServerProtocol ConfProtocol
        {
            get;
            set;
        }

        string UserName
        {
            get;
            set;
        }

        string Password
        {
            get;
            set;
        }

        string AppName
        {
            get;
            set;
        }

        int AddpServerTimeOut
        {
            get;
            set;
        }

        int AddpClientTimeOut
        {
            get;
            set;
        }

        string HostName
        {
            get;
            set;
        }

        string Port
        {
            get;
            set;
        }
    }
}
