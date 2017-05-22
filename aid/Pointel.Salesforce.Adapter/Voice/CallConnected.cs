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

        private Log logger = null;
        private static CallConnected connectedObject = null;
        private SFDCData sfdcPopupData = null;
        private VoiceEvents voiceEvents = null;

        #endregion Fields

        #region Constructor

        private CallConnected()
        {
            this.logger = Log.GenInstance();
            this.voiceEvents = VoiceEvents.GetInstance();
        }

        #endregion Constructor

        #region GetInstance

        public static CallConnected GetInstance()
        {
            if (connectedObject == null)
            {
                connectedObject = new CallConnected();
            }
            return connectedObject;
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
                this.logger.Info("PopupRecords : CallConnected.PopupRecords method Invoke");
                if (callType == SFDCCallType.Inbound)
                {
                    this.logger.Info("Popup SFDC for Inbound Call");
                    sfdcPopupData = this.voiceEvents.GetInboundPopupData(eventEstablished, callType, "established");
                    if (sfdcPopupData != null)
                    {
                        sfdcPopupData.InteractionId = eventEstablished.ConnID.ToString();
                        this.voiceEvents.ProcessSearchData(eventEstablished.ConnID.ToString(), sfdcPopupData, callType);
                    }
                    else
                        this.logger.Info("Search data is empty");
                }
                else if (callType == SFDCCallType.ConsultReceived)
                {
                    this.logger.Info("Popup SFDC for Consult Call");
                    sfdcPopupData = this.voiceEvents.GetConsultReceivedPopupData(eventEstablished, callType, "established");
                    if (sfdcPopupData != null)
                    {
                        sfdcPopupData.InteractionId = eventEstablished.ConnID.ToString();
                        this.voiceEvents.ProcessSearchData(eventEstablished.ConnID.ToString(), sfdcPopupData, callType);
                    }
                    else
                        this.logger.Info("Search data is empty");
                }
                else if (Settings.ClickToDialData.ContainsKey(eventEstablished.ConnID.ToString()))
                {
                    this.voiceEvents.GetClickToDialLogs(eventEstablished, eventEstablished.ConnID.ToString(),
                        callType, Settings.ClickToDialData[eventEstablished.ConnID.ToString()], "established");
                }
                else if (Settings.SFDCOptions.EnablePopupDialedFromDesktop)
                {
                    if (callType == SFDCCallType.OutboundSuccess)
                    {
                        this.logger.Info("Popup SFDC for outbound on EventEstablished Event ");
                        sfdcPopupData = this.voiceEvents.GetOutboundPopupData(eventEstablished, callType, "established");
                        if (sfdcPopupData != null)
                        {
                            sfdcPopupData.InteractionId = eventEstablished.ConnID.ToString();
                            this.voiceEvents.ProcessSearchData(eventEstablished.ConnID.ToString(), sfdcPopupData, callType);
                        }
                        else
                            this.logger.Info("Search data is empty");
                    }
                    else if (callType == SFDCCallType.ConsultSuccess)
                    {
                        this.logger.Info("Popup SFDC for Consult on EventEstablished Event ");
                        if (sfdcPopupData != null)
                        {
                            sfdcPopupData.InteractionId = eventEstablished.ConnID.ToString();
                            this.voiceEvents.ProcessSearchData(eventEstablished.ConnID.ToString(), sfdcPopupData, callType);
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