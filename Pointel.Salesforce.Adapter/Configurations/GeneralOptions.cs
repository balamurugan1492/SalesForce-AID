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

using Pointel.Salesforce.Adapter.Utility;
using System;
using System.Text;

namespace Pointel.Salesforce.Adapter.Configurations
{
    /// <summary>
    /// Comment: Reads and Initialize SFDC General Configuration using iWS Hierarchy
    /// Last Modified: 25-08-2015
    /// Created by: Pointel Inc
    /// </summary>
    ///
    internal class GeneralOptions
    {
        #region Field declaration

        //General SFDC Configuration
        public bool EnableSFDCIntegration
        {
            get;
            set;
        }

        public string SFDCPopupChannels
        {
            get;
            set;
        }

        public string SFDCLoginURL
        {
            get;
            set;
        }

        public int SFDCConnectPort
        {
            get;
            set;
        }

        public string[] SFDCPopupPages
        {
            get;
            set;
        }

        public bool AlertSFDCConnectionStatus
        {
            get;
            set;
        }

        public string SFDCConnectionSuccessMessage
        {
            get;
            set;
        }

        public string SFDCConnectionFailureMessage
        {
            get;
            set;
        }

        public string SFDCPopupContainer
        {
            get;
            set;
        }

        public string SFDCPopupBrowserName
        {
            get;
            set;
        }

        public bool EnableStatusbar
        {
            get;
            set;
        }

        public bool EnableAddressbar
        {
            get;
            set;
        }

        public bool EnablePopupDialedFromDesktop
        {
            get;
            set;
        }

        public bool EnableConsultDialingFromSFDC
        {
            get;
            set;
        }

        public string ActivityLogBusinessAttribute
        {
            get;
            set;
        }

        public string NewRecordDataBusinessAttribute
        {
            get;
            set;
        }

        public string OutboundVoiceDialPlanPrefix
        {
            get;
            set;
        }

        public string ConsultVoiceDialPlanPrefix
        {
            get;
            set;
        }

        public bool EnableClickToDialOnNotReady
        {
            get;
            set;
        }

        public string InboundSearchUserDataKeys
        {
            get;
            set;
        }

        public string InboundSearchAttributeKeys
        {
            get;
            set;
        }

        public string OutboundSearchUserDataKeys
        {
            get;
            set;
        }

        public string OutboundSearchAttributeKeys
        {
            get;
            set;
        }

        public string ConsultSearchUserDataKeys
        {
            get;
            set;
        }

        public string ConsultSearchAttributeKeys
        {
            get;
            set;
        }

        public string PhoneNumberSearchFormat
        {
            get;
            set;
        }

        public string VoiceSearchPriority
        {
            get;
            set;
        }

        public string InboundPopupEvent
        {
            get;
            set;
        }

        public string OutboundPopupEvent
        {
            get;
            set;
        }

        public string ConsultPopupEvent
        {
            get;
            set;
        }

        public string OutboundFailurePopupEvent
        {
            get;
            set;
        }

        public bool CanUseCommonSearchForVoice
        {
            get;
            set;
        }

        public bool CanUseCommonSearchForEmail
        {
            get;
            set;
        }

        public string CommonSearchConditionForVoice
        {
            get;
            set;
        }

        public string ChromeBrowserCommand
        {
            get;
            set;
        }

        public string ChromeBrowserTempDirectory
        {
            get;
            set;
        }

        public string PartnerServiceAPIUrl
        {
            get;
            set;
        }

        public string SearchPageUrl
        {
            get;
            set;
        }

        public bool SOAPAPIErrorMessageDisplay
        {
            get;
            set;
        }

        public int SOAPAPICallTimeout
        {
            get;
            set;
        }

        public int SOAPTimeoutRetryAttempt
        {
            get;
            set;
        }

        public bool NotifyAllConnectionStateChange
        {
            get;
            set;
        }

        private string timeZone = string.Empty;

