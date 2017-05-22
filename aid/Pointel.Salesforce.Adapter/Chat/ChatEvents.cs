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
using Pointel.Salesforce.Adapter.SFDCModels;
using Pointel.Salesforce.Adapter.SFDCUtils;
using Pointel.Salesforce.Adapter.Utility;
using System;
using System.Collections.Generic;

namespace Pointel.Salesforce.Adapter.Chat
{/// <summary>
    /// Comment: Provides Chat Common Methods
    /// Last Modified: 25-08-2015
    /// Created by: Pointel Inc
    /// </summary>
    internal class ChatEvents
    {
        #region Fields

        /// <summary>
        /// Fields for the Class ChatEvents
        /// </summary>
        private Log logger = null;

        private static ChatEvents currentObject = null;
        private ChatOptions leadOptions = null;
        private ChatOptions contactOptions = null;
        private ChatOptions accountOptions = null;
        private ChatOptions caseOptions = null;
        private ChatOptions opportunityOptions = null;
        private IDictionary<string, ChatOptions> customObjectOptions = null;
        private IDictionary<string, KeyValueCollection> activityLogs = null;
        private UserActivityOptions userActivityOptions = null;
        private KeyValueCollection userActivityLog = null;
        private SFDCUtility sfdcObject = null;
        private string[] PopupPages = null;
        private List<ICustomObject> tempCustomObject = new List<ICustomObject>();

        #endregion Fields

        #region Constructor

        /// <summary>
        /// Creates an Instance of the Class
        /// </summary>
        public ChatEvents()
        {
            this.logger = Log.GenInstance();
            this.leadOptions = Settings.LeadChatOptions;
            this.contactOptions = Settings.ContactChatOptions;
            this.accountOptions = Settings.AccountChatOptions;
            this.caseOptions = Settings.CaseChatOptions;
            this.opportunityOptions = Settings.OpportunityChatOptions;
            this.userActivityLog = (Settings.ChatActivityLogCollection.ContainsKey("useractivity")) ? Settings.ChatActivityLogCollection["useractivity"] : null;
            this.sfdcObject = SFDCUtility.GetInstance();
            this.activityLogs = Settings.ChatActivityLogCollection;
            this.customObjectOptions = Settings.CustomObjectChatOptions;
            this.userActivityOptions = Settings.UserActivityChatOptions;
            this.PopupPages = Settings.SFDCOptions.SFDCPopupPages;
        }

        #endregion Constructor

        #region GetInstance

        /// <summary>
        /// Gets the instance of the Class.s
        /// </summary>
        /// <returns></returns>
        public static ChatEvents GetInstance()
        {
            if (currentObject == null)
            {
                currentObject = new ChatEvents();
            }
            return currentObject;
        }

        #endregion GetInstance

        #region Inbound Popup Data

