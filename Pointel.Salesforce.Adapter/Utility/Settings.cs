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
using Pointel.Salesforce.Adapter.SFDCModels;
using Pointel.Salesforce.Adapter.SFDCUtils;
using System;
using System.Collections.Generic;

namespace Pointel.Salesforce.Adapter.Utility
{
    /// <summary>
    /// Comment: Holds the Common Properities for SFDC Popup Last Modified: 25-08-2015 Created by:
    /// Pointel Inc
    /// </summary>
    internal class Settings
    {
        #region Fields Declarations

        public static Dictionary<string, string> ClickToDialData = new Dictionary<string, string>();
        public static string CommonPopupObjects = string.Empty;
        public static Dictionary<string, string> CustomObjectNames = new Dictionary<string, string>();
        public static IDictionary<string, PopupData> SFDCPopupData = new Dictionary<string, PopupData>();
        public static IDictionary<string, UpdateLogData> UpdateSFDCLog = new Dictionary<string, UpdateLogData>();
        public static IDictionary<string, SFDCData> UpdateSFDCLogFinishedEvent = new Dictionary<string, SFDCData>();
        private static Log _logger = null;

        // storing and retrieving recent search data for email
        public static IDictionary<string, OutputValues> InboundEmailSearchResult = new Dictionary<string, OutputValues>();

        #region General Properties

        public static IAgentDetails AgentDetails
        {
            get;
            set;
        }

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

        #endregion General Properties

        #region SFDC CME Config Collections

        private static Dictionary<string, KeyValueCollection> _customObjectConfig = new Dictionary<string, KeyValueCollection>();

        private static Dictionary<string, KeyValueCollection> _customObjectNewRecordConfig = new Dictionary<string, KeyValueCollection>();

        public static KeyValueCollection AccountConfigs
        {
            get;
            set;
        }

        public static KeyValueCollection CaseConfigs
        {
            get;
            set;
        }

        public static KeyValueCollection ContactConfigs
        {
            get;
            set;
        }

        public static Dictionary<string, KeyValueCollection> CustomObjectConfigs
        {
            get
            {
                return _customObjectConfig;
            }
        }

        public static KeyValueCollection GeneralConfigs
        {
            get;
            set;
        }

        public static KeyValueCollection LeadConfigs
        {
            get;
            set;
        }

        public static KeyValueCollection OpportunityConfigs
        {
            get;
            set;
        }

        public static GeneralOptions SFDCOptions
        {
            get;
            set;
        }

        #endregion SFDC CME Config Collections

        #region Voice Options

        private static Dictionary<string, VoiceOptions> _customObjectVoiceOptions = new Dictionary<string, VoiceOptions>();

        public static VoiceOptions AccountVoiceOptions
        {
            get;
            set;
        }

        public static VoiceOptions CaseVoiceOptions
        {
            get;
            set;
        }

        public static VoiceOptions ContactVoiceOptions
        {
            get;
            set;
        }

        public static Dictionary<string, VoiceOptions> CustomObjectVoiceOptions
        {
            get
            {
                return _customObjectVoiceOptions;
            }
        }

        public static VoiceOptions LeadVoiceOptions
        {
            get;
            set;
        }

        public static VoiceOptions OpportunityVoiceOptions
        {
            get;
            set;
        }

        public static UserActivityOptions UserActivityVoiceOptions
        {
            get;
            set;
        }

        #endregion Voice Options

        #region ChatOptions

        private static Dictionary<string, ChatOptions> _customObjectChatOptions = new Dictionary<string, ChatOptions>();

        public static ChatOptions AccountChatOptions
        {
            get;
            set;
        }

        public static ChatOptions CaseChatOptions
        {
            get;
            set;
        }

        public static ChatOptions ContactChatOptions
        {
            get;
            set;
        }

        public static Dictionary<string, ChatOptions> CustomObjectChatOptions
        {
            get
            {
                return _customObjectChatOptions;
            }
        }

        public static ChatOptions LeadChatOptions
        {
            get;
            set;
        }

        public static ChatOptions OpportunityChatOptions
        {
            get;
            set;
        }

        public static UserActivityOptions UserActivityChatOptions
        {
            get;
            set;
        }

        public static int ChatProxyClientId { get; set; }

        #endregion ChatOptions

        #region EmailOptions

        public static Dictionary<string, string> ClickToEmailData = new Dictionary<string, string>();

        private static Dictionary<string, EmailOptions> _customObjectEmailOptions = new Dictionary<string, EmailOptions>();

