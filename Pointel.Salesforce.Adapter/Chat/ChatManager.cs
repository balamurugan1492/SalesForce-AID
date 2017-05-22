using Genesyslab.Platform.Commons.Collections;
using Pointel.Salesforce.Adapter.Configurations;
using Pointel.Salesforce.Adapter.LogMessage;
using Pointel.Salesforce.Adapter.SFDCModels;
using Pointel.Salesforce.Adapter.SFDCUtils;
using Pointel.Salesforce.Adapter.Utility;
using System;
using System.Collections.Generic;

namespace Pointel.Salesforce.Adapter.Chat
{/// <summary>
    /// Comment: Provides Chat Common Methods Last Modified: 25-08-2015 Created by: Pointel Inc </summary>
    internal class ChatManager
    {
        /// <summary>
        /// Fields for the Class ChatEvents
        /// </summary>

        #region Fields

        private static ChatManager _currentObject = null;
        private ChatOptions _accountOptions = null;
        private IDictionary<string, KeyValueCollection> _activityLogs = null;
        private ChatOptions _caseOptions = null;
        private ChatOptions _contactOptions = null;
        private IDictionary<string, ChatOptions> _customObjectOptions = null;
        private ChatOptions _leadOptions = null;
        private GeneralOptions _generalOptions = null;
        private Log _logger = null;
        private ChatOptions _opportunityOptions = null;
        private string[] _popupPages = null;
        private SFDCUtility _sFDCUtility = null;
        private List<CustomObjectData> _tempCustomObject = new List<CustomObjectData>();
        private KeyValueCollection _userActivityLog = null;
        private UserActivityOptions _userActivityOptions = null;
        private SFDCUtiltiyHelper _sfdcUtilityHelper = null;

        #endregion Fields

        #region Constructor

        /// <summary>
        /// Creates an Instance of the Class
        /// </summary>
        public ChatManager()
        {
            this._logger = Log.GenInstance();
            this._leadOptions = Settings.LeadChatOptions;
            this._contactOptions = Settings.ContactChatOptions;
            this._accountOptions = Settings.AccountChatOptions;
            this._caseOptions = Settings.CaseChatOptions;
            this._opportunityOptions = Settings.OpportunityChatOptions;
            this._userActivityLog = (Settings.ChatActivityLogCollection.ContainsKey("useractivity")) ? Settings.ChatActivityLogCollection["useractivity"] : null;
            this._sFDCUtility = SFDCUtility.GetInstance();
            this._activityLogs = Settings.ChatActivityLogCollection;
            this._customObjectOptions = Settings.CustomObjectChatOptions;
            this._userActivityOptions = Settings.UserActivityChatOptions;
            this._popupPages = Settings.SFDCOptions.SFDCPopupPages;
            this._generalOptions = Settings.SFDCOptions;
            this._sfdcUtilityHelper = SFDCUtiltiyHelper.GetInstance();
        }

        #endregion Constructor

        #region GetInstance

        /// <summary>
        /// Gets the instance of the Class.s
        /// </summary>
        /// <returns></returns>
        public static ChatManager GetInstance()
        {
            if (_currentObject == null)
            {
                _currentObject = new ChatManager();
            }
            return _currentObject;
        }

        #endregion GetInstance

        #region Inbound Popup Data

