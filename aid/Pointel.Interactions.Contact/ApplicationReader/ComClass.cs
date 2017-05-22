using System;
using System.Collections.Generic;
using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.CfgObjects;
using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.Queries;
using Pointel.Configuration.Manager;
using Pointel.Interactions.Contact.Settings;
using System.Threading;

namespace Pointel.Interactions.Contact.ApplicationReader
{
    public class ComClass
    {
        #region Field Declaration
        private Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
            "AID");
        #endregion

        ComClass()
        {
            var dataContext = ContactDataContext.GetInstance();
        }

        #region Single Instance
        private static ComClass _instance = null;

        public static ComClass GetInstance()
        {
            if (_instance == null)
            {
                _instance = new ComClass();
            }
            return _instance;
        }
        #endregion


        #region ReadPersonObject
        /// <summary>
        /// Reads the person details.
        /// </summary>
        /// <param name="username">The username.</param>
        //private void ReadPersonDetails(string username)
        //{
        //    loadCollection.Clear();
        //    try
        //    {
        //        CfgPersonQuery personQuery = new CfgPersonQuery();
        //        personQuery.UserName = username;
        //      //  application.TenantDbid =ConfigContainer.Instance().TenantDbId;
        //        personQuery.TenantDbid =ConfigContainer.Instance().TenantDbId;
        //        CfgApplicationQuery queryApp = new CfgApplicationQuery();
        //        _person = ContactDataContext.ComObject.RetrieveObject<CfgPerson>(personQuery);
        //        logger.Debug("Retrieving Values from Person : UserName : " + username);
        //        personenabledisableValues = (KeyValueCollection)_person.UserProperties["enable-disable-channels"];
        //        personixnValues = (KeyValueCollection)_person.UserProperties["agent.ixn.desktop"];
        //        ContactDataContext.GetInstance().PersonDBID = _person.DBID.ToString();
        //        if (personixnValues != null && personixnValues.Count > 0)
        //        {
        //            foreach (string key in personixnValues.AllKeys)
        //            {
        //                if (string.Compare(key, "contact.available-directory-page-sizes", true) == 0)
        //                {
        //                    logger.Info("Key : " + key + " values : " + personixnValues[key].ToString());
        //                    if (personixnValues[key].ToString() != "")
        //                    {
        //                        loadCollection.Add(key, personixnValues[key].ToString());
        //                    }
        //                }
        //                //else if (string.Compare(key, "ImagepathContact-path", true) == 0)
        //                //{
        //                //    if (!loadCollection.ContainsKey(key))
        //                //    {
        //                //        if (personixnValues[key].ToString() != "")
        //                //        {
        //                //            loadCollection.Add(key, personixnValues[key].ToString());
        //                //            logger.Info("ReadPersonObject key:" + key.ToString() + ":" + personixnValues[key].ToString());
        //                //        }
        //                //    }
        //                //}

        //                else if (string.Compare(key, "email.standard-response.default-search-type", true) == 0)
        //                {
        //                    logger.Info("Key : " + key + " values : " + personixnValues[key].ToString());
        //                    if (personixnValues[key].ToString() != "")
        //                    {
        //                        if(!loadCollection.ContainsKey(key))
        //                            loadCollection.Add(key, personixnValues[key].ToString());
        //                    }
        //                }
        //                else if (string.Compare(key, "interaction.casedata-disposition-object", true) == 0)
        //                {
        //                    logger.Info("Key : " + key + " values : " + personixnValues[key].ToString());
        //                    if (personixnValues[key].ToString() != "")
        //                    {
        //                        if (!loadCollection.ContainsKey(key))
        //                            loadCollection.Add(key, personixnValues[key].ToString());
        //                    }
        //                }
        //                else if (string.Compare(key, "interaction.casedata.use-transaction-object", true) == 0)
        //                {
        //                    logger.Info("Key : " + key + " values : " + personixnValues[key].ToString());
        //                    if (personixnValues[key].ToString() != "")
        //                    {
        //                        if (!loadCollection.ContainsKey(key))
        //                            loadCollection.Add(key, personixnValues[key].ToString());
        //                    }
        //                }

        //                else if (string.Compare(key, "contact.date-search-types", true) == 0)
        //                {
        //                    logger.Info("Key : " + key + " values : " + personixnValues[key].ToString());
        //                    if (personixnValues[key].ToString() != "")
        //                    {
        //                        loadCollection.Add(key, personixnValues[key].ToString());
        //                    }
        //                }
        //                else if (string.Compare(key, "contact.default-directory-page-size", true) == 0)
        //                {
        //                    logger.Info("Key : " + key + " values : " + personixnValues[key].ToString());
        //                    if (personixnValues[key].ToString() != "")
        //                    {
        //                        loadCollection.Add(key, personixnValues[key].ToString());
        //                    }
        //                }
        //                else if (string.Compare(key, "contact.displayed-attributes", true) == 0)
        //                {
        //                    logger.Info("Key : " + key + " values : " + personixnValues[key].ToString());
        //                    if (personixnValues[key].ToString() != "")
        //                    {
        //                        loadCollection.Add(key, personixnValues[key].ToString());
        //                    }
        //                }
        //                else if (string.Compare(key, "contact.directory-displayed-columns", true) == 0)
        //                {
        //                    logger.Info("Key : " + key + " values : " + personixnValues[key].ToString());
        //                    if (personixnValues[key].ToString() != "")
        //                    {
        //                        loadCollection.Add(key, personixnValues[key].ToString());
        //                    }
        //                }
        //                else if (string.Compare(key, "contact.history-default-time-filter-main", true) == 0)
        //                {
        //                    logger.Info("Key : " + key + " values : " + personixnValues[key].ToString());
        //                    if (personixnValues[key].ToString() != "")
        //                    {
        //                        loadCollection.Add(key, personixnValues[key].ToString());
        //                    }
        //                }
        //                else if (string.Compare(key, "contact.myhistory-default-time-filter-main", true) == 0)
        //                {
        //                    logger.Info("Key : " + key + " values : " + personixnValues[key].ToString());
        //                    if (personixnValues[key].ToString() != "")
        //                    {
        //                        loadCollection.Add(key, personixnValues[key].ToString());
        //                    }
        //                }
        //                else if (string.Compare(key, "contact.history-displayed-columns", true) == 0)
        //                {
        //                    logger.Info("Key : " + key + " values : " + personixnValues[key].ToString());
        //                    if (personixnValues[key].ToString() != "")
        //                    {
        //                        loadCollection.Add(key, personixnValues[key].ToString());
        //                    }
        //                }
        //                else if (string.Compare(key, "contact.history-search-attributes", true) == 0)
        //                {
        //                    logger.Info("Key : " + key + " values : " + personixnValues[key].ToString());
        //                    if (personixnValues[key].ToString() != "")
        //                    {
        //                        loadCollection.Add(key, personixnValues[key].ToString());
        //                    }
        //                }
        //                else if (string.Compare(key, "contact.directory-search-attributes", true) == 0)
        //                {
        //                    logger.Info("Key : " + key + " values : " + personixnValues[key].ToString());
        //                    if (personixnValues[key].ToString() != "")
        //                    {
        //                        loadCollection.Add(key, personixnValues[key].ToString());
        //                    }
        //                }
        //                else if (string.Compare(key, "contact.directory-search-types", true) == 0)
        //                {
        //                    logger.Info("Key : " + key + " values : " + personixnValues[key].ToString());
        //                    if (personixnValues[key].ToString() != "")
        //                    {
        //                        loadCollection.Add(key, personixnValues[key].ToString());
        //                    }
        //                }
        //                else if (string.Compare(key, "contact.directory-advanced-default", true) == 0)
        //                {
        //                    logger.Info("Key : " + key + " values : " + personixnValues[key].ToString());
        //                    if (personixnValues[key].ToString() != "")
        //                    {
        //                        loadCollection.Add(key, personixnValues[key].ToString());
        //                    }
        //                }

        //                else if (string.Compare(key, "contact.history.media-filters", true) == 0)
        //                {
        //                    logger.Info("Key : " + key + " values : " + personixnValues[key].ToString());
        //                    if (personixnValues[key].ToString() != "")
        //                    {
        //                        loadCollection.Add(key, personixnValues[key].ToString());
        //                    }
        //                }
        //                else if (string.Compare(key, "contact.mandatory-attributes", true) == 0)
        //                {
        //                    logger.Info("Key : " + key + " values : " + personixnValues[key].ToString());
        //                    if (personixnValues[key].ToString() != "")
        //                    {
        //                        loadCollection.Add(key, personixnValues[key].ToString());
        //                    }
        //                }
        //                else if (string.Compare(key, "contact.multiple-value-attributes", true) == 0)
        //                {
        //                    logger.Info("Key : " + key + " values : " + personixnValues[key].ToString());
        //                    if (personixnValues[key].ToString() != "")
        //                    {
        //                        loadCollection.Add(key, personixnValues[key].ToString());
        //                    }
        //                }
        //                else if (string.Compare(key, "chat.agent.prompt-color", true) == 0)
        //                {
        //                    logger.Info("Key : " + key + " values : " + personixnValues[key].ToString());
        //                    if (personixnValues[key].ToString() != "")
        //                    {
        //                        loadCollection.Add(key, personixnValues[key].ToString());
        //                    }
        //                }
        //                else if (string.Compare(key, "chat.agent.text-color", true) == 0)
        //                {
        //                    logger.Info("Key : " + key + " values : " + personixnValues[key].ToString());
        //                    if (personixnValues[key].ToString() != "")
        //                    {
        //                        loadCollection.Add(key, personixnValues[key].ToString());
        //                    }
        //                }
        //                else if (string.Compare(key, "chat.client.prompt-color", true) == 0)
        //                {
        //                    logger.Info("Key : " + key + " values : " + personixnValues[key].ToString());
        //                    if (personixnValues[key].ToString() != "")
        //                    {
        //                        loadCollection.Add(key, personixnValues[key].ToString());
        //                    }
        //                }
        //                else if (string.Compare(key, "chat.client.text-color", true) == 0)
        //                {
        //                    logger.Info("Key : " + key + " values : " + personixnValues[key].ToString());
        //                    if (personixnValues[key].ToString() != "")
        //                    {
        //                        loadCollection.Add(key, personixnValues[key].ToString());
        //                    }
        //                }
        //                else if (string.Compare(key, "chat.other-agent.prompt-color", true) == 0)
        //                {
        //                    logger.Info("Key : " + key + " values : " + personixnValues[key].ToString());
        //                    if (personixnValues[key].ToString() != "")
        //                    {
        //                        loadCollection.Add(key, personixnValues[key].ToString());
        //                    }
        //                }
        //                else if (string.Compare(key, "chat.other-agent.text-color", true) == 0)
        //                {
        //                    logger.Info("Key : " + key + " values : " + personixnValues[key].ToString());
        //                    if (personixnValues[key].ToString() != "")
        //                    {
        //                        loadCollection.Add(key, personixnValues[key].ToString());
        //                    }
        //                }
        //                else if (string.Compare(key, "chat.time-stamp-format", true) == 0)
        //                {
        //                    logger.Info("Key : " + key + " values : " + personixnValues[key].ToString());
        //                    if (personixnValues[key].ToString() != "")
        //                    {
        //                        loadCollection.Add(key, personixnValues[key].ToString());
        //                    }
        //                }
        //                else if (string.Compare(key, "contact.history-advanced-default", true) == 0)
        //                {
        //                    logger.Info("Key : " + key + " values : " + personixnValues[key].ToString());
        //                    if (personixnValues[key].ToString() != "")
        //                    {
        //                        loadCollection.Add(key, personixnValues[key].ToString());
        //                    }
        //                }
        //                else if (string.Compare(key, "interaction.case-data.transaction-object", true) == 0)
        //                {
        //                    logger.Info("Key : " + key + " values : " + personixnValues[key].ToString());
        //                    if (personixnValues[key].ToString() != "")
        //                    {
        //                        loadCollection.Add(key, personixnValues[key].ToString());
        //                    }
        //                }

