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
using Pointel.Salesforce.Adapter.Email;
using Pointel.Salesforce.Adapter.LogMessage;
using Pointel.Salesforce.Adapter.SFDCViews;
using Pointel.Salesforce.Adapter.Utility;
using Pointel.Salesforce.Adapter.Voice;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Pointel.Salesforce.Adapter
{
    /// <summary>
    /// Comment: Enables iWS/WDE/AID/ThirdParty Application to Connect with SFDC Last Modified:
    /// 25-08-2015 Created by: Pointel Inc
    /// </summary>
    public class SFDCController
    {
        #region Field Declaration

        public static bool IsIEWindowClosed = false;
        public static SHDocVw.WebBrowser SFDCbrowser = null;
        public Process browserProcess = null;
        private ChatEventHandler _chatPopup = null;
        private EmailEventHandler _emailPopup = null;
        private ConnectionId _lastReleaseId;
        private Log _logger = null;
        private VoiceEventHandler _voicePopup = null;

        #endregion Field Declaration

        #region Subscribe SFDC Popup

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
                    this._logger = Log.GenInstance().CreateLogger(subscirber, _logger);
                    InitializeSFDCIntegration(subscirber, agentDetails, confService);
                    _logger.Info("*****************************************************************");
                    this._logger.Info("SFDCController: Assembly Name    " + Assembly.GetExecutingAssembly().GetName().Name);
                    this._logger.Info("SFDCController: Assembly Version " + Assembly.GetExecutingAssembly().GetName().Version);
                    _logger.Info("*****************************************************************");
                    _logger.Info("Subscribe : SFDCController Subscription Success");
                }
                else
                {
                    throw new Exception("SFDC Popup Subscription Failed because Supplied Parameter(s) are null");
                }
            }
            catch (Exception generalException)
            {
                _logger.Error("Error: SubscribeSFDCPopup" + generalException);
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
                this._logger = Log.GenInstance().CreateLogger(subscirber, _logger);
                InitializeSFDCIntegration(subscirber, agentDetails, confService);
                _logger.Info("*****************************************************************");
                this._logger.Info("SFDCController: Assembly Name    " + Assembly.GetExecutingAssembly().GetName().Name);
                this._logger.Info("SFDCController: Assembly Version " + Assembly.GetExecutingAssembly().GetName().Version);
                _logger.Info("*****************************************************************");
                _logger.Info("Subscribe : SFDCController Subscription Success");
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
                this._logger = Log.GenInstance().CreateLogger(subscirber, sendLogsToSubscriber);
                InitializeSFDCIntegration(subscirber, agentDetails, confService);
                _logger.Info("*****************************************************************");
                this._logger.Info("SFDCController: Assembly Name    " + Assembly.GetExecutingAssembly().GetName().Name);
                this._logger.Info("SFDCController: Assembly Version " + Assembly.GetExecutingAssembly().GetName().Version);
                _logger.Info("*****************************************************************");
                _logger.Info("Subscribe : SFDCController Subscription Success");
            }
            else
            {
                throw new Exception("SFDC Popup Subscription Failed because Supplied Parameter(s) are null");
            }
        }

        #endregion Subscribe SFDC Popup

        #region Initialize

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
                _logger.Info("InitializeSFDCIntegration : Initializing Common Properties for SFDC Popup");

                Settings.GeneralConfigs = ReadConfiguration.GetInstance().ReadGeneralConfigurations(agentDetails.MyApplication, agentDetails.AgentGroups, agentDetails.Person);

                if (Settings.GeneralConfigs != null)
                {
                    Settings.SFDCOptions = Pointel.Salesforce.Adapter.Configurations.ReadProperties.GetInstance().GetSFDCGeneralProperties(Settings.GeneralConfigs);
                    if (Settings.SFDCOptions != null)
                    {
                        if (Settings.SFDCOptions.EnableSFDCIntegration)
                        {
                            // Initializing Common Properties in Settings
                            Settings.Initialize(subscirber, agentDetails, confService);

                            //Start SFDC Server
                            if (Settings.SFDCOptions.SFDCPopupChannels != null && Settings.SFDCOptions.SFDCConnectPort != 0)
                            {
                                SFDCHttpServer.GetInstance().StartListener(Environment.CurrentDirectory + @"\Files", Settings.SFDCOptions.SFDCConnectPort);
                                if (Settings.IsVoiceEnabled)
                                {
                                    _voicePopup = new VoiceEventHandler();
                                }
                                if (Settings.IsChatEnabled)
                                {
                                    _chatPopup = new ChatEventHandler();
                                }
                                if (Settings.IsEmailEnabled)
                                {
                                    _emailPopup = new EmailEventHandler();
                                }
                                //PopupBrowser
                                if (Settings.SFDCOptions.SFDCPopupContainer != null)
                                {
                                    if (Settings.SFDCOptions.SFDCPopupContainer.Equals("browser"))
                                    {
                                        if (Settings.SFDCOptions.SFDCLoginURL != null)
                                        {
                                            this._logger.Info("Lauching SFDC URL : " + PopupURL(Settings.SFDCOptions.SFDCLoginURL, Settings.SFDCOptions.SFDCPopupBrowserName,
                                                                           Settings.SFDCOptions.EnableAddressbar, Settings.SFDCOptions.EnableStatusbar));
                                        }
                                        else
                                            this._logger.Info("SFDC Login URL is null. ");
                                    }
                                    else
                                        Settings.SFDCListener.ReceiveSFDCWindow(new SFDCPopupWindow(Settings.SFDCOptions.SFDCLoginURL));
                                }
                                else
                                    this._logger.Info("SFDC Popup Container is null. ");
                                if (!Settings.SFDCListener.IsSFDCConnected && Settings.SFDCOptions.AlertSFDCConnectionStatus)
                                {
                                    Settings.SFDCListener.SFDCConnectionStatus(LogMode.Error, Settings.SFDCOptions.SFDCConnectionFailureMessage);
                                }
                            }
                            else
                            {
                                this._logger.Info("SFDC Popup Aborted, because either popup channel or port number is empty ");
                                this._logger.Info("SFDC Popup Channels : " + Settings.SFDCOptions.SFDCPopupChannels);
                                this._logger.Info("SFDC Port Number : " + Settings.SFDCOptions.SFDCConnectPort);
                            }
                        }
                        else
                        {
                            _logger.Info("InitializeSFDCIntegration : SFDC Integration Disabled");
                        }
                    }
                    else
                    {
                        _logger.Info("InitializeSFDCIntegration : SFDC General Config object is null...");
                    }
                }
                else
                {
                    _logger.Info("InitializeSFDCIntegration : SFDC popup can not be started because SFDC General Configuration has is found.");
                }
            }
            catch (Exception generalException)
            {
                _logger.Error("InitializeSFDCIntegration : Error Occurred while start SFDC Popup " + generalException.ToString());
            }
        }

        #endregion Initialize

        #region Popup SFDC Browser

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
                this._logger.Info("PopupURL : URL to popup is " + URL);

                if (!String.IsNullOrEmpty(browserType) && browserType.Trim().ToLower().Equals("ie"))
                {
                    this._logger.Info("PopupURL : Popup SFDC URL using Internet Explorer ");
                    this._logger.Info("PopupURL : Display AddressBar : " + addressBar.ToString());
                    this._logger.Info("PopupURL : Display StatusBar : " + statusBar.ToString());
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
                    this._logger.Info("PopupURL : Popup SFDC using Chrome browser ");
                    string PopupUrl = URL;
                    string chromeCommand = string.Empty;
                    string directoryName = string.Empty;
                    if (!String.IsNullOrEmpty(Settings.SFDCOptions.ChromeBrowserCommand))
                    {
                        this._logger.Info("PopupURL : Chrome browser command : " + Settings.SFDCOptions.ChromeBrowserCommand);
                        chromeCommand = Settings.SFDCOptions.ChromeBrowserCommand;
                    }
                    else
                    {
                        this._logger.Info("PopupURL : Chrome browser command is Null or Empty, Adapter will use default Command ");
                        if (Settings.SFDCOptions.SFDCConnectPort != 0)
                        {
                            chromeCommand = "--unsafely-treat-insecure-origin-as-secure=http://localhost:" + Settings.SFDCOptions.SFDCConnectPort + " --test-type --user-data-dir=";
                        }
                        else
                        {
                            chromeCommand = "--unsafely-treat-insecure-origin-as-secure=http://localhost:" + Settings.SFDCOptions.SFDCConnectPort + " --test-type --user-data-dir=";
                            this._logger.Warn("PopupURL : Using default Chrome Command but SFDC Connection Port is null");
                        }
                    }
                    if (!String.IsNullOrEmpty(Settings.SFDCOptions.ChromeBrowserTempDirectory))
                    {
                        if (!Directory.Exists(Settings.SFDCOptions.ChromeBrowserTempDirectory))
                        {
                            this._logger.Info("PopupURL : Chrome User Data directory is not Exists, Directory Name : " + Settings.SFDCOptions.ChromeBrowserTempDirectory);
                            this._logger.Info("PopupURL : Creating directory " + Settings.SFDCOptions.ChromeBrowserTempDirectory);
                            try
                            {
                                Directory.CreateDirectory(Settings.SFDCOptions.ChromeBrowserTempDirectory);
                                directoryName = Settings.SFDCOptions.ChromeBrowserTempDirectory;
                            }
                            catch (Exception generalException)
                            {
                                this._logger.Error("PopupURL : Exception occurred while creating chrome user data temp directory :" + generalException.ToString() + generalException.StackTrace);
                            }
                        }
                        else
                        {
                            this._logger.Info("PopupURL : Chrome User Data directory is already Exists, Directory Name : " + Settings.SFDCOptions.ChromeBrowserTempDirectory);
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
                            this._logger.Error("PopupURL : Exception occurred while creating chrome user data temp directory :" + generalException.ToString() + generalException.StackTrace);
                        }
                    }

                    if (!String.IsNullOrWhiteSpace(chromeCommand) && !String.IsNullOrWhiteSpace(directoryName) && !String.IsNullOrWhiteSpace(URL))
                    {
                        PopupUrl = chromeCommand + "\"" + directoryName + "\" " + URL;
                        this._logger.Info("PopupURL : Chrome popup using command " + PopupUrl);
                        browserProcess = Process.Start("chrome.exe", PopupUrl);
                        URL = PopupUrl;
                    }
                    else
                    {
                        this._logger.Warn("PopupURL : Chrome Browser not loaded becase Chrome command or directory name or Salesforce Login URL is null or empty");
                    }
                }
            }
            catch (Exception generalException)
            {
                try
                {
                    this._logger.Info("PopupURL : Browser Type : " + browserType);
                    this._logger.Error("PopupURL : Error Occurred while popuping SFDC Browser Window, Error Message : " + generalException.Message);
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

        private void SFDCbrowser_OnQuit()
        {
            IsIEWindowClosed = true;
            SFDCbrowser.OnQuit -= SFDCbrowser_OnQuit;
        }

        #endregion Popup SFDC Browser

        #region Stop SFDC Listener

        /// <summary>
        /// Stops the HttpListerner from Communicating with SFDC
        /// </summary>
        public void StopSFDCAdapter()
        {
            try
            {
                _logger.Info("StopSFDCAdapter : Stopping SFDC Listener....");
                SFDCHttpServer.GetInstance().StopListener();
                if (Settings.SFDCOptions.EnableAutoCloseBrowserOnAppExit)
                {
                    _logger.Info("StopSFDCAdapter : Closing SFDC Browser Window....");
                    if (browserProcess != null)
                    {
                        if (!browserProcess.HasExited)
                            browserProcess.CloseMainWindow();
                        else
                            this._logger.Info("StopSFDCAdapter: The SFDC Browser already closed....");
                    }
                    else if (SFDCbrowser != null)
                    {
                        SFDCbrowser.Quit();
                    }
                }
                else
                {
                    _logger.Info("Closing SFDC Browser Window while WDE close is disabled....");
                }
            }
            catch (Exception generalException)
            {
                this._logger.Error("StopSFDCAdapter: Error occurred " + generalException.ToString());
            }
        }

        #endregion Stop SFDC Listener

        #region Receive iWS Voice/Chat/Email Events

        public void InteactionEvents(IMessage message, TimeSpan? callDuration = null)
        {
            try
            {
                dynamic popupEvent = null;

                switch (message.Id)
                {
                    #region Events

                    case EventRinging.MessageId:

                        if (!Settings.SFDCListener.IsSFDCConnected && Settings.SFDCOptions.Alert_SFDC_Disconnection_OnCall && Settings.SFDCOptions.AlertSFDCConnectionStatus)
                        {
                            Settings.SFDCListener.SFDCConnectionStatus(LogMode.Error, Settings.SFDCOptions.SFDCConnectionFailureMessage);
                        }
                        if (_voicePopup != null)
                            _voicePopup.ReceiveCalls(message, callDuration);
                        break;

                    case EventEstablished.MessageId:
                        if (_voicePopup != null)
                            Task.Run(() => _voicePopup.ReceiveCalls(message, callDuration));
                        break;

                    case EventReleased.MessageId:
                        this._logger.Info("SFDCController: EventReleased ConnectionId = " + (message as EventReleased).ConnID.ToString());
                        if (_lastReleaseId != (message as EventReleased).ConnID)
                        {
                            _lastReleaseId = (message as EventReleased).ConnID;
                            if (_voicePopup != null)
                                Task.Run(() => _voicePopup.ReceiveCalls(message, callDuration));
                        }
                        else
                        {
                            this._logger.Info("SFDCController: Adapter skips EventReleased event because it is received twice for same call : " + (message as EventReleased).ConnID.ToString());
                        }

                        break;

                    case EventDialing.MessageId:
                        if (_voicePopup != null)
                            Task.Run(() => _voicePopup.ReceiveCalls(message, callDuration));
                        break;

                    case EventAttachedDataChanged.MessageId:

                    case EventPartyChanged.MessageId:

                    case EventAbandoned.MessageId:

                    case EventDestinationBusy.MessageId:
                        if (_voicePopup != null)
                            _voicePopup.ReceiveCalls(message, callDuration);
                        break;

                    case EventInvite.MessageId:
                        popupEvent = (EventInvite)message;
                        if (_chatPopup != null && popupEvent.Interaction.InteractionMediatype == "chat")
                        {
                            _chatPopup.ReceiveChatEvents(message);
                        }
                        if (_emailPopup != null && popupEvent.Interaction.InteractionMediatype == "email")
                        {
                            _emailPopup.ReceiveEmailEvents(message);
                        }
                        break;

                    case Genesyslab.Platform.WebMedia.Protocols.BasicChat.Events.EventSessionInfo.MessageId:

                        if (_chatPopup != null)
                            _chatPopup.ReceiveChatEvents(message);
                        break;

                    case Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events.EventError.MessageId:
                        popupEvent = (Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events.EventError)message;
                        if (_chatPopup != null && popupEvent.Interaction.InteractionMediatype == "chat")
                        {
                            _chatPopup.ReceiveChatEvents(message);
                        }
                        break;

                    case Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events.EventPartyAdded.MessageId:
                        popupEvent = (Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events.EventPartyAdded)message;
                        if (_chatPopup != null && popupEvent.Interaction.InteractionMediatype == "chat")
                        {
                            ThreadPool.QueueUserWorkItem(delegate
                               {
                                   _chatPopup.ReceiveChatEvents(message);
                               });
                        }
                        break;

                    case EventPartyRemoved.MessageId:
                        popupEvent = (EventPartyRemoved)message;
                        if (_chatPopup != null && popupEvent.Interaction.InteractionMediatype == "chat")
                        {
                            ThreadPool.QueueUserWorkItem(delegate
                            {
                                _chatPopup.ReceiveChatEvents(message);
                            });
                        }
                        break;

                    case EventPulledInteractions.MessageId:

                        if (_emailPopup != null)
                        {
                            ThreadPool.QueueUserWorkItem(delegate
                            {
                                _emailPopup.ReceiveEmailEvents(message);
                            });
                        }
                        break;

                    case EventAck.MessageId:
                        if (_emailPopup != null)
                        {
                            ThreadPool.QueueUserWorkItem(delegate
                           {
                               _emailPopup.ReceiveEmailEvents(message);
                           });
                        }
                        break;

                    default:
                        break;

                    #endregion Events
                }
            }
            catch (Exception generalException)
            {
                this._logger.Error("InteactionEvents :Error occurred while segregating voice and chat events : " + generalException.ToString());
            }
        }

        #endregion Receive iWS Voice/Chat/Email Events

        #region Email Events

        public void InteractionEventEmail(IXNCustomData ixnData)
        {
            try
            {
                ThreadPool.QueueUserWorkItem(delegate
                {
                    if (ixnData != null)
                    {
                        if (ixnData.MediaType == MediaType.Email && this._emailPopup != null)
                        {
                            this._emailPopup.ReceiveEmailEventsFromCommands(ixnData);
                        }
                        else if (ixnData.MediaType == MediaType.Chat && _chatPopup != null)
                        {
                            this._chatPopup.InteractionEventChat(ixnData);
                        }
                        else if (ixnData.MediaType == MediaType.Voice && _voicePopup != null)
                        {
                            _voicePopup.UpdateDispositionCodeChange(ixnData);
                        }
                    }
                    else
                    {
                        this._logger.Info("EmailData object is null");
                    }
                });
            }
            catch (Exception generalException)
            {
                _logger.Error("InteractionEventEmail:  Error occurred : " + generalException.ToString());
            }
        }

        #endregion Email Events

        #region DispositionCode Changed Event

        /// <summary>
        /// Receives the DispositionCode Changed Event from chat/ email interaction and updates SFDC Records
        /// </summary>
        /// <param name="InteractionData"></param>

        public void DispositionCodeChanged(IXNCustomData dispCode)
        {
            ThreadPool.QueueUserWorkItem(delegate
            {
                if (dispCode.MediaType == MediaType.Voice && _voicePopup != null)
                    _voicePopup.UpdateDispositionCodeChange(dispCode);
                else if (dispCode.MediaType == MediaType.Chat && _chatPopup != null)
                    _chatPopup.UpdateDispositionCodeChange(dispCode);
                else if (dispCode.MediaType == MediaType.Email && _emailPopup != null)
                    _emailPopup.UpdateDispositionCodeChange(dispCode);
            });
        }

        public void DispositionCodeChanged(IMessage dispCode)
        {
            ThreadPool.QueueUserWorkItem(delegate
            {
                if (_voicePopup != null)
                    _voicePopup.UpdateOnDispositionCodeChange(dispCode, null);
            });
        }

        #endregion DispositionCode Changed Event

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
                this._logger.Info("New Browser Instance is not created because, SFDC browser is already Running......");
            }
        }
    }
}