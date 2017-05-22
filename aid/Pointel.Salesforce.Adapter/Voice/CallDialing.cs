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

        private Log logger = null;
        private static CallDialing dialingObject = null;
        private SFDCData sfdcPopupData = null;
        private VoiceEvents voiceEvents = null;

        #endregion Fields

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        private CallDialing()
        {
            this.logger = Log.GenInstance();
            this.voiceEvents = VoiceEvents.GetInstance();
        }

        #endregion Constructor

        #region GetInstance

        /// <summary>
        /// Gets the Instance of the Class
        /// </summary>
        /// <returns></returns>
        public static CallDialing GetInstance()
        {
            if (dialingObject == null)
            {
                dialingObject = new CallDialing();
            }
            return dialingObject;
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
                this.logger.Info("PopupRecords : CallDialing.PopupRecords method Invoke");
                if (Settings.ClickToDialData.ContainsKey(eventDialing.ConnID.ToString()))
                {
                    this.voiceEvents.GetClickToDialLogs(eventDialing, eventDialing.ConnID.ToString(),
                        callType, Settings.ClickToDialData[eventDialing.ConnID.ToString()], "dialing");
                }
                else if (Settings.SFDCOptions.EnablePopupDialedFromDesktop)
                {
                    if (callType == SFDCCallType.OutboundSuccess)
                    {
                        this.logger.Info("Popup SFDC for Outbound Call ");
                        sfdcPopupData = this.voiceEvents.GetOutboundPopupData(eventDialing, callType, "dialing");
                        if (sfdcPopupData != null)
                        {
                            sfdcPopupData.InteractionId = eventDialing.ConnID.ToString();
                            this.voiceEvents.ProcessSearchData(eventDialing.ConnID.ToString(), sfdcPopupData, callType);
                        }
                        else
                            this.logger.Info("Search data is empty");
                    }
                    else if (callType == SFDCCallType.ConsultSuccess)
                    {
                        this.logger.Info("Popup SFDC for Consult Call");
                        sfdcPopupData = this.voiceEvents.GetConsultDialedPopupData(eventDialing, callType, "dialing");
                        if (sfdcPopupData != null)
                        {
                            sfdcPopupData.InteractionId = eventDialing.ConnID.ToString();
                            this.voiceEvents.ProcessSearchData(eventDialing.ConnID.ToString(), sfdcPopupData, callType);
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