        //            }
        //        }
        //        ReadAgentGroupValues("agent.ixn.desktop");

        //        if (personenabledisableValues != null && personenabledisableValues.Count > 0)
        //        {
        //            foreach (string key in personenabledisableValues.AllKeys)
        //            {
        //                if (string.Compare(key, "chat.enable.case-data-filter", true) == 0)
        //                {
        //                    logger.Info("Key : " + key + " values : " + personenabledisableValues[key].ToString());
        //                    if (!loadCollection.ContainsKey(key))
        //                    {
        //                        if (personenabledisableValues[key].ToString() != "")
        //                        {
        //                            loadCollection.Add(key, personenabledisableValues[key].ToString());
        //                        }
        //                    }
        //                }
        //                else if (string.Compare(key, "voice.enable.case-data-filter", true) == 0)
        //                {
        //                    logger.Info("Key : " + key + " values : " + personenabledisableValues[key].ToString());
        //                    if (!loadCollection.ContainsKey(key))
        //                    {
        //                        if (personenabledisableValues[key].ToString() != "")
        //                        {
        //                            loadCollection.Add(key, personenabledisableValues[key].ToString());
        //                        }
        //                    }
        //                }
        //                else if (string.Compare(key, "email.enable.case-data-filter", true) == 0)
        //                {
        //                    logger.Info("Key : " + key + " values : " + personenabledisableValues[key].ToString());
        //                    if (!loadCollection.ContainsKey(key))
        //                    {
        //                        if (personenabledisableValues[key].ToString() != "")
        //                        {
        //                            loadCollection.Add(key, personenabledisableValues[key].ToString());
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //        ReadAgentGroupValues("enable-disable-channels");
        //        //KeyValueCollection systemvalues = (KeyValueCollection)_person.UserProperties["_system_"];
        //        //if (systemvalues != null && systemvalues.Count>0)
        //        //{
        //        //    if (systemvalues.ContainsKey("image-path") && !loadCollection.ContainsKey("image-path"))
        //        //    {                           
        //        //        if (!string.IsNullOrEmpty(personixnValues["image-path"].ToString()))
        //        //        {
        //        //            loadCollection.Add("image-path", personixnValues["image-path"].ToString());
        //        //            logger.Info("ReadPersonObject key:image-path:" + personixnValues["image-path"].ToString());
        //        //        }                                      
        //        //    }
        //        //}
        //    }
        //    catch (Exception commonException)
        //    {
        //        logger.Error("Error occurred while reading ReadPersonDetails()" + username + "  =  " + commonException.ToString());
        //    }
        //}
        #endregion

        #region ReadAgentGroupObject
        /// <summary>
        /// Reads the agent group values.
        /// </summary>
        /// <param name="sectionName">Name of the section.</param>
        //private void ReadAgentGroupValues(string sectionName)
        //{
        //    try
        //    {
        //        CfgAgentGroupQuery qAgentGroup = new CfgAgentGroupQuery();
        //        qAgentGroup.PersonDbid = Convert.ToInt32(_person.DBID);
        //        qAgentGroup.TenantDbid =ConfigContainer.Instance().TenantDbId;
        //        logger.Debug("Retrieving Agent Group Values : Group Name : " + sectionName);
        //        AgentGroups = ContactDataContext.ComObject.RetrieveMultipleObjects<CfgAgentGroup>(qAgentGroup);
        //        foreach (CfgAgentGroup agentGroup in AgentGroups)
        //        {
        //            string[] appKeys = agentGroup.GroupInfo.UserProperties.AllKeys;
        //            KeyValueCollection kvColl = new KeyValueCollection();
        //            kvColl = (KeyValueCollection)agentGroup.GroupInfo.UserProperties[sectionName];
        //            if (kvColl != null)
        //            {
        //                #region agent.ixn.desktop
        //                if (string.Compare(sectionName, "agent.ixn.desktop") == 0)
        //                {
        //                    logger.Debug("Retrieving Values from agent.ixn.desktop Section");
        //                    personixnValues = null;
        //                    if (personixnValues == null)
        //                    {
        //                        personixnValues = kvColl;
        //                        foreach (string key in kvColl.AllKeys)
        //                        {
        //                            if (string.Compare(key, "contact.available-directory-page-sizes", true) == 0)
        //                            {
        //                                logger.Info("Key : " + key + " values : " + personixnValues[key].ToString());
        //                                if (personixnValues[key].ToString() != "")
        //                                {
        //                                    loadCollection.Add(key, personixnValues[key].ToString());
        //                                }

