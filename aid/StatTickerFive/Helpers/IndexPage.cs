using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using StatTickerFive.Helpers;
using StatTickerFive.Views;
using StatTickerFive.ViewModels;
using System.Configuration;
using Pointel.Statistics.Core;
using Pointel.Statistics.Core.Utility;
using System.Xml;
using System.IO;
using System.Text;
using Genesyslab.Platform.Commons.Collections;
using System.Windows.Controls;
using System.Threading;
using System.IO.Pipes;
using System.Linq;


namespace StatTickerFive.Helpers
{
    /// <summary>
    /// 
    /// </summary>
    public class IndexPage
    {

        private static Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "STF");
        public Dictionary<string, string> userCredentials = null;
        public static bool IsClosed = false;
        private Thread pipeThread = null;


        /// <summary>
        /// Starts this instance.
        /// </summary>
        public void Start()
        {
            LoginWindowViewModel objLoginVM;
            userCredentials = new Dictionary<string, string>();
            XMLStorage objXmlReader = new XMLStorage();
            LoginWindow objLogin;


            try
            {
                logger.Debug("IndexPage : Start : Method Entry");

                objXmlReader.ReadAppSettings();
                objLoginVM = new LoginWindowViewModel();
                objLogin = new LoginWindow();
                userCredentials = objXmlReader.LoadParameters();

                //StatisticsBase.GetInstance().IsGenesysSupport = Settings.ReadApplicationConfigServer;

                if (userCredentials["apptype"] == StatisticsEnum.StatSource.StatServer.ToString() || userCredentials["apptype"] == StatisticsEnum.StatSource.All.ToString())
                {
                    if (userCredentials["ConfigHost"] == "" || userCredentials["userName"] == "" || userCredentials["applicationName"] == "" || userCredentials["ConfigPort"] == "" || userCredentials["place"] == "" || userCredentials["apptype"] == "")
                    {
                        Settings.GetInstance().isFirstTime = true;
                        objLogin.DataContext = objLoginVM;
                        objLogin.Show();
                        objLoginVM.SystemMenuHook(objLogin);
                    }
                    else
                    {
                        Settings.GetInstance().isFirstTime = false;
                        objLoginVM.UserName = userCredentials["userName"];
                        objLoginVM.Password = objXmlReader.DecodeFrom64(userCredentials["password"]);
                        objLoginVM.ApplicationName = userCredentials["applicationName"];
                        objLoginVM.Host = userCredentials["ConfigHost"];
                        objLoginVM.Port = userCredentials["ConfigPort"];
                        objLoginVM.Place = userCredentials["place"];
                        Settings.GetInstance().ApplicationType = userCredentials["apptype"];
                        Settings.GetInstance().DefaultAuthentication = userCredentials["authenticationType"];
                        //Microsoft.Windows.Controls.MessageBox.Show("Login calling1");
                        objLoginVM.Login();
                    }
                }
                else if (userCredentials["apptype"] == StatisticsEnum.StatSource.DB.ToString())
                {
                    if (userCredentials["userName"] == "" || userCredentials["apptype"] == "")
                    {
                        Settings.GetInstance().isFirstTime = true;
                        objLogin.DataContext = objLoginVM;
                        objLogin.Show();
                        objLoginVM.SystemMenuHook(objLogin);
                    }
                    else
                    {
                        Settings.GetInstance().isFirstTime = false;
                        objLoginVM.UserName = userCredentials["userName"];
                        objLoginVM.Password = objXmlReader.DecodeFrom64(userCredentials["password"]);
                        Settings.GetInstance().ApplicationType = userCredentials["apptype"];
                        //Microsoft.Windows.Controls.MessageBox.Show("Login 2");
                        objLoginVM.Login();
                    }
                }
            }
            catch (KeyNotFoundException ex)
            {

                //System.Windows.MessageBox.Show("IndexPage:  Start: " + ex.Message);
                logger.Info("IndexPage : Start Method : " + ex.Message);

                objLogin = null;
                objLogin = new LoginWindow();
                Settings.GetInstance().isFirstTime = true;
                objLoginVM = new LoginWindowViewModel();
                objLogin.DataContext = objLoginVM;
                objLogin.Show();
                objLoginVM.SystemMenuHook(objLogin);
            }
            catch (Exception ex)
            {
                //System.Windows.MessageBox.Show("IndexPage:  Start: " + ex.Message);
                logger.Error("IndexPage : Start Method : " + ex.Message);
            }
            finally
            {
                userCredentials = null;
                objXmlReader = null;
                objLogin = null;
                objLoginVM = null;
                GC.Collect();
                logger.Debug("IndexPage : Start : Method Exit");
            }
        }

