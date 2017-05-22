using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using log4net;
using Pointel.Statistics.Core.Utility;
using Pointel.Statistics.Core.ConnectionManager;
using System.Collections;
using System.Text.RegularExpressions;
using Genesyslab.Platform.ApplicationBlocks.Commons.Broker;
using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.CfgObjects;
using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.Queries;
using Genesyslab.Platform.Commons.Collections;
using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel;
using Genesyslab.Platform.Commons.Protocols;
using Genesyslab.Platform.Configuration.Protocols.ConfServer.Events;
using Genesyslab.Platform.Configuration.Protocols.Types;
using Genesyslab.Platform.Configuration.Protocols.ConfServer.Requests.Objects;

namespace Pointel.Statistics.Core.Application
{
    public class StatisticsInformation
    {
        #region Field Declaration

        private static Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "STF");
        EventBrokerService eventBrokerService;

        #endregion

        /// <summary>
        /// Reads the application annex.
        /// </summary>
        public void ReadApplicationAnnex()
        {
            logger.Debug("StatisticsInformation : ReadApplicationAnnex Method: Entry");
            try
            {
                StatisticsSetting.GetInstance().StatistcisDetails = new Dictionary<string, Dictionary<string, string>>();

                string[] sections = StatisticsSetting.GetInstance().Application.UserProperties.AllKeys;

                foreach (string sectionName in sections)
                {
                    KeyValueCollection tempCollection = new KeyValueCollection();
                    tempCollection = (KeyValueCollection)StatisticsSetting.GetInstance().Application.UserProperties[sectionName];

                    Dictionary<string, string> tempStatProperties = new Dictionary<string, string>();

                    foreach (string key in tempCollection.Keys)
                    {
                        #region Read Application Annex values

                        logger.Debug("-------------------------------------------------------------------");
                        logger.Debug("ReadApplication  : ReadApplicationAnnex : " + sectionName.ToString());
                        logger.Debug("-------------------------------------------------------------------");

                        if (string.Compare(key, "Color1", true) == 0)
                        {
                            tempStatProperties.Add(key, tempCollection[key].ToString());
                            logger.Debug("ReadApplication  : ReadApplicationAnnex : " + tempCollection[key].ToString());
                        }
                        else if (string.Compare(key, "Color2", true) == 0)
                        {
                            tempStatProperties.Add(key, tempCollection[key].ToString());
                            logger.Debug("ReadApplication  : ReadApplicationAnnex : " + tempCollection[key].ToString());
                        }
                        else if (string.Compare(key, "Color3", true) == 0)
                        {
                            tempStatProperties.Add(key, tempCollection[key].ToString());
                            logger.Debug("ReadApplication  : ReadApplicationAnnex : " + tempCollection[key].ToString());
                        }
                        else if (string.Compare(key, "DisplayName", true) == 0)
                        {
                            tempStatProperties.Add(key, tempCollection[key].ToString());
                            logger.Debug("ReadApplication  : ReadApplicationAnnex : " + tempCollection[key].ToString());
                        }
                        else if (string.Compare(key, "Filter", true) == 0)
                        {
                            tempStatProperties.Add(key, tempCollection[key].ToString());
                            logger.Debug("ReadApplication  : ReadApplicationAnnex : " + tempCollection[key].ToString());
                        }
                        else if (string.Compare(key, "Format", true) == 0)
                        {
                            tempStatProperties.Add(key, tempCollection[key].ToString());
                            logger.Debug("ReadApplication  : ReadApplicationAnnex : " + tempCollection[key].ToString());
                        }
                        else if (string.Compare(key, "StatName", true) == 0)
                        {
                            tempStatProperties.Add(key, tempCollection[key].ToString());
                            logger.Debug("ReadApplication  : ReadApplicationAnnex : " + tempCollection[key].ToString());
                        }
                        else if (string.Compare(key, "stype", true) == 0)
                        {
                            tempStatProperties.Add(key, tempCollection[key].ToString());
                            logger.Debug("ReadApplication  : ReadApplicationAnnex : " + tempCollection[key].ToString());
                        }
                        else if (string.Compare(key, "ThresLevel1", true) == 0)
                        {
                            tempStatProperties.Add(key, tempCollection[key].ToString());
                            logger.Debug("ReadApplication  : ReadApplicationAnnex : " + tempCollection[key].ToString());
                        }
                        else if (string.Compare(key, "ThresLevel2", true) == 0)
                        {
                            tempStatProperties.Add(key, tempCollection[key].ToString());
                            logger.Debug("ReadApplication  : ReadApplicationAnnex : " + tempCollection[key].ToString());
                        }
                        else if (string.Compare(key, "TooltipName", true) == 0)
                        {
                            tempStatProperties.Add(key, tempCollection[key].ToString());
                            logger.Debug("ReadApplication  : ReadApplicationAnnex : " + tempCollection[key].ToString());
                        }

                        #endregion
                    }

                    StatisticsSetting.GetInstance().StatistcisDetails.Add(sectionName, tempStatProperties);
                }
            }
            catch (Exception GeneralException)
            {

                logger.Error("StatisticsInformation : ReadApplicationAnnex Method : " + GeneralException.Message);
            }

            logger.Debug("StatisticsInformation : ReadApplicationAnnex Method: Exit");
        }

        /// <summary>
        /// Reads the server filters.
        /// </summary>
        public void ReadServerFilters()
        {
            logger.Debug("StatisticsInformation : ReadServerFilters Method: Entry");
            try
            {
                StatisticsSetting.GetInstance().ServerFilters = new Dictionary<string, string>();

                KeyValueCollection tempValues = new KeyValueCollection();
                tempValues = (KeyValueCollection)StatisticsSetting.GetInstance().statisticsCollection.StatisticsLocalSetting.PrimaryStatServer[0].Options["Filters"];

                foreach (string Filter in tempValues.Keys)
                {
                    if (tempValues[Filter] != null)
                    {
                        StatisticsSetting.GetInstance().ServerFilters.Add(Filter, tempValues[Filter].ToString());
                    }
                }
            }
            catch (Exception GeneralException)
            {
                logger.Error("StatisticsInformation : ReadServerFilters Method: " + GeneralException.Message);
            }
            logger.Debug("StatisticsInformation : ReadServerFilters Method: Exit");
        }

