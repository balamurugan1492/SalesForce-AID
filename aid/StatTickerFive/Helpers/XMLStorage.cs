using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using System.Xml.XPath;
using Genesyslab.Platform.Commons.Collections;

namespace StatTickerFive.Helpers
{
    /// <summary>
    /// XMLStorage
    /// </summary>
    internal class XMLStorage
    {
        #region Private Members
        private const string rootElement = "config";
        private const string loginParamsElement = "StatTickerFive";
        private static Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "STF");
        #endregion

        #region SaveInitializeParameters

        /// <summary>
        /// Saves the initialize parameters.
        /// </summary>
        /// <param name="applicationName">Name of the application.</param>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        /// <param name="ConfigHost">The config host.</param>
        /// <param name="ConfigPort">The config port.</param>
        /// <returns></returns>
        public bool SaveInitializeParameters(string applicationName, string userName, string password, string Place, string ConfigHost,
                                             string ConfigPort, string applicationType, string AuthenticationType)
        {
            bool isSaved = false;
            XmlTextWriter writeUserDetails;
            XmlDocument doc = new XmlDocument();
            try
            {
                logger.Debug("XMLStorage : SaveInitializeParameters : Method Entry");

                writeUserDetails =
                    new XmlTextWriter(
                        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
                       "\\PHS\\AgentInteractionDesktop\\login_config.xml", Encoding.Default);

                writeUserDetails.WriteStartElement(rootElement);
                writeUserDetails.WriteStartElement(loginParamsElement);
                //Genesys Parameters Writing

                writeUserDetails.WriteStartElement("Initialize");


                writeUserDetails.WriteElementString("username", userName);
                writeUserDetails.WriteElementString("password", password);
                writeUserDetails.WriteElementString("place", Place);
                writeUserDetails.WriteElementString("applicationName", applicationName);
                writeUserDetails.WriteElementString("configHost", ConfigHost);
                writeUserDetails.WriteElementString("configPort", ConfigPort);
                writeUserDetails.WriteElementString("apptype", applicationType);
                writeUserDetails.WriteElementString("authenticationType", AuthenticationType);


                writeUserDetails.WriteEndElement();

                //writeUserDetails.WriteStartElement("Login");
                //writeUserDetails.WriteEndElement();


                writeUserDetails.Close();
                writeUserDetails = null;

            }
            catch (Exception generalException)
            {
                logger.Error("XML Storage Class : SaveInitialixeParameters Method : Exception caught : " +
                             generalException.Message);
            }
            finally
            {
                writeUserDetails = null;
                GC.Collect();
                logger.Debug("XMLStorage : SaveInitializeParameters : Method Exit");
            }
            return isSaved;
        }
        #endregion

