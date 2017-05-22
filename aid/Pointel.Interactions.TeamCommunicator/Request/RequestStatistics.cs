using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.Queries;
using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.CfgObjects;
using System.Collections;
using Pointel.Configuration.Manager;
using Genesyslab.Platform.Commons.Collections;
using Genesyslab.Platform.Reporting.Protocols.StatServer;
using Genesyslab.Platform.Reporting.Protocols.StatServer.Requests;
using Genesyslab.Platform.Commons.Protocols;
using Genesyslab.Platform.Reporting.Protocols;
using Pointel.Connection.Manager;
using Genesyslab.Platform.Reporting.Protocols.StatServer.Events;

namespace Pointel.Interactions.TeamCommunicator.Request
{
    class RequestStatistics
    {
        #region Private read only members
        private static Pointel.Logger.Core.ILog _logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
                    "AID");
        #endregion

        public static void RequestTeamCommunicatorStatistics(string businessAttributeName, string dynamicStatName, string[] objectIds, StatisticObjectType statObjectType, 
                        int seconds, int insensitivity)
        {
            _logger.Info("Request Statistics Entry");
            try
            {
                if (objectIds == null || objectIds.Length <= 0)
                {
                    _logger.Warn("object ids for requesting statistics is null or empty");
                    return;
                }

                System.Collections.Generic.Dictionary<string, string> dictionary = new System.Collections.Generic.Dictionary<string, string>();
                dictionary = GetDynamicStatsConfiguration(businessAttributeName, dynamicStatName);
                if (dictionary == null || dictionary.Count <= 0)
                {
                    _logger.Warn("Dynamic teamcommunicator statistics configuration throws null or empty collection");
                    return;
                }
                string tenantName = string.Empty;
                if (ConfigContainer.Instance().AllKeys.Contains("tenant-name"))
                    tenantName = (string)ConfigContainer.Instance().GetValue("tenant-name");

                int refID = 5000;
                for (int index = 0; index < objectIds.Length; index++)
                {
                    if (string.IsNullOrEmpty(objectIds[index])) continue;

                    var requestStat = RequestOpenStatisticEx.Create();

                    requestStat.StatisticObject = StatisticObject.Create();
                    requestStat.StatisticObject.ObjectId = objectIds[index];
                    requestStat.StatisticObject.ObjectType = statObjectType;
                    if(!string.IsNullOrEmpty(tenantName))
                        requestStat.StatisticObject.TenantName = tenantName;

                    requestStat.Notification = Notification.Create();
                    requestStat.Notification.Mode = NotificationMode.Immediate;
                    requestStat.Notification.Frequency = seconds;

                    DnActionMask mainMask = null;
                    DnActionMask relMask = null;

                    requestStat.StatisticMetricEx = StatisticMetricEx.Create();

                    if (dictionary.ContainsKey("Category"))
                    {
                        var values = Enum.GetValues(typeof(StatisticCategory));
                        foreach (StatisticCategory categoryItem in values)
                        {
                            if (string.Compare(categoryItem.ToString(), dictionary["Category"], true) == 0)
                            {
                                requestStat.StatisticMetricEx.Category = categoryItem;
                            }
                        }
                    }
                    if (dictionary.ContainsKey("MainMask"))
                    {
                        mainMask = ActionsMask.CreateDnActionMask();

                        string[] actions = dictionary["MainMask"].ToString().Split(',');

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

                    if (dictionary.ContainsKey("RelMask"))
                    {
                        relMask = ActionsMask.CreateDnActionMask();

                        string[] actions = dictionary["RelMask"].ToString().Split(',');

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
                    if (dictionary.ContainsKey("Subject"))
                    {
                        if (dictionary["Subject"] != null)
                        {
                            var values = Enum.GetValues(typeof(StatisticSubject));
                            foreach (StatisticSubject subjectItem in values)
                            {
                                if (string.Compare(subjectItem.ToString(), dictionary["Subject"], true) == 0)
                                {
                                    requestStat.StatisticMetricEx.Subject = subjectItem;
                                }
                            }
                        }
                    }
                    if (dictionary.ContainsKey("Filter"))
                    {
                        if (dictionary["Filter"] != null)
                        {
                            requestStat.StatisticMetricEx.Filter = dictionary["Filter"].ToString();
                        }
                        else
                        {
                            requestStat.StatisticMetricEx.Filter = string.Empty;
                        }

                    }

                    requestStat.StatisticMetricEx.IntervalType = StatisticInterval.GrowingWindow;
                    requestStat.StatisticMetricEx.IntervalLength = 0;
                    requestStat.StatisticMetricEx.MainMask = mainMask;
                    requestStat.StatisticMetricEx.RelativeMask = relMask;
                    requestStat.ReferenceId = refID;
                    _logger.Info("Request Team Communicator statistics : " + requestStat.ToString());
                    IMessage response = ((StatServerProtocol)ProtocolManagers.Instance().ProtocolManager[ServerType.Statisticsserver.ToString()]).Request(requestStat);
                    if (response != null)
                    {
                        switch (response.Id)
                        {
                            case EventStatisticOpened.MessageId:
                                EventStatisticOpened info;
                                info = (EventStatisticOpened)response;
                                _logger.Trace("RequestStatistics Method : EventStatisticsOpened : " + info.Name);
                                break;
                            case EventError.MessageId:
                                EventError eventError = (EventError)response;
                                _logger.Trace("RequestStatistics Method : EventError : " + eventError.StringValue);
                                break;
                        }
                    }
                    refID++;
                }

            }
            catch (Exception generalException)
            {
                _logger.Error("Error while requesting statistics : " + ((generalException.InnerException == null) ? generalException.Message : generalException.InnerException.ToString()));
            }
        }

        private static Dictionary<string, string> GetDynamicStatsConfiguration(string businessAttributeName, string dynamicStatName)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            try
            {
                CfgEnumeratorQuery cfgEnumeratorQuery = new CfgEnumeratorQuery();
                cfgEnumeratorQuery.Name = businessAttributeName;
                cfgEnumeratorQuery.TenantDbid = ConfigContainer.Instance().TenantDbId;
                CfgEnumerator dynamicStatBusinessAttribute = ConfigContainer.Instance().ConfServiceObject.RetrieveObject<CfgEnumerator>(cfgEnumeratorQuery);
                if (dynamicStatBusinessAttribute == null) return null;
                CfgEnumeratorValueQuery dynamicStatQuery = new CfgEnumeratorValueQuery();
                dynamicStatQuery.EnumeratorDbid = dynamicStatBusinessAttribute.DBID;
                ICollection<CfgEnumeratorValue> dynamicStats = ConfigContainer.Instance().ConfServiceObject.RetrieveMultipleObjects<CfgEnumeratorValue>(dynamicStatQuery);
                if (dynamicStats == null || dynamicStats.Count <= 0) return null;


                if (string.IsNullOrEmpty(dynamicStatName)) return null;
                CfgEnumeratorValue teamCommunicatorStatistics = dynamicStats.Where(item => item.Name.Equals(dynamicStatName)).FirstOrDefault();
                if (teamCommunicatorStatistics == null || teamCommunicatorStatistics.UserProperties == null || teamCommunicatorStatistics.UserProperties.Count == 0) return null;

                string teamCommunicatorStatisticsName = string.Empty;
                if (ConfigContainer.Instance().AllKeys.Contains("teamcommunicator.agent.statistics.name") &&
                            !string.IsNullOrEmpty((string)ConfigContainer.Instance().GetValue("teamcommunicator.agent.statistics.name")))
                    teamCommunicatorStatisticsName = (string)ConfigContainer.Instance().GetValue("teamcommunicator.agent.statistics.name");

                if (!teamCommunicatorStatistics.UserProperties.AllKeys.Contains(teamCommunicatorStatisticsName)) return null;
                KeyValueCollection kvColl = (KeyValueCollection)teamCommunicatorStatistics.UserProperties[teamCommunicatorStatisticsName];

                if (kvColl == null || kvColl.Count <= 0) return null;
                dictionary.Clear();
                if (kvColl.ContainsKey("Filter"))
                    dictionary.Add("Filter", kvColl["Filter"].ToString());
                if (kvColl.ContainsKey("Category"))
                    dictionary.Add("Category", kvColl["Category"].ToString());
                if (kvColl.ContainsKey("MainMask"))
                    dictionary.Add("MainMask", kvColl["MainMask"].ToString());
                if (kvColl.ContainsKey("RelMask"))
                    dictionary.Add("RelMask", kvColl["RelMask"].ToString());
                if (kvColl.ContainsKey("Subject"))
                    dictionary.Add("Subject", kvColl["Subject"].ToString());
            }
            catch (Exception generalException)
            {
                _logger.Error(((generalException.InnerException == null) ? generalException.Message.ToString() : generalException.InnerException.ToString()));
            }
            return dictionary;
        }

        public static void CloseStatistics(int referenceId)
        {
            try
            {
                RequestCloseStatistic closeStatistics = RequestCloseStatistic.Create(referenceId);
                if (referenceId >= 5000)
                {
                    IMessage response = ((StatServerProtocol)ProtocolManagers.Instance().ProtocolManager[ServerType.Statisticsserver.ToString()]).Request(closeStatistics);

                    if (response != null)
                    {
                        switch (response.Id)
                        {
                            case EventStatisticOpened.MessageId:
                                EventStatisticOpened info = (EventStatisticOpened)response;
                                _logger.Trace("RequestStatistics Method : EventStatisticsOpened : " + info.Name);
                                break;
                            case EventError.MessageId:
                                EventError eventError = (EventError)response;
                                _logger.Trace("RequestStatistics Method : EventError : " + eventError.StringValue);
                                break;
                            case EventStatisticClosed.MessageId:
                                EventStatisticClosed statisticClosed = (EventStatisticClosed)response;
                                _logger.Trace("StatisticClosed : EventStatisticClosed : " + statisticClosed.ReferenceId);
                                break;
                        }
                    }
                }
            }
            catch (Exception generalException)
            {
                _logger.Error("Error occurred while closing the statistics with Ref Id : " + referenceId + " " + 
                        (generalException.InnerException == null ? generalException.Message : generalException.InnerException.ToString()));
            }
        }

    }
}