        //                                else if (string.Compare(key, "contact.date-search-types", true) == 0)
        //                                {
        //                                    logger.Info("Key : " + key + " values : " + personixnValues[key].ToString());
        //                                    if (personixnValues[key].ToString() != "")
        //                                    {
        //                                        loadCollection.Add(key, personixnValues[key].ToString());
        //                                    }
        //                                }
        //                                else if (string.Compare(key, "email.standard-response.default-search-type", true) == 0)
        //                                {
        //                                    logger.Info("Key : " + key + " values : " + personixnValues[key].ToString());
        //                                    if (personixnValues[key].ToString() != "")
        //                                    {
        //                                        if (!loadCollection.ContainsKey(key))
        //                                            loadCollection.Add(key, personixnValues[key].ToString());
        //                                    }
        //                                }
        //                                else if (string.Compare(key, "contact.default-directory-page-size", true) == 0)
        //                                {
        //                                    logger.Info("Key : " + key + " values : " + personixnValues[key].ToString());
        //                                    if (personixnValues[key].ToString() != "")
        //                                    {
        //                                        loadCollection.Add(key, personixnValues[key].ToString());
        //                                    }
        //                                }
        //                                else if (string.Compare(key, "contact.displayed-attributes", true) == 0)
        //                                {
        //                                    logger.Info("Key : " + key + " values : " + personixnValues[key].ToString());
        //                                    if (personixnValues[key].ToString() != "")
        //                                    {
        //                                        loadCollection.Add(key, personixnValues[key].ToString());
        //                                    }
        //                                }
        //                                else if (string.Compare(key, "contact.directory-displayed-columns", true) == 0)
        //                                {
        //                                    logger.Info("Key : " + key + " values : " + personixnValues[key].ToString());
        //                                    if (personixnValues[key].ToString() != "")
        //                                    {
        //                                        loadCollection.Add(key, personixnValues[key].ToString());
        //                                    }
        //                                }
        //                                else if (string.Compare(key, "contact.history-default-time-filter-main", true) == 0)
        //                                {
        //                                    logger.Info("Key : " + key + " values : " + personixnValues[key].ToString());
        //                                    if (personixnValues[key].ToString() != "")
        //                                    {
        //                                        loadCollection.Add(key, personixnValues[key].ToString());
        //                                    }
        //                                }
        //                                else if (string.Compare(key, "contact.myhistory-default-time-filter-main", true) == 0)
        //                                {
        //                                    logger.Info("Key : " + key + " values : " + personixnValues[key].ToString());
        //                                    if (personixnValues[key].ToString() != "")
        //                                    {
        //                                        loadCollection.Add(key, personixnValues[key].ToString());
        //                                    }
        //                                }
        //                                else if (string.Compare(key, "contact.history-displayed-columns", true) == 0)
        //                                {
        //                                    logger.Info("Key : " + key + " values : " + personixnValues[key].ToString());
        //                                    if (personixnValues[key].ToString() != "")
        //                                    {
        //                                        loadCollection.Add(key, personixnValues[key].ToString());
        //                                    }
        //                                }
        //                                else if (string.Compare(key, "contact.history-search-attributes", true) == 0)
        //                                {
        //                                    logger.Info("Key : " + key + " values : " + personixnValues[key].ToString());
        //                                    if (personixnValues[key].ToString() != "")
        //                                    {
        //                                        loadCollection.Add(key, personixnValues[key].ToString());
        //                                    }
        //                                }
        //                                else if (string.Compare(key, "contact.directory-search-attributes", true) == 0)
        //                                {
        //                                    logger.Info("Key : " + key + " values : " + personixnValues[key].ToString());
        //                                    if (personixnValues[key].ToString() != "")
        //                                    {
        //                                        loadCollection.Add(key, personixnValues[key].ToString());
        //                                    }
        //                                }
        //                                else if (string.Compare(key, "contact.directory-advanced-default", true) == 0)
        //                                {
        //                                    logger.Info("Key : " + key + " values : " + personixnValues[key].ToString());
        //                                    if (personixnValues[key].ToString() != "")
        //                                    {
        //                                        loadCollection.Add(key, personixnValues[key].ToString());
        //                                    }
        //                                }
        //                                else if (string.Compare(key, "contact.directory-search-types", true) == 0)
        //                                {
        //                                    logger.Info("Key : " + key + " values : " + personixnValues[key].ToString());
        //                                    if (personixnValues[key].ToString() != "")
        //                                    {
        //                                        loadCollection.Add(key, personixnValues[key].ToString());
        //                                    }
        //                                }
        //                                else if (string.Compare(key, "contact.history.media-filters", true) == 0)
        //                                {
        //                                    logger.Info("Key : " + key + " values : " + personixnValues[key].ToString());
        //                                    if (personixnValues[key].ToString() != "")
        //                                    {
        //                                        loadCollection.Add(key, personixnValues[key].ToString());
        //                                    }
        //                                }
        //                                else if (string.Compare(key, "contact.mandatory-attributes", true) == 0)
        //                                {
        //                                    logger.Info("Key : " + key + " values : " + personixnValues[key].ToString());
        //                                    if (personixnValues[key].ToString() != "")
        //                                    {
        //                                        loadCollection.Add(key, personixnValues[key].ToString());
        //                                    }
        //                                }
        //                                else if (string.Compare(key, "contact.multiple-value-attributes", true) == 0)
        //                                {
        //                                    logger.Info("Key : " + key + " values : " + personixnValues[key].ToString());
        //                                    if (personixnValues[key].ToString() != "")
        //                                    {
        //                                        loadCollection.Add(key, personixnValues[key].ToString());
        //                                    }
        //                                }
        //                                else if (string.Compare(key, "chat.agent.prompt-color", true) == 0)
        //                                {
        //                                    logger.Info("Key : " + key + " Value : " + kvColl[key].ToString());
        //                                    if (!loadCollection.ContainsKey(key))
        //                                    {
        //                                        loadCollection.Add(key, kvColl[key].ToString());
        //                                    }
        //                                }
        //                                else if (string.Compare(key, "chat.agent.text-color", true) == 0)
        //                                {
        //                                    logger.Info("Key : " + key + " Value : " + kvColl[key].ToString());
        //                                    if (!loadCollection.ContainsKey(key))
        //                                    {
        //                                        loadCollection.Add(key, kvColl[key].ToString());
        //                                    }
        //                                }
        //                                else if (string.Compare(key, "chat.client.prompt-color", true) == 0)
        //                                {
        //                                    logger.Info("Key : " + key + " Value : " + kvColl[key].ToString());
        //                                    if (!loadCollection.ContainsKey(key))
        //                                    {
        //                                        loadCollection.Add(key, kvColl[key].ToString());
        //                                    }
        //                                }
        //                                else if (string.Compare(key, "chat.other-agent.prompt-color", true) == 0)
        //                                {
        //                                    logger.Info("Key : " + key + " Value : " + kvColl[key].ToString());
        //                                    if (!loadCollection.ContainsKey(key))
        //                                    {
        //                                        loadCollection.Add(key, kvColl[key].ToString());
        //                                    }
        //                                }
        //                                else if (string.Compare(key, "chat.other-agent.text-color", true) == 0)
        //                                {
        //                                    logger.Info("Key : " + key + " Value : " + kvColl[key].ToString());
        //                                    if (!loadCollection.ContainsKey(key))
        //                                    {
        //                                        loadCollection.Add(key, kvColl[key].ToString());
        //                                    }
        //                                }
        //                                else if (string.Compare(key, "chat.time-stamp-format", true) == 0)
        //                                {
        //                                    logger.Info("Key : " + key + " Value : " + kvColl[key].ToString());
        //                                    if (!loadCollection.ContainsKey(key))
        //                                    {
        //                                        loadCollection.Add(key, kvColl[key].ToString());
        //                                    }
        //                                }
        //                                else if (string.Compare(key, "contact.history-advanced-default", true) == 0)
        //                                {
        //                                    logger.Info("Key : " + key + " Value : " + kvColl[key].ToString());
        //                                    if (!loadCollection.ContainsKey(key))
        //                                    {
        //                                        loadCollection.Add(key, kvColl[key].ToString());
        //                                    }
        //                                }
        //                                else if (string.Compare(key, "interaction.case-data.transaction-object", true) == 0)
        //                                {
        //                                    logger.Info("Key : " + key + " values : " + personixnValues[key].ToString());
        //                                    if (personixnValues[key].ToString() != "")
        //                                    {
        //                                        loadCollection.Add(key, personixnValues[key].ToString());
        //                                    }
        //                                }
        //                                else if (string.Compare(key, "interaction.casedata-disposition-object", true) == 0)
        //                                {
        //                                    logger.Info("Key : " + key + " values : " + personixnValues[key].ToString());
        //                                    if (personixnValues[key].ToString() != "")
        //                                    {
        //                                        loadCollection.Add(key, personixnValues[key].ToString());
        //                                    }
        //                                }
        //                                else if (string.Compare(key, "interaction.casedata.use-transaction-object", true) == 0)
        //                                {
        //                                    logger.Info("Key : " + key + " values : " + personixnValues[key].ToString());
        //                                    if (personixnValues[key].ToString() != "")
        //                                    {
        //                                        loadCollection.Add(key, personixnValues[key].ToString());
        //                                    }
        //                                }


        //                            }
        //                        }
        //                    }
        //                #endregion

        //                #region enable-disable-channels

        //                if (string.Compare(sectionName, "enable-disable-channels") == 0)
        //                {
        //                    logger.Debug("Retrieving Values from enable-disable-channels Section");
        //                    personenabledisableValues = null;
        //                    if (personenabledisableValues == null)
        //                    {
        //                        personenabledisableValues = kvColl;
        //                        foreach (string _key in kvColl.Keys)
        //                        {
        //                            if (string.Compare(_key, "voice.enable.case-data-filter", true) == 0)
        //                            {
        //                                logger.Info("Key : " + _key + " Value : " + kvColl[_key].ToString());
        //                                if (!loadCollection.ContainsKey(_key))
        //                                {
        //                                    loadCollection.Add(_key, kvColl[_key].ToString());
        //                                }
        //                            }
        //                            else if (string.Compare(_key, "email.enable.case-data-filter", true) == 0)
        //                            {
        //                                logger.Info("Key : " + _key + " Value : " + kvColl[_key].ToString());
        //                                if (!loadCollection.ContainsKey(_key))
        //                                {
        //                                    loadCollection.Add(_key, kvColl[_key].ToString());
        //                                }
        //                            }
        //                            else if (string.Compare(_key, "chat.enable.case-data-filter", true) == 0)
        //                            {
        //                                logger.Info("Key : " + _key + " Value : " + kvColl[_key].ToString());
        //                                if (!loadCollection.ContainsKey(_key))
        //                                {
        //                                    loadCollection.Add(_key, kvColl[_key].ToString());
        //                                }
        //                            }

        //                        }
        //                    }
        //                }
        //                #endregion

        //                //KeyValueCollection systemvalues = (KeyValueCollection)agentGroup.GroupInfo.UserProperties["_system_"];
        //                //if (systemvalues != null && systemvalues.Count > 0)
        //                //{
        //                //    if (systemvalues.ContainsKey("image-path") && !loadCollection.ContainsKey("image-path"))
        //                //    {
        //                //        if (!string.IsNullOrEmpty(personixnValues["image-path"].ToString()))
        //                //        {
        //                //            loadCollection.Add("image-path", personixnValues["image-path"].ToString());
        //                //            logger.Info("ReadPersonObject key:image-path:" + personixnValues["image-path"].ToString());
        //                //        }
        //                //    }
        //                //}
        //                }
        //            }

        //        }
        //    }

        //    catch (Exception commonException)
        //    {
        //        logger.Error("Error occurred while ReadAgentGroupValues() " + commonException.ToString());
        //    }
        //    finally
        //    {
        //        //GC.Collect();
        //    }
        //}
        #endregion

        #region ReadApplicationObject
        /// <summary>
        /// Reads the application.
        /// </summary>
        /// <param name="applicationName">Name of the application.</param>
        //private void ReadApplication(string applicationName)
        //{
        //    try
        //    {
        //        application = new CfgApplication(ContactDataContext.ComObject);
        //        CfgApplicationQuery queryApp = new CfgApplicationQuery();
        //        queryApp.TenantDbid =ConfigContainer.Instance().TenantDbId;
        //        queryApp.Name = applicationName;
        //        application = ContactDataContext.ComObject.RetrieveObject<CfgApplication>(queryApp);
        //        if (application != null)
        //        {
        //            logger.Debug("Retrieving Values from Application " + applicationName);
        //            string[] applicationKeys = application.Options.AllKeys;

