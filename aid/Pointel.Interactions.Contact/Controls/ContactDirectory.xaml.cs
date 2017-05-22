using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Genesyslab.Platform.Commons.Protocols;
using Genesyslab.Platform.Contacts.Protocols.ContactServer;
using Genesyslab.Platform.Contacts.Protocols.ContactServer.Events;
using Pointel.Configuration.Manager;
using Pointel.Interactions.Contact.Core.Common;
using Pointel.Interactions.Contact.Forms;
using Pointel.Interactions.Contact.Helpers;
using Pointel.Interactions.Contact.Settings;
using Pointel.Interactions.IPlugins;
using System.Threading;
namespace Pointel.Interactions.Contact.Controls
{
    public class ContactInfoEventArgs
    {
        public ContactInfoEventArgs(string contactId)
        {
            this.ContactId = contactId;
        }
        public string ContactId
        {
            get;
            private set;
        }
        public string Description
        {
            get;
            internal set;
        }
        public string Reason
        {
            get;
            internal set;
        }
    }

    public delegate void ContactDirectoryHandler(ContactInfoEventArgs e);

    /// <summary>
    /// Interaction logic for ContactDirectory.xaml
    /// </summary>
    public partial class ContactDirectory : UserControl, IDisposable
    {
        private Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
           "AID");
        private AdvanceSearch _advanceSearch = null;
        private DataTable ContactMainTable = new DataTable();
        string emailAdress = string.Empty;
        string contactId = string.Empty;
        private string interactionID = string.Empty;
        ContactData contactDataContext = new ContactData();
        private int start = 0;
        private int end = 0;
        private int itemPerPage = 0;
        private int currentPage = 0;
        private int totalPage = 0;
        private int totalItems = 0;
        private bool isShowContactInfo = true;
        private bool enableOkCancelButton;
        private bool contactLoaded = false;
        private bool isMergeContactView = false;
        private DispatcherTimer _timerforcloseError = null;
        private List<string> searchAttribute = null;
        private Func<string, string, string, bool> NotifySelectedEmail = null;

        public event ContactDirectoryHandler ContactSelectedEvent;

        public ContactDirectory(bool enableOkCancelButton, string interactionID, Func<string, string, string, bool> notifySelectedEmailId, string toAddress = null, string ccAddress = null, string bccAddress = null)
        {
            InitializeComponent();

            this.enableOkCancelButton = enableOkCancelButton;
            this.NotifySelectedEmail = notifySelectedEmailId;

            if (!enableOkCancelButton)
            {
                this.interactionID = interactionID;
            }
            else
            {
                dgContactDirectory.SelectionMode = Microsoft.Windows.Controls.DataGridSelectionMode.Single;
                txtTo.Text = string.IsNullOrEmpty(toAddress) ? "" : toAddress.Replace(",", ";");
                txtCc.Text = string.IsNullOrEmpty(ccAddress) ? "" : ccAddress.Replace(",", ";");
                txtBcc.Text = string.IsNullOrEmpty(bccAddress) ? "" : bccAddress.Replace(",", ";");
            }

            ContactHandler.EmailStateNotificationEvent += new EmailStateNotifier(ContactHandler_EmailStateNotificationEvent);

        }

        void ContactHandler_EmailStateNotificationEvent()
        {
            GetSelectedContacts();
        }

        public ContactDirectory(bool isMergeContact = true)
        {
            InitializeComponent();

            btnOK.Content = "Merge";
            btnShowContactInfo.Visibility = btnAddContact.Visibility = Visibility.Collapsed;
            enableOkCancelButton = isMergeContactView = isMergeContact;
        }

        private void HideContactInformation()
        {
            grdColn_Seperator.Width = new GridLength(0);
            grdColn_Information.Width = new GridLength(0);
            contactInfo.Visibility = Visibility.Collapsed;
            dgContactDirectory.Width = ContactDirectoryView.Width - 15;
        }

        private void ShowContactInformation()
        {
            contactInfo.Visibility = Visibility.Visible;
            dgContactDirectory.Width = ContactDirectoryView.Width - 15;
            grdColn_Information.Width = new GridLength(1, GridUnitType.Star);
            grdColn_Seperator.Width = new GridLength(3);
        }

        private void LoadContact(string contactID)
        {
            try
            {
                contactInfo.Children.Clear();
                if (!string.IsNullOrEmpty(contactID))
                {
                    UserControl contactInfoControl = new Controls.ContactInformation(contactID, "all");
                    contactInfoControl.Margin = new Thickness(2);
                    contactInfoControl.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                    contactInfo.Children.Add(contactInfoControl);
                }
                else
                {
                    TextBlock tbMessage = new TextBlock();
                    tbMessage.Text = "Select contact to view information";
                    tbMessage.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                    tbMessage.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                    //btnMergeContact.Visibility =
                    btnUnMergeContact.Visibility = btnMergeContact.Visibility = btnDeleteContact.Visibility = Visibility.Collapsed;
                    contactInfo.Children.Add(tbMessage);
                }
            }
            catch (Exception ex)
            {
                logger.Error("LoadContact() : " + ex.Message);
            }
        }

