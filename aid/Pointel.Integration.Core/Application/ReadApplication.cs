#region System Namespaces

using System;
using System.Text.RegularExpressions;

#endregion System Namespaces

#region Log4Net Namespace



#endregion Log4Net Namespace

#region Genesys Namespaces

using Genesyslab.Platform.Commons.Collections;
using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.CfgObjects;
using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.Queries;
using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel;

#endregion Genesys Namespaces

#region Pointel Namespaces

using Pointel.Integration.Core.Util;
using Pointel.Integration.Core.iSubjects;
using Pointel.Integration.Core.Common;
using System.Collections.Generic;
using Pointel.Configuration.Manager;

#endregion Pointel Namespaces

namespace Pointel.Integration.Core.Application
{
    /// <summary>
    /// This class used to read the application object
    /// </summary>
    internal class ReadApplication
    {
        #region Field Declaration

        private Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
            "AID");
        private Settings setting = Settings.GetInstance();
      //  private CfgPerson _person = null;
        #endregion Field Declaration

        public void ReadApplicationValue(ConfService configProtocol, string applicationName)
        {
            try
            {
                // application = new CfgApplication(.Instance().ComObject);
                //CfgApplicationQuery appQuery = new CfgApplicationQuery();
                //appQuery.Name = applicationName;
                //CfgApplication application = configProtocol.RetrieveObject<CfgApplication>(appQuery);
                //if (application.Options.ContainsKey("agent.ixn.desktop"))
                //{
                //    KeyValueCollection keyValue = (KeyValueCollection)application.Options["agent.ixn.desktop"];
                if (ConfigContainer.Instance().AllKeys.Contains("interaction.case-data.transaction-object"))
                {
                    ReadTransaction(configProtocol, ConfigContainer.Instance().GetAsString("interaction.case-data.transaction-object"));
                }
                if (ConfigContainer.Instance().AllKeys.Contains("interaction.disposition.key-name"))
                {
                    setting.VoiceDispositionKeyName = ConfigContainer.Instance().GetAsString("interaction.disposition.key-name");
                }
                if (ConfigContainer.Instance().AllKeys.Contains("interaction.disposition-collection.key-name"))
                {
                    setting.VoiceDispositionCollectionName = ConfigContainer.Instance().GetAsString("interaction.disposition-collection.key-name");
                }

                //}
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred as " + ex.Message);
            }
        }

        public void ReadTransaction(ConfService configProtocol, string transactionName)
        {
            try
            {
                // application = new CfgApplication(.Instance().ComObject);
                //CfgTransactionQuery trasactionQuery = new CfgTransactionQuery();
                //trasactionQuery.Name = transactionName;
                //CfgTransaction transaction = configProtocol.RetrieveObject<CfgTransaction>(trasactionQuery);
                //if (transaction != null)
                //{
                //    if (transaction.UserProperties.ContainsKey("voice.case-data-filter"))
                //    {
                KeyValueCollection keys = ConfigContainer.Instance().GetValue("VoiceAttachDataFilterKey") as KeyValueCollection;
                //(KeyValueCollection)transaction.UserProperties["voice.case-data-filter"];
                if (keys != null)
                    foreach (object value in keys.Values)
                    {
                        setting.VoiceFilterKey.Add(value.ToString());
                    }
                //    }

                //}

            }
            catch (Exception ex)
            {
                logger.Error("Error occurred as " + ex.Message);
            }
        }


        /// <summary>
        /// Reads the file integration key collections.
        /// </summary>
        /// <param name="configProtocol">The configuration protocol.</param>
        /// <param name="applicationName">Name of the application.</param>
        /// <returns></returns>

        #region ReadFileIntegrationKeyCollections

