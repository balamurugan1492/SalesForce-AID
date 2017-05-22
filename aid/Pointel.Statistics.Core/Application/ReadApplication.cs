namespace Pointel.Statistics.Core.Application
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Security.AccessControl;
    using System.Security.Principal;
    using System.Text;
    using System.Windows;
    using System.Windows.Forms;
    using System.Xml;
    using System.Xml.XPath;

    using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel;
    using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.CfgObjects;
    using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.Queries;
    using Genesyslab.Platform.Commons.Collections;
    using Genesyslab.Platform.Configuration.Protocols.Types;
    using Genesyslab.Platform.Reporting.Protocols.StatServer;

    using Pointel.Logger.Core;
    using Pointel.Statistics.Core.ConnectionManager;
    using Pointel.Statistics.Core.StatisticsProvider;
    using Pointel.Statistics.Core.Utility;

    /// <summary>
    /// This class contains to read the application details from the CME
    /// </summary>
    internal class ReadApplication
    {
        #region Fields

        public int statorder = 1;

        private static ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "STF");

        StatisticsInformation statInfo = new StatisticsInformation();

        #endregion Fields

        #region Methods

        /// <summary>
        /// Reads the agent.
        /// </summary>
        /// <param name="uniqueId">Name of the person.</param>
        public void ReadAgent(string uniqueId, bool isEmpId)
        {
            try
            {
                logger.Debug("ReadApplication : ReadAgentValues Method: Entry");

                CfgPersonQuery _personQuery = new CfgPersonQuery();
                if (!isEmpId)
                    _personQuery.UserName = uniqueId;
                else
                    _personQuery.EmployeeId = uniqueId;
                _personQuery.TenantDbid = StatisticsSetting.GetInstance().CFGTenantDBID;

                StatisticsSetting.GetInstance().PersonDetails = StatisticsSetting.GetInstance().confObject.RetrieveObject<CfgPerson>(_personQuery);

                if (StatisticsSetting.GetInstance().PersonDetails != null)
                {
                    if (StatisticsSetting.GetInstance().PersonDetails.DBID != 0)
                    {
                        StatisticsSetting.GetInstance().agentDBID = StatisticsSetting.GetInstance().PersonDetails.DBID.ToString();
                    }
                    else
                    {
                        logger.Info("ReadApplication : ReadAgentValues Method: Person.DBID is 0 for " + uniqueId);
                    }
                }
                else
                {
                    logger.Info("ReadApplication : ReadAgentValues Method: Person " + uniqueId + ", is not found");
                }
            }
            catch (Exception GeneralException)
            {

            }

            logger.Debug("ReadApplication : ReadAgentValues Method: Exit");
        }

        public Dictionary<string, string> ReadAgentConfiguration()
        {
            //MessageBox.Show("as");
            Dictionary<string, string> output = new Dictionary<string, string>();
            StreamReader readUserDetails = null;
            try
            {
                logger.Debug("XMLStorage : ReadAgentConfiguration : Method Entry");

                if (Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
                                      "\\Pointel\\AgentInteractionDesktop"))
                {
                    if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
                                      "\\Pointel\\AgentInteractionDesktop\\" + StatisticsSetting.GetInstance().logUserName + ".stat.config"))
                    {
                        string user = Environment.UserDomainName + "\\" + Environment.UserName;
                        FileSecurity accessControlList = File.GetAccessControl(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
                                      "\\Pointel\\AgentInteractionDesktop\\" + StatisticsSetting.GetInstance().logUserName + ".stat.config");
                        bool isReadAccess = true;
                        if (accessControlList != null)
                        {
                            AuthorizationRuleCollection tempaccess = accessControlList.GetAccessRules(true, true, typeof(System.Security.Principal.NTAccount));
                            if (tempaccess != null)
                            {
                                foreach (FileSystemAccessRule rule in tempaccess)
                                {
                                    NTAccount ntAccount = rule.IdentityReference as NTAccount;
                                    if (ntAccount.Value.Equals(user))
                                    {
                                        if (rule.AccessControlType == AccessControlType.Deny)
                                        {
                                            if ((FileSystemRights.Read & rule.FileSystemRights) == FileSystemRights.Read)
                                            {
                                                isReadAccess = false;
                                                logger.Warn("Read Permission is denied to access the agent configuration file from app Data");
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        if (isReadAccess)
                        {
                            readUserDetails = new StreamReader(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
                                         "\\Pointel\\AgentInteractionDesktop\\" + StatisticsSetting.GetInstance().logUserName + ".stat.config");
                            logger.Debug("The config file path is  : " + Environment.SpecialFolder.ApplicationData +
                                         "\\Pointel\\AgentInteractionDesktop\\" + StatisticsSetting.GetInstance().logUserName + ".stat.config");
                        }
                        else
                        {
                            if (File.Exists(Environment.CurrentDirectory + "\\" + StatisticsSetting.GetInstance().logUserName + ".stat.config"))
                            {
                                readUserDetails = new StreamReader(Environment.CurrentDirectory + "\\" + StatisticsSetting.GetInstance().logUserName + ".stat.config");
                                logger.Debug("The config file path is  : " + Environment.CurrentDirectory + "\\" + StatisticsSetting.GetInstance().logUserName + ".stat.config");
                            }
                            else
                                logger.Warn("No agent config file exist in the installation location");

                        }

                    }
                    else
                    {
                        logger.Warn("No Agent Configuration File Exist in the AppData");
                        if (File.Exists(Environment.CurrentDirectory + "\\" + StatisticsSetting.GetInstance().logUserName + ".stat.config"))
                        {
                            readUserDetails = new StreamReader(Environment.CurrentDirectory + "\\" + StatisticsSetting.GetInstance().logUserName + ".stat.config");
                            logger.Debug("The config file path is  : " + Environment.CurrentDirectory + "\\" + StatisticsSetting.GetInstance().logUserName + ".stat.config");
                        }
                        else
                            logger.Warn("No agent config file exist in the installation location");

                    }
                }
                else
                {
                    logger.Warn("No Folder Exist in the AppData");
                    if (File.Exists(Environment.CurrentDirectory + "\\" + StatisticsSetting.GetInstance().logUserName + ".stat.config"))
                    {
                        readUserDetails = new StreamReader(Environment.CurrentDirectory + "\\" + StatisticsSetting.GetInstance().logUserName + ".stat.config");
                        logger.Debug("The config file path is  : " + Environment.CurrentDirectory + "\\" + StatisticsSetting.GetInstance().logUserName + ".stat.config");
                    }
                    else
                        logger.Warn("No agent config file exist in the installation location");
                }

            }
            catch (Exception ex)
            {
                logger.Error("XML Storage Class : ReadAgentConfiguration Method : Exception caught : " + ex.Message);
            }
            finally
            {
                GC.Collect();
            }
            XmlDocument configXml = new XmlDocument();
            if (readUserDetails != null)
            {
                try
                {
                    configXml.LoadXml(readUserDetails.ReadToEnd());
                }
                catch
                {
                    readUserDetails.Dispose();
                }
                finally
                {
                    GC.Collect();
                }

                readUserDetails.Dispose();

                XPathNavigator navigator = configXml.CreateNavigator();

                XPathNodeIterator nodeIterator = (XPathNodeIterator)navigator.Evaluate("AgentConfig/StatTickerFive/agent-tagged-statistics");

                if (nodeIterator != null && nodeIterator.Count != 0)
                {
                    nodeIterator.MoveNext();
                    output.Add("agent-tagged-statistics", nodeIterator.Current.ToString().Trim());
                }

                nodeIterator = (XPathNodeIterator)navigator.Evaluate("AgentConfig/StatTickerFive/statistics.gadget-position");
                if (nodeIterator != null && nodeIterator.Count != 0)
                {
                    nodeIterator.MoveNext();
                    output.Add("statistics.gadget-position", nodeIterator.Current.ToString().Trim());
                }

                nodeIterator = (XPathNodeIterator)navigator.Evaluate("AgentConfig/StatTickerFive/statistics.enable-header");
                if (nodeIterator != null && nodeIterator.Count != 0)
                {
                    nodeIterator.MoveNext();
                    output.Add("statistics.enable-header", nodeIterator.Current.ToString().Trim());
                }
                nodeIterator = (XPathNodeIterator)navigator.Evaluate("AgentConfig/StatTickerFive/statistics.enable-tag-vertical");
                if (nodeIterator != null && nodeIterator.Count != 0)
                {
                    nodeIterator.MoveNext();
                    output.Add("statistics.enable-tag-vertical", nodeIterator.Current.ToString().Trim());
                }
                nodeIterator = (XPathNodeIterator)navigator.Evaluate("AgentConfig/StatTickerFive/agent-statistics");
                if (nodeIterator != null && nodeIterator.Count != 0)
                {
                    nodeIterator.MoveNext();
                    output.Add("agent-statistics", nodeIterator.Current.ToString().Trim());
                }
                nodeIterator = (XPathNodeIterator)navigator.Evaluate("AgentConfig/StatTickerFive/agent-group-statistics");
                if (nodeIterator != null && nodeIterator.Count != 0)
                {
                    nodeIterator.MoveNext();
                    output.Add("agent-group-statistics", nodeIterator.Current.ToString().Trim());
                }
                nodeIterator = (XPathNodeIterator)navigator.Evaluate("AgentConfig/StatTickerFive/acd-queue-statistics");
                if (nodeIterator != null && nodeIterator.Count != 0)
                {
                    nodeIterator.MoveNext();
                    output.Add("acd-queue-statistics", nodeIterator.Current.ToString().Trim());
                }
                nodeIterator = (XPathNodeIterator)navigator.Evaluate("AgentConfig/StatTickerFive/dn-group-statistics");
                if (nodeIterator != null && nodeIterator.Count != 0)
                {
                    nodeIterator.MoveNext();
                    output.Add("dn-group-statistics", nodeIterator.Current.ToString().Trim());
                }
                nodeIterator = (XPathNodeIterator)navigator.Evaluate("AgentConfig/StatTickerFive/virtual-queue-statistics");
                if (nodeIterator != null && nodeIterator.Count != 0)
                {
                    nodeIterator.MoveNext();
                    output.Add("virtual-queue-statistics", nodeIterator.Current.ToString().Trim());
                }
                nodeIterator = (XPathNodeIterator)navigator.Evaluate("AgentConfig/StatTickerFive/statistics.objects-acd-queues");
                if (nodeIterator != null && nodeIterator.Count != 0)
                {
                    nodeIterator.MoveNext();
                    output.Add("statistics.objects-acd-queues", nodeIterator.Current.ToString().Trim());
                }
                nodeIterator = (XPathNodeIterator)navigator.Evaluate("AgentConfig/StatTickerFive/statistics.objects-dn-groups");
                if (nodeIterator != null && nodeIterator.Count != 0)
                {
                    nodeIterator.MoveNext();
                    output.Add("statistics.objects-dn-groups", nodeIterator.Current.ToString().Trim());
                }
                nodeIterator = (XPathNodeIterator)navigator.Evaluate("AgentConfig/StatTickerFive/statistics.objects-virtual-queues");
                if (nodeIterator != null && nodeIterator.Count != 0)
                {
                    nodeIterator.MoveNext();
                    output.Add("statistics.objects-virtual-queues", nodeIterator.Current.ToString().Trim());
                }

                navigator = null;
                nodeIterator = null;
            }
            logger.Debug("XMLStorage : ReadAgentConfiguration : Method Exit");
            return output;
        }

        /// <summary>
        /// Gets the person DBID.
        /// </summary>
        /// <param name="statisticsCollection">The statistics collection.</param>
        /// <param name="personName">Name of the person.</param>
        /// <returns></returns>
        public IStatisticsCollection ReadAgentDetails(IStatisticsCollection statisticsCollection, string personName)
        {
            if (StatisticsSetting.GetInstance().isAgentConfigFromLocal)
            {
                int kvpid = 1;
                try
                {
                    logger.Debug("ReadApplication : GetPersonDBID Method: Entry");

                    if (StatisticsSetting.GetInstance().PersonDetails != null)
                    {
                        if (StatisticsSetting.GetInstance().PersonDetails.EmployeeID != null)
                        {
                            StatisticsSetting.GetInstance().AgentEmpId = StatisticsSetting.GetInstance().PersonDetails.EmployeeID.ToString();
                        }

                        Dictionary<string, string> serverDetails = StatisticsSetting.GetInstance().agentConfigColl;

                        if (serverDetails != null)
                        {

                            foreach (string key in serverDetails.Keys)
                            {
                                #region Agent Statistics Configuration

                                if (key.StartsWith("agent-group-statistics"))
                                {
                                    statisticsCollection.StatisticsCommon.AgentGroupStatistics = Convert.ToString(serverDetails[key]);
                                }
                                else if (key.StartsWith("agent-statistics"))
                                {
                                    statisticsCollection.StatisticsCommon.AgentStatistics = Convert.ToString(serverDetails[key]);
                                }
                                else if (key.StartsWith("virtual-queue-statistics"))
                                {
                                    statisticsCollection.StatisticsCommon.VQueueStatistics = Convert.ToString(serverDetails[key]);
                                }
                                else if (key.StartsWith("acd-queue-statistics"))
                                {
                                    statisticsCollection.StatisticsCommon.ACDStatistics = Convert.ToString(serverDetails[key]);
                                }
                                else if (key.StartsWith("dn-group-statistics"))
                                {
                                    statisticsCollection.StatisticsCommon.DNGroupStatistics = Convert.ToString(serverDetails[key]);
                                }
                                else if (key.StartsWith("statistics.objects-acd-queues"))
                                {
                                    statisticsCollection.StatisticsCommon.ACDObjects = Convert.ToString(serverDetails[key]);

                                    StatisticsSetting.GetInstance().DictACDDisplays.Clear();
                                    string[] ACDQueueObjects = StatisticsSetting.GetInstance().statisticsCollection.StatisticsCommon.ACDObjects.Split(',');

                                    foreach (string ACDs in ACDQueueObjects)
                                    {
                                        string Dilimitor = "_@";
                                        string[] ACDDisplayObjects = ACDs.Split(new[] { Dilimitor }, StringSplitOptions.None);

                                        if (ACDDisplayObjects.Length > 1)
                                        {
                                            if (!StatisticsSetting.GetInstance().DictACDDisplays.ContainsKey(ACDDisplayObjects[0].ToString()))
                                                StatisticsSetting.GetInstance().DictACDDisplays.Add(ACDDisplayObjects[0].ToString(), ACDDisplayObjects[1].ToString());
                                        }
                                    }
                                }
                                else if (key.StartsWith("statistics.objects-dn-groups"))
                                {
                                    statisticsCollection.StatisticsCommon.DNGroupObjects = Convert.ToString(serverDetails[key]);
                                }
                                else if (key.StartsWith("statistics.objects-virtual-queues"))
                                {
                                    statisticsCollection.StatisticsCommon.VQueueObjects = Convert.ToString(serverDetails[key]);
                                }
                                else if (string.Compare(key, "statistics.display-width", true) == 0)
                                {
                                    StatisticsBase.GetInstance().GadgetWidth = Convert.ToDouble(serverDetails[key]);
                                }
                                else if (string.Compare(key, "statistics.objects-acd-display", true) == 0)
                                {
                                    StatisticsBase.GetInstance().QueueDisplayName = Convert.ToString(serverDetails[key]);
                                }
                                else if (string.Compare(key, "statistics.gadget-position", true) == 0)
                                {
                                    statisticsCollection.StatisticsCommon.Position = Convert.ToString(serverDetails[key]);

                                }
                                else if (string.Compare(key, "statistics.db-color", true) == 0)
                                {
                                    statisticsCollection.StatisticsCommon.DBColor = Color.FromName(serverDetails[key].ToString());
                                }
                                else if (string.Compare(key, "statistics.server-color", true) == 0)
                                {
                                    statisticsCollection.StatisticsCommon.ServerColor = Color.FromName(serverDetails[key].ToString());
                                }
                                else if (string.Compare(key, "statistics.enable-tag-vertical", true) == 0)
                                {
                                    if (serverDetails[key].ToString() != null && serverDetails[key].ToString() != string.Empty && ((string.Compare(serverDetails[key].ToString(), "true", true) == 0) || (string.Compare(serverDetails[key].ToString(), "false", true) == 0)))
                                    {
                                        statisticsCollection.ApplicationContainer.EnableTagVertical = Convert.ToBoolean(serverDetails[key]);
                                        if (!StatisticsSetting.GetInstance().DictEnableDisableChannels.ContainsKey("tagvertical"))
                                            StatisticsSetting.GetInstance().DictEnableDisableChannels.Add("tagvertical", Convert.ToBoolean(serverDetails[key]));
                                        else
                                            StatisticsSetting.GetInstance().DictEnableDisableChannels["tagvertical"] = Convert.ToBoolean(serverDetails[key]);
                                    }
                                }
                                else if (string.Compare(key, "statistics.enable-header", true) == 0)
                                {
                                    if (serverDetails[key].ToString() != null && serverDetails[key].ToString() != string.Empty && ((string.Compare(serverDetails[key].ToString(), "true", true) == 0) || (string.Compare(serverDetails[key].ToString(), "false", true) == 0)))
                                    {
                                        if (!StatisticsSetting.GetInstance().DictEnableDisableChannels.ContainsKey("isheaderenabled"))
                                            StatisticsSetting.GetInstance().DictEnableDisableChannels.Add("isheaderenabled", Convert.ToBoolean(serverDetails[key]));
                                        else
                                            StatisticsSetting.GetInstance().DictEnableDisableChannels["isheaderenabled"] = Convert.ToBoolean(serverDetails[key]);
                                    }

                                }

                                //for (; ; kvpid++)
                                //{

                                else if (string.Compare(key, "agent-tagged-statistics", true) == 0)
                                {
                                    if (StatisticsSetting.GetInstance().DictTaggedStats.Count != 0)
                                    {
                                        statorder = StatisticsSetting.GetInstance().DictTaggedStats.Count + 1;
                                    }
                                    if (StatisticsSetting.GetInstance().DictACDDisplays.Count <= 0 && serverDetails.Keys.Contains("statistics.objects-acd-queues"))
                                    {
                                        statisticsCollection.StatisticsCommon.ACDObjects = Convert.ToString(serverDetails["statistics.objects-acd-queues"]);

                                        //StatisticsSetting.GetInstance().DictACDDisplays.Clear();
                                        string[] ACDQueueObjects = StatisticsSetting.GetInstance().statisticsCollection.StatisticsCommon.ACDObjects.Split(',');

                                        foreach (string ACDs in ACDQueueObjects)
                                        {
                                            string Dilimitor = "_@";
                                            string[] ACDDisplayObjects = ACDs.Split(new[] { Dilimitor }, StringSplitOptions.None);

                                            if (ACDDisplayObjects.Length > 1)
                                            {
                                                if (!StatisticsSetting.GetInstance().DictACDDisplays.ContainsKey(ACDDisplayObjects[0].ToString()))
                                                    StatisticsSetting.GetInstance().DictACDDisplays.Add(ACDDisplayObjects[0].ToString(), ACDDisplayObjects[1].ToString());
                                            }
                                        }
                                    }

                                    if (serverDetails[key].ToString() != "" && serverDetails[key].ToString() != string.Empty && serverDetails[key].ToString() != null)
                                    {
                                        string[] taggedstats = serverDetails[key].ToString().Split(',');

                                        foreach (string statname in taggedstats)
                                        {
                                            string[] stat = statname.Split('@');

                                            if (stat[1].StartsWith("agent"))
                                            {
                                                foreach (AgentStatistics agentStat in statisticsCollection.AgentStatistics)
                                                {
                                                    if (agentStat.TempStatName == stat[1])
                                                    {
                                                        StatisticsSetting.GetInstance().DictTaggedStats.Add(statorder.ToString() + ',' + statname, agentStat.DisplayName);
                                                        statorder++;
                                                    }
                                                }
                                            }
                                            else if (stat[1].StartsWith("group"))
                                            {
                                                foreach (AgentGroupStatistics agentgroupStat in statisticsCollection.AgentGroupStatistics)
                                                {
                                                    if (agentgroupStat.TempStatName == stat[1])
                                                    {
                                                        StatisticsSetting.GetInstance().DictTaggedStats.Add(statorder.ToString() + ',' + statname, agentgroupStat.DisplayName);
                                                        statorder++;
                                                    }
                                                }
                                            }
                                            else if (stat[1].StartsWith("acd"))
                                            {
                                                foreach (ACDStatistics acdStat in statisticsCollection.ACDQueueStatistics)
                                                {
                                                    if (acdStat.TempStatName == stat[1])
                                                    {

                                                        if (StatisticsBase.GetInstance().QueueDisplayName == StatisticsEnum.ACDDisplayName.Queue.ToString() || StatisticsBase.GetInstance().QueueDisplayName == StatisticsEnum.ACDDisplayName.RoutingPoint.ToString())
                                                        {
                                                            string[] tempStatName = statname.Split('@');

                                                            if (StatisticsSetting.GetInstance().DictACDDisplays.ContainsKey(tempStatName[0].ToString()))
                                                            {
                                                                foreach (string StrTemp in StatisticsSetting.GetInstance().DictACDDisplays.Keys)
                                                                {
                                                                    if (tempStatName[0] == StrTemp)
                                                                    {
                                                                        StatisticsSetting.GetInstance().DictTaggedStats.Add(statorder.ToString() + ',' + StrTemp + '@' + tempStatName[1], acdStat.DisplayName + " " + StrTemp);
                                                                        statorder++;
                                                                    }
                                                                }
                                                            }
                                                            else if (StatisticsSetting.GetInstance().DictACDDisplays.ContainsValue(tempStatName[0].ToString()))
                                                            {
                                                                foreach (string StrTemp in StatisticsSetting.GetInstance().DictACDDisplays.Keys)
                                                                {
                                                                    if (tempStatName[0] == StatisticsSetting.GetInstance().DictACDDisplays[StrTemp])
                                                                    {
                                                                        StatisticsSetting.GetInstance().DictTaggedStats.Add(statorder.ToString() + ',' + StrTemp + '@' + tempStatName[1], acdStat.DisplayName + " " + StrTemp);
                                                                        statorder++;
                                                                    }
                                                                }
                                                            }
                                                        }
                                                        else if (StatisticsBase.GetInstance().QueueDisplayName == StatisticsEnum.ACDDisplayName.Skill.ToString())
                                                        {
                                                            string[] tempStatName = statname.Split('@');

                                                            if (StatisticsSetting.GetInstance().DictACDDisplays.ContainsKey(tempStatName[0].ToString()))
                                                            {
                                                                foreach (string StrTemp in StatisticsSetting.GetInstance().DictACDDisplays.Values)
                                                                {
                                                                    if (tempStatName[0] == StrTemp)
                                                                    {
                                                                        StatisticsSetting.GetInstance().DictTaggedStats.Add(statorder.ToString() + ',' + StrTemp + '@' + tempStatName[1], acdStat.DisplayName + " " + StrTemp);
                                                                        statorder++;
                                                                    }
                                                                }
                                                            }
                                                            else if (StatisticsSetting.GetInstance().DictACDDisplays.ContainsValue(tempStatName[0].ToString()))
                                                            {
                                                                foreach (string StrTemp in StatisticsSetting.GetInstance().DictACDDisplays.Values)
                                                                {
                                                                    if (tempStatName[0] == StrTemp)
                                                                    {
                                                                        StatisticsSetting.GetInstance().DictTaggedStats.Add(statorder.ToString() + ',' + StrTemp + '@' + tempStatName[1], acdStat.DisplayName + " " + StrTemp);
                                                                        statorder++;
                                                                    }
                                                                }
                                                            }
                                                        }
                                                        else
                                                        {
                                                            string[] tempStatName = statname.Split('@');

                                                            if (StatisticsSetting.GetInstance().DictACDDisplays.ContainsKey(tempStatName[0].ToString()))
                                                            {
                                                                foreach (string StrTemp in StatisticsSetting.GetInstance().DictACDDisplays.Keys)
                                                                {
                                                                    if (tempStatName[0] == StrTemp)
                                                                    {
                                                                        StatisticsSetting.GetInstance().DictTaggedStats.Add(statorder.ToString() + ',' + StrTemp + '@' + tempStatName[1], acdStat.DisplayName + " " + StrTemp);
                                                                        statorder++;
                                                                    }
                                                                }
                                                            }
                                                            else if (StatisticsSetting.GetInstance().DictACDDisplays.ContainsValue(tempStatName[0].ToString()))
                                                            {
                                                                foreach (string StrTemp in StatisticsSetting.GetInstance().DictACDDisplays.Keys)
                                                                {
                                                                    if (tempStatName[0] == StatisticsSetting.GetInstance().DictACDDisplays[StrTemp])
                                                                    {
                                                                        StatisticsSetting.GetInstance().DictTaggedStats.Add(statorder.ToString() + ',' + StrTemp + '@' + tempStatName[1], acdStat.DisplayName + " " + StrTemp);
                                                                        statorder++;
                                                                    }
                                                                }
                                                            }
                                                        }

                                                    }
                                                }
                                            }
                                            else if (stat[1].StartsWith("dn"))
                                            {
                                                foreach (DNGroupStatistics dnstat in statisticsCollection.DNGroupStatistics)
                                                {
                                                    if (dnstat.TempStatName == stat[1])
                                                    {
                                                        StatisticsSetting.GetInstance().DictTaggedStats.Add(statorder.ToString() + ',' + statname, dnstat.DisplayName);
                                                        statorder++;
                                                    }
                                                }
                                            }
                                            else if (stat[1].StartsWith("vq"))
                                            {
                                                foreach (VirtualQueueStatistics vqStat in statisticsCollection.VirtualQueueStatistics)
                                                {
                                                    if (vqStat.TempStatName == stat[1])
                                                    {
                                                        StatisticsSetting.GetInstance().DictTaggedStats.Add(statorder.ToString() + ',' + statname, vqStat.DisplayName);
                                                        statorder++;
                                                    }
                                                }
                                            }
                                            else if (statisticsCollection.StatisticsCommon.Source == StatisticsEnum.StatSource.DB.ToString())
                                            {
                                                if (statname.StartsWith("db"))
                                                {
                                                    foreach (DBValues dbValues in statisticsCollection.DBValues)
                                                    {
                                                        if (dbValues.TempStat == statname)
                                                        {
                                                            StatisticsSetting.GetInstance().DictTaggedStats.Add(statorder.ToString() + ',' + statname, dbValues.DisplayName);
                                                            statorder++;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    break;
                                }
                                //}

                                #endregion
                            }
                        }

                        //KeyValueCollection enabledisablechannels = (KeyValueCollection)StatisticsSetting.GetInstance().PersonDetails.UserProperties["enable-disable-channels"];
                        //if (enabledisablechannels != null)
                        //{
                        //    #region EnableDisableChannels

                        //    foreach (string key in enabledisablechannels.AllKeys)
                        //    {
                        //        if (string.Compare(key, "statistics.enable-alwaysontop", true) == 0)
                        //        {
                        //            if (enabledisablechannels[key].ToString() != null && enabledisablechannels[key].ToString() != string.Empty && ((string.Compare(enabledisablechannels[key].ToString(), "true", true) == 0) || (string.Compare(enabledisablechannels[key].ToString(), "false", true) == 0)))
                        //            {
                        //                statisticsCollection.ApplicationContainer.EnableAlwaysOnTop = Convert.ToBoolean(enabledisablechannels[key]);
                        //                if (!StatisticsSetting.GetInstance().DictEnableDisableChannels.ContainsKey("AlwaysOnTop"))
                        //                    StatisticsSetting.GetInstance().DictEnableDisableChannels.Add("AlwaysOnTop", Convert.ToBoolean(enabledisablechannels[key]));
                        //                else
                        //                    StatisticsSetting.GetInstance().DictEnableDisableChannels["AlwaysOnTop"] = Convert.ToBoolean(enabledisablechannels[key]);
                        //            }
                        //        }
                        //        else if (string.Compare(key, "statistics.enable-ccstat-aid", true) == 0)
                        //        {
                        //            if (enabledisablechannels[key].ToString() != null && enabledisablechannels[key].ToString() != string.Empty && ((string.Compare(enabledisablechannels[key].ToString(), "true", true) == 0) || (string.Compare(enabledisablechannels[key].ToString(), "false", true) == 0)))
                        //            {
                        //                statisticsCollection.ApplicationContainer.EnableCCStatAID = Convert.ToBoolean(enabledisablechannels[key]);
                        //                if (!StatisticsSetting.GetInstance().DictEnableDisableChannels.ContainsKey("ccstataid"))
                        //                    StatisticsSetting.GetInstance().DictEnableDisableChannels.Add("ccstataid", Convert.ToBoolean(enabledisablechannels[key]));
                        //                else
                        //                    StatisticsSetting.GetInstance().DictEnableDisableChannels["ccstataid"] = Convert.ToBoolean(enabledisablechannels[key]);
                        //            }
                        //        }
                        //        else if (string.Compare(key, "statistics.enable-log", true) == 0)
                        //        {
                        //            if (enabledisablechannels[key].ToString() != null && enabledisablechannels[key].ToString() != string.Empty && ((string.Compare(enabledisablechannels[key].ToString(), "true", true) == 0) || (string.Compare(enabledisablechannels[key].ToString(), "false", true) == 0)))
                        //            {
                        //                statisticsCollection.ApplicationContainer.EnableLog = Convert.ToBoolean(enabledisablechannels[key]);
                        //                if (!StatisticsSetting.GetInstance().DictEnableDisableChannels.ContainsKey("log"))
                        //                    StatisticsSetting.GetInstance().DictEnableDisableChannels.Add("log", Convert.ToBoolean(enabledisablechannels[key]));
                        //                else
                        //                    StatisticsSetting.GetInstance().DictEnableDisableChannels["log"] = Convert.ToBoolean(enabledisablechannels[key]);
                        //            }
                        //        }
                        //        else if (string.Compare(key, "statistics.enable-maingadget", true) == 0)
                        //        {
                        //            if (enabledisablechannels[key].ToString() != null && enabledisablechannels[key].ToString() != string.Empty && ((string.Compare(enabledisablechannels[key].ToString(), "true", true) == 0) || (string.Compare(enabledisablechannels[key].ToString(), "false", true) == 0)))
                        //            {
                        //                statisticsCollection.ApplicationContainer.EnableMainGadget = Convert.ToBoolean(enabledisablechannels[key]);
                        //                if (!StatisticsSetting.GetInstance().DictEnableDisableChannels.ContainsKey("maingadget"))
                        //                    StatisticsSetting.GetInstance().DictEnableDisableChannels.Add("maingadget", Convert.ToBoolean(enabledisablechannels[key]));
                        //                else
                        //                    StatisticsSetting.GetInstance().DictEnableDisableChannels["maingadget"] = Convert.ToBoolean(enabledisablechannels[key]);
                        //            }
                        //        }
                        //        else if (string.Compare(key, "statistics.enable-menu-button", true) == 0)
                        //        {
                        //            if (enabledisablechannels[key].ToString() != null && enabledisablechannels[key].ToString() != string.Empty && ((string.Compare(enabledisablechannels[key].ToString(), "true", true) == 0) || (string.Compare(enabledisablechannels[key].ToString(), "false", true) == 0)))
                        //            {
                        //                statisticsCollection.ApplicationContainer.EnableMenuButton = Convert.ToBoolean(enabledisablechannels[key]);
                        //                if (!StatisticsSetting.GetInstance().DictEnableDisableChannels.ContainsKey("menubutton"))
                        //                    StatisticsSetting.GetInstance().DictEnableDisableChannels.Add("menubutton", Convert.ToBoolean(enabledisablechannels[key]));
                        //                else
                        //                    StatisticsSetting.GetInstance().DictEnableDisableChannels["menubutton"] = Convert.ToBoolean(enabledisablechannels[key]);
                        //            }
                        //        }
                        //        else if (string.Compare(key, "statistics.enable-mystat-aid", true) == 0)
                        //        {
                        //            if (enabledisablechannels[key].ToString() != null && enabledisablechannels[key].ToString() != string.Empty && ((string.Compare(enabledisablechannels[key].ToString(), "true", true) == 0) || (string.Compare(enabledisablechannels[key].ToString(), "false", true) == 0)))
                        //            {
                        //                statisticsCollection.ApplicationContainer.EnableMyStatAID = Convert.ToBoolean(enabledisablechannels[key]);
                        //                if (!StatisticsSetting.GetInstance().DictEnableDisableChannels.ContainsKey("mystataid"))
                        //                    StatisticsSetting.GetInstance().DictEnableDisableChannels.Add("mystataid", Convert.ToBoolean(enabledisablechannels[key]));
                        //                else
                        //                    StatisticsSetting.GetInstance().DictEnableDisableChannels["mystataid"] = Convert.ToBoolean(enabledisablechannels[key]);
                        //            }
                        //        }
                        //        else if (string.Compare(key, "statistics.enable-notification-close", true) == 0)
                        //        {
                        //            if (enabledisablechannels[key].ToString() != null && enabledisablechannels[key].ToString() != string.Empty && ((string.Compare(enabledisablechannels[key].ToString(), "true", true) == 0) || (string.Compare(enabledisablechannels[key].ToString(), "false", true) == 0)))
                        //            {
                        //                statisticsCollection.ApplicationContainer.EnableTaskbarClose = Convert.ToBoolean(enabledisablechannels[key]);
                        //                if (!StatisticsSetting.GetInstance().DictEnableDisableChannels.ContainsKey("notificationclose"))
                        //                    StatisticsSetting.GetInstance().DictEnableDisableChannels.Add("notificationclose", Convert.ToBoolean(enabledisablechannels[key]));
                        //                else
                        //                    StatisticsSetting.GetInstance().DictEnableDisableChannels["notificationclose"] = Convert.ToBoolean(enabledisablechannels[key]);
                        //            }
                        //        }
                        //        else if (string.Compare(key, "statistics.enable-notification-balloon", true) == 0)
                        //        {
                        //            if (enabledisablechannels[key].ToString() != null && enabledisablechannels[key].ToString() != string.Empty && ((string.Compare(enabledisablechannels[key].ToString(), "true", true) == 0) || (string.Compare(enabledisablechannels[key].ToString(), "false", true) == 0)))
                        //            {
                        //                statisticsCollection.ApplicationContainer.EnableNotificationBalloon = Convert.ToBoolean(enabledisablechannels[key]);
                        //                if (!StatisticsSetting.GetInstance().DictEnableDisableChannels.ContainsKey("notificationballoon"))
                        //                    StatisticsSetting.GetInstance().DictEnableDisableChannels.Add("notificationballoon", Convert.ToBoolean(enabledisablechannels[key]));
                        //                else
                        //                    StatisticsSetting.GetInstance().DictEnableDisableChannels["notificationballoon"] = Convert.ToBoolean(enabledisablechannels[key]);
                        //            }
                        //        }
                        //        else if (string.Compare(key, "statistics.enable-submenu-ccstatistics", true) == 0)
                        //        {
                        //            if (enabledisablechannels[key].ToString() != null && enabledisablechannels[key].ToString() != string.Empty && ((string.Compare(enabledisablechannels[key].ToString(), "true", true) == 0) || (string.Compare(enabledisablechannels[key].ToString(), "false", true) == 0)))
                        //            {
                        //                statisticsCollection.ApplicationContainer.EnableMenuShowCCStat = Convert.ToBoolean(enabledisablechannels[key]);
                        //                if (!StatisticsSetting.GetInstance().DictEnableDisableChannels.ContainsKey("ccstatistics"))
                        //                    StatisticsSetting.GetInstance().DictEnableDisableChannels.Add("ccstatistics", Convert.ToBoolean(enabledisablechannels[key]));
                        //                else
                        //                    StatisticsSetting.GetInstance().DictEnableDisableChannels["ccstatistics"] = Convert.ToBoolean(enabledisablechannels[key]);
                        //            }
                        //        }
                        //        else if (string.Compare(key, "statistics.enable-submenu-close-gadget", true) == 0)
                        //        {
                        //            if (enabledisablechannels[key].ToString() != null && enabledisablechannels[key].ToString() != string.Empty && ((string.Compare(enabledisablechannels[key].ToString(), "true", true) == 0) || (string.Compare(enabledisablechannels[key].ToString(), "false", true) == 0)))
                        //            {
                        //                statisticsCollection.ApplicationContainer.EnableMenuClose = Convert.ToBoolean(enabledisablechannels[key]);
                        //                if (!StatisticsSetting.GetInstance().DictEnableDisableChannels.ContainsKey("closegadget"))
                        //                    StatisticsSetting.GetInstance().DictEnableDisableChannels.Add("closegadget", Convert.ToBoolean(enabledisablechannels[key]));
                        //                else
                        //                    StatisticsSetting.GetInstance().DictEnableDisableChannels["closegadget"] = Convert.ToBoolean(enabledisablechannels[key]);
                        //            }
                        //        }
                        //        else if (string.Compare(key, "statistics.enable-submenu-mystatistics", true) == 0)
                        //        {
                        //            if (enabledisablechannels[key].ToString() != null && enabledisablechannels[key].ToString() != string.Empty && ((string.Compare(enabledisablechannels[key].ToString(), "true", true) == 0) || (string.Compare(enabledisablechannels[key].ToString(), "false", true) == 0)))
                        //            {
                        //                statisticsCollection.ApplicationContainer.EnableMenuShowMyStat = Convert.ToBoolean(enabledisablechannels[key]);
                        //                if (!StatisticsSetting.GetInstance().DictEnableDisableChannels.ContainsKey("mystatistics"))
                        //                    StatisticsSetting.GetInstance().DictEnableDisableChannels.Add("mystatistics", Convert.ToBoolean(enabledisablechannels[key]));
                        //                else
                        //                    StatisticsSetting.GetInstance().DictEnableDisableChannels["mystatistics"] = Convert.ToBoolean(enabledisablechannels[key]);
                        //            }
                        //        }
                        //        else if (string.Compare(key, "statistics.enable-submenu-ontop", true) == 0)
                        //        {
                        //            if (enabledisablechannels[key].ToString() != null && enabledisablechannels[key].ToString() != string.Empty && ((string.Compare(enabledisablechannels[key].ToString(), "true", true) == 0) || (string.Compare(enabledisablechannels[key].ToString(), "false", true) == 0)))
                        //            {
                        //                statisticsCollection.ApplicationContainer.EnableMenuOnTop = Convert.ToBoolean(enabledisablechannels[key]);
                        //                if (!StatisticsSetting.GetInstance().DictEnableDisableChannels.ContainsKey("ontop"))
                        //                    StatisticsSetting.GetInstance().DictEnableDisableChannels.Add("ontop", Convert.ToBoolean(enabledisablechannels[key]));
                        //                else
                        //                    StatisticsSetting.GetInstance().DictEnableDisableChannels["ontop"] = Convert.ToBoolean(enabledisablechannels[key]);
                        //            }
                        //        }
                        //        else if (string.Compare(key, "statistics.enable-tag-button", true) == 0)
                        //        {
                        //            if (enabledisablechannels[key].ToString() != null && enabledisablechannels[key].ToString() != string.Empty && ((string.Compare(enabledisablechannels[key].ToString(), "true", true) == 0) || (string.Compare(enabledisablechannels[key].ToString(), "false", true) == 0)))
                        //            {
                        //                statisticsCollection.ApplicationContainer.EnableTagButton = Convert.ToBoolean(enabledisablechannels[key]);
                        //                if (!StatisticsSetting.GetInstance().DictEnableDisableChannels.ContainsKey("tagbutton"))
                        //                    StatisticsSetting.GetInstance().DictEnableDisableChannels.Add("tagbutton", Convert.ToBoolean(enabledisablechannels[key]));
                        //                else
                        //                    StatisticsSetting.GetInstance().DictEnableDisableChannels["tagbutton"] = Convert.ToBoolean(enabledisablechannels[key]);
                        //            }
                        //        }
                        //        else if (string.Compare(key, "statistics.enable-tag-vertical", true) == 0)
                        //        {
                        //            if (enabledisablechannels[key].ToString() != null && enabledisablechannels[key].ToString() != string.Empty && ((string.Compare(enabledisablechannels[key].ToString(), "true", true) == 0) || (string.Compare(enabledisablechannels[key].ToString(), "false", true) == 0)))
                        //            {
                        //                statisticsCollection.ApplicationContainer.EnableTagVertical = Convert.ToBoolean(enabledisablechannels[key]);
                        //                if (!StatisticsSetting.GetInstance().DictEnableDisableChannels.ContainsKey("tagvertical"))
                        //                    StatisticsSetting.GetInstance().DictEnableDisableChannels.Add("tagvertical", Convert.ToBoolean(enabledisablechannels[key]));
                        //                else
                        //                    StatisticsSetting.GetInstance().DictEnableDisableChannels["tagvertical"] = Convert.ToBoolean(enabledisablechannels[key]);
                        //            }
                        //        }
                        //        else if (string.Compare(key, "statistics.enable-untag-button", true) == 0)
                        //        {
                        //            if (enabledisablechannels[key].ToString() != null && enabledisablechannels[key].ToString() != string.Empty && ((string.Compare(enabledisablechannels[key].ToString(), "true", true) == 0) || (string.Compare(enabledisablechannels[key].ToString(), "false", true) == 0)))
                        //            {
                        //                statisticsCollection.ApplicationContainer.EnableUntagButton = Convert.ToBoolean(enabledisablechannels[key]);
                        //                if (!StatisticsSetting.GetInstance().DictEnableDisableChannels.ContainsKey("untagbutton"))
                        //                    StatisticsSetting.GetInstance().DictEnableDisableChannels.Add("untagbutton", Convert.ToBoolean(enabledisablechannels[key]));
                        //                else
                        //                    StatisticsSetting.GetInstance().DictEnableDisableChannels["untagbutton"] = Convert.ToBoolean(enabledisablechannels[key]);
                        //            }
                        //        }
                        //        else if (string.Compare(key, "statistics.enable-hhmmss-format", true) == 0)
                        //        {
                        //            if (!StatisticsSetting.GetInstance().DictEnableDisableChannels.ContainsKey("hhmmssformat"))
                        //                StatisticsSetting.GetInstance().DictEnableDisableChannels.Add("hhmmssformat", Convert.ToBoolean(enabledisablechannels[key]));
                        //            else
                        //                StatisticsSetting.GetInstance().DictEnableDisableChannels["hhmmssformat"] = Convert.ToBoolean(enabledisablechannels[key]);

                        //            statisticsCollection.ApplicationContainer.EnableHHMMSS = Convert.ToBoolean(enabledisablechannels[key]);
                        //        }
                        //        else if (string.Compare(key, "statistics.enable-submenu-alert-notification", true) == 0)
                        //        {
                        //            if (enabledisablechannels[key].ToString() != null && enabledisablechannels[key].ToString() != string.Empty && ((string.Compare(enabledisablechannels[key].ToString(), "true", true) == 0) || (string.Compare(enabledisablechannels[key].ToString(), "false", true) == 0)))
                        //            {
                        //                statisticsCollection.ApplicationContainer.EnableThresholdNotification = Convert.ToBoolean(enabledisablechannels[key]);
                        //                if (!StatisticsSetting.GetInstance().DictEnableDisableChannels.ContainsKey("thresholdnotification"))
                        //                    StatisticsSetting.GetInstance().DictEnableDisableChannels.Add("thresholdnotification", Convert.ToBoolean(enabledisablechannels[key]));
                        //                else
                        //                    StatisticsSetting.GetInstance().DictEnableDisableChannels["thresholdnotification"] = Convert.ToBoolean(enabledisablechannels[key]);
                        //            }
                        //        }
                        //        else if (string.Compare(key, "statistics.enable-system-draggable", true) == 0)
                        //        {
                        //            statisticsCollection.ApplicationContainer.SystemDraggable = Convert.ToBoolean(enabledisablechannels[key]);
                        //            StatisticsBase.GetInstance().IsSystemDraggable = Convert.ToBoolean(enabledisablechannels[key]);

                        //        }
                        //        else if (string.Compare(key, "statistics.enable-submenu-showheader", true) == 0)
                        //        {
                        //            if (enabledisablechannels[key].ToString() != null && enabledisablechannels[key].ToString() != string.Empty && ((string.Compare(enabledisablechannels[key].ToString(), "true", true) == 0) || (string.Compare(enabledisablechannels[key].ToString(), "false", true) == 0)))
                        //            {
                        //                if (!StatisticsSetting.GetInstance().DictEnableDisableChannels.ContainsKey("showheader"))
                        //                    StatisticsSetting.GetInstance().DictEnableDisableChannels.Add("showheader", Convert.ToBoolean(enabledisablechannels[key]));
                        //                else
                        //                    StatisticsSetting.GetInstance().DictEnableDisableChannels["showheader"] = Convert.ToBoolean(enabledisablechannels[key]);
                        //            }
                        //        }
                        //        else if (string.Compare(key, "statistics.enable-header", true) == 0)
                        //        {
                        //            if (enabledisablechannels[key].ToString() != null && enabledisablechannels[key].ToString() != string.Empty && ((string.Compare(enabledisablechannels[key].ToString(), "true", true) == 0) || (string.Compare(enabledisablechannels[key].ToString(), "false", true) == 0)))
                        //            {
                        //                if (!StatisticsSetting.GetInstance().DictEnableDisableChannels.ContainsKey("isheaderenabled"))
                        //                    StatisticsSetting.GetInstance().DictEnableDisableChannels.Add("isheaderenabled", Convert.ToBoolean(enabledisablechannels[key]));
                        //                else
                        //                    StatisticsSetting.GetInstance().DictEnableDisableChannels["isheaderenabled"] = Convert.ToBoolean(enabledisablechannels[key]);
                        //            }

                        //        }
                        //        else if (string.Compare(key, "statistics.enable-notify-primary-screen", true) == 0)
                        //        {
                        //            if (!StatisticsSetting.GetInstance().DictEnableDisableChannels.ContainsKey("notifyprimaryscreen"))
                        //                StatisticsSetting.GetInstance().DictEnableDisableChannels.Add("notifyprimaryscreen", Convert.ToBoolean(enabledisablechannels[key]));
                        //            else
                        //                StatisticsSetting.GetInstance().DictEnableDisableChannels["notifyprimaryscreen"] = Convert.ToBoolean(enabledisablechannels[key]);

                        //        }
                        //    }

                        //    #endregion
                        //}

                    }
                }
                catch (Exception GeneralException)
                {
                    logger.Error("ReadApplication : GetPersonDBID Method: " + GeneralException.Message);
                }
            }
            else
            {
                int kvpid = 1;
                try
                {
                    logger.Debug("ReadApplication : GetPersonDBID Method: Entry");

                    if (StatisticsSetting.GetInstance().PersonDetails != null)
                    {
                        if (StatisticsSetting.GetInstance().PersonDetails.EmployeeID != null)
                        {
                            StatisticsSetting.GetInstance().AgentEmpId = StatisticsSetting.GetInstance().PersonDetails.EmployeeID.ToString();
                        }

                        KeyValueCollection serverDetails = (KeyValueCollection)StatisticsSetting.GetInstance().PersonDetails.UserProperties["agent.ixn.desktop"];

                        if (serverDetails != null)
                        {

                            foreach (string key in serverDetails.AllKeys)
                            {
                                #region Agent Statistics Configuration

                                if (key.StartsWith("agent-group-statistics"))
                                {
                                    statisticsCollection.StatisticsCommon.AgentGroupStatistics = Convert.ToString(serverDetails[key]);
                                }
                                else if (key.StartsWith("agent-statistics"))
                                {
                                    statisticsCollection.StatisticsCommon.AgentStatistics = Convert.ToString(serverDetails[key]);
                                }
                                else if (key.StartsWith("virtual-queue-statistics"))
                                {
                                    statisticsCollection.StatisticsCommon.VQueueStatistics = Convert.ToString(serverDetails[key]);
                                }
                                else if (key.StartsWith("acd-queue-statistics"))
                                {
                                    statisticsCollection.StatisticsCommon.ACDStatistics = Convert.ToString(serverDetails[key]);
                                }
                                else if (key.StartsWith("dn-group-statistics"))
                                {
                                    statisticsCollection.StatisticsCommon.DNGroupStatistics = Convert.ToString(serverDetails[key]);
                                }
                                else if (key.StartsWith("statistics.objects-acd-queues"))
                                {
                                    statisticsCollection.StatisticsCommon.ACDObjects = Convert.ToString(serverDetails[key]);

                                    StatisticsSetting.GetInstance().DictACDDisplays.Clear();
                                    string[] ACDQueueObjects = StatisticsSetting.GetInstance().statisticsCollection.StatisticsCommon.ACDObjects.Split(',');

                                    foreach (string ACDs in ACDQueueObjects)
                                    {
                                        string Dilimitor = "_@";
                                        string[] ACDDisplayObjects = ACDs.Split(new[] { Dilimitor }, StringSplitOptions.None);

                                        if (ACDDisplayObjects.Length > 1)
                                        {
                                            if (!StatisticsSetting.GetInstance().DictACDDisplays.ContainsKey(ACDDisplayObjects[0].ToString()))
                                                StatisticsSetting.GetInstance().DictACDDisplays.Add(ACDDisplayObjects[0].ToString(), ACDDisplayObjects[1].ToString());
                                        }
                                    }
                                }
                                else if (key.StartsWith("statistics.objects-dn-groups"))
                                {
                                    statisticsCollection.StatisticsCommon.DNGroupObjects = Convert.ToString(serverDetails[key]);
                                }
                                else if (key.StartsWith("statistics.objects-virtual-queues"))
                                {
                                    statisticsCollection.StatisticsCommon.VQueueObjects = Convert.ToString(serverDetails[key]);
                                }
                                else if (string.Compare(key, "statistics.display-width", true) == 0)
                                {
                                    StatisticsBase.GetInstance().GadgetWidth = Convert.ToDouble(serverDetails[key]);
                                }
                                else if (string.Compare(key, "statistics.objects-acd-display", true) == 0)
                                {
                                    StatisticsBase.GetInstance().QueueDisplayName = Convert.ToString(serverDetails[key]);
                                }
                                else if (string.Compare(key, "statistics.gadget-position", true) == 0)
                                {
                                    statisticsCollection.StatisticsCommon.Position = Convert.ToString(serverDetails[key]);

                                }
                                else if (string.Compare(key, "statistics.db-color", true) == 0)
                                {
                                    statisticsCollection.StatisticsCommon.DBColor = Color.FromName(serverDetails[key].ToString());
                                }
                                else if (string.Compare(key, "statistics.server-color", true) == 0)
                                {
                                    statisticsCollection.StatisticsCommon.ServerColor = Color.FromName(serverDetails[key].ToString());
                                }

                                for (; ; kvpid++)
                                {
                                    if (StatisticsSetting.GetInstance().DictTaggedStats.Count != 0)
                                    {
                                        statorder = StatisticsSetting.GetInstance().DictTaggedStats.Count + 1;
                                    }

                                    //if (string.Compare(key, "agent-tagged-statistics_" + kvpid.ToString(), true) == 0)
                                    //{
                                    //    if (serverDetails[key].ToString() != "" && serverDetails[key].ToString() != string.Empty && serverDetails[key].ToString() != null)
                                    //    {
                                    //        string[] taggedstats = serverDetails[key].ToString().Split(',');

                                    //        foreach (string statname in taggedstats)
                                    //        {
                                    //            string[] stat = statname.Split('@');

                                    //            if (stat[1].StartsWith("agent"))
                                    //            {
                                    //                foreach (AgentStatistics agentStat in statisticsCollection.AgentStatistics)
                                    //                {
                                    //                    if (agentStat.TempStatName == stat[1])
                                    //                    {
                                    //                        StatisticsSetting.GetInstance().DictTaggedStats.Add(statorder.ToString() + ',' + statname, agentStat.DisplayName);
                                    //                        statorder++;
                                    //                    }
                                    //                }
                                    //            }
                                    //            else if (stat[1].StartsWith("group"))
                                    //            {
                                    //                foreach (AgentGroupStatistics agentgroupStat in statisticsCollection.AgentGroupStatistics)
                                    //                {
                                    //                    if (agentgroupStat.TempStatName == stat[1])
                                    //                    {
                                    //                        StatisticsSetting.GetInstance().DictTaggedStats.Add(statorder.ToString() + ',' + statname, agentgroupStat.DisplayName);
                                    //                        statorder++;
                                    //                    }
                                    //                }
                                    //            }
                                    //            else if (stat[1].StartsWith("acd"))
                                    //            {
                                    //                foreach (ACDStatistics acdStat in statisticsCollection.ACDQueueStatistics)
                                    //                {
                                    //                    if (acdStat.TempStatName == stat[1])
                                    //                    {

                                    //                        if (StatisticsBase.GetInstance().QueueDisplayName == StatisticsEnum.ACDDisplayName.Queue.ToString() || StatisticsBase.GetInstance().QueueDisplayName == StatisticsEnum.ACDDisplayName.VirtualQueue.ToString())
                                    //                        {
                                    //                            string[] tempStatName = statname.Split('@');

                                    //                            if (StatisticsSetting.GetInstance().DictACDDisplays.ContainsKey(tempStatName[0].ToString()))
                                    //                            {
                                    //                                foreach (string StrTemp in StatisticsSetting.GetInstance().DictACDDisplays.Keys)
                                    //                                {
                                    //                                    if (tempStatName[0] == StrTemp)
                                    //                                    {
                                    //                                        StatisticsSetting.GetInstance().DictTaggedStats.Add(statorder.ToString() + ',' + StrTemp + '@' + tempStatName[1], acdStat.DisplayName + " " + StrTemp);
                                    //                                        statorder++;
                                    //                                    }
                                    //                                }
                                    //                            }
                                    //                            else if (StatisticsSetting.GetInstance().DictACDDisplays.ContainsValue(tempStatName[0].ToString()))
                                    //                            {
                                    //                                foreach (string StrTemp in StatisticsSetting.GetInstance().DictACDDisplays.Keys)
                                    //                                {
                                    //                                    if (tempStatName[0] == StatisticsSetting.GetInstance().DictACDDisplays[StrTemp])
                                    //                                    {
                                    //                                        StatisticsSetting.GetInstance().DictTaggedStats.Add(statorder.ToString() + ',' + StrTemp + '@' + tempStatName[1], acdStat.DisplayName + " " + StrTemp);
                                    //                                        statorder++;
                                    //                                    }
                                    //                                }
                                    //                            }
                                    //                        }
                                    //                        else if (StatisticsBase.GetInstance().QueueDisplayName == StatisticsEnum.ACDDisplayName.Skill.ToString())
                                    //                        {
                                    //                            string[] tempStatName = statname.Split('@');

                                    //                            if (StatisticsSetting.GetInstance().DictACDDisplays.ContainsKey(tempStatName[0].ToString()))
                                    //                            {
                                    //                                foreach (string StrTemp in StatisticsSetting.GetInstance().DictACDDisplays.Values)
                                    //                                {
                                    //                                    if (tempStatName[0] == StrTemp)
                                    //                                    {
                                    //                                        StatisticsSetting.GetInstance().DictTaggedStats.Add(statorder.ToString() + ',' + StrTemp + '@' + tempStatName[1], acdStat.DisplayName + " " + StrTemp);
                                    //                                        statorder++;
                                    //                                    }
                                    //                                }
                                    //                            }
                                    //                            else if (StatisticsSetting.GetInstance().DictACDDisplays.ContainsValue(tempStatName[0].ToString()))
                                    //                            {
                                    //                                foreach (string StrTemp in StatisticsSetting.GetInstance().DictACDDisplays.Values)
                                    //                                {
                                    //                                    if (tempStatName[0] == StrTemp)
                                    //                                    {
                                    //                                        StatisticsSetting.GetInstance().DictTaggedStats.Add(statorder.ToString() + ',' + StrTemp + '@' + tempStatName[1], acdStat.DisplayName + " " + StrTemp);
                                    //                                        statorder++;
                                    //                                    }
                                    //                                }
                                    //                            }
                                    //                        }
                                    //                        else
                                    //                        {
                                    //                            string[] tempStatName = statname.Split('@');

                                    //                            if (StatisticsSetting.GetInstance().DictACDDisplays.ContainsKey(tempStatName[0].ToString()))
                                    //                            {
                                    //                                foreach (string StrTemp in StatisticsSetting.GetInstance().DictACDDisplays.Keys)
                                    //                                {
                                    //                                    if (tempStatName[0] == StrTemp)
                                    //                                    {
                                    //                                        StatisticsSetting.GetInstance().DictTaggedStats.Add(statorder.ToString() + ',' + StrTemp + '@' + tempStatName[1], acdStat.DisplayName + " " + StrTemp);
                                    //                                        statorder++;
                                    //                                    }
                                    //                                }
                                    //                            }
                                    //                            else if (StatisticsSetting.GetInstance().DictACDDisplays.ContainsValue(tempStatName[0].ToString()))
                                    //                            {
                                    //                                foreach (string StrTemp in StatisticsSetting.GetInstance().DictACDDisplays.Keys)
                                    //                                {
                                    //                                    if (tempStatName[0] == StatisticsSetting.GetInstance().DictACDDisplays[StrTemp])
                                    //                                    {
                                    //                                        StatisticsSetting.GetInstance().DictTaggedStats.Add(statorder.ToString() + ',' + StrTemp + '@' + tempStatName[1], acdStat.DisplayName + " " + StrTemp);
                                    //                                        statorder++;
                                    //                                    }
                                    //                                }
                                    //                            }
                                    //                        }

                                    //                    }
                                    //                }
                                    //            }
                                    //            else if (stat[1].StartsWith("dn"))
                                    //            {
                                    //                foreach (DNGroupStatistics dnstat in statisticsCollection.DNGroupStatistics)
                                    //                {
                                    //                    if (dnstat.TempStatName == stat[1])
                                    //                    {
                                    //                        StatisticsSetting.GetInstance().DictTaggedStats.Add(statorder.ToString() + ',' + statname, dnstat.DisplayName);
                                    //                        statorder++;
                                    //                    }
                                    //                }
                                    //            }
                                    //            else if (stat[1].StartsWith("vq"))
                                    //            {
                                    //                foreach (VirtualQueueStatistics vqStat in statisticsCollection.VirtualQueueStatistics)
                                    //                {
                                    //                    if (vqStat.TempStatName == stat[1])
                                    //                    {
                                    //                        StatisticsSetting.GetInstance().DictTaggedStats.Add(statorder.ToString() + ',' + statname, vqStat.DisplayName);
                                    //                        statorder++;
                                    //                    }
                                    //                }
                                    //            }
                                    //            else if (statisticsCollection.StatisticsCommon.Source == StatisticsEnum.StatSource.DB.ToString())
                                    //            {
                                    //                if (statname.StartsWith("db"))
                                    //                {
                                    //                    foreach (DBValues dbValues in statisticsCollection.DBValues)
                                    //                    {
                                    //                        if (dbValues.TempStat == statname)
                                    //                        {
                                    //                            StatisticsSetting.GetInstance().DictTaggedStats.Add(statorder.ToString() + ',' + statname, dbValues.DisplayName);
                                    //                            statorder++;
                                    //                        }
                                    //                    }
                                    //                }
                                    //            }
                                    //        }
                                    //    }
                                    //}
                                    if (string.Compare(key, "agent-tagged-statistics_" + kvpid.ToString(), true) == 0)
                                    {
                                        if (StatisticsSetting.GetInstance().DictACDDisplays.Count <= 0 && serverDetails.AllKeys.Contains("statistics.objects-acd-queues"))
                                        {
                                            statisticsCollection.StatisticsCommon.ACDObjects = Convert.ToString(serverDetails["statistics.objects-acd-queues"]);

                                            //StatisticsSetting.GetInstance().DictACDDisplays.Clear();
                                            string[] ACDQueueObjects = StatisticsSetting.GetInstance().statisticsCollection.StatisticsCommon.ACDObjects.Split(',');

                                            foreach (string ACDs in ACDQueueObjects)
                                            {
                                                string Dilimitor = "_@";
                                                string[] ACDDisplayObjects = ACDs.Split(new[] { Dilimitor }, StringSplitOptions.None);

                                                if (ACDDisplayObjects.Length > 1)
                                                {
                                                    if (!StatisticsSetting.GetInstance().DictACDDisplays.ContainsKey(ACDDisplayObjects[0].ToString()))
                                                        StatisticsSetting.GetInstance().DictACDDisplays.Add(ACDDisplayObjects[0].ToString(), ACDDisplayObjects[1].ToString());
                                                }
                                            }
                                        }

                                        if (serverDetails[key].ToString() != "" && serverDetails[key].ToString() != string.Empty && serverDetails[key].ToString() != null)
                                        {
                                            string[] taggedstats = serverDetails[key].ToString().Split(',');

                                            foreach (string statname in taggedstats)
                                            {
                                                string[] stat = statname.Split('@');

                                                if (stat[1].StartsWith("agent"))
                                                {
                                                    foreach (AgentStatistics agentStat in statisticsCollection.AgentStatistics)
                                                    {
                                                        if (agentStat.TempStatName == stat[1])
                                                        {
                                                            StatisticsSetting.GetInstance().DictTaggedStats.Add(statorder.ToString() + ',' + statname, agentStat.DisplayName);
                                                            statorder++;
                                                        }
                                                    }
                                                }
                                                else if (stat[1].StartsWith("group"))
                                                {
                                                    foreach (AgentGroupStatistics agentgroupStat in statisticsCollection.AgentGroupStatistics)
                                                    {
                                                        if (agentgroupStat.TempStatName == stat[1])
                                                        {
                                                            StatisticsSetting.GetInstance().DictTaggedStats.Add(statorder.ToString() + ',' + statname, agentgroupStat.DisplayName);
                                                            statorder++;
                                                        }
                                                    }
                                                }
                                                else if (stat[1].StartsWith("acd"))
                                                {
                                                    foreach (ACDStatistics acdStat in statisticsCollection.ACDQueueStatistics)
                                                    {
                                                        if (acdStat.TempStatName == stat[1])
                                                        {

                                                            if (StatisticsBase.GetInstance().QueueDisplayName == StatisticsEnum.ACDDisplayName.Queue.ToString() || StatisticsBase.GetInstance().QueueDisplayName == StatisticsEnum.ACDDisplayName.RoutingPoint.ToString())
                                                            {
                                                                string[] tempStatName = statname.Split('@');

                                                                if (StatisticsSetting.GetInstance().DictACDDisplays.ContainsKey(tempStatName[0].ToString()))
                                                                {
                                                                    foreach (string StrTemp in StatisticsSetting.GetInstance().DictACDDisplays.Keys)
                                                                    {
                                                                        if (tempStatName[0] == StrTemp)
                                                                        {
                                                                            StatisticsSetting.GetInstance().DictTaggedStats.Add(statorder.ToString() + ',' + StrTemp + '@' + tempStatName[1], acdStat.DisplayName + " " + StrTemp);
                                                                            statorder++;
                                                                        }
                                                                    }
                                                                }
                                                                else if (StatisticsSetting.GetInstance().DictACDDisplays.ContainsValue(tempStatName[0].ToString()))
                                                                {
                                                                    foreach (string StrTemp in StatisticsSetting.GetInstance().DictACDDisplays.Keys)
                                                                    {
                                                                        if (tempStatName[0] == StatisticsSetting.GetInstance().DictACDDisplays[StrTemp])
                                                                        {
                                                                            StatisticsSetting.GetInstance().DictTaggedStats.Add(statorder.ToString() + ',' + StrTemp + '@' + tempStatName[1], acdStat.DisplayName + " " + StrTemp);
                                                                            statorder++;
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                            else if (StatisticsBase.GetInstance().QueueDisplayName == StatisticsEnum.ACDDisplayName.Skill.ToString())
                                                            {
                                                                string[] tempStatName = statname.Split('@');

                                                                if (StatisticsSetting.GetInstance().DictACDDisplays.ContainsKey(tempStatName[0].ToString()))
                                                                {
                                                                    foreach (string StrTemp in StatisticsSetting.GetInstance().DictACDDisplays.Values)
                                                                    {
                                                                        if (tempStatName[0] == StrTemp)
                                                                        {
                                                                            StatisticsSetting.GetInstance().DictTaggedStats.Add(statorder.ToString() + ',' + StrTemp + '@' + tempStatName[1], acdStat.DisplayName + " " + StrTemp);
                                                                            statorder++;
                                                                        }
                                                                    }
                                                                }
                                                                else if (StatisticsSetting.GetInstance().DictACDDisplays.ContainsValue(tempStatName[0].ToString()))
                                                                {
                                                                    foreach (string StrTemp in StatisticsSetting.GetInstance().DictACDDisplays.Values)
                                                                    {
                                                                        if (tempStatName[0] == StrTemp)
                                                                        {
                                                                            StatisticsSetting.GetInstance().DictTaggedStats.Add(statorder.ToString() + ',' + StrTemp + '@' + tempStatName[1], acdStat.DisplayName + " " + StrTemp);
                                                                            statorder++;
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                            else
                                                            {
                                                                string[] tempStatName = statname.Split('@');

                                                                if (StatisticsSetting.GetInstance().DictACDDisplays.ContainsKey(tempStatName[0].ToString()))
                                                                {
                                                                    foreach (string StrTemp in StatisticsSetting.GetInstance().DictACDDisplays.Keys)
                                                                    {
                                                                        if (tempStatName[0] == StrTemp)
                                                                        {
                                                                            StatisticsSetting.GetInstance().DictTaggedStats.Add(statorder.ToString() + ',' + StrTemp + '@' + tempStatName[1], acdStat.DisplayName + " " + StrTemp);
                                                                            statorder++;
                                                                        }
                                                                    }
                                                                }
                                                                else if (StatisticsSetting.GetInstance().DictACDDisplays.ContainsValue(tempStatName[0].ToString()))
                                                                {
                                                                    foreach (string StrTemp in StatisticsSetting.GetInstance().DictACDDisplays.Keys)
                                                                    {
                                                                        if (tempStatName[0] == StatisticsSetting.GetInstance().DictACDDisplays[StrTemp])
                                                                        {
                                                                            StatisticsSetting.GetInstance().DictTaggedStats.Add(statorder.ToString() + ',' + StrTemp + '@' + tempStatName[1], acdStat.DisplayName + " " + StrTemp);
                                                                            statorder++;
                                                                        }
                                                                    }
                                                                }
                                                            }

                                                        }
                                                    }
                                                }
                                                else if (stat[1].StartsWith("dn"))
                                                {
                                                    foreach (DNGroupStatistics dnstat in statisticsCollection.DNGroupStatistics)
                                                    {
                                                        if (dnstat.TempStatName == stat[1])
                                                        {
                                                            StatisticsSetting.GetInstance().DictTaggedStats.Add(statorder.ToString() + ',' + statname, dnstat.DisplayName);
                                                            statorder++;
                                                        }
                                                    }
                                                }
                                                else if (stat[1].StartsWith("vq"))
                                                {
                                                    foreach (VirtualQueueStatistics vqStat in statisticsCollection.VirtualQueueStatistics)
                                                    {
                                                        if (vqStat.TempStatName == stat[1])
                                                        {
                                                            StatisticsSetting.GetInstance().DictTaggedStats.Add(statorder.ToString() + ',' + statname, vqStat.DisplayName);
                                                            statorder++;
                                                        }
                                                    }
                                                }
                                                else if (statisticsCollection.StatisticsCommon.Source == StatisticsEnum.StatSource.DB.ToString())
                                                {
                                                    if (statname.StartsWith("db"))
                                                    {
                                                        foreach (DBValues dbValues in statisticsCollection.DBValues)
                                                        {
                                                            if (dbValues.TempStat == statname)
                                                            {
                                                                StatisticsSetting.GetInstance().DictTaggedStats.Add(statorder.ToString() + ',' + statname, dbValues.DisplayName);
                                                                statorder++;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }

                                #endregion
                            }
                        }

                        KeyValueCollection enabledisablechannels = (KeyValueCollection)StatisticsSetting.GetInstance().PersonDetails.UserProperties["enable-disable-channels"];
                        if (enabledisablechannels != null)
                        {
                            #region EnableDisableChannels

                            foreach (string key in enabledisablechannels.AllKeys)
                            {
                                if (string.Compare(key, "statistics.enable-alwaysontop", true) == 0)
                                {
                                    if (enabledisablechannels[key].ToString() != null && enabledisablechannels[key].ToString() != string.Empty && ((string.Compare(enabledisablechannels[key].ToString(), "true", true) == 0) || (string.Compare(enabledisablechannels[key].ToString(), "false", true) == 0)))
                                    {
                                        statisticsCollection.ApplicationContainer.EnableAlwaysOnTop = Convert.ToBoolean(enabledisablechannels[key]);
                                        if (!StatisticsSetting.GetInstance().DictEnableDisableChannels.ContainsKey("AlwaysOnTop"))
                                            StatisticsSetting.GetInstance().DictEnableDisableChannels.Add("AlwaysOnTop", Convert.ToBoolean(enabledisablechannels[key]));
                                        else
                                            StatisticsSetting.GetInstance().DictEnableDisableChannels["AlwaysOnTop"] = Convert.ToBoolean(enabledisablechannels[key]);
                                    }
                                }
                                else if (string.Compare(key, "statistics.enable-ccstat-aid", true) == 0)
                                {
                                    if (enabledisablechannels[key].ToString() != null && enabledisablechannels[key].ToString() != string.Empty && ((string.Compare(enabledisablechannels[key].ToString(), "true", true) == 0) || (string.Compare(enabledisablechannels[key].ToString(), "false", true) == 0)))
                                    {
                                        statisticsCollection.ApplicationContainer.EnableCCStatAID = Convert.ToBoolean(enabledisablechannels[key]);
                                        if (!StatisticsSetting.GetInstance().DictEnableDisableChannels.ContainsKey("ccstataid"))
                                            StatisticsSetting.GetInstance().DictEnableDisableChannels.Add("ccstataid", Convert.ToBoolean(enabledisablechannels[key]));
                                        else
                                            StatisticsSetting.GetInstance().DictEnableDisableChannels["ccstataid"] = Convert.ToBoolean(enabledisablechannels[key]);
                                    }
                                }
                                else if (string.Compare(key, "statistics.enable-log", true) == 0)
                                {
                                    if (enabledisablechannels[key].ToString() != null && enabledisablechannels[key].ToString() != string.Empty && ((string.Compare(enabledisablechannels[key].ToString(), "true", true) == 0) || (string.Compare(enabledisablechannels[key].ToString(), "false", true) == 0)))
                                    {
                                        statisticsCollection.ApplicationContainer.EnableLog = Convert.ToBoolean(enabledisablechannels[key]);
                                        if (!StatisticsSetting.GetInstance().DictEnableDisableChannels.ContainsKey("log"))
                                            StatisticsSetting.GetInstance().DictEnableDisableChannels.Add("log", Convert.ToBoolean(enabledisablechannels[key]));
                                        else
                                            StatisticsSetting.GetInstance().DictEnableDisableChannels["log"] = Convert.ToBoolean(enabledisablechannels[key]);
                                    }
                                }
                                else if (string.Compare(key, "statistics.enable-maingadget", true) == 0)
                                {
                                    if (enabledisablechannels[key].ToString() != null && enabledisablechannels[key].ToString() != string.Empty && ((string.Compare(enabledisablechannels[key].ToString(), "true", true) == 0) || (string.Compare(enabledisablechannels[key].ToString(), "false", true) == 0)))
                                    {
                                        statisticsCollection.ApplicationContainer.EnableMainGadget = Convert.ToBoolean(enabledisablechannels[key]);
                                        if (!StatisticsSetting.GetInstance().DictEnableDisableChannels.ContainsKey("maingadget"))
                                            StatisticsSetting.GetInstance().DictEnableDisableChannels.Add("maingadget", Convert.ToBoolean(enabledisablechannels[key]));
                                        else
                                            StatisticsSetting.GetInstance().DictEnableDisableChannels["maingadget"] = Convert.ToBoolean(enabledisablechannels[key]);
                                    }
                                }
                                else if (string.Compare(key, "statistics.enable-menu-button", true) == 0)
                                {
                                    if (enabledisablechannels[key].ToString() != null && enabledisablechannels[key].ToString() != string.Empty && ((string.Compare(enabledisablechannels[key].ToString(), "true", true) == 0) || (string.Compare(enabledisablechannels[key].ToString(), "false", true) == 0)))
                                    {
                                        statisticsCollection.ApplicationContainer.EnableMenuButton = Convert.ToBoolean(enabledisablechannels[key]);
                                        if (!StatisticsSetting.GetInstance().DictEnableDisableChannels.ContainsKey("menubutton"))
                                            StatisticsSetting.GetInstance().DictEnableDisableChannels.Add("menubutton", Convert.ToBoolean(enabledisablechannels[key]));
                                        else
                                            StatisticsSetting.GetInstance().DictEnableDisableChannels["menubutton"] = Convert.ToBoolean(enabledisablechannels[key]);
                                    }
                                }
                                else if (string.Compare(key, "statistics.enable-mystat-aid", true) == 0)
                                {
                                    if (enabledisablechannels[key].ToString() != null && enabledisablechannels[key].ToString() != string.Empty && ((string.Compare(enabledisablechannels[key].ToString(), "true", true) == 0) || (string.Compare(enabledisablechannels[key].ToString(), "false", true) == 0)))
                                    {
                                        statisticsCollection.ApplicationContainer.EnableMyStatAID = Convert.ToBoolean(enabledisablechannels[key]);
                                        if (!StatisticsSetting.GetInstance().DictEnableDisableChannels.ContainsKey("mystataid"))
                                            StatisticsSetting.GetInstance().DictEnableDisableChannels.Add("mystataid", Convert.ToBoolean(enabledisablechannels[key]));
                                        else
                                            StatisticsSetting.GetInstance().DictEnableDisableChannels["mystataid"] = Convert.ToBoolean(enabledisablechannels[key]);
                                    }
                                }
                                else if (string.Compare(key, "statistics.enable-notification-close", true) == 0)
                                {
                                    if (enabledisablechannels[key].ToString() != null && enabledisablechannels[key].ToString() != string.Empty && ((string.Compare(enabledisablechannels[key].ToString(), "true", true) == 0) || (string.Compare(enabledisablechannels[key].ToString(), "false", true) == 0)))
                                    {
                                        statisticsCollection.ApplicationContainer.EnableTaskbarClose = Convert.ToBoolean(enabledisablechannels[key]);
                                        if (!StatisticsSetting.GetInstance().DictEnableDisableChannels.ContainsKey("notificationclose"))
                                            StatisticsSetting.GetInstance().DictEnableDisableChannels.Add("notificationclose", Convert.ToBoolean(enabledisablechannels[key]));
                                        else
                                            StatisticsSetting.GetInstance().DictEnableDisableChannels["notificationclose"] = Convert.ToBoolean(enabledisablechannels[key]);
                                    }
                                }
                                else if (string.Compare(key, "statistics.enable-notification-balloon", true) == 0)
                                {
                                    if (enabledisablechannels[key].ToString() != null && enabledisablechannels[key].ToString() != string.Empty && ((string.Compare(enabledisablechannels[key].ToString(), "true", true) == 0) || (string.Compare(enabledisablechannels[key].ToString(), "false", true) == 0)))
                                    {
                                        statisticsCollection.ApplicationContainer.EnableNotificationBalloon = Convert.ToBoolean(enabledisablechannels[key]);
                                        if (!StatisticsSetting.GetInstance().DictEnableDisableChannels.ContainsKey("notificationballoon"))
                                            StatisticsSetting.GetInstance().DictEnableDisableChannels.Add("notificationballoon", Convert.ToBoolean(enabledisablechannels[key]));
                                        else
                                            StatisticsSetting.GetInstance().DictEnableDisableChannels["notificationballoon"] = Convert.ToBoolean(enabledisablechannels[key]);
                                    }
                                }
                                else if (string.Compare(key, "statistics.enable-submenu-ccstatistics", true) == 0)
                                {
                                    if (enabledisablechannels[key].ToString() != null && enabledisablechannels[key].ToString() != string.Empty && ((string.Compare(enabledisablechannels[key].ToString(), "true", true) == 0) || (string.Compare(enabledisablechannels[key].ToString(), "false", true) == 0)))
                                    {
                                        statisticsCollection.ApplicationContainer.EnableMenuShowCCStat = Convert.ToBoolean(enabledisablechannels[key]);
                                        if (!StatisticsSetting.GetInstance().DictEnableDisableChannels.ContainsKey("ccstatistics"))
                                            StatisticsSetting.GetInstance().DictEnableDisableChannels.Add("ccstatistics", Convert.ToBoolean(enabledisablechannels[key]));
                                        else
                                            StatisticsSetting.GetInstance().DictEnableDisableChannels["ccstatistics"] = Convert.ToBoolean(enabledisablechannels[key]);
                                    }
                                }
                                else if (string.Compare(key, "statistics.enable-submenu-close-gadget", true) == 0)
                                {
                                    if (enabledisablechannels[key].ToString() != null && enabledisablechannels[key].ToString() != string.Empty && ((string.Compare(enabledisablechannels[key].ToString(), "true", true) == 0) || (string.Compare(enabledisablechannels[key].ToString(), "false", true) == 0)))
                                    {
                                        statisticsCollection.ApplicationContainer.EnableMenuClose = Convert.ToBoolean(enabledisablechannels[key]);
                                        if (!StatisticsSetting.GetInstance().DictEnableDisableChannels.ContainsKey("closegadget"))
                                            StatisticsSetting.GetInstance().DictEnableDisableChannels.Add("closegadget", Convert.ToBoolean(enabledisablechannels[key]));
                                        else
                                            StatisticsSetting.GetInstance().DictEnableDisableChannels["closegadget"] = Convert.ToBoolean(enabledisablechannels[key]);
                                    }
                                }
                                else if (string.Compare(key, "statistics.enable-submenu-mystatistics", true) == 0)
                                {
                                    if (enabledisablechannels[key].ToString() != null && enabledisablechannels[key].ToString() != string.Empty && ((string.Compare(enabledisablechannels[key].ToString(), "true", true) == 0) || (string.Compare(enabledisablechannels[key].ToString(), "false", true) == 0)))
                                    {
                                        statisticsCollection.ApplicationContainer.EnableMenuShowMyStat = Convert.ToBoolean(enabledisablechannels[key]);
                                        if (!StatisticsSetting.GetInstance().DictEnableDisableChannels.ContainsKey("mystatistics"))
                                            StatisticsSetting.GetInstance().DictEnableDisableChannels.Add("mystatistics", Convert.ToBoolean(enabledisablechannels[key]));
                                        else
                                            StatisticsSetting.GetInstance().DictEnableDisableChannels["mystatistics"] = Convert.ToBoolean(enabledisablechannels[key]);
                                    }
                                }
                                else if (string.Compare(key, "statistics.enable-submenu-ontop", true) == 0)
                                {
                                    if (enabledisablechannels[key].ToString() != null && enabledisablechannels[key].ToString() != string.Empty && ((string.Compare(enabledisablechannels[key].ToString(), "true", true) == 0) || (string.Compare(enabledisablechannels[key].ToString(), "false", true) == 0)))
                                    {
                                        statisticsCollection.ApplicationContainer.EnableMenuOnTop = Convert.ToBoolean(enabledisablechannels[key]);
                                        if (!StatisticsSetting.GetInstance().DictEnableDisableChannels.ContainsKey("ontop"))
                                            StatisticsSetting.GetInstance().DictEnableDisableChannels.Add("ontop", Convert.ToBoolean(enabledisablechannels[key]));
                                        else
                                            StatisticsSetting.GetInstance().DictEnableDisableChannels["ontop"] = Convert.ToBoolean(enabledisablechannels[key]);
                                    }
                                }
                                else if (string.Compare(key, "statistics.enable-tag-button", true) == 0)
                                {
                                    if (enabledisablechannels[key].ToString() != null && enabledisablechannels[key].ToString() != string.Empty && ((string.Compare(enabledisablechannels[key].ToString(), "true", true) == 0) || (string.Compare(enabledisablechannels[key].ToString(), "false", true) == 0)))
                                    {
                                        statisticsCollection.ApplicationContainer.EnableTagButton = Convert.ToBoolean(enabledisablechannels[key]);
                                        if (!StatisticsSetting.GetInstance().DictEnableDisableChannels.ContainsKey("tagbutton"))
                                            StatisticsSetting.GetInstance().DictEnableDisableChannels.Add("tagbutton", Convert.ToBoolean(enabledisablechannels[key]));
                                        else
                                            StatisticsSetting.GetInstance().DictEnableDisableChannels["tagbutton"] = Convert.ToBoolean(enabledisablechannels[key]);
                                    }
                                }
                                else if (string.Compare(key, "statistics.enable-tag-vertical", true) == 0)
                                {
                                    if (enabledisablechannels[key].ToString() != null && enabledisablechannels[key].ToString() != string.Empty && ((string.Compare(enabledisablechannels[key].ToString(), "true", true) == 0) || (string.Compare(enabledisablechannels[key].ToString(), "false", true) == 0)))
                                    {
                                        statisticsCollection.ApplicationContainer.EnableTagVertical = Convert.ToBoolean(enabledisablechannels[key]);
                                        if (!StatisticsSetting.GetInstance().DictEnableDisableChannels.ContainsKey("tagvertical"))
                                            StatisticsSetting.GetInstance().DictEnableDisableChannels.Add("tagvertical", Convert.ToBoolean(enabledisablechannels[key]));
                                        else
                                            StatisticsSetting.GetInstance().DictEnableDisableChannels["tagvertical"] = Convert.ToBoolean(enabledisablechannels[key]);
                                    }
                                }
                                else if (string.Compare(key, "statistics.enable-untag-button", true) == 0)
                                {
                                    if (enabledisablechannels[key].ToString() != null && enabledisablechannels[key].ToString() != string.Empty && ((string.Compare(enabledisablechannels[key].ToString(), "true", true) == 0) || (string.Compare(enabledisablechannels[key].ToString(), "false", true) == 0)))
                                    {
                                        statisticsCollection.ApplicationContainer.EnableUntagButton = Convert.ToBoolean(enabledisablechannels[key]);
                                        if (!StatisticsSetting.GetInstance().DictEnableDisableChannels.ContainsKey("untagbutton"))
                                            StatisticsSetting.GetInstance().DictEnableDisableChannels.Add("untagbutton", Convert.ToBoolean(enabledisablechannels[key]));
                                        else
                                            StatisticsSetting.GetInstance().DictEnableDisableChannels["untagbutton"] = Convert.ToBoolean(enabledisablechannels[key]);
                                    }
                                }
                                else if (string.Compare(key, "statistics.enable-hhmmss-format", true) == 0)
                                {
                                    if (!StatisticsSetting.GetInstance().DictEnableDisableChannels.ContainsKey("hhmmssformat"))
                                        StatisticsSetting.GetInstance().DictEnableDisableChannels.Add("hhmmssformat", Convert.ToBoolean(enabledisablechannels[key]));
                                    else
                                        StatisticsSetting.GetInstance().DictEnableDisableChannels["hhmmssformat"] = Convert.ToBoolean(enabledisablechannels[key]);

                                    statisticsCollection.ApplicationContainer.EnableHHMMSS = Convert.ToBoolean(enabledisablechannels[key]);
                                }
                                else if (string.Compare(key, "statistics.enable-submenu-alert-notification", true) == 0)
                                {
                                    if (enabledisablechannels[key].ToString() != null && enabledisablechannels[key].ToString() != string.Empty && ((string.Compare(enabledisablechannels[key].ToString(), "true", true) == 0) || (string.Compare(enabledisablechannels[key].ToString(), "false", true) == 0)))
                                    {
                                        statisticsCollection.ApplicationContainer.EnableThresholdNotification = Convert.ToBoolean(enabledisablechannels[key]);
                                        if (!StatisticsSetting.GetInstance().DictEnableDisableChannels.ContainsKey("thresholdnotification"))
                                            StatisticsSetting.GetInstance().DictEnableDisableChannels.Add("thresholdnotification", Convert.ToBoolean(enabledisablechannels[key]));
                                        else
                                            StatisticsSetting.GetInstance().DictEnableDisableChannels["thresholdnotification"] = Convert.ToBoolean(enabledisablechannels[key]);
                                    }
                                }
                                else if (string.Compare(key, "statistics.enable-system-draggable", true) == 0)
                                {
                                    statisticsCollection.ApplicationContainer.SystemDraggable = Convert.ToBoolean(enabledisablechannels[key]);
                                    StatisticsBase.GetInstance().IsSystemDraggable = Convert.ToBoolean(enabledisablechannels[key]);

                                }
                                else if (string.Compare(key, "statistics.enable-submenu-showheader", true) == 0)
                                {
                                    if (enabledisablechannels[key].ToString() != null && enabledisablechannels[key].ToString() != string.Empty && ((string.Compare(enabledisablechannels[key].ToString(), "true", true) == 0) || (string.Compare(enabledisablechannels[key].ToString(), "false", true) == 0)))
                                    {
                                        if (!StatisticsSetting.GetInstance().DictEnableDisableChannels.ContainsKey("showheader"))
                                            StatisticsSetting.GetInstance().DictEnableDisableChannels.Add("showheader", Convert.ToBoolean(enabledisablechannels[key]));
                                        else
                                            StatisticsSetting.GetInstance().DictEnableDisableChannels["showheader"] = Convert.ToBoolean(enabledisablechannels[key]);
                                    }
                                }
                                else if (string.Compare(key, "statistics.enable-header", true) == 0)
                                {
                                    if (enabledisablechannels[key].ToString() != null && enabledisablechannels[key].ToString() != string.Empty && ((string.Compare(enabledisablechannels[key].ToString(), "true", true) == 0) || (string.Compare(enabledisablechannels[key].ToString(), "false", true) == 0)))
                                    {
                                        if (!StatisticsSetting.GetInstance().DictEnableDisableChannels.ContainsKey("isheaderenabled"))
                                            StatisticsSetting.GetInstance().DictEnableDisableChannels.Add("isheaderenabled", Convert.ToBoolean(enabledisablechannels[key]));
                                        else
                                            StatisticsSetting.GetInstance().DictEnableDisableChannels["isheaderenabled"] = Convert.ToBoolean(enabledisablechannels[key]);
                                    }

                                }
                                else if (string.Compare(key, "statistics.enable-notify-primary-screen", true) == 0)
                                {
                                    if (!StatisticsSetting.GetInstance().DictEnableDisableChannels.ContainsKey("notifyprimaryscreen"))
                                        StatisticsSetting.GetInstance().DictEnableDisableChannels.Add("notifyprimaryscreen", Convert.ToBoolean(enabledisablechannels[key]));
                                    else
                                        StatisticsSetting.GetInstance().DictEnableDisableChannels["notifyprimaryscreen"] = Convert.ToBoolean(enabledisablechannels[key]);

                                }
                            }

                            #endregion
                        }
                    }
                }
                catch (Exception GeneralException)
                {
                    logger.Error("ReadApplication : GetPersonDBID Method: " + GeneralException.Message);
                }
            }

            logger.Debug("ReadApplication : GetPersonDBID Method: Exit");
            return statisticsCollection;
        }

        /// <summary>
        /// Collects the agent groups.
        /// </summary>
        /// <param name="statisticsCollection">The statistics collection.</param>
        /// <returns></returns>
        public IStatisticsCollection ReadAgentGroupDetails(IStatisticsCollection statisticsCollection, bool isPlugin)
        {
            try
            {
                logger.Debug("ReadApplication : CollectAgentGroups Method: Entry");

                StatisticsSetting instance = StatisticsSetting.GetInstance();

                CfgFolderQuery folderQuery = new CfgFolderQuery();
                folderQuery.Name = "Agent Groups";
                folderQuery.OwnerDbid = StatisticsSetting.GetInstance().CFGTenantDBID;
                CfgFolder folder = StatisticsSetting.GetInstance().confObject.RetrieveObject<CfgFolder>(folderQuery);

                ICollection<CfgAgentGroup> _agentGroup = null;
                if (!isPlugin)
                {

                    CfgAgentGroupQuery _agentGroupQuery = new CfgAgentGroupQuery();
                    _agentGroupQuery.PersonDbid = Convert.ToInt16(instance.agentDBID);
                    _agentGroupQuery.TenantDbid = StatisticsSetting.GetInstance().CFGTenantDBID;
                    _agentGroupQuery["folder_dbid"] = folder.DBID;
                    _agentGroup = StatisticsSetting.GetInstance().confObject.RetrieveMultipleObjects<CfgAgentGroup>(_agentGroupQuery);

                    if (_agentGroup.Count != 0)
                    {
                        foreach (CfgAgentGroup agentgroup in _agentGroup)
                        {
                            StatisticsSetting.GetInstance().AgentGroupCollection.Add(agentgroup);
                        }
                    }

                }

                foreach (CfgAgentGroup agentGroup in StatisticsSetting.GetInstance().AgentGroupCollection)
                {
                    if (agentGroup != null)
                    {
                        StatisticsSetting.GetInstance().AgentGroupsListCollections.Add(agentGroup.GroupInfo.Name.ToString());

                        KeyValueCollection serverDetails = (KeyValueCollection)agentGroup.GroupInfo.UserProperties["agent.ixn.desktop"];

                        if (serverDetails != null)
                        {
                            foreach (string key in serverDetails.AllKeys)
                            {
                                #region AgentGroup Statistics Configuration

                                if (key.StartsWith("agent-group-statistics"))
                                {
                                    if (string.IsNullOrEmpty(statisticsCollection.StatisticsCommon.AgentGroupStatistics))
                                        statisticsCollection.StatisticsCommon.AgentGroupStatistics = Convert.ToString(serverDetails[key]);
                                    else
                                        statisticsCollection.StatisticsCommon.AgentGroupStatistics = statisticsCollection.StatisticsCommon.AgentGroupStatistics + "," + Convert.ToString(serverDetails[key]);
                                }
                                else if (key.StartsWith("agent-statistics"))
                                {
                                    if (string.IsNullOrEmpty(statisticsCollection.StatisticsCommon.AgentStatistics))
                                        statisticsCollection.StatisticsCommon.AgentStatistics = Convert.ToString(serverDetails[key]);
                                    else

                                        statisticsCollection.StatisticsCommon.AgentStatistics = statisticsCollection.StatisticsCommon.AgentStatistics + "," + Convert.ToString(serverDetails[key]);

                                }
                                else if (key.StartsWith("virtual-queue-statistics"))
                                {
                                    if (string.IsNullOrEmpty(statisticsCollection.StatisticsCommon.VQueueStatistics))
                                        statisticsCollection.StatisticsCommon.VQueueStatistics = Convert.ToString(serverDetails[key]);
                                    else
                                        statisticsCollection.StatisticsCommon.VQueueStatistics = statisticsCollection.StatisticsCommon.VQueueStatistics + "," + Convert.ToString(serverDetails[key]);

                                }
                                else if (key.StartsWith("acd-queue-statistics"))
                                {
                                    if (string.IsNullOrEmpty(statisticsCollection.StatisticsCommon.ACDStatistics))
                                        statisticsCollection.StatisticsCommon.ACDStatistics = Convert.ToString(serverDetails[key]);
                                    else

                                        statisticsCollection.StatisticsCommon.ACDStatistics = statisticsCollection.StatisticsCommon.ACDStatistics + "," + Convert.ToString(serverDetails[key]);

                                }
                                else if (key.StartsWith("dn-group-statistics"))
                                {
                                    if (string.IsNullOrEmpty(statisticsCollection.StatisticsCommon.DNGroupStatistics))
                                        statisticsCollection.StatisticsCommon.DNGroupStatistics = Convert.ToString(serverDetails[key]);
                                    else
                                        statisticsCollection.StatisticsCommon.DNGroupStatistics = statisticsCollection.StatisticsCommon.DNGroupStatistics + "," + Convert.ToString(serverDetails[key]);

                                }
                                else if (key.StartsWith("statistics.objects-acd-queues"))
                                {
                                    if (string.IsNullOrEmpty(statisticsCollection.StatisticsCommon.ACDObjects))
                                        statisticsCollection.StatisticsCommon.ACDObjects = Convert.ToString(serverDetails[key]);
                                    else
                                        statisticsCollection.StatisticsCommon.ACDObjects = statisticsCollection.StatisticsCommon.ACDObjects + "," + Convert.ToString(serverDetails[key]);

                                }
                                else if (key.StartsWith("statistics.objects-dn-groups"))
                                {
                                    if (string.IsNullOrEmpty(statisticsCollection.StatisticsCommon.DNGroupObjects))
                                        statisticsCollection.StatisticsCommon.DNGroupObjects = Convert.ToString(serverDetails[key]);
                                    else

                                        statisticsCollection.StatisticsCommon.DNGroupObjects = statisticsCollection.StatisticsCommon.DNGroupObjects + "," + Convert.ToString(serverDetails[key]);

                                }
                                else if (key.StartsWith("statistics.objects-virtual-queues"))
                                {
                                    if (string.IsNullOrEmpty(statisticsCollection.StatisticsCommon.VQueueObjects))
                                        statisticsCollection.StatisticsCommon.VQueueObjects = Convert.ToString(serverDetails[key]);
                                    else
                                        statisticsCollection.StatisticsCommon.VQueueObjects = statisticsCollection.StatisticsCommon.VQueueObjects + "," + Convert.ToString(serverDetails[key]);

                                }

                                #endregion
                            }
                        }
                        KeyValueCollection enabledisablechannels = (KeyValueCollection)agentGroup.GroupInfo.UserProperties["enable-disable-channels"];
                        if (enabledisablechannels != null)
                        {
                            #region EnableDisableChannels

                            foreach (string key in enabledisablechannels.AllKeys)
                            {
                                if (string.Compare(key, "statistics.enable-alwaysontop", true) == 0)
                                {
                                    if (enabledisablechannels[key].ToString() != null && enabledisablechannels[key].ToString() != string.Empty && ((string.Compare(enabledisablechannels[key].ToString(), "true", true) == 0) || (string.Compare(enabledisablechannels[key].ToString(), "false", true) == 0)))
                                    {
                                        statisticsCollection.ApplicationContainer.EnableAlwaysOnTop = Convert.ToBoolean(enabledisablechannels[key]);
                                        if (!StatisticsSetting.GetInstance().DictEnableDisableChannels.ContainsKey("AlwaysOnTop"))
                                            StatisticsSetting.GetInstance().DictEnableDisableChannels.Add("AlwaysOnTop", Convert.ToBoolean(enabledisablechannels[key]));
                                        else
                                            StatisticsSetting.GetInstance().DictEnableDisableChannels["AlwaysOnTop"] = Convert.ToBoolean(enabledisablechannels[key]);
                                    }
                                }
                                else if (string.Compare(key, "statistics.enable-ccstat-aid", true) == 0)
                                {
                                    if (enabledisablechannels[key].ToString() != null && enabledisablechannels[key].ToString() != string.Empty && ((string.Compare(enabledisablechannels[key].ToString(), "true", true) == 0) || (string.Compare(enabledisablechannels[key].ToString(), "false", true) == 0)))
                                    {
                                        statisticsCollection.ApplicationContainer.EnableCCStatAID = Convert.ToBoolean(enabledisablechannels[key]);
                                        if (!StatisticsSetting.GetInstance().DictEnableDisableChannels.ContainsKey("ccstataid"))
                                            StatisticsSetting.GetInstance().DictEnableDisableChannels.Add("ccstataid", Convert.ToBoolean(enabledisablechannels[key]));
                                        else
                                            StatisticsSetting.GetInstance().DictEnableDisableChannels["ccstataid"] = Convert.ToBoolean(enabledisablechannels[key]);
                                    }
                                }
                                else if (string.Compare(key, "statistics.enable-log", true) == 0)
                                {
                                    if (enabledisablechannels[key].ToString() != null && enabledisablechannels[key].ToString() != string.Empty && ((string.Compare(enabledisablechannels[key].ToString(), "true", true) == 0) || (string.Compare(enabledisablechannels[key].ToString(), "false", true) == 0)))
                                    {
                                        statisticsCollection.ApplicationContainer.EnableLog = Convert.ToBoolean(enabledisablechannels[key]);
                                        if (!StatisticsSetting.GetInstance().DictEnableDisableChannels.ContainsKey("log"))
                                            StatisticsSetting.GetInstance().DictEnableDisableChannels.Add("log", Convert.ToBoolean(enabledisablechannels[key]));
                                        else
                                            StatisticsSetting.GetInstance().DictEnableDisableChannels["log"] = Convert.ToBoolean(enabledisablechannels[key]);
                                    }
                                }
                                else if (string.Compare(key, "statistics.enable-maingadget", true) == 0)
                                {
                                    if (enabledisablechannels[key].ToString() != null && enabledisablechannels[key].ToString() != string.Empty && ((string.Compare(enabledisablechannels[key].ToString(), "true", true) == 0) || (string.Compare(enabledisablechannels[key].ToString(), "false", true) == 0)))
                                    {
                                        statisticsCollection.ApplicationContainer.EnableMainGadget = Convert.ToBoolean(enabledisablechannels[key]);
                                        if (!StatisticsSetting.GetInstance().DictEnableDisableChannels.ContainsKey("maingadget"))
                                            StatisticsSetting.GetInstance().DictEnableDisableChannels.Add("maingadget", Convert.ToBoolean(enabledisablechannels[key]));
                                        else
                                            StatisticsSetting.GetInstance().DictEnableDisableChannels["maingadget"] = Convert.ToBoolean(enabledisablechannels[key]);
                                    }
                                }
                                else if (string.Compare(key, "statistics.enable-menu-button", true) == 0)
                                {
                                    if (enabledisablechannels[key].ToString() != null && enabledisablechannels[key].ToString() != string.Empty && ((string.Compare(enabledisablechannels[key].ToString(), "true", true) == 0) || (string.Compare(enabledisablechannels[key].ToString(), "false", true) == 0)))
                                    {
                                        statisticsCollection.ApplicationContainer.EnableMenuButton = Convert.ToBoolean(enabledisablechannels[key]);
                                        if (!StatisticsSetting.GetInstance().DictEnableDisableChannels.ContainsKey("menubutton"))
                                            StatisticsSetting.GetInstance().DictEnableDisableChannels.Add("menubutton", Convert.ToBoolean(enabledisablechannels[key]));
                                        else
                                            StatisticsSetting.GetInstance().DictEnableDisableChannels["menubutton"] = Convert.ToBoolean(enabledisablechannels[key]);
                                    }
                                }
                                else if (string.Compare(key, "statistics.enable-mystat-aid", true) == 0)
                                {
                                    if (enabledisablechannels[key].ToString() != null && enabledisablechannels[key].ToString() != string.Empty && ((string.Compare(enabledisablechannels[key].ToString(), "true", true) == 0) || (string.Compare(enabledisablechannels[key].ToString(), "false", true) == 0)))
                                    {
                                        statisticsCollection.ApplicationContainer.EnableMyStatAID = Convert.ToBoolean(enabledisablechannels[key]);
                                        if (!StatisticsSetting.GetInstance().DictEnableDisableChannels.ContainsKey("mystataid"))
                                            StatisticsSetting.GetInstance().DictEnableDisableChannels.Add("mystataid", Convert.ToBoolean(enabledisablechannels[key]));
                                        else
                                            StatisticsSetting.GetInstance().DictEnableDisableChannels["mystataid"] = Convert.ToBoolean(enabledisablechannels[key]);
                                    }
                                }
                                else if (string.Compare(key, "statistics.enable-notification-close", true) == 0)
                                {
                                    if (enabledisablechannels[key].ToString() != null && enabledisablechannels[key].ToString() != string.Empty && ((string.Compare(enabledisablechannels[key].ToString(), "true", true) == 0) || (string.Compare(enabledisablechannels[key].ToString(), "false", true) == 0)))
                                    {
                                        statisticsCollection.ApplicationContainer.EnableTaskbarClose = Convert.ToBoolean(enabledisablechannels[key]);
                                        if (!StatisticsSetting.GetInstance().DictEnableDisableChannels.ContainsKey("notificationclose"))
                                            StatisticsSetting.GetInstance().DictEnableDisableChannels.Add("notificationclose", Convert.ToBoolean(enabledisablechannels[key]));
                                        else
                                            StatisticsSetting.GetInstance().DictEnableDisableChannels["notificationclose"] = Convert.ToBoolean(enabledisablechannels[key]);
                                    }
                                }
                                else if (string.Compare(key, "statistics.enable-notification-balloon", true) == 0)
                                {
                                    if (enabledisablechannels[key].ToString() != null && enabledisablechannels[key].ToString() != string.Empty && ((string.Compare(enabledisablechannels[key].ToString(), "true", true) == 0) || (string.Compare(enabledisablechannels[key].ToString(), "false", true) == 0)))
                                    {
                                        statisticsCollection.ApplicationContainer.EnableNotificationBalloon = Convert.ToBoolean(enabledisablechannels[key]);
                                        if (!StatisticsSetting.GetInstance().DictEnableDisableChannels.ContainsKey("notificationballoon"))
                                            StatisticsSetting.GetInstance().DictEnableDisableChannels.Add("notificationballoon", Convert.ToBoolean(enabledisablechannels[key]));
                                        else
                                            StatisticsSetting.GetInstance().DictEnableDisableChannels["notificationballoon"] = Convert.ToBoolean(enabledisablechannels[key]);
                                    }
                                }
                                else if (string.Compare(key, "statistics.enable-submenu-ccstatistics", true) == 0)
                                {
                                    if (enabledisablechannels[key].ToString() != null && enabledisablechannels[key].ToString() != string.Empty && ((string.Compare(enabledisablechannels[key].ToString(), "true", true) == 0) || (string.Compare(enabledisablechannels[key].ToString(), "false", true) == 0)))
                                    {
                                        statisticsCollection.ApplicationContainer.EnableMenuShowCCStat = Convert.ToBoolean(enabledisablechannels[key]);
                                        if (!StatisticsSetting.GetInstance().DictEnableDisableChannels.ContainsKey("ccstatistics"))
                                            StatisticsSetting.GetInstance().DictEnableDisableChannels.Add("ccstatistics", Convert.ToBoolean(enabledisablechannels[key]));
                                        else
                                            StatisticsSetting.GetInstance().DictEnableDisableChannels["ccstatistics"] = Convert.ToBoolean(enabledisablechannels[key]);
                                    }
                                }
                                else if (string.Compare(key, "statistics.enable-submenu-close-gadget", true) == 0)
                                {
                                    if (enabledisablechannels[key].ToString() != null && enabledisablechannels[key].ToString() != string.Empty && ((string.Compare(enabledisablechannels[key].ToString(), "true", true) == 0) || (string.Compare(enabledisablechannels[key].ToString(), "false", true) == 0)))
                                    {
                                        statisticsCollection.ApplicationContainer.EnableMenuClose = Convert.ToBoolean(enabledisablechannels[key]);
                                        if (!StatisticsSetting.GetInstance().DictEnableDisableChannels.ContainsKey("closegadget"))
                                            StatisticsSetting.GetInstance().DictEnableDisableChannels.Add("closegadget", Convert.ToBoolean(enabledisablechannels[key]));
                                        else
                                            StatisticsSetting.GetInstance().DictEnableDisableChannels["closegadget"] = Convert.ToBoolean(enabledisablechannels[key]);
                                    }
                                }
                                else if (string.Compare(key, "statistics.enable-submenu-mystatistics", true) == 0)
                                {
                                    if (enabledisablechannels[key].ToString() != null && enabledisablechannels[key].ToString() != string.Empty && ((string.Compare(enabledisablechannels[key].ToString(), "true", true) == 0) || (string.Compare(enabledisablechannels[key].ToString(), "false", true) == 0)))
                                    {
                                        statisticsCollection.ApplicationContainer.EnableMenuShowMyStat = Convert.ToBoolean(enabledisablechannels[key]);
                                        if (!StatisticsSetting.GetInstance().DictEnableDisableChannels.ContainsKey("mystatistics"))
                                            StatisticsSetting.GetInstance().DictEnableDisableChannels.Add("mystatistics", Convert.ToBoolean(enabledisablechannels[key]));
                                        else
                                            StatisticsSetting.GetInstance().DictEnableDisableChannels["mystatistics"] = Convert.ToBoolean(enabledisablechannels[key]);
                                    }
                                }
                                else if (string.Compare(key, "statistics.enable-submenu-ontop", true) == 0)
                                {
                                    if (enabledisablechannels[key].ToString() != null && enabledisablechannels[key].ToString() != string.Empty && ((string.Compare(enabledisablechannels[key].ToString(), "true", true) == 0) || (string.Compare(enabledisablechannels[key].ToString(), "false", true) == 0)))
                                    {
                                        statisticsCollection.ApplicationContainer.EnableMenuOnTop = Convert.ToBoolean(enabledisablechannels[key]);
                                        if (!StatisticsSetting.GetInstance().DictEnableDisableChannels.ContainsKey("ontop"))
                                            StatisticsSetting.GetInstance().DictEnableDisableChannels.Add("ontop", Convert.ToBoolean(enabledisablechannels[key]));
                                        else
                                            StatisticsSetting.GetInstance().DictEnableDisableChannels["ontop"] = Convert.ToBoolean(enabledisablechannels[key]);
                                    }
                                }
                                else if (string.Compare(key, "statistics.enable-tag-button", true) == 0)
                                {
                                    if (enabledisablechannels[key].ToString() != null && enabledisablechannels[key].ToString() != string.Empty && ((string.Compare(enabledisablechannels[key].ToString(), "true", true) == 0) || (string.Compare(enabledisablechannels[key].ToString(), "false", true) == 0)))
                                    {
                                        statisticsCollection.ApplicationContainer.EnableTagButton = Convert.ToBoolean(enabledisablechannels[key]);
                                        if (!StatisticsSetting.GetInstance().DictEnableDisableChannels.ContainsKey("tagbutton"))
                                            StatisticsSetting.GetInstance().DictEnableDisableChannels.Add("tagbutton", Convert.ToBoolean(enabledisablechannels[key]));
                                        else
                                            StatisticsSetting.GetInstance().DictEnableDisableChannels["tagbutton"] = Convert.ToBoolean(enabledisablechannels[key]);
                                    }
                                }
                                else if (string.Compare(key, "statistics.enable-tag-vertical", true) == 0)
                                {
                                    if (enabledisablechannels[key].ToString() != null && enabledisablechannels[key].ToString() != string.Empty && ((string.Compare(enabledisablechannels[key].ToString(), "true", true) == 0) || (string.Compare(enabledisablechannels[key].ToString(), "false", true) == 0)))
                                    {
                                        statisticsCollection.ApplicationContainer.EnableTagVertical = Convert.ToBoolean(enabledisablechannels[key]);
                                        if (!StatisticsSetting.GetInstance().DictEnableDisableChannels.ContainsKey("tagvertical"))
                                            StatisticsSetting.GetInstance().DictEnableDisableChannels.Add("tagvertical", Convert.ToBoolean(enabledisablechannels[key]));
                                        else
                                            StatisticsSetting.GetInstance().DictEnableDisableChannels["tagvertical"] = Convert.ToBoolean(enabledisablechannels[key]);
                                    }
                                }
                                else if (string.Compare(key, "statistics.enable-untag-button", true) == 0)
                                {
                                    if (enabledisablechannels[key].ToString() != null && enabledisablechannels[key].ToString() != string.Empty && ((string.Compare(enabledisablechannels[key].ToString(), "true", true) == 0) || (string.Compare(enabledisablechannels[key].ToString(), "false", true) == 0)))
                                    {
                                        statisticsCollection.ApplicationContainer.EnableUntagButton = Convert.ToBoolean(enabledisablechannels[key]);
                                        if (!StatisticsSetting.GetInstance().DictEnableDisableChannels.ContainsKey("untagbutton"))
                                            StatisticsSetting.GetInstance().DictEnableDisableChannels.Add("untagbutton", Convert.ToBoolean(enabledisablechannels[key]));
                                        else
                                            StatisticsSetting.GetInstance().DictEnableDisableChannels["untagbutton"] = Convert.ToBoolean(enabledisablechannels[key]);
                                    }
                                }
                                else if (string.Compare(key, "statistics.enable-hhmmss-format", true) == 0)
                                {
                                    if (!StatisticsSetting.GetInstance().DictEnableDisableChannels.ContainsKey("hhmmssformat"))
                                        StatisticsSetting.GetInstance().DictEnableDisableChannels.Add("hhmmssformat", Convert.ToBoolean(enabledisablechannels[key]));
                                    else
                                        StatisticsSetting.GetInstance().DictEnableDisableChannels["hhmmssformat"] = Convert.ToBoolean(enabledisablechannels[key]);

                                    statisticsCollection.ApplicationContainer.EnableHHMMSS = Convert.ToBoolean(enabledisablechannels[key]);
                                }
                                else if (string.Compare(key, "statistics.enable-submenu-alert-notification", true) == 0)
                                {
                                    if (enabledisablechannels[key].ToString() != null && enabledisablechannels[key].ToString() != string.Empty && ((string.Compare(enabledisablechannels[key].ToString(), "true", true) == 0) || (string.Compare(enabledisablechannels[key].ToString(), "false", true) == 0)))
                                    {
                                        statisticsCollection.ApplicationContainer.EnableThresholdNotification = Convert.ToBoolean(enabledisablechannels[key]);
                                        if (!StatisticsSetting.GetInstance().DictEnableDisableChannels.ContainsKey("thresholdnotification"))
                                            StatisticsSetting.GetInstance().DictEnableDisableChannels.Add("thresholdnotification", Convert.ToBoolean(enabledisablechannels[key]));
                                        else
                                            StatisticsSetting.GetInstance().DictEnableDisableChannels["thresholdnotification"] = Convert.ToBoolean(enabledisablechannels[key]);
                                    }
                                }
                                else if (string.Compare(key, "statistics.enable-system-draggable", true) == 0)
                                {
                                    statisticsCollection.ApplicationContainer.SystemDraggable = Convert.ToBoolean(enabledisablechannels[key]);
                                    StatisticsBase.GetInstance().IsSystemDraggable = Convert.ToBoolean(enabledisablechannels[key]);

                                }
                                else if (string.Compare(key, "statistics.enable-submenu-showheader", true) == 0)
                                {
                                    if (enabledisablechannels[key].ToString() != null && enabledisablechannels[key].ToString() != string.Empty && ((string.Compare(enabledisablechannels[key].ToString(), "true", true) == 0) || (string.Compare(enabledisablechannels[key].ToString(), "false", true) == 0)))
                                    {
                                        if (!StatisticsSetting.GetInstance().DictEnableDisableChannels.ContainsKey("showheader"))
                                            StatisticsSetting.GetInstance().DictEnableDisableChannels.Add("showheader", Convert.ToBoolean(enabledisablechannels[key]));
                                        else
                                            StatisticsSetting.GetInstance().DictEnableDisableChannels["showheader"] = Convert.ToBoolean(enabledisablechannels[key]);
                                    }
                                }
                                else if (string.Compare(key, "statistics.enable-notify-primary-screen", true) == 0)
                                {
                                    if (!StatisticsSetting.GetInstance().DictEnableDisableChannels.ContainsKey("notifyprimaryscreen"))
                                        StatisticsSetting.GetInstance().DictEnableDisableChannels.Add("notifyprimaryscreen", Convert.ToBoolean(enabledisablechannels[key]));
                                    else
                                        StatisticsSetting.GetInstance().DictEnableDisableChannels["notifyprimaryscreen"] = Convert.ToBoolean(enabledisablechannels[key]);

                                }
                            }

                            #endregion
                        }
                    }
                }

                //StatisticsSetting.GetInstance().DictACDDisplays.Clear();
                string[] ACDQueueObjects = StatisticsSetting.GetInstance().statisticsCollection.StatisticsCommon.ACDObjects.Split(',');

                foreach (string ACDs in ACDQueueObjects)
                {
                    string Dilimitor = "_@";
                    string[] ACDDisplayObjects = ACDs.Split(new[] { Dilimitor }, StringSplitOptions.None);

                    if (ACDDisplayObjects.Length > 1)
                    {
                        if (!StatisticsSetting.GetInstance().DictACDDisplays.ContainsKey(ACDDisplayObjects[0].ToString()))
                            StatisticsSetting.GetInstance().DictACDDisplays.Add(ACDDisplayObjects[0].ToString(), ACDDisplayObjects[1].ToString());
                    }
                }

                instance = null;
            }
            catch (Exception generalException)
            {
                logger.Error("ReadApplication : CollectAgentGroups Method: " + generalException.Message.ToString());
            }
            finally
            {
                logger.Debug("ReadApplication : CollectAgentGroups Method: Exit");
                GC.Collect();
            }

            return statisticsCollection;
        }

        /// <summary>
        /// Reads the application details.
        /// </summary>
        /// <param name="applicationName">Name of the application.</param>
        /// <returns></returns>
        public void ReadApplicationDetails(string applicationName, string source)
        {
            #region Check Null Reference

            if (StatisticsSetting.GetInstance().statisticsCollection == null)
            {
                StatisticsSetting.GetInstance().statisticsCollection = new StatisticsCollection();
            }

            if (StatisticsSetting.GetInstance().statisticsCollection.StatisticsCommon == null)
            {
                StatisticsSetting.GetInstance().statisticsCollection.StatisticsCommon = new StatisticsCommon();
            }

            if (StatisticsSetting.GetInstance().statisticsCollection.AgentStatistics == null)
            {
                StatisticsSetting.GetInstance().statisticsCollection.AgentStatistics = new List<IAgentStatistics>();
            }

            if (StatisticsSetting.GetInstance().statisticsCollection.AgentGroupStatistics == null)
            {
                StatisticsSetting.GetInstance().statisticsCollection.AgentGroupStatistics = new List<IAgentGroupStatistics>();
            }

            if (StatisticsSetting.GetInstance().statisticsCollection.ACDQueueStatistics == null)
            {
                StatisticsSetting.GetInstance().statisticsCollection.ACDQueueStatistics = new List<IACDStatistics>();
            }

            if (StatisticsSetting.GetInstance().statisticsCollection.DNGroupStatistics == null)
            {
                StatisticsSetting.GetInstance().statisticsCollection.DNGroupStatistics = new List<IDNGroupStatistics>();
            }

            if (StatisticsSetting.GetInstance().statisticsCollection.VirtualQueueStatistics == null)
            {
                StatisticsSetting.GetInstance().statisticsCollection.VirtualQueueStatistics = new List<IVirtualQueueStatistics>();
            }

            if (StatisticsSetting.GetInstance().statisticsCollection.StatisticsLocalSetting == null)
            {
                StatisticsSetting.GetInstance().statisticsCollection.StatisticsLocalSetting = new StatisticsLocalSetting();
            }

            #endregion

            try
            {
                logger.Debug("ReadApplication : ReadApplicationDetails Method: Entry");

                CfgApplication application = new CfgApplication(StatisticsSetting.GetInstance().confObject);
                CfgApplicationQuery queryApp = new CfgApplicationQuery();
                queryApp.Name = applicationName;
                application = StatisticsSetting.GetInstance().confObject.RetrieveObject<CfgApplication>(queryApp);
                if (application != null)
                {
                    StatisticsSetting.GetInstance().IsApplicationFound = true;

                    #region Application Options

                    KeyValueCollection serverDetails = (KeyValueCollection)application.Options["agent.ixn.desktop"];

                    if (serverDetails != null)
                    {
                        #region Application Statistics Configuration

                        StatisticsCommon commonStatistics = (StatisticsCommon)StatisticsSetting.GetInstance().statisticsCollection.StatisticsCommon;
                        StatisticsLocalSetting localSettings = new StatisticsLocalSetting();

                        commonStatistics.Source = source;

                        if (commonStatistics.Source != null && commonStatistics.Source != "")
                        {

                            int servercount = 0;
                            CfgApplication Servers;
                            if (application.AppServers.Count > 0)
                            {

                                foreach (CfgConnInfo appDetails in application.AppServers)
                                {
                                    if (appDetails.AppServer.Type == CfgAppType.CFGStatServer)
                                    {
                                        if (localSettings.PrimaryStatServer == null)
                                        {
                                            localSettings.PrimaryStatServer = new List<CfgApplication>();
                                        }
                                        servercount++;
                                        Servers = new CfgApplication(StatisticsSetting.GetInstance().confObject);
                                        CfgApplicationQuery queryServer = new CfgApplicationQuery();
                                        queryServer.Name = appDetails.AppServer.Name;
                                        Servers = StatisticsSetting.GetInstance().confObject.RetrieveObject<CfgApplication>(queryServer);
                                        if (Servers != null)
                                        {
                                            localSettings.PrimaryStatServer.Add(Servers);
                                        }
                                    }
                                }
                            }

                            foreach (string key in serverDetails.AllKeys)
                            {

                                if (commonStatistics.Source == StatisticsEnum.StatSource.StatServer.ToString() || commonStatistics.Source == StatisticsEnum.StatSource.All.ToString())
                                {
                                    #region Read Server Details

                                    if (string.Compare(key, "statistics.primary-server-name", true) == 0)
                                    {
                                        if ((servercount == 0) && (localSettings.PrimaryStatServer == null))
                                        {
                                            #region Primary Server

                                            if (serverDetails[key] != "")
                                            {
                                                string[] serverName = Convert.ToString(serverDetails[key]).Split(',');
                                                foreach (string Sname in serverName)
                                                {
                                                    Servers = new CfgApplication(StatisticsSetting.GetInstance().confObject);
                                                    CfgApplicationQuery queryServer = new CfgApplicationQuery();
                                                    queryServer.Name = Sname;
                                                    Servers = StatisticsSetting.GetInstance().confObject.RetrieveObject<CfgApplication>(queryServer);

                                                    if (Servers != null)
                                                    {
                                                        if (Servers.Type == CfgAppType.CFGStatServer)
                                                        {
                                                            if (localSettings.PrimaryStatServer == null)
                                                            {
                                                                localSettings.PrimaryStatServer = new List<CfgApplication>();
                                                                localSettings.PrimaryStatServer.Add(Servers);
                                                            }
                                                            else
                                                            {
                                                                localSettings.PrimaryStatServer.Add(Servers);
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {

                                                        string[] url = Sname.Split(':');
                                                        if (url.Length == 2)
                                                        {
                                                            try
                                                            {

                                                                System.Net.Sockets.TcpClient client = new System.Net.Sockets.TcpClient();
                                                                client.Connect(url[0].Trim(), Convert.ToInt32(url[1].Trim()));
                                                                if (client.Connected)
                                                                {
                                                                    logger.Debug("ReadApplication : ReadApplicationDetails Tcp connection established");
                                                                    client.Close();
                                                                    string tempUri = "tcp://" + Sname;
                                                                    Uri myUri;
                                                                    if (Uri.TryCreate(tempUri, UriKind.RelativeOrAbsolute, out myUri))
                                                                    {
                                                                        if (localSettings.PrimaryStatServerUri == null)
                                                                        {
                                                                            localSettings.PrimaryStatServerUri = new List<Uri>();
                                                                            localSettings.PrimaryStatServerUri.Add(myUri);
                                                                            logger.Debug("ReadApplication : ReadApplicationDetails PrimaryStatServerUri added");
                                                                        }
                                                                        else
                                                                        {
                                                                            localSettings.PrimaryStatServerUri.Add(myUri);
                                                                            logger.Debug("ReadApplication : ReadApplicationDetails PrimaryStatServerUri added");
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                            catch (Exception generalException)
                                                            {

                                                                logger.Error("ReadApplication : ReadApplicationDetails Tcp connection Exception:" + generalException.Message.ToString());
                                                            }
                                                        }

                                                    }
                                                }

                                            }
                                            else
                                            {
                                                localSettings.PrimaryStatServer = null;
                                                localSettings.PrimaryStatServerUri = null;
                                            }

                                            #endregion
                                        }
                                    }
                                    else if (string.Compare(key, "statistics.secondary-server-name", true) == 0)
                                    {
                                        #region Secondary Server
                                        if ((servercount == 0) && (localSettings.PrimaryStatServer == null) && (localSettings.PrimaryStatServerUri == null))
                                        {
                                            #region Secondary Server

                                            if (serverDetails[key] != "")
                                            {
                                                string[] serverName = Convert.ToString(serverDetails[key]).Split(',');
                                                foreach (string Sname in serverName)
                                                {
                                                    Servers = new CfgApplication(StatisticsSetting.GetInstance().confObject);
                                                    CfgApplicationQuery queryServer = new CfgApplicationQuery();
                                                    queryServer.Name = Sname;
                                                    Servers = StatisticsSetting.GetInstance().confObject.RetrieveObject<CfgApplication>(queryServer);

                                                    if (Servers != null)
                                                    {
                                                        if (Servers.Type == CfgAppType.CFGStatServer)
                                                        {
                                                            if (localSettings.PrimaryStatServer == null)
                                                            {
                                                                localSettings.PrimaryStatServer = new List<CfgApplication>();
                                                                localSettings.PrimaryStatServer.Add(Servers);
                                                            }
                                                            else
                                                            {
                                                                localSettings.PrimaryStatServer.Add(Servers);
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {

                                                        string[] url = Sname.Split(':');
                                                        if (url.Length == 2)
                                                        {
                                                            try
                                                            {

                                                                System.Net.Sockets.TcpClient client = new System.Net.Sockets.TcpClient();
                                                                client.Connect(url[0].Trim(), Convert.ToInt32(url[1].Trim()));
                                                                if (client.Connected)
                                                                {
                                                                    logger.Debug("ReadApplication : ReadApplicationDetails Tcp connection established");
                                                                    client.Close();
                                                                    string tempUri = "tcp://" + Sname;
                                                                    Uri myUri;
                                                                    if (Uri.TryCreate(tempUri, UriKind.RelativeOrAbsolute, out myUri))
                                                                    {
                                                                        if (localSettings.PrimaryStatServerUri == null)
                                                                        {
                                                                            localSettings.PrimaryStatServerUri = new List<Uri>();
                                                                            localSettings.PrimaryStatServerUri.Add(myUri);
                                                                            logger.Debug("ReadApplication : ReadApplicationDetails PrimaryStatServerUri added");
                                                                        }
                                                                        else
                                                                        {
                                                                            localSettings.PrimaryStatServerUri.Add(myUri);
                                                                            logger.Debug("ReadApplication : ReadApplicationDetails PrimaryStatServerUri added");
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                            catch (Exception generalException)
                                                            {

                                                                logger.Error("ReadApplication : ReadApplicationDetails Tcp connection Exception:" + generalException.Message.ToString());
                                                            }
                                                        }
                                                    }
                                                }

                                            }
                                            else
                                            {
                                                localSettings.PrimaryStatServer = null;
                                                localSettings.PrimaryStatServerUri = null;
                                            }

                                            #endregion
                                        }

                                        #endregion
                                    }
                                    else if (string.Compare(key, "statistics.insensitivity", true) == 0)
                                    {
                                        #region Insensitivity

                                        commonStatistics.Insensitivity = Convert.ToInt32(serverDetails[key]);

                                        #endregion
                                    }
                                    else if (string.Compare(key, "statistics.notify-seconds", true) == 0)
                                    {
                                        #region Notify Seconds

                                        commonStatistics.NotifySeconds = Convert.ToInt32(serverDetails[key]);

                                        #endregion
                                    }
                                    else if (string.Compare(key, "statistics.tenant-name", true) == 0)
                                    {
                                        #region Tenant Name

                                        commonStatistics.TenantName = Convert.ToString(serverDetails[key]);

                                        CfgTenantQuery tenantQuery = new CfgTenantQuery();
                                        tenantQuery.Name = commonStatistics.TenantName;
                                        StatisticsSetting.GetInstance().CFGTenantDBID = StatisticsSetting.GetInstance().confObject.RetrieveObject<CfgTenant>(tenantQuery).DBID;

                                        #endregion
                                    }
                                    else if (key.StartsWith("agent-group-statistics"))
                                    {
                                        commonStatistics.AgentGroupStatistics = Convert.ToString(serverDetails[key]);
                                    }
                                    else if (key.StartsWith("agent-statistics"))
                                    {
                                        commonStatistics.AgentStatistics = Convert.ToString(serverDetails[key]);
                                    }
                                    else if (key.StartsWith("virtual-queue-statistics"))
                                    {
                                        commonStatistics.VQueueStatistics = Convert.ToString(serverDetails[key]);
                                    }
                                    else if (key.StartsWith("acd-queue-statistics"))
                                    {
                                        commonStatistics.ACDStatistics = Convert.ToString(serverDetails[key]);
                                    }
                                    else if (key.StartsWith("dn-group-statistics"))
                                    {
                                        commonStatistics.DNGroupStatistics = Convert.ToString(serverDetails[key]);
                                    }
                                    else if (key.StartsWith("statistics.objects-acd-queues"))
                                    {
                                        commonStatistics.ACDObjects = Convert.ToString(serverDetails[key]);
                                    }
                                    else if (key.StartsWith("statistics.objects-dn-groups"))
                                    {
                                        commonStatistics.DNGroupObjects = Convert.ToString(serverDetails[key]);
                                    }
                                    else if (key.StartsWith("statistics.objects-virtual-queues"))
                                    {
                                        commonStatistics.VQueueObjects = Convert.ToString(serverDetails[key]);
                                    }
                                    else if (string.Compare(key, "statistics.alert.audio-path", true) == 0)
                                    {
                                        StatisticsBase.GetInstance().ThresholdBreachAlertPath = Convert.ToString(serverDetails[key]);
                                        commonStatistics.AudioPath = Convert.ToString(serverDetails[key]);
                                    }
                                    else if (string.Compare(key, "statistics.alert.popup.screen-color", true) == 0)
                                    {
                                        if (Convert.ToString(serverDetails[key]).Contains(','))
                                        {
                                            string[] Backgrounds = Convert.ToString(serverDetails[key]).Split(',');
                                            StatisticsBase.GetInstance().ThresholdTitleBackground = Backgrounds[0].ToString();
                                            StatisticsBase.GetInstance().ThresholdContentBackground = Backgrounds[1].ToString();
                                        }
                                        else
                                        {
                                            StatisticsBase.GetInstance().ThresholdTitleBackground = Convert.ToString(serverDetails[key]);
                                            StatisticsBase.GetInstance().ThresholdContentBackground = "White";
                                        }

                                        commonStatistics.NotifyBackground = Convert.ToString(serverDetails[key]);
                                    }
                                    else if (string.Compare(key, "statistics.alert.popup.font-color", true) == 0)
                                    {
                                        StatisticsBase.GetInstance().ThresholdContentForeground = Convert.ToString(serverDetails[key]);
                                        commonStatistics.NotifyForeground = Convert.ToString(serverDetails[key]);
                                    }
                                    else if (string.Compare(key, "voice.my-queue-object-type", true) == 0)
                                    {
                                        commonStatistics.QueueObjectType = serverDetails[key].ToString();
                                    }
                                    else if (string.Compare(key, "voice.my-queue-switches", true) == 0)
                                    {
                                        if (serverDetails[key].ToString() != string.Empty)
                                        {
                                            commonStatistics.Switches = serverDetails[key].ToString();
                                        }
                                    }
                                    else if (string.Compare(key, "voice.my-queue-selection-limit", true) == 0)
                                    {
                                        if (serverDetails[key].ToString() != string.Empty)
                                        {
                                            commonStatistics.MaxObject = Convert.ToInt32(serverDetails[key]);
                                        }
                                    }
                                    else if (string.Compare(key, "statistics.display-width", true) == 0)
                                    {
                                        StatisticsBase.GetInstance().GadgetWidth = Convert.ToDouble(serverDetails[key]);
                                    }
                                    else if (string.Compare(key, "statistics.objects-acd-display", true) == 0)
                                    {
                                        StatisticsBase.GetInstance().QueueDisplayName = Convert.ToString(serverDetails[key]);
                                    }
                                    else if (string.Compare(key, "statistics.alert.popup.font-bold", true) == 0)
                                    {
                                        StatisticsBase.GetInstance().IsThresholdNotifierBold = Convert.ToBoolean(serverDetails[key]);
                                    }
                                    else if (string.Compare(key, "statistics.alert.popup.duration", true) == 0)
                                    {
                                        StatisticsBase.GetInstance().AlertNotifyTime = Convert.ToInt32(serverDetails[key]);
                                    }
                                    else if (string.Compare(key, "statistics.alert.audio-play.duration", true) == 0)
                                    {
                                        StatisticsBase.GetInstance().AlertAudioDuration = Convert.ToInt32(serverDetails[key]);
                                    }
                                    else if (string.Compare(key, "statistics.alert.audio-play.attempt", true) == 0)
                                    {
                                        StatisticsBase.GetInstance().AudioPlayAttempt = Convert.ToInt32(serverDetails[key]);
                                    }
                                    else if (string.Compare(key, "statistics.alert.popup.attempt", true) == 0)
                                    {
                                        StatisticsBase.GetInstance().AlertPopupAttempt = Convert.ToInt32(serverDetails[key]);
                                    }
                                    else if (string.Compare(key, "statistics.server-color", true) == 0)
                                    {
                                        commonStatistics.ServerColor = Color.FromName(serverDetails[key].ToString());
                                    }
                                    else if (string.Compare(key, "dynamic-stats", true) == 0)
                                    {
                                        commonStatistics.BusinessAttributeName = Convert.ToString(serverDetails[key]);
                                    }
                                    else if (string.Compare(key, "statistics.notready-reason.key-name", true) == 0)
                                    {
                                        StatisticsSetting.GetInstance().NotReadyReasonKeyName = serverDetails[key].ToString();
                                    }
                                    #endregion
                                }
                            }
                        }

                        StatisticsSetting.GetInstance().statisticsCollection.StatisticsCommon = commonStatistics;
                        StatisticsSetting.GetInstance().statisticsCollection.StatisticsLocalSetting = localSettings;

                        //string[] ACDQueueObjects = StatisticsSetting.GetInstance().statisticsCollection.StatisticsCommon.ACDObjects.Split(',');

                        //foreach (string ACDs in ACDQueueObjects)
                        //{
                        //    string[] ACDDisplayObjects = ACDs.Split('@');

                        //    if (ACDDisplayObjects.Length > 1)
                        //    {
                        //        if (!StatisticsSetting.GetInstance().DictACDDisplays.ContainsKey(ACDDisplayObjects[0].ToString()))
                        //            StatisticsSetting.GetInstance().DictACDDisplays.Add(ACDDisplayObjects[0].ToString(), ACDDisplayObjects[1].ToString());
                        //    }
                        //}
                        //System.Windows.Forms.MessageBox.Show('as');
                        string[] ACDQueueObjects = StatisticsSetting.GetInstance().statisticsCollection.StatisticsCommon.ACDObjects.Split(',');

                        foreach (string ACDs in ACDQueueObjects)
                        {
                            string Dilimitor = "_@";
                            string[] ACDDisplayObjects = ACDs.Split(new[] { Dilimitor }, StringSplitOptions.None);

                            if (ACDDisplayObjects.Length > 1)
                            {
                                if (!StatisticsSetting.GetInstance().DictACDDisplays.ContainsKey(ACDDisplayObjects[0].ToString()))
                                    StatisticsSetting.GetInstance().DictACDDisplays.Add(ACDDisplayObjects[0].ToString(), ACDDisplayObjects[1].ToString());
                            }
                        }

                        #endregion
                    }

                    ApplicationContainer appcontainer = new ApplicationContainer();

                    KeyValueCollection errorsDetails = (KeyValueCollection)application.Options["_errors_"];
                    if (errorsDetails != null)
                    {
                        #region ErrorDetails

                        foreach (string key in errorsDetails.AllKeys)
                        {
                            if (string.Compare(key, "config.connection", true) == 0)
                            {
                                if (errorsDetails[key].ToString() != null && errorsDetails[key].ToString() != string.Empty)
                                {
                                    appcontainer.ConfigConnection = errorsDetails[key].ToString();
                                    if (!StatisticsSetting.GetInstance().DictErrorValues.ContainsKey(key.ToString()))
                                        StatisticsSetting.GetInstance().DictErrorValues.Add(key.ToString(), errorsDetails[key].ToString());
                                    else
                                        StatisticsSetting.GetInstance().DictErrorValues[key.ToString()] = errorsDetails[key].ToString();
                                }
                            }
                            else if (string.Compare(key, "place.notfound", true) == 0)
                            {
                                if (errorsDetails[key].ToString() != null && errorsDetails[key].ToString() != string.Empty)
                                {
                                    appcontainer.PlaceNotFound = errorsDetails[key].ToString();
                                    if (!StatisticsSetting.GetInstance().DictErrorValues.ContainsKey(key.ToString()))
                                        StatisticsSetting.GetInstance().DictErrorValues.Add(key.ToString(), errorsDetails[key].ToString());
                                    else
                                        StatisticsSetting.GetInstance().DictErrorValues[key.ToString()] = errorsDetails[key].ToString();
                                }
                            }
                            else if (string.Compare(key, "pri.server", true) == 0)
                            {
                                if (errorsDetails[key].ToString() != null && errorsDetails[key].ToString() != string.Empty)
                                {
                                    appcontainer.PrimaryServer = errorsDetails[key].ToString();
                                    if (!StatisticsSetting.GetInstance().DictErrorValues.ContainsKey(key.ToString()))
                                        StatisticsSetting.GetInstance().DictErrorValues.Add(key.ToString(), errorsDetails[key].ToString());
                                    else
                                        StatisticsSetting.GetInstance().DictErrorValues[key.ToString()] = errorsDetails[key].ToString();
                                }
                            }
                            else if (string.Compare(key, "sec.server", true) == 0)
                            {
                                if (errorsDetails[key].ToString() != null && errorsDetails[key].ToString() != string.Empty)
                                {
                                    appcontainer.SecondaryServer = errorsDetails[key].ToString();
                                    if (!StatisticsSetting.GetInstance().DictErrorValues.ContainsKey(key.ToString()))
                                        StatisticsSetting.GetInstance().DictErrorValues.Add(key.ToString(), errorsDetails[key].ToString());
                                    else
                                        StatisticsSetting.GetInstance().DictErrorValues[key.ToString()] = errorsDetails[key].ToString();
                                }
                            }
                            else if (string.Compare(key, "server.down", true) == 0)
                            {
                                if (errorsDetails[key].ToString() != null && errorsDetails[key].ToString() != string.Empty)
                                {
                                    appcontainer.ServerDown = errorsDetails[key].ToString();
                                    if (!StatisticsSetting.GetInstance().DictErrorValues.ContainsKey(key.ToString()))
                                        StatisticsSetting.GetInstance().DictErrorValues.Add(key.ToString(), errorsDetails[key].ToString());
                                    else
                                        StatisticsSetting.GetInstance().DictErrorValues[key.ToString()] = errorsDetails[key].ToString();
                                }
                            }
                            else if (string.Compare(key, "user.authorization", true) == 0)
                            {
                                if (errorsDetails[key].ToString() != null && errorsDetails[key].ToString() != string.Empty)
                                {
                                    appcontainer.UserAuthorization = errorsDetails[key].ToString();
                                    if (!StatisticsSetting.GetInstance().DictErrorValues.ContainsKey(key.ToString()))
                                        StatisticsSetting.GetInstance().DictErrorValues.Add(key.ToString(), errorsDetails[key].ToString());
                                    else
                                        StatisticsSetting.GetInstance().DictErrorValues[key.ToString()] = errorsDetails[key].ToString();
                                }
                            }
                            else if (string.Compare(key, "user.permission", true) == 0)
                            {
                                if (errorsDetails[key].ToString() != null && errorsDetails[key].ToString() != string.Empty)
                                {
                                    appcontainer.UserPermission = errorsDetails[key].ToString();
                                    if (!StatisticsSetting.GetInstance().DictErrorValues.ContainsKey(key.ToString()))
                                        StatisticsSetting.GetInstance().DictErrorValues.Add(key.ToString(), errorsDetails[key].ToString());
                                    else
                                        StatisticsSetting.GetInstance().DictErrorValues[key.ToString()] = errorsDetails[key].ToString();
                                }
                            }
                            else if (string.Compare(key, "nostat.configured", true) == 0)
                            {
                                if (errorsDetails[key].ToString() != null && errorsDetails[key].ToString() != string.Empty)
                                {
                                    appcontainer.NoStats = errorsDetails[key].ToString();
                                    if (!StatisticsSetting.GetInstance().DictErrorValues.ContainsKey(key.ToString()))
                                        StatisticsSetting.GetInstance().DictErrorValues.Add(key.ToString(), errorsDetails[key].ToString());
                                    else
                                        StatisticsSetting.GetInstance().DictErrorValues[key.ToString()] = errorsDetails[key].ToString();

                                }
                            }
                            else if (string.Compare(key, "db.authentication", true) == 0)
                            {
                                if (errorsDetails[key].ToString() != null && errorsDetails[key].ToString() != string.Empty)
                                {
                                    appcontainer.dbAuthentication = errorsDetails[key].ToString();
                                    if (!StatisticsSetting.GetInstance().DictErrorValues.ContainsKey(key.ToString()))
                                        StatisticsSetting.GetInstance().DictErrorValues.Add(key.ToString(), errorsDetails[key].ToString());
                                    else
                                        StatisticsSetting.GetInstance().DictErrorValues[key.ToString()] = errorsDetails[key].ToString();
                                }
                            }
                            else if (string.Compare(key, "db.connection", true) == 0)
                            {
                                if (errorsDetails[key].ToString() != null && errorsDetails[key].ToString() != string.Empty)
                                {
                                    appcontainer.dbConnection = errorsDetails[key].ToString();
                                    if (!StatisticsSetting.GetInstance().DictErrorValues.ContainsKey(key.ToString()))
                                        StatisticsSetting.GetInstance().DictErrorValues.Add(key.ToString(), errorsDetails[key].ToString());
                                    else
                                        StatisticsSetting.GetInstance().DictErrorValues[key.ToString()] = errorsDetails[key].ToString();
                                }
                            }
                        }
                        #endregion
                    }

                    KeyValueCollection systemDetails = (KeyValueCollection)application.Options["_system_"];
                    if (systemDetails != null)
                    {
                        #region System Values

                        foreach (string key in systemDetails.AllKeys)
                        {
                            if (string.Compare(key, "admin.access-group", true) == 0)
                            {
                                if (systemDetails[key].ToString() != string.Empty)
                                {
                                    appcontainer.AdminGroupName = systemDetails[key].ToString();
                                }
                            }
                            else if (string.Compare(key, "statistics.display-time", true) == 0)
                            {
                                if (systemDetails[key].ToString() != string.Empty)
                                {
                                    appcontainer.DisplayTime = systemDetails[key].ToString();
                                    StatisticsBase.GetInstance().statsIntervalTime = Convert.ToInt32(systemDetails[key]);
                                }
                            }
                            else if (string.Compare(key, "user.access-group", true) == 0)
                            {
                                if (systemDetails[key].ToString() != string.Empty)
                                {
                                    appcontainer.UserGroupName = systemDetails[key].ToString();
                                }
                            }
                            else if (string.Compare(key, "statistics.bold", true) == 0)
                            {
                                if (systemDetails[key].ToString() != string.Empty)
                                {
                                    appcontainer.StatBold = Convert.ToBoolean(systemDetails[key]);
                                    StatisticsBase.GetInstance().StatBold = Convert.ToBoolean(systemDetails[key]);
                                }
                            }
                            else if (string.Compare(key, "login.error-display-count", true) == 0)
                            {
                                if (systemDetails[key].ToString() != string.Empty)
                                {
                                    if (systemDetails[key].ToString() == "0")
                                    {
                                        StatisticsBase.GetInstance().ErrorCount = 1;
                                    }
                                    else
                                    {
                                        StatisticsBase.GetInstance().ErrorCount = Convert.ToInt32(systemDetails[key]);
                                    }
                                }
                                else
                                {
                                    StatisticsBase.GetInstance().ErrorCount = 3;
                                }
                            }
                        }

                        //Get Switches for Admin
                        //if (string.IsNullOrEmpty(appcontainer.Switches))
                        //{
                        //    CfgTenantQuery tenantQuery = new CfgTenantQuery();
                        //    tenantQuery.Name = StatisticsSetting.GetInstance().statisticsCollection.StatisticsCommon.TenantName;
                        //    CfgTenant tenantObject = StatisticsSetting.GetInstance().confObject.RetrieveObject<CfgTenant>(tenantQuery);

                        //    ICollection<CfgSwitch> _switches = null;

                        //    if (tenantObject != null)
                        //    {
                        //        CfgSwitchQuery _switchQuery = new CfgSwitchQuery();
                        //        _switchQuery.TenantDbid = tenantObject.DBID;

                        //        _switches = StatisticsSetting.GetInstance().confObject.RetrieveMultipleObjects<CfgSwitch>(_switchQuery);

                        //    }

                        //    foreach (CfgSwitch switchObj in _switches)
                        //    {
                        //        if (string.IsNullOrEmpty(appcontainer.Switches))
                        //            appcontainer.Switches = switchObj.Name;
                        //        else
                        //            appcontainer.Switches = appcontainer.Switches + "," + switchObj.Name;
                        //    }
                        //}

                        #endregion
                    }

                    if (StatisticsBase.GetInstance().ErrorCount == 0)
                        StatisticsBase.GetInstance().ErrorCount = 3;

                    KeyValueCollection enabledisablechannels = (KeyValueCollection)application.Options["enable-disable-channels"];
                    if (enabledisablechannels != null)
                    {
                        #region EnableDisableChannels

                        foreach (string key in enabledisablechannels.AllKeys)
                        {
                            if (string.Compare(key, "statistics.enable-alwaysontop", true) == 0)
                            {
                                if (enabledisablechannels[key].ToString() != null && enabledisablechannels[key].ToString() != string.Empty && ((string.Compare(enabledisablechannels[key].ToString(), "true", true) == 0) || (string.Compare(enabledisablechannels[key].ToString(), "false", true) == 0)))
                                {
                                    appcontainer.EnableAlwaysOnTop = Convert.ToBoolean(enabledisablechannels[key]);
                                    if (!StatisticsSetting.GetInstance().DictEnableDisableChannels.ContainsKey("AlwaysOnTop"))
                                        StatisticsSetting.GetInstance().DictEnableDisableChannels.Add("AlwaysOnTop", Convert.ToBoolean(enabledisablechannels[key]));
                                    else
                                        StatisticsSetting.GetInstance().DictEnableDisableChannels["AlwaysOnTop"] = Convert.ToBoolean(enabledisablechannels[key]);
                                }
                            }
                            else if (string.Compare(key, "statistics.enable-ccstat-aid", true) == 0)
                            {
                                if (enabledisablechannels[key].ToString() != null && enabledisablechannels[key].ToString() != string.Empty && ((string.Compare(enabledisablechannels[key].ToString(), "true", true) == 0) || (string.Compare(enabledisablechannels[key].ToString(), "false", true) == 0)))
                                {
                                    appcontainer.EnableCCStatAID = Convert.ToBoolean(enabledisablechannels[key]);
                                    if (!StatisticsSetting.GetInstance().DictEnableDisableChannels.ContainsKey("ccstataid"))
                                        StatisticsSetting.GetInstance().DictEnableDisableChannels.Add("ccstataid", Convert.ToBoolean(enabledisablechannels[key]));
                                    else
                                        StatisticsSetting.GetInstance().DictEnableDisableChannels["ccstataid"] = Convert.ToBoolean(enabledisablechannels[key]);
                                }
                            }
                            else if (string.Compare(key, "statistics.enable-log", true) == 0)
                            {
                                if (enabledisablechannels[key].ToString() != null && enabledisablechannels[key].ToString() != string.Empty && ((string.Compare(enabledisablechannels[key].ToString(), "true", true) == 0) || (string.Compare(enabledisablechannels[key].ToString(), "false", true) == 0)))
                                {
                                    appcontainer.EnableLog = Convert.ToBoolean(enabledisablechannels[key]);
                                    if (!StatisticsSetting.GetInstance().DictEnableDisableChannels.ContainsKey("log"))
                                        StatisticsSetting.GetInstance().DictEnableDisableChannels.Add("log", Convert.ToBoolean(enabledisablechannels[key]));
                                    else
                                        StatisticsSetting.GetInstance().DictEnableDisableChannels["log"] = Convert.ToBoolean(enabledisablechannels[key]);
                                }
                            }
                            else if (string.Compare(key, "statistics.enable-maingadget", true) == 0)
                            {
                                if (enabledisablechannels[key].ToString() != null && enabledisablechannels[key].ToString() != string.Empty && ((string.Compare(enabledisablechannels[key].ToString(), "true", true) == 0) || (string.Compare(enabledisablechannels[key].ToString(), "false", true) == 0)))
                                {
                                    appcontainer.EnableMainGadget = Convert.ToBoolean(enabledisablechannels[key]);
                                    if (!StatisticsSetting.GetInstance().DictEnableDisableChannels.ContainsKey("maingadget"))
                                        StatisticsSetting.GetInstance().DictEnableDisableChannels.Add("maingadget", Convert.ToBoolean(enabledisablechannels[key]));
                                    else
                                        StatisticsSetting.GetInstance().DictEnableDisableChannels["maingadget"] = Convert.ToBoolean(enabledisablechannels[key]);
                                }
                            }
                            else if (string.Compare(key, "statistics.enable-menu-button", true) == 0)
                            {
                                if (enabledisablechannels[key].ToString() != null && enabledisablechannels[key].ToString() != string.Empty && ((string.Compare(enabledisablechannels[key].ToString(), "true", true) == 0) || (string.Compare(enabledisablechannels[key].ToString(), "false", true) == 0)))
                                {
                                    appcontainer.EnableMenuButton = Convert.ToBoolean(enabledisablechannels[key]);
                                    if (!StatisticsSetting.GetInstance().DictEnableDisableChannels.ContainsKey("menubutton"))
                                        StatisticsSetting.GetInstance().DictEnableDisableChannels.Add("menubutton", Convert.ToBoolean(enabledisablechannels[key]));
                                    else
                                        StatisticsSetting.GetInstance().DictEnableDisableChannels["menubutton"] = Convert.ToBoolean(enabledisablechannels[key]);
                                }
                            }
                            else if (string.Compare(key, "statistics.enable-mystat-aid", true) == 0)
                            {
                                if (enabledisablechannels[key].ToString() != null && enabledisablechannels[key].ToString() != string.Empty && ((string.Compare(enabledisablechannels[key].ToString(), "true", true) == 0) || (string.Compare(enabledisablechannels[key].ToString(), "false", true) == 0)))
                                {
                                    appcontainer.EnableMyStatAID = Convert.ToBoolean(enabledisablechannels[key]);
                                    if (!StatisticsSetting.GetInstance().DictEnableDisableChannels.ContainsKey("mystataid"))
                                        StatisticsSetting.GetInstance().DictEnableDisableChannels.Add("mystataid", Convert.ToBoolean(enabledisablechannels[key]));
                                    else
                                        StatisticsSetting.GetInstance().DictEnableDisableChannels["mystataid"] = Convert.ToBoolean(enabledisablechannels[key]);
                                }
                            }
                            else if (string.Compare(key, "statistics.enable-notification-close", true) == 0)
                            {
                                if (enabledisablechannels[key].ToString() != null && enabledisablechannels[key].ToString() != string.Empty && ((string.Compare(enabledisablechannels[key].ToString(), "true", true) == 0) || (string.Compare(enabledisablechannels[key].ToString(), "false", true) == 0)))
                                {
                                    appcontainer.EnableTaskbarClose = Convert.ToBoolean(enabledisablechannels[key]);
                                    if (!StatisticsSetting.GetInstance().DictEnableDisableChannels.ContainsKey("notificationclose"))
                                        StatisticsSetting.GetInstance().DictEnableDisableChannels.Add("notificationclose", Convert.ToBoolean(enabledisablechannels[key]));
                                    else
                                        StatisticsSetting.GetInstance().DictEnableDisableChannels["notificationclose"] = Convert.ToBoolean(enabledisablechannels[key]);
                                }
                            }
                            else if (string.Compare(key, "statistics.enable-notification-balloon", true) == 0)
                            {
                                if (enabledisablechannels[key].ToString() != null && enabledisablechannels[key].ToString() != string.Empty && ((string.Compare(enabledisablechannels[key].ToString(), "true", true) == 0) || (string.Compare(enabledisablechannels[key].ToString(), "false", true) == 0)))
                                {
                                    appcontainer.EnableNotificationBalloon = Convert.ToBoolean(enabledisablechannels[key]);
                                    if (!StatisticsSetting.GetInstance().DictEnableDisableChannels.ContainsKey("notificationballoon"))
                                        StatisticsSetting.GetInstance().DictEnableDisableChannels.Add("notificationballoon", Convert.ToBoolean(enabledisablechannels[key]));
                                    else
                                        StatisticsSetting.GetInstance().DictEnableDisableChannels["notificationballoon"] = Convert.ToBoolean(enabledisablechannels[key]);
                                }
                            }
                            else if (string.Compare(key, "statistics.enable-submenu-ccstatistics", true) == 0)
                            {
                                if (enabledisablechannels[key].ToString() != null && enabledisablechannels[key].ToString() != string.Empty && ((string.Compare(enabledisablechannels[key].ToString(), "true", true) == 0) || (string.Compare(enabledisablechannels[key].ToString(), "false", true) == 0)))
                                {
                                    appcontainer.EnableMenuShowCCStat = Convert.ToBoolean(enabledisablechannels[key]);
                                    if (!StatisticsSetting.GetInstance().DictEnableDisableChannels.ContainsKey("ccstatistics"))
                                        StatisticsSetting.GetInstance().DictEnableDisableChannels.Add("ccstatistics", Convert.ToBoolean(enabledisablechannels[key]));
                                    else
                                        StatisticsSetting.GetInstance().DictEnableDisableChannels["ccstatistics"] = Convert.ToBoolean(enabledisablechannels[key]);
                                }
                            }
                            else if (string.Compare(key, "statistics.enable-submenu-close-gadget", true) == 0)
                            {
                                if (enabledisablechannels[key].ToString() != null && enabledisablechannels[key].ToString() != string.Empty && ((string.Compare(enabledisablechannels[key].ToString(), "true", true) == 0) || (string.Compare(enabledisablechannels[key].ToString(), "false", true) == 0)))
                                {
                                    appcontainer.EnableMenuClose = Convert.ToBoolean(enabledisablechannels[key]);
                                    if (!StatisticsSetting.GetInstance().DictEnableDisableChannels.ContainsKey("closegadget"))
                                        StatisticsSetting.GetInstance().DictEnableDisableChannels.Add("closegadget", Convert.ToBoolean(enabledisablechannels[key]));
                                    else
                                        StatisticsSetting.GetInstance().DictEnableDisableChannels["closegadget"] = Convert.ToBoolean(enabledisablechannels[key]);
                                }
                            }
                            else if (string.Compare(key, "statistics.enable-submenu-mystatistics", true) == 0)
                            {
                                if (enabledisablechannels[key].ToString() != null && enabledisablechannels[key].ToString() != string.Empty && ((string.Compare(enabledisablechannels[key].ToString(), "true", true) == 0) || (string.Compare(enabledisablechannels[key].ToString(), "false", true) == 0)))
                                {
                                    appcontainer.EnableMenuShowMyStat = Convert.ToBoolean(enabledisablechannels[key]);
                                    if (!StatisticsSetting.GetInstance().DictEnableDisableChannels.ContainsKey("mystatistics"))
                                        StatisticsSetting.GetInstance().DictEnableDisableChannels.Add("mystatistics", Convert.ToBoolean(enabledisablechannels[key]));
                                    else
                                        StatisticsSetting.GetInstance().DictEnableDisableChannels["mystatistics"] = Convert.ToBoolean(enabledisablechannels[key]);
                                }
                            }
                            else if (string.Compare(key, "statistics.enable-submenu-ontop", true) == 0)
                            {
                                if (enabledisablechannels[key].ToString() != null && enabledisablechannels[key].ToString() != string.Empty && ((string.Compare(enabledisablechannels[key].ToString(), "true", true) == 0) || (string.Compare(enabledisablechannels[key].ToString(), "false", true) == 0)))
                                {
                                    appcontainer.EnableMenuOnTop = Convert.ToBoolean(enabledisablechannels[key]);
                                    if (!StatisticsSetting.GetInstance().DictEnableDisableChannels.ContainsKey("ontop"))
                                        StatisticsSetting.GetInstance().DictEnableDisableChannels.Add("ontop", Convert.ToBoolean(enabledisablechannels[key]));
                                    else
                                        StatisticsSetting.GetInstance().DictEnableDisableChannels["ontop"] = Convert.ToBoolean(enabledisablechannels[key]);
                                }
                            }
                            else if (string.Compare(key, "statistics.enable-tag-button", true) == 0)
                            {
                                if (enabledisablechannels[key].ToString() != null && enabledisablechannels[key].ToString() != string.Empty && ((string.Compare(enabledisablechannels[key].ToString(), "true", true) == 0) || (string.Compare(enabledisablechannels[key].ToString(), "false", true) == 0)))
                                {
                                    appcontainer.EnableTagButton = Convert.ToBoolean(enabledisablechannels[key]);
                                    if (!StatisticsSetting.GetInstance().DictEnableDisableChannels.ContainsKey("tagbutton"))
                                        StatisticsSetting.GetInstance().DictEnableDisableChannels.Add("tagbutton", Convert.ToBoolean(enabledisablechannels[key]));
                                    else
                                        StatisticsSetting.GetInstance().DictEnableDisableChannels["tagbutton"] = Convert.ToBoolean(enabledisablechannels[key]);
                                }
                            }
                            else if (string.Compare(key, "statistics.enable-tag-vertical", true) == 0)
                            {
                                if (enabledisablechannels[key].ToString() != null && enabledisablechannels[key].ToString() != string.Empty && ((string.Compare(enabledisablechannels[key].ToString(), "true", true) == 0) || (string.Compare(enabledisablechannels[key].ToString(), "false", true) == 0)))
                                {
                                    appcontainer.EnableTagVertical = Convert.ToBoolean(enabledisablechannels[key]);
                                    if (!StatisticsSetting.GetInstance().DictEnableDisableChannels.ContainsKey("tagvertical"))
                                        StatisticsSetting.GetInstance().DictEnableDisableChannels.Add("tagvertical", Convert.ToBoolean(enabledisablechannels[key]));
                                    else
                                        StatisticsSetting.GetInstance().DictEnableDisableChannels["tagvertical"] = Convert.ToBoolean(enabledisablechannels[key]);
                                }
                            }
                            else if (string.Compare(key, "statistics.enable-untag-button", true) == 0)
                            {
                                if (enabledisablechannels[key].ToString() != null && enabledisablechannels[key].ToString() != string.Empty && ((string.Compare(enabledisablechannels[key].ToString(), "true", true) == 0) || (string.Compare(enabledisablechannels[key].ToString(), "false", true) == 0)))
                                {
                                    appcontainer.EnableUntagButton = Convert.ToBoolean(enabledisablechannels[key]);
                                    if (!StatisticsSetting.GetInstance().DictEnableDisableChannels.ContainsKey("untagbutton"))
                                        StatisticsSetting.GetInstance().DictEnableDisableChannels.Add("untagbutton", Convert.ToBoolean(enabledisablechannels[key]));
                                    else
                                        StatisticsSetting.GetInstance().DictEnableDisableChannels["untagbutton"] = Convert.ToBoolean(enabledisablechannels[key]);
                                }
                            }
                            else if (string.Compare(key, "statistics.enable-hhmmss-format", true) == 0)
                            {
                                if (enabledisablechannels[key].ToString() != null && enabledisablechannels[key].ToString() != string.Empty && ((string.Compare(enabledisablechannels[key].ToString(), "true", true) == 0) || (string.Compare(enabledisablechannels[key].ToString(), "false", true) == 0)))
                                {
                                    if (!StatisticsSetting.GetInstance().DictEnableDisableChannels.ContainsKey("hhmmssformat"))
                                        StatisticsSetting.GetInstance().DictEnableDisableChannels.Add("hhmmssformat", Convert.ToBoolean(enabledisablechannels[key]));
                                    else
                                        StatisticsSetting.GetInstance().DictEnableDisableChannels["hhmmssformat"] = Convert.ToBoolean(enabledisablechannels[key]);

                                    appcontainer.EnableHHMMSS = Convert.ToBoolean(enabledisablechannels[key]);
                                }
                            }
                            else if (string.Compare(key, "statistics.enable-submenu-alert-notification", true) == 0)
                            {
                                if (enabledisablechannels[key].ToString() != null && enabledisablechannels[key].ToString() != string.Empty && ((string.Compare(enabledisablechannels[key].ToString(), "true", true) == 0) || (string.Compare(enabledisablechannels[key].ToString(), "false", true) == 0)))
                                {
                                    appcontainer.EnableThresholdNotification = Convert.ToBoolean(enabledisablechannels[key]);
                                    if (!StatisticsSetting.GetInstance().DictEnableDisableChannels.ContainsKey("thresholdnotification"))
                                        StatisticsSetting.GetInstance().DictEnableDisableChannels.Add("thresholdnotification", Convert.ToBoolean(enabledisablechannels[key]));
                                    else
                                        StatisticsSetting.GetInstance().DictEnableDisableChannels["thresholdnotification"] = Convert.ToBoolean(enabledisablechannels[key]);
                                }
                            }
                            else if (string.Compare(key, "statistics.enable-system-draggable", true) == 0)
                            {
                                if (enabledisablechannels[key].ToString() != null && enabledisablechannels[key].ToString() != string.Empty && ((string.Compare(enabledisablechannels[key].ToString(), "true", true) == 0) || (string.Compare(enabledisablechannels[key].ToString(), "false", true) == 0)))
                                {
                                    appcontainer.SystemDraggable = Convert.ToBoolean(enabledisablechannels[key]);
                                    StatisticsBase.GetInstance().IsSystemDraggable = Convert.ToBoolean(enabledisablechannels[key]);
                                }
                            }
                            else if (string.Compare(key, "statistics.enable-submenu-showheader", true) == 0)
                            {
                                if (enabledisablechannels[key].ToString() != null && enabledisablechannels[key].ToString() != string.Empty && ((string.Compare(enabledisablechannels[key].ToString(), "true", true) == 0) || (string.Compare(enabledisablechannels[key].ToString(), "false", true) == 0)))
                                {
                                    if (!StatisticsSetting.GetInstance().DictEnableDisableChannels.ContainsKey("showheader"))
                                        StatisticsSetting.GetInstance().DictEnableDisableChannels.Add("showheader", Convert.ToBoolean(enabledisablechannels[key]));
                                    else
                                        StatisticsSetting.GetInstance().DictEnableDisableChannels["showheader"] = Convert.ToBoolean(enabledisablechannels[key]);
                                }
                            }
                            else if (string.Compare(key, "statistics.enable-notify-primary-screen", true) == 0)
                            {
                                if (enabledisablechannels[key].ToString() != null && enabledisablechannels[key].ToString() != string.Empty && ((string.Compare(enabledisablechannels[key].ToString(), "true", true) == 0) || (string.Compare(enabledisablechannels[key].ToString(), "false", true) == 0)))
                                {
                                    if (!StatisticsSetting.GetInstance().DictEnableDisableChannels.ContainsKey("notifyprimaryscreen"))
                                        StatisticsSetting.GetInstance().DictEnableDisableChannels.Add("notifyprimaryscreen", Convert.ToBoolean(enabledisablechannels[key]));
                                    else
                                        StatisticsSetting.GetInstance().DictEnableDisableChannels["notifyprimaryscreen"] = Convert.ToBoolean(enabledisablechannels[key]);
                                }
                            }
                            else if (string.Compare(key, "voice.enable-my-queue-statistics", true) == 0)
                            {
                                if (enabledisablechannels[key].ToString() != null && enabledisablechannels[key].ToString() != string.Empty && ((string.Compare(enabledisablechannels[key].ToString(), "true", true) == 0) || (string.Compare(enabledisablechannels[key].ToString(), "false", true) == 0)))
                                {
                                    appcontainer.EnableMyQueueStatistics = Convert.ToBoolean(enabledisablechannels[key]);
                                }
                            }
                            else if (string.Compare(key, "voice.enable.my-queue-config", true) == 0)
                            {
                                if (enabledisablechannels[key].ToString() != null && enabledisablechannels[key].ToString() != string.Empty && ((string.Compare(enabledisablechannels[key].ToString(), "true", true) == 0) || (string.Compare(enabledisablechannels[key].ToString(), "false", true) == 0)))
                                {
                                    appcontainer.EnableMyQueueConfig = Convert.ToBoolean(enabledisablechannels[key]);
                                }
                            }
                            else if (string.Compare(key, "statistics.enable-agent-configuration", true) == 0)
                            {
                                if (enabledisablechannels[key].ToString() != null && enabledisablechannels[key].ToString() != string.Empty && ((string.Compare(enabledisablechannels[key].ToString(), "true", true) == 0)))
                                {
                                    StatisticsSetting.GetInstance().isAgentConfigFromLocal = Convert.ToBoolean(enabledisablechannels[key].ToString());
                                    StatisticsSetting.GetInstance().agentConfigColl = ReadAgentConfiguration();
                                }
                                else
                                {
                                    StatisticsSetting.GetInstance().isAgentConfigFromLocal = false;
                                }
                            }
                           
                        }

                        #endregion
                    }

                    StatisticsSetting.GetInstance().statisticsCollection.ApplicationContainer = appcontainer;
                    AdminValues adminValues = new AdminValues();
                    adminValues.AdminUsers = new List<string>();

                    KeyValueCollection adminConfig = (KeyValueCollection)application.Options["admin.config"];
                    if (adminConfig != null)
                    {
                        #region Admin Configurations

                        foreach (string key in adminConfig.Keys)
                        {
                            if (string.Compare(key, "existing.stat.color", true) == 0)
                            {
                                if (!string.IsNullOrEmpty(adminConfig[key].ToString()))
                                {
                                    adminValues.ExistingColor = Color.FromName(adminConfig[key].ToString());
                                }
                                else
                                {
                                    adminValues.ExistingColor = Color.White;
                                }
                            }
                            else if (string.Compare(key, "server.stat.color", true) == 0)
                            {
                                if (!string.IsNullOrEmpty(adminConfig[key].ToString()))
                                {
                                    adminValues.NewColor = Color.FromName(adminConfig[key].ToString());
                                }
                                else
                                {
                                    adminValues.NewColor = Color.White;
                                }
                            }
                            else if (string.Compare(key, "max.tabs", true) == 0)
                            {
                                if (!string.IsNullOrEmpty(adminConfig[key].ToString()))
                                {
                                    adminValues.MaxTabs = Convert.ToInt16(adminConfig[key]);
                                }
                                else
                                {
                                    adminValues.MaxTabs = 20;
                                }
                            }
                            else if (string.Compare(key, "add.not-ready", true) == 0)
                            {
                                if (!string.IsNullOrEmpty(adminConfig[key].ToString()))
                                {
                                    adminValues.AgentNR = Convert.ToBoolean(adminConfig[key].ToString());
                                }
                                else
                                {
                                    adminValues.AgentNR = false;
                                }
                            }
                            else if (string.Compare(key, "admin.user-names", true) == 0)
                            {
                                if (!string.IsNullOrEmpty(adminConfig[key].ToString()))
                                {
                                    string Dilimitor = "&&";
                                    string[] adminNames = adminConfig[key].ToString().Split(new[] { Dilimitor }, StringSplitOptions.None);

                                    foreach (string admin in adminNames)
                                    {
                                        if (!adminValues.AdminUsers.Contains(admin))
                                            adminValues.AdminUsers.Add(admin);
                                    }
                                }
                            }
                            else if (string.Compare(key, "not-ready-reason-code", true) == 0)
                            {
                                if (!string.IsNullOrEmpty(adminConfig[key].ToString()))
                                {
                                    adminValues.NRReasonCode = Convert.ToInt16(adminConfig[key]);
                                }
                                else
                                {
                                    adminValues.NRReasonCode = 1;
                                }
                            }
                            else if (string.Compare(key, "not-ready.reason-name", true) == 0)
                            {
                                if (!string.IsNullOrEmpty(adminConfig[key].ToString()))
                                {
                                    adminValues.NRReasonName = adminConfig[key].ToString();
                                }
                                else
                                {
                                    adminValues.NRReasonName = "Admin";
                                }
                            }
                            else if (string.Compare(key, "agent.loggedout-color", true) == 0)
                            {
                                if (!string.IsNullOrEmpty(adminConfig[key].ToString()))
                                {
                                    adminValues.LogoutColor = Color.FromName(adminConfig[key].ToString());
                                }
                                else
                                {
                                    adminValues.NewColor = Color.White;
                                }
                            }
                            else if (string.Compare(key, "agent.notready-color", true) == 0)
                            {
                                if (!string.IsNullOrEmpty(adminConfig[key].ToString()))
                                {
                                    adminValues.NRColor = Color.FromName(adminConfig[key].ToString());
                                }
                                else
                                {
                                    adminValues.NewColor = Color.White;
                                }
                            }
                            else if (string.Compare(key, "agent.ready-color", true) == 0)
                            {
                                if (!string.IsNullOrEmpty(adminConfig[key].ToString()))
                                {
                                    adminValues.ReadyColor = Color.FromName(adminConfig[key].ToString());
                                }
                                else
                                {
                                    adminValues.NewColor = Color.White;
                                }
                            }
                            else if (string.Compare(key, "auto.display-agents", true) == 0)
                            {
                                if (!string.IsNullOrEmpty(adminConfig[key].ToString()))
                                {
                                    adminValues.AutoAgents = Convert.ToBoolean(adminConfig[key].ToString());
                                }
                                else
                                {
                                    adminValues.AutoAgents = false;
                                }
                            }
                        }

                        #endregion
                    }

                    StatisticsSetting.GetInstance().statisticsCollection.AdminValues = adminValues;

                    #endregion

                    #region Application Annex

                    string[] appKeys = application.UserProperties.AllKeys;
                    foreach (string section in appKeys)
                    {
                        if (section.StartsWith("agent"))
                        {
                            #region Read Agent Statistics

                            List<string> tempThresholdValues = new List<string>();
                            List<Color> tempThresholdColors = new List<Color>();
                            KeyValueCollection kvColl = new KeyValueCollection();
                            kvColl = (KeyValueCollection)application.UserProperties[section];

                            AgentStatistics agentStat = new AgentStatistics();
                            agentStat.ReferenceID = StatisticsSetting.GetInstance().ReferenceId;
                            agentStat.TempStatName = section.ToString();

                            #region ANNEX

                            logger.Debug("-------------------------------------------------------------------");
                            logger.Debug("ReadApplication  : ReadApplicationDetails : " + section.ToString());
                            logger.Debug("-------------------------------------------------------------------");
                            foreach (string Key in kvColl.AllKeys)
                            {
                                if (string.Compare(Key, "Color1", true) == 0)
                                {
                                    agentStat.StatColor = Color.FromName(kvColl[Key].ToString());
                                    logger.Debug("ReadApplication  : ReadApplicationDetails : " + kvColl[Key].ToString());
                                }
                                else if (string.Compare(Key, "Color2", true) == 0)
                                {
                                    agentStat.ThresholdColorOne = Color.FromName(kvColl[Key].ToString());
                                    logger.Debug("ReadApplication  : ReadApplicationDetails : " + kvColl[Key].ToString());
                                }
                                else if (string.Compare(Key, "Color3", true) == 0)
                                {
                                    agentStat.ThresholdColorTwo = Color.FromName(kvColl[Key].ToString());
                                    logger.Debug("ReadApplication  : ReadApplicationDetails : " + kvColl[Key].ToString());
                                }
                                else if (string.Compare(Key, "DisplayName", true) == 0)
                                {
                                    agentStat.DisplayName = kvColl[Key].ToString();
                                    logger.Debug("ReadApplication  : ReadApplicationDetails : " + kvColl[Key].ToString());
                                }
                                else if (string.Compare(Key, "Filter", true) == 0)
                                {
                                    agentStat.FilterName = kvColl[Key].ToString();
                                    logger.Debug("ReadApplication  : ReadApplicationDetails : " + kvColl[Key].ToString());
                                }
                                else if (string.Compare(Key, "Format", true) == 0)
                                {
                                    agentStat.StatisticsFormat = kvColl[Key].ToString();
                                    logger.Debug("ReadApplication  : ReadApplicationDetails : " + kvColl[Key].ToString());
                                }
                                else if (string.Compare(Key, "StatName", true) == 0)
                                {
                                    agentStat.StatisticsName = kvColl[Key].ToString();
                                    logger.Debug("ReadApplication  : ReadApplicationDetails : " + kvColl[Key].ToString());
                                }
                                else if (string.Compare(Key, "ThresLevel1", true) == 0)
                                {
                                    agentStat.ThresholdLevelOne = kvColl[Key].ToString();
                                    logger.Debug("ReadApplication  : ReadApplicationDetails : " + kvColl[Key].ToString());
                                }
                                else if (string.Compare(Key, "ThresLevel2", true) == 0)
                                {
                                    agentStat.ThresholdLevelTwo = kvColl[Key].ToString();
                                    logger.Debug("ReadApplication  : ReadApplicationDetails : " + kvColl[Key].ToString());
                                }
                                else if (string.Compare(Key, "TooltipName", true) == 0)
                                {
                                    agentStat.ToolTipName = kvColl[Key].ToString();
                                    logger.Debug("ReadApplication  : ReadApplicationDetails : " + kvColl[Key].ToString());
                                }
                                else if (string.Compare(Key, "ServerName", true) == 0)
                                {
                                    agentStat.ServerName = kvColl[Key].ToString();
                                    logger.Debug("ReadApplication  : ReadApplicationDetails : " + kvColl[Key].ToString());
                                }

                            }
                            #endregion

                            if (!StatisticsSetting.GetInstance().ReferenceIds.ContainsKey(section))
                                StatisticsSetting.GetInstance().ReferenceIds.Add(section, agentStat.ReferenceID.ToString());

                            StatisticsSetting.GetInstance().statisticsCollection.AgentStatistics.Add(agentStat);

                            agentStat = null;

                            #endregion
                        }
                        else if (section.StartsWith("group"))
                        {
                            #region Read AgentGroup Statistics

                            List<string> tempThresholdValues = new List<string>();
                            List<Color> tempThresholdColors = new List<Color>();
                            KeyValueCollection kvColl = new KeyValueCollection();
                            kvColl = (KeyValueCollection)application.UserProperties[section];

                            AgentGroupStatistics agentGroupStat = new AgentGroupStatistics();
                            agentGroupStat.ReferenceID = StatisticsSetting.GetInstance().ReferenceId;
                            agentGroupStat.TempStatName = section.ToString();

                            #region ANNEX
                            logger.Debug("-------------------------------------------------------------------");
                            logger.Debug("ReadApplication  : ReadApplicationDetails : " + section.ToString());
                            logger.Debug("-------------------------------------------------------------------");

                            foreach (string Key in kvColl.AllKeys)
                            {
                                if (string.Compare(Key, "Color1", true) == 0)
                                {
                                    agentGroupStat.StatColor = Color.FromName(kvColl[Key].ToString());
                                    logger.Debug("ReadApplication  : ReadApplicationDetails : " + kvColl[Key].ToString());
                                }
                                else if (string.Compare(Key, "Color2", true) == 0)
                                {
                                    agentGroupStat.ThresholdColorOne = Color.FromName(kvColl[Key].ToString());
                                    logger.Debug("ReadApplication  : ReadApplicationDetails : " + kvColl[Key].ToString());
                                }
                                else if (string.Compare(Key, "Color3", true) == 0)
                                {
                                    agentGroupStat.ThresholdColorTwo = Color.FromName(kvColl[Key].ToString());
                                    logger.Debug("ReadApplication  : ReadApplicationDetails : " + kvColl[Key].ToString());
                                }
                                else if (string.Compare(Key, "DisplayName", true) == 0)
                                {
                                    agentGroupStat.DisplayName = kvColl[Key].ToString();
                                    logger.Debug("ReadApplication  : ReadApplicationDetails : " + kvColl[Key].ToString());
                                }
                                else if (string.Compare(Key, "Filter", true) == 0)
                                {
                                    agentGroupStat.FilterName = kvColl[Key].ToString();
                                    logger.Debug("ReadApplication  : ReadApplicationDetails : " + kvColl[Key].ToString());
                                }
                                else if (string.Compare(Key, "Format", true) == 0)
                                {
                                    agentGroupStat.StatisticsFormat = kvColl[Key].ToString();
                                    logger.Debug("ReadApplication  : ReadApplicationDetails : " + kvColl[Key].ToString());
                                }
                                else if (string.Compare(Key, "StatName", true) == 0)
                                {
                                    agentGroupStat.StatisticsName = kvColl[Key].ToString();
                                    logger.Debug("ReadApplication  : ReadApplicationDetails : " + kvColl[Key].ToString());
                                }
                                else if (string.Compare(Key, "ThresLevel1", true) == 0)
                                {
                                    agentGroupStat.ThresholdLevelOne = kvColl[Key].ToString();
                                    logger.Debug("ReadApplication  : ReadApplicationDetails : " + kvColl[Key].ToString());
                                }
                                else if (string.Compare(Key, "ThresLevel2", true) == 0)
                                {
                                    agentGroupStat.ThresholdLevelTwo = kvColl[Key].ToString();
                                    logger.Debug("ReadApplication  : ReadApplicationDetails : " + kvColl[Key].ToString());
                                }
                                else if (string.Compare(Key, "TooltipName", true) == 0)
                                {
                                    agentGroupStat.ToolTipName = kvColl[Key].ToString();
                                    logger.Debug("ReadApplication  : ReadApplicationDetails : " + kvColl[Key].ToString());
                                }
                                else if (string.Compare(Key, "ServerName", true) == 0)
                                {
                                    agentGroupStat.ServerName = kvColl[Key].ToString();
                                    logger.Debug("ReadApplication  : ReadApplicationDetails : " + kvColl[Key].ToString());
                                }
                            }
                            #endregion

                            if (!StatisticsSetting.GetInstance().ReferenceIds.ContainsKey(section))
                                StatisticsSetting.GetInstance().ReferenceIds.Add(section, agentGroupStat.ReferenceID.ToString());

                            StatisticsSetting.GetInstance().statisticsCollection.AgentGroupStatistics.Add(agentGroupStat);
                            agentGroupStat = null;

                            #endregion
                        }
                        else if (section.StartsWith("dn"))
                        {
                            #region Read DNGroup Statistics

                            List<string> tempThresholdValues = new List<string>();
                            List<Color> tempThresholdColors = new List<Color>();
                            KeyValueCollection kvColl = new KeyValueCollection();
                            kvColl = (KeyValueCollection)application.UserProperties[section];

                            DNGroupStatistics dnGroupStat = new DNGroupStatistics();
                            dnGroupStat.ReferenceID = StatisticsSetting.GetInstance().ReferenceId;
                            dnGroupStat.TempStatName = section.ToString();

                            #region ANNEX
                            logger.Debug("-------------------------------------------------------------------");
                            logger.Debug("ReadApplication  : ReadApplicationDetails : " + section.ToString());
                            logger.Debug("-------------------------------------------------------------------");

                            foreach (string Key in kvColl.AllKeys)
                            {
                                if (string.Compare(Key, "Color1", true) == 0)
                                {
                                    dnGroupStat.StatColor = Color.FromName(kvColl[Key].ToString());
                                    logger.Debug("ReadApplication  : ReadApplicationDetails : " + kvColl[Key].ToString());
                                }
                                else if (string.Compare(Key, "Color2", true) == 0)
                                {
                                    dnGroupStat.ThresholdColorOne = Color.FromName(kvColl[Key].ToString());
                                    logger.Debug("ReadApplication  : ReadApplicationDetails : " + kvColl[Key].ToString());
                                }
                                else if (string.Compare(Key, "Color3", true) == 0)
                                {
                                    dnGroupStat.ThresholdColorTwo = Color.FromName(kvColl[Key].ToString());
                                    logger.Debug("ReadApplication  : ReadApplicationDetails : " + kvColl[Key].ToString());
                                }
                                else if (string.Compare(Key, "DisplayName", true) == 0)
                                {
                                    dnGroupStat.DisplayName = kvColl[Key].ToString();
                                    logger.Debug("ReadApplication  : ReadApplicationDetails : " + kvColl[Key].ToString());
                                }
                                else if (string.Compare(Key, "Filter", true) == 0)
                                {
                                    dnGroupStat.FilterName = kvColl[Key].ToString();
                                    logger.Debug("ReadApplication  : ReadApplicationDetails : " + kvColl[Key].ToString());
                                }
                                else if (string.Compare(Key, "Format", true) == 0)
                                {
                                    dnGroupStat.StatisticsFormat = kvColl[Key].ToString();
                                    logger.Debug("ReadApplication  : ReadApplicationDetails : " + kvColl[Key].ToString());
                                }
                                else if (string.Compare(Key, "StatName", true) == 0)
                                {
                                    dnGroupStat.StatisticsName = kvColl[Key].ToString();
                                    logger.Debug("ReadApplication  : ReadApplicationDetails : " + kvColl[Key].ToString());
                                }
                                else if (string.Compare(Key, "stype", true) == 0)
                                {
                                    dnGroupStat.StatisticsType = kvColl[Key].ToString();
                                    logger.Debug("ReadApplication  : ReadApplicationDetails : " + kvColl[Key].ToString());
                                }
                                else if (string.Compare(Key, "ThresLevel1", true) == 0)
                                {
                                    dnGroupStat.ThresholdLevelOne = kvColl[Key].ToString();
                                    logger.Debug("ReadApplication  : ReadApplicationDetails : " + kvColl[Key].ToString());
                                }
                                else if (string.Compare(Key, "ThresLevel2", true) == 0)
                                {
                                    dnGroupStat.ThresholdLevelTwo = kvColl[Key].ToString();
                                    logger.Debug("ReadApplication  : ReadApplicationDetails : " + kvColl[Key].ToString());
                                }
                                else if (string.Compare(Key, "TooltipName", true) == 0)
                                {
                                    dnGroupStat.ToolTipName = kvColl[Key].ToString();
                                    logger.Debug("ReadApplication  : ReadApplicationDetails : " + kvColl[Key].ToString());
                                }
                                else if (string.Compare(Key, "ServerName", true) == 0)
                                {
                                    dnGroupStat.ServerName = kvColl[Key].ToString();
                                    logger.Debug("ReadApplication  : ReadApplicationDetails : " + kvColl[Key].ToString());
                                }
                            }
                            #endregion

                            if (!StatisticsSetting.GetInstance().ReferenceIds.ContainsKey(section))
                                StatisticsSetting.GetInstance().ReferenceIds.Add(section, dnGroupStat.ReferenceID.ToString());

                            StatisticsSetting.GetInstance().statisticsCollection.DNGroupStatistics.Add(dnGroupStat);

                            dnGroupStat = null;

                            #endregion
                        }
                        else if (section.StartsWith("acd"))
                        {
                            #region Read ACDQueue Statistics

                            List<string> tempThresholdValues = new List<string>();
                            List<Color> tempThresholdColors = new List<Color>();
                            KeyValueCollection kvColl = new KeyValueCollection();
                            kvColl = (KeyValueCollection)application.UserProperties[section];

                            ACDStatistics acdQueueStat = new ACDStatistics();
                            acdQueueStat.ReferenceID = StatisticsSetting.GetInstance().ReferenceId;
                            acdQueueStat.TempStatName = section.ToString();

                            #region ANNEX
                            logger.Debug("-------------------------------------------------------------------");
                            logger.Debug("ReadApplication  : ReadApplicationDetails : " + section.ToString());
                            logger.Debug("-------------------------------------------------------------------");

                            foreach (string Key in kvColl.AllKeys)
                            {
                                if (string.Compare(Key, "Color1", true) == 0)
                                {
                                    acdQueueStat.StatColor = Color.FromName(kvColl[Key].ToString());
                                    logger.Debug("ReadApplication  : ReadApplicationDetails : " + kvColl[Key].ToString());
                                }
                                else if (string.Compare(Key, "Color2", true) == 0)
                                {
                                    acdQueueStat.ThresholdColorOne = Color.FromName(kvColl[Key].ToString());
                                    logger.Debug("ReadApplication  : ReadApplicationDetails : " + kvColl[Key].ToString());
                                }
                                else if (string.Compare(Key, "Color3", true) == 0)
                                {
                                    acdQueueStat.ThresholdColorTwo = Color.FromName(kvColl[Key].ToString());
                                    logger.Debug("ReadApplication  : ReadApplicationDetails : " + kvColl[Key].ToString());
                                }
                                else if (string.Compare(Key, "DisplayName", true) == 0)
                                {
                                    acdQueueStat.DisplayName = kvColl[Key].ToString();
                                    logger.Debug("ReadApplication  : ReadApplicationDetails : " + kvColl[Key].ToString());
                                }
                                else if (string.Compare(Key, "Filter", true) == 0)
                                {
                                    acdQueueStat.FilterName = kvColl[Key].ToString();
                                    logger.Debug("ReadApplication  : ReadApplicationDetails : " + kvColl[Key].ToString());
                                }
                                else if (string.Compare(Key, "Format", true) == 0)
                                {
                                    acdQueueStat.StatisticsFormat = kvColl[Key].ToString();
                                    logger.Debug("ReadApplication  : ReadApplicationDetails : " + kvColl[Key].ToString());
                                }
                                else if (string.Compare(Key, "StatName", true) == 0)
                                {
                                    acdQueueStat.StatisticsName = kvColl[Key].ToString();
                                    logger.Debug("ReadApplication  : ReadApplicationDetails : " + kvColl[Key].ToString());
                                }
                                else if (string.Compare(Key, "stype", true) == 0)
                                {
                                    acdQueueStat.StatisticsType = kvColl[Key].ToString();
                                    logger.Debug("ReadApplication  : ReadApplicationDetails : " + kvColl[Key].ToString());
                                }
                                else if (string.Compare(Key, "ThresLevel1", true) == 0)
                                {
                                    acdQueueStat.ThresholdLevelOne = kvColl[Key].ToString();
                                    logger.Debug("ReadApplication  : ReadApplicationDetails : " + kvColl[Key].ToString());
                                }
                                else if (string.Compare(Key, "ThresLevel2", true) == 0)
                                {
                                    acdQueueStat.ThresholdLevelTwo = kvColl[Key].ToString();
                                    logger.Debug("ReadApplication  : ReadApplicationDetails : " + kvColl[Key].ToString());
                                }
                                else if (string.Compare(Key, "TooltipName", true) == 0)
                                {
                                    acdQueueStat.ToolTipName = kvColl[Key].ToString();
                                    logger.Debug("ReadApplication  : ReadApplicationDetails : " + kvColl[Key].ToString());
                                }
                                else if (string.Compare(Key, "ServerName", true) == 0)
                                {
                                    acdQueueStat.ServerName = kvColl[Key].ToString();
                                    logger.Debug("ReadApplication  : ReadApplicationDetails : " + kvColl[Key].ToString());
                                }
                            }
                            #endregion

                            if (!StatisticsSetting.GetInstance().ReferenceIds.ContainsKey(section))
                                StatisticsSetting.GetInstance().ReferenceIds.Add(section, acdQueueStat.ReferenceID.ToString());
                            StatisticsSetting.GetInstance().statisticsCollection.ACDQueueStatistics.Add(acdQueueStat);
                            acdQueueStat = null;

                            #endregion
                        }
                        else if (section.StartsWith("vq"))
                        {
                            #region Read Vitual Queue Statistics

                            List<string> tempThresholdValues = new List<string>();
                            List<Color> tempThresholdColors = new List<Color>();
                            KeyValueCollection kvColl = new KeyValueCollection();
                            kvColl = (KeyValueCollection)application.UserProperties[section];

                            VirtualQueueStatistics virtualQueueStat = new VirtualQueueStatistics();
                            virtualQueueStat.ReferenceID = StatisticsSetting.GetInstance().ReferenceId;
                            virtualQueueStat.TempStatName = section.ToString();

                            #region ANNEX
                            logger.Debug("-------------------------------------------------------------------");
                            logger.Debug("ReadApplication  : ReadApplicationDetails : " + section.ToString());
                            logger.Debug("-------------------------------------------------------------------");

                            foreach (string Key in kvColl.AllKeys)
                            {
                                if (string.Compare(Key, "Color1", true) == 0)
                                {
                                    virtualQueueStat.StatColor = Color.FromName(kvColl[Key].ToString());
                                    logger.Debug("ReadApplication  : ReadApplicationDetails : " + kvColl[Key].ToString());
                                }
                                else if (string.Compare(Key, "Color2", true) == 0)
                                {
                                    virtualQueueStat.ThresholdColorOne = Color.FromName(kvColl[Key].ToString());
                                    logger.Debug("ReadApplication  : ReadApplicationDetails : " + kvColl[Key].ToString());
                                }
                                else if (string.Compare(Key, "Color3", true) == 0)
                                {
                                    virtualQueueStat.ThresholdColorTwo = Color.FromName(kvColl[Key].ToString());
                                    logger.Debug("ReadApplication  : ReadApplicationDetails : " + kvColl[Key].ToString());
                                }
                                else if (string.Compare(Key, "DisplayName", true) == 0)
                                {
                                    virtualQueueStat.DisplayName = kvColl[Key].ToString();
                                    logger.Debug("ReadApplication  : ReadApplicationDetails : " + kvColl[Key].ToString());
                                }
                                else if (string.Compare(Key, "Filter", true) == 0)
                                {
                                    virtualQueueStat.FilterName = kvColl[Key].ToString();
                                    logger.Debug("ReadApplication  : ReadApplicationDetails : " + kvColl[Key].ToString());
                                }
                                else if (string.Compare(Key, "Format", true) == 0)
                                {
                                    virtualQueueStat.StatisticsFormat = kvColl[Key].ToString();
                                    logger.Debug("ReadApplication  : ReadApplicationDetails : " + kvColl[Key].ToString());
                                }
                                else if (string.Compare(Key, "StatName", true) == 0)
                                {
                                    virtualQueueStat.StatisticsName = kvColl[Key].ToString();
                                    logger.Debug("ReadApplication  : ReadApplicationDetails : " + kvColl[Key].ToString());
                                }
                                else if (string.Compare(Key, "stype", true) == 0)
                                {
                                    virtualQueueStat.StatisticsType = kvColl[Key].ToString();
                                    logger.Debug("ReadApplication  : ReadApplicationDetails : " + kvColl[Key].ToString());
                                }
                                else if (string.Compare(Key, "ThresLevel1", true) == 0)
                                {
                                    virtualQueueStat.ThresholdLevelOne = kvColl[Key].ToString();
                                    logger.Debug("ReadApplication  : ReadApplicationDetails : " + kvColl[Key].ToString());
                                }
                                else if (string.Compare(Key, "ThresLevel2", true) == 0)
                                {
                                    virtualQueueStat.ThresholdLevelTwo = kvColl[Key].ToString();
                                    logger.Debug("ReadApplication  : ReadApplicationDetails : " + kvColl[Key].ToString());
                                }
                                else if (string.Compare(Key, "TooltipName", true) == 0)
                                {
                                    virtualQueueStat.ToolTipName = kvColl[Key].ToString();
                                    logger.Debug("ReadApplication  : ReadApplicationDetails : " + kvColl[Key].ToString());
                                }
                                else if (string.Compare(Key, "ServerName", true) == 0)
                                {
                                    virtualQueueStat.ServerName = kvColl[Key].ToString();
                                    logger.Debug("ReadApplication  : ReadApplicationDetails : " + kvColl[Key].ToString());
                                }
                            }
                            #endregion

                            if (!StatisticsSetting.GetInstance().ReferenceIds.ContainsKey(section))
                                StatisticsSetting.GetInstance().ReferenceIds.Add(section, virtualQueueStat.ReferenceID.ToString());
                            StatisticsSetting.GetInstance().statisticsCollection.VirtualQueueStatistics.Add(virtualQueueStat);
                            virtualQueueStat = null;

                            #endregion
                        }
                        StatisticsSetting.GetInstance().ReferenceId++;
                    }

                    #endregion
                }
                else
                {
                    StatisticsSetting.GetInstance().IsApplicationFound = false;
                }
            }
            catch (Exception GeneralException)
            {
                logger.Error("ReadApplication : ReadApplicationDetails Method : " + GeneralException.Message.ToString());
            }

            logger.Debug("ReadApplication : ReadApplicationDetails Method : Exit");
        }

        public void ReadBusinessAttributeDetails(string businessAttributeName)
        {
            try
            {
                logger.Debug("ReadApplication : ReadBusinessAttributeDetails Method: Entry");

                CfgEnumeratorQuery businessAttributeQuery = new CfgEnumeratorQuery();
                businessAttributeQuery.Name = businessAttributeName;
                businessAttributeQuery.TenantDbid = StatisticsSetting.GetInstance().CFGTenantDBID;

                CfgEnumerator businessAttribute = StatisticsSetting.GetInstance().confObject.RetrieveObject<CfgEnumerator>(businessAttributeQuery);

                if (businessAttribute != null)
                {
                    CfgEnumeratorValueQuery attributeValuesQuery = new CfgEnumeratorValueQuery();
                    attributeValuesQuery.EnumeratorDbid = businessAttribute.DBID;
                    var attributeValues = StatisticsSetting.GetInstance().confObject.RetrieveMultipleObjects<CfgEnumeratorValue>(attributeValuesQuery);
                    if (attributeValues != null && attributeValues.Count > 0)
                    {
                        logger.Debug("Retrieving values from business attribute");
                        foreach (CfgEnumeratorValue enumerator in attributeValues)
                        {
                            if (string.Compare(enumerator.Name, "Stat1", true) == 0)
                            {
                                #region BusinessAttribute Annex

                                string[] BAttributeKeys = enumerator.UserProperties.AllKeys;
                                int TempReferenceId = StatisticsSetting.GetInstance().BAttributeReferenceId;

                                foreach (string section in BAttributeKeys)
                                {
                                    if (section.StartsWith("agent"))
                                    {
                                        #region Read Agent Statistics

                                        List<string> tempThresholdValues = new List<string>();
                                        List<Color> tempThresholdColors = new List<Color>();
                                        Dictionary<string, string> StatMetrics = new Dictionary<string, string>();

                                        KeyValueCollection kvColl = new KeyValueCollection();
                                        kvColl = (KeyValueCollection)enumerator.UserProperties[section];

                                        AgentStatistics agentStat = new AgentStatistics();
                                        agentStat.ReferenceID = TempReferenceId;
                                        agentStat.TempStatName = section.ToString();

                                        #region ANNEX

                                        logger.Debug("-------------------------------------------------------------------");
                                        logger.Debug("ReadApplication  : ReadBusinessAttributeDetails : " + section.ToString());
                                        logger.Debug("-------------------------------------------------------------------");
                                        foreach (string Key in kvColl.AllKeys)
                                        {
                                            if (string.Compare(Key, "Color1", true) == 0)
                                            {
                                                agentStat.StatColor = Color.FromName(kvColl[Key].ToString());
                                                logger.Debug("ReadApplication  : ReadBusinessAttributeDetails : " + kvColl[Key].ToString());
                                            }
                                            else if (string.Compare(Key, "Color2", true) == 0)
                                            {
                                                agentStat.ThresholdColorOne = Color.FromName(kvColl[Key].ToString());
                                                logger.Debug("ReadApplication  : ReadBusinessAttributeDetails : " + kvColl[Key].ToString());
                                            }
                                            else if (string.Compare(Key, "Color3", true) == 0)
                                            {
                                                agentStat.ThresholdColorTwo = Color.FromName(kvColl[Key].ToString());
                                                logger.Debug("ReadApplication  : ReadBusinessAttributeDetails : " + kvColl[Key].ToString());
                                            }
                                            else if (string.Compare(Key, "DisplayName", true) == 0)
                                            {
                                                agentStat.DisplayName = kvColl[Key].ToString();
                                                logger.Debug("ReadApplication  : ReadBusinessAttributeDetails : " + kvColl[Key].ToString());
                                            }
                                            else if (string.Compare(Key, "Filter", true) == 0)
                                            {
                                                agentStat.FilterName = kvColl[Key].ToString();
                                                logger.Debug("ReadApplication  : ReadBusinessAttributeDetails : " + kvColl[Key].ToString());
                                            }
                                            else if (string.Compare(Key, "Format", true) == 0)
                                            {
                                                agentStat.StatisticsFormat = kvColl[Key].ToString();
                                                logger.Debug("ReadApplication  : ReadBusinessAttributeDetails : " + kvColl[Key].ToString());
                                            }
                                            else if (string.Compare(Key, "StatName", true) == 0)
                                            {
                                                agentStat.StatisticsName = kvColl[Key].ToString();
                                                logger.Debug("ReadApplication  : ReadBusinessAttributeDetails : " + kvColl[Key].ToString());
                                            }
                                            else if (string.Compare(Key, "ThresLevel1", true) == 0)
                                            {
                                                agentStat.ThresholdLevelOne = kvColl[Key].ToString();
                                                logger.Debug("ReadApplication  : ReadBusinessAttributeDetails : " + kvColl[Key].ToString());
                                            }
                                            else if (string.Compare(Key, "ThresLevel2", true) == 0)
                                            {
                                                agentStat.ThresholdLevelTwo = kvColl[Key].ToString();
                                                logger.Debug("ReadApplication  : ReadBusinessAttributeDetails : " + kvColl[Key].ToString());
                                            }
                                            else if (string.Compare(Key, "TooltipName", true) == 0)
                                            {
                                                agentStat.ToolTipName = kvColl[Key].ToString();
                                                logger.Debug("ReadApplication  : ReadBusinessAttributeDetails : " + kvColl[Key].ToString());
                                            }
                                            else if (string.Compare(Key, "Category", true) == 0)
                                            {
                                                StatMetrics.Add(Key, kvColl[Key].ToString());
                                                logger.Debug("ReadApplication  : ReadBusinessAttributeDetails : " + kvColl[Key].ToString());
                                            }
                                            else if (string.Compare(Key, "MainMask", true) == 0)
                                            {
                                                StatMetrics.Add(Key, kvColl[Key].ToString());
                                                logger.Debug("ReadApplication  : ReadBusinessAttributeDetails : " + kvColl[Key].ToString());
                                            }
                                            else if (string.Compare(Key, "RelMask", true) == 0)
                                            {
                                                StatMetrics.Add(Key, kvColl[Key].ToString());
                                                logger.Debug("ReadApplication  : ReadBusinessAttributeDetails : " + kvColl[Key].ToString());
                                            }
                                            else if (string.Compare(Key, "Subject", true) == 0)
                                            {
                                                StatMetrics.Add(Key, kvColl[Key].ToString());
                                                logger.Debug("ReadApplication  : ReadBusinessAttributeDetails : " + kvColl[Key].ToString());
                                            }

                                        }
                                        #endregion

                                        if (!StatisticsSetting.GetInstance().ReferenceIds.ContainsKey(section))
                                            StatisticsSetting.GetInstance().ReferenceIds.Add(section, agentStat.ReferenceID.ToString());

                                        StatisticsSetting.GetInstance().statisticsCollection.AgentStatistics.Add(agentStat);
                                        StatisticsSetting.GetInstance().DictStatisticsMetrics.Add(section, StatMetrics);
                                        agentStat = null;

                                        #endregion
                                    }
                                    else if (section.StartsWith("group"))
                                    {
                                        #region Read AgentGroup Statistics

                                        List<string> tempThresholdValues = new List<string>();
                                        List<Color> tempThresholdColors = new List<Color>();
                                        KeyValueCollection kvColl = new KeyValueCollection();
                                        kvColl = (KeyValueCollection)enumerator.UserProperties[section];
                                        Dictionary<string, string> StatMetrics = new Dictionary<string, string>();

                                        AgentGroupStatistics agentGroupStat = new AgentGroupStatistics();
                                        agentGroupStat.ReferenceID = TempReferenceId;
                                        agentGroupStat.TempStatName = section.ToString();

                                        #region ANNEX
                                        logger.Debug("-------------------------------------------------------------------");
                                        logger.Debug("ReadApplication  : ReadBusinessAttributeDetails : " + section.ToString());
                                        logger.Debug("-------------------------------------------------------------------");

                                        foreach (string Key in kvColl.AllKeys)
                                        {
                                            if (string.Compare(Key, "Color1", true) == 0)
                                            {
                                                agentGroupStat.StatColor = Color.FromName(kvColl[Key].ToString());
                                                logger.Debug("ReadApplication  : ReadBusinessAttributeDetails : " + kvColl[Key].ToString());
                                            }
                                            else if (string.Compare(Key, "Color2", true) == 0)
                                            {
                                                agentGroupStat.ThresholdColorOne = Color.FromName(kvColl[Key].ToString());
                                                logger.Debug("ReadApplication  : ReadBusinessAttributeDetails : " + kvColl[Key].ToString());
                                            }
                                            else if (string.Compare(Key, "Color3", true) == 0)
                                            {
                                                agentGroupStat.ThresholdColorTwo = Color.FromName(kvColl[Key].ToString());
                                                logger.Debug("ReadApplication  : ReadBusinessAttributeDetails : " + kvColl[Key].ToString());
                                            }
                                            else if (string.Compare(Key, "DisplayName", true) == 0)
                                            {
                                                agentGroupStat.DisplayName = kvColl[Key].ToString();
                                                logger.Debug("ReadApplication  : ReadBusinessAttributeDetails : " + kvColl[Key].ToString());
                                            }
                                            else if (string.Compare(Key, "Filter", true) == 0)
                                            {
                                                agentGroupStat.FilterName = kvColl[Key].ToString();
                                                logger.Debug("ReadApplication  : ReadBusinessAttributeDetails : " + kvColl[Key].ToString());
                                            }
                                            else if (string.Compare(Key, "Format", true) == 0)
                                            {
                                                agentGroupStat.StatisticsFormat = kvColl[Key].ToString();
                                                logger.Debug("ReadApplication  : ReadBusinessAttributeDetails : " + kvColl[Key].ToString());
                                            }
                                            else if (string.Compare(Key, "StatName", true) == 0)
                                            {
                                                agentGroupStat.StatisticsName = kvColl[Key].ToString();
                                                logger.Debug("ReadApplication  : ReadBusinessAttributeDetails : " + kvColl[Key].ToString());
                                            }
                                            else if (string.Compare(Key, "ThresLevel1", true) == 0)
                                            {
                                                agentGroupStat.ThresholdLevelOne = kvColl[Key].ToString();
                                                logger.Debug("ReadApplication  : ReadBusinessAttributeDetails : " + kvColl[Key].ToString());
                                            }
                                            else if (string.Compare(Key, "ThresLevel2", true) == 0)
                                            {
                                                agentGroupStat.ThresholdLevelTwo = kvColl[Key].ToString();
                                                logger.Debug("ReadApplication  : ReadBusinessAttributeDetails : " + kvColl[Key].ToString());
                                            }
                                            else if (string.Compare(Key, "TooltipName", true) == 0)
                                            {
                                                agentGroupStat.ToolTipName = kvColl[Key].ToString();
                                                logger.Debug("ReadApplication  : ReadBusinessAttributeDetails : " + kvColl[Key].ToString());
                                            }
                                            else if (string.Compare(Key, "Category", true) == 0)
                                            {
                                                StatMetrics.Add(Key, kvColl[Key].ToString());
                                                logger.Debug("ReadApplication  : ReadBusinessAttributeDetails : " + kvColl[Key].ToString());
                                            }
                                            else if (string.Compare(Key, "MainMask", true) == 0)
                                            {
                                                StatMetrics.Add(Key, kvColl[Key].ToString());
                                                logger.Debug("ReadApplication  : ReadBusinessAttributeDetails : " + kvColl[Key].ToString());
                                            }
                                            else if (string.Compare(Key, "RelMask", true) == 0)
                                            {
                                                StatMetrics.Add(Key, kvColl[Key].ToString());
                                                logger.Debug("ReadApplication  : ReadBusinessAttributeDetails : " + kvColl[Key].ToString());
                                            }
                                            else if (string.Compare(Key, "Subject", true) == 0)
                                            {
                                                StatMetrics.Add(Key, kvColl[Key].ToString());
                                                logger.Debug("ReadApplication  : ReadBusinessAttributeDetails : " + kvColl[Key].ToString());
                                            }
                                        }
                                        #endregion

                                        if (!StatisticsSetting.GetInstance().ReferenceIds.ContainsKey(section))
                                            StatisticsSetting.GetInstance().ReferenceIds.Add(section, agentGroupStat.ReferenceID.ToString());

                                        StatisticsSetting.GetInstance().statisticsCollection.AgentGroupStatistics.Add(agentGroupStat);
                                        StatisticsSetting.GetInstance().DictStatisticsMetrics.Add(section, StatMetrics);
                                        agentGroupStat = null;

                                        #endregion
                                    }
                                    else if (section.StartsWith("dn"))
                                    {
                                        #region Read DNGroup Statistics

                                        List<string> tempThresholdValues = new List<string>();
                                        List<Color> tempThresholdColors = new List<Color>();
                                        KeyValueCollection kvColl = new KeyValueCollection();
                                        kvColl = (KeyValueCollection)enumerator.UserProperties[section];
                                        Dictionary<string, string> StatMetrics = new Dictionary<string, string>();

                                        DNGroupStatistics dnGroupStat = new DNGroupStatistics();
                                        dnGroupStat.ReferenceID = TempReferenceId;
                                        dnGroupStat.TempStatName = section.ToString();

                                        #region ANNEX
                                        logger.Debug("-------------------------------------------------------------------");
                                        logger.Debug("ReadApplication  : ReadBusinessAttributeDetails : " + section.ToString());
                                        logger.Debug("-------------------------------------------------------------------");

                                        foreach (string Key in kvColl.AllKeys)
                                        {
                                            if (string.Compare(Key, "Color1", true) == 0)
                                            {
                                                dnGroupStat.StatColor = Color.FromName(kvColl[Key].ToString());
                                                logger.Debug("ReadApplication  : ReadBusinessAttributeDetails : " + kvColl[Key].ToString());
                                            }
                                            else if (string.Compare(Key, "Color2", true) == 0)
                                            {
                                                dnGroupStat.ThresholdColorOne = Color.FromName(kvColl[Key].ToString());
                                                logger.Debug("ReadApplication  : ReadBusinessAttributeDetails : " + kvColl[Key].ToString());
                                            }
                                            else if (string.Compare(Key, "Color3", true) == 0)
                                            {
                                                dnGroupStat.ThresholdColorTwo = Color.FromName(kvColl[Key].ToString());
                                                logger.Debug("ReadApplication  : ReadBusinessAttributeDetails : " + kvColl[Key].ToString());
                                            }
                                            else if (string.Compare(Key, "DisplayName", true) == 0)
                                            {
                                                dnGroupStat.DisplayName = kvColl[Key].ToString();
                                                logger.Debug("ReadApplication  : ReadBusinessAttributeDetails : " + kvColl[Key].ToString());
                                            }
                                            else if (string.Compare(Key, "Filter", true) == 0)
                                            {
                                                dnGroupStat.FilterName = kvColl[Key].ToString();
                                                logger.Debug("ReadApplication  : ReadBusinessAttributeDetails : " + kvColl[Key].ToString());
                                            }
                                            else if (string.Compare(Key, "Format", true) == 0)
                                            {
                                                dnGroupStat.StatisticsFormat = kvColl[Key].ToString();
                                                logger.Debug("ReadApplication  : ReadBusinessAttributeDetails : " + kvColl[Key].ToString());
                                            }
                                            else if (string.Compare(Key, "StatName", true) == 0)
                                            {
                                                dnGroupStat.StatisticsName = kvColl[Key].ToString();
                                                logger.Debug("ReadApplication  : ReadBusinessAttributeDetails : " + kvColl[Key].ToString());
                                            }
                                            else if (string.Compare(Key, "stype", true) == 0)
                                            {
                                                dnGroupStat.StatisticsType = kvColl[Key].ToString();
                                                logger.Debug("ReadApplication  : ReadBusinessAttributeDetails : " + kvColl[Key].ToString());
                                            }
                                            else if (string.Compare(Key, "ThresLevel1", true) == 0)
                                            {
                                                dnGroupStat.ThresholdLevelOne = kvColl[Key].ToString();
                                                logger.Debug("ReadApplication  : ReadBusinessAttributeDetails : " + kvColl[Key].ToString());
                                            }
                                            else if (string.Compare(Key, "ThresLevel2", true) == 0)
                                            {
                                                dnGroupStat.ThresholdLevelTwo = kvColl[Key].ToString();
                                                logger.Debug("ReadApplication  : ReadBusinessAttributeDetails : " + kvColl[Key].ToString());
                                            }
                                            else if (string.Compare(Key, "TooltipName", true) == 0)
                                            {
                                                dnGroupStat.ToolTipName = kvColl[Key].ToString();
                                                logger.Debug("ReadApplication  : ReadBusinessAttributeDetails : " + kvColl[Key].ToString());
                                            }
                                            else if (string.Compare(Key, "Category", true) == 0)
                                            {
                                                StatMetrics.Add(Key, kvColl[Key].ToString());
                                                logger.Debug("ReadApplication  : ReadBusinessAttributeDetails : " + kvColl[Key].ToString());
                                            }
                                            else if (string.Compare(Key, "MainMask", true) == 0)
                                            {
                                                StatMetrics.Add(Key, kvColl[Key].ToString());
                                                logger.Debug("ReadApplication  : ReadBusinessAttributeDetails : " + kvColl[Key].ToString());
                                            }
                                            else if (string.Compare(Key, "RelMask", true) == 0)
                                            {
                                                StatMetrics.Add(Key, kvColl[Key].ToString());
                                                logger.Debug("ReadApplication  : ReadBusinessAttributeDetails : " + kvColl[Key].ToString());
                                            }
                                            else if (string.Compare(Key, "Subject", true) == 0)
                                            {
                                                StatMetrics.Add(Key, kvColl[Key].ToString());
                                                logger.Debug("ReadApplication  : ReadBusinessAttributeDetails : " + kvColl[Key].ToString());
                                            }
                                        }
                                        #endregion

                                        if (!StatisticsSetting.GetInstance().ReferenceIds.ContainsKey(section))
                                            StatisticsSetting.GetInstance().ReferenceIds.Add(section, dnGroupStat.ReferenceID.ToString());

                                        StatisticsSetting.GetInstance().statisticsCollection.DNGroupStatistics.Add(dnGroupStat);
                                        StatisticsSetting.GetInstance().DictStatisticsMetrics.Add(section, StatMetrics);
                                        dnGroupStat = null;

                                        #endregion
                                    }
                                    else if (section.StartsWith("acd"))
                                    {
                                        #region Read ACDQueue Statistics

                                        List<string> tempThresholdValues = new List<string>();
                                        List<Color> tempThresholdColors = new List<Color>();
                                        KeyValueCollection kvColl = new KeyValueCollection();
                                        kvColl = (KeyValueCollection)enumerator.UserProperties[section];
                                        Dictionary<string, string> StatMetrics = new Dictionary<string, string>();

                                        ACDStatistics acdQueueStat = new ACDStatistics();
                                        acdQueueStat.ReferenceID = TempReferenceId;
                                        acdQueueStat.TempStatName = section.ToString();

                                        #region ANNEX
                                        logger.Debug("-------------------------------------------------------------------");
                                        logger.Debug("ReadApplication  : ReadBusinessAttributeDetails : " + section.ToString());
                                        logger.Debug("-------------------------------------------------------------------");

                                        foreach (string Key in kvColl.AllKeys)
                                        {
                                            if (string.Compare(Key, "Color1", true) == 0)
                                            {
                                                acdQueueStat.StatColor = Color.FromName(kvColl[Key].ToString());
                                                logger.Debug("ReadApplication  : ReadBusinessAttributeDetails : " + kvColl[Key].ToString());
                                            }
                                            else if (string.Compare(Key, "Color2", true) == 0)
                                            {
                                                acdQueueStat.ThresholdColorOne = Color.FromName(kvColl[Key].ToString());
                                                logger.Debug("ReadApplication  : ReadBusinessAttributeDetails : " + kvColl[Key].ToString());
                                            }
                                            else if (string.Compare(Key, "Color3", true) == 0)
                                            {
                                                acdQueueStat.ThresholdColorTwo = Color.FromName(kvColl[Key].ToString());
                                                logger.Debug("ReadApplication  : ReadBusinessAttributeDetails : " + kvColl[Key].ToString());
                                            }
                                            else if (string.Compare(Key, "DisplayName", true) == 0)
                                            {
                                                acdQueueStat.DisplayName = kvColl[Key].ToString();
                                                logger.Debug("ReadApplication  : ReadBusinessAttributeDetails : " + kvColl[Key].ToString());
                                            }
                                            else if (string.Compare(Key, "Filter", true) == 0)
                                            {
                                                acdQueueStat.FilterName = kvColl[Key].ToString();
                                                logger.Debug("ReadApplication  : ReadBusinessAttributeDetails : " + kvColl[Key].ToString());
                                            }
                                            else if (string.Compare(Key, "Format", true) == 0)
                                            {
                                                acdQueueStat.StatisticsFormat = kvColl[Key].ToString();
                                                logger.Debug("ReadApplication  : ReadBusinessAttributeDetails : " + kvColl[Key].ToString());
                                            }
                                            else if (string.Compare(Key, "StatName", true) == 0)
                                            {
                                                acdQueueStat.StatisticsName = kvColl[Key].ToString();
                                                logger.Debug("ReadApplication  : ReadBusinessAttributeDetails : " + kvColl[Key].ToString());
                                            }
                                            else if (string.Compare(Key, "stype", true) == 0)
                                            {
                                                acdQueueStat.StatisticsType = kvColl[Key].ToString();
                                                logger.Debug("ReadApplication  : ReadBusinessAttributeDetails : " + kvColl[Key].ToString());
                                            }
                                            else if (string.Compare(Key, "ThresLevel1", true) == 0)
                                            {
                                                acdQueueStat.ThresholdLevelOne = kvColl[Key].ToString();
                                                logger.Debug("ReadApplication  : ReadBusinessAttributeDetails : " + kvColl[Key].ToString());
                                            }
                                            else if (string.Compare(Key, "ThresLevel2", true) == 0)
                                            {
                                                acdQueueStat.ThresholdLevelTwo = kvColl[Key].ToString();
                                                logger.Debug("ReadApplication  : ReadBusinessAttributeDetails : " + kvColl[Key].ToString());
                                            }
                                            else if (string.Compare(Key, "TooltipName", true) == 0)
                                            {
                                                acdQueueStat.ToolTipName = kvColl[Key].ToString();
                                                logger.Debug("ReadApplication  : ReadBusinessAttributeDetails : " + kvColl[Key].ToString());
                                            }
                                            else if (string.Compare(Key, "Category", true) == 0)
                                            {
                                                StatMetrics.Add(Key, kvColl[Key].ToString());
                                                logger.Debug("ReadApplication  : ReadBusinessAttributeDetails : " + kvColl[Key].ToString());
                                            }
                                            else if (string.Compare(Key, "MainMask", true) == 0)
                                            {
                                                StatMetrics.Add(Key, kvColl[Key].ToString());
                                                logger.Debug("ReadApplication  : ReadBusinessAttributeDetails : " + kvColl[Key].ToString());
                                            }
                                            else if (string.Compare(Key, "RelMask", true) == 0)
                                            {
                                                StatMetrics.Add(Key, kvColl[Key].ToString());
                                                logger.Debug("ReadApplication  : ReadBusinessAttributeDetails : " + kvColl[Key].ToString());
                                            }
                                            else if (string.Compare(Key, "Subject", true) == 0)
                                            {
                                                StatMetrics.Add(Key, kvColl[Key].ToString());
                                                logger.Debug("ReadApplication  : ReadBusinessAttributeDetails : " + kvColl[Key].ToString());
                                            }
                                        }
                                        #endregion

                                        if (!StatisticsSetting.GetInstance().ReferenceIds.ContainsKey(section))
                                            StatisticsSetting.GetInstance().ReferenceIds.Add(section, acdQueueStat.ReferenceID.ToString());
                                        StatisticsSetting.GetInstance().statisticsCollection.ACDQueueStatistics.Add(acdQueueStat);
                                        StatisticsSetting.GetInstance().DictStatisticsMetrics.Add(section, StatMetrics);
                                        acdQueueStat = null;

                                        #endregion
                                    }
                                    else if (section.StartsWith("vq"))
                                    {
                                        #region Read Vitual Queue Statistics

                                        List<string> tempThresholdValues = new List<string>();
                                        List<Color> tempThresholdColors = new List<Color>();
                                        KeyValueCollection kvColl = new KeyValueCollection();
                                        kvColl = (KeyValueCollection)enumerator.UserProperties[section];
                                        Dictionary<string, string> StatMetrics = new Dictionary<string, string>();

                                        VirtualQueueStatistics virtualQueueStat = new VirtualQueueStatistics();
                                        virtualQueueStat.ReferenceID = TempReferenceId;
                                        virtualQueueStat.TempStatName = section.ToString();

                                        #region ANNEX
                                        logger.Debug("-------------------------------------------------------------------");
                                        logger.Debug("ReadApplication  : ReadBusinessAttributeDetails : " + section.ToString());
                                        logger.Debug("-------------------------------------------------------------------");

                                        foreach (string Key in kvColl.AllKeys)
                                        {
                                            if (string.Compare(Key, "Color1", true) == 0)
                                            {
                                                virtualQueueStat.StatColor = Color.FromName(kvColl[Key].ToString());
                                                logger.Debug("ReadApplication  : ReadBusinessAttributeDetails : " + kvColl[Key].ToString());
                                            }
                                            else if (string.Compare(Key, "Color2", true) == 0)
                                            {
                                                virtualQueueStat.ThresholdColorOne = Color.FromName(kvColl[Key].ToString());
                                                logger.Debug("ReadApplication  : ReadBusinessAttributeDetails : " + kvColl[Key].ToString());
                                            }
                                            else if (string.Compare(Key, "Color3", true) == 0)
                                            {
                                                virtualQueueStat.ThresholdColorTwo = Color.FromName(kvColl[Key].ToString());
                                                logger.Debug("ReadApplication  : ReadBusinessAttributeDetails : " + kvColl[Key].ToString());
                                            }
                                            else if (string.Compare(Key, "DisplayName", true) == 0)
                                            {
                                                virtualQueueStat.DisplayName = kvColl[Key].ToString();
                                                logger.Debug("ReadApplication  : ReadBusinessAttributeDetails : " + kvColl[Key].ToString());
                                            }
                                            else if (string.Compare(Key, "Filter", true) == 0)
                                            {
                                                virtualQueueStat.FilterName = kvColl[Key].ToString();
                                                logger.Debug("ReadApplication  : ReadBusinessAttributeDetails : " + kvColl[Key].ToString());
                                            }
                                            else if (string.Compare(Key, "Format", true) == 0)
                                            {
                                                virtualQueueStat.StatisticsFormat = kvColl[Key].ToString();
                                                logger.Debug("ReadApplication  : ReadBusinessAttributeDetails : " + kvColl[Key].ToString());
                                            }
                                            else if (string.Compare(Key, "StatName", true) == 0)
                                            {
                                                virtualQueueStat.StatisticsName = kvColl[Key].ToString();
                                                logger.Debug("ReadApplication  : ReadBusinessAttributeDetails : " + kvColl[Key].ToString());
                                            }
                                            else if (string.Compare(Key, "stype", true) == 0)
                                            {
                                                virtualQueueStat.StatisticsType = kvColl[Key].ToString();
                                                logger.Debug("ReadApplication  : ReadBusinessAttributeDetails : " + kvColl[Key].ToString());
                                            }
                                            else if (string.Compare(Key, "ThresLevel1", true) == 0)
                                            {
                                                virtualQueueStat.ThresholdLevelOne = kvColl[Key].ToString();
                                                logger.Debug("ReadApplication  : ReadBusinessAttributeDetails : " + kvColl[Key].ToString());
                                            }
                                            else if (string.Compare(Key, "ThresLevel2", true) == 0)
                                            {
                                                virtualQueueStat.ThresholdLevelTwo = kvColl[Key].ToString();
                                                logger.Debug("ReadApplication  : ReadBusinessAttributeDetails : " + kvColl[Key].ToString());
                                            }
                                            else if (string.Compare(Key, "TooltipName", true) == 0)
                                            {
                                                virtualQueueStat.ToolTipName = kvColl[Key].ToString();
                                                logger.Debug("ReadApplication  : ReadBusinessAttributeDetails : " + kvColl[Key].ToString());
                                            }
                                            else if (string.Compare(Key, "Category", true) == 0)
                                            {
                                                StatMetrics.Add(Key, kvColl[Key].ToString());
                                                logger.Debug("ReadApplication  : ReadBusinessAttributeDetails : " + kvColl[Key].ToString());
                                            }
                                            else if (string.Compare(Key, "MainMask", true) == 0)
                                            {
                                                StatMetrics.Add(Key, kvColl[Key].ToString());
                                                logger.Debug("ReadApplication  : ReadBusinessAttributeDetails : " + kvColl[Key].ToString());
                                            }
                                            else if (string.Compare(Key, "RelMask", true) == 0)
                                            {
                                                StatMetrics.Add(Key, kvColl[Key].ToString());
                                                logger.Debug("ReadApplication  : ReadBusinessAttributeDetails : " + kvColl[Key].ToString());
                                            }
                                            else if (string.Compare(Key, "Subject", true) == 0)
                                            {
                                                StatMetrics.Add(Key, kvColl[Key].ToString());
                                                logger.Debug("ReadApplication  : ReadBusinessAttributeDetails : " + kvColl[Key].ToString());
                                            }
                                        }
                                        #endregion

                                        if (!StatisticsSetting.GetInstance().ReferenceIds.ContainsKey(section))
                                            StatisticsSetting.GetInstance().ReferenceIds.Add(section, virtualQueueStat.ReferenceID.ToString());
                                        StatisticsSetting.GetInstance().statisticsCollection.VirtualQueueStatistics.Add(virtualQueueStat);
                                        StatisticsSetting.GetInstance().DictStatisticsMetrics.Add(section, StatMetrics);
                                        virtualQueueStat = null;

                                        #endregion
                                    }
                                    TempReferenceId++;
                                }

                                #endregion

                            }
                        }
                    }
                }
            }
            catch (Exception generalException)
            {
                logger.Error("ReadApplication : ReadBusinessAttribute Method: exception caught" + generalException.Message.ToString());
            }
        }

        /// <summary>
        /// Reads the DB details.
        /// </summary>
        /// <param name="StatisticsCollection">The statistics collection.</param>
        /// <returns></returns>
        public void ReadDBDetails()
        {
            try
            {
                StatisticsCommon commonStatistics = new StatisticsCommon();
                ApplicationContainer appcontainer = new ApplicationContainer();
                if (StatisticsSetting.GetInstance().statisticsCollection.DBValues == null)
                    StatisticsSetting.GetInstance().statisticsCollection.DBValues = new List<IDBValules>();

                XmlDataDocument xmldoc = new XmlDataDocument();
                XmlNodeList xmlnode;
                int i = 0;
                FileStream fs = new FileStream(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\PHS\\AgentInteractionDesktop\\app_db_config.xml", FileMode.Open, FileAccess.Read);
                xmldoc.Load(fs);

                #region Statistics Properties

                KeyValueCollection kvColl = new KeyValueCollection();
                xmlnode = xmldoc.GetElementsByTagName("dbstatistics");
                int DBStatID = 1000;

                if (xmlnode.Count != 0)
                {
                    for (i = 0; i <= xmlnode[0].ChildNodes.Count - 1; i++)
                    {
                        if (xmlnode[0].ChildNodes[i].NodeType == XmlNodeType.Element)
                        {

                            DBValues dbValues = new DBValues();
                            dbValues.ReferenceID = DBStatID;
                            dbValues.TempStat = xmlnode[0].ChildNodes[i].Name.ToString();

                            kvColl = new KeyValueCollection();
                            XmlNodeList xmlInnernode = xmldoc.GetElementsByTagName(xmlnode[0].ChildNodes[i].Name.ToString());

                            for (int j = 0; j <= xmlInnernode[0].ChildNodes.Count - 1; j++)
                            {
                                kvColl.Add(xmlInnernode[0].ChildNodes[j].Name.ToString(), xmlInnernode[0].ChildNodes[j].InnerText.Trim());
                            }

                            #region Read Statistics Values

                            #region DBStatistics
                            logger.Debug("-------------------------------------------------------------------");
                            logger.Debug("ReadApplication  : ReadApplicationDetails : " + kvColl["db.displayname"].ToString());
                            logger.Debug("-------------------------------------------------------------------");

                            foreach (string Key in kvColl.AllKeys)
                            {
                                if (string.Compare(Key, "db.color1", true) == 0)
                                {
                                    dbValues.Color1 = Color.FromName(kvColl[Key].ToString());
                                    logger.Info("ReadApplication  : ReadApplicationDetails : Color1 : " + kvColl[Key].ToString());
                                }
                                else if (string.Compare(Key, "db.color2", true) == 0)
                                {
                                    dbValues.Color2 = Color.FromName(kvColl[Key].ToString());
                                    logger.Info("ReadApplication  : ReadApplicationDetails : Color2 : " + kvColl[Key].ToString());
                                }
                                else if (string.Compare(Key, "db.color3", true) == 0)
                                {
                                    dbValues.Color3 = Color.FromName(kvColl[Key].ToString());
                                    logger.Info("ReadApplication  : ReadApplicationDetails : Color3 : " + kvColl[Key].ToString());
                                }
                                else if (string.Compare(Key, "db.displayname", true) == 0)
                                {
                                    dbValues.DisplayName = kvColl[Key].ToString();
                                    logger.Info("ReadApplication  : ReadApplicationDetails : DisplayName : " + kvColl[Key].ToString());
                                }
                                else if (string.Compare(Key, "db.query", true) == 0)
                                {
                                    dbValues.Query = kvColl[Key].ToString();
                                    logger.Info("ReadApplication  : ReadApplicationDetails : Query : " + kvColl[Key].ToString());
                                }
                                else if (string.Compare(Key, "db.threshold1", true) == 0)
                                {
                                    dbValues.Threshold1 = kvColl[Key].ToString();
                                    logger.Info("ReadApplication  : ReadApplicationDetails : ThresLevel1 : " + kvColl[Key].ToString());
                                }
                                else if (string.Compare(Key, "db.threshold2", true) == 0)
                                {
                                    dbValues.Threshold2 = kvColl[Key].ToString();
                                    logger.Info("ReadApplication  : ReadApplicationDetails : ThresLevel2 : " + kvColl[Key].ToString());
                                }
                                else if (string.Compare(Key, "db.tooltipname", true) == 0)
                                {
                                    dbValues.TooltipName = kvColl[Key].ToString();
                                    logger.Info("ReadApplication  : ReadApplicationDetails : TooltipName " + kvColl[Key].ToString());
                                }
                                else if (string.Compare(Key, "db.format", true) == 0)
                                {
                                    dbValues.Format = kvColl[Key].ToString();
                                    logger.Info("ReadApplication  : ReadApplicationDetails : format " + kvColl[Key].ToString());
                                }
                            }
                            #endregion
                            if (!StatisticsSetting.GetInstance().DictDBStatHolder.ContainsKey(DBStatID.ToString()))
                                StatisticsSetting.GetInstance().DictDBStatHolder.Add(DBStatID.ToString(), dbValues);
                            else
                                StatisticsSetting.GetInstance().DictDBStatHolder[DBStatID.ToString()] = dbValues;

                            StatisticsSetting.GetInstance().statisticsCollection.DBValues.Add(dbValues);

                            dbValues = null;

                            #endregion

                            DBStatID++;
                        }
                    }
                }

                #endregion

                if (StatisticsBase.GetInstance().StatSource == StatisticsEnum.StatSource.DB.ToString() || (!StatisticsBase.GetInstance().isCMEAuthentication && StatisticsBase.GetInstance().StatSource == StatisticsEnum.StatSource.All.ToString()))
                {
                    #region Enable-Disable Channels

                    xmlnode = xmldoc.GetElementsByTagName("enable.disable-channels");
                    for (i = 0; i <= xmlnode[0].ChildNodes.Count - 1; i++)
                    {
                        XmlNodeList xmlInnernode = xmldoc.GetElementsByTagName(xmlnode[0].ChildNodes[i].Name.ToString());
                        if (xmlInnernode.Count != 0)
                        {
                            for (int j = 0; j <= xmlInnernode[0].ChildNodes.Count - 1; j++)
                            {
                                if (xmlInnernode[0].ChildNodes[j].ParentNode.Name.ToString().Trim().ToLower() == "statistics.enable-alwaysontop")
                                {
                                    if (xmlInnernode[0].ChildNodes[j].InnerText.Trim() != string.Empty && xmlInnernode[0].ChildNodes[j].InnerText.Trim() != "")
                                    {
                                        appcontainer.EnableAlwaysOnTop = Convert.ToBoolean(xmlInnernode[0].ChildNodes[j].InnerText.Trim());
                                        if (!StatisticsSetting.GetInstance().DictEnableDisableChannels.ContainsKey("AlwaysOnTop"))
                                            StatisticsSetting.GetInstance().DictEnableDisableChannels.Add("AlwaysOnTop", appcontainer.EnableAlwaysOnTop);
                                        else
                                            StatisticsSetting.GetInstance().DictEnableDisableChannels["AlwaysOnTop"] = appcontainer.EnableAlwaysOnTop;
                                    }
                                }
                                else if (xmlInnernode[0].ChildNodes[j].ParentNode.Name.ToString().Trim().ToLower() == "statistics.enable-hhmmssformat")
                                {
                                    if (xmlInnernode[0].ChildNodes[j].InnerText.Trim() != string.Empty && xmlInnernode[0].ChildNodes[j].InnerText.Trim() != "")
                                    {
                                        appcontainer.EnableHHMMSS = Convert.ToBoolean(xmlInnernode[0].ChildNodes[j].InnerText.Trim());

                                        if (!StatisticsSetting.GetInstance().DictEnableDisableChannels.ContainsKey("hhmmssformat"))
                                            StatisticsSetting.GetInstance().DictEnableDisableChannels.Add("hhmmssformat", appcontainer.EnableHHMMSS);
                                        else
                                            StatisticsSetting.GetInstance().DictEnableDisableChannels["hhmmssformat"] = appcontainer.EnableHHMMSS;
                                    }
                                }
                                else if (xmlInnernode[0].ChildNodes[j].ParentNode.Name.ToString().Trim().ToLower() == "statistics.enable-log")
                                {
                                    if (xmlInnernode[0].ChildNodes[j].InnerText.Trim() != string.Empty && xmlInnernode[0].ChildNodes[j].InnerText.Trim() != "")
                                    {
                                        appcontainer.EnableMainGadget = Convert.ToBoolean(xmlInnernode[0].ChildNodes[j].InnerText.Trim());

                                        if (!StatisticsSetting.GetInstance().DictEnableDisableChannels.ContainsKey("log"))
                                            StatisticsSetting.GetInstance().DictEnableDisableChannels.Add("log", appcontainer.EnableLog);
                                        else
                                            StatisticsSetting.GetInstance().DictEnableDisableChannels["log"] = appcontainer.EnableLog;
                                    }
                                }
                                else if (xmlInnernode[0].ChildNodes[j].ParentNode.Name.ToString().Trim().ToLower() == "statistics.enable-maingadget")
                                {
                                    if (xmlInnernode[0].ChildNodes[j].InnerText.Trim() != string.Empty && xmlInnernode[0].ChildNodes[j].InnerText.Trim() != "")
                                    {
                                        appcontainer.EnableMainGadget = Convert.ToBoolean(xmlInnernode[0].ChildNodes[j].InnerText.Trim());

                                        if (!StatisticsSetting.GetInstance().DictEnableDisableChannels.ContainsKey("maingadget"))
                                            StatisticsSetting.GetInstance().DictEnableDisableChannels.Add("maingadget", appcontainer.EnableMainGadget);
                                        else
                                            StatisticsSetting.GetInstance().DictEnableDisableChannels["maingadget"] = appcontainer.EnableMainGadget;
                                    }
                                }
                                else if (xmlInnernode[0].ChildNodes[j].ParentNode.Name.ToString().Trim().ToLower() == "statistics.enable-menu-button")
                                {
                                    if (xmlInnernode[0].ChildNodes[j].InnerText.Trim() != string.Empty && xmlInnernode[0].ChildNodes[j].InnerText.Trim() != "")
                                    {
                                        appcontainer.EnableMenuButton = Convert.ToBoolean(xmlInnernode[0].ChildNodes[j].InnerText.Trim());

                                        if (!StatisticsSetting.GetInstance().DictEnableDisableChannels.ContainsKey("menubutton"))
                                            StatisticsSetting.GetInstance().DictEnableDisableChannels.Add("menubutton", appcontainer.EnableMenuButton);
                                        else
                                            StatisticsSetting.GetInstance().DictEnableDisableChannels["menubutton"] = appcontainer.EnableMenuButton;
                                    }
                                }
                                else if (xmlInnernode[0].ChildNodes[j].ParentNode.Name.ToString().Trim().ToLower() == "statistics.enable-notification-balloon")
                                {
                                    if (xmlInnernode[0].ChildNodes[j].InnerText.Trim() != string.Empty && xmlInnernode[0].ChildNodes[j].InnerText.Trim() != "")
                                    {
                                        appcontainer.EnableNotificationBalloon = Convert.ToBoolean(xmlInnernode[0].ChildNodes[j].InnerText.Trim());
                                        if (!StatisticsSetting.GetInstance().DictEnableDisableChannels.ContainsKey("notificationballoon"))
                                            StatisticsSetting.GetInstance().DictEnableDisableChannels.Add("notificationballoon", appcontainer.EnableNotificationBalloon);
                                        else
                                            StatisticsSetting.GetInstance().DictEnableDisableChannels["notificationballoon"] = appcontainer.EnableNotificationBalloon;
                                    }
                                }
                                else if (xmlInnernode[0].ChildNodes[j].ParentNode.Name.ToString().Trim().ToLower() == "statistics.enable-notification-close")
                                {
                                    if (xmlInnernode[0].ChildNodes[j].InnerText.Trim() != string.Empty && xmlInnernode[0].ChildNodes[j].InnerText.Trim() != "")
                                    {
                                        appcontainer.EnableTaskbarClose = Convert.ToBoolean(xmlInnernode[0].ChildNodes[j].InnerText.Trim());

                                        if (!StatisticsSetting.GetInstance().DictEnableDisableChannels.ContainsKey("notificationclose"))
                                            StatisticsSetting.GetInstance().DictEnableDisableChannels.Add("notificationclose", appcontainer.EnableTaskbarClose);
                                        else
                                            StatisticsSetting.GetInstance().DictEnableDisableChannels["notificationclose"] = appcontainer.EnableTaskbarClose;
                                    }
                                }
                                else if (xmlInnernode[0].ChildNodes[j].ParentNode.Name.ToString().Trim().ToLower() == "statistics.enable-submenu-close-gadget")
                                {
                                    if (xmlInnernode[0].ChildNodes[j].InnerText.Trim() != string.Empty && xmlInnernode[0].ChildNodes[j].InnerText.Trim() != "")
                                    {
                                        appcontainer.EnableMenuClose = Convert.ToBoolean(xmlInnernode[0].ChildNodes[j].InnerText.Trim());

                                        if (!StatisticsSetting.GetInstance().DictEnableDisableChannels.ContainsKey("closegadget"))
                                            StatisticsSetting.GetInstance().DictEnableDisableChannels.Add("closegadget", appcontainer.EnableMenuClose);
                                        else
                                            StatisticsSetting.GetInstance().DictEnableDisableChannels["closegadget"] = appcontainer.EnableMenuClose;
                                    }
                                }
                                else if (xmlInnernode[0].ChildNodes[j].ParentNode.Name.ToString().Trim().ToLower() == "statistics.enable-submenu-ontop")
                                {
                                    if (xmlInnernode[0].ChildNodes[j].InnerText.Trim() != string.Empty && xmlInnernode[0].ChildNodes[j].InnerText.Trim() != "")
                                    {
                                        appcontainer.EnableMenuOnTop = Convert.ToBoolean(xmlInnernode[0].ChildNodes[j].InnerText.Trim());
                                        if (!StatisticsSetting.GetInstance().DictEnableDisableChannels.ContainsKey("ontop"))
                                            StatisticsSetting.GetInstance().DictEnableDisableChannels.Add("ontop", appcontainer.EnableMenuOnTop);
                                        else
                                            StatisticsSetting.GetInstance().DictEnableDisableChannels["ontop"] = appcontainer.EnableMenuOnTop;
                                    }
                                }
                                else if (xmlInnernode[0].ChildNodes[j].ParentNode.Name.ToString().Trim().ToLower() == "statistics.enable-systemdraggable")
                                {
                                    if (xmlInnernode[0].ChildNodes[j].InnerText.Trim() != string.Empty && xmlInnernode[0].ChildNodes[j].InnerText.Trim() != "")
                                    {
                                        appcontainer.EnableSystemDraggable = Convert.ToBoolean(xmlInnernode[0].ChildNodes[j].InnerText.Trim());
                                        StatisticsBase.GetInstance().IsSystemDraggable = Convert.ToBoolean(xmlInnernode[0].ChildNodes[j].InnerText.Trim());

                                        if (!StatisticsSetting.GetInstance().DictEnableDisableChannels.ContainsKey("systemdraggable"))
                                            StatisticsSetting.GetInstance().DictEnableDisableChannels.Add("systemdraggable", appcontainer.EnableSystemDraggable);
                                        else
                                            StatisticsSetting.GetInstance().DictEnableDisableChannels["systemdraggable"] = appcontainer.EnableSystemDraggable;
                                    }
                                }
                                else if (xmlInnernode[0].ChildNodes[j].ParentNode.Name.ToString().Trim().ToLower() == "statistics.enable-tag-button")
                                {
                                    if (xmlInnernode[0].ChildNodes[j].InnerText.Trim() != string.Empty && xmlInnernode[0].ChildNodes[j].InnerText.Trim() != "")
                                    {
                                        appcontainer.EnableTagButton = Convert.ToBoolean(xmlInnernode[0].ChildNodes[j].InnerText.Trim());
                                        if (!StatisticsSetting.GetInstance().DictEnableDisableChannels.ContainsKey("tagbutton"))
                                            StatisticsSetting.GetInstance().DictEnableDisableChannels.Add("tagbutton", appcontainer.EnableTagButton);
                                        else
                                            StatisticsSetting.GetInstance().DictEnableDisableChannels["tagbutton"] = appcontainer.EnableTagButton;
                                    }
                                }
                                else if (xmlInnernode[0].ChildNodes[j].ParentNode.Name.ToString().Trim().ToLower() == "statistics.enable-tag-vertical")
                                {
                                    if (xmlInnernode[0].ChildNodes[j].InnerText.Trim() != string.Empty && xmlInnernode[0].ChildNodes[j].InnerText.Trim() != "")
                                    {
                                        appcontainer.EnableTagVertical = Convert.ToBoolean(xmlInnernode[0].ChildNodes[j].InnerText.Trim());
                                        if (!StatisticsSetting.GetInstance().DictEnableDisableChannels.ContainsKey("tagvertical"))
                                            StatisticsSetting.GetInstance().DictEnableDisableChannels.Add("tagvertical", appcontainer.EnableTagVertical);
                                        else
                                            StatisticsSetting.GetInstance().DictEnableDisableChannels["tagvertical"] = appcontainer.EnableTagVertical;
                                    }
                                }
                                else if (xmlInnernode[0].ChildNodes[j].ParentNode.Name.ToString().Trim().ToLower() == "statistics.enable-untag-button")
                                {
                                    if (xmlInnernode[0].ChildNodes[j].InnerText.Trim() != string.Empty && xmlInnernode[0].ChildNodes[j].InnerText.Trim() != "")
                                    {
                                        appcontainer.EnableUntagButton = Convert.ToBoolean(xmlInnernode[0].ChildNodes[j].InnerText.Trim());
                                        if (!StatisticsSetting.GetInstance().DictEnableDisableChannels.ContainsKey("untagbutton"))
                                            StatisticsSetting.GetInstance().DictEnableDisableChannels.Add("untagbutton", appcontainer.EnableUntagButton);
                                        else
                                            StatisticsSetting.GetInstance().DictEnableDisableChannels["untagbutton"] = appcontainer.EnableUntagButton;
                                    }
                                }
                                else if (xmlInnernode[0].ChildNodes[j].ParentNode.Name.ToString().Trim().ToLower() == "statistics.enable-submenu-alertnotification")
                                {
                                    if (xmlInnernode[0].ChildNodes[j].InnerText.Trim() != string.Empty && xmlInnernode[0].ChildNodes[j].InnerText.Trim() != "")
                                    {
                                        appcontainer.EnableThresholdNotification = Convert.ToBoolean(xmlInnernode[0].ChildNodes[j].InnerText.Trim());
                                        if (!StatisticsSetting.GetInstance().DictEnableDisableChannels.ContainsKey("thresholdnotification"))
                                            StatisticsSetting.GetInstance().DictEnableDisableChannels.Add("thresholdnotification", appcontainer.EnableThresholdNotification);
                                        else
                                            StatisticsSetting.GetInstance().DictEnableDisableChannels["thresholdnotification"] = appcontainer.EnableThresholdNotification;
                                    }
                                }
                                else if (xmlInnernode[0].ChildNodes[j].ParentNode.Name.ToString().Trim().ToLower() == "statistics.enable-submenu-showheader")
                                {
                                    if (xmlInnernode[0].ChildNodes[j].InnerText.Trim() != string.Empty && xmlInnernode[0].ChildNodes[j].InnerText.Trim() != "")
                                    {
                                        if (!StatisticsSetting.GetInstance().DictEnableDisableChannels.ContainsKey("showheader"))
                                            StatisticsSetting.GetInstance().DictEnableDisableChannels.Add("showheader", Convert.ToBoolean(xmlInnernode[0].ChildNodes[j].InnerText.Trim()));
                                        else
                                            StatisticsSetting.GetInstance().DictEnableDisableChannels["showheader"] = Convert.ToBoolean(xmlInnernode[0].ChildNodes[j].InnerText.Trim());
                                    }
                                }
                                else if (xmlInnernode[0].ChildNodes[j].ParentNode.Name.ToString().Trim().ToLower() == "statistics.enable-header")
                                {
                                    if (xmlInnernode[0].ChildNodes[j].InnerText.Trim() != string.Empty && xmlInnernode[0].ChildNodes[j].InnerText.Trim() != "")
                                    {
                                        if (!StatisticsSetting.GetInstance().DictEnableDisableChannels.ContainsKey("isheaderenabled"))
                                            StatisticsSetting.GetInstance().DictEnableDisableChannels.Add("isheaderenabled", Convert.ToBoolean(xmlInnernode[0].ChildNodes[j].InnerText.Trim()));
                                        else
                                            StatisticsSetting.GetInstance().DictEnableDisableChannels["isheaderenabled"] = Convert.ToBoolean(xmlInnernode[0].ChildNodes[j].InnerText.Trim());
                                    }
                                }
                                else if (xmlInnernode[0].ChildNodes[j].ParentNode.Name.ToString().Trim().ToLower() == "statistics.enable-notify-primary-screen")
                                {
                                    if (xmlInnernode[0].ChildNodes[j].InnerText.Trim() != string.Empty && xmlInnernode[0].ChildNodes[j].InnerText.Trim() != "")
                                    {
                                        if (!StatisticsSetting.GetInstance().DictEnableDisableChannels.ContainsKey("notifyprimaryscreen"))
                                            StatisticsSetting.GetInstance().DictEnableDisableChannels.Add("notifyprimaryscreen", Convert.ToBoolean(xmlInnernode[0].ChildNodes[j].InnerText.Trim()));
                                        else
                                            StatisticsSetting.GetInstance().DictEnableDisableChannels["notifyprimaryscreen"] = Convert.ToBoolean(xmlInnernode[0].ChildNodes[j].InnerText.Trim());
                                    }
                                }
                            }
                        }
                    }

                    StatisticsSetting.GetInstance().statisticsCollection.ApplicationContainer = appcontainer;

                    #endregion

                    #region Alert

                    xmlnode = xmldoc.GetElementsByTagName("alert");
                    for (i = 0; i <= xmlnode[0].ChildNodes.Count - 1; i++)
                    {
                        XmlNodeList xmlInnernode = xmldoc.GetElementsByTagName(xmlnode[0].ChildNodes[i].Name.ToString());
                        if (xmlInnernode.Count != 0)
                        {
                            for (int j = 0; j <= xmlInnernode[0].ChildNodes.Count - 1; j++)
                            {
                                string sss = xmlInnernode[0].ChildNodes[j].ParentNode.Name.ToString().Trim().ToLower();
                                if (xmlInnernode[0].ChildNodes[j].ParentNode.Name.ToString().Trim().ToLower() == "statistics.alert.audio-path")
                                {
                                    if (xmlInnernode[0].ChildNodes[j].InnerText.Trim() != string.Empty && xmlInnernode[0].ChildNodes[j].InnerText.Trim() != "")
                                    {
                                        StatisticsBase.GetInstance().ThresholdBreachAlertPath = xmlInnernode[0].ChildNodes[j].InnerText.Trim();
                                    }
                                }
                                else if (xmlInnernode[0].ChildNodes[j].ParentNode.Name.ToString().Trim().ToLower() == "statistics.alert.audio-play.attempt")
                                {
                                    if (xmlInnernode[0].ChildNodes[j].InnerText.Trim() != string.Empty && xmlInnernode[0].ChildNodes[j].InnerText.Trim() != "")
                                    {
                                        StatisticsBase.GetInstance().AudioPlayAttempt = Convert.ToInt32(xmlInnernode[0].ChildNodes[j].InnerText.Trim());
                                    }
                                    else
                                    {
                                        StatisticsBase.GetInstance().AudioPlayAttempt = 3;
                                    }
                                }
                                else if (xmlInnernode[0].ChildNodes[j].ParentNode.Name.ToString().Trim().ToLower() == "statistics.alert.audio-play.duration")
                                {
                                    if (xmlInnernode[0].ChildNodes[j].InnerText.Trim() != string.Empty && xmlInnernode[0].ChildNodes[j].InnerText.Trim() != "")
                                    {
                                        StatisticsBase.GetInstance().AlertAudioDuration = Convert.ToInt32(xmlInnernode[0].ChildNodes[j].InnerText.Trim());
                                    }
                                    else
                                    {
                                        StatisticsBase.GetInstance().AlertAudioDuration = 3;
                                    }
                                }
                                else if (xmlInnernode[0].ChildNodes[j].ParentNode.Name.ToString().Trim().ToLower() == "statistics.alert.popup.attempt")
                                {
                                    if (xmlInnernode[0].ChildNodes[j].InnerText.Trim() != string.Empty && xmlInnernode[0].ChildNodes[j].InnerText.Trim() != "")
                                    {
                                        StatisticsBase.GetInstance().AlertPopupAttempt = Convert.ToInt32(xmlInnernode[0].ChildNodes[j].InnerText.Trim());
                                    }
                                    else
                                    {
                                        StatisticsBase.GetInstance().AlertPopupAttempt = 3;
                                    }
                                }
                                else if (xmlInnernode[0].ChildNodes[j].ParentNode.Name.ToString().Trim().ToLower() == "statistics.alert.popup.duration")
                                {
                                    if (xmlInnernode[0].ChildNodes[j].InnerText.Trim() != string.Empty && xmlInnernode[0].ChildNodes[j].InnerText.Trim() != "")
                                    {
                                        StatisticsBase.GetInstance().AlertNotifyTime = Convert.ToInt32(xmlInnernode[0].ChildNodes[j].InnerText.Trim());
                                    }
                                    else
                                    {
                                        StatisticsBase.GetInstance().AlertNotifyTime = 3;
                                    }
                                }
                                else if (xmlInnernode[0].ChildNodes[j].ParentNode.Name.ToString().Trim().ToLower() == "statistics.alert.popup.font-bold")
                                {
                                    if (xmlInnernode[0].ChildNodes[j].InnerText.Trim() != string.Empty && xmlInnernode[0].ChildNodes[j].InnerText.Trim() != "")
                                    {
                                        StatisticsBase.GetInstance().IsThresholdNotifierBold = Convert.ToBoolean(xmlInnernode[0].ChildNodes[j].InnerText.Trim());
                                    }
                                    else
                                    {
                                        StatisticsBase.GetInstance().IsThresholdNotifierBold = false;
                                    }
                                }
                                else if (xmlInnernode[0].ChildNodes[j].ParentNode.Name.ToString().Trim().ToLower() == "statistics.alert.popup.font-color")
                                {
                                    if (xmlInnernode[0].ChildNodes[j].InnerText.Trim() != string.Empty && xmlInnernode[0].ChildNodes[j].InnerText.Trim() != "")
                                    {
                                        StatisticsBase.GetInstance().ThresholdContentForeground = xmlInnernode[0].ChildNodes[j].InnerText.Trim();
                                        commonStatistics.NotifyForeground = xmlInnernode[0].ChildNodes[j].InnerText.Trim();
                                    }
                                    else
                                    {
                                        StatisticsBase.GetInstance().ThresholdContentForeground = "Black";
                                    }
                                }
                                else if (xmlInnernode[0].ChildNodes[j].ParentNode.Name.ToString().Trim().ToLower() == "statistics.alert.popup.screen-color")
                                {
                                    if (xmlInnernode[0].ChildNodes[j].InnerText.Trim() != string.Empty && xmlInnernode[0].ChildNodes[j].InnerText.Trim() != "")
                                    {
                                        if (xmlInnernode[0].ChildNodes[j].InnerText.Trim().Contains(','))
                                        {
                                            string[] Backgrounds = xmlInnernode[0].ChildNodes[j].InnerText.Trim().Split(',');
                                            StatisticsBase.GetInstance().ThresholdTitleBackground = Backgrounds[0].ToString();
                                            StatisticsBase.GetInstance().ThresholdContentBackground = Backgrounds[1].ToString();
                                        }
                                        else
                                        {
                                            StatisticsBase.GetInstance().ThresholdTitleBackground = StatisticsBase.GetInstance().ThresholdContentBackground = xmlInnernode[0].ChildNodes[j].InnerText.Trim();
                                        }

                                        commonStatistics.NotifyBackground = xmlInnernode[0].ChildNodes[j].InnerText.Trim();
                                    }
                                    else
                                    {
                                        StatisticsBase.GetInstance().ThresholdTitleBackground = "#007edf";
                                        StatisticsBase.GetInstance().ThresholdContentBackground = "#FFFFFF";
                                    }
                                }
                            }
                        }

                        StatisticsSetting.GetInstance().statisticsCollection.StatisticsCommon = commonStatistics;
                    }

                    #endregion

                    #region _errors_

                    xmlnode = xmldoc.GetElementsByTagName("_errors_");
                    for (i = 0; i <= xmlnode[0].ChildNodes.Count - 1; i++)
                    {
                        XmlNodeList xmlInnernode = xmldoc.GetElementsByTagName(xmlnode[0].ChildNodes[i].Name.ToString());
                        if (xmlInnernode.Count != 0)
                        {
                            for (int j = 0; j <= xmlInnernode[0].ChildNodes.Count - 1; j++)
                            {
                                if (string.Compare(xmlInnernode[0].ChildNodes[j].ParentNode.Name.ToString().Trim(), "db.authentication", true) == 0)
                                {

                                    if (xmlInnernode[0].ChildNodes[j].InnerText.Trim() != string.Empty && xmlInnernode[0].ChildNodes[j].InnerText.Trim() != "")
                                    {
                                        appcontainer.dbAuthentication = xmlInnernode[0].ChildNodes[j].InnerText.Trim();
                                        if (!StatisticsSetting.GetInstance().DictErrorValues.ContainsKey("db.authentication"))
                                            StatisticsSetting.GetInstance().DictErrorValues.Add("db.authentication", xmlInnernode[0].ChildNodes[j].InnerText.Trim());
                                        else
                                            StatisticsSetting.GetInstance().DictErrorValues["db.authentication"] = xmlInnernode[0].ChildNodes[j].InnerText.Trim();
                                    }
                                }
                                else if (string.Compare(xmlInnernode[0].ChildNodes[j].ParentNode.Name.ToString().Trim(), "db.connection", true) == 0)
                                {
                                    if (xmlInnernode[0].ChildNodes[j].InnerText.Trim() != string.Empty && xmlInnernode[0].ChildNodes[j].InnerText.Trim() != "")
                                    {
                                        appcontainer.dbConnection = xmlInnernode[0].ChildNodes[j].InnerText.Trim();
                                        if (!StatisticsSetting.GetInstance().DictErrorValues.ContainsKey("db.connection"))
                                            StatisticsSetting.GetInstance().DictErrorValues.Add("db.connection", xmlInnernode[0].ChildNodes[j].InnerText.Trim());
                                        else
                                            StatisticsSetting.GetInstance().DictErrorValues["db.connection"] = xmlInnernode[0].ChildNodes[j].InnerText.Trim();
                                    }
                                }
                                else if (string.Compare(xmlInnernode[0].ChildNodes[j].ParentNode.Name.ToString().Trim(), "nostat.configured", true) == 0)
                                {
                                    if (xmlInnernode[0].ChildNodes[j].InnerText.Trim() != string.Empty && xmlInnernode[0].ChildNodes[j].InnerText.Trim() != "")
                                    {
                                        appcontainer.NoStats = xmlInnernode[0].ChildNodes[j].InnerText.Trim();
                                        if (!StatisticsSetting.GetInstance().DictErrorValues.ContainsKey("nostat.configured"))
                                            StatisticsSetting.GetInstance().DictErrorValues.Add("nostat.configured", xmlInnernode[0].ChildNodes[j].InnerText.Trim());
                                        else
                                            StatisticsSetting.GetInstance().DictErrorValues["nostat.configured"] = xmlInnernode[0].ChildNodes[j].InnerText.Trim();

                                    }
                                }
                            }
                        }
                    }

                    #endregion
                }

                if ((StatisticsBase.GetInstance().StatSource == StatisticsEnum.StatSource.All.ToString() && StatisticsBase.GetInstance().isDBAuthentication) || (StatisticsBase.GetInstance().StatSource == StatisticsEnum.StatSource.DB.ToString() && StatisticsBase.GetInstance().isDBAuthentication) || (StatisticsBase.GetInstance().StatSource == StatisticsEnum.StatSource.All.ToString() && !StatisticsBase.GetInstance().DefaultAuthentication.Contains(StatisticsEnum.StatSource.DB.ToString())))
                {
                    #region Tagged Statistics

                    xmlnode = xmldoc.GetElementsByTagName("DbTaggedStats");

                    string[] TaggedStats = xmlnode[0].InnerText.Split(',');

                    int TagOrder = 1;
                    if (StatisticsSetting.GetInstance().DictTaggedStats.Count != 0)
                    {
                        TagOrder = StatisticsSetting.GetInstance().DictTaggedStats.Count + 1;
                    }

                    foreach (string stats in TaggedStats)
                    {
                        foreach (DBValues dbValues in StatisticsSetting.GetInstance().statisticsCollection.DBValues)
                        {
                            if ((dbValues.TempStat == stats) && (!StatisticsSetting.GetInstance().DictTaggedStats.ContainsKey(TagOrder.ToString())) && ((!StatisticsSetting.GetInstance().DictTaggedStats.ContainsValue(dbValues.DisplayName)) || (!StatisticsSetting.GetInstance().DictTaggedStats.Keys.Contains(stats))))
                            {
                                StatisticsSetting.GetInstance().DictTaggedStats.Add(TagOrder.ToString() + ',' + stats, dbValues.DisplayName);
                                TagOrder++;
                                break;
                            }
                        }
                    }

                    xmlnode = xmldoc.GetElementsByTagName("DbColor");

                    StatisticsSetting.GetInstance().statisticsCollection.StatisticsCommon.DBColor = Color.FromName(xmlnode[0].InnerText.ToString());

                    #endregion
                }

            }
            catch (Exception GeneralException)
            {
                logger.Error("ReadApplication : ReadLoggerDetails : " + GeneralException.Message.ToString());
            }
        }

        /// <summary>
        /// Reads the database logger details.
        /// </summary>
        public void ReadDbLoggerDetails()
        {
            ApplicationContainer appcontainer = new ApplicationContainer();
            XmlDataDocument xmldoc = new XmlDataDocument();
            XmlNodeList xmlnode;
            int i = 0;
            FileStream fs = new FileStream(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\PHS\\AgentInteractionDesktop\\app_db_config.xml", FileMode.Open, FileAccess.Read);
            xmldoc.Load(fs);
            if (StatisticsBase.GetInstance().StatSource == StatisticsEnum.StatSource.DB.ToString() || (!StatisticsBase.GetInstance().isCMEAuthentication && StatisticsBase.GetInstance().StatSource == StatisticsEnum.StatSource.All.ToString()))
            {
                #region Enable-Disable Channels

                xmlnode = xmldoc.GetElementsByTagName("enable.disable-channels");
                for (i = 0; i <= xmlnode[0].ChildNodes.Count - 1; i++)
                {
                    XmlNodeList xmlInnernode = xmldoc.GetElementsByTagName(xmlnode[0].ChildNodes[i].Name.ToString());
                    if (xmlInnernode.Count != 0)
                    {
                        for (int j = 0; j <= xmlInnernode[0].ChildNodes.Count - 1; j++)
                        {
                            string sss = xmlInnernode[0].ChildNodes[j].ParentNode.Name.ToString().Trim().ToLower();

                            if (xmlInnernode[0].ChildNodes[j].ParentNode.Name.ToString().Trim().ToLower() == "statistics.enable-log")
                            {
                                appcontainer.EnableLog = Convert.ToBoolean(xmlInnernode[0].ChildNodes[j].InnerText.Trim());

                                if (!StatisticsSetting.GetInstance().DictEnableDisableChannels.ContainsKey("log"))
                                    StatisticsSetting.GetInstance().DictEnableDisableChannels.Add("log", appcontainer.EnableLog);
                                else
                                    StatisticsSetting.GetInstance().DictEnableDisableChannels["log"] = appcontainer.EnableLog;
                            }
                        }
                    }
                }

                if (!StatisticsSetting.GetInstance().DictEnableDisableChannels.ContainsKey("log"))
                {
                    StatisticsSetting.GetInstance().DictEnableDisableChannels.Add("log", false);
                    StatisticsBase.GetInstance().ListMissedValues.Add("statistics.enable-log");
                }
                if (StatisticsSetting.GetInstance().statisticsCollection == null)
                    StatisticsSetting.GetInstance().statisticsCollection = new StatisticsCollection();
                StatisticsSetting.GetInstance().statisticsCollection.ApplicationContainer = appcontainer;

                #endregion

                #region statistics.log

                xmlnode = xmldoc.GetElementsByTagName("statistics.log");
                for (i = 0; i <= xmlnode[0].ChildNodes.Count - 1; i++)
                {
                    XmlNodeList xmlInnernode = xmldoc.GetElementsByTagName(xmlnode[0].ChildNodes[i].Name.ToString());
                    if (xmlInnernode.Count != 0)
                    {
                        for (int j = 0; j <= xmlInnernode[0].ChildNodes.Count - 1; j++)
                        {
                            string sss = xmlInnernode[0].ChildNodes[j].ParentNode.Name.ToString().Trim().ToLower();
                            if (xmlInnernode[0].ChildNodes[j].ParentNode.Name.ToString().Trim().ToLower() == "statistics.log.conversionpattern")
                                StatisticsSetting.GetInstance().logconversionPattern = xmlInnernode[0].ChildNodes[j].InnerText.Trim();
                            else if (xmlInnernode[0].ChildNodes[j].ParentNode.Name.ToString().Trim().ToLower() == "statistics.log.datepattern")
                                StatisticsSetting.GetInstance().logdatePattern = xmlInnernode[0].ChildNodes[j].InnerText.Trim();
                            else if (xmlInnernode[0].ChildNodes[j].ParentNode.Name.ToString().Trim().ToLower() == "statistics.log.filename")
                                StatisticsSetting.GetInstance().logFileName = xmlInnernode[0].ChildNodes[j].InnerText.Trim();
                            else if (xmlInnernode[0].ChildNodes[j].ParentNode.Name.ToString().Trim().ToLower() == "statistics.log.filterlevel")
                                StatisticsSetting.GetInstance().logFilterLevel = xmlInnernode[0].ChildNodes[j].InnerText.Trim();
                            else if (xmlInnernode[0].ChildNodes[j].ParentNode.Name.ToString().Trim().ToLower() == "statistics.log.level")
                                StatisticsSetting.GetInstance().logLevel = xmlInnernode[0].ChildNodes[j].InnerText.Trim();
                            else if (xmlInnernode[0].ChildNodes[j].ParentNode.Name.ToString().Trim().ToLower() == "statistics.log.maxfilesize")
                                StatisticsSetting.GetInstance().logmaxFileSize = xmlInnernode[0].ChildNodes[j].InnerText.Trim();
                            else if (xmlInnernode[0].ChildNodes[j].ParentNode.Name.ToString().Trim().ToLower() == "statistics.log.maxlevel")
                                StatisticsSetting.GetInstance().logMaxLevel = xmlInnernode[0].ChildNodes[j].InnerText.Trim();
                            else if (xmlInnernode[0].ChildNodes[j].ParentNode.Name.ToString().Trim().ToLower() == "statistics.log.maxrollbacksize")
                                StatisticsSetting.GetInstance().logmaxSizeRoll = xmlInnernode[0].ChildNodes[j].InnerText.ToString().Trim();
                            else if (xmlInnernode[0].ChildNodes[j].ParentNode.Name.ToString().Trim().ToLower() == "statistics.log.minlevel")
                                StatisticsSetting.GetInstance().logMinLevel = xmlInnernode[0].ChildNodes[j].InnerText.Trim();
                            else if (xmlInnernode[0].ChildNodes[j].ParentNode.Name.ToString().Trim().ToLower() == "statistics.log.rollstyle")
                                StatisticsSetting.GetInstance().rollingStyle = xmlInnernode[0].ChildNodes[j].InnerText.Trim();
                        }
                    }
                }

                #endregion
            }
        }

        /// <summary>
        /// Reads the logger details.
        /// </summary>
        /// <param name="applicationName">Name of the application.</param>
        public bool ReadLoggerDetails(string applicationName)
        {
            try
            {
                logger.Debug("ReadApplication : ReadLoggerDetails Method: Entry");
                CfgApplication Application = new CfgApplication(StatisticsSetting.GetInstance().confObject);
                CfgApplicationQuery queryApp = new CfgApplicationQuery();
                queryApp.Name = applicationName;
                Application = StatisticsSetting.GetInstance().confObject.RetrieveObject<CfgApplication>(queryApp);
                if (Application != null)
                {
                    if (Application.AppServers != null && Application.AppServers.Count != 0)
                    {
                        foreach (CfgConnInfo appDetails in Application.AppServers)
                        {
                            if (appDetails.AppServer.Type == CfgAppType.CFGStatServer)
                            {
                                StatisticsSetting.GetInstance().primaryStatServer = appDetails.AppServer;
                            }
                        }
                    }

                    KeyValueCollection statisticslog = (KeyValueCollection)Application.Options["agent.ixn.desktop"];
                    if (statisticslog != null)
                    {
                        #region StatisticsLog

                        foreach (string key in statisticslog.AllKeys)
                        {
                            if (string.Compare(key, "log.conversion-pattern", true) == 0)
                            {
                                if (statisticslog[key].ToString() != string.Empty)
                                    StatisticsSetting.GetInstance().logconversionPattern = statisticslog[key].ToString();
                            }
                            else if (string.Compare(key, "log.date-pattern", true) == 0)
                            {
                                if (statisticslog[key].ToString() != string.Empty)
                                    StatisticsSetting.GetInstance().logdatePattern = statisticslog[key].ToString();
                            }
                            else if (string.Compare(key, "log.filter-level", true) == 0)
                            {
                                if (statisticslog[key].ToString() != string.Empty)
                                    StatisticsSetting.GetInstance().logFilterLevel = statisticslog[key].ToString();
                            }
                            else if (string.Compare(key, "log.file-name", true) == 0)
                            {
                                if (statisticslog[key].ToString() != string.Empty)
                                    StatisticsSetting.GetInstance().logFileName = statisticslog[key].ToString();
                            }
                            else if (string.Compare(key, "log.levels-to-filter", true) == 0)
                            {
                                StatisticsSetting.GetInstance().logLevel = statisticslog[key].ToString();
                            }
                            else if (string.Compare(key, "log.max-file-size", true) == 0)
                            {
                                if (statisticslog[key].ToString() != string.Empty)
                                    StatisticsSetting.GetInstance().logmaxFileSize = statisticslog[key].ToString();
                            }
                            else if (string.Compare(key, "log.max-roll-size", true) == 0)
                            {
                                if (statisticslog[key].ToString() != string.Empty)
                                    StatisticsSetting.GetInstance().logmaxSizeRoll = statisticslog[key].ToString();
                            }
                            else if (string.Compare(key, "log.roll-style", true) == 0)
                            {
                                if (statisticslog[key].ToString() != string.Empty)
                                    StatisticsSetting.GetInstance().rollingStyle = statisticslog[key].ToString();
                            }
                        }

                        #endregion
                    }

                    #region Section enable-disable-channels

                    KeyValueCollection enableDisableDetails = (KeyValueCollection)Application.Options["enable-disable-channels"];

                    if (enableDisableDetails != null)
                    {
                        foreach (string key in enableDisableDetails.AllKeys)
                        {
                            StatisticsCollection statisticsCollection = new StatisticsCollection();
                            if (string.Compare(key, "statistics.enable-log", true) == 0)
                            {
                                if (enableDisableDetails[key].ToString() != string.Empty)
                                    StatisticsSetting.GetInstance().isLogEnabled = Convert.ToBoolean(enableDisableDetails[key]);
                                else
                                {
                                    StatisticsSetting.GetInstance().isLogEnabled = false;
                                    StatisticsBase.GetInstance().ListMissedValues.Add(key);
                                }
                            }
                        }
                    }

                    #endregion
                }
                else
                {
                    logger.Warn("ReadApplication : ReadLoggerDetails : Application Object is Null");
                }
            }
            catch (Exception generalException)
            {
                logger.Error("ReadApplication : ReadLoggerDetails Method: " + generalException.Message.ToString());
            }
            finally
            {
                logger.Debug("ReadApplication : ReadLoggerDetails : Method Exit");
                GC.Collect();
            }
            return StatisticsSetting.GetInstance().isLogEnabled;
        }

        /// <summary>
        /// Saves the missed values.
        /// </summary>
        public bool SaveMissedValues()
        {
            try
            {
                if ((StatisticsBase.GetInstance().StatSource == StatisticsEnum.StatSource.StatServer.ToString()) || (StatisticsBase.GetInstance().StatSource == StatisticsEnum.StatSource.All.ToString() && StatisticsBase.GetInstance().isCMEAuthentication))
                {
                    if ((StatisticsSetting.GetInstance().statisticsCollection.StatisticsLocalSetting.PrimaryStatServer == null) && (StatisticsSetting.GetInstance().statisticsCollection.StatisticsLocalSetting.PrimaryStatServerUri == null))
                    {
                        StatisticsBase.GetInstance().ListMissedValues.Add("statistics.primary-server-name");
                        StatisticsBase.GetInstance().ListMissedValues.Add("statistics.primary-server-uri");
                    }

                    //if (StatisticsSetting.GetInstance().statisticsCollection.StatisticsLocalSetting.PrimaryStatServerUri == null)
                    //    StatisticsBase.GetInstance().ListMissedValues.Add("statistics.primary-server-uri");

                    #region Error Messages

                    if (StatisticsSetting.GetInstance().statisticsCollection.ApplicationContainer.ConfigConnection == null)
                        StatisticsBase.GetInstance().ListMissedValues.Add("config.connection");

                    if (StatisticsSetting.GetInstance().statisticsCollection.ApplicationContainer.PlaceNotFound == null)
                        StatisticsBase.GetInstance().ListMissedValues.Add("place.notfound");

                    if (StatisticsSetting.GetInstance().statisticsCollection.ApplicationContainer.PrimaryServer == null)
                        StatisticsBase.GetInstance().ListMissedValues.Add("pri.server");

                    if (StatisticsSetting.GetInstance().statisticsCollection.ApplicationContainer.SecondaryServer == null)
                        StatisticsBase.GetInstance().ListMissedValues.Add("sec.server");

                    if (StatisticsSetting.GetInstance().statisticsCollection.ApplicationContainer.UserAuthorization == null)
                        StatisticsBase.GetInstance().ListMissedValues.Add("server.down");

                    if (StatisticsSetting.GetInstance().statisticsCollection.ApplicationContainer.UserPermission == null)
                        StatisticsBase.GetInstance().ListMissedValues.Add("nostat.configured");

                    if (StatisticsSetting.GetInstance().statisticsCollection.ApplicationContainer.UserPermission == null)
                        StatisticsBase.GetInstance().ListMissedValues.Add("user.authorization");

                    if (StatisticsSetting.GetInstance().statisticsCollection.ApplicationContainer.UserPermission == null)
                        StatisticsBase.GetInstance().ListMissedValues.Add("user.permission");

                    if (!StatisticsSetting.GetInstance().DictErrorValues.ContainsKey("nostat.configured"))
                        StatisticsBase.GetInstance().ListMissedValues.Add("nostat.configured");

                    #endregion

                    #region Statistics Log

                    if (!StatisticsBase.GetInstance().isPlugin)
                    {
                        if (StatisticsSetting.GetInstance().logconversionPattern == string.Empty)
                            StatisticsBase.GetInstance().ListMissedValues.Add("log.conversion-pattern");

                        if (StatisticsSetting.GetInstance().logdatePattern == string.Empty)
                            StatisticsBase.GetInstance().ListMissedValues.Add("log.date-pattern");

                        if (StatisticsSetting.GetInstance().logFileName == string.Empty)
                            StatisticsBase.GetInstance().ListMissedValues.Add("log.file-name");

                        if (StatisticsSetting.GetInstance().logFilterLevel == string.Empty)
                            StatisticsBase.GetInstance().ListMissedValues.Add("log.filter-level");

                        if (StatisticsSetting.GetInstance().logmaxFileSize == string.Empty)
                            StatisticsBase.GetInstance().ListMissedValues.Add("log.max-file-size");

                        if (StatisticsSetting.GetInstance().logmaxSizeRoll.ToString() == string.Empty)
                            StatisticsBase.GetInstance().ListMissedValues.Add("log.max-roll-size");

                        if (StatisticsSetting.GetInstance().rollingStyle == string.Empty)
                            StatisticsBase.GetInstance().ListMissedValues.Add("log.roll-style");
                    }

                    #endregion

                    #region System Values

                    //if ((StatisticsSetting.GetInstance().statisticsCollection.ApplicationContainer.AdminGroupName == null) && (!StatisticsBase.GetInstance().isPlugin))
                    //    StatisticsBase.GetInstance().ListMissedValues.Add("admin.access-group");

                    if (StatisticsSetting.GetInstance().statisticsCollection.ApplicationContainer.UserGroupName == null)
                        StatisticsBase.GetInstance().ListMissedValues.Add("user.access-group");

                    if (StatisticsSetting.GetInstance().statisticsCollection.ApplicationContainer.DisplayTime == null)
                        StatisticsBase.GetInstance().ListMissedValues.Add("statistics.display-time");

                    if (StatisticsSetting.GetInstance().statisticsCollection.ApplicationContainer.StatBold == null)
                        StatisticsBase.GetInstance().ListMissedValues.Add("statistics.bold");

                    #endregion

                    #region EnableDisableChannels

                    if (!StatisticsSetting.GetInstance().DictEnableDisableChannels.ContainsKey("AlwaysOnTop") && !StatisticsBase.GetInstance().ListMissedValues.Contains("statistics.enable-alwaysontop"))
                        StatisticsBase.GetInstance().ListMissedValues.Add("statistics.enable-alwaysontop");

                    if (!StatisticsSetting.GetInstance().DictEnableDisableChannels.ContainsKey("log") && !StatisticsBase.GetInstance().ListMissedValues.Contains("statistics.enable-log"))
                        StatisticsBase.GetInstance().ListMissedValues.Add("statistics.enable-log");

                    if (!StatisticsSetting.GetInstance().DictEnableDisableChannels.ContainsKey("maingadget") && !StatisticsBase.GetInstance().ListMissedValues.Contains("statistics.enable-maingadget"))
                        StatisticsBase.GetInstance().ListMissedValues.Add("statistics.enable-maingadget");

                    if (!StatisticsSetting.GetInstance().DictEnableDisableChannels.ContainsKey("menubutton") && !StatisticsBase.GetInstance().ListMissedValues.Contains("statistics.enable-menu-button"))
                        StatisticsBase.GetInstance().ListMissedValues.Add("statistics.enable-menu-button");

                    //if (!StatisticsSetting.GetInstance().DictEnableDisableChannels.ContainsKey("notificationclose") && !StatisticsBase.GetInstance().ListMissedValues.Contains("statistics.enable-notification-close") && (!StatisticsBase.GetInstance().isPlugin))
                    //    StatisticsBase.GetInstance().ListMissedValues.Add("statistics.enable-notification-close");

                    if (!StatisticsSetting.GetInstance().DictEnableDisableChannels.ContainsKey("notificationballoon") && !StatisticsBase.GetInstance().ListMissedValues.Contains("statistics.enable-notification-balloon"))
                        StatisticsBase.GetInstance().ListMissedValues.Add("statistics.enable-notification-balloon");

                    if (!StatisticsSetting.GetInstance().DictEnableDisableChannels.ContainsKey("ccstatistics") && !StatisticsBase.GetInstance().ListMissedValues.Contains("statistics.enable-submenu-ccstatistics"))
                        StatisticsBase.GetInstance().ListMissedValues.Add("statistics.enable-submenu-ccstatistics");

                    //if (!StatisticsSetting.GetInstance().DictEnableDisableChannels.ContainsKey("closegadget") && !StatisticsBase.GetInstance().ListMissedValues.Contains("statistics.enable-submenu-close-gadget"))
                    //    StatisticsBase.GetInstance().ListMissedValues.Add("statistics.enable-submenu-close-gadget");

                    if (!StatisticsSetting.GetInstance().DictEnableDisableChannels.ContainsKey("mystatistics") && !StatisticsBase.GetInstance().ListMissedValues.Contains("statistics.enable-submenu-mystatistics"))
                        StatisticsBase.GetInstance().ListMissedValues.Add("statistics.enable-submenu-mystatistics");

                    if (!StatisticsSetting.GetInstance().DictEnableDisableChannels.ContainsKey("ontop") && !StatisticsBase.GetInstance().ListMissedValues.Contains("statistics.enable-submenu-ontop"))
                        StatisticsBase.GetInstance().ListMissedValues.Add("statistics.enable-submenu-ontop");

                    if (!StatisticsSetting.GetInstance().DictEnableDisableChannels.ContainsKey("tagbutton") && !StatisticsBase.GetInstance().ListMissedValues.Contains("statistics.enable-tag-button"))
                        StatisticsBase.GetInstance().ListMissedValues.Add("statistics.enable-tag-button");

                    if (!StatisticsSetting.GetInstance().DictEnableDisableChannels.ContainsKey("tagvertical") && !StatisticsBase.GetInstance().ListMissedValues.Contains("statistics.enable-tag-vertical"))
                        StatisticsBase.GetInstance().ListMissedValues.Add("statistics.enable-tag-vertical");

                    if (!StatisticsSetting.GetInstance().DictEnableDisableChannels.ContainsKey("untagbutton") && !StatisticsBase.GetInstance().ListMissedValues.Contains("statistics.enable-untag-button"))
                        StatisticsBase.GetInstance().ListMissedValues.Add("statistics.enable-untag-button");

                    if (!StatisticsSetting.GetInstance().DictEnableDisableChannels.ContainsKey("hhmmssformat") && !StatisticsBase.GetInstance().ListMissedValues.Contains("statistics.enable-hhmmssformat"))
                        StatisticsBase.GetInstance().ListMissedValues.Add("statistics.enable-hhmmss-format");

                    if (!StatisticsSetting.GetInstance().DictEnableDisableChannels.ContainsKey("thresholdnotification") && !StatisticsBase.GetInstance().ListMissedValues.Contains("statistics.enable-submenu-alert-notification"))
                        StatisticsBase.GetInstance().ListMissedValues.Add("statistics.enable-submenu-alert-notification");

                    if (StatisticsBase.GetInstance().IsSystemDraggable == null && !StatisticsBase.GetInstance().ListMissedValues.Contains("statistics.enable-system-draggable"))
                        StatisticsBase.GetInstance().ListMissedValues.Add("statistics.enable-system-draggable");

                    if (!StatisticsSetting.GetInstance().DictEnableDisableChannels.ContainsKey("showheader") && !StatisticsBase.GetInstance().ListMissedValues.Contains("statistics.enable-submenu-showheader"))
                        StatisticsBase.GetInstance().ListMissedValues.Add("statistics.enable-submenu-showheader");

                    if (!StatisticsSetting.GetInstance().DictEnableDisableChannels.ContainsKey("notifyprimaryscreen") && !StatisticsBase.GetInstance().ListMissedValues.Contains("statistics.enable-notify-primary-screen"))
                        StatisticsBase.GetInstance().ListMissedValues.Add("statistics.enable-notify-primary-screen");

                    #endregion
                }
                else if ((StatisticsBase.GetInstance().StatSource == StatisticsEnum.StatSource.DB.ToString()) || (StatisticsBase.GetInstance().StatSource == StatisticsEnum.StatSource.All.ToString() && !StatisticsBase.GetInstance().isCMEAuthentication))
                {
                    #region Error Messages

                    if (StatisticsSetting.GetInstance().statisticsCollection.ApplicationContainer.dbAuthentication == null)
                        StatisticsBase.GetInstance().ListMissedValues.Add("db.authentication");

                    if (StatisticsSetting.GetInstance().statisticsCollection.ApplicationContainer.dbConnection == null)
                        StatisticsBase.GetInstance().ListMissedValues.Add("db.connection");

                    if (StatisticsSetting.GetInstance().statisticsCollection.ApplicationContainer.NoStats == null)
                        StatisticsBase.GetInstance().ListMissedValues.Add("nostat.configured");

                    #endregion

                    #region Statistics Log

                    if (StatisticsSetting.GetInstance().logconversionPattern == string.Empty)
                        StatisticsBase.GetInstance().ListMissedValues.Add("statistics.log.conversion-pattern");

                    if (StatisticsSetting.GetInstance().logdatePattern == string.Empty)
                        StatisticsBase.GetInstance().ListMissedValues.Add("statistics.log.date-pattern");

                    if (StatisticsSetting.GetInstance().logFileName == string.Empty)
                        StatisticsBase.GetInstance().ListMissedValues.Add("statistics.log.level");

                    if (StatisticsSetting.GetInstance().logFilterLevel == string.Empty)
                        StatisticsBase.GetInstance().ListMissedValues.Add("statistics.log.file-name");

                    if (StatisticsSetting.GetInstance().logLevel == string.Empty)
                        StatisticsBase.GetInstance().ListMissedValues.Add("statistics.log.filter-level");

                    if (StatisticsSetting.GetInstance().logmaxFileSize == string.Empty)
                        StatisticsBase.GetInstance().ListMissedValues.Add("statistics.log.max-file-size");

                    if (StatisticsSetting.GetInstance().logMaxLevel == string.Empty)
                        StatisticsBase.GetInstance().ListMissedValues.Add("statistics.log.max-roll-size");

                    if (StatisticsSetting.GetInstance().logmaxSizeRoll.ToString() == string.Empty)
                        StatisticsBase.GetInstance().ListMissedValues.Add("statistics.log.min-level");

                    if (StatisticsSetting.GetInstance().logMinLevel == string.Empty)
                        StatisticsBase.GetInstance().ListMissedValues.Add("statistics.log.max-level");

                    if (StatisticsSetting.GetInstance().rollingStyle == string.Empty)
                        StatisticsBase.GetInstance().ListMissedValues.Add("statistics.log.roll-style");

                    #endregion

                    #region Enable Disable Channels

                    if (!StatisticsSetting.GetInstance().DictEnableDisableChannels.ContainsKey("AlwaysOnTop") && !StatisticsBase.GetInstance().ListMissedValues.Contains("statistics.enable-alwaysontop"))
                        StatisticsBase.GetInstance().ListMissedValues.Add("statistics.enable-alwaysontop");

                    if (!StatisticsSetting.GetInstance().DictEnableDisableChannels.ContainsKey("hhmmssformat") && !StatisticsBase.GetInstance().ListMissedValues.Contains("statistics.enable-hhmmssformat"))
                        StatisticsBase.GetInstance().ListMissedValues.Add("statistics.enable-hhmmssformat");

                    if (!StatisticsSetting.GetInstance().DictEnableDisableChannels.ContainsKey("log") && !StatisticsBase.GetInstance().ListMissedValues.Contains("statistics.enable-log"))
                        StatisticsBase.GetInstance().ListMissedValues.Add("statistics.enable-log");

                    if (!StatisticsSetting.GetInstance().DictEnableDisableChannels.ContainsKey("maingadget") && !StatisticsBase.GetInstance().ListMissedValues.Contains("statistics.enable-maingadget"))
                        StatisticsBase.GetInstance().ListMissedValues.Add("statistics.enable-maingadget");

                    if (!StatisticsSetting.GetInstance().DictEnableDisableChannels.ContainsKey("menubutton") && !StatisticsBase.GetInstance().ListMissedValues.Contains("statistics.enable-menu-button"))
                        StatisticsBase.GetInstance().ListMissedValues.Add("statistics.enable-menu-button");

                    if (!StatisticsSetting.GetInstance().DictEnableDisableChannels.ContainsKey("notificationballoon") && !StatisticsBase.GetInstance().ListMissedValues.Contains("statistics.enable-notification-balloon"))
                        StatisticsBase.GetInstance().ListMissedValues.Add("statistics.enable-notification-balloon");

                    if (!StatisticsSetting.GetInstance().DictEnableDisableChannels.ContainsKey("notificationclose") && !StatisticsBase.GetInstance().ListMissedValues.Contains("statistics.enable-notification-close"))
                        StatisticsBase.GetInstance().ListMissedValues.Add("statistics.enable-notification-close");

                    if (!StatisticsSetting.GetInstance().DictEnableDisableChannels.ContainsKey("closegadget") && !StatisticsBase.GetInstance().ListMissedValues.Contains("statistics.enable-submenu-close-gadget"))
                        StatisticsBase.GetInstance().ListMissedValues.Add("statistics.enable-submenu-close-gadget");

                    if (!StatisticsSetting.GetInstance().DictEnableDisableChannels.ContainsKey("ontop") && !StatisticsBase.GetInstance().ListMissedValues.Contains("statistics.enable-submenu-ontop"))
                        StatisticsBase.GetInstance().ListMissedValues.Add("statistics.enable-submenu-ontop");

                    if (!StatisticsSetting.GetInstance().DictEnableDisableChannels.ContainsKey("systemdraggable") && !StatisticsBase.GetInstance().ListMissedValues.Contains("statistics.enable-systemdraggable"))
                        StatisticsBase.GetInstance().ListMissedValues.Add("statistics.enable-systemdraggable");

                    if (!StatisticsSetting.GetInstance().DictEnableDisableChannels.ContainsKey("tagbutton") && !StatisticsBase.GetInstance().ListMissedValues.Contains("statistics.enable-tag-button"))
                        StatisticsBase.GetInstance().ListMissedValues.Add("statistics.enable-tag-button");

                    if (!StatisticsSetting.GetInstance().DictEnableDisableChannels.ContainsKey("tagvertical") && !StatisticsBase.GetInstance().ListMissedValues.Contains("statistics.enable-tag-vertical"))
                        StatisticsBase.GetInstance().ListMissedValues.Add("statistics.enable-tag-vertical");

                    if (!StatisticsSetting.GetInstance().DictEnableDisableChannels.ContainsKey("untagbutton") && !StatisticsBase.GetInstance().ListMissedValues.Contains("statistics.enable-untag-button"))
                        StatisticsBase.GetInstance().ListMissedValues.Add("statistics.enable-untag-button");

                    if (!StatisticsSetting.GetInstance().DictEnableDisableChannels.ContainsKey("thresholdnotification") && !StatisticsBase.GetInstance().ListMissedValues.Contains("statistics.enable-submenu-alertnotification"))
                        StatisticsBase.GetInstance().ListMissedValues.Add("statistics.enable-submenu-alertnotification");

                    if (!StatisticsSetting.GetInstance().DictEnableDisableChannels.ContainsKey("showheader") && !StatisticsBase.GetInstance().ListMissedValues.Contains("statistics.enable-submenu-showheader"))
                        StatisticsBase.GetInstance().ListMissedValues.Add("statistics.enable-submenu-showheader");

                    if (!StatisticsSetting.GetInstance().DictEnableDisableChannels.ContainsKey("notifyprimaryscreen") && !StatisticsBase.GetInstance().ListMissedValues.Contains("statistics.enable-notify-primary-screen"))
                        StatisticsBase.GetInstance().ListMissedValues.Add("statistics.enable-notify-primary-screen");

                    #endregion
                }

                if (StatisticsBase.GetInstance().ListMissedValues.Count != 0)
                    return true;
                else
                    return false;
            }
            catch (Exception GeneralException)
            {
                return false;
            }
        }

        #endregion Methods
    }
}