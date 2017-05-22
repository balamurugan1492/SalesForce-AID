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
using Pointel.Salesforce.Adapter.Configurations;
using Pointel.Salesforce.Adapter.LogMessage;
using Pointel.Salesforce.Adapter.SFDCUtils;
using Pointel.Salesforce.Adapter.Utility;
using System;

namespace Pointel.Salesforce.Adapter.SFDCModels
{
    /// <summary>
    /// Comment: Provides Activity Log Data for User Profile Level Activity Last Modified: 25-08-2015
    /// Created by: Pointel Inc
    /// </summary>
    internal class SFDCUserActivity
    {
        #region Field Declaration

        private static SFDCUserActivity _sfdcUserActivity = null;
        private KeyValueCollection _chatUserActivityLog = null;
        private UserActivityOptions _chatUserActivityOptions = null;
        private KeyValueCollection _emailUserActivityLog = null;
        private UserActivityOptions _emailUserActivityOptions = null;
        private Log _logger = null;
        private SFDCUtility _sfdcUtility = null;
        private KeyValueCollection _voiceUserActivityLog = null;
        private UserActivityOptions _voiceUserActivityOptions = null;

        #endregion Field Declaration

        #region Constructor

        private SFDCUserActivity()
        {
            this._logger = Log.GenInstance();
            this._voiceUserActivityOptions = Settings.UserActivityVoiceOptions;
            this._chatUserActivityOptions = Settings.UserActivityChatOptions;
            this._voiceUserActivityLog = (Settings.VoiceActivityLogCollection.ContainsKey("useractivity")) ? Settings.VoiceActivityLogCollection["useractivity"] : null;
            this._chatUserActivityLog = (Settings.ChatActivityLogCollection.ContainsKey("useractivity")) ? Settings.ChatActivityLogCollection["useractivity"] : null;
            this._sfdcUtility = SFDCUtility.GetInstance();
            this._emailUserActivityOptions = Settings.UserActivityEmailOptions;
            this._emailUserActivityLog = (Settings.EmailActivityLogCollection.ContainsKey("useractivity")) ? Settings.EmailActivityLogCollection["useractivity"] : null;
        }

        #endregion Constructor

        #region GetInstance Method

        public static SFDCUserActivity GetInstance()
        {
            if (_sfdcUserActivity == null)
            {
                _sfdcUserActivity = new SFDCUserActivity();
            }
            return _sfdcUserActivity;
        }

        #endregion GetInstance Method

        #region GetVoiceCreateUserAcitivityData

        public IUserActivity GetVoiceCreateUserAcitivityData(IMessage message, SFDCCallType callType)
        {
            try
            {
                this._logger.Info("GetCreateUserAcitivityData :  Reading UserActivity Log Data.....");
                this._logger.Info("GetCreateUserAcitivityData :  Event Name : " + message.Name);
                dynamic popupEvent = Convert.ChangeType(message, message.GetType());
                if (popupEvent != null)
                {
                    IUserActivity userActivity = new UserActivityData();

                    #region Collect User Activity Data

                    userActivity.ObjectName = this._voiceUserActivityOptions.ObjectName;
                    if (callType == SFDCCallType.InboundVoice)
                    {
                        userActivity.CreateActvityLog = this._voiceUserActivityOptions.InboundCanCreateLog;
                        if (this._voiceUserActivityOptions.InboundCanCreateLog && this._voiceUserActivityLog != null)
                        {
                            userActivity.ActivityLogData = this._sfdcUtility.GetCreateActivityLogData(this._voiceUserActivityLog, popupEvent, callType);
                        }
                    }
                    else if (callType == SFDCCallType.OutboundVoiceSuccess)
                    {
                        userActivity.CreateActvityLog = this._voiceUserActivityOptions.OutboundCanCreateLog;
                        if (this._voiceUserActivityOptions.OutboundCanCreateLog && this._voiceUserActivityLog != null)
                        {
                            userActivity.CreateActvityLog = true;
                            userActivity.ActivityLogData = this._sfdcUtility.GetCreateActivityLogData(this._voiceUserActivityLog, popupEvent, callType);
                        }
                    }
                    else if (callType == SFDCCallType.OutboundVoiceFailure)
                    {
                        userActivity.CreateActvityLog = this._voiceUserActivityOptions.OutboundFailureCanCreateLog;
                        if (this._voiceUserActivityOptions.OutboundFailureCanCreateLog && this._voiceUserActivityLog != null)
                        {
                            userActivity.CreateActvityLog = true;
                            userActivity.ActivityLogData = this._sfdcUtility.GetCreateActivityLogData(this._voiceUserActivityLog, popupEvent, callType);
                        }
                    }

                    #endregion Collect User Activity Data

                    return userActivity;
                }
            }
            catch (Exception generalException)
            {
                this._logger.Error("GetCreateUserAcitivityData : Error occurred  : " + generalException.ToString());
            }
            return null;
        }