        /// <summary>
        /// Gets the Inbound Popup Data
        /// </summary>
        /// <param name="message"></param>
        /// <param name="callType"></param>
        /// <param name="eventName"></param>
        /// <returns></returns>
        public SFDCData GetInboundPopupData(IMessage message, SFDCCallType callType, string eventName)
        {
            SFDCData sfdcData = new SFDCData();
            try
            {
                logger.Info("GetInboundPopupData : Getting Inbound Popup Data");
                if (this.PopupPages != null)
                {
                    foreach (string key in this.PopupPages)
                    {
                        switch (key)
                        {
                            case "lead":
                                if (this.leadOptions != null && this.leadOptions.InboundPopupEvent != null && this.leadOptions.InboundPopupEvent.Equals(eventName))
                                {
                                    logger.Info("GetInboundPopupData : Reads Lead Chat PopupData");
                                    sfdcData.LeadData = SFDCLead.GetInstance().GetLeadChatPopupData(message, callType);
                                }
                                break;

                            case "contact":
                                if (this.contactOptions != null && this.contactOptions.InboundPopupEvent != null && this.contactOptions.InboundPopupEvent.Equals(eventName))
                                {
                                    logger.Info("GetInboundPopupData : Reads contact Chat PopupData");
                                    sfdcData.ContactData = SFDCContact.GetInstance().GetContactChatPopupData(message, callType);
                                }
                                break;

                            case "account":
                                if (this.accountOptions != null && this.accountOptions.InboundPopupEvent != null && this.accountOptions.InboundPopupEvent.Equals(eventName))
                                {
                                    logger.Info("GetInboundPopupData : Reads account Chat PopupData");
                                    sfdcData.AccountData = SFDCAccount.GetInstance().GetAccountChatPopupData(message, callType);
                                }
                                break;

                            case "case":
                                if (this.caseOptions != null && this.caseOptions.InboundPopupEvent != null && this.caseOptions.InboundPopupEvent.Equals(eventName))
                                {
                                    logger.Info("GetInboundPopupData : Reads case Chat PopupData");
                                    sfdcData.CaseData = SFDCCase.GetInstance().GetCaseChatPopupData(message, callType);
                                }
                                break;

                            case "opportunity":
                                if (this.opportunityOptions != null && this.opportunityOptions.InboundPopupEvent != null && this.opportunityOptions.InboundPopupEvent.Equals(eventName))
                                {
                                    logger.Info("GetInboundPopupData : Reads opportunity Chat PopupData");
                                    sfdcData.OpportunityData = SFDCOpportunity.GetInstance().GetOpportunityChatPopupData(message, callType);
                                }
                                break;

                            case "useractivity":
                                if (this.userActivityOptions != null && this.userActivityOptions.InboundPopupEvent != null &&
                    this.userActivityOptions.InboundPopupEvent.Equals(eventName))
                                {
                                    logger.Info("GetInboundPopupData : Reads useractivity Chat PopupData");
                                    sfdcData.UserActivityData = SFDCUserActivity.GetInstance().GetChatCreateUserAcitivityData(message, callType);
                                }
                                break;

                            default:
                                if (this.customObjectOptions.ContainsKey(key))
                                {
                                    if (this.customObjectOptions[key] != null && this.customObjectOptions[key].InboundPopupEvent != null &&
                                            this.customObjectOptions[key].InboundPopupEvent.Equals(eventName))
                                    {
                                        logger.Info("GetInboundPopupData : Reads CustomObject Chat PopupData");
                                        ICustomObject cstobject = SFDCCustomObject.GetInstance().GetCustomObjectChatPopupData(message, callType, key);
                                        if (cstobject != null)
                                        {
                                            this.tempCustomObject.Add(cstobject);
                                        }
                                    }
                                }
                                break;
                        }
                    }
                    if (this.tempCustomObject.Count > 0)
                    {
                        ICustomObject[] temp = this.tempCustomObject.ToArray();
                        sfdcData.CustomObjectData = temp;
                        this.tempCustomObject.Clear();
                    }
                }
            }
            catch (Exception generalException)
            {
                logger.Error("GetInboundPopupData : Error Occurred While Getting Inbound Popup Data : " + generalException.ToString());
            }
            return sfdcData;
        }

        #endregion Inbound Popup Data

        #region Consult Received Popup Data

