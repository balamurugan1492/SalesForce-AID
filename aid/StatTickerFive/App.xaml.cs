namespace StatTickerFive
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Diagnostics;
    using System.Drawing.Text;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Windows;
    using System.Xml;

    using Pointel.Statistics.Core;

    using StatTickerFive.Helpers;
    using StatTickerFive.ViewModels;
    using StatTickerFive.Views;
    using Pointel.Statistics.Core.Utility;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        #region Methods

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            try
            {
                string AIDValues = string.Empty;
                if (e.Args.Length != 0)
                {
                    string user = string.Empty, pwd = string.Empty, app = string.Empty, host = string.Empty, port = string.Empty, place = string.Empty;
                    //AIDValues = e.Args;
                    //AIDValues = "UserName:Gibran,Password:g,AppName:AID_PHS,Host:192.168.200.125,Port:2020,Place:AP3111,IsConfigQueue:True";
                    string[] Arguments = e.Args[0].Split(',');
                    //string[] Arguments = AIDValues.Split(',');
                    //string samp = "UserName:Alonda, Password:,AppName:AID,Host:192.168.200.125,Port:2020,Place:AP3107";
                    //string[] Arguments = samp.Split(',') ;
                    //foreach (var item in Arguments)
                    //    AIDValues += item + Environment.NewLine;
                    //System.Windows.MessageBox.Show("args: " + string.Join(", ", e.Args));
                    foreach (string args in Arguments)
                    {
                        string[] values;

                        if (!string.IsNullOrEmpty(args))
                        {
                            if (args.Contains(":"))
                            {
                                values = args.Split(':');
                                //System.Windows.MessageBox.Show("StatTickerFive Application StartUp: " + values[1].ToString());

                                switch (values[0])
                                {
                                    case "UserName":
                                        user = values[1];
                                        break;
                                    case "Password":
                                        pwd = values[1];
                                        break;
                                    case "AppName":
                                        app = values[1];
                                        break;
                                    case "Host":
                                        host = values[1];
                                        break;
                                    case "Port":
                                        port = values[1];
                                        break;
                                    case "Place":
                                        place = values[1];
                                        break;
                                    case "IsConfigQueue":
                                        Settings.GetInstance().IsQueueSelect = Convert.ToBoolean(values[1]);
                                        break;
                                    case "IsGadgetShow":
                                        Settings.GetInstance().IsGadgetShow = Convert.ToBoolean(values[1]);
                                        break;
                                    case "AddpServerTimeOut":
                                        Settings.GetInstance().AddpServerTimeOut = Convert.ToInt32(values[1]);
                                        break;
                                    case "AddpClientTimeOut":
                                        Settings.GetInstance().AddpClientTimeOut = Convert.ToInt32(values[1]);
                                        break;
                                }
                            }
                        }
                    }

                    // Microsoft.Windows.Controls.MessageBox.Show("Exiting for each");

                    if (!string.IsNullOrEmpty(user) && !string.IsNullOrEmpty(app) && !string.IsNullOrEmpty(host) && !string.IsNullOrEmpty(port) && !string.IsNullOrEmpty(place))
                    {

                        LoginWindowViewModel objLoginVM = new LoginWindowViewModel();
                        objLoginVM.UserName = user;
                        Settings.GetInstance().UserName = user;
                        objLoginVM.Password = pwd;
                        objLoginVM.ApplicationName = app;
                        objLoginVM.Host = host;
                        objLoginVM.Port = port;
                        objLoginVM.Place = place;
                        Settings.GetInstance().ApplicationType = Pointel.Statistics.Core.Utility.StatisticsEnum.StatSource.StatServer.ToString();
                        Settings.GetInstance().DefaultAuthentication = Pointel.Statistics.Core.Utility.StatisticsEnum.StatSource.StatServer.ToString();
                        Settings.GetInstance().Theme = "Outlook8";
                        //OutputValues output = new OutputValues();
                        //output = StatisticsBase.GetInstance().ConfigConnectionEstablish(host, port, app, user, pwd, user, "StatServer", String.Empty, true);
                        Helpers.IndexPage objIndexPage = new Helpers.IndexPage();
                        objIndexPage.StartStatisticsPipeListerner();
                        StatisticsBase.GetInstance().isPlugin = true;
                        //Microsoft.Windows.Controls.MessageBox.Show("Login");
                        objLoginVM.Login();
                    }
                    else
                    {
                        LoginUsual();
                        Settings.GetInstance().IsGadgetShow = true;
                    }
                }
                else
                {
                    Settings.GetInstance().IsGadgetShow = true;
                    LoginUsual();
                }

            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("StatTickerFive Application StartUp: " + ex.Message);
            }
        }

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                Exception ex = (Exception)e.ExceptionObject;
                Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
                        "STF").Error("--- Unhandled exception ---");

                Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
                        "STF").Error("Exception : " + ((ex.InnerException == null) ? ex.Message : ex.InnerException.ToString()));

                if (ex.TargetSite != null)
                    Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
                        "STF").Error("Exception Module : " + ex.TargetSite.Module.Name + " - Exception Method : " + ex.TargetSite.Name);

                Views.MessageBox msgbox;
                ViewModels.MessageBoxViewModel mboxvmodel;
                msgbox = new Views.MessageBox();
                mboxvmodel = new MessageBoxViewModel("Alert",
                                                     "StatTickerFive has encountered a problem. \nThe application will exit now, please contact your Administrator.",
                                                      msgbox, "Exception", Settings.GetInstance().Theme);
                msgbox.DataContext = mboxvmodel;
                msgbox.ShowDialog();

            }
            catch
            {
            }
        }

        private void LoginUsual()
        {
            try
            {
                XmlDataDocument xmldoc = new XmlDataDocument();
                XmlNodeList xmlnode;
                int i = 0;
                FileStream fs = new FileStream(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\PHS\AgentInteractionDesktop\app_config.xml", FileMode.Open, FileAccess.Read);
                xmldoc.Load(fs);

                xmlnode = xmldoc.GetElementsByTagName("AppSetting");
                for (i = 0; i <= xmlnode[0].ChildNodes.Count - 1; i++)
                {
                    XmlNodeList xmlInnernode = xmldoc.GetElementsByTagName(xmlnode[0].ChildNodes[i].Name.ToString());
                    if (xmlInnernode.Count != 0)
                    {
                        for (int j = 0; j <= xmlInnernode[0].ChildNodes.Count - 1; j++)
                        {
                            if (xmlInnernode[0].ChildNodes[j].ParentNode.Name.ToString().Trim().ToLower() == "runmultipleinstances")
                                Settings.GetInstance().RunMultipleInstances = Convert.ToBoolean(xmlInnernode[0].ChildNodes[j].InnerText.Trim());
                        }
                    }
                }
                //System.Windows.MessageBox.Show("Multiple Instance " + (Settings.GetInstance().RunMultipleInstances ? "Enabled." : "Disabled.") );
                if (Settings.GetInstance().RunMultipleInstances)
                {
                    Helpers.IndexPage objIndexPage = new Helpers.IndexPage();
                    objIndexPage.Start();
                }
                else
                {
                    Process thisProcess = Process.GetCurrentProcess();
                    if (Process.GetProcessesByName(thisProcess.ProcessName).Length > 1)
                    {
                        Environment.Exit(0);
                        return;
                    }
                    else
                    {
                        Helpers.IndexPage objIndexPage = new Helpers.IndexPage();
                        objIndexPage.Start();
                    }
                }
            }
            catch { }
        }

        #endregion Methods
    }
}