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
    using Genesyslab.Platform.Commons.Collections;
    using Pointel.Salesforce.Adapter.LogMessage;
    using Pointel.Salesforce.Adapter.Utility;
    using System;
    using System.Linq;
    using System.Net;

    /// <summary>
    /// Comment: Initializes SFDC Object's Configurations Last Modified: 25-08-2015 Created by:
    /// Pointel Inc.,
    /// </summary>
    internal class ReadProperties
    {
        #region Fields

        private static ReadProperties properties = null;

        private Log logger = null;

        #endregion Fields

        #region Constructors

        private ReadProperties()
        {
            this.logger = Log.GenInstance();
        }

        #endregion Constructors

        #region Methods

        public static ReadProperties GetInstance()
        {
            if (properties == null)
            {
                properties = new ReadProperties();
            }
            return properties;
        }

        public string CheckforDuplicates(string popupPages)
        {
            try
            {
                if (popupPages != null)
                {
                    string[] array = popupPages.Split(',');
                    if (array != null)
                    {
                        var duplicates = array
                        .GroupBy(p => p)
                        .Where(g => g.Count() > 1)
                        .Select(g => g.Key);
                        if (duplicates.Count() > 0)
                        {
                            string SFDCPages = string.Empty;
                            foreach (string page in array)
                            {
                                if (!SFDCPages.Contains(page))
                                {
                                    SFDCPages += page + ",";
                                }
                            }
                            if (SFDCPages.Length > 1)
                                return SFDCPages.Substring(0, SFDCPages.Length - 1);
                        }
                        else
                        {
                            return popupPages;
                        }
                    }
                }
            }
            catch (Exception generalException)
            {
                this.logger.Error("CheckforDuplicates : Error occurred while checking duplicates in popup pages : " + generalException.ToString());
            }
            return popupPages;
        }

        public GeneralOptions GetSFDCGeneralProperties(KeyValueCollection SFDCCollection)
        {
            try
            {
                if (SFDCCollection != null)
                {
                    #region General Config

                    GeneralOptions generalOptions = new GeneralOptions();
                    if (!String.IsNullOrEmpty(SFDCCollection.GetAsString("sfdc.enable.integration")) &&
                        SFDCCollection.GetAsString("sfdc.enable.integration").Trim().ToLower() == "false")
                    {
                        generalOptions.EnableSFDCIntegration = false;
                    }
                    else
                    {
                        generalOptions.EnableSFDCIntegration = true;
                        this.logger.Info("SFDC Integration Enabled ");
                    }
                    if (!String.IsNullOrEmpty(SFDCCollection.GetAsString("sfdc.enable.popup.channel")))
                    {
                        generalOptions.SFDCPopupChannels = SFDCCollection.GetAsString("sfdc.enable.popup.channel").Trim();
                    }
                    else
                    {
                        generalOptions.SFDCPopupChannels = "voice";
                        this.logger.Info("Default value taken for SFDC Popup Channels : voice ");
                    }
                    if (!String.IsNullOrEmpty(SFDCCollection.GetAsString("sfdc.integration.port")))
                    {
                        try
                        {
                            generalOptions.SFDCConnectPort = int.Parse(SFDCCollection.GetAsString("sfdc.integration.port").Trim());
                        }
                        catch (Exception generalException)
                        {
                            this.logger.Error("Error Occurred while parsing Port Number : " + SFDCCollection.GetAsString("sfdc.integration.port") + "\n Error Message : " + generalException.ToString());
                        }
                    }

                    if (!String.IsNullOrEmpty(SFDCCollection.GetAsString("sfdc.login-url")))
                    {
                        generalOptions.SFDCLoginURL = SFDCCollection.GetAsString("sfdc.login-url").Trim();
                    }
                    if (!String.IsNullOrEmpty(SFDCCollection.GetAsString("sfdc.popup.object-names")))
                    {
                        try
                        {
                            generalOptions.SFDCPopupPages = CheckforDuplicates(SFDCCollection.GetAsString("sfdc.popup.object-names").Trim().ToLower()).Split(',');
                            this.logger.Info("SFDC Popup Pages : " + SFDCCollection.GetAsString("sfdc.popup.object-names"));
                        }
                        catch (Exception generalException)
                        {
                            this.logger.Error("Error occurred while reading Popup Object Names " + generalException.ToString());
                        }
                    }
                    else
                        this.logger.Info("SFDC Popup Pages is Empty ");

                    if (!String.IsNullOrEmpty(SFDCCollection.GetAsString("sfdc.screen.popup")))
                    {
                        if (SFDCCollection.GetAsString("sfdc.screen.popup").Trim().ToLower() == "iws")
                            generalOptions.SFDCPopupContainer = "iws";
                        else if (SFDCCollection.GetAsString("sfdc.screen.popup").Trim().ToLower() == "aid")
                            generalOptions.SFDCPopupContainer = "aid";
                        else
                            generalOptions.SFDCPopupContainer = "browser";
                    }
                    else
                    {
                        generalOptions.SFDCPopupContainer = "browser";
                    }
                    if (!String.IsNullOrEmpty(SFDCCollection.GetAsString("sfdc.browser-type")) && SFDCCollection.GetAsString("sfdc.browser-type").Trim().ToLower() == "ie")
                    {
                        generalOptions.SFDCPopupBrowserName = "ie";
                    }
                    else
                    {
                        generalOptions.SFDCPopupBrowserName = "chrome";
                    }

                    if (!String.IsNullOrEmpty(SFDCCollection.GetAsString("sfdc.enable.address-bar")) &&
                        SFDCCollection.GetAsString("sfdc.enable.address-bar").Trim().ToLower() == "true")
                    {
                        generalOptions.EnableAddressbar = true;
                    }

                    if (!String.IsNullOrEmpty(SFDCCollection.GetAsString("sfdc.enable.status-bar")) &&
                        SFDCCollection.GetAsString("sfdc.enable.status-bar").Trim().ToLower() == "true")
                    {
                        generalOptions.EnableStatusbar = true;
                    }

                    if (!String.IsNullOrEmpty(SFDCCollection.GetAsString("sfdc.search-field")))
                    {
                        generalOptions.SearchField = SFDCCollection.GetAsString("sfdc.search-field");
                    }

                    if (!String.IsNullOrEmpty(SFDCCollection.GetAsString("sfdc.display.connect-status")) &&
                        SFDCCollection.GetAsString("sfdc.display.connect-status").Trim().ToLower() == "true")
                    {
                        generalOptions.AlertSFDCConnectionStatus = true;
                        this.logger.Info("Enable Status Message for SFDC Connection State Changes : true");
                    }

                    if (!String.IsNullOrEmpty(SFDCCollection.GetAsString("sfdc.success.connect-message")))
                    {
                        generalOptions.SFDCConnectionSuccessMessage = SFDCCollection.GetAsString("sfdc.success.connect-message");
                    }
                    if (!String.IsNullOrEmpty(SFDCCollection.GetAsString("sfdc.failure.connect-message")))
                    {
                        generalOptions.SFDCConnectionFailureMessage = SFDCCollection.GetAsString("sfdc.failure.connect-message");
                    }

                    // Read Business Attribute Name
                    if (!String.IsNullOrEmpty(SFDCCollection.GetAsString("sfdc.create.activity.business-attribute")))
                    {
                        generalOptions.ActivityLogBusinessAttribute = SFDCCollection.GetAsString("sfdc.create.activity.business-attribute").Trim();
                    }

                    if (!String.IsNullOrEmpty(SFDCCollection.GetAsString("sfdc.new-record.business-attribute")))
                    {
                        generalOptions.NewRecordDataBusinessAttribute = SFDCCollection.GetAsString("sfdc.new-record.business-attribute").Trim();
                    }

                    if (!String.IsNullOrEmpty(SFDCCollection.GetAsString("sfdc.popup-object.dialed.from.desktop-app")) &&
                        SFDCCollection.GetAsString("sfdc.popup-object.dialed.from.desktop-app").Trim().ToLower() == "true")
                    {
                        generalOptions.EnablePopupDialedFromDesktop = true;
                    }

                    if (!String.IsNullOrEmpty(SFDCCollection.GetAsString("sfdc.enable.consult-call")) &&
                        SFDCCollection.GetAsString("sfdc.enable.consult-call").Trim().ToLower() == "true")
                    {
                        generalOptions.EnableConsultDialingFromSFDC = true;
                    }

                    if (!String.IsNullOrEmpty(SFDCCollection.GetAsString("sfdc.voice.dial.outbound-call.on.not-ready")) &&
                        SFDCCollection.GetAsString("sfdc.voice.dial.outbound-call.on.not-ready").Trim().ToLower() == "true")
                    {
                        generalOptions.EnableClickToDialOnNotReady = true;
                    }

                    if (!String.IsNullOrEmpty(SFDCCollection.GetAsString("sfdc.voice.out.prefix.dial-plan")))
                    {
                        generalOptions.OutboundVoiceDialPlanPrefix = SFDCCollection.GetAsString("sfdc.voice.out.prefix.dial-plan").Trim();
                    }

                    if (!String.IsNullOrEmpty(SFDCCollection.GetAsString("sfdc.voice.con.prefix.dial-plan")))
                    {
                        generalOptions.ConsultVoiceDialPlanPrefix = SFDCCollection.GetAsString("sfdc.voice.con.prefix.dial-plan").Trim();
                    }

                    //Common search attributes
                    if (!String.IsNullOrEmpty(SFDCCollection.GetAsString("sfdc.voice.inb.search.user-data.key-names")))
                    {
                        generalOptions.InboundSearchUserDataKeys = SFDCCollection.GetAsString("sfdc.voice.inb.search.user-data.key-names").Trim();
                    }

                    if (!String.IsNullOrEmpty(SFDCCollection.GetAsString("sfdc.voice.inb.search.attribute.key-names")))
                    {
                        generalOptions.InboundSearchAttributeKeys = SFDCCollection.GetAsString("sfdc.voice.inb.search.attribute.key-names").Trim();
                    }

                    if (!String.IsNullOrEmpty(SFDCCollection.GetAsString("sfdc.voice.out.search.user-data.key-names")))
                    {
                        generalOptions.OutboundSearchUserDataKeys = SFDCCollection.GetAsString("sfdc.voice.out.search.user-data.key-names").Trim();
                    }

                    if (!String.IsNullOrEmpty(SFDCCollection.GetAsString("sfdc.voice.out.search.attribute.key-names")))
                    {
                        generalOptions.OutboundSearchAttributeKeys = SFDCCollection.GetAsString("sfdc.voice.out.search.attribute.key-names").Trim();
                    }
                    else
                        generalOptions.OutboundSearchAttributeKeys = "otherdn";

                    if (!String.IsNullOrEmpty(SFDCCollection.GetAsString("sfdc.voice.con.search.user-data.key-names")))
                    {
                        generalOptions.ConsultSearchUserDataKeys = SFDCCollection.GetAsString("sfdc.voice.con.search.user-data.key-names").Trim();
                    }

                    if (!String.IsNullOrEmpty(SFDCCollection.GetAsString("sfdc.voice.con.search.attribute.key-names")))
                    {
                        generalOptions.ConsultSearchAttributeKeys = SFDCCollection.GetAsString("sfdc.voice.con.search.attribute.key-names").Trim();
                    }

                    if (!String.IsNullOrEmpty(SFDCCollection.GetAsString("sfdc.voice.search.phone-number.format")))
                    {
                        generalOptions.PhoneNumberSearchFormat = SFDCCollection.GetAsString("sfdc.voice.search.phone-number.format").Trim();
                    }
                    else
                    {
                        generalOptions.PhoneNumberSearchFormat = "xxxxxxxxxx,xxx-xxx-xxxx,(xxx)-xxx-xxxx,(xxx)xxx-xxxx,(xxx)xxxxxxx";
                    }

                    if (!String.IsNullOrEmpty(SFDCCollection.GetAsString("sfdc.voice.search.priority")))
                    {
                        if (SFDCCollection.GetAsString("sfdc.voice.search.priority").Trim().ToLower() == "attribute")
                            generalOptions.VoiceSearchPriority = "attribute";
                        else if (SFDCCollection.GetAsString("sfdc.voice.search.priority").Trim().ToLower() == "both")
                            generalOptions.VoiceSearchPriority = "both";
                        else
                            generalOptions.VoiceSearchPriority = "user-data";
                    }
                    else
                        generalOptions.VoiceSearchPriority = "user-data";

                    if (!String.IsNullOrEmpty(SFDCCollection.GetAsString("sfdc.voice.inb.popup-event-names")))
                    {
                        generalOptions.InboundPopupEvent = SFDCCollection.GetAsString("sfdc.voice.inb.popup-event-names").Trim();
                    }
                    if (!String.IsNullOrEmpty(SFDCCollection.GetAsString("sfdc.voice.out.popup-event-names")))
                    {
                        generalOptions.OutboundPopupEvent = SFDCCollection.GetAsString("sfdc.voice.out.popup-event-names").Trim();
                    }
                    if (!String.IsNullOrEmpty(SFDCCollection.GetAsString("sfdc.voice.con.popup-event-names")))
                    {
                        generalOptions.ConsultPopupEvent = SFDCCollection.GetAsString("sfdc.voice.con.popup-event-names").Trim();
                    }
                    if (!String.IsNullOrEmpty(SFDCCollection.GetAsString("sfdc.voice.out.fail.create.activity-log.event-names")))
                    {
                        generalOptions.OutboundFailurePopupEvent = SFDCCollection.GetAsString("sfdc.voice.out.fail.create.activity-log.event-names").Trim();
                    }
                    if (!String.IsNullOrEmpty(SFDCCollection.GetAsString("sfdc.voice.enable.common.search")) &&
                    SFDCCollection.GetAsString("sfdc.voice.enable.common.search").Trim().ToLower() == "true")
                    {
                        generalOptions.CanUseCommonSearchData = true;
                    }

                    if (!String.IsNullOrEmpty(SFDCCollection.GetAsString("sfdc.voice.search.condition-type")) && SFDCCollection.GetAsString("sfdc.voice.search.condition-type").Trim().ToLower() == "and")
                    {
                        generalOptions.CommonSearchCondition = "AND";
                    }
                    else
                        generalOptions.CommonSearchCondition = "OR";

                    //Chrome Browser Command
                    if (!String.IsNullOrEmpty(SFDCCollection.GetAsString("sfdc.chrome.browser-command")))
                    {
                        generalOptions.ChromeBrowserCommand = SFDCCollection.GetAsString("sfdc.chrome.browser-command").Trim();
                    }
                    else
                        this.logger.Warn("Chrome Browser Command not configured...");

                    if (!String.IsNullOrEmpty(SFDCCollection.GetAsString("sfdc.chrome.browser.temp-directory")))
                    {
                        generalOptions.ChromeBrowserTempDirectory = SFDCCollection.GetAsString("sfdc.chrome.browser.temp-directory").Trim();
                    }
                    else
                        this.logger.Warn("Chrome Browser user-data directory not configured...");

                    if (!String.IsNullOrEmpty(SFDCCollection.GetAsString("sfdc.soap.service-url")))
                    {
                        generalOptions.PartnerServiceAPIUrl = SFDCCollection.GetAsString("sfdc.soap.service-url").Trim();
                    }
                    else
                    {
                        this.logger.Warn("InitializeGeneralConfig : Service URL not configured.....");
                    }
                    if (!String.IsNullOrEmpty(SFDCCollection.GetAsString("sfdc.search-page.url")))
                    {
                        generalOptions.SearchPageUrl = SFDCCollection.GetAsString("sfdc.search-page.url").Trim();
                    }
                    else
                    {
                        generalOptions.SearchPageUrl = "/_ui/search/ui/UnifiedSearchResults?searchType=2&";
                    }

                    if (!String.IsNullOrEmpty(SFDCCollection.GetAsString("sfdc.soap.api.call.time-out")))
                    {
                        try
                        {
                            generalOptions.SOAPAPICallTimeout = int.Parse(SFDCCollection.GetAsString("sfdc.soap.api.call.time-out").Trim());
                            this.logger.Info("SOAP API Call TimeOut : " + SFDCCollection.GetAsString("sfdc.soap.api.call.time-out"));
                        }
                        catch
                        {
                            this.logger.Error("Error Occurred while parsing SOAP API call Timeout ");
                        }
                    }
                    else
                    {
                        this.logger.Info("SOAP API Call Timeout not configured");
                    }
                    if (!String.IsNullOrEmpty(SFDCCollection.GetAsString("sfdc.soap.time-out.attempt")))
                    {
                        try
                        {
                            generalOptions.SOAPTimeoutRetryAttempt = int.Parse(SFDCCollection.GetAsString("sfdc.soap.time-out.attempt").Trim());
                            this.logger.Info("SOAP API Call TimeOut Retry attempts : " + SFDCCollection.GetAsString("sfdc.soap.time-out.attempt"));
                        }
                        catch
                        {
                            this.logger.Error("Error Occurred while parsing SOAP API call Timeout Retry attempts");
                        }
                    }
                    else
                    {
                        this.logger.Info("SOAP API Call Timeout Retry attempts not configured");
                    }
                    if (!String.IsNullOrEmpty(SFDCCollection.GetAsString("sfdc.soap.api.enable.error-message")) &&
                        SFDCCollection.GetAsString("sfdc.soap.api.enable.error-message").Trim().ToLower() == "true")
                    {
                        generalOptions.SOAPAPIErrorMessageDisplay = true;
                    }
                    else
                    {
                        generalOptions.SOAPAPIErrorMessageDisplay = false;
                    }
                    // update connection status all time
                    if (!String.IsNullOrEmpty(SFDCCollection.GetAsString("sfdc.enable.notify.all.state.change")) &&
                        SFDCCollection.GetAsString("sfdc.enable.notify.all.state.change").Trim().ToLower() == "false")
                    {
                        generalOptions.NotifyAllConnectionStateChange = false;
                    }
                    else
                    {
                        generalOptions.NotifyAllConnectionStateChange = true;
                    }

                    //Read SFDC TimeZone Configuration
                    if (!String.IsNullOrEmpty(SFDCCollection.GetAsString("sfdc.user.time-zone")))
                    {
                        string timezone = SFDCCollection.GetAsString("sfdc.user.time-zone").Trim();
                        if (timezone.Trim().ToUpper().Contains("USERPROFILE"))
                        {
                            SFDCHttpServer.CanGetTimeZoneFromSFDC = true;
                            generalOptions.SFDCTimeZone = "GMTZ";
                            this.logger.Info("TimeZone will be taken from Saleforce UserProfile...");
                        }
                        else
                        {
                            generalOptions.SFDCTimeZone = timezone;
                            this.logger.Info("TimeZone : " + timezone);
                        }
                    }
                    else
                    {
                        generalOptions.SFDCTimeZone = "GMTZ";
                        this.logger.Info("TimeZone not configured, default value taken :'GMTZ' ");
                    }

                    // Alert SFDC Dis Connect Notification On Call
                    if (!String.IsNullOrEmpty(SFDCCollection.GetAsString("sfdc.enable.notify.disconnect.status.on.call")) &&
                       SFDCCollection.GetAsString("sfdc.enable.notify.disconnect.status.on.call").Trim().ToLower() == "true")
                    {
                        generalOptions.Alert_SFDC_Disconnection_OnCall = true;
                    }
                    // Browser Auto close configuration
                    if (!String.IsNullOrEmpty(SFDCCollection.GetAsString("sfdc.close-browser.on.AID-close")) &&
                       SFDCCollection.GetAsString("sfdc.close-browser.on.AID-close").Trim().ToLower() == "false")
                    {
                        generalOptions.CanCloseBrowserOnWDEClose = false;
                        this.logger.Info("SFDC Browser auto-close is disabled on AID window close ");
                    }
                    else
                    {
                        generalOptions.CanCloseBrowserOnWDEClose = true;
                        this.logger.Info("SFDC Browser auto-close is enabled on AID window close ");
                    }

                    // Edit the dial Number
                    if (!String.IsNullOrEmpty(SFDCCollection.GetAsString("sfdc.enable.edit.click-to-dial.number")) &&
                       SFDCCollection.GetAsString("sfdc.enable.edit.click-to-dial.number").Trim().ToLower() == "true")
                    {
                        generalOptions.CanEditDialNo = true;
                        this.logger.Info("SFDC Edit Dial No Option is False ");
                    }
                    else
                    {
                        generalOptions.CanEditDialNo = false;
                        this.logger.Info("SFDC Edit Dial No Option is True ");
                    }
                    //Enable SFDC last ping checking using timer
                    if (!String.IsNullOrEmpty(SFDCCollection.GetAsString("sfdc.enable.timer.last-ping.check")) &&
                       SFDCCollection.GetAsString("sfdc.enable.timer.last-ping.check").Trim().ToLower() == "true")
                    {
                        generalOptions.EnableTimerForSFDCPingCheck = true;
                        this.logger.Info("Timer is enabled for checking ping status from SFDC ");
                    }
                    else
                    {
                        generalOptions.EnableTimerForSFDCPingCheck = false;
                        this.logger.Info("Timer is disabled for checking ping status from SFDC ");
                    }
                    if (!String.IsNullOrEmpty(SFDCCollection.GetAsString("sfdc.ping.check.elapsed-time")))
                    {
                        try
                        {
                            generalOptions.PingCheckElapsedTime = int.Parse(SFDCCollection.GetAsString("sfdc.ping.check.elapsed-time").Trim());
                        }
                        catch (Exception generalException)
                        {
                            this.logger.Error("Error Occurred while parsing status check elapsed time  : " + SFDCCollection.GetAsString("sfdc.ping.check.elapsed-time") + "\n Error Message : " + generalException.ToString());
                        }
                    }
                    else
                    {
                        generalOptions.PingCheckElapsedTime = 25000;
                    }

                    //Read profile level Business attribute
                    if (!String.IsNullOrEmpty(SFDCCollection.GetAsString("sfdc.profile-activity.business-attribute")))
                    {
                        generalOptions.ProfileActivityBusinessAttributeName = SFDCCollection.GetAsString("sfdc.profile-activity.business-attribute");
                    }
                    else
                    {
                        logger.Warn("Common search profile activity business attribute name is not configured");
                    }

                    //Enable/Disable multi-match popup when single match record found for the particular type
                    if (!String.IsNullOrEmpty(SFDCCollection.GetAsString("sfdc.common-search.multi-match.behavior")))
                    {
                        if (SFDCCollection.GetAsString("sfdc.common-search.multi-match.behavior").ToLower() == "search")
                            generalOptions.MultiMatchAction = MultiMatchBehaviour.SearchPage;
                        else if (SFDCCollection.GetAsString("sfdc.common-search.multi-match.behavior").ToLower() == "match-record")
                            generalOptions.MultiMatchAction = MultiMatchBehaviour.ExactMatchPopup;
                        else
                            generalOptions.MultiMatchAction = MultiMatchBehaviour.None;
                    }
                    else
                    {
                        generalOptions.MultiMatchAction = MultiMatchBehaviour.None;
                    }

                    //Enable/Disable profile activity popup for MultiMatchBehaviour.SearchPage
                    if (!String.IsNullOrEmpty(SFDCCollection.GetAsString("sfdc.common-search.multi-match.behaviour.enable.profile-activity.popup")) &&
                      SFDCCollection.GetAsString("sfdc.common-search.multi-match.behaviour.enable.profile-activity.popup").Trim().ToLower() == "true")
                    {
                        generalOptions.CanEnableMultiMatchProfileActivityPopup = true;
                    }

                    //Consult add attach data changes

                    if (SFDCCollection.ContainsKey("sfdc.voice.consult.add.attach-data.key-name") &&
                        !String.IsNullOrWhiteSpace(SFDCCollection.GetAsString("sfdc.voice.consult.add.attach-data.key-name")))
                    {
                        generalOptions.ConsultAttachDataKey = SFDCCollection.GetAsString("sfdc.voice.consult.add.attach-data.key-name");
                    }
                    else
                    {
                        generalOptions.ConsultAttachDataKey = "IsTransferedCall";
                    }

                    if (!String.IsNullOrEmpty(SFDCCollection.GetAsString("sfdc.voice.consult.subject.update.init-time")) &&
                      SFDCCollection.GetAsString("sfdc.voice.consult.subject.update.init-time").Trim().ToLower().Equals("true"))
                    {
                        generalOptions.IsEnabledConsultSubjectWithInitDateTime = true;
                    }

                    //set tls version
                    System.Net.SecurityProtocolType version = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls;
                    if (!String.IsNullOrEmpty(SFDCCollection.GetAsString("sfdc.tls.version")))
                    {
                        string[] versions = SFDCCollection.GetAsString("sfdc.tls.version").Split(',');
                        if (versions.Count() > 0)
                        {
                            foreach (string ver in versions)
                            {
                                if (ver == "1.1")
                                    version = version | SecurityProtocolType.Tls11;
                                else if (ver == "1.2")
                                    version = version | SecurityProtocolType.Tls12;
                            }
                            generalOptions.TlsVersion = version;
                        }
                        else
                        {
                            generalOptions.TlsVersion = version;
                            this.logger.Warn("InitializeGeneralConfig :TLS Version :" + SFDCCollection.GetAsString("sfdc.tls.version"));
                        }
                    }
                    else
                    {
                        generalOptions.TlsVersion = version;
                        this.logger.Warn("InitializeGeneralConfig :TLS Version key is not configured, default version set");
                    }
                    System.Net.ServicePointManager.SecurityProtocol = generalOptions.TlsVersion;

                    if (SFDCCollection.ContainsKey("sfdc.host-name") &&
                      !String.IsNullOrWhiteSpace(SFDCCollection.GetAsString("sfdc.host-name")))
                    {
                        generalOptions.HostName = SFDCCollection.GetAsString("sfdc.host-name");
                    }
                    else
                    {
                        generalOptions.HostName = "http://localhost";
                        this.logger.Warn("InitializeGeneralConfig : Host name not configured taking default value as " + generalOptions.HostName);
                    }
                    //retry session id if invalid in SFDCSearch
                    if (!String.IsNullOrEmpty(SFDCCollection.GetAsString("sfdc.enable.retry-invalid-sessionid")) &&
                      SFDCCollection.GetAsString("sfdc.enable.retry-invalid-sessionid").Trim().ToLower() == "true")
                    {
                        generalOptions.CanEnableRetryInvalidSessionID = true;
                    }
                    if (generalOptions.CanEnableRetryInvalidSessionID)
                    {
                        string sessionRetry = SFDCCollection.GetAsString("sfdc.retry-invalid-sessionid.attempts");
                        int maxSession = 1;
                        if (!String.IsNullOrEmpty(sessionRetry))
                            if (!int.TryParse(sessionRetry, out maxSession))
                                this.logger.Info("InitializeGeneralConfig : Invalid session retry attempt default value take as 1");

                        generalOptions.MaxNosSessionRetryRequest = maxSession;
                    }

                    if (!String.IsNullOrEmpty(SFDCCollection.GetAsString("sfdc.enable.keepalive-sessionid")) &&
                    SFDCCollection.GetAsString("sfdc.enable.keepalive-sessionid").Trim().ToLower() == "true")
                    {
                        generalOptions.CanEnableKeepAliveSessionID = true;
                    }

                    if (generalOptions.CanEnableKeepAliveSessionID)
                    {
                        string sessionInterval = SFDCCollection.GetAsString("sfdc.keepalive-sessionid.interval");
                        int maxSessioninterval = 1;
                        if (!String.IsNullOrEmpty(sessionInterval))
                            if (!int.TryParse(sessionInterval, out maxSessioninterval))
                                this.logger.Info("InitializeGeneralConfig : session keep alive interval default value take as 15");

                        generalOptions.KeepAliveSeesionIDInterval = maxSessioninterval;
                    }
                    if (!String.IsNullOrEmpty(SFDCCollection.GetAsString("sfdc.enable.retry-search.sessionid")) &&
                   SFDCCollection.GetAsString("sfdc.enable.retry-search.sessionid").Trim().ToLower() == "true")
                    {
                        generalOptions.CanEnableRetrySearchSessionID = true;
                    }
                    if (!String.IsNullOrEmpty(SFDCCollection.GetAsString("sfdc.enable.sessionid-in-log")) &&
                  SFDCCollection.GetAsString("sfdc.enable.sessionid-in-log").Trim().ToLower() == "true")
                    {
                        generalOptions.CanEnableSessionIDInLog = true;
                    }

                    #endregion General Config

                    this.logger.Info("InitializeGeneralConfig : General SFDC Properties Initialized : " + generalOptions.ToString());
                    return generalOptions;
                }
                else
                {
                    this.logger.Error("InitializeGeneralConfig : SFDC General Configuration not found");
                }
            }
            catch (Exception generalException)
            {
                this.logger.Error("InitializeGeneralConfig : Error occurred while reading configuration details :" + generalException.Message);
            }
            return null;
        }

        public ChatOptions GetSFDCObjectChatProperties(KeyValueCollection SFDCObject, string objectName)
        {
            try
            {
                this.logger.Info("GetSFDCObjectChatProperties : Initializing SFDC Object Configurations");
                this.logger.Info("GetSFDCObjectChatProperties : Object Name : " + objectName);

                ChatOptions sfdcOptions = new ChatOptions();

                if (!String.IsNullOrEmpty(SFDCObject.GetAsString("object-name")))
                {
                    sfdcOptions.ObjectName = SFDCObject.GetAsString("object-name").Trim();
                }

                if (!String.IsNullOrEmpty(SFDCObject.GetAsString("object.url-id")))
                {
                    sfdcOptions.CustomObjectURL = SFDCObject.GetAsString("object.url-id").Trim();
                }

                if (!String.IsNullOrEmpty(SFDCObject.GetAsString("chat.search.priority")))
                {
                    if (SFDCObject.GetAsString("chat.search.priority").Trim().ToLower() == "attribute")
                        sfdcOptions.SearchPriority = "attribute";
                    else if (SFDCObject.GetAsString("chat.search.priority").Trim().ToLower() == "both")
                        sfdcOptions.SearchPriority = "both";
                    else
                        sfdcOptions.SearchPriority = "user-data";
                }
                else
                    sfdcOptions.SearchPriority = "user-data";
                //Inbound multimatch action
                if (!String.IsNullOrEmpty(SFDCObject.GetAsString("chat.inb.search.multi-match")))
                {
                    if (SFDCObject.GetAsString("chat.inb.search.multi-match").Trim().ToLower() == "openall")
                        sfdcOptions.InbMultiMatchRecordAction = "openall";
                    else if (SFDCObject.GetAsString("chat.inb.search.multi-match").Trim().ToLower() == "none")
                        sfdcOptions.InbMultiMatchRecordAction = "none";
                    else
                        sfdcOptions.InbMultiMatchRecordAction = "searchpage";
                }
                else
                    sfdcOptions.InbMultiMatchRecordAction = "searchpage";

                if (!String.IsNullOrEmpty(SFDCObject.GetAsString("chat.con.search.multi-match")))
                {
                    if (SFDCObject.GetAsString("chat.con.search.multi-match").Trim().ToLower() == "openall")
                        sfdcOptions.ConMultiMatchRecordAction = "openall";
                    else if (SFDCObject.GetAsString("chat.con.search.multi-match").Trim().ToLower() == "none")
                        sfdcOptions.ConMultiMatchRecordAction = "none";
                    else
                        sfdcOptions.ConMultiMatchRecordAction = sfdcOptions.InbMultiMatchRecordAction;
                }
                else
                    sfdcOptions.ConMultiMatchRecordAction = sfdcOptions.InbMultiMatchRecordAction;

                //Inbound no match action
                if (!String.IsNullOrEmpty(SFDCObject.GetAsString("chat.inb.search.no-record")))
                {
                    if (SFDCObject.GetAsString("chat.inb.search.no-record").Trim().ToLower() == "searchpage")
                        sfdcOptions.InbNoMatchRecordAction = "searchpage";
                    else if (SFDCObject.GetAsString("chat.inb.search.no-record").Trim().ToLower() == "createnew")
                        sfdcOptions.InbNoMatchRecordAction = "createnew";
                    else if (SFDCObject.GetAsString("chat.inb.search.no-record").Trim().ToLower() == "none")
                        sfdcOptions.InbNoMatchRecordAction = "none";
                    else
                        sfdcOptions.InbNoMatchRecordAction = "opennew";
                }
                else
                    sfdcOptions.InbNoMatchRecordAction = "opennew";

                //Consult no match action
                if (!String.IsNullOrEmpty(SFDCObject.GetAsString("chat.con.search.no-record")))
                {
                    if (SFDCObject.GetAsString("chat.con.search.no-record").Trim().ToLower() == "searchpage")
                        sfdcOptions.ConNoMatchRecordAction = "searchpage";
                    else if (SFDCObject.GetAsString("chat.con.search.no-record").Trim().ToLower() == "createnew")
                        sfdcOptions.ConNoMatchRecordAction = "createnew";
                    else if (SFDCObject.GetAsString("chat.con.search.no-record").Trim().ToLower() == "none")
                        sfdcOptions.ConNoMatchRecordAction = "none";
                    else
                        sfdcOptions.ConNoMatchRecordAction = sfdcOptions.InbNoMatchRecordAction;
                }
                else
                    sfdcOptions.ConNoMatchRecordAction = sfdcOptions.InbNoMatchRecordAction;

                if (!String.IsNullOrEmpty(SFDCObject.GetAsString("chat.new-record.field-id")))
                {
                    sfdcOptions.NewrecordFieldIds = SFDCObject.GetAsString("chat.new-record.field-id").Trim();
                }

                if (!String.IsNullOrEmpty(SFDCObject.GetAsString("chat.search.condition-type")) && SFDCObject.GetAsString("chat.search.condition-type").Trim() == "and")
                {
                    sfdcOptions.SearchCondition = "AND";
                }
                else
                    sfdcOptions.SearchCondition = "OR";

                if (!String.IsNullOrEmpty(SFDCObject.GetAsString("chat.inb.search.user-data.key-names")))
                {
                    sfdcOptions.InboundSearchUserDataKeys = SFDCObject.GetAsString("chat.inb.search.user-data.key-names");
                }

                if (!String.IsNullOrEmpty(SFDCObject.GetAsString("chat.inb.search.attribute.key-names")))
                {
                    sfdcOptions.InboundSearchAttributeKeys = SFDCObject.GetAsString("chat.inb.search.attribute.key-names").Trim();
                }
                if (!String.IsNullOrEmpty(SFDCObject.GetAsString("chat.inb.popup-event-names")))
                {
                    sfdcOptions.InboundPopupEvent = SFDCObject.GetAsString("chat.inb.popup-event-names").Trim();
                }
                if (!String.IsNullOrEmpty(SFDCObject.GetAsString("chat.inb.create.activity-log")) &&
                    SFDCObject.GetAsString("chat.inb.create.activity-log").Trim().ToLower() == "false")
                {
                    sfdcOptions.InboundCanCreateLog = false;
                }
                else
                {
                    sfdcOptions.InboundCanCreateLog = true;
                }

                if (!String.IsNullOrEmpty(SFDCObject.GetAsString("chat.inb.update.activity-log")) &&
                    SFDCObject.GetAsString("chat.inb.update.activity-log").Trim().ToLower() == "true")
                {
                    sfdcOptions.InboundCanUpdateLog = true;
                }

                if (!String.IsNullOrEmpty(SFDCObject.GetAsString("chat.inb.update.activity-log.event-names")))
                {
                    sfdcOptions.InboundUpdateEvent = SFDCObject.GetAsString("chat.inb.update.activity-log.event-names").Trim();
                }

                // Consult Configurations
                if (!String.IsNullOrEmpty(SFDCObject.GetAsString("chat.con.search.user-data.key-names")))
                {
                    sfdcOptions.ConsultSearchUserDataKeys = SFDCObject.GetAsString("chat.con.search.user-data.key-names").Trim();
                }

                if (!String.IsNullOrEmpty(SFDCObject.GetAsString("chat.con.search.attribute.key-names")))
                {
                    sfdcOptions.ConsultSearchAttributeKeys = SFDCObject.GetAsString("chat.con.search.attribute.key-names").Trim();
                }

                if (!String.IsNullOrEmpty(SFDCObject.GetAsString("chat.con.popup-event-names")))
                {
                    sfdcOptions.ConsultPopupEvent = SFDCObject.GetAsString("chat.con.popup-event-names").Trim();
                }

                if (!String.IsNullOrEmpty(SFDCObject.GetAsString("chat.con.create.activity-log")) &&
                    SFDCObject.GetAsString("chat.con.create.activity-log").Trim().ToLower() == "false")
                {
                    sfdcOptions.ConsultCanCreateLog = false;
                }
                else
                {
                    sfdcOptions.ConsultCanCreateLog = true;
                }

                if (!String.IsNullOrEmpty(SFDCObject.GetAsString("chat.con.update.activity-log")) &&
                    SFDCObject.GetAsString("chat.con.update.activity-log").Trim().ToLower() == "true")
                {
                    sfdcOptions.ConsultCanUpdateLog = true;
                }

                if (!String.IsNullOrEmpty(SFDCObject.GetAsString("chat.con.update.activity-log.event-names")))
                {
                    sfdcOptions.ConsultUpdateEvent = SFDCObject.GetAsString("chat.con.update.activity-log.event-names").Trim();
                }

                if (!String.IsNullOrEmpty(SFDCObject.GetAsString("chat.max-record.open")))
                {
                    sfdcOptions.MaxNosRecordOpen = int.Parse(SFDCObject.GetAsString("chat.max-record.open").Trim());
                }
                else
                    sfdcOptions.MaxNosRecordOpen = 5;

                if (!String.IsNullOrEmpty(SFDCObject.GetAsString("chat.search.no-record.activity-log")) &&
                    SFDCObject.GetAsString("chat.search.no-record.activity-log").Trim().ToLower() == "false")
                {
                    sfdcOptions.CanCreateLogForNewRecordCreate = false;
                }
                else
                    sfdcOptions.CanCreateLogForNewRecordCreate = true;

                if (!String.IsNullOrEmpty(SFDCObject.GetAsString("chat.multi-match.records.searchpage")))
                {
                    sfdcOptions.SearchPageMode = SFDCObject.GetAsString("chat.multi-match.records.searchpage").Trim();
                }
                else
                {
                    if (!objectName.Contains("customobject"))
                        sfdcOptions.SearchPageMode = objectName;
                    else
                        sfdcOptions.SearchPageMode = sfdcOptions.CustomObjectURL;
                }

                if (!String.IsNullOrEmpty(SFDCObject.GetAsString("chat.update.new-record")) &&
                    SFDCObject.GetAsString("chat.update.new-record").Trim().ToLower() == "true")
                {
                    sfdcOptions.CanUpdateRecordData = true;
                }

                if (!String.IsNullOrEmpty(SFDCObject.GetAsString("chat.search.phone-number-format")))
                {
                    sfdcOptions.PhoneNumberSearchFormat = SFDCObject.GetAsString("chat.search.phone-number-format").Trim();
                }
                this.logger.Info("Configuration Read for " + objectName + ": " + sfdcOptions.ToString());
                return sfdcOptions;
            }
            catch (Exception generalException)
            {
                this.logger.Error("GetSFDCObjectChatProperties : Error occurred while reading SFDC Object Configurations:" + generalException.ToString());
            }
            return null;
        }

        public VoiceOptions GetSFDCObjectVoiceProperties(KeyValueCollection SFDCObject, string objectName)
        {
            try
            {
                this.logger.Info("GetSFDCObjectVoiceProperties : Initializing SFDC Object Configurations");

                this.logger.Info("GetSFDCObjectVoiceProperties : Object Name : " + objectName);

                VoiceOptions sfdcOptions = new VoiceOptions();
                if (!String.IsNullOrEmpty(SFDCObject.GetAsString("object-name")))
                {
                    sfdcOptions.ObjectName = SFDCObject.GetAsString("object-name").Trim();
                }

                if (!String.IsNullOrEmpty(SFDCObject.GetAsString("object.url-id")))
                {
                    sfdcOptions.CustomObjectURL = SFDCObject.GetAsString("object.url-id").Trim();
                }

                if (!String.IsNullOrEmpty(SFDCObject.GetAsString("voice.multi-match.records.searchpage")))
                {
                    sfdcOptions.SearchPageMode = SFDCObject.GetAsString("voice.multi-match.records.searchpage").Trim();
                }
                else
                {
                    if (!objectName.Contains("customobject"))
                        sfdcOptions.SearchPageMode = objectName;
                    else
                        sfdcOptions.SearchPageMode = sfdcOptions.CustomObjectURL;
                }

                if (!String.IsNullOrEmpty(SFDCObject.GetAsString("voice.update.new-record")) &&
                    SFDCObject.GetAsString("voice.update.new-record").Trim().ToLower() == "true")
                {
                    sfdcOptions.CanUpdateRecordData = true;
                }

                if (!String.IsNullOrEmpty(SFDCObject.GetAsString("voice.search.priority")))
                {
                    if (SFDCObject.GetAsString("voice.search.priority").Trim().ToLower() == "attribute")
                        sfdcOptions.SearchPriority = "attribute";
                    else if (SFDCObject.GetAsString("voice.search.priority").Trim().ToLower() == "both")
                        sfdcOptions.SearchPriority = "both";
                    else
                        sfdcOptions.SearchPriority = "user-data";
                }
                else
                    sfdcOptions.SearchPriority = "user-data";
                //Inbound multi match action
                if (!String.IsNullOrEmpty(SFDCObject.GetAsString("voice.inb.search.multi-match")))
                {
                    if (SFDCObject.GetAsString("voice.inb.search.multi-match").Trim().ToLower() == "openall")
                        sfdcOptions.InbMultiMatchRecordAction = "openall";
                    else if (SFDCObject.GetAsString("voice.inb.search.multi-match").Trim().ToLower() == "none")
                        sfdcOptions.InbMultiMatchRecordAction = "none";
                    else
                        sfdcOptions.InbMultiMatchRecordAction = "searchpage";
                }
                else
                    sfdcOptions.InbMultiMatchRecordAction = "searchpage";
                //Outbound multi match action
                if (!String.IsNullOrEmpty(SFDCObject.GetAsString("voice.out.search.multi-match")))
                {
                    if (SFDCObject.GetAsString("voice.out.search.multi-match").Trim().ToLower() == "openall")
                        sfdcOptions.OutMultiMatchRecordAction = "openall";
                    else if (SFDCObject.GetAsString("voice.out.search.multi-match").Trim().ToLower() == "none")
                        sfdcOptions.OutMultiMatchRecordAction = "none";
                    else
                        sfdcOptions.OutMultiMatchRecordAction = "searchpage";
                }
                else
                    sfdcOptions.OutMultiMatchRecordAction = "searchpage";
                //Consult multi match action
                if (!String.IsNullOrEmpty(SFDCObject.GetAsString("voice.con.search.multi-match")))
                {
                    if (SFDCObject.GetAsString("voice.con.search.multi-match").Trim().ToLower() == "openall")
                        sfdcOptions.ConMultiMatchRecordAction = "openall";
                    else if (SFDCObject.GetAsString("voice.con.search.multi-match").Trim().ToLower() == "none")
                        sfdcOptions.ConMultiMatchRecordAction = "none";
                    else if (SFDCObject.GetAsString("voice.con.search.multi-match").Trim().ToLower() == "searchpage")
                        sfdcOptions.ConMultiMatchRecordAction = "searchpage";
                    else
                        sfdcOptions.ConMultiMatchRecordAction = sfdcOptions.InbMultiMatchRecordAction;
                }
                else
                    sfdcOptions.ConMultiMatchRecordAction = sfdcOptions.InbMultiMatchRecordAction;

                //Inbound no match action
                if (!String.IsNullOrEmpty(SFDCObject.GetAsString("voice.inb.search.no-record")))
                {
                    if (SFDCObject.GetAsString("voice.inb.search.no-record").Trim().ToLower() == "searchpage")
                        sfdcOptions.InbNoMatchRecordAction = "searchpage";
                    else if (SFDCObject.GetAsString("voice.inb.search.no-record").Trim().ToLower() == "createnew")
                        sfdcOptions.InbNoMatchRecordAction = "createnew";
                    else if (SFDCObject.GetAsString("voice.inb.search.no-record").Trim().ToLower() == "none")
                        sfdcOptions.InbNoMatchRecordAction = "none";
                    else
                        sfdcOptions.InbNoMatchRecordAction = "opennew";
                }
                else
                    sfdcOptions.InbNoMatchRecordAction = "opennew";
                //Outbound no match action
                if (!String.IsNullOrEmpty(SFDCObject.GetAsString("voice.out.search.no-record")))
                {
                    if (SFDCObject.GetAsString("voice.out.search.no-record").Trim().ToLower() == "searchpage")
                        sfdcOptions.OutNoMatchRecordAction = "searchpage";
                    else if (SFDCObject.GetAsString("voice.out.search.no-record").Trim().ToLower() == "createnew")
                        sfdcOptions.OutNoMatchRecordAction = "createnew";
                    else if (SFDCObject.GetAsString("voice.out.search.no-record").Trim().ToLower() == "none")
                        sfdcOptions.OutNoMatchRecordAction = "none";
                    else
                        sfdcOptions.OutNoMatchRecordAction = "opennew";
                }
                else
                    sfdcOptions.OutNoMatchRecordAction = "opennew";
                //Consult no match action
                if (!String.IsNullOrEmpty(SFDCObject.GetAsString("voice.con.search.no-record")))
                {
                    if (SFDCObject.GetAsString("voice.con.search.no-record").Trim().ToLower() == "searchpage")
                        sfdcOptions.ConNoMatchRecordAction = "searchpage";
                    else if (SFDCObject.GetAsString("voice.con.search.no-record").Trim().ToLower() == "createnew")
                        sfdcOptions.ConNoMatchRecordAction = "createnew";
                    else if (SFDCObject.GetAsString("voice.con.search.no-record").Trim().ToLower() == "none")
                        sfdcOptions.ConNoMatchRecordAction = "none";
                    else if (SFDCObject.GetAsString("voice.con.search.no-record").Trim().ToLower() == "opennew")
                        sfdcOptions.ConNoMatchRecordAction = "opennew";
                    else
                        sfdcOptions.ConNoMatchRecordAction = sfdcOptions.InbNoMatchRecordAction;
                }
                else
                    sfdcOptions.ConNoMatchRecordAction = sfdcOptions.InbNoMatchRecordAction;

                //can create profile level activity log for multi match
                if (!String.IsNullOrEmpty(SFDCObject.GetAsString("voice.inb.search.enable.profile-activity")) &&
                   SFDCObject.GetAsString("voice.inb.search.enable.profile-activity").Trim().ToLower() == "true")
                {
                    sfdcOptions.InbCanCreateMultimatchActivity = true;
                }
                if (!String.IsNullOrEmpty(SFDCObject.GetAsString("voice.out.search.enable.profile-activity")) &&
                  SFDCObject.GetAsString("voice.out.search.enable.profile-activity").Trim().ToLower() == "true")
                {
                    sfdcOptions.OutCanCreateMultimatchActivity = true;
                }
                if (!String.IsNullOrEmpty(SFDCObject.GetAsString("voice.con.search.enable.profile-activity")) &&
                  SFDCObject.GetAsString("voice.con.search.enable.profile-activity").Trim().ToLower() == "true")
                {
                    sfdcOptions.ConCanCreateMultimatchActivity = true;
                }
                else
                {
                    sfdcOptions.ConCanCreateMultimatchActivity = sfdcOptions.InbCanCreateMultimatchActivity;
                    logger.Info("Consult multimatch activity enable/disable is not configured. Default value is taken from Inbound...");
                }

                //can popup profile level activity log for multi match
                if (!String.IsNullOrEmpty(SFDCObject.GetAsString("voice.inb.search.popup.profile-activity")) &&
                   SFDCObject.GetAsString("voice.inb.search.popup.profile-activity").Trim().ToLower() == "true")
                {
                    sfdcOptions.InbCanPopupMultimatchActivity = true;
                }
                if (!String.IsNullOrEmpty(SFDCObject.GetAsString("voice.out.search.popup.profile-activity")) &&
                  SFDCObject.GetAsString("voice.out.search.popup.profile-activity").Trim().ToLower() == "true")
                {
                    sfdcOptions.OutCanPopupMultimatchActivity = true;
                }
                if (!String.IsNullOrEmpty(SFDCObject.GetAsString("voice.con.search.popup.profile-activity")) &&
                  SFDCObject.GetAsString("voice.con.search.popup.profile-activity").Trim().ToLower() == "true")
                {
                    sfdcOptions.ConCanPopupMultimatchActivity = true;
                }
                else
                {
                    sfdcOptions.ConCanPopupMultimatchActivity = sfdcOptions.InbCanPopupMultimatchActivity;
                    logger.Info("Consult multimatch actvity popup is not configured. Default value is taken from Inbound...");
                }

                //can create profile level activity log for no match
                if (!String.IsNullOrEmpty(SFDCObject.GetAsString("voice.inb.search.no-record.enable.profile-activity")) &&
                   SFDCObject.GetAsString("voice.inb.search.no-record.enable.profile-activity").Trim().ToLower() == "true")
                {
                    sfdcOptions.InbCanCreateNorecordActivity = true;
                }
                if (!String.IsNullOrEmpty(SFDCObject.GetAsString("voice.out.search.no-record.enable.profile-activity")) &&
                  SFDCObject.GetAsString("voice.out.search.no-record.enable.profile-activity").Trim().ToLower() == "true")
                {
                    sfdcOptions.OutCanCreateNorecordActivity = true;
                }
                if (!String.IsNullOrEmpty(SFDCObject.GetAsString("voice.con.search.no-record.enable.profile-activity")) &&
                  SFDCObject.GetAsString("voice.con.search.no-record.enable.profile-activity").Trim().ToLower() == "true")
                {
                    sfdcOptions.ConCanCreateNorecordActivity = true;
                }
                else
                {
                    sfdcOptions.ConCanCreateNorecordActivity = sfdcOptions.InbCanCreateNorecordActivity;
                    logger.Info("Consult no match activity enable/disable is not configured. Default value is taken from Inbound...");
                }

                //can popup profile level activity log for no match
                if (!String.IsNullOrEmpty(SFDCObject.GetAsString("voice.inb.search.no-record.popup.profile-activity")) &&
                   SFDCObject.GetAsString("voice.inb.search.no-record.popup.profile-activity").Trim().ToLower() == "true")
                {
                    sfdcOptions.InbCanPopupNorecordActivity = true;
                }
                if (!String.IsNullOrEmpty(SFDCObject.GetAsString("voice.out.search.no-record.popup.profile-activity")) &&
                  SFDCObject.GetAsString("voice.out.search.no-record.popup.profile-activity").Trim().ToLower() == "true")
                {
                    sfdcOptions.OutCanPopupNorecordActivity = true;
                }
                if (!String.IsNullOrEmpty(SFDCObject.GetAsString("voice.con.search.no-record.popup.profile-activity")) &&
                  SFDCObject.GetAsString("voice.con.search.no-record.popup.profile-activity").Trim().ToLower() == "true")
                {
                    sfdcOptions.ConCanPopupNorecordActivity = true;
                }
                else
                {
                    sfdcOptions.ConCanPopupNorecordActivity = sfdcOptions.InbCanPopupNorecordActivity;
                    logger.Info("Consult no match actvity popup is not configured. Default value is taken from Inbound...");
                }

                if (!String.IsNullOrEmpty(SFDCObject.GetAsString("voice.new-record.field-id")))
                {
                    sfdcOptions.NewrecordFieldIds = SFDCObject.GetAsString("voice.new-record.field-id").Trim();
                }

                if (!String.IsNullOrEmpty(SFDCObject.GetAsString("voice.search.condition-type")) && SFDCObject.GetAsString("voice.search.condition-type").Trim().ToLower() == "and")
                {
                    sfdcOptions.SearchCondition = "AND";
                }
                else
                    sfdcOptions.SearchCondition = "OR";

                if (!String.IsNullOrEmpty(SFDCObject.GetAsString("voice.inb.search.user-data.key-names")))
                {
                    sfdcOptions.InboundSearchUserDataKeys = SFDCObject.GetAsString("voice.inb.search.user-data.key-names").Trim();
                }

                if (!String.IsNullOrEmpty(SFDCObject.GetAsString("voice.inb.search.attribute.key-names")))
                {
                    sfdcOptions.InboundSearchAttributeKeys = SFDCObject.GetAsString("voice.inb.search.attribute.key-names").Trim();
                }
                if (!String.IsNullOrEmpty(SFDCObject.GetAsString("voice.inb.popup-event-names")))
                {
                    sfdcOptions.InboundPopupEvent = SFDCObject.GetAsString("voice.inb.popup-event-names").Trim();
                }
                if (!String.IsNullOrEmpty(SFDCObject.GetAsString("voice.inb.create.activity-log")) &&
                    SFDCObject.GetAsString("voice.inb.create.activity-log").Trim().ToLower() == "false")
                {
                    sfdcOptions.InboundCanCreateLog = false;
                }
                else
                {
                    sfdcOptions.InboundCanCreateLog = true;
                }
                if (!String.IsNullOrEmpty(SFDCObject.GetAsString("voice.inb.update.activity-log")) &&
                    SFDCObject.GetAsString("voice.inb.update.activity-log").Trim().ToLower() == "true")
                {
                    sfdcOptions.InboundCanUpdateLog = true;
                }
                if (!String.IsNullOrEmpty(SFDCObject.GetAsString("voice.inb.update.activity-log.event-names")))
                {
                    sfdcOptions.InboundUpdateEvent = SFDCObject.GetAsString("voice.inb.update.activity-log.event-names").Trim();
                }
                // Outbound Configurations

                if (!String.IsNullOrEmpty(SFDCObject.GetAsString("voice.out.search.user-data.key-names")))
                {
                    sfdcOptions.OutboundSearchUserDataKeys = SFDCObject.GetAsString("voice.out.search.user-data.key-names").Trim();
                }

                if (!String.IsNullOrEmpty(SFDCObject.GetAsString("voice.out.search.attribute.key-names")))
                {
                    sfdcOptions.OutboundSearchAttributeKeys = SFDCObject.GetAsString("voice.out.search.attribute.key-names").Trim();
                }
                else
                    sfdcOptions.OutboundSearchAttributeKeys = "otherdn";

                if (!String.IsNullOrEmpty(SFDCObject.GetAsString("voice.out.popup-event-names")))
                {
                    sfdcOptions.OutboundPopupEvent = SFDCObject.GetAsString("voice.out.popup-event-names").Trim();
                }

                if (!String.IsNullOrEmpty(SFDCObject.GetAsString("voice.out.success.create.activity-log")) &&
                    SFDCObject.GetAsString("voice.out.success.create.activity-log").Trim().ToLower() == "false")
                {
                    sfdcOptions.OutboundCanCreateLog = false;
                }
                else
                {
                    sfdcOptions.OutboundCanCreateLog = true;
                }
                if (!String.IsNullOrEmpty(SFDCObject.GetAsString("voice.out.success.update.activity-log")) &&
                    SFDCObject.GetAsString("voice.out.success.update.activity-log").Trim().ToLower() == "true")
                {
                    sfdcOptions.OutboundCanUpdateLog = true;
                }

                if (!String.IsNullOrEmpty(SFDCObject.GetAsString("voice.out.success.update.activity-log.event-names")))
                {
                    sfdcOptions.OutboundUpdateEvent = SFDCObject.GetAsString("voice.out.success.update.activity-log.event-names").Trim();
                }

                if (!String.IsNullOrEmpty(SFDCObject.GetAsString("voice.out.fail.create.activity-log")) &&
                    SFDCObject.GetAsString("voice.out.fail.create.activity-log").Trim().ToLower() == "true")
                {
                    sfdcOptions.OutboundFailureCanCreateLog = true;
                }

                if (!String.IsNullOrEmpty(SFDCObject.GetAsString("voice.out.fail.create.activity-log.event-names")))
                {
                    sfdcOptions.OutboundFailurePopupEvent = SFDCObject.GetAsString("voice.out.fail.create.activity-log.event-names").Trim();
                }

                // Consult Configurations
                if (!String.IsNullOrEmpty(SFDCObject.GetAsString("voice.con.search.user-data.key-names")))
                {
                    sfdcOptions.ConsultSearchUserDataKeys = SFDCObject.GetAsString("voice.con.search.user-data.key-names").Trim();
                }

                if (!String.IsNullOrEmpty(SFDCObject.GetAsString("voice.con.search.attribute.key-names")))
                {
                    sfdcOptions.ConsultSearchAttributeKeys = SFDCObject.GetAsString("voice.con.search.attribute.key-names").Trim();
                }

                if (!String.IsNullOrEmpty(SFDCObject.GetAsString("voice.con.popup-event-names")))
                {
                    sfdcOptions.ConsultPopupEvent = SFDCObject.GetAsString("voice.con.popup-event-names").Trim();
                }

                if (!String.IsNullOrEmpty(SFDCObject.GetAsString("voice.con.success.create.activity-log.event-names")))
                {
                    sfdcOptions.ConsultSuccessLogEvent = SFDCObject.GetAsString("voice.con.success.create.activity-log.event-names").Trim();
                }

                if (!String.IsNullOrEmpty(SFDCObject.GetAsString("voice.con.success.create.activity-log")) &&
                    SFDCObject.GetAsString("voice.con.success.create.activity-log").Trim().ToLower() == "false")
                {
                    sfdcOptions.ConsultCanCreateLog = false;
                }
                else
                {
                    sfdcOptions.ConsultCanCreateLog = true;
                }

                if (!String.IsNullOrEmpty(SFDCObject.GetAsString("voice.con.success.update.activity-log")) &&
                    SFDCObject.GetAsString("voice.con.success.update.activity-log").Trim().ToLower() == "true")
                {
                    sfdcOptions.ConsultCanUpdateLog = true;
                }

                if (!String.IsNullOrEmpty(SFDCObject.GetAsString("voice.con.success.update.activity-log.event-names")))
                {
                    sfdcOptions.ConsultUpdateEvent = SFDCObject.GetAsString("voice.con.success.update.activity-log.event-names").Trim();
                }

                if (!String.IsNullOrEmpty(SFDCObject.GetAsString("voice.con.fail.create.activity-log")) &&
                    SFDCObject.GetAsString("voice.con.fail.create.activity-log").Trim().ToLower() == "false")
                {
                    sfdcOptions.ConsultFailureCanCreateLog = false;
                }
                else
                    sfdcOptions.ConsultFailureCanCreateLog = true;

                if (!String.IsNullOrEmpty(SFDCObject.GetAsString("voice.con.fail.create.activity-log.event-names")))
                {
                    sfdcOptions.ConsultFailurePopupEvent = SFDCObject.GetAsString("voice.con.fail.create.activity-log.event-names").Trim();
                }

                if (!String.IsNullOrEmpty(SFDCObject.GetAsString("voice.con.prefix.dial-plan")))
                {
                    sfdcOptions.ConsultDialPlanPrefix = SFDCObject.GetAsString("voice.con.prefix.dial-plan").Trim();
                }

                if (!String.IsNullOrEmpty(SFDCObject.GetAsString("voice.max-record.open")))
                {
                    try
                    {
                        sfdcOptions.MaxNosRecordOpen = int.Parse(SFDCObject.GetAsString("voice.max-record.open").Trim());
                    }
                    catch (Exception generalException)
                    {
                        this.logger.Error("Error occurred while reading max record open config :" + generalException.ToString());
                    }
                }
                else
                    sfdcOptions.MaxNosRecordOpen = 5;

                if (!String.IsNullOrEmpty(SFDCObject.GetAsString("voice.search.no-record.activity-log")) &&
                    SFDCObject.GetAsString("voice.search.no-record.activity-log").Trim().ToLower() == "true")
                {
                    sfdcOptions.CanCreateLogForNewRecordCreate = true;
                }

                if (!String.IsNullOrEmpty(SFDCObject.GetAsString("voice.search.phone-number.format")))
                {
                    sfdcOptions.PhoneNumberSearchFormat = SFDCObject.GetAsString("voice.search.phone-number.format").Trim();
                }
                else
                {
                    sfdcOptions.PhoneNumberSearchFormat = "xxxxxxxxxx,xxx-xxx-xxxx,(xxx)-xxx-xxxx,(xxx)xxx-xxxx,(xxx)xxxxxxx";
                }
                //Can create profile level activity for no record none scenario
                if (!String.IsNullOrEmpty(SFDCObject.GetAsString("voice.inb.search.no-record.none.activity-log")) &&
                   SFDCObject.GetAsString("voice.inb.search.no-record.none.activity-log").Trim().ToLower() == "true")
                {
                    sfdcOptions.CanCreateProfileActivityforInbNoRecord = true;
                }
                if (!String.IsNullOrEmpty(SFDCObject.GetAsString("voice.out.search.no-record.none.activity-log")) &&
                   SFDCObject.GetAsString("voice.out.search.no-record.none.activity-log").Trim().ToLower() == "true")
                {
                    sfdcOptions.CanCreateProfileActivityforOutNoRecord = true;
                }
                if (!String.IsNullOrEmpty(SFDCObject.GetAsString("voice.con.search.no-record.none.activity-log")) &&
                   SFDCObject.GetAsString("voice.con.search.no-record.none.activity-log").Trim().ToLower() == "true")
                {
                    sfdcOptions.CanCreateProfileActivityforConNoRecord = true;
                }
                if (!String.IsNullOrEmpty(SFDCObject.GetAsString("voice.search-field")))
                {
                    sfdcOptions.SearchFields = SFDCObject.GetAsString("voice.search-field").Trim();
                }

                this.logger.Info("Configuration Read for the Object  " + objectName + ": " + sfdcOptions.ToString());
                return sfdcOptions;
            }
            catch (Exception generalException)
            {
                this.logger.Error("GetSFDCObjectVoiceProperties : Error occurred while Initializing SFDC Object Configurations for the object " + objectName + " :" + generalException.ToString());
            }
            return null;
        }

        public UserActivityOptions GetSFDCUserActivityChatProperties(KeyValueCollection SFDCObject, string objectName)
        {
            try
            {
                this.logger.Info("GetSFDCUserActivityVoiceProperties : Initializing SFDC UserActivity Chat Configurations");
                this.logger.Info("GetSFDCUserActivityVoiceProperties : Object Name : " + objectName);

                UserActivityOptions sfdcOptions = new UserActivityOptions();
                if (!String.IsNullOrEmpty(SFDCObject.GetAsString("object-name")))
                {
                    sfdcOptions.ObjectName = SFDCObject.GetAsString("object-name").Trim();
                }

                if (!String.IsNullOrEmpty(SFDCObject.GetAsString("chat.inb.popup-event-names")))
                {
                    sfdcOptions.InboundPopupEvent = SFDCObject.GetAsString("chat.inb.popup-event-names").Trim();
                }
                if (!String.IsNullOrEmpty(SFDCObject.GetAsString("chat.inb.create.activity-log")) &&
                    SFDCObject.GetAsString("chat.inb.create.activity-log").Trim().ToLower() == "false")
                {
                    sfdcOptions.InboundCanCreateLog = false;
                }
                else
                {
                    sfdcOptions.InboundCanCreateLog = true;
                }
                if (!String.IsNullOrEmpty(SFDCObject.GetAsString("chat.inb.update.activity-log")) &&
                    SFDCObject.GetAsString("chat.inb.update.activity-log").Trim().ToLower() == "true")
                {
                    sfdcOptions.InboundCanUpdateLog = true;
                }
                if (!String.IsNullOrEmpty(SFDCObject.GetAsString("chat.inb.update.activity-log.event-names")))
                {
                    sfdcOptions.InboundUpdateEvent = SFDCObject.GetAsString("chat.inb.update.activity-log.event-names").Trim();
                }
                // Outbound Configurations

                if (!String.IsNullOrEmpty(SFDCObject.GetAsString("chat.out.popup-event-names")))
                {
                    sfdcOptions.OutboundPopupEvent = SFDCObject.GetAsString("chat.out.popup-event-names").Trim();
                }

                if (!String.IsNullOrEmpty(SFDCObject.GetAsString("chat.out.success.create.activity-log")) &&
                    SFDCObject.GetAsString("chat.out.success.create.activity-log").Trim().ToLower() == "false")
                {
                    sfdcOptions.OutboundCanCreateLog = false;
                }
                else
                {
                    sfdcOptions.OutboundCanCreateLog = true;
                }

                if (!String.IsNullOrEmpty(SFDCObject.GetAsString("chat.out.success.update.activity-log")) &&
                    SFDCObject.GetAsString("chat.out.success.update.activity-log").Trim().ToLower() == "true")
                {
                    sfdcOptions.OutboundCanUpdateLog = true;
                }

                if (!String.IsNullOrEmpty(SFDCObject.GetAsString("chat.out.success.update.activity-log.event-names")))
                {
                    sfdcOptions.OutboundUpdateEvent = SFDCObject.GetAsString("chat.out.success.update.activity-log.event-names").Trim();
                }

                if (!String.IsNullOrEmpty(SFDCObject.GetAsString("chat.out.fail.create.activity-log")) &&
                    SFDCObject.GetAsString("chat.out.fail.create.activity-log").Trim().ToLower() == "true")
                {
                    sfdcOptions.OutboundFailureCanCreateLog = true;
                }

                if (!String.IsNullOrEmpty(SFDCObject.GetAsString("chat.out.fail.create.activity-log.event-names")))
                {
                    sfdcOptions.OutboundFailurePopupEvent = SFDCObject.GetAsString("chat.out.fail.create.activity-log.event-names").Trim();
                }

                // Consult Configurations

                if (!String.IsNullOrEmpty(SFDCObject.GetAsString("chat.con.popup-event-names")))
                {
                    sfdcOptions.ConsultPopupEvent = SFDCObject.GetAsString("chat.con.popup-event-names").Trim();
                }

                if (!String.IsNullOrEmpty(SFDCObject.GetAsString("chat.con.success.create.activity-log.event-names")))
                {
                    sfdcOptions.ConsultSuccessLogEvent = SFDCObject.GetAsString("chat.con.success.create.activity-log.event-names").Trim();
                }

                if (!String.IsNullOrEmpty(SFDCObject.GetAsString("chat.con.success.create.activity-log")) &&
                    SFDCObject.GetAsString("chat.con.success.create.activity-log").Trim().ToLower() == "false")
                {
                    sfdcOptions.ConsultCanCreateLog = false;
                }
                else
                {
                    sfdcOptions.ConsultCanCreateLog = true;
                }

                if (!String.IsNullOrEmpty(SFDCObject.GetAsString("chat.con.success.update.activity-log")) &&
                    SFDCObject.GetAsString("chat.con.success.update.activity-log").Trim().ToLower() == "true")
                {
                    sfdcOptions.ConsultCanUpdateLog = true;
                }

                if (!String.IsNullOrEmpty(SFDCObject.GetAsString("chat.con.success.update.activity-log.event-names")))
                {
                    sfdcOptions.ConsultUpdateEvent = SFDCObject.GetAsString("chat.con.success.update.activity-log.event-names").Trim();
                }

                if (!String.IsNullOrEmpty(SFDCObject.GetAsString("chat.con.fail.create.activity-log")) &&
                    SFDCObject.GetAsString("chat.con.fail.create.activity-log").Trim().ToLower() == "true")
                {
                    sfdcOptions.ConsultFailureCanCreateLog = true;
                }

                if (!String.IsNullOrEmpty(SFDCObject.GetAsString("chat.con.fail.create.activity-log.event-names")))
                {
                    sfdcOptions.ConsultFailurePopupEvent = SFDCObject.GetAsString("chat.con.fail.create.activity-log.event-names").Trim();
                }
                this.logger.Info("Configuration Read for " + objectName + ": " + sfdcOptions.ToString());
                return sfdcOptions;
            }
            catch (Exception generalException)
            {
                this.logger.Error("GetSFDCObjectVoiceProperties : Error occurred while reading SFDC Object Configurations:" + generalException.ToString());
            }
            return null;
        }

        public UserActivityOptions GetSFDCUserActivityVoiceProperties(KeyValueCollection SFDCObject, string objectName)
        {
            try
            {
                this.logger.Info("GetSFDCUserActivityVoiceProperties : Initializing SFDC UserActivity Voice Configurations");
                this.logger.Info("GetSFDCUserActivityVoiceProperties : Object Name : " + objectName);

                UserActivityOptions sfdcOptions = new UserActivityOptions();
                if (!String.IsNullOrEmpty(SFDCObject.GetAsString("object-name")))
                {
                    sfdcOptions.ObjectName = SFDCObject.GetAsString("object-name").Trim();
                }

                if (!String.IsNullOrEmpty(SFDCObject.GetAsString("voice.inb.popup-event-names")))
                {
                    sfdcOptions.InboundPopupEvent = SFDCObject.GetAsString("voice.inb.popup-event-names").Trim();
                }
                if (!String.IsNullOrEmpty(SFDCObject.GetAsString("voice.inb.create.activity-log")) &&
                    SFDCObject.GetAsString("voice.inb.create.activity-log").Trim().ToLower() == "false")
                {
                    sfdcOptions.InboundCanCreateLog = false;
                }
                else
                {
                    sfdcOptions.InboundCanCreateLog = true;
                }
                if (!String.IsNullOrEmpty(SFDCObject.GetAsString("voice.inb.update.activity-log")) &&
                    SFDCObject.GetAsString("voice.inb.update.activity-log").Trim().ToLower() == "true")
                {
                    sfdcOptions.InboundCanUpdateLog = true;
                }
                if (!String.IsNullOrEmpty(SFDCObject.GetAsString("voice.inb.update.activity-log.event-names")))
                {
                    sfdcOptions.InboundUpdateEvent = SFDCObject.GetAsString("voice.inb.update.activity-log.event-names").Trim();
                }
                // Outbound Configurations

                if (!String.IsNullOrEmpty(SFDCObject.GetAsString("voice.out.popup-event-names")))
                {
                    sfdcOptions.OutboundPopupEvent = SFDCObject.GetAsString("voice.out.popup-event-names").Trim();
                }

                if (!String.IsNullOrEmpty(SFDCObject.GetAsString("voice.out.success.create.activity-log")) &&
                    SFDCObject.GetAsString("voice.out.success.create.activity-log").Trim().ToLower() == "false")
                {
                    sfdcOptions.OutboundCanCreateLog = false;
                }
                else
                {
                    sfdcOptions.OutboundCanCreateLog = true;
                }
                if (!String.IsNullOrEmpty(SFDCObject.GetAsString("voice.out.success.update.activity-log")) &&
                    SFDCObject.GetAsString("voice.out.success.update.activity-log").Trim().ToLower() == "true")
                {
                    sfdcOptions.OutboundCanUpdateLog = true;
                }

                if (!String.IsNullOrEmpty(SFDCObject.GetAsString("voice.out.success.update.activity-log.event-names")))
                {
                    sfdcOptions.OutboundUpdateEvent = SFDCObject.GetAsString("voice.out.success.update.activity-log.event-names").Trim();
                }

                if (!String.IsNullOrEmpty(SFDCObject.GetAsString("voice.out.fail.create.activity-log")) &&
                    SFDCObject.GetAsString("voice.out.fail.create.activity-log").Trim().ToLower() == "false")
                {
                    sfdcOptions.OutboundFailureCanCreateLog = false;
                }
                else
                {
                    sfdcOptions.OutboundFailureCanCreateLog = true;
                }

                if (!String.IsNullOrEmpty(SFDCObject.GetAsString("voice.out.fail.create.activity-log.event-names")))
                {
                    sfdcOptions.OutboundFailurePopupEvent = SFDCObject.GetAsString("voice.out.fail.create.activity-log.event-names").Trim();
                }

                // Consult Configurations

                if (!String.IsNullOrEmpty(SFDCObject.GetAsString("voice.con.popup-event-names")))
                {
                    sfdcOptions.ConsultPopupEvent = SFDCObject.GetAsString("voice.con.popup-event-names").Trim();
                }

                if (!String.IsNullOrEmpty(SFDCObject.GetAsString("voice.con.success.create.activity-log.event-names")))
                {
                    sfdcOptions.ConsultSuccessLogEvent = SFDCObject.GetAsString("voice.con.success.create.activity-log.event-names").Trim();
                }

                if (!String.IsNullOrEmpty(SFDCObject.GetAsString("voice.con.success.create.activity-log")) &&
                    SFDCObject.GetAsString("voice.con.success.create.activity-log").Trim().ToLower() == "false")
                {
                    sfdcOptions.ConsultCanCreateLog = false;
                }
                else
                {
                    sfdcOptions.ConsultCanCreateLog = true;
                }

                if (!String.IsNullOrEmpty(SFDCObject.GetAsString("voice.con.success.update.activity-log")) &&
                    SFDCObject.GetAsString("voice.con.success.update.activity-log").Trim().ToLower() == "true")
                {
                    sfdcOptions.ConsultCanUpdateLog = true;
                }

                if (!String.IsNullOrEmpty(SFDCObject.GetAsString("voice.con.success.update.activity-log.event-names")))
                {
                    sfdcOptions.ConsultUpdateEvent = SFDCObject.GetAsString("voice.con.success.update.activity-log.event-names").Trim();
                }

                if (!String.IsNullOrEmpty(SFDCObject.GetAsString("voice.con.fail.create.activity-log")) &&
                    SFDCObject.GetAsString("voice.con.fail.create.activity-log").Trim().ToLower() == "false")
                {
                    sfdcOptions.ConsultFailureCanCreateLog = false;
                }
                else
                {
                    sfdcOptions.ConsultFailureCanCreateLog = true;
                }

                if (!String.IsNullOrEmpty(SFDCObject.GetAsString("voice.con.fail.create.activity-log.event-names")))
                {
                    sfdcOptions.ConsultFailurePopupEvent = SFDCObject.GetAsString("voice.con.fail.create.activity-log.event-names").Trim();
                }
                this.logger.Info("Configuration Read for " + objectName + ": " + sfdcOptions.ToString());
                return sfdcOptions;
            }
            catch (Exception generalException)
            {
                this.logger.Error("GetSFDCObjectVoiceProperties : Error occurred while reading SFDC Object Configurations:" + generalException.ToString());
            }
            return null;
        }

        #endregion Methods
    }
}