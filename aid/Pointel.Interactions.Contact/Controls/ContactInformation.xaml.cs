using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Genesyslab.Platform.Commons.Protocols;
using Genesyslab.Platform.Contacts.Protocols.ContactServer;
using Genesyslab.Platform.Contacts.Protocols.ContactServer.Events;
using Pointel.Configuration.Manager;
using Pointel.Interactions.Contact.Converters;
using Pointel.Interactions.Contact.Core.Common;
using Pointel.Interactions.Contact.Settings;
using Pointel.Interactions.Contact.Helpers;

/*
 * Implemented hidden keys:
 * ------------------------
 * 1.contact.enable.duplicate-contact
 * */
namespace Pointel.Interactions.Contact.Controls
{
    /// <summary>
    /// Interaction logic for ContactInformation.xaml
    /// </summary>
    public partial class ContactInformation : UserControl
    {
        # region Variable Declaration
        private Func<string, List<Pointel.Interactions.Contact.Helpers.Attribute>, string> contactInsertEvent = null;
        private Func<string> NotifycontactUpdate = null;
        //Common
        private string InteractionID;
        private string MediaType;
        private string ContactID;

        private bool isContactInfoLoad;
        bool isContactHistoryLoaded = false;
        private Pointel.Logger.Core.ILog logger = null;
        ContactConvertor convertor = null;
        //Information Tab
        private string ErrorMessage = null;
        private Dictionary<string, string> deletedAttribute = null;
        private ObservableCollection<Helpers.Attribute> groupData = null;
        private ObservableCollection<Helpers.Attribute> ContactList = null;
        private string oldValue = null;
        private DispatcherTimer _timerforcloseError = null;
        private bool _isEnableControl;

        // private ContactHistory _contHistory;
        #endregion

        #region Common Operations

        public ContactInformation(string contactId, string mediaType, bool isEnableControl = true)
        {
            InitializeComponent();
            SubscribeServerNotification();
            ContactID = contactId;
            MediaType = mediaType;
            _isEnableControl = isEnableControl;
            dgGeneralInfo.IsEnabled = dgGroupData.IsEnabled = btnSave.IsEnabled = btnReset.IsEnabled = _isEnableControl;

        }

        public ContactInformation(string mediaType, Func<string, List<Pointel.Interactions.Contact.Helpers.Attribute>, string> contactInsertEvent)
            : this(null, mediaType)
        {
            SubscribeServerNotification();
            if (contactInsertEvent != null)
            {
                this.contactInsertEvent = contactInsertEvent;
            }

        }

        public ContactInformation(string contactId, string interactionID, string mediaType, Func<string> notifyContactUpdated)
            : this(contactId, mediaType)
        {
            SubscribeServerNotification();
            if (notifyContactUpdated != null)
            {
                this.NotifycontactUpdate = notifyContactUpdated;
            }

        }

        ~ContactInformation()
        {
            try
            {
                UnSubcribeEvent();
                contactInsertEvent = null;
                NotifycontactUpdate = null;
                InteractionID = MediaType = ContactID = oldValue = null;
                logger = null;
                convertor = null;
                ErrorMessage = null;
                deletedAttribute = null;
                groupData = null;
                ContactList = null;
                _timerforcloseError = null;
                GC.Collect();
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred as " + ex.Message);
            }
        }


        public void UpdateContactInformation(string contactId, string mediaType)
        {
            ContactID = contactId;
            MediaType = mediaType;
            isContactInfoLoad = false;
        }