        #endregion GetVoiceCreateUserAcitivityData

        #region GetVoiceUpdateUserAcitivityData

        public IUserActivity GetVoiceUpdateUserAcitivityData(IMessage message, SFDCCallType callType, string callDuration, string notes)
        {
            try
            {
                this._logger.Info("GetUpdateUserAcitivityData :  Reading UserActivity Log Data.....");
                this._logger.Info("GetUpdateUserAcitivityData :  Event Name : " + message.Name);
                dynamic popupEvent = Convert.ChangeType(message, message.GetType());

                if (popupEvent != null)
                {
                    IUserActivity userActivity = new UserActivityData();

                    #region Collect Lead Data

                    userActivity.ObjectName = this._voiceUserActivityOptions.ObjectName;
                    if (callType == SFDCCallType.InboundVoice)
                    {
                        userActivity.UpdateActivityLog = this._voiceUserActivityOptions.InboundCanUpdateLog;
                        if (this._voiceUserActivityOptions.InboundCanUpdateLog && this._voiceUserActivityLog != null)
                        {
                            userActivity.UpdateActivityLogData = this._sfdcUtility.GetUpdateActivityLogData(this._voiceUserActivityLog, popupEvent, callType, callDuration, voiceComments: notes);
                        }
                    }
                    else if (callType == SFDCCallType.OutboundVoiceSuccess || callType == SFDCCallType.OutboundVoiceFailure)
                    {
                        userActivity.UpdateActivityLog = this._voiceUserActivityOptions.OutboundCanUpdateLog;
                        if (this._voiceUserActivityOptions.OutboundCanUpdateLog && this._voiceUserActivityLog != null)
                        {
                            userActivity.UpdateActivityLogData = this._sfdcUtility.GetUpdateActivityLogData(this._voiceUserActivityLog, popupEvent, callType, callDuration, voiceComments: notes);
                        }
                    }
                    else if (callType == SFDCCallType.ConsultVoiceReceived)
                    {
                        userActivity.UpdateActivityLog = this._voiceUserActivityOptions.ConsultCanUpdateLog;
                        if (this._voiceUserActivityOptions.ConsultCanUpdateLog && this._voiceUserActivityLog != null)
                        {
                            userActivity.UpdateActivityLogData = this._sfdcUtility.GetUpdateActivityLogData(this._voiceUserActivityLog, popupEvent, callType, callDuration, voiceComments: notes);
                        }
                    }

                    #endregion Collect Lead Data

                    return userActivity;
                }
            }
            catch (Exception generalException)
            {
                this._logger.Error("GetInboundLeadPopupData : Error occurred while reading Lead Data on EventRinging Event : " + generalException.ToString());
            }
            return null;
        }

        #endregion GetVoiceUpdateUserAcitivityData

        #region GetChatCreateUserAcitivityData

