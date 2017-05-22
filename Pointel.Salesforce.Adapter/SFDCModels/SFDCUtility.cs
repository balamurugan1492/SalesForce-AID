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
using Genesyslab.Platform.Contacts.Protocols.ContactServer.Events;
using Pointel.Salesforce.Adapter.Configurations;
using Pointel.Salesforce.Adapter.LogMessage;
using Pointel.Salesforce.Adapter.PForce;
using Pointel.Salesforce.Adapter.SFDCUtils;
using Pointel.Salesforce.Adapter.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace Pointel.Salesforce.Adapter.SFDCModels
{
    internal class SFDCUtility
    {
        #region Field Declarations

        public static PForce.SforceService SForce = null;
        private static SFDCUtility currentObject = null;
        private string ConsultInitTime;
        private string ConsultInteractionId;
        private XmlElement element = null;
        private bool enableTimeStamp = false;
        private GeneralOptions generalOptions;
        private Log logger = null;
        private SFDCUtiltiyHelper sfdcUtilityHelper;
        private object CurrentSearchobject = new object();

        #endregion Field Declarations

        #region Constructor

        private SFDCUtility()
        {
            try
            {
                this.logger = Log.GenInstance();
                this.generalOptions = Settings.SFDCOptions;
                this.sfdcUtilityHelper = SFDCUtiltiyHelper.GetInstance();
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

        #endregion Constructor

        #region GetInstance

        public static SFDCUtility GetInstance()
        {
            if (currentObject == null)
            {
                currentObject = new SFDCUtility();
            }
            return currentObject;
        }

        #endregion GetInstance

        #region GetCreateActivityLogData

        public List<XmlElement> GetCreateActivityLogData(KeyValueCollection activityLog, IMessage Message, SFDCCallType callType, IXNCustomData emailData = null)
        {
            try
            {
                if (emailData == null)
                {
                    emailData = new IXNCustomData();
                }
                this.logger.Info("GetVoiceActivityLog : Collecting Activity Logs for the callType : " + callType.ToString());

                if (activityLog != null)
                {
                    List<XmlElement> ActivityHistory = new List<XmlElement>();
                    CustomMediaType currentMedia = sfdcUtilityHelper.GetCurrentInteractionType(callType);
                    string keyPrefix = string.Empty;
                    dynamic interactionEvent = null;
                    if (Message != null)
                        interactionEvent = Convert.ChangeType(Message, Message.GetType());

                    System.Xml.XmlDocument doc = new System.Xml.XmlDocument();

                    keyPrefix = sfdcUtilityHelper.GetPrefixString(callType);
                    foreach (KeyValueCollection temp1 in activityLog.AllValues)
                    {
                        if (temp1.GetValueAsBoolean("enable.update", false) || temp1.GetValueAsBoolean("enable.append", false))
                        {
                            continue;
                        }

                        string value = string.Empty;
                        element = null;
                        enableTimeStamp = false;
                        bool enableInlineAppend = false;
                        if (temp1.GetValueAsBoolean("enable.inline-append", false))
                            enableInlineAppend = true;

                        string fieldName = temp1.GetAsString("field-name");
                        string fieldType = temp1.GetAsString("field-type");
                        if (temp1.GetValueAsBoolean("enable.time-stamp", false))
                        {
                            enableTimeStamp = true;
                        }
                        if (!String.IsNullOrEmpty(fieldName))
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
                                    if (currentMedia == CustomMediaType.Voice)
                                    {
                                        value += sfdcUtilityHelper.GetLogDataForVoice(temp1, interactionEvent, keyPrefix, callType);
                                        if (enableInlineAppend)
                                            value += "  " + sfdcUtilityHelper.GetVoiceInlineAppendData(temp1, interactionEvent, keyPrefix, callType);
                                    }
                                    else if (currentMedia == CustomMediaType.Chat)
                                    {
                                        value += sfdcUtilityHelper.GetLogDataForChat(temp1, emailData, keyPrefix, callType);
                                        if (enableInlineAppend)
                                            value += "  " + sfdcUtilityHelper.GetChatInlineAppendData(temp1, emailData, keyPrefix, callType);
                                    }
                                    else if (currentMedia == CustomMediaType.Email)
                                    {
                                        value += sfdcUtilityHelper.GetLogDataForEmail(temp1, emailData, keyPrefix, callType);
                                        if (enableInlineAppend)
                                            value += "  " + sfdcUtilityHelper.GetEmailInlineAppendData(temp1, emailData, keyPrefix, callType);
                                    }

                                    if (enableTimeStamp && fieldType != "number")
                                    {
                                        value += "  " + System.DateTime.Now.ToString();
                                        if (currentMedia == CustomMediaType.Voice && callType == SFDCCallType.ConsultVoiceReceived && fieldName.ToLower().Equals("subject"))
                                        {
                                            if (ConsultInteractionId != interactionEvent.ConnID.ToString())
                                            {
                                                ConsultInteractionId = interactionEvent.ConnID.ToString();
                                                ConsultInitTime = "  " + System.DateTime.Now.ToString();
                                            }
                                        }
                                    }
                                }
                                else if (fieldType.Equals("dispositioncode"))
                                {
                                    if (emailData.DispositionCode != null)
                                        value += emailData.DispositionCode.Item2;
                                }
                                else if (fieldType.Equals("emailcontent"))
                                {
                                    if (emailData.InteractionContents != null)
                                        value += emailData.InteractionContents.Text;
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
                                if (fieldType.Equals("number") && string.IsNullOrEmpty(value))
                                {
                                    value += "0";
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

        #endregion GetCreateActivityLogData

        #region GetUpdateActivityLogData

        public List<XmlElement> GetUpdateActivityLogData(KeyValueCollection activityLog, IMessage Message, SFDCCallType callType, string duration,
            string chatContent = null, IXNCustomData emailData = null, bool isAppendLogData = false, string voiceComments = null)
        {
            try
            {
                this.logger.Info("GetVoiceUpdateActivityLog : Collection update log for the callType : " + callType.ToString());
                if (activityLog != null)
                {
                    if (emailData == null)
                    {
                        emailData = new IXNCustomData();
                    }
                    List<XmlElement> UpdateLogElements = new List<XmlElement>();
                    string keyPrefix = string.Empty;
                    CustomMediaType currentMedia = sfdcUtilityHelper.GetCurrentInteractionType(callType);
                    dynamic voiceEvent = null;
                    if (Message != null)
                        voiceEvent = Convert.ChangeType(Message, Message.GetType());
                    System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
                    keyPrefix = sfdcUtilityHelper.GetPrefixString(callType);
                    foreach (KeyValueCollection temp1 in activityLog.AllValues)
                    {
                        if (isAppendLogData)
                        {
                            if (!temp1.GetValueAsBoolean("enable.append", false))
                                continue;
                        }
                        else if (!temp1.GetValueAsBoolean("enable.update", false))
                            continue;

                        enableTimeStamp = false;
                        string value = string.Empty;
                        string fieldName = temp1.GetAsString("field-name");
                        string fieldType = temp1.GetAsString("field-type");
                        if (temp1.GetValueAsBoolean("enable.time-stamp", false))
                        {
                            enableTimeStamp = true;
                        }
                        if (!String.IsNullOrEmpty(fieldName))
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
                                    if (currentMedia == CustomMediaType.Voice)
                                        value += sfdcUtilityHelper.GetLogDataForVoice(temp1, voiceEvent, keyPrefix, callType);
                                    else if (currentMedia == CustomMediaType.Chat)
                                        value += sfdcUtilityHelper.GetLogDataForChat(temp1, emailData, keyPrefix, callType);
                                    else if (currentMedia == CustomMediaType.Email)
                                        value += sfdcUtilityHelper.GetLogDataForEmail(temp1, emailData, keyPrefix, callType);

                                    if (enableTimeStamp && fieldType != "number")
                                    {
                                        if (currentMedia == CustomMediaType.Voice && callType == SFDCCallType.ConsultVoiceReceived && fieldName.ToLower().Equals("subject")
                                            && Settings.SFDCOptions.IsEnabledConsultSubjectWithInitDateTime)
                                            value += ConsultInitTime;
                                        else
                                            value += System.DateTime.Now.ToString();
                                    }
                                }
                                else if (fieldType.Equals("duration"))
                                    value += duration;
                                else if (fieldType.Equals("chatcontent"))
                                {
                                    if (emailData.InteractionContents != null)
                                        value += sfdcUtilityHelper.GetFormattedChatData(emailData.InteractionContents.StructuredText, emailData.InteractionId);
                                }
                                else if (fieldType.Equals("emailcontent"))
                                {
                                    if (emailData.InteractionContents != null)
                                        value += emailData.InteractionContents.Text;
                                }
                                else if (fieldType.Equals("dispositioncode"))
                                {
                                    if (emailData.DispositionCode != null)
                                    {
                                        value += emailData.DispositionCode.Item2;
                                    }
                                }
                                else if (fieldType.Equals("comments"))
                                {
                                    if (voiceComments != null)
                                    {
                                        value += voiceComments;
                                    }
                                    else
                                    {
                                        value += emailData.InteractionNotes;
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
                                if (fieldType.Equals("number") && string.IsNullOrEmpty(value))
                                {
                                    value += "0";
                                }
                            }
                            if (fieldName != string.Empty)//&& value != string.Empty)Empty value allowed as ram requested to update empty disposition on 10 march 2016
                            {
                                element = doc.CreateElement(Convert.ToString(fieldName));
                                element.InnerText = Convert.ToString(value);
                                UpdateLogElements.Add(element);
                            }
                            else
                            {
                                this.logger.Info("Log value is empty , field Name : " + fieldName + ", field value : " + value);
                            }
                        }
                    }
                    return UpdateLogElements;
                }
            }
            catch (Exception generalException)
            {
                this.logger.Error("GetVoiceUpdateActivityLog : Error Occurred while Collecting activity Logs : " + generalException.ToString());
            }
            return null;
        }

        #endregion GetUpdateActivityLogData

        #region SearchSFDC

        public OutputValues SearchSFDC(string searchData, string searchCondition, string searchObjects, string searchFormat, string connID, SFDCData popupData, SFDCCallType callType)
        {
            try
            {
                lock (CurrentSearchobject)
                {
                    this.logger.Info("SearchSFDC: SearchData=" + searchData + "\t SearchObjects=" + searchObjects +
                        "\t SearchCondition=" + searchCondition);
                    OutputValues output = new OutputValues();
                    output.ObjectName = searchObjects;
                    output.ConnID = connID;
                    output.PopupData = popupData;
                    output.CallType = callType;

                    if (String.IsNullOrEmpty(SForce.SessionHeaderValue.sessionId))
                    {
                        SendSesssionStatusToClient();
                        return null;
                    }
                    if (String.IsNullOrWhiteSpace(searchData) || String.IsNullOrWhiteSpace(searchObjects))
                    {
                        this.logger.Info("Can not perform SFDC search at this time because either search data or search object is null");
                        // allow no record action to custom object only
                        if (!String.IsNullOrEmpty(searchObjects) && searchObjects.Contains("__c"))
                        {
                            this.logger.Info("Invoking NoRecordFoundScenario for the custom object " + searchObjects);
                            return output;
                        }
                    }
                    // check if advanced search enabled
                    if (Settings.SFDCOptions.EnableAdvancedSearch)
                    {
                        if (Settings.AdSearchList != null && Settings.AdSearchList.Count > 0)
                        {
                            foreach (AdSearch adsearch in Settings.AdSearchList)
                            {
                                DoAdvancedSearch(searchData, adsearch, output);
                                if (output.IsMatchFound)
                                {
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(searchFormat))
                        {
                            foreach (string format in searchFormat.Split(','))
                            {
                                DoOrdinarySearch(searchData, searchObjects, searchCondition, output, format);
                                if (output.IsMatchFound)
                                {
                                    break;
                                }
                            }
                        }
                        else
                        {
                            DoOrdinarySearch(searchData, searchObjects, searchCondition, output);
                        }
                    }

                    PrintResponseInLog(output.SearchRecord, output.Query);

                    return output;
                }
            }
            catch (Exception generalException)
            {
                this.logger.Error("Error occurred while invoking SFDCSearch: " + generalException);
            }
            return null;
        }

        public void DoAdvancedSearch(string originalSearchData, AdSearch adsearch, OutputValues output)
        {
            try
            {
                this.logger.Info("DoAdvancedSearch() :" + originalSearchData);

                if (adsearch.SKipSearchData != null && adsearch.SKipSearchData.Contains(originalSearchData))
                {
                    this.logger.Info("current seeach data is available in SKIP Search data configurations,..");
                    output.IsSearchCancelled = true;
                    return;
                }
                if (adsearch.SearchDataType != ADSearchDataType.None && !CheckSearchDataType(adsearch.SearchDataType, originalSearchData))
                {
                    this.logger.Info("current search data type is not supported in advaced search configurations..");
                    output.IsSearchCancelled = true;
                    return;
                }
                if (adsearch.SearchDataLength != 0 && originalSearchData.Length != adsearch.SearchDataLength)
                {
                    this.logger.Info("current search data length is not supported in advaced search configurations..");
                    output.IsSearchCancelled = true;
                    return;
                }

                if (adsearch.SearchDataFormat != null && adsearch.SearchDataFormat.Length > 0)
                {
                    foreach (string format in adsearch.SearchDataFormat)
                    {
                        var searchString = sfdcUtilityHelper.GetSearchString(originalSearchData, adsearch.SearchDelimiter, format);
                        if (!String.IsNullOrEmpty(searchString))
                        {
                            DoAdvSearch(adsearch, searchString, output);
                            if (output.IsMatchFound)
                            {
                                break;
                            }
                        }
                    }
                }
                else
                {
                    DoAdvSearch(adsearch, originalSearchData, output);
                }
            }
            catch (Exception generalException)
            {
                this.logger.Error("Error occurred at DoAdvancedSearch(), details:" + generalException);
            }
        }

        private bool CheckSearchDataType(ADSearchDataType type, string searchData)
        {
            switch (type)
            {
                case ADSearchDataType.Numeric:
                    return Regex.IsMatch(searchData, @"^\d+$");

                case ADSearchDataType.AlphaNumeric:
                    Regex rg = new Regex(@"^[a-zA-Z0-9\s,]*$");
                    return rg.IsMatch(searchData);

                default:
                    break;
            }
            return false;
        }

        private SearchResult InvokeSOSLQuery(string query, OutputValues output)
        {
            try
            {
                this.logger.Info("************Request from Adapter***********************");
                this.logger.Info("SOSL Query : " + query);
                this.logger.Info("***************************************************");
                return SForce.search(query);
            }
            catch (Exception generalException)
            {
                this.logger.Error("InvokeSOSLQuery: Exception thrown while requesting Salesforce Service : " + generalException.ToString() + generalException.StackTrace);
                if (generalException.Message.Contains("INVALID_SESSION_ID"))
                {
                    HandleSessionExpiry(output);
                    Settings.SFDCListener.SendSessionStatus(SFDCSessionStatus.NotConnected);
                }
                if (Settings.SFDCOptions.SOAPAPIErrorMessageDisplay)
                {
                    Settings.SFDCListener.SFDCConnectionStatus(LogMode.Error, "Exception thrown while requesting Salesforce Service\n" + generalException.Message);
                }
            }
            return null;
        }

        private SearchResult InvokeSOQLQuery(string soslQuery, OutputValues output)
        {
            SearchResult result = null;
            try
            {
                this.logger.Info("************Request from Adapter***********************");
                this.logger.Info("SOQL Query : " + soslQuery);
                QueryResult qResult = SForce.query(soslQuery);
                if (qResult != null && qResult.records != null)
                {
                    SearchRecord[] srecord = new SearchRecord[qResult.records.Length];
                    for (int i = 0; i < qResult.records.Length; i++)
                    {
                        srecord[i] = new SearchRecord();
                        srecord[i].record = qResult.records[i];
                    }
                    result = new SearchResult();
                    result.searchRecords = srecord;
                }
            }
            catch (Exception generalException)
            {
                this.logger.Error("InvokeSOQLQuery: Exception thrown while requesting Salesforce Service : " + generalException.ToString() + generalException.StackTrace);
                if (generalException.Message.Contains("INVALID_SESSION_ID"))
                {
                    HandleSessionExpiry(output);
                    Settings.SFDCListener.SendSessionStatus(SFDCSessionStatus.NotConnected);
                }
                if (Settings.SFDCOptions.SOAPAPIErrorMessageDisplay)
                {
                    Settings.SFDCListener.SFDCConnectionStatus(LogMode.Error, "Exception thrown while requesting Salesforce Service\n" + generalException.Message);
                }
            }
            return result;
        }

        private void PrintResponseInLog(SearchRecord[] searchRecords, string soslQuery)
        {
            try
            {
                if (searchRecords != null)
                {
                    this.logger.Info("##################Response from Salesforce####################");
                    this.logger.Info("Response from Salesforce for the Query : " + soslQuery);
                    foreach (var i in searchRecords)
                    {
                        this.logger.Info("Record Type: " + i.record.type + "\t Record Id :" + i.record.Id);
                    }
                    this.logger.Info("#########################################################");
                }
            }
            catch
            {
            }
        }

        private void SendSesssionStatusToClient()
        {
            this.logger.Error("Can not perform search at this time because SFDC SessionId is null....");
            if (SFDCHttpServer.connectionStatus)
            {
                Settings.SFDCListener.SendSessionStatus(SFDCSessionStatus.NotConnected);

                this.logger.Error("Requesting for Session Id from Salesforce.....");
                SFDCHttpServer.SessionFlag = true;
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
        }

        private string GetCustomSearchQuery(AdSearch adsearch, string searchData)
        {
            this.logger.Info("GetCustomSearchQuery()");
            string soqlQuery = string.Empty;
            try
            {
                if (adsearch.EnableCustomQuery)
                {
                    if (!string.IsNullOrWhiteSpace(adsearch.CustomQuery))
                    {
                        if (adsearch.CustomQuery.Contains("searchdata"))
                        {
                            soqlQuery = adsearch.CustomQuery.Replace("searchdata", searchData);
                            return soqlQuery;
                        }
                        else
                        {
                            this.logger.Info("Custom query does not contain key word - searchdata");
                        }
                    }
                    else
                    {
                        this.logger.Info("Custom query is empty");
                    }
                }

                if (!string.IsNullOrWhiteSpace(adsearch.LookupFields))
                {
                    string condition = string.Empty;
                    string[] lookupFields = adsearch.LookupFields.Split(',');
                    if (lookupFields.Length > 1)
                    {
                        for (int i = 0; i < lookupFields.Length; i++)
                        {
                            condition += lookupFields[i] + " = '" + searchData + "'";
                            if (i < lookupFields.Length - 1)
                            {
                                condition += " " + adsearch.SearchDelimiter + " ";
                            }
                        }
                    }
                    else
                    {
                        condition = adsearch.LookupFields + " = '" + searchData + "'";
                    }

                    if (!string.IsNullOrWhiteSpace(adsearch.ResponseFields))
                    {
                        soqlQuery = "SELECT " + adsearch.ResponseFields + " FROM " + adsearch.LookupObjects + " WHERE " + condition;
                    }
                    else
                    {
                        soqlQuery = "SELECT Id FROM " + adsearch.LookupObjects + " WHERE " + condition;
                    }
                }
            }
            catch (Exception generalException)
            {
                this.logger.Error("Error occurred while building custom search query, details:" + generalException);
            }

            return soqlQuery;
        }

        private void DoFilterRecords(SearchResult searchResult, AdSearch adSearch, string searchData)
        {
            try
            {
                this.logger.Info("DoFilterRecords()");
                List<SearchRecord> currentFoundFList = null;
                /// print records count before filter applied

                if (searchResult != null && searchResult.searchRecords != null)
                {
                    PrintResponseInLog(searchResult.searchRecords, "Before filter records: " + searchData);

                    foreach (SearchRecord localrecord in searchResult.searchRecords)
                    {
                        foreach (XmlElement element in localrecord.record.Any)
                        {
                            if (element.LocalName.Equals(adSearch.FilterResultFields))
                            {
                                // remove special charector
                                if (element.InnerText != null && element.InnerText.RemoveSpecialCharacters().Equals(searchData))
                                {
                                    if (currentFoundFList == null)
                                        currentFoundFList = new List<SearchRecord>();

                                    currentFoundFList.Add(localrecord);
                                    this.logger.Info("Exact match-record found with the following search data :" + searchData);
                                    this.logger.Info("Exact match-record Id :" + localrecord.record.Id);
                                }
                                break;
                            }
                        }
                    }
                    if (currentFoundFList != null)
                        searchResult.searchRecords = currentFoundFList.ToArray();
                    else
                    {
                        this.logger.Info("No Exact match-record found with the filter condition :" + searchData);
                    }
                }
            }
            catch (Exception generalException)
            {
                this.logger.Error("Error occurred while filtering response records, details:" + generalException);
            }
        }

        private void DoAdvSearch(AdSearch adsearch, string searchData, OutputValues output)
        {
            SearchResult result = null;
            try
            {
                this.logger.Info("DoAdvSearch():" + searchData);
                string searchQuery = string.Empty;
                if (adsearch.SearchFields == SearchFieldType.Custom)
                {
                    searchQuery = GetCustomSearchQuery(adsearch, searchData);
                    // do search
                    if (!string.IsNullOrWhiteSpace(searchQuery))
                    {
                        result = InvokeSOQLQuery(searchQuery, output);
                    }
                    else
                        this.logger.Info("Unable to invoke advanced search, because the query is null or empty..");
                }
                else
                {
                    searchQuery = "FIND {" + searchData + "} IN " + adsearch.SearchFields.ToString() + " FIELDS RETURNING ";
                    if (!string.IsNullOrWhiteSpace(adsearch.ResponseFields) && adsearch.ResponseFields.Contains("("))
                    {
                        searchQuery += adsearch.ResponseFields;
                    }
                    else
                    {
                        searchQuery += adsearch.LookupObjects;
                    }

                    result = InvokeSOSLQuery(searchQuery, output);

                    if (result != null && result.searchRecords != null)
                    {
                        //do filter response based on configuration
                        if (adsearch.EnableFilterResults && result.searchRecords.Length > 1)
                            DoFilterRecords(result, adsearch, searchData);
                        else
                            this.logger.Info("Response filter is not applied,because result record count: " + result.searchRecords.Length);
                    }
                }

                if (result != null)
                {
                    if (result.searchRecords != null && result.searchRecords.Length > 0)
                        output.IsMatchFound = true;

                    output.SearchRecord = result.searchRecords;
                }
                output.SearchData = searchData;
                output.ObjectName = adsearch.LookupObjects;
                output.Query = searchQuery;
            }
            catch (Exception generalException)
            {
                this.logger.Error("Error occurred while Do AdvSearch :" + generalException);
            }
        }

        private void DoOrdinarySearch(string searchData, string searchObjects, string searchCondition, OutputValues output, string format = null)
        {
            try
            {
                this.logger.Info("DoOrdinarySearch():" + searchData);
                var searchString = sfdcUtilityHelper.GetSearchString(searchData, searchCondition, format);
                if (!String.IsNullOrEmpty(searchString))
                {
                    var soslQuery = "FIND {" + searchString + "} IN All FIELDS RETURNING " + searchObjects;
                    SearchResult result = InvokeSOSLQuery(soslQuery, output);

                    if (result != null && result.searchRecords != null && result.searchRecords.Length > 0)
                    {
                        output.IsMatchFound = true;
                    }
                    else
                    {
                        output.IsMatchFound = false;
                    }
                    output.SearchData = searchString;
                    output.SearchRecord = result.searchRecords;
                    output.Query = soslQuery;
                }
                else
                {
                    this.logger.Warn("DoOrdinarySearch(): SearchData is null or empty :" + searchData);
                }
            }
            catch (Exception generalException)
            {
                this.logger.Error("Error occurred while invoking SOSL Query: " + generalException);
            }
        }

        #endregion SearchSFDC

        #region GetPreviousContent of SFDC Record Field

        public string GetPreviousContent(string TaskId, string objectName, string fieldName)
        {
            try
            {
                this.logger.Info("GetPreviousContent: Object Type :" + objectName + "\t Record Id :" + TaskId + "\t Field Name :" + fieldName);
                if (!string.IsNullOrWhiteSpace(TaskId) && !string.IsNullOrWhiteSpace(fieldName))
                {
                    sObject[] response = SForce.retrieve(fieldName, objectName, new string[] { TaskId });
                    if (response != null)
                    {
                        foreach (XmlElement element in response[0].Any)
                        {
                            if (element.LocalName.Equals(fieldName))
                            {
                                this.logger.Info("+++++Response from salesforce+++++++");
                                this.logger.Info("Field Name: " + fieldName + " \t Value: " + element.InnerText);
                                this.logger.Info("+++++Response from salesforce+++++++");
                                return element.InnerText;
                            }
                        }
                    }
                    else
                    {
                        this.logger.Info("+++++Response from salesforce+++++++");
                        this.logger.Info("Null response recieved for the field : " + fieldName);
                        this.logger.Info("+++++Response from salesforce+++++++");
                    }
                }
                else
                {
                    this.logger.Info("can not retrieve previous content becuase either task id or field name is empty");
                }
            }
            catch (Exception generalException)
            {
                this.logger.Error("Error occurred while retrieving " + fieldName + " field data of " + objectName + " object, Exception :" + generalException.ToString());
                throw;
            }
            return string.Empty;
        }

        #endregion GetPreviousContent of SFDC Record Field

        #region CreateNewRecord

        public string CreateNewRecord(string interactionId, List<XmlElement> taskFields, string objectName, int retryAttempt = 0)
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
                        foreach (XmlElement element in taskFields)
                        {
                            if (element != null)
                                log += "\tField Name : " + element.Name + "\t Field Value : " + element.InnerText + "\n";
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
                    sobject.Any = taskFields.ToArray();
                    PForce.sObject[] array = new sObject[1];
                    array[0] = sobject;
                    SaveResult[] saveResult = null;
                    this.logger.Info("************Request from WDE***********************");
                    this.logger.Info("Request sent to salesforce to create New Record for the Object : " + objectName);
                    this.logger.Info("************Request from WDE***********************");
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
                    SFDCTaskData currentTask = new SFDCTaskData
                    {
                        InteractionId = interactionId,
                        TaskFields = taskFields,
                        ObjectName = objectName,
                        CallType = SFDCCallType.Unknown,
                        RetryAttempt = retryAttempt,
                        Action = TaskAction.RecordCreate
                    };
                    HandleSessionExpiry(currentTask);
                    Settings.SFDCListener.SendSessionStatus(SFDCSessionStatus.NotConnected);
                }
                if (Settings.SFDCOptions.SOAPAPIErrorMessageDisplay)
                {
                    Settings.SFDCListener.SFDCConnectionStatus(LogMode.Error, "Error Returned while Creating New Record for the Type :" + objectName + "\nMessage : " + generalException.Message);
                }
            }
            return null;
        }

        #endregion CreateNewRecord

        #region CreateActivityLog

        public string CreateActivityLog(string interactionId, List<XmlElement> taskFields, string objectName, SFDCCallType callType, string recordId = null, int retryAttempt = 0)
        {
            try
            {
                bool isUserlevel = false;
                if (string.IsNullOrWhiteSpace(recordId))
                    isUserlevel = true;
                if (isUserlevel)
                    this.logger.Info("CreateActivityLog: Create Profile level Activity Log Invoked for the Object :" + objectName + " Interaction Id :" + interactionId);
                else
                    this.logger.Info("CreateActivityLog: Create Activity Log Invoked for the Object :" + objectName + " record id : " + recordId + " and  Interaction Id :" +
                        interactionId);
                try
                {
                    if (taskFields != null)
                    {
                        XmlDocument document = new XmlDocument();

                        if (!string.IsNullOrWhiteSpace(recordId))
                        {
                            switch (objectName.ToLower())
                            {
                                case "lead":
                                case "contact":
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
                        string log = "\n ++++++++++++create log data++++++++++++++++ \n";
                        for (int i = 0; i < taskFields.Count; i++)
                        {
                            if (taskFields[i] != null)
                                log += "\tField Name : " + taskFields[i].Name + "\t Field Value : " + taskFields[i].InnerText + "\n";
                        }
                        log += "++++++++++++create log data++++++++++++++++";
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
                    this.logger.Info("++++++++++++++++++++Request from WDE++++++++++++++++++++++++");
                    if (isUserlevel)
                        this.logger.Info("Request sent to salesforce to create user level activity log for the Object : " + objectName);
                    else
                        this.logger.Info("Request sent to salesforce to create activity log for the Object : " + objectName + " and the record id : "
                                        + recordId);

                    this.logger.Info("++++++++++++++++++Request from WDE+++++++++++++++++++++++");
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
                        this.logger.Info("++++++++++++++++response from salesforce++++++++++++++++++++");
                        if (isUserlevel)
                            this.logger.Error("CreateActivityLog: Error Returned while Creating new user level activity log, Error Message : " + saveResult[0].errors[0].message);
                        else
                            this.logger.Error("CreateActivityLog: Error Returned while Creating new activity log for the record id  :"
                                + recordId + ", Error Message : " + saveResult[0].errors[0].message);
                        this.logger.Info("++++++++++++++++response from salesforce++++++++++++++++++++");
                        return null;
                    }
                    else
                    {
                        this.logger.Info("++++++++++++++++response from salesforce++++++++++++++++++++");
                        if (isUserlevel)
                            this.logger.Info("Response from salesforce for creating user level activitylog for the Type :" + objectName);
                        else
                            this.logger.Info("Response from salesforce for creating activitylog for the Type :" + objectName
                                + " and the record Id :" + recordId);
                        this.logger.Info("Response Id :" + saveResult[0].id);
                        this.logger.Info("++++++++++++++++response from salesforce++++++++++++++++++++");
                        Task.Run(() => GetUpdateResponseData(interactionId, objectName, callType, saveResult[0].id));
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
                        Task.Factory.StartNew(() => CreateActivityLog(interactionId, taskFields, objectName, callType, recordId, retryAttempt));
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
                    SFDCTaskData currentTask = new SFDCTaskData
                    {
                        InteractionId = interactionId,
                        TaskFields = taskFields,
                        ObjectName = objectName,
                        RecordId = recordId,
                        CallType = callType,
                        RetryAttempt = retryAttempt,
                        Action = TaskAction.TaskCreate
                    };
                    HandleSessionExpiry(currentTask);
                    Settings.SFDCListener.SendSessionStatus(SFDCSessionStatus.NotConnected);
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

        #endregion CreateActivityLog

        #region UpdateActivityLog

        public void UpdateActivityLog(string interactionId, List<XmlElement> taskFields, string objectName, string recordId, int retryAttempt = 0)
        {
            try
            {
                this.logger.Info("UpdateActivityLog: Update Activity Log Invoked for the Object :" + objectName + " record id : " + recordId + " and  Interaction Id :" +
                    interactionId);
                bool canSendRequestForLog = false;
                try
                {
                    if (taskFields != null)
                    {
                        var element = (XmlElement)taskFields.Where(item => item.Name == "Id").FirstOrDefault();
                        var element1 = taskFields.IndexOf(element);
                        if (element1 <= 0)
                        {
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
                        string log = "\n++++++++++++update log data++++++++++++++++\n";
                        for (int i = 0; i < taskFields.Count; i++)
                        {
                            if (taskFields[i] != null)
                            {
                                canSendRequestForLog = true;
                                log += "\tField Name : " + taskFields[i].Name + "\t Field Value : " + taskFields[i].InnerText + "\n";
                            }
                        }
                        log += "++++++++++++update log data++++++++++++++++";
                        this.logger.Info(log);
                    }
                }
                catch
                {
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
                    this.logger.Info("+++++++++++++++++Request from WDE++++++++++++++++++++++");
                    this.logger.Info("Request sent to salesforce to Update activity log for the Object : " + objectName + " and the record id : "
                        + recordId);
                    this.logger.Info("+++++++++++++++++++Request from WDE+++++++++++++++++++++");
                    saveResult = SForce.update(array);
                    if (!saveResult[0].success)
                    {
                        if (Settings.SFDCOptions.SOAPAPIErrorMessageDisplay)
                        {
                            Settings.SFDCListener.SFDCConnectionStatus(LogMode.Error, "Error Returned while Updating activity log for the record id :"
                                + recordId + "\n Error Message : " + saveResult[0].errors[0].message);
                        }
                        this.logger.Info("++++++++++++++++response from salesforce++++++++++++++++++++");
                        this.logger.Error("UpdateActivityLog: Error Returned while Updating new activity log for the record id  :"
                            + recordId + ", Error Message : " + saveResult[0].errors[0].message);
                        this.logger.Info("++++++++++++++++response from salesforce++++++++++++++++++++");
                    }
                    else
                    {
                        this.logger.Info("++++++++++++++++response from salesforce++++++++++++++++++++");
                        this.logger.Info("Response from salesforce for Updating activitylog for the Type :" + objectName
                            + " and the record Id :" + recordId);
                        this.logger.Info("Response Id :" + saveResult[0].id);
                        this.logger.Info("++++++++++++++++response from salesforce++++++++++++++++++++");
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
                        Task.Factory.StartNew(() => UpdateActivityLog(interactionId, taskFields, objectName, recordId, retryAttempt));
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
                    generalException.Message);

                if (generalException.Message.Contains("INVALID_SESSION_ID"))
                {
                    SFDCTaskData currentTask = new SFDCTaskData
                    {
                        InteractionId = interactionId,
                        TaskFields = taskFields,
                        ObjectName = objectName,
                        RecordId = recordId,
                        CallType = SFDCCallType.Unknown,
                        RetryAttempt = retryAttempt,
                        Action = TaskAction.TaskUpdate
                    };
                    HandleSessionExpiry(currentTask);
                    Settings.SFDCListener.SendSessionStatus(SFDCSessionStatus.NotConnected);
                }
                if (Settings.SFDCOptions.SOAPAPIErrorMessageDisplay)
                {
                    Settings.SFDCListener.SFDCConnectionStatus(LogMode.Error, "Error Returned while Updating activity log for the Type :" +
                        objectName + "\n Error Message : " + generalException.Message);
                }
            }
        }

        #endregion UpdateActivityLog

        #region AppendActivityLog

        public void AppendActivityLog(List<XmlElement> taskFields, string objectName, string recordId)
        {
            try
            {
                this.logger.Info("AppendActivityLog: Append Activity Log, Record Id :" + recordId);
                if (taskFields != null && recordId != null)
                {
                    foreach (XmlElement element in taskFields)
                    {
                        try
                        {
                            string data = GetPreviousContent(recordId, objectName, element.Name);
                            this.logger.Info("Previous content in the field :" + data);
                            element.InnerText = data + "\n" + element.InnerText;
                            XmlElement idElement = (new XmlDocument()).CreateElement("Id");
                            idElement.InnerText = recordId;

                            sObject sobject = new sObject();
                            sobject.type = objectName;
                            sobject.Any = new XmlElement[] { element, idElement };
                            PForce.sObject[] array = new sObject[1];
                            array[0] = sobject;
                            this.logger.Info("+++++++++++++++++Request from WDE++++++++++++++++++++++");
                            this.logger.Info("Appending Activity Log: " + objectName + "field " + element.Name + " for the record id : " + recordId);
                            this.logger.Info("+++++++++++++++++++Request from WDE+++++++++++++++++++++");

                            SaveResult[] saveResult = SForce.update(array);

                            if (saveResult != null && saveResult[0].success)
                            {
                                this.logger.Info("Successfully appended");
                            }
                            else
                            {
                                if (saveResult != null)
                                    this.logger.Info("Appending Activity Log failed :" + saveResult[0].errors[0].message);
                            }
                        }
                        catch (Exception generalException)
                        {
                            this.logger.Error("Error occurred while appending activity log for " + element.Name + ", Exception :" + generalException.ToString());

                            if (generalException.Message.Contains("INVALID_SESSION_ID"))
                            {
                                SFDCTaskData currentTask = new SFDCTaskData
                                {
                                    TaskFields = taskFields,
                                    ObjectName = objectName,
                                    RecordId = recordId,
                                    CallType = SFDCCallType.Unknown,
                                    Action = TaskAction.TaskAppend
                                };
                                HandleSessionExpiry(currentTask);
                                Settings.SFDCListener.SendSessionStatus(SFDCSessionStatus.NotConnected);
                            }
                            if (Settings.SFDCOptions.SOAPAPIErrorMessageDisplay)
                            {
                                Settings.SFDCListener.SFDCConnectionStatus(LogMode.Error, "Error Returned while append activity log for the Type :" + objectName + "\nMessage : " + generalException.Message);
                            }
                        }
                    }
                }
            }
            catch (Exception generalException)
            {
                this.logger.Error("Error occurred while appending data, Exception :" + generalException.ToString());
            }
        }

        #endregion AppendActivityLog

        #region UpdateNewRecord

        public void UpdateNewRecord(string interactionId, List<XmlElement> taskFields, string objectName, string recordId, int retryAttempt = 0)
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
                        foreach (XmlElement element in taskFields)
                        {
                            if (element != null)
                            {
                                canSendRequest = true;
                                log += "\tField Name : " + element.Name + "\t Field Value : " + element.InnerText + "\n";
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

                    Task.Any = taskFields.ToArray();
                    PForce.sObject[] array = new sObject[1];
                    array[0] = Task;
                    SaveResult[] saveResult = null;
                    this.logger.Info("************Request from WDE***********************");
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
                    SFDCTaskData currentTask = new SFDCTaskData
                    {
                        InteractionId = interactionId,
                        TaskFields = taskFields,
                        ObjectName = objectName,
                        RecordId = recordId,
                        CallType = SFDCCallType.Unknown,
                        RetryAttempt = retryAttempt,
                        Action = TaskAction.RecordUpdate
                    };
                    HandleSessionExpiry(currentTask);
                    Settings.SFDCListener.SendSessionStatus(SFDCSessionStatus.NotConnected);
                }
                if (Settings.SFDCOptions.SOAPAPIErrorMessageDisplay)
                {
                    Settings.SFDCListener.SFDCConnectionStatus(LogMode.Error, "Error Returned while Updating Record for the Type :" + objectName + "\nMessage : " + generalException.Message);
                }
            }
        }

        #endregion UpdateNewRecord

        #region Test Request To Salesforce

        public void SendTestRequest()
        {
            try
            {
                this.logger.Info("SendTestRequest: Trying to send test request to salesforce api........");
                if (!String.IsNullOrEmpty(SForce.SessionHeaderValue.sessionId) && !String.IsNullOrEmpty(SForce.Url))
                {
                    this.logger.Info("***********************************");
                    string Query = "FIND {1234567890} IN All FIELDS RETURNING CONTACT";
                    this.logger.Info("Search Query for test Request : " + Query);
                    SearchResult result = SForce.search(Query);
                    if (result != null)
                    {
                        SFDCHttpServer.IsFirstRequestMade = true;
                        Settings.SFDCListener.SendSessionStatus(SFDCSessionStatus.Connected);
                        this.logger.Info("Test Request has Passed and Salesforce Connected with SFDC Adapter.....");
                        // call queued popup items
                        ThreadPool.QueueUserWorkItem(new WaitCallback(InvokeQueuedPopupItems), null);
                        if (Settings.SFDCOptions.AlertSFDCConnectionStatus)
                        {
                            Settings.SFDCListener.SFDCConnectionStatus(LogMode.Info, Settings.SFDCOptions.SFDCConnectionSuccessMessage);
                        }
                    }
                    else
                    {
                        SFDCHttpServer.IsFirstRequestMade = false;
                        this.logger.Info("ERROR: Test Request to Salesforce API has Failed");
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
                    Settings.SFDCListener.SendSessionStatus(SFDCSessionStatus.NotConnected);
                }
                this.logger.Error("SendTestRequest: Error Occurred while Sending test request to salesforce........" + generalException.ToString());
            }
        }

        #endregion Test Request To Salesforce

        #region GetUpdateResponseData

        private void GetUpdateResponseData(string ixnId, string objectName, SFDCCallType callType, string activityRecordId)
        {
            try
            {
                this.logger.Info("GetUpdateResponseData : Response received from SFDC ");
                this.logger.Info("GetUpdateResponseData : Interaction Id : " + ixnId);
                this.logger.Info("GetUpdateResponseData : Object Name : " + objectName);
                this.logger.Info("GetUpdateResponseData : New Activity Record Id : " + activityRecordId);
                // attach activity id with genesys attach data
                if (!string.IsNullOrEmpty(activityRecordId))
                {
                    this.logger.Info("GetUpdateResponseData :Attach activity id with attach data");
                    string activityKeyValue = string.Empty;
                    CustomMediaType interactionType = sfdcUtilityHelper.GetCurrentInteractionType(callType);
                    if (interactionType == CustomMediaType.Voice)
                    {
                        if (generalOptions.VoiceAttachActivityId)
                        {
                            this.logger.Info("GetUpdateResponseData :Voice interaction Activity ID Attached");
                            Settings.SFDCListener.SetAttachedData(ixnId, generalOptions.VoiceAttachActivityIdKeyname, activityRecordId, 0);
                        }
                    }
                    else if (interactionType == CustomMediaType.Email)
                    {
                        if (Settings.SFDCOptions.CanAddEmailAttachmentsInLog)// check configuration for enable/disable email attachment
                        {
                            if (callType == SFDCCallType.InboundEmail)
                            {
                                AddEmailAttachment(ixnId, activityRecordId);
                            }
                        }
                        if (generalOptions.EmailAttachActivityId)
                        {
                            this.logger.Info("GetUpdateResponseData :Email interaction Activity ID Attached");
                            Settings.SFDCListener.SetAttachedData(ixnId, generalOptions.EmailAttachActivityIdKeyname, activityRecordId, Settings.EmailProxyClientId);
                        }
                    }
                    else if (interactionType == CustomMediaType.Chat)
                    {
                        if (generalOptions.ChatAttachActivityId)
                        {
                            this.logger.Info("GetUpdateResponseData :Chat interaction Activity ID Attached");
                            Settings.SFDCListener.SetAttachedData(ixnId, generalOptions.ChatAttachActivityIdKeyname, activityRecordId, Settings.ChatProxyClientId);
                        }
                    }
                }

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

                                            foreach (CustomObjectData custonObj in sfdcData.CustomObjectData)
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
                            SearchHandler.GetInstance().ProcessUpdateData(ixnId, sfdcData);
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

        #endregion GetUpdateResponseData

        #region ConsolidateResponseIds

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

        #endregion ConsolidateResponseIds

        #region Email Attachment to SFDC log

        public void CreateAttachment(string activityLogId, Genesyslab.Platform.Contacts.Protocols.ContactServer.AttachmentList attachments)
        {
            try
            {
                this.logger.Info("Trying to add attachments to the activity log id: " + activityLogId);
                if (attachments == null)
                    return;

                for (int index = 0; index < attachments.Count; index++)
                {
                    logger.Info("FileName: " + attachments[index].TheName + "\t FileSize: " + Convert.ToString(attachments[index].TheSize) + "\t Document Id: " +
                        attachments[index].DocumentId);

                    var attachmentfile = Settings.SFDCListener.GetEmailAttachment(attachments[index].DocumentId, true);
                    if (attachmentfile == null)
                    {
                        this.logger.Info("Retrieving email attachment failed, null response received from UCS..");
                        continue;
                    }

                    if (attachmentfile is EventGetDocument)
                    {
                        XmlDocument doc = new XmlDocument();
                        sObject attach = new sObject();
                        attach.type = "Attachment";
                        PForce.sObject[] array = new sObject[1];
                        SaveResult[] saveResult = null;
                        attach.type = "Attachment";
                        attach.Any = new System.Xml.XmlElement[4];
                        attach.Any[0] = doc.CreateElement("Name");
                        attach.Any[0].InnerText = attachments[index].TheName;
                        attach.Any[1] = doc.CreateElement("IsPrivate");
                        attach.Any[1].InnerText = "false";
                        attach.Any[2] = doc.CreateElement("ParentId");
                        attach.Any[2].InnerText = activityLogId;
                        attach.Any[3] = doc.CreateElement("Body");
                        attach.Any[3].InnerText = Convert.ToBase64String((attachmentfile as EventGetDocument).Content);
                        array[0] = attach;
                        try
                        {
                            saveResult = SFDCUtility.SForce.create(array);
                            if (!saveResult[0].success)
                                this.logger.Error("adding attachment failed :" + saveResult[0].errors[0].message);
                            else
                                this.logger.Info("attachment successfully added......");
                        }
                        catch (Exception generalException)
                        {
                            this.logger.Error("Error while attaching the file " + attachments[index].TheName + ", exception details:" + generalException);
                        }
                    }
                    else
                    {
                        this.logger.Info("Retrieving email attachment failed: " + attachmentfile.ToString());
                        continue;
                    }
                }
            }
            catch (Exception generalException)
            {
                this.logger.Error("Error occurred while attaching file in sfdc log, exception :" + generalException);
            }
        }

        #endregion Email Attachment to SFDC log

        #region AddEmailAttachment

        public void AddEmailAttachment(string ixnId, string activityId)
        {
            try
            {
                this.logger.Info("Trying to get email attachment for the interaction id:" + ixnId);
                var emailContent = Settings.SFDCListener.GetOpenMediaInteractionContent(ixnId, true);

                if (emailContent as EventGetInteractionContent != null)
                {
                    if (emailContent.Attachments != null)
                    {
                        CreateAttachment(activityId, emailContent.Attachments);
                    }
                    else
                    {
                        this.logger.Info("No attachments found for the interaction id:" + ixnId);
                    }
                }
                else
                {
                    this.logger.Info("Fetching InteracionContent for the InteractionId :" + ixnId + " failed");
                    this.logger.Info("UCS Response for the InteractionId :" + emailContent);
                }
            }
            catch (Exception generalException)
            {
                this.logger.Error("Error Occurred :" + generalException);
            }
        }

        #endregion AddEmailAttachment

        #region HandleSessionExpiry

        private void HandleSessionExpiry(SFDCTaskData task)
        {
            SFDCHttpServer.SessionFlag = true;
            SFDCHttpServer.IsFirstRequestMade = false;
            if (Settings.SFDCOptions.EnableInvokeQueuedTaskOnSessionExpiry)
            {
                Settings.TaskCreateQueuedItems.Enqueue(task);
            }
        }

        private void HandleSessionExpiry(OutputValues task)
        {
            SFDCHttpServer.SessionFlag = true;
            SFDCHttpServer.IsFirstRequestMade = false;
            if (Settings.SFDCOptions.EnableInvokeQueuedTaskOnSessionExpiry)
            {
                Settings.PopupQueuedItems.Enqueue(task);
            }
        }

        #endregion HandleSessionExpiry

        #region InvokeQueuedPopupItems

        public void InvokeQueuedPopupItems(object o)
        {
            this.logger.Info("InvokeQueuedPopupItems()");
            if (!Settings.SFDCOptions.EnableInvokeQueuedTaskOnSessionExpiry)
            {
                this.logger.Info("Invoking queued tasks is disabled..");
                return;
            }
            if (SFDCHttpServer.IsFirstRequestMade)
            {
                //popup all the queued items
                try
                {
                    this.logger.Info("Search Items Count :" + Settings.PopupQueuedItems.Count.ToString());
                    this.logger.Info("Task Create Items Count :" + Settings.TaskCreateQueuedItems.Count.ToString());
                    while (Settings.PopupQueuedItems.Count > 0)
                    {
                        OutputValues output = Settings.PopupQueuedItems.Dequeue();
                        if (output != null)
                            SearchHandler.GetInstance().ProcessSearchData(output.ConnID, output.PopupData, output.CallType);
                    }

                    while (Settings.TaskCreateQueuedItems.Count > 0)
                    {
                        SFDCTaskData taskData = Settings.TaskCreateQueuedItems.Dequeue();
                        if (taskData != null)
                        {
                            switch (taskData.Action)
                            {
                                case TaskAction.TaskCreate:
                                    SFDCUtility.GetInstance().CreateActivityLog(taskData.InteractionId, taskData.TaskFields, taskData.ObjectName, taskData.CallType, taskData.RecordId, taskData.RetryAttempt);
                                    break;

                                case TaskAction.TaskUpdate:
                                    SFDCUtility.GetInstance().UpdateActivityLog(taskData.InteractionId, taskData.TaskFields, taskData.ObjectName, taskData.RecordId, taskData.RetryAttempt);
                                    break;

                                case TaskAction.TaskAppend:
                                    SFDCUtility.GetInstance().AppendActivityLog(taskData.TaskFields, taskData.ObjectName, taskData.RecordId);
                                    break;

                                case TaskAction.RecordUpdate:
                                    SFDCUtility.GetInstance().UpdateNewRecord(taskData.InteractionId, taskData.TaskFields, taskData.ObjectName, taskData.RecordId, taskData.RetryAttempt);
                                    break;

                                case TaskAction.RecordCreate:
                                    SFDCUtility.GetInstance().CreateNewRecord(taskData.InteractionId, taskData.TaskFields, taskData.ObjectName, taskData.RetryAttempt);
                                    break;

                                case TaskAction.None:

                                    break;

                                default:
                                    break;
                            }
                        }
                    }
                }
                catch (Exception generalException)
                {
                    this.logger.Error("Error occurred while invoking queued popup items, details :" + generalException);
                }
            }
            else
            {
                this.logger.Info("Queued popup items can not be invoked at this time, because SFDC test request should be executed first.");
            }
        }

        #endregion InvokeQueuedPopupItems
    }
}