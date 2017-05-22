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
{/// <summary>
    /// Comment: Provides Search and Activity Data for Case Object Last Modified: 25-08-2015 Created
    /// by: Pointel Inc </summary>
    internal class SFDCCase
    {
        #region Field Declaration

        private static SFDCCase _caseData = null;
        private KeyValueCollection _caseChatLogOptions = null;
        private ChatOptions _caseChatOptions = null;
        private KeyValueCollection _caseEmailLogOptions = null;
        private EmailOptions _caseEmailOptions = null;
        private KeyValueCollection _caseLogConfig = null;
        private KeyValueCollection _caseEmailRecordConfig = null;
        private KeyValueCollection _caseVoiceRecordConfig = null;
        private KeyValueCollection _caseChatRecordConfig = null;
        private VoiceOptions _caseVoiceOptions = null;
        private Log _logger = null;
        private SFDCUtility _sfdcUtility = null;
        private SFDCUtiltiyHelper _sfdcUtilityHelper = null;

        #endregion Field Declaration

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        private SFDCCase()
        {
            this._logger = Log.GenInstance();
            this._caseVoiceOptions = Settings.CaseVoiceOptions;
            this._caseChatOptions = Settings.CaseChatOptions;
            this._caseLogConfig = (Settings.VoiceActivityLogCollection.ContainsKey("case")) ? Settings.VoiceActivityLogCollection["case"] : null;
            this._caseChatLogOptions = (Settings.ChatActivityLogCollection.ContainsKey("case")) ? Settings.ChatActivityLogCollection["case"] : null;
            this._caseEmailRecordConfig = (Settings.EmailNewRecordCollection.ContainsKey("case")) ? Settings.EmailNewRecordCollection["case"] : null;
            this._caseVoiceRecordConfig = (Settings.VoiceNewRecordCollection.ContainsKey("case")) ? Settings.VoiceNewRecordCollection["case"] : null;
            this._caseChatRecordConfig = (Settings.ChatNewRecordCollection.ContainsKey("case")) ? Settings.ChatNewRecordCollection["case"] : null;
            this._sfdcUtility = SFDCUtility.GetInstance();
            this._caseEmailOptions = Settings.CaseEmailOptions;
            this._caseEmailLogOptions = (Settings.EmailActivityLogCollection.ContainsKey("case")) ? Settings.EmailActivityLogCollection["case"] : null;
            this._sfdcUtilityHelper = SFDCUtiltiyHelper.GetInstance();
        }

        #endregion Constructor

        #region GetInstance Method

        /// <summary>
        /// Gets the Instance of the Class
        /// </summary>
        /// <returns></returns>
        public static SFDCCase GetInstance()
        {
            if (_caseData == null)
            {
                _caseData = new SFDCCase();
            }
            return _caseData;
        }

        #endregion GetInstance Method

        #region GetCaseVoicePopupData

        /// <summary>
        /// Gets Case Popup Data
        /// </summary>
        /// <param name="message"></param>
        /// <param name="callType"></param>
        /// <returns></returns>
        public CaseData GetCaseVoicePopupData(IMessage message, SFDCCallType callType)
        {
            try
            {
                this._logger.Info("GetCaseVoicePopupData :  Reading Case Popup Data.....");
                this._logger.Info("GetCaseVoicePopupData :  Event Name : " + message.Name);
                this._logger.Info("GetCaseVoicePopupData :  CallType Name : " + callType.ToString());
                dynamic popupEvent = Convert.ChangeType(message, message.GetType());
                if (popupEvent != null)
                {
                    CaseData cases = new CaseData();

                    #region Collect case Data

                    cases.SearchData = this._sfdcUtilityHelper.GetVoiceSearchValue(_caseVoiceOptions, message, callType);
                    cases.ObjectName = _caseVoiceOptions.ObjectName;
                    cases.NoRecordFound = SFDCObjectHelper.GetNoRecordFoundAction(callType, _caseVoiceOptions);
                    cases.MultipleMatchRecord = SFDCObjectHelper.GetMultiMatchRecordAction(callType, _caseVoiceOptions);
                    cases.NewRecordFieldIds = _caseVoiceOptions.NewrecordFieldIds;
                    cases.SearchCondition = _caseVoiceOptions.SearchCondition;
                    cases.CreateLogForNewRecord = _caseVoiceOptions.CanCreateLogForNewRecordCreate;
                    cases.MaxRecordOpenCount = _caseVoiceOptions.MaxNosRecordOpen;
                    cases.SearchpageMode = _caseVoiceOptions.SearchPageMode;
                    cases.PhoneNumberSearchFormat = _caseVoiceOptions.PhoneNumberSearchFormat;
                    cases.CanCreateNoRecordActivityLog = SFDCObjectHelper.GetCanCreateProfileActivity(callType, _caseVoiceOptions, true);
                    cases.CanPopupNoRecordActivityLog = SFDCObjectHelper.GetCanPopupProfileActivity(callType, _caseVoiceOptions, true);
                    cases.CanCreateMultiMatchActivityLog = SFDCObjectHelper.GetCanCreateProfileActivity(callType, _caseVoiceOptions);
                    cases.CanPopupMultiMatchActivityLog = SFDCObjectHelper.GetCanPopupProfileActivity(callType, _caseVoiceOptions);
                    cases.CanCreateProfileActivityforInbNoRecord = cases.CanCreateProfileActivityforInbNoRecord;

                    if (cases.NoRecordFound.Equals("createnew") && this._caseVoiceRecordConfig != null)
                    {
                        cases.CreateRecordFieldData = this._sfdcUtility.GetCreateActivityLogData(this._caseVoiceRecordConfig, message, callType);
                    }
                    if (callType == SFDCCallType.InboundVoice)
                    {
                        if (_caseVoiceOptions.InboundCanCreateLog)
                        {
                            cases.CreateActvityLog = true;
                            cases.ActivityLogData = this._sfdcUtility.GetCreateActivityLogData(this._caseLogConfig, popupEvent, callType);
                        }
                    }
                    else if (callType == SFDCCallType.OutboundVoiceSuccess)
                    {
                        if (_caseVoiceOptions.OutboundCanCreateLog)
                        {
                            cases.CreateActvityLog = true;
                            cases.ActivityLogData = this._sfdcUtility.GetCreateActivityLogData(this._caseLogConfig, popupEvent, callType);
                        }
                    }
                    else if (callType == SFDCCallType.OutboundVoiceFailure)
                    {
                        if (_caseVoiceOptions.OutboundFailureCanCreateLog)
                        {
                            cases.CreateActvityLog = true;
                            cases.ActivityLogData = this._sfdcUtility.GetCreateActivityLogData(this._caseLogConfig, popupEvent, callType);
                        }
                    }
                    else if (callType == SFDCCallType.ConsultVoiceReceived)
                    {
                        if (_caseVoiceOptions.ConsultCanCreateLog)
                        {
                            cases.CreateActvityLog = true;
                            cases.ActivityLogData = this._sfdcUtility.GetCreateActivityLogData(this._caseLogConfig, popupEvent, callType);
                        }
                    }
                    return cases;

                    #endregion Collect case Data
                }
            }
            catch (Exception generalException)
            {
                this._logger.Error("GetCaseVoicePopupData : Error occurred while reading Case Data : " + generalException.ToString());
            }
            return null;
        }

        #endregion GetCaseVoicePopupData

        #region GetCaseEmailPopupData

        /// <summary>
        /// Get Account Popup Data for Email
        /// </summary>
        /// <param name="emailData"></param>
        /// <param name="callType"></param>
        /// <returns></returns>
        public CaseData GetCaseEmailPopupData(IXNCustomData emailData, SFDCCallType callType)
        {
            try
            {
                this._logger.Info("GetAccountEmailPopupData :  Reading Account Popup Data.....");
                this._logger.Info("GetAccountEmailPopupData :  CallType Name : " + callType.ToString());

                if (emailData != null)
                {
                    CaseData cases = new CaseData();

                    #region Collect Case Data

                    cases.SearchData = this._sfdcUtilityHelper.GetEmailSearchValue(_caseEmailOptions, emailData, callType);
                    cases.ObjectName = _caseEmailOptions.ObjectName;
                    cases.NoRecordFound = SFDCObjectHelper.GetNoRecordFoundAction(callType, _caseEmailOptions);
                    cases.MultipleMatchRecord = SFDCObjectHelper.GetMultiMatchRecordAction(callType, _caseEmailOptions);
                    cases.NewRecordFieldIds = _caseEmailOptions.NewrecordFieldIds;
                    cases.SearchCondition = _caseEmailOptions.SearchCondition;
                    cases.CreateLogForNewRecord = _caseEmailOptions.CanCreateLogForNewRecordCreate;
                    cases.MaxRecordOpenCount = _caseEmailOptions.MaxNosRecordOpen;
                    cases.SearchpageMode = _caseEmailOptions.SearchPageMode;
                    cases.PhoneNumberSearchFormat = _caseEmailOptions.PhoneNumberSearchFormat;
                    if (cases.NoRecordFound.Equals("createnew") && this._caseEmailRecordConfig != null)
                    {
                        cases.CreateRecordFieldData = this._sfdcUtility.GetCreateActivityLogData(this._caseEmailRecordConfig, null, callType, emailData);
                    }
                    if (callType == SFDCCallType.InboundEmail || callType == SFDCCallType.InboundEmailPulled)
                    {
                        if (_caseEmailOptions.InboundCanCreateLog)
                        {
                            cases.CreateActvityLog = true;
                            cases.ActivityLogData = this._sfdcUtility.GetCreateActivityLogData(this._caseChatLogOptions, null, callType, emailData);
                        }
                    }
                    else if (callType == SFDCCallType.OutboundEmailFailure || callType == SFDCCallType.OutboundEmailSuccess || callType == SFDCCallType.OutboundEmailPulled)
                    {
                        if (_caseEmailOptions.OutboundCanCreateLog)
                        {
                            cases.CreateActvityLog = true;
                            cases.ActivityLogData = this._sfdcUtility.GetCreateActivityLogData(this._caseChatLogOptions, null, callType, emailData);
                        }
                    }

                    #endregion Collect Case Data

                    return cases;
                }
            }
            catch (Exception generalException)
            {
                this._logger.Error("GetAccountEmailPopupData : Error occurred while reading Account Data : " + generalException.ToString());
            }
            return null;
        }

        #endregion GetCaseEmailPopupData

        #region GetCaseChatPopupData

        /// <summary>
        /// Gets Case Popup Data
        /// </summary>
        /// <param name="message"></param>
        /// <param name="callType"></param>
        /// <returns></returns>
        public CaseData GetCaseChatPopupData(IXNCustomData chatData, SFDCCallType callType)
        {
            try
            {
                IMessage message = chatData.InteractionEvent;
                this._logger.Info("GetCaseChatPopupData :  Reading Case Popup Data.....");
                this._logger.Info("GetCaseChatPopupData :  Event Name : " + message.Name);
                this._logger.Info("GetCaseChatPopupData :  CallType Name : " + callType.ToString());
                dynamic popupEvent = Convert.ChangeType(message, message.GetType());

                if (popupEvent != null)
                {
                    CaseData cases = new CaseData();
                    cases.SearchData = this._sfdcUtilityHelper.GetChatSearchValue(_caseChatOptions, chatData, callType);
                    cases.ObjectName = _caseChatOptions.ObjectName;
                    cases.NoRecordFound = SFDCObjectHelper.GetNoRecordFoundAction(callType, _caseChatOptions);
                    cases.MultipleMatchRecord = SFDCObjectHelper.GetMultiMatchRecordAction(callType, _caseChatOptions);
                    cases.NewRecordFieldIds = _caseChatOptions.NewrecordFieldIds;
                    cases.SearchCondition = _caseChatOptions.SearchCondition;
                    cases.CreateLogForNewRecord = _caseChatOptions.CanCreateLogForNewRecordCreate;
                    cases.MaxRecordOpenCount = _caseChatOptions.MaxNosRecordOpen;
                    cases.SearchpageMode = _caseChatOptions.SearchPageMode;
                    cases.PhoneNumberSearchFormat = _caseChatOptions.PhoneNumberSearchFormat;
                    cases.CanCreateNoRecordActivityLog = SFDCObjectHelper.GetCanCreateProfileActivity(callType, _caseChatOptions, true);
                    cases.CanPopupNoRecordActivityLog = SFDCObjectHelper.GetCanPopupProfileActivity(callType, _caseChatOptions, true);
                    cases.CanCreateMultiMatchActivityLog = SFDCObjectHelper.GetCanCreateProfileActivity(callType, _caseChatOptions);
                    cases.CanPopupMultiMatchActivityLog = SFDCObjectHelper.GetCanPopupProfileActivity(callType, _caseChatOptions);
                    cases.CanCreateProfileActivityforInbNoRecord = _caseChatOptions.CanCreateProfileActivityforInbNoRecord;
                    if (cases.NoRecordFound.Equals("createnew") && this._caseChatRecordConfig != null)
                    {
                        cases.CreateRecordFieldData = this._sfdcUtility.GetCreateActivityLogData(this._caseChatRecordConfig, message, callType, emailData: chatData);
                    }
                    if (callType == SFDCCallType.InboundChat)
                    {
                        if (_caseChatOptions.InboundCanCreateLog)
                        {
                            cases.CreateActvityLog = true;
                            cases.ActivityLogData = this._sfdcUtility.GetCreateActivityLogData(this._caseChatLogOptions, popupEvent, callType, emailData: chatData);
                        }
                    }
                    else if (callType == SFDCCallType.ConsultChatReceived)
                    {
                        if (_caseChatOptions.ConsultCanCreateLog)
                        {
                            cases.CreateActvityLog = true;
                            cases.ActivityLogData = this._sfdcUtility.GetCreateActivityLogData(this._caseChatLogOptions, popupEvent, callType, emailData: chatData);
                        }
                    }
                    return cases;
                }
            }
            catch (Exception generalException)
            {
                this._logger.Error("GetCaseChatPopupData : Error occurred while reading Case Data : " + generalException.ToString());
            }
            return null;
        }

        #endregion GetCaseChatPopupData

        #region GetCaseVoiceUpdateData

        /// <summary>
        /// Gets Case Update Data
        /// </summary>
        /// <param name="message"></param>
        /// <param name="callType"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        public CaseData GetCaseVoiceUpdateData(IMessage message, string eventName, SFDCCallType callType, string duration, string notes)
        {
            try
            {
                this._logger.Info("GetCaseVoiceUpdateData :  Reading Case Update Data.....");
                this._logger.Info("GetCaseVoiceUpdateData :  Event Name : " + message.Name);
                dynamic popupEvent = Convert.ChangeType(message, message.GetType());

                if (popupEvent != null)
                {
                    CaseData caseData = new CaseData();

                    #region Collect caseData Data

                    caseData.ObjectName = _caseVoiceOptions.ObjectName;

                    if (callType == SFDCCallType.InboundVoice)
                    {
                        if (_caseVoiceOptions.InboundCanUpdateLog)
                        {
                            caseData.UpdateActivityLog = true;
                            caseData.UpdateActivityLogData = this._sfdcUtility.GetUpdateActivityLogData(this._caseLogConfig, popupEvent, callType, duration, voiceComments: notes);
                            if (!string.IsNullOrWhiteSpace(_caseVoiceOptions.VoiceAppendActivityLogEventNames) && _caseVoiceOptions.VoiceAppendActivityLogEventNames.Contains(eventName))
                                caseData.AppendActivityLogData = this._sfdcUtility.GetUpdateActivityLogData(this._caseLogConfig, null, callType, duration, voiceComments: notes, isAppendLogData: true);
                        }
                    }
                    else if (callType == SFDCCallType.OutboundVoiceSuccess || callType == SFDCCallType.OutboundVoiceFailure)
                    {
                        if (_caseVoiceOptions.OutboundCanUpdateLog)
                        {
                            caseData.UpdateActivityLog = true;
                            caseData.UpdateActivityLogData = this._sfdcUtility.GetUpdateActivityLogData(this._caseLogConfig, popupEvent, callType, duration, voiceComments: notes);
                            if (!string.IsNullOrWhiteSpace(_caseVoiceOptions.VoiceAppendActivityLogEventNames) && _caseVoiceOptions.VoiceAppendActivityLogEventNames.Contains(eventName))
                                caseData.AppendActivityLogData = this._sfdcUtility.GetUpdateActivityLogData(this._caseLogConfig, null, callType, duration, voiceComments: notes, isAppendLogData: true);
                        }
                    }
                    else if (callType == SFDCCallType.ConsultVoiceReceived)
                    {
                        if (_caseVoiceOptions.ConsultCanUpdateLog)
                        {
                            caseData.UpdateActivityLog = true;
                            caseData.UpdateActivityLogData = this._sfdcUtility.GetUpdateActivityLogData(this._caseLogConfig, popupEvent, callType, duration, voiceComments: notes);
                            if (!string.IsNullOrWhiteSpace(_caseVoiceOptions.VoiceAppendActivityLogEventNames) && _caseVoiceOptions.VoiceAppendActivityLogEventNames.Contains(eventName))
                                caseData.AppendActivityLogData = this._sfdcUtility.GetUpdateActivityLogData(this._caseLogConfig, null, callType, duration, voiceComments: notes, isAppendLogData: true);
                        }
                    }

                    if (_caseVoiceOptions.CanUpdateRecordData)
                    {
                        caseData.UpdateRecordFields = true;
                        caseData.UpdateRecordFieldsData = this._sfdcUtility.GetUpdateActivityLogData(this._caseVoiceRecordConfig, popupEvent, callType, duration);
                    }

                    #endregion Collect caseData Data

                    return caseData;
                }
            }
            catch (Exception generalException)
            {
                this._logger.Error("GetCaseVoiceUpdateData : Error occurred while reading caseData Data : " + generalException.ToString());
            }
            return null;
        }

        #endregion GetCaseVoiceUpdateData

        #region GetCaseEmailUpdateData

        /// <summary>
        /// Gets the Chat Update data.
        /// </summary>
        /// <param name="emailData"></param>
        /// <param name="callType"></param>
        /// <param name="duration"></param>
        /// <param name="emailContent"></param>
        /// <returns></returns>
        public CaseData GetCaseEmailUpdateData(IXNCustomData emailData, SFDCCallType callType, string eventName)
        {
            CaseData cases = new CaseData();
            try
            {
                this._logger.Info("GetCaseOutboundEmailUpdateData :  Reading Account Update Data.....");

                if (emailData != null)
                {
                    #region Collect Case Data

                    cases.ObjectName = _caseEmailOptions.ObjectName;
                    if (callType == SFDCCallType.InboundEmail || callType == SFDCCallType.InboundEmailPulled)
                    {
                        if (_caseEmailOptions.InboundCanUpdateLog)
                        {
                            cases.UpdateActivityLog = true;
                            cases.UpdateActivityLogData = this._sfdcUtility.GetUpdateActivityLogData(this._caseEmailLogOptions, null, callType, emailData.Duration, emailData: emailData);
                            if (!string.IsNullOrWhiteSpace(_caseEmailOptions.EmailAppendActivityLogEventNames) && _caseEmailOptions.EmailAppendActivityLogEventNames.Contains(eventName))
                                cases.AppendActivityLogData = this._sfdcUtility.GetUpdateActivityLogData(this._caseEmailLogOptions, null, callType, emailData.Duration, emailData: emailData, isAppendLogData: true);
                        }
                    }
                    else if (callType == SFDCCallType.OutboundEmailFailure || callType == SFDCCallType.OutboundEmailSuccess || callType == SFDCCallType.OutboundEmailPulled)
                    {
                        if (_caseEmailOptions.OutboundCanUpdateLog)
                        {
                            cases.UpdateActivityLog = true;
                            cases.UpdateActivityLogData = this._sfdcUtility.GetUpdateActivityLogData(this._caseEmailLogOptions, null, callType, emailData.Duration, emailData: emailData);
                            if (!string.IsNullOrWhiteSpace(_caseEmailOptions.EmailAppendActivityLogEventNames) && _caseEmailOptions.EmailAppendActivityLogEventNames.Contains(eventName))
                                cases.AppendActivityLogData = this._sfdcUtility.GetUpdateActivityLogData(this._caseEmailLogOptions, null, callType, emailData.Duration, emailData: emailData, isAppendLogData: true);
                        }
                    }
                    //update account record fields
                    cases.UpdateRecordFields = _caseEmailOptions.CanUpdateRecordData;
                    cases.NoRecordFound = SFDCObjectHelper.GetNoRecordFoundAction(callType, _caseEmailOptions);
                    if (cases.NoRecordFound.Equals("createnew") && this._caseEmailRecordConfig != null)
                    {
                        if (_caseEmailOptions.CanUpdateRecordData)
                        {
                            cases.UpdateRecordFieldsData = this._sfdcUtility.GetUpdateActivityLogData(this._caseEmailRecordConfig, null, callType, emailData.Duration, emailData: emailData);
                        }
                    }

                    #endregion Collect Case Data

                    return cases;
                }
            }
            catch (Exception generalException)
            {
                this._logger.Error("GetCaseOutboundEmailUpdateData : Error occurred while reading Account Data : " + generalException.ToString());
                return cases;
            }
            return null;
        }

        #endregion GetCaseEmailUpdateData

        #region GetCaseChatUpdateData

        /// <summary>
        /// Gets chat Update Data
        /// </summary>
        /// <param name="message"></param>
        /// <param name="callType"></param>
        /// <param name="duration"></param>
        /// <param name="chatContent"></param>
        /// <returns></returns>
        public CaseData GetCaseChatUpdateData(IXNCustomData chatData, string eventName)
        {
            try
            {
                IMessage message = chatData.InteractionEvent;
                SFDCCallType callType = chatData.InteractionType;
                string duration = chatData.Duration;
                this._logger.Info("GetCaseChatUpdateData :  Reading Case Update Data.....");
                this._logger.Info("GetCaseChatUpdateData :  Event Name : " + message.Name);
                dynamic popupEvent = Convert.ChangeType(message, message.GetType());
                if (popupEvent != null)
                {
                    CaseData cases = new CaseData();

                    #region Collect Case Data

                    cases.ObjectName = _caseChatOptions.ObjectName;
                    if (callType == SFDCCallType.InboundChat)
                    {
                        if (_caseChatOptions.InboundCanUpdateLog)
                        {
                            cases.UpdateActivityLog = true;
                            cases.UpdateActivityLogData = this._sfdcUtility.GetUpdateActivityLogData(this._caseChatLogOptions, popupEvent, callType, duration, emailData: chatData);
                            if (!string.IsNullOrWhiteSpace(_caseChatOptions.ChatAppendActivityLogEventNames) && _caseChatOptions.ChatAppendActivityLogEventNames.Contains(eventName))
                                cases.AppendActivityLogData = this._sfdcUtility.GetUpdateActivityLogData(this._caseChatLogOptions, null, callType, duration, emailData: chatData, isAppendLogData: true);
                        }
                    }
                    else
                        if (callType == SFDCCallType.ConsultChatReceived)
                        {
                            if (_caseChatOptions.ConsultCanUpdateLog)
                            {
                                cases.UpdateActivityLog = true;
                                cases.UpdateActivityLogData = this._sfdcUtility.GetUpdateActivityLogData(this._caseChatLogOptions, popupEvent, callType, duration, emailData: chatData);
                                if (!string.IsNullOrWhiteSpace(_caseChatOptions.ChatAppendActivityLogEventNames) && _caseChatOptions.ChatAppendActivityLogEventNames.Contains(eventName))
                                    cases.AppendActivityLogData = this._sfdcUtility.GetUpdateActivityLogData(this._caseChatLogOptions, null, callType, duration, emailData: chatData, isAppendLogData: true);
                            }
                        }

                    //update case record fields
                    cases.UpdateRecordFields = _caseChatOptions.CanUpdateRecordData;
                    if (SFDCObjectHelper.GetNoRecordFoundAction(callType, _caseChatOptions).Equals("createnew") && this._caseChatRecordConfig != null)
                    {
                        if (_caseChatOptions.CanUpdateRecordData)
                        {
                            cases.UpdateRecordFieldsData = this._sfdcUtility.GetUpdateActivityLogData(this._caseChatRecordConfig, popupEvent, callType, duration, emailData: chatData);
                        }
                    }

                    #endregion Collect Case Data

                    return cases;
                }
            }
            catch (Exception generalException)
            {
                this._logger.Error("GetCaseChatUpdateData : Error occurred while reading Case Data : " + generalException.ToString());
            }
            return null;
        }

        #endregion GetCaseChatUpdateData
    }
}