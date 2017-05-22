#region System Namespace
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
#endregion
using Pointel.Statistics.Core.StatisticsProvider;

namespace Pointel.Statistics.Core.Utility
{
    internal class DNGroupStatistics : IDNGroupStatistics
    {
        #region Property

        private int _referenceId = 0;
        public int ReferenceID
        {
            get { return _referenceId; }
            set { _referenceId = value; }
        }

        private string _tempStatName = string.Empty;
        public string TempStatName
        {
            get { return _tempStatName; }
            set { _tempStatName = value; }
        }

        private string _displayName = string.Empty;
        public string DisplayName
        {
            get { return _displayName; }
            set { _displayName = value; }
        }

        private string _dnGroupName = string.Empty;
        public string DNGroupName
        {
            get { return _dnGroupName; }
            set { _dnGroupName = value; }
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

        private string _statisticsFormat = string.Empty;
        public string StatisticsFormat
        {
            get { return _statisticsFormat; }
            set { _statisticsFormat = value; }
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