        /// <summary>
        /// Reads the server statistcis.
        /// </summary>
        public void ReadServerStatistcis()
        {
            logger.Debug("StatisticsInformation : ReadServerStatistcis Method: Entry");
            try
            {
                foreach (string key in StatisticsSetting.GetInstance().statisticsCollection.StatisticsLocalSetting.PrimaryStatServer[0].Options.Keys)
                {
                    StatisticsSetting.GetInstance().ServerValues.Add(key, StatisticsSetting.GetInstance().statisticsCollection.StatisticsLocalSetting.PrimaryStatServer[0].Options[key]);
                }

                foreach (DictionaryEntry keypairs in StatisticsSetting.GetInstance().ServerValues)
                {
                    KeyValueCollection kvpcollection = new KeyValueCollection();
                    kvpcollection = keypairs.Value as KeyValueCollection;

                    if (kvpcollection.ContainsKey("Objects"))
                    {
                        if (!StatisticsSetting.GetInstance().ServerStatistics.ContainsKey(keypairs.Key.ToString()))
                        {
                            StatisticsSetting.GetInstance().ServerStatistics.Add(keypairs.Key.ToString(), kvpcollection);
                        }
                    }
                }
            }
            catch (Exception GeneralException)
            {
                logger.Error("StatisticsInformation : ReadServerStatistcis Method: " + GeneralException.Message);
            }
            logger.Debug("StatisticsInformation : ReadServerStatistcis Method: Exit");
        }

        /// <summary>
        /// Reads the existing values.
        /// </summary>
        public void ReadExistingValues()
        { 
            List<string> tempACDList = new List<string>();
            List<string> tempDNGroupList = new List<string>();
            List<string> tempVQList = new List<string>();

            try
            {
                logger.Debug("StatisticsInformation : ReadExistingValues Method: Entry");

                if (StatisticsSetting.GetInstance().Application != null)
                {
                   
                    StatisticsSetting.GetInstance().DictExistingValues = new Dictionary<string, List<string>>();

                    KeyValueCollection tempCollection = new KeyValueCollection();

                    CfgApplication application = new CfgApplication(StatisticsSetting.GetInstance().confObject);
                    CfgApplicationQuery queryApp = new CfgApplicationQuery();
                    queryApp.Name = StatisticsSetting.GetInstance().Application.Name;
                    application = StatisticsSetting.GetInstance().confObject.RetrieveObject<CfgApplication>(queryApp);

                    tempCollection = (KeyValueCollection)application.Options["agent.ixn.desktop"];

                    foreach (string key in tempCollection.Keys)
                    {
                        if (key.StartsWith("statistics.objects-acd-queues"))
                        {
                            #region ACD Queue Lists

                            string[] acds = tempCollection[key].ToString().Split(',');

                            foreach (string acd in acds)
                            {
                                if (!tempACDList.Contains(acd))
                                    tempACDList.Add(acd);
                            }

                            #endregion
                        }
                        else if (key.StartsWith("statistics.objects-dn-groups"))
                        {
                            #region DNGroup List

                            string[] dns = tempCollection[key].ToString().Split(',');

                            foreach (string dn in dns)
                            {
                                if (!tempDNGroupList.Contains(dn))
                                    tempDNGroupList.Add(dn);
                            }

                            #endregion
                        }
                        else if (key.StartsWith("statistics.objects-virtual-queues"))
                        {
                            #region VQ List

                            string[] vqs = tempCollection[key].ToString().Split(',');

                            foreach (string vq in vqs)
                            {
                                if (!tempVQList.Contains(vq))
                                    tempVQList.Add(vq);
                            }

                            #endregion
                        }
                    }


                    //if (tempACDList.Count != 0)
                    //{
                        StatisticsSetting.GetInstance().DictExistingValues.Add(StatisticsEnum.ObjectType.ACDQueue.ToString(), tempACDList);
                    //}

                    //if (tempDNGroupList.Count != 0)
                    //{
                        StatisticsSetting.GetInstance().DictExistingValues.Add(StatisticsEnum.ObjectType.DNGroup.ToString(), tempDNGroupList);
                    //}

                    //if (tempVQList.Count != 0)
                    //{
                        StatisticsSetting.GetInstance().DictExistingValues.Add(StatisticsEnum.ObjectType.VirtualQueue.ToString(), tempVQList);
                    //}
                }

            }
            catch (Exception GeneralException)
            {
                logger.Error("StatisticsInformation : ReadExistingValues Method: " + GeneralException.Message);
            }
            finally
            {
                tempACDList = null;
                tempDNGroupList = null;
                tempVQList = null;
            }
            logger.Debug("StatisticsInformation : ReadExistingValues Method : Exit");
        }