        void ContactHandler_ContactUpdateNotificationHandler(string contactId, List<Pointel.Interactions.Contact.Helpers.Attribute> contactRow)
        {
            try
            {
                int selectedIndex = dgContactDirectory.SelectedIndex;
                if (string.IsNullOrEmpty(contactId)) return;
                DataRow[] selectRow = ContactMainTable.Select("ContactId='" + contactId + "'");
                if (selectRow.Length > 0)
                {
                    int mainIndex = ContactMainTable.Rows.IndexOf(selectRow[0]);
                    foreach (DataColumn col in ContactMainTable.Columns)
                        ContactMainTable.Rows[mainIndex][col.ColumnName] = "";
                    foreach (Pointel.Interactions.Contact.Helpers.Attribute attribute in contactRow)
                    {

                        if (ContactMainTable.Columns.Contains(attribute.AttributeName))
                        {
                            if ((attribute.AttributeName.Equals(Settings.ContactDataContext.GetInstance().ContactValidAttribute["PhoneNumber"]) || attribute.AttributeName.Equals(Settings.ContactDataContext.GetInstance().ContactValidAttribute["EmailAddress"])))
                            //&& string.IsNullOrEmpty(dr[Settings.ContactDataContext.GetInstance().ContactValidAttribute[key]].ToString()))
                            {
                                if (attribute.Isprimary)
                                {
                                    if (string.IsNullOrEmpty(ContactMainTable.Rows[mainIndex][attribute.AttributeName].ToString()))
                                        ContactMainTable.Rows[mainIndex][attribute.AttributeName] = attribute.AttributeValue;
                                    else
                                        ContactMainTable.Rows[mainIndex][attribute.AttributeName] = attribute.AttributeValue + "," + ContactMainTable.Rows[mainIndex][attribute.AttributeName];
                                }
                                else
                                {
                                    if (string.IsNullOrEmpty(ContactMainTable.Rows[mainIndex][attribute.AttributeName].ToString()))
                                        ContactMainTable.Rows[mainIndex][attribute.AttributeName] = attribute.AttributeValue;
                                    else
                                        ContactMainTable.Rows[mainIndex][attribute.AttributeName] += "," + attribute.AttributeValue;
                                }

                            }
                            else
                                ContactMainTable.Rows[mainIndex][attribute.AttributeName] = attribute.AttributeValue;
                        }
                    }
                    ContactMainTable.Rows[mainIndex]["ContactId"] = contactId;
                    ContactMainTable.Rows[mainIndex]["EmailVisibility"] = "Hidden";
                    ContactMainTable.Rows[mainIndex]["CallVisibility"] = "Hidden";
                    bool isEmailEnable = (ConfigContainer.Instance().AllKeys.Contains("email.enable.plugin") && "true".Equals(((string)ConfigContainer.Instance().GetValue("email.enable.plugin")).ToLower())
                 && Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections.ContainsKey(Pointel.Interactions.IPlugins.Plugins.Email));
                    if (Settings.ContactDataContext.GetInstance().ContactValidAttribute.ContainsKey("EmailAddress")
                   && !string.IsNullOrEmpty(ContactMainTable.Rows[mainIndex][Settings.ContactDataContext.GetInstance().ContactValidAttribute["EmailAddress"]].ToString()))
                    {
                        ContactMainTable.Rows[mainIndex]["EmailToDisplay"] = ContactMainTable.Rows[mainIndex][Settings.ContactDataContext.GetInstance().ContactValidAttribute["EmailAddress"]].ToString().Split(',')[0];
                        if (isEmailEnable && !enableOkCancelButton && ContactDataContext.GetInstance().IsInteractionServerActive)
                            ContactMainTable.Rows[mainIndex]["EmailVisibility"] = "Visible";
                    }
                    if (Settings.ContactDataContext.GetInstance().ContactValidAttribute.ContainsKey("PhoneNumber")
                        && !string.IsNullOrEmpty(ContactMainTable.Rows[mainIndex][Settings.ContactDataContext.GetInstance().ContactValidAttribute["PhoneNumber"]].ToString()))
                    {
                        ContactMainTable.Rows[mainIndex]["PhoneNoToDisplay"] = ContactMainTable.Rows[mainIndex][Settings.ContactDataContext.GetInstance().ContactValidAttribute["PhoneNumber"]].ToString().Split(',')[0];
                        if (!enableOkCancelButton && ContactDataContext.GetInstance().IsTServerActive)
                            ContactMainTable.Rows[mainIndex]["CallVisibility"] = "Visible";
                    }

                    if (!enableOkCancelButton && btnShowContactInfo.Visibility == Visibility.Visible)
                        LoadContact(contactId);
                    GetSelectedContacts();
                    if (selectedIndex >= 0)
                        dgContactDirectory.SelectedIndex = selectedIndex;
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred as " + ex.Message);
            }
        }

        private string contactInsertEvent(string contactId, List<Pointel.Interactions.Contact.Helpers.Attribute> contactRow)
        {
            try
            {
                this.contactId = contactId;
                if (contactRow != null)
                    if (contactRow.Count > 0)
                    {
                        DataRow mainTableRow = ContactMainTable.NewRow();
                        foreach (DataColumn col in ContactMainTable.Columns)
                            mainTableRow[col.ColumnName] = "";
                        foreach (Pointel.Interactions.Contact.Helpers.Attribute attribute in contactRow)
                        {
                            if (ContactMainTable.Columns.Contains(attribute.AttributeName))
                            {
                                if ((attribute.AttributeName.Equals(Settings.ContactDataContext.GetInstance().ContactValidAttribute["PhoneNumber"]) || attribute.AttributeName.Equals(Settings.ContactDataContext.GetInstance().ContactValidAttribute["EmailAddress"])))
                                //&& string.IsNullOrEmpty(dr[Settings.ContactDataContext.GetInstance().ContactValidAttribute[key]].ToString()))
                                {
                                    if (string.IsNullOrEmpty(mainTableRow[attribute.AttributeName].ToString()))
                                        mainTableRow[attribute.AttributeName] = attribute.AttributeValue;
                                    else
                                        mainTableRow[attribute.AttributeName] += "," + attribute.AttributeValue;
                                }
                                else
                                    mainTableRow[attribute.AttributeName] = attribute.AttributeValue;
                            }
                        }
                        mainTableRow["ContactId"] = contactId;
                        mainTableRow["EmailVisibility"] = "Hidden";
                        mainTableRow["CallVisibility"] = "Hidden";
                        bool isEmailEnable = (ConfigContainer.Instance().AllKeys.Contains("email.enable.plugin") && "true".Equals(((string)ConfigContainer.Instance().GetValue("email.enable.plugin")).ToLower())
                 && Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections.ContainsKey(Pointel.Interactions.IPlugins.Plugins.Email));
                        if (Settings.ContactDataContext.GetInstance().ContactValidAttribute.ContainsKey("EmailAddress")
                       && !string.IsNullOrEmpty(mainTableRow[Settings.ContactDataContext.GetInstance().ContactValidAttribute["EmailAddress"]].ToString()))
                        {
                            mainTableRow["EmailToDisplay"] = mainTableRow[Settings.ContactDataContext.GetInstance().ContactValidAttribute["EmailAddress"]].ToString().Split(',')[0];
                            if (isEmailEnable && !enableOkCancelButton && ContactDataContext.GetInstance().IsInteractionServerActive)
                                mainTableRow["EmailVisibility"] = "Visible";
                        }
                        if (Settings.ContactDataContext.GetInstance().ContactValidAttribute.ContainsKey("PhoneNumber")
                            && !string.IsNullOrEmpty(mainTableRow[Settings.ContactDataContext.GetInstance().ContactValidAttribute["PhoneNumber"]].ToString()))
                        {
                            mainTableRow["PhoneNoToDisplay"] = mainTableRow[Settings.ContactDataContext.GetInstance().ContactValidAttribute["PhoneNumber"]].ToString().Split(',')[0];
                            if (!enableOkCancelButton && ContactDataContext.GetInstance().IsTServerActive)
                                mainTableRow["CallVisibility"] = "Visible";
                        }

                        ContactMainTable.Rows.InsertAt(mainTableRow, start);
                        GetSelectedContacts();
                        totalItems++;
                        totalPage = (totalItems / itemPerPage) + (((totalItems % itemPerPage) > 0) ? 1 : 0);
                        BindContacts();
                        dgContactDirectory.SelectedItem = dgContactDirectory.Items[0];
                        if (!enableOkCancelButton && btnShowContactInfo.Visibility == Visibility.Visible)
                            LoadContact(contactId);
                        dgContactDirectory.SelectedIndex = 0;

                    }
            }
            catch (Exception ex)
            {
                logger.Error("contactInsertEvent() : " + ex.Message);
            }
            return null;
        }

        private void InitiateContactTable()
        {
            ContactMainTable.Columns.Clear();
            searchAttribute = new List<string>();
            //List<string> listDirectoryDisplayColumn = ((string)ConfigContainer.Instance().GetValue("contact.directory-displayed-columns")).Split(',').ToList();
            foreach (string key in new string[] { "LastName", "FirstName", "PhoneNumber", "EmailAddress" })
            {
                if (Settings.ContactDataContext.GetInstance().ContactValidAttribute.ContainsKey(key))
                {
                    ContactMainTable.Columns.Add(Settings.ContactDataContext.GetInstance().ContactValidAttribute[key], typeof(string));
                    searchAttribute.Add(key);
                }
            }
            ContactMainTable.Columns.Add("ContactId", typeof(string));
            ContactMainTable.Columns.Add("EmailVisibility");
            ContactMainTable.Columns.Add("CallVisibility");
            ContactMainTable.Columns.Add("PhoneNoToDisplay");
            ContactMainTable.Columns.Add("EmailToDisplay");
            ContactMainTable.Columns.Add("MergeId");
            //
        }

        private void GetMatchedContact()
        {
            try
            {
                string query = "TenantId:" + ConfigContainer.Instance().TenantDbId;
                //List<string> listDirectoryDisplayColumn = (ConfigContainer.Instance().AllKeys.Contains("contact.directory-displayed-columns") ?
                //                                           ((string)ConfigContainer.Instance().GetValue("contact.directory-displayed-columns")) : "").Split(',').ToList();
                string subQuery = "";
                foreach (string key in new string[] { "LastName", "FirstName", "PhoneNumber", "EmailAddress" })
                {
                    if (!Settings.ContactDataContext.GetInstance().ContactValidAttribute.ContainsKey(key)) continue;
                    // subQuery += (!string.IsNullOrEmpty(subQuery) ? " OR ":"")+ key + ":" + (isAll == true ? "**" : txtSearch.Text + "*");
                    subQuery += (!string.IsNullOrEmpty(subQuery) ? " OR " : "") + key + ":" + txtSearch.Text + "*";
                }
                if (!string.IsNullOrEmpty(subQuery))
                    query += " AND (" + subQuery + ")";
                totalItems = totalPage = currentPage = start = end = 0;
                GetContactList(query);
                BindContacts();

            }
            catch (Exception ex)
            {
                logger.Error("Error occurred as " + ex.Message);
            }
        }

        /*
         * This Method used to retrieve the contact from the UCS.
         * It will retrieve the contact through search if the indexing is enabled.
         * Otherwise it will retrieve the contact through GetContactListMethod.
         */
        private void GetContactList(string query)
        {
            logger.Info("Contact search query:" + query);
            ContactHandler contactHandler = new ContactHandler();
            //For Get Contact using RequestSearch.
            if (ContactDataContext.GetInstance().IsContactIndexFound)
            {
                IMessage result = contactHandler.SearchContact(query, int.MaxValue);
                if (result != null)
                {
                    EventSearch eventSearch = result as EventSearch;
                    totalItems = eventSearch.FoundDocuments.Value;
                    if (totalItems > 0)
                    {
                        start = 0;
                        end = int.Parse(cmbItemsPerPage.Text);
                        if (totalItems < end)
                            end = totalItems;
                        totalPage = (totalItems / end) + (((totalItems % end) > 0) ? 1 : 0);
                    }
                    ConvertContacts(ref eventSearch);
                }
                else
                {
                    logger.Warn("Error occurred while reading contact using RequestSearch.");
                    ContactDataContext.GetInstance().IsContactIndexFound = false;
                    SearchContactUsingGetContactList(null);
                }

            }
            ////For Get Contact using RequestContactList.
            else
            {
                SearchContactUsingGetContactList(null);
            }


        }

        private void SearchContactUsingGetContactList(List<Criteria> searchCriteria, bool isMatchAllCondition = false)
        {
            ContactHandler contactHandler = new ContactHandler();
            SearchCriteriaCollection SearchCriteria = new SearchCriteriaCollection();
            StringList attributeList = new StringList();
            if (searchCriteria == null)
            {
                string attributeValue = "*".Equals(txtSearch.Text) ? "" : txtSearch.Text;

                for (int index = 0; index < searchAttribute.Count; index++)
                {
                    attributeList.Add(searchAttribute[index]);
                    ComplexSearchCriteria complexSearchCriteria = new ComplexSearchCriteria() { Prefix = Prefixes.Or };
                    complexSearchCriteria.Criterias = new SearchCriteriaCollection();
                    SimpleSearchCriteria searchCritiria1 = new SimpleSearchCriteria() { AttrName = searchAttribute[index], AttrValue = attributeValue + "*", Operator = new NullableOperators(Operators.Like) };
                    complexSearchCriteria.Criterias.Add(searchCritiria1);
                    SearchCriteria.Add(complexSearchCriteria);
                }
            }
            else
            {
                if (searchCriteria.Where(x => !string.IsNullOrEmpty(x.Value)).ToList().Count == 0)
                {
                    txtMessage.Text = "Please fill some search criteria and then try.";
                    starttimerforerror();
                    return;
                }
                bool isComplexSearch = searchCriteria.Where(x => !string.IsNullOrEmpty(x.Value)).ToList().Count > 1;
                for (int index = 0; index < searchCriteria.Count; index++)
                {
                    if (string.IsNullOrEmpty(searchCriteria[index].Value)) continue;
                    if (ContactDataContext.GetInstance().ContactValidAttribute.Where(x => x.Value == searchCriteria[index].Field).ToList().Count > 0)
                    {
                        if (isComplexSearch)
                        {
                            ComplexSearchCriteria complexSearchCriteria = null;
                            if (isMatchAllCondition)
                                complexSearchCriteria = new ComplexSearchCriteria() { Prefix = Prefixes.And };
                            else
                                complexSearchCriteria = new ComplexSearchCriteria() { Prefix = Prefixes.Or };
                            complexSearchCriteria.Criterias = new SearchCriteriaCollection();
                            SimpleSearchCriteria searchCritiria1 = new SimpleSearchCriteria()
                            {
                                AttrName = ContactDataContext.GetInstance().ContactValidAttribute.Where(x => x.Value == searchCriteria[index].Field).SingleOrDefault().Key
                                ,
                                AttrValue = searchCriteria[index].Value + "*",
                                Operator = new NullableOperators(Operators.Like)
                            };
                            complexSearchCriteria.Criterias.Add(searchCritiria1);
                            SearchCriteria.Add(complexSearchCriteria);
                        }
                        else
                        {
                            SimpleSearchCriteria searchCritiria1 = new SimpleSearchCriteria()
                            {
                                AttrName = ContactDataContext.GetInstance().ContactValidAttribute.Where(x => x.Value == searchCriteria[index].Field).SingleOrDefault().Key
                                ,
                                AttrValue = searchCriteria[index].Value + "*",
                                Operator = new NullableOperators(Operators.Like)
                            };
                            SearchCriteria.Add(searchCritiria1);

                        }

                    }
                }
            }
            attributeList.Add("MergeId");
            IMessage result = contactHandler.GetContactList(ConfigContainer.Instance().TenantDbId, attributeList, SearchCriteria); //txtSearch.Text, "OR", false);
            if (result != null)
            {
                EventContactListGet contactList = result as EventContactListGet;
                if (contactList != null && contactList.ContactData != null)
                {
                    if (contactList.TotalCount.Value > 100)
                        totalItems = contactList.TotalCount.Value;
                    else
                        totalItems = contactList.ContactData.Count;
                }

                if (totalItems > 0)
                {
                    start = 0;
                    end = int.Parse(cmbItemsPerPage.Text);
                    if (totalItems < end)
                        end = totalItems;
                    totalPage = (totalItems / end) + (((totalItems % end) > 0) ? 1 : 0);
                }
                ConvertContacts(ref contactList);
            }
            else
                logger.Warn("Error occurred while reading contact.");
        }

        private void ConvertContacts(ref EventSearch eventSearch)
        {
            ContactMainTable.Rows.Clear();
            logger.Info("Try to convert the contact.");
            // List<string> listDirectoryDisplayColumn = ((string)ConfigContainer.Instance().GetValue("contact.directory-displayed-columns")).Split(',').ToList();
            List<string> listDirectoryDisplayColumn = new List<string>(new string[] { "LastName", "FirstName", "PhoneNumber", "EmailAddress" });
            if (eventSearch.Documents != null && eventSearch.Documents.Count > 0)
            {
                logger.Info("total no of contact received: " + eventSearch.Documents.Count);
                for (int index = 0; index < eventSearch.Documents.Count && index < 100; index++)
                {
                    logger.Info(eventSearch.Documents[index].ToString());
                    DataRow dr = ContactMainTable.NewRow();
                    foreach (string key in eventSearch.Documents[index].Fields.AllKeys.Distinct())
                    {
                        string attrValue = "";
                        if (Settings.ContactDataContext.GetInstance().ContactValidAttribute.ContainsKey(key) && listDirectoryDisplayColumn.Contains(key))
                        {
                            if ((key.Equals("PhoneNumber") || key.Equals("EmailAddress")) && string.IsNullOrEmpty(dr[Settings.ContactDataContext.GetInstance().ContactValidAttribute[key]].ToString()))
                            {
                                object[] values = eventSearch.Documents[index].Fields.GetValues(key);
                                foreach (object value in values)
                                {
                                    attrValue = attrValue + (string.IsNullOrEmpty(attrValue) ? "" : ",") + value.ToString();
                                }
                            }
                            else
                                attrValue = eventSearch.Documents[index].Fields[key].ToString();
                            dr[Settings.ContactDataContext.GetInstance().ContactValidAttribute[key]] = attrValue;
                        }
                        else if (key.Equals("MergeId"))
                        {
                            dr["MergeId"] = eventSearch.Documents[index].Fields[key].ToString();
                        }
                    }
                    dr["ContactId"] = eventSearch.Documents[index].Fields["Id"].ToString();
                    dr["EmailVisibility"] = "Hidden";
                    dr["CallVisibility"] = "Hidden";
                    bool isEmailEnable = (ConfigContainer.Instance().AllKeys.Contains("email.enable.plugin") && "true".Equals(((string)ConfigContainer.Instance().GetValue("email.enable.plugin")).ToLower())
                   && Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections.ContainsKey(Pointel.Interactions.IPlugins.Plugins.Email));
                    if (Settings.ContactDataContext.GetInstance().ContactValidAttribute.ContainsKey("EmailAddress")
                        && !string.IsNullOrEmpty(dr[Settings.ContactDataContext.GetInstance().ContactValidAttribute["EmailAddress"]].ToString()))
                    {
                        dr["EmailToDisplay"] = dr[Settings.ContactDataContext.GetInstance().ContactValidAttribute["EmailAddress"]].ToString().Split(',')[0];
                        if (isEmailEnable && !enableOkCancelButton && ContactDataContext.GetInstance().IsInteractionServerActive)
                            dr["EmailVisibility"] = "Visible";
                    }
                    if (Settings.ContactDataContext.GetInstance().ContactValidAttribute.ContainsKey("PhoneNumber")
                        && !string.IsNullOrEmpty(dr[Settings.ContactDataContext.GetInstance().ContactValidAttribute["PhoneNumber"]].ToString()))
                    {
                        dr["PhoneNoToDisplay"] = dr[Settings.ContactDataContext.GetInstance().ContactValidAttribute["PhoneNumber"]].ToString().Split(',')[0];
                        if (!enableOkCancelButton && ContactDataContext.GetInstance().IsTServerActive)
                            dr["CallVisibility"] = "Visible";
                    }
                    ContactMainTable.Rows.Add(dr);

                }
            }
        }

        private void ConvertContacts(ref EventContactListGet contactList)
        {
            ContactMainTable.Rows.Clear();
            logger.Info("Try to convert the contact.");
            //  List<string> listDirectoryDisplayColumn = ((string)ConfigContainer.Instance().GetValue("contact.directory-displayed-columns")).Split(',').ToList();
            List<string> listDirectoryDisplayColumn = new List<string>(new string[] { "LastName", "FirstName", "PhoneNumber", "EmailAddress" });
            if (contactList.ContactData != null && contactList.ContactData.Count > 0)
            {
                logger.Info("total no of contact received: " + contactList.ContactData.Count);
                //foreach (Genesyslab.Platform.Contacts.Protocols.ContactServer.Contact contact in contactList.ContactData)
                for (int index = 0; index < contactList.ContactData.Count && index < 100; index++)
                {
                    // logger.Info(contact.ToString());

                    DataRow dr = ContactMainTable.NewRow();
                    foreach (Genesyslab.Platform.Contacts.Protocols.ContactServer.Attribute attribute in contactList.ContactData[index].ContactAttributesList)
                    {
                        string attrValue = "";
                        if (Settings.ContactDataContext.GetInstance().ContactValidAttribute.ContainsKey(attribute.AttributeName) && listDirectoryDisplayColumn.Contains(attribute.AttributeName))
                        {
                            if ((attribute.AttributeName.Equals("PhoneNumber") || attribute.AttributeName.Equals("EmailAddress")) && string.IsNullOrEmpty(dr[Settings.ContactDataContext.GetInstance().ContactValidAttribute[attribute.AttributeName]].ToString()))
                            {
                                List<Genesyslab.Platform.Contacts.Protocols.ContactServer.Attribute> attributeGroup =
                                    contactList.ContactData[index].ContactAttributesList.Cast<Genesyslab.Platform.Contacts.Protocols.ContactServer.Attribute>().
                                    Where(x => x.AttributeName == attribute.AttributeName).ToList();

                                foreach (Genesyslab.Platform.Contacts.Protocols.ContactServer.Attribute attr in attributeGroup)
                                {
                                    attrValue = attrValue + (string.IsNullOrEmpty(attrValue) ? "" : ",") + attr.AttributeValue.ToString();
                                }
                            }
                            else
                                attrValue = attribute.AttributeValue.ToString();
                            dr[Settings.ContactDataContext.GetInstance().ContactValidAttribute[attribute.AttributeName]] = attrValue;
                        }
                        else if (attribute.AttributeName.Equals("MergeId"))
                        {
                            dr["MergeId"] = attribute.AttributeValue.ToString();
                        }
                    }
                    dr["ContactId"] = contactList.ContactData[index].Id;
                    dr["EmailVisibility"] = "Hidden";
                    dr["CallVisibility"] = "Hidden";
                    bool isEmailEnable = (ConfigContainer.Instance().AllKeys.Contains("email.enable.plugin") && "true".Equals(((string)ConfigContainer.Instance().GetValue("email.enable.plugin")).ToLower())
                   && Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections.ContainsKey(Pointel.Interactions.IPlugins.Plugins.Email));
                    if (Settings.ContactDataContext.GetInstance().ContactValidAttribute.ContainsKey("EmailAddress")
                        && !string.IsNullOrEmpty(dr[Settings.ContactDataContext.GetInstance().ContactValidAttribute["EmailAddress"]].ToString()))
                    {
                        dr["EmailToDisplay"] = dr[Settings.ContactDataContext.GetInstance().ContactValidAttribute["EmailAddress"]].ToString().Split(',')[0];
                        if (isEmailEnable && !enableOkCancelButton && ContactDataContext.GetInstance().IsInteractionServerActive)
                            dr["EmailVisibility"] = "Visible";
                    }
                    if (Settings.ContactDataContext.GetInstance().ContactValidAttribute.ContainsKey("PhoneNumber")
                        && !string.IsNullOrEmpty(dr[Settings.ContactDataContext.GetInstance().ContactValidAttribute["PhoneNumber"]].ToString()))
                    {
                        dr["PhoneNoToDisplay"] = dr[Settings.ContactDataContext.GetInstance().ContactValidAttribute["PhoneNumber"]].ToString().Split(',')[0];
                        if (!enableOkCancelButton && ContactDataContext.GetInstance().IsTServerActive)
                            dr["CallVisibility"] = "Visible";
                    }
                    ContactMainTable.Rows.Add(dr);

                }
            }
        }

        private void BindContacts()
        {
            logger.Debug("Try to bind contact in grid.");
            GetSelectedContacts();
            //btnMergeContact.Visibility =
            btnUnMergeContact.Visibility = btnMergeContact.Visibility = btnDeleteContact.Visibility = Visibility.Collapsed;
            ShowCurrentPageIndex();
            if (contactId != null)
            {
                dgContactDirectory.SelectedItem = null;
                LoadContact(null);
            }
            if (contactDataContext.Contacts != null && contactDataContext.Contacts.Rows.Count > 0)
            {
                txtError.Visibility = Visibility.Collapsed;
                bdrPaging.IsEnabled = true;
            }
            else
            {
                txtError.Visibility = Visibility.Visible;
                if (!isShowContactInfo)
                {
                    btnShowContactInfo_Click(null, null);
                }
                else
                {
                    HideContactInformation();
                }
                bdrPaging.IsEnabled = false;
            }
            logger.Info("bind contact in gridb success.");

        }

        private void GetSelectedContacts()
        {
            logger.Debug("Start Rage:" + start + ", End range:" + end + ", TotalContacts" + totalItems);
            if (start == end && ContactMainTable.Rows.Count > 0)
            {
                if (ContactMainTable.Rows.Count > itemPerPage)
                {
                    start = ContactMainTable.Rows.Count - itemPerPage;
                }
                else
                {
                    start = 0;
                }
            }
            else if (end > ContactMainTable.Rows.Count)
            {
                totalItems = end = ContactMainTable.Rows.Count;
            }

            contactDataContext.Contacts = ContactMainTable.Clone();
            contactDataContext.Contacts.Clear();
            if (ContactMainTable.Columns != null && ContactMainTable.Columns.Count > 0)
            {
                contactDataContext.Contacts.Columns["EmailVisibility"].DefaultValue = contactDataContext.Contacts.Columns["CallVisibility"].DefaultValue = "Hidden";
                bool isEmailEnable = (ContactDataContext.GetInstance().IsEmailLogon && ConfigContainer.Instance().AllKeys.Contains("email.enable.plugin") && "true".Equals(((string)ConfigContainer.Instance().GetValue("email.enable.plugin")).ToLower())
                    && Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections.ContainsKey(Pointel.Interactions.IPlugins.Plugins.Email));
                for (int index = start; index < end; index++)
                {
                    DataRow row = contactDataContext.Contacts.NewRow();
                    for (int columnIndex = 0; ContactMainTable.Columns.Count > columnIndex; columnIndex++)
                        if (ContactMainTable.Columns[columnIndex].ColumnName == "EmailVisibility" && !isEmailEnable)
                            continue;
                        else
                            row[ContactMainTable.Columns[columnIndex].ColumnName] = ContactMainTable.Rows[index][ContactMainTable.Columns[columnIndex].ColumnName];
                    contactDataContext.Contacts.Rows.Add(row);
                }
            }
            logger.Info("Contact selected successfully");
        }

        private bool loadedEntered = false;

        private void LoadAndSetPaging()
        {
            List<int> lstPaging = new List<int>();
            int temp = 0;
            try
            {
                if (ConfigContainer.Instance().AllKeys.Contains("contact.available-directory-page-sizes") &&
                    !string.IsNullOrEmpty(ConfigContainer.Instance().GetAsString("contact.available-directory-page-sizes").Trim()))
                {
                    foreach (var item in ConfigContainer.Instance().GetAsString("contact.available-directory-page-sizes").Split(',').ToList())
                    {
                        if (int.TryParse(item, out temp) && !lstPaging.Contains(temp))
                            lstPaging.Add(temp);
                    }
                }
            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred as " + generalException.Message);
                logger.Trace("Error Strace: " + generalException.StackTrace);
            }

            if (lstPaging.Count == 0)
                lstPaging = new List<int>(new int[] { 5, 10, 25, 50 });

            string defaultSelection = null;

            if (ConfigContainer.Instance().AllKeys.Contains("contact.default-directory-page-size"))
                defaultSelection = ConfigContainer.Instance().GetAsString("contact.default-directory-page-size");

            int defaultPageSize;
            if (!int.TryParse(defaultSelection, out defaultPageSize))
                defaultPageSize = 10;



            if (!lstPaging.Contains(defaultPageSize))
                lstPaging.Add(defaultPageSize);


            lstPaging = lstPaging.Distinct().ToList();
            lstPaging.Sort();

            cmbItemsPerPage.ItemsSource = lstPaging;
            cmbItemsPerPage.SelectedItem = defaultPageSize;
        }

        private void SubscriptNotification()
        {
            ContactHandler.ContactUpdateNotificationHandler -= ContactHandler_ContactUpdateNotificationHandler;
            ContactHandler.InteractionServerNotificationHandler -= ContactHandler_InteractionServerNotificationHandler;
            ContactHandler.TServerNotificationHandler -= ContactHandler_TServerNotificationHandler;
            ContactHandler.ContactUpdateNotificationHandler += new ContactUpdateNotification(ContactHandler_ContactUpdateNotificationHandler);
            ContactHandler.InteractionServerNotificationHandler += new InteractionServerNotificationHandler(ContactHandler_InteractionServerNotificationHandler);
            ContactHandler.TServerNotificationHandler += new TServerNotificationHandler(ContactHandler_TServerNotificationHandler);
        }

        private void DetermineEmailSelectionState()
        {
            if (!enableOkCancelButton)
            {
                HideAddresses.Height = new GridLength(0);
            }
            else
            {
                HideAddresses.Height = GridLength.Auto;
                DetailsGrid.Height = GridLength.Auto;
                if (isMergeContactView)
                    gridEmailAddress.Visibility = Visibility.Collapsed;
                else
                    gridMergeContact.Visibility = Visibility.Collapsed;

                // Added by sakthi to resolve the multi selection problem in merg contact view. 
                // Date : 03-03-2016
                // :Start
                dgContactDirectory.SelectionMode = Microsoft.Windows.Controls.DataGridSelectionMode.Single;

                // : End

                //btnMergeContact.Visibility = 
                btnUnMergeContact.Visibility = btnMergeContact.Visibility = btnShowContactInfo.Visibility = btnAddContact.Visibility = btnDeleteContact.Visibility = Visibility.Collapsed;
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Thread contactThread = new Thread(delegate()
            {
                Dispatcher.BeginInvoke(
                new Action(delegate()
                {
                    try
                    {
                        SubscriptNotification();
                        loadedEntered = true;
                        DataContext = contactDataContext;
                        ContactHandler.ContactServerNotificationHandler += new ContactServerNotificationHandler(ContactServerNotification);

                        if (ConfigContainer.Instance().AllKeys.Contains("contact.default-directory-page-size"))
                        {
                            string pageSize = ConfigContainer.Instance().GetAsString("contact.default-directory-page-size");
                            if (!string.IsNullOrEmpty(pageSize))
                                int.TryParse(pageSize, out itemPerPage);
                        }

                        //if (!string.IsNullOrEmpty(ConfigContainer.Instance().AllKeys.Contains("contact.default-directory-page-size") ?
                        //    (string)ConfigContainer.Instance().GetValue("contact.default-directory-page-size") : "5"))
                        //    itemPerPage = end = int.Parse(ConfigContainer.Instance().AllKeys.Contains("contact.default-directory-page-size") ?
                        //        (string)ConfigContainer.Instance().GetValue("contact.default-directory-page-size") : "5");
                        LoadAndSetPaging();
                        CreateWorkbinFieldsContextMenuItem("First Name", "Last Name", "Phone Number", "E-mail Address");
                        if (ContactDataContext.GetInstance().IsContactServerActive)
                        {
                            CloseError(null, null);
                            InitiateContactTable();
                            GetMatchedContact();
                            BindContacts();
                            DetermineEmailSelectionState();
                            HideContactInformation();
                            this.KeyDown += new KeyEventHandler(ContactDirectory_KeyDown);
                            contactLoaded = true;
                        }
                        else
                        {
                            contactLoaded = false;
                            ShoWMessage();
                        }
                    }
                    catch (Exception exception)
                    {
                        logger.Error("Error occurred while load contact directory as " + exception.ToString());
                    }
                    HideLoading();
                }));
            });
            contactThread.Start();
        }

        void ContactDirectory_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.A && Keyboard.Modifiers == ModifierKeys.Alt)
                {
                    btnAddContact_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred as " + ex.Message);
            }
        }

