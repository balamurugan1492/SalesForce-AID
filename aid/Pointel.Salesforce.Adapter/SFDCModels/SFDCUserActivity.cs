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
using Genesyslab.Platform.Voice.Protocols.TServer.Events;
using Pointel.Salesforce.Adapter.Configurations;
using Pointel.Salesforce.Adapter.LogMessage;
using Pointel.Salesforce.Adapter.SFDCUtils;
using Pointel.Salesforce.Adapter.Utility;
using System;

namespace Pointel.Salesforce.Adapter.SFDCModels
{
    /// <summary>
    /// Comment: Provides Activity Log Data for User Profile Level Activity
    /// Last Modified: 25-08-2015
    /// Created by: Pointel Inc
    /// </summary>
    internal class SFDCUserActivity
    {
        #region Field Declaration

        private static SFDCUserActivity sfdcUserActivity = null;
        private UserActivityOptions voiceUserActivityOptions = null;
        private UserActivityOptions chatUserActivityOptions = null;
        private KeyValueCollection voiceUserActivityLog = null;
        private KeyValueCollection chatUserActivityLog = null;
        private SFDCUtility sfdcObject = null;
        private Log logger = null;

        #endregion Field Declaration

        #region Constructor

        private SFDCUserActivity()
        {
            this.logger = Log.GenInstance();
            this.voiceUserActivityOptions = Settings.UserActivityVoiceOptions;
            this.chatUserActivityOptions = Settings.UserActivityChatOptions;
            this.voiceUserActivityLog = (Settings.VoiceActivityLogCollection.ContainsKey("useractivity")) ? Settings.VoiceActivityLogCollection["useractivity"] : null;
            this.chatUserActivityLog = (Settings.ChatActivityLogCollection.ContainsKey("useractivity")) ? Settings.ChatActivityLogCollection["useractivity"] : null;
            this.sfdcObject = SFDCUtility.GetInstance();
        }

        #endregion Constructor

        #region GetInstance Method

        public static SFDCUserActivity GetInstance()
        {
            if (sfdcUserActivity == null)
            {
                sfdcUserActivity = new SFDCUserActivity();
            }
            return sfdcUserActivity;
        }

        #endregion GetInstance Method

        #region GetVoiceCreateUserAcitivityData

        public IUserActivity GetVoiceCreateUserAcitivityData(IMessage message, SFDCCallType callType)
        {
            try
            {
                this.logger.Info("GetCreateUserAcitivityData :  Reading UserActivity Log Data.....");
                this.logger.Info("GetCreateUserAcitivityData :  Event Name : " + message.Name);
                dynamic popupEvent = Convert.ChangeType(message, message.GetType());
                if (popupEvent != null)
                {
                    IUserActivity userActivity = new UserActivityData();

                    #region Collect User Activity Data

                    userActivity.ObjectName = this.voiceUserActivityOptions.ObjectName;
                    if (callType == SFDCCallType.Inbound)
                    {
                        userActivity.CreateActvityLog = this.voiceUserActivityOptions.InboundCanCreateLog;
                        if (this.voiceUserActivityOptions.InboundCanCreateLog && this.voiceUserActivityLog != null)
                        {
                            userActivity.ActivityLogData = this.sfdcObject.GetVoiceActivityLog(this.voiceUserActivityLog, popupEvent, callType);
                        }
                    }
                    else if (callType == SFDCCallType.OutboundSuccess)
                    {
                        userActivity.CreateActvityLog = this.voiceUserActivityOptions.OutboundCanCreateLog;
                        if (this.voiceUserActivityOptions.OutboundCanCreateLog && this.voiceUserActivityLog != null)
                        {
                            userActivity.CreateActvityLog = true;
                            userActivity.ActivityLogData = this.sfdcObject.GetVoiceActivityLog(this.voiceUserActivityLog, popupEvent, callType);
                        }
                    }
                    else if (callType == SFDCCallType.OutboundFailure)
                    {
                        userActivity.CreateActvityLog = this.voiceUserActivityOptions.OutboundFailureCanCreateLog;
                        if (this.voiceUserActivityOptions.OutboundFailureCanCreateLog && this.voiceUserActivityLog != null)
                        {
                            userActivity.CreateActvityLog = true;
                            userActivity.ActivityLogData = this.sfdcObject.GetVoiceActivityLog(this.voiceUserActivityLog, popupEvent, callType);
                        }
                    }
                    else if (callType == SFDCCallType.ConsultSuccess || callType == SFDCCallType.ConsultReceived)
                    {
                        userActivity.CreateActvityLog = this.voiceUserActivityOptions.ConsultCanCreateLog;
                        if (this.voiceUserActivityOptions.ConsultCanCreateLog && this.voiceUserActivityLog != null)
                        {
                            userActivity.ActivityLogData = this.sfdcObject.GetVoiceActivityLog(this.voiceUserActivityLog, popupEvent, callType);
                        }
                    }
                    else if (callType == SFDCCallType.ConsultFailure)
                    {
                        userActivity.CreateActvityLog = this.voiceUserActivityOptions.ConsultFailureCanCreateLog;
                        if (this.voiceUserActivityOptions.ConsultFailureCanCreateLog && this.voiceUserActivityLog != null)
                        {
                            userActivity.ActivityLogData = this.sfdcObject.GetVoiceActivityLog(this.voiceUserActivityLog, popupEvent, callType);
                        }
                    }

                    #endregion Collect User Activity Data

                    return userActivity;
                }
            }
            catch (Exception generalException)
            {
                this.logger.Error("GetCreateUserAcitivityData : Error occurred  : " + generalException.ToString());
            }
            return null;
        }

