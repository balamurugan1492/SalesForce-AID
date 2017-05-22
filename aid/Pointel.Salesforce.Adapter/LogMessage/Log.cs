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
    /// Comment: Writes Logs on Specified mode
    /// Last Modified: 25-08-2015
    /// Created by: Pointel Inc
    /// </summary>
    internal class Log
    {
        #region Fields

        private static Log thisLogger = null;
        private ILogger Genlogger = null;
        private ILog log4netLogger = null;
        private bool SendLogs = false;
        private ISFDCListener SFDCListener = null;

        #endregion Fields

        #region Log

        private Log()
        {
        }

        #endregion Log

        #region Logger.Error

        public void Error(string message)
        {
            if (Genlogger != null)
            {
                Genlogger.Error(message);
            }
            else if (SendLogs)
            {
                SFDCListener.WriteLogMessage(message, LogMode.Error);
            }
            else if (log4netLogger != null)
            {
                log4netLogger.Error(message);
            }
        }

        #endregion Logger.Error

        #region Logger.Info

        public void Info(string message)
        {
            if (Genlogger != null)
            {
                Genlogger.Info(message);
            }
            else if (SendLogs)
            {
                SFDCListener.WriteLogMessage(message, LogMode.Info);
            }
            else if (log4netLogger != null)
            {
                log4netLogger.Info(message);
            }
        }

        #endregion Logger.Info

        #region Logger.Warn

        public void Warn(string message)
        {
            if (Genlogger != null)
            {
                Genlogger.Warn(message);
            }
            else if (SendLogs)
            {
                SFDCListener.WriteLogMessage(message, LogMode.Warn);
            }
            else if (log4netLogger != null)
            {
                log4netLogger.Warn(message);
            }
        }

        #endregion Logger.Warn

        #region Logger.Debug

        public void Debug(string message)
        {
            if (Genlogger != null)
            {
                Genlogger.Debug(message);
            }
            else if (SendLogs)
            {
                SFDCListener.WriteLogMessage(message, LogMode.Debug);
            }
            else if (log4netLogger != null)
            {
                log4netLogger.Debug(message);
            }
        }

        #endregion Logger.Debug

        #region GetInstance

        public static Log GenInstance()
        {
            if (thisLogger == null)
            {
                thisLogger = new Log();
            }
            return thisLogger;
        }

        #endregion GetInstance

        #region CreateLogger

        public Log CreateLogger(ISFDCListener listener, ILogger genLoggger)
        {
            this.SFDCListener = listener;
            this.Genlogger = genLoggger.CreateChildLogger("SFDCAdapter");
            return thisLogger;
        }

        #endregion CreateLogger

        #region CreateLogger

        public Log CreateLogger(ISFDCListener listener, ILog log4net)
        {
            this.SFDCListener = listener;
            this.log4netLogger = log4net;
            return thisLogger;
        }

        #endregion CreateLogger

        #region CreateLogger

        public Log CreateLogger(ISFDCListener listener, bool sendLogsToSubscriber)
        {
            this.SFDCListener = listener;
            this.SendLogs = sendLogsToSubscriber;
            return thisLogger;
        }

        #endregion CreateLogger
    }
}