        #region Creating Config files

        ///// <summary>
        ///// Creates the config.
        ///// </summary>
        //private void CreateConfig()
        //{
        //    XmlDocument doc;
        //    XmlNode[] Nodes = new XmlNode[50];
        //    XmlTextWriter writeUserDetails;
        //    string rootElement = "config";
        //    string loginParamsElement = "StatTickerFive";
        //    try
        //    {
        //        if (Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
        //                              "\\Pointel\\StatTickerFive"))
        //        {
        //            if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
        //                              "\\Pointel\\StatTickerFive\\config.xml"))
        //            {
        //                XmlDataDocument xmldoc = new XmlDataDocument();
        //                XmlNodeList xmlnode;
        //                FileStream fs = new FileStream(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Pointel\\StatTickerFive\\config.xml", FileMode.Open, FileAccess.Read);
        //                xmldoc.Load(fs);
        //                xmlnode = xmldoc.GetElementsByTagName("dbstatistics");
        //                fs.Close();

        //                if (xmlnode.Count == 0 || xmlnode[0].ChildNodes[0].InnerText == "")
        //                {

        //                    Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
        //                                               "\\Pointel\\StatTickerFive");
        //                    doc = new XmlDocument();

        //                    Nodes[0] = doc.CreateElement("config");
        //                    doc.AppendChild(Nodes[0]);

        //                    Nodes[1] = doc.CreateElement("StatTickerFive");
        //                    Nodes[0].AppendChild(Nodes[1]);

        //                    Nodes[2] = doc.CreateElement("dbstatistics");
        //                    Nodes[1].AppendChild(Nodes[2]);

        //                    #region Statistics properties

        //                    Nodes[3] = doc.CreateElement("db.statname");

        //                    Nodes[4] = doc.CreateElement("db.displayname");
        //                    Nodes[3].AppendChild(Nodes[4]);

        //                    Nodes[5] = doc.CreateElement("db.threshold1");
        //                    Nodes[3].AppendChild(Nodes[5]);

        //                    Nodes[6] = doc.CreateElement("db.threshold2");
        //                    Nodes[3].AppendChild(Nodes[6]);

        //                    Nodes[7] = doc.CreateElement("db.color1");
        //                    Nodes[3].AppendChild(Nodes[7]);

        //                    Nodes[8] = doc.CreateElement("db.color2");
        //                    Nodes[3].AppendChild(Nodes[8]);

        //                    Nodes[9] = doc.CreateElement("db.color3");
        //                    Nodes[3].AppendChild(Nodes[9]);

        //                    Nodes[10] = doc.CreateElement("db.tooltipname");
        //                    Nodes[3].AppendChild(Nodes[10]);

        //                    Nodes[11] = doc.CreateElement("db.format");
        //                    Nodes[3].AppendChild(Nodes[11]);

        //                    Nodes[12] = doc.CreateElement("db.query");
        //                    Nodes[3].AppendChild(Nodes[12]);

        //                    Nodes[2].AppendChild(Nodes[3]);

        //                    #endregion

        //                    Nodes[13] = doc.CreateElement("enable.disable-channels");
        //                    Nodes[1].AppendChild(Nodes[13]);

        //                    #region Enable Disable Channels

