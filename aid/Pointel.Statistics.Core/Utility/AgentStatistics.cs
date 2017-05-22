#region System Namespace
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
#endregion

#region Pointel Namespace
using Pointel.Statistics.Core.StatisticsProvider;
#endregion

namespace Pointel.Statistics.Core.Utility
{
    internal class AgentStatistics : IAgentStatistics
    {
        #region Property

        private int _referenceId = 0;

        public int ReferenceID
        {
            get { return _referenceId; }
            set { _referenceId = value; }
        }

        private string _displayName = string.Empty;

        public string DisplayName
        {
            get { return _displayName; }
            set { _displayName = value; }
        }

        private string _tempStatName = string.Empty;

        public string TempStatName
        {
            get { return _tempStatName; }
            set { _tempStatName = value; }
        }

        private string _agentId = string.Empty;

        public string AgentID
        {
            get { return _agentId; }
            set { _agentId = value; }
        }


        private string _username = string.Empty;

        public string UserName
        {
            get { return _username; }
            set { _username = value; }
        }

        private Color _thresholdColorOne;

        public Color ThresholdColorOne
        {
            get { return _thresholdColorOne; }
            set { _thresholdColorOne = value; }
        }

        private Color _thresholdColorTwo;

        public Color ThresholdColorTwo
        {
            get { return _thresholdColorTwo; }
            set { _thresholdColorTwo = value; }
        }

        private Color _statColor;

        public Color StatColor
        {
            get { return _statColor; }
            set { _statColor = value; }
        }

        private string _filterName = string.Empty;

        public string FilterName
        {
            get { return _filterName; }
            set { _filterName = value; }
        }

        private string _statisticsformat = string.Empty;

        public string StatisticsFormat
        {
            get { return _statisticsformat; }
            set { _statisticsformat = value; }
        }

        private string _statisticsName = string.Empty;

        public string StatisticsName
        {
            get { return _statisticsName; }
            set { _statisticsName = value; }
        }

        private string _statisticsType = string.Empty;

        public string StatisticsType
        {
            get { return _statisticsType; }
            set { _statisticsType = value; }
        }

        private string _thresholdLevelOne = string.Empty;

        public string ThresholdLevelOne
        {
            get { return _thresholdLevelOne; }
            set { _thresholdLevelOne = value; }
        }

        private string _thresholdLevelTwo = string.Empty;

        public string ThresholdLevelTwo
        {
            get { return _thresholdLevelTwo; }
            set { _thresholdLevelTwo = value; }
        }

        private string _tooltipName = string.Empty;

        public string ToolTipName
        {
            get { return _tooltipName; }
            set { _tooltipName = value; }
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
