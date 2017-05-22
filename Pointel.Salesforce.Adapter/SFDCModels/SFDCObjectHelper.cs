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

using Pointel.Salesforce.Adapter.Configurations;
using Pointel.Salesforce.Adapter.Utility;

namespace Pointel.Salesforce.Adapter.SFDCModels
{
    internal class SFDCObjectHelper
    {
        #region GetCanCreateProfileActivity for voice

        internal static bool GetCanCreateProfileActivity(SFDCCallType callType, VoiceOptions voiceOptions, bool IsNoRecord = false)
        {
            switch (callType)
            {
                case SFDCCallType.InboundVoice:
                    return IsNoRecord ? voiceOptions.InbCanCreateNorecordActivity : voiceOptions.InbCanCreateMultimatchActivity;

                case SFDCCallType.OutboundVoiceFailure:
                case SFDCCallType.OutboundVoiceSuccess:
                    return IsNoRecord ? voiceOptions.OutCanCreateNorecordActivity : voiceOptions.OutCanCreateMultimatchActivity;

                case SFDCCallType.ConsultVoiceReceived:
                    return IsNoRecord ? voiceOptions.ConCanCreateNorecordActivity : voiceOptions.ConCanCreateMultimatchActivity;
            }
            return false;
        }

        #endregion GetCanCreateProfileActivity for voice

        #region GetCanCreateProfileActivity for Chat

        internal static bool GetCanCreateProfileActivity(SFDCCallType callType, ChatOptions chatOptions, bool IsNoRecord = false)
        {
            switch (callType)
            {
                case SFDCCallType.InboundChat:
                    return IsNoRecord ? chatOptions.InbCanCreateNorecordActivity : chatOptions.InbCanCreateMultimatchActivity;

                case SFDCCallType.ConsultChatReceived:
                    return IsNoRecord ? chatOptions.ConCanCreateNorecordActivity : chatOptions.ConCanCreateMultimatchActivity;
            }
            return false;
        }

        #endregion GetCanCreateProfileActivity for Chat

        #region GetCanCreateProfileActivity for Email

        internal static bool GetCanCreateProfileActivity(SFDCCallType callType, EmailOptions emailOptions, bool IsNoRecord = false)
        {
            switch (callType)
            {
                case SFDCCallType.InboundEmail:
                case SFDCCallType.InboundEmailPulled:
                    return IsNoRecord ? emailOptions.InbCanCreateNorecordActivity : emailOptions.InbCanCreateMultimatchActivity;

                case SFDCCallType.OutboundEmailFailure:
                case SFDCCallType.OutboundEmailSuccess:
                case SFDCCallType.OutboundEmailPulled:
                    return IsNoRecord ? emailOptions.OutCanCreateNorecordActivity : emailOptions.OutCanCreateMultimatchActivity;
            }
            return false;
        }

        #endregion GetCanCreateProfileActivity for Email

        #region GetCanPopupProfileActivity for voice

        internal static bool GetCanPopupProfileActivity(SFDCCallType callType, VoiceOptions voiceOptions, bool IsNoRecord = false)
        {
            switch (callType)
            {
                case SFDCCallType.InboundVoice:
                    return IsNoRecord ? voiceOptions.InbCanPopupNorecordActivity : voiceOptions.InbCanPopupMultimatchActivity;

                case SFDCCallType.OutboundVoiceFailure:
                case SFDCCallType.OutboundVoiceSuccess:
                    return IsNoRecord ? voiceOptions.OutCanPopupNorecordActivity : voiceOptions.OutCanPopupMultimatchActivity;

                case SFDCCallType.ConsultVoiceReceived:
                    return IsNoRecord ? voiceOptions.ConCanPopupNorecordActivity : voiceOptions.ConCanPopupMultimatchActivity;
            }
            return false;
        }

        #endregion GetCanPopupProfileActivity for voice

        #region GetCanPopupProfileActivity for chat

        internal static bool GetCanPopupProfileActivity(SFDCCallType callType, ChatOptions chatOptions, bool IsNoRecord = false)
        {
            switch (callType)
            {
                case SFDCCallType.InboundChat:
                    return IsNoRecord ? chatOptions.InbCanPopupNorecordActivity : chatOptions.InbCanPopupMultimatchActivity;

                case SFDCCallType.ConsultChatReceived:
                    return IsNoRecord ? chatOptions.ConCanPopupNorecordActivity : chatOptions.ConCanPopupMultimatchActivity;
            }
            return false;
        }

