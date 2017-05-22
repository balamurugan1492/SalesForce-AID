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

using Genesyslab.Platform.Voice.Protocols.TServer.Events;
using Pointel.Salesforce.Adapter.LogMessage;
using Pointel.Salesforce.Adapter.SFDCUtils;
using Pointel.Salesforce.Adapter.Utility;
using System;

namespace Pointel.Salesforce.Adapter.Voice
{
    /// <summary>
    /// Comment: Provides Inbound Voice Call SFDC Popup
    /// Last Modified: 25-08-2015
    /// Created by: Pointel Inc
    /// </summary>
    internal class CallRinging
    {
        #region Fields

        private Log logger = null;
        private static CallRinging ringingObject = null;
        private SFDCData sfdcPopupData = null;
        private VoiceEvents voiceEvents = null;

        #endregion Fields

        #region Constructor

        private CallRinging()
        {
            this.logger = Log.GenInstance();
            this.voiceEvents = VoiceEvents.GetInstance();
        }

        #endregion Constructor

        #region GetInstance

        public static CallRinging GetInstance()
        {
            if (ringingObject == null)
            {
                ringingObject = new CallRinging();
            }
            return ringingObject;
        }

        #endregion GetInstance

        #region PopupRecords

        /// <summary>
        /// Popup Records on EventRinging Event
        /// </summary>
        /// <param name="eventRinging"></param>
        /// <param name="callType"></param>
        public void PopupRecords(EventRinging eventRinging, SFDCCallType callType)
        {
            try
            {
                this.logger.Info("PopupRecords : CallRing.PopupRecords method Invoke");
                if (callType == SFDCCallType.Inbound)
                {
                    this.logger.Info("Popup SFDC for Inbound Call...");
                    //Collect Data for Activity History
                    sfdcPopupData = this.voiceEvents.GetInboundPopupData(eventRinging, callType, "ringing");
                    if (sfdcPopupData != null)
                    {
                        sfdcPopupData.InteractionId = eventRinging.ConnID.ToString();
                        this.voiceEvents.ProcessSearchData(eventRinging.ConnID.ToString(), sfdcPopupData, callType);
                    }
                    else
                        this.logger.Info("Search data is empty");
                }
                else if (callType == SFDCCallType.ConsultReceived)
                {
                    this.logger.Info("Popup SFDC for Consult call ");
                    sfdcPopupData = this.voiceEvents.GetConsultReceivedPopupData(eventRinging, callType, "ringing");
                    if (sfdcPopupData != null)
                    {
                        sfdcPopupData.InteractionId = eventRinging.ConnID.ToString();
                        this.voiceEvents.ProcessSearchData(eventRinging.ConnID.ToString(), sfdcPopupData, callType);
                    }
                    else
                        this.logger.Info("Search data is empty");
                }
            }
            catch (Exception generalException)
            {
                logger.Info("PopupRecords : Error Occurred  : " + generalException.ToString());
            }
        }

        #endregion PopupRecords
    }
}