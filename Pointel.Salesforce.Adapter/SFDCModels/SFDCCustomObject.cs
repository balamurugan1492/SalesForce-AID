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
using System.Collections.Generic;

namespace Pointel.Salesforce.Adapter.SFDCModels
{/// <summary>
    /// Comment: Provides Search and Activity Data for CustomObject Last Modified: 25-08-2015 Created
    /// by: Pointel Inc </summary>
    internal class SFDCCustomObject
    {
        #region Field Declaration

        private static SFDCCustomObject customObject = null;
        private IDictionary<string, KeyValueCollection> CustomChatLogConfigs = null;
        private Dictionary<string, ChatOptions> customchatOptions = null;
        private IDictionary<string, KeyValueCollection> CustomEmailLogConfigs = null;
        private Dictionary<string, EmailOptions> customemailOptions = null;
        private IDictionary<string, KeyValueCollection> CustomVoiceRecordConfigs = null;
        private IDictionary<string, KeyValueCollection> CustomEmailRecordConfigs = null;
        private IDictionary<string, KeyValueCollection> CustomChatRecordConfigs = null;
        private IDictionary<string, KeyValueCollection> CustomVoiceLogConfigs = null;
        private Dictionary<string, VoiceOptions> customvoiceOptions = null;
        private Log logger = null;
        private SFDCUtility sfdcUtility = null;
        private SFDCUtiltiyHelper sfdcUtilityHelper;

        #endregion Field Declaration

        #region Constructor

        private SFDCCustomObject()
        {
            this.logger = Log.GenInstance();
            this.customvoiceOptions = Settings.CustomObjectVoiceOptions;
            this.customchatOptions = Settings.CustomObjectChatOptions;
            this.CustomVoiceLogConfigs = Settings.VoiceActivityLogCollection;
            this.CustomVoiceRecordConfigs = Settings.VoiceNewRecordCollection;
            this.CustomEmailRecordConfigs = Settings.EmailNewRecordCollection;
            this.CustomChatRecordConfigs = Settings.ChatNewRecordCollection;
            this.CustomChatLogConfigs = Settings.ChatActivityLogCollection;
            this.sfdcUtility = SFDCUtility.GetInstance();
            this.CustomEmailLogConfigs = Settings.EmailActivityLogCollection;
            this.customemailOptions = Settings.CustomObjectEmailOptions;
            this.sfdcUtilityHelper = SFDCUtiltiyHelper.GetInstance();
        }

        #endregion Constructor

        #region GetInstance Method

        /// <summary>
        /// Gets the Instance
        /// </summary>
        /// <returns></returns>
        public static SFDCCustomObject GetInstance()
        {
            if (customObject == null)
            {
                customObject = new SFDCCustomObject();
            }
            return customObject;
        }

        #endregion GetInstance Method

        #region GetCustomObjectVoicePopupData

