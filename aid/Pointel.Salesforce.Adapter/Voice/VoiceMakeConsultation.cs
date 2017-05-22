#region Header

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

#endregion Header

namespace Pointel.Salesforce.Adapter.Voice
{
    using System;

    using Genesyslab.Platform.Commons.Collections;
    using Genesyslab.Platform.Voice.Protocols.TServer.Requests.Party;

    using Pointel.Salesforce.Adapter.LogMessage;
    using Pointel.Salesforce.Adapter.Utility;

    internal class VoiceMakeConsultation
    {
        #region Fields

        private static VoiceMakeConsultation thisInstance = null;

        private Log logger = null;

        #endregion Fields

        #region Constructors

        private VoiceMakeConsultation()
        {
            this.logger = Log.GenInstance();
        }

        #endregion Constructors

        #region Methods

        public static VoiceMakeConsultation GetInstance()
        {
            if (thisInstance == null)
            {
                thisInstance = new VoiceMakeConsultation();
            }
            return thisInstance;
        }

        public void DialVoiceConsultation(string OtherDN, string recordType, string recordId)
        {
            try
            {
                this.logger.Info("DialVoiceConsultation : Dialing Consult call for the DN : " + OtherDN);
                if (Settings.AgentDetails.TServer != null)
                {
                    RequestInitiateConference requestInitConference = RequestInitiateConference.Create();
                    requestInitConference.OtherDN = OtherDN;
                    if (SubscribeVoiceEvents.eventEstablished.TransferConnID != null)
                    {
                        requestInitConference.ConnID = SubscribeVoiceEvents.eventEstablished.TransferConnID;
                    }
                    else
                    {
                        requestInitConference.ConnID = SubscribeVoiceEvents.eventEstablished.ConnID;
                    }
                    KeyValueCollection reason = new KeyValueCollection();
                    reason.Add("ClickToDial", OtherDN + "," + recordType + "," + recordId);
                    reason.Add("OperationMode", "Conference");
                    requestInitConference.Reasons = reason;
                    requestInitConference.ThisDN = SubscribeVoiceEvents.eventEstablished.ThisDN;
                    requestInitConference.Location = SubscribeVoiceEvents.eventEstablished.Location;
                    requestInitConference.UserData = SubscribeVoiceEvents.eventEstablished.UserData;
                    requestInitConference.Extensions = SubscribeVoiceEvents.eventEstablished.Extensions;
                    logger.Info("*****************************************************************");
                    logger.Info("DialVoiceConsultation : Dialing Consult call ......");
                    logger.Info("DialVoiceConsultation : Request  Data :" + requestInitConference.ToString());
                    logger.Info("*****************************************************************");
                    Settings.AgentDetails.TServer.Send(requestInitConference);
                }
                else
                {
                    logger.Info("DialVoiceConsultation : T-Server Protocol is Null");
                }
            }
            catch (Exception generalException)
            {
                logger.Error("DialVoiceConsultation : Error Occured : " + generalException.ToString());
            }
        }

        #endregion Methods
    }
}