        private static IDictionary<string, KeyValueCollection> _emailActivityHistoryCollection = new Dictionary<string, KeyValueCollection>();

        public static EmailOptions AccountEmailOptions
        {
            get;
            set;
        }

        public static EmailOptions CaseEmailOptions
        {
            get;
            set;
        }

        public static EmailOptions ContactEmailOptions
        {
            get;
            set;
        }

        public static Dictionary<string, EmailOptions> CustomObjectEmailOptions
        {
            get
            {
                return _customObjectEmailOptions;
            }
        }

        public static IDictionary<string, KeyValueCollection> EmailActivityLogCollection
        {
            get
            {
                return _emailActivityHistoryCollection;
            }
        }

        public static EmailOptions LeadEmailOptions
        {
            get;
            set;
        }

        public static EmailOptions OpportunityEmailOptions
        {
            get;
            set;
        }

        public static UserActivityOptions UserActivityEmailOptions
        {
            get;
            set;
        }

        public static KeyValueCollection WorkbinConfigs
        {
            get;
            set;
        }

        public static int EmailProxyClientId { get; set; }

        #endregion EmailOptions

        #region Voice,Email,Chat new record collection

        private static IDictionary<string, KeyValueCollection> _emailNewRecordCollection = new Dictionary<string, KeyValueCollection>();
        private static IDictionary<string, KeyValueCollection> _chatNewRecordCollection = new Dictionary<string, KeyValueCollection>();
        private static IDictionary<string, KeyValueCollection> _voiceNewRecordCollection = new Dictionary<string, KeyValueCollection>();

        public static IDictionary<string, KeyValueCollection> EmailNewRecordCollection
        {
            get
            {
                return _emailNewRecordCollection;
            }
        }

        public static IDictionary<string, KeyValueCollection> ChatNewRecordCollection
        {
            get
            {
                return _chatNewRecordCollection;
            }
        }

        public static IDictionary<string, KeyValueCollection> VoiceNewRecordCollection
        {
            get
            {
                return _voiceNewRecordCollection;
            }
        }

        #endregion Voice,Email,Chat new record collection

        #region Voice,Chat User Level ActivityLog Config

        private static IDictionary<string, KeyValueCollection> _activityHistoryCollection = new Dictionary<string, KeyValueCollection>();

        private static IDictionary<string, KeyValueCollection> _chatActivityHistoryCollection = new Dictionary<string, KeyValueCollection>();

        public static IDictionary<string, KeyValueCollection> ChatActivityLogCollection
        {
            get
            {
                return _chatActivityHistoryCollection;
            }
        }

        public static KeyValueCollection UserActivityConfigs
        {
            get;
            set;
        }

        public static IDictionary<string, KeyValueCollection> VoiceActivityLogCollection
        {
            get
            {
                return _activityHistoryCollection;
            }
        }

        #endregion Voice,Chat User Level ActivityLog Config

        public static bool IsVoiceEnabled { get; set; }
        public static bool IsChatEnabled { get; set; }
        public static bool IsEmailEnabled { get; set; }
        private static KeyValueCollection ConfigData = null;

        public static Queue<OutputValues> PopupQueuedItems = new Queue<OutputValues>();
        public static Queue<SFDCTaskData> TaskCreateQueuedItems = new Queue<SFDCTaskData>();
        public static List<KeyValueCollection> AdSearchConfig { get; set; }
        public static List<AdSearch> AdSearchList { get; set; }

        #endregion Fields Declarations

        public static void Initialize(ISFDCListener subscirber, IAgentDetails agentDetails, IConfService confService)
        {
            try
            {
                _logger = Log.GenInstance();
                _logger.Info("Initialize: Reading Configuration and Initializing Properties......");
                AgentDetails = agentDetails;
                ConfigService = confService;
                SFDCListener = subscirber;
                ReadAdvancedSearchOptions();// Reading Advanced Search Configurations
                foreach (string sfdcObjectName in SFDCOptions.SFDCPopupPages)
                {
                    switch (sfdcObjectName)
                    {
                        case "lead":
                            ReadLeadConfigurations(sfdcObjectName);
                            break;

                        case "contact":
                            ReadContactConfigurations(sfdcObjectName);
                            break;

                        case "account":
                            ReadAccountConfigurations(sfdcObjectName);
                            break;

                        case "case":
                            ReadCaseConfigurations(sfdcObjectName);
                            break;

                        case "opportunity":
                            ReadOpportunityConfigurations(sfdcObjectName);
                            break;

                        case "useractivity":
                            _logger.Info("Initialize: Reading User Profile Level Activity Configuration and Initializing Properties......");
                            UserActivityConfigs = ReadConfiguration.GetInstance().ReadSFDCUtilityConfig(AgentDetails.MyApplication, AgentDetails.AgentGroups, AgentDetails.Person, sfdcObjectName);
                            break;

                        default:
                            if (sfdcObjectName.Contains("customobject"))
                            {
                                ReadCustomObjectConfigurations(sfdcObjectName);
                            }
                            break;
                    }
                }
                //Read  Profile Activity/ UserActivity
                ReadProfileActivityLog();
                if (!String.IsNullOrEmpty(CommonPopupObjects))
                {
                    CommonPopupObjects = CommonPopupObjects.Substring(0, CommonPopupObjects.Length - 1);
                }
            }
            catch (Exception generalException)
            {
                _logger.Error("Initialize: Error Occurred while Reading SFDC Object configurations : " + generalException.ToString());
            }
        }