        //                    Nodes[14] = doc.CreateElement("statistics.enable-alwaysontop");
        //                    Nodes[13].AppendChild(Nodes[14]);
        //                    Nodes[15] = doc.CreateElement("statistics.enable-ccstat-aid");
        //                    Nodes[13].AppendChild(Nodes[15]);
        //                    Nodes[16] = doc.CreateElement("statistics.enable-log");
        //                    Nodes[13].AppendChild(Nodes[16]);
        //                    Nodes[17] = doc.CreateElement("statistics.enable-maingadget");
        //                    Nodes[13].AppendChild(Nodes[17]);
        //                    Nodes[18] = doc.CreateElement("statistics.enable-menu-button");
        //                    Nodes[13].AppendChild(Nodes[18]);
        //                    Nodes[19] = doc.CreateElement("statistics.enable-mystat-aid");
        //                    Nodes[13].AppendChild(Nodes[19]);
        //                    Nodes[20] = doc.CreateElement("statistics.enable-notification-balloon");
        //                    Nodes[13].AppendChild(Nodes[20]);
        //                    Nodes[21] = doc.CreateElement("statistics.enable-notification-close");
        //                    Nodes[13].AppendChild(Nodes[21]);
        //                    Nodes[22] = doc.CreateElement("statistics.enable-submenu-ccstatistics");
        //                    Nodes[13].AppendChild(Nodes[22]);
        //                    Nodes[23] = doc.CreateElement("statistics.enable-submenu-close-gadget");
        //                    Nodes[13].AppendChild(Nodes[23]);
        //                    Nodes[24] = doc.CreateElement("statistics.enable-submenu-mystatistics");
        //                    Nodes[13].AppendChild(Nodes[24]);
        //                    Nodes[25] = doc.CreateElement("statistics.enable-submenu-ontop");
        //                    Nodes[13].AppendChild(Nodes[25]);
        //                    Nodes[26] = doc.CreateElement("statistics.enable-tag-button");
        //                    Nodes[13].AppendChild(Nodes[26]);
        //                    Nodes[27] = doc.CreateElement("statistics.enable-tag-vertical");
        //                    Nodes[13].AppendChild(Nodes[27]);
        //                    Nodes[28] = doc.CreateElement("statistics.enable-untag-button");
        //                    Nodes[13].AppendChild(Nodes[28]);

        //                    #endregion

        //                    Nodes[29] = doc.CreateElement("statistics.log");
        //                    Nodes[1].AppendChild(Nodes[29]);

        //                    #region Log

        //                    Nodes[30] = doc.CreateElement("statistics.log.conversionpattern");
        //                    Nodes[29].AppendChild(Nodes[30]);

        //                    Nodes[31] = doc.CreateElement("statistics.log.datepattern");
        //                    Nodes[29].AppendChild(Nodes[31]);

        //                    Nodes[32] = doc.CreateElement("statistics.log.filepath");
        //                    Nodes[29].AppendChild(Nodes[32]);

        //                    Nodes[33] = doc.CreateElement("statistics.log.level");
        //                    Nodes[29].AppendChild(Nodes[33]);

        //                    Nodes[34] = doc.CreateElement("statistics.log.maxfilesize");
        //                    Nodes[29].AppendChild(Nodes[34]);

        //                    Nodes[35] = doc.CreateElement("statistics.log.maxrollbacksize");
        //                    Nodes[29].AppendChild(Nodes[35]);

        //                    #endregion

        //                    doc.Save(Environment.CurrentDirectory + @"\config.xml");

        //                    writeUserDetails =
        //                        new XmlTextWriter(
        //                            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
        //                            "\\Pointel\\StatTickerFive\\config.xml", Encoding.Default);

        //                    writeUserDetails.WriteStartElement(rootElement);

        //                    writeUserDetails.WriteStartElement(loginParamsElement);
        //                    //Genesys Parameters Writing

        //                    #region Stat Properties

        //                    writeUserDetails.WriteStartElement("dbstatistics");
        //                    writeUserDetails.WriteStartElement("db.statname");

        //                    writeUserDetails.WriteElementString("db.displayname", "");
        //                    writeUserDetails.WriteElementString("db.threshold1", "");
        //                    writeUserDetails.WriteElementString("db.threshold2", "");
        //                    writeUserDetails.WriteElementString("db.color1", "");
        //                    writeUserDetails.WriteElementString("db.color2", "");
        //                    writeUserDetails.WriteElementString("db.color3", "");
        //                    writeUserDetails.WriteElementString("db.tooltipname", "");
        //                    writeUserDetails.WriteElementString("db.format", "");
        //                    writeUserDetails.WriteElementString("db.query", "");
        //                    writeUserDetails.WriteEndElement();
        //                    writeUserDetails.WriteEndElement();

