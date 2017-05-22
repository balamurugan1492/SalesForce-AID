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
    internal class CallPartyChanged
    {
        #region Fields

        private static CallPartyChanged _partyChangedObject = null;
        private Log _logger = null;
        private SearchHandler _searchHandler = null;
        private SFDCData _sfdcPopupData = null;
        private VoiceManager _voiceEvents = null;

        #endregion Fields

        #region Constructor

        private CallPartyChanged()
        {
            this._logger = Log.GenInstance();
            this._voiceEvents = VoiceManager.GetInstance();
            this._searchHandler = SearchHandler.GetInstance();
        }

        #endregion Constructor

        #region GetInstance

        public static CallPartyChanged GetInstance()
        {
            if (_partyChangedObject == null)
            {
                _partyChangedObject = new CallPartyChanged();
            }
            return _partyChangedObject;
        }

        #endregion GetInstance

        #region PopupRecords

        /// <summary>
        /// Popup SFDC Records on EventPartyChanged Event
        /// </summary>
        /// <param name="eventPartyChanged"></param>
        /// <param name="callType"></param>
        public void PopupRecords(EventPartyChanged eventPartyChanged, SFDCCallType callType, string duration)
        {
            try
            {
                this._logger.Info("Trying to Popup or update sfdc on EventPartyChanged event for the " + callType.ToString() + " call");
                if (callType == SFDCCallType.ConsultVoiceReceived)
                {
                    _sfdcPopupData = this._voiceEvents.GetConsultReceivedPopupData(eventPartyChanged, callType, "partychanged");
                    if (_sfdcPopupData != null)
                    {
                        _sfdcPopupData.InteractionId = eventPartyChanged.ConnID.ToString();
                        this._searchHandler.ProcessSearchData(eventPartyChanged.ConnID.ToString(), _sfdcPopupData, callType);
                    }
                    else
                        this._logger.Info("Search data is empty");

                    //Update Data
                    this._logger.Info("Update SFDC for Consult Call");
                    _sfdcPopupData = this._voiceEvents.GetConsultUpdateData(eventPartyChanged, callType, duration, "partychanged");
                    if (_sfdcPopupData != null)
                    {
                        _sfdcPopupData.InteractionId = eventPartyChanged.ConnID.ToString();
                        this._searchHandler.ProcessUpdateData(eventPartyChanged.ConnID.ToString(), _sfdcPopupData);
                    }
                    else
                        this._logger.Info("Search data is empty");
                }
            }
            catch (Exception generalException)
            {
                _logger.Info("EventPartyChanged: Error Occurred  : " + generalException.ToString());
            }
        }

        #endregion PopupRecords
    }
}