        public IUserActivity GetChatCreateUserAcitivityData(IXNCustomData chatData, SFDCCallType callType)
        {
            try
            {
                IMessage message = chatData.InteractionEvent;
                this._logger.Info("GetChatCreateUserAcitivityData :  Reading Chat UserActivity Log Data.....");
                this._logger.Info("GetChatCreateUserAcitivityData :  Event Name : " + message.Name);
                dynamic popupEvent = Convert.ChangeType(message, message.GetType());
                if (popupEvent != null)
                {
                    IUserActivity userActivity = new UserActivityData();

                    #region Collect Lead Data

                    userActivity.ObjectName = this._chatUserActivityOptions.ObjectName;
                    if (callType == SFDCCallType.InboundChat)
                    {
                        userActivity.CreateActvityLog = this._chatUserActivityOptions.InboundCanCreateLog;
                        if (this._chatUserActivityOptions.InboundCanCreateLog && this._chatUserActivityLog != null)
                        {
                            userActivity.ActivityLogData = this._sfdcUtility.GetCreateActivityLogData(this._chatUserActivityLog, popupEvent, callType, emailData: chatData);
                        }
                    }
                    else if (callType == SFDCCallType.ConsultChatReceived)
                    {
                        userActivity.CreateActvityLog = this._chatUserActivityOptions.ConsultCanCreateLog;
                        if (this._chatUserActivityOptions.ConsultCanCreateLog && this._chatUserActivityLog != null)
                        {
                            userActivity.ActivityLogData = this._sfdcUtility.GetCreateActivityLogData(this._chatUserActivityLog, popupEvent, callType, emailData: chatData);
                        }
                    }

                    #endregion Collect Lead Data

                    return userActivity;
                }
            }
            catch (Exception generalException)
            {
                this._logger.Error("GetChatCreateUserAcitivityData : Error occurred  : " + generalException.ToString());
            }
            return null;
        }

        #endregion GetChatCreateUserAcitivityData

        #region GetChatUpdateUserAcitivityData

        public IUserActivity GetChatUpdateUserAcitivityData(IXNCustomData chatData, string eventName)
        {
            try
            {
                IMessage message = chatData.InteractionEvent;
                SFDCCallType callType = chatData.InteractionType;
                string callDuration = chatData.Duration;
                this._logger.Info("GetChatUpdateUserAcitivityData :  Reading User Activity Update Data.....");
                this._logger.Info("GetChatUpdateUserAcitivityData :  Event Name : " + message.Name);
                dynamic popupEvent = Convert.ChangeType(message, message.GetType());

                if (popupEvent != null)
                {
                    IUserActivity userActivity = new UserActivityData();

                    #region Collect Lead Data

                    userActivity.ObjectName = this._voiceUserActivityOptions.ObjectName;

                    if (callType == SFDCCallType.InboundChat)
                    {
                        if (this._voiceUserActivityOptions.InboundCanUpdateLog)
                        {
                            userActivity.UpdateActivityLog = true;
                            userActivity.UpdateActivityLogData = this._sfdcUtility.GetUpdateActivityLogData(_chatUserActivityLog, popupEvent, callType, callDuration, emailData: chatData);
                        }
                    }
                    if (callType == SFDCCallType.ConsultChatReceived)
                    {
                        if (this._voiceUserActivityOptions.ConsultCanUpdateLog)
                        {
                            userActivity.UpdateActivityLog = true;
                            userActivity.UpdateActivityLogData = this._sfdcUtility.GetUpdateActivityLogData(_chatUserActivityLog, popupEvent, callType, callDuration, emailData: chatData);
                        }
                    }

                    #endregion Collect Lead Data

                    return userActivity;
                }
            }
            catch (Exception generalException)
            {
                this._logger.Error("GetChatUpdateUserAcitivityData : Error occurred while reading Lead Data : " + generalException.ToString());
            }
            return null;
        }

        #endregion GetChatUpdateUserAcitivityData

        #region GetEmailCreateUserAcitivityData