        //            foreach (string section in applicationKeys)
        //            {
        //                #region agent.ixn.desktop
        //                if (string.Compare(section, "agent.ixn.desktop", true) == 0)
        //                {
        //                    KeyValueCollection systemValues = (KeyValueCollection)application.Options[section];
        //                    logger.Debug("Retrieving Values from agent.ixn.desktop Section");
        //                    foreach (string key in systemValues.AllKeys)
        //                    {
        //                        if (string.Compare(key, "contact.available-directory-page-sizes", true) == 0)
        //                        {
        //                            logger.Info("Key : " + key + " Value : " + systemValues[key].ToString());
        //                            if (personixnValues != null)
        //                            {
        //                                if (!personixnValues.ContainsKey(key))
        //                                {
        //                                    if (!loadCollection.ContainsKey(key))
        //                                    {
        //                                        loadCollection.Add(key, systemValues[key].ToString());
        //                                    }
        //                                }
        //                                else if (personixnValues[key].ToString() == "")
        //                                {
        //                                    loadCollection.Remove(key);
        //                                    loadCollection.Add(key, systemValues[key].ToString());
        //                                }
        //                            }
        //                            else
        //                            {
        //                                if (!loadCollection.ContainsKey(key))
        //                                {
        //                                    loadCollection.Add(key, systemValues[key].ToString());
        //                                }
        //                            }
        //                        }
        //                        else if (string.Compare(key, "contact.date-search-types", true) == 0)
        //                        {
        //                            logger.Info("Key : " + key + " Value : " + systemValues[key].ToString());
        //                            if (personixnValues != null)
        //                            {
        //                                if (!personixnValues.ContainsKey(key))
        //                                {
        //                                    if (!loadCollection.ContainsKey(key))
        //                                    {
        //                                        loadCollection.Add(key, systemValues[key].ToString());
        //                                    }
        //                                }
        //                                else if (personixnValues[key].ToString() == "")
        //                                {
        //                                    loadCollection.Remove(key);
        //                                    loadCollection.Add(key, systemValues[key].ToString());
        //                                }
        //                            }
        //                            else
        //                            {
        //                                if (!loadCollection.ContainsKey(key))
        //                                {
        //                                    loadCollection.Add(key, systemValues[key].ToString());
        //                                }
        //                            }
        //                        }
        //                        else if (string.Compare(key, "email.standard-response.default-search-type", true) == 0)
        //                        {
        //                            logger.Info("Key : " + key + " Value : " + systemValues[key].ToString());
        //                            if (personixnValues != null)
        //                            {
        //                                if (!personixnValues.ContainsKey(key))
        //                                {
        //                                    if (!loadCollection.ContainsKey(key))
        //                                    {
        //                                        loadCollection.Add(key, systemValues[key].ToString());
        //                                    }
        //                                }
        //                                else if (personixnValues[key].ToString() == "")
        //                                {
        //                                    loadCollection.Remove(key);
        //                                    loadCollection.Add(key, systemValues[key].ToString());
        //                                }
        //                            }
        //                            else
        //                            {
        //                                if (!loadCollection.ContainsKey(key))
        //                                {
        //                                    loadCollection.Add(key, systemValues[key].ToString());
        //                                }
        //                            }
        //                        }
        //                        else if (string.Compare(key, "contact.default-directory-page-size", true) == 0)
        //                        {
        //                            logger.Info("Key : " + key + " Value : " + systemValues[key].ToString());
        //                            if (personixnValues != null)
        //                            {
        //                                if (!personixnValues.ContainsKey(key))
        //                                {
        //                                    if (!loadCollection.ContainsKey(key))
        //                                    {
        //                                        loadCollection.Add(key, systemValues[key].ToString());
        //                                    }
        //                                }
        //                                else if (personixnValues[key].ToString() == "")
        //                                {
        //                                    loadCollection.Remove(key);
        //                                    loadCollection.Add(key, systemValues[key].ToString());
        //                                }
        //                            }
        //                            else
        //                            {
        //                                if (!loadCollection.ContainsKey(key))
        //                                {
        //                                    loadCollection.Add(key, systemValues[key].ToString());
        //                                }
        //                            }
        //                        }
        //                        else if (string.Compare(key, "contact.displayed-attributes", true) == 0)
        //                        {
        //                            logger.Info("Key : " + key + " Value : " + systemValues[key].ToString());
        //                            if (personixnValues != null)
        //                            {
        //                                if (!personixnValues.ContainsKey(key))
        //                                {
        //                                    if (!loadCollection.ContainsKey(key))
        //                                    {
        //                                        loadCollection.Add(key, systemValues[key].ToString());
        //                                    }
        //                                }
        //                                else if (personixnValues[key].ToString() == "")
        //                                {
        //                                    loadCollection.Remove(key);
        //                                    loadCollection.Add(key, systemValues[key].ToString());
        //                                }
        //                            }
        //                            else
        //                            {
        //                                if (!loadCollection.ContainsKey(key))
        //                                {
        //                                    loadCollection.Add(key, systemValues[key].ToString());
        //                                }
        //                            }
        //                        }
        //                        else if (string.Compare(key, "contact.directory-displayed-columns", true) == 0)
        //                        {
        //                            logger.Info("Key : " + key + " Value : " + systemValues[key].ToString());
        //                            if (personixnValues != null)
        //                            {
        //                                if (!personixnValues.ContainsKey(key))
        //                                {
        //                                    if (!loadCollection.ContainsKey(key))
        //                                    {
        //                                        loadCollection.Add(key, systemValues[key].ToString());
        //                                    }
        //                                }
        //                                else if (personixnValues[key].ToString() == "")
        //                                {
        //                                    loadCollection.Remove(key);
        //                                    loadCollection.Add(key, systemValues[key].ToString());
        //                                }
        //                            }
        //                            else
        //                            {
        //                                if (!loadCollection.ContainsKey(key))
        //                                {
        //                                    loadCollection.Add(key, systemValues[key].ToString());
        //                                }
        //                            }
        //                        }
        //                        else if (string.Compare(key, "contact.history-default-time-filter-main", true) == 0)
        //                        {
        //                            logger.Info("Key : " + key + " Value : " + systemValues[key].ToString());
        //                            if (personixnValues != null)
        //                            {
        //                                if (!personixnValues.ContainsKey(key))
        //                                {
        //                                    if (!loadCollection.ContainsKey(key))
        //                                    {
        //                                        loadCollection.Add(key, systemValues[key].ToString());
        //                                    }
        //                                }
        //                                else if (personixnValues[key].ToString() == "")
        //                                {
        //                                    loadCollection.Remove(key);
        //                                    loadCollection.Add(key, systemValues[key].ToString());
        //                                }
        //                            }
        //                            else
        //                            {
        //                                if (!loadCollection.ContainsKey(key))
        //                                {
        //                                    loadCollection.Add(key, systemValues[key].ToString());
        //                                }
        //                            }
        //                        }
        //                        else if (string.Compare(key, "contact.myhistory-default-time-filter-main", true) == 0)
        //                        {
        //                            logger.Info("Key : " + key + " Value : " + systemValues[key].ToString());
        //                            if (personixnValues != null)
        //                            {
        //                                if (!personixnValues.ContainsKey(key))
        //                                {
        //                                    if (!loadCollection.ContainsKey(key))
        //                                    {
        //                                        loadCollection.Add(key, systemValues[key].ToString());
        //                                    }
        //                                }
        //                                else if (personixnValues[key].ToString() == "")
        //                                {
        //                                    loadCollection.Remove(key);
        //                                    loadCollection.Add(key, systemValues[key].ToString());
        //                                }
        //                            }
        //                            else
        //                            {
        //                                if (!loadCollection.ContainsKey(key))
        //                                {
        //                                    loadCollection.Add(key, systemValues[key].ToString());
        //                                }
        //                            }
        //                        }
        //                        else if (string.Compare(key, "contact.history-displayed-columns", true) == 0)
        //                        {
        //                            logger.Info("Key : " + key + " Value : " + systemValues[key].ToString());
        //                            if (personixnValues != null)
        //                            {
        //                                if (!personixnValues.ContainsKey(key))
        //                                {
        //                                    if (!loadCollection.ContainsKey(key))
        //                                    {
        //                                        loadCollection.Add(key, systemValues[key].ToString());
        //                                    }
        //                                }
        //                                else if (personixnValues[key].ToString() == "")
        //                                {
        //                                    loadCollection.Remove(key);
        //                                    loadCollection.Add(key, systemValues[key].ToString());
        //                                }
        //                            }
        //                            else
        //                            {
        //                                if (!loadCollection.ContainsKey(key))
        //                                {
        //                                    loadCollection.Add(key, systemValues[key].ToString());
        //                                }
        //                            }
        //                        }
        //                        else if (string.Compare(key, "contact.history-search-attributes", true) == 0)
        //                        {
        //                            logger.Info("Key : " + key + " Value : " + systemValues[key].ToString());
        //                            if (personixnValues != null)
        //                            {
        //                                if (!personixnValues.ContainsKey(key))
        //                                {
        //                                    if (!loadCollection.ContainsKey(key))
        //                                    {
        //                                        loadCollection.Add(key, systemValues[key].ToString());
        //                                    }
        //                                }
        //                                else if (personixnValues[key].ToString() == "")
        //                                {
        //                                    loadCollection.Remove(key);
        //                                    loadCollection.Add(key, systemValues[key].ToString());
        //                                }
        //                            }
        //                            else
        //                            {
        //                                if (!loadCollection.ContainsKey(key))
        //                                {
        //                                    loadCollection.Add(key, systemValues[key].ToString());
        //                                }
        //                            }
        //                        }
        //                        else if (string.Compare(key, "contact.directory-advanced-default", true) == 0)
        //                        {
        //                            logger.Info("Key : " + key + " Value : " + systemValues[key].ToString());
        //                            if (personixnValues != null)
        //                            {
        //                                if (!personixnValues.ContainsKey(key))
        //                                {
        //                                    if (!loadCollection.ContainsKey(key))
        //                                    {
        //                                        loadCollection.Add(key, systemValues[key].ToString());
        //                                    }
        //                                }
        //                                else if (personixnValues[key].ToString() == "")
        //                                {
        //                                    loadCollection.Remove(key);
        //                                    loadCollection.Add(key, systemValues[key].ToString());
        //                                }
        //                            }
        //                            else
        //                            {
        //                                if (!loadCollection.ContainsKey(key))
        //                                {
        //                                    loadCollection.Add(key, systemValues[key].ToString());
        //                                }
        //                            }
        //                        }
        //                        else if (string.Compare(key, "contact.history-default-time-filter-main", true) == 0)
        //                        {
        //                            logger.Info("Key : " + key + " Value : " + systemValues[key].ToString());
        //                            if (personixnValues != null)
        //                            {
        //                                if (!personixnValues.ContainsKey(key))
        //                                {
        //                                    if (!loadCollection.ContainsKey(key))
        //                                    {
        //                                        loadCollection.Add(key, systemValues[key].ToString());
        //                                    }
        //                                }
        //                                else if (personixnValues[key].ToString() == "")
        //                                {
        //                                    loadCollection.Remove(key);
        //                                    loadCollection.Add(key, systemValues[key].ToString());
        //                                }
        //                            }
        //                            else
        //                            {
        //                                if (!loadCollection.ContainsKey(key))
        //                                {
        //                                    loadCollection.Add(key, systemValues[key].ToString());
        //                                }
        //                            }
        //                        }

