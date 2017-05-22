/*
  Copyright (c) Pointel Inc., All Rights Reserved.

 This software is the confidential and proprietary information of
 Pointel Inc., ("Confidential Information"). You shall not
 disclose such Confidential Information and shall use it only in
 accordance with the terms of the license agreement you entered into
 with Pointel.

 POINTEL MAKES NO REPRESENTATIONS OR WARRANTIES ABOUT THE
  *SUITABILITY OF THE SOFTWARE, EITHER EXPRESS OR IMPLIED, INCLUDING
  *BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY,
  *FITNESS FOR A PARTICULAR PURPOSE, OR NON-INFRINGEMENT. POINTEL
  *SHALL NOT BE LIABLE FOR ANY DAMAGES SUFFERED BY LICENSEE AS A
  *RESULT OF USING, MODIFYING OR DISTRIBUTING THIS SOFTWARE OR ITS
  *DERIVATIVES.
 */

using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel;
using Genesyslab.Platform.Commons.Collections;
using Pointel.Salesforce.Adapter.Configurations;
using Pointel.Salesforce.Adapter.LogMessage;
using Pointel.Salesforce.Adapter.SFDCUtils;
using System;
using System.Collections.Generic;

namespace Pointel.Salesforce.Adapter.Utility
{
    /// <summary>
    /// Comment: Holds the Common Properities for SFDC Popup
    /// Last Modified: 25-08-2015
    /// Created by: Pointel Inc
    /// </summary>
    internal class Settings
    {
        #region Fields Declarations

        private static Log logger = null;

        #region General Properties

        public static IConfService ConfigService
        {
            get;
            set;
        }

        public static ISFDCListener SFDCListener
        {
            get;
            set;
        }

        public static IAgentDetails AgentDetails
        {
            get;
            set;
        }

        #endregion General Properties

        #region SFDC CME Config Collections

        public static KeyValueCollection GeneralConfigs
        {
            get;
            set;
        }

        public static GeneralOptions SFDCOptions
        {
            get;
            set;
        }

        public static KeyValueCollection LeadConfigs
        {
            get;
            set;
        }

        public static KeyValueCollection LeadNewRecordConfigs
        {
            get;
            set;
        }

        public static KeyValueCollection ContactConfigs
        {
            get;
            set;
        }

        public static KeyValueCollection ContactNewRecordConfigs
        {
            get;
            set;
        }

        public static KeyValueCollection CaseConfigs
        {
            get;
            set;
        }

        public static KeyValueCollection CaseNewRecordConfigs
        {
            get;
            set;
        }

        public static KeyValueCollection AccountConfigs
        {
            get;
            set;
        }

        public static KeyValueCollection AccountNewRecordConfigs
        {
            get;
            set;
        }

        public static KeyValueCollection OpportunityConfigs
        {
            get;
            set;
        }

        public static KeyValueCollection OpportunityNewRecordConfigs
        {
            get;
            set;
        }

        private static Dictionary<string, KeyValueCollection> customObjectConfig = new Dictionary<string, KeyValueCollection>();

        public static Dictionary<string, KeyValueCollection> CustomObjectConfigs
        {
            get
            {
                return customObjectConfig;
            }
        }

        private static Dictionary<string, KeyValueCollection> customObjectNewRecordConfig = new Dictionary<string, KeyValueCollection>();

        public static Dictionary<string, KeyValueCollection> CustomObjectNewRecordConfigs
        {
            get
            {
                return customObjectNewRecordConfig;
            }
        }

        #endregion SFDC CME Config Collections

        #region Voice Options

        public static VoiceOptions LeadVoiceOptions
        {
            get;
            set;
        }

        public static VoiceOptions ContactVoiceOptions
        {
            get;
            set;
        }

        public static VoiceOptions CaseVoiceOptions
        {
            get;
            set;
        }

        public static VoiceOptions AccountVoiceOptions
        {
            get;
            set;
        }

        public static VoiceOptions OpportunityVoiceOptions
        {
            get;
            set;
        }

        private static Dictionary<string, VoiceOptions> customObjectVoiceOptions = new Dictionary<string, VoiceOptions>();

        public static Dictionary<string, VoiceOptions> CustomObjectVoiceOptions
        {
            get
            {
                return customObjectVoiceOptions;
            }
        }

        public static UserActivityOptions UserActivityVoiceOptions
        {
            get;
            set;
        }