        public IUserActivity GetEmailCreateUserAcitivityData(IMessage message, SFDCCallType callType)
        {
            try
            {
                this._logger.Info("GetEmailCreateUserAcitivityData :  Reading Email UserActivity Log Data.....");
                this._logger.Info("GetEmailCreateUserAcitivityData :  Event Name : " + message.Name);
                dynamic popupEvent = null;
                switch (message.Id)
                {
                    #region Events

                    case Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events.EventInvite.MessageId:
                        popupEvent = (Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events.EventInvite)message;
                        break;

                    case Genesyslab.Platform.WebMedia.Protocols.BasicChat.Events.EventSessionInfo.MessageId:
                        popupEvent = (Genesyslab.Platform.WebMedia.Protocols.BasicChat.Events.EventSessionInfo)message;

                        break;

                    default:
                        break;

                    #endregion Events
                }
                if (popupEvent != null)
                {
                    IUserActivity userActivity = new UserActivityData();

                    #region Collect Lead Data

                    userActivity.ObjectName = this._emailUserActivityOptions.ObjectName;
                    if (callType == SFDCCallType.InboundEmail || callType == SFDCCallType.InboundEmailPulled)
                    {
                        userActivity.CreateActvityLog = this._emailUserActivityOptions.InboundCanCreateLog;
                        if (this._emailUserActivityOptions.InboundCanCreateLog && this._emailUserActivityLog != null)
                        {
                            userActivity.ActivityLogData = this._sfdcUtility.GetCreateActivityLogData(this._emailUserActivityLog, popupEvent, callType);
                        }
                    }
                    else if (callType == SFDCCallType.OutboundEmailFailure || callType == SFDCCallType.OutboundEmailSuccess || callType == SFDCCallType.OutboundEmailPulled)
                    {
                        userActivity.CreateActvityLog = this._emailUserActivityOptions.OutboundCanCreateLog;
                        if (this._emailUserActivityOptions.OutboundCanCreateLog && this._emailUserActivityLog != null)
                        {
                            userActivity.ActivityLogData = this._sfdcUtility.GetCreateActivityLogData(this._emailUserActivityLog, popupEvent, callType);
                        }
                    }

                    #endregion Collect Lead Data

                    return userActivity;
                }
            }
            catch (Exception generalException)
            {
                this._logger.Error("GetChatCreateUserAcitivityData : Error occurred  : " + generalException.ToString());
            }
            return null;
        }

        #endregion GetEmailCreateUserAcitivityData

        #region GetEmailUpdateUserAcitivityData

        public IUserActivity GetEmailUpdateUserAcitivityData(IMessage message, SFDCCallType callType, string callDuration, string emailContent)
        {
            try
            {
                this._logger.Info("GetEmailUpdateUserAcitivityData :  Reading User Activity Update Data.....");
                this._logger.Info("GetEmailUpdateUserAcitivityData :  Event Name : " + message.Name);
                dynamic popupEvent = null;
                switch (message.Id)
                {
                    #region Events

                    case Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events.EventInvite.MessageId:
                        popupEvent = (Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events.EventInvite)message;
                        break;

                    case Genesyslab.Platform.WebMedia.Protocols.BasicChat.Events.EventSessionInfo.MessageId:
                        popupEvent = (Genesyslab.Platform.WebMedia.Protocols.BasicChat.Events.EventSessionInfo)message;
                        break;

                    default:
                        break;

                    #endregion Events
                }

                if (popupEvent != null)
                {
                    IUserActivity userActivity = new UserActivityData();

                    #region Collect Lead Data

                    userActivity.ObjectName = this._emailUserActivityOptions.ObjectName;

                    if (callType == SFDCCallType.InboundEmail || callType == SFDCCallType.InboundEmailPulled)
                    {
                        if (this._emailUserActivityOptions.InboundCanUpdateLog)
                        {
                            userActivity.UpdateActivityLog = true;
                            userActivity.UpdateActivityLogData = this._sfdcUtility.GetUpdateActivityLogData(_emailUserActivityLog, popupEvent, callType, callDuration);
                        }
                    }
                    else if (callType == SFDCCallType.OutboundEmailSuccess || callType == SFDCCallType.OutboundEmailFailure || callType == SFDCCallType.OutboundEmailPulled)
                    {
                        if (this._emailUserActivityOptions.OutboundCanUpdateLog)
                        {
                            userActivity.UpdateActivityLog = true;
                            userActivity.UpdateActivityLogData = this._sfdcUtility.GetUpdateActivityLogData(_emailUserActivityLog, popupEvent, callType, callDuration);
                        }
                    }

                    #endregion Collect Lead Data

                    return userActivity;
                }
            }
            catch (Exception generalException)
            {
                this._logger.Error("GetEmailUpdateUserAcitivityData : Error occurred while reading Lead Data : " + generalException.ToString());
            }
            return null;
        }

        #endregion GetEmailUpdateUserAcitivityData

        #region GetOutboundEmailCreateUserAcitivityData

