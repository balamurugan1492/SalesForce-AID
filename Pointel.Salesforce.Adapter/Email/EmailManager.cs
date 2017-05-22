namespace Pointel.Salesforce.Adapter.Email
{
    using Genesyslab.Platform.Commons.Collections;
    using Pointel.Salesforce.Adapter.Configurations;
    using Pointel.Salesforce.Adapter.LogMessage;
    using Pointel.Salesforce.Adapter.SFDCModels;
    using Pointel.Salesforce.Adapter.SFDCUtils;
    using Pointel.Salesforce.Adapter.Utility;
    using System;
    using System.Collections.Generic;
    using System.Xml;

    internal class EmailManager
    {
        private static EmailManager currentObject = null;
        private EmailOptions _accountOptions = null;
        private IDictionary<string, KeyValueCollection> activityLogs = null;
        private EmailOptions _caseOptions = null;
        private EmailOptions _contactOptions = null;
        private IDictionary<string, EmailOptions> customObjectOptions = null;
        private GeneralOptions _generalOptions = null;
        private EmailOptions _leadOptions = null;
        private Log _logger = null;
        private EmailOptions _opportunityOptions = null;
        private string[] _popupPages = null;
        private SFDCUtility _sfdcUtility = null;
        private List<CustomObjectData> _tempCustomObject = new List<CustomObjectData>();
        private KeyValueCollection _userActivityLog = null;
        private UserActivityOptions _userActivityOptions = null;
        private SFDCUtiltiyHelper _sfdcUtilityHelper = null;

        public EmailManager()
        {
            this._logger = Log.GenInstance();
            this._leadOptions = Settings.LeadEmailOptions;
            this._contactOptions = Settings.ContactEmailOptions;
            this._accountOptions = Settings.AccountEmailOptions;
            this._caseOptions = Settings.CaseEmailOptions;
            this._opportunityOptions = Settings.OpportunityEmailOptions;
            this._userActivityLog = Settings.EmailActivityLogCollection.ContainsKey("useractivity") ? Settings.EmailActivityLogCollection["useractivity"] : null;
            this._sfdcUtility = SFDCUtility.GetInstance();
            this.activityLogs = Settings.EmailActivityLogCollection;
            this.customObjectOptions = Settings.CustomObjectEmailOptions;
            this._userActivityOptions = Settings.UserActivityEmailOptions;
            this._popupPages = Settings.SFDCOptions.SFDCPopupPages;
            this._generalOptions = Settings.SFDCOptions;
            this._sfdcUtilityHelper = SFDCUtiltiyHelper.GetInstance();
        }

        public static EmailManager GetInstance()
        {
            if (currentObject == null)
            {
                currentObject = new EmailManager();
            }
            return currentObject;
        }

        public void GetClickToEmailLogs(IXNCustomData emailData, string ConnId, SFDCCallType callType, string clickToemailData, string eventName)
        {
            try
            {
                this._logger.Info("GetClickToEmailLogs : ClickToEmailPopup");
                EmailOptions emailOption = null;
                string[] data = clickToemailData.Split(',');
                if (data != null && data.Length == 2)
                {
                    switch (data[0].ToLower())
                    {
                        case "lead":
                            emailOption = _leadOptions;
                            break;

                        case "contact":
                            emailOption = _contactOptions;
                            break;

                        case "account":
                            emailOption = _accountOptions;
                            break;

                        case "case":
                            emailOption = _caseOptions;
                            break;

                        case "opportunity":
                            emailOption = _opportunityOptions;
                            break;

                        default:
                            if (data[0].Contains("__c"))
                            {
                                string objectName = Settings.CustomObjectNames[data[0]];
                                if (objectName != null)
                                {
                                    emailOption = customObjectOptions[objectName];
                                }
                            }
                            break;
                    }

                    #region Popup

                    if (emailOption != null)
                    {
                        if ((callType == SFDCCallType.OutboundEmailSuccess && emailOption.OutboundPopupEvent != null &&
                        emailOption.OutboundPopupEvent.Contains(eventName)) || (callType == SFDCCallType.OutboundEmailFailure && emailOption.OutboundFailurePopupEvent != null &&
                        emailOption.OutboundFailurePopupEvent.Contains(eventName)))
                        {
                            ClickToEmailPopupUsingSOAPAPI(emailData, ConnId, callType, data[0], data[1]);
                        }
                    }
                    else
                    {
                        this._logger.Info("GetClickToEmailLogs : Object Name not found : " + data[0]);
                    }

                    #endregion Popup
                }
            }
            catch (Exception generalException)
            {
                _logger.Error("GetClickToEmailLogs : Error Occurred  : " + generalException.ToString());
            }
        }

        public SFDCData GetInboundPopupData(IXNCustomData emailData, SFDCCallType callType, string eventName)
        {
            SFDCData sfdcData = new SFDCData();
            try
            {
                _logger.Info("GetInboundPopupData : Getting Inbound Popup Data");
                if (this._popupPages != null)
                {
                    bool IsCurrentEventEnabled = false;
                    if (Settings.SFDCOptions.CanUseCommonSearchForEmail)
                    {
                        if (Settings.SFDCOptions.EmailInboundPopupEvent != null && Settings.SFDCOptions.EmailInboundPopupEvent.Contains(eventName))
                        {
                            IsCurrentEventEnabled = true;
                            sfdcData.CommonSearchData = GetEmailCommonSearchData(emailData, callType);
                            sfdcData.CommonPopupObjects = Settings.CommonPopupObjects;
                            sfdcData.CommonSearchFormats = Settings.SFDCOptions.PhoneNumberSearchFormat;
                            sfdcData.CommonSearchCondition = Settings.SFDCOptions.CommonSearchConditionForEmail;
                            if (this._userActivityLog != null)
                            {
                                sfdcData.ProfileActivityLogData = this._sfdcUtility.GetCreateActivityLogData(this._userActivityLog, null, callType, emailData: emailData);
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
                                    sfdcData.LeadData = SFDCLead.GetInstance().GetLeadEmailPopupData(emailData, callType);
                                else if (this._leadOptions != null && this._leadOptions.InboundPopupEvent != null && this._leadOptions.InboundPopupEvent.Contains(eventName))
                                    sfdcData.LeadData = SFDCLead.GetInstance().GetLeadEmailPopupData(emailData, callType);
                                break;

                            case "contact":
                                if (IsCurrentEventEnabled)
                                    sfdcData.ContactData = SFDCContact.GetInstance().GetContactEmailPopupData(emailData, callType);
                                else if (this._contactOptions != null && this._contactOptions.InboundPopupEvent != null && this._contactOptions.InboundPopupEvent.Contains(eventName))
                                    sfdcData.ContactData = SFDCContact.GetInstance().GetContactEmailPopupData(emailData, callType);
                                break;

                            case "account":
                                if (IsCurrentEventEnabled)
                                    sfdcData.AccountData = SFDCAccount.GetInstance().GetAccountEmailPopupData(emailData, callType);
                                else if (this._accountOptions != null && this._accountOptions.InboundPopupEvent != null && this._accountOptions.InboundPopupEvent.Contains(eventName))
                                    sfdcData.AccountData = SFDCAccount.GetInstance().GetAccountEmailPopupData(emailData, callType);
                                break;

                            case "case":
                                if (IsCurrentEventEnabled)
                                    sfdcData.CaseData = SFDCCase.GetInstance().GetCaseEmailPopupData(emailData, callType);
                                else if (this._caseOptions != null && this._caseOptions.InboundPopupEvent != null && this._caseOptions.InboundPopupEvent.Contains(eventName))
                                    sfdcData.CaseData = SFDCCase.GetInstance().GetCaseEmailPopupData(emailData, callType);
                                break;

                            case "opportunity":
                                if (IsCurrentEventEnabled)
                                    sfdcData.OpportunityData = SFDCOpportunity.GetInstance().GetOpportunityEmailPopupData(emailData, callType);
                                else if (this._opportunityOptions != null && this._opportunityOptions.InboundPopupEvent != null && this._opportunityOptions.InboundPopupEvent.Contains(eventName))
                                    sfdcData.OpportunityData = SFDCOpportunity.GetInstance().GetOpportunityEmailPopupData(emailData, callType);
                                break;

                            case "useractivity":
                                if (IsCurrentEventEnabled)
                                    sfdcData.UserActivityData = SFDCUserActivity.GetInstance().GetOutboundEmailCreateUserAcitivityData(emailData, callType);
                                else if (this._userActivityOptions != null && this._userActivityOptions.InboundPopupEvent != null && this._userActivityOptions.InboundPopupEvent.Contains(eventName))
                                    sfdcData.UserActivityData = SFDCUserActivity.GetInstance().GetOutboundEmailCreateUserAcitivityData(emailData, callType);
                                break;

                            default:
                                if (this.customObjectOptions != null)
                                {
                                    if (IsCurrentEventEnabled && this.customObjectOptions.ContainsKey(key))
                                    {
                                        CustomObjectData cstobject = SFDCCustomObject.GetInstance().GetCustomObjectEmailPopupData(emailData, callType, key);
                                        if (cstobject != null)
                                        {
                                            this._tempCustomObject.Add(cstobject);
                                        }
                                    }
                                    else if (this.customObjectOptions[key] != null && this.customObjectOptions[key].InboundPopupEvent != null && this.customObjectOptions[key].InboundPopupEvent.Contains(eventName))
                                    {
                                        _logger.Info("GetInboundPopupData : Reads CustomObject Email PopupData");
                                        CustomObjectData cstobject = SFDCCustomObject.GetInstance().GetCustomObjectEmailPopupData(emailData, callType, key);
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

        public SFDCData GetInboundUpdateData(IXNCustomData emailData, SFDCCallType callType, string eventName)
        {
            SFDCData sfdcData = new SFDCData();
            try
            {
                _logger.Info("GetInboundUpdateData : Getting Inbound Update Data");
                if (this._popupPages != null)
                {
                    if (_generalOptions.CanUseCommonSearchForEmail && this._userActivityLog != null)
                    {
                        sfdcData.ProfileUpdateActivityLogData = _sfdcUtility.GetUpdateActivityLogData(this._userActivityLog, null, callType, emailData.Duration, emailData: emailData);
                        sfdcData.ProfileActivityLogAppendData = _sfdcUtility.GetUpdateActivityLogData(this._userActivityLog, null, callType, emailData.Duration, emailData: emailData, isAppendLogData: true);
                    }

                    foreach (string key in this._popupPages)
                    {
                        switch (key)
                        {
                            case "lead":
                                if (this._leadOptions != null && this._leadOptions.InboundUpdateEvent != null && this._leadOptions.InboundUpdateEvent.Contains(eventName))
                                {
                                    _logger.Info("GetInboundUpdateData : Reading Lead popup data");
                                    sfdcData.LeadData = SFDCLead.GetInstance().GetLeadEmailUpdateData(emailData, callType, eventName);
                                }
                                break;

                            case "contact":
                                if (this._contactOptions != null && this._contactOptions.InboundUpdateEvent != null && this._contactOptions.InboundUpdateEvent.Contains(eventName))
                                {
                                    _logger.Info("GetInboundUpdateData : Reading contact popup data");
                                    sfdcData.ContactData = SFDCContact.GetInstance().GetContactEmailUpdateData(emailData, callType, eventName);
                                }
                                break;

                            case "account":
                                if (this._accountOptions != null && this._accountOptions.InboundUpdateEvent != null && this._accountOptions.InboundUpdateEvent.Contains(eventName))
                                {
                                    _logger.Info("GetInboundUpdateData : Reading account popup data");
                                    sfdcData.AccountData = SFDCAccount.GetInstance().GetAccountEmailUpdateData(emailData, callType, eventName);
                                }
                                break;

                            case "case":
                                if (this._caseOptions != null && this._caseOptions.InboundUpdateEvent != null && this._caseOptions.InboundUpdateEvent.Contains(eventName))
                                {
                                    _logger.Info("GetInboundUpdateData : Reading case popup data");
                                    sfdcData.CaseData = SFDCCase.GetInstance().GetCaseEmailUpdateData(emailData, callType, eventName);
                                }
                                break;

                            case "opportunity":
                                if (this._opportunityOptions != null && this._opportunityOptions.InboundUpdateEvent != null && this._opportunityOptions.InboundUpdateEvent.Contains(eventName))
                                {
                                    _logger.Info("GetInboundUpdateData : Reading opportunity popup data");
                                    sfdcData.OpportunityData = SFDCOpportunity.GetInstance().GetOpportunityEmailUpdateData(emailData, callType, eventName);
                                }
                                break;

                            case "useractivity":
                                if (this._userActivityOptions != null && this._userActivityOptions.InboundUpdateEvent != null &&
                    this._userActivityOptions.InboundUpdateEvent.Contains(eventName))
                                {
                                    _logger.Info("GetInboundUpdateData : Reading useractivity popup data");
                                    // sfdcData.UserActivityData = SFDCUserActivity.GetInstance().GetEmailUpdateUserAcitivityData(emailData, callType,null,null);
                                }
                                break;

                            default:
                                if (this.customObjectOptions.ContainsKey(key))
                                {
                                    if (this.customObjectOptions[key] != null && this.customObjectOptions[key].InboundUpdateEvent != null && this.customObjectOptions[key].InboundUpdateEvent.Contains(eventName))
                                    {
                                        CustomObjectData cstobject = SFDCCustomObject.GetInstance().GetCustomObjectEmailUpdateData(emailData, callType, key, eventName);
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
                }

                if (this._tempCustomObject.Count > 0)
                {
                    CustomObjectData[] temp = this._tempCustomObject.ToArray();
                    sfdcData.CustomObjectData = temp;
                    this._tempCustomObject.Clear();
                }
            }
            catch (Exception generalException)
            {
                _logger.Error("GetInboundUpdateData : Error Occurred while Collecting Inbound Update Data : " + generalException.ToString());
            }
            return sfdcData;
        }

        public SFDCData GetOutboundEmailPopupData(IXNCustomData emailData, SFDCCallType callType, string eventName)
        {
            SFDCData sfdcData = new SFDCData();
            try
            {
                this._logger.Info("GetOutboundPopupData : Collecting popup data for outbound on Event : " + eventName);
                if (this._popupPages != null)
                {
                    bool IsCurrentEventEnabled = false;
                    if (Settings.SFDCOptions.CanUseCommonSearchForEmail)
                    {
                        if (Settings.SFDCOptions.EmailOutboundPopupEvent != null && Settings.SFDCOptions.EmailOutboundPopupEvent.Contains(eventName))
                        {
                            IsCurrentEventEnabled = true;
                            sfdcData.CommonSearchData = GetEmailCommonSearchData(emailData, callType);
                            sfdcData.CommonPopupObjects = Settings.CommonPopupObjects;
                            sfdcData.CommonSearchFormats = Settings.SFDCOptions.PhoneNumberSearchFormat;
                            sfdcData.CommonSearchCondition = Settings.SFDCOptions.CommonSearchConditionForVoice;
                            if (this._userActivityLog != null)
                            {
                                sfdcData.ProfileActivityLogData = this._sfdcUtility.GetCreateActivityLogData(this._userActivityLog, null, callType, emailData: emailData);
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
                                    sfdcData.LeadData = SFDCLead.GetInstance().GetLeadEmailPopupData(emailData, callType);
                                else if (this._leadOptions != null && this._leadOptions.OutboundPopupEvent != null && this._leadOptions.OutboundPopupEvent.Contains(eventName))
                                    sfdcData.LeadData = SFDCLead.GetInstance().GetLeadEmailPopupData(emailData, callType);
                                break;

                            case "contact":
                                if (IsCurrentEventEnabled)
                                    sfdcData.ContactData = SFDCContact.GetInstance().GetContactEmailPopupData(emailData, callType);
                                else if (this._contactOptions != null && this._contactOptions.OutboundPopupEvent != null && _contactOptions.OutboundPopupEvent.Contains(eventName))
                                    sfdcData.ContactData = SFDCContact.GetInstance().GetContactEmailPopupData(emailData, callType);
                                break;

                            case "account":
                                if (IsCurrentEventEnabled)
                                    sfdcData.AccountData = SFDCAccount.GetInstance().GetAccountEmailPopupData(emailData, callType);
                                else if (this._accountOptions != null && this._accountOptions.OutboundPopupEvent != null && this._accountOptions.OutboundPopupEvent.Contains(eventName))
                                    sfdcData.AccountData = SFDCAccount.GetInstance().GetAccountEmailPopupData(emailData, callType);
                                break;

                            case "case":
                                if (IsCurrentEventEnabled)
                                    sfdcData.CaseData = SFDCCase.GetInstance().GetCaseEmailPopupData(emailData, callType);
                                else if (this._caseOptions != null && this._caseOptions.OutboundPopupEvent != null && this._caseOptions.OutboundPopupEvent.Contains(eventName))
                                    sfdcData.CaseData = SFDCCase.GetInstance().GetCaseEmailPopupData(emailData, callType);
                                break;

                            case "opportunity":
                                if (IsCurrentEventEnabled)
                                    sfdcData.OpportunityData = SFDCOpportunity.GetInstance().GetOpportunityEmailPopupData(emailData, callType);
                                else if (this._opportunityOptions != null && this._opportunityOptions.OutboundPopupEvent != null && this._opportunityOptions.OutboundPopupEvent.Contains(eventName))
                                    sfdcData.OpportunityData = SFDCOpportunity.GetInstance().GetOpportunityEmailPopupData(emailData, callType);
                                break;

                            case "useractivity":
                                if (IsCurrentEventEnabled)
                                    sfdcData.UserActivityData = SFDCUserActivity.GetInstance().GetOutboundEmailCreateUserAcitivityData(emailData, callType);
                                else if (this._userActivityOptions != null && this._userActivityOptions.OutboundPopupEvent != null && this._userActivityOptions.OutboundPopupEvent.Contains(eventName))
                                    sfdcData.UserActivityData = SFDCUserActivity.GetInstance().GetOutboundEmailCreateUserAcitivityData(emailData, callType);
                                break;

                            default:
                                if (this.customObjectOptions.ContainsKey(key))
                                {
                                    if (IsCurrentEventEnabled)
                                    {
                                        CustomObjectData cstobject = SFDCCustomObject.GetInstance().GetCustomObjectEmailPopupData(emailData, callType, key);
                                        if (cstobject != null)
                                        {
                                            this._tempCustomObject.Add(cstobject);
                                        }
                                    }
                                    else if (this.customObjectOptions[key] != null && this.customObjectOptions[key].OutboundPopupEvent != null && this.customObjectOptions[key].OutboundPopupEvent.Contains(eventName))
                                    {
                                        CustomObjectData cstobject = SFDCCustomObject.GetInstance().GetCustomObjectEmailPopupData(emailData, callType, key);
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
                _logger.Error("GetOutboundPopupData : Error Occurred  : " + generalException.ToString());
            }
            return sfdcData;
        }

        public SFDCData GetOutboundFailurePopupData(IXNCustomData emailData, SFDCCallType callType, string eventName)
        {
            SFDCData sfdcData = new SFDCData();
            try
            {
                this._logger.Info("GetOutboundFailurePopupData : Collecting popup data for outbound failure on Event : " + eventName);
                if (this._popupPages != null)
                {
                    foreach (string key in this._popupPages)
                    {
                        switch (key)
                        {
                            case "lead":
                                if (this._leadOptions != null && this._leadOptions.OutboundFailurePopupEvent != null && this._leadOptions.OutboundFailurePopupEvent.Contains(eventName))
                                {
                                    sfdcData.LeadData = SFDCLead.GetInstance().GetLeadEmailPopupData(emailData, callType);
                                }
                                break;

                            case "contact":
                                if (this._contactOptions != null && this._contactOptions.OutboundFailurePopupEvent != null && _contactOptions.OutboundFailurePopupEvent.Equals(eventName))
                                {
                                    sfdcData.ContactData = SFDCContact.GetInstance().GetContactEmailPopupData(emailData, callType);
                                }
                                break;

                            case "account":
                                if (this._accountOptions != null && this._accountOptions.OutboundFailurePopupEvent != null && this._accountOptions.OutboundFailurePopupEvent.Equals(eventName))
                                {
                                    sfdcData.AccountData = SFDCAccount.GetInstance().GetAccountEmailPopupData(emailData, callType);
                                }
                                break;

                            case "case":
                                if (this._caseOptions != null && this._caseOptions.OutboundFailurePopupEvent != null && this._caseOptions.OutboundFailurePopupEvent.Equals(eventName))
                                {
                                    sfdcData.CaseData = SFDCCase.GetInstance().GetCaseEmailPopupData(emailData, callType);
                                }
                                break;

                            case "opportunity":
                                if (this._opportunityOptions != null && this._opportunityOptions.OutboundFailurePopupEvent != null && this._opportunityOptions.OutboundFailurePopupEvent.Equals(eventName))
                                {
                                    sfdcData.OpportunityData = SFDCOpportunity.GetInstance().GetOpportunityEmailPopupData(emailData, callType);
                                }
                                break;

                            case "useractivity":
                                if (this._userActivityOptions != null && this._userActivityOptions.OutboundFailurePopupEvent != null &&
                    this._userActivityOptions.OutboundFailurePopupEvent.Equals(eventName))
                                {
                                    sfdcData.UserActivityData = SFDCUserActivity.GetInstance().GetOutboundEmailCreateUserAcitivityData(emailData, callType);
                                }
                                break;

                            default:
                                if (this.customObjectOptions.ContainsKey(key))
                                {
                                    if (this.customObjectOptions[key] != null && this.customObjectOptions[key].OutboundFailurePopupEvent != null &&
                                            this.customObjectOptions[key].OutboundFailurePopupEvent.Equals(eventName))
                                    {
                                        CustomObjectData cstobject = SFDCCustomObject.GetInstance().GetCustomObjectEmailPopupData(emailData, callType, key);
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
                _logger.Error("GetOutboundFailurePopupData : Error Occurred  : " + generalException.ToString());
            }
            return sfdcData;
        }

        public SFDCData GetOutboundEmailUpdateData(IXNCustomData emailData, SFDCCallType callType, string eventName)
        {
            SFDCData sfdcData = new SFDCData();
            try
            {
                this._logger.Info("GetOutboundUpdateData : Collecting update data for the Outbound call on Event : " + eventName);
                if (this._popupPages != null)
                {
                    if (_generalOptions.CanUseCommonSearchForEmail && this._userActivityLog != null)
                    {
                        sfdcData.ProfileUpdateActivityLogData = _sfdcUtility.GetUpdateActivityLogData(this._userActivityLog, null, callType, emailData.Duration, emailData: emailData);
                        sfdcData.ProfileActivityLogAppendData = _sfdcUtility.GetUpdateActivityLogData(this._userActivityLog, null, callType, emailData.Duration, emailData: emailData, isAppendLogData: true);
                    }
                    foreach (string key in this._popupPages)
                    {
                        switch (key)
                        {
                            case "lead":
                                if (this._leadOptions != null && this._leadOptions.OutboundUpdateEvent != null && this._leadOptions.OutboundUpdateEvent.Contains(eventName))
                                {
                                    sfdcData.LeadData = SFDCLead.GetInstance().GetLeadEmailUpdateData(emailData, callType, eventName);
                                }
                                break;

                            case "contact":
                                if (this._contactOptions != null && this._contactOptions.OutboundUpdateEvent != null && this._contactOptions.OutboundUpdateEvent.Contains(eventName))
                                {
                                    sfdcData.ContactData = SFDCContact.GetInstance().GetContactEmailUpdateData(emailData, callType, eventName);
                                }
                                break;

                            case "account":
                                if (this._accountOptions != null && this._accountOptions.OutboundUpdateEvent != null && this._accountOptions.OutboundUpdateEvent.Contains(eventName))
                                {
                                    sfdcData.AccountData = SFDCAccount.GetInstance().GetAccountEmailUpdateData(emailData, callType, eventName);
                                }
                                break;

                            case "case":
                                if (this._caseOptions != null && this._caseOptions.OutboundUpdateEvent != null && this._caseOptions.OutboundUpdateEvent.Contains(eventName))
                                {
                                    sfdcData.CaseData = SFDCCase.GetInstance().GetCaseEmailUpdateData(emailData, callType, eventName);
                                }
                                break;

                            case "opportunity":
                                if (this._opportunityOptions != null && this._opportunityOptions.OutboundUpdateEvent != null && this._opportunityOptions.OutboundUpdateEvent.Contains(eventName))
                                {
                                    sfdcData.OpportunityData = SFDCOpportunity.GetInstance().GetOpportunityEmailUpdateData(emailData, callType, eventName);
                                }
                                break;

                            case "useractivity":
                                if (this._userActivityOptions != null && this._userActivityOptions.OutboundUpdateEvent != null &&
                    this._userActivityOptions.OutboundUpdateEvent.Contains(eventName))
                                {
                                    sfdcData.UserActivityData = SFDCUserActivity.GetInstance().GetOutboundEmailUpdateUserAcitivityData(emailData, callType, eventName);
                                }
                                break;

                            default:
                                if (this.customObjectOptions.ContainsKey(key))
                                {
                                    if (this.customObjectOptions[key] != null && this.customObjectOptions[key].OutboundUpdateEvent != null &&
                                            this.customObjectOptions[key].OutboundUpdateEvent.Contains(eventName))
                                    {
                                        CustomObjectData cstobject = SFDCCustomObject.GetInstance().GetCustomObjectEmailUpdateData(emailData, callType, key, eventName);
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
                _logger.Error("GetOutboundUpdateData : Error Occurred  : " + generalException.ToString());
            }
            return sfdcData;
        }

        private void ClickToEmailPopupUsingScript(IXNCustomData emailData, string connId, SFDCCallType callType, string objectType, string objectId)
        {
            try
            {
                this._logger.Info("ClickToEmailPopup : Click-to-email Popup.... ");
                this._logger.Info("ClickToEmailPopup : Call Type  : " + Convert.ToString(callType));
                this._logger.Info("ClickToEmailPopup : SFDC Object Type  : " + objectType);
                this._logger.Info("ClickToEmailPopup : SFDC Object Id  : " + objectId);
                SFDCData data = null;
                if (!string.IsNullOrEmpty(objectType))
                {
                    switch (objectType.ToLower())
                    {
                        case "lead":
                            if (this._leadOptions.OutboundCanCreateLog)
                            {
                                this._logger.Info("ClickToEmailPopup : Create an click2dial activity log data for lead...");
                                data = new SFDCData();
                                data.InteractionId = connId;
                                data.LeadData = new LeadData();
                                data.LeadData.ObjectName = this._leadOptions.ObjectName;
                                data.LeadData.ClickToDialRecordId = objectId;
                                data.LeadData.CreateActvityLog = true;
                                data.LeadData.OutboundDialedFromSFDC = true;
                                KeyValueCollection LeadEmailLogConfig = null;
                                Settings.EmailActivityLogCollection.TryGetValue("lead", out LeadEmailLogConfig);
                                data.LeadData.ActivityLogData = this._sfdcUtility.GetCreateActivityLogData(LeadEmailLogConfig, null, callType, emailData);
                                if (data.LeadData.ActivityLogData != null)
                                {
                                    this._sfdcUtility.CreateActivityLog(data.InteractionId, data.LeadData.ActivityLogData, data.LeadData.ObjectName, callType, objectId);
                                }
                            }
                            break;

                        case "contact":
                            data = new SFDCData();
                            data.InteractionId = connId;
                            data.ContactData = new ContactData();
                            data.ContactData.ObjectName = this._contactOptions.ObjectName;
                            data.ContactData.ClickToDialRecordId = objectId;
                            data.ContactData.CreateActvityLog = true;
                            data.ContactData.OutboundDialedFromSFDC = true;
                            //data.ContactData.ActivityLogData = SFDCUtility.GetOutboundEmailActivityLog(this.activityLogs["contact"], emailData, callType);
                            //Settings.SFDCPopupData.Add(connId, data);
                            break;

                        case "account":
                            data = new SFDCData();
                            data.InteractionId = connId;
                            data.AccountData = new AccountData();
                            data.AccountData.ObjectName = this._accountOptions.ObjectName;
                            data.AccountData.ClickToDialRecordId = objectId;
                            data.AccountData.CreateActvityLog = true;
                            data.AccountData.OutboundDialedFromSFDC = true;
                            //data.AccountData.ActivityLogData = SFDCUtility.GetOutboundEmailActivityLog(this.activityLogs["account"], emailData, callType);
                            //Settings.SFDCPopupData.Add(connId, data);
                            break;

                        case "case":
                            data = new SFDCData();
                            data.InteractionId = connId;
                            data.CaseData = new CaseData();
                            data.CaseData.ObjectName = this._caseOptions.ObjectName;
                            data.CaseData.ClickToDialRecordId = objectId;
                            data.CaseData.CreateActvityLog = true;
                            data.CaseData.OutboundDialedFromSFDC = true;
                            //data.CaseData.ActivityLogData = SFDCUtility.GetOutboundEmailActivityLog(this.activityLogs["case"], emailData, callType);
                            // Settings.SFDCPopupData.Add(connId, data);
                            break;

                        case "opportunity":
                            data = new SFDCData();
                            data.InteractionId = connId;
                            data.OpportunityData = new OpportunityData();
                            data.OpportunityData.ObjectName = this._opportunityOptions.ObjectName;
                            data.OpportunityData.ClickToDialRecordId = objectId;
                            data.OpportunityData.CreateActvityLog = true;
                            data.OpportunityData.OutboundDialedFromSFDC = true;
                            //data.OpportunityData.ActivityLogData = SFDCUtility.GetOutboundEmailActivityLog(this.activityLogs["opportunity"], emailData, callType);
                            //Settings.SFDCPopupData.Add(connId, data);
                            break;

                        default:

                            if (objectType.Contains("__c") && Settings.CustomObjectNames.ContainsKey(objectType))
                            {
                                string objectName = Settings.CustomObjectNames[objectType];
                                if (this.activityLogs.ContainsKey(objectName))
                                {
                                    data = new SFDCData();
                                    data.InteractionId = connId;
                                    CustomObjectData[] customObject = new CustomObjectData[1];
                                    CustomObjectData custom = new CustomObjectData();
                                    custom.ObjectName = this.customObjectOptions[objectName].ObjectName;
                                    custom.ClickToDialRecordId = objectId;
                                    custom.CreateActvityLog = true;
                                    custom.OutboundDialedFromSFDC = true;
                                    //custom.ActivityLogData = SFDCUtility.GetOutboundEmailActivityLog(this.activityLogs[objectName], emailData, callType);
                                    customObject[0] = custom;
                                    data.CustomObjectData = customObject;
                                    // Settings.SFDCPopupData.Add(connId, data);
                                }
                                else
                                {
                                    this._logger.Info("ClickToEmailPopup : " + objectName + " Configuration not found ");
                                }
                            }
                            else
                            {
                                this._logger.Info("ClickToEmailPopup : Custom Object not found with Name : " + objectType);
                            }
                            break;
                    }
                }
                else
                {
                    _logger.Error("ClickToEmailPopup : SFDC Object Type is null or empty ");
                }
            }
            catch (Exception generalException)
            {
                _logger.Error("ClickToEmailPopup : Error Occurred  : " + generalException.ToString());
            }
        }

        private void ClickToEmailPopupUsingSOAPAPI(IXNCustomData emailData, string connId, SFDCCallType callType, string objectType, string objectId)
        {
            try
            {
                this._logger.Info("ClickToEmailPopup : Click-to-email using SOAP API.... ");
                this._logger.Info("ClickToEmailPopup : Call Type  : " + Convert.ToString(callType));
                this._logger.Info("ClickToEmailPopup : SFDC Object Type  : " + objectType);
                this._logger.Info("ClickToEmailPopup : SFDC Object Id  : " + objectId);
                if (!string.IsNullOrEmpty(objectType))
                {
                    List<XmlElement> element = new List<XmlElement>();
                    KeyValueCollection EmailLogConfig = null;
                    switch (objectType.ToLower())
                    {
                        case "lead":
                            if (this._leadOptions.OutboundCanCreateLog)
                            {
                                this._logger.Info("ClickToEmailPopupUsingSOAPAPI : Creating click2email activity log for lead...");
                                Settings.EmailActivityLogCollection.TryGetValue("lead", out EmailLogConfig);
                                element = this._sfdcUtility.GetCreateActivityLogData(EmailLogConfig, null, callType, emailData);
                                if (element != null)
                                {
                                    this._sfdcUtility.CreateActivityLog(connId, element, objectType, callType, objectId);
                                }
                                else
                                    this._logger.Info("Log configuration not found for the lead object");
                            }
                            break;

                        case "contact":
                            if (this._contactOptions.OutboundCanCreateLog)
                            {
                                this._logger.Info("ClickToEmailPopupUsingSOAPAPI : Creating click2email activity log for contact...");
                                Settings.EmailActivityLogCollection.TryGetValue("contact", out EmailLogConfig);
                                element = this._sfdcUtility.GetCreateActivityLogData(EmailLogConfig, null, callType, emailData);
                                if (element != null)
                                {
                                    this._sfdcUtility.CreateActivityLog(connId, element, objectType, callType, objectId);
                                }
                                else
                                    this._logger.Info("Log configuration not found for the contact object");
                            }
                            break;

                        case "account":
                            if (this._accountOptions.OutboundCanCreateLog)
                            {
                                this._logger.Info("ClickToEmailPopupUsingSOAPAPI : Creating click2email activity log for account...");
                                Settings.EmailActivityLogCollection.TryGetValue("account", out EmailLogConfig);
                                element = this._sfdcUtility.GetCreateActivityLogData(EmailLogConfig, null, callType, emailData);
                                if (element != null)
                                {
                                    this._sfdcUtility.CreateActivityLog(connId, element, objectType, callType, objectId);
                                }
                                else
                                    this._logger.Info("Log configuration not found for the account object");
                            }
                            break;

                        case "case":
                            if (this._caseOptions.OutboundCanCreateLog)
                            {
                                this._logger.Info("ClickToEmailPopupUsingSOAPAPI : Creating click2email activity log for case...");
                                Settings.EmailActivityLogCollection.TryGetValue("case", out EmailLogConfig);
                                element = this._sfdcUtility.GetCreateActivityLogData(EmailLogConfig, null, callType, emailData);
                                if (element != null)
                                {
                                    this._sfdcUtility.CreateActivityLog(connId, element, objectType, callType, objectId);
                                }
                                else
                                    this._logger.Info("Log configuration not found for the case object");
                            }
                            break;

                        case "opportunity":
                            if (this._opportunityOptions.OutboundCanCreateLog)
                            {
                                this._logger.Info("ClickToEmailPopupUsingSOAPAPI : Creating click2email activity log for opportunity...");
                                Settings.EmailActivityLogCollection.TryGetValue("opportunity", out EmailLogConfig);
                                element = this._sfdcUtility.GetCreateActivityLogData(EmailLogConfig, null, callType, emailData);
                                if (element != null)
                                {
                                    this._sfdcUtility.CreateActivityLog(connId, element, objectType, callType, objectId);
                                }
                                else
                                    this._logger.Info("Log configuration not found for the opportunity object");
                            }
                            break;

                        default:

                            if (objectType.Contains("__c") && Settings.CustomObjectNames.ContainsKey(objectType))
                            {
                                string objectName = Settings.CustomObjectNames[objectType];
                                if (this.activityLogs.ContainsKey(objectName))
                                {
                                    if (this.customObjectOptions[objectName] != null && this.customObjectOptions[objectName].OutboundCanCreateLog)
                                    {
                                        this._logger.Info("ClickToEmailPopupUsingSOAPAPI : Creating click2email activity log for " + objectType);
                                        Settings.EmailActivityLogCollection.TryGetValue(objectName, out EmailLogConfig);
                                        element = this._sfdcUtility.GetCreateActivityLogData(EmailLogConfig, null, callType, emailData);
                                        if (element != null)
                                        {
                                            this._sfdcUtility.CreateActivityLog(connId, element, objectType, callType, objectId);
                                        }
                                        else
                                            this._logger.Info("Log configuration not found for the " + objectType + " object");
                                    }
                                }
                                else
                                {
                                    this._logger.Info("ClickToEmailPopupUsingSOAPAPI: Log Configuration for " + objectName + " is not found ");
                                }
                            }
                            else
                            {
                                this._logger.Info("ClickToEmailPopupUsingSOAPAPI: Custom Object not found with Name : " + objectType);
                            }
                            break;
                    }
                }
                else
                {
                    _logger.Error("ClickToEmailPopupUsingSOAPAPI: SFDC Object Type is null or empty ");
                }
            }
            catch (Exception generalException)
            {
                _logger.Error("ClickToEmailPopupUsingSOAPAPI: Error Occurred  : " + generalException.ToString());
            }
        }

        internal string GetEmailCommonSearchData(IXNCustomData emailData, SFDCCallType callType)
        {
            try
            {
                this._logger.Info("GetEmailCommonSearchData :  Reading Email Common search data.....");
                this._logger.Info("GetEmailCommonSearchData :  Event Name : " + emailData.EventName);
                this._logger.Info("GetEmailCommonSearchData :  CallType : " + callType.ToString());
                string[] userDataSearchKeys = null;
                string[] attributeSearchKeys = null;
                string searchValue = string.Empty;
                if (callType == SFDCCallType.InboundEmail || callType == SFDCCallType.InboundEmailPulled)
                {
                    userDataSearchKeys = (this._generalOptions.EmailInboundSearchUserDataKeys != null) ? this._generalOptions.EmailInboundSearchUserDataKeys.Split(',') : null;
                    attributeSearchKeys = (this._generalOptions.EmailInboundSearchAttributeKeys != null) ? this._generalOptions.EmailInboundSearchAttributeKeys.Split(',') : null;
                }
                else if (callType == SFDCCallType.OutboundEmailSuccess || callType == SFDCCallType.OutboundEmailFailure || callType == SFDCCallType.OutboundEmailPulled)
                {
                    userDataSearchKeys = (this._generalOptions.EmailOutboundSearchUserDataKeys != null) ? this._generalOptions.EmailOutboundSearchUserDataKeys.Split(',') : null;
                    attributeSearchKeys = (this._generalOptions.EmailOutboundSearchAttributeKeys != null) ? this._generalOptions.EmailOutboundSearchAttributeKeys.Split(',') : null;
                }

                if (this._generalOptions.EmailSearchPriority == "user-data" && emailData.UserData != null)
                {
                    if (userDataSearchKeys != null)
                    {
                        searchValue = this._sfdcUtilityHelper.GetUserDataSearchValues(emailData.UserData, userDataSearchKeys);
                        if (!this._sfdcUtilityHelper.ValidateSearchData(searchValue))
                        {
                            this._logger.Info("search data from user-data keys not found, Reading attribute search keys.....");
                            searchValue = this._sfdcUtilityHelper.GetEmailAttributeValueForSearch(emailData, attributeSearchKeys);
                        }
                    }
                    else if (attributeSearchKeys != null)
                    {
                        this._logger.Info("Either user-data search keys or user-data is null, Reading attribute search keys.....");
                        searchValue = this._sfdcUtilityHelper.GetEmailAttributeValueForSearch(emailData, attributeSearchKeys);
                    }
                }
                else if (this._generalOptions.VoiceSearchPriority == "attribute")
                {
                    searchValue = this._sfdcUtilityHelper.GetEmailAttributeValueForSearch(emailData, attributeSearchKeys);
                    if (!this._sfdcUtilityHelper.ValidateSearchData(searchValue))
                    {
                        this._logger.Info("search data from attribute keys not found, Reading user-data search keys.....");
                        if (userDataSearchKeys != null && emailData.UserData != null)
                        {
                            searchValue = this._sfdcUtilityHelper.GetUserDataSearchValues(emailData.UserData, userDataSearchKeys);
                        }
                        else
                        {
                            this._logger.Info("Either user-data or search keys are not found.....");
                        }
                    }
                }
                else if (this._generalOptions.VoiceSearchPriority == "both")
                {
                    if (emailData.UserData != null)
                        searchValue = this._sfdcUtilityHelper.GetUserDataSearchValues(emailData.UserData, userDataSearchKeys);
                    if (!this._sfdcUtilityHelper.ValidateSearchData(searchValue))
                    {
                        string temp = this._sfdcUtilityHelper.GetEmailAttributeValueForSearch(emailData, attributeSearchKeys);
                        if (temp != string.Empty)
                        {
                            searchValue += "," + temp;
                        }
                    }
                    else
                    {
                        searchValue = this._sfdcUtilityHelper.GetEmailAttributeValueForSearch(emailData, attributeSearchKeys);
                    }
                }
                return searchValue;
            }
            catch (Exception generalException)
            {
                this._logger.Error("GetEmailCommonSearchData : Error occurred while reading Lead Data : " + generalException.ToString());
            }
            return null;
        }
    }
}