        public iCallData ReadFileIntegrationKeyCollections(ConfService configProtocol, string applicationName)
        {
            iCallData result = null;
            string value = string.Empty;
            int paramKey = 0;
            try
            {
                result = new CallData();
                result.FileData = new FileIntegration();
                result.PortData = new PortIntegration();
                result.PipeData = new PipeIntegration();
                result.CrmDbData = new CrmDbIntegration();

                CfgApplication application = new CfgApplication(configProtocol);
                CfgApplicationQuery queryApp = new CfgApplicationQuery();
                //queryApp.TenantDbid = WellKnownDbids.EnterpriseModeTenantDbid;
                queryApp.Name = applicationName;
                application = configProtocol.RetrieveObject<CfgApplication>(queryApp);
                if (application != null)
                {
                    string[] applicationKeys = application.Options.AllKeys;

                    string[] applicationUserPropertieskeys = application.UserProperties.AllKeys;

                    foreach (string section in applicationUserPropertieskeys)
                    {
                        if (string.Compare(section, "facet.user.data", true) == 0)
                        {
                            KeyValueCollection kvColl = new KeyValueCollection();
                            kvColl = (KeyValueCollection)application.UserProperties["facet.user.data"];
                            logger.Debug("Retrieving values from facet.user.data section");
                            setting.attachDataList.Clear();
                            if (kvColl != null)
                                for (int i = 1; i <= (kvColl.Count / 2); i++)
                                {
                                    if (kvColl.ContainsKey("facet.userdata.key" + i.ToString()))
                                    {
                                        if (kvColl["facet.userdata.key" + i.ToString()].ToString() != null &&
                                            kvColl["facet.userdata.key" + i.ToString()].ToString() != string.Empty)
                                        {
                                            if (kvColl.ContainsKey("facet.tag.name" + i.ToString()))
                                                if (kvColl["facet.tag.name" + i.ToString()].ToString() != null &&
                                                kvColl["facet.tag.name" + i.ToString()].ToString() != string.Empty)
                                                {
                                                    if (!setting.attachDataList.ContainsKey(kvColl["facet.tag.name" + i.ToString()].ToString()))
                                                    {
                                                        setting.attachDataList.Add(kvColl["facet.tag.name" + i.ToString()].ToString(),
                                                        kvColl["facet.userdata.key" + i.ToString()].ToString());
                                                        logger.Debug("Key : facet.tag.name" + i.ToString() + " Value : " + kvColl["facet.tag.name" + i.ToString()].ToString());
                                                        logger.Debug("Key : facet.userdata.key" + i.ToString() + " Value : " + kvColl["facet.userdata.key" + i.ToString()].ToString());
                                                    }
                                                }
                                        }
                                    }
                                }
                        }
                    }

                    ConfigContainer.Instance().ReadSection("file-integration");
                    if(ConfigContainer.Instance().AllKeys.Contains("file-integration"))
                    {
                        KeyValueCollection _tempcoll = ConfigContainer.Instance().GetValue("file-integration");
                    }
                    foreach (string section in applicationKeys)
                    {
                        if (string.Compare(section, "file-integration", true) == 0 && Settings.GetInstance().EnableFileCommunication)
                        {
                            KeyValueCollection getFileIntegrationCollection = (KeyValueCollection)application.Options[section];
                            //code added by vinoth for bcbs version to show calldata pop up
                            result.FileData.EnableView = true;
                            //End
                            foreach (string fileKey in getFileIntegrationCollection.AllKeys)
                            {
                                Regex re = new Regex(@"\d+");
                                Match m = re.Match(fileKey);
                                if (m.Success)
                                {
                                    paramKey = Convert.ToInt16(m.Value);
                                }
                                if (string.Compare(fileKey, "directory", true) == 0)
                                {
                                    logger.Debug("Key : " + fileKey + " Value : " + getFileIntegrationCollection[fileKey].ToString());
                                    result.FileData.DirectoryPath = getFileIntegrationCollection[fileKey].ToString();
                                }
                                else if (string.Compare(fileKey, "file-name", true) == 0)
                                {
                                    logger.Debug("Key : " + fileKey + " Value : " + getFileIntegrationCollection[fileKey].ToString());
                                    result.FileData.FileName = getFileIntegrationCollection[fileKey].ToString();
                                }
                                else if (string.Compare(fileKey, "file-format", true) == 0)
                                {
                                    logger.Debug("Key : " + fileKey + " Value : " + getFileIntegrationCollection[fileKey].ToString());
                                    result.FileData.FileFormat = getFileIntegrationCollection[fileKey].ToString();
                                }
                                else if (string.Compare(fileKey, "file.string-delimiter", true) == 0)
                                {
                                    logger.Debug("Key : " + fileKey + " Value : " + getFileIntegrationCollection[fileKey].ToString());
                                    result.FileData.Delimiter = getFileIntegrationCollection[fileKey].ToString();
                                }
                                else if (string.Compare(fileKey, "file.event.data-type", true) == 0)
                                {
                                    logger.Debug("Key : " + fileKey + " Value : " + getFileIntegrationCollection[fileKey].ToString());
                                    result.FileData.CallDataEventFileType = getFileIntegrationCollection[fileKey].ToString().Split(new char[] { ',' });
                                }
                                else if (string.Compare(fileKey, "enable.view", true) == 0)
                                {
                                    logger.Debug("Key : " + fileKey + " Value : " + getFileIntegrationCollection[fileKey].ToString());
                                    result.FileData.EnableView = Convert.ToBoolean(getFileIntegrationCollection[fileKey].ToString());
                                }
                                else if (string.Compare(fileKey, "content-type", true) == 0)
                                {
                                    logger.Debug("Key : " + fileKey + " Value : " + getFileIntegrationCollection[fileKey].ToString());
                                    result.FileData.ContentType = getFileIntegrationCollection[fileKey].ToString();
                                }
                                //else if (string.Compare(fileKey, "file" + paramKey + ".attribute", true) == 0)
                                //{
                                //    value += getFileIntegrationCollection[fileKey].ToString() + ",";

                                //}
                                else if (string.Compare(fileKey, "file" + paramKey + ".param", true) == 0)
                                {
                                    try
                                    {
                                        if (getFileIntegrationCollection.ContainsKey("file" + paramKey + ".user-data.key"))
                                            result.FileData.ParameterName.Add(getFileIntegrationCollection[fileKey].ToString()
                                                , Convert.ToString(getFileIntegrationCollection["file" + paramKey + ".user-data.key"]));
                                    }
                                    catch (Exception paramValue)
                                    {
                                        logger.Error("No value configured for given Parameter " + getFileIntegrationCollection[fileKey].ToString() + "  " + paramValue.ToString());
                                    }
                                    // paramKey++;
                                }
                                else if (string.Compare(fileKey, "file" + paramKey + ".attribute", true) == 0)
                                {
                                    try
                                    {
                                        if (getFileIntegrationCollection.ContainsKey("file" + paramKey + ".attribute.param"))
                                            result.FileData.ParameterValue.Add(getFileIntegrationCollection[fileKey].ToString()
                                                , Convert.ToString(getFileIntegrationCollection["file" + paramKey + ".attribute.param"]));
                                    }
                                    catch (Exception paramValue)
                                    {
                                        logger.Error("No value configured for given Parameter " + getFileIntegrationCollection[fileKey].ToString() + "  " + paramValue.ToString());
                                    }
                                    // paramKey++;
                                }
                            }
                            //result.FileData.AttributeFilter= value.ToString().Split(new char[] { ',' }); ;
                            getFileIntegrationCollection.Clear();
                            value = string.Empty;
                        }
                        else if (string.Compare(section, "port-integration", true) == 0 && Settings.GetInstance().EnablePortCommunication)
                        {
                            KeyValueCollection getPortIntegrationCollection = (KeyValueCollection)application.Options[section];
                            // paramKey = 0;
                            foreach (string portKey in getPortIntegrationCollection.AllKeys)
                            {
                                switch (portKey)
                                {
                                    case "ip-address":
                                        setting.PortSetting.HostName = getPortIntegrationCollection[portKey].ToString();
                                        break;
                                    case "receiving.data.port":
                                        setting.PortSetting.IncomingPortNumber = int.Parse(getPortIntegrationCollection[portKey].ToString());
                                        break;
                                    case "sending.data.port":
                                        setting.PortSetting.OutGoingPortNumber = int.Parse(getPortIntegrationCollection[portKey].ToString());
                                        break;
                                    case "port.send.delimiter":
                                        setting.PortSetting.SendDataDelimiter = getPortIntegrationCollection[portKey].ToString();
                                        break;
                                    case "port.receive.delimiter":
                                        setting.PortSetting.ReceiveDataDelimiter = getPortIntegrationCollection[portKey].ToString();
                                        break;
                                    case "port.call-media":
                                        setting.PortSetting.CallMedia = new List<string>(getPortIntegrationCollection[portKey].ToString().Split(','));
                                        break;
                                    case "port.listen.event":
                                        //if (!string.IsNullOrEmpty(getPortIntegrationCollection[portKey].ToString()) && !string.IsNullOrWhiteSpace(getPortIntegrationCollection[portKey].ToString()))
                                        setting.PortSetting.CallDataEventType = new List<string>(getPortIntegrationCollection[portKey].ToString().Split(','));
                                        break;
                                    case "port.receive.key-name":
                                        setting.PortSetting.ReceiveDatakey = new List<string>(getPortIntegrationCollection[portKey].ToString().Split(','));
                                        break;
                                    case "port.receive.connid-key-name":
                                        setting.PortSetting.ReceiveConnectionIdName = getPortIntegrationCollection[portKey].ToString();
                                        break;
                                    case "port.webservicereference-url":
                                        setting.PortSetting.WebServiceURL = getPortIntegrationCollection[portKey].ToString();
                                        break;

                                }
                            }

                            //Code to get list of key names and param name
                            for (int i = 0; true; i++)
                            {
                                string keyName = "port.attribute" + i + ".key-name";
                                string paramName = "port.attribute" + i + ".param-name";
                                if (getPortIntegrationCollection.ContainsKey(keyName) && getPortIntegrationCollection.ContainsKey(paramName))
                                {
                                    setting.PortSetting.SendAttributeKeyName.Add(getPortIntegrationCollection[keyName].ToString());
                                    setting.PortSetting.SendAttributeValue.Add(getPortIntegrationCollection[paramName].ToString());
                                }
                                else
                                    break;
                            }

                            //Code to get list of user data's key names and param name
                            for (int j = 0; true; j++)
                            {
                                string keyName = "port.user-data" + j + ".key-name";
                                string paramName = "port.user-data" + j + ".param-name";
                                if (getPortIntegrationCollection.ContainsKey(keyName) && getPortIntegrationCollection.ContainsKey(paramName))
                                {
                                    setting.PortSetting.SendUserDataName.Add(getPortIntegrationCollection[keyName].ToString());
                                    setting.PortSetting.SendUserDataValue.Add(getPortIntegrationCollection[paramName].ToString());
                                }
                                else
                                    break;
                            }
                            getPortIntegrationCollection.Clear();

                        }
                        else if (string.Compare(section, "pipe-integration", true) == 0 && Settings.GetInstance().EnablePipeCommunication)
                        {
                            //KeyValueCollection getPipeIntegrationCollection = (KeyValueCollection)application.Options[section];
                            ////paramKey = 0;
                            //foreach (string pipeKey in getPipeIntegrationCollection.AllKeys)
                            //{
                            //    Regex re = new Regex(@"\d+");
                            //    Match m = re.Match(pipeKey);
                            //    if (m.Success)
                            //    {
                            //        paramKey = Convert.ToInt16(m.Value);
                            //    }
                            //    if (string.Compare(pipeKey, "pipe.server-first", true) == 0)
                            //    {
                            //        logger.Debug("Key : " + pipeKey + " Value : " + getPipeIntegrationCollection[pipeKey].ToString());
                            //        result.PipeData.PipeFist = Convert.ToBoolean(getPipeIntegrationCollection[pipeKey].ToString().ToLower());
                            //    }
                            //    else if (string.Compare(pipeKey, "pipe.name", true) == 0)
                            //    {
                            //        logger.Debug("Key : " + pipeKey + " Value : " + getPipeIntegrationCollection[pipeKey].ToString());
                            //        result.PipeData.PipeName = getPipeIntegrationCollection[pipeKey].ToString();
                            //    }

                            //    else if (string.Compare(pipeKey, "pipe.string-delimiter", true) == 0)
                            //    {
                            //        logger.Debug("Key : " + pipeKey + " Value : " + getPipeIntegrationCollection[pipeKey].ToString());
                            //        result.PipeData.Delimiter = getPipeIntegrationCollection[pipeKey].ToString();
                            //    }
                            //    else if (string.Compare(pipeKey, "pipe.format", true) == 0)
                            //    {
                            //        logger.Debug("Key : " + pipeKey + " Value : " + getPipeIntegrationCollection[pipeKey].ToString());
                            //        result.PipeData.FileFormat = getPipeIntegrationCollection[pipeKey].ToString();
                            //    }
                            //    else if (string.Compare(pipeKey, "pipe.event.data-type", true) == 0)
                            //    {
                            //        logger.Debug("Key : " + pipeKey + " Value : " + getPipeIntegrationCollection[pipeKey].ToString());
                            //        result.PipeData.CallDataEventPipeType = getPipeIntegrationCollection[pipeKey].ToString().Split(new char[] { ',' });
                            //    }
                            //    //else if (string.Compare(pipeKey, "pipe" + paramKey + ".attribute", true) == 0)
                            //    //{
                            //    //    value += getPipeIntegrationCollection[pipeKey].ToString() + ",";

                            //    //}
                            //    //else if (string.Compare(pipeKey, "pipe" + paramKey + ".attribute", true) == 0)
                            //    //{
                            //    //    value += getPipeIntegrationCollection[pipeKey].ToString() + ",";

                            //    //}
                            //    else if (string.Compare(pipeKey, "pipe" + paramKey + ".param", true) == 0)
                            //    {
                            //        try
                            //        {
                            //            if (getPipeIntegrationCollection.ContainsKey("pipe" + paramKey + ".user-data.key"))
                            //                result.PipeData.ParameterName.Add(getPipeIntegrationCollection[pipeKey].ToString()
                            //                    , Convert.ToString(getPipeIntegrationCollection["pipe" + paramKey + ".user-data.key"]));
                            //        }
                            //        catch (Exception paramValue)
                            //        {
                            //            logger.Error("No value configured for given Parameter " + getPipeIntegrationCollection[pipeKey].ToString() + "  " + paramValue.ToString());
                            //        }
                            //        result.PipeData.ParameterName.Add(pipeKey, getPipeIntegrationCollection[pipeKey].ToString());
                            //        // paramKey++;
                            //    }
                            //    else if (string.Compare(pipeKey, "pipe" + paramKey + ".attribute", true) == 0)
                            //    {
                            //        try
                            //        {
                            //            if (getPipeIntegrationCollection.ContainsKey("pipe" + paramKey + ".attribute.param"))
                            //                result.PipeData.ParameterValue.Add(getPipeIntegrationCollection[pipeKey].ToString()
                            //                    , Convert.ToString(getPipeIntegrationCollection["pipe" + paramKey + ".attribute.param"]));
                            //        }
                            //        catch (Exception paramValue)
                            //        {
                            //            logger.Error("No value configured for given Parameter " + getPipeIntegrationCollection[pipeKey].ToString() + "  " + paramValue.ToString());
                            //        }
                            //        // paramKey++;
                            //    }
                            //}
                            //// result.PipeData.AttributeFilter = value.ToString().Split(new char[] { ',' }); ;
                            //getPipeIntegrationCollection.Clear();
                            //value = string.Empty;
                        }
                        else if (string.Compare(section, "url-integration", true) == 0 && Settings.GetInstance().EnableURLCommunication)
                        {
                            //KeyValueCollection getUrlIntegrationCollection = (KeyValueCollection)application.Options[section];
                            //// paramKey = 0;
                            //foreach (string urlKey in getUrlIntegrationCollection.AllKeys)
                            //{
                            //    Regex re = new Regex(@"\d+");
                            //    Match m = re.Match(urlKey);
                            //    if (m.Success)
                            //    {
                            //        paramKey = Convert.ToInt16(m.Value);
                            //    }

                            //    if (string.Compare(urlKey, "enable.validate.queue-login", true) == 0)
                            //    {
                            //        logger.Debug("Key : " + urlKey + " Value : " + getUrlIntegrationCollection[urlKey].ToString());
                            //        result.URLData.IsValidateQueueLogin = Convert.ToBoolean(getUrlIntegrationCollection[urlKey].ToString());
                            //    }
                            //    else if (string.Compare(urlKey, "browser-type", true) == 0)
                            //    {
                            //        logger.Debug("Key : " + urlKey + " Value : " + getUrlIntegrationCollection[urlKey].ToString());
                            //        result.URLData.BrowserType = getUrlIntegrationCollection[urlKey].ToString();
                            //    }
                            //    else if (string.Compare(urlKey, "popup.url", true) == 0)
                            //    {
                            //        logger.Debug("Key : " + urlKey + " Value : " + getUrlIntegrationCollection[urlKey].ToString());
                            //        string[] popurl = getUrlIntegrationCollection[urlKey].ToString().Split(new char[] { ',' });
                            //        result.URLData.PopUpUrl = popurl[0];
                            //    }
                            //    else if (string.Compare(urlKey, "login.url", true) == 0)
                            //    {
                            //        logger.Debug("Key : " + urlKey + " Value : " + getUrlIntegrationCollection[urlKey].ToString());
                            //        string[] popurl = getUrlIntegrationCollection[urlKey].ToString().Split(new char[] { ',' });
                            //        result.URLData.LoginUrl = popurl[0];
                            //    }
                            //    else if (string.Compare(urlKey, "enable.login-popup-url", true) == 0)
                            //    {
                            //        logger.Debug("Key : " + urlKey + " Value : " + getUrlIntegrationCollection[urlKey].ToString());
                            //        result.URLData.IsLoginUrlEnable = Convert.ToBoolean(getUrlIntegrationCollection[urlKey].ToString());
                            //    }

                            //    else if (string.Compare(urlKey, "enable.web-page-address-bar", true) == 0)
                            //    {
                            //        logger.Debug("Key : " + urlKey + " Value : " + getUrlIntegrationCollection[urlKey].ToString());
                            //        result.URLData.IsBrowserAddress = Convert.ToBoolean(getUrlIntegrationCollection[urlKey].ToString());
                            //    }
                            //    else if (string.Compare(urlKey, "enable.web-page-status-bar", true) == 0)
                            //    {
                            //        logger.Debug("Key : " + urlKey + " Value : " + getUrlIntegrationCollection[urlKey].ToString());
                            //        result.URLData.IsBrowserStatusBar = Convert.ToBoolean(getUrlIntegrationCollection[urlKey].ToString());
                            //    }

                            //    else if (string.Compare(urlKey, "url.qs.delimiter", true) == 0)
                            //    {
                            //        logger.Debug("Key : " + urlKey + " Value : " + getUrlIntegrationCollection[urlKey].ToString());
                            //        result.URLData.Delimiter = getUrlIntegrationCollection[urlKey].ToString();
                            //    }
                            //    else if (string.Compare(urlKey, "url.event.data-type", true) == 0)
                            //    {
                            //        logger.Debug("Key : " + urlKey + " Value : " + getUrlIntegrationCollection[urlKey].ToString());
                            //        result.URLData.CallDataEventUrlType = getUrlIntegrationCollection[urlKey].ToString().Split(new char[] { ',' });
                            //    }
                            //    else if (string.Compare(urlKey, "enable.web-page-inside-aid", true) == 0)
                            //    {
                            //        logger.Debug("Key : " + urlKey + " Value : " + getUrlIntegrationCollection[urlKey].ToString());
                            //        result.URLData.IsWebPageEnabled = Convert.ToBoolean(getUrlIntegrationCollection[urlKey].ToString());
                            //    }
                            //    else if (string.Compare(urlKey, "web-page.name-aid", true) == 0)
                            //    {
                            //        logger.Debug("Key : " + urlKey + " Value : " + getUrlIntegrationCollection[urlKey].ToString());
                            //        result.URLData.WebPageName = getUrlIntegrationCollection[urlKey].ToString();
                            //    }
                            //    else if (string.Compare(urlKey, "control.popup-single-browser", true) == 0)
                            //    {
                            //        logger.Debug("Key : " + urlKey + " Value : " + getUrlIntegrationCollection[urlKey].ToString());
                            //        result.URLData.IsSingleBrowser = Convert.ToBoolean(getUrlIntegrationCollection[urlKey].ToString());
                            //    }
                            //    else if (string.Compare(urlKey, "url" + paramKey + ".param", true) == 0)
                            //    {
                            //        try
                            //        {
                            //            if (getUrlIntegrationCollection.ContainsKey("url" + paramKey + ".user-data.key"))
                            //                result.URLData.ParameterName.Add(getUrlIntegrationCollection[urlKey].ToString()
                            //                    , Convert.ToString(getUrlIntegrationCollection["url" + paramKey + ".user-data.key"]));
                            //        }
                            //        catch (Exception paramValue)
                            //        {
                            //            logger.Error("No value configured for given Paramater " + getUrlIntegrationCollection[urlKey].ToString() + "  " + paramValue.ToString());
                            //        }
                            //        //  paramKey++;
                            //    }
                            //    else if (string.Compare(urlKey, "url" + paramKey + ".attribute", true) == 0)
                            //    {
                            //        try
                            //        {
                            //            if (getUrlIntegrationCollection.ContainsKey("url" + paramKey + ".attribute.param"))
                            //                result.URLData.ParameterValue.Add(getUrlIntegrationCollection[urlKey].ToString()
                            //                    , Convert.ToString(getUrlIntegrationCollection["url" + paramKey + ".attribute.param"]));
                            //        }
                            //        catch (Exception paramValue)
                            //        {
                            //            logger.Error("No value configured for given Paramater " + getUrlIntegrationCollection[urlKey].ToString() + "  " + paramValue.ToString());
                            //        }
                            //        // paramKey++;
                            //    }
                            //}
                            ////result.URLData .AttributeFilter = value.ToString().Split(new char[] { ',' }); ;
                            //getUrlIntegrationCollection.Clear();
                            //value = string.Empty;
                        }
                        else if (string.Compare(section, "db-integration", true) == 0 && Settings.GetInstance().EnableCrmDbCommunication)
                        {
                            KeyValueCollection getCrmIntegrationCollection = (KeyValueCollection)application.Options[section];
                            //paramKey = 0;

                            foreach (string portKey in getCrmIntegrationCollection.AllKeys)
                            {
                                Regex re = new Regex(@"\d+");
                                Match m = re.Match(portKey);
                                if (m.Success)
                                {
                                    paramKey = Convert.ToInt16(m.Value);
                                }
                                if (string.Compare(portKey, "db.sqlliteconnectionstring", true) == 0)
                                {
                                    logger.Debug("Key : " + portKey + " Value : " + getCrmIntegrationCollection[portKey].ToString());
                                    result.CrmDbData.DirectoryPath = getCrmIntegrationCollection[portKey].ToString();
                                }

                                else if (string.Compare(portKey, "db-type", true) == 0)
                                {
                                    logger.Debug("Key : " + portKey + " Value : " + getCrmIntegrationCollection[portKey].ToString());
                                    result.CrmDbData.CrmDbFormat = getCrmIntegrationCollection[portKey].ToString();
                                }
                                else if (string.Compare(portKey, "db.string-delimiter", true) == 0)
                                {
                                    logger.Debug("Key : " + portKey + " Value : " + getCrmIntegrationCollection[portKey].ToString());
                                    result.CrmDbData.Delimiter = getCrmIntegrationCollection[portKey].ToString();
                                }
                                else if (string.Compare(portKey, "db.sqlconnectionstring", true) == 0)
                                {
                                    logger.Debug("Key : " + portKey + " Value : " + getCrmIntegrationCollection[portKey].ToString());
                                    result.CrmDbData.ConnectionSqlPath = getCrmIntegrationCollection[portKey].ToString();
                                }
                                else if (string.Compare(portKey, "db.oracleconnectionstring", true) == 0)
                                {
                                    logger.Debug("Key : " + portKey + " Value : " + getCrmIntegrationCollection[portKey].ToString());
                                    result.CrmDbData.ConnectionOraclePath = getCrmIntegrationCollection[portKey].ToString();
                                }
                                else if (string.Compare(portKey, "db.event.data-type", true) == 0)
                                {
                                    logger.Debug("Key : " + portKey + " Value : " + getCrmIntegrationCollection[portKey].ToString());
                                    result.CrmDbData.CallDataEventDBType = getCrmIntegrationCollection[portKey].ToString().Split(new char[] { ',' });
                                }
                                //else if (string.Compare(portKey, "db" + paramKey + ".attribute", true) == 0)
                                //{
                                //    value += getCrmIntegrationCollection[portKey].ToString() + ",";

                                //}
                                else if (string.Compare(portKey, "db" + paramKey + ".param", true) == 0)
                                {
                                    try
                                    {
                                        if (getCrmIntegrationCollection.ContainsKey("db" + paramKey + ".user-data.key"))
                                            result.CrmDbData.ParameterName.Add(getCrmIntegrationCollection[portKey].ToString()
                                                , Convert.ToString(getCrmIntegrationCollection["db" + paramKey + ".user-data.key"]));
                                    }
                                    catch (Exception paramValue)
                                    {
                                        logger.Error("No value configured for given Parameter " + getCrmIntegrationCollection[portKey].ToString() + "  " + paramValue.ToString());
                                    }
                                    // paramKey++;
                                }
                                else if (string.Compare(portKey, "db" + paramKey + ".attribute", true) == 0)
                                {
                                    try
                                    {
                                        if (getCrmIntegrationCollection.ContainsKey("db" + paramKey + ".attribute.param"))
                                            result.CrmDbData.ParameterValue.Add(getCrmIntegrationCollection[portKey].ToString()
                                                , Convert.ToString(getCrmIntegrationCollection["db" + paramKey + ".attribute.param"]));
                                    }
                                    catch (Exception paramValue)
                                    {
                                        logger.Error("No value configured for given Parameter " + getCrmIntegrationCollection[portKey].ToString() + "  " + paramValue.ToString());
                                    }
                                    // paramKey++;
                                }
                            }
                            //result.CrmDbData.AttributeFilter = value.ToString().Split(new char[] { ',' }); ;
                            getCrmIntegrationCollection.Clear();
                            value = string.Empty;
                        }
                    }
                }
            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred while reading KVP's for File Popup " + generalException.ToString());
            }
            return result;
        }