        /// <summary>
        /// Gets Custom Object Popup Data
        /// </summary>
        /// <param name="message"></param>
        /// <param name="callType"></param>
        /// <param name="objectName"></param>
        /// <returns></returns>
        public CustomObjectData GetCustomObjectVoicePopupData(IMessage message, SFDCCallType callType, string objectName)
        {
            try
            {
                this.logger.Info("GetCustomObjectVoicePopupData :  Reading CustomObject Popup Data.....");
                this.logger.Info("GetCustomObjectVoicePopupData :  Event Name : " + message.Name);
                this.logger.Info("GetCustomObjectVoicePopupData :  CallType Name : " + callType.ToString());
                dynamic popupEvent = Convert.ChangeType(message, message.GetType());
                if (popupEvent != null && customvoiceOptions.ContainsKey(objectName))
                {
                    CustomObjectData custom = new CustomObjectData();
                    VoiceOptions customObjectOptions = customvoiceOptions[objectName];

                    #region Collect CustomObject Popup data

                    custom.SearchData = this.sfdcUtilityHelper.GetVoiceSearchValue(customObjectOptions, message, callType);
                    custom.ObjectName = customObjectOptions.ObjectName;
                    custom.NoRecordFound = SFDCObjectHelper.GetNoRecordFoundAction(callType, customObjectOptions);
                    custom.MultipleMatchRecord = SFDCObjectHelper.GetMultiMatchRecordAction(callType, customObjectOptions);
                    custom.NewRecordFieldIds = customObjectOptions.NewrecordFieldIds;
                    custom.SearchCondition = customObjectOptions.SearchCondition;
                    custom.CreateLogForNewRecord = customObjectOptions.CanCreateLogForNewRecordCreate;
                    custom.MaxRecordOpenCount = customObjectOptions.MaxNosRecordOpen;
                    custom.SearchpageMode = customObjectOptions.SearchPageMode;
                    custom.CustomObjectURL = customObjectOptions.CustomObjectURL;
                    custom.PhoneNumberSearchFormat = customObjectOptions.PhoneNumberSearchFormat;
                    custom.CanCreateNoRecordActivityLog = SFDCObjectHelper.GetCanCreateProfileActivity(callType, customObjectOptions, true);
                    custom.CanPopupNoRecordActivityLog = SFDCObjectHelper.GetCanPopupProfileActivity(callType, customObjectOptions, true);
                    custom.CanCreateMultiMatchActivityLog = SFDCObjectHelper.GetCanCreateProfileActivity(callType, customObjectOptions);
                    custom.CanPopupMultiMatchActivityLog = SFDCObjectHelper.GetCanPopupProfileActivity(callType, customObjectOptions);
                    custom.CanCreateProfileActivityforInbNoRecord = customObjectOptions.CanCreateProfileActivityforInbNoRecord;
                    custom.CanCreateProfileActivityforOutNoRecord = customObjectOptions.CanCreateProfileActivityforOutNoRecord;
                    custom.CanCreateProfileActivityforConNoRecord = customObjectOptions.CanCreateProfileActivityforConNoRecord;

                    if (CustomVoiceRecordConfigs != null && CustomVoiceRecordConfigs.ContainsKey(objectName))
                    {
                        KeyValueCollection RecordConfig = CustomVoiceRecordConfigs[objectName];
                        if (RecordConfig != null)
                        {
                            if (custom.NoRecordFound.Equals("createnew"))
                            {
                                custom.CreateRecordFieldData = this.sfdcUtility.GetCreateActivityLogData(RecordConfig, message, callType);
                            }
                        }
                    }
                    KeyValueCollection LogConfig = null;
                    if (this.CustomVoiceLogConfigs != null && this.CustomVoiceLogConfigs.ContainsKey(objectName))
                    {
                        LogConfig = this.CustomVoiceLogConfigs[objectName];
                    }

                    if (callType == SFDCCallType.InboundVoice)
                    {
                        custom.CreateActvityLog = customObjectOptions.InboundCanCreateLog;
                        if (LogConfig != null)
                        {
                            if (customObjectOptions.InboundCanCreateLog)
                            {
                                custom.ActivityLogData = this.sfdcUtility.GetCreateActivityLogData(LogConfig, popupEvent, callType);
                            }
                        }
                    }
                    else if (callType == SFDCCallType.OutboundVoiceSuccess)
                    {
                        custom.CreateActvityLog = customObjectOptions.OutboundCanCreateLog;
                        if (LogConfig != null)
                        {
                            if (customObjectOptions.OutboundCanCreateLog)
                            {
                                custom.ActivityLogData = this.sfdcUtility.GetCreateActivityLogData(LogConfig, popupEvent, callType);
                            }
                        }
                    }
                    else if (callType == SFDCCallType.OutboundVoiceFailure)
                    {
                        custom.CreateActvityLog = customObjectOptions.OutboundFailureCanCreateLog;
                        if (LogConfig != null)
                        {
                            if (customObjectOptions.OutboundFailureCanCreateLog)
                            {
                                custom.ActivityLogData = this.sfdcUtility.GetCreateActivityLogData(LogConfig, popupEvent, callType);
                            }
                        }
                    }
                    else if (callType == SFDCCallType.ConsultVoiceReceived)
                    {
                        custom.CreateActvityLog = customObjectOptions.ConsultCanCreateLog;
                        if (LogConfig != null)
                        {
                            if (customObjectOptions.ConsultCanCreateLog)
                            {
                                custom.ActivityLogData = this.sfdcUtility.GetCreateActivityLogData(LogConfig, popupEvent, callType);
                            }
                        }
                    }

                    return custom;
                }

                    #endregion Collect CustomObject Popup data
            }
            catch (Exception generalException)
            {
                this.logger.Error("GetCustomObjectVoicePopupData : Error occurred while reading Contact Data on EventRinging Event : " + generalException.ToString());
            }
            return null;
        }

