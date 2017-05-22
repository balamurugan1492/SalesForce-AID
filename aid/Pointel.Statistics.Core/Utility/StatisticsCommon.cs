#region System Namespace
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#endregion

#region Pointel Namespace
using Pointel.Statistics.Core.StatisticsProvider;
using System.Drawing;
#endregion

namespace Pointel.Statistics.Core.Utility
{
    internal class StatisticsCommon : IStatisticsCommon
    {
        #region Property

        private string _source = string.Empty;
        public string Source
        {
            get { return _source; }
            set { _source = value; }
        }

        private int _displayTime = 0;
        public int DisplayTime
        {
            get { return _displayTime; }
            set { _displayTime = value; }
        }

        private int _insensitivity = 0;
        public int Insensitivity
        {
            get { return _insensitivity; }
            set { _insensitivity = value; }
        }

        private int _notifySeconds = 0;
        public int NotifySeconds
        {
            get { return _notifySeconds; }
            set { _notifySeconds = value; }
        }
        
        private string _primaryServerName = string.Empty;
        public string PrimaryServerName
        {
            get { return _primaryServerName; }
            set { _primaryServerName = value; }
        }

        private string _secondaryServerName = string.Empty;
        public string SecondaryServerName
        {
            get { return _secondaryServerName; }
            set { _secondaryServerName = value; }
        }

        private string _tenantName = string.Empty;
        public string TenantName
        {
            get
            {
                return _tenantName;
            }
            set
            {
                _tenantName = value;
            }
        }
        
        private string _agentStatistics = string.Empty;
        public string AgentStatistics
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

        private string _agentGroupStatistics = string.Empty;
        public string AgentGroupStatistics
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

        private string _acdStatistcis = string.Empty;
        public string ACDStatistics
        {
            get
            {
                return _acdStatistcis;
            }
            set
            {
                _acdStatistcis = value;
            }
        }

        private string _dnGroupStatistics = string.Empty;
        public string DNGroupStatistics
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

        private string _vqueueStatistics = string.Empty;
        public string VQueueStatistics
        {
            get
            {
                return _vqueueStatistics;
            }
            set
            {
                _vqueueStatistics = value;
            }
        }

        private string _agentObjects = string.Empty;
        public string AgentObjects
        {
            get
            {
                return _agentObjects;
            }
            set
            {
                _agentObjects = value;
            }
        }

        private string _agentGroupObjects = string.Empty;
        public string AgentGroupObjects
        {
            get
            {
                return _agentGroupObjects;
            }
            set
            {
                _agentGroupObjects = value;
            }
        }

        private string _acdObjects = string.Empty;
        public string ACDObjects
        {
            get
            {
                return _acdObjects;
            }
            set
            {
                _acdObjects = value;
            }
        }

        private string _dnGroupObjects = string.Empty;
        public string DNGroupObjects
        {
            get
            {
                return _dnGroupObjects;
            }
            set
            {
                _dnGroupObjects = value;
            }
        }

        private string _vQueueObjects = string.Empty;
        public string VQueueObjects
        {
            get
            {
                return _vQueueObjects;
            }
            set
            {
                _vQueueObjects = value;
            }
        }

        private string _Position;
        public string Position
        {
            get
            {
                return _Position;
            }
            set
            {
                _Position = value;
            }
        }



        private string _AudioPath;
        public string AudioPath
        {
            get
            {
                return _AudioPath;
            }
            set
            {
                _AudioPath = value;
            }
        }

        private string _NotifyBackground;
        public string NotifyBackground
        {
            get
            {
                return _NotifyBackground;
            }
            set
            {
                _NotifyBackground = value;
            }
        }

        private string _NotifyForeground;
        public string NotifyForeground
        {
            get
            {
                return _NotifyForeground;
            }
            set
            {
                _NotifyForeground = value;
            }
        }

        private Color _dbColor;
        public Color DBColor
        {
            get { return _dbColor; }
            set { _dbColor = value; }
        }

        private Color _serverColor;
        public Color ServerColor
        {
            get { return _serverColor; }
            set { _serverColor = value; }
        }

        private string _businessAttributeName;
        public string BusinessAttributeName
        {
            get { return _businessAttributeName; }
            set { _businessAttributeName = value; }
           
        }

        private string _queueObjectType;
        public string QueueObjectType
        {
            get { return _queueObjectType; }
            set { _queueObjectType = value; }
        }

        private string _switches;
        public string Switches
        {
            get
            {
                return _switches;
            }
            set
            {
                _switches = value;
            }
        }

        private int _maxObject;
        public int MaxObject
        {
            get
            {
                return _maxObject;
            }
            set
            {
                _maxObject = value;
            }
        }

        private string _serverName = string.Empty;
        public string ServerName
        {
            get { return _serverName; }
            set { _serverName = value; }
        }
        #endregion
    }
}
