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

using Genesyslab.Platform.Commons.Logging;
using log4net;

namespace Pointel.Salesforce.Adapter.LogMessage
{
    /// <summary>
    /// Comment: Writes Logs on Specified mode Last Modified: 25-08-2015 Created by: Pointel Inc
    /// </summary>
    internal class Log
    {
        #region Fields

        private static Log _thisLogger = null;
        private ILogger _genlogger = null;
        private ILog _log4netLogger = null;
        private bool _sendLogs = false;
        private ISFDCListener _sFDCListener = null;

        #endregion Fields

        #region Log

        private Log()
        {
        }

        #endregion Log

        #region Logger.Error

        public void Error(string message)
        {
            if (_genlogger != null)
            {
                _genlogger.Error(message);
            }
            else if (_sendLogs)
            {
                _sFDCListener.WriteLogMessage(message, LogMode.Error);
            }
            else if (_log4netLogger != null)
            {
                _log4netLogger.Error(message);
            }
        }

        #endregion Logger.Error

        #region Logger.Info

        public void Info(string message)
        {
            if (_genlogger != null)
            {
                _genlogger.Info(message);
            }
            else if (_sendLogs)
            {
                _sFDCListener.WriteLogMessage(message, LogMode.Info);
            }
            else if (_log4netLogger != null)
            {
                _log4netLogger.Info(message);
            }
        }

        #endregion Logger.Info

        #region Logger.Warn

        public void Warn(string message)
        {
            if (_genlogger != null)
            {
                _genlogger.Warn(message);
            }
            else if (_sendLogs)
            {
                _sFDCListener.WriteLogMessage(message, LogMode.Warn);
            }
            else if (_log4netLogger != null)
            {
                _log4netLogger.Warn(message);
            }
        }

        #endregion Logger.Warn

        #region Logger.Debug

        public void Debug(string message)
        {
            if (_genlogger != null)
            {
                _genlogger.Debug(message);
            }
            else if (_sendLogs)
            {
                _sFDCListener.WriteLogMessage(message, LogMode.Debug);
            }
            else if (_log4netLogger != null)
            {
                _log4netLogger.Debug(message);
            }
        }

        #endregion Logger.Debug

        #region GetInstance

        public static Log GenInstance()
        {
            if (_thisLogger == null)
            {
                _thisLogger = new Log();
            }
            return _thisLogger;
        }

        #endregion GetInstance

        #region CreateLogger

        public Log CreateLogger(ISFDCListener listener, ILogger genLoggger)
        {
            this._sFDCListener = listener;
            this._genlogger = genLoggger.CreateChildLogger("SFDCAdapter");
            return _thisLogger;
        }

        #endregion CreateLogger

        #region CreateLogger

        public Log CreateLogger(ISFDCListener listener, ILog log4net)
        {
            this._sFDCListener = listener;
            this._log4netLogger = log4net;
            return _thisLogger;
        }

        #endregion CreateLogger

        #region CreateLogger

        public Log CreateLogger(ISFDCListener listener, bool sendLogsToSubscriber)
        {
            this._sFDCListener = listener;
            this._sendLogs = sendLogsToSubscriber;
            return _thisLogger;
        }

        #endregion CreateLogger
    }
}