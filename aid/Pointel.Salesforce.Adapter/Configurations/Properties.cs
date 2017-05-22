namespace Pointel.Salesforce.Adapter.Configurations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Genesyslab.Platform.Commons.Collections;

    using Pointel.Salesforce.Adapter.LogMessage;
    using Pointel.Salesforce.Adapter.Utility;

    /// <summary>
    /// Comment: Initializes SFDC Object's Configurations
    /// Last Modified: 25-08-2015
    /// Created by: Pointel Inc.,
    /// </summary>
    /// 
    public class Properties
    {
        #region Fields

        private static Properties properties = null;

        Log logger = null;

        #endregion Fields

        #region Constructors

        private Properties()
        {
            this.logger = Log.GenInstance();
        }

        #endregion Constructors

        #region Methods

        public static Properties GetInstance()
        {
            if (properties == null)
            {
                properties = new Properties();
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
                    if (!String.IsNullOrEmpty(SFDCCollection.GetAsString("sfdc.chrome.browser-command")))
                    {
                        generalOptions.BrowserCommand = SFDCCollection.GetAsString("sfdc.chrome.browser-command").Trim();
                    }
                    if (!String.IsNullOrEmpty(SFDCCollection.GetAsString("sfdc.chrome.browser.temp-directory")))
                    {
                        generalOptions.BrowserTempDirectory = SFDCCollection.GetAsString("sfdc.chrome.browser.temp-directory").Trim();
                    }
                    #endregion
                    this.logger.Error("InitializeGeneralConfig : General SFDC Properties Initialized : " + generalOptions.ToString());
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

                if (!String.IsNullOrEmpty(SFDCObject.GetAsString("chat.search.multi-match")))
                {
                    if (SFDCObject.GetAsString("chat.search.multi-match").Trim().ToLower() == "openall")
                        sfdcOptions.MultiMatchRecordAction = "openall";
                    else if (SFDCObject.GetAsString("chat.search.multi-match").Trim().ToLower() == "none")
                        sfdcOptions.MultiMatchRecordAction = "none";
                    else
                        sfdcOptions.MultiMatchRecordAction = "searchpage";
                }
                else
                    sfdcOptions.MultiMatchRecordAction = "searchpage";

                if (!String.IsNullOrEmpty(SFDCObject.GetAsString("chat.search.no-record")))
                {
                    if (SFDCObject.GetAsString("chat.search.no-record").Trim().ToLower() == "searchpage")
                        sfdcOptions.NoMatchRecordAction = "searchpage";
                    else if (SFDCObject.GetAsString("chat.search.no-record").Trim().ToLower() == "createnew")
                        sfdcOptions.NoMatchRecordAction = "createnew";
                    else if (SFDCObject.GetAsString("chat.search.no-record").Trim().ToLower() == "none")
                        sfdcOptions.NoMatchRecordAction = "none";
                    else
                        sfdcOptions.NoMatchRecordAction = "opennew";
                }
                else
                    sfdcOptions.NoMatchRecordAction = "opennew";

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

                if (!String.IsNullOrEmpty(SFDCObject.GetAsString("chat.update.new-record")) &&
                    SFDCObject.GetAsString("chat.update.new-record").Trim().ToLower() == "true")
                {
                    sfdcOptions.CanUpdateRecordData = true;
                }

                if (!String.IsNullOrEmpty(SFDCObject.GetAsString("object.url-id")))
                {
                    sfdcOptions.CustomObjectURL = SFDCObject.GetAsString("object.url-id").Trim();
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

                if (!String.IsNullOrEmpty(SFDCObject.GetAsString("voice.multi-match.records.searchpage")))
                {
                    sfdcOptions.SearchPageMode = SFDCObject.GetAsString("voice.multi-match.records.searchpage").Trim();
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

                if (!String.IsNullOrEmpty(SFDCObject.GetAsString("voice.search.multi-match")))
                {
                    if (SFDCObject.GetAsString("voice.search.multi-match").Trim().ToLower() == "openall")
                        sfdcOptions.MultiMatchRecordAction = "openall";
                    else if (SFDCObject.GetAsString("voice.search.multi-match").Trim().ToLower() == "none")
                        sfdcOptions.MultiMatchRecordAction = "none";
                    else
                        sfdcOptions.MultiMatchRecordAction = "searchpage";
                }
                else
                    sfdcOptions.MultiMatchRecordAction = "searchpage";

                if (!String.IsNullOrEmpty(SFDCObject.GetAsString("voice.search.no-record")))
                {
                    if (SFDCObject.GetAsString("voice.search.no-record").Trim().ToLower() == "searchpage")
                        sfdcOptions.NoMatchRecordAction = "searchpage";
                    else if (SFDCObject.GetAsString("voice.search.no-record").Trim().ToLower() == "createnew")
                        sfdcOptions.NoMatchRecordAction = "createnew";
                    else if (SFDCObject.GetAsString("voice.search.no-record").Trim().ToLower() == "none")
                        sfdcOptions.NoMatchRecordAction = "none";
                    else
                        sfdcOptions.NoMatchRecordAction = "opennew";
                }
                else
                    sfdcOptions.NoMatchRecordAction = "opennew";

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
                    SFDCObject.GetAsString("voice.search.no-record.activity-log").Trim().ToLower() == "false")
                {
                    sfdcOptions.CanCreateLogForNewRecordCreate = false;
                }
                else
                    sfdcOptions.CanCreateLogForNewRecordCreate = true;

                if (!String.IsNullOrEmpty(SFDCObject.GetAsString("object.url-id")))
                {
                    sfdcOptions.CustomObjectURL = SFDCObject.GetAsString("object.url-id").Trim();
                }
                if (!String.IsNullOrEmpty(SFDCObject.GetAsString("voice.search.phone-number.format")))
                {
                    sfdcOptions.PhoneNumberSearchFormat = SFDCObject.GetAsString("voice.search.phone-number.format").Trim();
                }
                else
                {
                    sfdcOptions.PhoneNumberSearchFormat = "xxx-xxx-xxxx,(xxx)-xxx-xxxx,(xxx)xxx-xxxx,(xxx)xxxxxxx";
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

                if (!String.IsNullOrEmpty(SFDCObject.GetAsString("chat.con.success.create.activity-log.event-names")))
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