        public void ReadApplicationObjectValues()
        {
            try
            {
                logger.Debug("StatisticsInformation : ReadApplicationObjectValues Method : Entry");

                List<string> tempList = new List<string>();

                if (StatisticsSetting.GetInstance().Application == null)
                {
                    CfgApplication application = new CfgApplication(StatisticsSetting.GetInstance().confObject);
                    CfgApplicationQuery queryApp = new CfgApplicationQuery();
                    queryApp.Name = StatisticsSetting.GetInstance().AppName;
                    StatisticsSetting.GetInstance().Application = StatisticsSetting.GetInstance().confObject.RetrieveObject<CfgApplication>(queryApp);
                }

                        KeyValueCollection serverDetails = (KeyValueCollection)StatisticsSetting.GetInstance().Application.Options["agent.ixn.desktop"];

                        if (serverDetails != null)
                        {
                            foreach (string key in serverDetails.AllKeys)
                            {
                                #region AgentGroup Statistics Configuration

                                if (key.Contains("agent-group-statistics"))
                                {
                                    string[] stats = serverDetails[key].ToString().Split(',');

                                    foreach (string statistics in stats)
                                    {
                                        tempList.Add(statistics);
                                    }
                                }
                                else if (key.Contains("agent-statistics"))
                                {
                                    string[] stats = serverDetails[key].ToString().Split(',');

                                    foreach (string statistics in stats)
                                    {
                                        tempList.Add(statistics);
                                    }
                                }
                                else if (key.Contains("virtual-queue-statistics"))
                                {
                                    string[] stats = serverDetails[key].ToString().Split(',');

                                    foreach (string statistics in stats)
                                    {
                                        tempList.Add(statistics);
                                    }
                                }
                                else if (key.Contains("acd-queue-statistics"))
                                {
                                    string[] stats = serverDetails[key].ToString().Split(',');

                                    foreach (string statistics in stats)
                                    {
                                        tempList.Add(statistics);
                                    }
                                }
                                else if (key.Contains("dn-group-statistics"))
                                {
                                    string[] stats = serverDetails[key].ToString().Split(',');

                                    foreach (string statistics in stats)
                                    {
                                        tempList.Add(statistics);
                                    }
                                }
                                #endregion
                            }

                           // StatisticsSetting.GetInstance().DictAgentGroupStatistics.Add(agentGroup.GroupInfo.Name, tempList);
                        }
                        else
                        {
                          //  StatisticsSetting.GetInstance().DictAgentGroupStatistics.Add(agentGroup.GroupInfo.Name, new List<string>());
                        }
            }
            catch (Exception GeneralException)
            {
                logger.Error("StatisticsInformation : ReadApplicationObjectValues Method : " + GeneralException.Message);
            }
            logger.Debug("StatisticsInformation : ReadApplicationObjectValues Method : Exit");
        }

        public void ReadAllAgentGroupValues()
        {
            try
            {
                logger.Debug("StatisticsInformation : ReadAgentGroupValues Method : Entry");

                List<string> tempList = new List<string>();
                CfgFolderQuery folderQuery = new CfgFolderQuery();
                folderQuery.Name = "Agent Groups";
                folderQuery.OwnerDbid = StatisticsSetting.GetInstance().CFGTenantDBID;
                CfgFolder folder = StatisticsSetting.GetInstance().confObject.RetrieveObject<CfgFolder>(folderQuery);

                CfgAgentGroupQuery _agentGroupQuery = new CfgAgentGroupQuery();
                _agentGroupQuery.TenantDbid = StatisticsSetting.GetInstance().CFGTenantDBID;
                _agentGroupQuery["folder_dbid"] = folder.DBID;
                ICollection<CfgAgentGroup> _agentGroupDetails = StatisticsSetting.GetInstance().confObject.RetrieveMultipleObjects<CfgAgentGroup>(_agentGroupQuery);

                if (_agentGroupDetails != null)
                {
                    foreach (CfgAgentGroup agentGroup in _agentGroupDetails)
                    {
                        tempList = new List<string>();

                        List<string> tempACDList = new List<string>();
                        List<string> tempDNList = new List<string>();
                        List<string> tempVQList = new List<string>();
                        Dictionary<string, List<string>> dictTempObjects = new Dictionary<string, List<string>>();

                        Dictionary<string, List<string>> dictTemp = new Dictionary<string, List<string>>();
                        KeyValueCollection serverDetails = (KeyValueCollection)agentGroup.GroupInfo.UserProperties["agent.ixn.desktop"];

                        if (serverDetails != null)
                        {
                            foreach (string key in serverDetails.AllKeys)
                            {
                                #region AgentGroup Statistics Configuration

                                if (key.Contains("agent-group-statistics"))
                                {
                                    if (!string.IsNullOrEmpty(serverDetails[key].ToString()))
                                    {
                                        string[] stats = serverDetails[key].ToString().Split(',');

                                        foreach (string statistics in stats)
                                        {
                                            tempList.Add(statistics);
                                        }
                                    }
                                }
                                else if (key.Contains("agent-statistics"))
                                {
                                    if (!string.IsNullOrEmpty(serverDetails[key].ToString()))
                                    {
                                        string[] stats = serverDetails[key].ToString().Split(',');

                                        foreach (string statistics in stats)
                                        {
                                            tempList.Add(statistics);
                                        }
                                    }
                                }
                                else if (key.Contains("virtual-queue-statistics"))
                                {
                                    if (!string.IsNullOrEmpty(serverDetails[key].ToString()))
                                    {
                                        string[] stats = serverDetails[key].ToString().Split(',');

                                        foreach (string statistics in stats)
                                        {
                                            tempList.Add(statistics);
                                        }
                                    }
                                }
                                else if (key.Contains("acd-queue-statistics"))
                                {
                                    if (!string.IsNullOrEmpty(serverDetails[key].ToString()))
                                    {
                                        string[] stats = serverDetails[key].ToString().Split(',');

                                        foreach (string statistics in stats)
                                        {
                                            tempList.Add(statistics);
                                        }
                                    }
                                }
                                else if (key.Contains("dn-group-statistics"))
                                {
                                    if (!string.IsNullOrEmpty(serverDetails[key].ToString()))
                                    {
                                        string[] stats = serverDetails[key].ToString().Split(',');

                                        foreach (string statistics in stats)
                                        {
                                            tempList.Add(statistics);
                                        }
                                    }
                                }

                                else if (key.StartsWith("statistics.objects-acd-queues"))
                                {
                                    if (!string.IsNullOrEmpty(serverDetails[key].ToString()))
                                    {
                                        string[] objects = serverDetails[key].ToString().Split(',');

                                        foreach (string obj in objects)
                                        {
                                            if (!tempACDList.Contains(obj))
                                                tempACDList.Add(obj);
                                        }
                                    }
                                }
                                else if (key.StartsWith("statistics.objects-dn-groups"))
                                {
                                    if (!string.IsNullOrEmpty(serverDetails[key].ToString()))
                                    {
                                        string[] objects = serverDetails[key].ToString().Split(',');

                                        foreach (string obj in objects)
                                        {
                                            if (!tempDNList.Contains(obj))
                                                tempDNList.Add(obj);
                                        }
                                    }
                                }
                                else if (key.StartsWith("statistics.objects-virtual-queues"))
                                {
                                    if (!string.IsNullOrEmpty(serverDetails[key].ToString()))
                                    {
                                        string[] objects = serverDetails[key].ToString().Split(',');

                                        foreach (string obj in objects)
                                        {
                                            if (!tempVQList.Contains(obj))
                                                tempVQList.Add(obj);
                                        }
                                    }
                                }

                                #endregion
                            }

                            //if (tempACDList.Count != 0)
                                dictTempObjects.Add(StatisticsEnum.ObjectType.ACDQueue.ToString(), tempACDList);

                            //if (tempDNList.Count != 0)
                                dictTempObjects.Add(StatisticsEnum.ObjectType.DNGroup.ToString(), tempDNList);

                            //if (tempVQList.Count != 0)
                                dictTempObjects.Add(StatisticsEnum.ObjectType.VirtualQueue.ToString(), tempVQList);

                            StatisticsSetting.GetInstance().DictAgentGroupObjects.Add(agentGroup.GroupInfo.Name, dictTempObjects);

                            StatisticsSetting.GetInstance().DictAgentGroupStatistics.Add(agentGroup.GroupInfo.Name, tempList);
                        }
                        else
                        {
                            dictTempObjects.Add(StatisticsEnum.ObjectType.ACDQueue.ToString(), tempACDList);
                            dictTempObjects.Add(StatisticsEnum.ObjectType.DNGroup.ToString(), tempDNList);
                            dictTempObjects.Add(StatisticsEnum.ObjectType.VirtualQueue.ToString(), tempVQList);

                            StatisticsSetting.GetInstance().DictAgentGroupObjects.Add(agentGroup.GroupInfo.Name, dictTempObjects);
                            StatisticsSetting.GetInstance().DictAgentGroupStatistics.Add(agentGroup.GroupInfo.Name, new List<string>());
                        }
                    }
                }
            }
            catch (Exception GeneralException)
            {
                logger.Error("StatisticsInformation : ReadAgentGroupValues Method : "+GeneralException.Message);
            }
            logger.Debug("StatisticsInformation : ReadAgentGroupValues Method : Exit");
        }

