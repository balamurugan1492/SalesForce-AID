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
    internal class CallRejected
    {
        #region Fields

        private Log logger = null;
        private static CallRejected callRejected = null;
        private SFDCData sfdcPopupData = null;
        private VoiceEvents voiceEvents = null;

        #endregion Fields

        #region Constructor

        private CallRejected()
        {
            this.logger = Log.GenInstance();
            this.voiceEvents = VoiceEvents.GetInstance();
        }

        #endregion Constructor

        #region GetInstance

        public static CallRejected GetInstance()
        {
            if (callRejected == null)
            {
                callRejected = new CallRejected();
            }
            return callRejected;
        }

        #endregion GetInstance

        #region Popup/Update Records

        /// <summary>
        /// Popup and Update SFDC Records on EventReleased Event
        /// </summary>
        /// <param name="eventReleased"></param>
        /// <param name="callType"></param>
        /// <param name="callDuration"></param>
        public void PopupRecords(EventReleased eventReleased, SFDCCallType callType, string callDuration)
        {
            try
            {
                if (callType == SFDCCallType.Inbound)
                {
                    this.logger.Info("PopupRecords : CallRejected.PopupRecords method Invoke");
                    //Update Data
                    this.logger.Info("Update SFDC records for Inbound Call");
                    sfdcPopupData = this.voiceEvents.GetInboundUpdateData(eventReleased, callType, callDuration, "rejected");
                    if (sfdcPopupData != null)
                    {
                        sfdcPopupData.InteractionId = eventReleased.ConnID.ToString();
                        this.voiceEvents.ProcessUpdateData(eventReleased.ConnID.ToString(), sfdcPopupData);
                    }
                    else
                        this.logger.Info("Search data is empty");
                }
                else if (callType == SFDCCallType.OutboundSuccess)
                {
                    //Update Data
                    this.logger.Info("Update SFDC records for Outbound Call");
                    sfdcPopupData = this.voiceEvents.GetOutboundUpdateData(eventReleased, callType, callDuration, "rejected");
                    if (sfdcPopupData != null)
                    {
                        sfdcPopupData.InteractionId = eventReleased.ConnID.ToString();
                        this.voiceEvents.ProcessUpdateData(eventReleased.ConnID.ToString(), sfdcPopupData);
                    }
                    else
                        this.logger.Info("Search data is empty");
                }
                else if (callType == SFDCCallType.ConsultSuccess)
                {
                    //Update Data
                    this.logger.Info("Update SFDC for Consult Call Received");
                    sfdcPopupData = this.voiceEvents.GetConsultUpdateData(eventReleased, callType, callDuration, "rejected");
                    if (sfdcPopupData != null)
                    {
                        sfdcPopupData.InteractionId = eventReleased.ConnID.ToString();
                        this.voiceEvents.ProcessUpdateData(eventReleased.ConnID.ToString(), sfdcPopupData);
                    }
                    else
                        this.logger.Info("Search data is empty");
                }
                else if (callType == SFDCCallType.ConsultReceived)
                {
                    //Update Data
                    this.logger.Info("Update SFDC for Consult Call Received");
                    sfdcPopupData = this.voiceEvents.GetConsultUpdateData(eventReleased, callType, callDuration, "rejected");
                    if (sfdcPopupData != null)
                    {
                        sfdcPopupData.InteractionId = eventReleased.ConnID.ToString();
                        this.voiceEvents.ProcessUpdateData(eventReleased.ConnID.ToString(), sfdcPopupData);
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

        #endregion Popup/Update Records
    }
}