        #endregion GetCanPopupProfileActivity for chat

        #region GetCanPopupProfileActivity for Email

        internal static bool GetCanPopupProfileActivity(SFDCCallType callType, EmailOptions emailOptions, bool IsNoRecord = false)
        {
            switch (callType)
            {
                case SFDCCallType.InboundEmail:
                case SFDCCallType.InboundEmailPulled:
                    return IsNoRecord ? emailOptions.InbCanPopupNorecordActivity : emailOptions.InbCanPopupMultimatchActivity;

                case SFDCCallType.OutboundEmailFailure:
                case SFDCCallType.OutboundEmailSuccess:
                case SFDCCallType.OutboundEmailPulled:
                    return IsNoRecord ? emailOptions.OutCanPopupNorecordActivity : emailOptions.OutCanPopupMultimatchActivity;
            }
            return false;
        }

        #endregion GetCanPopupProfileActivity for Email

        #region GetMultiMatchRecordAction for chat

        internal static string GetMultiMatchRecordAction(SFDCCallType calltype, ChatOptions chatOptions)
        {
            switch (calltype)
            {
                case SFDCCallType.InboundChat:
                    return chatOptions.InbMultiMatchRecordAction;

                case SFDCCallType.ConsultVoiceReceived:
                    return chatOptions.ConMultiMatchRecordAction;
            }
            return null;
        }

        #endregion GetMultiMatchRecordAction for chat

        #region GetMultiMatchRecordAction for voice

        internal static string GetMultiMatchRecordAction(SFDCCallType calltype, VoiceOptions voiceOptions)
        {
            switch (calltype)
            {
                case SFDCCallType.InboundVoice:
                    return voiceOptions.InbMultiMatchRecordAction;

                case SFDCCallType.OutboundVoiceFailure:
                case SFDCCallType.OutboundVoiceSuccess:
                    return voiceOptions.OutMultiMatchRecordAction;

                case SFDCCallType.ConsultVoiceReceived:
                    return voiceOptions.ConMultiMatchRecordAction;
            }
            return null;
        }

        #endregion GetMultiMatchRecordAction for voice

        #region GetNoRecordFoundAction  for Chat

        internal static string GetNoRecordFoundAction(SFDCCallType calltype, ChatOptions options)
        {
            switch (calltype)
            {
                case SFDCCallType.InboundChat:
                    return options.InbNoMatchRecordAction;

                case SFDCCallType.ConsultChatReceived:
                    return options.ConNoMatchRecordAction;
            }
            return null;
        }

        #endregion GetNoRecordFoundAction  for Chat

        #region GetNoRecordFoundAction for voice

        internal static string GetNoRecordFoundAction(SFDCCallType calltype, VoiceOptions options)
        {
            switch (calltype)
            {
                case SFDCCallType.InboundVoice:
                    return options.InbNoMatchRecordAction;

                case SFDCCallType.OutboundVoiceFailure:
                case SFDCCallType.OutboundVoiceSuccess:
                    return options.OutNoMatchRecordAction;

                case SFDCCallType.ConsultVoiceReceived:
                    return options.ConNoMatchRecordAction;
            }
            return null;
        }

        #endregion GetNoRecordFoundAction for voice

        #region GetMultiMatchRecordAction for Email

        internal static string GetMultiMatchRecordAction(SFDCCallType callType, EmailOptions emailOptions)
        {
            switch (callType)
            {
                case SFDCCallType.InboundEmail:
                case SFDCCallType.InboundEmailPulled:
                    return emailOptions.InbMultiMatchRecordAction;

                case SFDCCallType.OutboundEmailFailure:
                case SFDCCallType.OutboundEmailSuccess:
                case SFDCCallType.OutboundEmailPulled:
                    return emailOptions.OutMultiMatchRecordAction;
            }
            return null;
        }

        #endregion GetMultiMatchRecordAction for Email

        #region GetNoRecordFoundAction for Email

        internal static string GetNoRecordFoundAction(SFDCCallType callType, EmailOptions emailOptions)
        {
            switch (callType)
            {
                case SFDCCallType.InboundEmail:
                case SFDCCallType.InboundEmailPulled:
                    return emailOptions.InbNoMatchRecordAction;

                case SFDCCallType.OutboundEmailFailure:
                case SFDCCallType.OutboundEmailSuccess:
                case SFDCCallType.OutboundEmailPulled:
                    return emailOptions.OutNoMatchRecordAction;
            }
            return null;
        }

        #endregion GetNoRecordFoundAction for Email
    }
}