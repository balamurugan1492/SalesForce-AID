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

using Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events;
using Pointel.Salesforce.Adapter.LogMessage;
using Pointel.Salesforce.Adapter.SFDCUtils;
using Pointel.Salesforce.Adapter.Utility;
using System;

namespace Pointel.Salesforce.Adapter.Chat
{/// <summary>
    /// Comment: Provides Inbound Voice Call SFDC Popup
    /// Last Modified: 25-08-2015
    /// Created by: Pointel Inc
    /// </summary>
    internal class InboundChatInvite
    {
        #region Fields

        private Log logger = null;
        private static InboundChatInvite InviteObject = null;
        private SFDCData sfdcPopupData = null;
        private ChatEvents chatEvents = null;

        #endregion Fields

        #region Constructor

        private InboundChatInvite()
        {
            this.logger = Log.GenInstance();
            this.chatEvents = ChatEvents.GetInstance();
        }

        #endregion Constructor

        #region GetInstance

        public static InboundChatInvite GetInstance()
        {
            if (InviteObject == null)
            {
                InviteObject = new InboundChatInvite();
            }
            return InviteObject;
        }

        #endregion GetInstance

        #region PopupRecords

        public void PopupRecords(EventInvite eventInvite, SFDCCallType callType)
        {
            try
            {
                if (callType == SFDCCallType.InboundChat)
                {
                    this.logger.Info("PopupRecords : Popup SFDC for Inbound on EventInvite Event ");
                    sfdcPopupData = this.chatEvents.GetInboundPopupData(eventInvite, callType, "invite");
                    if (sfdcPopupData != null)
                    {
                        sfdcPopupData.InteractionId = eventInvite.Interaction.InteractionId.ToString();
                        //Settings.SFDCPopupData.Add(eventInvite.Interaction.InteractionId.ToString()+"invite", sfdcPopupData);
                    }
                    else
                        this.logger.Info("PopupRecords ; Search data is empty");
                }
                else if (callType == SFDCCallType.ConsultChatReceived)
                {
                    this.logger.Info("PopupRecords : Popup SFDC for Consult on EventInvite Event ");
                    sfdcPopupData = this.chatEvents.GetConsultReceivedPopupData(eventInvite, callType, "invite");
                    if (sfdcPopupData != null)
                    {
                        sfdcPopupData.InteractionId = eventInvite.Interaction.InteractionId.ToString();
                        //Settings.SFDCPopupData.Add(eventInvite.Interaction.InteractionId.ToString() + "invite", sfdcPopupData);
                    }
                    else
                        this.logger.Info("PopupRecords : Search data is empty");
                }
            }
            catch (Exception generalException)
            {
                logger.Error("PopupRecords : Error Occurred while collecting Log data : " + generalException.ToString());
            }
        }

        #endregion PopupRecords

        #region UpdateRecords

        public void UpdateRecords(EventInvite eventInvite, SFDCCallType callType, string callDuration, string chatContent)
        {
            try
            {
                if (Settings.SFDCOptions.SFDCPopupPages != null)
                {
                    if (callType == SFDCCallType.InboundChat || callType == SFDCCallType.ConsultChatReceived)
                    {
                        this.logger.Info("UpdateRecords : Update SFDC Logs for Inbound ");
                        sfdcPopupData = this.chatEvents.GetInboundUpdateData(eventInvite, callType, callDuration, chatContent, "datachanged");
                        if (sfdcPopupData != null)
                        {
                            sfdcPopupData.InteractionId = eventInvite.Interaction.InteractionId;
                            this.chatEvents.SendUpdateLogData(eventInvite.Interaction.InteractionId, sfdcPopupData);
                        }
                        else
                            this.logger.Info("UpdateRecords : Search data is empty");
                    }
                }
            }
            catch (Exception generalException)
            {
                this.logger.Error("UpdateRecords : Error Occurred in Updating sfdc data : " + generalException.ToString());
            }
        }

        #endregion UpdateRecords
    }
}