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
    internal class ACDStatisticsSubscriber : IObserver<IStatisticsCollection>
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
            int tempRefid = StatisticsSetting.GetInstance().ReferenceId + 50;
            try
            {
                logger.Debug("ACDStatisticsSubscriber : OnNext Method : Entry");
                if (value != null)
                {
                    if (value.ACDQueueStatistics != null && value.ACDQueueStatistics.Count != 0)
                    {
                        try
                        {
                            string[] ACDQueues = value.StatisticsCommon.ACDObjects.Split(',');
                            foreach (string acdqueues in ACDQueues)
                            {
                                StatisticsSetting.GetInstance().ACDQueuesListCollections.Add(acdqueues);
                            }

                            foreach (IACDStatistics acdstat in value.ACDQueueStatistics)
                            {
                                string[] configuredstat = value.StatisticsCommon.ACDStatistics.Split(',');
                                {
                                    foreach (string configstat in configuredstat)
                                    {
                                        if (configstat == acdstat.TempStatName)
                                        {
                                            for (int ACDIndex = 0; ACDIndex < StatisticsSetting.GetInstance().ACDQueuesListCollections.Count; ACDIndex++)
                                            {
                                                string Dilimitor = "_@";
                                                string statName = acdstat.StatisticsName.ToString();
                                                string filter = acdstat.FilterName.ToString();
                                                string serverName = acdstat.ServerName.ToString();
                                                string format = acdstat.StatisticsFormat.ToString();
                                                string[] Queue_switch = StatisticsSetting.GetInstance().ACDQueuesListCollections[ACDIndex].Split(new[] { Dilimitor }, StringSplitOptions.None);
                                                string ACDQueue = string.Empty;

                                                if (StatisticsBase.GetInstance().QueueDisplayName == StatisticsEnum.ACDDisplayName.Skill.ToString())
                                                {
                                                    if (Queue_switch.Length > 2)
                                                    {
                                                        ACDQueue = Queue_switch[0] + "@" + Queue_switch[1];
                                                    }
                                                }
                                                else if (StatisticsBase.GetInstance().QueueDisplayName == StatisticsEnum.ACDDisplayName.Queue.ToString() || StatisticsBase.GetInstance().QueueDisplayName == StatisticsEnum.ACDDisplayName.VirtualQueue.ToString() || StatisticsBase.GetInstance().QueueDisplayName == "")
                                                {
                                                    if (Queue_switch.Length > 2)
                                                    {
                                                        ACDQueue = Queue_switch[0] + "@" + Queue_switch[2];
                                                    }
                                                    else if (Queue_switch.Length == 2)
                                                    {
                                                        ACDQueue = Queue_switch[0] + "@" + Queue_switch[1];
                                                    }
                                                }
                                                else
                                                {
                                                    if (Queue_switch.Length == 2)
                                                    {
                                                        ACDQueue = Queue_switch[0] + "@" + Queue_switch[1];
                                                    }

                                                }

                                                //if (string.Compare(acdstat.TempStatName, Queue_switch[2].ToString(), true) == 0)
                                                {
                                                    logger.Info("ACDStatisticsSubscriber : OnNext Method : Request : ReferenceId - " + tempRefid);
                                                    logger.Info("ACDStatisticsSubscriber : OnNext Method : Request : StatName - " + statName);
                                                    logger.Info("ACDStatisticsSubscriber : OnNext Method : Request : QueueName - " + ACDQueue);
                                                    logger.Info("ACDStatisticsSubscriber : OnNext Method : Request : serverName - " + serverName);
                                                    IMessage response = null;
                                                    if(tempRefid < StatisticsSetting.GetInstance().BAttributeReferenceId)
                                                    {
                                                        response = statreq.StatRequest(value.StatisticsCommon.TenantName, ACDQueue,
                                                        StatisticObjectType.Queue, statName, value.StatisticsCommon.NotifySeconds,
                                                         filter, value.StatisticsCommon.Insensitivity, tempRefid,serverName);
                                                    }
                                                    else
                                                    {
                                                       response = statreq.StatRequest(value.StatisticsCommon.TenantName, ACDQueue,
                                                       StatisticObjectType.Queue, value.StatisticsCommon.NotifySeconds,
                                                        filter, configstat, tempRefid, serverName);
                                                    }

                                                    

                                                    logger.Info("ACDStatisticsSubscriber : OnNext Method : Response : " + response.ToString());

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
                                                                statVariables.Statformat = format;
                                                                statVariables.Tooltip = acdstat.ToolTipName;
                                                                statVariables.ObjectType = acdstat.TempStatName;
                                                                statVariables.ReferenceId = info.ReferenceId.ToString();
                                                                string[] tempQueueName = StatisticsSetting.GetInstance().ACDQueuesListCollections[ACDIndex].Split(new[] { Dilimitor }, StringSplitOptions.None);

                                                                if (StatisticsBase.GetInstance().QueueDisplayName == StatisticsEnum.ACDDisplayName.Queue.ToString() || StatisticsBase.GetInstance().QueueDisplayName == StatisticsEnum.ACDDisplayName.VirtualQueue.ToString())
                                                                {
                                                                    statVariables.ObjectID = tempQueueName[0].ToString();
                                                                    statVariables.DisplayName = acdstat.DisplayName + " " + tempQueueName[0].ToString();
                                                                }
                                                                else if (StatisticsBase.GetInstance().QueueDisplayName == StatisticsEnum.ACDDisplayName.Skill.ToString())
                                                                {
                                                                    statVariables.ObjectID = tempQueueName[1].ToString();
                                                                    statVariables.DisplayName = acdstat.DisplayName + " " + tempQueueName[1].ToString();
                                                                }
                                                                else
                                                                {
                                                                    statVariables.ObjectID = tempQueueName[0].ToString();
                                                                    statVariables.DisplayName = acdstat.DisplayName + " " + tempQueueName[0].ToString();
                                                                }

                                                                tempThresholdValues = new List<string>();
                                                                tempThresholdValues.Add(acdstat.ThresholdLevelOne);
                                                                tempThresholdValues.Add(acdstat.ThresholdLevelTwo);

                                                                tempThresholdColors = new List<Color>();
                                                                tempThresholdColors.Add(acdstat.StatColor);
                                                                tempThresholdColors.Add(acdstat.ThresholdColorOne);
                                                                tempThresholdColors.Add(acdstat.ThresholdColorTwo);

                                                                StatisticsBase.GetInstance().IsCCStats = true;

                                                                if (!StatisticsSetting.GetInstance().ThresholdValues.ContainsKey(info.ReferenceId.ToString()))
                                                                    StatisticsSetting.GetInstance().ThresholdValues.Add(info.ReferenceId.ToString(), tempThresholdValues);

                                                                if (!StatisticsSetting.GetInstance().ThresholdColors.ContainsKey(info.ReferenceId.ToString()))
                                                                    StatisticsSetting.GetInstance().ThresholdColors.Add(info.ReferenceId.ToString(), tempThresholdColors);

                                                                if (!StatisticsSetting.GetInstance().DictAllStats.ContainsKey(info.ReferenceId.ToString()))
                                                                    StatisticsSetting.GetInstance().DictAllStats.Add(info.ReferenceId.ToString(), acdstat.TempStatName);

                                                                if (!StatisticsSetting.GetInstance().ACDStatisticsValueHolder.ContainsKey(info.ReferenceId))
                                                                {
                                                                    StatisticsSetting.GetInstance().ACDStatisticsValueHolder.Add(info.ReferenceId, statVariables);
                                                                }

                                                                StatisticsSetting.GetInstance().FinalACDPackageID.Add(info.ReferenceId);
                                                                break;
                                                            case EventError.MessageId:
                                                                EventError eventError = (EventError)response;
                                                                logger.Error("ACDStatisticsSubscriber : OnNext Method: " + eventError.StringValue);
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
                            logger.Error("ACDStatisticsSubscriber : OnNext Method: " + generalException.Message);
                        }
                        finally
                        {
                        }
                    }
                }
            }
            catch (Exception GeneralException)
            {
                logger.Error("ACDStatisticsSubscriber : OnNext Method: " + GeneralException.Message);
            }
            finally
            {
                statreq = null;
                logger.Debug("ACDStatisticsSubscriber : OnNext Method: Method Exit");
            }
        }
    }
}