        /// <summary>
        /// Gets the Consult Received popup Data
        /// </summary>
        /// <param name="message"></param>
        /// <param name="callType"></param>
        /// <param name="eventName"></param>
        /// <returns></returns>
        public SFDCData GetConsultReceivedPopupData(IMessage message, SFDCCallType callType, string eventName)
        {
            SFDCData sfdcData = new SFDCData();
            try
            {
                logger.Info("GetConsultReceivedPopupData : Getting Consult Received Popup Data");
                if (this.PopupPages != null)
                {
                    foreach (string key in this.PopupPages)
                    {
                        switch (key)
                        {
                            case "lead":
                                if (this.leadOptions != null && this.leadOptions.ConsultPopupEvent != null &&
                    this.leadOptions.ConsultPopupEvent.Equals(eventName))
                                {
                                    logger.Info("GetConsultReceivedPopupData : Reading Lead Popup Data");
                                    sfdcData.LeadData = SFDCLead.GetInstance().GetLeadChatPopupData(message, callType);
                                }
                                break;

                            case "contact":
                                if (this.contactOptions != null && this.contactOptions.ConsultPopupEvent != null &&
                    this.contactOptions.ConsultPopupEvent.Equals(eventName))
                                {
                                    logger.Info("GetConsultReceivedPopupData : Reading Lead Popup Data");
                                    sfdcData.ContactData = SFDCContact.GetInstance().GetContactChatPopupData(message, callType);
                                }
                                break;

                            case "account":
                                if (this.accountOptions != null && this.accountOptions.ConsultPopupEvent != null &&
                   this.accountOptions.ConsultPopupEvent.Equals(eventName))
                                {
                                    logger.Info("GetConsultReceivedPopupData : Reading account Popup Data");
                                    sfdcData.AccountData = SFDCAccount.GetInstance().GetAccountChatPopupData(message, callType);
                                }
                                break;

                            case "case":
                                if (this.caseOptions != null && this.caseOptions.ConsultPopupEvent != null &&
                    this.caseOptions.ConsultPopupEvent.Equals(eventName))
                                {
                                    logger.Info("GetConsultReceivedPopupData : Reading case Popup Data");
                                    sfdcData.CaseData = SFDCCase.GetInstance().GetCaseChatPopupData(message, callType);
                                }
                                break;

                            case "opportunity":
                                if (this.opportunityOptions != null && this.opportunityOptions.ConsultPopupEvent != null &&
                   this.opportunityOptions.ConsultPopupEvent.Equals(eventName))
                                {
                                    logger.Info("GetConsultReceivedPopupData : Reading opportunity Popup Data");
                                    sfdcData.OpportunityData = SFDCOpportunity.GetInstance().GetOpportunityChatPopupData(message, callType);
                                }
                                break;

                            case "useractivity":
                                if (this.userActivityOptions != null && this.userActivityOptions.ConsultPopupEvent != null &&
                   this.userActivityOptions.ConsultPopupEvent.Equals(eventName))
                                {
                                    logger.Info("GetConsultReceivedPopupData : Reading useractivity Popup Data");
                                    sfdcData.UserActivityData = SFDCUserActivity.GetInstance().GetChatCreateUserAcitivityData(message, callType);
                                }
                                break;

                            default:
                                if (this.customObjectOptions.ContainsKey(key))
                                {
                                    if (this.customObjectOptions[key] != null && this.customObjectOptions[key].ConsultPopupEvent != null &&
                                            this.customObjectOptions[key].ConsultPopupEvent.Equals(eventName))
                                    {
                                        logger.Info("GetConsultReceivedPopupData : Reading CustomObject Popup Data");
                                        ICustomObject cstobject = SFDCCustomObject.GetInstance().GetCustomObjectChatPopupData(message, callType, key);
                                        if (cstobject != null)
                                        {
                                            this.tempCustomObject.Add(cstobject);
                                        }
                                    }
                                }
                                break;
                        }
                    }
                    if (this.tempCustomObject.Count > 0)
                    {
                        ICustomObject[] temp = this.tempCustomObject.ToArray();
                        sfdcData.CustomObjectData = temp;
                        this.tempCustomObject.Clear();
                    }
                }
            }
            catch (Exception generalException)
            {
                logger.Error("GetConsultReceivedPopupData : Error Occurred While Reading Consult received Popup Data : " + generalException.ToString());
            }
            return sfdcData;
        }

        #endregion Consult Received Popup Data

        #region Inbound Update Data

