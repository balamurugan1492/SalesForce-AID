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

namespace Pointel.Salesforce.Adapter.SFDCModels
{
    using System;

    using Genesyslab.Platform.Commons.Collections;
    using Genesyslab.Platform.Commons.Protocols;

    using Pointel.Salesforce.Adapter.Configurations;
    using Pointel.Salesforce.Adapter.LogMessage;
    using Pointel.Salesforce.Adapter.SFDCUtils;
    using Pointel.Salesforce.Adapter.Utility;

    /// <summary>
    /// Comment: Provides Search and Activity Data for Lead Object
    /// Last Modified: 25-08-2015
    /// Created by: Pointel Inc
    /// </summary>
    internal class SFDCLead
    {
        #region Fields

        private static SFDCLead leadData = null;

        private KeyValueCollection LeadChatLogConfig = null;
        private ChatOptions leadChatOptions = null;
        private KeyValueCollection LeadLogConfig = null;
        private KeyValueCollection LeadRecordConfig = null;
        private VoiceOptions leadVoiceOptions = null;
        private Log logger = null;
        private SFDCUtility sfdcObject = null;

        #endregion Fields

        #region Constructors

        private SFDCLead()
        {
            this.logger = Log.GenInstance();
            this.sfdcObject = SFDCUtility.GetInstance();
            this.leadVoiceOptions = Settings.LeadVoiceOptions;
            this.leadChatOptions = Settings.LeadChatOptions;
            this.LeadLogConfig = (Settings.VoiceActivityLogCollection.ContainsKey("lead")) ? Settings.VoiceActivityLogCollection["lead"] : null;
            this.LeadChatLogConfig = (Settings.ChatActivityLogCollection.ContainsKey("lead")) ? Settings.ChatActivityLogCollection["lead"] : null;
            this.LeadRecordConfig = Settings.LeadNewRecordConfigs;
        }

        #endregion Constructors

        #region Methods

        public static SFDCLead GetInstance()
        {
            if (leadData == null)
            {
                leadData = new SFDCLead();
            }
            return leadData;
        }

        public ILead GetLeadChatPopupData(IMessage message, SFDCCallType callType)
        {
            try
            {
                this.logger.Info("GetLeadChatPopupData :  Reading Lead Popup Data.....");
                this.logger.Info("GetLeadChatPopupData :  Event Name : " + message.Name);
                if (this.leadChatOptions != null)
                {
                    dynamic popupEvent = Convert.ChangeType(message, message.GetType());
                    if (popupEvent != null)
                    {
                        ILead lead = new LeadData();

                        #region Collect Lead Data

                        lead.SearchData = GetLeadChatSearchValue(popupEvent.Interaction.InteractionUserData, message, callType);
                        lead.ObjectName = leadChatOptions.ObjectName;
                        lead.NoRecordFound = GetNoRecordFoundAction(callType, leadChatOptions);
                        lead.MultipleMatchRecord = GetMultiMatchRecordAction(callType, leadChatOptions);
                        lead.NewRecordFieldIds = leadChatOptions.NewrecordFieldIds;
                        lead.SearchCondition = leadChatOptions.SearchCondition;
                        lead.CreateLogForNewRecord = leadChatOptions.CanCreateLogForNewRecordCreate;
                        lead.MaxRecordOpenCount = leadChatOptions.MaxNosRecordOpen;
                        lead.SearchpageMode = leadChatOptions.SearchPageMode;
                        lead.PhoneNumberSearchFormat = leadChatOptions.PhoneNumberSearchFormat;
                        if (lead.NoRecordFound.Equals("createnew") && this.LeadRecordConfig != null)
                        {
                            lead.CreateRecordFieldData = this.sfdcObject.GetChatRecordData(this.LeadRecordConfig, message, callType);
                        }
                        if (callType == SFDCCallType.InboundChat)
                        {
                            lead.CreateActvityLog = leadChatOptions.InboundCanCreateLog;
                            if (leadChatOptions.InboundCanCreateLog && this.LeadChatLogConfig != null)
                            {
                                lead.ActivityLogData = this.sfdcObject.GetChatActivityLog(this.LeadChatLogConfig, popupEvent, callType);
                            }
                        }
                        else if (callType == SFDCCallType.ConsultChatReceived)
                        {
                            lead.CreateActvityLog = leadChatOptions.ConsultCanCreateLog;
                            if (leadChatOptions.ConsultCanCreateLog && this.LeadLogConfig != null)
                            {
                                lead.ActivityLogData = this.sfdcObject.GetChatActivityLog(this.LeadLogConfig, popupEvent, callType);
                            }
                        }

                        #endregion Collect Lead Data

                        return lead;
                    }
                }
                else
                {
                    this.logger.Info("Can not Collect Lead Popup data because Lead Configuration is null.");
                }
            }
            catch (Exception generalException)
            {
                this.logger.Error("GetLeadChatPopupData : Error occurred while reading Lead Data : " + generalException.ToString());
            }
            return null;
        }

