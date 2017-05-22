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
using Pointel.Salesforce.Adapter.SFDCUtils;
using Pointel.Salesforce.Adapter.Utility;
using System;

namespace Pointel.Salesforce.Adapter.SFDCModels
{
    /// <summary>
    /// Comment: Provides Search and Activity Data for Lead Object Last Modified: 25-08-2015 Created
    /// by: Pointel Inc
    /// </summary>
    internal class SFDCLead
    {
        #region Field Declaration

        private static SFDCLead leadData = null;
        private KeyValueCollection LeadChatLogConfig = null;
        private ChatOptions leadChatOptions = null;
        private KeyValueCollection LeadEmailLogConfig = null;
        private EmailOptions leadEmailOptions = null;
        private KeyValueCollection LeadEmailWorkbinConfig = null;
        private KeyValueCollection LeadLogConfig = null;
        private KeyValueCollection LeadEmailRecordConfig = null;
        private KeyValueCollection LeadVoiceRecordConfig = null;
        private KeyValueCollection LeadChatRecordConfig = null;
        private VoiceOptions leadVoiceOptions = null;
        private Log logger = null;
        private SFDCUtility sfdcUtility = null;
        private SFDCUtiltiyHelper sfdcUtilityHelper = null;

        #endregion Field Declaration

        #region Constructor

        private SFDCLead()
        {
            this.logger = Log.GenInstance();
            this.sfdcUtility = SFDCUtility.GetInstance();
            this.leadVoiceOptions = Settings.LeadVoiceOptions;
            this.leadChatOptions = Settings.LeadChatOptions;
            this.LeadLogConfig = (Settings.VoiceActivityLogCollection.ContainsKey("lead")) ? Settings.VoiceActivityLogCollection["lead"] : null;
            this.LeadChatLogConfig = (Settings.ChatActivityLogCollection.ContainsKey("lead")) ? Settings.ChatActivityLogCollection["lead"] : null;
            this.LeadEmailRecordConfig = (Settings.EmailNewRecordCollection.ContainsKey("lead")) ? Settings.EmailNewRecordCollection["lead"] : null;
            this.LeadVoiceRecordConfig = (Settings.VoiceNewRecordCollection.ContainsKey("lead")) ? Settings.VoiceNewRecordCollection["lead"] : null;
            this.LeadChatRecordConfig = (Settings.ChatNewRecordCollection.ContainsKey("lead")) ? Settings.ChatNewRecordCollection["lead"] : null;
            this.LeadEmailLogConfig = (Settings.EmailActivityLogCollection.ContainsKey("lead")) ? Settings.EmailActivityLogCollection["lead"] : null;
            this.LeadEmailWorkbinConfig = (Settings.EmailActivityLogCollection.ContainsKey("workbin")) ? Settings.EmailActivityLogCollection["workbin"] : null;
            this.leadEmailOptions = Settings.LeadEmailOptions;
            this.sfdcUtilityHelper = SFDCUtiltiyHelper.GetInstance();
        }

        #endregion Constructor

        #region GetInstance Method

        public static SFDCLead GetInstance()
        {
            if (leadData == null)
            {
                leadData = new SFDCLead();
            }
            return leadData;
        }

        #endregion GetInstance Method

        #region GetLeadVoicePopupData

        public LeadData GetLeadVoicePopupData(IMessage message, SFDCCallType callType)
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
                        LeadData lead = new LeadData();

                        #region Collect Lead Data

