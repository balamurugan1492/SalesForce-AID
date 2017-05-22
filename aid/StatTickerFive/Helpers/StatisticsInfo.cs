using System.Drawing;
using Pointel.Statistics.Core.Utility;

namespace StatTickerFive.Helpers
{
    /// <summary>
    /// 
    /// </summary>
    public class StatisticsInfo
    {
        private string _statName;
        /// <summary>
        /// Gets or sets the name of the stat.
        /// </summary>
        /// <value>The name of the stat.</value>
        public string StatName
        {
            get { return _statName; }
            set { _statName = value; }
        }

        private string _statValue;
        /// <summary>
        /// Gets or sets the stat value.
        /// </summary>
        /// <value>The stat value.</value>
        public string StatValue
        {
            get { return _statValue; }
            set { _statValue = value; }
        }

        private string _statToolTip;
        /// <summary>
        /// Gets or sets the stat tool tip.
        /// </summary>
        /// <value>The stat tool tip.</value>
        public string StatToolTip
        {
            get { return _statToolTip; }
            set { _statToolTip = value; }
        }

        private Color _statColor;
        /// <summary>
        /// Gets or sets the color of the stat.
        /// </summary>
        /// <value>The color of the stat.</value>
        public Color StatColor
        {
            get { return _statColor; }
            set { _statColor = value; }
        }

        private string _statType;
        /// <summary>
        /// Gets or sets the type of the stat.
        /// </summary>
        /// <value>The type of the stat.</value>
        public string StatType
        {
            get { return _statType; }
            set { _statType = value; }
        }

        private string _statTag;
        /// <summary>
        /// Gets or sets the stat tag.
        /// </summary>
        /// <value>The stat tag.</value>
        public string StatTag
        {
            get { return _statTag; }
            set { _statTag = value; }
        }

        private string _refId;
        /// <summary>
        /// Gets or sets the ref id.
        /// </summary>
        /// <value>
        /// The ref id.
        /// </value>
        public string RefId
        {
            get { return _refId; }
            set { _refId = value; }
        }

        private string _dbStatName;
        /// <summary>
        /// Gets or sets the name of the database stat.
        /// </summary>
        /// <value>
        /// The name of the database stat.
        /// </value>        /// 
        public string DBStatName
        {
            get { return _dbStatName; }
            set { _dbStatName = value; }
        }

        private string _applicationType;
        public string ApplicationType
        {
            get
            {
                return _applicationType;
            }
            set
            {
                _applicationType = value;
            }
        }

        private bool _isLevelTwo;
        /// <summary>
        /// Gets or sets a value indicating whether [is level two].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [is level two]; otherwise, <c>false</c>.
        /// </value>
        public bool IsLevelTwo
        {
            get
            {
                return _isLevelTwo;
            }
            set
            {
                _isLevelTwo = value;
            }
        }

        private Color _backgroundColor;
        /// <summary>
        /// Gets or sets the background colors.
        /// </summary>
        /// <value>
        /// The background colors.
        /// </value>
        public Color BackgroundColors
        {
            get
            {
                return _backgroundColor;
            }
            set
            {
                _backgroundColor = value;
            }
        }

    }
}
