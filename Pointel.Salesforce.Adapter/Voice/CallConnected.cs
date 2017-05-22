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
    internal class CallConnected
    {
        #region Fields

        private static CallConnected _connectedObject = null;
        private Log _logger = null;
        private SearchHandler _searchHandler = null;
        private SFDCData _sfdcPopupData = null;
        private VoiceManager _voiceEvents = null;

        #endregion Fields

        #region Constructor

        private CallConnected()
        {
            this._logger = Log.GenInstance();
            this._voiceEvents = VoiceManager.GetInstance();
            _searchHandler = SearchHandler.GetInstance();
        }

        #endregion Constructor

        #region GetInstance

        public static CallConnected GetInstance()
        {
            if (_connectedObject == null)
            {
                _connectedObject = new CallConnected();
            }
            return _connectedObject;
        }

        #endregion GetInstance

        #region PopupRecords

        /// <summary>
        /// Popup SFDC Records on EventEstablished Event
        /// </summary>
        /// <param name="eventEstablished"></param>
        /// <param name="callType"></param>
        public void PopupRecords(EventEstablished eventEstablished, SFDCCallType callType)
        {
            try
            {
                this._logger.Info("Trying to Popup sfdc on EventEstablished event for the " + callType.ToString() + " call");
                if (callType == SFDCCallType.InboundVoice)
                    _sfdcPopupData = this._voiceEvents.GetInboundPopupData(eventEstablished, callType, "established");
                else if (callType == SFDCCallType.ConsultVoiceReceived)
                    _sfdcPopupData = this._voiceEvents.GetConsultReceivedPopupData(eventEstablished, callType, "established");
                else if (callType == SFDCCallType.OutboundVoiceSuccess)
                {
                    if (Settings.ClickToDialData.ContainsKey(eventEstablished.ConnID.ToString()))
                    {
                        this._logger.Info("Trying to popup Click 2 Dial call on EventEstablished event...");
                        this._voiceEvents.GetClickToDialLogs(eventEstablished, eventEstablished.ConnID.ToString(),
                            callType, Settings.ClickToDialData[eventEstablished.ConnID.ToString()], "established");
                    }
                    else if (Settings.SFDCOptions.EnablePopupDialedFromDesktop)
                        _sfdcPopupData = this._voiceEvents.GetOutboundPopupData(eventEstablished, callType, "established");
                }
                if (_sfdcPopupData != null)
                {
                    _sfdcPopupData.InteractionId = eventEstablished.ConnID.ToString();
                    this._searchHandler.ProcessSearchData(eventEstablished.ConnID.ToString(), _sfdcPopupData, callType);
                }
                else
                    this._logger.Info("Search data is empty");
            }
            catch (Exception generalException)
            {
                _logger.Info("EventEstablished : Error Occurred  : " + generalException.ToString());
            }
        }

        #endregion PopupRecords
    }
}