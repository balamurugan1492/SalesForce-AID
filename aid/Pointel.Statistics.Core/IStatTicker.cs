#region System Namespace
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
#endregion

#region Pointel Namespace
using Pointel.Statistics.Core.Utility;
#endregion

#region Genesys Namespace
using Genesyslab.Platform.Reporting.Protocols.StatServer.Events;
#endregion

namespace Pointel.Statistics.Core
{
    public interface IStatTicker
    {
        #region NotifyStatisticsErrorMessage
        /// <summary>
        /// Notifies the stat error message.
        /// </summary>
        /// <param name="errorMessage">The error message.</param>
        void NotifyStatErrorMessage(OutputValues output);
        #endregion

        #region NotifyAgentStatistics
        /// <summary>
        /// Notifies the agent statistics.
        /// </summary>
        /// <param name="statisticName">Name of the statistic.</param>
        /// <param name="statisticValue">The statistic value.</param>
        /// <param name="statType">Type of the stat.</param>
        /// <param name="toolTip">The tool tip.</param>
        /// <param name="color">The color.</param>
        void NotifyAgentStatistics(string refid, string statisticsName, string statisticsValue, string toolTip, Color statColor, string statType, bool isThresholdBreach, bool isLevelTwo);
        #endregion

        #region NotifyAgentGroupStatistics
        /// <summary>
        /// Notifies the agent group statistics.
        /// </summary>
        /// <param name="statisticName">Name of the statistic.</param>
        /// <param name="statisticValue">The statistic value.</param>
        /// <param name="statType">Type of the stat.</param>
        /// <param name="toolTip">The tool tip.</param>
        /// <param name="color">The color.</param>
        void NotifyAgentGroupStatistics(string refid, string statisticsName, string statisticsValue, string toolTip, Color statColor, string statType, bool isThresholdBreach, bool isLevelTwo);
        #endregion

        #region  NotifyQueueStatistics
        /// <summary>
        /// Notifies the queue statistics.
        /// </summary>
        /// <param name="statisticName">Name of the statistic.</param>
        /// <param name="statisticValue">The statistic value.</param>
        /// <param name="statType">Type of the stat.</param>
        /// <param name="toolTip">The tool tip.</param>
        /// <param name="color">The color.</param>
        void NotifyQueueStatistics(string refid, string statisticsName, string statisticsValue, string toolTip, Color statColor, string statType, bool isThresholdBreach, bool isLevelTwo);
        #endregion

        #region NotifyShowMyStatistics
        /// <summary>
        /// Notifies the show my statistics.
        /// </summary>
        /// <param name="isMyStatistics">if set to <c>true</c> [is my statistics].</param>
        void NotifyShowMyStatistics(bool isMyStatistics);
        #endregion

        #region NotifyShowCCStatistics
        /// <summary>
        /// Notifies the show CC statistics.
        /// </summary>
        /// <param name="isCCStatistics">if set to <c>true</c> [is CC statistics].</param>
        void NotifyShowCCStatistics(bool isCCStatistics);
        #endregion

        #region NotifyGadgetStatus
        /// <summary>
        /// Notifies the gadget status.
        /// </summary>
        /// <param name="gadgetstate">The gadgetstate.</param>
        void NotifyGadgetStatus(StatisticsEnum.GadgetState gadgetstate);
        #endregion

        #region NotifyAIDStatistics
        /// <summary>
        /// Notifies the aid statistics.
        /// </summary>
        /// <param name="StatisticsEvents">The statistics events.</param>
        void NotifyAIDStatistics(EventInfo StatisticsEvents);
        #endregion

        #region Notify DBStatistics value
        /// <summary>
        /// Notifies the database statistics.
        /// </summary>
        /// <param name="refid">The refid.</param>
        /// <param name="statisticsName">Name of the statistics.</param>
        /// <param name="statisticsValue">The statistics value.</param>
        /// <param name="toolTip">The tool tip.</param>
        /// <param name="statColor">Color of the stat.</param>
        /// <param name="isThresholdBreach">if set to <c>true</c> [is threshold breach].</param>
        /// <param name="dbStatName">Name of the database stat.</param>
        void NotifyDBStatistics(string refid, string statisticsName, string statisticsValue, string toolTip, Color statColor, bool isThresholdBreach, string dbStatName, bool isLevelTwo);
        #endregion

        #region Notify Admin Values

        void NotifyAgentStatus(string agentid, string status);

        #endregion

        #region NotifyStatServerStatustoTC
        void NotifyStatServerStatustoTC(bool status, string serverName);
        #endregion
    }
}
