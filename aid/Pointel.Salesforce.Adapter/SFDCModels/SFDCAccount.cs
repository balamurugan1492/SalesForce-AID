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
    /// Comment: Provides Search and Activity Data for Account Object
    /// Last Modified: 25-08-2015
    /// Created by: Pointel Inc
    /// </summary>
    internal class SFDCAccount
    {
        #region Fields

        private static SFDCAccount accountData = null;

        private KeyValueCollection AccountChatLogConfig = null;
        private ChatOptions accountChatOptions = null;
        private KeyValueCollection AccountLogConfig = null;
        private KeyValueCollection AccountRecordConfig = null;
        private VoiceOptions accountVoiceOptions = null;
        private Log logger = null;
        private SFDCUtility sfdcObject = null;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        private SFDCAccount()
        {
            this.logger = Log.GenInstance();
            this.accountVoiceOptions = Settings.AccountVoiceOptions;
            this.accountChatOptions = Settings.AccountChatOptions;
            AccountLogConfig = (Settings.VoiceActivityLogCollection.ContainsKey("account")) ? Settings.VoiceActivityLogCollection["account"] : null;
            AccountChatLogConfig = (Settings.ChatActivityLogCollection.ContainsKey("account")) ? Settings.ChatActivityLogCollection["account"] : null;
            AccountRecordConfig = Settings.AccountNewRecordConfigs;
            this.sfdcObject = SFDCUtility.GetInstance();
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Gets the Intstance of the class
        /// </summary>
        public static SFDCAccount GetInstance()
        {
            if (accountData == null)
            {
                accountData = new SFDCAccount();
            }
            return accountData;
        }

        /// <summary>
        /// Get Account Popup Data for Chat
        /// </summary>
        /// <param name="message"></param>
        /// <param name="callType"></param>
        /// <returns></returns>
        public IAccount GetAccountChatPopupData(IMessage message, SFDCCallType callType)
        {
            try
            {
                this.logger.Info("GetAccountChatPopupData :  Reading Account Popup Data.....");
                this.logger.Info("GetAccountChatPopupData :  Event Name : " + message.Name);
                this.logger.Info("GetAccountChatPopupData :  CallType Name : " + callType.ToString());
                dynamic popupEvent = Convert.ChangeType(message, message.GetType());

                if (popupEvent != null)
                {
                    IAccount account = new AccountData();

                    #region Collect Lead Data

                    account.SearchData = GetAccountChatSearchValue(popupEvent.Interaction.InteractionUserData, message, callType);
                    account.ObjectName = accountChatOptions.ObjectName;
                    account.NoRecordFound = GetNoRecordFoundAction(callType, accountChatOptions);
                    account.MultipleMatchRecord = GetMultiMatchRecordAction(callType, accountChatOptions);
                    account.NewRecordFieldIds = accountChatOptions.NewrecordFieldIds;
                    account.SearchCondition = accountChatOptions.SearchCondition;
                    account.CreateLogForNewRecord = accountChatOptions.CanCreateLogForNewRecordCreate;
                    account.MaxRecordOpenCount = accountChatOptions.MaxNosRecordOpen;
                    account.SearchpageMode = accountChatOptions.SearchPageMode;
                    account.PhoneNumberSearchFormat = accountChatOptions.PhoneNumberSearchFormat;
                    if (account.NoRecordFound.Equals("createnew"))
                    {
                        account.CreateRecordFieldData = this.sfdcObject.GetChatRecordData(this.AccountRecordConfig, message, callType);
                    }
                    if (callType == SFDCCallType.InboundChat)
                    {
                        if (accountChatOptions.InboundCanCreateLog)
                        {
                            account.CreateActvityLog = true;
                            account.ActivityLogData = this.sfdcObject.GetChatActivityLog(this.AccountChatLogConfig, popupEvent, callType);
                        }
                    }
                    else if (callType == SFDCCallType.ConsultChatReceived)
                    {
                        if (accountChatOptions.ConsultCanCreateLog)
                        {
                            account.CreateActvityLog = true;
                            account.ActivityLogData = this.sfdcObject.GetChatActivityLog(this.AccountChatLogConfig, popupEvent, callType);
                        }
                    }

                    #endregion Collect Lead Data

                    return account;
                }
            }
            catch (Exception generalException)
            {
                this.logger.Error("GetAccountChatPopupData : Error occurred while reading Account Data : " + generalException.ToString());
            }
            return null;
        }

        /// <summary>
        /// Gets the Chat Update data.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="callType"></param>
        /// <param name="duration"></param>
        /// <param name="chatContent"></param>
        /// <returns></returns>
        public IAccount GetAccountChatUpdateData(IMessage message, SFDCCallType callType, string duration, string chatContent)
        {
            try
            {
                this.logger.Info("GetAccountChatUpdateData :  Reading Account Update Data.....");
                this.logger.Info("GetAccountChatUpdateData :  Event Name : " + message.Name);
                dynamic popupEvent = Convert.ChangeType(message, message.GetType());
                if (popupEvent != null)
                {
                    IAccount account = new AccountData();

                    #region Collect Lead Data

                    account.ObjectName = accountChatOptions.ObjectName;
                    if (callType == SFDCCallType.InboundChat)
                    {
                        if (accountChatOptions.InboundCanUpdateLog)
                        {
                            account.UpdateActivityLog = true;
                            account.UpdateActivityLogData = this.sfdcObject.GetChatUpdateActivityLog(this.AccountChatLogConfig, popupEvent, callType, duration, chatContent);
                        }
                    }
                    else if (callType == SFDCCallType.ConsultChatReceived)
                    {
                        if (accountChatOptions.ConsultCanUpdateLog)
                        {
                            account.UpdateActivityLog = true;
                            account.UpdateActivityLogData = this.sfdcObject.GetChatUpdateActivityLog(this.AccountChatLogConfig, popupEvent, callType, duration, chatContent);
                        }
                    }
                    else if (callType == SFDCCallType.ConsultChatSuccess || callType == SFDCCallType.ConsultChatFailure)
                    {
                        if (accountChatOptions.ConsultCanUpdateLog)
                        {
                            account.UpdateActivityLog = true;
                            account.UpdateActivityLogData = this.sfdcObject.GetChatUpdateActivityLog(this.AccountChatLogConfig, popupEvent, callType, duration, chatContent);
                        }
                    }
                    //update account record fields
                    account.UpdateRecordFields = accountChatOptions.CanUpdateRecordData;
                    if (GetNoRecordFoundAction(callType, accountChatOptions).Equals("createnew") && this.AccountRecordConfig != null)
                    {
                        if (accountChatOptions.CanUpdateRecordData)
                        {
                            account.UpdateRecordFieldsData = this.sfdcObject.GetChatUpdateActivityLog(this.AccountRecordConfig, popupEvent, callType, duration, chatContent);
                        }
                    }

                    #endregion Collect Lead Data

                    return account;
                }
            }
            catch (Exception generalException)
            {
                this.logger.Error("GetAccountChatUpdateData : Error occurred while reading Account Data : " + generalException.ToString());
            }
            return null;
        }

        /// <summary>
        /// Gets Account Update Data
        /// </summary>
        /// <param name="message"></param>
        /// <param name="callType"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        public IAccount GetAccountUpdateData(IMessage message, SFDCCallType callType, string duration)
        {
            try
            {
                this.logger.Info("GetAccountUpdateData :  Reading Account Update Data.....");
                this.logger.Info("GetAccountUpdateData :  Event Name : " + message.Name);
                dynamic popupEvent = Convert.ChangeType(message, message.GetType());

                if (popupEvent != null)
                {
                    IAccount account = new AccountData();

                    #region Collect account Data

                    account.ObjectName = accountVoiceOptions.ObjectName;

                    if (callType == SFDCCallType.Inbound)
                    {
                        if (accountVoiceOptions.InboundCanUpdateLog)
                        {
                            account.UpdateActivityLog = true;
                            account.UpdateActivityLogData = this.sfdcObject.GetVoiceUpdateActivityLog(this.AccountLogConfig, popupEvent, callType, duration);
                        }
                    }
                    else if (callType == SFDCCallType.OutboundSuccess || callType == SFDCCallType.OutboundFailure)
                    {
                        if (accountVoiceOptions.OutboundCanUpdateLog)
                        {
                            account.UpdateActivityLog = true;
                            account.UpdateActivityLogData = this.sfdcObject.GetVoiceUpdateActivityLog(this.AccountLogConfig, popupEvent, callType, duration);
                        }
                    }
                    else if (callType == SFDCCallType.ConsultSuccess || callType == SFDCCallType.ConsultReceived || callType == SFDCCallType.ConsultFailure)
                    {
                        if (accountVoiceOptions.ConsultCanUpdateLog)
                        {
                            account.UpdateActivityLog = true;
                            account.UpdateActivityLogData = this.sfdcObject.GetVoiceUpdateActivityLog(this.AccountLogConfig, popupEvent, callType, duration);
                        }
                    }

                    if (accountVoiceOptions.CanUpdateRecordData)
                    {
                        account.UpdateRecordFields = true;
                        account.UpdateRecordFieldsData = this.sfdcObject.GetVoiceUpdateRecordData(this.AccountRecordConfig, popupEvent, callType, duration);
                    }

                    #endregion Collect account Data

                    return account;
                }
            }
            catch (Exception generalException)
            {
                this.logger.Error("GetAccountUpdateData : Error occurred while reading account Data : " + generalException.ToString());
            }
            return null;
        }

        /// <summary>
        /// Get Account Popup Data
        /// </summary>
        /// <param name="message"></param>
        /// <param name="callType"></param>
        /// <returns></returns>
        public IAccount GetAccountVoicePopupData(IMessage message, SFDCCallType callType)
        {
            try
            {
                this.logger.Info("GetAccountVoicePopupData :  Reading Account Popup Data.....");
                this.logger.Info("GetAccountVoicePopupData :  Event Name : " + message.Name);
                this.logger.Info("GetAccountVoicePopupData :  CallType Name : " + callType.ToString());
                dynamic popupEvent = Convert.ChangeType(message, message.GetType());

                if (popupEvent != null)
                {
                    IAccount account = new AccountData();

                    #region Collect account Data

                    account.SearchData = GetAccountVoiceSearchValue(popupEvent.UserData, message, callType);
                    account.SearchFields = accountVoiceOptions.SearchFields;
                    account.ObjectName = accountVoiceOptions.ObjectName;
                    account.NoRecordFound = GetNoRecordFoundAction(callType, accountVoiceOptions);
                    account.MultipleMatchRecord = GetMultiMatchRecordAction(callType, accountVoiceOptions);
                    account.NewRecordFieldIds = accountVoiceOptions.NewrecordFieldIds;
                    account.SearchCondition = accountVoiceOptions.SearchCondition;
                    account.CreateLogForNewRecord = accountVoiceOptions.CanCreateLogForNewRecordCreate;
                    account.MaxRecordOpenCount = accountVoiceOptions.MaxNosRecordOpen;
                    account.SearchpageMode = accountVoiceOptions.SearchPageMode;
                    account.PhoneNumberSearchFormat = accountVoiceOptions.PhoneNumberSearchFormat;
                    account.CanCreateNoRecordActivityLog = GetCanCreateProfileActivity(callType, accountVoiceOptions, true);
                    account.CanPopupNoRecordActivityLog = GetCanPopupProfileActivity(callType, accountVoiceOptions, true);
                    account.CanCreateMultiMatchActivityLog = GetCanCreateProfileActivity(callType, accountVoiceOptions);
                    account.CanPopupMultiMatchActivityLog = GetCanPopupProfileActivity(callType, accountVoiceOptions);
                    account.CanCreateProfileActivityforInbNoRecord = accountVoiceOptions.CanCreateProfileActivityforInbNoRecord;
                    account.CanCreateProfileActivityforOutNoRecord = accountVoiceOptions.CanCreateProfileActivityforOutNoRecord;
                    account.CanCreateProfileActivityforConNoRecord = accountVoiceOptions.CanCreateProfileActivityforConNoRecord;

                    if (account.NoRecordFound.Equals("createnew"))
                    {
                        account.CreateRecordFieldData = this.sfdcObject.GetVoiceRecordData(this.AccountRecordConfig, message, callType);
                    }
                    if (callType == SFDCCallType.Inbound)
                    {
                        if (accountVoiceOptions.InboundCanCreateLog)
                        {
                            account.CreateActvityLog = true;
                            account.ActivityLogData = this.sfdcObject.GetVoiceActivityLog(this.AccountLogConfig, popupEvent, callType);
                        }
                    }
                    else if (callType == SFDCCallType.OutboundSuccess)
                    {
                        if (accountVoiceOptions.OutboundCanCreateLog)
                        {
                            account.CreateActvityLog = true;
                            account.ActivityLogData = this.sfdcObject.GetVoiceActivityLog(this.AccountLogConfig, popupEvent, callType);
                        }
                    }
                    else if (callType == SFDCCallType.OutboundFailure)
                    {
                        if (accountVoiceOptions.OutboundFailureCanCreateLog)
                        {
                            account.CreateActvityLog = true;
                            account.ActivityLogData = this.sfdcObject.GetVoiceActivityLog(this.AccountLogConfig, popupEvent, callType);
                        }
                    }
                    else if (callType == SFDCCallType.ConsultReceived)
                    {
                        if (accountVoiceOptions.ConsultCanCreateLog)
                        {
                            account.CreateActvityLog = true;
                            account.ActivityLogData = this.sfdcObject.GetVoiceActivityLog(this.AccountLogConfig, popupEvent, callType);
                        }
                    }
                    else if (callType == SFDCCallType.ConsultSuccess)
                    {
                        account.NoRecordFound = "none";
                        if (accountVoiceOptions.ConsultCanCreateLog)
                        {
                            account.CreateActvityLog = true;
                            account.ActivityLogData = this.sfdcObject.GetVoiceActivityLog(this.AccountLogConfig, popupEvent, callType);
                        }
                    }
                    else if (callType == SFDCCallType.ConsultFailure)
                    {
                        account.NoRecordFound = "none";
                        if (accountVoiceOptions.ConsultFailureCanCreateLog)
                        {
                            account.CreateActvityLog = true;
                            account.ActivityLogData = this.sfdcObject.GetVoiceActivityLog(this.AccountLogConfig, popupEvent, callType);
                        }
                    }

                    #endregion Collect account Data

                    return account;
                }
            }
            catch (Exception generalException)
            {
                this.logger.Error("GetAccountVoicePopupData : Error occurred while reading Account Data : " + generalException.ToString());
            }
            return null;
        }

        /// <summary>
        /// Gets Chat Search Value
        /// </summary>
        /// <param name="userData"></param>
        /// <param name="message"></param>
        /// <param name="callType"></param>
        /// <returns></returns>
        private string GetAccountChatSearchValue(KeyValueCollection userData, IMessage message, SFDCCallType callType)
        {
            try
            {
                this.logger.Info("GetAccountChatSearchValue :  Reading Account Popup Data.....");
                this.logger.Info("GetAccountChatSearchValue :  UserData Name : " + Convert.ToString(userData));
                this.logger.Info("GetAccountChatSearchValue :  Event Name : " + message.Name);
                this.logger.Info("GetAccountChatSearchValue :  CallType Name : " + callType.ToString());
                string[] userDataSearchKeys = null;
                string[] attributeSearchKeys = null;
                string searchValue = string.Empty;
                if (callType == SFDCCallType.InboundChat)
                {
                    userDataSearchKeys = (this.accountChatOptions.InboundSearchUserDataKeys != null) ? this.accountChatOptions.InboundSearchUserDataKeys.Split(',') : null;
                    attributeSearchKeys = (this.accountChatOptions.InboundSearchAttributeKeys != null) ? this.accountChatOptions.InboundSearchAttributeKeys.Split(',') : null;
                }
                else if (callType == SFDCCallType.ConsultChatReceived)
                {
                    userDataSearchKeys = (this.accountChatOptions.ConsultSearchUserDataKeys != null) ? this.accountChatOptions.ConsultSearchUserDataKeys.Split(',') : null;
                    attributeSearchKeys = (this.accountChatOptions.ConsultSearchAttributeKeys != null) ? this.accountChatOptions.ConsultSearchAttributeKeys.Split(',') : null;
                }

                if (accountChatOptions.SearchPriority == "user-data")
                {
                    searchValue = this.sfdcObject.GetUserDataSearchValues(userData, userDataSearchKeys);
                }
                else if (accountChatOptions.SearchPriority == "attribute")
                {
                    searchValue = this.sfdcObject.GetAttributeSearchValues(message, attributeSearchKeys);
                }
                else if (accountChatOptions.SearchPriority == "both")
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
                this.logger.Error("GetAccountChatSearchValue : Error occurred while reading Account Data : " + generalException.ToString());
            }
            return null;
        }

        /// <summary>
        /// Gets Account Search Value
        /// </summary>
        /// <param name="userData"></param>
        /// <param name="message"></param>
        /// <param name="callType"></param>
        /// <returns></returns>
        private string GetAccountVoiceSearchValue(KeyValueCollection userData, IMessage message, SFDCCallType callType)
        {
            try
            {
                this.logger.Info("GetAccountVoiceSearchValue :  Reading Account Popup Data.....");
                this.logger.Info("GetAccountVoiceSearchValue :  UserData Name : " + Convert.ToString(userData));
                this.logger.Info("GetAccountVoiceSearchValue :  Event Name : " + message.Name);
                this.logger.Info("GetAccountVoiceSearchValue :  CallType Name : " + callType.ToString());
                string[] userDataSearchKeys = null;
                string[] attributeSearchKeys = null;
                string searchValue = string.Empty;

                if (callType == SFDCCallType.Inbound)
                {
                    userDataSearchKeys = (this.accountVoiceOptions.InboundSearchUserDataKeys != null) ? this.accountVoiceOptions.InboundSearchUserDataKeys.Split(',') : null;
                    attributeSearchKeys = (this.accountVoiceOptions.InboundSearchAttributeKeys != null) ? this.accountVoiceOptions.InboundSearchAttributeKeys.Split(',') : null;
                }
                else if (callType == SFDCCallType.OutboundSuccess || callType == SFDCCallType.OutboundFailure)
                {
                    userDataSearchKeys = (this.accountVoiceOptions.OutboundSearchUserDataKeys != null) ? this.accountVoiceOptions.OutboundSearchUserDataKeys.Split(',') : null;
                    attributeSearchKeys = (this.accountVoiceOptions.OutboundSearchAttributeKeys != null) ? this.accountVoiceOptions.OutboundSearchAttributeKeys.Split(',') : null;
                }
                else if (callType == SFDCCallType.ConsultSuccess || callType == SFDCCallType.ConsultFailure)
                {
                    return this.sfdcObject.GetAttributeSearchValues(message, new string[] { "otherdn" });
                }
                else if (callType == SFDCCallType.ConsultReceived)
                {
                    userDataSearchKeys = (this.accountVoiceOptions.ConsultSearchUserDataKeys != null) ? this.accountVoiceOptions.ConsultSearchUserDataKeys.Split(',') : null;
                    attributeSearchKeys = (this.accountVoiceOptions.ConsultSearchAttributeKeys != null) ? this.accountVoiceOptions.ConsultSearchAttributeKeys.Split(',') : null;
                }

                #region OLD

                //if (this.accountVoiceOptions.SearchPriority == "user-data")
                //{
                //    if (userDataSearchKeys != null)
                //        searchValue = this.sfdcObject.GetUserDataSearchValues(userData, userDataSearchKeys);
                //    else if (attributeSearchKeys != null)
                //        searchValue = this.sfdcObject.GetAttributeSearchValues(message, attributeSearchKeys);
                //}
                //else if (this.accountVoiceOptions.SearchPriority == "attribute")
                //{
                //    searchValue = this.sfdcObject.GetAttributeSearchValues(message, attributeSearchKeys);
                //}
                //else if (this.accountVoiceOptions.SearchPriority == "both")
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

                if (this.accountVoiceOptions.SearchPriority == "user-data")
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
                else if (this.accountVoiceOptions.SearchPriority == "attribute")
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
                else if (this.accountVoiceOptions.SearchPriority == "both")
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
                this.logger.Error("GetAccountVoiceSearchValue : Error occurred while reading Account Search Value : " + generalException.ToString());
            }
            return null;
        }

        private bool GetCanCreateProfileActivity(SFDCCallType callType, VoiceOptions voiceOption, bool IsNoRecord = false)
        {
            bool canCreateNoRecordMultiMatchLog = false;
            switch (callType)
            {
                case SFDCCallType.Inbound:
                    canCreateNoRecordMultiMatchLog = IsNoRecord ? voiceOption.InbCanCreateNorecordActivity : voiceOption.InbCanCreateMultimatchActivity;
                    break;

                case SFDCCallType.OutboundFailure:
                case SFDCCallType.OutboundSuccess:
                    canCreateNoRecordMultiMatchLog = IsNoRecord ? voiceOption.OutCanCreateNorecordActivity : voiceOption.OutCanCreateMultimatchActivity;
                    break;

                case SFDCCallType.ConsultSuccess:
                case SFDCCallType.ConsultReceived:
                case SFDCCallType.ConsultFailure:
                    canCreateNoRecordMultiMatchLog = IsNoRecord ? voiceOption.ConCanCreateNorecordActivity : voiceOption.ConCanCreateMultimatchActivity;
                    break;
            }
            return canCreateNoRecordMultiMatchLog;
        }

        private bool GetCanPopupProfileActivity(SFDCCallType callType, VoiceOptions voiceOption, bool IsNoRecord = false)
        {
            bool canPopupNoRecordMultiMatchLog = false;
            switch (callType)
            {
                case SFDCCallType.Inbound:
                    canPopupNoRecordMultiMatchLog = IsNoRecord ? voiceOption.InbCanPopupNorecordActivity : voiceOption.InbCanPopupMultimatchActivity;
                    break;

                case SFDCCallType.OutboundFailure:
                case SFDCCallType.OutboundSuccess:
                    canPopupNoRecordMultiMatchLog = IsNoRecord ? voiceOption.OutCanPopupNorecordActivity : voiceOption.OutCanPopupMultimatchActivity;
                    break;

                case SFDCCallType.ConsultSuccess:
                case SFDCCallType.ConsultReceived:
                case SFDCCallType.ConsultFailure:
                    canPopupNoRecordMultiMatchLog = IsNoRecord ? voiceOption.ConCanPopupNorecordActivity : voiceOption.ConCanPopupMultimatchActivity;
                    break;
            }
            return canPopupNoRecordMultiMatchLog;
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