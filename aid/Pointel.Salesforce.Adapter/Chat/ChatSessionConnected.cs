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
    internal class ChatSessionConnected
    {/// <summary>
        /// Comment: Provides Inbound Chat SFDC Popup
        /// Last Modified: 25-08-2015
        /// Created by: Pointel Inc
        /// </summary>

        #region Fields

        private Log logger = null;
        private static ChatSessionConnected InviteObject = null;
        private SFDCData sfdcPopupData = null;
        private ChatEvents chatEvents = null;

        #endregion Fields

        #region Constructor

        /// <summary>
        /// Constructor of the Class
        /// </summary>
        private ChatSessionConnected()
        {
            this.logger = Log.GenInstance();
            this.chatEvents = ChatEvents.GetInstance();
        }

        #endregion Constructor

        #region GetInstance

        /// <summary>
        /// Gets the Instance of the class
        /// </summary>
        /// <returns></returns>
        public static ChatSessionConnected GetInstance()
        {
            if (InviteObject == null)
            {
                InviteObject = new ChatSessionConnected();
            }
            return InviteObject;
        }

        #endregion GetInstance

        #region PopupRecords

        /// <summary>
        /// Popup SFDC on EventInvite Event
        /// </summary>
        /// <param name="eventInvite"></param>
        /// <param name="callType"></param>
        public void PopupRecords(EventInvite eventInvite, SFDCCallType callType)
        {
            try
            {
                if (callType == SFDCCallType.InboundChat)
                {
                    this.logger.Info("PopupRecords : Popup SFDC for Inbound on ChatConnected Event ");
                    sfdcPopupData = this.chatEvents.GetInboundPopupData(eventInvite, callType, "established");
                    if (sfdcPopupData != null)
                    {
                        sfdcPopupData.InteractionId = eventInvite.Interaction.InteractionId;
                        this.chatEvents.SendPopupData(eventInvite.Interaction.InteractionId + "estab", sfdcPopupData);
                    }
                    else
                        this.logger.Info("PopupRecords : Search data is empty");
                }
            }
            catch (Exception generalException)
            {
                this.logger.Error("PopupRecords : Error Occurred at ChatConnected : " + generalException.ToString());
            }
        }

        #endregion PopupRecords
    }
}