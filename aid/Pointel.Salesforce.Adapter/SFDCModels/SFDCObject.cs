using Genesyslab.Platform.Commons.Collections;
using Genesyslab.Platform.Commons.Protocols;
using Genesyslab.Platform.Voice.Protocols.TServer.Events;
using Pointel.Salesforce.Adapter.LogMessage;
using Pointel.Salesforce.Adapter.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pointel.Salesforce.Adapter.SFDCModels
{
    public class SFDCObject
    {
        #region Field Declarations
        private Log logger = null;
        private static SFDCObject currentObject = null;
        string field = "";
        string value = "";
        string fieldType = string.Empty;
        string fieldName = string.Empty;
        bool updatefield = false;
        bool enableTimeStamp = false;
        #endregion

        #region Constructor
        private SFDCObject()
        {
            this.logger = Log.GenInstance();
        }
        #endregion

        #region GetInstance
        public static SFDCObject GetInstance()
        {
            if (currentObject == null)
            {
                currentObject = new SFDCObject();
            }
            return currentObject;
        }
        #endregion

        #region Get User-Data SearchValues
        public string GetUserDataSearchValues(KeyValueCollection userData, string[] userDataSearchKeys)
        {
            try
            {
                if (userData != null && userDataSearchKeys != null)
                {
                    string searchValues = string.Empty;
                    this.logger.Info("GetUserDataSearchValues : Reading User-Data Search Values, Keys : " + string.Join(",", userDataSearchKeys));
                    foreach (string key in userDataSearchKeys)
                    {
                        if (userData.ContainsKey(key))
                        {
                            searchValues += userData.GetAsString(key) + ",";
                        }
                        else
                        {
                            searchValues += "^,";
                        }
                    }

                    return (searchValues != string.Empty) ? searchValues.Substring(0, searchValues.Length - 1) : searchValues;
                }
                else
                {
                    this.logger.Info("GetUserDataSearchValues : user-data search key is empty");
                }
            }
            catch (Exception generalException)
            {
                this.logger.Error("GetSearchUserDataValue : Error occurred while getting User-Data Search values : " + generalException.ToString());
            }
            return null;
        }
        #endregion

        #region Get Attribute SearchValues
        public string GetAttributeSearchValues(IMessage message, string[] attributeSearchKeys)
        {
            try
            {
                if (attributeSearchKeys != null && message != null)
                {
                    string searchValues = string.Empty;
                    this.logger.Info("GetAttributeSearchValues : Reading Attribute Search Values, Keys : " + string.Join(",", attributeSearchKeys));
                    dynamic voiceEvent = null;
                    switch (message.Id)
                    {
                        case EventRinging.MessageId:
                            voiceEvent = (EventRinging)message;
                            break;
                        case EventEstablished.MessageId:
                            voiceEvent = (EventEstablished)message;
                            break;
                        case EventReleased.MessageId:
                            voiceEvent = (EventReleased)message;
                            break;
                        case EventDialing.MessageId:
                            voiceEvent = (EventDialing)message;
                            break;
                        case EventError.MessageId:
                            voiceEvent = (EventError)message;
                            break;
                        case EventAbandoned.MessageId:
                            voiceEvent = (EventAbandoned)message;
                            break;
                        case EventDestinationBusy.MessageId:
                            voiceEvent = (EventDestinationBusy)message;
                            break;
                        case Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events.EventInvite.MessageId:
                            voiceEvent = (Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events.EventInvite)message;
                            break;
                        case Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events.EventPartyAdded.MessageId:
                            voiceEvent = (Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events.EventPartyAdded)message;
                            break;
                        case Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events.EventPartyRemoved.MessageId:
                            voiceEvent = (Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events.EventPartyRemoved)message;
                            break;
                        default:
                            break;
                    }

                    if (voiceEvent != null)
                    {
                        #region EventRinging Attribute data
                        foreach (string key in attributeSearchKeys)
                        {
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
                                    break;
                            }
                        }
                        #endregion
                    }


                    return (searchValues != string.Empty) ? searchValues.Substring(0, searchValues.Length - 1) : searchValues;
                }
                else
                {
                    this.logger.Info("GetAttributeSearchValues : attribute search key is empty");
                }
            }
            catch (Exception generalException)
            {
                this.logger.Error("GetSearchAttributeValue : Error occurred  while getting Attribute Search values : " + generalException.ToString());
            }
            return null;
        }
        #endregion

        #region Get Truncated number
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
        #endregion

        #region Get AttributeValue ForLog
        public string GetAttributeValueForLog(KeyValueCollection collection, IMessage Message, SFDCCallType callType)
        {
            try
            {
                dynamic interactionEvent = null;
                string keyPrefix = string.Empty;
                switch (Message.Id)
                {
                    case EventRinging.MessageId:
                        interactionEvent = (EventRinging)Message;
                        break;
                    case EventEstablished.MessageId:
                        interactionEvent = (EventEstablished)Message;
                        break;
                    case EventReleased.MessageId:
                        interactionEvent = (EventReleased)Message;
                        break;
                    case EventDialing.MessageId:
                        interactionEvent = (EventDialing)Message;
                        break;
                    case EventError.MessageId:
                        interactionEvent = (EventError)Message;
                        break;
                    case EventAbandoned.MessageId:
                        interactionEvent = (EventAbandoned)Message;
                        break;
                    case EventDestinationBusy.MessageId:
                        interactionEvent = (EventDestinationBusy)Message;
                        break;
                    case EventAttachedDataChanged.MessageId:
                        interactionEvent = (EventAttachedDataChanged)Message;
                        break;
                    case EventUserEvent.MessageId:
                        interactionEvent = (EventUserEvent)Message;
                        break;
                    case Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events.EventInvite.MessageId:
                        interactionEvent = (Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events.EventInvite)Message;
                        break;
                    default:
                        break;
                }

                if (interactionEvent != null)
                {
                    if (callType == SFDCCallType.Inbound || callType == SFDCCallType.InboundChat || 
                        callType == SFDCCallType.ConsultReceived || callType == SFDCCallType.ConsultChatReceived)
                    {
                        keyPrefix = "inb";
                    }
                    else if (callType == SFDCCallType.OutboundSuccess)
                    {
                        keyPrefix = "out.success";
                    }
                    else if (callType == SFDCCallType.OutboundFailure)
                    {
                        keyPrefix = "out.fail";
                    }
                    else if (callType == SFDCCallType.ConsultSuccess)
                    {
                        keyPrefix = "con.success";
                    }
                    else if (callType == SFDCCallType.ConsultFailure)
                    {
                        keyPrefix = "con.fail";
                    }

                    if (collection.GetAsString(keyPrefix + ".attrib.key-name").ToLower().Equals("ani"))
                    {
                        return interactionEvent.ANI;
                    }
                    else if (collection.GetAsString(keyPrefix + ".attrib.key-name").ToLower().Equals("thisdn"))
                    {
                        return interactionEvent.ThisDN;
                    }
                    else if (collection.GetAsString(keyPrefix + ".attrib.key-name").ToLower().Equals("otherdn"))
                    {
                        return interactionEvent.OtherDN;
                    }
                    else if (collection.GetAsString(keyPrefix + ".attrib.key-name").ToLower().Equals("connid"))
                    {
                        return interactionEvent.ConnID.ToString();
                    }
                    else if (collection.GetAsString(keyPrefix + ".attrib.key-name").ToLower().Equals("agentid"))
                    {
                        return interactionEvent.AgentID;
                    }
                    else if (collection.GetAsString(keyPrefix + ".attrib.key-name").ToLower().Equals("dnis"))
                    {
                        return interactionEvent.DNIS;
                    }
                    else if (collection.GetAsString(keyPrefix + ".attrib.key-name").ToLower().Equals("calltype"))
                    {
                        return interactionEvent.CallType.ToString();
                    }
                }

            }
            catch (Exception generalException)
            {
                this.logger.Error("GetAttributeValueForLog : Reading Attribute Search Values, Keys : " + generalException.ToString());
            }
            return null;
        }
        #endregion

        #region GetVoiceActivityLog
        public string GetVoiceActivityLog(KeyValueCollection activityLog, IMessage Message, SFDCCallType callType)
        {
            try
            {
                this.logger.Info("GetVoiceActivityLog : Collecting Activity Logs for the callType : " + callType.ToString());

                if (activityLog != null)
                {
                    string ActivityHistory = "&";
                    string keyPrefix = string.Empty;
                    dynamic interactionEvent = null;

                    #region EventType
                    if (callType == SFDCCallType.Inbound || callType == SFDCCallType.ConsultReceived)
                    {
                        keyPrefix = "inb";
                    }
                    else if (callType == SFDCCallType.OutboundSuccess)
                    {
                        keyPrefix = "out.success";
                    }
                    else if (callType == SFDCCallType.OutboundFailure)
                    {
                        keyPrefix = "out.fail";
                    }
                    else if (callType == SFDCCallType.ConsultSuccess)
                    {
                        keyPrefix = "con.success";
                    }
                    else if (callType == SFDCCallType.ConsultFailure)
                    {
                        keyPrefix = "con.fail";
                    }

                    #endregion

                    #region Event Intialize
                    switch (Message.Id)
                    {
                        case EventRinging.MessageId:
                            interactionEvent = (EventRinging)Message;
                            break;
                        case EventEstablished.MessageId:
                            interactionEvent = (EventEstablished)Message;
                            break;
                        case EventReleased.MessageId:
                            interactionEvent = (EventReleased)Message;
                            break;
                        case EventDialing.MessageId:
                            interactionEvent = (EventDialing)Message;
                            break;
                        case EventError.MessageId:
                            interactionEvent = (EventError)Message;
                            break;
                        case EventAbandoned.MessageId:
                            interactionEvent = (EventAbandoned)Message;
                            break;
                        case EventDestinationBusy.MessageId:
                            interactionEvent = (EventDestinationBusy)Message;
                            break;
                        case Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events.EventInvite.MessageId:
                            interactionEvent = (Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events.EventInvite)Message;
                            break;
                        case Genesyslab.Platform.WebMedia.Protocols.BasicChat.Events.EventSessionInfo.MessageId:
                            interactionEvent = (Genesyslab.Platform.WebMedia.Protocols.BasicChat.Events.EventSessionInfo)Message;
                            break;
                        default:
                            break;
                    }
                    #endregion
                    foreach (KeyValueCollection temp1 in activityLog.AllValues)
                    {
                        field = string.Empty;
                        value = string.Empty;
                        updatefield = false;
                        enableTimeStamp = false;
                        fieldName = temp1.GetAsString("field-name");
                        fieldType = temp1.GetAsString("field-type");
                        if (!String.IsNullOrEmpty(temp1.GetAsString("enable.update")) && temp1.GetAsString("enable.update").Trim().ToLower()=="true")
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
                                    field = fieldName.Trim() + "=";
                                    value += temp1.GetAsString("record-type.id");
                                }
                            }
                            else
                            {
                                field = fieldName.Trim() + "="; 
                                if (!String.IsNullOrEmpty(fieldType))
                                { fieldType = fieldType.Trim().ToLower(); }
                                if (String.IsNullOrEmpty(fieldType)|| fieldType== "text" || fieldType== "number")
                                {
                                    if (!String.IsNullOrEmpty(temp1.GetAsString(keyPrefix + ".user-data.key-name")) && interactionEvent.UserData != null)
                                    {
                                        value += interactionEvent.UserData.GetAsString(temp1.GetAsString(keyPrefix + ".user-data.key-name"));
                                    }
                                    else if (!String.IsNullOrEmpty(temp1.GetAsString(keyPrefix + ".attrib.key-name")))
                                    {
                                        value += GetAttributeValueForLog(temp1, interactionEvent, callType);
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
                        if (field != string.Empty)
                        {
                            ActivityHistory += field + value + "&";
                        }

                    }
                    return ActivityHistory.Substring(0, ActivityHistory.Length - 1);
                }
            }
            catch (Exception generalException)
            {
                this.logger.Error("GetVoiceActivityLog : Error Occurred while Collecting activity Logs : " + generalException.ToString());
            }

            return null;
        }
        #endregion

        #region GetVoiceUpdateActivityLog
        public string GetVoiceUpdateActivityLog(KeyValueCollection activityLog, IMessage Message, SFDCCallType callType, string duration)
        {
            try
            {
                this.logger.Info("GetVoiceUpdateActivityLog : Collection update log for the callType : " + callType.ToString());
                if (activityLog != null)
                {
                    string ActivityHistory = "&";
                    string keyPrefix = string.Empty;
                    dynamic voiceEvent = null;

                    #region EventType
                    if (callType == SFDCCallType.Inbound || callType == SFDCCallType.ConsultReceived)
                    {
                        keyPrefix = "inb";
                    }
                    else if (callType == SFDCCallType.OutboundSuccess)
                    {
                        keyPrefix = "out.success";
                    }
                    else if (callType == SFDCCallType.OutboundFailure)
                    {
                        keyPrefix = "out.fail";
                    }
                    else if (callType == SFDCCallType.ConsultSuccess)
                    {
                        keyPrefix = "con.success";
                    }
                    else if (callType == SFDCCallType.ConsultFailure)
                    {
                        keyPrefix = "con.fail";
                    }
                    #endregion

                    switch (Message.Id)
                    {
                        #region Events
                        
                        case EventReleased.MessageId:
                            voiceEvent = (EventReleased)Message;
                            break;
                        case EventAttachedDataChanged.MessageId:
                            voiceEvent = (EventAttachedDataChanged)Message;
                            break;
                        case EventUserEvent.MessageId:
                            voiceEvent = (EventUserEvent)Message;
                            break;
                        default:
                            break;
                        #endregion
                    }
                    foreach (KeyValueCollection temp1 in activityLog.AllValues)
                    {
                        field = string.Empty;
                        value = string.Empty;
                        updatefield = false;
                        enableTimeStamp = false;
                        fieldName = temp1.GetAsString("field-name");
                        fieldType = temp1.GetAsString("field-type");
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
                                    field = fieldName.Trim() + "=";
                                    value += temp1.GetAsString("record-type.id");
                                }
                            }
                            else
                            {
                                field = fieldName.Trim() + "=";
                                if (!String.IsNullOrEmpty(fieldType))
                                { fieldType = fieldType.Trim().ToLower(); }
                                if (String.IsNullOrEmpty(fieldType) || fieldType == "text" || fieldType == "number")
                                {
                                    if (!String.IsNullOrEmpty(temp1.GetAsString(keyPrefix + ".user-data.key-name")) && voiceEvent.UserData != null)
                                    {
                                        value += voiceEvent.UserData.GetAsString(temp1.GetAsString(keyPrefix + ".user-data.key-name"));
                                    }
                                    else if (!String.IsNullOrEmpty(temp1.GetAsString(keyPrefix + ".attrib.key-name")))
                                    {
                                        value += GetAttributeValueForLog(temp1, voiceEvent, callType);
                                    }
                                    else if (!String.IsNullOrEmpty(temp1.GetAsString(keyPrefix + ".default.value")))
                                    {
                                        value += temp1.GetAsString(keyPrefix + ".default.value");
                                    }
                                    if (enableTimeStamp && fieldType!="number")
                                    {
                                        value += System.DateTime.Now.ToString();
                                    }
                                }
                                else if(fieldType=="duration")
                                {
                                    value += duration;
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
                        if (field != string.Empty)
                        {
                            ActivityHistory += field + value + "&";
                        }
                    }
                    return ActivityHistory.Substring(0, ActivityHistory.Length - 1);
                }
            }
            catch (Exception generalException)
            {
                this.logger.Error("GetVoiceUpdateActivityLog : Error Occurred while Collecting activity Logs : " + generalException.ToString());
            }
            return null;
        }
        #endregion

        #region GetChatActivityLog
        public string GetChatActivityLog(KeyValueCollection activityLog, IMessage Message, SFDCCallType callType)
        {
            try
            {
                this.logger.Error("GetChatActivityLog : Collecting Activity Logs for the callType : " + callType.ToString());

                if (activityLog != null)
                {
                    string ActivityHistory = "&";
                    string keyPrefix = string.Empty;
                    dynamic interactionEvent = null;

                    #region EventType
                    if (callType == SFDCCallType.InboundChat || callType == SFDCCallType.ConsultChatReceived)
                    {
                        keyPrefix = "inb";
                    }
                    #endregion

                    #region Event Intialize
                    switch (Message.Id)
                    {
                        case Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events.EventInvite.MessageId:
                            interactionEvent = (Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events.EventInvite)Message;
                            break;                       
                        case Genesyslab.Platform.WebMedia.Protocols.BasicChat.Events.EventSessionInfo.MessageId:
                            interactionEvent = (Genesyslab.Platform.WebMedia.Protocols.BasicChat.Events.EventSessionInfo)Message;
                            break;
                        default:
                            break;
                    }
                    #endregion
                    foreach (KeyValueCollection temp1 in activityLog.AllValues)
                    {
                        field = string.Empty;
                        value = string.Empty;
                        updatefield = false;
                        enableTimeStamp = false;
                        fieldName = temp1.GetAsString("field-name");
                        fieldType = temp1.GetAsString("field-type");
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
                                    field = fieldName.Trim() + "=";
                                    value += temp1.GetAsString("record-type.id");
                                }
                            }
                            else
                            {
                                field = fieldName.Trim() + "=";
                                if (!String.IsNullOrEmpty(fieldType))
                                { fieldType = fieldType.Trim().ToLower(); }
                                if (String.IsNullOrEmpty(fieldType) || fieldType == "text" || fieldType == "number")
                                {
                                    if (!String.IsNullOrEmpty(temp1.GetAsString(keyPrefix + ".user-data.key-name")) && interactionEvent.Interaction.InteractionUserData != null)
                                    {
                                        value += interactionEvent.Interaction.InteractionUserData.GetAsString(temp1.GetAsString(keyPrefix + ".user-data.key-name"));
                                    }
                                    else if (!String.IsNullOrEmpty(temp1.GetAsString(keyPrefix + ".attrib.key-name")))
                                    {
                                        value += GetAttributeValueForLog(temp1, interactionEvent, callType);
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
                        if (field != string.Empty)
                        {
                            ActivityHistory += field + value + "&";
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
        #endregion

        #region GetChatUpdateActivityLog
        public string GetChatUpdateActivityLog(KeyValueCollection activityLog, IMessage Message, SFDCCallType callType, string duration, string chatContent)
        {
            try
            {
                this.logger.Info("GetUpdateActivityLog : Collection update log for the callType : " + callType.ToString());
                if (activityLog != null)
                {
                    string ActivityHistory = "&";
                    string keyPrefix = string.Empty;
                    dynamic chatEvents = null;

                    #region EventType
                    if (callType == SFDCCallType.InboundChat || callType == SFDCCallType.ConsultChatReceived)
                    {
                        keyPrefix = "inb";
                    }
                    #endregion

                    switch (Message.Id)
                    {
                        #region Events
                        case Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events.EventInvite.MessageId:
                            chatEvents = (Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events.EventInvite)Message;
                            break;
                        case Genesyslab.Platform.WebMedia.Protocols.BasicChat.Events.EventSessionInfo.MessageId:
                            chatEvents = (Genesyslab.Platform.WebMedia.Protocols.BasicChat.Events.EventSessionInfo)Message;
                            break;
                        default:
                            break;
                        #endregion
                    }
                    foreach (KeyValueCollection temp1 in activityLog.AllValues)
                    {
                        field = string.Empty;
                        value = string.Empty;
                        updatefield = false;
                        enableTimeStamp = false;
                        fieldName = temp1.GetAsString("field-name");
                        fieldType = temp1.GetAsString("field-type");
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
                                    field = fieldName.Trim() + "=";
                                    value += temp1.GetAsString("record-type.id");
                                }
                            }
                            else
                            {
                                field = fieldName.Trim() + "=";
                                if (!String.IsNullOrEmpty(fieldType))
                                { fieldType = fieldType.Trim().ToLower(); }
                                if (String.IsNullOrEmpty(fieldType) || fieldType == "text" || fieldType == "number")
                                {
                                    if (!String.IsNullOrEmpty(temp1.GetAsString(keyPrefix + ".user-data.key-name")) && chatEvents.Interaction.InteractionUserData != null)
                                    {
                                        value += chatEvents.Interaction.InteractionUserData.GetAsString(temp1.GetAsString(keyPrefix + ".user-data.key-name"));
                                    }
                                    else if (!String.IsNullOrEmpty(temp1.GetAsString(keyPrefix + ".attrib.key-name")))
                                    {
                                        value += GetAttributeValueForLog(temp1, chatEvents, callType);
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
                        if (field != string.Empty)
                        {
                            ActivityHistory += field + value + "&";
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
        #endregion
    }
}