        private static void ReadProfileActivityLog()
        {
            // Reading User Profile Level Activity Log
            if (IsVoiceEnabled)
            {
                _logger.Info("Reading Voice Options for the UserActivity(Task) object");
                if (UserActivityConfigs != null)
                    UserActivityVoiceOptions = ReadProperties.GetInstance().GetSFDCUserActivityVoiceProperties(UserActivityConfigs, "useractivity");

                KeyValueCollection voiceUserActivityLog = ReadConfiguration.GetInstance().ReadBusinessAttribuiteConfig(ConfigService, AgentDetails.Person.Tenant.DBID, SFDCOptions.ActivityLogBusinessAttribute, "voice.useractivity");
                if (voiceUserActivityLog != null)
                    VoiceActivityLogCollection.Add("useractivity", voiceUserActivityLog);
                else
                    _logger.Info("User Profile Level Activity Log Configuration not found for the Voice Channel");
            }
            if (IsChatEnabled)
            {
                _logger.Info("Reading Chat Options for the UserActivity(Task) object");
                if (UserActivityConfigs != null)
                    UserActivityChatOptions = ReadProperties.GetInstance().GetSFDCUserActivityChatProperties(UserActivityConfigs, "useractivity");

                KeyValueCollection chatuseractivitylog = ReadConfiguration.GetInstance().ReadBusinessAttribuiteConfig(ConfigService, AgentDetails.Person.Tenant.DBID, SFDCOptions.ActivityLogBusinessAttribute, "chat.useractivity");
                if (chatuseractivitylog != null)
                    ChatActivityLogCollection.Add("useractivity", chatuseractivitylog);
                else
                    _logger.Info("User Profile Level Activity Log Configuration not found for the Chat Channel");
            }
            if (IsEmailEnabled)
            {
                _logger.Info("Reading Email Options for the UserActivity(Task) object");
                if (UserActivityConfigs != null)
                    UserActivityEmailOptions = ReadProperties.GetInstance().GetSFDCUserActivityEmailProperties(UserActivityConfigs, "useractivity");

                KeyValueCollection emailuseractivitylog = ReadConfiguration.GetInstance().ReadBusinessAttribuiteConfig(ConfigService, AgentDetails.Person.Tenant.DBID, SFDCOptions.ActivityLogBusinessAttribute, "email.useractivity");
                if (emailuseractivitylog != null)
                    EmailActivityLogCollection.Add("useractivity", emailuseractivitylog);
                else
                    _logger.Info("User Profile Level Activity Log Configuration not found for the Email Channel");
            }
        }

