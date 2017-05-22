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

        private static CallEventDestinationBusy _destinationBusyObject = null;
        private Log _logger = null;
        private SearchHandler _searchHandler = null;
        private SFDCData _sfdcPopupData = null;
        private VoiceManager _voiceEvents = null;

        #endregion Fields

        #region Constructor

        private CallEventDestinationBusy()
        {
            this._logger = Log.GenInstance();
            this._voiceEvents = VoiceManager.GetInstance();
            this._searchHandler = SearchHandler.GetInstance();
        }

        #endregion Constructor

        #region GetInstance

        public static CallEventDestinationBusy GetInstance()
        {
            if (_destinationBusyObject == null)
            {
                _destinationBusyObject = new CallEventDestinationBusy();
            }
            return _destinationBusyObject;
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
                this._logger.Info("Trying to Popup sfdc on EventDestinationBusy event for the " + callType.ToString() + " call");
                if (Settings.ClickToDialData.ContainsKey(eventDestinationBusy.ConnID.ToString()))
                {
                    this._voiceEvents.GetClickToDialLogs(eventDestinationBusy, eventDestinationBusy.ConnID.ToString(),
                        callType, Settings.ClickToDialData[eventDestinationBusy.ConnID.ToString()], "busy");
                }
                else if (Settings.SFDCOptions.EnablePopupDialedFromDesktop)
                {
                    if (callType == SFDCCallType.OutboundVoiceFailure)
                    {
                        _sfdcPopupData = this._voiceEvents.GetOutboundFailurePopupData(eventDestinationBusy, callType, "busy");
                        if (_sfdcPopupData != null)
                        {
                            _sfdcPopupData.InteractionId = eventDestinationBusy.ConnID.ToString();
                            this._searchHandler.ProcessSearchData(eventDestinationBusy.ConnID.ToString(), _sfdcPopupData, callType);
                        }
                        else
                            this._logger.Info("Search data is empty");
                    }
                }
            }
            catch (Exception generalException)
            {
                _logger.Info("EventDestinationBusy : Error Occurred  : " + generalException.ToString());
            }
        }

        #endregion PopupRecords
    }
}