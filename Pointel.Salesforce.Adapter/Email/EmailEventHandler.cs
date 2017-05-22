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

using Genesyslab.Platform.Commons.Collections;
using Genesyslab.Platform.Commons.Protocols;
using Genesyslab.Platform.Contacts.Protocols.ContactServer.Events;
using Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events;
using Pointel.Salesforce.Adapter.LogMessage;
using Pointel.Salesforce.Adapter.Utility;
using System;

namespace Pointel.Salesforce.Adapter.Email
{
    public class EmailEventHandler
    {
        #region Field Declarations

        public static bool IsClick2Email = false;
        private EventAck _eventAck = null;
        private EventInvite _eventInvite = null;
        private EventPulledInteractions _eventPulledInteractions = null;
        private Log _logger = null;

        #endregion Field Declarations

        #region Constructor

        public EmailEventHandler()
        {
            this._logger = Log.GenInstance();
        }

        #endregion Constructor

        #region Receive EmailEvents From Commands

        /// <summary>
        /// Receives the email events from commands.
        /// </summary>
        /// <param name="emailData">The email data.</param>
        public void ReceiveEmailEventsFromCommands(IXNCustomData emailData)
        {
            try
            {
                switch (emailData.EventName)
                {
                    case "ToasterInteractionEmailAccept":
                        GetEmailData(emailData.InteractionId, emailData);
                        if (Settings.SFDCListener.EmailActivityIdCollection.Keys.Contains(emailData.InteractionId))
                        {
                            if (Settings.SFDCOptions.EmailAttachActivityId)
                            {
                                Settings.SFDCListener.SetAttachedData(emailData.InteractionId, Settings.SFDCOptions.EmailAttachActivityIdKeyname, Settings.SFDCListener.EmailActivityIdCollection[emailData.InteractionId], Settings.EmailProxyClientId);
                                Settings.SFDCListener.EmailActivityIdCollection.Remove(emailData.InteractionId);
                            }
                        }
                        EmailConnected.GetInstance().PopupRecords(emailData, SFDCCallType.InboundEmail);
                        return;

                    case "ToasterInteractionEmailDecline":
                        GetEmailData(emailData.InteractionId, emailData);
                        EmailRejected.GetInstance().UpdateRecords(emailData, emailData.InteractionType);
                        return;

                    case "InteractionMarkDone"://InteractionEmailWorkflow"://mark done
                        GetEmailData(emailData.InteractionId, emailData);
                        EmailMarkDone.GetInstance().UpdateRecords(emailData, emailData.InteractionType);
                        return;

                    case "InteractionEmailMoveToWorkbin":
                        GetEmailData(emailData.InteractionId, emailData);
                        EmailMoveToWorkbin.GetInstance().UpdateRecords(emailData, emailData.InteractionType);
                        return;

                    case "InteractionEmailReply":
                    case "InteractionEmailReplyById":
                        GetEmailData(emailData.InteractionId, emailData);
                        EmailReply.GetInstance().PopupRecords(emailData, emailData.InteractionType);
                        return;

                    case "InteractionEmailSend":
                        emailData.InteractionType = SFDCCallType.OutboundEmailSuccess;
                        GetEmailData(emailData.InteractionId, emailData);
                        EmailSend.GetInstance().PopupRecords(emailData, SFDCCallType.OutboundEmailSuccess);
                        return;

                    case "InteractionEmailDelete":
                        if (emailData.InteractionId != null)
                        {
                            emailData.InteractionType = SFDCCallType.OutboundEmailSuccess;
                            GetEmailData(emailData.InteractionId, emailData);
                            EmailDelete.GetInstance().UpdateRecords(emailData, SFDCCallType.OutboundEmailSuccess);
                        }
                        return;

                    case "InteractionEmailDeleteById":
                        if (emailData.InteractionId != null)
                        {
                            emailData.InteractionType = SFDCCallType.OutboundEmailSuccess;
                            GetEmailData(emailData.InteractionId, emailData);
                            EmailDelete.GetInstance().UpdateRecords(emailData, SFDCCallType.OutboundEmailSuccess);
                        }
                        return;

                    default:
                        _logger.Info("Event skip :" + emailData.EventName);
                        break;
                }
            }
            catch (Exception generalException)
            {
                _logger.Error("Error Occurred, Exception :" + generalException.ToString());
            }
        }

        #endregion Receive EmailEvents From Commands

        #region ReceiveEmailEvents

