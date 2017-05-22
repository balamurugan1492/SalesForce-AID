#region System Namespace
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#endregion

using Pointel.Statistics.Core.StatisticsProvider;

namespace Pointel.Statistics.Core.Utility
{
    internal class StatisticsCollection : IStatisticsCollection
    {
        #region Field Declaration

        List<IAgentStatistics> _agentStatistics;
        List<IAgentGroupStatistics> _agentGroupStatistics;
        List<IACDStatistics> _acdStatistics;
        List<IDNGroupStatistics> _dnGroupStatistics;
        List<IVirtualQueueStatistics> _virtualQueueStatistics;
        IStatisticsCommon _statisticsCommon;
        IStatisticsLocalSetting _statisticsLocalSetting;
        IApplicationContainer _applicationContainer;
        List<IDBValules> _dbValues;
        IAdminValues _adminValues;

        #endregion

        #region Property

        public List<IAgentStatistics> AgentStatistics
        {
            get
            {
                return _agentStatistics;
            }
            set
            {
                _agentStatistics = value;
            }
        }

        public IStatisticsCommon StatisticsCommon
        {
            get
            {
                return _statisticsCommon;
            }
            set
            {
                _statisticsCommon = value;
            }
        }

        public List<IAgentGroupStatistics> AgentGroupStatistics
        {
            get
            {
                return _agentGroupStatistics;
            }
            set
            {
                _agentGroupStatistics = value;
            }
        }

        public List<IACDStatistics> ACDQueueStatistics
        {
            get
            {
                return _acdStatistics;
            }
            set
            {
                _acdStatistics = value;
            }
        }

        public List<IDNGroupStatistics> DNGroupStatistics
        {
            get
            {
                return _dnGroupStatistics;
            }
            set
            {
                _dnGroupStatistics = value;
            }
        }

        public List<IVirtualQueueStatistics> VirtualQueueStatistics
        {
            get
            {
                return _virtualQueueStatistics;
            }
            set
            {
                _virtualQueueStatistics = value;
            }
        }

        public IStatisticsLocalSetting StatisticsLocalSetting
        {
            get
            {
                return _statisticsLocalSetting;
            }
            set
            {
                _statisticsLocalSetting = value;
            }
        }

        public IApplicationContainer ApplicationContainer
        {
            get
            {
                return _applicationContainer;
            }
            set
            {
                _applicationContainer = value;
            }
        }

        public List<IDBValules> DBValues
        {
            get
            {
                return _dbValues;
            }
            set
            {
                _dbValues = value;
            }
        }

        public IAdminValues AdminValues
        {
            get
            {
                return _adminValues;
            }
            set
            {
                _adminValues = value;
            }
        }

        #endregion
    }
}