                        lead.SearchData = this.sfdcUtilityHelper.GetVoiceSearchValue(leadVoiceOptions, message, callType);
                        lead.ObjectName = leadVoiceOptions.ObjectName;
                        lead.NoRecordFound = SFDCObjectHelper.GetNoRecordFoundAction(callType, leadVoiceOptions);
                        lead.MultipleMatchRecord = SFDCObjectHelper.GetMultiMatchRecordAction(callType, leadVoiceOptions);
                        lead.NewRecordFieldIds = leadVoiceOptions.NewrecordFieldIds;
                        lead.SearchCondition = leadVoiceOptions.SearchCondition;
                        lead.CreateLogForNewRecord = leadVoiceOptions.CanCreateLogForNewRecordCreate;
                        lead.MaxRecordOpenCount = leadVoiceOptions.MaxNosRecordOpen;
                        lead.SearchpageMode = leadVoiceOptions.SearchPageMode;
                        lead.PhoneNumberSearchFormat = leadVoiceOptions.PhoneNumberSearchFormat;
                        lead.CanCreateNoRecordActivityLog = SFDCObjectHelper.GetCanCreateProfileActivity(callType, leadVoiceOptions, true);
                        lead.CanPopupNoRecordActivityLog = SFDCObjectHelper.GetCanPopupProfileActivity(callType, leadVoiceOptions, true);
                        lead.CanCreateMultiMatchActivityLog = SFDCObjectHelper.GetCanCreateProfileActivity(callType, leadVoiceOptions);
                        lead.CanPopupMultiMatchActivityLog = SFDCObjectHelper.GetCanPopupProfileActivity(callType, leadVoiceOptions);
                        lead.CanCreateProfileActivityforInbNoRecord = leadVoiceOptions.CanCreateProfileActivityforInbNoRecord;
                        lead.CanCreateProfileActivityforOutNoRecord = leadVoiceOptions.CanCreateProfileActivityforOutNoRecord;
                        lead.CanCreateProfileActivityforConNoRecord = leadVoiceOptions.CanCreateProfileActivityforConNoRecord;
                        if (lead.NoRecordFound.Equals("createnew") && this.LeadVoiceRecordConfig != null)
                        {
                            lead.CreateRecordFieldData = this.sfdcUtility.GetCreateActivityLogData(this.LeadVoiceRecordConfig, message, callType);
                        }

