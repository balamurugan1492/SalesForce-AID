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

namespace Pointel.Salesforce.Adapter.Utility
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Text;
    using System.Threading;

    using Newtonsoft.Json;

    using Pointel.Salesforce.Adapter.Configurations;
    using Pointel.Salesforce.Adapter.LogMessage;
    using Pointel.Salesforce.Adapter.PForce;
    using Pointel.Salesforce.Adapter.SFDCModels;
    using Pointel.Salesforce.Adapter.SFDCUtils;
    using Pointel.Salesforce.Adapter.Voice;

    internal class SFDCHttpServer
    {
        #region Fields

        public static bool CanGetTimeZoneFromSFDC = false;
        public static bool connectionStatus = false;
        public static bool flagPrompt = false;
        public static bool IsFirstRequestMade = false;
        public static bool NewSessionIDFlag = false;
        public static bool sessionFlag = false;

        private static SFDCHttpServer thisObject = null;
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

        private bool AdapeterStopped = false;
        private GeneralOptions generalOptions;
        private String jsonEvent = null;
        private DateTime lastPingTime;
        private Log logger;
        private VoiceMakeOutbound makeOutboundCall = null;
        private System.Timers.Timer pingTimer = null;
        private System.Timers.Timer sessionKeepAliveTimer = null;
        private PopupData sfdcData = null;
        private string startPage = "index.html";
        private GetUserInfoResult userinforesult = null;
        private VoiceEvents voiceEvents = null;
        private string _host;
        private HttpListener _listener;
        private int _port;
        private string _rootDirectory;
        private Thread _serverThread;

        #endregion Fields

        #region Constructors

        private SFDCHttpServer()
        {
            this.logger = Log.GenInstance();
            this.makeOutboundCall = VoiceMakeOutbound.GetInstance();
            this.generalOptions = Settings.SFDCOptions;
            voiceEvents = VoiceEvents.GetInstance();
            if (Settings.SFDCOptions.EnableTimerForSFDCPingCheck)
            {
                pingTimer = new System.Timers.Timer(Settings.SFDCOptions.PingCheckElapsedTime);
                pingTimer.Elapsed += pingTimer_Elapsed;
            }
        }

        #endregion Constructors

        #region Methods

        public static SFDCHttpServer GetInstance()
        {
            if (thisObject == null)
            {
                thisObject = new SFDCHttpServer();
            }
            return thisObject;
        }

        public void SessionKeepAliveInit()
        {
            if (Settings.SFDCOptions.CanEnableKeepAliveSessionID)
            {
                if (sessionKeepAliveTimer != null)
                {
                    sessionKeepAliveTimer.Stop();
                    sessionKeepAliveTimer.Elapsed -= sessionKeepAliveTimer_Elapsed;
                    sessionKeepAliveTimer = null;
                }
                sessionKeepAliveTimer = new System.Timers.Timer(Settings.SFDCOptions.KeepAliveSeesionIDInterval);
                sessionKeepAliveTimer.Elapsed += sessionKeepAliveTimer_Elapsed;
                sessionKeepAliveTimer.Start();
            }
        }

        public void StartListener(string path, int port, string host)
        {
            try
            {
                this._rootDirectory = path;
                this._port = port;
                this._host = host;
                _serverThread = new Thread(this.Listen);
                _serverThread.Start();
            }
            catch (Exception generalException)
            {
                logger.Error("StartListener : Error occurred while initializing Salesforce HTTP Server " + generalException.ToString());
            }
        }

        public void StopListener()
        {
            try
            {
                if (_listener != null && _serverThread != null)
                {
                    AdapeterStopped = true;
                    Thread.Sleep(2000);
                    _listener.Stop();
                    _serverThread.Abort();
                    if (Settings.SFDCOptions.EnableTimerForSFDCPingCheck && pingTimer != null)
                    {
                        pingTimer.Stop();
                    }
                }
            }
            catch (Exception generalException)
            {
                logger.Error("StopListener : Error occurred while stopping HTTP Server listener :  " + generalException.ToString());
            }
        }

        private void Listen()
        {
            try
            {
                logger.Info("HTTP Server started listening ");
                _listener = new HttpListener();
                _listener.Prefixes.Add(_host + ":" + _port.ToString() + "/");
                _listener.Start();
                if (Settings.SFDCOptions.EnableTimerForSFDCPingCheck && pingTimer != null)
                {
                    pingTimer.Start();
                }
                while (!AdapeterStopped)
                {
                    try
                    {
                        IAsyncResult result = _listener.BeginGetContext(new AsyncCallback(Process), _listener);
                        result.AsyncWaitHandle.WaitOne();
                    }
                    catch (Exception generalException)
                    {
                        logger.Error("Error on asynchronous listener : " + generalException.ToString());
                    }
                }
            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred while listening HTTP Server : " + generalException.ToString());
            }
        }

        private void pingTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                if (lastPingTime != null && DateTime.Now.Subtract(lastPingTime).TotalMilliseconds > Settings.SFDCOptions.PingCheckElapsedTime)
                {
                    //set disconnect status
                    if (Settings.SFDCListener.IsSFDCConnected)
                    {
                        if (Settings.SFDCOptions.NotifyAllConnectionStateChange)
                        {
                            Settings.SFDCListener.SendSessionStatus(SFDCSessionStatus.NotConnected);
                        }
                        connectionStatus = false;
                        this.logger.Warn("Salesforce DisConnected from SFDC Adapter.....");
                        if (Settings.SFDCOptions.AlertSFDCConnectionStatus)
                        {
                            Settings.SFDCListener.SFDCConnectionStatus(LogMode.Error, Settings.SFDCOptions.SFDCConnectionFailureMessage);
                        }
                    }
                }
            }
            catch (Exception generalException)
            {
                logger.Error("pingTimer_Elapsed: Error occurred checking SFDC last ping time :  " + generalException.ToString());
            }
        }

        private void Process(IAsyncResult result)
        {
            try
            {
                lastPingTime = DateTime.Now;
                HttpListener listener = (HttpListener)result.AsyncState;
                listener.IgnoreWriteExceptions = true;
                if (listener.IsListening && !AdapeterStopped)
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
                        if (File.Exists(Path.Combine(_rootDirectory, startPage)))
                        {
                            filename = startPage;
                        }
                    }

                    #region Dial Region

                    if (filename.Equals("dial"))
                    {
                        try
                        {
                            string phoneno = context.Request.QueryString["phoneno"];
                            logger.Info("Phone number received from salesforce : " + phoneno);
                            if (!String.IsNullOrEmpty(phoneno))
                            {
                                //phoneno = TruncateNumbers(phoneno, 10);
                                logger.Info("Invoking Call dialing procedure");
                                this.makeOutboundCall.MakeVoiceCall(phoneno, context.Request.QueryString["Type"], context.Request.QueryString["Id"]);
                                context.Response.OutputStream.Flush();
                                context.Response.StatusCode = (int)HttpStatusCode.OK;
                                context.Response.OutputStream.Close();
                                return;
                            }
                        }
                        catch (Exception generalException)
                        {
                            logger.Error("Process : Error occurred while processing dial request : " + generalException.ToString());
                        }
                    }

                    #endregion Dial Region

                    #region SessionId Region

                    if (filename.Equals("sessionid"))
                    {
                        try
                        {
                            string sessionId = context.Request.QueryString["sessionid"];
                            context.Response.ContentType = "text/html";
                            context.Response.AddHeader("Date", DateTime.Now.ToString("r"));
                            logger.Info("SessionId Received from Salesforce : " + (string.IsNullOrEmpty(sessionId) ? "0 Length" :
                                Convert.ToString(sessionId.Length)));
                            if (Settings.SFDCOptions.CanEnableSessionIDInLog)
                            {
                                logger.Info("Received session id is:" + sessionId);
                            }
                            if (!string.IsNullOrEmpty(sessionId))
                            {
                                //if (!string.IsNullOrEmpty(SFDCUtility.SForce.SessionHeaderValue.sessionId) &&
                                //    (SFDCUtility.SForce.SessionHeaderValue.sessionId == sessionId))
                                //{
                                //    logger.Info("Same SessionId Received from Salesforce, TestRequest not invoked");
                                //}
                                //else
                                {
                                    SFDCUtility.SForce.SessionHeaderValue.sessionId = sessionId;
                                    SessionKeepAliveInit();
                                    if (!NewSessionIDFlag)
                                    {
                                        logger.Info("New SessionId Received from Salesforce, Invoking Salesforce TestRequest");

                                        new System.Threading.Tasks.Task(() =>
                                       Pointel.Salesforce.Adapter.SFDCModels.SFDCUtility.GetInstance().SendTestRequest()).Start();
                                    }
                                    else
                                    {
                                        logger.Info("sessionId received retrying search operation");
                                        SFDCUtility sFDCUtility = SFDCUtility.GetInstance();
                                        if (sFDCUtility != null)
                                        {
                                            Settings.SFDCListener.SendSessionStatus(SFDCSessionStatus.Connected);
                                            logger.Info("sFDCUtility.SessionSearchCollection count :" + sFDCUtility.SessionSearchCollection.Count);
                                            logger.Info("sFDCUtility.SessionCreateActivityCollection count :" + sFDCUtility.SessionCreateActivityCollection.Count);
                                            logger.Info("sFDCUtility.SessionUpdateActivityCollection count :" + sFDCUtility.SessionUpdateActivityCollection.Count);
                                            if (sFDCUtility.SessionSearchCollection.Count > 0)
                                            {
                                                foreach (var tuple in sFDCUtility.SessionSearchCollection.ToArray())
                                                {
                                                    logger.Info("search method invoked for interaction id :" + tuple.Item1);
                                                    VoiceEvents.GetInstance().ProcessSearchData(tuple.Item1, tuple.Item3, tuple.Item2);
                                                }
                                                NewSessionIDFlag = false;
                                            }
                                            if (sFDCUtility.SessionCreateActivityCollection.Count > 0)
                                            {
                                                foreach (var tuple in sFDCUtility.SessionCreateActivityCollection.ToArray())
                                                {
                                                    logger.Info("Create activity log method invoked for interaction id :" + tuple.Item1);
                                                    sFDCUtility.CreateActivityLog(tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5);
                                                }
                                                NewSessionIDFlag = false;
                                            }
                                            if (sFDCUtility.SessionUpdateActivityCollection.Count > 0)
                                            {
                                                foreach (var tuple in sFDCUtility.SessionUpdateActivityCollection.ToArray())
                                                {
                                                    logger.Info("update activity log method invoked for interaction id :" + tuple.Item1);
                                                    sFDCUtility.UpdateActivityLog(tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5);
                                                }
                                                NewSessionIDFlag = false;
                                            }
                                        }
                                        else
                                            logger.Error("SFDCHttpServer: Session id: SFDC utility object is null cannot perform retry");
                                    }
                                }
                            }
                            context.Response.OutputStream.Flush();
                            context.Response.StatusCode = (int)HttpStatusCode.OK;
                            context.Response.OutputStream.Close();
                            return;
                        }
                        catch (Exception generalException)
                        {
                            logger.Error("Process : Error occurred while receiving sesssionId : " + generalException.ToString());
                        }
                    }

                    #endregion SessionId Region

                    #region TimeZone Region

                    if (filename.Equals("timezone"))
                    {
                        try
                        {
                            string timeZone = context.Request.QueryString["timezone"];
                            logger.Info("TimeZone received from Salesforce :" + timeZone);
                            if (!string.IsNullOrEmpty(timeZone))
                            {
                                timeZone = timeZone.Substring(1, timeZone.IndexOf(')') - 1);
                                if (CanGetTimeZoneFromSFDC)
                                {
                                    Settings.SFDCOptions.SFDCTimeZone = timeZone;
                                    logger.Info("TimeZone value taken as :" + timeZone);
                                }
                            }
                            context.Response.OutputStream.Flush();
                            context.Response.StatusCode = (int)HttpStatusCode.OK;
                            context.Response.OutputStream.Close();
                            return;
                        }
                        catch (Exception generalException)
                        {
                            logger.Error("Process : Error occurred while receiving TimeZone from SFDC : " + generalException.ToString());
                        }
                    }

                    #endregion TimeZone Region

                    #region ScriptError Region

                    if (filename.Equals("scripterror"))
                    {
                        try
                        {
                            logger.Error("Script Error Received from SFDC Script : " + context.Request.QueryString["log"]);
                            context.Response.OutputStream.Flush();
                            context.Response.StatusCode = (int)HttpStatusCode.OK;
                            context.Response.OutputStream.Close();
                            return;
                        }
                        catch (Exception generalException)
                        {
                            logger.Error("Process : Error occurred receiving script error : " + generalException.ToString());
                        }
                    }

                    #endregion ScriptError Region

                    #region SFDCConnection Opened

                    if (filename.Equals("opened"))
                    {
                        flagPrompt = true;
                        if (IsFirstRequestMade && Settings.SFDCOptions.NotifyAllConnectionStateChange)
                        {
                            Settings.SFDCListener.SendSessionStatus(SFDCSessionStatus.Connected);
                        }
                        connectionStatus = true;

                        if (IsFirstRequestMade)
                            this.logger.Info("Salesforce Connected with SFDC Adapter.....");
                        else
                            this.logger.Info("Adapter page loaded in Salesforce,SessionId is not verfied yet..");

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
                        this.logger.Warn("Salesforce DisConnected from SFDC Adapter.....");
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
                                if (sessionFlag)
                                {
                                    sessionFlag = false;

                                    string session = " { \"Event\":  " + "\"" + "sessionid" + "\"" + ", \"ANI\": " + "\"" + "test" + "\"" + " }";
                                    byte[] sessionResponse = ASCIIEncoding.ASCII.GetBytes(session);
                                    context.Response.OutputStream.Write(sessionResponse, 0, sessionResponse.Length);
                                    context.Response.OutputStream.Flush();
                                    context.Response.StatusCode = (int)HttpStatusCode.OK;
                                    context.Response.OutputStream.Close();
                                    return;
                                }
                                if (flagPrompt && Settings.SFDCOptions.CanEditDialNo)
                                {
                                    flagPrompt = false;
                                    string promptEvent = " { \"Event\":  " + "\"" + "init" + "\"" + ", \"EnablePrompt\": " + "\"" + "true" + "\"" + " }";
                                    byte[] promptResponse = ASCIIEncoding.ASCII.GetBytes(promptEvent);

                                    context.Response.OutputStream.Write(promptResponse, 0, promptResponse.Length);
                                    context.Response.OutputStream.Flush();
                                    context.Response.StatusCode = (int)HttpStatusCode.OK;
                                    context.Response.OutputStream.Close();
                                }
                                if (Settings.SFDCPopupData.Count > 0)
                                {
                                    foreach (string key in Settings.SFDCPopupData.Keys)
                                    {
                                        sfdcData = Settings.SFDCPopupData[key];
                                        Settings.SFDCPopupData.Remove(key);
                                        break;
                                    }
                                    try
                                    {
                                        jsonEvent = JsonConvert.SerializeObject(sfdcData);
                                        byte[] respons = ASCIIEncoding.ASCII.GetBytes(jsonEvent);
                                        context.Response.OutputStream.Write(respons, 0, respons.Length);
                                        context.Response.OutputStream.Flush();
                                        context.Response.StatusCode = (int)HttpStatusCode.OK;
                                        context.Response.OutputStream.Close();
                                        try
                                        {
                                            logger.Info("\n******Sending popup data to SalesForce ******" + sfdcData.ToString()
                                                + "\n***************************************");
                                        }
                                        catch (Exception)
                                        {
                                        }
                                    }
                                    catch (Exception generalException)
                                    {
                                        logger.Error("Error occurred while sending search and Activity Log data to salesforce : " + generalException.ToString());
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
                            logger.Warn("Process : Listener write operation can't be performed : " + listenerException.ToString());
                        }
                        catch (Exception generalException)
                        {
                            logger.Error("Process : Error occurred while sending json data to Salesforce  : " + generalException.ToString());
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
                            logger.Error("process() : Error occurred while executing start page file from process method  " + generalException.ToString());
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
                logger.Error("Process : Error occurred while processing request from Salesforce or AID : " + generalException.ToString());
            }
        }

        private void sessionKeepAliveTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                userinforesult = null;
                userinforesult = SFDCUtility.SForce.getUserInfo();
                if (userinforesult != null)
                {
                    this.logger.Info("Session keep alive check , Timeout in : " + userinforesult.sessionSecondsValid);
                }
                else
                    this.logger.Error("Session keep alive check returns null");
            }
            catch (Exception generalException)
            {
                this.logger.Error("TruncateNumbers : Error occurred while Truncating incoming data from Salesforce :" + generalException.ToString());
            }
        }

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
                this.logger.Error("TruncateNumbers : Error occurred while Truncating incoming data from Salesforce :" + generalException.ToString());
            }
            return string.Empty;
        }

        #endregion Methods
    }
}