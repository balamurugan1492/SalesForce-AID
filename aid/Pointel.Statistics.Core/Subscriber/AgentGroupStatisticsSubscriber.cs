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

    class AgentGroupStatisticsSubscriber : IObserver<IStatisticsCollection>
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
            int tempRefid = StatisticsSetting.GetInstance().ReferenceId + 1;
            try
            {
                logger.Debug("AgentGroupStatisticsSubscriber : OnNext Method : Entry");
                if (value != null)
                {
                    if (value.AgentGroupStatistics != null && value.AgentGroupStatistics.Count != 0)
                    {
                        try
                        {
                            foreach (IAgentGroupStatistics agentgroupstat in value.AgentGroupStatistics)
                            {
                                string[] configuredstat = value.StatisticsCommon.AgentGroupStatistics.Split(',');
                                {
                                    foreach (string configstat in configuredstat)
                                    {
                                        if (configstat == agentgroupstat.TempStatName)
                                        {
                                            for (int groupIndex = 0; groupIndex < StatisticsSetting.GetInstance().AgentGroupsListCollections.Count; groupIndex++)
                                            {
                                                string statName = agentgroupstat.StatisticsName.ToString();
                                                string filter = agentgroupstat.FilterName.ToString();
                                                string serverName = agentgroupstat.ServerName.ToString();
                                                string format = agentgroupstat.StatisticsFormat.ToString();

                                                logger.Info("AgentGroupStatisticsSubscriber : OnNext Method : Request : ReferenceId - " + tempRefid);
                                                logger.Info("AgentGroupStatisticsSubscriber : OnNext Method : Request : StatName - " + statName);
                                                logger.Info("AgentGroupStatisticsSubscriber : OnNext Method : Request : AgentGroupName - " + StatisticsSetting.GetInstance().AgentGroupsListCollections[groupIndex]);
                                                logger.Info("AgentGroupStatisticsSubscriber : OnNext Method : Request : ServerName - " + serverName);

                                                IMessage response = null;
                                                if (tempRefid < StatisticsSetting.GetInstance().BAttributeReferenceId)
                                                {
                                                    response = statreq.StatRequest(value.StatisticsCommon.TenantName, StatisticsSetting.GetInstance().AgentGroupsListCollections[groupIndex],
                                                      StatisticObjectType.GroupAgents, statName, value.StatisticsCommon.NotifySeconds,
                                                       filter, value.StatisticsCommon.Insensitivity, tempRefid, serverName);
                                                }
                                                else
                                                {
                                                    response = statreq.StatRequest(value.StatisticsCommon.TenantName, StatisticsSetting.GetInstance().AgentGroupsListCollections[groupIndex],
                                                      StatisticObjectType.GroupAgents, value.StatisticsCommon.NotifySeconds,
                                                       filter, configstat, tempRefid, serverName);
                                                }

                                                logger.Info("AgentGroupStatisticsSubscriber : OnNext Method : Request : StatName - " + statName);

                                                if (response != null)
                                                {
                                                    statVariables = new StatVariables();
                                                    switch (response.Id)
                                                    {
                                                        case EventStatisticOpened.MessageId:
                                                            EventStatisticOpened info;
                                                            info = (EventStatisticOpened)response;
                                                            statVariables.Statfilter = filter;
                                                            statVariables.StatType = StatisticObjectType.GroupAgents.ToString();
                                                            statVariables.DisplayName = agentgroupstat.DisplayName;
                                                            statVariables.Tooltip = agentgroupstat.ToolTipName;
                                                            statVariables.Statformat = format;
                                                            statVariables.ObjectType = agentgroupstat.TempStatName;
                                                            statVariables.ObjectID = StatisticsSetting.GetInstance().AgentGroupsListCollections[groupIndex];
                                                            statVariables.ReferenceId = info.ReferenceId.ToString();

                                                            tempThresholdValues = new List<string>();
                                                            tempThresholdValues.Add(agentgroupstat.ThresholdLevelOne);
                                                            tempThresholdValues.Add(agentgroupstat.ThresholdLevelTwo);

                                                            tempThresholdColors = new List<Color>();
                                                            tempThresholdColors.Add(agentgroupstat.StatColor);
                                                            tempThresholdColors.Add(agentgroupstat.ThresholdColorOne);
                                                            tempThresholdColors.Add(agentgroupstat.ThresholdColorTwo);

                                                            StatisticsBase.GetInstance().IsMyStats = true;

                                                            if (!StatisticsSetting.GetInstance().ListRequestIds.Contains(tempRefid))
                                                                StatisticsSetting.GetInstance().ListRequestIds.Add(tempRefid);

                                                            if (!StatisticsSetting.GetInstance().ThresholdValues.ContainsKey(info.ReferenceId.ToString()))
                                                                StatisticsSetting.GetInstance().ThresholdValues.Add(info.ReferenceId.ToString(), tempThresholdValues);

                                                            if (!StatisticsSetting.GetInstance().ThresholdColors.ContainsKey(info.ReferenceId.ToString()))
                                                                StatisticsSetting.GetInstance().ThresholdColors.Add(info.ReferenceId.ToString(), tempThresholdColors);

                                                            if (!StatisticsSetting.GetInstance().DictAllStats.ContainsKey(info.ReferenceId.ToString()))
                                                                StatisticsSetting.GetInstance().DictAllStats.Add(info.ReferenceId.ToString(), agentgroupstat.TempStatName);

                                                            if (!StatisticsSetting.GetInstance().agentGroupStatsValueHolder.ContainsKey(info.ReferenceId))
                                                            {
                                                                StatisticsSetting.GetInstance().agentGroupStatsValueHolder.Add(info.ReferenceId, statVariables);
                                                            }

                                                            if (!StatisticsSetting.GetInstance().FinalAgentGroupPackageID.Contains(info.ReferenceId))
                                                                StatisticsSetting.GetInstance().FinalAgentGroupPackageID.Add(info.ReferenceId);

                                                            break;
                                                        case EventError.MessageId:
                                                            EventError eventError = (EventError)response;
                                                            logger.Error("AgentGroupStatisticsSubscriber : OnNext Method : " + eventError.StringValue);
                                                            break;
                                                    }
                                                    statVariables = null;
                                                }
                                                tempRefid++;
                                            }
                                        }
                                    }
                                }

                            }
                        }
                        catch (Exception generalException)
                        {
                            logger.Error("AgentGroupStatisticsSubscriber : OnNext Method : " + generalException.Message);
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
                logger.Error("AgentGroupStatisticsSubscriber : OnNext Method : " + GeneralException.Message);
            }
            finally
            {
                logger.Debug("AgentGroupStatisticsSubscriber : OnNext Method : Exit");
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

        //StatisticMetric statMetric;

        #endregion Other
    }
}