        public void ReadAllAgentValues()
        {
            try
            {
                logger.Debug("StatisticsInformation : ReadAllAgentValues Method : Entry");
                
                List<string> tempList = new List<string>();

                CfgPersonQuery _personQuery = new CfgPersonQuery();
                _personQuery.TenantDbid = StatisticsSetting.GetInstance().CFGTenantDBID;

                ICollection<CfgPerson> _agentCollections = StatisticsSetting.GetInstance().confObject.RetrieveMultipleObjects<CfgPerson>(_personQuery);

                if (_agentCollections != null)
                {
                    foreach (CfgPerson agent in _agentCollections)
                    {
                        tempList = new List<string>();
                        
                        List<string> tempACDList = new List<string>();
                        List<string> tempDNList = new List<string>();
                        List<string> tempVQList = new List<string>();
                        Dictionary<string,List<string>> dictTempObjects=new Dictionary<string,List<string>>();

                        Dictionary<string, List<string>> dictTemp = new Dictionary<string, List<string>>();
                        KeyValueCollection serverDetails = (KeyValueCollection)agent.UserProperties["agent.ixn.desktop"];

                        if (serverDetails != null)
                        {
                            foreach (string key in serverDetails.AllKeys)
                            {
                                #region Agent Statistics Configuration

                                if (key.StartsWith("agent-group-statistics"))
                                {
                                    if (!string.IsNullOrEmpty(serverDetails[key].ToString()))
                                    {
                                        string[] stats = serverDetails[key].ToString().Split(',');

                                        foreach (string statistics in stats)
                                        {
                                            tempList.Add(statistics);
                                        }
                                    }
                                }
                                else if (key.StartsWith("agent-statistics"))
                                {
                                    if (!string.IsNullOrEmpty(serverDetails[key].ToString()))
                                    {

                                        string[] stats = serverDetails[key].ToString().Split(',');

                                        foreach (string statistics in stats)
                                        {
                                            tempList.Add(statistics);
                                        }
                                    }
                                }
                                else if (key.StartsWith("virtual-queue-statistics"))
                                {
                                    if (!string.IsNullOrEmpty(serverDetails[key].ToString()))
                                    {
                                        string[] stats = serverDetails[key].ToString().Split(',');

                                        foreach (string statistics in stats)
                                        {
                                            tempList.Add(statistics);
                                        }
                                    }
                                }
                                else if (key.StartsWith("acd-queue-statistics"))
                                {
                                    if (!string.IsNullOrEmpty(serverDetails[key].ToString()))
                                    {
                                        string[] stats = serverDetails[key].ToString().Split(',');

                                        foreach (string statistics in stats)
                                        {
                                            tempList.Add(statistics);
                                        }
                                    }
                                }
                                else if (key.StartsWith("dn-group-statistics"))
                                {
                                    if (!string.IsNullOrEmpty(serverDetails[key].ToString()))
                                    {
                                        string[] stats = serverDetails[key].ToString().Split(',');

                                        foreach (string statistics in stats)
                                        {
                                            tempList.Add(statistics);
                                        }
                                    }
                                }
                                else if (key.StartsWith("statistics.objects-acd-queues"))
                                {
                                    if (!string.IsNullOrEmpty(serverDetails[key].ToString()))
                                    {
                                        string[] objects = serverDetails[key].ToString().Split(',');

                                        foreach (string obj in objects)
                                        {
                                            if (!tempACDList.Contains(obj))
                                                tempACDList.Add(obj);
                                        }
                                    }
                                }
                                else if (key.StartsWith("statistics.objects-dn-groups"))
                                {
                                    if (!string.IsNullOrEmpty(serverDetails[key].ToString()))
                                    {
                                        string[] objects = serverDetails[key].ToString().Split(',');

                                        foreach (string obj in objects)
                                        {
                                            if (!tempDNList.Contains(obj))
                                                tempDNList.Add(obj);
                                        }
                                    }
                                }
                                else if (key.StartsWith("statistics.objects-virtual-queues"))
                                {
                                    if (!string.IsNullOrEmpty(serverDetails[key].ToString()))
                                    {
                                        string[] objects = serverDetails[key].ToString().Split(',');

                                        foreach (string obj in objects)
                                        {
                                            if (!tempVQList.Contains(obj))
                                                tempVQList.Add(obj);
                                        }
                                    }
                                }
                                #endregion
                            }

                            //if(tempACDList.Count!=0)
                            dictTempObjects.Add(StatisticsEnum.ObjectType.ACDQueue.ToString(), tempACDList);

                            //if (tempDNList.Count != 0)
                            dictTempObjects.Add(StatisticsEnum.ObjectType.DNGroup.ToString(), tempDNList);

                            //if (tempVQList.Count != 0)
                            dictTempObjects.Add(StatisticsEnum.ObjectType.VirtualQueue.ToString(), tempVQList);

                            if (!StatisticsSetting.GetInstance().DictAgentObjects.ContainsKey(agent.EmployeeID))
                                StatisticsSetting.GetInstance().DictAgentObjects.Add(agent.EmployeeID, dictTempObjects);
                            else
                                StatisticsSetting.GetInstance().DictAgentObjects[agent.EmployeeID] = dictTempObjects;

                            if (!StatisticsSetting.GetInstance().DictAgentNames.ContainsKey(agent.EmployeeID))
                                StatisticsSetting.GetInstance().DictAgentNames.Add(agent.EmployeeID, agent.FirstName + "," + agent.LastName);
                            else
                                StatisticsSetting.GetInstance().DictAgentNames[agent.EmployeeID] = agent.FirstName + "," + agent.LastName;


                            if (!StatisticsSetting.GetInstance().DictAgentStatistics.ContainsKey(agent.EmployeeID))
                                StatisticsSetting.GetInstance().DictAgentStatistics.Add(agent.EmployeeID, tempList);
                            else
                                StatisticsSetting.GetInstance().DictAgentStatistics[agent.EmployeeID] = tempList;
                            
                        }
                        else
                        {
                            dictTempObjects.Add(StatisticsEnum.ObjectType.ACDQueue.ToString(), tempACDList);

                            dictTempObjects.Add(StatisticsEnum.ObjectType.DNGroup.ToString(), tempDNList);

                            dictTempObjects.Add(StatisticsEnum.ObjectType.VirtualQueue.ToString(), tempVQList);

                            if (!StatisticsSetting.GetInstance().DictAgentObjects.ContainsKey(agent.EmployeeID))
                                StatisticsSetting.GetInstance().DictAgentObjects.Add(agent.EmployeeID, dictTempObjects);
                            else
                                StatisticsSetting.GetInstance().DictAgentObjects[agent.EmployeeID] = dictTempObjects;

                            if (!StatisticsSetting.GetInstance().DictAgentNames.ContainsKey(agent.EmployeeID))
                                StatisticsSetting.GetInstance().DictAgentNames.Add(agent.EmployeeID, agent.FirstName + "," + agent.LastName);
                            else
                                StatisticsSetting.GetInstance().DictAgentNames[agent.EmployeeID] = agent.FirstName + "," + agent.LastName;

                            if (!StatisticsSetting.GetInstance().DictAgentStatistics.ContainsKey(agent.EmployeeID))
                                StatisticsSetting.GetInstance().DictAgentStatistics.Add(agent.EmployeeID, new List<string>());
                            else
                                StatisticsSetting.GetInstance().DictAgentStatistics[agent.EmployeeID] = new List<string>();
                        }
                    }
                }
            }
            catch (Exception GeneralException)
            {
                logger.Error("StatisticsInformation : ReadAllAgentValues Method : " + GeneralException.Message);
            }
            logger.Debug("StatisticsInformation : ReadAllAgentValues Method : Exit");
        }