        private static void ReadLeadConfigurations(string sfdcObjectName)
        {
            try
            {
                _logger.Info("Initialize: Reading Lead Configuration and Initializing Properties......");
                CommonPopupObjects += "Lead,";
                LeadConfigs = ReadConfiguration.GetInstance().ReadSFDCUtilityConfig(AgentDetails.MyApplication, AgentDetails.AgentGroups, AgentDetails.Person, sfdcObjectName);
                if (LeadConfigs != null)
                {
                    if (IsVoiceEnabled)
                    {
                        _logger.Info("Reading Voice Options for the Lead object");
                        VoiceNewRecordCollection.Add(sfdcObjectName, ReadConfiguration.GetInstance().ReadBusinessAttribuiteConfig(ConfigService, AgentDetails.Person.Tenant.DBID, SFDCOptions.NewRecordDataBusinessAttribute, "voice." + sfdcObjectName));
                        LeadVoiceOptions = ReadProperties.GetInstance().GetSFDCObjectVoiceProperties(LeadConfigs, sfdcObjectName);
                        ConfigData = ReadConfiguration.GetInstance().ReadBusinessAttribuiteConfig(ConfigService, AgentDetails.Person.Tenant.DBID, SFDCOptions.ActivityLogBusinessAttribute, "voice." + sfdcObjectName);
                        if (ConfigData != null)
                            VoiceActivityLogCollection.Add(sfdcObjectName, ConfigData);
                    }

                    if (IsChatEnabled)
                    {
                        _logger.Info("Reading Chat Options for the Lead object");
                        ChatNewRecordCollection.Add(sfdcObjectName, ReadConfiguration.GetInstance().ReadBusinessAttribuiteConfig(ConfigService, AgentDetails.Person.Tenant.DBID, SFDCOptions.NewRecordDataBusinessAttribute, "chat." + sfdcObjectName));
                        LeadChatOptions = ReadProperties.GetInstance().GetSFDCObjectChatProperties(LeadConfigs, sfdcObjectName);
                        ConfigData = ReadConfiguration.GetInstance().ReadBusinessAttribuiteConfig(ConfigService, AgentDetails.Person.Tenant.DBID, SFDCOptions.ActivityLogBusinessAttribute, "chat." + sfdcObjectName);
                        if (ConfigData != null)
                            ChatActivityLogCollection.Add(sfdcObjectName, ConfigData);
                    }

                    if (IsEmailEnabled)
                    {
                        _logger.Info("Reading Email Options for the Lead object");
                        EmailNewRecordCollection.Add(sfdcObjectName, ReadConfiguration.GetInstance().ReadBusinessAttribuiteConfig(ConfigService, AgentDetails.Person.Tenant.DBID, SFDCOptions.NewRecordDataBusinessAttribute, "email." + sfdcObjectName));
                        LeadEmailOptions = ReadProperties.GetInstance().GetSFDCObjectEmailProperties(LeadConfigs, sfdcObjectName);
                        ConfigData = ReadConfiguration.GetInstance().ReadBusinessAttribuiteConfig(ConfigService, AgentDetails.Person.Tenant.DBID, SFDCOptions.ActivityLogBusinessAttribute, "email." + sfdcObjectName);
                        if (ConfigData != null)
                            EmailActivityLogCollection.Add(sfdcObjectName, ConfigData);
                    }
                }
                else
                    _logger.Info("Initialize: Lead Configuration Not Found.");
            }
            catch (Exception generalException)
            {
                _logger.Error("Error occurred in ReadLeadConfigurations() method, Exception:" + generalException.ToString());
            }
        }

        private static void ReadContactConfigurations(string sfdcObjectName)
        {
            try
            {
                _logger.Info("Initialize: Reading Contact Configuration and Initializing Properties......");
                CommonPopupObjects += "Contact,";
                ContactConfigs = ReadConfiguration.GetInstance().ReadSFDCUtilityConfig(AgentDetails.MyApplication, AgentDetails.AgentGroups, AgentDetails.Person, sfdcObjectName);
                if (ContactConfigs != null)
                {
                    if (IsVoiceEnabled)
                    {
                        _logger.Info("Reading Voice Options for the Contact object");
                        VoiceNewRecordCollection.Add(sfdcObjectName, ReadConfiguration.GetInstance().ReadBusinessAttribuiteConfig(ConfigService, AgentDetails.Person.Tenant.DBID, SFDCOptions.NewRecordDataBusinessAttribute, "voice." + sfdcObjectName));
                        ContactVoiceOptions = ReadProperties.GetInstance().GetSFDCObjectVoiceProperties(ContactConfigs, sfdcObjectName);
                        ConfigData = ReadConfiguration.GetInstance().ReadBusinessAttribuiteConfig(ConfigService, AgentDetails.Person.Tenant.DBID, SFDCOptions.ActivityLogBusinessAttribute, "voice." + sfdcObjectName);
                        if (ConfigData != null)
                            VoiceActivityLogCollection.Add(sfdcObjectName, ConfigData);
                    }

                    if (IsChatEnabled)
                    {
                        _logger.Info("Reading Chat Options for the Contact object");
                        ChatNewRecordCollection.Add(sfdcObjectName, ReadConfiguration.GetInstance().ReadBusinessAttribuiteConfig(ConfigService, AgentDetails.Person.Tenant.DBID, SFDCOptions.NewRecordDataBusinessAttribute, "chat." + sfdcObjectName));
                        ContactChatOptions = ReadProperties.GetInstance().GetSFDCObjectChatProperties(ContactConfigs, sfdcObjectName);
                        ConfigData = ReadConfiguration.GetInstance().ReadBusinessAttribuiteConfig(ConfigService, AgentDetails.Person.Tenant.DBID, SFDCOptions.ActivityLogBusinessAttribute, "chat." + sfdcObjectName);
                        if (ConfigData != null)
                            ChatActivityLogCollection.Add(sfdcObjectName, ConfigData);
                    }

                    if (IsEmailEnabled)
                    {
                        _logger.Info("Reading Email Options for the Contact object");
                        EmailNewRecordCollection.Add(sfdcObjectName, ReadConfiguration.GetInstance().ReadBusinessAttribuiteConfig(ConfigService, AgentDetails.Person.Tenant.DBID, SFDCOptions.NewRecordDataBusinessAttribute, "email." + sfdcObjectName));
                        ContactEmailOptions = ReadProperties.GetInstance().GetSFDCObjectEmailProperties(ContactConfigs, sfdcObjectName);
                        ConfigData = ReadConfiguration.GetInstance().ReadBusinessAttribuiteConfig(ConfigService, AgentDetails.Person.Tenant.DBID, SFDCOptions.ActivityLogBusinessAttribute, "email." + sfdcObjectName);
                        if (ConfigData != null)
                            EmailActivityLogCollection.Add(sfdcObjectName, ConfigData);
                    }
                }
                else
                    _logger.Info("Initialize: Contact Configuration Not Found.");
            }
            catch (Exception generalException)
            {
                _logger.Error("Error occurred in ReadContactConfigurations() method, Exception:" + generalException.ToString());
            }
        }

