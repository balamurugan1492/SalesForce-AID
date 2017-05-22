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

namespace Pointel.Salesforce.Adapter.Configurations
{
    using Pointel.Salesforce.Adapter.Utility;
    using System;
    using System.Text;

    /// <summary>
    /// Comment: Reads and Initialize SFDC General Configuration using iWS Hierarchy Last Modified:
    /// 25-08-2015 Created by: Pointel Inc
    /// </summary>
    internal class GeneralOptions
    {
        #region Fields

        private string timeZone = string.Empty;

        #endregion Fields

        #region Properties

        public string ActivityLogBusinessAttribute
        {
            get;
            set;
        }

        public bool Alert_SFDC_Disconnection_OnCall
        {
            get;
            set;
        }

        public bool AlertSFDCConnectionStatus
        {
            get;
            set;
        }

        public bool CanCloseBrowserOnWDEClose
        {
            get;
            set;
        }

        public bool CanEditDialNo
        {
            get;
            set;
        }

        public bool CanEnableKeepAliveSessionID
        {
            get;
            set;
        }

        public bool CanEnableMultiMatchProfileActivityPopup
        {
            get;
            set;
        }

        public bool CanEnableRetryInvalidSessionID
        {
            get;
            set;
        }

        public bool CanEnableRetrySearchSessionID
        {
            get;
            set;
        }

        public bool CanEnableSessionIDInLog
        {
            get;
            set;
        }

        public bool CanUseCommonSearchData
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

        public string CommonSearchCondition
        {
            get;
            set;
        }

        public string ConsultAttachDataKey
        {
            get;
            set;
        }

        public string ConsultPopupEvent
        {
            get;
            set;
        }

        public string ConsultSearchAttributeKeys
        {
            get;
            set;
        }

        public string ConsultSearchUserDataKeys
        {
            get;
            set;
        }

        public string ConsultVoiceDialPlanPrefix
        {
            get;
            set;
        }

        public bool EnableAddressbar
        {
            get;
            set;
        }

        public bool EnableClickToDialOnNotReady
        {
            get;
            set;
        }

        public bool EnableConsultDialingFromSFDC
        {
            get;
            set;
        }

        public bool EnablePopupDialedFromDesktop
        {
            get;
            set;
        }

        //General SFDC Configuration
        public bool EnableSFDCIntegration
        {
            get;
            set;
        }

        public bool EnableStatusbar
        {
            get;
            set;
        }

        public bool EnableTimerForSFDCPingCheck
        {
            get;
            set;
        }

        public string HostName
        {
            get;
            set;
        }

        public string InboundPopupEvent
        {
            get;
            set;
        }

        public string InboundSearchAttributeKeys
        {
            get;
            set;
        }

        public string InboundSearchUserDataKeys
        {
            get;
            set;
        }

        public bool IsEnabledConsultSubjectWithInitDateTime
        {
            get;
            set;
        }

        public int KeepAliveSeesionIDInterval
        {
            get;
            set;
        }

        public int MaxNosSessionRetryRequest
        {
            get;
            set;
        }

        public MultiMatchBehaviour MultiMatchAction
        {
            get;
            set;
        }

        public string NewRecordDataBusinessAttribute
        {
            get;
            set;
        }

        public bool NotifyAllConnectionStateChange
        {
            get;
            set;
        }

        public string OutboundFailurePopupEvent
        {
            get;
            set;
        }

        public string OutboundPopupEvent
        {
            get;
            set;
        }

        public string OutboundSearchAttributeKeys
        {
            get;
            set;
        }

        public string OutboundSearchUserDataKeys
        {
            get;
            set;
        }

        public string OutboundVoiceDialPlanPrefix
        {
            get;
            set;
        }

        public string PartnerServiceAPIUrl
        {
            get;
            set;
        }

        public string PhoneNumberSearchFormat
        {
            get;
            set;
        }

        public int PingCheckElapsedTime
        {
            get;
            set;
        }

        public string ProfileActivityBusinessAttributeName
        {
            get;
            set;
        }

        public string SearchField
        {
            get;
            set;
        }

        public string SearchPageUrl
        {
            get;
            set;
        }

        public string SFDCConnectionFailureMessage
        {
            get;
            set;
        }

        public string SFDCConnectionSuccessMessage
        {
            get;
            set;
        }

        public int SFDCConnectPort
        {
            get;
            set;
        }

        public string SFDCLoginURL
        {
            get;
            set;
        }

        public string SFDCPopupBrowserName
        {
            get;
            set;
        }

        public string SFDCPopupChannels
        {
            get;
            set;
        }

        public string SFDCPopupContainer
        {
            get;
            set;
        }

        public string[] SFDCPopupPages
        {
            get;
            set;
        }

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

        public int SOAPAPICallTimeout
        {
            get;
            set;
        }

        public bool SOAPAPIErrorMessageDisplay
        {
            get;
            set;
        }

        public int SOAPTimeoutRetryAttempt
        {
            get;
            set;
        }

        public System.Net.SecurityProtocolType TlsVersion
        {
            get;
            set;
        }

        public string VoiceSearchPriority
        {
            get;
            set;
        }

        #endregion Properties

        #region Methods

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

        #endregion Methods
    }
}