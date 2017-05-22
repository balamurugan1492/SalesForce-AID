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
using Pointel.Salesforce.Adapter.Utility;
using Pointel.Salesforce.Adapter.Voice;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml;

namespace Pointel.Salesforce.Adapter.SFDCModels
{
    internal class SFDCUtiltiyHelper
    {
        #region Fields

        private static SFDCUtiltiyHelper _currentObject = null;
        private Log _logger;
        private XmlNodeList _newParty = null;
        private XmlNodeList _rootNode = null;
        private XmlDocument _xml = new XmlDocument();

        #endregion Fields

        #region Constructor

        private SFDCUtiltiyHelper()
        {
            this._logger = Log.GenInstance();
        }

        #endregion Constructor

        #region GetInstance

        public static SFDCUtiltiyHelper GetInstance()
        {
            if (_currentObject == null)
            {
                _currentObject = new SFDCUtiltiyHelper();
            }
            return _currentObject;
        }

        #endregion GetInstance

        #region Get User-Data SearchValues

        public string GetUserDataSearchValues(KeyValueCollection userData, string[] userDataSearchKeys)
        {
            try
            {
                if (userData != null && userDataSearchKeys != null)
                {
                    string searchValues = string.Empty;
                    this._logger.Info("GetUserDataSearchValues: Reading User-Data Search Values, Keys : " + string.Join(",", userDataSearchKeys));
                    foreach (string key in userDataSearchKeys)
                    {
                        this._logger.Info("Reading attach data for the key : " + key);
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
                            this._logger.Info("GetUserDataSearchValues: No Attach data found for the key : " + key);
                            searchValues += "^,";
                        }
                    }
                    return (searchValues != string.Empty) ? searchValues.Substring(0, searchValues.Length - 1) : searchValues;
                }
                else
                {
                    this._logger.Info("GetUserDataSearchValues: Either user-data or search keys are null....");
                }
            }
            catch (Exception generalException)
            {
                this._logger.Error("GetSearchUserDataValue: Error occurred while getting User-Data Search values : " + generalException.ToString());
            }
            return string.Empty;
        }

        #endregion Get User-Data SearchValues

        #region GetVoiceAttributeSearchValues

