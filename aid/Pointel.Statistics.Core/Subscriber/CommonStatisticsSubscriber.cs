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
    using System.Windows.Forms;

    internal class CommonStatisticsSubscriber : IObserver<IStatisticsCollection>
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
                logger.Debug("CommonStatisticsSubscriber : OnNext Method : Entry");
                if (value != null)
                {
                    try
                    {
                        string[] ACDQueues = value.StatisticsCommon.ACDObjects.Split(',');
                        if (ACDQueues.Length > 0)
                        {
                            StatisticsSetting.GetInstance().ACDQueuesListCollections.Clear();
                            foreach (string acdqueues in ACDQueues)
                            {
                                StatisticsSetting.GetInstance().ACDQueuesListCollections.Add(acdqueues);
                            }

                        }

                        string[] DNGroups = value.StatisticsCommon.DNGroupObjects.Split(',');
                        if (DNGroups.Length > 0)
                        {
                            StatisticsSetting.GetInstance().DNGroupListCollections.Clear();
                            foreach (string dngroup in DNGroups)
                            {
                                StatisticsSetting.GetInstance().DNGroupListCollections.Add(dngroup);
                            }
                        }

                        string[] VQueues = value.StatisticsCommon.VQueueObjects.Split(',');
                        if (VQueues.Length > 0)
                        {
                            StatisticsSetting.GetInstance().VQueueListCollections.Clear();
                            foreach (string vqueue in VQueues)
                            {
                                StatisticsSetting.GetInstance().VQueueListCollections.Add(vqueue);
                            }
                        }

                        Request(value.ACDQueueStatistics, "acd", StatisticsSetting.GetInstance().ACDQueuesListCollections, value.StatisticsCommon, "acd");

                        Request(value.DNGroupStatistics, "acd", StatisticsSetting.GetInstance().ACDQueuesListCollections, value.StatisticsCommon, "dn");

                        Request(value.VirtualQueueStatistics, "acd", StatisticsSetting.GetInstance().ACDQueuesListCollections, value.StatisticsCommon, "vq");

                        Request(value.ACDQueueStatistics, "dn", StatisticsSetting.GetInstance().DNGroupListCollections, value.StatisticsCommon, "acd");

                        Request(value.DNGroupStatistics, "dn", StatisticsSetting.GetInstance().DNGroupListCollections, value.StatisticsCommon, "dn");

                        Request(value.VirtualQueueStatistics, "dn", StatisticsSetting.GetInstance().DNGroupListCollections, value.StatisticsCommon, "vq");

                        Request(value.ACDQueueStatistics, "vq", StatisticsSetting.GetInstance().VQueueListCollections, value.StatisticsCommon, "acd");

                        Request(value.DNGroupStatistics, "vq", StatisticsSetting.GetInstance().VQueueListCollections, value.StatisticsCommon, "dn");

                        Request(value.VirtualQueueStatistics, "vq", StatisticsSetting.GetInstance().VQueueListCollections, value.StatisticsCommon, "vq");

                    }
                    catch (Exception generalException)
                    {
                        logger.Error("ACD Subscriber  : OnNext Method: " + generalException.Message);
                    }
                    finally
                    {
                    }

                }
            }
            catch (Exception GeneralException)
            {
                logger.Error("CommonStatisticsSubscriber : OnNext Method: " + GeneralException.Message);
            }
            finally
            {
                statreq = null;
                logger.Debug("CommonStatisticsSubscriber : OnNext Method: Method Exit");
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

        /// <summary>
        /// Requests the specified collection.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="objectCollection">The object collection.</param>
        /// <param name="statCommon">The stat common.</param>
        /// <param name="statType">Type of the stat.</param>
        private void Request(object collection, string objectType, List<string> objectCollection, IStatisticsCommon statCommon, string statType)
        {
            try
            {
                IStatisticsCollection value = new StatisticsCollection();
                Request statreq = new Request();
                List<string> tempThresholdValues;
                List<Color> tempThresholdColors;
                value.StatisticsCommon = statCommon;
                if (collection is List<IACDStatistics>)
                {

                    var collACD = collection as List<IACDStatistics>;
                    int tempRefid = StatisticsSetting.GetInstance().ReferenceId + 50;
                    foreach (var item in collACD)
                    {
                        string[] configuredstat;
                        if (objectType == "acd")
                        {
                            configuredstat = value.StatisticsCommon.ACDStatistics.Split(',');
                        }
                        else if (objectType == "dn")
                        {
                            configuredstat = value.StatisticsCommon.DNGroupStatistics.Split(',');
                        }
                        else
                        {
                            configuredstat = value.StatisticsCommon.VQueueStatistics.Split(',');
                        }

                        foreach (string configstat in configuredstat)
                        {
                            if (configstat == item.TempStatName)
                            {
                                for (int objIndex = 0; objIndex < objectCollection.Count; objIndex++)
                                {
                                    string Dilimitor = "_@";
                                    string statName = item.StatisticsName.ToString();
                                    string filter = item.FilterName.ToString();
                                    string serverName = item.ServerName.ToString();
                                    string format = item.StatisticsFormat.ToString();
                                    string queuename = string.Empty; ;
                                    IMessage response = null;
                                    if (objectType == "acd" || objectType == "vq")
                                    {
                                        string[] Queue_switch = objectCollection[objIndex].Split(new[] { Dilimitor }, StringSplitOptions.None);

                                        if (StatisticsBase.GetInstance().QueueDisplayName == StatisticsEnum.ACDDisplayName.Skill.ToString())
                                        {
                                            if (Queue_switch.Length > 2)
                                            {
                                                queuename = Queue_switch[0] + "@" + Queue_switch[1];
                                            }
                                        }
                                        else if (StatisticsBase.GetInstance().QueueDisplayName == StatisticsEnum.ACDDisplayName.Queue.ToString() || StatisticsBase.GetInstance().QueueDisplayName == StatisticsEnum.ACDDisplayName.VirtualQueue.ToString() || StatisticsBase.GetInstance().QueueDisplayName == "")
                                        {
                                            if (Queue_switch.Length > 2)
                                            {
                                                queuename = Queue_switch[0] + "@" + Queue_switch[2];
                                            }
                                            else if (Queue_switch.Length == 2)
                                            {
                                                queuename = Queue_switch[0] + "@" + Queue_switch[1];
                                            }
                                        }
                                        else
                                        {
                                            if (Queue_switch.Length == 2)
                                            {
                                                queuename = Queue_switch[0] + "@" + Queue_switch[1];
                                            }

                                        }

                                        logger.Info("ACD Subscriber : OnNext Method : Request : ReferenceId - " + tempRefid);
                                        logger.Info("ACD Subscriber : OnNext Method : Request : StatName - " + statName);
                                        logger.Info("ACD Subscriber : OnNext Method : Request : QueueName - " + queuename);
                                        logger.Info("ACD Subscriber : OnNext Method : Request : ServerName - " + serverName);

                                        if (!string.IsNullOrEmpty(queuename))
                                        {
                                            if (tempRefid < StatisticsSetting.GetInstance().BAttributeReferenceId)
                                            {
                                                //MessageBox.Show("acdstatrequest=" + queuename);
                                                response = statreq.StatRequest(value.StatisticsCommon.TenantName, queuename,
                                                StatisticObjectType.Queue, statName, value.StatisticsCommon.NotifySeconds,
                                                 filter, value.StatisticsCommon.Insensitivity, tempRefid, serverName);

                                                if (!StatisticsSetting.GetInstance().ListRequestIds.Contains(tempRefid))
                                                    StatisticsSetting.GetInstance().ListRequestIds.Add(tempRefid);
                                            }
                                            else
                                            {
                                                //MessageBox.Show("acdstatrequest=" + queuename);
                                                response = statreq.StatRequest(value.StatisticsCommon.TenantName, queuename,
                                                StatisticObjectType.Queue, value.StatisticsCommon.NotifySeconds,
                                                 filter, configstat, tempRefid, serverName);

                                                if (!StatisticsSetting.GetInstance().ListRequestIds.Contains(tempRefid))
                                                    StatisticsSetting.GetInstance().ListRequestIds.Add(tempRefid);
                                            }
                                        }
                                        else
                                        {
                                            if (objectType == "vq")
                                                logger.Warn("DN Subscriber : OnNext Method : Request : VQ is Null");
                                            else
                                                logger.Warn("DN Subscriber : OnNext Method : Request : ACD is Null");
                                        }
                                    }
                                    else if (objectType == "dn")
                                    {
                                        queuename = objectCollection[objIndex];

                                        logger.Info("ACD Subscriber : OnNext Method : Request : ReferenceId - " + tempRefid);
                                        logger.Info("ACD Subscriber : OnNext Method : Request : StatName - " + statName);
                                        logger.Info("ACD Subscriber : OnNext Method : Request : QueueName - " + queuename);
                                        logger.Info("ACD Subscriber : OnNext Method : Request : ServerName - " + serverName);

                                        if (!string.IsNullOrEmpty(queuename))
                                        {
                                            if (tempRefid < StatisticsSetting.GetInstance().BAttributeReferenceId)
                                            {
                                                //MessageBox.Show("acdstatrequest=" + queuename);
                                                response = statreq.StatRequest(value.StatisticsCommon.TenantName, queuename,
                                                StatisticObjectType.GroupQueues, statName, value.StatisticsCommon.NotifySeconds,
                                                 filter, value.StatisticsCommon.Insensitivity, tempRefid, serverName);

                                                if (!StatisticsSetting.GetInstance().ListRequestIds.Contains(tempRefid))
                                                    StatisticsSetting.GetInstance().ListRequestIds.Add(tempRefid);
                                            }
                                            else
                                            {
                                                //MessageBox.Show("acdstatrequest=" + queuename);
                                                response = statreq.StatRequest(value.StatisticsCommon.TenantName, queuename,
                                                StatisticObjectType.GroupQueues, value.StatisticsCommon.NotifySeconds,
                                                 filter, configstat, tempRefid, serverName);

                                                if (!StatisticsSetting.GetInstance().ListRequestIds.Contains(tempRefid))
                                                    StatisticsSetting.GetInstance().ListRequestIds.Add(tempRefid);
                                            }
                                        }
                                        else
                                        {
                                                logger.Warn("DN Subscriber : OnNext Method : Request : DNGroup is Null");
                                        }
                                    }

                                    logger.Info("ACD Subscriber : OnNext Method : Response : " + response.ToString());

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
                                                statVariables.Tooltip = item.ToolTipName;
                                                statVariables.ObjectType = item.TempStatName;
                                                statVariables.ReferenceId = info.ReferenceId.ToString();
                                                string[] tempQueueName = objectCollection[objIndex].Split(new[] { Dilimitor }, StringSplitOptions.None);

                                                if (StatisticsBase.GetInstance().QueueDisplayName == StatisticsEnum.ACDDisplayName.Queue.ToString() || StatisticsBase.GetInstance().QueueDisplayName == StatisticsEnum.ACDDisplayName.VirtualQueue.ToString())
                                                {
                                                    statVariables.ObjectID = tempQueueName[0].ToString();
                                                    statVariables.DisplayName = item.DisplayName + " " + tempQueueName[0].ToString();
                                                }
                                                else if (StatisticsBase.GetInstance().QueueDisplayName == StatisticsEnum.ACDDisplayName.Skill.ToString())
                                                {
                                                    statVariables.ObjectID = tempQueueName[1].ToString();
                                                    statVariables.DisplayName = item.DisplayName + " " + tempQueueName[1].ToString();
                                                }
                                                else
                                                {
                                                    statVariables.ObjectID = tempQueueName[0].ToString();
                                                    statVariables.DisplayName = item.DisplayName + " " + tempQueueName[0].ToString();
                                                }

                                                tempThresholdValues = new List<string>();
                                                tempThresholdValues.Add(item.ThresholdLevelOne);
                                                tempThresholdValues.Add(item.ThresholdLevelTwo);

                                                tempThresholdColors = new List<Color>();
                                                tempThresholdColors.Add(item.StatColor);
                                                tempThresholdColors.Add(item.ThresholdColorOne);
                                                tempThresholdColors.Add(item.ThresholdColorTwo);

                                                StatisticsBase.GetInstance().IsCCStats = true;

                                                if (!StatisticsSetting.GetInstance().ThresholdValues.ContainsKey(info.ReferenceId.ToString()))
                                                    StatisticsSetting.GetInstance().ThresholdValues.Add(info.ReferenceId.ToString(), tempThresholdValues);

                                                if (!StatisticsSetting.GetInstance().ThresholdColors.ContainsKey(info.ReferenceId.ToString()))
                                                    StatisticsSetting.GetInstance().ThresholdColors.Add(info.ReferenceId.ToString(), tempThresholdColors);

                                                if (!StatisticsSetting.GetInstance().DictAllStats.ContainsKey(info.ReferenceId.ToString()))
                                                    StatisticsSetting.GetInstance().DictAllStats.Add(info.ReferenceId.ToString(), item.TempStatName);

                                                if (!StatisticsSetting.GetInstance().ACDStatisticsValueHolder.ContainsKey(info.ReferenceId))
                                                {
                                                    StatisticsSetting.GetInstance().ACDStatisticsValueHolder.Add(info.ReferenceId, statVariables);
                                                }

                                                StatisticsSetting.GetInstance().FinalACDPackageID.Add(info.ReferenceId);
                                                break;
                                            case EventError.MessageId:
                                                EventError eventError = (EventError)response;
                                                logger.Error("ACD Subscriber : OnNext Method: " + eventError.StringValue);
                                                break;
                                        }
                                        statVariables = null;
                                    }
                                    tempRefid++;
                                }
                            }
                        }
                    }

                    StatisticsSetting.GetInstance().ReferenceId = tempRefid;
                }
                else if (collection is List<IDNGroupStatistics>)
                {
                    var collDn = collection as List<IDNGroupStatistics>;
                    int tempRefid = StatisticsSetting.GetInstance().ReferenceId + 50;
                    foreach (var item in collDn)
                    {

                        string[] configuredstat;
                        if (objectType == "acd")
                        {
                            configuredstat = value.StatisticsCommon.ACDStatistics.Split(',');
                        }
                        else if (objectType == "dn")
                        {
                            configuredstat = value.StatisticsCommon.DNGroupStatistics.Split(',');
                        }
                        else
                        {
                            configuredstat = value.StatisticsCommon.VQueueStatistics.Split(',');
                        }

                        foreach (string configstat in configuredstat)
                        {
                            if (configstat == item.TempStatName)
                            {
                                for (int objIndex = 0; objIndex < objectCollection.Count; objIndex++)
                                {
                                    string Dilimitor = "_@";
                                    string statName = item.StatisticsName.ToString();
                                    string filter = item.FilterName.ToString();
                                    string serverName = item.ServerName.ToString();
                                    string format = item.StatisticsFormat.ToString();
                                    string queuename = string.Empty;

                                    IMessage response = null;

                                    if (objectType == "acd" || objectType == "vq")
                                    {
                                        string[] Queue_switch = objectCollection[objIndex].Split(new[] { Dilimitor }, StringSplitOptions.None);

                                        if (StatisticsBase.GetInstance().QueueDisplayName == StatisticsEnum.ACDDisplayName.Skill.ToString())
                                        {
                                            if (Queue_switch.Length > 2)
                                            {
                                                queuename = Queue_switch[0] + "@" + Queue_switch[1];
                                            }
                                        }
                                        else if (StatisticsBase.GetInstance().QueueDisplayName == StatisticsEnum.ACDDisplayName.Queue.ToString() || StatisticsBase.GetInstance().QueueDisplayName == StatisticsEnum.ACDDisplayName.VirtualQueue.ToString() || StatisticsBase.GetInstance().QueueDisplayName == "")
                                        {
                                            if (Queue_switch.Length > 2)
                                            {
                                                queuename = Queue_switch[0] + "@" + Queue_switch[2];
                                            }
                                            else if (Queue_switch.Length == 2)
                                            {
                                                queuename = Queue_switch[0] + "@" + Queue_switch[1];
                                            }
                                        }
                                        else
                                        {
                                            if (Queue_switch.Length == 2)
                                            {
                                                queuename = Queue_switch[0] + "@" + Queue_switch[1];
                                            }

                                        }

                                        logger.Info("DN Subscriber : OnNext Method : Request : ReferenceId - " + tempRefid);
                                        logger.Info("DN Subscriber : OnNext Method : Request : StatName - " + statName);
                                        logger.Info("DN Subscriber : OnNext Method : Request : QueueName - " + queuename);
                                        logger.Info("DN Subscriber : OnNext Method : Request : Servername - " + serverName);

                                        if (!string.IsNullOrEmpty(queuename))
                                        {
                                            if (tempRefid < StatisticsSetting.GetInstance().BAttributeReferenceId)
                                            {
                                                //MessageBox.Show("dnstatrequest=" + queuename);
                                                response = statreq.StatRequest(value.StatisticsCommon.TenantName, queuename,
                                                StatisticObjectType.Queue, statName, value.StatisticsCommon.NotifySeconds,
                                                 filter, value.StatisticsCommon.Insensitivity, tempRefid, serverName);

                                                if (!StatisticsSetting.GetInstance().ListRequestIds.Contains(tempRefid))
                                                    StatisticsSetting.GetInstance().ListRequestIds.Add(tempRefid);
                                            }
                                            else
                                            {
                                                //MessageBox.Show("dnstatrequest=" + queuename);
                                                response = statreq.StatRequest(value.StatisticsCommon.TenantName, queuename,
                                                StatisticObjectType.Queue, value.StatisticsCommon.NotifySeconds,
                                                 filter, configstat, tempRefid, serverName);

                                                if (!StatisticsSetting.GetInstance().ListRequestIds.Contains(tempRefid))
                                                    StatisticsSetting.GetInstance().ListRequestIds.Add(tempRefid);
                                            }
                                        }
                                        else
                                        {
                                            if (objectType == "vq")
                                                logger.Warn("DN Subscriber : OnNext Method : Request : VQ is Null");
                                            else
                                                logger.Warn("DN Subscriber : OnNext Method : Request : ACD is Null");
                                        }

                                    }
                                    else if (objectType == "dn")
                                    {
                                        queuename = objectCollection[objIndex];

                                        logger.Info("DN Subscriber : OnNext Method : Request : ReferenceId - " + tempRefid);
                                        logger.Info("DN Subscriber : OnNext Method : Request : StatName - " + statName);
                                        logger.Info("DN Subscriber : OnNext Method : Request : QueueName - " + queuename);
                                        logger.Info("DN Subscriber : OnNext Method : Request : Servername - " + serverName);
                                        if (tempRefid < StatisticsSetting.GetInstance().BAttributeReferenceId)
                                        {
                                            //MessageBox.Show("dnstatrequest=" + queuename);
                                            response = statreq.StatRequest(value.StatisticsCommon.TenantName, queuename,
                                            StatisticObjectType.GroupQueues, statName, value.StatisticsCommon.NotifySeconds,
                                             filter, value.StatisticsCommon.Insensitivity, tempRefid, serverName);

                                            if (!StatisticsSetting.GetInstance().ListRequestIds.Contains(tempRefid))
                                                StatisticsSetting.GetInstance().ListRequestIds.Add(tempRefid);
                                        }
                                        else
                                        {
                                            //MessageBox.Show("dnstatrequest=" + queuename);
                                            response = statreq.StatRequest(value.StatisticsCommon.TenantName, queuename,
                                            StatisticObjectType.GroupQueues, value.StatisticsCommon.NotifySeconds,
                                             filter, configstat, tempRefid, serverName);

                                            if (!StatisticsSetting.GetInstance().ListRequestIds.Contains(tempRefid))
                                                StatisticsSetting.GetInstance().ListRequestIds.Add(tempRefid);
                                        }

                                    }

                                    logger.Info("DN Subscriber : OnNext Method : Response : " + response.ToString());

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
                                                statVariables.Tooltip = item.ToolTipName;
                                                statVariables.ObjectType = item.TempStatName;
                                                statVariables.ReferenceId = info.ReferenceId.ToString();
                                                string[] tempQueueName = objectCollection[objIndex].Split(new[] { Dilimitor }, StringSplitOptions.None);
                                                //statVariables.ObjectID = tempQueueName[0].ToString();
                                                statVariables.DisplayName = item.DisplayName;
                                                if (StatisticsBase.GetInstance().QueueDisplayName == StatisticsEnum.ACDDisplayName.Queue.ToString() || StatisticsBase.GetInstance().QueueDisplayName == StatisticsEnum.ACDDisplayName.VirtualQueue.ToString())
                                                {
                                                    statVariables.ObjectID = tempQueueName[0].ToString();
                                                    statVariables.CCStat = item.DisplayName + " " + tempQueueName[0].ToString();
                                                }
                                                else if (StatisticsBase.GetInstance().QueueDisplayName == StatisticsEnum.ACDDisplayName.Skill.ToString())
                                                {
                                                    statVariables.ObjectID = tempQueueName[1].ToString();
                                                    statVariables.CCStat = item.DisplayName + " " + tempQueueName[1].ToString();
                                                }
                                                else
                                                {
                                                    statVariables.ObjectID = tempQueueName[0].ToString();
                                                    statVariables.CCStat = item.DisplayName + " " + tempQueueName[0].ToString();
                                                }

                                                tempThresholdValues = new List<string>();
                                                tempThresholdValues.Add(item.ThresholdLevelOne);
                                                tempThresholdValues.Add(item.ThresholdLevelTwo);

                                                tempThresholdColors = new List<Color>();
                                                tempThresholdColors.Add(item.StatColor);
                                                tempThresholdColors.Add(item.ThresholdColorOne);
                                                tempThresholdColors.Add(item.ThresholdColorTwo);

                                                StatisticsBase.GetInstance().IsCCStats = true;

                                                if (!StatisticsSetting.GetInstance().ThresholdValues.ContainsKey(info.ReferenceId.ToString()))
                                                    StatisticsSetting.GetInstance().ThresholdValues.Add(info.ReferenceId.ToString(), tempThresholdValues);

                                                if (!StatisticsSetting.GetInstance().ThresholdColors.ContainsKey(info.ReferenceId.ToString()))
                                                    StatisticsSetting.GetInstance().ThresholdColors.Add(info.ReferenceId.ToString(), tempThresholdColors);

                                                if (!StatisticsSetting.GetInstance().DictAllStats.ContainsKey(info.ReferenceId.ToString()))
                                                    StatisticsSetting.GetInstance().DictAllStats.Add(info.ReferenceId.ToString(), item.TempStatName);

                                                if (!StatisticsSetting.GetInstance().dnGroupStatisticsValueHolder.ContainsKey(info.ReferenceId))
                                                {
                                                    StatisticsSetting.GetInstance().dnGroupStatisticsValueHolder.Add(info.ReferenceId, statVariables);
                                                }

                                                StatisticsSetting.GetInstance().FinalDNGroupsPackageID.Add(info.ReferenceId);
                                                break;
                                            case EventError.MessageId:
                                                EventError eventError = (EventError)response;
                                                logger.Error("DN Subscriber : OnNext Method: " + eventError.StringValue);
                                                break;
                                        }
                                        statVariables = null;
                                    }
                                    tempRefid++;
                                }
                            }
                        }
                    }
                    StatisticsSetting.GetInstance().ReferenceId = tempRefid;
                }

                else if (collection is List<IVirtualQueueStatistics>)
                {
                    int tempRefid = StatisticsSetting.GetInstance().ReferenceId + 50;
                    var collVq = collection as List<IVirtualQueueStatistics>;
                    foreach (var item in collVq)
                    {

                        string[] configuredstat;
                        if (objectType == "acd")
                        {
                            configuredstat = value.StatisticsCommon.ACDStatistics.Split(',');
                        }
                        else if (objectType == "dn")
                        {
                            configuredstat = value.StatisticsCommon.DNGroupStatistics.Split(',');
                        }
                        else
                        {
                            configuredstat = value.StatisticsCommon.VQueueStatistics.Split(',');
                        }

                        foreach (string configstat in configuredstat)
                        {
                            if (configstat == item.TempStatName)
                            {
                                for (int objIndex = 0; objIndex < objectCollection.Count; objIndex++)
                                {
                                    string Dilimitor = "_@";
                                    string statName = item.StatisticsName.ToString();                                    
                                    string filter = item.FilterName.ToString();
                                    string serverName = item.ServerName.ToString();
                                    string format = item.StatisticsFormat.ToString();
                                    string queuename = string.Empty;

                                    IMessage response = null;
                                    if (objectType == "acd" || objectType == "vq")
                                    {
                                        string[] Queue_switch = objectCollection[objIndex].Split(new[] { Dilimitor }, StringSplitOptions.None);

                                        if (StatisticsBase.GetInstance().QueueDisplayName == StatisticsEnum.ACDDisplayName.Skill.ToString())
                                        {
                                            if (Queue_switch.Length > 2)
                                            {
                                                queuename = Queue_switch[0] + "@" + Queue_switch[1];
                                            }
                                        }
                                        else if (StatisticsBase.GetInstance().QueueDisplayName == StatisticsEnum.ACDDisplayName.Queue.ToString() || StatisticsBase.GetInstance().QueueDisplayName == StatisticsEnum.ACDDisplayName.VirtualQueue.ToString() || StatisticsBase.GetInstance().QueueDisplayName == "")
                                        {
                                            if (Queue_switch.Length > 2)
                                            {
                                                queuename = Queue_switch[0] + "@" + Queue_switch[2];
                                            }
                                            else if (Queue_switch.Length == 2)
                                            {
                                                queuename = Queue_switch[0] + "@" + Queue_switch[1];
                                            }
                                        }
                                        else
                                        {
                                            if (Queue_switch.Length == 2)
                                            {
                                                queuename = Queue_switch[0] + "@" + Queue_switch[1];
                                            }

                                        }

                                        logger.Info("VQ Subscriber : OnNext Method : Request : ReferenceId - " + tempRefid);
                                        logger.Info("VQ Subscriber : OnNext Method : Request : StatName - " + statName);
                                        logger.Info("VQ Subscriber : OnNext Method : Request : QueueName - " + queuename);
                                        logger.Info("VQ Subscriber : OnNext Method : Request : ServerName - " + serverName);

                                        if (!string.IsNullOrEmpty(queuename))
                                        {
                                            if (tempRefid < StatisticsSetting.GetInstance().BAttributeReferenceId)
                                            {
                                                //MessageBox.Show("vqstatrequest=" + queuename);
                                                response = statreq.StatRequest(value.StatisticsCommon.TenantName, queuename,
                                                StatisticObjectType.Queue, statName, value.StatisticsCommon.NotifySeconds,
                                                 filter, value.StatisticsCommon.Insensitivity, tempRefid, serverName);

                                                if (!StatisticsSetting.GetInstance().ListRequestIds.Contains(tempRefid))
                                                    StatisticsSetting.GetInstance().ListRequestIds.Add(tempRefid);
                                            }
                                            else
                                            {
                                                //MessageBox.Show("vqstatrequest=" + queuename);
                                                response = statreq.StatRequest(value.StatisticsCommon.TenantName, queuename,
                                                StatisticObjectType.Queue, value.StatisticsCommon.NotifySeconds,
                                                 filter, configstat, tempRefid, serverName);

                                                if (!StatisticsSetting.GetInstance().ListRequestIds.Contains(tempRefid))
                                                    StatisticsSetting.GetInstance().ListRequestIds.Add(tempRefid);
                                            }
                                        }
                                        else
                                        {

                                            if (objectType == "vq")
                                                logger.Warn("DN Subscriber : OnNext Method : Request : VQ is Null");
                                            else
                                                logger.Warn("DN Subscriber : OnNext Method : Request : ACD is Null");
                                        }
                                    }
                                    else if (objectType == "dn")
                                    {
                                        queuename = objectCollection[objIndex];

                                        logger.Info("VQ Subscriber : OnNext Method : Request : ReferenceId - " + tempRefid);
                                        logger.Info("VQ Subscriber : OnNext Method : Request : StatName - " + statName);
                                        logger.Info("VQ Subscriber : OnNext Method : Request : QueueName - " + queuename);
                                        logger.Info("VQ Subscriber : OnNext Method : Request : ServerName - " + serverName);
                                        if (tempRefid < StatisticsSetting.GetInstance().BAttributeReferenceId)
                                        {
                                            //MessageBox.Show("vqstatrequest=" + queuename);
                                            response = statreq.StatRequest(value.StatisticsCommon.TenantName, queuename,
                                            StatisticObjectType.GroupQueues, statName, value.StatisticsCommon.NotifySeconds,
                                             filter, value.StatisticsCommon.Insensitivity, tempRefid, serverName);

                                            if (!StatisticsSetting.GetInstance().ListRequestIds.Contains(tempRefid))
                                                StatisticsSetting.GetInstance().ListRequestIds.Add(tempRefid);
                                        }
                                        else
                                        {
                                            //MessageBox.Show("vqstatrequest=" + queuename);
                                            response = statreq.StatRequest(value.StatisticsCommon.TenantName, queuename,
                                            StatisticObjectType.GroupQueues, value.StatisticsCommon.NotifySeconds,
                                             filter, configstat, tempRefid, serverName);

                                            if (!StatisticsSetting.GetInstance().ListRequestIds.Contains(tempRefid))
                                                StatisticsSetting.GetInstance().ListRequestIds.Add(tempRefid);
                                        }

                                    }

                                    logger.Info("VQ Subscriber : OnNext Method : Response : " + response.ToString());

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
                                                statVariables.Tooltip = item.ToolTipName;
                                                statVariables.ObjectType = item.TempStatName;
                                                statVariables.ReferenceId = info.ReferenceId.ToString();
                                                string[] tempQueueName = objectCollection[objIndex].Split(new[] { Dilimitor }, StringSplitOptions.None);
                                                statVariables.DisplayName = item.DisplayName;
                                                if (StatisticsBase.GetInstance().QueueDisplayName == StatisticsEnum.ACDDisplayName.Queue.ToString() || StatisticsBase.GetInstance().QueueDisplayName == StatisticsEnum.ACDDisplayName.VirtualQueue.ToString())
                                                {
                                                    statVariables.ObjectID = tempQueueName[0].ToString();
                                                    statVariables.CCStat = item.DisplayName + " " + tempQueueName[0].ToString();
                                                }
                                                else if (StatisticsBase.GetInstance().QueueDisplayName == StatisticsEnum.ACDDisplayName.Skill.ToString())
                                                {
                                                    statVariables.ObjectID = tempQueueName[1].ToString();
                                                    statVariables.CCStat = item.DisplayName + " " + tempQueueName[1].ToString();
                                                }
                                                else
                                                {
                                                    statVariables.ObjectID = tempQueueName[0].ToString();
                                                    statVariables.CCStat = item.DisplayName + " " + tempQueueName[0].ToString();
                                                }
                                                //statVariables.ObjectID = tempQueueName[0].ToString();
                                                tempThresholdValues = new List<string>();
                                                tempThresholdValues.Add(item.ThresholdLevelOne);
                                                tempThresholdValues.Add(item.ThresholdLevelTwo);

                                                tempThresholdColors = new List<Color>();
                                                tempThresholdColors.Add(item.StatColor);
                                                tempThresholdColors.Add(item.ThresholdColorOne);
                                                tempThresholdColors.Add(item.ThresholdColorTwo);

                                                StatisticsBase.GetInstance().IsCCStats = true;

                                                if (!StatisticsSetting.GetInstance().ThresholdValues.ContainsKey(info.ReferenceId.ToString()))
                                                    StatisticsSetting.GetInstance().ThresholdValues.Add(info.ReferenceId.ToString(), tempThresholdValues);

                                                if (!StatisticsSetting.GetInstance().ThresholdColors.ContainsKey(info.ReferenceId.ToString()))
                                                    StatisticsSetting.GetInstance().ThresholdColors.Add(info.ReferenceId.ToString(), tempThresholdColors);

                                                if (!StatisticsSetting.GetInstance().DictAllStats.ContainsKey(info.ReferenceId.ToString()))
                                                    StatisticsSetting.GetInstance().DictAllStats.Add(info.ReferenceId.ToString(), item.TempStatName);

                                                if (!StatisticsSetting.GetInstance().VQStatisticsValueHolder.ContainsKey(info.ReferenceId))
                                                {
                                                    StatisticsSetting.GetInstance().VQStatisticsValueHolder.Add(info.ReferenceId, statVariables);
                                                }

                                                StatisticsSetting.GetInstance().FinalVQPackageID.Add(info.ReferenceId);
                                                break;
                                            case EventError.MessageId:
                                                EventError eventError = (EventError)response;
                                                logger.Error("VQ Subscriber : OnNext Method: " + eventError.StringValue);
                                                break;
                                        }
                                        statVariables = null;
                                    }
                                    tempRefid++;
                                }
                            }
                        }
                    }

                    StatisticsSetting.GetInstance().ReferenceId = tempRefid;
                }

            }
            catch (Exception GeneralException)
            {

            }
        }

        #endregion Methods

        #region Other

        //StatisticMetric statMetric;

        #endregion Other
    }
}