        /// <summary>
        /// Gets the Inbound Popup Data
        /// </summary>
        /// <param name="message"></param>
        /// <param name="callType"></param>
        /// <param name="eventName"></param>
        /// <returns></returns>
        public SFDCData GetInboundPopupData(IXNCustomData chatPopupData, SFDCCallType callType, string eventName)
        {
            SFDCData sfdcData = new SFDCData();
            try
            {
                _logger.Info("Reading inbound chat popup data....");
                if (this._popupPages != null)
                {
                    bool IsCurrentEventEnabled = false;
                    if (Settings.SFDCOptions.CanUseCommonSearchForChat)
                    {
                        if (Settings.SFDCOptions.ChatInboutPopupEvent != null && Settings.SFDCOptions.ChatInboutPopupEvent.Contains(eventName))
                        {
                            IsCurrentEventEnabled = true;
                            sfdcData.CommonSearchData = GetChatCommonSearchData(chatPopupData, callType);
                            sfdcData.CommonPopupObjects = Settings.CommonPopupObjects;
                            sfdcData.CommonSearchFormats = Settings.SFDCOptions.PhoneNumberSearchFormat;
                            sfdcData.CommonSearchCondition = Settings.SFDCOptions.CommonSearchConditionForChat;
                            if (this._userActivityLog != null)
                            {
                                sfdcData.ProfileActivityLogData = this._sFDCUtility.GetCreateActivityLogData(this._userActivityLog, null, callType, emailData: chatPopupData);
                            }
                        }
                        else
                            return null;
                    }
                    foreach (string key in this._popupPages)
                    {
                        switch (key)
                        {
                            case "lead":
                                if (IsCurrentEventEnabled)
                                    sfdcData.LeadData = SFDCLead.GetInstance().GetLeadChatPopupData(chatPopupData, chatPopupData.InteractionType);
                                else if (this._leadOptions != null && this._leadOptions.InboundPopupEvent != null && this._leadOptions.InboundPopupEvent.Equals(eventName))
                                    sfdcData.LeadData = SFDCLead.GetInstance().GetLeadChatPopupData(chatPopupData, chatPopupData.InteractionType);
                                break;

                            case "contact":
                                if (IsCurrentEventEnabled)
                                    sfdcData.ContactData = SFDCContact.GetInstance().GetContactChatPopupData(chatPopupData, chatPopupData.InteractionType);
                                else if (this._contactOptions != null && this._contactOptions.InboundPopupEvent != null && this._contactOptions.InboundPopupEvent.Equals(eventName))
                                    sfdcData.ContactData = SFDCContact.GetInstance().GetContactChatPopupData(chatPopupData, chatPopupData.InteractionType);
                                break;

                            case "account":
                                if (IsCurrentEventEnabled)
                                    sfdcData.AccountData = SFDCAccount.GetInstance().GetAccountChatPopupData(chatPopupData, chatPopupData.InteractionType);
                                else if (this._accountOptions != null && this._accountOptions.InboundPopupEvent != null && this._accountOptions.InboundPopupEvent.Equals(eventName))
                                    sfdcData.AccountData = SFDCAccount.GetInstance().GetAccountChatPopupData(chatPopupData, chatPopupData.InteractionType);
                                break;

                            case "case":
                                if (IsCurrentEventEnabled)
                                    sfdcData.CaseData = SFDCCase.GetInstance().GetCaseChatPopupData(chatPopupData, chatPopupData.InteractionType);
                                if (this._caseOptions != null && this._caseOptions.InboundPopupEvent != null && this._caseOptions.InboundPopupEvent.Equals(eventName))
                                    sfdcData.CaseData = SFDCCase.GetInstance().GetCaseChatPopupData(chatPopupData, chatPopupData.InteractionType);
                                break;

                            case "opportunity":
                                if (IsCurrentEventEnabled)
                                    sfdcData.OpportunityData = SFDCOpportunity.GetInstance().GetOpportunityChatPopupData(chatPopupData, chatPopupData.InteractionType);
                                else if (this._opportunityOptions != null && this._opportunityOptions.InboundPopupEvent != null && this._opportunityOptions.InboundPopupEvent.Equals(eventName))
                                    sfdcData.OpportunityData = SFDCOpportunity.GetInstance().GetOpportunityChatPopupData(chatPopupData, chatPopupData.InteractionType);
                                break;

                            case "useractivity":
                                if (IsCurrentEventEnabled)
                                    sfdcData.UserActivityData = SFDCUserActivity.GetInstance().GetChatCreateUserAcitivityData(chatPopupData, chatPopupData.InteractionType);
                                else if (this._userActivityOptions != null && this._userActivityOptions.InboundPopupEvent != null && this._userActivityOptions.InboundPopupEvent.Equals(eventName))
                                    sfdcData.UserActivityData = SFDCUserActivity.GetInstance().GetChatCreateUserAcitivityData(chatPopupData, chatPopupData.InteractionType);
                                break;

                            default:
                                if (this._customObjectOptions != null)
                                {
                                    if (IsCurrentEventEnabled && this._customObjectOptions.ContainsKey(key))
                                    {
                                        CustomObjectData cstobject = SFDCCustomObject.GetInstance().GetCustomObjectChatPopupData(chatPopupData, chatPopupData.InteractionType, key);
                                        if (cstobject != null)
                                        {
                                            this._tempCustomObject.Add(cstobject);
                                        }
                                    }
                                    else if (this._customObjectOptions[key] != null && this._customObjectOptions[key].InboundPopupEvent != null &&
                                           this._customObjectOptions[key].InboundPopupEvent.Equals(eventName))
                                    {
                                        CustomObjectData cstobject = SFDCCustomObject.GetInstance().GetCustomObjectChatPopupData(chatPopupData, chatPopupData.InteractionType, key);
                                        if (cstobject != null)
                                        {
                                            this._tempCustomObject.Add(cstobject);
                                        }
                                    }
                                }
                                break;
                        }
                    }
                    if (this._tempCustomObject.Count > 0)
                    {
                        CustomObjectData[] temp = this._tempCustomObject.ToArray();
                        sfdcData.CustomObjectData = temp;
                        this._tempCustomObject.Clear();
                    }
                }
            }
            catch (Exception generalException)
            {
                _logger.Error("GetInboundPopupData : Error Occurred While Getting Inbound Popup Data : " + generalException.ToString());
            }
            return sfdcData;
        }