        public ILead GetLeadChatUpdateData(IMessage message, SFDCCallType callType, string duration, string chatContent)
        {
            try
            {
                this.logger.Info("GetLeadChatUpdateData :  Reading Lead Update Data.....");
                this.logger.Info("GetLeadChatUpdateData :  Event Name : " + message.Name);
                if (this.leadChatOptions != null)
                {
                    dynamic popupEvent = Convert.ChangeType(message, message.GetType());

                    if (popupEvent != null)
                    {
                        ILead lead = new LeadData();

                        #region Collect Lead Data

                        lead.ObjectName = leadChatOptions.ObjectName;

                        if (callType == SFDCCallType.InboundChat)
                        {
                            if (leadChatOptions.InboundCanUpdateLog)
                            {
                                lead.UpdateActivityLog = true;
                                lead.UpdateActivityLogData = this.sfdcObject.GetChatUpdateActivityLog(this.LeadChatLogConfig, popupEvent, callType, duration, chatContent);
                            }
                        }
                        if (callType == SFDCCallType.ConsultChatReceived)
                        {
                            if (leadChatOptions.ConsultCanUpdateLog)
                            {
                                lead.UpdateActivityLog = true;
                                lead.UpdateActivityLogData = this.sfdcObject.GetChatUpdateActivityLog(this.LeadChatLogConfig, popupEvent, callType, duration, chatContent);
                            }
                        }
                        //update lead record fields
                        lead.UpdateRecordFields = leadChatOptions.CanUpdateRecordData;
                        if (GetNoRecordFoundAction(callType, leadChatOptions).Equals("createnew") && this.LeadRecordConfig != null)
                        {
                            if (leadChatOptions.CanUpdateRecordData)
                            {
                                lead.UpdateRecordFieldsData = this.sfdcObject.GetChatUpdateActivityLog(this.LeadRecordConfig, popupEvent, callType, duration, chatContent);
                            }
                        }

                        #endregion Collect Lead Data

                        return lead;
                    }
                }
                else
                {
                    this.logger.Info("Can not Collect Lead Update data because Lead Configuration is null.");
                }
            }
            catch (Exception generalException)
            {
                this.logger.Error("GetLeadChatUpdateData : Error occurred while reading Lead Data : " + generalException.ToString());
            }
            return null;
        }