        #endregion GetCustomObjectVoicePopupData

        #region GetCustomObjectEmailPopupData

        /// <summary>
        /// Gets Custom Object Chat Popup Data
        /// </summary>
        /// <param name="emailData"></param>
        /// <param name="callType"></param>
        /// <param name="objectName"></param>
        /// <returns></returns>
        public CustomObjectData GetCustomObjectEmailPopupData(IXNCustomData emailData, SFDCCallType callType, string objectName)
        {
            try
            {
                this.logger.Info("GetCustomObjectEmailPopupData :  Reading CustomObject Chat Popup Data.....");
                this.logger.Info("GetCustomObjectEmailPopupData :  CallType Name : " + callType.ToString());

                if (emailData != null && customchatOptions.ContainsKey(objectName))
                {
                    CustomObjectData custom = new CustomObjectData();
                    EmailOptions customObjectOptions = customemailOptions[objectName];

                    #region Collect CustomObject Popup data

                    custom.SearchData = this.sfdcUtilityHelper.GetEmailSearchValue(customObjectOptions, emailData, callType);
                    custom.ObjectName = customObjectOptions.ObjectName;
                    custom.NoRecordFound = SFDCObjectHelper.GetNoRecordFoundAction(callType, customObjectOptions);
                    custom.MultipleMatchRecord = SFDCObjectHelper.GetMultiMatchRecordAction(callType, customObjectOptions);
                    custom.NewRecordFieldIds = customObjectOptions.NewrecordFieldIds;
                    custom.SearchCondition = customObjectOptions.SearchCondition;
                    custom.CreateLogForNewRecord = customObjectOptions.CanCreateLogForNewRecordCreate;
                    custom.MaxRecordOpenCount = customObjectOptions.MaxNosRecordOpen;
                    custom.SearchpageMode = customObjectOptions.SearchPageMode;
                    custom.PhoneNumberSearchFormat = customObjectOptions.PhoneNumberSearchFormat;
                    custom.CanCreateNoRecordActivityLog = SFDCObjectHelper.GetCanCreateProfileActivity(callType, customObjectOptions, true);
                    custom.CanPopupNoRecordActivityLog = SFDCObjectHelper.GetCanPopupProfileActivity(callType, customObjectOptions, true);
                    custom.CanCreateMultiMatchActivityLog = SFDCObjectHelper.GetCanCreateProfileActivity(callType, customObjectOptions);
                    custom.CanPopupMultiMatchActivityLog = SFDCObjectHelper.GetCanPopupProfileActivity(callType, customObjectOptions);
                    custom.CanCreateProfileActivityforInbNoRecord = customObjectOptions.CanCreateProfileActivityforInbNoRecord;
                    custom.CanCreateProfileActivityforOutNoRecord = customObjectOptions.CanCreateProfileActivityforOutNoRecord;
                    if (CustomEmailRecordConfigs != null && CustomEmailRecordConfigs.ContainsKey(objectName))
                    {
                        KeyValueCollection RecordConfig = CustomEmailRecordConfigs[objectName];
                        if (RecordConfig != null)
                        {
                            if (custom.NoRecordFound.Equals("createnew"))
                            {
                                custom.CreateRecordFieldData = this.sfdcUtility.GetCreateActivityLogData(RecordConfig, null, callType, emailData);
                            }
                        }
                    }
                    KeyValueCollection LogConfig = null;
                    if (this.CustomEmailLogConfigs != null && this.CustomEmailLogConfigs.ContainsKey(objectName))
                    {
                        LogConfig = this.CustomEmailLogConfigs[objectName];
                    }

                    if (callType == SFDCCallType.InboundEmail || callType == SFDCCallType.InboundEmailPulled)
                    {
                        custom.CreateActvityLog = customObjectOptions.InboundCanCreateLog;
                        if (LogConfig != null)
                        {
                            if (customObjectOptions.InboundCanCreateLog)
                            {
                                custom.ActivityLogData = this.sfdcUtility.GetCreateActivityLogData(LogConfig, null, callType, emailData);
                            }
                        }
                    }
                    else if (callType == SFDCCallType.OutboundEmailFailure || callType == SFDCCallType.OutboundEmailSuccess || callType == SFDCCallType.OutboundEmailPulled)
                    {
                        custom.CreateActvityLog = customObjectOptions.OutboundCanCreateLog;
                        if (LogConfig != null)
                        {
                            if (customObjectOptions.OutboundCanCreateLog)
                            {
                                custom.ActivityLogData = this.sfdcUtility.GetCreateActivityLogData(LogConfig, null, callType, emailData);
                            }
                        }
                    }
                    return custom;
                }

                    #endregion Collect CustomObject Popup data
            }
            catch (Exception generalException)
            {
                this.logger.Error("GetCustomObjectEmailPopupData : Error occurred  : " + generalException.ToString());
            }
            return null;
        }

