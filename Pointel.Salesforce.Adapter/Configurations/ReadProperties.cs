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
using Pointel.Salesforce.Adapter.LogMessage;
using Pointel.Salesforce.Adapter.SFDCUtils;
using Pointel.Salesforce.Adapter.Utility;
using System;
using System.Linq;
using System.Net;

namespace Pointel.Salesforce.Adapter.Configurations
{
    /// <summary>
    /// Comment: Initializes SFDC Object's Configurations Last Modified: 25-08-2015 Created by:
    /// Pointel Inc.,
    /// </summary>
    internal class ReadProperties
    {
        #region DeclarationFields

        private static ReadProperties properties = null;
        private Log logger = null;

        #endregion DeclarationFields

        #region Constructor

        private ReadProperties()
        {
            this.logger = Log.GenInstance();
        }

        #endregion Constructor

        #region GetInstance Method

        public static ReadProperties GetInstance()
        {
            if (properties == null)
            {
                properties = new ReadProperties();
            }
            return properties;
        }

        #endregion GetInstance Method

        #region GetGeneralSFDCProperties

        public GeneralOptions GetSFDCGeneralProperties(KeyValueCollection SFDCCollection)
        {
            try
            {
                if (SFDCCollection != null)
                {
                    #region General Configuration

                    GeneralOptions generalOptions = new GeneralOptions();
                    generalOptions.EnableSFDCIntegration = SFDCCollection.GetValueAsBoolean("sfdc.enable.integration", true);
                    generalOptions.SFDCPopupChannels = SFDCCollection.GetValueAsString("sfdc.enable.popup.channel", "voice", "Default value taken for SFDC Popup Channels : voice ");
                    generalOptions.SFDCConnectPort = SFDCCollection.GetValueAsInt("sfdc.integration.port", 4040);
                    generalOptions.SFDCLoginURL = SFDCCollection.GetValueAsString("sfdc.login-url", "https://login.salesforce.com/console");
                    string popupObjects = SFDCCollection.GetValueAsString("sfdc.popup.object-names", null);

                    Settings.IsVoiceEnabled = generalOptions.SFDCPopupChannels.ToLower().Contains("voice");
                    Settings.IsChatEnabled = generalOptions.SFDCPopupChannels.ToLower().Contains("chat");
                    Settings.IsEmailEnabled = generalOptions.SFDCPopupChannels.ToLower().Contains("email");

                    if (!String.IsNullOrEmpty(popupObjects))
                    {
                        try
                        {
                            generalOptions.SFDCPopupPages = CheckforDuplicates(popupObjects.ToLower()).Split(',');
                            this.logger.Info("SFDC Popup Pages : " + popupObjects);
                        }
                        catch (Exception generalException)
                        {
                            this.logger.Error("Error occurred while reading Popup Object Names " + generalException.ToString());
                        }
                    }
                    else
                        this.logger.Info("SFDC Popup Object is Empty ");

                    generalOptions.SFDCPopupContainer = SFDCCollection.GetValueAsLowerString("sfdc.screen.popup", "iws,aid", "browser");
                    generalOptions.SFDCPopupBrowserName = SFDCCollection.GetValueAsLowerString("sfdc.browser-type", "ie", "chrome");
                    generalOptions.EnableAddressbar = SFDCCollection.GetValueAsBoolean("sfdc.enable.address-bar", false);
                    generalOptions.EnableStatusbar = SFDCCollection.GetValueAsBoolean("sfdc.enable.status-bar", false);
                    generalOptions.AlertSFDCConnectionStatus = SFDCCollection.GetValueAsBoolean("sfdc.display.connect-status", false);
                    generalOptions.SFDCConnectionSuccessMessage = SFDCCollection.GetValueAsString("sfdc.success.connect-message", null);
                    generalOptions.SFDCConnectionFailureMessage = SFDCCollection.GetValueAsString("sfdc.failure.connect-message", null);
                    generalOptions.ActivityLogBusinessAttribute = SFDCCollection.GetValueAsString("sfdc.create.activity.business-attribute", null);
                    generalOptions.NewRecordDataBusinessAttribute = SFDCCollection.GetValueAsString("sfdc.new-record.business-attribute", null);

                    //Common search attributes
                    if (Settings.IsVoiceEnabled)
                    {
                        generalOptions.EnablePopupDialedFromDesktop = SFDCCollection.GetValueAsBoolean("sfdc.popup-object.dialed.from.desktop-app", false);
                        generalOptions.EnableConsultDialingFromSFDC = SFDCCollection.GetValueAsBoolean("sfdc.enable.consult-call", false);
                        generalOptions.EnableClickToDialOnNotReady = SFDCCollection.GetValueAsBoolean("sfdc.voice.dial.outbound-call.on.not-ready", false);
                        generalOptions.OutboundVoiceDialPlanPrefix = SFDCCollection.GetValueAsString("sfdc.voice.out.prefix.dial-plan", null);
                        generalOptions.ConsultVoiceDialPlanPrefix = SFDCCollection.GetValueAsString("sfdc.voice.con.prefix.dial-plan", null);
                        generalOptions.InboundSearchUserDataKeys = SFDCCollection.GetValueAsString("sfdc.voice.inb.search.user-data.key-names", null);
                        generalOptions.InboundSearchAttributeKeys = SFDCCollection.GetValueAsString("sfdc.voice.inb.search.attribute.key-names", null);
                        generalOptions.OutboundSearchUserDataKeys = SFDCCollection.GetValueAsString("sfdc.voice.out.search.user-data.key-names", null);
                        generalOptions.OutboundSearchAttributeKeys = SFDCCollection.GetValueAsString("sfdc.voice.out.search.attribute.key-names", "otherdn");
                        generalOptions.ConsultSearchUserDataKeys = SFDCCollection.GetValueAsString("sfdc.voice.con.search.user-data.key-names", null);
                        generalOptions.ConsultSearchAttributeKeys = SFDCCollection.GetValueAsString("sfdc.voice.con.search.attribute.key-names", null);
                        generalOptions.PhoneNumberSearchFormat = SFDCCollection.GetValueAsString("sfdc.voice.search.phone-number.format", "(xxx)xxx-xxxx,xxx-xxx-xxxx,(xxx)-xxx-xxxx,(xxx)xxxxxxx");
                        generalOptions.VoiceSearchPriority = SFDCCollection.GetValueAsLowerString("sfdc.voice.search.priority", "attribute,both", "user-data");
                        generalOptions.InboundPopupEvent = SFDCCollection.GetValueAsString("sfdc.voice.inb.popup-event-names", null);
                        generalOptions.OutboundPopupEvent = SFDCCollection.GetValueAsString("sfdc.voice.out.popup-event-names", null);
                        generalOptions.ConsultPopupEvent = SFDCCollection.GetValueAsString("sfdc.voice.con.popup-event-names", null);
                        generalOptions.OutboundFailurePopupEvent = SFDCCollection.GetValueAsString("sfdc.voice.out.fail.create.activity-log.event-names", null);
                        generalOptions.CanUseCommonSearchForVoice = SFDCCollection.GetValueAsBoolean("sfdc.voice.enable.common.search", false);
                        generalOptions.CommonSearchConditionForVoice = SFDCCollection.GetValueAsUpperString("sfdc.voice.search.condition-type", "and", "OR");
                        generalOptions.VoiceAttachActivityIdKeyname = SFDCCollection.GetValueAsString("sfdc.voice.attach.activity-id.key-name", "Activity_ID");
                        generalOptions.VoiceAttachActivityId = SFDCCollection.GetValueAsBoolean("sfdc.voice.enable.attach.activity-id", false);
                    }

                    //Email options for common search
                    if (Settings.IsEmailEnabled)
                    {
                        generalOptions.EmailInboundSearchUserDataKeys = SFDCCollection.GetValueAsString("sfdc.email.inb.search.user-data.key-names", null);
                        generalOptions.EmailInboundSearchAttributeKeys = SFDCCollection.GetValueAsString("sfdc.email.inb.search.attribute.key-names", null);
                        generalOptions.EmailOutboundSearchUserDataKeys = SFDCCollection.GetValueAsString("sfdc.email.out.search.user-data.key-names", null);
                        generalOptions.EmailOutboundSearchAttributeKeys = SFDCCollection.GetValueAsString("sfdc.email.out.search.attribute.key-names", "to");
                        generalOptions.EmailSearchPriority = SFDCCollection.GetValueAsLowerString("sfdc.email.search.priority", "attribute,both", "user-data");
                        generalOptions.EmailInboundPopupEvent = SFDCCollection.GetValueAsString("sfdc.email.inb.popup-event-names", null);
                        generalOptions.EmailOutboundPopupEvent = SFDCCollection.GetValueAsString("sfdc.email.out.popup-event-names", null);
                        generalOptions.CanUseCommonSearchForEmail = SFDCCollection.GetValueAsBoolean("sfdc.email.enable.common.search", false);
                        generalOptions.CommonSearchConditionForEmail = SFDCCollection.GetValueAsUpperString("sfdc.email.search.condition-type", "and", "OR");
                        generalOptions.EmailAttachActivityIdKeyname = SFDCCollection.GetValueAsString("sfdc.email.attach.activity-id.key-name", "Activity_ID");
                        generalOptions.EmailAttachActivityId = SFDCCollection.GetValueAsBoolean("sfdc.email.enable.attach.activity-id", false);
                        generalOptions.CanAddEmailAttachmentsInLog = SFDCCollection.GetValueAsBoolean("sfdc.email.enable.activity-log.add.attachments", false);
                    }
                    if (Settings.IsChatEnabled)
                    {
                        generalOptions.CanUseCommonSearchForChat = SFDCCollection.GetValueAsBoolean("sfdc.chat.enable.common.search", false);
                        generalOptions.ChatInboutPopupEvent = SFDCCollection.GetValueAsString("sfdc.chat.inb.popup-event-names", null);
                        generalOptions.ChatInboundSearchUserdataKeys = SFDCCollection.GetValueAsString("sfdc.chat.inb.search.user-data.key-names", null);
                        generalOptions.ChatInboundAttributeKeys = SFDCCollection.GetValueAsString("sfdc.chat.inb.search.attribute.key-names", null);
                        generalOptions.ChatSearchPriority = SFDCCollection.GetValueAsLowerString("sfdc.chat.search.priority", "attribute,both", "user-data");
                        generalOptions.ChatConsultPopupEvent = SFDCCollection.GetValueAsString("sfdc.chat.con.popup-event-names", null);
                        generalOptions.ChatConsultSearchUserdataKeys = SFDCCollection.GetValueAsString("sfdc.chat.con.search.user-data.key-names", null);
                        generalOptions.ChatConsultAttributeKeys = SFDCCollection.GetValueAsString("sfdc.chat.con.search.attribute.key-names", null);
                        generalOptions.CommonSearchConditionForChat = SFDCCollection.GetValueAsUpperString("sfdc.chat.search.condition-type", "and", "OR");
                        generalOptions.ChatAttachActivityId = SFDCCollection.GetValueAsBoolean("sfdc.chat.enable.attach.activity-id", false);
                        generalOptions.ChatAttachActivityIdKeyname = SFDCCollection.GetValueAsString("sfdc.chat.attach.activity-id.key-name", "Activity_ID");
                    }
                    //Chrome Browser Command
                    generalOptions.ChromeBrowserCommand = SFDCCollection.GetValueAsString("sfdc.chrome.browser-command", null, "Chrome Browser Command not configured...");
                    generalOptions.ChromeBrowserTempDirectory = SFDCCollection.GetValueAsString("sfdc.chrome.browser.temp-directory", null, "Chrome Browser user-data directory not configured...");
                    generalOptions.PartnerServiceAPIUrl = SFDCCollection.GetValueAsString("sfdc.soap.service-url", null, " SFDC SOAP API Service URL not configured");
                    generalOptions.SearchPageUrl = SFDCCollection.GetValueAsString("sfdc.search-page.url", "/_ui/search/ui/UnifiedSearchResults?searchType=2&", "SFDC Search URL not found");
                    generalOptions.SOAPAPICallTimeout = SFDCCollection.GetValueAsInt("sfdc.soap.api.call.time-out", 0);
                    generalOptions.SOAPTimeoutRetryAttempt = SFDCCollection.GetValueAsInt("sfdc.soap.time-out.attempt", 0);
                    generalOptions.SOAPAPIErrorMessageDisplay = SFDCCollection.GetValueAsBoolean("sfdc.soap.api.enable.error-message", false);

                    // update connection status all time
                    generalOptions.NotifyAllConnectionStateChange = SFDCCollection.GetValueAsBoolean("sfdc.enable.notify.all.state.change", true);
                    //Read SFDC TimeZone Configuration
                    string TimeZone = SFDCCollection.GetValueAsString("sfdc.user.time-zone", "GMTZ", "TimeZone not configured, default value taken :'GMTZ' ");
                    if (!String.IsNullOrEmpty(TimeZone))
                    {
                        if (TimeZone.ToUpper().Contains("USERPROFILE"))
                        {
                            SFDCHttpServer.CanGetTimeZoneFromSFDC = true;
                            generalOptions.SFDCTimeZone = "GMTZ";
                            this.logger.Info("TimeZone will be taken from Saleforce UserProfile...");
                        }
                    }

                    generalOptions.Alert_SFDC_Disconnection_OnCall = SFDCCollection.GetValueAsBoolean("sfdc.enable.notify.disconnect.status.on.call", false);
                    // Browser Auto close configuration
                    if (SFDCCollection.ContainsKey("sfdc.close-browser.on.app-exit"))
                    {
                        generalOptions.EnableAutoCloseBrowserOnAppExit = SFDCCollection.GetValueAsBoolean("sfdc.close-browser.on.app-exit", true);
                    }
                    else
                    {
                        generalOptions.EnableAutoCloseBrowserOnAppExit = SFDCCollection.GetValueAsBoolean("sfdc.close-browser.on.wde-close", true);
                    }

                    //Enable SFDC last ping checking using timer
                    generalOptions.EnableTimerForSFDCPingCheck = SFDCCollection.GetValueAsBoolean("sfdc.enable.timer.last-ping.check", false);
                    generalOptions.PingCheckElapsedTime = SFDCCollection.GetValueAsInt("sfdc.ping.check.elapsed-time", 25000);

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
                    generalOptions.CanEnableMultiMatchProfileActivityPopup = SFDCCollection.GetValueAsBoolean("sfdc.common-search.multi-match.behaviour.enable.profile-activity.popup", false);

                    //Consult add attach data changes
                    generalOptions.ConsultAttachDataKey = SFDCCollection.GetValueAsString("sfdc.voice.consult.add.attach-data.key-name", "IsTransferedCall");
                    generalOptions.IsEnabledConsultSubjectWithInitDateTime = SFDCCollection.GetValueAsBoolean("sfdc.voice.consult.subject.update.init-time", false);

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
                            this.logger.Warn("GetSFDCGeneralProperties :Invalid TLS Version :" + SFDCCollection.GetAsString("sfdc.tls.version"));
                        }
                    }
                    else
                    {
                        generalOptions.TlsVersion = version;
                        this.logger.Warn("GetSFDCGeneralProperties :TLS Version key is not configured");
                    }
                    System.Net.ServicePointManager.SecurityProtocol = generalOptions.TlsVersion;

                    // Adding SFDC Listener URL
                    generalOptions.ListenerURLs = SFDCCollection.GetValueAsString("sfdc.adapter.listener-url", "http://localhost:" + Convert.ToString(generalOptions.SFDCConnectPort) + "/").Split(',');
                    generalOptions.CanUseGenesysCallDuration = SFDCCollection.GetValueAsBoolean("sfdc.voice.enable.call-duration.from-wde", false);

                    generalOptions.EnableAdvancedSearch = SFDCCollection.GetValueAsBoolean("sfdc.enable.advanced-search", false);
                    generalOptions.AdvancedSearchSectionNames = SFDCCollection.GetValueAsString("sfdc.advanced-search.section-names", null);
                    generalOptions.EnableClick2DialNumberEditPrompt = SFDCCollection.GetValueAsBoolean("sfdc.enable.click-to-dial.number.edit-prompt", false);
                    generalOptions.EnableTruncateClick2DialNumber = SFDCCollection.GetValueAsBoolean("sfdc.enable.truncate.click-to-dial.number", true);
                    generalOptions.TruncateClick2DialNumberLength = SFDCCollection.GetValueAsInt("sfdc.truncate.click-to-dial.number.length", 10);
                    generalOptions.EnableInvokeQueuedTaskOnSessionExpiry = SFDCCollection.GetValueAsBoolean("sfdc.enable.session-expiry.invoke-queued-tasks", true);

                    #endregion General Configuration

                    this.logger.Info("GetSFDCGeneralProperties : General SFDC Properties Initialized : " + generalOptions.ToString());
                    return generalOptions;
                }
                else
                {
                    this.logger.Error("GetSFDCGeneralProperties : SFDC General Configuration not found");
                }
            }
            catch (Exception generalException)
            {
                this.logger.Error("GetSFDCGeneralProperties : Error occurred while reading configuration details :" + generalException.Message);
            }