        private static void ReadAccountConfigurations(string sfdcObjectName)
        {
            try
            {
                _logger.Info("Initialize: Reading Account Configuration and Initializing Properties......");
                CommonPopupObjects += "Account,";
                AccountConfigs = ReadConfiguration.GetInstance().ReadSFDCUtilityConfig(AgentDetails.MyApplication, AgentDetails.AgentGroups, AgentDetails.Person, sfdcObjectName);
                if (AccountConfigs != null)
                {
                    if (IsVoiceEnabled)
                    {
                        _logger.Info("Reading Voice Options for the Account object");
                        VoiceNewRecordCollection.Add(sfdcObjectName, ReadConfiguration.GetInstance().ReadBusinessAttribuiteConfig(ConfigService, AgentDetails.Person.Tenant.DBID, SFDCOptions.NewRecordDataBusinessAttribute, "voice." + sfdcObjectName));
                        AccountVoiceOptions = ReadProperties.GetInstance().GetSFDCObjectVoiceProperties(AccountConfigs, sfdcObjectName);
                        ConfigData = ReadConfiguration.GetInstance().ReadBusinessAttribuiteConfig(ConfigService, AgentDetails.Person.Tenant.DBID, SFDCOptions.ActivityLogBusinessAttribute, "voice." + sfdcObjectName);
                        if (ConfigData != null)
                            VoiceActivityLogCollection.Add(sfdcObjectName, ConfigData);
                    }

                    if (IsChatEnabled)
                    {
                        _logger.Info("Reading Chat Options for the Account object");
                        ChatNewRecordCollection.Add(sfdcObjectName, ReadConfiguration.GetInstance().ReadBusinessAttribuiteConfig(ConfigService, AgentDetails.Person.Tenant.DBID, SFDCOptions.NewRecordDataBusinessAttribute, "chat." + sfdcObjectName));
                        AccountChatOptions = ReadProperties.GetInstance().GetSFDCObjectChatProperties(AccountConfigs, sfdcObjectName);
                        ConfigData = ReadConfiguration.GetInstance().ReadBusinessAttribuiteConfig(ConfigService, AgentDetails.Person.Tenant.DBID, SFDCOptions.ActivityLogBusinessAttribute, "chat." + sfdcObjectName);
                        if (ConfigData != null)
                            ChatActivityLogCollection.Add(sfdcObjectName, ConfigData);
                    }

                    if (IsEmailEnabled)
                    {
                        _logger.Info("Reading Email Options for the Account object");
                        EmailNewRecordCollection.Add(sfdcObjectName, ReadConfiguration.GetInstance().ReadBusinessAttribuiteConfig(ConfigService, AgentDetails.Person.Tenant.DBID, SFDCOptions.NewRecordDataBusinessAttribute, "email." + sfdcObjectName));
                        AccountEmailOptions = ReadProperties.GetInstance().GetSFDCObjectEmailProperties(AccountConfigs, sfdcObjectName);
                        ConfigData = ReadConfiguration.GetInstance().ReadBusinessAttribuiteConfig(ConfigService, AgentDetails.Person.Tenant.DBID, SFDCOptions.ActivityLogBusinessAttribute, "email." + sfdcObjectName);
                        if (ConfigData != null)
                            EmailActivityLogCollection.Add(sfdcObjectName, ConfigData);
                    }
                }
                else
                    _logger.Info("Initialize: Account Configuration Not Found.");
            }
            catch (Exception generalException)
            {
                _logger.Error("Error occurred in ReadAccountConfigurations() method, Exception:" + generalException.ToString());
            }
        }