        #endregion GetCustomObjectEmailPopupData

        #region GetCustomObjectChatPopupData

        /// <summary>
        /// Gets Custom Object Chat Popup Data
        /// </summary>
        /// <param name="message"></param>
        /// <param name="callType"></param>
        /// <param name="objectName"></param>
        /// <returns></returns>
        public CustomObjectData GetCustomObjectChatPopupData(IXNCustomData chatData, SFDCCallType callType, string objectName)
        {
            try
            {
                IMessage message = chatData.InteractionEvent;
                this.logger.Info("GetCustomObjectChatPopupData :  Reading CustomObject Chat Popup Data.....");
                this.logger.Info("GetCustomObjectChatPopupData :  Event Name : " + message.Name);
                this.logger.Info("GetCustomObjectChatPopupData :  CallType Name : " + callType.ToString());
                dynamic popupEvent = Convert.ChangeType(message, message.GetType());

                if (popupEvent != null && customchatOptions.ContainsKey(objectName))
                {
                    CustomObjectData custom = new CustomObjectData();
                    ChatOptions customObjectOptions = customchatOptions[objectName];

                    #region Collect CustomObject Popup data

                    custom.SearchData = this.sfdcUtilityHelper.GetChatSearchValue(customObjectOptions, chatData, callType);
                    custom.ObjectName = customObjectOptions.ObjectName;
                    custom.NoRecordFound = SFDCObjectHelper.GetNoRecordFoundAction(callType, customObjectOptions);
                    custom.MultipleMatchRecord = SFDCObjectHelper.GetMultiMatchRecordAction(callType, customObjectOptions);
                    custom.NewRecordFieldIds = customObjectOptions.NewrecordFieldIds;
                    custom.SearchCondition = customObjectOptions.SearchCondition;
                    custom.CreateLogForNewRecord = customObjectOptions.CanCreateLogForNewRecordCreate;
                    custom.MaxRecordOpenCount = customObjectOptions.MaxNosRecordOpen;
                    custom.SearchpageMode = customObjectOptions.SearchPageMode;
                    custom.CustomObjectURL = customObjectOptions.CustomObjectURL;
                    custom.PhoneNumberSearchFormat = customObjectOptions.PhoneNumberSearchFormat;
                    custom.CanCreateNoRecordActivityLog = SFDCObjectHelper.GetCanCreateProfileActivity(callType, customObjectOptions, true);
                    custom.CanPopupNoRecordActivityLog = SFDCObjectHelper.GetCanPopupProfileActivity(callType, customObjectOptions, true);
                    custom.CanCreateMultiMatchActivityLog = SFDCObjectHelper.GetCanCreateProfileActivity(callType, customObjectOptions);
                    custom.CanPopupMultiMatchActivityLog = SFDCObjectHelper.GetCanPopupProfileActivity(callType, customObjectOptions);
                    custom.CanCreateProfileActivityforInbNoRecord = customObjectOptions.CanCreateProfileActivityforInbNoRecord;
                    if (CustomChatRecordConfigs != null && CustomChatRecordConfigs.ContainsKey(objectName))
                    {
                        KeyValueCollection RecordConfig = CustomChatRecordConfigs[objectName];
                        if (RecordConfig != null)
                        {
                            if (custom.NoRecordFound.Equals("createnew"))
                            {
                                custom.CreateRecordFieldData = this.sfdcUtility.GetCreateActivityLogData(RecordConfig, message, callType, emailData: chatData);
                            }
                        }
                    }
                    KeyValueCollection LogConfig = null;
                    if (this.CustomChatLogConfigs != null && this.CustomChatLogConfigs.ContainsKey(objectName))
                    {
                        LogConfig = this.CustomChatLogConfigs[objectName];
                    }

                    if (callType == SFDCCallType.InboundChat)
                    {
                        custom.CreateActvityLog = customObjectOptions.InboundCanCreateLog;
                        if (LogConfig != null)
                        {
                            if (customObjectOptions.InboundCanCreateLog)
                            {
                                custom.ActivityLogData = this.sfdcUtility.GetCreateActivityLogData(LogConfig, popupEvent, callType, emailData: chatData);
                            }
                        }
                    }
                    else if (callType == SFDCCallType.ConsultChatReceived)
                    {
                        custom.CreateActvityLog = customObjectOptions.ConsultCanCreateLog;
                        if (LogConfig != null)
                        {
                            if (customObjectOptions.ConsultCanCreateLog)
                            {
                                custom.ActivityLogData = this.sfdcUtility.GetCreateActivityLogData(LogConfig, popupEvent, callType, emailData: chatData);
                            }
                        }
                    }
                    return custom;
                }

                    #endregion Collect CustomObject Popup data
            }
            catch (Exception generalException)
            {
                this.logger.Error("GetCustomObjectChatPopupData : Error occurred  : " + generalException.ToString());
            }
            return null;
        }