        public string GetVoiceAttributeSearchValues(IMessage message, string[] attributeSearchKeys)
        {
            try
            {
                if (attributeSearchKeys != null && message != null)
                {
                    string searchValues = string.Empty;
                    this._logger.Info("GetVoiceAttributeSearchValues: Reading Attribute Search Values, Keys : " + string.Join(",", attributeSearchKeys));
                    dynamic voiceEvent = Convert.ChangeType(message, message.GetType());
                    if (voiceEvent != null)
                    {
                        #region EventRinging Attribute data

                        foreach (string key in attributeSearchKeys)
                        {
                            this._logger.Info("Reading attribute search data for the key : " + key);
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
                                    this._logger.Info("No attribute search data found for the key : " + key);
                                    break;
                            }
                        }

                        #endregion EventRinging Attribute data
                    }
                    return (searchValues != string.Empty) ? searchValues.Substring(0, searchValues.Length - 1) : searchValues;
                }
                else
                {
                    this._logger.Info("GetVoiceAttributeSearchValues: Attribute search keys are empty");
                }
            }
            catch (Exception generalException)
            {
                this._logger.Error("GetVoiceSearchAttributeValue: Error occurred  while getting Attribute Search values : " + generalException.ToString());
            }
            return string.Empty;
        }

        #endregion GetVoiceAttributeSearchValues

        #region Get Truncated number

        public string TruncateNumbers(string source, int tail_length)
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
                this._logger.Error("TruncateNumbers : Error occured while Truncating incoming data from Salesforce :" + generalException.ToString());
            }
            return string.Empty;
        }

        #endregion Get Truncated number

        #region GetVoiceAttributeValueForLog

        public string GetVoiceAttributeValueForLog(KeyValueCollection collection, dynamic interactionEvent, string searchKey, SFDCCallType callType)
        {
            string output = null;
            try
            {
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

                                case "dnis":
                                    output = interactionEvent.DNIS;
                                    break;

                                case "calltype":
                                    if (callType == SFDCCallType.ConsultVoiceReceived && !string.IsNullOrWhiteSpace(VoiceEventHandler.ConsultText) && VoiceEventHandler.IsCallTransfered)
                                        output = VoiceEventHandler.ConsultText;
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
                    this._logger.Info("GetAttributeValueForLog: Attribute key is null or empty");
                }
            }
            catch (Exception generalException)
            {
                this._logger.Error("GetAttributeValueForLog : Reading Attribute Search Values, Keys : " + generalException.ToString());
            }
            return output;
        }

        #endregion GetVoiceAttributeValueForLog

        #region GetLogDataForVoice In Hierarchal Fashion

        public string GetLogDataForVoice(KeyValueCollection temp1, dynamic message, string keyPrefix, SFDCCallType callType)
        {
            string output = string.Empty;
            try
            {
                this._logger.Info("GetLogDataForVoice: Reading Log/Record data for the field: " + temp1.GetAsString("field-name"));
                this._logger.Info("KeyPrefix : " + keyPrefix);
                if (!String.IsNullOrEmpty(temp1.GetAsString(keyPrefix + ".user-data.key-name")) && message.UserData != null)
                {
                    output = GetSplitUserData(temp1.GetAsString(keyPrefix + ".user-data.key-name"), message.UserData);
                    this._logger.Info("GetLogDataForVoice: User-data value found : " + output);
                }
                if (String.IsNullOrWhiteSpace(output))
                {
                    if (!String.IsNullOrEmpty(temp1.GetAsString(keyPrefix + ".attrib.key-name")))
                    {
                        output = GetVoiceAttributeValueForLog(temp1, message, temp1.GetAsString(keyPrefix + ".attrib.key-name"), callType);
                        if (String.IsNullOrWhiteSpace(output))
                        {
                            if (!String.IsNullOrEmpty(temp1.GetAsString(keyPrefix + ".default.value")))
                            {
                                output = temp1.GetAsString(keyPrefix + ".default.value");
                                this._logger.Info("GetLogDataForVoice: default value found: " + output);
                            }
                        }
                        else
                        {
                            this._logger.Info("GetLogDataForVoice: attribute value found: " + output);
                        }
                    }
                    else if (!String.IsNullOrEmpty(temp1.GetAsString(keyPrefix + ".default.value")))
                    {
                        output = temp1.GetAsString(keyPrefix + ".default.value");
                        this._logger.Info("GetLogDataForVoice: default value found: " + output);
                    }
                }
            }
            catch (Exception generalException)
            {
                this._logger.Error("GetLogDataForVoice : Error Occurred while Collecting Log data : " + generalException.ToString());
            }

            if (String.IsNullOrWhiteSpace(output) && keyPrefix == "con")
            {
                output = GetLogDataForVoice(temp1, message, "inb", callType);
            }

            if (output != null)
                return output;
            else
                return string.Empty;
        }

        #endregion GetLogDataForVoice In Hierarchal Fashion

        #region GetLogDataForChat In Hierarchal Fashion

        public string GetLogDataForChat(KeyValueCollection temp1, IXNCustomData chatData, string keyPrefix, SFDCCallType callType)
        {
            string output = string.Empty;
            try
            {
                string fieldname = temp1.GetAsString("field-name");
                this._logger.Info("GetLogDataForChat: Reading Log/Record data for the field: " + temp1.GetAsString("field-name"));
                this._logger.Info("GetLogDataForChat: KeyPrefix : " + keyPrefix);
                if (!String.IsNullOrEmpty(temp1.GetAsString(keyPrefix + ".user-data.key-name")) && chatData.UserData != null)
                {
                    output = GetSplitUserData(temp1.GetAsString(keyPrefix + ".user-data.key-name"), chatData.UserData);
                    this._logger.Info("GetLogDataForChat: User-data value found : " + output);
                }
                if (String.IsNullOrWhiteSpace(output))
                {
                    if (!String.IsNullOrEmpty(temp1.GetAsString(keyPrefix + ".attrib.key-name")))
                    {
                        output = GetChatAttributeValueForLog(temp1, chatData, temp1.GetAsString(keyPrefix + ".attrib.key-name"), callType);
                        if (String.IsNullOrWhiteSpace(output))
                        {
                            if (!String.IsNullOrEmpty(temp1.GetAsString(keyPrefix + ".default.value")))
                            {
                                output = temp1.GetAsString(keyPrefix + ".default.value");
                                this._logger.Info("GetLogDataForChat: default value found: " + output);
                            }
                        }
                        else
                        {
                            this._logger.Info("GetLogDataForChat: attribute value found: " + output);
                        }
                    }
                    else if (!String.IsNullOrEmpty(temp1.GetAsString(keyPrefix + ".default.value")))
                    {
                        output = temp1.GetAsString(keyPrefix + ".default.value");
                        this._logger.Info("GetLogDataForChat: default value found: " + output);
                    }
                }
            }
            catch (Exception generalException)
            {
                this._logger.Error("GetLogDataForChat : Error Occurred while Collecting Log data : " + generalException.ToString());
            }

            if (String.IsNullOrWhiteSpace(output) && keyPrefix == "con")
            {
                output = GetLogDataForChat(temp1, chatData, "inb", callType);
            }

            if (output != null)
                return output;
            else
                return string.Empty;
        }

        #endregion GetLogDataForChat In Hierarchal Fashion

        #region GetLogDataForEmail In Hierarchal Fashion

        public string GetLogDataForEmail(KeyValueCollection temp1, IXNCustomData emailData, string keyPrefix, SFDCCallType callType)
        {
            string output = string.Empty;
            try
            {
                this._logger.Info("GetLogDataForEmail: Reading Log/Record data for the field: " + temp1.GetAsString("field-name"));
                this._logger.Info("GetLogDataForEmail: KeyPrefix : " + keyPrefix);
                if (!String.IsNullOrEmpty(temp1.GetAsString(keyPrefix + ".user-data.key-name")) && emailData != null && emailData.UserData != null)
                {
                    output = GetSplitUserData(temp1.GetAsString(keyPrefix + ".user-data.key-name"), emailData.UserData);
                    if (string.IsNullOrWhiteSpace(output) && emailData.DispositionCode != null)
                    {
                        if (temp1.GetAsString(keyPrefix + ".user-data.key-name").Equals(emailData.DispositionCode.Item1))
                        {
                            output = emailData.DispositionCode.Item2;
                        }
                    }

                    this._logger.Info("GetLogDataForEmail: User-data value found : " + output);
                }
                if (String.IsNullOrWhiteSpace(output))
                {
                    if (!String.IsNullOrEmpty(temp1.GetAsString(keyPrefix + ".attrib.key-name")))
                    {
                        output = GetEmailAttributeValueForLog(temp1.GetAsString(keyPrefix + ".attrib.key-name"), emailData, callType);

                        if (String.IsNullOrWhiteSpace(output))
                        {
                            if (!String.IsNullOrEmpty(temp1.GetAsString(keyPrefix + ".default.value")))
                            {
                                output = temp1.GetAsString(keyPrefix + ".default.value");
                                this._logger.Info("GetLogDataForEmail: default value found: " + output);
                            }
                        }
                        else
                        {
                            this._logger.Info("GetLogDataForEmail: attribute value found: " + output);
                        }
                    }
                    else if (!String.IsNullOrEmpty(temp1.GetAsString(keyPrefix + ".default.value")))
                    {
                        output = temp1.GetAsString(keyPrefix + ".default.value");
                        this._logger.Info("GetLogDataForEmail: default value found: " + output);
                    }
                }
            }
            catch (Exception generalException)
            {
                this._logger.Error("GetLogDataForEmail : Error Occurred while Collecting Log data : " + generalException.ToString());
            }

            if (String.IsNullOrWhiteSpace(output))
            {
                if (keyPrefix == "con" || keyPrefix == "workbin.inb")
                    output = GetLogDataForEmail(temp1, emailData, "inb", callType);
                else if (keyPrefix == "workbin.out")
                    output = GetLogDataForEmail(temp1, emailData, "out.success", callType);
            }

            if (output != null)
                return output;
            else
                return string.Empty;
        }

        #endregion GetLogDataForEmail In Hierarchal Fashion

        public string GetVoiceInlineAppendData(KeyValueCollection temp1, dynamic message, string keyPrefix, SFDCCallType callType)
        {
            string output = string.Empty;
            try
            {
                if (message == null)
                    return output;

                if (!String.IsNullOrEmpty(temp1.GetAsString(keyPrefix + ".append.user-data.key-name")) && message.UserData != null)
                    output = GetSplitUserData(temp1.GetAsString(keyPrefix + ".append.user-data.key-name"), message.UserData);
                if (String.IsNullOrWhiteSpace(output) && !String.IsNullOrEmpty(temp1.GetAsString(keyPrefix + ".append.attrib.key-name")))
                    return GetVoiceAttributeValueForLog(temp1, message, temp1.GetAsString(keyPrefix + ".append.attrib.key-name"), callType);
            }
            catch (Exception generalException)
            {
                this._logger.Error("GetVoiceInlineAppendData: " + generalException);
            }
            return output;
        }

        public string GetChatInlineAppendData(KeyValueCollection temp1, IXNCustomData chatData, string keyPrefix, SFDCCallType callType)
        {
            string output = string.Empty;
            try
            {
                if (!String.IsNullOrEmpty(temp1.GetAsString(keyPrefix + ".append.user-data.key-name")) && chatData.UserData != null)
                {
                    output = GetSplitUserData(temp1.GetAsString(keyPrefix + ".append.user-data.key-name"), chatData.UserData);
                }
                if (String.IsNullOrWhiteSpace(output) && !String.IsNullOrEmpty(temp1.GetAsString(keyPrefix + ".append.attrib.key-name")))
                {
                    return GetChatAttributeValueForLog(temp1, chatData, temp1.GetAsString(keyPrefix + ".append.attrib.key-name"), callType);
                }
            }
            catch (Exception generalException)
            {
                this._logger.Error("GetChatInlineAppendData: " + generalException);
            }
            return output;
        }

        public string GetEmailInlineAppendData(KeyValueCollection temp1, IXNCustomData emailData, string keyPrefix, SFDCCallType callType)
        {
            string output = string.Empty;
            try
            {
                if (!String.IsNullOrEmpty(temp1.GetAsString(keyPrefix + ".append.user-data.key-name")) && emailData != null && emailData.UserData != null)
                    output = GetSplitUserData(temp1.GetAsString(keyPrefix + ".append.user-data.key-name"), emailData.UserData);
                if (String.IsNullOrWhiteSpace(output) && !String.IsNullOrEmpty(temp1.GetAsString(keyPrefix + ".append.attrib.key-name")))
                    return GetEmailAttributeValueForLog(temp1.GetAsString(keyPrefix + ".append.attrib.key-name"), emailData, callType);
            }
            catch (Exception generalException)
            {
                this._logger.Error("GetEmailInlineAppendData: " + generalException);
            }
            return output;
        }

        #region GetSplitUserData

        public string GetSplitUserData(dynamic _input, dynamic _userdata)
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
                    this._logger.Warn("GetSplitUserData: userdata key is empty/null");
                }
            }
            catch (Exception generalException)
            {
                this._logger.Error("GetSplitUserData : Error Occurred while Collecting user data : " + generalException.ToString());
            }
            return null;
        }

        #endregion GetSplitUserData

        #region Format PhoneNumber

        private string FormatPhoneNumber(string format, string phone)
        {
            try
            {
                string data = string.Empty;
                char[] formatArray = format.ToCharArray();
                char[] phoneArray = phone.ToCharArray();
                int j = 0;
                for (int i = 0; i < formatArray.Length; i++)
                {
                    if (formatArray[i] == 'x')
                    {
                        if (phoneArray.Length > j)
                        {
                            data += phoneArray[j];
                            j++;
                        }
                    }
                    else
                    {
                        data += formatArray[i];
                    }
                }
                return data;
            }
            catch (Exception generalException)
            {
                this._logger.Error("FormatPhoneNumber : Error Occurred while formatting phone number : " + phone + " formate  :" + format + " Error :" + generalException.ToString());
            }
            return null;

            #region Old code

            //int length = format.Length < phone.Length ? format.Length : phone.Length;
            //char[] cformat = format.ToCharArray();
            //char[] cphone = phone.ToCharArray();
            //int d = 0;
            //string data = string.Empty;
            //for (int i = 0; i < length; i++)
            //{
            //    if (cformat[i] == 'x')
            //    {
            //        data += cphone[d];   //phone number assign
            //        d++;
            //    }
            //    else
            //    {
            //        data += cformat[i];//spl char assignment
            //        if (cformat.Length >= length + 1)
            //            length++;
            //    }
            //}
            //return data;

            #endregion Old code
        }

        #endregion Format PhoneNumber

        #region Validate SearchText

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

        #endregion Validate SearchText

        #region GetSearchString

        public string GetSearchString(string searchData, string searchCondition, string format = null)
        {
            try
            {
                this._logger.Info("GetSearchString() : " + format);
                searchData = searchData.Replace("^,", "").Replace(",^", "").Replace("^", "");
                string[] searchvalues = null;
                string searchstring = string.Empty;
                if (searchData.Contains(","))
                {
                    searchvalues = searchData.Split(',');
                }
                else
                {
                    searchvalues = new string[] { searchData };
                }

                foreach (string searchkey in searchvalues)
                {
                    if (Regex.IsMatch(searchkey, @"^\d+$") && searchkey.Length == 10 && !searchkey.StartsWith("1") && !string.IsNullOrWhiteSpace(format))
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
                this._logger.Error("GetSearchString : Error Occurred while getting search string  :" + generalException.ToString());
            }
            return null;
        }

        #endregion GetSearchString

        #region GetPrefixString

        public string GetPrefixString(SFDCCallType callType)
        {
            if (callType == SFDCCallType.InboundVoice)
            {
                return "inb";
            }
            else if (callType == SFDCCallType.OutboundVoiceSuccess)
            {
                return "out.success";
            }
            else if (callType == SFDCCallType.ConsultVoiceReceived)
            {
                return "con";
            }
            else if (callType == SFDCCallType.OutboundVoiceFailure)
            {
                return "out.fail";
            }
            else if (callType == SFDCCallType.InboundEmail)
            {
                return "inb";
            }
            else if (callType == SFDCCallType.InboundEmailPulled)
            {
                return "workbin.inb";
            }
            else if (callType == SFDCCallType.OutboundEmailPulled)
            {
                return "workbin.out";
            }
            else if (callType == SFDCCallType.InboundEmail)
            {
                return "inb";
            }
            else if (callType == SFDCCallType.OutboundEmailSuccess || callType == SFDCCallType.OutboundEmailFailure)
            {
                return "out.success";
            }
            else if (callType == SFDCCallType.InboundChat || callType == SFDCCallType.ConsultChatReceived)
            {
                return "inb";
            }

            return string.Empty;
        }

        #endregion GetPrefixString

        #region GetChatAttributeValueForSearch

        public string GetChatAttributeValueForSearch(IXNCustomData chatData, string[] attributeSearchKeys)
        {
            try
            {
                List<string> searchData = new List<string>();
                string finalSearchData = string.Empty;
                if (chatData != null)
                {
                    dynamic MyEntityAttributes = null;
                    if (chatData.EntityAttributes != null)
                    {
                        MyEntityAttributes = chatData.EntityAttributes;
                    }
                    //for (int i = 0; i <= attributeSearchKeys.Length - 1; i++)
                    //{
                    //switch (attributeSearchKeys[i].ToLower())
                    //{
                    //}
                    finalSearchData = searchData != null ? String.Join(",", searchData) : string.Empty;
                    return finalSearchData;
                    //}
                }
            }
            catch (Exception generalException)
            {
                this._logger.Error("GetChatAttributeValueForSearch : Reading Attribute Search Values, Keys : " + generalException.ToString());
            }
            return null;
        }

        #endregion GetChatAttributeValueForSearch

        #region GetEmailAttributeValueForSearch

        public string GetEmailAttributeValueForSearch(IXNCustomData emailData, string[] attributeSearchKeys)
        {
            try
            {
                List<string> searchData = new List<string>();
                // string[] searchData1 = new string[20];
                string finalSearchData = string.Empty;
                if (emailData != null)
                {
                    dynamic MyEntityAttributes = null;
                    if (emailData.EntityAttributes != null)
                    {
                        MyEntityAttributes = emailData.EntityAttributes;
                    }
                    for (int i = 0; i <= attributeSearchKeys.Length - 1; i++)
                    {
                        switch (attributeSearchKeys[i].ToLower())
                        {
                            case "to":
                                if (MyEntityAttributes != null)
                                {
                                    searchData.Add(string.Join(",", MyEntityAttributes.ToAddresses));
                                }
                                break;

                            case "from":
                                if (MyEntityAttributes != null)
                                {
                                    searchData.Add(MyEntityAttributes.FromAddress);
                                }
                                break;

                            case "cc":
                                if (MyEntityAttributes != null)
                                {
                                    searchData.Add(string.Join(",", MyEntityAttributes.CcAddresses));
                                }
                                break;

                            case "bcc":
                                if (MyEntityAttributes != null)
                                {
                                    searchData.Add(string.Join(",", MyEntityAttributes.BccAddresses));
                                }
                                break;

                            case "subject":
                                if (emailData.IXN_Attributes != null & !string.IsNullOrWhiteSpace(emailData.IXN_Attributes.Subject))
                                {
                                    searchData.Add(emailData.IXN_Attributes.Subject);
                                }
                                break;

                            case "frompersonal":
                                if (MyEntityAttributes != null)
                                {
                                    searchData.Add(MyEntityAttributes.FromPersonal);
                                }
                                break;

                            case "mailbox":
                                if (MyEntityAttributes != null)
                                {
                                    searchData.Add(MyEntityAttributes.Mailbox);
                                }
                                break;

                            default:
                                _logger.Error("GetEmailAttributeValueForSearch : Attribute Search Key - " + attributeSearchKeys[i] + " is invalid ");
                                break;
                        }
                    }
                    finalSearchData = searchData != null ? String.Join(",", searchData) : string.Empty;
                    return finalSearchData;
                }
            }
            catch (Exception generalException)
            {
                this._logger.Error("GetEmailAttributeValueForSearch : Reading Attribute Search Values, Keys : " + generalException.ToString());
            }
            return null;
        }

        #endregion GetEmailAttributeValueForSearch

        #region GetEmailAttributeValueForLog

        public string GetEmailAttributeValueForLog(string searchKey, IXNCustomData emailData, SFDCCallType callType)
        {
            string output = null;
            try
            {
                if (emailData != null && !String.IsNullOrWhiteSpace(searchKey))
                {
                    string[] keys = searchKey.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    dynamic MyEntityAttributes = null;
                    if (emailData.EntityAttributes != null)
                    {
                        MyEntityAttributes = emailData.EntityAttributes;
                    }

                    foreach (var key in keys)
                    {
                        if (String.IsNullOrWhiteSpace(output))
                        {
                            switch (key)
                            {
                                case "ixnid":
                                    output = emailData.InteractionId;
                                    break;

                                case "to":
                                    if (MyEntityAttributes != null)
                                    {
                                        output = (string.Join(",", MyEntityAttributes.ToAddresses));
                                    }
                                    break;

                                case "from":
                                    if (MyEntityAttributes != null)
                                    {
                                        output = (MyEntityAttributes.FromAddress);
                                    }
                                    break;

                                case "cc":
                                    if (MyEntityAttributes != null)
                                    {
                                        output = (string.Join(",", MyEntityAttributes.CcAddresses));
                                    }
                                    break;

                                case "bcc":
                                    if (MyEntityAttributes != null)
                                    {
                                        output = (string.Join(",", MyEntityAttributes.BccAddresses));
                                    }
                                    break;

                                case "subject":
                                    if (emailData.IXN_Attributes != null)
                                    {
                                        output = emailData.IXN_Attributes.Subject;
                                    }
                                    break;

                                case "comments":
                                    if (!string.IsNullOrWhiteSpace(emailData.InteractionNotes))
                                        output = emailData.InteractionNotes;
                                    else if (emailData.IXN_Attributes != null)
                                        output = emailData.IXN_Attributes.TheComment;
                                    break;

                                case "frompersonal":
                                    if (MyEntityAttributes != null)
                                    {
                                        output = MyEntityAttributes.FromPersonal;
                                    }
                                    break;

                                case "mailbox":
                                    if (MyEntityAttributes != null)
                                    {
                                        output = MyEntityAttributes.Mailbox;
                                    }
                                    break;

                                case "messagetext":
                                    if (emailData.InteractionContents != null)
                                    {
                                        output = emailData.InteractionContents.Text;
                                    }
                                    break;

                                case "contactid":
                                    if (emailData.IXN_Attributes != null)
                                    {
                                        output = emailData.IXN_Attributes.ContactId;
                                    }
                                    break;

                                case "ixn-type":
                                    if (emailData.IXN_Attributes != null)
                                    {
                                        output = emailData.IXN_Attributes.TypeId;
                                    }
                                    break;

                                case "ixn-subtype":
                                    if (emailData.IXN_Attributes != null)
                                    {
                                        output = emailData.IXN_Attributes.SubtypeId;
                                    }
                                    break;

                                case "media-type":
                                    if (emailData.IXN_Attributes != null)
                                    {
                                        output = emailData.IXN_Attributes.MediaTypeId;
                                    }
                                    break;

                                case "status":
                                    if (emailData.IXN_Attributes != null)
                                    {
                                        output = Convert.ToString(emailData.IXN_Attributes.Status);
                                    }
                                    break;

                                case "attachment-list":
                                    if (emailData.IXN_Attributes != null && emailData.IXN_Attributes.AllAttributes != null)
                                    {
                                        output = emailData.IXN_Attributes.AllAttributes.GetAsString("_AttachmentFileNames");
                                    }
                                    break;

                                default:
                                    break;
                            }
                        }
                    }
                }
            }
            catch (Exception generalException)
            {
                this._logger.Error("GetEmailAttributeValueForLog : Error occurred while Reading email Attribute  value for log, Exception: " + generalException.ToString());
            }
            return output;
        }

        #endregion GetEmailAttributeValueForLog

        #region GetEmailSearchValue

        public string GetEmailSearchValue(EmailOptions objectEmailOptions, IXNCustomData emailData, SFDCCallType callType)
        {
            try
            {
                this._logger.Info("GetEmailSearchValue :  Reading Searchkey for : " + objectEmailOptions.ObjectName);
                this._logger.Info("GetEmailSearchValue :  CallType Name : " + callType.ToString());
                string[] userDataSearchKeys = null;
                string[] attributeSearchKeys = null;
                string searchValue = string.Empty;
                if (callType == SFDCCallType.InboundEmail || callType == SFDCCallType.InboundEmailPulled)
                {
                    userDataSearchKeys = (objectEmailOptions.InboundSearchUserDataKeys != null) ? objectEmailOptions.InboundSearchUserDataKeys.Split(',') : null;
                    attributeSearchKeys = (objectEmailOptions.InboundSearchAttributeKeys != null) ? objectEmailOptions.InboundSearchAttributeKeys.Split(',') : null;
                }
                else if (callType == SFDCCallType.OutboundEmailSuccess || callType == SFDCCallType.OutboundEmailPulled)
                {
                    userDataSearchKeys = (objectEmailOptions.OutboundSearchUserDataKeys != null) ? objectEmailOptions.OutboundSearchUserDataKeys.Split(',') : null;
                    attributeSearchKeys = (objectEmailOptions.OutboundSearchAttributeKeys != null) ? objectEmailOptions.OutboundSearchAttributeKeys.Split(',') : null;
                }
                if (objectEmailOptions.SearchPriority == "user-data")
                {
                    if (userDataSearchKeys != null)
                    {
                        searchValue = GetUserDataSearchValues(emailData.UserData, userDataSearchKeys);
                        if (!ValidateSearchData(searchValue))
                        {
                            this._logger.Info("search data from user-data keys not found, Reading attribute search keys.....");
                            searchValue = GetEmailAttributeValueForSearch(emailData, attributeSearchKeys);
                        }
                    }
                    else if (attributeSearchKeys != null)
                    {
                        this._logger.Info("Either user-data search keys or user-data is null, Reading attribute search keys.....");
                        searchValue = GetEmailAttributeValueForSearch(emailData, attributeSearchKeys);
                    }
                }
                else if (objectEmailOptions.SearchPriority == "attribute")
                {
                    searchValue = GetEmailAttributeValueForSearch(emailData, attributeSearchKeys);
                    if (!ValidateSearchData(searchValue))
                    {
                        this._logger.Info("search data from attribute keys not found, Reading user-data search keys.....");
                        if (userDataSearchKeys != null && emailData.UserData != null)
                        {
                            searchValue = GetUserDataSearchValues(emailData.UserData, userDataSearchKeys);
                        }
                        else
                        {
                            this._logger.Info("Either user-data keys or attached data is null.....");
                        }
                    }
                }
                else if (objectEmailOptions.SearchPriority == "both")
                {
                    if (userDataSearchKeys != null)
                    {
                        searchValue = GetUserDataSearchValues(emailData.UserData, userDataSearchKeys);
                        if (searchValue != string.Empty)
                        {
                            string temp = GetEmailAttributeValueForSearch(emailData, attributeSearchKeys);
                            if (temp != string.Empty)
                            {
                                searchValue += "," + temp;
                            }
                        }
                    }
                    else
                    {
                        searchValue = GetEmailAttributeValueForSearch(emailData, attributeSearchKeys);
                    }
                }
                return searchValue;
            }
            catch (Exception generalException)
            {
                this._logger.Error("GetEmailSearchValue : Error occurred while reading Email search value : " + generalException.ToString());
            }
            return null;
        }

        #endregion GetEmailSearchValue

        #region GetChatSearchValue

        public string GetChatSearchValue(ChatOptions chatOptions, IXNCustomData ixnData, SFDCCallType callType)
        {
            try
            {
                this._logger.Info("GetChatSearchValue :  Reading Searchkey for : " + chatOptions.ObjectName);
                this._logger.Info("GetChatSearchValue :  CallType Name : " + callType.ToString());
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
                    if (userDataSearchKeys != null)
                    {
                        searchValue = GetUserDataSearchValues(ixnData.UserData, userDataSearchKeys);
                        if (!ValidateSearchData(searchValue))
                        {
                            this._logger.Info("search data from user-data keys not found, Reading attribute search keys.....");
                            searchValue = GetChatAttributeValueForSearch(ixnData, attributeSearchKeys);
                        }
                    }
                    else if (attributeSearchKeys != null)
                    {
                        this._logger.Info("Either user-data search keys or user-data is null, Reading attribute search keys.....");
                        searchValue = GetChatAttributeValueForSearch(ixnData, attributeSearchKeys);
                    }
                }
                else if (chatOptions.SearchPriority == "attribute")
                {
                    searchValue = GetChatAttributeValueForSearch(ixnData, attributeSearchKeys);
                    if (!ValidateSearchData(searchValue))
                    {
                        this._logger.Info("search data from attribute keys not found, Reading user-data search keys.....");
                        if (userDataSearchKeys != null && ixnData.UserData != null)
                        {
                            searchValue = GetUserDataSearchValues(ixnData.UserData, userDataSearchKeys);
                        }
                        else
                        {
                            this._logger.Info("Either user-data keys or attached data is null.....");
                        }
                    }
                }
                else if (chatOptions.SearchPriority == "both")
                {
                    if (userDataSearchKeys != null)
                    {
                        searchValue = GetUserDataSearchValues(ixnData.UserData, userDataSearchKeys);
                        if (searchValue != string.Empty)
                        {
                            string temp = GetChatAttributeValueForSearch(ixnData, attributeSearchKeys);
                            if (temp != string.Empty)
                            {
                                searchValue += "," + temp;
                            }
                        }
                    }
                    else
                    {
                        searchValue = GetChatAttributeValueForSearch(ixnData, attributeSearchKeys);
                    }
                }
                return searchValue;
            }
            catch (Exception generalException)
            {
                this._logger.Error("GetChatSearchValue : Error occurred while reading Lead Data : " + generalException.ToString());
            }
            return null;
        }

        #endregion GetChatSearchValue

        #region GetVoiceSearchValue

        public string GetVoiceSearchValue(VoiceOptions voiceOptions, IMessage message, SFDCCallType callType)
        {
            try
            {
                this._logger.Info("GetVoiceSearchValue :  Reading Searchkey for : " + voiceOptions.ObjectName);
                this._logger.Info("GetVoiceSearchValue :  CallType Name : " + callType.ToString());
                string[] userDataSearchKeys = null;
                string[] attributeSearchKeys = null;
                string searchValue = string.Empty;
                dynamic popupEvent = Convert.ChangeType(message, message.GetType());
                if (popupEvent != null)
                {
                    if (callType == SFDCCallType.InboundVoice)
                    {
                        userDataSearchKeys = (voiceOptions.InboundSearchUserDataKeys != null) ? voiceOptions.InboundSearchUserDataKeys.Split(',') : null;
                        attributeSearchKeys = (voiceOptions.InboundSearchAttributeKeys != null) ? voiceOptions.InboundSearchAttributeKeys.Split(',') : null;
                    }
                    else if (callType == SFDCCallType.OutboundVoiceSuccess || callType == SFDCCallType.OutboundVoiceFailure)
                    {
                        userDataSearchKeys = (voiceOptions.OutboundSearchUserDataKeys != null) ? voiceOptions.OutboundSearchUserDataKeys.Split(',') : null;
                        attributeSearchKeys = (voiceOptions.OutboundSearchAttributeKeys != null) ? voiceOptions.OutboundSearchAttributeKeys.Split(',') : null;
                    }
                    else if (callType == SFDCCallType.ConsultVoiceReceived)
                    {
                        userDataSearchKeys = (voiceOptions.ConsultSearchUserDataKeys != null) ? voiceOptions.ConsultSearchUserDataKeys.Split(',') : null;
                        attributeSearchKeys = (voiceOptions.ConsultSearchAttributeKeys != null) ? voiceOptions.ConsultSearchAttributeKeys.Split(',') : null;
                    }

                    if (voiceOptions.SearchPriority == "user-data")
                    {
                        if (userDataSearchKeys != null)
                        {
                            searchValue = GetUserDataSearchValues(popupEvent.UserData, userDataSearchKeys);
                            if (!ValidateSearchData(searchValue))
                            {
                                this._logger.Info("search data from user-data keys not found, Reading attribute search keys.....");
                                searchValue = GetVoiceAttributeSearchValues(message, attributeSearchKeys);
                            }
                        }
                        else if (attributeSearchKeys != null)
                        {
                            this._logger.Info("Either user-data search keys or user-data is null, Reading attribute search keys.....");
                            searchValue = GetVoiceAttributeSearchValues(message, attributeSearchKeys);
                        }
                    }
                    else if (voiceOptions.SearchPriority == "attribute")
                    {
                        searchValue = GetVoiceAttributeSearchValues(message, attributeSearchKeys);
                        if (!ValidateSearchData(searchValue))
                        {
                            this._logger.Info("search data from attribute keys not found, Reading user-data search keys.....");
                            if (userDataSearchKeys != null && popupEvent.UserData != null)
                            {
                                searchValue = GetUserDataSearchValues(popupEvent.UserData, userDataSearchKeys);
                            }
                            else
                            {
                                this._logger.Info("Either user-data keys or attached data is null.....");
                            }
                        }
                    }
                    else if (voiceOptions.SearchPriority == "both")
                    {
                        if (userDataSearchKeys != null)
                        {
                            searchValue = GetUserDataSearchValues(popupEvent.UserData, userDataSearchKeys);
                            if (searchValue != string.Empty)
                            {
                                string temp = GetVoiceAttributeSearchValues(message, attributeSearchKeys);
                                if (temp != string.Empty)
                                {
                                    searchValue += "," + temp;
                                }
                            }
                        }
                        else
                        {
                            searchValue = GetVoiceAttributeSearchValues(message, attributeSearchKeys);
                        }
                    }
                }
                return searchValue;
            }
            catch (Exception generalException)
            {
                this._logger.Error("GetVoiceSearchValue : Error occurred while reading Lead Data : " + generalException.ToString());
            }
            return null;
        }

        #endregion GetVoiceSearchValue

        #region GetInteractionType

        public CustomMediaType GetCurrentInteractionType(SFDCCallType callType)
        {
            switch (callType)
            {
                case SFDCCallType.InboundVoice:
                case SFDCCallType.OutboundVoiceSuccess:
                case SFDCCallType.OutboundVoiceFailure:
                case SFDCCallType.ConsultVoiceReceived:

                    return CustomMediaType.Voice;

                case SFDCCallType.InboundChat:
                case SFDCCallType.ConsultChatReceived:

                    return CustomMediaType.Chat;

                case SFDCCallType.InboundEmail:
                case SFDCCallType.OutboundEmailSuccess:
                case SFDCCallType.OutboundEmailFailure:
                case SFDCCallType.InboundEmailPulled:
                case SFDCCallType.OutboundEmailPulled:
                    return CustomMediaType.Email;
            }
            return CustomMediaType.Voice;
        }

        #endregion GetInteractionType

        #region GetChatAttributeValueForLog

        public string GetChatAttributeValueForLog(KeyValueCollection collection, IXNCustomData chatData, string searchKey, SFDCCallType callType)
        {
            string output = string.Empty;
            try
            {
                dynamic interactionEvent = chatData.InteractionEvent;
                if (interactionEvent != null && !String.IsNullOrWhiteSpace(searchKey))
                {
                    string[] keys = searchKey.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (var key in keys)
                    {
                        if (String.IsNullOrWhiteSpace(output))
                        {
                            switch (key)
                            {
                                case "ixnid":
                                    output = interactionEvent.Interaction.InteractionId.ToString();
                                    break;

                                case "state":
                                    output = interactionEvent.Interaction.InteractionState.ToString();
                                    break;

                                case "moved-to-queue-at":
                                    output = interactionEvent.Interaction.InteractionMovedToQueueAt.ToString();
                                    break;

                                case "queue":
                                    output = interactionEvent.Interaction.InteractionQueue.ToString();
                                    break;

                                case "subtype":
                                    output = interactionEvent.Interaction.InteractionSubtype.ToString();
                                    break;

                                case "media-type":
                                    output = interactionEvent.Interaction.InteractionMediatype.ToString();
                                    break;

                                case "received-time":
                                    output = interactionEvent.Interaction.InteractionReceivedAt.ToString();
                                    break;

                                case "submitted-time":
                                    output = interactionEvent.Interaction.InteractionSubmittedAt.ToString();
                                    break;

                                case "submitted-by":
                                    output = interactionEvent.Interaction.InteractionSubmittedBy.ToString();
                                    break;

                                case "subject":
                                    if (chatData.IXN_Attributes != null)
                                        output = chatData.IXN_Attributes.Subject;
                                    break;

                                case "startdate":
                                    if (chatData.IXN_Attributes != null && chatData.IXN_Attributes.StartDate != null)
                                        output = chatData.IXN_Attributes.StartDate.ToString();
                                    break;

                                case "contactid":
                                    if (chatData.IXN_Attributes != null)
                                        output = chatData.IXN_Attributes.ContactId;
                                    break;

                                case "comments":
                                    if (!string.IsNullOrWhiteSpace(chatData.InteractionNotes))
                                        output = chatData.InteractionNotes;
                                    else if (chatData.IXN_Attributes != null)
                                        output = chatData.IXN_Attributes.TheComment;
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
                    this._logger.Info("GetChatAttributeValueForLog: Attribute key is null or empty");
                }
            }
            catch (Exception generalException)
            {
                this._logger.Error("GetChatAttributeValueForLog : Reading Attribute Search Values, Keys : " + generalException.ToString());
            }
            return output;
        }

        #endregion GetChatAttributeValueForLog

        #region GetFormattedChatData

        /// <summary>
        /// Gets Formatted Chat data
        /// </summary>
        /// <param name="xmldata"></param>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        internal string GetFormattedChatData(string structuredText, string sessionId)
        {
            try
            {
                string xmldata = structuredText;
                _logger.Info("GetFormattedChatData : Formatting the Chat Content : " + xmldata);
                if (!string.IsNullOrWhiteSpace(xmldata))
                {
                    _xml.LoadXml(xmldata);
                    _rootNode = _xml.GetElementsByTagName("chatTranscript");
                    _newParty = _xml.GetElementsByTagName("newParty");
                    IDictionary<string, string> userInfo = new Dictionary<string, string>();
                    string ChatMessage = string.Empty;

                    if (_rootNode != null)
                    {
                        DateTime chatStartTime = Convert.ToDateTime(_rootNode[0].Attributes["startAt"].Value);
                        XmlNodeList nodes = _rootNode[0].ChildNodes;
                        foreach (XmlNode node in nodes)
                        {
                            if (node.Name == "newParty")
                            {
                                foreach (XmlNode party in node.ChildNodes)
                                {
                                    if (party.Name == "userInfo")
                                    {
                                        if (party.Attributes["userType"].Value == "CLIENT")
                                        {
                                            userInfo.Add(node.Attributes["userId"].Value, "Customer_" + party.Attributes["userNick"].Value);
                                            ChatMessage += "[" + chatStartTime.AddSeconds(int.Parse(node.Attributes["timeShift"].Value)).ToString("hh:mm:ss tt") + "] : New Party " + userInfo[node.Attributes["userId"].Value] + " has joined the session.\n";
                                        }
                                        else if (party.Attributes["userType"].Value == "AGENT")
                                        {
                                            userInfo.Add(node.Attributes["userId"].Value, "Agent_" + party.Attributes["userNick"].Value);
                                            ChatMessage += "[" + chatStartTime.AddSeconds(int.Parse(node.Attributes["timeShift"].Value)).ToString("hh:mm:ss tt") + "] : New Party " + userInfo[node.Attributes["userId"].Value] + "  has joined the session.\n";
                                        }
                                    }
                                }
                            }
                            else if (node.Name == "message")
                            {
                                ChatMessage += "[" + chatStartTime.AddSeconds(int.Parse(node.Attributes["timeShift"].Value)).ToString("hh:mm:ss tt") + "] :" + userInfo[node.Attributes["userId"].Value] + "  : " + node.FirstChild.InnerText + "\n";
                            }
                            else if (node.Name == "partyLeft")
                            {
                                ChatMessage += "[" + chatStartTime.AddSeconds(int.Parse(node.Attributes["timeShift"].Value)).ToString("hh:mm:ss tt") + "] : Party " + userInfo[node.Attributes["userId"].Value] + "   has left the session. " + node.FirstChild.InnerText + "\n";
                            }
                        }
                    }
                    else
                    {
                        this._logger.Error("GetFormattedChatData : ChatTranscript is null.");
                    }

                    return ChatMessage;
                }
                else
                {
                    this._logger.Info("GetFormattedChatData: Chat content is null or empty for the interactionId :" + sessionId);
                }
            }
            catch (Exception generalException)
            {
                this._logger.Error("GetFormattedChatData : Error occurred :" + generalException.ToString());
            }
            return string.Empty;
        }

        #endregion GetFormattedChatData
    }
}