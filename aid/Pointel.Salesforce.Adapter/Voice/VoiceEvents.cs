#region Header

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

#endregion Header

namespace Pointel.Salesforce.Adapter.Voice
{
    using Genesyslab.Platform.Commons.Collections;
    using Genesyslab.Platform.Commons.Protocols;
    using Genesyslab.Platform.Voice.Protocols.TServer.Events;
    using Pointel.Salesforce.Adapter.Configurations;
    using Pointel.Salesforce.Adapter.LogMessage;
    using Pointel.Salesforce.Adapter.PForce;
    using Pointel.Salesforce.Adapter.SFDCModels;
    using Pointel.Salesforce.Adapter.SFDCUtils;
    using Pointel.Salesforce.Adapter.Utility;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Xml;

    internal class VoiceEvents
    {
        #region Fields

        private static VoiceEvents currentObject = null;

        private VoiceOptions accountOptions = null;
        private IDictionary<string, KeyValueCollection> activityLogs = null;
        private VoiceOptions caseOptions = null;
        private VoiceOptions contactOptions = null;
        private IDictionary<string, VoiceOptions> customObjectOptions = null;
        private GeneralOptions generalOptions = null;
        private VoiceOptions leadOptions = null;
        private Log logger = null;
        private VoiceOptions opportunityOptions = null;
        private string[] PopupPages = null;
        private SFDCUtility sfdcObject = null;
        private List<ICustomObject> tempCustomObject = new List<ICustomObject>();
        private KeyValueCollection userActivityLog = null;
        private UserActivityOptions userActivityOptions = null;

        #endregion Fields

        #region Constructors

        private VoiceEvents()
        {
            this.logger = Log.GenInstance();
            this.leadOptions = Settings.LeadVoiceOptions;
            this.contactOptions = Settings.ContactVoiceOptions;
            this.accountOptions = Settings.AccountVoiceOptions;
            this.caseOptions = Settings.CaseVoiceOptions;
            this.opportunityOptions = Settings.OpportunityVoiceOptions;
            this.userActivityLog = (Settings.VoiceActivityLogCollection.ContainsKey("useractivity")) ? Settings.VoiceActivityLogCollection["useractivity"] : null;
            this.sfdcObject = SFDCUtility.GetInstance();
            this.activityLogs = Settings.VoiceActivityLogCollection;
            this.customObjectOptions = Settings.CustomObjectVoiceOptions;
            this.userActivityOptions = Settings.UserActivityVoiceOptions;
            this.PopupPages = Settings.SFDCOptions.SFDCPopupPages;
            generalOptions = Settings.SFDCOptions;
        }

        #endregion Constructors

        #region Methods

        public static VoiceEvents GetInstance()
        {
            if (currentObject == null)
            {
                currentObject = new VoiceEvents();
            }
            return currentObject;
        }

        public void GetClickToDialLogs(IMessage message, string ConnId, SFDCCallType callType, string clickTodialData, string eventName)
        {
            try
            {
                this.logger.Info("GetClickToDialLogs : ClickToDialPopup");
                VoiceOptions voiceOption = null;
                string[] data = clickTodialData.Split(',');
                if (data != null && data.Length == 3)
                {
                    switch (data[1].ToLower())
                    {
                        case "lead":
                            voiceOption = leadOptions;
                            break;

                        case "contact":
                            voiceOption = contactOptions;
                            break;

                        case "account":
                            voiceOption = accountOptions;
                            break;

                        case "case":
                            voiceOption = caseOptions;
                            break;

                        case "opportunity":
                            voiceOption = opportunityOptions;
                            break;

                        default:
                            if (data[1].Contains("__c"))
                            {
                                string objectName = Settings.CustomObjectNames[data[1]];
                                if (objectName != null)
                                {
                                    voiceOption = customObjectOptions[objectName];
                                }
                            }
                            break;
                    }

                    #region Popup

                    if (voiceOption != null)
                    {
                        if (this.generalOptions.CanUseCommonSearchData)
                        {
                            if ((callType == SFDCCallType.OutboundSuccess && generalOptions.OutboundPopupEvent != null &&
                            generalOptions.OutboundPopupEvent.Equals(eventName)) || (callType == SFDCCallType.OutboundFailure && generalOptions.OutboundFailurePopupEvent != null &&
                            generalOptions.OutboundFailurePopupEvent.Equals(eventName)) || (callType == SFDCCallType.ConsultSuccess && voiceOption.ConsultSuccessLogEvent != null &&
                            voiceOption.ConsultSuccessLogEvent.Equals(eventName)) || (callType == SFDCCallType.ConsultFailure && voiceOption.ConsultFailurePopupEvent != null &&
                            voiceOption.ConsultFailurePopupEvent.Equals(eventName)))
                            {
                                ClickToDialPopup(message, ConnId, callType, data[1], data[2]);
                            }
                        }
                        else if ((callType == SFDCCallType.OutboundSuccess && voiceOption.OutboundPopupEvent != null &&
                            voiceOption.OutboundPopupEvent.Equals(eventName)) || (callType == SFDCCallType.OutboundFailure && voiceOption.OutboundFailurePopupEvent != null &&
                            voiceOption.OutboundFailurePopupEvent.Equals(eventName)) || (callType == SFDCCallType.ConsultSuccess && voiceOption.ConsultSuccessLogEvent != null &&
                            voiceOption.ConsultSuccessLogEvent.Equals(eventName)) || (callType == SFDCCallType.ConsultFailure && voiceOption.ConsultFailurePopupEvent != null &&
                            voiceOption.ConsultFailurePopupEvent.Equals(eventName)))
                        {
                            ClickToDialPopup(message, ConnId, callType, data[1], data[2]);
                        }
                    }
                    else
                    {
                        this.logger.Info("GetClickToDialLogs : Object Name not found : " + data[1]);
                    }

                    #endregion Popup
                }
            }
            catch (Exception generalException)
            {
                logger.Error("GetClickToDialLogs : Error Occurred  : " + generalException.ToString());
            }
        }

        public SFDCData GetConsultDialedPopupData(IMessage message, SFDCCallType callType, string eventName)
        {
            SFDCData sfdcData = new SFDCData();
            try
            {
                this.logger.Info("GetConsultDialedPopupData : Collecting popup data for consult dialed on Event : " + eventName);
                if (this.PopupPages != null)
                {
                    if (generalOptions.CanUseCommonSearchData && Settings.ProfileLevelActivity != null)
                        sfdcData.ProfileActivityLogData = SFDCUtility.GetInstance().GetVoiceActivityLog(Settings.ProfileLevelActivity, message, callType);
                    foreach (string key in this.PopupPages)
                    {
                        switch (key)
                        {
                            case "lead":
                                if (this.leadOptions != null && this.leadOptions.ConsultSuccessLogEvent != null && this.leadOptions.ConsultSuccessLogEvent.Equals(eventName))
                                {
                                    sfdcData.LeadData = SFDCLead.GetInstance().GetLeadVoicePopupData(message, callType);
                                }
                                break;

                            case "contact":
                                if (this.contactOptions != null && this.contactOptions.ConsultSuccessLogEvent != null && contactOptions.ConsultSuccessLogEvent.Equals(eventName))
                                {
                                    sfdcData.ContactData = SFDCContact.GetInstance().GetContactVoicePopupData(message, callType);
                                }
                                break;

                            case "account":
                                if (this.accountOptions != null && this.accountOptions.ConsultSuccessLogEvent != null && this.accountOptions.ConsultSuccessLogEvent.Equals(eventName))
                                {
                                    sfdcData.AccountData = SFDCAccount.GetInstance().GetAccountVoicePopupData(message, callType);
                                }
                                break;

                            case "case":
                                if (this.caseOptions != null && this.caseOptions.ConsultSuccessLogEvent != null && this.caseOptions.ConsultSuccessLogEvent.Equals(eventName))
                                {
                                    sfdcData.CaseData = SFDCCase.GetInstance().GetCaseVoicePopupData(message, callType);
                                }
                                break;

                            case "opportunity":
                                if (this.opportunityOptions != null && this.opportunityOptions.ConsultSuccessLogEvent != null && this.opportunityOptions.ConsultSuccessLogEvent.Equals(eventName))
                                {
                                    sfdcData.OpportunityData = SFDCOpportunity.GetInstance().GetOpportunityVoicePopupData(message, callType);
                                }
                                break;

                            case "useractivity":
                                if (this.userActivityOptions != null && this.userActivityOptions.ConsultSuccessLogEvent != null &&
                    this.userActivityOptions.ConsultSuccessLogEvent.Equals(eventName))
                                {
                                    sfdcData.UserActivityData = SFDCUserActivity.GetInstance().GetVoiceCreateUserAcitivityData(message, callType);
                                }
                                break;

                            default:
                                if (this.customObjectOptions.ContainsKey(key))
                                {
                                    if (this.customObjectOptions[key] != null && this.customObjectOptions[key].ConsultSuccessLogEvent != null &&
                                            this.customObjectOptions[key].ConsultSuccessLogEvent.Equals(eventName))
                                    {
                                        ICustomObject cstobject = SFDCCustomObject.GetInstance().GetCustomObjectVoicePopupData(message, callType, key);
                                        if (cstobject != null)
                                        {
                                            this.tempCustomObject.Add(cstobject);
                                        }
                                    }
                                }
                                break;
                        }
                    }
                    if (this.tempCustomObject.Count > 0)
                    {
                        ICustomObject[] temp = this.tempCustomObject.ToArray();
                        sfdcData.CustomObjectData = temp;
                        this.tempCustomObject.Clear();
                    }
                }
            }
            catch (Exception generalException)
            {
                logger.Error("GetConsultDialedPopupData : Error Occurred  : " + generalException.ToString());
            }
            return sfdcData;
        }

        public SFDCData GetConsultFailurePopupData(IMessage message, SFDCCallType callType, string eventName)
        {
            SFDCData sfdcData = new SFDCData();
            try
            {
                this.logger.Info("GetConsultFailurePopupData : Collecting popup data for consult failure on Event : " + eventName);
                if (this.PopupPages != null)
                {
                    if (generalOptions.CanUseCommonSearchData && Settings.ProfileLevelActivity != null)
                        sfdcData.ProfileActivityLogData = SFDCUtility.GetInstance().GetVoiceActivityLog(Settings.ProfileLevelActivity, message, callType);
                    foreach (string key in this.PopupPages)
                    {
                        switch (key)
                        {
                            case "lead":
                                if (this.leadOptions != null && this.leadOptions.ConsultFailurePopupEvent != null && this.leadOptions.ConsultFailurePopupEvent.Equals(eventName))
                                {
                                    sfdcData.LeadData = SFDCLead.GetInstance().GetLeadVoicePopupData(message, callType);
                                }
                                break;

                            case "contact":
                                if (this.contactOptions != null && this.contactOptions.ConsultFailurePopupEvent != null && this.contactOptions.ConsultFailurePopupEvent.Equals(eventName))
                                {
                                    sfdcData.ContactData = SFDCContact.GetInstance().GetContactVoicePopupData(message, callType);
                                }
                                break;

                            case "account":
                                if (this.accountOptions != null && this.accountOptions.ConsultFailurePopupEvent != null && this.accountOptions.ConsultFailurePopupEvent.Equals(eventName))
                                {
                                    sfdcData.AccountData = SFDCAccount.GetInstance().GetAccountVoicePopupData(message, callType);
                                }
                                break;

                            case "case":
                                if (this.caseOptions != null && this.caseOptions.ConsultFailurePopupEvent != null && this.caseOptions.ConsultFailurePopupEvent.Equals(eventName))
                                {
                                    sfdcData.CaseData = SFDCCase.GetInstance().GetCaseVoicePopupData(message, callType);
                                }
                                break;

                            case "opportunity":
                                if (this.opportunityOptions != null && this.opportunityOptions.ConsultFailurePopupEvent != null && this.opportunityOptions.ConsultFailurePopupEvent.Equals(eventName))
                                {
                                    sfdcData.OpportunityData = SFDCOpportunity.GetInstance().GetOpportunityVoicePopupData(message, callType);
                                }
                                break;

                            case "useractivity":
                                if (this.userActivityOptions != null && this.userActivityOptions.ConsultFailurePopupEvent != null &&
                    this.userActivityOptions.ConsultFailurePopupEvent.Equals(eventName))
                                {
                                    sfdcData.UserActivityData = SFDCUserActivity.GetInstance().GetVoiceCreateUserAcitivityData(message, callType);
                                }
                                break;

                            default:
                                if (this.customObjectOptions.ContainsKey(key))
                                {
                                    if (this.customObjectOptions[key] != null && this.customObjectOptions[key].ConsultFailurePopupEvent != null &&
                                            this.customObjectOptions[key].ConsultFailurePopupEvent.Equals(eventName))
                                    {
                                        ICustomObject cstobject = SFDCCustomObject.GetInstance().GetCustomObjectVoicePopupData(message, callType, key);
                                        if (cstobject != null)
                                        {
                                            this.tempCustomObject.Add(cstobject);
                                        }
                                    }
                                }
                                break;
                        }
                    }
                    if (this.tempCustomObject.Count > 0)
                    {
                        ICustomObject[] temp = this.tempCustomObject.ToArray();
                        sfdcData.CustomObjectData = temp;
                        this.tempCustomObject.Clear();
                    }
                }
            }
            catch (Exception generalException)
            {
                logger.Error("GetConsultFailurePopupData : Error Occurred  : " + generalException.ToString());
            }
            return sfdcData;
        }

