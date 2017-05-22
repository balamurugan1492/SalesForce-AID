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

        private Log logger = null;
        private static CallPartyChanged partyChangedObject = null;
        private SFDCData sfdcPopupData = null;
        private VoiceEvents voiceEvents = null;

        #endregion Fields

        #region Constructor

        private CallPartyChanged()
        {
            this.logger = Log.GenInstance();
            this.voiceEvents = VoiceEvents.GetInstance();
        }

        #endregion Constructor

        #region GetInstance

        public static CallPartyChanged GetInstance()
        {
            if (partyChangedObject == null)
            {
                partyChangedObject = new CallPartyChanged();
            }
            return partyChangedObject;
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
                if (Settings.SFDCOptions.SFDCPopupPages != null)
                {
                    if (callType == SFDCCallType.ConsultReceived)
                    {
                        this.logger.Info("PopupRecords : EventPartyChanged.PopupRecords method Invoke ");
                        sfdcPopupData = this.voiceEvents.GetConsultReceivedPopupData(eventPartyChanged, callType, "partychanged");
                        if (sfdcPopupData != null)
                        {
                            sfdcPopupData.InteractionId = eventPartyChanged.ConnID.ToString();
                            this.voiceEvents.ProcessSearchData(eventPartyChanged.ConnID.ToString(), sfdcPopupData, callType);
                        }
                        else
                            this.logger.Info("Search data is empty");

                        //Update Data
                        this.logger.Info("Update SFDC for Consult Call");
                        sfdcPopupData = this.voiceEvents.GetConsultUpdateData(eventPartyChanged, callType, duration, "partychanged");
                        if (sfdcPopupData != null)
                        {
                            sfdcPopupData.InteractionId = eventPartyChanged.ConnID.ToString();
                            this.voiceEvents.ProcessUpdateData(eventPartyChanged.ConnID.ToString(), sfdcPopupData);
                        }
                        else
                            this.logger.Info("Search data is empty");
                    }
                }
            }
            catch (Exception generalException)
            {
                logger.Info("PopupRecords : Error Occurred  : " + generalException.ToString());
            }
        }

        #endregion PopupRecords
    }
}