        public ILead GetLeadVoicePopupData(IMessage message, SFDCCallType callType)
        {
            try
            {
                this.logger.Info("GetLeadVoicePopupData :  Reading Lead Popup Data.....");
                this.logger.Info("GetLeadVoicePopupData :  Event Name : " + message.Name);
                dynamic popupEvent = Convert.ChangeType(message, message.GetType());
                if (this.leadVoiceOptions != null)
                {
                    if (popupEvent != null)
                    {
                        ILead lead = new LeadData();

                        #region Collect Lead Data

                        lead.SearchData = GetLeadVoiceSearchValue(popupEvent.UserData, message, callType);
                        lead.SearchFields = leadVoiceOptions.SearchFields;
                        lead.ObjectName = leadVoiceOptions.ObjectName;
                        lead.NoRecordFound = GetNoRecordFoundAction(callType, leadVoiceOptions);
                        lead.MultipleMatchRecord = GetMultiMatchRecordAction(callType, leadVoiceOptions);
                        lead.NewRecordFieldIds = leadVoiceOptions.NewrecordFieldIds;
                        lead.SearchCondition = leadVoiceOptions.SearchCondition;
                        lead.CreateLogForNewRecord = leadVoiceOptions.CanCreateLogForNewRecordCreate;
                        lead.MaxRecordOpenCount = leadVoiceOptions.MaxNosRecordOpen;
                        lead.SearchpageMode = leadVoiceOptions.SearchPageMode;
                        lead.PhoneNumberSearchFormat = leadVoiceOptions.PhoneNumberSearchFormat;
                        lead.CanCreateNoRecordActivityLog = GetCanCreateProfileActivity(callType, leadVoiceOptions, true);
                        lead.CanPopupNoRecordActivityLog = GetCanPopupProfileActivity(callType, leadVoiceOptions, true);
                        lead.CanCreateMultiMatchActivityLog = GetCanCreateProfileActivity(callType, leadVoiceOptions);
                        lead.CanPopupMultiMatchActivityLog = GetCanPopupProfileActivity(callType, leadVoiceOptions);
                        lead.CanCreateProfileActivityforInbNoRecord = leadVoiceOptions.CanCreateProfileActivityforInbNoRecord;
                        lead.CanCreateProfileActivityforOutNoRecord = leadVoiceOptions.CanCreateProfileActivityforOutNoRecord;
                        lead.CanCreateProfileActivityforConNoRecord = leadVoiceOptions.CanCreateProfileActivityforConNoRecord;
                        if (lead.NoRecordFound.Equals("createnew") && this.LeadRecordConfig != null)
                        {
                            lead.CreateRecordFieldData = this.sfdcObject.GetVoiceRecordData(this.LeadRecordConfig, message, callType);
                        }

                        switch (callType)
                        {
                            case SFDCCallType.Inbound:
                                lead.CreateActvityLog = leadVoiceOptions.InboundCanCreateLog;
                                if (leadVoiceOptions.InboundCanCreateLog && this.LeadLogConfig != null)
                                {
                                    lead.ActivityLogData = this.sfdcObject.GetVoiceActivityLog(this.LeadLogConfig, popupEvent, callType);
                                }
                                break;

                            case SFDCCallType.OutboundSuccess:
                                lead.CreateActvityLog = leadVoiceOptions.OutboundCanCreateLog;
                                if (leadVoiceOptions.OutboundCanCreateLog && this.LeadLogConfig != null)
                                {
                                    lead.ActivityLogData = this.sfdcObject.GetVoiceActivityLog(this.LeadLogConfig, popupEvent, callType);
                                }
                                break;

                            case SFDCCallType.OutboundFailure:
                                lead.CreateActvityLog = leadVoiceOptions.OutboundFailureCanCreateLog;
                                if (leadVoiceOptions.OutboundFailureCanCreateLog && this.LeadLogConfig != null)
                                {
                                    lead.ActivityLogData = this.sfdcObject.GetVoiceActivityLog(this.LeadLogConfig, popupEvent, callType);
                                }
                                break;

                            case SFDCCallType.ConsultReceived:
                                lead.CreateActvityLog = leadVoiceOptions.ConsultCanCreateLog;
                                if (leadVoiceOptions.ConsultCanCreateLog && this.LeadLogConfig != null)
                                {
                                    lead.ActivityLogData = this.sfdcObject.GetVoiceActivityLog(this.LeadLogConfig, popupEvent, callType);
                                }
                                break;

                            case SFDCCallType.ConsultSuccess:
                                lead.CreateActvityLog = leadVoiceOptions.ConsultCanCreateLog;
                                lead.NoRecordFound = "none";
                                if (leadVoiceOptions.ConsultCanCreateLog && this.LeadLogConfig != null)
                                {
                                    lead.ActivityLogData = this.sfdcObject.GetVoiceActivityLog(this.LeadLogConfig, popupEvent, callType);
                                }
                                break;

                            case SFDCCallType.ConsultFailure:
                                lead.CreateActvityLog = leadVoiceOptions.ConsultFailureCanCreateLog;
                                lead.NoRecordFound = "none";
                                if (leadVoiceOptions.ConsultFailureCanCreateLog && this.LeadLogConfig != null)
                                {
                                    lead.ActivityLogData = this.sfdcObject.GetVoiceActivityLog(this.LeadLogConfig, popupEvent, callType);
                                }
                                break;

                            default:
                                break;
                        }

                        #endregion Collect Lead Data

                        return lead;
                    }
                }
                else
                {
                    this.logger.Info("Can not Collect Lead Popup data because Lead Configuration is null.");
                }
            }
            catch (Exception generalException)
            {
                this.logger.Error("GetLeadVoicePopupData : Error occurred while reading Lead Data : " + generalException.ToString());
            }
            return null;
        }