        //                        else if (string.Compare(key, "contact.directory-search-attributes", true) == 0)
        //                        {
        //                            logger.Info("Key : " + key + " Value : " + systemValues[key].ToString());
        //                            if (personixnValues != null)
        //                            {
        //                                if (!personixnValues.ContainsKey(key))
        //                                {
        //                                    if (!loadCollection.ContainsKey(key))
        //                                    {
        //                                        loadCollection.Add(key, systemValues[key].ToString());
        //                                    }
        //                                }
        //                                else if (personixnValues[key].ToString() == "")
        //                                {
        //                                    loadCollection.Remove(key);
        //                                    loadCollection.Add(key, systemValues[key].ToString());
        //                                }
        //                            }
        //                            else
        //                            {
        //                                if (!loadCollection.ContainsKey(key))
        //                                {
        //                                    loadCollection.Add(key, systemValues[key].ToString());
        //                                }
        //                            }
        //                        }
        //                        else if (string.Compare(key, "contact.directory-search-types", true) == 0)
        //                        {
        //                            logger.Info("Key : " + key + " Value : " + systemValues[key].ToString());
        //                            if (personixnValues != null)
        //                            {
        //                                if (!personixnValues.ContainsKey(key))
        //                                {
        //                                    if (!loadCollection.ContainsKey(key))
        //                                    {
        //                                        loadCollection.Add(key, systemValues[key].ToString());
        //                                    }
        //                                }
        //                                else if (personixnValues[key].ToString() == "")
        //                                {
        //                                    loadCollection.Remove(key);
        //                                    loadCollection.Add(key, systemValues[key].ToString());
        //                                }
        //                            }
        //                            else
        //                            {
        //                                if (!loadCollection.ContainsKey(key))
        //                                {
        //                                    loadCollection.Add(key, systemValues[key].ToString());
        //                                }
        //                            }
        //                        }
        //                        else if (string.Compare(key, "contact.history.media-filters", true) == 0)
        //                        {
        //                            logger.Info("Key : " + key + " Value : " + systemValues[key].ToString());
        //                            if (personixnValues != null)
        //                            {
        //                                if (!personixnValues.ContainsKey(key))
        //                                {
        //                                    if (!loadCollection.ContainsKey(key))
        //                                    {
        //                                        loadCollection.Add(key, systemValues[key].ToString());
        //                                    }
        //                                }
        //                                else if (personixnValues[key].ToString() == "")
        //                                {
        //                                    loadCollection.Remove(key);
        //                                    loadCollection.Add(key, systemValues[key].ToString());
        //                                }
        //                            }
        //                            else
        //                            {
        //                                if (!loadCollection.ContainsKey(key))
        //                                {
        //                                    loadCollection.Add(key, systemValues[key].ToString());
        //                                }
        //                            }
        //                        }
        //                        else if (string.Compare(key, "contact.mandatory-attributes", true) == 0)
        //                        {
        //                            logger.Info("Key : " + key + " Value : " + systemValues[key].ToString());
        //                            if (personixnValues != null)
        //                            {
        //                                if (!personixnValues.ContainsKey(key))
        //                                {
        //                                    if (!loadCollection.ContainsKey(key))
        //                                    {
        //                                        loadCollection.Add(key, systemValues[key].ToString());
        //                                    }
        //                                }
        //                                else if (personixnValues[key].ToString() == "")
        //                                {
        //                                    loadCollection.Remove(key);
        //                                    loadCollection.Add(key, systemValues[key].ToString());
        //                                }
        //                            }
        //                            else
        //                            {
        //                                if (!loadCollection.ContainsKey(key))
        //                                {
        //                                    loadCollection.Add(key, systemValues[key].ToString());
        //                                }
        //                            }
        //                        }
        //                        else if (string.Compare(key, "contact.multiple-value-attributes", true) == 0)
        //                        {
        //                            logger.Info("Key : " + key + " Value : " + systemValues[key].ToString());
        //                            if (personixnValues != null)
        //                            {
        //                                if (!personixnValues.ContainsKey(key))
        //                                {
        //                                    if (!loadCollection.ContainsKey(key))
        //                                    {
        //                                        loadCollection.Add(key, systemValues[key].ToString());
        //                                    }
        //                                }
        //                                else if (personixnValues[key].ToString() == "")
        //                                {
        //                                    loadCollection.Remove(key);
        //                                    loadCollection.Add(key, systemValues[key].ToString());
        //                                }
        //                            }
        //                            else
        //                            {
        //                                if (!loadCollection.ContainsKey(key))
        //                                {
        //                                    loadCollection.Add(key, systemValues[key].ToString());
        //                                }
        //                            }
        //                        }
        //                        else if (string.Compare(key, "chat.agent.prompt-color", true) == 0)
        //                        {
        //                            logger.Info("Key : " + key + " Value : " + systemValues[key].ToString());
        //                            if (personixnValues != null)
        //                            {
        //                                if (!personixnValues.ContainsKey(key))
        //                                {
        //                                    if (!loadCollection.ContainsKey(key))
        //                                    {
        //                                        loadCollection.Add(key, systemValues[key].ToString());
        //                                    }
        //                                }
        //                                else if (personixnValues[key].ToString() == "")
        //                                {
        //                                    loadCollection.Remove(key);
        //                                    loadCollection.Add(key, systemValues[key].ToString());
        //                                }
        //                            }
        //                            else
        //                            {
        //                                if (!loadCollection.ContainsKey(key))
        //                                {
        //                                    loadCollection.Add(key, systemValues[key].ToString());
        //                                }
        //                            }
        //                        }
        //                        else if (string.Compare(key, "chat.agent.text-color", true) == 0)
        //                        {
        //                            logger.Info("Key : " + key + " Value : " + systemValues[key].ToString());
        //                            if (personixnValues != null)
        //                            {
        //                                if (!personixnValues.ContainsKey(key))
        //                                {
        //                                    if (!loadCollection.ContainsKey(key))
        //                                    {
        //                                        loadCollection.Add(key, systemValues[key].ToString());
        //                                    }
        //                                }
        //                                else if (personixnValues[key].ToString() == "")
        //                                {
        //                                    loadCollection.Remove(key);
        //                                    loadCollection.Add(key, systemValues[key].ToString());
        //                                }
        //                            }
        //                            else
        //                            {
        //                                if (!loadCollection.ContainsKey(key))
        //                                {
        //                                    loadCollection.Add(key, systemValues[key].ToString());
        //                                }
        //                            }
        //                        }
        //                        else if (string.Compare(key, "chat.client.prompt-color", true) == 0)
        //                        {
        //                            logger.Info("Key : " + key + " Value : " + systemValues[key].ToString());
        //                            if (personixnValues != null)
        //                            {
        //                                if (!personixnValues.ContainsKey(key))
        //                                {
        //                                    if (!loadCollection.ContainsKey(key))
        //                                    {
        //                                        loadCollection.Add(key, systemValues[key].ToString());
        //                                    }
        //                                }
        //                                else if (personixnValues[key].ToString() == "")
        //                                {
        //                                    loadCollection.Remove(key);
        //                                    loadCollection.Add(key, systemValues[key].ToString());
        //                                }
        //                            }
        //                            else
        //                            {
        //                                if (!loadCollection.ContainsKey(key))
        //                                {
        //                                    loadCollection.Add(key, systemValues[key].ToString());
        //                                }
        //                            }
        //                        }
        //                        else if (string.Compare(key, "chat.client.text-color", true) == 0)
        //                        {
        //                            logger.Info("Key : " + key + " Value : " + systemValues[key].ToString());
        //                            if (personixnValues != null)
        //                            {
        //                                if (!personixnValues.ContainsKey(key))
        //                                {
        //                                    if (!loadCollection.ContainsKey(key))
        //                                    {
        //                                        loadCollection.Add(key, systemValues[key].ToString());
        //                                    }
        //                                }
        //                                else if (personixnValues[key].ToString() == "")
        //                                {
        //                                    loadCollection.Remove(key);
        //                                    loadCollection.Add(key, systemValues[key].ToString());
        //                                }
        //                            }
        //                            else
        //                            {
        //                                if (!loadCollection.ContainsKey(key))
        //                                {
        //                                    loadCollection.Add(key, systemValues[key].ToString());
        //                                }
        //                            }
        //                        }
        //                        else if (string.Compare(key, "chat.other-agent.prompt-color", true) == 0)
        //                        {
        //                            logger.Info("Key : " + key + " Value : " + systemValues[key].ToString());
        //                            if (personixnValues != null)
        //                            {
        //                                if (!personixnValues.ContainsKey(key))
        //                                {
        //                                    if (!loadCollection.ContainsKey(key))
        //                                    {
        //                                        loadCollection.Add(key, systemValues[key].ToString());
        //                                    }
        //                                }
        //                                else if (personixnValues[key].ToString() == "")
        //                                {
        //                                    loadCollection.Remove(key);
        //                                    loadCollection.Add(key, systemValues[key].ToString());
        //                                }
        //                            }
        //                            else
        //                            {
        //                                if (!loadCollection.ContainsKey(key))
        //                                {
        //                                    loadCollection.Add(key, systemValues[key].ToString());
        //                                }
        //                            }
        //                        }
        //                        else if (string.Compare(key, "chat.other-agent.text-color", true) == 0)
        //                        {
        //                            logger.Info("Key : " + key + " Value : " + systemValues[key].ToString());
        //                            if (personixnValues != null)
        //                            {
        //                                if (!personixnValues.ContainsKey(key))
        //                                {
        //                                    if (!loadCollection.ContainsKey(key))
        //                                    {
        //                                        loadCollection.Add(key, systemValues[key].ToString());
        //                                    }
        //                                }
        //                                else if (personixnValues[key].ToString() == "")
        //                                {
        //                                    loadCollection.Remove(key);
        //                                    loadCollection.Add(key, systemValues[key].ToString());
        //                                }
        //                            }
        //                            else
        //                            {
        //                                if (!loadCollection.ContainsKey(key))
        //                                {
        //                                    loadCollection.Add(key, systemValues[key].ToString());
        //                                }
        //                            }
        //                        }
        //                        else if (string.Compare(key, "chat.time-stamp-format", true) == 0)
        //                        {
        //                            logger.Info("Key : " + key + " Value : " + systemValues[key].ToString());
        //                            if (personixnValues != null)
        //                            {
        //                                if (!personixnValues.ContainsKey(key))
        //                                {
        //                                    if (!loadCollection.ContainsKey(key))
        //                                    {
        //                                        loadCollection.Add(key, systemValues[key].ToString());
        //                                    }
        //                                }
        //                                else if (personixnValues[key].ToString() == "")
        //                                {
        //                                    loadCollection.Remove(key);
        //                                    loadCollection.Add(key, systemValues[key].ToString());
        //                                }
        //                            }
        //                            else
        //                            {
        //                                if (!loadCollection.ContainsKey(key))
        //                                {
        //                                    loadCollection.Add(key, systemValues[key].ToString());
        //                                }
        //                            }
        //                        }
        //                        else if (string.Compare(key, "contact.history-advanced-default", true) == 0)
        //                        {
        //                            logger.Info("Key : " + key + " Value : " + systemValues[key].ToString());
        //                            if (personixnValues != null)
        //                            {
        //                                if (!personixnValues.ContainsKey(key))
        //                                {
        //                                    if (!loadCollection.ContainsKey(key))
        //                                    {
        //                                        loadCollection.Add(key, systemValues[key].ToString());
        //                                    }
        //                                }
        //                                else if (personixnValues[key].ToString() == "")
        //                                {
        //                                    loadCollection.Remove(key);
        //                                    loadCollection.Add(key, systemValues[key].ToString());
        //                                }
        //                            }
        //                            else
        //                            {
        //                                if (!loadCollection.ContainsKey(key))
        //                                {
        //                                    loadCollection.Add(key, systemValues[key].ToString());
        //                                }
        //                            }
        //                        }
        //                        else if (string.Compare(key, "interaction.casedata-disposition-object", true) == 0)
        //                        {
        //                            logger.Info("Key : " + key + " Value : " + systemValues[key].ToString());
        //                            if (personixnValues != null)
        //                            {
        //                                if (!personixnValues.ContainsKey(key))
        //                                {
        //                                    if (!loadCollection.ContainsKey(key))
        //                                    {
        //                                        loadCollection.Add(key, systemValues[key].ToString());
        //                                    }
        //                                }
        //                                else if (personixnValues[key].ToString() == "")
        //                                {
        //                                    loadCollection.Remove(key);
        //                                    loadCollection.Add(key, systemValues[key].ToString());
        //                                }
        //                            }
        //                            else
        //                            {
        //                                if (!loadCollection.ContainsKey(key))
        //                                {
        //                                    loadCollection.Add(key, systemValues[key].ToString());
        //                                }
        //                            }
        //                        }
        //                        else if (string.Compare(key, "interaction.casedata.use-transaction-object", true) == 0)
        //                        {
        //                            logger.Info("Key : " + key + " Value : " + systemValues[key].ToString());
        //                            if (personixnValues != null)
        //                            {
        //                                if (!personixnValues.ContainsKey(key))
        //                                {
        //                                    if (!loadCollection.ContainsKey(key))
        //                                    {
        //                                        loadCollection.Add(key, systemValues[key].ToString());
        //                                    }
        //                                }
        //                                else if (personixnValues[key].ToString() == "")
        //                                {
        //                                    loadCollection.Remove(key);
        //                                    loadCollection.Add(key, systemValues[key].ToString());
        //                                }
        //                            }
        //                            else
        //                            {
        //                                if (!loadCollection.ContainsKey(key))
        //                                {
        //                                    loadCollection.Add(key, systemValues[key].ToString());
        //                                }
        //                            }
        //                        }
        //                        else if (string.Compare(key, "interaction.case-data.transaction-object", true) == 0)
        //                        {
        //                            logger.Info("Key : " + key + " Value : " + systemValues[key].ToString());
        //                            if (personixnValues != null)
        //                            {
        //                                if (!personixnValues.ContainsKey(key))
        //                                {
        //                                    if (!loadCollection.ContainsKey(key))
        //                                    {
        //                                        loadCollection.Add(key, systemValues[key].ToString());
        //                                    }
        //                                }
        //                                else if (personixnValues[key].ToString() == "")
        //                                {
        //                                    loadCollection.Remove(key);
        //                                    loadCollection.Add(key, systemValues[key].ToString());
        //                                }
        //                            }
        //                            else
        //                            {
        //                                if (!loadCollection.ContainsKey(key))
        //                                {
        //                                    loadCollection.Add(key, systemValues[key].ToString());
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //                #endregion

