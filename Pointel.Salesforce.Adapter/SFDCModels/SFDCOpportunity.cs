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
    /// Comment: Provides Search and Activity Data for Opportunity Object Last Modified: 25-08-2015
    /// Created by: Pointel Inc
    /// </summary>
    internal class SFDCOpportunity
    {
        #region Field Declaration

        private static SFDCOpportunity _sfdcOpportunity = null;
        private Log _logger = null;
        private KeyValueCollection _opportunityChatLogConfig = null;
        private ChatOptions _opportunityChatOptions = null;
        private KeyValueCollection _opportunityEmailLogConfig = null;
        private EmailOptions _opportunityEmailOptions = null;
        private KeyValueCollection _opportunityLogConfig = null;
        private KeyValueCollection _opportunityEmailRecordConfig = null;
        private KeyValueCollection _opportunityVoiceRecordConfig = null;
        private KeyValueCollection _opportunityChatRecordConfig = null;
        private VoiceOptions _opportunityVoiceOptions = null;
        private SFDCUtility _sfdcUtility = null;
        private SFDCUtiltiyHelper _sfdcUtilityHelper = null;

        #endregion Field Declaration

        #region Constructor

        private SFDCOpportunity()
        {
            this._logger = Log.GenInstance();
            this._opportunityVoiceOptions = Settings.OpportunityVoiceOptions;
            this._opportunityChatOptions = Settings.OpportunityChatOptions;
            this._opportunityLogConfig = (Settings.VoiceActivityLogCollection.ContainsKey("opportunity")) ? Settings.VoiceActivityLogCollection["opportunity"] : null;
            this._opportunityChatLogConfig = (Settings.ChatActivityLogCollection.ContainsKey("opportunity")) ? Settings.ChatActivityLogCollection["opportunity"] : null;
            this._opportunityEmailRecordConfig = (Settings.EmailNewRecordCollection.ContainsKey("opportunity")) ? Settings.EmailNewRecordCollection["opportunity"] : null;
            this._opportunityVoiceRecordConfig = (Settings.VoiceNewRecordCollection.ContainsKey("opportunity")) ? Settings.VoiceNewRecordCollection["opportunity"] : null;
            this._opportunityChatRecordConfig = (Settings.ChatNewRecordCollection.ContainsKey("opportunity")) ? Settings.ChatNewRecordCollection["opportunity"] : null;
            this._sfdcUtility = SFDCUtility.GetInstance();
            this._sfdcUtilityHelper = SFDCUtiltiyHelper.GetInstance();
            this._opportunityEmailLogConfig = (Settings.EmailActivityLogCollection.ContainsKey("opportunity")) ? Settings.EmailActivityLogCollection["opportunity"] : null;
            this._opportunityEmailOptions = Settings.OpportunityEmailOptions;
        }

        #endregion Constructor

        #region GetInstance Method

        public static SFDCOpportunity GetInstance()
        {
            if (_sfdcOpportunity == null)
            {
                _sfdcOpportunity = new SFDCOpportunity();
            }
            return _sfdcOpportunity;
        }

        #endregion GetInstance Method

        #region GetOpportunityVoicePopupData

        public OpportunityData GetOpportunityVoicePopupData(IMessage message, SFDCCallType callType)
        {
            try
            {
                this._logger.Info("GetOpportunityVoicePopupData :  Reading Opportunity Popup Data.....");
                this._logger.Info("GetOpportunityVoicePopupData :  Event Name : " + message.Name);
                this._logger.Info("GetOpportunityVoicePopupData :  CallType Name : " + callType.ToString());
                dynamic _popupEvent = Convert.ChangeType(message, message.GetType());
                if (_popupEvent != null)
                {
                    OpportunityData _opportunity = new OpportunityData();
                    _opportunity.SearchData = this._sfdcUtilityHelper.GetVoiceSearchValue(_opportunityVoiceOptions, message, callType);
                    _opportunity.ObjectName = _opportunityVoiceOptions.ObjectName;
                    _opportunity.NoRecordFound = SFDCObjectHelper.GetNoRecordFoundAction(callType, _opportunityVoiceOptions);
                    _opportunity.MultipleMatchRecord = SFDCObjectHelper.GetMultiMatchRecordAction(callType, _opportunityVoiceOptions);
                    _opportunity.NewRecordFieldIds = _opportunityVoiceOptions.NewrecordFieldIds;
                    _opportunity.SearchCondition = _opportunityVoiceOptions.SearchCondition;
                    _opportunity.CreateLogForNewRecord = _opportunityVoiceOptions.CanCreateLogForNewRecordCreate;
                    _opportunity.MaxRecordOpenCount = _opportunityVoiceOptions.MaxNosRecordOpen;
                    _opportunity.SearchpageMode = _opportunityVoiceOptions.SearchPageMode;
                    _opportunity.PhoneNumberSearchFormat = _opportunityVoiceOptions.PhoneNumberSearchFormat;
                    _opportunity.CanCreateNoRecordActivityLog = SFDCObjectHelper.GetCanCreateProfileActivity(callType, _opportunityVoiceOptions, true);
                    _opportunity.CanPopupNoRecordActivityLog = SFDCObjectHelper.GetCanPopupProfileActivity(callType, _opportunityVoiceOptions, true);
                    _opportunity.CanCreateMultiMatchActivityLog = SFDCObjectHelper.GetCanCreateProfileActivity(callType, _opportunityVoiceOptions);
                    _opportunity.CanPopupMultiMatchActivityLog = SFDCObjectHelper.GetCanPopupProfileActivity(callType, _opportunityVoiceOptions);
                    _opportunity.CanCreateProfileActivityforInbNoRecord = _opportunityVoiceOptions.CanCreateProfileActivityforInbNoRecord;
                    _opportunity.CanCreateProfileActivityforOutNoRecord = _opportunityVoiceOptions.CanCreateProfileActivityforOutNoRecord;
                    _opportunity.CanCreateProfileActivityforConNoRecord = _opportunityVoiceOptions.CanCreateProfileActivityforConNoRecord;
                    if (_opportunity.NoRecordFound.Equals("createnew") && this._opportunityVoiceRecordConfig != null)
                    {
                        _opportunity.CreateRecordFieldData = this._sfdcUtility.GetCreateActivityLogData(this._opportunityVoiceRecordConfig, message, callType);
                    }
                    if (callType == SFDCCallType.InboundVoice)
                    {
                        if (_opportunityVoiceOptions.InboundCanCreateLog)
                        {
                            _opportunity.CreateActvityLog = true;
                            _opportunity.ActivityLogData = this._sfdcUtility.GetCreateActivityLogData(this._opportunityLogConfig, _popupEvent, callType);
                        }
                    }
                    else if (callType == SFDCCallType.OutboundVoiceSuccess)
                    {
                        if (_opportunityVoiceOptions.OutboundCanCreateLog)
                        {
                            _opportunity.CreateActvityLog = true;
                            _opportunity.ActivityLogData = this._sfdcUtility.GetCreateActivityLogData(this._opportunityLogConfig, _popupEvent, callType);
                        }
                    }
                    else if (callType == SFDCCallType.OutboundVoiceFailure)
                    {
                        if (_opportunityVoiceOptions.OutboundFailureCanCreateLog)
                        {
                            _opportunity.CreateActvityLog = true;
                            _opportunity.ActivityLogData = this._sfdcUtility.GetCreateActivityLogData(this._opportunityLogConfig, _popupEvent, callType);
                        }
                    }
                    else if (callType == SFDCCallType.ConsultVoiceReceived)
                    {
                        if (_opportunityVoiceOptions.ConsultCanCreateLog)
                        {
                            _opportunity.CreateActvityLog = true;
                            _opportunity.ActivityLogData = this._sfdcUtility.GetCreateActivityLogData(this._opportunityLogConfig, _popupEvent, callType);
                        }
                    }
                    return _opportunity;
                }
            }
            catch (Exception generalException)
            {
                this._logger.Error("GetOpportunityVoicePopupData : Error occurred while reading Opportunity Data : " + generalException.ToString());
            }
            return null;
        }

        #endregion GetOpportunityVoicePopupData

        #region GetOpportunityEmailPopupData

        /// <summary>
        /// Get Account Popup Data for Email
        /// </summary>
        /// <param name="emailData"></param>
        /// <param name="callType"></param>
        /// <returns></returns>
        public OpportunityData GetOpportunityEmailPopupData(IXNCustomData emailData, SFDCCallType callType)
        {
            try
            {
                this._logger.Info("GetLeadEmailPopupData :  Reading Account Popup Data.....");
                this._logger.Info("GetLeadEmailPopupData :  CallType Name : " + callType.ToString());

                if (emailData != null)
                {
                    OpportunityData _opportunity = new OpportunityData();

                    #region Collect opportunity Data

                    _opportunity.SearchData = this._sfdcUtilityHelper.GetEmailSearchValue(_opportunityEmailOptions, emailData, callType);
                    _opportunity.ObjectName = _opportunityEmailOptions.ObjectName;
                    _opportunity.NoRecordFound = SFDCObjectHelper.GetNoRecordFoundAction(callType, _opportunityEmailOptions);
                    _opportunity.MultipleMatchRecord = SFDCObjectHelper.GetMultiMatchRecordAction(callType, _opportunityEmailOptions);
                    _opportunity.NewRecordFieldIds = _opportunityEmailOptions.NewrecordFieldIds;
                    _opportunity.SearchCondition = _opportunityEmailOptions.SearchCondition;
                    _opportunity.CreateLogForNewRecord = _opportunityEmailOptions.CanCreateLogForNewRecordCreate;
                    _opportunity.MaxRecordOpenCount = _opportunityEmailOptions.MaxNosRecordOpen;
                    _opportunity.SearchpageMode = _opportunityEmailOptions.SearchPageMode;
                    _opportunity.PhoneNumberSearchFormat = _opportunityEmailOptions.PhoneNumberSearchFormat;
                    _opportunity.CanCreateNoRecordActivityLog = SFDCObjectHelper.GetCanCreateProfileActivity(callType, _opportunityEmailOptions, true);
                    _opportunity.CanPopupNoRecordActivityLog = SFDCObjectHelper.GetCanPopupProfileActivity(callType, _opportunityEmailOptions, true);
                    _opportunity.CanCreateMultiMatchActivityLog = SFDCObjectHelper.GetCanCreateProfileActivity(callType, _opportunityEmailOptions);
                    _opportunity.CanPopupMultiMatchActivityLog = SFDCObjectHelper.GetCanPopupProfileActivity(callType, _opportunityEmailOptions);
                    _opportunity.CanCreateProfileActivityforInbNoRecord = _opportunityEmailOptions.CanCreateProfileActivityforInbNoRecord;
                    _opportunity.CanCreateProfileActivityforOutNoRecord = _opportunityEmailOptions.CanCreateProfileActivityforOutNoRecord;
                    // _opportunity.CanCreateProfileActivityforConNoRecord = _opportunityEmailOptions.CanCreateProfileActivityforConNoRecord;
                    if (_opportunity.NoRecordFound.Equals("createnew") && this._opportunityEmailRecordConfig != null)
                    {
                        _opportunity.CreateRecordFieldData = this._sfdcUtility.GetCreateActivityLogData(this._opportunityEmailRecordConfig, null, callType, emailData);
                    }
                    if (callType == SFDCCallType.InboundEmail || callType == SFDCCallType.InboundEmailPulled)
                    {
                        if (_opportunityEmailOptions.InboundCanCreateLog)
                        {
                            _opportunity.CreateActvityLog = true;
                            _opportunity.ActivityLogData = this._sfdcUtility.GetCreateActivityLogData(this._opportunityLogConfig, null, callType, emailData);
                        }
                    }
                    else if (callType == SFDCCallType.OutboundEmailFailure || callType == SFDCCallType.OutboundEmailSuccess || callType == SFDCCallType.OutboundEmailPulled)
                    {
                        if (_opportunityEmailOptions.OutboundCanCreateLog)
                        {
                            _opportunity.CreateActvityLog = true;
                            _opportunity.ActivityLogData = this._sfdcUtility.GetCreateActivityLogData(this._opportunityLogConfig, null, callType, emailData);
                        }
                    }

                    #endregion Collect opportunity Data

                    return _opportunity;
                }
            }
            catch (Exception generalException)
            {
                this._logger.Error("GetLeadEmailPopupData : Error occurred while reading Account Data : " + generalException.ToString());
            }
            return null;
        }

        #endregion GetOpportunityEmailPopupData

        #region GetOpportunityChatPopupData

        public OpportunityData GetOpportunityChatPopupData(IXNCustomData chatData, SFDCCallType callType)
        {
            try
            {
                IMessage message = chatData.InteractionEvent;
                this._logger.Info("GetOpportunityChatPopupData :  Reading Opportunity Popup Data.....");
                this._logger.Info("GetOpportunityChatPopupData :  Event Name : " + message.Name);
                this._logger.Info("GetOpportunityChatPopupData :  CallType Name : " + callType.ToString());
                dynamic popupEvent = Convert.ChangeType(message, message.GetType());
                if (popupEvent != null)
                {
                    OpportunityData _opportunity = new OpportunityData();

                    #region Collect opportunity Data

                    _opportunity.SearchData = this._sfdcUtilityHelper.GetChatSearchValue(_opportunityChatOptions, chatData, callType);
                    _opportunity.ObjectName = _opportunityChatOptions.ObjectName;
                    _opportunity.NoRecordFound = SFDCObjectHelper.GetNoRecordFoundAction(callType, _opportunityChatOptions);
                    _opportunity.MultipleMatchRecord = SFDCObjectHelper.GetMultiMatchRecordAction(callType, _opportunityChatOptions);
                    _opportunity.NewRecordFieldIds = _opportunityChatOptions.NewrecordFieldIds;
                    _opportunity.SearchCondition = _opportunityChatOptions.SearchCondition;
                    _opportunity.CreateLogForNewRecord = _opportunityChatOptions.CanCreateLogForNewRecordCreate;
                    _opportunity.MaxRecordOpenCount = _opportunityChatOptions.MaxNosRecordOpen;
                    _opportunity.SearchpageMode = _opportunityChatOptions.SearchPageMode;
                    _opportunity.PhoneNumberSearchFormat = _opportunityChatOptions.PhoneNumberSearchFormat;
                    _opportunity.CanCreateNoRecordActivityLog = SFDCObjectHelper.GetCanCreateProfileActivity(callType, _opportunityChatOptions, true);
                    _opportunity.CanPopupNoRecordActivityLog = SFDCObjectHelper.GetCanPopupProfileActivity(callType, _opportunityChatOptions, true);
                    _opportunity.CanCreateMultiMatchActivityLog = SFDCObjectHelper.GetCanCreateProfileActivity(callType, _opportunityChatOptions);
                    _opportunity.CanPopupMultiMatchActivityLog = SFDCObjectHelper.GetCanPopupProfileActivity(callType, _opportunityChatOptions);
                    _opportunity.CanCreateProfileActivityforInbNoRecord = _opportunityChatOptions.CanCreateProfileActivityforInbNoRecord;
                    if (_opportunity.NoRecordFound.Equals("createnew") && this._opportunityChatRecordConfig != null)
                    {
                        _opportunity.CreateRecordFieldData = this._sfdcUtility.GetCreateActivityLogData(this._opportunityChatRecordConfig, message, callType, emailData: chatData);
                    }
                    if (callType == SFDCCallType.InboundChat)
                    {
                        if (_opportunityChatOptions.InboundCanCreateLog)
                        {
                            _opportunity.CreateActvityLog = true;
                            _opportunity.ActivityLogData = this._sfdcUtility.GetCreateActivityLogData(this._opportunityChatLogConfig, popupEvent, callType, emailData: chatData);
                        }
                    }
                    else if (callType == SFDCCallType.ConsultChatReceived)
                    {
                        if (_opportunityChatOptions.ConsultCanCreateLog)
                        {
                            _opportunity.CreateActvityLog = true;
                            _opportunity.ActivityLogData = this._sfdcUtility.GetCreateActivityLogData(this._opportunityChatLogConfig, popupEvent, callType, emailData: chatData);
                        }
                    }
                    return _opportunity;

                    #endregion Collect opportunity Data
                }
            }
            catch (Exception generalException)
            {
                this._logger.Error("GetOpportunityChatPopupData : Error occurred while reading Opportunity Data : " + generalException.ToString());
            }
            return null;
        }

        #endregion GetOpportunityChatPopupData

        #region GetOpportunityVoiceUpdateData

        public OpportunityData GetOpportunityVoiceUpdateData(IMessage message, string eventName, SFDCCallType callType, string duration, string notes)
        {
            try
            {
                this._logger.Info("GetOpportunityVoiceUpdateData :  Reading Opportunity Update Data.....");
                this._logger.Info("GetOpportunityVoiceUpdateData :  Event Name : " + message.Name);
                dynamic popupEvent = Convert.ChangeType(message, message.GetType());

                if (popupEvent != null)
                {
                    OpportunityData opportunity = new OpportunityData();

                    #region Collect opportunity Data

                    opportunity.ObjectName = _opportunityVoiceOptions.ObjectName;

                    if (callType == SFDCCallType.InboundVoice)
                    {
                        if (_opportunityVoiceOptions.InboundCanUpdateLog)
                        {
                            opportunity.UpdateActivityLog = true;
                            opportunity.UpdateActivityLogData = this._sfdcUtility.GetUpdateActivityLogData(this._opportunityLogConfig, popupEvent, callType, duration, voiceComments: notes);
                            if (!string.IsNullOrWhiteSpace(_opportunityVoiceOptions.VoiceAppendActivityLogEventNames) && _opportunityVoiceOptions.VoiceAppendActivityLogEventNames.Contains(eventName))
                                opportunity.AppendActivityLogData = this._sfdcUtility.GetUpdateActivityLogData(this._opportunityLogConfig, null, callType, duration, voiceComments: notes, isAppendLogData: true);
                        }
                    }
                    else if (callType == SFDCCallType.OutboundVoiceSuccess || callType == SFDCCallType.OutboundVoiceFailure)
                    {
                        if (_opportunityVoiceOptions.OutboundCanUpdateLog)
                        {
                            opportunity.UpdateActivityLog = true;
                            opportunity.UpdateActivityLogData = this._sfdcUtility.GetUpdateActivityLogData(this._opportunityLogConfig, popupEvent, callType, duration, voiceComments: notes);
                            if (!string.IsNullOrWhiteSpace(_opportunityVoiceOptions.VoiceAppendActivityLogEventNames) && _opportunityVoiceOptions.VoiceAppendActivityLogEventNames.Contains(eventName))
                                opportunity.AppendActivityLogData = this._sfdcUtility.GetUpdateActivityLogData(this._opportunityLogConfig, null, callType, duration, voiceComments: notes, isAppendLogData: true);
                        }
                    }
                    else if (callType == SFDCCallType.ConsultVoiceReceived)
                    {
                        if (_opportunityVoiceOptions.ConsultCanUpdateLog)
                        {
                            opportunity.UpdateActivityLog = true;
                            opportunity.UpdateActivityLogData = this._sfdcUtility.GetUpdateActivityLogData(this._opportunityLogConfig, popupEvent, callType, duration, voiceComments: notes);
                            if (!string.IsNullOrWhiteSpace(_opportunityVoiceOptions.VoiceAppendActivityLogEventNames) && _opportunityVoiceOptions.VoiceAppendActivityLogEventNames.Contains(eventName))
                                opportunity.AppendActivityLogData = this._sfdcUtility.GetUpdateActivityLogData(this._opportunityLogConfig, null, callType, duration, voiceComments: notes, isAppendLogData: true);
                        }
                    }

                    if (_opportunityVoiceOptions.CanUpdateRecordData)
                    {
                        opportunity.UpdateRecordFields = true;
                        opportunity.UpdateRecordFieldsData = this._sfdcUtility.GetUpdateActivityLogData(this._opportunityVoiceRecordConfig, popupEvent, callType, duration);
                    }

                    #endregion Collect opportunity Data

                    return opportunity;
                }
            }
            catch (Exception generalException)
            {
                this._logger.Error("GetOpportunityVoiceUpdateData : Error occurred while reading opportunity Data : " + generalException.ToString());
            }
            return null;
        }

        #endregion GetOpportunityVoiceUpdateData

        #region GetOpportunityChatUpdateData

        public OpportunityData GetOpportunityChatUpdateData(IXNCustomData chatData, string eventName)
        {
            try
            {
                IMessage message = chatData.InteractionEvent;
                SFDCCallType callType = chatData.InteractionType;
                string duration = chatData.Duration;
                this._logger.Info("GetOpportunityChatUpdateData :  Reading Opportunity Update Data.....");
                this._logger.Info("GetOpportunityChatUpdateData :  Event Name : " + message.Name);
                dynamic popupEvent = Convert.ChangeType(message, message.GetType());

                if (popupEvent != null)
                {
                    OpportunityData opportunity = new OpportunityData();

                    #region Collect opportunity Data

                    opportunity.ObjectName = _opportunityChatOptions.ObjectName;
                    if (callType == SFDCCallType.InboundChat)
                    {
                        if (_opportunityChatOptions.InboundCanUpdateLog)
                        {
                            opportunity.UpdateActivityLog = true;
                            opportunity.UpdateActivityLogData = this._sfdcUtility.GetUpdateActivityLogData(this._opportunityChatLogConfig, popupEvent, callType, duration, emailData: chatData);
                            if (!string.IsNullOrWhiteSpace(_opportunityChatOptions.ChatAppendActivityLogEventNames) && _opportunityChatOptions.ChatAppendActivityLogEventNames.Contains(eventName))
                                opportunity.AppendActivityLogData = this._sfdcUtility.GetUpdateActivityLogData(this._opportunityChatLogConfig, null, callType, duration, emailData: chatData, isAppendLogData: true);
                        }
                    }
                    else
                        if (callType == SFDCCallType.ConsultChatReceived)
                        {
                            if (_opportunityChatOptions.ConsultCanUpdateLog)
                            {
                                opportunity.UpdateActivityLog = true;
                                opportunity.UpdateActivityLogData = this._sfdcUtility.GetUpdateActivityLogData(this._opportunityChatLogConfig, popupEvent, callType, duration, emailData: chatData);
                                if (!string.IsNullOrWhiteSpace(_opportunityChatOptions.ChatAppendActivityLogEventNames) && _opportunityChatOptions.ChatAppendActivityLogEventNames.Contains(eventName))
                                    opportunity.AppendActivityLogData = this._sfdcUtility.GetUpdateActivityLogData(this._opportunityChatLogConfig, null, callType, duration, emailData: chatData, isAppendLogData: true);
                            }
                        }

                    //update opportunity record fields
                    opportunity.UpdateRecordFields = _opportunityChatOptions.CanUpdateRecordData;
                    if (SFDCObjectHelper.GetNoRecordFoundAction(callType, _opportunityChatOptions).Equals("createnew") && this._opportunityChatRecordConfig != null)
                    {
                        if (_opportunityChatOptions.CanUpdateRecordData)
                        {
                            opportunity.UpdateRecordFieldsData = this._sfdcUtility.GetUpdateActivityLogData(this._opportunityChatRecordConfig, popupEvent, callType, duration, emailData: chatData);
                        }
                    }

                    #endregion Collect opportunity Data

                    return opportunity;
                }
            }
            catch (Exception generalException)
            {
                this._logger.Error("GetOpportunityChatUpdateData : Error occurred while reading Opportunity Data : " + generalException.ToString());
            }
            return null;
        }

        #endregion GetOpportunityChatUpdateData

        #region GetOpportunityEmailUpdateData

        /// <summary>
        /// Gets the Chat Update data.
        /// </summary>
        /// <param name="emailData"></param>
        /// <param name="callType"></param>
        /// <param name="duration"></param>
        /// <param name="emailContent"></param>
        /// <returns></returns>
        public OpportunityData GetOpportunityEmailUpdateData(IXNCustomData emailData, SFDCCallType callType, string eventName)
        {
            OpportunityData opportunity = new OpportunityData();
            try
            {
                this._logger.Info("GetOpportunityOutboundEmailUpdateData :  Reading Account Update Data.....");

                if (emailData != null)
                {
                    #region Collect Lead Data

                    opportunity.ObjectName = _opportunityEmailOptions.ObjectName;
                    if (callType == SFDCCallType.InboundEmail || callType == SFDCCallType.InboundEmailPulled)
                    {
                        if (_opportunityEmailOptions.InboundCanUpdateLog)
                        {
                            opportunity.UpdateActivityLog = true;
                            opportunity.UpdateActivityLogData = this._sfdcUtility.GetUpdateActivityLogData(this._opportunityEmailLogConfig, null, callType, emailData.Duration, emailData: emailData);
                            if (!string.IsNullOrWhiteSpace(_opportunityEmailOptions.EmailAppendActivityLogEventNames) && _opportunityEmailOptions.EmailAppendActivityLogEventNames.Contains(eventName))
                                opportunity.AppendActivityLogData = this._sfdcUtility.GetUpdateActivityLogData(this._opportunityEmailLogConfig, null, callType, emailData.Duration, emailData: emailData, isAppendLogData: true);
                        }
                    }
                    else if (callType == SFDCCallType.OutboundEmailFailure || callType == SFDCCallType.OutboundEmailSuccess || callType == SFDCCallType.OutboundEmailPulled)
                    {
                        if (_opportunityEmailOptions.OutboundCanUpdateLog)
                        {
                            opportunity.UpdateActivityLog = true;
                            opportunity.UpdateActivityLogData = this._sfdcUtility.GetUpdateActivityLogData(this._opportunityEmailLogConfig, null, callType, emailData.Duration, emailData: emailData);
                            if (!string.IsNullOrWhiteSpace(_opportunityEmailOptions.EmailAppendActivityLogEventNames) && _opportunityEmailOptions.EmailAppendActivityLogEventNames.Contains(eventName))
                                opportunity.AppendActivityLogData = this._sfdcUtility.GetUpdateActivityLogData(this._opportunityEmailLogConfig, null, callType, emailData.Duration, emailData: emailData, isAppendLogData: true);
                        }
                    }
                    //update account record fields
                    opportunity.UpdateRecordFields = _opportunityEmailOptions.CanUpdateRecordData;
                    opportunity.NoRecordFound = SFDCObjectHelper.GetNoRecordFoundAction(callType, _opportunityEmailOptions);
                    if (opportunity.NoRecordFound.Equals("createnew") && this._opportunityEmailRecordConfig != null)
                    {
                        if (_opportunityEmailOptions.CanUpdateRecordData)
                        {
                            opportunity.UpdateRecordFieldsData = this._sfdcUtility.GetUpdateActivityLogData(this._opportunityEmailRecordConfig, null, callType, emailData.Duration, emailData: emailData);
                        }
                    }

                    #endregion Collect Lead Data

                    return opportunity;
                }
            }
            catch (Exception generalException)
            {
                this._logger.Error("GetOpportunityOutboundEmailUpdateData : Error occurred while reading Account Data : " + generalException.ToString());
                return opportunity;
            }
            return null;
        }

        #endregion GetOpportunityEmailUpdateData
    }
}