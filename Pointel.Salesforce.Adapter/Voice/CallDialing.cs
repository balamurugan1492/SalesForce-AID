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
    internal class CallDialing
    {
        #region Fields

        private static CallDialing _dialingObject = null;
        private Log _logger = null;
        private SearchHandler _searchHandler = null;
        private SFDCData _sfdcPopupData = null;
        private VoiceManager _voiceEvents = null;

        #endregion Fields

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        private CallDialing()
        {
            this._logger = Log.GenInstance();
            this._voiceEvents = VoiceManager.GetInstance();
            this._searchHandler = SearchHandler.GetInstance();
        }

        #endregion Constructor

        #region GetInstance

        /// <summary>
        /// Gets the Instance of the Class
        /// </summary>
        /// <returns></returns>
        public static CallDialing GetInstance()
        {
            if (_dialingObject == null)
            {
                _dialingObject = new CallDialing();
            }
            return _dialingObject;
        }

        #endregion GetInstance

        #region PopupRecords

        /// <summary>
        /// Popup SFDC Records on EventDialing Event
        /// </summary>
        /// <param name="eventDialing"></param>
        /// <param name="callType"></param>
        public void PopupRecords(EventDialing eventDialing, SFDCCallType callType)
        {
            try
            {
                this._logger.Info("Trying to Popup sfdc on EventDialing event for the " + callType.ToString() + " call");
                if (callType == SFDCCallType.OutboundVoiceSuccess)
                {
                    if (Settings.ClickToDialData.ContainsKey(eventDialing.ConnID.ToString()))
                    {
                        this._voiceEvents.GetClickToDialLogs(eventDialing, eventDialing.ConnID.ToString(),
                            callType, Settings.ClickToDialData[eventDialing.ConnID.ToString()], "dialing");
                    }
                    else if (Settings.SFDCOptions.EnablePopupDialedFromDesktop)
                    {
                        _sfdcPopupData = this._voiceEvents.GetOutboundPopupData(eventDialing, callType, "dialing");
                        if (_sfdcPopupData != null)
                        {
                            _sfdcPopupData.InteractionId = eventDialing.ConnID.ToString();
                            this._searchHandler.ProcessSearchData(eventDialing.ConnID.ToString(), _sfdcPopupData, callType);
                        }
                        else
                            this._logger.Info("Search data is empty");
                    }
                }
            }
            catch (Exception generalException)
            {
                _logger.Info("EventDialing: Error Occurred  : " + generalException.ToString());
            }
        }

        #endregion PopupRecords
    }
}