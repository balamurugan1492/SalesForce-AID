using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Drawing;
using System.Windows.Threading;
using System.Security.Principal;
using System.Collections.Generic;
using StatTickerFive;
using StatTickerFive.Views;
using StatTickerFive.Helpers;
using StatTickerFive.ViewModels;
using Pointel.Logger;
using Pointel.Statistics.Core;
using Pointel.IPlugin.Interface;
using Pointel.Statistics.Core.Utility;
using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel;


namespace StatTickerFive
{
    /// <summary>
    /// 
    /// </summary>
    public class PluginContainer : IPlugin
    {
        static Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "STF");
        StatisticsBase stat = new StatisticsBase();
        OutputValues Output = new OutputValues();
        XMLStorage objXmlReader = new XMLStorage();
        public bool isAuthenticated;
        public bool isAuthorized;
        public static IStatTicker messageToClient;
        StatTickerFive.ViewModels.MainWindowViewModel mainVm;

        #region Subscribe
        /// <summary>
        /// Receives the message to client object.
        /// </summary>
        /// <param name="Listener">The listener.</param>
        public void ReceiveMessageToClientObject(IStatTicker Listener)
        {
            messageToClient = Listener;
        }
        #endregion

        #region InitializeStatistics

        /// <summary>
        /// Initializes the statistics.
        /// </summary>
        /// <param name="UserName">Name of the user.</param>
        /// <param name="ApplicationName">Name of the application.</param>
        /// <param name="ConfService">The conf service.</param>
        /// <param name="AIDQueue"></param>
        public void InitializeStatistics(string UserName, string ApplicationName, ConfService ConfService, string AIDQueue, string source)
        {
        //    try
        //    {
        //        StatisticsBase.GetInstance().isPlugin = true;
        //        logger.Debug("PluginContainer : InitializeStatistics : Method Entry");

        //        WindowsIdentity windowsIdentity = WindowsIdentity.GetCurrent();
        //        if (windowsIdentity != null)
        //            Settings.GetInstance().logUserName = windowsIdentity.Name.Split('\\')[1];

        //        StatisticsBase.GetInstance().StatSource = Pointel.Statistics.Core.Utility.StatisticsEnum.StatSource.StatServer.ToString();
        //        Output = stat.ConfigConnectionEstablish(ApplicationName, ConfService, Settings.GetInstance().logUserName, UserName, source);
        //        if (Output.MessageCode == "200")
        //        {

        //            logger.Warn("PluginContainer : InitializeStatistics : Config Connection Establish success");
        //            Settings.GetInstance().UserName = UserName;
        //            Settings.GetInstance().ApplicationName = ApplicationName;

        //            string op = stat.CheckUserPrevileges(Settings.GetInstance().UserName);
        //            string[] Authenticated = op.Split(',');
        //            isAuthenticated = Convert.ToBoolean(Authenticated[0]);
        //            //
        //            Settings.GetInstance().DictEnableDisableChannels = stat.GetEnableDisableChannels();
        //            Settings.GetInstance().DictErrorValues = stat.GetErrorValues();
        //            Settings.GetInstance().ApplicationType = Pointel.Statistics.Core.Utility.StatisticsEnum.StatSource.StatServer.ToString();

        //            if (isAuthenticated)
        //            {
        //                logger.Warn("PluginContainer : InitializeStatistics : IsAuthenticated : True");
        //                Settings.GetInstance().Theme = "Outlook8";

        //                if (messageToClient == null)
        //                {
        //                    stat.SaveQueues(AIDQueue);

        //                   // bool valid = stat.StartStatistics(Settings.GetInstance().UserName, Settings.GetInstance().ApplicationName,false);
        //                }
        //                else
        //                {
        //                    MainWindow mainview = new MainWindow();
        //                    StatTickerFive.ViewModels.MainWindowViewModel mainVm =
        //                        new MainWindowViewModel();
        //                    mainview.DataContext = mainVm;
        //                    mainview.Show();
        //                    Window window = Application.Current.Windows[0];
        //                    if (window != null)
        //                        mainVm.dragwindowInitialize(mainview);

        //                }
        //            }
        //            else
        //            {
        //                logger.Warn("PluginContainer : InitializeStatistics : IsAuthenticated : False");

        //                Views.MessageBox msgbox = new Views.MessageBox();
        //                MessageBoxViewModel mboxviewmodel = new MessageBoxViewModel("Problem Encountered in Statistics", Settings.GetInstance().DictErrorValues["user.authorization"].ToString(), msgbox, "MainWindow", Settings.GetInstance().Theme);
        //                msgbox.DataContext = mboxviewmodel;
        //                msgbox.ShowDialog();

        //                stat.ShowGadgetState(Pointel.Statistics.Core.Utility.StatisticsEnum.GadgetState.Closed);
        //            }
        //        }
        //        else
        //        {
        //            if (StatisticsBase.GetInstance().IsMandatoryFieldsMissing)
        //            {
        //                string Message = string.Empty;
        //                int CurrentErrrorCount = 1;
        //                foreach (string MissingKeys in StatisticsBase.GetInstance().ListMissedValues)
        //                {
        //                    if (CurrentErrrorCount <= (StatisticsBase.GetInstance().ErrorCount == 0 ? 3 : StatisticsBase.GetInstance().ErrorCount))
        //                    {
        //                        if (Message == string.Empty)
        //                            Message = "'" + MissingKeys + "' value is Missing";
        //                        else
        //                            Message = Message + "\n '" + MissingKeys + "' value is Missing";
        //                    }
        //                    CurrentErrrorCount++;
        //                }
        //                Views.MessageBox msgbox = new Views.MessageBox();
        //                MessageBoxViewModel mboxviewmodel = new MessageBoxViewModel("Problem Encountered in Statistics", Message, msgbox, "MainWindow", Settings.GetInstance().Theme);
        //                msgbox.DataContext = mboxviewmodel;
        //                msgbox.ShowDialog();

        //                stat.ShowGadgetState(Pointel.Statistics.Core.Utility.StatisticsEnum.GadgetState.Closed);
        //            }
        //        }
        //    }
        //    catch (Exception generalException)
        //    {

        //        logger.Error("InitializeStatistics Method: Error occured while Initialize Statistics " +
        //            generalException.ToString());
        //    }
        //    finally
        //    {

        //        logger.Debug("PluginContainer : InitializeStatistics : Method Exit");
        //    }
        }


