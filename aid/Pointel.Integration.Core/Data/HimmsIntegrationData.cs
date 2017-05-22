namespace Pointel.Integration.Core.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Genesyslab.Platform.Commons.Collections;

    using Pointel.Integration.Core.iSubjects;

    public class HimmsIntegrationData
    {
        #region Constructors

        //enable.single-window-popup
        public HimmsIntegrationData()
        {
            PopupEvent = new List<string>();
            IsEnableSingleWindowPopup = true;
            IsEnableResizeWindow = IsEnableMenuBar = IsEnableStatusBar = IsEnableAddressBar = true;
        }

        #endregion Constructors

        #region Properties

        public string AppLocation
        {
            get;
            set;
        }

        public BrowserType BrowserType
        {
            get;
            private set;
        }

        public string CaseIdPopupUrl
        {
            get;
            set;
        }

        public bool IsEnableAddressBar
        {
            get;
            set;
        }

        public bool IsEnabled
        {
            get;
            set;
        }

        public bool IsEnableFullScreen
        {
            get;
            set;
        }

        public bool IsEnableMenuBar
        {
            get;
            set;
        }

        public bool IsEnablePopupButton
        {
            get;
            set;
        }

        public bool IsEnableResizeWindow
        {
            get;
            set;
        }

        //public string UserName { get; set; }
        //public string Password { get; set; }
        public bool IsEnableSingleWindowPopup
        {
            get;
            set;
        }

        public bool IsEnableStatusBar
        {
            get;
            set;
        }

        public string LoginUrl
        {
            get;
            set;
        }

        //  public bool IsConditional { get; set; }
        public MediaType MediaType
        {
            get;
            private set;
        }

        public string MIDPopupUrl
        {
            get;
            set;
        }

        public List<string> PopupEvent
        {
            get;
            set;
        }

        public int WindowHeight
        {
            get;
            set;
        }

        public int WindowWidth
        {
            get;
            set;
        }

        #endregion Properties

        #region Methods

        public void ParseConfiguration(KeyValueCollection kvConfiguration)
        {
            BrowserType = BrowserType.AID;

            if (kvConfiguration != null)
            {
                foreach (string keyName in kvConfiguration.AllKeys)
                {
                    switch (keyName)
                    {
                        case "enable.integration":
                            string data = kvConfiguration.GetAsString(keyName).ToLower();
                            IsEnabled = data == "true";
                            //   IsConditional = data == "conditional";
                            break;

                        case "media-type":
                            string mediaType = kvConfiguration.GetAsString(keyName).ToLower();
                            if (mediaType == "email")
                                MediaType = MediaType.Email;
                            else if (mediaType == "chat")
                                MediaType = MediaType.Chat;
                            else if (mediaType == "sms")
                                MediaType = MediaType.SMS;
                            break;

                        case "enable.single-window-popup":
                            IsEnableSingleWindowPopup = kvConfiguration.GetAsString(keyName).ToLower() == "true";
                            break;

                        case "browser-type":
                            string browsertype = kvConfiguration.GetAsString(keyName).ToLower();
                            switch (browsertype)
                            {
                                case "ie":
                                    BrowserType = BrowserType.IExplorer;
                                    break;
                                case "chrome":
                                    BrowserType = BrowserType.Chrome;
                                    break;
                                case "firefox":
                                    BrowserType = BrowserType.Firefox;
                                    break;
                                case "aid":
                                    BrowserType = BrowserType.AID;
                                    break;
                                case "ie-shell":
                                    BrowserType = BrowserType.IEShell;
                                    break;
                                case "ie-external":
                                    BrowserType = BrowserType.IeExternal;
                                    break;
                            }
                            break;
                        case "login-url":
                            LoginUrl = kvConfiguration.GetAsString(keyName);
                            break;

                        case "mid-popup-url":
                            MIDPopupUrl = kvConfiguration.GetAsString(keyName);
                            break;

                        case "caseid-popup-url":
                            CaseIdPopupUrl = kvConfiguration.GetAsString(keyName);
                            break;

                        case "enable.popup-button":
                            IsEnablePopupButton = kvConfiguration.GetAsString(keyName).ToLower() == "true";
                            break;

                        case "app-location":
                            AppLocation = kvConfiguration.GetAsString(keyName).ToLower();
                            break;

                        case "popup.event":
                            PopupEvent = kvConfiguration.GetAsString(keyName).ToLower().Split(',').ToList();
                            break;

                        case "popup.window-width":
                            WindowWidth = Convert.ToInt32(kvConfiguration.GetAsString(keyName));
                            break;
                        case "popup.window-height":
                            WindowHeight = Convert.ToInt32(kvConfiguration.GetAsString(keyName));
                            break;
                        case "popup.window.enable.address-bar":
                            IsEnableAddressBar = kvConfiguration.GetAsString(keyName).ToLower() == "true";
                            break;
                        case "popup.window.enable.status-bar":
                            IsEnableStatusBar = kvConfiguration.GetAsString(keyName).ToLower() == "true";
                            break;
                        case "popup.window.enable.full-screen":
                            IsEnableFullScreen = kvConfiguration.GetAsString(keyName).ToLower() == "true";
                            break;
                        case "popup.window.enable.menubar":
                            IsEnableMenuBar = kvConfiguration.GetAsString(keyName).ToLower() == "true";
                            break;
                        case "popup.window.enable.resize":
                            IsEnableResizeWindow = kvConfiguration.GetAsString(keyName).ToLower() == "true";
                            break;

                        //_ieBrowser.FullScreen = false; // Is Need full screen.
                        //_ieBrowser.MenuBar = false;
                        //_ieBrowser.Resizable = true;

                    }
                }
            }
        }

        #endregion Methods
    }
}