        public void ReceiveEmailEvents(IMessage events)
        {
            try
            {
                if ((Settings.SFDCOptions.SFDCPopupPages != null) && (events != null))
                {
                    this._logger.Info("Agent Email Event : " + events.Name);
                    switch (events.Id)
                    {
                        case EventInvite.MessageId:
                            this._eventInvite = (EventInvite)events;
                            Settings.EmailProxyClientId = _eventInvite.ProxyClientId;
                            IXNCustomData emailInviteData = new IXNCustomData();
                            emailInviteData.InteractionId = this._eventInvite.Interaction.InteractionId;
                            emailInviteData.OpenMediaInteraction = this._eventInvite.Interaction;
                            emailInviteData.UserData = this._eventInvite.Interaction.InteractionUserData;
                            GetEmailData(emailInviteData.InteractionId, emailInviteData);
                            EmailInvite.GetInstance().PopupRecords(emailInviteData, emailInviteData.InteractionType);

                            if ((this._eventInvite.VisibilityMode.ToString() != "Coach") && (this._eventInvite.VisibilityMode.ToString() != "Conference"))
                            {
                            }

                            break;

                        case EventPulledInteractions.MessageId:
                            this._eventPulledInteractions = (EventPulledInteractions)events;
                            if (this._eventPulledInteractions.Interactions.Count == 1)
                            {
                                string str = this._eventPulledInteractions.Interactions.AllKeys.GetValue(0).ToString();
                                object[] kvc = this._eventPulledInteractions.Interactions.GetValues(str);
                                KeyValueCollection collection = (KeyValueCollection)kvc[0];
                                IXNCustomData emailPulledData = new IXNCustomData();
                                emailPulledData.InteractionId = collection["InteractionId"].ToString();
                                GetEmailData(emailPulledData.InteractionId, emailPulledData);
                                if (emailPulledData.IXN_Attributes.TypeId == "Inbound")
                                {
                                    emailPulledData.InteractionType = SFDCCallType.InboundEmailPulled;
                                }
                                else if (emailPulledData.IXN_Attributes.TypeId == "Outbound")
                                {
                                    emailPulledData.InteractionType = SFDCCallType.OutboundEmailPulled;
                                }
                                EmailPulled.GetInstance().PopupRecords(emailPulledData, emailPulledData.InteractionType);
                            }
                            break;

                        case EventAck.MessageId:
                            this._eventAck = (EventAck)events;
                            if (_eventAck.Extension != null && _eventAck.Extension.ContainsKey("InteractionId") && _eventAck.Extension["InteractionId"] != null)
                            {
                                string type = GetEmailInteractionContent(_eventAck.Extension["InteractionId"].ToString(), "type");
                                if (type.Equals("OutboundNew"))
                                {
                                    string ToAddress = GetEmailInteractionContent(_eventAck.Extension["InteractionId"].ToString(), "to");
                                    if (!String.IsNullOrEmpty(ToAddress))
                                    {
                                        IXNCustomData emailAckData = new IXNCustomData();
                                        emailAckData.InteractionType = SFDCCallType.OutboundEmailSuccess;
                                        emailAckData.InteractionId = _eventAck.Extension["InteractionId"].ToString();
                                        GetEmailData(emailAckData.InteractionId, emailAckData);
                                        if (IsClick2Email && SFDCHttpServer._emailData.ContainsKey(ToAddress))
                                        {
                                            Settings.ClickToEmailData.Add(_eventAck.Extension["InteractionId"].ToString(), SFDCHttpServer._emailData[ToAddress]);
                                            EmailManager.GetInstance().GetClickToEmailLogs(emailAckData, _eventAck.Extension["InteractionId"].ToString(), SFDCCallType.OutboundEmailSuccess, SFDCHttpServer._emailData[ToAddress], "create");
                                            IsClick2Email = false;
                                        }
                                        else
                                        {
                                            EmailCreate.GetInstance().PopupRecords(emailAckData, SFDCCallType.OutboundEmailSuccess);
                                        }
                                    }
                                    else
                                        this._logger.Warn("ReceiveEmailEvents : ToAddress is not found in EventAck");
                                }
                                else if (type.Equals("OutboundReply"))
                                {
                                    string ToAddress = GetEmailInteractionContent(_eventAck.Extension["InteractionId"].ToString(), "to");
                                    if (!String.IsNullOrEmpty(ToAddress))
                                    {
                                        IXNCustomData outEmailData = new IXNCustomData();
                                        outEmailData.InteractionType = SFDCCallType.OutboundEmailSuccess;
                                        outEmailData.InteractionId = _eventAck.Extension["InteractionId"].ToString();
                                        GetEmailData(outEmailData.InteractionId, outEmailData);
                                        if (IsClick2Email && SFDCHttpServer._emailData.ContainsKey(ToAddress))
                                        {
                                            Settings.ClickToEmailData.Add(_eventAck.Extension["InteractionId"].ToString(), SFDCHttpServer._emailData[ToAddress]);
                                            EmailManager.GetInstance().GetClickToEmailLogs(outEmailData, _eventAck.Extension["InteractionId"].ToString(), SFDCCallType.OutboundEmailSuccess, SFDCHttpServer._emailData[ToAddress], "create");
                                            IsClick2Email = false;
                                        }
                                        else
                                        {
                                            EmailCreate.GetInstance().PopupRecords(outEmailData, SFDCCallType.OutboundEmailSuccess);
                                        }
                                    }
                                    else
                                        this._logger.Warn("ReceiveEmailEvents : ToAddress is not found in EventAck");
                                }
                            }
                            break;
                    }
                }
            }
            catch (Exception exception)
            {
                this._logger.Error("ReceiveEmailEvents : Error occured while receiving Email events " + exception.ToString());
            }
        }