            return null;
        }

        #endregion GetGeneralSFDCProperties

        #region GetSFDCObjectVoiceProperties

        public VoiceOptions GetSFDCObjectVoiceProperties(KeyValueCollection SFDCObject, string objectName)
        {
            try
            {
                this.logger.Info("GetSFDCObjectVoiceProperties : Initializing SFDC Object Configurations");

                this.logger.Info("GetSFDCObjectVoiceProperties : Object Name : " + objectName);
                VoiceOptions sfdcOptions = new VoiceOptions();
                sfdcOptions.ObjectName = SFDCObject.GetValueAsString("object-name", null);
                sfdcOptions.CustomObjectURL = SFDCObject.GetValueAsString("object.url-id", null);
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

                sfdcOptions.CanUpdateRecordData = SFDCObject.GetValueAsBoolean("voice.update.new-record", false);
                sfdcOptions.SearchPriority = SFDCObject.GetValueAsLowerString("voice.search.priority", "attribute,both", "user-data");

                //Inbound multi match action
                sfdcOptions.InbMultiMatchRecordAction = SFDCObject.GetValueAsLowerString("voice.inb.search.multi-match", "openall,none", "searchpage");
                //Outbound multi match action
                sfdcOptions.OutMultiMatchRecordAction = SFDCObject.GetValueAsLowerString("voice.out.search.multi-match", "openall,none", "searchpage");
                //Consult multi match action
                sfdcOptions.ConMultiMatchRecordAction = SFDCObject.GetValueAsLowerString("voice.con.search.multi-match", "openall,none,searchpage", sfdcOptions.InbMultiMatchRecordAction);
                //Inbound no match action
                sfdcOptions.InbNoMatchRecordAction = SFDCObject.GetValueAsLowerString("voice.inb.search.no-record", "searchpage,createnew,none", "opennew");
                //Outbound no match action
                sfdcOptions.OutNoMatchRecordAction = SFDCObject.GetValueAsLowerString("voice.out.search.no-record", "searchpage,createnew,none", "opennew");
                //Consult no match action
                sfdcOptions.ConNoMatchRecordAction = SFDCObject.GetValueAsLowerString("voice.con.search.no-record", "searchpage,createnew,opennew,none", sfdcOptions.InbNoMatchRecordAction);
                //can create profile level activity log for multi match
                sfdcOptions.InbCanCreateMultimatchActivity = SFDCObject.GetValueAsBoolean("voice.inb.search.enable.profile-activity", false);
                sfdcOptions.OutCanCreateMultimatchActivity = SFDCObject.GetValueAsBoolean("voice.out.search.enable.profile-activity", false);
                if (SFDCObject.GetValueAsBoolean("voice.con.search.enable.profile-activity", false))
                {
                    sfdcOptions.ConCanCreateMultimatchActivity = true;
                }
                else
                {
                    sfdcOptions.ConCanCreateMultimatchActivity = sfdcOptions.InbCanCreateMultimatchActivity;
                    logger.Info("Consult multimatch activity enable/disable is not configured. Default value is taken from Inbound...");
                }

                //can popup profile level activity log for multi match
                sfdcOptions.InbCanPopupMultimatchActivity = SFDCObject.GetValueAsBoolean("voice.inb.search.popup.profile-activity", false);
                sfdcOptions.OutCanPopupMultimatchActivity = SFDCObject.GetValueAsBoolean("voice.out.search.popup.profile-activity", false);
                if (SFDCObject.GetValueAsBoolean("voice.con.search.popup.profile-activity", false))
                {
                    sfdcOptions.ConCanPopupMultimatchActivity = true;
                }
                else
                {
                    sfdcOptions.ConCanPopupMultimatchActivity = sfdcOptions.InbCanPopupMultimatchActivity;
                    logger.Info("Consult multimatch activity popup is not configured. Default value is taken from Inbound...");
                }

                //can create profile level activity log for no match
                sfdcOptions.InbCanCreateNorecordActivity = SFDCObject.GetValueAsBoolean("voice.inb.search.no-record.enable.profile-activity", false);
                sfdcOptions.OutCanCreateNorecordActivity = SFDCObject.GetValueAsBoolean("voice.out.search.no-record.enable.profile-activity", false);
                if (SFDCObject.GetValueAsBoolean("voice.con.search.no-record.enable.profile-activity", false))
                {
                    sfdcOptions.ConCanCreateNorecordActivity = true;
                }
                else
                {
                    sfdcOptions.ConCanCreateNorecordActivity = sfdcOptions.InbCanCreateNorecordActivity;
                    logger.Info("Consult no match activity enable/disable is not configured. Default value is taken from Inbound...");
                }

                //can popup profile level activity log for no match
                sfdcOptions.InbCanPopupNorecordActivity = SFDCObject.GetValueAsBoolean("voice.inb.search.no-record.popup.profile-activity", false);
                sfdcOptions.OutCanPopupNorecordActivity = SFDCObject.GetValueAsBoolean("voice.out.search.no-record.popup.profile-activity", false);
                if (SFDCObject.GetValueAsBoolean("voice.con.search.no-record.popup.profile-activity", false))
                {
                    sfdcOptions.ConCanPopupNorecordActivity = true;
                }
                else
                {
                    sfdcOptions.ConCanPopupNorecordActivity = sfdcOptions.InbCanPopupNorecordActivity;
                    logger.Info("Consult no match activity popup is not configured. Default value is taken from Inbound...");
                }
                sfdcOptions.NewrecordFieldIds = SFDCObject.GetValueAsString("voice.new-record.field-id", null);
                sfdcOptions.SearchCondition = SFDCObject.GetValueAsUpperString("voice.search.condition-type", "and", "OR");
                sfdcOptions.InboundSearchUserDataKeys = SFDCObject.GetValueAsString("voice.inb.search.user-data.key-names", null);
                sfdcOptions.InboundSearchAttributeKeys = SFDCObject.GetValueAsString("voice.inb.search.attribute.key-names", null);
                sfdcOptions.InboundPopupEvent = SFDCObject.GetValueAsString("voice.inb.popup-event-names", null);
                sfdcOptions.InboundCanCreateLog = SFDCObject.GetValueAsBoolean("voice.inb.create.activity-log", true);
                sfdcOptions.InboundCanUpdateLog = SFDCObject.GetValueAsBoolean("voice.inb.update.activity-log", false);
                sfdcOptions.InboundUpdateEvent = SFDCObject.GetValueAsString("voice.inb.update.activity-log.event-names", null);
                // Outbound Configurations
                sfdcOptions.OutboundSearchUserDataKeys = SFDCObject.GetValueAsString("voice.out.search.user-data.key-names", null);
                sfdcOptions.OutboundSearchAttributeKeys = SFDCObject.GetValueAsString("voice.out.search.attribute.key-names", "otherdn");
                sfdcOptions.OutboundPopupEvent = SFDCObject.GetValueAsString("voice.out.popup-event-names", null);
                sfdcOptions.OutboundCanCreateLog = SFDCObject.GetValueAsBoolean("voice.out.success.create.activity-log", true);
                sfdcOptions.OutboundCanUpdateLog = SFDCObject.GetValueAsBoolean("voice.out.success.update.activity-log", false);
                sfdcOptions.OutboundUpdateEvent = SFDCObject.GetValueAsString("voice.out.success.update.activity-log.event-names", null);
                sfdcOptions.OutboundFailureCanCreateLog = SFDCObject.GetValueAsBoolean("voice.out.fail.create.activity-log", false);
                sfdcOptions.OutboundFailurePopupEvent = SFDCObject.GetValueAsString("voice.out.fail.create.activity-log.event-names", null);

                // Consult Configurations
                sfdcOptions.ConsultSearchUserDataKeys = SFDCObject.GetValueAsString("voice.con.search.user-data.key-names", null);
                sfdcOptions.ConsultSearchAttributeKeys = SFDCObject.GetValueAsString("voice.con.search.attribute.key-names", null);
                sfdcOptions.ConsultPopupEvent = SFDCObject.GetValueAsString("voice.con.popup-event-names", null);
                sfdcOptions.ConsultLogEvent = SFDCObject.GetValueAsString("voice.con.create.activity-log.event-names", null);
                sfdcOptions.ConsultCanCreateLog = SFDCObject.GetValueAsBoolean("voice.con.create.activity-log", true);
                sfdcOptions.ConsultCanUpdateLog = SFDCObject.GetValueAsBoolean("voice.con.update.activity-log", false);
                sfdcOptions.ConsultUpdateEvent = SFDCObject.GetValueAsString("voice.con.update.activity-log.event-names", null);
                sfdcOptions.ConsultDialPlanPrefix = SFDCObject.GetValueAsString("voice.con.prefix.dial-plan", null);
                sfdcOptions.MaxNosRecordOpen = SFDCObject.GetValueAsInt("voice.max-record.open", 5);
                sfdcOptions.CanCreateLogForNewRecordCreate = SFDCObject.GetValueAsBoolean("voice.search.no-record.activity-log", false);
                sfdcOptions.PhoneNumberSearchFormat = SFDCObject.GetValueAsString("voice.search.phone-number.format", "xxx-xxx-xxxx,(xxx)-xxx-xxxx,(xxx)xxx-xxxx,(xxx)xxxxxxx");
                //Can create profile level activity for no record none scenario
                sfdcOptions.CanCreateProfileActivityforInbNoRecord = SFDCObject.GetValueAsBoolean("voice.inb.search.no-record.none.activity-log", false);
                sfdcOptions.CanCreateProfileActivityforOutNoRecord = SFDCObject.GetValueAsBoolean("voice.out.search.no-record.none.activity-log", false);
                sfdcOptions.CanCreateProfileActivityforConNoRecord = SFDCObject.GetValueAsBoolean("voice.con.search.no-record.none.activity-log", false);
                sfdcOptions.VoiceAppendActivityLogEventNames = SFDCObject.GetValueAsString("voice.append.activity-log.event-names", null);
                this.logger.Info("Configuration Read for the Object  " + objectName + ": " + sfdcOptions.ToString());
                return sfdcOptions;
            }
            catch (Exception generalException)
            {
                this.logger.Error("GetSFDCObjectVoiceProperties : Error occurred while Initializing SFDC Object Configurations for the object " + objectName + " :" + generalException.ToString());
            }
            return null;
        }

        #endregion GetSFDCObjectVoiceProperties

        #region GetSFDCObjectChatProperties

        public ChatOptions GetSFDCObjectChatProperties(KeyValueCollection SFDCObject, string objectName)
        {
            try
            {
                this.logger.Info("GetSFDCObjectChatProperties : Initializing SFDC Object Configurations");
                this.logger.Info("GetSFDCObjectChatProperties : Object Name : " + objectName);

                ChatOptions sfdcOptions = new ChatOptions();

                sfdcOptions.ObjectName = SFDCObject.GetValueAsString("object-name", null);
                sfdcOptions.CustomObjectURL = SFDCObject.GetValueAsString("object.url-id", null);
                sfdcOptions.NewrecordFieldIds = SFDCObject.GetValueAsString("chat.new-record.field-id", null);
                sfdcOptions.SearchPriority = SFDCObject.GetValueAsLowerString("chat.search.priority", "attribute,both", "user-data");
                //Inbound multimatch action
                sfdcOptions.InbMultiMatchRecordAction = SFDCObject.GetValueAsLowerString("chat.inb.search.multi-match", "openall,none", "searchpage");
                //Consult multi match action
                sfdcOptions.ConMultiMatchRecordAction = SFDCObject.GetValueAsLowerString("chat.con.search.multi-match", "openall,none,searchpage", sfdcOptions.InbMultiMatchRecordAction);
                //Inbound no match action
                sfdcOptions.InbNoMatchRecordAction = SFDCObject.GetValueAsLowerString("chat.inb.search.no-record", "searchpage,createnew,none", "opennew");
                //can create profile level activity log for no match
                sfdcOptions.InbCanCreateNorecordActivity = SFDCObject.GetValueAsBoolean("chat.inb.search.no-record.enable.profile-activity", false);
                //can popup profile level activity log for no match
                sfdcOptions.InbCanPopupNorecordActivity = SFDCObject.GetValueAsBoolean("chat.inb.search.no-record.popup.profile-activity", false);
                //can popup profile level activity log for multi match
                sfdcOptions.InbCanPopupMultimatchActivity = SFDCObject.GetValueAsBoolean("chat.inb.search.popup.profile-activity", false);
                //Can create profile level activity for no record none scenario
                sfdcOptions.CanCreateProfileActivityforInbNoRecord = SFDCObject.GetValueAsBoolean("chat.inb.search.no-record.none.activity-log", false);

                //Consult no match action
                sfdcOptions.ConNoMatchRecordAction = SFDCObject.GetValueAsLowerString("chat.con.search.no-record", "searchpage,createnew,opennew,none", sfdcOptions.InbNoMatchRecordAction);
                sfdcOptions.NewrecordFieldIds = SFDCObject.GetValueAsString("chat.new-record.field-id", null);
                sfdcOptions.SearchCondition = SFDCObject.GetValueAsUpperString("chat.search.condition-type", "and", "OR");
                sfdcOptions.InboundSearchUserDataKeys = SFDCObject.GetValueAsString("chat.inb.search.user-data.key-names", null);
                sfdcOptions.InboundSearchAttributeKeys = SFDCObject.GetValueAsString("chat.inb.search.attribute.key-names", null);
                sfdcOptions.InboundPopupEvent = SFDCObject.GetValueAsString("chat.inb.popup-event-names", null);
                sfdcOptions.InboundCanCreateLog = SFDCObject.GetValueAsBoolean("chat.inb.create.activity-log", true);
                sfdcOptions.InboundCanUpdateLog = SFDCObject.GetValueAsBoolean("chat.inb.update.activity-log", false);
                sfdcOptions.InboundUpdateEvent = SFDCObject.GetValueAsString("chat.inb.update.activity-log.event-names", null);
                sfdcOptions.InbCanCreateMultimatchActivity = SFDCObject.GetValueAsBoolean("chat.inb.search.enable.profile-activity", false);
                // Consult Configurations
                sfdcOptions.ConsultSearchUserDataKeys = SFDCObject.GetValueAsString("chat.con.search.user-data.key-names", null);
                sfdcOptions.ConsultSearchAttributeKeys = SFDCObject.GetValueAsString("chat.con.search.attribute.key-names", null);
                sfdcOptions.ConsultPopupEvent = SFDCObject.GetValueAsString("chat.con.popup-event-names", null);
                sfdcOptions.ConsultCanCreateLog = SFDCObject.GetValueAsBoolean("chat.con.create.activity-log", true);
                sfdcOptions.ConsultCanUpdateLog = SFDCObject.GetValueAsBoolean("chat.con.update.activity-log", false);
                sfdcOptions.ConsultUpdateEvent = SFDCObject.GetValueAsString("chat.con.update.activity-log.event-names", null);
                sfdcOptions.MaxNosRecordOpen = SFDCObject.GetValueAsInt("chat.max-record.open", 5);
                sfdcOptions.CanCreateLogForNewRecordCreate = SFDCObject.GetValueAsBoolean("chat.search.no-record.activity-log", true);
                sfdcOptions.ChatAppendActivityLogEventNames = SFDCObject.GetValueAsString("chat.append.activity-log.event-names", null);
                if (SFDCObject.GetValueAsBoolean("chat.con.search.enable.profile-activity", false))
                {
                    sfdcOptions.ConCanCreateMultimatchActivity = true;
                }
                else
                {
                    sfdcOptions.ConCanCreateMultimatchActivity = sfdcOptions.InbCanCreateMultimatchActivity;
                    logger.Info("Consult multimatch activity enable/disable is not configured. Default value is taken from Inbound...");
                }

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

                sfdcOptions.CanUpdateRecordData = SFDCObject.GetValueAsBoolean("chat.update.new-record", false);
                sfdcOptions.PhoneNumberSearchFormat = SFDCObject.GetValueAsString("chat.search.phone-number-format", "xxx-xxx-xxxx,(xxx)-xxx-xxxx,(xxx)xxx-xxxx,(xxx)xxxxxxx");
                if (SFDCObject.GetValueAsBoolean("chat.con.search.no-record.enable.profile-activity", false))
                {
                    sfdcOptions.ConCanCreateNorecordActivity = true;
                }
                else
                {
                    sfdcOptions.ConCanCreateNorecordActivity = sfdcOptions.InbCanCreateNorecordActivity;
                    logger.Info("Consult no match activity enable/disable is not configured. Default value is taken from Inbound...");
                }
                if (SFDCObject.GetValueAsBoolean("chat.con.search.no-record.popup.profile-activity", false))
                {
                    sfdcOptions.ConCanPopupNorecordActivity = true;
                }
                else
                {
                    sfdcOptions.ConCanPopupNorecordActivity = sfdcOptions.InbCanPopupNorecordActivity;
                    logger.Info("Consult no match activity popup is not configured. Default value is taken from Inbound...");
                }
                if (SFDCObject.GetValueAsBoolean("chat.con.search.popup.profile-activity", false))
                {
                    sfdcOptions.ConCanPopupMultimatchActivity = true;
                }
                else
                {
                    sfdcOptions.ConCanPopupMultimatchActivity = sfdcOptions.InbCanPopupMultimatchActivity;
                    logger.Info("Consult multimatch activity popup is not configured. Default value is taken from Inbound...");
                }
                sfdcOptions.ConsultLogEvent = SFDCObject.GetValueAsString("chat.con.create.activity-log.event-names", null);
                this.logger.Info("Configuration Read for " + objectName + ": " + sfdcOptions.ToString());
                return sfdcOptions;
            }
            catch (Exception generalException)
            {
                this.logger.Error("GetSFDCObjectChatProperties : Error occurred while reading SFDC Object Configurations:" + generalException.ToString());
            }
            return null;
        }

        #endregion GetSFDCObjectChatProperties

        #region GetSFDCObjectEmailProperties

        public EmailOptions GetSFDCObjectEmailProperties(KeyValueCollection SFDCObject, string objectName)
        {
            try
            {
                this.logger.Info("GetSFDCObjectEmailProperties : Initializing SFDC Object Configurations");
                this.logger.Info("GetSFDCObjectEmailProperties : Object Name : " + objectName);

                EmailOptions sfdcOptions = new EmailOptions();

                sfdcOptions.ObjectName = SFDCObject.GetValueAsString("object-name", null);
                sfdcOptions.SearchPriority = SFDCObject.GetValueAsLowerString("email.search.priority", "attribute,both", "user-data");
                sfdcOptions.NewrecordFieldIds = SFDCObject.GetValueAsString("email.new-record.field-id", null);
                sfdcOptions.SearchCondition = SFDCObject.GetValueAsUpperString("email.search.condition-type", "and", "OR");
                //inbound
                sfdcOptions.InboundSearchUserDataKeys = SFDCObject.GetValueAsString("email.inb.search.user-data.key-names", null);
                sfdcOptions.InboundSearchAttributeKeys = SFDCObject.GetValueAsString("email.inb.search.attribute.key-names", null);
                sfdcOptions.InboundPopupEvent = SFDCObject.GetValueAsString("email.inb.popup-event-names", null);
                sfdcOptions.InboundCanCreateLog = SFDCObject.GetValueAsBoolean("email.inb.create.activity-log", true);
                sfdcOptions.InboundCanUpdateLog = SFDCObject.GetValueAsBoolean("email.inb.update.activity-log", false);
                sfdcOptions.InboundUpdateEvent = SFDCObject.GetValueAsString("email.inb.update.activity-log.event-names", null);
                sfdcOptions.InbCanCreateMultimatchActivity = SFDCObject.GetValueAsBoolean("email.inb.search.enable.profile-activity", false);
                //Inbound multimatch action
                sfdcOptions.InbMultiMatchRecordAction = SFDCObject.GetValueAsLowerString("email.inb.search.multi-match", "openall,none", "searchpage");
                //Inbound no match action
                sfdcOptions.InbNoMatchRecordAction = SFDCObject.GetValueAsLowerString("email.inb.search.no-record", "searchpage,createnew,none", "opennew");
                //can create profile level activity log for no match
                sfdcOptions.InbCanCreateNorecordActivity = SFDCObject.GetValueAsBoolean("email.inb.search.no-record.enable.profile-activity", false);
                //can popup profile level activity log for no match
                sfdcOptions.InbCanPopupNorecordActivity = SFDCObject.GetValueAsBoolean("email.inb.search.no-record.popup.profile-activity", false);
                //can popup profile level activity log for multi match
                sfdcOptions.InbCanPopupMultimatchActivity = SFDCObject.GetValueAsBoolean("email.inb.search.popup.profile-activity", false);
                //Can create profile level activity for no record none scenario
                sfdcOptions.CanCreateProfileActivityforInbNoRecord = SFDCObject.GetValueAsBoolean("email.inb.search.no-record.none.activity-log", false);

                //outbound
                sfdcOptions.OutboundSearchUserDataKeys = SFDCObject.GetValueAsString("email.out.search.user-data.key-names", null);
                sfdcOptions.OutboundSearchAttributeKeys = SFDCObject.GetValueAsString("email.out.search.attribute.key-names", "to");
                sfdcOptions.OutboundPopupEvent = SFDCObject.GetValueAsString("email.out.popup-event-names", null);
                sfdcOptions.OutboundCanCreateLog = SFDCObject.GetValueAsBoolean("email.out.success.create.activity-log", true);
                sfdcOptions.OutboundCanUpdateLog = SFDCObject.GetValueAsBoolean("email.out.success.update.activity-log", false);
                sfdcOptions.OutboundUpdateEvent = SFDCObject.GetValueAsString("email.out.success.update.activity-log.event-names", null);
                sfdcOptions.OutboundFailureCanCreateLog = SFDCObject.GetValueAsBoolean("email.out.fail.create.activity-log", false);
                sfdcOptions.OutboundFailurePopupEvent = SFDCObject.GetValueAsString("email.out.fail.create.activity-log.event-names", null);
                sfdcOptions.OutCanCreateMultimatchActivity = SFDCObject.GetValueAsBoolean("email.out.search.enable.profile-activity", false);
                sfdcOptions.OutCanPopupMultimatchActivity = SFDCObject.GetValueAsBoolean("email.out.search.popup.profile-activity", false);
                //Outbound multi match action
                sfdcOptions.OutMultiMatchRecordAction = SFDCObject.GetValueAsLowerString("email.out.search.multi-match", "openall,none", "searchpage");
                //Outbound no match action
                sfdcOptions.OutNoMatchRecordAction = SFDCObject.GetValueAsLowerString("email.out.search.no-record", "searchpage,createnew,none", "opennew");
                sfdcOptions.OutCanCreateNorecordActivity = SFDCObject.GetValueAsBoolean("email.out.search.no-record.enable.profile-activity", false);
                sfdcOptions.OutCanPopupNorecordActivity = SFDCObject.GetValueAsBoolean("email.out.search.no-record.popup.profile-activity", false);
                sfdcOptions.CanCreateProfileActivityforOutNoRecord = SFDCObject.GetValueAsBoolean("email.out.search.no-record.none.activity-log", false);

                sfdcOptions.MaxNosRecordOpen = SFDCObject.GetValueAsInt("email.max-record.open", 5);
                sfdcOptions.CanCreateLogForNewRecordCreate = SFDCObject.GetValueAsBoolean("email.search.no-record.activity-log", true);
                sfdcOptions.SearchPageMode = SFDCObject.GetValueAsString("email.multi-match.records.searchpage", objectName);
                sfdcOptions.CanUpdateRecordData = SFDCObject.GetValueAsBoolean("email.update.new-record", false);
                sfdcOptions.CustomObjectURL = SFDCObject.GetValueAsString("object.url-id", null);
                sfdcOptions.PhoneNumberSearchFormat = SFDCObject.GetValueAsString("email.search.phone-number-format", "(xxx)xxx-xxxx,xxx-xxx-xxxx,(xxx)-xxx-xxxx,(xxx)xxxxxxx");
                sfdcOptions.EmailAppendActivityLogEventNames = SFDCObject.GetValueAsString("email.append.activity-log.event-names", null);
                this.logger.Info("Configuration Read for " + objectName + ": " + sfdcOptions.ToString());
                return sfdcOptions;
            }
            catch (Exception generalException)
            {
                this.logger.Error("GetSFDCObjectEmailProperties : Error occurred while reading SFDC Object Configurations:" + generalException.ToString());
            }
            return null;
        }

        #endregion GetSFDCObjectEmailProperties

        #region GetSFDCUserActivityEmailProperties

        public UserActivityOptions GetSFDCUserActivityEmailProperties(KeyValueCollection SFDCObject, string objectName)
        {
            try
            {
                this.logger.Info("GetSFDCUserActivityEmailProperties : Initializing SFDC UserActivity Email Configurations");
                this.logger.Info("GetSFDCUserActivityEmailProperties : Object Name : " + objectName);

                UserActivityOptions sfdcOptions = new UserActivityOptions();
                sfdcOptions.ObjectName = SFDCObject.GetValueAsString("object-name", null);
                sfdcOptions.InboundPopupEvent = SFDCObject.GetValueAsString("email.inb.popup-event-names", null);
                sfdcOptions.InboundCanCreateLog = SFDCObject.GetValueAsBoolean("email.inb.create.activity-log", true);
                sfdcOptions.InboundCanUpdateLog = SFDCObject.GetValueAsBoolean("email.inb.update.activity-log", false);
                sfdcOptions.InboundUpdateEvent = SFDCObject.GetValueAsString("email.inb.update.activity-log.event-names", null);
                // Outbound Configurations
                sfdcOptions.OutboundPopupEvent = SFDCObject.GetValueAsString("email.out.popup-event-names", null);
                sfdcOptions.OutboundCanCreateLog = SFDCObject.GetValueAsBoolean("email.out.success.create.activity-log", true);
                sfdcOptions.OutboundCanUpdateLog = SFDCObject.GetValueAsBoolean("email.out.success.update.activity-log", false);
                sfdcOptions.OutboundUpdateEvent = SFDCObject.GetValueAsString("email.out.success.update.activity-log.event-names", null);
                sfdcOptions.OutboundFailureCanCreateLog = SFDCObject.GetValueAsBoolean("email.out.fail.create.activity-log", true);
                sfdcOptions.OutboundFailurePopupEvent = SFDCObject.GetValueAsString("email.out.fail.create.activity-log.event-names", null);
                // Consult Configurations
                sfdcOptions.ConsultPopupEvent = SFDCObject.GetValueAsString("email.con.popup-event-names", null);
                sfdcOptions.ConsultLogEvent = SFDCObject.GetValueAsString("email.con.create.activity-log.event-names", null);
                sfdcOptions.ConsultCanCreateLog = SFDCObject.GetValueAsBoolean("email.con.create.activity-log", true);
                sfdcOptions.ConsultCanUpdateLog = SFDCObject.GetValueAsBoolean("email.con.update.activity-log", false);
                sfdcOptions.ConsultUpdateEvent = SFDCObject.GetValueAsString("email.con.update.activity-log.event-names", null);

                this.logger.Info("Configuration Read for " + objectName + ": " + sfdcOptions.ToString());
                return sfdcOptions;
            }
            catch (Exception generalException)
            {
                this.logger.Error("GetSFDCObjectemailProperties : Error occurred while reading SFDC Object Configurations:" + generalException.ToString());
            }
            return null;
        }

        #endregion GetSFDCUserActivityEmailProperties

        #region GetSFDCUserActivityVoiceProperties

        public UserActivityOptions GetSFDCUserActivityVoiceProperties(KeyValueCollection SFDCObject, string objectName)
        {
            try
            {
                this.logger.Info("GetSFDCUserActivityVoiceProperties : Initializing SFDC UserActivity Voice Configurations");
                this.logger.Info("GetSFDCUserActivityVoiceProperties : Object Name : " + objectName);

                UserActivityOptions sfdcOptions = new UserActivityOptions();
                sfdcOptions.ObjectName = SFDCObject.GetValueAsString("object-name", null);
                sfdcOptions.InboundPopupEvent = SFDCObject.GetValueAsString("voice.inb.popup-event-names", null);
                sfdcOptions.InboundCanCreateLog = SFDCObject.GetValueAsBoolean("voice.inb.create.activity-log", true);
                sfdcOptions.InboundCanUpdateLog = SFDCObject.GetValueAsBoolean("voice.inb.update.activity-log", false);
                sfdcOptions.InboundUpdateEvent = SFDCObject.GetValueAsString("voice.inb.update.activity-log.event-names", null);
                // Outbound Configurations
                sfdcOptions.OutboundPopupEvent = SFDCObject.GetValueAsString("voice.out.popup-event-names", null);
                sfdcOptions.OutboundCanCreateLog = SFDCObject.GetValueAsBoolean("voice.out.success.create.activity-log", true);
                sfdcOptions.OutboundCanUpdateLog = SFDCObject.GetValueAsBoolean("voice.out.success.update.activity-log", false);
                sfdcOptions.OutboundUpdateEvent = SFDCObject.GetValueAsString("voice.out.success.update.activity-log.event-names", null);
                sfdcOptions.OutboundFailureCanCreateLog = SFDCObject.GetValueAsBoolean("voice.out.fail.create.activity-log", true);
                sfdcOptions.OutboundFailurePopupEvent = SFDCObject.GetValueAsString("voice.out.fail.create.activity-log.event-names", null);
                // Consult Configurations
                sfdcOptions.ConsultPopupEvent = SFDCObject.GetValueAsString("voice.con.popup-event-names", null);
                sfdcOptions.ConsultLogEvent = SFDCObject.GetValueAsString("voice.con.create.activity-log.event-names", null);
                sfdcOptions.ConsultCanCreateLog = SFDCObject.GetValueAsBoolean("voice.con.create.activity-log", true);
                sfdcOptions.ConsultCanUpdateLog = SFDCObject.GetValueAsBoolean("voice.con.update.activity-log", false);
                sfdcOptions.ConsultUpdateEvent = SFDCObject.GetValueAsString("voice.con.update.activity-log.event-names", null);
                this.logger.Info("Configuration Read for " + objectName + ": " + sfdcOptions.ToString());
                return sfdcOptions;
            }
            catch (Exception generalException)
            {
                this.logger.Error("GetSFDCUserActivityVoiceProperties : Error occurred while reading SFDC Object Configurations:" + generalException.ToString());
            }
            return null;
        }

        #endregion GetSFDCUserActivityVoiceProperties

        #region GetSFDCUserActivityChatProperties

        public UserActivityOptions GetSFDCUserActivityChatProperties(KeyValueCollection SFDCObject, string objectName)
        {
            try
            {
                this.logger.Info("GetSFDCUserActivityVoiceProperties : Initializing SFDC UserActivity Chat Configurations");
                this.logger.Info("GetSFDCUserActivityVoiceProperties : Object Name : " + objectName);

                UserActivityOptions sfdcOptions = new UserActivityOptions();
                sfdcOptions.ObjectName = SFDCObject.GetValueAsString("object-name", null);
                sfdcOptions.InboundPopupEvent = SFDCObject.GetValueAsString("chat.inb.popup-event-names", null);
                sfdcOptions.InboundCanCreateLog = SFDCObject.GetValueAsBoolean("chat.inb.create.activity-log", true);
                sfdcOptions.InboundCanUpdateLog = SFDCObject.GetValueAsBoolean("chat.inb.update.activity-log", false);
                sfdcOptions.InboundUpdateEvent = SFDCObject.GetValueAsString("chat.inb.update.activity-log.event-names", null);
                // Outbound Configurations
                sfdcOptions.OutboundPopupEvent = SFDCObject.GetValueAsString("chat.out.popup-event-names", null);
                sfdcOptions.OutboundCanCreateLog = SFDCObject.GetValueAsBoolean("chat.out.success.create.activity-log", true);
                sfdcOptions.OutboundCanUpdateLog = SFDCObject.GetValueAsBoolean("chat.out.success.update.activity-log", false);
                sfdcOptions.OutboundUpdateEvent = SFDCObject.GetValueAsString("chat.out.success.update.activity-log.event-names", null);
                sfdcOptions.OutboundFailureCanCreateLog = SFDCObject.GetValueAsBoolean("chat.out.fail.create.activity-log", false);
                sfdcOptions.OutboundFailurePopupEvent = SFDCObject.GetValueAsString("chat.out.fail.create.activity-log.event-names", null);
                // Consult Configurations
                sfdcOptions.ConsultPopupEvent = SFDCObject.GetValueAsString("chat.con.popup-event-names", null);
                sfdcOptions.ConsultLogEvent = SFDCObject.GetValueAsString("chat.con.create.activity-log.event-names", null);
                sfdcOptions.ConsultCanCreateLog = SFDCObject.GetValueAsBoolean("chat.con.create.activity-log", true);
                sfdcOptions.ConsultCanUpdateLog = SFDCObject.GetValueAsBoolean("chat.con.update.activity-log", false);
                sfdcOptions.ConsultUpdateEvent = SFDCObject.GetValueAsString("chat.con.update.activity-log.event-names", null);
                this.logger.Info("Configuration Read for " + objectName + ": " + sfdcOptions.ToString());
                return sfdcOptions;
            }
            catch (Exception generalException)
            {
                this.logger.Error("GetSFDCUserActivityChatProperties : Error occurred while reading SFDC Object Configurations:" + generalException.ToString());
            }

            return null;
        }

        #endregion GetSFDCUserActivityChatProperties

        #region FindDuplicate in array

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

        #endregion FindDuplicate in array

        #region ReadAdvancedSearchConfiguration

        public void ReadAdvancedSearchConfiguration()
        {
            try
            {
                this.logger.Info("ReadAdvancedSearchConfiguration()");
                if (Settings.SFDCOptions.EnableAdvancedSearch && !string.IsNullOrWhiteSpace(Settings.SFDCOptions.AdvancedSearchSectionNames))
                {
                    string[] sectionList = Settings.SFDCOptions.AdvancedSearchSectionNames.Split(',');
                    if (sectionList != null && sectionList.Length > 0)
                    {
                        foreach (string key in sectionList)
                        {
                            KeyValueCollection adsearchConfig = ReadConfiguration.GetInstance().ReadSFDCUtilityConfig(Settings.AgentDetails.MyApplication, Settings.AgentDetails.AgentGroups, Settings.AgentDetails.Person, key);
                            if (adsearchConfig != null)
                            {
                                if (Settings.AdSearchConfig == null)
                                    Settings.AdSearchConfig = new System.Collections.Generic.List<KeyValueCollection>();

                                Settings.AdSearchConfig.Add(adsearchConfig);
                            }
                            else
                            {
                                this.logger.Info("Advanced Search configuration is not found for the section name :" + key);
                            }
                        }
                    }
                    else
                    {
                        this.logger.Info("Advanced Search configuration is not found");
                    }
                }
                else
                {
                    this.logger.Info("Advanced Search is not enabled..");
                }
            }
            catch (Exception generalException)
            {
                this.logger.Error("Error occurred while reading advanced search configurations, details:" + generalException);
            }
        }

        #endregion ReadAdvancedSearchConfiguration

        #region BuildAdSearchobjects

        public AdSearch BuildAdSearchobjects(KeyValueCollection configs)
        {
            AdSearch adsearch = new AdSearch();
            try
            {
                if (configs == null)
                    return null;

                switch (configs.GetValueAsString("search.field-type", "All").ToLower())
                {
                    case "all":
                        adsearch.SearchFields = SearchFieldType.All;
                        break;

                    case "phone":
                        adsearch.SearchFields = SearchFieldType.Phone;
                        break;

                    case "name":
                        adsearch.SearchFields = SearchFieldType.Name;
                        break;

                    case "email":
                        adsearch.SearchFields = SearchFieldType.Email;
                        break;

                    case "custom":
                        adsearch.SearchFields = SearchFieldType.Custom;
                        break;

                    case "sidebar":
                        adsearch.SearchFields = SearchFieldType.Sidebar;
                        break;

                    default:
                        adsearch.SearchFields = SearchFieldType.All;
                        break;
                }

                adsearch.LookupFields = configs.GetValueAsString("search.field-names", null);
                adsearch.ResponseFields = configs.GetValueAsString("search.response-fields", null);
                adsearch.LookupObjects = configs.GetValueAsString("search.object-names", null);
                switch (configs.GetValueAsString("search.data.condition", "none").ToLower())
                {
                    case "numeric":
                        adsearch.SearchDataType = ADSearchDataType.Numeric;
                        break;

                    case "alphanumeric":
                        adsearch.SearchDataType = ADSearchDataType.AlphaNumeric;
                        break;

                    case "none":
                        adsearch.SearchDataType = ADSearchDataType.None;
                        break;

                    default:
                        adsearch.SearchDataType = ADSearchDataType.None;
                        break;
                }

                adsearch.SearchDataLength = configs.GetValueAsInt("search.condition.data.length", 0);
                adsearch.SearchDelimiter = configs.GetValueAsString("search.delimiter", "OR");
                adsearch.EnableFilterResults = configs.GetValueAsBoolean("search.enable.response-filter", false);
                adsearch.FilterResultFields = configs.GetValueAsString("search.response-filter.field-name", null);

                if (!string.IsNullOrWhiteSpace(configs.GetValueAsString("search.data.skip-values", null)))
                {
                    adsearch.SKipSearchData = configs.GetAsString("search.data.skip-values").Split(',').ToList();
                }

                var formats = configs.GetValueAsString("search.data.format", null);
                if (!string.IsNullOrWhiteSpace(formats))
                {
                    adsearch.SearchDataFormat = formats.Split(',');
                }

                //custom query
                adsearch.EnableCustomQuery = configs.GetValueAsBoolean("search.enable.custom-query", false);
                adsearch.CustomQuery = configs.GetValueAsString("search.custom.query", null);

                // Add ID as a default Response field for the advanced search
                if (!string.IsNullOrWhiteSpace(adsearch.ResponseFields))
                {
                    if (adsearch.ResponseFields.Contains("("))
                        adsearch.ResponseFields = adsearch.ResponseFields.Replace("(", "(Id,");
                    else
                        adsearch.ResponseFields = "Id," + adsearch.ResponseFields;
                }
                else
                {
                    adsearch.ResponseFields = "Id";
                }
            }
            catch (Exception generalException)
            {
                this.logger.Error("Error occurred while building Advanced Search objects, details :" + generalException);
            }
            return adsearch;
        }

        #endregion BuildAdSearchobjects
    }
}