        #endregion Voice Options

        #region ChatOptions

        public static ChatOptions ContactChatOptions
        {
            get;
            set;
        }

        public static ChatOptions LeadChatOptions
        {
            get;
            set;
        }

        public static ChatOptions CaseChatOptions
        {
            get;
            set;
        }

        public static ChatOptions AccountChatOptions
        {
            get;
            set;
        }

        public static ChatOptions OpportunityChatOptions
        {
            get;
            set;
        }

        private static Dictionary<string, ChatOptions> customObjectChatOptions = new Dictionary<string, ChatOptions>();

        public static Dictionary<string, ChatOptions> CustomObjectChatOptions
        {
            get
            {
                return customObjectChatOptions;
            }
        }

        public static UserActivityOptions UserActivityChatOptions
        {
            get;
            set;
        }

        #endregion ChatOptions

        #region Voice,Chat User Level ActivityLog Config

        private static IDictionary<string, KeyValueCollection> activityHistoryCollection = new Dictionary<string, KeyValueCollection>();

        public static IDictionary<string, KeyValueCollection> VoiceActivityLogCollection
        {
            get
            {
                return activityHistoryCollection;
            }
        }

        private static IDictionary<string, KeyValueCollection> chatActivityHistoryCollection = new Dictionary<string, KeyValueCollection>();

        public static IDictionary<string, KeyValueCollection> ChatActivityLogCollection
        {
            get
            {
                return chatActivityHistoryCollection;
            }
        }

        public static KeyValueCollection UserActivityConfigs
        {
            get;
            set;
        }

        #endregion Voice,Chat User Level ActivityLog Config

        public static KeyValueCollection ProfileLevelActivity { get; set; }

        public static IDictionary<string, SFDCData> UpdateSFDCLogFinishedEvent = new Dictionary<string, SFDCData>();
        public static IDictionary<string, UpdateLogData> UpdateSFDCLog = new Dictionary<string, UpdateLogData>();
        public static IDictionary<string, PopupData> SFDCPopupData = new Dictionary<string, PopupData>();
        public static Dictionary<string, string> CustomObjectNames = new Dictionary<string, string>();

        public static Dictionary<string, string> ClickToDialData = new Dictionary<string, string>();

        //public static Dictionary<string, SFDCCallType> UserEventData = new Dictionary<string, SFDCCallType>();

        public static string CommonPopupObjects = string.Empty;

        #endregion Fields Declarations

        #region Initialize Settings Common Properties