        /// <summary>
        /// Gets the Inbound Chat Update Data
        /// </summary>
        /// <param name="message"></param>
        /// <param name="callType"></param>
        /// <param name="callDuration"></param>
        /// <param name="chatContent"></param>
        /// <param name="eventName"></param>
        /// <returns></returns>
        public SFDCData GetInboundUpdateData(IMessage message, SFDCCallType callType, string callDuration, string chatContent, string eventName)
        {
            SFDCData sfdcData = new SFDCData();
            try
            {
                logger.Info("GetInboundUpdateData : Getting Inbound Update Data");
                if (this.PopupPages != null)
                {
                    foreach (string key in this.PopupPages)
                    {
                        switch (key)
                        {
                            case "lead":
                                if (this.leadOptions != null && this.leadOptions.InboundUpdateEvent != null && this.leadOptions.InboundUpdateEvent.Contains(eventName))
                                {
                                    logger.Info("GetInboundUpdateData : Reading Lead popup data");
                                    sfdcData.LeadData = SFDCLead.GetInstance().GetLeadChatUpdateData(message, callType, callDuration, chatContent);
                                }
                                break;

                            case "contact":
                                if (this.contactOptions != null && this.contactOptions.InboundUpdateEvent != null && this.contactOptions.InboundUpdateEvent.Contains(eventName))
                                {
                                    logger.Info("GetInboundUpdateData : Reading contact popup data");
                                    sfdcData.ContactData = SFDCContact.GetInstance().GetContactChatUpdateData(message, callType, callDuration, chatContent);
                                }
                                break;

                            case "account":
                                if (this.accountOptions != null && this.accountOptions.InboundUpdateEvent != null && this.accountOptions.InboundUpdateEvent.Contains(eventName))
                                {
                                    logger.Info("GetInboundUpdateData : Reading account popup data");
                                    sfdcData.AccountData = SFDCAccount.GetInstance().GetAccountChatUpdateData(message, callType, callDuration, chatContent);
                                }
                                break;

                            case "case":
                                if (this.caseOptions != null && this.caseOptions.InboundUpdateEvent != null && this.caseOptions.InboundUpdateEvent.Contains(eventName))
                                {
                                    logger.Info("GetInboundUpdateData : Reading case popup data");
                                    sfdcData.CaseData = SFDCCase.GetInstance().GetCaseChatUpdateData(message, callType, callDuration, chatContent);
                                }
                                break;

                            case "opportunity":
                                if (this.opportunityOptions != null && this.opportunityOptions.InboundUpdateEvent != null && this.opportunityOptions.InboundUpdateEvent.Contains(eventName))
                                {
                                    logger.Info("GetInboundUpdateData : Reading opportunity popup data");
                                    sfdcData.OpportunityData = SFDCOpportunity.GetInstance().GetOpportunityChatUpdateData(message, callType, callDuration, chatContent);
                                }
                                break;

                            case "useractivity":
                                if (this.userActivityOptions != null && this.userActivityOptions.InboundUpdateEvent != null &&
                    this.userActivityOptions.InboundUpdateEvent.Contains(eventName))
                                {
                                    logger.Info("GetInboundUpdateData : Reading useractivity popup data");
                                    sfdcData.UserActivityData = SFDCUserActivity.GetInstance().GetChatUpdateUserAcitivityData(message, callType, callDuration, chatContent);
                                }
                                break;

                            default:
                                if (this.customObjectOptions.ContainsKey(key))
                                {
                                    if (this.customObjectOptions[key] != null && this.customObjectOptions[key].InboundUpdateEvent != null &&
                                            this.customObjectOptions[key].InboundUpdateEvent.Contains(eventName))
                                    {
                                        ICustomObject cstobject = SFDCCustomObject.GetInstance().GetCustomObjectChatUpdateData(message, callType, callDuration, chatContent, key);
                                        if (cstobject != null)
                                        {
                                            logger.Info("GetInboundUpdateData : Reading customObject popup data");
                                            this.tempCustomObject.Add(cstobject);
                                        }
                                    }
                                }
                                break;
                        }
                    }
                    if (this.tempCustomObject.Count > 0)
                    {
                        ICustomObject[] temp = this.tempCustomObject.ToArray();
                        sfdcData.CustomObjectData = temp;
                        this.tempCustomObject.Clear();
                    }
                }
            }
            catch (Exception generalException)
            {
                logger.Error("GetInboundUpdateData : Error Occurred while Collecting Inbound Update Data : " + generalException.ToString());
            }
            return sfdcData;
        }

        #endregion Inbound Update Data

        #region Consult Update Data