        #endregion GetCustomObjectChatPopupData

        #region GetCusotmObjectVoiceUpdateData

        /// <summary>
        /// Gets the Custom Objects Update Data
        /// </summary>
        /// <param name="message"></param>
        /// <param name="callType"></param>
        /// <param name="duration"></param>
        /// <param name="objectName"></param>
        /// <returns></returns>
        public CustomObjectData GetCusotmObjectVoiceUpdateData(IMessage message, SFDCCallType callType, string duration, string objectName)
        {
            try
            {
                this.logger.Info("GetCusotmObjectVoiceUpdateData :  Reading CustomObject Update Data.....");
                this.logger.Info("GetCusotmObjectVoiceUpdateData :  Event Name : " + message.Name);
                dynamic popupEvent = Convert.ChangeType(message, message.GetType());

                if (popupEvent != null && customvoiceOptions.ContainsKey(objectName))
                {
                    CustomObjectData customObject = new CustomObjectData();
                    VoiceOptions currentObjectOptions = customvoiceOptions[objectName];
                    KeyValueCollection currentObjectConfigs = null;
                    KeyValueCollection currentObjectRecordConfig = null;
                    if (CustomVoiceLogConfigs != null && CustomVoiceLogConfigs.ContainsKey(objectName))
                        currentObjectConfigs = CustomVoiceLogConfigs[objectName];
                    if (CustomVoiceRecordConfigs != null && CustomVoiceRecordConfigs.ContainsKey(objectName))
                        currentObjectRecordConfig = CustomVoiceRecordConfigs[objectName];

                    #region Collect customObject Data

                    if (currentObjectOptions != null)
                    {
                        customObject.ObjectName = currentObjectOptions.ObjectName;
                        customObject.CustomObjectURL = currentObjectOptions.CustomObjectURL;
                        if (callType == SFDCCallType.InboundVoice)
                        {
                            if (currentObjectOptions.InboundCanUpdateLog)
                            {
                                customObject.UpdateActivityLog = true;
                                customObject.UpdateActivityLogData = this.sfdcUtility.GetUpdateActivityLogData(currentObjectConfigs, popupEvent, callType, duration);
                            }
                        }
                        else if (callType == SFDCCallType.OutboundVoiceSuccess || callType == SFDCCallType.OutboundVoiceFailure)
                        {
                            if (currentObjectOptions.OutboundCanUpdateLog)
                            {
                                customObject.UpdateActivityLog = true;
                                customObject.UpdateActivityLogData = this.sfdcUtility.GetUpdateActivityLogData(currentObjectConfigs, popupEvent, callType, duration);
                            }
                        }
                        else if (callType == SFDCCallType.ConsultVoiceReceived)
                        {
                            if (currentObjectOptions.ConsultCanUpdateLog)
                            {
                                customObject.UpdateActivityLog = true;
                                customObject.UpdateActivityLogData = this.sfdcUtility.GetUpdateActivityLogData(currentObjectConfigs, popupEvent, callType, duration);
                            }
                        }

                        if (currentObjectRecordConfig != null)
                        {
                            if (currentObjectOptions.CanUpdateRecordData)
                            {
                                customObject.UpdateRecordFields = true;
                                customObject.UpdateRecordFieldsData = this.sfdcUtility.GetUpdateActivityLogData(currentObjectRecordConfig, popupEvent, callType, duration);
                            }
                        }
                    }

                    #endregion Collect customObject Data

                    return customObject;
                }
            }
            catch (Exception generalException)
            {
                this.logger.Error("GetCusotmObjectVoiceUpdateData : Error occurred while reading customObject Data : " + generalException.ToString());
            }
            return null;
        }

