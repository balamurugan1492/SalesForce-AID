#region System Namespace
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
#endregion

namespace Pointel.Statistics.Core.StatisticsProvider
{
    /// <summary>
    /// IStatisticsCollection
    /// </summary>
    internal interface IStatisticsCollection
    {
        List<IAgentStatistics> AgentStatistics
        {
            get;
            set;
        }

        List<IAgentGroupStatistics> AgentGroupStatistics
        {
            get;
            set;
        }

        List<IACDStatistics> ACDQueueStatistics
        {
            get;
            set;
        }

        List<IDNGroupStatistics> DNGroupStatistics
        {
            get;
            set;
        }

        List<IVirtualQueueStatistics> VirtualQueueStatistics
        {
            get;
            set;
        }

        IStatisticsCommon StatisticsCommon
        {
            get;
            set;
        }

        IStatisticsLocalSetting StatisticsLocalSetting
        {
            get;
            set;
        }

        IApplicationContainer ApplicationContainer
        {
            get;
            set;
        }
        List<IDBValules> DBValues
        {
            get;
            set;
        }

        IAdminValues AdminValues
        {
            get;
            set;
        }
    }
}