        private static void ReadCaseConfigurations(string sfdcObjectName)
        {
            try
            {
                _logger.Info("Initialize: Reading Case Configuration and Initializing Properties......");
                CommonPopupObjects += "Case,";
                CaseConfigs = ReadConfiguration.GetInstance().ReadSFDCUtilityConfig(AgentDetails.MyApplication, AgentDetails.AgentGroups, AgentDetails.Person, sfdcObjectName);
                if (CaseConfigs != null)
                {
                    if (IsVoiceEnabled)
                    {
                        _logger.Info("Reading Voice Options for the Case object");
                        VoiceNewRecordCollection.Add(sfdcObjectName, ReadConfiguration.GetInstance().ReadBusinessAttribuiteConfig(ConfigService, AgentDetails.Person.Tenant.DBID, SFDCOptions.NewRecordDataBusinessAttribute, "voice." + sfdcObjectName));
                        CaseVoiceOptions = ReadProperties.GetInstance().GetSFDCObjectVoiceProperties(CaseConfigs, sfdcObjectName);
                        ConfigData = ReadConfiguration.GetInstance().ReadBusinessAttribuiteConfig(ConfigService, AgentDetails.Person.Tenant.DBID, SFDCOptions.ActivityLogBusinessAttribute, "voice." + sfdcObjectName);
                        if (ConfigData != null)
                            VoiceActivityLogCollection.Add(sfdcObjectName, ConfigData);
                    }

                    if (IsChatEnabled)
                    {
                        _logger.Info("Reading Chat Options for the Case object");
                        ChatNewRecordCollection.Add(sfdcObjectName, ReadConfiguration.GetInstance().ReadBusinessAttribuiteConfig(ConfigService, AgentDetails.Person.Tenant.DBID, SFDCOptions.NewRecordDataBusinessAttribute, "chat." + sfdcObjectName));
                        CaseChatOptions = ReadProperties.GetInstance().GetSFDCObjectChatProperties(CaseConfigs, sfdcObjectName);
                        ConfigData = ReadConfiguration.GetInstance().ReadBusinessAttribuiteConfig(ConfigService, AgentDetails.Person.Tenant.DBID, SFDCOptions.ActivityLogBusinessAttribute, "chat." + sfdcObjectName);
                        if (ConfigData != null)
                            ChatActivityLogCollection.Add(sfdcObjectName, ConfigData);
                    }

                    if (IsEmailEnabled)
                    {
                        _logger.Info("Reading Email Options for the Case object");
                        EmailNewRecordCollection.Add(sfdcObjectName, ReadConfiguration.GetInstance().ReadBusinessAttribuiteConfig(ConfigService, AgentDetails.Person.Tenant.DBID, SFDCOptions.NewRecordDataBusinessAttribute, "email." + sfdcObjectName));
                        CaseEmailOptions = ReadProperties.GetInstance().GetSFDCObjectEmailProperties(CaseConfigs, sfdcObjectName);
                        ConfigData = ReadConfiguration.GetInstance().ReadBusinessAttribuiteConfig(ConfigService, AgentDetails.Person.Tenant.DBID, SFDCOptions.ActivityLogBusinessAttribute, "email." + sfdcObjectName);
                        if (ConfigData != null)
                            EmailActivityLogCollection.Add(sfdcObjectName, ConfigData);
                    }
                }
                else
                    _logger.Info("Initialize: Case Configuration Not Found.");
            }
            catch (Exception generalException)
            {
                _logger.Error("Error occurred in ReadCaseConfigurations() method, Exception:" + generalException.ToString());
            }
        }

