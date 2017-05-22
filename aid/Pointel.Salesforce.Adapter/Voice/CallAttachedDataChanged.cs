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
    internal class CallAttachedDataChanged
    {
        #region Fields

        private Log logger = null;
        private static CallAttachedDataChanged userEventObject = null;
        private SFDCData sfdcPopupData = null;
        private VoiceEvents voiceEvents = null;

        #endregion Fields

        #region Constructor

        private CallAttachedDataChanged()
        {
            this.logger = Log.GenInstance();
            this.voiceEvents = VoiceEvents.GetInstance();
        }

        #endregion Constructor

        #region GetInstance

        public static CallAttachedDataChanged GetInstance()
        {
            if (userEventObject == null)
            {
                userEventObject = new CallAttachedDataChanged();
            }
            return userEventObject;
        }

        #endregion GetInstance

        #region UpdateRecords

        /// <summary>
        /// Updates SDFC Records on EventAttachedDataChanged Event
        /// </summary>
        /// <param name="datachanged"></param>
        /// <param name="callType"></param>
        /// <param name="callDuration"></param>
        public void UpdateRecords(EventAttachedDataChanged datachanged, SFDCCallType callType, string callDuration)
        {
            try
            {
                logger.Info("UpdateRecords : CallAttachedDataChanged.UpdateRecords method Invoke");
                if (callType == SFDCCallType.Inbound)
                {
                    //Update Data
                    this.logger.Info("Update SFDC Logs for Inbound Call");
                    sfdcPopupData = this.voiceEvents.GetInboundUpdateData(datachanged, callType, callDuration, "datachanged");
                    if (sfdcPopupData != null)
                    {
                        sfdcPopupData.InteractionId = datachanged.ConnID.ToString();
                        this.voiceEvents.ProcessUpdateData(datachanged.ConnID.ToString(), sfdcPopupData);
                    }
                    else
                        this.logger.Info("Search data is empty");
                }
                else if (callType == SFDCCallType.OutboundSuccess)
                {
                    //Update Data
                    this.logger.Info("Update SFDC Logs for Outbound Call");
                    sfdcPopupData = this.voiceEvents.GetOutboundUpdateData(datachanged, callType, callDuration, "datachanged");
                    if (sfdcPopupData != null)
                    {
                        sfdcPopupData.InteractionId = datachanged.ConnID.ToString();
                        this.voiceEvents.ProcessUpdateData(datachanged.ConnID.ToString(), sfdcPopupData);
                    }
                    else
                        this.logger.Info("Search data is empty");
                }
                else if (callType == SFDCCallType.ConsultSuccess || callType == SFDCCallType.ConsultReceived)
                {
                    //Update Data
                    this.logger.Info("Update SFDC Logs for Consult Call");
                    sfdcPopupData = this.voiceEvents.GetConsultUpdateData(datachanged, callType, callDuration, "datachanged");
                    if (sfdcPopupData != null)
                    {
                        sfdcPopupData.InteractionId = datachanged.ConnID.ToString();
                        this.voiceEvents.ProcessUpdateData(datachanged.ConnID.ToString(), sfdcPopupData);
                    }
                    else
                        this.logger.Info("Search data is empty");
                }
            }
            catch (Exception generalException)
            {
                logger.Info("UpdateRecords : Error Occurred  : " + generalException.ToString());
            }
        }

        #endregion UpdateRecords
    }
}