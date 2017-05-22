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
    /// Comment: Provides Search and Activity Data for Contact Object Last Modified: 25-08-2015
    /// Created by: Pointel Inc </summary>
    internal class SFDCContact
    {
        #region Field Declaration

        private static SFDCContact sfdcContact = null;
        private KeyValueCollection ContactChatLogConfig = null;
        private ChatOptions contactChatOptions = null;
        private KeyValueCollection ContactEmailLogConfig = null;
        private EmailOptions contactEmailOptions = null;
        private KeyValueCollection ContactLogConfig = null;
        private KeyValueCollection ContactEmailRecordConfig = null;
        private KeyValueCollection ContactVoiceRecordConfig = null;
        private KeyValueCollection ContactChatRecordConfig = null;
        private VoiceOptions contactVoiceOptions = null;
        private Log logger = null;
        private SFDCUtility sfdcUtility = null;
        private SFDCUtiltiyHelper sfdcUtilityHelper = null;

        #endregion Field Declaration

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        private SFDCContact()
        {
            this.logger = Log.GenInstance();
            this.sfdcUtility = SFDCUtility.GetInstance();
            this.contactVoiceOptions = Settings.ContactVoiceOptions;
            this.contactChatOptions = Settings.ContactChatOptions;
            this.ContactLogConfig = (Settings.VoiceActivityLogCollection.ContainsKey("contact")) ? Settings.VoiceActivityLogCollection["contact"] : null;
            ContactChatLogConfig = (Settings.ChatActivityLogCollection.ContainsKey("contact")) ? Settings.ChatActivityLogCollection["contact"] : null;
            this.ContactEmailRecordConfig = (Settings.EmailNewRecordCollection.ContainsKey("contact")) ? Settings.EmailNewRecordCollection["contact"] : null;
            this.ContactVoiceRecordConfig = (Settings.VoiceNewRecordCollection.ContainsKey("contact")) ? Settings.VoiceNewRecordCollection["contact"] : null;
            this.ContactChatRecordConfig = (Settings.ChatNewRecordCollection.ContainsKey("contact")) ? Settings.ChatNewRecordCollection["contact"] : null;
            this.ContactEmailLogConfig = (Settings.EmailActivityLogCollection.ContainsKey("contact")) ? Settings.EmailActivityLogCollection["contact"] : null;
            this.contactEmailOptions = Settings.ContactEmailOptions;
            this.sfdcUtilityHelper = SFDCUtiltiyHelper.GetInstance();
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
        public ContactData GetContactVoicePopupData(IMessage message, SFDCCallType callType)
        {
            try
            {
                this.logger.Info("GetContactVoicePopupData :  Reading Contact Popup Data.....");
                this.logger.Info("GetContactVoicePopupData :  Event Name : " + message.Name);
                this.logger.Info("GetContactVoicePopupData :  CallType Name : " + callType.ToString());
                dynamic _popupEvent = Convert.ChangeType(message, message.GetType());
                if (_popupEvent != null)
                {
                    ContactData _contact = new ContactData();

                    #region Collect contact Data

                    _contact.SearchData = this.sfdcUtilityHelper.GetVoiceSearchValue(contactVoiceOptions, message, callType);
                    _contact.ObjectName = contactVoiceOptions.ObjectName;
                    _contact.NoRecordFound = SFDCObjectHelper.GetNoRecordFoundAction(callType, contactVoiceOptions);
                    _contact.MultipleMatchRecord = SFDCObjectHelper.GetMultiMatchRecordAction(callType, contactVoiceOptions);
                    _contact.NewRecordFieldIds = contactVoiceOptions.NewrecordFieldIds;
                    _contact.SearchCondition = contactVoiceOptions.SearchCondition;
                    _contact.CreateLogForNewRecord = contactVoiceOptions.CanCreateLogForNewRecordCreate;
                    _contact.MaxRecordOpenCount = contactVoiceOptions.MaxNosRecordOpen;
                    _contact.SearchpageMode = contactVoiceOptions.SearchPageMode;
                    _contact.PhoneNumberSearchFormat = contactVoiceOptions.PhoneNumberSearchFormat;
                    _contact.CanCreateNoRecordActivityLog = SFDCObjectHelper.GetCanCreateProfileActivity(callType, contactVoiceOptions, true);
                    _contact.CanPopupNoRecordActivityLog = SFDCObjectHelper.GetCanPopupProfileActivity(callType, contactVoiceOptions, true);
                    _contact.CanCreateMultiMatchActivityLog = SFDCObjectHelper.GetCanCreateProfileActivity(callType, contactVoiceOptions);
                    _contact.CanPopupMultiMatchActivityLog = SFDCObjectHelper.GetCanPopupProfileActivity(callType, contactVoiceOptions);
                    _contact.CanCreateProfileActivityforInbNoRecord = contactVoiceOptions.CanCreateProfileActivityforInbNoRecord;
                    _contact.CanCreateProfileActivityforOutNoRecord = contactVoiceOptions.CanCreateProfileActivityforOutNoRecord;
                    _contact.CanCreateProfileActivityforConNoRecord = contactVoiceOptions.CanCreateProfileActivityforConNoRecord;
                    if (_contact.NoRecordFound.Equals("createnew") && this.ContactVoiceRecordConfig != null)
                    {
                        _contact.CreateRecordFieldData = this.sfdcUtility.GetCreateActivityLogData(this.ContactVoiceRecordConfig, message, callType);
                    }
                    if (callType == SFDCCallType.InboundVoice)
                    {
                        if (contactVoiceOptions.InboundCanCreateLog)
                        {
                            _contact.CreateActvityLog = true;
                            _contact.ActivityLogData = this.sfdcUtility.GetCreateActivityLogData(this.ContactLogConfig, _popupEvent, callType);
                        }
                    }
                    else if (callType == SFDCCallType.OutboundVoiceSuccess)
                    {
                        if (contactVoiceOptions.OutboundCanCreateLog)
                        {
                            _contact.CreateActvityLog = true;
                            _contact.ActivityLogData = this.sfdcUtility.GetCreateActivityLogData(this.ContactLogConfig, _popupEvent, callType);
                        }
                    }
                    else if (callType == SFDCCallType.OutboundVoiceFailure)
                    {
                        if (contactVoiceOptions.OutboundFailureCanCreateLog)
                        {
                            _contact.CreateActvityLog = true;
                            _contact.ActivityLogData = this.sfdcUtility.GetCreateActivityLogData(this.ContactLogConfig, _popupEvent, callType);
                        }
                    }
                    else if (callType == SFDCCallType.ConsultVoiceReceived)
                    {
                        if (contactVoiceOptions.ConsultCanCreateLog)
                        {
                            _contact.CreateActvityLog = true;
                            _contact.ActivityLogData = this.sfdcUtility.GetCreateActivityLogData(this.ContactLogConfig, _popupEvent, callType);
                        }
                    }
                    return _contact;
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

        #region GetContactEmailPopupData

        /// <summary>
        /// Get Account Popup Data for Email
        /// </summary>
        /// <param name="emailData"></param>
        /// <param name="callType"></param>
        /// <returns></returns>
        public ContactData GetContactEmailPopupData(IXNCustomData emailData, SFDCCallType callType)
        {
            try
            {
                this.logger.Info("GetContactEmailPopupData :  Reading Account Popup Data.....");
                this.logger.Info("GetContactEmailPopupData :  CallType Name : " + callType.ToString());

                if (emailData != null)
                {
                    ContactData _contact = new ContactData();

                    #region Collect Contact Data

                    _contact.SearchData = this.sfdcUtilityHelper.GetEmailSearchValue(contactEmailOptions, emailData, callType);
                    _contact.ObjectName = contactEmailOptions.ObjectName;
                    _contact.NoRecordFound = SFDCObjectHelper.GetNoRecordFoundAction(callType, contactEmailOptions);
                    _contact.MultipleMatchRecord = SFDCObjectHelper.GetMultiMatchRecordAction(callType, contactEmailOptions);
                    _contact.NewRecordFieldIds = contactEmailOptions.NewrecordFieldIds;
                    _contact.SearchCondition = contactEmailOptions.SearchCondition;
                    _contact.CreateLogForNewRecord = contactEmailOptions.CanCreateLogForNewRecordCreate;
                    _contact.MaxRecordOpenCount = contactEmailOptions.MaxNosRecordOpen;
                    _contact.SearchpageMode = contactEmailOptions.SearchPageMode;
                    _contact.PhoneNumberSearchFormat = contactEmailOptions.PhoneNumberSearchFormat;
                    _contact.CanCreateNoRecordActivityLog = SFDCObjectHelper.GetCanCreateProfileActivity(callType, contactEmailOptions, true);
                    _contact.CanPopupNoRecordActivityLog = SFDCObjectHelper.GetCanPopupProfileActivity(callType, contactEmailOptions, true);
                    _contact.CanCreateMultiMatchActivityLog = SFDCObjectHelper.GetCanCreateProfileActivity(callType, contactEmailOptions);
                    _contact.CanPopupMultiMatchActivityLog = SFDCObjectHelper.GetCanPopupProfileActivity(callType, contactEmailOptions);
                    _contact.CanCreateProfileActivityforInbNoRecord = contactEmailOptions.CanCreateProfileActivityforInbNoRecord;
                    _contact.CanCreateProfileActivityforOutNoRecord = contactEmailOptions.CanCreateProfileActivityforOutNoRecord;
                    //_contact.CanCreateProfileActivityforConNoRecord = contactEmailOptions.CanCreateProfileActivityforConNoRecord;
                    if (_contact.NoRecordFound.Equals("createnew") && this.ContactEmailRecordConfig != null)
                    {
                        _contact.CreateRecordFieldData = this.sfdcUtility.GetCreateActivityLogData(this.ContactEmailRecordConfig, null, callType, emailData);
                    }
                    if (callType == SFDCCallType.InboundEmail || callType == SFDCCallType.InboundEmailPulled)
                    {
                        if (contactEmailOptions.InboundCanCreateLog)
                        {
                            _contact.CreateActvityLog = true;
                            _contact.ActivityLogData = this.sfdcUtility.GetCreateActivityLogData(this.ContactEmailLogConfig, null, callType, emailData);
                        }
                    }
                    else if (callType == SFDCCallType.OutboundEmailFailure || callType == SFDCCallType.OutboundEmailSuccess || callType == SFDCCallType.OutboundEmailPulled)
                    {
                        if (contactEmailOptions.OutboundCanCreateLog)
                        {
                            _contact.CreateActvityLog = true;
                            _contact.ActivityLogData = this.sfdcUtility.GetCreateActivityLogData(this.ContactEmailLogConfig, null, callType, emailData);
                        }
                    }

                    #endregion Collect Contact Data

                    return _contact;
                }
            }
            catch (Exception generalException)
            {
                this.logger.Error("GetAccountEmailPopupData : Error occurred while reading Account Data : " + generalException.ToString());
            }
            return null;
        }

        #endregion GetContactEmailPopupData

        #region GetContactChatPopupData

        /// <summary>
        /// Gets Contact Popup Data
        /// </summary>
        /// <param name="message"></param>
        /// <param name="callType"></param>
        /// <returns></returns>
        public ContactData GetContactChatPopupData(IXNCustomData chatData, SFDCCallType callType)
        {
            try
            {
                IMessage message = chatData.InteractionEvent;
                this.logger.Info("GetContactChatPopupData :  Reading Contact Popup Data.....");
                this.logger.Info("GetContactChatPopupData :  Event Name : " + message.Name);
                this.logger.Info("GetContactChatPopupData :  CallType Name : " + callType.ToString());
                dynamic popupEvent = Convert.ChangeType(message, message.GetType());

                if (popupEvent != null)
                {
                    ContactData contact = new ContactData();

                    #region Collect contact Data

                    contact.SearchData = this.sfdcUtilityHelper.GetChatSearchValue(contactChatOptions, chatData, callType);
                    contact.ObjectName = contactChatOptions.ObjectName;
                    contact.NoRecordFound = SFDCObjectHelper.GetNoRecordFoundAction(callType, contactChatOptions);
                    contact.MultipleMatchRecord = SFDCObjectHelper.GetMultiMatchRecordAction(callType, contactChatOptions);
                    contact.NewRecordFieldIds = contactChatOptions.NewrecordFieldIds;
                    contact.SearchCondition = contactChatOptions.SearchCondition;
                    contact.CreateLogForNewRecord = contactChatOptions.CanCreateLogForNewRecordCreate;
                    contact.MaxRecordOpenCount = contactChatOptions.MaxNosRecordOpen;
                    contact.SearchpageMode = contactChatOptions.SearchPageMode;
                    contact.PhoneNumberSearchFormat = contactChatOptions.PhoneNumberSearchFormat;
                    contact.CanCreateNoRecordActivityLog = SFDCObjectHelper.GetCanCreateProfileActivity(callType, contactChatOptions, true);
                    contact.CanPopupNoRecordActivityLog = SFDCObjectHelper.GetCanPopupProfileActivity(callType, contactChatOptions, true);
                    contact.CanCreateMultiMatchActivityLog = SFDCObjectHelper.GetCanCreateProfileActivity(callType, contactChatOptions);
                    contact.CanPopupMultiMatchActivityLog = SFDCObjectHelper.GetCanPopupProfileActivity(callType, contactChatOptions);
                    contact.CanCreateProfileActivityforInbNoRecord = contactChatOptions.CanCreateProfileActivityforInbNoRecord;
                    if (contact.NoRecordFound.Equals("createnew") && this.ContactChatRecordConfig != null)
                    {
                        contact.CreateRecordFieldData = this.sfdcUtility.GetCreateActivityLogData(this.ContactChatRecordConfig, popupEvent, callType, emailData: chatData);
                    }
                    if (callType == SFDCCallType.InboundChat)
                    {
                        if (contactChatOptions.InboundCanCreateLog)
                        {
                            contact.CreateActvityLog = true;
                            contact.ActivityLogData = this.sfdcUtility.GetCreateActivityLogData(this.ContactChatLogConfig, popupEvent, callType, emailData: chatData);
                        }
                    }
                    else if (callType == SFDCCallType.ConsultChatReceived)
                    {
                        if (contactChatOptions.ConsultCanCreateLog)
                        {
                            contact.CreateActvityLog = true;
                            contact.ActivityLogData = this.sfdcUtility.GetCreateActivityLogData(this.ContactChatLogConfig, popupEvent, callType, emailData: chatData);
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

        #region GetContactVoiceUpdateData

        /// <summary>
        /// Gets the Contact Update Data
        /// </summary>
        /// <param name="message"></param>
        /// <param name="callType"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        public ContactData GetContactVoiceUpdateData(IMessage message, string eventName, SFDCCallType callType, string duration, string notes)
        {
            try
            {
                this.logger.Info("GetContactVoiceUpdateData :  Reading contact Update Data.....");
                this.logger.Info("GetContactVoiceUpdateData :  Event Name : " + message.Name);
                dynamic popupEvent = Convert.ChangeType(message, message.GetType());
                if (popupEvent != null)
                {
                    ContactData contact = new ContactData();

                    #region Collect contact Data

                    contact.ObjectName = contactVoiceOptions.ObjectName;

                    if (callType == SFDCCallType.InboundVoice)
                    {
                        if (contactVoiceOptions.InboundCanUpdateLog)
                        {
                            contact.UpdateActivityLog = true;
                            contact.UpdateActivityLogData = this.sfdcUtility.GetUpdateActivityLogData(this.ContactLogConfig, popupEvent, callType, duration, voiceComments: notes);
                            if (!string.IsNullOrWhiteSpace(contactVoiceOptions.VoiceAppendActivityLogEventNames) && contactVoiceOptions.VoiceAppendActivityLogEventNames.Contains(eventName))
                                contact.AppendActivityLogData = this.sfdcUtility.GetUpdateActivityLogData(this.ContactLogConfig, null, callType, duration, voiceComments: notes, isAppendLogData: true);
                        }
                    }
                    else if (callType == SFDCCallType.OutboundVoiceSuccess || callType == SFDCCallType.OutboundVoiceFailure)
                    {
                        if (contactVoiceOptions.OutboundCanUpdateLog)
                        {
                            contact.UpdateActivityLog = true;
                            contact.UpdateActivityLogData = this.sfdcUtility.GetUpdateActivityLogData(this.ContactLogConfig, popupEvent, callType, duration, voiceComments: notes);
                            if (!string.IsNullOrWhiteSpace(contactVoiceOptions.VoiceAppendActivityLogEventNames) && contactVoiceOptions.VoiceAppendActivityLogEventNames.Contains(eventName))
                                contact.AppendActivityLogData = this.sfdcUtility.GetUpdateActivityLogData(this.ContactLogConfig, null, callType, duration, voiceComments: notes, isAppendLogData: true);
                        }
                    }
                    else if (callType == SFDCCallType.ConsultVoiceReceived)
                    {
                        if (contactVoiceOptions.ConsultCanUpdateLog)
                        {
                            contact.UpdateActivityLog = true;
                            contact.UpdateActivityLogData = this.sfdcUtility.GetUpdateActivityLogData(this.ContactLogConfig, popupEvent, callType, duration, voiceComments: notes);
                            if (!string.IsNullOrWhiteSpace(contactVoiceOptions.VoiceAppendActivityLogEventNames) && contactVoiceOptions.VoiceAppendActivityLogEventNames.Contains(eventName))
                                contact.AppendActivityLogData = this.sfdcUtility.GetUpdateActivityLogData(this.ContactLogConfig, null, callType, duration, voiceComments: notes, isAppendLogData: true);
                        }
                    }

                    if (contactVoiceOptions.CanUpdateRecordData)
                    {
                        contact.UpdateRecordFields = true;
                        contact.UpdateRecordFieldsData = this.sfdcUtility.GetUpdateActivityLogData(this.ContactVoiceRecordConfig, popupEvent, callType, duration);
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

        #region GetContactChatUpdateData

        /// <summary>
        /// Gets Chat Update Data
        /// </summary>
        /// <param name="message"></param>
        /// <param name="callType"></param>
        /// <param name="duration"></param>
        /// <param name="chatContent"></param>
        /// <returns></returns>
        public ContactData GetContactChatUpdateData(IXNCustomData chatData, string eventName)
        {
            try
            {
                IMessage message = chatData.InteractionEvent;
                SFDCCallType callType = chatData.InteractionType;
                string duration = chatData.Duration;
                this.logger.Info("GetContactChatUpdateData :  Reading Contact Update Data.....");
                this.logger.Info("GetContactChatUpdateData :  Event Name : " + message.Name);
                dynamic popupEvent = Convert.ChangeType(message, message.GetType());
                if (popupEvent != null)
                {
                    ContactData contact = new ContactData();

                    #region Collect Contact Data

                    contact.ObjectName = contactChatOptions.ObjectName;
                    if (callType == SFDCCallType.InboundChat)
                    {
                        if (contactChatOptions.InboundCanUpdateLog)
                        {
                            contact.UpdateActivityLog = true;
                            contact.UpdateActivityLogData = this.sfdcUtility.GetUpdateActivityLogData(ContactChatLogConfig, popupEvent, callType, duration, emailData: chatData);
                            if (!string.IsNullOrWhiteSpace(contactChatOptions.ChatAppendActivityLogEventNames) && contactChatOptions.ChatAppendActivityLogEventNames.Contains(eventName))
                                contact.AppendActivityLogData = this.sfdcUtility.GetUpdateActivityLogData(this.ContactChatLogConfig, null, callType, duration, emailData: chatData, isAppendLogData: true);
                        }
                    }
                    else if (callType == SFDCCallType.ConsultChatReceived)
                    {
                        if (contactChatOptions.ConsultCanUpdateLog)
                        {
                            contact.UpdateActivityLog = true;
                            contact.UpdateActivityLogData = this.sfdcUtility.GetUpdateActivityLogData(this.ContactChatLogConfig, popupEvent, callType, duration, emailData: chatData);
                            if (!string.IsNullOrWhiteSpace(contactChatOptions.ChatAppendActivityLogEventNames) && contactChatOptions.ChatAppendActivityLogEventNames.Contains(eventName))
                                contact.AppendActivityLogData = this.sfdcUtility.GetUpdateActivityLogData(this.ContactChatLogConfig, null, callType, duration, emailData: chatData, isAppendLogData: true);
                        }
                    }

                    //update case record fields
                    contact.UpdateRecordFields = contactChatOptions.CanUpdateRecordData;
                    if (SFDCObjectHelper.GetNoRecordFoundAction(callType, contactChatOptions).Equals("createnew") && this.ContactChatRecordConfig != null)
                    {
                        if (contactChatOptions.CanUpdateRecordData)
                        {
                            contact.UpdateRecordFieldsData = this.sfdcUtility.GetUpdateActivityLogData(this.ContactChatRecordConfig, popupEvent, callType, duration, emailData: chatData);
                        }
                    }

                    #endregion Collect Contact Data

                    return contact;
                }
            }
            catch (Exception generalException)
            {
                this.logger.Error("GetContactChatUpdateData : Error occurred while reading Contact Data : " + generalException.ToString());
            }
            return null;
        }

        #endregion GetContactChatUpdateData

        #region GetContactEmailUpdateData

        /// <summary>
        /// Gets the Chat Update data.
        /// </summary>
        /// <param name="emailData"></param>
        /// <param name="callType"></param>
        /// <param name="duration"></param>
        /// <param name="emailContent"></param>
        /// <returns></returns>
        public ContactData GetContactEmailUpdateData(IXNCustomData emailData, SFDCCallType callType, string eventName)
        {
            ContactData contact = new ContactData();
            try
            {
                this.logger.Info("GetContactOutboundEmailUpdateData :  Reading Account Update Data.....");

                if (emailData != null)
                {
                    #region Collect cases Data

                    contact.ObjectName = contactEmailOptions.ObjectName;
                    if (callType == SFDCCallType.InboundEmail || callType == SFDCCallType.InboundEmailPulled)
                    {
                        if (contactEmailOptions.InboundCanUpdateLog)
                        {
                            contact.UpdateActivityLog = true;
                            contact.UpdateActivityLogData = this.sfdcUtility.GetUpdateActivityLogData(this.ContactEmailLogConfig, null, callType, emailData.Duration, emailData: emailData);
                            if (!string.IsNullOrWhiteSpace(contactEmailOptions.EmailAppendActivityLogEventNames) && contactEmailOptions.EmailAppendActivityLogEventNames.Contains(eventName))
                                contact.AppendActivityLogData = this.sfdcUtility.GetUpdateActivityLogData(this.ContactEmailLogConfig, null, callType, emailData.Duration, emailData: emailData, isAppendLogData: true);
                        }
                    }
                    else if (callType == SFDCCallType.OutboundEmailFailure || callType == SFDCCallType.OutboundEmailSuccess || callType == SFDCCallType.OutboundEmailPulled)
                    {
                        if (contactEmailOptions.OutboundCanUpdateLog)
                        {
                            contact.UpdateActivityLog = true;
                            contact.UpdateActivityLogData = this.sfdcUtility.GetUpdateActivityLogData(this.ContactEmailLogConfig, null, callType, emailData.Duration, emailData: emailData);
                            if (!string.IsNullOrWhiteSpace(contactEmailOptions.EmailAppendActivityLogEventNames) && contactEmailOptions.EmailAppendActivityLogEventNames.Contains(eventName))
                                contact.AppendActivityLogData = this.sfdcUtility.GetUpdateActivityLogData(this.ContactEmailLogConfig, null, callType, emailData.Duration, emailData: emailData, isAppendLogData: true);
                        }
                    }
                    //update account record fields
                    contact.UpdateRecordFields = contactEmailOptions.CanUpdateRecordData;
                    contact.NoRecordFound = SFDCObjectHelper.GetNoRecordFoundAction(callType, contactEmailOptions);
                    if (contact.NoRecordFound.Equals("createnew") && this.ContactEmailRecordConfig != null)
                    {
                        if (contactEmailOptions.CanUpdateRecordData)
                        {
                            contact.UpdateRecordFieldsData = this.sfdcUtility.GetUpdateActivityLogData(this.ContactEmailRecordConfig, null, callType, emailData.Duration, emailData: emailData);
                        }
                    }

                    #endregion Collect cases Data

                    return contact;
                }
            }
            catch (Exception generalException)
            {
                this.logger.Error("GetContactOutboundEmailUpdateData : Error occurred while reading Account Data : " + generalException.ToString());
                return contact;
            }
            return null;
        }

        #endregion GetContactEmailUpdateData
    }
}