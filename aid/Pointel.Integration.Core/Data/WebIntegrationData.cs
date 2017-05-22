namespace Pointel.Integration.Core.Data
{
    using System;

    using Genesyslab.Platform.Commons.Collections;

    using Pointel.Integration.Core.iSubjects;
    using Pointel.Integration.PlugIn;

    #region Enumerations

    public enum BrowserType
    {
        Chrome,
        Firefox,
        IExplorer,
        AID,
        IEShell,
        IeExternal
    }

    #endregion Enumerations

    /// <summary>
    /// This contain the configuration information to Web intergration.
    /// </summary>
    public class WebIntegrationData
    {
        #region Constructors

        public WebIntegrationData(KeyValueCollection section)
        {
            MediaType = MediaType.Voice;
            AllowDuplicateWindow = true;
            ParseData(section);
        }

        #endregion Constructors

        #region Properties

        public bool AllowDuplicateWindow
        {
            get;
            private set;
        }

        public ApplicationDataDetails ApplicationData
        {
            get;
            set;
        }

        public string ApplicationName
        {
            get;
            private set;
        }

        public BrowserType BrowserType
        {
            get;
            private set;
        }

        public string DataSection
        {
            get;
            private set;
        }

        public KeyValueCollection DataToPost
        {
            get;
            private set;
        }

        public string Delimiter
        {
            get;
            private set;
        }

        public string[] EventType
        {
            get;
            private set;
        }

        public bool IsConditional
        {
            get;
            private set;
        }

        public bool IsEnableAddressBar
        {
            get;
            private set;
        }

        public bool IsEnableIntegration
        {
            get;
            private set;
        }

        public bool IsEnableNewWindowHook
        {
            get;
            private set;
        }

        public bool IsEnableStatusBar
        {
            get;
            private set;
        }

        public bool IsSuppressScript
        {
            get;
            private set;
        }

        public string LoginURL
        {
            get;
            private set;
        }

        public MediaType MediaType
        {
            get;
            private set;
        }

        public string PopupURL
        {
            get;
            private set;
        }

        public string PostType
        {
            get;
            private set;
        }

        public string ValueSeperator
        {
            get;
            private set;
        }

        #endregion Properties

        #region Methods

        private void ParseData(KeyValueCollection section)
        {
            PostType = "querystring";
            BrowserType = BrowserType.AID;
            if (section == null)
                return;

            DataToPost = new KeyValueCollection();
            foreach (string keyName in section.AllKeys)
            {
                switch (keyName)
                {
                    case "application-name":
                        ApplicationName = section.GetAsString("application-name");
                        break;
                    case "param.value.separator":
                        ValueSeperator = section.GetAsString("param.value.separator");
                        break;
                    case "delimiter":
                        Delimiter = section.GetAsString("delimiter");
                        break;
                    case "popup.event":
                        EventType = section.GetAsString("popup.event").Split(',');
                        break;
                    case "login-url":
                        LoginURL = section.GetAsString("login-url");
                        break;
                    case "popup-url":
                        PopupURL = section.GetAsString("popup-url");
                        break;
                    case "enable.integration":
                        if (section.GetAsString("enable.integration").ToLower() == "conditional")
                            IsConditional = true;
                        else
                            IsEnableIntegration = Convert.ToBoolean(section.GetAsString("enable.integration").ToLower());
                        break;
                    case "enable.status-bar":
                        IsEnableStatusBar = Convert.ToBoolean(section.GetAsString("enable.status-bar").ToLower());
                        break;
                    case "enable.address-bar":
                        IsEnableAddressBar = Convert.ToBoolean(section.GetAsString("enable.address-bar").ToLower());
                        break;
                    case "data.post-type":
                        PostType = section.GetAsString("data.post-type");
                        break;
                    case "data-section":
                        DataSection = section.GetAsString("data-section");
                        break;
                    case "enable.hook.new-window":
                        IsEnableNewWindowHook = Convert.ToBoolean(section.GetAsString("enable.hook.new-window").ToLower());
                        break;
                    case "enable.suppress-script":
                        IsSuppressScript = Convert.ToBoolean(section.GetAsString("enable.suppress-script").ToLower());
                        break;
                    case "allow.duplicate-window":
                        AllowDuplicateWindow = Convert.ToBoolean(section.GetAsString("allow.duplicate-window").ToLower());
                        break;
                    case "enable.popup.channel":
                        if (section.GetAsString("enable.popup.channel").ToLower().Equals("email"))
                            MediaType = MediaType.Email;
                        else if (section.GetAsString("enable.popup.channel").ToLower().Equals("chat"))
                            MediaType = MediaType.Chat;
                        else if (section.GetAsString("enable.popup.channel").ToLower().Equals("sms"))
                            MediaType = MediaType.SMS;
                        break;
                    case "browser-type":
                        string browsertype = section.GetAsString("browser-type").ToLower();
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
                    default:
                        DataToPost.Add(keyName, section[keyName]);
                        break;
                }
            }
        }

        #endregion Methods
    }
}