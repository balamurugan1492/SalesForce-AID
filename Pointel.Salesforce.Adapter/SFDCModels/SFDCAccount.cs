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
    /// Comment: Provides Search and Activity Data for Account Object Last Modified: 25-08-2015
    /// Created by: Pointel Inc
    /// </summary>
    internal class SFDCAccount
    {
        #region Field Declaration

        private static SFDCAccount _accountData = null;
        private KeyValueCollection _accountChatLogConfig = null;
        private ChatOptions _accountChatOptions = null;
        private KeyValueCollection _accountEmailLogConfig = null;
        private EmailOptions _accountEmailOptions = null;
        private KeyValueCollection _accountLogConfig = null;
        private KeyValueCollection _accountEmailRecordConfig = null;
        private KeyValueCollection _accountVoiceRecordConfig = null;
        private KeyValueCollection _accountChatRecordConfig = null;
        private VoiceOptions _accountVoiceOptions = null;
        private Log _logger = null;
        private SFDCUtility _sfdcUtility = null;
        private SFDCUtiltiyHelper _sfdcUtilityHelper = null;

        #endregion Field Declaration

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        private SFDCAccount()
        {
            this._logger = Log.GenInstance();
            this._accountVoiceOptions = Settings.AccountVoiceOptions;
            this._accountChatOptions = Settings.AccountChatOptions;
            _accountLogConfig = (Settings.VoiceActivityLogCollection.ContainsKey("account")) ? Settings.VoiceActivityLogCollection["account"] : null;
            _accountChatLogConfig = (Settings.ChatActivityLogCollection.ContainsKey("account")) ? Settings.ChatActivityLogCollection["account"] : null;
            this._accountEmailRecordConfig = (Settings.EmailNewRecordCollection.ContainsKey("account")) ? Settings.EmailNewRecordCollection["account"] : null;
            this._accountVoiceRecordConfig = (Settings.VoiceNewRecordCollection.ContainsKey("account")) ? Settings.VoiceNewRecordCollection["account"] : null;
            this._accountChatRecordConfig = (Settings.ChatNewRecordCollection.ContainsKey("account")) ? Settings.ChatNewRecordCollection["account"] : null;
            this._sfdcUtility = SFDCUtility.GetInstance();
            this._accountEmailOptions = Settings.AccountEmailOptions;
            this._accountEmailLogConfig = Settings.EmailActivityLogCollection.ContainsKey("account") ? Settings.EmailActivityLogCollection["account"] : null;
            this._sfdcUtilityHelper = SFDCUtiltiyHelper.GetInstance();
        }

        #endregion Constructor

        #region GetInstance Method

        /// <summary>
        /// Gets the Intstance of the class
        /// </summary>
        public static SFDCAccount GetInstance()
        {
            if (_accountData == null)
            {
                _accountData = new SFDCAccount();
            }
            return _accountData;
        }

        #endregion GetInstance Method

        #region GetAccountVoicePopupData

        /// <summary>
        /// Get Account Popup Data
        /// </summary>
        /// <param name="message"></param>
        /// <param name="callType"></param>
        /// <returns></returns>
        public AccountData GetAccountVoicePopupData(IMessage message, SFDCCallType callType)
        {
            try
            {
                this._logger.Info("GetAccountVoicePopupData :  Reading Account Popup Data.....");
                this._logger.Info("GetAccountVoicePopupData :  Event Name : " + message.Name);
                this._logger.Info("GetAccountVoicePopupData :  CallType Name : " + callType.ToString());
                dynamic _popupEvent = Convert.ChangeType(message, message.GetType());

                if (_popupEvent != null)
                {
                    AccountData _account = new AccountData();

                    #region Collect account Data

                    _account.SearchData = this._sfdcUtilityHelper.GetVoiceSearchValue(_accountVoiceOptions, message, callType);
                    _account.ObjectName = _accountVoiceOptions.ObjectName;
                    _account.NoRecordFound = SFDCObjectHelper.GetNoRecordFoundAction(callType, _accountVoiceOptions);
                    _account.MultipleMatchRecord = SFDCObjectHelper.GetMultiMatchRecordAction(callType, _accountVoiceOptions);
                    _account.NewRecordFieldIds = _accountVoiceOptions.NewrecordFieldIds;
                    _account.SearchCondition = _accountVoiceOptions.SearchCondition;
                    _account.CreateLogForNewRecord = _accountVoiceOptions.CanCreateLogForNewRecordCreate;
                    _account.MaxRecordOpenCount = _accountVoiceOptions.MaxNosRecordOpen;
                    _account.SearchpageMode = _accountVoiceOptions.SearchPageMode;
                    _account.PhoneNumberSearchFormat = _accountVoiceOptions.PhoneNumberSearchFormat;
                    _account.CanCreateNoRecordActivityLog = SFDCObjectHelper.GetCanCreateProfileActivity(callType, _accountVoiceOptions, true);
                    _account.CanPopupNoRecordActivityLog = SFDCObjectHelper.GetCanPopupProfileActivity(callType, _accountVoiceOptions, true);
                    _account.CanCreateMultiMatchActivityLog = SFDCObjectHelper.GetCanCreateProfileActivity(callType, _accountVoiceOptions);
                    _account.CanPopupMultiMatchActivityLog = SFDCObjectHelper.GetCanPopupProfileActivity(callType, _accountVoiceOptions);
                    _account.CanCreateProfileActivityforInbNoRecord = _accountVoiceOptions.CanCreateProfileActivityforInbNoRecord;
                    _account.CanCreateProfileActivityforOutNoRecord = _accountVoiceOptions.CanCreateProfileActivityforOutNoRecord;
                    _account.CanCreateProfileActivityforConNoRecord = _accountVoiceOptions.CanCreateProfileActivityforConNoRecord;

                    if (_account.NoRecordFound.Equals("createnew") && this._accountVoiceRecordConfig != null)
                    {
                        _account.CreateRecordFieldData = this._sfdcUtility.GetCreateActivityLogData(this._accountVoiceRecordConfig, message, callType);
                    }
                    if (callType == SFDCCallType.InboundVoice)
                    {
                        if (_accountVoiceOptions.InboundCanCreateLog)
                        {
                            _account.CreateActvityLog = true;
                            _account.ActivityLogData = this._sfdcUtility.GetCreateActivityLogData(this._accountLogConfig, _popupEvent, callType);
                        }
                    }
                    else if (callType == SFDCCallType.OutboundVoiceSuccess)
                    {
                        if (_accountVoiceOptions.OutboundCanCreateLog)
                        {
                            _account.CreateActvityLog = true;
                            _account.ActivityLogData = this._sfdcUtility.GetCreateActivityLogData(this._accountLogConfig, _popupEvent, callType);
                        }
                    }
                    else if (callType == SFDCCallType.OutboundVoiceFailure)
                    {
                        if (_accountVoiceOptions.OutboundFailureCanCreateLog)
                        {
                            _account.CreateActvityLog = true;
                            _account.ActivityLogData = this._sfdcUtility.GetCreateActivityLogData(this._accountLogConfig, _popupEvent, callType);
                        }
                    }
                    else if (callType == SFDCCallType.ConsultVoiceReceived)
                    {
                        if (_accountVoiceOptions.ConsultCanCreateLog)
                        {
                            _account.CreateActvityLog = true;
                            _account.ActivityLogData = this._sfdcUtility.GetCreateActivityLogData(this._accountLogConfig, _popupEvent, callType);
                        }
                    }

                    #endregion Collect account Data

                    return _account;
                }
            }
            catch (Exception generalException)
            {
                this._logger.Error("GetAccountVoicePopupData : Error occurred while reading Account Data : " + generalException.ToString());
            }
            return null;
        }

        #endregion GetAccountVoicePopupData

        #region GetAccountEmailPopupData

        /// <summary>
        /// Get Account Popup Data for Email
        /// </summary>
        /// <param name="emailData"></param>
        /// <param name="callType"></param>
        /// <returns></returns>
        public AccountData GetAccountEmailPopupData(IXNCustomData emailData, SFDCCallType callType)
        {
            try
            {
                this._logger.Info("GetAccountEmailPopupData :  Reading Account Popup Data.....");
                this._logger.Info("GetAccountEmailPopupData :  CallType Name : " + callType.ToString());

                if (emailData != null)
                {
                    AccountData _account = new AccountData();

                    #region Collect Account Data

                    _account.SearchData = this._sfdcUtilityHelper.GetEmailSearchValue(_accountEmailOptions, emailData, callType);
                    _account.ObjectName = _accountEmailOptions.ObjectName;
                    _account.NoRecordFound = SFDCObjectHelper.GetNoRecordFoundAction(callType, _accountEmailOptions);
                    _account.MultipleMatchRecord = SFDCObjectHelper.GetMultiMatchRecordAction(callType, _accountEmailOptions);
                    _account.NewRecordFieldIds = _accountEmailOptions.NewrecordFieldIds;
                    _account.SearchCondition = _accountEmailOptions.SearchCondition;
                    _account.CreateLogForNewRecord = _accountEmailOptions.CanCreateLogForNewRecordCreate;
                    _account.MaxRecordOpenCount = _accountEmailOptions.MaxNosRecordOpen;
                    _account.SearchpageMode = _accountEmailOptions.SearchPageMode;
                    _account.PhoneNumberSearchFormat = _accountEmailOptions.PhoneNumberSearchFormat;
                    _account.CanCreateNoRecordActivityLog = SFDCObjectHelper.GetCanCreateProfileActivity(callType, _accountEmailOptions, true);
                    _account.CanPopupNoRecordActivityLog = SFDCObjectHelper.GetCanPopupProfileActivity(callType, _accountEmailOptions, true);
                    _account.CanCreateMultiMatchActivityLog = SFDCObjectHelper.GetCanCreateProfileActivity(callType, _accountEmailOptions);
                    _account.CanPopupMultiMatchActivityLog = SFDCObjectHelper.GetCanPopupProfileActivity(callType, _accountEmailOptions);
                    _account.CanCreateProfileActivityforInbNoRecord = _accountEmailOptions.CanCreateProfileActivityforInbNoRecord;
                    _account.CanCreateProfileActivityforOutNoRecord = _accountEmailOptions.CanCreateProfileActivityforOutNoRecord;
                    if (_account.NoRecordFound.Equals("createnew") && this._accountEmailRecordConfig != null)
                    {
                        _account.CreateRecordFieldData = this._sfdcUtility.GetCreateActivityLogData(this._accountEmailRecordConfig, null, callType, emailData);
                    }
                    if (callType == SFDCCallType.InboundEmail || callType == SFDCCallType.InboundEmailPulled)
                    {
                        if (_accountEmailOptions.InboundCanCreateLog)
                        {
                            _account.CreateActvityLog = true;
                            _account.ActivityLogData = this._sfdcUtility.GetCreateActivityLogData(this._accountEmailLogConfig, null, callType, emailData);
                        }
                    }
                    else if (callType == SFDCCallType.OutboundEmailFailure || callType == SFDCCallType.OutboundEmailSuccess || callType == SFDCCallType.OutboundEmailPulled)
                    {
                        if (_accountEmailOptions.OutboundCanCreateLog)
                        {
                            _account.CreateActvityLog = true;
                            _account.ActivityLogData = this._sfdcUtility.GetCreateActivityLogData(this._accountEmailLogConfig, null, callType, emailData);
                        }
                    }

                    #endregion Collect Account Data

                    return _account;
                }
            }
            catch (Exception generalException)
            {
                this._logger.Error("GetAccountEmailPopupData : Error occurred while reading Account Data : " + generalException.ToString());
            }
            return null;
        }

        #endregion GetAccountEmailPopupData

        #region GetAccountChatPopupData

        /// <summary>
        /// Get Account Popup Data for Chat
        /// </summary>
        /// <param name="message"></param>
        /// <param name="callType"></param>
        /// <returns></returns>
        public AccountData GetAccountChatPopupData(IXNCustomData chatData, SFDCCallType callType)
        {
            try
            {
                IMessage message = chatData.InteractionEvent;
                this._logger.Info("GetAccountChatPopupData :  Reading Account Popup Data.....");
                this._logger.Info("GetAccountChatPopupData :  Event Name : " + message.Name);
                this._logger.Info("GetAccountChatPopupData :  CallType Name : " + callType.ToString());
                dynamic _popupEvent = Convert.ChangeType(message, message.GetType());

                if (_popupEvent != null)
                {
                    AccountData _account = new AccountData();

                    #region Collect Account Data

                    _account.SearchData = this._sfdcUtilityHelper.GetChatSearchValue(_accountChatOptions, chatData, callType);
                    _account.ObjectName = _accountChatOptions.ObjectName;
                    _account.NoRecordFound = SFDCObjectHelper.GetNoRecordFoundAction(callType, _accountChatOptions);
                    _account.MultipleMatchRecord = SFDCObjectHelper.GetMultiMatchRecordAction(callType, _accountChatOptions);
                    _account.NewRecordFieldIds = _accountChatOptions.NewrecordFieldIds;
                    _account.SearchCondition = _accountChatOptions.SearchCondition;
                    _account.CreateLogForNewRecord = _accountChatOptions.CanCreateLogForNewRecordCreate;
                    _account.MaxRecordOpenCount = _accountChatOptions.MaxNosRecordOpen;
                    _account.SearchpageMode = _accountChatOptions.SearchPageMode;
                    _account.PhoneNumberSearchFormat = _accountChatOptions.PhoneNumberSearchFormat;
                    _account.CanCreateNoRecordActivityLog = SFDCObjectHelper.GetCanCreateProfileActivity(callType, _accountChatOptions, true);
                    _account.CanPopupNoRecordActivityLog = SFDCObjectHelper.GetCanPopupProfileActivity(callType, _accountChatOptions, true);
                    _account.CanCreateMultiMatchActivityLog = SFDCObjectHelper.GetCanCreateProfileActivity(callType, _accountChatOptions);
                    _account.CanPopupMultiMatchActivityLog = SFDCObjectHelper.GetCanPopupProfileActivity(callType, _accountChatOptions);
                    _account.CanCreateProfileActivityforInbNoRecord = _accountChatOptions.CanCreateProfileActivityforInbNoRecord;
                    if (_account.NoRecordFound.Equals("createnew") && this._accountChatRecordConfig != null)
                    {
                        _account.CreateRecordFieldData = this._sfdcUtility.GetCreateActivityLogData(this._accountChatRecordConfig, message, callType, emailData: chatData);
                    }
                    if (callType == SFDCCallType.InboundChat)
                    {
                        if (_accountChatOptions.InboundCanCreateLog)
                        {
                            _account.CreateActvityLog = true;
                            _account.ActivityLogData = this._sfdcUtility.GetCreateActivityLogData(this._accountChatLogConfig, _popupEvent, callType, emailData: chatData);
                        }
                    }
                    else if (callType == SFDCCallType.ConsultChatReceived)
                    {
                        if (_accountChatOptions.ConsultCanCreateLog)
                        {
                            _account.CreateActvityLog = true;
                            _account.ActivityLogData = this._sfdcUtility.GetCreateActivityLogData(this._accountChatLogConfig, _popupEvent, callType, emailData: chatData);
                        }
                    }

                    #endregion Collect Account Data

                    return _account;
                }
            }
            catch (Exception generalException)
            {
                this._logger.Error("GetAccountChatPopupData : Error occurred while reading Account Data : " + generalException.ToString());
            }
            return null;
        }

        #endregion GetAccountChatPopupData

        #region GetAccountVoiceUpdateData

        /// <summary>
        /// Gets Account Update Data
        /// </summary>
        /// <param name="message"></param>
        /// <param name="callType"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        public AccountData GetAccountUpdateData(IMessage message, string eventName, SFDCCallType callType, string duration, string notes)
        {
            try
            {
                this._logger.Info("GetAccountUpdateData :  Reading Account Update Data.....");
                this._logger.Info("GetAccountUpdateData :  Event Name : " + message.Name);
                dynamic _popupEvent = Convert.ChangeType(message, message.GetType());

                if (_popupEvent != null)
                {
                    AccountData _account = new AccountData();

                    #region Collect account Data

                    _account.ObjectName = _accountVoiceOptions.ObjectName;

                    if (callType == SFDCCallType.InboundVoice)
                    {
                        if (_accountVoiceOptions.InboundCanUpdateLog)
                        {
                            _account.UpdateActivityLog = true;
                            _account.UpdateActivityLogData = this._sfdcUtility.GetUpdateActivityLogData(this._accountLogConfig, _popupEvent, callType, duration, voiceComments: notes);
                            if (!string.IsNullOrWhiteSpace(_accountVoiceOptions.VoiceAppendActivityLogEventNames) && _accountVoiceOptions.VoiceAppendActivityLogEventNames.Contains(eventName))
                                _account.AppendActivityLogData = this._sfdcUtility.GetUpdateActivityLogData(this._accountLogConfig, null, callType, duration, voiceComments: notes, isAppendLogData: true);
                        }
                    }
                    else if (callType == SFDCCallType.OutboundVoiceSuccess || callType == SFDCCallType.OutboundVoiceFailure)
                    {
                        if (_accountVoiceOptions.OutboundCanUpdateLog)
                        {
                            _account.UpdateActivityLog = true;
                            _account.UpdateActivityLogData = this._sfdcUtility.GetUpdateActivityLogData(this._accountLogConfig, _popupEvent, callType, duration, voiceComments: notes);
                            if (!string.IsNullOrWhiteSpace(_accountVoiceOptions.VoiceAppendActivityLogEventNames) && _accountVoiceOptions.VoiceAppendActivityLogEventNames.Contains(eventName))
                                _account.AppendActivityLogData = this._sfdcUtility.GetUpdateActivityLogData(this._accountLogConfig, null, callType, duration, voiceComments: notes, isAppendLogData: true);
                        }
                    }
                    else if (callType == SFDCCallType.ConsultVoiceReceived)
                    {
                        if (_accountVoiceOptions.ConsultCanUpdateLog)
                        {
                            _account.UpdateActivityLog = true;
                            _account.UpdateActivityLogData = this._sfdcUtility.GetUpdateActivityLogData(this._accountLogConfig, _popupEvent, callType, duration, voiceComments: notes);
                            if (!string.IsNullOrWhiteSpace(_accountVoiceOptions.VoiceAppendActivityLogEventNames) && _accountVoiceOptions.VoiceAppendActivityLogEventNames.Contains(eventName))
                                _account.AppendActivityLogData = this._sfdcUtility.GetUpdateActivityLogData(this._accountLogConfig, null, callType, duration, voiceComments: notes, isAppendLogData: true);
                        }
                    }

                    if (_accountVoiceOptions.CanUpdateRecordData)
                    {
                        _account.UpdateRecordFields = true;
                        _account.UpdateRecordFieldsData = this._sfdcUtility.GetUpdateActivityLogData(this._accountVoiceRecordConfig, _popupEvent, callType, duration);
                    }

                    #endregion Collect account Data

                    return _account;
                }
            }
            catch (Exception generalException)
            {
                this._logger.Error("GetAccountUpdateData : Error occurred while reading account Data : " + generalException.ToString());
            }
            return null;
        }

        #endregion GetAccountVoiceUpdateData

        #region GetAccountEmailUpdateData

        /// <summary>
        /// Gets the Chat Update data.
        /// </summary>
        /// <param name="emailData"></param>
        /// <param name="callType"></param>
        /// <param name="duration"></param>
        /// <param name="emailContent"></param>
        /// <returns></returns>
        public AccountData GetAccountEmailUpdateData(IXNCustomData emailData, SFDCCallType callType, string eventName)
        {
            AccountData _account = new AccountData();
            try
            {
                this._logger.Info("GetAccountOutboundEmailUpdateData :  Reading Account Update Data.....");

                if (emailData != null)
                {
                    #region Collect account Data

                    _account.ObjectName = _accountEmailOptions.ObjectName;
                    if (callType == SFDCCallType.InboundEmail || callType == SFDCCallType.InboundEmailPulled)
                    {
                        if (_accountEmailOptions.InboundCanUpdateLog)
                        {
                            _account.UpdateActivityLog = true;
                            _account.UpdateActivityLogData = this._sfdcUtility.GetUpdateActivityLogData(this._accountEmailLogConfig, null, callType, emailData.Duration, emailData: emailData);
                            if (!string.IsNullOrWhiteSpace(_accountEmailOptions.EmailAppendActivityLogEventNames) && _accountEmailOptions.EmailAppendActivityLogEventNames.Contains(eventName))
                                _account.AppendActivityLogData = this._sfdcUtility.GetUpdateActivityLogData(this._accountEmailLogConfig, null, callType, emailData.Duration, emailData: emailData, isAppendLogData: true);
                        }
                    }
                    else if (callType == SFDCCallType.OutboundEmailFailure || callType == SFDCCallType.OutboundEmailSuccess || callType == SFDCCallType.OutboundEmailPulled)
                    {
                        if (_accountEmailOptions.OutboundCanUpdateLog)
                        {
                            _account.UpdateActivityLog = true;
                            _account.UpdateActivityLogData = this._sfdcUtility.GetUpdateActivityLogData(this._accountEmailLogConfig, null, callType, emailData.Duration, emailData: emailData);
                            if (!string.IsNullOrWhiteSpace(_accountEmailOptions.EmailAppendActivityLogEventNames) && _accountEmailOptions.EmailAppendActivityLogEventNames.Contains(eventName))
                                _account.AppendActivityLogData = this._sfdcUtility.GetUpdateActivityLogData(this._accountEmailLogConfig, null, callType, emailData.Duration, emailData: emailData, isAppendLogData: true);
                        }
                    }
                    //update account record fields
                    _account.UpdateRecordFields = _accountEmailOptions.CanUpdateRecordData;
                    _account.NoRecordFound = SFDCObjectHelper.GetNoRecordFoundAction(callType, _accountEmailOptions);
                    if (_account.NoRecordFound.Equals("createnew") && this._accountEmailRecordConfig != null)
                    {
                        if (_accountEmailOptions.CanUpdateRecordData)
                        {
                            _account.UpdateRecordFieldsData = this._sfdcUtility.GetUpdateActivityLogData(this._accountEmailRecordConfig, null, callType, emailData.Duration, emailData: emailData);
                        }
                    }

                    #endregion Collect account Data

                    return _account;
                }
            }
            catch (Exception generalException)
            {
                this._logger.Error("GetAccountOutboundEmailUpdateData : Error occurred while reading Account Data : " + generalException.ToString());
                return _account;
            }
            return null;
        }

        #endregion GetAccountEmailUpdateData

        #region GetAccountChatUpdateData

        /// <summary>
        /// Gets the Chat Update data.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="callType"></param>
        /// <param name="duration"></param>
        /// <param name="chatContent"></param>
        /// <returns></returns>
        public AccountData GetAccountChatUpdateData(IXNCustomData chatData, string eventName)
        {
            try
            {
                IMessage message = chatData.InteractionEvent;
                SFDCCallType callType = chatData.InteractionType;
                string duration = chatData.Duration;
                this._logger.Info("GetAccountChatUpdateData :  Reading Account Update Data.....");
                this._logger.Info("GetAccountChatUpdateData :  Event Name : " + message.Name);
                dynamic _popupEvent = Convert.ChangeType(message, message.GetType());
                if (_popupEvent != null)
                {
                    AccountData _account = new AccountData();

                    #region Collect Account Data

                    _account.ObjectName = _accountChatOptions.ObjectName;
                    if (callType == SFDCCallType.InboundChat)
                    {
                        if (_accountChatOptions.InboundCanUpdateLog)
                        {
                            _account.UpdateActivityLog = true;
                            _account.UpdateActivityLogData = this._sfdcUtility.GetUpdateActivityLogData(this._accountChatLogConfig, _popupEvent, callType, duration, emailData: chatData);
                            if (!string.IsNullOrWhiteSpace(_accountChatOptions.ChatAppendActivityLogEventNames) && _accountChatOptions.ChatAppendActivityLogEventNames.Contains(eventName))
                                _account.AppendActivityLogData = this._sfdcUtility.GetUpdateActivityLogData(this._accountChatLogConfig, null, callType, duration, emailData: chatData, isAppendLogData: true);
                        }
                    }
                    else if (callType == SFDCCallType.ConsultChatReceived)
                    {
                        if (_accountChatOptions.ConsultCanUpdateLog)
                        {
                            _account.UpdateActivityLog = true;
                            _account.UpdateActivityLogData = this._sfdcUtility.GetUpdateActivityLogData(this._accountChatLogConfig, _popupEvent, callType, duration, emailData: chatData);
                            if (!string.IsNullOrWhiteSpace(_accountChatOptions.ChatAppendActivityLogEventNames) && _accountChatOptions.ChatAppendActivityLogEventNames.Contains(eventName))
                                _account.AppendActivityLogData = this._sfdcUtility.GetUpdateActivityLogData(this._accountChatLogConfig, null, callType, duration, emailData: chatData, isAppendLogData: true);
                        }
                    }

                    //update account record fields
                    _account.UpdateRecordFields = _accountChatOptions.CanUpdateRecordData;
                    if (SFDCObjectHelper.GetNoRecordFoundAction(callType, _accountChatOptions).Equals("createnew") && this._accountChatRecordConfig != null)
                    {
                        if (_accountChatOptions.CanUpdateRecordData)
                        {
                            _account.UpdateRecordFieldsData = this._sfdcUtility.GetUpdateActivityLogData(this._accountChatRecordConfig, _popupEvent, callType, duration, emailData: chatData);
                        }
                    }

                    #endregion Collect Account Data

                    return _account;
                }
            }
            catch (Exception generalException)
            {
                this._logger.Error("GetAccountChatUpdateData : Error occurred while reading Account Data : " + generalException.ToString());
            }
            return null;
        }

        #endregion GetAccountChatUpdateData
    }
}