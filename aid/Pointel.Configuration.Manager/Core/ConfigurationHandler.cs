#region Header

/*
* =====================================
* Pointel.Configuration.Manager.Core.Request
* ====================================
* Project    : Agent Interaction Desktop
* Created on : 31-March-2015
* Author     : Manikandan
* Owner      : Pointel Solutions
* ====================================
*/

#endregion Header

namespace Pointel.Configuration.Manager.Core
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;

    using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel;
    using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.CfgObjects;
    using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.Queries;
    using Genesyslab.Platform.Commons.Collections;
    using Genesyslab.Platform.Configuration.Protocols.Types;

    using log4net;

    using Pointel.Configuration.Manager.Common;
    using Pointel.Configuration.Manager.ConnectionManager;
    using Pointel.Configuration.Manager.Helpers;

    internal class ConfigurationHandler
    {
        #region Fields

        private Dictionary<string, string> chatcodes = new Dictionary<string, string>();
        private Dictionary<string, Dictionary<string, string>> chatSubDict = new Dictionary<string, Dictionary<string, string>>();
        private ConfigValue configValue = new ConfigValue();
        private List<string> dummyChatSubDispositions = new List<string>();
        private List<string> dummyEmailSubDispositions = new List<string>();
        private List<string> dummyVoiceSubDispositions = new List<string>();
        private Dictionary<string, string> emailcodes = new Dictionary<string, string>();
        private Dictionary<string, Dictionary<string, string>> emailSubDict = new Dictionary<string, Dictionary<string, string>>();
        private KeyValueCollection systemValuesCollection;
        private Dictionary<string, string> voicecodes = new Dictionary<string, string>();
        private Dictionary<string, Dictionary<string, string>> voiceSubDict = new Dictionary<string, Dictionary<string, string>>();
        private CfgApplication _application;
        private ConfigContainer _configContainer = ConfigContainer.Instance();
        private int _currentAgentGroupId = 0;
        private Pointel.Logger.Core.ILog _logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
             "AID");

        //#region Single Instance
        //private static ConfigurationHandler _instance = null;
        //public static ConfigurationHandler GetInstance()
        //{
        //    if (_instance == null)
        //    {
        //        _instance = new ConfigurationHandler();
        //        return _instance;
        //    }
        //    return _instance;
        //}
        //#endregion
        private CfgPerson _person;

        #endregion Fields

        #region Constructors

        public ConfigurationHandler()
        {
        }

        ~ConfigurationHandler()
        {
            voicecodes = null;
            chatcodes = null;
            emailcodes = null;
            voiceSubDict = null;
            chatSubDict = null;
            emailSubDict = null;
            dummyVoiceSubDispositions = null;
            dummyEmailSubDispositions = null;
            dummyChatSubDispositions = null;
            systemValuesCollection = null;
            _logger = null;
            _person = null;
            _application = null;
        }

        #endregion Constructors

        #region Methods

        public void CMEObjectChanged(ConfEvent confEvent)
        {
            try
            {
                if (confEvent.EventType == ConfEventType.ObjectUpdated)
                {
                    KeyValueCollection changedKeyValuePair = null;
                    KeyValueCollection addedKeyValuePair = null;
                    int _dbId = 0;
                    ConfigValue.CFGValueObjects tempValueObject = ConfigValue.CFGValueObjects.None;
                    switch (confEvent.ObjectType)
                    {
                        case CfgObjectType.CFGApplication:
                            var deltaApplication = confEvent.Delta as CfgDeltaApplication;
                            if (deltaApplication == null || deltaApplication.DBID != _configContainer.ApplicationDbId)
                                return;
                            _dbId = deltaApplication.DBID;
                            if (deltaApplication.ChangedOptions != null)
                                changedKeyValuePair = deltaApplication.ChangedOptions;
                            if (deltaApplication.AddedOptions != null)
                                addedKeyValuePair = deltaApplication.AddedOptions;
                            //else if (deltaAplication.DeletedOptions != null)
                            //    secKeyValuePair = deltaAplication.DeletedOptions;

                            tempValueObject = ConfigValue.CFGValueObjects.Application;
                            break;

                        case CfgObjectType.CFGAgentGroup:
                            var deltaAgentGroup = confEvent.Delta as CfgDeltaAgentGroup;
                            if (deltaAgentGroup == null || deltaAgentGroup.DBID != _currentAgentGroupId)
                                return;
                            _dbId = deltaAgentGroup.DBID;

                            if (deltaAgentGroup.DeltaGroupInfo.ChangedUserProperties != null)
                                changedKeyValuePair = deltaAgentGroup.DeltaGroupInfo.ChangedUserProperties;
                            if (deltaAgentGroup.DeltaGroupInfo.AddedUserProperties != null)
                                addedKeyValuePair = deltaAgentGroup.DeltaGroupInfo.AddedUserProperties;
                            //else if (deltaAgentGroup.DeltaGroupInfo.DeletedUserProperties != null)
                            //    secKeyValuePair = deltaAgentGroup.DeltaGroupInfo.DeletedUserProperties;
                            tempValueObject = ConfigValue.CFGValueObjects.AgentGroup;
                            break;

                        case CfgObjectType.CFGPerson:
                            var deltaPerson = confEvent.Delta as CfgDeltaPerson;
                            if (deltaPerson == null || deltaPerson.DBID != _configContainer.PersonDbId)
                                return;
                            _dbId = deltaPerson.DBID;
                            if (deltaPerson.ChangedUserProperties != null)
                                changedKeyValuePair = deltaPerson.ChangedUserProperties;
                            if (deltaPerson.AddedUserProperties != null)
                                addedKeyValuePair = deltaPerson.AddedUserProperties;

                            if (deltaPerson.AgentInfo != null)
                            {
                                var defaultPlaceDBID = deltaPerson.AgentInfo.GetPlaceDBID();
                                if (defaultPlaceDBID != null)
                                {
                                    var defaultPlace = _configContainer.ConfServiceObject.RetrieveObject<CfgPlace>((new CfgPlaceQuery() { Dbid = defaultPlaceDBID ?? 0, TenantDbid = _configContainer.TenantDbId }));
                                }

                                var addedSkills = deltaPerson.AgentInfo.SkillLevels;
                                if (addedSkills != null && addedSkills.Count > 0)
                                {
                                    Dictionary<string, string> addedNewSkills = new Dictionary<string, string>();
                                    foreach (CfgSkillLevel skillLevel in addedSkills)
                                        addedNewSkills.Add(skillLevel.Skill.Name, skillLevel.Level.ToString());
                                    if (ConfigValue.SendToClient != null && addedNewSkills != null && addedNewSkills.Count > 0)
                                        ConfigValue.SendToClient.NotifyCMEObjectChanged(ConfigValue.CFGValueObjects.PersonSkill, new KeyValuePair<ConfigValue.CFGOperation, Dictionary<string, string>>(ConfigValue.CFGOperation.Add, addedNewSkills));
                                }
                            }

                            if (deltaPerson.DeltaAgentInfo != null)
                            {
                                var deletedLoginDBIDs = deltaPerson.DeltaAgentInfo.GetDeletedAgentLoginDBIDs();
                                if (deletedLoginDBIDs != null)
                                {
                                    foreach (var id in deletedLoginDBIDs)
                                    {
                                        var AgentLogin = _configContainer.ConfServiceObject.RetrieveObject<CfgAgentLogin>((new CfgAgentLoginQuery() { Dbid = id, TenantDbid = _configContainer.TenantDbId }));
                                        if (AgentLogin == null)
                                            continue;
                                        var loginCode = AgentLogin.LoginCode;
                                    }
                                }
                                var deletedSkillsDBIDs = deltaPerson.DeltaAgentInfo.GetDeletedSkillDBIDs();
                                if (deletedSkillsDBIDs != null && deletedSkillsDBIDs.Count > 0)
                                {
                                    Dictionary<string, string> deletedSkills = new Dictionary<string, string>();
                                    foreach (var id in deletedSkillsDBIDs)
                                    {
                                        var skill = _configContainer.ConfServiceObject.RetrieveObject<CfgSkill>((new CfgSkillQuery() { Dbid = id, TenantDbid = _configContainer.TenantDbId }));
                                        if (skill == null)
                                            continue;
                                        deletedSkills.Add(skill.Name, "0");
                                        var skillName = skill.Name;
                                    }
                                    if (ConfigValue.SendToClient != null)
                                        ConfigValue.SendToClient.NotifyCMEObjectChanged(ConfigValue.CFGValueObjects.PersonSkill, new KeyValuePair<ConfigValue.CFGOperation, Dictionary<string, string>>(ConfigValue.CFGOperation.Delete, deletedSkills));
                                }

                                if (deltaPerson.DeltaAgentInfo.ChangedSkillLevels != null && deltaPerson.DeltaAgentInfo.ChangedSkillLevels.Count > 0)
                                {
                                    Dictionary<string, string> changedSkills = new Dictionary<string, string>();
                                    var updatedSkills = deltaPerson.DeltaAgentInfo.ChangedSkillLevels;
                                    foreach (CfgSkillLevel skillLevel in updatedSkills)
                                    {
                                        changedSkills.Add(skillLevel.Skill.Name, skillLevel.Level.ToString());
                                    }
                                    if (ConfigValue.SendToClient != null)
                                        ConfigValue.SendToClient.NotifyCMEObjectChanged(ConfigValue.CFGValueObjects.PersonSkill, new KeyValuePair<ConfigValue.CFGOperation, Dictionary<string, string>>(ConfigValue.CFGOperation.Update, changedSkills));
                                }
                            }
                            tempValueObject = ConfigValue.CFGValueObjects.Agent;
                            break;
                    }

                    if (changedKeyValuePair == null)
                        changedKeyValuePair = new KeyValueCollection();
                    if (addedKeyValuePair != null)
                        foreach (string item in addedKeyValuePair.Keys)
                            changedKeyValuePair.Add(item, addedKeyValuePair[item].ToString());

                    if (changedKeyValuePair != null && tempValueObject != ConfigValue.CFGValueObjects.None)
                        foreach (string item in changedKeyValuePair.Keys)
                        {
                            if (item == "speed-dial.contacts")
                            {
                                object updatedContacts = GetUpdateSpeedDialContacts(_dbId, tempValueObject);
                                if (updatedContacts != null)
                                {
                                    if (tempValueObject == ConfigValue.CFGValueObjects.Application)
                                    {
                                        //if (!_configContainer.AllKeys.Contains("GlobalContacts"))
                                        //    _configContainer.AllKeys.Add("GlobalContacts");
                                        if (!_configContainer.CMEValues.ContainsKey("GlobalContacts"))
                                            _configContainer.CMEValues.Add("GlobalContacts", updatedContacts);
                                        else
                                        {
                                            _configContainer.CMEValues.Remove("GlobalContacts");
                                            _configContainer.CMEValues.Add("GlobalContacts", updatedContacts);
                                        }
                                    }
                                    else if (tempValueObject == ConfigValue.CFGValueObjects.AgentGroup)
                                    {
                                        //if (!_configContainer.AllKeys.Contains("GroupContacts"))
                                        //    _configContainer.AllKeys.Add("GroupContacts");
                                        if (!_configContainer.CMEValues.ContainsKey("GroupContacts"))
                                            _configContainer.CMEValues.Add("GroupContacts", updatedContacts);
                                        else
                                        {
                                            _configContainer.CMEValues.Remove("GroupContacts");
                                            _configContainer.CMEValues.Add("GroupContacts", updatedContacts);
                                        }
                                    }
                                    else if (tempValueObject == ConfigValue.CFGValueObjects.Agent)
                                    {
                                        //if (!_configContainer.AllKeys.Contains("AgentContacts"))
                                        //    _configContainer.AllKeys.Add("AgentContacts");
                                        if (!_configContainer.CMEValues.ContainsKey("AgentContacts"))
                                            _configContainer.CMEValues.Add("AgentContacts", updatedContacts);
                                        else
                                        {
                                            _configContainer.CMEValues.Remove("AgentContacts");
                                            _configContainer.CMEValues.Add("AgentContacts", updatedContacts);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                var keyValuePair = changedKeyValuePair[item] as KeyValueCollection;
                                foreach (string key in keyValuePair.Keys)
                                    AddToDictionary(_configContainer.CMEValues, key, keyValuePair[key].ToString(), tempValueObject);
                            }
                        }
                }
            }
            catch (Exception ex)
            {
                _logger.Error((ex.InnerException == null) ? ex.Message : ex.InnerException.ToString());
            }
        }

        public void ConfigServiceHandler(Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.ConfEvent message)
        {
            if (message.EventType.Equals(ConfEventType.ObjectUpdated))
            {
                #region Commented code

                //CfgDeltaApplication cfgDeltaApplication = message.Delta as CfgDeltaApplication;
                //if (cfgDeltaApplication != null)
                //{
                //    System.Collections.Generic.IList<ConfigElement> list = new System.Collections.Generic.List<ConfigElement>();
                //    lock (this.lockCollection)
                //    {
                //        KeyValueCollection addedOptions = cfgDeltaApplication.AddedOptions;
                //        if (addedOptions != null && addedOptions.Count != 0)
                //        {
                //            KeyValueCollection asKeyValueCollection = addedOptions.GetAsKeyValueCollection("interaction-workspace");
                //            if (asKeyValueCollection != null && asKeyValueCollection.Count != 0)
                //            {
                //                string[] allKeys = asKeyValueCollection.AllKeys;
                //                for (int i = 0; i < allKeys.Length; i++)
                //                {
                //                    string key = allKeys[i];
                //                    ConfigElement configElement = this.SetValue(key, asKeyValueCollection.Get(key));
                //                    if (configElement != null)
                //                    {
                //                        list.Add(configElement);
                //                    }
                //                }
                //            }
                //        }
                //        KeyValueCollection changedOptions = cfgDeltaApplication.ChangedOptions;
                //        if (changedOptions != null && changedOptions.Count != 0)
                //        {
                //            KeyValueCollection asKeyValueCollection = changedOptions.GetAsKeyValueCollection("interaction-workspace");
                //            if (asKeyValueCollection != null && asKeyValueCollection.Count != 0)
                //            {
                //                string[] allKeys2 = asKeyValueCollection.AllKeys;
                //                for (int j = 0; j < allKeys2.Length; j++)
                //                {
                //                    string key2 = allKeys2[j];
                //                    ConfigElement configElement = this.SetValue(key2, asKeyValueCollection.Get(key2));
                //                    if (configElement != null)
                //                    {
                //                        list.Add(configElement);
                //                    }
                //                }
                //            }
                //        }
                //        KeyValueCollection deletedOptions = cfgDeltaApplication.DeletedOptions;
                //        if (deletedOptions != null && deletedOptions.Count != 0)
                //        {
                //            KeyValueCollection asKeyValueCollection = deletedOptions.GetAsKeyValueCollection("interaction-workspace");
                //            if (asKeyValueCollection != null && asKeyValueCollection.Count != 0)
                //            {
                //                string[] allKeys3 = asKeyValueCollection.AllKeys;
                //                for (int k = 0; k < allKeys3.Length; k++)
                //                {
                //                    string key3 = allKeys3[k];
                //                    ConfigElement configElement = this.Remove(key3);
                //                    if (configElement != null)
                //                    {
                //                        list.Add(configElement);
                //                    }
                //                }
                //            }
                //        }
                //    }
                //    this.OnMultipleElementDictionaryChanged(list);
                //}

                #endregion Commented code
            }
        }

        public CfgDNType GetDNType(string name)
        {
            try
            {
                switch (name)
                {
                    case "ACDQueue":
                        return CfgDNType.CFGACDQueue;

                    case "VirtualQueue":
                        return CfgDNType.CFGVirtACDQueue;

                    case "RoutingQueue":
                        return CfgDNType.CFGRoutingQueue;
                }
            }
            catch (Exception commonException)
            {
                _logger.Error("Error occurred Get DN Type " + commonException.ToString());
            }
            return CfgDNType.CFGACDQueue;
        }

        public KeyValueCollection GetSection(string sectionName, bool readOnlyApplication = true)
        {
            try
            {
                if (!readOnlyApplication)
                {
                    CfgPersonQuery personQuery = new CfgPersonQuery();
                    personQuery.Dbid = ConfigContainer.Instance().PersonDbId;
                    personQuery.TenantDbid = _configContainer.TenantDbId;
                    CfgPerson personObject = _configContainer.ConfServiceObject.RetrieveObject<CfgPerson>(personQuery);
                    if (personObject != null)
                    {
                        if (personObject.UserProperties.ContainsKey(sectionName))
                            return personObject.UserProperties[sectionName] as KeyValueCollection;
                    }

                    CfgAgentGroupQuery agentGroupQuery = new CfgAgentGroupQuery();
                    agentGroupQuery.PersonDbid = personObject.DBID;
                    agentGroupQuery.TenantDbid = personObject.Tenant.DBID;

                    ICollection<CfgAgentGroup> agentGroupCollection = _configContainer.ConfServiceObject.RetrieveMultipleObjects<CfgAgentGroup>(agentGroupQuery);
                    if (agentGroupCollection != null && agentGroupCollection.Count > 0)
                    {
                        KeyValueCollection agentGroupContacts = new KeyValueCollection();
                        foreach (CfgAgentGroup agentGroup in agentGroupCollection)
                            if (agentGroup.GroupInfo.UserProperties != null && agentGroup.GroupInfo.UserProperties.Count > 0 &&
                               agentGroup.GroupInfo.UserProperties.ContainsKey(sectionName))
                                return agentGroup.GroupInfo.UserProperties[sectionName] as KeyValueCollection;
                    }
                }
                CfgApplicationQuery objApplicationQuery = new CfgApplicationQuery();
                objApplicationQuery.Dbid = ConfigContainer.Instance().ApplicationDbId;
                CfgApplication objApplication = ConfigContainer.Instance().ConfServiceObject.RetrieveObject<CfgApplication>(objApplicationQuery);
                if (objApplication != null)
                {
                    if (objApplication.Options != null && objApplication.Options.ContainsKey(sectionName))
                        return objApplication.Options[sectionName] as KeyValueCollection;
                    else
                        _logger.Warn("There is no section available in the name '" + sectionName + "'");
                }
                else
                    _logger.Warn("There is no application available.");
            }
            catch (Exception _generalException)
            {
                _logger.Error("Error occurred while reading application section as " + _generalException.Message);
            }
            return null;
        }

        public void GetSubDispositionBusinessAttribute(string businessAttributeName, string media)
        {
            try
            {
                CfgEnumeratorQuery businessAttributeQuery = new CfgEnumeratorQuery();
                businessAttributeQuery.Name = businessAttributeName;
                businessAttributeQuery.EnumeratorType = Convert.ToInt32(CfgEnumeratorType.CFGENTInteractionOperationalAttribute);
                businessAttributeQuery.TenantDbid = _configContainer.TenantDbId;

                var businessAttribute = _configContainer.ConfServiceObject.RetrieveObject<CfgEnumerator>(businessAttributeQuery);
                if (businessAttribute != null)
                {
                    CfgEnumeratorValueQuery attributeValuesQuery = new CfgEnumeratorValueQuery();
                    attributeValuesQuery.EnumeratorDbid = businessAttribute.DBID;
                    var attributeValues = _configContainer.ConfServiceObject.RetrieveMultipleObjects<CfgEnumeratorValue>(attributeValuesQuery);

                    if (attributeValues != null && attributeValues.Count > 0)
                    {
                        string multiDispositionKeyName = string.Empty;

                        if (media.ToLower().Equals("voice"))
                            multiDispositionKeyName = "voice.multi-disposition";
                        else if (media.ToLower().Equals("email"))
                            multiDispositionKeyName = "email.multi-disposition";
                        else if (media.ToLower().Equals("chat"))
                            multiDispositionKeyName = "chat.multi-disposition";

                        Dictionary<string, string> tempList = new Dictionary<string, string>();
                        foreach (CfgEnumeratorValue enumerator in attributeValues)
                        {
                            if (enumerator.State == CfgObjectState.CFGEnabled && media.ToLower().Equals("voice"))
                                tempList.Add(enumerator.Name, enumerator.DisplayName);
                            if (enumerator.IsDefault == CfgFlag.CFGTrue && media.ToLower().Equals("voice"))
                                tempList.Add("DefaultDispositionCode", enumerator.Name);
                            if (enumerator.UserProperties != null && enumerator.UserProperties.ContainsKey("agent.ixn.desktop"))
                            {
                                if (((KeyValueCollection)enumerator.UserProperties["agent.ixn.desktop"]).ContainsKey("channels"))
                                {
                                    if (((KeyValueCollection)enumerator.UserProperties["agent.ixn.desktop"])["channels"].ToString().Contains(media.ToLower()))
                                    {
                                        if (enumerator.State == CfgObjectState.CFGEnabled)
                                            tempList.Add(enumerator.Name, enumerator.DisplayName);
                                        if (enumerator.IsDefault == CfgFlag.CFGTrue)
                                            tempList.Add("DefaultDispositionCode", enumerator.Name);
                                    }
                                }
                                if (((KeyValueCollection)enumerator.UserProperties["agent.ixn.desktop"]).ContainsKey(multiDispositionKeyName))
                                {
                                    if (((KeyValueCollection)enumerator.UserProperties["agent.ixn.desktop"])[multiDispositionKeyName].ToString() != string.Empty)
                                    {
                                        if (!tempList.ContainsKey(enumerator.Name))
                                            tempList.Add(enumerator.Name, enumerator.DisplayName + "," + ((KeyValueCollection)enumerator.UserProperties["agent.ixn.desktop"])[multiDispositionKeyName].ToString());
                                        else
                                            tempList[enumerator.Name] = enumerator.DisplayName + "," + ((KeyValueCollection)enumerator.UserProperties["agent.ixn.desktop"])[multiDispositionKeyName].ToString();
                                        GetSubDispositionBusinessAttribute(((KeyValueCollection)enumerator.UserProperties["agent.ixn.desktop"])[multiDispositionKeyName].ToString(), media);
                                    }
                                }
                            }
                        }

                        var sortedTempList = from pair in tempList
                                             orderby pair.Value ascending
                                             select pair;
                        tempList = sortedTempList.ToDictionary(pair => pair.Key, pair => pair.Value);
                        if (media.ToLower().Equals("voice"))
                        {
                            if (!voiceSubDict.ContainsKey(businessAttributeName))
                                voiceSubDict.Add(businessAttributeName, tempList);
                        }
                        else if (media.ToLower().Equals("email"))
                        {
                            if (!emailSubDict.ContainsKey(businessAttributeName))
                                emailSubDict.Add(businessAttributeName, tempList);
                        }
                        else if (media.ToLower().Equals("chat"))
                        {
                            if (!chatSubDict.ContainsKey(businessAttributeName))
                                chatSubDict.Add(businessAttributeName, tempList);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error in GetSubTransaction : " + ex.Message.ToString());
            }
        }

        public void GetSubTransaction(string transactionName, string section)
        {
            try
            {
                CfgTransaction Transaction;
                CfgTransactionQuery qTransaction = new CfgTransactionQuery();
                qTransaction.Name = transactionName;
                qTransaction.TenantDbid = _configContainer.TenantDbId;
                Transaction = (CfgTransaction)_configContainer.ConfServiceObject.RetrieveObject<CfgTransaction>(qTransaction);
                if (Transaction == null)
                    return;
                KeyValueCollection DispositionUserProperties = (KeyValueCollection)Transaction.UserProperties[section];
                Dictionary<string, string> tempList = new Dictionary<string, string>();
                if (DispositionUserProperties != null && DispositionUserProperties.Count > 0)
                {
                    if (section.ToLower().Contains("voice"))
                    {
                        foreach (string key in DispositionUserProperties.AllKeys)
                        {
                            if (!DispositionUserProperties[key].ToString().Contains(','))
                                tempList.Add(key, DispositionUserProperties[key].ToString());
                            else if (DispositionUserProperties[key].ToString().Contains(','))
                            {
                                dummyVoiceSubDispositions.Add(transactionName);
                                string[] split = DispositionUserProperties[key].ToString().Split(',');
                                if (!dummyVoiceSubDispositions.Contains(split[1].Trim()))
                                {
                                    tempList.Add(key, DispositionUserProperties[key].ToString());
                                    if (split[1] != string.Empty)
                                    {
                                        GetSubTransaction(split[1].Trim(), section.Trim());
                                    }
                                }
                            }
                        }
                        voiceSubDict.Add(transactionName, tempList);
                    }
                    if (section.ToLower().Contains("chat"))
                    {
                        foreach (string key in DispositionUserProperties.AllKeys)
                        {
                            if (!DispositionUserProperties[key].ToString().Contains(','))
                                tempList.Add(key, DispositionUserProperties[key].ToString());
                            else if (DispositionUserProperties[key].ToString().Contains(','))
                            {
                                dummyChatSubDispositions.Add(transactionName);
                                string[] split = DispositionUserProperties[key].ToString().Split(',');
                                if (!dummyChatSubDispositions.Contains(split[1].Trim()))
                                {
                                    tempList.Add(key, DispositionUserProperties[key].ToString());
                                    if (split[1] != string.Empty)
                                    {
                                        GetSubTransaction(split[1].Trim(), section.Trim());
                                    }
                                }
                            }
                        }
                        chatSubDict.Add(transactionName, tempList);
                    }
                    if (section.ToLower().Contains("email"))
                    {
                        foreach (string key in DispositionUserProperties.AllKeys)
                        {
                            if (!DispositionUserProperties[key].ToString().Contains(','))
                                tempList.Add(key, DispositionUserProperties[key].ToString());
                            else if (DispositionUserProperties[key].ToString().Contains(','))
                            {
                                dummyEmailSubDispositions.Add(transactionName);
                                string[] split = DispositionUserProperties[key].ToString().Split(',');
                                if (!dummyEmailSubDispositions.Contains(split[1].Trim()))
                                {
                                    tempList.Add(key, DispositionUserProperties[key].ToString());
                                    if (split[1] != string.Empty)
                                    {
                                        GetSubTransaction(split[1].Trim(), section.Trim());
                                    }
                                }
                            }
                        }
                        emailSubDict.Add(transactionName, tempList);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error in GetSubTransaction : " + ex.Message.ToString());
            }
        }

        public OutputValues ReadAccessPermission(string groupName)
        {
            OutputValues outputMessage = new OutputValues();
            try
            {
                _logger.Debug("Retrieving Access Permissions from Group Name : " + groupName + " DBID : " + _configContainer.PersonDbId);
                CfgAccessGroupQuery queryAccessGroup = new CfgAccessGroupQuery();
                queryAccessGroup.Name = groupName;
                queryAccessGroup.TenantDbid = _configContainer.TenantDbId;
                CfgAccessGroup AdminGroup = _configContainer.ConfServiceObject.RetrieveObject<CfgAccessGroup>(queryAccessGroup);
                List<CfgID> memberIds = new List<CfgID>();
                if (AdminGroup != null)
                {
                    if (AdminGroup.MemberIDs != null && AdminGroup.MemberIDs.Count != 0)
                    {
                        bool _isAgentAvailable = false;
                        memberIds = (List<CfgID>)AdminGroup.MemberIDs;
                        if (memberIds != null && memberIds.Count > 0)
                        {
                            foreach (CfgID _memid in memberIds)
                            {
                                if (_memid.DBID == _configContainer.PersonDbId)
                                {
                                    _isAgentAvailable = true;
                                    outputMessage.MessageCode = "200";
                                    outputMessage.Message = "Person is available in Access Group " + groupName;
                                    _logger.Debug("Person is available in Access Group " + groupName);
                                    break;
                                }
                            }
                        }
                        if (!_isAgentAvailable)
                        {
                            outputMessage.MessageCode = "2001";
                            outputMessage.Message = "Agent is not available in " + groupName + " Access Group";
                            _logger.Error("Agent is not available in" + groupName + "Access Group");
                        }
                    }
                    else
                    {
                        outputMessage.MessageCode = "2001";
                        outputMessage.Message = "Agent is not available in " + groupName + " Access Group";
                        _logger.Error("Agent is not available in" + groupName + "Access Group");
                    }
                }
                else
                {
                    outputMessage.MessageCode = "2001";
                    outputMessage.Message = "Unable to find the Access Group with Name : " + groupName;
                    _logger.Error("Unable to find the Access Group with Name : " + groupName);
                }
            }
            catch (Exception commonException)
            {
                outputMessage.MessageCode = "2001";
                outputMessage.Message = "Error occurred while Reading AccessPermission " + commonException.ToString();
                _logger.Error("Error occurred while Reading AccessPermission " + commonException.ToString());
            }
            return outputMessage;
        }

        public void ReadAgentGroup(string userName, string[] sectionToRead, params string[] sections)
        {
            try
            {
                CfgPersonQuery personQuery = new CfgPersonQuery();
                personQuery.UserName = userName;
                personQuery.TenantDbid = _configContainer.TenantDbId;

                CfgPerson personObject = _configContainer.ConfServiceObject.RetrieveObject<CfgPerson>(personQuery);

                if (personObject != null)
                {
                    CfgAgentGroupQuery agentGroupQuery = new CfgAgentGroupQuery();
                    agentGroupQuery.PersonDbid = personObject.DBID;
                    agentGroupQuery.TenantDbid = personObject.Tenant.DBID;

                    ICollection<CfgAgentGroup> agentGroupCollection = _configContainer.ConfServiceObject.RetrieveMultipleObjects<CfgAgentGroup>(agentGroupQuery);
                    if (agentGroupCollection != null && agentGroupCollection.Count > 0)
                    {
                        if (!_configContainer.CMEValues.ContainsKey("CfgAgentGroup"))
                            _configContainer.CMEValues.Add("CfgAgentGroup", agentGroupCollection.ToList<CfgAgentGroup>());
                        else
                            _configContainer.CMEValues["CfgAgentGroup"] = agentGroupCollection.ToList<CfgAgentGroup>();

                        KeyValueCollection agentGroupContacts = new KeyValueCollection();
                        foreach (CfgAgentGroup agentGroup in agentGroupCollection)
                        {
                            if (agentGroup != null)
                            {
                                _currentAgentGroupId = agentGroup.DBID;
                                if (agentGroup.GroupInfo.UserProperties != null && agentGroup.GroupInfo.UserProperties.Count > 0)
                                {
                                    _logger.Debug("Reading agent group values from Agent group : " + agentGroup.GroupInfo.Name);
                                    foreach (string kvCollkey in agentGroup.GroupInfo.UserProperties.AllKeys)
                                    {
                                        if (sectionToRead == null || !sectionToRead.Contains(kvCollkey))
                                            continue;
                                        if (agentGroup.GroupInfo.UserProperties[kvCollkey] != null && ((KeyValueCollection)agentGroup.GroupInfo.UserProperties[kvCollkey]).Count > 0 &&
                                                kvCollkey != "speed-dial.contacts" && (kvCollkey == "agent.ixn.desktop" || kvCollkey == "enable-disable-channels"))
                                        {
                                            _logger.Debug("Reading Agent group : " + agentGroup.GroupInfo.Name + " Section : " +
                                                            agentGroup.GroupInfo.UserProperties[kvCollkey].ToString());
                                            foreach (string kvp in ((KeyValueCollection)agentGroup.GroupInfo.UserProperties[kvCollkey]).AllKeys)
                                            {
                                                _logger.Debug("Reading Agent Group values Key : " + kvp + " Value : " +
                                                            ((KeyValueCollection)agentGroup.GroupInfo.UserProperties[kvCollkey])[kvp].ToString());
                                                AddToDictionary(_configContainer.CMEValues, kvp, ((KeyValueCollection)agentGroup.GroupInfo.UserProperties[kvCollkey])[kvp].ToString(),
                                                    ConfigValue.CFGValueObjects.AgentGroup);
                                            }
                                        }

                                        //if (kvColl != null && kvColl.Count > 0)
                                        //{
                                        //    foreach (string kvp in kvColl.AllKeys)
                                        //    {
                                        //        AddToDictionary(_configContainer.CMEValues, kvp, kvColl[kvp].ToString(), ConfigValue.CFGValueObjects.AgentGroup);
                                        //    }
                                        //}
                                        else if (kvCollkey == "speed-dial.contacts" && agentGroup.GroupInfo.UserProperties[kvCollkey] != null &&
                                                ((KeyValueCollection)agentGroup.GroupInfo.UserProperties[kvCollkey]).Count > 0)
                                        {
                                            agentGroupContacts.Add(((KeyValueCollection)agentGroup.GroupInfo.UserProperties[kvCollkey]));
                                            _logger.Debug("Agent Group : " + agentGroup.GroupInfo.Name + " Speed dial contacts : " +
                                                        agentGroup.GroupInfo.UserProperties[kvCollkey].ToString());
                                            //Dictionary<string, string> agentGroupContacts = new Dictionary<string, string>();
                                            //foreach (string kvp in ((KeyValueCollection)agentGroup.GroupInfo.UserProperties[kvCollkey]).Keys)
                                            //    agentGroupContacts.Add(kvp, ((KeyValueCollection)agentGroup.GroupInfo.UserProperties[kvCollkey])[kvp].ToString());

                                            //if (!_configContainer.AllKeys.Contains("GroupContacts"))
                                            //        _configContainer.AllKeys.Add("GroupContacts");

                                            //if (_configContainer.CMEValues.ContainsKey("GroupContacts"))
                                            //    _configContainer.CMEValues.Add("GroupContacts", agentGroupContacts);
                                            //else
                                            //    _configContainer.CMEValues["GroupContacts"] = agentGroupContacts;
                                        }
                                    }
                                }
                            }
                        }

                        //_configContainer.AllKeys.Add("GroupContacts");

                        if (!_configContainer.CMEValues.ContainsKey("GroupContacts"))
                            _configContainer.CMEValues.Add("GroupContacts", agentGroupContacts);
                        else
                            _configContainer.CMEValues["GroupContacts"] = agentGroupContacts;
                    }
                }
            }
            catch (Exception generalException)
            {
                _logger.Error("Error occurred while reading the Agent group : " +
                   ((generalException.InnerException == null) ? generalException.Message : generalException.InnerException.ToString()));
            }
        }

        public void ReadAllDNs(int switchDBID)
        {
            _logger.Debug("Retrieving DN Details start.");
            List<string> allExDNs = new List<string>();
            try
            {
                CfgDNQuery dnQuery = new CfgDNQuery();
                dnQuery.TenantDbid = _configContainer.TenantDbId;

                var dntype = _configContainer.GetAsString("call-control") == "both" ? CfgDNType.CFGExtension : (_configContainer.GetAsString("call-control") == "acd" ?
                                                               CfgDNType.CFGACDPosition : CfgDNType.CFGExtension);
                dnQuery.DnType = dntype;
                dnQuery.SwitchDbid = switchDBID;
                ICollection<CfgDN> DNCollection = _configContainer.ConfServiceObject.RetrieveMultipleObjects<CfgDN>(dnQuery);

                if (DNCollection != null && DNCollection.Count > 0)
                {

                    allExDNs = DNCollection.Cast<CfgDN>().Select(x => x.Number).ToList();
                }

                _logger.Debug("ReadAllDNs : The total dn count - " + allExDNs.Count.ToString());
                if (!_configContainer.AllKeys.Contains("ForwardDns") && !_configContainer.CMEValues.ContainsKey("ForwardDns"))
                {
                    _configContainer.CMEValues.Add("ForwardDns", allExDNs);
                }
            }
            catch (Exception commonException)
            {
                _logger.Error("Error occurred while reading all DNs " + commonException.ToString());
            }
        }

        public void ReadAllPlaces()
        {
            //tring ExtACDDN;
            //List<string> placeTable = new List<string>();
            try
            {
                CfgPlaceQuery placeQuery = new CfgPlaceQuery();
                placeQuery.TenantDbid = _configContainer.TenantDbId;
                ICollection<CfgPlace> placeCollections = _configContainer.ConfServiceObject.RetrieveMultipleObjects<CfgPlace>(placeQuery);
                _logger.Debug("Retrieving Place Details ");
                if (placeCollections != null && placeCollections.Count > 0)
                {

                    if (!_configContainer.AllKeys.Contains("AllPlaces") && !_configContainer.CMEValues.ContainsKey("AllPlaces"))
                    {
                        _configContainer.AllKeys.Add("AllPlaces");
                        _configContainer.CMEValues.Add("AllPlaces", placeCollections.Cast<CfgPlace>().Select(x => x.Name).ToList());
                    }
                    //foreach (CfgPlace place in placeCollections)
                    //{
                    //    ExtACDDN = string.Empty;
                    //    IList<CfgDN> DNCollection = (IList<CfgDN>)place.DNs;

                    //    if (DNCollection != null && DNCollection.Count > 0)
                    //    {
                    //        foreach (CfgDN DN in DNCollection)
                    //        {
                    //            if (DN.Type == CfgDNType.CFGACDPosition)
                    //            {
                    //                if (ExtACDDN.Equals(string.Empty))
                    //                {
                    //                    ExtACDDN = "A" + DN.Number;
                    //                }
                    //                else
                    //                {
                    //                    ExtACDDN += ":" + "A" + DN.Number;
                    //                }
                    //            }
                    //            else if (DN.Type == CfgDNType.CFGExtension)
                    //            {
                    //                if (!ExtACDDN.Equals(string.Empty))
                    //                {
                    //                    ExtACDDN += ":" + "E" + DN.Number;
                    //                }
                    //                else
                    //                {
                    //                    ExtACDDN = "E" + DN.Number;
                    //                }
                    //            }
                    //        }
                    //        placeTable.Add(place.Name.ToString(), ExtACDDN);
                    //    }
                    //}
                }
                else
                {
                    if (!_configContainer.AllKeys.Contains("AllPlaces") && !_configContainer.CMEValues.ContainsKey("AllPlaces"))
                    {
                        _configContainer.AllKeys.Add("AllPlaces");
                        _configContainer.CMEValues.Add("AllPlaces", null);
                    }
                }
                _logger.Debug("Retrieving Place Details End.");
            }
            catch (Exception ex)
            {
                _logger.Error("Error occurred at reading all places : " + ((ex.InnerException == null) ? ex.Message.ToString() : ex.InnerException.ToString()));
            }
        }

        public void ReadAllSkills()
        {
            try
            {
                List<string> _allSkills = new List<string>();
                CfgSkillQuery skillQuery = new CfgSkillQuery();
                skillQuery.TenantDbid = _configContainer.TenantDbId;

                ICollection<CfgSkill> allSkillsCollection = _configContainer.ConfServiceObject.RetrieveMultipleObjects<CfgSkill>(skillQuery);
                _logger.Debug("Retrieving All Skills Details");
                if (allSkillsCollection != null)
                {
                    foreach (CfgSkill skill in allSkillsCollection)
                    {
                        _allSkills.Add(skill.Name.ToString());
                    }
                }
                if (_allSkills != null && !_configContainer.CMEValues.ContainsKey("AllSkills"))
                {
                    _logger.Info("Total Skill count : " + _allSkills.Count);
                    //_configContainer.AllKeys.Add("AllSkills");
                    _configContainer.CMEValues.Add("AllSkills", _allSkills);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error occurred at reading all skills : " + ((ex.InnerException == null) ? ex.Message.ToString() : ex.InnerException.ToString()));
            }
        }

        public void ReadApplication(string applicationName, string[] sectionToRead, params string[] sections)
        {
            try
            {
                CfgApplicationQuery applicationQuery = new CfgApplicationQuery();
                applicationQuery.Name = applicationName;
                _logger.Debug("Reading application values from : " + applicationName);
                //applicationQuery.TenantDbid = WellKnownDbids.EnterpriseModeTenantDbid;

                CfgApplication applicationObject = _configContainer.ConfServiceObject.RetrieveObject<CfgApplication>(applicationQuery);
                if (applicationObject != null)
                {
                    if (!_configContainer.AllKeys.Contains("CfgApplication") && !_configContainer.CMEValues.ContainsKey("CfgApplication"))
                    {
                        _configContainer.CMEValues.Add("CfgApplication", applicationObject);
                    }
                    _application = applicationObject;
                    _configContainer.ApplicationDbId = applicationObject.DBID;
                    if (applicationObject.Options != null && applicationObject.Options.Count > 0)
                    {
                        foreach (string kvCollkey in applicationObject.Options.AllKeys)
                        {
                            if (applicationObject.Options[kvCollkey] != null && ((KeyValueCollection)applicationObject.Options[kvCollkey]).Count > 0)
                            {
                                //Changed by sakthi on 12-0-2015 for differentiate read as key or section.
                                if (!sectionToRead.Contains(kvCollkey))
                                    continue;
                                if (applicationObject.Options[kvCollkey] != null && ((KeyValueCollection)applicationObject.Options[kvCollkey]).Count > 0)
                                {
                                    if (sections != null && sections.Contains(kvCollkey))
                                    {
                                        if (kvCollkey == "speed-dial.contacts")
                                        {
                                            _logger.Debug("Application level Speed dial contacts : " + ((KeyValueCollection)applicationObject.Options[kvCollkey]));
                                            //_configContainer.AllKeys.Add("GlobalContacts");
                                            _configContainer.CMEValues.Add("GlobalContacts", ((KeyValueCollection)applicationObject.Options[kvCollkey]));
                                        }
                                        else
                                        {
                                            _logger.Debug("Reading application section name : " + kvCollkey + " Value : " + ((KeyValueCollection)applicationObject.Options[kvCollkey]).ToString());
                                            //_configContainer.AllKeys.Add(kvCollkey);
                                            _configContainer.CMEValues.Add(kvCollkey, ((KeyValueCollection)applicationObject.Options[kvCollkey]));
                                        }
                                    }
                                    else
                                        foreach (string kvp in ((KeyValueCollection)applicationObject.Options[kvCollkey]).AllKeys)
                                        {
                                            _logger.Debug("Reading application values Key : " + kvp + " Value : " + ((KeyValueCollection)applicationObject.Options[kvCollkey])[kvp].ToString());
                                            AddToDictionary(_configContainer.CMEValues, kvp, ((KeyValueCollection)applicationObject.Options[kvCollkey])[kvp].ToString(),
                                                ConfigValue.CFGValueObjects.Application);
                                        }
                                }

                                #region Old Code

                                //switch (kvCollkey)
                                //{
                                //    case "speed-dial.contacts":
                                //        _logger.Debug("Application level Speed dial contacts : " + ((KeyValueCollection)applicationObject.Options[kvCollkey]));
                                //        _configContainer.AllKeys.Add("GlobalContacts");
                                //        _configContainer.CMEValues.Add("GlobalContacts", ((KeyValueCollection)applicationObject.Options[kvCollkey]));
                                //        break;

                                //    case "citrix-popup":
                                //        _logger.Debug("Application level citrix-popup : " + ((KeyValueCollection)applicationObject.Options[kvCollkey]));
                                //        _configContainer.AllKeys.Add("citrixKeysValues");
                                //        _configContainer.CMEValues.Add("citrixKeysValues", ((KeyValueCollection)applicationObject.Options[kvCollkey]));
                                //        break;

                                //    default:
                                //        _logger.Debug("Reading application values from Section : " + kvCollkey);
                                //        foreach (string kvp in ((KeyValueCollection)applicationObject.Options[kvCollkey]).AllKeys)
                                //        {
                                //            _logger.Debug("Reading application values Key : " + kvp + " Value : " + ((KeyValueCollection)applicationObject.Options[kvCollkey])[kvp].ToString());
                                //            AddToDictionary(_configContainer.CMEValues, kvp, ((KeyValueCollection)applicationObject.Options[kvCollkey])[kvp].ToString(),
                                //                ConfigValue.CFGValueObjects.Application);
                                //        }
                                //        break;
                                //}

                                #endregion Old Code
                            }

                            #region Oldcode

                            //if (applicationObject.Options[kvCollkey] != null && ((KeyValueCollection)applicationObject.Options[kvCollkey]).Count > 0 &&
                            //            kvCollkey != "speed-dial.contacts" && kvCollkey != "citrix-popup")
                            //{
                            //    _logger.Debug("Reading application values from Section : " + kvCollkey);
                            //    foreach (string kvp in ((KeyValueCollection)applicationObject.Options[kvCollkey]).AllKeys)
                            //    {
                            //        _logger.Debug("Reading application values Key : " + kvp + " Value : " + ((KeyValueCollection)applicationObject.Options[kvCollkey])[kvp].ToString());
                            //        AddToDictionary(_configContainer.CMEValues, kvp, ((KeyValueCollection)applicationObject.Options[kvCollkey])[kvp].ToString(),
                            //            ConfigValue.CFGValueObjects.Application);
                            //    }
                            //}
                            //else if (kvCollkey == "speed-dial.contacts" &&
                            //    applicationObject.Options[kvCollkey] != null && ((KeyValueCollection)applicationObject.Options[kvCollkey]).Count > 0)
                            //{
                            //    //Dictionary<string, string> applicatoionContacts = new Dictionary<string, string>();
                            //    //foreach (string kvp in ((KeyValueCollection)applicationObject.Options[kvCollkey]).Keys)
                            //    //    applicatoionContacts.Add(kvp, ((KeyValueCollection)applicationObject.Options[kvCollkey])[kvp].ToString());
                            //    _logger.Debug("Application level Speed dial contacts : " + ((KeyValueCollection)applicationObject.Options[kvCollkey]));
                            //    _configContainer.AllKeys.Add("GlobalContacts");
                            //    _configContainer.CMEValues.Add("GlobalContacts", ((KeyValueCollection)applicationObject.Options[kvCollkey]));
                            //}

                            #endregion Oldcode
                        }
                    }
                    if (applicationObject.AppServers != null)
                    {
                        foreach (CfgConnInfo appDetails in applicationObject.AppServers)
                        {
                            if (appDetails.AppServer.Type == CfgAppType.CFGContactServer)
                            {
                                if (appDetails.AppServer == null)
                                {
                                    _logger.Warn("Contact Server Primary server is not available");
                                    //if (!_configContainer.AllKeys.Contains("IsUCSAvailable"))
                                    //    _configContainer.AllKeys.Add("IsUCSAvailable");
                                    if (!_configContainer.CMEValues.ContainsKey("IsUCSAvailable"))
                                        _configContainer.CMEValues.Add("IsUCSAvailable", "false");
                                }
                                else
                                {
                                    //if (!_configContainer.AllKeys.Contains("IsUCSAvailable"))
                                    //    _configContainer.AllKeys.Add("IsUCSAvailable");
                                    if (!_configContainer.CMEValues.ContainsKey("IsUCSAvailable"))
                                        _configContainer.CMEValues.Add("IsUCSAvailable", "true");
                                }
                                break;
                            }
                        }
                    }
                }
                else
                {
                    _logger.Warn("Application is not available");
                }
            }
            catch (Exception generalException)
            {
                _logger.Error("Error occurred while reading the application : " +
                    ((generalException.InnerException == null) ? generalException.Message : generalException.InnerException.ToString()));
            }
        }

        public void ReadCaseDataFilterTransactionObject(string name)
        {
            KeyValueCollection caseDataFilterUserProperties = new KeyValueCollection();
            try
            {
                List<string> loadAttachDataFilterKey = new List<string>();
                Dictionary<string, string> loadOutboundCallResult = new Dictionary<string, string>();
                _logger.Debug("Retrieving values from Case Data Filter Transaction Object : Transaction Name : " + name);

                CfgTransaction Transaction;
                CfgTransactionQuery qTransaction = new CfgTransactionQuery();
                qTransaction.TenantDbid = _configContainer.TenantDbId;
                qTransaction.Name = name;
                Transaction = (CfgTransaction)_configContainer.ConfServiceObject.RetrieveObject<CfgTransaction>(qTransaction);
                if (Transaction == null)
                {
                    _logger.Warn(" case data filter Transaction object : " + name + " is not found");
                    return;
                }

                caseDataFilterUserProperties = (KeyValueCollection)Transaction.UserProperties["voice.case-data-filter"];
                loadAttachDataFilterKey.Clear();
                if (caseDataFilterUserProperties != null && caseDataFilterUserProperties.Count > 0)
                {
                    foreach (string key in caseDataFilterUserProperties.AllKeys)
                    {
                        _logger.Debug("Voice Case data Filter : " + key);
                        loadAttachDataFilterKey.Add(caseDataFilterUserProperties[key].ToString());
                    }
                }
                if (!_configContainer.CMEValues.ContainsKey("VoiceAttachDataFilterKey"))
                {
                    // _configContainer.AllKeys.Add("VoiceAttachDataFilterKey");
                    _configContainer.CMEValues.Add("VoiceAttachDataFilterKey", loadAttachDataFilterKey);
                }

                caseDataFilterUserProperties = (KeyValueCollection)Transaction.UserProperties["email.case-data-filter"];
                loadAttachDataFilterKey.Clear();
                if (caseDataFilterUserProperties != null && caseDataFilterUserProperties.Count > 0)
                {
                    foreach (string key in caseDataFilterUserProperties.AllKeys)
                    {
                        _logger.Debug("Email Case data Filter : " + key);
                        loadAttachDataFilterKey.Add(caseDataFilterUserProperties[key].ToString());
                    }
                }
                if (!_configContainer.CMEValues.ContainsKey("EmailAttachDataFilterKey"))
                {
                    //_configContainer.AllKeys.Add("EmailAttachDataFilterKey");
                    _configContainer.CMEValues.Add("EmailAttachDataFilterKey", loadAttachDataFilterKey);
                }

                caseDataFilterUserProperties = (KeyValueCollection)Transaction.UserProperties["chat.case-data-filter"];
                loadAttachDataFilterKey.Clear();
                if (caseDataFilterUserProperties != null && caseDataFilterUserProperties.Count > 0)
                {
                    foreach (string key in caseDataFilterUserProperties.AllKeys)
                    {
                        _logger.Debug("Chat Case data Filter : " + key);
                        loadAttachDataFilterKey.Add(caseDataFilterUserProperties[key].ToString());
                    }
                }
                if (!_configContainer.CMEValues.ContainsKey("ChatAttachDataFilterKey"))
                {
                    //_configContainer.AllKeys.Add("ChatAttachDataFilterKey");
                    _configContainer.CMEValues.Add("ChatAttachDataFilterKey", loadAttachDataFilterKey);
                }

                caseDataFilterUserProperties = (KeyValueCollection)Transaction.UserProperties["voice.outbound-call-result"];
                loadOutboundCallResult.Clear();
                if (caseDataFilterUserProperties != null && caseDataFilterUserProperties.Count > 0)
                {
                    foreach (string key in caseDataFilterUserProperties.AllKeys)
                    {
                        _logger.Debug("voice outbound call result : " + key);
                        loadOutboundCallResult.Add(key, caseDataFilterUserProperties[key].ToString());
                    }
                }
                if (!_configContainer.CMEValues.ContainsKey("OutboundCallResult"))
                {
                    // _configContainer.AllKeys.Add("OutboundCallResult");
                    _configContainer.CMEValues.Add("OutboundCallResult", loadOutboundCallResult);
                }
            }
            catch (Exception commonException)
            {
                _logger.Error("Error occurred while reading CaseDataFilterTransactionObject " + name + "  =  " + commonException.ToString());
            }
        }

        /// <summary>
        /// Reads the case data from business attribute.
        /// </summary>
        /// <param name="businessAttributeName">Name of the business attribute.</param>
        public void ReadCaseDataFromBusinessAttribute(string businessAttributeName)
        {
            try
            {
                string dispKey = "";
                string dispCollKey = "";
                if (_configContainer.CMEValues.ContainsKey("interaction.disposition.key-name") &&
                            !string.IsNullOrEmpty(((ConfigValue)_configContainer.CMEValues["interaction.disposition.key-name"]).Value))
                    dispKey = ((ConfigValue)_configContainer.CMEValues["interaction.disposition.key-name"]).Value.ToString();
                if (_configContainer.CMEValues.ContainsKey("interaction.disposition-collection.key-name") &&
                            !string.IsNullOrEmpty(((ConfigValue)_configContainer.CMEValues["interaction.disposition-collection.key-name"]).Value))
                    dispCollKey = ((ConfigValue)_configContainer.CMEValues["interaction.disposition-collection.key-name"]).Value.ToString();

                _logger.Debug("ReadCaseDataFromBusinessAttribute Entry");
                CfgEnumeratorQuery businessAttributeQuery = new CfgEnumeratorQuery();
                businessAttributeQuery.Name = businessAttributeName;
                businessAttributeQuery.EnumeratorType = Convert.ToInt32(CfgEnumeratorType.CFGENTInteractionOperationalAttribute);
                businessAttributeQuery.TenantDbid = _configContainer.TenantDbId;

                _logger.Debug("ReadCaseDataFromBusinessAttribute business attribute name : " + businessAttributeName);
                var businessAttribute = _configContainer.ConfServiceObject.RetrieveObject<CfgEnumerator>(businessAttributeQuery);
                if (businessAttribute != null)
                {
                    CfgEnumeratorValueQuery attributeValuesQuery = new CfgEnumeratorValueQuery();
                    attributeValuesQuery.EnumeratorDbid = businessAttribute.DBID;
                    var attributeValues = _configContainer.ConfServiceObject.RetrieveMultipleObjects<CfgEnumeratorValue>(attributeValuesQuery);
                    if (attributeValues != null && attributeValues.Count > 0)
                    {
                        _logger.Debug("Retrieving values from business attribute");
                        foreach (CfgEnumeratorValue enumerator in attributeValues)
                        {
                            #region Voice

                            if (enumerator != null && enumerator.Name.ToLower().Equals("voice"))
                            {
                                _logger.Debug("Retrieving values from business attribute media : " + enumerator.Name);
                                KeyValueCollection caseDataUserProperties = new KeyValueCollection();
                                if (enumerator.UserProperties.ContainsKey("case-data-add"))
                                    caseDataUserProperties = (KeyValueCollection)enumerator.UserProperties["case-data-add"];
                                List<string> loadAttachDataAddKey = new List<string>();
                                List<string> loadAttachDataFilterKey = new List<string>();
                                List<string> loadAttachDataSortKey = new List<string>();
                                if (caseDataUserProperties != null && caseDataUserProperties.Count > 0)
                                {
                                    loadAttachDataAddKey.Clear();
                                    foreach (string key in caseDataUserProperties.AllKeys)
                                    {
                                        _logger.Debug("Retrieving voice case data add values : " + key);
                                        if (string.Compare(dispKey, caseDataUserProperties[key].ToString(), true) != 0 && string.Compare(dispCollKey, caseDataUserProperties[key].ToString(), true) != 0)
                                            loadAttachDataAddKey.Add(caseDataUserProperties[key].ToString());
                                    }
                                }
                                //Adding voice case data to the container
                                if (!_configContainer.CMEValues.ContainsKey("VoiceAttachDataKey"))
                                    _configContainer.CMEValues.Add("VoiceAttachDataKey", loadAttachDataAddKey);
                                //if (!_configContainer.AllKeys.Contains("VoiceAttachDataKey"))
                                //    _configContainer.AllKeys.Add("VoiceAttachDataKey");
                                //End

                                caseDataUserProperties = new KeyValueCollection();
                                if (enumerator.UserProperties.ContainsKey("case-data-filter"))
                                    caseDataUserProperties = (KeyValueCollection)enumerator.UserProperties["case-data-filter"];

                                if (caseDataUserProperties != null && caseDataUserProperties.Count > 0)
                                {
                                    loadAttachDataFilterKey.Clear();
                                    foreach (string key in caseDataUserProperties.AllKeys)
                                    {
                                        _logger.Debug("Retrieving voice case data filter values : " + key);
                                        loadAttachDataFilterKey.Add(caseDataUserProperties[key].ToString());
                                    }
                                }
                                //Adding the voice attach data filter keys
                                if (!_configContainer.CMEValues.ContainsKey("VoiceAttachDataFilterKey"))
                                    _configContainer.CMEValues.Add("VoiceAttachDataFilterKey", loadAttachDataFilterKey);
                                //if (!_configContainer.AllKeys.Contains("VoiceAttachDataFilterKey"))
                                //    _configContainer.AllKeys.Add("VoiceAttachDataFilterKey");
                                //End

                                caseDataUserProperties = new KeyValueCollection();
                                if (enumerator.UserProperties.ContainsKey("case-data-sorting-order"))
                                    caseDataUserProperties = (KeyValueCollection)enumerator.UserProperties["case-data-sorting-order"];

                                if (caseDataUserProperties != null && caseDataUserProperties.Count > 0)
                                {
                                    loadAttachDataSortKey.Clear();
                                    foreach (string key in caseDataUserProperties.AllKeys)
                                    {
                                        _logger.Debug("Retrieving voice case data sort values : " + key);
                                        loadAttachDataSortKey.Add(caseDataUserProperties[key].ToString());
                                    }
                                }
                                //Adding the voice attach data sorting keys
                                if (!_configContainer.CMEValues.ContainsKey("VoiceAttachDataSortKey"))
                                    _configContainer.CMEValues.Add("VoiceAttachDataSortKey", loadAttachDataSortKey);
                                //if (!_configContainer.AllKeys.Contains("VoiceAttachDataSortKey"))
                                //    _configContainer.AllKeys.Add("VoiceAttachDataSortKey");
                                //End
                            }

                            #endregion Voice

                            #region Chat

                            else if (enumerator.Name.ToLower().Equals("chat"))
                            {
                                _logger.Debug("Retrieving values from business attribute media : " + enumerator.Name);
                                KeyValueCollection caseDataUserProperties = new KeyValueCollection();
                                if (enumerator.UserProperties.ContainsKey("case-data-add"))
                                    caseDataUserProperties = (KeyValueCollection)enumerator.UserProperties["case-data-add"];
                                List<string> loadAttachDataKey = new List<string>();
                                List<string> loadAttachDataFilterKey = new List<string>();
                                List<string> loadAttachDataSortKey = new List<string>();
                                if (caseDataUserProperties != null && caseDataUserProperties.Count > 0)
                                {
                                    foreach (string key in caseDataUserProperties.AllKeys)
                                    {
                                        _logger.Debug("Retrieving chat case data add values : " + key);
                                        if (string.Compare(dispKey, caseDataUserProperties[key].ToString(), true) != 0 && string.Compare(dispCollKey, caseDataUserProperties[key].ToString(), true) != 0)
                                            loadAttachDataKey.Add(caseDataUserProperties[key].ToString());
                                    }
                                }
                                //Adding chat case data to the container
                                if (!_configContainer.CMEValues.ContainsKey("ChatAttachDataKey"))
                                    _configContainer.CMEValues.Add("ChatAttachDataKey", loadAttachDataKey);
                                //if (!_configContainer.AllKeys.Contains("ChatAttachDataKey"))
                                //    _configContainer.AllKeys.Add("ChatAttachDataKey");
                                //End

                                caseDataUserProperties = new KeyValueCollection();
                                if (enumerator.UserProperties.ContainsKey("case-data-filter"))
                                    caseDataUserProperties = (KeyValueCollection)enumerator.UserProperties["case-data-filter"];

                                if (caseDataUserProperties != null && caseDataUserProperties.Count > 0)
                                {
                                    loadAttachDataFilterKey.Clear();
                                    foreach (string key in caseDataUserProperties.AllKeys)
                                    {
                                        _logger.Debug("Retrieving chat case data filter values : " + key);
                                        loadAttachDataFilterKey.Add(caseDataUserProperties[key].ToString());
                                    }
                                }
                                //Adding the chat attach data filter keys
                                if (!_configContainer.CMEValues.ContainsKey("ChatAttachDataFilterKey"))
                                    _configContainer.CMEValues.Add("ChatAttachDataFilterKey", loadAttachDataFilterKey);
                                //if (!_configContainer.AllKeys.Contains("ChatAttachDataFilterKey"))
                                //    _configContainer.AllKeys.Add("ChatAttachDataFilterKey");
                                //End

                                caseDataUserProperties = new KeyValueCollection();
                                if (enumerator.UserProperties.ContainsKey("case-data-sorting-order"))
                                    caseDataUserProperties = (KeyValueCollection)enumerator.UserProperties["case-data-sorting-order"];

                                if (caseDataUserProperties != null && caseDataUserProperties.Count > 0)
                                {
                                    loadAttachDataSortKey.Clear();
                                    foreach (string key in caseDataUserProperties.AllKeys)
                                    {
                                        _logger.Debug("Retrieving chat case data sort values : " + key);
                                        loadAttachDataSortKey.Add(caseDataUserProperties[key].ToString());
                                    }
                                }
                                //Adding the voice attach data sorting keys
                                if (!_configContainer.CMEValues.ContainsKey("ChatAttachDataSortKey"))
                                    _configContainer.CMEValues.Add("ChatAttachDataSortKey", loadAttachDataSortKey);
                                //if (!_configContainer.AllKeys.Contains("ChatAttachDataSortKey"))
                                //    _configContainer.AllKeys.Add("ChatAttachDataSortKey");
                                //End

                                //push URL
                                Dictionary<string, string> loadChatPushURL = new Dictionary<string, string>();
                                caseDataUserProperties = new KeyValueCollection();
                                if (enumerator.UserProperties.ContainsKey("push-url"))
                                    caseDataUserProperties = (KeyValueCollection)enumerator.UserProperties["push-url"];
                                if (caseDataUserProperties != null && caseDataUserProperties.Count > 0)
                                {
                                    loadChatPushURL.Clear();
                                    foreach (string key in caseDataUserProperties.AllKeys)
                                    {
                                        _logger.Debug("Retrieving chat values from chat push url : " + key);
                                        loadChatPushURL.Add(key, caseDataUserProperties[key].ToString());
                                    }
                                }
                                //Adding Push URL
                                if (!_configContainer.CMEValues.ContainsKey("ChatPushUrl"))
                                    _configContainer.CMEValues.Add("ChatPushUrl", loadChatPushURL);
                                //if (!_configContainer.AllKeys.Contains("ChatPushUrl"))
                                //    _configContainer.AllKeys.Add("ChatPushUrl");
                                //end
                            }

                            #endregion Chat

                            #region Email

                            else if (enumerator.Name.ToLower().Equals("email"))
                            {
                                _logger.Debug("Retrieving values from business attribute media : " + enumerator.Name);
                                KeyValueCollection caseDataUserProperties = new KeyValueCollection();
                                if (enumerator.UserProperties.ContainsKey("case-data-add"))
                                    caseDataUserProperties = (KeyValueCollection)enumerator.UserProperties["case-data-add"];
                                List<string> loadAttachDataKey = new List<string>();
                                List<string> loadAttachDataFilterKey = new List<string>();
                                List<string> loadAttachDataSortKey = new List<string>();

                                if (caseDataUserProperties != null && caseDataUserProperties.Count > 0)
                                {
                                    loadAttachDataKey.Clear();
                                    foreach (string key in caseDataUserProperties.AllKeys)
                                    {
                                        _logger.Debug("Retrieving email values from case data add : " + key);
                                        if (string.Compare(dispKey, caseDataUserProperties[key].ToString(), true) != 0 && string.Compare(dispCollKey, caseDataUserProperties[key].ToString(), true) != 0)
                                            loadAttachDataKey.Add(caseDataUserProperties[key].ToString());
                                    }
                                }
                                //Adding email case data to the container
                                if (!_configContainer.CMEValues.ContainsKey("EmailAttachDataKey"))
                                    _configContainer.CMEValues.Add("EmailAttachDataKey", loadAttachDataKey);
                                //if (!_configContainer.AllKeys.Contains("EmailAttachDataKey"))
                                //    _configContainer.AllKeys.Add("EmailAttachDataKey");
                                //End
                                caseDataUserProperties = new KeyValueCollection();
                                if (enumerator.UserProperties.ContainsKey("case-data-filter"))
                                    caseDataUserProperties = (KeyValueCollection)enumerator.UserProperties["case-data-filter"];

                                if (caseDataUserProperties != null && caseDataUserProperties.Count > 0)
                                {
                                    loadAttachDataFilterKey.Clear();
                                    foreach (string key in caseDataUserProperties.AllKeys)
                                    {
                                        _logger.Debug("Retrieving email values from case data filter values : " + key);
                                        loadAttachDataFilterKey.Add(caseDataUserProperties[key].ToString());
                                    }
                                }
                                //Adding the voice attach data filter keys
                                if (!_configContainer.CMEValues.ContainsKey("EmailAttachDataFilterKey"))
                                    _configContainer.CMEValues.Add("EmailAttachDataFilterKey", loadAttachDataFilterKey);
                                //if (!_configContainer.AllKeys.Contains("EmailAttachDataFilterKey"))
                                //    _configContainer.AllKeys.Add("EmailAttachDataFilterKey");
                                //End

                                caseDataUserProperties = new KeyValueCollection();
                                if (enumerator.UserProperties.ContainsKey("case-data-sorting-order"))
                                    caseDataUserProperties = (KeyValueCollection)enumerator.UserProperties["case-data-sorting-order"];

                                if (caseDataUserProperties != null && caseDataUserProperties.Count > 0)
                                {
                                    loadAttachDataSortKey.Clear();
                                    foreach (string key in caseDataUserProperties.AllKeys)
                                    {
                                        _logger.Debug("Retrieving email values from case data sort values : " + key);
                                        loadAttachDataSortKey.Add(caseDataUserProperties[key].ToString());
                                    }
                                }
                                //Adding the voice attach data sorting keys
                                if (!_configContainer.CMEValues.ContainsKey("EmailAttachDataSortKey"))
                                    _configContainer.CMEValues.Add("EmailAttachDataSortKey", loadAttachDataSortKey);
                                //if (!_configContainer.AllKeys.Contains("EmailAttachDataSortKey"))
                                //    _configContainer.AllKeys.Add("EmailAttachDataSortKey");
                                //End
                            }

                            #endregion Email

                            #region Outbound

                            else if (enumerator.Name.ToLower().Equals("outbound"))
                            {
                                _logger.Debug("Retrieving values from business attribute media : " + enumerator.Name);
                                var caseDataUserProperties = new KeyValueCollection();
                                Dictionary<string, string> loadOutboundCallResult = new Dictionary<string, string>();
                                if (enumerator.UserProperties.ContainsKey("call-result"))
                                    caseDataUserProperties = (KeyValueCollection)enumerator.UserProperties["call-result"];

                                if (caseDataUserProperties != null && caseDataUserProperties.Count > 0)
                                {
                                    loadOutboundCallResult.Clear();
                                    foreach (var key in caseDataUserProperties.AllKeys)
                                    {
                                        _logger.Debug("Retrieving voice outbound Call Results : " + key);
                                        loadOutboundCallResult.Add(key, caseDataUserProperties[key].ToString());
                                    }

                                    //Adding the voice Outbound Call Result keys and values
                                    if (!_configContainer.CMEValues.ContainsKey("OutboundCallResult"))
                                        _configContainer.CMEValues.Add("OutboundCallResult", loadOutboundCallResult);
                                    //if (!_configContainer.AllKeys.Contains("OutboundCallResult"))
                                    //    _configContainer.AllKeys.Add("OutboundCallResult");
                                    //End
                                }
                            }

                            #endregion Outbound
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error((ex.InnerException == null) ? ex.Message : ex.InnerException.ToString());
            }
            _logger.Debug("ReadCasedDataFromBusinessAttribute Exit ");
        }

        public void ReadCaseDataSortingTransactionObject(string name)
        {
            KeyValueCollection caseDataFilterUserProperties = new KeyValueCollection();
            try
            {
                List<string> loadAttachDataSortKey = new List<string>();
                _logger.Debug("Retrieving values from Case Data Sorting Transaction Object : Transaction Name : " + name);

                CfgTransaction Transaction;
                CfgTransactionQuery qTransaction = new CfgTransactionQuery();
                qTransaction.TenantDbid = _configContainer.TenantDbId;
                qTransaction.Name = name;
                Transaction = (CfgTransaction)_configContainer.ConfServiceObject.RetrieveObject<CfgTransaction>(qTransaction);

                if (Transaction == null)
                {
                    _logger.Debug("Case data sorting Transaction name : " + name + " is not found");
                    return;
                }

                caseDataFilterUserProperties = (KeyValueCollection)Transaction.UserProperties["voice.case-data-sorting-order"];
                loadAttachDataSortKey.Clear();
                if (caseDataFilterUserProperties != null && caseDataFilterUserProperties.Count > 0)
                {
                    foreach (string key in caseDataFilterUserProperties.AllKeys)
                    {
                        _logger.Debug("Voice Case data Sort : " + key);
                        loadAttachDataSortKey.Add(caseDataFilterUserProperties[key].ToString());
                    }
                }
                if (!_configContainer.CMEValues.ContainsKey("VoiceAttachDataSortKey"))
                {
                    //_configContainer.AllKeys.Add("VoiceAttachDataSortKey");
                    _configContainer.CMEValues.Add("VoiceAttachDataSortKey", loadAttachDataSortKey);
                }

                caseDataFilterUserProperties = (KeyValueCollection)Transaction.UserProperties["email.case-data-sorting-order"];
                loadAttachDataSortKey.Clear();
                if (caseDataFilterUserProperties != null && caseDataFilterUserProperties.Count > 0)
                {
                    foreach (string key in caseDataFilterUserProperties.AllKeys)
                    {
                        _logger.Debug("Email Case data Sort : " + key);
                        loadAttachDataSortKey.Add(caseDataFilterUserProperties[key].ToString());
                    }
                }
                if (!_configContainer.CMEValues.ContainsKey("EmailAttachDataSortKey"))
                {
                    //_configContainer.AllKeys.Add("EmailAttachDataSortKey");
                    _configContainer.CMEValues.Add("EmailAttachDataSortKey", loadAttachDataSortKey);
                }

                caseDataFilterUserProperties = (KeyValueCollection)Transaction.UserProperties["chat.case-data-sorting-order"];
                loadAttachDataSortKey.Clear();
                if (caseDataFilterUserProperties != null && caseDataFilterUserProperties.Count > 0)
                {
                    foreach (string key in caseDataFilterUserProperties.AllKeys)
                    {
                        _logger.Debug("Chat Case data Sort : " + key);
                        loadAttachDataSortKey.Add(caseDataFilterUserProperties[key].ToString());
                    }
                }
                if (!_configContainer.CMEValues.ContainsKey("ChatAttachDataSortKey"))
                {
                    //_configContainer.AllKeys.Add("ChatAttachDataSortKey");
                    _configContainer.CMEValues.Add("ChatAttachDataSortKey", loadAttachDataSortKey);
                }
            }
            catch (Exception commonException)
            {
                _logger.Error("Error occurred while reading CaseDataSortingTransactionObject " + name + "  =  " + commonException.ToString());
            }
        }

        public void ReadCaseDataTransactionObject(string name)
        {
            KeyValueCollection caseDataUserProperties = new KeyValueCollection();
            try
            {
                List<string> loadAttachDataKey = new List<string>();
                Dictionary<string, string> loadChatPushURL = new Dictionary<string, string>();
                _logger.Debug("Retrieving values from Case Data Transaction Object : Transaction Name : " + name);
                CfgTransaction Transaction;
                CfgTransactionQuery qTransaction = new CfgTransactionQuery();
                qTransaction.TenantDbid = _configContainer.TenantDbId;
                qTransaction.Name = name;
                Transaction = (CfgTransaction)_configContainer.ConfServiceObject.RetrieveObject<CfgTransaction>(qTransaction);

                if (Transaction == null)
                {
                    _logger.Debug("Case data Transaction name : " + name + " is not found");
                    return;
                }
                caseDataUserProperties = (KeyValueCollection)Transaction.UserProperties["voice.case-data"];
                loadAttachDataKey.Clear();
                if (caseDataUserProperties != null && caseDataUserProperties.Count > 0)
                {
                    foreach (string key in caseDataUserProperties.AllKeys)
                    {
                        _logger.Debug("Voice Case data : " + key);
                        loadAttachDataKey.Add(caseDataUserProperties[key].ToString());
                    }
                }

                if (!_configContainer.CMEValues.ContainsKey("VoiceAttachDataKey"))
                {
                    //_configContainer.AllKeys.Add("VoiceAttachDataKey");
                    _configContainer.CMEValues.Add("VoiceAttachDataKey", loadAttachDataKey);
                }

                caseDataUserProperties = (KeyValueCollection)Transaction.UserProperties["email.case-data"];
                loadAttachDataKey.Clear();
                if (caseDataUserProperties != null && caseDataUserProperties.Count > 0)
                {
                    foreach (string key in caseDataUserProperties.AllKeys)
                    {
                        _logger.Debug("Email Case data : " + key);
                        loadAttachDataKey.Add(caseDataUserProperties[key].ToString());
                    }
                }
                if (!_configContainer.CMEValues.ContainsKey("EmailAttachDataKey"))
                {
                    //_configContainer.AllKeys.Add("EmailAttachDataKey");
                    _configContainer.CMEValues.Add("EmailAttachDataKey", loadAttachDataKey);
                }

                caseDataUserProperties = (KeyValueCollection)Transaction.UserProperties["chat.case-data"];
                loadAttachDataKey.Clear();
                if (caseDataUserProperties != null && caseDataUserProperties.Count > 0)
                {
                    foreach (string key in caseDataUserProperties.AllKeys)
                    {
                        _logger.Debug("Chat Case data : " + key);
                        loadAttachDataKey.Add(caseDataUserProperties[key].ToString());
                    }
                }
                if (!_configContainer.CMEValues.ContainsKey("ChatAttachDataKey"))
                {
                    //_configContainer.AllKeys.Add("ChatAttachDataKey");
                    _configContainer.CMEValues.Add("ChatAttachDataKey", loadAttachDataKey);
                }

                caseDataUserProperties = (KeyValueCollection)Transaction.UserProperties["chat.available-url"];
                loadChatPushURL.Clear();
                if (caseDataUserProperties != null && caseDataUserProperties.Count > 0)
                {
                    foreach (string key in caseDataUserProperties.AllKeys)
                    {
                        _logger.Debug("Chat Push url : " + key);
                        loadChatPushURL.Add(key, caseDataUserProperties[key].ToString());
                    }
                }

                if (!_configContainer.CMEValues.ContainsKey("ChatPushUrl"))
                {
                    //_configContainer.AllKeys.Add("ChatPushUrl");
                    _configContainer.CMEValues.Add("ChatPushUrl", loadChatPushURL);
                }
            }
            catch (Exception commonException)
            {
                _logger.Error("Error occurred while reading CaseDataTransactionObject " + name + "  =  " + commonException.ToString());
            }
        }

        public void ReadChannelNotReadyReasonCodes()
        {
            Dictionary<string, string> voiceNotReadyReasonCodes = new Dictionary<string, string>();
            Dictionary<string, string> emailNotReadyReasonCodes = new Dictionary<string, string>();
            Dictionary<string, string> chatNotReadyReasonCodes = new Dictionary<string, string>();
            Dictionary<string, string> outboundNotReadyReasonCodes = new Dictionary<string, string>();
            try
            {
                _logger.Debug("Reading Not Ready Reason Action Codes ");
                CfgActionCodeQuery actionCodeQuery = new CfgActionCodeQuery(_configContainer.ConfServiceObject);
                actionCodeQuery.TenantDbid = _configContainer.TenantDbId;
                actionCodeQuery.CodeType = CfgActionCodeType.CFGNotReady;
                ICollection<CfgActionCode> actionCodes = _configContainer.ConfServiceObject.RetrieveMultipleObjects<CfgActionCode>(actionCodeQuery);
                if (actionCodes == null && actionCodes.Count <= 0) return;
                foreach (CfgActionCode actionCode in actionCodes)
                {
                    KeyValueCollection kvCollection = (KeyValueCollection)actionCode.UserProperties["agent.ixn.desktop"];
                    if (!actionCode.UserProperties.ContainsKey("agent.ixn.desktop"))
                    {
                        if (!voiceNotReadyReasonCodes.ContainsKey(actionCode.Name))
                            voiceNotReadyReasonCodes.Add(actionCode.Name.Trim(), actionCode.Subcodes.Count != 0 ? actionCode.Subcodes.FirstOrDefault().Code : actionCode.Code);
                        if (!emailNotReadyReasonCodes.ContainsKey(actionCode.Name))
                            emailNotReadyReasonCodes.Add(actionCode.Name, actionCode.Subcodes.Count != 0 ? actionCode.Subcodes.FirstOrDefault().Code : actionCode.Code);
                        if (!chatNotReadyReasonCodes.ContainsKey(actionCode.Name))
                            chatNotReadyReasonCodes.Add(actionCode.Name, actionCode.Subcodes.Count != 0 ? actionCode.Subcodes.FirstOrDefault().Code : actionCode.Code);
                        if (!outboundNotReadyReasonCodes.ContainsKey(actionCode.Name))
                            outboundNotReadyReasonCodes.Add(actionCode.Name, actionCode.Subcodes.Count != 0 ? actionCode.Subcodes.FirstOrDefault().Code : actionCode.Code);
                        //return;
                        continue;
                    }
                    else if (kvCollection == null || !kvCollection.ContainsKey("channels"))
                    {
                        if (!voiceNotReadyReasonCodes.ContainsKey(actionCode.Name))
                            voiceNotReadyReasonCodes.Add(actionCode.Name.Trim(), actionCode.Subcodes.Count != 0 ? actionCode.Subcodes.FirstOrDefault().Code : actionCode.Code);
                        if (!emailNotReadyReasonCodes.ContainsKey(actionCode.Name))
                            emailNotReadyReasonCodes.Add(actionCode.Name, actionCode.Subcodes.Count != 0 ? actionCode.Subcodes.FirstOrDefault().Code : actionCode.Code);
                        if (!chatNotReadyReasonCodes.ContainsKey(actionCode.Name))
                            chatNotReadyReasonCodes.Add(actionCode.Name, actionCode.Subcodes.Count != 0 ? actionCode.Subcodes.FirstOrDefault().Code : actionCode.Code);
                        if (!outboundNotReadyReasonCodes.ContainsKey(actionCode.Name))
                            outboundNotReadyReasonCodes.Add(actionCode.Name, actionCode.Subcodes.Count != 0 ? actionCode.Subcodes.FirstOrDefault().Code : actionCode.Code);
                        //return;
                        continue;
                    }
                    string[] notReadyChannels = kvCollection["channels"].ToString().Split(',');
                    if (notReadyChannels == null && notReadyChannels.Length <= 0) continue;
                    if (actionCode.Subcodes != null)
                    {
                        var subCodes = actionCode.Subcodes.OrderBy(x => x.Name);
                        if (subCodes == null) return;
                        if (notReadyChannels.Contains("voice"))
                            if (!voiceNotReadyReasonCodes.ContainsKey(actionCode.Name))
                                voiceNotReadyReasonCodes.Add(actionCode.Name, actionCode.Subcodes.Count != 0 ? subCodes.FirstOrDefault().Code : actionCode.Code);
                        if (notReadyChannels.Contains("email"))
                            if (!emailNotReadyReasonCodes.ContainsKey(actionCode.Name))
                                emailNotReadyReasonCodes.Add(actionCode.Name, actionCode.Subcodes.Count != 0 ? subCodes.FirstOrDefault().Code : actionCode.Code);
                        if (notReadyChannels.Contains("chat"))
                            if (!chatNotReadyReasonCodes.ContainsKey(actionCode.Name))
                                chatNotReadyReasonCodes.Add(actionCode.Name, actionCode.Subcodes.Count != 0 ? subCodes.FirstOrDefault().Code : actionCode.Code);
                        if (notReadyChannels.Contains("outboundpreview"))
                            if (!outboundNotReadyReasonCodes.ContainsKey(actionCode.Name))
                                outboundNotReadyReasonCodes.Add(actionCode.Name, actionCode.Subcodes.Count != 0 ? subCodes.FirstOrDefault().Code : actionCode.Code);
                    }
                    else
                    {
                        if (notReadyChannels.Contains("voice"))
                            if (!voiceNotReadyReasonCodes.ContainsKey(actionCode.Name))
                                voiceNotReadyReasonCodes.Add(actionCode.Name, actionCode.Code);
                        if (notReadyChannels.Contains("email"))
                            if (!emailNotReadyReasonCodes.ContainsKey(actionCode.Name))
                                emailNotReadyReasonCodes.Add(actionCode.Name, actionCode.Code);
                        if (notReadyChannels.Contains("chat"))
                            if (!chatNotReadyReasonCodes.ContainsKey(actionCode.Name))
                                chatNotReadyReasonCodes.Add(actionCode.Name, actionCode.Code);
                        if (notReadyChannels.Contains("outboundpreview"))
                            if (!outboundNotReadyReasonCodes.ContainsKey(actionCode.Name))
                                outboundNotReadyReasonCodes.Add(actionCode.Name, actionCode.Code);
                    }
                }
                if (!_configContainer.CMEValues.ContainsKey("VoiceNotReadyReasonCodes")
                    && voiceNotReadyReasonCodes != null)
                {
                    //_configContainer.AllKeys.Add("VoiceNotReadyReasonCodes");
                    _configContainer.CMEValues.Add("VoiceNotReadyReasonCodes", voiceNotReadyReasonCodes);
                }

                if (!_configContainer.CMEValues.ContainsKey("EmailNotReadyReasonCodes")
                    && emailNotReadyReasonCodes != null)
                {
                    //_configContainer.AllKeys.Add("EmailNotReadyReasonCodes");
                    _configContainer.CMEValues.Add("EmailNotReadyReasonCodes", emailNotReadyReasonCodes);
                }

                if (!_configContainer.CMEValues.ContainsKey("ChatNotReadyReasonCodes")
                    && chatNotReadyReasonCodes != null)
                {
                    //_configContainer.AllKeys.Add("ChatNotReadyReasonCodes");
                    _configContainer.CMEValues.Add("ChatNotReadyReasonCodes", chatNotReadyReasonCodes);
                }
                if (!_configContainer.CMEValues.ContainsKey("OutboundNotReadyReasonCodes")
                    && outboundNotReadyReasonCodes != null)
                {
                    //_configContainer.AllKeys.Add("OutboundNotReadyReasonCodes");
                    _configContainer.CMEValues.Add("OutboundNotReadyReasonCodes", outboundNotReadyReasonCodes);
                }
            }
            catch (Exception generalException)
            {
                _logger.Error((generalException.InnerException == null) ? generalException.Message : generalException.InnerException.ToString());
            }
        }

        /// <summary>
        /// Reads the disposition from business attribute.
        /// </summary>
        /// <param name="businessAttributeName">Name of the business attribute.</param>
        public void ReadDispositionFromBusinessAttribute(string businessAttributeName)
        {
            try
            {
                CfgEnumeratorQuery businessAttributeQuery = new CfgEnumeratorQuery();
                businessAttributeQuery.Name = businessAttributeName;
                businessAttributeQuery.EnumeratorType = Convert.ToInt32(CfgEnumeratorType.CFGENTInteractionOperationalAttribute);
                businessAttributeQuery.TenantDbid = _configContainer.TenantDbId;

                var businessAttribute = _configContainer.ConfServiceObject.RetrieveObject<CfgEnumerator>(businessAttributeQuery);
                if (businessAttribute != null)
                {
                    CfgEnumeratorValueQuery attributeValuesQuery = new CfgEnumeratorValueQuery();
                    attributeValuesQuery.EnumeratorDbid = businessAttribute.DBID;
                    var attributeValues = _configContainer.ConfServiceObject.RetrieveMultipleObjects<CfgEnumeratorValue>(attributeValuesQuery);
                    if (attributeValues != null && attributeValues.Count > 0)
                    {
                        voicecodes.Clear();
                        emailcodes.Clear();
                        chatcodes.Clear();
                        foreach (CfgEnumeratorValue enumerator in attributeValues)
                        {
                            if (enumerator.State == CfgObjectState.CFGEnabled)
                            {
                                _logger.Debug("Voice Disposition code : " + enumerator.Name + " Disposition value : " + enumerator.DisplayName);
                                voicecodes.Add(enumerator.Name, enumerator.DisplayName);
                            }

                            if (enumerator.IsDefault == CfgFlag.CFGTrue)
                                voicecodes.Add("DefaultDispositionCode", enumerator.Name);

                            if (enumerator.UserProperties != null && enumerator.UserProperties.ContainsKey("agent.ixn.desktop"))
                            {
                                KeyValueCollection kvCollection = (KeyValueCollection)enumerator.UserProperties["agent.ixn.desktop"];
                                if (kvCollection != null && kvCollection.Count > 0 && kvCollection.ContainsKey("channels"))
                                {
                                    if (kvCollection["channels"].ToString().ToLower().Contains("email") && enumerator.State == CfgObjectState.CFGEnabled)
                                    {
                                        emailcodes.Add(enumerator.Name, enumerator.DisplayName);
                                        _logger.Debug("Email Disposition code : " + enumerator.Name + " Disposition value : " + enumerator.DisplayName);
                                        if (enumerator.IsDefault == CfgFlag.CFGTrue)
                                            emailcodes.Add("DefaultDispositionCode", enumerator.Name);
                                    }
                                    if (kvCollection["channels"].ToString().ToLower().Contains("chat") && enumerator.State == CfgObjectState.CFGEnabled)
                                    {
                                        _logger.Debug("Chat Disposition code : " + enumerator.Name + " Disposition value : " + enumerator.DisplayName);
                                        chatcodes.Add(enumerator.Name, enumerator.DisplayName);
                                        if (enumerator.IsDefault == CfgFlag.CFGTrue)
                                            chatcodes.Add("DefaultDispositionCode", enumerator.Name);
                                    }
                                }
                                if (kvCollection != null && kvCollection.ContainsKey("voice.multi-disposition"))
                                {
                                    if (kvCollection["voice.multi-disposition"].ToString() != string.Empty)
                                    {
                                        if (!voicecodes.ContainsKey(enumerator.Name))
                                            voicecodes.Add(enumerator.Name, enumerator.DisplayName + "," + kvCollection["voice.multi-disposition"].ToString());
                                        else
                                            voicecodes[enumerator.Name] = enumerator.DisplayName + "," + kvCollection["voice.multi-disposition"].ToString();
                                        GetSubDispositionBusinessAttribute(kvCollection["voice.multi-disposition"].ToString(), "voice");
                                    }
                                }
                                if (kvCollection != null && kvCollection.ContainsKey("email.multi-disposition"))
                                {
                                    if (kvCollection["email.multi-disposition"].ToString() != string.Empty)
                                    {
                                        if (!emailcodes.ContainsKey(enumerator.Name))
                                            emailcodes.Add(enumerator.Name, enumerator.DisplayName + "," + kvCollection["email.multi-disposition"].ToString());
                                        else
                                            emailcodes[enumerator.Name] = enumerator.DisplayName + "," + kvCollection["email.multi-disposition"].ToString();
                                        GetSubDispositionBusinessAttribute(kvCollection["email.multi-disposition"].ToString(), "email");
                                    }
                                }
                                if (kvCollection != null && kvCollection.ContainsKey("chat.multi-disposition"))
                                {
                                    if (kvCollection["chat.multi-disposition"].ToString() != string.Empty)
                                    {
                                        if (!chatcodes.ContainsKey(enumerator.Name))
                                            chatcodes.Add(enumerator.Name, enumerator.DisplayName + "," + kvCollection["chat.multi-disposition"].ToString());
                                        else
                                            chatcodes[enumerator.Name] = enumerator.DisplayName + "," + kvCollection["chat.multi-disposition"].ToString();
                                        GetSubDispositionBusinessAttribute(kvCollection["chat.multi-disposition"].ToString(), "chat");
                                    }
                                }
                            }
                        }
                    }

                    //if (!voicecodes.ContainsKey("DefaultDispositionCode"))
                    //    voicecodes.Add("DefaultDispositionCode", "None");
                    //if (!emailcodes.ContainsKey("DefaultDispositionCode"))
                    //    emailcodes.Add("DefaultDispositionCode", "None");
                    //if (!chatcodes.ContainsKey("DefaultDispositionCode"))
                    //    chatcodes.Add("DefaultDispositionCode", "None");

                    var sortVoiceDict = from pair in voicecodes
                                        orderby pair.Key ascending
                                        select pair;
                    voicecodes = sortVoiceDict.ToDictionary(pair => pair.Key, pair => pair.Value);

                    var sortEmailDict = from pair in emailcodes
                                        orderby pair.Key ascending
                                        select pair;
                    emailcodes = sortEmailDict.ToDictionary(pair => pair.Key, pair => pair.Value);

                    var sortChatDict = from pair in chatcodes
                                       orderby pair.Key ascending
                                       select pair;
                    chatcodes = sortChatDict.ToDictionary(pair => pair.Key, pair => pair.Value);

                    var sortedVoiceSubDict = from pair in voiceSubDict
                                             orderby pair.Key ascending
                                             select pair;
                    voiceSubDict = sortedVoiceSubDict.ToDictionary(pair => pair.Key, pair => pair.Value);

                    var sortedEmailSubDict = from pair in emailSubDict
                                             orderby pair.Key ascending
                                             select pair;
                    emailSubDict = sortedEmailSubDict.ToDictionary(pair => pair.Key, pair => pair.Value);

                    var sortedChatSubDict = from pair in chatSubDict
                                            orderby pair.Key ascending
                                            select pair;
                    chatSubDict = sortedChatSubDict.ToDictionary(pair => pair.Key, pair => pair.Value);

                    if (!_configContainer.CMEValues.ContainsKey("voice.disposition.codes"))
                        _configContainer.CMEValues.Add("voice.disposition.codes", voicecodes);
                    //if (!_configContainer.AllKeys.Contains("voice.disposition.codes"))
                    //    _configContainer.AllKeys.Add("voice.disposition.codes");

                    if (!_configContainer.CMEValues.ContainsKey("chat.disposition.codes"))
                        _configContainer.CMEValues.Add("chat.disposition.codes", chatcodes);
                    //if (!_configContainer.AllKeys.Contains("chat.disposition.codes"))
                    //    _configContainer.AllKeys.Add("chat.disposition.codes");

                    if (!_configContainer.CMEValues.ContainsKey("email.disposition.codes"))
                        _configContainer.CMEValues.Add("email.disposition.codes", emailcodes);
                    //if (!_configContainer.AllKeys.Contains("email.disposition.codes"))
                    //    _configContainer.AllKeys.Add("email.disposition.codes");

                    if (!_configContainer.CMEValues.ContainsKey("voice.subdisposition.codes"))
                        _configContainer.CMEValues.Add("voice.subdisposition.codes", voiceSubDict);
                    //if (!_configContainer.AllKeys.Contains("voice.subdisposition.codes"))
                    //    _configContainer.AllKeys.Add("voice.subdisposition.codes");

                    if (!_configContainer.CMEValues.ContainsKey("chat.subdisposition.codes"))
                        _configContainer.CMEValues.Add("chat.subdisposition.codes", chatSubDict);
                    //if (!_configContainer.AllKeys.Contains("chat.subdisposition.codes"))
                    //    _configContainer.AllKeys.Add("chat.subdisposition.codes");

                    if (!_configContainer.CMEValues.ContainsKey("email.subdisposition.codes"))
                        _configContainer.CMEValues.Add("email.subdisposition.codes", emailSubDict);
                    //if (!_configContainer.AllKeys.Contains("email.subdisposition.codes"))
                    //    _configContainer.AllKeys.Add("email.subdisposition.codes");
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception while reading disposition from business attribute : " + ((ex.InnerException == null) ? ex.Message : ex.InnerException.ToString()));
            }
            finally
            {
                //voicecodes = null;
                //chatcodes = null;
                //emailcodes = null;
                //voiceSubDict = null;
                //chatSubDict = null;
                //emailSubDict = null;
                //dummyVoiceSubDispositions = null;
                //dummyEmailSubDispositions = null;
                //dummyChatSubDispositions = null;
            }
        }

        public void ReadDispositionTransctionObject(string name)
        {
            try
            {
                _logger.Debug("Retrieving values from Disposition Transaction Object : Transaction Name : " + name);

                CfgTransaction Transaction;
                CfgTransactionQuery qTransaction = new CfgTransactionQuery();
                qTransaction.TenantDbid = _configContainer.TenantDbId;
                qTransaction.Name = name;
                Transaction = (CfgTransaction)_configContainer.ConfServiceObject.RetrieveObject<CfgTransaction>(qTransaction);
                if (Transaction == null)
                {
                    _logger.Debug("Transaction name : " + name + " is not found");
                    return;
                }
                KeyValueCollection DispositionUserProperties = (KeyValueCollection)Transaction.UserProperties["voice.disposition-code"];
                dummyVoiceSubDispositions.Add(name);
                if (DispositionUserProperties != null && DispositionUserProperties.Count > 0)
                {
                    _logger.Debug("Reading Voice Disposition codes");
                    foreach (string key in DispositionUserProperties.AllKeys)
                    {
                        if (DispositionUserProperties[key].ToString().Contains(','))
                        {
                            string[] split = DispositionUserProperties[key].ToString().Split(',');
                            if (split[1] != string.Empty)
                            {
                                GetSubTransaction(split[1].Trim(), "voice.disposition-code");
                            }
                        }
                        voicecodes.Add(key.Trim(), DispositionUserProperties[key].ToString().Trim());
                        _logger.Debug("Disposition Code : " + key.Trim() + " Disposition Value : " + DispositionUserProperties[key].ToString().Trim());
                    }
                }
                DispositionUserProperties = (KeyValueCollection)Transaction.UserProperties["chat.disposition-code"];
                if (DispositionUserProperties != null && DispositionUserProperties.Count > 0)
                {
                    _logger.Debug("Reading Chat disposition codes");
                    foreach (string key in DispositionUserProperties.AllKeys)
                    {
                        if (DispositionUserProperties[key].ToString().Contains(','))
                        {
                            string[] split = DispositionUserProperties[key].ToString().Split(',');
                            if (split[1] != string.Empty)
                            {
                                GetSubTransaction(split[1], "chat.disposition-code");
                            }
                        }
                        chatcodes.Add(key.Trim(), DispositionUserProperties[key].ToString().Trim());
                        _logger.Debug("Disposition Code : " + key.Trim() + " Disposition Value : " + DispositionUserProperties[key].ToString().Trim());
                    }
                }
                DispositionUserProperties = (KeyValueCollection)Transaction.UserProperties["email.disposition-code"];
                if (DispositionUserProperties != null && DispositionUserProperties.Count > 0)
                {
                    _logger.Debug("Reading Email Disposition codes");
                    foreach (string key in DispositionUserProperties.AllKeys)
                    {
                        if (DispositionUserProperties[key].ToString().Contains(','))
                        {
                            string[] split = DispositionUserProperties[key].ToString().Split(',');
                            if (split[1] != string.Empty)
                            {
                                GetSubTransaction(split[1], "email.disposition-code");
                            }
                        }
                        emailcodes.Add(key.Trim(), DispositionUserProperties[key].ToString().Trim());
                        _logger.Debug("Disposition Code : " + key.Trim() + " Disposition Value : " + DispositionUserProperties[key].ToString().Trim());
                    }
                }

                if (!_configContainer.CMEValues.ContainsKey("voice.disposition.codes"))
                {
                    //_configContainer.AllKeys.Add("voice.disposition.codes");
                    _configContainer.CMEValues.Add("voice.disposition.codes", voicecodes);
                }
                if (!_configContainer.CMEValues.ContainsKey("chat.disposition.codes"))
                {
                    //_configContainer.AllKeys.Add("chat.disposition.codes");
                    _configContainer.CMEValues.Add("chat.disposition.codes", chatcodes);
                }
                if (!_configContainer.CMEValues.ContainsKey("email.disposition.codes"))
                {
                    //_configContainer.AllKeys.Add("email.disposition.codes");
                    _configContainer.CMEValues.Add("email.disposition.codes", emailcodes);
                }
                if (!_configContainer.CMEValues.ContainsKey("voice.subdisposition.codes"))
                {
                    //_configContainer.AllKeys.Add("voice.subdisposition.codes");
                    _configContainer.CMEValues.Add("voice.subdisposition.codes", voiceSubDict);
                }
                if (!_configContainer.CMEValues.ContainsKey("email.subdisposition.codes"))
                {
                    _configContainer.AllKeys.Add("email.subdisposition.codes");
                    // _configContainer.CMEValues.Add("email.subdisposition.codes", emailSubDict);
                }
                if (!_configContainer.CMEValues.ContainsKey("chat.subdisposition.codes"))
                {
                    //_configContainer.AllKeys.Add("chat.subdisposition.codes");
                    _configContainer.CMEValues.Add("chat.subdisposition.codes", chatSubDict);
                }
                //End
            }
            catch (Exception commonException)
            {
                _logger.Error("Error occurred while reading DispositionTransactionObject " + name + "  =  " + commonException.ToString());
            }
            finally
            {
                //voicecodes = null;
                //chatcodes = null;
                //emailcodes = null;
                //voiceSubDict = null;
                //chatSubDict = null;
                //emailSubDict = null;
                //dummyVoiceSubDispositions = null;
                //dummyEmailSubDispositions = null;
                //dummyChatSubDispositions = null;
            }
        }

        public void ReadGlobalNotReadyReasonCodes(string[] notReadyReason)
        {
            Dictionary<string, string> globalReasonCodes = new Dictionary<string, string>();
            try
            {
                _logger.Debug("Reading Not Ready Reason Action Codes for Global Status");
                globalReasonCodes.Clear();
                CfgActionCodeQuery actionCodeQuery = new CfgActionCodeQuery(_configContainer.ConfServiceObject);
                actionCodeQuery.TenantDbid = _configContainer.TenantDbId;
                actionCodeQuery.CodeType = CfgActionCodeType.CFGNotReady;
                ICollection<CfgActionCode> actionCodes = _configContainer.ConfServiceObject.RetrieveMultipleObjects<CfgActionCode>(actionCodeQuery);
                if (actionCodes != null && actionCodes.Count > 0)
                {
                    foreach (string code in notReadyReason)
                    {
                        var actionCode = actionCodes.FirstOrDefault(actioncode => actioncode.Name.Trim().Equals(code));
                        if (actionCode != null && actionCode.Name.Trim().Equals(code))
                        {
                            if (!globalReasonCodes.ContainsKey(actionCode.Name.Trim()))
                                globalReasonCodes.Add(actionCode.Name.Trim(), actionCode.Subcodes.Count != 0 ? actionCode.Subcodes.FirstOrDefault().Code : actionCode.Code);
                        }
                    }
                }

                if (!_configContainer.CMEValues.ContainsKey("GlobalNotReadyReasonCodes")
                    && globalReasonCodes != null)
                {
                    //_configContainer.AllKeys.Add("GlobalNotReadyReasonCodes");
                    _configContainer.CMEValues.Add("GlobalNotReadyReasonCodes", globalReasonCodes);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error occurred while reading Global Not Ready Action Codes" + ex.ToString());
            }
        }

        public void ReadLoggerData(string userName, string applicationName)
        {
            Dictionary<string, string> _loggerCollection = new Dictionary<string, string>();

            #region Application Object

            try
            {
                CfgApplicationQuery applicationQuery = new CfgApplicationQuery();
                applicationQuery.Name = applicationName;
                // applicationQuery.TenantDbid = _configContainer.TenantDbId;

                CfgApplication applicationObject = _configContainer.ConfServiceObject.RetrieveObject<CfgApplication>(applicationQuery);

                if (applicationObject != null)
                {
                    string[] applicationKeys = applicationObject.Options.AllKeys;
                    if (applicationKeys.All(s => applicationKeys.Contains("agent.ixn.desktop")))
                    {
                        KeyValueCollection systemValues = (KeyValueCollection)applicationObject.Options["agent.ixn.desktop"];
                        if (systemValues != null)
                        {
                            if (systemValues.AllKeys.Contains("log.conversion-pattern"))
                            {
                                AddToDictionary(_configContainer.CMEValues, "log.conversion-pattern", systemValues["log.conversion-pattern"].ToString(),
                                    ConfigValue.CFGValueObjects.Application);
                            }
                            if (systemValues.AllKeys.Contains("log.date-pattern"))
                            {
                                AddToDictionary(_configContainer.CMEValues, "log.date-pattern", systemValues["log.date-pattern"].ToString(),
                                   ConfigValue.CFGValueObjects.Application);
                            }
                            if (systemValues.AllKeys.Contains("log.file-name"))
                            {
                                AddToDictionary(_configContainer.CMEValues, "log.file-name", systemValues["log.file-name"].ToString(),
                                   ConfigValue.CFGValueObjects.Application);
                            }
                            if (systemValues.AllKeys.Contains("log.levels-to-filter"))
                            {
                                AddToDictionary(_configContainer.CMEValues, "log.levels-to-filter", systemValues["log.levels-to-filter"].ToString(),
                                   ConfigValue.CFGValueObjects.Application);
                            }
                            if (systemValues.AllKeys.Contains("log.max-file-size"))
                            {
                                AddToDictionary(_configContainer.CMEValues, "log.max-file-size", systemValues["log.max-file-size"].ToString(),
                                   ConfigValue.CFGValueObjects.Application);
                            }
                            if (systemValues.AllKeys.Contains("log.max-roll-size"))
                            {
                                AddToDictionary(_configContainer.CMEValues, "log.max-roll-size", systemValues["log.max-roll-size"].ToString(),
                                   ConfigValue.CFGValueObjects.Application);
                            }

                            //Implemented for multi _logger
                            if (systemValues.AllKeys.Contains("log.appender-name"))
                            {
                                AddToDictionary(_configContainer.CMEValues, "log.appender-name", systemValues["log.appender-name"].ToString(),
                                   ConfigValue.CFGValueObjects.Application);
                            }
                            if (systemValues.AllKeys.Contains("log.filter-level"))
                            {
                                AddToDictionary(_configContainer.CMEValues, "log.filter-level", systemValues["log.filter-level"].ToString(),
                                   ConfigValue.CFGValueObjects.Application);
                            }
                            if (systemValues.AllKeys.Contains("log.max-level"))
                            {
                                AddToDictionary(_configContainer.CMEValues, "log.max-level", systemValues["log.max-level"].ToString(),
                                   ConfigValue.CFGValueObjects.Application);
                            }
                            if (systemValues.AllKeys.Contains("log.min-level"))
                            {
                                AddToDictionary(_configContainer.CMEValues, "log.min-level", systemValues["log.min-level"].ToString(),
                                   ConfigValue.CFGValueObjects.Application);
                            }
                            if (systemValues.AllKeys.Contains("log.roll-style"))
                            {
                                AddToDictionary(_configContainer.CMEValues, "log.roll-style", systemValues["log.roll-style"].ToString(),
                                   ConfigValue.CFGValueObjects.Application);
                            }
                        }
                    }
                    if (applicationKeys.All(s => applicationKeys.Contains("enable-disable-channels")))
                    {
                        KeyValueCollection systemValues = (KeyValueCollection)applicationObject.Options["enable-disable-channels"];
                        if (systemValues != null)
                        {
                            if (systemValues.AllKeys.Contains("log.enable-append-file"))
                            {
                                AddToDictionary(_configContainer.CMEValues, "log.enable-append-file", systemValues["log.enable-append-file"].ToString(),
                                   ConfigValue.CFGValueObjects.Application);
                            }
                            if (systemValues.AllKeys.Contains("log.enable-print-template-fields"))
                            {
                                AddToDictionary(_configContainer.CMEValues, "log.enable-print-template-fields", systemValues["log.enable-print-template-fields"].ToString(),
                                   ConfigValue.CFGValueObjects.Application);
                            }
                            if (systemValues.AllKeys.Contains("log.enable-static-file"))
                            {
                                AddToDictionary(_configContainer.CMEValues, "log.enable-static-file", systemValues["log.enable-static-file"].ToString(),
                                   ConfigValue.CFGValueObjects.Application);
                            }
                        }
                    }
                    //End
                }
            }
            catch (Exception commonException)
            {
                _logger.Error("Error occurred while Reading _logger Data" + commonException.ToString());
            }

            #endregion Application Object

            #region Agent Object

            try
            {
                CfgPersonQuery application = new CfgPersonQuery();
                application.UserName = userName;
                application.TenantDbid = _configContainer.TenantDbId;
                _person = _configContainer.ConfServiceObject.RetrieveObject<CfgPerson>(application);
                _logger.Debug("Retrieving Values from Person : UserName : " + userName);
                //if (personenabledisableValues != null)
                {
                    string[] applicationKeys = _person.UserProperties.AllKeys;
                    if (applicationKeys.All(s => applicationKeys.Contains("agent.ixn.desktop")))
                    {
                        KeyValueCollection personValues = (KeyValueCollection)_person.UserProperties["agent.ixn.desktop"];
                        if (personValues != null)
                        {
                            if (personValues.AllKeys.Contains("log.conversion-pattern"))
                            {
                                AddToDictionary(_configContainer.CMEValues, "log.conversion-pattern", personValues["log.conversion-pattern"].ToString(),
                                   ConfigValue.CFGValueObjects.Agent);
                            }
                            if (personValues.AllKeys.Contains("log.date-pattern"))
                            {
                                AddToDictionary(_configContainer.CMEValues, "log.date-pattern", personValues["log.date-pattern"].ToString(),
                                   ConfigValue.CFGValueObjects.Agent);
                            }
                            if (personValues.AllKeys.Contains("log.file-name"))
                            {
                                AddToDictionary(_configContainer.CMEValues, "log.file-name", personValues["log.file-name"].ToString(),
                                   ConfigValue.CFGValueObjects.Agent);
                            }
                            if (personValues.AllKeys.Contains("log.levels-to-filter"))
                            {
                                AddToDictionary(_configContainer.CMEValues, "log.levels-to-filter", personValues["log.levels-to-filter"].ToString(),
                                   ConfigValue.CFGValueObjects.Agent);
                            }
                            if (personValues.AllKeys.Contains("log.max-file-size"))
                            {
                                AddToDictionary(_configContainer.CMEValues, "log.max-file-size", personValues["log.max-file-size"].ToString(),
                                   ConfigValue.CFGValueObjects.Agent);
                            }
                            if (personValues.AllKeys.Contains("log.max-roll-size"))
                            {
                                AddToDictionary(_configContainer.CMEValues, "log.max-roll-size", personValues["log.max-roll-size"].ToString(),
                                   ConfigValue.CFGValueObjects.Agent);
                            }

                            //Implemented for multi _logger
                            if (personValues.AllKeys.Contains("log.appender-name"))
                            {
                                AddToDictionary(_configContainer.CMEValues, "log.appender-name", personValues["log.appender-name"].ToString(),
                                   ConfigValue.CFGValueObjects.Agent);
                            }
                            if (personValues.AllKeys.Contains("log.filter-level"))
                            {
                                AddToDictionary(_configContainer.CMEValues, "log.filter-level", personValues["log.filter-level"].ToString(),
                                   ConfigValue.CFGValueObjects.Agent);
                            }
                            if (personValues.AllKeys.Contains("log.max-level"))
                            {
                                AddToDictionary(_configContainer.CMEValues, "log.max-level", personValues["log.max-level"].ToString(),
                                   ConfigValue.CFGValueObjects.Agent);
                            }
                            if (personValues.AllKeys.Contains("log.min-level"))
                            {
                                AddToDictionary(_configContainer.CMEValues, "log.min-level", personValues["log.min-level"].ToString(),
                                   ConfigValue.CFGValueObjects.Agent);
                            }
                            if (personValues.AllKeys.Contains("log.roll-style"))
                            {
                                AddToDictionary(_configContainer.CMEValues, "log.roll-style", personValues["log.roll-style"].ToString(),
                                   ConfigValue.CFGValueObjects.Agent);
                            }
                        }
                    }
                    if (applicationKeys.All(s => applicationKeys.Contains("enable-disable-channels")))
                    {
                        KeyValueCollection personValues = (KeyValueCollection)_person.UserProperties["enable-disable-channels"];
                        if (personValues != null)
                        {
                            if (personValues.AllKeys.Contains("log.enable-append-file"))
                            {
                                AddToDictionary(_configContainer.CMEValues, "log.enable-append-file", personValues["log.enable-append-file"].ToString(),
                                   ConfigValue.CFGValueObjects.Agent);
                            }
                            if (personValues.AllKeys.Contains("log.enable-print-template-fields"))
                            {
                                AddToDictionary(_configContainer.CMEValues, "log.enable-print-template-fields", personValues["log.enable-print-template-fields"].ToString(),
                                   ConfigValue.CFGValueObjects.Agent);
                            }
                            if (personValues.AllKeys.Contains("log.enable-static-file"))
                            {
                                AddToDictionary(_configContainer.CMEValues, "log.enable-static-file", personValues["log.enable-static-file"].ToString(),
                                   ConfigValue.CFGValueObjects.Agent);
                            }
                        }
                    }
                    //End
                }
            }
            catch (Exception commonException)
            {
                _logger.Error("Error occurred while Reading _logger Data" + commonException.ToString());
            }

            #endregion Agent Object

            #region Agent Group Object

            try
            {
                if (_person == null)
                    return;
                CfgAgentGroupQuery qAgentGroup = new CfgAgentGroupQuery();
                qAgentGroup.PersonDbid = Convert.ToInt32(_person.DBID);
                qAgentGroup.TenantDbid = _configContainer.TenantDbId;
                _logger.Debug("Retrieving Agent Group Values : Group Name : " + "agent.ixn.desktop");

                ICollection<CfgAgentGroup> AgentGroups = _configContainer.ConfServiceObject.RetrieveMultipleObjects<CfgAgentGroup>(qAgentGroup);
                if (AgentGroups != null && AgentGroups.Count > 0)
                {
                    foreach (CfgAgentGroup agentGroup in AgentGroups)
                    {
                        _logger.Info("Retrieving values from Agent Group " + agentGroup.GroupInfo.Name.ToString());
                        string[] applicationKeys = agentGroup.GroupInfo.UserProperties.AllKeys;
                        KeyValueCollection kvColl = new KeyValueCollection();

                        if (applicationKeys.All(s => applicationKeys.Contains("agent.ixn.desktop")))
                        {
                            kvColl = (KeyValueCollection)agentGroup.GroupInfo.UserProperties["agent.ixn.desktop"];
                            if (kvColl != null)
                            {
                                if (kvColl.AllKeys.Contains("log.conversion-pattern"))
                                {
                                    AddToDictionary(_configContainer.CMEValues, "log.conversion-pattern", kvColl["log.conversion-pattern"].ToString(),
                                       ConfigValue.CFGValueObjects.AgentGroup);
                                }
                                if (kvColl.AllKeys.Contains("log.date-pattern"))
                                {
                                    AddToDictionary(_configContainer.CMEValues, "log.date-pattern", kvColl["log.date-pattern"].ToString(),
                                       ConfigValue.CFGValueObjects.AgentGroup);
                                }
                                if (kvColl.AllKeys.Contains("log.file-name"))
                                {
                                    AddToDictionary(_configContainer.CMEValues, "log.file-name", kvColl["log.file-name"].ToString(),
                                       ConfigValue.CFGValueObjects.AgentGroup);
                                }
                                if (kvColl.AllKeys.Contains("log.levels-to-filter"))
                                {
                                    AddToDictionary(_configContainer.CMEValues, "log.levels-to-filter", kvColl["log.levels-to-filter"].ToString(),
                                       ConfigValue.CFGValueObjects.AgentGroup);
                                }
                                if (kvColl.AllKeys.Contains("log.max-file-size"))
                                {
                                    AddToDictionary(_configContainer.CMEValues, "log.max-file-size", kvColl["log.max-file-size"].ToString(),
                                       ConfigValue.CFGValueObjects.AgentGroup);
                                }
                                if (kvColl.AllKeys.Contains("log.max-roll-size"))
                                {
                                    AddToDictionary(_configContainer.CMEValues, "log.max-roll-size", kvColl["log.max-roll-size"].ToString(),
                                       ConfigValue.CFGValueObjects.AgentGroup);
                                }

                                //Implemented for multi _logger
                                if (kvColl.AllKeys.Contains("log.appender-name"))
                                {
                                    AddToDictionary(_configContainer.CMEValues, "log.appender-name", kvColl["log.appender-name"].ToString(),
                                       ConfigValue.CFGValueObjects.AgentGroup);
                                }
                                if (kvColl.AllKeys.Contains("log.filter-level"))
                                {
                                    AddToDictionary(_configContainer.CMEValues, "log.filter-level", kvColl["log.filter-level"].ToString(),
                                       ConfigValue.CFGValueObjects.AgentGroup);
                                }
                                if (kvColl.AllKeys.Contains("log.max-level"))
                                {
                                    AddToDictionary(_configContainer.CMEValues, "log.max-level", kvColl["log.max-level"].ToString(),
                                       ConfigValue.CFGValueObjects.AgentGroup);
                                }
                                if (kvColl.AllKeys.Contains("log.min-level"))
                                {
                                    AddToDictionary(_configContainer.CMEValues, "log.min-level", kvColl["log.min-level"].ToString(),
                                       ConfigValue.CFGValueObjects.AgentGroup);
                                }
                                if (kvColl.AllKeys.Contains("log.roll-style"))
                                {
                                    AddToDictionary(_configContainer.CMEValues, "log.roll-style", kvColl["log.roll-style"].ToString(),
                                       ConfigValue.CFGValueObjects.AgentGroup);
                                }
                            }
                        }

                        if (applicationKeys.All(s => applicationKeys.Contains("enable-disable-channels")))
                        {
                            kvColl = (KeyValueCollection)agentGroup.GroupInfo.UserProperties["enable-disable-channels"];
                            if (kvColl != null)
                            {
                                if (kvColl.AllKeys.Contains("log.enable-append-file"))
                                {
                                    AddToDictionary(_configContainer.CMEValues, "log.enable-append-file", kvColl["log.enable-append-file"].ToString(),
                                       ConfigValue.CFGValueObjects.AgentGroup);
                                }
                                if (kvColl.AllKeys.Contains("log.enable-print-template-fields"))
                                {
                                    AddToDictionary(_configContainer.CMEValues, "log.enable-print-template-fields", kvColl["log.enable-print-template-fields"].ToString(),
                                       ConfigValue.CFGValueObjects.AgentGroup);
                                }
                                if (kvColl.AllKeys.Contains("log.enable-static-file"))
                                {
                                    AddToDictionary(_configContainer.CMEValues, "log.enable-static-file", kvColl["log.enable-static-file"].ToString(),
                                       ConfigValue.CFGValueObjects.AgentGroup);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception commonException)
            {
                _logger.Error("Error occurred while Reading _logger Data" + commonException.ToString());
            }

            #endregion Agent Group Object
        }

        public void ReadPerson(string userName, string[] sectionToRead, params string[] sections)
        {
            try
            {
                CfgPersonQuery personQuery = new CfgPersonQuery();
                personQuery.UserName = userName;
                personQuery.TenantDbid = _configContainer.TenantDbId;

                CfgPerson personObject = _configContainer.ConfServiceObject.RetrieveObject<CfgPerson>(personQuery);
                if (personObject != null)
                {
                    _person = personObject;
                    _configContainer.PersonDbId = personObject.DBID;
                    _configContainer.AgentId = personObject.EmployeeID;
                    if (personObject.UserProperties != null && personObject.UserProperties.Count > 0)
                    {
                        _logger.Debug("Reading Agent annex Key values");
                        foreach (string kvCollkey in personObject.UserProperties.AllKeys)
                        {
                            if (sectionToRead == null || !sectionToRead.Contains(kvCollkey))
                                continue;
                            if (kvCollkey != "speed-dial.contacts" && (kvCollkey == "agent.ixn.desktop" || kvCollkey == "enable-disable-channels"))
                            {
                                _logger.Debug("Reading Agent : " + userName + " Section : " +
                                                personObject.UserProperties[kvCollkey]);
                                foreach (string kvp in ((KeyValueCollection)personObject.UserProperties[kvCollkey]).AllKeys)
                                {
                                    _logger.Debug("Reading Agent values Key : " + kvp + " Value : " +
                                                ((KeyValueCollection)personObject.UserProperties[kvCollkey])[kvp].ToString());

                                    AddToDictionary(_configContainer.CMEValues, kvp, ((KeyValueCollection)personObject.UserProperties[kvCollkey])[kvp].ToString(),
                                        ConfigValue.CFGValueObjects.Agent);
                                }
                            }

                            //if (kvColl != null && kvColl.Count > 0)
                            //{
                            //    foreach (string kvp in kvColl.AllKeys)
                            //    {
                            //        AddToDictionary(_configContainer.CMEValues, kvp, kvColl[kvp].ToString(), ConfigValue.CFGValueObjects.Agent);
                            //    }
                            //}
                            else if (kvCollkey == "speed-dial.contacts" && personObject.UserProperties[kvCollkey] != null &&
                                    ((KeyValueCollection)personObject.UserProperties[kvCollkey]).Count > 0)
                            {
                                Dictionary<string, string> agentContacts = new Dictionary<string, string>();

                                foreach (string kvp in ((KeyValueCollection)personObject.UserProperties[kvCollkey]).Keys)
                                    agentContacts.Add(kvp, ((KeyValueCollection)personObject.UserProperties[kvCollkey])[kvp].ToString());

                                //_configContainer.AllKeys.Add("AgentContacts");

                                if (!_configContainer.CMEValues.ContainsKey("AgentContacts"))
                                    _configContainer.CMEValues.Add("AgentContacts", agentContacts);
                                else
                                    _configContainer.CMEValues["AgentContacts"] = agentContacts;

                                _logger.Debug("Agent : " + userName + " Speed dial contacts : " + agentContacts.ToString());
                            }
                        }
                    }
                    if (!_configContainer.CMEValues.ContainsKey("CfgPerson"))
                    {
                        //_configContainer.AllKeys.Add("CfgPerson");
                        _configContainer.CMEValues.Add("CfgPerson", personObject);
                        personObject = null;
                    }
                }
            }
            catch (Exception generalException)
            {
                _logger.Error("Error occurred while reading the person : " +
                    ((generalException.InnerException == null) ? generalException.Message : generalException.InnerException.ToString()));
            }
        }

        public void ReadQueues(string queueName)
        {
            List<string> lstLoadQueues = new List<string>();
            CfgSwitch switchq = null;
            try
            {
                _logger.Debug("Retrieving values from Queues : Queue Name : " + queueName);
                ICollection<CfgAgentLoginInfo> agentInfo = (ICollection<CfgAgentLoginInfo>)_person.AgentInfo.AgentLogins;
                if (agentInfo == null || agentInfo.Count == 0)
                {
                    _logger.Warn("Agent has no login id");
                    return;
                }
                foreach (CfgAgentLoginInfo LoginDetails in agentInfo)
                {
                    switchq = LoginDetails.AgentLogin.Switch;
                    _logger.Debug(" Switch : " + LoginDetails.AgentLogin.Switch);
                }
                if (switchq != null)
                {
                    CfgDNQuery CfgQueuequery = new CfgDNQuery();
                    CfgQueuequery.TenantDbid = _configContainer.TenantDbId;
                    CfgQueuequery.DnType = GetDNType(queueName);
                    CfgQueuequery.SwitchDbid = switchq.DBID;
                    ICollection<CfgDN> cfgDNCollection = _configContainer.ConfServiceObject.RetrieveMultipleObjects<CfgDN>(CfgQueuequery);
                    if (cfgDNCollection != null)
                    {
                        if (cfgDNCollection.Count > 0)
                        {
                            foreach (CfgDN dnDetails in cfgDNCollection)
                            {
                                lstLoadQueues.Add(dnDetails.Number);
                                _logger.Debug(" DN : " + dnDetails.Number);
                            }
                        }
                        lstLoadQueues.Sort();

                        if (!_configContainer.CMEValues.ContainsKey("QueueCollection"))
                        {
                            //_configContainer.AllKeys.Add("QueueCollection");
                            _configContainer.CMEValues.Add("QueueCollection", lstLoadQueues);
                        }
                    }
                }
            }
            catch (Exception commonException)
            {
                _logger.Error("Error occurred while reading queues " + queueName + "  =  " + commonException.ToString());
            }
        }

        public OutputValues ReadSystemSection(string applicationName)
        {
            OutputValues result = null;
            result = new OutputValues();
            try
            {
                CfgApplicationQuery applicationQuery = new CfgApplicationQuery();
                applicationQuery.Name = applicationName;
                //applicationQuery.TenantDbid = WellKnownDbids.EnterpriseModeTenantDbid;

                CfgApplication application = _configContainer.ConfServiceObject.RetrieveObject<CfgApplication>(applicationQuery);

                if (application != null)
                {
                    string[] applicationKeys = application.Options.AllKeys;
                    string[] applicationUserPropertieskeys = application.UserProperties.AllKeys;
                    if (applicationKeys.Contains("_system_"))
                    {
                        _logger.Debug("Retrieving values from _system_ Section");
                        systemValuesCollection = (KeyValueCollection)application.Options["_system_"];
                        if (systemValuesCollection.ContainsKey("tenant-name"))
                        {
                            if (systemValuesCollection["tenant-name"].ToString() != string.Empty)
                            {
                                GetTenantDBIdByName(systemValuesCollection["tenant-name"].ToString());
                                result.MessageCode = "200";
                            }
                            else
                            {
                                result.MessageCode = "2001";
                                result.Message = "Tenant name is not configured.";
                                _logger.Warn("tenant-name value is not configured.");
                            }
                        }
                        else
                        {
                            GetTenantDBIdByName("");
                            result.MessageCode = "200";
                        }
                    }
                    else
                    {
                        result.MessageCode = "2001";
                        result.Message = "Mandatory configuration is missing.";
                        _logger.Warn("_system_ section is not configured");
                    }
                }
                else
                {
                    return new OutputValues() { MessageCode = "2001", Message = "Application is not found" };
                }
            }
            catch (Exception ex)
            {
                return new OutputValues() { MessageCode = "2001", Message = "The Application type does not matches. Please contact your administrator." };
            }
            return new OutputValues() { MessageCode = result.MessageCode, Message = result.Message };
        }

        public void ReadVoiceHierarchyLevelNotReadyReasonCodes(string[] notReadyReason)
        {
            Dictionary<string, string> voiceReasonCodes = new Dictionary<string, string>();
            try
            {
                _logger.Debug("Reading Not Ready Reason Action Codes for Global Status");
                voiceReasonCodes.Clear();
                CfgActionCodeQuery actionCodeQuery = new CfgActionCodeQuery(_configContainer.ConfServiceObject);
                actionCodeQuery.TenantDbid = _configContainer.TenantDbId;
                actionCodeQuery.CodeType = CfgActionCodeType.CFGNotReady;
                ICollection<CfgActionCode> actionCodes = _configContainer.ConfServiceObject.RetrieveMultipleObjects<CfgActionCode>(actionCodeQuery);
                if (actionCodes != null && actionCodes.Count > 0)
                {
                    foreach (string code in notReadyReason)
                    {
                        var actionCode = actionCodes.FirstOrDefault(actioncode => actioncode.Name.Trim().Equals(code));
                        if (actionCode != null && actionCode.Name.Trim().Equals(code))
                        {
                            if (!voiceReasonCodes.ContainsKey(actionCode.Name.Trim()))
                                voiceReasonCodes.Add(actionCode.Name.Trim(), actionCode.Subcodes.Count != 0 ? actionCode.Subcodes.FirstOrDefault().Code : actionCode.Code);
                        }
                    }
                }

                if (!_configContainer.CMEValues.ContainsKey("VoiceHierarchyLevelNotReadyReasonCodes")
                    && voiceReasonCodes != null)
                {
                    //_configContainer.AllKeys.Add("GlobalNotReadyReasonCodes");
                    _configContainer.CMEValues.Add("VoiceHierarchyLevelNotReadyReasonCodes", voiceReasonCodes);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error occurred while reading Global Not Ready Action Codes" + ex.ToString());
            }
        }

        public void RegisterCMEAlterNotification(int tenantDbid)
        {
            _configContainer.ConfServiceObject.Register(new Action<ConfEvent>(CMEObjectChanged));

            //Subscribing Application Object
            NotificationQuery notifyApplication = new NotificationQuery();
            notifyApplication.ObjectType = CfgObjectType.CFGApplication;
            notifyApplication.TenantDbid = tenantDbid;
            notifyApplication.ObjectDbid = ConfigContainer.Instance().ApplicationDbId;
            _configContainer.ConfServiceObject.Subscribe(notifyApplication);

            //Subscribing Agent Group object
            NotificationQuery notificationAgentGroup = new NotificationQuery();
            notificationAgentGroup.ObjectType = CfgObjectType.CFGAgentGroup;
            notificationAgentGroup.TenantDbid = tenantDbid;
            _configContainer.ConfServiceObject.Subscribe(notificationAgentGroup);

            //Subscribing Person Object
            NotificationQuery notificationPerson = new NotificationQuery();
            notificationPerson.ObjectType = CfgObjectType.CFGPerson;
            notificationPerson.TenantDbid = tenantDbid;
            notificationPerson.ObjectDbid = ConfigContainer.Instance().PersonDbId;
            _configContainer.ConfServiceObject.Subscribe(notificationPerson);
        }

        private void AddToDictionary(Dictionary<string, dynamic> dic, string key, string value,
            Pointel.Configuration.Manager.Helpers.ConfigValue.CFGValueObjects cfgObj)
        {
            try
            {
                configValue = new ConfigValue();
                if (!string.IsNullOrEmpty(key) && dic != null)//&& !string.IsNullOrEmpty(value)
                {
                    if (!_configContainer.AllKeys.Contains(key))
                        _configContainer.AllKeys.Add(key);

                    if (!dic.ContainsKey(key))
                    {
                        configValue.Key = key;
                        configValue.Value = value;
                        configValue.ValueObject = cfgObj;
                        dic.Add(key, configValue);
                    }
                    else
                    {
                        //Get previous values
                        ConfigValue prevConfigValue = new ConfigValue();
                        prevConfigValue = (ConfigValue)dic[key];
                        if (prevConfigValue != null && !string.IsNullOrEmpty(value.Trim()))//Additional condition added to prevent override empty data.
                        {
                            //Value to be updated from Agent Section
                            if (cfgObj == ConfigValue.CFGValueObjects.Agent)
                            {
                                dic.Remove(key);
                                prevConfigValue.Key = key;
                                prevConfigValue.Value = value;
                                prevConfigValue.ValueObject = cfgObj;
                                dic.Add(key, prevConfigValue);
                            }
                            //Value to be updated from Agent Group Section
                            else if (cfgObj == ConfigValue.CFGValueObjects.AgentGroup && prevConfigValue.ValueObject != ConfigValue.CFGValueObjects.Agent)
                            {
                                //Update the value if the existing value is not from Agent Section
                                dic.Remove(key);
                                prevConfigValue.Key = key;
                                prevConfigValue.Value = value;
                                prevConfigValue.ValueObject = cfgObj;
                                dic.Add(key, prevConfigValue);
                            }
                            //Value to be updated from Application Section
                            else if (cfgObj == ConfigValue.CFGValueObjects.Application && prevConfigValue.ValueObject == ConfigValue.CFGValueObjects.Application)
                            {
                                //Update the value if the existing value is not from Application
                                dic.Remove(key);
                                prevConfigValue.Key = key;
                                prevConfigValue.Value = value;
                                prevConfigValue.ValueObject = cfgObj;
                                dic.Add(key, prevConfigValue);
                            }
                            prevConfigValue = null;
                        }
                    }
                }
            }
            catch (Exception generalException)
            {
                _logger.Error("Error occurred while adding the KVP to dictionary : " +
                    ((generalException.InnerException == null) ? generalException.Message : generalException.InnerException.ToString()));
            }
        }

        private void GetTenantDBIdByName(string tenantName)
        {
            if (string.IsNullOrEmpty(tenantName))
            {
                _logger.Debug("Agent TenantDbid is assigned as Tenant");
                _configContainer.TenantDbId = WellKnownDbids.EnterpriseModeTenantDbid;
            }
            else
            {
                try
                {
                    CfgTenantQuery tenantQuery = new CfgTenantQuery();
                    tenantQuery.Name = tenantName;
                    tenantQuery.AllTenants = 1;
                    CfgTenant cfgTenant = _configContainer.ConfServiceObject.RetrieveObject<CfgTenant>(tenantQuery);
                    if (cfgTenant != null)
                    {
                        _configContainer.TenantDbId = cfgTenant.DBID;
                    }
                    else
                    {
                        _logger.Warn("Tenant : " + tenantName + " is not found");
                        _logger.Debug("Agent TenantDbid is assigned as Tenant");
                        //_configContainer.TenantDbId = WellKnownDbids.EnterpriseModeTenantDbid;
                    }
                }
                catch (Exception commonException)
                {
                    _logger.Error("Error occurred GetTenantDBIdByName " + tenantName + " as Tenant  :  " + commonException.ToString());
                }
            }
        }

        private object GetUpdateSpeedDialContacts(int dbId, ConfigValue.CFGValueObjects cfgObjType)
        {
            object kvColl = null;
            try
            {
                if (cfgObjType == ConfigValue.CFGValueObjects.Application)
                {
                    CfgApplicationQuery applicationQuery = new CfgApplicationQuery();
                    applicationQuery.Dbid = dbId;
                    //applicationQuery.TenantDbid = WellKnownDbids.EnterpriseModeTenantDbid;
                    _application = _configContainer.ConfServiceObject.RetrieveObject<CfgApplication>(applicationQuery);
                    if (_application != null)
                    {
                        if (_application.Options != null && _application.Options.ContainsKey("speed-dial.contacts"))
                            kvColl = (KeyValueCollection)_application.Options["speed-dial.contacts"];
                        return kvColl;
                    }
                }
                else if (cfgObjType == ConfigValue.CFGValueObjects.AgentGroup)
                {
                    CfgAgentGroupQuery agentGroupQuery = new CfgAgentGroupQuery();
                    agentGroupQuery.Dbid = dbId;
                    agentGroupQuery.TenantDbid = _configContainer.TenantDbId;
                    CfgAgentGroup _agentGroup = _configContainer.ConfServiceObject.RetrieveObject<CfgAgentGroup>(agentGroupQuery);
                    if (_agentGroup != null)
                    {
                        if (_agentGroup.GroupInfo.UserProperties != null && _agentGroup.GroupInfo.UserProperties.ContainsKey("speed-dial.contacts"))
                            kvColl = (KeyValueCollection)_agentGroup.GroupInfo.UserProperties["speed-dial.contacts"];
                        return kvColl;
                    }
                }
                else if (cfgObjType == ConfigValue.CFGValueObjects.Agent)
                {
                    CfgPersonQuery personQuery = new CfgPersonQuery();
                    personQuery.Dbid = dbId;
                    personQuery.TenantDbid = _configContainer.TenantDbId;
                    CfgPerson person = _configContainer.ConfServiceObject.RetrieveObject<CfgPerson>(personQuery);
                    if (person != null)
                    {
                        if (person.UserProperties != null && person.UserProperties.ContainsKey("speed-dial.contacts"))
                            kvColl = (KeyValueCollection)person.UserProperties["speed-dial.contacts"];
                        if (kvColl != null)
                        {
                            Dictionary<string, string> agentContacts = new Dictionary<string, string>();
                            foreach (string kvp in ((KeyValueCollection)kvColl).Keys)
                                agentContacts.Add(kvp, ((KeyValueCollection)kvColl)[kvp].ToString());
                            return agentContacts;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error((ex.InnerException == null) ? ex.Message : ex.InnerException.ToString());
            }
            return kvColl;
        }

        private void UpdateCMEObject(XDocument xmlData, ConfigValue.CFGValueObjects cfgObjType, int objId)
        {
        }

        #endregion Methods

        #region Other

        //private static CfgDelta CreateDelta(XDocument confObj, Endpoint ep)
        //{
        //    if (confObj == null || ep == null || confObj.Root == null || confObj.Root.FirstNode == null)
        //    {
        //        throw new ArgumentOutOfRangeException("confObj");
        //    }
        //    string localName = confObj.Root.Elements().First<XElement>().Name.LocalName;
        //    CfgDelta result = null;
        //    IConfService confService = ConfServiceFactory.RetrieveConfService(ep);
        //    if (confService != null)
        //    {
        //        result = (CfgDelta)Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.CfgObjectActivator.CreateInstance(localName, confService, confObj.Root.Elements().First<XElement>(), null);
        //    }
        //    return result;
        //}
        //internal static int GetDbidFromDelta(XDocument deltaDoc)
        //{
        //    XPathNavigator xPathNavigator = deltaDoc.CreateNavigator();
        //    XmlNamespaceManager xmlNamespaceManager = new XmlNamespaceManager(xPathNavigator.NameTable);
        //    xmlNamespaceManager.AddNamespace("ns", deltaDoc.Root.Name.NamespaceName);
        //    XElement xElement = deltaDoc.XPathSelectElement("//ns:DBID", xmlNamespaceManager);
        //    if (xElement == null)
        //    {
        //        return -1;
        //    }
        //    return int.Parse(xElement.Attribute("value").Value);
        //}
        //internal static string GetNamespace(IProtocol protocol)
        //{
        //    Genesyslab.Platform.Configuration.Protocols.ConfServerProtocol confServerProtocol = protocol as Genesyslab.Platform.Configuration.Protocols.ConfServerProtocol;
        //    if (confServerProtocol == null || !confServerProtocol.UseConfDataNs)
        //    {
        //        return "";
        //    }
        //    return confServerProtocol.ProtocolDescription.NS;
        //}
        //public static string GetName(CfgObjectType type)
        //{
        //    switch (type)
        //    {
        //        case CfgObjectType.CFGSwitch:
        //            return "CfgSwitch";
        //        case CfgObjectType.CFGDN:
        //            return "CfgDN";
        //        case CfgObjectType.CFGPerson:
        //            return "CfgPerson";
        //        case CfgObjectType.CFGPlace:
        //            return "CfgPlace";
        //        case CfgObjectType.CFGAgentGroup:
        //            return "CfgAgentGroup";
        //        case CfgObjectType.CFGPlaceGroup:
        //            return "CfgPlaceGroup";
        //        case CfgObjectType.CFGTenant:
        //            return "CfgTenant";
        //        case CfgObjectType.CFGService:
        //            return "CfgService";
        //        case CfgObjectType.CFGApplication:
        //            return "CfgApplication";
        //        case CfgObjectType.CFGHost:
        //            return "CfgHost";
        //        case CfgObjectType.CFGPhysicalSwitch:
        //            return "CfgPhysicalSwitch";
        //        case CfgObjectType.CFGScript:
        //            return "CfgScript";
        //        case CfgObjectType.CFGSkill:
        //            return "CfgSkill";
        //        case CfgObjectType.CFGActionCode:
        //            return "CfgSwitch";
        //        case CfgObjectType.CFGAgentLogin:
        //            return "CfgAgentLogin";
        //        case CfgObjectType.CFGTransaction:
        //            return "CfgTransaction";
        //        case CfgObjectType.CFGDNGroup:
        //            return "CfgDNGroup";
        //        case CfgObjectType.CFGStatDay:
        //            return "CfgStatDay";
        //        case CfgObjectType.CFGStatTable:
        //            return "CfgStatTable";
        //        case CfgObjectType.CFGAppPrototype:
        //            return "CfgAppPrototype";
        //        case CfgObjectType.CFGAccessGroup:
        //            return "CfgAccessGroup";
        //        case CfgObjectType.CFGFolder:
        //            return "CfgFolder";
        //        case CfgObjectType.CFGField:
        //            return "CfgField";
        //        case CfgObjectType.CFGFormat:
        //            return "CfgFormat";
        //        case CfgObjectType.CFGTableAccess:
        //            return "CfgTableAccess";
        //        case CfgObjectType.CFGCallingList:
        //            return "CfgCallingList";
        //        case CfgObjectType.CFGCampaign:
        //            return "CfgCampaign";
        //        case CfgObjectType.CFGTreatment:
        //            return "CfgTreatment";
        //        case CfgObjectType.CFGFilter:
        //            return "CfgFilter";
        //        case CfgObjectType.CFGTimeZone:
        //            return "CfgTimeZone";
        //        case CfgObjectType.CFGVoicePrompt:
        //            return "CfgVoicePrompt";
        //        case CfgObjectType.CFGIVRPort:
        //            return "CfgIVRPort";
        //        case CfgObjectType.CFGIVR:
        //            return "CfgIVR";
        //        case CfgObjectType.CFGAlarmCondition:
        //            return "CfgAlarmCondition";
        //        case CfgObjectType.CFGEnumerator:
        //            return "CfgEnumerator";
        //        case CfgObjectType.CFGEnumeratorValue:
        //            return "CfgEnumeratorValue";
        //        case CfgObjectType.CFGObjectiveTable:
        //            return "CfgObjectiveTable";
        //        case CfgObjectType.CFGCampaignGroup:
        //            return "CfgCampaignGroup";
        //        case CfgObjectType.CFGGVPReseller:
        //            return "CfgGVPReseller";
        //        case CfgObjectType.CFGGVPCustomer:
        //            return "CfgGVPCustomer";
        //        case CfgObjectType.CFGGVPIVRProfile:
        //            return "CfgGVPIVRProfile";
        //        case CfgObjectType.CFGScheduledTask:
        //            return "CfgScheduledTask";
        //        case CfgObjectType.CFGRole:
        //            return "CfgRole";
        //        case CfgObjectType.CFGPersonLastLogin:
        //            return "CfgPersonLastLogin";
        //        default:
        //            return null;
        //    }
        //}

        #endregion Other
    }
}