        //                #region enable-disable-channels

        //                else if (string.Compare(section, "enable-disable-channels", true) == 0)
        //                {
        //                    logger.Debug("Retrieving values from enable-disable-channels Section");
        //                    KeyValueCollection enabledisableValues = (KeyValueCollection)application.Options[section];
        //                    if (enabledisableValues != null && enabledisableValues.Count > 0)
        //                    {
        //                        string key="email.enable.pull-from-history";
        //                        if (enabledisableValues.ContainsKey(key) && !string.IsNullOrEmpty(enabledisableValues[key].ToString()))
        //                        {
        //                            loadCollection.Add(key, enabledisableValues[key].ToString());
        //                        }
        //                        key = "voice.enable.case-data-filter";
        //                        if (enabledisableValues.ContainsKey(key) && !string.IsNullOrEmpty(enabledisableValues[key].ToString()))
        //                        {
        //                            loadCollection.Add(key, enabledisableValues[key].ToString());
        //                        }
        //                        key = "email.enable.case-data-filter";
        //                        if (enabledisableValues.ContainsKey(key) && !string.IsNullOrEmpty(enabledisableValues[key].ToString()))
        //                        {
        //                            loadCollection.Add(key, enabledisableValues[key].ToString());
        //                        }
        //                        key = "chat.enable.case-data-filter";
        //                        if (enabledisableValues.ContainsKey(key) && !string.IsNullOrEmpty(enabledisableValues[key].ToString()))
        //                        {
        //                            loadCollection.Add(key, enabledisableValues[key].ToString());
        //                        }
        //                    }
        //                }
        //                #endregion

        //                else if (string.Compare(section, "_system_", true) == 0)
        //                {
        //                    KeyValueCollection systemvalues = (KeyValueCollection)application.Options["_system_"];
        //                    if (systemvalues != null && systemvalues.Count > 0)
        //                    {
        //                        if (systemvalues.ContainsKey("image-path") && !loadCollection.ContainsKey("image-path"))
        //                        {
        //                            if (!string.IsNullOrEmpty(systemvalues["image-path"].ToString()))
        //                            {
        //                                loadCollection.Add("image-path", systemvalues["image-path"].ToString());
        //                                logger.Info("ReadPersonObject key:image-path:" + systemvalues["image-path"].ToString());
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //        else
        //        {
        //            logger.Info("The Application is not found ");
        //        }
        //    }

        //    catch (Exception commonException)
        //    {
        //        logger.Error("Error occurred while reading ReadApplication()" + applicationName + "  =  " + commonException.ToString());
        //    }
        //    finally
        //    {
        //        GC.Collect();
        //    }
        //}
        #endregion ReadApplicationObject
        //public void ReadCaseDataFromBusinessAttribute(string businessAttributeName)
        //{
        //    KeyValueCollection emailCaseDataFilterUserProperties = new KeyValueCollection();
        //    KeyValueCollection chatCaseDataFilterUserProperties = new KeyValueCollection();
        //    KeyValueCollection voiceCaseDataFilterUserProperties = new KeyValueCollection();
        //    try
        //    {
        //        loadEmailCaseDataFilterKey.Clear();
        //        loadChatCaseDataFilterKey.Clear();
        //        loadVoiceCaseDataFilterKey.Clear();
        //        logger.Debug("Retrieving values from Case Data Business Attribute Object : Transaction Name : " + businessAttributeName);
        //        CfgEnumeratorQuery businessAttributeQuery = new CfgEnumeratorQuery();
        //        businessAttributeQuery.Name = businessAttributeName;
        //        businessAttributeQuery.EnumeratorType = Convert.ToInt32(Genesyslab.Platform.Configuration.Protocols.Types.CfgEnumeratorType.CFGENTInteractionOperationalAttribute);
        //        businessAttributeQuery.TenantDbid =ConfigContainer.Instance().TenantDbId;

        //        var businessAttribute = ContactDataContext.ComObject.RetrieveObject<CfgEnumerator>(businessAttributeQuery);
        //        if (businessAttribute != null)
        //        {
        //            emailCaseDataFilterUserProperties = (KeyValueCollection)businessAttribute.UserProperties["email.case-data-filter"];
        //            chatCaseDataFilterUserProperties = (KeyValueCollection)businessAttribute.UserProperties["chat.case-data-filter"];
        //            voiceCaseDataFilterUserProperties = (KeyValueCollection)businessAttribute.UserProperties["voice.case-data-filter"];
        //            if (emailCaseDataFilterUserProperties != null && emailCaseDataFilterUserProperties.Count > 0)
        //            {
        //                foreach (string key in emailCaseDataFilterUserProperties.AllKeys)
        //                {
        //                    loadEmailCaseDataFilterKey.Add(emailCaseDataFilterUserProperties[key].ToString());
        //                }
        //            }
        //            if (chatCaseDataFilterUserProperties != null && chatCaseDataFilterUserProperties.Count > 0)
        //            {
        //                foreach (string key in chatCaseDataFilterUserProperties.AllKeys)
        //                {
        //                    loadChatCaseDataFilterKey.Add(chatCaseDataFilterUserProperties[key].ToString());
        //                }
        //            }
        //            if (voiceCaseDataFilterUserProperties != null && voiceCaseDataFilterUserProperties.Count > 0)
        //            {
        //                foreach (string key in voiceCaseDataFilterUserProperties.AllKeys)
        //                {
        //                    loadVoiceCaseDataFilterKey.Add(voiceCaseDataFilterUserProperties[key].ToString());
        //                }
        //            }
        //            emailCaseDataFilterUserProperties.Clear();
        //            chatCaseDataFilterUserProperties.Clear();
        //            voiceCaseDataFilterUserProperties.Clear();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.Error((ex.InnerException == null) ? ex.Message : ex.InnerException.ToString());
        //    }
        //}
        #region ReadCaseDataTransactionObject
        //public void ReadCaseDataTransactionObject(string name)
        //{
        //    KeyValueCollection emailCaseDataFilterUserProperties = new KeyValueCollection();
        //    KeyValueCollection chatCaseDataFilterUserProperties = new KeyValueCollection();
        //    KeyValueCollection voiceCaseDataFilterUserProperties = new KeyValueCollection();
        //    try
        //    {
        //        loadEmailCaseDataFilterKey.Clear();
        //        loadChatCaseDataFilterKey.Clear();
        //        loadVoiceCaseDataFilterKey.Clear();
        //        logger.Debug("Retrieving values from Case Data Transaction Object : Transaction Name : " + name);
        //        CfgTransaction Transaction;
        //        CfgTransactionQuery qTransaction = new CfgTransactionQuery();
        //        qTransaction.TenantDbid =ConfigContainer.Instance().TenantDbId;
        //        qTransaction.Name = name;
        //        Transaction = (CfgTransaction)ContactDataContext.ComObject.RetrieveObject<CfgTransaction>(qTransaction);
        //        emailCaseDataFilterUserProperties = (KeyValueCollection)Transaction.UserProperties["email.case-data-filter"];
        //        chatCaseDataFilterUserProperties = (KeyValueCollection)Transaction.UserProperties["chat.case-data-filter"];
        //        voiceCaseDataFilterUserProperties = (KeyValueCollection)Transaction.UserProperties["voice.case-data-filter"];
        //        if (emailCaseDataFilterUserProperties != null && emailCaseDataFilterUserProperties.Count > 0)
        //        {
        //            foreach (string key in emailCaseDataFilterUserProperties.AllKeys)
        //            {
        //                loadEmailCaseDataFilterKey.Add(emailCaseDataFilterUserProperties[key].ToString());
        //            }
        //        }
        //        if (chatCaseDataFilterUserProperties != null && chatCaseDataFilterUserProperties.Count > 0)
        //        {
        //            foreach (string key in chatCaseDataFilterUserProperties.AllKeys)
        //            {
        //                loadChatCaseDataFilterKey.Add(chatCaseDataFilterUserProperties[key].ToString());
        //            }
        //        }
        //        if (voiceCaseDataFilterUserProperties != null && voiceCaseDataFilterUserProperties.Count > 0)
        //        {
        //            foreach (string key in voiceCaseDataFilterUserProperties.AllKeys)
        //            {
        //                loadVoiceCaseDataFilterKey.Add(voiceCaseDataFilterUserProperties[key].ToString());
        //            }
        //        }
        //        emailCaseDataFilterUserProperties.Clear();
        //        chatCaseDataFilterUserProperties.Clear();
        //        voiceCaseDataFilterUserProperties.Clear();
        //    }
        //    catch (Exception commonException)
        //    {
        //        logger.Error("Error occurred while reading CaseDataTransactionObject " + name + "  =  " + commonException.ToString());
        //    }
        //}
        #endregion ReadCaseDataTransactionObject

