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

using Newtonsoft.Json;
using Pointel.Salesforce.Adapter.Configurations;
using Pointel.Salesforce.Adapter.Email;
using Pointel.Salesforce.Adapter.LogMessage;
using Pointel.Salesforce.Adapter.SFDCModels;
using Pointel.Salesforce.Adapter.SFDCUtils;
using Pointel.Salesforce.Adapter.Voice;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pointel.Salesforce.Adapter.Utility
{
    internal class SFDCHttpServer
    {
        #region Fields

        public static bool CanGetTimeZoneFromSFDC = false;
        public static bool connectionStatus = false;
        public static bool IsFirstRequestMade = false;
        public static bool SessionFlag = false;
        public static bool EnableNumberEditPrompt = false;
        internal static Dictionary<string, string> _emailData = new Dictionary<string, string>();

        private static IDictionary<string, string> _mimeTypeMappings = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase) {

        #region extension to MIME type list

        {".asf", "video/x-ms-asf"},
        {".asx", "video/x-ms-asf"},
        {".avi", "video/x-msvideo"},
        {".bin", "application/octet-stream"},
        {".cco", "application/x-cocoa"},
        {".crt", "application/x-x509-ca-cert"},
        {".css", "text/css"},
        {".deb", "application/octet-stream"},
        {".der", "application/x-x509-ca-cert"},
        {".dll", "application/octet-stream"},
        {".dmg", "application/octet-stream"},
        {".ear", "application/java-archive"},
        {".eot", "application/octet-stream"},
        {".exe", "application/octet-stream"},
        {".flv", "video/x-flv"},
        {".gif", "image/gif"},
        {".hqx", "application/mac-binhex40"},
        {".htc", "text/x-component"},
        {".htm", "text/html"},
        {".html", "text/html"},
        {".ico", "image/x-icon"},
        {".img", "application/octet-stream"},
        {".iso", "application/octet-stream"},
        {".jar", "application/java-archive"},
        {".jardiff", "application/x-java-archive-diff"},
        {".jng", "image/x-jng"},
        {".jnlp", "application/x-java-jnlp-file"},
        {".jpeg", "image/jpeg"},
        {".jpg", "image/jpeg"},
        {".js", "application/x-javascript"},
        {".mml", "text/mathml"},
        {".mng", "video/x-mng"},
        {".mov", "video/quicktime"},
        {".mp3", "audio/mpeg"},
        {".mpeg", "video/mpeg"},
        {".mpg", "video/mpeg"},
        {".msi", "application/octet-stream"},
        {".msm", "application/octet-stream"},
        {".msp", "application/octet-stream"},
        {".pdb", "application/x-pilot"},
        {".pdf", "application/pdf"},
        {".pem", "application/x-x509-ca-cert"},
        {".pl", "application/x-perl"},
        {".pm", "application/x-perl"},
        {".png", "image/png"},
        {".prc", "application/x-pilot"},
        {".ra", "audio/x-realaudio"},
        {".rar", "application/x-rar-compressed"},
        {".rpm", "application/x-redhat-package-manager"},
        {".rss", "text/xml"},
        {".run", "application/x-makeself"},
        {".sea", "application/x-sea"},
        {".shtml", "text/html"},
        {".sit", "application/x-stuffit"},
        {".swf", "application/x-shockwave-flash"},
        {".tcl", "application/x-tcl"},
        {".tk", "application/x-tcl"},
        {".txt", "text/plain"},
        {".war", "application/java-archive"},
        {".wbmp", "image/vnd.wap.wbmp"},
        {".wmv", "video/x-ms-wmv"},
        {".xml", "text/xml"},
        {".xpi", "application/x-xpinstall"},
        {".zip", "application/zip"},
        #endregion extension to MIME type list
    };

        private static SFDCHttpServer _thisObject = null;
        private bool _adapeterStopped = false;
        private GeneralOptions _generalOptions;
        private String _jsonEvent = null;
        private DateTime _lastPingTime;
        private HttpListener _listener;
        private Log _logger;
        private System.Timers.Timer _pingTimer = null;
        private int _port;
        private string _rootDirectory;
        private Thread _serverThread;
        private PopupData _sfdcData = null;
        private string _startPage = "index.html";
        private VoiceManager _voiceEvents = null;

        #endregion Fields

        #region Constructor

        private SFDCHttpServer()
        {
            this._logger = Log.GenInstance();
            this._logger.Info("SFDCHttpServer Instance created...");
            this._generalOptions = Settings.SFDCOptions;
            _voiceEvents = VoiceManager.GetInstance();
            if (Settings.SFDCOptions.EnableTimerForSFDCPingCheck)
            {
                this._logger.Info("SFDCHttpServer:Timer is enabled for checking sfdc connection status..");
                _pingTimer = new System.Timers.Timer(Settings.SFDCOptions.PingCheckElapsedTime);
                _pingTimer.Elapsed += pingTimer_Elapsed;
            }
        }

        #endregion Constructor

        #region GetInstance

        public static SFDCHttpServer GetInstance()
        {
            if (_thisObject == null)
            {
                _thisObject = new SFDCHttpServer();
            }
            return _thisObject;
        }

        #endregion GetInstance

        #region StartListener

        public void StartListener(string path, int port)
        {
            try
            {
                this._logger.Info("Starting HTTP Listener thread...");
                this._logger.Info("Port number :" + Convert.ToString(port));
                this._logger.Info("Root Directory Path :" + path);
                this._rootDirectory = path;
                this._port = port;
                _serverThread = new Thread(this.Listen);
                _serverThread.Start();
            }
            catch (Exception generalException)
            {
                _logger.Error("StartListener : Error occurred while initializing Salesforce HTTP Server " + generalException.ToString());
            }
        }

        #endregion StartListener

        #region Listen

        private void Listen()
        {
            try
            {
                _logger.Info("HTTP Server started listening ");
                _listener = new HttpListener();
                //add url to listen
                foreach (string url in Settings.SFDCOptions.ListenerURLs)
                {
                    _listener.Prefixes.Add(url);
                    _logger.Info("URL Added in HttpListerner :" + url);
                }
                _listener.Start();
                if (Settings.SFDCOptions.EnableTimerForSFDCPingCheck && _pingTimer != null)
                {
                    _pingTimer.Start();
                }
                while (!_adapeterStopped)
                {
                    try
                    {
                        IAsyncResult result = _listener.BeginGetContext(new AsyncCallback(Process), _listener);
                        result.AsyncWaitHandle.WaitOne();
                    }
                    catch (Exception generalException)
                    {
                        _logger.Error("Error on asynchronous listener : " + generalException.ToString());
                    }
                }
            }
            catch (Exception generalException)
            {
                _logger.Error("Error occurred while listening HTTP Server : " + generalException.ToString());
            }
        }

        private void pingTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                if (_lastPingTime != null && DateTime.Now.Subtract(_lastPingTime).TotalMilliseconds > Settings.SFDCOptions.PingCheckElapsedTime)
                {
                    //set disconnect status
                    if (Settings.SFDCListener.IsSFDCConnected)
                    {
                        if (Settings.SFDCOptions.NotifyAllConnectionStateChange)
                        {
                            Settings.SFDCListener.SendSessionStatus(SFDCSessionStatus.NotConnected);
                        }
                        connectionStatus = false;
                        this._logger.Info("pingTimer_Elapsed: Salesforce DisConnected from SFDC Adapter.....");
                        if (Settings.SFDCOptions.AlertSFDCConnectionStatus)
                        {
                            Settings.SFDCListener.SFDCConnectionStatus(LogMode.Error, Settings.SFDCOptions.SFDCConnectionFailureMessage);
                        }
                    }
                }
            }
            catch (Exception generalException)
            {
                _logger.Error("pingTimer_Elapsed: Error occurred checking SFDC last ping time :  " + generalException.ToString());
            }
        }

        #endregion Listen

        #region Stop Listener

        public void StopListener()
        {
            try
            {
                this._logger.Info("StopListener: Stopping HTTP Listener thread...");
                if (_listener != null && _serverThread != null)
                {
                    _adapeterStopped = true;
                    Thread.Sleep(2000);
                    _listener.Stop();
                    _logger.Error("StopListener : Aborting the Listener Thread...");
                    _serverThread.Abort();
                    if (Settings.SFDCOptions.EnableTimerForSFDCPingCheck && _pingTimer != null)
                    {
                        _pingTimer.Stop();
                    }
                }
            }
            catch (ThreadAbortException abortException)
            {
                _logger.Error("StopListener : Thread Abort exception Handled :" + abortException.Message);
            }
            catch (Exception generalException)
            {
                _logger.Error("StopListener : Error occurred while stopping HTTP Server listener :  " + generalException.ToString());
            }
        }

        #endregion Stop Listener

        #region Process Request and Response

        private void Process(IAsyncResult result)
        {
            try
            {
                _lastPingTime = DateTime.Now;
                HttpListener listener = (HttpListener)result.AsyncState;
                listener.IgnoreWriteExceptions = true;
                if (listener.IsListening && !_adapeterStopped)
                {
                    HttpListenerContext context = listener.EndGetContext(result);
                    HttpListenerRequest request = context.Request;
                    // Obtain a response object.
                    HttpListenerResponse response = context.Response;
                    string filename = context.Request.Url.AbsolutePath;
                    string ack = context.Request.QueryString["ack"];
                    filename = filename.Substring(1);

                    if (string.IsNullOrEmpty(filename))
                    {
                        if (File.Exists(Path.Combine(_rootDirectory, _startPage)))
                        {
                            filename = _startPage;
                        }
                    }

                    #region Dial Region

                    if (filename.Equals("dial"))
                    {
                        try
                        {
                            string phoneno = context.Request.QueryString["phoneno"];
                            _logger.Info("Click2Dial Phone number received from salesforce : " + phoneno);
                            _logger.Info("Click2Dial Record Type :" + context.Request.QueryString["Type"]);
                            _logger.Info("Click2Dial Record Id :" + context.Request.QueryString["Id"]);
                            if (!string.IsNullOrEmpty(phoneno))
                            {
                                Task.Run(() => HandleClickToDial(phoneno, context.Request.QueryString["Type"], context.Request.QueryString["Id"]));
                                context.Response.OutputStream.Flush();
                                context.Response.StatusCode = (int)HttpStatusCode.OK;
                                context.Response.OutputStream.Close();
                                return;
                            }
                            else
                            {
                                this._logger.Info("Can not dial a call because phone number is null");
                            }
                        }
                        catch (Exception generalException)
                        {
                            _logger.Error("Process : Error occurred while processing dial request : " + generalException.ToString());
                            context.Response.OutputStream.Flush();
                            context.Response.StatusCode = (int)HttpStatusCode.OK;
                            context.Response.OutputStream.Close();
                            return;
                        }
                    }

                    #endregion Dial Region

                    #region Click2Email

                    if (filename.Equals("click2email"))
                    {
                        try
                        {
                            string id = context.Request.QueryString["id"];
                            string objectName = context.Request.QueryString["object"];
                            string email = context.Request.QueryString["email"];
                            _logger.Info("Email id received from salesforce, Id: " + email + " Object : " + objectName + " RecordId :" + id);
                            if (!String.IsNullOrEmpty(email))
                            {
                                Task.Run(() => ClickToEmailData(email, objectName, id));
                            }
                            context.Response.OutputStream.Flush();
                            context.Response.StatusCode = (int)HttpStatusCode.OK;
                            context.Response.OutputStream.Close();
                            return;
                        }
                        catch (Exception generalException)
                        {
                            _logger.Error("Process : Error occured while processing click2email request : " + generalException.ToString());
                            context.Response.OutputStream.Flush();
                            context.Response.StatusCode = (int)HttpStatusCode.OK;
                            context.Response.OutputStream.Close();
                            return;
                        }
                    }

                    #endregion Click2Email

                    #region SessionId Region

                    if (filename.Equals("sessionid"))
                    {
                        try
                        {
                            string sessionId = context.Request.QueryString["sessionid"];
                            _logger.Info("SessionId Received from Salesforce : " + (string.IsNullOrEmpty(sessionId) ? "0 Length" :
                                Convert.ToString(sessionId.Length)));
                            if (!string.IsNullOrEmpty(sessionId))
                            {
                                if (!string.IsNullOrEmpty(SFDCUtility.SForce.SessionHeaderValue.sessionId) &&
                                    (SFDCUtility.SForce.SessionHeaderValue.sessionId == sessionId))
                                {
                                    _logger.Info("Same SessionId Received from Salesforce, TestRequest is not invoked");
                                }
                                else
                                {
                                    SFDCUtility.SForce.SessionHeaderValue.sessionId = sessionId;
                                    _logger.Info("New SessionId Received from Salesforce, Invoking Salesforce TestRequest");
                                    new System.Threading.Tasks.Task(() =>
                                   Pointel.Salesforce.Adapter.SFDCModels.SFDCUtility.GetInstance().SendTestRequest()).Start();
                                }
                            }
                            context.Response.OutputStream.Flush();
                            context.Response.StatusCode = (int)HttpStatusCode.OK;
                            context.Response.OutputStream.Close();
                            return;
                        }
                        catch (Exception generalException)
                        {
                            _logger.Error("Process : Error occurred while receiving sesssionId : " + generalException.ToString());
                        }
                    }

                    #endregion SessionId Region

                    #region TimeZone Region

                    if (filename.Equals("timezone"))
                    {
                        try
                        {
                            string timeZone = context.Request.QueryString["timezone"];
                            _logger.Info("TimeZone received from Salesforce :" + timeZone);
                            if (!string.IsNullOrEmpty(timeZone))
                            {
                                if (CanGetTimeZoneFromSFDC)
                                {
                                    timeZone = timeZone.Substring(1, timeZone.IndexOf(')') - 1);
                                    Settings.SFDCOptions.SFDCTimeZone = timeZone;
                                    _logger.Info("TimeZone value taken as :" + timeZone);
                                }
                            }
                            context.Response.OutputStream.Flush();
                            context.Response.StatusCode = (int)HttpStatusCode.OK;
                            context.Response.OutputStream.Close();
                            return;
                        }
                        catch (Exception generalException)
                        {
                            _logger.Error("Process : Error occurred while receiving TimeZone from SFDC : " + generalException.ToString());
                        }
                    }

                    #endregion TimeZone Region

                    #region ScriptError Region

                    if (filename.Equals("scripterror"))
                    {
                        try
                        {
                            _logger.Error("Script Error Received: " + context.Request.QueryString["log"]);
                            context.Response.OutputStream.Flush();
                            context.Response.StatusCode = (int)HttpStatusCode.OK;
                            context.Response.OutputStream.Close();
                            return;
                        }
                        catch (Exception generalException)
                        {
                            _logger.Error("Process : Error occurred receiving script error : " + generalException.ToString());
                        }
                    }

                    #endregion ScriptError Region

                    #region SFDCConnection Opened

                    if (filename.Equals("opened"))
                    {
                        EnableNumberEditPrompt = true;
                        if (IsFirstRequestMade && Settings.SFDCOptions.NotifyAllConnectionStateChange)
                        {
                            Settings.SFDCListener.SendSessionStatus(SFDCSessionStatus.Connected);
                        }
                        connectionStatus = true;

                        if (IsFirstRequestMade)
                            this._logger.Info("Salesforce Connected with SFDC Adapter.....");
                        else
                            this._logger.Info("Adapter page loaded in Salesforce,SessionId is not verfied yet..");

                        if (IsFirstRequestMade && Settings.SFDCOptions.AlertSFDCConnectionStatus)
                        {
                            Settings.SFDCListener.SFDCConnectionStatus(LogMode.Info, Settings.SFDCOptions.SFDCConnectionSuccessMessage);
                        }
                        context.Response.OutputStream.Flush();
                        context.Response.StatusCode = (int)HttpStatusCode.OK;
                        context.Response.OutputStream.Close();
                    }

                    #endregion SFDCConnection Opened

                    #region SFDCConnection Closed

                    if (filename.Equals("closed"))
                    {
                        if (Settings.SFDCOptions.NotifyAllConnectionStateChange)
                        {
                            Settings.SFDCListener.SendSessionStatus(SFDCSessionStatus.NotConnected);
                        }
                        connectionStatus = false;
                        this._logger.Warn("Salesforce DisConnected from SFDC Adapter.....");
                        if (Settings.SFDCOptions.AlertSFDCConnectionStatus)
                        {
                            Settings.SFDCListener.SFDCConnectionStatus(LogMode.Error, Settings.SFDCOptions.SFDCConnectionFailureMessage);
                        }
                        context.Response.OutputStream.Flush();
                        context.Response.StatusCode = (int)HttpStatusCode.OK;
                        context.Response.OutputStream.Close();
                    }

                    #endregion SFDCConnection Closed

                    #region Push Region

                    if (filename.Equals("push"))
                    {
                        try
                        {
                            if (!Settings.SFDCListener.IsSFDCConnected)
                            {
                                if (IsFirstRequestMade && Settings.SFDCOptions.NotifyAllConnectionStateChange)
                                {
                                    Settings.SFDCListener.SendSessionStatus(SFDCSessionStatus.Connected);
                                }
                                if (IsFirstRequestMade && Settings.SFDCOptions.AlertSFDCConnectionStatus)
                                {
                                    Settings.SFDCListener.SFDCConnectionStatus(LogMode.Info, Settings.SFDCOptions.SFDCConnectionSuccessMessage);
                                }
                            }
                            context.Response.ContentType = "text/html";
                            context.Response.AddHeader("Date", DateTime.Now.ToString("r"));
                            int i = 0;
                            string noEvent = " { \"Event\":  " + "\"" + "Ping" + "\"" + ", \"ANI\": " + "\"" + "test" + "\"" + " }";
                            byte[] pingResponse = ASCIIEncoding.ASCII.GetBytes(noEvent);
                            while (i <= 20)
                            {
                                if (ack == "true")
                                {
                                    context.Response.OutputStream.Write(pingResponse, 0, pingResponse.Length);

                                    context.Response.OutputStream.Flush();
                                    context.Response.StatusCode = (int)HttpStatusCode.OK;
                                    context.Response.OutputStream.Close();
                                    return;
                                }
                                if (SessionFlag)
                                {
                                    SessionFlag = false;
                                    string session = " { \"Event\":  " + "\"" + "sessionid" + "\"" + ", \"ANI\": " + "\"" + "test" + "\"" + " }";
                                    byte[] sessionResponse = ASCIIEncoding.ASCII.GetBytes(session);
                                    context.Response.OutputStream.Write(sessionResponse, 0, sessionResponse.Length);
                                    context.Response.OutputStream.Flush();
                                    context.Response.StatusCode = (int)HttpStatusCode.OK;
                                    context.Response.OutputStream.Close();
                                    return;
                                }

                                if (EnableNumberEditPrompt && Settings.SFDCOptions.EnableClick2DialNumberEditPrompt)
                                {
                                    EnableNumberEditPrompt = false;
                                    string promptEvent = " { \"Event\":  " + "\"" + "init" + "\"" + ", \"EnablePrompt\": " + "\"" + "true" + "\"" + " }";
                                    byte[] promptResponse = ASCIIEncoding.ASCII.GetBytes(promptEvent);

                                    context.Response.OutputStream.Write(promptResponse, 0, promptResponse.Length);
                                    context.Response.OutputStream.Flush();
                                    context.Response.StatusCode = (int)HttpStatusCode.OK;
                                    context.Response.OutputStream.Close();
                                    return;
                                }

                                if (Settings.SFDCPopupData.Count > 0)
                                {
                                    foreach (string key in Settings.SFDCPopupData.Keys)
                                    {
                                        _sfdcData = Settings.SFDCPopupData[key];
                                        Settings.SFDCPopupData.Remove(key);
                                        break;
                                    }
                                    try
                                    {
                                        _jsonEvent = JsonConvert.SerializeObject(_sfdcData);
                                        byte[] respons = ASCIIEncoding.ASCII.GetBytes(_jsonEvent);
                                        context.Response.OutputStream.Write(respons, 0, respons.Length);
                                        context.Response.OutputStream.Flush();
                                        context.Response.StatusCode = (int)HttpStatusCode.OK;
                                        context.Response.OutputStream.Close();
                                        try
                                        {
                                            _logger.Info("\n******Sending popup data to SalesForce ******" + _sfdcData.ToString()
                                                + "\n***************************************");
                                        }
                                        catch (Exception)
                                        {
                                        }
                                    }
                                    catch (Exception generalException)
                                    {
                                        _logger.Error("Error occurred while sending search and Activity Log data to salesforce : " + generalException.ToString());
                                    }
                                    return;
                                }
                                Thread.Sleep(200);
                                i++;
                            }
                            context.Response.OutputStream.Write(pingResponse, 0, pingResponse.Length);
                            context.Response.OutputStream.Flush();
                            context.Response.StatusCode = (int)HttpStatusCode.OK;
                            context.Response.OutputStream.Close();
                            return;
                        }
                        catch (HttpListenerException listenerException)
                        {
                            _logger.Warn("Process : Listener write operation can't be performed : " + listenerException.ToString());
                        }
                        catch (Exception generalException)
                        {
                            _logger.Error("Process : Error occurred while sending json data to Salesforce  : " + generalException.ToString());
                        }
                    }

                    #endregion Push Region

                    #region Basic Function

                    filename = Path.Combine(_rootDirectory, filename);
                    if (File.Exists(filename))
                    {
                        try
                        {
                            Stream input = new FileStream(filename, FileMode.Open, FileAccess.Read);
                            //Adding permanent http response headers
                            string mime;
                            context.Response.ContentType = _mimeTypeMappings.TryGetValue(Path.GetExtension(filename), out mime) ? mime : "application/octet-stream";
                            context.Response.ContentLength64 = input.Length;
                            context.Response.AddHeader("Date", DateTime.Now.ToString("r"));
                            context.Response.AddHeader("Last-Modified", System.IO.File.GetLastWriteTime(filename).ToString("r"));

                            byte[] buffer = new byte[1024 * 16];
                            int nbytes;
                            while ((nbytes = input.Read(buffer, 0, buffer.Length)) > 0)
                                context.Response.OutputStream.Write(buffer, 0, nbytes);
                            input.Close();
                            context.Response.OutputStream.Flush();
                            context.Response.StatusCode = (int)HttpStatusCode.OK;
                        }
                        catch (Exception generalException)
                        {
                            _logger.Error("process() : Error occurred while executing start page file from process method  " + generalException.ToString());
                            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        }
                    }
                    else
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    }

                    #endregion Basic Function

                    context.Response.OutputStream.Close();
                }
            }
            catch (Exception generalException)
            {
                _logger.Error("Process : Error occurred while processing request from Salesforce or iWS/WDE : " + generalException.ToString());
            }
        }

        #endregion Process Request and Response

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
                this._logger.Error("TruncateNumbers : Error occurred while Truncating incoming data from Salesforce :" + generalException.ToString());
            }
            return string.Empty;
        }

        #endregion Get Truncated number

        #region ClickToEmailData

        private void ClickToEmailData(string email, string objectType, string recordId)
        {
            try
            {
                if (!String.IsNullOrEmpty(email) && !String.IsNullOrEmpty(objectType) && !String.IsNullOrEmpty(recordId))
                {
                    //add to collection
                    EmailEventHandler.IsClick2Email = true;
                    if (!_emailData.ContainsKey(email))
                    {
                        _emailData.Add(email, objectType + "," + recordId);
                    }
                    else
                    {
                        _emailData[email] = objectType + "," + recordId;
                    }
                    Settings.SFDCListener.MakeOutboundEmail(email, recordId, objectType);
                }
            }
            catch (Exception generalException)
            {
                this._logger.Error("Error occurred on Click2Email data procession : " + generalException.ToString());
            }
        }

        #endregion ClickToEmailData

        private void HandleClickToDial(string phoneno, string type, string id)
        {
            if (Settings.SFDCOptions.EnableClickToDialOnNotReady || Settings.AgentDetails.AgentStatus == CurrentAgentStatus.Ready)
            {
                if (Settings.SFDCOptions.EnableTruncateClick2DialNumber && Settings.SFDCOptions.TruncateClick2DialNumberLength > 0)
                {
                    phoneno = TruncateNumbers(phoneno, Settings.SFDCOptions.TruncateClick2DialNumberLength);
                    _logger.Info("Phone number after truncating the click2dial number : " + phoneno);
                }
                if (!string.IsNullOrWhiteSpace(Settings.SFDCOptions.OutboundVoiceDialPlanPrefix))
                {
                    phoneno = Settings.SFDCOptions.OutboundVoiceDialPlanPrefix + phoneno;
                    this._logger.Info("Click2Dial Number after appending outbound prefix :" + phoneno);
                }
                if (Settings.AgentDetails.IsAgentOnCall > 0)
                {
                    if (Settings.SFDCOptions.EnableConsultDialingFromSFDC)
                        Settings.SFDCListener.MakeConsultCall(phoneno, "ClickToDial", phoneno + "," + type + "," + id, VoiceEventHandler.eventEstablished.ConnID);
                    else
                        this._logger.Info("Dialing consult call is disabled...");
                }
                else
                {
                    Settings.SFDCListener.MakeOutboundCall(phoneno, "ClickToDial", phoneno + "," + type + "," + id);
                }
            }
            else
            {
                this._logger.Info("Can not dial outbound call at this time");
            }
        }
    }
}