        #endregion ReadFileIntegrationKeyCollections

        /// <summary>
        /// Reads the integration decision key collections.
        /// </summary>
        /// <param name="configProtocol">The configuration protocol.</param>
        /// <param name="applicationName">Name of the application.</param>
        /// <returns></returns>

        #region ReadIntegrationDecisionKeyCollections

        public void ReadIntegrationDecisionKeyCollections()
        {
            if (ConfigContainer.Instance().AllKeys.Contains("screen.pop.enable.port-integration"))
            {
                Settings.GetInstance().EnablePortCommunication = ConfigContainer.Instance().GetAsBoolean("screen.pop.enable.port-integration");
            }
            if (ConfigContainer.Instance().AllKeys.Contains("screen.pop.enable.file-integration"))
            {
                Settings.GetInstance().EnableFileCommunication = ConfigContainer.Instance().GetAsBoolean("screen.pop.enable.file-integration");
            }

            if (ConfigContainer.Instance().AllKeys.Contains("screen.pop.enable.url-integration"))
            {
                Settings.GetInstance().EnableURLCommunication = ConfigContainer.Instance().GetAsBoolean("screen.pop.enable.url-integration");
            }
            if (ConfigContainer.Instance().AllKeys.Contains("screen.pop.enable.pipe-integration"))
            {
                Settings.GetInstance().EnablePipeCommunication = ConfigContainer.Instance().GetAsBoolean("screen.pop.enable.pipe-integration");
            }
            if (ConfigContainer.Instance().AllKeys.Contains("screen.pop.enable.db-integration"))
            {
                Settings.GetInstance().EnableCrmDbCommunication = ConfigContainer.Instance().GetAsBoolean("screen.pop.enable.db-integration");
            }
            if (ConfigContainer.Instance().AllKeys.Contains("screen.pop.enable.ws.integration"))
            {
                Settings.GetInstance().EnableDualCommunication = ConfigContainer.Instance().GetAsBoolean("screen.pop.enable.ws.integration");
            }
            if (ConfigContainer.Instance().AllKeys.Contains("voice.enable.facet-all-property"))
            {
                Settings.GetInstance().EnablePortCommunication = ConfigContainer.Instance().GetAsBoolean("voice.enable.facet-all-property");
            }
        }

        #endregion ReadIntegrationDecisionKeyCollections


    }
}