        #region ReadStatsiticValues

        public List<string>  ReadStatisticValues(string objectname, string objectType)
        {
            List<string> ListObjectStatistics = new List<string>();

            try
            {
                logger.Debug("StatisticsInformation : ReadStatisticValues Method : Entry");

                if (objectType == StatisticsEnum.ObjectType.Agent.ToString())
                {
                    CfgPersonQuery _personQuery=new CfgPersonQuery();
                    _personQuery.EmployeeId=objectname;
                    CfgPerson cfgAgent=StatisticsSetting.GetInstance().confObject.RetrieveObject<CfgPerson>(_personQuery);

                    CfgFolderQuery folderQuery = new CfgFolderQuery();
                    folderQuery.Name = "Agent Groups";
                    folderQuery.OwnerDbid = StatisticsSetting.GetInstance().CFGTenantDBID; 
                    CfgFolder folder = StatisticsSetting.GetInstance().confObject.RetrieveObject<CfgFolder>(folderQuery);

                    if (cfgAgent != null)
                    {
                        CfgAgentGroupQuery _agentGroupQuery = new CfgAgentGroupQuery();
                        _agentGroupQuery.PersonDbid = Convert.ToInt16(cfgAgent.DBID);
                        _agentGroupQuery.TenantDbid = StatisticsSetting.GetInstance().CFGTenantDBID;
                        _agentGroupQuery["folder_dbid"] = folder.DBID;
                        ICollection<CfgAgentGroup> _agentGroupDetails = StatisticsSetting.GetInstance().confObject.RetrieveMultipleObjects<CfgAgentGroup>(_agentGroupQuery);

                        foreach (CfgAgentGroup agentGroup in _agentGroupDetails)
                        {
                                StatisticsSetting.GetInstance().AgentGroupsListCollections.Add(agentGroup.GroupInfo.Name.ToString());

                                KeyValueCollection serverDetails = (KeyValueCollection)agentGroup.GroupInfo.UserProperties["agent.ixn.desktop"];

                                if (serverDetails != null)
                                {
                                    foreach (string key in serverDetails.AllKeys)
                                    {
                                        if (key == "agent-group-statistics" || key == "agent-statistics" ||
                                            key == "virtual-queue-statistics" || key == "acd-queue-statistics" ||
                                            key == "dn-group-statistics")
                                        {
                                            ListObjectStatistics.Clear();
                                        }
                                    }

                                    foreach (string key in serverDetails.AllKeys)
                                    {
                                        string[] stats;

                                        #region AgentGroup Statistics Value

                                        if (key.StartsWith("agent-group-statistics"))
                                        {
                                                if (!ListObjectStatistics.Contains(serverDetails[key].ToString()))
                                                {
                                                    stats = serverDetails[key].ToString().Split(',');

                                                    foreach (string values in stats)
                                                    {
                                                        ListObjectStatistics.Add(values);
                                                    }
                                                }
                                         }
                                        else if (key.StartsWith("agent-statistics"))
                                        {
                                            if (!ListObjectStatistics.Contains(serverDetails[key].ToString()))
                                            {
                                                stats = serverDetails[key].ToString().Split(',');

                                                foreach (string values in stats)
                                                {
                                                    ListObjectStatistics.Add(values);
                                                }
                                            }
                                        }
                                        else if (key.StartsWith("virtual-queue-statistics"))
                                        {
                                            if (!ListObjectStatistics.Contains(serverDetails[key].ToString()))
                                            {
                                                stats = serverDetails[key].ToString().Split(',');

                                                foreach (string values in stats)
                                                {
                                                    ListObjectStatistics.Add(values);
                                                }
                                            }
                                        }
                                        else if (key.StartsWith("acd-queue-statistics"))
                                        {
                                            if (!ListObjectStatistics.Contains(serverDetails[key].ToString()))
                                            {
                                                stats = serverDetails[key].ToString().Split(',');

                                                foreach (string values in stats)
                                                {
                                                    ListObjectStatistics.Add(values);
                                                }
                                            }
                                        }
                                        else if (key.StartsWith("dn-group-statistics"))
                                        {
                                            if (!ListObjectStatistics.Contains(serverDetails[key].ToString()))
                                            {
                                                stats = serverDetails[key].ToString().Split(',');

                                                foreach (string values in stats)
                                                {
                                                    ListObjectStatistics.Add(values);
                                                }
                                            }
                                        }
                                        #endregion
                                    }
                                }
                        }

                        if (ListObjectStatistics.Count == 0)
                        {
                            ListObjectStatistics = ReadApplicationStatisticsValue();
                        }
                    }
                    
                }
                else if (objectType == StatisticsEnum.ObjectType.AgentGroup.ToString())
                {
                    ListObjectStatistics = ReadApplicationStatisticsValue();
                }

            }
            catch (Exception GeneralException)
            {
                logger.Error("StatisticsInformation : ReadStatisticValues Method : " + GeneralException.Message);
            }
            logger.Debug("StatisticsInformation : ReadStatisticValues Method : Exit");
            return ListObjectStatistics;
        }

