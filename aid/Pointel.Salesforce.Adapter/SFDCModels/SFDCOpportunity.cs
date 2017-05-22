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
    /// Comment: Provides Search and Activity Data for Opportunity Object
    /// Last Modified: 25-08-2015
    /// Created by: Pointel Inc
    /// </summary>
    internal class SFDCOpportunity
    {
        #region Field Declaration

        private static SFDCOpportunity sfdcOpportunity = null;
        private Log logger = null;
        private VoiceOptions opportunityVoiceOptions = null;
        private ChatOptions opportunityChatOptions = null;
        private KeyValueCollection OpportunityLogConfig = null;
        private KeyValueCollection OpportunityChatLogConfig = null;
        private KeyValueCollection OpportunityRecordConfig = null;
        private SFDCUtility sfdcObject = null;

        #endregion Field Declaration

        #region Constructor

        private SFDCOpportunity()
        {
            this.logger = Log.GenInstance();
            this.opportunityVoiceOptions = Settings.OpportunityVoiceOptions;
            this.opportunityChatOptions = Settings.OpportunityChatOptions;
            this.OpportunityLogConfig = (Settings.VoiceActivityLogCollection.ContainsKey("opportunity")) ? Settings.VoiceActivityLogCollection["opportunity"] : null;
            this.OpportunityChatLogConfig = (Settings.ChatActivityLogCollection.ContainsKey("opportunity")) ? Settings.ChatActivityLogCollection["opportunity"] : null;
            this.OpportunityRecordConfig = Settings.OpportunityNewRecordConfigs;
            this.sfdcObject = SFDCUtility.GetInstance();
        }

        #endregion Constructor

        #region GetInstance Method

        public static SFDCOpportunity GetInstance()
        {
            if (sfdcOpportunity == null)
            {
                sfdcOpportunity = new SFDCOpportunity();
            }
            return sfdcOpportunity;
        }

        #endregion GetInstance Method

        #region GetOpportunityVoicePopupData

        public IOpportunity GetOpportunityVoicePopupData(IMessage message, SFDCCallType callType)
        {
            try
            {
                this.logger.Info("GetOpportunityVoicePopupData :  Reading Opportunity Popup Data.....");
                this.logger.Info("GetOpportunityVoicePopupData :  Event Name : " + message.Name);
                this.logger.Info("GetOpportunityVoicePopupData :  CallType Name : " + callType.ToString());
                dynamic popupEvent = Convert.ChangeType(message, message.GetType());
                if (popupEvent != null)
                {
                    IOpportunity Opportunity = new OpportunityData();
                    Opportunity.SearchData = GetOpportunityVoiceSearchValue(popupEvent.UserData, message, callType);
                    Opportunity.ObjectName = opportunityVoiceOptions.ObjectName;
                    Opportunity.NoRecordFound = GetNoRecordFoundAction(callType, opportunityVoiceOptions);
                    Opportunity.MultipleMatchRecord = GetMultiMatchRecordAction(callType, opportunityVoiceOptions);
                    Opportunity.NewRecordFieldIds = opportunityVoiceOptions.NewrecordFieldIds;
                    Opportunity.SearchCondition = opportunityVoiceOptions.SearchCondition;
                    Opportunity.CreateLogForNewRecord = opportunityVoiceOptions.CanCreateLogForNewRecordCreate;
                    Opportunity.MaxRecordOpenCount = opportunityVoiceOptions.MaxNosRecordOpen;
                    Opportunity.SearchpageMode = opportunityVoiceOptions.SearchPageMode;
                    Opportunity.PhoneNumberSearchFormat = opportunityVoiceOptions.PhoneNumberSearchFormat;
                    Opportunity.CanCreateNoRecordActivityLog = GetCanCreateProfileActivity(callType, opportunityVoiceOptions, true);
                    Opportunity.CanPopupNoRecordActivityLog = GetCanPopupProfileActivity(callType, opportunityVoiceOptions, true);
                    Opportunity.CanCreateMultiMatchActivityLog = GetCanCreateProfileActivity(callType, opportunityVoiceOptions);
                    Opportunity.CanPopupMultiMatchActivityLog = GetCanPopupProfileActivity(callType, opportunityVoiceOptions);
                    Opportunity.CanCreateProfileActivityforInbNoRecord = opportunityVoiceOptions.CanCreateProfileActivityforInbNoRecord;
                    Opportunity.CanCreateProfileActivityforOutNoRecord = opportunityVoiceOptions.CanCreateProfileActivityforOutNoRecord;
                    Opportunity.CanCreateProfileActivityforConNoRecord = opportunityVoiceOptions.CanCreateProfileActivityforConNoRecord;
                    if (Opportunity.NoRecordFound.Equals("createnew"))
                    {
                        Opportunity.CreateRecordFieldData = this.sfdcObject.GetVoiceRecordData(this.OpportunityRecordConfig, message, callType);
                    }
                    if (callType == SFDCCallType.Inbound)
                    {
                        if (opportunityVoiceOptions.InboundCanCreateLog)
                        {
                            Opportunity.CreateActvityLog = true;
                            Opportunity.ActivityLogData = this.sfdcObject.GetVoiceActivityLog(this.OpportunityLogConfig, popupEvent, callType);
                        }
                    }
                    else if (callType == SFDCCallType.OutboundSuccess)
                    {
                        if (opportunityVoiceOptions.OutboundCanCreateLog)
                        {
                            Opportunity.CreateActvityLog = true;
                            Opportunity.ActivityLogData = this.sfdcObject.GetVoiceActivityLog(this.OpportunityLogConfig, popupEvent, callType);
                        }
                    }
                    else if (callType == SFDCCallType.OutboundFailure)
                    {
                        if (opportunityVoiceOptions.OutboundFailureCanCreateLog)
                        {
                            Opportunity.CreateActvityLog = true;
                            Opportunity.ActivityLogData = this.sfdcObject.GetVoiceActivityLog(this.OpportunityLogConfig, popupEvent, callType);
                        }
                    }
                    else if (callType == SFDCCallType.ConsultReceived)
                    {
                        if (opportunityVoiceOptions.ConsultCanCreateLog)
                        {
                            Opportunity.CreateActvityLog = true;
                            Opportunity.ActivityLogData = this.sfdcObject.GetVoiceActivityLog(this.OpportunityLogConfig, popupEvent, callType);
                        }
                    }
                    else if (callType == SFDCCallType.ConsultSuccess)
                    {
                        Opportunity.NoRecordFound = "none";
                        if (opportunityVoiceOptions.ConsultCanCreateLog)
                        {
                            Opportunity.CreateActvityLog = true;
                            Opportunity.ActivityLogData = this.sfdcObject.GetVoiceActivityLog(this.OpportunityLogConfig, popupEvent, callType);
                        }
                    }
                    else if (callType == SFDCCallType.ConsultFailure)
                    {
                        Opportunity.NoRecordFound = "none";
                        if (opportunityVoiceOptions.ConsultFailureCanCreateLog)
                        {
                            Opportunity.CreateActvityLog = true;
                            Opportunity.ActivityLogData = this.sfdcObject.GetVoiceActivityLog(this.OpportunityLogConfig, popupEvent, callType);
                        }
                    }
                    return Opportunity;
                }
            }
            catch (Exception generalException)
            {
                this.logger.Error("GetOpportunityVoicePopupData : Error occurred while reading Opportunity Data : " + generalException.ToString());
            }
            return null;
        }

        #endregion GetOpportunityVoicePopupData

        #region GetOpportunityChatPopupData

        public IOpportunity GetOpportunityChatPopupData(IMessage message, SFDCCallType callType)
        {
            try
            {
                this.logger.Info("GetOpportunityChatPopupData :  Reading Opportunity Popup Data.....");
                this.logger.Info("GetOpportunityChatPopupData :  Event Name : " + message.Name);
                this.logger.Info("GetOpportunityChatPopupData :  CallType Name : " + callType.ToString());
                dynamic popupEvent = Convert.ChangeType(message, message.GetType());
                if (popupEvent != null)
                {
                    IOpportunity Opportunity = new OpportunityData();

                    #region Collect Lead Data

                    Opportunity.SearchData = GetOpportunityChatSearchValue(popupEvent.Interaction.InteractionUserData, message, callType);
                    Opportunity.ObjectName = opportunityChatOptions.ObjectName;
                    Opportunity.NoRecordFound = GetNoRecordFoundAction(callType, opportunityChatOptions);
                    Opportunity.MultipleMatchRecord = GetMultiMatchRecordAction(callType, opportunityChatOptions);
                    Opportunity.NewRecordFieldIds = opportunityChatOptions.NewrecordFieldIds;
                    Opportunity.SearchCondition = opportunityChatOptions.SearchCondition;
                    Opportunity.CreateLogForNewRecord = opportunityChatOptions.CanCreateLogForNewRecordCreate;
                    Opportunity.MaxRecordOpenCount = opportunityChatOptions.MaxNosRecordOpen;
                    Opportunity.SearchpageMode = opportunityChatOptions.SearchPageMode;
                    Opportunity.PhoneNumberSearchFormat = opportunityChatOptions.PhoneNumberSearchFormat;
                    if (Opportunity.NoRecordFound.Equals("createnew"))
                    {
                        Opportunity.CreateRecordFieldData = this.sfdcObject.GetChatRecordData(this.OpportunityRecordConfig, message, callType);
                    }
                    if (callType == SFDCCallType.InboundChat)
                    {
                        if (opportunityChatOptions.InboundCanCreateLog)
                        {
                            Opportunity.CreateActvityLog = true;
                            Opportunity.ActivityLogData = this.sfdcObject.GetChatActivityLog(this.OpportunityChatLogConfig, popupEvent, callType);
                        }
                    }
                    else if (callType == SFDCCallType.ConsultChatSuccess || callType == SFDCCallType.ConsultChatFailure || callType == SFDCCallType.ConsultChatReceived)
                    {
                        if (opportunityChatOptions.ConsultCanCreateLog)
                        {
                            Opportunity.CreateActvityLog = true;
                            Opportunity.ActivityLogData = this.sfdcObject.GetChatActivityLog(this.OpportunityChatLogConfig, popupEvent, callType);
                        }
                    }
                    return Opportunity;

                    #endregion Collect Lead Data
                }
            }
            catch (Exception generalException)
            {
                this.logger.Error("GetOpportunityChatPopupData : Error occurred while reading Opportunity Data : " + generalException.ToString());
            }
            return null;
        }

        #endregion GetOpportunityChatPopupData

        #region GetOpportunityVoiceSearchValue

        private string GetOpportunityVoiceSearchValue(KeyValueCollection userData, IMessage message, SFDCCallType callType)
        {
            try
            {
                this.logger.Info("GetOpportunityVoiceSearchValue :  Reading Opportunity Popup Data.....");
                this.logger.Info("GetOpportunityVoiceSearchValue :  UserData Name : " + Convert.ToString(userData));
                this.logger.Info("GetOpportunityVoiceSearchValue :  Event Name : " + message.Name);
                this.logger.Info("GetOpportunityVoiceSearchValue :  CallType Name : " + callType.ToString());

                string[] userDataSearchKeys = null;
                string[] attributeSearchKeys = null;
                string searchValue = string.Empty;
                if (callType == SFDCCallType.Inbound)
                {
                    userDataSearchKeys = (this.opportunityVoiceOptions.InboundSearchUserDataKeys != null) ? this.opportunityVoiceOptions.InboundSearchUserDataKeys.Split(',') : null;
                    attributeSearchKeys = (this.opportunityVoiceOptions.InboundSearchAttributeKeys != null) ? this.opportunityVoiceOptions.InboundSearchAttributeKeys.Split(',') : null;
                }
                else if (callType == SFDCCallType.OutboundSuccess || callType == SFDCCallType.OutboundFailure)
                {
                    userDataSearchKeys = (this.opportunityVoiceOptions.OutboundSearchUserDataKeys != null) ? this.opportunityVoiceOptions.OutboundSearchUserDataKeys.Split(',') : null;
                    attributeSearchKeys = (this.opportunityVoiceOptions.OutboundSearchAttributeKeys != null) ? this.opportunityVoiceOptions.OutboundSearchAttributeKeys.Split(',') : null;
                }
                else if (callType == SFDCCallType.ConsultSuccess || callType == SFDCCallType.ConsultFailure)
                {
                    return this.sfdcObject.GetAttributeSearchValues(message, new string[] { "otherdn" });
                }
                else if (callType == SFDCCallType.ConsultReceived)
                {
                    userDataSearchKeys = (this.opportunityVoiceOptions.ConsultSearchUserDataKeys != null) ? this.opportunityVoiceOptions.ConsultSearchUserDataKeys.Split(',') : null;
                    attributeSearchKeys = (this.opportunityVoiceOptions.ConsultSearchAttributeKeys != null) ? this.opportunityVoiceOptions.ConsultSearchAttributeKeys.Split(',') : null;
                }

                #region OLD

                //if (opportunityVoiceOptions.SearchPriority == "user-data")
                //{
                //    if (userDataSearchKeys != null)
                //        searchValue = this.sfdcObject.GetUserDataSearchValues(userData, userDataSearchKeys);
                //    else if (attributeSearchKeys != null)
                //        searchValue = this.sfdcObject.GetAttributeSearchValues(message, attributeSearchKeys);
                //}
                //else if (opportunityVoiceOptions.SearchPriority == "attribute")
                //{
                //    searchValue = this.sfdcObject.GetAttributeSearchValues(message, attributeSearchKeys);
                //}
                //else if (opportunityVoiceOptions.SearchPriority == "both")
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

                if (opportunityVoiceOptions.SearchPriority == "user-data")
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
                else if (opportunityVoiceOptions.SearchPriority == "attribute")
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
                else if (opportunityVoiceOptions.SearchPriority == "both")
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
                this.logger.Error("GetOpportunityVoiceSearchValue : Error occurred while reading Opportunity Search value : " + generalException.ToString());
            }
            return null;
        }

        #endregion GetOpportunityVoiceSearchValue

        #region GetOpportunityVoiceUpdateData

        public IOpportunity GetOpportunityVoiceUpdateData(IMessage message, SFDCCallType callType, string duration)
        {
            try
            {
                this.logger.Info("GetOpportunityVoiceUpdateData :  Reading Opportunity Update Data.....");
                this.logger.Info("GetOpportunityVoiceUpdateData :  Event Name : " + message.Name);
                dynamic popupEvent = Convert.ChangeType(message, message.GetType());

                if (popupEvent != null)
                {
                    IOpportunity opportunity = new OpportunityData();

                    #region Collect opportunity Data

                    opportunity.ObjectName = opportunityVoiceOptions.ObjectName;

                    if (callType == SFDCCallType.Inbound)
                    {
                        if (opportunityVoiceOptions.InboundCanUpdateLog)
                        {
                            opportunity.UpdateActivityLog = true;
                            opportunity.UpdateActivityLogData = this.sfdcObject.GetVoiceUpdateActivityLog(this.OpportunityLogConfig, popupEvent, callType, duration);
                        }
                    }
                    else if (callType == SFDCCallType.OutboundSuccess || callType == SFDCCallType.OutboundFailure)
                    {
                        if (opportunityVoiceOptions.OutboundCanUpdateLog)
                        {
                            opportunity.UpdateActivityLog = true;
                            opportunity.UpdateActivityLogData = this.sfdcObject.GetVoiceUpdateActivityLog(this.OpportunityLogConfig, popupEvent, callType, duration);
                        }
                    }
                    else if (callType == SFDCCallType.ConsultSuccess || callType == SFDCCallType.ConsultReceived || callType == SFDCCallType.ConsultFailure)
                    {
                        if (opportunityVoiceOptions.ConsultCanUpdateLog)
                        {
                            opportunity.UpdateActivityLog = true;
                            opportunity.UpdateActivityLogData = this.sfdcObject.GetVoiceUpdateActivityLog(this.OpportunityLogConfig, popupEvent, callType, duration);
                        }
                    }

                    if (opportunityVoiceOptions.CanUpdateRecordData)
                    {
                        opportunity.UpdateRecordFields = true;
                        opportunity.UpdateRecordFieldsData = this.sfdcObject.GetVoiceUpdateRecordData(this.OpportunityRecordConfig, popupEvent, callType, duration);
                    }

                    #endregion Collect opportunity Data

                    return opportunity;
                }
            }
            catch (Exception generalException)
            {
                this.logger.Error("GetOpportunityVoiceUpdateData : Error occurred while reading opportunity Data : " + generalException.ToString());
            }
            return null;
        }

        #endregion GetOpportunityVoiceUpdateData

        #region GetOpportunityChatSearchValue

        private string GetOpportunityChatSearchValue(KeyValueCollection userData, IMessage message, SFDCCallType callType)
        {
            try
            {
                this.logger.Info("GetOpportunityChatSearchValue :  Reading Opportunity Popup Data.....");
                this.logger.Info("GetOpportunityChatSearchValue :  UserData Name : " + Convert.ToString(userData));
                this.logger.Info("GetOpportunityChatSearchValue :  Event Name : " + message.Name);
                this.logger.Info("GetOpportunityChatSearchValue :  CallType Name : " + callType.ToString());
                string[] userDataSearchKeys = null;
                string[] attributeSearchKeys = null;
                string searchValue = string.Empty;
                if (callType == SFDCCallType.InboundChat)
                {
                    userDataSearchKeys = (this.opportunityChatOptions.InboundSearchUserDataKeys != null) ? this.opportunityChatOptions.InboundSearchUserDataKeys.Split(',') : null;
                    attributeSearchKeys = (this.opportunityChatOptions.InboundSearchAttributeKeys != null) ? this.opportunityChatOptions.InboundSearchAttributeKeys.Split(',') : null;
                }
                else if (callType == SFDCCallType.ConsultChatReceived)
                {
                    userDataSearchKeys = (this.opportunityChatOptions.ConsultSearchUserDataKeys != null) ? this.opportunityChatOptions.ConsultSearchUserDataKeys.Split(',') : null;
                    attributeSearchKeys = (this.opportunityChatOptions.ConsultSearchAttributeKeys != null) ? this.opportunityChatOptions.ConsultSearchAttributeKeys.Split(',') : null;
                }

                if (opportunityChatOptions.SearchPriority == "user-data")
                {
                    searchValue = this.sfdcObject.GetUserDataSearchValues(userData, userDataSearchKeys);
                }
                else if (opportunityChatOptions.SearchPriority == "attribute")
                {
                    searchValue = this.sfdcObject.GetAttributeSearchValues(message, attributeSearchKeys);
                }
                else if (opportunityChatOptions.SearchPriority == "both")
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
                this.logger.Error("GetOpportunityChatSearchValue : Error occurred while reading Opportunity Data : " + generalException.ToString());
            }
            return null;
        }

        #endregion GetOpportunityChatSearchValue

        #region GetOpportunityChatUpdateData

        public IOpportunity GetOpportunityChatUpdateData(IMessage message, SFDCCallType callType, string duration, string chatContent)
        {
            try
            {
                this.logger.Info("GetOpportunityChatUpdateData :  Reading Opportunity Update Data.....");
                this.logger.Info("GetOpportunityChatUpdateData :  Event Name : " + message.Name);
                dynamic popupEvent = Convert.ChangeType(message, message.GetType());

                if (popupEvent != null)
                {
                    IOpportunity opportunity = new OpportunityData();

                    #region Collect Lead Data

                    opportunity.ObjectName = opportunityChatOptions.ObjectName;
                    if (callType == SFDCCallType.InboundChat)
                    {
                        if (opportunityChatOptions.InboundCanUpdateLog)
                        {
                            opportunity.UpdateActivityLog = true;
                            opportunity.UpdateActivityLogData = this.sfdcObject.GetChatUpdateActivityLog(this.OpportunityChatLogConfig, popupEvent, callType, duration, chatContent);
                        }
                    }
                    else
                        if (callType == SFDCCallType.ConsultChatReceived)
                        {
                            if (opportunityChatOptions.ConsultCanUpdateLog)
                            {
                                opportunity.UpdateActivityLog = true;
                                opportunity.UpdateActivityLogData = this.sfdcObject.GetChatUpdateActivityLog(this.OpportunityChatLogConfig, popupEvent, callType, duration, chatContent);
                            }
                        }
                        else if (callType == SFDCCallType.ConsultChatSuccess || callType == SFDCCallType.ConsultChatFailure)
                        {
                            if (opportunityChatOptions.ConsultCanUpdateLog)
                            {
                                opportunity.UpdateActivityLog = true;
                                opportunity.UpdateActivityLogData = this.sfdcObject.GetChatUpdateActivityLog(this.OpportunityChatLogConfig, popupEvent, callType, duration, chatContent);
                            }
                        }
                    //update opportunity record fields
                    opportunity.UpdateRecordFields = opportunityChatOptions.CanUpdateRecordData;
                    if (GetNoRecordFoundAction(callType, opportunityChatOptions).Equals("createnew") && this.OpportunityRecordConfig != null)
                    {
                        if (opportunityChatOptions.CanUpdateRecordData)
                        {
                            opportunity.UpdateRecordFieldsData = this.sfdcObject.GetChatUpdateActivityLog(this.OpportunityRecordConfig, popupEvent, callType, duration, chatContent);
                        }
                    }

                    #endregion Collect Lead Data

                    return opportunity;
                }
            }
            catch (Exception generalException)
            {
                this.logger.Error("GetOpportunityChatUpdateData : Error occurred while reading Opportunity Data : " + generalException.ToString());
            }
            return null;
        }

        #endregion GetOpportunityChatUpdateData

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

        #region GetNoRecordFoundAction  for voice

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

        #endregion GetNoRecordFoundAction  for voice

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