        private void ShoWMessage()
        {
            grMessage.Height = GridLength.Auto;
            this.IsEnabled = false;
            btnCloseError.Visibility = Visibility.Collapsed;
        }

        private void HideMessage()
        {
            grMessage.Height = new GridLength(0);
            this.IsEnabled = true;
            btnCloseError.Visibility = Visibility.Visible;
        }

        //This method handle the T-Server functionality in Contact directory.
        //If the T-server disconnected mean, the Call button will hide. otherwise it will show.
        void ContactHandler_TServerNotificationHandler()
        {
            try
            {
                string value = "Hidden";
                if (ContactDataContext.GetInstance().IsTServerActive)
                    value = "Visible";
                if (contactDataContext.Contacts != null && contactDataContext.Contacts.Rows.Count > 0)
                    foreach (DataRow dr in contactDataContext.Contacts.Rows)
                    {
                        int mobileNoIndex = contactDataContext.Contacts.Columns.IndexOf(Settings.ContactDataContext.GetInstance().ContactValidAttribute["PhoneNumber"]);
                        if (mobileNoIndex >= 0 && !string.IsNullOrEmpty(dr[mobileNoIndex].ToString()))
                            dr["CallVisibility"] = value;
                    }
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred as " + ex.Message);
            }

        }

        //This method handle the interaction server functionality in Contact directory.
        //If the interaction server disconnected mean, the email button will hide. otherwise it will show.
        void ContactHandler_InteractionServerNotificationHandler()
        {
            try
            {
                string value = "Hidden";
                if (ContactDataContext.GetInstance().IsInteractionServerActive)
                    value = "Visible";
                if (contactDataContext.Contacts != null && contactDataContext.Contacts.Rows.Count > 0)
                    foreach (DataRow dr in contactDataContext.Contacts.Rows)
                    {
                        int emailIndex = contactDataContext.Contacts.Columns.IndexOf(Settings.ContactDataContext.GetInstance().ContactValidAttribute["EmailAddress"]);
                        if (emailIndex >= 0 && !string.IsNullOrEmpty(dr[emailIndex].ToString()))
                            dr["EmailVisibility"] = value;
                    }
            }
            catch (Exception ex)
            {
                logger.Error("Eorror occurred as " + ex.Message);
            }
        }

