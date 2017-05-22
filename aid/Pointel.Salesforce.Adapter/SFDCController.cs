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

namespace Pointel.Salesforce.Adapter
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;

    using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel;
    using Genesyslab.Platform.Commons.Logging;
    using Genesyslab.Platform.Commons.Protocols;
    using Genesyslab.Platform.Contacts.Protocols;
    using Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events;
    using Genesyslab.Platform.Voice.Protocols;
    using Genesyslab.Platform.Voice.Protocols.TServer.Events;

    using log4net;

    using Pointel.Salesforce.Adapter.Chat;
    using Pointel.Salesforce.Adapter.Configurations;
    using Pointel.Salesforce.Adapter.LogMessage;
    using Pointel.Salesforce.Adapter.SFDCViews;
    using Pointel.Salesforce.Adapter.Utility;
    using Pointel.Salesforce.Adapter.Voice;

    /// <summary>
    /// Comment: Enables iWS/WDE/AID/ThirdParty Application to Connect with SFDC
    /// Last Modified: 25-08-2015
    /// Created by: Pointel Inc
    /// </summary>
    public class SFDCController
    {
        #region Fields

        public static bool IsIEWindowClosed = false;
        public static SHDocVw.WebBrowser SFDCbrowser = null;

        public Process browserProcess = null;

        private SubscribeChatEvents chatPopup = null;
        private Log logger = null;
        private SubscribeVoiceEvents voicePopup = null;

        #endregion Fields

        #region Methods

        /// <summary>
        /// Receives the DispositionCode Changed Event from voice interaction and updates SFDC Records
        /// </summary>
        /// <param name="message"></param>
        public void DispositionCodeChanged(IMessage message)
        {
            if (voicePopup != null)
            {
                voicePopup.UpdateOnDispositionCodeChange(message);
            }
        }

        /// <summary>
        /// Receives the DispositionCode Changed Event from chat/ email interaction and updates SFDC Records
        /// </summary>
        /// <param name="ixnId"></param>
        /// <param name="dispKeyName"></param>
        /// <param name="selectedCode"></param>
        public void DispositionCodeChanged(string ixnId, string dispKeyName, string selectedCode)
        {
            if (chatPopup != null)
            {
                chatPopup.UpdateDispositionCodeChange(ixnId, dispKeyName, selectedCode);
            }
        }

        public void InteactionEvents(IMessage message)
        {
            try
            {
                dynamic popupEvent = null;
                if (voicePopup != null)
                {
                    switch (message.Id)
                    {
                        #region Events

                        case EventRinging.MessageId:

                            if (!Settings.SFDCListener.IsSFDCConnected && Settings.SFDCOptions.Alert_SFDC_Disconnection_OnCall && Settings.SFDCOptions.AlertSFDCConnectionStatus)
                            {
                                Settings.SFDCListener.SFDCConnectionStatus(LogMode.Error, Settings.SFDCOptions.SFDCConnectionFailureMessage);
                            }
                            voicePopup.ReceiveCalls(message);
                            break;

                        case EventEstablished.MessageId:

                            voicePopup.ReceiveCalls(message);
                            break;

                        case EventReleased.MessageId:
                            voicePopup.ReceiveCalls(message);
                            this.logger.Info("***********************************************************");
                            this.logger.Info("SFDCController: EventReleased Interaction Id = " + (message as EventReleased).ConnID.ToString());
                            this.logger.Info("***********************************************************");
                            break;

                        case EventAttachedDataChanged.MessageId:
                            voicePopup.ReceiveCalls(message);
                            break;

                        case EventDialing.MessageId:
                            voicePopup.ReceiveCalls(message);
                            break;

                        case EventPartyChanged.MessageId:
                            voicePopup.ReceiveCalls(message);
                            break;

                        case Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events.EventError.MessageId:
                            popupEvent = (Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events.EventError)message;
                            if (chatPopup != null && popupEvent.Interaction.InteractionMediatype == "chat")
                            {
                                chatPopup.ReceiveChatEvents(message);
                            }
                            break;

                        case EventAbandoned.MessageId:
                            voicePopup.ReceiveCalls(message);
                            break;

                        case EventDestinationBusy.MessageId:
                            voicePopup.ReceiveCalls(message);
                            break;

                        case Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events.EventPartyAdded.MessageId:
                            popupEvent = (Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events.EventPartyAdded)message;
                            if (chatPopup != null && popupEvent.Interaction.InteractionMediatype == "chat")
                            {
                                chatPopup.ReceiveChatEvents(message);
                            }
                            break;

                        case EventPartyRemoved.MessageId:
                            popupEvent = (EventPartyRemoved)message;
                            if (chatPopup != null && popupEvent.Interaction.InteractionMediatype == "chat")
                            {
                                chatPopup.ReceiveChatEvents(message);
                            }
                            break;

                        case EventInvite.MessageId:
                            popupEvent = (EventInvite)message;
                            if (chatPopup != null && popupEvent.Interaction.InteractionMediatype == "chat")
                            {
                                chatPopup.ReceiveChatEvents(message);
                            }
                            break;

                        case Genesyslab.Platform.WebMedia.Protocols.BasicChat.Events.EventSessionInfo.MessageId:
                            if (chatPopup != null)
                            {
                                chatPopup.ReceiveChatEvents(message);
                            }
                            break;

                        default:
                            break;

                        #endregion Events
                    }
                }
            }
            catch (Exception generalException)
            {
                this.logger.Error("InteactionEvents :Error occurred while segregating voice and chat events : " + generalException.ToString());
            }
        }

        public void PopupBrowser()
        {
            if (Settings.SFDCOptions.SFDCPopupContainer != null && Settings.SFDCOptions.SFDCPopupContainer.ToLower().Equals("browser")
                    && !string.IsNullOrWhiteSpace(Settings.SFDCOptions.SFDCPopupBrowserName) && (browserProcess == null || browserProcess.HasExited)
                    && (SFDCController.SFDCbrowser == null || SFDCController.IsIEWindowClosed))
            {
                PopupURL(Settings.SFDCOptions.SFDCLoginURL, Settings.SFDCOptions.SFDCPopupBrowserName, Settings.SFDCOptions.EnableAddressbar, Settings.SFDCOptions.EnableStatusbar);
            }
            else
            {
                this.logger.Info("New Browser Instance is not created because, SFDC browser is already Running......");
            }
        }

        /// <summary>
        /// This method used to Display SFDC in Browser window
        /// </summary>
        /// <param name="URL"></param>
        /// <param name="browserType"></param>
        /// <param name="addressBar"></param>
        /// <param name="statusBar"></param>
        /// <returns></returns>
        public string PopupURL(string URL, string browserType, bool addressBar, bool statusBar)
        {
            try
            {
                this.logger.Info("PopupURL : URL to popup is " + URL);

                if (!String.IsNullOrEmpty(browserType) && browserType.Trim().ToLower().Equals("ie"))
                {
                    this.logger.Info("PopupURL : Popup SFDC URL using Internet Explorer ");
                    this.logger.Info("PopupURL : Display AddressBar : " + addressBar.ToString());
                    this.logger.Info("PopupURL : Display StatusBar : " + statusBar.ToString());
                    SFDCbrowser = new SHDocVw.InternetExplorer() as SHDocVw.WebBrowser;
                    SFDCbrowser.AddressBar = addressBar;
                    SFDCbrowser.StatusBar = statusBar;
                    SFDCbrowser.Navigate(URL);
                    SFDCbrowser.Visible = true;
                    SFDCbrowser.OnQuit += SFDCbrowser_OnQuit;
                    IsIEWindowClosed = false;
                }
                else
                {
                    this.logger.Info("PopupURL : Popup SFDC using Chrome browser ");
                    string PopupUrl = URL;
                    string chromeCommand = string.Empty;
                    string directoryName = string.Empty;
                    if (!String.IsNullOrEmpty(Settings.SFDCOptions.ChromeBrowserCommand))
                    {
                        this.logger.Info("PopupURL : Chrome browser command : " + Settings.SFDCOptions.ChromeBrowserCommand);
                        chromeCommand = Settings.SFDCOptions.ChromeBrowserCommand;
                    }
                    else
                    {
                        this.logger.Info("PopupURL : Chrome browser command is Null or Empty, Adapter will use default Command ");
                        if (Settings.SFDCOptions.SFDCConnectPort != 0)
                        {
                            chromeCommand = "--unsafely-treat-insecure-origin-as-secure=http://localhost:" + Settings.SFDCOptions.SFDCConnectPort + " --test-type --user-data-dir=";
                        }
                        else
                        {
                            chromeCommand = "--unsafely-treat-insecure-origin-as-secure=http://localhost:" + Settings.SFDCOptions.SFDCConnectPort + " --test-type --user-data-dir=";
                            this.logger.Warn("PopupURL : Using default Chrome Command but SFDC Connection Port is null");
                        }
                    }
                    if (!String.IsNullOrEmpty(Settings.SFDCOptions.ChromeBrowserTempDirectory))
                    {
                        if (!Directory.Exists(Settings.SFDCOptions.ChromeBrowserTempDirectory))
                        {
                            this.logger.Info("PopupURL : Chrome User Data directory is not Exists, Directory Name : " + Settings.SFDCOptions.ChromeBrowserTempDirectory);
                            this.logger.Info("PopupURL : Creating directory " + Settings.SFDCOptions.ChromeBrowserTempDirectory);
                            try
                            {
                                Directory.CreateDirectory(Settings.SFDCOptions.ChromeBrowserTempDirectory);
                                directoryName = Settings.SFDCOptions.ChromeBrowserTempDirectory;
                            }
                            catch (Exception generalException)
                            {
                                this.logger.Error("PopupURL : Exception occurred while creating chrome user data temp directory :" + generalException.ToString() + generalException.StackTrace);
                            }
                        }
                        else
                        {
                            this.logger.Info("PopupURL : Chrome User Data directory is already Exists, Directory Name : " + Settings.SFDCOptions.ChromeBrowserTempDirectory);
                            directoryName = Settings.SFDCOptions.ChromeBrowserTempDirectory;
                        }
                    }
                    else
                    {
                        try
                        {
                            if (!Directory.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"\Genesys Telecommunication\InteractionWorkspace\Temp_Chrome")))
                                Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"\Genesys Telecommunication\InteractionWorkspace\Temp_Chrome"));

                            directoryName = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Genesys Telecommunication\InteractionWorkspace\Temp_Chrome";
                        }
                        catch (Exception generalException)
                        {
                            this.logger.Error("PopupURL : Exception occurred while creating chrome user data temp directory :" + generalException.ToString() + generalException.StackTrace);
                        }
                    }

                    if (!String.IsNullOrWhiteSpace(chromeCommand) && !String.IsNullOrWhiteSpace(directoryName) && !String.IsNullOrWhiteSpace(URL))
                    {
                        PopupUrl = chromeCommand + "\"" + directoryName + "\" " + URL;
                        this.logger.Info("PopupURL : Chrome popup using command " + PopupUrl);
                        browserProcess = Process.Start("chrome.exe", PopupUrl);
                        URL = PopupUrl;
                    }
                    else
                    {
                        this.logger.Warn("PopupURL : Chrome Browser not loaded because Chrome command or directory name or Salesforce Login URL is null or empty");
                    }
                }
            }
            catch (Exception generalException)
            {
                try
                {
                    this.logger.Info("PopupURL : Browser Type : " + browserType);
                    this.logger.Error("PopupURL : Error Occurred while popuping SFDC Browser Window, Error Message : " + generalException.Message);
                    if (generalException.Message == "The system cannot find the file specified")
                    {
                        browserProcess = Process.Start(URL);
                    }
                }
                catch (Exception generalException1)
                {
                    return ("PopupURL : Error in opening salesforce url Error message: " + generalException1.Message);
                }
            }
            return URL + "  URL launched Successfully";
        }

        /// <summary>
        /// Stops the HttpListerner from Communicating with SFDC
        /// </summary>
        public void StopSFDCAdapter()
        {
            try
            {
                logger.Info("StopSFDCAdapter : Stopping SFDC Listener....");
                SFDCHttpServer.GetInstance().StopListener();
                if (Settings.SFDCOptions.CanCloseBrowserOnWDEClose)
                {
                    logger.Info("StopSFDCAdapter : Closing SFDC Browser Window....");
                    if (browserProcess != null)
                    {
                        if (!browserProcess.HasExited)
                            browserProcess.CloseMainWindow();
                        else
                            this.logger.Info("StopSFDCAdapter: The SFDC Browser already closed....");
                    }
                    else if (SFDCbrowser != null)
                    {
                        SFDCbrowser.Quit();
                    }
                }
                else
                {
                    logger.Info("Closing SFDC Browser Window while AID close is disabled....");
                }
            }
            catch (Exception generalException)
            {
                this.logger.Error("StopSFDCAdapter: Error occurred " + generalException.ToString());
            }
        }

        /// <summary>
        /// Get Subscriber object
        /// </summary>
        /// <param name="subscirber"></param>
        public void SubscribeSFDCPopup(ISFDCListener subscirber, ILogger _logger, IAgentDetails agentDetails, IConfService confService)
        {
            try
            {
                if (subscirber != null && _logger != null && agentDetails != null && confService != null)
                {
                    this.logger = Log.GenInstance().CreateLogger(subscirber, _logger);
                    InitializeSFDCIntegration(subscirber, agentDetails, confService);
                    logger.Info("*****************************************************************");
                    this.logger.Info("SFDCController: Assembly Name    " + Assembly.GetExecutingAssembly().GetName().Name);
                    this.logger.Info("SFDCController: Assembly Version " + Assembly.GetExecutingAssembly().GetName().Version);
                    logger.Info("*****************************************************************");
                    logger.Info("Subscribe : SFDCController Subscription Success");
                }
                else
                {
                    throw new Exception("SFDC Popup Subscription Failed because Supplied Parameter(s) are null");
                }
            }
            catch (Exception generalException)
            {
                logger.Error("Error: SubscribeSFDCPopup" + generalException);
            }
        }

        /// <summary>
        /// Get Subscriber object
        /// </summary>
        /// <param name="subscirber"></param>
        public void SubscribeSFDCPopup(ISFDCListener subscirber, ILog _logger, IAgentDetails agentDetails, IConfService confService)
        {
            if (subscirber != null && _logger != null && agentDetails != null && confService != null)
            {
                this.logger = Log.GenInstance().CreateLogger(subscirber, _logger);
                InitializeSFDCIntegration(subscirber, agentDetails, confService);
                logger.Info("*****************************************************************");
                this.logger.Info("SFDCController: Assembly Name    " + Assembly.GetExecutingAssembly().GetName().Name);
                this.logger.Info("SFDCController: Assembly Version " + Assembly.GetExecutingAssembly().GetName().Version);
                logger.Info("*****************************************************************");
                logger.Info("Subscribe : SFDCController Subscription Success");
            }
            else
            {
                throw new Exception("SFDC Popup Subscription Failed because Supplied Parameter(s) are null");
            }
        }

        public void SubscribeSFDCPopup(ISFDCListener subscirber, IAgentDetails agentDetails, IConfService confService, bool sendLogsToSubscriber, TServerProtocol tServerProtocol, UniversalContactServerProtocol ucsProtocol)
        {
            if (subscirber != null && agentDetails != null && confService != null && sendLogsToSubscriber)
            {
                this.logger = Log.GenInstance().CreateLogger(subscirber, sendLogsToSubscriber);
                InitializeSFDCIntegration(subscirber, agentDetails, confService);
                logger.Info("*****************************************************************");
                this.logger.Info("SFDCController: Assembly Name    " + Assembly.GetExecutingAssembly().GetName().Name);
                this.logger.Info("SFDCController: Assembly Version " + Assembly.GetExecutingAssembly().GetName().Version);
                logger.Info("*****************************************************************");
                logger.Info("Subscribe : SFDCController Subscription Success");
            }
            else
            {
                throw new Exception("SFDC Popup Subscription Failed because Supplied Parameter(s) are null");
            }
        }

        /// <summary>
        /// Initialize SFDC Adapter
        /// </summary>
        /// <param name="agentDetails">Agent Details</param>
        /// <param name="tServer">T-Server Protocol</param>
        /// <param name="configProtocol">Config Server Protocol</param>
        private void InitializeSFDCIntegration(ISFDCListener subscirber, IAgentDetails agentDetails, IConfService confService)
        {
            try
            {
                logger.Info("InitializeSFDCIntegration : Initializing Common Properties for SFDC Popup");

                Settings.GeneralConfigs = ReadConfiguration.GetInstance().ReadGeneralConfigurations(agentDetails.MyApplication, agentDetails.AgentGroups, agentDetails.Person);

                if (Settings.GeneralConfigs != null)
                {
                    Settings.SFDCOptions = Pointel.Salesforce.Adapter.Configurations.ReadProperties.GetInstance().GetSFDCGeneralProperties(Settings.GeneralConfigs);

                    if (Settings.SFDCOptions != null)
                    {
                        if (Settings.SFDCOptions.EnableSFDCIntegration)
                        {
                            // Initializing Common Properties in Settings
                            Settings.InitializeUtils(subscirber, agentDetails, confService);

                            //Start SFDC Server
                            if (Settings.SFDCOptions.SFDCPopupChannels != null && Settings.SFDCOptions.SFDCConnectPort != 0 && !string.IsNullOrWhiteSpace(Settings.SFDCOptions.HostName))
                            {
                                SFDCHttpServer.GetInstance().StartListener(Environment.CurrentDirectory + @"\Files", Settings.SFDCOptions.SFDCConnectPort, Settings.SFDCOptions.HostName);
                                if (Settings.SFDCOptions.SFDCPopupChannels.ToLower().Contains("voice"))
                                {
                                    voicePopup = new SubscribeVoiceEvents();
                                }
                                if (Settings.SFDCOptions.SFDCPopupChannels.ToLower().Contains("chat"))
                                {
                                    chatPopup = new SubscribeChatEvents();
                                }

                                //PopupBrowser
                                if (Settings.SFDCOptions.SFDCPopupContainer != null)
                                {
                                    if (Settings.SFDCOptions.SFDCPopupContainer.Equals("browser"))
                                    {
                                        if (Settings.SFDCOptions.SFDCLoginURL != null)
                                        {
                                            this.logger.Info("Lauching SFDC URL : " + PopupURL(Settings.SFDCOptions.SFDCLoginURL, Settings.SFDCOptions.SFDCPopupBrowserName,
                                                                           Settings.SFDCOptions.EnableAddressbar, Settings.SFDCOptions.EnableStatusbar));
                                        }
                                        else
                                            this.logger.Info("SFDC Login URL is null. ");
                                    }
                                    else
                                        Settings.SFDCListener.ReceiveSFDCWindow(new SFDCPopupWindow(Settings.SFDCOptions.SFDCLoginURL));
                                }
                                else
                                    this.logger.Info("SFDC Popup Container is null. ");
                                if (!Settings.SFDCListener.IsSFDCConnected && Settings.SFDCOptions.AlertSFDCConnectionStatus)
                                {
                                    Settings.SFDCListener.SFDCConnectionStatus(LogMode.Error, Settings.SFDCOptions.SFDCConnectionFailureMessage);
                                }
                            }
                            else
                            {
                                this.logger.Info("SFDC Popup Aborted, because either popup channel or port number is empty ");
                                this.logger.Info("SFDC Popup Channels : " + Settings.SFDCOptions.SFDCPopupChannels);
                                this.logger.Info("SFDC Port Number : " + Settings.SFDCOptions.SFDCConnectPort);
                            }
                        }
                        else
                        {
                            logger.Info("InitializeSFDCIntegration : SFDC Integration Disabled");
                        }
                    }
                    else
                    {
                        logger.Info("InitializeSFDCIntegration : SFDC General Config object is null...");
                    }
                }
                else
                {
                    logger.Info("InitializeSFDCIntegration : SFDC popup can not be started because SFDC General Configuration has is found.");
                }
            }
            catch (Exception generalException)
            {
                logger.Error("InitializeSFDCIntegration : Error Occurred while start SFDC Popup " + generalException.ToString());
            }
        }

        private void SFDCbrowser_OnQuit()
        {
            IsIEWindowClosed = true;
            SFDCbrowser.OnQuit -= SFDCbrowser_OnQuit;
        }

        #endregion Methods
    }
}