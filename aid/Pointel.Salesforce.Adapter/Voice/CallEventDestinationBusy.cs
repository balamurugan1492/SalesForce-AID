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
    internal class CallEventDestinationBusy
    {
        #region Fields

        private Log logger = null;
        private static CallEventDestinationBusy destinationBusyObject = null;
        private SFDCData sfdcPopupData = null;
        private VoiceEvents voiceEvents = null;

        #endregion Fields

        #region Constructor

        private CallEventDestinationBusy()
        {
            this.logger = Log.GenInstance();
            this.voiceEvents = VoiceEvents.GetInstance();
        }

        #endregion Constructor

        #region GetInstance

        public static CallEventDestinationBusy GetInstance()
        {
            if (destinationBusyObject == null)
            {
                destinationBusyObject = new CallEventDestinationBusy();
            }
            return destinationBusyObject;
        }

        #endregion GetInstance

        #region PopupRecords

        /// <summary>
        /// Popup SFDC Records on EventDestinationBusy Event
        /// </summary>
        /// <param name="eventDestinationBusy"></param>
        /// <param name="callType"></param>
        public void PopupRecords(EventDestinationBusy eventDestinationBusy, SFDCCallType callType)
        {
            try
            {
                this.logger.Info("PopupRecords : CallEventDestinationBusy.PopupRecords method Invoke");
                if (Settings.ClickToDialData.ContainsKey(eventDestinationBusy.ConnID.ToString()))
                {
                    this.voiceEvents.GetClickToDialLogs(eventDestinationBusy, eventDestinationBusy.ConnID.ToString(),
                        callType, Settings.ClickToDialData[eventDestinationBusy.ConnID.ToString()], "busy");
                }
                else if (Settings.SFDCOptions.EnablePopupDialedFromDesktop)
                {
                    if (callType == SFDCCallType.OutboundFailure)
                    {
                        this.logger.Info("Popup SFDC for Outbound Call");
                        sfdcPopupData = this.voiceEvents.GetOutboundFailurePopupData(eventDestinationBusy, callType, "busy");
                        if (sfdcPopupData != null)
                        {
                            sfdcPopupData.InteractionId = eventDestinationBusy.ConnID.ToString();
                            this.voiceEvents.ProcessSearchData(eventDestinationBusy.ConnID.ToString(), sfdcPopupData, callType);
                        }
                        else
                            this.logger.Info("Search data is empty");
                    }
                    else if (callType == SFDCCallType.ConsultFailure)
                    {
                        this.logger.Info("Popup SFDC for Consult Call");
                        sfdcPopupData = this.voiceEvents.GetConsultFailurePopupData(eventDestinationBusy, callType, "busy");
                        if (sfdcPopupData != null)
                        {
                            sfdcPopupData.InteractionId = eventDestinationBusy.ConnID.ToString();
                            this.voiceEvents.ProcessSearchData(eventDestinationBusy.ConnID.ToString(), sfdcPopupData, callType);
                        }
                        else
                            this.logger.Info("Search data is empty");
                    }
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