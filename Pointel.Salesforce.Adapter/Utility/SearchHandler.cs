using Genesyslab.Platform.Commons.Collections;
using Pointel.Salesforce.Adapter.Configurations;
using Pointel.Salesforce.Adapter.LogMessage;
using Pointel.Salesforce.Adapter.PForce;
using Pointel.Salesforce.Adapter.SFDCModels;
using Pointel.Salesforce.Adapter.SFDCUtils;
using System;
using System.Text.RegularExpressions;

namespace Pointel.Salesforce.Adapter.Utility
{
    public class SearchHandler
    {
        #region Fields

        private static SearchHandler _currentObject;
        private GeneralOptions _generalOptions = null;
        private Log _logger = null;
        private SFDCUtility _sFDCUtility = null;

        #endregion Fields

        #region Constructor

        public SearchHandler()
        {
            this._logger = Log.GenInstance();
            this._generalOptions = Settings.SFDCOptions;
            this._sFDCUtility = SFDCUtility.GetInstance();
        }

        #endregion Constructor

        #region GetInstance

        public static SearchHandler GetInstance()
        {
            if (_currentObject == null)
                return _currentObject = new SearchHandler();
            return _currentObject;
        }

        #endregion GetInstance

        #region ProcessSearchData

        internal void ProcessSearchData(string connId, SFDCData data, SFDCCallType calltype)
        {
            try
            {
                this._logger.Info("ProcessSearchData: Processing data for SFDC Pop-up for the connectionId : " + connId);
                if (data != null)
                {
                    if (!String.IsNullOrEmpty(data.CommonSearchData) && !String.IsNullOrEmpty(data.CommonPopupObjects))
                    {
                        ProcessSFDCResponseForCommonSearch(_sFDCUtility.SearchSFDC(data.CommonSearchData, data.CommonSearchCondition, data.CommonPopupObjects,
                                data.CommonSearchFormats, connId, data, calltype), data, connId, calltype);
                    }
                    else
                    {
                        if (_generalOptions.CanUseCommonSearchForVoice || _generalOptions.CanUseCommonSearchForChat || _generalOptions.CanUseCommonSearchForEmail)
                        {
                            this._logger.Info("ProcessSearchData: common search data is null or empty, Invoking object based search functionality..");
                        }
                        if (data.LeadData != null)
                        {
                            ProcessSFDCResponse(connId, _sFDCUtility.SearchSFDC(data.LeadData.SearchData, data.LeadData.SearchCondition, data.LeadData.ObjectName,
                                data.LeadData.PhoneNumberSearchFormat, connId, data, calltype), data.LeadData, calltype);
                        }
                        if (data.AccountData != null)
                        {
                            ProcessSFDCResponse(connId, _sFDCUtility.SearchSFDC(data.AccountData.SearchData, data.AccountData.SearchCondition, data.AccountData.ObjectName,
                                data.AccountData.PhoneNumberSearchFormat, connId, data, calltype), data.AccountData, calltype);
                        }
                        if (data.CaseData != null)
                        {
                            ProcessSFDCResponse(connId, _sFDCUtility.SearchSFDC(data.CaseData.SearchData, data.CaseData.SearchCondition, data.CaseData.ObjectName,
                               data.CaseData.PhoneNumberSearchFormat, connId, data, calltype), data.CaseData, calltype);
                        }
                        if (data.ContactData != null)
                        {
                            ProcessSFDCResponse(connId, _sFDCUtility.SearchSFDC(data.ContactData.SearchData, data.ContactData.SearchCondition, data.ContactData.ObjectName,
                               data.ContactData.PhoneNumberSearchFormat, connId, data, calltype), data.ContactData, calltype);
                        }
                        if (data.OpportunityData != null)
                        {
                            ProcessSFDCResponse(connId, _sFDCUtility.SearchSFDC(data.OpportunityData.SearchData, data.OpportunityData.SearchCondition, data.OpportunityData.ObjectName,
                               data.OpportunityData.PhoneNumberSearchFormat, connId, data, calltype), data.OpportunityData, calltype);
                        }
                        if (data.CustomObjectData != null)
                        {
                            foreach (CustomObjectData customObject in data.CustomObjectData)
                            {
                                ProcessSFDCResponse(connId, _sFDCUtility.SearchSFDC(customObject.SearchData, customObject.SearchCondition, customObject.ObjectName,
                              customObject.PhoneNumberSearchFormat, connId, data, calltype), customObject, calltype);
                            }
                        }
                    }
                }
            }
            catch (Exception generalException)
            {
                this._logger.Error("ProcessSearchData: Error Occurred :" + generalException.ToString());
            }
        }

        #endregion ProcessSearchData

        #region ProcessSFDCResponse for Object Based Search