        #endregion ReceiveEmailEvents

        #region GetEmailInteractionContent

        /// <summary>
        /// Gets Email interaction Content
        /// </summary>
        /// <param name="ixnId"></param>
        /// <returns></returns>
        public string GetEmailInteractionContent(string ixnId, string type)
        {
            try
            {
                this._logger.Info("Retrieving email Content for the IXN Id : " + ixnId);
                EventGetInteractionContent emailContent = Settings.SFDCListener.GetOpenMediaInteractionContent(ixnId, false);
                if (emailContent != null)
                {
                    if (type.Equals("subject") && !String.IsNullOrEmpty(emailContent.InteractionContent.Text))
                    {
                        return emailContent.InteractionContent.Text;
                    }
                    else if (type.Equals("to") && emailContent.InteractionAttributes != null && emailContent.InteractionAttributes.AllAttributes != null && emailContent.InteractionAttributes.AllAttributes.ContainsKey("To"))
                    {
                        return emailContent.InteractionAttributes.AllAttributes.GetAsString("To");
                    }
                    else if (type.Equals("type") && emailContent.InteractionAttributes != null && emailContent.InteractionAttributes.SubtypeId != null)
                    {
                        return emailContent.InteractionAttributes.SubtypeId;
                    }
                }
                else
                    this._logger.Error("Null Response received from UCS..");
            }
            catch (Exception generalException)
            {
                this._logger.Error("GetEmailInteractionContent : Error occurred at :" + generalException.ToString());
            }
            return string.Empty;
        }

        #endregion GetEmailInteractionContent

        #region UpdateDispositionCode

        /// <summary>
        /// Gets Updated Disposition Code Change
        /// </summary>
        /// <param name="ixnId"></param>
        /// <param name="key"></param>
        /// <param name="code"></param>
        public void UpdateDispositionCodeChange(IXNCustomData dispCode)
        {
            try
            {
                GetEmailData(dispCode.InteractionId, dispCode);
                EmailInvite.GetInstance().UpdateRecords(dispCode, dispCode.InteractionType);
            }
            catch (Exception generalException)
            {
                this._logger.Error("UpdateDispositionCodeChange : Error at updating disposition code Changed : " + generalException.ToString());
            }
        }

        #endregion UpdateDispositionCode

        #region GetEmailData From UCS

        private void GetEmailData(string ixnId, IXNCustomData emailData)
        {
            try
            {
                this._logger.Info("Retrieving UCS content for the InteractionId :" + ixnId);
                EventGetInteractionContent emailContent = Settings.SFDCListener.GetOpenMediaInteractionContent(ixnId, false);
                if (emailContent != null)
                {
                    if (emailContent.InteractionAttributes.TypeId == "Inbound")
                        emailData.InteractionType = SFDCCallType.InboundEmail;
                    else if (emailContent.InteractionAttributes.TypeId == "Outbound")
                        emailData.InteractionType = SFDCCallType.OutboundEmailSuccess;

                    emailData.IXN_Attributes = emailContent.InteractionAttributes;
                    emailData.EntityAttributes = emailContent.EntityAttributes;
                    emailData.AttachmentLists = emailContent.Attachments;
                    emailData.InteractionContents = emailContent.InteractionContent;
                    if (emailData.UserData == null)
                        emailData.UserData = emailContent.InteractionAttributes.AllAttributes;
                }
                else
                    this._logger.Info("Null is returned from UCS");
            }
            catch (Exception generalException)
            {
                this._logger.Info("Error Occurred, Exception : " + generalException.ToString());
            }
        }

        #endregion GetEmailData From UCS
    }
}