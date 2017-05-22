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

using Genesyslab.Platform.Commons.Collections;
using Genesyslab.Platform.Commons.Protocols;
using Pointel.Salesforce.Adapter.Configurations;
using Pointel.Salesforce.Adapter.LogMessage;
using Pointel.Salesforce.Adapter.SFDCModels;
using Pointel.Salesforce.Adapter.SFDCUtils;
using Pointel.Salesforce.Adapter.Utility;
using System;
using System.Collections.Generic;
using System.Xml;

namespace Pointel.Salesforce.Adapter.Voice
{
    internal class VoiceManager
    {
        #region Fields

        private static VoiceManager _currentObject = null;
        private VoiceOptions _accountOptions = null;
        private IDictionary<string, KeyValueCollection> _activityLogs = null;
        private VoiceOptions _caseOptions = null;
        private VoiceOptions _contactOptions = null;
        private IDictionary<string, VoiceOptions> _customObjectOptions = null;
        private GeneralOptions _generalOptions = null;
        private VoiceOptions _leadOptions = null;
        private Log _logger = null;
        private VoiceOptions _opportunityOptions = null;
        private string[] _PopupPages = null;
        private SFDCUtility _sFDCUtility = null;
        private SFDCUtiltiyHelper _sfdcUtilityHelper = null;
        private List<CustomObjectData> _tempCustomObject = new List<CustomObjectData>();
        private KeyValueCollection _userActivityLog = null;
        private UserActivityOptions _userActivityOptions = null;

        #endregion Fields

        #region Constructor

        private VoiceManager()
        {
            this._logger = Log.GenInstance();
            this._leadOptions = Settings.LeadVoiceOptions;
            this._contactOptions = Settings.ContactVoiceOptions;
            this._accountOptions = Settings.AccountVoiceOptions;
            this._caseOptions = Settings.CaseVoiceOptions;
            this._opportunityOptions = Settings.OpportunityVoiceOptions;
            this._userActivityLog = (Settings.VoiceActivityLogCollection.ContainsKey("useractivity")) ? Settings.VoiceActivityLogCollection["useractivity"] : null;
            this._sFDCUtility = SFDCUtility.GetInstance();
            this._activityLogs = Settings.VoiceActivityLogCollection;
            this._userActivityOptions = Settings.UserActivityVoiceOptions;
            this._PopupPages = Settings.SFDCOptions.SFDCPopupPages;
            this._generalOptions = Settings.SFDCOptions;
            this._sfdcUtilityHelper = SFDCUtiltiyHelper.GetInstance();
            this._customObjectOptions = Settings.CustomObjectVoiceOptions;
        }

        #endregion Constructor

        #region GetInstance

        public static VoiceManager GetInstance()
        {
            if (_currentObject == null)
            {
                _currentObject = new VoiceManager();
            }
            return _currentObject;
        }

        #endregion GetInstance

        #region Inbound Pop-up Data

