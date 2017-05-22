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
    internal class CallUserEvent
    {
        #region Fields

        private static CallUserEvent _userEventObject = null;
        private Log _logger = null;
        private SearchHandler _searchHandler = null;
        private SFDCData _sfdcPopupData = null;
        private VoiceManager _voiceEvents = null;

        #endregion Fields

        #region Constructor

        private CallUserEvent()
        {
            this._logger = Log.GenInstance();
            this._voiceEvents = VoiceManager.GetInstance();
            this._searchHandler = SearchHandler.GetInstance();
        }

        #endregion Constructor

        #region GetInstance

        public static CallUserEvent GetInstance()
        {
            if (_userEventObject == null)
            {
                _userEventObject = new CallUserEvent();
            }
            return _userEventObject;
        }

        #endregion GetInstance

        #region UpdateRecords

        /// <summary>
        /// </summary>
        /// <param name="eventUserEvent"></param>
        /// <param name="callType"></param>
        /// <param name="callDuration"></param>
        public void UpdateRecords(EventUserEvent eventUserEvent, SFDCCallType callType, string callDuration)
        {
            try
            {
                this._logger.Info("Trying to update sfdc on EventUserEvent event for the " + callType.ToString() + " call");
                if (callType == SFDCCallType.InboundVoice)
                    _sfdcPopupData = this._voiceEvents.GetInboundUpdateData(eventUserEvent, callType, callDuration, "userevent");
                else if (callType == SFDCCallType.OutboundVoiceSuccess)
                    _sfdcPopupData = this._voiceEvents.GetOutboundUpdateData(eventUserEvent, callType, callDuration, "userevent");
                else if (callType == SFDCCallType.ConsultVoiceReceived)
                    _sfdcPopupData = this._voiceEvents.GetConsultUpdateData(eventUserEvent, callType, callDuration, "userevent");

                if (_sfdcPopupData != null)
                {
                    _sfdcPopupData.InteractionId = eventUserEvent.ConnID.ToString();
                    this._searchHandler.ProcessUpdateData(eventUserEvent.ConnID.ToString(), _sfdcPopupData);
                }
                else
                    this._logger.Info("Search data is empty");
            }
            catch (Exception generalException)
            {
                _logger.Info("CallUserEvent : Error Occurred  : " + generalException.ToString());
            }
        }

        #endregion UpdateRecords
    }
}