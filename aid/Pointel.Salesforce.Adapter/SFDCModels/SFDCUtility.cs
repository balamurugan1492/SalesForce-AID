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
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml;

    using Genesyslab.Platform.Commons.Collections;
    using Genesyslab.Platform.Commons.Protocols;

    using Pointel.Salesforce.Adapter.Configurations;
    using Pointel.Salesforce.Adapter.LogMessage;
    using Pointel.Salesforce.Adapter.PForce;
    using Pointel.Salesforce.Adapter.SFDCUtils;
    using Pointel.Salesforce.Adapter.Utility;
    using Pointel.Salesforce.Adapter.Voice;

    internal class SFDCUtility
    {
        #region Fields

        public static PForce.SforceService SForce = null;

        public List<Tuple<string, List<XmlElement>, string, string, int>> SessionCreateActivityCollection = new List<Tuple<string, List<XmlElement>, string, string, int>>();
        public List<Tuple<string, SFDCCallType, SFDCData>> SessionSearchCollection = new List<Tuple<string, SFDCCallType, SFDCData>>();
        public List<Tuple<string, List<XmlElement>, string, string, int>> SessionUpdateActivityCollection = new List<Tuple<string, List<XmlElement>, string, string, int>>();

        private static SFDCUtility currentObject = null;

        private string ConsultInitTime;
        private string ConsultInteractionId;
        private XmlElement element = null;
        private bool enableTimeStamp = false;
        private GeneralOptions generalOptions;
        private Log logger = null;
        private bool updatefield = false;
        private int _sessionRetrycount = 0;

        #endregion Fields

        #region Constructors

        private SFDCUtility()
        {
            try
            {
                this.logger = Log.GenInstance();
                this.generalOptions = Settings.SFDCOptions;
                SForce = new SforceService();
                SForce.SessionHeaderValue = new SessionHeader();
                SForce.Url = Settings.SFDCOptions.PartnerServiceAPIUrl;
                if (Settings.SFDCOptions.SOAPAPICallTimeout > 0)
                    SForce.Timeout = Settings.SFDCOptions.SOAPAPICallTimeout;
            }
            catch
            {
            }
        }

        #endregion Constructors

        #region Methods

        public static SFDCUtility GetInstance()
        {
            if (currentObject == null)
            {
                currentObject = new SFDCUtility();
            }
            return currentObject;
        }

        public string CreateActivityLog(string interactionId, List<XmlElement> taskFields, string objectName, string recordId = null, int retryAttempt = 0)
        {
            try
            {
                bool isUserlevel = false;
                if (string.IsNullOrWhiteSpace(recordId))
                    isUserlevel = true;
                if (isUserlevel)
                    this.logger.Info("CreateActivityLog: Create user level Activity Log Invoked for the Object :" + objectName + " Interaction Id :" + interactionId);
                else
                    this.logger.Info("CreateActivityLog: Create Activity Log Invoked for the Object :" + objectName + " record id : " + recordId + " and  Interaction Id :" +
                        interactionId);
                try
                {
                    if (taskFields != null)
                    {
                        //removed if element already there for session
                        XmlElement[] elements = taskFields.ToArray();
                        foreach (var item in elements)
                        {
                            if (item.Name == "WhoId")
                            {
                                taskFields.Remove(item);
                            }
                            if (item.Name == "WhatId")
                            {
                                taskFields.Remove(item);
                            }
                        }
                        XmlDocument document = new XmlDocument();

                        if (!string.IsNullOrWhiteSpace(recordId))
                        {
                            switch (objectName)
                            {
                                case "Lead":
                                case "Contact":

                                    XmlElement element = document.CreateElement("WhoId");
                                    element.InnerText = recordId;
                                    taskFields.Add(element);

                                    break;

                                default:
                                    XmlElement elementnew = document.CreateElement("WhatId");
                                    elementnew.InnerText = recordId;
                                    taskFields.Add(elementnew);
                                    break;
                            }
                        }
                        this.logger.Info("Activity Log for the object : " + objectName);
                        string log = "\n *********************************** \n";
                        for (int i = 0; i < taskFields.Count; i++)
                        {
                            if (taskFields[i] != null)
                                log += "\tField Name : " + taskFields[i].Name + "\t Field Value : " + taskFields[i].InnerText + "\n";
                        }
                        log += "************************************";
                        this.logger.Info(log);
                    }
                }
                catch
                {
                }

                if (taskFields != null)
                {
                    XmlElement[] logData = taskFields.ToArray();
                    sObject sobject = new sObject();
                    sobject.type = "Task";
                    sobject.Any = logData;
                    PForce.sObject[] array = new sObject[1];
                    array[0] = sobject;
                    SaveResult[] saveResult = null;
                    this.logger.Info("************Request from AID***********************");
                    if (isUserlevel)
                        this.logger.Info("Request sent to salesforce to create user level activity log for the Object : " + objectName);
                    else
                        this.logger.Info("Request sent to salesforce to create activity log for the Object : " + objectName + " and the record id : "
                                        + recordId);

                    this.logger.Info("************Request from AID***********************");
                    saveResult = SForce.create(array);
                    if (!saveResult[0].success)
                    {
                        if (Settings.SFDCOptions.SOAPAPIErrorMessageDisplay)
                        {
                            if (isUserlevel)
                                Settings.SFDCListener.SFDCConnectionStatus(LogMode.Error, "Error Returned while Creating user level activity log \n Error Message : " + saveResult[0].errors[0].message);
                            else
                                Settings.SFDCListener.SFDCConnectionStatus(LogMode.Error, "Error Returned while Creating activity log for the record id :"
                                    + recordId + "\n Error Message : " + saveResult[0].errors[0].message);
                        }
                        this.logger.Info("************Service Response***********************");
                        if (isUserlevel)
                            this.logger.Error("CreateActivityLog: Error Returned while Creating new user level activity log, Error Message : " + saveResult[0].errors[0].message);
                        else
                            this.logger.Error("CreateActivityLog: Error Returned while Creating new activity log for the record id  :"
                                + recordId + ", Error Message : " + saveResult[0].errors[0].message);
                        this.logger.Error("*********************************************");
                        return null;
                    }
                    else
                    {
                        this.logger.Info("************Service Response***********************");
                        if (isUserlevel)
                            this.logger.Info("Response from salesforce for creating user level activitylog for the Type :" + objectName);
                        else
                            this.logger.Info("Response from salesforce for creating activitylog for the Type :" + objectName
                                + " and the record Id :" + recordId);
                        this.logger.Info("Response Id :" + saveResult[0].id);
                        this.logger.Info("*********************************************");
                        GetUpdateResponseData(interactionId, objectName, saveResult[0].id);
                        if (Settings.SFDCOptions.CanEnableRetrySearchSessionID)
                        {
                            var remvoeTuple = SessionCreateActivityCollection.Find(x => x.Item1 == interactionId);
                            if (remvoeTuple != null)
                            {
                                this.logger.Info("CreateActivityLog : Removed from collection for create activity retry, Interactionid : " + interactionId);
                                this.logger.Info("CreateActivityLog : SessionCreateActivityCollection count before remove" + SessionCreateActivityCollection.Count);
                                SessionCreateActivityCollection.Remove(remvoeTuple);
                                this.logger.Info("CreateActivityLog : SessionCreateActivityCollection count after remove" + SessionCreateActivityCollection.Count);
                            }
                            else
                                this.logger.Info("CreateActivityLog : SessionCreateActivityCollection find returns null");
                        }
                        else
                            this.logger.Info("CreateActivityLog : CanEnableRetrySearchSessionID is false cannot remove details from collection");

                        return saveResult[0].id;
                    }
                }
                else
                {
                    this.logger.Error("CreateActivityLog: Can not create activity log because log Data is null ...");
                }
            }
            catch (WebException webException)
            {
                if (webException.Status == WebExceptionStatus.Timeout)
                {
                    //Retry on time out error.
                    if (!string.IsNullOrWhiteSpace(recordId))
                        this.logger.Error("CreateActivityLog: Timeout Error Occurred while creating activity log for the record id " + recordId
                            + " :" + webException.Message);
                    else
                        this.logger.Error("CreateActivityLog: Timeout Error Occurred while creating user level activity log :" + webException.Message);
                    retryAttempt += 1;
                    if (retryAttempt <= Settings.SFDCOptions.SOAPTimeoutRetryAttempt)
                    {
                        this.logger.Info("CreateActivityLog : Adapter Executing CreateActivityLog method again due to SOAP Time out error");
                        this.logger.Info("CreateActivityLog : Timeout attempt = " + retryAttempt);
                        this.logger.Info("CreateActivityLog : Configured Max Timeout Attempt = " + Settings.SFDCOptions.SOAPTimeoutRetryAttempt);
                        Task.Factory.StartNew(() => CreateActivityLog(interactionId, taskFields, objectName, recordId, retryAttempt));
                    }
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(recordId))
                        this.logger.Error("CreateActivityLog: Error Occurred while creating activity log for the record id  " + recordId + " :" +
                            webException.Message);
                    else
                        this.logger.Error("CreateActivityLog: Error Occurred while creating user level activity log :" + webException.Message);
                }
                if (Settings.SFDCOptions.SOAPAPIErrorMessageDisplay)
                {
                    if (!string.IsNullOrWhiteSpace(recordId))
                        Settings.SFDCListener.SFDCConnectionStatus(LogMode.Error, "Error Returned while Creating activity log for the record id :" +
                            recordId + "\nMessage : " + webException.Message);
                    else
                        Settings.SFDCListener.SFDCConnectionStatus(LogMode.Error, "Error Returned while Creating user level activity log \nMessage : " + webException.Message);
                }
            }
            catch (Exception generalException)
            {
                if (!string.IsNullOrWhiteSpace(recordId))
                    this.logger.Error("CreateActivityLog: Error Occurred while creating activity log for the record id  " + recordId + " :" +
                        generalException.Message);
                else
                    this.logger.Error("CreateActivityLog: Error Occurred while creating user level activity log :" +
                   generalException.Message);

                if (generalException.Message.Contains("INVALID_SESSION_ID"))
                {
                    if (Settings.SFDCOptions.CanEnableSessionIDInLog)
                    {
                        logger.Info("Create activity Failed session id is:" + SForce.SessionHeaderValue.sessionId);
                    }
                    Settings.SFDCListener.SendSessionStatus(SFDCSessionStatus.NotConnected);
                    if (Settings.SFDCOptions.CanEnableRetrySearchSessionID)
                    {
                        var isAnyItemExisit = SessionCreateActivityCollection.Any(x => x.Item1 == interactionId);
                        if (!isAnyItemExisit)
                        {
                            this.logger.Info("CreateActivityLog : Added activity details in collection for create retry");
                            this.logger.Info("CreateActivityLog : SessionCreateActivityCollection before count" + SessionCreateActivityCollection.Count);
                            SessionCreateActivityCollection.Add(new Tuple<string, List<XmlElement>, string, string, int>(interactionId, taskFields, objectName, recordId, retryAttempt));
                            this.logger.Info("CreateActivityLog : SessionCreateActivityCollection after count" + SessionCreateActivityCollection.Count);
                        }
                        else
                            this.logger.Info("CreateActivityLog : Added activity details in collection for create retry");
                        SFDCHttpServer.sessionFlag = true;
                        SFDCHttpServer.NewSessionIDFlag = true;
                    }
                }
                if (Settings.SFDCOptions.SOAPAPIErrorMessageDisplay)
                {
                    if (!string.IsNullOrWhiteSpace(recordId))
                        Settings.SFDCListener.SFDCConnectionStatus(LogMode.Error, "Error Returned while Creating activity log for the Type :" +
                            objectName + "\n Error Message : " + generalException.Message);
                    else
                        Settings.SFDCListener.SFDCConnectionStatus(LogMode.Error, "Error Returned while Creating user level activity log for the Type :" +
                            objectName + "\n Error Message : " + generalException.Message);
                }
            }
            return null;
        }

        public string CreateNewRecord(string interactionId, System.Xml.XmlElement[] taskFields, string objectName, int retryAttempt = 0)
        {
            try
            {
                this.logger.Info("CreateNewRecord: Creating New Record Invoked for the Object :" + objectName + " for the Interaction Id :" +
                    interactionId);
                try
                {
                    if (taskFields != null)
                    {
                        this.logger.Info("Record Data for the object : " + objectName);
                        string log = "\n *********************************** \n ";
                        for (int i = 0; i < taskFields.Length; i++)
                        {
                            if (taskFields[i] != null)
                                log += "\tField Name : " + taskFields[i].Name + "\t Field Value : " + taskFields[i].InnerText + "\n";
                        }
                        log += "***********************************";
                        this.logger.Info(log);
                    }
                }
                catch
                {
                }

                if (taskFields != null)
                {
                    sObject sobject = new sObject();
                    sobject.type = objectName;
                    sobject.Any = taskFields;
                    PForce.sObject[] array = new sObject[1];
                    array[0] = sobject;
                    SaveResult[] saveResult = null;
                    this.logger.Info("************Request from AID***********************");
                    this.logger.Info("Request sent to salesforce to create New Record for the Object : " + objectName);
                    this.logger.Info("************Request from AID***********************");
                    saveResult = SForce.create(array);
                    if (!saveResult[0].success)
                    {
                        if (Settings.SFDCOptions.SOAPAPIErrorMessageDisplay)
                        {
                            Settings.SFDCListener.SFDCConnectionStatus(LogMode.Error, "Error Returned while Creating New Record for the Type :" + objectName + "\nMessage : " + saveResult[0].errors[0].message);
                        }
                        this.logger.Info("************Service Response***********************");
                        this.logger.Error("CreateNewRecord: Error Returned while Creating New Record for the Type :" + objectName + ", Message : " + saveResult[0].errors[0].message);
                        this.logger.Error("*********************************************");
                    }
                    else
                    {
                        this.logger.Info("************Service Response***********************");
                        this.logger.Info("Response from salesforce for creating new Record for the Type :" + objectName
                            + " and the Interaction Id :" + interactionId);
                        this.logger.Info("Response Id :" + saveResult[0].id);
                        this.logger.Info("*********************************************");
                        UpdateLogData updateLogData = new UpdateLogData();
                        switch (objectName.ToLower())
                        {
                            case "lead":
                                updateLogData = new UpdateLogData();
                                if (!Settings.UpdateSFDCLog.ContainsKey(interactionId))
                                {
                                    updateLogData.LeadRecordId = saveResult[0].id;
                                    Settings.UpdateSFDCLog.Add(interactionId, updateLogData);
                                }
                                else
                                {
                                    updateLogData = Settings.UpdateSFDCLog[interactionId];
                                    updateLogData.LeadRecordId = saveResult[0].id;
                                    Settings.UpdateSFDCLog[interactionId] = updateLogData;
                                }
                                break;

                            case "contact":
                                updateLogData = new UpdateLogData();
                                if (!Settings.UpdateSFDCLog.ContainsKey(interactionId))
                                {
                                    updateLogData.ContactRecordId = saveResult[0].id;
                                    Settings.UpdateSFDCLog.Add(interactionId, updateLogData);
                                }
                                else
                                {
                                    updateLogData = Settings.UpdateSFDCLog[interactionId];
                                    updateLogData.ContactRecordId = saveResult[0].id;
                                    Settings.UpdateSFDCLog[interactionId] = updateLogData;
                                }
                                break;

                            case "case":
                                updateLogData = new UpdateLogData();
                                if (!Settings.UpdateSFDCLog.ContainsKey(interactionId))
                                {
                                    updateLogData.CaseRecordId = saveResult[0].id;
                                    Settings.UpdateSFDCLog.Add(interactionId, updateLogData);
                                }
                                else
                                {
                                    updateLogData = Settings.UpdateSFDCLog[interactionId];
                                    updateLogData.CaseRecordId = saveResult[0].id;
                                    Settings.UpdateSFDCLog[interactionId] = updateLogData;
                                }
                                break;

                            case "account":
                                updateLogData = new UpdateLogData();
                                if (!Settings.UpdateSFDCLog.ContainsKey(interactionId))
                                {
                                    updateLogData.AccountRecordId = saveResult[0].id;
                                    Settings.UpdateSFDCLog.Add(interactionId, updateLogData);
                                }
                                else
                                {
                                    updateLogData = Settings.UpdateSFDCLog[interactionId];
                                    updateLogData.AccountRecordId = saveResult[0].id;
                                    Settings.UpdateSFDCLog[interactionId] = updateLogData;
                                }
                                break;

                            case "opportunity":
                                updateLogData = new UpdateLogData();
                                if (!Settings.UpdateSFDCLog.ContainsKey(interactionId))
                                {
                                    updateLogData.OpportunityRecordId = saveResult[0].id;
                                    Settings.UpdateSFDCLog.Add(interactionId, updateLogData);
                                }
                                else
                                {
                                    updateLogData = Settings.UpdateSFDCLog[interactionId];
                                    updateLogData.OpportunityRecordId = saveResult[0].id;
                                    Settings.UpdateSFDCLog[interactionId] = updateLogData;
                                }
                                break;

                            default:
                                updateLogData = new UpdateLogData();
                                if (!Settings.UpdateSFDCLog.ContainsKey(interactionId))
                                {
                                    updateLogData.CustomObject = new Dictionary<string, KeyValueCollection>();
                                    KeyValueCollection collection = new KeyValueCollection();
                                    collection.Add("newRecordId", saveResult[0].id);
                                    updateLogData.CustomObject.Add(objectName, collection);
                                    Settings.UpdateSFDCLog.Add(interactionId, updateLogData);
                                }
                                else
                                {
                                    updateLogData = Settings.UpdateSFDCLog[interactionId];
                                    if (updateLogData != null)
                                    {
                                        if (updateLogData.CustomObject != null)
                                        {
                                            if (updateLogData.CustomObject.ContainsKey(objectName))
                                            {
                                                if (!updateLogData.CustomObject[objectName].ContainsKey("newRecordId"))
                                                    updateLogData.CustomObject[objectName].Add("newRecordId", saveResult[0].id);
                                                else
                                                    updateLogData.CustomObject[objectName]["newRecordId"] = saveResult[0].id;
                                            }
                                            else
                                            {
                                                KeyValueCollection collection = new KeyValueCollection();
                                                collection.Add("newRecordId", saveResult[0].id);
                                                updateLogData.CustomObject.Add(objectName, collection);
                                            }
                                        }
                                        else
                                        {
                                            updateLogData.CustomObject = new Dictionary<string, KeyValueCollection>();
                                            KeyValueCollection collection = new KeyValueCollection();
                                            collection.Add("newRecordId", saveResult[0].id);
                                            updateLogData.CustomObject.Add(objectName, collection);
                                        }
                                    }
                                }

                                break;
                        }
                        return saveResult[0].id;
                    }
                }
                else
                {
                    this.logger.Error("CreateNewRecord: Can not create record because New Record Data is null ...");
                }
            }
            catch (WebException webException)
            {
                if (webException.Status == WebExceptionStatus.Timeout)
                {
                    //Retry on time out error.
                    this.logger.Error("CreateNewRecord: Timeout Error Occurred while creating new Record for the type " + objectName + " :" + webException.Message);
                    retryAttempt += 1;
                    if (retryAttempt <= Settings.SFDCOptions.SOAPTimeoutRetryAttempt)
                    {
                        this.logger.Info("CreateNewRecord : Adapter Executing CreateNewRecord method again due to SOAP Time out error");
                        this.logger.Info("CreateNewRecord : Timeout attempt = " + retryAttempt);
                        this.logger.Info("CreateNewRecord : Configured Max Timeout Attempt = " + Settings.SFDCOptions.SOAPTimeoutRetryAttempt);
                        Task.Factory.StartNew(() => CreateNewRecord(interactionId, taskFields, objectName, retryAttempt));
                    }
                }
                else
                {
                    this.logger.Error("CreateNewRecord: Error Occurred while creating new Record for the type " + objectName + " :" + webException.Message);
                }
                if (Settings.SFDCOptions.SOAPAPIErrorMessageDisplay)
                {
                    Settings.SFDCListener.SFDCConnectionStatus(LogMode.Error, "Error Returned while Creating New Record for the Type :" + objectName + "\nMessage : " + webException.Message);
                }
            }
            catch (Exception generalException)
            {
                this.logger.Error("CreateNewRecord: Error Occurred while creating new Record for the type " + objectName + " :" + generalException.Message);

                if (generalException.Message.Contains("INVALID_SESSION_ID"))
                {
                    Settings.SFDCListener.SendSessionStatus(SFDCSessionStatus.NotConnected);
                }
                if (Settings.SFDCOptions.SOAPAPIErrorMessageDisplay)
                {
                    Settings.SFDCListener.SFDCConnectionStatus(LogMode.Error, "Error Returned while Creating New Record for the Type :" + objectName + "\nMessage : " + generalException.Message);
                }
            }
            return null;
        }

        public string GetAttributeSearchValues(IMessage message, string[] attributeSearchKeys)
        {
            try
            {
                if (attributeSearchKeys != null && message != null)
                {
                    string searchValues = string.Empty;
                    this.logger.Info("GetAttributeSearchValues: Reading Attribute Search Values, Keys : " + string.Join(",", attributeSearchKeys));
                    dynamic voiceEvent = Convert.ChangeType(message, message.GetType());
                    if (voiceEvent != null)
                    {
                        #region EventRinging Attribute data

                        foreach (string key in attributeSearchKeys)
                        {
                            this.logger.Info("Reading attribute search data for the key : " + key);
                            switch (key)
                            {
                                case "ani":

                                    if (!String.IsNullOrEmpty(voiceEvent.ANI))
                                    {
                                        if (voiceEvent.ANI.Length > 10)
                                        {
                                            searchValues += TruncateNumbers(voiceEvent.ANI, 10) + ",";
                                        }
                                        else
                                            searchValues += voiceEvent.ANI + ",";
                                    }
                                    else
                                        searchValues += "^,";

                                    break;

                                case "thisdn":
                                    if (!String.IsNullOrEmpty(voiceEvent.ThisDN))
                                    {
                                        if (voiceEvent.ThisDN.Length > 10)
                                        {
                                            searchValues += TruncateNumbers(voiceEvent.ThisDN, 10) + ",";
                                        }
                                        else
                                            searchValues += voiceEvent.ThisDN + ",";
                                    }
                                    else
                                        searchValues += "^,";
                                    break;

                                case "otherdn":
                                    if (!String.IsNullOrEmpty(voiceEvent.OtherDN))
                                    {
                                        if (voiceEvent.OtherDN.Length > 10)
                                        {
                                            searchValues += TruncateNumbers(voiceEvent.OtherDN, 10) + ",";
                                        }
                                        else
                                            searchValues += voiceEvent.OtherDN + ",";
                                    }
                                    else
                                        searchValues += "^,";
                                    break;

                                case "connid":
                                    if (!String.IsNullOrEmpty(voiceEvent.ConnID.ToString()))
                                        searchValues += voiceEvent.ConnID.ToString() + ",";
                                    else
                                        searchValues += "^,";
                                    break;

                                case "agentid":
                                    if (!String.IsNullOrEmpty(voiceEvent.AgentID))
                                        searchValues += voiceEvent.AgentID + ",";
                                    else
                                        searchValues += "^,";
                                    break;

                                case "calluuid":
                                    if (!String.IsNullOrEmpty(voiceEvent.CallUuid))
                                        searchValues += voiceEvent.CallUuid + ",";
                                    else
                                        searchValues += "^,";
                                    break;

                                case "dnis":
                                    if (!String.IsNullOrEmpty(voiceEvent.DNIS))
                                    {
                                        if (voiceEvent.DNIS.Length > 10)
                                        {
                                            searchValues += TruncateNumbers(voiceEvent.DNIS, 10) + ",";
                                        }
                                        else
                                            searchValues += voiceEvent.DNIS + ",";
                                    }
                                    else
                                        searchValues += "^,";
                                    break;

                                default:
                                    this.logger.Info("No attribute search data found for the key : " + key);
                                    break;
                            }
                        }

                        #endregion EventRinging Attribute data
                    }
                    return (searchValues != string.Empty) ? searchValues.Substring(0, searchValues.Length - 1) : searchValues;
                }
                else
                {
                    this.logger.Info("GetAttributeSearchValues: Attribute search keys are empty");
                }
            }
            catch (Exception generalException)
            {
                this.logger.Error("GetSearchAttributeValue: Error occurred  while getting Attribute Search values : " + generalException.ToString());
            }
            return string.Empty;
        }

        public string GetChatActivityLog(KeyValueCollection activityLog, IMessage Message, SFDCCallType callType)
        {
            try
            {
                this.logger.Error("GetChatActivityLog : Collecting Activity Logs for the callType : " + callType.ToString());

                if (activityLog != null)
                {
                    string ActivityHistory = "&";
                    string keyPrefix = string.Empty;
                    dynamic interactionEvent = Convert.ChangeType(Message, Message.GetType());

                    #region EventType

                    if (callType == SFDCCallType.InboundChat || callType == SFDCCallType.ConsultChatReceived)
                    {
                        keyPrefix = "inb";
                    }

                    #endregion EventType

                    foreach (KeyValueCollection temp1 in activityLog.AllValues)
                    {
                        string value = string.Empty;
                        updatefield = false;
                        enableTimeStamp = false;
                        string fieldName = temp1.GetAsString("field-name");
                        string fieldType = temp1.GetAsString("field-type");
                        if (!String.IsNullOrEmpty(temp1.GetAsString("enable.update")) && temp1.GetAsString("enable.update").Trim().ToLower() == "true")
                        {
                            updatefield = true;
                        }
                        if (!String.IsNullOrEmpty(temp1.GetAsString("enable.time-stamp")) && temp1.GetAsString("enable.time-stamp").Trim().ToLower() == "true")
                        {
                            enableTimeStamp = true;
                        }
                        if (!String.IsNullOrEmpty(fieldName) && !updatefield)
                        {
                            if (fieldName.Equals("RecordTypeId"))
                            {
                                if (!String.IsNullOrEmpty(temp1.GetAsString("record-type.id")))
                                {
                                    fieldName = fieldName.Trim() + "=";
                                    value += temp1.GetAsString("record-type.id");
                                }
                            }
                            else
                            {
                                fieldName = fieldName.Trim() + "=";
                                if (!String.IsNullOrEmpty(fieldType))
                                { fieldType = fieldType.Trim().ToLower(); }
                                else
                                {
                                    fieldType = "datetime";
                                    enableTimeStamp = true;
                                }
                                if (fieldType == "text" || fieldType == "number")
                                {
                                    if (!String.IsNullOrEmpty(temp1.GetAsString(keyPrefix + ".user-data.key-name")) && interactionEvent.Interaction.InteractionUserData != null)
                                    {
                                        value += interactionEvent.Interaction.InteractionUserData.GetAsString(temp1.GetAsString(keyPrefix + ".user-data.key-name"));
                                    }
                                    else if (!String.IsNullOrEmpty(temp1.GetAsString(keyPrefix + ".attrib.key-name")))
                                    {
                                        value += GetAttributeValueForLog(temp1, interactionEvent, keyPrefix, callType);
                                    }
                                    else if (!String.IsNullOrEmpty(temp1.GetAsString(keyPrefix + ".default.value")))
                                    {
                                        value += temp1.GetAsString(keyPrefix + ".default.value");
                                    }
                                    if (enableTimeStamp && fieldType != "number")
                                    {
                                        value += System.DateTime.Now.ToString();
                                    }
                                }
                                else if (enableTimeStamp)
                                {
                                    if (fieldType == "date")
                                        value += System.DateTime.Now.ToString("yyyy-MM-dd");
                                    else if (fieldType == "time")
                                        value += System.DateTime.Now.ToShortTimeString();
                                    else if (fieldType == "datetime")
                                        value += System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                }
                                if (fieldType == "number")
                                {
                                    if (value == string.Empty || value == null)
                                    {
                                        value += "0";
                                    }
                                }
                            }
                        }
                        if (fieldName != string.Empty)
                        {
                            ActivityHistory += fieldName + value + "&";
                        }
                    }
                    return ActivityHistory.Substring(0, ActivityHistory.Length - 1);
                }
            }
            catch (Exception generalException)
            {
                this.logger.Error("GetActivityLog : Error Occurred while Collecting activity Logs : " + generalException.ToString());
            }

            return null;
        }

        public XmlElement[] GetChatRecordData(KeyValueCollection activityLog, IMessage Message, SFDCCallType callType)
        {
            try
            {
                this.logger.Info("GetChatRecordData : Collecting Record Data for the callType : " + callType.ToString());

                if (activityLog != null)
                {
                    string keyPrefix = string.Empty;
                    dynamic interactionEvent = Convert.ChangeType(Message, Message.GetType());
                    int index = 0;
                    System.Xml.XmlElement[] Fields = new System.Xml.XmlElement[20];
                    System.Xml.XmlDocument doc = new System.Xml.XmlDocument();

                    #region EventType

                    if (callType == SFDCCallType.InboundChat || callType == SFDCCallType.ConsultChatReceived)
                    {
                        keyPrefix = "inb";
                    }

                    #endregion EventType

                    foreach (KeyValueCollection temp1 in activityLog.AllValues)
                    {
                        string value = string.Empty;
                        updatefield = false;
                        enableTimeStamp = false;
                        string fieldName = temp1.GetAsString("field-name");
                        string fieldType = temp1.GetAsString("field-type");
                        if (!String.IsNullOrEmpty(temp1.GetAsString("enable.update")) && temp1.GetAsString("enable.update").Trim().ToLower() == "true")
                        {
                            updatefield = true;
                        }
                        if (!String.IsNullOrEmpty(temp1.GetAsString("enable.time-stamp")) && temp1.GetAsString("enable.time-stamp").Trim().ToLower() == "true")
                        {
                            enableTimeStamp = true;
                        }
                        if (!String.IsNullOrEmpty(fieldName) && !updatefield)
                        {
                            if (fieldName.Equals("RecordTypeId"))
                            {
                                if (!String.IsNullOrEmpty(temp1.GetAsString("record-type.id")))
                                {
                                    fieldName = fieldName.Trim();
                                    value += temp1.GetAsString("record-type.id");
                                }
                            }
                            else
                            {
                                fieldName = fieldName.Trim();
                                if (!String.IsNullOrEmpty(fieldType))
                                {
                                    fieldType = fieldType.Trim().ToLower();
                                }
                                else
                                {
                                    fieldType = "datetime";
                                    enableTimeStamp = true;
                                }
                                if (fieldType == "text" || fieldType == "number")
                                {
                                    if (!String.IsNullOrEmpty(temp1.GetAsString(keyPrefix + ".user-data.key-name")) && interactionEvent.UserData != null)
                                    {
                                        value += interactionEvent.UserData.GetAsString(temp1.GetAsString(keyPrefix + ".user-data.key-name"));
                                    }
                                    else if (!String.IsNullOrEmpty(temp1.GetAsString(keyPrefix + ".attrib.key-name")))
                                    {
                                        value += GetAttributeValueForLog(temp1, interactionEvent, keyPrefix, callType);
                                    }
                                    else if (!String.IsNullOrEmpty(temp1.GetAsString(keyPrefix + ".default.value")))
                                    {
                                        value += temp1.GetAsString(keyPrefix + ".default.value");
                                    }
                                    if (enableTimeStamp && fieldType != "number")
                                    {
                                        value += System.DateTime.Now.ToString();
                                    }
                                }
                                else if (enableTimeStamp)
                                {
                                    if (fieldType == "date")
                                        value += System.DateTime.Now.ToString("yyyy-MM-dd");
                                    else if (fieldType == "time")
                                        value += System.DateTime.Now.ToShortTimeString();
                                    else if (fieldType == "datetime")
                                        value += System.DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
                                }
                                if (fieldType == "number")
                                {
                                    if (value == string.Empty || value == null)
                                    {
                                        value += "0";
                                    }
                                }
                            }
                        }
                        if (fieldName != string.Empty && value != string.Empty)
                        {
                            Fields[index] = doc.CreateElement(Convert.ToString(fieldName));
                            Fields[index].InnerText = Convert.ToString(value);
                            index++;
                        }
                        else
                        {
                            this.logger.Info("field name or field value is empty , field Name : " + fieldName + ", field value : " + value);
                        }
                    }
                    return Fields;
                }
            }
            catch (Exception generalException)
            {
                this.logger.Error("GetChatRecordData : Error Occurred while Collecting Recors data : " + generalException.ToString());
            }

            return null;
        }

        public string GetChatUpdateActivityLog(KeyValueCollection activityLog, IMessage Message, SFDCCallType callType, string duration, string chatContent)
        {
            try
            {
                this.logger.Info("GetUpdateActivityLog : Collection update log for the callType : " + callType.ToString());
                if (activityLog != null)
                {
                    string ActivityHistory = "&";
                    string keyPrefix = string.Empty;
                    dynamic chatEvents = Convert.ChangeType(Message, Message.GetType());

                    #region EventType

                    if (callType == SFDCCallType.InboundChat || callType == SFDCCallType.ConsultChatReceived)
                    {
                        keyPrefix = "inb";
                    }

                    #endregion EventType

                    foreach (KeyValueCollection temp1 in activityLog.AllValues)
                    {
                        string value = string.Empty;
                        updatefield = false;
                        enableTimeStamp = false;
                        string fieldName = temp1.GetAsString("field-name");
                        string fieldType = temp1.GetAsString("field-type");
                        if (!String.IsNullOrEmpty(temp1.GetAsString("enable.update")) && temp1.GetAsString("enable.update").Trim().ToLower() == "true")
                        {
                            updatefield = true;
                        }
                        if (!String.IsNullOrEmpty(temp1.GetAsString("enable.time-stamp")) && temp1.GetAsString("enable.time-stamp").Trim().ToLower() == "true")
                        {
                            enableTimeStamp = true;
                        }
                        if (!String.IsNullOrEmpty(fieldName) && updatefield)
                        {
                            if (fieldName.Equals("RecordTypeId"))
                            {
                                if (!String.IsNullOrEmpty(temp1.GetAsString("record-type.id")))
                                {
                                    fieldName = fieldName.Trim() + "=";
                                    value += temp1.GetAsString("record-type.id");
                                }
                            }
                            else
                            {
                                fieldName = fieldName.Trim() + "=";
                                if (!String.IsNullOrEmpty(fieldType))
                                { fieldType = fieldType.Trim().ToLower(); }
                                else
                                {
                                    fieldType = "datetime";
                                    enableTimeStamp = true;
                                }
                                if (fieldType == "text" || fieldType == "number")
                                {
                                    if (!String.IsNullOrEmpty(temp1.GetAsString(keyPrefix + ".user-data.key-name")) && chatEvents.Interaction.InteractionUserData != null)
                                    {
                                        value += chatEvents.Interaction.InteractionUserData.GetAsString(temp1.GetAsString(keyPrefix + ".user-data.key-name"));
                                    }
                                    else if (!String.IsNullOrEmpty(temp1.GetAsString(keyPrefix + ".attrib.key-name")))
                                    {
                                        value += GetAttributeValueForLog(temp1, chatEvents, keyPrefix, callType);
                                    }
                                    else if (!String.IsNullOrEmpty(temp1.GetAsString(keyPrefix + ".default.value")))
                                    {
                                        value += temp1.GetAsString(keyPrefix + ".default.value");
                                    }
                                    if (enableTimeStamp && fieldType != "number")
                                    {
                                        value += System.DateTime.Now.ToString();
                                    }
                                }
                                else if (fieldType == "duration")
                                {
                                    value += duration;
                                }
                                else if (fieldType == "chatcontent")
                                {
                                    value += chatContent;
                                }
                                else if (enableTimeStamp)
                                {
                                    if (fieldType == "date")
                                        value += System.DateTime.Now.ToString("yyyy-MM-dd");
                                    else if (fieldType == "time")
                                        value += System.DateTime.Now.ToShortTimeString();
                                    else if (fieldType == "datetime")
                                        value += System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                }
                                if (fieldType == "number")
                                {
                                    if (value == string.Empty || value == null)
                                    {
                                        value += "0";
                                    }
                                }
                            }
                        }
                        if (fieldName != string.Empty)
                        {
                            ActivityHistory += fieldName + value + "&";
                        }
                    }
                    return ActivityHistory.Substring(0, ActivityHistory.Length - 1);
                }
            }
            catch (Exception generalException)
            {
                this.logger.Error("GetUpdateActivityLog : Error Occurred while Collecting activity Logs : " + generalException.ToString());
            }
            return null;
        }

        public XmlElement[] GetChatUpdateRecordData(KeyValueCollection activityLog, IMessage Message, SFDCCallType callType, string duration, string chatContent)
        {
            try
            {
                this.logger.Info("GetChatUpdateRecordData : Collection update Record data for the callType : " + callType.ToString());
                if (activityLog != null)
                {
                    int index = 0;
                    System.Xml.XmlElement[] Fields = new System.Xml.XmlElement[20];
                    System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
                    string keyPrefix = string.Empty;
                    dynamic chatEvents = Convert.ChangeType(Message, Message.GetType());

                    #region EventType

                    if (callType == SFDCCallType.InboundChat || callType == SFDCCallType.ConsultChatReceived)
                    {
                        keyPrefix = "inb";
                    }

                    #endregion EventType

                    foreach (KeyValueCollection temp1 in activityLog.AllValues)
                    {
                        string value = string.Empty;
                        updatefield = false;
                        enableTimeStamp = false;
                        string fieldName = temp1.GetAsString("field-name");
                        string fieldType = temp1.GetAsString("field-type");
                        if (!String.IsNullOrEmpty(temp1.GetAsString("enable.update")) && temp1.GetAsString("enable.update").Trim().ToLower() == "true")
                        {
                            updatefield = true;
                        }
                        if (!String.IsNullOrEmpty(temp1.GetAsString("enable.time-stamp")) && temp1.GetAsString("enable.time-stamp").Trim().ToLower() == "true")
                        {
                            enableTimeStamp = true;
                        }
                        if (!String.IsNullOrEmpty(fieldName) && updatefield)
                        {
                            if (fieldName.Equals("RecordTypeId"))
                            {
                                if (!String.IsNullOrEmpty(temp1.GetAsString("record-type.id")))
                                {
                                    fieldName = fieldName.Trim();
                                    value += temp1.GetAsString("record-type.id");
                                }
                            }
                            else
                            {
                                fieldName = fieldName.Trim();
                                if (!String.IsNullOrEmpty(fieldType))
                                { fieldType = fieldType.Trim().ToLower(); }
                                else
                                {
                                    fieldType = "datetime";
                                    enableTimeStamp = true;
                                }
                                if (fieldType == "text" || fieldType == "number")
                                {
                                    if (!String.IsNullOrEmpty(temp1.GetAsString(keyPrefix + ".user-data.key-name")) && chatEvents.Interaction.InteractionUserData != null)
                                    {
                                        value += chatEvents.Interaction.InteractionUserData.GetAsString(temp1.GetAsString(keyPrefix + ".user-data.key-name"));
                                    }
                                    else if (!String.IsNullOrEmpty(temp1.GetAsString(keyPrefix + ".attrib.key-name")))
                                    {
                                        value += GetAttributeValueForLog(temp1, chatEvents, keyPrefix, callType);
                                    }
                                    else if (!String.IsNullOrEmpty(temp1.GetAsString(keyPrefix + ".default.value")))
                                    {
                                        value += temp1.GetAsString(keyPrefix + ".default.value");
                                    }
                                    if (enableTimeStamp && fieldType != "number")
                                    {
                                        value += System.DateTime.Now.ToString();
                                    }
                                }
                                else if (fieldType == "duration")
                                {
                                    value += duration;
                                }
                                else if (fieldType == "chatcontent")
                                {
                                    value += chatContent;
                                }
                                else if (enableTimeStamp)
                                {
                                    if (fieldType == "date")
                                        value += System.DateTime.Now.ToString("yyyy-MM-dd");
                                    else if (fieldType == "time")
                                        value += System.DateTime.Now.ToShortTimeString();
                                    else if (fieldType == "datetime")
                                        value += System.DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
                                }
                                if (fieldType == "number")
                                {
                                    if (value == string.Empty || value == null)
                                    {
                                        value += "0";
                                    }
                                }
                            }
                        }
                        if (fieldName != string.Empty && value != string.Empty)
                        {
                            Fields[index] = doc.CreateElement(Convert.ToString(fieldName));
                            Fields[index].InnerText = Convert.ToString(value);
                            index++;
                        }
                        else
                        {
                            this.logger.Info("Log field name or field value is empty , field Name : " + fieldName + ", field value : " + value);
                        }
                    }
                    return Fields;
                }
            }
            catch (Exception generalException)
            {
                this.logger.Error("GetChatUpdateRecordData : Error Occurred while Collecting Record data : " + generalException.ToString());
            }
            return null;
        }

        public string GetUserDataSearchValues(KeyValueCollection userData, string[] userDataSearchKeys)
        {
            try
            {
                if (userData != null && userDataSearchKeys != null)
                {
                    string searchValues = string.Empty;
                    this.logger.Info("GetUserDataSearchValues: Reading User-Data Search Values, Keys : " + string.Join(",", userDataSearchKeys));
                    foreach (string key in userDataSearchKeys)
                    {
                        this.logger.Info("Reading attach data for the key : " + key);
                        if (userData.ContainsKey(key) && !String.IsNullOrEmpty(userData.GetAsString(key)))
                        {
                            if (Regex.IsMatch((userData.GetAsString(key)), @"^\d+$"))
                            {
                                searchValues += TruncateNumbers((userData.GetAsString(key)), 10) + ",";
                            }
                            else
                                searchValues += userData.GetAsString(key) + ",";
                        }
                        else
                        {
                            this.logger.Info("GetUserDataSearchValues: No Attach data found for the key : " + key);
                            searchValues += "^,";
                        }
                    }
                    return (searchValues != string.Empty) ? searchValues.Substring(0, searchValues.Length - 1) : searchValues;
                }
                else
                {
                    this.logger.Info("GetUserDataSearchValues: Either user-data or search keys are null....");
                }
            }
            catch (Exception generalException)
            {
                this.logger.Error("GetSearchUserDataValue: Error occurred while getting User-Data Search values : " + generalException.ToString());
            }
            return string.Empty;
        }

        public List<XmlElement> GetVoiceActivityLog(KeyValueCollection activityLog, IMessage Message, SFDCCallType callType)
        {
            try
            {
                this.logger.Info("GetVoiceActivityLog : Collecting Activity Logs for the callType : " + callType.ToString());

                if (activityLog != null)
                {
                    List<XmlElement> ActivityHistory = new List<XmlElement>();
                    string keyPrefix = string.Empty;
                    dynamic interactionEvent = Convert.ChangeType(Message, Message.GetType());
                    System.Xml.XmlDocument doc = new System.Xml.XmlDocument();

                    keyPrefix = GetPrefixString(callType);
                    foreach (KeyValueCollection temp1 in activityLog.AllValues)
                    {
                        string value = string.Empty;
                        element = null;
                        updatefield = false;
                        enableTimeStamp = false;
                        string fieldName = temp1.GetAsString("field-name");
                        string fieldType = temp1.GetAsString("field-type");

                        if (!String.IsNullOrEmpty(temp1.GetAsString("enable.update")) && temp1.GetAsString("enable.update").Trim().ToLower() == "true")
                        {
                            updatefield = true;
                        }
                        if (!String.IsNullOrEmpty(temp1.GetAsString("enable.time-stamp")) && temp1.GetAsString("enable.time-stamp").Trim().ToLower() == "true")
                        {
                            enableTimeStamp = true;
                        }
                        if (!String.IsNullOrEmpty(fieldName) && !updatefield)
                        {
                            if (fieldName.Equals("RecordTypeId"))
                            {
                                if (!String.IsNullOrEmpty(temp1.GetAsString("record-type.id")))
                                {
                                    fieldName = fieldName.Trim();
                                    value += temp1.GetAsString("record-type.id");
                                }
                            }
                            else
                            {
                                fieldName = fieldName.Trim();
                                if (!String.IsNullOrEmpty(fieldType))
                                { fieldType = fieldType.Trim().ToLower(); }
                                else
                                {
                                    fieldType = "datetime";
                                    enableTimeStamp = true;
                                }
                                if (fieldType == "text" || fieldType == "number")
                                {
                                    value += GetLogData(temp1, interactionEvent, keyPrefix, callType);
                                    if (enableTimeStamp && fieldType != "number")
                                    {
                                        value += "  " + System.DateTime.Now.ToString();
                                        if (callType == SFDCCallType.ConsultReceived && fieldName.ToLower().Equals("subject"))
                                        {
                                            if (ConsultInteractionId != interactionEvent.ConnID.ToString())
                                            {
                                                ConsultInteractionId = interactionEvent.ConnID.ToString();
                                                ConsultInitTime = "  " + System.DateTime.Now.ToString();
                                            }
                                        }
                                    }
                                }
                                if (enableTimeStamp)
                                {
                                    if (fieldType == "date")
                                        value += System.DateTime.Now.ToString("yyyy-MM-dd");
                                    else if (fieldType == "time")
                                        value += System.DateTime.Now.ToShortTimeString();
                                    else if (fieldType == "datetime")
                                    {
                                        value += System.DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
                                    }
                                    else if (fieldType == "date-timezone")
                                    {
                                        if (!String.IsNullOrEmpty(Settings.SFDCOptions.SFDCTimeZone))
                                        {
                                            value += System.DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss") + Settings.SFDCOptions.SFDCTimeZone;
                                        }
                                        else
                                        {
                                            value += System.DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
                                            this.logger.Info("No TimeZone Configured....");
                                        }
                                    }
                                }
                                if (fieldType == "number")
                                {
                                    if (value == string.Empty || value == null)
                                    {
                                        value += "0";
                                    }
                                }
                            }
                            if (fieldName != string.Empty && value != string.Empty)
                            {
                                element = doc.CreateElement(Convert.ToString(fieldName));
                                element.InnerText = Convert.ToString(value);
                                ActivityHistory.Add(element);
                            }
                            else
                            {
                                this.logger.Info("Log value is empty for the field Name : " + fieldName + ", field value : " + value);
                            }
                        }
                    }
                    return ActivityHistory;
                }
            }
            catch (Exception generalException)
            {
                this.logger.Error("GetVoiceActivityLog : Error Occurred while Collecting activity Logs : " + generalException.ToString());
            }

            return null;
        }

        public XmlElement[] GetVoiceRecordData(KeyValueCollection activityLog, IMessage Message, SFDCCallType callType)
        {
            try
            {
                this.logger.Info("GetVoiceRecordData : Collecting Record Data for the callType : " + callType.ToString());

                if (activityLog != null)
                {
                    string keyPrefix = string.Empty;
                    dynamic interactionEvent = Convert.ChangeType(Message, Message.GetType());
                    int index = 0;
                    System.Xml.XmlElement[] Fields = null;
                    if (activityLog.Count > 0)
                        Fields = new System.Xml.XmlElement[activityLog.AllValues.Length];

                    System.Xml.XmlDocument doc = new System.Xml.XmlDocument();

                    #region EventType

                    keyPrefix = GetPrefixString(callType);

                    #endregion EventType

                    foreach (KeyValueCollection temp1 in activityLog.AllValues)
                    {
                        string value = string.Empty;
                        updatefield = false;
                        enableTimeStamp = false;
                        string fieldName = temp1.GetAsString("field-name");
                        string fieldType = temp1.GetAsString("field-type");
                        if (!String.IsNullOrEmpty(temp1.GetAsString("enable.update")) && temp1.GetAsString("enable.update").Trim().ToLower() == "true")
                        {
                            updatefield = true;
                        }
                        if (!String.IsNullOrEmpty(temp1.GetAsString("enable.time-stamp")) && temp1.GetAsString("enable.time-stamp").Trim().ToLower() == "true")
                        {
                            enableTimeStamp = true;
                        }
                        if (!String.IsNullOrEmpty(fieldName) && !updatefield)
                        {
                            if (fieldName.Equals("RecordTypeId"))
                            {
                                if (!String.IsNullOrEmpty(temp1.GetAsString("record-type.id")))
                                {
                                    fieldName = fieldName.Trim();
                                    value += temp1.GetAsString("record-type.id");
                                }
                            }
                            else
                            {
                                fieldName = fieldName.Trim();
                                if (!String.IsNullOrEmpty(fieldType))
                                { fieldType = fieldType.Trim().ToLower(); }
                                else
                                {
                                    fieldType = "datetime";
                                    enableTimeStamp = true;
                                }
                                if (fieldType == "text" || fieldType == "number")
                                {
                                    value += GetLogData(temp1, interactionEvent, keyPrefix, callType);
                                    if (enableTimeStamp && fieldType != "number")
                                    {
                                        value += System.DateTime.Now.ToString();
                                    }
                                }
                                if (enableTimeStamp)
                                {
                                    if (fieldType == "date")
                                        value += System.DateTime.Now.ToString("yyyy-MM-dd");
                                    else if (fieldType == "time")
                                        value += System.DateTime.Now.ToShortTimeString();
                                    else if (fieldType == "datetime")
                                    {
                                        value += System.DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
                                    }
                                    else if (fieldType == "date-timezone")
                                    {
                                        if (!String.IsNullOrEmpty(Settings.SFDCOptions.SFDCTimeZone))
                                        {
                                            value += System.DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss") + Settings.SFDCOptions.SFDCTimeZone;
                                        }
                                        else
                                        {
                                            value += System.DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
                                            this.logger.Info("No TimeZone Configured....");
                                        }
                                    }
                                }
                                if (fieldType == "number")
                                {
                                    if (value == string.Empty || value == null)
                                    {
                                        value += "0";
                                    }
                                }
                            }
                            if (fieldName != string.Empty && value != string.Empty)
                            {
                                Fields[index] = doc.CreateElement(Convert.ToString(fieldName));
                                Fields[index].InnerText = Convert.ToString(value);
                                index++;
                            }
                            else
                            {
                                this.logger.Info("data is empty for the field Name : " + fieldName);
                            }
                        }
                    }
                    return Fields;
                }
            }
            catch (Exception generalException)
            {
                this.logger.Error("GetVoiceRecordData : Error Occurred while Collecting Record data : " + generalException.ToString());
            }

            return null;
        }

        public List<XmlElement> GetVoiceUpdateActivityLog(KeyValueCollection activityLog, IMessage Message, SFDCCallType callType, string duration)
        {
            try
            {
                this.logger.Info("GetVoiceUpdateActivityLog : Collection update log for the callType : " + callType.ToString());
                if (activityLog != null)
                {
                    List<XmlElement> ActivityHistory = new List<XmlElement>();
                    string keyPrefix = string.Empty;
                    dynamic voiceEvent = Convert.ChangeType(Message, Message.GetType());
                    System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
                    keyPrefix = GetPrefixString(callType);
                    foreach (KeyValueCollection temp1 in activityLog.AllValues)
                    {
                        string value = string.Empty;
                        updatefield = false;
                        enableTimeStamp = false;
                        string fieldName = temp1.GetAsString("field-name");
                        string fieldType = temp1.GetAsString("field-type");
                        if (!String.IsNullOrEmpty(temp1.GetAsString("enable.update")) && temp1.GetAsString("enable.update").Trim().ToLower() == "true")
                        {
                            updatefield = true;
                        }
                        if (!String.IsNullOrEmpty(temp1.GetAsString("enable.time-stamp")) && temp1.GetAsString("enable.time-stamp").Trim().ToLower() == "true")
                        {
                            enableTimeStamp = true;
                        }
                        if (!String.IsNullOrEmpty(fieldName) && updatefield)
                        {
                            if (fieldName.Equals("RecordTypeId"))
                            {
                                if (!String.IsNullOrEmpty(temp1.GetAsString("record-type.id")))
                                {
                                    fieldName = fieldName.Trim();
                                    value += temp1.GetAsString("record-type.id");
                                }
                            }
                            else
                            {
                                fieldName = fieldName.Trim();
                                if (!String.IsNullOrEmpty(fieldType))
                                { fieldType = fieldType.Trim().ToLower(); }
                                else
                                {
                                    fieldType = "datetime";
                                    enableTimeStamp = true;
                                }
                                if (fieldType == "text" || fieldType == "number")
                                {
                                    value += GetLogData(temp1, voiceEvent, keyPrefix, callType);
                                    if (enableTimeStamp && fieldType != "number")
                                    {
                                        if (callType == SFDCCallType.ConsultReceived && fieldName.ToLower().Equals("subject")
                                            && Settings.SFDCOptions.IsEnabledConsultSubjectWithInitDateTime)
                                            value += ConsultInitTime;
                                        else
                                            value += System.DateTime.Now.ToString();
                                    }
                                }
                                else if (fieldType == "duration")
                                {
                                    value += duration;
                                }
                                if (enableTimeStamp)
                                {
                                    if (fieldType == "date")
                                        value += System.DateTime.Now.ToString("yyyy-MM-dd");
                                    else if (fieldType == "time")
                                        value += System.DateTime.Now.ToShortTimeString();
                                    else if (fieldType == "datetime")
                                    {
                                        value += System.DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
                                    }
                                    else if (fieldType == "date-timezone")
                                    {
                                        if (!String.IsNullOrEmpty(Settings.SFDCOptions.SFDCTimeZone))
                                        {
                                            value += System.DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss") + Settings.SFDCOptions.SFDCTimeZone;
                                        }
                                        else
                                        {
                                            value += System.DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
                                            this.logger.Info("No TimeZone Configured....");
                                        }
                                    }
                                }
                                if (fieldType == "number")
                                {
                                    if (value == string.Empty || value == null)
                                    {
                                        value += "0";
                                    }
                                }
                            }
                            if (fieldName != string.Empty)//&& value != string.Empty)Empty value allowed as ram requested to update empty disposition on 10 march 2016
                            {
                                element = doc.CreateElement(Convert.ToString(fieldName));
                                element.InnerText = Convert.ToString(value);
                                ActivityHistory.Add(element);
                            }
                            else
                            {
                                this.logger.Info("Log value is empty , field Name : " + fieldName + ", field value : " + value);
                            }
                        }
                    }
                    return ActivityHistory;
                }
            }
            catch (Exception generalException)
            {
                this.logger.Error("GetVoiceUpdateActivityLog : Error Occurred while Collecting activity Logs : " + generalException.ToString());
            }
            return null;
        }

        public XmlElement[] GetVoiceUpdateRecordData(KeyValueCollection activityLog, IMessage Message, SFDCCallType callType, string duration)
        {
            try
            {
                this.logger.Info("GetVoiceUpdateRecordData : Collecting update Record for the callType : " + callType.ToString());
                this.logger.Info("GetVoiceUpdateRecordData : Call duration : " + duration);
                if (activityLog != null)
                {
                    string keyPrefix = string.Empty;
                    dynamic voiceEvent = Convert.ChangeType(Message, Message.GetType());
                    int index = 0;
                    System.Xml.XmlElement[] Fields = null;
                    if (activityLog.Count > 0)
                        Fields = new System.Xml.XmlElement[activityLog.AllValues.Length];

                    System.Xml.XmlDocument doc = new System.Xml.XmlDocument();

                    #region EventType

                    keyPrefix = GetPrefixString(callType);

                    #endregion EventType

                    foreach (KeyValueCollection temp1 in activityLog.AllValues)
                    {
                        string value = string.Empty;
                        updatefield = false;
                        enableTimeStamp = false;
                        string fieldName = temp1.GetAsString("field-name");
                        string fieldType = temp1.GetAsString("field-type");
                        if (!String.IsNullOrEmpty(temp1.GetAsString("enable.update")) && temp1.GetAsString("enable.update").Trim().ToLower() == "true")
                        {
                            updatefield = true;
                        }
                        if (!String.IsNullOrEmpty(temp1.GetAsString("enable.time-stamp")) && temp1.GetAsString("enable.time-stamp").Trim().ToLower() == "true")
                        {
                            enableTimeStamp = true;
                        }
                        if (!String.IsNullOrEmpty(fieldName) && updatefield)
                        {
                            if (fieldName.Equals("RecordTypeId"))
                            {
                                if (!String.IsNullOrEmpty(temp1.GetAsString("record-type.id")))
                                {
                                    fieldName = fieldName.Trim();
                                    value += temp1.GetAsString("record-type.id");
                                }
                            }
                            else
                            {
                                fieldName = fieldName.Trim();
                                if (!String.IsNullOrEmpty(fieldType))
                                { fieldType = fieldType.Trim().ToLower(); }
                                else
                                {
                                    fieldType = "datetime";
                                    enableTimeStamp = true;
                                }
                                if (fieldType == "text" || fieldType == "number")
                                {
                                    value += GetLogData(temp1, voiceEvent, keyPrefix, callType);
                                    if (enableTimeStamp && fieldType != "number")
                                    {
                                        value += System.DateTime.Now.ToString();
                                    }
                                }
                                else if (fieldType == "duration")
                                {
                                    value += duration;
                                }
                                if (enableTimeStamp)
                                {
                                    if (fieldType == "date")
                                        value += System.DateTime.Now.ToString("yyyy-MM-dd");
                                    else if (fieldType == "time")
                                        value += System.DateTime.Now.ToShortTimeString();
                                    else if (fieldType == "datetime")
                                        value += System.DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
                                    else if (fieldType == "date-timezone")
                                    {
                                        if (!String.IsNullOrEmpty(Settings.SFDCOptions.SFDCTimeZone))
                                        {
                                            value += System.DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss") + Settings.SFDCOptions.SFDCTimeZone;
                                        }
                                        else
                                        {
                                            value += System.DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
                                            this.logger.Info("No TimeZone Configured....");
                                        }
                                    }
                                }
                                if (fieldType == "number")
                                {
                                    if (value == string.Empty || value == null)
                                    {
                                        value += "0";
                                    }
                                }
                            }
                            if (fieldName != string.Empty && value != string.Empty)
                            {
                                Fields[index] = doc.CreateElement(Convert.ToString(fieldName));
                                Fields[index].InnerText = Convert.ToString(value);
                                index++;
                            }
                            else
                            {
                                this.logger.Info("data is empty for the field Name : " + fieldName);
                            }
                        }
                    }
                    return Fields;
                }
            }
            catch (Exception generalException)
            {
                this.logger.Error("GetVoiceUpdateRecordData : Error Occurred while Collecting activity Logs : " + generalException.ToString());
            }
            return null;
        }

        public OutputValues SearchSFDC(string searchData, string searchCondition, string searchObjects, string searchFields, string searchFormat, SFDCData data, SFDCCallType calltype, string connId)
        {
            try
            {
                this.logger.Info("SearchSFDC: SearchData=" + searchData + "\t SearchObjects=" + searchObjects +
                    "\t SearchCondition=" + searchCondition + "\t SearchFields=" + searchFields);
                OutputValues output = new OutputValues();
                output.ObjectName = searchObjects;
                if (String.IsNullOrEmpty(SForce.SessionHeaderValue.sessionId))
                {
                    this.logger.Error("Can not perform search at this time because SFDC SessionId is null....");
                    if (SFDCHttpServer.connectionStatus)
                    {
                        Settings.SFDCListener.SendSessionStatus(SFDCSessionStatus.NotConnected);

                        this.logger.Error("Requesting for Session Id from Salesforce.....");
                        SFDCHttpServer.sessionFlag = true;
                        if (Settings.SFDCOptions.AlertSFDCConnectionStatus)
                        {
                            Settings.SFDCListener.SFDCConnectionStatus(LogMode.Error, "Salesforce Session Id is null \n Requested for Salesforce Session Id");
                        }
                    }
                    else
                    {
                        Settings.SFDCListener.SendSessionStatus(SFDCSessionStatus.NotConnected);

                        if (Settings.SFDCOptions.SOAPAPIErrorMessageDisplay)
                        {
                            Settings.SFDCListener.SFDCConnectionStatus(LogMode.Error, "Salesforce not Connected \nSalesforce Session Id is null");
                        }
                        this.logger.Error("Can not perform search at this time because SFDC SessionId is null....");
                        this.logger.Error("Salesforce not connected...");
                    }
                    return null;
                }

                if (!String.IsNullOrEmpty(searchData) && !String.IsNullOrEmpty(searchObjects))
                {
                    string soslQuery = string.Empty;
                    string searchString = string.Empty;
                    SearchResult sresult = null;

                    if (string.IsNullOrEmpty(searchFormat))
                    {
                        searchFormat = "xxxxxxxxxx";
                    }
                    //{
                    foreach (string format in searchFormat.Split(','))
                    {
                        try
                        {
                            searchString = GetSearchString(searchData, searchCondition, format);
                            if (!String.IsNullOrEmpty(searchString))
                            {
                                this.logger.Info("************Request from AID***********************");
                                this.logger.Info("Get Search string using format  : " + format);
                                soslQuery = "FIND {" + searchString + "} IN All FIELDS RETURNING " + searchObjects;
                                bool isSpecificFieldsSearch = false;
                                if (!searchData.Contains(",") && !string.IsNullOrEmpty(searchFields))
                                {
                                    soslQuery += "(Id," + searchFields + ")";
                                    isSpecificFieldsSearch = true;
                                }

                                this.logger.Info("Search Record using Query : " + soslQuery);

                                sresult = SForce.search(soslQuery);
                                if (sresult != null && sresult.searchRecords != null)
                                {
                                    if (isSpecificFieldsSearch)
                                    {
                                        var searchRecord = new List<SearchRecord>();
                                        foreach (SearchRecord item in sresult.searchRecords)
                                        {
                                            if (item.record.Any[1].InnerText == searchString)
                                            {
                                                searchRecord.Add(item);
                                            }
                                        }
                                        if (searchRecord.Count > 0)
                                        {
                                            output.SearchData = searchString;
                                            output.SearchRecord = searchRecord.ToArray();
                                        }
                                        else
                                        {
                                            output.SearchData = searchString;
                                            this.logger.Info("************Service Response***********************");
                                            this.logger.Warn("No Record found using the search query :" + soslQuery);
                                            this.logger.Info("***********************************");
                                        }
                                    }
                                    else
                                    {
                                        output.SearchData = searchString;
                                        output.SearchRecord = sresult.searchRecords;
                                    }
                                    if (Settings.SFDCOptions.CanEnableRetrySearchSessionID)
                                    {
                                        var remvoeTuple = SessionSearchCollection.Find(x => x.Item1 == connId);
                                        if (remvoeTuple != null)
                                        {
                                            this.logger.Info("SearchSFDC : Removed search details from collection for search retry, Connid: " + connId);
                                            this.logger.Info("SearchSFDC : SessionSearchCollection count before remove :" + SessionSearchCollection.Count);
                                            SessionSearchCollection.Remove(remvoeTuple);
                                            this.logger.Info("SearchSFDC : SessionSearchCollection count after remove :" + SessionSearchCollection.Count);
                                        }
                                        else
                                        {
                                            this.logger.Info("SearchSFDC : SessionSearchCollection find return null for connection id " + connId);
                                        }
                                    }
                                    else
                                    {
                                        this.logger.Info("SearchSFDC : CanEnableRetrySearchSessionID is false, to remove from collection");
                                    }
                                    break;
                                }
                                else
                                {
                                    output.SearchData = searchString;
                                    this.logger.Info("************Service Response***********************");
                                    this.logger.Warn("No Record found using the search query :" + soslQuery);
                                    this.logger.Info("***********************************");
                                }
                            }
                            else
                            {
                                this.logger.Warn("GetSearchString method returned null or empty string for the searchData :" + searchData);
                            }
                        }
                        catch (Exception generalException)
                        {
                            if (generalException.Message.Contains("INVALID_SESSION_ID"))
                            {
                                this.logger.Error("Invalid session id error detected ");
                                if (Settings.SFDCOptions.CanEnableSessionIDInLog)
                                {
                                    logger.Info("Search : Failed session id is:" + SForce.SessionHeaderValue.sessionId);
                                }
                                if (Settings.SFDCOptions.CanEnableRetryInvalidSessionID)
                                {
                                    _sessionRetrycount += 1;
                                    if (_sessionRetrycount <= Settings.SFDCOptions.MaxNosSessionRetryRequest)
                                    {
                                        this.logger.Info("SearchSFDC : Adapter requesting session id to script due to SOAP Time out error : Attempt :" + _sessionRetrycount);
                                        this.logger.Info("SearchSFDC : Max session id Retry Attempt = " + Settings.SFDCOptions.MaxNosSessionRetryRequest);
                                        SFDCHttpServer.sessionFlag = true;
                                    }
                                }
                                if (Settings.SFDCOptions.CanEnableRetrySearchSessionID)
                                {
                                    if (!SessionSearchCollection.Any(x => x.Item1 == connId))
                                    {
                                        this.logger.Info("SearchSFDC : Added search details in collection for search retry");
                                        SessionSearchCollection.Add(new Tuple<string, SFDCCallType, SFDCData>(connId, calltype, data));
                                    }
                                    else
                                        this.logger.Info("SearchSFDC : search details not in collection for search retry any returns null");
                                    SFDCHttpServer.sessionFlag = true;
                                    SFDCHttpServer.NewSessionIDFlag = true;
                                }
                                else
                                {
                                    this.logger.Info("SearchSFDC : CanEnableRetrySearchSessionID is false, to add to collection");
                                }
                                Settings.SFDCListener.SendSessionStatus(SFDCSessionStatus.NotConnected);
                                break;
                            }
                            if (Settings.SFDCOptions.SOAPAPIErrorMessageDisplay)
                            {
                                Settings.SFDCListener.SFDCConnectionStatus(LogMode.Error, "Exception thrown while requesting Salesforce Service\n" + generalException.Message);
                            }
                            this.logger.Error("SearchSFDC: Exception thrown while requesting Salesforce Service : " + generalException.ToString() + generalException.StackTrace);
                        }
                    }
                    //}
                    //else
                    //{
                    //    searchString = GetSearchString(searchData, searchCondition, "(xxx) xxx-xxxx");
                    //    if (!String.IsNullOrEmpty(searchString))
                    //    {
                    //        this.logger.Info("************Request from AID***********************");
                    //        soslQuery = "FIND {" + searchString + "} IN All FIELDS RETURNING " + searchObjects;
                    //        this.logger.Info("Search Record using Query : " + soslQuery);

                    // sresult = SForce.search(soslQuery);

                    // if (sresult != null) { if (sresult.searchRecords != null) {
                    // this.logger.Info("************Service Response***********************");
                    // this.logger.Info("Response from Salesforce : " +
                    // sresult.searchRecords.ToString()); }

                    //            output.SearchRecord = sresult.searchRecords;
                    //            output.SearchData = searchString;
                    //        }
                    //        else
                    //        {
                    //            this.logger.Info("************Service Response***********************");
                    //            this.logger.Warn("No Record found using the search query :" + soslQuery);
                    //        }
                    //    }
                    //    else
                    //    {
                    //        this.logger.Warn("GetSearchString method returned null or empty string for the searchData :" + searchData);
                    //    }
                    //}

                    try
                    {
                        if (output.SearchRecord != null)
                        {
                            this.logger.Info("************Service Response***********************");
                            this.logger.Info("Response from Salesforce for the Query : " + soslQuery);
                            foreach (var i in sresult.searchRecords)
                            {
                                this.logger.Info("Record Type: " + i.record.type + "\t Record Id :" + i.record.Id);
                            }
                            this.logger.Info("\n***********************************");
                        }
                    }
                    catch
                    {
                    }
                    return output;
                }
                else
                {
                    if (!String.IsNullOrEmpty(searchObjects) && searchObjects.Contains("__c"))
                    {
                        this.logger.Info("custom object searchData is not found, Invoking NoRecordFoundScenario for the object " + searchObjects);
                        return output;
                    }
                }
            }
            catch (Exception generalException)
            {
                //if (generalException.Message.Contains("INVALID_SESSION_ID"))
                //{
                //    this.logger.Error("Invalid session id error detected " + generalException.Message);
                //    Settings.SFDCListener.SendSessionStatus(SFDCSessionStatus.NotConnected);
                //}
                if (Settings.SFDCOptions.SOAPAPIErrorMessageDisplay)
                {
                    Settings.SFDCListener.SFDCConnectionStatus(LogMode.Error, "Exception thrown while requesting Salesforce Service\n" + generalException.Message);
                }
                this.logger.Error("SearchSFDC: Exception thrown while requesting Salesforce Service : " + generalException.ToString() + generalException.StackTrace);
            }
            return null;
        }

        public void SendTestRequest()
        {
            try
            {
                this.logger.Info("SendTestRequest: Sending test request to salesforce........");
                if (!String.IsNullOrEmpty(SForce.SessionHeaderValue.sessionId) && !String.IsNullOrEmpty(SForce.Url))
                {
                    this.logger.Info("***********************************");
                    string Query = "FIND {1234567890} IN All FIELDS RETURNING CONTACT";
                    this.logger.Info("Search Query for test Request : " + Query);
                    SearchResult result = SForce.search(Query);
                    this.logger.Info("Send request done.");
                    if (result != null)
                    {
                        this.logger.Info("Salesforce Connected with SFDC Adapter.....");
                        this.logger.Info("SendTestRequest: Success Response received for test request ");
                        SFDCHttpServer.IsFirstRequestMade = true;
                        Settings.SFDCListener.SendSessionStatus(SFDCSessionStatus.Connected);
                        if (Settings.SFDCOptions.AlertSFDCConnectionStatus)
                        {
                            Settings.SFDCListener.SFDCConnectionStatus(LogMode.Info, Settings.SFDCOptions.SFDCConnectionSuccessMessage);
                        }
                    }
                    else
                    {
                        SFDCHttpServer.IsFirstRequestMade = false;
                        this.logger.Info("SendTestRequest: Null Response received for test request ");
                    }
                    this.logger.Info("***********************************");
                }
                else
                {
                    this.logger.Info("SendTestRequest: Can not make test request to salesforce because either the session id or Url is null...");
                }
            }
            catch (Exception generalException)
            {
                SFDCHttpServer.IsFirstRequestMade = false;
                if (generalException.Message.Contains("INVALID_SESSION_ID"))
                {
                    if (Settings.SFDCOptions.CanEnableRetrySearchSessionID)
                    {
                        this.logger.Info("Requested for new session id");
                        SFDCHttpServer.sessionFlag = true;
                    }
                    Settings.SFDCListener.SendSessionStatus(SFDCSessionStatus.NotConnected);
                }
                this.logger.Error("SendTestRequest: Error Occurred while Sending test request to salesforce........" + generalException.ToString());
            }
        }

        public void UpdateActivityLog(string interactionId, List<XmlElement> taskFields, string objectName, string recordId, int retryAttempt = 0)
        {
            try
            {
                this.logger.Info("UpdateActivityLog: Update Activity Log Invoked for the Object :" + objectName + " record id : " + recordId + " and  Interaction Id :" +
                    interactionId);
                bool canSendRequestForLog = false;
                if (taskFields != null)
                {
                    var element = (XmlElement)taskFields.Where(item => item.Name == "Id").FirstOrDefault();
                    var element1 = taskFields.IndexOf(element);
                    if (element1 <= 0)
                    {
                        //Remove ID field if already exist fix for session retry
                        XmlElement[] elements = taskFields.ToArray();
                        foreach (var item in elements)
                        {
                            if (item.Name == "Id")
                            {
                                taskFields.Remove(item);
                            }
                        }
                        element = (new XmlDocument()).CreateElement("Id");
                        element.InnerText = recordId;
                        taskFields.Add(element);
                    }
                    else
                    {
                        element = (XmlElement)taskFields.Where(item => item.Name == "Id" && item.InnerText == recordId).FirstOrDefault();
                        element.InnerText = recordId;
                        taskFields[element1] = element;
                    }

                    this.logger.Info("Activity Log for the object : " + objectName);
                    string log = "\n***********************************\n";
                    for (int i = 0; i < taskFields.Count; i++)
                    {
                        if (taskFields[i] != null)
                        {
                            canSendRequestForLog = true;
                            log += "\tField Name : " + taskFields[i].Name + "\t Field Value : " + taskFields[i].InnerText + "\n";
                        }
                    }
                    log += "***********************************";
                    this.logger.Info(log);
                }
                else
                {
                    logger.Warn("Taskfields in null");
                }

                if (taskFields != null && canSendRequestForLog)
                {
                    XmlElement[] logData = taskFields.ToArray();
                    sObject sobject = new sObject();
                    sobject.type = "Task";
                    sobject.Any = logData;
                    PForce.sObject[] array = new sObject[1];
                    array[0] = sobject;
                    SaveResult[] saveResult = null;
                    this.logger.Info("************Request from AID***********************");
                    this.logger.Info("Request sent to salesforce to Update activity log for the Object : " + objectName + " and the record id : "
                        + recordId);
                    this.logger.Info("************Request from AID***********************");
                    saveResult = SForce.update(array);
                    if (!saveResult[0].success)
                    {
                        if (Settings.SFDCOptions.SOAPAPIErrorMessageDisplay)
                        {
                            Settings.SFDCListener.SFDCConnectionStatus(LogMode.Error, "Error Returned while Updating activity log for the record id :"
                                + recordId + "\n Error Message : " + saveResult[0].errors[0].message);
                        }
                        this.logger.Info("************Service Response***********************");
                        this.logger.Error("UpdateActivityLog: Error Returned while Updating new activity log for the record id  :"
                            + recordId + ", Error Message : " + saveResult[0].errors[0].message);
                        this.logger.Error("*********************************************");
                    }
                    else
                    {
                        this.logger.Info("************Service Response***********************");
                        this.logger.Info("Response from salesforce for Updating activitylog for the Type :" + objectName
                            + " and the record Id :" + recordId);
                        this.logger.Info("Response Id :" + saveResult[0].id);
                        this.logger.Info("*********************************************");
                        if (Settings.SFDCOptions.CanEnableRetrySearchSessionID)
                        {
                            var remvoeTuple = SessionUpdateActivityCollection.Find(x => x.Item1 == interactionId);
                            if (remvoeTuple != null)
                            {
                                this.logger.Info("UpdateActivityLog : Removed activity details from collection for update retry, recordid: " + recordId);
                                this.logger.Info("UpdateActivityLog : SessionUpdateActivityCollection  before remove count: " + SessionUpdateActivityCollection.Count);
                                SessionUpdateActivityCollection.Remove(remvoeTuple);
                                this.logger.Info("UpdateActivityLog :  SessionUpdateActivityCollection  after remove count: " + SessionUpdateActivityCollection.Count);
                            }
                            else
                                this.logger.Info("UpdateActivityLog :  SessionUpdateActivityCollection  find returns null for record id: " + recordId);
                        }
                        else
                        {
                            this.logger.Info("UpdateActivityLog : CanEnableRetrySearchSessionID is false, to remove from collection");
                        }
                    }
                }
                else
                {
                    this.logger.Error("UpdateActivityLog: Can not update log for the object " + objectName + " because Update log Data is empty ...");
                }
            }
            catch (WebException webException)
            {
                if (webException.Status == WebExceptionStatus.Timeout)
                {
                    //Retry on time out error.
                    this.logger.Error("UpdateActivityLog: Timeout Error Occurred while Updating activity log for the record id " + recordId
                        + " :" + webException.Message);
                    retryAttempt += 1;
                    if (retryAttempt <= Settings.SFDCOptions.SOAPTimeoutRetryAttempt)
                    {
                        this.logger.Info("UpdateActivityLog : Adapter Executing CreateActivityLog method again due to SOAP Time out error");
                        this.logger.Info("UpdateActivityLog : Timeout attempt = " + retryAttempt);
                        this.logger.Info("UpdateActivityLog : Configured Max Timeout Attempt = " + Settings.SFDCOptions.SOAPTimeoutRetryAttempt);
                        Task.Factory.StartNew(() => CreateActivityLog(interactionId, taskFields, objectName, recordId, retryAttempt));
                    }
                }
                else
                {
                    this.logger.Error("UpdateActivityLog: Error Occurred while Updating activity log for the record id  " + recordId + " :" +
                        webException.Message);
                }
                if (Settings.SFDCOptions.SOAPAPIErrorMessageDisplay)
                {
                    Settings.SFDCListener.SFDCConnectionStatus(LogMode.Error, "Error Returned while Updating activity log for the record id :" +
                        recordId + "\nMessage : " + webException.Message);
                }
            }
            catch (Exception generalException)
            {
                this.logger.Error("UpdateActivityLog: Error Occurred while Updating activity log for the record id  " + recordId + " :" +
                    generalException.ToString() + "\n" + generalException.StackTrace);

                if (generalException.Message.Contains("INVALID_SESSION_ID"))
                {
                    if (Settings.SFDCOptions.CanEnableSessionIDInLog)
                    {
                        logger.Info("Update activity : Failed session id is:" + SForce.SessionHeaderValue.sessionId);
                    }
                    Settings.SFDCListener.SendSessionStatus(SFDCSessionStatus.NotConnected);
                    if (Settings.SFDCOptions.CanEnableRetrySearchSessionID)
                    {
                        var record = SessionUpdateActivityCollection.Find(x => x.Item1 == interactionId);
                        if (record == null)
                        {
                            this.logger.Info("UpdateActivityLog : Added update activity details in collection for update activity retry");
                            this.logger.Info("Parameters are : interaction id : " + interactionId + " Taskfields :" + taskFields.Count + " object name :" + objectName + " recordid : " + recordId + " Retry attempt : " + retryAttempt);

                            SessionUpdateActivityCollection.Add(new Tuple<string, List<XmlElement>, string, string, int>(interactionId, taskFields, objectName, recordId, retryAttempt));
                        }
                        else
                        {
                            SessionUpdateActivityCollection.Remove(record);
                            SessionUpdateActivityCollection.Add(new Tuple<string, List<XmlElement>, string, string, int>(interactionId, taskFields, objectName, recordId, retryAttempt));
                            this.logger.Info("UpdateActivityLog : update activity details are replaced with new values in collection for update activity retry");
                            this.logger.Info("Old values are : interaction id : " + record.Item1 + " Taskfields :" + record.Item2.Count + " object name :" + record.Item3 + " recordid : " + record.Item4 + " Retry attempt : " + record.Item5);
                            this.logger.Info("New values are : interaction id : " + interactionId + " Taskfields :" + taskFields.Count + " object name :" + objectName + " recordid : " + recordId + " Retry attempt : " + retryAttempt);
                        }
                        SFDCHttpServer.sessionFlag = true;
                        SFDCHttpServer.NewSessionIDFlag = true;
                    }
                    else
                    {
                        this.logger.Info("UpdateActivityLog : CanEnableRetrySearchSessionID is false, to add in collection");
                    }
                }
                if (Settings.SFDCOptions.SOAPAPIErrorMessageDisplay)
                {
                    Settings.SFDCListener.SFDCConnectionStatus(LogMode.Error, "Error Returned while Updating activity log for the Type :" +
                        objectName + "\n Error Message : " + generalException.Message);
                }
            }
        }

        public void UpdateNewRecord(string interactionId, System.Xml.XmlElement[] taskFields, string objectName, string recordId, int retryAttempt = 0)
        {
            try
            {
                bool canSendRequest = false;
                this.logger.Info("UpdateNewRecord: Update Record for the Object :" + objectName);
                this.logger.Info("UpdateNewRecord: Interaction Id :" + interactionId);
                this.logger.Info("UpdateNewRecord: Record Id :" + recordId);
                try
                {
                    if (taskFields != null)
                    {
                        this.logger.Info(" Display Update Data for the object : " + objectName);
                        string log = "\n***********************************\n";
                        for (int i = 0; i < taskFields.Length; i++)
                        {
                            if (taskFields[i] != null)
                            {
                                canSendRequest = true;
                                log += "\tField Name : " + taskFields[i].Name + "\t Field Value : " + taskFields[i].InnerText + "\n";
                            }
                        }
                        log += "***********************************";
                        this.logger.Info(log);
                    }
                }
                catch
                {
                }
                if (taskFields != null && canSendRequest)
                {
                    sObject Task = new sObject();
                    Task.type = objectName;
                    Task.Id = recordId;

                    Task.Any = taskFields;
                    PForce.sObject[] array = new sObject[1];
                    array[0] = Task;
                    SaveResult[] saveResult = null;
                    this.logger.Info("************Request from AID***********************");
                    this.logger.Info("UpdateNewRecord: Request sent to salesforce to update SFDC Record for the Id : " + recordId);
                    saveResult = SForce.update(array);
                    if (!saveResult[0].success)
                    {
                        if (Settings.SFDCOptions.SOAPAPIErrorMessageDisplay)
                        {
                            Settings.SFDCListener.SFDCConnectionStatus(LogMode.Error, "Error Returned while Updating Record for the Type :" + objectName + "\nMessage : " + saveResult[0].errors[0].message);
                        }
                        this.logger.Info("************Service Response***********************");
                        this.logger.Error("UpdateNewRecord: Error Returned while Creating New Record for the Type :" + objectName + ", Message : " + saveResult[0].errors[0].message);
                        this.logger.Error("*******************************************************");
                    }
                    else
                    {
                        this.logger.Info("************Service Response***********************");
                        this.logger.Info("Update Record is success for the id : " + saveResult[0].id);
                        this.logger.Info("*******************************************************");
                    }
                }
                else
                {
                    this.logger.Error("UpdateNewRecord: Can not update record for the object " + objectName + " because Update Record Data is empty ...");
                }
            }
            catch (WebException webException)
            {
                if (webException.Status == WebExceptionStatus.Timeout)
                {
                    //Retry on time out error.
                    this.logger.Error("UpdateNewRecord : Timeout Error Occurred while Updating Record for the type " + objectName + " :" + webException.Message);
                    retryAttempt += 1;
                    if (retryAttempt <= Settings.SFDCOptions.SOAPTimeoutRetryAttempt)
                    {
                        this.logger.Info("UpdateNewRecord : Adapter Executing UpdateNewRecord method again due to SOAP Time out error");
                        this.logger.Info("UpdateNewRecord : Timeout attempt = " + retryAttempt);
                        this.logger.Info("UpdateNewRecord : Configured Max Timeout Attempt = " + Settings.SFDCOptions.SOAPTimeoutRetryAttempt);
                        Task.Factory.StartNew(() => UpdateNewRecord(interactionId, taskFields, objectName, recordId, retryAttempt));
                    }
                }
                else
                {
                    this.logger.Error("UpdateNewRecord: Error Occurred while Updating Record for the type " + objectName + " :" + webException.Message);
                }
                if (Settings.SFDCOptions.SOAPAPIErrorMessageDisplay)
                {
                    Settings.SFDCListener.SFDCConnectionStatus(LogMode.Error, "Error Returned while Updating Record for the Type :" + objectName + "\nMessage : " + webException.Message);
                }
            }
            catch (Exception generalException)
            {
                this.logger.Error("UpdateNewRecord: Error at while Update New Record :" + generalException.Message);
                if (generalException.Message.Contains("INVALID_SESSION_ID"))
                {
                    Settings.SFDCListener.SendSessionStatus(SFDCSessionStatus.NotConnected);
                }
                if (Settings.SFDCOptions.SOAPAPIErrorMessageDisplay)
                {
                    Settings.SFDCListener.SFDCConnectionStatus(LogMode.Error, "Error Returned while Updating Record for the Type :" + objectName + "\nMessage : " + generalException.Message);
                }
            }
        }

        public bool ValidateSearchData(string data)
        {
            try
            {
                if (!String.IsNullOrEmpty(data))
                {
                    return (Regex.Matches(data, @"[a-zA-Z0-9]").Count) > 0;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        private void ConsolidateResponseIds(string ixnId, string objectName, string activityRecordId)
        {
            try
            {
                this.logger.Info("Consolidating sfdc respose id for the interaction:" + ixnId);
                if (!Settings.SFDCListener.ConsultResponseIds.ContainsKey(ixnId))
                {
                    Settings.SFDCListener.ConsultResponseIds.Add(ixnId, activityRecordId);
                }
                else
                {
                    this.logger.Info("Overriding Activity Log response id :" + Settings.SFDCListener.ConsultResponseIds[ixnId] + " with new Id :" + activityRecordId);
                    Settings.SFDCListener.ConsultResponseIds[ixnId] = activityRecordId;
                }
                if (Settings.UpdateSFDCLog.ContainsKey(ixnId))
                {
                    UpdateLogData oldResponseData = Settings.UpdateSFDCLog[ixnId];
                    switch (objectName.ToLower())
                    {
                        case "lead":
                            oldResponseData.LeadActivityId = activityRecordId;
                            break;

                        case "contact":
                            oldResponseData.ContactActivityId = activityRecordId;
                            break;

                        case "account":
                            oldResponseData.AccountActivityId = activityRecordId;
                            break;

                        case "case":
                            oldResponseData.CaseActivityId = activityRecordId;
                            break;

                        case "opportunity":
                            oldResponseData.OpportunityActivityId = activityRecordId;
                            break;

                        case "useractivity":
                            oldResponseData.UserActivityId = activityRecordId;
                            break;

                        case "profileactivity":
                            oldResponseData.ProfileActivityId = activityRecordId;
                            break;

                        default:
                            if (oldResponseData.CustomObject != null)
                            {
                                if (!oldResponseData.CustomObject.ContainsKey(objectName))
                                {
                                    KeyValueCollection coll = new KeyValueCollection();
                                    if (activityRecordId != null)
                                    {
                                        coll.Add("activityRecordId", activityRecordId);
                                    }
                                    if (coll.Count > 0)
                                        oldResponseData.CustomObject.Add(objectName, coll);
                                }
                                else
                                {
                                    if (activityRecordId != null)
                                    {
                                        if (oldResponseData.CustomObject[objectName].ContainsKey("activityRecordId"))
                                            oldResponseData.CustomObject[objectName]["activityRecordId"] = activityRecordId;
                                        else
                                            oldResponseData.CustomObject[objectName].Add("activityRecordId", activityRecordId);
                                    }
                                }
                            }
                            break;
                    }
                }
                else
                {
                    UpdateLogData responseData = new UpdateLogData();
                    switch (objectName.ToLower())
                    {
                        case "lead":
                            responseData.LeadActivityId = activityRecordId;
                            break;

                        case "contact":
                            responseData.ContactActivityId = activityRecordId;
                            break;

                        case "account":
                            responseData.AccountActivityId = activityRecordId;
                            break;

                        case "case":
                            responseData.CaseActivityId = activityRecordId;
                            break;

                        case "opportunity":
                            responseData.OpportunityActivityId = activityRecordId;
                            break;

                        case "useractivity":
                            responseData.UserActivityId = activityRecordId;
                            break;

                        case "profileactivity":
                            responseData.ProfileActivityId = activityRecordId;
                            break;

                        default:
                            if (responseData.CustomObject != null)
                            {
                                KeyValueCollection dic = new KeyValueCollection();
                                if (!String.IsNullOrEmpty(activityRecordId))
                                {
                                    dic.Add("activityRecordId", activityRecordId);
                                }
                                if (dic.Count > 0)
                                {
                                    responseData.CustomObject.Add(objectName, dic);
                                }
                            }
                            break;
                    }
                    Settings.UpdateSFDCLog.Add(ixnId, responseData);
                }
            }
            catch (Exception generalException)
            {
                this.logger.Error("ConsolidateResponseIds : Error occurred : " + generalException.ToString());
            }
        }

        private string FormatPhoneNumber(string format, string phone)
        {
            int length = format.Length < phone.Length ? format.Length : phone.Length;
            char[] cformat = format.ToCharArray();
            char[] cphone = phone.ToCharArray();
            int d = 0;
            string data = string.Empty;
            for (int i = 0; i < length; i++)
            {
                if (cformat[i] == 'x')
                {
                    data += cphone[d];   //phone number assign
                    d++;
                }
                else
                {
                    data += cformat[i];//spl char assignment
                    if (cformat.Length >= length + 1)
                        length++;
                }
            }
            return data;
        }

        private string GetAttributeValueForLog(KeyValueCollection collection, dynamic interactionEvent, string keyPrefix, SFDCCallType callType)
        {
            string output = null;
            try
            {
                string searchKey = collection.GetAsString(keyPrefix + ".attrib.key-name");
                if (interactionEvent != null && !String.IsNullOrWhiteSpace(searchKey))
                {
                    string[] keys = searchKey.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (var key in keys)
                    {
                        if (String.IsNullOrWhiteSpace(output))
                        {
                            switch (key)
                            {
                                case "ani":
                                    output = interactionEvent.ANI;
                                    break;

                                case "thisdn":
                                    output = interactionEvent.ThisDN;
                                    break;

                                case "otherdn":
                                    output = interactionEvent.OtherDN;
                                    break;

                                case "connid":
                                    output = interactionEvent.ConnID.ToString();
                                    break;

                                case "agentid":
                                    output = interactionEvent.AgentID;
                                    break;

                                case "calluuid":
                                    output = interactionEvent.CallUuid;
                                    break;

                                case "dnis":
                                    output = interactionEvent.DNIS;
                                    break;

                                case "calltype":
                                    if (callType == SFDCCallType.ConsultReceived && !string.IsNullOrWhiteSpace(SubscribeVoiceEvents.ConsultText) && SubscribeVoiceEvents.IsCallTransfered)
                                        output = SubscribeVoiceEvents.ConsultText;
                                    else
                                        output = interactionEvent.CallType.ToString();
                                    break;

                                default:
                                    break;
                            }
                        }
                        else
                            return output;
                    }
                }
                else
                {
                    this.logger.Info("GetAttributeValueForLog: Attribute key is null or empty");
                }
            }
            catch (Exception generalException)
            {
                this.logger.Error("GetAttributeValueForLog : Reading Attribute Search Values, Keys : " + generalException.ToString());
            }
            return output;
        }

        private string GetLogData(KeyValueCollection temp1, dynamic message, string keyPrefix, SFDCCallType callType)
        {
            string output = string.Empty;
            try
            {
                this.logger.Info("GetLogData: Reading Log/Record data for the field: " + temp1.GetAsString("field-name"));
                this.logger.Info("KeyPrefix : " + keyPrefix);
                if (!String.IsNullOrEmpty(temp1.GetAsString(keyPrefix + ".user-data.key-name")) && message.UserData != null)
                {
                    output = GetSplitUserData(temp1.GetAsString(keyPrefix + ".user-data.key-name"), message.UserData);
                    this.logger.Info("GetLogData: User-data value found : " + output);
                }
                if (String.IsNullOrWhiteSpace(output))
                {
                    if (!String.IsNullOrEmpty(temp1.GetAsString(keyPrefix + ".attrib.key-name")))
                    {
                        output = GetAttributeValueForLog(temp1, message, keyPrefix, callType);
                        if (String.IsNullOrWhiteSpace(output))
                        {
                            if (!String.IsNullOrEmpty(temp1.GetAsString(keyPrefix + ".default.value")))
                            {
                                output = temp1.GetAsString(keyPrefix + ".default.value");
                                this.logger.Info("GetLogData: default value found: " + output);
                            }
                        }
                        else
                        {
                            this.logger.Info("GetLogData: attribute value found: " + output);
                        }
                    }
                    else if (!String.IsNullOrEmpty(temp1.GetAsString(keyPrefix + ".default.value")))
                    {
                        output = temp1.GetAsString(keyPrefix + ".default.value");
                        this.logger.Info("GetLogData: default value found: " + output);
                    }
                }
            }
            catch (Exception generalException)
            {
                this.logger.Error("GetLogData : Error Occurred while Collecting Log data : " + generalException.ToString());
            }

            if (String.IsNullOrWhiteSpace(output) && keyPrefix == "con")
            {
                output = GetLogData(temp1, message, "inb", callType);
            }

            if (output != null)
                return output;
            else
                return string.Empty;
        }

        private string GetPrefixString(SFDCCallType callType)
        {
            if (callType == SFDCCallType.Inbound)
            {
                return "inb";
            }
            else if (callType == SFDCCallType.OutboundSuccess)
            {
                return "out.success";
            }
            else if (callType == SFDCCallType.ConsultReceived)
            {
                return "con";
            }
            else if (callType == SFDCCallType.OutboundFailure)
            {
                return "out.fail";
            }
            else if (callType == SFDCCallType.ConsultSuccess)
            {
                return "con.success";
            }
            else if (callType == SFDCCallType.ConsultFailure)
            {
                return "con.fail";
            }
            return string.Empty;
        }

        private string GetSearchString(string searchData, string searchCondition, string format)
        {
            try
            {
                searchData = searchData.Replace("^,", "").Replace(",^", "").Replace("^", "");
                string[] searchvalues = null;
                string searchstring = string.Empty;
                if (searchData.Contains(','))
                {
                    searchvalues = searchData.Split(',');
                }
                else
                {
                    searchvalues = new string[] { searchData };
                }

                foreach (string searchkey in searchvalues)
                {
                    if (Regex.IsMatch(searchkey, @"^\d+$") && searchkey.Length == 10 && !searchkey.StartsWith("1"))
                    {
                        if (searchstring != string.Empty)
                        {
                            searchstring += " " + searchCondition + " " + FormatPhoneNumber(format, searchkey);
                        }
                        else
                        {
                            searchstring += FormatPhoneNumber(format, searchkey);
                        }
                    }
                    else
                    {
                        if (searchstring != string.Empty)
                        {
                            searchstring += " " + searchCondition + " " + searchkey;
                        }
                        else
                        {
                            searchstring += searchkey;
                        }
                    }
                }
                return searchstring;
            }
            catch (Exception generalException)
            {
                this.logger.Error("Error Occurred while getting searchstring  :" + generalException.ToString());
            }
            return null;
        }

        private string GetSplitUserData(dynamic _input, dynamic _userdata)
        {
            try
            {
                var input = Convert.ToString(_input);
                var userdata = (KeyValueCollection)_userdata;
                if (!string.IsNullOrWhiteSpace(input))
                {
                    string[] userdataKeys = input.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    if (userdataKeys != null && userdata != null)
                    {
                        foreach (string key in userdataKeys)
                        {
                            if (userdata.CheckKeyAndValue(key))
                                return userdata.GetAsString(key);
                        }
                    }
                }
                else
                {
                    this.logger.Warn("GetSplitUserData: userdata key is empty/null");
                }
            }
            catch (Exception generalException)
            {
                this.logger.Error("GetSplitUserData : Error Occurred while Collecting user data : " + generalException.ToString());
            }
            return null;
        }

        private void GetUpdateResponseData(string ixnId, string objectName, string activityRecordId)
        {
            try
            {
                this.logger.Info("GetUpdateResponseData : Response received from SFDC ");
                this.logger.Info("GetUpdateResponseData : Interaction Id : " + ixnId);
                this.logger.Info("GetUpdateResponseData : Object Name : " + objectName);
                this.logger.Info("GetUpdateResponseData : New Activity Record Id : " + activityRecordId);

                if (!String.IsNullOrEmpty(ixnId) && !String.IsNullOrEmpty(objectName))
                {
                    if (!String.IsNullOrEmpty(activityRecordId))
                    {
                        if (Settings.UpdateSFDCLogFinishedEvent.ContainsKey(ixnId))
                        {
                            SFDCData sfdcData = Settings.UpdateSFDCLogFinishedEvent[ixnId];
                            switch (objectName.ToLower())
                            {
                                case "lead":
                                    if (sfdcData.LeadData != null)
                                    {
                                        sfdcData.LeadData.ActivityRecordID = activityRecordId;
                                    }
                                    break;

                                case "contact":
                                    if (sfdcData.ContactData != null)
                                    {
                                        sfdcData.ContactData.ActivityRecordID = activityRecordId;
                                    }
                                    break;

                                case "account":
                                    if (sfdcData.AccountData != null)
                                    {
                                        sfdcData.AccountData.ActivityRecordID = activityRecordId;
                                    }
                                    break;

                                case "case":
                                    if (sfdcData.CaseData != null)
                                    {
                                        sfdcData.CaseData.ActivityRecordID = activityRecordId;
                                    }
                                    break;

                                case "opportunity":
                                    if (sfdcData.OpportunityData != null)
                                    {
                                        sfdcData.OpportunityData.ActivityRecordID = activityRecordId;
                                    }
                                    break;

                                case "useractivity":
                                    if (sfdcData.UserActivityData != null)
                                    {
                                        sfdcData.UserActivityData.RecordID = activityRecordId;
                                    }
                                    break;

                                case "profileactivity":
                                    break;

                                default:
                                    if (sfdcData.CustomObjectData != null)
                                    {
                                        if (objectName.Contains("__c") && Settings.CustomObjectNames.ContainsKey(objectName))
                                        {
                                            string objName = Settings.CustomObjectNames[objectName];

                                            foreach (ICustomObject custonObj in sfdcData.CustomObjectData)
                                            {
                                                if (custonObj.ObjectName == objectName)
                                                {
                                                    custonObj.ActivityRecordID = activityRecordId;
                                                    break;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            this.logger.Info("GetClickToDialLogs : Custom Object not found with Name : " + objectName);
                                        }
                                    }
                                    break;
                            }
                            ConsolidateResponseIds(ixnId, objectName, activityRecordId);
                            VoiceEvents.GetInstance().ProcessUpdateData(ixnId, sfdcData);
                        }
                        else
                        {
                            ConsolidateResponseIds(ixnId, objectName, activityRecordId);
                        }
                    }
                }
            }
            catch (Exception generalException)
            {
                this.logger.Error("GetUpdateResponseData : Error occurred : " + generalException.ToString());
            }
        }

        private string TruncateNumbers(string source, int tail_length)
        {
            try
            {
                if (tail_length < 0)
                {
                    return source;
                }

                if (tail_length >= source.Length)
                {
                    return source;
                }
                return source.Substring(source.Length - tail_length);
            }
            catch (Exception generalException)
            {
                this.logger.Error("TruncateNumbers : Error occured while Truncating incoming data from Salesforce :" + generalException.ToString());
            }
            return string.Empty;
        }

        #endregion Methods

        #region Other

        //public XmlElement[] GetCustomObjectRecordData(KeyValueCollection activityLog, IMessage Message, SFDCCallType callType)
        //{
        //    try
        //    {
        //        this.logger.Info("GetCustomObjectRecordData : Collecting Record Data for the callType : " + callType.ToString());
        //        if (activityLog != null)
        //        {
        //            string keyPrefix = string.Empty;
        //            dynamic interactionEvent = null;
        //            int index = 0;
        //            System.Xml.XmlElement[] Fields = null;
        //            if (activityLog.Count > 0)
        //                Fields = new System.Xml.XmlElement[activityLog.AllValues.Length];
        //            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
        //            #region EventType
        //            if (callType == SFDCCallType.Inbound || callType == SFDCCallType.ConsultReceived)
        //            {
        //                keyPrefix = "inb";
        //            }
        //            else if (callType == SFDCCallType.OutboundSuccess)
        //            {
        //                keyPrefix = "out.success";
        //            }
        //            else if (callType == SFDCCallType.OutboundFailure)
        //            {
        //                keyPrefix = "out.fail";
        //            }
        //            else if (callType == SFDCCallType.ConsultSuccess)
        //            {
        //                keyPrefix = "con.success";
        //            }
        //            else if (callType == SFDCCallType.ConsultFailure)
        //            {
        //                keyPrefix = "con.fail";
        //            }
        //            #endregion
        //            #region Event Initialize
        //            switch (Message.Id)
        //            {
        //                case EventRinging.MessageId:
        //                    interactionEvent = (EventRinging)Message;
        //                    break;
        //                case EventEstablished.MessageId:
        //                    interactionEvent = (EventEstablished)Message;
        //                    break;
        //                case EventReleased.MessageId:
        //                    interactionEvent = (EventReleased)Message;
        //                    break;
        //                case EventDialing.MessageId:
        //                    interactionEvent = (EventDialing)Message;
        //                    break;
        //                case EventError.MessageId:
        //                    interactionEvent = (EventError)Message;
        //                    break;
        //                case EventAbandoned.MessageId:
        //                    interactionEvent = (EventAbandoned)Message;
        //                    break;
        //                case EventDestinationBusy.MessageId:
        //                    interactionEvent = (EventDestinationBusy)Message;
        //                    break;
        //                case Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events.EventInvite.MessageId:
        //                    interactionEvent = (Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events.EventInvite)Message;
        //                    break;
        //                case Genesyslab.Platform.WebMedia.Protocols.BasicChat.Events.EventSessionInfo.MessageId:
        //                    interactionEvent = (Genesyslab.Platform.WebMedia.Protocols.BasicChat.Events.EventSessionInfo)Message;
        //                    break;
        //                default:
        //                    break;
        //            }
        //            #endregion
        //            foreach (KeyValueCollection temp1 in activityLog.AllValues)
        //            {
        //                string value = string.Empty;
        //                updatefield = false;
        //                enableTimeStamp = false;
        //                string fieldName = temp1.GetAsString("field-name");
        //                string fieldType = temp1.GetAsString("field-type");
        //                if (!String.IsNullOrEmpty(temp1.GetAsString("enable.update")) && temp1.GetAsString("enable.update").Trim().ToLower() == "true")
        //                {
        //                    updatefield = true;
        //                }
        //                if (!String.IsNullOrEmpty(temp1.GetAsString("enable.time-stamp")) && temp1.GetAsString("enable.time-stamp").Trim().ToLower() == "true")
        //                {
        //                    enableTimeStamp = true;
        //                }
        //                if (!String.IsNullOrEmpty(fieldName))
        //                {
        //                    if (!updatefield)
        //                    {
        //                        if (fieldName.Equals("RecordTypeId"))
        //                        {
        //                            if (!String.IsNullOrEmpty(temp1.GetAsString("record-type.id")))
        //                            {
        //                                fieldName = fieldName.Trim();
        //                                value += temp1.GetAsString("record-type.id");
        //                            }
        //                        }
        //                        else
        //                        {
        //                            fieldName = fieldName.Trim();
        //                            if (!String.IsNullOrEmpty(fieldType))
        //                            {
        //                                fieldType = fieldType.Trim().ToLower();
        //                            }
        //                            else
        //                            {
        //                                fieldType = "datetime";
        //                                enableTimeStamp = true;
        //                            }
        //                            if (fieldType == "text" || fieldType == "number")
        //                            {
        //                                if (!String.IsNullOrEmpty(temp1.GetAsString(keyPrefix + ".user-data.key-name")) && interactionEvent.UserData != null)
        //                                {
        //                                    value += interactionEvent.UserData.GetAsString(temp1.GetAsString(keyPrefix + ".user-data.key-name"));
        //                                }
        //                                else if (!String.IsNullOrEmpty(temp1.GetAsString(keyPrefix + ".attrib.key-name")))
        //                                {
        //                                    value += GetAttributeValueForLog(temp1, interactionEvent, callType);
        //                                }
        //                                else if (!String.IsNullOrEmpty(temp1.GetAsString(keyPrefix + ".default.value")))
        //                                {
        //                                    value += temp1.GetAsString(keyPrefix + ".default.value");
        //                                }
        //                                if (enableTimeStamp && fieldType != "number")
        //                                {
        //                                    value += System.DateTime.Now.ToString();
        //                                }
        //                            }
        //                            if (enableTimeStamp)
        //                            {
        //                                if (fieldType == "date")
        //                                    value += System.DateTime.Now.ToString("yyyy-MM-dd");
        //                                else if (fieldType == "time")
        //                                    value += System.DateTime.Now.ToShortTimeString();
        //                                else if (fieldType == "datetime")
        //                                    value += System.DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
        //                                else if (fieldType == "date-timezone")
        //                                {
        //                                    if (!String.IsNullOrEmpty(Settings.SFDCOptions.SFDCTimeZone))
        //                                    {
        //                                        value += System.DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss") + Settings.SFDCOptions.SFDCTimeZone;
        //                                    }
        //                                    else
        //                                    {
        //                                        value += System.DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
        //                                        this.logger.Info("No TimeZone Configured....");
        //                                    }
        //                                }
        //                            }
        //                            if (fieldType == "number")
        //                            {
        //                                if (value == string.Empty || value == null)
        //                                {
        //                                    value += "0";
        //                                }
        //                            }
        //                        }
        //                    }
        //                    else
        //                    {
        //                        if (!String.IsNullOrEmpty(temp1.GetAsString("default.value")))
        //                        {
        //                            string defualt = temp1.GetAsString("default.value");
        //                            if (defualt == "date")
        //                                value += System.DateTime.Now.ToString("yyyy-MM-dd");
        //                            else if (defualt == "time")
        //                                value += System.DateTime.Now.ToShortTimeString();
        //                            else if (defualt == "datetime")
        //                                value += System.DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
        //                            else if (defualt == "date-timezone")
        //                            {
        //                                if (!String.IsNullOrEmpty(Settings.SFDCOptions.SFDCTimeZone))
        //                                {
        //                                    value += System.DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss") + Settings.SFDCOptions.SFDCTimeZone;
        //                                }
        //                                else
        //                                {
        //                                    value += System.DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
        //                                    this.logger.Info("No TimeZone Configured....");
        //                                }
        //                            }
        //                            else
        //                                value = defualt;
        //                        }
        //                    }
        //                    if (fieldName != string.Empty && value != string.Empty)
        //                    {
        //                        Fields[index] = doc.CreateElement(Convert.ToString(fieldName));
        //                        Fields[index].InnerText = Convert.ToString(value);
        //                        index++;
        //                    }
        //                    else
        //                    {
        //                        this.logger.Info("field name or field value is empty , field Name : " + fieldName + ", field value : " + value);
        //                    }
        //                }
        //            }
        //            return Fields;
        //        }
        //    }
        //    catch (Exception generalException)
        //    {
        //        this.logger.Error("GetCustomObjectRecordData : Error Occurred while Collecting Record data : " + generalException.ToString());
        //    }
        //    return null;
        //}

        #endregion Other
    }
}