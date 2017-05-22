namespace Pointel.Integration.Core
{
    using System;
    using System.Collections.Generic;
    using System.Data.OracleClient;
    using System.Data.SqlClient;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading;

    using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel;
    using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.CfgObjects;
    using Genesyslab.Platform.Commons.Collections;
    using Genesyslab.Platform.Commons.Protocols;

    using Pointel.Configuration.Manager;
    using Pointel.Desktop.Access.Control;
    using Pointel.Integration.Core.Application;
    using Pointel.Integration.Core.Data;
    using Pointel.Integration.Core.iSubjects;
    using Pointel.Integration.Core.Observers;
    using Pointel.Integration.Core.Providers;
    using Pointel.Integration.Core.Util;
    using Pointel.Integration.PlugIn;
    using Pointel.Softphone.Voice.Core;
    using Genesyslab.Platform.Voice.Protocols.TServer.Events;

    public class DesktopMessenger : IDesktopMessenger, ISoftphoneListener
    {
        #region Fields

        public static IDesktopCommunicator communicateUI = null;

        public static IMIDHandler midHandler;

        internal static byte totalWebIntegration = 0;

        private static iCallData callData;
        private static CallDataProviders newCallDataProvider;

        private CrmDbSubscriber crmDbSubscriber;

        //private FileSubscriber fileSubscriber;
        //private PipeSubscriber pipeSubscriber;
        private PortSubscriber portSubscriber;
        private Settings setting = Settings.GetInstance();

        //private UrlSubscriber urlSubscriber;
        private Pointel.Logger.Core.ILog _logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
            "AID");

        #endregion Fields

        #region Methods

        public static iCallData GetCallData()
        {
            return callData;
        }

        public void GetAgentInfo(PlugIn.Common.AgentInfo agentInfo)
        {
            if (agentInfo != null)
            {
                setting.FirstName = agentInfo.FirstName;
                setting.LastName = agentInfo.LastName;
                setting.UserName = agentInfo.UserName;
                setting.QueueName = agentInfo.Queue;
                setting.AgentLoginID = agentInfo.LoginID;
                setting.EmployeeID = agentInfo.EmployeeID;
            }
        }

        public void InitializeIntegration(ConfService confProtocol, string applicationName, System.Collections.Generic.Dictionary<string, bool> integrationMediaList = null)
        {
            Thread newThread = new Thread(() =>
            {

                _logger.Info("**********************************************************************************************************************");
                _logger.Info("Pointel.Integration.Core :" + Assembly.GetExecutingAssembly().GetName().Version);
                _logger.Info("***********************************************************************************************************************");
                _logger.Info("Retrieving Values from Application third party integration start.");
                ReadApplication readApplication = new ReadApplication();
                Settings settings = Settings.GetInstance();

                try
                {
                    readApplication.ReadIntegrationDecisionKeyCollections();
                    callData = readApplication.ReadFileIntegrationKeyCollections(confProtocol, applicationName);
                    readApplication.ReadApplicationValue(confProtocol, applicationName);
                    newCallDataProvider = new CallDataProviders();

                    if (settings.EnableFileCommunication)
                        InitFileIntegration();
                    else
                        _logger.Warn("File Communication Disabled");

                    if (settings.EnablePortCommunication)
                    {
                        try
                        {
                            portSubscriber = new PortSubscriber();
                            portSubscriber.Subscribe(newCallDataProvider);
                            callData.PortData.Decision = IntegrationAction.Open;
                            newCallDataProvider.NewCallData(callData);
                        }
                        catch (Exception ex)
                        {
                            _logger.Error("Error occurred while subcribe Port as " + ex.Message);
                        }

                    }
                    else
                        _logger.Info("Port integration disabled.");

                    if (settings.EnablePipeCommunication)
                        InitPipeIntegration();
                    else
                        _logger.Info("Pipe integration disabled.");

                    if (settings.EnableURLCommunication)
                        InitWebUrlIntegration(integrationMediaList);
                    else
                        _logger.Info("URL integration disabled.");

                    if (settings.EnableCrmDbCommunication)
                    {
                        try
                        {
                            crmDbSubscriber = new CrmDbSubscriber();
                            crmDbSubscriber.Subscribe(newCallDataProvider);
                        }
                        catch (Exception ex)
                        {
                            _logger.Error("Error occurred while subcribe DB Communication as " + ex.Message);
                        }
                    }
                    else
                        _logger.Info("Database integration disabled");

                    if (ConfigContainer.Instance().AllKeys.Contains("voice.enable.agent-activity-db-integration") && ConfigContainer.Instance().GetAsBoolean("voice.enable.agent-activity-db-integration"))
                    {
                        try
                        {
                            var agentActivitySubscriber = new AgentActivitySubscriber();
                            agentActivitySubscriber.Subscribe(newCallDataProvider);
                        }
                        catch (Exception ex)
                        {
                            _logger.Error("Error occurred while subcribe DB Communication as " + ex.Message);
                        }
                    }
                    else
                        _logger.Info("Agent interaction activity disabled");


                    ISoftphoneListener softSubscriber = new DesktopMessenger();
                    SoftPhone softPhone = new SoftPhone();
                    softPhone.Subscribe(softSubscriber, Softphone.Voice.SoftPhoneSubscriber.Integration);

                    StartHIMMSIntegration();
                }
                catch (Exception generalException)
                {
                    _logger.Error("Error occurred while reading integration part from the application " + generalException.ToString());
                }
            }); newThread.Start();
        }

        /// <summary>
        /// This method Notifies Agent Current Status
        /// </summary>
        /// <param name="agentStatus"></param>
        public void NotifyAgentStatus(Softphone.Voice.AgentStatus agentStatus)
        {
            //throw new NotImplementedException();
        }

        /// <summary>
        /// This method Notifies Desktop about call ringing
        /// </summary>
        /// <param name="message"></param>
        public void NotifyCallRinging(IMessage message)
        {
            
            Settings setting = Settings.GetInstance();
            try
            {
               
                if (message != null)
                {
                    if (message != null && message.Name == EventRinging.MessageName && DesktopMessenger.communicateUI != null)
                    {
                        DesktopMessenger.midHandler = null;
                        DesktopMessenger.communicateUI.NotifyMIDState(false, null);
                    }
                        

                    callData.EventMessage = message;
                    callData.MediaType = MediaType.Voice;
                    newCallDataProvider.NewCallData(callData);
                }
            }
            catch (Exception generalException)
            {
                _logger.Error("NotifyCallRinging" + generalException.ToString());
            }
        }

        /// <summary>
        /// Notifies the error message.
        /// </summary>
        /// <param name="errorMessage">The error message.</param>
        public void NotifyErrorMessage(Softphone.Voice.Common.OutputValues errorMessage)
        {
            //throw new NotImplementedException();
        }

        public void NotifyEvent(IMessage message, int media = 0)
        {
            if (message != null)
            {
                callData.EventMessage = message;
                callData.MediaType = MediaType.Voice;
                newCallDataProvider.NewCallData(callData);
            }
            else
                _logger.Warn("The event message sent as null for the media " + media);
        }

        /// <summary>
        /// This method Notifies the subscriber.
        /// </summary>
        /// <param name="callData">The call data.</param>
        public void NotifySubscriber(VoiceEvents voiceEvents, object callData)
        {
            //throw new NotImplementedException();
        }

        /// <summary>
        /// This method Notifies the UI status.
        /// </summary>
        /// <param name="status">The status.</param>
        public void NotifyUIStatus(SoftPhoneStatusController status)
        {
            //throw new NotImplementedException();
        }

        public void PopupMID(string mid)
        {
            try
            {
                //   HimmsSubscriber objHimmsSubscriber = newCallDataProvider.GetObserver<HimmsSubscriber>();
                if (DesktopMessenger.midHandler != null)
                    DesktopMessenger.midHandler.PopupMID(mid);
            }
            catch (Exception generalException)
            {
                _logger.Error("Error occurred as " + generalException.Message);
                _logger.Trace("Error Trace : " + generalException.StackTrace);
            }
        }

        public void StartHIMMSIntegration()
        {
            Thread objThread = new Thread(() =>
            {
                try
                {
                    CfgApplication application = null;
                    if (ConfigContainer.Instance().AllKeys.Contains("CfgApplication"))
                        application = ConfigContainer.Instance().GetValue("CfgApplication");
                    if (application != null)
                    {
                        if (!application.Options.ContainsKey("himms-integration"))
                        {
                            _logger.Warn("The HIMMS integration's configuration not found.");
                            return;
                        }

                        KeyValueCollection kvHIMMS = application.Options.GetAsKeyValueCollection("himms-integration");

                        HimmsIntegrationData objHimmsConfiguration = new HimmsIntegrationData();
                        _logger.Trace("Try to parse the HIMMS configuration.");
                        objHimmsConfiguration.ParseConfiguration(kvHIMMS);
                        _logger.Trace("The HIMMS configuration parsed successfully.");
                        //mannual login.
                        //objHimmsConfiguration.UserName = username;
                        //objHimmsConfiguration.Password = password;
                        if (objHimmsConfiguration.IsEnabled)
                        {
                            HimmsSubscriber objHimmsSubscriber = new HimmsSubscriber(objHimmsConfiguration);

                            _logger.Trace("Try to subcribe the HIMMS integration.");

                            objHimmsSubscriber.Subscribe(newCallDataProvider);

                            _logger.Trace("The HIMMS integration subscribed successfully.");
                        }
                    }
                    else
                        _logger.Warn("The application object is null while start HIMMS integration.");
                }
                catch (Exception generalException)
                {
                    _logger.Error("Error occurred as " + generalException.Message);
                    _logger.Trace("Error Trace : " + generalException.StackTrace);
                }
            });
            objThread.Start();
        }

        public void StartWebIntegration(List<ApplicationDataDetails> lstApplication)
        {
            try
            {
                if (lstApplication != null)
                {
                    CfgApplication application = null;
                    if (ConfigContainer.Instance().AllKeys.Contains("CfgApplication"))
                        application = ConfigContainer.Instance().GetValue("CfgApplication");

                    if (application != null)
                    {
                        List<WebIntegrationData> lstWebIntegration = GetConfiguredIntegration<WebIntegrationData>("web-integration.");
                        if (lstWebIntegration != null)
                        {
                            for (byte index = 0; index < lstWebIntegration.Count; index++)
                            {
                                if (lstWebIntegration[index].IsConditional && lstApplication.Any(x => x.ApplicationName == lstWebIntegration[index].ApplicationName))
                                {
                                    DesktopMessenger.totalWebIntegration++;
                                    lstWebIntegration[index].ApplicationData = lstApplication.SingleOrDefault(x => x.ApplicationName == lstWebIntegration[index].ApplicationName);
                                    UrlSubscriber objUrlIntegration = new UrlSubscriber(lstWebIntegration[index]);
                                    objUrlIntegration.Subscribe(newCallDataProvider);

                                    // If it is need , Need to call OnNext method.
                                    //objUrlIntegration.OnNext(null);
                                }
                            }
                        }
                    }
                }
                else
                    _logger.Warn("The web integration is null.");
            }
            catch (Exception generalException)
            {
                _logger.Error("Error occurred as " + generalException.Message);
                _logger.Trace("Error trace: " + generalException.Message);
            }
        }

        public void Subscribe(IDesktopCommunicator messagetoClient)
        {
            try
            {
                communicateUI = messagetoClient;
            }
            catch (Exception generalException)
            {
                _logger.Error("Error occurred while subscribing Plugin " + generalException.ToString());
            }

            Settings settings = Settings.GetInstance();
            if (settings.EnableCrmDbCommunication)
            {
                try
                {
                    if (callData.CrmDbData.CrmDbFormat.Equals("SqlServer"))
                    {
                        Settings.GetInstance().cn = new SqlConnection(callData.CrmDbData.ConnectionSqlPath);
                        Settings.GetInstance().cn.Open();
                    }
                    else if (callData.CrmDbData.CrmDbFormat.Equals("Oracle"))
                    {
                        Settings.GetInstance().connection = new OracleConnection(callData.CrmDbData.ConnectionOraclePath);
                        Settings.GetInstance().connection.Open();
                    }
                }
                catch (Exception exception)
                {
                    DesktopMessenger.communicateUI.NotifyDesktopErrorMessage(exception.Message.ToString());
                    _logger.Error("Could not open db connection" + exception.ToString());
                }
            }
            if (setting.EnableURLCommunication)
            {
                newCallDataProvider.NotifyLoginURL();
            }
        }

        public void UnSubcribe()
        {
            try
            {
                communicateUI = null;
                callData = null;
                if (newCallDataProvider != null)
                    newCallDataProvider.CloseIntegration();
                //if (fileSubscriber != null)
                //    fileSubscriber.Unsubscribe();
                if (portSubscriber != null)
                    portSubscriber.Unsubscribe();
                //if (urlSubscriber != null)
                //    urlSubscriber.Unsubscribe();
                //if (pipeSubscriber != null)
                //    pipeSubscriber.Unsubscribe();
            }
            catch (Exception generalException)
            {
                _logger.Error("UnSubcribe()" + generalException.ToString());
            }
        }

        private List<T> GetConfiguredIntegration<T>(string stringToMatch)
        {
            List<T> lstConfigurations = new List<T>();
            CfgPerson person = null;
            if (ConfigContainer.Instance().AllKeys.Contains("CfgPerson"))
                person = ConfigContainer.Instance().GetValue("CfgPerson");

            List<CfgAgentGroup> agentGroup = null;
            if (ConfigContainer.Instance().AllKeys.Contains("CfgAgentGroup"))
                agentGroup = ConfigContainer.Instance().GetValue("CfgAgentGroup");

            CfgApplication application = null;
            if (ConfigContainer.Instance().AllKeys.Contains("CfgApplication"))
                application = ConfigContainer.Instance().GetValue("CfgApplication");
            if (application != null)
            {
                List<string> lstSections = new List<string>();

                if (person != null && person.UserProperties != null)
                    foreach (string sectionName in person.UserProperties.AllKeys.Where(x => x.StartsWith(stringToMatch)).ToArray())
                        lstSections.Add(sectionName);

                if (agentGroup != null)
                    for (int index = 0; index < agentGroup.Count; index++)
                        if (agentGroup[index].GroupInfo != null && agentGroup[index].GroupInfo.UserProperties != null)
                            foreach (string sectionName in agentGroup[index].GroupInfo.UserProperties.AllKeys.Where(x => x.StartsWith(stringToMatch)).ToArray())
                                if (!lstSections.Contains(sectionName))
                                    lstSections.Add(sectionName);

                foreach (string sectionName in application.Options.AllKeys.Where(x => x.StartsWith(stringToMatch)).ToArray())
                    if (!lstSections.Contains(sectionName))
                        lstSections.Add(sectionName);

                _logger.Trace("Number of web integration configured: " + lstSections.Count);
                for (int index = 0; index < lstSections.Count; index++)
                {
                    KeyValueCollection section = null;
                    if (application.Options.ContainsKey(lstSections[index]))
                        section = application.Options.GetAsKeyValueCollection(lstSections[index]);

                    if (person != null && person.UserProperties != null && person.UserProperties.ContainsKey(lstSections[index]))
                        section = person.UserProperties.GetAsKeyValueCollection(lstSections[index]);
                    else if (agentGroup != null)
                    {
                        for (int agentGroupIndex = 0; agentGroupIndex < agentGroup.Count; agentGroupIndex++)
                            if (agentGroup[agentGroupIndex].GroupInfo != null && agentGroup[agentGroupIndex].GroupInfo.UserProperties != null
                        && agentGroup[agentGroupIndex].GroupInfo.UserProperties.ContainsKey(lstSections[index]))
                            {
                                section = agentGroup[agentGroupIndex].GroupInfo.UserProperties.GetAsKeyValueCollection(lstSections[index]);
                                break;
                            }

                    }
                    if (section != null)
                        lstConfigurations.Add((T)Activator.CreateInstance(typeof(T), section));
                }
            }
            else
                _logger.Warn("Application object is null");

            return lstConfigurations;
        }

        private void InitFileIntegration()
        {
            try
            {
                List<FileIntegrationData> lstFileIntegration = GetConfiguredIntegration<FileIntegrationData>("file-integration.");
                if (lstFileIntegration != null)
                {
                    _logger.Trace("Number of file integration configured: " + lstFileIntegration.Count);
                    for (byte index = 0; index < lstFileIntegration.Count; index++)
                        if (lstFileIntegration[index].IsEnabled)
                        {
                            FileSubscriber objPipeSubscriber = new FileSubscriber(lstFileIntegration[index]);
                            objPipeSubscriber.Subscribe(newCallDataProvider);
                        }
                }
            }
            catch (Exception generalException)
            {
                _logger.Error("Error occurred as " + generalException.Message);
            }
        }

        private void InitPipeIntegration()
        {
            try
            {
                List<PipeIntegrationData> lstPipeIntegration = GetConfiguredIntegration<PipeIntegrationData>("pipe-integration.");
                if (lstPipeIntegration != null)
                {
                    _logger.Trace("Number of pipe integration configured: " + lstPipeIntegration.Count);
                    for (byte index = 0; index < lstPipeIntegration.Count; index++)
                        if (lstPipeIntegration[index].IsEnabled)
                        {
                            PipeSubscriber objPipeSubscriber = new PipeSubscriber(lstPipeIntegration[index]);
                            objPipeSubscriber.Subscribe(newCallDataProvider);
                        }
                        else
                            _logger.Warn("Integration disabled for the pipe '" + lstPipeIntegration[index].PipeName + "'");
                }
            }
            catch (Exception generalException)
            {
                _logger.Error("Error occurred while initialize pipe integration as " + generalException.Message);
            }
        }

        /// <summary>
        /// Its going to init the set of Web URL Integration.
        /// </summary>
        /// <param name="integrationMediaList">The integration media list.</param>
        private void InitWebUrlIntegration(System.Collections.Generic.Dictionary<string, bool> integrationMediaList = null)
        {
            System.Threading.Thread objThread = new System.Threading.Thread(() =>
            {
                try
                {
                    CfgApplication application = null;
                    if (ConfigContainer.Instance().AllKeys.Contains("CfgApplication"))
                        application = ConfigContainer.Instance().GetValue("CfgApplication");
                    if (application != null)
                    {
                        var file = Path.Combine(Path.Combine(System.Windows.Forms.Application.StartupPath, "Plugins"), "Pointel.Salesforce.Adapter.dll");
                        if (ConfigContainer.Instance().AllKeys.Contains("salesforce.enable.plugin") && ConfigContainer.Instance().GetAsBoolean("salesforce.enable.plugin")
                                && File.Exists(file))
                        {
                            if (application.Options.ContainsKey("salesforce-integration"))
                            {
                                KeyValueCollection salesforceSection = application.Options.GetAsKeyValueCollection("salesforce-integration");
                                if (salesforceSection.ContainsKey("sfdc.screen.popup") && salesforceSection.GetAsString("sfdc.screen.popup").ToLower() == "aid")
                                    DesktopMessenger.totalWebIntegration += 1;
                            }
                        }
                    }
                    List<WebIntegrationData> lstWebIntegration = GetConfiguredIntegration<WebIntegrationData>("web-integration.");
                    //null;
                    if (lstWebIntegration != null)
                    {
                        _logger.Trace("Number of web integration configured: " + lstWebIntegration.Count);
                        for (byte index = 0; index < lstWebIntegration.Count; index++)
                        {
                            if (lstWebIntegration[index].IsEnableIntegration)
                            {
                                if (lstWebIntegration[index].ApplicationName.ToLower().Equals("lawson"))
                                {
                                    if (integrationMediaList != null && integrationMediaList.ContainsKey("Lawson") && integrationMediaList["Lawson"])
                                        Settings.GetInstance().IsLawsonEnabled = integrationMediaList["Lawson"];
                                    else
                                        continue;
                                }

                                UrlSubscriber objUrlIntegration = new UrlSubscriber(lstWebIntegration[index]);
                                if (lstWebIntegration[index].BrowserType == BrowserType.AID)
                                    totalWebIntegration++;
                                objUrlIntegration.Subscribe(newCallDataProvider);
                            }
                            else
                                _logger.Warn("Integration disabled for the application '" + lstWebIntegration[index].ApplicationName + "'");
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error("Error occurred while subcribe URL Popup as " + ex.Message);
                }
            });
            objThread.Start();
        }

        #endregion Methods
    }
}