        private static void ReadOpportunityConfigurations(string sfdcObjectName)
        {
            try
            {
                _logger.Info("Initialize: Reading Opportunity Configuration and Initializing Properties......");
                CommonPopupObjects += "Opportunity,";
                OpportunityConfigs = ReadConfiguration.GetInstance().ReadSFDCUtilityConfig(AgentDetails.MyApplication, AgentDetails.AgentGroups, AgentDetails.Person, sfdcObjectName);
                if (OpportunityConfigs != null)
                {
                    if (IsVoiceEnabled)
                    {
                        _logger.Info("Reading Voice Options for the Opportunity object");
                        VoiceNewRecordCollection.Add(sfdcObjectName, ReadConfiguration.GetInstance().ReadBusinessAttribuiteConfig(ConfigService, AgentDetails.Person.Tenant.DBID, SFDCOptions.NewRecordDataBusinessAttribute, "voice." + sfdcObjectName));
                        OpportunityVoiceOptions = ReadProperties.GetInstance().GetSFDCObjectVoiceProperties(OpportunityConfigs, sfdcObjectName);
                        ConfigData = ReadConfiguration.GetInstance().ReadBusinessAttribuiteConfig(ConfigService, AgentDetails.Person.Tenant.DBID, SFDCOptions.ActivityLogBusinessAttribute, "voice." + sfdcObjectName);
                        if (ConfigData != null)
                            VoiceActivityLogCollection.Add(sfdcObjectName, ConfigData);
                    }

                    if (IsChatEnabled)
                    {
                        _logger.Info("Reading Chat Options for the Opportunity object");
                        ChatNewRecordCollection.Add(sfdcObjectName, ReadConfiguration.GetInstance().ReadBusinessAttribuiteConfig(ConfigService, AgentDetails.Person.Tenant.DBID, SFDCOptions.NewRecordDataBusinessAttribute, "chat." + sfdcObjectName));
                        OpportunityChatOptions = ReadProperties.GetInstance().GetSFDCObjectChatProperties(OpportunityConfigs, sfdcObjectName);
                        ConfigData = ReadConfiguration.GetInstance().ReadBusinessAttribuiteConfig(ConfigService, AgentDetails.Person.Tenant.DBID, SFDCOptions.ActivityLogBusinessAttribute, "chat." + sfdcObjectName);
                        if (ConfigData != null)
                            ChatActivityLogCollection.Add(sfdcObjectName, ConfigData);
                    }

                    if (IsEmailEnabled)
                    {
                        _logger.Info("Reading Email Options for the Opportunity object");
                        EmailNewRecordCollection.Add(sfdcObjectName, ReadConfiguration.GetInstance().ReadBusinessAttribuiteConfig(ConfigService, AgentDetails.Person.Tenant.DBID, SFDCOptions.NewRecordDataBusinessAttribute, "email." + sfdcObjectName));
                        OpportunityEmailOptions = ReadProperties.GetInstance().GetSFDCObjectEmailProperties(OpportunityConfigs, sfdcObjectName);
                        ConfigData = ReadConfiguration.GetInstance().ReadBusinessAttribuiteConfig(ConfigService, AgentDetails.Person.Tenant.DBID, SFDCOptions.ActivityLogBusinessAttribute, "email." + sfdcObjectName);
                        if (ConfigData != null)
                            EmailActivityLogCollection.Add(sfdcObjectName, ConfigData);
                    }
                }
                else
                    _logger.Info("Initialize: Opportunity Configuration Not Found.");
            }
            catch (Exception generalException)
            {
                _logger.Error("Error occurred in ReadOpportunityConfigurations() method, Exception:" + generalException.ToString());
            }
        }