        #region Get all KVP's based on the hierarchical level
        public void GetAllValues()
        {
            try
            {
                //   ReadPersonDetails(ContactDataContext.GetInstance().UserName);
                //ReadApplication(ContactDataContext.GetInstance().ApplicationName);
                //if (loadCollection.ContainsKey("image-path"))
                //{
                //    ConfigContainer.Instance().GetValue("image-path") = loadCollection["image-path"].ToString();
                //}
                //if (loadCollection.ContainsKey("chat.agent.prompt-color"))
                //{
                //    ContactDataContext.GetInstance().AgentPromptColor = Color.FromName(loadCollection["chat.agent.prompt-color"].ToString());
                //}
                //if (loadCollection.ContainsKey("chat.agent.text-color"))
                //{
                //    ContactDataContext.GetInstance().AgentTextColor = Color.FromName(loadCollection["chat.agent.text-color"].ToString());
                //}
                //if (loadCollection.ContainsKey("chat.client.prompt-color"))
                //{
                //    ContactDataContext.GetInstance().ClientPromptColor = Color.FromName(loadCollection["chat.client.prompt-color"].ToString());
                //}
                //if (loadCollection.ContainsKey("chat.client.text-color"))
                //{
                //    ContactDataContext.GetInstance().ClientTextColor = Color.FromName(loadCollection["chat.client.text-color"].ToString());
                //}
                //if (loadCollection.ContainsKey("chat.other-agent.prompt-color"))
                //{
                //    ContactDataContext.GetInstance().OtherAgentPromptColor = Color.FromName(loadCollection["chat.other-agent.prompt-color"].ToString());
                //}
                //if (loadCollection.ContainsKey("chat.other-agent.text-color"))
                //{
                //    ContactDataContext.GetInstance().OtherAgentTextColor = Color.FromName(loadCollection["chat.other-agent.text-color"].ToString());
                //}
                //if (loadCollection.ContainsKey("chat.time-stamp-format"))
                //{
                //    ContactDataContext.GetInstance().ChatTimeStampFormat = loadCollection["chat.time-stamp-format"].ToString();
                //}
                //if (loadCollection.ContainsKey("contact.default-directory-page-size"))
                //{
                //    ContactDataContext.GetInstance().DefaultDirPageSize = loadCollection["contact.default-directory-page-size"].ToString();
                ////}
                //if (loadCollection.ContainsKey("email.enable.pull-from-history"))
                //{
                //    ContactDataContext.GetInstance().IsEnablePullFromHistory = Convert.ToBoolean(loadCollection["email.enable.pull-from-history"]);
                //}
                //if (loadCollection.ContainsKey("contact.available-directory-page-sizes"))
                //{
                //    string pageSize = loadCollection["contact.available-directory-page-sizes"].ToString();
                //    if (!string.IsNullOrEmpty(pageSize))
                //    {
                //        string[] pageSizes = pageSize.Split(',');
                //        if (pageSizes.Length > 0)
                //        {
                //            ContactDataContext.GetInstance().AvailableDirPageSizes = pageSizes.Distinct().Where(x => !string.IsNullOrEmpty(x) && !string.IsNullOrWhiteSpace(x)).ToList();
                //        }
                //    }
                //}
                //if (loadCollection.ContainsKey("contact.date-search-types"))
                //{
                //    string dateSearchType = loadCollection["contact.date-search-types"].ToString();
                //    if (dateSearchType != string.Empty)
                //    {
                //        string[] dateSearchTypes = dateSearchType.Split(',');
                //        if (dateSearchTypes.Length > 0)
                //        {
                //            ContactDataContext.GetInstance().DateSearchTypes = dateSearchTypes.Distinct().Where(x => !string.IsNullOrEmpty(x) && !string.IsNullOrWhiteSpace(x)).ToList();
                //            dateSearchType = null;
                //            dateSearchTypes = null;
                //        }
                //    }
                //}
                //if (loadCollection.ContainsKey("contact.displayed-attributes"))
                //{
                //    string displayedAttribute = loadCollection["contact.displayed-attributes"].ToString();
                //    if (displayedAttribute != string.Empty)
                //    {
                //        string[] displayedAttributes = displayedAttribute.Split(',');
                //        if (displayedAttributes.Length > 0)
                //        {
                //            ContactDataContext.GetInstance().DisplayedAttributes = displayedAttributes.Distinct().Where(x => !string.IsNullOrEmpty(x) && !string.IsNullOrWhiteSpace(x)).ToList();
                //            displayedAttribute = null;
                //            displayedAttributes = null;
                //        }
                //    }
                //}
                //if (loadCollection.ContainsKey("contact.directory-displayed-columns"))
                //{
                //    string displayedAttribute = loadCollection["contact.directory-displayed-columns"].ToString();
                //    if (displayedAttribute != string.Empty)
                //    {
                //        string[] displayedAttributes = displayedAttribute.Split(',');
                //        if (displayedAttributes.Length > 0)
                //        {
                //            ContactDataContext.GetInstance().DirectoryDisplayedColumns = displayedAttributes.Distinct().Where(x => !string.IsNullOrEmpty(x) && !string.IsNullOrWhiteSpace(x)).ToList();
                //            displayedAttribute = null;
                //            displayedAttributes = null;
                //        }
                //    }
                //}
                //if (loadCollection.ContainsKey("contact.history-default-time-filter-main"))
                //{
                //    ContactDataContext.GetInstance().DefaultHistoryTimeFilter = loadCollection["contact.history-default-time-filter-main"].ToString();
                //}
                //if (loadCollection.ContainsKey("contact.history-displayed-columns"))
                //{
                //    string displayedColumn = loadCollection["contact.history-displayed-columns"].ToString();
                //    if (displayedColumn != string.Empty)
                //    {
                //        string[] displayedColumns = displayedColumn.Split(',');
                //        if (displayedColumns.Length > 0)
                //        {
                //            ContactDataContext.GetInstance().HistoryDisplayedColumns = displayedColumns.Distinct().Where(x => !string.IsNullOrEmpty(x) && !string.IsNullOrWhiteSpace(x)).ToList();
                //            displayedColumn = null;
                //            displayedColumns = null;
                //        }
                //    }
                //}
                //if (loadCollection.ContainsKey("contact.history.media-filters"))
                //{
                //    string mediaFilter = loadCollection["contact.history.media-filters"].ToString();
                //    if (mediaFilter != string.Empty)
                //    {
                //        string[] mediaFilters = mediaFilter.Split(',');
                //        if (mediaFilters.Length > 0)
                //        {
                //            ContactDataContext.GetInstance().HistoryMediaFilters = mediaFilters.Distinct().Where(x => !string.IsNullOrEmpty(x) && !string.IsNullOrWhiteSpace(x)).ToList();
                //            mediaFilter = null;
                //            mediaFilters = null;
                //        }
                //    }
                //}
                //if (loadCollection.ContainsKey("contact.mandatory-attributes"))
                //{
                //    string mandatoryAttribute = loadCollection["contact.mandatory-attributes"].ToString();
                //    if (mandatoryAttribute != string.Empty)
                //    {
                //        string[] mandatoryAttributes = mandatoryAttribute.Split(',');
                //        if (mandatoryAttributes.Length > 0)
                //        {
                //            ContactDataContext.GetInstance().MandatoryAttributes = mandatoryAttributes.Distinct().Where(x => !string.IsNullOrEmpty(x) && !string.IsNullOrWhiteSpace(x)).ToList();
                //            mandatoryAttribute = null;
                //            mandatoryAttributes = null;
                //        }
                //    }
                //}
                //if (loadCollection.ContainsKey("contact.multiple-value-attributes"))
                //{
                //    string multiValueAttribute = loadCollection["contact.multiple-value-attributes"].ToString();
                //    if (multiValueAttribute != string.Empty)
                //    {
                //        string[] multiValueAttributes = multiValueAttribute.Split(',');
                //        if (multiValueAttributes.Length > 0)
                //        {
                //            ContactDataContext.GetInstance().MultipleValueAttributes = multiValueAttributes.Distinct().Where(x => !string.IsNullOrEmpty(x) && !string.IsNullOrWhiteSpace(x)).ToList();
                //            multiValueAttribute = null;
                //            multiValueAttributes = null;
                //        }
                //    }
                //}
                //if (loadCollection.ContainsKey("contact.history-advanced-default"))
                //{
                //    string historyAdvancedDefault = loadCollection["contact.history-advanced-default"].ToString();
                //    if (historyAdvancedDefault != string.Empty)
                //    {
                //        string[] historyAdvancedDefaults = historyAdvancedDefault.Split(',');
                //        if (historyAdvancedDefaults.Length > 0)
                //        {
                //            ContactDataContext.GetInstance().HistoryAdvancedDefault = historyAdvancedDefaults.Distinct().Where(x => !string.IsNullOrEmpty(x) && !string.IsNullOrWhiteSpace(x)).ToList();
                //            historyAdvancedDefault = null;
                //            historyAdvancedDefaults = null;
                //        }
                //    }
                //}
                //if (loadCollection.ContainsKey("contact.history-search-attributes"))
                //{
                //    string searchAttribute = loadCollection["contact.history-search-attributes"].ToString();
                //    if (searchAttribute != string.Empty)
                //    {
                //        string[] searchAttributes = searchAttribute.Split(',');
                //        if (searchAttributes.Length > 0)
                //        {
                //            foreach (string item in searchAttributes.Distinct().Where(x => !string.IsNullOrEmpty(x) && !string.IsNullOrWhiteSpace(x)).ToList())
                //            {
                //                ComboBoxItem cmbItem = new ComboBoxItem() { Content = item };
                //                if (ContactDataContext.GetInstance().HistoryAdvancedDefault.Contains(item))
                //                    cmbItem.Tag = "Default";
                //                ContactDataContext.GetInstance().HistorySearchAttributes.Add(cmbItem);
                //            }

                //            searchAttribute = null;
                //            searchAttributes = null;
                //        }
                //    }
                //}
                //if (loadCollection.ContainsKey("contact.directory-advanced-default"))
                //{
                //    string directoryAdvancedDefault = loadCollection["contact.directory-advanced-default"].ToString();
                //    if (directoryAdvancedDefault != string.Empty)
                //    {
                //        string[] directoryAdvancedDefaults = directoryAdvancedDefault.Split(',');
                //        if (directoryAdvancedDefaults.Length > 0)
                //        {
                //            ContactDataContext.GetInstance().DirectoryAdvancedDefault = directoryAdvancedDefaults.Distinct().Where(x => !string.IsNullOrEmpty(x) && !string.IsNullOrWhiteSpace(x)).ToList();
                //            directoryAdvancedDefault = null;
                //            directoryAdvancedDefaults = null;
                //        }
                //    }
                //}

                //if (loadCollection.ContainsKey("contact.directory-search-types"))
                //{
                //    string directorySearchType = loadCollection["contact.directory-search-types"].ToString();
                //    if (directorySearchType != string.Empty)
                //    {
                //        string[] directorySearchTypes = directorySearchType.Split(',');
                //        if (directorySearchTypes.Length > 0)
                //        {
                //            ContactDataContext.GetInstance().DirectorySearchType = directorySearchTypes.Distinct().Where(x => !string.IsNullOrEmpty(x) && !string.IsNullOrWhiteSpace(x)).ToList();
                //            directorySearchType = null;
                //            directorySearchTypes = null;
                //        }
                //    }
                //}

                //if (loadCollection.ContainsKey("contact.history-default-time-filter-main"))
                //{

                //    if (loadCollection["contact.history-default-time-filter-main"].ToString() != string.Empty)
                //    {
                //        ContactDataContext.GetInstance().HistoryDefaultTimeFilter = loadCollection["contact.history-default-time-filter-main"].ToString();
                //    }
                //}

                //if (loadCollection.ContainsKey("contact.myhistory-default-time-filter-main"))
                //{

                //    if (loadCollection["contact.myhistory-default-time-filter-main"].ToString() != string.Empty)
                //    {
                //        ContactDataContext.GetInstance().MyHistoryDefaultTimeFilter = loadCollection["contact.myhistory-default-time-filter-main"].ToString();
                //    }
                //}

                //if (loadCollection.ContainsKey("contact.directory-search-attributes"))
                //{
                //    string searchAttribute = loadCollection["contact.directory-search-attributes"].ToString();
                //    if (searchAttribute != string.Empty)
                //    {
                //        string[] searchAttributes = searchAttribute.Split(',');
                //        if (searchAttributes.Length > 0)
                //        {
                //            foreach (string item in searchAttributes.Distinct().Where(x => !string.IsNullOrEmpty(x) && !string.IsNullOrWhiteSpace(x)).ToList())
                //            {
                //                ComboBoxItem cmbItem = new ComboBoxItem() { Content = ContactDataContext.GetInstance().ContactValidAttribute[item] };

                //                //if (ContactDataContext.GetInstance().DirectoryAdvancedDefault.Contains(item))
                //                    cmbItem.Tag = "Default";
                //              //  ContactDataContext.GetInstance().DirectoryAttributesSearch.Add(cmbItem);
                //            }

                //            searchAttribute = null;
                //            searchAttributes = null;
                //        }
                //    }
                //}

                //if (loadCollection.ContainsKey("contact.directory-search-types"))
                //{
                //    string searchType = loadCollection["contact.directory-search-types"].ToString();
                //    if (searchType != string.Empty)
                //    {
                //        string[] searchTypes = searchType.Split(',');
                //        if (searchTypes.Length > 0)
                //        {
                //            foreach (string item in searchTypes.Distinct().Where(x => !string.IsNullOrEmpty(x) && !string.IsNullOrWhiteSpace(x)).ToList())
                //            {
                //                ComboBoxItem cmbItem = new ComboBoxItem() { Content = item };
                //                ContactDataContext.GetInstance().DirectoryAttributesSearchType.Add(cmbItem);
                //            }

                //            searchType = null;
                //            searchTypes = null;
                //        }
                //    }
                //}








                //if (loadCollection.ContainsKey("interaction.case-data.transaction-object"))
                //{
                //    ContactDataContext.GetInstance().CaseDataTransactionObject = loadCollection["interaction.case-data.transaction-object"].ToString();
                //}
                //if (loadCollection.ContainsKey("email.standard-response.default-search-type"))
                //{
                //    ContactDataContext.GetInstance().HistorySearchAttributess = loadCollection["email.standard-response.default-search-type"].ToString();
                //}

                //if (loadCollection.ContainsKey("interaction.casedata-disposition-object"))
                //{
                //    ContactDataContext.GetInstance().CaseDataDispositionObject = loadCollection["interaction.casedata-disposition-object"].ToString();
                //    logger.Info(ContactDataContext.GetInstance().CaseDataDispositionObject);
                //}
                //if (loadCollection.ContainsKey("interaction.casedata.use-transaction-object"))
                //{
                //    ContactDataContext.GetInstance().IsEnableCaseDataFromTransaction = loadCollection["interaction.casedata.use-transaction-object"].ToLower().Equals("true") ? true : false;
                //    logger.Info(ContactDataContext.GetInstance().IsEnableCaseDataFromTransaction);
                //}
                //bool voicecasedatafilter = false;
                //bool emailcasedatafilter = false;
                //bool chatcasedatafilter = false;

                //if (loadCollection.ContainsKey("voice.enable.case-data-filter"))
                //{
                //    voicecasedatafilter = loadCollection["voice.enable.case-data-filter"].ToLower().Equals("true");
                //    logger.Info(ContactDataContext.GetInstance().IsEnableCaseDataFromTransaction);
                //}
                //if (loadCollection.ContainsKey("email.enable.case-data-filter"))
                //{
                //    emailcasedatafilter = loadCollection["email.enable.case-data-filter"].ToLower().Equals("true");
                //    logger.Info(ContactDataContext.GetInstance().IsEnableCaseDataFromTransaction);
                //}
                //if (loadCollection.ContainsKey("chat.enable.case-data-filter"))
                //{
                //    chatcasedatafilter = loadCollection["chat.enable.case-data-filter"].ToLower().Equals("true");
                //    logger.Info(ContactDataContext.GetInstance().IsEnableCaseDataFromTransaction);
                //}

                //code to read business attribute.
                //if (ContactDataContext.GetInstance().IsEnableCaseDataFromTransaction)
                //    ReadCaseDataTransactionObject(ContactDataContext.GetInstance().CaseDataDispositionObject);
                //else
                //    ReadCaseDataFromBusinessAttribute(ContactDataContext.GetInstance().CaseDataDispositionObject);
                //  ReadCaseDataTransactionObject(ContactDataContext.GetInstance().CaseDataTransactionObject);
                //if (emailcasedatafilter && loadEmailCaseDataFilterKey.Count > 0 && loadEmailCaseDataFilterKey != null)
                //{
                //    ContactDataContext.GetInstance().LoadEmailCaseDataFilterKeys = (List<string>)loadEmailCaseDataFilterKey;
                //}
                //if (chatcasedatafilter && loadChatCaseDataFilterKey.Count > 0 && loadChatCaseDataFilterKey != null)
                //{
                //    ContactDataContext.GetInstance().LoadChatCaseDataFilterKeys = (List<string>)loadChatCaseDataFilterKey;
                //}
                //if (voicecasedatafilter && loadVoiceCaseDataFilterKey.Count > 0 && loadVoiceCaseDataFilterKey != null)
                //{
                //    ContactDataContext.GetInstance().LoadVoiceCaseDataFilterKeys = (List<string>)loadVoiceCaseDataFilterKey;
                //}





            }
            catch (Exception commonException)
            {
                logger.Error("Error occurred while reading GetAllValues() =  " + commonException.ToString());
            }
            finally
            {
                //GC.Collect();
            }
        }
        #endregion

