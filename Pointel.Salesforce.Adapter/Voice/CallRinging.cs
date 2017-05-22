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
    /// Comment: Provides Inbound Voice Call SFDC Popup Last Modified: 25-08-2015 Created by: Pointel Inc
    /// </summary>
    internal class CallRinging
    {
        #region Fields

        private static CallRinging _ringingObject = null;
        private Log _logger = null;
        private SearchHandler _searchHandler = null;
        private SFDCData _sfdcPopupData = null;
        private VoiceManager _voiceEvents = null;

        #endregion Fields

        #region Constructor

        private CallRinging()
        {
            this._logger = Log.GenInstance();
            this._voiceEvents = VoiceManager.GetInstance();
            this._searchHandler = SearchHandler.GetInstance();
        }

        #endregion Constructor

        #region GetInstance

        public static CallRinging GetInstance()
        {
            if (_ringingObject == null)
            {
                _ringingObject = new CallRinging();
            }
            return _ringingObject;
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
                this._logger.Info("Trying to Popup sfdc on EventRinging event for the " + callType.ToString() + " call");

                if (callType == SFDCCallType.InboundVoice)
                    _sfdcPopupData = this._voiceEvents.GetInboundPopupData(eventRinging, callType, "ringing");
                else if (callType == SFDCCallType.ConsultVoiceReceived)
                    _sfdcPopupData = this._voiceEvents.GetConsultReceivedPopupData(eventRinging, callType, "ringing");

                if (_sfdcPopupData != null)
                {
                    _sfdcPopupData.InteractionId = eventRinging.ConnID.ToString();
                    this._searchHandler.ProcessSearchData(eventRinging.ConnID.ToString(), _sfdcPopupData, callType);
                }
                else
                    this._logger.Info("Search data is empty");
            }
            catch (Exception generalException)
            {
                _logger.Info("EventRinging : Error Occurred  : " + generalException.ToString());
            }
        }

        #endregion PopupRecords
    }
}