        /// <summary>
        /// Gets the Chat Consult Update data
        /// </summary>
        /// <param name="message"></param>
        /// <param name="callType"></param>
        /// <param name="callDuration"></param>
        /// <param name="chatContent"></param>
        /// <param name="eventName"></param>
        /// <returns></returns>
        public SFDCData GetConsultUpdateData(IMessage message, SFDCCallType callType, string callDuration, string chatContent, string eventName)
        {
            SFDCData sfdcData = new SFDCData();
            try
            {
                logger.Info("GetConsultUpdateData : Getting lead Consult update data");
                if (this.PopupPages != null)
                {
                    foreach (string key in this.PopupPages)
                    {
                        switch (key)
                        {
                            case "lead":
                                if (this.leadOptions != null && this.leadOptions.ConsultUpdateEvent != null && this.leadOptions.ConsultUpdateEvent.Contains(eventName))
                                {
                                    logger.Info("GetConsultUpdateData : Reading lead popup data");
                                    sfdcData.LeadData = SFDCLead.GetInstance().GetLeadChatUpdateData(message, callType, callDuration, chatContent);
                                }
                                break;

                            case "contact":
                                if (this.contactOptions != null && this.contactOptions.ConsultUpdateEvent != null && this.contactOptions.ConsultUpdateEvent.Contains(eventName))
                                {
                                    logger.Info("GetConsultUpdateData : Reading contact popup data");
                                    sfdcData.ContactData = SFDCContact.GetInstance().GetContactChatUpdateData(message, callType, callDuration, chatContent);
                                }
                                break;

                            case "account":
                                if (this.accountOptions != null && this.accountOptions.ConsultUpdateEvent != null && this.accountOptions.ConsultUpdateEvent.Contains(eventName))
                                {
                                    logger.Info("GetConsultUpdateData : Reading account popup data");
                                    sfdcData.AccountData = SFDCAccount.GetInstance().GetAccountChatUpdateData(message, callType, callDuration, chatContent);
                                }
                                break;

                            case "case":
                                if (this.caseOptions != null && this.caseOptions.ConsultUpdateEvent != null && this.caseOptions.ConsultUpdateEvent.Contains(eventName))
                                {
                                    logger.Info("GetConsultUpdateData : Reading case popup data");
                                    sfdcData.CaseData = SFDCCase.GetInstance().GetCaseChatUpdateData(message, callType, callDuration, chatContent);
                                }
                                break;

                            case "opportunity":
                                if (this.opportunityOptions != null && this.opportunityOptions.ConsultUpdateEvent != null && this.opportunityOptions.ConsultUpdateEvent.Contains(eventName))
                                {
                                    logger.Info("GetConsultUpdateData : Reading opportunity popup data");
                                    sfdcData.OpportunityData = SFDCOpportunity.GetInstance().GetOpportunityChatUpdateData(message, callType, callDuration, chatContent);
                                }
                                break;

                            case "useractivity":
                                if (this.userActivityOptions != null && this.userActivityOptions.ConsultUpdateEvent != null &&
                    this.userActivityOptions.ConsultUpdateEvent.Contains(eventName))
                                {
                                    logger.Info("GetConsultUpdateData : Reading useractivity popup data");
                                    sfdcData.UserActivityData = SFDCUserActivity.GetInstance().GetChatUpdateUserAcitivityData(message, callType, callDuration, chatContent);
                                }
                                break;

                            default:
                                if (this.customObjectOptions.ContainsKey(key))
                                {
                                    if (this.customObjectOptions[key] != null && this.customObjectOptions[key].ConsultUpdateEvent != null &&
                                            this.customObjectOptions[key].ConsultUpdateEvent.Contains(eventName))
                                    {
                                        ICustomObject cstobject = SFDCCustomObject.GetInstance().GetCustomObjectChatUpdateData(message, callType, callDuration, chatContent, key);
                                        if (cstobject != null)
                                        {
                                            logger.Info("GetConsultUpdateData : Reading customObject popup data");
                                            this.tempCustomObject.Add(cstobject);
                                        }
                                    }
                                }
                                break;
                        }
                    }
                    if (this.tempCustomObject.Count > 0)
                    {
                        ICustomObject[] temp = this.tempCustomObject.ToArray();
                        sfdcData.CustomObjectData = temp;
                        this.tempCustomObject.Clear();
                    }
                }
            }
            catch (Exception generalException)
            {
                logger.Error("GetConsultUpdateData : Error Occurred while collecting Consult Update data : " + generalException.ToString());
            }
            return sfdcData;
        }

        #endregion Consult Update Data

        #region Send Update Log to SFDC