        #region LoadParameters
        /// <summary>
        /// Loads the parameters.
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> LoadParameters()
        {
            Dictionary<string, string> output = new Dictionary<string, string>();
            StreamReader readUserDetails = null;
            try
            {
                logger.Debug("XMLStorage : LoadParameters : Method Entry");

                if (Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
                                      "\\PHS\\AgentInteractionDesktop"))
                {
                    if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
                                      "\\PHS\\AgentInteractionDesktop\\login_config.xml"))
                    {
                        readUserDetails = new StreamReader(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
                                          "\\PHS\\AgentInteractionDesktop\\login_config.xml");
                    }
                }
                else
                {
                    Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
                                              "\\PHS\\AgentInteractionDesktop");
                }

            }
            catch (Exception ex)
            {
                logger.Error("XML Storage Class : SaveInitialixeParameters Method : Exception caught : " + ex.Message);
            }
            finally
            {
                GC.Collect();
            }
            XmlDocument configXml = new XmlDocument();
            if (readUserDetails != null)
            {
                try
                {
                    configXml.LoadXml(readUserDetails.ReadToEnd());
                }
                catch
                {
                    readUserDetails.Dispose();
                }
                finally
                {
                    GC.Collect();
                }

                readUserDetails.Dispose();

                XPathNavigator navigator = configXml.CreateNavigator();

                XPathNodeIterator nodeIterator = (XPathNodeIterator)navigator.Evaluate(rootElement + "/" + loginParamsElement + "/Initialize/location");

                if (nodeIterator != null && nodeIterator.Count != 0)
                {
                    nodeIterator.MoveNext();
                    output.Add("location", nodeIterator.Current.ToString().Trim());
                }

                nodeIterator = (XPathNodeIterator)navigator.Evaluate(rootElement + "/" + loginParamsElement + "/Initialize/configHost");
                if (nodeIterator != null && nodeIterator.Count != 0)
                {
                    nodeIterator.MoveNext();
                    output.Add("ConfigHost", nodeIterator.Current.ToString().Trim());
                }

                nodeIterator = (XPathNodeIterator)navigator.Evaluate(rootElement + "/" + loginParamsElement + "/Initialize/configPort");
                if (nodeIterator != null && nodeIterator.Count != 0)
                {
                    nodeIterator.MoveNext();
                    output.Add("ConfigPort", nodeIterator.Current.ToString().Trim());
                }

                nodeIterator = (XPathNodeIterator)navigator.Evaluate(rootElement + "/" + loginParamsElement + "/Initialize/applicationName");
                if (nodeIterator != null && nodeIterator.Count != 0)
                {
                    nodeIterator.MoveNext();
                    output.Add("applicationName", nodeIterator.Current.ToString().Trim());
                }

                nodeIterator = (XPathNodeIterator)navigator.Evaluate(rootElement + "/" + loginParamsElement + "/Initialize/username");
                if (nodeIterator != null && nodeIterator.Count != 0)
                {
                    nodeIterator.MoveNext();
                    output.Add("userName", nodeIterator.Current.ToString().Trim());
                }

                nodeIterator = (XPathNodeIterator)navigator.Evaluate(rootElement + "/" + loginParamsElement + "/Initialize/password");
                if (nodeIterator != null && nodeIterator.Count != 0)
                {
                    nodeIterator.MoveNext();
                    output.Add("password", nodeIterator.Current.ToString().Trim());
                }

                nodeIterator = (XPathNodeIterator)navigator.Evaluate(rootElement + "/" + loginParamsElement + "/Initialize/place");
                if (nodeIterator != null && nodeIterator.Count != 0)
                {
                    nodeIterator.MoveNext();
                    output.Add("place", nodeIterator.Current.ToString().Trim());
                }

                nodeIterator = (XPathNodeIterator)navigator.Evaluate(rootElement + "/" + loginParamsElement + "/Initialize/apptype");
                if (nodeIterator != null && nodeIterator.Count != 0)
                {
                    nodeIterator.MoveNext();
                    Settings.GetInstance().ApplicationType = nodeIterator.Current.ToString().Trim();
                    output.Add("apptype", nodeIterator.Current.ToString().Trim());
                }

                nodeIterator = (XPathNodeIterator)navigator.Evaluate(rootElement + "/" + loginParamsElement + "/Initialize/authenticationType");
                if (nodeIterator != null && nodeIterator.Count != 0)
                {
                    nodeIterator.MoveNext();
                    Settings.GetInstance().ApplicationType = nodeIterator.Current.ToString().Trim();
                    output.Add("authenticationType", nodeIterator.Current.ToString().Trim());
                }

                navigator = null;
                nodeIterator = null;
            }
            logger.Debug("XMLStorage : LoadParameters : Method Exit");
            return output;
        }
        #endregion

        //#region Read Agent Configuration
        //public Dictionary<string,string> ReadAgentConfiguration()
        //{
        //    Dictionary<string, string> output = new Dictionary<string, string>();
        //    StreamReader readUserDetails = null;
        //    try
        //    {
        //        logger.Debug("XMLStorage : ReadAgentConfiguration : Method Entry");

        //        if (Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
        //                              "\\Pointel\\AgentInteractionDesktop"))
        //        {
        //            if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
        //                              "\\Pointel\\AgentInteractionDesktop\\login_config.xml"))
        //            {
        //                readUserDetails = new StreamReader(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
        //                                  "\\Pointel\\AgentInteractionDesktop\\login_config.xml");
        //            }
        //        }
        //        //else
        //        //{
        //        //    Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
        //        //                              "\\Pointel\\AgentInteractionDesktop");
        //        //}

        //    }
        //    catch (Exception ex)
        //    {
        //        logger.Error("XML Storage Class : ReadAgentConfiguration Method : Exception caught : " + ex.Message);
        //    }
        //    finally
        //    {
        //        GC.Collect();
        //    }
        //    XmlDocument configXml = new XmlDocument();
        //    if (readUserDetails != null)
        //    {
        //        try
        //        {
        //            configXml.LoadXml(readUserDetails.ReadToEnd());
        //        }
        //        catch
        //        {
        //            readUserDetails.Dispose();
        //        }
        //        finally
        //        {
        //            GC.Collect();
        //        }

        //        readUserDetails.Dispose();

        //        XPathNavigator navigator = configXml.CreateNavigator();

        //        XPathNodeIterator nodeIterator = (XPathNodeIterator)navigator.Evaluate("AgentConfig/StatTickerFive/agent-tagged-statistics");

        //        if (nodeIterator != null && nodeIterator.Count != 0)
        //        {
        //            nodeIterator.MoveNext();
        //            output.Add("agent-tagged-statistics", nodeIterator.Current.ToString().Trim());
        //        }

        //        nodeIterator = (XPathNodeIterator)navigator.Evaluate("AgentConfig/StatTickerFive/statistics.gadget-position");
        //        if (nodeIterator != null && nodeIterator.Count != 0)
        //        {
        //            nodeIterator.MoveNext();
        //            output.Add("statistics.gadget-position", nodeIterator.Current.ToString().Trim());
        //        }

        //        nodeIterator = (XPathNodeIterator)navigator.Evaluate("AgentConfig/StatTickerFive/statistics.enable-header");
        //        if (nodeIterator != null && nodeIterator.Count != 0)
        //        {
        //            nodeIterator.MoveNext();
        //            output.Add("statistics.enable-header", nodeIterator.Current.ToString().Trim());
        //        }

        //        nodeIterator = (XPathNodeIterator)navigator.Evaluate("AgentConfig/StatTickerFive/agent-tagged-statistics");
        //        if (nodeIterator != null && nodeIterator.Count != 0)
        //        {
        //            nodeIterator.MoveNext();
        //            output.Add("agent-tagged-statistics", nodeIterator.Current.ToString().Trim());
        //        }

        //        nodeIterator = (XPathNodeIterator)navigator.Evaluate("AgentConfig/StatTickerFive/statistics.enable-tag-vertical");
        //        if (nodeIterator != null && nodeIterator.Count != 0)
        //        {
        //            nodeIterator.MoveNext();
        //            output.Add("statistics.enable-tag-vertical", nodeIterator.Current.ToString().Trim());
        //        }
                
        //        navigator = null;
        //        nodeIterator = null;
        //    }
        //    logger.Debug("XMLStorage : ReadAgentConfiguration : Method Exit");
        //    return output;
            
        //}
        //#endregion

        #region Read DB Settings

        /// <summary>
        /// Reads the database settings.
        /// </summary>
        public void ReadDBSettings()
        {
            try
            {
                logger.Debug("XMLStorage : ReadDBSettings : Method Entry");

                XmlDataDocument xmldoc = new XmlDataDocument();
                XmlNodeList xmlnode;
                int i = 0;
                FileStream fs = new FileStream(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\PHS\\AgentInteractionDesktop\\app_db_config.xml", FileMode.Open, FileAccess.Read);
                xmldoc.Load(fs);

                #region DB Settings

                xmlnode = xmldoc.GetElementsByTagName("DbSetting");
                for (i = 0; i <= xmlnode[0].ChildNodes.Count - 1; i++)
                {
                    XmlNodeList xmlInnernode = xmldoc.GetElementsByTagName(xmlnode[0].ChildNodes[i].Name.ToString());
                    if (xmlInnernode.Count != 0)
                    {
                        for (int j = 0; j <= xmlInnernode[0].ChildNodes.Count - 1; j++)
                        {
                            string sss = xmlInnernode[0].ChildNodes[j].ParentNode.Name.ToString().Trim().ToLower();
                            if (xmlInnernode[0].ChildNodes[j].ParentNode.Name.ToString().Trim().ToLower() == "db-type")
                                Settings.GetInstance().dbType = xmlInnernode[0].ChildNodes[j].InnerText.Trim();
                            else if (xmlInnernode[0].ChildNodes[j].ParentNode.Name.ToString().Trim().ToLower() == "db-host")
                                Settings.GetInstance().dbHost = xmlInnernode[0].ChildNodes[j].InnerText.Trim();
                            else if (xmlInnernode[0].ChildNodes[j].ParentNode.Name.ToString().Trim().ToLower() == "db-port")
                                Settings.GetInstance().dbPort = xmlInnernode[0].ChildNodes[j].InnerText.Trim();
                            else if (xmlInnernode[0].ChildNodes[j].ParentNode.Name.ToString().Trim().ToLower() == "db-name")
                                Settings.GetInstance().dbName = xmlInnernode[0].ChildNodes[j].InnerText.Trim();
                            else if (xmlInnernode[0].ChildNodes[j].ParentNode.Name.ToString().Trim().ToLower() == "db-username")
                                Settings.GetInstance().dbUsername = xmlInnernode[0].ChildNodes[j].InnerText.Trim();
                            else if (xmlInnernode[0].ChildNodes[j].ParentNode.Name.ToString().Trim().ToLower() == "db-password")
                                Settings.GetInstance().dbPassword = xmlInnernode[0].ChildNodes[j].InnerText.Trim();
                            else if (xmlInnernode[0].ChildNodes[j].ParentNode.Name.ToString().Trim().ToLower() == "db-sid")
                                Settings.GetInstance().dbSid = xmlInnernode[0].ChildNodes[j].InnerText.Trim();
                            else if (xmlInnernode[0].ChildNodes[j].ParentNode.Name.ToString().Trim().ToLower() == "db-sname")
                                Settings.GetInstance().dbSname = xmlInnernode[0].ChildNodes[j].InnerText.Trim();
                            else if (xmlInnernode[0].ChildNodes[j].ParentNode.Name.ToString().Trim().ToLower() == "db-source")
                                Settings.GetInstance().dbSource = xmlInnernode[0].ChildNodes[j].InnerText.Trim();
                            else if (xmlInnernode[0].ChildNodes[j].ParentNode.Name.ToString().Trim().ToLower() == "db-loginquery")
                                Settings.GetInstance().dbLoginQuery = xmlInnernode[0].ChildNodes[j].InnerText.Trim();

                        }
                    }
                }

                #endregion

            }
            catch (Exception GeneralException)
            {
                logger.Error("XMLStorage : ReadDBSettings : Exception caught : " + GeneralException.Message);
            }
            finally
            {
                logger.Debug("XMLStorage : ReadDBSettings : Method Exit");
            }
        }

        #endregion

        #region Read AppSettings

        /// <summary>
        /// Reads the application settings.
        /// </summary>
        public void ReadAppSettings()
        {
            try
            {
                logger.Debug("XMLStorage : ReadAppSettings : Method Entry");


                //System.Windows.Forms.MessageBox.Show("XMLStorage : ReadAppSettings() : Started");

                XmlDataDocument xmldoc = new XmlDataDocument();
                XmlNodeList xmlnode;
                int i = 0;
                FileStream fs = new FileStream(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\PHS\\AgentInteractionDesktop\\app_config.xml", FileMode.Open, FileAccess.Read);
                xmldoc.Load(fs);

                #region Application Settings

                xmlnode = xmldoc.GetElementsByTagName("AppSetting");
                for (i = 0; i <= xmlnode[0].ChildNodes.Count - 1; i++)
                {
                    XmlNodeList xmlInnernode = xmldoc.GetElementsByTagName(xmlnode[0].ChildNodes[i].Name.ToString());
                    if (xmlInnernode.Count != 0)
                    {
                        for (int j = 0; j <= xmlInnernode[0].ChildNodes.Count - 1; j++)
                        {
                            string sss = xmlInnernode[0].ChildNodes[j].ParentNode.Name.ToString().Trim().ToLower();
                            if (string.Compare(xmlInnernode[0].ChildNodes[j].ParentNode.Name.ToString().Trim(),"host",true)==0)
                                Settings.GetInstance().Host = xmlInnernode[0].ChildNodes[j].InnerText.Trim();
                            else if (string.Compare(xmlInnernode[0].ChildNodes[j].ParentNode.Name.ToString().Trim(),"port",true)==0)
                                Settings.GetInstance().Port = xmlInnernode[0].ChildNodes[j].InnerText.Trim();
                            else if (string.Compare(xmlInnernode[0].ChildNodes[j].ParentNode.Name.ToString().Trim(),"applicationname",true)==0)
                                Settings.GetInstance().ApplicationName = xmlInnernode[0].ChildNodes[j].InnerText.Trim();
                            else if (string.Compare(xmlInnernode[0].ChildNodes[j].ParentNode.Name.ToString().Trim(),"place",true)==0)
                                Settings.GetInstance().Place = xmlInnernode[0].ChildNodes[j].InnerText.Trim();
                            else if (string.Compare(xmlInnernode[0].ChildNodes[j].ParentNode.Name.ToString().Trim(),"enablepassword",true)==0)
                                Settings.GetInstance().IsPasswordEnabled = Convert.ToBoolean(xmlInnernode[0].ChildNodes[j].InnerText.Trim());
                            else if (string.Compare(xmlInnernode[0].ChildNodes[j].ParentNode.Name.ToString().Trim(),"enablehost",true)==0)
                                Settings.GetInstance().IsHostEnabled = Convert.ToBoolean(xmlInnernode[0].ChildNodes[j].InnerText.Trim());
                            else if (string.Compare(xmlInnernode[0].ChildNodes[j].ParentNode.Name.ToString().Trim(),"enableport",true)==0)
                                Settings.GetInstance().IsPortEnabled = Convert.ToBoolean(xmlInnernode[0].ChildNodes[j].InnerText.Trim());
                            else if (string.Compare(xmlInnernode[0].ChildNodes[j].ParentNode.Name.ToString().Trim(),"enableapplicationname",true)==0)
                                Settings.GetInstance().IsAppNameEnabled = Convert.ToBoolean(xmlInnernode[0].ChildNodes[j].InnerText.Trim());
                            else if (string.Compare(xmlInnernode[0].ChildNodes[j].ParentNode.Name.ToString().Trim() ,"enableapptype",true)==0)
                                Settings.GetInstance().IsAppTypeEnabled = Convert.ToBoolean(xmlInnernode[0].ChildNodes[j].InnerText.Trim());
                            else if (string.Compare(xmlInnernode[0].ChildNodes[j].ParentNode.Name.ToString().Trim(),"theme",true)==0)
                                Settings.GetInstance().Theme = xmlInnernode[0].ChildNodes[j].InnerText.Trim();
                            else if (string.Compare(xmlInnernode[0].ChildNodes[j].ParentNode.Name.ToString().Trim(), "position",true)==0)
                                Settings.GetInstance().Position = xmlInnernode[0].ChildNodes[j].InnerText.Trim();
                            else if (string.Compare(xmlInnernode[0].ChildNodes[j].ParentNode.Name.ToString().Trim(),"gadgetwidth",true)==0)
                                Settings.GetInstance().Width = Convert.ToDouble(xmlInnernode[0].ChildNodes[j].InnerText.Trim());
                            else if (string.Compare(xmlInnernode[0].ChildNodes[j].ParentNode.Name.ToString().Trim(),"displaytime",true)==0)
                                Settings.GetInstance().DisplayTime = Convert.ToInt32(xmlInnernode[0].ChildNodes[j].InnerText.Trim());
                            else if (string.Compare(xmlInnernode[0].ChildNodes[j].ParentNode.Name.ToString().Trim(),"statisticsbold",true)==0)
                                Settings.GetInstance().IsStatBold = Convert.ToBoolean(xmlInnernode[0].ChildNodes[j].InnerText.Trim());
                            else if (string.Compare(xmlInnernode[0].ChildNodes[j].ParentNode.Name.ToString().Trim(), "ErrorDisplayCount", true) == 0)
                            {
                                if (xmlInnernode[0].ChildNodes[j].InnerText.Trim() == "0")
                                    Settings.GetInstance().ErrorDisplayCount = 1;
                                else if (xmlInnernode[0].ChildNodes[j].InnerText.Trim() == string.Empty)
                                    Settings.GetInstance().ErrorDisplayCount = 3;
                                else
                                    Settings.GetInstance().ErrorDisplayCount = Convert.ToInt32(xmlInnernode[0].ChildNodes[j].InnerText.Trim());
                            }
                            else if (string.Compare(xmlInnernode[0].ChildNodes[j].ParentNode.Name.ToString().Trim(), "defaultauthenticationtype", true) == 0)
                            {
                                Settings.GetInstance().DefaultAuthentication = xmlInnernode[0].ChildNodes[j].InnerText.Trim();
                            }
                            else if (string.Compare(xmlInnernode[0].ChildNodes[j].ParentNode.Name.ToString().Trim(), "defaultusername", true) == 0)
                                Settings.GetInstance().DefaultUsername = xmlInnernode[0].ChildNodes[j].InnerText.Trim();
                            else if (string.Compare(xmlInnernode[0].ChildNodes[j].ParentNode.Name.ToString().Trim(), "defaultpassword", true) == 0)
                                Settings.GetInstance().DefaultPassword = xmlInnernode[0].ChildNodes[j].InnerText.Trim();
                            if (!Settings.GetInstance().IsAppTypeEnabled)
                                if (xmlInnernode[0].ChildNodes[j].ParentNode.Name.ToString().Trim().ToLower() == "defaultapptype")
                                    Settings.GetInstance().ApplicationType = xmlInnernode[0].ChildNodes[j].InnerText.Trim();
                        }
                    }
                }

                if (Settings.GetInstance().ErrorDisplayCount == 0)
                    Settings.GetInstance().ErrorDisplayCount = 3;

                xmldoc = null;

                #endregion

            }
            catch (Exception GeneralException)
            {
                //System.Windows.Forms.MessageBox.Show("XMLStorage : ReadAppSettings() : Exception : " + GeneralException.Message);
                logger.Error("XMLStorage : ReadAppSettings : Exception caught : " + GeneralException.Message);
            }
            finally
            {
                //System.Windows.Forms.MessageBox.Show("XMLStorage : ReadAppSettings() : Started");
                logger.Debug("XMLStorage : ReadAppSettings : Method Exit");
            }
        }

        #endregion

        # region CreateXMLFile
        //<summary>
        //Creates the config.
        //</summary>
        private void CreateConfig()
        {
            XmlDocument doc;
            XmlNode[] Nodes = new XmlNode[10];
            try
            {
                logger.Debug("XMLStorage : CreateConfig : Method Entry");
                doc = new XmlDocument();

                Nodes[0] = doc.CreateElement("config");
                doc.AppendChild(Nodes[0]);

                Nodes[1] = doc.CreateElement("StatTickerFive");
                Nodes[0].AppendChild(Nodes[1]);

                Nodes[2] = doc.CreateElement("Initialize");

                Nodes[3] = doc.CreateElement("username");
                Nodes[2].AppendChild(Nodes[3]);

                Nodes[4] = doc.CreateElement("password");
                Nodes[2].AppendChild(Nodes[4]);

                Nodes[5] = doc.CreateElement("applicationName");
                Nodes[2].AppendChild(Nodes[5]);

                Nodes[6] = doc.CreateElement("configHost");
                Nodes[2].AppendChild(Nodes[6]);

                Nodes[6] = doc.CreateElement("configPort");
                Nodes[2].AppendChild(Nodes[6]);

                Nodes[7] = doc.CreateElement("place");
                Nodes[2].AppendChild(Nodes[7]);

                Nodes[1].AppendChild(Nodes[2]);

                Nodes[8] = doc.CreateElement("Login");

                Nodes[1].AppendChild(Nodes[8]);

                doc.Save(Environment.CurrentDirectory + @"\config.xml");
            }
            catch (Exception GeneralException)
            {
                logger.Error("XMLStorage : CreateConfig : ExceptionCaught : " + GeneralException.Message);
            }
            finally
            {
                GC.Collect();
                logger.Debug("XMLStorage : CreateConfig : Method Exit");
            }
        }

        //private void CreateConfig()
        //{
        //    XmlDocument doc;
        //    XmlNode[] Nodes = new XmlNode[13];
        //    XmlTextWriter writeUserDetails;
        //    string rootElement = "config";
        //    string loginParamsElement = "StatTickerFive";
        //    try
        //    {
        //        logger.Debug("XMLStorage : CreateConfig : Method Entry");
        //        doc = new XmlDocument();

        //        Nodes[0] = doc.CreateElement("config");
        //        doc.AppendChild(Nodes[0]);

        //        Nodes[1] = doc.CreateElement("StatTickerFive");
        //        Nodes[0].AppendChild(Nodes[1]);

        //        Nodes[2] = doc.CreateElement("Initialize");

        //        Nodes[3] = doc.CreateElement("username");
        //        Nodes[2].AppendChild(Nodes[3]);

        //        Nodes[4] = doc.CreateElement("password");
        //        Nodes[2].AppendChild(Nodes[4]);

        //        Nodes[5] = doc.CreateElement("applicationName");
        //        Nodes[2].AppendChild(Nodes[5]);

        //        Nodes[6] = doc.CreateElement("configHost");
        //        Nodes[2].AppendChild(Nodes[6]);

        //        Nodes[6] = doc.CreateElement("configPort");
        //        Nodes[2].AppendChild(Nodes[6]);

        //        Nodes[7] = doc.CreateElement("place");
        //        Nodes[2].AppendChild(Nodes[7]);

        //        Nodes[8] = doc.CreateElement("db-type");
        //        Nodes[2].AppendChild(Nodes[3]);

        //        Nodes[9] = doc.CreateElement("db-host");
        //        Nodes[2].AppendChild(Nodes[4]);

        //        Nodes[10] = doc.CreateElement("db-port");
        //        Nodes[2].AppendChild(Nodes[5]);

        //        Nodes[11] = doc.CreateElement("db-name");
        //        Nodes[2].AppendChild(Nodes[6]);

        //        Nodes[1].AppendChild(Nodes[2]);

        //        Nodes[12] = doc.CreateElement("database-connection");

        //        Nodes[1].AppendChild(Nodes[12]);

        //        doc.Save(Environment.CurrentDirectory + @"\config.xml");

        //        writeUserDetails =
        //            new XmlTextWriter(
        //                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
        //                "\\Pointel\\StatTickerFive\\config.xml", Encoding.Default);

        //        writeUserDetails.WriteStartElement(rootElement);
        //        writeUserDetails.WriteStartElement(loginParamsElement);
        //        //Genesys Parameters Writing

        //        writeUserDetails.WriteStartElement("Initialize");
        //        writeUserDetails.WriteStartElement("login");


        //        writeUserDetails.WriteElementString("username", "");
        //        writeUserDetails.WriteElementString("password", "");
        //        writeUserDetails.WriteElementString("applicationName", "");
        //        writeUserDetails.WriteElementString("configHost", "");
        //        writeUserDetails.WriteElementString("configPort", "");
        //        writeUserDetails.WriteElementString("place", "");
        //        writeUserDetails.WriteEndElement();
        //        writeUserDetails.WriteStartElement("databse-connection");
        //        writeUserDetails.WriteElementString("db-type", "");
        //        writeUserDetails.WriteElementString("db-host", "");
        //        writeUserDetails.WriteElementString("db-port", "");
        //        writeUserDetails.WriteElementString("db-name", "");


        //        writeUserDetails.WriteEndElement();
        //        writeUserDetails.WriteEndElement();

        //        writeUserDetails.Close();
        //        writeUserDetails = null;
        //    }
        //    catch (Exception GeneralException)
        //    {
        //        logger.Error("XMLStorage : CreateConfig : ExceptionCaught : " + GeneralException.Message);
        //    }
        //    finally
        //    {
        //        GC.Collect();
        //        logger.Debug("XMLStorage : CreateConfig : Method Exit");
        //    }
        //}
        # endregion

        #region Encoding

        /// <summary>
        /// Encodes the to64 UT f8.
        /// </summary>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        public string EncodeTo64UTF8(string password)
        {
            string returnValue=string.Empty;
            try
            {
                logger.Debug("XMLStorage : EncodeTo64UTF8 Method() - Entry");
                byte[] toEncodeAsBytes = Encoding.UTF8.GetBytes(password);
                returnValue = Convert.ToBase64String(toEncodeAsBytes);
              
            }
            catch(Exception generalException)
            {
                logger.Error("XMLStorage : EncodeTo64UTF8 Method() - Exception caught : "+ generalException.Message.ToString());
            }
            logger.Debug("XMLStorage : EncodeTo64UTF8 Method() - Exit");
            return returnValue;
        }

        #endregion

        #region Decoding

        /// <summary>
        /// Decodes the from64.
        /// </summary>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        public string DecodeFrom64(string password)
        {
            byte[] encodedDataAsBytes = Convert.FromBase64String(password);
            string returnValue = Encoding.UTF8.GetString(encodedDataAsBytes);
            return returnValue;
        }

        #endregion
    }
}