        private void ContactServerNotification()
        {
            try
            {
                if (ContactDataContext.GetInstance().IsContactServerActive)
                {
                    HideMessage();
                    if (!contactLoaded && loadedEntered)
                        UserControl_Loaded(null, null);
                    btnAdvanceSearchResponse_Click(null, null);
                }
                else
                {
                    ShoWMessage();
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred as " + ex.Message);
            }

        }

        /// <summary>
        /// Shows the index of the current page.
        /// </summary>
        private void ShowCurrentPageIndex()
        {
            try
            {
                if (totalItems > 0)
                {
                    txtStart.Text = (start + 1).ToString();
                    txtCurrentPage.Text = (currentPage + 1).ToString();
                }
                else
                {
                    txtCurrentPage.Text = txtStart.Text = "0";
                }

                txtTotalItems.Text = totalItems.ToString();
                txtTotalPage.Text = totalPage.ToString();
                txtEnd.Text = end.ToString();
                btnFirst.IsEnabled = btnPrevious.IsEnabled = string.Compare(txtCurrentPage.Text, "1") != 0;
                btnNext.IsEnabled = btnLastIndex.IsEnabled = string.Compare(txtCurrentPage.Text, txtTotalPage.Text) != 0;

            }
            catch (Exception generalException)
            {

                logger.Error("Error occurred as " + generalException.ToString());
            }
        }

        private void btnFirstIndex_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (totalItems > 0)
                {
                    totalPage = (totalItems / itemPerPage) + (((totalItems % itemPerPage) > 0) ? 1 : 0);
                    start = currentPage = 0;
                    end = ((start + itemPerPage) < totalItems) ? (start + itemPerPage) : totalItems;
                }
                else
                {
                    end = totalPage = start = currentPage = 0;
                }
                BindContacts();
            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred while move to first page  as " + generalException.ToString());
            }
        }

        private void btnPreviousIndex_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if ((start - itemPerPage) >= 0)
                {
                    start -= itemPerPage;
                    currentPage--;
                    if ((end % itemPerPage) != 0)
                    {
                        end -= (end % itemPerPage);
                    }
                    else
                    {
                        end -= itemPerPage;
                    }
                    BindContacts();
                }
            }
            catch (Exception generalException)
            {

                logger.Error("Error occurred while moving to previous page as " + generalException.ToString());
            }
        }

        private void btnNextPage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if ((start + itemPerPage) < totalItems)
                {
                    if ((start + itemPerPage) < 100)
                    {
                        start += itemPerPage;
                        currentPage++;
                        end += itemPerPage;
                        if (end > totalItems)
                            end = totalItems;
                        BindContacts();
                    }
                    else
                    {
                        txtMessage.Text = "The application cannot render more than 100 records out of " + totalItems + " matching records. Please redefine your query.";
                        starttimerforerror();
                        //start -= itemPerPage;
                        //currentPage--;
                    }

                }
            }
            catch (Exception generalException)
            {

                logger.Error("Error occurred while move to next page as  " + generalException.ToString());
            }
        }

        private void btnLastIndex_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if ((start + itemPerPage) < totalItems)
                {
                    currentPage = totalPage - 1;
                    start = (totalItems / itemPerPage - 1) * itemPerPage;
                    start += totalItems % itemPerPage == 0 ? 0 : itemPerPage;
                    end = totalItems;
                    BindContacts();
                }
            }
            catch (Exception generalException)
            {

                logger.Error("btnLastIndex_Click:Error occurred as " + generalException.ToString());
            }
        }

        private void cmbFrom_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            btnFirstIndex_Click(null, null);
        }

        private void dgContactDirectory_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (e.ChangedButton == MouseButton.Left)
                {
                    var selectedContact = dgContactDirectory.SelectedItem as DataRowView;
                    if (selectedContact != null)
                    {
                        DataRow row = selectedContact.Row;
                        if (row.ItemArray[3] != null)
                        {
                            emailAdress = row.ItemArray[3].ToString();
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                logger.Error("Error occurred while mouse down on contact grid as " + ex.Message);
            }
        }

        /// <summary>
        /// Handles the Click event of the btnTo control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void btnTo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(emailAdress))
                {
                    txtTo.Text = txtTo.Text.Replace(",", ";");
                    if (txtTo.Text.Length == 0)
                    {
                        txtTo.Text += emailAdress;
                    }
                    else if (!txtTo.Text.Contains(emailAdress))
                    {
                        if (txtTo.Text.Trim().EndsWith(";"))
                            txtTo.Text += emailAdress;
                        else
                            txtTo.Text += ";" + emailAdress;
                    }

                }


            }
            catch (Exception generalException)
            {

                logger.Error("btnTo_Click: Error occurred as  " + generalException.ToString());
            }
        }

        /// <summary>
        /// Handles the Click event of the btnCc control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void btnCc_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(emailAdress))
                {
                    txtCc.Text = txtCc.Text.Replace(",", ";");
                    if (txtCc.Text.Length == 0)
                    {
                        txtCc.Text += emailAdress;
                    }
                    else if (!txtCc.Text.Contains(emailAdress))
                    {
                        if (txtCc.Text.Trim().EndsWith(";"))
                            txtCc.Text += emailAdress;
                        else
                            txtCc.Text += ";" + emailAdress;
                    }
                }

            }
            catch (Exception generalException)
            {

                logger.Error("Error occurred as  " + generalException.ToString());
            }
        }

        /// <summary>
        /// Handles the Click event of the btnBcc control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void btnBcc_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(emailAdress))
                {
                    txtBcc.Text = txtBcc.Text.Replace(",", ";");
                    if (txtBcc.Text.Length == 0)
                    {
                        txtBcc.Text += emailAdress;
                    }
                    else if (!txtBcc.Text.Contains(emailAdress))
                    {
                        if (txtBcc.Text.Trim().EndsWith(";"))
                            txtBcc.Text += emailAdress;
                        else
                            txtBcc.Text += ";" + emailAdress;
                    }
                }
            }
            catch (Exception generalException)
            {

                logger.Error("btnBcc_Click: Error occurred as  " + generalException.ToString());
            }
        }

        /// <summary>
        /// Handles the Click event of the btnOK control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //For Notify E-mail address to E-mail Window.
                if ("Ok".Equals(btnOK.Content.ToString()))
                {
                    if ((!string.IsNullOrEmpty(txtTo.Text) && !string.IsNullOrWhiteSpace(txtTo.Text)) ||
                    (!string.IsNullOrEmpty(txtCc.Text) && !string.IsNullOrWhiteSpace(txtCc.Text)) ||
                    (!string.IsNullOrEmpty(txtBcc.Text) && !string.IsNullOrWhiteSpace(txtBcc.Text)))
                        if (NotifySelectedEmail != null)
                            NotifySelectedEmail(txtTo.Text, txtCc.Text, txtBcc.Text);
                }
                //Notify Selected Contact details to merge contact.
                else
                {
                    DataRowView item = dgContactDirectory.SelectedItem as DataRowView;
                    if (item != null)
                    {
                        DataRow row = item.Row;
                        if (row != null)
                        {
                            contactId = row.ItemArray[contactDataContext.Contacts.Columns.IndexOf("ContactId")].ToString();
                        }
                    }


                    if (string.IsNullOrEmpty(contactId))
                    {

                    }
                    else
                    {
                        ContactInfoEventArgs contactInfo = new ContactInfoEventArgs(contactId);
                        contactInfo.Reason = txtReason.Text;
                        contactInfo.Description = txtDescription.Text;
                        ContactSelectedEvent.Invoke(contactInfo);
                    }
                }

                CloseParentWindow(sender, true);
            }
            catch (Exception exception)
            {

                logger.Error("Error occurred as " + exception.ToString());
            }
            finally
            {
                txtTo.Text = string.Empty;
                txtCc.Text = string.Empty;
                txtBcc.Text = string.Empty;
            }
        }

        /// <summary>
        /// Handles the Click event of the btnCancel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CloseParentWindow(sender);
            }
            catch (Exception ex)
            {
                logger.Error("Error occrred as " + ex.Message);
            }

        }

        /// <summary>
        /// Closes the parent window.
        /// </summary>
        /// <param name="sender">The sender.</param>
        private void CloseParentWindow(object sender, bool dialogresult = false)
        {
            try
            {
                //Traverse the VisualTree until finding the window hosting this control.
                UIElement UIElement = (UIElement)sender;
                DependencyObject parent = VisualTreeHelper.GetParent(UIElement);
                while (parent as Window == null)
                {
                    parent = VisualTreeHelper.GetParent(parent);
                }
                //Close the window
                if (parent != null && parent is Window)
                {
                    Window w = ((Window)parent);
                    w.DialogResult = dialogresult;
                    w.Close();
                }

            }
            catch (Exception exception)
            {

                logger.Error("CloseParentWindow:" + exception.ToString());
            }
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            try
            {
                emailAdress = string.Empty;
                ContactMainTable = null;
                ContactHandler.EmailStateNotificationEvent -= ContactHandler_EmailStateNotificationEvent;
            }
            catch (Exception exception)
            {
                logger.Error("UserControl_Unloaded" + exception.ToString());
            }
        }



        private void btnAdvanceSearch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var _dataContextContact = ContactDataContext.GetInstance();
                var _configContainer = ConfigContainer.Instance();
                txtSearch.Text = string.Empty;
                if (search.Height != GridLength.Auto)
                {
                    if (_advanceSearch == null)
                    {
                        List<ComboBoxItem> DirectoryAttributesSearch = new List<ComboBoxItem>();

                        List<string> listSearchAttribute = ReadKey.ReadConfigKeys("contact.directory-search-attributes",
                                                                                  new string[] { "LastName", "FirstName", "PhoneNumber", "EmailAddress" },
                                                                                  ContactDataContext.GetInstance().ContactValidAttribute.Keys.ToList());
                        List<string> listAdvancedDefault = ReadKey.ReadConfigKeys("contact.directory-advanced-default",
                                                                                 new string[] { "LastName", "PhoneNumber" },
                                                                                 ContactDataContext.GetInstance().ContactValidAttribute.Keys.ToList());

                        listSearchAttribute.AddRange(listAdvancedDefault);
                        listSearchAttribute = listSearchAttribute.Distinct().ToList();

                        string directoryAdvancedDefault = string.Join<string>(",", listAdvancedDefault);


                        foreach (string item in listSearchAttribute.Where(x => !string.IsNullOrEmpty(x) && !string.IsNullOrWhiteSpace(x)).ToList())
                        {
                            ComboBoxItem cmbItem = new ComboBoxItem() { Content = ContactDataContext.GetInstance().ContactValidAttribute[item] };

                            if (directoryAdvancedDefault.Contains(item))
                                cmbItem.Tag = "Default";
                            DirectoryAttributesSearch.Add(cmbItem);
                        }


                        _advanceSearch = new AdvanceSearch(DirectoryAttributesSearch) { HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch };
                        _advanceSearch.eventReceivedFilteredData += new AdvanceSearch.EventReceivedFilteredData(AdvanceSearch_eventReceivedFilteredData);
                    }
                    grbAdvanceSearch.Content = _advanceSearch;
                    search.Height = GridLength.Auto;
                    AdvanceSearchImage.Source = new BitmapImage(new Uri(ConfigContainer.Instance().GetValue("image-path") + "\\Contact\\HideAdvanceSearchOption-01.png", UriKind.Relative));
                    ShowAdvanceSearchContent.Text = "Hide advance search options";
                    grdQuickSearch.IsEnabled = false;
                    brdSearchBar.Visibility = Visibility.Collapsed;
                }
                else
                {
                    grdQuickSearch.IsEnabled = true;
                    grbAdvanceSearch.Content = null;
                    search.Height = new GridLength(0);
                    //txtSearch.IsEnabled = true;
                    AdvanceSearchImage.Source = new BitmapImage(new Uri(ConfigContainer.Instance().GetValue("image-path") + "\\Contact\\DoubleArrow.png", UriKind.Relative));
                    ShowAdvanceSearchContent.Text = "Show advance search options";
                    brdSearchBar.Visibility = Visibility.Visible;
                }
                // txtSearch.IsEnabled = !txtSearch.IsEnabled;
            }
            catch (Exception generalException)
            {

                logger.Error("btnAdvanceSearch_Click: Error occurred as " + generalException.ToString());
            }
        }

        private void AdvanceSearch_eventReceivedFilteredData(List<Criteria> searchCriterias, MatchCondition matchCondition)
        {
            try
            {

                string logicalOperator = " OR ";
                if (matchCondition == MatchCondition.MatchAll)
                    logicalOperator = " AND ";
                totalItems = totalPage = currentPage = start = end = 0;
                if (ContactDataContext.GetInstance().IsContactIndexFound)
                {
                    string query = "TenantId:" + ConfigContainer.Instance().TenantDbId;
                    string subQuery = "";
                    foreach (Criteria search in searchCriterias)
                    {

                        if (Settings.ContactDataContext.GetInstance().ContactValidAttribute.Where(x => x.Value == search.Field).ToList().Count == 0) continue;
                        string attributeName = Settings.ContactDataContext.GetInstance().ContactValidAttribute.Where(x => x.Value == search.Field).Single().Key;
                        subQuery += (!string.IsNullOrEmpty(subQuery) ? logicalOperator : "") + ("(" + attributeName + ":" + search.Value + "* OR " + attributeName + ":*" + search.Value + " OR " + attributeName + ":*" + search.Value + "* OR " + attributeName + ":" + search.Value + ")");
                    }
                    if (!string.IsNullOrEmpty(subQuery))
                        query += " AND (" + subQuery + ")";
                    GetContactList(query);
                }
                else
                {
                    SearchContactUsingGetContactList(searchCriterias, matchCondition == MatchCondition.MatchAll);
                }

                //searchAttribute = null;
                BindContacts();
            }
            catch (Exception ex)
            {
                logger.Error("AdvanceSearch_eventReceivedFilteredData: Error occurred as " + ex.ToString());
            }

        }

        private void dgContactDirectory_SelectedCellsChanged(object sender, Microsoft.Windows.Controls.SelectedCellsChangedEventArgs e)
        {
            try
            {
                if (dgContactDirectory.SelectedIndex >= 0)
                {
                    SelectEmailOrLoadContactInfo();
                    if (!enableOkCancelButton)
                    {
                        if (isMergeContactView)
                        {

                        }
                        else
                        {
                            btnUnMergeContact.Visibility = btnMergeContact.Visibility = btnDeleteContact.Visibility = Visibility.Visible;
                            if (dgContactDirectory.SelectedItems != null && dgContactDirectory.SelectedItems.Count > 1)
                                txtDeleteContactToolTip.Text = "Delete selected contacts from contact directory.";
                            else
                                txtDeleteContactToolTip.Text = "Delete selected contact from contact directory.";
                            btnUnMergeContact.Visibility = btnMergeContact.Visibility = (dgContactDirectory.SelectedItems != null && dgContactDirectory.SelectedItems.Count == 1) ? Visibility.Visible : Visibility.Collapsed;
                            if (btnMergeContact.IsEnabled)
                            {
                                DataRowView dataRowView = dgContactDirectory.SelectedItem as DataRowView;
                                if (dataRowView != null)
                                {
                                    string mergeContactId = dataRowView.Row["MergeId"] as string;
                                    if (string.IsNullOrEmpty(mergeContactId))
                                    {
                                        btnUnMergeContact.Visibility = Visibility.Collapsed;
                                    }
                                }
                            }

                        }
                    }
                    else
                    {
                        btnUnMergeContact.Visibility = btnMergeContact.Visibility = btnDeleteContact.Visibility = Visibility.Collapsed;
                    }
                }

            }
            catch (Exception ex)
            {
                logger.Error("Error occurred as " + ex.Message);
            }
        }

        //This method will load the if the user select the contact row from my contact, It will show contact information (enableOkCancelButton=true).
        //If the user select the contact row from email contact directory, that time it will select the email address.
        private void SelectEmailOrLoadContactInfo()
        {
            try
            {

                DataRow row = (dgContactDirectory.SelectedItem as DataRowView).Row; //GetSelectedRow();
                if (row != null)
                {
                    if (enableOkCancelButton)
                    {
                        int emailIndex = contactDataContext.Contacts.Columns.IndexOf(Settings.ContactDataContext.GetInstance().ContactValidAttribute["EmailAddress"]);
                        if (emailIndex >= 0)
                            emailAdress = row.ItemArray[emailIndex].ToString();
                    }
                    else if (btnShowContactInfo.Visibility == Visibility.Visible)
                    {
                        int contactIdIndex = contactDataContext.Contacts.Columns.IndexOf("ContactId");
                        if (contactIdIndex == -1) return;
                        contactId = row.ItemArray[contactIdIndex].ToString();
                        LoadContact(contactId);
                    }
                }

            }
            catch (Exception ex)
            {
                logger.Error("SelectEmail() : " + ex.Message);
            }
        }

        private void btnAdvanceSearchResponse_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                totalItems = totalPage = currentPage = start = end = 0;
                GetMatchedContact();
                BindContacts();
            }
            catch (Exception generalExcption)
            {

                logger.Error("Error occurred while search contact as " + generalExcption.ToString());
            }
        }


        private void btnShowContactInfo_Click(object sender, RoutedEventArgs e)
        {
            if (isShowContactInfo)
            {
                ShowContactInformation();
                imgContactInfoShow.Source = new BitmapImage(new Uri(ConfigContainer.Instance().GetValue("image-path") + "\\Contact\\HideContactDetails.png", UriKind.Relative));
                ShowContactInfoHeading.Text = "Hide Details";
                ShowContactInfoContacent.Text = "Hide details of selected contact.";
            }
            else
            {
                HideContactInformation();
                imgContactInfoShow.Source = new BitmapImage(new Uri(ConfigContainer.Instance().GetValue("image-path") + "\\Contact\\ShowContactDetails.png", UriKind.Relative));
                ShowContactInfoHeading.Text = "Show Details";
                ShowContactInfoContacent.Text = "Show the details to selected contact.";
                if (_advanceSearch != null)
                {
                    grbAdvanceSearch.Content = null;
                    grbAdvanceSearch.Content = _advanceSearch;
                }
            }
            isShowContactInfo = !isShowContactInfo;

        }

        private void btnAddContact_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                contactId = null;
                dgContactDirectory.SelectedItem = null;
                UserControl contactInfoControl = new Controls.ContactInformation("all", contactInsertEvent) as UserControl;
                contactInfoControl.Margin = new Thickness(5);
                contactInfoControl.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                contactInfo.Children.Clear();
                contactInfo.Children.Add(contactInfoControl);
                if (!isShowContactInfo)
                    isShowContactInfo = !isShowContactInfo;
                btnShowContactInfo_Click(null, null);
            }
            catch (Exception ex)
            {
                logger.Error("btnAddContact_Click() : " + ex.Message);
            }
        }

        private void btnDeleteContact_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (contactDataContext.Contacts.Rows.Count <= 0) return;
                var selectedContact = dgContactDirectory.SelectedItems;
                if (selectedContact == null) return;
                if (selectedContact.Count <= 0) return;
                PopUpWindow popup = new PopUpWindow();
                popup.Message = "Are you sure that you want to remove " + (selectedContact.Count > 1 ? "these contacts?" : "this contact?");
                popup.ShowDialog();
                if (popup.DialogResult == true)
                {
                    foreach (DataRowView item in selectedContact)
                    {
                        DataRow row = item.Row;
                        contactId = row.ItemArray[contactDataContext.Contacts.Columns.IndexOf("ContactId")].ToString();
                        OutputValues result = Pointel.Interactions.Contact.Core.Request.RequestToDeleteContact.RequestDeleteContact(contactId);
                        if (result.IContactMessage != null & result.IContactMessage.Id == EventDelete.MessageId)
                        {
                            //Implemented changes to notify  contact Delete to Softphone bar.
                            Settings.ContactDataContext.GetInstance().NotifyContactUpdate("Delete", contactId, null);
                            DataRow[] selectRow = ContactMainTable.Select("ContactId='" + contactId + "'");
                            if (selectRow.Length > 0)
                            {
                                ContactMainTable.Rows.RemoveAt(ContactMainTable.Rows.IndexOf(selectRow[0]));
                            }
                            totalItems--;
                        }
                        else
                        {
                            txtMessage.Text = "Contact request Failed.";
                            starttimerforerror();
                            break;
                        }
                    }

                    //btnMergeContact.Visibility =
                    btnUnMergeContact.Visibility = btnMergeContact.Visibility = btnDeleteContact.Visibility = Visibility.Visible;
                    totalPage = (totalItems / itemPerPage) + (((totalItems % itemPerPage) > 0) ? 1 : 0);
                    if (end > ContactMainTable.Rows.Count)
                    {
                        end = ContactMainTable.Rows.Count;
                    }
                    if (totalItems > 0 && end == start)
                    {
                        start -= itemPerPage;
                        if (start < 0)
                            start = 0;
                    }
                    BindContacts();
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred while delete contact as as : " + ex.Message);
            }
        }

        private void cmbItemsPerPage_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                itemPerPage = Convert.ToInt32(cmbItemsPerPage.SelectedValue.ToString());
                btnFirstIndex_Click(null, null);
            }
            catch (Exception generalException)
            {

                logger.Error("Error occurred while page selection as : " + generalException.ToString());
            }
        }

        private void txtSearch_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter)
                    btnAdvanceSearchResponse_Click(null, null);
            }
            catch (Exception ex)
            {
                logger.Error("txtSearch_KeyUp() : " + ex.Message);
            }
        }

        private void btnCloseError_Click(object sender, RoutedEventArgs e)
        {
            CloseError(null, null);
        }

        private void txtMessage_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                txtMessage.Width = grdErrorColumn.ActualWidth;
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred as  " + ex.Message);
            }
        }

        void starttimerforerror()
        {
            try
            {
                grMessage.Height = GridLength.Auto;
                DispatcherTimer _timerforcloseError = new DispatcherTimer();
                _timerforcloseError.Interval = TimeSpan.FromSeconds(10);
                _timerforcloseError.Tick += new EventHandler(CloseError);
                _timerforcloseError.Start();
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred as  " + ex.Message);
            }
        }

        void CloseError(object sender, EventArgs e)
        {
            try
            {
                grMessage.Height = new GridLength(0);
                if (_timerforcloseError != null)
                {
                    _timerforcloseError.Stop();
                    _timerforcloseError.Tick -= CloseError;
                    _timerforcloseError = null;
                    txtMessage.Text = "Contact server is not active.";

                }
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred as  " + ex.Message);
            }

        }

        private void ButtonWithDropDownCall_Click(string value)
        {
            Dispatcher.BeginInvoke(
                new Action(() =>
                {
                    try
                    {
                        if (ContactDataContext.messageToClient != null)
                        {
                            ContactDataContext.messageToClient.PluginDialEvents(PluginType.Contact, value);
                            ContactDataContext.messageToClient.PluginDialEvents(PluginType.Contact, "Dial");
                        }
                        else
                        {
                            logger.Warn("Call back object is null. so we can't make call");
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Error("Error occurred as " + ex.Message);
                    }
                }));

        }

        private void ButtonWithDropDownSendEmail_Click(string value)
        {
            Dispatcher.BeginInvoke(
                  new Action(() =>
                  {
                      try
                      {
                          if (Utility.EmailUtility.IsEmailReachMaximumCount())
                          {
                              txtMessage.Text = "Email reached maximum count. Please close opened mail and then try to open.";
                              starttimerforerror();
                              return;
                          }
                          if (!string.IsNullOrEmpty(value) && Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections.ContainsKey(Pointel.Interactions.IPlugins.Plugins.Email))
                              ((IEmailPlugin)Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections[Pointel.Interactions.IPlugins.Plugins.Email]).NotifyNewEmail(value, contactId);
                      }
                      catch (Exception ex)
                      {
                          logger.Error("Error occurred as " + ex.Message);
                      }

                  }));

        }

        private void btnMergeContact_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ContactDirectoryWindow contactWindow = new ContactDirectoryWindow();
                contactWindow.ShowDialog();
                if (contactWindow.DialogResult == true)
                {
                    OutputValues result = Pointel.Interactions.Contact.Core.Request.RequestToMergeContact.MergeContact(ConfigContainer.Instance().TenantDbId, int.Parse(ConfigContainer.Instance().AgentId),
                        contactId, contactWindow.ContactId, contactWindow.Description, contactWindow.Reason);
                    if (result.MessageCode == "200")
                    {
                        txtMessage.Text = "Contact Merged successfully.";
                        btnAdvanceSearchResponse_Click(null, null);
                    }
                    else
                    {
                        txtMessage.Text = "Some problem occurred while merge contact.";
                    }
                    starttimerforerror();
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred as " + ex.Message);
            }
        }

        private void btnUnMergeContact_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OutputValues result = Pointel.Interactions.Contact.Core.Request.RequestToUnmergeContact.UNMergeContact(contactId);
                if (result.MessageCode == "200")
                {
                    txtMessage.Text = "Contact Unmerged successfully.";
                    btnAdvanceSearchResponse_Click(null, null);
                }
                else
                {
                    txtMessage.Text = "Some problem occurred while merge contact.";
                }
                starttimerforerror();
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred as " + ex.Message);
            }
        }

        public void Dispose()
        {
            logger = null;
            _advanceSearch = null;
            ContactMainTable = null;
            emailAdress = null;
            contactId = null;
            interactionID = null;
            contactDataContext = null;
            _timerforcloseError = null;
            searchAttribute = null;
            NotifySelectedEmail = null;
        }

        private void WorkbinExplorerContextMenu_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MenuItem selectedItem = sender as MenuItem;
                if (selectedItem != null)
                {
                    if (selectedItem.Icon == null)
                    {
                        if (dgContactDirectory.Columns.Where(x => x.Header.Equals(selectedItem.Header)).ToList().Count > 0)
                        {
                            selectedItem.Icon = new Image
                            {
                                Height = 15,
                                Width = 15,
                                Source =
                                    new BitmapImage(new Uri(ConfigContainer.Instance().GetValue("image-path") + "\\Contact\\TickBlack.png", UriKind.Relative))
                            };
                            dgContactDirectory.Columns.Where(x => x.Header.Equals(selectedItem.Header)).SingleOrDefault().Visibility = Visibility.Visible;
                        }

                    }
                    else
                    {
                        if (dgContactDirectory.Columns.Where(x => x.Header.Equals(selectedItem.Header)).ToList().Count > 0
                            && dgContactDirectory.Columns.Where(x => x.Visibility == Visibility.Visible).ToList().Count > 1)
                        {
                            selectedItem.Icon = null;
                            dgContactDirectory.Columns.Where(x => x.Header.Equals(selectedItem.Header)).SingleOrDefault().Visibility = Visibility.Collapsed;
                        }
                    }
                }


            }
            catch (Exception ex)
            {
                logger.Error("Error occurred as " + ex.Message);
            }

        }

        private void CreateWorkbinFieldsContextMenuItem(params string[] menuItems)
        {
            dgContactDirectory.ContextMenu.Items.Clear();
            for (int index = 0; index < menuItems.Length; index++)
            {
                var menuItem = new MenuItem();
                menuItem.Margin = new Thickness(2);
                menuItem.Header = menuItems[index];
                menuItem.Icon = new Image
                {
                    Height = 15,
                    Width = 15,
                    Source =
                        new BitmapImage(new Uri(ConfigContainer.Instance().GetValue("image-path") + "\\Contact\\TickBlack.png", UriKind.Relative))
                };
                menuItem.Click += new RoutedEventHandler(WorkbinExplorerContextMenu_Click);
                dgContactDirectory.ContextMenu.Items.Add(menuItem);
            }

        }

        private void MenuTo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MenuItem menuItem = sender as MenuItem;
                if (menuItem != null)
                {
                    if (menuItem.Header.ToString() == "Add to ToAddress")
                        btnTo_Click(null, null);
                    else
                    {
                        string tempEmail = emailAdress + ";";
                        txtTo.Text = txtTo.Text.Contains(tempEmail) ? txtTo.Text.Replace(tempEmail, "") : txtTo.Text.Replace(emailAdress, "");
                        txtTo.Text = txtTo.Text.EndsWith(";") ? txtTo.Text.TrimEnd(';') : txtTo.Text;
                    }

                }

            }
            catch (Exception ex)
            {
                logger.Error("Error occrred as " + ex.Message);
            }
        }

        private void MenuBCC_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MenuItem menuItem = sender as MenuItem;
                if (menuItem != null)
                {
                    if (menuItem.Header.ToString() == "Add to BCC")
                        btnBcc_Click(null, null);
                    else
                    {
                        string tempEmail = emailAdress + ";";
                        txtBcc.Text = txtBcc.Text.Contains(tempEmail) ? txtBcc.Text.Replace(tempEmail, "") : txtBcc.Text.Replace(emailAdress, "");
                        txtBcc.Text = txtBcc.Text.EndsWith(";") ? txtBcc.Text.TrimEnd(';') : txtBcc.Text;
                    }

                }

            }
            catch (Exception ex)
            {
                logger.Error("Error occrred as " + ex.Message);
            }
        }

        private void MenuCC_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MenuItem menuItem = sender as MenuItem;
                if (menuItem != null)
                {
                    if (menuItem.Header.ToString() == "Add to CC")
                        btnCc_Click(null, null);
                    else
                    {
                        string tempEmail = emailAdress + ";";
                        txtCc.Text = txtCc.Text.Contains(tempEmail) ? txtCc.Text.Replace(tempEmail, "") : txtCc.Text.Replace(emailAdress, "");
                        txtCc.Text = txtCc.Text.EndsWith(";") ? txtCc.Text.TrimEnd(';') : txtCc.Text;
                    }


                }
            }
            catch (Exception ex)
            {
                logger.Error("Error occrred as " + ex.Message);
            }
        }

        private void PerformMenuItemVisibility(ContextMenu menu, Visibility visibility)
        {
            (menu.Items[0] as MenuItem).Visibility = visibility;
            (menu.Items[1] as MenuItem).Visibility = visibility;
            (menu.Items[2] as MenuItem).Visibility = visibility;
        }

        private void ContextMenu_Opened(object sender, RoutedEventArgs e)
        {
            try
            {
                ContextMenu menu = sender as ContextMenu;
                if (!enableOkCancelButton)
                {
                    e.Handled = true;
                    PerformMenuItemVisibility(menu, Visibility.Collapsed);
                    return;
                }
                if (string.IsNullOrEmpty(emailAdress))
                {
                    e.Handled = true;
                    PerformMenuItemVisibility(menu, Visibility.Collapsed);
                    return;
                }
                else
                {
                    PerformMenuItemVisibility(menu, Visibility.Visible);
                    if (txtTo.Text.Contains(emailAdress))
                    {
                        (menu.Items[0] as MenuItem).Header = "Remove from ToAddress";
                        (menu.Items[0] as MenuItem).Icon = new Image
                        {
                            Height = 15,
                            Width = 15,
                            Source =
                                new BitmapImage(new Uri(ConfigContainer.Instance().GetValue("image-path") + "\\Contact\\Remove.png", UriKind.Relative))
                        };
                    }
                    else
                    {
                        (menu.Items[0] as MenuItem).Header = "Add to ToAddress";
                        (menu.Items[0] as MenuItem).Icon = new Image
                        {
                            Height = 15,
                            Width = 15,
                            Source =
                                new BitmapImage(new Uri(ConfigContainer.Instance().GetValue("image-path") + "\\Contact\\Add.png", UriKind.Relative))
                        };
                    }

                    if (txtCc.Text.Contains(emailAdress))
                    {
                        (menu.Items[1] as MenuItem).Header = "Remove from CC";
                        (menu.Items[1] as MenuItem).Icon = new Image
                        {
                            Height = 15,
                            Width = 15,
                            Source =
                                new BitmapImage(new Uri(ConfigContainer.Instance().GetValue("image-path") + "\\Contact\\Remove.png", UriKind.Relative))
                        };
                    }
                    else
                    {
                        (menu.Items[1] as MenuItem).Header = "Add to CC";
                        (menu.Items[1] as MenuItem).Icon = new Image
                        {
                            Height = 15,
                            Width = 15,
                            Source =
                                new BitmapImage(new Uri(ConfigContainer.Instance().GetValue("image-path") + "\\Contact\\Add.png", UriKind.Relative))
                        };
                    }

                    if (txtBcc.Text.Contains(emailAdress))
                    {
                        (menu.Items[2] as MenuItem).Header = "Remove from BCC";
                        (menu.Items[2] as MenuItem).Icon = new Image
                        {
                            Height = 15,
                            Width = 15,
                            Source =
                                new BitmapImage(new Uri(ConfigContainer.Instance().GetValue("image-path") + "\\Contact\\Remove.png", UriKind.Relative))
                        };
                    }
                    else
                    {
                        (menu.Items[2] as MenuItem).Header = "Add to BCC";
                        (menu.Items[2] as MenuItem).Icon = new Image
                        {
                            Height = 15,
                            Width = 15,
                            Source =
                                new BitmapImage(new Uri(ConfigContainer.Instance().GetValue("image-path") + "\\Contact\\Add.png", UriKind.Relative))
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error occrred as " + ex.Message);
            }
        }


        private void ShowLoading()
        {
            Dispatcher.BeginInvoke(new Action(delegate()
            {
                BrdOpacity.Visibility = System.Windows.Visibility.Visible;
                gridPreload.Visibility = System.Windows.Visibility.Visible;
                Panel.SetZIndex(BrdOpacity, 1);
                Panel.SetZIndex(gridPreload, 2);

            }));
        }

        private void HideLoading()
        {
            Dispatcher.BeginInvoke(new Action(delegate()
            {
                BrdOpacity.Visibility = System.Windows.Visibility.Collapsed;
                gridPreload.Visibility = System.Windows.Visibility.Collapsed;
                Panel.SetZIndex(BrdOpacity, 0);
                Panel.SetZIndex(gridPreload, 0);
            }));
        }
    }
}