        public static void InitializeUtils(ISFDCListener _subscirber, IAgentDetails _agentDetails, IConfService _confService)
        {
            try
            {
                logger = Log.GenInstance();
                logger.Info("InitializeUtils : Reading Configuration and Initializing Properties......");
                AgentDetails = _agentDetails;
                ConfigService = _confService;
                SFDCListener = _subscirber;

                foreach (string sfdcObject in SFDCOptions.SFDCPopupPages)
                {
                    KeyValueCollection ConfigData = null;
                    if (sfdcObject == "lead")
                    {
                        CommonPopupObjects += "Lead,";
                        logger.Info("InitializeUtils : Reading SFDCLead Configuration and Initializing Properties......");
                        LeadConfigs = ReadConfiguration.GetInstance().ReadSFDCObjectConfig(_agentDetails.MyApplication, _agentDetails.AgentGroups, _agentDetails.Person, sfdcObject);
                        if (LeadConfigs != null)
                        {
                            LeadVoiceOptions = ReadProperties.GetInstance().GetSFDCObjectVoiceProperties(LeadConfigs, sfdcObject);
                            LeadChatOptions = ReadProperties.GetInstance().GetSFDCObjectChatProperties(LeadConfigs, sfdcObject);

                            ConfigData = ReadConfiguration.GetInstance().ReadBusinessAttribuiteConfig(ConfigService, _agentDetails.Person.Tenant.DBID, SFDCOptions.ActivityLogBusinessAttribute, "voice." + sfdcObject);
                            if (ConfigData != null)
                                VoiceActivityLogCollection.Add(sfdcObject, ConfigData);

                            ConfigData = ReadConfiguration.GetInstance().ReadBusinessAttribuiteConfig(ConfigService, _agentDetails.Person.Tenant.DBID, SFDCOptions.ActivityLogBusinessAttribute, "chat." + sfdcObject);
                            if (ConfigData != null)
                                ChatActivityLogCollection.Add(sfdcObject, ConfigData);

                            LeadNewRecordConfigs = ReadConfiguration.GetInstance().ReadBusinessAttribuiteConfig(ConfigService, _agentDetails.Person.Tenant.DBID, SFDCOptions.NewRecordDataBusinessAttribute, sfdcObject);
                        }
                        else
                            logger.Info("InitializeUtils : No Lead Configuration Found.");
                    }
                    else if (sfdcObject == "contact")
                    {
                        CommonPopupObjects += "Contact,";
                        logger.Info("InitializeUtils : Reading SFDCContact Configuration and Initializing Properties......");
                        ContactConfigs = ReadConfiguration.GetInstance().ReadSFDCObjectConfig(_agentDetails.MyApplication, _agentDetails.AgentGroups, _agentDetails.Person, sfdcObject);
                        if (ContactConfigs != null)
                        {
                            ContactVoiceOptions = ReadProperties.GetInstance().GetSFDCObjectVoiceProperties(ContactConfigs, sfdcObject);
                            ContactChatOptions = ReadProperties.GetInstance().GetSFDCObjectChatProperties(ContactConfigs, sfdcObject);

                            ConfigData = ReadConfiguration.GetInstance().ReadBusinessAttribuiteConfig(ConfigService, _agentDetails.Person.Tenant.DBID, SFDCOptions.ActivityLogBusinessAttribute, "voice." + sfdcObject);
                            if (ConfigData != null)
                                VoiceActivityLogCollection.Add(sfdcObject, ConfigData);

                            ConfigData = ReadConfiguration.GetInstance().ReadBusinessAttribuiteConfig(ConfigService, _agentDetails.Person.Tenant.DBID, SFDCOptions.ActivityLogBusinessAttribute, "chat." + sfdcObject);
                            if (ConfigData != null)
                                ChatActivityLogCollection.Add(sfdcObject, ConfigData);

                            ContactNewRecordConfigs = ReadConfiguration.GetInstance().ReadBusinessAttribuiteConfig(ConfigService, _agentDetails.Person.Tenant.DBID, SFDCOptions.NewRecordDataBusinessAttribute, sfdcObject);
                        }
                        else
                            logger.Info("InitializeUtils : No Contact Configuration Found.");
                    }
                    else if (sfdcObject == "account")
                    {
                        CommonPopupObjects += "Account,";
                        logger.Info("InitializeUtils : Reading SFDCAccount Configuration and Initializing Properties......");
                        AccountConfigs = ReadConfiguration.GetInstance().ReadSFDCObjectConfig(_agentDetails.MyApplication, _agentDetails.AgentGroups, _agentDetails.Person, sfdcObject);
                        if (AccountConfigs != null)
                        {
                            AccountVoiceOptions = ReadProperties.GetInstance().GetSFDCObjectVoiceProperties(AccountConfigs, sfdcObject);
                            AccountChatOptions = ReadProperties.GetInstance().GetSFDCObjectChatProperties(AccountConfigs, sfdcObject);

                            AccountNewRecordConfigs = ReadConfiguration.GetInstance().ReadBusinessAttribuiteConfig(ConfigService, _agentDetails.Person.Tenant.DBID, SFDCOptions.NewRecordDataBusinessAttribute, sfdcObject);

                            ConfigData = ReadConfiguration.GetInstance().ReadBusinessAttribuiteConfig(ConfigService, _agentDetails.Person.Tenant.DBID, SFDCOptions.ActivityLogBusinessAttribute, "voice." + sfdcObject);
                            if (ConfigData != null)
                                VoiceActivityLogCollection.Add(sfdcObject, ConfigData);

                            ConfigData = ReadConfiguration.GetInstance().ReadBusinessAttribuiteConfig(ConfigService, _agentDetails.Person.Tenant.DBID, SFDCOptions.ActivityLogBusinessAttribute, "chat." + sfdcObject);
                            if (ConfigData != null)
                                ChatActivityLogCollection.Add(sfdcObject, ConfigData);
                        }
                        else
                            logger.Info("InitializeUtils : No Account Configuration Found.");
                    }
                    else if (sfdcObject == "case")
                    {
                        CommonPopupObjects += "Case,";
                        logger.Info("InitializeUtils : Reading SFDCCase Configuration and Initializing Properties......");
                        CaseConfigs = ReadConfiguration.GetInstance().ReadSFDCObjectConfig(_agentDetails.MyApplication, _agentDetails.AgentGroups, _agentDetails.Person, sfdcObject);
                        if (CaseConfigs != null)
                        {
                            CaseVoiceOptions = ReadProperties.GetInstance().GetSFDCObjectVoiceProperties(CaseConfigs, sfdcObject);
                            CaseChatOptions = ReadProperties.GetInstance().GetSFDCObjectChatProperties(CaseConfigs, sfdcObject);
                            CaseNewRecordConfigs = ReadConfiguration.GetInstance().ReadBusinessAttribuiteConfig(ConfigService, _agentDetails.Person.Tenant.DBID, SFDCOptions.NewRecordDataBusinessAttribute, sfdcObject);

                            ConfigData = ReadConfiguration.GetInstance().ReadBusinessAttribuiteConfig(ConfigService, _agentDetails.Person.Tenant.DBID, SFDCOptions.ActivityLogBusinessAttribute, "voice." + sfdcObject);
                            if (ConfigData != null)
                                VoiceActivityLogCollection.Add(sfdcObject, ConfigData);

                            ConfigData = ReadConfiguration.GetInstance().ReadBusinessAttribuiteConfig(ConfigService, _agentDetails.Person.Tenant.DBID, SFDCOptions.ActivityLogBusinessAttribute, "chat." + sfdcObject);
                            if (ConfigData != null)
                                ChatActivityLogCollection.Add(sfdcObject, ConfigData);
                        }
                        else
                            logger.Info("InitializeUtils : No Case Configuration Found.");
                    }
                    else if (sfdcObject == "opportunity")
                    {
                        CommonPopupObjects += "Opportunity,";
                        logger.Info("InitializeUtils : Reading SFDCOpportunity Configuration and Initializing Properties......");
                        OpportunityConfigs = ReadConfiguration.GetInstance().ReadSFDCObjectConfig(_agentDetails.MyApplication, _agentDetails.AgentGroups, _agentDetails.Person, sfdcObject);
                        if (OpportunityConfigs != null)
                        {
                            OpportunityVoiceOptions = ReadProperties.GetInstance().GetSFDCObjectVoiceProperties(OpportunityConfigs, sfdcObject);
                            OpportunityChatOptions = ReadProperties.GetInstance().GetSFDCObjectChatProperties(OpportunityConfigs, sfdcObject);
                            OpportunityNewRecordConfigs = ReadConfiguration.GetInstance().ReadBusinessAttribuiteConfig(ConfigService, _agentDetails.Person.Tenant.DBID, SFDCOptions.NewRecordDataBusinessAttribute, sfdcObject);

                            ConfigData = ReadConfiguration.GetInstance().ReadBusinessAttribuiteConfig(ConfigService, _agentDetails.Person.Tenant.DBID, SFDCOptions.ActivityLogBusinessAttribute, "voice." + sfdcObject);
                            if (ConfigData != null)
                                VoiceActivityLogCollection.Add(sfdcObject, ConfigData);

                            ConfigData = ReadConfiguration.GetInstance().ReadBusinessAttribuiteConfig(ConfigService, _agentDetails.Person.Tenant.DBID, SFDCOptions.ActivityLogBusinessAttribute, "chat." + sfdcObject);
                            if (ConfigData != null)
                                ChatActivityLogCollection.Add(sfdcObject, ConfigData);
                        }
                        else
                            logger.Info("InitializeUtils : No Opportunity Configuration Found.");
                    }
                    else if (sfdcObject.Contains("customobject"))
                    {
                        logger.Info("InitializeUtils : Reading SFDCCustomObject Configuration and Initializing Properties......");
                        logger.Info("InitializeUtils : Object Name : " + sfdcObject);
                        KeyValueCollection customConfig = ReadConfiguration.GetInstance().ReadSFDCObjectConfig(_agentDetails.MyApplication, _agentDetails.AgentGroups, _agentDetails.Person, sfdcObject);
                        if (customConfig != null)
                        {
                            CustomObjectConfigs.Add(sfdcObject, customConfig);
                            VoiceOptions voiceOptions = ReadProperties.GetInstance().GetSFDCObjectVoiceProperties(customConfig, sfdcObject);
                            if (voiceOptions != null)
                            {
                                if (voiceOptions.ObjectName != null)
                                {
                                    CommonPopupObjects += voiceOptions.ObjectName + ",";
                                    CustomObjectNames.Add(voiceOptions.ObjectName, sfdcObject);
                                }
                                CustomObjectVoiceOptions.Add(sfdcObject, voiceOptions);
                            }
                            ChatOptions chatOptions = ReadProperties.GetInstance().GetSFDCObjectChatProperties(customConfig, sfdcObject);
                            if (chatOptions != null)
                            {
                                CustomObjectChatOptions.Add(sfdcObject, chatOptions);
                            }
                            CustomObjectNewRecordConfigs.Add(sfdcObject, ReadConfiguration.GetInstance().ReadBusinessAttribuiteConfig(ConfigService, _agentDetails.Person.Tenant.DBID, SFDCOptions.NewRecordDataBusinessAttribute, sfdcObject));

                            ConfigData = ReadConfiguration.GetInstance().ReadBusinessAttribuiteConfig(ConfigService, _agentDetails.Person.Tenant.DBID, SFDCOptions.ActivityLogBusinessAttribute, "voice." + sfdcObject);
                            if (ConfigData != null)
                                VoiceActivityLogCollection.Add(sfdcObject, ConfigData);

                            ConfigData = ReadConfiguration.GetInstance().ReadBusinessAttribuiteConfig(ConfigService, _agentDetails.Person.Tenant.DBID, SFDCOptions.ActivityLogBusinessAttribute, "chat." + sfdcObject);
                            if (ConfigData != null)
                                ChatActivityLogCollection.Add(sfdcObject, ConfigData);
                        }
                        else
                            logger.Info("InitializeUtils : No Configuration Found for the CustomObject : " + sfdcObject);
                    }
                    else if (sfdcObject == "useractivity")
                    {
                        logger.Info("InitializeUtils : Reading SFDCUserActivity Configuration and Initializing Properties......");
                        UserActivityConfigs = ReadConfiguration.GetInstance().ReadSFDCObjectConfig(_agentDetails.MyApplication, _agentDetails.AgentGroups, _agentDetails.Person, sfdcObject);
                        if (UserActivityConfigs != null)
                        {
                            UserActivityVoiceOptions = ReadProperties.GetInstance().GetSFDCUserActivityVoiceProperties(UserActivityConfigs, sfdcObject);
                            KeyValueCollection voiceUserActivityLog = ReadConfiguration.GetInstance().ReadBusinessAttribuiteConfig(ConfigService, _agentDetails.Person.Tenant.DBID, SFDCOptions.ActivityLogBusinessAttribute, "voice." + sfdcObject);
                            if (voiceUserActivityLog != null)
                            {
                                VoiceActivityLogCollection.Add(sfdcObject, voiceUserActivityLog);
                            }
                            UserActivityChatOptions = ReadProperties.GetInstance().GetSFDCUserActivityChatProperties(UserActivityConfigs, sfdcObject);
                            KeyValueCollection chatuseractivitylog = ReadConfiguration.GetInstance().ReadBusinessAttribuiteConfig(ConfigService, _agentDetails.Person.Tenant.DBID, SFDCOptions.ActivityLogBusinessAttribute, "chat." + sfdcObject);
                            if (chatuseractivitylog != null)
                            {
                                ChatActivityLogCollection.Add(sfdcObject, chatuseractivitylog);
                            }
                        }
                        else
                            logger.Info("InitializeUtils : No SFDCUserActivity Configuration Found.");
                    }
                }
                if (!String.IsNullOrEmpty(CommonPopupObjects))
                {
                    CommonPopupObjects = CommonPopupObjects.Substring(0, CommonPopupObjects.Length - 1);
                }
                if (SFDCOptions.CanUseCommonSearchData)
                {
                    //Read business attribute for Profile level activity
                    if (!string.IsNullOrWhiteSpace(SFDCOptions.ProfileActivityBusinessAttributeName))
                    {
                        ProfileLevelActivity = ReadConfiguration.GetInstance().ReadBusinessAttribuiteConfig(ConfigService, _agentDetails.Person.Tenant.DBID, SFDCOptions.ProfileActivityBusinessAttributeName, "voice.useractivity");
                    }
                    else
                        logger.Info("ProfileLevel Activity Log Creation is not configured");
                }
            }
            catch (Exception generalException)
            {
                logger.Error("InitializeUtils : Error Occurred while Reading SFDC Object configurations : " + generalException.ToString());
            }
        }

        #endregion Initialize Settings Common Properties
    }
}