        #endregion GetCusotmObjectVoiceUpdateData

        #region GetCustomObjectEmailUpdateData

        public CustomObjectData GetCustomObjectEmailUpdateData(IXNCustomData emailData, SFDCCallType callType, string objectName, string eventName)
        {
            CustomObjectData customObject = new CustomObjectData();
            try
            {
                this.logger.Info("GetCustomObjectOutboundEmailUpdateData :  Reading CustomObject Chat Update Data.....");

                if (emailData != null && customemailOptions.ContainsKey(objectName))
                {
                    EmailOptions currentObjectOptions = this.customemailOptions[objectName];
                    KeyValueCollection currentObjectLogConfigs = null;
                    KeyValueCollection currentObjectRecordConfig = null;
                    if (CustomEmailLogConfigs != null && CustomEmailLogConfigs.ContainsKey(objectName))
                        currentObjectLogConfigs = CustomEmailLogConfigs[objectName];
                    if (CustomEmailRecordConfigs != null && CustomEmailRecordConfigs.ContainsKey(objectName))
                        currentObjectRecordConfig = CustomEmailRecordConfigs[objectName];

                    #region Collect customObject Data

                    customObject.ObjectName = currentObjectOptions.ObjectName;
                    if (callType == SFDCCallType.InboundEmail || callType == SFDCCallType.InboundEmailPulled)
                    {
                        if (currentObjectOptions.InboundCanUpdateLog)
                        {
                            customObject.UpdateActivityLog = true;
                            customObject.UpdateActivityLogData = this.sfdcUtility.GetUpdateActivityLogData(currentObjectLogConfigs, null, callType, emailData.Duration, emailData: emailData);
                            if (!string.IsNullOrWhiteSpace(currentObjectOptions.EmailAppendActivityLogEventNames) && currentObjectOptions.EmailAppendActivityLogEventNames.Contains(eventName))
                                customObject.AppendActivityLogData = this.sfdcUtility.GetUpdateActivityLogData(currentObjectLogConfigs, null, callType, emailData.Duration, emailData: emailData, isAppendLogData: true);
                        }
                    }
                    else if (callType == SFDCCallType.OutboundEmailFailure || callType == SFDCCallType.OutboundEmailSuccess || callType == SFDCCallType.OutboundEmailPulled)
                    {
                        if (currentObjectOptions.OutboundCanUpdateLog)
                        {
                            customObject.UpdateActivityLog = true;
                            customObject.UpdateActivityLogData = this.sfdcUtility.GetUpdateActivityLogData(currentObjectLogConfigs, null, callType, emailData.Duration, emailData: emailData);
                            if (!string.IsNullOrWhiteSpace(currentObjectOptions.EmailAppendActivityLogEventNames) && currentObjectOptions.EmailAppendActivityLogEventNames.Contains(eventName))
                                customObject.AppendActivityLogData = this.sfdcUtility.GetUpdateActivityLogData(currentObjectLogConfigs, null, callType, emailData.Duration, emailData: emailData, isAppendLogData: true);
                        }
                    }

                    //update CustomObject record fields
                    customObject.UpdateRecordFields = customemailOptions[objectName].CanUpdateRecordData;
                    customObject.NoRecordFound = SFDCObjectHelper.GetNoRecordFoundAction(callType, customemailOptions[objectName]);
                    if (customObject.NoRecordFound.Equals("createnew") && this.CustomEmailRecordConfigs[objectName] != null)
                    {
                        if (customemailOptions[objectName].CanUpdateRecordData)
                        {
                            customObject.UpdateRecordFieldsData = this.sfdcUtility.GetUpdateActivityLogData(this.CustomEmailRecordConfigs[objectName], null, callType, emailData.Duration, emailData: emailData);
                        }
                    }

                    #endregion Collect customObject Data

                    return customObject;
                }
            }
            catch (Exception generalException)
            {
                this.logger.Error("GetCustomObjectOutboundEmailUpdateData : Error occurred : " + generalException.ToString());
                return customObject;
            }
            return null;
        }

