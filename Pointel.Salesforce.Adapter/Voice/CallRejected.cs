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

        private static CallRejected _callRejected = null;
        private Log _logger = null;
        private SearchHandler _searchHandler = null;
        private SFDCData _sfdcPopupData = null;
        private VoiceManager _voiceEvents = null;

        #endregion Fields

        #region Constructor

        private CallRejected()
        {
            this._logger = Log.GenInstance();
            this._voiceEvents = VoiceManager.GetInstance();
            this._searchHandler = SearchHandler.GetInstance();
        }

        #endregion Constructor

        #region GetInstance

        public static CallRejected GetInstance()
        {
            if (_callRejected == null)
            {
                _callRejected = new CallRejected();
            }
            return _callRejected;
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
                this._logger.Info("Trying to update activity log on call rejected event for the calltype: " + callType.ToString() + "\t call duration: " + callDuration);
                if (callType == SFDCCallType.InboundVoice)
                    _sfdcPopupData = this._voiceEvents.GetInboundUpdateData(eventReleased, callType, callDuration, "rejected");
                else if (callType == SFDCCallType.OutboundVoiceSuccess)
                    _sfdcPopupData = this._voiceEvents.GetOutboundUpdateData(eventReleased, callType, callDuration, "rejected");
                else if (callType == SFDCCallType.ConsultVoiceReceived)
                    _sfdcPopupData = this._voiceEvents.GetConsultUpdateData(eventReleased, callType, callDuration, "rejected");

                if (_sfdcPopupData != null)
                {
                    _sfdcPopupData.InteractionId = eventReleased.ConnID.ToString();
                    this._searchHandler.ProcessUpdateData(eventReleased.ConnID.ToString(), _sfdcPopupData);
                }
                else
                    this._logger.Info("Search data is empty");
            }
            catch (Exception generalException)
            {
                _logger.Info("CallRejected: Error Occurred  : " + generalException.ToString());
            }
        }

        #endregion Popup/Update Records
    }
}