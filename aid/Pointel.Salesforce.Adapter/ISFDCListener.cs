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

using Pointel.Salesforce.Adapter.LogMessage;
using System.Collections.Generic;
using System.Windows.Controls;

namespace Pointel.Salesforce.Adapter
{
    /// <summary>
    /// Comment: Enables the third party application to connect with SFDCAdapter
    /// Last Modified: 25-08-2015
    /// Created by: Pointel Inc
    /// </summary>

    public interface ISFDCListener
    {
        /// <summary>
        /// Send SFDC Connection Status Message to Subscriber (AID/WDE/iWS/any 3rd party applications)
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="message"></param>
        void SFDCConnectionStatus(LogMode mode, string message);

        /// <summary>
        /// Sends the session status.
        /// </summary>
        /// <param name="sessionStatus">The session status.</param>
        void SendSessionStatus(SFDCSessionStatus sessionStatus);

        /// <summary>
        ///  Send SFDC Log Messages to Subscriber (AID/WDE/iWS/any 3rd party applications)
        /// </summary>
        /// <param name="message"></param>
        /// <param name="mode"></param>
        void WriteLogMessage(string message, LogMode mode);

        /// <summary>
        /// Receives the SFDC UserControl
        /// </summary>
        /// <param name="SFDCBrowserWindow"></param>
        void ReceiveSFDCWindow(UserControl SFDCBrowserWindow);

        bool IsSFDCConnected
        {
            get;
            set;
        }

        IDictionary<string, string> ConsultResponseIds
        {
            get;
        }
    }
}