        internal void ProcessSFDCResponse(string interactionId, OutputValues output, SFDCUtilityProperty data, SFDCCallType calltype)
        {
            try
            {
                this._logger.Info("ProcessSFDCResponse: Response from SFDC Search for the InteractionId : " + interactionId);
                if (output != null)
                {
                    this._logger.Info("ProcessSFDCResponse: Object Based Pop-up Invoked for the object " + output.ObjectName);
                    this._logger.Info("ProcessSFDCResponse: Data Received from SFDC : " + output.ToString());

                    if (output.SearchRecord == null && output.IsSearchCancelled)
                    {
                        this._logger.Info("Can not invoke the No-Match Found behavior at this time, because SFDC Search is not performed..");
                        return;
                    }

                    PopupData popup = new PopupData();
                    PopupData ActivityPopup = new PopupData();
                    ActivityPopup.InteractionId = interactionId;
                    ActivityPopup.ObjectName = data.ObjectName;
                    string userLevelLog = string.Empty;
                    switch (output.ObjectName.ToLower())
                    {
                        case "lead":
                        case "account":
                        case "case":
                        case "contact":
                        case "opportunity":
                            if (output.SearchRecord != null)
                            {
                                if (output.SearchRecord.Length > 1)
                                {
                                    if (data.MultipleMatchRecord == "searchpage")
                                    {
                                        popup.InteractionId = interactionId;
                                        //multi match user level activity log creation
                                        if (data.CanCreateMultiMatchActivityLog && data.ActivityLogData != null)
                                        {
                                            userLevelLog = this._sFDCUtility.CreateActivityLog(interactionId, data.ActivityLogData, data.ObjectName, calltype);
                                            if (data.CanPopupMultiMatchActivityLog && !string.IsNullOrWhiteSpace(userLevelLog))
                                            {
                                                ActivityPopup.PopupUrl = userLevelLog;
                                                SendPopupData(interactionId + data.ObjectName + "MM", ActivityPopup);
                                            }
                                        }
                                        popup.PopupUrl = this._generalOptions.SearchPageUrl + data.SearchpageMode + "str=" + output.SearchData;
                                        SendPopupData(interactionId, popup);
                                    }
                                    else if (data.MultipleMatchRecord == "openall")
                                    {
                                        popup.InteractionId = interactionId;
                                        popup.PopupUrl = GetSearchPageIds(output.SearchRecord, data.MaxRecordOpenCount);
                                        SendPopupData(interactionId, popup);
                                    }
                                    else
                                        this._logger.Info("Multimatch Action for " + data.ObjectName + " objects is : None ");
                                }
                                else
                                {
                                    UpdateExistingRecord(interactionId, output, data);
                                    popup.InteractionId = interactionId;
                                    popup.PopupUrl = output.SearchRecord[0].record.Id;    // "/e?lea10=" + CallRinging.currentDNIS;
                                    if (data.ActivityLogData != null)
                                    {
                                        this._sFDCUtility.CreateActivityLog(interactionId, data.ActivityLogData, data.ObjectName, calltype, output.SearchRecord[0].record.Id);
                                    }
                                    popup.ObjectName = data.ObjectName;
                                    SendPopupData(interactionId, popup);
                                }
                            }
                            else if (!string.IsNullOrEmpty(output.SearchData))
                            {
                                if (data.NoRecordFound == "opennew")
                                {
                                    popup.InteractionId = interactionId;
                                    //no record found user level activity log creation
                                    if (data.CanCreateNoRecordActivityLog && data.ActivityLogData != null)
                                    {
                                        userLevelLog = this._sFDCUtility.CreateActivityLog(interactionId, data.ActivityLogData, data.ObjectName, calltype);
                                        if (data.CanPopupNoRecordActivityLog && !string.IsNullOrWhiteSpace(userLevelLog))
                                        {
                                            ActivityPopup.PopupUrl = userLevelLog;
                                            SendPopupData(interactionId + data.ObjectName + "LNM", ActivityPopup);
                                        }
                                    }
                                    popup.PopupUrl = GetNewPageUrl(data.SearchData, data.ObjectName, data.NewRecordFieldIds);
                                    SendPopupData(interactionId, popup);
                                }
                                else if (data.NoRecordFound == "createnew")
                                {
                                    popup.InteractionId = interactionId;
                                    popup.ObjectName = output.ObjectName;
                                    if (data.CreateRecordFieldData != null)
                                    {
                                        string record = _sFDCUtility.CreateNewRecord(interactionId, data.CreateRecordFieldData, output.ObjectName);
                                        if (record != null)
                                        {
                                            if (data.CreateLogForNewRecord)
                                            {
                                                if (data.ActivityLogData != null)
                                                    this._sFDCUtility.CreateActivityLog(interactionId, data.ActivityLogData, data.ObjectName, calltype, record);
                                                else
                                                    this._logger.Warn("Can not create Activity Log  for " + output.ObjectName + " because data is null");
                                            }
                                            popup.PopupUrl = record;
                                            SendPopupData(interactionId, popup);
                                        }
                                        else
                                        {
                                            this._logger.Warn("Can not pop-up New " + data.ObjectName + "Record because Null Returned while creating New Record");
                                        }
                                    }
                                    else
                                    {
                                        this._logger.Warn("Can not create New " + data.ObjectName + " Record because data is null");
                                    }
                                }
                                else if (data.NoRecordFound == "searchpage")
                                {
                                    popup.InteractionId = interactionId;
                                    popup.PopupUrl = this._generalOptions.SearchPageUrl + data.SearchpageMode + "str=" + output.SearchData;
                                    SendPopupData(interactionId, popup);
                                }
                                else if (data.NoRecordFound.ToLower() == "none" && data.ActivityLogData != null)
                                {
                                    if (CheckCanCreateNoRecordforNone(data, calltype))
                                    {
                                        userLevelLog = this._sFDCUtility.CreateActivityLog(interactionId, data.ActivityLogData, data.ObjectName, calltype);
                                        if (data.CanPopupNoRecordActivityLog && !string.IsNullOrWhiteSpace(userLevelLog))
                                        {
                                            ActivityPopup.PopupUrl = userLevelLog;
                                            SendPopupData(interactionId + data.ObjectName + "NM", ActivityPopup);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                this._logger.Warn("No Record found scenario can not be invoked because search data is empty");
                            }
                            break;

                        default:
                            if (output.ObjectName.Contains(","))
                            {
                                this._logger.Info("Common search must be enabled for the multiple object data handling...");
                            }
                            if (output.ObjectName.Contains("__c"))
                            {
                                if (Settings.CustomObjectNames.ContainsKey(output.ObjectName))
                                {
                                    if (output.SearchRecord != null)
                                    {
                                        if (output.SearchRecord.Length > 1)
                                        {
                                            if (data.MultipleMatchRecord == "searchpage")
                                            {
                                                popup.InteractionId = interactionId;
                                                //Multi match user level activity log creation
                                                if (data.CanCreateMultiMatchActivityLog && data.ActivityLogData != null)
                                                {
                                                    userLevelLog = this._sFDCUtility.CreateActivityLog(interactionId, data.ActivityLogData, data.ObjectName, calltype);
                                                    if (data.CanPopupMultiMatchActivityLog && !string.IsNullOrWhiteSpace(userLevelLog))
                                                    {
                                                        ActivityPopup.PopupUrl = userLevelLog;
                                                        SendPopupData(interactionId + data.ObjectName.Substring(0, 2) + "MM", ActivityPopup);
                                                    }
                                                }
                                                popup.PopupUrl = this._generalOptions.SearchPageUrl + data.SearchpageMode + "str=" + output.SearchData;
                                                SendPopupData(interactionId, popup);
                                            }
                                            else if (data.MultipleMatchRecord == "openall")
                                            {
                                                popup.InteractionId = interactionId;
                                                popup.PopupUrl = GetSearchPageIds(output.SearchRecord, data.MaxRecordOpenCount);
                                                SendPopupData(interactionId, popup);
                                            }
                                            else
                                                this._logger.Info("Multimatch Action for Opportunity objects is : None ");
                                        }
                                        else
                                        {
                                            popup.InteractionId = interactionId;
                                            popup.PopupUrl = output.SearchRecord[0].record.Id;
                                            if (data.ActivityLogData != null)
                                            {
                                                this._sFDCUtility.CreateActivityLog(interactionId, data.ActivityLogData, data.ObjectName, calltype, output.SearchRecord[0].record.Id);
                                            }
                                            popup.ObjectName = data.ObjectName;
                                            SendPopupData(interactionId, popup);
                                        }
                                    }
                                    else
                                    {
                                        if (data.NoRecordFound == "opennew")
                                        {
                                            popup.InteractionId = interactionId;
                                            //no record found user level activity log creation
                                            if (data.CanCreateNoRecordActivityLog && data.ActivityLogData != null)
                                            {
                                                userLevelLog = this._sFDCUtility.CreateActivityLog(interactionId, data.ActivityLogData, data.ObjectName, calltype);
                                                if (data.CanPopupNoRecordActivityLog && !string.IsNullOrWhiteSpace(userLevelLog))
                                                {
                                                    ActivityPopup.PopupUrl = userLevelLog;
                                                    SendPopupData(interactionId + data.ObjectName.Substring(0, 2) + "NM", ActivityPopup);
                                                }
                                            }
                                            popup.PopupUrl = GetNewPageUrl(data.SearchData, data.CustomObjectURL, data.NewRecordFieldIds);
                                            SendPopupData(interactionId, popup);
                                        }
                                        else if (data.NoRecordFound == "createnew")
                                        {
                                            popup.InteractionId = interactionId;
                                            popup.ObjectName = output.ObjectName;
                                            if (data.CreateRecordFieldData != null)
                                            {
                                                string record = _sFDCUtility.CreateNewRecord(interactionId, data.CreateRecordFieldData, output.ObjectName);
                                                if (record != null)
                                                {
                                                    if (data.CreateLogForNewRecord)
                                                    {
                                                        if (data.ActivityLogData != null)
                                                            this._sFDCUtility.CreateActivityLog(interactionId, data.ActivityLogData, data.ObjectName, calltype, record);
                                                        else
                                                            this._logger.Warn("Can not create Activity Log  for " + output.ObjectName + " because data is null");
                                                    }
                                                    popup.PopupUrl = record;
                                                    SendPopupData(interactionId, popup);
                                                }
                                                else
                                                {
                                                    this._logger.Warn("Can not pop-up New " + data.ObjectName + "Record because Null Returned while creating New Record");
                                                }
                                            }
                                            else
                                            {
                                                this._logger.Warn("Can not create New " + data.ObjectName + " Record because data is null");
                                            }
                                        }
                                        else if (data.NoRecordFound == "searchpage")
                                        {
                                            popup.InteractionId = interactionId;
                                            popup.PopupUrl = this._generalOptions.SearchPageUrl + data.SearchpageMode + "str=" + output.SearchData;
                                            SendPopupData(interactionId, popup);
                                        }
                                        else if (data.NoRecordFound.ToLower() == "none" && data.ActivityLogData != null)
                                        {
                                            if (CheckCanCreateNoRecordforNone(data, calltype))
                                            {
                                                userLevelLog = this._sFDCUtility.CreateActivityLog(interactionId, data.ActivityLogData, data.ObjectName, calltype);
                                                if (data.CanPopupNoRecordActivityLog && !string.IsNullOrWhiteSpace(userLevelLog))
                                                {
                                                    ActivityPopup.PopupUrl = userLevelLog;
                                                    SendPopupData(interactionId + "NM", ActivityPopup);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            break;
                    }
                }
                else
                {
                    this._logger.Error("ProcessSFDCResponse: Cannot pop-up records because SFDC Search method returned null ");
                }
            }
            catch (Exception generalException)
            {
                this._logger.Error("ProcessSFDCResponse: Error Occurred :" + generalException.ToString());
            }
        }

        private void UpdateExistingRecord(string interactionId, OutputValues output, SFDCUtilityProperty data)
        {
        }

        #endregion ProcessSFDCResponse for Object Based Search

        #region ProcessSFDCResponse for CommonSearch

        internal void ProcessSFDCResponseForCommonSearch(OutputValues outputValues, SFDCData sfdcData, string interactionId, SFDCCallType calltype)
        {
            try
            {
                this._logger.Info("ProcessSFDCResponseForCommonSearch: CommonSearch based Pop-up");
                this._logger.Info("ProcessSFDCResponseForCommonSearch: InteractionId : " + interactionId);
                if (outputValues == null)
                {
                    this._logger.Error("Cannot process the common search data because SFDC response is null...");
                    return;
                }

                if (outputValues.SearchRecord == null && outputValues.IsSearchCancelled)
                {
                    this._logger.Info("Can not invoke the No-Match Found behavior at this time, because SFDC Search is not performed..");
                    return;
                }

                if (calltype == SFDCCallType.InboundEmail || calltype == SFDCCallType.InboundEmailPulled)
                {
                    if (!Settings.InboundEmailSearchResult.ContainsKey(interactionId))
                    {
                        Settings.InboundEmailSearchResult.Add(interactionId, outputValues);
                    }
                }
                string multimatchURL = string.Empty;
                string openAllIds = string.Empty;
                string SearchPageObjectName = string.Empty;
                string userLevelLog = string.Empty;
                bool canCreateMultiMatchLog = false;
                bool canPopupMultiMatchLog = false;
                bool canCreateNoRecordLog = false;
                bool canPopupNoRecordLog = false;

                this._logger.Info("ProcessSFDCResponse: Data received from SFDC for CommonSearch : " + outputValues.ToString());
                if (outputValues.SearchRecord != null)
                {
                    #region Record Found Scenario

                    KeyValueCollection RecordIds = GetRecordIdsFromRecords(outputValues.SearchRecord);
                    if (RecordIds != null && RecordIds.Count > 0)
                    {
                        bool IsExactMatchFound = false;
                        if (_generalOptions.MultiMatchAction == MultiMatchBehaviour.SearchPage && RecordIds.Count > 1)
                        {
                            //pouup search page
                            PopupData popup = new PopupData();
                            popup.InteractionId = interactionId;
                            popup.PopupUrl = this._generalOptions.SearchPageUrl + "&str=" + outputValues.SearchData;
                            SendPopupData(interactionId, popup);
                            //create profile level activity
                            string profileActivittyID = _sFDCUtility.CreateActivityLog(interactionId, sfdcData.ProfileActivityLogData, "profileactivity", calltype);
                            if (profileActivittyID != null)
                            {
                                if (this._generalOptions.CanEnableMultiMatchProfileActivityPopup)
                                {
                                    //pop-up Activity
                                    PopupData ProfilePopupData = new PopupData
                                    {
                                        InteractionId = interactionId,
                                        ObjectName = "profileactivity",
                                        PopupUrl = profileActivittyID
                                    };
                                    SendPopupData(interactionId + "NMMM", ProfilePopupData);
                                }
                            }
                            return;
                        }
                        else if (_generalOptions.MultiMatchAction == MultiMatchBehaviour.ExactMatchPopup)
                        {
                            IsExactMatchFound = CheckForExactMatchRecordFound(RecordIds);
                        }
                        foreach (string key in RecordIds.AllKeys)
                        {
                            userLevelLog = string.Empty;
                            switch (key)
                            {
                                case "Lead":
                                    _logger.Info("ProcessSFDCResponse: Collecting Inbound Voice Pop-up data for Lead");

                                    #region Lead Process

                                    if (sfdcData.LeadData != null)
                                    {
                                        PopupData leadPopup = new PopupData();
                                        if (RecordIds.ContainsKey(key))
                                        {
                                            if (RecordIds[key].ToString().Contains(","))
                                            {
                                                if (!IsExactMatchFound)
                                                {
                                                    SearchPageObjectName = sfdcData.LeadData.ObjectName;
                                                    if (sfdcData.LeadData.MultipleMatchRecord.Equals("searchpage"))
                                                    {
                                                        multimatchURL += sfdcData.LeadData.SearchpageMode;
                                                        //Multi Match create user level activity
                                                        if (!canCreateMultiMatchLog)
                                                            canCreateMultiMatchLog = sfdcData.LeadData.CanCreateMultiMatchActivityLog;
                                                        if (!canPopupMultiMatchLog)
                                                            canPopupMultiMatchLog = sfdcData.LeadData.CanPopupMultiMatchActivityLog;
                                                    }
                                                    else if (sfdcData.LeadData.MultipleMatchRecord.Equals("openall"))
                                                        openAllIds += GetSearchPageIdsCommonSearch(Convert.ToString(RecordIds[key]), sfdcData.LeadData.MaxRecordOpenCount) + ",";
                                                    else if (sfdcData.LeadData.MultipleMatchRecord.Equals("none"))
                                                        _logger.Info("ProcessSFDCResponse: Lead multiple match record action is none");
                                                }
                                                else
                                                    this._logger.Info("Multi-Match pop-up is disabled since exact match found in one/more common search objects");
                                            }
                                            else
                                            {
                                                leadPopup.InteractionId = interactionId;
                                                leadPopup.ObjectName = sfdcData.LeadData.ObjectName;
                                                if (sfdcData.LeadData.ActivityLogData != null)
                                                    this._sFDCUtility.CreateActivityLog(interactionId, sfdcData.LeadData.ActivityLogData, sfdcData.LeadData.ObjectName, calltype, Convert.ToString(RecordIds[key]));
                                                else
                                                    this._logger.Info("ProcessSFDCResponse: Activity Log is null for the Lead object");

                                                leadPopup.PopupUrl = Convert.ToString(RecordIds[key]);
                                                SendPopupData(interactionId + "leadpopup", leadPopup);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        _logger.Warn("CommonPopup : Pop-up data for lead is null");
                                    }

                                    #endregion Lead Process

                                    break;

                                case "Contact":
                                    _logger.Info("ProcessSFDCResponse: Collecting Inbound Voice Pop-up data for contact");

                                    #region Contact Process

                                    if (sfdcData.ContactData != null)
                                    {
                                        PopupData contactPopup = new PopupData();
                                        if (RecordIds.ContainsKey(key))
                                        {
                                            if (RecordIds[key].ToString().Contains(","))
                                            {
                                                if (!IsExactMatchFound)
                                                {
                                                    SearchPageObjectName = sfdcData.ContactData.ObjectName;
                                                    if (sfdcData.ContactData.MultipleMatchRecord.Equals("searchpage"))
                                                    {
                                                        multimatchURL += sfdcData.ContactData.SearchpageMode;
                                                        //Multi Match create user level activity
                                                        if (!canCreateMultiMatchLog)
                                                            canCreateMultiMatchLog = sfdcData.ContactData.CanCreateMultiMatchActivityLog;
                                                        if (!canPopupMultiMatchLog)
                                                            canPopupMultiMatchLog = sfdcData.ContactData.CanPopupMultiMatchActivityLog;
                                                    }
                                                    else if (sfdcData.ContactData.MultipleMatchRecord.Equals("openall"))
                                                        openAllIds += GetSearchPageIdsCommonSearch(Convert.ToString(RecordIds[key]), sfdcData.ContactData.MaxRecordOpenCount) + ",";
                                                    else if (sfdcData.ContactData.MultipleMatchRecord.Equals("none"))
                                                        _logger.Info("ProcessSFDCResponse: Contact multiple match record action is none");
                                                }
                                                else
                                                    this._logger.Info("Multi-Match pop-up is disabled since exact match found in one/more common search objects");
                                            }
                                            else
                                            {
                                                contactPopup.InteractionId = interactionId;
                                                contactPopup.ObjectName = sfdcData.ContactData.ObjectName;
                                                if (sfdcData.ContactData.ActivityLogData != null)
                                                    this._sFDCUtility.CreateActivityLog(interactionId, sfdcData.ContactData.ActivityLogData, sfdcData.ContactData.ObjectName, calltype, Convert.ToString(RecordIds[key]));
                                                else
                                                    this._logger.Info("ProcessSFDCResponse: Activity Log is null for the Contact object");

                                                contactPopup.PopupUrl = Convert.ToString(RecordIds[key]);
                                                SendPopupData(interactionId + "contactpopup", contactPopup);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        _logger.Warn("CommonPopup : Pop-up data for Contact is null");
                                    }

                                    #endregion Contact Process

                                    break;

                                case "Account":
                                    _logger.Info("ProcessSFDCResponse: Collecting Inbound Voice Pop-up data for account");

                                    #region Account Process

                                    if (sfdcData.AccountData != null)
                                    {
                                        PopupData accountPopup = new PopupData();
                                        if (RecordIds.ContainsKey(key))
                                        {
                                            if (RecordIds[key].ToString().Contains(","))
                                            {
                                                if (!IsExactMatchFound)
                                                {
                                                    SearchPageObjectName = sfdcData.AccountData.ObjectName;
                                                    if (sfdcData.AccountData.MultipleMatchRecord.Equals("searchpage"))
                                                    {
                                                        multimatchURL += sfdcData.AccountData.SearchpageMode;
                                                        //Multi Match create user level activity
                                                        if (!canCreateMultiMatchLog)
                                                            canCreateMultiMatchLog = sfdcData.AccountData.CanCreateMultiMatchActivityLog;
                                                        if (!canPopupMultiMatchLog)
                                                            canPopupMultiMatchLog = sfdcData.AccountData.CanPopupMultiMatchActivityLog;
                                                    }
                                                    else if (sfdcData.AccountData.MultipleMatchRecord.Equals("openall"))
                                                        openAllIds += GetSearchPageIdsCommonSearch(Convert.ToString(RecordIds[key]), sfdcData.AccountData.MaxRecordOpenCount) + ",";
                                                    else if (sfdcData.AccountData.MultipleMatchRecord.Equals("none"))
                                                        _logger.Info("ProcessSFDCResponse: Account multiple match record action is none");
                                                }
                                                else
                                                    this._logger.Info("Multi-Match pop-up is disabled since exact match found in one/more common search objects");
                                            }
                                            else
                                            {
                                                accountPopup.InteractionId = interactionId;
                                                accountPopup.ObjectName = sfdcData.AccountData.ObjectName;
                                                if (sfdcData.AccountData.ActivityLogData != null)
                                                    this._sFDCUtility.CreateActivityLog(interactionId, sfdcData.AccountData.ActivityLogData, sfdcData.AccountData.ObjectName, calltype, Convert.ToString(RecordIds[key]));
                                                else
                                                    this._logger.Info("ProcessSFDCResponse: Activity Log is null for the Account object");

                                                accountPopup.PopupUrl = Convert.ToString(RecordIds[key]);
                                                SendPopupData(interactionId + "accountpopup", accountPopup);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        _logger.Warn("CommonPopup : Pop-up data for Account is null");
                                    }

                                    #endregion Account Process

                                    break;

                                case "Case":
                                    _logger.Info("ProcessSFDCResponse: Collecting Inbound Voice Pop-up data for case");

                                    #region Case Process

                                    if (sfdcData.CaseData != null)
                                    {
                                        PopupData casePopup = new PopupData();
                                        if (RecordIds.ContainsKey(key))
                                        {
                                            if (RecordIds[key].ToString().Contains(","))
                                            {
                                                if (!IsExactMatchFound)
                                                {
                                                    SearchPageObjectName = sfdcData.CaseData.ObjectName;
                                                    if (sfdcData.CaseData.MultipleMatchRecord.Equals("searchpage"))
                                                    {
                                                        multimatchURL += sfdcData.CaseData.SearchpageMode;
                                                        //Multi Match create user level activity
                                                        if (!canCreateMultiMatchLog)
                                                            canCreateMultiMatchLog = sfdcData.CaseData.CanCreateMultiMatchActivityLog;
                                                        if (!canPopupMultiMatchLog)
                                                            canPopupMultiMatchLog = sfdcData.CaseData.CanPopupMultiMatchActivityLog;
                                                    }
                                                    else if (sfdcData.CaseData.MultipleMatchRecord.Equals("openall"))
                                                        openAllIds += GetSearchPageIdsCommonSearch(Convert.ToString(RecordIds[key]), sfdcData.CaseData.MaxRecordOpenCount) + ",";
                                                    else if (sfdcData.CaseData.MultipleMatchRecord.Equals("none"))
                                                        _logger.Info("ProcessSFDCResponse: Case multiple match record action is none");
                                                }
                                                else
                                                    this._logger.Info("Multi-Match pop-up is disabled since exact match found in one/more common search objects");
                                            }
                                            else
                                            {
                                                casePopup.InteractionId = interactionId;
                                                casePopup.ObjectName = sfdcData.CaseData.ObjectName;
                                                if (sfdcData.CaseData.ActivityLogData != null)
                                                    this._sFDCUtility.CreateActivityLog(interactionId, sfdcData.CaseData.ActivityLogData, sfdcData.CaseData.ObjectName, calltype, Convert.ToString(RecordIds[key]));
                                                else
                                                    this._logger.Info("ProcessSFDCResponse: Activity Log is null for the Case object");

                                                casePopup.PopupUrl = Convert.ToString(RecordIds[key]);
                                                SendPopupData(interactionId + "casepopup", casePopup);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        _logger.Warn("CommonPopup : Pop-up data for Case is null");
                                    }

                                    #endregion Case Process

                                    break;

                                case "Opportunity":

                                    _logger.Info("ProcessSFDCResponse: Collecting Inbound Voice Pop-up data for opportunity");
                                    if (sfdcData.OpportunityData != null)
                                    {
                                        #region Opportunity

                                        PopupData opportunityPopup = new PopupData();
                                        if (RecordIds.ContainsKey(key))
                                        {
                                            if (RecordIds[key].ToString().Contains(","))
                                            {
                                                if (!IsExactMatchFound)
                                                {
                                                    SearchPageObjectName = sfdcData.OpportunityData.ObjectName;
                                                    if (sfdcData.OpportunityData.MultipleMatchRecord.Equals("searchpage"))
                                                    {
                                                        multimatchURL += sfdcData.OpportunityData.SearchpageMode;
                                                        //Multi Match create user level activity
                                                        if (!canCreateMultiMatchLog)
                                                            canCreateMultiMatchLog = sfdcData.OpportunityData.CanCreateMultiMatchActivityLog;
                                                        if (!canPopupMultiMatchLog)
                                                            canPopupMultiMatchLog = sfdcData.OpportunityData.CanPopupMultiMatchActivityLog;
                                                    }
                                                    else if (sfdcData.OpportunityData.MultipleMatchRecord.Equals("openall"))
                                                        openAllIds += GetSearchPageIdsCommonSearch(Convert.ToString(RecordIds[key]), sfdcData.OpportunityData.MaxRecordOpenCount) + ",";
                                                    else if (sfdcData.OpportunityData.MultipleMatchRecord.Equals("none"))
                                                        _logger.Info("ProcessSFDCResponse: Opportunity multiple match record action is none");
                                                }
                                                else
                                                    this._logger.Info("Multi-Match pop-up is disabled since exact match found in one/more common search objects");
                                            }
                                            else
                                            {
                                                opportunityPopup.InteractionId = interactionId;
                                                opportunityPopup.ObjectName = sfdcData.OpportunityData.ObjectName;
                                                if (sfdcData.OpportunityData.ActivityLogData != null)
                                                    this._sFDCUtility.CreateActivityLog(interactionId, sfdcData.OpportunityData.ActivityLogData, sfdcData.OpportunityData.ObjectName, calltype, Convert.ToString(RecordIds[key]));
                                                else
                                                    this._logger.Info("ProcessSFDCResponse: Activity Log is null for the Opportunity object");

                                                opportunityPopup.PopupUrl = Convert.ToString(RecordIds[key]);
                                                SendPopupData(interactionId + "opportunitypopup", opportunityPopup);
                                            }
                                        }

                                        #endregion Opportunity
                                    }
                                    else
                                    {
                                        _logger.Warn("ProcessSFDCResponse: Pop-up data for opportunity is null");
                                    }
                                    break;

                                default:
                                    if (key.Contains("__c"))
                                    {
                                        #region CustomObject

                                        CustomObjectData customData = null;
                                        string objName = string.Empty;

                                        foreach (CustomObjectData data in sfdcData.CustomObjectData)
                                        {
                                            if (key == data.ObjectName)
                                            {
                                                customData = data;
                                                break;
                                            }
                                        }

                                        if (customData != null)
                                        {
                                            PopupData customPopup = new PopupData();
                                            if (RecordIds.ContainsKey(customData.ObjectName))
                                            {
                                                if (RecordIds[customData.ObjectName].ToString().Contains(","))
                                                {
                                                    if (!IsExactMatchFound)
                                                    {
                                                        SearchPageObjectName = customData.ObjectName;
                                                        if (customData.MultipleMatchRecord.Equals("searchpage"))
                                                        {
                                                            multimatchURL += customData.SearchpageMode;
                                                            //Multi Match create user level activity
                                                            if (!canCreateMultiMatchLog)
                                                                canCreateMultiMatchLog = customData.CanCreateMultiMatchActivityLog;
                                                            if (!canPopupMultiMatchLog)
                                                                canPopupMultiMatchLog = customData.CanPopupMultiMatchActivityLog;
                                                        }
                                                        else if (customData.MultipleMatchRecord.Equals("openall"))
                                                            openAllIds += GetSearchPageIdsCommonSearch(Convert.ToString(RecordIds[customData.ObjectName]), customData.MaxRecordOpenCount) + ",";
                                                        else if (customData.MultipleMatchRecord.Equals("none"))
                                                            _logger.Info("ProcessSFDCResponse: " + customData.ObjectName + " multiple match action is none");
                                                    }
                                                    else
                                                        this._logger.Info("Multi-Match pop-up is disabled since exact match found in one/more common search objects");
                                                }
                                                else
                                                {
                                                    customPopup.InteractionId = interactionId;
                                                    customPopup.ObjectName = customData.ObjectName;
                                                    if (customData.ActivityLogData != null)
                                                        this._sFDCUtility.CreateActivityLog(interactionId, customData.ActivityLogData, customData.ObjectName, calltype, Convert.ToString(RecordIds[customData.ObjectName]));
                                                    else
                                                        this._logger.Info("ProcessSFDCResponse: Activity Log is null for the " + customData.ObjectName + " object");

                                                    customPopup.PopupUrl = Convert.ToString(RecordIds[key]);
                                                    SendPopupData(interactionId + "custompopup", customPopup);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            _logger.Warn("ProcessSFDCResponse: Pop-up data for CustomObject is null");
                                        }

                                        #endregion CustomObject
                                    }
                                    break;
                            }
                        }
                    }

                    #endregion Record Found Scenario
                }
                else
                {
                    #region No Record Found Scenario

                    foreach (string pkey in Settings.CommonPopupObjects.Split(','))
                    {
                        switch (pkey)
                        {
                            case "Lead":
                                if (sfdcData.LeadData != null)
                                {
                                    #region Lead Pop-up

                                    PopupData leadPopup = new PopupData();
                                    if (sfdcData.LeadData.NoRecordFound == "opennew")
                                    {
                                        leadPopup.InteractionId = interactionId;
                                        leadPopup.ObjectName = sfdcData.LeadData.ObjectName;
                                        leadPopup.PopupUrl = GetNewPageUrl(sfdcData.CommonSearchData, sfdcData.LeadData.ObjectName, sfdcData.LeadData.NewRecordFieldIds);
                                        SendPopupData(interactionId + "leadpp", leadPopup);
                                        //no record create user level activity
                                        if (!canCreateNoRecordLog)
                                            canCreateNoRecordLog = sfdcData.LeadData.CanCreateNoRecordActivityLog;
                                        if (!canPopupNoRecordLog)
                                            canPopupNoRecordLog = sfdcData.LeadData.CanPopupNoRecordActivityLog;
                                    }
                                    else if (sfdcData.LeadData.NoRecordFound == "createnew")
                                    {
                                        leadPopup.InteractionId = interactionId;
                                        leadPopup.ObjectName = sfdcData.LeadData.ObjectName;
                                        if (sfdcData.LeadData.CreateRecordFieldData != null)
                                        {
                                            string record = _sFDCUtility.CreateNewRecord(interactionId, sfdcData.LeadData.CreateRecordFieldData, sfdcData.LeadData.ObjectName);
                                            if (record != null)
                                            {
                                                if (sfdcData.LeadData.CreateLogForNewRecord && sfdcData.LeadData.ActivityLogData != null)
                                                {
                                                    this._sFDCUtility.CreateActivityLog(interactionId, sfdcData.LeadData.ActivityLogData, sfdcData.LeadData.ObjectName, calltype, record);
                                                }
                                                leadPopup.PopupUrl = record;
                                                SendPopupData(interactionId + "leadpp", leadPopup);
                                            }
                                            else
                                            {
                                                this._logger.Warn("Can not pop-up New Lead Record because Null Returned while creating New Record");
                                            }
                                        }
                                        else
                                        {
                                            this._logger.Warn("Can not create New " + sfdcData.LeadData.ObjectName + " Record because data is null");
                                        }
                                    }
                                    else if (sfdcData.LeadData.NoRecordFound == "searchpage")
                                    {
                                        leadPopup.InteractionId = interactionId;
                                        leadPopup.ObjectName = sfdcData.LeadData.ObjectName;
                                        leadPopup.PopupUrl = this._generalOptions.SearchPageUrl + sfdcData.LeadData.SearchpageMode + "str=" + outputValues.SearchData;
                                        SendPopupData(interactionId + "leadpp", leadPopup);
                                    }
                                    else if (sfdcData.LeadData.NoRecordFound.ToLower() == "none" && CheckCanCreateNoRecordforNone(sfdcData.LeadData, calltype))
                                    {
                                        canCreateNoRecordLog = true;
                                        if (!canPopupNoRecordLog)
                                            canPopupNoRecordLog = sfdcData.LeadData.CanPopupNoRecordActivityLog;
                                    }

                                    #endregion Lead Pop-up
                                }
                                break;

                            case "Account":
                                if (sfdcData.AccountData != null)
                                {
                                    #region Account Pop-up

                                    PopupData accountPopup = new PopupData();
                                    if (sfdcData.AccountData.NoRecordFound == "opennew")
                                    {
                                        accountPopup.InteractionId = interactionId;
                                        accountPopup.ObjectName = sfdcData.AccountData.ObjectName;
                                        accountPopup.PopupUrl = GetNewPageUrl(sfdcData.CommonSearchData, sfdcData.AccountData.ObjectName, sfdcData.AccountData.NewRecordFieldIds);
                                        SendPopupData(interactionId + "accountpp", accountPopup);
                                        //norecord create user level activity
                                        if (!canCreateNoRecordLog)
                                            canCreateNoRecordLog = sfdcData.AccountData.CanCreateNoRecordActivityLog;
                                        if (!canPopupNoRecordLog)
                                            canPopupNoRecordLog = sfdcData.AccountData.CanPopupNoRecordActivityLog;
                                    }
                                    else if (sfdcData.AccountData.NoRecordFound == "createnew")
                                    {
                                        accountPopup.InteractionId = interactionId;
                                        accountPopup.ObjectName = sfdcData.AccountData.ObjectName;
                                        if (sfdcData.AccountData.CreateRecordFieldData != null)
                                        {
                                            string record = _sFDCUtility.CreateNewRecord(interactionId, sfdcData.AccountData.CreateRecordFieldData, sfdcData.AccountData.ObjectName);
                                            if (!String.IsNullOrEmpty(record))
                                            {
                                                if (sfdcData.AccountData.CreateLogForNewRecord && sfdcData.AccountData.ActivityLogData != null)
                                                {
                                                    this._sFDCUtility.CreateActivityLog(interactionId, sfdcData.AccountData.ActivityLogData, sfdcData.AccountData.ObjectName, calltype, record);
                                                }
                                                accountPopup.PopupUrl = record;
                                                SendPopupData(interactionId + "accountpp", accountPopup);
                                            }
                                            else
                                            {
                                                this._logger.Warn("Can not pop-up New Account Record because Null Returned while creating New Record");
                                            }
                                        }
                                        else
                                        {
                                            this._logger.Warn("Can not create New " + sfdcData.AccountData.ObjectName + " Record because data is null");
                                        }
                                    }
                                    else if (sfdcData.AccountData.NoRecordFound == "searchpage")
                                    {
                                        accountPopup.InteractionId = interactionId;
                                        accountPopup.ObjectName = sfdcData.AccountData.ObjectName;
                                        accountPopup.PopupUrl = this._generalOptions.SearchPageUrl + sfdcData.AccountData.SearchpageMode + "str=" + outputValues.SearchData;
                                        SendPopupData(interactionId + "accountpp", accountPopup);
                                    }
                                    else if (sfdcData.AccountData.NoRecordFound.ToLower() == "none" && CheckCanCreateNoRecordforNone(sfdcData.AccountData, calltype))
                                    {
                                        canCreateNoRecordLog = true;
                                        if (!canPopupNoRecordLog)
                                            canPopupNoRecordLog = sfdcData.AccountData.CanPopupNoRecordActivityLog;
                                    }

                                    #endregion Account Pop-up
                                }
                                break;

                            case "Contact":
                                if (sfdcData.ContactData != null)
                                {
                                    #region contact pop-up

                                    PopupData contactPopup = new PopupData();
                                    if (sfdcData.ContactData.NoRecordFound == "opennew")
                                    {
                                        contactPopup.InteractionId = interactionId;
                                        contactPopup.ObjectName = sfdcData.ContactData.ObjectName;
                                        contactPopup.PopupUrl = GetNewPageUrl(sfdcData.CommonSearchData, sfdcData.ContactData.ObjectName, sfdcData.ContactData.NewRecordFieldIds);
                                        SendPopupData(interactionId + "contactpp", contactPopup);
                                        //norecord create user level activity
                                        if (!canCreateNoRecordLog)
                                            canCreateNoRecordLog = sfdcData.ContactData.CanCreateNoRecordActivityLog;
                                        if (!canPopupNoRecordLog)
                                            canPopupNoRecordLog = sfdcData.ContactData.CanPopupNoRecordActivityLog;
                                    }
                                    else if (sfdcData.ContactData.NoRecordFound == "createnew")
                                    {
                                        contactPopup.InteractionId = interactionId;
                                        contactPopup.ObjectName = sfdcData.ContactData.ObjectName;
                                        if (sfdcData.ContactData.CreateRecordFieldData != null)
                                        {
                                            string record = _sFDCUtility.CreateNewRecord(interactionId, sfdcData.ContactData.CreateRecordFieldData, sfdcData.ContactData.ObjectName);
                                            if (!String.IsNullOrEmpty(record))
                                            {
                                                if (sfdcData.ContactData.CreateLogForNewRecord && sfdcData.ContactData.ActivityLogData != null)
                                                {
                                                    this._sFDCUtility.CreateActivityLog(interactionId, sfdcData.ContactData.ActivityLogData, sfdcData.ContactData.ObjectName, calltype, record);
                                                }
                                                contactPopup.PopupUrl = record;
                                                SendPopupData(interactionId + "contactpp", contactPopup);
                                            }
                                            else
                                            {
                                                this._logger.Warn("Can not pop-up New Contact Record because Null Returned while creating New Record");
                                            }
                                        }
                                        else
                                        {
                                            this._logger.Warn("Can not create New " + sfdcData.ContactData.ObjectName + " Record because data is null");
                                        }
                                    }
                                    else if (sfdcData.ContactData.NoRecordFound == "searchpage")
                                    {
                                        contactPopup.InteractionId = interactionId;
                                        contactPopup.ObjectName = sfdcData.ContactData.ObjectName;
                                        contactPopup.PopupUrl = this._generalOptions.SearchPageUrl + sfdcData.ContactData.SearchpageMode + "str=" + outputValues.SearchData;
                                        SendPopupData(interactionId + "contactpp", contactPopup);
                                    }
                                    else if (sfdcData.ContactData.NoRecordFound.ToLower() == "none" && CheckCanCreateNoRecordforNone(sfdcData.ContactData, calltype))
                                    {
                                        canCreateNoRecordLog = true;
                                        if (!canPopupNoRecordLog)
                                            canPopupNoRecordLog = sfdcData.ContactData.CanPopupNoRecordActivityLog;
                                    }

                                    #endregion contact pop-up
                                }
                                break;

                            case "Case":
                                if (sfdcData.CaseData != null)
                                {
                                    #region Case Pop-up

                                    PopupData casePopup = new PopupData();
                                    if (sfdcData.CaseData.NoRecordFound == "opennew")
                                    {
                                        casePopup.InteractionId = interactionId;
                                        casePopup.ObjectName = sfdcData.CaseData.ObjectName;
                                        casePopup.PopupUrl = GetNewPageUrl(sfdcData.CommonSearchData, sfdcData.CaseData.ObjectName, sfdcData.CaseData.NewRecordFieldIds);
                                        SendPopupData(interactionId + "Casepp", casePopup);
                                        //norecord create user level activity
                                        if (!canCreateNoRecordLog)
                                            canCreateNoRecordLog = sfdcData.CaseData.CanCreateNoRecordActivityLog;
                                        if (!canPopupNoRecordLog)
                                            canPopupNoRecordLog = sfdcData.CaseData.CanPopupNoRecordActivityLog;
                                    }
                                    else if (sfdcData.CaseData.NoRecordFound == "createnew")
                                    {
                                        casePopup.InteractionId = interactionId;
                                        casePopup.ObjectName = sfdcData.CaseData.ObjectName;
                                        if (sfdcData.CaseData.CreateRecordFieldData != null)
                                        {
                                            string record = _sFDCUtility.CreateNewRecord(interactionId, sfdcData.CaseData.CreateRecordFieldData, sfdcData.CaseData.ObjectName);
                                            if (record != null)
                                            {
                                                if (sfdcData.CaseData.CreateLogForNewRecord && sfdcData.CaseData.ActivityLogData != null)
                                                {
                                                    this._sFDCUtility.CreateActivityLog(interactionId, sfdcData.CaseData.ActivityLogData, sfdcData.CaseData.ObjectName, calltype, record);
                                                }
                                                casePopup.PopupUrl = record;
                                                SendPopupData(interactionId + "Casepp", casePopup);
                                            }
                                            else
                                            {
                                                this._logger.Warn("Can not pop-up New Case Record because Null Returned while creating New Record");
                                            }
                                        }
                                        else
                                        {
                                            this._logger.Warn("Can not create New " + sfdcData.CaseData.ObjectName + " Record because data is null");
                                        }
                                    }
                                    else if (sfdcData.CaseData.NoRecordFound == "searchpage")
                                    {
                                        casePopup.InteractionId = interactionId;
                                        casePopup.ObjectName = sfdcData.CaseData.ObjectName;
                                        casePopup.PopupUrl = this._generalOptions.SearchPageUrl + sfdcData.CaseData.SearchpageMode + "str=" + outputValues.SearchData;
                                        SendPopupData(interactionId + "Casepp", casePopup);
                                    }
                                    else if (sfdcData.CaseData.NoRecordFound.ToLower() == "none" && CheckCanCreateNoRecordforNone(sfdcData.CaseData, calltype))
                                    {
                                        canCreateNoRecordLog = true;
                                        if (!canPopupNoRecordLog)
                                            canPopupNoRecordLog = sfdcData.CaseData.CanPopupNoRecordActivityLog;
                                    }

                                    #endregion Case Pop-up
                                }
                                break;

                            case "Opportunity":
                                if (sfdcData.OpportunityData != null)
                                {
                                    #region Opportunity Pop-up

                                    PopupData opportunityPopup = new PopupData();
                                    if (sfdcData.OpportunityData.NoRecordFound == "opennew")
                                    {
                                        opportunityPopup.InteractionId = interactionId;
                                        opportunityPopup.ObjectName = sfdcData.OpportunityData.ObjectName;
                                        opportunityPopup.PopupUrl = GetNewPageUrl(sfdcData.CommonSearchData, sfdcData.OpportunityData.ObjectName, sfdcData.OpportunityData.NewRecordFieldIds);
                                        SendPopupData(interactionId + "oppopp", opportunityPopup);
                                        //norecord create user level activity
                                        if (!canCreateNoRecordLog)
                                            canCreateNoRecordLog = sfdcData.OpportunityData.CanCreateNoRecordActivityLog;
                                        if (!canPopupNoRecordLog)
                                            canPopupNoRecordLog = sfdcData.OpportunityData.CanPopupNoRecordActivityLog;
                                    }
                                    else if (sfdcData.OpportunityData.NoRecordFound == "createnew")
                                    {
                                        opportunityPopup.InteractionId = interactionId;
                                        opportunityPopup.ObjectName = sfdcData.OpportunityData.ObjectName;
                                        if (sfdcData.OpportunityData.CreateRecordFieldData != null)
                                        {
                                            string record = _sFDCUtility.CreateNewRecord(interactionId, sfdcData.OpportunityData.CreateRecordFieldData, sfdcData.OpportunityData.ObjectName);
                                            if (record != null)
                                            {
                                                if (sfdcData.OpportunityData.CreateLogForNewRecord && sfdcData.OpportunityData.ActivityLogData != null)
                                                {
                                                    this._sFDCUtility.CreateActivityLog(interactionId, sfdcData.OpportunityData.ActivityLogData, sfdcData.OpportunityData.ObjectName, calltype, record);
                                                }
                                                opportunityPopup.PopupUrl = record;
                                                SendPopupData(interactionId + "oppopp", opportunityPopup);
                                            }
                                            else
                                            {
                                                this._logger.Warn("Can not pop-up New Opportunity Record because Null Returned while creating New Record");
                                            }
                                        }
                                        else
                                        {
                                            this._logger.Warn("Can not create New " + sfdcData.OpportunityData.ObjectName + " Record because data is null");
                                        }
                                    }
                                    else if (sfdcData.OpportunityData.NoRecordFound == "searchpage")
                                    {
                                        opportunityPopup.InteractionId = interactionId;
                                        opportunityPopup.ObjectName = sfdcData.OpportunityData.ObjectName;
                                        opportunityPopup.PopupUrl = this._generalOptions.SearchPageUrl + sfdcData.OpportunityData.SearchpageMode + "str=" + outputValues.SearchData;
                                        SendPopupData(interactionId + "oppopp", opportunityPopup);
                                    }
                                    else if (sfdcData.OpportunityData.NoRecordFound.ToLower() == "none" && CheckCanCreateNoRecordforNone(sfdcData.OpportunityData, calltype))
                                    {
                                        canCreateNoRecordLog = true;
                                        if (!canPopupNoRecordLog)
                                            canPopupNoRecordLog = sfdcData.OpportunityData.CanPopupNoRecordActivityLog;
                                    }

                                    #endregion Opportunity Pop-up
                                }
                                break;

                            default:
                                if (sfdcData.CustomObjectData != null)
                                {
                                    #region CustomObject Pop-up

                                    foreach (CustomObjectData dat in sfdcData.CustomObjectData)
                                    {
                                        if (pkey == dat.ObjectName)
                                        {
                                            PopupData customPopup = new PopupData();
                                            if (dat.NoRecordFound == "opennew")
                                            {
                                                customPopup.InteractionId = interactionId;
                                                customPopup.ObjectName = dat.ObjectName;
                                                customPopup.PopupUrl = GetNewPageUrl(sfdcData.CommonSearchData, dat.CustomObjectURL, dat.NewRecordFieldIds);
                                                SendPopupData(interactionId + "custompp", customPopup);
                                                //norecord create user level activity
                                                if (!canCreateNoRecordLog)
                                                    canCreateNoRecordLog = dat.CanCreateNoRecordActivityLog;
                                                if (!canPopupNoRecordLog)
                                                    canPopupNoRecordLog = dat.CanPopupNoRecordActivityLog;
                                            }
                                            else if (dat.NoRecordFound == "createnew")
                                            {
                                                customPopup.InteractionId = interactionId;
                                                customPopup.ObjectName = dat.ObjectName;
                                                if (dat.CreateRecordFieldData != null)
                                                {
                                                    string record = _sFDCUtility.CreateNewRecord(interactionId, dat.CreateRecordFieldData, dat.ObjectName);
                                                    if (record != null)
                                                    {
                                                        if (dat.CreateLogForNewRecord && dat.ActivityLogData != null)
                                                        {
                                                            this._sFDCUtility.CreateActivityLog(interactionId, dat.ActivityLogData, dat.ObjectName, calltype, record);
                                                        }
                                                        customPopup.PopupUrl = record;
                                                        SendPopupData(interactionId + "custompp", customPopup);
                                                    }
                                                    else
                                                    {
                                                        this._logger.Warn("Can not pop-up New " + dat.ObjectName + " Record because Null Returned while creating New Record");
                                                    }
                                                }
                                                else
                                                {
                                                    this._logger.Warn("Can not create New " + dat.ObjectName + " Record because data is null");
                                                }
                                            }
                                            else if (dat.NoRecordFound == "searchpage")
                                            {
                                                customPopup.InteractionId = interactionId;
                                                customPopup.ObjectName = dat.ObjectName;
                                                customPopup.PopupUrl = this._generalOptions.SearchPageUrl + dat.SearchpageMode + "str=" + outputValues.SearchData;
                                                SendPopupData(interactionId + "custompp", customPopup);
                                            }
                                            else if (dat.NoRecordFound.ToLower() == "none" && CheckCanCreateNoRecordforNone(dat, calltype))
                                            {
                                                canCreateNoRecordLog = true;
                                                if (!canPopupNoRecordLog)
                                                    canPopupNoRecordLog = dat.CanPopupNoRecordActivityLog;
                                            }
                                        }
                                    }

                                    #endregion CustomObject Pop-up
                                }
                                break;
                        }
                    }

                    #endregion No Record Found Scenario
                }

                if (!string.IsNullOrWhiteSpace(multimatchURL))
                {
                    PopupData popup = new PopupData();
                    popup.InteractionId = interactionId;
                    popup.ObjectName = SearchPageObjectName;
                    popup.PopupUrl = this._generalOptions.SearchPageUrl + multimatchURL + "&str=" + outputValues.SearchData;
                    SendPopupData(interactionId, popup);
                    multimatchURL = string.Empty;
                }
                if (!string.IsNullOrWhiteSpace(openAllIds))
                {
                    if (openAllIds.Split(',').Length < 51)
                    {
                        PopupData popupAll = new PopupData();
                        popupAll.InteractionId = interactionId;
                        popupAll.ObjectName = SearchPageObjectName;
                        popupAll.PopupUrl = openAllIds;
                        SendPopupData(interactionId, popupAll);
                        openAllIds = string.Empty;
                    }
                    else
                    {
                        string[] array = openAllIds.Split(',');
                        string temp = string.Empty;
                        if (array != null)
                        {
                            for (int i = 0; i < 50; i++)
                            {
                                temp += array[i] + ",";
                            }
                        }
                        if (temp != string.Empty)
                        {
                            PopupData popupAll = new PopupData();
                            popupAll.InteractionId = interactionId;
                            popupAll.ObjectName = SearchPageObjectName;
                            popupAll.PopupUrl = temp;
                            SendPopupData(interactionId, popupAll);
                            openAllIds = string.Empty;
                        }
                    }
                }
                if (canCreateMultiMatchLog)
                {
                    //create profile level activity
                    string profileActivittyID = _sFDCUtility.CreateActivityLog(interactionId, sfdcData.ProfileActivityLogData, "profileactivity", calltype);
                    if (canPopupMultiMatchLog && profileActivittyID != null)
                    {
                        //pop-up Activity
                        PopupData ProfilePopupData = new PopupData
                        {
                            InteractionId = interactionId,
                            ObjectName = "profileactivity",
                            PopupUrl = profileActivittyID
                        };
                        SendPopupData(interactionId + "NMMM", ProfilePopupData);
                    }
                }
                else if (canCreateNoRecordLog)
                {
                    //create profile level activity
                    string profileActivittyID = _sFDCUtility.CreateActivityLog(interactionId, sfdcData.ProfileActivityLogData, "profileactivity", calltype);
                    if (canPopupNoRecordLog && profileActivittyID != null)
                    {
                        //pop-up Activity
                        PopupData ProfilePopupData = new PopupData
                        {
                            InteractionId = interactionId,
                            ObjectName = "profileactivity",
                            PopupUrl = profileActivittyID
                        };
                        SendPopupData(interactionId + "NMMM", ProfilePopupData);
                    }
                }
            }
            catch (Exception generalException)
            {
                _logger.Error("CommonPopup : Error Occurred  : " + generalException.ToString());
            }
        }

        /// <summary>
        /// Checks for exact match record found.
        /// </summary>
        /// <param name="RecordIds">The record ids.</param>
        /// <returns></returns>
        private bool CheckForExactMatchRecordFound(KeyValueCollection RecordIds)
        {
            foreach (string value in RecordIds.AllValues)
            {
                if (!value.Contains(","))
                {
                    return true;
                }
            }
            return false;
        }

        #endregion ProcessSFDCResponse for CommonSearch

        #region GetSearchPageIdsCommonSearch

        private string GetSearchPageIdsCommonSearch(string recordIds, int maxRecordCount)
        {
            try
            {
                string records = string.Empty;
                this._logger.Info("GetSearchPageIdsCommonSearch: Getting search page record Ids...");
                if (!string.IsNullOrEmpty(recordIds))
                {
                    string[] recordIdArray = recordIds.Split(',');
                    if (recordIdArray.Length != 0 && maxRecordCount != 0 && maxRecordCount > 0 && maxRecordCount < 51)
                    {
                        if (recordIdArray.Length >= maxRecordCount)
                        {
                            for (int i = 0; i < maxRecordCount; i++)
                            {
                                records += recordIdArray[i] + ",";
                            }
                        }
                        else
                            records += recordIds + ",";

                        if (records.Length > 1)
                            return records.Substring(0, (records.Length - 1));
                    }
                    else
                        this._logger.Info("GetSearchPageIdsCommonSearch : Max record popup count :" + maxRecordCount);
                }
            }
            catch (Exception generalException)
            {
                this._logger.Error("GetSearchPageIds: " + generalException.ToString());
            }
            return null;
        }

        #endregion GetSearchPageIdsCommonSearch

        #region GetSearchPageIds

        private string GetSearchPageIds(SearchRecord[] records, int maxRecordCount)
        {
            try
            {
                this._logger.Info("GetSearchPageIds: Getting search page record Ids...");
                if (records != null)
                {
                    string recordIds = string.Empty;
                    if (maxRecordCount != 0 && maxRecordCount > 0 && maxRecordCount < 51)
                    {
                        if (records.Length >= maxRecordCount)
                        {
                            for (int i = 0; i < maxRecordCount; i++)
                            {
                                recordIds += records[i].record.Id + ",";
                            }
                        }
                        else
                        {
                            foreach (SearchRecord srecord in records)
                            {
                                recordIds += srecord.record.Id + ",";
                            }
                        }
                        if (recordIds.Length > 1)
                            return recordIds.Substring(0, (recordIds.Length - 1));
                    }
                    else
                        this._logger.Info("GetSearchPageIds : Max record popup count :" + maxRecordCount);
                }
            }
            catch (Exception generalException)
            {
                this._logger.Error("GetSearchPageIds: " + generalException.ToString());
            }
            return null;
        }

        #endregion GetSearchPageIds

        #region GetNewPageUrl

        private string GetNewPageUrl(string searchData, string urlId, string prepopulateIds)
        {
            try
            {
                this._logger.Info("GetNewPageUrl: SearchData : " + searchData + "\n UrlId : " + urlId + "\n PrePopulateIds : " + prepopulateIds);

                switch (urlId.ToLower())
                {
                    case "lead":
                        urlId = SFDCObjectUrlIDs.LEAD_URLID;
                        break;

                    case "account":
                        urlId = SFDCObjectUrlIDs.ACCOUNT_URLID;
                        break;

                    case "case":
                        urlId = SFDCObjectUrlIDs.CASE_URLID;
                        break;

                    case "contact":
                        urlId = SFDCObjectUrlIDs.CONTACT_URLID;
                        break;

                    case "opportunity":
                        urlId = SFDCObjectUrlIDs.OPPORTUNITY_URLID;
                        break;

                    default:
                        break;
                }
                string data = "/" + urlId + "/e?";
                if (!String.IsNullOrEmpty(searchData) && !String.IsNullOrEmpty(prepopulateIds))
                {
                    searchData = searchData.Replace("^,", "").Replace(",^", "").Replace("^", "");
                    string[] sdata = searchData.Split(',');
                    string[] ids = prepopulateIds.Split(',');

                    for (int i = 0; i < sdata.Length && i < ids.Length; i++)
                    {
                        if (Regex.IsMatch(searchData, @"^\d+$"))
                        {
                            if (searchData.Length == 10)
                            {
                                data += ids[i] + "=" + sdata[i] + "&";
                            }
                            else
                                this._logger.Info("phone number is skipped from pre-populate in sfdc, because the length is not equal to 10 : " + sdata[i]);
                        }
                        else
                        {
                            data += ids[i] + "=" + sdata[i] + "&";
                        }
                    }

                    #region OLD Code

                    //if (searchData.Contains(","))
                    //{
                    //    if (sdata != null && ids != null)
                    //    {
                    //        if (ids.Length > sdata.Length || ids.Length == sdata.Length)
                    //        {
                    //            for (int i = 0; i < sdata.Length; i++)
                    //            {
                    //                data += ids[i] + "=" + sdata[i] + "&";
                    //            }
                    //        }
                    //        else
                    //        {
                    //            for (int i = 0; i < ids.Length; i++)
                    //            {
                    //                data += ids[i] + "=" + sdata[i] + "&";
                    //            }
                    //        }
                    //    }
                    //}
                    //else
                    //{
                    //    if (Regex.IsMatch(searchData, @"^\d+$"))
                    //    {
                    //        if (searchData.Length == 10)
                    //        {
                    //            if (prepopulateIds.Contains(","))
                    //            {
                    //                data += prepopulateIds.Substring(0, (prepopulateIds.IndexOf(','))) + "=" + searchData;
                    //            }
                    //            else
                    //            {
                    //                data += prepopulateIds + "=" + searchData;
                    //            }
                    //        }
                    //    }
                    //    else
                    //    {
                    //        if (prepopulateIds.Contains(","))
                    //        {
                    //            data += prepopulateIds.Substring(0, (prepopulateIds.IndexOf(','))) + "=" + searchData;
                    //        }
                    //        else
                    //        {
                    //            data += prepopulateIds + "=" + searchData;
                    //        }
                    //    }
                    //}

                    #endregion OLD Code
                }
                return data;
            }
            catch (Exception generalException)
            {
                this._logger.Error("GetNewPageUrl : " + generalException.ToString());
            }
            return null;
        }

        #endregion GetNewPageUrl

        #region CheckCanCreateNoRecordforNone

        private bool CheckCanCreateNoRecordforNone(ISFDCUtilityProperty data, SFDCCallType calltype)
        {
            if (calltype.Equals(SFDCCallType.InboundVoice) && data.CanCreateProfileActivityforInbNoRecord)
            {
                return true;
            }
            if ((calltype.Equals(SFDCCallType.OutboundVoiceFailure) || calltype.Equals(SFDCCallType.OutboundVoiceSuccess))
                && data.CanCreateProfileActivityforOutNoRecord)
            {
                return true;
            }
            if (calltype.Equals(SFDCCallType.ConsultVoiceReceived)
                && data.CanCreateProfileActivityforConNoRecord)
            {
                return true;
            }
            if ((calltype.Equals(SFDCCallType.InboundEmail) || calltype.Equals(SFDCCallType.InboundEmailPulled)) && data.CanCreateProfileActivityforInbNoRecord)
            {
                return true;
            }
            if ((calltype.Equals(SFDCCallType.OutboundEmailSuccess) || calltype.Equals(SFDCCallType.OutboundEmailFailure) || calltype.Equals(SFDCCallType.OutboundEmailPulled)) &&
                data.CanCreateProfileActivityforOutNoRecord)
            {
                return true;
            }
            return false;
        }

        #endregion CheckCanCreateNoRecordforNone

        #region GetRecordIdsFromRecords

        private KeyValueCollection GetRecordIdsFromRecords(SearchRecord[] searchRecords)
        {
            try
            {
                this._logger.Info("GetRecordIdsFromRecords: Split Search Records object wise");
                KeyValueCollection RecordIds = new KeyValueCollection();
                if (searchRecords != null)
                {
                    foreach (SearchRecord searchRecord in searchRecords)
                    {
                        if (searchRecord != null && searchRecord.record != null && !string.IsNullOrWhiteSpace(searchRecord.record.Id))
                        {
                            switch (searchRecord.record.type)
                            {
                                case "Lead":
                                    if (RecordIds.ContainsKey("Lead"))
                                    {
                                        RecordIds["Lead"] += "," + searchRecord.record.Id;
                                    }
                                    else
                                    {
                                        RecordIds.Add("Lead", searchRecord.record.Id);
                                    }
                                    break;

                                case "Contact":
                                    if (RecordIds.ContainsKey("Contact"))
                                    {
                                        RecordIds["Contact"] += "," + searchRecord.record.Id;
                                    }
                                    else
                                    {
                                        RecordIds.Add("Contact", searchRecord.record.Id);
                                    }
                                    break;

                                case "Case":
                                    if (RecordIds.ContainsKey("Case"))
                                    {
                                        RecordIds["Case"] += "," + searchRecord.record.Id;
                                    }
                                    else
                                    {
                                        RecordIds.Add("Case", searchRecord.record.Id);
                                    }
                                    break;

                                case "Opportunity":
                                    if (RecordIds.ContainsKey("Opportunity"))
                                    {
                                        RecordIds["Opportunity"] += "," + searchRecord.record.Id;
                                    }
                                    else
                                    {
                                        RecordIds.Add("Opportunity", searchRecord.record.Id);
                                    }
                                    break;

                                case "Account":
                                    if (RecordIds.ContainsKey("Account"))
                                    {
                                        RecordIds["Account"] += "," + searchRecord.record.Id;
                                    }
                                    else
                                    {
                                        RecordIds.Add("Account", searchRecord.record.Id);
                                    }
                                    break;

                                default:
                                    if (RecordIds.ContainsKey(searchRecord.record.type))
                                    {
                                        RecordIds[searchRecord.record.type] += "," + searchRecord.record.Id;
                                    }
                                    else
                                    {
                                        RecordIds.Add(searchRecord.record.type, searchRecord.record.Id);
                                    }
                                    break;
                            }
                        }
                    }
                }
                return RecordIds;
            }
            catch (Exception generalException)
            {
                this._logger.Error("Error Occurred while parsing record id from SFDC Response : " + generalException.ToString());
            }
            return null;
        }

        #endregion GetRecordIdsFromRecords

        #region Send Update Log to SFDC

        internal void ProcessUpdateData(string connId, SFDCData sfdcData, bool updateAttachment = false)
        {
            try
            {
                this._logger.Info("ProcessUpdateData : Processing Update Log Data for the ConnId : " + connId);
                if (Settings.UpdateSFDCLog.ContainsKey(connId))
                {
                    UpdateLogData updateData = Settings.UpdateSFDCLog[connId];
                    if (updateData != null)
                    {
                        if (!String.IsNullOrEmpty(updateData.LeadActivityId) && sfdcData.LeadData != null)
                        {
                            if (sfdcData.LeadData.UpdateActivityLogData != null)
                            {
                                this._sFDCUtility.UpdateActivityLog(connId, sfdcData.LeadData.UpdateActivityLogData, sfdcData.LeadData.ObjectName, updateData.LeadActivityId);
                            }
                            else
                            {
                                this._logger.Info("Can not update Activity Log for Lead id : " + updateData.LeadActivityId + " because Update Activity Log data null...");
                            }
                            if (sfdcData.LeadData.AppendActivityLogData != null && sfdcData.LeadData.AppendActivityLogData.Count > 0)
                            {
                                this._sFDCUtility.AppendActivityLog(sfdcData.LeadData.AppendActivityLogData, "Task", updateData.LeadActivityId);
                            }
                            if (updateAttachment)
                                _sFDCUtility.AddEmailAttachment(connId, updateData.LeadActivityId);
                        }
                        if (!String.IsNullOrEmpty(updateData.LeadRecordId) && sfdcData.LeadData != null)
                        {
                            _sFDCUtility.UpdateNewRecord(connId, sfdcData.LeadData.UpdateRecordFieldsData, sfdcData.LeadData.ObjectName.ToString(), updateData.LeadRecordId);
                        }

                        if (!String.IsNullOrEmpty(updateData.ContactActivityId) && sfdcData.ContactData != null)
                        {
                            if (sfdcData.ContactData.UpdateActivityLogData != null)
                            {
                                this._sFDCUtility.UpdateActivityLog(connId, sfdcData.ContactData.UpdateActivityLogData, sfdcData.ContactData.ObjectName, updateData.ContactActivityId);
                            }
                            else
                            {
                                this._logger.Info("Can not update Activity Log for Contact id : " + updateData.ContactActivityId + " because Update Activity Log data null...");
                            }
                            if (sfdcData.ContactData.AppendActivityLogData != null && sfdcData.ContactData.AppendActivityLogData.Count > 0)
                            {
                                this._sFDCUtility.AppendActivityLog(sfdcData.ContactData.AppendActivityLogData, "Task", updateData.ContactActivityId);
                            }

                            if (updateAttachment)
                                _sFDCUtility.AddEmailAttachment(connId, updateData.ContactActivityId);
                        }
                        if (!String.IsNullOrEmpty(updateData.ContactRecordId) && sfdcData.ContactData != null)
                        {
                            _sFDCUtility.UpdateNewRecord(connId, sfdcData.ContactData.UpdateRecordFieldsData, sfdcData.ContactData.ObjectName, updateData.ContactRecordId);
                        }

                        if (!String.IsNullOrEmpty(updateData.AccountActivityId) && sfdcData.AccountData != null)
                        {
                            if (sfdcData.AccountData.UpdateActivityLogData != null)
                            {
                                this._sFDCUtility.UpdateActivityLog(connId, sfdcData.AccountData.UpdateActivityLogData, sfdcData.AccountData.ObjectName, updateData.AccountActivityId);
                            }
                            else
                            {
                                this._logger.Info("Can not update Activity Log for Account id : " + updateData.AccountActivityId + " because Update Activity Log data null...");
                            }
                            if (sfdcData.AccountData.AppendActivityLogData != null && sfdcData.AccountData.AppendActivityLogData.Count > 0)
                            {
                                this._sFDCUtility.AppendActivityLog(sfdcData.AccountData.AppendActivityLogData, "Task", updateData.AccountActivityId);
                            }
                            if (updateAttachment)
                                _sFDCUtility.AddEmailAttachment(connId, updateData.AccountActivityId);
                        }
                        if (!String.IsNullOrEmpty(updateData.AccountRecordId) && sfdcData.AccountData != null)
                        {
                            _sFDCUtility.UpdateNewRecord(connId, sfdcData.AccountData.UpdateRecordFieldsData, sfdcData.AccountData.ObjectName, updateData.AccountRecordId);
                        }

                        if (!String.IsNullOrEmpty(updateData.CaseActivityId) && sfdcData.CaseData != null)
                        {
                            if (sfdcData.CaseData.UpdateActivityLogData != null)
                            {
                                this._sFDCUtility.UpdateActivityLog(connId, sfdcData.CaseData.UpdateActivityLogData, sfdcData.CaseData.ObjectName, updateData.CaseActivityId);
                            }
                            else
                            {
                                this._logger.Info("Can not update Activity Log for Case id : " + updateData.CaseActivityId + " because Update Activity Log data null...");
                            }
                            if (sfdcData.CaseData.AppendActivityLogData != null && sfdcData.CaseData.AppendActivityLogData.Count > 0)
                            {
                                this._sFDCUtility.AppendActivityLog(sfdcData.CaseData.AppendActivityLogData, "Task", updateData.CaseActivityId);
                            }

                            if (updateAttachment)
                                _sFDCUtility.AddEmailAttachment(connId, updateData.CaseActivityId);
                        }
                        if (!String.IsNullOrEmpty(updateData.CaseRecordId) && sfdcData.CaseData != null)
                        {
                            _sFDCUtility.UpdateNewRecord(connId, sfdcData.CaseData.UpdateRecordFieldsData, sfdcData.CaseData.ObjectName, updateData.CaseRecordId);
                        }

                        if (!String.IsNullOrEmpty(updateData.OpportunityActivityId) && sfdcData.OpportunityData != null)
                        {
                            if (sfdcData.OpportunityData.UpdateActivityLogData != null)
                            {
                                this._sFDCUtility.UpdateActivityLog(connId, sfdcData.OpportunityData.UpdateActivityLogData, sfdcData.OpportunityData.ObjectName, updateData.OpportunityActivityId);
                            }
                            else
                            {
                                this._logger.Info("Can not update Activity Log for Opportunity id : " + updateData.OpportunityActivityId + " because Update Activity Log data null...");
                            }
                            if (sfdcData.OpportunityData.AppendActivityLogData != null && sfdcData.OpportunityData.AppendActivityLogData.Count > 0)
                            {
                                this._sFDCUtility.AppendActivityLog(sfdcData.OpportunityData.AppendActivityLogData, "Task", updateData.OpportunityActivityId);
                            }
                            if (updateAttachment)
                                _sFDCUtility.AddEmailAttachment(connId, updateData.OpportunityActivityId);
                        }
                        if (!String.IsNullOrEmpty(updateData.OpportunityRecordId) && sfdcData.OpportunityData != null)
                        {
                            _sFDCUtility.UpdateNewRecord(connId, sfdcData.OpportunityData.UpdateRecordFieldsData, sfdcData.OpportunityData.ObjectName, updateData.OpportunityRecordId);
                        }

                        if (sfdcData.UserActivityData != null)
                        {
                            sfdcData.UserActivityData.RecordID = updateData.UserActivityId;
                        }
                        //profile level log update
                        if (!String.IsNullOrEmpty(updateData.ProfileActivityId) && sfdcData.ProfileUpdateActivityLogData != null)
                        {
                            this._sFDCUtility.UpdateActivityLog(connId, sfdcData.ProfileUpdateActivityLogData, "profileactivity", updateData.ProfileActivityId);

                            if (sfdcData.ProfileActivityLogAppendData != null && sfdcData.ProfileActivityLogAppendData.Count > 0)
                            {
                                this._sFDCUtility.AppendActivityLog(sfdcData.ProfileActivityLogAppendData, "Task", updateData.ProfileActivityId);
                            }
                            if (updateAttachment)
                                _sFDCUtility.AddEmailAttachment(connId, updateData.ProfileActivityId);
                        }
                        if (sfdcData.CustomObjectData != null && updateData.CustomObject != null)
                        {
                            foreach (CustomObjectData custom in sfdcData.CustomObjectData)
                            {
                                if (!String.IsNullOrEmpty(custom.ObjectName) && updateData.CustomObject.ContainsKey(custom.ObjectName))
                                {
                                    KeyValueCollection collection = updateData.CustomObject[custom.ObjectName];
                                    if (collection != null)
                                    {
                                        if (collection.ContainsKey("newRecordId"))
                                        {
                                            if (custom.UpdateRecordFieldsData != null)
                                            {
                                                _sFDCUtility.UpdateNewRecord(connId, custom.UpdateRecordFieldsData, custom.ObjectName, collection["newRecordId"].ToString());
                                            }
                                            else
                                            {
                                                this._logger.Error("Can not update Custom Object Record because Update data is null...");
                                            }
                                        }
                                        if (collection.ContainsKey("activityRecordId"))
                                        {
                                            if (custom.UpdateActivityLogData != null)
                                            {
                                                this._sFDCUtility.UpdateActivityLog(connId, custom.UpdateActivityLogData, custom.ObjectName, collection["activityRecordId"].ToString());
                                            }
                                            else
                                            {
                                                this._logger.Info("Can not update Activity Log for " + custom.ObjectName + " id : " + collection["activityRecordId"].ToString() + " because Update Activity Log data null...");
                                            }

                                            if (custom.AppendActivityLogData != null && custom.AppendActivityLogData.Count > 0)
                                            {
                                                this._sFDCUtility.AppendActivityLog(custom.AppendActivityLogData, "Task", collection["activityRecordId"].ToString());
                                            }

                                            if (updateAttachment)
                                                _sFDCUtility.AddEmailAttachment(connId, collection["activityRecordId"].ToString());
                                        }
                                    }
                                }
                                else
                                {
                                    this._logger.Info("object name is null in customObject data");
                                    this._logger.Info("Object name : " + Convert.ToString(custom.ObjectName));
                                }
                            }
                        }
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
                this._logger.Error("ProcessUpdateData : Error occurred while processing update Log :" + generalException.ToString() + "\r \n" + generalException.StackTrace);
            }
        }

        #endregion Send Update Log to SFDC

        #region SendPopupData

        internal void SendPopupData(string connId, PopupData data)
        {
            try
            {
                _logger.Info("SendPopupData : Sending Pop-up data for the connection Id : " + connId);
                if (data != null)
                {
                    _logger.Info("SendPopupData : Pop-up data added to the SFDC collection : " + data.ToString());
                    if (!Settings.SFDCPopupData.ContainsKey(connId))
                    {
                        Settings.SFDCPopupData.Add(connId, data);
                    }
                    else
                    {
                        Settings.SFDCPopupData.Add(connId + System.DateTime.Now.ToString("HH:mm:ss.ffffff"), data);
                    }
                }
                else
                {
                    _logger.Info("SendPopupData : Pop-up data is null for the connection Id : " + connId);
                }
            }
            catch (Exception generalException)
            {
                _logger.Error("SendPopupData : Error Occurred while adding pop-up data to collection " + generalException.ToString());
            }
        }

        #endregion SendPopupData

        #region SendUpdateData

        internal void SendUpdateData(string connId, PopupData data)
        {
            try
            {
                _logger.Info("SendUpdateData : Sending Update data for the connection Id : " + connId);
                if (data != null)
                {
                    _logger.Info("SendUpdateData : Update data added to the SFDC collection  : " + data.ToString());
                    if (!Settings.SFDCPopupData.ContainsKey(connId))
                    {
                        Settings.SFDCPopupData.Add(connId, data);
                    }
                    else
                    {
                        Settings.SFDCPopupData.Add(connId + System.DateTime.Now.ToString("HH:mm:ss.ffffff"), data);
                    }
                }
                else
                {
                    _logger.Info("SendUpdateData : Pop-up data is null for the connection Id : " + connId);
                }
            }
            catch (Exception generalException)
            {
                _logger.Error("SendUpdateData : Error Occurred while adding Update data to collection " + generalException.ToString());
                Settings.SFDCPopupData.Add(connId + System.DateTime.Now.ToString("HH:mm:ss.ffffff") + "upt", data);
            }
        }

        #endregion SendUpdateData
    }
}