        #endregion

        #region Read Application statistics objects

        public List<string> ReadApplicationStatisticsValue()
        {
            List<string> ListObjectStatistics = new List<string>();
            try
            {
                logger.Debug("StatisticsInformation : ReadApplicationStatisticsValue Method : Entry");
                if (StatisticsSetting.GetInstance().Application != null)
                {
                    KeyValueCollection serverDetails = (KeyValueCollection)StatisticsSetting.GetInstance().Application.Options["agent.ixn.desktop"];
                    foreach (string key in serverDetails.AllKeys)
                    {
                        string[] stats;

                        #region Application Value

                        if (key.StartsWith("agent-group-statistics"))
                        {
                            if (!ListObjectStatistics.Contains(serverDetails[key].ToString()))
                            {
                                stats = serverDetails[key].ToString().Split(',');

                                foreach (string values in stats)
                                {
                                    ListObjectStatistics.Add(values);
                                }
                            }
                        }
                        else if (key.StartsWith("agent-statistics"))
                        {
                            if (!ListObjectStatistics.Contains(serverDetails[key].ToString()))
                            {
                                stats = serverDetails[key].ToString().Split(',');

                                foreach (string values in stats)
                                {
                                    ListObjectStatistics.Add(values);
                                }
                            }
                        }
                        else if (key.StartsWith("virtual-queue-statistics"))
                        {
                            if (!ListObjectStatistics.Contains(serverDetails[key].ToString()))
                            {
                                stats = serverDetails[key].ToString().Split(',');

                                foreach (string values in stats)
                                {
                                    ListObjectStatistics.Add(values);
                                }
                            }
                        }
                        else if (key.StartsWith("acd-queue-statistics"))
                        {
                            if (!ListObjectStatistics.Contains(serverDetails[key].ToString()))
                            {
                                stats = serverDetails[key].ToString().Split(',');

                                foreach (string values in stats)
                                {
                                    ListObjectStatistics.Add(values);
                                }
                            }
                        }
                        else if (key.StartsWith("dn-group-statistics"))
                        {
                            if (!ListObjectStatistics.Contains(serverDetails[key].ToString()))
                            {
                                stats = serverDetails[key].ToString().Split(',');

                                foreach (string values in stats)
                                {
                                    ListObjectStatistics.Add(values);
                                }
                            }
                        }

                        #endregion
                    }
                }
            }
            catch (Exception GeneralException)
            {
                logger.Error("StatisticsInformation : ReadApplicationStatisticsValue Method : " + GeneralException.Message);
            }
            logger.Debug("StatisticsInformation : ReadApplicationStatisticsValue Method : Exit");
            return ListObjectStatistics;
        }