        #endregion Inbound Popup Data

        #region Consult Received Popup Data

        /// <summary>
        /// Gets the Consult Received popup Data
        /// </summary>
        /// <param name="chatData"></param>
        /// <param name="callType"></param>
        /// <param name="eventName"></param>
        /// <returns></returns>
        public SFDCData GetConsultReceivedPopupData(IXNCustomData chatData, SFDCCallType callType, string eventName)
        {
            SFDCData sfdcData = new SFDCData();
            try
            {
                _logger.Info("GetConsultReceivedPopupData : Getting Consult Received Popup Data");

                if (this._popupPages != null)
                {
                    if (Settings.SFDCOptions.CanUseCommonSearchForChat)
                    {
                        if (Settings.SFDCOptions.ChatInboutPopupEvent != null && Settings.SFDCOptions.ChatInboutPopupEvent.Contains(eventName))
                        {
                            sfdcData.CommonSearchData = GetChatCommonSearchData(chatData, callType);
                            sfdcData.CommonPopupObjects = Settings.CommonPopupObjects;
                            sfdcData.CommonSearchFormats = Settings.SFDCOptions.PhoneNumberSearchFormat;
                            sfdcData.CommonSearchCondition = Settings.SFDCOptions.CommonSearchConditionForChat;
                            if (this._userActivityLog != null)
                            {
                                sfdcData.ProfileActivityLogData = this._sFDCUtility.GetCreateActivityLogData(this._userActivityLog, null, callType, emailData: chatData);
                            }
                        }
                        else
                            return null;
                    }
                    foreach (string key in this._popupPages)
                    {
                        switch (key)
                        {
                            case "lead":
                                if (this._leadOptions != null && this._leadOptions.ConsultPopupEvent != null &&
                    this._leadOptions.ConsultPopupEvent.Equals(eventName))
                                {
                                    _logger.Info("GetConsultReceivedPopupData : Reading Lead Popup Data");
                                    sfdcData.LeadData = SFDCLead.GetInstance().GetLeadChatPopupData(chatData, callType);
                                }
                                break;

                            case "contact":
                                if (this._contactOptions != null && this._contactOptions.ConsultPopupEvent != null &&
                    this._contactOptions.ConsultPopupEvent.Equals(eventName))
                                {
                                    _logger.Info("GetConsultReceivedPopupData : Reading Lead Popup Data");
                                    sfdcData.ContactData = SFDCContact.GetInstance().GetContactChatPopupData(chatData, callType);
                                }
                                break;

                            case "account":
                                if (this._accountOptions != null && this._accountOptions.ConsultPopupEvent != null &&
                   this._accountOptions.ConsultPopupEvent.Equals(eventName))
                                {
                                    _logger.Info("GetConsultReceivedPopupData : Reading account Popup Data");
                                    sfdcData.AccountData = SFDCAccount.GetInstance().GetAccountChatPopupData(chatData, callType);
                                }
                                break;

                            case "case":
                                if (this._caseOptions != null && this._caseOptions.ConsultPopupEvent != null &&
                    this._caseOptions.ConsultPopupEvent.Equals(eventName))
                                {
                                    _logger.Info("GetConsultReceivedPopupData : Reading case Popup Data");
                                    sfdcData.CaseData = SFDCCase.GetInstance().GetCaseChatPopupData(chatData, callType);
                                }
                                break;

                            case "opportunity":
                                if (this._opportunityOptions != null && this._opportunityOptions.ConsultPopupEvent != null &&
                   this._opportunityOptions.ConsultPopupEvent.Equals(eventName))
                                {
                                    _logger.Info("GetConsultReceivedPopupData : Reading opportunity Popup Data");
                                    sfdcData.OpportunityData = SFDCOpportunity.GetInstance().GetOpportunityChatPopupData(chatData, callType);
                                }
                                break;

                            case "useractivity":
                                if (this._userActivityOptions != null && this._userActivityOptions.ConsultPopupEvent != null &&
                   this._userActivityOptions.ConsultPopupEvent.Equals(eventName))
                                {
                                    _logger.Info("GetConsultReceivedPopupData : Reading useractivity Popup Data");
                                    sfdcData.UserActivityData = SFDCUserActivity.GetInstance().GetChatCreateUserAcitivityData(chatData, callType);
                                }
                                break;

                            default:
                                if (this._customObjectOptions.ContainsKey(key))
                                {
                                    if (this._customObjectOptions[key] != null && this._customObjectOptions[key].ConsultPopupEvent != null &&
                                            this._customObjectOptions[key].ConsultPopupEvent.Equals(eventName))
                                    {
                                        _logger.Info("GetConsultReceivedPopupData : Reading CustomObject Popup Data");
                                        CustomObjectData cstobject = SFDCCustomObject.GetInstance().GetCustomObjectChatPopupData(chatData, callType, key);
                                        if (cstobject != null)
                                        {
                                            this._tempCustomObject.Add(cstobject);
                                        }
                                    }
                                }
                                break;
                        }
                    }
                    if (this._tempCustomObject.Count > 0)
                    {
                        CustomObjectData[] temp = this._tempCustomObject.ToArray();
                        sfdcData.CustomObjectData = temp;
                        this._tempCustomObject.Clear();
                    }
                }
            }
            catch (Exception generalException)
            {
                _logger.Error("GetConsultReceivedPopupData : Error Occurred While Reading Consult Popup Data : " + generalException.ToString());
            }
            return sfdcData;
        }