        //                    #endregion

        //                    #region Enable Disable Channels

        //                    writeUserDetails.WriteStartElement("enable.disable-channels");

        //                    writeUserDetails.WriteElementString("statistics.enable-alwaysontop", "");
        //                    writeUserDetails.WriteElementString("statistics.enable-ccstat-aid", "");
        //                    writeUserDetails.WriteElementString("statistics.enable-log", "");
        //                    writeUserDetails.WriteElementString("statistics.enable-maingadget", "");
        //                    writeUserDetails.WriteElementString("statistics.enable-menu-button", "");
        //                    writeUserDetails.WriteElementString("statistics.enable-mystat-aid", "");
        //                    writeUserDetails.WriteElementString("statistics.enable-notification-balloon", "");
        //                    writeUserDetails.WriteElementString("statistics.enable-notification-close", "");
        //                    writeUserDetails.WriteElementString("statistics.enable-submenu-ccstatistics", "");
        //                    writeUserDetails.WriteElementString("statistics.enable-submenu-close-gadget", "");
        //                    writeUserDetails.WriteElementString("statistics.enable-submenu-mystatistics", "");
        //                    writeUserDetails.WriteElementString("statistics.enable-submenu-ontop", "");
        //                    writeUserDetails.WriteElementString("statistics.enable-tag-button", "");
        //                    writeUserDetails.WriteElementString("statistics.enable-tag-vertical", "");
        //                    writeUserDetails.WriteElementString("statistics.enable-untag-button", "");

        //                    writeUserDetails.WriteEndElement();

        //                    #endregion

        //                    #region Logs

        //                    writeUserDetails.WriteStartElement("statistics.log");

        //                    writeUserDetails.WriteElementString("statistics.log.conversionpattern", "");
        //                    writeUserDetails.WriteElementString("statistics.log.datepattern", "");
        //                    writeUserDetails.WriteElementString("statistics.log.filepath", "");
        //                    writeUserDetails.WriteElementString("statistics.log.level", "");
        //                    writeUserDetails.WriteElementString("statistics.log.maxfilesize", "");
        //                    writeUserDetails.WriteElementString("statistics.log.maxrollbacksize", "");

        //                    writeUserDetails.WriteEndElement();

        //                    #endregion

        //                    writeUserDetails.WriteEndElement();

        //                    writeUserDetails.Close();
        //                    writeUserDetails = null;
        //                }
        //            }
        //        }


        //    }
        //    catch (Exception GeneralException)
        //    {
        //        System.Windows.Forms.MessageBox.Show("Exception Caught " + GeneralException.Message);
        //    }
        //    finally
        //    {
        //        GC.Collect();
        //    }
        //}

        #endregion

