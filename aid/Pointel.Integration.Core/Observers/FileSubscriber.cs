namespace Pointel.Integration.Core.Observers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;

    using Genesyslab.Platform.Commons.Collections;
    using Genesyslab.Platform.Voice.Protocols.TServer.Events;

    using Pointel.Configuration.Manager;
    using Pointel.Integration.Core.Data;
    using Pointel.Integration.Core.Helper;
    using Pointel.Integration.Core.iSubjects;
    using Pointel.Integration.Core.Providers;
    using Pointel.Integration.Core.Util;

    /// <summary>
    /// create class file subscriber
    /// </summary>
    internal class FileSubscriber : IObserver<iCallData>
    {
        #region Fields

        private IDisposable cancellation;
        private Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
            "AID");
        private FileIntegrationData objConfiguration;
        private Settings setting = Settings.GetInstance();

        #endregion Fields

        #region Constructors

        public FileSubscriber(FileIntegrationData objConfiguration)
        {
            this.objConfiguration = objConfiguration;
        }

        #endregion Constructors

        #region Methods

        public void OnCompleted()
        {
            Unsubscribe();
        }

        public void OnError(Exception error)
        {
            //throw new NotImplementedException();
        }

        public void OnNext(iCallData value)
        {
            try
            {
                if (value == null || value.EventMessage == null)
                {
                    logger.Warn("Event detail is null.");
                    return;
                }

                if (objConfiguration.MediaType != value.MediaType)
                    return;

                if (value.EventMessage.Name.ToLower() != objConfiguration.FileEvent)
                    return;

                KeyValueCollection userData = null;
                Type objType = null;
                object obj = null;

                MediaEventHelper objEventHelper = new MediaEventHelper();
                switch (value.MediaType)
                {
                    case MediaType.Voice:
                        if (!objEventHelper.ConvertVoiceEvent(ref objType, ref obj, ref userData, value.EventMessage))
                            logger.Warn("Voice event conversion getting failed");
                        break;
                    case MediaType.Email:
                        if (!objEventHelper.ConvertEmailEvent(ref objType, ref obj, ref userData, value.EventMessage))
                            logger.Warn("Voice event conversion getting failed");
                        break;
                    case MediaType.Chat:
                        if (!objEventHelper.ConvertChatEvent(ref objType, ref obj, ref userData, value.EventMessage))
                            logger.Warn("Voice event conversion getting failed");
                        break;
                    case MediaType.SMS:
                        if (!objEventHelper.ConvertVoiceEvent(ref objType, ref obj, ref userData, value.EventMessage))
                            logger.Warn("Voice event conversion getting failed");
                        break;
                    default:
                        logger.Warn("Unsupported media type");
                        break;
                }
                objEventHelper = null;

                // Functionality to send data in the specified format.
                if (objType != null && obj != null)
                {

                    switch (objConfiguration.FileFormat)
                    {
                        case "text":
                            ProcessTextFile(objType, obj, userData);
                            break;
                        case "json":
                            //SendJsonData(objType, obj, userdata);
                            break;
                        case "xml":
                            //SendXMLData(objType, obj, userdata);
                            break;
                        case "custom":
                            //SendTextData(objType, obj, userdata);
                            break;
                        default:
                            logger.Warn("The specified format not supported in the pipe integration");
                            break;
                    }

                }
                else
                    logger.Warn("Required data is null.");

                //if (ConfigContainer.Instance().AllKeys.Contains("enable.eclipse-integration") && ConfigContainer.Instance().GetAsBoolean("enable.eclipse-integration"))
                //{
                //    string sectionName = string.Empty;
                //    if (ConfigContainer.Instance().AllKeys.Contains("file.data-section"))
                //        sectionName = ConfigContainer.Instance().GetAsString("file.data-section");
                //    if (!string.IsNullOrEmpty(sectionName))
                //        PopupEclipseIntegration(objType, obj, userData, sectionName);
                //    else
                //        logger.Warn("Data section not available to file subscriber");
                //    return;
                //}
                //if (value.FileData.FileFormat == "text")
                //    WriteTextFile(objType, obj, userData, value);
                //if (value.FileData.FileFormat == "xml")
                //    WriteXMLFile(objType, obj, userData, value);
                //else
                //    PopupInformation(objType, obj, userData, value);

                DesktopMessenger.communicateUI.NotifyFileData(userData, setting.attachDataList, value.FileData.EnableView);
            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred while writing call data to a file " + generalException.ToString());
            }
        }

        public virtual void Subscribe(CallDataProviders provider)
        {
            if (provider != null)
                cancellation = provider.Subscribe(this);
        }

        public virtual void Unsubscribe()
        {
            //  cancellation.Dispose();
        }

        //private void PopupEclipseIntegration(Type objType, object obj, KeyValueCollection objUserData, string dataSection)
        //{
        //    try
        //    {
        //        if (objUserData == null)
        //            logger.Warn("User data is null in eclipse integration");
        //        //KeyValueCollection filterKeys = null;
        //        //if (!ConfigContainer.Instance().AllKeys.Contains(dataSection))
        //        //    ConfigContainer.Instance().ReadSection(dataSection);
        //        //if (!ConfigContainer.Instance().AllKeys.Contains(dataSection))
        //        //    filterKeys = ConfigContainer.Instance().GetValue(dataSection) as KeyValueCollection;
        //        string data = string.Empty;
        //        //if (filterKeys != null)
        //        //{
        //        //    foreach (string key in filterKeys.AllKeys)
        //        //    {
        //        //        if (objUserData.ContainsKey(filterKeys[key].ToString()))
        //        //            data = data + ((string.IsNullOrEmpty(data)) ? string.Empty : "<") + key + ":" + objUserData[filterKeys[key].ToString()].ToString();
        //        //        else
        //        //            data = data + ((string.IsNullOrEmpty(data)) ? string.Empty : "<") + key + ":N/A";
        //        //    }
        //        //}
        //        //else
        //        //{
        //        //    logger.Warn("Filter keys not available in eclipse integration.");
        //        //    return;
        //        //}
        //        DataParser objDataParser = new DataParser();
        //        string delimiter = "<";
        //        if (ConfigContainer.Instance().AllKeys.Contains("file.string-delimiter"))
        //            delimiter = ConfigContainer.Instance().GetAsString("file.string-delimiter");
        //        data = objDataParser.ParseTextEclipse(objType, obj, objUserData, dataSection, ":", "N/A");
        //        if (ConfigContainer.Instance().AllKeys.Contains("file-name"))
        //        {
        //            string filePath = ConfigContainer.Instance().GetAsString("file-name");
        //            //e:/ directory
        //            if (ConfigContainer.Instance().AllKeys.Contains("directory"))
        //                filePath = Path.Combine(ConfigContainer.Instance().GetAsString("directory"), filePath);
        //            if (!filePath.EndsWith(".txt"))
        //                filePath += ".txt";
        //            File.WriteAllText(filePath, data);
        //        }
        //        else
        //        {
        //            logger.Warn("File path is null to eclipse integration.");
        //            return;
        //        }
        //    }
        //    catch (Exception _generalException)
        //    {
        //        logger.Error("Error occurred as " + _generalException.Message);
        //    }
        //}
        private void PopupInformation(Type objEventType, object obj, KeyValueCollection userData, iCallData value)
        {
            try
            {
                string xmlString = string.Empty;
                DataParser objDataParser = new DataParser();
                string dataSection = string.Empty;
                if (ConfigContainer.Instance().AllKeys.Contains("file.data-section"))
                    dataSection = ConfigContainer.Instance().GetAsString("file.data-section");
                xmlString = objDataParser.ParseXML(objEventType, obj, userData, dataSection);

                #region Old Code

                //switch (value.VoiceEvent.Id)
                //{
                //    case EventRinging.MessageId:
                //        EventRinging eventRinging = (EventRinging)value.VoiceEvent;
                //        xmlString = objDataParser.GetXML(eventRinging.GetType(), eventRinging, userData, section);
                //        break;
                //    case EventReleased.MessageId:
                //        EventReleased eventReleased = (EventReleased)value.VoiceEvent;
                //        xmlString = objDataParser.GetXML(eventReleased.GetType(), eventReleased, userData, section);
                //        break;

                //    case EventEstablished.MessageId:
                //        EventEstablished eventEstablished = (EventEstablished)value.VoiceEvent;
                //        xmlString = objDataParser.GetXML(eventEstablished.GetType(), eventEstablished, userData, section);
                //        break;

                //    case EventHeld.MessageId:
                //        EventHeld eventHeld = (EventHeld)value.VoiceEvent;
                //        xmlString = objDataParser.GetXML(eventHeld.GetType(), eventHeld, userData, section);
                //        break;

                //    case EventPartyChanged.MessageId:
                //        EventPartyChanged eventPartyChanged = (EventPartyChanged)value.VoiceEvent;
                //        xmlString = objDataParser.GetXML(eventPartyChanged.GetType(), eventPartyChanged, userData, section);
                //        break;

                //    case EventAttachedDataChanged.MessageId:
                //        EventAttachedDataChanged eventAttachedDataChanged = (EventAttachedDataChanged)value.VoiceEvent;
                //        xmlString = objDataParser.GetXML(eventAttachedDataChanged.GetType(), eventAttachedDataChanged, userData, section);
                //        break;

                //    case EventDialing.MessageId:
                //        EventDialing eventDialing = (EventDialing)value.VoiceEvent;
                //        xmlString = objDataParser.GetXML(eventDialing.GetType(), eventDialing, userData, section);
                //        break;

                //    case EventRetrieved.MessageId:
                //        EventRetrieved eventRetrieved = (EventRetrieved)value.VoiceEvent;
                //        xmlString = objDataParser.GetXML(eventRetrieved.GetType(), eventRetrieved, userData, section);
                //        break;

                //    case EventAbandoned.MessageId:
                //        EventAbandoned eventAbandoned = (EventAbandoned)value.VoiceEvent;
                //        xmlString = objDataParser.GetXML(eventAbandoned.GetType(), eventAbandoned, userData, section);
                //        break;

                //    case EventPartyAdded.MessageId:
                //        EventAbandoned eventPartyAdded = (EventAbandoned)value.VoiceEvent;
                //        xmlString = objDataParser.GetXML(eventPartyAdded.GetType(), eventPartyAdded, userData, section);
                //        break;

                //    case EventPartyDeleted.MessageId:
                //        EventPartyDeleted eventPartyDeleted = (EventPartyDeleted)value.VoiceEvent;
                //        xmlString = objDataParser.GetXML(eventPartyDeleted.GetType(), eventPartyDeleted, userData, section);
                //        break;
                //}

                #endregion

                string memberID = string.Empty;
                string memberSX = string.Empty;
                string memberPhone = string.Empty;
                string memberExtension = string.Empty;
                string message = string.Empty;
                string memberFirstName = string.Empty;
                string memberLastName = string.Empty;
                string memberDOB = string.Empty;
                string memberTopValidated = string.Empty;
                string cstk_summary = string.Empty;
                string cstk_mctr_category = string.Empty;
                string cssc_mctr_call = string.Empty;
                string callertype = string.Empty;
                string providerID = string.Empty;
                string groupID = string.Empty;
                string cstk_mem_summary = string.Empty;
                string[] tempValues;
                string memValidated = string.Empty;
                string callingFor = string.Empty;

                #region Old Code

                KeyValueCollection data = userData;
                if (data != null)
                {
                    if (!data.Equals(string.Empty))
                    {
                        foreach (string keys in data.Keys)
                        {
                            if (setting.attachDataList.ContainsValue(keys))
                            {
                                string tag = setting.attachDataList.Where(kvp => kvp.Value == keys).Select(kvp => kvp.Key).FirstOrDefault();
                                if ((tag == "SBSB_ID" || tag == "MEME_SFX") && tag != null)
                                {
                                    try
                                    {
                                        tempValues = null;
                                        if (!string.IsNullOrEmpty(Convert.ToString(data[keys])))
                                        {
                                            tempValues = data[keys].ToString().Split('|');
                                            memberID = tempValues[0].ToString().Substring(0, 9);
                                            memberSX = data[keys].ToString().Substring(9, 2);
                                            logger.Info("Search MemIDTranslated : " + memberID);
                                        }
                                    }
                                    catch (Exception memberException)
                                    {
                                        logger.Error("Error occurred while splitting member ID " + memberException.ToString());
                                    }
                                }
                                if ((tag == "CSCI_PHONE") && tag != null)
                                {
                                    memberPhone = data[keys].ToString();
                                    logger.Info("memberPhone : " + memberPhone);
                                }
                                else if (tag == "CSCI_FIRST_NAME" || tag == "CSCI_MID_INIT" || tag == "CSCI_LAST_NAME")
                                {
                                    try
                                    {
                                        tempValues = null;
                                        tempValues = data[keys].ToString().Split('|');
                                        string[] name = tempValues[0].ToString().Split(' ');
                                        memberFirstName = name[0];
                                        memberLastName = name[1];
                                        logger.Info("Search FristName : " + memberFirstName);
                                        logger.Info("Search LastName : " + memberLastName);
                                    }
                                    catch (Exception splitException)
                                    {
                                        logger.Error("Error occurred while spliting the string value " + splitException.ToString());
                                    }
                                }
                                else if (tag == "MEMBER_DOB")
                                {
                                    tempValues = null;
                                    tempValues = data[keys].ToString().Split('|');
                                    memberDOB = tempValues[0].ToString();
                                    logger.Info("Search memdob : " + memberDOB);
                                }
                                //else if (tag == "CSSC_MCTR_CALL")
                                //{
                                //    callertype = data[keys].ToString();
                                //    logger.Info("Search CallerType : " + callertype);
                                //}
                                else if (tag == "PRPR_ID")
                                {
                                    providerID = data[keys].ToString();
                                    logger.Info("Search CallerIDTranslated : " + providerID);
                                }
                                else if (tag == "GRGR_ID")
                                {
                                    tempValues = null;
                                    tempValues = data[keys].ToString().Split('|');
                                    groupID = tempValues[0].ToString();
                                    logger.Info("Search groupid : " + groupID);
                                }
                                else if (string.Compare(keys, "memdos", true) == 0)
                                {
                                    cstk_mem_summary = data[keys].ToString();
                                    logger.Info("Search MemDOS : " + cstk_mem_summary);
                                }
                                else if (tag == "CSTK_SUMMARY")
                                {
                                    cstk_summary = data[keys].ToString();
                                    logger.Info("Search BenDisc : " + cstk_summary);
                                }
                                else if (tag == "CSTK_CUST_IND")
                                {
                                    callertype = data[keys].ToString();
                                    logger.Info("Search CallerType : " + callertype);
                                }
                                else if (tag == "CSSC_MCTR_CALL")
                                {
                                    tempValues = null;
                                    tempValues = data[keys].ToString().Split('|');
                                    cssc_mctr_call = tempValues[0].ToString();
                                    logger.Info("MCTR call type : " + cssc_mctr_call);
                                }
                                else if (tag == "CSTK_MCTR_CATG")
                                {
                                    tempValues = null;
                                    tempValues = data[keys].ToString().Split('|');
                                    cstk_mctr_category = tempValues[0].ToString();
                                    logger.Info("Search CallReason : " + cstk_mctr_category);
                                }
                                else if (tag == "MEMBER_INFO")
                                {
                                    tempValues = null;
                                    memValidated = data[keys].ToString();
                                    logger.Info("Search MemValidated : " + memValidated);
                                }
                                else if (tag == "CALLING_FOR")
                                {
                                    callingFor = data[keys].ToString();
                                    logger.Info("Search CALLING_FOR : " + callingFor);
                                }
                            }
                        }
                    }
                }
                else
                    logger.Warn("Attached Data is empty");

                #endregion

                string originalCallerType = callertype;
                callertype = callertype.ToLower() == "p" ? "M" : callertype;
                //string CSSC_MCTR_CALLValue = string.IsNullOrEmpty(callertype) == false ? (callertype.ToLower() == "m" ? "1300" : "1800") : string.Empty;

                if (Settings.GetInstance().EnableFacetCommunication)
                    message = "<ListenerMessage><Identification> <Destination>FID</Destination> <Origin>Company Server</Origin></Identification><Execution> <PZAP_APP_ID>CST0</PZAP_APP_ID> " +
                    "<Action>New</Action></Execution><Data><Navigation> <SectionName></SectionName> <ActiveRow></ActiveRow> <SubSectionName></SubSectionName> <AppAction></AppAction> " +
                    "</Navigation><REC_CUST>" + "<CSTK_CUST_IND>" + callertype + "</CSTK_CUST_IND><CSTK_MCTR_CATG>" + cstk_mctr_category + "</CSTK_MCTR_CATG><CSTK_SUMMARY>" + cstk_summary + "</CSTK_SUMMARY>" +
                    "<SBSB_ID>" + memberID + "</SBSB_ID><MEME_SFX>" + memberSX + "</MEME_SFX><PRPR_ID>" + providerID + "</PRPR_ID><GRGR_ID>" + groupID + "</GRGR_ID><SGSG_ID></SGSG_ID> " +
                    "<CSSC_MCTR_CALL>" + cssc_mctr_call + "</CSSC_MCTR_CALL><CSCI_FIRST_NAME>" + (originalCallerType.ToLower() == "p" ? string.Empty : memberFirstName) + "</CSCI_FIRST_NAME><CSCI_LAST_NAME>" + (originalCallerType.ToLower() == "p" ? string.Empty : memberLastName) + "</CSCI_LAST_NAME>" +
                    "<CSCI_MID_INIT></CSCI_MID_INIT><CSCI_TITLE></CSCI_TITLE><CSCI_PHONE>" + memberPhone + "</CSCI_PHONE><CSCI_PHONE_EXT>" + memberExtension + "</CSCI_PHONE_EXT><CSCI_SSN></CSCI_SSN>" +
                    "<CSSC_CALLIN_METHOD>1</CSSC_CALLIN_METHOD></REC_CUST><REC_IVR_DATA><MCPD_SOURCE>CS01</MCPD_SOURCE><CALL_RECEIVED_ON>Customer Care</CALL_RECEIVED_ON><MEMBER_ID>" + memberID + "</MEMBER_ID>" +
                    "<CALLING_FOR>" + callingFor + "</CALLING_FOR><MEMBER_DOB> " + memberDOB + "</MEMBER_DOB><MEMBER_INFO>" + memValidated + "</MEMBER_INFO><PAGE_TYPE>Medical Claims</PAGE_TYPE></REC_IVR_DATA></Data></ListenerMessage>";
                else
                    message = "<ListenerMessage><Identification> <Destination>FID</Destination> <Origin>Company Server</Origin></Identification><Execution> <PZAP_APP_ID>CST0</PZAP_APP_ID> " +
                    "<Action>New</Action></Execution><Data><Navigation> <SectionName></SectionName> <ActiveRow></ActiveRow> <SubSectionName></SubSectionName> <AppAction></AppAction> " +
                    "</Navigation><REC_CUST>" + "<CSTK_CUST_IND>" + callertype + "</CSTK_CUST_IND><CSTK_MCTR_CATG>" + cstk_mctr_category + "</CSTK_MCTR_CATG><CSTK_SUMMARY>" + cstk_summary + "</CSTK_SUMMARY>" +
                    "<SBSB_ID>" + memberID + "</SBSB_ID><MEME_SFX>" + memberSX + "</MEME_SFX><PRPR_ID></PRPR_ID><GRGR_ID></GRGR_ID><SGSG_ID></SGSG_ID> " +
                    "<CSSC_MCTR_CALL>" + cssc_mctr_call + "</CSSC_MCTR_CALL><CSCI_FIRST_NAME>" + (originalCallerType.ToLower() == "p" ? string.Empty : memberFirstName) + "</CSCI_FIRST_NAME><CSCI_LAST_NAME>" + (originalCallerType.ToLower() == "p" ? string.Empty : memberLastName) + "</CSCI_LAST_NAME>" +
                    "<CSCI_MID_INIT></CSCI_MID_INIT><CSCI_TITLE></CSCI_TITLE><CSCI_PHONE>" + memberPhone + "</CSCI_PHONE><CSCI_PHONE_EXT>" + memberExtension + "</CSCI_PHONE_EXT><CSCI_SSN></CSCI_SSN>" +
                    "<CSSC_CALLIN_METHOD>1</CSSC_CALLIN_METHOD></REC_CUST><REC_IVR_DATA><MCPD_SOURCE>CS01</MCPD_SOURCE><CALL_RECEIVED_ON>Customer Care</CALL_RECEIVED_ON><MEMBER_ID>" + memberID + "</MEMBER_ID>" +
                    "<CALLING_FOR>" + callingFor + "</CALLING_FOR><MEMBER_DOB> " + memberDOB + "</MEMBER_DOB><MEMBER_INFO>" + memValidated + "</MEMBER_INFO><PAGE_TYPE>Medical Claims</PAGE_TYPE></REC_IVR_DATA></Data></ListenerMessage>";

                //Code compare default file and BCBS
                //message +="File from Default file = "+ result;

                //code Added on 25-02-2014 to implement storing of call data in specified location
                if (/*Settings.InteractionDataLocation*/value.FileData.DirectoryPath != string.Empty)
                {
                    try
                    {
                        string folder = Path.Combine(/*Settings.InteractionDataLocation*/value.FileData.DirectoryPath, "");
                        if (!Directory.Exists(folder))
                            Directory.CreateDirectory(folder);
                        string file = "";
                        if (value.FileData.FileName != string.Empty)
                        {
                            file = Path.Combine(folder, value.FileData.FileName + ".txt");
                        }
                        else
                        {
                            logger.Warn("File Name is empty");
                            file = Path.Combine(folder, "calldata_vd.txt");
                        }
                        File.WriteAllText(file, message);
                    }
                    catch (Exception fileException)
                    {
                        logger.Error("Error occurred while writing data in Text file" + fileException.ToString());
                    }
                }
                else
                {
                    logger.Warn("File Directory Path is empty");
                    try
                    {
                        // string folder = Path.Combine(new DirectoryInfo(Environment.SystemDirectory.ToString()).Root.Name.ToString(), Environment.SpecialFolder.ProgramFiles.ToString(), "Pointel");
                        string folder = "C:\\Program Files (x86)\\Pointel";
                        if (!Directory.Exists(folder))
                            Directory.CreateDirectory(folder);
                        string file = "";
                        if (value.FileData.FileName != string.Empty)
                        {
                            file = Path.Combine(folder, value.FileData.FileName + ".txt");
                        }
                        else
                        {
                            logger.Warn("File Name is empty");
                            file = Path.Combine(folder, "calldata_vd.txt");
                        }
                        File.WriteAllText(file, message);
                    }
                    catch (Exception fileException)
                    {
                        logger.Error("Error occurred while writing data in Text file" + fileException.ToString());
                    }
                }
                //End
                logger.Info("XML Message " + message);
            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred while constructing XML message for FACET " + generalException.ToString());
            }
        }

        private void ProcessTextFile(Type objEventType, object obj, KeyValueCollection userData)
        {
            try
            {
                DataParser objDataParser = new DataParser();
                string data = objDataParser.ParseTextString(objEventType, obj, userData, objConfiguration.DataToPost, objConfiguration.Delimiter, objConfiguration.ValueSeperator, "N/A", objConfiguration.KeyValueLeftSymbol, objConfiguration.KeyValueRightSymbol);
                SaveIntoFile(data);
            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred as " + generalException.Message);
            }
        }

        private void SaveIntoFile(string content)
        {
            try
            {
                if (content != null)
                {
                    if (!string.IsNullOrEmpty(objConfiguration.Directory))
                    {
                        if (!Directory.Exists(objConfiguration.Directory))
                            try
                            {
                                Directory.CreateDirectory(objConfiguration.Directory);
                            }
                            catch (DirectoryNotFoundException)
                            {
                                logger.Error("Part of the directory path not found. Directory path:'" + objConfiguration.Directory + "'");
                                return;
                            }
                            catch (Exception generalException)
                            {
                                logger.Error("Error occurred as " + generalException.Message);
                                return;
                            }
                    }
                    else
                        logger.Warn("The folder name is empty");

                    if (!string.IsNullOrEmpty(objConfiguration.FileName))
                    {
                        Regex notAllowedPattern = new Regex("[\"*/:<>\\|]|[\n]{2}");
                        string tempFileName = objConfiguration.FileName;
                        if (objConfiguration.FileName.Contains("<"))
                        {
                            var matchCollection = Regex.Matches(objConfiguration.FileName, @"\<([^>]*)\>");
                            foreach (Match item in matchCollection)
                            {
                                switch (item.Groups[1].Value.ToLower())
                                {
                                    case "agent.firstname":
                                        tempFileName = tempFileName.Replace(item.Groups[0].Value, setting.FirstName);
                                        break;
                                    case "agent.lastname":
                                        tempFileName = tempFileName.Replace(item.Groups[0].Value, setting.LastName);
                                        break;
                                    case "agent.username":
                                        tempFileName = tempFileName.Replace(item.Groups[0].Value, setting.UserName);
                                        break;
                                    case "agent.empid":
                                        tempFileName = tempFileName.Replace(item.Groups[0].Value, setting.EmployeeID);
                                        break;
                                    case "environmental.username":
                                        tempFileName = tempFileName.Replace(item.Groups[0].Value, Environment.UserName);
                                        break;
                                    case "environmental.machinename":
                                        tempFileName = tempFileName.Replace(item.Groups[0].Value, Environment.MachineName);
                                        break;
                                    default:
                                        tempFileName = tempFileName.Replace(item.Groups[0].Value, "");
                                        break;
                                }
                            }
                        }
                        tempFileName = notAllowedPattern.Replace(tempFileName, "");
                        string fileWithPath = string.Empty;
                        if (!string.IsNullOrEmpty(objConfiguration.Directory))
                            fileWithPath = Path.Combine(objConfiguration.Directory, tempFileName);
                        else
                            fileWithPath = Path.Combine(Environment.CurrentDirectory, tempFileName);

                        File.WriteAllText(fileWithPath, content);
                    }
                    else
                        logger.Warn("File name is null or empty.");
                }
                else
                    logger.Warn("The file content to write in the file is null.");
            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred as " + generalException.Message);
            }
        }

        /// <summary>
        /// Writes the text file.
        /// </summary>
        /// <param name="userData">The user data.</param>
        /// <param name="value">The value.</param>
        /// <param name="parameter">The parameter.</param>void WriteTextFile(KeyValueCollection userData, iCallData value, [Optional] Dictionary<string, string> parameter)
        void WriteTextFile(Type objEventType, object obj, KeyValueCollection userData, iCallData value)
        {
            try
            {
                //DataParser objDataParser = new DataParser();

                //string delimiter = "|";
                //string dataSection = string.Empty;
                //if (ConfigContainer.Instance().AllKeys.Contains("file.string-delimiter"))
                //    delimiter = ConfigContainer.Instance().GetAsString("file.string-delimiter");
                //if (ConfigContainer.Instance().AllKeys.Contains("file.data-section"))
                //    dataSection = ConfigContainer.Instance().GetAsString("file.data-section");

                //logger.Debug("Try to parse the data as text");
                //string result = objDataParser.ParseTextString(objEventType, obj, userData, delimiter, dataSection);
                //logger.Info("Parsed text: " + result);
                //string folder = string.Empty;
                //if (ConfigContainer.Instance().AllKeys.Contains("directory"))
                //    folder = ConfigContainer.Instance().GetAsString("directory");
                ////Path.Combine(value.FileData.DirectoryPath, "");
                //if (!string.IsNullOrEmpty(folder))
                //{
                //    if (!Directory.Exists(folder))
                //        Directory.CreateDirectory(folder);
                //}
                //else
                //    logger.Warn("The folder name is empty");

                //string file = string.Empty;
                //if (ConfigContainer.Instance().AllKeys.Contains("file-name"))
                //    file = ConfigContainer.Instance().GetAsString("file-name");
                //if (file != string.Empty)
                //    file = Path.Combine(folder, file + ".txt");
                //else
                //{
                //    logger.Warn("Filename is empty");
                //    file = Path.Combine(folder, "calldata_vd");
                //}
                //File.WriteAllText(file, result);
            }
            catch (Exception _generalException)
            {
                logger.Error("Error occurred as " + _generalException.Message);
            }

            //try
            //{
            //    result = string.Empty;

            //    foreach (KeyValuePair<string, string> keys in parameter)
            //    {
            //        if (userData != null)
            //        {
            //            if (userData.ContainsKey(keys.Value))
            //            {
            //                result += keys.Key + "=" + Convert.ToString(userData[keys.Value]) + value.FileData.Delimiter;
            //            }
            //        }
            //    }
            //    AttributeFilter(value, userData);
            //    if (value.FileData.DirectoryPath != string.Empty)
            //    {
            //        try
            //        {
            //            string folder = Path.Combine(/*Settings.InteractionDataLocation*/value.FileData.DirectoryPath, "");
            //            if (!Directory.Exists(folder))
            //                Directory.CreateDirectory(folder);
            //            string file = "";
            //            if (value.FileData.FileName != string.Empty)
            //            {
            //                file = Path.Combine(folder, value.FileData.FileName + ".txt");
            //            }
            //            else
            //            {
            //                logger.Warn("Filename is empty");
            //                file = Path.Combine(folder, "calldata_vd.xml");
            //            }
            //            File.WriteAllText(file, result.Substring(0, result.Length - 1));
            //        }
            //        catch (Exception fileException)
            //        {
            //            logger.Error("Error occurred while writing data in Text file" + fileException.ToString());
            //        }
            //    }
            //    else
            //    {
            //        logger.Warn("Folder path is empty");
            //        try
            //        {
            //            string folder = Path.Combine(Environment.CurrentDirectory.ToString(), "");
            //            if (!Directory.Exists(folder))
            //                Directory.CreateDirectory(folder);
            //            string file = "";
            //            if (value.FileData.FileName != string.Empty)
            //            {
            //                file = Path.Combine(folder, value.FileData.FileName + ".txt");
            //            }
            //            else
            //            {
            //                logger.Warn("Filename is empty");
            //                file = Path.Combine(folder, "calldata_vd.txt");
            //            }
            //            File.WriteAllText(file, result.Substring(0, result.Length - 1));
            //        }
            //        catch (Exception fileException)
            //        {
            //            logger.Error("Error occurred while writing data in Text file" + fileException.ToString());
            //        }
            //    }

            //    DesktopMessenger.communicatUI.NotifyFileData(userData, setting.attachDataList, value.FileData.EnableView);
            //}
            //catch (Exception exception)
            //{
            //    logger.Error("WriteTextFile" + exception.ToString());
            //}
        }

        private void WriteXMLFile(Type objEventType, object obj, KeyValueCollection userData, iCallData value)
        {
            try
            {
                DataParser objDataParser = new DataParser();
                string dataSection = string.Empty;
                if (ConfigContainer.Instance().AllKeys.Contains("file.data-section"))
                    dataSection = ConfigContainer.Instance().GetAsString("file.data-section");
                logger.Debug("Try to parse the data from the data section '" + dataSection + "'");
                string result = objDataParser.ParseXML(objEventType, obj, userData, dataSection);
                logger.Info("Parsed data: " + result);

                #region File Writing functionality.
                string folder = string.Empty;
                if (ConfigContainer.Instance().AllKeys.Contains("directory"))
                    folder = ConfigContainer.Instance().GetAsString("directory");
                //Path.Combine(value.FileData.DirectoryPath, "");
                if (!string.IsNullOrEmpty(folder))
                {
                    if (!Directory.Exists(folder))
                        Directory.CreateDirectory(folder);
                }
                else
                    folder = Path.Combine(Environment.CurrentDirectory.ToString(), "");

                string file = string.Empty;
                if (ConfigContainer.Instance().AllKeys.Contains("file-name"))
                    file = ConfigContainer.Instance().GetAsString("file-name");
                if (file != string.Empty)
                    file = Path.Combine(folder, file + ".txt");
                else
                {
                    logger.Warn("Filename is empty");
                    file = Path.Combine(folder, "calldata_vd.xml");
                }
                File.WriteAllText(file, result);
                #endregion

            }
            catch (Exception _generalException)
            {
                logger.Error("Error occurred as " + _generalException.Message);
            }

            //result = string.Empty;

            //char[] delimiters = new char[] { '|' };
            //try
            //{
            //    foreach (KeyValuePair<string, string> keys in parameter)
            //    {
            //        if (userData != null)
            //        {
            //            if (userData.ContainsKey(keys.Value))
            //            {
            //                result += keys.Key + "=" + Convert.ToString(userData[keys.Value]) + value.FileData.Delimiter;
            //            }
            //        }
            //    }

            //    AttributeFilter(value, userData);
            //    string[] parts = result.Split(delimiters,
            //                        StringSplitOptions.RemoveEmptyEntries);
            //    Dictionary<string, string> dicLoadArrayValues = new Dictionary<string, string>();
            //    foreach (string loopValues in parts)
            //    {
            //        if (loopValues != "")
            //        {
            //            char[] delimiter = new char[] { '=' };

            //            string[] part = loopValues.Split(delimiter,
            //                             StringSplitOptions.RemoveEmptyEntries);
            //            if (part[0] != "" && part[1] != "")
            //                if (!dicLoadArrayValues.ContainsKey(part[0].ToString()))
            //                    dicLoadArrayValues.Add(part[0].ToString(), part[1].ToString());
            //        }
            //    }
            //    XML_Storage.XMLFile createXMLFile = new XML_Storage.XMLFile();
            //    createXMLFile.SaveInitializeParameters(dicLoadArrayValues, value);
            //    DesktopMessenger.communicatUI.NotifyFileData(userData, setting.attachDataList, value.FileData.EnableView);
            //}
            //catch (Exception exception)
            //{
            //    logger.Error("WriteXMLFile:Error in while creating file for XML " + exception.ToString());
            //}
        }

        #endregion Methods

        #region Other

        /// <summary>
        /// Subscribes the specified provider.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <summary>
        /// Unsubscribe this instance.
        /// </summary>
        /// <summary>
        /// Notifies the observer that the provider has finished sending push-based notifications.
        /// </summary>
        /// <summary>
        /// Notifies the observer that the provider has experienced an error condition.
        /// </summary>
        /// <param name="error">An object that provides additional information about the error.</param>
        /// <summary>
        /// Provides the observer with new data.
        /// </summary>
        /// <param name="value">The current notification information.</param>
        /// <summary>
        /// Attributes the filter.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="userData">The user data.</param>
        /// <returns></returns>
        /// <summary>
        /// Popups the information.
        /// </summary>
        /// <param name="userData">The user data.</param>
        /// <param name="value">The value.</param>
        /// <param name="txtOrXml">The text original XML.</param>

        #endregion Other
    }
}