        #endregion Consult Received Popup Data

        #region Inbound Update Data

        /// <summary>
        /// Gets the inbound update data.
        /// </summary>
        /// <param name="chatData">The chat data.</param>
        /// <param name="eventName">Name of the event.</param>
        /// <returns></returns>
        public SFDCData GetInboundUpdateData(IXNCustomData chatData, string eventName)
        {
            SFDCData sfdcData = new SFDCData();
            try
            {
                _logger.Info("GetInboundUpdateData : Getting Inbound Update Data");
                if (this._popupPages != null)
                {
                    foreach (string key in this._popupPages)
                    {
                        switch (key)
                        {
                            case "lead":
                                if (this._leadOptions != null && this._leadOptions.InboundUpdateEvent != null && this._leadOptions.InboundUpdateEvent.Contains(eventName))
                                {
                                    _logger.Info("GetInboundUpdateData : Reading Lead popup data");
                                    sfdcData.LeadData = SFDCLead.GetInstance().GetLeadChatUpdateData(chatData, eventName);
                                }
                                break;

                            case "contact":
                                if (this._contactOptions != null && this._contactOptions.InboundUpdateEvent != null && this._contactOptions.InboundUpdateEvent.Contains(eventName))
                                {
                                    _logger.Info("GetInboundUpdateData : Reading contact popup data");
                                    sfdcData.ContactData = SFDCContact.GetInstance().GetContactChatUpdateData(chatData, eventName);
                                }
                                break;

                            case "account":
                                if (this._accountOptions != null && this._accountOptions.InboundUpdateEvent != null && this._accountOptions.InboundUpdateEvent.Contains(eventName))
                                {
                                    _logger.Info("GetInboundUpdateData : Reading account popup data");
                                    sfdcData.AccountData = SFDCAccount.GetInstance().GetAccountChatUpdateData(chatData, eventName);
                                }
                                break;

                            case "case":
                                if (this._caseOptions != null && this._caseOptions.InboundUpdateEvent != null && this._caseOptions.InboundUpdateEvent.Contains(eventName))
                                {
                                    _logger.Info("GetInboundUpdateData : Reading case popup data");
                                    sfdcData.CaseData = SFDCCase.GetInstance().GetCaseChatUpdateData(chatData, eventName);
                                }
                                break;

                            case "opportunity":
                                if (this._opportunityOptions != null && this._opportunityOptions.InboundUpdateEvent != null && this._opportunityOptions.InboundUpdateEvent.Contains(eventName))
                                {
                                    _logger.Info("GetInboundUpdateData : Reading opportunity popup data");
                                    sfdcData.OpportunityData = SFDCOpportunity.GetInstance().GetOpportunityChatUpdateData(chatData, eventName);
                                }
                                break;

                            case "useractivity":
                                if (this._userActivityOptions != null && this._userActivityOptions.InboundUpdateEvent != null &&
                    this._userActivityOptions.InboundUpdateEvent.Contains(eventName))
                                {
                                    _logger.Info("GetInboundUpdateData : Reading useractivity popup data");
                                    sfdcData.UserActivityData = SFDCUserActivity.GetInstance().GetChatUpdateUserAcitivityData(chatData, eventName);
                                }
                                break;

                            default:
                                if (this._customObjectOptions.ContainsKey(key))
                                {
                                    if (this._customObjectOptions[key] != null && this._customObjectOptions[key].InboundUpdateEvent != null &&
                                            this._customObjectOptions[key].InboundUpdateEvent.Contains(eventName))
                                    {
                                        CustomObjectData cstobject = SFDCCustomObject.GetInstance().GetCustomObjectChatUpdateData(chatData, key, eventName);
                                        if (cstobject != null)
                                        {
                                            _logger.Info("GetInboundUpdateData : Reading customObject popup data");
                                            this._tempCustomObject.Add(cstobject);
                                        }
                                    }
                                }
                                break;
                        }
                    }
                    if (this._tempCustomObject.Count > 0)
                    {
                        CustomObjectData[] temp = this._tempCustomObject.ToArray();
                        sfdcData.CustomObjectData = temp;
                        this._tempCustomObject.Clear();
                    }
                }
            }
            catch (Exception generalException)
            {
                _logger.Error("GetInboundUpdateData : Error Occurred while Collecting Inbound Update Data : " + generalException.ToString());
            }
            return sfdcData;
        }