        public void GetContactBusinessAttribute(string businessAttributeName)
        {
            Thread configurationThread = new Thread(delegate()
            {
                try
                {
                    CfgEnumeratorQuery enumaratorQuery = new CfgEnumeratorQuery();
                    enumaratorQuery.TenantDbid = ConfigContainer.Instance().TenantDbId;
                    enumaratorQuery.Name = businessAttributeName;
                    if (ConfigContainer.Instance().ConfServiceObject != null)
                    {
                        CfgEnumerator enumarator = ConfigContainer.Instance().ConfServiceObject.RetrieveObject<CfgEnumerator>(enumaratorQuery);
                        if (enumarator != null)
                        {
                            CfgEnumeratorValueQuery enumaeratorValueQuery = new CfgEnumeratorValueQuery();
                            enumaeratorValueQuery.EnumeratorDbid = enumarator.DBID;
                            // enumarator.SetTenantDBID(ConfigContainer.Instance().TenantDbId);
                            ICollection<CfgEnumeratorValue> enumeratorValue = ConfigContainer.Instance().ConfServiceObject.RetrieveMultipleObjects<CfgEnumeratorValue>(enumaeratorValueQuery);
                            foreach (CfgEnumeratorValue enumVal in enumeratorValue)
                            {
                                ContactDataContext.GetInstance().ContactValidAttribute.Add(enumVal.Name, enumVal.DisplayName);
                            }
                        }
                    }

                }
                catch (OperationCanceledException ex)
                {
                    logger.Warn("Get Operation Cancelled issue while reading contact business attribute as " + ex.Message);
                    GetContactBusinessAttribute(businessAttributeName);
                }
                catch (Exception ex)
                {
                    logger.Error("Error occurred as while reading contact attribute as " + ex.Message);
                }
            });
            configurationThread.Start();


        }


    }
}
