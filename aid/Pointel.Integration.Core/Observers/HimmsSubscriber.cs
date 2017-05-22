namespace Pointel.Integration.Core.Observers
{
    using System;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Threading;

    using Genesyslab.Platform.Commons.Collections;

    using mshtml;

    using Pointel.Integration.Core.Data;
    using Pointel.Integration.Core.Helper;
    using Pointel.Integration.Core.iSubjects;
    using Pointel.Integration.Core.Providers;
    using Pointel.WebDriver;

    using SHDocVw;

    class HimmsSubscriber : IObserver<iCallData>, IMIDHandler
    {
        #region Fields

        private int ieProcessID;
        private bool isCaseIDOpened;
        private bool isCaseURLOpened;
        private bool isMIDOpened;
        private BrowserHelper _browserHelper;
        private BrowserHelper _browserHelperCaseData;
        private string _caseId;
        private HimmsIntegrationData _himmsConfiguration;

        //private WebBrowser _midBrowser;
        //private WebBrowser _caseIDBrowser;
        //private WebBrowser _caseURLBrowser;
        private Pointel.Logger.Core.ILog _logger;
        private WebBrowser _loginBrowser;

        #endregion Fields

        #region Constructors

        public HimmsSubscriber(HimmsIntegrationData objHimmsIntegrationData)
        {
            _logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
               "AID");
            _himmsConfiguration = objHimmsIntegrationData;
        }

        #endregion Constructors

        #region Methods

        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
        }

        public void OnNext(iCallData value)
        {
            Thread objThread = new Thread(() =>
            {

                //if (DesktopMessenger.communicateUI != null)
                //    DesktopMessenger.communicateUI.NotifyMIDState(_himmsConfiguration.IsEnablePopupButton);

                //  DesktopMessenger.communicateUI.NotifyMIDState(false);
                if (value == null)
                {
                    _logger.Warn("The event data is nul.");
                    return;
                }

                if (value.MediaType != _himmsConfiguration.MediaType)
                {
                    _logger.Info("The media type is not matching to HIMMS integration");
                    return;
                }

                if (!_himmsConfiguration.PopupEvent.Contains(value.EventMessage.Name.ToLower()))
                    return;

                if (value.MediaType == MediaType.Voice)
                {
                    //DesktopMessenger.communicateUI.NotifyMIDState(false,0);
                    HandleVoice(value);
                }
                else if (value.MediaType == MediaType.Email)
                {

                }
                else if (value.MediaType == MediaType.Chat)
                {

                }

            });

            objThread.Start();
        }

        public void PopupMID(string mid)
        {
            try
            {
                if (!string.IsNullOrEmpty(_caseId) && _caseId != "invalid" && _caseId != "unknown")
                    Navigate(_himmsConfiguration.CaseIdPopupUrl + "?extcaseId=" + _caseId, "caseManagement");
                //PopupCaseID(_himmsConfiguration.CaseIdPopupUrl + "?extcaseId=" + _caseId, "caseManagement");
                else if (!string.IsNullOrEmpty(mid) && mid != "invalid" && mid != "unknown")
                {
                    string url = _himmsConfiguration.MIDPopupUrl + "?mid=" + mid;
                    //PopupMID(url, "MerchantPopup");
                    Navigate(url, "MerchantPopup");
                }
            }
            catch (Exception generalException)
            {
                _logger.Error("Error occurred as " + generalException.Message);
                _logger.Trace("Error Trace : " + generalException.StackTrace);
            }
        }

        public virtual void Subscribe(CallDataProviders provider)
        {
            try
            {
                provider.Subscribe(this);
                if (!string.IsNullOrEmpty(_himmsConfiguration.LoginUrl))
                {
                    _logger.Trace("Login URL: " + _himmsConfiguration.LoginUrl);
                    PopupLoginUrl(_himmsConfiguration.LoginUrl);
                }
                else
                    _logger.Warn("The login url is not specified.");

            }
            catch (Exception generalExcetion)
            {
                _logger.Error("Error occurred as " + generalExcetion.Message);
            }
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool BringWindowToTop(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int ShowWindow(IntPtr hWnd, int nCmdShow);

        private void DoEUPopup(KeyValueCollection kvUserData)
        {
            try
            {
                _caseId = null;
                if (kvUserData == null)
                {
                    _logger.Warn("The userdata is null to process the HIMMS integration.");
                    return;
                }

                if (kvUserData.ContainsKey("ConnectionId"))
                {
                    DesktopMessenger.midHandler = this;
                    DesktopMessenger.communicateUI.NotifyMIDState(true, kvUserData.GetAsString("ConnectionId"));
                }

                bool isShowPopupTab = kvUserData.ContainsKey("MIDPop") && kvUserData.GetAsString("MIDPop").ToLower() == "true";
                string mid = string.Empty;
                //  string caseId = string.Empty;
                string caseIdPopupURL = _himmsConfiguration.CaseIdPopupUrl;
                string urlToPopup = "";
                string title = "";
                if (kvUserData.ContainsKey("MID"))
                    mid = kvUserData.GetAsString("MID");

                if (kvUserData.ContainsKey("CaseID"))
                    _caseId = kvUserData.GetAsString("CaseID");

                if (kvUserData.ContainsKey("CASD_URL") && kvUserData.GetAsString("CASD_URL").ToLower() != "undefined" &&
                    kvUserData.GetAsString("CASD_URL").ToLower() != "null")
                {
                    caseIdPopupURL = kvUserData.GetAsString("CASD_URL");
                    title = "casd";
                    //  PopupCaseURL(caseIdPopupURL, "casd");
                    urlToPopup = caseIdPopupURL;
                    //  return;
                }

                if (isShowPopupTab) return;

                if (!string.IsNullOrEmpty(_caseId) && _caseId != "invalid" && _caseId != "unknown")
                {
                    //PopupCaseID(_himmsConfiguration.CaseIdPopupUrl + "?extcaseId=" + _caseId, "caseManagement");
                    urlToPopup = _himmsConfiguration.CaseIdPopupUrl + "?extcaseId=" + _caseId;
                    title = "caseManagement";
                }
                else if (!string.IsNullOrEmpty(mid) && mid != "invalid" && mid != "unknown")
                {
                    title = "MerchantPopup";
                    urlToPopup = _himmsConfiguration.MIDPopupUrl + "?mid=" + mid;
                    //PopupMID(_himmsConfiguration.MIDPopupUrl + "?mid=" + mid, "MerchantPopup");
                }

                else
                {
                    title = "MerchantPopup";
                    urlToPopup = _himmsConfiguration.MIDPopupUrl + "?mid=" + mid;
                    //PopupMID(_himmsConfiguration.MIDPopupUrl + "?mid=" + mid, "MerchantPopup");
                }

                Navigate(urlToPopup, title);

            }
            catch (Exception generalException)
            {
                _logger.Error("Error occurred as " + generalException.Message);
                _logger.Trace("Error Trace : " + generalException.StackTrace);
            }
        }

        private void DoNAPopup(KeyValueCollection kvUserData)
        {
            try
            {
                _caseId = null;
                if (kvUserData != null)
                {
                    string mid = string.Empty;

                    if (kvUserData.ContainsKey("MID"))
                        mid = kvUserData.GetAsString("MID");
                    if (string.IsNullOrEmpty(mid))
                        mid = "undefined";

                    if (!string.IsNullOrEmpty(_himmsConfiguration.MIDPopupUrl))
                    {
                        if (kvUserData.ContainsKey("ConnectionId"))
                        {
                            DesktopMessenger.midHandler = this;
                            DesktopMessenger.communicateUI.NotifyMIDState(true, kvUserData.GetAsString("ConnectionId"));
                        }

                        //PopupMID(_himmsConfiguration.MIDPopupUrl + "?mid=" + mid, "MerchantPopup");
                        Navigate(_himmsConfiguration.MIDPopupUrl + "?mid=" + mid, "MerchantPopup");
                    }

                    else
                        _logger.Warn("The popup url is not specified.");
                }
                else
                    _logger.Warn("The userdata object is null.");

            }
            catch (Exception generalException)
            {
                _logger.Error("Error occurred as " + generalException.Message);
                _logger.Trace("Error Trace : " + generalException.StackTrace);
            }
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

                    if (objType != null && obj != null)
                    {
                        if (userdata != null && userdata.ContainsKey("AppName"))
                        {
                            if (userdata.GetAsString("AppName").ToLower() == "cs")
                            {
                                if (_himmsConfiguration.AppLocation == "na")
                                    DoNAPopup(userdata);
                                else if (_himmsConfiguration.AppLocation == "eu")
                                    DoEUPopup(userdata);
                                else
                                    _logger.Warn("Invalid application location in the HIMMS integration. Application location: " + _himmsConfiguration.AppLocation);
                            }
                            else
                                _logger.Trace("There is invalid value in the 'AppName'");
                        }
                        else
                            _logger.Trace("The 'AppName' key not found in the user data.");
                    }
                    else
                        _logger.Warn("The event object is null.");
                }
                else
                    _logger.Warn("Voice event conversion getting failed");

            }
            catch (Exception generalException)
            {
                _logger.Error("Error occurred as " + generalException.Message);
                _logger.Trace("Error Trace : " + generalException.StackTrace);
            }
        }

        private void Navigate(string urlToPopup, string title)
        {
            switch (_himmsConfiguration.BrowserType)
            {
                case Pointel.Integration.Core.Data.BrowserType.IEShell:
                    NavigateIEShell(urlToPopup, title);
                    break;
                case Pointel.Integration.Core.Data.BrowserType.IExplorer:
                    NavigateIE(urlToPopup, title);
                    break;
                case Pointel.Integration.Core.Data.BrowserType.AID:
                    NavigateURLInAID(urlToPopup, "HIMMS");
                    break;
                default:
                    NavigateExternalBrowser(urlToPopup);
                    break;
            }
        }

        private void NavigateExternalBrowser(string url)
        {
            try
            {
                if (_browserHelperCaseData == null)
                    _browserHelperCaseData = new BrowserHelper();
                BrowserResult result = _browserHelperCaseData.Navigate((Pointel.WebDriver.BrowserType)((int)_himmsConfiguration.BrowserType), url);
            }
            catch (Exception generalException)
            {
                _logger.Error("Error occuured as " + generalException.Message);
                _logger.Trace("Error Trace: " + generalException.StackTrace);
            }
        }

        private void NavigateIE(string url, string title)
        {
            try
            {

                if (_loginBrowser != null)
                {
                    if (!isCaseIDOpened)
                    {
                        _logger.Trace("Going to take the document from the control.");
                        HTMLDocument htmlDoc = (HTMLDocument)_loginBrowser.Document;
                        _logger.Trace("The document retrieved from the control.");

                        var scriptErrorSuppressed = (IHTMLScriptElement)htmlDoc.createElement("SCRIPT");
                        scriptErrorSuppressed.type = "text/javascript";
                        scriptErrorSuppressed.text = "var caseIdWindow=null; function OpenCaseIdWindow(url,title){ if(caseIdWindow==null){caseIdWindow=window.open(url,title,\"location=no,directories=no,status=no,menubar=no,scrollbars=1,resizable=yes\");} else{caseIdWindow.location=url;} }";
                        scriptErrorSuppressed.text += "OpenCaseIdWindow('" + url + "','" + title + "')";
                        IHTMLElementCollection nodes = htmlDoc.getElementsByTagName("head");
                        foreach (IHTMLElement elem in nodes)
                        {
                            var head = (HTMLHeadElement)elem;
                            head.appendChild((IHTMLDOMNode)scriptErrorSuppressed);
                            break;
                        }
                    }
                    else
                    {
                        _logger.Trace("Going to take the document from the control.");
                        HTMLDocument htmlDoc = (HTMLDocument)_loginBrowser.Document;
                        _logger.Trace("The document retrieved from the control.");

                        var scriptErrorSuppressed = (IHTMLScriptElement)htmlDoc.createElement("SCRIPT");
                        scriptErrorSuppressed.type = "text/javascript";
                        scriptErrorSuppressed.text += "OpenCaseIdWindow('" + url + "','" + title + "')";
                        IHTMLElementCollection nodes = htmlDoc.getElementsByTagName("head");
                        foreach (IHTMLElement elem in nodes)
                        {
                            var head = (HTMLHeadElement)elem;
                            head.appendChild((IHTMLDOMNode)scriptErrorSuppressed);
                            break;
                        }
                    }
                }
                else
                    _logger.Trace("The browser window is null.");
            }
            catch (Exception generalException)
            {
                _logger.Error("Error occuured as " + generalException.Message);
                _logger.Trace("Error Trace: " + generalException.StackTrace);
            }
        }

        private void NavigateIEShell(string url, string title)
        {
            try
            {
                _logger.Trace("navigate the url. URL: " + url);

                var newBrowser = (SHDocVw.WebBrowser)(new SHDocVw.InternetExplorer());

                Uri uri = new Uri(url);
                SHDocVw.ShellWindows shellWindows = new ShellWindows();

                if (shellWindows != null && ieProcessID != 0 && shellWindows.Cast<SHDocVw.IWebBrowser2>().Any(x => x.HWND == ieProcessID))
                {
                    _logger.Trace("The exist window found to the machine");

                    var windows = shellWindows.Cast<SHDocVw.IWebBrowser2>().SingleOrDefault(x => x.HWND == ieProcessID);

                    if (windows.Document != null)
                    {
                        _logger.Trace("Going to retrieve the document from the browser.");

                        HTMLDocument htmlDoc = (HTMLDocument)windows.Document;

                        _logger.Trace("The document has been retrieved from the browser window.");
                        _logger.Trace("Going to inject the script to navigate the page.");

                        //IHTMLElementCollection nodes = htmlDoc.getElementsByTagName("script");
                        //if (nodes != null && nodes.length > 0)
                        //{
                        //    _logger.Trace("The script tag found in the document.");
                        //    foreach (IHTMLScriptElement scriptElement in nodes)
                        //    {
                        //        scriptElement.text = scriptElement.text + "window.location='" + url + "';";
                        //        _logger.Trace("Script injected successfully.");
                        //        break;
                        //    }
                        //}
                        //else
                        //{
                        //_logger.Trace("The script tag not found in the document.");

                        var scriptErrorSuppressed = (IHTMLScriptElement)htmlDoc.createElement("SCRIPT");
                        scriptErrorSuppressed.type = "text/javascript";
                        //scriptErrorSuppressed.text = "window.location='" + url + "';";
                        scriptErrorSuppressed.text = " window.open('" + url + "', '_self','" + title + "');";

                        IHTMLElementCollection headElement = htmlDoc.getElementsByTagName("head");
                        foreach (IHTMLElement elem in headElement)
                        {
                            var head = (HTMLHeadElement)elem;
                            head.appendChild((IHTMLDOMNode)scriptErrorSuppressed);
                            _logger.Trace("Script injected successfully.");
                            break;
                        }
                        //}

                    }
                    else
                        _logger.Warn("The document is null to the window");

                    //_logger.Trace("Going to navigate the URL in the old window - " + windows.HWND);

                    //windows.Navigate(url);

                    //_logger.Trace("The URL navigated in the old window.");
                }
                else
                {
                    _logger.Trace("The existing window is not found. So going to open new window.");

                    var newBrowser = (SHDocVw.WebBrowser)(new SHDocVw.InternetExplorer());
                    //newBrowser.Name = "HimmsMainWindow";

                    _logger.Trace("Going to navigate the URL in the new window.");

                    newBrowser.Navigate(url);
                    var inptr = new IntPtr(newBrowser.HWND);
                    ieProcessID = newBrowser.HWND;
                    ShowWindow(inptr, 9);

                    _logger.Trace("The URL navigated in the new window.");

                }
            }
            catch (Exception generalException)
            {
                _logger.Error("Error occurred as " + generalException.Message);
                _logger.Trace("Error Trace : " + generalException.StackTrace);
            }
        }

        private void NavigateURLInAID(string url, string appName)
        {
            try
            {
                if (DesktopMessenger.communicateUI != null)
                {
                    _logger.Trace("Going to notify the web url in the AID.");

                    DesktopMessenger.communicateUI.NotifyWebUrl(url, "himms", DesktopMessenger.totalWebIntegration);
                    _logger.Trace("The web url in the AID notified successfully.");
                }
                else
                    _logger.Warn("The Desktopn manager interface is null.");

            }
            catch (Exception generalException)
            {
                _logger.Error("Error occuured as " + generalException.Message);
                _logger.Trace("Error Trace: " + generalException.StackTrace);
            }
        }

        private void PopupLoginUrl(string url)
        {
            try
            {

                if (_himmsConfiguration.BrowserType == Pointel.Integration.Core.Data.BrowserType.IExplorer)
                {
                    if (!_himmsConfiguration.IsEnableSingleWindowPopup)
                        _loginBrowser = null;

                    if (_loginBrowser == null)
                    {
                        isCaseIDOpened = isCaseURLOpened = isMIDOpened = false;
                        try
                        {
                            _loginBrowser = (SHDocVw.WebBrowser)(new SHDocVw.InternetExplorer());
                            _loginBrowser.DocumentComplete += new DWebBrowserEvents2_DocumentCompleteEventHandler(_loginBrowser_DocumentComplete);
                            _loginBrowser.AddressBar = _himmsConfiguration.IsEnableAddressBar;
                            _loginBrowser.StatusBar = _himmsConfiguration.IsEnableStatusBar;
                            if (_himmsConfiguration.WindowWidth != 0)
                                _loginBrowser.Width = _himmsConfiguration.WindowWidth;
                            if (_himmsConfiguration.WindowHeight != 0)
                                _loginBrowser.Height = _himmsConfiguration.WindowHeight;
                            _loginBrowser.FullScreen = _himmsConfiguration.IsEnableFullScreen;
                            _loginBrowser.MenuBar = _himmsConfiguration.IsEnableMenuBar;
                            _loginBrowser.Resizable = _himmsConfiguration.IsEnableResizeWindow;
                        }
                        catch (Exception generalExcption)
                        {
                            _logger.Error("Error occurred as " + generalExcption.Message);
                            _logger.Trace("Error trace : " + generalExcption.StackTrace);
                            return;
                        }
                    }

                    _logger.Trace("Try to navigate through IE browser for login.");
                    try
                    {
                        _loginBrowser.Navigate(url);
                        var inptr = new IntPtr(_loginBrowser.HWND);
                        ShowWindow(inptr, 9);
                    }
                    catch (Exception generalException)
                    {
                        _logger.Warn("Error occurred as " + generalException.Message);
                        _loginBrowser = null;
                        PopupLoginUrl(url);
                    }

                }
                else if (_himmsConfiguration.BrowserType == Pointel.Integration.Core.Data.BrowserType.AID)
                {
                    DesktopMessenger.communicateUI.NotifyWebUrl(url, "himms", DesktopMessenger.totalWebIntegration);
                }
                else if (_himmsConfiguration.BrowserType == Pointel.Integration.Core.Data.BrowserType.Chrome || _himmsConfiguration.BrowserType == Pointel.Integration.Core.Data.BrowserType.Firefox)
                {
                    if (_browserHelperCaseData == null)
                        _browserHelperCaseData = new BrowserHelper();
                    BrowserResult result = _browserHelperCaseData.Navigate((Pointel.WebDriver.BrowserType)((int)_himmsConfiguration.BrowserType), url);
                }
                else
                {
                    _logger.Trace("Try to navigate through IE shell browser for login.");
                    NavigateIEShell(url, "");
                }
            }
            catch (Exception generalException)
            {
                _logger.Error("Error occurred as " + generalException.Message);
                _logger.Trace("Error Trace : " + generalException.StackTrace);
            }
        }

        void _loginBrowser_DocumentComplete(object pDisp, ref object URL)
        {
            isCaseIDOpened = isCaseURLOpened = isMIDOpened = false;
        }

        #endregion Methods
    }
}