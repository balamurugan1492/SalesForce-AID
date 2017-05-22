using System;
using System.Collections.Generic;
using System.Reflection;
using Genesyslab.Platform.Commons.Collections;
using Genesyslab.Platform.Commons.Protocols;
using Genesyslab.Platform.Voice.Protocols.TServer.Events;
using Pointel.Integration.Core.Util;
namespace Pointel.Integration.Core.Helper
{
    public static class PortServiceHelper
    {
        private static Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
           "AID");

        public static string GetSendCallDetailString(IMessage message)
        {
            string queryString = string.Empty;
            try
            {
                if (message != null)
                {
                    switch (message.Id)
                    {
                        case EventRinging.MessageId:
                            {
                                EventRinging eventRinging = message as EventRinging;
                                queryString = ProcessKeyName(eventRinging.GetType(), eventRinging);
                                queryString += ((queryString.Length > 0) ? Settings.GetInstance().PortSetting.SendDataDelimiter : "") + ProcessUserData(eventRinging.UserData);
                                break;
                            }

                        case EventReleased.MessageId:
                            {
                                EventReleased eventRelease = message as EventReleased;
                                queryString = ProcessKeyName(eventRelease.GetType(), eventRelease);
                                queryString += ((queryString.Length > 0) ? Settings.GetInstance().PortSetting.SendDataDelimiter : "") + ProcessUserData(eventRelease.UserData);
                                break;
                            }

                        case EventEstablished.MessageId:
                            {
                                EventEstablished eventEstablished = message as EventEstablished;
                                queryString = ProcessKeyName(eventEstablished.GetType(), eventEstablished);
                                queryString += ((queryString.Length > 0) ? Settings.GetInstance().PortSetting.SendDataDelimiter : "") + ProcessUserData(eventEstablished.UserData);
                                break;
                            }

                        case EventPartyInfo.MessageId:
                            {
                                EventPartyInfo eventPartyInfo = message as EventPartyInfo;
                                queryString = ProcessKeyName(eventPartyInfo.GetType(), eventPartyInfo);
                                queryString += ((queryString.Length > 0) ? Settings.GetInstance().PortSetting.SendDataDelimiter : "") + ProcessUserData(eventPartyInfo.UserData);
                                break;
                            }


                        case EventHeld.MessageId:
                            {
                                EventHeld eventHeld = message as EventHeld;
                                queryString = ProcessKeyName(eventHeld.GetType(), eventHeld);
                                queryString += ((queryString.Length > 0) ? Settings.GetInstance().PortSetting.SendDataDelimiter : "") + ProcessUserData(eventHeld.UserData);
                                break;
                            }

                        case EventPartyChanged.MessageId:
                            {
                                EventPartyChanged eventPartyChanged = message as EventPartyChanged;
                                queryString = ProcessKeyName(eventPartyChanged.GetType(), eventPartyChanged);
                                queryString += ((queryString.Length > 0) ? Settings.GetInstance().PortSetting.SendDataDelimiter : "") + ProcessUserData(eventPartyChanged.UserData);
                                break;
                            }

                        case EventAttachedDataChanged.MessageId:
                            {
                                EventAttachedDataChanged eventDataChanged = message as EventAttachedDataChanged;
                                queryString = ProcessKeyName(eventDataChanged.GetType(), eventDataChanged);
                                queryString += ((queryString.Length > 0) ? Settings.GetInstance().PortSetting.SendDataDelimiter : "") + ProcessUserData(eventDataChanged.UserData);
                                break;
                            }

                        case EventDialing.MessageId:
                            {
                                EventDialing eventDialing = message as EventDialing;
                                queryString = ProcessKeyName(eventDialing.GetType(), eventDialing);
                                queryString += ((queryString.Length > 0) ? Settings.GetInstance().PortSetting.SendDataDelimiter : "") + ProcessUserData(eventDialing.UserData);
                                break;
                            }
                        case EventRetrieved.MessageId:
                            {
                                EventRetrieved eventRetrieved = message as EventRetrieved;
                                queryString = ProcessKeyName(eventRetrieved.GetType(), eventRetrieved);
                                queryString += ((queryString.Length > 0) ? Settings.GetInstance().PortSetting.SendDataDelimiter : "") + ProcessUserData(eventRetrieved.UserData);
                                break;
                            }

                        case EventAbandoned.MessageId:
                            {
                                EventAbandoned eventAbandoned = message as EventAbandoned;
                                queryString = ProcessKeyName(eventAbandoned.GetType(), eventAbandoned);
                                queryString += ((queryString.Length > 0) ? Settings.GetInstance().PortSetting.SendDataDelimiter : "") + ProcessUserData(eventAbandoned.UserData);
                                break;
                            }

                        case EventPartyAdded.MessageId:
                            {
                                EventPartyAdded eventPartyAdded = message as EventPartyAdded;
                                queryString = ProcessKeyName(eventPartyAdded.GetType(), eventPartyAdded);
                                queryString += ((queryString.Length > 0) ? Settings.GetInstance().PortSetting.SendDataDelimiter : "") + ProcessUserData(eventPartyAdded.UserData);
                                break;
                            }

                        case EventPartyDeleted.MessageId:
                            {
                                EventPartyDeleted eventpartyDeleted = message as EventPartyDeleted;
                                queryString = ProcessKeyName(eventpartyDeleted.GetType(), eventpartyDeleted);
                                queryString += ((queryString.Length > 0) ? Settings.GetInstance().PortSetting.SendDataDelimiter : "") + ProcessUserData(eventpartyDeleted.UserData);
                                break;
                            }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return queryString;
        }

        private static string ProcessUserData(KeyValueCollection userData)
        {
            string queryString = string.Empty;
            int paramIndex = 0;
            if (userData != null)
                foreach (string userDataName in Settings.GetInstance().PortSetting.SendUserDataName)
                {
                    if (userData.ContainsKey(userDataName))
                    {
                        queryString += ((queryString.Length > 0) ? Settings.GetInstance().PortSetting.SendDataDelimiter : "") + Settings.GetInstance().PortSetting.SendUserDataValue[paramIndex] + "=" + userData[userDataName].ToString();
                        paramIndex++;
                    }
                }
            return queryString;
        }

        private static string ProcessKeyName(Type objType, object obj)
        {
            string queryString = string.Empty;
            int paramIndex = 0;
            foreach (string keyName in Settings.GetInstance().PortSetting.SendAttributeKeyName)
            {
                if (keyName.Equals("FirstName") || keyName.Equals("LastName") || keyName.Equals("UserName"))
                {
                    switch (keyName)
                    {
                        case "FirstName":
                            queryString += ((queryString.Length > 0) ? Settings.GetInstance().PortSetting.SendDataDelimiter : "") + Settings.GetInstance().PortSetting.SendAttributeValue[paramIndex] + "=" + Settings.GetInstance().FirstName;
                            break;
                        case "LastName":
                            queryString += ((queryString.Length > 0) ? Settings.GetInstance().PortSetting.SendDataDelimiter : "") + Settings.GetInstance().PortSetting.SendAttributeValue[paramIndex] + "=" + Settings.GetInstance().LastName;
                            break;
                        case "UserName":
                            queryString += ((queryString.Length > 0) ? Settings.GetInstance().PortSetting.SendDataDelimiter : "") + Settings.GetInstance().PortSetting.SendAttributeValue[paramIndex] + "=" + Settings.GetInstance().UserName;
                            break;
                    }
                }
                else
                {
                    PropertyInfo property = objType.GetProperty(keyName, BindingFlags.Instance | BindingFlags.Public);
                    if (property != null)
                    {
                        if (property.CanRead)
                        {
                            object propertyValue = property.GetValue(obj, null);
                            queryString += ((queryString.Length > 0) ? Settings.GetInstance().PortSetting.SendDataDelimiter : "") +
                                Settings.GetInstance().PortSetting.SendAttributeValue[paramIndex] + "=" + propertyValue.ToString();
                        }
                    }
                }
                paramIndex++;
            }
            return queryString;
        }

        public static void InsertCallDetails(object msg)
        {
            IMessage message = msg as IMessage;
            if (message != null)
            {
                switch (message.Id)
                {
                    case EventRinging.MessageId:
                        {
                            EventRinging eventRinging = message as EventRinging;
                            ProcessInsertCallDetails(eventRinging.GetType(), eventRinging);
                            break;
                        }

                    case EventReleased.MessageId:
                        {
                            EventReleased eventRelease = message as EventReleased;
                            ProcessInsertCallDetails(eventRelease.GetType(), eventRelease);
                            break;
                        }

                    case EventEstablished.MessageId:
                        {
                            EventEstablished eventEstablished = message as EventEstablished;
                            ProcessInsertCallDetails(eventEstablished.GetType(), eventEstablished);
                            break;
                        }

                    case EventPartyInfo.MessageId:
                        {
                            EventPartyInfo eventPartyInfo = message as EventPartyInfo;
                            ProcessInsertCallDetails(eventPartyInfo.GetType(), eventPartyInfo);
                            break;
                        }


                    case EventHeld.MessageId:
                        {
                            EventHeld eventHeld = message as EventHeld;
                            ProcessInsertCallDetails(eventHeld.GetType(), eventHeld);
                            break;
                        }

                    case EventPartyChanged.MessageId:
                        {
                            EventPartyChanged eventPartyChanged = message as EventPartyChanged;
                            ProcessInsertCallDetails(eventPartyChanged.GetType(), eventPartyChanged);
                            break;
                        }

                    case EventAttachedDataChanged.MessageId:
                        {
                            EventAttachedDataChanged eventDataChanged = message as EventAttachedDataChanged;
                            ProcessInsertCallDetails(eventDataChanged.GetType(), eventDataChanged);
                            break;
                        }

                    case EventDialing.MessageId:
                        {
                            EventDialing eventDialing = message as EventDialing;
                            ProcessInsertCallDetails(eventDialing.GetType(), eventDialing);
                            break;
                        }
                    case EventRetrieved.MessageId:
                        {
                            EventRetrieved eventRetrieved = message as EventRetrieved;
                            ProcessInsertCallDetails(eventRetrieved.GetType(), eventRetrieved);
                            break;
                        }

                    case EventAbandoned.MessageId:
                        {
                            EventAbandoned eventAbandoned = message as EventAbandoned;
                            ProcessInsertCallDetails(eventAbandoned.GetType(), eventAbandoned);
                            break;
                        }

                    case EventPartyAdded.MessageId:
                        {
                            EventPartyAdded eventPartyAdded = message as EventPartyAdded;
                            ProcessInsertCallDetails(eventPartyAdded.GetType(), eventPartyAdded);
                            break;
                        }

                    case EventPartyDeleted.MessageId:
                        {
                            EventPartyDeleted eventpartyDeleted = message as EventPartyDeleted;
                            ProcessInsertCallDetails(eventpartyDeleted.GetType(), eventpartyDeleted);
                            break;
                        }
                }
            }


            if (message.Id == EventRinging.MessageId)
            {
                // ProcessEventRaising(message as EventRinging);

            }
            else if (message.Id == EventReleased.MessageId)
            {
                //  ProcessEventRelease(message as EventReleased);
                EventReleased eventRelease = message as EventReleased;
                ProcessInsertCallDetails(eventRelease.GetType(), eventRelease);
            }
        }

        public static void ProcessInsertCallDetails(Type objType, object obj)
        {
            try
            {
                Dictionary<string, string> filterKey = new Dictionary<string, string>();
                InsertData calldata = new InsertData();
                calldata.CallDetails = new CallDetails();
                calldata.UserData = new UserData();
                calldata.KeyTable = new KeyTable();
                calldata.CallDetails.FirstName = Settings.GetInstance().FirstName;
                calldata.CallDetails.LastName = Settings.GetInstance().LastName;
                calldata.CallDetails.UserName = Settings.GetInstance().UserName;

                object value = null;
                value = GetValueFromObject(objType, obj, "ConnID");
                if (value != null)
                {
                    calldata.UserData.ConnID = calldata.UserData.ConnID = calldata.CallDetails.ConnID = value.ToString();

                }

                value = GetValueFromObject(objType, obj, "ANI");
                if (value != null)
                    calldata.CallDetails.Ani = value.ToString();

                value = GetValueFromObject(objType, obj, "DNIS");
                if (value != null)
                    calldata.CallDetails.Dnis = value.ToString();

                value = GetValueFromObject(objType, obj, "Name");
                if (value != null)
                    calldata.CallDetails.EventName = value.ToString();

                value = GetValueFromObject(objType, obj, "ThisDN");
                if (value != null)
                    calldata.CallDetails.ThisDN = value.ToString();

                value = GetValueFromObject(objType, obj, "Place");
                if (value != null)
                    calldata.CallDetails.AgentPlace = value.ToString();

                value = GetValueFromObject(objType, obj, "UserData");
                if (value != null)
                {
                    KeyValueCollection userData = value as KeyValueCollection;
                    //Add All Keys and values in user data table.
                    if (userData != null)
                    {
                        //Add Filter keys
                        foreach (string key in Settings.GetInstance().VoiceFilterKey)
                        {
                            if (userData.ContainsKey(key))
                                filterKey.Add(key, userData[key].ToString());
                        }

                        //the keys does not reach, then for add other keys
                        if (filterKey.Count < 10)
                        {
                            foreach (string key in userData.AllKeys)
                            {
                                if (!filterKey.ContainsKey(key) && filterKey.Count < 10)
                                    filterKey.Add(key, userData[key].ToString());
                                else if (filterKey.Count == 10)
                                    break;
                            }
                        }
                        if (userData.ContainsKey(Settings.GetInstance().VoiceDispositionCollectionName))
                        {
                            KeyValueCollection dispositionCollection = userData[Settings.GetInstance().VoiceDispositionCollectionName] as KeyValueCollection;
                            if (dispositionCollection != null)
                            {
                                foreach (string dispositionValue in dispositionCollection.AllValues)
                                {
                                    if (string.IsNullOrEmpty(calldata.CallDetails.Disposition1))
                                        calldata.CallDetails.Disposition1 = dispositionValue;
                                    else if (string.IsNullOrEmpty(calldata.CallDetails.Disposition2))
                                        calldata.CallDetails.Disposition2 = dispositionValue;
                                }

                            }
                        }
                        //For Read Dispotion Key
                        if (userData.ContainsKey(Settings.GetInstance().VoiceDispositionKeyName))
                        {
                            if (string.IsNullOrEmpty(calldata.CallDetails.Disposition1))
                                calldata.CallDetails.Disposition1 = userData[Settings.GetInstance().VoiceDispositionKeyName].ToString();
                            else if (string.IsNullOrEmpty(calldata.CallDetails.Disposition2))
                                calldata.CallDetails.Disposition2 = userData[Settings.GetInstance().VoiceDispositionKeyName].ToString();
                            else if (string.IsNullOrEmpty(calldata.CallDetails.Disposition3))
                                calldata.CallDetails.Disposition3 = userData[Settings.GetInstance().VoiceDispositionKeyName].ToString();
                        }

                        //For adding all keys in user data table
                        calldata.UserData.UserDatas = new ArrayOfKeyValueOfstringstringKeyValueOfstringstring[userData.Count];
                        int index = 0;
                        foreach (string key in userData.AllKeys)
                        {
                            //calldata.UserData.UserDatas[key] = userData[key].ToString();
                            calldata.UserData.UserDatas[index] = new ArrayOfKeyValueOfstringstringKeyValueOfstringstring();
                            calldata.UserData.UserDatas[index].Key = key;
                            calldata.UserData.UserDatas[index].Value = userData[key].ToString();
                            index++;
                        }
                        if (string.IsNullOrEmpty(calldata.CallDetails.AgentPlace))
                            if (userData.ContainsKey("RTargetPlaceSelected"))
                            {
                                calldata.CallDetails.AgentPlace = userData["RTargetPlaceSelected"].ToString();
                            }
                    }

                    int count = 1;
                    //For Store url key
                    Action<string, string> MapValue;
                    MapValue = delegate(string key, string Value)
                    {
                        //if (key.Equals("ReferenceId"))
                        //    return;                  
                        switch (count)
                        {
                            case 1:
                                calldata.CallDetails.Ad1 = Value;
                                calldata.KeyTable.Key1 = key;
                                break;
                            case 2:
                                calldata.CallDetails.Ad2 = Value;
                                calldata.KeyTable.Key2 = key;
                                break;
                            case 3:
                                calldata.CallDetails.Ad3 = Value;
                                calldata.KeyTable.Key3 = key;
                                break;
                            case 4:
                                calldata.CallDetails.Ad4 = Value;
                                calldata.KeyTable.Key4 = key;
                                break;
                            case 5:
                                calldata.CallDetails.Ad5 = Value;
                                calldata.KeyTable.Key5 = key;
                                break;
                            case 6:
                                calldata.CallDetails.Ad6 = Value;
                                calldata.KeyTable.Key6 = key;
                                break;
                            case 7:
                                calldata.CallDetails.Ad7 = Value;
                                calldata.KeyTable.Key7 = key;
                                break;
                            case 8:
                                calldata.CallDetails.Ad8 = Value;
                                calldata.KeyTable.Key8 = key;
                                break;
                            case 9:
                                calldata.CallDetails.Ad9 = Value;
                                calldata.KeyTable.Key9 = key;
                                break;
                            case 10:
                                calldata.CallDetails.Ad10 = Value;
                                calldata.KeyTable.Key10 = key;
                                break;
                        }
                        count++;
                    };

                    foreach (string key in Settings.GetInstance().PortSetting.ReceiveDatakey)
                    {
                        if (count > 10)
                            break;
                        MapValue(key, string.Empty);
                    }

                    foreach (string key in filterKey.Keys)
                    {
                        if (count > 10)
                            break;
                        if (!PortSettings.GetInstance().ReceiveDatakey.Contains(key))
                            MapValue(key, filterKey[key]);
                    }

                }

                BasicService serviceClient = new BasicService(Settings.GetInstance().PortSetting.WebServiceURL);
                ServiceResult serviceResult = serviceClient.InsertCallDetailsAsyc(calldata);
                //logger.Info("Data received from web service while insert call details, Code: " + serviceResult.ResultCode + ", Description: " + serviceResult.ResultDescription);



            }
            catch //(Exception ex)
            {
                //logger.Error("Error occurred as " + ex.Message);
            }
        }

        private static object GetValueFromObject(Type objType, object obj, string propertyName)
        {
            object propertyValue = null;
            PropertyInfo property = objType.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);
            if (property != null)
            {
                if (property.CanRead)
                {
                    propertyValue = property.GetValue(obj, null);

                }
            }
            return propertyValue;
        }

        public static void UpdateCallDetails(string message)
        {
            try
            {

                UpdateData calldata = new UpdateData();
                calldata.CallDetails = new CallDetails();
                foreach (string data in message.Split(PortSettings.GetInstance().ReceiveDataDelimiter.ToCharArray()))
                {
                    string[] keyvalue = data.Split('=');
                    if (keyvalue[0].ToLower().Equals(Settings.GetInstance().PortSetting.ReceiveConnectionIdName))
                    {
                        calldata.CallDetails.ConnID = keyvalue[1];
                    }
                    else if (PortSettings.GetInstance().ReceiveDatakey.Contains(keyvalue[0]))
                    {
                        switch (PortSettings.GetInstance().ReceiveDatakey.IndexOf(keyvalue[0]))
                        {
                            case 0:
                                calldata.CallDetails.Ad1 = keyvalue[1];
                                break;
                            case 1:
                                calldata.CallDetails.Ad2 = keyvalue[1];
                                break;
                            case 2:
                                calldata.CallDetails.Ad3 = keyvalue[1];
                                break;
                            case 3:
                                calldata.CallDetails.Ad4 = keyvalue[1];
                                break;
                            case 4:
                                calldata.CallDetails.Ad5 = keyvalue[1];
                                break;
                            case 5:
                                calldata.CallDetails.Ad6 = keyvalue[1];
                                break;
                            case 6:
                                calldata.CallDetails.Ad7 = keyvalue[1];
                                break;
                            case 7:
                                calldata.CallDetails.Ad8 = keyvalue[1];
                                break;
                            case 8:
                                calldata.CallDetails.Ad9 = keyvalue[1];
                                break;
                            case 9:
                                calldata.CallDetails.Ad10 = keyvalue[1];
                                break;
                        }
                    }
                }

                BasicService serviceClient = new BasicService(Settings.GetInstance().PortSetting.WebServiceURL);
                ServiceResult serviceResult = serviceClient.UpdateCallDetailsAsyc(calldata);
                //logger.Info("Data received from web service while update call details, Code: " + serviceResult.ResultCode + ", Description: " + serviceResult.ResultDescription);

            }
            catch //(Exception ex)
            {
                //logger.Error("Error occurred as " + ex.Message);
            }
        }

    }
}
