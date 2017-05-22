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
    /// Comment: Provides Search and Activity Data for CustomObject
    /// Last Modified: 25-08-2015
    /// Created by: Pointel Inc
    /// </summary>
    internal class SFDCCustomObject
    {
        #region Field Declaration

        private static SFDCCustomObject customObject = null;
        private Log logger = null;
        private Dictionary<string, VoiceOptions> customvoiceOptions = null;
        private Dictionary<string, ChatOptions> customchatOptions = null;
        private IDictionary<string, KeyValueCollection> CustomVoiceLogConfigs = null;
        private Dictionary<string, KeyValueCollection> CustomRecordConfigs = null;
        private IDictionary<string, KeyValueCollection> CustomChatLogConfigs = null;
        private SFDCUtility sfdcObject = null;

        #endregion Field Declaration

        #region Constructor

        private SFDCCustomObject()
        {
            this.logger = Log.GenInstance();
            this.customvoiceOptions = Settings.CustomObjectVoiceOptions;
            this.customchatOptions = Settings.CustomObjectChatOptions;
            this.CustomVoiceLogConfigs = Settings.VoiceActivityLogCollection;
            this.CustomRecordConfigs = Settings.CustomObjectNewRecordConfigs;
            this.CustomChatLogConfigs = Settings.ChatActivityLogCollection;
            this.sfdcObject = SFDCUtility.GetInstance();
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
        public ICustomObject GetCustomObjectVoicePopupData(IMessage message, SFDCCallType callType, string objectName)
        {
            try
            {
                this.logger.Info("GetCustomObjectVoicePopupData :  Reading CustomObject Popup Data.....");
                this.logger.Info("GetCustomObjectVoicePopupData :  Event Name : " + message.Name);
                this.logger.Info("GetCustomObjectVoicePopupData :  CallType Name : " + callType.ToString());
                dynamic popupEvent = Convert.ChangeType(message, message.GetType());
                if (popupEvent != null && customvoiceOptions.ContainsKey(objectName))
                {
                    ICustomObject custom = new CustomObjectData();
                    VoiceOptions customObjectOptions = customvoiceOptions[objectName];

                    #region Collect CustomObject Popup data

                    custom.SearchData = GetVoiceSearchValue(popupEvent.UserData, message, customObjectOptions, callType);
                    custom.ObjectName = customObjectOptions.ObjectName;
                    custom.NoRecordFound = GetNoRecordFoundAction(callType, customObjectOptions);
                    custom.MultipleMatchRecord = GetMultiMatchRecordAction(callType, customObjectOptions);
                    custom.NewRecordFieldIds = customObjectOptions.NewrecordFieldIds;
                    custom.SearchCondition = customObjectOptions.SearchCondition;
                    custom.CreateLogForNewRecord = customObjectOptions.CanCreateLogForNewRecordCreate;
                    custom.MaxRecordOpenCount = customObjectOptions.MaxNosRecordOpen;
                    custom.SearchpageMode = customObjectOptions.SearchPageMode;
                    custom.CustomObjectURL = customObjectOptions.CustomObjectURL;
                    custom.PhoneNumberSearchFormat = customObjectOptions.PhoneNumberSearchFormat;
                    custom.CanCreateNoRecordActivityLog = GetCanCreateProfileActivity(callType, customObjectOptions, true);
                    custom.CanPopupNoRecordActivityLog = GetCanPopupProfileActivity(callType, customObjectOptions, true);
                    custom.CanCreateMultiMatchActivityLog = GetCanCreateProfileActivity(callType, customObjectOptions);
                    custom.CanPopupMultiMatchActivityLog = GetCanPopupProfileActivity(callType, customObjectOptions);
                    custom.CanCreateProfileActivityforInbNoRecord = customObjectOptions.CanCreateProfileActivityforInbNoRecord;
                    custom.CanCreateProfileActivityforOutNoRecord = customObjectOptions.CanCreateProfileActivityforOutNoRecord;
                    custom.CanCreateProfileActivityforConNoRecord = customObjectOptions.CanCreateProfileActivityforConNoRecord;

                    if (CustomRecordConfigs != null && CustomRecordConfigs.ContainsKey(objectName))
                    {
                        KeyValueCollection RecordConfig = CustomRecordConfigs[objectName];
                        if (RecordConfig != null)
                        {
                            if (custom.NoRecordFound.Equals("createnew"))
                            {
                                custom.CreateRecordFieldData = this.sfdcObject.GetVoiceRecordData(RecordConfig, message, callType);
                            }
                        }
                    }
                    KeyValueCollection LogConfig = null;
                    if (this.CustomVoiceLogConfigs != null && this.CustomVoiceLogConfigs.ContainsKey(objectName))
                    {
                        LogConfig = this.CustomVoiceLogConfigs[objectName];
                    }

                    if (callType == SFDCCallType.Inbound)
                    {
                        custom.CreateActvityLog = customObjectOptions.InboundCanCreateLog;
                        if (LogConfig != null)
                        {
                            if (customObjectOptions.InboundCanCreateLog)
                            {
                                custom.ActivityLogData = this.sfdcObject.GetVoiceActivityLog(LogConfig, popupEvent, callType);
                            }
                        }
                    }
                    else if (callType == SFDCCallType.OutboundSuccess)
                    {
                        custom.CreateActvityLog = customObjectOptions.OutboundCanCreateLog;
                        if (LogConfig != null)
                        {
                            if (customObjectOptions.OutboundCanCreateLog)
                            {
                                custom.ActivityLogData = this.sfdcObject.GetVoiceActivityLog(LogConfig, popupEvent, callType);
                            }
                        }
                    }
                    else if (callType == SFDCCallType.OutboundFailure)
                    {
                        custom.CreateActvityLog = customObjectOptions.OutboundFailureCanCreateLog;
                        if (LogConfig != null)
                        {
                            if (customObjectOptions.OutboundFailureCanCreateLog)
                            {
                                custom.ActivityLogData = this.sfdcObject.GetVoiceActivityLog(LogConfig, popupEvent, callType);
                            }
                        }
                    }
                    else if (callType == SFDCCallType.ConsultReceived)
                    {
                        custom.CreateActvityLog = customObjectOptions.ConsultCanCreateLog;
                        if (LogConfig != null)
                        {
                            if (customObjectOptions.ConsultCanCreateLog)
                            {
                                custom.ActivityLogData = this.sfdcObject.GetVoiceActivityLog(LogConfig, popupEvent, callType);
                            }
                        }
                    }
                    else if (callType == SFDCCallType.ConsultSuccess)
                    {
                        custom.NoRecordFound = "none";
                        custom.CreateActvityLog = customObjectOptions.ConsultCanCreateLog;
                        if (LogConfig != null)
                        {
                            if (customObjectOptions.ConsultCanCreateLog)
                            {
                                custom.ActivityLogData = this.sfdcObject.GetVoiceActivityLog(LogConfig, popupEvent, callType);
                            }
                        }
                    }
                    else if (callType == SFDCCallType.ConsultFailure)
                    {
                        custom.NoRecordFound = "none";
                        custom.CreateActvityLog = customObjectOptions.ConsultFailureCanCreateLog;
                        if (LogConfig != null)
                        {
                            if (customObjectOptions.ConsultFailureCanCreateLog)
                            {
                                custom.ActivityLogData = this.sfdcObject.GetVoiceActivityLog(LogConfig, popupEvent, callType);
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

        #region GetVoiceSearchValue

        /// <summary>
        /// Gets Voice search Valus
        /// </summary>
        /// <param name="userData"></param>
        /// <param name="message"></param>
        /// <param name="voiceOptions"></param>
        /// <param name="callType"></param>
        /// <returns></returns>
        private string GetVoiceSearchValue(KeyValueCollection userData, IMessage message, VoiceOptions voiceOptions, SFDCCallType callType)
        {
            try
            {
                this.logger.Info("GetVoiceSearchValue :  Reading Custom Object Popup Data.....");
                this.logger.Info("GetVoiceSearchValue :  UserData Name : " + Convert.ToString(userData));
                this.logger.Info("GetVoiceSearchValue :  Event Name : " + message.Name);
                this.logger.Info("GetVoiceSearchValue :  CallType Name : " + callType.ToString());
                string[] userDataSearchKeys = null;
                string[] attributeSearchKeys = null;
                string searchValue = string.Empty;
                if (callType == SFDCCallType.Inbound)
                {
                    userDataSearchKeys = (voiceOptions.InboundSearchUserDataKeys != null) ? voiceOptions.InboundSearchUserDataKeys.Split(',') : null;
                    attributeSearchKeys = (voiceOptions.InboundSearchAttributeKeys != null) ? voiceOptions.InboundSearchAttributeKeys.Split(',') : null;
                }
                else if (callType == SFDCCallType.OutboundSuccess || callType == SFDCCallType.OutboundFailure)
                {
                    userDataSearchKeys = (voiceOptions.OutboundSearchUserDataKeys != null) ? voiceOptions.OutboundSearchUserDataKeys.Split(',') : null;
                    attributeSearchKeys = (voiceOptions.OutboundSearchAttributeKeys != null) ? voiceOptions.OutboundSearchAttributeKeys.Split(',') : null;
                }
                else if (callType == SFDCCallType.ConsultSuccess || callType == SFDCCallType.ConsultFailure)
                {
                    return this.sfdcObject.GetAttributeSearchValues(message, new string[] { "otherdn" });
                }
                else if (callType == SFDCCallType.ConsultReceived)
                {
                    userDataSearchKeys = (voiceOptions.ConsultSearchUserDataKeys != null) ? voiceOptions.ConsultSearchUserDataKeys.Split(',') : null;
                    attributeSearchKeys = (voiceOptions.ConsultSearchAttributeKeys != null) ? voiceOptions.ConsultSearchAttributeKeys.Split(',') : null;
                }

                #region OLD

                //if (voiceOptions.SearchPriority == "user-data")
                //{
                //    if (userDataSearchKeys != null)
                //        searchValue = this.sfdcObject.GetUserDataSearchValues(userData, userDataSearchKeys);
                //    else if (attributeSearchKeys != null)
                //        searchValue = this.sfdcObject.GetAttributeSearchValues(message, attributeSearchKeys);
                //}
                //else if (voiceOptions.SearchPriority == "attribute")
                //{
                //    searchValue = this.sfdcObject.GetAttributeSearchValues(message, attributeSearchKeys);
                //}
                //else if (voiceOptions.SearchPriority == "both")
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

                if (voiceOptions.SearchPriority == "user-data")
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
                else if (voiceOptions.SearchPriority == "attribute")
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
                else if (voiceOptions.SearchPriority == "both")
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
                this.logger.Error("GetVoiceSearchValue : Error occurred while reading Search Values : " + generalException.ToString());
            }
            return null;
        }

        #endregion GetVoiceSearchValue

        #region GetCusotmObjectVoiceUpdateData

        /// <summary>
        /// Gets the Custom Objects Update Data
        /// </summary>
        /// <param name="message"></param>
        /// <param name="callType"></param>
        /// <param name="duration"></param>
        /// <param name="objectName"></param>
        /// <returns></returns>
        public ICustomObject GetCusotmObjectVoiceUpdateData(IMessage message, SFDCCallType callType, string duration, string objectName)
        {
            try
            {
                this.logger.Info("GetCusotmObjectVoiceUpdateData :  Reading CustomObject Update Data.....");
                this.logger.Info("GetCusotmObjectVoiceUpdateData :  Event Name : " + message.Name);
                dynamic popupEvent = Convert.ChangeType(message, message.GetType());

                if (popupEvent != null && customvoiceOptions.ContainsKey(objectName))
                {
                    ICustomObject customObject = new CustomObjectData();
                    VoiceOptions currentObjectOptions = customvoiceOptions[objectName];
                    KeyValueCollection currentObjectConfigs = null;
                    KeyValueCollection currentObjectRecordConfig = null;
                    if (CustomVoiceLogConfigs != null && CustomVoiceLogConfigs.ContainsKey(objectName))
                        currentObjectConfigs = CustomVoiceLogConfigs[objectName];
                    if (CustomRecordConfigs != null && CustomRecordConfigs.ContainsKey(objectName))
                        currentObjectRecordConfig = CustomRecordConfigs[objectName];

                    #region Collect customObject Data

                    if (currentObjectOptions != null)
                    {
                        customObject.ObjectName = currentObjectOptions.ObjectName;
                        customObject.CustomObjectURL = currentObjectOptions.CustomObjectURL;
                        if (callType == SFDCCallType.Inbound)
                        {
                            if (currentObjectOptions.InboundCanUpdateLog)
                            {
                                customObject.UpdateActivityLog = true;
                                customObject.UpdateActivityLogData = this.sfdcObject.GetVoiceUpdateActivityLog(currentObjectConfigs, popupEvent, callType, duration);
                            }
                        }
                        else if (callType == SFDCCallType.OutboundSuccess || callType == SFDCCallType.OutboundFailure)
                        {
                            if (currentObjectOptions.OutboundCanUpdateLog)
                            {
                                customObject.UpdateActivityLog = true;
                                customObject.UpdateActivityLogData = this.sfdcObject.GetVoiceUpdateActivityLog(currentObjectConfigs, popupEvent, callType, duration);
                            }
                        }
                        else if (callType == SFDCCallType.ConsultSuccess || callType == SFDCCallType.ConsultReceived || callType == SFDCCallType.ConsultFailure)
                        {
                            if (currentObjectOptions.ConsultCanUpdateLog)
                            {
                                customObject.UpdateActivityLog = true;
                                customObject.UpdateActivityLogData = this.sfdcObject.GetVoiceUpdateActivityLog(currentObjectConfigs, popupEvent, callType, duration);
                            }
                        }

                        if (currentObjectRecordConfig != null)
                        {
                            if (currentObjectOptions.CanUpdateRecordData)
                            {
                                customObject.UpdateRecordFields = true;
                                customObject.UpdateRecordFieldsData = this.sfdcObject.GetVoiceUpdateRecordData(currentObjectRecordConfig, popupEvent, callType, duration);
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

        #region GetCustomObjectChatPopupData

        /// <summary>
        /// Gets Custom Object Chat Popup Data
        /// </summary>
        /// <param name="message"></param>
        /// <param name="callType"></param>
        /// <param name="objectName"></param>
        /// <returns></returns>
        public ICustomObject GetCustomObjectChatPopupData(IMessage message, SFDCCallType callType, string objectName)
        {
            try
            {
                this.logger.Info("GetCustomObjectChatPopupData :  Reading CustomObject Chat Popup Data.....");
                this.logger.Info("GetCustomObjectChatPopupData :  Event Name : " + message.Name);
                this.logger.Info("GetCustomObjectChatPopupData :  CallType Name : " + callType.ToString());
                dynamic popupEvent = Convert.ChangeType(message, message.GetType());

                if (popupEvent != null && customchatOptions.ContainsKey(objectName))
                {
                    ICustomObject custom = new CustomObjectData();
                    ChatOptions customObjectOptions = customchatOptions[objectName];

                    #region Collect CustomObject Popup data

                    custom.SearchData = GetChatSearchValue(popupEvent.Interaction.InteractionUserData, message, customObjectOptions, callType);
                    custom.ObjectName = customObjectOptions.ObjectName;
                    custom.NoRecordFound = GetNoRecordFoundAction(callType, customObjectOptions);
                    custom.MultipleMatchRecord = GetMultiMatchRecordAction(callType, customObjectOptions);
                    custom.NewRecordFieldIds = customObjectOptions.NewrecordFieldIds;
                    custom.SearchCondition = customObjectOptions.SearchCondition;
                    custom.CreateLogForNewRecord = customObjectOptions.CanCreateLogForNewRecordCreate;
                    custom.MaxRecordOpenCount = customObjectOptions.MaxNosRecordOpen;
                    custom.SearchpageMode = customObjectOptions.SearchPageMode;
                    custom.CustomObjectURL = customObjectOptions.CustomObjectURL;
                    custom.PhoneNumberSearchFormat = customObjectOptions.PhoneNumberSearchFormat;
                    if (CustomRecordConfigs != null && CustomRecordConfigs.ContainsKey(objectName))
                    {
                        KeyValueCollection RecordConfig = CustomRecordConfigs[objectName];
                        if (RecordConfig != null)
                        {
                            if (custom.NoRecordFound.Equals("createnew"))
                            {
                                custom.CreateRecordFieldData = this.sfdcObject.GetChatRecordData(RecordConfig, message, callType);
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
                                custom.ActivityLogData = this.sfdcObject.GetChatActivityLog(LogConfig, popupEvent, callType);
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
                                custom.ActivityLogData = this.sfdcObject.GetChatActivityLog(LogConfig, popupEvent, callType);
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

        #region GetCustomObjectChatUpdateData

        public ICustomObject GetCustomObjectChatUpdateData(IMessage message, SFDCCallType callType, string callDuration, string chatContent, string objectName)
        {
            try
            {
                this.logger.Info("GetCustomObjectChatUpdateData :  Reading CustomObject Chat Update Data.....");
                this.logger.Info("GetCustomObjectChatUpdateData :  Event Name : " + message.Name);
                dynamic popupEvent = Convert.ChangeType(message, message.GetType());

                if (popupEvent != null && customchatOptions.ContainsKey(objectName))
                {
                    ICustomObject customObject = new CustomObjectData();
                    ChatOptions currentObjectOptions = this.customchatOptions[objectName];
                    KeyValueCollection currentObjectLogConfigs = null;
                    KeyValueCollection currentObjectRecordConfig = null;
                    if (CustomChatLogConfigs != null && CustomChatLogConfigs.ContainsKey(objectName))
                        currentObjectLogConfigs = CustomChatLogConfigs[objectName];
                    if (CustomRecordConfigs != null && CustomRecordConfigs.ContainsKey(objectName))
                        currentObjectRecordConfig = CustomRecordConfigs[objectName];

                    #region Collect Lead Data

                    customObject.ObjectName = currentObjectOptions.ObjectName;

                    if (callType == SFDCCallType.InboundChat)
                    {
                        if (currentObjectOptions.InboundCanUpdateLog)
                        {
                            customObject.UpdateActivityLog = true;
                            customObject.UpdateActivityLogData = this.sfdcObject.GetChatUpdateActivityLog(currentObjectLogConfigs, popupEvent, callType, callDuration, chatContent);
                        }
                    }
                    if (callType == SFDCCallType.ConsultChatReceived)
                    {
                        if (currentObjectOptions.ConsultCanUpdateLog)
                        {
                            customObject.UpdateActivityLog = true;
                            customObject.UpdateActivityLogData = this.sfdcObject.GetChatUpdateActivityLog(currentObjectLogConfigs, popupEvent, callType, callDuration, chatContent);
                        }
                    }
                    else if (callType == SFDCCallType.ConsultChatSuccess || callType == SFDCCallType.ConsultChatFailure)
                    {
                        if (currentObjectOptions.ConsultCanUpdateLog)
                        {
                            customObject.UpdateActivityLog = true;
                            customObject.UpdateActivityLogData = this.sfdcObject.GetChatUpdateActivityLog(currentObjectLogConfigs, popupEvent, callType, callDuration, chatContent);
                        }
                    }
                    //update CustomObject record fields
                    customObject.UpdateRecordFields = customchatOptions[objectName].CanUpdateRecordData;
                    if (GetNoRecordFoundAction(callType, customchatOptions[objectName]).Equals("createnew") && this.CustomRecordConfigs[objectName] != null)
                    {
                        if (customchatOptions[objectName].CanUpdateRecordData)
                        {
                            customObject.UpdateRecordFieldsData = this.sfdcObject.GetChatUpdateActivityLog(this.CustomRecordConfigs[objectName], popupEvent, callType, callDuration, chatContent);
                        }
                    }

                    #endregion Collect Lead Data

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

        #region GetChatSearchValue

        /// <summary>
        /// Gets Voice search Valus
        /// </summary>
        /// <param name="userData"></param>
        /// <param name="message"></param>
        /// <param name="chatOptions"></param>
        /// <param name="callType"></param>
        /// <returns></returns>
        private string GetChatSearchValue(KeyValueCollection userData, IMessage message, ChatOptions chatOptions, SFDCCallType callType)
        {
            try
            {
                this.logger.Info("GetChatSearchValue :  Reading Custom Popup Data.....");
                this.logger.Info("GetChatSearchValue :  UserData Name : " + Convert.ToString(userData));
                this.logger.Info("GetChatSearchValue :  Event Name : " + message.Name);
                this.logger.Info("GetChatSearchValue :  CallType Name : " + callType.ToString());
                string[] userDataSearchKeys = null;
                string[] attributeSearchKeys = null;
                string searchValue = string.Empty;
                if (callType == SFDCCallType.InboundChat)
                {
                    userDataSearchKeys = (chatOptions.InboundSearchUserDataKeys != null) ? chatOptions.InboundSearchUserDataKeys.Split(',') : null;
                    attributeSearchKeys = (chatOptions.InboundSearchAttributeKeys != null) ? chatOptions.InboundSearchAttributeKeys.Split(',') : null;
                }
                else if (callType == SFDCCallType.ConsultChatReceived)
                {
                    userDataSearchKeys = (chatOptions.ConsultSearchUserDataKeys != null) ? chatOptions.ConsultSearchUserDataKeys.Split(',') : null;
                    attributeSearchKeys = (chatOptions.ConsultSearchAttributeKeys != null) ? chatOptions.ConsultSearchAttributeKeys.Split(',') : null;
                }
                if (chatOptions.SearchPriority == "user-data")
                {
                    searchValue = this.sfdcObject.GetUserDataSearchValues(userData, userDataSearchKeys);
                }
                else if (chatOptions.SearchPriority == "attribute")
                {
                    searchValue = this.sfdcObject.GetAttributeSearchValues(message, attributeSearchKeys);
                }
                else if (chatOptions.SearchPriority == "both")
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
                this.logger.Error("GetChatSearchValue : Error occurred while reading Search Values : " + generalException.ToString());
            }
            return null;
        }

        #endregion GetChatSearchValue

        #region GetMultiMatchRecordAction for voice

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

        #endregion GetMultiMatchRecordAction for voice

        #region GetMultiMatchRecordAction for chat

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

        #endregion GetMultiMatchRecordAction for chat

        #region GetNoRecordFoundAction

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

        #endregion GetNoRecordFoundAction

        #region GetNoRecordFoundAction  for Chat

        private string GetNoRecordFoundAction(SFDCCallType calltype, ChatOptions options)
        {
            string NoMatchRecordAction = string.Empty;
            switch (calltype)
            {
                case SFDCCallType.InboundChat:
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

        #endregion GetNoRecordFoundAction  for Chat

        #region GetCanCreateProfileActivity

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

        #endregion GetCanCreateProfileActivity

        #region GetCanPopupProfileActivity

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

        #endregion GetCanPopupProfileActivity
    }
}