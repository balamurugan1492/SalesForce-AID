#region Header

/*
* =====================================
* Pointel.Interactions.TeamCommunicator.Helpers
* ====================================
* Project    : Agent Interaction Desktop
* Created on : 05-Sep-2014
* Author     : Manikandan
* Owner      : Pointel Solutions
* ====================================
*/

#endregion Header

namespace Pointel.Interactions.TeamCommunicator.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Windows.Threading;

    using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel;
    using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.CfgObjects;
    using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.Queries;
    using Genesyslab.Platform.Commons.Collections;
    using Genesyslab.Platform.Configuration.Protocols.Types;
    using Genesyslab.Platform.Contacts.Protocols.ContactServer;
    using Genesyslab.Platform.Contacts.Protocols.ContactServer.Events;

    using Lucene.Net.Documents;
    using Lucene.Net.Index;
    using Lucene.Net.Store;

    using Pointel.Configuration.Manager;
    using Pointel.Interactions.TeamCommunicator.Settings;

    public class ConfigParser
    {
        #region Fields

        private Pointel.Logger.Core.ILog _logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
                    "AID");

        #endregion Fields

        #region Methods

        /// <summary>
        /// Contacts the updation.
        /// </summary>
        /// <param name="operationType">Type of the operation.</param>
        /// <param name="contactId">The contact unique identifier.</param>
        /// <param name="attributesList">The attributes list.</param>
        public void ContactUpdation(string operationType, string contactId, Genesyslab.Platform.Contacts.Protocols.ContactServer.ContactAttributeList attributesList)
        {
            if (operationType == "Add")
            {
                Dictionary<string, string> record = AddUCSContactRecord(attributesList, contactId, "", contactId, "", DateTime.Now.ToString(), DateTime.Now.ToString(),
                    "", "", "", "");
                if (record != null)
                    AddUCSContactRecord(record);
                record = null;
            }
            else if (operationType == "Update" || operationType == "Delete")
            {
                IndexReader reader = IndexReader.Open(Datacontext.GetInstance().Directory);
                reader.DeleteDocuments(new Lucene.Net.Index.Term("ContactId", contactId));
                reader.Close();
                if (operationType == "Update")
                {
                    Dictionary<string, string> record = AddUCSContactRecord(attributesList, contactId, "", contactId, "", DateTime.Now.ToString(),
                               DateTime.Now.ToString(), "", "", "", "");
                    if (record != null)
                        AddUCSContactRecord(record);
                }
            }
        }

        /// <summary>
        /// Loads the initial data.
        /// </summary>
        public void LoadInitialData()
        {
            try
            {
                Datacontext.GetInstance().Directory = FSDirectory.Open(new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData).ToString()
                    + @"\Pointel\temp\" + Datacontext.GetInstance().UserName));
                Datacontext.GetInstance().Writer = new IndexWriter(Datacontext.GetInstance().Directory, Datacontext.GetInstance().Analyzer, true);
                Datacontext.GetInstance().Writer.SetMergeScheduler(new SerialMergeScheduler());

                if (ConfigContainer.Instance().ConfServiceObject != null &&
                        ((ConfService)ConfigContainer.Instance().ConfServiceObject).Protocol.State == Genesyslab.Platform.Commons.Protocols.ChannelState.Opened)
                {
                    Datacontext.GetInstance().hshAgentEmployeeIdUserName.Clear();

                    if (ConfigContainer.Instance().AllKeys.Contains("teamcommunicator.enable-config-agent-groups") &&
                    ((string)ConfigContainer.Instance().GetValue("teamcommunicator.enable-config-agent-groups")).ToLower().Equals("true") &&
                    ConfigContainer.Instance().AllKeys.Contains("teamcommunicator.consult-agent-groups") &&
                    ((string)ConfigContainer.Instance().GetValue("teamcommunicator.consult-agent-groups")).ToString() != string.Empty)
                    {
                        Thread thAgentGroup = new Thread(delegate()
                            {
                                string[] agentGroups = ((string)ConfigContainer.Instance().GetValue("teamcommunicator.consult-agent-groups")).Split(',');
                                if (agentGroups != null && agentGroups.Length > 0)
                                {
                                    foreach (string groupName in agentGroups)
                                    {
                                        //bool isretreived = false;
                                        //while (!isretreived)
                                        //{
                                        try
                                        {
                                            CfgAgentGroupQuery qAgentGroup = new CfgAgentGroupQuery();
                                            qAgentGroup.TenantDbid = ConfigContainer.Instance().TenantDbId;
                                            qAgentGroup.Name = groupName;
                                            CfgAgentGroup agentGroup = ConfigContainer.Instance().ConfServiceObject.RetrieveObject<CfgAgentGroup>(qAgentGroup);
                                            if (agentGroup != null && agentGroup.Agents != null)
                                            {
                                                //isretreived = true;
                                                _logger.Info("Agent Group : " + groupName + " is available.");

                                                _logger.Info("Agent Group : " + groupName + " has " + agentGroup.Agents.Count + "  agents.");

                                                foreach (CfgPerson person in agentGroup.Agents)
                                                {
                                                    if (person != null && person.UserName != Datacontext.GetInstance().UserName)
                                                    {
                                                        Datacontext.GetInstance().hshAgentEmployeeIdUserName.Add(person.EmployeeID, person.UserName);
                                                        Dictionary<string, string> record = AddCfgPersonRecord(person, person.ObjectType.ToString(), person.DBID.ToString(), "", "",
                                                            DateTime.Now.ToString(), "", person.State.ToString(), person.ObjectPath, "");
                                                        if (record != null)
                                                            AddAgentObject(record);
                                                    }
                                                }
                                                Datacontext.GetInstance().Writer.Commit();
                                            }
                                            else
                                                _logger.Warn("No Agent group is available with name : " + groupName);
                                        }
                                        catch { }
                                        //}
                                    }
                                }
                            });
                        thAgentGroup.Start();
                    }
                    else
                    {
                        _logger.Info("teamcommunicator.consult-agent-groups is not configured");
                        if (Datacontext.GetInstance().AllPersons != null && Datacontext.GetInstance().AllPersons.Count > 0)
                        {
                            foreach (CfgPerson person in Datacontext.GetInstance().AllPersons)
                            {
                                if (person.UserName != Datacontext.GetInstance().UserName)
                                {
                                    Datacontext.GetInstance().hshAgentEmployeeIdUserName.Add(person.EmployeeID, person.UserName);
                                    Dictionary<string, string> record = AddCfgPersonRecord(person, person.ObjectType.ToString(), person.DBID.ToString(), "", "",
                                        DateTime.Now.ToString(), "", person.State.ToString(), person.ObjectPath, "");
                                    if (record != null)
                                        AddAgentObject(record);
                                    record = null;
                                }
                            }
                            Datacontext.GetInstance().Writer.Commit();
                        }
                    }
                }

                if (ConfigContainer.Instance().ConfServiceObject != null &&
                        ((ConfService)ConfigContainer.Instance().ConfServiceObject).Protocol.State == Genesyslab.Platform.Commons.Protocols.ChannelState.Opened)
                {
                    Thread thAgentGroup = new Thread(delegate()
                        {
                            bool isretreived = false;
                            while (!isretreived)
                            {
                                try
                                {
                                    CfgAgentGroupQuery _agentGroupQuery = new CfgAgentGroupQuery();
                                    _agentGroupQuery.TenantDbid = ConfigContainer.Instance().TenantDbId;
                                    ICollection<CfgAgentGroup> _agentGroupCollection = ((ConfService)ConfigContainer.Instance().ConfServiceObject).RetrieveMultipleObjects<CfgAgentGroup>(_agentGroupQuery);
                                    if (_agentGroupCollection != null && _agentGroupCollection.Count > 0)
                                    {
                                        isretreived = true;
                                        foreach (CfgAgentGroup aGroup in _agentGroupCollection)
                                        {
                                            Dictionary<string, string> record = AddCfgAgentGroupRecord(aGroup, aGroup.ObjectType.ToString(), aGroup.DBID.ToString(), "",
                                                "", DateTime.Now.ToString(), "", aGroup.GroupInfo.State.ToString(), aGroup.ObjectPath, "");
                                            if (record != null)
                                                AddAgentGroupObject(record);
                                            record = null;
                                        }
                                        Datacontext.GetInstance().Writer.Commit();
                                    }
                                    _agentGroupCollection = null;
                                }
                                catch { }
                            }
                        });
                    thAgentGroup.Start();
                }
                if (ConfigContainer.Instance().ConfServiceObject != null &&
                        ((ConfService)ConfigContainer.Instance().ConfServiceObject).Protocol.State == Genesyslab.Platform.Commons.Protocols.ChannelState.Opened)
                {
                    Thread thAgentGroup = new Thread(delegate()
                        {
                            bool isretreived = false;
                            while (!isretreived)
                            {
                                try
                                {
                                    CfgSkillQuery _skillQuery = new CfgSkillQuery();
                                    _skillQuery.TenantDbid = ConfigContainer.Instance().TenantDbId;
                                    ICollection<CfgSkill> _skillCollection = ((ConfService)ConfigContainer.Instance().ConfServiceObject).RetrieveMultipleObjects<CfgSkill>(_skillQuery);
                                    if (_skillCollection != null && _skillCollection.Count > 0)
                                    {
                                        isretreived = true;
                                        foreach (CfgSkill skill in _skillCollection)
                                        {
                                            Dictionary<string, string> record = AddCfgSkillRecord(skill, skill.ObjectType.ToString(), skill.DBID.ToString(), "",
                                                "", DateTime.Now.ToString(), "", "", skill.ObjectPath, "");
                                            if (record != null)
                                                AddConfigSkillRecord(record);
                                            record = null;
                                        }
                                        Datacontext.GetInstance().Writer.Commit();
                                    }
                                    _skillCollection = null;
                                }
                                catch { }
                            }
                        });
                    thAgentGroup.Start();
                }
                if (ConfigContainer.Instance().ConfServiceObject != null &&
                        ((ConfService)ConfigContainer.Instance().ConfServiceObject).Protocol.State == Genesyslab.Platform.Commons.Protocols.ChannelState.Opened)
                {
                    Thread thAgentGroup = new Thread(delegate()
                       {
                           bool isretreived = false;
                           while (!isretreived)
                           {
                               try
                               {
                                   CfgScriptQuery qScripts = new CfgScriptQuery();
                                   qScripts.ScriptType = Genesyslab.Platform.Configuration.Protocols.Types.CfgScriptType.CFGInteractionQueue;
                                   qScripts.TenantDbid = ConfigContainer.Instance().TenantDbId;
                                   ICollection<CfgScript> _scriptCollection = ((ConfService)ConfigContainer.Instance().ConfServiceObject).RetrieveMultipleObjects<CfgScript>(qScripts);
                                   if (_scriptCollection != null && _scriptCollection.Count > 0)
                                   {
                                       isretreived = true;
                                       foreach (CfgScript script in _scriptCollection)
                                       {
                                           Dictionary<string, string> record = AddCfgInteractionQueueRecord(script, script.ObjectType.ToString(), script.DBID.ToString(), "", "", "",
                                                DateTime.Now.ToString(), "", script.State.ToString(), script.ObjectPath, "");
                                           if (record != null)
                                               AddConfigInteractionQueueRecord(record);
                                           record = null;
                                       }
                                       Datacontext.GetInstance().Writer.Commit();
                                   }
                                   _scriptCollection = null;
                               }
                               catch { }
                           }
                       });
                    thAgentGroup.Start();
                }

                if (ConfigContainer.Instance().ConfServiceObject != null &&
                       ((ConfService)ConfigContainer.Instance().ConfServiceObject).Protocol.State == Genesyslab.Platform.Commons.Protocols.ChannelState.Opened)
                {
                    Thread thAgentGroup = new Thread(delegate()
                       {
                           bool isretreived = false;
                           while (!isretreived)
                           {
                               try
                               {
                                   CfgDNQuery acdQueueQuery = new CfgDNQuery();
                                   acdQueueQuery.DnType = Genesyslab.Platform.Configuration.Protocols.Types.CfgDNType.CFGACDQueue;
                                   acdQueueQuery.TenantDbid = ConfigContainer.Instance().TenantDbId;
                                   ICollection<CfgDN> _acdQueueCollection = ((ConfService)ConfigContainer.Instance().ConfServiceObject).RetrieveMultipleObjects<CfgDN>(acdQueueQuery);
                                   if (_acdQueueCollection != null && _acdQueueCollection.Count > 0)
                                   {
                                       isretreived = true;
                                       foreach (CfgDN acdQueue in _acdQueueCollection)
                                       {
                                           Dictionary<string, string> record = AddCfgACDQueueRecord(acdQueue, acdQueue.ObjectType.ToString(), acdQueue.DBID.ToString(), "", "", "", "",
                                               DateTime.Now.ToString(), acdQueue.ObjectPath, acdQueue.State.ToString());
                                           if (record != null)
                                               AddConfigQueueRecord(record);
                                           record = null;
                                       }
                                       Datacontext.GetInstance().Writer.Commit();
                                   }
                                   _acdQueueCollection = null;
                               }
                               catch { }
                           }
                       });
                    thAgentGroup.Start();
                }

                if (ConfigContainer.Instance().ConfServiceObject != null &&
                       ((ConfService)ConfigContainer.Instance().ConfServiceObject).Protocol.State == Genesyslab.Platform.Commons.Protocols.ChannelState.Opened)
                {
                    Thread thAgentGroup = new Thread(delegate()
                       {
                           bool isretreived = false;
                           while (!isretreived)
                           {
                               try
                               {
                                   CfgDNQuery routingDnQuery = new CfgDNQuery();
                                   routingDnQuery.DnType = Genesyslab.Platform.Configuration.Protocols.Types.CfgDNType.CFGRoutingPoint;
                                   routingDnQuery.TenantDbid = ConfigContainer.Instance().TenantDbId;
                                   ICollection<CfgDN> _routingDnCollection = ((ConfService)ConfigContainer.Instance().ConfServiceObject).RetrieveMultipleObjects<CfgDN>(routingDnQuery);
                                   if (_routingDnCollection != null && _routingDnCollection.Count > 0)
                                   {
                                       isretreived = true;
                                       foreach (CfgDN routingDN in _routingDnCollection)
                                       {
                                           Dictionary<string, string> record = AddCfgDNRecord(routingDN, routingDN.ObjectType.ToString(), routingDN.DBID.ToString(), "", "", "", "",
                                               DateTime.Now.ToString(), routingDN.ObjectPath, routingDN.State.ToString());
                                           if (record != null)
                                               AddConfigDNRecord(record);
                                           record = null;
                                       }
                                       Datacontext.GetInstance().Writer.Commit();
                                   }
                                   _routingDnCollection = null;
                               }
                               catch { }
                           }
                       });
                    thAgentGroup.Start();
                }

                //Add DN's based on call control
                if (ConfigContainer.Instance().ConfServiceObject != null &&
                       ((ConfService)ConfigContainer.Instance().ConfServiceObject).Protocol.State == Genesyslab.Platform.Commons.Protocols.ChannelState.Opened)
                {
                    Thread thAgentGroup = new Thread(delegate()
                       {
                           bool isdnretreived = false;
                           while (!isdnretreived)
                           {
                               try
                               {
                                   CfgDNQuery _dnQuery = new CfgDNQuery();
                                   if (ConfigContainer.Instance().AllKeys.Contains("call-control"))
                                   {
                                       if (((string)ConfigContainer.Instance().GetValue("call-control")).ToLower().Equals("acd"))
                                           _dnQuery.DnType = Genesyslab.Platform.Configuration.Protocols.Types.CfgDNType.CFGACDPosition;
                                       else
                                           _dnQuery.DnType = Genesyslab.Platform.Configuration.Protocols.Types.CfgDNType.CFGExtension;
                                   }
                                   else
                                       _dnQuery.DnType = Genesyslab.Platform.Configuration.Protocols.Types.CfgDNType.CFGExtension;

                                   _dnQuery.TenantDbid = ConfigContainer.Instance().TenantDbId;
                                   ICollection<CfgDN> _dnCollection = ((ConfService)ConfigContainer.Instance().ConfServiceObject).RetrieveMultipleObjects<CfgDN>(_dnQuery);
                                   if (_dnCollection != null && _dnCollection.Count > 0)
                                   {
                                       isdnretreived = true;
                                       foreach (CfgDN dN in _dnCollection)
                                       {
                                           Dictionary<string, string> record = AddCfgDNRecord(dN, dN.ObjectType.ToString(), dN.DBID.ToString(), "", "", "", "",
                                                DateTime.Now.ToString(), dN.ObjectPath, dN.State.ToString());
                                           if (record != null)
                                               AddConfigDNRecord(record);
                                           record = null;
                                       }
                                       Datacontext.GetInstance().Writer.Commit();
                                   }
                                   _dnCollection = null;

                               }
                               catch { }
                           }
                       });
                    thAgentGroup.Start();
                }

                //Add all UCS Contacts
                if (Datacontext.GetInstance().ContactsIMessage != null)
                {
                    EventGetContacts eventGetContact = Datacontext.GetInstance().ContactsIMessage as EventGetContacts;
                    if (eventGetContact != null)
                    {
                        for (int index = 0; index < eventGetContact.ContactData.Count; index++)
                        {
                            ContactAttributeList contactAttributesList = eventGetContact.ContactData.Get(index).ContactAttributesList;

                            Dictionary<string, string> record = AddUCSContactRecord(contactAttributesList, eventGetContact.ContactData.Get(index).Id, "",
                                eventGetContact.ContactData.Get(index).Id, "", DateTime.Now.ToString(),
                                DateTime.Now.ToString(), "", "", "", "");
                            if (record != null)
                                AddUCSContactRecord(record);
                            record = null;
                        }
                        Datacontext.GetInstance().Writer.Commit();
                    }
                    eventGetContact = null;
                }

                //Datacontext.GetInstance().Writer.Commit();
            }
            catch (Exception ex)
            {
                _logger.Error((ex.InnerException == null) ? ex.Message : ex.InnerException.ToString());
            }
        }

        /// <summary>
        /// Adds the agent group object.
        /// </summary>
        /// <param name="record">The record.</param>
        private void AddAgentGroupObject(Dictionary<string, string> record)
        {
            Document document = new Document();
            try
            {
                for (int i = 0; i < Datacontext.GetInstance().ConfigAgentGroupAttributes.Length; i++)
                {
                    string str = "";
                    if (record.TryGetValue(Datacontext.GetInstance().ConfigAgentGroupAttributes[i], out str) && (str != null))
                    {
                        document.Add(new Field(Datacontext.GetInstance().ConfigAgentGroupAttributes[i], str, Field.Store.YES, Field.Index.ANALYZED));
                    }
                }
                Datacontext.GetInstance().Writer.AddDocument(document);
            }
            catch (Exception ex)
            {
                _logger.Error((ex.InnerException == null) ? ex.Message : ex.InnerException.ToString());
            }
            finally
            {
                document = null;
            }
        }

        /// <summary>
        /// Adds the agent object.
        /// </summary>
        /// <param name="record">The record.</param>
        private void AddAgentObject(Dictionary<string, string> record)
        {
            Document document = new Document();
            try
            {
                for (int i = 0; i < Datacontext.GetInstance().ConfigAgentAttributes.Length; i++)
                {
                    string str = "";
                    if (record.TryGetValue(Datacontext.GetInstance().ConfigAgentAttributes[i], out str) && (str != null))
                    {
                        document.Add(new Field(Datacontext.GetInstance().ConfigAgentAttributes[i], str, Field.Store.YES, Field.Index.ANALYZED));
                    }
                }
                Datacontext.GetInstance().Writer.AddDocument(document);
            }
            catch (Exception ex)
            {
                _logger.Error((ex.InnerException == null) ? ex.Message : ex.InnerException.ToString());
            }
            finally
            {
                document = null;
            }
        }

        /// <summary>
        /// Adds the CFG acd queue record.
        /// </summary>
        /// <param name="cfgDN">The CFG dn.</param>
        /// <param name="LevelType">Type of the level.</param>
        /// <param name="LevelDbid">The level dbid.</param>
        /// <param name="FavoriteCategory">The favorite category.</param>
        /// <param name="RecentDateTime">The recent date time.</param>
        /// <param name="recentMediaStartDate">The recent media start date.</param>
        /// <param name="recentMediaType">Type of the recent media.</param>
        /// <param name="recentMediaState">State of the recent media.</param>
        /// <param name="recentMediaDirection">The recent media direction.</param>
        /// <param name="recentMediaStatus">The recent media status.</param>
        /// <returns></returns>
        private Dictionary<string, string> AddCfgACDQueueRecord(CfgDN cfgDN, string LevelType, string LevelDbid, string FavoriteCategory, string RecentDateTime,
            string recentMediaStartDate, string recentMediaType, string recentMediaState, string recentMediaDirection, string recentMediaStatus)
        {
            if (cfgDN == null)
                return null;
            Dictionary<string, string> record = new Dictionary<string, string>();
            record.Add("ContactType", Datacontext.SelectorFilters.Queue.ToString());
            record.Add("DBID", cfgDN.DBID.ToString(CultureInfo.InvariantCulture));
            if (cfgDN.Number != null)
            {
                record.Add("Number", cfgDN.Number);
            }
            else
            {
                record.Add("Number", "");
            }
            if (cfgDN.Name != null)
            {
                record.Add("Name", cfgDN.Name);
            }
            else
            {
                record.Add("Name", "");
            }
            if (LevelType != null)
            {
                record.Add("LevelType", LevelType);
            }
            else
            {
                record.Add("LevelType", "");
            }
            if (LevelDbid != null)
            {
                record.Add("LevelDBID", LevelDbid);
            }
            else
            {
                record.Add("LevelDBID", "");
            }
            if (FavoriteCategory != null)
            {
                record.Add("Category", FavoriteCategory);
            }
            else
            {
                record.Add("Category", "");
            }
            if (RecentDateTime != null)
            {
                record.Add("DateTime", RecentDateTime);
            }
            else
            {
                record.Add("DateTime", "");
            }
            if (recentMediaStartDate != null)
            {
                record.Add("RecentMediaStartDate", recentMediaStartDate);
            }
            else
            {
                record.Add("RecentMediaStartDate", "");
            }
            if (recentMediaType != null)
            {
                record.Add("RecentMediaType", recentMediaType);
            }
            else
            {
                record.Add("RecentMediaType", "");
            }
            if (recentMediaState != null)
            {
                record.Add("RecentMediaState", recentMediaState);
            }
            else
            {
                record.Add("RecentMediaState", "");
            }
            if (recentMediaDirection != null)
            {
                record.Add("RecentMediaDirection", recentMediaDirection);
            }
            else
            {
                record.Add("RecentMediaDirection", "");
            }
            if (recentMediaStatus != null)
            {
                record.Add("RecentMediaStatus", recentMediaStatus);
            }
            else
            {
                record.Add("RecentMediaStatus", "");
            }
            string text = "";
            CfgSwitch @switch = cfgDN.Switch;
            if (@switch != null)
            {
                text = @switch.Name;
            }
            if (text == null)
            {
                text = "";
            }
            record.Add("Location", text);

            @switch = null;

            CfgTenant tenant2 = cfgDN.Tenant;
            if (tenant2 != null)
            {
                record.Add("TenantName", tenant2.Name);
                record.Add("TenantPassword", tenant2.Password);
            }
            else
            {
                record.Add("TenantName", "");
                record.Add("TenantPassword", "");
            }
            tenant2 = null;
            cfgDN = null;
            return record;
        }

        /// <summary>
        /// Adds the CFG agent group record.
        /// </summary>
        /// <param name="cfgAgentGroup">The CFG agent group.</param>
        /// <param name="LevelType">Type of the level.</param>
        /// <param name="LevelDbid">The level dbid.</param>
        /// <param name="FavoriteCategory">The favorite category.</param>
        /// <param name="RecentDateTime">The recent date time.</param>
        /// <param name="recentMediaStartDate">The recent media start date.</param>
        /// <param name="recentMediaType">Type of the recent media.</param>
        /// <param name="recentMediaState">State of the recent media.</param>
        /// <param name="recentMediaDirection">The recent media direction.</param>
        /// <param name="recentMediaStatus">The recent media status.</param>
        /// <returns></returns>
        private Dictionary<string, string> AddCfgAgentGroupRecord(CfgAgentGroup cfgAgentGroup, string LevelType, string LevelDbid, string FavoriteCategory,
            string RecentDateTime, string recentMediaStartDate, string recentMediaType, string recentMediaState, string recentMediaDirection, string recentMediaStatus)
        {
            if (cfgAgentGroup == null)
                return null;
            Dictionary<string, string> record = new Dictionary<string, string>();
            record.Add("ContactType", Datacontext.SelectorFilters.AgentGroup.ToString());
            record.Add("DBID", cfgAgentGroup.DBID.ToString(CultureInfo.InvariantCulture));
            CfgGroup groupInfo = cfgAgentGroup.GroupInfo;
            if ((groupInfo != null) && (groupInfo.Name != null))
            {
                record.Add("Name", groupInfo.Name);
            }
            else
            {
                record.Add("Name", "");
            }
            if (LevelType != null)
            {
                record.Add("LevelType", LevelType);
            }
            else
            {
                record.Add("LevelType", "");
            }
            if (LevelDbid != null)
            {
                record.Add("LevelDBID", LevelDbid);
            }
            else
            {
                record.Add("LevelDBID", "");
            }
            if (FavoriteCategory != null)
            {
                record.Add("Category", FavoriteCategory);
            }
            else
            {
                record.Add("Category", "");
            }
            if (RecentDateTime != null)
            {
                record.Add("DateTime", RecentDateTime);
            }
            else
            {
                record.Add("DateTime", "");
            }
            if (recentMediaStartDate != null)
            {
                record.Add("RecentMediaStartDate", recentMediaStartDate);
            }
            else
            {
                record.Add("RecentMediaStartDate", "");
            }
            if (recentMediaType != null)
            {
                record.Add("RecentMediaType", recentMediaType);
            }
            else
            {
                record.Add("RecentMediaType", "");
            }
            if (recentMediaState != null)
            {
                record.Add("RecentMediaState", recentMediaState);
            }
            else
            {
                record.Add("RecentMediaState", "");
            }
            if (recentMediaDirection != null)
            {
                record.Add("RecentMediaDirection", recentMediaDirection);
            }
            else
            {
                record.Add("RecentMediaDirection", "");
            }
            if (recentMediaStatus != null)
            {
                record.Add("RecentMediaStatus", recentMediaStatus);
            }
            else
            {
                record.Add("RecentMediaStatus", "");
            }
            if ((groupInfo != null) && (groupInfo.Tenant != null))
            {
                CfgTenant tenant2 = groupInfo.Tenant;
                record.Add("TenantName", tenant2.Name);
                record.Add("TenantPassword", tenant2.Password);
                tenant2 = null;
            }
            else
            {
                record.Add("TenantName", "");
                record.Add("TenantPassword", "");
            }
            cfgAgentGroup = null;
            return record;
        }

        /// <summary>
        /// Adds the CFG routing point record.
        /// </summary>
        /// <param name="cfgDN">The CFG dn.</param>
        /// <param name="LevelType">Type of the level.</param>
        /// <param name="LevelDbid">The level dbid.</param>
        /// <param name="FavoriteCategory">The favorite category.</param>
        /// <param name="RecentDateTime">The recent date time.</param>
        /// <param name="recentMediaStartDate">The recent media start date.</param>
        /// <param name="recentMediaType">Type of the recent media.</param>
        /// <param name="recentMediaState">State of the recent media.</param>
        /// <param name="recentMediaDirection">The recent media direction.</param>
        /// <param name="recentMediaStatus">The recent media status.</param>
        /// <returns></returns>
        private Dictionary<string, string> AddCfgDNRecord(CfgDN cfgDN, string LevelType, string LevelDbid, string FavoriteCategory, string RecentDateTime,
            string recentMediaStartDate, string recentMediaType, string recentMediaState, string recentMediaDirection, string recentMediaStatus)
        {
            if (cfgDN == null)
                return null;
            Dictionary<string, string> record = new Dictionary<string, string>();
            if (cfgDN.Type == CfgDNType.CFGRoutingPoint)
                record.Add("ContactType", Datacontext.SelectorFilters.RoutingPoint.ToString());
            else
                record.Add("ContactType", Datacontext.SelectorFilters.DN.ToString());
            record.Add("DBID", cfgDN.DBID.ToString(CultureInfo.InvariantCulture));
            if (cfgDN.Number != null)
            {
                record.Add("Number", cfgDN.Number);
            }
            else
            {
                record.Add("Number", "");
            }
            if (cfgDN.Name != null)
            {
                record.Add("Name", cfgDN.Name);
            }
            else
            {
                record.Add("Name", "");
            }
            string name = "";
            CfgSwitch switch2 = cfgDN.Switch;
            if (switch2 != null)
            {
                name = switch2.Name;
            }
            if (name == null)
            {
                name = "";
            }
            record.Add("Location", name);
            if (LevelType != null)
            {
                record.Add("LevelType", LevelType);
            }
            else
            {
                record.Add("LevelType", "");
            }
            if (LevelDbid != null)
            {
                record.Add("LevelDBID", LevelDbid);
            }
            else
            {
                record.Add("LevelDBID", "");
            }
            if (FavoriteCategory != null)
            {
                record.Add("Category", FavoriteCategory);
            }
            else
            {
                record.Add("Category", "");
            }
            if (RecentDateTime != null)
            {
                record.Add("DateTime", RecentDateTime);
            }
            else
            {
                record.Add("DateTime", "");
            }
            if (recentMediaStartDate != null)
            {
                record.Add("RecentMediaStartDate", recentMediaStartDate);
            }
            else
            {
                record.Add("RecentMediaStartDate", "");
            }
            if (recentMediaType != null)
            {
                record.Add("RecentMediaType", recentMediaType);
            }
            else
            {
                record.Add("RecentMediaType", "");
            }
            if (recentMediaState != null)
            {
                record.Add("RecentMediaState", recentMediaState);
            }
            else
            {
                record.Add("RecentMediaState", "");
            }
            if (recentMediaDirection != null)
            {
                record.Add("RecentMediaDirection", recentMediaDirection);
            }
            else
            {
                record.Add("RecentMediaDirection", "");
            }
            if (recentMediaStatus != null)
            {
                record.Add("RecentMediaStatus", recentMediaStatus);
            }
            else
            {
                record.Add("RecentMediaStatus", "");
            }
            CfgTenant tenant2 = cfgDN.Tenant;
            if (tenant2 != null)
            {
                record.Add("TenantName", tenant2.Name);
                record.Add("TenantPassword", tenant2.Password);
            }
            else
            {
                record.Add("TenantName", "");
                record.Add("TenantPassword", "");
            }
            tenant2 = null;
            cfgDN = null;
            return record;
        }

        /// <summary>
        /// Adds the CFG interaction queue record.
        /// </summary>
        /// <param name="cfgScript">The CFG script.</param>
        /// <param name="LevelType">Type of the level.</param>
        /// <param name="LevelDbid">The level dbid.</param>
        /// <param name="FavoriteCategory">The favorite category.</param>
        /// <param name="RecentDateTime">The recent date time.</param>
        /// <param name="Medialist">The medialist.</param>
        /// <param name="recentMediaStartDate">The recent media start date.</param>
        /// <param name="recentMediaType">Type of the recent media.</param>
        /// <param name="recentMediaState">State of the recent media.</param>
        /// <param name="recentMediaDirection">The recent media direction.</param>
        /// <param name="recentMediaStatus">The recent media status.</param>
        /// <returns></returns>
        private Dictionary<string, string> AddCfgInteractionQueueRecord(CfgScript cfgScript, string LevelType, string LevelDbid, string FavoriteCategory,
            string RecentDateTime, string Medialist, string recentMediaStartDate, string recentMediaType, string recentMediaState, string recentMediaDirection,
            string recentMediaStatus)
        {
            if (cfgScript == null)
                return null;
            Dictionary<string, string> record = new Dictionary<string, string>();
            record.Add("ContactType", Datacontext.SelectorFilters.InteractionQueue.ToString());
            record.Add("DBID", cfgScript.DBID.ToString(CultureInfo.InvariantCulture));
            if (cfgScript.Name != null)
            {
                record.Add("Name", cfgScript.Name);
                if (cfgScript.UserProperties != null)
                {
                    KeyValueCollection values = cfgScript.UserProperties["Namespace"] as KeyValueCollection;
                    if (values != null)
                    {
                        if (values["Name"] != null)
                        {
                            record.Add("DisplayName", values["Name"].ToString());
                        }
                        else
                        {
                            record.Add("DisplayName", cfgScript.Name);
                        }
                    }
                    else
                    {
                        record.Add("DisplayName", cfgScript.Name);
                    }
                }
                else
                {
                    record.Add("DisplayName", cfgScript.Name);
                }
            }
            else
            {
                record.Add("Name", "");
                record.Add("DisplayName", "");
            }
            if (LevelType != null)
            {
                record.Add("LevelType", LevelType);
            }
            else
            {
                record.Add("LevelType", "");
            }
            if (LevelDbid != null)
            {
                record.Add("LevelDBID", LevelDbid);
            }
            else
            {
                record.Add("LevelDBID", "");
            }
            if (FavoriteCategory != null)
            {
                record.Add("Category", FavoriteCategory);
            }
            else
            {
                record.Add("Category", "");
            }
            if (RecentDateTime != null)
            {
                record.Add("DateTime", RecentDateTime);
            }
            else
            {
                record.Add("DateTime", "");
            }
            if (recentMediaStartDate != null)
            {
                record.Add("RecentMediaStartDate", recentMediaStartDate);
            }
            else
            {
                record.Add("RecentMediaStartDate", "");
            }
            if (recentMediaType != null)
            {
                record.Add("RecentMediaType", recentMediaType);
            }
            else
            {
                record.Add("RecentMediaType", "");
            }
            if (recentMediaState != null)
            {
                record.Add("RecentMediaState", recentMediaState);
            }
            else
            {
                record.Add("RecentMediaState", "");
            }
            if (recentMediaDirection != null)
            {
                record.Add("RecentMediaDirection", recentMediaDirection);
            }
            else
            {
                record.Add("RecentMediaDirection", "");
            }
            if (recentMediaStatus != null)
            {
                record.Add("RecentMediaStatus", recentMediaStatus);
            }
            else
            {
                record.Add("RecentMediaStatus", "");
            }
            record.Add("MediaList", Medialist);

            CfgTenant tenant2 = cfgScript.Tenant;
            if (tenant2 != null)
            {
                record.Add("TenantName", tenant2.Name);
                record.Add("TenantPassword", tenant2.Password);
            }
            else
            {
                record.Add("TenantName", "");
                record.Add("TenantPassword", "");
            }
            tenant2 = null;
            cfgScript = null;
            return record;
        }

        /// <summary>
        /// Adds the CFG person record.
        /// </summary>
        /// <param name="cfgPerson">The CFG person.</param>
        /// <param name="LevelType">Type of the level.</param>
        /// <param name="LevelDbid">The level dbid.</param>
        /// <param name="FavoriteCategory">The favorite category.</param>
        /// <param name="RecentDateTime">The recent date time.</param>
        /// <param name="recentMediaStartDate">The recent media start date.</param>
        /// <param name="recentMediaType">Type of the recent media.</param>
        /// <param name="recentMediaState">State of the recent media.</param>
        /// <param name="recentMediaDirection">The recent media direction.</param>
        /// <param name="recentMediaStatus">The recent media status.</param>
        /// <returns></returns>
        private Dictionary<string, string> AddCfgPersonRecord(CfgPerson cfgPerson, string LevelType, string LevelDbid, string FavoriteCategory, string RecentDateTime, string
            recentMediaStartDate, string recentMediaType, string recentMediaState, string recentMediaDirection, string recentMediaStatus)
        {
            if (cfgPerson == null)
                return null;
            Dictionary<string, string> record = new Dictionary<string, string>();
            record.Add("ContactType", Datacontext.SelectorFilters.Agent.ToString());
            record.Add("DBID", cfgPerson.DBID.ToString(CultureInfo.InvariantCulture));
            if (cfgPerson.EmailAddress != null)
            {
                record.Add("EmailAddress", cfgPerson.EmailAddress);
            }
            else
            {
                record.Add("EmailAddress", "");
            }
            if (cfgPerson.EmployeeID != null)
            {
                record.Add("EmployeeID", cfgPerson.EmployeeID);
            }
            else
            {
                record.Add("EmployeeID", "");
            }
            if (cfgPerson.ExternalID != null)
            {
                record.Add("ExternalID", cfgPerson.ExternalID);
            }
            else
            {
                record.Add("ExternalID", "");
            }
            if (cfgPerson.FirstName != null)
            {
                record.Add("FirstName", cfgPerson.FirstName);
            }
            else
            {
                record.Add("FirstName", "");
            }
            if (cfgPerson.LastName != null)
            {
                record.Add("LastName", cfgPerson.LastName);
            }
            else
            {
                record.Add("LastName", "");
            }
            record.Add("Phones", "");
            if (LevelType != null)
            {
                record.Add("LevelType", LevelType);
            }
            else
            {
                record.Add("LevelType", "");
            }
            if (LevelDbid != null)
            {
                record.Add("LevelDBID", LevelDbid);
            }
            else
            {
                record.Add("LevelDBID", "");
            }
            if (FavoriteCategory != null)
            {
                record.Add("Category", FavoriteCategory);
            }
            else
            {
                record.Add("Category", "");
            }
            if (RecentDateTime != null)
            {
                record.Add("DateTime", RecentDateTime);
            }
            else
            {
                record.Add("DateTime", "");
            }
            if (recentMediaStartDate != null)
            {
                record.Add("RecentMediaStartDate", recentMediaStartDate);
            }
            else
            {
                record.Add("RecentMediaStartDate", "");
            }
            if (recentMediaType != null)
            {
                record.Add("RecentMediaType", recentMediaType);
            }
            else
            {
                record.Add("RecentMediaType", "");
            }
            if (recentMediaState != null)
            {
                record.Add("RecentMediaState", recentMediaState);
            }
            else
            {
                record.Add("RecentMediaState", "");
            }
            if (recentMediaDirection != null)
            {
                record.Add("RecentMediaDirection", recentMediaDirection);
            }
            else
            {
                record.Add("RecentMediaDirection", "");
            }
            if (recentMediaStatus != null)
            {
                record.Add("RecentMediaStatus", recentMediaStatus);
            }
            else
            {
                record.Add("RecentMediaStatus", "");
            }

            {
                CfgTenant tenant2 = cfgPerson.Tenant;
                if (tenant2 != null)
                {
                    record.Add("TenantName", tenant2.Name);
                    record.Add("TenantPassword", tenant2.Password);
                }
                else
                {
                    record.Add("TenantName", "");
                    record.Add("TenantPassword", "");
                }
                tenant2 = null;
            }
            record.Add("UserName", cfgPerson.UserName);
            cfgPerson = null;
            return record;
        }

        /// <summary>
        /// Adds the CFG skill record.
        /// </summary>
        /// <param name="cfgSkill">The CFG skill.</param>
        /// <param name="LevelType">Type of the level.</param>
        /// <param name="LevelDbid">The level dbid.</param>
        /// <param name="FavoriteCategory">The favorite category.</param>
        /// <param name="RecentDateTime">The recent date time.</param>
        /// <param name="recentMediaStartDate">The recent media start date.</param>
        /// <param name="recentMediaType">Type of the recent media.</param>
        /// <param name="recentMediaState">State of the recent media.</param>
        /// <param name="recentMediaDirection">The recent media direction.</param>
        /// <param name="recentMediaStatus">The recent media status.</param>
        /// <returns></returns>
        private Dictionary<string, string> AddCfgSkillRecord(CfgSkill cfgSkill, string LevelType, string LevelDbid, string FavoriteCategory, string RecentDateTime,
            string recentMediaStartDate, string recentMediaType, string recentMediaState, string recentMediaDirection, string recentMediaStatus)
        {
            if (cfgSkill == null)
                return null;
            Dictionary<string, string> record = new Dictionary<string, string>();
            record.Add("ContactType", Datacontext.SelectorFilters.Skill.ToString());
            record.Add("DBID", cfgSkill.DBID.ToString(CultureInfo.InvariantCulture));
            if (cfgSkill.Name != null)
            {
                record.Add("Name", cfgSkill.Name);
            }
            else
            {
                record.Add("Name", "");
            }
            if (LevelType != null)
            {
                record.Add("LevelType", LevelType);
            }
            else
            {
                record.Add("LevelType", "");
            }
            if (LevelDbid != null)
            {
                record.Add("LevelDBID", LevelDbid);
            }
            else
            {
                record.Add("LevelDBID", "");
            }
            if (FavoriteCategory != null)
            {
                record.Add("Category", FavoriteCategory);
            }
            else
            {
                record.Add("Category", "");
            }
            if (RecentDateTime != null)
            {
                record.Add("DateTime", RecentDateTime);
            }
            else
            {
                record.Add("DateTime", "");
            }
            if (recentMediaStartDate != null)
            {
                record.Add("RecentMediaStartDate", recentMediaStartDate);
            }
            else
            {
                record.Add("RecentMediaStartDate", "");
            }
            if (recentMediaType != null)
            {
                record.Add("RecentMediaType", recentMediaType);
            }
            else
            {
                record.Add("RecentMediaType", "");
            }
            if (recentMediaState != null)
            {
                record.Add("RecentMediaState", recentMediaState);
            }
            else
            {
                record.Add("RecentMediaState", "");
            }
            if (recentMediaDirection != null)
            {
                record.Add("RecentMediaDirection", recentMediaDirection);
            }
            else
            {
                record.Add("RecentMediaDirection", "");
            }
            if (recentMediaStatus != null)
            {
                record.Add("RecentMediaStatus", recentMediaStatus);
            }
            else
            {
                record.Add("RecentMediaStatus", "");
            }
            CfgTenant tenant2 = cfgSkill.Tenant;
            if (tenant2 != null)
            {
                record.Add("TenantName", tenant2.Name);
                record.Add("TenantPassword", tenant2.Password);
            }
            else
            {
                record.Add("TenantName", "");
                record.Add("TenantPassword", "");
            }
            tenant2 = null;
            cfgSkill = null;
            return record;
        }

        /// <summary>
        /// Adds the configuration routing point record.
        /// </summary>
        /// <param name="record">The record.</param>
        private void AddConfigDNRecord(Dictionary<string, string> record)
        {
            Document document = new Document();
            try
            {
                for (int i = 0; i < Datacontext.GetInstance().ConfigDNAttributes.Length; i++)
                {
                    string str = "";
                    if (record.TryGetValue(Datacontext.GetInstance().ConfigDNAttributes[i], out str) && (str != null))
                    {
                        document.Add(new Field(Datacontext.GetInstance().ConfigDNAttributes[i], str, Field.Store.YES, Field.Index.ANALYZED));
                    }
                }
                Datacontext.GetInstance().Writer.AddDocument(document);
            }
            catch (Exception ex)
            {
                _logger.Error((ex.InnerException == null) ? ex.Message : ex.InnerException.ToString());
            }
            finally
            {
                document = null;
            }
        }

        /// <summary>
        /// Adds the configuration interaction queue record.
        /// </summary>
        /// <param name="record">The record.</param>
        private void AddConfigInteractionQueueRecord(Dictionary<string, string> record)
        {
            Document document = new Document();
            try
            {
                for (int i = 0; i < Datacontext.GetInstance().ConfigInteractionQueueAttributes.Length; i++)
                {
                    string str = "";
                    if (record.TryGetValue(Datacontext.GetInstance().ConfigInteractionQueueAttributes[i], out str) && (str != null))
                    {
                        document.Add(new Field(Datacontext.GetInstance().ConfigInteractionQueueAttributes[i], str, Field.Store.YES, Field.Index.ANALYZED));
                    }
                }
                Datacontext.GetInstance().Writer.AddDocument(document);
            }
            catch (Exception ex)
            {
                _logger.Error((ex.InnerException == null) ? ex.Message : ex.InnerException.ToString());
            }
            finally
            {
                document = null;
            }
        }

        /// <summary>
        /// Adds the configuration queue record.
        /// </summary>
        /// <param name="record">The record.</param>
        private void AddConfigQueueRecord(Dictionary<string, string> record)
        {
            Document document = new Document();
            try
            {
                for (int i = 0; i < Datacontext.GetInstance().ConfigQueueAttributes.Length; i++)
                {
                    string text = "";
                    if (record.TryGetValue(Datacontext.GetInstance().ConfigQueueAttributes[i], out text) && text != null)
                    {
                        document.Add(new Field(Datacontext.GetInstance().ConfigQueueAttributes[i], text, Field.Store.YES, Field.Index.ANALYZED));
                    }
                }
                Datacontext.GetInstance().Writer.AddDocument(document);
            }
            catch (Exception ex)
            {
                _logger.Error((ex.InnerException == null) ? ex.Message : ex.InnerException.ToString());
            }
            finally
            {
                document = null;
            }
        }

        /// <summary>
        /// Adds the configuration skill record.
        /// </summary>
        /// <param name="record">The record.</param>
        private void AddConfigSkillRecord(Dictionary<string, string> record)
        {
            Document document = new Document();
            try
            {
                for (int i = 0; i < Datacontext.GetInstance().ConfigSkillAttributes.Length; i++)
                {
                    string str = "";
                    if (record.TryGetValue(Datacontext.GetInstance().ConfigSkillAttributes[i], out str) && (str != null))
                    {
                        document.Add(new Field(Datacontext.GetInstance().ConfigSkillAttributes[i], str, Field.Store.YES, Field.Index.ANALYZED));
                    }
                }
                Datacontext.GetInstance().Writer.AddDocument(document);
            }
            catch (Exception ex)
            {
                _logger.Error((ex.InnerException == null) ? ex.Message : ex.InnerException.ToString());
            }
            finally
            {
                document = null;
            }
        }

        /// <summary>
        /// Adds the ucs contact record.
        /// </summary>
        /// <param name="record">The record.</param>
        private void AddUCSContactRecord(Dictionary<string, string> record)
        {
            Document document = new Document();
            try
            {
                for (int i = 0; i < Datacontext.GetInstance().ConfigContactAttributes.Length; i++)
                {
                    string str = "";
                    if (record.TryGetValue(Datacontext.GetInstance().ConfigContactAttributes[i], out str) && (str != null))
                    {
                        document.Add(new Field(Datacontext.GetInstance().ConfigContactAttributes[i], str, Field.Store.YES, Field.Index.ANALYZED));
                    }
                }
                Datacontext.GetInstance().Writer.AddDocument(document);
            }
            catch (Exception ex)
            {
                _logger.Error((ex.InnerException == null) ? ex.Message : ex.InnerException.ToString());
            }
            finally
            {
                document = null;
            }
        }

        /// <summary>
        /// Adds the ucs contact record.
        /// </summary>
        /// <param name="attributeList">The attribute list.</param>
        /// <param name="contactId">The contact unique identifier.</param>
        /// <param name="LevelType">Type of the level.</param>
        /// <param name="LevelDbid">The level dbid.</param>
        /// <param name="FavoriteCategory">The favorite category.</param>
        /// <param name="RecentDateTime">The recent date time.</param>
        /// <param name="recentMediaStartDate">The recent media start date.</param>
        /// <param name="recentMediaType">Type of the recent media.</param>
        /// <param name="recentMediaState">State of the recent media.</param>
        /// <param name="recentMediaDirection">The recent media direction.</param>
        /// <param name="recentMediaStatus">The recent media status.</param>
        /// <returns></returns>
        private Dictionary<string, string> AddUCSContactRecord(Genesyslab.Platform.Contacts.Protocols.ContactServer.ContactAttributeList attributeList, string contactId,
            string LevelType, string LevelDbid,
            string FavoriteCategory, string RecentDateTime, string recentMediaStartDate, string recentMediaType, string recentMediaState, string recentMediaDirection,
            string recentMediaStatus)
        {
            if (attributeList == null)
                return null;
            Dictionary<string, string> record = new Dictionary<string, string>();
            record.Add("ContactType", Datacontext.SelectorFilters.Contact.ToString());
            record.Add("DBID", contactId.ToString(CultureInfo.InvariantCulture));
            record.Add("ContactId", contactId.ToString(CultureInfo.InvariantCulture));

            if (LevelType != null)
            {
                record.Add("LevelType", LevelType);
            }
            else
            {
                record.Add("LevelType", "");
            }
            if (LevelDbid != null)
            {
                record.Add("LevelDBID", LevelDbid);
            }
            else
            {
                record.Add("LevelDBID", "");
            }
            if (FavoriteCategory != null)
            {
                record.Add("Category", FavoriteCategory);
            }
            else
            {
                record.Add("Category", "");
            }
            if (RecentDateTime != null)
            {
                record.Add("DateTime", RecentDateTime);
            }
            else
            {
                record.Add("DateTime", "");
            }
            if (recentMediaStartDate != null)
            {
                record.Add("RecentMediaStartDate", recentMediaStartDate);
            }
            else
            {
                record.Add("RecentMediaStartDate", "");
            }
            if (recentMediaType != null)
            {
                record.Add("RecentMediaType", recentMediaType);
            }
            else
            {
                record.Add("RecentMediaType", "");
            }
            if (recentMediaState != null)
            {
                record.Add("RecentMediaState", recentMediaState);
            }
            else
            {
                record.Add("RecentMediaState", "");
            }
            if (recentMediaDirection != null)
            {
                record.Add("RecentMediaDirection", recentMediaDirection);
            }
            else
            {
                record.Add("RecentMediaDirection", "");
            }
            if (recentMediaStatus != null)
            {
                record.Add("RecentMediaStatus", recentMediaStatus);
            }
            else
            {
                record.Add("RecentMediaStatus", "");
            }

            if (attributeList != null)
            {
                List<Genesyslab.Platform.Contacts.Protocols.ContactServer.Attribute> attribute =
                    attributeList.Cast<Genesyslab.Platform.Contacts.Protocols.ContactServer.Attribute>().Where(x =>
                        x.AttributeName.Equals("FirstName")).ToList();
                if (attribute != null && attribute.Count > 0)
                {
                    if (!record.ContainsKey("FirstName"))
                    {
                        record.Add("FirstName", attribute[0].AttributeValue.ToString());
                    }
                    else
                    {
                        record["FirstName"] = attribute[0].AttributeValue.ToString();
                    }
                }
                else
                {
                    if (!record.ContainsKey("FirstName"))
                    {
                        record.Add("FirstName", "");
                    }
                    else
                        record["FirstName"] = "";
                }

                attribute = attributeList.Cast<Genesyslab.Platform.Contacts.Protocols.ContactServer.Attribute>().Where(x =>
                        x.AttributeName.Equals("LastName")).ToList();
                if (attribute != null && attribute.Count > 0)
                {
                    if (!record.ContainsKey("LastName"))
                    {
                        record.Add("LastName", attribute[0].AttributeValue.ToString());
                        if (attribute[0].AttributeValue.ToString() == "T1")
                        {
                            var test = "teset";
                            var teststt = "";
                            teststt = test;
                        }
                    }
                    else
                    {
                        record["LastName"] = attribute[0].AttributeValue.ToString();
                    }
                }
                else
                {
                    if (!record.ContainsKey("LastName"))
                    {
                        record.Add("LastName", "");
                    }
                    else
                        record["LastName"] = "";
                }

                attribute = attributeList.Cast<Genesyslab.Platform.Contacts.Protocols.ContactServer.Attribute>().Where(x =>
                        x.AttributeName.Equals("PhoneNumber")).ToList();
                if (attribute != null && attribute.Count > 0)
                {
                    string phNos = string.Empty;
                    foreach (Genesyslab.Platform.Contacts.Protocols.ContactServer.Attribute attrib in attribute)
                    {
                        if (phNos == string.Empty)
                            phNos += attrib.AttributeValue.StringValue;
                        else
                            phNos += " " + attrib.AttributeValue.StringValue;
                    }
                    if (!record.ContainsKey("PhoneNumber"))
                    {
                        record.Add("PhoneNumber", phNos);
                    }
                    else
                    {
                        record["PhoneNumber"] = phNos;
                    }
                }
                else
                {
                    if (!record.ContainsKey("PhoneNumber"))
                    {
                        record.Add("PhoneNumber", "");
                    }
                    else
                        record["PhoneNumber"] = "";
                }

                attribute = attributeList.Cast<Genesyslab.Platform.Contacts.Protocols.ContactServer.Attribute>().Where(x =>
                        x.AttributeName.Equals("EmailAddress")).ToList();

                if (attribute != null && attribute.Count > 0)
                {
                    string emailAddress = string.Empty;
                    foreach (Genesyslab.Platform.Contacts.Protocols.ContactServer.Attribute attrib in attribute)
                    {
                        if (emailAddress == string.Empty)
                            emailAddress += attrib.AttributeValue.StringValue;
                        else
                            emailAddress += " " + attrib.AttributeValue.StringValue;
                    }
                    if (!record.ContainsKey("EmailAddress"))
                    {
                        record.Add("EmailAddress", emailAddress);
                    }
                    else
                    {
                        record["EmailAddress"] = emailAddress;
                    }
                }
                else
                {
                    if (!record.ContainsKey("EmailAddress"))
                    {
                        record.Add("EmailAddress", "");
                    }
                    else
                        record["EmailAddress"] = "";
                }
            }

            record.Add("TenantName", "");
            record.Add("TenantPassword", "");
            attributeList = null;
            return record;
        }

        #endregion Methods
    }
}