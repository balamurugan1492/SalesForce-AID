namespace Pointel.Integration.Core.Helper
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Genesyslab.Platform.Commons.Collections;

    using Pointel.Configuration.Manager;

    public class DataParser : IDisposable
    {
        #region Fields

        private List<KeyValue> keyValue;
        private List<string> listAddedKeyNames;
        private Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
            "AID");

        #endregion Fields

        #region Methods

        public void Dispose()
        {
            logger = null;
            keyValue = null;
            listAddedKeyNames = null;
        }

        public string ParseJson(Type objEventType, object obj, KeyValueCollection userdata, string section)
        {
            string data = string.Empty;
            if (obj != null && objEventType != null)
            {
                if (!string.IsNullOrEmpty(section))
                {
                    if (!ConfigContainer.Instance().AllKeys.Contains(section))
                        ConfigContainer.Instance().ReadSection(section);
                    if (ConfigContainer.Instance().AllKeys.Contains(section)
                        && ConfigContainer.Instance().GetValue(section) != null)
                    {
                        var cfgSection = (KeyValueCollection)ConfigContainer.Instance().GetValue(section);
                        foreach (var key in cfgSection.AllKeys)
                        {
                            string keyName = string.Empty;
                            string parameterName = string.Empty;
                            string value = string.Empty;
                            if (cfgSection[key] != null)
                                keyName = cfgSection[key].ToString();
                            if (key.EndsWith(".key-name"))
                            {
                                parameterName = key.TrimEnd(".key-name".ToCharArray());
                                PropertyInfo property = objEventType.GetProperty(keyName, BindingFlags.Instance | BindingFlags.Public);
                                if (property != null && property.CanRead)
                                {
                                    object propertyValue = property.GetValue(obj, null);
                                    value = propertyValue as string;
                                }
                            }
                            else if (key.EndsWith(".user-data"))
                            {
                                parameterName = key.TrimEnd(".user-data".ToCharArray());
                                if (userdata != null && userdata.ContainsKey(keyName))
                                {
                                    if (userdata[keyName] != null)
                                        value = userdata[keyName] as string;
                                }
                            }

                            if (!string.IsNullOrEmpty(parameterName))
                            {
                                if (string.IsNullOrEmpty(data))
                                    data += "'" + parameterName + "':'" + value + "'";
                                else
                                    data += ",'" + parameterName + "':'" + value + "'";
                            }

                        }
                    }
                }
                else
                    throw new Exception("The section is null");
            }
            data = "{" + data + "}";
            return data;
        }

        public string ParseJsonString(Type objEventType, object obj, KeyValueCollection userdata, KeyValueCollection dataToFilter, string nullValue = null)
        {
            string dataString = ParseTextString(objEventType, obj, userdata, dataToFilter, ",", ":", nullValue);
            if (!string.IsNullOrEmpty(dataString))
                dataString = "{" + dataString + "}";
            return dataString;
        }

        public string ParseTextEclipse(Type objEventType, object obj, KeyValueCollection userdata, string section, string seperator = null, string nullValue = null)
        {
            string data = string.Empty;
            if (obj != null && objEventType != null)
            {
                if (!string.IsNullOrEmpty(section))
                {
                    if (!ConfigContainer.Instance().AllKeys.Contains(section))
                        ConfigContainer.Instance().ReadSection(section);
                    if (ConfigContainer.Instance().AllKeys.Contains(section)
                        && ConfigContainer.Instance().GetValue(section) != null)
                    {
                        var cfgSection = (KeyValueCollection)ConfigContainer.Instance().GetValue(section);
                        foreach (var key in cfgSection.AllKeys)
                        {
                            string keyName = string.Empty;
                            string parameterName = string.Empty;
                            string value = string.Empty;
                            if (cfgSection[key] != null)
                                keyName = cfgSection[key].ToString();
                            if (key.EndsWith(".key-name"))
                            {
                                parameterName = key.Remove(key.Length - 9);
                                PropertyInfo property = objEventType.GetProperty(keyName, BindingFlags.Instance | BindingFlags.Public);
                                if (property != null && property.CanRead)
                                {
                                    object propertyValue = property.GetValue(obj, null);
                                    value = propertyValue as string;
                                }
                            }
                            else if (key.EndsWith(".user-data"))
                            {
                                parameterName = key.Remove(key.Length - 10);
                                if (userdata != null && userdata.ContainsKey(keyName))
                                {
                                    if (userdata[keyName] != null)
                                        value = userdata[keyName] as string;
                                }
                            }
                            if (seperator == null)
                                seperator = "=";
                            if (string.IsNullOrEmpty(value) && nullValue != null)
                                value = nullValue;
                            if (!string.IsNullOrEmpty(parameterName))
                            {
                                data += "<" + parameterName + seperator + value + ">";

                            }
                        }
                    }
                }
                else
                    throw new Exception("The section is null");
            }
            //data = "{" + data + "}";

            return data;
        }

        public string ParseTextString(Type objEventType, object obj, KeyValueCollection userdata, KeyValueCollection dataToFilter, string delimiter, string seperator = null, string nullValue = null, string leftCoverSymbol = "", string rightCoverSymbol = "")
        {
            string dataString = string.Empty;
            if (dataToFilter != null)
            {
                foreach (string keyName in dataToFilter.AllKeys)
                {
                    string parametername = string.Empty;
                    string value = string.Empty;

                    if (keyName.StartsWith("param.user-data.key-name."))
                    {

                        string userDataKey = dataToFilter.GetAsString(keyName);
                        if (userdata != null && userdata.ContainsKey(userDataKey))
                            value = userdata.GetAsString(userDataKey);
                    }
                    else if (keyName.StartsWith("param.attrib.key-name."))
                    {
                        string attributeKey = dataToFilter.GetAsString(keyName);
                        PropertyInfo property = objEventType.GetProperty(attributeKey, BindingFlags.Instance | BindingFlags.Public);
                        if (property != null && property.CanRead)
                        {
                            object propertyValue = property.GetValue(obj, null);
                            value = propertyValue as string;
                        }
                    }
                    else
                        continue;


                    if (string.IsNullOrEmpty(value))
                    {
                        string defaultKeyName = "param.default.value." + keyName.Replace("param.user-data.key-name.", "").Replace("param.attrib.key-name.", "");
                        if (dataToFilter.ContainsKey(defaultKeyName))
                            value = dataToFilter.GetAsString(defaultKeyName);
                    }
                    if (!string.IsNullOrEmpty(keyName))
                    {
                        string defaultKeyName = "param-name." + keyName.Replace("param.user-data.key-name.", "").Replace("param.attrib.key-name.", "");
                        if (dataToFilter.ContainsKey(defaultKeyName))
                            parametername = dataToFilter.GetAsString(defaultKeyName);
                    }
                    if (string.IsNullOrEmpty(value) && nullValue != null)
                        value = nullValue;

                    if (leftCoverSymbol == null || rightCoverSymbol == null)
                        leftCoverSymbol = rightCoverSymbol = "";

                    if (!string.IsNullOrEmpty(parametername))
                    {
                        if (string.IsNullOrEmpty(dataString))
                            dataString = leftCoverSymbol + parametername + seperator + value + rightCoverSymbol;
                        else
                            dataString += delimiter + leftCoverSymbol + parametername + seperator + value + rightCoverSymbol;
                    }

                }
            }
            return dataString;
        }

        public string ParseXML(Type objEventType, object obj, KeyValueCollection userdata, string section)
        {
            string xml = string.Empty;
            try
            {
                if (!ConfigContainer.Instance().AllKeys.Contains(section))
                    ConfigContainer.Instance().ReadSection(section);

                keyValue = new List<KeyValue>();

                // Get Keys and Values from cfg
                if (ConfigContainer.Instance().AllKeys.Contains(section)
                    && ConfigContainer.Instance().GetValue(section) != null)
                {
                    var readCFG = (KeyValueCollection)ConfigContainer.Instance().GetValue(section);
                    foreach (var key in readCFG.AllKeys)
                    {
                        var value = readCFG[key] != null ?
                                         (!string.IsNullOrEmpty(readCFG[key].ToString()) ?
                                                readCFG[key].ToString() :
                                                string.Empty) :
                                         string.Empty;
                        keyValue.Add(new KeyValue { Key = key, Value = value });
                    }
                }

                // Get Keys that need to update
                var keynames = from x in keyValue
                               where x.Key.EndsWith("key-name")
                               select x;

                listAddedKeyNames = new List<string>();
                //bool isUserDataPriority = (!ConfigContainer.Instance().AllKeys.Contains("cfgpriority")
                //    || (ConfigContainer.Instance().AllKeys.Contains("cfgpriority")
                //    && ConfigContainer.Instance().GetAsString("cfgpriority").StartsWith("userdata")));

                bool isUserDataPriority = true;
                if (ConfigContainer.Instance().AllKeys.Contains("cfgpriority") && !(ConfigContainer.Instance().GetAsBoolean("cfgpriority")))
                    isUserDataPriority = false;

                foreach (var keyname in keynames)
                {
                    // Get Exact key name for updating in xml
                    string key = keyname.Key;
                    key = key.Replace("param.child-", "");
                    key = key.Replace(".key-name", "");
                    //key = key.Replace(".attribute-name", "");
                    //key = key.Replace(".default.value", "");

                    string[] keyNames = keyname.Value.Split('|');
                    string[] values = new string[keyNames.Length];

                    for (int i = 0; i < keyNames.Length; i++)//eg:ANI$1000|DNIS$0-5|ConnID
                    {
                        int startIndex = 0;
                        int endIndex = -1;
                        string keyName = string.Empty;
                        string value = string.Empty;
                        if (keyNames[i].Contains("$"))
                        {
                            var _temp = keyNames[i].Split('$');
                            keyName = _temp[0].ToString().Trim();
                            if (_temp[1].Contains("-"))
                            {
                                if (_temp.Length > 1)
                                    _temp = _temp[1].Split('-');
                                int.TryParse(_temp[0].ToString(), out startIndex);
                                //                            startIndex = Convert.ToInt32(_temp[0].ToString().Trim());
                                if (_temp.Length > 1)
                                    int.TryParse(_temp[1].ToString(), out endIndex);
                                //endIndex = Convert.ToInt32(_temp[1].ToString().Trim());
                            }
                            else
                                int.TryParse(_temp[1].ToString(), out startIndex);
                        }
                        // Read value from userdata
                        if (isUserDataPriority && userdata != null && userdata.ContainsKey(keyName))
                        {
                            if (endIndex == -1)
                                value = userdata[keyName].ToString().Substring(startIndex);
                            else
                                value = userdata[keyName].ToString().Substring(startIndex, Convert.ToInt32(endIndex) - startIndex);

                            // Assign Value to array
                            if (!string.IsNullOrEmpty(value))
                                values[i] = value;
                        }
                        else
                        {
                            try
                            {
                                // Get Value
                                PropertyInfo property = objEventType.GetProperty(keyName, BindingFlags.Instance | BindingFlags.Public);
                                if (property != null)
                                {
                                    if (property.CanRead)
                                    {
                                        object propertyValue = property.GetValue(obj, null);
                                        value = propertyValue as string;
                                        if (endIndex > 0)
                                            value = value.Substring(startIndex, Convert.ToInt32(endIndex) - startIndex);
                                    }
                                }
                                // Assign Value to array
                                if (!string.IsNullOrEmpty(value))
                                    values[i] = value;
                            }
                            catch (Exception)
                            {
                                values[i] = string.Empty;
                            }
                        }

                    }
                    string finalValue = string.Empty;
                    foreach (var item in values)
                    {
                        if (!string.IsNullOrEmpty(item))
                            finalValue += item.Trim();
                    }

                    //Update Value to cfg
                    if (!string.IsNullOrEmpty(finalValue))
                    {
                        var getKeyValue = from x in keyValue
                                          where x.Key.Equals("param.child-" + key + ".default.value")
                                          select x;
                        getKeyValue.First().Value = finalValue;
                        listAddedKeyNames.Add(key);
                    }
                }

                var rootKeyValue = from x in keyValue
                                   where x.Key.Contains("param.root-")
                                   select x;

                xml = rootKeyValue.First().Key;
                // Frame Root Element
                xml = "<" + xml.Replace("param.root-", "") + ">" + GetChildElementsfromCFG(string.Empty, rootKeyValue.First().Value.Split(',')) + "</" + xml.Replace("param.root-", "") + ">";
            }
            catch (Exception exception)
            {
                logger.Error("Error While converting cfg to xml" + exception.Message);
                return string.Empty;
            }
            return xml;
        }

        //public string ParseTextString(Type objEventType, object obj, KeyValueCollection userdata, KeyValueCollection dataToFilter, string delimiter, string seperator = null, string nullValue = null)
        //{
        //    string dataString = string.Empty;
        //    if (dataToFilter != null)
        //    {
        //        foreach (string keyName in dataToFilter.AllKeys)
        //        {
        //            string parametername = string.Empty;
        //            string value = string.Empty;

        //            if (keyName.StartsWith("param.user-data.key-name."))
        //            {
        //                parametername = "param-name." + keyName.Replace("param.user-data.key-name.", "");
        //                string userDataKey = dataToFilter.GetAsString(keyName);
        //                if (userdata != null && userdata.ContainsKey(userDataKey))
        //                    value = userdata.GetAsString(userDataKey);
        //            }
        //            else if (keyName.StartsWith("param.attrib.key-name."))
        //            {
        //                parametername = "param-name." + keyName.Replace("param.attrib.key-name.", "");
        //                string attributeKey = dataToFilter.GetAsString(keyName);
        //                PropertyInfo property = objEventType.GetProperty(attributeKey, BindingFlags.Instance | BindingFlags.Public);
        //                if (property != null && property.CanRead)
        //                {
        //                    object propertyValue = property.GetValue(obj, null);
        //                    value = propertyValue as string;
        //                }
        //            }
        //            else
        //                continue;

        //            if (string.IsNullOrEmpty(value))
        //            {
        //                string defaultKeyName = "param.default.value." + keyName.Replace("param.user-data.key-name.", "").Replace("param.attrib.key-name.", "");
        //                if (dataToFilter.ContainsKey(defaultKeyName))
        //                    value = dataToFilter.GetAsString(defaultKeyName);
        //            }
        //            if (string.IsNullOrEmpty(value) && nullValue != null)
        //                value = nullValue;

        //            //if (string.IsNullOrEmpty(parametername))
        //            //{
        //                if (string.IsNullOrEmpty(dataString))
        //                    dataString = parametername + seperator + value;
        //                else
        //                    dataString += delimiter + parametername + seperator + value;
        //            //}
        //        }
        //    }
        //    return dataString;
        //}

        //public string ParseJsonString(Type objEventType, object obj, KeyValueCollection userdata, KeyValueCollection dataToFilter, string nullValue = null)
        //{
        //    string dataString = ParseTextString(objEventType, obj, userdata, dataToFilter, ",", ":", nullValue);
        //    return dataString;
        //}

        private string GetChildElementsfromCFG(string xml, string[] childElements)
        {
            List<string> lstResult = new List<string>();
            foreach (var childElement in childElements)
            {
                if (!string.IsNullOrEmpty(childElement))
                {
                    xml = string.Empty;

                    var childKeyValue = from x in keyValue
                                        where x.Key.Equals("param.child-" + childElement)
                                        select x;
                    if (childKeyValue.Count() > 0)
                    {
                        string child = GetChildElementsfromCFG(xml, childKeyValue.First().Value.Split(','));
                        xml += "<" + childElement + ">" + child + "</" + childElement + ">";
                        keyValue.Remove(childKeyValue.First());
                    }
                    else
                    {
                        childKeyValue = from x in keyValue
                                        where x.Key.Equals("param.child-" + childElement + ".default.value")
                                        select x;
                        if (childKeyValue.Count() > 0)
                        {
                            xml += "<" + childElement + ">" + childKeyValue.First().Value + "</" + childElement + ">";
                        }
                    }
                    lstResult.Add(xml);
                }
            }
            xml = string.Empty;
            foreach (var item in lstResult)
            {
                xml += item.ToString();
            }
            return xml;
        }

        #endregion Methods
    }

    public class KeyValue
    {
        #region Properties

        public string Key
        {
            get;
            set;
        }

        public string Value
        {
            get;
            set;
        }

        #endregion Properties
    }
}