        #endregion

        #region Read objects

        public Dictionary<string, Dictionary<string, List<string>>> ReadStatisticsObjects(string objectId, string objectType)
        {
            Dictionary<string, List<string>> dictObjects = new Dictionary<string, List<string>>();
            Dictionary<string, Dictionary<string, List<string>>> dictObjectsReturn = new Dictionary<string, Dictionary<string, List<string>>>();

            try
            {
                List<string> tempACDList = new List<string>();
                List<string> tempDNList = new List<string>();
                List<string> tempVQList = new List<string>();

                if (objectType == StatisticsEnum.ObjectType.Agent.ToString())
                {
                    CfgPersonQuery _personQuery = new CfgPersonQuery();
                    _personQuery.EmployeeId = objectId;
                    CfgPerson cfgAgent = StatisticsSetting.GetInstance().confObject.RetrieveObject<CfgPerson>(_personQuery);

                    CfgFolderQuery folderQuery = new CfgFolderQuery();
                    folderQuery.Name = "Agent Groups";
                    folderQuery.OwnerDbid = StatisticsSetting.GetInstance().CFGTenantDBID;
                    CfgFolder folder = StatisticsSetting.GetInstance().confObject.RetrieveObject<CfgFolder>(folderQuery);

                    if (cfgAgent != null)
                    {
                        CfgAgentGroupQuery _agentGroupQuery = new CfgAgentGroupQuery();
                        _agentGroupQuery.PersonDbid = Convert.ToInt16(cfgAgent.DBID);
                        _agentGroupQuery.TenantDbid = StatisticsSetting.GetInstance().CFGTenantDBID;
                        _agentGroupQuery["folder_dbid"] = folder.DBID;
                        ICollection<CfgAgentGroup> _agentGroupDetails = StatisticsSetting.GetInstance().confObject.RetrieveMultipleObjects<CfgAgentGroup>(_agentGroupQuery);

                        foreach (CfgAgentGroup agentGroup in _agentGroupDetails)
                        {
                            StatisticsSetting.GetInstance().AgentGroupsListCollections.Add(agentGroup.GroupInfo.Name.ToString());

                            KeyValueCollection serverDetails = (KeyValueCollection)agentGroup.GroupInfo.UserProperties["agent.ixn.desktop"];

                            if (serverDetails != null)
                            {
                                foreach (string key in serverDetails.AllKeys)
                                {
                                    if (key == "statistics.objects-acd-queues" || key == "statistics.objects-dn-groups" ||
                                        key == "statistics.objects-virtual-queues")
                                    {
                                        dictObjects.Clear();
                                    }
                                }

                                foreach (string key in serverDetails.AllKeys)
                                {
                                    string[] objects;

                                    #region AgentGroup object Value

                                    if (key.StartsWith("statistics.objects-acd-queues"))
                                    {
                                        if (!dictObjects.ContainsKey("statistics.objects-acd-queues"))
                                        {
                                            objects = serverDetails[key].ToString().Split(',');

                                            foreach (string obj in objects)
                                            {
                                                if (!tempACDList.Contains(obj))
                                                    tempACDList.Add(obj);
                                            }
                                        }
                                    }
                                    else if (key.StartsWith("statistics.objects-dn-groups"))
                                    {
                                        if (!dictObjects.ContainsKey("statistics.objects-dn-groups"))
                                        {
                                            objects = serverDetails[key].ToString().Split(',');

                                            foreach (string obj in objects)
                                            {
                                                if (!tempDNList.Contains(obj))
                                                    tempDNList.Add(obj);
                                            }
                                        }
                                    }
                                    else if (key.StartsWith("statistics.objects-virtual-queues"))
                                    {
                                        if (!dictObjects.ContainsKey("statistics.objects-virtual-queues"))
                                        {
                                            objects = serverDetails[key].ToString().Split(',');

                                            foreach (string obj in objects)
                                            {
                                                if (!tempVQList.Contains(obj))
                                                    tempVQList.Add(obj);
                                            }
                                        }
                                    }
                                    #endregion
                                }

                                //if (tempACDList.Count != 0)
                                dictObjects.Add(StatisticsEnum.ObjectType.ACDQueue.ToString(), tempACDList);

                                //if (tempDNList.Count != 0)
                                dictObjects.Add(StatisticsEnum.ObjectType.DNGroup.ToString(), tempDNList);

                                //if (tempVQList.Count != 0)
                                dictObjects.Add(StatisticsEnum.ObjectType.VirtualQueue.ToString(), tempVQList);
                            }
                        }

                        if (dictObjects.Count == 0)
                        {
                            dictObjects = ReadApplicationObjectValue();
                            dictObjectsReturn.Add("Application", dictObjects);
                        }
                        else
                        {
                            dictObjectsReturn.Add("Objects", dictObjects);
                        }
                    }

                }
                else if (objectType == StatisticsEnum.ObjectType.AgentGroup.ToString())
                {
                    dictObjects = ReadApplicationObjectValue();
                    dictObjectsReturn.Add("Application", dictObjects);
                }
            }
            catch (Exception GeneralException)
            {

            }

            return dictObjectsReturn;
        }