        #endregion Inbound Update Data

        #region Consult Update Data

        /// <summary>
        /// Gets the Chat Consult Update data
        /// </summary>
        /// <param name="chatData">The chat data.</param>
        /// <param name="eventName">Name of the event.</param>
        /// <returns></returns>

        public SFDCData GetConsultUpdateData(IXNCustomData chatData, string eventName)
        {
            SFDCData sfdcData = new SFDCData();
            try
            {
                _logger.Info("GetConsultUpdateData : Getting lead Consult update data");
                if (this._popupPages != null)
                {
                    foreach (string key in this._popupPages)
                    {
                        switch (key)
                        {
                            case "lead":
                                if (this._leadOptions != null && this._leadOptions.ConsultUpdateEvent != null && this._leadOptions.ConsultUpdateEvent.Contains(eventName))
                                {
                                    _logger.Info("GetConsultUpdateData : Reading lead popup data");
                                    sfdcData.LeadData = SFDCLead.GetInstance().GetLeadChatUpdateData(chatData, eventName);
                                }
                                break;

                            case "contact":
                                if (this._contactOptions != null && this._contactOptions.ConsultUpdateEvent != null && this._contactOptions.ConsultUpdateEvent.Contains(eventName))
                                {
                                    _logger.Info("GetConsultUpdateData : Reading contact popup data");
                                    sfdcData.ContactData = SFDCContact.GetInstance().GetContactChatUpdateData(chatData, eventName);
                                }
                                break;

                            case "account":
                                if (this._accountOptions != null && this._accountOptions.ConsultUpdateEvent != null && this._accountOptions.ConsultUpdateEvent.Contains(eventName))
                                {
                                    _logger.Info("GetConsultUpdateData : Reading account popup data");
                                    sfdcData.AccountData = SFDCAccount.GetInstance().GetAccountChatUpdateData(chatData, eventName);
                                }
                                break;

                            case "case":
                                if (this._caseOptions != null && this._caseOptions.ConsultUpdateEvent != null && this._caseOptions.ConsultUpdateEvent.Contains(eventName))
                                {
                                    _logger.Info("GetConsultUpdateData : Reading case popup data");
                                    sfdcData.CaseData = SFDCCase.GetInstance().GetCaseChatUpdateData(chatData, eventName);
                                }
                                break;

                            case "opportunity":
                                if (this._opportunityOptions != null && this._opportunityOptions.ConsultUpdateEvent != null && this._opportunityOptions.ConsultUpdateEvent.Contains(eventName))
                                {
                                    _logger.Info("GetConsultUpdateData : Reading opportunity popup data");
                                    sfdcData.OpportunityData = SFDCOpportunity.GetInstance().GetOpportunityChatUpdateData(chatData, eventName);
                                }
                                break;

                            case "useractivity":
                                if (this._userActivityOptions != null && this._userActivityOptions.ConsultUpdateEvent != null &&
                    this._userActivityOptions.ConsultUpdateEvent.Contains(eventName))
                                {
                                    _logger.Info("GetConsultUpdateData : Reading useractivity popup data");
                                    sfdcData.UserActivityData = SFDCUserActivity.GetInstance().GetChatUpdateUserAcitivityData(chatData, eventName);
                                }
                                break;

                            default:
                                if (this._customObjectOptions.ContainsKey(key))
                                {
                                    if (this._customObjectOptions[key] != null && this._customObjectOptions[key].ConsultUpdateEvent != null &&
                                            this._customObjectOptions[key].ConsultUpdateEvent.Contains(eventName))
                                    {
                                        CustomObjectData cstobject = SFDCCustomObject.GetInstance().GetCustomObjectChatUpdateData(chatData, key, eventName);
                                        if (cstobject != null)
                                        {
                                            _logger.Info("GetConsultUpdateData : Reading customObject popup data");
                                            this._tempCustomObject.Add(cstobject);
                                        }
                                    }
                                }
                                break;
                        }
                    }
                    if (this._tempCustomObject.Count > 0)
                    {
                        CustomObjectData[] temp = this._tempCustomObject.ToArray();
                        sfdcData.CustomObjectData = temp;
                        this._tempCustomObject.Clear();
                    }
                }
            }
            catch (Exception generalException)
            {
                _logger.Error("GetConsultUpdateData : Error Occurred while collecting Consult Update data : " + generalException.ToString());
            }
            return sfdcData;
        }