                        switch (callType)
                        {
                            case SFDCCallType.InboundVoice:
                                lead.CreateActvityLog = leadVoiceOptions.InboundCanCreateLog;
                                if (leadVoiceOptions.InboundCanCreateLog && this.LeadLogConfig != null)
                                {
                                    lead.ActivityLogData = this.sfdcUtility.GetCreateActivityLogData(this.LeadLogConfig, popupEvent, callType);
                                }
                                break;

                            case SFDCCallType.OutboundVoiceSuccess:
                                lead.CreateActvityLog = leadVoiceOptions.OutboundCanCreateLog;
                                if (leadVoiceOptions.OutboundCanCreateLog && this.LeadLogConfig != null)
                                {
                                    lead.ActivityLogData = this.sfdcUtility.GetCreateActivityLogData(this.LeadLogConfig, popupEvent, callType);
                                }
                                break;

                            case SFDCCallType.OutboundVoiceFailure:
                                lead.CreateActvityLog = leadVoiceOptions.OutboundFailureCanCreateLog;
                                if (leadVoiceOptions.OutboundFailureCanCreateLog && this.LeadLogConfig != null)
                                {
                                    lead.ActivityLogData = this.sfdcUtility.GetCreateActivityLogData(this.LeadLogConfig, popupEvent, callType);
                                }
                                break;

                            case SFDCCallType.ConsultVoiceReceived:
                                lead.CreateActvityLog = leadVoiceOptions.ConsultCanCreateLog;
                                if (leadVoiceOptions.ConsultCanCreateLog && this.LeadLogConfig != null)
                                {
                                    lead.ActivityLogData = this.sfdcUtility.GetCreateActivityLogData(this.LeadLogConfig, popupEvent, callType);
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

        #endregion GetLeadVoicePopupData

        #region GetLeadEmailPopupData

        /// <summary>
        /// Get Account Popup Data for Email
        /// </summary>
        /// <param name="emailData"></param>
        /// <param name="callType"></param>
        /// <returns></returns>
        public LeadData GetLeadEmailPopupData(IXNCustomData emailData, SFDCCallType callType)
        {
            try
            {
                this.logger.Info("GetLeadEmailPopupData :  Reading Account Popup Data.....");
                this.logger.Info("GetLeadEmailPopupData :  CallType Name : " + callType.ToString());

                if (emailData != null)
                {
                    LeadData _lead = new LeadData();

                    #region Collect Lead Data

                    _lead.SearchData = this.sfdcUtilityHelper.GetEmailSearchValue(leadEmailOptions, emailData, callType);
                    _lead.ObjectName = leadEmailOptions.ObjectName;
                    _lead.NoRecordFound = SFDCObjectHelper.GetNoRecordFoundAction(callType, leadEmailOptions);
                    _lead.MultipleMatchRecord = SFDCObjectHelper.GetMultiMatchRecordAction(callType, leadEmailOptions);
                    _lead.NewRecordFieldIds = leadEmailOptions.NewrecordFieldIds;
                    _lead.SearchCondition = leadEmailOptions.SearchCondition;
                    _lead.CreateLogForNewRecord = leadEmailOptions.CanCreateLogForNewRecordCreate;
                    _lead.MaxRecordOpenCount = leadEmailOptions.MaxNosRecordOpen;
                    _lead.SearchpageMode = leadEmailOptions.SearchPageMode;
                    _lead.PhoneNumberSearchFormat = leadEmailOptions.PhoneNumberSearchFormat;
                    _lead.CanCreateNoRecordActivityLog = SFDCObjectHelper.GetCanCreateProfileActivity(callType, leadEmailOptions, true);
                    _lead.CanPopupNoRecordActivityLog = SFDCObjectHelper.GetCanPopupProfileActivity(callType, leadEmailOptions, true);
                    _lead.CanCreateMultiMatchActivityLog = SFDCObjectHelper.GetCanCreateProfileActivity(callType, leadEmailOptions);
                    _lead.CanPopupMultiMatchActivityLog = SFDCObjectHelper.GetCanPopupProfileActivity(callType, leadEmailOptions);
                    _lead.CanCreateProfileActivityforInbNoRecord = leadEmailOptions.CanCreateProfileActivityforInbNoRecord;
                    _lead.CanCreateProfileActivityforOutNoRecord = leadEmailOptions.CanCreateProfileActivityforOutNoRecord;

                    if (_lead.NoRecordFound.Equals("createnew") && this.LeadEmailRecordConfig != null)
                    {
                        _lead.CreateRecordFieldData = this.sfdcUtility.GetCreateActivityLogData(this.LeadEmailRecordConfig, null, callType, emailData);
                    }
                    if (callType == SFDCCallType.InboundEmail || callType == SFDCCallType.InboundEmailPulled)
                    {
                        if (leadEmailOptions.InboundCanCreateLog)
                        {
                            _lead.CreateActvityLog = true;
                            _lead.ActivityLogData = this.sfdcUtility.GetCreateActivityLogData(this.LeadEmailLogConfig, null, callType, emailData);
                        }
                    }
                    else if (callType == SFDCCallType.OutboundEmailFailure || callType == SFDCCallType.OutboundEmailSuccess || callType == SFDCCallType.OutboundEmailPulled)
                    {
                        if (leadEmailOptions.OutboundCanCreateLog)
                        {
                            _lead.CreateActvityLog = true;
                            _lead.ActivityLogData = this.sfdcUtility.GetCreateActivityLogData(this.LeadEmailLogConfig, null, callType, emailData);
                        }
                    }

                    #endregion Collect Lead Data

                    return _lead;
                }
            }
            catch (Exception generalException)
            {
                this.logger.Error("GetLeadEmailPopupData : Error occurred while reading Account Data : " + generalException.ToString());
            }
            return null;
        }

        #endregion GetLeadEmailPopupData

        #region GetLeadChatPopupData

        public LeadData GetLeadChatPopupData(IXNCustomData chatData, SFDCCallType callType)
        {
            try
            {
                IMessage message = chatData.InteractionEvent;
                this.logger.Info("GetLeadChatPopupData :  Reading Lead Popup Data.....");
                this.logger.Info("GetLeadChatPopupData :  Event Name : " + message.Name);
                if (this.leadChatOptions != null)
                {
                    dynamic popupEvent = Convert.ChangeType(message, message.GetType());
                    if (popupEvent != null)
                    {
                        LeadData lead = new LeadData();

                        #region Collect Lead Data

                        lead.SearchData = this.sfdcUtilityHelper.GetChatSearchValue(leadChatOptions, chatData, callType);
                        lead.ObjectName = leadChatOptions.ObjectName;
                        lead.NoRecordFound = SFDCObjectHelper.GetNoRecordFoundAction(callType, leadChatOptions);
                        lead.MultipleMatchRecord = SFDCObjectHelper.GetMultiMatchRecordAction(callType, leadChatOptions);
                        lead.NewRecordFieldIds = leadChatOptions.NewrecordFieldIds;
                        lead.SearchCondition = leadChatOptions.SearchCondition;
                        lead.CreateLogForNewRecord = leadChatOptions.CanCreateLogForNewRecordCreate;
                        lead.MaxRecordOpenCount = leadChatOptions.MaxNosRecordOpen;
                        lead.SearchpageMode = leadChatOptions.SearchPageMode;
                        lead.PhoneNumberSearchFormat = leadChatOptions.PhoneNumberSearchFormat;
                        lead.CanCreateNoRecordActivityLog = SFDCObjectHelper.GetCanCreateProfileActivity(callType, leadChatOptions, true);
                        lead.CanPopupNoRecordActivityLog = SFDCObjectHelper.GetCanPopupProfileActivity(callType, leadChatOptions, true);
                        lead.CanCreateMultiMatchActivityLog = SFDCObjectHelper.GetCanCreateProfileActivity(callType, leadChatOptions);
                        lead.CanPopupMultiMatchActivityLog = SFDCObjectHelper.GetCanPopupProfileActivity(callType, leadChatOptions);
                        lead.CanCreateProfileActivityforInbNoRecord = leadChatOptions.CanCreateProfileActivityforInbNoRecord;
                        if (lead.NoRecordFound.Equals("createnew") && this.LeadChatRecordConfig != null)
                        {
                            lead.CreateRecordFieldData = this.sfdcUtility.GetCreateActivityLogData(this.LeadChatRecordConfig, message, callType, emailData: chatData);
                        }
                        if (callType == SFDCCallType.InboundChat)
                        {
                            lead.CreateActvityLog = leadChatOptions.InboundCanCreateLog;
                            if (leadChatOptions.InboundCanCreateLog && this.LeadChatLogConfig != null)
                            {
                                lead.ActivityLogData = this.sfdcUtility.GetCreateActivityLogData(this.LeadChatLogConfig, popupEvent, callType, emailData: chatData);
                            }
                        }
                        else if (callType == SFDCCallType.ConsultChatReceived)
                        {
                            lead.CreateActvityLog = leadChatOptions.ConsultCanCreateLog;
                            if (leadChatOptions.ConsultCanCreateLog && this.LeadLogConfig != null)
                            {
                                lead.ActivityLogData = this.sfdcUtility.GetCreateActivityLogData(this.LeadLogConfig, popupEvent, callType, emailData: chatData);
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

        #endregion GetLeadChatPopupData

        #region GetLeadVoiceUpdateData

        public LeadData GetLeadVoiceUpdateData(IMessage message, string eventName, SFDCCallType callType, string duration, string notes)
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
                        LeadData lead = new LeadData();

                        #region Collect Lead Data

                        lead.ObjectName = leadVoiceOptions.ObjectName;

                        if (callType == SFDCCallType.InboundVoice)
                        {
                            if (leadVoiceOptions.InboundCanUpdateLog)
                            {
                                lead.UpdateActivityLog = true;
                                lead.UpdateActivityLogData = this.sfdcUtility.GetUpdateActivityLogData(this.LeadLogConfig, popupEvent, callType, duration, voiceComments: notes);
                                if (!string.IsNullOrWhiteSpace(leadVoiceOptions.VoiceAppendActivityLogEventNames) && leadVoiceOptions.VoiceAppendActivityLogEventNames.Contains(eventName))
                                    lead.AppendActivityLogData = this.sfdcUtility.GetUpdateActivityLogData(this.LeadLogConfig, null, callType, duration, voiceComments: notes, isAppendLogData: true);
                            }
                        }
                        else if (callType == SFDCCallType.OutboundVoiceSuccess || callType == SFDCCallType.OutboundVoiceFailure)
                        {
                            if (leadVoiceOptions.OutboundCanUpdateLog)
                            {
                                lead.UpdateActivityLog = true;
                                lead.UpdateActivityLogData = this.sfdcUtility.GetUpdateActivityLogData(this.LeadLogConfig, popupEvent, callType, duration, voiceComments: notes);
                                if (!string.IsNullOrWhiteSpace(leadVoiceOptions.VoiceAppendActivityLogEventNames) && leadVoiceOptions.VoiceAppendActivityLogEventNames.Contains(eventName))
                                    lead.AppendActivityLogData = this.sfdcUtility.GetUpdateActivityLogData(this.LeadLogConfig, null, callType, duration, voiceComments: notes, isAppendLogData: true);
                            }
                        }
                        else if (callType == SFDCCallType.ConsultVoiceReceived)
                        {
                            if (leadVoiceOptions.ConsultCanUpdateLog)
                            {
                                lead.UpdateActivityLog = true;
                                lead.UpdateActivityLogData = this.sfdcUtility.GetUpdateActivityLogData(this.LeadLogConfig, popupEvent, callType, duration, voiceComments: notes);
                                if (!string.IsNullOrWhiteSpace(leadVoiceOptions.VoiceAppendActivityLogEventNames) && leadVoiceOptions.VoiceAppendActivityLogEventNames.Contains(eventName))
                                    lead.AppendActivityLogData = this.sfdcUtility.GetUpdateActivityLogData(this.LeadLogConfig, null, callType, duration, voiceComments: notes, isAppendLogData: true);
                            }
                        }
                        if (leadVoiceOptions.CanUpdateRecordData)
                        {
                            lead.UpdateRecordFields = true;
                            lead.UpdateRecordFieldsData = this.sfdcUtility.GetUpdateActivityLogData(this.LeadVoiceRecordConfig, popupEvent, callType, duration);
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

        #endregion GetLeadVoiceUpdateData

        #region GetLeadEmailUpdateData

        /// <summary>
        /// Gets the Email Update data.
        /// </summary>
        /// <param name="emailData"></param>
        /// <param name="callType"></param>
        /// <param name="duration"></param>
        /// <param name="emailContent"></param>
        /// <returns></returns>
        public LeadData GetLeadEmailUpdateData(IXNCustomData emailData, SFDCCallType callType, string eventName)
        {
            try
            {
                this.logger.Info("GetLeadEmailUpdateData :  Reading Account Update Data.....");

                if (emailData != null)
                {
                    LeadData _lead = new LeadData();

                    #region Collect Lead Data

                    _lead.ObjectName = leadEmailOptions.ObjectName;
                    if (callType == SFDCCallType.InboundEmail || callType == SFDCCallType.InboundEmailPulled)
                    {
                        if (leadEmailOptions.InboundCanUpdateLog)
                        {
                            _lead.UpdateActivityLog = true;
                            _lead.UpdateActivityLogData = this.sfdcUtility.GetUpdateActivityLogData(this.LeadEmailLogConfig, null, callType, emailData.Duration, emailData: emailData);
                            if (!string.IsNullOrWhiteSpace(leadEmailOptions.EmailAppendActivityLogEventNames) && leadEmailOptions.EmailAppendActivityLogEventNames.Contains(eventName))
                                _lead.AppendActivityLogData = this.sfdcUtility.GetUpdateActivityLogData(this.LeadEmailLogConfig, null, callType, emailData.Duration, emailData: emailData, isAppendLogData: true);
                        }
                    }
                    else if (callType == SFDCCallType.OutboundEmailFailure || callType == SFDCCallType.OutboundEmailSuccess || callType == SFDCCallType.OutboundEmailPulled)
                    {
                        if (leadEmailOptions.OutboundCanUpdateLog)
                        {
                            _lead.UpdateActivityLog = true;
                            _lead.UpdateActivityLogData = this.sfdcUtility.GetUpdateActivityLogData(this.LeadEmailLogConfig, null, callType, emailData.Duration, emailData: emailData);
                            if (!string.IsNullOrWhiteSpace(leadEmailOptions.EmailAppendActivityLogEventNames) && leadEmailOptions.EmailAppendActivityLogEventNames.Contains(eventName))
                                _lead.AppendActivityLogData = this.sfdcUtility.GetUpdateActivityLogData(this.LeadEmailLogConfig, null, callType, emailData.Duration, emailData: emailData, isAppendLogData: true);
                        }
                    }
                    //update account record fields
                    _lead.UpdateRecordFields = leadEmailOptions.CanUpdateRecordData;
                    _lead.NoRecordFound = SFDCObjectHelper.GetNoRecordFoundAction(callType, leadEmailOptions);
                    if (_lead.NoRecordFound.Equals("createnew") && this.LeadEmailRecordConfig != null)
                    {
                        if (leadEmailOptions.CanUpdateRecordData)
                        {
                            _lead.UpdateRecordFieldsData = this.sfdcUtility.GetUpdateActivityLogData(this.LeadEmailRecordConfig, null, callType, emailData.Duration, emailData: emailData);
                        }
                    }

                    #endregion Collect Lead Data

                    return _lead;
                }
            }
            catch (Exception generalException)
            {
                this.logger.Error("GetLeadEmailUpdateData : Error occurred while reading Account Data : " + generalException.ToString());
            }
            return null;
        }

        #endregion GetLeadEmailUpdateData

        #region GetLeadChatUpdateData

        // public LeadData GetLeadChatUpdateData(IMessage message, string eventName, SFDCCallType callType, string duration, string chatContent)
        public LeadData GetLeadChatUpdateData(IXNCustomData chatData, string eventName)
        {
            try
            {
                this.logger.Info("GetLeadChatUpdateData :  Reading Lead Update Data.....");
                this.logger.Info("GetLeadChatUpdateData :  Event Name : " + chatData.EventName);
                if (this.leadChatOptions != null)
                {
                    dynamic popupEvent = Convert.ChangeType(chatData.InteractionEvent, chatData.InteractionEvent.GetType());

                    if (popupEvent != null)
                    {
                        LeadData lead = new LeadData();

                        #region Collect Lead Data

                        lead.ObjectName = leadChatOptions.ObjectName;

                        if (chatData.InteractionType == SFDCCallType.InboundChat)
                        {
                            if (leadChatOptions.InboundCanUpdateLog)
                            {
                                lead.UpdateActivityLog = true;
                                lead.UpdateActivityLogData = this.sfdcUtility.GetUpdateActivityLogData(this.LeadChatLogConfig, popupEvent, chatData.InteractionType, chatData.Duration, emailData: chatData);
                                if (!string.IsNullOrWhiteSpace(leadChatOptions.ChatAppendActivityLogEventNames) && leadChatOptions.ChatAppendActivityLogEventNames.Contains(eventName))
                                    lead.AppendActivityLogData = this.sfdcUtility.GetUpdateActivityLogData(this.LeadChatLogConfig, null, chatData.InteractionType, chatData.Duration, emailData: chatData, isAppendLogData: true);
                            }
                        }
                        if (chatData.InteractionType == SFDCCallType.ConsultChatReceived)
                        {
                            if (leadChatOptions.ConsultCanUpdateLog)
                            {
                                lead.UpdateActivityLog = true;
                                lead.UpdateActivityLogData = this.sfdcUtility.GetUpdateActivityLogData(this.LeadChatLogConfig, popupEvent, chatData.InteractionType, chatData.Duration, emailData: chatData);
                                if (!string.IsNullOrWhiteSpace(leadChatOptions.ChatAppendActivityLogEventNames) && leadChatOptions.ChatAppendActivityLogEventNames.Contains(eventName))
                                    lead.AppendActivityLogData = this.sfdcUtility.GetUpdateActivityLogData(this.LeadChatLogConfig, null, chatData.InteractionType, chatData.Duration, emailData: chatData, isAppendLogData: true);
                            }
                        }
                        //update lead record fields
                        lead.UpdateRecordFields = leadChatOptions.CanUpdateRecordData;
                        if (SFDCObjectHelper.GetNoRecordFoundAction(chatData.InteractionType, leadChatOptions).Equals("createnew") && this.LeadChatRecordConfig != null)
                        {
                            if (leadChatOptions.CanUpdateRecordData)
                            {
                                lead.UpdateRecordFieldsData = this.sfdcUtility.GetUpdateActivityLogData(this.LeadChatRecordConfig, popupEvent, chatData.InteractionType, chatData.Duration, emailData: chatData);
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

        #endregion GetLeadChatUpdateData
    }
}