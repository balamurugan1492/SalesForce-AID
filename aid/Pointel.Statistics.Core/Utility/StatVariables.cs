#region System Namespace
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#endregion

namespace Pointel.Statistics.Core.Utility
{
    public class StatVariables
    {
        #region Field Declarations

        private string _objectType = string.Empty;
        private string _referenceId = string.Empty;
        private string _displayName = string.Empty;
        private string _objectID = string.Empty;
        private string _statType = string.Empty;
        private string _statValue = string.Empty;
        private string _statfilter = string.Empty;
        private string _statformat = string.Empty;
        private string _agentState = string.Empty;
        private string _tooltip = string.Empty;
        private string _ccStat = string.Empty;
        #endregion

        #region Properties

        #region Object Type

        /// <summary>
        /// Gets or sets the type of the object.
        /// </summary>
        /// <value>
        /// The type of the object.
        /// </value>
        public string ObjectType
        {
            get
            {
                return _objectType;
            }
            set
            {
                _objectType = value;
            }
        }

        #endregion

        #region Object DisplayName

        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        /// <value>
        /// The display name.
        /// </value>
        public string DisplayName
        {
            get
            {
                return _displayName;
            }
            set
            {
                _displayName = value;
            }
        }

        #endregion

        #region Object Tooltip

        /// <summary>
        /// Gets or sets the tooltip.
        /// </summary>
        /// <value>
        /// The tooltip.
        /// </value>
        public string Tooltip
        {
            get
            {
                return _tooltip;
            }
            set
            {
                _tooltip = value;
            }
        }

        #endregion

        #region Object Id

        /// <summary>
        /// Gets or sets the object unique identifier.
        /// </summary>
        /// <value>
        /// The object unique identifier.
        /// </value>
        public string ObjectID
        {
            get
            {
                return _objectID;
            }
            set
            {
                _objectID = value;
            }
        }

        #endregion

        #region Stat Type

        /// <summary>
        /// Gets or sets the type of the stat.
        /// </summary>
        /// <value>
        /// The type of the stat.
        /// </value>
        public string StatType
        {
            get
            {
                return _statType;
            }
            set
            {
                _statType = value;
            }
        }

        #endregion

        #region Stat Value

        /// <summary>
        /// Gets or sets the stat value.
        /// </summary>
        /// <value>
        /// The stat value.
        /// </value>
        public string StatValue
        {
            get
            {
                return _statValue;
            }
            set
            {
                _statValue = value;
            }
        }

        #endregion

        #region Stat Filter

        /// <summary>
        /// Gets or sets the statfilter.
        /// </summary>
        /// <value>
        /// The statfilter.
        /// </value>
        public string Statfilter
        {
            get { return _statfilter; }
            set { _statfilter = value; }
        }

        #endregion

        #region Stat Format

        /// <summary>
        /// Gets or sets the statformat.
        /// </summary>
        /// <value>
        /// The statformat.
        /// </value>
        public string Statformat
        {
            get { return _statformat; }
            set { _statformat = value; }
        }

        public string ReferenceId
        {
            get { return _referenceId; }
            set { _referenceId = value; }
        }

        #endregion

        #region Agent Status

        /// <summary>
        /// Gets or sets the agent status.
        /// </summary>
        /// <value>The agent status.</value>
        public string AgentStatus
        {
            get { return _agentState; }
            set { _agentState = value; }
        }

        #endregion

        #region CC Stat

        /// <summary>
        /// Gets or sets the agent status.
        /// </summary>
        /// <value>The agent status.</value>
        public string CCStat
        {
            get { return _ccStat; }
            set { _ccStat = value; }
        }

        #endregion

        #endregion
    }
}