        public string SFDCTimeZone
        {
            get
            {
                return timeZone;
            }
            set
            {
                if (!String.IsNullOrEmpty(value) && value.ToUpper().Contains("GMT"))
                {
                    string data = value.Trim().ToUpper().Replace("GMT", "");
                    if (data != string.Empty)
                    {
                        data = data.Trim();

                        if (data.ToUpper() == "Z" || data.Contains("-") || data.Contains("+"))
                        {
                            timeZone = data;
                        }
                        else
                        {
                            timeZone = "+" + data;
                        }
                    }
                }
                else if (timeZone != value)
                {
                    timeZone = value;
                }
            }
        }

        public bool Alert_SFDC_Disconnection_OnCall
        {
            get;
            set;
        }

        public bool EnableAutoCloseBrowserOnAppExit
        {
            get;
            set;
        }

        public bool EnableTimerForSFDCPingCheck
        {
            get;
            set;
        }

        public int PingCheckElapsedTime { get; set; }

        public MultiMatchBehaviour MultiMatchAction
        {
            get;
            set;
        }

        public bool CanEnableMultiMatchProfileActivityPopup { get; set; }

        #endregion Field declaration

        public string ConsultAttachDataKey { get; set; }

        public bool IsEnabledConsultSubjectWithInitDateTime { get; set; }

        public System.Net.SecurityProtocolType TlsVersion { get; set; }

        //Email Properties
        public bool EnableEmailWorkbin { get; set; }

        public string EmailInboundPopupEvent { get; set; }

        public string EmailOutboundPopupEvent { get; set; }

        public string EmailInboundSearchUserDataKeys { get; set; }

        public string EmailInboundSearchAttributeKeys { get; set; }

        public string EmailOutboundSearchUserDataKeys { get; set; }

        public string EmailOutboundSearchAttributeKeys { get; set; }

        public string EmailSearchPriority { get; set; }

        //chat Properties
        public string ChatInboutPopupEvent { get; set; }

        public string ChatInboundSearchUserdataKeys { get; set; }

        public string ChatInboundAttributeKeys { get; set; }

        public string ChatSearchPriority { get; set; }

        public string ChatConsultPopupEvent { get; set; }

        public string ChatConsultSearchUserdataKeys { get; set; }

        public string ChatConsultAttributeKeys { get; set; }
        public bool CanUseCommonSearchForChat { get; set; }
        public string CommonSearchConditionForChat { get; set; }

        #region ToString Method

        public override string ToString()
        {
            StringBuilder txt = new StringBuilder();
            try
            {
                foreach (System.ComponentModel.PropertyDescriptor descriptor in System.ComponentModel.TypeDescriptor.GetProperties(this))
                {
                    string name = descriptor.Name;
                    object value = descriptor.GetValue(this);
                    txt.Append(String.Format("{0} = {1}\n", name, value));
                }
            }
            catch (Exception)
            {
                return "";
            }
            return txt.ToString();
        }

        #endregion ToString Method

        public string CommonSearchConditionForEmail { get; set; }

        public string EmailAttachActivityIdKeyname { get; set; }

        public string VoiceAttachActivityIdKeyname { get; set; }

        public string ChatAttachActivityIdKeyname { get; set; }

        public bool VoiceAttachActivityId { get; set; }
        public bool EmailAttachActivityId { get; set; }
        public bool ChatAttachActivityId { get; set; }

        public string[] ListenerURLs { get; set; }

        public bool CanUseGenesysCallDuration { get; set; }

        public bool CanAddEmailAttachmentsInLog { get; set; }

        // advanced search
        public bool EnableAdvancedSearch { get; set; }

        public string AdvancedSearchSectionNames { get; set; }

        public bool EnableClick2DialNumberEditPrompt { get; set; }

        public bool EnableTruncateClick2DialNumber { get; set; }

        public int TruncateClick2DialNumberLength { get; set; }

        public bool EnableInvokeQueuedTaskOnSessionExpiry { get; set; }
    }
}