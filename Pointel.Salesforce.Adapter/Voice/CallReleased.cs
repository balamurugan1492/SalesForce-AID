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
    internal class CallReleased
    {
        #region Fields

        private static CallReleased _releasedObject = null;
        private Log _logger = null;
        private SearchHandler _searchHandler = null;
        private SFDCData _sfdcPopupData = null;
        private VoiceManager _voiceEvents = null;

        #endregion Fields

        #region Constructor

        private CallReleased()
        {
            this._logger = Log.GenInstance();
            this._voiceEvents = VoiceManager.GetInstance();
            this._searchHandler = SearchHandler.GetInstance();
        }

        #endregion Constructor

        #region GetInstance

        public static CallReleased GetInstance()
        {
            if (_releasedObject == null)
            {
                _releasedObject = new CallReleased();
            }
            return _releasedObject;
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
                this._logger.Info("Trying to Popup or update on eventreleased event for the " + callType.ToString() + "\t call duration: " + callDuration);
                string ixnNotes = Settings.SFDCListener.GetVoiceComments(eventReleased.ConnID.ToString());
                if (callType == SFDCCallType.InboundVoice)
                {
                    _sfdcPopupData = this._voiceEvents.GetInboundPopupData(eventReleased, callType, "released");
                    if (_sfdcPopupData != null)
                    {
                        _sfdcPopupData.InteractionId = eventReleased.ConnID.ToString();
                        this._searchHandler.ProcessSearchData(eventReleased.ConnID.ToString(), _sfdcPopupData, callType);
                    }
                    else
                        this._logger.Info("Search data is empty");

                    //Update activity log on released event
                    _sfdcPopupData = this._voiceEvents.GetInboundUpdateData(eventReleased, callType, callDuration, "released", ixnNotes);
                    if (_sfdcPopupData != null)
                    {
                        _sfdcPopupData.InteractionId = eventReleased.ConnID.ToString();
                        this._searchHandler.ProcessUpdateData(eventReleased.ConnID.ToString(), _sfdcPopupData);
                    }
                    else
                        this._logger.Info("Search data is empty");
                }
                else if (callType == SFDCCallType.OutboundVoiceSuccess)
                {
                    if (Settings.ClickToDialData.ContainsKey(eventReleased.ConnID.ToString()))
                    {
                        this._voiceEvents.GetClickToDialLogs(eventReleased, eventReleased.ConnID.ToString(),
                            callType, Settings.ClickToDialData[eventReleased.ConnID.ToString()], "released");
                    }
                    else if (Settings.SFDCOptions.EnablePopupDialedFromDesktop)
                    {
                        _sfdcPopupData = this._voiceEvents.GetOutboundPopupData(eventReleased, callType, "released");
                        if (_sfdcPopupData != null)
                        {
                            _sfdcPopupData.InteractionId = eventReleased.ConnID.ToString();
                            this._searchHandler.ProcessSearchData(eventReleased.ConnID.ToString(), _sfdcPopupData, callType);
                        }
                        else
                            this._logger.Info("Search data is empty");
                    }

                    //Update Data
                    _sfdcPopupData = this._voiceEvents.GetOutboundUpdateData(eventReleased, callType, callDuration, "released", ixnNotes);
                    if (_sfdcPopupData != null)
                    {
                        _sfdcPopupData.InteractionId = eventReleased.ConnID.ToString();
                        this._searchHandler.ProcessUpdateData(eventReleased.ConnID.ToString(), _sfdcPopupData);
                    }
                    else
                        this._logger.Info("Search data is empty");
                }
                else if (callType == SFDCCallType.ConsultVoiceReceived)
                {
                    _sfdcPopupData = this._voiceEvents.GetConsultReceivedPopupData(eventReleased, callType, "released");
                    if (_sfdcPopupData != null)
                    {
                        _sfdcPopupData.InteractionId = eventReleased.ConnID.ToString();
                        this._searchHandler.ProcessSearchData(eventReleased.ConnID.ToString(), _sfdcPopupData, callType);
                    }
                    else
                        this._logger.Info("Search data is empty");

                    //Update Data
                    _sfdcPopupData = this._voiceEvents.GetConsultUpdateData(eventReleased, callType, callDuration, "released", ixnNotes);
                    if (_sfdcPopupData != null)
                    {
                        _sfdcPopupData.InteractionId = eventReleased.ConnID.ToString();
                        this._searchHandler.ProcessUpdateData(eventReleased.ConnID.ToString(), _sfdcPopupData);
                    }
                    else
                        this._logger.Info("Search data is empty");
                }
            }
            catch (Exception generalException)
            {
                _logger.Info("PopupRecords : Error Occurred  : " + generalException.ToString());
            }
        }

        #endregion Popup/Update Records

        #region UpdateRecords on DispositionCodeChanged after Call Released

        /// <summary>
        /// Update SFDC Records on EventReleased Event
        /// </summary>
        /// <param name="eventReleased"></param>
        /// <param name="callType"></param>
        /// <param name="callDuration"></param>
        public void UpdateRecords(EventReleased eventReleased, SFDCCallType callType, string callDuration)
        {
            try
            {
                string ixnNotes = Settings.SFDCListener.GetVoiceComments(eventReleased.ConnID.ToString());
                if (callType == SFDCCallType.InboundVoice)
                    _sfdcPopupData = this._voiceEvents.GetInboundUpdateData(eventReleased, callType, callDuration, "released", ixnNotes);
                else if (callType == SFDCCallType.ConsultVoiceReceived)
                    _sfdcPopupData = this._voiceEvents.GetConsultUpdateData(eventReleased, callType, callDuration, "released", ixnNotes);
                else if (callType == SFDCCallType.OutboundVoiceSuccess)
                    _sfdcPopupData = this._voiceEvents.GetOutboundUpdateData(eventReleased, callType, callDuration, "released", ixnNotes);

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
                _logger.Info("UpdateRecords : Error Occurred  : " + generalException.ToString());
            }
        }

        #endregion UpdateRecords on DispositionCodeChanged after Call Released
    }
}