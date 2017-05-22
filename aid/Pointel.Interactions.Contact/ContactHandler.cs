namespace Pointel.Interactions.Contact
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using System.Linq;

    using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel;
    using Genesyslab.Platform.Commons.Collections;
    using Genesyslab.Platform.Commons.Protocols;
    using Genesyslab.Platform.Contacts.Protocols.ContactServer;
    using Genesyslab.Platform.Contacts.Protocols.ContactServer.Events;

    using Pointel.Configuration.Manager;
    using Pointel.Interactions.Contact.ApplicationReader;
    using Pointel.Interactions.Contact.Core;
    using Pointel.Interactions.Contact.Core.Common;
    using Pointel.Interactions.Contact.Core.Request;
    using Pointel.Interactions.Contact.Settings;
    using Pointel.Interactions.IPlugins;
    using Pointel.Interactions.Contact.Helpers;


    #region Delegates

    internal delegate void ContactServerNotificationHandler();

    internal delegate void ContactUpdateNotification(string contactId, List<Pointel.Interactions.Contact.Helpers.Attribute> contactRow);

    internal delegate void HistoryRefresh(string interactionId, bool isDelete);

    internal delegate void InteractionServerNotificationHandler();

    internal delegate void TServerNotificationHandler();

    internal delegate void WorkbinChangedEvent(string interactionId);

    internal delegate void EmailStateNotifier();

    #endregion Delegates

    public class ContactHandler : IContactPlugin
    {
        #region Fields

        internal static WorkbinChangedEvent WorkbinChangedEvent;

        private Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
         "AID");

        #endregion Fields

        #region Events

        internal static event ContactServerNotificationHandler ContactServerNotificationHandler;

        internal static event ContactUpdateNotification ContactUpdateNotificationHandler;

        internal static event HistoryRefresh HistoryRefreshHandler;

        internal static event InteractionServerNotificationHandler InteractionServerNotificationHandler;

        internal static event TServerNotificationHandler TServerNotificationHandler;

        internal static event EmailStateNotifier EmailStateNotificationEvent;

        #endregion Events

        #region Methods

        public static void NofityContactUpdate(string contactId, List<Pointel.Interactions.Contact.Helpers.Attribute> contactRow)
        {
            if (ContactHandler.ContactUpdateNotificationHandler != null)
                ContactHandler.ContactUpdateNotificationHandler.Invoke(contactId, contactRow);
        }

        public IMessage AddAttachmentDocument(string interactionId, string filePath, bool isDocID = false)
        {
            IMessage response = null;

            try
            {
                if (isDocID)
                {
                    OutputValues output = RequestAddAttachDocument.AddAttachDocumentbyDocID(interactionId, filePath);
                    response = output.IContactMessage;
                }
                else
                {
                    OutputValues output = RequestAddAttachDocument.AddAttachDocument(interactionId, filePath);
                    response = output.IContactMessage;
                }
            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred as : " + generalException.ToString());
            }
            return response;
        }

        public IMessage DeleteInteraction(string interactionId)
        {
            IMessage response = null;

            try
            {
                OutputValues output = Pointel.Interactions.Contact.Core.Request.RequestToDeleteInteraction.DeleteInteractionFromUCS(interactionId);
                response = output.IContactMessage;
            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred as : " + generalException.ToString());
            }
            return response;
        }

        public IMessage GetAllAttributes(string contactId, List<string> attributeNames)
        {
            try
            {
                OutputValues output = RequestGetAllAttributes.GetAttributeValues(contactId, attributeNames);
                if (output != null)
                    return output.IContactMessage;

            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred as : " + generalException.ToString());
            }
            return null;
        }

        public System.Windows.Controls.UserControl GetContactDirectoryUserControl(bool enableOkCancelButton, string interactionId, Func<string, string, string, bool> notifySelectedEmailId
            , string toAddress = null, string ccAddress = null, string bccAddress = null)
        {
            return new Controls.ContactDirectory(enableOkCancelButton, interactionId, notifySelectedEmailId, toAddress, ccAddress, bccAddress);
        }

        public System.Windows.Controls.UserControl GetContactHistoryUserControl(string OwnerIDorPersonDBID)
        {
            try
            {
                if (!String.IsNullOrEmpty(OwnerIDorPersonDBID))
                {
                    return new Controls.ContactHistory(OwnerIDorPersonDBID);
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred as " + ex.Message);
            }
            return null;
        }

        public Dictionary<ContactDetails, string> GetContactId(string tenantId, MediaTypes mediaType, KeyValueCollection userData)
        {
            Dictionary<ContactDetails, string> contactDetails = null;
            OutputValues output = Pointel.Interactions.Contact.Core.Request.RequestToIdentifyContact.IdentifyContact(mediaType.ToString().ToLower(), ConfigContainer.Instance().TenantDbId, userData);
            if (output.MessageCode == "200")
            {
                IMessage message = output.IContactMessage;
                if (message != null)
                {
                    contactDetails = new Dictionary<ContactDetails, string>();
                    switch (message.Id)
                    {
                        case EventIdentifyContact.MessageId:
                            EventIdentifyContact eventIdentifyContact = (EventIdentifyContact)message;
                            logger.Trace(eventIdentifyContact.ToString());
                            if (eventIdentifyContact.ContactId != null)
                                contactDetails.Add(ContactDetails.ContactId, eventIdentifyContact.ContactId);
                            else if (eventIdentifyContact.ContactIdList != null)
                            {
                                if (eventIdentifyContact.ContactIdList.Count > 0)
                                    contactDetails.Add(ContactDetails.ContactId, eventIdentifyContact.ContactIdList[0].ToString());
                            }
                            if (eventIdentifyContact.OtherFields != null)
                            {
                                foreach (var fields in eventIdentifyContact.OtherFields.Keys)
                                {
                                    ContactDetails tempenum;
                                    if (Enum.TryParse(fields.ToString(), true, out tempenum))
                                    {
                                        contactDetails.Add(tempenum, eventIdentifyContact.OtherFields[fields.ToString()].ToString());
                                    }
                                }
                            }
                            break;
                        case Genesyslab.Platform.Contacts.Protocols.ContactServer.Events.EventError.MessageId:
                            Genesyslab.Platform.Contacts.Protocols.ContactServer.Events.EventError eventError = (Genesyslab.Platform.Contacts.Protocols.ContactServer.Events.EventError)message;
                            logger.Trace(eventError.ToString());
                            break;
                    }
                    return contactDetails;
                }
            }
            return contactDetails;
        }

        public IMessage GetContactList(int tenantDbId, StringList attributeList, SearchCriteriaCollection SearchCriteria, int maxCount = 0)
        {
            IMessage response = null;
            try
            {
                OutputValues output = Pointel.Interactions.Contact.Core.Request.RequestContactLists.GetContactList(tenantDbId, SearchCriteria, attributeList, maxCount);
                if (output.IContactMessage != null)
                {
                    logger.Info("RequestContactListGet result:" + output.IContactMessage.ToString());
                    if ("200".Equals(output.MessageCode))
                        response = output.IContactMessage;
                }
                else
                {
                    logger.Error("Error occurred while RequestContactListGet : " + output.Message);
                }

            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred as : " + generalException.ToString());
            }
            return response;
        }

        public System.Windows.Controls.UserControl GetContactUserControl(string contactId, MediaTypes mediaType, bool isEnableControl = true)
        {
            try
            {
                return new Controls.ContactInformation(contactId, mediaType.ToString(), isEnableControl);
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred as " + ex.Message);
            }
            return null;
        }

        public System.Windows.Controls.UserControl GetContactUserControl(string contactId, string interactionId, MediaTypes mediaType, Func<string> notifyContactUpdated)
        {
            try
            {
                return new Controls.ContactInformation(contactId, interactionId, mediaType.ToString(), notifyContactUpdated);
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred as " + ex.Message);
            }
            return null;
        }

        public IMessage GetDocument(string documentId)
        {
            IMessage response = null;
            try
            {
                OutputValues output = Pointel.Interactions.Contact.Core.Request.RequestToGetDocument.GetAttachDocument(documentId);
                if (output.MessageCode == "200")
                    response = output.IContactMessage;
                else
                    response = null;
            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred as : " + generalException.ToString());
            }
            return response;
        }

        public EmailSignature GetEmailSignature(string responsePath, ref bool isHTML)
        {
            return Helpers.ResponseHelper.GetSignature(responsePath, ref isHTML);
        }

        public IMessage GetIndexProperties()
        {
            IMessage response = null;
            try
            {
                OutputValues output = Pointel.Interactions.Contact.Core.Request.GetIndexProperties.GetResult();
                if (output.IContactMessage != null)
                {
                    logger.Info("RequestGetIndexProperties result:" + output.IContactMessage.ToString());
                    if ("200".Equals(output.MessageCode))
                        response = output.IContactMessage;
                }
                else
                {
                    logger.Error("Error occurred while RequestGetIndexProperties : " + output.Message);
                }

            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred as : " + generalException.ToString());
            }
            return response;
        }

        public IMessage GetInteractionContent(string interactionID, bool isAttachments)
        {
            return Pointel.Interactions.Contact.Core.Request.RequestToGetInteractionContent.GetInteractionContent(interactionID, isAttachments);
        }

        public IMessage GetInteractionList(string contactID, int tenantId, string interactionId, List<string> attributesNames)
        {
            IMessage response = null;
            try
            {
                OutputValues output = Pointel.Interactions.Contact.Core.Request.RequestGetInteractionList.GetRecentInteractionList(contactID, tenantId, interactionId, attributesNames);
                if (output != null)
                    response = output.IContactMessage;

            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred as : " + generalException.ToString());
            }
            return response;
        }

        public IMessage GetInteractionList(int tenantId, int pagemaxSize, EntityTypes entitypes, SearchCriteriaCollection searchCriteriaCollection, List<string> attributesNames)
        {
            return RequestGetInteractionList.GetInteractionList(tenantId, pagemaxSize, entitypes, searchCriteriaCollection, attributesNames).IContactMessage;
        }

        public IMessage GetMediaViceInteractionCount(string mediaType, string contactId, string interactionId)
        {
            IMessage response = null;
            try
            {
                OutputValues output = Pointel.Interactions.Contact.Core.Request.RequestInteractionCount.GetMediaViceInteractionCount(mediaType, contactId, interactionId);
                if (output != null)
                    response = output.IContactMessage;

            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred as : " + generalException.ToString());
            }
            return response;
        }

        public void GetResponse(string Name, string getresponse)
        {
        }

        public string GetResponseFieldContents(AgentProperties agentProperties, ContactProperties contactProperties, ContactInteractionProperties interactionProperties,
            string renderText)
        {
            string renderedText = string.Empty;
            OutputValues output = OutputValues.GetInstance();
            output = RequestGetFieldCodes.GetRenderFieldCodes(agentProperties, contactProperties, interactionProperties, renderText);
            if (output.IContactMessage != null)
            {
                if (output.IContactMessage.Name == EventRenderFieldCodes.MessageName)
                    renderedText = ((EventRenderFieldCodes)output.IContactMessage).Text;
                else if (output.IContactMessage.Name == EventError.MessageName)
                    renderedText = ((EventError)output.IContactMessage).ErrorDescription;
            }
            return renderedText;
        }

        public System.Windows.Controls.UserControl GetResponseUserControl(bool isWebHostEnable, Func<string, string, string, string, AttachmentList, string> refFunction)
        {
            return new Controls.ContactResponse(isWebHostEnable, refFunction);
        }

        public IMessage GetTotalInteractionCount(string contactId, string interactionId)
        {
            IMessage response = null;
            try
            {
                OutputValues output = Pointel.Interactions.Contact.Core.Request.RequestInteractionCount.GetTotalInteractionCount(contactId, interactionId);
                if (output != null)
                    response = output.IContactMessage;

            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred as : " + generalException.ToString());
            }
            return response;
        }

        public void InitializeContact(string username, string agetnId, string placeId, int tenantDbId, string contactAppName, ConfService comObject,
            IPluginCallBack listener, int ixnProxyId)
        {
            ContactService contactService = new ContactService();
            OutputValues output = contactService.ConnectUCS(comObject, tenantDbId, contactAppName, ContactServerStateNotification);
            //ContactDataContext.GetInstance().UserName = username;
            // ContactDataContext.GetInstance().PlaceID = placeId;
            //ContactDataContext.GetInstance().AgentID = agetnId;
            ContactDataContext.messageToClient = listener;
            ContactDataContext.GetInstance().IxnProxyId = ixnProxyId;

            if (output.MessageCode == "200")
            {
                //ContactDataContext.ComObject = comObject;
                //ContactDataContext.GetInstance().ApplicationName = applicationName;
                // ConfigContainer.Instance().TenantDbId =ConfigContainer.Instance().TenantDbId;
                ConfigContainer.Instance().TenantDbId = tenantDbId;
                //ComClass.GetInstance().GetContactBusinessAttribute("ContactAttributes");
                if (ConfigContainer.Instance().AllKeys.Contains("contactBusinessAttribute"))
                {
                    ContactDataContext.GetInstance().ContactValidAttribute = (Dictionary<string, string>)ConfigContainer.Instance().GetValue("contactBusinessAttribute");
                }
                ContactDataContext.GetInstance().ContactDisplayedAttributes = ReadKey.ReadConfigKeys("contact.displayed-attributes",
                                                                              new string[] { "Title", "FirstName", "LastName", "PhoneNumber", "EmailAddress" },
                                                                              ContactDataContext.GetInstance().ContactValidAttribute.Keys.ToList());
                ContactDataContext.GetInstance().ContactMandatoryAttributes = ReadKey.ReadConfigKeys("contact.mandatory-attributes",
                                                                              new string[] { "Title", "FirstName", "LastName", "PhoneNumber", "EmailAddress" },
                                                                             ContactDataContext.GetInstance().ContactDisplayedAttributes);
                ContactDataContext.GetInstance().ContactMultipleValueAttributes = ReadKey.ReadConfigKeys("contact.multiple-value-attributes",
                                                                             new string[] { "PhoneNumber", "EmailAddress" },
                                                                            ContactDataContext.GetInstance().ContactDisplayedAttributes);

                ComClass.GetInstance().GetAllValues();
            }
        }

        public void NotifyAgentLogin(bool isLogedin, int? proxyId = null)
        {
            if (proxyId != null)
                ContactDataContext.GetInstance().IxnProxyId = (int)proxyId;
            //WorkbinUtility.Instance().IsAgentLoginIXN = isLogedin;
            //if (!isLogedin) return;
        }

        public void NotifyInteractionServerState(bool isOpen)
        {
            if (ContactDataContext.GetInstance().IsInteractionServerActive != isOpen)
            {
                Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                    try
                    {
                        ContactDataContext.GetInstance().IsInteractionServerActive = isOpen;
                        if (ContactHandler.InteractionServerNotificationHandler != null)
                            ContactHandler.InteractionServerNotificationHandler.Invoke();
                    }
                    catch (Exception ex)
                    {
                        logger.Error("Error occurred as " + ex.Message);
                    }
                }));
            }
        }

        public void NotifyTServerState(bool isOpen)
        {
            ContactDataContext.GetInstance().IsTServerActive = isOpen;
            //if (ContactDataContext.GetInstance().IsTServerActive != isOpen)
            //{
            //    Application.Current.Dispatcher.Invoke((Action)(delegate
            //    {
            //        ContactDataContext.GetInstance().IsTServerActive = isOpen;
            //        if (ContactHandler.TServerNotificationHandler != null)
            //            ContactHandler.TServerNotificationHandler.Invoke();
            //    }));
            //}
        }

        public void NotifyVoiceMediaStatus(bool isVoiceEnabled)
        {
        }

        public void NotifyWorkbinContentChanged(string interactionId, bool isPlacedIn)
        {
            if (!string.IsNullOrEmpty(interactionId))
            {
                //if (isPlacedIn)
                //{
                //    if (ContactDataContext.GetInstance().OpenedMailIds.Contains(interactionId))
                //        ContactDataContext.GetInstance().OpenedMailIds.Remove(interactionId);
                //}
                //else
                //{
                //    ContactDataContext.GetInstance().OpenedMailIds.Add(interactionId);
                //}
                if (ContactHandler.WorkbinChangedEvent != null)
                {
                    ContactHandler.WorkbinChangedEvent.Invoke(interactionId);
                }
            }
        }

        public void RefreshContactHistory(string interactionId, bool isDelete)
        {
            Thread notificationThread = new Thread(() =>
            {
                Application.Current.Dispatcher.BeginInvoke((Action)(delegate
                {
                    if (ContactHandler.HistoryRefreshHandler != null)
                        ContactHandler.HistoryRefreshHandler.Invoke(interactionId, isDelete);
                }));
            });
            notificationThread.Start();
        }

        public IMessage RemoveAttachDocument(string interactionId, string documentId)
        {
            IMessage response = null;
            try
            {
                OutputValues output = Pointel.Interactions.Contact.Core.Request.RequestToRemoveDocument.RemoveAttachDocument(interactionId, documentId);
                response = output.IContactMessage;
            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred as : " + generalException.ToString());
            }
            return response;
        }

        public IMessage RequestToInsertInteraction(InteractionContent interactionContent, InteractionAttributes interactionAttributes, BaseEntityAttributes baseEntityAttributes)
        {
            IMessage response = null;
            try
            {
                response = Pointel.Interactions.Contact.Core.Request.RequestToInsertInteraction.RequestInsertInteraction(interactionContent, interactionAttributes, baseEntityAttributes);
                //service.RequestToInsertInteraction(interactionContent, interactionAttributes, baseEntityAttributes);
            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred as : " + generalException.ToString());
            }
            return response;
        }

        public IMessage RequestToUpdate(InteractionContent interactionContent, InteractionAttributes interactionAttributes, BaseEntityAttributes baseEntityAttributes)
        {
            IMessage response = null;
            try
            {
                response = Pointel.Interactions.Contact.Core.Request.RequestToUpdateInteraction.RequestUpdateInteraction(interactionContent, interactionAttributes, baseEntityAttributes);
                //service.RequestToUpdateInteraction(interactionContent, interactionAttributes, baseEntityAttributes);
            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred as : " + generalException.ToString());
            }
            return response;
        }

        public IMessage SearchContact(string query, int maxCount, string searchType = "contact")
        {
            IMessage response = null;
            try
            {
                OutputValues output = Pointel.Interactions.Contact.Core.Request.RequestContactSearch.GetSearchResult(query, maxCount, searchType);
                if (output.IContactMessage != null)
                    logger.Info("Contact Search Result:" + output.IContactMessage.ToString());
                if ("200".Equals(output.MessageCode))
                    response = output.IContactMessage;
            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred as : " + generalException.ToString());
            }
            return response;
        }

        public void ShowContactDirectoryAddress(string to, string cc, string bcc)
        {
            // throw new NotImplementedException();
        }

        public void SubscribeUpdateNotification(Func<string, string, Genesyslab.Platform.Contacts.Protocols.ContactServer.AttributesList, string> eventUpdateNotification)
        {
            Settings.ContactDataContext.GetInstance().ContactUpdateEventNotification += new ContactUpdateEvent(eventUpdateNotification);
        }

        public void UnSubscribeUpdateNotification(Func<string, string, Genesyslab.Platform.Contacts.Protocols.ContactServer.AttributesList, string> eventUpdateNotification)
        {
            Settings.ContactDataContext.GetInstance().ContactUpdateEventNotification -= new ContactUpdateEvent(eventUpdateNotification);
        }

        public void UpdateContactUserControl(System.Windows.Controls.UserControl contactUsercontrol, string contactId, MediaTypes mediaType)
        {
            try
            {
                if (contactUsercontrol is Controls.ContactInformation)
                {
                    var usControl = contactUsercontrol as Controls.ContactInformation;
                    usControl.UpdateContactInformation(contactId, mediaType.ToString());
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred as " + ex.Message);
            }
        }

        public IMessage UpdateInteraction(string interactionID, int ownerId, string comment, KeyValueCollection userData, int status, string dtEndDate = null)
        {
            IMessage response = null;
            try
            {
                OutputValues output = Pointel.Interactions.Contact.Core.Request.RequestToUpdateInteraction.UpdateInteraction(interactionID, ownerId, comment, userData, status, dtEndDate);
                if (output != null)
                    response = output.IContactMessage;
            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred as : " + generalException.ToString());
            }
            return response;
        }

        private bool ContactServerStateNotification(bool isOpen)
        {
            Application.Current.Dispatcher.Invoke((Action)(delegate
               {
                   try
                   {
                       ContactDataContext.GetInstance().IsContactIndexFound = false;
                       ContactDataContext.GetInstance().IsInteractionIndexFound = false;
                       ContactDataContext.GetInstance().IsContactServerActive = isOpen;
                       if (isOpen)
                       {
                           IMessage result = GetIndexProperties();
                           if (result != null)
                           {
                               EventGetIndexProperties eventGetIndexProperties = result as EventGetIndexProperties;
                               foreach (IndexData index in eventGetIndexProperties.Indexes)
                               {
                                   switch (index.IndexName)
                                   {
                                       case "contact":
                                           ContactDataContext.GetInstance().IsContactIndexFound = true; break;
                                       case "interaction":
                                           ContactDataContext.GetInstance().IsInteractionIndexFound = true; break;

                                   }
                               }
                           }
                       }
                       if (ContactHandler.ContactServerNotificationHandler != null)
                       {
                           ContactHandler.ContactServerNotificationHandler.Invoke();
                       }
                       if (Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections.ContainsKey(Pointel.Interactions.IPlugins.Plugins.Workbin))
                       {
                           ((IWorkbinPlugin)Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections[Pointel.Interactions.IPlugins.Plugins.Workbin]).NotifyContactServerState(isOpen);
                       }
                       if (Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections.ContainsKey(Pointel.Interactions.IPlugins.Plugins.Email))
                       {
                           ((IEmailPlugin)Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections[Pointel.Interactions.IPlugins.Plugins.Email]).NotifyContactServerState(isOpen);
                       }
                   }
                   catch (Exception ex)
                   {
                       logger.Error("Error occurred as " + ex.Message);
                   }

               }));

            return true;
        }

        public void NotifyEmailMediaState(bool state)
        {
            ContactDataContext.GetInstance().IsEmailLogon = state;
            if (ContactHandler.EmailStateNotificationEvent != null)
                ContactHandler.EmailStateNotificationEvent.Invoke();
        }

        public IMessage GetContactsforForwardMail(string searchText, int maxresults, out bool isIndex, string[] contactIDs = null)
        {
            IMessage response = null;
            try
            {
                var stringlist = new string[] { "EmailAddress", "FirstName", "LastName", "PhoneNumber" };
                if (ContactDataContext.GetInstance().IsContactIndexFound)
                {
                    isIndex = true;
                    string query = "TenantId:" + ConfigContainer.Instance().TenantDbId;

                    //Add EmailAddress, FirstName, LastName and PhoneNumber for searching
                    string subQuery = "";
                    foreach (string key in stringlist)
                    {
                        if (!Settings.ContactDataContext.GetInstance().ContactValidAttribute.ContainsKey(key)) continue;
                        subQuery += (!string.IsNullOrEmpty(subQuery) ? " OR " : "") + key + ":" + searchText + "*";
                    }
                    if (!string.IsNullOrEmpty(subQuery))
                        query += " AND (" + subQuery + ")";

                    //Add ContactIDs for searching
                    subQuery = string.Empty;
                    if (contactIDs != null && contactIDs.Length > 0)
                    {
                        //System.Threading.Tasks.Parallel.ForEach(contactIDs, contactID =>
                        //{
                        //    subQuery += (!string.IsNullOrEmpty(subQuery) ? " OR " : "") + "Id" + ":" + contactID;
                        //});
                        for (int i = 0; i < contactIDs.Length; i++)
                        {
                            subQuery += (!string.IsNullOrEmpty(subQuery) ? " OR " : "") + "Id" + ":" + contactIDs[i];
                        }
                        if (!string.IsNullOrEmpty(subQuery))
                        {
                            query += " AND (" + subQuery + ")";
                        }
                        maxresults = contactIDs.Length;
                    }
                    response = SearchContact(query, maxresults);
                }
                else
                {
                    isIndex = false;
                    if (contactIDs != null && contactIDs.Length > 0)
                        return response;
                    //Assign Search Criteria Collection for searching
                    SearchCriteriaCollection searchCriteriaCollection = new SearchCriteriaCollection();

                    //Assign Complex Search Criteria for searching                  

                    var csub1 = new ComplexSearchCriteria() { Prefix = Prefixes.And }; // Add cChildOr1 in csub1
                    var csub2 = new ComplexSearchCriteria() { Prefix = Prefixes.And }; // Add cChildOr2 in csub2                  


                    //Add EmailAddress, FirstName, LastName and PhoneNumber for searching
                    StringList attributeList = new StringList();
                    string attributeValue = "*".Equals(searchText) ? "" : searchText;
                    csub1.Criterias = new SearchCriteriaCollection();
                    for (int index = 0; index < stringlist.Length; index++)
                    {
                        attributeList.Add(stringlist[index]);
                        var cChildOr1 = new ComplexSearchCriteria() { Prefix = Prefixes.Or };
                        cChildOr1.Criterias = new SearchCriteriaCollection();
                        SimpleSearchCriteria simpleSearchCriteria = new SimpleSearchCriteria() { AttrName = stringlist[index], AttrValue = attributeValue + "*", Operator = new NullableOperators(Operators.Like) };
                        cChildOr1.Criterias.Add(simpleSearchCriteria);
                        if (contactIDs != null && contactIDs.Length > 0)
                            csub1.Criterias.Add(cChildOr1);
                        else
                            searchCriteriaCollection.Add(cChildOr1);
                    }

                    //if (contactIDs != null && contactIDs.Length > 0)
                    //{                       
                    //Add ContactIDs for searching
                    //csub2.Criterias = new SearchCriteriaCollection();
                    //for (int i = 0; i < contactIDs.Length; i++)
                    //{
                    //    var cChildOr2 = new ComplexSearchCriteria() { Prefix = Prefixes.Or };
                    //    cChildOr2.Criterias = new SearchCriteriaCollection();
                    //    SimpleSearchCriteria simpleSearchCriteria = new SimpleSearchCriteria() { AttrName = Genesyslab.Platform.Contacts.Protocols.ContactSearchCriteriaConstants.ContactId, AttrValue = contactIDs[i], Operator = new NullableOperators(Operators.Equal) };
                    //    cChildOr2.Criterias.Add(simpleSearchCriteria);
                    //    // Add cChildOr2 in csub2
                    //    csub2.Criterias.Add(cChildOr2);
                    //}

                    //Finally add  sub1 and sub2 in searchCriteriaCollection
                    //searchCriteriaCollection.Add(csub1);
                    //searchCriteriaCollection.Add(csub2);
                    //}


                    attributeList.Add("MergeId");
                    response = GetContactList(ConfigContainer.Instance().TenantDbId, attributeList, searchCriteriaCollection, maxresults);
                }
            }
            catch (Exception generalException)
            {
                isIndex = false;
                logger.Error("Error occurred as : " + generalException.ToString());
            }
            return response;
        }
        #endregion Methods

    }
}