        #endregion GetVoiceCreateUserAcitivityData

        #region GetVoiceUpdateUserAcitivityData

        public IUserActivity GetVoiceUpdateUserAcitivityData(IMessage message, SFDCCallType callType, string callDuration)
        {
            try
            {
                this.logger.Info("GetUpdateUserAcitivityData :  Reading UserActivity Log Data.....");
                this.logger.Info("GetUpdateUserAcitivityData :  Event Name : " + message.Name);
                dynamic popupEvent = Convert.ChangeType(message, message.GetType());

                if (popupEvent != null)
                {
                    IUserActivity userActivity = new UserActivityData();

                    #region Collect Lead Data

                    userActivity.ObjectName = this.voiceUserActivityOptions.ObjectName;
                    if (callType == SFDCCallType.Inbound)
                    {
                        userActivity.UpdateActivityLog = this.voiceUserActivityOptions.InboundCanUpdateLog;
                        if (this.voiceUserActivityOptions.InboundCanUpdateLog && this.voiceUserActivityLog != null)
                        {
                            userActivity.UpdateActivityLogData = this.sfdcObject.GetVoiceUpdateActivityLog(this.voiceUserActivityLog, popupEvent, callType, callDuration);
                        }
                    }
                    else if (callType == SFDCCallType.OutboundSuccess || callType == SFDCCallType.OutboundFailure)
                    {
                        userActivity.UpdateActivityLog = this.voiceUserActivityOptions.OutboundCanUpdateLog;
                        if (this.voiceUserActivityOptions.OutboundCanUpdateLog && this.voiceUserActivityLog != null)
                        {
                            userActivity.UpdateActivityLogData = this.sfdcObject.GetVoiceUpdateActivityLog(this.voiceUserActivityLog, popupEvent, callType, callDuration);
                        }
                    }
                    else if (callType == SFDCCallType.ConsultSuccess || callType == SFDCCallType.ConsultFailure || callType == SFDCCallType.ConsultReceived)
                    {
                        userActivity.UpdateActivityLog = this.voiceUserActivityOptions.ConsultCanUpdateLog;
                        if (this.voiceUserActivityOptions.ConsultCanUpdateLog && this.voiceUserActivityLog != null)
                        {
                            userActivity.UpdateActivityLogData = this.sfdcObject.GetVoiceUpdateActivityLog(this.voiceUserActivityLog, popupEvent, callType, callDuration);
                        }
                    }

                    #endregion Collect Lead Data

                    return userActivity;
                }
            }
            catch (Exception generalException)
            {
                this.logger.Error("GetInboundLeadPopupData : Error occurred while reading Lead Data on EventRinging Event : " + generalException.ToString());
            }
            return null;
        }

        #endregion GetVoiceUpdateUserAcitivityData

        #region GetChatCreateUserAcitivityData