        public SFDCData GetConsultReceivedPopupData(IMessage message, SFDCCallType callType, string eventName)
        {
            SFDCData sfdcData = new SFDCData();
            try
            {
                this.logger.Info("GetConsultReceivedPopupData : Collecting popup data for Cosult Recieved on Event : " + eventName);
                if (this.PopupPages != null)
                {
                    if (generalOptions.CanUseCommonSearchData)
                    {
                        if (this.generalOptions.ConsultPopupEvent != null && this.generalOptions.ConsultPopupEvent.Equals(eventName))
                        {
                            sfdcData.CommonSearchData = GetCommonVoiceSearchValue(message, callType);
                            sfdcData.CommonPopupObjects = Settings.CommonPopupObjects;
                            sfdcData.CommonSearchFormats = this.generalOptions.PhoneNumberSearchFormat;
                            sfdcData.CommonSearchCondition = this.generalOptions.CommonSearchCondition;
                            sfdcData.CommonSearchFields = this.generalOptions.SearchField;
                            if (Settings.ProfileLevelActivity != null)
                                sfdcData.ProfileActivityLogData = SFDCUtility.GetInstance().GetVoiceActivityLog(Settings.ProfileLevelActivity, message, callType);

                            foreach (string key in this.PopupPages)
                            {
                                switch (key)
                                {
                                    case "lead":
                                        if (leadOptions != null)
                                        {
                                            logger.Info("GetConsultReceivedPopupData : Collecting Cosult Recieved Voice Popup data for Lead");
                                            sfdcData.LeadData = SFDCLead.GetInstance().GetLeadVoicePopupData(message, callType);
                                        }
                                        else
                                        {
                                            logger.Warn("GetConsultReceivedPopupData : Lead Options is empty");
                                        }
                                        break;

                                    case "contact":
                                        if (contactOptions != null)
                                        {
                                            logger.Info("GetConsultReceivedPopupData : Collecting Cosult Recieved Voice Popup data for contact");
                                            sfdcData.ContactData = SFDCContact.GetInstance().GetContactVoicePopupData(message, callType);
                                        }
                                        else
                                        {
                                            logger.Warn("GetConsultReceivedPopupData : Contact Options is empty");
                                        }
                                        break;

                                    case "account":
                                        if (accountOptions != null)
                                        {
                                            logger.Info("GetConsultReceivedPopupData : Collecting Cosult Recieved Voice Popup data for account");
                                            sfdcData.AccountData = SFDCAccount.GetInstance().GetAccountVoicePopupData(message, callType);
                                        }
                                        else
                                        {
                                            logger.Warn("GetConsultReceivedPopupData : Account Options is empty");
                                        }
                                        break;

                                    case "case":
                                        if (caseOptions != null)
                                        {
                                            logger.Info("GetConsultReceivedPopupData : Collecting Cosult Recieved Voice Popup data for case");
                                            sfdcData.CaseData = SFDCCase.GetInstance().GetCaseVoicePopupData(message, callType);
                                        }
                                        else
                                        {
                                            logger.Warn("GetConsultReceivedPopupData : case Options is empty");
                                        }
                                        break;

                                    case "opportunity":
                                        if (opportunityOptions != null)
                                        {
                                            logger.Info("GetConsultReceivedPopupData : Collecting Cosult Recieved Voice Popup data for opportunity");
                                            sfdcData.OpportunityData = SFDCOpportunity.GetInstance().GetOpportunityVoicePopupData(message, callType);
                                        }
                                        else
                                        {
                                            logger.Warn("GetConsultReceivedPopupData : Opportunity Options is empty");
                                        }
                                        break;

                                    case "useractivity":
                                        if (userActivityOptions != null)
                                        {
                                            logger.Info("GetConsultReceivedPopupData : Collecting Cosult Recieved Voice Popup data for useractivity");
                                            sfdcData.UserActivityData = SFDCUserActivity.GetInstance().GetVoiceCreateUserAcitivityData(message, callType);
                                        }
                                        else
                                        {
                                            logger.Warn("GetConsultReceivedPopupData : Useractivity Options is empty");
                                        }
                                        break;

                                    default:
                                        if (customObjectOptions != null)
                                        {
                                            if (this.customObjectOptions.ContainsKey(key))
                                            {
                                                ICustomObject cstobject = SFDCCustomObject.GetInstance().GetCustomObjectVoicePopupData(message, callType, key);
                                                if (cstobject != null)
                                                {
                                                    this.tempCustomObject.Add(cstobject);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            logger.Warn("GetConsultReceivedPopupData : CustomObjectOptions Options is empty");
                                        }
                                        break;
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (string key in this.PopupPages)
                        {
                            switch (key)
                            {
                                case "lead":
                                    if (this.leadOptions != null && this.leadOptions.ConsultPopupEvent != null &&
                        this.leadOptions.ConsultPopupEvent.Equals(eventName))
                                    {
                                        logger.Info("GetConsultReceivedPopupData : Collecting Consult received popup data for Lead");
                                        sfdcData.LeadData = SFDCLead.GetInstance().GetLeadVoicePopupData(message, callType);
                                    }
                                    break;

                                case "contact":
                                    if (this.contactOptions != null && this.contactOptions.ConsultPopupEvent != null &&
                        this.contactOptions.ConsultPopupEvent.Equals(eventName))
                                    {
                                        logger.Info("GetConsultReceivedPopupData : Collecting Consult received popup data for Contact");
                                        sfdcData.ContactData = SFDCContact.GetInstance().GetContactVoicePopupData(message, callType);
                                    }
                                    break;

                                case "account":
                                    if (this.accountOptions != null && this.accountOptions.ConsultPopupEvent != null &&
                       this.accountOptions.ConsultPopupEvent.Equals(eventName))
                                    {
                                        logger.Info("GetConsultReceivedPopupData : Collecting Consult received popup data for Account");
                                        sfdcData.AccountData = SFDCAccount.GetInstance().GetAccountVoicePopupData(message, callType);
                                    }
                                    break;

                                case "case":
                                    if (this.caseOptions != null && this.caseOptions.ConsultPopupEvent != null &&
                        this.caseOptions.ConsultPopupEvent.Equals(eventName))
                                    {
                                        logger.Info("GetConsultReceivedPopupData : Collecting Consult received popup data for Case");
                                        sfdcData.CaseData = SFDCCase.GetInstance().GetCaseVoicePopupData(message, callType);
                                    }
                                    break;

                                case "opportunity":
                                    if (this.opportunityOptions != null && this.opportunityOptions.ConsultPopupEvent != null &&
                       this.opportunityOptions.ConsultPopupEvent.Equals(eventName))
                                    {
                                        logger.Info("GetConsultReceivedPopupData : Collecting Consult received popup data for Opportunity");
                                        sfdcData.OpportunityData = SFDCOpportunity.GetInstance().GetOpportunityVoicePopupData(message, callType);
                                    }
                                    break;

                                case "useractivity":
                                    if (this.userActivityOptions != null && this.userActivityOptions.ConsultPopupEvent != null &&
                       this.userActivityOptions.ConsultPopupEvent.Equals(eventName))
                                    {
                                        logger.Info("GetConsultReceivedPopupData : Collecting Consult received popup data for Useractivity");
                                        sfdcData.UserActivityData = SFDCUserActivity.GetInstance().GetVoiceCreateUserAcitivityData(message, callType);
                                    }
                                    break;

                                default:
                                    if (this.customObjectOptions.ContainsKey(key))
                                    {
                                        if (this.customObjectOptions[key] != null && this.customObjectOptions[key].ConsultPopupEvent != null &&
                                                this.customObjectOptions[key].ConsultPopupEvent.Equals(eventName))
                                        {
                                            ICustomObject cstobject = SFDCCustomObject.GetInstance().GetCustomObjectVoicePopupData(message, callType, key);
                                            if (cstobject != null)
                                            {
                                                this.tempCustomObject.Add(cstobject);
                                            }
                                        }
                                    }
                                    break;
                            }
                        }
                    }
                    if (this.tempCustomObject.Count > 0)
                    {
                        logger.Info("GetConsultReceivedPopupData : Collecting Consult received popup data for CustomObject");
                        ICustomObject[] temp = this.tempCustomObject.ToArray();
                        sfdcData.CustomObjectData = temp;
                        this.tempCustomObject.Clear();
                    }
                }
            }
            catch (Exception generalException)
            {
                logger.Error("GetConsultReceivedPopupData : Error Occurred while collecting Consult Received Popup Data : " + generalException.ToString());
            }
            return sfdcData;
        }

        public SFDCData GetConsultUpdateData(IMessage message, SFDCCallType callType, string callDuration, string eventName)
        {
            SFDCData sfdcData = new SFDCData();
            try
            {
                this.logger.Info("GetConsultUpdateData : Collecting update data for the consult call on Event : " + eventName);
                if (this.PopupPages != null)
                {
                    if (generalOptions.CanUseCommonSearchData && Settings.ProfileLevelActivity != null)
                        sfdcData.ProfileUpdateActivityLogData = SFDCUtility.GetInstance().GetVoiceUpdateActivityLog(Settings.ProfileLevelActivity, message, callType, callDuration);
                    foreach (string key in this.PopupPages)
                    {
                        switch (key)
                        {
                            case "lead":
                                if (this.leadOptions != null && this.leadOptions.ConsultUpdateEvent != null && this.leadOptions.ConsultUpdateEvent.Contains(eventName))
                                {
                                    sfdcData.LeadData = SFDCLead.GetInstance().GetLeadVoiceUpdateData(message, callType, callDuration);
                                }
                                break;

                            case "contact":
                                if (this.contactOptions != null && this.contactOptions.ConsultUpdateEvent != null && this.contactOptions.ConsultUpdateEvent.Contains(eventName))
                                {
                                    sfdcData.ContactData = SFDCContact.GetInstance().GetContactVoiceUpdateData(message, callType, callDuration);
                                }
                                break;

                            case "account":
                                if (this.accountOptions != null && this.accountOptions.ConsultUpdateEvent != null && this.accountOptions.ConsultUpdateEvent.Contains(eventName))
                                {
                                    sfdcData.AccountData = SFDCAccount.GetInstance().GetAccountUpdateData(message, callType, callDuration);
                                }
                                break;

                            case "case":
                                if (this.caseOptions != null && this.caseOptions.ConsultUpdateEvent != null && this.caseOptions.ConsultUpdateEvent.Contains(eventName))
                                {
                                    sfdcData.CaseData = SFDCCase.GetInstance().GetCaseVoiceUpdateData(message, callType, callDuration);
                                }
                                break;

                            case "opportunity":
                                if (this.opportunityOptions != null && this.opportunityOptions.ConsultUpdateEvent != null && this.opportunityOptions.ConsultUpdateEvent.Contains(eventName))
                                {
                                    sfdcData.OpportunityData = SFDCOpportunity.GetInstance().GetOpportunityVoiceUpdateData(message, callType, callDuration);
                                }
                                break;

                            case "useractivity":
                                if (this.userActivityOptions != null && this.userActivityOptions.ConsultUpdateEvent != null &&
                    this.userActivityOptions.ConsultUpdateEvent.Contains(eventName))
                                {
                                    sfdcData.UserActivityData = SFDCUserActivity.GetInstance().GetVoiceUpdateUserAcitivityData(message, callType, callDuration);
                                }
                                break;

                            default:
                                if (this.customObjectOptions.ContainsKey(key))
                                {
                                    if (this.customObjectOptions[key] != null && this.customObjectOptions[key].ConsultUpdateEvent != null &&
                                            this.customObjectOptions[key].ConsultUpdateEvent.Contains(eventName))
                                    {
                                        ICustomObject cstobject = SFDCCustomObject.GetInstance().GetCusotmObjectVoiceUpdateData(message, callType, callDuration, key);
                                        if (cstobject != null)
                                        {
                                            this.tempCustomObject.Add(cstobject);
                                        }
                                    }
                                }
                                break;
                        }
                    }
                    if (this.tempCustomObject.Count > 0)
                    {
                        ICustomObject[] temp = this.tempCustomObject.ToArray();
                        sfdcData.CustomObjectData = temp;
                        this.tempCustomObject.Clear();
                    }
                }
            }
            catch (Exception generalException)
            {
                logger.Error("GetConsultUpdateData : Error Occurred  : " + generalException.ToString());
            }
            return sfdcData;
        }

        public SFDCData GetInboundPopupData(IMessage message, SFDCCallType callType, string eventName)
        {
            SFDCData sfdcData = new SFDCData();
            try
            {
                this.logger.Info("GetInboundPopupData : Collecting popup data for Inbound on Event : " + eventName);
                if (this.PopupPages != null)
                {
                    if (generalOptions.CanUseCommonSearchData)
                    {
                        if (this.generalOptions.InboundPopupEvent != null && this.generalOptions.InboundPopupEvent.Equals(eventName))
                        {
                            sfdcData.CommonSearchData = GetCommonVoiceSearchValue(message, callType);
                            sfdcData.CommonSearchFields = this.generalOptions.SearchField;
                            sfdcData.CommonPopupObjects = Settings.CommonPopupObjects;
                            sfdcData.CommonSearchFormats = this.generalOptions.PhoneNumberSearchFormat;
                            sfdcData.CommonSearchCondition = this.generalOptions.CommonSearchCondition;
                            if (Settings.ProfileLevelActivity != null)
                            {
                                sfdcData.ProfileActivityLogData = SFDCUtility.GetInstance().GetVoiceActivityLog(Settings.ProfileLevelActivity, message, callType);
                            }
                            foreach (string key in this.PopupPages)
                            {
                                switch (key)
                                {
                                    case "lead":
                                        if (this.leadOptions != null)
                                        {
                                            logger.Info("GetInboundPopupData : Collecting Inbound Voice Popup data for Lead");
                                            sfdcData.LeadData = SFDCLead.GetInstance().GetLeadVoicePopupData(message, callType);
                                        }
                                        else
                                        {
                                            logger.Warn("GetInboundPopupData : Lead Options is empty");
                                        }
                                        break;

                                    case "contact":
                                        if (this.contactOptions != null)
                                        {
                                            logger.Info("GetInboundPopupData : Collecting Inbound Voice Popup data for contact");
                                            sfdcData.ContactData = SFDCContact.GetInstance().GetContactVoicePopupData(message, callType);
                                        }
                                        else
                                        {
                                            logger.Warn("GetInboundPopupData : contact Options is empty");
                                        }
                                        break;

                                    case "account":
                                        if (this.accountOptions != null)
                                        {
                                            logger.Info("GetInboundPopupData : Collecting Inbound Voice Popup data for account");
                                            sfdcData.AccountData = SFDCAccount.GetInstance().GetAccountVoicePopupData(message, callType);
                                        }
                                        else
                                        {
                                            logger.Warn("GetInboundPopupData : account Options is empty");
                                        }
                                        break;

                                    case "case":
                                        if (this.caseOptions != null)
                                        {
                                            logger.Info("GetInboundPopupData : Collecting Inbound Voice Popup data for case");
                                            sfdcData.CaseData = SFDCCase.GetInstance().GetCaseVoicePopupData(message, callType);
                                        }
                                        else
                                        {
                                            logger.Warn("GetInboundPopupData : case Options is empty");
                                        }
                                        break;

                                    case "opportunity":
                                        if (this.opportunityOptions != null)
                                        {
                                            logger.Info("GetInboundPopupData : Collecting Inbound Voice Popup data for opportunity");
                                            sfdcData.OpportunityData = SFDCOpportunity.GetInstance().GetOpportunityVoicePopupData(message, callType);
                                        }
                                        else
                                        {
                                            logger.Warn("GetInboundPopupData : opportunity Options is empty");
                                        }
                                        break;

                                    case "useractivity":
                                        if (this.userActivityOptions != null)
                                        {
                                            logger.Info("GetInboundPopupData : Collecting Inbound Voice Popup data for useractivity");
                                            sfdcData.UserActivityData = SFDCUserActivity.GetInstance().GetVoiceCreateUserAcitivityData(message, callType);
                                        }
                                        else
                                        {
                                            logger.Warn("GetInboundPopupData : useractivity Options is empty");
                                        }
                                        break;

                                    default:
                                        if (customObjectOptions != null)
                                        {
                                            if (this.customObjectOptions.ContainsKey(key))
                                            {
                                                ICustomObject cstobject = SFDCCustomObject.GetInstance().GetCustomObjectVoicePopupData(message, callType, key);
                                                if (cstobject != null)
                                                {
                                                    this.tempCustomObject.Add(cstobject);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            logger.Warn("GetInboundPopupData : customObjectOptions Options is empty");
                                        }
                                        break;
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (string key in this.PopupPages)
                        {
                            switch (key)
                            {
                                case "lead":
                                    if (this.leadOptions != null && this.leadOptions.InboundPopupEvent != null && this.leadOptions.InboundPopupEvent.Equals(eventName))
                                    {
                                        logger.Info("GetInboundPopupData : Collecting Inbound Voice Popup data for Lead");
                                        sfdcData.LeadData = SFDCLead.GetInstance().GetLeadVoicePopupData(message, callType);
                                    }
                                    break;

                                case "contact":
                                    if (this.contactOptions != null && this.contactOptions.InboundPopupEvent != null && this.contactOptions.InboundPopupEvent.Equals(eventName))
                                    {
                                        logger.Info("GetInboundPopupData : Collecting Inbound Voice Popup data for contact");
                                        sfdcData.ContactData = SFDCContact.GetInstance().GetContactVoicePopupData(message, callType);
                                    }
                                    break;

                                case "account":
                                    if (this.accountOptions != null && this.accountOptions.InboundPopupEvent != null && this.accountOptions.InboundPopupEvent.Equals(eventName))
                                    {
                                        logger.Info("GetInboundPopupData : Collecting Inbound Voice Popup data for account");
                                        sfdcData.AccountData = SFDCAccount.GetInstance().GetAccountVoicePopupData(message, callType);
                                    }
                                    break;

                                case "case":
                                    if (this.caseOptions != null && this.caseOptions.InboundPopupEvent != null && this.caseOptions.InboundPopupEvent.Equals(eventName))
                                    {
                                        logger.Info("GetInboundPopupData : Collecting Inbound Voice Popup data for case");
                                        sfdcData.CaseData = SFDCCase.GetInstance().GetCaseVoicePopupData(message, callType);
                                    }
                                    break;

                                case "opportunity":
                                    if (this.opportunityOptions != null && this.opportunityOptions.InboundPopupEvent != null && this.opportunityOptions.InboundPopupEvent.Equals(eventName))
                                    {
                                        logger.Info("GetInboundPopupData : Collecting Inbound Voice Popup data for opportunity");
                                        sfdcData.OpportunityData = SFDCOpportunity.GetInstance().GetOpportunityVoicePopupData(message, callType);
                                    }
                                    break;

                                case "useractivity":
                                    if (this.userActivityOptions != null && this.userActivityOptions.InboundPopupEvent != null &&
                        this.userActivityOptions.InboundPopupEvent.Equals(eventName))
                                    {
                                        logger.Info("GetInboundPopupData : Collecting Inbound Voice Popup data for useractivity");
                                        sfdcData.UserActivityData = SFDCUserActivity.GetInstance().GetVoiceCreateUserAcitivityData(message, callType);
                                    }
                                    break;

                                default:
                                    if (this.customObjectOptions.ContainsKey(key))
                                    {
                                        if (this.customObjectOptions[key] != null && this.customObjectOptions[key].InboundPopupEvent != null &&
                                                this.customObjectOptions[key].InboundPopupEvent.Equals(eventName))
                                        {
                                            ICustomObject cstobject = SFDCCustomObject.GetInstance().GetCustomObjectVoicePopupData(message, callType, key);
                                            if (cstobject != null)
                                            {
                                                this.tempCustomObject.Add(cstobject);
                                            }
                                        }
                                    }
                                    break;
                            }
                        }
                    }
                    if (this.tempCustomObject.Count > 0)
                    {
                        logger.Info("GetInboundPopupData : Collecting Inbound Voice Popup data for CustomObject");
                        ICustomObject[] temp = this.tempCustomObject.ToArray();
                        sfdcData.CustomObjectData = temp;
                        this.tempCustomObject.Clear();
                    }
                }
            }
            catch (Exception generalException)
            {
                logger.Error("GetInboundPopupData : Error Occurred while collecting Inbound Popup data : " + generalException.ToString());
            }
            return sfdcData;
        }

        public SFDCData GetInboundUpdateData(IMessage message, SFDCCallType callType, string callDuration, string eventName)
        {
            SFDCData sfdcData = new SFDCData();
            try
            {
                if (generalOptions.CanUseCommonSearchData && Settings.ProfileLevelActivity != null)
                    sfdcData.ProfileUpdateActivityLogData = SFDCUtility.GetInstance().GetVoiceUpdateActivityLog(Settings.ProfileLevelActivity, message, callType, callDuration);

                this.logger.Info("GetInboundUpdateData : Collecting update data for the Inbound call on Event : " + eventName);
                if (this.PopupPages != null)
                {
                    foreach (string key in this.PopupPages)
                    {
                        switch (key)
                        {
                            case "lead":
                                if (this.leadOptions != null && this.leadOptions.InboundUpdateEvent != null && this.leadOptions.InboundUpdateEvent.Contains(eventName))
                                {
                                    sfdcData.LeadData = SFDCLead.GetInstance().GetLeadVoiceUpdateData(message, callType, callDuration);
                                }
                                break;

                            case "contact":
                                if (this.contactOptions != null && this.contactOptions.InboundUpdateEvent != null && this.contactOptions.InboundUpdateEvent.Contains(eventName))
                                {
                                    sfdcData.ContactData = SFDCContact.GetInstance().GetContactVoiceUpdateData(message, callType, callDuration);
                                }
                                break;

                            case "account":
                                if (this.accountOptions != null && this.accountOptions.InboundUpdateEvent != null && this.accountOptions.InboundUpdateEvent.Contains(eventName))
                                {
                                    sfdcData.AccountData = SFDCAccount.GetInstance().GetAccountUpdateData(message, callType, callDuration);
                                }
                                break;

                            case "case":
                                if (this.caseOptions != null && this.caseOptions.InboundUpdateEvent != null && this.caseOptions.InboundUpdateEvent.Contains(eventName))
                                {
                                    sfdcData.CaseData = SFDCCase.GetInstance().GetCaseVoiceUpdateData(message, callType, callDuration);
                                }
                                break;

                            case "opportunity":
                                if (this.opportunityOptions != null && this.opportunityOptions.InboundUpdateEvent != null && this.opportunityOptions.InboundUpdateEvent.Contains(eventName))
                                {
                                    sfdcData.OpportunityData = SFDCOpportunity.GetInstance().GetOpportunityVoiceUpdateData(message, callType, callDuration);
                                }
                                break;

                            case "useractivity":
                                if (this.userActivityOptions != null && this.userActivityOptions.InboundUpdateEvent != null &&
                    this.userActivityOptions.InboundUpdateEvent.Contains(eventName))
                                {
                                    sfdcData.UserActivityData = SFDCUserActivity.GetInstance().GetVoiceUpdateUserAcitivityData(message, callType, callDuration);
                                }
                                break;

                            default:
                                if (this.customObjectOptions.ContainsKey(key))
                                {
                                    if (this.customObjectOptions[key] != null && this.customObjectOptions[key].InboundUpdateEvent != null &&
                                            this.customObjectOptions[key].InboundUpdateEvent.Contains(eventName))
                                    {
                                        ICustomObject cstobject = SFDCCustomObject.GetInstance().GetCusotmObjectVoiceUpdateData(message, callType, callDuration, key);
                                        if (cstobject != null)
                                        {
                                            this.tempCustomObject.Add(cstobject);
                                        }
                                    }
                                }
                                break;
                        }
                    }
                    if (this.tempCustomObject.Count > 0)
                    {
                        ICustomObject[] temp = this.tempCustomObject.ToArray();
                        sfdcData.CustomObjectData = temp;
                        this.tempCustomObject.Clear();
                    }
                }
            }
            catch (Exception generalException)
            {
                logger.Error("GetInboundUpdateData : Error Occurred  : " + generalException.ToString());
            }
            return sfdcData;
        }

        public SFDCData GetOutboundFailurePopupData(IMessage message, SFDCCallType callType, string eventName)
        {
            SFDCData sfdcData = new SFDCData();
            try
            {
                this.logger.Info("GetOutboundFailurePopupData : Collecting popup data for outbound failure on Event : " + eventName);
                if (this.PopupPages != null)
                {
                    if (generalOptions.CanUseCommonSearchData)
                    {
                        if (this.generalOptions.OutboundFailurePopupEvent != null && this.generalOptions.OutboundFailurePopupEvent.Contains(eventName))
                        {
                            sfdcData.CommonSearchData = GetCommonVoiceSearchValue(message, callType);
                            sfdcData.CommonSearchFields = this.generalOptions.SearchField;
                            sfdcData.CommonPopupObjects = Settings.CommonPopupObjects;
                            sfdcData.CommonSearchFormats = this.generalOptions.PhoneNumberSearchFormat;
                            sfdcData.CommonSearchCondition = this.generalOptions.CommonSearchCondition;
                            if (generalOptions.CanUseCommonSearchData && Settings.ProfileLevelActivity != null)
                                sfdcData.ProfileActivityLogData = SFDCUtility.GetInstance().GetVoiceActivityLog(Settings.ProfileLevelActivity, message, callType);
                            foreach (string key in this.PopupPages)
                            {
                                switch (key)
                                {
                                    case "lead":
                                        if (leadOptions != null)
                                        {
                                            logger.Info("GetOutboundFailurePopupData : Collecting Inbound Voice Popup data for Lead");
                                            sfdcData.LeadData = SFDCLead.GetInstance().GetLeadVoicePopupData(message, callType);
                                        }
                                        else
                                        {
                                            logger.Warn("GetOutboundFailurePopupData : Lead Options is empty");
                                        }
                                        break;

                                    case "contact":
                                        if (contactOptions != null)
                                        {
                                            logger.Info("GetOutboundFailurePopupData : Collecting Inbound Voice Popup data for contact");
                                            sfdcData.ContactData = SFDCContact.GetInstance().GetContactVoicePopupData(message, callType);
                                        }
                                        else
                                        {
                                            logger.Warn("GetOutboundFailurePopupData : Contact Options is empty");
                                        }
                                        break;

                                    case "account":
                                        if (accountOptions != null)
                                        {
                                            logger.Info("GetOutboundFailurePopupData : Collecting Inbound Voice Popup data for account");
                                            sfdcData.AccountData = SFDCAccount.GetInstance().GetAccountVoicePopupData(message, callType);
                                        }
                                        else
                                        {
                                            logger.Warn("GetOutboundFailurePopupData : Account Options is empty");
                                        }
                                        break;

                                    case "case":
                                        if (caseOptions != null)
                                        {
                                            logger.Info("GetOutboundFailurePopupData : Collecting Inbound Voice Popup data for case");
                                            sfdcData.CaseData = SFDCCase.GetInstance().GetCaseVoicePopupData(message, callType);
                                        }
                                        else
                                        {
                                            logger.Warn("GetOutboundFailurePopupData : Case Options is empty");
                                        }
                                        break;

                                    case "opportunity":
                                        if (opportunityOptions != null)
                                        {
                                            logger.Info("GetOutboundFailurePopupData : Collecting Inbound Voice Popup data for opportunity");
                                            sfdcData.OpportunityData = SFDCOpportunity.GetInstance().GetOpportunityVoicePopupData(message, callType);
                                        }
                                        else
                                        {
                                            logger.Warn("GetOutboundFailurePopupData : Opportunity Options is empty");
                                        }
                                        break;

                                    case "useractivity":
                                        if (userActivityOptions != null)
                                        {
                                            logger.Info("GetOutboundFailurePopupData : Collecting Inbound Voice Popup data for useractivity");
                                            sfdcData.UserActivityData = SFDCUserActivity.GetInstance().GetVoiceCreateUserAcitivityData(message, callType);
                                        }
                                        else
                                        {
                                            logger.Warn("GetOutboundFailurePopupData : Useractivity Options is empty");
                                        }
                                        break;

                                    default:
                                        if (this.customObjectOptions != null && this.customObjectOptions.ContainsKey(key))
                                        {
                                            ICustomObject cstobject = SFDCCustomObject.GetInstance().GetCustomObjectVoicePopupData(message, callType, key);
                                            if (cstobject != null)
                                            {
                                                this.tempCustomObject.Add(cstobject);
                                            }
                                        }
                                        else
                                        {
                                            logger.Warn("GetOutboundFailurePopupData : CustomObjectOptions Options is empty");
                                        }
                                        break;
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (string key in this.PopupPages)
                        {
                            switch (key)
                            {
                                case "lead":
                                    if (this.leadOptions != null && this.leadOptions.OutboundFailurePopupEvent != null && this.leadOptions.OutboundFailurePopupEvent.Contains(eventName))
                                    {
                                        sfdcData.LeadData = SFDCLead.GetInstance().GetLeadVoicePopupData(message, callType);
                                    }
                                    break;

                                case "contact":
                                    if (this.contactOptions != null && this.contactOptions.OutboundFailurePopupEvent != null && contactOptions.OutboundFailurePopupEvent.Contains(eventName))
                                    {
                                        sfdcData.ContactData = SFDCContact.GetInstance().GetContactVoicePopupData(message, callType);
                                    }
                                    break;

                                case "account":
                                    if (this.accountOptions != null && this.accountOptions.OutboundFailurePopupEvent != null && this.accountOptions.OutboundFailurePopupEvent.Contains(eventName))
                                    {
                                        sfdcData.AccountData = SFDCAccount.GetInstance().GetAccountVoicePopupData(message, callType);
                                    }
                                    break;

                                case "case":
                                    if (this.caseOptions != null && this.caseOptions.OutboundFailurePopupEvent != null && this.caseOptions.OutboundFailurePopupEvent.Contains(eventName))
                                    {
                                        sfdcData.CaseData = SFDCCase.GetInstance().GetCaseVoicePopupData(message, callType);
                                    }
                                    break;

                                case "opportunity":
                                    if (this.opportunityOptions != null && this.opportunityOptions.OutboundFailurePopupEvent != null && this.opportunityOptions.OutboundFailurePopupEvent.Contains(eventName))
                                    {
                                        sfdcData.OpportunityData = SFDCOpportunity.GetInstance().GetOpportunityVoicePopupData(message, callType);
                                    }
                                    break;

                                case "useractivity":
                                    if (this.userActivityOptions != null && this.userActivityOptions.OutboundFailurePopupEvent != null &&
                        this.userActivityOptions.OutboundFailurePopupEvent.Contains(eventName))
                                    {
                                        sfdcData.UserActivityData = SFDCUserActivity.GetInstance().GetVoiceCreateUserAcitivityData(message, callType);
                                    }
                                    break;

                                default:
                                    if (this.customObjectOptions.ContainsKey(key))
                                    {
                                        if (this.customObjectOptions[key] != null && this.customObjectOptions[key].OutboundFailurePopupEvent != null &&
                                                this.customObjectOptions[key].OutboundFailurePopupEvent.Contains(eventName))
                                        {
                                            ICustomObject cstobject = SFDCCustomObject.GetInstance().GetCustomObjectVoicePopupData(message, callType, key);
                                            if (cstobject != null)
                                            {
                                                this.tempCustomObject.Add(cstobject);
                                            }
                                        }
                                    }
                                    break;
                            }
                        }
                    }
                    if (this.tempCustomObject.Count > 0)
                    {
                        ICustomObject[] temp = this.tempCustomObject.ToArray();
                        sfdcData.CustomObjectData = temp;
                        this.tempCustomObject.Clear();
                    }
                }
            }
            catch (Exception generalException)
            {
                logger.Error("GetOutboundFailurePopupData : Error Occurred  : " + generalException.ToString());
            }
            return sfdcData;
        }

        public SFDCData GetOutboundPopupData(IMessage message, SFDCCallType callType, string eventName)
        {
            SFDCData sfdcData = new SFDCData();
            try
            {
                this.logger.Info("GetOutboundPopupData : Collecting popup data for outbound on Event : " + eventName);
                if (this.PopupPages != null)
                {
                    if (generalOptions.CanUseCommonSearchData)
                    {
                        if (this.generalOptions.OutboundPopupEvent != null && this.generalOptions.OutboundPopupEvent.Equals(eventName))
                        {
                            sfdcData.CommonSearchData = GetCommonVoiceSearchValue(message, callType);
                            sfdcData.CommonSearchFields = this.generalOptions.SearchField;
                            sfdcData.CommonPopupObjects = Settings.CommonPopupObjects;
                            sfdcData.CommonSearchFormats = this.generalOptions.PhoneNumberSearchFormat;
                            sfdcData.CommonSearchCondition = this.generalOptions.CommonSearchCondition;
                            if (Settings.ProfileLevelActivity != null)
                            {
                                sfdcData.ProfileActivityLogData = SFDCUtility.GetInstance().GetVoiceActivityLog(Settings.ProfileLevelActivity, message, callType);
                            }
                            foreach (string key in this.PopupPages)
                            {
                                switch (key)
                                {
                                    case "lead":
                                        if (leadOptions != null)
                                        {
                                            logger.Info("GetOutboundPopupData : Collecting Outbound Voice Popup data for Lead");
                                            sfdcData.LeadData = SFDCLead.GetInstance().GetLeadVoicePopupData(message, callType);
                                        }
                                        else
                                        {
                                            logger.Warn("GetOutboundPopupData : Lead Options is empty");
                                        }
                                        break;

                                    case "contact":
                                        if (contactOptions != null)
                                        {
                                            logger.Info("GetOutboundPopupData : Collecting Outbound Voice Popup data for contact");
                                            sfdcData.ContactData = SFDCContact.GetInstance().GetContactVoicePopupData(message, callType);
                                        }
                                        else
                                        {
                                            logger.Warn("GetOutboundPopupData : Contact Options is empty");
                                        }
                                        break;

                                    case "account":
                                        if (accountOptions != null)
                                        {
                                            logger.Info("GetOutboundPopupData : Collecting Outbound Voice Popup data for account");
                                            sfdcData.AccountData = SFDCAccount.GetInstance().GetAccountVoicePopupData(message, callType);
                                        }
                                        else
                                        {
                                            logger.Warn("GetOutboundPopupData : Account Options is empty");
                                        }
                                        break;

                                    case "case":
                                        if (caseOptions != null)
                                        {
                                            logger.Info("GetOutboundPopupData : Collecting Outbound Voice Popup data for case");
                                            sfdcData.CaseData = SFDCCase.GetInstance().GetCaseVoicePopupData(message, callType);
                                        }
                                        else
                                        {
                                            logger.Warn("GetOutboundPopupData : Case Options is empty");
                                        }
                                        break;

                                    case "opportunity":
                                        if (opportunityOptions != null)
                                        {
                                            logger.Info("GetOutboundPopupData : Collecting Outbound Voice Popup data for opportunity");
                                            sfdcData.OpportunityData = SFDCOpportunity.GetInstance().GetOpportunityVoicePopupData(message, callType);
                                        }
                                        else
                                        {
                                            logger.Warn("GetOutboundPopupData : Opportunity Options is empty");
                                        }
                                        break;

                                    case "useractivity":
                                        if (userActivityOptions != null)
                                        {
                                            logger.Info("GetOutboundPopupData : Collecting Outbound Voice Popup data for useractivity");
                                            sfdcData.UserActivityData = SFDCUserActivity.GetInstance().GetVoiceCreateUserAcitivityData(message, callType);
                                        }
                                        else
                                        {
                                            logger.Warn("GetOutboundPopupData : Useractivity Options is empty");
                                        }
                                        break;

                                    default:
                                        if (customObjectOptions != null)
                                        {
                                            if (this.customObjectOptions.ContainsKey(key))
                                            {
                                                ICustomObject cstobject = SFDCCustomObject.GetInstance().GetCustomObjectVoicePopupData(message, callType, key);
                                                if (cstobject != null)
                                                {
                                                    this.tempCustomObject.Add(cstobject);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            logger.Warn("GetOutboundPopupData : CustomObjectOptions Options is empty");
                                        }
                                        break;
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (string key in this.PopupPages)
                        {
                            switch (key)
                            {
                                case "lead":
                                    if (this.leadOptions != null && this.leadOptions.OutboundPopupEvent != null && this.leadOptions.OutboundPopupEvent.Equals(eventName))
                                    {
                                        logger.Info("GetOutboundPopupData : Collecting Outbound Popup Data for Lead");
                                        sfdcData.LeadData = SFDCLead.GetInstance().GetLeadVoicePopupData(message, callType);
                                    }
                                    break;

                                case "contact":
                                    if (this.contactOptions != null && this.contactOptions.OutboundPopupEvent != null && contactOptions.OutboundPopupEvent.Equals(eventName))
                                    {
                                        logger.Info("GetOutboundPopupData : Collecting Outbound Popup Data for Lead");
                                        sfdcData.ContactData = SFDCContact.GetInstance().GetContactVoicePopupData(message, callType);
                                    }
                                    break;

                                case "account":
                                    if (this.accountOptions != null && this.accountOptions.OutboundPopupEvent != null && this.accountOptions.OutboundPopupEvent.Equals(eventName))
                                    {
                                        logger.Info("GetOutboundPopupData : Collecting Outbound Popup Data for Lead");
                                        sfdcData.AccountData = SFDCAccount.GetInstance().GetAccountVoicePopupData(message, callType);
                                    }
                                    break;

                                case "case":
                                    if (this.caseOptions != null && this.caseOptions.OutboundPopupEvent != null && this.caseOptions.OutboundPopupEvent.Equals(eventName))
                                    {
                                        logger.Info("GetOutboundPopupData : Collecting Outbound Popup Data for Lead");
                                        sfdcData.CaseData = SFDCCase.GetInstance().GetCaseVoicePopupData(message, callType);
                                    }
                                    break;

                                case "opportunity":
                                    if (this.opportunityOptions != null && this.opportunityOptions.OutboundPopupEvent != null && this.opportunityOptions.OutboundPopupEvent.Equals(eventName))
                                    {
                                        logger.Info("GetOutboundPopupData : Collecting Outbound Popup Data for Lead");
                                        sfdcData.OpportunityData = SFDCOpportunity.GetInstance().GetOpportunityVoicePopupData(message, callType);
                                    }
                                    break;

                                case "useractivity":
                                    if (this.userActivityOptions != null && this.userActivityOptions.OutboundPopupEvent != null &&
                        this.userActivityOptions.OutboundPopupEvent.Equals(eventName))
                                    {
                                        logger.Info("GetOutboundPopupData : Collecting Outbound Popup Data for Lead");
                                        sfdcData.UserActivityData = SFDCUserActivity.GetInstance().GetVoiceCreateUserAcitivityData(message, callType);
                                    }
                                    break;

                                default:
                                    if (this.customObjectOptions.ContainsKey(key))
                                    {
                                        if (this.customObjectOptions[key] != null && this.customObjectOptions[key].OutboundPopupEvent != null &&
                                                this.customObjectOptions[key].OutboundPopupEvent.Equals(eventName))
                                        {
                                            ICustomObject cstobject = SFDCCustomObject.GetInstance().GetCustomObjectVoicePopupData(message, callType, key);
                                            if (cstobject != null)
                                            {
                                                this.tempCustomObject.Add(cstobject);
                                            }
                                        }
                                    }
                                    break;
                            }
                        }
                    }
                    if (this.tempCustomObject.Count > 0)
                    {
                        ICustomObject[] temp = this.tempCustomObject.ToArray();
                        sfdcData.CustomObjectData = temp;
                        this.tempCustomObject.Clear();
                    }
                }
            }
            catch (Exception generalException)
            {
                logger.Error("GetOutboundPopupData : Error Occurred  : " + generalException.ToString());
            }
            return sfdcData;
        }

        public SFDCData GetOutboundUpdateData(IMessage message, SFDCCallType callType, string callDuration, string eventName)
        {
            SFDCData sfdcData = new SFDCData();
            try
            {
                this.logger.Info("GetOutboundUpdateData : Collecting update data for the Outbound call on Event : " + eventName);
                if (this.PopupPages != null)
                {
                    if (generalOptions.CanUseCommonSearchData && Settings.ProfileLevelActivity != null)
                        sfdcData.ProfileUpdateActivityLogData = SFDCUtility.GetInstance().GetVoiceUpdateActivityLog(Settings.ProfileLevelActivity, message, callType, callDuration);
                    foreach (string key in this.PopupPages)
                    {
                        switch (key)
                        {
                            case "lead":
                                if (this.leadOptions != null && this.leadOptions.OutboundUpdateEvent != null && this.leadOptions.OutboundUpdateEvent.Contains(eventName))
                                {
                                    sfdcData.LeadData = SFDCLead.GetInstance().GetLeadVoiceUpdateData(message, callType, callDuration);
                                }
                                break;

                            case "contact":
                                if (this.contactOptions != null && this.contactOptions.OutboundUpdateEvent != null && this.contactOptions.OutboundUpdateEvent.Contains(eventName))
                                {
                                    sfdcData.ContactData = SFDCContact.GetInstance().GetContactVoiceUpdateData(message, callType, callDuration);
                                }
                                break;

                            case "account":
                                if (this.accountOptions != null && this.accountOptions.OutboundUpdateEvent != null && this.accountOptions.OutboundUpdateEvent.Contains(eventName))
                                {
                                    sfdcData.AccountData = SFDCAccount.GetInstance().GetAccountUpdateData(message, callType, callDuration);
                                }
                                break;

                            case "case":
                                if (this.caseOptions != null && this.caseOptions.OutboundUpdateEvent != null && this.caseOptions.OutboundUpdateEvent.Contains(eventName))
                                {
                                    sfdcData.CaseData = SFDCCase.GetInstance().GetCaseVoiceUpdateData(message, callType, callDuration);
                                }
                                break;

                            case "opportunity":
                                if (this.opportunityOptions != null && this.opportunityOptions.OutboundUpdateEvent != null && this.opportunityOptions.OutboundUpdateEvent.Contains(eventName))
                                {
                                    sfdcData.OpportunityData = SFDCOpportunity.GetInstance().GetOpportunityVoiceUpdateData(message, callType, callDuration);
                                }
                                break;

                            case "useractivity":
                                if (this.userActivityOptions != null && this.userActivityOptions.OutboundUpdateEvent != null &&
                    this.userActivityOptions.OutboundUpdateEvent.Contains(eventName))
                                {
                                    sfdcData.UserActivityData = SFDCUserActivity.GetInstance().GetVoiceUpdateUserAcitivityData(message, callType, callDuration);
                                }
                                break;

                            default:
                                if (this.customObjectOptions.ContainsKey(key))
                                {
                                    if (this.customObjectOptions[key] != null && this.customObjectOptions[key].OutboundUpdateEvent != null &&
                                            this.customObjectOptions[key].OutboundUpdateEvent.Contains(eventName))
                                    {
                                        ICustomObject cstobject = SFDCCustomObject.GetInstance().GetCusotmObjectVoiceUpdateData(message, callType, callDuration, key);
                                        if (cstobject != null)
                                        {
                                            this.tempCustomObject.Add(cstobject);
                                        }
                                    }
                                }
                                break;
                        }
                    }
                    if (this.tempCustomObject.Count > 0)
                    {
                        ICustomObject[] temp = this.tempCustomObject.ToArray();
                        sfdcData.CustomObjectData = temp;
                        this.tempCustomObject.Clear();
                    }
                }
            }
            catch (Exception generalException)
            {
                logger.Error("GetOutboundUpdateData : Error Occurred  : " + generalException.ToString());
            }
            return sfdcData;
        }

        public void ProcessSearchData(string connId, SFDCData data, SFDCCallType calltype)
        {
            try
            {
                this.logger.Info("ProcessSearchData: Processing data for SFDC Popup for the connectionId : " + connId);
                if (data != null)
                {
                    if (!String.IsNullOrEmpty(data.CommonSearchData) && !String.IsNullOrEmpty(data.CommonPopupObjects))
                    {
                        ProcessSFDCResponseForCommonSearch(sfdcObject.SearchSFDC(data.CommonSearchData, data.CommonSearchCondition, data.CommonPopupObjects, data.CommonSearchFields,
                                data.CommonSearchFormats, data, calltype, connId), data, connId, calltype);
                    }
                    else
                    {
                        if (generalOptions.CanUseCommonSearchData)
                        {
                            this.logger.Info("ProcessSearchData: common search data is null or empty, Invoking object based search functionality..");
                        }
                        if (data.LeadData != null)
                        {
                            ProcessSFDCResponse(connId, sfdcObject.SearchSFDC(data.LeadData.SearchData, data.LeadData.SearchCondition, data.LeadData.ObjectName, data.LeadData.SearchFields,
                                data.LeadData.PhoneNumberSearchFormat, data, calltype, connId), data.LeadData, calltype);
                        }
                        if (data.AccountData != null)
                        {
                            ProcessSFDCResponse(connId, sfdcObject.SearchSFDC(data.AccountData.SearchData, data.AccountData.SearchCondition, data.AccountData.ObjectName, data.AccountData.SearchFields,
                                data.AccountData.PhoneNumberSearchFormat, data, calltype, connId), data.AccountData, calltype);
                        }
                        if (data.CaseData != null)
                        {
                            ProcessSFDCResponse(connId, sfdcObject.SearchSFDC(data.CaseData.SearchData, data.CaseData.SearchCondition, data.CaseData.ObjectName, data.CaseData.SearchFields,
                               data.CaseData.PhoneNumberSearchFormat, data, calltype, connId), data.CaseData, calltype);
                        }
                        if (data.ContactData != null)
                        {
                            ProcessSFDCResponse(connId, sfdcObject.SearchSFDC(data.ContactData.SearchData, data.ContactData.SearchCondition, data.ContactData.ObjectName, data.ContactData.SearchFields,
                               data.ContactData.PhoneNumberSearchFormat, data, calltype, connId), data.ContactData, calltype);
                        }
                        if (data.OpportunityData != null)
                        {
                            ProcessSFDCResponse(connId, sfdcObject.SearchSFDC(data.OpportunityData.SearchData, data.OpportunityData.SearchCondition, data.OpportunityData.ObjectName, data.OpportunityData.SearchFields,
                               data.OpportunityData.PhoneNumberSearchFormat, data, calltype, connId), data.OpportunityData, calltype);
                        }
                        if (data.CustomObjectData != null)
                        {
                            foreach (ICustomObject customObject in data.CustomObjectData)
                            {
                                ProcessSFDCResponse(connId, sfdcObject.SearchSFDC(customObject.SearchData, customObject.SearchCondition, customObject.ObjectName, customObject.SearchFields,
                              customObject.PhoneNumberSearchFormat, data, calltype, connId), customObject, calltype);
                            }
                        }
                    }
                }
            }
            catch (Exception generalException)
            {
                this.logger.Error("ProcessSearchData: Error Occurred :" + generalException.ToString());
            }
        }

        public void ProcessUpdateData(string connId, SFDCData sfdcData)
        {
            try
            {
                this.logger.Info("ProcessUpdateData : Processing Update Log Data for the ConnId : " + connId);
                if (Settings.UpdateSFDCLog.ContainsKey(connId))
                {
                    UpdateLogData updateData = Settings.UpdateSFDCLog[connId];
                    if (updateData != null)
                    {
                        if (!String.IsNullOrEmpty(updateData.LeadActivityId) && sfdcData.LeadData != null)
                        {
                            if (sfdcData.LeadData.UpdateActivityLogData != null)
                            {
                                this.sfdcObject.UpdateActivityLog(connId, sfdcData.LeadData.UpdateActivityLogData, sfdcData.LeadData.ObjectName, updateData.LeadActivityId);
                            }
                            else
                            {
                                this.logger.Info("Can not update Activity Log for Lead id : " + updateData.LeadActivityId + " because Update Activity Log data null...");
                            }
                        }
                        if (!String.IsNullOrEmpty(updateData.LeadRecordId) && sfdcData.LeadData != null)
                        {
                            sfdcObject.UpdateNewRecord(connId, sfdcData.LeadData.UpdateRecordFieldsData, sfdcData.LeadData.ObjectName.ToString(), updateData.LeadRecordId);
                        }

                        if (!String.IsNullOrEmpty(updateData.ContactActivityId) && sfdcData.ContactData != null)
                        {
                            if (sfdcData.ContactData.UpdateActivityLogData != null)
                            {
                                this.sfdcObject.UpdateActivityLog(connId, sfdcData.ContactData.UpdateActivityLogData, sfdcData.ContactData.ObjectName, updateData.ContactActivityId);
                            }
                            else
                            {
                                this.logger.Info("Can not update Activity Log for Contact id : " + updateData.ContactActivityId + " because Update Activity Log data null...");
                            }
                        }
                        if (!String.IsNullOrEmpty(updateData.ContactRecordId) && sfdcData.ContactData != null)
                        {
                            sfdcObject.UpdateNewRecord(connId, sfdcData.ContactData.UpdateRecordFieldsData, sfdcData.ContactData.ObjectName, updateData.ContactRecordId);
                        }

                        if (!String.IsNullOrEmpty(updateData.AccountActivityId) && sfdcData.AccountData != null)
                        {
                            if (sfdcData.AccountData.UpdateActivityLogData != null)
                            {
                                this.sfdcObject.UpdateActivityLog(connId, sfdcData.AccountData.UpdateActivityLogData, sfdcData.AccountData.ObjectName, updateData.AccountActivityId);
                            }
                            else
                            {
                                this.logger.Info("Can not update Activity Log for Account id : " + updateData.AccountActivityId + " because Update Activity Log data null...");
                            }
                        }
                        if (!String.IsNullOrEmpty(updateData.AccountRecordId) && sfdcData.AccountData != null)
                        {
                            sfdcObject.UpdateNewRecord(connId, sfdcData.AccountData.UpdateRecordFieldsData, sfdcData.AccountData.ObjectName, updateData.AccountRecordId);
                        }

                        if (!String.IsNullOrEmpty(updateData.CaseActivityId) && sfdcData.CaseData != null)
                        {
                            if (sfdcData.CaseData.UpdateActivityLogData != null)
                            {
                                this.sfdcObject.UpdateActivityLog(connId, sfdcData.CaseData.UpdateActivityLogData, sfdcData.CaseData.ObjectName, updateData.CaseActivityId);
                            }
                            else
                            {
                                this.logger.Info("Can not update Activity Log for Case id : " + updateData.CaseActivityId + " because Update Activity Log data null...");
                            }
                        }
                        if (!String.IsNullOrEmpty(updateData.CaseRecordId) && sfdcData.CaseData != null)
                        {
                            sfdcObject.UpdateNewRecord(connId, sfdcData.CaseData.UpdateRecordFieldsData, sfdcData.CaseData.ObjectName, updateData.CaseRecordId);
                        }

                        if (!String.IsNullOrEmpty(updateData.OpportunityActivityId) && sfdcData.OpportunityData != null)
                        {
                            if (sfdcData.OpportunityData.UpdateActivityLogData != null)
                            {
                                this.sfdcObject.UpdateActivityLog(connId, sfdcData.OpportunityData.UpdateActivityLogData, sfdcData.OpportunityData.ObjectName, updateData.OpportunityActivityId);
                            }
                            else
                            {
                                this.logger.Info("Can not update Activity Log for Opportunity id : " + updateData.OpportunityActivityId + " because Update Activity Log data null...");
                            }
                        }
                        if (!String.IsNullOrEmpty(updateData.OpportunityRecordId) && sfdcData.OpportunityData != null)
                        {
                            sfdcObject.UpdateNewRecord(connId, sfdcData.OpportunityData.UpdateRecordFieldsData, sfdcData.OpportunityData.ObjectName, updateData.OpportunityRecordId);
                        }

                        if (sfdcData.UserActivityData != null)
                        {
                            sfdcData.UserActivityData.RecordID = updateData.UserActivityId;
                        }
                        //profile level log update
                        if (!String.IsNullOrEmpty(updateData.ProfileActivityId) && sfdcData.ProfileUpdateActivityLogData != null)
                        {
                            this.sfdcObject.UpdateActivityLog(connId, sfdcData.ProfileUpdateActivityLogData, "profileactivity", updateData.ProfileActivityId);
                        }
                        if (sfdcData.CustomObjectData != null && updateData.CustomObject != null)
                        {
                            foreach (ICustomObject custom in sfdcData.CustomObjectData)
                            {
                                if (!String.IsNullOrEmpty(custom.ObjectName) && updateData.CustomObject.ContainsKey(custom.ObjectName))
                                {
                                    KeyValueCollection collection = updateData.CustomObject[custom.ObjectName];
                                    if (collection != null)
                                    {
                                        if (collection.ContainsKey("newRecordId"))
                                        {
                                            if (custom.UpdateRecordFieldsData != null)
                                            {
                                                sfdcObject.UpdateNewRecord(connId, custom.UpdateRecordFieldsData, custom.ObjectName, collection["newRecordId"].ToString());
                                            }
                                            else
                                            {
                                                this.logger.Error("Can not update Custom Object Record because Update data is null...");
                                            }
                                        }
                                        if (collection.ContainsKey("activityRecordId"))
                                        {
                                            if (custom.UpdateActivityLogData != null)
                                            {
                                                this.sfdcObject.UpdateActivityLog(connId, custom.UpdateActivityLogData, custom.ObjectName, collection["activityRecordId"].ToString());
                                            }
                                            else
                                            {
                                                this.logger.Info("Can not update Activity Log for " + custom.ObjectName + " id : " + collection["activityRecordId"].ToString() + " because Update Activity Log data null...");
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    this.logger.Info("object name is null in customObject data");
                                    this.logger.Info("Object name : " + Convert.ToString(custom.ObjectName));
                                }
                            }
                        }
                    }
                }
                if (!Settings.UpdateSFDCLogFinishedEvent.ContainsKey(connId))
                {
                    Settings.UpdateSFDCLogFinishedEvent.Add(connId, sfdcData);
                }
                else
                {
                    Settings.UpdateSFDCLogFinishedEvent[connId] = sfdcData;
                }
            }
            catch (Exception generalException)
            {
                this.logger.Error("ProcessUpdateData : Error occurred while processing update Log :" + generalException.ToString() + "\r \n" + generalException.StackTrace);
            }
        }

        public void SendPopupData(string connId, PopupData data)
        {
            try
            {
                this.logger.Info("SendPopupData : Sending Popup data for the connection Id : " + connId);
                if (data != null)
                {
                    this.logger.Info("SendPopupData : Popup data added to the SFDC collection : " + data.ToString());
                    if (!Settings.SFDCPopupData.ContainsKey(connId))
                    {
                        Settings.SFDCPopupData.Add(connId, data);
                    }
                    else
                    {
                        Settings.SFDCPopupData.Add(connId + System.DateTime.Now.ToString("HH:mm:ss.ffffff"), data);
                    }
                }
                else
                {
                    this.logger.Info("SendPopupData : Popup data is null for the connection Id : " + connId);
                }
            }
            catch (Exception generalException)
            {
                this.logger.Error("SendPopupData : Error Occurred while adding popup data to collection " + generalException.ToString());
            }
        }

        public void SendUpdateData(string connId, PopupData data)
        {
            try
            {
                this.logger.Info("SendUpdateData : Sending Update data for the connection Id : " + connId);
                if (data != null)
                {
                    this.logger.Info("SendUpdateData : Update data added to the SFDC collection  : " + data.ToString());
                    if (!Settings.SFDCPopupData.ContainsKey(connId))
                    {
                        Settings.SFDCPopupData.Add(connId, data);
                    }
                    else
                    {
                        Settings.SFDCPopupData.Add(connId + System.DateTime.Now.ToString("HH:mm:ss.ffffff"), data);
                    }
                }
                else
                {
                    this.logger.Info("SendUpdateData : Popup data is null for the connection Id : " + connId);
                }
            }
            catch (Exception generalException)
            {
                this.logger.Error("SendUpdateData : Error Occurred while adding Update data to collection " + generalException.ToString());
                Settings.SFDCPopupData.Add(connId + System.DateTime.Now.ToString("HH:mm:ss.ffffff") + "upt", data);
            }
        }

        private bool CheckCanCreateNoRecordforNone(ISFDCObjectProperty data, SFDCCallType calltype)
        {
            if (calltype.Equals(SFDCCallType.Inbound) && data.CanCreateProfileActivityforInbNoRecord)
            {
                return true;
            }
            if ((calltype.Equals(SFDCCallType.OutboundFailure) || calltype.Equals(SFDCCallType.OutboundSuccess))
                && data.CanCreateProfileActivityforOutNoRecord)
            {
                return true;
            }
            if ((calltype.Equals(SFDCCallType.ConsultFailure)
                || calltype.Equals(SFDCCallType.ConsultReceived) || calltype.Equals(SFDCCallType.ConsultSuccess))
                && data.CanCreateProfileActivityforConNoRecord)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Checks for exact match record found.
        /// </summary>
        /// <param name="RecordIds">The record ids.</param>
        /// <returns></returns>
        private bool CheckForExactMatchRecordFound(KeyValueCollection RecordIds)
        {
            foreach (string value in RecordIds.AllValues)
            {
                if (!value.Contains(','))
                {
                    return true;
                }
            }
            return false;
        }

        private void ClickToDialPopup(IMessage message, string connId, SFDCCallType callType, string objectType, string objectId)
        {
            try
            {
                this.logger.Info("GetClickToDialLogs : ClickToDial Popup.... ");
                this.logger.Info("GetClickToDialLogs : Call Type  : " + Convert.ToString(callType));
                this.logger.Info("GetClickToDialLogs : SFDC Obejct Type  : " + objectType);
                this.logger.Info("GetClickToDialLogs : SFDC Obejct Id  : " + objectId);
                PopupData popdata = new PopupData();
                List<XmlElement> element = new List<XmlElement>();
                switch (objectType)
                {
                    case "lead":
                        popdata.InteractionId = connId;
                        popdata.ObjectName = objectType;
                        element = sfdcObject.GetVoiceActivityLog(this.activityLogs["lead"], message, callType);
                        this.sfdcObject.CreateActivityLog(connId, element, "Lead", objectId);
                        Settings.SFDCPopupData.Add(connId, popdata);
                        break;

                    case "contact":
                        popdata.InteractionId = connId;
                        popdata.ObjectName = objectType;
                        element = sfdcObject.GetVoiceActivityLog(this.activityLogs["contact"], message, callType);
                        this.sfdcObject.CreateActivityLog(connId, element, "Contact", objectId);
                        Settings.SFDCPopupData.Add(connId, popdata);
                        break;

                    case "account":
                        popdata.InteractionId = connId;
                        popdata.ObjectName = objectType;
                        element = sfdcObject.GetVoiceActivityLog(this.activityLogs["account"], message, callType);
                        this.sfdcObject.CreateActivityLog(connId, element, "Account", objectId);
                        Settings.SFDCPopupData.Add(connId, popdata);
                        break;

                    case "case":
                        popdata.InteractionId = connId;
                        popdata.ObjectName = objectType;
                        element = sfdcObject.GetVoiceActivityLog(this.activityLogs["case"], message, callType);
                        this.sfdcObject.CreateActivityLog(connId, element, "Case", objectId);
                        Settings.SFDCPopupData.Add(connId, popdata);
                        break;

                    case "opportunity":
                        popdata.InteractionId = connId;
                        popdata.ObjectName = objectType;
                        element = sfdcObject.GetVoiceActivityLog(this.activityLogs["opportunity"], message, callType);
                        this.sfdcObject.CreateActivityLog(connId, element, "Opportunity", objectId);
                        Settings.SFDCPopupData.Add(connId, popdata);
                        break;

                    default:

                        if (objectType.Contains("__c") && Settings.CustomObjectNames.ContainsKey(objectType))
                        {
                            string objectName = Settings.CustomObjectNames[objectType];
                            if (this.activityLogs.ContainsKey(objectName))
                            {
                                popdata.InteractionId = connId;
                                popdata.ObjectName = objectType;
                                element = sfdcObject.GetVoiceActivityLog(this.activityLogs[objectName], message, callType);
                                this.sfdcObject.CreateActivityLog(connId, element, objectType, objectId);
                                Settings.SFDCPopupData.Add(connId, popdata);
                            }
                            else
                            {
                                this.logger.Info("GetClickToDialLogs : " + objectType + " Configuration not found ");
                            }
                        }
                        else
                        {
                            this.logger.Info("GetClickToDialLogs : Custom Object not found with Name : " + objectType);
                        }
                        break;
                }
            }
            catch (Exception generalException)
            {
                logger.Error("GetClickToDialLogs : Error Occurred  : " + generalException.ToString());
            }
        }

        private string GetCommonVoiceSearchValue(IMessage message, SFDCCallType callType)
        {
            try
            {
                this.logger.Info("GetCommonVoiceSearchValue :  Reading Common search data.....");
                this.logger.Info("GetCommonVoiceSearchValue :  Event Name : " + message.Name);
                this.logger.Info("GetCommonVoiceSearchValue :  CallType : " + callType.ToString());
                string[] userDataSearchKeys = null;
                string[] attributeSearchKeys = null;
                string searchValue = string.Empty;
                dynamic popupEvent = null;
                switch (message.Id)
                {
                    #region Events

                    case EventRinging.MessageId:
                        popupEvent = (EventRinging)message;
                        break;

                    case EventEstablished.MessageId:
                        popupEvent = (EventEstablished)message;
                        break;

                    case EventReleased.MessageId:
                        popupEvent = (EventReleased)message;
                        break;

                    case EventDialing.MessageId:
                        popupEvent = (EventDialing)message;
                        break;

                    case EventPartyChanged.MessageId:
                        popupEvent = (EventPartyChanged)message;
                        break;

                    case EventError.MessageId:
                        popupEvent = (EventError)message;
                        break;

                    case EventAbandoned.MessageId:
                        popupEvent = (EventAbandoned)message;
                        break;

                    case EventDestinationBusy.MessageId:
                        popupEvent = (EventDestinationBusy)message;
                        break;

                    default:
                        break;

                    #endregion Events
                }
                if (callType == SFDCCallType.Inbound)
                {
                    userDataSearchKeys = (this.generalOptions.InboundSearchUserDataKeys != null) ? this.generalOptions.InboundSearchUserDataKeys.Split(',') : null;
                    attributeSearchKeys = (this.generalOptions.InboundSearchAttributeKeys != null) ? this.generalOptions.InboundSearchAttributeKeys.Split(',') : null;
                }
                else if (callType == SFDCCallType.OutboundSuccess || callType == SFDCCallType.OutboundFailure)
                {
                    userDataSearchKeys = (this.generalOptions.OutboundSearchUserDataKeys != null) ? this.generalOptions.OutboundSearchUserDataKeys.Split(',') : null;
                    attributeSearchKeys = (this.generalOptions.OutboundSearchAttributeKeys != null) ? this.generalOptions.OutboundSearchAttributeKeys.Split(',') : null;
                }
                else if (callType == SFDCCallType.ConsultSuccess || callType == SFDCCallType.ConsultFailure)
                {
                    return this.sfdcObject.GetAttributeSearchValues(message, new string[] { "otherdn" });
                }
                else if (callType == SFDCCallType.ConsultReceived)
                {
                    userDataSearchKeys = (generalOptions.ConsultSearchUserDataKeys != null) ? generalOptions.ConsultSearchUserDataKeys.Split(',') : null;
                    attributeSearchKeys = (generalOptions.ConsultSearchAttributeKeys != null) ? generalOptions.ConsultSearchAttributeKeys.Split(',') : null;
                }

                if (this.generalOptions.VoiceSearchPriority == "user-data" && popupEvent != null)
                {
                    if (userDataSearchKeys != null && popupEvent.UserData != null)
                    {
                        searchValue = this.sfdcObject.GetUserDataSearchValues(popupEvent.UserData, userDataSearchKeys);
                        if (!this.sfdcObject.ValidateSearchData(searchValue))
                        {
                            this.logger.Info("search data from user-data keys not found, Reading attribute search keys.....");
                            searchValue = this.sfdcObject.GetAttributeSearchValues(message, attributeSearchKeys);
                        }
                    }
                    else if (attributeSearchKeys != null)
                    {
                        this.logger.Info("Either user-data search keys or user-data is null, Reading attribute search keys.....");
                        searchValue = this.sfdcObject.GetAttributeSearchValues(message, attributeSearchKeys);
                    }
                }
                else if (this.generalOptions.VoiceSearchPriority == "attribute")
                {
                    searchValue = this.sfdcObject.GetAttributeSearchValues(message, attributeSearchKeys);
                    if (!this.sfdcObject.ValidateSearchData(searchValue))
                    {
                        this.logger.Info("search data from attribute keys not found, Reading user-data search keys.....");
                        if (userDataSearchKeys != null && popupEvent.UserData != null)
                        {
                            searchValue = this.sfdcObject.GetUserDataSearchValues(popupEvent.UserData, userDataSearchKeys);
                        }
                        else
                        {
                            this.logger.Info("Either user-data or search keys are not found.....");
                        }
                    }
                }
                else if (this.generalOptions.VoiceSearchPriority == "both")
                {
                    if (popupEvent != null && popupEvent.UserData != null)
                        searchValue = this.sfdcObject.GetUserDataSearchValues(popupEvent.UserData, userDataSearchKeys);
                    if (!this.sfdcObject.ValidateSearchData(searchValue))
                    {
                        string temp = this.sfdcObject.GetAttributeSearchValues(message, attributeSearchKeys);
                        if (temp != string.Empty)
                        {
                            searchValue += "," + temp;
                        }
                    }
                    else
                    {
                        searchValue = this.sfdcObject.GetAttributeSearchValues(message, attributeSearchKeys);
                    }
                }
                return searchValue;
            }
            catch (Exception generalException)
            {
                this.logger.Error("GetCommonVoiceSearchValue : Error occurred while reading Lead Data : " + generalException.ToString());
            }
            return null;
        }

        private string GetNewPageUrl(string searchData, string urlId, string prepopulateIds)
        {
            try
            {
                this.logger.Info("GetNewPageUrl: SearchData : " + searchData + "\n UrlId : " + urlId + "\n PrePopulateIds : " + prepopulateIds);
                string data = "/" + urlId + "/e?";
                if (!String.IsNullOrEmpty(searchData) && !String.IsNullOrEmpty(prepopulateIds))
                {
                    searchData = searchData.Replace("^,", "").Replace(",^", "").Replace("^", "");
                    if (searchData.Contains(','))
                    {
                        string[] sdata = searchData.Split(',');
                        string[] pdata = prepopulateIds.Split(',');

                        if (sdata != null && pdata != null)
                        {
                            if (pdata.Length > sdata.Length || pdata.Length == sdata.Length)
                            {
                                for (int i = 0; i < sdata.Length; i++)
                                {
                                    data += pdata[i] + "=" + sdata[i] + "&";
                                }
                            }
                            else
                            {
                                for (int i = 0; i < pdata.Length; i++)
                                {
                                    data += pdata[i] + "=" + sdata[i] + "&";
                                }
                            }
                        }
                    }
                    else
                    {
                        if (searchData.Length == 10 && Regex.IsMatch(searchData, @"^\d+$"))
                        {
                            if (prepopulateIds.Contains(','))
                            {
                                data += prepopulateIds.Substring(0, (prepopulateIds.IndexOf(','))) + "=" + searchData;
                            }
                            else
                            {
                                data += prepopulateIds + "=" + searchData;
                            }
                        }
                    }
                }
                return data;
            }
            catch (Exception generalException)
            {
                this.logger.Error("GetNewPageUrl : " + generalException.ToString());
            }
            return null;
        }

        private KeyValueCollection GetRecordIdsFromRecords(SearchRecord[] searchRecords)
        {
            try
            {
                this.logger.Info("GetRecordIdsFromRecords: Spliting Search Records objectwise");
                KeyValueCollection RecordIds = new KeyValueCollection();
                if (searchRecords != null)
                {
                    foreach (SearchRecord searchRecord in searchRecords)
                    {
                        if (searchRecord != null && searchRecord.record != null && !string.IsNullOrWhiteSpace(searchRecord.record.Id))
                        {
                            switch (searchRecord.record.type)
                            {
                                case "Lead":
                                    if (RecordIds.ContainsKey("Lead"))
                                    {
                                        RecordIds["Lead"] += "," + searchRecord.record.Id;
                                    }
                                    else
                                    {
                                        RecordIds.Add("Lead", searchRecord.record.Id);
                                    }
                                    break;

                                case "Contact":
                                    if (RecordIds.ContainsKey("Contact"))
                                    {
                                        RecordIds["Contact"] += "," + searchRecord.record.Id;
                                    }
                                    else
                                    {
                                        RecordIds.Add("Contact", searchRecord.record.Id);
                                    }
                                    break;

                                case "Case":
                                    if (RecordIds.ContainsKey("Case"))
                                    {
                                        RecordIds["Case"] += "," + searchRecord.record.Id;
                                    }
                                    else
                                    {
                                        RecordIds.Add("Case", searchRecord.record.Id);
                                    }
                                    break;

                                case "Opportunity":
                                    if (RecordIds.ContainsKey("Opportunity"))
                                    {
                                        RecordIds["Opportunity"] += "," + searchRecord.record.Id;
                                    }
                                    else
                                    {
                                        RecordIds.Add("Opportunity", searchRecord.record.Id);
                                    }
                                    break;

                                case "Account":
                                    if (RecordIds.ContainsKey("Account"))
                                    {
                                        RecordIds["Account"] += "," + searchRecord.record.Id;
                                    }
                                    else
                                    {
                                        RecordIds.Add("Account", searchRecord.record.Id);
                                    }
                                    break;

                                default:
                                    if (RecordIds.ContainsKey(searchRecord.record.type))
                                    {
                                        RecordIds[searchRecord.record.type] += "," + searchRecord.record.Id;
                                    }
                                    else
                                    {
                                        RecordIds.Add(searchRecord.record.type, searchRecord.record.Id);
                                    }
                                    break;
                            }
                        }
                    }
                }
                return RecordIds;
            }
            catch (Exception generalException)
            {
                this.logger.Error("Error Occurred while parsing record id from SFDC Response : " + generalException.ToString());
            }
            return null;
        }

        private string GetSearchPageIds(SearchRecord[] records)
        {
            try
            {
                this.logger.Info("GetSearchPageIds: Getting search page record Ids...");
                if (records != null)
                {
                    string recordIds = string.Empty;
                    foreach (SearchRecord srecord in records)
                    {
                        recordIds += srecord.record.Id + ",";
                    }
                    if (recordIds.Length > 1)
                        return recordIds.Substring(0, (recordIds.Length - 1));
                }
            }
            catch (Exception generalException)
            {
                this.logger.Error("GetSearchPageIds: " + generalException.ToString());
            }
            return null;
        }

        private void ProcessSFDCResponse(string interactionId, OutputValues output, ISFDCObjectProperty data, SFDCCallType calltype)
        {
            try
            {
                this.logger.Info("ProcessSFDCResponse: Response from SFDC Search for the InteractionId : " + interactionId);
                if (output != null)
                {
                    this.logger.Info("ProcessSFDCResponse: Object Based Popup Invoked for the object " + output.ObjectName);
                    this.logger.Info("ProcessSFDCResponse: Data Received from SFDC : " + output.ToString());
                    PopupData popup = new PopupData();
                    PopupData ActivityPopup = new PopupData();
                    ActivityPopup.InteractionId = interactionId;
                    ActivityPopup.ObjectName = data.ObjectName;
                    string userLevelLog = string.Empty;
                    switch (output.ObjectName)
                    {
                        case "Lead":
                            if (output.SearchRecord != null)
                            {
                                #region Lead Process

                                if (output.SearchRecord.Length > 1)
                                {
                                    if (data.MultipleMatchRecord == "searchpage")
                                    {
                                        popup.InteractionId = interactionId;
                                        //multi match user level activity log creation
                                        if (data.CanCreateMultiMatchActivityLog && data.ActivityLogData != null)
                                        {
                                            userLevelLog = this.sfdcObject.CreateActivityLog(interactionId, data.ActivityLogData, data.ObjectName);
                                            if (data.CanPopupMultiMatchActivityLog && !string.IsNullOrWhiteSpace(userLevelLog))
                                            {
                                                ActivityPopup.PopupUrl = userLevelLog;
                                                this.SendPopupData(interactionId + "LMM", ActivityPopup);
                                            }
                                        }
                                        popup.PopupUrl = this.generalOptions.SearchPageUrl + this.leadOptions.SearchPageMode + "str=" + output.SearchData;
                                        this.SendPopupData(interactionId, popup);
                                    }
                                    else if (data.MultipleMatchRecord == "openall")
                                    {
                                        popup.InteractionId = interactionId;
                                        popup.PopupUrl = GetSearchPageIds(output.SearchRecord);
                                        this.SendPopupData(interactionId, popup);
                                    }
                                    else
                                        this.logger.Info("Multimatch Action for Lead objects is : None ");
                                }
                                else
                                {
                                    popup.InteractionId = interactionId;
                                    popup.PopupUrl = output.SearchRecord[0].record.Id;    // "/e?lea10=" + CallRinging.currentDNIS;
                                    if (data.ActivityLogData != null)
                                    {
                                        this.sfdcObject.CreateActivityLog(interactionId, data.ActivityLogData, data.ObjectName, output.SearchRecord[0].record.Id);
                                    }
                                    popup.ObjectName = "Lead";
                                    this.SendPopupData(interactionId, popup);
                                }
                            }
                            else if (!string.IsNullOrEmpty(output.SearchData))
                            {
                                if (data.NoRecordFound == "opennew")
                                {
                                    popup.InteractionId = interactionId;
                                    //no record found user level activity log creation
                                    if (data.CanCreateNoRecordActivityLog && data.ActivityLogData != null)
                                    {
                                        userLevelLog = this.sfdcObject.CreateActivityLog(interactionId, data.ActivityLogData, data.ObjectName);
                                        if (data.CanPopupNoRecordActivityLog && !string.IsNullOrWhiteSpace(userLevelLog))
                                        {
                                            ActivityPopup.PopupUrl = userLevelLog;
                                            this.SendPopupData(interactionId + "LNM", ActivityPopup);
                                        }
                                    }
                                    popup.PopupUrl = GetNewPageUrl(data.SearchData, "00Q", this.leadOptions.NewrecordFieldIds);
                                    this.SendPopupData(interactionId, popup);
                                }
                                else if (data.NoRecordFound == "createnew")
                                {
                                    popup.InteractionId = interactionId;
                                    popup.ObjectName = output.ObjectName;
                                    if (data.CreateRecordFieldData != null)
                                    {
                                        string record = sfdcObject.CreateNewRecord(interactionId, data.CreateRecordFieldData, output.ObjectName);
                                        if (record != null)
                                        {
                                            if (this.leadOptions.CanCreateLogForNewRecordCreate)
                                            {
                                                if (data.ActivityLogData != null)
                                                    this.sfdcObject.CreateActivityLog(interactionId, data.ActivityLogData, data.ObjectName, record);
                                                else
                                                    this.logger.Warn("Can not create Activity Log  for " + output.ObjectName + " because data is null");
                                            }
                                            popup.PopupUrl = record;
                                            this.SendPopupData(interactionId, popup);
                                        }
                                        else
                                        {
                                            this.logger.Warn("Can not popup New " + data.ObjectName + "Record because Null Returned while creating New Record");
                                        }
                                    }
                                    else
                                    {
                                        this.logger.Warn("Can not create New " + data.ObjectName + " Record because data is null");
                                    }
                                }
                                else if (data.NoRecordFound == "searchpage")
                                {
                                    popup.InteractionId = interactionId;
                                    popup.PopupUrl = this.generalOptions.SearchPageUrl + this.leadOptions.SearchPageMode + "str=" + output.SearchData;
                                    this.SendPopupData(interactionId, popup);
                                }
                                else if (data.NoRecordFound.ToLower() == "none" && data.ActivityLogData != null)
                                {
                                    if (CheckCanCreateNoRecordforNone(data, calltype))
                                    {
                                        userLevelLog = this.sfdcObject.CreateActivityLog(interactionId, data.ActivityLogData, data.ObjectName);
                                        if (data.CanPopupNoRecordActivityLog && !string.IsNullOrWhiteSpace(userLevelLog))
                                        {
                                            ActivityPopup.PopupUrl = userLevelLog;
                                            this.SendPopupData(interactionId + "NM", ActivityPopup);
                                        }
                                    }
                                }

                                #endregion Lead Process
                            }
                            else
                            {
                                this.logger.Warn("No Record found scenario can not be invoked because search data is empty");
                            }
                            break;

                        case "Account":
                            if (output.SearchRecord != null)
                            {
                                #region Account Process

                                if (output.SearchRecord.Length > 1)
                                {
                                    if (data.MultipleMatchRecord == "searchpage")
                                    {
                                        popup.InteractionId = interactionId;
                                        //Multi match user level activity log creation
                                        if (data.CanCreateMultiMatchActivityLog && data.ActivityLogData != null)
                                        {
                                            userLevelLog = this.sfdcObject.CreateActivityLog(interactionId, data.ActivityLogData, data.ObjectName);
                                            if (data.CanPopupMultiMatchActivityLog && !string.IsNullOrWhiteSpace(userLevelLog))
                                            {
                                                ActivityPopup.PopupUrl = userLevelLog;
                                                this.SendPopupData(interactionId + "AMM", ActivityPopup);
                                            }
                                        }
                                        popup.PopupUrl = this.generalOptions.SearchPageUrl + this.accountOptions.SearchPageMode + "str=" + output.SearchData;
                                        this.SendPopupData(interactionId, popup);
                                    }
                                    else if (data.MultipleMatchRecord == "openall")
                                    {
                                        popup.InteractionId = interactionId;
                                        popup.PopupUrl = GetSearchPageIds(output.SearchRecord);
                                        this.SendPopupData(interactionId, popup);
                                    }
                                    else
                                        this.logger.Info("Multimatch Action for Account objects is : None ");
                                }
                                else
                                {
                                    popup.InteractionId = interactionId;
                                    popup.PopupUrl = output.SearchRecord[0].record.Id;
                                    if (data.ActivityLogData != null)
                                    {
                                        this.sfdcObject.CreateActivityLog(interactionId, data.ActivityLogData, data.ObjectName, output.SearchRecord[0].record.Id);
                                    }
                                    popup.ObjectName = "Account";
                                    this.SendPopupData(interactionId, popup);
                                }
                            }
                            else if (!string.IsNullOrEmpty(output.SearchData))
                            {
                                if (data.NoRecordFound == "opennew")
                                {
                                    popup.InteractionId = interactionId;
                                    //no record found user level activity log creation
                                    if (data.CanCreateNoRecordActivityLog && data.ActivityLogData != null)
                                    {
                                        userLevelLog = this.sfdcObject.CreateActivityLog(interactionId, data.ActivityLogData, data.ObjectName);
                                        if (data.CanPopupNoRecordActivityLog && !string.IsNullOrWhiteSpace(userLevelLog))
                                        {
                                            ActivityPopup.PopupUrl = userLevelLog;
                                            this.SendPopupData(interactionId + "ANM", ActivityPopup);
                                        }
                                    }
                                    popup.PopupUrl = GetNewPageUrl(data.SearchData, "001", this.accountOptions.NewrecordFieldIds);
                                    this.SendPopupData(interactionId, popup);
                                }
                                else if (data.NoRecordFound == "createnew")
                                {
                                    popup.InteractionId = interactionId;
                                    popup.ObjectName = output.ObjectName;
                                    if (data.CreateRecordFieldData != null)
                                    {
                                        string record = sfdcObject.CreateNewRecord(interactionId, data.CreateRecordFieldData, output.ObjectName);
                                        if (record != null)
                                        {
                                            if (this.accountOptions.CanCreateLogForNewRecordCreate)
                                            {
                                                if (data.ActivityLogData != null)
                                                    this.sfdcObject.CreateActivityLog(interactionId, data.ActivityLogData, data.ObjectName, record);
                                                else
                                                    this.logger.Warn("Can not create Activity Log  for " + output.ObjectName + " because data is null");
                                            }
                                            popup.PopupUrl = record;
                                            this.SendPopupData(interactionId, popup);
                                        }
                                        else
                                        {
                                            this.logger.Warn("Can not popup New " + data.ObjectName + "Record because Null Returned while creating New Record");
                                        }
                                    }
                                    else
                                    {
                                        this.logger.Warn("Can not create New " + data.ObjectName + " Record because data is null");
                                    }
                                }
                                else if (data.NoRecordFound == "searchpage")
                                {
                                    popup.InteractionId = interactionId;
                                    popup.PopupUrl = this.generalOptions.SearchPageUrl + this.accountOptions.SearchPageMode + "str=" + output.SearchData;
                                    this.SendPopupData(interactionId, popup);
                                }
                                else if (data.NoRecordFound.ToLower() == "none" && data.ActivityLogData != null)
                                {
                                    if (CheckCanCreateNoRecordforNone(data, calltype))
                                    {
                                        userLevelLog = this.sfdcObject.CreateActivityLog(interactionId, data.ActivityLogData, data.ObjectName);
                                        if (data.CanPopupNoRecordActivityLog && !string.IsNullOrWhiteSpace(userLevelLog))
                                        {
                                            ActivityPopup.PopupUrl = userLevelLog;
                                            this.SendPopupData(interactionId + "NM", ActivityPopup);
                                        }
                                    }
                                }

                                #endregion Account Process
                            }
                            else
                            {
                                this.logger.Warn("No Record found scenario can not be invoked because search data is empty");
                            }
                            break;

                        case "Case":
                            if (output.SearchRecord != null)
                            {
                                #region Case Process

                                if (output.SearchRecord.Length > 1)
                                {
                                    if (data.MultipleMatchRecord == "searchpage")
                                    {
                                        popup.InteractionId = interactionId;
                                        //Multi match user level activity log creation
                                        if (data.CanCreateMultiMatchActivityLog && data.ActivityLogData != null)
                                        {
                                            userLevelLog = this.sfdcObject.CreateActivityLog(interactionId, data.ActivityLogData, data.ObjectName);
                                            if (data.CanPopupMultiMatchActivityLog && !string.IsNullOrWhiteSpace(userLevelLog))
                                            {
                                                ActivityPopup.PopupUrl = userLevelLog;
                                                this.SendPopupData(interactionId + "CaMM", ActivityPopup);
                                            }
                                        }
                                        popup.PopupUrl = this.generalOptions.SearchPageUrl + this.caseOptions.SearchPageMode + "str=" + output.SearchData;
                                        this.SendPopupData(interactionId, popup);
                                    }
                                    else if (data.MultipleMatchRecord == "openall")
                                    {
                                        popup.InteractionId = interactionId;
                                        popup.PopupUrl = GetSearchPageIds(output.SearchRecord);
                                        this.SendPopupData(interactionId, popup);
                                    }
                                    else
                                        this.logger.Info("Multimatch Action for Case objects is : None ");
                                }
                                else
                                {
                                    popup.InteractionId = interactionId;
                                    popup.PopupUrl = output.SearchRecord[0].record.Id;
                                    if (data.ActivityLogData != null)
                                    {
                                        this.sfdcObject.CreateActivityLog(interactionId, data.ActivityLogData, data.ObjectName, output.SearchRecord[0].record.Id);
                                    }
                                    popup.ObjectName = "Case";
                                    this.SendPopupData(interactionId, popup);
                                }
                            }
                            else if (!string.IsNullOrEmpty(output.SearchData))
                            {
                                if (data.NoRecordFound == "opennew")
                                {
                                    popup.InteractionId = interactionId;
                                    //no record found user level activity log creation
                                    if (data.CanCreateNoRecordActivityLog && data.ActivityLogData != null)
                                    {
                                        userLevelLog = this.sfdcObject.CreateActivityLog(interactionId, data.ActivityLogData, data.ObjectName);
                                        if (data.CanPopupNoRecordActivityLog && !string.IsNullOrWhiteSpace(userLevelLog))
                                        {
                                            ActivityPopup.PopupUrl = userLevelLog;
                                            this.SendPopupData(interactionId + "CaNM", ActivityPopup);
                                        }
                                    }
                                    popup.PopupUrl = GetNewPageUrl(data.SearchData, "500", this.caseOptions.NewrecordFieldIds);
                                    this.SendPopupData(interactionId, popup);
                                }
                                else if (data.NoRecordFound == "createnew")
                                {
                                    popup.InteractionId = interactionId;
                                    popup.ObjectName = output.ObjectName;
                                    if (data.CreateRecordFieldData != null)
                                    {
                                        string record = sfdcObject.CreateNewRecord(interactionId, data.CreateRecordFieldData, output.ObjectName);
                                        if (record != null)
                                        {
                                            if (this.caseOptions.CanCreateLogForNewRecordCreate)
                                            {
                                                if (data.ActivityLogData != null)
                                                    this.sfdcObject.CreateActivityLog(interactionId, data.ActivityLogData, data.ObjectName, record);
                                                else
                                                    this.logger.Warn("Can not create Activity Log  for " + output.ObjectName + " because data is null");
                                            }
                                            popup.PopupUrl = record;
                                            this.SendPopupData(interactionId, popup);
                                        }
                                        else
                                        {
                                            this.logger.Warn("Can not popup New " + data.ObjectName + "Record because Null Returned while creating New Record");
                                        }
                                    }
                                    else
                                    {
                                        this.logger.Warn("Can not create New " + data.ObjectName + " Record because data is null");
                                    }
                                }
                                else if (data.NoRecordFound == "searchpage")
                                {
                                    popup.InteractionId = interactionId;
                                    popup.PopupUrl = this.generalOptions.SearchPageUrl + this.caseOptions.SearchPageMode + "str=" + output.SearchData;
                                    this.SendPopupData(interactionId, popup);
                                }
                                else if (data.NoRecordFound.ToLower() == "none" && data.ActivityLogData != null)
                                {
                                    if (CheckCanCreateNoRecordforNone(data, calltype))
                                    {
                                        userLevelLog = this.sfdcObject.CreateActivityLog(interactionId, data.ActivityLogData, data.ObjectName);
                                        if (data.CanPopupNoRecordActivityLog && !string.IsNullOrWhiteSpace(userLevelLog))
                                        {
                                            ActivityPopup.PopupUrl = userLevelLog;
                                            this.SendPopupData(interactionId + "NM", ActivityPopup);
                                        }
                                    }
                                }

                                #endregion Case Process
                            }
                            else
                            {
                                this.logger.Warn("No Record found scenario can not be invoked because search data is empty");
                            }
                            break;

                        case "Contact":
                            if (output.SearchRecord != null)
                            {
                                #region Contact Process

                                if (output.SearchRecord.Length > 1)
                                {
                                    if (data.MultipleMatchRecord == "searchpage")
                                    {
                                        popup.InteractionId = interactionId;
                                        //Multi match user level activity log creation
                                        if (data.CanCreateMultiMatchActivityLog && data.ActivityLogData != null)
                                        {
                                            userLevelLog = this.sfdcObject.CreateActivityLog(interactionId, data.ActivityLogData, data.ObjectName);
                                            if (data.CanPopupMultiMatchActivityLog && !string.IsNullOrWhiteSpace(userLevelLog))
                                            {
                                                ActivityPopup.PopupUrl = userLevelLog;
                                                this.SendPopupData(interactionId + "CMM", ActivityPopup);
                                            }
                                        }
                                        popup.PopupUrl = this.generalOptions.SearchPageUrl + this.contactOptions.SearchPageMode + "str=" + output.SearchData;
                                        this.SendPopupData(interactionId, popup);
                                    }
                                    else if (data.MultipleMatchRecord == "openall")
                                    {
                                        popup.InteractionId = interactionId;
                                        popup.PopupUrl = GetSearchPageIds(output.SearchRecord);
                                        this.SendPopupData(interactionId, popup);
                                    }
                                    else
                                        this.logger.Info("Multimatch Action for Contact objects is : None ");
                                }
                                else
                                {
                                    popup.InteractionId = interactionId;
                                    popup.PopupUrl = output.SearchRecord[0].record.Id;
                                    if (data.ActivityLogData != null)
                                    {
                                        this.sfdcObject.CreateActivityLog(interactionId, data.ActivityLogData, data.ObjectName, output.SearchRecord[0].record.Id);
                                    }
                                    popup.ObjectName = "Contact";
                                    this.SendPopupData(interactionId, popup);
                                }
                            }
                            else if (!string.IsNullOrEmpty(output.SearchData))
                            {
                                if (data.NoRecordFound == "opennew")
                                {
                                    popup.InteractionId = interactionId;
                                    //no record found user level activity log creation
                                    if (data.CanCreateNoRecordActivityLog && data.ActivityLogData != null)
                                    {
                                        userLevelLog = this.sfdcObject.CreateActivityLog(interactionId, data.ActivityLogData, data.ObjectName);
                                        if (data.CanPopupNoRecordActivityLog && !string.IsNullOrWhiteSpace(userLevelLog))
                                        {
                                            ActivityPopup.PopupUrl = userLevelLog;
                                            this.SendPopupData(interactionId + "CNM", ActivityPopup);
                                        }
                                    }
                                    popup.PopupUrl = GetNewPageUrl(data.SearchData, "003", this.contactOptions.NewrecordFieldIds);
                                    this.SendPopupData(interactionId, popup);
                                }
                                else if (data.NoRecordFound == "createnew")
                                {
                                    popup.InteractionId = interactionId;
                                    popup.ObjectName = output.ObjectName;
                                    if (data.CreateRecordFieldData != null)
                                    {
                                        string record = sfdcObject.CreateNewRecord(interactionId, data.CreateRecordFieldData, output.ObjectName);
                                        if (record != null)
                                        {
                                            if (this.contactOptions.CanCreateLogForNewRecordCreate)
                                            {
                                                if (data.ActivityLogData != null)
                                                    this.sfdcObject.CreateActivityLog(interactionId, data.ActivityLogData, data.ObjectName, record);
                                                else
                                                    this.logger.Warn("Can not create Activity Log  for " + output.ObjectName + " because data is null");
                                            }
                                            popup.PopupUrl = record;
                                            this.SendPopupData(interactionId, popup);
                                        }
                                        else
                                        {
                                            this.logger.Warn("Can not popup New " + data.ObjectName + "Record because Null Returned while creating New Record");
                                        }
                                    }
                                    else
                                    {
                                        this.logger.Warn("Can not create New " + data.ObjectName + " Record because data is null");
                                    }
                                }
                                else if (data.NoRecordFound == "searchpage")
                                {
                                    popup.InteractionId = interactionId;
                                    popup.PopupUrl = this.generalOptions.SearchPageUrl + this.contactOptions.SearchPageMode + "str=" + output.SearchData;
                                    this.SendPopupData(interactionId, popup);
                                }
                                else if (data.NoRecordFound.ToLower() == "none" && data.ActivityLogData != null)
                                {
                                    if (CheckCanCreateNoRecordforNone(data, calltype))
                                    {
                                        userLevelLog = this.sfdcObject.CreateActivityLog(interactionId, data.ActivityLogData, data.ObjectName);
                                        if (data.CanPopupNoRecordActivityLog && !string.IsNullOrWhiteSpace(userLevelLog))
                                        {
                                            ActivityPopup.PopupUrl = userLevelLog;
                                            this.SendPopupData(interactionId + "NM", ActivityPopup);
                                        }
                                    }
                                }

                                #endregion Contact Process
                            }
                            else
                            {
                                this.logger.Warn("No Record found scenario can not be invoked because search data is empty");
                            }
                            break;

                        case "Opportunity":
                            if (output.SearchRecord != null)
                            {
                                #region Opportunity Process

                                if (output.SearchRecord.Length > 1)
                                {
                                    if (data.MultipleMatchRecord == "searchpage")
                                    {
                                        popup.InteractionId = interactionId;
                                        //multi match user level activity log creation
                                        if (data.CanCreateMultiMatchActivityLog && data.ActivityLogData != null)
                                        {
                                            userLevelLog = this.sfdcObject.CreateActivityLog(interactionId, data.ActivityLogData, data.ObjectName);
                                            if (data.CanPopupMultiMatchActivityLog && !string.IsNullOrWhiteSpace(userLevelLog))
                                            {
                                                ActivityPopup.PopupUrl = userLevelLog;
                                                this.SendPopupData(interactionId + "OMM", ActivityPopup);
                                            }
                                        }
                                        popup.PopupUrl = this.generalOptions.SearchPageUrl + this.opportunityOptions.SearchPageMode + "str=" + output.SearchData;
                                        this.SendPopupData(interactionId, popup);
                                    }
                                    else if (data.MultipleMatchRecord == "openall")
                                    {
                                        popup.InteractionId = interactionId;
                                        popup.PopupUrl = GetSearchPageIds(output.SearchRecord);
                                        this.SendPopupData(interactionId, popup);
                                    }
                                    else
                                        this.logger.Info("Multimatch Action for Opportunity objects is : None ");
                                }
                                else
                                {
                                    popup.InteractionId = interactionId;
                                    popup.PopupUrl = output.SearchRecord[0].record.Id;
                                    if (data.ActivityLogData != null)
                                    {
                                        this.sfdcObject.CreateActivityLog(interactionId, data.ActivityLogData, data.ObjectName, output.SearchRecord[0].record.Id);
                                    }
                                    popup.ObjectName = "Opportunity";
                                    this.SendPopupData(interactionId, popup);
                                }
                            }
                            else if (!string.IsNullOrEmpty(output.SearchData))
                            {
                                if (data.NoRecordFound == "opennew")
                                {
                                    popup.InteractionId = interactionId;
                                    //No record found user level activity log creation
                                    if (data.CanCreateNoRecordActivityLog && data.ActivityLogData != null)
                                    {
                                        userLevelLog = this.sfdcObject.CreateActivityLog(interactionId, data.ActivityLogData, data.ObjectName);
                                        if (data.CanPopupNoRecordActivityLog && !string.IsNullOrWhiteSpace(userLevelLog))
                                        {
                                            ActivityPopup.PopupUrl = userLevelLog;
                                            this.SendPopupData(interactionId + "OMM", ActivityPopup);
                                        }
                                    }
                                    popup.PopupUrl = GetNewPageUrl(data.SearchData, "006", this.opportunityOptions.NewrecordFieldIds);
                                    this.SendPopupData(interactionId, popup);
                                }
                                else if (data.NoRecordFound == "createnew")
                                {
                                    popup.InteractionId = interactionId;
                                    popup.ObjectName = output.ObjectName;
                                    if (data.CreateRecordFieldData != null)
                                    {
                                        string record = sfdcObject.CreateNewRecord(interactionId, data.CreateRecordFieldData, output.ObjectName);
                                        if (record != null)
                                        {
                                            if (this.opportunityOptions.CanCreateLogForNewRecordCreate)
                                            {
                                                if (data.ActivityLogData != null)
                                                    this.sfdcObject.CreateActivityLog(interactionId, data.ActivityLogData, data.ObjectName, record);
                                                else
                                                    this.logger.Warn("Can not create Activity Log  for " + output.ObjectName + " because data is null");
                                            }
                                            popup.PopupUrl = record;
                                            this.SendPopupData(interactionId, popup);
                                        }
                                        else
                                        {
                                            this.logger.Warn("Can not popup New " + data.ObjectName + "Record because Null Returned while creating New Record");
                                        }
                                    }
                                    else
                                    {
                                        this.logger.Warn("Can not create New " + data.ObjectName + " Record because data is null");
                                    }
                                }
                                else if (data.NoRecordFound == "searchpage")
                                {
                                    popup.InteractionId = interactionId;
                                    popup.PopupUrl = this.generalOptions.SearchPageUrl + this.opportunityOptions.SearchPageMode + "str=" + output.SearchData;
                                    this.SendPopupData(interactionId, popup);
                                }
                                else if (data.NoRecordFound.ToLower() == "none" && data.ActivityLogData != null)
                                {
                                    if (CheckCanCreateNoRecordforNone(data, calltype))
                                    {
                                        userLevelLog = this.sfdcObject.CreateActivityLog(interactionId, data.ActivityLogData, data.ObjectName);
                                        if (data.CanPopupNoRecordActivityLog && !string.IsNullOrWhiteSpace(userLevelLog))
                                        {
                                            ActivityPopup.PopupUrl = userLevelLog;
                                            this.SendPopupData(interactionId + "NM", ActivityPopup);
                                        }
                                    }
                                }

                                #endregion Opportunity Process
                            }
                            else
                            {
                                this.logger.Warn("No Record found scenario can not be invoked because search data is empty");
                            }
                            break;

                        default:
                            if (output.ObjectName.Contains("__c"))
                            {
                                if (Settings.CustomObjectNames.ContainsKey(output.ObjectName))
                                {
                                    VoiceOptions option = this.customObjectOptions[Settings.CustomObjectNames[output.ObjectName]];
                                    if (option != null)
                                    {
                                        if (output.SearchRecord != null)
                                        {
                                            if (output.SearchRecord.Length > 1)
                                            {
                                                if (data.MultipleMatchRecord == "searchpage")
                                                {
                                                    popup.InteractionId = interactionId;
                                                    //Multi match user level activity log creation
                                                    if (data.CanCreateMultiMatchActivityLog && data.ActivityLogData != null)
                                                    {
                                                        userLevelLog = this.sfdcObject.CreateActivityLog(interactionId, data.ActivityLogData, data.ObjectName);
                                                        if (data.CanPopupMultiMatchActivityLog && !string.IsNullOrWhiteSpace(userLevelLog))
                                                        {
                                                            ActivityPopup.PopupUrl = userLevelLog;
                                                            this.SendPopupData(interactionId + data.ObjectName.Substring(0, 2) + "MM", ActivityPopup);
                                                        }
                                                    }
                                                    popup.PopupUrl = this.generalOptions.SearchPageUrl + option.SearchPageMode + "str=" + output.SearchData;
                                                    this.SendPopupData(interactionId, popup);
                                                }
                                                else if (data.MultipleMatchRecord == "openall")
                                                {
                                                    popup.InteractionId = interactionId;
                                                    popup.PopupUrl = GetSearchPageIds(output.SearchRecord);
                                                    this.SendPopupData(interactionId, popup);
                                                }
                                                else
                                                    this.logger.Info("Multimatch Action for Opportunity objects is : None ");
                                            }
                                            else
                                            {
                                                popup.InteractionId = interactionId;
                                                popup.PopupUrl = output.SearchRecord[0].record.Id;
                                                if (data.ActivityLogData != null)
                                                {
                                                    this.sfdcObject.CreateActivityLog(interactionId, data.ActivityLogData, data.ObjectName, output.SearchRecord[0].record.Id);
                                                }
                                                popup.ObjectName = data.ObjectName;
                                                this.SendPopupData(interactionId, popup);
                                            }
                                        }
                                        else
                                        {
                                            if (data.NoRecordFound == "opennew")
                                            {
                                                popup.InteractionId = interactionId;
                                                //no record found user level activity log creation
                                                if (data.CanCreateNoRecordActivityLog && data.ActivityLogData != null)
                                                {
                                                    userLevelLog = this.sfdcObject.CreateActivityLog(interactionId, data.ActivityLogData, data.ObjectName);
                                                    if (data.CanPopupNoRecordActivityLog && !string.IsNullOrWhiteSpace(userLevelLog))
                                                    {
                                                        ActivityPopup.PopupUrl = userLevelLog;
                                                        this.SendPopupData(interactionId + data.ObjectName.Substring(0, 2) + "NM", ActivityPopup);
                                                    }
                                                }
                                                popup.PopupUrl = GetNewPageUrl(data.SearchData, option.CustomObjectURL, option.NewrecordFieldIds);
                                                this.SendPopupData(interactionId, popup);
                                            }
                                            else if (data.NoRecordFound == "createnew")
                                            {
                                                popup.InteractionId = interactionId;
                                                popup.ObjectName = output.ObjectName;
                                                if (data.CreateRecordFieldData != null)
                                                {
                                                    string record = sfdcObject.CreateNewRecord(interactionId, data.CreateRecordFieldData, output.ObjectName);
                                                    if (record != null)
                                                    {
                                                        if (option.CanCreateLogForNewRecordCreate)
                                                        {
                                                            if (data.ActivityLogData != null)
                                                                this.sfdcObject.CreateActivityLog(interactionId, data.ActivityLogData, data.ObjectName, record);
                                                            else
                                                                this.logger.Warn("Can not create Activity Log  for " + output.ObjectName + " because data is null");
                                                        }
                                                        popup.PopupUrl = record;
                                                        this.SendPopupData(interactionId, popup);
                                                    }
                                                    else
                                                    {
                                                        this.logger.Warn("Can not popup New " + data.ObjectName + "Record because Null Returned while creating New Record");
                                                    }
                                                }
                                                else
                                                {
                                                    this.logger.Warn("Can not create New " + data.ObjectName + " Record because data is null");
                                                }
                                            }
                                            else if (data.NoRecordFound == "searchpage")
                                            {
                                                popup.InteractionId = interactionId;
                                                popup.PopupUrl = this.generalOptions.SearchPageUrl + option.SearchPageMode + "str=" + output.SearchData;
                                                this.SendPopupData(interactionId, popup);
                                            }
                                            else if (data.NoRecordFound.ToLower() == "none" && data.ActivityLogData != null)
                                            {
                                                if (CheckCanCreateNoRecordforNone(data, calltype))
                                                {
                                                    userLevelLog = this.sfdcObject.CreateActivityLog(interactionId, data.ActivityLogData, data.ObjectName);
                                                    if (data.CanPopupNoRecordActivityLog && !string.IsNullOrWhiteSpace(userLevelLog))
                                                    {
                                                        ActivityPopup.PopupUrl = userLevelLog;
                                                        this.SendPopupData(interactionId + "NM", ActivityPopup);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            break;
                    }
                }
                else
                {
                    this.logger.Error("ProcessSFDCResponse: Cannot popup records because SFDC Search method returned null ");
                }
            }
            catch (Exception generalException)
            {
                this.logger.Error("ProcessSFDCResponse: Error Occurred :" + generalException.ToString());
            }
        }

        private void ProcessSFDCResponseForCommonSearch(OutputValues outputValues, SFDCData sfdcData, string interactionId, SFDCCallType calltype)
        {
            try
            {
                this.logger.Info("ProcessSFDCResponse: CommonSearch based Popup");
                this.logger.Info("ProcessSFDCResponse: InteractionId : " + interactionId);
                string multimatchURL = string.Empty;
                string openAllIds = string.Empty;
                string SearchPageObjectName = string.Empty;
                string userLevelLog = string.Empty;
                bool canCreateMultiMatchLog = false;
                bool canPopupMultiMatchLog = false;
                bool canCreateNoRecordLog = false;
                bool canPopupNoRecordLog = false;
                if (outputValues != null)
                {
                    this.logger.Info("ProcessSFDCResponse: Data received from SFDC for CommonSearch : " + outputValues.ToString());
                    if (outputValues.SearchRecord != null)
                    {
                        KeyValueCollection RecordIds = GetRecordIdsFromRecords(outputValues.SearchRecord);
                        if (RecordIds != null && RecordIds.Count > 0)
                        {
                            bool IsExactMatchFound = false;
                            if (generalOptions.MultiMatchAction == MultiMatchBehaviour.SearchPage && RecordIds.Count > 1)
                            {
                                //pouup search page
                                PopupData popup = new PopupData();
                                popup.InteractionId = interactionId;
                                popup.PopupUrl = this.generalOptions.SearchPageUrl + "&str=" + outputValues.SearchData;
                                SendPopupData(interactionId, popup);
                                //create profile level activity
                                string profileActivittyID = sfdcObject.CreateActivityLog(interactionId, sfdcData.ProfileActivityLogData, "profileactivity");
                                if (profileActivittyID != null)
                                {
                                    if (this.generalOptions.CanEnableMultiMatchProfileActivityPopup)
                                    {
                                        //popup Activity
                                        PopupData ProfilePopupData = new PopupData
                                        {
                                            InteractionId = interactionId,
                                            ObjectName = "profileactivity",
                                            PopupUrl = profileActivittyID
                                        };
                                        SendPopupData(interactionId + "NMMM", ProfilePopupData);
                                    }
                                }
                                return;
                            }
                            else if (generalOptions.MultiMatchAction == MultiMatchBehaviour.ExactMatchPopup)
                            {
                                IsExactMatchFound = CheckForExactMatchRecordFound(RecordIds);
                            }
                            foreach (string key in RecordIds.AllKeys)
                            {
                                userLevelLog = string.Empty;
                                switch (key)
                                {
                                    case "Lead":
                                        logger.Info("ProcessSFDCResponse : Collecting Inbound Voice Popup data for Lead");

                                        #region Lead Process

                                        if (sfdcData.LeadData != null)
                                        {
                                            PopupData leadPopup = new PopupData();
                                            if (RecordIds.ContainsKey(key))
                                            {
                                                if (RecordIds[key].ToString().Contains(","))
                                                {
                                                    if (!IsExactMatchFound)
                                                    {
                                                        SearchPageObjectName = sfdcData.LeadData.ObjectName;
                                                        if (sfdcData.LeadData.MultipleMatchRecord.Equals("searchpage"))
                                                        {
                                                            multimatchURL += sfdcData.LeadData.SearchpageMode;
                                                            //Multi Match create user level activity
                                                            if (!canCreateMultiMatchLog)
                                                                canCreateMultiMatchLog = sfdcData.LeadData.CanCreateMultiMatchActivityLog;
                                                            if (!canPopupMultiMatchLog)
                                                                canPopupMultiMatchLog = sfdcData.LeadData.CanPopupMultiMatchActivityLog;
                                                        }
                                                        else if (sfdcData.LeadData.MultipleMatchRecord.Equals("openall"))
                                                            openAllIds += Convert.ToString(RecordIds[key]) + ",";
                                                        else if (sfdcData.LeadData.MultipleMatchRecord.Equals("none"))
                                                            logger.Info("ProcessSFDCResponse : Lead multiple match record action is none");
                                                    }
                                                    else
                                                        this.logger.Info("Multi-Match popup is disabled since exact match found in one/more common search objects");
                                                }
                                                else
                                                {
                                                    leadPopup.InteractionId = interactionId;
                                                    leadPopup.ObjectName = sfdcData.LeadData.ObjectName;
                                                    if (sfdcData.LeadData.ActivityLogData != null)
                                                        this.sfdcObject.CreateActivityLog(interactionId, sfdcData.LeadData.ActivityLogData, sfdcData.LeadData.ObjectName, Convert.ToString(RecordIds[key]));
                                                    else
                                                        this.logger.Info("ProcessSFDCResponse: Activity Log is null for the Lead object");

                                                    leadPopup.PopupUrl = Convert.ToString(RecordIds[key]);
                                                    SendPopupData(interactionId + "leadpopup", leadPopup);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            logger.Warn("CommonPopup : Popup data for lead is null");
                                        }

                                        #endregion Lead Process

                                        break;

                                    case "Contact":
                                        logger.Info("ProcessSFDCResponse : Collecting Inbound Voice Popup data for contact");

                                        #region Contact Process

                                        if (sfdcData.ContactData != null)
                                        {
                                            PopupData contactPopup = new PopupData();
                                            if (RecordIds.ContainsKey(key))
                                            {
                                                if (RecordIds[key].ToString().Contains(","))
                                                {
                                                    if (!IsExactMatchFound)
                                                    {
                                                        SearchPageObjectName = sfdcData.ContactData.ObjectName;
                                                        if (sfdcData.ContactData.MultipleMatchRecord.Equals("searchpage"))
                                                        {
                                                            multimatchURL += sfdcData.ContactData.SearchpageMode;
                                                            //Multi Match create user level activity
                                                            if (!canCreateMultiMatchLog)
                                                                canCreateMultiMatchLog = sfdcData.ContactData.CanCreateMultiMatchActivityLog;
                                                            if (!canPopupMultiMatchLog)
                                                                canPopupMultiMatchLog = sfdcData.ContactData.CanPopupMultiMatchActivityLog;
                                                        }
                                                        else if (sfdcData.ContactData.MultipleMatchRecord.Equals("openall"))
                                                            openAllIds += Convert.ToString(RecordIds[key]) + ",";
                                                        else if (sfdcData.ContactData.MultipleMatchRecord.Equals("none"))
                                                            logger.Info("ProcessSFDCResponse : Contact multiple match record action is none");
                                                    }
                                                    else
                                                        this.logger.Info("Multi-Match popup is disabled since exact match found in one/more common search objects");
                                                }
                                                else
                                                {
                                                    contactPopup.InteractionId = interactionId;
                                                    contactPopup.ObjectName = sfdcData.ContactData.ObjectName;
                                                    if (sfdcData.ContactData.ActivityLogData != null)
                                                        this.sfdcObject.CreateActivityLog(interactionId, sfdcData.ContactData.ActivityLogData, sfdcData.ContactData.ObjectName, Convert.ToString(RecordIds[key]));
                                                    else
                                                        this.logger.Info("ProcessSFDCResponse: Activity Log is null for the Contact object");

                                                    contactPopup.PopupUrl = Convert.ToString(RecordIds[key]);
                                                    SendPopupData(interactionId + "contactpopup", contactPopup);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            logger.Warn("CommonPopup : Popup data for Contact is null");
                                        }

                                        #endregion Contact Process

                                        break;

                                    case "Account":
                                        logger.Info("ProcessSFDCResponse : Collecting Inbound Voice Popup data for account");

                                        #region Account Process

                                        if (sfdcData.AccountData != null)
                                        {
                                            PopupData accountPopup = new PopupData();
                                            if (RecordIds.ContainsKey(key))
                                            {
                                                if (RecordIds[key].ToString().Contains(","))
                                                {
                                                    if (!IsExactMatchFound)
                                                    {
                                                        SearchPageObjectName = sfdcData.AccountData.ObjectName;
                                                        if (sfdcData.AccountData.MultipleMatchRecord.Equals("searchpage"))
                                                        {
                                                            multimatchURL += sfdcData.AccountData.SearchpageMode;
                                                            //Multi Match create user level activity
                                                            if (!canCreateMultiMatchLog)
                                                                canCreateMultiMatchLog = sfdcData.AccountData.CanCreateMultiMatchActivityLog;
                                                            if (!canPopupMultiMatchLog)
                                                                canPopupMultiMatchLog = sfdcData.AccountData.CanPopupMultiMatchActivityLog;
                                                        }
                                                        else if (sfdcData.AccountData.MultipleMatchRecord.Equals("openall"))
                                                            openAllIds += Convert.ToString(RecordIds[key]) + ",";
                                                        else if (sfdcData.AccountData.MultipleMatchRecord.Equals("none"))
                                                            logger.Info("ProcessSFDCResponse : Account multiple match record action is none");
                                                    }
                                                    else
                                                        this.logger.Info("Multi-Match popup is disabled since exact match found in one/more common search objects");
                                                }
                                                else
                                                {
                                                    accountPopup.InteractionId = interactionId;
                                                    accountPopup.ObjectName = sfdcData.AccountData.ObjectName;
                                                    if (sfdcData.AccountData.ActivityLogData != null)
                                                        this.sfdcObject.CreateActivityLog(interactionId, sfdcData.AccountData.ActivityLogData, sfdcData.AccountData.ObjectName, Convert.ToString(RecordIds[key]));
                                                    else
                                                        this.logger.Info("ProcessSFDCResponse: Activity Log is null for the Account object");

                                                    accountPopup.PopupUrl = Convert.ToString(RecordIds[key]);
                                                    SendPopupData(interactionId + "accountpopup", accountPopup);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            logger.Warn("CommonPopup : Popup data for Account is null");
                                        }

                                        #endregion Account Process

                                        break;

                                    case "Case":
                                        logger.Info("ProcessSFDCResponse : Collecting Inbound Voice Popup data for case");

                                        #region Case Process

                                        if (sfdcData.CaseData != null)
                                        {
                                            PopupData casePopup = new PopupData();
                                            if (RecordIds.ContainsKey(key))
                                            {
                                                if (RecordIds[key].ToString().Contains(","))
                                                {
                                                    if (!IsExactMatchFound)
                                                    {
                                                        SearchPageObjectName = sfdcData.CaseData.ObjectName;
                                                        if (sfdcData.CaseData.MultipleMatchRecord.Equals("searchpage"))
                                                        {
                                                            multimatchURL += sfdcData.CaseData.SearchpageMode;
                                                            //Multi Match create user level activity
                                                            if (!canCreateMultiMatchLog)
                                                                canCreateMultiMatchLog = sfdcData.CaseData.CanCreateMultiMatchActivityLog;
                                                            if (!canPopupMultiMatchLog)
                                                                canPopupMultiMatchLog = sfdcData.CaseData.CanPopupMultiMatchActivityLog;
                                                        }
                                                        else if (sfdcData.CaseData.MultipleMatchRecord.Equals("openall"))
                                                            openAllIds += Convert.ToString(RecordIds[key]) + ",";
                                                        else if (sfdcData.CaseData.MultipleMatchRecord.Equals("none"))
                                                            logger.Info("ProcessSFDCResponse : Case multiple match record action is none");
                                                    }
                                                    else
                                                        this.logger.Info("Multi-Match popup is disabled since exact match found in one/more common search objects");
                                                }
                                                else
                                                {
                                                    casePopup.InteractionId = interactionId;
                                                    casePopup.ObjectName = sfdcData.CaseData.ObjectName;
                                                    if (sfdcData.CaseData.ActivityLogData != null)
                                                        this.sfdcObject.CreateActivityLog(interactionId, sfdcData.CaseData.ActivityLogData, sfdcData.CaseData.ObjectName, Convert.ToString(RecordIds[key]));
                                                    else
                                                        this.logger.Info("ProcessSFDCResponse: Activity Log is null for the Case object");

                                                    casePopup.PopupUrl = Convert.ToString(RecordIds[key]);
                                                    SendPopupData(interactionId + "casepopup", casePopup);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            logger.Warn("CommonPopup : Popup data for Case is null");
                                        }

                                        #endregion Case Process

                                        break;

                                    case "Opportunity":

                                        logger.Info("ProcessSFDCResponse : Collecting Inbound Voice Popup data for opportunity");
                                        if (sfdcData.OpportunityData != null)
                                        {
                                            #region Opportunity

                                            PopupData opportunityPopup = new PopupData();
                                            if (RecordIds.ContainsKey(key))
                                            {
                                                if (RecordIds[key].ToString().Contains(","))
                                                {
                                                    if (!IsExactMatchFound)
                                                    {
                                                        SearchPageObjectName = sfdcData.OpportunityData.ObjectName;
                                                        if (sfdcData.OpportunityData.MultipleMatchRecord.Equals("searchpage"))
                                                        {
                                                            multimatchURL += sfdcData.OpportunityData.SearchpageMode;
                                                            //Multi Match create user level activity
                                                            if (!canCreateMultiMatchLog)
                                                                canCreateMultiMatchLog = sfdcData.OpportunityData.CanCreateMultiMatchActivityLog;
                                                            if (!canPopupMultiMatchLog)
                                                                canPopupMultiMatchLog = sfdcData.OpportunityData.CanPopupMultiMatchActivityLog;
                                                        }
                                                        else if (sfdcData.OpportunityData.MultipleMatchRecord.Equals("openall"))
                                                            openAllIds += Convert.ToString(RecordIds[key]) + ",";
                                                        else if (sfdcData.OpportunityData.MultipleMatchRecord.Equals("none"))
                                                            logger.Info("ProcessSFDCResponse : Opportunity multiple match record action is none");
                                                    }
                                                    else
                                                        this.logger.Info("Multi-Match popup is disabled since exact match found in one/more common search objects");
                                                }
                                                else
                                                {
                                                    opportunityPopup.InteractionId = interactionId;
                                                    opportunityPopup.ObjectName = sfdcData.OpportunityData.ObjectName;
                                                    if (sfdcData.OpportunityData.ActivityLogData != null)
                                                        this.sfdcObject.CreateActivityLog(interactionId, sfdcData.OpportunityData.ActivityLogData, sfdcData.OpportunityData.ObjectName, Convert.ToString(RecordIds[key]));
                                                    else
                                                        this.logger.Info("ProcessSFDCResponse: Activity Log is null for the Opportunity object");

                                                    opportunityPopup.PopupUrl = Convert.ToString(RecordIds[key]);
                                                    SendPopupData(interactionId + "opportunitypopup", opportunityPopup);
                                                }
                                            }

                                            #endregion Opportunity
                                        }
                                        else
                                        {
                                            logger.Warn("ProcessSFDCResponse : Popup data for opportunity is null");
                                        }
                                        break;

                                    default:
                                        if (key.Contains("__c"))
                                        {
                                            #region CustomObject

                                            ICustomObject customData = null;
                                            string objName = string.Empty;

                                            foreach (ICustomObject data in sfdcData.CustomObjectData)
                                            {
                                                if (key == data.ObjectName)
                                                {
                                                    customData = data;
                                                    break;
                                                }
                                            }
                                            VoiceOptions options = null;
                                            if (Settings.CustomObjectNames.ContainsKey(key))
                                                options = this.customObjectOptions[Settings.CustomObjectNames[key]];

                                            if (customData != null)
                                            {
                                                PopupData customPopup = new PopupData();
                                                if (RecordIds.ContainsKey(customData.ObjectName))
                                                {
                                                    if (RecordIds[customData.ObjectName].ToString().Contains(",") && options != null)
                                                    {
                                                        if (!IsExactMatchFound)
                                                        {
                                                            SearchPageObjectName = customData.ObjectName;
                                                            if (customData.MultipleMatchRecord.Equals("searchpage"))
                                                            {
                                                                multimatchURL += customData.SearchpageMode;
                                                                //Multi Match create user level activity
                                                                if (!canCreateMultiMatchLog)
                                                                    canCreateMultiMatchLog = customData.CanCreateMultiMatchActivityLog;
                                                                if (!canPopupMultiMatchLog)
                                                                    canPopupMultiMatchLog = customData.CanPopupMultiMatchActivityLog;
                                                            }
                                                            else if (customData.MultipleMatchRecord.Equals("openall"))
                                                                openAllIds += Convert.ToString(RecordIds[customData.ObjectName]) + ",";
                                                            else if (customData.MultipleMatchRecord.Equals("none"))
                                                                logger.Info("ProcessSFDCResponse : " + options.ObjectName + " multiple match action is none");
                                                        }
                                                        else
                                                            this.logger.Info("Multi-Match popup is disabled since exact match found in one/more common search objects");
                                                    }
                                                    else
                                                    {
                                                        customPopup.InteractionId = interactionId;
                                                        customPopup.ObjectName = customData.ObjectName;
                                                        if (customData.ActivityLogData != null)
                                                            this.sfdcObject.CreateActivityLog(interactionId, customData.ActivityLogData, customData.ObjectName, Convert.ToString(RecordIds[customData.ObjectName]));
                                                        else
                                                            this.logger.Info("ProcessSFDCResponse: Activity Log is null for the " + customData.ObjectName + " object");

                                                        customPopup.PopupUrl = Convert.ToString(RecordIds[key]);
                                                        SendPopupData(interactionId + "custompopup", customPopup);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                logger.Warn("ProcessSFDCResponse : Popup data for CustomObject is null");
                                            }

                                            #endregion CustomObject
                                        }
                                        break;
                                }
                            }
                        }
                    }
                    else
                    {
                        //No Record Found Scenario for all objects

                        #region No Record Found

                        foreach (string pkey in Settings.CommonPopupObjects.Split(','))
                        {
                            switch (pkey)
                            {
                                case "Lead":
                                    if (sfdcData.LeadData != null)
                                    {
                                        #region Lead Popup

                                        PopupData leadPopup = new PopupData();
                                        if (sfdcData.LeadData.NoRecordFound == "opennew")
                                        {
                                            leadPopup.InteractionId = interactionId;
                                            leadPopup.ObjectName = sfdcData.LeadData.ObjectName;
                                            leadPopup.PopupUrl = GetNewPageUrl(sfdcData.CommonSearchData, "00Q", this.leadOptions.NewrecordFieldIds);
                                            this.SendPopupData(interactionId + "leadpp", leadPopup);
                                            //no record create user level activity
                                            if (!canCreateNoRecordLog)
                                                canCreateNoRecordLog = sfdcData.LeadData.CanCreateNoRecordActivityLog;
                                            if (!canPopupNoRecordLog)
                                                canPopupNoRecordLog = sfdcData.LeadData.CanPopupNoRecordActivityLog;
                                        }
                                        else if (sfdcData.LeadData.NoRecordFound == "createnew")
                                        {
                                            leadPopup.InteractionId = interactionId;
                                            leadPopup.ObjectName = sfdcData.LeadData.ObjectName;
                                            if (sfdcData.LeadData.CreateRecordFieldData != null)
                                            {
                                                string record = sfdcObject.CreateNewRecord(interactionId, sfdcData.LeadData.CreateRecordFieldData, sfdcData.LeadData.ObjectName);
                                                if (record != null)
                                                {
                                                    if (this.leadOptions.CanCreateLogForNewRecordCreate && sfdcData.LeadData.ActivityLogData != null)
                                                    {
                                                        this.sfdcObject.CreateActivityLog(interactionId, sfdcData.LeadData.ActivityLogData, sfdcData.LeadData.ObjectName, record);
                                                    }
                                                    leadPopup.PopupUrl = record;
                                                    this.SendPopupData(interactionId + "leadpp", leadPopup);
                                                }
                                                else
                                                {
                                                    this.logger.Warn("Can not popup New Lead Record because Null Returned while creating New Record");
                                                }
                                            }
                                            else
                                            {
                                                this.logger.Warn("Can not create New " + sfdcData.LeadData.ObjectName + " Record because data is null");
                                            }
                                        }
                                        else if (sfdcData.LeadData.NoRecordFound == "searchpage")
                                        {
                                            leadPopup.InteractionId = interactionId;
                                            leadPopup.ObjectName = sfdcData.LeadData.ObjectName;
                                            leadPopup.PopupUrl = this.generalOptions.SearchPageUrl + this.leadOptions.SearchPageMode + "str=" + outputValues.SearchData;
                                            this.SendPopupData(interactionId + "leadpp", leadPopup);
                                        }
                                        else if (sfdcData.LeadData.NoRecordFound.ToLower() == "none" && CheckCanCreateNoRecordforNone(sfdcData.LeadData, calltype))
                                        {
                                            canCreateNoRecordLog = true;
                                            if (!canPopupNoRecordLog)
                                                canPopupNoRecordLog = sfdcData.LeadData.CanPopupNoRecordActivityLog;
                                        }

                                        #endregion Lead Popup
                                    }
                                    break;

                                case "Account":
                                    if (sfdcData.AccountData != null)
                                    {
                                        #region Account Popup

                                        PopupData accountPopup = new PopupData();
                                        if (sfdcData.AccountData.NoRecordFound == "opennew")
                                        {
                                            accountPopup.InteractionId = interactionId;
                                            accountPopup.ObjectName = sfdcData.AccountData.ObjectName;
                                            accountPopup.PopupUrl = GetNewPageUrl(sfdcData.CommonSearchData, "001", this.accountOptions.NewrecordFieldIds);
                                            this.SendPopupData(interactionId + "accountpp", accountPopup);
                                            //norecord create user level activity
                                            if (!canCreateNoRecordLog)
                                                canCreateNoRecordLog = sfdcData.AccountData.CanCreateNoRecordActivityLog;
                                            if (!canPopupNoRecordLog)
                                                canPopupNoRecordLog = sfdcData.AccountData.CanPopupNoRecordActivityLog;
                                        }
                                        else if (sfdcData.AccountData.NoRecordFound == "createnew")
                                        {
                                            accountPopup.InteractionId = interactionId;
                                            accountPopup.ObjectName = sfdcData.AccountData.ObjectName;
                                            if (sfdcData.AccountData.CreateRecordFieldData != null)
                                            {
                                                string record = sfdcObject.CreateNewRecord(interactionId, sfdcData.AccountData.CreateRecordFieldData, sfdcData.AccountData.ObjectName);
                                                if (!String.IsNullOrEmpty(record))
                                                {
                                                    if (this.accountOptions.CanCreateLogForNewRecordCreate && sfdcData.AccountData.ActivityLogData != null)
                                                    {
                                                        this.sfdcObject.CreateActivityLog(interactionId, sfdcData.AccountData.ActivityLogData, sfdcData.AccountData.ObjectName, record);
                                                    }
                                                    accountPopup.PopupUrl = record;
                                                    this.SendPopupData(interactionId + "accountpp", accountPopup);
                                                }
                                                else
                                                {
                                                    this.logger.Warn("Can not popup New Account Record because Null Returned while creating New Record");
                                                }
                                            }
                                            else
                                            {
                                                this.logger.Warn("Can not create New " + sfdcData.AccountData.ObjectName + " Record because data is null");
                                            }
                                        }
                                        else if (sfdcData.AccountData.NoRecordFound == "searchpage")
                                        {
                                            accountPopup.InteractionId = interactionId;
                                            accountPopup.ObjectName = sfdcData.AccountData.ObjectName;
                                            accountPopup.PopupUrl = this.generalOptions.SearchPageUrl + this.accountOptions.SearchPageMode + "str=" + outputValues.SearchData;
                                            this.SendPopupData(interactionId + "accountpp", accountPopup);
                                        }
                                        else if (sfdcData.AccountData.NoRecordFound.ToLower() == "none" && CheckCanCreateNoRecordforNone(sfdcData.AccountData, calltype))
                                        {
                                            canCreateNoRecordLog = true;
                                            if (!canPopupNoRecordLog)
                                                canPopupNoRecordLog = sfdcData.AccountData.CanPopupNoRecordActivityLog;
                                        }

                                        #endregion Account Popup
                                    }
                                    break;

                                case "Contact":
                                    if (sfdcData.ContactData != null)
                                    {
                                        #region contact popup

                                        PopupData contactPopup = new PopupData();
                                        if (sfdcData.ContactData.NoRecordFound == "opennew")
                                        {
                                            contactPopup.InteractionId = interactionId;
                                            contactPopup.ObjectName = sfdcData.ContactData.ObjectName;
                                            contactPopup.PopupUrl = GetNewPageUrl(sfdcData.CommonSearchData, "003", this.contactOptions.NewrecordFieldIds);
                                            this.SendPopupData(interactionId + "contactpp", contactPopup);
                                            //norecord create user level activity
                                            if (!canCreateNoRecordLog)
                                                canCreateNoRecordLog = sfdcData.ContactData.CanCreateNoRecordActivityLog;
                                            if (!canPopupNoRecordLog)
                                                canPopupNoRecordLog = sfdcData.ContactData.CanPopupNoRecordActivityLog;
                                        }
                                        else if (sfdcData.ContactData.NoRecordFound == "createnew")
                                        {
                                            contactPopup.InteractionId = interactionId;
                                            contactPopup.ObjectName = sfdcData.ContactData.ObjectName;
                                            if (sfdcData.ContactData.CreateRecordFieldData != null)
                                            {
                                                string record = sfdcObject.CreateNewRecord(interactionId, sfdcData.ContactData.CreateRecordFieldData, sfdcData.ContactData.ObjectName);
                                                if (!String.IsNullOrEmpty(record))
                                                {
                                                    if (this.contactOptions.CanCreateLogForNewRecordCreate && sfdcData.ContactData.ActivityLogData != null)
                                                    {
                                                        this.sfdcObject.CreateActivityLog(interactionId, sfdcData.ContactData.ActivityLogData, sfdcData.ContactData.ObjectName, record);
                                                    }
                                                    contactPopup.PopupUrl = record;
                                                    this.SendPopupData(interactionId + "contactpp", contactPopup);
                                                }
                                                else
                                                {
                                                    this.logger.Warn("Can not popup New Contact Record because Null Returned while creating New Record");
                                                }
                                            }
                                            else
                                            {
                                                this.logger.Warn("Can not create New " + sfdcData.ContactData.ObjectName + " Record because data is null");
                                            }
                                        }
                                        else if (sfdcData.ContactData.NoRecordFound == "searchpage")
                                        {
                                            contactPopup.InteractionId = interactionId;
                                            contactPopup.ObjectName = sfdcData.ContactData.ObjectName;
                                            contactPopup.PopupUrl = this.generalOptions.SearchPageUrl + this.contactOptions.SearchPageMode + "str=" + outputValues.SearchData;
                                            this.SendPopupData(interactionId + "contactpp", contactPopup);
                                        }
                                        else if (sfdcData.ContactData.NoRecordFound.ToLower() == "none" && CheckCanCreateNoRecordforNone(sfdcData.ContactData, calltype))
                                        {
                                            canCreateNoRecordLog = true;
                                            if (!canPopupNoRecordLog)
                                                canPopupNoRecordLog = sfdcData.ContactData.CanPopupNoRecordActivityLog;
                                        }

                                        #endregion contact popup
                                    }
                                    break;

                                case "Case":
                                    if (sfdcData.CaseData != null)
                                    {
                                        #region Case Popup

                                        PopupData casePopup = new PopupData();
                                        if (sfdcData.CaseData.NoRecordFound == "opennew")
                                        {
                                            casePopup.InteractionId = interactionId;
                                            casePopup.ObjectName = sfdcData.CaseData.ObjectName;
                                            casePopup.PopupUrl = GetNewPageUrl(sfdcData.CommonSearchData, "500", this.caseOptions.NewrecordFieldIds);
                                            this.SendPopupData(interactionId + "Casepp", casePopup);
                                            //norecord create user level activity
                                            if (!canCreateNoRecordLog)
                                                canCreateNoRecordLog = sfdcData.CaseData.CanCreateNoRecordActivityLog;
                                            if (!canPopupNoRecordLog)
                                                canPopupNoRecordLog = sfdcData.CaseData.CanPopupNoRecordActivityLog;
                                        }
                                        else if (sfdcData.CaseData.NoRecordFound == "createnew")
                                        {
                                            casePopup.InteractionId = interactionId;
                                            casePopup.ObjectName = sfdcData.CaseData.ObjectName;
                                            if (sfdcData.CaseData.CreateRecordFieldData != null)
                                            {
                                                string record = sfdcObject.CreateNewRecord(interactionId, sfdcData.CaseData.CreateRecordFieldData, sfdcData.CaseData.ObjectName);
                                                if (record != null)
                                                {
                                                    if (this.caseOptions.CanCreateLogForNewRecordCreate && sfdcData.CaseData.ActivityLogData != null)
                                                    {
                                                        this.sfdcObject.CreateActivityLog(interactionId, sfdcData.CaseData.ActivityLogData, sfdcData.CaseData.ObjectName, record);
                                                    }
                                                    casePopup.PopupUrl = record;
                                                    this.SendPopupData(interactionId + "Casepp", casePopup);
                                                }
                                                else
                                                {
                                                    this.logger.Warn("Can not popup New Case Record because Null Returned while creating New Record");
                                                }
                                            }
                                            else
                                            {
                                                this.logger.Warn("Can not create New " + sfdcData.CaseData.ObjectName + " Record because data is null");
                                            }
                                        }
                                        else if (sfdcData.CaseData.NoRecordFound == "searchpage")
                                        {
                                            casePopup.InteractionId = interactionId;
                                            casePopup.ObjectName = sfdcData.CaseData.ObjectName;
                                            casePopup.PopupUrl = this.generalOptions.SearchPageUrl + this.caseOptions.SearchPageMode + "str=" + outputValues.SearchData;
                                            this.SendPopupData(interactionId + "Casepp", casePopup);
                                        }
                                        else if (sfdcData.CaseData.NoRecordFound.ToLower() == "none" && CheckCanCreateNoRecordforNone(sfdcData.CaseData, calltype))
                                        {
                                            canCreateNoRecordLog = true;
                                            if (!canPopupNoRecordLog)
                                                canPopupNoRecordLog = sfdcData.CaseData.CanPopupNoRecordActivityLog;
                                        }

                                        #endregion Case Popup
                                    }
                                    break;

                                case "Opportunity":
                                    if (sfdcData.OpportunityData != null)
                                    {
                                        #region Opportunity Popup

                                        PopupData opportunityPopup = new PopupData();
                                        if (sfdcData.OpportunityData.NoRecordFound == "opennew")
                                        {
                                            opportunityPopup.InteractionId = interactionId;
                                            opportunityPopup.ObjectName = sfdcData.OpportunityData.ObjectName;
                                            opportunityPopup.PopupUrl = GetNewPageUrl(sfdcData.CommonSearchData, "006", this.opportunityOptions.NewrecordFieldIds);
                                            this.SendPopupData(interactionId + "oppopp", opportunityPopup);
                                            //norecord create user level activity
                                            if (!canCreateNoRecordLog)
                                                canCreateNoRecordLog = sfdcData.OpportunityData.CanCreateNoRecordActivityLog;
                                            if (!canPopupNoRecordLog)
                                                canPopupNoRecordLog = sfdcData.OpportunityData.CanPopupNoRecordActivityLog;
                                        }
                                        else if (sfdcData.OpportunityData.NoRecordFound == "createnew")
                                        {
                                            opportunityPopup.InteractionId = interactionId;
                                            opportunityPopup.ObjectName = sfdcData.OpportunityData.ObjectName;
                                            if (sfdcData.OpportunityData.CreateRecordFieldData != null)
                                            {
                                                string record = sfdcObject.CreateNewRecord(interactionId, sfdcData.OpportunityData.CreateRecordFieldData, sfdcData.OpportunityData.ObjectName);
                                                if (record != null)
                                                {
                                                    if (this.opportunityOptions.CanCreateLogForNewRecordCreate && sfdcData.OpportunityData.ActivityLogData != null)
                                                    {
                                                        this.sfdcObject.CreateActivityLog(interactionId, sfdcData.OpportunityData.ActivityLogData, sfdcData.OpportunityData.ObjectName, record);
                                                    }
                                                    opportunityPopup.PopupUrl = record;
                                                    this.SendPopupData(interactionId + "oppopp", opportunityPopup);
                                                }
                                                else
                                                {
                                                    this.logger.Warn("Can not popup New Opportunity Record because Null Returned while creating New Record");
                                                }
                                            }
                                            else
                                            {
                                                this.logger.Warn("Can not create New " + sfdcData.OpportunityData.ObjectName + " Record because data is null");
                                            }
                                        }
                                        else if (sfdcData.OpportunityData.NoRecordFound == "searchpage")
                                        {
                                            opportunityPopup.InteractionId = interactionId;
                                            opportunityPopup.ObjectName = sfdcData.OpportunityData.ObjectName;
                                            opportunityPopup.PopupUrl = this.generalOptions.SearchPageUrl + this.opportunityOptions.SearchPageMode + "str=" + outputValues.SearchData;
                                            this.SendPopupData(interactionId + "oppopp", opportunityPopup);
                                        }
                                        else if (sfdcData.OpportunityData.NoRecordFound.ToLower() == "none" && CheckCanCreateNoRecordforNone(sfdcData.OpportunityData, calltype))
                                        {
                                            canCreateNoRecordLog = true;
                                            if (!canPopupNoRecordLog)
                                                canPopupNoRecordLog = sfdcData.OpportunityData.CanPopupNoRecordActivityLog;
                                        }

                                        #endregion Opportunity Popup
                                    }
                                    break;

                                default:
                                    if (sfdcData.CustomObjectData != null)
                                    {
                                        #region CustomObject Popup

                                        foreach (ICustomObject dat in sfdcData.CustomObjectData)
                                        {
                                            if (pkey == dat.ObjectName)
                                            {
                                                VoiceOptions customOptions = this.customObjectOptions[Settings.CustomObjectNames[pkey]];
                                                if (customOptions != null)
                                                {
                                                    PopupData customPopup = new PopupData();
                                                    if (dat.NoRecordFound == "opennew")
                                                    {
                                                        customPopup.InteractionId = interactionId;
                                                        customPopup.ObjectName = dat.ObjectName;
                                                        customPopup.PopupUrl = GetNewPageUrl(sfdcData.CommonSearchData, dat.CustomObjectURL, customOptions.NewrecordFieldIds);
                                                        this.SendPopupData(interactionId + "custompp", customPopup);
                                                        //norecord create user level activity
                                                        if (!canCreateNoRecordLog)
                                                            canCreateNoRecordLog = dat.CanCreateNoRecordActivityLog;
                                                        if (!canPopupNoRecordLog)
                                                            canPopupNoRecordLog = dat.CanPopupNoRecordActivityLog;
                                                    }
                                                    else if (dat.NoRecordFound == "createnew")
                                                    {
                                                        customPopup.InteractionId = interactionId;
                                                        customPopup.ObjectName = dat.ObjectName;
                                                        if (dat.CreateRecordFieldData != null)
                                                        {
                                                            string record = sfdcObject.CreateNewRecord(interactionId, dat.CreateRecordFieldData, dat.ObjectName);
                                                            if (record != null)
                                                            {
                                                                if (customOptions.CanCreateLogForNewRecordCreate && dat.ActivityLogData != null)
                                                                {
                                                                    this.sfdcObject.CreateActivityLog(interactionId, dat.ActivityLogData, dat.ObjectName, record);
                                                                }
                                                                customPopup.PopupUrl = record;
                                                                this.SendPopupData(interactionId + "custompp", customPopup);
                                                            }
                                                            else
                                                            {
                                                                this.logger.Warn("Can not popup New " + dat.ObjectName + " Record because Null Returned while creating New Record");
                                                            }
                                                        }
                                                        else
                                                        {
                                                            this.logger.Warn("Can not create New " + dat.ObjectName + " Record because data is null");
                                                        }
                                                    }
                                                    else if (dat.NoRecordFound == "searchpage")
                                                    {
                                                        customPopup.InteractionId = interactionId;
                                                        customPopup.ObjectName = dat.ObjectName;
                                                        customPopup.PopupUrl = this.generalOptions.SearchPageUrl + customOptions.SearchPageMode + "str=" + outputValues.SearchData;
                                                        this.SendPopupData(interactionId + "custompp", customPopup);
                                                    }
                                                    else if (dat.NoRecordFound.ToLower() == "none" && CheckCanCreateNoRecordforNone(dat, calltype))
                                                    {
                                                        canCreateNoRecordLog = true;
                                                        if (!canPopupNoRecordLog)
                                                            canPopupNoRecordLog = dat.CanPopupNoRecordActivityLog;
                                                    }
                                                }
                                            }
                                        }

                                        #endregion CustomObject Popup
                                    }
                                    break;
                            }
                        }

                        #endregion No Record Found
                    }
                }
                else
                {
                    this.logger.Error("Cannot popup records because SFDC Search method returned null...");
                }

                if (!string.IsNullOrWhiteSpace(multimatchURL))
                {
                    PopupData popup = new PopupData();
                    popup.InteractionId = interactionId;
                    popup.ObjectName = SearchPageObjectName;
                    popup.PopupUrl = this.generalOptions.SearchPageUrl + multimatchURL + "&str=" + outputValues.SearchData;
                    SendPopupData(interactionId, popup);
                    multimatchURL = string.Empty;
                }
                if (!string.IsNullOrWhiteSpace(openAllIds))
                {
                    if (openAllIds.Split(',').Length < 51)
                    {
                        PopupData popupAll = new PopupData();
                        popupAll.InteractionId = interactionId;
                        popupAll.ObjectName = SearchPageObjectName;
                        popupAll.PopupUrl = openAllIds;
                        SendPopupData(interactionId, popupAll);
                        openAllIds = string.Empty;
                    }
                    else
                    {
                        string[] array = openAllIds.Split(',');
                        string temp = string.Empty;
                        if (array != null)
                        {
                            for (int i = 0; i < 50; i++)
                            {
                                temp += array[i] + ",";
                            }
                        }
                        if (temp != string.Empty)
                        {
                            PopupData popupAll = new PopupData();
                            popupAll.InteractionId = interactionId;
                            popupAll.ObjectName = SearchPageObjectName;
                            popupAll.PopupUrl = temp;
                            SendPopupData(interactionId, popupAll);
                            openAllIds = string.Empty;
                        }
                    }
                }
                if (canCreateMultiMatchLog)
                {
                    //create profile level activity
                    string profileActivittyID = sfdcObject.CreateActivityLog(interactionId, sfdcData.ProfileActivityLogData, "profileactivity");
                    if (canPopupMultiMatchLog && profileActivittyID != null)
                    {
                        //popup Activity
                        PopupData ProfilePopupData = new PopupData
                        {
                            InteractionId = interactionId,
                            ObjectName = "profileactivity",
                            PopupUrl = profileActivittyID
                        };
                        SendPopupData(interactionId + "NMMM", ProfilePopupData);
                    }
                }
                else if (canCreateNoRecordLog)
                {
                    //create profile level activity
                    string profileActivittyID = sfdcObject.CreateActivityLog(interactionId, sfdcData.ProfileActivityLogData, "profileactivity");
                    if (canPopupNoRecordLog && profileActivittyID != null)
                    {
                        //popup Activity
                        PopupData ProfilePopupData = new PopupData
                        {
                            InteractionId = interactionId,
                            ObjectName = "profileactivity",
                            PopupUrl = profileActivittyID
                        };
                        SendPopupData(interactionId + "NMMM", ProfilePopupData);
                    }
                }
            }
            catch (Exception generalException)
            {
                logger.Error("CommonPopup : Error Occurred  : " + generalException.ToString());
            }
        }

        #endregion Methods
    }
}