        private void SubscribeServerNotification()
        {
            ContactHandler.ContactUpdateNotificationHandler += ContactHandler_ContactUpdateNotificationHandler;
            ContactHandler.ContactServerNotificationHandler += ContactHandler_ContactServerNotificationHandler;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
         "AID");
                if (ContactDataContext.GetInstance().IsContactServerActive)
                {
                    if (!isContactInfoLoad)
                    {
                        if (string.IsNullOrEmpty(ContactID))
                            tabItemHistory.Visibility = Visibility.Hidden;
                        stkPnlInfo.Visibility = Visibility.Visible;
                        if (!BindContactDetails())
                        {
                            stkPnlInfo.Visibility = Visibility.Hidden;
                            return;
                        }
                        rowDockError.Height = new GridLength(0);
                        this.DataContext = ContactDataContext.GetInstance();
                        isContactInfoLoad = true;
                        tabitemInformation.IsSelected = true;
                    }
                }
                //If the contact server is not available, that time the error message will show only first time.
                else
                {
                    ShowUCSDisConnectMessage();
                }
                scv_Information.ScrollToTop();

            }
            catch (Exception ex)
            {
                logger.Error("Error occurred as " + ex.Message);
            }
        }

        void ContactHandler_ContactUpdateNotificationHandler(string contactId, List<Pointel.Interactions.Contact.Helpers.Attribute> contactRow)
        {
            if (contactId.Equals(ContactID))
                BindContactDetails();
        }

        private void ShowUCSDisConnectMessage()
        {
            lblError.Text = "Contact server is not active.";
            rowDockError.Height = GridLength.Auto;
            this.IsEnabled = false;
            //starttimerforerror();
        }
        private void HideMessage()
        {
            lblError.Text = "Contact server is not active.";
            rowDockError.Height = new GridLength(0);
            this.IsEnabled = true;
            //starttimerforerror();
        }

        void ContactHandler_ContactServerNotificationHandler()
        {
            try
            {
                if (ContactDataContext.GetInstance().IsContactServerActive)
                {
                    HideMessage();
                    if (!isContactInfoLoad)
                        UserControl_Loaded(null, null);
                }
                else
                    ShowUCSDisConnectMessage();
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred as " + ex.Message);
            }
        }

        private void UnSubcribeEvent()
        {
            ContactHandler.ContactServerNotificationHandler -= ContactHandler_ContactServerNotificationHandler;
            ContactHandler.ContactUpdateNotificationHandler -= ContactHandler_ContactUpdateNotificationHandler;
        }

        #endregion


        #region Contact Information Tab

        private void gridInfoLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                //BindContactDetails();
                // rowDockError.Height = new GridLength(0);
                switch (tbCallBack.SelectedIndex)
                {
                    case 0: stk_Options.Visibility = System.Windows.Visibility.Visible;
                        break;

                    case 1: stk_Options.Visibility = System.Windows.Visibility.Hidden;
                        break;

                    default: break;
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred as " + ex.Message);
            }
        }

        private bool BindContactDetails()
        {
            try
            {
                deletedAttribute = new Dictionary<string, string>();
                convertor = new ContactConvertor();
                if (string.IsNullOrEmpty(ContactID) || !ContactDataContext.GetInstance().IsContactServerActive)
                    ContactList = convertor.ConvertToObservableCollection(new AttributesList());
                else
                //(!string.IsNullOrEmpty(ContactID) && ContactDataContext.GetInstance().IsContactServerActive)
                {
                    AttributesList attribList = GetContactAttributeList(ContactID);
                    if (attribList == null)
                    {
                        starttimerforerror("Contact request failed");
                        // btnSave.IsEnabled = btnReset.IsEnabled = false;
                        EnableorDisableSaveandReset(false);
                        return false;
                    }
                    ContactList = convertor.ConvertToObservableCollection(attribList);
                }

                BindContactGrid();
                if (ContactDataContext.GetInstance().IsContactServerActive)
                {
                    btnSave.Visibility = btnReset.Visibility = Visibility.Visible;
                    EnableorDisableSaveandReset(false);
                }
                else
                {
                    btnSave.Visibility = btnReset.Visibility = Visibility.Collapsed;
                    lblError.Text = "Contact server is not active";
                }
                return true;

            }
            catch (Exception ex)
            {
                logger.Error("Error occurred as " + ex.Message);
            }
            return false;
        }

        private void BindContactGrid()
        {
            try
            {
                if (ContactList != null)
                {
                    logger.Debug("BindContactGrid() : Try to Bind contact grid");
                    if (Settings.ContactDataContext.GetInstance().ContactValidAttribute.Count == 0)
                    {
                        starttimerforerror("Unable to get the contact attribute from configuration.");
                        return;
                    }

                    dgGeneralInfo.ItemsSource = ContactList.Where(x => x.Type == Settings.ContactDataContext.AttributeType.Single);

                    groupData = new ObservableCollection<Helpers.Attribute>(ContactList.Where(x => x.Type == Settings.ContactDataContext.AttributeType.Multiple));
                    if (groupData != null)
                    {
                        ListCollectionView collection = new ListCollectionView(groupData);
                        collection.GroupDescriptions.Add(new PropertyGroupDescription("AttributeName"));
                        dgGroupData.ItemsSource = collection;

                        // For hiding radio button 
                        if (dgGroupData.Items != null && dgGroupData.Items.Groups != null)
                        {
                            var groups = dgGroupData.Items.Groups;
                            foreach (var item in groups)
                            {
                                CollectionViewGroup s = item as CollectionViewGroup;
                                if (ContactList.Where(x => x.AttributeName.Equals(s.Name.ToString())).ToList().Count == 1)
                                {
                                    int rowIndex = dgGroupData.Items.IndexOf(s.Items[0]);

                                    Microsoft.Windows.Controls.DataGridCell cell = GetCell(dgGroupData, rowIndex, 3);
                                    if (cell != null)
                                        cell.Visibility = Visibility.Collapsed;

                                    var rbPrimary = GetVisualChild<RadioButton>(GetCell(dgGroupData, rowIndex, 3));
                                    if (rbPrimary != null)
                                        rbPrimary.IsChecked = true;

                                    TextBox txtattribValue = GetVisualChild<TextBox>(GetCell(dgGroupData, rowIndex, 0));
                                    if (txtattribValue != null && string.IsNullOrEmpty(txtattribValue.Text))
                                    {
                                        Button btnDelete = GetVisualChild<Button>(GetCell(dgGroupData, rowIndex, 2));
                                        if (btnDelete != null)
                                            btnDelete.Visibility = Visibility.Collapsed;
                                    }

                                }
                            }
                            foreach (var item in dgGeneralInfo.Items)
                            {
                                var selectedAttribute = item as Helpers.Attribute;
                                //if (string.IsNullOrEmpty(GetVisualChild<TextBox>(GetCell(dgGeneralInfo, dgGeneralInfo.Items.IndexOf(selectedAttribute), 1)).Text))
                                if (string.IsNullOrEmpty(selectedAttribute.AttributeValue))
                                {
                                    Button btnDelete = GetVisualChild<Button>(GetCell(dgGeneralInfo, dgGeneralInfo.Items.IndexOf(selectedAttribute), 2));
                                    if (btnDelete != null)
                                        btnDelete.Visibility = System.Windows.Visibility.Collapsed;
                                }

                            }
                            logger.Info("Contact grid bound");
                        }
                        else
                        {
                            logger.Warn("There is no contact item found");
                        }
                    }
                    else
                    {
                        logger.Warn("Group data is null in contact");
                    }

                }
            }
            catch (Exception ex)
            {
                logger.Error("BindContactGrid() : " + ex.Message);
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Converters.ContactConvertor contactConvertor = new Converters.ContactConvertor();

                if (contactConvertor.checkMandatoryfields(ContactList, deletedAttribute, ref ErrorMessage))
                {
                    if (CheckPrimaryKeyValidation())
                    {
                        //Implemented by sakthi to Validate duplicate contact
                        //contact.enable.duplicate-contact - By default False
                        if (!(ConfigContainer.Instance().AllKeys.Contains("contact.enable.duplicate-contact")
                            && "true".Equals((string)ConfigContainer.Instance().GetValue("contact.enable.duplicate-contact"))))
                        {
                            //Need to check the if the contact exist or not. if exist means do not allow the user to create.
                            if (IsContactExist())
                            {
                                starttimerforerror("The contact with the same details already exist.");
                                return;
                            }
                        }
                        //Code to update Existing Contact.
                        if (ContactID != null)
                        {
                            OutputValues result = Pointel.Interactions.Contact.Core.Request.RequestUpdateAttribute.RequestUpdateAllAttribute(ContactID, ConfigContainer.Instance().TenantDbId, contactConvertor.GetInsertedAttributeList(ContactList), contactConvertor.GetUpdatedAttributeList(ContactList), contactConvertor.GetDeletedAttributeList(deletedAttribute));
                            if (result.IContactMessage != null && result.MessageCode != "2001" || result.IContactMessage.Name != "EventError")
                            {
                                CloseError(null, null);
                                //Implemented changes to notify  contact Update to Softphone bar.
                                Settings.ContactDataContext.GetInstance().NotifyContactUpdate("Update", ContactID, GetContactAttributeList(ContactID));
                                //Need to Write the code to Notify Contact Update.
                                List<Pointel.Interactions.Contact.Helpers.Attribute> primaryContact = ContactList.ToList();
                                //.Where(x => x.Isprimary == true || x.Type == ContactDataContext.AttributeType.Single).ToList();
                                string firstName = string.Empty;
                                string lastName = string.Empty;
                                string email = string.Empty;
                                string mobileNo = string.Empty;
                                ContactHandler.NofityContactUpdate(ContactID, primaryContact);
                            }
                            else
                            {
                                starttimerforerror("Cannot save the contact.");
                            }

                        }
                        //Code to Insert new contact.
                        else if (ContactID == null && contactInsertEvent != null)
                        {
                            OutputValues result = Pointel.Interactions.Contact.Core.Request.RequestToInsertContact.RequestInsertContact(ConfigContainer.Instance().TenantDbId, contactConvertor.GetInsertedAttributeList(ContactList));
                            if (result.IContactMessage != null && result.MessageCode != "2001" || result.IContactMessage.Name != "EventError")
                            {

                                CloseError(null, null);
                                List<Pointel.Interactions.Contact.Helpers.Attribute> primaryContact = ContactList.ToList();
                                //ContactList.Where(x => x.Isprimary == true || x.Type == ContactDataContext.AttributeType.Single).ToList();
                                EventInsert insertEvent = result.IContactMessage as EventInsert;
                                //Implemented changes to notify  contact Update to Softphone bar.
                                Settings.ContactDataContext.GetInstance().NotifyContactUpdate("Add", insertEvent.ContactId, GetContactAttributeList(ContactID));
                                contactInsertEvent(insertEvent.ContactId, primaryContact);
                            }
                            else
                            {
                                rowDockError.Height = GridLength.Auto;
                                starttimerforerror("Cannot save the contact.");
                            }

                        }

                        BindContactDetails();
                    }
                    else
                    {
                        starttimerforerror(ErrorMessage);
                        ErrorMessage = null;
                    }
                }
                else
                {
                    starttimerforerror(ErrorMessage);
                    ErrorMessage = null;
                }

            }
            catch (Exception ex)
            {
                logger.Error("btnSave_Click() : " + ex.Message);
            }
        }

        private bool IsContactExist()
        {
            // List<string> manditoryContact = (ConfigContainer.Instance().AllKeys.Contains("contact.mandatory-attributes") ? (string)ConfigContainer.Instance().GetValue("contact.mandatory-attributes") : "").Split(',').ToList();
            if (ContactDataContext.GetInstance().IsContactIndexFound)
            {
                Dictionary<string, string> attributeValue = new Dictionary<string, string>();
                string query = "TenantId:" + ConfigContainer.Instance().TenantDbId;
                foreach (string attribute in ContactDataContext.GetInstance().ContactMandatoryAttributes)
                {
                    string subQuery = "";
                    foreach (Helpers.Attribute attr in ContactList.Where(x => x.AttributeName.Equals(ContactDataContext.GetInstance().ContactValidAttribute[attribute]) && !string.IsNullOrEmpty(x.AttributeValue)).ToList())
                    {
                        subQuery += !string.IsNullOrEmpty(subQuery) ? " OR " : attribute + ":" + attr.AttributeValue;
                    }
                    if (!string.IsNullOrEmpty(subQuery))
                        query += " AND (" + subQuery + ")";
                }
                ContactHandler contactHandler = new ContactHandler();
                IMessage result = contactHandler.SearchContact(query, 50);
                if (result != null)
                {
                    EventSearch eventSearch = result as EventSearch;
                    if (string.IsNullOrEmpty(ContactID))
                        return (eventSearch.Documents != null && eventSearch.Documents.Count > 0);
                    else if (eventSearch.Documents != null && eventSearch.Documents.Count > 0)
                    {
                        //Id
                        bool existingcontactFound = false;
                        for (int index = 0; index < eventSearch.Documents.Count; index++)
                            if (eventSearch.Documents[0].Fields["Id"].ToString() == ContactID)
                            {
                                existingcontactFound = true;
                                break;
                            }

                        if ((existingcontactFound && eventSearch.Documents.Count > 1) || (!existingcontactFound && eventSearch.Documents.Count > 0))
                        {
                            return true;
                        }


                    }
                }
                else
                {
                    ContactDataContext.GetInstance().IsContactIndexFound = false;
                    return IsContactExist();
                }
            }
            else
            {
                SearchCriteriaCollection SearchCriteria = new SearchCriteriaCollection();
                StringList attributeList = new StringList();
                foreach (string attribute in ContactDataContext.GetInstance().ContactMandatoryAttributes)
                {
                    attributeList.Add(attribute);
                    foreach (Helpers.Attribute attr in ContactList.Where(x => x.AttributeName.Equals(ContactDataContext.GetInstance().ContactValidAttribute[attribute]) && !string.IsNullOrEmpty(x.AttributeValue)).ToList())
                    {
                        ComplexSearchCriteria complexSearchCriteria = null;
                        complexSearchCriteria = new ComplexSearchCriteria() { Prefix = Prefixes.And };
                        complexSearchCriteria.Criterias = new SearchCriteriaCollection();
                        SimpleSearchCriteria searchCritiria1 = new SimpleSearchCriteria()
                        {
                            AttrName = attribute,
                            AttrValue = attr.AttributeValue,
                            Operator = new NullableOperators(Operators.Like)
                        };
                        complexSearchCriteria.Criterias.Add(searchCritiria1);
                        SearchCriteria.Add(complexSearchCriteria);
                    }
                }
                ContactHandler contactHandler = new ContactHandler();
                IMessage result = contactHandler.GetContactList(ConfigContainer.Instance().TenantDbId, attributeList, SearchCriteria);
                if (result != null)
                {
                    EventContactListGet eventContactListGet = result as EventContactListGet;
                    if (string.IsNullOrEmpty(ContactID))
                        return (eventContactListGet != null && eventContactListGet.ContactData != null && eventContactListGet.ContactData.Count > 0);
                    else if (eventContactListGet != null && eventContactListGet.ContactData != null && eventContactListGet.ContactData.Count > 0)
                    {
                        if (eventContactListGet.ContactData.Cast<Genesyslab.Platform.Contacts.Protocols.ContactServer.Contact>().Where(x => x.Id == ContactID).ToList().Count > 0)
                            return eventContactListGet.ContactData.Count != 1;
                        else return true;
                    }
                }
            }
            return false;
        }

        private bool CheckPrimaryKeyValidation()
        {
            try
            {
                if (ContactList.Where(x => string.IsNullOrEmpty(x.AttributeValue.Trim()) && x.Isprimary && x.Type == ContactDataContext.AttributeType.Multiple && (ContactList.Where(y => y.AttributeName.Equals(x.AttributeName)).ToList().Count > 1)).ToList().Count > 0)
                {
                    ErrorMessage = "Primary value cannot be empty";
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                logger.Error("CheckPrimaryKeyValidation() : " + ex.Message);
                return false;
            }
        }

        void starttimerforerror(string msg)
        {
            try
            {
                rowDockError.Height = GridLength.Auto;
                lblError.Text = msg;
                msg = null;
                _timerforcloseError = new DispatcherTimer();
                _timerforcloseError.Interval = TimeSpan.FromSeconds(10);
                _timerforcloseError.Tick += new EventHandler(CloseError);
                _timerforcloseError.Start();
            }
            catch (Exception ex)
            {
                logger.Error("starttimerforerror() : " + ex.Message);
            }

        }

        void CloseError(object sender, EventArgs e)
        {
            try
            {
                rowDockError.Height = new GridLength(0);
                if (_timerforcloseError != null)
                {
                    _timerforcloseError.Stop();
                    _timerforcloseError.Tick -= CloseError;
                    _timerforcloseError = null;
                }
            }
            catch (Exception ex)
            {
                logger.Error("CloseError() : " + ex.Message);
            }

        }

        private bool checkMandatoryfields()
        {
            try
            {
                CloseError(null, null);
                bool status = true;
                //List<string> items = ((string)ConfigContainer.Instance().GetValue("contact.mandatory-attributes")).Split(',').ToList();
                foreach (string mandatoryList in ContactDataContext.GetInstance().ContactMandatoryAttributes)
                {
                    string attributeName = Settings.ContactDataContext.GetInstance().ContactValidAttribute.Where(y => y.Value.Equals(mandatoryList)).Single().Key;
                    if (ContactList.Where(x => x.AttributeName.Equals(attributeName) && !string.IsNullOrEmpty(x.AttributeValue) && (deletedAttribute.Count != 0 ? !deletedAttribute.ContainsKey(x.AttributeId) : true)).ToList().Count == 0)
                    {
                        status = false;
                        if (!string.IsNullOrEmpty(ErrorMessage))
                        {
                            ErrorMessage += ", " + attributeName;
                        }
                        else
                        {
                            ErrorMessage = attributeName;
                        }

                    }
                }
                if (!status)
                {
                    if (ErrorMessage.IndexOf(',') >= 0)
                        ErrorMessage = "Can't save this contact. Mandatory fields are missing(" + ErrorMessage + ")";
                    else
                        ErrorMessage = "Can't save this contact. Mandatory field is missing(" + ErrorMessage + ")";
                }

                return status;
            }
            catch (Exception ex)
            {
                logger.Error("checkMandatoryfields() : " + ex.Message);
                return false;
            }
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                BindContactDetails();
                CloseError(null, null);
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred as " + ex.Message);
            }
        }


        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                string attributeName = ((((sender as Button).Content) as StackPanel).Children[1] as TextBlock).Text;
                if (ContactList.Count(p => p.AttributeName == attributeName) == 1)
                {
                    int rowIndex = GetGroupRowIndex(attributeName);
                    GetVisualChild<Button>(GetCell(dgGroupData, rowIndex, 2)).Visibility = Visibility.Visible;
                    //GetCell(dgGroupData, rowIndex, 2).Visibility = Visibility.Visible;
                    GetCell(dgGroupData, rowIndex, 3).Visibility = Visibility.Visible;
                    var rbPrimary = GetVisualChild<RadioButton>(GetCell(dgGroupData, rowIndex, 3));
                    rbPrimary.IsChecked = true;
                }
                if (attributeName != string.Empty)
                {
                    Helpers.Attribute attribute = new Helpers.Attribute { AttributeId = "", AttributeName = attributeName, AttributeValue = "", Description = "", Isprimary = false, IsAltered = true, IsMandatory = false, Type = Settings.ContactDataContext.AttributeType.Multiple };
                    ContactList.Add(attribute);
                    groupData.Add(attribute);
                }

            }
            catch (Exception ex)
            {
                logger.Error("Error occurred as " + ex.Message);
            }
        }

        private void rbPrimary_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                RadioButton rbPrimary = sender as RadioButton;
                rbPrimary.Content = "Primary";
                var attrID = rbPrimary.Tag;
                var groupName = rbPrimary.GroupName;
                if (!string.IsNullOrEmpty(groupName))
                    if (attrID != null && attrID != "")
                    {
                        if (convertor.TempOldContact.Where(x => x.AttributeId == attrID && x.Isprimary == true).ToList().Count > 0)
                        {
                            convertor.TempOldContact.Where(x => (x.AttributeId == attrID) && x.AttributeName.Equals(groupName)).Single().IsAltered = false;

                            if (convertor.TempOldContact.Where(x => x.IsAltered == true).ToList().Count == 0)
                            {
                                //if (ContactList.Where(x => !string.IsNullOrEmpty(x.AttributeValue) && !string.IsNullOrEmpty(x.Description) && x.AttributeId == "" && x.Type == ContactDataContext.AttributeType.Multiple).ToList().Count > 0)
                                if (ContactList.Where(x => !string.IsNullOrEmpty(x.AttributeValue) && x.AttributeId == "" && x.Type == ContactDataContext.AttributeType.Multiple).ToList().Count == 0)
                                {
                                    EnableorDisableSaveandReset(false);
                                }
                            }
                            Style removeunsavedstyle = (Style)FindResource("rbstyle");
                            rbPrimary.Style = removeunsavedstyle;
                        }
                        else
                        {
                            convertor.TempOldContact.Where(x => (x.AttributeId == attrID) && x.AttributeName.Equals(groupName)).Single().IsAltered = true;
                            EnableorDisableSaveandReset(true);
                            Style unsavedstyle = (Style)FindResource("rbunsavedstyle");
                            rbPrimary.Style = unsavedstyle;
                        }

                    }
                    else
                    {

                        EnableorDisableSaveandReset(true);
                        Style unsavedstyle = (Style)FindResource("rbunsavedstyle");
                        rbPrimary.Style = unsavedstyle;
                    }

            }
            catch (Exception ex)
            {
                logger.Error("Error occurred as " + ex.Message);
            }
        }

        private void rbPrimary_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                RadioButton rbPrimary = sender as RadioButton;
                rbPrimary.Content = "";
                var attrID = rbPrimary.Tag;
                var groupName = rbPrimary.GroupName;
                if (!string.IsNullOrEmpty(groupName))
                    if (attrID != null && attrID != "")
                    {
                        if (convertor.TempOldContact.Where(x => x.AttributeId == attrID && x.Isprimary == false).ToList().Count > 0)
                        {
                            convertor.TempOldContact.Where(x => (x.AttributeId == attrID) && x.AttributeName.Equals(groupName)).Single().IsAltered = false;
                            if (convertor.TempOldContact.Where(x => x.IsAltered == true).ToList().Count == 0)
                            {
                                if (ContactList.Where(x => !string.IsNullOrEmpty(x.AttributeValue) && x.AttributeId == "" && x.Type == ContactDataContext.AttributeType.Multiple).ToList().Count == 0)
                                {
                                    EnableorDisableSaveandReset(false);
                                }
                            }
                            Style removeunsavedstyle = (Style)FindResource("rbstyle");
                            rbPrimary.Style = removeunsavedstyle;
                        }
                        else
                        {
                            convertor.TempOldContact.Where(x => (x.AttributeId == attrID) && x.AttributeName.Equals(groupName)).Single().IsAltered = true;
                            EnableorDisableSaveandReset(true);
                            Style unsavedstyle = (Style)FindResource("rbunsavedstyle");
                            rbPrimary.Style = unsavedstyle;
                        }
                    }
                    else
                    {
                        if (convertor.TempOldContact.Where(x => x.IsAltered == true).ToList().Count == 0)
                        {
                            if (ContactList.Where(x => !string.IsNullOrEmpty(x.AttributeValue) && x.AttributeId == "" && x.Type == ContactDataContext.AttributeType.Multiple).ToList().Count == 0)
                            {
                                EnableorDisableSaveandReset(false);
                            }
                        }
                        Style removeunsavedstyle = (Style)FindResource("rbstyle");
                        rbPrimary.Style = removeunsavedstyle;
                    }
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred as " + ex.Message);
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                var selectedAttribute = dgGroupData.SelectedItem as Helpers.Attribute;

                if (selectedAttribute != null)
                {
                    if (ContactList.Count > 0)
                    {

                        if (ContactList.Count(p => p.AttributeName == selectedAttribute.AttributeName) == 1)
                        {
                            int rowIndex = GetGroupRowIndex(selectedAttribute.AttributeName);

                            if (!string.IsNullOrEmpty(selectedAttribute.AttributeId) && !string.IsNullOrEmpty(selectedAttribute.AttributeName))
                            {
                                Helpers.Attribute attribute = new Helpers.Attribute { AttributeId = "", AttributeName = selectedAttribute.AttributeName, AttributeValue = "", Description = "", Isprimary = true, IsAltered = true, IsMandatory = false, Type = Settings.ContactDataContext.AttributeType.Multiple };
                                ContactList.Add(attribute);
                                groupData.Add(attribute);

                                if (!deletedAttribute.ContainsKey(selectedAttribute.AttributeId))
                                    deletedAttribute.Add(selectedAttribute.AttributeId, selectedAttribute.AttributeName);
                                else
                                    deletedAttribute[selectedAttribute.AttributeId] = selectedAttribute.AttributeName;

                                if (ContactList.Contains(selectedAttribute))
                                    ContactList.Remove(selectedAttribute);
                                groupData.Remove(selectedAttribute);

                                var txtAttrValue = GetVisualChild<TextBox>(GetCell(dgGroupData, rowIndex, 0));
                                var txtDescription = GetVisualChild<TextBox>(GetCell(dgGroupData, rowIndex, 1));
                                convertor.TempOldContact.Where(x => (x.AttributeId == selectedAttribute.AttributeId) && x.AttributeName.Equals(selectedAttribute.AttributeName)).Single().IsAltered = true;
                                EnableorDisableSaveandReset(true);
                                Style unsavedstyle = (Style)FindResource("MyWaterMarkStyleunsaved");
                                txtAttrValue.Style = unsavedstyle;
                                txtDescription.Style = unsavedstyle;
                            }
                            else
                            {
                                var txtAttrValue = GetVisualChild<TextBox>(GetCell(dgGroupData, rowIndex, 0));
                                var txtDescription = GetVisualChild<TextBox>(GetCell(dgGroupData, rowIndex, 1));
                                txtAttrValue.Text = string.Empty;
                                txtDescription.Text = string.Empty;
                                Style unsavedstyle = (Style)FindResource("MyWaterMarkStyleunsaved");
                                txtAttrValue.Style = unsavedstyle;
                                txtDescription.Style = unsavedstyle;
                            }
                            GetVisualChild<Button>(GetCell(dgGroupData, rowIndex, 2)).Visibility = Visibility.Collapsed;
                            //    GetCell(dgGroupData, rowIndex, 2).Visibility = Visibility.Hidden;
                            GetCell(dgGroupData, rowIndex, 3).Visibility = Visibility.Hidden;

                        }
                        else if (ContactList.Count(p => p.AttributeName == selectedAttribute.AttributeName) > 1)
                        {
                            int rowIndex = GetGroupRowIndex(selectedAttribute.AttributeName);
                            var rbPrimary = GetVisualChild<RadioButton>(GetCell(dgGroupData, dgGroupData.Items.IndexOf(selectedAttribute), 3));

                            ContactList.Remove(selectedAttribute);
                            groupData.Remove(selectedAttribute);
                            if (!string.IsNullOrEmpty(selectedAttribute.AttributeId) && !string.IsNullOrEmpty(selectedAttribute.AttributeName))
                            {
                                if (!deletedAttribute.ContainsKey(selectedAttribute.AttributeId))
                                    deletedAttribute.Add(selectedAttribute.AttributeId, selectedAttribute.AttributeName);
                                else
                                    deletedAttribute[selectedAttribute.AttributeId] = selectedAttribute.AttributeName;
                                convertor.TempOldContact.Where(x => (x.AttributeId == selectedAttribute.AttributeId) && x.AttributeName.Equals(selectedAttribute.AttributeName)).Single().IsAltered = true;
                                EnableorDisableSaveandReset(true);
                            }
                            else
                            {
                                if (convertor.TempOldContact.Where(x => x.IsAltered == true).ToList().Count == 0)
                                {
                                    if (ContactList.Where(x => !string.IsNullOrEmpty(x.AttributeValue) && x.AttributeId == "" && x.Type == ContactDataContext.AttributeType.Multiple).ToList().Count == 0)
                                    {
                                        EnableorDisableSaveandReset(false);
                                    }
                                }
                            }

                            //  BindContactGrid();
                            if (rbPrimary.IsChecked == true)
                            {
                                rbPrimary = GetVisualChild<RadioButton>(GetCell(dgGroupData, rowIndex, 3));
                                rbPrimary.IsChecked = true;
                            }


                            if (ContactList.Count(p => p.AttributeName == selectedAttribute.AttributeName) == 1)
                            {
                                bool IsDataExist = false;

                                var txtAttrValue = GetVisualChild<TextBox>(GetCell(dgGroupData, rowIndex, 0));
                                if (!string.IsNullOrEmpty(txtAttrValue.Text))
                                    IsDataExist = true;

                                var txtDescription = GetVisualChild<TextBox>(GetCell(dgGroupData, rowIndex, 1));
                                if (!string.IsNullOrEmpty(txtAttrValue.Text))
                                    IsDataExist = true;

                                if (!IsDataExist)
                                {
                                    GetVisualChild<Button>(GetCell(dgGroupData, rowIndex, 2)).Visibility = Visibility.Collapsed;
                                    // GetCell(dgGroupData, rowIndex, 2).Visibility = Visibility.Hidden;
                                }
                                GetCell(dgGroupData, rowIndex, 3).Visibility = Visibility.Hidden;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred as " + ex.Message);
            }
        }

        private Microsoft.Windows.Controls.DataGridCell TryToFindGridCell(Microsoft.Windows.Controls.DataGrid grid, Microsoft.Windows.Controls.DataGridCellInfo cellInfo)
        {
            try
            {
                Microsoft.Windows.Controls.DataGridCell result = null;
                Microsoft.Windows.Controls.DataGridRow row = null;
                grid.ScrollIntoView(cellInfo.Item);
                grid.UpdateLayout();
                row = (Microsoft.Windows.Controls.DataGridRow)grid.ItemContainerGenerator.ContainerFromItem(cellInfo.Item);
                if (row != null)
                {
                    int columnIndex = grid.Columns.IndexOf(cellInfo.Column);
                    if (columnIndex > -1)
                    {
                        Microsoft.Windows.Controls.Primitives.DataGridCellsPresenter presenter = GetVisualChild<Microsoft.Windows.Controls.Primitives.DataGridCellsPresenter>(row);
                        result = presenter.ItemContainerGenerator.ContainerFromIndex(columnIndex) as Microsoft.Windows.Controls.DataGridCell;
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred as " + ex.Message);
                return null;
            }
        }

        public T GetVisualChild<T>(Visual parent) where T : Visual
        {
            try
            {
                if (parent != null)
                {
                    T child = default(T);
                    int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
                    if (numVisuals > 0)
                    {
                        var vw = (Visual)VisualTreeHelper.GetChild(parent, 0);
                        for (int i = 0; i < numVisuals; i++)
                        {
                            var v = (Visual)VisualTreeHelper.GetChild(parent, i);
                            child = v as T ?? GetVisualChild<T>(v);
                            if (child != null)
                            {
                                break;
                            }
                        }
                        return child;
                    }

                }

            }
            catch (Exception ex)
            {
                logger.Error("Error occurred as " + ex.Message);
            }
            return null;
        }

        private int GetGroupRowIndex(string attributeName)
        {
            try
            {

                int dgRowIndex = 0;
                var groups = dgGroupData.Items.Groups;
                foreach (var item in groups)
                {
                    CollectionViewGroup s = item as CollectionViewGroup;
                    if (s.Name.ToString() == attributeName)
                    {
                        dgRowIndex = dgGroupData.Items.IndexOf(s.Items[0]);
                    }
                }
                return dgRowIndex;
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred as " + ex.Message);
                return -1;
            }
        }

        private Microsoft.Windows.Controls.DataGridCell GetCell(Microsoft.Windows.Controls.DataGrid grid, int rowIndex, int coloumnCount)
        {
            try
            {
                Microsoft.Windows.Controls.DataGridCell cell = null;
                var dataGridCellInfo = new Microsoft.Windows.Controls.DataGridCellInfo(grid.Items[rowIndex], grid.Columns[coloumnCount]);
                cell = TryToFindGridCell(grid, dataGridCellInfo);
                return cell;
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred as " + ex.Message);
                return null;
            }
        }

        #region Monitor attribute change event for general contact
        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                oldValue = (sender as TextBox).Text;
                //Added by sakthi to handle the issues bw single and multiple attribute. 21-02-2015
                dgGroupData.SelectedItem = null;
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred as " + ex.Message);
            }

        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {

                if (!(sender as TextBox).Text.Equals(oldValue))
                {
                    var selectedAttribute = dgGeneralInfo.SelectedItem as Helpers.Attribute;
                    if (selectedAttribute.AttributeId != null)
                        if (deletedAttribute.ContainsKey(selectedAttribute.AttributeId))
                            deletedAttribute.Remove(selectedAttribute.AttributeId);
                    //ContactList[ContactList.IndexOf(selectedAttribute)].AttributeValue = (sender as TextBox).Text;
                    ContactList[ContactList.IndexOf(selectedAttribute)].IsAltered = true;
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred as " + ex.Message);
            }
        }

        private void btnDeleteSingleAtt_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                (sender as Button).Visibility = Visibility.Hidden;
                var selectedAttribute = dgGeneralInfo.SelectedItem as Helpers.Attribute;
                var txtAttrValue = GetVisualChild<TextBox>(GetCell(dgGeneralInfo, dgGeneralInfo.Items.IndexOf(selectedAttribute), 1));
                txtAttrValue.Text = string.Empty;
                if (!string.IsNullOrEmpty(selectedAttribute.AttributeId) && !string.IsNullOrEmpty(selectedAttribute.AttributeName))
                {
                    if (!deletedAttribute.ContainsKey(selectedAttribute.AttributeId))
                        deletedAttribute.Add(selectedAttribute.AttributeId, selectedAttribute.AttributeName);
                    else
                        deletedAttribute[selectedAttribute.AttributeId] = selectedAttribute.AttributeName;
                }
                if (convertor.TempOldContact.Where(x => x.AttributeId == selectedAttribute.AttributeId && x.AttributeValue == string.Empty).ToList().Count == 0)
                {
                    convertor.TempOldContact.Where(x => (x.AttributeId == selectedAttribute.AttributeId) && x.AttributeName.Equals(selectedAttribute.AttributeName)).Single().IsAltered = true;
                    EnableorDisableSaveandReset(true);
                    Style unsavedstyle = (Style)FindResource("MyWaterMarkStyleunsaved");
                    txtAttrValue.Style = unsavedstyle;
                }
                else
                {
                    convertor.TempOldContact.Where(x => (x.AttributeId == selectedAttribute.AttributeId) && x.AttributeName.Equals(selectedAttribute.AttributeName)).Single().IsAltered = false;
                    if (convertor.TempOldContact.Where(x => x.IsAltered == true).ToList().Count == 0)
                    {
                        EnableorDisableSaveandReset(false);
                    }
                    Style removeunsavedstyle = (Style)FindResource("MyWaterMarkStyle");
                    txtAttrValue.Style = removeunsavedstyle;
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred as " + ex.Message);
            }
        }
        #endregion

        private void txtAttributeAndDescription_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                var obcoll = groupData;
                if (!(sender as TextBox).Text.Equals(oldValue))
                {

                    var selectedAttribute = dgGroupData.SelectedItem as Helpers.Attribute;
                    if (selectedAttribute != null)
                    {
                        ContactList[ContactList.IndexOf(selectedAttribute)].IsAltered = true;
                        if (selectedAttribute.AttributeId != null)
                            if (deletedAttribute.ContainsKey(selectedAttribute.AttributeId))
                                deletedAttribute.Remove(selectedAttribute.AttributeId);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred as " + ex.Message);
            }
        }

        private void EnableorDisableSaveandReset(bool cond)
        {
            btnSave.IsEnabled = btnReset.IsEnabled = cond;
        }

        private void txtAttributeAndDescription_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                var selectedAttribute = dgGroupData.SelectedItem as Helpers.Attribute;

                if (selectedAttribute != null)
                {
                    if (ContactList.Count(p => p.AttributeName == selectedAttribute.AttributeName) == 1)
                    {
                        var txtAttrValue = GetVisualChild<TextBox>(GetCell(dgGroupData, dgGroupData.Items.IndexOf(selectedAttribute), 0));
                        var txtDescription = GetVisualChild<TextBox>(GetCell(dgGroupData, dgGroupData.Items.IndexOf(selectedAttribute), 1));
                        if (txtAttrValue.Text.Trim() == "" && txtDescription.Text.Trim() == "")
                            GetVisualChild<Button>(GetCell(dgGroupData, dgGroupData.Items.IndexOf(selectedAttribute), 2)).Visibility = Visibility.Collapsed;
                        //GetCell(dgGroupData, dgGroupData.Items.IndexOf(selectedAttribute), 2).Visibility = Visibility.Hidden;
                        else
                            GetVisualChild<Button>(GetCell(dgGroupData, dgGroupData.Items.IndexOf(selectedAttribute), 2)).Visibility = Visibility.Visible;
                        //GetCell(dgGroupData, dgGroupData.Items.IndexOf(selectedAttribute), 2).Visibility = Visibility.Visible;
                    }
                    if (selectedAttribute.AttributeId != null && selectedAttribute.AttributeId != string.Empty)
                    {
                        if ((sender as TextBox).Name == "txtAttributeValue")
                        {
                            if (convertor.TempOldContact.Where(x => x.AttributeId == selectedAttribute.AttributeId && x.AttributeValue == (sender as TextBox).Text).ToList().Count == 0)
                            {
                                convertor.TempOldContact.Where(x => (x.AttributeId == selectedAttribute.AttributeId) && x.AttributeName.Equals(selectedAttribute.AttributeName)).Single().IsAltered = true;
                                EnableorDisableSaveandReset(true);
                                Style unsavedstyle = (Style)FindResource("MyWaterMarkStyleunsaved");
                                (sender as TextBox).Style = unsavedstyle;
                            }
                            else
                            {
                                convertor.TempOldContact.Where(x => (x.AttributeId == selectedAttribute.AttributeId) && x.AttributeName.Equals(selectedAttribute.AttributeName)).Single().IsAltered = false;
                                if (convertor.TempOldContact.Where(x => x.IsAltered == true).ToList().Count == 0)
                                {
                                    if (ContactList.Where(x => !string.IsNullOrEmpty(x.AttributeValue) && x.AttributeId == "" && x.Type == ContactDataContext.AttributeType.Multiple).ToList().Count == 0)
                                    {

                                        EnableorDisableSaveandReset(false);

                                    }
                                }
                                Style removeunsavedstyle = (Style)FindResource("MyWaterMarkStyle");
                                (sender as TextBox).Style = removeunsavedstyle;
                            }

                        }
                        else
                        {
                            //Changed the logic to check the null with empty string.  By sakthi 21-02-2015

                            //if (convertor.TempOldContact.Where(x => x.AttributeId == selectedAttribute.AttributeId && x.Description == (sender as TextBox).Text).ToList().Count == 0)

                            if (convertor.TempOldContact.Where(x => x.AttributeId == selectedAttribute.AttributeId && ((x.Description == null) ? "" : x.Description).Equals((sender as TextBox).Text)).ToList().Count == 0)
                            {
                                convertor.TempOldContact.Where(x => (x.AttributeId == selectedAttribute.AttributeId) && x.AttributeName.Equals(selectedAttribute.AttributeName)).Single().IsAltered = true;
                                EnableorDisableSaveandReset(true);
                                Style unsavedstyle = (Style)FindResource("MyWaterMarkStyleunsaved");
                                (sender as TextBox).Style = unsavedstyle;
                            }
                            else
                            {
                                convertor.TempOldContact.Where(x => (x.AttributeId == selectedAttribute.AttributeId) && x.AttributeName.Equals(selectedAttribute.AttributeName)).Single().IsAltered = false;
                                if (convertor.TempOldContact.Where(x => x.IsAltered == true).ToList().Count == 0)
                                {
                                    if (ContactList.Where(x => !string.IsNullOrEmpty(x.AttributeValue) && x.AttributeId == "" && x.Type == ContactDataContext.AttributeType.Multiple).ToList().Count == 0)
                                    {
                                        if (ContactList.Where(x => !string.IsNullOrEmpty(x.AttributeValue) && x.AttributeId == "" && x.Type == ContactDataContext.AttributeType.Multiple).ToList().Count == 0)
                                        {
                                            EnableorDisableSaveandReset(false);
                                        }
                                    }
                                }
                                Style removeunsavedstyle = (Style)FindResource("MyWaterMarkStyle");
                                (sender as TextBox).Style = removeunsavedstyle;
                            }
                        }
                    }
                    else
                    {
                        if ((sender as TextBox).Text == "")
                        {
                            if (convertor.TempOldContact.Where(x => x.IsAltered == true).ToList().Count == 0)
                            {
                                if (ContactList.Where(x => !string.IsNullOrEmpty(x.AttributeValue) && x.AttributeId == "" && x.Type == ContactDataContext.AttributeType.Multiple).ToList().Count == 0)
                                {
                                    EnableorDisableSaveandReset(false);
                                }
                            }
                            Style removeunsavedstyle = (Style)FindResource("MyWaterMarkStyle");
                            (sender as TextBox).Style = removeunsavedstyle;
                        }
                        else
                        {
                            EnableorDisableSaveandReset(true);
                            Style unsavedstyle = (Style)FindResource("MyWaterMarkStyleunsaved");
                            (sender as TextBox).Style = unsavedstyle;

                        }

                    }
                }


            }
            catch (Exception ex)
            {
                logger.Error("Error occurred as " + ex.Message);
            }
        }

        private void btnCloseError_Click(object sender, RoutedEventArgs e)
        {
            CloseError(null, null);

        }

        private void lblError_Loaded(object sender, RoutedEventArgs e)
        {
            lblError.Width = grdErrorColumn.ActualWidth;
        }

        private void txtSingleAttr_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                var selectedAttribute = dgGeneralInfo.SelectedItem as Helpers.Attribute;
                if (selectedAttribute != null)
                {
                    var btndelsingleattr = GetVisualChild<Button>(GetCell(dgGeneralInfo, dgGeneralInfo.Items.IndexOf(selectedAttribute), 2));
                    if ((sender as TextBox).Text == "")
                        btndelsingleattr.Visibility = Visibility.Hidden;
                    else
                        btndelsingleattr.Visibility = Visibility.Visible;

                    if (convertor.TempOldContact.Where(x => x.AttributeId == selectedAttribute.AttributeId && x.AttributeValue == (sender as TextBox).Text).ToList().Count == 0)
                    {
                        convertor.TempOldContact.Where(x => (x.AttributeId == selectedAttribute.AttributeId) && x.AttributeName.Equals(selectedAttribute.AttributeName)).Single().IsAltered = true;
                        EnableorDisableSaveandReset(true);
                        Style unsavedstyle = (Style)FindResource("MyWaterMarkStyleunsaved");
                        (sender as TextBox).Style = unsavedstyle;
                    }
                    else
                    {
                        convertor.TempOldContact.Where(x => (x.AttributeId == selectedAttribute.AttributeId) && x.AttributeName.Equals(selectedAttribute.AttributeName)).Single().IsAltered = false;
                        if (convertor.TempOldContact.Where(x => x.IsAltered == true).ToList().Count == 0)
                        {
                            EnableorDisableSaveandReset(false);
                        }
                        Style removeunsavedstyle = (Style)FindResource("MyWaterMarkStyle");
                        (sender as TextBox).Style = removeunsavedstyle;
                    }
                    //if (tmplstContact.IndexOf(selectedAttribute)].AttributeValue != (sender as TextBox).Text)
                    //{
                    //    EnableorDisableSaveandReset(true);
                    //    Style unsavedstyle = (Style)FindResource("MyWaterMarkStyleunsaved");
                    //    (sender as TextBox).Style = unsavedstyle;
                    //}
                    //else
                    //{
                    //    EnableorDisableSaveandReset(false);
                    //    Style removeunsavedstyle = (Style)FindResource("MyWaterMarkStyle");
                    //    (sender as TextBox).Style = removeunsavedstyle;
                    //}
                }


            }
            catch (Exception ex)
            {
                logger.Error("Error occurred as " + ex.Message);
            }
        }

        #endregion

        #region History Tab

        private void brHistory_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                //if (_contHistory != null)
                //    _contHistory.MinWidth = 375;
                switch (tbCallBack.SelectedIndex)
                {
                    case 0:
                        stk_Options.Visibility = System.Windows.Visibility.Visible;
                        break;

                    case 1:
                        stk_Options.Visibility = System.Windows.Visibility.Collapsed;
                        break;

                    default: break;
                }
            }
            catch (Exception ex)
            {

                logger.Error("Error occurred as " + ex.Message);
            }
        }

        #endregion

        private void brHistory_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            try
            {
                brHistory.SizeChanged -= brHistory_SizeChanged;
                if (brHistory.Child != null)
                    (brHistory.Child as UserControl).Width = brHistory.ActualWidth;
                brHistory.SizeChanged += brHistory_SizeChanged;
            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred as " + generalException.ToString());
            }
        }

        private AttributesList GetContactAttributeList(string contactId)
        {
            try
            {
                //List<string> listContactDisplayedAttr;
                //if (ConfigContainer.Instance().AllKeys.Contains("contact.displayed-attributes") &&
                //   !string.IsNullOrEmpty(ConfigContainer.Instance().GetAsString("contact.displayed-attributes")))
                //    listContactDisplayedAttr = ConfigContainer.Instance().GetAsString("contact.displayed-attributes").Split(',').ToList();
                //else
                //    listContactDisplayedAttr = new List<string>(new string[] { "Title", "FirstName", "LastName", "PhoneNumber", "EmailAddress" });

                //listContactDisplayedAttr = listContactDisplayedAttr.Distinct().ToList();

                //foreach (var item in listContactDisplayedAttr)
                //{
                //    if (!(ContactDataContext.GetInstance().ContactValidAttribute.Keys.Contains(item.ToLower().Trim())))
                //    {
                //        listContactDisplayedAttr.Remove(item);
                //    }
                //}

                //if (listContactDisplayedAttr.Count == 0)
                //{
                //    listContactDisplayedAttr = new List<string>(new string[] { "Title", "FirstName", "LastName", "PhoneNumber", "EmailAddress" });
                //}

                //List<string> listContactDisplayedAttr = ReadKey.ReadConfigKeys("contact.displayed-attributes",
                //                                                                new string[] { "Title", "FirstName", "LastName", "PhoneNumber", "EmailAddress" },
                //                                                                ContactDataContext.GetInstance().ContactValidAttribute.Keys.ToList());

                OutputValues result = Pointel.Interactions.Contact.Core.Request.RequestGetAllAttributes.GetAttributeValues(contactId, ContactDataContext.GetInstance().ContactDisplayedAttributes);
                //ContactDataContext.GetInstance().DisplayedAttributes);
                if (result != null && result.IContactMessage.Name.Equals("EventGetAttributes"))
                {
                    EventGetAttributes attribute = result.IContactMessage as EventGetAttributes;

                    return attribute.Attributes;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return null;
        }

        private void DataGrid_GotFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                UpdateLayout();
                // Lookup for the source to be DataGridCell
                if (e.OriginalSource.GetType() == typeof(Microsoft.Windows.Controls.DataGridCell))
                {
                    var uiElement = GetVisualChild<TextBox>(e.OriginalSource as Microsoft.Windows.Controls.DataGridCell);
                    if (uiElement != null && !uiElement.Focusable)
                        uiElement.Focus();
                    var uiElement1 = GetVisualChild<Button>(e.OriginalSource as Microsoft.Windows.Controls.DataGridCell);
                    if (uiElement1 != null && !uiElement.Focusable)
                        uiElement1.Focus();
                    // Starts the Edit on the row;
                    //Microsoft.Windows.Controls.DataGrid grd = (Microsoft.Windows.Controls.DataGrid)sender;
                    //grd.BeginEdit(e);
                }
                else
                {
                    Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Input,
                    (System.Threading.ThreadStart)delegate
                    {
                        (e.OriginalSource as UIElement).Focus();
                    });
                }
            }
            catch { }
        }

        private void UserControl_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.S && Keyboard.Modifiers == ModifierKeys.Control)
                {
                    btnSave.Focus();
                    btnSave_Click(null, null);
                }
                else if (e.Key == Key.R && Keyboard.Modifiers == ModifierKeys.Control)
                {
                    btnReset_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred as " + ex.Message);
            }
        }

        private void tbCallBack_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (!isContactHistoryLoaded && tbCallBack.SelectedIndex == 1)
                {
                    brHistory.Child = new ContactHistory(ContactID, MediaType);
                    isContactHistoryLoaded = true;

                }
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred as " + ex.Message);
            }
        }

    }
}
