namespace Pointel.Integration.Core.Observers
{
    using System;
    using System.Linq;
    using System.Runtime.Serialization.Json;

    using Genesyslab.Platform.Commons.Collections;
    using Genesyslab.Platform.Voice.Protocols.TServer.Events;

    using Pointel.Integration.Core.Application;
    using Pointel.Integration.Core.Data;
    using Pointel.Integration.Core.Helper;
    using Pointel.Integration.Core.iSubjects;
    using Pointel.Integration.Core.Providers;
    using Pointel.Integration.Core.Util;
    using Pointel.Integration.PlugIn;
    using Pointel.WebDriver;

    ﻿using System.Collections.Generic;

    /// <summary>
    /// create class for UrlSubscriber
    /// </summary>
    internal class UrlSubscriber : IObserver<iCallData>, IMIDHandler
    {
        #region Fields

        private BrowserHelper browserHelper;
        private IDisposable cancellation;
        private Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
            "AID");

        //private bool IsFirstAgentLogin = true;
        private WebIntegrationData objURLConfiguration;
        ReadApplication readApplication = new ReadApplication();
        private string result = string.Empty;
        private DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(List<string>));
        private Settings setting = Settings.GetInstance();
        string url = string.Empty;

        #endregion Fields

        #region Constructors

        public UrlSubscriber(WebIntegrationData objConfiguration)
        {
            objURLConfiguration = objConfiguration;
        }

        #endregion Constructors

        #region Methods

        public void OnCompleted()
        {
            Unsubscribe();
        }

        /// <summary>
        /// Notifies the observer that the provider has experienced an error condition.
        /// </summary>
        /// <param name="error">An object that provides additional information about the error.</param>
        public void OnError(Exception error)
        {
            //throw new NotImplementedException();
        }

        /// <summary>
        /// Provides the observer with new data.
        /// </summary>
        /// <param name="value">The current notification information.</param>
        public void OnNext(iCallData value)
        {
            try
            {

                if (value != null)
                {
                    if (objURLConfiguration.ApplicationName.ToLower() == "lawson" && value.EventMessage.Id == EventAgentLogin.MessageId)
                    {
                        PopupLawsonLogin(value);
                        return;
                    }

                    if (objURLConfiguration.MediaType != value.MediaType)
                        return;

                    if (objURLConfiguration.EventType == null
                        || objURLConfiguration.EventType.Where(x => x.ToLower() == value.EventMessage.Name.ToLower()).ToList().Count == 0)
                        return;

                    if (value.MediaType == MediaType.Voice)
                        HandleVoice(value);
                    else if (value.MediaType == MediaType.Email)
                    {

                    }
                    else if (value.MediaType == MediaType.Chat)
                    {

                    }
                }
                else if (objURLConfiguration.IsConditional) // If it is Gvas/Evas/Nvas
                {
                    if (DesktopMessenger.communicateUI != null &&
                        (objURLConfiguration.ApplicationName.ToLower() == "gvas" || objURLConfiguration.ApplicationName.ToLower() == "evas"
                        || objURLConfiguration.ApplicationName.ToLower() == "nvas"))
                    {
                        DesktopMessenger.communicateUI.NotifyWebUrl("about:blank", objURLConfiguration.ApplicationName, DesktopMessenger.totalWebIntegration, objURLConfiguration.IsEnableNewWindowHook, isSurpressScript: objURLConfiguration.IsSuppressScript);
                        DesktopMessenger.communicateUI.PostFormData(objURLConfiguration.ApplicationName, objURLConfiguration.LoginURL,
                           objURLConfiguration.ApplicationData.DataToSent);
                    }

                }
                else if (!objURLConfiguration.ApplicationName.ToLower().Equals("lawson"))
                    PopupLoginPage();
            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred while writing call data to a file " + generalException.ToString());
            }
        }

        public void PopupMID(string mid)
        {
            try
            {

                if (objURLConfiguration.ApplicationName.ToLower() == "evas")
                {
                    if (DesktopMessenger.communicateUI != null)
                        DesktopMessenger.communicateUI.PostDataToEvas(null, objURLConfiguration.ApplicationName, objURLConfiguration.PopupURL);
                    else
                        logger.Warn("The desktop communicator is null.");
                }
                else if (objURLConfiguration.ApplicationName.ToLower() == "nvas")
                {
                    if (DesktopMessenger.communicateUI != null)
                        DesktopMessenger.communicateUI.PostDataToNvas(null, objURLConfiguration.ApplicationName, objURLConfiguration.PopupURL);
                    else
                        logger.Warn("The desktop communicator is null.");
                }
                else if (objURLConfiguration.ApplicationName.ToLower() == "gvas")
                {
                    if (DesktopMessenger.communicateUI != null)
                        DesktopMessenger.communicateUI.PostDataInGVAS(null, objURLConfiguration.ApplicationName, url);
                    else
                        logger.Warn("The desktop communicator is null.");
                }

            }
            catch (Exception exception)
            {
                logger.Error("Error occurred as " + exception.ToString());
            }
        }

        /// <summary>
        /// Subscribes the specified provider.
        /// </summary>
        /// <param name="provider">The provider.</param>
        public virtual void Subscribe(CallDataProviders provider)
        {
            try
            {
                cancellation = provider.Subscribe(this);
            }
            catch (Exception _generalException)
            {
                logger.Error("Error occurred as " + _generalException.Message);
            }
        }

        /// <summary>
        /// Unsubscribe this instance.
        /// </summary>
        public virtual void Unsubscribe()
        {
            //cancellation.Dispose();
        }

        private void HandleVoice(iCallData value)
        {
            try
            {
                Type objType = null;
                object obj = null;
                KeyValueCollection userdata = null;
                MediaEventHelper objEventHelper = new MediaEventHelper();
                if (objEventHelper.ConvertVoiceEvent(ref objType, ref obj, ref userdata, value.EventMessage))
                {

                    if (objType != null && obj != null && objURLConfiguration.PostType != null)
                    {
                        if (objURLConfiguration.PostType.ToLower() == "querystring")
                            SendTextData(objType, obj, userdata, value);
                        else if (objURLConfiguration.PostType.ToLower() == "json")
                            SendJsonData(objType, obj, userdata, value);
                        else if (objURLConfiguration.PostType.ToLower() == "form")
                            SendFormData(objType, obj, userdata, value);
                        else if (objURLConfiguration.PostType.ToLower() == "custom" && objURLConfiguration.ApplicationName.ToLower() == "lawson")
                            PopupLawson(value);
                        //else if (objURLConfiguration.PostType.ToLower() == "custom" && objURLConfiguration.ApplicationName.ToLower() == "gvas")
                        //    DesktopMessenger.communicateUI.PostDataInGVAS(userdata, objURLConfiguration.ApplicationName, objURLConfiguration.PopupURL);
                        else if (objURLConfiguration.PostType.ToLower() == "custom" && objURLConfiguration.ApplicationName.ToLower() == "gvas")
                        {
                            // Implemented changes for popup multiple URL in GVAS -- 17-11-2015 by sakthikumar
                            // : Start

                            if (!string.IsNullOrEmpty(objURLConfiguration.PopupURL) && objURLConfiguration.PopupURL.Contains(","))
                            {
                                string[] urls = objURLConfiguration.PopupURL.Split(',');
                                if (userdata != null && userdata.ContainsKey("AppName") && urls.Where(x => x.StartsWith(userdata.GetAsString("AppName"))).ToList().Count > 0)
                                {
                                    url = urls.Where(x => x.StartsWith(userdata.GetAsString("AppName"))).SingleOrDefault().Replace(userdata.GetAsString("AppName") + "=", "");
                                }
                            }
                            else
                                url = objURLConfiguration.PopupURL;

                            // : End

                            if (!string.IsNullOrEmpty(url))
                            {
                                if (userdata != null && userdata.ContainsKey("AppName")
                                    && (userdata.GetAsString("AppName").ToUpper() == "VA" || userdata.GetAsString("AppName").ToUpper() == "NVAS"))
                                {
                                    DesktopMessenger.midHandler = this;
                                    DesktopMessenger.communicateUI.PostDataInGVAS(userdata, objURLConfiguration.ApplicationName, url);
                                }
                            }

                            else
                                logger.Warn("Popup url is not configured.");
                        }
                        else if (objURLConfiguration.PostType.ToLower() == "custom" && objURLConfiguration.ApplicationName.ToLower() == "evas")
                        {
                            if (userdata != null && userdata.ContainsKey("AppName") && userdata.GetAsString("AppName").ToUpper() == "VA")
                            {
                                if (DesktopMessenger.communicateUI != null)
                                {
                                    DesktopMessenger.midHandler = this;
                                    DesktopMessenger.communicateUI.PostDataToEvas(userdata, objURLConfiguration.ApplicationName, objURLConfiguration.PopupURL);
                                }
                                else
                                    logger.Warn("The desktop communicator is null.");
                            }

                        }
                        else if (objURLConfiguration.PostType.ToLower() == "custom" && objURLConfiguration.ApplicationName.ToLower() == "nvas")
                        {
                            if (userdata != null && userdata.ContainsKey("AppName") && userdata.GetAsString("AppName").ToUpper() == "NVAS")
                            {
                                if (DesktopMessenger.communicateUI != null)
                                {
                                    DesktopMessenger.midHandler = this;
                                    DesktopMessenger.communicateUI.PostDataToNvas(userdata, objURLConfiguration.ApplicationName, objURLConfiguration.PopupURL);
                                }
                                else
                                    logger.Warn("The desktop communicator is null.");
                            }
                        }
                        else if (objURLConfiguration.PostType.ToLower() == "custom")
                            SendCustomData(objType, obj, userdata, value);
                    }
                }
                else
                    logger.Warn("Voice event conversion getting failed");

            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred as " + generalException.Message);
            }
        }

        private void PopupLawson(iCallData value)
        {
            try
            {
                if (value != null && value.EventMessage != null)
                {
                    EventRinging eventRinging = (EventRinging)value.EventMessage;
                    if (eventRinging != null)
                    {
                        if (eventRinging.UserData == null || !eventRinging.UserData.ContainsKey("MemberType") && (eventRinging.UserData["MemberType"].ToString().ToUpper() != "LAWSON"))
                            return;
                        if (eventRinging.CallType.ToString() == "Internal" && Configuration.Manager.ConfigContainer.Instance().AllKeys.Contains("enable.internal-screen-pop") && Configuration.Manager.ConfigContainer.Instance().GetAsBoolean("enable.internal-screen-pop"))
                        {
                            goto Lawson;
                        }
                        else if (eventRinging.CallType.ToString() == "Inbound" && Configuration.Manager.ConfigContainer.Instance().AllKeys.Contains("enable.inbound-screen-pop") && Configuration.Manager.ConfigContainer.Instance().GetAsBoolean("enable.inbound-screen-pop"))
                        {
                            goto Lawson;
                        }
                        else if (eventRinging.CallType.ToString() == "Outbound" && Configuration.Manager.ConfigContainer.Instance().AllKeys.Contains("enable.vhs-screen-pop") && Configuration.Manager.ConfigContainer.Instance().GetAsBoolean("enable.vhs-screen-pop"))
                        {
                            goto Lawson;
                        }
                        else if (eventRinging.CallType.ToString() == "Consult")
                        {
                            if (eventRinging.UserData != null && eventRinging.UserData.ContainsKey("GSW_CAMPAIGN_NAME") && !string.IsNullOrEmpty(eventRinging.UserData["GSW_CAMPAIGN_NAME"].ToString()))
                            {
                                if (Configuration.Manager.ConfigContainer.Instance().AllKeys.Contains("enable.outbound-screen-pop") && Configuration.Manager.ConfigContainer.Instance().GetAsBoolean("enable.outbound-screen-pop"))
                                    goto Lawson;
                            }
                            else if (eventRinging.UserData != null && eventRinging.UserData.ContainsKey("VHT_VIS_DISPOSITION"))
                            {
                                if (eventRinging.UserData["VHT_VIS_DISPOSITION"].ToString().ToLower() == "callback")
                                {
                                    if (Configuration.Manager.ConfigContainer.Instance().AllKeys.Contains("enable.consult-screen-pop") && Configuration.Manager.ConfigContainer.Instance().GetAsBoolean("enable.consult-screen-pop")
                                        && Configuration.Manager.ConfigContainer.Instance().AllKeys.Contains("enable.vhs-screen-pop") && Configuration.Manager.ConfigContainer.Instance().GetAsBoolean("enable.vhs-screen-pop"))
                                        goto Lawson;
                                }
                            }
                            else if (eventRinging.UserData != null && eventRinging.UserData.ContainsKey("VH_CALL_ID"))
                            {
                                if (!string.IsNullOrEmpty(eventRinging.UserData["VH_CALL_ID"].ToString()))
                                {
                                    if (Configuration.Manager.ConfigContainer.Instance().AllKeys.Contains("enable.consult-screen-pop") && Configuration.Manager.ConfigContainer.Instance().GetAsBoolean("enable.consult-screen-pop")
                                           && Configuration.Manager.ConfigContainer.Instance().AllKeys.Contains("enable.vhs-screen-pop") && Configuration.Manager.ConfigContainer.Instance().GetAsBoolean("enable.vhs-screen-pop"))
                                        goto Lawson;
                                }
                            }
                            else if (Configuration.Manager.ConfigContainer.Instance().AllKeys.Contains("enable.consult-screen-pop") && Configuration.Manager.ConfigContainer.Instance().GetAsBoolean("enable.consult-screen-pop")
                                           && Configuration.Manager.ConfigContainer.Instance().AllKeys.Contains("enable.inbound-screen-pop") && Configuration.Manager.ConfigContainer.Instance().GetAsBoolean("enable.inbound-screen-pop"))
                            {
                                if (eventRinging.UserData != null && eventRinging.UserData.ContainsKey("CallTypeKey") && eventRinging.UserData["CallTypeKey"].ToString().ToLower() == "inbound")
                                    goto Lawson;
                            }
                            else if (Configuration.Manager.ConfigContainer.Instance().AllKeys.Contains("enable.consult-screen-pop") && Configuration.Manager.ConfigContainer.Instance().GetAsBoolean("enable.consult-screen-pop")
                                           && Configuration.Manager.ConfigContainer.Instance().AllKeys.Contains("enable.internal-screen-pop") && Configuration.Manager.ConfigContainer.Instance().GetAsBoolean("enable.internal-screen-pop"))
                            {
                                if (eventRinging.UserData != null && eventRinging.UserData.ContainsKey("CallTypeKey") && eventRinging.UserData["CallTypeKey"].ToString().ToLower() == "internal")
                                    goto Lawson;
                            }
                        }

                    Lawson:
                        string queryString = string.Empty;

                        if (eventRinging.UserData != null && eventRinging.UserData.ContainsKey("EmployeeID"))
                        {
                            if (eventRinging.UserData["EmployeeID"] != null)
                            {
                                queryString = eventRinging.UserData["EmployeeID"].ToString();
                            }
                        }
                        if (!string.IsNullOrEmpty(objURLConfiguration.PopupURL))
                        {
                            string urlString = string.Empty;
                            if (!string.IsNullOrEmpty(queryString))
                                urlString = urlString + "?" + queryString;
                            if (objURLConfiguration.BrowserType == Pointel.Integration.Core.Data.BrowserType.AID)
                            {
                                logger.Info("Browser enabled inside the application to the web integration '" + objURLConfiguration.ApplicationName + "'");
                                DesktopMessenger.communicateUI.NotifyWebUrl(urlString, objURLConfiguration.ApplicationName, DesktopMessenger.totalWebIntegration, objURLConfiguration.IsEnableNewWindowHook, objURLConfiguration.AllowDuplicateWindow, objURLConfiguration.IsSuppressScript, value.EventMessage);
                            }
                            else
                            {

                            }

                        }
                        else
                            logger.Warn("The popup URL is null");
                    }
                    else
                        logger.Warn("Event is null");
                }
                else
                    logger.Warn("Event is null");
            }
            catch (Exception _generalException)
            {
                logger.Error("Error occurred as " + _generalException.Message);
            }
        }

        private void PopupLawsonLogin(iCallData value)
        {
            try
            {
                if (!string.IsNullOrEmpty(objURLConfiguration.LoginURL))
                {
                    if (objURLConfiguration.BrowserType == Pointel.Integration.Core.Data.BrowserType.AID)
                    {
                        DesktopMessenger.communicateUI.NotifyWebUrl(objURLConfiguration.LoginURL, objURLConfiguration.ApplicationName, DesktopMessenger.totalWebIntegration, objURLConfiguration.IsEnableNewWindowHook, objURLConfiguration.AllowDuplicateWindow, objURLConfiguration.IsSuppressScript, null);
                    }
                    else
                    {

                    }
                }
                else
                    logger.Warn("Lawson login url not configured.");

            }
            catch (Exception _generalException)
            {
                logger.Error("Error occurred as " + _generalException.Message);
            }
        }

        private void PopupLoginPage()
        {
            try
            {
                if (string.IsNullOrEmpty(objURLConfiguration.LoginURL))
                {
                    logger.Warn("Login page is null or empty");
                    return;
                }

                if (objURLConfiguration.BrowserType == Pointel.Integration.Core.Data.BrowserType.AID)
                {
                    logger.Info("Browser enabled inside the application to the web integration '" + objURLConfiguration.ApplicationName + "'");
                    DesktopMessenger.communicateUI.NotifyWebUrl(objURLConfiguration.LoginURL, objURLConfiguration.ApplicationName, DesktopMessenger.totalWebIntegration, objURLConfiguration.IsEnableNewWindowHook, objURLConfiguration.AllowDuplicateWindow, objURLConfiguration.IsSuppressScript);

                }
                else
                {
                    // Code to handle External browser.
                    browserHelper = new BrowserHelper();
                    BrowserResult result = browserHelper.Navigate((Pointel.WebDriver.BrowserType)((int)objURLConfiguration.BrowserType), objURLConfiguration.LoginURL);
                    // System.Windows.MessageBox.Show(objURLConfiguration.LoginURL);
                    if (!result.IsSuccess)
                        logger.Warn("External browser getting error while callin,'" + result.Message + "'");
                }
            }
            catch (Exception _generalException)
            {
                logger.Error("Error occurred as " + _generalException.Message);
            }
        }

        private void SendCustomData(Type objType, object obj, KeyValueCollection userData, iCallData value)
        {
            try
            {
                if (objURLConfiguration.ApplicationName.ToLower() == "lawson")
                {
                    PopupLawson(value);
                    return;
                }
                DataParser objDataParser = new DataParser();
                string data = objDataParser.ParseTextString(objType, obj, userData, objURLConfiguration.DataToPost, objURLConfiguration.Delimiter, objURLConfiguration.ValueSeperator);
                if (objURLConfiguration.BrowserType == Pointel.Integration.Core.Data.BrowserType.AID)
                {
                    logger.Info("Browser enabled inside the application to the web integration '" + objURLConfiguration.ApplicationName + "'");
                    DesktopMessenger.communicateUI.NotifyWebUrl(objURLConfiguration.PopupURL + "?" + data, objURLConfiguration.ApplicationName, DesktopMessenger.totalWebIntegration, objURLConfiguration.IsEnableNewWindowHook, objURLConfiguration.AllowDuplicateWindow, objURLConfiguration.IsSuppressScript, null);
                }
                else
                {
                    // Code to handle External browser.
                    browserHelper = new BrowserHelper();
                    BrowserResult result = browserHelper.Navigate((Pointel.WebDriver.BrowserType)((int)objURLConfiguration.BrowserType), objURLConfiguration.PopupURL + "?" + data);
                    // System.Windows.MessageBox.Show(objURLConfiguration.PopupURL + "?" + data);
                    if (!result.IsSuccess)
                        logger.Warn("External browser getting error while callin,'" + result.Message + "'");
                }
            }
            catch (Exception exception)
            {

                logger.Error("Error occurred as " + exception.ToString());
            }
        }

        private void SendFormData(Type objType, object obj, KeyValueCollection userData, iCallData value)
        {
            try
            {
                DataParser objDataParser = new DataParser();
                string data = objDataParser.ParseTextString(objType, obj, userData, objURLConfiguration.DataToPost, objURLConfiguration.Delimiter, objURLConfiguration.ValueSeperator);
                if (objURLConfiguration.BrowserType == Pointel.Integration.Core.Data.BrowserType.AID)
                {
                    logger.Info("Browser enabled inside the application to the web integration '" + objURLConfiguration.ApplicationName + "'");
                    DesktopMessenger.communicateUI.NotifyWebUrl(objURLConfiguration.PopupURL + "?" + data, objURLConfiguration.ApplicationName, DesktopMessenger.totalWebIntegration, objURLConfiguration.IsEnableNewWindowHook, objURLConfiguration.AllowDuplicateWindow, objURLConfiguration.IsSuppressScript, null);
                }
                else
                {
                    // Code to handle External browser.
                    browserHelper = new BrowserHelper();
                    BrowserResult result = browserHelper.Navigate((Pointel.WebDriver.BrowserType)((int)objURLConfiguration.BrowserType), objURLConfiguration.PopupURL + "?" + data);
                    //System.Windows.MessageBox.Show(objURLConfiguration.PopupURL + "?" + data);
                    if (!result.IsSuccess)
                        logger.Warn("External browser getting error while callin,'" + result.Message + "'");
                }
            }
            catch (Exception exception)
            {

                logger.Error("Error occurred as " + exception.ToString());
            }
        }

        private void SendJsonData(Type objType, object obj, KeyValueCollection userData, iCallData value)
        {
            try
            {
                DataParser objDataParser = new DataParser();
                string data = objDataParser.ParseJsonString(objType, obj, userData, objURLConfiguration.DataToPost);
                if (objURLConfiguration.BrowserType == Pointel.Integration.Core.Data.BrowserType.AID)
                {
                    logger.Info("Browser enabled inside the application to the web integration '" + objURLConfiguration.ApplicationName + "'");
                    DesktopMessenger.communicateUI.NotifyWebUrl(objURLConfiguration.PopupURL + "?" + data, objURLConfiguration.ApplicationName, DesktopMessenger.totalWebIntegration, objURLConfiguration.IsEnableNewWindowHook, objURLConfiguration.AllowDuplicateWindow, objURLConfiguration.IsSuppressScript, null);
                }
                else
                {
                    // Code to handle External browser.
                    browserHelper = new BrowserHelper();
                    BrowserResult result = browserHelper.Navigate((Pointel.WebDriver.BrowserType)((int)objURLConfiguration.BrowserType), objURLConfiguration.PopupURL + "?" + data);
                    //System.Windows.MessageBox.Show(objURLConfiguration.PopupURL + "?" + data);
                    if (!result.IsSuccess)
                        logger.Warn("External browser getting error while callin,'" + result.Message + "'");
                }
            }
            catch (Exception exception)
            {

                logger.Error("Error occurred as " + exception.ToString());
            }
        }

        private void SendTextData(Type objType, object obj, KeyValueCollection userData, iCallData value)
        {
            try
            {
                DataParser objDataParser = new DataParser();
                string data = objDataParser.ParseTextString(objType, obj, userData, objURLConfiguration.DataToPost, objURLConfiguration.Delimiter, objURLConfiguration.ValueSeperator);
                if (objURLConfiguration.BrowserType == Pointel.Integration.Core.Data.BrowserType.AID)
                {
                    logger.Info("Browser enabled inside the application to the web integration '" + objURLConfiguration.ApplicationName + "'");
                    DesktopMessenger.communicateUI.NotifyWebUrl(objURLConfiguration.PopupURL + "?" + data, objURLConfiguration.ApplicationName, DesktopMessenger.totalWebIntegration, objURLConfiguration.IsEnableNewWindowHook, objURLConfiguration.AllowDuplicateWindow, objURLConfiguration.IsSuppressScript, null);
                }
                else
                {
                    // Code to handle External browser.
                    browserHelper = new BrowserHelper();
                    BrowserResult result = browserHelper.Navigate((Pointel.WebDriver.BrowserType)((int)objURLConfiguration.BrowserType), objURLConfiguration.PopupURL + "?" + data);
                    // System.Windows.MessageBox.Show(objURLConfiguration.PopupURL + "?" + data);
                    if (!result.IsSuccess)
                        logger.Warn("External browser getting error while callin,'" + result.Message + "'");
                }
            }
            catch (Exception exception)
            {

                logger.Error("Error occurred as " + exception.ToString());
            }
        }

        #endregion Methods
    }
}