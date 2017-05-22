#region System Namespace
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
#endregion

#region Pointel Namespace
using Pointel.Logger.Core;
using Pointel.Statistics.Core.ConnectionManager;
using Pointel.Statistics.Core.StatisticsProvider;
using Pointel.Statistics.Core.Provider;
using Pointel.Statistics.Core.StatisticsRequest;
using Pointel.Statistics.Core.Utility;
#endregion

#region Genesys Namespace
using Genesyslab.Platform.Reporting.Protocols.StatServer;
using Genesyslab.Platform.Reporting.Protocols.StatServer.Events;
using Genesyslab.Platform.Commons.Protocols;
#endregion


namespace Pointel.Statistics.Core.Subscriber
{
    internal class DNGroupStatisticsSubscriber : IObserver<IStatisticsCollection>
    {
        #region Field Declaration
        private static ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "STF");
        private IDisposable _cancellation;
        StatisticsSetting statSettings = new StatisticsSetting();
        StatVariables statVariables;
        //StatisticMetric statMetric;

        #endregion

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
            int tempRefid = StatisticsSetting.GetInstance().ReferenceId + 100;
            try
            {
                logger.Debug("DNGroupStatisticsSubscriber : OnNext Method : Entry");
                if (value != null)
                {
                    if (value.DNGroupStatistics != null && value.DNGroupStatistics.Count != 0)
                    {
                        try
                        {
                            string[] DNGroups = value.StatisticsCommon.DNGroupObjects.Split(',');
                            foreach (string dngroup in DNGroups)
                            {
                                StatisticsSetting.GetInstance().DNGroupListCollections.Add(dngroup);
                            }

                            foreach (IDNGroupStatistics dnstat in value.DNGroupStatistics)
                            {
                                string[] configuredstat = value.StatisticsCommon.DNGroupStatistics.Split(',');
                                {
                                    foreach (string configstat in configuredstat)
                                    {
                                        if (configstat == dnstat.TempStatName)
                                        {
                                            for (int DNIndex = 0; DNIndex < StatisticsSetting.GetInstance().DNGroupListCollections.Count; DNIndex++)
                                            {
                                                string statName = dnstat.StatisticsName.ToString();
                                                string filter = dnstat.FilterName.ToString();
                                                string serverName = dnstat.ServerName.ToString();
                                                string strStatType = dnstat.StatisticsType.ToString();
                                                string format = dnstat.StatisticsFormat.ToString();

                                                string Dilimitor = "_@";
                                                string[] vq_Switch = StatisticsSetting.GetInstance().DNGroupListCollections[DNIndex].Split(new[] { Dilimitor }, StringSplitOptions.None);


                                                logger.Info("DNGroupStatisticsSubscriber : OnNext Method : Request : ReferenceId - " + tempRefid);
                                                logger.Info("DNGroupStatisticsSubscriber : OnNext Method : Request : StatName - " + statName);
                                                logger.Info("DNGroupStatisticsSubscriber : OnNext Method : Request : DNGroupName - " + StatisticsSetting.GetInstance().DNGroupListCollections[DNIndex]);
                                                logger.Info("DNGroupStatisticsSubscriber : OnNext Method : Request : ServerName - " + serverName);

                                                IMessage response;
                                                if (tempRefid < StatisticsSetting.GetInstance().BAttributeReferenceId)
                                                {
                                                    response = statreq.StatRequest(value.StatisticsCommon.TenantName, vq_Switch[0],
                                                    StatisticObjectType.GroupQueues, statName, value.StatisticsCommon.NotifySeconds,
                                                     filter, value.StatisticsCommon.Insensitivity, tempRefid, serverName);
                                                }
                                                else
                                                {
                                                    response = statreq.StatRequest(value.StatisticsCommon.TenantName, vq_Switch[0],
                                                   StatisticObjectType.GroupQueues, value.StatisticsCommon.NotifySeconds,
                                                    filter, configstat, tempRefid, serverName);
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
                                                            statVariables.StatType = StatisticObjectType.GroupPlaces.ToString();
                                                            //statVariables.DisplayName = dnstat.DisplayName;
                                                            statVariables.Tooltip = dnstat.ToolTipName;
                                                            statVariables.Statformat = format;
                                                            statVariables.ObjectType = dnstat.TempStatName;
                                                            statVariables.ReferenceId = info.ReferenceId.ToString();
                                                            //string[] tempQueueName = StatisticsSetting.GetInstance().DNGroupListCollections[DNIndex].Split(new char[] { '@' });
                                                            //statVariables.ObjectID = tempQueueName[0].ToString();
                                                            string[] tempQueueName = StatisticsSetting.GetInstance().DNGroupListCollections[DNIndex].Split(new char[] { '@' });
                                                            statVariables.DisplayName = dnstat.DisplayName;
                                                            if (StatisticsBase.GetInstance().QueueDisplayName == StatisticsEnum.ACDDisplayName.Queue.ToString() || StatisticsBase.GetInstance().QueueDisplayName == StatisticsEnum.ACDDisplayName.RoutingPoint.ToString())
                                                            {
                                                                statVariables.ObjectID = tempQueueName[0].ToString();
                                                                statVariables.CCStat = dnstat.DisplayName + " " + tempQueueName[0].ToString();
                                                            }
                                                            else if (StatisticsBase.GetInstance().QueueDisplayName == StatisticsEnum.ACDDisplayName.Skill.ToString())
                                                            {
                                                                statVariables.ObjectID = tempQueueName[1].ToString();
                                                                statVariables.CCStat = dnstat.DisplayName + " " + tempQueueName[1].ToString();
                                                            }
                                                            else
                                                            {
                                                                statVariables.ObjectID = tempQueueName[0].ToString();
                                                                statVariables.CCStat = dnstat.DisplayName + " " + tempQueueName[0].ToString();
                                                            }

                                                            tempThresholdValues = new List<string>();
                                                            tempThresholdValues.Add(dnstat.ThresholdLevelOne);
                                                            tempThresholdValues.Add(dnstat.ThresholdLevelTwo);

                                                            tempThresholdColors = new List<Color>();
                                                            tempThresholdColors.Add(dnstat.StatColor);
                                                            tempThresholdColors.Add(dnstat.ThresholdColorOne);
                                                            tempThresholdColors.Add(dnstat.ThresholdColorTwo);

                                                            StatisticsBase.GetInstance().IsCCStats = true;

                                                            if (!StatisticsSetting.GetInstance().ThresholdValues.ContainsKey(info.ReferenceId.ToString()))
                                                                StatisticsSetting.GetInstance().ThresholdValues.Add(info.ReferenceId.ToString(), tempThresholdValues);

                                                            if (!StatisticsSetting.GetInstance().ThresholdColors.ContainsKey(info.ReferenceId.ToString()))
                                                                StatisticsSetting.GetInstance().ThresholdColors.Add(info.ReferenceId.ToString(), tempThresholdColors);

                                                            if (!StatisticsSetting.GetInstance().DictAllStats.ContainsKey(info.ReferenceId.ToString()))
                                                                StatisticsSetting.GetInstance().DictAllStats.Add(info.ReferenceId.ToString(), dnstat.TempStatName);

                                                            if (!StatisticsSetting.GetInstance().dnGroupStatisticsValueHolder.ContainsKey(info.ReferenceId))
                                                            {
                                                                StatisticsSetting.GetInstance().dnGroupStatisticsValueHolder.Add(info.ReferenceId, statVariables);
                                                            }

                                                            StatisticsSetting.GetInstance().FinalDNGroupsPackageID.Add(info.ReferenceId);
                                                            break;
                                                        case EventError.MessageId:
                                                            EventError eventError = (EventError)response;
                                                            logger.Error("DNGroupStatisticsSubscriber : OnNext Method : " + eventError.StringValue);
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
                            logger.Error("DNGroupStatisticsSubscriber : OnNext Method : " + generalException.Message);
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
                logger.Error("DNGroupStatisticsSubscriber : OnNext Method : " + GeneralException.Message);
            }
            finally
            {
                logger.Debug("DNGroupStatisticsSubscriber : OnNext Method : Exit");
                statreq = null;
            }
        }

    }
}
