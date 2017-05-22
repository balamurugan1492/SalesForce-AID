namespace Pointel.Statistics.Core.StatisticsRequest
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Genesyslab.Platform.Commons.Collections;
    using Genesyslab.Platform.Commons.Protocols;
    using Genesyslab.Platform.Reporting.Protocols.StatServer;
    using Genesyslab.Platform.Reporting.Protocols.StatServer.Events;
    using Genesyslab.Platform.Reporting.Protocols.StatServer.Requests;

    using Pointel.Logger.Core;
    using Pointel.Statistics.Core.ConnectionManager;
    using Pointel.Statistics.Core.Utility;

    internal class Request
    {
        #region Fields

        private static ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "STF");

        KeyValueCollection insValue = new KeyValueCollection();
        Notification notification;
        StatisticMetric statMetric;
        StatisticObject statObject;
        StatisticsSetting StatSettings = StatisticsSetting.GetInstance();

        #endregion Fields

        #region Methods

        public void CancelRequest(int refId)
        {
            try
            {
                RequestCloseStatistic closeStat = RequestCloseStatistic.Create(refId);

                StatSettings.statServerProtocol.Request(closeStat);
            }
            catch (Exception generalExcepiton)
            {

            }
        }

        /// <summary>
        /// Stats the request.
        /// </summary>
        /// <param name="TenantName">Name of the tenant.</param>
        /// <param name="ObjectID">The object ID.</param>
        /// <param name="statObjectType">Type of the stat object.</param>
        /// <param name="statTypeName">Name of the stat type.</param>
        /// <param name="Seconds">The seconds.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="insensitivity">The insensitivity.</param>
        /// <param name="refID">The reference ID.</param>
        /// <returns></returns>
        public IMessage StatRequest(string TenantName, string ObjectID, StatisticObjectType statObjectType, string statTypeName,
            int Seconds, string filter, int insensitivity, int refID,string serverName)
        {
            IMessage message = null;
            string response = string.Empty;
            try
            {
                logger.Debug("Request : StatRequest Method : Entry");
                statObject = StatisticObject.Create(ObjectID, statObjectType, TenantName);
                statMetric = StatisticMetric.Create(statTypeName);
                statMetric.Filter = filter;
                notification = Notification.Create();
                notification.Mode = NotificationMode.Periodical;
                //notification.Mode = NotificationMode.Immediate ;
                notification.Frequency = Seconds;
                insValue.Add("Insens", insensitivity);
                RequestOpenStatistic requestStat;
                requestStat = RequestOpenStatistic.Create(statObject, statMetric, notification, insValue, refID);
                requestStat.ReferenceId = refID;
                if(string.IsNullOrEmpty(serverName))
                {
                    if (StatisticsSetting.GetInstance().statProtocols.Count > 0)
                    {
                        if (StatisticsSetting.GetInstance().statProtocols[0] != null)
                        {
                            if (StatisticsSetting.GetInstance().statProtocols[0].State == ChannelState.Opened || StatisticsSetting.GetInstance().statProtocols[0].State == ChannelState.Opening)
                            {
                                logger.Info("Request : StatRequest Method :" + requestStat.ToString());
                                message = StatisticsSetting.GetInstance().statProtocols[0].Request(requestStat);
                                logger.Info("Request : StatRequest Method :" + message.ToString());
                            }
                        }
                    }
                }
                else
                {
                    if (StatisticsSetting.GetInstance().rptProtocolManager[serverName] != null)
                    {
                        if (StatisticsSetting.GetInstance().rptProtocolManager[serverName].State == ChannelState.Opened || StatisticsSetting.GetInstance().rptProtocolManager[serverName].State == ChannelState.Opening)
                        {
                            logger.Info("Request : StatRequest Method :" + requestStat.ToString());
                            message = StatisticsSetting.GetInstance().rptProtocolManager[serverName].Request(requestStat);
                            logger.Info("Request : StatRequest Method :" + message.ToString());
                        }
                    }
                }
                
            }
            catch (ProtocolException Protocolexception)
            {
                logger.Error("Request : StatRequest Method : " + Protocolexception.Message);
            }
            catch (Exception GeneralException)
            {
                logger.Error("Request : StatRequest Method : " + GeneralException.Message);
            }
            finally
            {
                logger.Debug("Request : StatRequest Method : Exit");
                GC.SuppressFinalize(message);
                GC.Collect();
            }
            return message;
        }

        /// <summary>
        /// DynamicStats request.
        /// </summary>
        /// <param name="TenantName">Name of the tenant.</param>
        /// <param name="ObjectID">The object ID.</param>
        /// <param name="statObjectType">Type of the stat object.</param>
        /// <param name="statTypeName">Name of the stat type.</param>
        /// <param name="Seconds">The seconds.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="insensitivity">The insensitivity.</param>
        /// <param name="refID">The reference ID.</param>
        /// <returns></returns>
        public IMessage StatRequest(string TenantName, string ObjectID, StatisticObjectType statObjectType,
            int Seconds, string filter, string sectionName, int RefId,string serverName)
        {
            IMessage message = null;
            try
            {
                logger.Info("Request : StatRequest Method :Entry");

                var requestStat = RequestOpenStatisticEx.Create();

                requestStat.StatisticObject = StatisticObject.Create();
                requestStat.StatisticObject.ObjectId = ObjectID;
                requestStat.StatisticObject.ObjectType = statObjectType;
                requestStat.StatisticObject.TenantName = TenantName;

                requestStat.Notification = Notification.Create();
                requestStat.Notification.Mode = NotificationMode.Periodical;
                requestStat.Notification.Frequency = Seconds;

                DnActionMask mainMask = null;
                DnActionMask relMask = null;

                requestStat.StatisticMetricEx = StatisticMetricEx.Create();

                if (StatisticsSetting.GetInstance().DictStatisticsMetrics.ContainsKey(sectionName))
                {
                    Dictionary<string, string> statMetricDetail = new Dictionary<string, string>();
                    statMetricDetail = StatisticsSetting.GetInstance().DictStatisticsMetrics[sectionName];

                    if (statMetricDetail.Count != 0)
                    {
                        foreach (string key in statMetricDetail.Keys)
                        {
                            if (string.Compare(key, "Category", true) == 0)
                            {
                                if (statMetricDetail[key] != null)
                                {
                                    var values = Enum.GetValues(typeof(StatisticCategory));
                                    foreach (StatisticCategory categoryItem in values)
                                    {
                                        if (string.Compare(categoryItem.ToString(), statMetricDetail[key], true) == 0)
                                        {
                                            requestStat.StatisticMetricEx.Category = categoryItem;
                                        }
                                    }
                                }
                            }
                            else if (string.Compare(key, "MainMask", true) == 0)
                            {
                                mainMask = ActionsMask.CreateDnActionMask();

                                string[] actions = statMetricDetail[key].ToString().Split(',');

                                foreach (string customAction in actions)
                                {
                                    string myAction = string.Empty;
                                    if (customAction.Contains('~'))
                                    {

                                        myAction = customAction.Substring(1, customAction.Length - 1);
                                        var values = Enum.GetValues(typeof(DnActions));
                                        foreach (DnActions action in values)
                                        {
                                            if (string.Compare(action.ToString(), myAction, true) == 0)
                                            {
                                                mainMask.ClearBit(action);
                                            }
                                        }
                                    }
                                    else if (customAction.Contains('*'))
                                    {
                                        var values = Enum.GetValues(typeof(DnActions));
                                        foreach (DnActions action in values)
                                        {
                                            mainMask.SetBit(action);
                                        }
                                    }
                                    else
                                    {
                                        var values = Enum.GetValues(typeof(DnActions));
                                        foreach (DnActions action in values)
                                        {
                                            if (string.Compare(action.ToString(), customAction.Trim().ToString(), true) == 0)
                                            {
                                                mainMask.SetBit(action);
                                            }
                                        }
                                    }
                                }
                            }
                            else if (string.Compare(key, "RelMask", true) == 0)
                            {
                                relMask = ActionsMask.CreateDnActionMask();

                                if (statMetricDetail[key] != null)
                                {
                                    string[] actions = statMetricDetail[key].ToString().Split(',');

                                    foreach (string customAction in actions)
                                    {
                                        string myAction = string.Empty;
                                        if (customAction.Contains('~'))
                                        {
                                            myAction = customAction.Substring(1, customAction.Length - 1);
                                            var values = Enum.GetValues(typeof(DnActions));
                                            foreach (DnActions action in values)
                                            {
                                                if (string.Compare(action.ToString(), myAction, true) == 0)
                                                {
                                                    mainMask.ClearBit(action);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            var values = Enum.GetValues(typeof(DnActions));
                                            foreach (DnActions action in values)
                                            {
                                                if (string.Compare(action.ToString(), customAction.Trim().ToString(), true) == 0)
                                                {
                                                    relMask.SetBit(action);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            else if (string.Compare(key, "Subject", true) == 0)
                            {
                                if (statMetricDetail[key] != null)
                                {
                                    var values = Enum.GetValues(typeof(StatisticSubject));
                                    foreach (StatisticSubject subjectItem in values)
                                    {
                                        if (string.Compare(subjectItem.ToString(), statMetricDetail[key], true) == 0)
                                        {
                                            requestStat.StatisticMetricEx.Subject = subjectItem;
                                        }
                                    }
                                }

                            }
                        }
                    }
                }
                requestStat.StatisticMetricEx.IntervalType = StatisticInterval.GrowingWindow;
                requestStat.StatisticMetricEx.IntervalLength = 0;
                requestStat.StatisticMetricEx.MainMask = mainMask;
                requestStat.StatisticMetricEx.RelativeMask = relMask;
                requestStat.StatisticMetricEx.Filter = filter;
                requestStat.ReferenceId = RefId;
                if (string.IsNullOrEmpty(serverName))
                {
                    if (StatisticsSetting.GetInstance().statProtocols.Count>0)
                    {
                        if (StatisticsSetting.GetInstance().statProtocols[0] != null)
                        {
                            if (StatisticsSetting.GetInstance().statProtocols[0].State == ChannelState.Opened || StatisticsSetting.GetInstance().statProtocols[0].State == ChannelState.Opening)
                            {
                                logger.Info("Request : StatRequest Method :" + requestStat.ToString());
                                message = StatisticsSetting.GetInstance().statProtocols[0].Request(requestStat);
                                logger.Info("Request : StatRequest Method :" + message.ToString());
                            }
                        }
                    }                   
                }
                else
                {
                    if (StatisticsSetting.GetInstance().rptProtocolManager[serverName] != null)
                    {
                        if (StatisticsSetting.GetInstance().rptProtocolManager[serverName].State == ChannelState.Opened)
                        {
                            logger.Info("Request : StatRequest Method :" + requestStat.ToString());
                            message = StatisticsSetting.GetInstance().rptProtocolManager[serverName].Request(requestStat);
                            logger.Info("Request : StatRequest Method :" + message.ToString());
                        }
                    }
                }
               

            }
            catch (ProtocolException Protocolexception)
            {
                logger.Error("Request : StatRequest Method : " + Protocolexception.Message);
            }
            catch (Exception GeneralException)
            {
                logger.Error("Request : StatRequest Method : " + GeneralException.Message);
            }
            finally
            {
                logger.Debug("Request : StatRequest Method : Exit");
                GC.SuppressFinalize(message);
                GC.Collect();
            }
            return message;
        }

        /// <summary>
        /// DynamicStats request.
        /// </summary>
        /// <param name="TenantName">Name of the tenant.</param>
        /// <param name="ObjectID">The object ID.</param>
        /// <param name="statObjectType">Type of the stat object.</param>
        /// <param name="statTypeName">Name of the stat type.</param>
        /// <param name="Seconds">The seconds.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="insensitivity">The insensitivity.</param>
        /// <param name="refID">The reference ID.</param>
        /// <returns></returns>
        public IMessage StatRequest(RequestOpenStatisticEx requestStat)
        {
            IMessage message = null;
            try
            {
                logger.Info("Request : StatRequest Method :Entry");

                if (StatisticsSetting.GetInstance().statServerProtocol != null)
                {
                    if (StatSettings.statServerProtocol.State == ChannelState.Opened)
                    {
                        logger.Info("Request : StatRequest Method :" + requestStat.ToString());
                        message = StatSettings.statServerProtocol.Request(requestStat);
                        logger.Info("Request : StatRequest Method :" + message.ToString());
                    }
                }

            }
            catch (ProtocolException Protocolexception)
            {
                logger.Error("Request : StatRequest Method : " + Protocolexception.Message);
            }
            catch (Exception GeneralException)
            {
                logger.Error("Request : StatRequest Method : " + GeneralException.Message);
            }
            finally
            {
                logger.Debug("Request : StatRequest Method : Exit");
                GC.SuppressFinalize(message);
                GC.Collect();
            }
            return message;
        }


        //Team Communicator Request
        public IMessage StatRequest(RequestOpenStatisticEx requestStat,string serverName)
        {
            IMessage message = null;
            try
            {
                logger.Info("Request : StatRequest Method :Entry");
                if (string.IsNullOrEmpty(serverName.Trim()))
                {
                    if (StatisticsSetting.GetInstance().statProtocols.Count > 0)
                    {
                        if (StatisticsSetting.GetInstance().statProtocols[0] != null)
                        {
                            if (StatisticsSetting.GetInstance().statProtocols[0].State == ChannelState.Opened || StatisticsSetting.GetInstance().statProtocols[0].State == ChannelState.Opening)
                            {
                                logger.Info("Request : StatRequest Method :" + requestStat.ToString());
                                message = StatisticsSetting.GetInstance().statProtocols[0].Request(requestStat);
                                logger.Info("Request : StatRequest Method :" + message.ToString());
                            }
                        }
                    }
                }
                else
                {
                    if (StatisticsSetting.GetInstance().rptProtocolManager[serverName] != null)
                    {
                        if (StatisticsSetting.GetInstance().rptProtocolManager[serverName].State == ChannelState.Opened || StatisticsSetting.GetInstance().rptProtocolManager[serverName].State == ChannelState.Opening)
                        {
                            logger.Info("Request : StatRequest Method :" + requestStat.ToString());
                            message = StatisticsSetting.GetInstance().rptProtocolManager[serverName].Request(requestStat);
                            logger.Info("Request : StatRequest Method :" + message.ToString());
                        }
                    }
                }
                
            }

            catch (ProtocolException Protocolexception)
            {
                logger.Error("Request : StatRequest Method : " + Protocolexception.Message);
            }
            catch (Exception GeneralException)
            {
                logger.Error("Request : StatRequest Method : " + GeneralException.Message);
            }
            finally
            {
                logger.Debug("Request : StatRequest Method : Exit");
                GC.SuppressFinalize(message);
                GC.Collect();
            }
            return message;
        }
        #endregion Methods
    }
}