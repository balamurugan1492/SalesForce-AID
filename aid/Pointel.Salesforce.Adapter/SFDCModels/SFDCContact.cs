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
    /// Comment: Provides Search and Activity Data for Contact Object
    /// Last Modified: 25-08-2015
    /// Created by: Pointel Inc
    /// </summary>
    internal class SFDCContact
    {
        #region Field Declaration

        private static SFDCContact sfdcContact = null;
        private Log logger = null;
        private VoiceOptions contactVoiceOptions = null;
        private ChatOptions contactChatOptions = null;
        private KeyValueCollection ContactLogConfig = null;
        private KeyValueCollection ContactChatLogConfig = null;
        private KeyValueCollection ContactRecordConfig = null;
        private SFDCUtility sfdcObject = null;

        #endregion Field Declaration

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        private SFDCContact()
        {
            this.logger = Log.GenInstance();
            this.sfdcObject = SFDCUtility.GetInstance();
            this.contactVoiceOptions = Settings.ContactVoiceOptions;
            this.contactChatOptions = Settings.ContactChatOptions;
            this.ContactLogConfig = (Settings.VoiceActivityLogCollection.ContainsKey("contact")) ? Settings.VoiceActivityLogCollection["contact"] : null;
            ContactChatLogConfig = (Settings.ChatActivityLogCollection.ContainsKey("contact")) ? Settings.ChatActivityLogCollection["contact"] : null;
            ContactRecordConfig = Settings.ContactNewRecordConfigs;
        }

        #endregion Constructor

        #region GetInstance Method

        /// <summary>
        /// Gets the Instance of the Class
        /// </summary>
        /// <returns></returns>
        public static SFDCContact GetInstance()
        {
            if (sfdcContact == null)
            {
                sfdcContact = new SFDCContact();
            }
            return sfdcContact;
        }

        #endregion GetInstance Method

        #region GetContactVoicePopupData

        /// <summary>
        /// Gets Contact Popup Data
        /// </summary>
        /// <param name="message"></param>
        /// <param name="callType"></param>
        /// <returns></returns>
        public IContact GetContactVoicePopupData(IMessage message, SFDCCallType callType)
        {
            try
            {
                this.logger.Info("GetContactVoicePopupData :  Reading Contact Popup Data.....");
                this.logger.Info("GetContactVoicePopupData :  Event Name : " + message.Name);
                this.logger.Info("GetContactVoicePopupData :  CallType Name : " + callType.ToString());
                dynamic popupEvent = Convert.ChangeType(message, message.GetType());
                if (popupEvent != null)
                {
                    IContact contact = new ContactData();

                    #region Collect contact Data

                    contact.SearchData = GetContactVoiceSearchValue(popupEvent.UserData, message, callType);
                    contact.ObjectName = contactVoiceOptions.ObjectName;
                    contact.NoRecordFound = GetNoRecordFoundAction(callType, contactVoiceOptions);
                    contact.MultipleMatchRecord = GetMultiMatchRecordAction(callType, contactVoiceOptions);
                    contact.NewRecordFieldIds = contactVoiceOptions.NewrecordFieldIds;
                    contact.SearchCondition = contactVoiceOptions.SearchCondition;
                    contact.CreateLogForNewRecord = contactVoiceOptions.CanCreateLogForNewRecordCreate;
                    contact.MaxRecordOpenCount = contactVoiceOptions.MaxNosRecordOpen;
                    contact.SearchpageMode = contactVoiceOptions.SearchPageMode;
                    contact.PhoneNumberSearchFormat = contactVoiceOptions.PhoneNumberSearchFormat;
                    contact.CanCreateNoRecordActivityLog = GetCanCreateProfileActivity(callType, contactVoiceOptions, true);
                    contact.CanPopupNoRecordActivityLog = GetCanPopupProfileActivity(callType, contactVoiceOptions, true);
                    contact.CanCreateMultiMatchActivityLog = GetCanCreateProfileActivity(callType, contactVoiceOptions);
                    contact.CanPopupMultiMatchActivityLog = GetCanPopupProfileActivity(callType, contactVoiceOptions);
                    contact.CanCreateProfileActivityforInbNoRecord = contactVoiceOptions.CanCreateProfileActivityforInbNoRecord;
                    contact.CanCreateProfileActivityforOutNoRecord = contactVoiceOptions.CanCreateProfileActivityforOutNoRecord;
                    contact.CanCreateProfileActivityforConNoRecord = contactVoiceOptions.CanCreateProfileActivityforConNoRecord;
                    if (contact.NoRecordFound.Equals("createnew"))
                    {
                        contact.CreateRecordFieldData = this.sfdcObject.GetVoiceRecordData(this.ContactRecordConfig, message, callType);
                    }
                    if (callType == SFDCCallType.Inbound)
                    {
                        if (contactVoiceOptions.InboundCanCreateLog)
                        {
                            contact.CreateActvityLog = true;
                            contact.ActivityLogData = this.sfdcObject.GetVoiceActivityLog(this.ContactLogConfig, popupEvent, callType);
                        }
                    }
                    else if (callType == SFDCCallType.OutboundSuccess)
                    {
                        if (contactVoiceOptions.OutboundCanCreateLog)
                        {
                            contact.CreateActvityLog = true;
                            contact.ActivityLogData = this.sfdcObject.GetVoiceActivityLog(this.ContactLogConfig, popupEvent, callType);
                        }
                    }
                    else if (callType == SFDCCallType.OutboundFailure)
                    {
                        if (contactVoiceOptions.OutboundFailureCanCreateLog)
                        {
                            contact.CreateActvityLog = true;
                            contact.ActivityLogData = this.sfdcObject.GetVoiceActivityLog(this.ContactLogConfig, popupEvent, callType);
                        }
                    }
                    else if (callType == SFDCCallType.ConsultReceived)
                    {
                        if (contactVoiceOptions.ConsultCanCreateLog)
                        {
                            contact.CreateActvityLog = true;
                            contact.ActivityLogData = this.sfdcObject.GetVoiceActivityLog(this.ContactLogConfig, popupEvent, callType);
                        }
                    }
                    else if (callType == SFDCCallType.ConsultSuccess)
                    {
                        contact.NoRecordFound = "none";
                        if (contactVoiceOptions.ConsultCanCreateLog)
                        {
                            contact.CreateActvityLog = true;
                            contact.ActivityLogData = this.sfdcObject.GetVoiceActivityLog(this.ContactLogConfig, popupEvent, callType);
                        }
                    }
                    else if (callType == SFDCCallType.ConsultFailure)
                    {
                        contact.NoRecordFound = "none";
                        if (contactVoiceOptions.ConsultFailureCanCreateLog)
                        {
                            contact.CreateActvityLog = true;
                            contact.ActivityLogData = this.sfdcObject.GetVoiceActivityLog(this.ContactLogConfig, popupEvent, callType);
                        }
                    }
                    return contact;
                }

                    #endregion Collect contact Data
            }
            catch (Exception generalException)
            {
                this.logger.Error("GetContactVoicePopupData : Error occurred while reading Contact Data on EventRinging Event : " + generalException.ToString());
            }
            return null;
        }

        #endregion GetContactVoicePopupData

        #region GetContactChatPopupData

        /// <summary>
        /// Gets Contact Popup Data
        /// </summary>
        /// <param name="message"></param>
        /// <param name="callType"></param>
        /// <returns></returns>
        public IContact GetContactChatPopupData(IMessage message, SFDCCallType callType)
        {
            try
            {
                this.logger.Info("GetContactChatPopupData :  Reading Contact Popup Data.....");
                this.logger.Info("GetContactChatPopupData :  Event Name : " + message.Name);
                this.logger.Info("GetContactChatPopupData :  CallType Name : " + callType.ToString());
                dynamic popupEvent = Convert.ChangeType(message, message.GetType());

                if (popupEvent != null)
                {
                    IContact contact = new ContactData();

                    #region Collect contact Data

                    contact.SearchData = GetContactChatSearchValue(popupEvent.Interaction.InteractionUserData, message, callType);
                    contact.ObjectName = contactChatOptions.ObjectName;
                    contact.NoRecordFound = GetNoRecordFoundAction(callType, contactChatOptions);
                    contact.MultipleMatchRecord = GetMultiMatchRecordAction(callType, contactChatOptions);
                    contact.NewRecordFieldIds = contactChatOptions.NewrecordFieldIds;
                    contact.SearchCondition = contactChatOptions.SearchCondition;
                    contact.CreateLogForNewRecord = contactChatOptions.CanCreateLogForNewRecordCreate;
                    contact.MaxRecordOpenCount = contactChatOptions.MaxNosRecordOpen;
                    contact.SearchpageMode = contactChatOptions.SearchPageMode;
                    contact.PhoneNumberSearchFormat = contactChatOptions.PhoneNumberSearchFormat;
                    if (contact.NoRecordFound.Equals("createnew"))
                    {
                        contact.CreateRecordFieldData = this.sfdcObject.GetChatRecordData(this.ContactRecordConfig, message, callType);
                    }
                    if (callType == SFDCCallType.InboundChat)
                    {
                        if (contactChatOptions.InboundCanCreateLog)
                        {
                            contact.CreateActvityLog = true;
                            contact.ActivityLogData = this.sfdcObject.GetChatActivityLog(this.ContactChatLogConfig, popupEvent, callType);
                        }
                    }
                    else if (callType == SFDCCallType.ConsultChatReceived)
                    {
                        if (contactChatOptions.ConsultCanCreateLog)
                        {
                            contact.CreateActvityLog = true;
                            contact.ActivityLogData = this.sfdcObject.GetChatActivityLog(this.ContactChatLogConfig, popupEvent, callType);
                        }
                    }
                    return contact;

                    #endregion Collect contact Data
                }
            }
            catch (Exception generalException)
            {
                this.logger.Error("GetContactChatPopupData : Error occurred while reading Contact Data on EventRinging Event : " + generalException.ToString());
            }
            return null;
        }

        #endregion GetContactChatPopupData

        #region GetContactVoiceSearchValue

        /// <summary>
        /// Gets Contact Search Value
        /// </summary>
        /// <param name="userData"></param>
        /// <param name="message"></param>
        /// <param name="callType"></param>
        /// <returns></returns>
        private string GetContactVoiceSearchValue(KeyValueCollection userData, IMessage message, SFDCCallType callType)
        {
            try
            {
                this.logger.Info("GetContactVoiceSearchValue :  Reading Contact Popup Data.....");
                this.logger.Info("GetContactVoiceSearchValue :  UserData Name : " + Convert.ToString(userData));
                this.logger.Info("GetContactVoiceSearchValue :  Event Name : " + message.Name);
                this.logger.Info("GetContactVoiceSearchValue :  CallType Name : " + callType.ToString());

                string[] userDataSearchKeys = null;
                string[] attributeSearchKeys = null;
                string searchValue = string.Empty;
                if (callType == SFDCCallType.Inbound)
                {
                    userDataSearchKeys = (this.contactVoiceOptions.InboundSearchUserDataKeys != null) ? this.contactVoiceOptions.InboundSearchUserDataKeys.Split(',') : null;
                    attributeSearchKeys = (this.contactVoiceOptions.InboundSearchAttributeKeys != null) ? this.contactVoiceOptions.InboundSearchAttributeKeys.Split(',') : null;
                }
                else if (callType == SFDCCallType.OutboundSuccess || callType == SFDCCallType.OutboundFailure)
                {
                    userDataSearchKeys = (this.contactVoiceOptions.OutboundSearchUserDataKeys != null) ? this.contactVoiceOptions.OutboundSearchUserDataKeys.Split(',') : null;
                    attributeSearchKeys = (this.contactVoiceOptions.OutboundSearchAttributeKeys != null) ? this.contactVoiceOptions.OutboundSearchAttributeKeys.Split(',') : null;
                }
                else if (callType == SFDCCallType.ConsultSuccess || callType == SFDCCallType.ConsultFailure)
                {
                    return this.sfdcObject.GetAttributeSearchValues(message, new string[] { "otherdn" });
                }
                else if (callType == SFDCCallType.ConsultReceived)
                {
                    userDataSearchKeys = (this.contactVoiceOptions.ConsultSearchUserDataKeys != null) ? this.contactVoiceOptions.ConsultSearchUserDataKeys.Split(',') : null;
                    attributeSearchKeys = (this.contactVoiceOptions.ConsultSearchAttributeKeys != null) ? this.contactVoiceOptions.ConsultSearchAttributeKeys.Split(',') : null;
                }

                #region OLD

                //if (contactVoiceOptions.SearchPriority == "user-data")
                //{
                //    if (userDataSearchKeys != null)
                //        searchValue = this.sfdcObject.GetUserDataSearchValues(userData, userDataSearchKeys);
                //    else if (attributeSearchKeys != null)
                //        searchValue = this.sfdcObject.GetAttributeSearchValues(message, attributeSearchKeys);
                //}
                //else if (contactVoiceOptions.SearchPriority == "attribute")
                //{
                //    searchValue = this.sfdcObject.GetAttributeSearchValues(message, attributeSearchKeys);
                //}
                //else if (contactVoiceOptions.SearchPriority == "both")
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

                if (this.contactVoiceOptions.SearchPriority == "user-data")
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
                else if (this.contactVoiceOptions.SearchPriority == "attribute")
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
                else if (contactVoiceOptions.SearchPriority == "both")
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
                this.logger.Error("GetContactVoiceSearchValue : Error occurred while reading Search Values : " + generalException.ToString());
            }
            return null;
        }

        #endregion GetContactVoiceSearchValue

        #region GetContactVoiceUpdateData

        /// <summary>
        /// Gets the Contact Update Data
        /// </summary>
        /// <param name="message"></param>
        /// <param name="callType"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        public IContact GetContactVoiceUpdateData(IMessage message, SFDCCallType callType, string duration)
        {
            try
            {
                this.logger.Info("GetContactVoiceUpdateData :  Reading contact Update Data.....");
                this.logger.Info("GetContactVoiceUpdateData :  Event Name : " + message.Name);
                dynamic popupEvent = Convert.ChangeType(message, message.GetType());
                if (popupEvent != null)
                {
                    IContact contact = new ContactData();

                    #region Collect contact Data

                    contact.ObjectName = contactVoiceOptions.ObjectName;

                    if (callType == SFDCCallType.Inbound)
                    {
                        if (contactVoiceOptions.InboundCanUpdateLog)
                        {
                            contact.UpdateActivityLog = true;
                            contact.UpdateActivityLogData = this.sfdcObject.GetVoiceUpdateActivityLog(this.ContactLogConfig, popupEvent, callType, duration);
                        }
                    }
                    else if (callType == SFDCCallType.OutboundSuccess || callType == SFDCCallType.OutboundFailure)
                    {
                        if (contactVoiceOptions.OutboundCanUpdateLog)
                        {
                            contact.UpdateActivityLog = true;
                            contact.UpdateActivityLogData = this.sfdcObject.GetVoiceUpdateActivityLog(this.ContactLogConfig, popupEvent, callType, duration);
                        }
                    }
                    else if (callType == SFDCCallType.ConsultSuccess || callType == SFDCCallType.ConsultReceived || callType == SFDCCallType.ConsultFailure)
                    {
                        if (contactVoiceOptions.ConsultCanUpdateLog)
                        {
                            contact.UpdateActivityLog = true;
                            contact.UpdateActivityLogData = this.sfdcObject.GetVoiceUpdateActivityLog(this.ContactLogConfig, popupEvent, callType, duration);
                        }
                    }

                    if (contactVoiceOptions.CanUpdateRecordData)
                    {
                        contact.UpdateRecordFields = true;
                        contact.UpdateRecordFieldsData = this.sfdcObject.GetVoiceUpdateRecordData(this.ContactRecordConfig, popupEvent, callType, duration);
                    }

                    #endregion Collect contact Data

                    return contact;
                }
            }
            catch (Exception generalException)
            {
                this.logger.Error("GetContactVoiceUpdateData : Error occurred while reading contact Data : " + generalException.ToString());
            }
            return null;
        }

        #endregion GetContactVoiceUpdateData

        #region GetContactChatSearchValue

        /// <summary>
        /// Gets Chat Search Value
        /// </summary>
        /// <param name="userData"></param>
        /// <param name="message"></param>
        /// <param name="callType"></param>
        /// <returns></returns>
        private string GetContactChatSearchValue(KeyValueCollection userData, IMessage message, SFDCCallType callType)
        {
            try
            {
                this.logger.Info("GetContactChatSearchValue :  Reading Contact Popup Data.....");
                this.logger.Info("GetContactChatSearchValue :  UserData Name : " + Convert.ToString(userData));
                this.logger.Info("GetContactChatSearchValue :  Event Name : " + message.Name);
                this.logger.Info("GetContactChatSearchValue :  CallType Name : " + callType.ToString());
                string[] userDataSearchKeys = null;
                string[] attributeSearchKeys = null;
                string searchValue = string.Empty;

                if (callType == SFDCCallType.InboundChat)
                {
                    userDataSearchKeys = (this.contactChatOptions.InboundSearchUserDataKeys != null) ? this.contactChatOptions.InboundSearchUserDataKeys.Split(',') : null;
                    attributeSearchKeys = (this.contactChatOptions.InboundSearchAttributeKeys != null) ? this.contactChatOptions.InboundSearchAttributeKeys.Split(',') : null;
                }
                else if (callType == SFDCCallType.ConsultChatReceived)
                {
                    userDataSearchKeys = (this.contactChatOptions.ConsultSearchUserDataKeys != null) ? this.contactChatOptions.ConsultSearchUserDataKeys.Split(',') : null;
                    attributeSearchKeys = (this.contactChatOptions.ConsultSearchAttributeKeys != null) ? this.contactChatOptions.ConsultSearchAttributeKeys.Split(',') : null;
                }

                if (contactChatOptions.SearchPriority == "user-data")
                {
                    searchValue = this.sfdcObject.GetUserDataSearchValues(userData, userDataSearchKeys);
                }
                else if (contactChatOptions.SearchPriority == "attribute")
                {
                    searchValue = this.sfdcObject.GetAttributeSearchValues(message, attributeSearchKeys);
                }
                else if (contactChatOptions.SearchPriority == "both")
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
                this.logger.Error("GetContactChatSearchValue : Error occurred while reading Contact Data : " + generalException.ToString());
            }
            return null;
        }

        #endregion GetContactChatSearchValue

        #region GetContactChatUpdateData

        /// <summary>
        /// Gets Chat Update Data
        /// </summary>
        /// <param name="message"></param>
        /// <param name="callType"></param>
        /// <param name="duration"></param>
        /// <param name="chatContent"></param>
        /// <returns></returns>
        public IContact GetContactChatUpdateData(IMessage message, SFDCCallType callType, string duration, string chatContent)
        {
            try
            {
                this.logger.Info("GetContactChatUpdateData :  Reading Contact Update Data.....");
                this.logger.Info("GetContactChatUpdateData :  Event Name : " + message.Name);
                dynamic popupEvent = Convert.ChangeType(message, message.GetType());
                if (popupEvent != null)
                {
                    IContact contact = new ContactData();

                    #region Collect Lead Data

                    contact.ObjectName = contactChatOptions.ObjectName;
                    if (callType == SFDCCallType.InboundChat)
                    {
                        if (contactChatOptions.InboundCanUpdateLog)
                        {
                            contact.UpdateActivityLog = true;
                            contact.UpdateActivityLogData = this.sfdcObject.GetChatUpdateActivityLog(this.ContactChatLogConfig, popupEvent, callType, duration, chatContent);
                        }
                    }
                    else if (callType == SFDCCallType.ConsultChatReceived)
                    {
                        if (contactChatOptions.ConsultCanUpdateLog)
                        {
                            contact.UpdateActivityLog = true;
                            contact.UpdateActivityLogData = this.sfdcObject.GetChatUpdateActivityLog(this.ContactChatLogConfig, popupEvent, callType, duration, chatContent);
                        }
                    }
                    else if (callType == SFDCCallType.ConsultChatSuccess || callType == SFDCCallType.ConsultChatFailure)
                    {
                        if (contactChatOptions.ConsultCanUpdateLog)
                        {
                            contact.UpdateActivityLog = true;
                            contact.UpdateActivityLogData = this.sfdcObject.GetChatUpdateActivityLog(this.ContactChatLogConfig, popupEvent, callType, duration, chatContent);
                        }
                    }
                    //update case record fields
                    contact.UpdateRecordFields = contactChatOptions.CanUpdateRecordData;
                    if (GetNoRecordFoundAction(callType, contactChatOptions).Equals("createnew") && this.ContactRecordConfig != null)
                    {
                        if (contactChatOptions.CanUpdateRecordData)
                        {
                            contact.UpdateRecordFieldsData = this.sfdcObject.GetChatUpdateActivityLog(this.ContactRecordConfig, popupEvent, callType, duration, chatContent);
                        }
                    }

                    #endregion Collect Lead Data

                    return contact;
                }
            }
            catch (Exception generalException)
            {
                this.logger.Error("GetContactChatUpdateData : Error occurred while reading Lead Data : " + generalException.ToString());
            }
            return null;
        }

        #endregion GetContactChatUpdateData

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