        #endregion Consult Update Data

        #region GetChatCommonSearchData

        internal string GetChatCommonSearchData(IXNCustomData chatData, SFDCCallType callType)
        {
            try
            {
                this._logger.Info("GetChatCommonSearchData :  Reading Chat Common search data.....");
                this._logger.Info("GetChatCommonSearchData :  Event Name : " + chatData.EventName);
                this._logger.Info("GetChatCommonSearchData :  CallType : " + callType.ToString());
                string[] userDataSearchKeys = null;
                string[] attributeSearchKeys = null;
                string searchValue = string.Empty;
                if (callType == SFDCCallType.InboundChat)
                {
                    userDataSearchKeys = (this._generalOptions.ChatInboundSearchUserdataKeys != null) ? this._generalOptions.ChatInboundSearchUserdataKeys.Split(',') : null;
                    attributeSearchKeys = (this._generalOptions.ChatInboundAttributeKeys != null) ? this._generalOptions.ChatInboundAttributeKeys.Split(',') : null;
                }
                else if (callType == SFDCCallType.ConsultChatReceived)
                {
                    userDataSearchKeys = (this._generalOptions.ChatConsultSearchUserdataKeys != null) ? this._generalOptions.ChatConsultSearchUserdataKeys.Split(',') : null;
                    attributeSearchKeys = (this._generalOptions.ChatConsultAttributeKeys != null) ? this._generalOptions.ChatConsultAttributeKeys.Split(',') : null;
                }

                if (this._generalOptions.ChatSearchPriority == "user-data" && chatData.UserData != null)
                {
                    if (userDataSearchKeys != null)
                    {
                        searchValue = this._sfdcUtilityHelper.GetUserDataSearchValues(chatData.UserData, userDataSearchKeys);
                        if (!this._sfdcUtilityHelper.ValidateSearchData(searchValue))
                        {
                            this._logger.Info("search data from user-data keys not found, Reading attribute search keys.....");
                            searchValue = this._sfdcUtilityHelper.GetChatAttributeValueForSearch(chatData, attributeSearchKeys);
                        }
                    }
                    else if (attributeSearchKeys != null)
                    {
                        this._logger.Info("Either user-data search keys or user-data is null, Reading attribute search keys.....");
                        searchValue = this._sfdcUtilityHelper.GetChatAttributeValueForSearch(chatData, attributeSearchKeys);
                    }
                }
                else if (this._generalOptions.ChatSearchPriority == "attribute")
                {
                    searchValue = this._sfdcUtilityHelper.GetChatAttributeValueForSearch(chatData, attributeSearchKeys);
                    if (!this._sfdcUtilityHelper.ValidateSearchData(searchValue))
                    {
                        this._logger.Info("search data from attribute keys not found, Reading user-data search keys.....");
                        if (userDataSearchKeys != null && chatData.UserData != null)
                        {
                            searchValue = this._sfdcUtilityHelper.GetUserDataSearchValues(chatData.UserData, userDataSearchKeys);
                        }
                        else
                        {
                            this._logger.Info("Either user-data or search keys are not found.....");
                        }
                    }
                }
                else if (this._generalOptions.ChatSearchPriority == "both")
                {
                    if (chatData.UserData != null)
                        searchValue = this._sfdcUtilityHelper.GetUserDataSearchValues(chatData.UserData, userDataSearchKeys);
                    if (!this._sfdcUtilityHelper.ValidateSearchData(searchValue))
                    {
                        string temp = this._sfdcUtilityHelper.GetChatAttributeValueForSearch(chatData, attributeSearchKeys);
                        if (temp != string.Empty)
                        {
                            searchValue += "," + temp;
                        }
                    }
                    else
                    {
                        searchValue = this._sfdcUtilityHelper.GetChatAttributeValueForSearch(chatData, attributeSearchKeys);
                    }
                }
                return searchValue;
            }
            catch (Exception generalException)
            {
                this._logger.Error("GetChatCommonSearchData : Error occurred while reading common search Data : " + generalException.ToString());
            }
            return null;
        }

        #endregion GetChatCommonSearchData
    }
}