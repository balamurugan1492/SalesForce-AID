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
{
    internal class ChatSessionEnded
    {
        #region Fields

        private Log logger = null;
        private static ChatSessionEnded InviteObject = null;
        private SFDCData sfdcPopupData = null;
        private ChatEvents chatEvents = null;

        #endregion Fields

        #region Constructor

        /// <summary>
        /// Constructor of the Class ChatSessionEnded
        /// </summary>
        private ChatSessionEnded()
        {
            this.logger = Log.GenInstance();
            this.chatEvents = ChatEvents.GetInstance();
        }

        #endregion Constructor

        #region GetInstance

        /// <summary>
        /// Gets Instance of the class ChatSessionEnded
        /// </summary>
        /// <returns></returns>
        public static ChatSessionEnded GetInstance()
        {
            if (InviteObject == null)
            {
                InviteObject = new ChatSessionEnded();
            }
            return InviteObject;
        }

        #endregion GetInstance

        #region PopupRecords

        /// <summary>
        /// Popup SFDC on Chat Connected
        /// </summary>
        /// <param name="eventInvite"></param>
        /// <param name="callType"></param>
        /// <param name="callDuration"></param>
        /// <param name="chatContent"></param>
        public void PopupRecords(EventInvite eventInvite, SFDCCallType callType, string callDuration, string chatContent)
        {
            try
            {
                if (callType == SFDCCallType.InboundChat)
                {
                    this.logger.Info("PopupRecords : Popup SFDC for Inbound on ChatSessionEnded ");
                    sfdcPopupData = this.chatEvents.GetInboundPopupData(eventInvite, callType, "released");
                    if (sfdcPopupData != null)
                    {
                        sfdcPopupData.InteractionId = eventInvite.Interaction.InteractionId;
                        this.chatEvents.SendPopupData(eventInvite.Interaction.InteractionId + "release", sfdcPopupData);
                    }
                    else
                        this.logger.Info(" PopupRecords : Search data is empty");

                    //Update Data
                    this.logger.Info("PopupRecords : Update SFDC Logs for Inbound on ChatSessionEnded ");
                    sfdcPopupData = this.chatEvents.GetInboundUpdateData(eventInvite, callType, callDuration, chatContent, "released");
                    if (sfdcPopupData != null)
                    {
                        sfdcPopupData.InteractionId = eventInvite.Interaction.InteractionId;
                        this.chatEvents.SendUpdateLogData(eventInvite.Interaction.InteractionId, sfdcPopupData);
                    }
                    else
                        this.logger.Info("PopupRecords : Search data is empty");
                }
                else if (callType == SFDCCallType.ConsultChatReceived)
                {
                    this.logger.Info("PopupRecords : Popup SFDC for Inbound on ChatSessionEnded ");
                    sfdcPopupData = this.chatEvents.GetConsultReceivedPopupData(eventInvite, callType, "released");
                    if (sfdcPopupData != null)
                    {
                        sfdcPopupData.InteractionId = eventInvite.Interaction.InteractionId;
                        this.chatEvents.SendPopupData(eventInvite.Interaction.InteractionId + "release", sfdcPopupData);
                    }
                    else
                        this.logger.Info("PopupRecords : Search data is empty");

                    //Update Data
                    this.logger.Info("PopupRecords : Update SFDC Logs for Inbound on ChatSessionEnded ");
                    sfdcPopupData = this.chatEvents.GetConsultUpdateData(eventInvite, callType, callDuration, chatContent, "released");
                    if (sfdcPopupData != null)
                    {
                        sfdcPopupData.InteractionId = eventInvite.Interaction.InteractionId;
                        this.chatEvents.SendUpdateLogData(eventInvite.Interaction.InteractionId, sfdcPopupData);
                    }
                    else
                        this.logger.Info("PopupRecords : Search data is empty");
                }
            }
            catch (Exception generalException)
            {
                this.logger.Error("PopupRecords : Error Occurred While collecting log data : " + generalException.ToString());
            }
        }

        #endregion PopupRecords
    }
}