        public ILead GetLeadVoiceUpdateData(IMessage message, SFDCCallType callType, string duration)
        {
            try
            {
                this.logger.Info("GetLeadVoiceUpdateData :  Reading Lead Update Data.....");
                this.logger.Info("GetLeadVoiceUpdateData :  Event Name : " + message.Name);
                if (this.leadVoiceOptions != null)
                {
                    dynamic popupEvent = Convert.ChangeType(message, message.GetType());

                    if (popupEvent != null)
                    {
                        ILead lead = new LeadData();

                        #region Collect Lead Data

                        lead.ObjectName = leadVoiceOptions.ObjectName;

                        if (callType == SFDCCallType.Inbound)
                        {
                            if (leadVoiceOptions.InboundCanUpdateLog)
                            {
                                lead.UpdateActivityLog = true;
                                lead.UpdateActivityLogData = this.sfdcObject.GetVoiceUpdateActivityLog(this.LeadLogConfig, popupEvent, callType, duration);
                            }
                        }
                        else if (callType == SFDCCallType.OutboundSuccess || callType == SFDCCallType.OutboundFailure)
                        {
                            if (leadVoiceOptions.OutboundCanUpdateLog)
                            {
                                lead.UpdateActivityLog = true;
                                lead.UpdateActivityLogData = this.sfdcObject.GetVoiceUpdateActivityLog(this.LeadLogConfig, popupEvent, callType, duration);
                            }
                        }
                        else if (callType == SFDCCallType.ConsultSuccess || callType == SFDCCallType.ConsultFailure || callType == SFDCCallType.ConsultReceived)
                        {
                            if (leadVoiceOptions.ConsultCanUpdateLog)
                            {
                                lead.UpdateActivityLog = true;
                                lead.UpdateActivityLogData = this.sfdcObject.GetVoiceUpdateActivityLog(this.LeadLogConfig, popupEvent, callType, duration);
                            }
                        }
                        if (leadVoiceOptions.CanUpdateRecordData)
                        {
                            lead.UpdateRecordFields = true;
                            lead.UpdateRecordFieldsData = this.sfdcObject.GetVoiceUpdateRecordData(this.LeadRecordConfig, popupEvent, callType, duration);
                        }

                        #endregion Collect Lead Data

                        return lead;
                    }
                }
                else
                {
                    this.logger.Info("Can not Collect Lead Update data because Lead Configuration is null.");
                }
            }
            catch (Exception generalException)
            {
                this.logger.Error("GetLeadVoiceUpdateData : Error occurred while reading Lead Data : " + generalException.ToString());
            }
            return null;
        }

        private bool GetCanCreateProfileActivity(SFDCCallType callType, VoiceOptions voiceOptions, bool IsNoRecord = false)
        {
            bool canCreateNoRecordMultiMatchLog = false;
            switch (callType)
            {
                case SFDCCallType.Inbound:
                    canCreateNoRecordMultiMatchLog = IsNoRecord ? voiceOptions.InbCanCreateNorecordActivity : voiceOptions.InbCanCreateMultimatchActivity;
                    break;

                case SFDCCallType.OutboundFailure:
                case SFDCCallType.OutboundSuccess:
                    canCreateNoRecordMultiMatchLog = IsNoRecord ? voiceOptions.OutCanCreateNorecordActivity : voiceOptions.OutCanCreateMultimatchActivity;
                    break;

                case SFDCCallType.ConsultSuccess:
                case SFDCCallType.ConsultReceived:
                case SFDCCallType.ConsultFailure:
                    canCreateNoRecordMultiMatchLog = IsNoRecord ? voiceOptions.ConCanCreateNorecordActivity : voiceOptions.ConCanCreateMultimatchActivity;
                    break;
            }
            return canCreateNoRecordMultiMatchLog;
        }