        private static void ReadCustomObjectConfigurations(string sfdcObjectName)
        {
            try
            {
                _logger.Info("Initialize: Reading CustomObject Configuration and Initializing Properties......");
                _logger.Info("Initialize: Object Name : " + sfdcObjectName);
                KeyValueCollection customConfig = ReadConfiguration.GetInstance().ReadSFDCUtilityConfig(AgentDetails.MyApplication, AgentDetails.AgentGroups, AgentDetails.Person, sfdcObjectName);
                if (customConfig != null)
                {
                    CustomObjectConfigs.Add(sfdcObjectName, customConfig);
                    if (IsVoiceEnabled)
                    {
                        _logger.Info("Reading Voice Options for the " + sfdcObjectName + " object");
                        VoiceNewRecordCollection.Add(sfdcObjectName, ReadConfiguration.GetInstance().ReadBusinessAttribuiteConfig(ConfigService, AgentDetails.Person.Tenant.DBID, SFDCOptions.NewRecordDataBusinessAttribute, "voice." + sfdcObjectName));
                        VoiceOptions voiceOptions = ReadProperties.GetInstance().GetSFDCObjectVoiceProperties(customConfig, sfdcObjectName);
                        if (voiceOptions != null)
                        {
                            if (voiceOptions.ObjectName != null && !CustomObjectNames.ContainsKey(voiceOptions.ObjectName + ","))
                            {
                                CommonPopupObjects += voiceOptions.ObjectName + ",";
                                CustomObjectNames.Add(voiceOptions.ObjectName, sfdcObjectName);
                            }
                            CustomObjectVoiceOptions.Add(sfdcObjectName, voiceOptions);
                        }
                        ConfigData = ReadConfiguration.GetInstance().ReadBusinessAttribuiteConfig(ConfigService, AgentDetails.Person.Tenant.DBID, SFDCOptions.ActivityLogBusinessAttribute, "voice." + sfdcObjectName);
                        if (ConfigData != null)
                            VoiceActivityLogCollection.Add(sfdcObjectName, ConfigData);
                    }

                    if (IsChatEnabled)
                    {
                        _logger.Info("Reading Chat Options for the " + sfdcObjectName + " object");
                        ChatNewRecordCollection.Add(sfdcObjectName, ReadConfiguration.GetInstance().ReadBusinessAttribuiteConfig(ConfigService, AgentDetails.Person.Tenant.DBID, SFDCOptions.NewRecordDataBusinessAttribute, "chat." + sfdcObjectName));
                        ChatOptions chatOptions = ReadProperties.GetInstance().GetSFDCObjectChatProperties(customConfig, sfdcObjectName);
                        if (chatOptions != null)
                        {
                            if (chatOptions.ObjectName != null && !CustomObjectNames.ContainsKey(chatOptions.ObjectName + ","))
                            {
                                CommonPopupObjects += chatOptions.ObjectName + ",";
                                CustomObjectNames.Add(chatOptions.ObjectName, sfdcObjectName);
                            }
                            CustomObjectChatOptions.Add(sfdcObjectName, chatOptions);
                        }
                        ConfigData = ReadConfiguration.GetInstance().ReadBusinessAttribuiteConfig(ConfigService, AgentDetails.Person.Tenant.DBID, SFDCOptions.ActivityLogBusinessAttribute, "chat." + sfdcObjectName);
                        if (ConfigData != null)
                            ChatActivityLogCollection.Add(sfdcObjectName, ConfigData);
                    }

                    if (IsEmailEnabled)
                    {
                        _logger.Info("Reading Email Options for the " + sfdcObjectName + " object");
                        EmailOptions emailOptions = ReadProperties.GetInstance().GetSFDCObjectEmailProperties(customConfig, sfdcObjectName);
                        if (emailOptions != null)
                        {
                            if (emailOptions.ObjectName != null && !CustomObjectNames.ContainsKey(emailOptions.ObjectName + ","))
                            {
                                CommonPopupObjects += emailOptions.ObjectName + ",";
                                CustomObjectNames.Add(emailOptions.ObjectName, sfdcObjectName);
                            }
                            CustomObjectEmailOptions.Add(sfdcObjectName, emailOptions);
                        }
                        EmailNewRecordCollection.Add(sfdcObjectName, ReadConfiguration.GetInstance().ReadBusinessAttribuiteConfig(ConfigService, AgentDetails.Person.Tenant.DBID, SFDCOptions.NewRecordDataBusinessAttribute, "email." + sfdcObjectName));
                        ConfigData = ReadConfiguration.GetInstance().ReadBusinessAttribuiteConfig(ConfigService, AgentDetails.Person.Tenant.DBID, SFDCOptions.ActivityLogBusinessAttribute, "email." + sfdcObjectName);
                        if (ConfigData != null)
                            EmailActivityLogCollection.Add(sfdcObjectName, ConfigData);
                    }
                }
                else
                    _logger.Info("Initialize: CustomObject Configuration Not Found.");
            }
            catch (Exception generalException)
            {
                _logger.Error("Error occurred in CustomObjectConfigurations() method, Exception:" + generalException.ToString());
            }
        }

        private static void ReadAdvancedSearchOptions()
        {
            try
            {
                //Read Advanced search Configurations
                ReadProperties.GetInstance().ReadAdvancedSearchConfiguration();
                if (Settings.AdSearchConfig != null)
                {
                    AdSearchList = new List<AdSearch>();
                    foreach (KeyValueCollection configs in Settings.AdSearchConfig)
                    {
                        var adSearchObject = ReadProperties.GetInstance().BuildAdSearchobjects(configs);
                        if (adSearchObject != null)
                        {
                            AdSearchList.Add(adSearchObject);
                        }
                    }
                }
            }
            catch (Exception generalException)
            {
                _logger.Error("Error occurred while reading advanced search properties, details:" + generalException);
            }
        }
    }
}