        #endregion

        #region Read Application Object Values

        public Dictionary<string, List<string>> ReadApplicationObjectValue()
        {
            Dictionary<string, List<string>> DictObjectStatistics = new Dictionary<string, List<string>>();
            try
            {
                logger.Debug("StatisticsInformation : ReadApplicationObjectValue Method : Entry");

                List<string> tempACDList = new List<string>();
                List<string> tempDNList = new List<string>();
                List<string> tempVQList = new List<string>();

                if (StatisticsSetting.GetInstance().Application != null)
                {
                    KeyValueCollection serverDetails = (KeyValueCollection)StatisticsSetting.GetInstance().Application.Options["agent.ixn.desktop"];
                    foreach (string key in serverDetails.AllKeys)
                    {
                        string[] objects;

                        #region AgentGroup object Value

                        if (key.StartsWith("statistics.objects-acd-queues"))
                        {
                            if (!DictObjectStatistics.ContainsKey("statistics.objects-acd-queues"))
                            {
                                objects = serverDetails[key].ToString().Split(',');

                                foreach (string obj in objects)
                                {
                                    if (!tempACDList.Contains(obj))
                                        tempACDList.Add(obj);
                                }
                            }
                        }
                        else if (key.StartsWith("statistics.objects-dn-groups"))
                        {
                            if (!DictObjectStatistics.ContainsKey("statistics.objects-dn-groups"))
                            {
                                objects = serverDetails[key].ToString().Split(',');

                                foreach (string obj in objects)
                                {
                                    if (!tempDNList.Contains(obj))
                                        tempDNList.Add(obj);
                                }
                            }
                        }
                        else if (key.StartsWith("statistics.objects-virtual-queues"))
                        {
                            if (!DictObjectStatistics.ContainsKey("statistics.objects-virtual-queues"))
                            {
                                objects = serverDetails[key].ToString().Split(',');

                                foreach (string obj in objects)
                                {
                                    if (!tempVQList.Contains(obj))
                                        tempVQList.Add(obj);
                                }
                            }
                        }
                        #endregion
                    }

                    //if (tempACDList.Count != 0)
                        DictObjectStatistics.Add(StatisticsEnum.ObjectType.ACDQueue.ToString(), tempACDList);

                    //if (tempDNList.Count != 0)
                        DictObjectStatistics.Add(StatisticsEnum.ObjectType.DNGroup.ToString(), tempDNList);

                   // if (tempVQList.Count != 0)
                        DictObjectStatistics.Add(StatisticsEnum.ObjectType.VirtualQueue.ToString(), tempVQList);
                }
            }
            catch (Exception GeneralException)
            {
                logger.Error("StatisticsInformation : ReadApplicationObjectValue Method : " + GeneralException.Message);
            }
            logger.Debug("StatisticsInformation : ReadApplicationObjectValue Method : Exit");
            return DictObjectStatistics;
        }

        #endregion

        #region Read AgentGroup and Agent Values

        internal void ReadAdminValues()
        {
            try
            {
                logger.Debug("StatisticsInformation : ReadAdminValues Method : Entry");

                StatisticsSetting.GetInstance().DictDisplayObjects = new Dictionary<string, List<CfgPerson>>();

                List<CfgPerson> tempAgentList ;

                CfgFolderQuery _folderQuery = new CfgFolderQuery();
                _folderQuery.Name = "Agent Groups";
                _folderQuery.OwnerDbid = StatisticsSetting.GetInstance().CFGTenantDBID;
                CfgFolder folder = StatisticsSetting.GetInstance().confObject.RetrieveObject<CfgFolder>(_folderQuery);

                CfgAgentGroupQuery _agentGroupQuery = new CfgAgentGroupQuery();
                _agentGroupQuery.TenantDbid = StatisticsSetting.GetInstance().CFGTenantDBID;
                _agentGroupQuery["folder_dbid"] = folder.DBID;
                ICollection<CfgAgentGroup> _agentGroupDetails = StatisticsSetting.GetInstance().confObject.RetrieveMultipleObjects<CfgAgentGroup>(_agentGroupQuery);

                if (_agentGroupDetails != null)
                {
                    foreach (CfgAgentGroup agentGroup in _agentGroupDetails)
                    {
                        tempAgentList = new List<CfgPerson>();

                        CfgPersonQuery _personQuery = new CfgPersonQuery();
                        _personQuery.GroupDbid = agentGroup.DBID;
                        ICollection<CfgPerson> Persons = StatisticsSetting.GetInstance().confObject.RetrieveMultipleObjects<CfgPerson>(_personQuery);

                        if (Persons != null)
                        {
                            foreach (CfgPerson agent in Persons)
                            {
                                if (StatisticsSetting.GetInstance().LstAgentUNames.Contains(agent.UserName))
                                {
                                    tempAgentList.Add(agent);
                                }                                
                            }
                        }

                        if (tempAgentList.Count != 0)
                            StatisticsSetting.GetInstance().DictDisplayObjects.Add(agentGroup.GroupInfo.Name, tempAgentList);
                    }
                }

            }
            catch (Exception GeneralException)
            {
                logger.Error("StatisticsInformation : ReadAdminValues Method : " + GeneralException.Message);
            }
            logger.Debug("StatisticsInformation : ReadAdminValues Method : Exit");
        }

        #endregion
    }
}