        private bool GetCanPopupProfileActivity(SFDCCallType callType, VoiceOptions voiceOptions, bool IsNoRecord = false)
        {
            bool canPopupNoRecordMultiMatchLog = false;
            switch (callType)
            {
                case SFDCCallType.Inbound:
                    canPopupNoRecordMultiMatchLog = IsNoRecord ? voiceOptions.InbCanPopupNorecordActivity : voiceOptions.InbCanPopupMultimatchActivity;
                    break;

                case SFDCCallType.OutboundFailure:
                case SFDCCallType.OutboundSuccess:
                    canPopupNoRecordMultiMatchLog = IsNoRecord ? voiceOptions.OutCanPopupNorecordActivity : voiceOptions.OutCanPopupMultimatchActivity;
                    break;

                case SFDCCallType.ConsultSuccess:
                case SFDCCallType.ConsultReceived:
                case SFDCCallType.ConsultFailure:
                    canPopupNoRecordMultiMatchLog = IsNoRecord ? voiceOptions.ConCanPopupNorecordActivity : voiceOptions.ConCanPopupMultimatchActivity;
                    break;
            }
            return canPopupNoRecordMultiMatchLog;
        }

        private string GetLeadChatSearchValue(KeyValueCollection userData, IMessage message, SFDCCallType callType)
        {
            try
            {
                this.logger.Info("GetLeadChatSearchValue :  Reading Case Popup Data.....");
                this.logger.Info("GetLeadChatSearchValue :  UserData Name : " + Convert.ToString(userData));
                this.logger.Info("GetLeadChatSearchValue :  Event Name : " + message.Name);
                this.logger.Info("GetLeadChatSearchValue :  CallType Name : " + callType.ToString());
                string[] userDataSearchKeys = null;
                string[] attributeSearchKeys = null;
                string searchValue = string.Empty;
                if (callType == SFDCCallType.InboundChat)
                {
                    userDataSearchKeys = (leadChatOptions.InboundSearchUserDataKeys != null) ? leadChatOptions.InboundSearchUserDataKeys.Split(',') : null;
                    attributeSearchKeys = (leadChatOptions.InboundSearchAttributeKeys != null) ? leadChatOptions.InboundSearchAttributeKeys.Split(',') : null;
                }
                else if (callType == SFDCCallType.ConsultChatReceived)
                {
                    userDataSearchKeys = (leadChatOptions.ConsultSearchUserDataKeys != null) ? leadChatOptions.ConsultSearchUserDataKeys.Split(',') : null;
                    attributeSearchKeys = (leadChatOptions.ConsultSearchAttributeKeys != null) ? leadChatOptions.ConsultSearchAttributeKeys.Split(',') : null;
                }

                if (leadChatOptions.SearchPriority == "user-data")
                {
                    searchValue = this.sfdcObject.GetUserDataSearchValues(userData, userDataSearchKeys);
                }
                else if (leadChatOptions.SearchPriority == "attribute")
                {
                    searchValue = this.sfdcObject.GetAttributeSearchValues(message, attributeSearchKeys);
                }
                else if (leadChatOptions.SearchPriority == "both")
                {
                    searchValue = this.sfdcObject.GetUserDataSearchValues(userData, userDataSearchKeys);
                    if (searchValue != string.Empty)
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
                this.logger.Error("GetLeadChatSearchValue : Error occurred while reading Lead Data : " + generalException.ToString());
            }
            return null;
        }

        private string GetLeadVoiceSearchValue(KeyValueCollection userData, IMessage message, SFDCCallType callType)
        {
            try
            {
                this.logger.Info("GetLeadVoiceSearchValue :  Reading Lead Popup Data.....");
                this.logger.Info("GetLeadVoiceSearchValue :  UserData Name : " + Convert.ToString(userData));
                this.logger.Info("GetLeadVoiceSearchValue :  Event Name : " + message.Name);
                this.logger.Info("GetLeadVoiceSearchValue :  CallType Name : " + callType.ToString());
                string[] userDataSearchKeys = null;
                string[] attributeSearchKeys = null;
                string searchValue = string.Empty;
                if (callType == SFDCCallType.Inbound)
                {
                    userDataSearchKeys = (this.leadVoiceOptions.InboundSearchUserDataKeys != null) ? this.leadVoiceOptions.InboundSearchUserDataKeys.Split(',') : null;
                    attributeSearchKeys = (this.leadVoiceOptions.InboundSearchAttributeKeys != null) ? this.leadVoiceOptions.InboundSearchAttributeKeys.Split(',') : null;
                }
                else if (callType == SFDCCallType.OutboundSuccess || callType == SFDCCallType.OutboundFailure)
                {
                    userDataSearchKeys = (this.leadVoiceOptions.OutboundSearchUserDataKeys != null) ? this.leadVoiceOptions.OutboundSearchUserDataKeys.Split(',') : null;
                    attributeSearchKeys = (this.leadVoiceOptions.OutboundSearchAttributeKeys != null) ? this.leadVoiceOptions.OutboundSearchAttributeKeys.Split(',') : null;
                }
                else if (callType == SFDCCallType.ConsultSuccess || callType == SFDCCallType.ConsultFailure)
                {
                    return this.sfdcObject.GetAttributeSearchValues(message, new string[] { "otherdn" });
                }
                else if (callType == SFDCCallType.ConsultReceived)
                {
                    userDataSearchKeys = (leadVoiceOptions.ConsultSearchUserDataKeys != null) ? leadVoiceOptions.ConsultSearchUserDataKeys.Split(',') : null;
                    attributeSearchKeys = (leadVoiceOptions.ConsultSearchAttributeKeys != null) ? leadVoiceOptions.ConsultSearchAttributeKeys.Split(',') : null;
                }

                #region OLD

                //if (leadVoiceOptions.SearchPriority == "user-data")
                //{
                //    if (userDataSearchKeys != null)
                //        searchValue = this.sfdcObject.GetUserDataSearchValues(userData, userDataSearchKeys);
                //    else if (attributeSearchKeys != null)
                //        searchValue = this.sfdcObject.GetAttributeSearchValues(message, attributeSearchKeys);

                //}
                //else if (leadVoiceOptions.SearchPriority == "attribute")
                //{
                //    searchValue = this.sfdcObject.GetAttributeSearchValues(message, attributeSearchKeys);
                //}
                //else if (leadVoiceOptions.SearchPriority == "both")
                //{
                //    searchValue = this.sfdcObject.GetUserDataSearchValues(userData, userDataSearchKeys);
                //    if (searchValue != string.Empty)
                //    {
                //        string temp = this.sfdcObject.GetAttributeSearchValues(message, attributeSearchKeys);
                //        if (temp != string.Empty)
                //        {
                //            searchValue += "," + temp;
                //        }
                //    }
                //    else
                //    {
                //        searchValue = this.sfdcObject.GetAttributeSearchValues(message, attributeSearchKeys);
                //    }
                //}
                //return searchValue;

                #endregion OLD

                if (leadVoiceOptions.SearchPriority == "user-data")
                {
                    if (userDataSearchKeys != null && userData != null)
                    {
                        searchValue = this.sfdcObject.GetUserDataSearchValues(userData, userDataSearchKeys);
                        if (!this.sfdcObject.ValidateSearchData(searchValue))
                        {
                            searchValue = this.sfdcObject.GetAttributeSearchValues(message, attributeSearchKeys);
                        }
                    }
                    else if (attributeSearchKeys != null)
                    {
                        searchValue = this.sfdcObject.GetAttributeSearchValues(message, attributeSearchKeys);
                    }
                }
                else if (leadVoiceOptions.SearchPriority == "attribute")
                {
                    searchValue = this.sfdcObject.GetAttributeSearchValues(message, attributeSearchKeys);
                    if (!this.sfdcObject.ValidateSearchData(searchValue))
                    {
                        if (userDataSearchKeys != null && userData != null)
                        {
                            searchValue = this.sfdcObject.GetUserDataSearchValues(userData, userDataSearchKeys);
                        }
                    }
                }
                else if (leadVoiceOptions.SearchPriority == "both")
                {
                    if (userData != null)
                        searchValue = this.sfdcObject.GetUserDataSearchValues(userData, userDataSearchKeys);
                    if (!this.sfdcObject.ValidateSearchData(searchValue))
                    {
                        searchValue = this.sfdcObject.GetAttributeSearchValues(message, attributeSearchKeys);
                    }
                    else
                    {
                        string temp = this.sfdcObject.GetAttributeSearchValues(message, attributeSearchKeys);
                        if (temp != string.Empty)
                        {
                            searchValue += "," + temp;
                        }
                    }
                }
                return searchValue;
            }
            catch (Exception generalException)
            {
                this.logger.Error("GetLeadVoiceSearchValue : Error occurred while reading Lead Data : " + generalException.ToString());
            }
            return null;
        }

        private string GetMultiMatchRecordAction(SFDCCallType calltype, VoiceOptions voiceOptions)
        {
            string NoMatchRecordAction = string.Empty;
            switch (calltype)
            {
                case SFDCCallType.Inbound:
                    NoMatchRecordAction = voiceOptions.InbMultiMatchRecordAction;
                    break;

                case SFDCCallType.OutboundFailure:
                case SFDCCallType.OutboundSuccess:
                    NoMatchRecordAction = voiceOptions.OutMultiMatchRecordAction;
                    break;

                case SFDCCallType.ConsultFailure:
                case SFDCCallType.ConsultReceived:
                case SFDCCallType.ConsultSuccess:
                    NoMatchRecordAction = voiceOptions.ConMultiMatchRecordAction;
                    break;
            }
            return NoMatchRecordAction;
        }

        private string GetMultiMatchRecordAction(SFDCCallType calltype, ChatOptions chatOptions)
        {
            string NoMatchRecordAction = string.Empty;
            switch (calltype)
            {
                case SFDCCallType.InboundChat:
                    NoMatchRecordAction = chatOptions.InbMultiMatchRecordAction;
                    break;

                case SFDCCallType.ConsultChatFailure:
                case SFDCCallType.ConsultReceived:
                case SFDCCallType.ConsultSuccess:
                    NoMatchRecordAction = chatOptions.ConMultiMatchRecordAction;
                    break;
            }
            return NoMatchRecordAction;
        }

        private string GetNoRecordFoundAction(SFDCCallType calltype, VoiceOptions options)
        {
            string NoMatchRecordAction = string.Empty;
            switch (calltype)
            {
                case SFDCCallType.Inbound:
                    NoMatchRecordAction = options.InbNoMatchRecordAction;
                    break;

                case SFDCCallType.OutboundFailure:
                case SFDCCallType.OutboundSuccess:
                    NoMatchRecordAction = options.OutNoMatchRecordAction;
                    break;

                case SFDCCallType.ConsultFailure:
                case SFDCCallType.ConsultReceived:
                case SFDCCallType.ConsultSuccess:
                    NoMatchRecordAction = options.ConNoMatchRecordAction;
                    break;
            }
            return NoMatchRecordAction;
        }

        private string GetNoRecordFoundAction(SFDCCallType calltype, ChatOptions options)
        {
            string NoMatchRecordAction = string.Empty;
            switch (calltype)
            {
                case SFDCCallType.Inbound:
                    NoMatchRecordAction = options.InbNoMatchRecordAction;
                    break;

                case SFDCCallType.ConsultChatFailure:
                case SFDCCallType.ConsultChatReceived:
                case SFDCCallType.ConsultChatSuccess:
                    NoMatchRecordAction = options.ConNoMatchRecordAction;
                    break;
            }
            return NoMatchRecordAction;
        }

        #endregion Methods
    }
}