        #endregion

        #region IPlugin Members

        /// <summary>
        /// Closes the gadget.
        /// </summary>
        public void CloseGadget()
        {
            try
            {
                logger.Debug("PluginContainer : CloseGadget : Method Exit");
                StatisticsBase.GetInstance().isGadgetClose = true;
                Settings.GetInstance().IsPluginReaded = true;
                if (mainVm == null)
                    mainVm = new MainWindowViewModel();
                mainVm.gadgetclose();
                //commented
                stat.ShowGadgetState(Pointel.Statistics.Core.Utility.StatisticsEnum.GadgetState.Closed);
            }
            catch (Exception GeneralException)
            {
                logger.Error("PluginContainer : CloseGadget Method : Exception Caught : " + GeneralException.Message);
            }
            finally
            {
                logger.Debug("PluginContainer : CloseGadget : Method Exit");
            }
        }

        /// <summary>
        /// Shows the gadget.
        /// </summary>
        /// <param name="isStartup"></param>
        public void ShowGadget(bool isStartup)
        {
            bool isOpened = false;
            StatisticsBase.GetInstance().isPlugin = true;
            StatisticsBase.GetInstance().isGadgetClose = false;

            try
            {
                logger.Debug("PluginContainer : ShowGadget : Method Entry");

                if (StatisticsBase.GetInstance().IsMandatoryFieldsMissing)
                {
                    if (!isStartup)
                    {
                        string Message = string.Empty;
                        int CurrentErrrorCount = 1;
                        foreach (string MissingKeys in StatisticsBase.GetInstance().ListMissedValues)
                        {
                            if (CurrentErrrorCount <= (StatisticsBase.GetInstance().ErrorCount == 0 ? 3 : StatisticsBase.GetInstance().ErrorCount))
                            {
                                if (Message == string.Empty)
                                    Message = "'" + MissingKeys + "' value is Missing";
                                else
                                    Message = Message + "\n '" + MissingKeys + "' value is Missing";
                            }
                            CurrentErrrorCount++;
                        }
                        Views.MessageBox msgbox = new Views.MessageBox();
                        MessageBoxViewModel mboxviewmodel = new MessageBoxViewModel("Problem Encountered in Statistics", Message, msgbox, "MainWindow", Settings.GetInstance().Theme);
                        msgbox.DataContext = mboxviewmodel;
                        msgbox.ShowDialog();
                    }
                }
                else if (!isAuthenticated)
                {
                    if (!isStartup)
                    {
                        Views.MessageBox msgbox = new Views.MessageBox();
                        MessageBoxViewModel mboxviewmodel = new MessageBoxViewModel("Problem Encountered in Statistics", Settings.GetInstance().DictErrorValues["user.authorization"].ToString(), msgbox, "MainWindow", Settings.GetInstance().Theme);
                        msgbox.DataContext = mboxviewmodel;
                        msgbox.ShowDialog();
                    }
                }
                else
                {
                    Application.Current.Dispatcher.BeginInvoke((Action)(delegate
                    {
                        //foreach (Window appwindow in Application.Current.Windows)
                        //{
                        //    if (appwindow.Title == "StatGadget")
                        //    {
                        //        //
                        //        isOpened = true;
                        //        if (Settings.GetInstance().isMinimized)
                        //        {
                        //            Settings.GetInstance().isMinimized = false;
                         
                        //            appwindow.WindowState = WindowState.Maximized;
                        //            appwindow.Show();
                        //        }
                        //        else
                        //            appwindow.Show();
                        //        stat.ShowGadgetState(Pointel.Statistics.Core.Utility.StatisticsEnum.GadgetState.Opened);

                        //        break;
                        //    }
                        //}

                        MainWindow mainview = new MainWindow();
                        MainWindowViewModel mainVm = new MainWindowViewModel();
                        mainview.DataContext = mainVm;                      

                        mainview.Show();
                        mainVm.dragwindowInitialize(mainview);


                        if (!isOpened)
                        {
                           // MainWindow mainview = new MainWindow();
                            mainVm = new MainWindowViewModel();
                            mainview.DataContext = mainVm;
                            mainview.Show();
                            stat.ShowGadgetState(Pointel.Statistics.Core.Utility.StatisticsEnum.GadgetState.Opened);
                           // Window window = Application.Current.Windows[0];
                            //comment removed
                           // if (window != null)
                                mainVm.dragwindowInitialize(mainview);
                        }
                    }), DispatcherPriority.Background);
                }
            }
            catch (Exception GeneralException)
            {
                logger.Error("PluginContainer : ShowGadget Method : Exception Caught : " + GeneralException.Message);
            }
            finally
            {
                logger.Debug("PluginContainer : ShowGadget : Method Exit");
            }
        }

        #endregion

    }
}