        public IUserActivity GetOutboundEmailCreateUserAcitivityData(IXNCustomData emailData, SFDCCallType callType)
        {
            try
            {
                this._logger.Info("GetEmailCreateUserAcitivityData :  Reading Email UserActivity Log Data.....");

                if (emailData != null)
                {
                    IUserActivity userActivity = new UserActivityData();

                    #region Collect userActivity Data

                    userActivity.ObjectName = this._emailUserActivityOptions.ObjectName;
                    if (callType == SFDCCallType.InboundEmail || callType == SFDCCallType.InboundEmailPulled)
                    {
                        userActivity.CreateActvityLog = this._emailUserActivityOptions.InboundCanCreateLog;
                        if (this._emailUserActivityOptions.InboundCanCreateLog && this._emailUserActivityLog != null)
                        {
                            //userActivity.ActivityLogData = this.sfdcUtility.GetCreateActivityLogData(this.emailUserActivityLog, popupEvent, callType);
                        }
                    }
                    else if (callType == SFDCCallType.OutboundEmailFailure || callType == SFDCCallType.OutboundEmailSuccess || callType == SFDCCallType.OutboundEmailPulled)
                    {
                        userActivity.CreateActvityLog = this._emailUserActivityOptions.OutboundCanCreateLog;
                        if (this._emailUserActivityOptions.OutboundCanCreateLog && this._emailUserActivityLog != null)
                        {
                            // userActivity.ActivityLogData =
                            // this.sfdcUtility.GetCreateActivityLogData(this.emailUserActivityLog,
                            // popupEvent, callType);
                        }
                    }

                    #endregion Collect userActivity Data

                    return userActivity;
                }
            }
            catch (Exception generalException)
            {
                this._logger.Error("GetChatCreateUserAcitivityData : Error occurred  : " + generalException.ToString());
            }
            return null;
        }

        #endregion GetOutboundEmailCreateUserAcitivityData

        #region GetOutboundEmailUpdateUserAcitivityData

        public IUserActivity GetOutboundEmailUpdateUserAcitivityData(IXNCustomData emailData, SFDCCallType callType, string eventName)
        {
            try
            {
                this._logger.Info("GetOutboundEmailUpdateUserAcitivityData :  Reading User Activity Update Data.....");
                if (emailData != null)
                {
                    IUserActivity userActivity = new UserActivityData();

                    #region Collect Lead Data

                    userActivity.ObjectName = this._emailUserActivityOptions.ObjectName;

                    if (callType == SFDCCallType.InboundEmail || callType == SFDCCallType.InboundEmailPulled)
                    {
                        if (_emailUserActivityOptions.InboundCanUpdateLog)
                        {
                            userActivity.UpdateActivityLog = true;
                            userActivity.UpdateActivityLogData = this._sfdcUtility.GetUpdateActivityLogData(_emailUserActivityLog, null, callType, emailData.Duration, emailData: emailData);
                            //if (!string.IsNullOrWhiteSpace(_emailUserActivityOptions.AppendActivityLogEventNames) && _emailUserActivityOptions.AppendActivityLogEventNames.Contains(eventName))
                            //    userActivity.AppendActivityLogData = this._sfdcUtility.GetUpdateActivityLogData(this._emailUserActivityLog, null, callType, emailData.Duration, emailData: emailData, isAppendLogData: true);
                        }
                    }
                    else if (callType == SFDCCallType.OutboundEmailFailure || callType == SFDCCallType.OutboundEmailSuccess || callType == SFDCCallType.OutboundEmailPulled)
                    {
                        if (this._emailUserActivityOptions.OutboundCanUpdateLog)
                        {
                            userActivity.UpdateActivityLog = true;
                            userActivity.UpdateActivityLogData = this._sfdcUtility.GetUpdateActivityLogData(_emailUserActivityLog, null, callType
                                , emailData.Duration, emailData: emailData);
                            //if (!string.IsNullOrWhiteSpace(_emailUserActivityOptions.AppendActivityLogEventNames) && _emailUserActivityOptions.AppendActivityLogEventNames.Contains(eventName))
                            //    userActivity.AppendActivityLogData = this._sfdcUtility.GetUpdateActivityLogData(this._emailUserActivityLog, null, callType, emailData.Duration, emailData: emailData, isAppendLogData: true);
                        }
                    }

                    #endregion Collect Lead Data

                    return userActivity;
                }
            }
            catch (Exception generalException)
            {
                this._logger.Error("GetOutboundEmailUpdateUserAcitivityData : Error occurred while reading Lead Data : " + generalException.ToString());
            }
            return null;
        }

        #endregion GetOutboundEmailUpdateUserAcitivityData
    }
}