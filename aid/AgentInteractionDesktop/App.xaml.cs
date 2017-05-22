namespace AgentInteractrionDesktop
{
    using Agent.Interaction.Desktop;
    using Agent.Interaction.Desktop.Settings;
    using System;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using System.Configuration;
    using System.Deployment.Application;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading;
    using System.Windows;
    using System.Windows.Media.Animation;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        #region Fields

        private Datacontext _dataContext = Datacontext.GetInstance();

        #endregion Fields

        #region Methods

        /// <summary>
        /// Sets the language.
        /// </summary>
        public void SetLanguage()
        {
            try
            {
                //switch (Thread.CurrentThread.CurrentCulture.ToString())
                //{
                //    case "en-US":
                //        Login.CultureCode = "en-US";
                //        break;

                //    //case "es-ES":
                //    //    Login.CultureCode = "es-ES";
                //    //    break;

                //    default:
                //        Login.CultureCode = "en-US";
                //        break;
                //}
                var cultureInfo = new CultureInfo("en-US");
                Thread.CurrentThread.CurrentCulture = cultureInfo;
                Thread.CurrentThread.CurrentUICulture = cultureInfo;
                var dictionary = (from d in _dataContext.ImportCatalog.ResourceDictionaryList
                                  where d.Metadata.ContainsKey("Culture")
                                  && d.Metadata["Culture"].ToString().Equals("en-US")
                                  select d).FirstOrDefault();
                if (dictionary != null && dictionary.Value != null)
                {
                    this.Resources.MergedDictionaries.Add(dictionary.Value);
                }
            }
            catch { }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Application.Startup" /> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.StartupEventArgs" /> that contains the event data.</param>
        protected override void OnStartup(StartupEventArgs e)
        {
            //try
            //{
            if (ApplicationDeployment.IsNetworkDeployed)
            {
                if (ConfigurationManager.AppSettings.AllKeys.Contains("auto.update") && ConfigurationManager.AppSettings["auto.update"].ToLower() == "true")
                {
                    //if (InstallUpdateSyncWithInfo())
                    //    return;
                }
                // <add key="login.url" value="tcp://win2003se:2020/AID" />
                if (!File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData).ToString() + @"\Pointel\AgentInteractionDesktop\localuser.settings.config"))
                {
                    if (ConfigurationManager.AppSettings.AllKeys.Contains("login.url"))
                    {
                        if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["login.url"]))
                        {
                            string uri = ConfigurationManager.AppSettings["login.url"];
                            Uri _uri = new Uri(uri);
                            _dataContext.ApplicationName = _uri.PathAndQuery.Replace("/", "");
                            _dataContext.HostNameText = _dataContext.HostNameSelectedValue = _uri.Host;
                            _dataContext.PortText = _dataContext.PortSelectedValue = (_uri.Port > 0 ? _uri.Port.ToString() : "");
                        }
                    }
                }
            }
            _dataContext.IsDebug = (e.Args.Contains("debug"));
            bool IsSplashScreenEnabled = true;
            if (e.Args.Length > 0)
                IsSplashScreenEnabled = !(e.Args.Contains("NoSplashScreen"));
            if (IsSplashScreenEnabled)
            {
                var splashScreen = new SplashScreen(System.Reflection.Assembly.GetExecutingAssembly(), "Images/MainSplashScreen.png");
                splashScreen.Show(true);
            }
            //}
            //catch (Exception ex)
            //{
            //    System.Windows.MessageBox.Show("Error occurred while moving settings file to required path as : " + ex.ToString());
            //}

            SetImportCatalog();
            SetLanguage();
            Application.Current.DispatcherUnhandledException += new System.Windows.Threading.DispatcherUnhandledExceptionEventHandler(AppDispatcherUnhandledException);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

            //Code added to reduce cpu usage to minimal by reducing the application refresh frame per second.
            Timeline.DesiredFrameRateProperty.OverrideMetadata(typeof(Timeline), new FrameworkPropertyMetadata { DefaultValue = 21 });
            base.OnStartup(e);

            Login objLogin = new Login();
            objLogin.Show();

            //System.Windows.MessageBox.Show("The application is deployed by " + (ApplicationDeployment.IsNetworkDeployed == true ? "Click once." : "Stand alone." ));
        }

        private void InstallUpdateSyncWithInfo()
        {
            UpdateCheckInfo info;
            ApplicationDeployment applicationDeployment = ApplicationDeployment.CurrentDeployment;
            info = applicationDeployment.CheckForDetailedUpdate();
            if (!applicationDeployment.IsFirstRun)
            {
                if (info.UpdateAvailable)
                {
                    applicationDeployment.UpdateCompleted += applicationDeployment_UpdateCompleted;
                    applicationDeployment.UpdateAsync();
                }
            }
        }

        private void applicationDeployment_UpdateProgressChanged(object sender, DeploymentProgressChangedEventArgs e)
        {
            System.Windows.MessageBox.Show("applicationDeployment_UpdateProgressChanged event called..");
        }

        private void applicationDeployment_UpdateCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            System.Windows.MessageBox.Show("applicationDeployment_UpdateCompleted event is called...");
        }

        private void applicationDeployment_DownloadFileGroupProgressChanged(object sender, DeploymentProgressChangedEventArgs e)
        {
            System.Windows.MessageBox.Show("applicationDeployment_DownloadFileGroupProgressChanged event is callled...");
        }

        private void applicationDeployment_DownloadFileGroupCompleted(object sender, DownloadFileGroupCompletedEventArgs e)
        {
            System.Windows.MessageBox.Show("applicationDeployment_DownloadFileGroupCompleted event is called...");
        }

        private void applicationDeployment_CheckForUpdateProgressChanged(object sender, DeploymentProgressChangedEventArgs e)
        {
            System.Windows.MessageBox.Show("applicationDeployment_CheckForUpdateProgressChanged event is called");
        }

        private void applicationDeployment_CheckForUpdateCompleted(object sender, CheckForUpdateCompletedEventArgs e)
        {
            System.Windows.MessageBox.Show("applicationDeployment_CheckForUpdateCompleted event is called...");
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                Exception ex = (Exception)e.ExceptionObject;
                Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
                        "AID").Error("--- Unhandled exception ---");

                Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
                        "AID").Error("Exception : " + ((ex.InnerException == null) ? ex.Message : ex.InnerException.ToString()));

                if (ex.TargetSite != null)
                    Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
                        "AID").Error("Exception Module : " + ex.TargetSite.Module.Name + " - Exception Method : " + ex.TargetSite.Name);

                var showMessageBox = new Agent.Interaction.Desktop.MessageBox("Alert",
                        "Agent Interaction Desktop has encountered a problem.\nThe application will exit now, please contact your Administrator", "", "_OK", false);
                showMessageBox.ShowDialog();

                if (showMessageBox.DialogResult == true)
                {
                    Environment.Exit(0);
                }
            }
            catch { }
            finally
            {
            }
        }

        private void AppDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            // System.Windows.MessageBox.Show(e.Exception.Message);
            try
            {
                Exception ex = (Exception)e.Exception;
                Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
                        "AID").Fatal("--- Unhandled exception ---");

                Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
                        "AID").Fatal("Exception : " + ((ex.InnerException == null) ? ex.Message : ex.InnerException.ToString()));

                if (ex.TargetSite != null)
                    Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
                        "AID").Fatal("Exception Module : " + ex.TargetSite.Module.Name + " - Exception Method : " + ex.TargetSite.Name);

                var showMessageBox = new Agent.Interaction.Desktop.MessageBox("Alert",
                        "Agent Interaction Desktop has encountered a problem.\nThe application will exit now, please contact your Administrator", "", "_OK", false);
                showMessageBox.ShowDialog();

                if (showMessageBox.DialogResult == true)
                {
                    Environment.Exit(0);
                }
            }
            catch { }
            finally
            {
            }
        }

        /// <summary>
        /// Sets the import catalog.
        /// </summary>
        private void SetImportCatalog()
        {
            try
            {
                var path = AppDomain.CurrentDomain.BaseDirectory;
                var catalog = new DirectoryCatalog(path);
                var container = new CompositionContainer(catalog);
                container.ComposeParts(_dataContext.ImportCatalog);
            }
            catch (ReflectionTypeLoadException ex)
            {
                var sb = new StringBuilder();
                foreach (var exSub in ex.LoaderExceptions)
                {
                    sb.AppendLine(exSub.Message);
                    if (exSub is FileNotFoundException)
                    {
                        var exFileNotFound = exSub as FileNotFoundException;
                        if (!string.IsNullOrEmpty(exFileNotFound.FusionLog))
                        {
                            sb.AppendLine("Fusion Log:");
                            sb.AppendLine(exFileNotFound.FusionLog);
                        }
                    }
                    sb.AppendLine();
                }
                //Display or log the error based on your application.
            }
        }

        #endregion Methods
    }
}