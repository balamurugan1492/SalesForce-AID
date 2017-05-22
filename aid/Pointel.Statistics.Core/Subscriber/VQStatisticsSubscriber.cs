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
    internal class VQStatisticsSubscriber : IObserver<IStatisticsCollection>
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
            int tempRefid = StatisticsSetting.GetInstance().ReferenceId + 150;
            try
            {
                logger.Debug("VQStatisticsSubscriber : OnNext Method : Entry");
                if (value != null)
                {
                    if (value.VirtualQueueStatistics != null && value.VirtualQueueStatistics.Count != 0)
                    {
                        try
                        {
                            string[] VQueues = value.StatisticsCommon.VQueueObjects.Split(',');
                            foreach (string vqueue in VQueues)
                            {
                                StatisticsSetting.GetInstance().VQueueListCollections.Add(vqueue);
                            }

                            foreach (IVirtualQueueStatistics vqstat in value.VirtualQueueStatistics)
                            {
                                string[] configuredstat = value.StatisticsCommon.VQueueStatistics.Split(',');
                                {
                                    foreach (string configstat in configuredstat)
                                    {
                                        if (configstat == vqstat.TempStatName)
                                        {
                                            for (int VQIndex = 0; VQIndex < StatisticsSetting.GetInstance().VQueueListCollections.Count; VQIndex++)
                                            {
                                                string Dilimitor = "_@";
                                                string[] vq_Switch = StatisticsSetting.GetInstance().VQueueListCollections[VQIndex].Split(new[] { Dilimitor }, StringSplitOptions.None);

                                                string statName = vqstat.StatisticsName.ToString();
                                                string filter = vqstat.FilterName.ToString();
                                                string serverName = vqstat.ServerName.ToString();
                                                string format = vqstat.StatisticsFormat.ToString();

                                                //if (string.Compare(vqstat.TempStatName, vq_Switch[2].ToString(), true) == 0)
                                                {
                                                    logger.Info("VQStatisticsSubscriber : OnNext Method : Request : ReferenceId - " + tempRefid);
                                                    logger.Info("VQStatisticsSubscriber : OnNext Method : Request : StatName - " + statName);
                                                    logger.Info("VQStatisticsSubscriber : OnNext Method : Request : VQueueName - " + vq_Switch[0] + "@" + vq_Switch[1]);
                                                    logger.Info("VQStatisticsSubscriber : OnNext Method : Request : serverName - " + serverName);
                                                     IMessage response;
                                                     if (tempRefid < StatisticsSetting.GetInstance().BAttributeReferenceId)
                                                     {
                                                        response = statreq.StatRequest(value.StatisticsCommon.TenantName, vq_Switch[0] + "@" + vq_Switch[1],
                                                      StatisticObjectType.Queue, statName, value.StatisticsCommon.NotifySeconds,
                                                       filter, value.StatisticsCommon.Insensitivity, tempRefid, serverName);
                                                     }
                                                    else
                                                     {
                                                         response = statreq.StatRequest(value.StatisticsCommon.TenantName, vq_Switch[0] + "@" + vq_Switch[1],
                                                     StatisticObjectType.Queue, value.StatisticsCommon.NotifySeconds,
                                                      filter, configstat, tempRefid, serverName);
                                                     }
                                                  

                                                    logger.Info("VQStatisticsSubscriber : OnNext Method : Request : " + response.ToString());

                                                    if (response != null)
                                                    {
                                                        statVariables = new StatVariables();
                                                        switch (response.Id)
                                                        {
                                                            case EventStatisticOpened.MessageId:
                                                                EventStatisticOpened info;
                                                                info = (EventStatisticOpened)response;
                                                                statVariables.Statfilter = filter;
                                                                statVariables.StatType = StatisticObjectType.Queue.ToString();
                                                                //statVariables.DisplayName = vqstat.DisplayName;
                                                                statVariables.Tooltip = vqstat.ToolTipName;
                                                                statVariables.Statformat = format;
                                                                statVariables.ObjectType = vqstat.TempStatName;
                                                                statVariables.ReferenceId = info.ReferenceId.ToString();

                                                                string[] tempQueueName = StatisticsSetting.GetInstance().VQueueListCollections[VQIndex].Split(new[] { Dilimitor }, StringSplitOptions.None);
                                                                statVariables.DisplayName = vqstat.DisplayName;
                                                                if (StatisticsBase.GetInstance().QueueDisplayName == StatisticsEnum.ACDDisplayName.Queue.ToString() || StatisticsBase.GetInstance().QueueDisplayName == StatisticsEnum.ACDDisplayName.RoutingPoint.ToString())
                                                                {
                                                                    statVariables.ObjectID = tempQueueName[0].ToString();
                                                                    statVariables.CCStat = vqstat.DisplayName + " " + tempQueueName[0].ToString();
                                                                }
                                                                else if (StatisticsBase.GetInstance().QueueDisplayName == StatisticsEnum.ACDDisplayName.Skill.ToString())
                                                                {
                                                                    statVariables.ObjectID = tempQueueName[1].ToString();
                                                                    statVariables.CCStat = vqstat.DisplayName + " " + tempQueueName[1].ToString();
                                                                }
                                                                else
                                                                {
                                                                    statVariables.ObjectID = tempQueueName[0].ToString();
                                                                    statVariables.CCStat = vqstat.DisplayName + " " + tempQueueName[0].ToString();
                                                                }

                                                                tempThresholdValues = new List<string>();
                                                                tempThresholdValues.Add(vqstat.ThresholdLevelOne);
                                                                tempThresholdValues.Add(vqstat.ThresholdLevelTwo);

                                                                tempThresholdColors = new List<Color>();
                                                                tempThresholdColors.Add(vqstat.StatColor);
                                                                tempThresholdColors.Add(vqstat.ThresholdColorOne);
                                                                tempThresholdColors.Add(vqstat.ThresholdColorTwo);

                                                                StatisticsBase.GetInstance().IsCCStats = true;

                                                                if (!StatisticsSetting.GetInstance().ThresholdValues.ContainsKey(info.ReferenceId.ToString()))
                                                                    StatisticsSetting.GetInstance().ThresholdValues.Add(info.ReferenceId.ToString(), tempThresholdValues);

                                                                if (!StatisticsSetting.GetInstance().ThresholdColors.ContainsKey(info.ReferenceId.ToString()))
                                                                    StatisticsSetting.GetInstance().ThresholdColors.Add(info.ReferenceId.ToString(), tempThresholdColors);

                                                                if (!StatisticsSetting.GetInstance().DictAllStats.ContainsKey(info.ReferenceId.ToString()))
                                                                    StatisticsSetting.GetInstance().DictAllStats.Add(info.ReferenceId.ToString(), vqstat.TempStatName);

                                                                if (!StatisticsSetting.GetInstance().VQStatisticsValueHolder.ContainsKey(info.ReferenceId))
                                                                {
                                                                    StatisticsSetting.GetInstance().VQStatisticsValueHolder.Add(info.ReferenceId, statVariables);
                                                                }

                                                                StatisticsSetting.GetInstance().FinalVQPackageID.Add(info.ReferenceId);
                                                                break;
                                                            case EventError.MessageId:
                                                                EventError eventError = (EventError)response;
                                                                logger.Error("VQStatisticsSubscriber : OnNext Method: " + eventError.StringValue);
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
                        }
                        catch (Exception generalException)
                        {
                            logger.Error("VQStatisticsSubscriber : OnNext Method : " + generalException.Message);
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
                logger.Error("VQStatisticsSubscriber : OnNext Method : " + GeneralException.Message);
            }
            finally
            {
                logger.Debug("VQStatisticsSubscriber : OnNext Method : Exit");
                statreq = null;
            }
        }

    }
}
