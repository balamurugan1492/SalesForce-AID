namespace Pointel.Statistics.Core.Subscriber
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Text;

    using Genesyslab.Platform.Commons.Protocols;
    using Genesyslab.Platform.Reporting.Protocols.StatServer;
    using Genesyslab.Platform.Reporting.Protocols.StatServer.Events;

    using Pointel.Logger.Core;
    using Pointel.Statistics.Core.ConnectionManager;
    using Pointel.Statistics.Core.Provider;
    using Pointel.Statistics.Core.StatisticsProvider;
    using Pointel.Statistics.Core.StatisticsRequest;
    using Pointel.Statistics.Core.Utility;

    internal class AgentStatisticsSubscriber : IObserver<IStatisticsCollection>
    {
        #region Fields

        private static ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "STF");

        StatisticsSetting statSettings = new StatisticsSetting();
        StatVariables statVariables;
        private IDisposable _cancellation;

        #endregion Fields

        #region Methods

        /// <summary>
        /// Called when [completed].
        /// </summary>
        public void OnCompleted()
        {
            Unsubscribe();
        }

        /// <summary>
        /// Called when [error].
        /// </summary>
        /// <param name="error">The error.</param>
        public void OnError(Exception error)
        {
            //throw new NotImplementedException();
        }

        /// <summary>
        /// Called when [next].
        /// </summary>
        /// <param name="value">The value.</param>
        public void OnNext(IStatisticsCollection value)
        {
            Request statreq = new Request();
            List<string> tempThresholdValues;
            List<Color> tempThresholdColors;
            try
            {
                logger.Debug("AgentStatisticsSubscriber : OnNext Method : Entry");
                if (value != null)
                {
                    if (value.AgentStatistics != null && value.AgentStatistics.Count != 0)
                    {
                        try
                        {
                            foreach (IAgentStatistics agentstat in value.AgentStatistics)
                            {
                                string[] configuredstat = value.StatisticsCommon.AgentStatistics.Split(',');
                                {
                                    foreach (string configstat in configuredstat)
                                    {
                                        if (configstat == agentstat.TempStatName)
                                        {
                                            string statName = agentstat.StatisticsName.ToString();
                                            string filter = agentstat.FilterName.ToString();
                                            string serverName = agentstat.ServerName.ToString();
                                            string format = agentstat.StatisticsFormat.ToString();

                                            logger.Info("AgentStatisticsSubscriber : OnNext Method : Request : ReferenceId - " + agentstat.ReferenceID);
                                            logger.Info("AgentStatisticsSubscriber : OnNext Method : Request : StatName - " + statName);
                                            logger.Info("AgentStatisticsSubscriber : OnNext Method : Request : ServerName - " + serverName);

                                            IMessage response;
                                            if (!StatisticsSetting.GetInstance().isAdmin)
                                            {
                                                if (agentstat.ReferenceID < StatisticsSetting.GetInstance().BAttributeReferenceId)
                                                {
                                                    logger.Info("AgentStatisticsSubscriber : OnNext Method : Request : AgentEmpId - " + StatisticsSetting.GetInstance().AgentEmpId);
                                                    response = statreq.StatRequest(value.StatisticsCommon.TenantName, StatisticsSetting.GetInstance().AgentEmpId,
                                                       StatisticObjectType.Agent, statName, value.StatisticsCommon.NotifySeconds,
                                                        filter, value.StatisticsCommon.Insensitivity, agentstat.ReferenceID, serverName);
                                                }
                                                else
                                                {
                                                    //logger.Info("AgentStatisticsSubscriber : OnNext Method : Request : AgentEmpId - " + StatisticsSetting.GetInstance().AgentEmpId);
                                                    response = statreq.StatRequest(value.StatisticsCommon.TenantName, StatisticsSetting.GetInstance().AgentEmpId,
                                                       StatisticObjectType.Agent, value.StatisticsCommon.NotifySeconds,
                                                        filter, configstat, agentstat.ReferenceID, serverName);

                                                }
                                            }
                                            else
                                            {
                                                if (!StatisticsSetting.GetInstance().RequestIds.Contains(agentstat.ReferenceID))
                                                    StatisticsSetting.GetInstance().RequestIds.Add(agentstat.ReferenceID);
                                                response = statreq.StatRequest(value.StatisticsCommon.TenantName, StatisticsSetting.GetInstance().IndividualAgent,
                                                    StatisticObjectType.Agent, statName, value.StatisticsCommon.NotifySeconds,
                                                     filter, value.StatisticsCommon.Insensitivity, agentstat.ReferenceID, serverName);
                                            }

                                            if (response != null)
                                            {
                                                statVariables = new StatVariables();
                                                switch (response.Id)
                                                {
                                                    case EventStatisticOpened.MessageId:
                                                        EventStatisticOpened info;
                                                        info = (EventStatisticOpened)response;
                                                        statVariables.Statfilter = filter;
                                                        statVariables.StatType = StatisticObjectType.Agent.ToString();
                                                        statVariables.DisplayName = agentstat.DisplayName;
                                                        statVariables.Tooltip = agentstat.ToolTipName;
                                                        statVariables.Statformat = format;
                                                        statVariables.ObjectType = agentstat.TempStatName;
                                                        statVariables.ObjectID = StatisticsSetting.GetInstance().AgentEmpId;
                                                        statVariables.ReferenceId = info.ReferenceId.ToString();

                                                        tempThresholdValues = new List<string>();
                                                        tempThresholdValues.Add(agentstat.ThresholdLevelOne);
                                                        tempThresholdValues.Add(agentstat.ThresholdLevelTwo);

                                                        tempThresholdColors = new List<Color>();
                                                        tempThresholdColors.Add(agentstat.StatColor);
                                                        tempThresholdColors.Add(agentstat.ThresholdColorOne);
                                                        tempThresholdColors.Add(agentstat.ThresholdColorTwo);

                                                        StatisticsBase.GetInstance().IsMyStats = true;

                                                        if (!StatisticsSetting.GetInstance().ListRequestIds.Contains(agentstat.ReferenceID))
                                                            StatisticsSetting.GetInstance().ListRequestIds.Add(agentstat.ReferenceID);

                                                        if (!StatisticsSetting.GetInstance().ThresholdValues.ContainsKey(agentstat.ReferenceID.ToString()))
                                                            StatisticsSetting.GetInstance().ThresholdValues.Add(agentstat.ReferenceID.ToString(), tempThresholdValues);

                                                        if (!StatisticsSetting.GetInstance().ThresholdColors.ContainsKey(agentstat.ReferenceID.ToString()))
                                                            StatisticsSetting.GetInstance().ThresholdColors.Add(agentstat.ReferenceID.ToString(), tempThresholdColors);

                                                        if (!StatisticsSetting.GetInstance().DictAllStats.ContainsKey(info.ReferenceId.ToString()))
                                                            StatisticsSetting.GetInstance().DictAllStats.Add(info.ReferenceId.ToString(), agentstat.TempStatName);

                                                        if (!StatisticsSetting.GetInstance().agentStatisticsPluginHolder.ContainsKey(info.ReferenceId))
                                                        {
                                                            StatisticsSetting.GetInstance().agentStatisticsPluginHolder.Add(info.ReferenceId, statVariables);
                                                        }

                                                        StatisticsSetting.GetInstance().FinalAgentPackageID.Add(agentstat.ReferenceID);
                                                        break;
                                                    case EventError.MessageId:
                                                        EventError eventError = (EventError)response;
                                                        logger.Error("AgentStatisticsSubscriber : OnNext Method : " + eventError.StringValue);
                                                        break;
                                                }
                                                statVariables = null;
                                            }
                                        }
                                    }
                                }

                            }
                        }
                        catch (Exception generalException)
                        {
                            logger.Error("AgentStatisticsSubscriber : OnNext Method : " + generalException.Message);
                        }
                        finally
                        {
                            GC.Collect();
                        }
                    }
                }
            }
            catch (Exception GeneralException)
            {
                logger.Error("AgentStatisticsSubscriber : OnNext Method : " + GeneralException.Message);
            }
            finally
            {
                logger.Debug("AgentStatisticsSubscriber : OnNext Method : Exit");
                statreq = null;
            }
        }

        /// <summary>
        /// Subscribes the specified provider.
        /// </summary>
        /// <param name="provider">The provider.</param>
        public virtual void Subscribe(StatisticsDataProvider provider)
        {
            _cancellation = provider.Subscribe(this);
        }

        /// <summary>
        /// Unsubscribes this instance.
        /// </summary>
        public virtual void Unsubscribe()
        {
            _cancellation.Dispose();
        }

        #endregion Methods

        #region Other

        // StatisticMetric statMetric;

        #endregion Other
    }
}