        public void StartStatisticsPipeListerner()
        {
            pipeThread = new Thread(() =>
            {
                //System.Windows.MessageBox.Show("Stat : Show");
                try
                {
                    bool exitCall = false;
                    NamedPipeServerStream namedPipeServerStream = new NamedPipeServerStream("AIDPipe_" + Settings.GetInstance().UserName, PipeDirection.InOut, 2, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);

                    while (true && !exitCall)
                    {
                        byte[] msgBuff;
                        while (true)
                        {
                            namedPipeServerStream.WaitForConnection();
                            if (namedPipeServerStream.IsConnected)
                            {
                                string data = null;
                                msgBuff = new byte[8];
                                namedPipeServerStream.Read(msgBuff, 0, msgBuff.Length);
                                data = System.Text.Encoding.UTF8.GetString(msgBuff, 0, msgBuff.Length);
                                namedPipeServerStream.Disconnect();

                                Application.Current.Dispatcher.BeginInvoke((Action)(delegate
                                {
                                    try
                                    {
                                        
                                        if (data.Contains("1"))
                                        {
                                            //Show Gadget
                                            //System.Windows.MessageBox.Show("Stat : Show");
                                            logger.Debug("IndexPage : StartStatisticsPipeListerner : Show Gadget");                                           
                                            StatisticsBase.GetInstance().isPlugin = true;
                                            if (Settings.GetInstance().mainview == null)
                                            {
                                                //Output = stat.ConfigConnectionEstablish(Host, Port, ApplicationName, UserName, Password, UserName, Settings.GetInstance().ApplicationType, String.Empty, true);
                                                //Output = StatisticsBase.GetInstance().ConfigConnectionEstablish(_dataContext.ApplicationName, _configContainer.ConfServiceObject, _dataContext.UserName, _dataContext.UserName, "StatServer", agentGroup);
                                                Settings.GetInstance().mainview = new MainWindow();
                                                if (Settings.GetInstance().mainVm == null)
                                                    Settings.GetInstance().mainVm = new MainWindowViewModel();
                                                Settings.GetInstance().mainview.DataContext = Settings.GetInstance().mainVm;
                                            }


                                            Settings.GetInstance().mainview.Show();
                                            Settings.GetInstance().mainVm.dragwindowInitialize(Settings.GetInstance().mainview);

                                        }
                                        else if (data.Contains("2"))
                                        {
                                            //Hide Gadget
                                            //System.Windows.MessageBox.Show("Stat : Hide");
                                            logger.Debug("IndexPage : StartStatisticsPipeListerner : Hide Gadget");
                                            StatisticsBase.GetInstance().isPlugin = true;
                                            if (Settings.GetInstance().mainVm != null)
                                                Settings.GetInstance().mainVm.gadgetclose(false);
                                        }
                                        if (data.Contains("3"))
                                        {
                                            //CLose Application if (Settings.GetInstance().mainVm == null)
                                            //System.Windows.MessageBox.Show("Stat : Close");
                                            logger.Debug("IndexPage : StartStatisticsPipeListerner : Close Gadget");
                                            StatisticsBase.GetInstance().isPlugin = true;
                                            if (Settings.GetInstance().mainVm != null)
                                                Settings.GetInstance().mainVm.gadgetclose();
                                            exitCall = true;
                                        }
                                        //if (data.Contains("4"))
                                        //{
                                        //    try
                                        //    {
                                        //        //Microsoft.Windows.Controls.MessageBox.Show("Display Object Config");

                                        //        var queueSelection = IsWindowOpen<Window>("ObjectConfigurations");
                                        //        if (queueSelection != null)
                                        //        {
                                        //            System.Windows.Threading.Dispatcher.CurrentDispatcher.BeginInvoke((Action)delegate
                                        //            {
                                        //                queueSelection.Close();
                                        //                Settings.GetInstance().QueueConfigWin = new ObjectConfigWindow();
                                        //                Settings.GetInstance().QueueConfigVM = new ObjectConfigWindowViewModel();
                                        //                Settings.GetInstance().QueueConfigWin.DataContext = Settings.GetInstance().QueueConfigVM;
                                        //                Settings.GetInstance().QueueConfigWin.Show();
                                        //            });
                                        //        }
                                        //        else
                                        //        {
                                        //            System.Windows.Threading.Dispatcher.CurrentDispatcher.BeginInvoke((Action)delegate
                                        //            {
                                        //                Settings.GetInstance().QueueConfigWin = new ObjectConfigWindow();
                                        //                Settings.GetInstance().QueueConfigVM = new ObjectConfigWindowViewModel();
                                        //                Settings.GetInstance().QueueConfigWin.DataContext = Settings.GetInstance().QueueConfigVM;
                                        //                Settings.GetInstance().QueueConfigWin.Show();
                                        //            });
                                        //        }

                                        //        //Microsoft.Windows.Controls.MessageBox.Show("Display Object Config");

                                        //        //var queueSelection = IsWindowOpen<Window>("ObjectConfigurations");
                                        //        //if (queueSelection != null)
                                        //        //{
                                        //        //    System.Windows.Threading.Dispatcher.CurrentDispatcher.BeginInvoke((Action)delegate
                                        //        //    {
                                        //        //        Microsoft.Windows.Controls.MessageBox.Show("VM not null queue selection not null");
                                        //        //        queueSelection.Close();
                                        //        //        ObjectConfigWindow objView = new ObjectConfigWindow();
                                        //        //        objView.DataContext = Settings.GetInstance().QueueConfigVM;
                                        //        //        objView.Show();
                                        //        //    });
                                        //        //}
                                        //        //else
                                        //        //{
                                        //        //    System.Windows.Threading.Dispatcher.CurrentDispatcher.BeginInvoke((Action)delegate
                                        //        //    {
                                        //        //        Microsoft.Windows.Controls.MessageBox.Show("VM not null queue selection null");
                                        //        //        ObjectConfigWindow objView = new ObjectConfigWindow();
                                        //        //        Settings.GetInstance().QueueConfigVM = new ObjectConfigWindowViewModel();
                                        //        //        objView.DataContext = Settings.GetInstance().QueueConfigVM;
                                        //        //        objView.Show();
                                        //        //    });
                                        //        //}


                                        //        ////if (Settings.GetInstance().ObjectConfigVM == null)
                                        //        ////{
                                        //        ////    Microsoft.Windows.Controls.MessageBox.Show("VM null");
                                        //        ////    ObjectConfigWindow objView = new ObjectConfigWindow();
                                        //        ////    Settings.GetInstance().ObjectConfigVM = new ObjectConfigWindowViewModel();
                                        //        ////    Settings.GetInstance().ObjectConfigVM.LoadObjectConfiguration();
                                        //        ////    objView.DataContext = Settings.GetInstance().ObjectConfigVM;
                                        //        ////    objView.Show();
                                        //        ////}
                                        //        ////else
                                        //        ////{
                                        //        ////    Microsoft.Windows.Controls.MessageBox.Show("VM not null ");

                                        //        ////    foreach (Window currentwindow in System.Windows.Application.Current.Windows)
                                        //        ////    {
                                        //        ////        if (currentwindow.Title == "ObjectConfigurations")
                                        //        ////        {
                                        //        ////            currentwindow.Show();
                                        //        ////        }
                                        //        ////    }
                                        //        ////    var queueSelection = IsWindowOpen<Window>("ObjectConfigurations");
                                        //        ////        if (queueSelection != null)
                                        //        ////        {
                                        //        ////            Microsoft.Windows.Controls.MessageBox.Show("VM not null queue selection not null");
                                        //        ////            queueSelection.Close();
                                        //        ////            ObjectConfigWindow objView = new ObjectConfigWindow();
                                        //        ////            objView.DataContext = Settings.GetInstance().ObjectConfigVM;
                                        //        ////            objView.Show();
                                        //        ////        }
                                        //        ////        else
                                        //        ////        {
                                        //        ////            Microsoft.Windows.Controls.MessageBox.Show("VM not null queue selection null");
                                        //        ////            ObjectConfigWindow objView = new ObjectConfigWindow();
                                        //        ////            objView.DataContext = Settings.GetInstance().ObjectConfigVM;
                                        //        ////            objView.Show();
                                        //        ////        }

                                        //        ////}
                                        //    }
                                        //    catch (Exception ex)
                                        //    {
                                        //        //Microsoft.Windows.Controls.MessageBox.Show("Exception opening : " + ex.Message);
                                        //        logger.Error("IndexPage : StartStatisticsPipeListerner Method : " + ex.Message);
                                        //    }
                                        //}
                                    }
                                    catch (Exception ex)
                                    {
                                        //System.Windows.MessageBox.Show("Indexpage 1 : " + ex.Message);
                                        logger.Error("IndexPage : StartStatisticsPipeListerner Method : " + ex.Message);
                                    }
                                }));
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    //System.Windows.MessageBox.Show("Indexpage : " + ex.Message);
                    logger.Error("IndexPage : StartStatisticsPipeListerner Method : " + ex.Message);
                }
            });
            pipeThread.Start();
        }

        public static T IsWindowOpen<T>(string title = null)
            where T : Window
        {
            var windows = Application.Current.Windows.OfType<T>();
            return string.IsNullOrEmpty(title) ? windows.FirstOrDefault() : windows.FirstOrDefault(w => w.Title.Equals(title));
        }
    }
}
