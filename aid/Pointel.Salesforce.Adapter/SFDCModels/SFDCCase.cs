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
    /// Comment: Provides Search and Activity Data for Case Object
    /// Last Modified: 25-08-2015
    /// Created by: Pointel Inc
    /// </summary>
    internal class SFDCCase
    {
        #region Fields

        private static SFDCCase caseData = null;

        private KeyValueCollection CaseChatLogOptions = null;
        private ChatOptions CaseChatOptions = null;
        private KeyValueCollection CaseLogConfig = null;
        private KeyValueCollection CaseRecordConfig = null;
        private VoiceOptions CaseVoiceOptions = null;
        private Log logger = null;
        private SFDCUtility sfdcObject = null;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        private SFDCCase()
        {
            this.logger = Log.GenInstance();
            this.CaseVoiceOptions = Settings.CaseVoiceOptions;
            this.CaseChatOptions = Settings.CaseChatOptions;
            this.CaseLogConfig = (Settings.VoiceActivityLogCollection.ContainsKey("case")) ? Settings.VoiceActivityLogCollection["case"] : null;
            this.CaseChatLogOptions = (Settings.ChatActivityLogCollection.ContainsKey("case")) ? Settings.ChatActivityLogCollection["case"] : null;
            this.CaseRecordConfig = Settings.CaseNewRecordConfigs;
            this.sfdcObject = SFDCUtility.GetInstance();
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Gets the Instance of the Class
        /// </summary>
        /// <returns></returns>
        public static SFDCCase GetInstance()
        {
            if (caseData == null)
            {
                caseData = new SFDCCase();
            }
            return caseData;
        }

        /// <summary>
        /// Gets Case Popup Data
        /// </summary>
        /// <param name="message"></param>
        /// <param name="callType"></param>
        /// <returns></returns>
        public ICase GetCaseChatPopupData(IMessage message, SFDCCallType callType)
        {
            try
            {
                this.logger.Info("GetCaseChatPopupData :  Reading Case Popup Data.....");
                this.logger.Info("GetCaseChatPopupData :  Event Name : " + message.Name);
                this.logger.Info("GetCaseChatPopupData :  CallType Name : " + callType.ToString());
                dynamic popupEvent = Convert.ChangeType(message, message.GetType());

                if (popupEvent != null)
                {
                    ICase cases = new CaseData();
                    cases.SearchData = GetCaseChatSearchValue(popupEvent.Interaction.InteractionUserData, message, callType);
                    cases.ObjectName = CaseChatOptions.ObjectName;
                    cases.SearchFields = CaseChatOptions.SearchFields;
                    cases.NoRecordFound = GetNoRecordFoundAction(callType, CaseChatOptions);
                    cases.MultipleMatchRecord = GetMultiMatchRecordAction(callType, CaseChatOptions);
                    cases.NewRecordFieldIds = CaseChatOptions.NewrecordFieldIds;
                    cases.SearchCondition = CaseChatOptions.SearchCondition;
                    cases.CreateLogForNewRecord = CaseChatOptions.CanCreateLogForNewRecordCreate;
                    cases.MaxRecordOpenCount = CaseChatOptions.MaxNosRecordOpen;
                    cases.SearchpageMode = CaseChatOptions.SearchPageMode;
                    cases.PhoneNumberSearchFormat = CaseChatOptions.PhoneNumberSearchFormat;
                    if (cases.NoRecordFound.Equals("createnew"))
                    {
                        cases.CreateRecordFieldData = this.sfdcObject.GetChatRecordData(this.CaseRecordConfig, message, callType);
                    }
                    if (callType == SFDCCallType.InboundChat)
                    {
                        if (CaseChatOptions.InboundCanCreateLog)
                        {
                            cases.CreateActvityLog = true;
                            cases.ActivityLogData = this.sfdcObject.GetChatActivityLog(this.CaseChatLogOptions, popupEvent, callType);
                        }
                    }
                    else if (callType == SFDCCallType.ConsultChatReceived)
                    {
                        if (CaseChatOptions.ConsultCanCreateLog)
                        {
                            cases.CreateActvityLog = true;
                            cases.ActivityLogData = this.sfdcObject.GetChatActivityLog(this.CaseChatLogOptions, popupEvent, callType);
                        }
                    }
                    return cases;
                }
            }
            catch (Exception generalException)
            {
                this.logger.Error("GetCaseChatPopupData : Error occurred while reading Case Data : " + generalException.ToString());
            }
            return null;
        }

        /// <summary>
        /// Gets chat Update Data
        /// </summary>
        /// <param name="message"></param>
        /// <param name="callType"></param>
        /// <param name="duration"></param>
        /// <param name="chatContent"></param>
        /// <returns></returns>
        public ICase GetCaseChatUpdateData(IMessage message, SFDCCallType callType, string duration, string chatContent)
        {
            try
            {
                this.logger.Info("GetCaseChatUpdateData :  Reading Case Update Data.....");
                this.logger.Info("GetCaseChatUpdateData :  Event Name : " + message.Name);
                dynamic popupEvent = Convert.ChangeType(message, message.GetType());
                if (popupEvent != null)
                {
                    ICase cases = new CaseData();

                    #region Collect Lead Data

                    cases.ObjectName = CaseChatOptions.ObjectName;
                    if (callType == SFDCCallType.InboundChat)
                    {
                        if (CaseChatOptions.InboundCanUpdateLog)
                        {
                            cases.UpdateActivityLog = true;
                            cases.UpdateActivityLogData = this.sfdcObject.GetChatUpdateActivityLog(this.CaseChatLogOptions, popupEvent, callType, duration, chatContent);
                        }
                    }
                    else
                        if (callType == SFDCCallType.ConsultChatReceived)
                        {
                            if (CaseChatOptions.ConsultCanUpdateLog)
                            {
                                cases.UpdateActivityLog = true;
                                cases.UpdateActivityLogData = this.sfdcObject.GetChatUpdateActivityLog(this.CaseChatLogOptions, popupEvent, callType, duration, chatContent);
                            }
                        }
                        else if (callType == SFDCCallType.ConsultChatSuccess || callType == SFDCCallType.ConsultChatFailure)
                        {
                            if (CaseChatOptions.ConsultCanUpdateLog)
                            {
                                cases.UpdateActivityLog = true;
                                cases.UpdateActivityLogData = this.sfdcObject.GetChatUpdateActivityLog(this.CaseChatLogOptions, popupEvent, callType, duration, chatContent);
                            }
                        }
                    //update case record fields
                    cases.UpdateRecordFields = CaseChatOptions.CanUpdateRecordData;
                    if (GetNoRecordFoundAction(callType, CaseChatOptions).Equals("createnew") && this.CaseRecordConfig != null)
                    {
                        if (CaseChatOptions.CanUpdateRecordData)
                        {
                            cases.UpdateRecordFieldsData = this.sfdcObject.GetChatUpdateActivityLog(this.CaseRecordConfig, popupEvent, callType, duration, chatContent);
                        }
                    }

                    #endregion Collect Lead Data

                    return cases;
                }
            }
            catch (Exception generalException)
            {
                this.logger.Error("GetCaseChatUpdateData : Error occurred while reading Case Data : " + generalException.ToString());
            }
            return null;
        }

        /// <summary>
        /// Gets Case Popup Data
        /// </summary>
        /// <param name="message"></param>
        /// <param name="callType"></param>
        /// <returns></returns>
        public ICase GetCaseVoicePopupData(IMessage message, SFDCCallType callType)
        {
            try
            {
                this.logger.Info("GetCaseVoicePopupData :  Reading Case Popup Data.....");
                this.logger.Info("GetCaseVoicePopupData :  Event Name : " + message.Name);
                this.logger.Info("GetCaseVoicePopupData :  CallType Name : " + callType.ToString());
                dynamic popupEvent = Convert.ChangeType(message, message.GetType());
                if (popupEvent != null)
                {
                    ICase cases = new CaseData();

                    #region Collect case Data

                    cases.SearchData = GetCaseVoiceSearchValue(popupEvent.UserData, message, callType);
                    cases.SearchFields = CaseVoiceOptions.SearchFields;
                    cases.ObjectName = CaseVoiceOptions.ObjectName;
                    cases.NoRecordFound = GetNoRecordFoundAction(callType, CaseVoiceOptions);
                    cases.MultipleMatchRecord = GetMultiMatchRecordAction(callType, CaseVoiceOptions);
                    cases.NewRecordFieldIds = CaseVoiceOptions.NewrecordFieldIds;
                    cases.SearchCondition = CaseVoiceOptions.SearchCondition;
                    cases.CreateLogForNewRecord = CaseVoiceOptions.CanCreateLogForNewRecordCreate;
                    cases.MaxRecordOpenCount = CaseVoiceOptions.MaxNosRecordOpen;
                    cases.SearchpageMode = CaseVoiceOptions.SearchPageMode;
                    cases.PhoneNumberSearchFormat = CaseVoiceOptions.PhoneNumberSearchFormat;
                    cases.CanCreateNoRecordActivityLog = GetCanCreateProfileActivity(callType, CaseVoiceOptions, true);
                    cases.CanPopupNoRecordActivityLog = GetCanPopupProfileActivity(callType, CaseVoiceOptions, true);
                    cases.CanCreateMultiMatchActivityLog = GetCanCreateProfileActivity(callType, CaseVoiceOptions);
                    cases.CanPopupMultiMatchActivityLog = GetCanPopupProfileActivity(callType, CaseVoiceOptions);
                    cases.CanCreateProfileActivityforInbNoRecord = cases.CanCreateProfileActivityforInbNoRecord;

                    if (cases.NoRecordFound.Equals("createnew"))
                    {
                        cases.CreateRecordFieldData = this.sfdcObject.GetVoiceRecordData(this.CaseRecordConfig, message, callType);
                    }
                    if (callType == SFDCCallType.Inbound)
                    {
                        if (CaseVoiceOptions.InboundCanCreateLog)
                        {
                            cases.CreateActvityLog = true;
                            cases.ActivityLogData = this.sfdcObject.GetVoiceActivityLog(this.CaseLogConfig, popupEvent, callType);
                        }
                    }
                    else if (callType == SFDCCallType.OutboundSuccess)
                    {
                        if (CaseVoiceOptions.OutboundCanCreateLog)
                        {
                            cases.CreateActvityLog = true;
                            cases.ActivityLogData = this.sfdcObject.GetVoiceActivityLog(this.CaseLogConfig, popupEvent, callType);
                        }
                    }
                    else if (callType == SFDCCallType.OutboundFailure)
                    {
                        if (CaseVoiceOptions.OutboundFailureCanCreateLog)
                        {
                            cases.CreateActvityLog = true;
                            cases.ActivityLogData = this.sfdcObject.GetVoiceActivityLog(this.CaseLogConfig, popupEvent, callType);
                        }
                    }
                    else if (callType == SFDCCallType.ConsultReceived)
                    {
                        if (CaseVoiceOptions.ConsultCanCreateLog)
                        {
                            cases.CreateActvityLog = true;
                            cases.ActivityLogData = this.sfdcObject.GetVoiceActivityLog(this.CaseLogConfig, popupEvent, callType);
                        }
                    }
                    else if (callType == SFDCCallType.ConsultSuccess)
                    {
                        cases.NoRecordFound = "none";
                        if (CaseVoiceOptions.ConsultCanCreateLog)
                        {
                            cases.CreateActvityLog = true;
                            cases.ActivityLogData = this.sfdcObject.GetVoiceActivityLog(this.CaseLogConfig, popupEvent, callType);
                        }
                    }
                    else if (callType == SFDCCallType.ConsultFailure)
                    {
                        cases.NoRecordFound = "none";
                        if (CaseVoiceOptions.ConsultFailureCanCreateLog)
                        {
                            cases.CreateActvityLog = true;
                            cases.ActivityLogData = this.sfdcObject.GetVoiceActivityLog(this.CaseLogConfig, popupEvent, callType);
                        }
                    }

                    return cases;

                    #endregion Collect case Data
                }
            }
            catch (Exception generalException)
            {
                this.logger.Error("GetCaseVoicePopupData : Error occurred while reading Case Data : " + generalException.ToString());
            }
            return null;
        }

        /// <summary>
        /// Gets Case Update Data
        /// </summary>
        /// <param name="message"></param>
        /// <param name="callType"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        public ICase GetCaseVoiceUpdateData(IMessage message, SFDCCallType callType, string duration)
        {
            try
            {
                this.logger.Info("GetCaseVoiceUpdateData :  Reading Case Update Data.....");
                this.logger.Info("GetCaseVoiceUpdateData :  Event Name : " + message.Name);
                dynamic popupEvent = Convert.ChangeType(message, message.GetType());

                if (popupEvent != null)
                {
                    ICase caseData = new CaseData();

                    #region Collect caseData Data

                    caseData.ObjectName = CaseVoiceOptions.ObjectName;

                    if (callType == SFDCCallType.Inbound)
                    {
                        if (CaseVoiceOptions.InboundCanUpdateLog)
                        {
                            caseData.UpdateActivityLog = true;
                            caseData.UpdateActivityLogData = this.sfdcObject.GetVoiceUpdateActivityLog(this.CaseLogConfig, popupEvent, callType, duration);
                        }
                    }
                    else if (callType == SFDCCallType.OutboundSuccess || callType == SFDCCallType.OutboundFailure)
                    {
                        if (CaseVoiceOptions.OutboundCanUpdateLog)
                        {
                            caseData.UpdateActivityLog = true;
                            caseData.UpdateActivityLogData = this.sfdcObject.GetVoiceUpdateActivityLog(this.CaseLogConfig, popupEvent, callType, duration);
                        }
                    }
                    else if (callType == SFDCCallType.ConsultSuccess || callType == SFDCCallType.ConsultReceived || callType == SFDCCallType.ConsultFailure)
                    {
                        if (CaseVoiceOptions.ConsultCanUpdateLog)
                        {
                            caseData.UpdateActivityLog = true;
                            caseData.UpdateActivityLogData = this.sfdcObject.GetVoiceUpdateActivityLog(this.CaseLogConfig, popupEvent, callType, duration);
                        }
                    }

                    if (CaseVoiceOptions.CanUpdateRecordData)
                    {
                        caseData.UpdateRecordFields = true;
                        caseData.UpdateRecordFieldsData = this.sfdcObject.GetVoiceUpdateRecordData(this.CaseRecordConfig, popupEvent, callType, duration);
                    }

                    #endregion Collect caseData Data

                    return caseData;
                }
            }
            catch (Exception generalException)
            {
                this.logger.Error("GetCaseVoiceUpdateData : Error occurred while reading caseData Data : " + generalException.ToString());
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

        /// <summary>
        /// Gets Chat Search Value
        /// </summary>
        /// <param name="userData"></param>
        /// <param name="message"></param>
        /// <param name="callType"></param>
        /// <returns></returns>
        private string GetCaseChatSearchValue(KeyValueCollection userData, IMessage message, SFDCCallType callType)
        {
            try
            {
                this.logger.Info("GetCaseChatSearchValue :  Reading Case Popup Data.....");
                this.logger.Info("GetCaseChatSearchValue :  UserData Name : " + Convert.ToString(userData));
                this.logger.Info("GetCaseChatSearchValue :  Event Name : " + message.Name);
                this.logger.Info("GetCaseChatSearchValue :  CallType Name : " + callType.ToString());
                string[] userDataSearchKeys = null;
                string[] attributeSearchKeys = null;
                string searchValue = string.Empty;
                if (callType == SFDCCallType.InboundChat)
                {
                    userDataSearchKeys = (this.CaseChatOptions.InboundSearchUserDataKeys != null) ? this.CaseChatOptions.InboundSearchUserDataKeys.Split(',') : null;
                    attributeSearchKeys = (this.CaseChatOptions.InboundSearchAttributeKeys != null) ? this.CaseChatOptions.InboundSearchAttributeKeys.Split(',') : null;
                }
                else if (callType == SFDCCallType.ConsultChatReceived)
                {
                    userDataSearchKeys = (this.CaseChatOptions.ConsultSearchUserDataKeys != null) ? this.CaseChatOptions.ConsultSearchUserDataKeys.Split(',') : null;
                    attributeSearchKeys = (this.CaseChatOptions.ConsultSearchAttributeKeys != null) ? this.CaseChatOptions.ConsultSearchAttributeKeys.Split(',') : null;
                }

                if (CaseChatOptions.SearchPriority == "user-data")
                {
                    searchValue = this.sfdcObject.GetUserDataSearchValues(userData, userDataSearchKeys);
                }
                else if (CaseChatOptions.SearchPriority == "attribute")
                {
                    searchValue = this.sfdcObject.GetAttributeSearchValues(message, attributeSearchKeys);
                }
                else if (CaseChatOptions.SearchPriority == "both")
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
                this.logger.Error("GetCaseChatSearchValue : Error occurred while reading Case Data : " + generalException.ToString());
            }
            return null;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="userData"></param>
        /// <param name="message"></param>
        /// <param name="callType"></param>
        /// <returns></returns>
        private string GetCaseVoiceSearchValue(KeyValueCollection userData, IMessage message, SFDCCallType callType)
        {
            try
            {
                this.logger.Info("GetCaseVoiceSearchValue :  Reading Case Popup Data.....");
                this.logger.Info("GetCaseVoiceSearchValue :  UserData Name : " + Convert.ToString(userData));
                this.logger.Info("GetCaseVoiceSearchValue :  Event Name : " + message.Name);
                this.logger.Info("GetCaseVoiceSearchValue :  CallType Name : " + callType.ToString());
                string[] userDataSearchKeys = null;
                string[] attributeSearchKeys = null;
                string searchValue = string.Empty;
                if (callType == SFDCCallType.Inbound)
                {
                    userDataSearchKeys = (this.CaseVoiceOptions.InboundSearchUserDataKeys != null) ? this.CaseVoiceOptions.InboundSearchUserDataKeys.Split(',') : null;
                    attributeSearchKeys = (this.CaseVoiceOptions.InboundSearchAttributeKeys != null) ? this.CaseVoiceOptions.InboundSearchAttributeKeys.Split(',') : null;
                }
                else if (callType == SFDCCallType.OutboundSuccess || callType == SFDCCallType.OutboundFailure)
                {
                    userDataSearchKeys = (this.CaseVoiceOptions.OutboundSearchUserDataKeys != null) ? this.CaseVoiceOptions.OutboundSearchUserDataKeys.Split(',') : null;
                    attributeSearchKeys = (this.CaseVoiceOptions.OutboundSearchAttributeKeys != null) ? this.CaseVoiceOptions.OutboundSearchAttributeKeys.Split(',') : null;
                }
                else if (callType == SFDCCallType.ConsultSuccess || callType == SFDCCallType.ConsultFailure)
                {
                    return this.sfdcObject.GetAttributeSearchValues(message, new string[] { "otherdn" });
                }
                else if (callType == SFDCCallType.ConsultReceived)
                {
                    userDataSearchKeys = (this.CaseVoiceOptions.ConsultSearchUserDataKeys != null) ? this.CaseVoiceOptions.ConsultSearchUserDataKeys.Split(',') : null;
                    attributeSearchKeys = (this.CaseVoiceOptions.ConsultSearchAttributeKeys != null) ? this.CaseVoiceOptions.ConsultSearchAttributeKeys.Split(',') : null;
                }

                #region OLD

                //if (CaseVoiceOptions.SearchPriority == "user-data")
                //{
                //    if (userDataSearchKeys != null)
                //        searchValue = this.sfdcObject.GetUserDataSearchValues(userData, userDataSearchKeys);
                //    else if (attributeSearchKeys != null)
                //        searchValue = this.sfdcObject.GetAttributeSearchValues(message, attributeSearchKeys);
                //}
                //else if (CaseVoiceOptions.SearchPriority == "attribute")
                //{
                //    searchValue = this.sfdcObject.GetAttributeSearchValues(message, attributeSearchKeys);
                //}
                //else if (CaseVoiceOptions.SearchPriority == "both")
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

                if (this.CaseVoiceOptions.SearchPriority == "user-data")
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
                else if (this.CaseVoiceOptions.SearchPriority == "attribute")
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
                else if (this.CaseVoiceOptions.SearchPriority == "both")
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
                this.logger.Error("GetCaseVoiceSearchValue : Error occurred while reading Case Search Value : " + generalException.ToString());
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

        #endregion Methods
    }
}