        #endregion GetCustomObjectEmailUpdateData

        #region GetCustomObjectChatUpdateData

        public CustomObjectData GetCustomObjectChatUpdateData(IXNCustomData chatData, string objectName, string eventName)
        {
            try
            {
                IMessage message = chatData.InteractionEvent;
                SFDCCallType callType = chatData.InteractionType;
                string callDuration = chatData.Duration;
                this.logger.Info("GetCustomObjectChatUpdateData :  Reading CustomObject Chat Update Data.....");
                this.logger.Info("GetCustomObjectChatUpdateData :  Event Name : " + message.Name);
                dynamic popupEvent = Convert.ChangeType(message, message.GetType());

                if (popupEvent != null && customchatOptions.ContainsKey(objectName))
                {
                    CustomObjectData customObject = new CustomObjectData();
                    ChatOptions currentObjectOptions = this.customchatOptions[objectName];
                    KeyValueCollection currentObjectLogConfigs = null;
                    KeyValueCollection currentObjectRecordConfig = null;
                    if (CustomChatLogConfigs != null && CustomChatLogConfigs.ContainsKey(objectName))
                        currentObjectLogConfigs = CustomChatLogConfigs[objectName];
                    if (CustomChatRecordConfigs != null && CustomChatRecordConfigs.ContainsKey(objectName))
                        currentObjectRecordConfig = CustomChatRecordConfigs[objectName];

                    #region Collect customObject Data

                    customObject.ObjectName = currentObjectOptions.ObjectName;

                    if (callType == SFDCCallType.InboundChat)
                    {
                        if (currentObjectOptions.InboundCanUpdateLog)
                        {
                            customObject.UpdateActivityLog = true;
                            customObject.UpdateActivityLogData = this.sfdcUtility.GetUpdateActivityLogData(currentObjectLogConfigs, popupEvent, callType, callDuration, emailData: chatData);
                            if (!string.IsNullOrWhiteSpace(currentObjectOptions.ChatAppendActivityLogEventNames) && currentObjectOptions.ChatAppendActivityLogEventNames.Contains(eventName))
                                customObject.AppendActivityLogData = this.sfdcUtility.GetUpdateActivityLogData(currentObjectLogConfigs, null, callType, callDuration, emailData: chatData, isAppendLogData: true);
                        }
                    }
                    if (callType == SFDCCallType.ConsultChatReceived)
                    {
                        if (currentObjectOptions.ConsultCanUpdateLog)
                        {
                            customObject.UpdateActivityLog = true;
                            customObject.UpdateActivityLogData = this.sfdcUtility.GetUpdateActivityLogData(currentObjectLogConfigs, popupEvent, callType, callDuration, emailData: chatData);
                            if (!string.IsNullOrWhiteSpace(currentObjectOptions.ChatAppendActivityLogEventNames) && currentObjectOptions.ChatAppendActivityLogEventNames.Contains(eventName))
                                customObject.AppendActivityLogData = this.sfdcUtility.GetUpdateActivityLogData(currentObjectLogConfigs, null, callType, callDuration, emailData: chatData, isAppendLogData: true);
                        }
                    }

                    //update CustomObject record fields
                    customObject.UpdateRecordFields = customchatOptions[objectName].CanUpdateRecordData;
                    if (SFDCObjectHelper.GetNoRecordFoundAction(callType, customchatOptions[objectName]).Equals("createnew") && this.CustomChatRecordConfigs[objectName] != null)
                    {
                        if (customchatOptions[objectName].CanUpdateRecordData)
                        {
                            customObject.UpdateRecordFieldsData = this.sfdcUtility.GetUpdateActivityLogData(this.CustomChatRecordConfigs[objectName], popupEvent, callType, callDuration, emailData: chatData);
                        }
                    }

                    #endregion Collect customObject Data

                    return customObject;
                }
            }
            catch (Exception generalException)
            {
                this.logger.Error("GetCustomObjectChatUpdateData : Error occurred : " + generalException.ToString());
            }
            return null;
        }

        #endregion GetCustomObjectChatUpdateData
    }
}