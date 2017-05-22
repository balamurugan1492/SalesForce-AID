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
    internal class CallEventAbandoned
    {
        #region Fields

        private static CallEventAbandoned _abandonedObject = null;
        private Log _logger = null;
        private SearchHandler _searchHandler = null;
        private SFDCData _sfdcPopupData = null;
        private VoiceManager _voiceEvents = null;

        #endregion Fields

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        private CallEventAbandoned()
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
        public static CallEventAbandoned GetInstance()
        {
            if (_abandonedObject == null)
            {
                _abandonedObject = new CallEventAbandoned();
            }
            return _abandonedObject;
        }

        #endregion GetInstance

        #region PopupRecords

        /// <summary>
        /// Popup SFDC Records on EventAbandoned Event
        /// </summary>
        /// <param name="eventAbandoned"></param>
        /// <param name="callType"></param>
        public void PopupRecords(EventAbandoned eventAbandoned, SFDCCallType callType)
        {
            try
            {
                this._logger.Info("CallEventAbandoned PopupRecords method Invoked for the " + callType.ToString() + " call");
                if (Settings.ClickToDialData.ContainsKey(eventAbandoned.ConnID.ToString()))
                {
                    this._voiceEvents.GetClickToDialLogs(eventAbandoned, eventAbandoned.ConnID.ToString(),
                        callType, Settings.ClickToDialData[eventAbandoned.ConnID.ToString()], "not reachable");
                }
                else if (Settings.SFDCOptions.EnablePopupDialedFromDesktop)
                {
                    if (callType == SFDCCallType.OutboundVoiceFailure)
                    {
                        _sfdcPopupData = this._voiceEvents.GetOutboundFailurePopupData(eventAbandoned, callType, "not reachable");
                        if (_sfdcPopupData != null)
                        {
                            _sfdcPopupData.InteractionId = eventAbandoned.ConnID.ToString();
                            this._searchHandler.ProcessSearchData(eventAbandoned.ConnID.ToString(), _sfdcPopupData, callType);
                        }
                        else
                            this._logger.Info("Search data is empty");
                    }
                }
            }
            catch (Exception generalException)
            {
                _logger.Info("PopupRecords : Error Occurred  : " + generalException.ToString());
            }
        }

        #endregion PopupRecords
    }
}