        public SFDCData GetInboundPopupData(IMessage message, SFDCCallType callType, string eventName)
        {
            SFDCData sfdcData = new SFDCData();
            try
            {
                this._logger.Info("GetInboundPopupData : Collecting pop-up data for Inbound on Event : " + eventName);
                if (this._PopupPages != null)
                {
                    if (_generalOptions.CanUseCommonSearchForVoice)
                    {
                        if (this._generalOptions.InboundPopupEvent != null && this._generalOptions.InboundPopupEvent.Equals(eventName))
                        {
                            sfdcData.CommonSearchData = GetCommonVoiceSearchValue(message, callType);
                            sfdcData.CommonPopupObjects = Settings.CommonPopupObjects;
                            sfdcData.CommonSearchFormats = this._generalOptions.PhoneNumberSearchFormat;
                            sfdcData.CommonSearchCondition = this._generalOptions.CommonSearchConditionForVoice;
                            if (this._userActivityLog != null)
                            {
                                sfdcData.ProfileActivityLogData = _sFDCUtility.GetCreateActivityLogData(this._userActivityLog, message, callType);
                            }
                            foreach (string key in this._PopupPages)
                            {
                                switch (key)
                                {
                                    case "lead":
                                        if (this._leadOptions != null)
                                        {
                                            _logger.Info("GetInboundPopupData : Collecting Inbound Voice Pop-up data for Lead");
                                            sfdcData.LeadData = SFDCLead.GetInstance().GetLeadVoicePopupData(message, callType);
                                        }
                                        else
                                        {
                                            _logger.Warn("GetInboundPopupData : Lead Options is empty");
                                        }
                                        break;

                                    case "contact":
                                        if (this._contactOptions != null)
                                        {
                                            _logger.Info("GetInboundPopupData : Collecting Inbound Voice Pop-up data for contact");
                                            sfdcData.ContactData = SFDCContact.GetInstance().GetContactVoicePopupData(message, callType);
                                        }
                                        else
                                        {
                                            _logger.Warn("GetInboundPopupData : contact Options is empty");
                                        }
                                        break;

                                    case "account":
                                        if (this._accountOptions != null)
                                        {
                                            _logger.Info("GetInboundPopupData : Collecting Inbound Voice Pop-up data for account");
                                            sfdcData.AccountData = SFDCAccount.GetInstance().GetAccountVoicePopupData(message, callType);
                                        }
                                        else
                                        {
                                            _logger.Warn("GetInboundPopupData : account Options is empty");
                                        }
                                        break;

                                    case "case":
                                        if (this._caseOptions != null)
                                        {
                                            _logger.Info("GetInboundPopupData : Collecting Inbound Voice Pop-up data for case");
                                            sfdcData.CaseData = SFDCCase.GetInstance().GetCaseVoicePopupData(message, callType);
                                        }
                                        else
                                        {
                                            _logger.Warn("GetInboundPopupData : case Options is empty");
                                        }
                                        break;

                                    case "opportunity":
                                        if (this._opportunityOptions != null)
                                        {
                                            _logger.Info("GetInboundPopupData : Collecting Inbound Voice Pop-up data for opportunity");
                                            sfdcData.OpportunityData = SFDCOpportunity.GetInstance().GetOpportunityVoicePopupData(message, callType);
                                        }
                                        else
                                        {
                                            _logger.Warn("GetInboundPopupData : opportunity Options is empty");
                                        }
                                        break;

                                    case "useractivity":
                                        if (this._userActivityOptions != null)
                                        {
                                            _logger.Info("GetInboundPopupData : Collecting Inbound Voice Pop-up data for useractivity");
                                            sfdcData.UserActivityData = SFDCUserActivity.GetInstance().GetVoiceCreateUserAcitivityData(message, callType);
                                        }
                                        else
                                        {
                                            _logger.Warn("GetInboundPopupData : useractivity Options is empty");
                                        }
                                        break;

                                    default:
                                        if (_customObjectOptions != null)
                                        {
                                            if (this._customObjectOptions.ContainsKey(key))
                                            {
                                                CustomObjectData cstobject = SFDCCustomObject.GetInstance().GetCustomObjectVoicePopupData(message, callType, key);
                                                if (cstobject != null)
                                                {
                                                    this._tempCustomObject.Add(cstobject);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            _logger.Warn("GetInboundPopupData : customObjectOptions Options is empty");
                                        }
                                        break;
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (string key in this._PopupPages)
                        {
                            switch (key)
                            {
                                case "lead":
                                    if (this._leadOptions != null && this._leadOptions.InboundPopupEvent != null && this._leadOptions.InboundPopupEvent.Equals(eventName))
                                    {
                                        _logger.Info("GetInboundPopupData : Collecting Inbound Voice Pop-up data for Lead");
                                        sfdcData.LeadData = SFDCLead.GetInstance().GetLeadVoicePopupData(message, callType);
                                    }
                                    break;

                                case "contact":
                                    if (this._contactOptions != null && this._contactOptions.InboundPopupEvent != null && this._contactOptions.InboundPopupEvent.Equals(eventName))
                                    {
                                        _logger.Info("GetInboundPopupData : Collecting Inbound Voice Pop-up data for contact");
                                        sfdcData.ContactData = SFDCContact.GetInstance().GetContactVoicePopupData(message, callType);
                                    }
                                    break;

                                case "account":
                                    if (this._accountOptions != null && this._accountOptions.InboundPopupEvent != null && this._accountOptions.InboundPopupEvent.Equals(eventName))
                                    {
                                        _logger.Info("GetInboundPopupData : Collecting Inbound Voice Pop-up data for account");
                                        sfdcData.AccountData = SFDCAccount.GetInstance().GetAccountVoicePopupData(message, callType);
                                    }
                                    break;

                                case "case":
                                    if (this._caseOptions != null && this._caseOptions.InboundPopupEvent != null && this._caseOptions.InboundPopupEvent.Equals(eventName))
                                    {
                                        _logger.Info("GetInboundPopupData : Collecting Inbound Voice Pop-up data for case");
                                        sfdcData.CaseData = SFDCCase.GetInstance().GetCaseVoicePopupData(message, callType);
                                    }
                                    break;

                                case "opportunity":
                                    if (this._opportunityOptions != null && this._opportunityOptions.InboundPopupEvent != null && this._opportunityOptions.InboundPopupEvent.Equals(eventName))
                                    {
                                        _logger.Info("GetInboundPopupData : Collecting Inbound Voice Pop-up data for opportunity");
                                        sfdcData.OpportunityData = SFDCOpportunity.GetInstance().GetOpportunityVoicePopupData(message, callType);
                                    }
                                    break;

                                case "useractivity":
                                    if (this._userActivityOptions != null && this._userActivityOptions.InboundPopupEvent != null &&
                        this._userActivityOptions.InboundPopupEvent.Equals(eventName))
                                    {
                                        _logger.Info("GetInboundPopupData : Collecting Inbound Voice Pop-up data for useractivity");
                                        sfdcData.UserActivityData = SFDCUserActivity.GetInstance().GetVoiceCreateUserAcitivityData(message, callType);
                                    }
                                    break;

                                default:
                                    if (this._customObjectOptions.ContainsKey(key))
                                    {
                                        if (this._customObjectOptions[key] != null && this._customObjectOptions[key].InboundPopupEvent != null &&
                                                this._customObjectOptions[key].InboundPopupEvent.Equals(eventName))
                                        {
                                            CustomObjectData cstobject = SFDCCustomObject.GetInstance().GetCustomObjectVoicePopupData(message, callType, key);
                                            if (cstobject != null)
                                            {
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
                        _logger.Info("GetInboundPopupData : Collecting Inbound Voice Pop-up data for CustomObject");
                        CustomObjectData[] temp = this._tempCustomObject.ToArray();
                        sfdcData.CustomObjectData = temp;
                        this._tempCustomObject.Clear();
                    }
                }
            }
            catch (Exception generalException)
            {
                _logger.Error("GetInboundPopupData : Error Occurred while collecting Inbound Pop-up data : " + generalException.ToString());
            }
            return sfdcData;
        }

        #endregion Inbound Pop-up Data

        #region Consult Received Pop-up Data

        public SFDCData GetConsultReceivedPopupData(IMessage message, SFDCCallType callType, string eventName)
        {
            SFDCData sfdcData = new SFDCData();
            try
            {
                this._logger.Info("GetConsultReceivedPopupData : Collecting pop-up data for Consult Received on Event : " + eventName);
                if (this._PopupPages != null)
                {
                    if (_generalOptions.CanUseCommonSearchForVoice)
                    {
                        if (this._generalOptions.ConsultPopupEvent != null && this._generalOptions.ConsultPopupEvent.Equals(eventName))
                        {
                            sfdcData.CommonSearchData = GetCommonVoiceSearchValue(message, callType);
                            sfdcData.CommonPopupObjects = Settings.CommonPopupObjects;
                            sfdcData.CommonSearchFormats = this._generalOptions.PhoneNumberSearchFormat;
                            sfdcData.CommonSearchCondition = this._generalOptions.CommonSearchConditionForVoice;
                            if (this._userActivityLog != null)
                                sfdcData.ProfileActivityLogData = _sFDCUtility.GetCreateActivityLogData(this._userActivityLog, message, callType);

                            foreach (string key in this._PopupPages)
                            {
                                switch (key)
                                {
                                    case "lead":
                                        if (_leadOptions != null)
                                        {
                                            _logger.Info("GetConsultReceivedPopupData : Collecting Consult Received Voice Pop-up data for Lead");
                                            sfdcData.LeadData = SFDCLead.GetInstance().GetLeadVoicePopupData(message, callType);
                                        }
                                        else
                                        {
                                            _logger.Warn("GetConsultReceivedPopupData : Lead Options is empty");
                                        }
                                        break;

                                    case "contact":
                                        if (_contactOptions != null)
                                        {
                                            _logger.Info("GetConsultReceivedPopupData : Collecting Consult Received Voice Pop-up data for contact");
                                            sfdcData.ContactData = SFDCContact.GetInstance().GetContactVoicePopupData(message, callType);
                                        }
                                        else
                                        {
                                            _logger.Warn("GetConsultReceivedPopupData : Contact Options is empty");
                                        }
                                        break;

                                    case "account":
                                        if (_accountOptions != null)
                                        {
                                            _logger.Info("GetConsultReceivedPopupData : Collecting Consult Received Voice Pop-up data for account");
                                            sfdcData.AccountData = SFDCAccount.GetInstance().GetAccountVoicePopupData(message, callType);
                                        }
                                        else
                                        {
                                            _logger.Warn("GetConsultReceivedPopupData : Account Options is empty");
                                        }
                                        break;

                                    case "case":
                                        if (_caseOptions != null)
                                        {
                                            _logger.Info("GetConsultReceivedPopupData : Collecting Consult Received Voice Pop-up data for case");
                                            sfdcData.CaseData = SFDCCase.GetInstance().GetCaseVoicePopupData(message, callType);
                                        }
                                        else
                                        {
                                            _logger.Warn("GetConsultReceivedPopupData : case Options is empty");
                                        }
                                        break;

                                    case "opportunity":
                                        if (_opportunityOptions != null)
                                        {
                                            _logger.Info("GetConsultReceivedPopupData : Collecting Consult Received Voice Pop-up data for opportunity");
                                            sfdcData.OpportunityData = SFDCOpportunity.GetInstance().GetOpportunityVoicePopupData(message, callType);
                                        }
                                        else
                                        {
                                            _logger.Warn("GetConsultReceivedPopupData : Opportunity Options is empty");
                                        }
                                        break;

                                    case "useractivity":
                                        if (_userActivityOptions != null)
                                        {
                                            _logger.Info("GetConsultReceivedPopupData : Collecting Consult Received Voice Pop-up data for user activity");
                                            sfdcData.UserActivityData = SFDCUserActivity.GetInstance().GetVoiceCreateUserAcitivityData(message, callType);
                                        }
                                        else
                                        {
                                            _logger.Warn("GetConsultReceivedPopupData : User activity Options is empty");
                                        }
                                        break;

                                    default:
                                        if (_customObjectOptions != null)
                                        {
                                            if (this._customObjectOptions.ContainsKey(key))
                                            {
                                                CustomObjectData cstobject = SFDCCustomObject.GetInstance().GetCustomObjectVoicePopupData(message, callType, key);
                                                if (cstobject != null)
                                                {
                                                    this._tempCustomObject.Add(cstobject);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            _logger.Warn("GetConsultReceivedPopupData : CustomObjectOptions Options is empty");
                                        }
                                        break;
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (string key in this._PopupPages)
                        {
                            switch (key)
                            {
                                case "lead":
                                    if (this._leadOptions != null && this._leadOptions.ConsultPopupEvent != null &&
                        this._leadOptions.ConsultPopupEvent.Equals(eventName))
                                    {
                                        _logger.Info("GetConsultReceivedPopupData : Collecting Consult received pop-up data for Lead");
                                        sfdcData.LeadData = SFDCLead.GetInstance().GetLeadVoicePopupData(message, callType);
                                    }
                                    break;

                                case "contact":
                                    if (this._contactOptions != null && this._contactOptions.ConsultPopupEvent != null &&
                        this._contactOptions.ConsultPopupEvent.Equals(eventName))
                                    {
                                        _logger.Info("GetConsultReceivedPopupData : Collecting Consult received pop-up data for Contact");
                                        sfdcData.ContactData = SFDCContact.GetInstance().GetContactVoicePopupData(message, callType);
                                    }
                                    break;

                                case "account":
                                    if (this._accountOptions != null && this._accountOptions.ConsultPopupEvent != null &&
                       this._accountOptions.ConsultPopupEvent.Equals(eventName))
                                    {
                                        _logger.Info("GetConsultReceivedPopupData : Collecting Consult received pop-up data for Account");
                                        sfdcData.AccountData = SFDCAccount.GetInstance().GetAccountVoicePopupData(message, callType);
                                    }
                                    break;

                                case "case":
                                    if (this._caseOptions != null && this._caseOptions.ConsultPopupEvent != null &&
                        this._caseOptions.ConsultPopupEvent.Equals(eventName))
                                    {
                                        _logger.Info("GetConsultReceivedPopupData : Collecting Consult received pop-up data for Case");
                                        sfdcData.CaseData = SFDCCase.GetInstance().GetCaseVoicePopupData(message, callType);
                                    }
                                    break;

                                case "opportunity":
                                    if (this._opportunityOptions != null && this._opportunityOptions.ConsultPopupEvent != null &&
                       this._opportunityOptions.ConsultPopupEvent.Equals(eventName))
                                    {
                                        _logger.Info("GetConsultReceivedPopupData : Collecting Consult received pop-up data for Opportunity");
                                        sfdcData.OpportunityData = SFDCOpportunity.GetInstance().GetOpportunityVoicePopupData(message, callType);
                                    }
                                    break;

                                case "useractivity":
                                    if (this._userActivityOptions != null && this._userActivityOptions.ConsultPopupEvent != null &&
                       this._userActivityOptions.ConsultPopupEvent.Equals(eventName))
                                    {
                                        _logger.Info("GetConsultReceivedPopupData : Collecting Consult received pop-up data for User activity");
                                        sfdcData.UserActivityData = SFDCUserActivity.GetInstance().GetVoiceCreateUserAcitivityData(message, callType);
                                    }
                                    break;

                                default:
                                    if (this._customObjectOptions.ContainsKey(key))
                                    {
                                        if (this._customObjectOptions[key] != null && this._customObjectOptions[key].ConsultPopupEvent != null &&
                                                this._customObjectOptions[key].ConsultPopupEvent.Equals(eventName))
                                        {
                                            CustomObjectData cstobject = SFDCCustomObject.GetInstance().GetCustomObjectVoicePopupData(message, callType, key);
                                            if (cstobject != null)
                                            {
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
                        _logger.Info("GetConsultReceivedPopupData : Collecting Consult received pop-up data for CustomObject");
                        CustomObjectData[] temp = this._tempCustomObject.ToArray();
                        sfdcData.CustomObjectData = temp;
                        this._tempCustomObject.Clear();
                    }
                }
            }
            catch (Exception generalException)
            {
                _logger.Error("GetConsultReceivedPopupData : Error Occurred while collecting Consult Received Pop-up Data : " + generalException.ToString());
            }
            return sfdcData;
        }

        #endregion Consult Received Pop-up Data

        #region Outbound Pop-up Data

        public SFDCData GetOutboundPopupData(IMessage message, SFDCCallType callType, string eventName)
        {
            SFDCData sfdcData = new SFDCData();
            try
            {
                this._logger.Info("GetOutboundPopupData : Collecting pop-up data for outbound on Event : " + eventName);
                if (this._PopupPages != null)
                {
                    if (_generalOptions.CanUseCommonSearchForVoice)
                    {
                        if (this._generalOptions.OutboundPopupEvent != null && this._generalOptions.OutboundPopupEvent.Equals(eventName))
                        {
                            sfdcData.CommonSearchData = GetCommonVoiceSearchValue(message, callType);
                            sfdcData.CommonPopupObjects = Settings.CommonPopupObjects;
                            sfdcData.CommonSearchFormats = this._generalOptions.PhoneNumberSearchFormat;
                            sfdcData.CommonSearchCondition = this._generalOptions.CommonSearchConditionForVoice;
                            if (this._userActivityLog != null)
                            {
                                sfdcData.ProfileActivityLogData = _sFDCUtility.GetCreateActivityLogData(this._userActivityLog, message, callType);
                            }
                            foreach (string key in this._PopupPages)
                            {
                                switch (key)
                                {
                                    case "lead":
                                        if (_leadOptions != null)
                                        {
                                            _logger.Info("GetOutboundPopupData : Collecting Outbound Voice Pop-up data for Lead");
                                            sfdcData.LeadData = SFDCLead.GetInstance().GetLeadVoicePopupData(message, callType);
                                        }
                                        else
                                        {
                                            _logger.Warn("GetOutboundPopupData : Lead Options is empty");
                                        }
                                        break;

                                    case "contact":
                                        if (_contactOptions != null)
                                        {
                                            _logger.Info("GetOutboundPopupData : Collecting Outbound Voice Pop-up data for contact");
                                            sfdcData.ContactData = SFDCContact.GetInstance().GetContactVoicePopupData(message, callType);
                                        }
                                        else
                                        {
                                            _logger.Warn("GetOutboundPopupData : Contact Options is empty");
                                        }
                                        break;

                                    case "account":
                                        if (_accountOptions != null)
                                        {
                                            _logger.Info("GetOutboundPopupData : Collecting Outbound Voice Pop-up data for account");
                                            sfdcData.AccountData = SFDCAccount.GetInstance().GetAccountVoicePopupData(message, callType);
                                        }
                                        else
                                        {
                                            _logger.Warn("GetOutboundPopupData : Account Options is empty");
                                        }
                                        break;

                                    case "case":
                                        if (_caseOptions != null)
                                        {
                                            _logger.Info("GetOutboundPopupData : Collecting Outbound Voice Pop-up data for case");
                                            sfdcData.CaseData = SFDCCase.GetInstance().GetCaseVoicePopupData(message, callType);
                                        }
                                        else
                                        {
                                            _logger.Warn("GetOutboundPopupData : Case Options is empty");
                                        }
                                        break;

                                    case "opportunity":
                                        if (_opportunityOptions != null)
                                        {
                                            _logger.Info("GetOutboundPopupData : Collecting Outbound Voice Pop-up data for opportunity");
                                            sfdcData.OpportunityData = SFDCOpportunity.GetInstance().GetOpportunityVoicePopupData(message, callType);
                                        }
                                        else
                                        {
                                            _logger.Warn("GetOutboundPopupData : Opportunity Options is empty");
                                        }
                                        break;

                                    case "useractivity":
                                        if (_userActivityOptions != null)
                                        {
                                            _logger.Info("GetOutboundPopupData : Collecting Outbound Voice Pop-up data for useractivity");
                                            sfdcData.UserActivityData = SFDCUserActivity.GetInstance().GetVoiceCreateUserAcitivityData(message, callType);
                                        }
                                        else
                                        {
                                            _logger.Warn("GetOutboundPopupData : Useractivity Options is empty");
                                        }
                                        break;

                                    default:
                                        if (_customObjectOptions != null)
                                        {
                                            if (this._customObjectOptions.ContainsKey(key))
                                            {
                                                CustomObjectData cstobject = SFDCCustomObject.GetInstance().GetCustomObjectVoicePopupData(message, callType, key);
                                                if (cstobject != null)
                                                {
                                                    this._tempCustomObject.Add(cstobject);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            _logger.Warn("GetOutboundPopupData : CustomObjectOptions Options is empty");
                                        }
                                        break;
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (string key in this._PopupPages)
                        {
                            switch (key)
                            {
                                case "lead":
                                    if (this._leadOptions != null && this._leadOptions.OutboundPopupEvent != null && this._leadOptions.OutboundPopupEvent.Equals(eventName))
                                    {
                                        _logger.Info("GetOutboundPopupData : Collecting Outbound Pop-up Data for Lead");
                                        sfdcData.LeadData = SFDCLead.GetInstance().GetLeadVoicePopupData(message, callType);
                                    }
                                    break;

                                case "contact":
                                    if (this._contactOptions != null && this._contactOptions.OutboundPopupEvent != null && _contactOptions.OutboundPopupEvent.Equals(eventName))
                                    {
                                        _logger.Info("GetOutboundPopupData : Collecting Outbound Pop-up Data for Lead");
                                        sfdcData.ContactData = SFDCContact.GetInstance().GetContactVoicePopupData(message, callType);
                                    }
                                    break;

                                case "account":
                                    if (this._accountOptions != null && this._accountOptions.OutboundPopupEvent != null && this._accountOptions.OutboundPopupEvent.Equals(eventName))
                                    {
                                        _logger.Info("GetOutboundPopupData : Collecting Outbound Pop-up Data for Lead");
                                        sfdcData.AccountData = SFDCAccount.GetInstance().GetAccountVoicePopupData(message, callType);
                                    }
                                    break;

                                case "case":
                                    if (this._caseOptions != null && this._caseOptions.OutboundPopupEvent != null && this._caseOptions.OutboundPopupEvent.Equals(eventName))
                                    {
                                        _logger.Info("GetOutboundPopupData : Collecting Outbound Pop-up Data for Lead");
                                        sfdcData.CaseData = SFDCCase.GetInstance().GetCaseVoicePopupData(message, callType);
                                    }
                                    break;

                                case "opportunity":
                                    if (this._opportunityOptions != null && this._opportunityOptions.OutboundPopupEvent != null && this._opportunityOptions.OutboundPopupEvent.Equals(eventName))
                                    {
                                        _logger.Info("GetOutboundPopupData : Collecting Outbound Pop-up Data for Lead");
                                        sfdcData.OpportunityData = SFDCOpportunity.GetInstance().GetOpportunityVoicePopupData(message, callType);
                                    }
                                    break;

                                case "useractivity":
                                    if (this._userActivityOptions != null && this._userActivityOptions.OutboundPopupEvent != null &&
                        this._userActivityOptions.OutboundPopupEvent.Equals(eventName))
                                    {
                                        _logger.Info("GetOutboundPopupData : Collecting Outbound Pop-up Data for Lead");
                                        sfdcData.UserActivityData = SFDCUserActivity.GetInstance().GetVoiceCreateUserAcitivityData(message, callType);
                                    }
                                    break;

                                default:
                                    if (this._customObjectOptions.ContainsKey(key))
                                    {
                                        if (this._customObjectOptions[key] != null && this._customObjectOptions[key].OutboundPopupEvent != null &&
                                                this._customObjectOptions[key].OutboundPopupEvent.Equals(eventName))
                                        {
                                            CustomObjectData cstobject = SFDCCustomObject.GetInstance().GetCustomObjectVoicePopupData(message, callType, key);
                                            if (cstobject != null)
                                            {
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
            }
            catch (Exception generalException)
            {
                _logger.Error("GetOutboundPopupData : Error Occurred  : " + generalException.ToString());
            }
            return sfdcData;
        }

        #endregion Outbound Pop-up Data

        #region Consult Dialed PopupData

        public SFDCData GetConsultDialedPopupData(IMessage message, SFDCCallType callType, string eventName)
        {
            SFDCData sfdcData = new SFDCData();
            try
            {
                this._logger.Info("GetConsultDialedPopupData : Collecting pop-up data for consult dialed on Event : " + eventName);
                if (this._PopupPages != null)
                {
                    if (_generalOptions.CanUseCommonSearchForVoice && this._userActivityLog != null)
                        sfdcData.ProfileActivityLogData = _sFDCUtility.GetCreateActivityLogData(this._userActivityLog, message, callType);
                    foreach (string key in this._PopupPages)
                    {
                        switch (key)
                        {
                            case "lead":
                                if (this._leadOptions != null && this._leadOptions.ConsultLogEvent != null && this._leadOptions.ConsultLogEvent.Equals(eventName))
                                {
                                    sfdcData.LeadData = SFDCLead.GetInstance().GetLeadVoicePopupData(message, callType);
                                }
                                break;

                            case "contact":
                                if (this._contactOptions != null && this._contactOptions.ConsultLogEvent != null && _contactOptions.ConsultLogEvent.Equals(eventName))
                                {
                                    sfdcData.ContactData = SFDCContact.GetInstance().GetContactVoicePopupData(message, callType);
                                }
                                break;

                            case "account":
                                if (this._accountOptions != null && this._accountOptions.ConsultLogEvent != null && this._accountOptions.ConsultLogEvent.Equals(eventName))
                                {
                                    sfdcData.AccountData = SFDCAccount.GetInstance().GetAccountVoicePopupData(message, callType);
                                }
                                break;

                            case "case":
                                if (this._caseOptions != null && this._caseOptions.ConsultLogEvent != null && this._caseOptions.ConsultLogEvent.Equals(eventName))
                                {
                                    sfdcData.CaseData = SFDCCase.GetInstance().GetCaseVoicePopupData(message, callType);
                                }
                                break;

                            case "opportunity":
                                if (this._opportunityOptions != null && this._opportunityOptions.ConsultLogEvent != null && this._opportunityOptions.ConsultLogEvent.Equals(eventName))
                                {
                                    sfdcData.OpportunityData = SFDCOpportunity.GetInstance().GetOpportunityVoicePopupData(message, callType);
                                }
                                break;

                            case "useractivity":
                                if (this._userActivityOptions != null && this._userActivityOptions.ConsultLogEvent != null &&
                    this._userActivityOptions.ConsultLogEvent.Equals(eventName))
                                {
                                    sfdcData.UserActivityData = SFDCUserActivity.GetInstance().GetVoiceCreateUserAcitivityData(message, callType);
                                }
                                break;

                            default:
                                if (this._customObjectOptions.ContainsKey(key))
                                {
                                    if (this._customObjectOptions[key] != null && this._customObjectOptions[key].ConsultLogEvent != null &&
                                            this._customObjectOptions[key].ConsultLogEvent.Equals(eventName))
                                    {
                                        CustomObjectData cstobject = SFDCCustomObject.GetInstance().GetCustomObjectVoicePopupData(message, callType, key);
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
                _logger.Error("GetConsultDialedPopupData : Error Occurred  : " + generalException.ToString());
            }
            return sfdcData;
        }

        #endregion Consult Dialed PopupData

        #region Outbound Failure Pop-up

        public SFDCData GetOutboundFailurePopupData(IMessage message, SFDCCallType callType, string eventName)
        {
            SFDCData sfdcData = new SFDCData();
            try
            {
                this._logger.Info("GetOutboundFailurePopupData : Collecting pop-up data for outbound failure on Event : " + eventName);
                if (this._PopupPages != null)
                {
                    if (_generalOptions.CanUseCommonSearchForVoice)
                    {
                        if (this._generalOptions.OutboundFailurePopupEvent != null && this._generalOptions.OutboundFailurePopupEvent.Contains(eventName))
                        {
                            sfdcData.CommonSearchData = GetCommonVoiceSearchValue(message, callType);
                            sfdcData.CommonPopupObjects = Settings.CommonPopupObjects;
                            sfdcData.CommonSearchFormats = this._generalOptions.PhoneNumberSearchFormat;
                            sfdcData.CommonSearchCondition = this._generalOptions.CommonSearchConditionForVoice;
                            if (_generalOptions.CanUseCommonSearchForVoice && this._userActivityLog != null)
                                sfdcData.ProfileActivityLogData = _sFDCUtility.GetCreateActivityLogData(this._userActivityLog, message, callType);
                            foreach (string key in this._PopupPages)
                            {
                                switch (key)
                                {
                                    case "lead":
                                        if (_leadOptions != null)
                                        {
                                            _logger.Info("GetOutboundFailurePopupData : Collecting Inbound Voice Pop-up data for Lead");
                                            sfdcData.LeadData = SFDCLead.GetInstance().GetLeadVoicePopupData(message, callType);
                                        }
                                        else
                                        {
                                            _logger.Warn("GetOutboundFailurePopupData : Lead Options is empty");
                                        }
                                        break;

                                    case "contact":
                                        if (_contactOptions != null)
                                        {
                                            _logger.Info("GetOutboundFailurePopupData : Collecting Inbound Voice Pop-up data for contact");
                                            sfdcData.ContactData = SFDCContact.GetInstance().GetContactVoicePopupData(message, callType);
                                        }
                                        else
                                        {
                                            _logger.Warn("GetOutboundFailurePopupData : Contact Options is empty");
                                        }
                                        break;

                                    case "account":
                                        if (_accountOptions != null)
                                        {
                                            _logger.Info("GetOutboundFailurePopupData : Collecting Inbound Voice Pop-up data for account");
                                            sfdcData.AccountData = SFDCAccount.GetInstance().GetAccountVoicePopupData(message, callType);
                                        }
                                        else
                                        {
                                            _logger.Warn("GetOutboundFailurePopupData : Account Options is empty");
                                        }
                                        break;

                                    case "case":
                                        if (_caseOptions != null)
                                        {
                                            _logger.Info("GetOutboundFailurePopupData : Collecting Inbound Voice Pop-up data for case");
                                            sfdcData.CaseData = SFDCCase.GetInstance().GetCaseVoicePopupData(message, callType);
                                        }
                                        else
                                        {
                                            _logger.Warn("GetOutboundFailurePopupData : Case Options is empty");
                                        }
                                        break;

                                    case "opportunity":
                                        if (_opportunityOptions != null)
                                        {
                                            _logger.Info("GetOutboundFailurePopupData : Collecting Inbound Voice Pop-up data for opportunity");
                                            sfdcData.OpportunityData = SFDCOpportunity.GetInstance().GetOpportunityVoicePopupData(message, callType);
                                        }
                                        else
                                        {
                                            _logger.Warn("GetOutboundFailurePopupData : Opportunity Options is empty");
                                        }
                                        break;

                                    case "useractivity":
                                        if (_userActivityOptions != null)
                                        {
                                            _logger.Info("GetOutboundFailurePopupData : Collecting Inbound Voice Pop-up data for useractivity");
                                            sfdcData.UserActivityData = SFDCUserActivity.GetInstance().GetVoiceCreateUserAcitivityData(message, callType);
                                        }
                                        else
                                        {
                                            _logger.Warn("GetOutboundFailurePopupData : Useractivity Options is empty");
                                        }
                                        break;

                                    default:
                                        if (this._customObjectOptions != null && this._customObjectOptions.ContainsKey(key))
                                        {
                                            CustomObjectData cstobject = SFDCCustomObject.GetInstance().GetCustomObjectVoicePopupData(message, callType, key);
                                            if (cstobject != null)
                                            {
                                                this._tempCustomObject.Add(cstobject);
                                            }
                                        }
                                        else
                                        {
                                            _logger.Warn("GetOutboundFailurePopupData : CustomObjectOptions Options is empty");
                                        }
                                        break;
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (string key in this._PopupPages)
                        {
                            switch (key)
                            {
                                case "lead":
                                    if (this._leadOptions != null && this._leadOptions.OutboundFailurePopupEvent != null && this._leadOptions.OutboundFailurePopupEvent.Contains(eventName))
                                    {
                                        sfdcData.LeadData = SFDCLead.GetInstance().GetLeadVoicePopupData(message, callType);
                                    }
                                    break;

                                case "contact":
                                    if (this._contactOptions != null && this._contactOptions.OutboundFailurePopupEvent != null && _contactOptions.OutboundFailurePopupEvent.Contains(eventName))
                                    {
                                        sfdcData.ContactData = SFDCContact.GetInstance().GetContactVoicePopupData(message, callType);
                                    }
                                    break;

                                case "account":
                                    if (this._accountOptions != null && this._accountOptions.OutboundFailurePopupEvent != null && this._accountOptions.OutboundFailurePopupEvent.Contains(eventName))
                                    {
                                        sfdcData.AccountData = SFDCAccount.GetInstance().GetAccountVoicePopupData(message, callType);
                                    }
                                    break;

                                case "case":
                                    if (this._caseOptions != null && this._caseOptions.OutboundFailurePopupEvent != null && this._caseOptions.OutboundFailurePopupEvent.Contains(eventName))
                                    {
                                        sfdcData.CaseData = SFDCCase.GetInstance().GetCaseVoicePopupData(message, callType);
                                    }
                                    break;

                                case "opportunity":
                                    if (this._opportunityOptions != null && this._opportunityOptions.OutboundFailurePopupEvent != null && this._opportunityOptions.OutboundFailurePopupEvent.Contains(eventName))
                                    {
                                        sfdcData.OpportunityData = SFDCOpportunity.GetInstance().GetOpportunityVoicePopupData(message, callType);
                                    }
                                    break;

                                case "useractivity":
                                    if (this._userActivityOptions != null && this._userActivityOptions.OutboundFailurePopupEvent != null &&
                        this._userActivityOptions.OutboundFailurePopupEvent.Contains(eventName))
                                    {
                                        sfdcData.UserActivityData = SFDCUserActivity.GetInstance().GetVoiceCreateUserAcitivityData(message, callType);
                                    }
                                    break;

                                default:
                                    if (this._customObjectOptions.ContainsKey(key))
                                    {
                                        if (this._customObjectOptions[key] != null && this._customObjectOptions[key].OutboundFailurePopupEvent != null &&
                                                this._customObjectOptions[key].OutboundFailurePopupEvent.Contains(eventName))
                                        {
                                            CustomObjectData cstobject = SFDCCustomObject.GetInstance().GetCustomObjectVoicePopupData(message, callType, key);
                                            if (cstobject != null)
                                            {
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
            }
            catch (Exception generalException)
            {
                _logger.Error("GetOutboundFailurePopupData : Error Occurred  : " + generalException.ToString());
            }
            return sfdcData;
        }

        #endregion Outbound Failure Pop-up

        #region Inbound Update Data

        public SFDCData GetInboundUpdateData(IMessage message, SFDCCallType callType, string callDuration, string eventName, string notes = null)
        {
            SFDCData sfdcData = new SFDCData();
            try
            {
                if (_generalOptions.CanUseCommonSearchForVoice && this._userActivityLog != null)
                {
                    sfdcData.ProfileUpdateActivityLogData = _sFDCUtility.GetUpdateActivityLogData(this._userActivityLog, message, callType, callDuration, voiceComments: notes);
                    sfdcData.ProfileActivityLogAppendData = _sFDCUtility.GetUpdateActivityLogData(this._userActivityLog, message, callType, callDuration, voiceComments: notes, isAppendLogData: true);
                }

                this._logger.Info("GetInboundUpdateData : Collecting update data for the Inbound call on Event : " + eventName);
                if (this._PopupPages != null)
                {
                    foreach (string key in this._PopupPages)
                    {
                        switch (key)
                        {
                            case "lead":
                                if (this._leadOptions != null && this._leadOptions.InboundUpdateEvent != null && this._leadOptions.InboundUpdateEvent.Contains(eventName))
                                {
                                    sfdcData.LeadData = SFDCLead.GetInstance().GetLeadVoiceUpdateData(message, eventName, callType, callDuration, notes);
                                }
                                break;

                            case "contact":
                                if (this._contactOptions != null && this._contactOptions.InboundUpdateEvent != null && this._contactOptions.InboundUpdateEvent.Contains(eventName))
                                {
                                    sfdcData.ContactData = SFDCContact.GetInstance().GetContactVoiceUpdateData(message, eventName, callType, callDuration, notes);
                                }
                                break;

                            case "account":
                                if (this._accountOptions != null && this._accountOptions.InboundUpdateEvent != null && this._accountOptions.InboundUpdateEvent.Contains(eventName))
                                {
                                    sfdcData.AccountData = SFDCAccount.GetInstance().GetAccountUpdateData(message, eventName, callType, callDuration, notes);
                                }
                                break;

                            case "case":
                                if (this._caseOptions != null && this._caseOptions.InboundUpdateEvent != null && this._caseOptions.InboundUpdateEvent.Contains(eventName))
                                {
                                    sfdcData.CaseData = SFDCCase.GetInstance().GetCaseVoiceUpdateData(message, eventName, callType, callDuration, notes);
                                }
                                break;

                            case "opportunity":
                                if (this._opportunityOptions != null && this._opportunityOptions.InboundUpdateEvent != null && this._opportunityOptions.InboundUpdateEvent.Contains(eventName))
                                {
                                    sfdcData.OpportunityData = SFDCOpportunity.GetInstance().GetOpportunityVoiceUpdateData(message, eventName, callType, callDuration, notes);
                                }
                                break;

                            case "useractivity":
                                if (this._userActivityOptions != null && this._userActivityOptions.InboundUpdateEvent != null &&
                    this._userActivityOptions.InboundUpdateEvent.Contains(eventName))
                                {
                                    sfdcData.UserActivityData = SFDCUserActivity.GetInstance().GetVoiceUpdateUserAcitivityData(message, callType, callDuration, notes);
                                }
                                break;

                            default:
                                if (this._customObjectOptions.ContainsKey(key))
                                {
                                    if (this._customObjectOptions[key] != null && this._customObjectOptions[key].InboundUpdateEvent != null &&
                                            this._customObjectOptions[key].InboundUpdateEvent.Contains(eventName))
                                    {
                                        CustomObjectData cstobject = SFDCCustomObject.GetInstance().GetCusotmObjectVoiceUpdateData(message, callType, callDuration, key);
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
                _logger.Error("GetInboundUpdateData : Error Occurred  : " + generalException.ToString());
            }
            return sfdcData;
        }

        #endregion Inbound Update Data

        #region Outbound Update Data

        public SFDCData GetOutboundUpdateData(IMessage message, SFDCCallType callType, string callDuration, string eventName, string notes = null)
        {
            SFDCData sfdcData = new SFDCData();
            try
            {
                this._logger.Info("GetOutboundUpdateData : Collecting update data for the Outbound call on Event : " + eventName);
                if (this._PopupPages != null)
                {
                    if (_generalOptions.CanUseCommonSearchForVoice && this._userActivityLog != null)
                    {
                        sfdcData.ProfileUpdateActivityLogData = _sFDCUtility.GetUpdateActivityLogData(this._userActivityLog, message, callType, callDuration, voiceComments: notes);
                        sfdcData.ProfileActivityLogAppendData = _sFDCUtility.GetUpdateActivityLogData(this._userActivityLog, message, callType, callDuration, voiceComments: notes, isAppendLogData: true);
                    }
                    foreach (string key in this._PopupPages)
                    {
                        switch (key)
                        {
                            case "lead":
                                if (this._leadOptions != null && this._leadOptions.OutboundUpdateEvent != null && this._leadOptions.OutboundUpdateEvent.Contains(eventName))
                                {
                                    sfdcData.LeadData = SFDCLead.GetInstance().GetLeadVoiceUpdateData(message, eventName, callType, callDuration, notes);
                                }
                                break;

                            case "contact":
                                if (this._contactOptions != null && this._contactOptions.OutboundUpdateEvent != null && this._contactOptions.OutboundUpdateEvent.Contains(eventName))
                                {
                                    sfdcData.ContactData = SFDCContact.GetInstance().GetContactVoiceUpdateData(message, eventName, callType, callDuration, notes);
                                }
                                break;

                            case "account":
                                if (this._accountOptions != null && this._accountOptions.OutboundUpdateEvent != null && this._accountOptions.OutboundUpdateEvent.Contains(eventName))
                                {
                                    sfdcData.AccountData = SFDCAccount.GetInstance().GetAccountUpdateData(message, eventName, callType, callDuration, notes);
                                }
                                break;

                            case "case":
                                if (this._caseOptions != null && this._caseOptions.OutboundUpdateEvent != null && this._caseOptions.OutboundUpdateEvent.Contains(eventName))
                                {
                                    sfdcData.CaseData = SFDCCase.GetInstance().GetCaseVoiceUpdateData(message, eventName, callType, callDuration, notes);
                                }
                                break;

                            case "opportunity":
                                if (this._opportunityOptions != null && this._opportunityOptions.OutboundUpdateEvent != null && this._opportunityOptions.OutboundUpdateEvent.Contains(eventName))
                                {
                                    sfdcData.OpportunityData = SFDCOpportunity.GetInstance().GetOpportunityVoiceUpdateData(message, eventName, callType, callDuration, notes);
                                }
                                break;

                            case "useractivity":
                                if (this._userActivityOptions != null && this._userActivityOptions.OutboundUpdateEvent != null &&
                    this._userActivityOptions.OutboundUpdateEvent.Contains(eventName))
                                {
                                    sfdcData.UserActivityData = SFDCUserActivity.GetInstance().GetVoiceUpdateUserAcitivityData(message, callType, callDuration, notes);
                                }
                                break;

                            default:
                                if (this._customObjectOptions.ContainsKey(key))
                                {
                                    if (this._customObjectOptions[key] != null && this._customObjectOptions[key].OutboundUpdateEvent != null &&
                                            this._customObjectOptions[key].OutboundUpdateEvent.Contains(eventName))
                                    {
                                        CustomObjectData cstobject = SFDCCustomObject.GetInstance().GetCusotmObjectVoiceUpdateData(message, callType, callDuration, key);
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

        #endregion Outbound Update Data

        #region Consult Update Data

        public SFDCData GetConsultUpdateData(IMessage message, SFDCCallType callType, string callDuration, string eventName, string notes = null)
        {
            SFDCData sfdcData = new SFDCData();
            try
            {
                this._logger.Info("GetConsultUpdateData : Collecting update data for the consult call on Event : " + eventName);
                if (this._PopupPages != null)
                {
                    if (_generalOptions.CanUseCommonSearchForVoice && this._userActivityLog != null)
                    {
                        sfdcData.ProfileUpdateActivityLogData = _sFDCUtility.GetUpdateActivityLogData(this._userActivityLog, message, callType, callDuration, voiceComments: notes);
                        sfdcData.ProfileActivityLogAppendData = _sFDCUtility.GetUpdateActivityLogData(this._userActivityLog, message, callType, callDuration, voiceComments: notes, isAppendLogData: true);
                    }
                    foreach (string key in this._PopupPages)
                    {
                        switch (key)
                        {
                            case "lead":
                                if (this._leadOptions != null && this._leadOptions.ConsultUpdateEvent != null && this._leadOptions.ConsultUpdateEvent.Contains(eventName))
                                {
                                    sfdcData.LeadData = SFDCLead.GetInstance().GetLeadVoiceUpdateData(message, eventName, callType, callDuration, notes);
                                }
                                break;

                            case "contact":
                                if (this._contactOptions != null && this._contactOptions.ConsultUpdateEvent != null && this._contactOptions.ConsultUpdateEvent.Contains(eventName))
                                {
                                    sfdcData.ContactData = SFDCContact.GetInstance().GetContactVoiceUpdateData(message, eventName, callType, callDuration, notes);
                                }
                                break;

                            case "account":
                                if (this._accountOptions != null && this._accountOptions.ConsultUpdateEvent != null && this._accountOptions.ConsultUpdateEvent.Contains(eventName))
                                {
                                    sfdcData.AccountData = SFDCAccount.GetInstance().GetAccountUpdateData(message, eventName, callType, callDuration, notes);
                                }
                                break;

                            case "case":
                                if (this._caseOptions != null && this._caseOptions.ConsultUpdateEvent != null && this._caseOptions.ConsultUpdateEvent.Contains(eventName))
                                {
                                    sfdcData.CaseData = SFDCCase.GetInstance().GetCaseVoiceUpdateData(message, eventName, callType, callDuration, notes);
                                }
                                break;

                            case "opportunity":
                                if (this._opportunityOptions != null && this._opportunityOptions.ConsultUpdateEvent != null && this._opportunityOptions.ConsultUpdateEvent.Contains(eventName))
                                {
                                    sfdcData.OpportunityData = SFDCOpportunity.GetInstance().GetOpportunityVoiceUpdateData(message, eventName, callType, callDuration, notes);
                                }
                                break;

                            case "useractivity":
                                if (this._userActivityOptions != null && this._userActivityOptions.ConsultUpdateEvent != null &&
                    this._userActivityOptions.ConsultUpdateEvent.Contains(eventName))
                                {
                                    sfdcData.UserActivityData = SFDCUserActivity.GetInstance().GetVoiceUpdateUserAcitivityData(message, callType, callDuration, notes);
                                }
                                break;

                            default:
                                if (this._customObjectOptions.ContainsKey(key))
                                {
                                    if (this._customObjectOptions[key] != null && this._customObjectOptions[key].ConsultUpdateEvent != null &&
                                            this._customObjectOptions[key].ConsultUpdateEvent.Contains(eventName))
                                    {
                                        CustomObjectData cstobject = SFDCCustomObject.GetInstance().GetCusotmObjectVoiceUpdateData(message, callType, callDuration, key);
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
                _logger.Error("GetConsultUpdateData : Error Occurred  : " + generalException.ToString());
            }
            return sfdcData;
        }

        #endregion Consult Update Data

        #region ClickToDialLogs

        public void GetClickToDialLogs(IMessage message, string ConnId, SFDCCallType callType, string clickTodialData, string eventName)
        {
            try
            {
                this._logger.Info("GetClickToDialLogs : ClickToDialPopup");
                VoiceOptions voiceOption = null;
                string[] data = clickTodialData.Split(',');
                if (data != null && data.Length == 3)
                {
                    switch (data[1].ToLower())
                    {
                        case "lead":
                            voiceOption = _leadOptions;
                            break;

                        case "contact":
                            voiceOption = _contactOptions;
                            break;

                        case "account":
                            voiceOption = _accountOptions;
                            break;

                        case "case":
                            voiceOption = _caseOptions;
                            break;

                        case "opportunity":
                            voiceOption = _opportunityOptions;
                            break;

                        default:
                            if (data[1].Contains("__c"))
                            {
                                string objectName = Settings.CustomObjectNames[data[1]];
                                if (objectName != null)
                                {
                                    voiceOption = _customObjectOptions[objectName];
                                }
                            }
                            break;
                    }

                    #region Pop-up

                    if (voiceOption != null)
                    {
                        if (this._generalOptions.CanUseCommonSearchForVoice)
                        {
                            if ((callType == SFDCCallType.OutboundVoiceSuccess && _generalOptions.OutboundPopupEvent != null &&
                            _generalOptions.OutboundPopupEvent.Equals(eventName)) || (callType == SFDCCallType.OutboundVoiceFailure && _generalOptions.OutboundFailurePopupEvent != null &&
                            _generalOptions.OutboundFailurePopupEvent.Contains(eventName)))
                            {
                                ClickToDialPopup(message, ConnId, callType, data[1], data[2]);
                            }
                        }
                        else if ((callType == SFDCCallType.OutboundVoiceSuccess && voiceOption.OutboundPopupEvent != null &&
                            voiceOption.OutboundPopupEvent.Equals(eventName)) || (callType == SFDCCallType.OutboundVoiceFailure && voiceOption.OutboundFailurePopupEvent != null &&
                            voiceOption.OutboundFailurePopupEvent.Contains(eventName)))
                        {
                            ClickToDialPopup(message, ConnId, callType, data[1], data[2]);
                        }
                    }
                    else
                    {
                        this._logger.Info("GetClickToDialLogs : Object Name not found : " + data[1]);
                    }

                    #endregion Pop-up
                }
            }
            catch (Exception generalException)
            {
                _logger.Error("GetClickToDialLogs : Error Occurred  : " + generalException.ToString());
            }
        }

        #endregion ClickToDialLogs

        #region ClickToDial Pop-up

        private void ClickToDialPopup(IMessage message, string connId, SFDCCallType callType, string objectType, string objectId)
        {
            try
            {
                this._logger.Info("GetClickToDialLogs : ClickToDial Pop-up.... ");
                this._logger.Info("GetClickToDialLogs : Call Type  : " + Convert.ToString(callType));
                this._logger.Info("GetClickToDialLogs : SFDC Obejct Type  : " + objectType);
                this._logger.Info("GetClickToDialLogs : SFDC Obejct Id  : " + objectId);
                PopupData popdata = new PopupData();
                List<XmlElement> element = new List<XmlElement>();
                switch (objectType)
                {
                    case "lead":
                        popdata.InteractionId = connId;
                        popdata.ObjectName = objectType;
                        element = _sFDCUtility.GetCreateActivityLogData(this._activityLogs["lead"], message, callType);
                        this._sFDCUtility.CreateActivityLog(connId, element, "Lead", callType, objectId);
                        Settings.SFDCPopupData.Add(connId, popdata);
                        break;

                    case "contact":
                        popdata.InteractionId = connId;
                        popdata.ObjectName = objectType;
                        element = _sFDCUtility.GetCreateActivityLogData(this._activityLogs["contact"], message, callType);
                        this._sFDCUtility.CreateActivityLog(connId, element, "Contact", callType, objectId);
                        Settings.SFDCPopupData.Add(connId, popdata);
                        break;

                    case "account":
                        popdata.InteractionId = connId;
                        popdata.ObjectName = objectType;
                        element = _sFDCUtility.GetCreateActivityLogData(this._activityLogs["account"], message, callType);
                        this._sFDCUtility.CreateActivityLog(connId, element, "Account", callType, objectId);
                        Settings.SFDCPopupData.Add(connId, popdata);
                        break;

                    case "case":
                        popdata.InteractionId = connId;
                        popdata.ObjectName = objectType;
                        element = _sFDCUtility.GetCreateActivityLogData(this._activityLogs["case"], message, callType);
                        this._sFDCUtility.CreateActivityLog(connId, element, "Case", callType, objectId);
                        Settings.SFDCPopupData.Add(connId, popdata);
                        break;

                    case "opportunity":
                        popdata.InteractionId = connId;
                        popdata.ObjectName = objectType;
                        element = _sFDCUtility.GetCreateActivityLogData(this._activityLogs["opportunity"], message, callType);
                        this._sFDCUtility.CreateActivityLog(connId, element, "Opportunity", callType, objectId);
                        Settings.SFDCPopupData.Add(connId, popdata);
                        break;

                    default:

                        if (objectType.Contains("__c") && Settings.CustomObjectNames.ContainsKey(objectType))
                        {
                            string objectName = Settings.CustomObjectNames[objectType];
                            if (this._activityLogs.ContainsKey(objectName))
                            {
                                popdata.InteractionId = connId;
                                popdata.ObjectName = objectType;
                                element = _sFDCUtility.GetCreateActivityLogData(this._activityLogs[objectName], message, callType);
                                this._sFDCUtility.CreateActivityLog(connId, element, objectType, callType, objectId);
                                Settings.SFDCPopupData.Add(connId, popdata);
                            }
                            else
                            {
                                this._logger.Info("GetClickToDialLogs : " + objectType + " Configuration not found ");
                            }
                        }
                        else
                        {
                            this._logger.Info("GetClickToDialLogs : Custom Object not found with Name : " + objectType);
                        }
                        break;
                }
            }
            catch (Exception generalException)
            {
                _logger.Error("GetClickToDialLogs : Error Occurred  : " + generalException.ToString());
            }
        }

        #endregion ClickToDial Pop-up

        #region GetCommonVoiceSearchValue

        internal string GetCommonVoiceSearchValue(IMessage message, SFDCCallType callType)
        {
            try
            {
                this._logger.Info("GetCommonVoiceSearchValue :  Reading Common search data.....");
                this._logger.Info("GetCommonVoiceSearchValue :  Event Name : " + message.Name);
                this._logger.Info("GetCommonVoiceSearchValue :  CallType : " + callType.ToString());
                string[] userDataSearchKeys = null;
                string[] attributeSearchKeys = null;
                string searchValue = string.Empty;
                dynamic popupEvent = Convert.ChangeType(message, message.GetType());

                if (callType == SFDCCallType.InboundVoice)
                {
                    userDataSearchKeys = (this._generalOptions.InboundSearchUserDataKeys != null) ? this._generalOptions.InboundSearchUserDataKeys.Split(',') : null;
                    attributeSearchKeys = (this._generalOptions.InboundSearchAttributeKeys != null) ? this._generalOptions.InboundSearchAttributeKeys.Split(',') : null;
                }
                else if (callType == SFDCCallType.OutboundVoiceSuccess || callType == SFDCCallType.OutboundVoiceFailure)
                {
                    userDataSearchKeys = (this._generalOptions.OutboundSearchUserDataKeys != null) ? this._generalOptions.OutboundSearchUserDataKeys.Split(',') : null;
                    attributeSearchKeys = (this._generalOptions.OutboundSearchAttributeKeys != null) ? this._generalOptions.OutboundSearchAttributeKeys.Split(',') : null;
                }
                else if (callType == SFDCCallType.ConsultVoiceReceived)
                {
                    userDataSearchKeys = (_generalOptions.ConsultSearchUserDataKeys != null) ? _generalOptions.ConsultSearchUserDataKeys.Split(',') : null;
                    attributeSearchKeys = (_generalOptions.ConsultSearchAttributeKeys != null) ? _generalOptions.ConsultSearchAttributeKeys.Split(',') : null;
                }

                if (this._generalOptions.VoiceSearchPriority == "user-data" && popupEvent != null)
                {
                    if (userDataSearchKeys != null && popupEvent.UserData != null)
                    {
                        searchValue = this._sfdcUtilityHelper.GetUserDataSearchValues(popupEvent.UserData, userDataSearchKeys);
                        if (!this._sfdcUtilityHelper.ValidateSearchData(searchValue))
                        {
                            this._logger.Info("search data from user-data keys not found, Reading attribute search keys.....");
                            searchValue = this._sfdcUtilityHelper.GetVoiceAttributeSearchValues(message, attributeSearchKeys);
                        }
                    }
                    else if (attributeSearchKeys != null)
                    {
                        this._logger.Info("Either user-data search keys or user-data is null, Reading attribute search keys.....");
                        searchValue = this._sfdcUtilityHelper.GetVoiceAttributeSearchValues(message, attributeSearchKeys);
                    }
                }
                else if (this._generalOptions.VoiceSearchPriority == "attribute")
                {
                    searchValue = this._sfdcUtilityHelper.GetVoiceAttributeSearchValues(message, attributeSearchKeys);
                    if (!this._sfdcUtilityHelper.ValidateSearchData(searchValue))
                    {
                        this._logger.Info("search data from attribute keys not found, Reading user-data search keys.....");
                        if (userDataSearchKeys != null && popupEvent.UserData != null)
                        {
                            searchValue = this._sfdcUtilityHelper.GetUserDataSearchValues(popupEvent.UserData, userDataSearchKeys);
                        }
                        else
                        {
                            this._logger.Info("Either user-data or search keys are not found.....");
                        }
                    }
                }
                else if (this._generalOptions.VoiceSearchPriority == "both")
                {
                    if (popupEvent != null && popupEvent.UserData != null)
                        searchValue = this._sfdcUtilityHelper.GetUserDataSearchValues(popupEvent.UserData, userDataSearchKeys);
                    if (!this._sfdcUtilityHelper.ValidateSearchData(searchValue))
                    {
                        string temp = this._sfdcUtilityHelper.GetVoiceAttributeSearchValues(message, attributeSearchKeys);
                        if (temp != string.Empty)
                        {
                            searchValue += "," + temp;
                        }
                    }
                    else
                    {
                        searchValue = this._sfdcUtilityHelper.GetVoiceAttributeSearchValues(message, attributeSearchKeys);
                    }
                }
                return searchValue;
            }
            catch (Exception generalException)
            {
                this._logger.Error("GetCommonVoiceSearchValue : Error occurred while reading Lead Data : " + generalException.ToString());
            }
            return null;
        }

        #endregion GetCommonVoiceSearchValue
    }
}