        /// <summary>
        /// Sends Update Log Data
        /// </summary>
        /// <param name="connId"></param>
        /// <param name="sfdcData"></param>
        public void SendUpdateLogData(string connId, SFDCData sfdcData)
        {
            try
            {
                logger.Info("SendUpdateLogData : Collects Update Log data");
                logger.Info("Connection ID : " + connId);

                if (Settings.UpdateSFDCLog.ContainsKey(connId))
                {
                    UpdateLogData updateData = Settings.UpdateSFDCLog[connId];
                    if (updateData != null)
                    {
                        if (sfdcData.LeadData != null)
                        {
                            logger.Info("SendUpdateLogData : Sending Lead Update Log data");
                            sfdcData.LeadData.ActivityRecordID = updateData.LeadActivityId;
                            sfdcData.LeadData.RecordID = updateData.LeadRecordId;
                        }
                        if (sfdcData.ContactData != null)
                        {
                            logger.Info("SendUpdateLogData : Sending Contact Update Log data");
                            sfdcData.ContactData.ActivityRecordID = updateData.ContactActivityId;
                            sfdcData.ContactData.RecordID = updateData.ContactRecordId;
                        }
                        if (sfdcData.AccountData != null)
                        {
                            logger.Info("SendUpdateLogData : Sending Account Update Log data");
                            sfdcData.AccountData.ActivityRecordID = updateData.AccountActivityId;
                            sfdcData.AccountData.RecordID = updateData.AccountRecordId;
                        }
                        if (sfdcData.CaseData != null)
                        {
                            logger.Info("SendUpdateLogData : Sending Case Update Log data");
                            sfdcData.CaseData.ActivityRecordID = updateData.CaseActivityId;
                            sfdcData.CaseData.RecordID = updateData.CaseRecordId;
                        }
                        if (sfdcData.OpportunityData != null)
                        {
                            logger.Info("SendUpdateLogData : Sending Opportunity Update Log data");
                            sfdcData.OpportunityData.ActivityRecordID = updateData.OpportunityActivityId;
                            sfdcData.OpportunityData.RecordID = updateData.OpportunityRecordId;
                        }
                        if (sfdcData.UserActivityData != null)
                        {
                            logger.Info("SendUpdateLogData : Sending UserActivity Update Log data");
                            sfdcData.UserActivityData.RecordID = updateData.UserActivityId;
                        }
                        if (sfdcData.CustomObjectData != null && updateData.CustomObject != null)
                        {
                            foreach (CustomObjectData custom in sfdcData.CustomObjectData)
                            {
                                if (updateData.CustomObject.ContainsKey(custom.ObjectName))
                                {
                                    KeyValueCollection collection = updateData.CustomObject[custom.ObjectName];
                                    if (collection != null)
                                    {
                                        logger.Info("SendUpdateLogData : Sending CustomObject Update Log data");
                                        if (collection.ContainsKey("newRecordId"))
                                            custom.RecordID = collection["newRecordId"].ToString();

                                        if (collection.ContainsKey("activityRecordId"))
                                            custom.ActivityRecordID = collection["activityRecordId"].ToString();
                                    }
                                }
                            }
                        }
                        SendPopupData(connId, sfdcData);
                    }
                }
                if (!Settings.UpdateSFDCLogFinishedEvent.ContainsKey(connId))
                {
                    Settings.UpdateSFDCLogFinishedEvent.Add(connId, sfdcData);
                }
                else
                {
                    Settings.UpdateSFDCLogFinishedEvent[connId] = sfdcData;
                }
            }
            catch (Exception generalException)
            {
                this.logger.Error("SendUpdateLogData : Error occurred while sending update Log :" + generalException.ToString());
            }
        }

        #endregion Send Update Log to SFDC

        #region SendPopupData

        /// <summary>
        /// Sends PopupData
        /// </summary>
        /// <param name="connId"></param>
        /// <param name="data"></param>
        public void SendPopupData(string connId, SFDCData data)
        {
            try
            {
                if (data != null)
                {
                    if (data.LeadData != null || data.ContactData != null || data.AccountData != null ||
                        data.CaseData != null || data.CustomObjectData != null || data.UserActivityData != null || data.OpportunityData != null)
                    {
                        if (!Settings.SFDCPopupData.ContainsKey(connId))
                        {
                            logger.Info("SFDC Popupdata Added with Connection ID");
                            //Settings.SFDCPopupData.Add(connId, data);
                        }
                        else
                        {
                            logger.Info("SFDC Popupdata Assigned to SFDCPopupData with connection id as key");
                            //Settings.SFDCPopupData[connId] = data;
                        }
                    }
                }
            }
            catch (Exception generalException)
            {
                this.logger.Error("SendPopupData : Error occurred while sending update Log :" + generalException.ToString());
            }
        }

        #endregion SendPopupData
    }
}