        public IUserActivity GetChatCreateUserAcitivityData(IMessage message, SFDCCallType callType)
        {
            try
            {
                this.logger.Info("GetChatCreateUserAcitivityData :  Reading Chat UserActivity Log Data.....");
                this.logger.Info("GetChatCreateUserAcitivityData :  Event Name : " + message.Name);
                dynamic popupEvent = Convert.ChangeType(message, message.GetType());
                if (popupEvent != null)
                {
                    IUserActivity userActivity = new UserActivityData();

                    #region Collect Lead Data

                    userActivity.ObjectName = this.chatUserActivityOptions.ObjectName;
                    if (callType == SFDCCallType.InboundChat)
                    {
                        userActivity.CreateActvityLog = this.chatUserActivityOptions.InboundCanCreateLog;
                        if (this.chatUserActivityOptions.InboundCanCreateLog && this.chatUserActivityLog != null)
                        {
                            userActivity.ActivityLogData = this.sfdcObject.GetChatActivityLog(this.chatUserActivityLog, popupEvent, callType);
                        }
                    }
                    else if (callType == SFDCCallType.ConsultChatSuccess || callType == SFDCCallType.ConsultChatFailure || callType == SFDCCallType.ConsultChatReceived)
                    {
                        userActivity.CreateActvityLog = this.chatUserActivityOptions.ConsultCanCreateLog;
                        if (this.chatUserActivityOptions.ConsultCanCreateLog && this.chatUserActivityLog != null)
                        {
                            userActivity.ActivityLogData = this.sfdcObject.GetChatActivityLog(this.chatUserActivityLog, popupEvent, callType);
                        }
                    }

                    #endregion Collect Lead Data

                    return userActivity;
                }
            }
            catch (Exception generalException)
            {
                this.logger.Error("GetChatCreateUserAcitivityData : Error occurred  : " + generalException.ToString());
            }
            return null;
        }

        #endregion GetChatCreateUserAcitivityData

        #region GetChatUpdateUserAcitivityData

        public IUserActivity GetChatUpdateUserAcitivityData(IMessage message, SFDCCallType callType, string callDuration, string chatContent)
        {
            try
            {
                this.logger.Info("GetChatUpdateUserAcitivityData :  Reading User Activity Update Data.....");
                this.logger.Info("GetChatUpdateUserAcitivityData :  Event Name : " + message.Name);
                dynamic popupEvent = Convert.ChangeType(message, message.GetType());

                if (popupEvent != null)
                {
                    IUserActivity userActivity = new UserActivityData();

                    #region Collect Lead Data

                    userActivity.ObjectName = this.voiceUserActivityOptions.ObjectName;

                    if (callType == SFDCCallType.InboundChat)
                    {
                        if (this.voiceUserActivityOptions.InboundCanUpdateLog)
                        {
                            userActivity.UpdateActivityLog = true;
                            userActivity.UpdateActivityLogData = this.sfdcObject.GetChatUpdateActivityLog(chatUserActivityLog, popupEvent, callType, callDuration, chatContent);
                        }
                    }
                    if (callType == SFDCCallType.ConsultChatReceived)
                    {
                        if (this.voiceUserActivityOptions.ConsultCanUpdateLog)
                        {
                            userActivity.UpdateActivityLog = true;
                            userActivity.UpdateActivityLogData = this.sfdcObject.GetChatUpdateActivityLog(chatUserActivityLog, popupEvent, callType, callDuration, chatContent);
                        }
                    }
                    else if (callType == SFDCCallType.ConsultChatSuccess || callType == SFDCCallType.ConsultChatFailure)
                    {
                        if (this.voiceUserActivityOptions.ConsultCanUpdateLog)
                        {
                            userActivity.UpdateActivityLog = true;
                            userActivity.UpdateActivityLogData = this.sfdcObject.GetChatUpdateActivityLog(chatUserActivityLog, popupEvent, callType, callDuration, chatContent);
                        }
                    }

                    #endregion Collect Lead Data

                    return userActivity;
                }
            }
            catch (Exception generalException)
            {
                this.logger.Error("GetChatUpdateUserAcitivityData : Error occurred while reading Lead Data : " + generalException.ToString());
            }
            return null;
        }

        #endregion GetChatUpdateUserAcitivityData
    }
}