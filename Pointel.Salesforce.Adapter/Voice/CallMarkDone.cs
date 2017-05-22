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

using Pointel.Salesforce.Adapter.LogMessage;
using Pointel.Salesforce.Adapter.SFDCUtils;
using Pointel.Salesforce.Adapter.Utility;
using System;

namespace Pointel.Salesforce.Adapter.Voice
{
    internal class CallMarkDone
    {
        #region Fields

        private static CallMarkDone _markdone = null;
        private Log _logger = null;
        private SearchHandler _searchHandler = null;
        private SFDCData _sfdcPopupData = null;
        private VoiceManager _voiceEvents = null;

        #endregion Fields

        #region Constructor

        private CallMarkDone()
        {
            this._logger = Log.GenInstance();
            this._voiceEvents = VoiceManager.GetInstance();
            this._searchHandler = SearchHandler.GetInstance();
        }

        #endregion Constructor

        #region GetInstance

        public static CallMarkDone GetInstance()
        {
            if (_markdone == null)
            {
                _markdone = new CallMarkDone();
            }
            return _markdone;
        }

        #endregion GetInstance

        #region Popup/Update Records

        /// <summary>
        /// Update SFDC Records on Call MarkDone Event
        /// </summary>
        /// <param name="eventReleased"></param>
        /// <param name="callType"></param>
        /// <param name="callDuration"></param>

        #endregion Popup/Update Records

        #region UpdateRecords on Markdone

        /// <summary>
        /// Update SFDC Records on EventReleased Event
        /// </summary>
        /// <param name="eventReleased"></param>
        /// <param name="callType"></param>
        /// <param name="callDuration"></param>
        public void UpdateRecords(IXNCustomData ixnData)
        {
            try
            {
                _logger.Info("CallMarkDone : Update activity log on markdone event ");
                if (ixnData.InteractionType == SFDCCallType.InboundVoice)
                    _sfdcPopupData = this._voiceEvents.GetInboundUpdateData(ixnData.InteractionEvent, ixnData.InteractionType, ixnData.Duration, "markdone", ixnData.InteractionNotes);
                else if (ixnData.InteractionType == SFDCCallType.ConsultVoiceReceived)
                    _sfdcPopupData = this._voiceEvents.GetConsultUpdateData(ixnData.InteractionEvent, ixnData.InteractionType, ixnData.Duration, "markdone", ixnData.InteractionNotes);
                else if (ixnData.InteractionType == SFDCCallType.OutboundVoiceSuccess)
                    _sfdcPopupData = this._voiceEvents.GetOutboundUpdateData(ixnData.InteractionEvent, ixnData.InteractionType, ixnData.Duration, "markdone", ixnData.InteractionNotes);
                else if (ixnData.InteractionType == SFDCCallType.OutboundVoiceFailure)
                    _sfdcPopupData = this._voiceEvents.GetOutboundUpdateData(ixnData.InteractionEvent, ixnData.InteractionType, ixnData.Duration, "markdone", ixnData.InteractionNotes);

                if (_sfdcPopupData != null)
                {
                    _sfdcPopupData.InteractionId = ixnData.InteractionId;
                    this._searchHandler.ProcessUpdateData(ixnData.InteractionId, _sfdcPopupData);
                }
                else
                    this._logger.Info("Search data is empty");
            }
            catch (Exception generalException)
            {
                _logger.Error("CallMarkDone : Error Occurred  : " + generalException.ToString());
            }
        }

        #endregion UpdateRecords on Markdone
    }
}